// <copyright file="TableUtils.cs" company="None">
//    <para>
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at</para>
//    <para>
//    http://www.apache.org/licenses/LICENSE-2.0
//    </para><para>
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.</para>
//    <para>
//    Original Java code from Snmp4J Copyright (C) 2003-2016 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Util
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    class TableUtils : AbstractSnmpUtility
    {
        private static readonly log4net.ILog log = log4net.LogManager
          .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // RowStatus TC enumerated values
        public static readonly int ROWSTATUS_ACTIVE = 1;
        public static readonly int ROWSTATUS_NOTINSERVICE = 2;
        public static readonly int ROWSTATUS_NOTREADY = 3;
        public static readonly int ROWSTATUS_CREATEANDGO = 4;
        public static readonly int ROWSTATUS_CREATEANDWAIT = 5;
        public static readonly int ROWSTATUS_DESTROY = 6;

        private int maxNumOfRowsPerPDU = 10;
        private int maxNumColumnsPerPDU = 10;

        /**
         * Creates a <code>TableUtils</code> instance. The created instance is thread
         * safe as long as the supplied <code>Session</code> and <code>PDUFactory</code>
         * are thread safe.
         *
         * @param snmpSession
         *    a SNMP <code>Session</code> instance.
         * @param pduFactory
         *    a <code>PDUFactory</code> instance that creates the PDU that are used
         *    by this instance to retrieve table data using GETBULK/GETNEXT
         *    operations.
         */
        public TableUtils(ISession snmpSession, IPDUFactory pduFactory)
            : base(snmpSession, pduFactory)
        {
        }

        /**
         * Gets synchronously SNMP tabular data from one or more tables.
         * The data is returned row-by-row as a list of {@link TableEvent} instances.
         * Each instance represents a row (or an error condition). Besides the
         * target agent, the OIDs of the columnar objects have to be specified
         * for which instances should be retrieved. With a lower bound index and
         * an upper bound index, the result set can be narrowed to improve
         * performance. This method can be executed concurrently by multiple threads.
         *
         * @param target
         *    a <code>Target</code> instance.
         * @param columnOIDs
         *    an array of OIDs of the columnar objects whose instances should be
         *    retrieved. The columnar objects may belong to different tables.
         *    Typically they belong to tables that share a common index or sub-index
         *    prefix. Note: The result of this method is not defined if instance OIDs
         *    are supplied in this array!
         * @param lowerBoundIndex
         *    an optional parameter that specifies the lower bound index.
         *    If not <code>null</code>, all returned rows have an index greater than
         *    <code>lowerBoundIndex</code>.
         * @param upperBoundIndex
         *    an optional parameter that specifies the upper bound index.
         *    If not <code>null</code>, all returned rows have an index less or equal
         *    than <code>upperBoundIndex</code>.
         * @return
         *    a <code>List</code> of {@link TableEvent} instances. Each instance
         *    represents successfully retrieved row or an error condition. Error
         *    conditions (any status other than {@link TableEvent#STATUS_OK})
         *    may only appear at the last element of the list.
         */
        public IList<TableEvent> GetTable(ITarget target,
                             OID[] columnOIDs,
                             OID lowerBoundIndex,
                             OID upperBoundIndex)
        {

            if ((columnOIDs == null) || (columnOIDs.Length == 0))
            {
                throw new ArgumentException("No column OIDs specified");
            }

            InternalTableListener listener = new InternalTableListener();
            TableRequest req = CreateTableRequest(
                target,
                columnOIDs, 
                listener,
                null,
                lowerBoundIndex,
                upperBoundIndex);

            lock(listener) {
                if (req.SendNextChunk())
                {
                    try
                    {
                        while (!listener.Finished())
                        {
                            listener.wait();
                        }
                    }
                    catch (InterruptedException ex)
                    {
                        Thread.currentThread().interrupt();
                    }
                }
            }

            return listener.Rows;
        }

        protected TableRequest CreateTableRequest(ITarget target, OID[] columnOIDs,
                                                  TableListener listener,
                                                  Object userObject,
                                                  OID lowerBoundIndex,
                                                  OID upperBoundIndex)
        {
            return new TableRequest(target, columnOIDs, listener,
                                    userObject, lowerBoundIndex, upperBoundIndex, this);
        }

        /**
         * Gets SNMP tabular data from one or more tables. The data is returned
         * asynchronously row-by-row through a supplied callback. Besides the
         * target agent, the OIDs of the columnar objects have to be specified
         * for which instances should be retrieved. With a lower bound index and
         * an upper bound index, the result set can be narrowed to improve
         * performance.
         * <p>
         * This method may call the {@link TableListener#finished} method before
         * it returns. If you want to synchronize the main thread with the
         * finishing of the table retrieval, follow this pattern:
         * <pre>
         *      synchronized (this) {
         *         TableListener myListener = ... {
         *            private boolean finished;
         *
         *            public boolean isFinished() {
         *              return finished;
         *            }
         *
         *            public void finished(TableEvent event) {
         *               ..
         *               finished = true;
         *               synchronized (event.getUserObject()) {
         *                  event.getUserObject().notify();
         *               }
         *            }
         *         };
         *         tableUtil.getTable(..,..,myListener,this,..,..);
         *         while (!myListener.isFinished()) {
         *           wait();
         *         }
         *      }
         * </pre>
         *
         * @param target
         *    a <code>Target</code> instance.
         * @param columnOIDs
         *    an array of OIDs of the columnar objects whose instances should be
         *    retrieved. The columnar objects may belong to different tables.
         *    Typically they belong to tables that share a common index or sub-index
         *    prefix. Note: The result of this method is not defined if instance OIDs
         *    are supplied in this array!
         * @param listener
         *    a <code>TableListener</code> that is called with {@link TableEvent}
         *    objects when an error occured, new rows have been retrieved, or when
         *    the table has been retrieved completely.
         * @param userObject
         *    an user object that is transparently supplied to the above call back.
         * @param lowerBoundIndex
         *    an optional parameter that specifies the lower bound index.
         *    If not <code>null</code>, all returned rows have an index greater than
         *    <code>lowerBoundIndex</code>.
         * @param upperBoundIndex
         *    an optional parameter that specifies the upper bound index.
         *    If not <code>null</code>, all returned rows have an index less or equal
         *    than <code>upperBoundIndex</code>.
         */
        public void GetTable(ITarget target,
                             OID[] columnOIDs,
                             TableListener listener,
                             Object userObject,
                             OID lowerBoundIndex,
                             OID upperBoundIndex)
        {
            if ((columnOIDs == null) || (columnOIDs.Length == 0))
            {
                throw new ArgumentException("No column OIDs specified");
            }

            TableRequest req = new TableRequest(
                target,
                columnOIDs,
                listener,
                userObject,
                lowerBoundIndex,
                upperBoundIndex,
                this);
            req.SendNextChunk();
        }

        /**
         * Gets SNMP tabular data from one or more tables. The data is returned
         * asynchronously row-by-row through a supplied callback. Besides the
         * target agent, the OIDs of the columnar objects have to be specified
         * for which instances should be retrieved. With a lower bound index and
         * an upper bound index, the result set can be narrowed to improve
         * performance.
         * <p>
         * This implementation must not be used with sparese tables, because it
         * is optimized for dense tables and will not return correct results for
         * sparse tables.
         * </p>
         *
         * @param target
         *    a <code>Target</code> instance.
         * @param columnOIDs
         *    an array of OIDs of the columnar objects whose instances should be
         *    retrieved. The columnar objects may belong to different tables.
         *    Typically they belong to tables that share a common index or sub-index
         *    prefix. Note: The result of this method is not defined if instance OIDs
         *    are supplied in this array!
         * @param listener
         *    a <code>TableListener</code> that is called with {@link TableEvent}
         *    objects when an error occurred, new rows have been retrieved, or when
         *    the table has been retrieved completely.
         * @param userObject
         *    an user object that is transparently supplied to the above call back.
         * @param lowerBoundIndex
         *    an optional parameter that specifies the lower bound index.
         *    If not <code>null</code>, all returned rows have an index greater than
         *    <code>lowerBoundIndex</code>.
         * @param upperBoundIndex
         *    an optional parameter that specifies the upper bound index.
         *    If not <code>null</code>, all returned rows have an index less or equal
         *    than <code>lowerBoundIndex</code>.
         * @since 1.5
         */
        public void GetDenseTable(ITarget target,
                                  OID[] columnOIDs,
                                  TableListener listener,
                                  Object userObject,
                                  OID lowerBoundIndex,
                                  OID upperBoundIndex)
        {
            if ((columnOIDs == null) || (columnOIDs.Length == 0))
            {
                throw new ArgumentException("No column OIDs specified");
            }

            TableRequest req = new TableRequest(
                target,
                columnOIDs,
                listener,
                userObject,
                lowerBoundIndex,
                upperBoundIndex,
                this);
            req.SendNextChunk();
        }

        /**
         * Gets or sets the maximum number of rows that will be retrieved per SNMP GETBULK
         * request.
         *
         * @return
         *    an integer greater than zero that specifies the maximum number of rows
         *    to retrieve per SNMP GETBULK operation.
         */


        /**
         * Sets the maximum number of rows that will be retrieved per SNMP GETBULK
         * request. The default is 10.
         *
         * @param numberOfRowsPerChunk
         *    an integer greater than zero that specifies the maximum number of rows
         *    to retrieve per SNMP GETBULK operation.
         */

        public int MaxNumRowsPerPDU
        {
            get
            {
                return maxNumOfRowsPerPDU;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException(
                        "The number of rows per PDU must be > 0");
                }

                this.maxNumOfRowsPerPDU = value;
            }
            
        }

        /**
         * Gets the maximum number of columns that will be retrieved per SNMP GETNEXT
         * or GETBULK request.
         *
         * @return
         *    an integer greater than zero that specifies the maximum columns of rows
         *    to retrieve per SNMP GETNEXT or GETBULK operation.
         */

        /**
         * Sets the maximum number of columns that will be retrieved per SNMP GETNEXT
         * or GETBULK request. The default is 10.
         *
         * @param numberOfColumnsPerChunk
         *    an integer greater than zero that specifies the maximum columns of rows
         *    to retrieve per SNMP GETNEXT or GETBULK operation.
         */
        public int MaxNumColumnsPerPDU
        {
            get
            {
                return maxNumColumnsPerPDU;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException(
                        "The number of columns per PDU must be > 0");
                }

                this.maxNumColumnsPerPDU = value;
            }
        }

        

        

  

        class InternalTableListener : TableListener
        {

            private IList<TableEvent> rows = new LinkedList<TableEvent>();
            private volatile bool finished = false;

            public bool Next(TableEvent evt)
            {
                rows.Add(evt);
                return true;
            }

            //synchronized
            public void Finished(TableEvent evt)
            {
                if ((evt.getStatus() != TableEvent.STATUS_OK) ||
                    (evt.getIndex() != null))
                {
                    rows.Add(evt);
                }

                finished = true;
                Notify();
            }

            public IList<TableEvent> Rows
            {
                get
                {
                    return rows;
                }
            }

            public bool Finished()
            {
                return finished;
            }
        }
    }
}
