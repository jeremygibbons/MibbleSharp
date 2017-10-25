// <copyright file="OctetString.cs" company="None">
//    <para>
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at</para>
//    <para>
//    http://www.apache.org/licenses/LICENSE-2.0
//    </para><para>
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.</para>
//    <para>
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.SMI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using JunoSnmp.ASN1;
    using JunoSnmp.Util;

    /// <summary>
    /// The <c>OctetString</c> class represents the SMI type OCTET STRING.
    /// </summary>
    public class OctetString : AbstractVariable, IAssignableFrom<byte[]>, IAssignableFrom<string>, IEquatable<OctetString>
    {
        private static readonly char DefaultHexDelimiter = ':';

        private byte[] value = new byte[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="OctetString"/> class, of zero length.
        /// </summary>
        public OctetString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctetString"/> class from a byte array.
        /// </summary>
        /// <param name="rawValue">An array of bytes</param>
        public OctetString(byte[] rawValue) : this(rawValue, 0, rawValue.Length)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctetString"/> class from a byte array
        /// </summary>
        /// <param name="rawValue">The array of bytes to initialize from</param>
        /// <param name="offset">The zero-based position of the first byte to be copied from
        /// <c>rawValue</c> into the new <c>OctetString</c>
        /// </param>
        /// <param name="length">The number of bytes to be copied</param>
        public OctetString(byte[] rawValue, int offset, int length)
        {
            this.value = new byte[length];
            System.Array.Copy(rawValue, offset, this.value, 0, length);
        }

        /// <summary>
        /// Creates an octet string from a string.
        /// </summary>
        /// <param name="stringValue">A string representing the OctetString</param>
        public OctetString(string stringValue)
        {
            this.value = Encoding.UTF8.GetBytes(stringValue);
        }

        /// <summary>
        /// Creates an octet string from another OctetString by cloning its value.
        /// </summary>
        /// <param name="other">An Octet String instance</param>
        public OctetString(OctetString other)
        {
            this.value = new byte[0];
            this.Append(other);
        }

        /// <summary>Appends a single byte to this octet string.</summary>
        /// <param name="b">A byte value</param>
        public void Append(byte b)
        {
            byte[] newValue = new byte[this.value.Length + 1];
            System.Array.Copy(this.value, 0, newValue, 0, this.value.Length);
            newValue[this.value.Length] = b;
            this.value = newValue;
        }

        /// <summary>
        /// Appends an array of bytes to this octet string.
        /// </summary>
        /// <param name="bytes">An array of bytes to append</param>
        public void Append(byte[] bytes)
        {
            byte[] newValue = new byte[this.value.Length + bytes.Length];
            System.Array.Copy(this.value, 0, newValue, 0, this.value.Length);
            System.Array.Copy(bytes, 0, newValue, this.value.Length, bytes.Length);
            value = newValue;
        }

        /// <summary>
        /// Appends an octet string.
        /// </summary>
        /// <param name="octetString">An <see cref="OctetString"/> to append to this one</param>
        public void Append(OctetString octetString)
        {
            this.Append(octetString.GetValue());
        }

        /**
         * 
         * @param string
         *    a String instance.
         */
        /// <summary>
        /// Appends the supplied string to this <c>OctetString</c>. Calling this
        /// method is identical to <see cref="Append(Encoding.UTF8.GetBytes(str))"/>.
        /// </summary>
        /// <param name="str">The string to append</param>
        public void Append(string str)
        {
            Append(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Sets the value of the octet string to a zero length string.
        /// </summary>
        public void Clear()
        {
            this.value = new byte[0];
        }

        /// <summary>
        /// Encodes the <see cref="OctetString"/> to a BER stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeString(outputStream, BER.OCTETSTRING, this.GetValue());
        }

        /// <summary>
        /// Decodes the value of the <see cref="OctetString"/> from a <see cref="BERInputStream"/>
        /// </summary>
        /// <param name="inputStream">The stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            byte[] v = BER.DecodeString(inputStream, out byte type);
            if (type != BER.OCTETSTRING)
            {
                throw new IOException("Wrong type encountered when decoding OctetString: " +
                                      type);
            }

            this.SetValue(v);
        }

        /// <summary>
        /// Gets the length of the BER encoding of the <see cref="OctetString"/>
        /// </summary>
        public override int BERLength
        {
            get
            {
                return this.value.Length + BER.GetBERLengthOfLength(this.value.Length) + 1;
            }

        }

        /// <summary>
        /// Gets the syntax of this variable
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxOctetString;
            }
        }

        /// <summary>
        /// Gets or sets the byte at the specified index.
        /// </summary>
        /// <param name="index">a zero-based index into the octet string.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>index</c> &lt; 0 or &gt; <see cref="Length"/></exception>
        public byte this[int index]
        {
            get
            {
                return this.value[index];
            }

            set
            {
                this.value[index] = value;
            }
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < this.value.Length; i++)
            {
                hash += this.value[i] * 31 ^ ((this.value.Length - 1) - i);
            }

            return hash;
        }

        /// <summary>
        /// Check whether this object is equal to another
        /// </summary>
        /// <param name="o">The object to compare to</param>
        /// <returns>True if the two objects are identical <see cref="OctetString"/>s, false if not</returns>
        public override bool Equals(object o)
        {
            return (o is OctetString os) ? this.Equals(os) : false;
        }

        /// <summary>
        /// Check whether this object is equal to another
        /// </summary>
        /// <param name="o">The object to compare to</param>
        /// <returns>True if the two objects are identical <see cref="OctetString"/>s, false if not</returns>
        public bool Equals(OctetString other)
        {
            return Enumerable.SequenceEqual(this.value, other.value);
        }

        /**
         * Checks if the value of this OctetString equals the argument.
         * @param v
         *    the byte array to compare with this OctetStrings value member.
         * @return
         *    <code>Arrays.equals(value, (byte[])v)</code>
         * @since 2.0
         */
        public bool EqualsValue(byte[] v)
        {
            return Enumerable.SequenceEqual(this.value, v);
        }

        public override int CompareTo(IVariable o)
        {
            OctetString os = o as OctetString;

            if (os == null)
            {
                throw new ArgumentException(o.GetType().Name + " is not an OctetString");
            }

            int maxlen = Math.Min(this.value.Length, os.value.Length);
            for (int i = 0; i < maxlen; i++)
            {
                if (value[i] != os.value[i])
                {
                    if ((value[i] & 0xFF) < (os.value[i] & 0xFF))
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            return value.Length - os.value.Length;
        }

        /**
         * Returns a new string that is a substring of this string. The substring
         * begins at the specified <code>beginIndex</code> and extends to the
         * character at index <code>endIndex - 1</code>.
         * Thus the length of the substring is <code>endIndex-beginIndex</code>.
         * @param beginIndex
         *    the beginning index, inclusive.
         * @param endIndex
         *    the ending index, exclusive.
         * @return
         *    the specified substring.
         * @since 1.3
         */
        public OctetString Substring(int beginIndex, int endIndex)
        {
            if (beginIndex < 0)
            {
                throw new ArgumentOutOfRangeException("beginIndex", "beginIndex may not be negative.");
            }

            if (endIndex > this.Length)
            {
                throw new ArgumentOutOfRangeException("endIndex", "endIndex may not be greater than the size of the string");
            }

            byte[] substring = new byte[endIndex - beginIndex];
            System.Array.Copy(this.value, beginIndex, substring, 0, substring.Length);
            return new OctetString(substring);
        }

        /**
         * Tests if this octet string starts with the specified prefix.
         * @param prefix
         *    the prefix.
         * @return
         *    <code>true</code> if the bytes of this octet string up to the length
         *    of <code>prefix</code> equal those of <code>prefix</code>.
         * @since 1.2
         */
        public bool StartsWith(OctetString prefix)
        {
            if ((prefix == null) || prefix.Length > this.Length)
            {
                return false;
            }

            for (int i = 0; i < prefix.Length; i++)
            {
                if (prefix[i] != this.value[i])
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * Determines whether this octet string contains non ISO control characters
         * only.
         * @return
         *    <code>false</code> if this octet string contains any ISO control
         *    characters as defined by <code>Character.isISOControl(char)</code>
         *    except if these ISO control characters are all whitespace characters
         *    as defined by <code>Character.isWhitespace(char)</code> and not
         *    <code>'&#92;u001C'</code>-<code>'&#92;u001F'</code>.
         */
        public bool IsPrintable
        {
            get
            {
                foreach (byte aValue in value)
                {
                    char c = (char)aValue;
                    if ((char.IsControl(c) || ((c & 0xFF) >= 0x80)) &&
                        ((!char.IsWhiteSpace(c)) ||
                            (((c & 0xFF) >= 0x1C)) && ((c & 0xFF) <= 0x1F)))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override string ToString()
        {
            if (this.IsPrintable)
            {
                return Encoding.UTF8.GetString(this.value);
            }

            return this.ToHexString();
        }

        public string ToHexString()
        {
            return this.ToHexString(OctetString.DefaultHexDelimiter);
        }

        public string ToHexString(char separator)
        {
            return this.ToString(separator, 16);
        }

        public static OctetString FromHexString(String hexString)
        {
            return OctetString.FromHexString(hexString, OctetString.DefaultHexDelimiter);
        }

        public static OctetString FromHexString(String hexString, char delimiter)
        {
            return OctetString.FromString(hexString, delimiter, 16);
        }


        public static OctetString FromString(string str, char delimiter, int radix)
        {
            string[] tokens = str.Split(delimiter);
            byte[] val = tokens.Select(t => (byte)Convert.ToInt32(t, radix)).ToArray();
            return new OctetString(val);
        }

        /**
         * Create an OctetString from a hexadecimal string of 2-byte pairs without
         * delimiter. For example: 08A69E
         * @param hexString
         *    a string of characters a-f,A-F,0-9 with length 2*b, where b is the length
         *    of the string in bytes.
         * @return
         *    an OctetString instance with the length <code>hexString.length()/2</code>.
         * @since 2.1
         */
        public static OctetString FromHexStringPairs(string hexString)
        {
            byte[] val = Enumerable.Range(0, hexString.Count() / 2)
                .Select(h => (byte)Convert.ToInt32(hexString.Substring(h, 2), 16)).ToArray();
            return new OctetString(val);
        }

        /**
         * Creates an OctetString from a string represantation in the specified
         * radix.
         * @param string
         *    the string representation of an octet string.
         * @param radix
         *    the radix of the string represantion.
         * @return
         *    the OctetString instance.
         * @since 1.6
         */
        public static OctetString FromString(string str, int radix)
        {
            int digits = (int)(Math.Round((float)Math.Log(256) / Math.Log(radix)));
            byte[] val = new byte[str.Length / digits];

            for (int n = 0; n < str.Length; n += digits)
            {
                string s = str.Substring(n, n + digits);
                val[n / digits] = (byte)Convert.ToInt32(s, radix);
            }

            return new OctetString(val);
        }

        public string ToString(char separator, int radix)
        {
            int digits = (int)(Math.Round((float)Math.Log(256) / Math.Log(radix)));
            StringBuilder buf = new StringBuilder(this.value.Length * (digits + 1));
            for (int i = 0; i < this.value.Length; i++)
            {
                if (i > 0)
                {
                    buf.Append(separator);
                }

                int v = (this.value[i] & 0xFF);

                string val = Convert.ToString(v, radix);

                for (int j = 0; j < digits - val.Length; j++)
                {
                    buf.Append('0');
                }

                buf.Append(val);
            }

            return buf.ToString();
        }

        /**
         * Returns a string representation of this octet string in the radix
         * specified. There will be no separation characters, but each byte will
         * be represented by <code>round(log(256)/log(radix))</code> digits.
         *
         * @param radix
         *    the radix to use in the string representation.
         * @return
         *    a string representation of this ocetet string in the specified radix.
         * @since 1.6
         */
        public string ToString(int radix)
        {
            int digits = (int)(Math.Round((float)Math.Log(256) / Math.Log(radix)));
            StringBuilder buf = new StringBuilder(value.Length * (digits + 1));
            foreach (byte aValue in value)
            {
                int v = (aValue & 0xFF);
                string val = Convert.ToString(v, radix);
                for (int j = 0; j < digits - val.Length; j++)
                {
                    buf.Append('0');
                }

                buf.Append(val);
            }
            return buf.ToString();
        }


        /**
         * Formats the content into a ASCII string. Non-printable characters are
         * replaced by the supplied placeholder character.
         * @param placeholder
         *    a placeholder character, for example '.'.
         * @return
         *    the contents of this octet string as ASCII formatted string.
         * @since 1.6
         */
        public string ToASCII(char placeholder)
        {
            StringBuilder buf = new StringBuilder(value.Length);
            foreach (byte aValue in value)
            {
                if ((char.IsControl((char)aValue)) ||
                    ((aValue & 0xFF) >= 0x80))
                {
                    buf.Append(placeholder);
                }
                else
                {
                    buf.Append((char)aValue);
                }
            }

            return buf.ToString();
        }

        public void SetValue(string val)
        {
            this.SetValue(Encoding.UTF8.GetBytes(val));
        }

        public void SetValue(byte[] val)
        {
            if (val == null)
            {
                throw new ArgumentException(
                    "OctetString must not be assigned a null value");
            }
            this.value = val;
        }

        public byte[] GetValue()
        {
            return value;
        }

        /**
         * Gets the length of the byte string.
         * @return
         *    an integer >= 0.
         */
        public int Length
        {
            get
            {
                return value.Length;
            }
        }

        public override object Clone()
        {
            return new OctetString(value);
        }

        /**
         * Returns the length of the payload of this <code>BERSerializable</code>
         * object in bytes when encoded according to the Basic Encoding Rules (BER).
         *
         * @return the BER encoded length of this variable.
         */
        public override int BERPayloadLength
        {
            get
            {
                return this.value.Length;
            }
        }

        public override int IntValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long LongValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /**
         * Returns a copy of this OctetString where each bit not set in the supplied
         * mask zeros the corresponding bit in the returned OctetString.
         * @param mask
         *    a mask where the n-th bit corresponds to the n-th bit in the returned
         *    OctetString.
         * @return
         *    the masked OctetString.
         * @since 1.7
         */
        public OctetString Mask(OctetString mask)
        {
            byte[] masked = new byte[this.value.Length];
            System.Array.Copy(this.value, 0, masked, 0, this.value.Length);

            for (int i = 0; (i < mask.Length) && (i < masked.Length); i++)
            {
                masked[i] = (byte)(masked[i] & mask[i]);
            }

            return new OctetString(masked);
        }

        /// <summary>
        /// Converts the value of this <c>Variable</c> to a (sub-)index value
        /// </summary>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. { @link OctetString}
        /// and <see cref="OID"/>. For other variables it has no effect.
        /// </param>
        /// <returns>An OID that represents this value as a sub index</returns>
        /// <exception cref="NotSupportedException">If this variable cannot be used as a sub index</exception>
        public override OID ToSubIndex(bool impliedLength)
        {
            long[] subIndex;
            int offset = 0;

            if (!impliedLength)
            {
                subIndex = new long[this.Length + 1];
                subIndex[offset++] = this.Length;
            }
            else
            {
                subIndex = new long[this.Length];
            }

            for (int i = 0; i < this.Length; i++)
            {
                subIndex[offset + i] = this[i] & 0xFF;
            }

            return new OID(subIndex);
        }

        /// <summary>
        /// Sets the value of this <c>IVariable</c> from the supplied (sub-)index.
        /// </summary>
        /// <param name="subIndex">The sub-index OID</param>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. <see cref="OctetString"/>
        /// and <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <exception cref="NotSupportedException">If this variable cannot be set from a sub index</exception>
        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            if (impliedLength)
            {
                this.SetValue(subIndex.ByteArrayValue);
            }
            else
            {
                OID suffix = new OID(subIndex.GetValue(), 1, subIndex.Size - 1);
                this.SetValue(suffix.ByteArrayValue);
            }
        }

        /**
         * Splits an <code>OctetString</code> using a set of delimiter characters
         * similar to how a StringTokenizer would do it.
         * @param octetString
         *    the input string to tokenize.
         * @param delimOctets
         *    a set of delimiter octets.
         * @return
         *    a Collection of OctetString instances that contain the tokens.
         */
        public static IEnumerable<OctetString> Split(OctetString octetString,
                                                    OctetString delimOctets)
        {
            List<OctetString> parts = new List<OctetString>();
            int maxDelim = -1;

            for (int i = 0; i < delimOctets.Length; i++)
            {
                int delim = delimOctets[i] & 0xFF;
                if (delim > maxDelim)
                {
                    maxDelim = delim;
                }
            }

            int startPos = 0;
            for (int i = 0; i < octetString.Length; i++)
            {
                int c = octetString.value[i] & 0xFF;
                bool isDelim = false;
                if (c <= maxDelim)
                {
                    for (int j = 0; j < delimOctets.Length; j++)
                    {
                        if (c == (delimOctets[j] & 0xFF))
                        {
                            if ((startPos >= 0) && (i > startPos))
                            {
                                parts.Add(new OctetString(octetString.value,
                                                          startPos, i - startPos));
                            }

                            startPos = -1;
                            isDelim = true;
                        }
                    }
                }

                if (!isDelim && (startPos < 0))
                {
                    startPos = i;
                }
            }

            if (startPos >= 0)
            {
                parts.Add(new OctetString(octetString.value, startPos,
                                          octetString.Length - startPos));
            }

            return parts;
        }

        /**
         * Creates an <code>OctetString</code> from an byte array.
         * @param value
         *    a byte array that is copied into the value of the created
         *     <code>OctetString</code> or <code>null</code>.
         * @return
         *    an OctetString or <code>null</code> if <code>value</code>
         *    is <code>null</code>.
         * @since 1.7
         */
        public static OctetString FromByteArray(byte[] value)
        {
            if (value == null)
            {
                return null;
            }

            return new OctetString(value);
        }

        public byte[] ToByteArray()
        {
            return GetValue();
        }
    }
}
