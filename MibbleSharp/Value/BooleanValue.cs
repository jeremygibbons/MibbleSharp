using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Value
{

    /**
     * A boolean MIB value.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class BooleanValue : MibValue
    {

    /**
     * The boolean true value.
     */
    public static readonly BooleanValue TRUE = new BooleanValue(true);

    /**
     * The boolean false value.
     */
    public static readonly BooleanValue FALSE = new BooleanValue(false);

    /**
     * The underlying boolean value.
     */
    private bool value;

    /**
     * Creates a new boolean MIB value.
     *
     * @param value          the boolean value
     */
    private BooleanValue(bool value) : base("BOOLEAN")
    {
        this.value = value;
    }

    /**
     * Initializes the MIB value. This will remove all levels of
     * indirection present, such as references to other values. No
     * value information is lost by this operation. This method may
     * modify this object as a side-effect, and will return the basic
     * value.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @param log            the MIB loader log
     * @param type           the value type
     *
     * @return the basic MIB value
     */
    public override MibValue Initialize(MibLoaderLog log, MibType type)
    {
        return this;
    }

    /**
     * Creates a value reference to this value. The value reference
     * is normally an identical value. Only certain values support
     * being referenced, and the default implementation of this
     * method throws an exception.<p>
     *
     * <strong>NOTE:</strong> This is an internal method that should
     * only be called by the MIB loader.
     *
     * @return the MIB value reference
     *
     * @since 2.2
     */
    public override MibValue CreateReference()
    {
        return new BooleanValue(value);
    }

    /**
     * Compares this object with the specified object for order. This
     * method will only compare the string representations with each
     * other.
     *
     * @param obj            the object to compare to
     *
     * @return less than zero if this object is less than the specified,
     *         zero if the objects are equal, or
     *         greater than zero otherwise
     *
     * @since 2.6
     */
    public int CompareTo(Object obj)
    {
        return ToString().CompareTo(obj.ToString());
    }

    /**
     * Checks if this object equals another object. This method will
     * compare the string representations for equality.
     *
     * @param obj            the object to compare with
     *
     * @return true if the objects are equal, or
     *         false otherwise
     *
     * @since 2.6
     */
    public override bool Equals(Object obj)
    {
        return ToString().Equals(obj.ToString());
    }

    /**
     * Returns a hash code for this object.
     *
     * @return a hash code for this object
     *
     * @since 2.6
     */
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    /**
     * Returns a string representation of this value.
     *
     * @return a string representation of this value
     */
    public override string ToString()
    {
        return value ? "TRUE" : "FALSE";
    }

        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }
    }

}
