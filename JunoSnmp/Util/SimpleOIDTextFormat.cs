// <copyright file="SimpleOIDTextFormat.cs" company="None">
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
//    Original Java code from Snmp4J Copyright (C) 2003-2016 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Util
{
    using System;
    using System.Text;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>SimpleOIDTextFormat</code> implements a simple textual
    /// representation for object IDs as dotted string.
    /// </summary>
    public class SimpleOIDTextFormat : OIDTextFormat
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleOIDTextFormat"/> class.
        /// </summary>
        public SimpleOIDTextFormat()
        {
        }
        
        /// <summary>
        /// Returns a textual representation of a raw object ID as dotted
        /// string ("1.3.6.1.4").
        /// </summary>
        /// <param name="value">The OID value to format</param>
        /// <returns>The textual representation</returns>
        public static string FormatOID(long[] value)
        {
            StringBuilder buf = new StringBuilder(3 * value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                if (i != 0)
                {
                    buf.Append('.');
                }

                buf.Append((value[i] & 0xFFFFFFFFL));
            }

            return buf.ToString();
        }
        
        /// <summary>
        /// Returns a textual representation of a raw object ID as dotted
        /// string ("1.3.6.1.4").
        /// </summary>
        /// <param name="value">The OID value to format</param>
        /// <returns>The textual representation</returns>
        public string Format(long[] value)
        {
            return SimpleOIDTextFormat.FormatOID(value);
        }

        /// <summary>
        /// Returns a textual representation of a raw object ID as dotted
        /// string ("1.3.6.1.4"). This method is the same as {@link #format(int[])}.
        /// </summary>
        /// <param name="value">The OID value to format</param>
        /// <returns>The textual representationn</returns>
        public string FormatForRoundTrip(long[] value)
        {
            return this.Format(value);
        }

        /// <summary>
        /// Parses a textual representation of an object ID as dotted string
        /// (e.g. "1.3.6.1.2.1.1") and returns its raw value.
        /// </summary>
        /// <param name="text">A textual representation of an OID</param>
        /// <returns>The raw OID value</returns>
        /// <exception cref="ParseException">If the OID cannot be parsed successfully</exception>
        public static long[] ParseOID(string text)
        {
            string[] subIDs = text.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            long[] value = new long[subIDs.Length];
            int size = 0;
            StringBuilder buf = null;

            foreach (string s in subIDs)
            {
                string t;
                if ((buf == null) && s.StartsWith("'"))
                {
                    buf = new StringBuilder();
                    t = s.Substring(1);
                }
                else
                {
                    t = s;
                }

                if ((buf != null) && (t.EndsWith("'")))
                {
                    buf.Append(t.Substring(0, t.Length - 1));
                    OID o = new OctetString(buf.ToString()).ToSubIndex(true);
                    long[] h = value;
                    value = new long[subIDs.Length + h.Length + o.Size];
                    System.Array.Copy(h, 0, value, 0, size);
                    System.Array.Copy(o.GetValue(), 0, value, size, o.Size);
                    size += o.Size;
                    buf = null;
                }
                else if (buf != null)
                {
                    buf.Append(t);
                }
                else if (!".".Equals(t))
                {
                    value[size++] = long.Parse(t.Trim());
                }
            }

            if (size < value.Length)
            {
                long[] h = value;
                value = new long[size];
                System.Array.Copy(h, 0, value, 0, size);
            }

            return value;
        }
        
        /// <summary>
        /// Parses a textual representation of an object ID as dotted string
        /// (e.g. "1.3.6.1.2.1.1") and returns its raw value.
        /// </summary>
        /// <param name="text">A textual representation of an OID</param>
        /// <returns>The raw OID value</returns>
        /// <exception cref="ParseException">If the OID cannot be parsed successfully</exception>
        public long[] Parse(string text)
        {
            return SimpleOIDTextFormat.ParseOID(text);
        }
    }
}
