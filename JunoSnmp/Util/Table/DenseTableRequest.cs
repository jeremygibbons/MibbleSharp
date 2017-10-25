using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunoSnmp.Util.Table
{
    /**
         * The <code>DenseTableRequest</code> extends TableRequest to implement a
         * faster table retrieval than the original. Caution:
         * This version does not correctly retrieve sparse tables!
         *
         * @author Frank Fock
         * @since 1.5
         */
    class DenseTableRequest : TableRequest
    {
        protected DenseTableRequest(ITarget target,
                                    OID[] columnOIDs,
                                    TableListener listener,
                                    Object userObject,
                                    OID lowerBoundIndex,
                                    OID upperBoundIndex,
                                    TableUtils util)
            : base(target, columnOIDs, listener, userObject, lowerBoundIndex,
                  upperBoundIndex, util)
        {

        }

        //syncronized
        public void OnResponse(ResponseEvent evt)
        {
            // Do not forget to cancel the asynchronous request! ;-)
            this.util.session.Cancel(evt.getRequest(), this);
            if (finished)
            {
                return;
            }
            if (CheckResponse(evt))
            {
                int startCol = (int)evt.getUserObject();
                PDU request = evt.getRequest();
                PDU response = evt.getResponse();
                int cols = request.Count;
                int rows = response.Count / cols;
                OID lastMinIndex = null;
                for (int r = 0; r < rows; r++)
                {
                    Row row = null;
                    for (int c = 0; c < request.Count; c++)
                    {
                        int pos = startCol + c;
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

                            if (row == null)
                            {
                                row = new Row(index);
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
                    }

                    if (row != null)
                    {
                        if (!listener.next(new TableEvent(this, userObject, row.getRowIndex(),
                                                          row.toArray(new VariableBinding[row.size()]))))
                        {
                            finished = true;
                            listener.finished(new TableEvent(this, userObject));
                            return;
                        }
                    }
                }

                if (!SendNextChunk())
                {
                    finished = true;
                    listener.finished(new TableEvent(this, userObject));
                }
            }
        }
    }

    /**
     * Creates a SNMP table row for a table that supports the RowStatus
     * mechanism for row creation.
     * @param target
     *    the Target SNMP entity for the operation.
     * @param rowStatusColumnOID
     *    the column OID of the RowStatus column (without any instance identifier).
     * @param rowIndex
     *    the OID denoting the index of the table row to create.
     * @param values
     *    the values of columns to set in the row. If <code>values</code> is
     *    <code>null</code> the row is created via the tripple mode row creation
     *    mechanism (RowStatus is set to createAndWait). Otherwise, each variable
     *    binding has to contain the OID of the columnar object ID (without any
     *    instance identifier) and its value. On return, the variable bindings
     *    will be modified so that the variable binding OIDs will contain the
     *    instance OIDs of the respective columns (thus column OID + rowIndex).
     * @return ResponseEvent
     *    the ResponseEvent instance returned by the SNMP session on response
     *    of the internally sent SET request. If <code>null</code>, an IO
     *    exception has occurred. Otherwise, if the response PDU is
     *    <code>null</code> a timeout has occurred. Otherwise, check the error
     *    status for {@link SnmpConstants#SNMP_ERROR_SUCCESS} to verify that the
     *    row creation was successful.
     * @since 1.6
     */
    public ResponseEvent CreateRow(ITarget target,
                                   OID rowStatusColumnOID, OID rowIndex,
                                   VariableBinding[] values)
    {
        PDU pdu = pduFactory.CreatePDU(target);
        OID rowStatusID = new OID(rowStatusColumnOID);
        rowStatusID.Append(rowIndex);
        VariableBinding rowStatus = new VariableBinding(rowStatusID);
        if (values != null)
        {
            // one-shot mode
            rowStatus.Variable = new Integer32(ROWSTATUS_CREATEANDGO);
        }
        else
        {
            rowStatus.Variable = new Integer32(ROWSTATUS_CREATEANDWAIT);
        }

        pdu.Add(rowStatus);

        if (values != null)
        {
            // append index to all columnar values
            foreach (VariableBinding value in values)
            {
                OID columnOID = new OID(value.Oid);
                columnOID.Append(rowIndex);
                value.Oid = columnOID;
            }

            pdu.AddAll(values);
        }

        pdu.Type = PDU.SET;

        try
        {
            return session.Send(pdu, target);
        }
        catch (IOException ex)
        {
            log.Error(ex);
        }

        return null;
    }

    /**
     * Destroys a SNMP table row from a table that support the RowStatus
     * mechanism for row creation/deletion.
     * @param target
     *    the Target SNMP entity for the operation.
     * @param rowStatusColumnOID
     *    the column OID of the RowStatus column (without any instance identifier).
     * @param rowIndex
     *    the OID denoting the index of the table row to destroy.
     * @return ResponseEvent
     *    the ResponseEvent instance returned by the SNMP session on response
     *    of the internally sent SET request. If <code>null</code>, an IO
     *    exception has occurred. Otherwise, if the response PDU is
     *    <code>null</code> a timeout has occured, Otherwise, check the error
     *    status for {@link SnmpConstants#SNMP_ERROR_SUCCESS} to verify that the
     *    row creation was successful.
     * @since 1.7.6
     */
    public ResponseEvent DestroyRow(ITarget target,
                                    OID rowStatusColumnOID, OID rowIndex)
    {
        PDU pdu = pduFactory.CreatePDU(target);
        OID rowStatusID = new OID(rowStatusColumnOID);
        rowStatusID.Append(rowIndex);
        VariableBinding rowStatus = new VariableBinding(rowStatusID);
        rowStatus.Variable = new Integer32(ROWSTATUS_DESTROY);
        pdu.Add(rowStatus);
        pdu.Type = PDU.SET;
        try
        {
            ResponseEvent responseEvent = session.Send(pdu, target);
            return responseEvent;
        }
        catch (IOException ex)
        {
            logger.error(ex);
        }
        return null;
    }
}
