// <copyright file="OID.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using JunoSnmp.ASN1;
    using JunoSnmp.Util;

    /// <summary><para>
    /// The Object Identifier Class.
    /// </para><para>
    /// The Object Identifier(OID) class is the encapsulation of an
    /// SMI object identifier.The SMI object is a data identifier for a
    /// data element found in a Management Information Base (MIB), as
    /// defined by a MIB definition.The<code>OID</code> class allows definition and
    /// manipulation of object identifiers.
    /// </para></summary>
    public class OID : AbstractVariable, IAssignableFrom<string>, IAssignableFrom<long[]>, IEquatable<OID>
    {
        public static readonly int MaxOIDLen = 128;
        public static readonly long MaxSubIDValue = 0xFFFFFFFF;

        private static readonly long[] NullOID = new long[0];

        private static IOIDTextFormat oidTextFormat = new SimpleOIDTextFormat();

        private long[] value = NullOID;

        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class with zero length
        /// </summary>
        public OID()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class from a dotted string.
        /// The string can contain embedded strings enclosed by a single quote(') that are converted to
        /// the corresponding OIO value. For example the following OID pairs are equal:
        ///     OID a = new OID("1.3.6.2.1.5.'hallo'.1");
        ///     OID b = new OID("1.3.6.2.1.5.104.97.108.108.111.1");
        ///     assertEquals(a, b);
        ///     a = new OID("1.3.6.2.1.5.'hal.lo'.1");
        ///     b = new OID("1.3.6.2.1.5.104.97.108.46.108.111.1");
        ///     assertEquals(a, b);
        ///     a = new OID("1.3.6.2.1.5.'hal.'.'''.'lo'.1");
        ///     b = new OID("1.3.6.2.1.5.104.97.108.46.39.108.111.1");
        /// </summary>
        /// <param name="oid">
        /// A dotted OID String, for example "1.3.6.1.2.2.1.0"
        /// </param>
        public OID(string oid)
        {
            this.value = OID.ParseDottedString(oid);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class from an array of integer values.
        /// </summary>
        /// <param name="rawOID">
        /// an array of <code>int</code> values. The array
        /// is copied.Later changes to<code> rawOID</code> will therefore not
        /// affect the OID's value.
        /// </param>
        public OID(long[] rawOID) : this(rawOID, 0, rawOID.Length)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class from two arrays 
        /// of long values where the first represents the OID prefix
        /// (i.e., the object class ID) and the second one represents the 
        /// OID suffix(i.e., the instance identifier).
        /// </summary>
        /// <param name="prefixOID">
        /// An array of <c>long</c> values. The array
        /// is copied.Later changes to <c>prefixOID</c> will therefore not
        /// affect the OID's value.
        /// </param>
        /// <param name="suffixOID">
        /// an array of <c>long</c> values which will be appended to the
        /// <c>prefixOID</c> OID.The array is copied.Later changes to
        /// <c>suffixOID</c> will therefore not affect the OID's value.
        /// </param>
        public OID(long[] prefixOID, long[] suffixOID)
        {
            this.value = new long[prefixOID.Length + suffixOID.Length];
            System.Array.Copy(prefixOID, 0, this.value, 0, prefixOID.Length);
            System.Array.Copy(suffixOID, 0, this.value, prefixOID.Length, suffixOID.Length);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class from two arrays 
        /// of long values where the first represents the OID prefix
        /// (i.e., the object class ID) and the second one represents the OID 
        /// suffix(i.e., the instance identifier).
        /// </summary>
        /// <param name="prefixOID">
        /// an array of <c>long</c> values. The array
        /// is copied.Later changes to <c>prefixOID</c> will therefore not
        /// affect the OID's value.
        /// </param>
        /// <param name="suffixID">
        /// A <c>long</c> value which will be appended to the
        /// <c>prefixOID</c> OID.
        /// </param>
        public OID(long[] prefixOID, long suffixID)
        {
            this.value = new long[prefixOID.Length + 1];
            System.Array.Copy(prefixOID, 0, this.value, 0, prefixOID.Length);
            this.value[prefixOID.Length] = suffixID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class from an array of long values
        /// </summary>
        /// <param name="rawOID">
        /// An array of <c>long</c> values. The array is copied. Later changes to <c>rawOID</c>
        /// will therefore not affect the OID's value
        /// </param>
        /// <param name="offset">The zero-based offset into the <c>rawOID</c> array that points to
        /// the first sub-identifier of the new OID
        /// </param>
        /// <param name="length">
        /// The length of the new OID, where <c>offset + length</c> must be less than or equal to
        /// the length of <c>rawOID</c>. If not, an ArgumentOutOfRange exception is thrown
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>offset + length</c> is greater than the size of <c>rawOID</c>
        /// </exception>
        public OID(long[] rawOID, int offset, int length)
        {
            this.SetValue(rawOID, offset, length);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OID"/> class by copying the value of another OID
        /// </summary>
        /// <param name="other">The OID to copy</param>
        public OID(OID other) : this(other.GetValue())
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <code>OID</code> can be BER encoded.
        /// </summary>
        /// <remarks>
        /// Value is true of Size &gt;= 2 and Size &lt;= 128 and if the first two sub identifiers
        /// are less than 3 and 40 respectively
        /// </remarks>
        public bool IsValid
        {
            get
            {
                return (this.Size >= 2) && (this.Size <= 128) &&
                    ((this.value[0] & 0xFFFFFFFFL) <= 2L) &&
                    ((this.value[1] & 0xFFFFFFFFL) < 40L);
            }
        }

        /// <summary>
        /// Gets the number of sub-identifiers in this <code>OID</code>.
        /// </summary>
        public int Size
        {
            get
            {
                return this.value.Length;
            }
        }

        /// <summary>
        /// Gets an integer representation of this Variable
        /// </summary>
        /// <remarks>Not supported by <see cref="OID"/></remarks>
        public override int IntValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets an long representation of this Variable
        /// </summary>
        /// <remarks>Not supported by <see cref="OID"/></remarks>
        public override long LongValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the syntax for this variable
        /// </summary>
        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxObjectIdentifier;
            }
        }

        /// <summary>
        /// Gets the length of the BER encoding of this object
        /// </summary>
        public override int BERLength
        {
            get
            {
                int length = BER.GetOIDLength(this.value);
                return length + BER.GetBERLengthOfLength(length) + 1;
            }
        }

        /// <summary>
        /// Gets the successor OID for this OID, i.e. an OID clone of this OID with a zero 
        /// sub-identifier appended.
        /// </summary>
        public OID Successor
        {
            get
            {
                if (this.value.Length == MaxOIDLen)
                {
                    for (int i = MaxOIDLen - 1; i >= 0; i--)
                    {
                        if (this.value[i] != MaxSubIDValue)
                        {
                            long[] succ = new long[i + 1];
                            System.Array.Copy(this.value, 0, succ, 0, i + 1);
                            succ[i]++;
                            return new OID(succ);
                        }
                    }

                    return new OID();
                }
                else
                {
                    long[] succ = new long[this.value.Length + 1];
                    System.Array.Copy(this.value, 0, succ, 0, this.value.Length);
                    succ[this.value.Length] = 0;
                    return new OID(succ);
                }
            }
        }

        /// <summary>
        /// Gets the predecessor OID for this OID.
        /// </summary>
        /// <remarks>
        /// if this OID ends on 0, then a <see cref="MaxOIDLen"/>
        /// sub-identifier OID is returned where each sub-ID for index greater
        /// or equal to <see cref="Size"/> is set to <see cref="MaxSubIDValue"/>.
        /// </remarks>
        public OID Predecessor
        {
            get
            {
                if (this.Last != 0)
                {
                    long[] pval = new long[MaxOIDLen];
                    System.Array.Copy(this.value, 0, pval, 0, this.value.Length);
                    ////Arrays.fill(pval, value.Length, pval.Length, MAX_SUBID_VALUE);
                    pval.Populate<long>(MaxSubIDValue, this.value.Length, pval.Length - this.value.Length);
                    OID pred = new OID(pval);
                    pred[this.Size - 1] = this.Last - 1;
                    return pred;
                }
                else
                {
                    OID pred = new OID(this);
                    pred.RemoveLast();
                    return pred;
                }
            }
        }

        /// <summary>
        /// Gets the next following OID with the same or lesser size (length).
        /// This is the next OID on the same or upper level or a clone of this OID, if
        /// it has a zero length or is 2^32-1.
        /// </summary>
        public OID NextPeer
        {
            get
            {
                OID next = new OID(this);
                if ((next.Size > 0) && (this.Last != MaxSubIDValue))
                {
                    next[next.Size - 1] = this.Last + 1;
                }
                else if (next.Size > 1)
                {
                    next.Trim(1);
                    next = this.NextPeer;
                }

                return next;
            }
        }
        
        /// <summary>
        /// Gets the last sub-identifier as a long value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the OID is empty</exception>
        public long Last
        {
            get
            {
                if (this.value.Length > 0)
                {
                    return this.value[this.value.Length - 1];
                }

                throw new ArgumentOutOfRangeException("OID is empty");
            }
        }

        /// <summary>
        /// Gets the OID as dotted string (e.g., "1.3.6.1.4.1") regardless of what
        /// <see cref="IOIDTextFormat"/> instance is set in <see cref="JunoSnmpSettings"/>.
        /// </summary>
        public string DottedStringValue
        {
            get
            {
                return OID.oidTextFormat.Format(this.value);
            }
        }
        
        /// <summary>
        /// Gets the content of the as a byte array. This method can be used
        /// to convert an index value to an <c>OctetString</c> or
        /// <c>IpAddress</c> instance.
        /// </summary>
        /// <remarks>
        /// The caller must take care to check that values in the array are
        /// appropriate for casting to byte (i.e. smaller than 256). Any values
        /// &gt;= 256 will be truncated when down-cast to byte
        /// </remarks>
        public byte[] ByteArrayValue
        {
            get
            {
                byte[] b = new byte[this.value.Length];

                for (int i = 0; i < this.value.Length; i++)
                {
                    b[i] = (byte)(this.value[i] & 0xFF);
                }

                return b;
            }
        }

        /// <summary>
        /// Gets or sets the sub identifier value at the specified position.
        /// </summary>
        /// <param name="index">The index of the sub identifier to get or set</param>
        /// <returns>
        /// The sub identifier value at <c>index</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is out of range</exception>
        public long this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Size)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return this.value[index];
            }

            set
            {
                if (index < 0 || index >= this.Size)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.value[index] = value;
            }
        }

        /// <summary>
        /// Returns the greater of the two OID values.
        /// </summary>
        /// <param name="a">An OID</param>
        /// <param name="b">A second OID</param>
        /// <returns><c>a</c> if a &gt;= b, <c>b</c> if not.</returns>
        public static OID Max(OID a, OID b)
        {
            if (a.CompareTo(b) >= 0)
            {
                return a;
            }

            return b;
        }

        /// <summary>
        /// Returns the lesser of two OID values.
        /// </summary>
        /// <param name="a">An OID</param>
        /// <param name="b">A second OID</param>
        /// <returns><c>a</c> if a &lt;= b, <c>b</c> if not.</returns>
        public static OID Min(OID a, OID b)
        {
            if (a.CompareTo(b) <= 0)
            {
                return a;
            }

            return b;
        }

        /// <summary>
        /// Get a hash code for this object
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode()
        {
            long hash = 0;

            for (int i = 0; i < this.value.Length; i++)
            {
                hash += this.value[i] * 31 ^ ((this.value.Length - 1) - i);
            }

            return (int)hash;
        }

        /// <summary>
        /// Test this OID for equality with another object. Two OIDs are equal if
        /// they are of equal length and all of their values are equal.
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>True if the other object is an identical OID, false if not.</returns>
        public override bool Equals(object o)
        {
            return (o is OID oi) ? this.Equals(oi) : false;
        }

        /// <summary>
        /// Test this OID for equality with another object. Two OIDs are equal if
        /// they are of equal length and all of their values are equal.
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>True if the other object is an identical OID, false if not.</returns>
        public bool Equals(OID o)
        {
            if (o.value.Length != this.value.Length)
            {
                return false;
            }

            for (int i = 0; i < this.value.Length; i++)
            {
                if (this.value[i] != o.value[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compares this OID to another IVariable
        /// </summary>
        /// <param name="o">Another IVariable</param>
        /// <returns>0 if the two OIDs are equal, the result of a left-side comparison if not</returns>
        /// <exception cref="ArgumentException">If the other IVariable is not an OID</exception>
        public override int CompareTo(IVariable o)
        {
            OID other = o as OID;

            if (other == null)
            {
                throw new ArgumentException("Variable is not an OID: " + o.ToString());
            }

            int min = Math.Min(this.value.Length, other.value.Length);
            int result = this.LeftMostCompare(min, other);

            if (result == 0)
            {
                return this.value.Length - other.value.Length;
            }

            return result;
        }

        /// <summary>
        /// Returns a copy of this OID where sub-identifiers have been set to zero
        /// for all <c>n-th</c> sub-identifier where the <c>n-th</c> bit of mask is zero.
        /// </summary>
        /// <param name="mask">
        /// A mask where the <c>n-th</c> bit corresponds to the <c>n-th</c> sub-identifier.
        /// </param>
        /// <returns>The masked OID</returns>
        public OID Mask(OctetString mask)
        {
            long[] masked = new long[this.value.Length];

            System.Array.Copy(this.value, 0, masked, 0, this.value.Length);

            for (int i = 0; (i < mask.Length * 8) && (i < masked.Length); i++)
            {
                byte b = (byte)(0x80 >> (i % 8));
                if ((mask[i / 8] & b) == 0)
                {
                    masked[i] = 0;
                }
            }

            return new OID(masked);
        }

        /// <summary>
        /// Return a string representation of this OID that can be parsed again to an identical
        /// <see cref="OID"/> through <see cref="OID.OID(string)"/>
        /// </summary>
        /// <returns>
        /// A formatted string representation of this OID (e.g. <c>"ifDescr.1"</c>) that
        /// can be parsed again as defined by
        /// <see cref="IOIDTextFormat.FormatForRoundTrip(long[])"/>
        /// in <see cref="JunoSnmpSettings"/>
        /// </returns>
        public override string ToString()
        {
            return JunoSnmpSettings.OIDTextFormat.FormatForRoundTrip(this.value);
        }
        
        /// <summary>
        /// Format the OID as text. This could return to same result as <see cref="ToString"/>
        /// but also fully converted index-to-text values like
        /// <c>MessageDispatcherImpl</c>
        /// </summary>
        /// <returns>The OID formatted as text</returns>
        public string Format()
        {
            return JunoSnmpSettings.OIDTextFormat.Format(this.value);
        }
       
        /// <summary>
        /// Encodes this <see cref="OID"/> to a BER stream
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeOID(outputStream, BER.OID, this.value);
        }

        /// <summary>
        /// Decodes an OID from an input stream
        /// </summary>
        /// <param name="inputStream">The stream to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            long[] v = BER.DecodeOID(inputStream, out byte type);
            if (type != BER.OID)
            {
                throw new IOException("Wrong type encountered when decoding OID: " +
                                      type);
            }

            this.SetValue(v);
        }

        /// <summary>
        /// Sets the value of this OID from a given dotted string representation
        /// </summary>
        /// <param name="str">A string containing an OID in dotted-string format</param>
        public void SetValue(string str)
        {
            this.value = OID.ParseDottedString(str);
        }
        
        /// <summary>
        /// Sets the value from an array of long values.
        /// </summary>
        /// <param name="arr">The new value as an array of longs representing the sub identifiers</param>
        /// <exception cref="ArgumentNullException">If the value is null</exception>
        public void SetValue(long[] arr)
        {
            if (arr == null)
            {
                throw new ArgumentNullException("OID value must not be set to null");
            }

            this.value = arr;
        }

        /// <summary>
        /// Gets all sub-identifiers as an array of long.
        /// </summary>
        /// <returns>An array of longs representing the sub identifiers</returns>
        public long[] GetValue()
        {
            return this.value;
        }
        
        /// <summary>
        /// Appends a dotted String OID to this <c>OID</c>.
        /// </summary>
        /// <param name="oid">A dotted string with numerical sub identifiers</param>
        /// <returns>This OID with the string appended to it</returns>
        public OID Append(string oid)
        {
            OID suffix = new OID(oid);
            return this.Append(suffix);
        }

        /// <summary>
        /// Appends a dotted String OID to this <c>OID</c>.
        /// </summary>
        /// <param name="oid">An OID to append to this one</param>
        /// <returns>This OID with the string appended to it</returns>
        public OID Append(OID oid)
        {
            long[] newValue = new long[this.value.Length + oid.value.Length];
            System.Array.Copy(this.value, 0, newValue, 0, this.value.Length);
            System.Array.Copy(oid.value, 0, newValue, this.value.Length, oid.value.Length);
            this.value = newValue;
            return this;
        }
        
        /// <summary>
        /// Appends a single sub-identifier to this OID.
        /// </summary>
        /// <param name="subID">A long value representing a sub identifier</param>
        /// <returns>This OID with the string appended to it</returns>
        public OID Append(long subID)
        {
            long[] newValue = new long[this.value.Length + 1];
            System.Array.Copy(this.value, 0, newValue, 0, this.value.Length);
            newValue[this.value.Length] = subID;
            this.value = newValue;
            return this;
        }
        
        /// <summary>
        /// Compares the n leftmost sub-identifiers with the given <code>OID</code>
        /// in left-to-right direction.
        /// </summary>
        /// <param name="n">The number of sub identifiers to compare</param>
        /// <param name="other">An <c>OID</c> to compare against</param>
        /// <returns>
        /// 0 if the first <c>n</c> sub identifiers are the same, a negative value if the first
        /// <c>n</c> sub identifiers of this <see cref="OID"/> are lexicographically smaller than 
        /// those of <c>other</c>, a positive value if not.
        /// </returns>
        public int LeftMostCompare(int n, OID other)
        {
            for (int i = 0; i < n && i < this.value.Length && i < other.Size; i++)
            {
                if (this.value[i] != other.value[i])
                {
                    if ((this.value[i] & 0xFFFFFFFFL) <
                        (other.value[i] & 0xFFFFFFFFL))
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            if (n > this.value.Length)
            {
                return -1;
            }
            else if (n > other.Size)
            {
                return 1;
            }

            return 0;
        }
        
        /// <summary>
        /// Compares the n rightmost sub-identifiers in direction right-to-left
        /// with those of the given<c> OID</c>.
        /// </summary>
        /// <param name="n">The number of sub identifiers to compare</param>
        /// <param name="other">An <c>OID</c> to compare against</param>
        /// <returns>
        /// 0 if the last <c>n</c> sub identifiers are the same, a negative value if the last
        /// <c>n</c> sub identifiers of this <see cref="OID"/> are lexicographically smaller (starting
        /// with the last one and working left) than those of <c>other</c>, a positive value if not.
        /// </returns>
        public int RightMostCompare(int n, OID other)
        {
            int cursorA = this.value.Length - 1;
            int cursorB = other.value.Length - 1;
            for (int i = n - 1; i >= 0; i--, cursorA--, cursorB--)
            {
                if (this.value[cursorA] != other.value[cursorB])
                {
                    if (this.value[cursorA] < other.value[cursorB])
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }
        
        /// <summary>
        /// Check if the given OID is a prefix of this OID.
        /// </summary>
        /// <param name="other">The OID to compare against</param>
        /// <returns>
        /// True if the first <c>other.Length</c> sub identifiers in both OIDs are equal.
        /// False in all other cases
        /// </returns>
        public bool StartsWith(OID other)
        {
            if (other.value.Length > this.value.Length)
            {
                return false;
            }

            int min = Math.Min(this.value.Length, other.value.Length);
            return this.LeftMostCompare(min, other) == 0;
        }

        /// <summary>
        /// Creates a duplicate of this OID
        /// </summary>
        /// <returns>A new OID with the same value as this one</returns>
        public override object Clone()
        {
            return new OID(this.value);
        }
        
        /// <summary>
        /// Removes the last sub-identifier (if available) from this <c>OID</c>
        /// and returns it.
        /// </summary>
        /// <returns>
        /// The last sub identifier or -1 if there is no sub identifier left in this <see cref="OID"/>
        /// </returns>
        public long RemoveLast()
        {
            if (this.value.Length == 0)
            {
                return -1;
            }

            long[] newValue = new long[this.value.Length - 1];
            System.Array.Copy(this.value, 0, newValue, 0, this.value.Length - 1);
            long retValue = this.value[this.value.Length - 1];
            this.value = newValue;
            return retValue;
        }
        
        /// <summary>
        /// Remove the n rightmost sub identifiers from this OID.
        /// </summary>
        /// <param name="n">
        /// the number of sub identifiers to remove. If <c>n</c> is zero or
        /// negative then this OID will not be changed.If <c>n</c> is greater
        /// than <see cref="Size"/> all sub identifiers will be removed from this OID.
        /// </param>
        public void Trim(int n)
        {
            if (n > 0)
            {
                if (n > this.value.Length)
                {
                    n = this.value.Length;
                }

                long[] newValue = new long[this.value.Length - n];
                System.Array.Copy(this.value, 0, newValue, 0, this.value.Length - n);
                this.value = newValue;
            }
        }

        /// <summary>
        /// Returns a new copy of this OID with the last sub identifier removed.
        /// </summary>
        /// <returns>
        /// A copy of this OID with <code>n-1</code> sub-identifiers where
        /// <c>n</c> is the size of this OID and greater than zero, otherwise
        /// a zero length OID is returned.
        /// </returns>
        public OID Trim()
        {
            return new OID(this.value, 0, Math.Max(this.value.Length - 1, 0));
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
            if (impliedLength)
            {
                return new OID(this.value);
            }

            OID subIndex = new OID(new long[] { this.Size });
            subIndex.Append(this);
            return subIndex;
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
            int offset = 1;
            if (impliedLength)
            {
                offset = 0;
            }

            this.SetValue(subIndex.GetValue(), offset, subIndex.Size - offset);
        }
        
        /// <summary>
        /// Parses a textual representation of an object ID and returns its raw value.
        /// </summary>
        /// <param name="oid">A textual representation of an OID</param>
        /// <returns>The raw OID value</returns>
        /// <exception cref="ParseException">If the OID cannot be parsed successfully</exception>
        private static long[] ParseDottedString(string oid)
        {
            return JunoSnmpSettings.OIDTextFormat.Parse(oid);
        }

        /// <summary>
        /// Sets the value from an array of long values.
        /// </summary>
        /// <param name="rawOID">The new value as an array of longs representing the sub identifiers</param>
        /// <param name="offset">The offset at which to start reading values in <c>rawOID</c></param>
        /// <param name="length">The number of elements to read from <c>rawOID</c></param>
        /// <exception cref="ArgumentNullException">If the value is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the length of the array is less than offset + length
        /// </exception>
        private void SetValue(long[] rawOID, int offset, int length)
        {
            if (rawOID == null)
            {
                throw new ArgumentNullException();
            }

            if (rawOID.Length < offset + length)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.value = new long[length];
            System.Array.Copy(rawOID, offset, this.value, 0, length);
        }
    }
}
