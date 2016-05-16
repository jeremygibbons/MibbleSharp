using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP notification type macro. This macro type was added to
     * SMIv2 and is defined in RFC 2578. The notification type macro is
     * used instead of the trap type macro in SMIv2.
     *
     * @see SnmpTrapType
     * @see <a href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 (SNMPv2-SMI)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpNotificationType : SnmpType
    {

        /**
         * The value objects.
         */
        private IList<MibValue> objects;

        /**
         * The notification type status.
         */
        private SnmpStatus status;

        /**
         * The notification type reference.
         */
        private string reference;

        /**
         * Creates a new SNMP notification type.
         *
         * @param objects        the value objects
         * @param status         the notification type status
         * @param description    the notification type description
         * @param reference      the notification type reference, or null
         */
        public SnmpNotificationType(IList<MibValue> objects,
                                    SnmpStatus status,
                                    string description,
                                    string reference)
                : base("NOTIFICATION-TYPE", description)
        {
            this.objects = objects;
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

            objects = objects.Select(o => o.Initialize(log, null)).ToList();

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
         * Returns the value objects. The returned list will consist of
         * MibValue instances.
         *
         * @return the value objects
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IList<MibValue> getObjects()
        {
            return objects;
        }

        /**
         * Returns the notification type status.
         *
         * @return the notification type status
         */
        public SnmpStatus getStatus()
        {
            return status;
        }

        /**
         * Returns the notification type reference.
         *
         * @return the notification type reference, or
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
            builder.Append("\n  Objects: ");
            builder.Append(objects);
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
