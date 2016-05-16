using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Snmp
{
    /**
     * An SNMP module compliance value. This declaration is used inside
     * the module compliance macro type.
     *
     * @see SnmpModuleCompliance
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    public class SnmpModule
    {

        /**
         * The module name.
         */
        private string module;

        /**
         * The list of mandatory group values.
         */
        private IList<MibValue> groups;

        /**
         * The list of compliances.
         */
        private IList<SnmpCompliance> compliances;

        /**
         * The module comment.
         */
        private string comment = null;

        /**
         * Creates a new module compliance declaration.
         *
         * @param module         the module name, or null
         * @param groups         the list of mandatory group values
         * @param compliances    the list of compliances
         */
        public SnmpModule(string module,
                          IList<MibValue> groups,
                          IList<SnmpCompliance> compliances)
        {

            this.module = module;
            this.groups = groups;
            this.compliances = compliances;
        }

        /**
         * Initializes the object. This will remove all levels of
         * indirection present, such as references to other types, and
         * returns the basic type. No type information is lost by this
         * operation. This method may modify this object as a
         * side-effect, and will be called by the MIB loader.
         *
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public void Initialize(MibLoaderLog log)
        {
            groups = groups.Select(g => g.Initialize(log, null)).ToList();

            foreach (SnmpCompliance c in compliances)
                c.Initialize(log);
        }

        /**
         * Returns the module name.
         *
         * @return the module name, or
         *         null if not set
         */
        public string getModule()
        {
            return module;
        }

        /**
         * Returns the list of mandatory group values. The returned list
         * will consist of MibValue instances.
         *
         * @return the list of mandatory group values
         *
         * @see net.percederberg.mibble.MibValue
         */
        public IEnumerable<MibValue> getGroups()
        {
            return groups;
        }

        /**
         * Returns the list of compliances. The returned list will
         * consist of SnmpCompliance instances.
         *
         * @return the list of compliances
         *
         * @see SnmpCompliance
         */
        public IEnumerable<SnmpCompliance> getCompliances()
        {
            return compliances;
        }

        /**
         * Returns the module comment.
         *
         * @return the module comment, or
         *         null if no comment was set
         *
         * @since 2.9
         */
        public string getComment()
        {
            return comment;
        }

        /**
         * Sets the module comment.
         *
         * @param comment        the module comment
         *
         * @since 2.9
         */
        public void setComment(string comment)
        {
            if (module != null || !"THIS MODULE".Equals(comment, StringComparison.CurrentCultureIgnoreCase))
            {
                this.comment = comment;
            }
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (module != null)
            {
                builder.Append(module);
            }
            if (groups.Count > 0)
            {
                builder.Append("\n    Mandatory Groups: ");
                builder.Append(groups);
            }
            foreach(SnmpCompliance c in compliances)
            {
                builder.Append("\n    Module: ");
                builder.Append(c);
            }
            return builder.ToString();
        }
    }

}
