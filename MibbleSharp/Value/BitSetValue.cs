using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Value
{
    /**
     * A bit set MIB value.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.8
     * @since    2.0
     */
    public class BitSetValue : MibValue
    {

        /**
         * The bit set value.
         */
        private BitArray value;

        /**
         * The additional value references.
         */
        private IList<ValueReference> references;

        /**
         * Creates a new bit set MIB value.
         *
         * @param value          the bit set value
         */
        public BitSetValue(BitArray value) : this(value, null)
        {

        }

        /**
         * Creates a new bit set MIB value.
         *
         * @param value          the bit set value
         * @param references     the additional referenced bit values
         */
        public BitSetValue(BitArray value, IList<ValueReference> references) : base("BIT STRING")
        {
            this.value = value;
            this.references = references;
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
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public override MibValue Initialize(MibLoaderLog log, MibType type)
        {

            if (references != null)
            {
                foreach(ValueReference vref in references)
                {
                    Initialize(log, type, vref);
                }
                references = null;
            }
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
            return new BitSetValue(value, references);
        }

        /**
         * Initializes a the MIB value from a value reference. This will
         * resolve the reference, and set the bit corresponding to the
         * value.
         *
         * @param log            the MIB loader log
         * @param type           the value type
         * @param ref            the value reference to resolve
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        private void Initialize(MibLoaderLog log, MibType type, ValueReference vref)
        {

            MibValue val = vref.Initialize(log, type);

            NumberValue nv = val as NumberValue;

            if (nv != null)
            {
                value.Set((int) nv.value, true);
            }
            else
            {
                throw new MibException(vref.getLocation(),
                                       "referenced value is not a number");
            }
        }

        /**
         * Clears and prepares this value for garbage collection. This
         * method will recursively clear any associated types or values,
         * making sure that no data structures references this object.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         */
        public override void Clear()
        {
            base.Clear();
            value = null;
            references = null;
        }

        /**
         * Returns all the bits in this bit set as individual number
         * values.
         *
         * @return the number values for all bits in this bit set
         */
        public IList<NumberValue> getBits()
        {

            IList<NumberValue> components = new List<NumberValue>();
            foreach (bool b in value)
            {
                components.Add(new NumberValue((ulong) (b ? 1 : 0)));
            }
            return components;
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
         * Returns a Java BitSet representation of this value.
         *
         * @return a Java BitSet representation of this value
         */
        public Object toObject()
        {
            return value;
        }

        /**
         * Returns a string representation of this value.
         *
         * @return a string representation of this value
         */
        public override string ToString()
        {
            return value.ToString();
        }

        /**
         * Returns an ASN.1 representation of this value. The string will
         * contain named references to any values that can be found in the
         * specified list.
         *
         * @param values         the defined symbol values
         *
         * @return an ASN.1 representation of this value
         * 
         * @since 2.8
         */
        public string toAsn1String(MibValueSymbol[] values)
        {
            StringBuilder builder = new StringBuilder();

            foreach (bool b in value)
            {
                if (b)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(toAsn1String(b, values));
                }
            }

            if (builder.Length > 0)
            {
                return "{ " + builder.ToString() + " }";
            }
            else
            {
                return "{}";
            }
        }

        /**
         * Returns an ASN.1 representation of a bit number. The value
         * name will be returned if found in the specified array.
         *
         * @param bit            the bit number
         * @param values         the defined bit names
         *
         * @return the ASN.1 representation of the bit number
         */
        private string toAsn1String(bool bit, MibValueSymbol[] values)
        {
            foreach (MibValueSymbol s in values)
            {
                if (s.getValue().Equals(bit))
                {
                    return s.getName();
                }
            }
            return bit.ToString();
        }

        public override int CompareTo(MibValue other)
        {
            throw new NotImplementedException();
        }
    }

}
