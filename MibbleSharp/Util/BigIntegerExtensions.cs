/// Per StackOverflow requirements, this code is licensed under Creative Commons
/// CC BY-SA 3.0
/// <see cref="http://creativecommons.org/licenses/by-sa/3.0/"/>
/// You are free to:
///
/// Share — copy and redistribute the material in any medium or format
/// Adapt — remix, transform, and build upon the material for any purpose, even
/// commercially.
/// The licensor cannot revoke these freedoms as long as you follow the license
/// terms.
///
/// Under the following terms:
///
/// Attribution — You must give appropriate credit, provide a link to the
/// license, and indicate if changes were made.You may do so in any reasonable
/// manner, but not in any way that suggests the licensor endorses you or your
/// use.
/// ShareAlike — If you remix, transform, or build upon the material, you must
/// distribute your contributions under the same license as the original.
/// No additional restrictions — You may not apply legal terms or technological
/// measures that legally restrict others from doing anything the license
/// permits.
///
/// Notices:
///
/// You do not have to comply with the license for elements of the material in
/// the public domain or where your use is permitted by an applicable exception
/// or limitation.
/// No warranties are given.The license may not give you all of the permissions
/// necessary for your intended use. For example, other rights such as 
/// publicity, privacy, or moral rights may limit how you use the material.

using System;
using System.Numerics;
using System.Text;

namespace MibbleSharp.Util
{
    /// <summary>
    /// Extension methods to convert <see cref="System.Numerics.BigInteger"/>
    /// instances to hexadecimal, octal, and binary strings.
    /// </summary>
    /// <remarks>
    /// Originally from StackOverflow
    /// <see cref="http://stackoverflow.com/a/15447131/5765929">BigInteger to Hex/Decimal/Octal/Binary strings?</see>
    /// Author Kevin P. Rice <see cref="http://stackoverflow.com/users/733805/kevin-p-rice"/>
    /// </remarks>
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a binary
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToBinaryString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a hexadecimal string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing a hexadecimal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToHexadecimalString(this BigInteger bigint)
        {
            return bigint.ToString("X");
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a octal string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="System.String"/> containing an octal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToOctalString(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base8 = new StringBuilder(((bytes.Length / 3) + 1) * 8);

            // Calculate how many bytes are extra when byte array is split
            // into three-byte (24-bit) chunks.
            var extra = bytes.Length % 3;

            // If no bytes are extra, use three bytes for first chunk.
            if (extra == 0)
            {
                extra = 3;
            }

            // Convert first chunk (24-bits) to integer value.
            int int24 = 0;
            for (; extra != 0; extra--)
            {
                int24 <<= 8;
                int24 += bytes[idx--];
            }

            // Convert 24-bit integer to octal without adding leading zeros.
            var octal = Convert.ToString(int24, 8);

            // Ensure leading zero exists if value is positive.
            if (octal[0] != '0' && bigint.Sign == 1)
            {
                base8.Append('0');
            }

            // Append first converted chunk to StringBuilder.
            base8.Append(octal);

            // Convert remaining 24-bit chunks, adding leading zeros.
            for (; idx >= 0; idx -= 3)
            {
                int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
                base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
            }

            return base8.ToString();
        }
    }
}
