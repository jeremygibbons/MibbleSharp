// <copyright file="USM.cs" company="None">
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

namespace JunoSnmp.Security
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;
    /**
 * The <code>USM</code> class implements the User Based Security Model (USM)
 * as defined in RFC 3414.
 * <p>
 * When a user is added or removed from the USM, a <code>UsmUserEvent</code>
 * is fired and forwarded to registered listeners.
 *
 * @author Frank Fock
 * @version 2.0
 */
    public class USM : SNMPv3SecurityModel
    {

        private static readonly int MAXLEN_USMUSERNAME = 32;

        private static readonly log4net.ILog log = log4net.LogManager
              .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // Table containing localized and non-localized users
        private UsmUserTable userTable;

        private UsmTimeTable timeTable;

        private bool engineDiscoveryEnabled = true;

        private SecurityProtocols securityProtocols;

        private CounterSupport counterSupport;

        public delegate void UsmUserChangeHandler(object o, UsmUserChangeEventArgs args);

        public event UsmUserChangeHandler OnAddUser;
        public event UsmUserChangeHandler OnChangeUser;
        public event UsmUserChangeHandler OnRemoveUser;

        /**
         * Creates a USM with the support for the supplied security protocols.
         *
         * @param securityProtocols
         *    the security protocols to support.
         * @param localEngineID
         *    the local engine ID.
         * @param engineBoots
         *    the number of engine boots.
         * @since 1.2
         */
        public USM(SecurityProtocols securityProtocols,
                   OctetString localEngineID, int engineBoots)
        {
            this.localEngineID = localEngineID;
            timeTable = new UsmTimeTable(localEngineID, engineBoots);
            userTable = new UsmUserTable();
            this.securityProtocols = securityProtocols;
            counterSupport = CounterSupport.Instance;
        }

        /**
         * Default constructor with random engine ID with the default enterprise ID and a zero engineBoots counter.
         * The security protocols instance defined by {@link org.snmp4j.security.SecurityProtocols#getInstance()} with the
         * default protocols is used.
         * @since 2.2.4
         */
        public USM() : this(SecurityProtocols.GetInstance().AddDefaultProtocols(),
                new OctetString(MPv3.CreateLocalEngineID(randomID())), 0)
        {
        }

        private static OctetString randomID()
        {
            Random random = new Random();
            byte[] randomID = new byte[8];
            random.NextBytes(randomID);
            return new OctetString(randomID);
        }

        public override SecurityModel.SecurityModelID ID
        {
            get
            {
                return SecurityModel.SecurityModelID.SECURITY_MODEL_USM;
            }
        }

        public override bool SupportsEngineIdDiscovery
        {
            get
            {
                return true;
            }
        }

        public override bool HasAuthoritativeEngineID
        {
            get
            {
                return true;
            }
        }

        /**
         * Sets the local engine ID, number of boots, and time after boot.
         * @param localEngineID
         *    the local engine ID.
         * @param engineBoots
         *    the number of engine boots.
         * @param engineTime
         *    the number sendonds since the last boot.
         */
        public void SetLocalEngine(OctetString localEngineID,
                                   int engineBoots, int engineTime)
        {
            this.localEngineID = localEngineID;
            timeTable.setLocalTime(new UsmTimeEntry(localEngineID, engineBoots,
                                                    engineTime));
        }

        /**
         * Returns the number of engine boots counted for the local engine ID.
         * @return
         *    the number of engine boots (zero based).
         */

        /**
         * Sets the number of engine boots.
         * @param engineBoots
         *    the number of engine boots.
         */
        public int EngineBoots
        {
            get
            {
                return this.timeTable.EngineBoots;
            }

            set
            {
                this.timeTable.EngineBoots = value;
            }
        }

        /**
         * Returns the number of seconds since the value of
         * the engineBoots object last changed. When incrementing this object's value
         * would cause it to exceed its maximum, engineBoots is incremented as if a
         * re-initialization had occurred, and this
         * object's value consequently reverts to zero.
         *
         * @return
         *    a positive integer value denoting the number of seconds since
         *    the engineBoots value has been changed.
         * @since 1.2
         */
        public int EngineTime
        {
            get
            {
                return this.timeTable.EngineTime;
            }
        }

        public override ISecurityParameters NewSecurityParametersInstance()
        {
            return new UsmSecurityParameters();
        }

        public override ISecurityStateReference NewSecurityStateReference()
        {
            return new UsmSecurityStateReference();
        }

        public override int GenerateRequestMessage(MessageProcessingModel.MessageProcessingModels snmpVersion,
                                          byte[] globalData,
                                          int maxMessageSize,
                                          SecurityModel.SecurityModelID securityModel,
                                          byte[] securityEngineID,
                                          byte[] securityName,
                                          SecurityLevel securityLevel,
                                          BERInputStream scopedPDU,
                                          ISecurityParameters securityParameters,
                                          BEROutputStream wholeMsg,
                                          TransportStateReference tmStateReference)
        {

            return GenerateResponseMessage(snmpVersion,
                                           globalData,
                                           maxMessageSize,
                                           securityModel,
                                           securityEngineID,
                                           securityName,
                                           securityLevel,
                                           scopedPDU,
                                           null,
                                           securityParameters,
                                           wholeMsg);
        }

        /**
         * Checks if the specified user is known by this USM.
         * @param engineID
         *   the engineID of the user (may be <code>null</code> if any target should
         *   match).
         * @param securityName
         *   the security name of the user to earch for.
         * @return
         *   <code>true</code> if the user is either known for the specified engine ID
         *   or without a specific engine ID (discovery only).
         * @since
         */
        public bool HasUser(OctetString engineID, OctetString securityName)
        {
            UsmUserEntry entry = userTable.GetUser(engineID, securityName);
            if (entry == null)
            {
                entry = userTable.GetUser(securityName);
                if ((entry == null) && (securityName.Length > 0))
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Looks up a {@link org.snmp4j.security.UsmUserEntry} by an engine ID and
         * security name. If an user exists that is not localized for the provided
         * engine ID, it will be localized and then the localized user entry is
         * returned. If the provided engine ID has a zero length then an empty
         * {@link org.snmp4j.security.UsmUserEntry} is returned with just the provided
         * securityName set.
         * @param engineID
         *    an engine ID.
         * @param securityName
         *    a security name.
         * @return
         *    an localized {@link org.snmp4j.security.UsmUserEntry} if the provided
         *    engineID's length is greater than zero and <code>null</code> if the
         *    securityName cannot be found in the USM.
         */
        public UsmUserEntry GetUser(OctetString engineID, OctetString securityName)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("getUser(engineID=" + engineID.ToHexString() +
                             ", securityName=" + securityName.ToString() + ")");
            }

            UsmUserEntry entry = userTable.GetUser(engineID, securityName);

            if (entry == null)
            {
                entry = userTable.GetUser(securityName);
                if ((entry == null) && (securityName.Length > 0))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("USM.getUser - User '" + securityName + "' unknown");
                    }

                    return null;
                }
                else
                {
                    if ((entry == null) || (engineID.Length == 0))
                    {
                        // do not add user
                        entry = new UsmUserEntry();
                        entry.UserName = securityName;
                        entry.UsmUser = new UsmUser(securityName, null, null, null, null);
                        return entry;
                    }
                    else
                    {
                        // add a new user
                        OID authProtocolOID = entry.UsmUser.AuthenticationProtocol;
                        OID privProtocolOID = entry.UsmUser.PrivacyProtocol;
                        if (authProtocolOID != null)
                        {
                            byte[] authKey;

                            if (entry.UsmUser.IsLocalized)
                            {
                                authKey =
                                    entry.UsmUser.AuthenticationPassphrase.GetValue();
                            }
                            else
                            {
                                authKey = securityProtocols.PasswordToKey(authProtocolOID,
                                    entry.UsmUser.AuthenticationPassphrase,
                                    engineID.GetValue());
                            }

                            byte[] privKey = null;

                            if (privProtocolOID != null)
                            {
                                if (entry.UsmUser.IsLocalized)
                                {
                                    privKey = entry.UsmUser.PrivacyPassphrase.GetValue();
                                }
                                else
                                {
                                    privKey = securityProtocols.PasswordToKey(privProtocolOID,
                                        authProtocolOID,
                                        entry.UsmUser.PrivacyPassphrase,
                                        engineID.GetValue());
                                }
                            }

                            entry = AddLocalizedUser(engineID.GetValue(), securityName,
                                                     authProtocolOID, authKey,
                                                     privProtocolOID, privKey);
                        }
                    }
                }
            }

            return entry;
        }

        public override int GenerateResponseMessage(MessageProcessingModel.MessageProcessingModels snmpVersion,
                                           byte[] globalData,
                                           int maxMessageSize,
                                           SecurityModel.SecurityModelID securityModel,
                                           byte[] securityEngineID,
                                           byte[] securityName,
                                           SecurityLevel securityLevel,
                                           BERInputStream scopedPDU,
                                           ISecurityStateReference securityStateReference,
                                           ISecurityParameters securityParameters,
                                           BEROutputStream wholeMsg)
        {
            UsmSecurityParameters usmSecurityParams = (UsmSecurityParameters)securityParameters;
            if (securityStateReference != null)
            {
                // this is a response or report
                UsmSecurityStateReference usmSecurityStateReference =
                    (UsmSecurityStateReference)securityStateReference;
                if (usmSecurityStateReference.SecurityEngineID == null)
                {
                    usmSecurityParams.AuthoritativeEngineID = securityEngineID;
                    usmSecurityStateReference.SecurityEngineID = securityEngineID;
                }
                if (usmSecurityStateReference.SecurityName == null)
                {
                    OctetString userName = new OctetString(securityName);
                    usmSecurityStateReference.SecurityName = userName.GetValue();
                    usmSecurityParams.UserName = userName;

                    OctetString secName =
                        getSecurityName(new OctetString(securityEngineID), userName);

                    if ((secName != null) &&
                        (secName.Length <= MAXLEN_USMUSERNAME))
                    {
                        usmSecurityParams.UserName = secName;
                    }

                }
                else
                {
                    usmSecurityParams.UserName = new OctetString(usmSecurityStateReference.SecurityName);
                }

                usmSecurityParams.AuthenticationProtocol = usmSecurityStateReference.AuthenticationProtocol;
                usmSecurityParams.PrivacyProtocol = usmSecurityStateReference.PrivacyProtocol;
                usmSecurityParams.AuthenticationKey = usmSecurityStateReference.AuthenticationKey;
                usmSecurityParams.PrivacyKey = usmSecurityStateReference.PrivacyKey;
            }
            else
            {
                OctetString secEngineID = new OctetString();
                if (securityEngineID != null)
                {
                    secEngineID.SetValue(securityEngineID);
                }

                OctetString secName = new OctetString(securityName);
                UsmUserEntry user = null;

                if (secEngineID.Length == 0)
                {
                    if (EngineDiscoveryEnabled)
                    {
                        if (HasUser(null, secName))
                        {
                            user = new UsmUserEntry();
                        }
                    }
                    else
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Engine ID unknown and discovery disabled");
                        }
                        return SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID;
                    }
                }
                else
                {
                    user = GetUser(secEngineID, secName);
                }
                if (user == null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Security name not found for engineID=" +
                                     secEngineID.ToHexString() + ", securityName=" +
                                     secName.ToHexString());
                    }

                    return SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME;
                }

                IAuthenticationProtocol auth =
                    securityProtocols.GetAuthenticationProtocol(user.UsmUser.AuthenticationProtocol);
                IPrivacyProtocol priv = securityProtocols.GetPrivacyProtocol(user.UsmUser.PrivacyProtocol);
                usmSecurityParams.AuthenticationProtocol = auth;
                usmSecurityParams.PrivacyProtocol = priv;
                usmSecurityParams.AuthenticationKey = user.AuthenticationKey;
                usmSecurityParams.PrivacyKey = user.PrivacyKey;
                usmSecurityParams.UserName = user.UsmUser.SecurityName;
                usmSecurityParams.AuthoritativeEngineID = secEngineID.GetValue();
            }

            // Check length of userName and engineID
            if (usmSecurityParams.AuthoritativeEngineID.Length > MPv3.MAXLEN_ENGINE_ID)
            {
                log.Error("Engine ID too long: " +
                             usmSecurityParams.AuthoritativeEngineID.Length + ">" +
                             MPv3.MAXLEN_ENGINE_ID + " for " +
                             new OctetString(usmSecurityParams.AuthoritativeEngineID)
                             .ToHexString());
                return SnmpConstants.SNMPv3_USM_ENGINE_ID_TOO_LONG;
            }

            if (securityName.Length > MAXLEN_USMUSERNAME)
            {
                log.Error("Security name too long: " +
                             usmSecurityParams.AuthoritativeEngineID.Length + ">" +
                             MAXLEN_USMUSERNAME + " for " +
                             new OctetString(securityName).ToHexString());
                return SnmpConstants.SNMPv3_USM_SECURITY_NAME_TOO_LONG;
            }

            if ((int)securityLevel >= (int)SecurityLevel.AuthNoPriv)
            {
                if (securityStateReference != null)
                {
                    // request or response
                    usmSecurityParams.AuthoritativeEngineBoots = EngineBoots;
                    usmSecurityParams.AuthoritativeEngineTime = EngineTime;
                }
                else
                {
                    // get engineBoots, engineTime
                    OctetString secEngineID = new OctetString(securityEngineID);
                    UsmTimeEntry entry = timeTable.getTime(secEngineID);
                    if (entry == null)
                    {
                        entry =
                            new UsmTimeEntry(secEngineID,
                                             usmSecurityParams.AuthoritativeEngineBoots,
                                             usmSecurityParams.AuthoritativeEngineTime);

                        timeTable.AddEntry(entry);
                    }
                    else
                    {
                        usmSecurityParams.AuthoritativeEngineBoots = entry.EngineBoots;
                        usmSecurityParams.AuthoritativeEngineTime = entry.
                            LatestReceivedTime;
                    }
                }
            }

            if (((int)securityLevel >= (int)SecurityLevel.AuthNoPriv) &&
                (usmSecurityParams.AuthenticationProtocol == null))
            {
                return SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL;
            }

            byte[] scopedPduBytes = BuildMessageBuffer(scopedPDU);

            if ((int)securityLevel == (int)SecurityLevel.AuthPriv)
            {
                if (usmSecurityParams.PrivacyProtocol == null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Unsupported security level (missing or unsupported privacy protocol): Security params are "
                            + usmSecurityParams);
                    }

                    return SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL;
                }

                log.Debug("RFC3414 §3.1.4.a Outgoing message needs to be encrypted");

                DecryptParams decryptParams = new DecryptParams();
                byte[] encryptedScopedPdu =
                    usmSecurityParams
                    .PrivacyProtocol
                    .Encrypt(
                        scopedPduBytes,
                        0,
                        scopedPduBytes.Length,
                        usmSecurityParams.PrivacyKey,
                        usmSecurityParams.AuthoritativeEngineBoots,
                        usmSecurityParams.AuthoritativeEngineTime,
                        decryptParams);

                if (encryptedScopedPdu == null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Encryption error");
                    }

                    return SnmpConstants.SNMPv3_USM_ENCRYPTION_ERROR;
                }

                usmSecurityParams.PrivacyParameters = new OctetString(decryptParams.Array);
                OctetString encryptedString = new OctetString(encryptedScopedPdu);
                using(BEROutputStream os = new BEROutputStream()) {
                    encryptedString.EncodeBER(os);
                    scopedPduBytes = os.ToArray();
                }
                
            }
            else
            {
                log.Debug("RFC3414 §3.1.4.b Outgoing message is not encrypted");
                usmSecurityParams.PrivacyParameters = new OctetString();
            }

            byte[] wholeMessage;

            if ((int)securityLevel >= (int)SecurityLevel.AuthNoPriv)
            {
                /* Build message with authentication */
                IAuthenticationProtocol authenticationProtocol = usmSecurityParams.AuthenticationProtocol;
                byte[] blank =
                    new byte[authenticationProtocol.AuthenticationCodeLength];
                usmSecurityParams.AuthenticationParameters = new OctetString(blank);
                wholeMessage =
                    BuildWholeMessage(new Integer32((int)snmpVersion),
                                      scopedPduBytes, globalData, usmSecurityParams);

                int authParamsPos =
                    usmSecurityParams.AuthParametersPosition +
                    usmSecurityParams.SecurityParametersPosition;

                bool authOK = usmSecurityParams
                    .AuthenticationProtocol
                    .Authenticate(
                    usmSecurityParams.AuthenticationKey,
                    wholeMessage,
                    0,
                    wholeMessage.Length,
                    new ByteArrayWindow(
                        wholeMessage,
                        authParamsPos,
                        authenticationProtocol.AuthenticationCodeLength));

                if (!authOK)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Outgoing message could not be authenticated");
                    }
                    return SnmpConstants.SNMPv3_USM_AUTHENTICATION_ERROR;
                }
            }
            else
            {
                // Set engineBoots and engineTime to zero!
                usmSecurityParams.AuthoritativeEngineBoots = 0;
                usmSecurityParams.AuthenticationParameters = new OctetString();
                usmSecurityParams.AuthoritativeEngineTime = 0;

                //build Message without authentication
                wholeMessage = BuildWholeMessage(
                    new Integer32((int)snmpVersion),
                    scopedPduBytes,
                    globalData,
                    usmSecurityParams);
            }

            wholeMsg.Write(wholeMessage, 0, wholeMessage.Length);
            
            return SnmpConstants.SNMPv3_USM_OK;
        }

        private OctetString getSecurityName(OctetString engineID,
                                            OctetString userName)
        {
            if (userName.Length == 0)
            {
                return userName;
            }

            UsmUserEntry user = userTable.GetUser(engineID, userName);

            if (user != null)
            {
                return user.UsmUser.SecurityName;
            }
            else if (EngineDiscoveryEnabled)
            {
                user = userTable.GetUser(userName);
                if (user != null)
                {
                    return user.UsmUser.SecurityName;
                }
            }

            return null;
        }

        public override int ProcessIncomingMsg(MessageProcessingModel.MessageProcessingModels snmpVersion, // typically, SNMP version
                                      int maxMessageSize, // of the sending SNMP entity - maxHeaderLength of the MP
                                      ISecurityParameters securityParameters, // for the received message
                                      SecurityModel.SecurityModelID securityModel, // for the received message
                                      SecurityLevel securityLevel, // Level of Security
                                      BERInputStream wholeMsg, // as received on the wire
                                      TransportStateReference tmStateReference,
                                      // output parameters
                                      OctetString securityEngineID, // authoritative SNMP entity
                                      OctetString securityName, // identification of the principal
                                      BEROutputStream scopedPDU, // message (plaintext) payload
                                      int maxSizeResponseScopedPDU, // maximum size of the Response PDU
                                      ISecurityStateReference securityStateReference, // reference to security state information, needed for response
                                      StatusInformation statusInfo
                                      )
        {
            UsmSecurityParameters usmSecurityParameters = (UsmSecurityParameters)securityParameters;
            UsmSecurityStateReference usmSecurityStateReference = (UsmSecurityStateReference)securityStateReference;
            securityEngineID.SetValue(usmSecurityParameters.AuthoritativeEngineID);

            byte[] message = BuildMessageBuffer(wholeMsg);

            if ((securityEngineID.Length == 0) ||
                (timeTable.CheckEngineID(securityEngineID,
                                         engineDiscoveryEnabled,
                                         usmSecurityParameters.AuthoritativeEngineBoots,
                                         usmSecurityParameters.AuthoritativeEngineTime) !=
                 SnmpConstants.SNMPv3_USM_OK))
            {
                // generate report
                if (log.IsDebugEnabled)
                {
                    log.Debug("RFC3414 §3.2.3 Unknown engine ID: " + securityEngineID.ToHexString());
                }

                securityEngineID.SetValue(usmSecurityParameters.AuthoritativeEngineID);
                securityName.SetValue(usmSecurityParameters.UserName.GetValue());

                if (statusInfo != null)
                {
                    CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsUnknownEngineIDs);
                    FireIncrementCounter(evt);

                    statusInfo.SecurityLevel = securityLevel;
                    statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                }

                return SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID;
            }

            securityName.SetValue(usmSecurityParameters.UserName.GetValue());

            int scopedPDUPosition = usmSecurityParameters.ScopedPduPosition;

            // get security name
            if ((usmSecurityParameters.UserName.Length > 0) ||
                ((int)securityLevel > (int)SecurityLevel.NoAuthNoPriv))
            {
                OctetString secName = getSecurityName(securityEngineID, usmSecurityParameters.UserName);
                if (secName == null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("RFC3414 §3.2.4 Unknown security name: " +
                                     securityName.ToHexString());
                    }
                    if (statusInfo != null)
                    {
                        CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsUnknownUserNames);
                        FireIncrementCounter(evt);

                        statusInfo.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                        statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                    }
                    return SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME;
                }
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Accepting zero length security name");
                }
                securityName.SetValue(new byte[0]);
            }

            if ((usmSecurityParameters.UserName.Length > 0) ||
                ((int)securityLevel > (int)SecurityLevel.NoAuthNoPriv))
            {
                UsmUserEntry user = GetUser(securityEngineID, securityName);
                if (user == null)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("RFC3414 §3.2.4 Unknown security name: " +
                                     securityName.ToHexString() + " for engine ID " +
                                     securityEngineID.ToHexString());
                    }

                    CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsUnknownUserNames);
                    FireIncrementCounter(evt);

                    if (statusInfo != null)
                    {
                        if (JunoSnmpSettings.ReportSecurityLevelStrategy ==
                            JunoSnmpSettings.ReportSecurityLevelOption.noAuthNoPrivIfNeeded)
                        {
                            statusInfo.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                        }

                        statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                    }

                    return SnmpConstants.SNMPv3_USM_UNKNOWN_SECURITY_NAME;
                }

                usmSecurityStateReference.UserName = user.UserName.GetValue();

                IAuthenticationProtocol auth =
                    securityProtocols.GetAuthenticationProtocol(
                                          user.UsmUser.AuthenticationProtocol);
                IPrivacyProtocol priv =
                    securityProtocols.GetPrivacyProtocol(
                                          user.UsmUser.PrivacyProtocol);

                usmSecurityStateReference.AuthenticationKey = user.AuthenticationKey;
                usmSecurityStateReference.PrivacyKey = user.PrivacyKey;
                usmSecurityStateReference.AuthenticationProtocol = auth;
                usmSecurityStateReference.PrivacyProtocol = priv;

                if ((((int)securityLevel >= (int)SecurityLevel.AuthNoPriv) && (auth == null)) ||
                    ((((int)securityLevel >= (int)SecurityLevel.AuthPriv) && (priv == null))))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("RFC3414 §3.2.5 - Unsupported security level: " +
                                     securityLevel + " by user " + user);
                    }

                    CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsUnsupportedSecLevels);
                    FireIncrementCounter(evt);

                    if (JunoSnmpSettings.ReportSecurityLevelStrategy ==
                        JunoSnmpSettings.ReportSecurityLevelOption.noAuthNoPrivIfNeeded)
                    {
                        statusInfo.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                    }

                    statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                    return SnmpConstants.SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL;
                }

                if ((int)securityLevel >= (int)SecurityLevel.AuthNoPriv)
                {
                    if (statusInfo != null)
                    {
                        int authParamsPos =
                            usmSecurityParameters.AuthParametersPosition +
                            usmSecurityParameters.SecurityParametersPosition;
                        bool authentic =
                            auth.IsAuthentic(user.AuthenticationKey,
                                             message, 0, message.Length,
                                             new ByteArrayWindow(message, authParamsPos,
                                             auth.AuthenticationCodeLength));
                        if (!authentic)
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.Debug(
                                    "RFC3414 §3.2.6 Wrong digest -> authentication failure: " +
                                    usmSecurityParameters.AuthenticationParameters.ToHexString());
                            }

                            CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsWrongDigests);
                            FireIncrementCounter(evt);

                            if (JunoSnmpSettings.ReportSecurityLevelStrategy ==
                                JunoSnmpSettings.ReportSecurityLevelOption.noAuthNoPrivIfNeeded)
                            {
                                statusInfo.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                            }

                            statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                            return SnmpConstants.SNMPv3_USM_AUTHENTICATION_FAILURE;
                        }

                        // check time
                        int status = timeTable.CheckTime(new UsmTimeEntry(securityEngineID,
                          usmSecurityParameters.AuthoritativeEngineBoots,
                          usmSecurityParameters.AuthoritativeEngineTime));

                        switch (status)
                        {
                            case SnmpConstants.SNMPv3_USM_NOT_IN_TIME_WINDOW:
                                {
                                    log.Debug("RFC3414 §3.2.7.a Not in time window; engineID='" +
                                                 securityEngineID +
                                                 "', engineBoots=" +
                                                 usmSecurityParameters.AuthoritativeEngineBoots +
                                                 ", engineTime=" +
                                                 usmSecurityParameters.AuthoritativeEngineTime);

                                    CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsNotInTimeWindows);
                                    FireIncrementCounter(evt);

                                    statusInfo.SecurityLevel = SecurityLevel.AuthNoPriv;
                                    statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                                    return status;
                                }
                            case SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID:
                                {
                                    if (log.IsDebugEnabled)
                                    {
                                        log.Debug("RFC3414 §3.2.7.b - Unknown engine ID: " +
                                                     securityEngineID);
                                    }

                                    CounterIncrEventArgs evt = new CounterIncrEventArgs(SnmpConstants.usmStatsUnknownEngineIDs);
                                    FireIncrementCounter(evt);
                                    if (JunoSnmpSettings.ReportSecurityLevelStrategy ==
                                        JunoSnmpSettings.ReportSecurityLevelOption.noAuthNoPrivIfNeeded)
                                    {
                                        statusInfo.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                                    }

                                    statusInfo.ErrorIndication = new VariableBinding(evt.Oid, evt.CurrentValue);
                                    return status;

                                }
                        }
                    }

                    if ((int)securityLevel >= (int)SecurityLevel.AuthPriv)
                    {
                        OctetString privParams = usmSecurityParameters.PrivacyParameters;
                        DecryptParams decryptParams = new DecryptParams(privParams.GetValue(),
                                                                        0, privParams.Length);
                        try
                        {
                            int scopedPDUHeaderLength = message.Length - scopedPDUPosition;

                            MemoryStream bis = new MemoryStream(message, (int)scopedPDU.Position, scopedPDUHeaderLength);
                            
                            BERInputStream scopedPDUHeader = new BERInputStream(bis.ToArray());
                            long headerStartingPosition = scopedPDUHeader.Position;
                            byte type;
                            int scopedPDULength = BER.DecodeHeader(scopedPDUHeader, out type);
                            int scopedPDUPayloadPosition = scopedPDUPosition +
                                  (int)(scopedPDUHeader.Position - headerStartingPosition);
                            scopedPDUHeader.Close();
                            // early release pointer:
                            scopedPDUHeader = null;
                            byte[] scopedPduBytes =
                                priv.Decrypt(message, scopedPDUPayloadPosition, scopedPDULength,
                                             user.PrivacyKey,
                                             usmSecurityParameters.AuthoritativeEngineBoots,
                                             usmSecurityParameters.AuthoritativeEngineTime,
                                             decryptParams);
                            scopedPDU.Write(scopedPduBytes, 0, scopedPduBytes.Length);
                        }
                        catch (Exception ex)
                        {
                            log.Debug("RFC 3414 §3.2.8 Decryption error: " + ex.Message);
                            return SnmpConstants.SNMPv3_USM_DECRYPTION_ERROR;
                        }
                    }
                    else
                    {
                        int scopedPduLength = message.Length - scopedPDUPosition;
                        scopedPDU.Write(message, scopedPDUPosition, scopedPduLength);
                    }
                }
                else
                {
                    int scopedPduLength = message.Length - scopedPDUPosition;
                    scopedPDU.Write(message, scopedPDUPosition, scopedPduLength);
                }
            }
            else
            {
                int scopedPduLength = message.Length - scopedPDUPosition;
                scopedPDU.Write(message, scopedPDUPosition, scopedPduLength);
            }

            // compute real max size response pdu according  to RFC3414 §3.2.9
            int maxSecParamsOverhead =
                usmSecurityParameters.GetBERMaxLength((int)securityLevel);

            maxSizeResponseScopedPDU = maxMessageSize -
                                              maxSecParamsOverhead;

            usmSecurityStateReference.SecurityName = securityName.GetValue();
            return SnmpConstants.SNMPv3_USM_OK;
        }

        protected void FireIncrementCounter(CounterIncrEventArgs e)
        {
            counterSupport.IncrementCounter(this, e);
        }

        /**
         * Adds an USM user to the internal user name table.
         * @param userName
         *    a user name.
         * @param user
         *    the <code>UsmUser</code> to add.
         */
        public void AddUser(OctetString userName, UsmUser user)
        {
            this.AddUser(userName, new OctetString(), user);
        }

        /**
         * Adds an USM user to the internal user name table.
         * The user's security name is used as userName.
         * @param user
         *    the <code>UsmUser</code> to add.
         * @since 2.0
         */
        public void AddUser(UsmUser user)
        {
            this.AddUser(user.SecurityName, new OctetString(), user);
        }

        /**
         * Adds an USM user to the internal user name table and associates it with
         * an authoritative engine ID. This user can only be used with the specified
         * engine ID - other engine IDs cannot be discovered on behalf of this entry.
         * <p>
         * The engine ID must be at least {@link MPv3#MINLEN_ENGINE_ID} bytes long and
         * not longer than {@link MPv3#MAXLEN_ENGINE_ID}.
         * </p>
         * The security name of the <code>user</code> must be not longer than {@link #MAXLEN_USMUSERNAME}
         * bytes.
         * @param userName
         *    a user name.
         * @param engineID
         *    the authoritative engine ID to be associated with this entry. If
         *    <code>engineID</code> is <code>null</code> this method behaves exactly
         *    like {@link #addUser(OctetString userName, UsmUser user)}.
         * @param user
         *    the <code>UsmUser</code> to add.
         * @throws
         *    IllegalArgumentException if (a) the length of the engine ID is less than
         *    {@link MPv3#MINLEN_ENGINE_ID} or more than {@link MPv3#MAXLEN_ENGINE_ID} bytes
         *    (b) if the security name of the <code>user</code> is longer than
         *    {@link #MAXLEN_USMUSERNAME}.
         */
        public void AddUser(OctetString userName, OctetString engineID, UsmUser user)
        {
            byte[] authKey = null;
            byte[] privKey = null;
            if (user.SecurityName.Length > MAXLEN_USMUSERNAME)
            {
                string txt = "User '" + user.SecurityName +
                    "' not added because of its too long security name with length " + user.SecurityName.Length;
                log.Warn(txt);
                throw new ArgumentException(txt);
            }
            if ((engineID != null) && (engineID.Length > 0))
            {
                if (engineID.Length < MPv3.MINLEN_ENGINE_ID || engineID.Length > MPv3.MAXLEN_ENGINE_ID)
                {
                    String txt = "User '" + userName +
                        "' not added because of an engine ID of incorrect length " + engineID.Length;
                    log.Warn(txt);
                    throw new ArgumentException(txt);
                }
                if (user.AuthenticationProtocol != null)
                {
                    if (user.IsLocalized)
                    {
                        authKey = user.AuthenticationPassphrase.GetValue();
                    }
                    else
                    {
                        authKey = securityProtocols.PasswordToKey(
                            user.AuthenticationProtocol,
                            user.AuthenticationPassphrase,
                            engineID.GetValue());
                    }

                    if (user.PrivacyProtocol != null)
                    {
                        if (user.IsLocalized)
                        {
                            privKey = user.PrivacyPassphrase.GetValue();
                        }
                        else
                        {
                            privKey = securityProtocols.PasswordToKey(
                                user.PrivacyProtocol,
                                user.AuthenticationProtocol,
                                user.PrivacyPassphrase,
                                engineID.GetValue());
                        }
                    }
                }
            }

            OctetString userEngineID;

            if (user.IsLocalized)
            {
                userEngineID = user.LocalizationEngineID;
            }
            else
            {
                userEngineID = (engineID == null) ? new OctetString() : engineID;
            }

            UsmUserEntry entry = new UsmUserEntry(userName, userEngineID, user);
            entry.AuthenticationKey = authKey;
            entry.PrivacyKey = privKey;
            userTable.addUser(entry);
            OnChangeUser(this, new UsmUserChangeEventArgs(entry, UsmUserChangeEventArgs.USER_ADDED));
        }

        /**
         * Updates the USM user entry with the same engine ID and user name as the
         * supplied instance and fires an appropriate <code>UsmUserEvent</code>.
         * If the corresponding user entry does not yet exist then it will be added.
         * @param entry
         *    an <code>UsmUserEntry</code> instance not necessarily the same as an
         *    already existing entry.
         * @since 1.2
         */
        public void UpdateUser(UsmUserEntry entry)
        {
            UsmUserEntry oldEntry = userTable.addUser(entry);
            if (oldEntry == null)
            {
                OnChangeUser(this, new UsmUserChangeEventArgs(entry, UsmUserChangeEventArgs.USER_ADDED));
            }
            else
            {
                OnChangeUser(this, new UsmUserChangeEventArgs(entry, UsmUserChangeEventArgs.USER_CHANGED));
            }
        }

        /**
         * Sets the users of this USM. All previously added users and all localized
         * user information will be discarded and replaced by the supplied users.
         *
         * @param users
         *    a possibly empty <code>UsmUser</code> array of users.
         * @since 1.1
         */
        public void SetUsers(UsmUser[] users)
        {
            if ((users == null) || (users.Length == 0))
            {
                userTable.Clear();
            }
            else
            {
                List<UsmUserEntry> v = new List<UsmUserEntry>(users.Length);
                foreach (UsmUser user in users)
                {
                    UsmUserEntry entry = new UsmUserEntry(user.SecurityName, (UsmUser)user.Clone());
                    v.Add(entry);
                }

                userTable.setUsers(v);
            }
        }

        /**
         * Returns the <code>UsmUserTable</code> instance used by the USM for local
         * storage of USM user information. The returned table should not be modified,
         * because modifications will not be reported to registered
         * <code>UsmUserListener</code>s.
         *
         * @return
         *    the <code>UsmUserTable</code> instance containing the users known by
         *    this USM.
         */
        public UsmUserTable UserTable
        {
            get
            {
                return userTable;
            }
        }

        /**
         * Returns the <code>UsmTimeTable</code> instance used by this USM for holding
         * timing information about the local and remote SNMP entities.
         *
         * @return UsmTimeTable
         * @since 1.6
         */
        public UsmTimeTable TimeTable
        {
            get
            {
                return timeTable;
            }
        }

        /**
         * Removes all USM user from the internal user name table with the specified user
         * name and (optional) engine ID. If the engine ID is not provided (null)
         * then any user (including localized) are removed that have the specified user name.
         *
         * @param userName
         *    a user name.
         * @param engineID
         *    the authoritative engine ID associated with the user by localization, or
         *    <code>null</code> if all users with <code>userName</code> should be
         *    deleted.
         * @return
         *    the removed <code>UsmUser</code> instances as a List. If the user could
         *    be found, an empty list is returned.
         * @since
         *    2.2
         */
        public List<UsmUser> RemoveAllUsers(OctetString userName, OctetString engineID)
        {
            IList<UsmUserEntry> entries = userTable.RemoveAllUsers(userName, engineID);
            if (entries.Count > 0)
            {
                List<UsmUser> users = new List<UsmUser>();
                foreach (UsmUserEntry entry in entries)
                {
                    users.Add(entry.UsmUser);
                    OnRemoveUser(this, new UsmUserChangeEventArgs(entry, UsmUserChangeEventArgs.USER_REMOVED));
                }

                return users;
            }

            return new List<UsmUser>();
        }

        /**
         * Removes all USM user from the internal user name table with the specified user
         * name. This is the same as {@link #removeAllUsers(org.snmp4j.smi.OctetString, org.snmp4j.smi.OctetString)}
         * with engineID set to <code>null</code>.
         *
         * @param userName
         *    a user name.
         * @return
         *    the removed <code>UsmUser</code> instances as a List. If the user could
         *    be found, an empty list is returned.
         * @since
         *    2.2
         */
        public List<UsmUser> RemoveAllUsers(OctetString userName)
        {
            return this.RemoveAllUsers(userName, null);
        }

        /**
         * Removes an USM user from the internal user name table.
         * @param engineID
         *    the authoritative engine ID associated with the user, or
         *    <code>null</code>
         * @param userName
         *    a user name.
         * @return
         *    the removed <code>UsmUser</code> instance associate with the given
         *    <code>userName</code> or <code>null</code> if such a user could not
         *    be found.
         * @deprecated
         *    If the engineID <code>null</code> is provided this method does only
         *    delete the generic user. All already localized users will not be deleted.
         *    To delete those users too, use {@link #removeAllUsers()} instead.
         */
        public UsmUser RemoveUser(OctetString engineID, OctetString userName)
        {
            UsmUserEntry entry = userTable.RemoveUser(engineID, userName);
            if (entry != null)
            {
                OnRemoveUser(this, new UsmUserChangeEventArgs(entry, UsmUserChangeEventArgs.USER_REMOVED));
                return entry.UsmUser;
            }

            return null;
        }

        /**
         * Removes all users from the USM.
         */
        public void RemoveAllUsers()
        {
            userTable.Clear();
            OnRemoveUser(this, new UsmUserChangeEventArgs(null, UsmUserChangeEventArgs.USER_REMOVED));
        }

        /**
         * Adds a localized user to the USM.
         * @param engineID
         *    the engine ID for which the user has been localized.
         * @param userName
         *    the user's name.
         * @param authProtocol
         *    the authentication protocol ID.
         * @param authKey
         *    the authentication key.
         * @param privProtocol
         *    the privacy protocol ID.
         * @param privKey
         *    the privacy key.
         * @return
         *    the added <code>UsmUserEntry</code>.
         */
        public UsmUserEntry AddLocalizedUser(byte[] engineID,
                                             OctetString userName,
                                             OID authProtocol, byte[] authKey,
                                             OID privProtocol, byte[] privKey)
        {
            UsmUserEntry newEntry = new UsmUserEntry(engineID, userName,
                                                     authProtocol, authKey,
                                                     privProtocol, privKey);
            userTable.addUser(newEntry);
            OnAddUser(this, new UsmUserChangeEventArgs(newEntry, UsmUserChangeEventArgs.USER_ADDED));
            return newEntry;
        }

        /**
         * Checks whether engine ID discovery is enabled or not. If enabled, the USM
         * will try to discover unknown engine IDs "on-the-fly" while processing the
         * message.
         * @return
         *    <code>true</code> if discovery is enabled, <code>false</code> otherwise.
         */
        public bool EngineDiscoveryEnabled
        {
            get
            {
                return engineDiscoveryEnabled;
            }

            set
            {
                this.engineDiscoveryEnabled = value;
            }
        }

        /**
         * Removes the specified engine ID from the internal time cache and thus
         * forces an engine time rediscovery the next time the SNMP engine with
         * the supplied ID is contacted.
         *
         * @param engineID
         *    the SNMP engine ID whose engine time to remove.
         * @since 1.6
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveEngineTime(OctetString engineID)
        {
            timeTable.RemoveEntry(engineID);
        }

        /**
         * Gets the counter support instance that can be used to register for
         * counter incrementation events.
         * @return
         *    a <code>CounterSupport</code> instance that is used to fire
         *    {@link CounterEvent}.
         */
        public CounterSupport CounterSupport
        {
            get
            {
                return this.counterSupport;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.counterSupport = value;
            }
        }

        /**
         * Returns the security protocol collection used by this USM.
         * @return
         *    a <code>SecurityProtocols</code> instance which is by default the
         *    same instance as returned by {@link SecurityProtocols#getInstance()}.
         * @since 1.2
         */
        public SecurityProtocols SecurityProtocols
        {
            get
            {
                return this.securityProtocols;
            }
        }


        /**
         * Sets the counter support instance. By default, the singleton instance
         * provided by the {@link CounterSupport} instance is used.
         * @param counterSupport
         *    a <code>CounterSupport</code> subclass instance.
         */
    }
}
