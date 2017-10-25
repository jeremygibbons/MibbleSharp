// <copyright file="DummyTransport.cs" company="None">
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
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>DummyTransport</c> is a test TransportMapping for Command Generators
    /// which actually does not sent messages over the network.Instead it provides the message
    /// transparently as incoming message over the <see cref="DummyTransportResponder"/>
    /// on a virtual listen address, regardless to which outbound address the message was sent.The messages
    /// are returned even if the <c>listenAddress</c> is left <c>null</c>.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class DummyTransport<A> : AbstractTransportMapping<A> where A : IAddress
    {

        private readonly ConcurrentQueue<MessageContainer> requests = new ConcurrentQueue<MessageContainer>();
        private readonly ConcurrentQueue<MessageContainer> responses = new ConcurrentQueue<MessageContainer>();
        private bool listening;
        private A listenAddress;
        private A receiverAddress;
        private long sessionID = 0;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken ct;

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DummyTransport()
        {
            this.listening = false;
        }

        public DummyTransport(A senderAddress)
        {
            this.listenAddress = senderAddress;
        }

        public DummyTransport(A senderAddress, A receiverAddress)
        {
            this.listenAddress = senderAddress;
            this.receiverAddress = receiverAddress;
        }

        public override Type SupportedAddressClass
        {
            get
            {
                return typeof(IpAddress);
            }
        }

        public override A ListenAddress
        {
            get
            {
                return listenAddress;
            }

            set
            {
                listenAddress = value;
            }
        }

        public void setListenAddress(A listenAddress)
        {
            this.listenAddress = listenAddress;
        }


        public override void  SendMessage(A address, byte[] message, TransportStateReference tmStateReference)
        {
            lock(requests) {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Send request message to '" + address + "': " + new OctetString(message).ToHexString());
                }
                requests.Enqueue(new MessageContainer(address, new OctetString(message)));
            }
        }


        public override void Close()
        {
            cts.Cancel();
            listening = false;
        }


        public override Task Listen()
        {
            listening = true;
            sessionID++;

            return Task.Factory.StartNew(() => {
                while (!cts.Token.IsCancellationRequested)
                {
                    MessageContainer nextMessage = null;
                    while (!requests.TryDequeue(out nextMessage)) ;

                    if (nextMessage != null)
                    {
                        TransportStateReference stateReference =
                          new TransportStateReference((ITransportMapping<IAddress>)this, this.listenAddress, null,
                                                      SecurityLevel.Undefined, SecurityLevel.Undefined,
                                                      false, this.sessionID);
                        this.FireProcessMessage(this.listenAddress,
                            new MemoryStream(nextMessage.Payload.GetValue()), stateReference);
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            }, cts.Token);
        }

        public override bool IsListening
        {
            get
            {
                return listening;
            }
        }

        public AbstractTransportMapping<A> GetResponder(A receiverAddress)
        {
            this.receiverAddress = receiverAddress;
            return new DummyTransportResponder(this);
        }

        public class DummyTransportResponder : AbstractTransportMapping<A>
        {

            private bool listening;
            private readonly DummyTransport<A> dummyTransport;

            public DummyTransportResponder(DummyTransport<A> dt)
            {
                this.dummyTransport = dt;
            }

            public override Type SupportedAddressClass
            {
                get
                {
                    return typeof(IpAddress);
                }

            }

            public override A ListenAddress
            {
                get
                {
                    return dummyTransport.receiverAddress;
                }
                set
                {
                    dummyTransport.receiverAddress = value;
                }

            }

            public override void SendMessage(A address, byte[] message, TransportStateReference tmStateReference)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Send response message to '" + address + "': " + new OctetString(message).ToHexString());
                }

                dummyTransport.responses.Enqueue(new MessageContainer(address, new OctetString(message)));
            }


            public override void Close()
            {
                this.listening = false;
                this.dummyTransport.cts.Cancel();
            }

            public override Task Listen()
            {

                this.listening = true;
                dummyTransport.sessionID++;

                return Task.Factory.StartNew(() => {
                    while (!this.dummyTransport.cts.Token.IsCancellationRequested)
                    {
                        MessageContainer nextMessage = null;
                        while (!this.dummyTransport.requests.TryDequeue(out nextMessage)) ;

                        if (nextMessage != null)
                        {
                            TransportStateReference stateReference =
                              new TransportStateReference((ITransportMapping<IAddress>)this, this.dummyTransport.listenAddress, null,
                                                          SecurityLevel.Undefined, SecurityLevel.Undefined,
                                                          false, this.dummyTransport.sessionID);
                            this.FireProcessMessage(this.dummyTransport.listenAddress,
                                new MemoryStream(nextMessage.Payload.GetValue()), stateReference);
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
                    }
                }, this.dummyTransport.cts.Token);
            }

            public override bool IsListening
            {
                get
                {
                    return this.listening;
                }
            }
        }

        public override string ToString()
        {
            return "DummyTransport{" +
                "requests=" + this.requests +
                ", responses=" + this.responses +
                ", listening=" + this.listening +
                ", listenAddress=" + this.listenAddress +
                ", receiverAddress=" + this.receiverAddress +
                ", sessionID=" + this.sessionID +
                '}';
        }

        private class MessageContainer
        {
            private readonly IAddress sourceAddress;
            private readonly OctetString payload;

            public MessageContainer(IAddress sourceAddress, OctetString payload)
            {
                this.payload = payload;
                this.sourceAddress = sourceAddress;
            }

            public OctetString Payload
            {
                get
                {
                    return this.payload;
                }
            }

            public IAddress SourceAddress
            {
                get
                {
                    return this.sourceAddress;
                }
            }
        }
    }
}
