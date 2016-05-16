using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP object identity macro type. This macro type was added to
     * SMIv2 and is defined in RFC 2578.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2578.txt">RFC 2578 (SNMPv2-SMI)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.5
     * @since    2.0
     */
    public class SnmpObjectIdentity : SnmpType
    {

        /**
         * The object identity status.
         */
        private SnmpStatus status;

        /**
         * The object identity reference.
         */
        private string reference;

        /**
         * Creates a new SNMP object identity.
         *
         * @param status         the object identity status
         * @param description    the object identity description
         * @param reference      the object identity reference, or null
         */
        public SnmpObjectIdentity(SnmpStatus status,
                                  string description,
                                  string reference)
                : base("OBJECT-IDENTITY", description)
        {
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
         * Returns the object identity status.
         *
         * @return the object identity status
         */
        public SnmpStatus getStatus()
        {
            return status;
        }

        /**
         * Returns the object identity reference.
         *
         * @return the object identity reference, or
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
