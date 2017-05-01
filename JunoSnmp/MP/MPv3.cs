// <copyright file="MPv3.cs" company="None">
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

namespace JunoSnmp.MP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using JunoSnmp;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Util;

    /**
 * The <code>MPv3</code> is the message processing model for SNMPv3.
 *
 * @author Frank Fock
 * @version 1.9.2
 */
    public class MPv3 : MessageProcessingModel, IEngineIdCacheSize
    {

        public static readonly MessageProcessingModels MPId = MessageProcessingModel.MessageProcessingModels.MPv3;
        public static readonly int MPv3_REPORTABLE_FLAG = 4;
        public static readonly int MAX_MESSAGE_ID = 2147483647;

        private static readonly int INT_LOW_16BIT_MASK = 0x0000FFFF;

        /// <summary>
        /// Local engine ID constant for context engineID discovery as defined by RFC 5343.
        /// </summary>
        public static readonly OctetString LOCAL_ENGINE_ID = OctetString.FromHexString("80:00:00:00:06");

        public static readonly int MAXLEN_ENGINE_ID = 32;
        public static readonly int MINLEN_ENGINE_ID = 5;

        private static readonly int MAX_HEADER_PAYLOAD_LENGTH =
            // length of msgFlags
            new OctetString("\0").BERLength +
            // length of msgID, msgMaxSize, securityModel
            3 * new Integer32(int.MaxValue).BERLength;

        private static readonly int MAX_HEADER_LENGTH =
            MAX_HEADER_PAYLOAD_LENGTH +
            BER.GetBERLengthOfLength(MAX_HEADER_PAYLOAD_LENGTH) + 1;

        private SecurityProtocols securityProtocols;

        private static readonly log4net.ILog log = log4net.LogManager
          .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private JunoSnmp.Security.SecurityModels securityModels;

        private Cache cache;
        private IDictionary<IAddress, OctetString> engineIDs;
        private int maxEngineIdCacheSize = JunoSnmpSettings.MaxEngineIdCacheSize;
        private byte[] localEngineID;

        private int currentMsgID = new Random().Next(MAX_MESSAGE_ID);

        private CounterSupport counterSupport;

        protected IEngineIdCacheFactory engineIdCacheFactory = new LimitedCapacityEngineIdCacheFactory();

        public delegate void EngineChangeHandler(object o, EngineChangeArgs args);

        public event EngineChangeHandler OnAddEngine;
        public event EngineChangeHandler OnRemoveEngine;
        public event EngineChangeHandler OnIgnoreEngine;

        protected IPDUFactory incomingPDUFactory = new PDUv3Factory();

        /**
         * Creates a MPv3 with a default local engine ID.
         */
        public MPv3() : this(CreateLocalEngineID(), null)
        {

        }

        /**
         * Creates a MPv3 with a supplied local engine ID.
         * @param localEngineID
         *    the local engine ID. Its length must be >= 5 and <= 32.
         */
        public MPv3(byte[] localEngineID) : this(localEngineID, null)
        {
            this.LocalEngineID = localEngineID;
        }

        /**
         * Creates a MPv3 with a supplied local engine ID and {@link PDUFactory}
         * for incoming messages.
         * @param localEngineID
         *    the local engine ID. Its length must be >= 5 and <= 32.
         * @param incomingPDUFactory
         *    a {@link PDUFactory}. If <code>null</code> the default factory will be
         *    used which creates {@link ScopedPDU} instances.
         * @since 1.9.1
         */
        public MPv3(byte[] localEngineID, IPDUFactory incomingPDUFactory) :
        this(localEngineID, incomingPDUFactory, SecurityProtocols.GetInstance(),
                JunoSnmp.Security.SecurityModels.Instance, CounterSupport.Instance)
        {
        }

        /**
         * This is a convenience constructor which can be used to create a MPv3 which
         * is bound to a specific USM instance. A dedicated USM instance per
         * MPv3 is necessary if multiple {@link Snmp} instances are used within a VM.
         * @param usm
         *    an USM instance.
         * @since 1.10
         */
        public MPv3(USM usm) : this(usm.LocalEngineID.GetValue(), null,
                SecurityProtocols.GetInstance(),
                JunoSnmp.Security.SecurityModels.GetCollection(new SecurityModel[] { usm }),
                CounterSupport.Instance)
        {
        }

        /**
         * Creates a fully qualified MPv3 instance with custom security protocols
         * and models as well as a custom counter support.
         * The current message ID is set using the USM engine boots counter (if available)
         * according to the RFC3412 §6.2.
         * @param localEngineID
         *    the local engine ID. Its length must be >= 5 and <= 32.
         * @param incomingPDUFactory
         *    a {@link PDUFactory}. If <code>null</code> the default factory will be
         *    used which creates {@link ScopedPDU} instances.
         * @param secProtocols
         *    the SecurityProtocols instance to use when looking up a security
         *    protocol. To get a default instance, use
         *    {@link SecurityProtocols#getInstance()}.
         * @param secModels
         *    the SecurityModels instance to use when looking up a security model.
         *    If you use more than one USM instance, you need to create a
         *    SecurityProtocols instance (container) for each such USM instance (and
         *    MPv3 combination). To get a default instance, use
         *    {@link SecurityProtocols#getInstance()}.
         * @param counterSupport
         *    The CounterSupport instance to be used to count events created by this
         *    MPv3 instance. To get a default instance, use
         *    {@link CounterSupport#getInstance()}.
         * @since 1.10
         */
        public MPv3(byte[] localEngineID, IPDUFactory incomingPDUFactory,
            SecurityProtocols secProtocols,
            JunoSnmp.Security.SecurityModels secModels,
            CounterSupport counterSupport)
        {
            if (incomingPDUFactory != null)
            {
                this.incomingPDUFactory = incomingPDUFactory;
            }

            engineIDs = engineIdCacheFactory.CreateEngineIdMap(this);
            cache = new Cache();

            if (secProtocols == null)
            {
                throw new ArgumentNullException();
            }

            securityProtocols = secProtocols;

            securityModels = secModels;

            if (counterSupport == null)
            {
                throw new ArgumentNullException();
            }

            this.counterSupport = counterSupport;
            LocalEngineID = localEngineID;
            SecurityModel usm = secModels[SecurityModel.SecurityModelID.SECURITY_MODEL_USM];

            if (usm is USM)
            {
                setCurrentMsgID(MPv3.RandomMsgID(((USM)usm).EngineBoots));
            }
        }

        /**
         * Returns the enging ID factory that was used to create the current engine ID cache.
         * @return
         *    a {@link org.snmp4j.mp.MPv3.EngineIdCacheFactory} implementation.
         * @since 2.3.4
         */

        /**
         * Sets the engine ID cache factory and resets (clears) the current cache. The maximum size of the
         * cache is determined using {@link #getMaxEngineIdCacheSize()} as this implements the {@link EngineIdCacheSize}
         * interface. By default the maximum cache size {@link SNMP4JSettings#getMaxEngineIdCacheSize()} is used.
         * @param engineIdCacheFactory
         *    a {@link org.snmp4j.mp.MPv3.EngineIdCacheFactory} implementation that is used to create a new cache.
         * @since 2.3.4
         */
        public IEngineIdCacheFactory EngineIdCacheFactory
        {
            get
            {
                return engineIdCacheFactory;
            }

            set
            {
                engineIDs = value.CreateEngineIdMap(this);
                this.engineIdCacheFactory = value;
            }
        }


        /**
         * Sets the upper limit for the engine ID cache. Modifying this value will not immediately
         * take effect on the cache size.
         * @param maxEngineIdCacheSize
         *    the maximum number of engine IDs hold in the internal cache. If more than
         *    those engine IDs are used by the MPv3, the eldest engine ID is removed
         *    from the cache. Eldest means the eldest initial use.
         *    A different cache can be implemented by using a custom
         *    {@link EngineIdCacheFactory} and setting it after calling
         *    this constructor.
         */
        public int MaxEngineIdCacheSize
        {
            get
            {
                return maxEngineIdCacheSize;
            }

            set
            {
                this.maxEngineIdCacheSize = value;
            }
        }

        /**
         * Creates a local engine ID based on the local IP address and additional four random bytes.
         * WARNING: Do not use this engine ID generator for a command responder (agent) if you DO NOT
         * persistently save the one time generated engine ID for subsequent use when the agent is
         * restarted.
         *
         * @return
         *    a new local engine ID with a random part to avoid engine ID clashes for multiple command
         *    generators on the same system.
         */
        public static byte[] CreateLocalEngineID()
        {
            int enterpriseID = JunoSnmpSettings.EnterpriseID;
            byte[] engineID = new byte[5];
            engineID[0] = (byte)(0x80 | ((enterpriseID >> 24) & 0xFF));
            engineID[1] = (byte)((enterpriseID >> 16) & 0xFF);
            engineID[2] = (byte)((enterpriseID >> 8) & 0xFF);
            engineID[3] = (byte)(enterpriseID & 0xFF);
            engineID[4] = 2;
            OctetString os = new OctetString();

            try
            {
                IPAddress ip = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(a => a.AddressFamily == AddressFamily.InterNetwork || a.AddressFamily == AddressFamily.InterNetworkV6);

                /*
                 * Note: Snmp4J uses the algorithm specified in RFC 2571 SNMP-FRAMEWORK-MIB, but adds
                 * random padding to prevent collisions if there are several engines on a single host.
                 * Accordingly, the 5th byte had been changed here to an enterprise-specific value
                 * rather than one of the original values which did not specify padding.
                 */
                engineID[4] = 128;
                os.SetValue(ip.GetAddressBytes());
            }
            catch (Exception)
            {
                log.Debug("Local host cannot be determined for creation of local engine ID");
                engineID[4] = 4;
                os.SetValue(Encoding.UTF8.GetBytes("JunoSnmp"));
            }
            
            OctetString ownEngineID = new OctetString(engineID);
            Random random = new Random((int)DateTime.Now.Ticks);
            byte[] fourBytes = new byte[4];
            random.NextBytes(fourBytes);
            ownEngineID.Append(os);
            ownEngineID.Append(fourBytes);
            return ownEngineID.GetValue();
        }

        /**
         * Creates a local engine ID based on the ID string supplied
         * @param id
         *    an ID string.
         * @return
         *    a new local engine ID.
         */
        public static byte[] CreateLocalEngineID(OctetString id)
        {
            int enterpriseID = JunoSnmpSettings.EnterpriseID;
            byte[] engineID = new byte[5];
            engineID[0] = (byte)(0x80 | ((enterpriseID >> 24) & 0xFF));
            engineID[1] = (byte)((enterpriseID >> 16) & 0xFF);
            engineID[2] = (byte)((enterpriseID >> 8) & 0xFF);
            engineID[3] = (byte)(enterpriseID & 0xFF);
            engineID[4] = 4;
            OctetString ownEngineID = new OctetString(engineID);

            // The maximum engineID length is 32 bytes, including the 5 defined above
            if(id.Length > 27)
            {
                ownEngineID.Append(id.Substring(0, 26));
            }
            else
            {
                ownEngineID.Append(id);
            }
            
            return ownEngineID.GetValue();
        }

        /**
         * Creates a random message ID according to the method proposed by RFC3412:
         * "Values for msgID SHOULD be generated in a manner that avoids re-use
         * of any outstanding values.  Doing so provides protection against some
         * replay attacks.  One possible implementation strategy would be to use
         * the low-order bits of snmpEngineBoots [RFC3411] as the high-order
         * portion of the msgID value and a monotonically increasing integer for
         * the low-order portion of msgID."
         *
         * @param engineBoots
         */
        public static int RandomMsgID(int engineBoots)
        {
            return (new Random().Next(MAX_MESSAGE_ID) & INT_LOW_16BIT_MASK) | ((engineBoots & INT_LOW_16BIT_MASK) << 16);
        }

        /**
         * Gets a copy of the local engine ID.
         * @return
         *    a byte array containing the local engine ID.
         */

        /**
         * Sets the local engine ID. This value must not be changed after message
         * processing has been started.
         * Note: When setting the local engine ID, the engine boots counter should
         * be known at the same time. Thus, please also call
         * <pre>
         *   setCurrentMsgID(randomMsgID(engineBoots));
         * </pre>
         * before starting the message processing.
         * @param engineID
         *    the local engine ID. Its length must be >= 5 and <= 32.
         */
        public byte[] LocalEngineID
        {
            get
            {
                byte[] retval = new byte[localEngineID.Length];
                System.Array.Copy(localEngineID, 0, retval, 0, localEngineID.Length);
                return retval;
            }

            set
            {
                if ((value == null) ||
                    (value.Length < MINLEN_ENGINE_ID) ||
                    (value.Length > MAXLEN_ENGINE_ID))
                {
                    throw new ArgumentException("Illegal (local) engine ID");
                }

                this.localEngineID = value;
            }
        }

        /**
         * Creates and initializes the default security protocols.
         * @see SecurityProtocols#addDefaultProtocols()
         */
        public void InitDefaults()
        {
            securityProtocols.AddDefaultProtocols();
        }

        /**
         * Gets an authentication protocol for the supplied ID.
         * @param id
         *    an authentication protocol OID.
         * @return
         *    an <code>AuthenticationProtocol</code> instance if the supplied ID
         *    is supported, otherwise <code>null</code> is returned.
         */
        public IAuthenticationProtocol GetAuthProtocol(OID id)
        {
            return securityProtocols.GetAuthenticationProtocol(id);
        }

        /**
         * Gets an privacy protocol for the supplied ID.
         * @param id
         *    an privacy protocol OID.
         * @return
         *    an <code>PrivacyProtocol</code> instance if the supplied ID
         *    is supported, otherwise <code>null</code> is returned.
         */
        public IPrivacyProtocol GetPrivProtocol(OID id)
        {
            return securityProtocols.GetPrivacyProtocol(id);
        }

        /**
         * Gets the security model for the supplied ID.
         * @param id
         *    a security model ID.
         * @return
         *    a <code>SecurityModel</code> instance if the supplied ID
         *    is supported, otherwise <code>null</code> is returned.
         */
        public SecurityModel GetSecurityModel(int id)
        {
            if(Enum.IsDefined(typeof(JunoSnmp.Security.SecurityModel.SecurityModelID), id))
            {
                return securityModels[(JunoSnmp.Security.SecurityModel.SecurityModelID)id];
            }

            return null;
        }

        public MessageProcessingModels ID
        {
            get
            {
                return MPv3.MPId;
            }
            
        }

        public override bool IsProtocolVersionSupported(int version)
        {
            return (version == SnmpConstants.version3);
        }

        /**
         * Adds an engine ID (other than the local engine ID) to the internal storage.
         * @param address
         *    the <code>Address</code> of the remote SNMP engine.
         * @param engineID
         *    the engine ID of the remote SNMP engine.
         * @return
         *    <code>true</code> if the engine ID has been added, <code>false</code>
         *    otherwise (if the supplied <code>engineID</code> equals the local one).
         */
        public bool AddEngineID(IAddress address, OctetString engineID)
        {
            if (!Array.Equals(this.localEngineID, engineID.GetValue()))
            {
                try
                {
                    OctetString previousEngineID = AddEngineIdToCache(address, engineID);
                    if (previousEngineID == null || !previousEngineID.Equals(engineID))
                    {
                        OnAddEngine(this, new EngineChangeArgs(engineID, address));
                    }
                }
                catch (ArgumentException)
                {
                    OnIgnoreEngine(this, new EngineChangeArgs(engineID, address));
                    return false;
                }
                return true;
            }
            return false;
        }

        /**
         * Put the engine ID for the given address into the internal cache. If the cache
         * reached its limit,
         * @param address
         *    the address of the engine ID
         * @param engineID
         *    the engine ID to cache.
         * @return
         *    the previous engine ID or <code>null</code> if there was no engine ID
         *    cached for the given address.
         * @throws IllegalArgumentException when the local maximum cache size is exceeded.
         * @since 2.3.4
         */
        protected OctetString AddEngineIdToCache(IAddress address, OctetString engineID)
        {
            // Save previous value, if any.
            OctetString prevID = null;
            engineIDs.TryGetValue(address, out prevID);

            if ((maxEngineIdCacheSize > 0) && (engineIDs.Count >= maxEngineIdCacheSize))
            {
                string msg = "MPv3: Failed to add engineID '" + engineID.ToHexString() + "' for address '" + address +
                    "' to local cache because its size limit of " + maxEngineIdCacheSize + "has been reached";
                log.Warn(msg);
                throw new ArgumentException(msg);
            }
            else
            {
                engineIDs.Add(address, engineID);
            }

            return prevID;
        }

        /**
         * Gets the engine ID associated with the supplied address from the local
         * storage and fires the corresponding {@link SnmpEngineEvent}.
         * @param address
         *    the <code>Address</code> of the remote SNMP engine.
         * @return
         *    the engine ID of the remote SNMP engine or <code>null</code> if there
         *    is no entry for <code>address</code> in the local storage.
         */
        public OctetString GetEngineID(IAddress address)
        {
            return engineIDs[address];
        }

        /**
         * Removes an engine ID association from the local storage and fires the
         * corresponding {@link SnmpEngineEvent}.
         * @param address
         *    the <code>Address</code> of the remote SNMP engine for whose engine ID
         *    is to be removed.
         * @return
         *    the removed engine ID of the remote SNMP engine or <code>null</code> if
         *    there is no entry for <code>address</code> in the local storage.
         */
        public OctetString RemoveEngineID(IAddress address)
        {
            OctetString engineID = null;
            engineIDs.TryGetValue(address, out engineID);
            
            if (engineID != null)
            {
                engineIDs.Remove(address);

                OnRemoveEngine(this, new EngineChangeArgs(engineID, address));
            }

            return engineID;
        }


        /**
         * The <code>CacheEntry</code> class holds state reference information
         * for the MPv3 message processing model for a single message.
         * @author Frank Fock
         * @version 1.0
         */
        protected class CacheEntry : StateReference
        {
            int msgID;
            long transactionID;
            byte[] secEngineID;
            SecurityModel secModel;
            byte[] secName;
            SecurityLevel secLevel;
            byte[] contextEngineID;
            byte[] contextName;
            ISecurityStateReference secStateReference;
            int errorCode;

            public CacheEntry(int msgID,
                              long reqID,
                              byte[] secEngineID,
                              SecurityModel secModel,
                              byte[] secName,
                              SecurityLevel secLevel,
                              byte[] contextEngineID,
                              byte[] contextName,
                              ISecurityStateReference secStateReference,
                              int errorCode)
            {
                this.msgID = msgID;
                this.transactionID = reqID;
                this.secEngineID = secEngineID;
                this.secModel = secModel;
                this.secName = secName;
                this.secLevel = secLevel;
                this.contextEngineID = contextEngineID;
                this.contextName = contextName;
                this.secStateReference = secStateReference;
                this.errorCode = errorCode;
            }
        }

        /**
         * The <code>Cache</code> stores state reference information for the MPv3.
         * @author Frank Fock
         * @version 1.0
         */
        protected class Cache
        {
            //TODO: this should be a weakreference-based collection
            ////private ConditionalWeakTable<PduHandle, StateReference> entries = new ConditionalWeakTable<PduHandle, StateReference>();
            private Dictionary<PduHandle, StateReference> entries = new Dictionary<PduHandle, StateReference>();
            /**
             * Adds a <code>StateReference</code> to the cache.
             * The <code>PduHandle</code> of the supplied entry will be set to
             * <code>null</code> when the new entry is already part of the cache, because the
             * cache uses a <code>WeakHashMap</code> internally which uses the
             * <code>PduHandle</code> as key. If the new entry equals an existing entry
             * except of the message ID then the new message ID will be added to the
             * existing entry.
             * @param entry
             *    the state reference to add.
             * @return
             *    {@link SnmpConstants#SNMP_MP_DOUBLED_MESSAGE} if the entry already
             *    exists and {@link SnmpConstants#SNMP_MP_OK} on success.
             */
            [MethodImpl(MethodImplOptions.Synchronized)]
            public int AddEntry(StateReference entry)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Adding cache entry: " + entry);
                }

                StateReference existing = null;
                bool result = entries.TryGetValue(entry.PduHandle, out existing);
                if (result)
                {
                    // reassign handle for comparison:
                    existing.PduHandle = entry.PduHandle;
                    if (existing.Equals(entry))
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Doubled message: " + entry);
                        }
                        // clear it again to remove strong self-reference
                        existing.PduHandle = null;
                        return SnmpConstants.SNMP_MP_DOUBLED_MESSAGE;
                    }
                    else if (existing.EqualsExceptMsgID(entry))
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Adding previous message IDs " + existing.getMessageIDs() + " to new entry " + entry);
                        }

                        entry.addMessageIDs(existing.getMessageIDs());
                    }
                    else if (log.IsDebugEnabled)
                    {
                        log.Debug("New entry does not match existing, although request ID is the same " + entry + " != " + existing);
                    }
                    // clear it again to remove strong self-reference
                    existing.PduHandle = null;
                }
                // add it
                PduHandle key = entry.PduHandle;
                // because we are using a weak hash map for the cache, we need to null out
                // our key from the entry.
                entry.PduHandle = null;
                entries.Add(key, entry);
                return SnmpConstants.SNMP_MP_OK;
            }

            /**
             * Delete the cache entry with the supplied <code>PduHandle</code>.
             * @param pduHandle
             *    a pduHandle.
             * @return
             *    <code>true</code> if an entry has been deleted, <code>false</code>
             *    otherwise.
             */
             [MethodImpl(MethodImplOptions.Synchronized)]
            public bool DeleteEntry(PduHandle pduHandle)
            {
                return entries.Remove(pduHandle);
            }

            /**
             * Pop the cache entry with the supplied ID from the cache.
             * @param msgID
             *    a message ID.
             * @return
             *    a <code>CacheEntry</code> instance with the given message ID or
             *    <code>null</code> if such an entry cannot be found. If a cache entry
             *   is returned, the same is removed from the cache.
             */
            [MethodImpl(MethodImplOptions.Synchronized)]
            public StateReference PopEntry(int msgID)
            {
                StateReference e = null;
                PduHandle p = null;
                foreach (PduHandle ph in entries.Keys)
                {
                    bool result = entries.TryGetValue(ph, out e);
                    if ((e != null) && (e.IsMatchingMessageID(msgID)))
                    {
                        p = ph;
                        break;
                    }
                }

                if(p != null)
                {
                    entries.Remove(p);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Removed cache entry: " + e);
                    }
                    e.PduHandle = p;
                    return e;
                }

                return null;
            }
        }

        /**
         * The <code>HeaderData</code> represents the message header information
         * of SNMPv3 message.
         * @author Frank Fock
         * @version 1.0
         */
        public class HeaderData : IBERSerializable
        {
            public static readonly byte FLAG_AUTH = 0x01;
            public static readonly byte FLAG_PRIV = 0x02;

            Integer32 msgID = new Integer32(0);
            Integer32 msgMaxSize = new Integer32(int.MaxValue);
            OctetString msgFlags = new OctetString(new byte[1]);
            SecurityModel.SecurityModelID securityModel = JunoSnmp.Security.SecurityModel.SecurityModelID.SECURITY_MODEL_ANY;

            public int MsgID
            {
                get
                {
                    return msgID.GetValue();
                }

                set
                {
                    this.msgID.SetValue(value);
                }
            }

            public int MsgMaxSize
            {
                get
                {
                    return msgMaxSize.GetValue();
                }

                set
                {
                    this.msgMaxSize.SetValue(value);
                }
            }

            public int MsgFlags
            {
                get
                {
                    return msgFlags.GetValue()[0] & 0xFF;
                }

                set
                {
                    this.msgFlags.GetValue()[0] = (byte)value;
                }
            }

            public SecurityModel.SecurityModelID SecurityModel
            {
                get
                {
                    return this.securityModel;
                }

                set
                {
                    this.securityModel = value;
                }
            }

            public int BERPayloadLength
            {
                get
                {
                    int length = msgID.BERLength;
                    length += msgMaxSize.BERLength;
                    length += msgFlags.BERLength;
                    Integer32 secMod = new Integer32((int)securityModel);
                    length += secMod.BERLength;
                    return length;
                }
            }

            public int BERLength
            {
                get
                {
                    int length = this.BERPayloadLength;
                    length += BER.GetBERLengthOfLength(length) + 1;
                    return length;
                }
            }

            public void DecodeBER(BERInputStream message)
            {
                byte type;
                int length = BER.DecodeHeader(message, out type);
                if (type != BER.SEQUENCE)
                {
                    throw new ArgumentException("Unexpected sequence header type: " +
                                          type);
                }

                msgID.DecodeBER(message);
                msgMaxSize.DecodeBER(message);
                if (msgMaxSize.GetValue() < 484)
                {
                    throw new ArgumentException("Invalid msgMaxSize: " + msgMaxSize);
                }

                msgFlags.DecodeBER(message);
                if (msgFlags.Length != 1)
                {
                    throw new ArgumentException("Message flags length != 1: " + msgFlags.Length);
                }

                Integer32 secMod = new Integer32();
                secMod.DecodeBER(message);
                securityModel = (JunoSnmp.Security.SecurityModel.SecurityModelID)secMod.IntValue;

                if (log.IsDebugEnabled)
                {
                    log.Debug("SNMPv3 header decoded: msgId=" + msgID +
                                 ", msgMaxSize=" + msgMaxSize +
                                 ", msgFlags=" + msgFlags.ToHexString() +
                                 ", secModel=" + securityModel);
                }

                BER.CheckSequenceLength(length, this);
            }

            public void EncodeBER(Stream outputStream)
            {
                BER.EncodeHeader(outputStream, BER.SEQUENCE, this.BERPayloadLength);
                msgID.EncodeBER(outputStream);
                msgMaxSize.EncodeBER(outputStream);
                msgFlags.EncodeBER(outputStream);
                Integer32 secMod = new Integer32((int)securityModel);
                secMod.EncodeBER(outputStream);
            }
        }
        
        /// <summary>
        /// Gets a unique message ID between 1 and <see cref="MAX_MESSAGE_ID"/>
        /// </summary>
        public int NextMessageID
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (this.currentMsgID >= MAX_MESSAGE_ID)
                {
                    this.currentMsgID = 1;
                }

                return this.currentMsgID++;
            }
        }

        /**
         * Gets the security protocols supported by this <code>MPv3</code>.
         * @return
         *    return a <code>SecurityProtocols</code>.
         */

        /**
         * Sets the security protocols for this <code>MPv3</code>.
         * @param securityProtocols SecurityProtocols
         */
        public SecurityProtocols SecurityProtocols
        {
            get
            {
                return this.securityProtocols;
            }

            set
            {
                this.securityProtocols = value;
            }
        }

        public override void ReleaseStateReference(PduHandle pduHandle)
        {
            cache.DeleteEntry(pduHandle);
        }

        public override int PrepareOutgoingMessage(IAddress transportAddress,
                                          int maxMessageSize,
                                          MessageProcessingModels messageProcessingModel,
                                          SecurityModel.SecurityModelID securityModel,
                                          byte[] securityName,
                                          SecurityLevel securityLevel,
                                          PDU pdu,
                                          bool expectResponse,
                                          PduHandle sendPduHandle,
                                          IAddress destTransportAddress,
                                          BEROutputStream outgoingMessage,
                                          TransportStateReference tmStateReference)
        {
            if (!(pdu is ScopedPDU))
            {
                throw new ArgumentException(
                    "MPv3 only accepts ScopedPDU instances as pdu parameter");
            }

            ScopedPDU scopedPDU = (ScopedPDU)pdu;
            SecurityModel secModel = securityModels[securityModel];

            if (secModel == null)
            {
                return SnmpConstants.SNMP_MP_UNSUPPORTED_SECURITY_MODEL;
            }
            
            // lookup engine ID
            byte[] secEngineID;
            if (secModel.HasAuthoritativeEngineID)
            {
                OctetString securityEngineID = engineIDs[transportAddress];
                if (securityEngineID != null)
                {
                    secEngineID = securityEngineID.GetValue();
                    if (scopedPDU.ContextEngineID.Length == 0)
                    {
                        switch (pdu.Type)
                        {
                            case PDU.NOTIFICATION:
                            case PDU.INFORM:
                                {
                                    OctetString localEngineID = new OctetString(LocalEngineID);
                                    if (log.IsDebugEnabled)
                                    {
                                        log.Debug("Context engine ID of scoped PDU is empty! Setting it to local engine ID: " +
                                            localEngineID.ToHexString());
                                    }

                                    scopedPDU.ContextEngineID = localEngineID;
                                }
                                break;
                            default:
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug("Context engine ID of scoped PDU is empty! Setting it to authoritative engine ID: " +
                                        securityEngineID.ToHexString());
                                }

                                scopedPDU.ContextEngineID = new OctetString(secEngineID);
                                break;
                        }
                    }
                }
                else
                {
                    secEngineID = new byte[0];
                }
            }
            else
            {
                secEngineID = new byte[0];
            }
            // determine request type
            if (pdu.IsConfirmedPdu)
            {
                if (secEngineID.Length == 0)
                {
                    if (secModel.SupportsEngineIdDiscovery)
                    {
                        securityLevel = SecurityLevel.NoAuthNoPriv;
                        // do not send any management information
                        scopedPDU = (ScopedPDU)scopedPDU.Clone();
                        scopedPDU.Clear();
                    }
                    else if ((scopedPDU.ContextEngineID == null) ||
                             (scopedPDU.ContextEngineID.Length == 0))
                    {
                        log.Warn("ScopedPDU with empty context engine ID");
                    }
                    else if (!LOCAL_ENGINE_ID.Equals(scopedPDU.ContextEngineID) &&
                             GetEngineID(transportAddress) == null)
                    {
                        // Learn context engine ID
                        AddEngineID(transportAddress, scopedPDU.ContextEngineID);
                    }
                }
            }
            else
            {
                if (scopedPDU.ContextEngineID.Length == 0)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Context engine ID of unconfirmed scoped PDU is empty! " +
                                     "Setting it to local engine ID");
                    }
                    scopedPDU.ContextEngineID = new OctetString(localEngineID);
                }
            }

            // get length of scoped PDU
            int scopedPDULength = scopedPDU.BERLength;
            BEROutputStream scopedPdu =
                //new BEROutputStream(ByteBuffer.allocate(scopedPDULength));
                new BEROutputStream();
            scopedPDU.EncodeBER(scopedPdu);

            HeaderData headerData = new HeaderData();
            int flags = 0;

            switch (securityLevel)
            {
                case SecurityLevel.NoAuthNoPriv:
                    flags = 0;
                    break;
                case SecurityLevel.AuthNoPriv:
                    flags = 1;
                    break;
                case SecurityLevel.AuthPriv:
                    flags = 3;
                    break;
            }

            if (scopedPDU.IsConfirmedPdu)
            {
                flags |= MPv3_REPORTABLE_FLAG;
            }
            else
            {
                secEngineID = localEngineID;
            }

            int msgID = this.NextMessageID;
            headerData.MsgFlags = flags;
            headerData.MsgID = msgID;
            headerData.MsgMaxSize = maxMessageSize;
            headerData.SecurityModel = securityModel;

            ////ByteBuffer globalDataBuffer =
            ////    ByteBuffer.allocate(headerData.BERLength);
            BEROutputStream globalDataOutputStream =
                new BEROutputStream();
            headerData.EncodeBER(globalDataOutputStream);

            BERInputStream scopedPDUInput = new BERInputStream(scopedPdu.ToArray());

            // output data
            ISecurityParameters securityParameters =
                secModel.NewSecurityParametersInstance();

            int status =
                secModel.GenerateRequestMessage(messageProcessingModel,
                                                globalDataOutputStream.ToArray(),
                                                maxMessageSize,
                                                securityModel,
                                                secEngineID,
                                                securityName,
                                                securityLevel,
                                                scopedPDUInput,
                                                securityParameters,
                                                outgoingMessage,
                                                tmStateReference);
            if (status == SnmpConstants.SNMPv3_USM_OK)
            {
                if (expectResponse)
                {
                    cache.AddEntry(new StateReference(msgID,
                                                      flags,
                                                      maxMessageSize,
                                                      sendPduHandle,
                                                      transportAddress,
                                                      null,
                                                      secEngineID, securityModel,
                                                      securityName, securityLevel,
                                                      scopedPDU.ContextEngineID.
                                                      GetValue(),
                                                      scopedPDU.ContextName.GetValue(),
                                                      null,
                                                      status));
                }
            }
            return status;
        }

        public override int PrepareResponseMessage(MessageProcessingModels messageProcessingModel,
                                          int maxMessageSize,
                                          SecurityModel.SecurityModelID securityModel,
                                          byte[] securityName,
                                          SecurityLevel securityLevel,
                                          PDU pdu,
                                          int maxSizeResponseScopedPDU,
                                          StateReference stateReference,
                                          StatusInformation statusInformation,
                                          BEROutputStream outgoingMessage)
        {
            /** Leave entry in cache or remove it? RFC3414 §3.1.a.1 says discard it*/
            StateReference cacheEntry = cache.PopEntry(stateReference.MsgID.MessageID);
            if (cacheEntry == null)
            {
                return SnmpConstants.SNMP_MP_UNKNOWN_MSGID;
            }

            // get length of scoped PDU
            int scopedPDULength = pdu.BERLength;
            BEROutputStream scopedPDU;
            // check length
            if (scopedPDULength > maxSizeResponseScopedPDU)
            {
                PDU tooBigPDU = new ScopedPDU((ScopedPDU)pdu);
                tooBigPDU.Clear();
                tooBigPDU.RequestID = pdu.RequestID;
                tooBigPDU.ErrorStatus = SnmpConstants.SNMP_ERROR_TOO_BIG;
                tooBigPDU.ErrorIndex = 0;
                scopedPDULength = tooBigPDU.BERLength;
                scopedPDU = new BEROutputStream();
                tooBigPDU.EncodeBER(scopedPDU);
            }
            else
            {
                scopedPDU = new BEROutputStream();
                pdu.EncodeBER(scopedPDU);
            }

            HeaderData headerData = new HeaderData();
            int flags = 0;
            switch (securityLevel)
            {
                case SecurityLevel.NoAuthNoPriv:
                    flags = 0;
                    break;
                case SecurityLevel.AuthNoPriv:
                    flags = 1;
                    break;
                case SecurityLevel.AuthPriv:
                    flags = 3;
                    break;
            }
            // response message is not reportable
            headerData.MsgFlags = flags;
            headerData.MsgID = stateReference.MsgID.MessageID;
            headerData.MsgMaxSize = maxMessageSize;
            headerData.SecurityModel = securityModel;

            ////ByteBuffer globalDataBuffer =
            ////    ByteBuffer.allocate(headerData.BERLength);
            ////BEROutputStream globalDataOutputStream = new BEROutputStream(globalDataBuffer);
            BEROutputStream globalDataOutputStream = new BEROutputStream();
            headerData.EncodeBER(globalDataOutputStream);

            OctetString securityEngineID;
            switch (pdu.Type)
            {
                case PDU.RESPONSE:
                case PDU.TRAP:
                case PDU.REPORT:
                case PDU.V1TRAP:
                    securityEngineID = new OctetString(localEngineID);
                    break;
                default:
                    securityEngineID = new OctetString(cacheEntry.SecurityEngineID);
                    break;
            }

            BERInputStream scopedPDUInput = new BERInputStream(scopedPDU.ToArray());

            SecurityModel secModel = securityModels[securityModel];
            // output data
            ISecurityParameters securityParameters =
                secModel.NewSecurityParametersInstance();

            int status =
                secModel.GenerateResponseMessage(this.ID,
                    globalDataOutputStream.ToArray(),
                    maxMessageSize,
                    securityModel,
                    securityEngineID.GetValue(),
                    securityName,
                    securityLevel,
                    scopedPDUInput,
                    cacheEntry.SecurityStateReference,
                    securityParameters,
                    outgoingMessage);
            return status;
        }

        /**
         * Sends a report message.
         * @param messageDispatcher
         *    Send the message on behalf the supplied MessageDispatcher instance.
         * @param pdu ScopedPDU
         *    If <code>null</code>, then contextEngineID, contextName, and requestID
         *    of the report generated will be zero length and zero respective.
         *    Otherwise these values are extracted from the PDU.
         * @param securityLevel
         *    The security level to use when sending this report.
         * @param securityModel
         *    The security model to use when sending this report.
         * @param securityName
         *    The security name to use when sending this report.
         * @param maxSizeResponseScopedPDU
         *    the maximum size of of the report message (will be most likely ignored
         *    because a report should always fit in 484  bytes).
         * @param stateReference
         *    the state reference associated with the original message.
         * @param payload
         *    the variable binding to include in the report message.
         * @return
         *    an SNMP MPv3 error code or 0 if the report has been send successfully.
         */
        public int SendReport(IMessageDispatcher messageDispatcher,
                              ScopedPDU pdu,
                              SecurityLevel securityLevel,
                              SecurityModel.SecurityModelID securityModel,
                              OctetString securityName,
                              int maxSizeResponseScopedPDU,
                              StateReference stateReference,
                              VariableBinding payload)
        {
            ScopedPDU reportPDU = new ScopedPDU();
            reportPDU.Type = PDU.REPORT;
            if (pdu != null)
            {
                reportPDU.ContextEngineID = pdu.ContextEngineID;
                reportPDU.ContextName = pdu.ContextName;
                reportPDU.RequestID = pdu.RequestID;
            }
            else
            {
                // RFC 3412 §7.1.3d)
                reportPDU.ContextEngineID = new OctetString(LocalEngineID);
            }

            reportPDU.Add(payload);
            StatusInformation statusInformation = new StatusInformation();
            try
            {
                int status = messageDispatcher.ReturnResponsePdu(this.ID,
                    securityModel,
                    securityName.GetValue(),
                    securityLevel,
                    reportPDU,
                    maxSizeResponseScopedPDU,
                    stateReference,
                    statusInformation);
                if (status != SnmpConstants.SNMP_ERROR_SUCCESS)
                {
                    log.Warn("Error while sending report: " + status);
                    return SnmpConstants.SNMP_MP_ERROR;
                }
            }
            catch (MessageException mex)
            {
                log.Error("Error while sending report: " + mex.Message);
                return SnmpConstants.SNMP_MP_ERROR;
            }
            return SnmpConstants.SNMP_MP_OK;
        }

        public override int PrepareDataElements(IMessageDispatcher messageDispatcher,
                                       IAddress transportAddress,
                                       BERInputStream wholeMsg,
                                       TransportStateReference tmStateReference,
                                       MessageProcessingModels messageProcessingModel,
                                       SecurityModel.SecurityModelID securityModel,
                                       OctetString securityName,
                                       SecurityLevel securityLevel,
                                       MutablePDU pdu,
                                       PduHandle sendPduHandle,
                                       int maxSizeResponseScopedPDU,
                                       StatusInformation statusInformation,
                                       MutableStateReference mutableStateReference)
        {
            try
            {
                StateReference stateReference = new StateReference();
                // check if there is transport mapping information
                if (mutableStateReference.StateReference != null)
                {
                    stateReference.TransportMapping = 
                        mutableStateReference.StateReference.TransportMapping;
                }
                messageProcessingModel = MessageProcessingModels.MPv3;

                long pos = wholeMsg.Position;

                int length = BER.DecodeHeader(wholeMsg, out byte type);
                if (type != BER.SEQUENCE)
                {
                    return SnmpConstants.SNMP_MP_PARSE_ERROR;
                }

                long lengthOfLength = wholeMsg.Position;

                wholeMsg.Position = pos;

                if (wholeMsg.Skip(lengthOfLength) != lengthOfLength)
                {
                    return SnmpConstants.SNMP_MP_PARSE_ERROR;
                }

                Integer32 snmpVersion = new Integer32();
                snmpVersion.DecodeBER(wholeMsg);
                if (snmpVersion.GetValue() != SnmpConstants.version3)
                {
                    // internal error -> should not happen
                    throw new InvalidDataException(
                        "Internal error unexpected SNMP version read");
                }
                // decode SNMPv3 header
                HeaderData header = new HeaderData();
                header.DecodeBER(wholeMsg);
                securityModel = header.SecurityModel;

                stateReference.SetMsgID(header.MsgID);
                stateReference.MsgFlags = header.MsgFlags;
                stateReference.Address = transportAddress;

                mutableStateReference.StateReference = stateReference;

                // the usm has to recalculate this value
                maxSizeResponseScopedPDU = header.MsgMaxSize -
                                                  MAX_HEADER_LENGTH;

                ScopedPDU scopedPdu = (ScopedPDU)incomingPDUFactory.CreatePDU(this);
                pdu.Pdu = scopedPdu;

                SecurityModel secModel = securityModels[securityModel];
                if (secModel == null)
                {
                    log.Error("RFC3412 §7.2.4 - Unsupported security model: " +
                                 securityModel);

                    counterSupport.IncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmpUnknownSecurityModels));

                    return SnmpConstants.SNMP_MP_UNSUPPORTED_SECURITY_MODEL;
                }

                // determine security level
                switch (header.MsgFlags& 0x03)
                {
                    case 3:
                        {
                            securityLevel = SecurityLevel.AuthPriv;
                            break;
                        }
                    case 0:
                        {
                            securityLevel = SecurityLevel.NoAuthNoPriv;
                            break;
                        }
                    case 1:
                        {
                            securityLevel = SecurityLevel.AuthNoPriv;
                            break;
                        }
                    default:
                        {
                            securityLevel = SecurityLevel.NoAuthNoPriv;
                            log.Debug("RFC3412 §7.2.5 - Invalid message (illegal msgFlags)");

                            counterSupport.IncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmpInvalidMsgs));
                            // do not send back report
                            return SnmpConstants.SNMP_MP_INVALID_MESSAGE;
                        }
                }

                statusInformation.SecurityLevel = securityLevel;

                int secParametersPosition = (int)wholeMsg.Position;
                // get security parameters
                ISecurityParameters secParameters =
                    secModel.NewSecurityParametersInstance();
                secParameters.DecodeBER(wholeMsg);
                secParameters.SecurityParametersPosition = secParametersPosition;

                // reportable flag
                bool reportableFlag = ((header.MsgFlags& 0x04) > 0);

                OctetString securityEngineID = new OctetString();
                // create a new security state reference
                ISecurityStateReference secStateReference =
                    secModel.NewSecurityStateReference();
                // create output stream for scoped PDU
                // may be optimized by an output stream that maps directly into the
                // original input
                wholeMsg.Position = pos;

                BEROutputStream scopedPDU = new BEROutputStream();
                int status =
                    secModel.ProcessIncomingMsg((MessageProcessingModels)snmpVersion.GetValue(),
                                                header.MsgMaxSize-
                                                MAX_HEADER_LENGTH,
                                                secParameters,
                                                securityModel,
                                                securityLevel,
                                                wholeMsg,
                                                tmStateReference,
                                                // output parameters
                                                securityEngineID,
                                                securityName,
                                                scopedPDU,
                                                maxSizeResponseScopedPDU,
                                                secStateReference,
                                                statusInformation);
                wholeMsg.Close();

                if (status == SnmpConstants.SNMPv3_USM_OK)
                {
                    try
                    {
                        BERInputStream scopedPduStream =
                            new BERInputStream(scopedPDU.ToArray());
                        scopedPdu.DecodeBER(scopedPduStream);
                        sendPduHandle.TransactionID = scopedPdu.RequestID.GetValue();

                        // add the engine ID to the local cache if it is a report or response, do not add traps.
                        if ((securityEngineID.Length > 0) && scopedPdu.IsResponsePdu)
                        {
                            this.AddEngineID(transportAddress, securityEngineID);
                        }
                    }
                    catch (IOException iox)
                    {
                        log.Warn("ASN.1 parse error: " + iox.Message);
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(iox.StackTrace);
                        }
                        
                        counterSupport.IncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmpInASNParseErrs));
                        return SnmpConstants.SNMP_MP_PARSE_ERROR;
                    }
                    if (((scopedPdu.ContextEngineID == null) ||
                          (scopedPdu.ContextEngineID.Length == 0)) &&
                        ((scopedPdu.Type != PDU.RESPONSE) &&
                         (scopedPdu.Type != PDU.REPORT)))
                    {
                        CounterIncrArgs evt = new CounterIncrArgs(SnmpConstants.snmpUnknownPDUHandlers);
                        counterSupport.IncrementCounter(this, evt);
                        VariableBinding errorIndication =
                            new VariableBinding(evt.Oid, evt.CurrentValue);
                        statusInformation.ErrorIndication = errorIndication;
                        status = SnmpConstants.SNMP_MP_UNKNOWN_PDU_HANDLERS;
                    }
                }

                stateReference.SecurityName = securityName.GetValue();
                stateReference.SecurityEngineID = securityEngineID.GetValue();
                stateReference.SecurityLevel = securityLevel;
                stateReference.SecurityModel = securityModel;
                stateReference.SecurityStateReference = secStateReference;
                stateReference.PduHandle = sendPduHandle;

                if (status != SnmpConstants.SNMPv3_USM_OK)
                {
                    if ((reportableFlag) &&
                        (statusInformation.ErrorIndication != null))
                    {
                        // RFC3412 §7.2.6.a - generate a report
                        try
                        {
                            if (scopedPDU.Length != 0)
                            {
                                BERInputStream scopedPduStream =
                                    new BERInputStream(scopedPDU.ToArray());
                                scopedPdu.DecodeBER(scopedPduStream);
                            }
                            else
                            { // incoming message could not be decoded
                                scopedPdu = null;
                            }
                        }
                        catch (IOException iox)
                        {
                            log.Warn(iox);
                            scopedPdu = null;
                        }

                        StateReference cacheEntry =
                            new StateReference(header.MsgID,
                                               header.MsgFlags,
                                               maxSizeResponseScopedPDU,
                                               sendPduHandle,
                                               transportAddress,
                                               null,
                                               securityEngineID.GetValue(),
                                               securityModel, 
                                               securityName.GetValue(),
                                               securityLevel,
                                               (scopedPdu == null) ? new byte[0] :
                                               scopedPdu.ContextEngineID.GetValue(),
                                               (scopedPdu == null) ? new byte[0] :
                                               scopedPdu.ContextName.GetValue(),
                                               secStateReference, status);
                        cache.AddEntry(cacheEntry);

                        int reportStatus =
                            SendReport(messageDispatcher, scopedPdu,
                                       statusInformation.SecurityLevel,
                                       secModel.ID, securityName,
                                       maxSizeResponseScopedPDU,
                                       stateReference,
                                       statusInformation.ErrorIndication);
                        if (reportStatus != SnmpConstants.SNMP_MP_OK)
                        {
                            log.Warn("Sending report failed with error code: " +
                                        reportStatus);
                        }
                    }
                    return status;
                }

                stateReference.ContextEngineID = scopedPdu.ContextEngineID.GetValue();
                stateReference.ContextName = scopedPdu.ContextName.GetValue();
                stateReference.MaxSizeResponseScopedPDU = maxSizeResponseScopedPDU;

                if ((scopedPdu.Type == PDU.RESPONSE) ||
                    (scopedPdu.Type == PDU.REPORT))
                {
                    StateReference cacheEntry = cache.PopEntry(header.MsgID);
                    if (cacheEntry != null)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("RFC3412 §7.2.10 - Received PDU (msgID=" +
                                         header.MsgID+ ") is a response or " +
                                         "an internal class message. PduHandle.transactionID = " +
                                         cacheEntry.PduHandle.TransactionID);
                        }
                        sendPduHandle.CopyFrom(cacheEntry.PduHandle);

                        if (scopedPdu.Type == PDU.REPORT)
                        {

                            statusInformation.ContextEngineID = scopedPdu.ContextEngineID.GetValue();
                            statusInformation.ContextName = scopedPdu.ContextName.GetValue();
                            statusInformation.SecurityLevel = securityLevel;

                            if (((cacheEntry.SecurityEngineID.Length != 0) &&
                                  (!securityEngineID.EqualsValue(cacheEntry.SecurityEngineID))) ||
                                (secModel.ID != cacheEntry.SecurityModel) ||
                                ((!securityName.EqualsValue(cacheEntry.SecurityName) &&
                                  (securityName.Length != 0))))
                            {
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug(
                                        "RFC 3412 §7.2.11 - Received report message does not match sent message. Cache entry is: " +
                                        cacheEntry + ", received secName=" + securityName + ",secModel=" + secModel +
                                        ",secEngineID=" + securityEngineID);
                                }
                                //cache.deleteEntry(cacheEntry.getPduHandle());
                                mutableStateReference.StateReference = null;
                                return SnmpConstants.SNMP_MP_MATCH_ERROR;
                            }
                            if (!AddEngineID(cacheEntry.Address, securityEngineID))
                            {
                                if (log.IsWarnEnabled)
                                {
                                    log.Warn("Engine ID '" + securityEngineID +
                                                "' could not be added to engine ID cache for " +
                                                "target address '" + cacheEntry.Address +
                                                "' because engine ID matches local engine ID or cache size limit is reached");
                                }
                            }

                            //cache.deleteEntry(cacheEntry.getPduHandle());
                            mutableStateReference.StateReference = null;
                            log.Debug("MPv3 finished");
                            return SnmpConstants.SNMP_MP_OK;
                        }
                        if (scopedPdu.Type == PDU.RESPONSE)
                        {
                            if (((!securityEngineID.EqualsValue(cacheEntry.SecurityEngineID)) &&
                                 (cacheEntry.SecurityEngineID.Length != 0)) ||
                                (secModel.ID != cacheEntry.SecurityModel) ||
                                (!securityName.EqualsValue(cacheEntry.SecurityName)) ||
                                (securityLevel != cacheEntry.SecurityLevel) ||
                                ((!scopedPdu.ContextEngineID.EqualsValue(cacheEntry.ContextEngineID)) &&
                                 (cacheEntry.ContextEngineID.Length != 0)) ||
                                ((!scopedPdu.ContextName.EqualsValue(cacheEntry.ContextName) &&
                                  (cacheEntry.ContextName.Length != 0))))
                            {
                                log.Debug(
                                    "RFC 3412 §7.2.12.b - Received response message does not match sent message");
                                //cache.deleteEntry(cacheEntry.getPduHandle());
                                mutableStateReference.StateReference = null;
                                return SnmpConstants.SNMP_MP_MATCH_ERROR;
                            }
                            //cache.deleteEntry(cacheEntry.getPduHandle());
                            mutableStateReference.StateReference = null;
                            log.Debug("MPv3 finished");
                            return SnmpConstants.SNMP_MP_OK;
                        }
                    }
                    else
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("RFC3412 §7.2.10 - Received PDU (msgID=" +
                                         header.MsgID+ ") is a response or " +
                                         "internal class message, but cached " +
                                         "information for the msgID could not be found");
                        }
                        return SnmpConstants.SNMP_MP_UNKNOWN_MSGID;
                    }
                }
                else
                {
                    log.Debug("RFC3412 §7.2.10 - Received PDU is NOT a response or " +
                                 "internal class message -> unchanged PduHandle = " +
                                 sendPduHandle);
                }
                switch (scopedPdu.Type)
                {
                    case PDU.GET:
                    case PDU.GETBULK:
                    case PDU.GETNEXT:
                    case PDU.INFORM:
                    case PDU.SET:
                        {
                            if (securityEngineID.Length == 0)
                            {
                                log.Debug("Received confirmed message with 0 length security engine ID");
                            }
                            else if (!securityEngineID.EqualsValue(localEngineID))
                            {
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug("RFC 3412 §7.2.13.a - Security engine ID " +
                                                 securityEngineID.ToHexString() +
                                                 " does not match local engine ID " +
                                                 new OctetString(localEngineID).ToHexString());
                                }
                                mutableStateReference.StateReference = null;
                                return SnmpConstants.SNMP_MP_INVALID_ENGINEID;
                            }
                            int cacheStatus = cache.AddEntry(stateReference);
                            if (cacheStatus == SnmpConstants.SNMP_MP_DOUBLED_MESSAGE)
                            {
                                mutableStateReference.StateReference = null;
                            }
                            return SnmpConstants.SNMP_MP_OK;
                        }
                    case PDU.TRAP:
                    case PDU.V1TRAP:
                        {
                            mutableStateReference.StateReference = null;
                            return SnmpConstants.SNMP_MP_OK;
                        }
                }
                // this line should not be reached
                return SnmpConstants.SNMP_MP_ERROR;
            }
            catch (IOException iox)
            {
                log.Warn("MPv3 parse error: " + iox.Message);
                if (log.IsDebugEnabled)
                {
                    log.Debug(iox.StackTrace);
                }
                return SnmpConstants.SNMP_MP_PARSE_ERROR;
            }
        }

        /**
         * Gets the security models supported by this MPv3.
         * @return
         *   a <code>SecurityModels</code> instance.
         */

        /**
         * Sets the security models supported by this MPv3.
         * @param securityModels
         *    a <code>SecurityModels</code> instance.
         */
        public SecurityModels SupportedSecurityModels
        {
            get
            {
                return this.securityModels;
            }

            set
            {
                this.securityModels = value;
            }
        }

        /**
         * Gets the enterprise ID used for creating the local engine ID.
         * @return
         *    an enterprise ID as registered by the IANA (see http://www.iana.org).
         * @deprecated
         *    Use {@link SNMP4JSettings#getEnterpriseID()} instead.
         */

        /**
         * Sets the IANA enterprise ID to be used for creating local engine ID by
         * {@link #createLocalEngineID()}.
         * @param newEnterpriseID
         *    an enterprise ID as registered by the IANA (see http://www.iana.org).
         * @deprecated
         *    Use {@link SNMP4JSettings#setEnterpriseID(int)} instead.
         */
        public static int EnterpriseID
        {
            get
            {
                return JunoSnmpSettings.EnterpriseID;
            }

            set
            {
                JunoSnmpSettings.EnterpriseID = value;
            }
        }

        /**
         * Gets the counter support instance that can be used to register for
         * counter incrementation events.
         * @return
         *    a <code>CounterSupport</code> instance that is used to fire
         *    {@link CounterEvent}.
         */

        /**
         * Sets the counter support instance. By default, the singleton instance
         * provided by the {@link CounterSupport} instance is used.
         * @param counterSupport
         *    a <code>CounterSupport</code> subclass instance.
         */
        public CounterSupport CounterSupport
        {
            get
            {
                return counterSupport;
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
         * Get the number of cached engine IDs.
         * @return
         *    size of the internal engine ID cache.
         * @since 2.3.4
         */
        public int EngineIdCacheSize
        {
            get
            {
                return engineIDs.Count;
            }
        }

        /**
         * Creates a PDU class that is used to parse incoming SNMP messages.
         * @param target
         *    the <code>target</code> parameter must be ignored.
         * @return
         *    a {@link ScopedPDU} instance by default.
         * @since 1.9.1
         * @deprecated
         *    Use {@link org.snmp4j.util.DefaultPDUFactory#createPDU(MessageProcessingModel, int)} instead.
         */
        public PDU CreatePDU(ITarget target)
        {
            return incomingPDUFactory.CreatePDU(target);
        }

        /**
         * Gets the message ID that will be used for the next request to be sent by this message processing model.
         * @return
         *    the next message ID used by the MPv3.
         * @since 2.4.3
         */
        public int NextMsgID
        {
            get
            {
                return currentMsgID;
            }
        }

        /**
         * Sets the next message ID. According to RFC3412, the message ID should be unique across reboots:
         * "Values for msgID SHOULD be generated in a manner that avoids re-use
         * of any outstanding values.  Doing so provides protection against some
         * replay attacks.  One possible implementation strategy would be to use
         * the low-order bits of snmpEngineBoots [RFC3411] as the high-order
         * portion of the msgID value and a monotonically increasing integer for
         * the low-order portion of msgID."
         *
         * @param nextMsgID
         *    a message ID that has not been used by this SNMP entity yet (preferably also not
         *    used during previous runs).
         * @since 2.4.3
         */
        public void setCurrentMsgID(int nextMsgID)
        {
            this.currentMsgID = nextMsgID;
        }

        /// <summary>
        /// The <c>EngineIdCacheFactory</c> creates an engine ID cache with upper limit.
        /// </summary>
        public interface IEngineIdCacheFactory
        {
            /// <summary>
            /// Create a engine ID map with the given maximum capacity. If more than those engine IDs are added,
            /// the eldest engine IDs will be removed from the map before the new one is added.
            /// </summary>
            /// <param name="maximumCapacity">The upper limit of the number of engine ID mappings in this map</param>
            /// <returns>The created map</returns>
            IDictionary<IAddress, OctetString> CreateEngineIdMap(IEngineIdCacheSize maximumCapacity);
        }

        /// <summary>
        /// A concrete implementation of the <see cref="IEngineIdCacheFactory"/> interface.
        /// </summary>
        private class LimitedCapacityEngineIdCacheFactory : IEngineIdCacheFactory
        {
            public IDictionary<IAddress, OctetString> CreateEngineIdMap(IEngineIdCacheSize cacheSize)
            {
                return new System.Collections.Concurrent.ConcurrentDictionary<IAddress, OctetString>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class PDUv3Factory : IPDUFactory
        {
            public PDU CreatePDU(ITarget target)
            {
                return new ScopedPDU();
            }

            public PDU CreatePDU(MessageProcessingModel messageProcessingModel)
            {
                return new ScopedPDU();
            }
        }
    }
}
