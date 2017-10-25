

namespace JunoSnmp.Util.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.SMI;
    using JunoSnmp.Util;

    public class TableRequest
    {

        public event ResponseHandler OnResponse;

        ITarget target;
        protected OID[] columnOIDs;
        TableListener listener;
        Object userObject;
        protected OID lowerBoundIndex;
        protected OID upperBoundIndex;
        protected readonly TableUtils util;
        private int sent = 0;
        private readonly bool anyMatch = false;
        private List<OID> lastSent = null;
        private readonly LinkedList<Row> rowCache = new LinkedList<Row>();
        protected List<OID> lastReceived;

        volatile bool finished = false;

        public TableRequest(ITarget target,
                            OID[] columnOIDs,
                            TableListener listener,
                            Object userObject,
                            OID lowerBoundIndex,
                            OID upperBoundIndex,
                            TableUtils util)
        {
            this.target = target;
            this.columnOIDs = columnOIDs;
            this.listener = listener;
            this.userObject = userObject;
            this.lastReceived = new List<OID>(Arrays.asList(columnOIDs));
            this.upperBoundIndex = upperBoundIndex;
            this.lowerBoundIndex = lowerBoundIndex;
            this.util = util;
            if (lowerBoundIndex != null)
            {
                for (int i = 0; i < lastReceived.Count; i++)
                {
                    OID oid = new OID(lastReceived[i]);
                    oid.Append(lowerBoundIndex);
                    lastReceived[i] = oid;
                }
            }
        }

        public bool SendNextChunk()
        {
            if (sent >= lastReceived.Count)
            {
                return false;
            }

            PDU pdu = util.pduFactory.CreatePDU(target);
            if (target.Version == SnmpConstants.version1)
            {
                pdu.Type = PDU.GETNEXT;
            }
            else if (pdu.Type != PDU.GETNEXT)
            {
                pdu.Type = PDU.GETBULK;
            }
            int sz = Math.Min(lastReceived.Count - sent, util.maxNumColumnsPerPDU);
            if (pdu.Type == PDU.GETBULK)
            {
                if (util.maxNumOfRowsPerPDU > 0)
                {
                    pdu.MaxRepetitions = util.maxNumOfRowsPerPDU;
                    pdu.NonRepeaters = 0;
                }
                else
                {
                    pdu.NonRepeaters = sz;
                    pdu.MaxRepetitions = 0;
                }
            }

            lastSent = new List<OID>(sz + 1);
            List<int> sentColumns = new List<int>(sz);
            int chunkSize = 0;
            for (int i = sent; i < sent + sz; i++)
            {
                OID col = lastReceived[i];
                // only sent columns that are not complete yet
                if (col.StartsWith(columnOIDs[i]))
                {
                    VariableBinding vb = new VariableBinding(col);
                    pdu.Add(vb);
                    if (pdu.BERLength > target.MaxSizeRequestPDU)
                    {
                        pdu.Trim();
                        break;
                    }
                    else
                    {
                        lastSent.Add(lastReceived[i]);
                        chunkSize++;
                    }

                    sentColumns.Add(i);
                }
                else
                {
                    chunkSize++;
                }
            }

            try
            {
                sent += chunkSize;
                if (pdu.Count == 0)
                {
                    return false;
                }

                SendRequest(pdu, target, sentColumns);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled)
                {
                    log.Debug(ex.StackTrace);
                }

                listener.finished(new TableEvent(this, userObject, ex));
                return false;
            }

            return true;
        }

        protected void SendRequest(PDU pdu, ITarget target, List<int> sendColumns)
        {
            util.session.Send(pdu, target, sendColumns, this);
        }

        public void OnResponse(ResponseEvent evt)
        {
            // Do not forget to cancel the asynchronous request! ;-)
            util.session.Cancel(evt.getRequest(), this);
            if (finished)
            {
                return;
            }

            lock (this)
            {
                if (CheckResponse(evt))
                {
                    bool anyMatchInChunk = false;
                    List<int> colsOfRequest = (List<int>)evt.getUserObject();
                    PDU request = evt.getRequest();
                    PDU response = evt.getResponse();
                    int cols = request.Count;
                    int rows = response.Count / cols;
                    OID lastMinIndex = null;

                    for (int r = 0; r < rows; r++)
                    {
                        Row row = null;
                        anyMatchInChunk = false;
                        for (int c = 0; c < request.Count; c++)
                        {
                            int pos = colsOfRequest[c];
                            VariableBinding vb = response[r * cols + c];
                            if (vb.IsException)
                            {
                                continue;
                            }

                            OID id = vb.Oid;
                            OID col = columnOIDs[pos];
                            if (id.StartsWith(col))
                            {
                                OID index =
                                    new OID(id.GetValue(), col.Size, id.Size - col.Size);
                                if ((upperBoundIndex != null) &&
                                    (index.CompareTo(upperBoundIndex) > 0))
                                {
                                    continue;
                                }

                                if ((lastMinIndex == null) ||
                                    (index.CompareTo(lastMinIndex) < 0))
                                {
                                    lastMinIndex = index;
                                }

                                anyMatchInChunk = true;

                                if ((row == null) || (!row.RowIndex.Equals(index)))
                                {
                                    row = null;
                                    foreach (Row lastrow in rowCache.Reverse())
                                    {
                                        int compareResult = index.CompareTo(lastrow.RowIndex);
                                        if (compareResult == 0)
                                        {
                                            row = lastrow;
                                            break;
                                        }
                                        else if (compareResult > 0)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (row == null)
                                {
                                    row = new Row(index);
                                    if (rowCache.Count == 0)
                                    {
                                        rowCache.AddLast(row);
                                    }
                                    else if (rowCache.First.Value.RowIndex.CompareTo(index) >= 0)
                                    {
                                        rowCache.AddFirst(row);
                                    }
                                    else
                                    {
                                        for (ListIterator<Row> it = rowCache.listIterator(rowCache.size());
                                             it.hasPrevious();)
                                        {
                                            Row lastRow = it.previous();
                                            if (index.CompareTo(lastRow.RowIndex) >= 0)
                                            {
                                                it.set(row);
                                                it.add(lastRow);
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (((!row.setNumComplete(pos)) ||
                                     (row.Count > pos)) && (row[pos] != null))
                                {
                                    finished = true;
                                    listener.finished(new TableEvent(this, userObject,
                                        TableEvent.STATUS_WRONG_ORDER));
                                    return;
                                }

                                row.NumComplete = pos;
                                if (pos < row.NumComplete)
                                {
                                    row[pos] = vb;
                                }
                                else
                                {
                                    row.Add(vb);
                                }

                                lastReceived[pos] = vb.Oid;
                            }
                            else
                            {
                                lastReceived[pos] = vb.Oid;
                            }
                        }
                    }

                    anyMatch |= anyMatchInChunk;
                    while ((rowCache.Count > 0)
                        && (rowCache.First.Value.NumComplete == columnOIDs.Length)
                        && ((lastMinIndex == null) || (rowCache.First.Value.RowIndex.CompareTo(lastMinIndex) < 0)))
                    {
                        if (!listener.next(getTableEvent()))
                        {
                            finished = true;
                            listener.finished(new TableEvent(this, userObject));
                            return;
                        }
                    }

                    if (!SendNextChunk())
                    {
                        if (anyMatch)
                        {
                            sent = 0;
                            anyMatch = false;
                            SendNextChunk();
                        }
                        else
                        {
                            EmptyCache();
                            finished = true;
                            listener.finished(new TableEvent(this, userObject));
                        }
                    }
                }
            }
        }

        protected bool CheckResponse(ResponseEvent evt)
        {
            if (evt.getError() != null)
            {
                finished = true;
                EmptyCache();
                listener.finished(new TableEvent(this, userObject, evt.getError()));
            }
            else if (evt.getResponse() == null)
            {
                finished = true;
                // timeout
                EmptyCache();
                listener.finished(new TableEvent(this, userObject,
                                                 TableEvent.STATUS_TIMEOUT));
            }
            else if (evt.getResponse().getType() == PDU.REPORT)
            {
                finished = true;
                EmptyCache();
                listener.finished(new TableEvent(this, userObject,
                                     evt.getResponse()));
            }
            else if (evt.getResponse().getErrorStatus() != PDU.noError)
            {
                finished = true;
                EmptyCache();
                listener.finished(new TableEvent(this, userObject,
                                                 evt.getResponse().getErrorStatus()));
            }
            else
            {
                return true;
            }

            return false;
        }

        private void EmptyCache()
        {
            while (rowCache.Count > 0)
            {
                if (!listener.next(getTableEvent()))
                {
                    break;
                }
            }
        }

        private TableEvent GetTableEvent()
        {
            Row r = rowCache.First();
            rowCache.RemoveFirst();
            r.NumComplete = columnOIDs.Length;
            VariableBinding[] vbs = new VariableBinding[r.Count];
            r.CopyInto(vbs);
            return new TableEvent(this, userObject, r.RowIndex, vbs);
        }

        public Row this[OID index]
        {
            get
            {
                foreach (Row r in rowCache.Reverse())
                {
                    if (index.Equals(r.RowIndex))
                    {
                        return r;
                    }
                }

                return null;
            }
        }
    }
}
