// <copyright file="Snmp.cs" company="None">
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

namespace JunoSnmp
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;
    /**
 * The <code>Snmp</code> class is the core of SNMP4J. It provides functions to
 * send and receive SNMP PDUs. All SNMP PDU types can be sent. Confirmed
 * PDUs can be sent synchronously and asynchronously.
 * <p>
 * The <code>Snmp</code> class is transport protocol independent. Support for
 * a specific {@link TransportMapping} instance is added by calling the
 * {@link #addTransportMapping(TransportMapping transportMapping)} method or
 * creating a <code>Snmp</code> instance by using the non-default constructor
 * with the corresponding transport mapping. Transport mappings are used
 * for incoming and outgoing messages.
 * <p>
 * To setup a default SNMP session for UDP transport and with SNMPv3 support
 * the following code snippet can be used:
 * <p>
 * <pre>
 *   Address targetAddress = GenericAddress.parse("udp:127.0.0.1/161");
 *   TransportMapping transport = new DefaultUdpTransportMapping();
 *   snmp = new Snmp(transport);
 *   USM usm = new USM(SecurityProtocols.getInstance(),
 *                     new OctetString(MPv3.createLocalEngineID()), 0);
 *   SecurityModels.getInstance().addSecurityModel(usm);
 *   transport.listen();
 * </pre>
 * <p>
 * How a synchronous SNMPv3 message with authentication and privacy is then
 * sent illustrates the following code snippet:
 * <p>
 * <pre>
 *   // add user to the USM
 *   snmp.getUSM().addUser(new OctetString("MD5DES"),
 *                         new UsmUser(new OctetString("MD5DES"),
 *                                     AuthMD5.ID,
 *                                     new OctetString("MD5DESUserAuthPassword"),
 *                                     PrivDES.ID,
 *                                     new OctetString("MD5DESUserPrivPassword")));
 *   // create the target
 *   UserTarget target = new UserTarget();
 *   target.setAddress(targetAddress);
 *   target.setRetries(1);
 *   target.setTimeout(5000);
 *   target.setVersion(SnmpConstants.version3);
 *   target.setSecurityLevel(SecurityLevel.AUTH_PRIV);
 *   target.setSecurityName(new OctetString("MD5DES"));
 *
 *   // create the PDU
 *   PDU pdu = new ScopedPDU();
 *   pdu.add(new VariableBinding(new OID("1.3.6")));
 *   pdu.setType(PDU.GETNEXT);
 *
 *   // send the PDU
 *   ResponseEvent response = snmp.send(pdu, target);
 *   // extract the response PDU (could be null if timed out)
 *   PDU responsePDU = response.getResponse();
 *   // extract the address used by the agent to send the response:
 *   Address peerAddress = response.getPeerAddress();
 * </pre>
 * <p>
 * An asynchronous SNMPv1 request is sent by the following code:
 * <pre>
 *   // setting up target
 *   CommunityTarget target = new CommunityTarget();
 *   target.setCommunity(new OctetString("public"));
 *   target.setAddress(targetAddress);
 *   target.setRetries(2);
 *   target.setTimeout(1500);
 *   target.setVersion(SnmpConstants.version1);
 *   // creating PDU
 *   PDU pdu = new PDU();
 *   pdu.add(new VariableBinding(new OID(new int[] {1,3,6,1,2,1,1,1})));
 *   pdu.add(new VariableBinding(new OID(new int[] {1,3,6,1,2,1,1,2})));
 *   pdu.setType(PDU.GETNEXT);
 *   // sending request
 *   ResponseListener listener = new ResponseListener() {
 *     public void onResponse(ResponseEvent event) {
 *       // Always cancel async request when response has been received
 *       // otherwise a memory leak is created! Not canceling a request
 *       // immediately can be useful when sending a request to a broadcast
 *       // address.
 *       ((Snmp)event.getSource()).cancel(event.getRequest(), this);
 *       System.out.println("Received response PDU is: "+event.getResponse());
 *     }
 *   };
 *   snmp.sendPDU(pdu, target, null, listener);
 * </pre>
 * </p>
 * Traps (notifications) and other SNMP PDUs can be received by adding the
 * folling code to the first code snippet above:
 * <pre>
 *   CommandResponder trapPrinter = new CommandResponder() {
 *     public synchronized void processPdu(CommandResponderEvent e) {
 *       PDU command = e.getPDU();
 *       if (command != null) {
 *         System.out.println(command.toString());
 *       }
 *     }
 *   };
 *   snmp.addCommandResponder(trapPrinter);
 * </pre>
 * </p>
 *
 * @author Frank Fock
 * @version 1.10
 */
    public class Snmp : ISession, ICommandResponder
    {

        private static readonly log4net.ILog log = log4net.LogManager
                  .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int DEFAULT_MAX_REQUEST_STATUS = 2;
        private static readonly int ENGINE_ID_DISCOVERY_MAX_REQUEST_STATUS = 0;

        // Message processing implementation
        private IMessageDispatcher messageDispatcher;

        /// <summary>
        /// The <c>pendingRequests</c> table contains pending requests
        /// accessed trough the key<code>PduHandle</code>
        /// </summary>
        private Dictionary<PduHandle, PendingRequest> pendingRequests = new Dictionary<PduHandle, PendingRequest>(50);

        /// <summary>
        /// The <c>asyncRequests</c> table contains pending requests
        /// accessed trough the key userObject
        /// </summary>
        private Dictionary<object, PduHandle> asyncRequests = new Dictionary<object, PduHandle>(50);

        /// <summary>
        /// Timer for retrying pending requests
        /// </summary>
        private CommonTimer timer;

        /// <summary>
        /// Listeners for request and trap PDUs
        /// </summary>
        private List<ICommandResponder> commandResponderListeners;

        private ITimeoutModel timeoutModel = new DefaultTimeoutModel();

        // Dispatcher for notification listeners - not needed by default
        private NotificationDispatcher notificationDispatcher = null;

        // Default ReportHandler
        private ReportHandler reportHandler = new ReportProcessor();

        private ConcurrentDictionary<IAddress, OctetString> contextEngineIDs = new
            ConcurrentDictionary<IAddress, OctetString>();
        private bool contextEngineIdDiscoveryDisabled;

        private CounterSupport counterSupport;

        /**
         * Creates a <code>Snmp</code> instance that uses a
         * <code>MessageDispatcherImpl</code> with no message processing
         * models and no security protols (by default). You will have to add
         * those by calling the appropriate methods on
         * {@link #getMessageDispatcher()}.
         * <p>
         * At least one transport mapping has to be added before {@link #listen()}
         * is called in order to be able to send and receive SNMP messages.
         * <p>
         * To initialize a <code>Snmp</code> instance created with this constructor
         * follow this sample code:
         * <pre>
         * Transport transport = ...;
         * Snmp snmp = new Snmp();
         * SecurityProtocols.getInstance().addDefaultProtocols();
         * MessageDispatcher disp = snmp.getMessageDispatcher();
         * disp.addMessageProcessingModel(new MPv1());
         * disp.addMessageProcessingModel(new MPv2c());
         * snmp.addTransportMapping(transport);
         * OctetString localEngineID = new OctetString(
         *    MPv3.createLocalEngineID());
         *    // For command generators, you may use the following code to avoid
         *    // engine ID clashes:
         *    // MPv3.createLocalEngineID(
         *    //   new OctetString("MyUniqueID"+System.currentTimeMillis())));
         * USM usm = new USM(SecurityProtocols.getInstance(), localEngineID, 0);
         * disp.addMessageProcessingModel(new MPv3(usm));
         * snmp.listen();
         * </pre>
         */
        public Snmp()
        {
            this.messageDispatcher = new MessageDispatcherImpl();
            if (JunoSnmpSettings.JunoSnmpStatisticsLevel != JunoSnmpSettings.JunoSnmpStatistics.none)
            {
                counterSupport = CounterSupport.Instance;
            }
        }

        /// <summary>
        /// Interface for handling reports.
        /// </summary>
        public interface ReportHandler
        {
            void ProcessReport(PduHandle pduHandle, CommandResponderArgs evt);
        }

        protected void InitMessageDispatcher()
        {
            this.messageDispatcher.OnIncomingPdu += new CommandResponder(ProcessPdu);
            this.messageDispatcher.AddMessageProcessingModel(new MPv2c());
            this.messageDispatcher.AddMessageProcessingModel(new MPv1());
            this.messageDispatcher.AddMessageProcessingModel(new MPv3());
            SecurityProtocols.GetInstance().AddDefaultProtocols();
        }

        /**
         * Creates a <code>Snmp</code> instance that uses a
         * <code>MessageDispatcherImpl</code> with all supported message processing
         * models and the default security protols for dispatching.
         * <p>
         * To initialize a <code>Snmp</code> instance created with this constructor
         * follow this sample code:
         * <pre>
         * Transport transport = ...;
         * Snmp snmp = new Snmp(transport);
         * OctetString localEngineID =
         *   new OctetString(snmp.getMPv3().getLocalEngineID());
         * USM usm = new USM(SecurityProtocols.getInstance(), localEngineID, 0);
         * SecurityModels.getInstance().addSecurityModel(usm);
         * snmp.listen();
         * </pre>
         *
         * @param transportMapping TransportMapping
         *    the initial <code>TransportMapping</code>. You can add more or remove
         *    the same later.
         */
        public Snmp(ITransportMapping<IAddress> transportMapping) : this()
        {

            InitMessageDispatcher();
            if (transportMapping != null)
            {
                AddTransportMapping(transportMapping);
            }
        }

        /**
         * Creates a <code>Snmp</code> instance by supplying a <code>
         * MessageDispatcher</code> and a <code>TransportMapping</code>.
         * <p>
         * As of version 1.1, the supplied message dispatcher is not altered
         * in terms of adding any message processing models to it. This has to be
         * done now outside the Snmp class.
         * <p>
         * To initialize a <code>Snmp</code> instance created with this constructor
         * follow this sample code:
         * <pre>
         * Transport transport = ...;
         * SecurityProtocols.getInstance().addDefaultProtocols();
         * MessageDispatcher disp = new MessageDispatcherImpl();
         * disp.addMessageProcessingModel(new MPv1());
         * disp.addMessageProcessingModel(new MPv2c());
         * Snmp snmp = new Snmp(disp, transport);
         * OctetString localEngineID = new OctetString(
         *    MPv3.createLocalEngineID());
         *    // For command generators, you may use the following code to avoid
         *    // engine ID clashes:
         *    // MPv3.createLocalEngineID(
         *    //   new OctetString("MyUniqueID"+System.currentTimeMillis())));
         * USM usm = new USM(SecurityProtocols.getInstance(), localEngineID, 0);
         * disp.addMessageProcessingModel(new MPv3(usm));
         * snmp.listen();
         * </pre>
         * @param messageDispatcher
         *    a <code>MessageDispatcher</code> instance that will be used to
         *    dispatch incoming and outgoing messages.
         * @param transportMapping
         *    the initial <code>TransportMapping</code>,
         *    which may be <code>null</code>. You can add or remove transport
         *    mappings later using {@link #addTransportMapping} and
         *    {@link #removeTransportMapping} respectively.
         */
        public Snmp(IMessageDispatcher messageDispatcher,
                    ITransportMapping<IAddress> transportMapping)
        {
            this.messageDispatcher = messageDispatcher;
            this.messageDispatcher.OnIncomingPdu += new CommandResponder(ProcessPdu);

            if (transportMapping != null)
            {
                AddTransportMapping(transportMapping);
            }

            if (JunoSnmpSettings.JunoSnmpStatisticsLevel != JunoSnmpSettings.JunoSnmpStatistics.none)
            {
                counterSupport = CounterSupport.Instance;
            }
        }

        /**
         * Creates a <code>Snmp</code> instance by supplying a <code>
         * MessageDispatcher</code>.
         * <p>
         * The supplied message dispatcher is not altered
         * in terms of adding any message processing models to it. This has to be
         * done now outside the Snmp class.
         * </p>
         * <p>
         * Do not forget to add at least one transport mapping before calling the
         * listen method!
         * <p>
         * To initialize a <code>Snmp</code> instance created with this constructor
         * follow this sample code:
         * <pre>
         * Transport transport = ...;
         * SecurityProtocols.getInstance().addDefaultProtocols();
         * MessageDispatcher disp = new MessageDispatcherImpl();
         * disp.addMessageProcessingModel(new MPv1());
         * disp.addMessageProcessingModel(new MPv2c());
         * Snmp snmp = new Snmp(disp);
         * snmp.addTransportMapping(transport);
         * OctetString localEngineID = new OctetString(
         *    MPv3.createLocalEngineID());
         *    // For command generators, you may use the following code to avoid
         *    // engine ID clashes:
         *    // MPv3.createLocalEngineID(
         *    //   new OctetString("MyUniqueID"+System.currentTimeMillis())));
         * USM usm = new USM(SecurityProtocols.getInstance(), localEngineID, 0);
         * disp.addMessageProcessingModel(new MPv3(usm));
         * snmp.listen();
         * </pre>
         *
         * @param messageDispatcher
         *    a <code>MessageDispatcher</code> instance that will be used to
         *    dispatch incoming and outgoing messages.
         * @since 1.5
         */
        public Snmp(IMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            this.messageDispatcher.OnIncomingPdu += new CommandResponder(ProcessPdu);
            if (JunoSnmpSettings.JunoSnmpStatisticsLevel != JunoSnmpSettings.JunoSnmpStatistics.none)
            {
                counterSupport = CounterSupport.Instance;
            }
        }

        /**
         * Returns the message dispatcher associated with this SNMP session.
         * @return
         *   a <code>MessageDispatcher</code> instance.
         * @since 1.1
         */
        public IMessageDispatcher MessageDispatcher
        {
            get
            {
                return messageDispatcher;
            }
        }

        /**
         * Adds a <code>TransportMapping</code> to this SNMP session.
         * @param transportMapping
         *    a <code>TransportMapping</code> instance.
         */
        public void AddTransportMapping(ITransportMapping<IAddress> transportMapping)
        {
            // connect transport mapping with message dispatcher
            messageDispatcher.AddTransportMapping(transportMapping);
            transportMapping.AddTransportListener(messageDispatcher);
        }

        /**
         * Removes the specified transport mapping from this SNMP session.
         * If the transport mapping is not currently part of this SNMP session,
         * this method will have no effect.
         * @param transportMapping
         *    a previously added <code>TransportMapping</code>.
         */
        public void RemoveTransportMapping(ITransportMapping<IAddress> transportMapping)
        {
            messageDispatcher.RemoveTransportMapping(transportMapping);
            transportMapping.RemoveTransportListener(messageDispatcher);
        }

        /**
         * Adds a notification listener to this Snmp instance. Calling this method
         * will create a transport mapping for the specified listening address and
         * registers the provided <code>CommandResponder</code> with the internal
         * <code>NotificationDispatcher</code>.
         *
         * @param transportMapping
         *    the TransportMapping that is listening on the provided listenAddress.
         *    Call <code>TransportMappings.getInstance().createTransportMapping(listenAddress);</code>
         *    to create such a transport mapping.
         * @param listenAddress
         *    the <code>Address</code> denoting the transport end-point
         *    (interface and port) to listen for incoming notifications.
         * @param listener
         *    the <code>CommandResponder</code> instance that should handle
         *    the received notifications.
         * @return
         *    <code>true</code> if registration was successful and <code>false</code>
         *    if, for example, the transport mapping for the listen address could not
         *    be created.
         * @since 2.5.0
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddNotificationListener(ITransportMapping<IAddress> transportMapping,
                                                            IAddress listenAddress,
                                                            ICommandResponder listener)
        {
            if (transportMapping is IConnectionOrientedTransportMapping<IAddress>)
            {
                ((IConnectionOrientedTransportMapping<IAddress>)transportMapping).SetConnectionTimeout(0);
            }
            transportMapping.AddTransportListener(messageDispatcher);

            if (notificationDispatcher == null)
            {
                notificationDispatcher = new NotificationDispatcher();
                AddCommandResponder(notificationDispatcher);
            }

            notificationDispatcher.AddNotificationListener(listenAddress, transportMapping, listener);
            try
            {
                transportMapping.Listen();
                if (log.IsInfoEnabled)
                {
                    log.Info("Added notification listener for address: " +
                        listenAddress);
                }
                return true;
            }
            catch (IOException ex)
            {
                log.Warn("Failed to initialize notification listener for address '" +
                    listenAddress + "': " + ex.Message);
                return false;
            }
        }

        /**
         * Adds a notification listener to this Snmp instance. Calling this method
         * will create a transport mapping for the specified listening address and
         * registers the provided <code>CommandResponder</code> with the internal
         * <code>NotificationDispatcher</code>.
         *
         * @param listenAddress
         *    the <code>Address</code> denoting the transport end-point
         *    (interface and port) to listen for incoming notifications.
         * @param listener
         *    the <code>CommandResponder</code> instance that should handle
         *    the received notifications.
         * @return
         *    <code>true</code> if registration was successful and <code>false</code>
         *    if, for example, the transport mapping for the listen address could not
         *    be created.
         * @since 1.6
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool AddNotificationListener(IAddress listenAddress, ICommandResponder listener)
        {
            ITransportMapping<IAddress> tm =
                TransportMappings.GetInstance().CreateTransportMapping(listenAddress);
            if (tm == null)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Failed to add notification listener for address: " +
                                listenAddress);
                }

                return false;
            }

            return AddNotificationListener(tm, listenAddress, listener);
        }

        /**
         * Removes (deletes) the notification listener for the specified transport
         * endpoint.
         * @param listenAddress
         *    the listen <code>Address</code> to be removed.
         * @return
         *    <code>true</code> if the notification listener has been removed
         *    successfully.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveNotificationListener(IAddress listenAddress)
        {
            if (notificationDispatcher != null)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Removing notification listener for address: " +
                                listenAddress);
                }

                return notificationDispatcher.RemoveNotificationListener(listenAddress);
            }
            else
            {
                return false;
            }
        }

        /**
         * Gets the transport mapping registered for the specified listen address.
         * @param listenAddress
         *    the listen address.
         * @return
         *    the {@link TransportMapping} for the specified listen address or <code>null</code>
         *    if there is no notification listener for that address.
         * @since 2.5.0
         */
        public ITransportMapping<IAddress> GetNotificationListenerTM(IAddress listenAddress)
        {
            NotificationDispatcher nd = notificationDispatcher;
            if (nd != null)
            {
                return nd.GetTransportMapping(listenAddress);
            }

            return null;
        }

        /**
         * Puts all associated transport mappings into listen mode.
         * @throws IOException
         *    if a transport mapping throws an <code>IOException</code> when its
         *    {@link TransportMapping#listen()} method has been called.
         */
        public void Listen()
        {
            foreach (ITransportMapping<IAddress> tm in messageDispatcher.TransportMappings)
            {
                if (!tm.IsListening)
                {
                    tm.Listen();
                }
            }
        }

        /**
         * Gets the next unique request ID. The returned ID is unique across
         * the last 2^31-1 IDs generated by this message dispatcher.
         * @return
         *    an integer value in the range 1..2^31-1. The returned ID can be used
         *    to map responses to requests send through this message dispatcher.
         * @since 1.1
         * @see MessageDispatcher#getNextRequestID
         */
        public int GetNextRequestID()
        {
            return messageDispatcher.NextRequestID;
        }

        /**
         * Closes the session and frees any allocated resources, i.e. sockets and
         * the internal thread for processing request timeouts.
         * <p>
         * If there are any pending requests, the {@link ResponseListener} associated
         * with the pending requests, will be called with a <code>null</code>
         * response and a {@link InterruptedException} in the error member of the
         * {@link ResponseEvent} returned.
         * <p>
         * After a <code>Session</code> has been closed it must not be used anymore.
         * @throws IOException
         *    if a transport mapping cannot be closed successfully.
         */
        public void Close()
        {
            foreach (ITransportMapping<IAddress> tm in messageDispatcher.TransportMappings)
            {
                if (tm.IsListening)
                {
                    tm.Close();
                }
            }

            CommonTimer t = timer;
            timer = null;

            if (t != null)
            {
                t.cancel();
            }

            // close all notification listeners
            if (notificationDispatcher != null)
            {
                notificationDispatcher.closeAll();
            }

            List<PendingRequest> pr;

            lock (pendingRequests)
            {
                pr = new ArrayList<PendingRequest>(pendingRequests.values());
            }

            foreach (PendingRequest pending in pr)
            {
                pending.Cancel();
                ResponseEvent e =
                    new ResponseEvent(this, null, pending.pdu, null, pending.userObject,
                        new InterruptedException(
                            "Snmp session has been closed"));
                ResponseListener l = pending.listener;
                if (l != null)
                {
                    l.onResponse(e);
                }
            }

            pendingRequests.Clear();
            asyncRequests.Clear();
        }

        /**
         * Sends a GET request to a target. This method sets the PDU's type to
         * {@link PDU#GET} and then sends a synchronous request to the supplied
         * target.
         * @param pdu
         *    a <code>PDU</code> instance. For SNMPv3 messages, the supplied PDU
         *    instance has to be a <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public ResponseEvent Get(PDU pdu, ITarget target)
        {
            pdu.Type = PDU.GET;
            return Send(pdu, target);
        }

        /**
         * Asynchronously sends a GET request <code>PDU</code> to the given target.
         * The response is then returned by calling the supplied
         * <code>ResponseListener</code> instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public void Get(PDU pdu, ITarget target, object userHandle,
                        ResponseListener listener)
        {
            pdu.Type = PDU.GET;
            Send(pdu, target, userHandle, listener);
        }

        /**
         * Sends a GETNEXT request to a target. This method sets the PDU's type to
         * {@link PDU#GETNEXT} and then sends a synchronous request to the supplied
         * target. This method is a convenience wrapper for the
         * {@link #send(PDU pdu, Target target)} method.
         * @param pdu
         *    a <code>PDU</code> instance. For SNMPv3 messages, the supplied PDU
         *    instance has to be a <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public ResponseEvent GetNext(PDU pdu, ITarget target)
        {
            pdu.Type = PDU.GETNEXT;
            return Send(pdu, target);
        }

        /**
         * Asynchronously sends a GETNEXT request <code>PDU</code> to the given
         * target. The response is then returned by calling the supplied
         * <code>ResponseListener</code> instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public void GetNext(PDU pdu, ITarget target, object userHandle,
                            ResponseListener listener)
        {
            pdu.Type = PDU.GETNEXT;
            Send(pdu, target, userHandle, listener);
        }

        /**
         * Sends a GETBULK request to a target. This method sets the PDU's type to
         * {@link PDU#GETBULK} and then sends a synchronous request to the supplied
         * target. This method is a convenience wrapper for the
         * {@link #send(PDU pdu, Target target)} method.
         * @param pdu
         *    a <code>PDU</code> instance. For SNMPv3 messages, the supplied PDU
         *    instance has to be a <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public ResponseEvent GetBulk(PDU pdu, ITarget target)
        {
            pdu.Type = PDU.GETBULK;
            return Send(pdu, target);
        }

        /**
         * Asynchronously sends a GETBULK request <code>PDU</code> to the given
         * target. The response is then returned by calling the supplied
         * <code>ResponseListener</code> instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public void GetBulk(PDU pdu, ITarget target, object userHandle,
                            ResponseListener listener)
        {
            pdu.Type = PDU.GETBULK;
            Send(pdu, target, userHandle, listener);
        }

        /**
         * Sends an INFORM request to a target. This method sets the PDU's type to
         * {@link PDU#INFORM} and then sends a synchronous request to the supplied
         * target. This method is a convenience wrapper for the
         * {@link #send(PDU pdu, Target target)} method.
         * @param pdu
         *    a <code>PDU</code> instance. For SNMPv3 messages, the supplied PDU
         *    instance has to be a <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>.
         * @throws IOException
         *    if the inform request could not be send to the specified target.
         * @since 1.1
         */
        public ResponseEvent Inform(PDU pdu, ITarget target)
        {
            pdu.Type = PDU.INFORM;
            return Send(pdu, target);
        }

        /**
         * Asynchronously sends an INFORM request <code>PDU</code> to the given
         * target. The response is then returned by calling the supplied
         * <code>ResponseListener</code> instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public void Inform(
            PDU pdu,
            ITarget target,
            object userHandle,
            ResponseListener listener)
        {
            pdu.Type = PDU.INFORM;
            Send(pdu, target, userHandle, listener);
        }

        /**
         * Sends a SNMPv1 trap to a target. This method sets the PDU's type to
         * {@link PDU#V1TRAP} and then sends it to the supplied target. This method
         * is a convenience wrapper for the  {@link #send(PDU pdu, Target target)}
         * method.
         * @param pdu
         *    a <code>PDUv1</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>. The selected SNMP protocol version for the
         *    target must be {@link SnmpConstants#version1}.
         * @throws IOException
         *    if the trap cannot be sent.
         * @since 1.1
         */
        public void Trap(PDUv1 pdu, ITarget target)
        {
            if (target.Version != SnmpConstants.version1)
            {
                throw new ArgumentException(
                    "SNMPv1 trap PDU must be used with SNMPv1");
            }
            pdu.Type = PDU.V1TRAP;
            Send(pdu, target);
        }

        /**
         * Sends a SNMPv2c or SNMPv3 notification to a target. This method sets the
         * PDU's type to {@link PDU#NOTIFICATION} and then sends it to the supplied
         * target. This method is a convenience wrapper for the
         * {@link #send(PDU pdu, Target target)} method.
         * @param pdu
         *    a <code>PDUv1</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>. The selected SNMP protocol version for the
         *    target must be {@link SnmpConstants#version2c} or
         *    {@link SnmpConstants#version2c}.
         * @throws IOException
         *    if the notification cannot be sent.
         * @since 1.1
         */
        public void Notify(PDU pdu, ITarget target)
        {
            if (target.Version == SnmpConstants.version1)
            {
                throw new ArgumentException(
                    "Notifications PDUs cannot be used with SNMPv1");
            }

            pdu.Type = PDU.NOTIFICATION;
            Send(pdu, target);
        }


        /**
         * Sends a SET request to a target. This method sets the PDU's type to
         * {@link PDU#SET} and then sends a synchronous request to the supplied
         * target.
         * @param pdu
         *    a <code>PDU</code> instance. For SNMPv3 messages, the supplied PDU
         *    instance has to be a <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public ResponseEvent Set(
            PDU pdu,
            ITarget target)
        {
            pdu.Type = PDU.SET;
            return Send(pdu, target);
        }

        /**
         * Asynchronously sends a SET request <code>PDU</code> to the given target.
         * The response is then returned by calling the supplied
         * <code>ResponseListener</code> instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the PDU cannot be sent to the target.
         * @since 1.1
         */
        public void Set(
            PDU pdu,
            ITarget target,
            object userHandle,
            ResponseListener listener)
        {
            pdu.Type = PDU.SET;
            Send(pdu, target, userHandle, listener);
        }

        public ResponseEvent Send(PDU pdu, ITarget target)
        {
            return Send(pdu, target, null);
        }

        /**
         * Sends a <code>PDU</code> to the given target and if the <code>PDU</code>
         * is a confirmed request, then the received response is returned
         * synchronously.
         * @param pdu
         *    a <code>PDU</code> instance. When sending a SNMPv1 trap PDU, the
         *    supplied PDU instance must be a <code>PDUv1</code>. For all types of
         *    SNMPv3 messages, the supplied PDU instance has to be a
         *    <code>ScopedPDU</code> instance.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param transport
         *    specifies the <code>TransportMapping</code> to be used when sending
         *    the PDU. If <code>transport</code> is <code>null</code>, the associated
         *    message dispatcher will try to determine the transport mapping by the
         *    <code>target</code>'s address.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>. If the sent <code>pdu</code>
         *    is an unconfirmed PDU (notification, response, or report), then
         *    <code>null</code> will be returned.
         * @throws IOException
         *    if the message could not be sent.
         * @see PDU
         * @see ScopedPDU
         * @see PDUv1
         */
        public ResponseEvent Send(PDU pdu, ITarget target,
                                  ITransportMapping<IAddress> transport)
        {
            return Send(pdu, target, transport, DEFAULT_MAX_REQUEST_STATUS);
        }

        private ResponseEvent Send(PDU pdu, ITarget target,
                                   ITransportMapping<IAddress> transport,
                                   int maxRequestStatus)
        {
            if (!pdu.IsConfirmedPdu)
            {
                SendMessage(pdu, target, transport, null);
                return null;
            }

            if (timer == null)
            {
                CreatePendingTimer();
            }

            SyncResponseListener syncResponse = new SyncResponseListener();
            PendingRequest retryRequest = null;
            lock (syncResponse)
            {
                PduHandle handle = null;
                PendingRequest request =
                    new PendingRequest(syncResponse, target, pdu, target, transport);
                request.maxRequestStatus = maxRequestStatus;
                handle = SendMessage(request.Pdu, target, transport, request);
                long totalTimeout =
                    timeoutModel.GetRequestTimeout(target.Retries,
                                                   target.Timeout);
                long stopTime = DateTime.Now.Ticks + totalTimeout * SnmpConstants.MILLISECOND_TO_NANOSECOND;
                try
                {
                    while ((syncResponse.GetResponse() == null) &&
                           (DateTime.Now.Ticks < stopTime))
                    {
                        syncResponse.Wait(totalTimeout);
                    }
                    retryRequest = pendingRequests.Remove(handle);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Removed pending request with handle: " + handle);
                    }
                    request.SetFinished();
                    request.Cancel();
                }
                catch (InterruptedException iex)
                {
                    log.Warn(iex);
                    // cleanup request
                    request.SetFinished();
                    request.Cancel();
                    retryRequest = pendingRequests.Remove(handle);
                    if (retryRequest != null)
                    {
                        retryRequest.SetFinished();
                        retryRequest.Cancel();
                    }

                    Thread.currentThread().interrupt();
                }
                finally
                {
                    if (!request.finished)
                    {
                        // free resources
                        retryRequest = pendingRequests.Remove(handle);
                        if (retryRequest != null)
                        {
                            retryRequest.SetFinished();
                            retryRequest.Cancel();
                        }
                    }
                }
            }

            if (retryRequest != null)
            {
                retryRequest.SetFinished();
                retryRequest.Cancel();
            }

            if (syncResponse.GetResponse() == null)
            {
                syncResponse.Response =
                    new ResponseEvent(Snmp, null, pdu, null, null);
            }

            return syncResponse.response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CreatePendingTimer()
        {
            if (timer == null)
            {
                timer = JunoSnmpSettings.getTimerFactory().createTimer();
            }
        }

        public void Send(
            PDU pdu,
            ITarget target,
            object userHandle,
            ResponseListener listener)
        {
            send(pdu, target, null, userHandle, listener);
        }

        public void Send(
            PDU pdu,
            ITarget target,
            ITransportMapping<IAddress> transport,
            object userHandle,
            ResponseListener listener)
        {
            if (!pdu.IsConfirmedPdu)
            {
                SendMessage(pdu, target, transport, null);
                return;
            }
            if (timer == null)
            {
                CreatePendingTimer();
            }
            PendingRequest request =
                new AsyncPendingRequest(listener, userHandle, pdu, target, transport);
            SendMessage(request.pdu, target, transport, request);
        }

        /**
         * Actually sends a PDU to a target and returns a handle for the sent PDU.
         * @param pdu
         *    the <code>PDU</code> instance to be sent.
         * @param target
         *    a <code>Target</code> instance denoting the target SNMP entity.
         * @param transport
         *    the (optional) transport mapping to be used to send the request.
         *    If <code>transport</code> is <code>null</code> a suitable transport
         *    mapping is determined from the <code>target</code> address.
         * @param pduHandleCallback
         *    callback for newly created PDU handles before the request is sent out.
         * @throws IOException
         *    if the transport fails to send the PDU or the if the message cannot
         *    be BER encoded.
         * @return PduHandle
         *    that uniquely identifies the sent PDU for further reference.
         */
        protected PduHandle SendMessage(PDU pdu, ITarget target,
                                        ITransportMapping<IAddress> transport,
                                        IPduHandleCallback<PDU> pduHandleCallback)

        {
            ITransportMapping<IAddress> tm = transport;
            if (tm == null)
            {
                tm = LookupTransportMapping(target);
            }
            PduHandle handle = messageDispatcher.SendPdu(tm, target,
                                               pdu, true, pduHandleCallback);
            return handle;
        }

        protected ITransportMapping<IAddress> LookupTransportMapping(ITarget target)
        {
            IList<ITransportMapping<IAddress>> preferredTransports =
                target.PreferredTransports;
            if (preferredTransports != null)
            {
                foreach (ITransportMapping<IAddress> tm in preferredTransports)
                {
                    if (target.Address is tm.SupportedAddressClass)
                    {
                        return tm;
                    }
                }
            }
            return null;
        }

        public void Cancel(PDU request, ResponseListener listener)
        {
            AsyncRequestKey key = new AsyncRequestKey(request, listener);
            PduHandle pending = asyncRequests.Remove(key);
            if (log.IsDebugEnabled)
            {
                log.Debug("Cancelling pending request with handle " + pending);
            }
            if (pending != null)
            {
                PendingRequest pendingRequest =
                        pendingRequests.Remove(pending);
                if (pendingRequest != null)
                {
                    lock (pendingRequest)
                    {
                        pendingRequest.setFinished();
                        pendingRequest.Cancel();
                    }
                }
            }
        }

        /**
         * Sets the local engine ID for the SNMP entity represented by this
         * <code>Snmp</code> instance. This is a convenience method that sets
         * the local engine ID in the associated <code>MPv3</code> and
         * <code>USM</code>.
         * @param engineID
         *    a byte array containing the local engine ID. The length and content
         *    has to comply with the constraints defined in the SNMP-FRAMEWORK-MIB.
         * @param engineBoots
         *    the number of boots of this SNMP engine (zero based).
         * @param engineTime
         *    the number of seconds since the value of engineBoots last changed.
         * @see MPv3
         * @see USM
         */
        public void SetLocalEngine(byte[] engineID,
                                   int engineBoots,
                                   int engineTime)
        {
            MPv3 mpv3 = this.MPv3;
            mpv3.LocalEngineID = engineID;
            mpv3.setCurrentMsgID(MPv3.RandomMsgID(engineBoots));
            USM usm = (USM)mpv3.GetSecurityModel((int)SecurityModel.SecurityModelID.SECURITY_MODEL_USM);
            usm.SetLocalEngine(new OctetString(engineID), engineBoots, engineTime);
        }

        /**
         * Gets the local engine ID if the MPv3 is available, otherwise a runtime
         * exception is thrown.
         * @return byte[]
         *    the local engine ID.
         */
        public byte[] LocalEngineID
        {
            get
            {
                return this.MPv3.LocalEngineID;
            }
        }

        private MPv3 MPv3
        {
            get
            {
                MPv3 mpv3 = (MPv3)GetMessageProcessingModel(MessageProcessingModel.MPv3);
                if (mpv3 == null)
                {
                    throw new NotSupportedException("MPv3 not available");
                }
                return mpv3;
            }
        }

        /**
         * Discovers the engine ID of the SNMPv3 entity denoted by the supplied
         * address. This method does not need to be called for normal operation,
         * because SNMP4J automatically discovers authoritative engine IDs and
         * also automatically synchronize engine time values.
         * <p>
         * <em>For this method to operate succesfully, the discover engine IDs
         * flag in {@link USM} must be <code>true</code> (which is the default).
         * </em>
         * @param address
         *    an Address instance representing the transport address of the SNMPv3
         *    entity for which its authoritative engine ID should be discovered.
         * @param timeout
         *    the maximum time in milliseconds to wait for a response.
         * @return
         *    a byte array containing the authoritative engine ID or <code>null</code>
         *    if it could not be discovered.
         * @see USM#setEngineDiscoveryEnabled(bool enableEngineDiscovery)
         */
        public byte[] DiscoverAuthoritativeEngineID(IAddress address, long timeout)
        {
            MPv3 mpv3 = this.MPv3;
            // We need to remove the engine ID explicitly to be sure that it is updated
            OctetString engineID = mpv3.RemoveEngineID(address);
            // Now try to remove the engine as well
            if (engineID != null)
            {
                USM usm = getUSM();
                if (usm != null)
                {
                    usm.RemoveEngineTime(engineID);
                }
            }
            ScopedPDU scopedPDU = new ScopedPDU();
            scopedPDU.Type = PDU.GET;
            SecureTarget target = new UserTarget();
            target.Timeout = timeout;
            target.Address = address;
            target.SecurityLevel = SecurityLevel.NoAuthNoPriv;
            try
            {
                Send(scopedPDU, target, null, ENGINE_ID_DISCOVERY_MAX_REQUEST_STATUS);
                OctetString authoritativeEngineID = mpv3.GetEngineID(address);
                if (authoritativeEngineID == null)
                {
                    return null;
                }
                // we copy the byte array here, so we are sure nobody can modify the
                // internal cache.
                return new OctetString(authoritativeEngineID.GetValue()).GetValue();
            }
            catch (IOException ex)
            {
                log.Error(
                    "IO error while trying to discover authoritative engine ID: " +
                    ex);
                return null;
            }
        }

        /**
         * Gets the User Based Security Model (USM). This is a convenience method
         * that uses the {@link MPv3#getSecurityModel} method of the associated MPv3
         * instance to get the USM.
         * @return
         *    the <code>USM</code> instance associated with the MPv3 bound to this
         *    <code>Snmp</code> instance, or <code>null</code> otherwise.
         */
        public USM GetUSM()
        {
            MPv3 mp = (MPv3)GetMessageProcessingModel((int)MPv3.ID);
            if (mp != null)
            {
                return (USM)mp.GetSecurityModel((int)SecurityModel.SecurityModelID.SECURITY_MODEL_USM);
            }
            return null;
        }

        /**
         * Gets the message processing model for the supplied ID.
         * @param messageProcessingModel
         *    a mesage processing model ID as defined in {@link MessageProcessingModel}.
         * @return MessageProcessingModel
         *    a <code>MessageProcessingModel</code> if
         *    <code>messageProcessingModel</code> has been registered with the
         *    message dispatcher associated with this SNMP session.
         */
        public MessageProcessingModel GetMessageProcessingModel(int messageProcessingModel)
        {
            return messageDispatcher.GetMessageProcessingModel(messageProcessingModel);
        }

        /**
         * Process an incoming request or notification PDU.
         *
         * @param event
         *   a <code>CommandResponderEvent</code> with the decoded incoming PDU as
         *   dispatched to this method call by the associated message dispatcher.
         */
        public void ProcessPdu(object source, CommandResponderArgs evt)
        {
            PduHandle handle = evt.PduHandle;
            if ((JunoSnmpSettings.JunoSnmpStatisticsLevel == JunoSnmpSettings.JunoSnmpStatistics.extended) &&
                (handle is IRequestStatistics))
            {
                IRequestStatistics requestStatistics = (IRequestStatistics)handle;
                counterSupport.IncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmp4jStatsRequestRuntime,
                    requestStatistics.ResponseRuntimeNanos));
                CounterIncrArgs counterEvent =
                    new CounterIncrArgs(SnmpConstants.snmp4jStatsReqTableRuntime, requestStatistics.ResponseRuntimeNanos, evt.PeerAddress);
                counterSupport.IncrementCounter(this, counterEvent);
            }

            PDU pdu = evt.PDU;

            if (pdu.Type == PDU.REPORT)
            {
                evt.Processed = true;
                reportHandler.ProcessReport(handle, evt);
            }
            else if (pdu.Type == PDU.RESPONSE)
            {
                evt.Processed = true;
                PendingRequest request;

                if (log.IsDebugEnabled)
                {
                    log.Debug("Looking up pending request with handle " + handle);
                }

                lock (pendingRequests)
                {
                    request = pendingRequests[handle];
                    if (request != null)
                    {
                        request.ResponseReceived();
                    }
                }

                if (request == null)
                {
                    if (log.IsWarnEnabled)
                    {
                        log.Warn("Received response that cannot be matched to any " +
                            "outstanding request, address=" +
                            evt.PeerAddress+
                            ", requestID=" + pdu.RequestID);
                    }
                }
                else if (!ResendRequest(request, pdu))
                {
                    ResponseListener l = request.listener;
                    if (l != null)
                    {
                        l.onResponse(new ResponseEvent(this,
                                                       evt.PeerAddress,
                                                       request.Pdu,
                                                       pdu,
                                                       request.userObject));
                    }
                }
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Fire process PDU event: " + evt.toString());
                }

                FireProcessPdu(evt);
            }
        }

        /**
         * Checks whether RFC5343 based context engine ID discovery is disabled or not.
         * The default value is <code>false</code>.
         * @return
         *    <code>true</code> if context engine ID discovery is disabled.
         * @since 2.0
         */
        public bool IsContextEngineIdDiscoveryDisabled
        {
            get
            {
                return contextEngineIdDiscoveryDisabled;
            }
        }

        /**
         * Sets the RFC5343 based context engine ID discovery.
         * The default value is <code>false</code>.
         * @param contextEngineIdDiscoveryDisabled
         *    <code>true</code> to disable context engine ID discovery,
         *    <code>false</code> to enable context engine ID discovery.
         * @since 2.0
         */
        public void SetContextEngineIdDiscoveryDisabled(bool contextEngineIdDiscoveryDisabled)
        {
            this.contextEngineIdDiscoveryDisabled = contextEngineIdDiscoveryDisabled;
        }

        protected bool ResendRequest(PendingRequest request, PDU response)
        {
            if (request.UseNextPDU())
            {
                request.IsResponseReceived = false;
                lock (pendingRequests)
                {
                    pendingRequests.Remove(request.key);
                    PduHandle holdKeyUntilResendDone = request.key;
                    request.key = null;
                    handleInternalResponse(response, request.pdu, request.target.getAddress());

                    try
                    {
                        SendMessage(request.pdu, request.target, request.transport, request);
                    }
                    catch (IOException e)
                    {
                        log.Error("IOException while resending request after RFC 5343 context engine ID discovery: " +
                            e.Message, e);
                    }

                    // now the previous retry can be released
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Releasing PDU handle " + holdKeyUntilResendDone);
                    }

                    holdKeyUntilResendDone = null;
                }

                return true;
            }

            return false;
        }

        protected void HandleInternalResponse(PDU response, PDU pdu, IAddress target)
        {
            IVariable contextEngineID = response.GetVariable(SnmpConstants.snmpEngineID);
            if (contextEngineID is OctetString)
            {
                if (pdu is ScopedPDU)
                {
                    ((ScopedPDU)pdu).ContextEngineID = (OctetString)contextEngineID;
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Discovered contextEngineID '" + contextEngineID +
                                    "' by RFC 5343 for " + target);
                    }
                }
            }
        }

        class ReportProcessor : ReportHandler
        {

            public void ProcessReport(PduHandle handle, CommandResponderArgs e)
            {
                PDU pdu = e.PDU;
                log.Debug("Searching pending request with handle" + handle);
                PendingRequest request = pendingRequests[handle];

                VariableBinding vb = CheckReport(e, pdu, request);
                if (vb == null) return;

                OID firstOID = vb.Oid;
                bool resend = false;
                if (request.requestStatus < request.maxRequestStatus)
                {
                    switch (request.requestStatus)
                    {
                        case 0:
                            if (SnmpConstants.usmStatsUnknownEngineIDs.equals(firstOID))
                            {
                                resend = true;
                            }
                            else if (SnmpConstants.usmStatsNotInTimeWindows.equals(firstOID))
                            {
                                request.requestStatus++;
                                resend = true;
                            }
                            break;
                        case 1:
                            if (SnmpConstants.usmStatsNotInTimeWindows.equals(firstOID))
                            {
                                resend = true;
                            }
                            break;
                    }
                }
                // if legal report PDU received, then resend request
                if (resend)
                {
                    log.Debug("Send new request after report.");
                    request.requestStatus++;
                    try
                    {
                        // We need no callback here because we already have an equivalent
                        // handle registered.
                        PduHandle resentHandle =
                            SendMessage(request.pdu, request.target, e.getTransportMapping(),
                                        null);
                        // make sure reference to handle is hold until request is finished,
                        // because otherwise cache information may get lost (WeakHashMap)
                        request.key = resentHandle;
                    }
                    catch (IOException iox)
                    {
                        log.Error("Failed to send message to " + request.target + ": " +
                                     iox.getMessage());
                        return;
                    }
                }
                else
                {
                    bool intime;
                    // Get the request members needed before canceling the request
                    // which resets it
                    ResponseListener reqListener = request.listener;
                    PDU reqPDU = request.Pdu;
                    object reqUserObject = request.userObject;

                    lock (request)
                    {
                        intime = request.Cancel();
                    }

                    // remove pending request
                    // (sync is not needed as request is already canceled)
                    pendingRequests.remove(handle);
                    if (intime && (reqListener != null))
                    {
                        // return report
                        reqListener.onResponse(new ResponseEvent(this,
                            e.getPeerAddress(),
                            reqPDU,
                            pdu,
                            reqUserObject));
                    }
                    else
                    {
                        // silently drop late report
                        if (log.IsInfoEnabled)
                        {
                            log.Info("Received late report from " +
                                        e.getPeerAddress() +
                                        " with request ID " + pdu.RequestID);
                        }
                    }
                }
            }

            protected VariableBinding CheckReport(CommandResponderArgs e, PDU pdu, PendingRequest request)
            {
                if (request == null)
                {
                    log.Warn("Unmatched report PDU received from " + e.PeerAddress);
                    return null;
                }

                if (pdu.size == 0)
                {
                    log.Error("Illegal report PDU received from " + e.PeerAddress +
                                 " missing report variable binding");
                    return null;
                }

                VariableBinding vb = pdu[0];
                if (vb == null)
                {
                    log.Error("Received illegal REPORT PDU from " + e.PeerAddress);
                    return null;
                }

                // RFC 7.2.11 (b):
                if (e.SecurityModel != request.target.SecurityModel)
                {
                    log.Warn("RFC3412 ยง7.2.11.b: Received REPORT PDU with different security model than cached one: " + e);
                    return null;
                }

                // RFC 7.2.11 (b):
                if ((e.SecurityLevel == SecurityLevel.NoAuthNoPriv) &&
                    (JunoSnmpSettings.ReportSecurityLevelStrategy !=
                     JunoSnmpSettings.ReportSecurityLevelOption.noAuthNoPrivIfNeeded) &&
                    ((e.SecurityLevel != request.target.getSecurityLevel()) &&
                     (!SnmpConstants.usmStatsUnknownUserNames.Equals(vb.Oid)) &&
                     (!SnmpConstants.usmStatsUnknownEngineIDs.Equals(vb.Oid))))
                {
                    log.Warn("RFC3412 ยง7.2.11.b:Received REPORT PDU with security level noAuthNoPriv from '" + e + "'. " +
                                "Ignoring it, because report strategy is set to " +
                                JunoSnmpSettings.ReportSecurityLevelStrategy);
                    return null;
                }

                return vb;
            }
        }


        /**
         * Removes a <code>CommandResponder</code> from this SNMP session.
         * @param listener
         *    a previously added <code>CommandResponder</code> instance.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveCommandResponder(CommandResponder listener)
        {
            if (commandResponderListeners != null &&
                commandResponderListeners.Contains(listener))
            {
                ArrayList<CommandResponder> v = new ArrayList<CommandResponder>(commandResponderListeners);
                v.remove(listener);
                commandResponderListeners = v;
            }
        }

        /**
         * Adds a <code>CommandResponder</code> to this SNMP session.
         * The command responder will then be informed about incoming SNMP PDUs of
         * any kind that are not related to any outstanding requests of this SNMP
         * session.
         *
         * @param listener
         *    the <code>CommandResponder</code> instance to be added.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCommandResponder(CommandResponder listener)
        {
            ArrayList<CommandResponder> v = (commandResponderListeners == null) ?
                new ArrayList<CommandResponder>(2) :
                new ArrayList<CommandResponder>(commandResponderListeners);
            if (!v.contains(listener))
            {
                v.add(listener);
                commandResponderListeners = v;
            }
        }

        /**
         * Fires a <code>CommandResponderEvent</code> event to inform listeners about
         * a received PDU. If a listener has marked the event as processed further
         * listeners will not be informed about the event.
         * @param event
         *    a <code>CommandResponderEvent</code>.
         */
        protected void FireProcessPdu(CommandResponderArgs evt)
        {
            if (commandResponderListeners != null)
            {
                List<CommandResponder> listeners = commandResponderListeners;
                foreach (CommandResponder listener in listeners)
                {
                    listener.processPdu(evt);
                    // if event is marked as processed the event is not forwarded to
                    // remaining listeners
                    if (evt.isProcessed())
                    {
                        return;
                    }
                }
            }
        }

        /**
         * Gets the timeout model associated with this SNMP session.
         * @return
         *    a TimeoutModel instance (never <code>null</code>).
         * @see #setTimeoutModel(TimeoutModel timeoutModel)
         */

        /**
         * Sets the timeout model for this SNMP session. The default timeout model
         * sends retries whenever the time specified by the <code>timeout</code>
         * parameter of the target has elapsed without a response being received for
         * the request. By specifying a different timeout model this behaviour can
         * be changed.
         * @param timeoutModel
         *    a <code>TimeoutModel</code> instance (must not be <code>null</code>).
         */
        public ITimeoutModel TimeoutModel
        {
            get
            {
                return timeoutModel;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Timeout model cannot be null");
                }

                this.timeoutModel = value;
            }
        }

        /**
         * Returns the report handler which is used internally to process reports
         * received from command responders.
         * @return
         *    the <code>ReportHandler</code> instance.
         * @since 1.6
         */

        /**
         * Sets the report handler and overrides the default report handler.
         * @param reportHandler
         *    a <code>ReportHandler</code> instance which must not be
         *    <code>null</code>.
         * @since 1.6
         */
        public ReportHandler ReportHandler
        {
            get
            {
                return reportHandler;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("ReportHandler must not be null");
                }
                this.reportHandler = value;
            }
        }

        /**
         * Gets the counter support for Snmp related counters. These are for example:
         * snmp4jStatsRequestTimeouts,
         * snmp4jStatsRequestTimeouts,
         * snmp4jStatsRequestWaitTime
         * @return
         *   the counter support if available. If the {@link JunoSnmpSettings#getSnmp4jStatistics()} value is
         *   {@link org.snmp4j.JunoSnmpSettings.Snmp4jStatistics#none} then no counter support will be created
         *   and no statistics will be collected.
         * @since 2.4.2
         */

        /**
         * Sets the counter support instance to handle counter events on behalf of this Snmp instance.
         * @param counterSupport
         *   the counter support instance that collects the statistics events created by this Snmp instance.
         *   See also {@link #getCounterSupport()}.
         * @since 2.4.2
         */
        public CounterSupport CounterSupport
        {
            get
            {
                return counterSupport;
            }

            set
            {
                this.counterSupport = value;
            }
        }

        /**
         * Gets the number of currently pending synchronous requests.
         * @return
         *    the size of the synchronous request queue.
         * @since 2.4.2
         */
        public int PendingSyncRequestCount
        {
            get
            {
                return pendingRequests.Count;
            }
        }

        /**
         * Gets the number of currently pending asynchronous requests.
         * @return
         *    the size of the asynchronous request queue.
         * @since 2.4.2
         */
        public int PendingAsyncRequestCount
        {
            get
            {
                return asyncRequests.Count;
            }
        }

        private bool IsEmptyContextEngineID(PDU pdu)
        {
            if (pdu is ScopedPDU)
            {
                ScopedPDU scopedPDU = (ScopedPDU)pdu;
                return ((scopedPDU.ContextEngineID == null) ||
                        (scopedPDU.ContextEngineID.Length == 0));
            }
            return false;
        }


        class PendingRequest : TimerTask, IPduHandleCallback<PDU>, ICloneable
        {

            private PduHandle key;
            protected int retryCount;
            protected ResponseListener listener;
            protected object userObject;

            protected PDU pdu;
            protected ITarget target;
            protected ITransportMapping<IAddress> transport;

            private int requestStatus = 0;
            // Maximum request status - allows to receive up to two reports and then
            // send the original request again. A value of 0 is used for discovery.
            private int maxRequestStatus = DEFAULT_MAX_REQUEST_STATUS;

            private volatile bool finished = false;
            private volatile bool responseReceived = false;
            private volatile bool pendingRetry = false;
            private volatile bool cancelled = false;

            private CounterEvent waitTime;
            private CounterEvent waitTimeTarget;

            /**
             * The <code>nextPDU</code> field holds a PDU that has to be sent
             * when the response of the <code>pdu</code> has been received.
             * Usually, this is used for (context) engine ID discovery.
             */
            private PDU nextPDU;

            public PendingRequest(ResponseListener listener,
                                  object userObject,
                                  PDU pdu,
                                  ITarget target,
                                  ITransportMapping<IAddress> transport)
            {
                this.userObject = userObject;
                this.listener = listener;
                this.retryCount = target.Retries;
                this.pdu = pdu;
                this.target = (ITarget)target.Clone();
                this.transport = transport;
                if (JunoSnmpSettings.JunoSnmpStatisticsLevel != JunoSnmpSettings.JunoSnmpStatistics.none)
                {
                    waitTime = new CounterIncrArgs(SnmpConstants.snmp4jStatsRequestWaitTime, DateTime.Now.Ticks);
                    if (JunoSnmpSettings.JunoSnmpStatisticsLevel == JunoSnmpSettings.JunoSnmpStatistics.extended)
                    {
                        waitTimeTarget =
                            new CounterIncrArgs(SnmpConstants.snmp4jStatsReqTableWaitTime,
                                DateTime.Now.Ticks, target.Address);
                    }
                }
                if (IsEmptyContextEngineID(pdu))
                {
                    OctetString contextEngineID = contextEngineIDs.get(target.getAddress());
                    if (contextEngineID != null)
                    {
                        ((ScopedPDU)pdu).ContextEngineID = contextEngineID;
                    }
                    else if (!contextEngineIdDiscoveryDisabled)
                    {
                        DiscoverContextEngineID();
                    }
                }
            }

            private PendingRequest(PendingRequest other)
            {
                this.userObject = other.userObject;
                this.listener = other.listener;
                this.retryCount = other.retryCount - 1;
                this.pdu = other.pdu;
                this.target = other.target;
                this.requestStatus = other.requestStatus;
                this.responseReceived = other.responseReceived;
                this.transport = other.transport;
                this.nextPDU = other.nextPDU;
                this.waitTime = other.waitTime;
            }

            private void DiscoverContextEngineID()
            {
                MessageProcessingModel mp = messageDispatcher.getMessageProcessingModel(target.getVersion());
                if ((mp is MPv3) && (target is SecureTarget))
                {
                    MPv3 mpv3 = (MPv3)mp;
                    SecureTarget st = (SecureTarget)target;
                    SecurityModel sm = mpv3.getSecurityModel(st.getSecurityModel());
                    if ((sm != null) && (!sm.supportsEngineIdDiscovery()))
                    {
                        // Perform context engine ID discovery according to RFC 5343
                        if (log.IsInfoEnabled)
                        {
                            log.Info("Performing RFC 5343 contextEngineID discovery on " + target);
                        }
                        ScopedPDU discoverPDU = new ScopedPDU();
                        discoverPDU.ContextEngineID = MPv3.LOCAL_ENGINE_ID;
                        discoverPDU.Add(new VariableBinding(SnmpConstants.snmpEngineID));
                        InsertFirstPDU(discoverPDU);
                    }
                }
            }

            protected virtual void RegisterRequest(PduHandle handle)
            {
                // overwritten by subclasses
            }

            public bool UseNextPDU()
            {
                if (nextPDU != null)
                {
                    pdu = nextPDU;
                    nextPDU = null;
                    return true;
                }
                return false;
            }

            public void InsertFirstPDU(PDU firstPDU)
            {
                nextPDU = this.pdu;
                this.pdu = firstPDU;
            }

            public void ResponseReceived()
            {
                this.responseReceived = true;
                if (waitTime != null)
                {
                    CounterSupport counterSupport = getCounterSupport();
                    if (counterSupport != null)
                    {
                        long increment = (DateTime.Now.Ticks - waitTime.getIncrement()) / SnmpConstants.MILLISECOND_TO_NANOSECOND;
                        waitTime.setIncrement(increment);
                        counterSupport.IncrementCounter(waitTime);
                        if (waitTimeTarget != null)
                        {
                            waitTimeTarget.setIncrement(increment);
                            counterSupport.IncrementCounter(waitTimeTarget);
                        }
                    }
                }
            }


            public PDU NextPDU
            {
                get
                {
                    return nextPDU;
                }

                set
                {
                    this.nextPDU = value;
                }
            }

            public object Clone()
            {
                return base.Clone();
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void PduHandleAssigned(PduHandle handle, PDU pdu)
            {
                if (key == null)
                {
                    key = handle;
                    // get pointer to target before adding request to pending list
                    // to make sure that this retry is not being cancelled before we
                    // got the target pointer.
                    ITarget t = target;
                    if ((t != null) && (!cancelled))
                    {
                        PendingRequests.put(handle, this);
                        RegisterRequest(handle);
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Running pending " +
                                         ((listener is SyncResponseListener) ?
                                          "sync" : "async") +
                                         " request with handle " + handle +
                                         " and retry count left " + retryCount);
                        }
                        long delay =
                            timeoutModel.getRetryTimeout(t.Retries - retryCount,
                                                         t.Retries,
                                                         t.Timeout);
                        if ((!finished) && (!responseReceived) && (!cancelled))
                        {
                            try
                            {
                                CommonTimer timerCopy = timer;
                                if (timerCopy != null)
                                {
                                    timerCopy.schedule(this, delay);
                                }
                                // pending request will be removed by the close() call
                            }
                            catch (IllegalStateException isex)
                            {
                                // ignore
                            }
                        }
                        else
                        {
                            pendingRequests.remove(handle);
                        }
                    }
                }
            }

            /**
             * Process retries of a pending request.
             */
            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Run()
            {
                PduHandle m_key = key;
                PDU m_pdu = pdu;
                ITarget m_target = target;
                ITransportMapping<IAddress> m_transport = transport;
                ResponseListener m_listener = listener;
                Object m_userObject = userObject;

                if ((m_key == null) || (m_pdu == null) || (m_target == null) ||
                    (m_listener == null))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("PendingRequest canceled key=" + m_key + ", pdu=" + m_pdu +
                            ", target=" + m_target + ", transport=" + m_transport + ", listener=" +
                            m_listener);
                    }
                    return;
                }

                try
                {
                    lock (pendingRequests)
                    {
                        this.pendingRetry =
                            (!finished) && (retryCount > 0) && (!responseReceived);
                    }

                    if (this.pendingRetry)
                    {
                        try
                        {
                            PendingRequest nextRetry = new PendingRequest(this);
                            SendMessage(m_pdu, m_target, m_transport, nextRetry);
                            this.pendingRetry = false;
                            if (waitTime != null)
                            {
                                CounterSupport counterSupport = getCounterSupport();
                                if (counterSupport != null)
                                {
                                    counterSupport.IncrementCounter(this,
                                        new CounterIncrArgs(SnmpConstants.snmp4jStatsRequestRetries));
                                    if (JunoSnmpSettings.JunoSnmpStatisticsLevel == JunoSnmpSettings.JunoSnmpStatistics.extended)
                                    {
                                        counterSupport.IncrementCounter(this, 
                                            new CounterIncrArgs(SnmpConstants.snmp4jStatsReqTableRetries, 1, m_target.Address));
                                    }
                                }
                            }
                        }
                        catch (IOException ex)
                        {
                            ResponseListener l = listener;
                            finished = true;
                            log.Error("Failed to send SNMP message to " + m_target +
                                         ": " +
                                         ex.getMessage());
                            messageDispatcher.releaseStateReference(m_target.Version,
                                m_key);
                            if (l != null)
                            {
                                listener.onResponse(new ResponseEvent(Snmp, null,
                                    m_pdu, null, m_userObject, ex));
                            }
                        }
                    }
                    else if (!finished)
                    {
                        finished = true;
                        pendingRequests.remove(m_key);
                        if (!cancelled)
                        {
                            // request timed out
                            if (log.IsDebugEnabled)
                            {
                                log.Debug("Request timed out: " + m_key.TransactionID);
                            }

                            messageDispatcher.releaseStateReference(m_target.Version,
                                                                    m_key);
                            m_listener.onResponse(new ResponseEvent(this, null,
                                                                  m_pdu, null, m_userObject));
                        }
                    }
                    else
                    {
                        // make sure pending request is removed even if response listener
                        // failed to call Snmp.cancel
                        pendingRequests.Remove(m_key);
                    }
                }
                catch (RuntimeException ex)
                {
                    log.Error("Failed to process pending request " + m_key +
                                 " because " + ex.getMessage(), ex);
                    throw ex;
                }
                catch (Error er)
                {
                    log.Fatal("Failed to process pending request " + m_key +
                                 " because " + er.getMessage(), er);
                    throw er;
                }
            }

            public bool SetFinished()
            {
                bool currentState = finished;
                this.finished = true;
                return currentState;
            }

            public int MaxRequestStatus
            {
                get
                {
                    return maxRequestStatus;
                }

                set
                {
                    this.maxRequestStatus = value;
                }
            }

            public bool IsResponseReceived
            {
                get
                {

                    return responseReceived;
                }
            }

            /**
             * Cancels the request and clears all internal fields by setting them
             * to <code>null</code>.
             * @return
             *    <code>true</code> if cancellation was successful.
             */
            public bool Cancel()
            {
                cancelled = true;
                bool result = base.Cancel();
                ITarget m_target = target;
                if (waitTime != null && !isResponseReceived())
                {
                    CounterSupport counterSupport = getCounterSupport();
                    if (counterSupport != null)
                    {
                        counterSupport.IncrementCounter(
                            new CounterEvent(Snmp, SnmpConstants.snmp4jStatsRequestTimeouts));
                        if (JunoSnmpSettings.JunoSnmpStatisticsLevel == JunoSnmpSettings.JunoSnmpStatistics.extended && (m_target != null))
                        {
                            counterSupport.IncrementCounter(
                                new CounterEvent(Snmp, SnmpConstants.snmp4jStatsReqTableTimeouts, m_target.getAddress(), 1));
                        }
                    }
                }

                // free objects early
                if (!pendingRetry)
                {
                    key = null;
                    pdu = null;
                    target = null;
                    transport = null;
                    listener = null;
                    userObject = null;
                }
                return result;
            }
        }

        class AsyncPendingRequest : PendingRequest
        {
            public AsyncPendingRequest(ResponseListener listener,
                                       Object userObject,
                                       PDU pdu,
                                       Target target,
                                       TransportMapping transport)
                        : base(listener, userObject, pdu, target, transport)
            {
            }

            protected override void RegisterRequest(PduHandle handle)
            {
                AsyncRequestKey key = new AsyncRequestKey(super.pdu, super.listener);
                asyncRequests.put(key, handle);
            }

        }

        class AsyncRequestKey
        {
            private PDU request;
            private ResponseListener listener;

            public AsyncRequestKey(PDU request, ResponseListener listener)
            {
                this.request = request;
                this.listener = listener;
            }

            /**
             * Indicates whether some other object is "equal to" this one.
             *
             * @param obj the reference object with which to compare.
             * @return <code>true</code> if this object is the same as the obj argument;
             *   <code>false</code> otherwise.
             */
            public override bool Equals(object obj)
            {
                if (obj is AsyncRequestKey)
                {
                    AsyncRequestKey other = (AsyncRequestKey)obj;
                    return (request.Equals(other.request) && listener.equals(other.listener));
                }
                return false;
            }

            public override int GetHashCode()
            {
                return request.GetHashCode();
            }
        }

        static class SyncResponseListener : ResponseListener
        {

            private ResponseEvent response = null;

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void OnResponse(ResponseEvent evt)
            {
                this.response = evt;
                this.notify();
            }

            public ResponseEvent GetResponse()
            {
                return response;
            }

        }

        /**
         * The <code>NotificationDispatcher</code> dispatches traps, notifications,
         * and to registered listeners.
         *
         * @author Frank Fock
         * @version 2.5.0
         * @since 1.6
         */
        class NotificationDispatcher : CommandResponder
        {
            // A mapping of transport addresses to transport mappings of notification
            // listeners
            private Dictionary<IAddress, ITransportMapping<IAddress>> notificationListeners = new Dictionary<IAddress, ITransportMapping<IAddress>>(10);
            private Dictionary<ITransportMapping<IAddress>, CommandResponder> notificationTransports = new Dictionary<ITransportMapping<IAddress>, CommandResponder>(10);

            protected NotificationDispatcher()
            {
            }

            public ITransportMapping<IAddress> GetTransportMapping(IAddress listenAddress)
            {
                return notificationListeners[listenAddress];
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void AddNotificationListener(IAddress listenAddress,
                                                         ITransportMapping<IAddress> transport,
                                                         CommandResponder listener)
            {
                notificationListeners[listenAddress] = transport;
                notificationTransports[transport] = listener;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public bool RemoveNotificationListener(IAddress listenAddress)
            {
                ITransportMapping<IAddress> tm =
                        notificationListeners.Remove(listenAddress);
                if (tm == null)
                {
                    return false;
                }
                tm.removeTransportListener(messageDispatcher);
                notificationTransports.remove(tm);

                CloseTransportMapping(tm);
                return true;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void CloseAll()
            {
                notificationTransports.Clear();
                foreach (ITransportMapping<IAddress> tm in notificationListeners.values())
                {
                    CloseTransportMapping(tm);
                }

                notificationListeners.Clear();
            }

            public void ProcessPdu(CommandResponderEvent evt)
            {
                CommandResponder listener;
                lock (this)
                {
                    listener = notificationTransports.get(evt.getTransportMapping());
                }

                if ((evt.getPDU() != null) && (evt.getPDU().getType() == PDU.INFORM))
                {
                    // try to send INFORM response
                    try
                    {
                        SendInformResponse(evt);
                    }
                    catch (MessageException mex)
                    {
                        if (log.IsWarnEnabled)
                        {
                            log.Warn("Failed to send response on INFORM PDU event (" +
                            evt + "): " + mex.Message);
                        }
                    }
                }
                if (listener != null)
                {
                    listener.processPdu(evt);
                }
            }

            /**
             * Sends a RESPONSE PDU to the source address of a INFORM request.
             * @param event
             *    the <code>CommandResponderEvent</code> with the INFORM request.
             * @throws
             *    MessageException if the response could not be created and sent.
             */
            protected void SendInformResponse(CommandResponderEvent evt)
            {
                PDU responsePDU = (PDU)evt.getPDU().clone();
                responsePDU.Type = PDU.RESPONSE;
                responsePDU.ErrorStatus = PDU.noError;
                responsePDU.ErrorIndex = 0;
                messageDispatcher.returnResponsePdu(evt.getMessageProcessingModel(),
                                              evt.getSecurityModel(),
                                              evt.getSecurityName(),
                                              evt.getSecurityLevel(),
                                              responsePDU,
                                              evt.getMaxSizeResponsePDU(),
                                              evt.getStateReference(),
                                              new StatusInformation());
            }
        }

        protected void CloseTransportMapping(ITransportMapping<IAddress> tm)
        {
            try
            {
                tm.Close();
            }
            catch (IOException ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled)
                {
                    ex.printStackTrace();
                }
            }
        }

    }
}
