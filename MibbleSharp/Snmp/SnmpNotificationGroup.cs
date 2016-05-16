using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP notification group macro type. This macro type was added
     * to SMIv2 and is defined in RFC 2580.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2580.txt">RFC 2580 (SNMPv2-CONF)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpNotificationGroup : SnmpType
    {

        /**
         * The list of notification values.
         */
        private IList<MibValue> notifications;

        /**
         * The type status.
         */
        private SnmpStatus status;

        /**
         * The type reference.
         */
        private string reference;

        /**
         * Creates a new SNMP notification group.
         *
         * @param notifications  the list of notification values
         * @param status         the type status
         * @param description    the type description
         * @param reference      the type reference, or null
         */
        public SnmpNotificationGroup(IList<MibValue> notifications,
                                     SnmpStatus status,
                                     string description,
                                     string reference)
                : base("NOTIFICATION-GROUP", description)
        {
            this.notifications = notifications;
            this.status = status;
            this.reference = reference;
        }

        /**
         * Initializes the MIB type. This will remove all levels of
         * indirection present, such as references to types or values. No
         * information is lost by this operation. This method may modify
         * this object as a side-effect, and will return the basic
         * type.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param symbol         the MIB symbol containing this type
         * @param log            the MIB loader log
         *
         * @return the basic MIB type
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         *
         * @since 2.2
         */
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            if (!(symbol is MibValueSymbol))
            {
                throw new MibException(symbol.getLocation(),
                                       "only values can have the " +
                                       Name + " type");
            }
            notifications = notifications.Select(n => n.Initialize(log, null)).ToList();
            return this;
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if and only if it is an object identifier
         * value.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return value is ObjectIdentifierValue;
        }

        /**
         * Returns the list of notification MIB values.
         *
         * @return the list of notification MIB values
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IEnumerable<MibValue> getNotifications()
        {
            return notifications;
        }

        /**
         * Returns the type status.
         *
         * @return the type status
         */
        public SnmpStatus getStatus()
        {
            return status;
        }

        /**
         * Returns the type reference.
         *
         * @return the type reference, or
         *         null if no reference has been set
         */
        public string getReference()
        {
            return reference;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString());
            builder.Append(" (");
            builder.Append("\n  Notifications: ");
            builder.Append(notifications);
            builder.Append("\n  Status: ");
            builder.Append(status);
            builder.Append("\n  Description: ");
            builder.Append(getDescription("               "));
            if (reference != null)
            {
                builder.Append("\n  Reference: ");
                builder.Append(reference);
            }
            builder.Append("\n)");
            return builder.ToString();
        }
    }

}
