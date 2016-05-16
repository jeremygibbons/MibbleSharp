using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Snmp
{
    /**
     * An SNMP status value. This class is used to encapsulate the status
     * value constants used in several SNMP macro types. Note that, due
     * to the support for both SMIv1 and SMIv2, not all of the constants
     * defined in this class can be present in all files. Please see the
     * comments for each individual constant regarding the support for
     * different SMI versions.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.2
     * @since    2.0
     */
    public class SnmpStatus
    {

        /**
         * The mandatory SNMP status. This status is only used in SMIv1.
         */
        public static readonly SnmpStatus MANDATORY =
        new SnmpStatus("mandatory");

        /**
         * The optional SNMP status. This status is only used in SMIv1.
         */
        public static readonly SnmpStatus OPTIONAL =
        new SnmpStatus("optional");

        /**
         * The current SNMP status. This status is only used in SMIv2
         * and later.
         */
        public static readonly SnmpStatus CURRENT =
        new SnmpStatus("current");

        /**
         * The deprecated SNMP status. This status is only used in SMIv2
         * and later.
         */
        public static readonly SnmpStatus DEPRECATED =
        new SnmpStatus("deprecated");

        /**
         * The obsolete SNMP status.
         */
        public static readonly SnmpStatus OBSOLETE =
        new SnmpStatus("obsolete");

        /**
         * The status description.
         */
        private string description;

        /**
         * Creates a new SNMP status.
         *
         * @param description    the status description
         */
        private SnmpStatus(string description)
        {
            this.description = description;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            return description;
        }
    }

}
