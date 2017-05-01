// <copyright file="MessageDispatcherImpl.cs" company="None">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /**
 * The <code>MessageDispatcherImpl</code> decodes and dispatches incoming
 * messages using {@link MessageProcessingModel} instances and encodes
 * and sends outgoing messages using an appropriate {@link TransportMapping}
 * instances.
 * <p>
 * The method {@link #processMessage} will be called from a
 * <code>TransportMapping</code> whereas the method {@link #sendPdu} will be
 * called by the application.
 *
 * @see Snmp
 * @see TransportMapping
 * @see MessageProcessingModel
 * @see MPv1
 * @see MPv2c
 * @see MPv3
 *
 * @author Frank Fock
 * @version 2.0
 */
    public class MessageDispatcherImpl : IMessageDispatcher
    {

        private static readonly log4net.ILog log = log4net.LogManager
                .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<MessageProcessingModel> mpm = new List<MessageProcessingModel>(3);
        private Dictionary<IAddress, List<ITransportMapping<IAddress>>> transportMappings =
            new Dictionary<IAddress, List<ITransportMapping<IAddress>>>(5);

        private int nextTransactionID = new Random().Next(int.MaxValue - 2) + 1;

        private bool checkOutgoingMsg = true;

        public event CommandResponder OnIncomingPdu;
        public delegate void CounterIncrementHandler(object o, CounterIncrArgs args);
        public event CounterIncrementHandler OnIncrementCounter;
        public delegate void AuthenticationFailureHandler(object o, AuthenticationFailureArgs args);
        public event AuthenticationFailureHandler OnAuthenticationFailure;

        /**
         * Default constructor creates a message dispatcher without any associated
         * message processing models.
         */
        public MessageDispatcherImpl()
        {
        }

        /**
         * Adds a message processing model to this message dispatcher. If a message
         * processing model with the same ID as the supplied one already exists it
         * will not be changed. Please call {@link #removeMessageProcessingModel}
         * before to replace a message processing model.
         * @param model
         *    a MessageProcessingModel instance.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessageProcessingModel(MessageProcessingModel model)
        {
            while (mpm.Count <= (int)model.MessageProcModelId)
            {
                mpm.Add(null);
            }
            if (mpm.get(model.getID()) == null)
            {
                mpm.set(model.getID(), model);
            }
        }

        /**
         * Removes a message processing model from this message dispatcher.
         * @param model
         *    a previously added MessageProcessingModel instance.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveMessageProcessingModel(MessageProcessingModel model)
        {
            mpm.set(model.getID(), null);
        }

        /**
         * Adds a transport mapping. When an outgoing message is processed where
         * no specific transport mapping has been specified, then the
         * message dispatcher will use the transport mapping
         * that supports the supplied address class of the target.
         * @param transport
         *    a TransportMapping instance. If there is already another transport
         *    mapping registered that supports the same address class, then
         *    <code>transport</code> will be registered but not used for messages
         *    without specific transport mapping.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddTransportMapping(ITransportMapping<IAddress> transport)
        {
            List<ITransportMapping<IAddress>> transports =
                    transportMappings[transport.SupportedAddressClass];
            if (transports == null)
            {
                transports = new List<ITransportMapping<IAddress>>();
                transportMappings.Add(transport.SupportedAddressClass, transports);
            }
            transports.Add(transport);
        }

        /**
         * Removes a transport mapping.
         * @param transport
         *    a previously added TransportMapping instance.
         * @return
         *    the supplied TransportMapping if it has been successfully removed,
         *    <code>null</code>otherwise.
         */
        public ITransportMapping<IAddress> RemoveTransportMapping(ITransportMapping<IAddress> transport)
        {
            List<ITransportMapping<IAddress>> tm =
                    transportMappings.Remove(transport.SupportedAddressClass);
            if (tm != null)
            {
                if (tm.Remove(transport))
                {
                    return transport;
                }
            }
            return null;
        }

        /**
         * Gets a collection of all registered transport mappings.
         * @return
         *    a Collection instance.
         */
        public IEnumerable<ITransportMapping<IAddress>> TransportMappings
        {
            get
            {
                List<ITransportMapping<IAddress>> l = new List<ITransportMapping<IAddress>>(transportMappings.size());
                lock (transportMappings)
                {
                    foreach (List<ITransportMapping<IAddress>> tm in transportMappings.Values)
                    {
                        l.AddRange(tm);
                    }
                }
                return l;
            }
        }

        public int NextRequestID
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                int nextID = nextTransactionID++;
                if (nextID <= 0)
                {
                    nextID = 1;
                    nextTransactionID = 2;
                }
                return nextID;
            }
        }

        protected PduHandle CreatePduHandle()
        {
            return new PduHandle(GetNextRequestID());
        }

        /**
         * Sends a message using the <code>TransportMapping</code> that has been
         * assigned for the supplied address type.
         *
         * @param transport
         *    the transport mapping to be used to send the message.
         * @param destAddress
         *    the transport address where to send the message. The
         *    <code>destAddress</code> must be compatible with the supplied
         *    <code>transport</code>.
         * @param message
         *    the SNMP message to send.
         * @throws IOException
         *    if an I/O error occurred while sending the message or if there is
         *    no transport mapping defined for the supplied address type.
         */
        protected void SendMessage(ITransportMapping<IAddress> transport,
                                   IAddress destAddress, byte[] message,
                                   TransportStateReference tmStateReference)
        {

            if (transport != null)
            {
                if (destAddress is GenericAddress)
                {
                    transport.SendMessage(((GenericAddress)destAddress).Address, message, tmStateReference);
                }
                else
                {
                    transport.SendMessage(destAddress, message, tmStateReference);
                }
            }
            else
            {
                string txt = "No transport mapping for address class: " +
                             nameof(destAddress) + "=" + destAddress;
                log.Error(txt);
                throw new IOException(txt);
            }
        }

        /**
         * Returns a transport mapping that can handle the supplied address.
         * @param destAddress
         *    an Address instance.
         * @return
         *    a <code>TransportMapping</code> instance that can be used to sent
         *    a SNMP message to <code>destAddress</code> or <code>null</code> if
         *    such a transport mapping does not exists.
         * @since 1.6
         */
        public ITransportMapping<IAddress> GetTransport(IAddress destAddress)
        {
            Type addressClass = destAddress.GetType();
            do
            {
                List<ITransportMapping<IAddress>> l = transportMappings.get(addressClass);
                if ((l != null) && (l.size() > 0))
                {
                    return l.get(0);
                }
            }
            //TODO: WTF???
            while ((addressClass = addressClass.getSuperclass()) != null);
            return null;
        }

        /**
         * Actually decodes and dispatches an incoming SNMP message using the supplied
         * message processing model.
         *
         *
         * @param sourceTransport
         *   a <code>TransportMapping</code> that matches the incomingAddress type.
         * @param mp
         *   a <code>MessageProcessingModel</code> to process the message.
         * @param incomingAddress
         *   the <code>Address</code> from the entity that sent this message.
         * @param wholeMessage
         *   the <code>BERInputStream</code> containing the SNMP message.
         * @param tmStateReference
         *   the transport model state reference as defined by RFC 5590.
         * @throws IOException
         *   if the message cannot be decoded.
         */
        protected void DispatchMessage(ITransportMapping<IAddress> sourceTransport,
                                       MessageProcessingModel mp,
                                       IAddress incomingAddress,
                                       BERInputStream wholeMessage,
                                       TransportStateReference tmStateReference)
        {
            MutablePDU pdu = new MutablePDU();
            Integer32 messageProcessingModel = new Integer32();
            Integer32 securityModel = new Integer32();
            OctetString securityName = new OctetString();
            SecurityLevel securityLevel = new Integer32();

            PduHandle handle = CreatePduHandle();

            Integer32 maxSizeRespPDU =
                new Integer32(sourceTransport.MaxInboundMessageSize);
            StatusInformation statusInfo = new StatusInformation();
            MutableStateReference mutableStateReference = new MutableStateReference();
            // add the transport mapping to the state reference to allow the MP to
            // return REPORTs on the same interface/port the message had been received.
            StateReference stateReference = new StateReference();
            stateReference.TransportMapping = sourceTransport;
            stateReference.Address = incomingAddress;
            mutableStateReference.StateReference = stateReference;

            int status = mp.PrepareDataElements(this, incomingAddress, wholeMessage,
                                                tmStateReference,
                                                messageProcessingModel, securityModel,
                                                securityName, securityLevel, pdu,
                                                handle, maxSizeRespPDU, statusInfo,
                                                mutableStateReference);
            if (mutableStateReference.StateReference != null)
            {
                // make sure transport mapping is set
                mutableStateReference.StateReference.TransportMapping = sourceTransport;
            }
            if (status == SnmpConstants.SNMP_ERROR_SUCCESS)
            {
                // dispatch it
                CommandResponderArgs e =
                    new CommandResponderArgs(this,
                                              sourceTransport,
                                              incomingAddress,
                                              messageProcessingModel.GetValue(),
                                              securityModel.GetValue(),
                                              securityName.GetValue(),
                                              securityLevel,
                                              handle,
                                              pdu.Pdu,
                                              maxSizeRespPDU.GetValue(),
                                              mutableStateReference.StateReference);

                CounterIncrArgs responseTimeEvent = null;
                if (JunoSnmpSettings.JunoSnmpStatisticsLevel != JunoSnmpSettings.JunoSnmpStatistics.none)
                {
                    responseTimeEvent = new CounterIncrArgs(SnmpConstants.snmp4jStatsResponseProcessTime,
                        DateTime.Now.Ticks, incomingAddress);
                }

                OnIncomingPdu(this, e);

                if (responseTimeEvent != null)
                {
                    long increment = (DateTime.Now.Ticks - responseTimeEvent.Increment) / SnmpConstants.MILLISECOND_TO_NANOSECOND;
                    responseTimeEvent.Increment = increment;
                    OnIncrementCounter(this, responseTimeEvent);
                }
            }
            else
            {
                switch (status)
                {
                    case SnmpConstants.SNMP_MP_UNSUPPORTED_SECURITY_MODEL:
                    case SnmpConstants.SNMPv3_USM_AUTHENTICATION_FAILURE:
                    case SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL:
                    case SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME:
                    case SnmpConstants.SNMPv3_USM_AUTHENTICATION_ERROR:
                    case SnmpConstants.SNMPv3_USM_NOT_IN_TIME_WINDOW:
                    case SnmpConstants.SNMPv3_USM_UNSUPPORTED_AUTHPROTOCOL:
                    case SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID:
                    case SnmpConstants.SNMP_MP_WRONG_USER_NAME:
                    case SnmpConstants.SNMPv3_TSM_INADEQUATE_SECURITY_LEVELS:
                    case SnmpConstants.SNMP_MP_USM_ERROR:
                        {
                            AuthenticationFailureArgs evt = new AuthenticationFailureArgs(
                                incomingAddress,
                                sourceTransport,
                                status,
                                wholeMessage);
                            OnAuthenticationFailure(this, evt);
                            break;
                        }
                }

                log.Warn("statusInfo=" + statusInfo + ", status=" + status);
            }
        }

        public void ProcessMessage(ITransportMapping<IAddress> sourceTransport,
                                   IAddress incomingAddress,
                                   Stream wholeMessage,
                                   TransportStateReference tmStateReference)
        {
            ProcessMessage(sourceTransport, incomingAddress,
                new BERInputStream(wholeMessage),
                tmStateReference);
        }

        public void ProcessMessage(ITransportMapping<IAddress> sourceTransport,
                                   IAddress incomingAddress,
                                   BERInputStream wholeMessage,
                                   TransportStateReference tmStateReference)
        {
            OnIncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmpInPkts));
            if (!wholeMessage.markSupported())
            {
                string txt = "Message stream must support marks";
                log.Error(txt);
                throw new ArgumentException(txt);
            }
            try
            {
                wholeMessage.mark(16);
                BER.MutableByte type = new BER.MutableByte();
                // decode header but do not check length here, because we do only decode
                // the first 16 bytes.
                BER.DecodeHeader(wholeMessage, out type, false);
                if (type.Value != BER.SEQUENCE)
                {
                    log.Error("ASN.1 parse error (message is not a sequence)");
                    CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpInASNParseErrs);
                    OnIncrementCounter(this, evt);
                }
                Integer32 version = new Integer32();
                version.DecodeBER(wholeMessage);
                MessageProcessingModel mp = GetMessageProcessingModel(version.GetValue());
                if (mp == null)
                {
                    log.Warn("SNMP version " + version + " is not supported");
                    CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpInBadVersions);
                    OnIncrementCounter(this, evt);
                }
                else
                {
                    // reset it
                    wholeMessage.reset();
                    // dispatch it
                    DispatchMessage(sourceTransport, mp, incomingAddress, wholeMessage, tmStateReference);
                }
            }
            catch (IOException iox)
            {
                log.Debug(iox.StackTrace);
                log.Warn(iox);
                CounterIncrArgs evt =
                    new CounterIncrArgs(SnmpConstants.snmpInvalidMsgs);
                OnIncrementCounter(this, evt);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (log.IsDebugEnabled)
                {
                    log.Debug(ex.StackTrace);
                }
                if (JunoSnmpSettings.ForwardRuntimeExceptions)
                {
                    throw new RuntimeException(ex);
                }
            }
            catch (OutOfMemoryError oex)
            {
                log.Error(oex);
                if (JunoSnmpSettings.ForwardRuntimeExceptions)
                {
                    throw oex;
                }
            }
        }

        public PduHandle SendPdu(ITarget target,
                                 PDU pdu,
                                 bool expectResponse)
        {
            return SendPdu(null, target, pdu, expectResponse);
        }

        public PduHandle SendPdu(ITransportMapping<IAddress> transport,
                                 ITarget target,
                                 PDU pdu,
                                 bool expectResponse,
                                 IPduHandleCallback<PDU> pduHandleCallback)
        {
            int messageProcessingModel = target.Version;
            IAddress transportAddress = target.Address;
            SecurityModel.SecurityModelID securityModel = target.SecurityModel;
            SecurityLevel securityLevel = target.SecurityLevel;
            try
            {
                byte[] securityName = target.SecurityName.GetValue();
                MessageProcessingModel mp =
                    GetMessageProcessingModel(messageProcessingModel);
                if (mp == null)
                {
                    throw new MessageException("Unsupported message processing model: "
                                               + messageProcessingModel, SnmpConstants.SNMP_MD_UNSUPPORTED_MP_MODEL);
                }
                if (!mp.IsProtocolVersionSupported(messageProcessingModel))
                {
                    throw new MessageException("SNMP version " + messageProcessingModel +
                                               " is not supported " +
                                               "by message processing model " +
                                               messageProcessingModel, SnmpConstants.SNMP_MD_UNSUPPORTED_SNMP_VERSION);
                }
                if (transport == null)
                {
                    transport = GetTransport(transportAddress);
                }
                if (transport == null)
                {
                    throw new UnsupportedAddressClassException(
                        "Unsupported address class (transport mapping): " +
                        transportAddress.getClass().getName(),
                        transportAddress.getClass());
                }
                else if (pdu.IsConfirmedPdu)
                {
                    CheckListening4ConfirmedPDU(pdu, target.Address, transport);
                }

                // check if contextEngineID discovery is needed


                // check PDU type
                CheckOutgoingMsg(transportAddress, messageProcessingModel, pdu);

                // if request ID is == 0 then create one here, otherwise use the request
                // ID because it may be a resent request.
                PduHandle pduHandle;
                Integer32 reqID = pdu.RequestID;
                if (((reqID == null) || (reqID.GetValue() == 0)) &&
                    (pdu.Type != PDU.RESPONSE))
                {
                    pduHandle = CreatePduHandle();
                }
                else
                {
                    pduHandle = new PduHandle(pdu.RequestID.GetValue());
                }

                // assign request ID
                if (pdu.Type != PDU.V1TRAP)
                {
                    pdu.RequestID = new Integer32(pduHandle.TransactionID);
                }

                // parameters to receive
                GenericAddress destAddress = new GenericAddress();

                CertifiedIdentity certifiedIdentity = null;
                if (target is CertifiedIdentity)
                {
                    certifiedIdentity = (CertifiedIdentity)target;
                }
                TransportStateReference tmStateReference =
                    new TransportStateReference(transport,
                                                transportAddress,
                                                new OctetString(securityName),
                                                SecurityLevel.Get(securityLevel),
                                                SecurityLevel.Undefined,
                                                false, null, certifiedIdentity);

                if (pdu.IsConfirmedPdu)
                {
                    configureAuthoritativeEngineID(target, mp);
                }
                BEROutputStream outgoingMessage = new BEROutputStream();
                int status = mp.PrepareOutgoingMessage(transportAddress,
                                                       transport.MaxInboundMessageSize,
                                                       messageProcessingModel,
                                                       securityModel,
                                                       securityName,
                                                       securityLevel,
                                                       pdu,
                                                       expectResponse,
                                                       pduHandle,
                                                       destAddress,
                                                       outgoingMessage,
                                                       tmStateReference);

                if (status == SnmpConstants.SNMP_ERROR_SUCCESS)
                {
                    // inform callback about PDU new handle
                    if (pduHandleCallback != null)
                    {
                        pduHandleCallback.PduHandleAssigned(pduHandle, pdu);
                    }
                    byte[] messageBytes = outgoingMessage.GetBuffer().ToArray();
                    SendMessage(transport, transportAddress, messageBytes, tmStateReference);
                }
                else
                {
                    throw new MessageException("Message processing model " +
                                               mp.MessageProcModelId + " returned error: " +
                                               SnmpConstants.MpErrorMessage(status), status);
                }
                return pduHandle;
            }
            catch (IndexOutOfBoundsException iobex)
            {
                throw new MessageException("Unsupported message processing model: "
                                           + messageProcessingModel, SnmpConstants.SNMP_MD_UNSUPPORTED_MP_MODEL, iobex);
            }
            catch (MessageException mex)
            {
                if (log.IsDebugEnabled)
                {
                    mex.printStackTrace();
                }
                throw mex;
            }
            catch (IOException iox)
            {
                if (log.IsDebugEnabled)
                {
                    iox.printStackTrace();
                }
                throw new MessageException(iox.getMessage(), SnmpConstants.SNMP_MD_ERROR, iox);
            }
        }

        protected void ConfigureAuthoritativeEngineID(ITarget target, MessageProcessingModel mp)
        {
            if ((target is UserTarget) && (mp is MPv3))
            {
                UserTarget userTarget = (UserTarget)target;
                if ((userTarget.AuthoritativeEngineID!= null) && (userTarget.AuthoritativeEngineID.Length > 0))
                {
                    ((MPv3)mp).AddEngineID(target.Address, new OctetString(userTarget.AuthoritativeEngineID));
                }
            }
        }

        private static void CheckListening4ConfirmedPDU(PDU pdu, IAddress target,
                                                        ITransportMapping<IAddress> transport)
        {
            if ((transport != null) && (!transport.IsListening))
            {
                log.Warn("Sending confirmed PDU " + pdu + " to target " + target +
                            " although transport mapping " + transport +
                            " is not listening for a response");
            }
        }

        /**
         * Checks outgoing messages for consistency between PDU and target used.
         * @param transportAddress
         *    the target address.
         * @param messageProcessingModel
         *    the message processing model to be used.
         * @param pdu
         *    the PDU to be sent.
         * @throws MessageException
         *    if unrecoverable inconsistencies have been detected.
         */
        protected void CheckOutgoingMsg(IAddress transportAddress,
                                        int messageProcessingModel, PDU pdu)
        {
            if (checkOutgoingMsg)
            {
                if (messageProcessingModel == MessageProcessingModel.MessageProcessingModels.MPv1 || JunoSnmpSettings.NoGetBulk)
                {
                    if (pdu.Type == PDU.GETBULK)
                    {
                        if (messageProcessingModel == MessageProcessingModel.MessageProcessingModels.MPv1)
                        {
                            log.Warn("Converting GETBULK PDU to GETNEXT for SNMPv1 target: " + transportAddress);
                        }
                        else
                        {
                            log.Info("Converting GETBULK PDU to GETNEXT for target: " + transportAddress);
                        }
                        pdu.Type = PDU.GETNEXT;
                        if (!(pdu is PDUv1))
                        {
                            pdu.MaxRepetitions = 1;
                            pdu.NonRepeaters = 0;
                        }
                    }
                }
            }
        }

        public int ReturnResponsePdu(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                     SecurityModel.SecurityModelID securityModel,
                                     byte[] securityName,
                                     SecurityLevel securityLevel,
                                     PDU pdu,
                                     int maxSizeResponseScopedPDU,
                                     StateReference stateReference,
                                     StatusInformation statusInformation)
        {
            try
            {
                MessageProcessingModel mp =
                    GetMessageProcessingModel(messageProcessingModel);
                if (mp == null)
                {
                    throw new MessageException("Unsupported message processing model: "
                                               + messageProcessingModel, SnmpConstants.SNMP_MD_UNSUPPORTED_MP_MODEL);
                }
                ITransportMapping<IAddress> transport = stateReference.TransportMapping;
                if (transport == null)
                {
                    transport = GetTransport(stateReference.Address);
                }
                if (transport == null)
                {
                    throw new MessageException("Unsupported address class (transport mapping): " +
                                               stateReference.Address.GetType().Name,
                                              SnmpConstants.SNMP_MD_UNSUPPORTED_ADDRESS_CLASS);
                }
                BEROutputStream outgoingMessage = new BEROutputStream();
                int status = mp.PrepareResponseMessage(messageProcessingModel,
                                                       transport.MaxInboundMessageSize,
                                                       securityModel,
                                                       securityName, securityLevel, pdu,
                                                       maxSizeResponseScopedPDU,
                                                       stateReference,
                                                       statusInformation,
                                                       outgoingMessage);
                if (status == SnmpConstants.SNMP_MP_OK)
                {
                    TransportStateReference tmStateReference = null;
                    if (stateReference.SecurityStateReference is TsmSecurityStateReference)
                    {
                        tmStateReference = ((TsmSecurityStateReference)
                            stateReference.SecurityStateReference).TmStateReference;
                    }

                    SendMessage(transport,
                                stateReference.Address,
                                outgoingMessage.GetBuffer().ToArray(),
                                tmStateReference);
                }

                return status;
            }
            catch (ArrayIndexOutOfBoundsException aex)
            {
                throw new MessageException("Unsupported message processing model: "
                                           + messageProcessingModel, SnmpConstants.SNMP_MD_UNSUPPORTED_MP_MODEL, aex);
            }
            catch (IOException iox)
            {
                throw new MessageException(iox.Message, SnmpConstants.SNMP_MD_ERROR, iox);
            }
        }

        public void ReleaseStateReference(MessageProcessingModel.MessageProcessingModels messageProcessingModel,
                                          PduHandle pduHandle)
        {
            MessageProcessingModel mp = GetMessageProcessingModel(messageProcessingModel);
            if (mp == null)
            {
                throw new ArgumentException("Unsupported message processing model: " +
                                                   messageProcessingModel);
            }

            mp.ReleaseStateReference(pduHandle);
        }

        /**
         * Fires a <code>CommandResponderEvent</code>. Listeners are called
         * in order of their registration  until a listener has processed the
         * PDU successfully.
         * @param e
         *   a <code>CommandResponderEvent</code> event.
         */
        protected void FireProcessPdu(CommandResponderArgs e)
        {
            OnIncomingPdu(this, e);
        }

        /**
         * Gets the <code>MessageProcessingModel</code> for the supplied message
         * processing model ID.
         *
         * @param messageProcessingModel
         *    a message processing model ID
         *    (see {@link MessageProcessingModel#getID()}).
         * @return
         *    a MessageProcessingModel instance if the ID is known, otherwise
         *    <code>null</code>
         */
        public MessageProcessingModel GetMessageProcessingModel(int messageProcessingModel)
        {
            try
            {
                return mpm[messageProcessingModel];
            }
            catch (IndexOutOfBoundsException iobex)
            {
                return null;
            }
        }

        /**
         * Fires a counter incrementation event.
         * @param event
         *    the <code>CounterEvent</code> containing the OID of the counter
         *    that needs to be incremented.
         */
        protected void FireIncrementCounter(CounterIncrArgs evt)
        {
            OnIncrementCounter(this, evt);
        }

        /**
         * Returns whether consistency checks for outgoing messages are activated.
         * @return
         *    if <code>true</code> outgoing messages are checked for consistency.
         *    If <code>false</code>, no checks are performed.
         */

        /**
         * Enables or disables the consistency checks for outgoing messages.
         * If the checks are enabled, then GETBULK messages sent to SNMPv1
         * targets will be converted to GETNEXT messages.
         * <p>
         * In general, if an automatically conversion is not possible, an
         * error is thrown when such a message is to be sent.
         * <p>
         * The default is consistency checks enabled.
         *
         * @param checkOutgoingMsg
         *    if <code>true</code> outgoing messages are checked for consistency.
         *    Currently, only the PDU type will be checked against the used SNMP
         *    version. If <code>false</code>, no checks will be performed.
         */
        public bool CheckOutgoingMsgFlag
        {
            get
            {
                return checkOutgoingMsg;
            }

            set
            {
                this.checkOutgoingMsg = value;
            }
        }


        /**
         * Fires an <code>AuthenticationFailureEvent</code> to all registered
         * listeners.
         * @param event
         *    the event to fire.
         */
        protected void FireAuthenticationFailure(AuthenticationFailureArgs evt)
        {
            OnAuthenticationFailure(this, evt);
        }

        public PduHandle SendPdu(ITransportMapping<IAddress> transportMapping,
                                 ITarget target,
                                 PDU pdu,
                                 bool expectResponse)
        {
            return SendPdu(transportMapping, target, pdu, expectResponse, null);
        }
    }
}
