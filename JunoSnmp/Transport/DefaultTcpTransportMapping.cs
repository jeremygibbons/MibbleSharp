// <copyright file="DefaultTcpTransportMapping.cs" company="None">
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
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;
    /// <summary>
    /// The <code>DefaultTcpTransportMapping</code> implements a TCP transport
    /// mapping with the Java 1.4 new IO API. It uses a single thread for processing incoming and outgoing messages.
    /// The thread is started when the<code> listen</code> method is called, or
    /// when an outgoing request is sent using the <code>sendMessage</code> method.
    /// </summary>
    public class DefaultTcpTransportMapping : TcpTransportMapping
    {

        /// <summary>
        /// The maximum number of loops trying to read data from an incoming port but no data has been received.
        /// A value of 0 or less disables the check.
        /// </summary>
        public static readonly int DEFAULT_MAX_BUSY_LOOPS = 100;

        private static readonly log4net.ILog log = log4net.LogManager
                    .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private Dictionary<IAddress, SocketEntry> sockets = new Dictionary<IAddress, SocketEntry>();
        private WorkerTask server;
        private ServerThread serverThread;

        private CommonTimer socketCleaner;
        // 1 minute default timeout
        private long connectionTimeout = 60000;
        private bool serverEnabled = false;

        private static readonly int MIN_SNMP_HEADER_LENGTH = 6;
        private MessageLengthDecoder messageLengthDecoder =
            new SnmpMesssageLengthDecoder();
        private int maxBusyLoops = DEFAULT_MAX_BUSY_LOOPS;

        /**
         * Creates a default TCP transport mapping with the server for incoming
         * messages disabled.
         * @throws IOException
         *    on failure of binding a local port.
         */
        public DefaultTcpTransportMapping() : base(new TcpAddress(InetAddress.getLocalHost(), 0))
        {
        }

        /**
         * Creates a default TCP transport mapping that binds to the given address
         * (interface) on the local host.
         *
         * @param serverAddress
         *    the TcpAddress instance that describes the server address to listen
         *    on incoming connection requests.
         * @throws IOException
         *    if the given address cannot be bound.
         */
        public DefaultTcpTransportMapping(TcpAddress serverAddress) : base(serverAddress)
        {
            this.serverEnabled = true;
        }

        /**
         * Listen for incoming and outgoing requests. If the <code>serverEnabled</code>
         * member is <code>false</code> the server for incoming requests is not
         * started. This starts the internal server thread that processes messages.
         * @throws SocketException
         *    when the transport is already listening for incoming/outgoing messages.
         * @throws IOException
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Listen()
        {
            if (server != null)
            {
                throw new SocketException("Port already listening");
            }
            serverThread = new ServerThread();
            server = JunoSnmpSettings.getThreadFactory().createWorkerThread(
              "DefaultTCPTransportMapping_" + getAddress(), serverThread, true);
            if (connectionTimeout > 0)
            {
                // run as daemon
                socketCleaner = JunoSnmpSettings.getTimerFactory().createTimer();
            }
            server.run();
        }

        /**
         * Returns the priority of the internal listen thread.
         * @return
         *    a value between {@link Thread#MIN_PRIORITY} and
         *    {@link Thread#MAX_PRIORITY}.
         * @since 1.2.2
         */

        /**
         * Changes the priority of the server thread for this TCP transport mapping.
         * This method has no effect, if called before {@link #listen()} has been
         * called for this transport mapping or if SNMP4J is configured to use
         * a non-default thread factory.
         *
         * @param newPriority
         *    the new priority.
         * @see Thread#setPriority
         * @since 1.2.2
         */
        public int Priority
        {
            get
            {
                WorkerTask st = server;
                if (st is Thread)
                {
                    return ((Thread)st).getPriority();
                }
                else
                {
                    return Thread.NORM_PRIORITY;
                }
            }

            set
            {
                WorkerTask st = server;
                if (st is Thread)
                {
                    ((Thread)st).setPriority(value);
                }
            }
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
                    Socket s = entry.Socket;
                    if (s != null)
                    {
                        try
                        {
                            SocketChannel sc = s.getChannel();
                            s.close();
                            if (log.IsDebugEnabled)
                            {
                                log.Debug("Socket to " + entry.PeerAddress+ " closed");
                            }
                            if (sc != null)
                            {
                                sc.close();
                                if (log.IsDebugEnabled)
                                {
                                    log.Debug("Socket channel to " +
                                        entry.PeerAddress+ " closed");
                                }
                            }
                        }
                        catch (IOException iox)
                        {
                            // ingore
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
         * @throws IOException
         *    if the remote address cannot be closed due to an IO exception.
         * @since 1.7.1
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Close(TcpAddress remoteAddress)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Closing socket for peer address " + remoteAddress);
            }
            SocketEntry entry = sockets.remove(remoteAddress);
            if (entry != null)
            {
                Socket s = entry.Socket;
                if (s != null)
                {
                    SocketChannel sc = entry.Socket.getChannel();
                    entry.Socket.close();
                    if (log.IsInfoEnabled)
                    {
                        log.Info("Socket to " + entry.PeerAddress+ " closed");
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
         * @throws IOException
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


        /**
         * Sets the message length decoder. Default message length decoder is the
         * {@link SnmpMesssageLengthDecoder}. The message length decoder must be
         * able to decode the total length of a message for this transport mapping
         * protocol(s).
         * @param messageLengthDecoder
         *    a <code>MessageLengthDecoder</code> instance.
         */
        public override MessageLengthDecoder MessageLengthDecoder
        {
            get
            {
                return messageLengthDecoder;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.messageLengthDecoder = value;
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
        private void TimeoutSocket(SocketEntry entry)
        {
            if (connectionTimeout > 0)
            {
                socketCleaner.schedule(new SocketTimeout(entry), connectionTimeout);
            }
        }

        public bool Listening
        {
            get
            {
                return (server != null);
            }
        }


        protected int MaxBusyLoops
        {
            get
            {
                return maxBusyLoops;
            }

            set
            {
                this.maxBusyLoops = value;
            }
        }

        /**
         * Sets optional server socket options. The default implementation does
         * nothing.
         * @param serverSocket
         *    the <code>ServerSocket</code> to apply additional non-default options.
         */
        protected void SetSocketOptions(ServerSocket serverSocket)
        {
        }

        class SocketEntry
        {
            private Socket socket;
            private TcpAddress peerAddress;
            private long lastUse;
            private LinkedList<byte[]> message = new LinkedList<byte[]>();
            private ByteBuffer readBuffer = null;
            private volatile int registrations = 0;
            private volatile int busyLoops = 0;

            public SocketEntry(TcpAddress address, Socket socket)
            {
                this.peerAddress = address;
                this.socket = socket;
                this.lastUse = System.DateTime.Now.Ticks;
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

            public Socket Socket
            {
                get
                {
                    return socket;
                }
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

            public byte[] NextMessage
            {
                [MethodImpl(MethodImplOptions.Synchronized)]
                get
                {
                    if (this.message.Count > 0)
                    {
                        byte[] head = this.message.First.Value;
                        this.message.RemoveFirst();
                        return head;
                    }

                    return null;
                }
            }

            public bool HasMessage
            {
                [MethodImpl(MethodImplOptions.Synchronized)]
                get
                {
                    return !this.message.isEmpty();
                }
            }

            public ByteBuffer ReadBuffer
            {
                get
                {
                    return readBuffer;
                }

                set
                {
                    this.readBuffer = value;
                }
            }

            public int NextBusyLoop
            {
                get
                {
                    return ++busyLoops;
                }
            }

            public void ResetBusyLoops()
            {
                busyLoops = 0;
            }

            public override string ToString()
            {
                return "SocketEntry[peerAddress=" + peerAddress +
                    ",socket=" + socket + ",lastUse=" + new Date(lastUse / SnmpConstants.MILLISECOND_TO_NANOSECOND) +
                    ",readBufferPosition=" + ((readBuffer == null) ? -1 : readBuffer.position()) +
                    "]";
            }
        }

        public class SnmpMesssageLengthDecoder : MessageLengthDecoder
        {
            public int MinHeaderLength
            {
                get
                {
                    return MIN_SNMP_HEADER_LENGTH;
                }
            }

            public MessageLength GetMessageLength(MemoryStream buf)
            {
                BER.MutableByte type = new BER.MutableByte();
                BERInputStream ins = new BERInputStream(buf.GetBuffer());
                int ml = BER.DecodeHeader(ins, out type, false);
                int hl = (int)ins.Position;
                MessageLength messageLength = new MessageLength(hl, ml);
                return messageLength;
            }
        }

        class SocketTimeout : TimerTask
        {
            private SocketEntry entry;
            private DefaultTcpTransportMapping ttm;

            public SocketTimeout(DefaultTcpTransportMapping ttm, SocketEntry entry)
            {
                this.entry = entry;
                this.ttm = ttm;
            }

            public void Run()
            {
                long now = System.DateTime.Now.Ticks;
                SocketEntry entryCopy = entry;
                if (entryCopy == null)
                {
                    // nothing to do
                    return;
                }
                long idleMillis = (now - entryCopy.LastUse) / SnmpConstants.MILLISECOND_TO_NANOSECOND;
                if ((socketCleaner == null) ||
                    (idleMillis >= ttm.ConnectionTimeout))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Socket has not been used for " + idleMillis + " milliseconds, closing it");
                    }
                    try
                    {
                        lock(entryCopy) {
                            if (idleMillis >= ttm.ConnectionTimeout)
                            {
                                ttm.sockets.remove(entryCopy.PeerAddress);
                                entryCopy.Socket.close();
                                if (log.IsInfoEnabled)
                                {
                                    log.Info("Socket to " + entryCopy.PeerAddress+ " closed due to timeout");
                                }
                            }
                            else
                            {
                                RescheduleCleanup(idleMillis, entryCopy);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                    }
                }
                else
                {
                    RescheduleCleanup(idleMillis, entryCopy);
                }
            }

            private void RescheduleCleanup(long idleMillisAlreadyElapsed, SocketEntry entry)
            {
                long nextRun = ttm.ConnectionTimeout - idleMillisAlreadyElapsed;

                if (log.IsDebugEnabled)
                {
                    log.Debug("Scheduling " + nextRun);
                }

                ttm.socketCleaner.schedule(new SocketTimeout(entry), nextRun);
            }

            public bool Cancel()
            {
                bool result = base.Cancel();
                // free objects early
                entry = null;
                return result;
            }
        }

        class ServerThread : WorkerTask
        {
            private byte[] buf;
            private volatile bool stop = false;
            private Throwable lastError = null;
            private ServerSocketChannel ssc;
            private Selector selector;
            private DefaultTcpTransportMapping ttm;

            private LinkedList<SocketEntry> pending = new LinkedList<SocketEntry>();

            public ServerThread(DefaultTcpTransportMapping ttm)
            {
                this.ttm = ttm;
                buf = new byte[ttm.MaxInboundMessageSize];
                // Selector for incoming requests
                selector = Selector.open();

                if (ttm.ServerEnabled)
                {
                    // Create a new server socket and set to non blocking mode
                    ssc = ServerSocketChannel.open();
                    try
                    {
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
                    catch (IOException iox)
                    {
                        log.Warn("Socket bind failed for " + ttm.tcpAddress + ": " + iox.Message);
                        try
                        {
                            ssc.close();
                        }
                        catch (IOException ioxClose)
                        {
                            log.Warn("Socket close failed after bind failure for " + ttm.tcpAddress + ": " + ioxClose.Message);
                        }
                        throw iox;
                    }
                }
            }

            private void ProcessPending()
            {
                lock (pending)
                {
                    for (int i = 0; i < pending.size(); i++)
                    {
                        SocketEntry entry = pending.get(i);
                        try
                        {
                            // Register the channel with the selector, indicating
                            // interest in connection completion and attaching the
                            // target object so that we can get the target back
                            // after the key is added to the selector's
                            // selected-key set
                            if (entry.Socket.isConnected())
                            {
                                entry.AddRegistration(selector, SelectionKey.OP_WRITE);
                            }
                            else
                            {
                                entry.AddRegistration(selector, SelectionKey.OP_CONNECT);
                            }
                        }
                        catch (CancelledKeyException ckex)
                        {
                            log.Warn(ckex);
                            pending.remove(entry);
                            try
                            {
                                entry.Socket.getChannel().close();
                                TransportStateEventArgs e =
                                    new TransportStateEventArgs(ttm,
                                                            entry.PeerAddress,
                                                            TransportStateEventArgs.STATE_CLOSED,
                                                            null,
                                                            entry.message);
                                FireConnectionStateChanged(e);
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
                                entry.Socket.getChannel().close();
                                TransportStateEventArgs e =
                                    new TransportStateEventArgs(ttm,
                                                            entry.PeerAddress,
                                                            TransportStateEventArgs.STATE_CLOSED,
                                                            iox, entry.message);
                                FireConnectionStateChanged(e);
                            }
                            catch (IOException ex)
                            {
                                log.Error(ex);
                            }
                            lastError = iox;
                            if (JunoSnmpSettings.ForwardRuntimeExceptions)
                            {
                                throw new RuntimeException(iox);
                            }
                        }
                    }
                }
            }

            public Throwable LastError
            {
                get
                {
                    return lastError;
                }
            }

            public void SendMessage(IAddress address, byte[] message,
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
                    lock(entry) {
                        entry.used();
                        s = entry.Socket;
                    }
                }
                if ((s == null) || (s.isClosed()) || (!s.isConnected()))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Socket for address '" + address +
                                     "' is closed, opening it...");
                    }

                    lock(pending) {
                        pending.remove(entry);
                    }

                    SocketChannel sc = null;
                    try
                    {
                        IPEndPoint targetAddress =
                            new IPEndPoint(((TcpAddress)address).GetSystemNetIpAddress(),
                                                  ((TcpAddress)address).Port);
                        if ((s == null) || (s.isClosed()))
                        {
                            // Open the channel, set it to non-blocking, initiate connect
                            sc = SocketChannel.open();
                            sc.configureBlocking(false);
                            sc.connect(targetAddress);
                        }
                        else
                        {
                            sc = s.getChannel();
                            sc.configureBlocking(false);
                            if (!sc.isConnectionPending())
                            {
                                sc.connect(targetAddress);
                            }
                        }
                        s = sc.socket();
                        entry = new SocketEntry((TcpAddress)address, s);
                        entry.addMessage(message);
                        sockets.put(address, entry);

                        lock(pending) {
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
                }
                else
                {
                    entry.AddMessage(message);
                    lock(pending) {
                        pending.AddLast(entry);
                    }
                    log.Debug("Waking up selector for new message");
                    selector.wakeup();
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
                                            SocketEntry entry = new SocketEntry(incomingAddress, s);
                                            entry.addRegistration(selector, SelectionKey.OP_READ);
                                            sockets.put(incomingAddress, entry);
                                            timeoutSocket(entry);
                                            TransportStateEventArgs e =
                                                new TransportStateEventArgs(ttm,
                                                                        incomingAddress,
                                                                        TransportStateEventArgs.
                                                                        STATE_CONNECTED,
                                                                        null);
                                            FireConnectionStateChanged(e);
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
                                                if (!readMessage(sk, readChannel, incomingAddress))
                                                {
                                                    SocketEntry entry = (SocketEntry)sk.attachment();
                                                    if ((entry != null) && (getMaxBusyLoops() > 0))
                                                    {
                                                        int busyLoops = entry.nextBusyLoop;
                                                        if (busyLoops > getMaxBusyLoops())
                                                        {
                                                            if (log.IsDebugEnabled)
                                                            {
                                                                log.Debug("After " + busyLoops + " read key has been removed: " + entry);
                                                            }
                                                            entry.removeRegistration(selector, SelectionKey.OP_READ);
                                                            entry.resetBusyLoops();
                                                        }
                                                    }
                                                }
                                            }
                                            catch (IOException iox)
                                            {
                                                // IO exception -> channel closed remotely
                                                socketClosedRemotely(sk, readChannel, incomingAddress);
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
                    synchronized(DefaultTcpTransportMapping.this) {
                        server = null;
                    }
                }

                if (log.IsDebugEnabled)
                {
                    log.Debug("Worker task finished: " + this.GetType().Name);
                }
            }

            private void ConnectChannel(SelectionKey sk, TcpAddress incomingAddress)
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
                            log.Debug("Connected to " + entry.PeerAddress);
                            // make sure conncetion is closed if not used for timeout
                            // micro seconds
                            timeoutSocket(entry);
                            entry.RemoveRegistration(selector, SelectionKey.OP_CONNECT);
                            entry.AddRegistration(selector, SelectionKey.OP_WRITE);
                        }
                        else
                        {
                            entry = null;
                        }
                    }
                    if (entry != null)
                    {
                        Address addr = (incomingAddress == null) ?
                                                    entry.PeerAddress: incomingAddress;
                        log.Debug("Fire connected event for " + addr);
                        TransportStateEvent e =
                            new TransportStateEvent(DefaultTcpTransportMapping.this,
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
                    if ((entry != null) && (!entry.hasMessage))
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
                        new TransportStateEvent(DefaultTcpTransportMapping.this,
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

            private bool ReadMessage(SelectionKey sk, SocketChannel readChannel,
                                        TcpAddress incomingAddress)
            {
                SocketEntry entry = (SocketEntry)sk.attachment();
                if (entry == null)
                {
                    // slow but in some cases needed:
                    entry = sockets.get(incomingAddress);
                }
                if (entry != null)
                {
                    // note that socket has been used
                    entry.used();
                    ByteBuffer readBuffer = entry.ReadBuffer;
                    if (readBuffer != null)
                    {
                        int bytesRead = readChannel.read(readBuffer);
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Read " + bytesRead + " bytes from " + incomingAddress);
                        }
                        if ((bytesRead >= 0) &&
                            (readBuffer.hasRemaining() || (readBuffer.position() < messageLengthDecoder.getMinHeaderLength())))
                        {
                            entry.addRegistration(selector, SelectionKey.OP_READ);
                        }
                        else if (bytesRead < 0)
                        {
                            socketClosedRemotely(sk, readChannel, incomingAddress);
                        }
                        else
                        {
                            readSnmpMessagePayload(readChannel, incomingAddress, entry, readBuffer);
                        }
                        if (bytesRead != 0)
                        {
                            entry.resetBusyLoops();
                            return true;
                        }
                        return false;
                    }
                }
                ByteBuffer byteBuffer = ByteBuffer.wrap(buf);
                byteBuffer.limit(messageLengthDecoder.getMinHeaderLength());
                if (!readChannel.isOpen())
                {
                    sk.cancel();
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Read channel not open, no bytes read from " +
                                     incomingAddress);
                    }
                    return false;
                }
                long bytesRead;
                try
                {
                    bytesRead = readChannel.read(byteBuffer);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Reading header " + bytesRead + " bytes from " +
                                     incomingAddress);
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
                    return false;
                }
                if (byteBuffer.position() >= messageLengthDecoder.getMinHeaderLength())
                {
                    readSnmpMessagePayload(readChannel, incomingAddress, entry, byteBuffer);
                }
                else if (bytesRead < 0)
                {
                    socketClosedRemotely(sk, readChannel, incomingAddress);
                }
                else if ((entry != null) && (bytesRead > 0))
                {
                    addBufferToReadBuffer(entry, byteBuffer);
                    entry.addRegistration(selector, SelectionKey.OP_READ);
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("No socket entry found for incoming address " + incomingAddress +
                                     " for incomplete message with length " + bytesRead);
                    }
                }
                if ((entry != null) && (bytesRead != 0))
                {
                    entry.resetBusyLoops();
                    return true;
                }
                return false;
            }

            private void ReadSnmpMessagePayload(SocketChannel readChannel, TcpAddress incomingAddress,
                                                SocketEntry entry, ByteBuffer byteBuffer)
            {
                MessageLength messageLength =
                      messageLengthDecoder.getMessageLength(ByteBuffer.wrap(byteBuffer.array()));
                if (log.IsDebugEnabled)
                {
                    log.Debug("Message length is " + messageLength);
                }
                if ((messageLength.getMessageLength() > getMaxInboundMessageSize()) ||
                    (messageLength.getMessageLength() <= 0))
                {
                    log.Error("Received message length " + messageLength +
                                 " is greater than inboundBufferSize " +
                                 getMaxInboundMessageSize());
                    if (entry != null)
                    {
                        Socket s = entry.Socket;
                        if (s != null)
                        {
                            s.close();
                            log.Info("Socket to " + entry.PeerAddress+
                                        " closed due to an error");
                        }
                    }
                }
                else
                {
                    int messageSize = messageLength.getMessageLength();
                    if (byteBuffer.position() < messageSize)
                    {
                        if (byteBuffer.capacity() < messageSize)
                        {
                            if (log.IsDebugEnabled)
                            {
                                log.Debug("Extending message buffer size according to message length to " + messageSize);
                            }
                            // Enhance capacity to expected message size and replace existing (too short) read buffer
                            byte[] newBuffer = new byte[messageSize];
                            int len = byteBuffer.position();
                            byteBuffer.flip();
                            byteBuffer.get(newBuffer, 0, len);
                            byteBuffer = ByteBuffer.wrap(newBuffer);
                            byteBuffer.position(len);
                            if (entry != null)
                            {
                                byteBuffer.limit(messageSize);
                                entry.ReadBuffer = byteBuffer;
                            }
                        }
                        else
                        {
                            byteBuffer.limit(messageSize);
                        }
                        readChannel.read(byteBuffer);
                    }
                    long bytesRead = byteBuffer.position();
                    if (bytesRead >= messageSize)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Message completed with " + bytesRead + " bytes and " + byteBuffer.limit() + " buffer limit");
                        }
                        if (entry != null)
                        {
                            entry.ReadBuffer = null;
                        }
                        dispatchMessage(incomingAddress, byteBuffer, bytesRead, entry);
                    }
                    else if ((entry != null) && (byteBuffer != entry.ReadBuffer))
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Adding buffer content to read buffer of entry " + entry + ", buffer " + byteBuffer);
                        }
                        addBufferToReadBuffer(entry, byteBuffer);
                    }
                    if (entry != null)
                    {
                        entry.addRegistration(selector, SelectionKey.OP_READ);
                    }
                }
            }

            private void DispatchMessage(TcpAddress incomingAddress,
                                         ByteBuffer byteBuffer, long bytesRead,
                                         Object sessionID)
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
                TransportStateReference stateReference =
                  new TransportStateReference(DefaultTcpTransportMapping.this, incomingAddress, null,
                                              SecurityLevel.undefined, SecurityLevel.undefined,
                                              false, sessionID);
                fireProcessMessage(incomingAddress, bis, stateReference);
            }

            private void WriteMessage(SocketEntry entry, SocketChannel sc)
            {
                byte[]
              message = entry.nextMessage;
                if (message != null)
                {
                    ByteBuffer buffer = ByteBuffer.wrap(message);
                    sc.write(buffer);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Sent message with length " +
                                     message.length + " to " +
                                     entry.PeerAddress+ ": " +
                                     new OctetString(message).toHexString());
                    }
                    entry.AddRegistration(selector, SelectionKey.OP_READ);
                }
                else
                {
                    entry.RemoveRegistration(selector, SelectionKey.OP_WRITE);
                    // Make sure that we did not clear a selection key that was concurrently
                    // added:
                    if (entry.HasMessage && !entry.IsRegistered(SelectionKey.OP_WRITE))
                    {
                        entry.AddRegistration(selector, SelectionKey.OP_WRITE);
                        log.Debug("Waking up selector");
                        selector.wakeup();
                    }
                }
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
                    log.Debug("Terminated worker task: " + this.GetType().Name);
                }
            }

            public void Join()
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Joining worker task: " + this.GetType().Name);
                }
            }

            public void Interrupt()
            {
                stop = true;
                if (log.IsDebugEnabled)
                {
                    log.Debug("Interrupting worker task: " + this.GetType().Name);
                }
                selector.wakeup();
            }
        }

        private void AddBufferToReadBuffer(SocketEntry entry, ByteBuffer byteBuffer)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Adding data " + byteBuffer + " to read buffer " + entry.ReadBuffer);
            }
            int buflen = byteBuffer.position();
            if (entry.ReadBuffer!= null)
            {
                entry.ReadBuffer.put(byteBuffer.array(), 0, buflen);
            }
            else
            {
                byte[] message = new byte[byteBuffer.limit()];
                byteBuffer.flip();
                byteBuffer.get(message, 0, buflen);
                ByteBuffer newBuffer = ByteBuffer.wrap(message);
                newBuffer.position(buflen);
                entry.ReadBuffer = newBuffer;
            }
        }

        private void SocketClosedRemotely(SelectionKey sk, SocketChannel readChannel, TcpAddress incomingAddress)
        {
            log.Debug("Socket closed remotely");
            sk.cancel();
            readChannel.close();
            TransportStateEventArgs e =
                new TransportStateEventArgs(ttm,
                                        incomingAddress,
                                        TransportStateEventArgs.
                                        STATE_DISCONNECTED_REMOTELY,
                                        null);
            FireConnectionStateChanged(e);
            sockets.Remove(incomingAddress);
        }

    }

}
