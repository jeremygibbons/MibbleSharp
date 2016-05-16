using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP trap type macro. This macro type is only present in
     * SMIv1 and is defined in RFC 1215. In SMIv2 and later, the
     * notification type macro should be used instead.
     *
     * @see SnmpNotificationType
     * @see <a href="http://www.ietf.org/rfc/rfc1215.txt">RFC 1215 (RFC-1215)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class SnmpTrapType : SnmpType
    {

        /**
         * The enterprise value.
         */
        private MibValue enterprise;

        /**
         * The list of MIB values.
         */
        private IList<MibValue> variables;

        /**
         * The type reference.
         */
        private string reference;

        /**
         * Creates a new SNMP trap type.
         *
         * @param enterprise     the enterprise value
         * @param variables      the list of MIB values
         * @param description    the type description, or null
         * @param reference      the type reference, or null
         */
        public SnmpTrapType(MibValue enterprise,
                            IList<MibValue> variables,
                            string description,
                            string reference)
                : base("TRAP-TYPE", description)
        {
            this.enterprise = enterprise;
            this.variables = variables;
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

            enterprise = enterprise.Initialize(log, null);
            variables = variables.Select(v => v.Initialize(log, null)).ToList();
            return this;
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if and only if it is an integer number
         * value.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            return value is NumberValue;
        }

        /**
         * Returns the enterprise value.
         *
         * @return the enterprise value
         */
        public MibValue getEnterprise()
        {
            return enterprise;
        }

        /**
         * Returns the list of MIB values.
         *
         * @return the list of MIB values
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IList<MibValue> getVariables()
        {
            return variables;
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
            builder.Append("\n  Enterprise: ");
            builder.Append(enterprise);
            builder.Append("\n  Variables: ");
            builder.Append(variables);
            if (getUnformattedDescription() != null)
            {
                builder.Append("\n  Description: ");
                builder.Append(getDescription("               "));
            }
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
