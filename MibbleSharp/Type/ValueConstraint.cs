using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{
    /**
     * A MIB type value constraint. This class represents a single value
     * in a set of value constraints.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class ValueConstraint : Constraint
    {

    /**
     * The constraint location. This value is reset to null once the
     * constraint has been initialized. 
     */
    private FileLocation location;

    /**
     * The constraint value.
     */
    private MibValue value;

    /**
     * Creates a new value constraint.
     *
     * @param location       the constraint location
     * @param value          the constraint value
     */
    public ValueConstraint(FileLocation location, MibValue value)
    {
        this.location = location;
        this.value = value;
    }

    /**
     * Initializes the constraint. This will remove all levels of
     * indirection present, such as references to types or values. No
     * constraint information is lost by this operation. This method
     * may modify this object as a side-effect, and will be called by
     * the MIB loader.
     *
     * @param type           the type to constrain
     * @param log            the MIB loader log
     *
     * @throws MibException if an error was encountered during the
     *             initialization
     */
    public void Initialize(MibType type, MibLoaderLog log)
    {

        string message;

        value = value.Initialize(log, type);
        if (location != null && !IsCompatible(type)) {
            message = "Value constraint not compatible with this type";
            log.addWarning(location, message);
        }
        location = null;
    }

    /**
     * Checks if the specified type is compatible with this
     * constraint.
     *
     * @param type            the type to check
     *
     * @return true if the type is compatible, or
     *         false otherwise
     */
    public bool IsCompatible(MibType type)
    {
        return type == null || value == null || type.IsCompatible(value);
    }

    /**
     * Checks if the specified value is compatible with this
     * constraint.
     *
     * @param value          the value to check
     *
     * @return true if the value is compatible, or
     *         false otherwise
     */
    public bool IsCompatible(MibValue value)
    {
        string str1 = this.value.ToString();
        string str2 = value.ToString();

        if (this.value is NumberValue
         && value is NumberValue) {

            return str1.Equals(str2);
        } else if (this.value is StringValue
                && value is StringValue) {

            return str1.Equals(str2);
        } else {
            return false;
        }
    }

    /**
     * Returns the constraint value.
     *
     * @return the constraint value
     */
    public MibValue getValue()
    {
        return value;
    }

    /**
     * Returns a string representation of this object.
     *
     * @return a string representation of this object
     */
    public override string ToString()
    {
        return value.ToString();
    }
}

}
