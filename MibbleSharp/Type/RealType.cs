using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{
    /**
     * A real MIB type.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.2
     * @since    2.0
     */
    public class RealType : MibType
    {

        /**
         * Creates a new real MIB type.
         */
        public RealType() : this(true)
        {

        }

        /**
         * Creates a new real MIB type.
         *
         * @param primitive      the primitive type flag
         */
        private RealType(bool primitive) : base("REAL", primitive)
        {

            setTag(true, MibTypeTag.REAL);
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
         * @since 2.2
         */
        public override MibType Initialize(MibSymbol symbol, MibLoaderLog log)
        {
            return this;
        }

        /**
         * Creates a type reference to this type. The type reference is
         * normally an identical type, but with the primitive flag set to
         * false. Only certain types support being referenced, and the
         * default implementation of this method throws an exception.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @return the MIB type reference
         *
         * @since 2.2
         */
        public override MibType CreateReference()
        {
            RealType type = new RealType(false);

            type.setTag(true, getTag());
            return type;
        }

        /**
         * Checks if the specified value is compatible with this type. A
         * value is compatible if and only if it is an numeric value
         * representing positive or negative infinity.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public override bool IsCompatible(MibValue value)
        {
            RealValue number = value as RealValue;

            if(number != null)
            {
                return double.IsInfinity(number.Value);
            }

            return false;
        }
    }
}
