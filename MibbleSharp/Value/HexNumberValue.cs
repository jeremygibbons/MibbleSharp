using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MibbleSharp.Util;

namespace MibbleSharp.Value
{
    /**
     * A hexadecimal numeric MIB value.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.6
     */
    public class HexNumberValue : NumberValue
    {

        /**
         * The minimum number of hexadecimal characters to print.
         */
        private int minLength;

        /**
         * Creates a new hexadecimal number value. A default minimum
         * print length of one (1) will be used.
         *
         * @param value          the number value
         */
        public HexNumberValue(BigInteger value) : this(value, 1)
        {

        }

        /**
         * Creates a new hexadecimal number value.
         *
         * @param value          the number value
         * @param minLength      the minimum print length
         *
         * @since 2.9
         */
        public HexNumberValue(BigInteger value, int minLength) : base(value)
        {
            this.minLength = minLength;
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
            int bytes = minLength / 2 + ((minLength % 2 > 0) ? 1 : 0);
            int length = getByteSize(type, bytes) * 2;
            if (length > minLength)
            {
                minLength = length;
            }
            return this;
        }

        /**
         * Returns a string representation of this value.
         *
         * @return a string representation of this value
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string value;

            builder.Append("'");
            value = toHexString();
            if (value.Equals("0"))
            {
                value = "";
            }
            for (int i = value.Length; i < minLength; i++)
            {
                builder.Append("0");
            }
            builder.Append(value);
            builder.Append("'H");
            return builder.ToString();
        }

        /**
         * Returns a hexadecimal representation of this value.
         *
         * @return a hexadecimal representation of this value
         */
        private string toHexString()
        {
            return value.ToHexadecimalString();
        }
    }
}
