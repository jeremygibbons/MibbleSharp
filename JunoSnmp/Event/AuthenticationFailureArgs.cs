
namespace JunoSnmp.Event
{
    using System;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;

    /**
 * The <code>AuthenticationFailureEvent</code> class describes the source
 * and type of an authentication failure as well as the message that caused
 * the error.
 *
 * @author Frank Fock
 * @version 1.5
 * @since 1.5
 */
    public class AuthenticationFailureArgs : EventArgs
    {
        private object source;
        private IAddress address;
        private ITransportMapping<IAddress> transport;
        private BERInputStream message;
        private int error;

        /**
         * Creates an authentication failure event.
         * @param source
         *    the instance that generated the event.
         * @param sourceAddress
         *    the address from where the failed message has been received.
         * @param transport
         *    the <code>TransportMapping</code> with which the message has been
         *    received.
         * @param error
         *    the SNMP4J MP error status caused by the message
         *    (see {@link SnmpConstants}).
         * @param message
         *    the message as received at the position where processing the message
         *    has stopped.
         */
        public AuthenticationFailureArgs(IAddress sourceAddress,
                                          ITransportMapping<IAddress> transport,
                                          int error,
                                          BERInputStream message)
        {
            this.address = sourceAddress;
            this.transport = transport;
            this.error = error;
            this.message = message;
        }

        /**
         * Returns the transport mapping over which the message has bee received.
         * @return
         *    a <code>TransportMapping</code> instance.
         */
        public ITransportMapping<IAddress> Transport
        {
            get
            {
                return transport;
            }
        }

        /**
         * Returns the message received.
         * @return
         *    a <code>BERInputStream</code> at the position where processing of the
         *    message has stopped.
         */
        public BERInputStream Message
        {
            get
            {
                return message;
            }
        }

        /**
         * Returns the SNMP4J internal error status caused by the message.
         * @return
         *    the error status.
         */
        public int Error
        {
            get
            {
                return error;
            }
        }

        /**
         * Returns the source address from which the message has been received.
         * @return
         *    the source <code>Address</code>.
         */
        public IAddress Address
        {
            get
            {
                return address;
            }
        }
    }

}
