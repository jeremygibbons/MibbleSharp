using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Snmp
{
    /**
     * The SNMP module compliance macro type. This macro type was added
     * to SMIv2 and is defined in RFC 2580.
     *
     * @see <a href="http://www.ietf.org/rfc/rfc2580.txt">RFC 2580 (SNMPv2-CONF)</a>
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.5
     * @since    2.0
     */
    public class SnmpModuleCompliance : SnmpType
    {

    /**
     * The type status.
     */
    private SnmpStatus status;

    /**
     * The type reference.
     */
    private string reference;

    /**
     * The list of modules.
     */
    private IList<SnmpModule> modules;

    /**
     * Creates a new SNMP module compliance type.
     *
     * @param status         the type status
     * @param description    the type description
     * @param reference      the type reference, or null
     * @param modules        the list of SNMP modules
     */
    public SnmpModuleCompliance(SnmpStatus status,
                                string description,
                                string reference,
                                IList<SnmpModule> modules)
            : base("MODULE-COMPLIANCE", description)
        {
        this.status = status;
        this.reference = reference;
        this.modules = modules;
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

        if (!(symbol is MibValueSymbol)) {
            throw new MibException(symbol.getLocation(),
                                   "only values can have the " +
                                   Name + " type");
        }
            foreach (var m in modules)
                m.Initialize(log);

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
     * Returns the list of SNMP modules. The returned list will
     * consist of SnmpModule instances.
     *
     * @return the list of SNMP modules
     *
     * @see SnmpModule
     */
    public IEnumerable<SnmpModule> getModules()
    {
        return modules;
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
        foreach(var m in modules)
        {
            builder.Append("\n  Module: ");
            builder.Append(m);
        }
        builder.Append("\n)");
        return builder.ToString();
    }
}

}
