// <copyright file="DefaultUdpTransportMapping.cs" company="None">
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
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>DefaultUdpTransportMapping</c> implements a UDP transport
    /// mapping based on C# standard IO and using an internal thread for
    /// listening on the inbound socket.
    /// </summary>
    public class DefaultUdpTransportMapping : UdpTransportMapping
    {

        private static readonly log4net.ILog log = log4net.LogManager
                    .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected UdpClient socket = null;

        private Task listener;

        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private int socketTimeout = 0;

        private int stopListening = 0;

        private int receiveBufferSize = 0; // not set by default

        /**
         * Creates a UDP transport with an arbitrary local port on all local
         * interfaces.
         *
         * @throws IOException
         *    if socket binding fails.
         */
        public DefaultUdpTransportMapping() : base(new UdpAddress("0.0.0.0/0"))
        {

            socket = new UdpClient(udpAddress.Port);
        }

        /**
         * Creates a UDP transport with optional reusing the address if is currently
         * in timeout state (TIME_WAIT) after the connection is closed.
         *
         * @param udpAddress
         *    the local address for sending and receiving of UDP messages.
         * @param reuseAddress
         *    if <code>true</code> addresses are reused which provides faster socket
         *    binding if an application is restarted for instance.
         * @throws IOException
         *    if socket binding fails.
         * @since 1.7.3
         */
        public DefaultUdpTransportMapping(UdpAddress udpAddress,
                                          bool reuseAddress) : base(udpAddress)
        {
            socket = new UdpClient(null);
            socket.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                reuseAddress);
            IPEndPoint addr =
                new IPEndPoint(udpAddress.GetSystemNetIpAddress(), udpAddress.Port);
            socket.Client.Bind(addr);
        }

        /**
         * Creates a UDP transport on the specified address. The address will not be
         * reused if it is currently in timeout state (TIME_WAIT).
         *
         * @param udpAddress
         *    the local address for sending and receiving of UDP messages.
         * @throws IOException
         *    if socket binding fails.
         */
        public DefaultUdpTransportMapping(UdpAddress udpAddress) : base(udpAddress)
        {
            socket = new UdpClient(
                new IPEndPoint(udpAddress.GetSystemNetIpAddress(), udpAddress.Port));
        }

        public override void SendMessage(UdpAddress targetAddress, byte[] message,
                                TransportStateReference tmStateReference)

        {
            IPEndPoint targetSocketAddress = new IPEndPoint(
                targetAddress.GetSystemNetIpAddress(),
                targetAddress.Port);

            if (log.IsDebugEnabled)
            {
                log.Debug("Sending message to " + targetAddress + " with length " +
                             message.Length + ": " +
                             new OctetString(message).ToHexString());
            }

            UdpClient s = EnsureSocket();
            s.Send(message, message.Length, targetSocketAddress);
        }

        /**
         * Closes the socket and stops the listener thread.
         *
         * @throws IOException
         */
        public override void Close()
        {
            if (listener != null)
            {
                cancelTokenSource.Cancel();
                try
                {
                    listener.Wait();
                }
                catch (AggregateException ae)
                {
                    if(log.IsDebugEnabled)
                    {
                        foreach (Exception e in ae.InnerExceptions)
                        {
                            log.Debug(e.Message);
                        }
                    }
                }
            }

            socket.Close();

            socket = null;
        }

        /**
         * Starts the listener thread that accepts incoming messages. The thread is
         * started in daemon mode and thus it will not block application terminated.
         * Nevertheless, the {@link #close()} method should be called to stop the
         * listen thread gracefully and free associated ressources.
         *
         * @throws IOException
         */
        // synchronized
        public override Task Listen()
        {
            if (listener != null)
            {
                throw new SocketException();
            }

            EnsureSocket();
            return listener = UDPListen();
        }

        //synchronized
        private UdpClient EnsureSocket()
        {
            UdpClient s = socket;

            if (s == null)
            {
                s = new UdpClient(udpAddress.Port);
                s.Client.ReceiveTimeout = socketTimeout;
                s.Client.SendTimeout = socketTimeout;
                this.socket = s;
            }

            return s;
        }
        
        public void SetMaxInboundMessageSize(int maxInboundMessageSize)
        {
            this.MaxInboundMessageSize = maxInboundMessageSize;
        }

        /**
         * Returns the socket timeout.
         * 0 returns implies that the option is disabled (i.e., timeout of infinity).
         * @return
         *    the socket timeout setting.
         */

        /**
         * Sets the socket timeout in milliseconds.
         * @param socketTimeout
         *    the socket timeout for incoming messages in milliseconds.
         *    A timeout of zero is interpreted as an infinite timeout.
         */
        public int SocketTimeout
        {
            get
            {
                return socketTimeout;
            }

            set
            {
                this.socketTimeout = value;
                if (socket != null)
                {
                    socket.Client.SendTimeout = value;
                    socket.Client.ReceiveTimeout = value;
                }
            }
        }

        /**
         * Gets the requested receive buffer size for the underlying UDP socket.
         * This size might not reflect the actual size of the receive buffer, which
         * is implementation specific.
         * @return
         *    &lt;=0 if the default buffer size of the OS is used, or a value >0 if the
         *    user specified a buffer size.
         */

        /**
         * Sets the receive buffer size, which should be > the maximum inbound message
         * size. This method has to be called before {@link #listen()} to be
         * effective.
         * @param receiveBufferSize
         *    an integer value >0 and > {@link #getMaxInboundMessageSize()}.
         */
        public int ReceiveBufferSize
        {
            get
            {
                return receiveBufferSize;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Receive buffer size must be > 0");
                }
                this.receiveBufferSize = value;
            }
        }

        public override bool IsListening
        {
            get
            {
                return (listener != null);
            }
        }

        public UdpAddress GetListenAddress()
        {
            UdpAddress actualListenAddress = null;

            if (socket != null)
            {
                IPEndPoint ipep = (IPEndPoint)(socket.Client.LocalEndPoint);
                actualListenAddress = new UdpAddress(ipep.Address, ipep.Port);
            }

            return actualListenAddress;
        }

        public override UdpAddress ListenAddress
        {
            get;
            set;
        }

        /**
         * If receiving new datagrams fails with a {@link SocketException}, this method is called to renew the
         * socket - if possible.
         * @param socketException
         *    the exception that occurred.
         * @param failedSocket
         *    the socket that caused the exception. By default, he socket will be closed
         *    in order to be able to reopen it. Implementations may also try to reuse the socket, in dependence
         *    of the <code>socketException</code>.
         * @return
         *    the new socket or <code>null</code> if the listen thread should be terminated with the provided
         *    exception.
         * @throws SocketException
         *    a new socket exception if the socket could not be renewed.
         * @since 2.2.2
         */
        protected UdpClient RenewSocketAfterException(SocketException socketException,
                                                           UdpClient failedSocket)
        {
            if (failedSocket != null)
            {
                failedSocket.Close();
            }

            UdpClient s = new UdpClient(new IPEndPoint(udpAddress.GetSystemNetIpAddress(), udpAddress.Port));
            s.Client.ReceiveTimeout = socketTimeout;
            s.Client.SendTimeout = socketTimeout;
            return s;
        }

        public Task UDPListen()
        {
            CancellationToken ct = cancelTokenSource.Token;

            return Task.Factory.StartNew(() =>
            {
                while (ct.IsCancellationRequested == false)
                {
                    IPEndPoint ep = new IPEndPoint(udpAddress.GetSystemNetIpAddress(), udpAddress.Port);
                    byte[] recvBytes = null;
                    try
                    {
                        
                        if (socket == null)
                        {
                            Interlocked.Exchange(ref stopListening, 1);
                            continue;
                        }

                        recvBytes = socket.Receive(ref ep);
                        
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Received message from " + udpAddress.GetSystemNetIpAddress() + "/" +
                                         udpAddress.Port +
                                         " with length " + recvBytes.Length + ": " +
                                         new OctetString(recvBytes, 0,
                                                         recvBytes.Length).ToHexString());
                        }

                        MemoryStream bis = new MemoryStream();
                        bis.Write(recvBytes, 0, recvBytes.Length);

                        TransportStateReference stateReference =
                          new TransportStateReference((ITransportMapping<IAddress>)this, udpAddress, null,
                                                      SecurityLevel.Undefined, SecurityLevel.Undefined,
                                                      false, socket);

                        FireProcessMessage(new UdpAddress(ep.Address,
                                                          ep.Port), bis, stateReference);
                    }
                    catch (Exception ex)
                    {
                        log.Warn(ex.Message);
                    }
                }
            }, cancelTokenSource.Token);
        }
    }
}
