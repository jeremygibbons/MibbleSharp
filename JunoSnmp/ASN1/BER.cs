// <copyright file="BER.cs" company="None">
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

namespace JunoSnmp.ASN1
{
    using System;
    using System.IO;

    /// <summary>
    /// The BER class provides utility methods for BER encoding and decoding.
    /// </summary>
    public class BER
    {
        public const byte Asn1Boolean = 0x01;
        public const byte Asn1Integer = 0x02;
        public const byte Asn1BitString = 0x03;
        public const byte Asn1OctetString = 0x04;
        public const byte Asn1Null = 0x05;
        public const byte Asn1ObjectIdentifier = 0x06;
        public const byte Asn1Sequence = 0x10;
        public const byte Asn1Set = 0x11;
        public const byte Asn1Universal = 0x00;
        public const byte Asn1Application = 0x40;
        public const byte Asn1Context = (byte)0x80;
        public const byte Asn1Private = (byte)0xC0;
        public const byte Asn1Primitive = (byte)0x00;
        public const byte Asn1Constructor = (byte)0x20;

        public const byte Asn1LongLength = (byte)0x80;
        public const byte Asn1ExtensionId = (byte)0x1F;
        public const byte Asn1Bit8 = (byte)0x80;

        public const byte INTEGER = Asn1Universal | 0x02;
        public const byte INTEGER32 = Asn1Universal | 0x02;
        public const byte BITSTRING = Asn1Universal | 0x03;
        public const byte OCTETSTRING = Asn1Universal | 0x04;
        public const byte NULL = Asn1Universal | 0x05;
        public const byte OID = Asn1Universal | 0x06;
        public const byte SEQUENCE = Asn1Constructor | 0x10;

        public const byte IPADDRESS = Asn1Application | 0x00;
        public const byte COUNTER = Asn1Application | 0x01;
        public const byte COUNTER32 = Asn1Application | 0x01;
        public const byte GAUGE = Asn1Application | 0x02;
        public const byte GAUGE32 = Asn1Application | 0x02;
        public const byte TIMETICKS = Asn1Application | 0x03;
        public const byte OPAQUE = Asn1Application | 0x04;
        public const byte COUNTER64 = Asn1Application | 0x06;

        public const int NoSuchObject = 0x80;
        public const int NoSuchInstance = 0x81;
        public const int EndOfMibView = 0x82;

        public const int MaskOidLength = 127;
        private const int LenMask = 0x0ff;

        private static bool checkSequenceLengthFlag = true;
        private static bool checkValueLengthFlag = true;

        /// <summary>
        /// Gets or sets a value indicating whether to check if a value's indicated length is
        /// in fact available from the input stream.
        /// </summary>
        public static bool CheckValueLengthFlag
        {
            get
            {
                return BER.checkValueLengthFlag;
            }

            set
            {
                BER.checkValueLengthFlag = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the indicated length of a parsed SEQUENCE should 
        /// be checked against the real length of the parsed objects.
        /// </summary>
        public static bool CheckSequenceLengthFlag
        {
            get
            {
                return BER.checkSequenceLengthFlag;
            }

            set
            {
                BER.checkSequenceLengthFlag = value;
            }
        }

        /// <summary>
        /// Encodes an ASN.1 header for an object with the ID and
        /// length specified.
        /// </summary>
        /// <param name="os">An <see cref="Stream"/> to which the header is written</param>
        /// <param name="type">
        /// The type of the ASN.1 object. It must be smaller than 30, i.e. no extension bytes
        /// </param>
        /// <param name="length">The length of the object. The maximum value is 0xFFFFFFFF</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeHeader(Stream os, int type, int length)
        {
            os.WriteByte((byte)type);
            BER.EncodeLength(os, length);
        }

        /// <summary>
        /// Encodes an ASN.1 header for an object with the ID and
        /// length specified. The number of bytes used to encode the length must
        /// also be supplied
        /// </summary>
        /// <param name="os">The <see cref="Stream"/> to which the header is written</param>
        /// <param name="type">
        /// The type of the ASN.1 object. It must be smaller than 30, i.e. no extension bytes
        /// </param>
        /// <param name="length">The length of the object. The maximum value is 0xFFFFFFFF</param>
        /// <param name="numBytesLength">the number of bytes used to encode the length of the length.</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeHeader(
            Stream os, 
            int type, 
            int length,
            int numBytesLength)
        {
            os.WriteByte((byte)type);
            BER.EncodeLength(os, length, numBytesLength);
        }

        /// <summary>Compute the space needed to encode the length.</summary>
        /// <param name="length">The length to encode</param>
        /// <returns>The count of bytes needed to encode the <c>length</c> value</returns>
        public static int GetBERLengthOfLength(int length)
        {
            if (length < 0)
            {
                return 5;
            }
            else if (length < 0x80)
            {
                return 1;
            }
            else if (length <= 0xFF)
            {
                return 2;
            }
            else if (length <= 0xFFFF)
            { 
                // 0xFF < length <= 0xFFFF
                return 3;
            }
            else if (length <= 0xFFFFFF)
            { 
                // 0xFFFF < length <= 0xFFFFFF
                return 4;
            }

            return 5;
        }
        
        /// <summary>Encodes the length of an ASN.1 object.</summary>
        /// <param name="os">An <see cref="Stream"/> to which the header is written</param>
        /// <param name="length">The length of the object. The maximum value is 0xFFFFFFFF</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeLength(Stream os, int length)
        {
            if (length < 0)
            {
                os.WriteByte(0x04 | Asn1LongLength);
                os.WriteByte((byte)((length >> 24) & 0xFF));
                os.WriteByte((byte)((length >> 16) & 0xFF));
                os.WriteByte((byte)((length >> 8) & 0xFF));
                os.WriteByte((byte)(length & 0xFF));
            }
            else if (length < 0x80)
            {
                os.WriteByte((byte)length);
            }
            else if (length <= 0xFF)
            {
                os.WriteByte(0x01 | Asn1LongLength);
                os.WriteByte((byte)length);
            }
            else if (length <= 0xFFFF)
            { 
                // 0xFF < length <= 0xFFFF
                os.WriteByte(0x02 | Asn1LongLength);
                os.WriteByte((byte)((length >> 8) & 0xFF));
                os.WriteByte((byte)(length & 0xFF));
            }
            else if (length <= 0xFFFFFF)
            { 
                // 0xFFFF < length <= 0xFFFFFF
                os.WriteByte(0x03 | Asn1LongLength);
                os.WriteByte((byte)((length >> 16) & 0xFF));
                os.WriteByte((byte)((length >> 8) & 0xFF));
                os.WriteByte((byte)(length & 0xFF));
            }
            else
            {
                os.WriteByte(0x04 | Asn1LongLength);
                os.WriteByte((byte)((length >> 24) & 0xFF));
                os.WriteByte((byte)((length >> 16) & 0xFF));
                os.WriteByte((byte)((length >> 8) & 0xFF));
                os.WriteByte((byte)(length & 0xFF));
            }
        }
        
        /// <summary>Encodes the length of an ASN.1 object.</summary>
        /// <param name="os">An <see cref="Stream"/> to which the header is written</param>
        /// <param name="length">The length of the object. The maximum value is 0xFFFFFFFF</param>
        /// <param name="numLengthBytes">
        /// The number of bytes to be used to encode the length using the long form.
        /// </param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeLength(Stream os, int length, int numLengthBytes)
        {
            os.WriteByte((byte)(numLengthBytes | Asn1LongLength));

            for (int i = (numLengthBytes - 1) * 8; i >= 0; i -= 8)
            {
                os.WriteByte((byte)((length >> i) & 0xFF));
            }
        }
        
        /// <summary>Encode a signed integer.</summary>
        /// <param name="os">The <see cref="Stream"/> to which the length is written</param>
        /// <param name="type">The tag type for the integer (typically 0x02)</param>
        /// <param name="value">The integer value to encode</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeInteger(Stream os, byte type, int value)
        {
            int integer = value;
            int mask;
            int intsize = 4;

            // Truncate "unnecessary" bytes off of the most significant end of this
            // 2's complement integer.  There should be no sequence of 9
            // consecutive 1's or 0's at the most significant end of the
            // integer.
            mask = 0x1FF << ((8 * 3) - 1);
            
            // mask is 0xFF800000 on a big-endian machine
            while ((((integer & mask) == 0) || ((integer & mask) == mask))
                  && intsize > 1)
            {
                intsize--;
                integer <<= 8;
            }

            BER.EncodeHeader(os, type, intsize);
            mask = 0xFF << (8 * 3);

            // mask is 0xFF000000 on a big-endian machine
            while ((intsize--) > 0)
            {
                os.WriteByte((byte)((integer & mask) >> (8 * 3)));
                integer <<= 8;
            }
        }

        /// <summary>
        /// Encode an unsigned integer. Format: <c>ASN.1 integer ::= 0x02 asnlength byte {byte}*</c>
        /// </summary>
        /// <param name="os">The <see cref="Stream"/> to which the length is written</param>
        /// <param name="type">The tag type for the integer (typically 0x02)</param>
        /// <param name="value">The integer value to encode</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeUnsignedInteger(Stream os, byte type, long value)
        {
            // figure out the len
            int len = 1;
            if (((value >> 24) & LenMask) != 0)
            {
                len = 4;
            }
            else if (((value >> 16) & LenMask) != 0)
            {
                len = 3;
            }
            else if (((value >> 8) & LenMask) != 0)
            {
                len = 2;
            }

            // check for 5 byte len where first byte will be
            // a null
            if (((value >> (8 * (len - 1))) & 0x080) != 0)
            {
                len++;
            }

            // build up the header
            BER.EncodeHeader(os, type, len);  
            
            // length of BER encoded item

            // special case, add a null byte for len of 5
            if (len == 5)
            {
                os.WriteByte(0);
                for (int x = 1; x < len; x++)
                {
                    os.WriteByte((byte)(value >> (8 * (4 - x) & LenMask)));
                }
            }
            else
            {
                for (int x = 0; x < len; x++)
                {
                    os.WriteByte((byte)(value >> (8 * ((len - 1) - x) & LenMask)));
                }
            }
        }
        
        /// <summary>
        /// Encode an ASN.1 octet string filled with the supplied input string.
        /// </summary>
        /// <param name="os">The <see cref="Stream"/> to which the length is written</param>
        /// <param name="type">The tag type for the octet string (typically 0x04)</param>
        /// <param name="str">The byte array containing the Octet String value to encode</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeString(Stream os, byte type, byte[] str)
        {
            // ASN.1 octet string ::= primstring | cmpdstring
            // primstring ::= 0x04 asnlength byte {byte}*
            // cmpdstring ::= 0x24 asnlength string {string}*
            // This code will never send a compound string.
            BER.EncodeHeader(os, type, str.Length);
            os.Write(str, 0, str.Length);
        }

        /// <summary>
        /// Encode an ASN.1 header for a sequence with the ID and length specified.
        /// This only works on data types smaller than 30, i.e. no extension octets. 
        /// The maximum length is 0xFFFF;
        /// </summary>
        /// <param name="os">The <see cref="Stream"/> to which the length is written</param>
        /// <param name="type">The tag type for the sequence (typically 0x10)</param>
        /// <param name="length">The length of the sequence to encode</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeSequence(Stream os, byte type, int length)
        {
            os.WriteByte(type);
            BER.EncodeLength(os, length);
        }

        /// <summary>Gets the payload length in bytes of the BER encoded OID value.</summary>
        /// <param name="value">
        /// An array of unsigned integer values representing an object identifier</param>
        /// <returns>The BER encoded length of the OID without the header and length fields</returns>
        public static int GetOIDLength(long[] value)
        {
            int length = 1;

            if (value.Length > 1)
            {  
                // for first 2 subids, one sub-id is saved by special encoding
                length = BER.GetSubIDLength((value[0] * 40) + value[1]);
            }

            for (int i = 2; i < value.Length; i++)
            {
                length += BER.GetSubIDLength(value[i]);
            }

            return length;
        }

        /// <summary>Encode an ASN.1 oid filled with the supplied oid value.</summary>
        /// <param name="os">An <see cref="Stream"/> to which the encoded OID is written</param>
        /// <param name="type">The tag type for the OID (typically 0x06)</param>
        /// <param name="oid">The <c>int</c> array containing the OID value</param>
        /// <exception cref="IOException">If an error is encountered writing to the stream</exception>
        public static void EncodeOID(Stream os, byte type, long[] oid)
        {
            // ASN.1 objid ::= 0x06 asnlength subidentifier {subidentifier}*
            // subidentifier ::= {leadingbyte}* lastbyte
            // leadingbyte ::= 1 7bitvalue
            // lastbyte ::= 0 7bitvalue   
            BER.EncodeHeader(os, type, BER.GetOIDLength(oid));

            int encodedLength = oid.Length;
            int rpos = 0;

            if (oid.Length < 2)
            {
                os.WriteByte(0);
                encodedLength = 0;
            }
            else
            {
                long firstSubID = oid[0];

                if (firstSubID < 0 || firstSubID > 2)
                {
                    throw new IOException("Invalid first sub-identifier (must be 0, 1, or 2)");
                }

                BER.EncodeSubID(os, oid[1] + (firstSubID * 40));
                encodedLength -= 2;
                rpos = 2;
            }

            while (encodedLength-- > 0)
            {
                BER.EncodeSubID(os, oid[rpos++]);
            }
        }

        /// <summary>
        /// Helper method to encode a 64bit unsigned integer
        /// </summary>
        /// <param name="os">An <see cref="Stream"/> to which the encoded OID is written</param>
        /// <param name="type">The tag type for the OID (typically 0x02)</param>
        /// <param name="value">The integer value to be encoded</param>
        public static void EncodeUnsignedInt64(Stream os, byte type, long value)
        {
            int len;
            
            // Truncate "unnecessary" bytes off of the most significant end of this
            // 2's complement integer.  There should be no sequence of 9
            // consecutive 1's or 0's at the most significant end of the
            // integer.
            for (len = 8; len > 1; len--)
            {
                if (((value >> (8 * (len - 1))) & 0xFF) != 0)
                {
                    break;
                }
            }

            if (((value >> (8 * (len - 1))) & 0x080) != 0)
            {
                len++;
            }

            BER.EncodeHeader(os, type, len);

            if (len == 9)
            {
                os.WriteByte(0);
                len--;
            }

            for (int x = 0; x < len; x++)
            {
                os.WriteByte((byte)(value >> (8 * ((len - 1) - x) & LenMask)));
            }
        }

        /// <summary>Decodes an ASN.1 length</summary>
        /// <param name="ins">A <see cref="BERInputStream"/> to read from</param>
        /// <returns>The decoded length</returns>
        /// <exception cref="IOException">If an error occurs reading from the stream</exception>
        public static int DecodeLength(BERInputStream ins)
        {
            return BER.DecodeLength(ins, true);
        }
        
        /// <summary>Decodes an ASN.1 length</summary>
        /// <param name="ins">A <see cref="BERInputStream"/> to read from</param>
        /// <param name="checkLength">If <c>false</c> then length check is suppressed</param>
        /// <returns>The decoded length</returns>
        /// <exception cref="IOException">If an error occurs reading from the stream</exception>
        public static int DecodeLength(BERInputStream ins, bool checkLength)
        {
            int length = 0;
            int lengthbyte = ins.ReadByte();

            if ((lengthbyte & Asn1LongLength) > 0)
            {
                lengthbyte &= ~Asn1LongLength;    /* turn MSb off */

                if (lengthbyte == 0)
                {
                    throw new IOException("Indefinite lengths are not supported");
                }

                if (lengthbyte > 4)
                {
                    throw new IOException("Data length > 4 bytes are not supported!");
                }

                for (int i = 0; i < lengthbyte; i++)
                {
                    int l = ins.ReadByte() & 0xFF;
                    length |= l << (8 * (lengthbyte - 1 - i));
                }

                if (length < 0)
                {
                    throw new IOException("SNMP does not support data lengths > 2^31");
                }
            }
            else
            { 
                // short asnlength
                length = lengthbyte & 0xFF;
            }
            
            // If activated we do a length check here: length > is.available() -> throw
            // exception  
            if (checkLength)
            {
                CheckLength(ins, length);
            }

            return length;
        }
        
        /// <summary>
        /// Decodes an ASN.1 header for an object with the ID and
        /// length specified.
        /// </summary>
        /// <remarks>On entry, data length is input as the number of valid bytes following
        /// "data".  On exit, it is returned as the number of valid bytes
        /// in this object following the id and length.
        /// This only works on data types smaller than 30, i.e.no extension octets.
        /// The maximum length is 0xFFFF;
        /// </remarks>
        /// <param name="ins">The <see cref="BERInputStream"/> to read from</param>
        /// <param name="type">The type of the object at the current position in the stream</param>
        /// <param name="checkLength">if false, length check is not performed</param>
        /// <returns>The decoded length of the object</returns>
        /// <exception cref="IOException">If an error occurred reading from the stream</exception>
        public static int DecodeHeader(
            BERInputStream ins,
            out MutableByte type,
            bool checkLength)
        {
            // this only works on data types < 30, i.e. no extension octets
            byte t = (byte)ins.ReadByte();
            if ((t & Asn1ExtensionId) == Asn1ExtensionId)
            {
                throw new IOException("Cannot process extension IDs" +
                                      ins.PositionMessage);
            }

            type = new MutableByte(t);
            return BER.DecodeLength(ins, checkLength);
        }
        
        /// <summary>
        /// Decodes an ASN.1 header for an object with the specified ID and length
        /// </summary>
        /// <remarks>
        /// On entry, data length is input as the number of valid bytes following
        /// "data". On exit it is returned as the number of valid bytes in this
        /// object following the id and length.
        /// This works only on data types smaller than 30, i.e. no extension bytes. 
        /// The maximum length is 0xFFFF.
        /// </remarks>
        /// <param name="ins">The <see cref="BERInputStream"/> to read from</param>
        /// <param name="type">The expected type of the object at the current position
        /// in the input stream
        /// </param>
        /// <returns>The decoded length of the object</returns>
        /// <exception cref="IOException">If an error occurred reading from the stream</exception>
        public static int DecodeHeader(BERInputStream ins, out MutableByte type)
        {
            return DecodeHeader(ins, out type, true);
        }

        /// <summary>
        /// Decodes an integer from a BER input stream
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to be read from</param>
        /// <param name="type">The variable which will contain the decoded value type </param>
        /// <returns>The value of the decoded integer</returns>
        public static int DecodeInteger(BERInputStream ins, out MutableByte type)
        {
            int length;
            int value = 0;

            type = new MutableByte();

            type.Value = (byte)ins.ReadByte();

            if ((type.Value != 0x02) && (type.Value != 0x43) &&
                (type.Value != 0x41))
            {
                throw new IOException(
                    "Wrong ASN.1 type. Not an integer: " + type.Value +
                    ins.PositionMessage);
            }

            length = BER.DecodeLength(ins);

            if (length > 4)
            {
                throw new IOException(
                    "Length greater than 32bit are not supported " +
                    " for integers: " + ins.PositionMessage);
            }

            int b = ins.ReadByte() & 0xFF;

            if ((b & 0x80) > 0)
            {
                value = -1; /* integer is negative */
            }

            while (length-- > 0)
            {
                value = (value << 8) | b;
                if (length > 0)
                {
                    b = ins.ReadByte();
                }
            }

            return value;
        }

        /// <summary>
        /// Decodes an unsigned integer value from a BER input stream
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to be read from</param>
        /// <param name="type">The type read from the input stream</param>
        /// <returns>The value of the integer read, as a long</returns>
        public static long DecodeUnsignedInteger(BERInputStream ins, out MutableByte type)
        {
            int length;
            long value = 0;

            type = new MutableByte();

            type.Value = (byte)ins.ReadByte();
            if ((type.Value != 0x02) && (type.Value != 0x43) &&
                (type.Value != 0x41) && (type.Value != 0x42) &&
                (type.Value != 0x47))
            {
                throw new IOException(
                    "Wrong ASN.1 type. Not an unsigned integer: " +
                    type.Value +
                    ins.PositionMessage);
            }

            // pick up the len
            length = BER.DecodeLength(ins);

            // check for legal uint size
            int b = ins.ReadByte();
            if ((length > 5) || ((length > 4) && (b != 0x00)))
            {
                throw new IOException("Only 32bit unsigned integers are supported" +
                                      ins.PositionMessage);
            }

            // check for leading  0 octet
            if (b == 0x00)
            {
                if (length > 1)
                {
                    b = ins.ReadByte();
                }

                length--;
            }

            // calculate the value
            for (int i = 0; i < length; i++)
            {
                //// value = (value << 8) | (b & 0xFF);
                value = (long)((((ulong)value) << 8) | (uint)(b & 0xFF));

                if (i + 1 < length)
                {
                    b = ins.ReadByte();
                }
            }

            return value;
        }

        /// <summary>
        /// Decodes an string value from an input stream and returns it as a byte array. This covers
        /// the Octet String, IP Address, Opaque and Bit String types
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to be read from</param>
        /// <param name="type">The type read from the stream</param>
        /// <returns>The decoded string, as an array of bytes</returns>
        public static byte[] DecodeString(BERInputStream ins, out MutableByte type)
        {
            // ASN.1 octet string ::= primstring | cmpdstring
            // primstring ::= 0x04 asnlength byte {byte}*
            // cmpdstring ::= 0x24 asnlength string {string}*
            // ipaddress  ::= 0x40 4 byte byte byte byte
            type = new MutableByte();

            type.Value = (byte)ins.ReadByte();
            if ((type.Value != BER.OCTETSTRING) && (type.Value != 0x24) &&
                (type.Value != BER.IPADDRESS) && (type.Value != BER.OPAQUE) &&
                (type.Value != BER.BITSTRING) &&
                (type.Value != 0x45))
            {
                throw new IOException("Wrong ASN.1 type. Not a string: " + type.Value +
                                      ins.PositionMessage);
            }

            int length = BER.DecodeLength(ins);

            byte[]
            value = new byte[length];
            int pos = 0;

            while ((pos < length) && (ins.Available > 0))
            {
                int read = ins.Read(value, 0, length);
                if (read > 0)
                {
                    pos += read;
                }
                else if (read < 0)
                {
                    throw new IOException("Wrong string length " + read + " < " + length);
                }
            }

            return value;
        }

        /// <summary>
        /// Decodes an Object Identifier from a BER input stream and returns it as an integer array
        /// </summary>
        /// <param name="ins">The input stream to read from</param>
        /// <param name="type">An out parameter representing the type read from the stream</param>
        /// <returns>An integer array representing the OID as a set of subIDs</returns>
        public static long[] DecodeOID(BERInputStream ins, out MutableByte type)
        {
            /* ASN.1 objid ::= 0x06 asnlength subidentifier {subidentifier}*
             * subidentifier ::= {leadingbyte}* lastbyte
             * leadingbyte ::= 1 7bitvalue
             * lastbyte ::= 0 7bitvalue
             */
            long subidentifier;
            int length;

            type = new MutableByte();

            type.Value = (byte)ins.ReadByte();
            if (type.Value != 0x06)
            {
                throw new IOException("Wrong type. Not an OID: " + type.Value +
                                      ins.PositionMessage);
            }

            length = BER.DecodeLength(ins);

            long[] oid = new long[length + 2];
            
            // Handle invalid object identifier encodings of the form 06 00 robustly
            if (length == 0)
            {
                oid[0] = oid[1] = 0;
            }

            int pos = 1;
            while (length > 0)
            {
                subidentifier = 0;
                int b;

                do
                {
                    // shift and add in low order 7 bits
                    int next = ins.ReadByte();
                    if (next < 0)
                    {
                        throw new IOException("Unexpected end of input stream" +
                                              ins.PositionMessage);
                    }

                    b = next & 0xFF;
                    subidentifier = (subidentifier << 7) + (b & ~Asn1Bit8);
                    length--;
                }
                while ((length > 0) && ((b & Asn1Bit8) != 0));    // last byte has high bit clear

                oid[pos++] = subidentifier;
            }

            // The first two subidentifiers are encoded into the first component
            // with the value (X * 40) + Y, where:
            // X is the value of the first subidentifier.
            // Y is the value of the second subidentifier.
            subidentifier = oid[1];
            if (subidentifier == 0x2B)
            {
                oid[0] = 1;
                oid[1] = 3;
            }
            else if (subidentifier >= 0 && subidentifier < 80)
            {
                if (subidentifier < 40)
                {
                    oid[0] = 0;
                    oid[1] = subidentifier;
                }
                else
                {
                    oid[0] = 1;
                    oid[1] = subidentifier - 40;
                }
            }
            else
            {
                oid[0] = 2;
                oid[1] = subidentifier - 80;
            }

            if (pos < 2)
            {
                pos = 2;
            }

            long[] value = new long[pos];
            System.Array.Copy(oid, 0, value, 0, pos);
            return value;
        }

        /// <summary>
        /// Decodes a Null type from a BER input stream
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to read from</param>
        /// <param name="type">An out parameter which returns the type read from the stream</param>
        public static void DecodeNull(BERInputStream ins, out MutableByte type)
        {
            type = new MutableByte();
            type.Value = (byte)(ins.ReadByte() & 0xFF);
            if ((type.Value != (byte)0x05) && (type.Value != (byte)0x80) &&
                (type.Value != (byte)0x81) && (type.Value != (byte)0x82))
            {
                throw new IOException("Wrong ASN.1 type. Is not null: " + type.Value +
                                      ins.PositionMessage);
            }

            int length = BER.DecodeLength(ins);

            if (length != 0)
            {
                throw new IOException("Invalid Null encoding, length is not zero: " +
                                      length + ins.PositionMessage);
            }
        }

        /// <summary>
        /// Decodes an unsigned 64bit integer from a BER input stream
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to read from</param>
        /// <param name="type">An out parameter which returns the type read from the stream</param>
        /// <returns>The integer read from the stream</returns>
        public static long DecodeUnsignedInt64(BERInputStream ins, out MutableByte type)
        {
            type = new MutableByte();
            type.Value = (byte)ins.ReadByte();

            if ((type.Value != 0x02) && (type.Value != 0x46))
            {
                throw new IOException("Wrong type. Not an integer 64: " + type.Value +
                                      ins.PositionMessage);
            }

            int length = BER.DecodeLength(ins);
            int b = ins.ReadByte() & 0xFF;

            if (length > 9)
            {
                throw new IOException("Invalid 64bit unsigned integer length: " + length +
                                     ins.PositionMessage);
            }
            
            // check for leading  0 octet
            if (b == 0x00)
            {
                if (length > 1)
                {
                    b = ins.ReadByte();
                }

                length--;
            }

            long value = 0;
            
            // calculate the value
            for (int i = 0; i < length; i++)
            {
                //// value = (value << 8) | (b & 0xFF);
                value = (long)((((ulong)value) << 8) | (uint)(b & 0xFF));

                if (i + 1 < length)
                {
                    b = ins.ReadByte();
                }
            }

            return value;
        }

        /// <summary>
        /// Checks a sequence length against the length of the parsed items. An IOException
        /// is thrown if the lengths do not match.
        /// </summary>
        /// <param name="expectedLength">The expected length of the sequence</param>
        /// <param name="sequence">The sequence to be verified</param>
        /// <exception cref="IOException">If the lengths do not match</exception>
        public static void CheckSequenceLength(
            int expectedLength,
            IBERSerializable sequence)
        {
            if (BER.CheckSequenceLengthFlag &&
                (expectedLength != sequence.BERPayloadLength))
            {
                throw new IOException("The actual length of the SEQUENCE object " +
                                      sequence.GetType().Name +
                                      " is " + sequence.BERPayloadLength + ", but " +
                                      expectedLength + " was expected");
            }
        }

        /// <summary>
        /// Checks a sequence length against the length of the parsed items. An IOException
        /// is thrown if the lengths do not match.
        /// </summary>
        /// <param name="expectedLength">The expected length of the sequence</param>
        /// <param name="actualLength">The actual length of the read items</param>
        /// <param name="sequence">The sequence to be verified</param>
        /// <exception cref="IOException">If the lengths do not match</exception>
        public static void CheckSequenceLength(
            int expectedLength,
            int actualLength,
            IBERSerializable sequence)
        {
            if (BER.CheckSequenceLengthFlag &&
                (expectedLength != actualLength))
            {
                throw new IOException("The actual length of the SEQUENCE object " +
                                      sequence.GetType().Name +
                                      " is " + actualLength + ", but " +
                                      expectedLength + " was expected");
            }
        }

        /// <summary>
        /// Helper method to encode a subID in BER format
        /// </summary>
        /// <param name="os">An <see cref="Stream"/> to which the encoded OID is written</param>
        /// <param name="subID">The subID value to encode</param>
        private static void EncodeSubID(Stream os, long subID)
        {
            long subid = subID & 0xFFFFFFFFL;

            if (subid < 127)
            {
                os.WriteByte((byte)(subid & 0xFF));
            }
            else
            {
                long mask = 0x7F; // handle subid == 0 case
                int bits = 0;
                int testbits = 0;

                // testmask *MUST* !!!! be of an unsigned type
                for (long testmask = 0x7F; testmask != 0;
                     testmask <<= 7, testbits += 7)
                {
                    if ((subid & testmask) > 0)
                    {
                        // if any bits set
                        mask = testmask;
                        bits = testbits;
                    }
                }

                // mask can't be zero here
                for (; mask != 0x7F; mask >>= 7, bits -= 7)
                {
                    // fix a mask that got truncated above
                    if (mask == 0x1E00000)
                    {
                        mask = 0xFE00000;
                    }

                    os.WriteByte((byte)(((subid & mask) >> bits) | Asn1Bit8));
                }

                os.WriteByte((byte)(subid & mask));
            }
        }

        /// <summary>
        /// Checks whether the length of that was encoded is also available from the stream
        /// </summary>
        /// <param name="ins">The <see cref="BERInputStream"/> to be read from</param>
        /// <param name="length">The required number of bytes</param>
        /// <exception cref="IOException">
        /// If there are insufficient available bytes without blocking
        /// </exception>
        private static void CheckLength(BERInputStream ins, int length)
        {
            if (!checkValueLengthFlag)
            {
                return;
            }

            if ((length < 0) || (length > ins.Length))
            {
                throw new IOException("The encoded length " +
                                      length +
                                      " exceeds the number of bytes left in input" +
                                      ins.PositionMessage +
                                      " which actually is " + ins.Length);
            }
        }

        /// <summary>
        /// Helper method to determine the length of the encoding of a subID
        /// </summary>
        /// <param name="subID">The subID whose encoding length must be determined</param>
        /// <returns>The encoding length</returns>
        private static int GetSubIDLength(long subID)
        {
            long v = subID & 0xFFFFFFFFL;

            if (v < 0x80)
            {
                // 7 bits long subid
                return 1;
            }
            else if (v < 0x4000)
            {
                // 14 bits long subid
                return 2;
            }
            else if (v < 0x200000)
            {
                // 21 bits long subid
                return 3;
            }
            else if (v < 0x10000000)
            {
                // 28 bits long subid
                return 4;
            }

            // 32 bits long subid
            return 5;
        }

        /// <summary>
        /// The <code>MutableByte</code> class serves for exchanging type information
        /// from the various Decode* methods.
        /// </summary>
        public class MutableByte
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MutableByte"/> class.
            /// </summary>
            public MutableByte()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MutableByte"/> class
            /// with an initial supplied value
            /// </summary>
            /// <param name="value">The initial value</param>
            public MutableByte(byte value)
            {
                this.Value = value;
            }

            /// <summary>
            /// Gets or sets the value of the Mutable Byte
            /// </summary>
            public byte Value
            {
                get; set;
            }
        }
    }
}
