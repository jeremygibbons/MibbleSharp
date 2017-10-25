// <copyright file="TreeUtils.cs" company="None">
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
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Util.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    public class TreeUtils : AbstractSnmpUtility
    {

        private static readonly log4net.ILog log = log4net.LogManager
                .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int maxRepetitions = 10;
        private bool ignoreLexicographicOrder;

        public delegate void TreeEventHandler(object o, TreeEventArgs args);

        public event TreeEventHandler OnNext;
        public event TreeEventHandler OnFinished;

        /**
         * Creates a <code>TreeUtils</code> instance. The created instance is thread
         * safe as long as the supplied <code>Session</code> and
         * <code>PDUFactory</code> are thread safe.
         *
         * @param snmpSession
         *    a SNMP <code>Session</code> instance.
         * @param pduFactory
         *    a <code>PDUFactory</code> instance that creates the PDU that are used
         *    by this instance to retrieve MIB tree data using GETBULK/GETNEXT
         *    operations.
         */
        public TreeUtils(ISession snmpSession, IPDUFactory pduFactory)
                : base(snmpSession, pduFactory)
        {
        }

        /**
         * Gets a subtree with GETNEXT (SNMPv1) or GETBULK (SNMP2c, SNMPv3) operations
         * from the specified target synchronously.
         *
         * @param target
         *    a <code>Target</code> that specifies the target command responder
         *    including its network transport address.
         * @param rootOID
         *    the OID that specifies the root of the sub-tree to retrieve
         *    (not included).
         * @return
         *    a possibly empty List of <code>TreeEvent</code> instances where each
         *    instance carries zero or more values (or an error condition)
         *    in depth-first-order.
         */
        public List<TreeEventArgs> GetSubtree(ITarget target, OID rootOID)
        {
            IList<TreeEventArgs> l = new List<TreeEventArgs>();
            InternalTreeListener listener = new InternalTreeListener(l, this);
            lock (listener)
            {
                OID[] rootOIDs = new OID[] { rootOID };
                Walk(target, rootOIDs, null, listener);
                try
                {
                    listener.Wait();
                }
                catch (InterruptedException ex)
                {
                    log.Warn("Tree retrieval interrupted: " + ex.getMessage());
                }
            }

            return l.ToList();
        }

        /**
         * Walks a subtree with GETNEXT (SNMPv1) or GETBULK (SNMP2c, SNMPv3) operations
         * from the specified target asynchronously.
         *
         * @param target
         *    a <code>Target</code> that specifies the target command responder
         *    including its network transport address.
         * @param rootOIDs
         *    the OIDs which specify the subtrees to walk. Each OID defines a sub-tree
         *    that is walked. The walk ends if (a) an SNMP error occurs, (b) all
         *    returned variable bindings for an iteration contain an exception value
         *    (i.e., {@link Null#endOfMibView}) or for each rootOIDs element, the returned
         *    VariableBinding's OID has not the same prefix, (c) a VariableBinding out of
         *    lexicographic order is returned.
         * @return
         *    a possibly empty List of <code>TreeEvent</code> instances where each
         *    instance carries zero or <code>rootOIDs.length</code> values.
         * @since 2.1
         */
        public List<TreeEventArgs> Walk(ITarget target, OID[] rootOIDs)
        {
            LinkedList<TreeEventArgs> l = new LinkedList<TreeEventArgs>();
            TreeListener listener = new InternalTreeListener(l);

            lock (listener)
            {
                Walk(target, rootOIDs, null, listener);
                try
                {
                    listener.wait();
                }
                catch (InterruptedException ex)
                {
                    log.Warn("Tree retrieval interrupted: " + ex.getMessage());
                }
            }

            return l.ToList();
        }


        /**
         * Gets a subtree with GETNEXT (SNMPv1) or GETBULK (SNMP2c, SNMPv3) operations
         * from the specified target asynchronously.
         *
         * @param target
         *    a <code>Target</code> that specifies the target command responder
         *    including its network transport address.
         * @param rootOID
         *    the OID that specifies the root of the sub-tree to retrieve
         *    (not included).
         * @param userObject
         *    an optional user object that will be transparently handed over to the
         *    supplied <code>TreeListener</code>.
         * @param listener
         *    the <code>TreeListener</code> that processes the {@link TreeEvent}s
         *    generated by this method. Each event object may carry zero or more
         *    object instances from the sub-tree in depth-first-order.
         */
        public void GetSubtree(ITarget target, OID rootOID,
                               Object userObject, TreeListener listener)
        {
            Walk(target, new OID[] { rootOID }, userObject, listener);
        }

        /**
         * Walks a subtree with GETNEXT (SNMPv1) or GETBULK (SNMP2c, SNMPv3) operations
         * from the specified target asynchronously.
         *
         * @param target
         *    a <code>Target</code> that specifies the target command responder
         *    including its network transport address.
         * @param rootOIDs
         *    the OIDs which specify the subtrees to walk. Each OID defines a sub-tree
         *    that is walked. The walk ends if (a) an SNMP error occurs, (b) all 
         *    returned variable bindings for an iteration contain an exception value
         *    (i.e., {@link Null#endOfMibView}) or for each rootOIDs element, the returned
         *    VariableBinding's OID has not the same prefix, (c) a VariableBinding out of
         *    lexicographic order is returned.
         * @param userObject
         *    an optional user object that will be transparently handed over to the
         *    supplied <code>TreeListener</code>.
         * @param listener
         *    the <code>TreeListener</code> that processes the {@link TreeEvent}s
         *    generated by this method. Each event object may carry zero or more
         *    object instances from the sub-tree in depth-first-order if rootOIDs
         *    has a single element. If it has more than one element, then each
         *    {@link TreeEvent} contains the variable bindings of each iteration.
         * @since 2.1
         */
        public void Walk(ITarget target, OID[] rootOIDs,
                         Object userObject, TreeListener listener)
        {
            PDU request = pduFactory.CreatePDU(target);

            foreach (OID oid in rootOIDs)
            {
                request.Add(new VariableBinding(oid));
            }

            if (target.Version == SnmpConstants.version1)
            {
                request.Type = PDU.GETNEXT;
            }
            else if (request.Type != PDU.GETNEXT)
            {
                request.Type = PDU.GETBULK;
                request.MaxRepetitions = maxRepetitions;
            }

            TreeRequest treeRequest =
                new TreeRequest(listener, rootOIDs, target, userObject, request);

            treeRequest.Send();
        }

        /**
         * Gets the maximum number of the variable bindings per <code>TreeEvent</code>
         * returned by this instance.
         * @return
         *    the maximum repetitions used for GETBULK requests. For SNMPv1 this
         *    values has no effect (it is then implicitly one).
         */

        /**
         * Sets the maximum number of the variable bindings per <code>TreeEvent</code>
         * returned by this instance.
         * @param maxRepetitions
         *    the maximum repetitions used for GETBULK requests. For SNMPv1 this
         *    values has no effect (it is then implicitly one).
         */
        public int MaxRepetitions
        {
            get
            {
                return maxRepetitions;
            }

            set
            {
                this.maxRepetitions = value;
            }
        }

        /**
         * Return the ignore lexicographic order errors flage value.
         * @return
         *    <code>true</code> if lexicographic order errors are ignored,
         *    <code>false</code> otherwise (default).
         * @since 1.10.1
         */

        /**
         * Set the ignore lexicographic order errors flage value.
         * @param ignoreLexicographicOrder
         *    <code>true</code> to ignore lexicographic order errors,
         *    <code>false</code> otherwise (default).
         * @since 1.10.1
         */
        public bool IgnoreLexicographicOrder
        {
            get
            {
                return ignoreLexicographicOrder;
            }

            set
            {
                this.ignoreLexicographicOrder = value;
            }
        }

        class TreeRequest : ResponseListener
        {

            private TreeListener listener;
            private Object userObject;
            private PDU request;
            private OID[] rootOIDs;
            private ITarget target;
            private readonly TreeUtils utils;

            public TreeRequest(TreeListener listener, OID[] rootOIDs, ITarget target,
                               Object userObject, PDU request, TreeUtils utils)
            {
                this.listener = listener;
                this.userObject = userObject;
                this.request = request;
                this.rootOIDs = rootOIDs;
                this.target = target;
                this.utils = utils;
            }

            public void Send()
            {
                try
                {
                    utils.session.Send(request, target, null, this);
                }
                catch (IOException iox)
                {
                    listener.finished(new TreeEventArgs(userObject, iox));
                }
            }

            public void onResponse(ResponseEventArgs evt)
            {
                utils.session.Cancel(evt.Request, this);
                PDU respPDU = evt.Response;
                if (respPDU == null)
                {
                    listener.finished(new TreeEventArgs(userObject,
                                                    RetrievalEventArgs.STATUS_TIMEOUT));
                }
                else if (respPDU.ErrorStatus != 0)
                {
                    if (target.Version == SnmpConstants.version1 && respPDU.ErrorStatus == PDU.noSuchName)
                    {
                        listener.finished(new TreeEventArgs(userObject, new VariableBinding[0]));
                    }
                    listener.finished(new TreeEventArgs(userObject,
                                                    respPDU.ErrorStatus));
                }
                else if (respPDU.Type == PDU.REPORT)
                {
                    listener.finished(new TreeEventArgs(userObject, respPDU));
                }
                else
                {
                    List<VariableBinding> l = new ArrayList<VariableBinding>(respPDU.Count);
                    List<OID> lastOIDs = null;
                    if (!utils.IgnoreLexicographicOrder)
                    {
                        lastOIDs = new ArrayList<OID>(request.Count);
                        for (int i = 0; i < request.Count; i++)
                        {
                            lastOIDs.Add(request[i].Oid);
                        }
                    }

                    bool finished = false;

                    for (int i = 0; ((!finished) || (i % rootOIDs.Count() > 0)) && (i < respPDU.Count); i++)
                    {
                        int r = i % rootOIDs.Count();
                        VariableBinding vb = respPDU[i];
                        if ((vb.Oid == null) ||
                            (vb.Oid.Size < rootOIDs[r].Size) ||
                            (rootOIDs[r].LeftMostCompare(rootOIDs[r].Size, vb.Oid) != 0))
                        {
                            finished = true;
                        }
                        else if (Null.IsExceptionSyntax(vb.Variable.Syntax))
                        {
                            finished = true;
                        }
                        else if (!utils.IgnoreLexicographicOrder && (lastOIDs != null) &&
                                 (vb.Oid.CompareTo(lastOIDs[r]) <= 0))
                        {
                            listener.finished(new TreeEventArgs(userObject,
                                                            RetrievalEventArgs.STATUS_WRONG_ORDER));
                            finished = true;
                            break;
                        }
                        else
                        {
                            finished = false;
                            if (lastOIDs != null)
                            {
                                lastOIDs[r] = vb.Oid;
                            }

                            l.Add(vb);
                        }

                        if ((rootOIDs.Count() > 1) && (i + 1) % rootOIDs.Count() == 0)
                        {
                            // next "row"
                            VariableBinding[] vars = l.ToArray();
                            listener.next(new TreeEventArgs(userObject, vars));
                            l.Clear();
                        }
                    }

                    if (respPDU.Count == 0)
                    {
                        finished = true;
                    }

                    VariableBinding[] vbs = l.ToArray();

                    if (finished)
                    {
                        listener.finished(new TreeEventArgs(userObject, vbs));
                    }
                    else
                    {
                        if (listener.next(new TreeEventArgs(userObject, vbs)))
                        {
                            int lastRowIndex = ((respPDU.Count / rootOIDs.Count()) - 1) * rootOIDs.Count();
                            request.Clear();

                            for (int i = Math.Max(0, lastRowIndex); i < lastRowIndex + rootOIDs.Count(); i++)
                            {
                                VariableBinding next = (VariableBinding)respPDU[i].Clone();
                                next.Variable = new Null();
                                request.Add(next);
                            }

                            if (request.Count > 0)
                            {
                                Send();
                            }
                            else
                            {
                                listener.finished(new TreeEventArgs(userObject, new VariableBinding[0]));
                            }
                        }
                        else
                        {
                            listener.finished(new TreeEventArgs(userObject, vbs));
                        }
                    }
                }
            }
        }

        class InternalTreeListener
        {
            private readonly IList<TreeEventArgs> collectedEvents;
            private volatile bool finished = false;
            private readonly TreeUtils utils;

            public InternalTreeListener(IList<TreeEventArgs> eventList, TreeUtils utils)
            {
                collectedEvents = eventList;
                this.utils = utils;
                utils.OnFinished += this.Next;
            }
            //synchronized
            public void Next(object source, TreeEventArgs evt)
            {
                collectedEvents.Add(evt);
            }
            //synchronized
            public void Finished(object source, TreeEventArgs evt)
            {
                collectedEvents.Add(evt);
                finished = true;
                Notify();
            }

            public IList<TreeEventArgs> CollectedEvents
            {
                get
                {
                    return collectedEvents;
                }
            }

            public bool IsFinished()
            {
                return finished;
            }
        }
    }
}
