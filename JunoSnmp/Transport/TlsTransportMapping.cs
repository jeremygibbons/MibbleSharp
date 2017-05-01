// <copyright file="TlsTransportMapping.cs" company="None">
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

namespace JunoSnmp.Transport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;


    /**
 * The <code>TLSTM</code> implements the Transport Layer Security
 * Transport Mapping (TLS-TM) as defined by RFC 5953
 * with the new IO API and {@link javax.net.ssl.SSLEngine}.
 * <p>
 * It uses a single thread for processing incoming and outgoing messages.
 * The thread is started when the <code>listen</code> method is called, or
 * when an outgoing request is sent using the <code>sendMessage</code> method.
 *
 * @author Frank Fock
 * @version 2.0
 * @since 2.0
 */
    public class TlsTransportMapping : TcpTransportMapping
    {

        private static readonly log4net.ILog log = log4net.LogManager
                  .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<IAddress, SocketEntry> sockets = new Dictionary<IAddress, SocketEntry>();
        private WorkerTask server;
        private ServerThread serverThread;

        private CommonTimer socketCleaner;
        // 1 minute default timeout
        private long connectionTimeout = 60000;
        private bool serverEnabled = false;

        private long nextSessionID = 1;

        private SSLEngineConfigurator sslEngineConfigurator =
            new DefaultSSLEngineConfiguration();

        private TlsTmSecurityCallback<X509Certificate> securityCallback;
        private CounterSupport counterSupport;

        public static readonly string DEFAULT_TLSTM_PROTOCOLS = "TLSv1";
        public static readonly int MAX_TLS_PAYLOAD_SIZE = 32 * 1024;

        private string localCertificateAlias;
        private string keyStore;
        private string keyStorePassword;
        private string[] tlsProtocols;
        private TLSTMTrustManagerFactory trustManagerFactory = new DefaultTLSTMTrustManagerFactory();

        /**
         * Creates a default TCP transport mapping with the server for incoming
         * messages disabled.
         * @throws UnknownHostException
         *    if the local host cannot be determined.
         */
        public TlsTransportMapping() : base(new TlsAddress(InetAddress.getLocalHost(), 0))
        {
            
            this.counterSupport = CounterSupport.Instance;
            base.maxInboundMessageSize = MAX_TLS_PAYLOAD_SIZE;
        }

        /**
         * Creates a TLS transport mapping with the server for incoming
         * messages bind to the given address. The <code>securityCallback</code>
         * needs to be specified before {@link #listen()} is called.
         *
         * @throws java.io.IOException
         *    on failure of binding a local port.
         */
        public TlsTransportMapping(TlsAddress address) : base(address)

        {
            base.maxInboundMessageSize = MAX_TLS_PAYLOAD_SIZE;
            this.serverEnabled = true;
            this.counterSupport = CounterSupport.Instance;
            try
            {
                if (Class.forName("javax.net.ssl.X509ExtendedTrustManager") != null)
                {
                    Class trustManagerFactoryClass =
                        Class.forName("org.snmp4j.transport.tls.TLSTMExtendedTrustManagerFactory");
                    Constructor c = trustManagerFactoryClass.getConstructors()[0];
                    TLSTMTrustManagerFactory trustManagerFactory =
                        (TLSTMTrustManagerFactory)c.newInstance(this);
                    TrustManagerFactory = trustManagerFactory;
                }
            }
            catch (ClassNotFoundException ex)
            {
                //throw new IOException("Failed to load TLSTMTrustManagerFactory: "+ex.getMessage(), ex);
            }
            catch (InvocationTargetException ex)
            {
                throw new IOException("Failed to init TLSTMTrustManagerFactory: " + ex.getMessage(), ex);
            }
            catch (IllegalArgumentException ex)
            {
                throw new IOException("Failed to setup TLSTMTrustManagerFactory: " + ex.getMessage(), ex);
            }
            catch (IllegalAccessException ex)
            {
                throw new IOException("Failed to init TLSTMTrustManagerFactory: " + ex.getMessage(), ex);
            }
            catch (InstantiationException ex)
            {
                throw new IOException("Failed to instantiate TLSTMTrustManagerFactory: " + ex.getMessage(), ex);
            }
        }

        /**
         * Creates a TLS transport mapping that binds to the given address
         * (interface) on the local host.
         *
         * @param securityCallback
         *    a security name callback to resolve X509 certificates to tmSecurityNames.
         * @param serverAddress
         *    the TcpAddress instance that describes the server address to listen
         *    on incoming connection requests.
         * @throws java.io.IOException
         *    if the given address cannot be bound.
         */
        public TlsTransportMapping(TlsTmSecurityCallback<X509Certificate> securityCallback,
                 TlsAddress serverAddress) : this(securityCallback, serverAddress, CounterSupport.Instance)
        {
        }

        /**
         * Creates a TLS transport mapping that binds to the given address
         * (interface) on the local host.
         *
         * @param securityCallback
         *    a security name callback to resolve X509 certificates to tmSecurityNames.
         * @param serverAddress
         *    the TcpAddress instance that describes the server address to listen
         *    on incoming connection requests.
         * @param counterSupport
         *    The CounterSupport instance to be used to count events created by this
         *    TLSTM instance. To get a default instance, use
         *    {@link CounterSupport#getInstance()}.
         * @throws java.io.IOException
         *    if the given address cannot be bound.
         */
        public TlsTransportMapping(TlsTmSecurityCallback<X509Certificate> securityCallback,
                 TlsAddress serverAddress, CounterSupport counterSupport) : base(serverAddress)
        {
            base.maxInboundMessageSize = MAX_TLS_PAYLOAD_SIZE;
            this.serverEnabled = true;
            this.securityCallback = securityCallback;
            this.counterSupport = counterSupport;
        }

        public string getLocalCertificateAlias()
        {
            if (localCertificateAlias == null)
            {
                return System.getProperty(SnmpConfigurator.P_TLS_LOCAL_ID, null);
            }
            return localCertificateAlias;
        }


        /**
         * Sets the TLS protocols/versions that TLSTM should use during handshake.
         * The default is defined by {@link #DEFAULT_TLSTM_PROTOCOLS}.
         *
         * @param tlsProtocols
         *    an array of TLS protocol (version) names supported by the SunJSSE provider.
         *    The order in the array defines which protocol is tried during handshake
         *    first.
         * @since 2.0.3
         */
        public string[] TlsProtocols
        {
            get
            {
                if (tlsProtocols == null)
                {
                    string s = System.getProperty(SnmpConfigurator.P_TLS_VERSION, DEFAULT_TLSTM_PROTOCOLS);
                    return s.Split(",", StringSplitOptions.None);
                }
                return tlsProtocols;
            }

            set
            {
                this.tlsProtocols = value;
            }
        }


        public string KeyStore
        {
            get
            {
                if (keyStore == null)
                {
                    return System.getProperty("javax.net.ssl.keyStore");
                }
                return keyStore;
            }

            set
            {
                this.keyStore = value;
            }
        }


        public string KeyStorePassword
        {
            get
            {
                if (keyStorePassword == null)
                {
                    return System.getProperty("javax.net.ssl.keyStorePassword");
                }
                return keyStorePassword;
            }

            set
            {
                this.keyStorePassword = value;
            }
        }

        /**
         * Sets the certificate alias used for client and server authentication
         * by this TLSTM. Setting this property to a value other than <code>null</code>
         * filters out any certificates which are not in the chain of the given
         * alias.
         *
         * @param localCertificateAlias
         *    a certificate alias which filters a single certification chain from
         *    the <code>javax.net.ssl.keyStore</code> key store to be used to
         *    authenticate this TLS transport mapping. If <code>null</code> no
         *    filtering appears, which could lead to more than a single chain
         *    available for authentication by the peer, which would violate the
         *    TLSTM standard requirements.
         */
        public void setLocalCertificateAlias(string localCertificateAlias)
        {
            this.localCertificateAlias = localCertificateAlias;
        }

        public CounterSupport getCounterSupport()
        {
            return counterSupport;
        }

        public Type GetSupportedAddressClass()
        {
            return typeof(TlsAddress);
        }


        public TlsTmSecurityCallback<X509Certificate> SecurityCallback
        {
            get
            {
                return securityCallback;
            }

            set
            {
                this.securityCallback = value;
            }
        }


        /**
         * Set the TLSTM trust manager factory. Using a trust manager factory other than the
         * default allows to add support for Java 1.7 X509ExtendedTrustManager.
         * @param trustManagerFactory
         *   a X.509 trust manager factory implementing the interface {@link TLSTMTrustManagerFactory}.
         * @since 2.0.3
         */
        public TLSTMTrustManagerFactory TrustManagerFactory
        {
            get
            {
                return trustManagerFactory;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }
                this.trustManagerFactory = value;
            }
        }

        /**
         * Listen for incoming and outgoing requests. If the <code>serverEnabled</code>
         * member is <code>false</code> the server for incoming requests is not
         * started. This starts the internal server thread that processes messages.
         * @throws java.net.SocketException
         *    when the transport is already listening for incoming/outgoing messages.
         * @throws java.io.IOException
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Listen()
        {
            if (server != null)
            {
                throw new SocketException("Port already listening");
            }
            try
            {
                serverThread = new ServerThread();
            }
            catch (NoSuchAlgorithmException e)
            {
                throw new IOException("SSL not available: " + e.getMessage(), e);
            }
            server = JunoSnmpSettings.getThreadFactory().createWorkerThread(
              "TLSTM_" + getAddress(), serverThread, true);
            if (connectionTimeout > 0)
            {
                // run as daemon
                socketCleaner = JunoSnmpSettings.getTimerFactory().createTimer();
            }
            server.run();
        }

        /**
         * Returns the name of the listen thread.
         * @return
         *    the thread name if in listening mode, otherwise <code>null</code>.
         * @since 1.6
         */

        /**
         * Sets the name of the listen thread for this UDP transport mapping.
         * This method has no effect, if called before {@link #listen()} has been
         * called for this transport mapping.
         *
         * @param name
         *    the new thread name.
         * @since 1.6
         */
        public string ThreadName
        {
            get
            {
                WorkerTask st = server;
                if (st != null)
                {
                    return ((Thread)st).getName();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                WorkerTask st = server;
                if (st is Thread)
                {
                    ((Thread)st).setName(value);
                }
            }
        }

        /**
         * Closes all open sockets and stops the internal server thread that
         * processes messages.
         */
        public override void Close()
        {
            foreach (SocketEntry entry in sockets.values())
            {
                entry.closeSession();
            }

            WorkerTask st = server;
            if (st != null)
            {
                st.terminate();
                st.interrupt();
                try
                {
                    st.join();
                }
                catch (InterruptedException ex)
                {
                    log.Warn(ex);
                }
                server = null;
                foreach (SocketEntry entry in sockets.values())
                {
                    Socket s = entry.getSocket();
                    if (s != null)
                    {
                        try
                        {
                            SocketChannel sc = s.getChannel();
                            s.close();
                            if (log.IsDebugEnabled)
                            {
                                log.Debug("Socket to " + entry.getPeerAddress() + " closed");
                            }
                            if (sc != null)
                            {
                                sc.close();
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug("Socket channel to " +
                                        entry.getPeerAddress() + " closed");
                                }
                            }
                        }
                        catch (IOException iox)
                        {
                            // ignore
                            log.Debug(iox);
                        }
                    }
                }
                if (socketCleaner != null)
                {
                    socketCleaner.cancel();
                }
                socketCleaner = null;
            }
        }

        /**
         * Closes a connection to the supplied remote address, if it is open. This
         * method is particularly useful when not using a timeout for remote
         * connections.
         *
         * @param remoteAddress
         *    the address of the peer socket.
         * @return
         *    <code>true</code> if the connection has been closed and
         *    <code>false</code> if there was nothing to close.
         * @throws java.io.IOException
         *    if the remote address cannot be closed due to an IO exception.
         * @since 1.7.1
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool close(TcpAddress remoteAddress)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Closing socket for peer address " + remoteAddress);
            }
            SocketEntry entry = sockets.remove(remoteAddress);
            if (entry != null)
            {
                Socket s = entry.getSocket();
                if (s != null)
                {
                    SocketChannel sc = entry.getSocket().getChannel();
                    entry.getSocket().close();
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Socket to " + entry.PeerAddress + " closed");
                    }
                    if (sc != null)
                    {
                        sc.close();
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Closed socket channel for peer address " +
                                         remoteAddress);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /**
         * Sends a SNMP message to the supplied address.
         * @param address
         *    an <code>TcpAddress</code>. A <code>ClassCastException</code> is thrown
         *    if <code>address</code> is not a <code>TcpAddress</code> instance.
         * @param message byte[]
         *    the message to sent.
         * @param tmStateReference
         *    the (optional) transport model state reference as defined by
         *    RFC 5590 section 6.1.
         * @throws java.io.IOException
         */
        public override void SendMessage(TcpAddress address, byte[] message,
                                TransportStateReference tmStateReference)

        {
            if (server == null)
            {
                Listen();
            }
            serverThread.SendMessage(address, message, tmStateReference);
        }

        /**
         * Gets the connection timeout. This timeout specifies the time a connection
         * may be idle before it is closed.
         * @return long
         *    the idle timeout in milliseconds.
         */

        /**
         * Sets the connection timeout. This timeout specifies the time a connection
         * may be idle before it is closed.
         * @param connectionTimeout
         *    the idle timeout in milliseconds. A zero or negative value will disable
         *    any timeout and connections opened by this transport mapping will stay
         *    opened until they are explicitly closed.
         */
        public long ConnectionTimeout
        {
            get
            {
                return connectionTimeout;
            }

            set
            {
                this.connectionTimeout = value;
            }
        }

        /**
         * Checks whether a server for incoming requests is enabled.
         * @return bool
         */

        /**
         * Sets whether a server for incoming requests should be created when
         * the transport is set into listen state. Setting this value has no effect
         * until the {@link #listen()} method is called (if the transport is already
         * listening, {@link #close()} has to be called before).
         * @param serverEnabled
         *    if <code>true</code> if the transport will listens for incoming
         *    requests after {@link #listen()} has been called.
         */
        public bool ServerEnabled
        {
            get
            {
                return serverEnabled;
            }

            set
            {
                this.serverEnabled = value;
            }
        }

        public override MessageLengthDecoder MessageLengthDecoder
        {
            get
            {
                return null;
            }

            set
            {
                /*
                    if (messageLengthDecoder == null) {
                      throw new NullPointerException();
                    }
                    this.messageLengthDecoder = messageLengthDecoder;
                    */
            }
        }

        /**
         * Gets the inbound buffer size for incoming requests. When SNMP packets are
         * received that are longer than this maximum size, the messages will be
         * silently dropped and the connection will be closed.
         * @return
         *    the maximum inbound buffer size in bytes.
         */

        /**
         * Sets the maximum buffer size for incoming requests. When SNMP packets are
         * received that are longer than this maximum size, the messages will be
         * silently dropped and the connection will be closed.
         * @param maxInboundMessageSize
         *    the length of the inbound buffer in bytes.
         */
        public override int MaxInboundMessageSize
        {
            get
            {
                return base.MaxInboundMessageSize;
            }

            set
            {
                this.maxInboundMessageSize = value;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void timeoutSocket(SocketEntry entry)
        {
            if (connectionTimeout > 0)
            {
                socketCleaner.schedule(new SocketTimeout(entry), connectionTimeout);
            }
        }

        public bool isListening()
        {
            return (server != null);
        }

        public static OctetString GetFingerprint(X509Certificate cert)
        {
            OctetString certFingerprint = null;
            try
            {
                string algo = cert.getSigAlgName();
                if (algo.Contains("with"))
                {
                    algo = algo.Substring(0, algo.IndexOf("with"));
                }
                MessageDigest md = MessageDigest.getInstance(algo);
                md.update(cert.getEncoded());
                certFingerprint = new OctetString(md.digest());
            }
            catch (NoSuchAlgorithmException e)
            {
                log.Error("No such digest algorithm exception while getting fingerprint from " +
                             cert + ": " + e.getMessage(), e);
            }
            catch (CertificateEncodingException e)
            {
                log.Error("Certificate encoding exception while getting fingerprint from " +
                             cert + ": " + e.getMessage(), e);
            }
            return certFingerprint;
        }

        public static object getSubjAltName(Collection<List<?>> subjAltNames, int type)
        {
            if (subjAltNames != null)
            {
                foreach (List <?> entry in subjAltNames)
                {
                    int t = (Integer)entry.get(0);
                    if (t == type)
                    {
                        return entry.get(1);
                    }
                }
            }
            return null;
        }

        /**
         * Sets optional server socket options. The default implementation does
         * nothing.
         * @param serverSocket
         *    the <code>ServerSocket</code> to apply additional non-default options.
         */
        protected void setSocketOptions(ServerSocket serverSocket)
        {
        }

        class SocketEntry
        {
            private Socket socket;
            private TcpAddress peerAddress;
            private long lastUse;
            private LinkedList<byte[]> message = new LinkedList<byte[]>();
            private MemoryStream inNetBuffer;
            private MemoryStream inAppBuffer;
            private MemoryStream outAppBuffer;
            private MemoryStream outNetBuffer;
            private volatile int registrations = 0;
            private SSLEngine sslEngine;
            private long sessionID;
            private TransportStateReference tmStateReference;
            private bool handshakeFinished;
            private TlsTransportMapping transportMapping;

            private object outboundLock = new object();
            private object inboundLock = new object();

            public SocketEntry(TlsTransportMapping transportMapping, TcpAddress address, Socket socket,
                               bool useClientMode,
                               TransportStateReference tmStateReference)
            {
                this.transportMapping = transportMapping;
                this.inAppBuffer = new MemoryStream(transportMapping.MaxInboundMessageSize);
                this.inNetBuffer = new MemoryStream(transportMapping.MaxInboundMessageSize);
                this.outNetBuffer = new MemoryStream(transportMapping.MaxInboundMessageSize);
                this.peerAddress = address;
                this.tmStateReference = tmStateReference;
                this.socket = socket;
                this.lastUse = System.DateTime.Now.Ticks;
                if (tmStateReference == null)
                {
                    transportMapping.counterSupport.IncrementCounter(this, new CounterIncrArgs(SnmpConstants.snmpTlstmSessionAccepts));
                }

                SSLContext sslContext = sslEngineConfigurator.getSSLContext(useClientMode, tmStateReference);
                this.sslEngine = sslContext.createSSLEngine(address.getInetAddress().getHostName(), address.getPort());
                sslEngine.setUseClientMode(useClientMode);
                //      sslEngineConfigurator.configure(SSLContext.getDefault(), useClientMode);
                sslEngineConfigurator.configure(sslEngine);
                lock(this) {
                    sessionID = transportMapping.nextSessionID++;
                }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void AddRegistration(Selector selector, int opKey)

            {
                if ((this.registrations & opKey) == 0)
                {
                    this.registrations |= opKey;
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Adding operation " + opKey + " for: " + ToString());
                    }
                    socket.getChannel().register(selector, registrations, this);
                }
                else if (!socket.getChannel().isRegistered())
                {
                    this.registrations = opKey;
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Registering new operation " + opKey + " for: " + ToString());
                    }

                    socket.getChannel().register(selector, opKey, this);
                }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void RemoveRegistration(Selector selector, int opKey)

            {
                if ((this.registrations & opKey) == opKey)
                {
                    this.registrations &= ~opKey;
                    socket.getChannel().register(selector, this.registrations, this);
                }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public bool IsRegistered(int opKey)
            {
                return (this.registrations & opKey) == opKey;
            }

            public long LastUse
            {
                get
                {
                    return lastUse;
                }
            }

            public void Used()
            {
                lastUse = System.DateTime.Now.Ticks;
            }

            public Socket GetSocket()
            {
                return socket;
            }

            public TcpAddress PeerAddress
            {
                get
                {
                    return peerAddress;
                }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void AddMessage(byte[] message)
            {
                this.message.AddLast(message);
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public byte[] NextMessage()
            {
                if (this.message.size() > 0)
                {
                    return this.message.removeFirst();
                }
                return null;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            public bool HasMessage()
            {
                return this.message.Count > 0;
            }

            public MemoryStream InNetBuffer
            {
                get
                {
                    return inNetBuffer;
                }

                set
                {
                    this.inNetBuffer = value;
                }
            }

            public MemoryStream OutNetBuffer
            {
                get
                {
                    return outNetBuffer;
                }

                set
                {
                    this.outNetBuffer = value;
                }
            }

            public override string ToString()
            {
                return "SocketEntry[peerAddress=" + peerAddress +
                    ",socket=" + socket + ",lastUse=" + new Date(lastUse / SnmpConstants.MILLISECOND_TO_NANOSECOND) +
                    ",inNetBuffer=" + inNetBuffer +
                    ",inAppBuffer=" + inAppBuffer +
                    ",outNetBuffer=" + outNetBuffer +
                    "]";
            }

            /*
            public bool equals(object o) {
              if (o instanceof SocketEntry) {
                SocketEntry other = (SocketEntry)o;
                return other.peerAddress.equals(peerAddress) &&
                    ((other.message == message) ||
                     ((message != null) && (message.equals(other.message))));
              }
              return false;
            }

            public int hashCode() {
              return peerAddress.hashCode();
            }
        */

            public void CheckTransportStateReference()
            {
                if (tmStateReference == null)
                {
                    tmStateReference =
                        new TransportStateReference(TLSTM.this, peerAddress, new OctetString(),
                            SecurityLevel.authPriv, SecurityLevel.authPriv,
                            true, sessionID);
                    OctetString securityName = null;
                    if (securityCallback != null)
                    {
                        try
                        {
                            securityName = securityCallback.getSecurityName(
                                (X509Certificate[])sslEngine.getSession().getPeerCertificates());
                        }
                        catch (SSLPeerUnverifiedException e)
                        {
                            log.Error("SSL peer '" + peerAddress + "' is not verified: " + e.getMessage(), e);
                            sslEngine.setEnableSessionCreation(false);
                        }
                    }
                    tmStateReference.setSecurityName(securityName);
                }
                else if (tmStateReference.getTransportSecurityLevel().equals(SecurityLevel.undefined))
                {
                    tmStateReference.setTransportSecurityLevel(SecurityLevel.authPriv);
                }
            }

            public MemoryStream InAppBuffer
            {
                get
                {
                    return inAppBuffer;
                }

                set
                {
                    this.inAppBuffer = value;
                }
            }

            public bool HandshakeFinished
            {
                get
                {
                    return handshakeFinished;
                }

                set
                {
                    this.handshakeFinished = value;
                }
            }

            public bool IsAppOutPending()
            {
                lock (outboundLock)
                {
                    return (outAppBuffer != null) && (outAppBuffer.limit() > 0);
                }
            }

            public long GetSessionID()
            {
                return sessionID;
            }

            public void CloseSession()
            {
                sslEngine.closeOutbound();
                counterSupport.fireIncrementCounter(new CounterEvent(this, SnmpConstants.snmpTlstmSessionServerCloses));
                try
                {
                    SSLEngineResult result;
                    do
                    {
                        result = sendNetMessage(this);
                    }
                    while ((result.getStatus() != SSLEngineResult.Status.CLOSED) &&
                           (result.getHandshakeStatus() == SSLEngineResult.HandshakeStatus.NEED_WRAP));

                }
                catch (IOException e)
                {
                    log.Error("IOException while closing outbound channel of " + this + ": " + e.getMessage(), e);
                }
                /*
                if (sslEngine.isOutboundDone()) {
                  // try to receive close alert message
                  SSLEngineResult result;
                  try {
                    int i=0;
                    do {
                      synchronized (this.inboundLock) {
                        this.inNetBuffer.flip();
                        this.inNetBuffer.limit(this.inNetBuffer.capacity());
                        log.Debug("TLS inNetBuffer = "+this.inNetBuffer);
                        result =
                            this.sslEngine.unwrap(this.inNetBuffer, this.inAppBuffer);
          //              adjustInNetBuffer(this, result);
                      }
                    }
                    while ((result.getStatus() != SSLEngineResult.Status.CLOSED) && (i++ < 5) && !sslEngine.isInboundDone() &&
                           (result.getHandshakeStatus() == SSLEngineResult.HandshakeStatus.NEED_UNWRAP));
                    sslEngine.closeInbound();
                  } catch (SSLException e) {
                    log.Error("SSLException while closing inbound channel of " + this + ": " + e.getMessage(), e);
                  }
                }
                */
            }
        }

        class SocketTimeout : TimerTask
        {
            private SocketEntry entry;

            public SocketTimeout(SocketEntry entry)
            {
                this.entry = entry;
            }

            /**
             * run
             */
            public void Run()
            {
                long now = System.nanoTime();
                if ((socketCleaner == null) ||
                    ((now - entry.getLastUse()) / SnmpConstants.MILLISECOND_TO_NANOSECOND >= connectionTimeout))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Socket has not been used for " +
                                     (now - entry.getLastUse()) +
                                     " milliseconds, closing it");
                    }
                    sockets.remove(entry.getPeerAddress());
                    SocketEntry entryCopy = entry;
                    try
                    {
                        synchronized(entryCopy) {
                            entryCopy.getSocket().close();
                        }
                        log.Info("Socket to " + entryCopy.getPeerAddress() +
                                    " closed due to timeout");
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                    }
                }
                else
                {
                    long nextRun = System.currentTimeMillis() +
                        (now - entry.getLastUse()) / SnmpConstants.MILLISECOND_TO_NANOSECOND + connectionTimeout;
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Scheduling " + nextRun);
                    }
                    socketCleaner.schedule(new SocketTimeout(entry), nextRun);
                }
            }

            public bool Cancel()
            {
                bool result = super.cancel();
                // free objects early
                entry = null;
                return result;
            }
        }

        class ServerThread : WorkerTask
        {

            private volatile bool stop = false;
            private Throwable lastError = null;
            private ServerSocketChannel ssc;
            private Selector selector;

            private LinkedList<SocketEntry> pending = new LinkedList<SocketEntry>();
            private BlockingQueue<SocketEntry> outQueue = new LinkedBlockingQueue<SocketEntry>();
            private BlockingQueue<SocketEntry> inQueue = new LinkedBlockingQueue<SocketEntry>();

            public ServerThread(), NoSuchAlgorithmException {
      // Selector for incoming requests
      selector = Selector.open();
      if (serverEnabled) {
        // Create a new server socket and set to non blocking mode
        ssc = ServerSocketChannel.open();
        ssc.configureBlocking(false);

        // Bind the server socket
        InetSocketAddress isa = new InetSocketAddress(tcpAddress.getInetAddress(),
            tcpAddress.getPort());
        setSocketOptions(ssc.socket());
        ssc.socket().bind(isa);
            // Register accepts on the server socket with the selector. This
            // step tells the selector that the socket wants to be put on the
            // ready list when accept operations occur, so allowing multiplexed
            // non-blocking I/O to take place.
            ssc.register(selector, SelectionKey.OP_ACCEPT);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private void ProcessQueues()
    {
        while (!outQueue.isEmpty() || !inQueue.isEmpty())
        {
            while (!outQueue.isEmpty())
            {
                SocketEntry entry = null;
                try
                {
                    SSLEngineResult result;
                    entry = outQueue.take();
                    result = sendNetMessage(entry);
                    if ((result != null) && runDelegatedTasks(result, entry))
                    {
                        if (entry.isAppOutPending())
                        {
                            writeMessage(entry, entry.getSocket().getChannel());
                        }
                    }
                }
                catch (IOException iox)
                {
                    log.Error("IO exception caught while SSL processing: " + iox.getMessage(), iox);
                    while (inQueue.remove(entry))
                    {
                        // no body
                    }
                }
                catch (InterruptedException e)
                {
                    log.Error("SSL processing interrupted: " + e.getMessage(), e);
                    return;
                }
            }
            while (!inQueue.isEmpty())
            {
                SocketEntry entry = null;
                try
                {
                    entry = inQueue.take();
                    synchronized(entry.inboundLock) {
                        entry.inNetBuffer.flip();
                        log.Debug("TLS inNetBuffer = " + entry.inNetBuffer);
                        SSLEngineResult nextResult =
                            entry.sslEngine.unwrap(entry.inNetBuffer, entry.inAppBuffer);
                        adjustInNetBuffer(entry, nextResult);
                        if (runDelegatedTasks(nextResult, entry))
                        {
                            switch (nextResult.getStatus())
                            {
                                case BUFFER_UNDERFLOW:
                                    entry.inNetBuffer.limit(entry.inNetBuffer.capacity());
                                    entry.addRegistration(selector, SelectionKey.OP_READ);
                                    break;
                                case BUFFER_OVERFLOW:
                                    // TODO
                                    break;
                                case CLOSED:
                                    continue;
                                case OK:
                                    if (entry.isAppOutPending())
                                    {
                                        // we have a message to send
                                        writeMessage(entry, entry.getSocket().getChannel());
                                    }
                                    entry.inAppBuffer.flip();
                                    log.Debug("Dispatching inAppBuffer=" + entry.inAppBuffer);
                                    if (entry.inAppBuffer.limit() > 0)
                                    {
                                        dispatchMessage(entry.getPeerAddress(),
                                            entry.inAppBuffer, entry.inAppBuffer.limit(),
                                            entry.sessionID, entry.tmStateReference);
                                    }
                                    entry.inAppBuffer.clear();
                            }
                        }
                    }
                }
                catch (IOException iox)
                {
                    log.Error("IO exception caught while SSL processing: " + iox.getMessage(), iox);
                    while (inQueue.remove(entry))
                    {
                        // no body
                    }
                }
                catch (InterruptedException e)
                {
                    log.Error("SSL processing interrupted: " + e.getMessage(), e);
                    return;
                }
            }
        }
    }

    private void ProcessPending()
    {
        lock (pending)
        {
            for (int i = 0; i < pending.size(); i++)
            {
                SocketEntry entry = pending.getFirst();
                try
                {
                    // Register the channel with the selector, indicating
                    // interest in connection completion and attaching the
                    // target object so that we can get the target back
                    // after the key is added to the selector's
                    // selected-key set
                    if (entry.getSocket().isConnected())
                    {
                        if (entry.isHandshakeFinished())
                        {
                            entry.addRegistration(selector, SelectionKey.OP_WRITE);
                        }
                    }
                    else
                    {
                        entry.addRegistration(selector, SelectionKey.OP_CONNECT);
                    }
                }
                catch (CancelledKeyException ckex)
                {
                    log.Warn(ckex);
                    pending.remove(entry);
                    try
                    {
                        entry.getSocket().getChannel().close();
                        TransportStateEvent e =
                            new TransportStateEvent(TLSTM.this,
                                                    entry.getPeerAddress(),
                                                    TransportStateEvent.STATE_CLOSED,
                                                    null);
                        fireConnectionStateChanged(e);
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                    }
                }
                catch (IOException iox)
                {
                    log.Error(iox);
                    pending.remove(entry);
                    // Something went wrong, so close the channel and
                    // record the failure
                    try
                    {
                        entry.getSocket().getChannel().close();
                        TransportStateEvent e =
                            new TransportStateEvent(TLSTM.this,
                                                    entry.getPeerAddress(),
                                                    TransportStateEvent.STATE_CLOSED,
                                                    iox);
                        fireConnectionStateChanged(e);
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                    }
                    lastError = iox;
                    if (JunoSnmpSettings.isForwardRuntimeExceptions())
                    {
                        throw new RuntimeException(iox);
                    }
                }
            }
        }
    }

    /**
     * If the result indicates that we have outstanding tasks to do,
     * go ahead and run them in this thread.
     * @param result
     *    the SSLEngine wrap/unwrap result.
     * @param entry
     *    the session to use.
     * @return
     *    <code>true</code> if processing of delegated tasks has been
     *    finished, <code>false</code> otherwise.
     */
    public bool RunDelegatedTasks(SSLEngineResult result,
                                     SocketEntry entry)
    {
        if (log.IsDebugEnabled)
        {
            log.Debug("Running delegated task on " + entry + ": " + result);
        }
        SSLEngineResult.HandshakeStatus status = result.getHandshakeStatus();
        if (status == SSLEngineResult.HandshakeStatus.NEED_TASK)
        {
            Runnable runnable;
            while ((runnable = entry.sslEngine.getDelegatedTask()) != null)
            {
                log.Debug("Running delegated task...");
                runnable.run();
            }
            status = entry.sslEngine.getHandshakeStatus();
            if (status == SSLEngineResult.HandshakeStatus.NEED_TASK)
            {
                throw new IOException("Inconsistent Handshake status");
            }
            log.Info("Handshake status = " + status);
        }
        System.err.println("TASK:" + result);
        switch (result.getStatus())
        {
            case BUFFER_UNDERFLOW:
                entry.inNetBuffer.limit(entry.inNetBuffer.capacity());
                entry.addRegistration(selector, SelectionKey.OP_READ);
                return false;
            case CLOSED:
                return false;
        }
        switch (status)
        {
            case NEED_WRAP:
                outQueue.add(entry);
                //          entry.addRegistration(selector, SelectionKey.OP_WRITE);
                break;
            case NEED_UNWRAP:
                log.Debug("NEED_UNRWAP processing with inNetBuffer=" + entry.inNetBuffer);
                inQueue.add(entry);
                entry.addRegistration(selector, SelectionKey.OP_READ);
                break;
            case FINISHED:
                log.Debug("TLS handshake finished");
                entry.setHandshakeFinished(true);/*
          if (result.bytesProduced() > 0) {
            writeNetBuffer(entry, entry.getSocket().getChannel());
          }
          /*
          if (entry.isAppOutPending()) {
            writeMessage(entry, entry.getSocket().getChannel());
          }
          */
                                                 // fall through
            case NOT_HANDSHAKING:
                if (result.bytesProduced() > 0)
                {
                    writeNetBuffer(entry, entry.getSocket().getChannel());
                }
                return true;
        }
        return false;
    }

    public Throwable GetLastError()
    {
        return lastError;
    }

    public void SendMessage(Address address, byte[] message,
                            TransportStateReference tmStateReference)

    {
        Socket s = null;
        SocketEntry entry = sockets.get(address);
        if (log.IsDebugEnabled)
        {
            log.Debug("Looking up connection for destination '" + address +
                         "' returned: " + entry);
            log.Debug(sockets.toString());
        }
        if (entry != null)
        {
            if ((tmStateReference != null) && (tmStateReference.getSessionID() != null) &&
                (!tmStateReference.getSessionID().equals(entry.getSessionID())))
            {
                // session IDs do not match -> drop message
                counterSupport.fireIncrementCounter(
                    new CounterEvent(this, SnmpConstants.snmpTlstmSessionNoSessions));
                throw new IOException("Session " + tmStateReference.getSessionID() + " not available");
            }
            s = entry.getSocket();
        }
        if ((s == null) || (s.isClosed()) || (!s.isConnected()))
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Socket for address '" + address +
                             "' is closed, opening it...");
            }
            synchronized(pending) {
                pending.remove(entry);
            }
            SocketChannel sc;
            try
            {
                InetSocketAddress targetAddress =
                    new InetSocketAddress(((TcpAddress)address).getInetAddress(),
                                          ((TcpAddress)address).getPort());
                if ((s == null) || (s.isClosed()))
                {
                    // Open the channel, set it to non-blocking, initiate connect
                    sc = SocketChannel.open();
                    sc.configureBlocking(false);
                    sc.connect(targetAddress);
                    counterSupport.fireIncrementCounter(
                        new CounterEvent(this, SnmpConstants.snmpTlstmSessionOpens));
                }
                else
                {
                    sc = s.getChannel();
                    sc.configureBlocking(false);
                    if (!sc.isConnectionPending())
                    {
                        sc.connect(targetAddress);
                        counterSupport.fireIncrementCounter(
                            new CounterEvent(this, SnmpConstants.snmpTlstmSessionOpens));
                    }
                    else
                    {
                        if (matchingStateReferences(tmStateReference, entry.tmStateReference))
                        {
                            entry.addMessage(message);
                            synchronized(pending) {
                                pending.add(entry);
                            }
                            selector.wakeup();
                            return;
                        }
                        else
                        {
                            log.Error("TransportStateReferences refNew=" + tmStateReference +
                                         ",refOld=" + entry.tmStateReference + " do not match, message dropped");
                            throw new IOException("Transport state reference does not match existing reference" +
                                " for this session/target");
                        }
                    }
                }
                s = sc.socket();
                entry = new SocketEntry((TcpAddress)address, s, true, tmStateReference);
                entry.addMessage(message);
                sockets.put(address, entry);

                synchronized(pending) {
                    pending.add(entry);
                }

                selector.wakeup();
                log.Debug("Trying to connect to " + address);
            }
            catch (IOException iox)
            {
                log.Error(iox);
                throw iox;
            }
            catch (NoSuchAlgorithmException e)
            {
                log.Error("NoSuchAlgorithmException while sending message to " + address + ": " + e.getMessage(), e);
            }
        }
        else if (matchingStateReferences(tmStateReference, entry.tmStateReference))
        {
            entry.addMessage(message);
            synchronized(pending) {
                pending.addFirst(entry);
            }
            log.Debug("Waking up selector for new message");
            selector.wakeup();
        }
        else
        {
            log.Error("TransportStateReferences refNew=" + tmStateReference +
                ",refOld=" + entry.tmStateReference + " do not match, message dropped");
            throw new IOException("Transport state reference does not match existing reference" +
                " for this session/target");

        }
    }


    public void Run()
    {
        // Here's where everything happens. The select method will
        // return when any operations registered above have occurred, the
        // thread has been interrupted, etc.
        try
        {
            while (!stop)
            {
                try
                {
                    processQueues();
                    if (selector.select() > 0)
                    {
                        if (stop)
                        {
                            break;
                        }
                        // Someone is ready for I/O, get the ready keys
                        Set<SelectionKey> readyKeys = selector.selectedKeys();
                        Iterator<SelectionKey> it = readyKeys.iterator();

                        // Walk through the ready keys collection and process date requests.
                        while (it.hasNext())
                        {
                            try
                            {
                                SocketEntry entry = null;
                                SelectionKey sk = it.next();
                                it.remove();
                                SocketChannel readChannel = null;
                                TcpAddress incomingAddress = null;
                                if (sk.isAcceptable())
                                {
                                    log.Debug("Key is acceptable");
                                    // The key indexes into the selector so you
                                    // can retrieve the socket that's ready for I/O
                                    ServerSocketChannel nextReady =
                                        (ServerSocketChannel)sk.channel();
                                    Socket s = nextReady.accept().socket();
                                    readChannel = s.getChannel();
                                    readChannel.configureBlocking(false);

                                    incomingAddress = new TcpAddress(s.getInetAddress(),
                                                                     s.getPort());
                                    entry = new SocketEntry(incomingAddress, s, false, null);
                                    entry.addRegistration(selector, SelectionKey.OP_READ);
                                    sockets.put(incomingAddress, entry);
                                    timeoutSocket(entry);
                                    TransportStateEvent e =
                                        new TransportStateEvent(TLSTM.this,
                                                                incomingAddress,
                                                                TransportStateEvent.
                                                                STATE_CONNECTED,
                                                                null);
                                    fireConnectionStateChanged(e);
                                    if (e.isCancelled())
                                    {
                                        log.Warn("Incoming connection cancelled");
                                        s.close();
                                        sockets.remove(incomingAddress);
                                        readChannel = null;
                                    }
                                }
                                else if (sk.isWritable())
                                {
                                    log.Debug("Key is writable");
                                    incomingAddress = writeData(sk, incomingAddress);
                                }
                                else if (sk.isReadable())
                                {
                                    log.Debug("Key is readable");
                                    readChannel = (SocketChannel)sk.channel();
                                    incomingAddress =
                                        new TcpAddress(readChannel.socket().getInetAddress(),
                                                       readChannel.socket().getPort());
                                }
                                else if (sk.isConnectable())
                                {
                                    log.Debug("Key is connectable");
                                    connectChannel(sk, incomingAddress);
                                }

                                if (readChannel != null)
                                {
                                    log.Debug("Key is reading");
                                    try
                                    {
                                        readMessage(sk, readChannel, incomingAddress, entry);
                                    }
                                    catch (IOException iox)
                                    {
                                        // IO exception -> channel closed remotely
                                        log.Warn(iox);
                                        iox.printStackTrace();
                                        sk.cancel();
                                        readChannel.close();
                                        TransportStateEvent e =
                                            new TransportStateEvent(TLSTM.this,
                                                                    incomingAddress,
                                                                    TransportStateEvent.
                                                                    STATE_DISCONNECTED_REMOTELY,
                                                                    iox);
                                        fireConnectionStateChanged(e);
                                    }
                                }
                            }
                            catch (CancelledKeyException ckex)
                            {
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug("Selection key cancelled, skipping it");
                                }
                            }
                            catch (NoSuchAlgorithmException e)
                            {
                                log.Error("NoSuchAlgorithm while reading from server socket: " + e.getMessage(), e);
                            }
                        }
                    }
                }
                catch (NullPointerException npex)
                {
                    // There seems to happen a NullPointerException within the select()
                    npex.printStackTrace();
                    log.Warn("NullPointerException within select()?");
                    stop = true;
                }
                processPending();
            }
            if (ssc != null)
            {
                ssc.close();
            }
            if (selector != null)
            {
                selector.close();
            }
        }
        catch (IOException iox)
        {
            log.Error(iox);
            lastError = iox;
        }
        if (!stop)
        {
            stop = true;
            synchronized(TLSTM.this) {
                server = null;
            }
        }
        if (log.IsDebugEnabled)
        {
            log.Debug("Worker task finished: " + getClass().getName());
        }
    }

    private void connectChannel(SelectionKey sk, TcpAddress incomingAddress)
    {
        SocketEntry entry = (SocketEntry)sk.attachment();
        try
        {
            SocketChannel sc = (SocketChannel)sk.channel();
            if (!sc.isConnected())
            {
                if (sc.finishConnect())
                {
                    sc.configureBlocking(false);
                    log.Debug("Connected to " + entry.getPeerAddress());
                    // make sure connection is closed if not used for timeout
                    // micro seconds
                    timeoutSocket(entry);
                    entry.removeRegistration(selector, SelectionKey.OP_CONNECT);
                    entry.addRegistration(selector, SelectionKey.OP_WRITE);
                }
                else
                {
                    entry = null;
                }
            }
            if (entry != null)
            {
                Address addr = (incomingAddress == null) ?
                                            entry.getPeerAddress() : incomingAddress;
                log.Debug("Fire connected event for " + addr);
                TransportStateEvent e =
                    new TransportStateEvent(TLSTM.this,
                                            addr,
                                            TransportStateEvent.
                                            STATE_CONNECTED,
                                            null);
                fireConnectionStateChanged(e);
            }
        }
        catch (IOException iox)
        {
            log.Warn(iox);
            sk.cancel();
            closeChannel(sk.channel());
            if (entry != null)
            {
                pending.remove(entry);
            }
        }
    }

    private TcpAddress WriteData(SelectionKey sk, TcpAddress incomingAddress)
    {
        SocketEntry entry = (SocketEntry)sk.attachment();
        try
        {
            SocketChannel sc = (SocketChannel)sk.channel();
            incomingAddress =
                new TcpAddress(sc.socket().getInetAddress(),
                               sc.socket().getPort());
            if ((entry != null) && (!entry.hasMessage()))
            {
                synchronized(pending) {
                    pending.remove(entry);
                    entry.removeRegistration(selector, SelectionKey.OP_WRITE);
                }
            }
            if (entry != null)
            {
                writeMessage(entry, sc);
            }
        }
        catch (IOException iox)
        {
            log.Warn(iox);
            TransportStateEvent e =
                new TransportStateEvent(TLSTM.this,
                                        incomingAddress,
                                        TransportStateEvent.
                                        STATE_DISCONNECTED_REMOTELY,
                                        iox);
            fireConnectionStateChanged(e);
            // make sure channel is closed properly:
            closeChannel(sk.channel());
        }
        return incomingAddress;
    }

    private void CloseChannel(SelectableChannel channel)
    {
        try
        {
            channel.close();
        }
        catch (IOException channelCloseException)
        {
            log.Warn(channelCloseException);
        }
    }

    private void ReadMessage(SelectionKey sk, SocketChannel readChannel,
                             TcpAddress incomingAddress,
                             SocketEntry session)
    {
        SocketEntry entry = (SocketEntry)sk.attachment();
        if (entry == null)
        {
            entry = session;
        }
        if (entry == null)
        {
            log.Error("SocketEntry null in readMessage");
        }
        assert(entry != null);
        // note that socket has been used
        entry.used();
        ByteBuffer inNetBuffer = entry.getInNetBuffer();
        ByteBuffer inAppBuffer = entry.getInAppBuffer();
        try
        {
            long bytesRead = readChannel.read(inNetBuffer);
            inNetBuffer.flip();
            if (log.IsDebugEnabled)
            {
                log.Debug("Read " + bytesRead + " bytes from " + incomingAddress);
                log.Debug("TLS inNetBuffer: " + inNetBuffer);
            }
            if (bytesRead < 0)
            {
                log.Debug("Socket closed remotely");
                sk.cancel();
                readChannel.close();
                TransportStateEvent e =
                    new TransportStateEvent(TLSTM.this,
                                            incomingAddress,
                                            TransportStateEvent.
                                            STATE_DISCONNECTED_REMOTELY,
                                            null);
                fireConnectionStateChanged(e);
                return;
            }
            if (bytesRead == 0)
            {
                entry.inNetBuffer.clear();
                //entry.addRegistration(selector, SelectionKey.OP_READ);
            }
            else
            {
                SSLEngineResult result;
                synchronized(entry.inboundLock) {
                    result = entry.sslEngine.unwrap(inNetBuffer, inAppBuffer);
                    adjustInNetBuffer(entry, result);
                    switch (result.getStatus())
                    {
                        /*
                                      case BUFFER_UNDERFLOW:
                                        entry.addRegistration(selector, SelectionKey.OP_READ);
                                        return;
                        */
                        case BUFFER_OVERFLOW:
                            // TODO handle overflow
                            System.err.println("BUFFER_OVERFLOW");
                            throw new IOException("BUFFER_OVERFLOW");
                    }
                    if (runDelegatedTasks(result, entry))
                    {
                        log.Info("SSL session established");
                        if (result.bytesProduced() > 0)
                        {
                            entry.inAppBuffer.flip();
                            log.Debug("SSL established, dispatching inappBuffer=" + entry.inAppBuffer);
                            // SSL session is established
                            entry.checkTransportStateReference();
                            dispatchMessage(incomingAddress, inAppBuffer, inAppBuffer.limit(),
                                            entry.sessionID,
                                            entry.tmStateReference);
                            entry.getInAppBuffer().clear();
                        }
                        else if (entry.isAppOutPending())
                        {
                            writeMessage(entry, entry.getSocket().getChannel());
                        }
                    }
                }
            }
        }
        catch (ClosedChannelException ccex)
        {
            sk.cancel();
            if (log.IsDebugEnabled)
            {
                log.Debug("Read channel not open, no bytes read from " +
                             incomingAddress);
            }
            return;
        }
    }

    private ByteBuffer CreateBufferCopy(ByteBuffer buffer)
    {
        byte[] conInNetData = new byte[buffer.limit()];
        int buflen = buffer.limit() - buffer.remaining();
        buffer.flip();
        buffer.get(conInNetData, 0, buflen);
        ByteBuffer bufferCopy = ByteBuffer.wrap(conInNetData);
        bufferCopy.position(buflen);
        return bufferCopy;
    }

    private void DispatchMessage(TcpAddress incomingAddress,
                                 ByteBuffer byteBuffer, long bytesRead,
                                 object sessionID,
                                 TransportStateReference tmStateReference)
    {
        byteBuffer.flip();
        if (log.IsDebugEnabled)
        {
            log.Debug("Received message from " + incomingAddress +
                         " with length " + bytesRead + ": " +
                         new OctetString(byteBuffer.array(), 0,
                                         (int)bytesRead).toHexString());
        }
        ByteBuffer bis;
        if (isAsyncMsgProcessingSupported())
        {
            byte[] bytes = new byte[(int)bytesRead];
            System.arraycopy(byteBuffer.array(), 0, bytes, 0, (int)bytesRead);
            bis = ByteBuffer.wrap(bytes);
        }
        else
        {
            bis = ByteBuffer.wrap(byteBuffer.array(),
                                  0, (int)bytesRead);
        }
        fireProcessMessage(incomingAddress, bis, tmStateReference);
    }

    private void WriteMessage(SocketEntry entry, SocketChannel sc)
    {
        lock (entry.outboundLock)
        {
            if (entry.outAppBuffer == null)
            {
                byte[] message = entry.nextMessage();
                if (message != null)
                {
                    entry.outAppBuffer = ByteBuffer.wrap(message);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Sending message with length " +
                                     message.length + " to " +
                                     entry.getPeerAddress() + ": " +
                                     new OctetString(message).toHexString());
                    }
                }
                else
                {
                    entry.removeRegistration(selector, SelectionKey.OP_WRITE);
                    // Make sure that we did not clear a selection key that was concurrently
                    // added:
                    if (entry.hasMessage() &&
                        !entry.isRegistered(SelectionKey.OP_WRITE))
                    {
                        entry.addRegistration(selector, SelectionKey.OP_WRITE);
                        log.Debug("Waking up selector");
                        selector.wakeup();
                    }
                    entry.addRegistration(selector, SelectionKey.OP_READ);
                    return;
                }
            }
            SSLEngineResult result;
            result = entry.sslEngine.wrap(entry.outAppBuffer, entry.outNetBuffer);
            if (result.getStatus() == SSLEngineResult.Status.OK)
            {
                if (result.bytesProduced() > 0)
                {
                    writeNetBuffer(entry, sc);
                }
            }
            else if (runDelegatedTasks(result, entry))
            {
                log.Debug("SSL session OK");
                /*
                          if (entry.isAppOutPending()) {
                            writeMessage(entry, entry.getSocket().getChannel());
                          }
                          */
            }
            if (result.bytesConsumed() >= entry.outAppBuffer.limit())
            {
                log.Debug("Payload sent completely");
                entry.outAppBuffer = null;
            }
        }
        entry.addRegistration(selector, SelectionKey.OP_READ);
    }

    private void WriteNetBuffer(SocketEntry entry, SocketChannel sc)
    {
        entry.outNetBuffer.flip();
        // Send SSL/TLS encoded data to peer
        while (entry.outNetBuffer.hasRemaining())
        {
            log.Debug("Writing TLS outNetBuffer(PAYLOAD): " + entry.outNetBuffer);
            int num = sc.write(entry.outNetBuffer);
            log.Debug("Wrote TLS " + num + " bytes from outNetBuffer(PAYLOAD)");
            if (num == -1)
            {
                throw new IOException("TLS connection closed");
            }
            else if (num == 0)
            {
                entry.outNetBuffer.compact();
                //entry.outNetBuffer.limit(entry.outNetBuffer.capacity());
                return;
            }
        }
        entry.outNetBuffer.clear();
    }

    public void Close()
    {
        stop = true;
        WorkerTask st = server;
        if (st != null)
        {
            st.terminate();
        }
    }

    public void Terminate()
    {
        stop = true;
        if (log.IsDebugEnabled)
        {
            log.Debug("Terminated worker task: " + getClass().getName());
        }
    }

    public void Join()
    {
        if (log.IsDebugEnabled)
        {
            log.Debug("Joining worker task: " + getClass().getName());
        }
    }

    public void Interrupt()
    {
        stop = true;
        if (log.IsDebugEnabled)
        {
            log.Debug("Interrupting worker task: " + getClass().getName());
        }
        selector.wakeup();
    }
}

private bool matchingStateReferences(TransportStateReference tmStateReferenceNew,
                                        TransportStateReference tmStateReferenceExisting)
{
    if ((tmStateReferenceExisting == null) || (tmStateReferenceNew == null))
    {
        log.Error("Failed to compare TransportStateReferences refNew=" + tmStateReferenceNew +
                     ",refOld=" + tmStateReferenceExisting);
        return false;
    }
    if ((tmStateReferenceNew.getSecurityName() == null) ||
        (tmStateReferenceExisting.getSecurityName() == null))
    {
        log.Error("Could not match TransportStateReferences refNew=" + tmStateReferenceNew +
                     ",refOld=" + tmStateReferenceExisting);
        return false;
    }
    else if (!tmStateReferenceNew.getSecurityName().equals(tmStateReferenceExisting.getSecurityName()))
    {
        return false;
    }
    return true;
}

private SSLEngineResult sendNetMessage(SocketEntry entry)
{
    SSLEngineResult result;
    synchronized(entry.outboundLock) {
        if (!entry.outNetBuffer.hasRemaining())
        {
            return null;
        }
        result = entry.sslEngine.wrap(ByteBuffer.allocate(0), entry.outNetBuffer);
        entry.outNetBuffer.flip();
        log.Debug("TLS outNetBuffer = " + entry.outNetBuffer);
        entry.socket.getChannel().write(entry.outNetBuffer);
        entry.outNetBuffer.clear();
    }
    return result;
}

interface SSLEngineConfigurator
{
    /**
     * Configure the supplied SSLEngine for TLS.
     * Configuration includes enabled protocol(s),
     * cipher codes, etc.
     *
     * @param sslEngine
     *    a {@link SSLEngine} to configure.
     */
    void Configure(SSLEngine sslEngine);

    /**
     * Gets the SSLContext for this SSL connection.
     * @param useClientMode
     *    <code>true</code> if the connection is established in client mode.
     * @param transportStateReference
     *    the transportStateReference with additional
     *    security information for the SSL connection
     *    to establish.
     * @return
     *    the SSLContext.
     */
    SSLContext GetSSLContext(bool useClientMode, TransportStateReference transportStateReference);
}

protected class DefaultSSLEngineConfiguration : SSLEngineConfigurator
{

    private TrustManager[] trustManagers;

    public override void Configure(SSLEngine sslEngine)
    {
        log.Debug("Configuring SSL engine, supported protocols are " +
            Arrays.asList(sslEngine.getSupportedProtocols()) + ", supported ciphers are " +
            Arrays.asList(sslEngine.getSupportedCipherSuites()) + ", https defaults are " +
                          System.getProperty("https.cipherSuites"));
        string[] supportedCipherSuites = sslEngine.getEnabledCipherSuites();
        List<string> enabledCipherSuites = new ArrayList<string>(supportedCipherSuites.length);
        foreach (string cs in supportedCipherSuites)
        {
            if (!cs.contains("_anon_") && (!cs.contains("_NULL_")))
            {
                enabledCipherSuites.add(cs);
            }
        }
        //enabledCipherSuites.add("TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256");
        sslEngine.setEnabledCipherSuites(enabledCipherSuites.toArray(new string[enabledCipherSuites.size()]));
        sslEngine.setEnabledProtocols(getTlsProtocols());
        if (!sslEngine.getUseClientMode())
        {
            sslEngine.setNeedClientAuth(true);
            sslEngine.setWantClientAuth(true);
            log.Info("Need client authentication set to true");
        }
        log.Info("Configured SSL engine, enabled protocols are " +
                    Arrays.asList(sslEngine.getEnabledProtocols()) + ", enabled ciphers are " +
                    Arrays.asList(sslEngine.getEnabledCipherSuites()));
    }

    public override SSLContext GetSSLContext(bool useClientMode, TransportStateReference transportStateReference)
    {
        try
        {
            string protocol = DEFAULT_TLSTM_PROTOCOLS;
            if ((getTlsProtocols() != null) && (getTlsProtocols().length > 0))
            {
                protocol = getTlsProtocols()[0];

            }
            SSLContext sslContext = SSLContext.getInstance(protocol);
            TrustManagerFactory tmf =
                TrustManagerFactory.getInstance("SunPKIX");
            // use default keystore
            try
            {
                KeyStore ks = KeyStore.getInstance("JKS");
                FileInputStream fis =
                    new FileInputStream(getKeyStore());
                ks.load(fis, (getKeyStorePassword() != null) ? getKeyStorePassword().toCharArray() : null);
                if (log.IsInfoEnabled)
                {
                    log.Info("KeyStore '" + fis + "' contains: " + Collections.list(ks.aliases()));
                }

                filterCertificates(ks, transportStateReference);

                // Set up key manager factory to use our key store
                KeyManagerFactory kmf = KeyManagerFactory.getInstance("SunX509");
                kmf.init(ks, (getKeyStorePassword() != null) ? getKeyStorePassword().toCharArray() : null);

                tmf.init(ks);
                trustManagers = tmf.getTrustManagers();
                if (log.IsDebugEnabled)
                {
                    log.Debug("SSL context initializing with TrustManagers: " + Arrays.asList(trustManagers) +
                                 " and factory " + trustManagerFactory.getClass().getName());
                }
                sslContext.init(kmf.getKeyManagers(),
                    new TrustManager[]{ trustManagerFactory.create((X509TrustManager) trustManagers[0],
                                    useClientMode, transportStateReference)},
                    null);
                return sslContext;
            }
            catch (KeyStoreException e)
            {
                log.Error("Failed to initialize SSLContext because of a KeyStoreException: " + e.getMessage(), e);
            }
            catch (KeyManagementException e)
            {
                log.Error("Failed to initialize SSLContext because of a KeyManagementException: " + e.getMessage(), e);
            }
            catch (UnrecoverableKeyException e)
            {
                log.Error("Failed to initialize SSLContext because of an UnrecoverableKeyException: " + e.getMessage(), e);
            }
            catch (CertificateException e)
            {
                log.Error("Failed to initialize SSLContext because of a CertificateException: " + e.getMessage(), e);
            }
            catch (FileNotFoundException e)
            {
                log.Error("Failed to initialize SSLContext because of a FileNotFoundException: " + e.getMessage(), e);
            }
            catch (IOException e)
            {
                log.Error("Failed to initialize SSLContext because of an IOException: " + e.getMessage(), e);
            }
        }
        catch (NoSuchAlgorithmException e)
        {
            log.Error("Failed to initialize SSLContext because of an NoSuchAlgorithmException: " + e.getMessage(), e);
        }
        return null;
    }

    private void filterCertificates(KeyStore ks, TransportStateReference transportStateReference)
    {
        string localCertAlias = localCertificateAlias;
        if ((securityCallback != null) && (transportStateReference != null))
        {
            localCertAlias = securityCallback.getLocalCertificateAlias(transportStateReference.getAddress());
            if (localCertAlias == null)
            {
                localCertAlias = localCertificateAlias;
            }
        }
        if (localCertAlias != null)
        {
            try
            {
                java.security.cert.Certificate[] chain = ks.getCertificateChain(localCertAlias);
                if (chain == null)
                {
                    log.Warn("Local certificate with alias '" + localCertAlias + "' not found. Known aliases are: " +
                        Collections.list(ks.aliases()));
                }
                else
                {
                    List<string> chainAliases = new ArrayList<string>(chain.length);
                    for (java.security.cert.Certificate certificate : chain)
                    {
                        string alias = ks.getCertificateAlias(certificate);
                        if (alias != null)
                        {
                            chainAliases.add(alias);
                        }
                    }
                    // now delete all others from key store
                    for (string alias : Collections.list(ks.aliases()))
                    {
                        if (chainAliases.contains(alias))
                        {
                            ks.deleteEntry(alias);
                        }
                    }
                }
            }
            catch (KeyStoreException e)
            {
                log.Error("Failed to get certificate chain for alias " +
                    localCertAlias + ": " + e.getMessage(), e);
            }
        }
    }
}

protected class TlsTrustManager : X509TrustManager
{

    X509TrustManager trustManager;
    private bool useClientMode;
    private TransportStateReference tmStateReference;

    protected TlsTrustManager(X509TrustManager trustManager, bool useClientMode,
                              TransportStateReference tmStateReference)
    {
        this.trustManager = trustManager;
        this.useClientMode = useClientMode;
        this.tmStateReference = tmStateReference;
    }


    public override void CheckClientTrusted(X509Certificate[] x509Certificates, string s)

    {
        if ((tmStateReference != null) && (tmStateReference.getCertifiedIdentity() != null))
        {
            OctetString fingerprint = tmStateReference.getCertifiedIdentity().getClientFingerprint();
            if (isMatchingFingerprint(x509Certificates, fingerprint))
            {
                return;
            }
        }
        TlsTmSecurityCallback<X509Certificate> callback = securityCallback;
        if (!useClientMode && (callback != null))
        {
            if (callback.isClientCertificateAccepted(x509Certificates[0]))
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Client is trusted with certificate '" + x509Certificates[0] + "'");
                }
                return;
            }
        }
        try
        {
            trustManager.checkClientTrusted(x509Certificates, s);
        }
        catch (CertificateException cex)
        {
            counterSupport.fireIncrementCounter(new CounterEvent(this, SnmpConstants.snmpTlstmSessionOpenErrors));
            counterSupport.fireIncrementCounter(new CounterEvent(this, SnmpConstants.snmpTlstmSessionInvalidClientCertificates));
            log.Warn("Client certificate validation failed for '" + x509Certificates[0] + "'");
            throw cex;
        }
    }

    public override void checkServerTrusted(X509Certificate[] x509Certificates, string s)
    {
        if (tmStateReference.getCertifiedIdentity() != null)
        {
            OctetString fingerprint = tmStateReference.getCertifiedIdentity().getServerFingerprint();
            if (isMatchingFingerprint(x509Certificates, fingerprint)) return;
        }
        object entry = null;
        try
        {
            entry = TLSTM.getSubjAltName(x509Certificates[0].getSubjectAlternativeNames(), 2);
        }
        catch (CertificateParsingException e)
        {
            log.Error("CertificateParsingException while verifying server certificate " +
                Arrays.asList(x509Certificates));
        }
        if (entry == null)
        {
            X500Principal x500Principal = x509Certificates[0].getSubjectX500Principal();
            if (x500Principal != null)
            {
                entry = x500Principal.getName();
            }
        }
        if (entry != null)
        {
            string dNSName = ((string)entry).toLowerCase();
            string hostName = ((IpAddress)tmStateReference.getAddress())
                .getInetAddress().getCanonicalHostName();
            if (dNSName.Length > 0)
            {
                if (dNSName[0] == '*')
                {
                    int pos = hostName.IndexOf('.');
                    hostName = hostName.Substring(pos);
                    dNSName = dNSName.Substring(1);
                }
                if (hostName.Equals(dNSName, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Peer hostname " + hostName + " matches dNSName " + dNSName);
                    }
                    return;
                }
            }
            if (log.IsDebugEnabled)
            {
                log.Debug("Peer hostname " + hostName + " did not match dNSName " + dNSName);
            }
        }
        try
        {
            trustManager.checkServerTrusted(x509Certificates, s);
        }
        catch (CertificateException cex)
        {
            counterSupport.fireIncrementCounter(new CounterEvent(this, SnmpConstants.snmpTlstmSessionOpenErrors));
            counterSupport.fireIncrementCounter(new CounterEvent(this, SnmpConstants.snmpTlstmSessionUnknownServerCertificate));
            log.Warn("Server certificate validation failed for '" + x509Certificates[0] + "'");
            throw cex;
        }
        TlsTmSecurityCallback<X509Certificate> callback = securityCallback;
        if (useClientMode && (callback != null))
        {
            if (!callback.isServerCertificateAccepted(x509Certificates))
            {
                log.Info("Server is NOT trusted with certificate '" + Arrays.asList(x509Certificates) + "'");
                throw new CertificateException("Server's certificate is not trusted by this application (although it was trusted by the JRE): " +
                Arrays.asList(x509Certificates));
            }
        }
    }

    private bool isMatchingFingerprint(X509Certificate[] x509Certificates, OctetString fingerprint)
    {
        if ((fingerprint != null) && (fingerprint.length() > 0))
        {
            for (X509Certificate cert : x509Certificates)
            {
                OctetString certFingerprint = null;
                certFingerprint = getFingerprint(cert);
                if (log.IsDebugEnabled)
                {
                    log.Debug("Comparing certificate fingerprint " + certFingerprint +
                        " with " + fingerprint);
                }
                if (certFingerprint == null)
                {
                    log.Error("Failed to determine fingerprint for certificate " + cert +
                        " and algorithm " + cert.getSigAlgName());
                }
                else if (certFingerprint.equals(fingerprint))
                {
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Peer is trusted by fingerprint '" + fingerprint + "' of certificate: '" + cert + "'");
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public override X509Certificate[] getAcceptedIssuers()
    {
        TlsTmSecurityCallback<X509Certificate> callback = securityCallback;
        X509Certificate[] accepted = trustManager.getAcceptedIssuers();
        if ((accepted != null) && (callback != null))
        {
            ArrayList<X509Certificate> acceptedIssuers = new ArrayList<X509Certificate>(accepted.length);
            for (X509Certificate cert : accepted)
            {
                if (callback.isAcceptedIssuer(cert))
                {
                    acceptedIssuers.add(cert);
                }
            }
            return acceptedIssuers.toArray(new X509Certificate[acceptedIssuers.size()]);
        }
        return accepted;
    }
}

private void adjustInNetBuffer(SocketEntry entry, SSLEngineResult result)
{
    if (result.bytesConsumed() == entry.inNetBuffer.limit())
    {
        entry.inNetBuffer.clear();
    }
    else if (result.bytesConsumed() > 0)
    {
        entry.inNetBuffer.compact();
    }
}

public interface TLSTMTrustManagerFactory
{
    X509TrustManager create(X509TrustManager trustManager, bool useClientMode,
                            TransportStateReference tmStateReference);
}

private class DefaultTLSTMTrustManagerFactory : TLSTMTrustManagerFactory
{
    public X509TrustManager create(X509TrustManager trustManager, bool useClientMode,
                                 TransportStateReference tmStateReference)
    {
        return new TlsTrustManager(trustManager, useClientMode, tmStateReference);
    }
}
}

}
