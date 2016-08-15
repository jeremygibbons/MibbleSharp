// <copyright file="TestBER.cs" company="None">
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

namespace JunoSnmp.Test
{
    using System;
    using System.IO;
    using System.Text;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    /// <summary>
    /// Tests for verifying BER encoding and decoding.
    /// </summary>
    [TestFixture]
    public class TestBER
    {
        [TestCase]
        public void TestEncodeHeader()
        {
            byte[] header = { (byte)0x80, (byte)0x83, (byte)0x73, (byte)0x59, (byte)0xB5 };
            MemoryStream os1 = new MemoryStream();
            int type2 = 0x80;
            int length3 = 7559605;
            BER.EncodeHeader(os1, type2, length3);
            Assert.AreEqual(os1.Length, header.Length);
            byte[] result = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(result[i], header[i]);
            }
        }


        public void TestEncodeInteger()
        {
            byte[] result = { 0x02, 0x02, (byte)0x96, (byte)0xB5 };
            MemoryStream os1 = new MemoryStream();
            byte type2 = 0x02;
            int value3 = -26955;
            BER.EncodeInteger(os1, type2, value3);

            Assert.AreEqual(os1.Length, result.Length);
            byte[] value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(value[i], result[i]);
            }


            Integer32 i32 = new Integer32(value3);
            Assert.AreEqual(result.Length, i32.BERLength);
        }

        public void TestEncodeOID()
        {
            byte[] result = { 0x06, 0x04, 0x2B, 0x06, (byte)0x99, 0x37 };
            MemoryStream os1 = new MemoryStream();
            byte type2 = 6;
            long[] oid3 = { 1, 3, 6, 3255 };
            BER.EncodeOID(os1, type2, oid3);

            Assert.AreEqual(os1.Length, result.Length);
            byte[] value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(value[i], result[i]);
            }

            OID variable = new OID(oid3);
            Assert.AreEqual(result.Length, variable.BERLength);
        }

        public void TestEncodeOIDMaxSubID()
        {
            MemoryStream os1 = new MemoryStream();
            byte type2 = 6;
            long[] oid3 = { 1, 3, 6, -1 };
            BER.EncodeOID(os1, type2, oid3);

            byte[] value = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            OID variable = new OID(BER.DecodeOID(new BERInputStream(value), out b));
            Assert.AreEqual(new OID(oid3), variable);
        }

        public void TestEncodeSequence()
        {
            byte[]
                result = {
            0x30, 0x09, 0x02, 0x01, 0x00, 0x04, 0x04,
                      (byte)0xEB, 0x06, (byte)0x99,
                      0x37 };
            MemoryStream os1 = new MemoryStream();
            byte type2 = 0x30;
            int length3 = 9;
            byte[] value = null;
            BER.EncodeSequence(os1, type2, length3);
            BER.EncodeInteger(os1, (byte)0x02, 0);
            BER.EncodeString(os1, (byte)0x04, new byte[] { (byte)0xEB, 0x06, (byte)0x99, 0x37 });

            Assert.AreEqual(result.Length, os1.Length);
            value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(value[i], result[i]);
            }
        }


        public void TestEncodeString()
        {
            byte[] result = { 0x04, 0x04, (byte)0xEB, 0x06, (byte)0x99, 0x37 };
            MemoryStream os1 = new MemoryStream();
            byte[] value = new byte[] { (byte)0xEB, 0x06, (byte)0x99, 0x37 };
            BER.EncodeString(os1, (byte)0x04, value);

            Assert.AreEqual(result.Length, os1.Length);
            byte[] encoded = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(encoded[i], result[i]);
            }

            OctetString variable = new OctetString(value);
            Assert.AreEqual(result.Length, variable.BERLength);
        }

        public void TestEncodeUnsignedInteger()
        {
            byte[] result = { 0x42, 0x05, 0x00, (byte)0x80, 0x00, 0x00, 0x00 };
            MemoryStream os1 = new MemoryStream();
            byte type2 = 0x42;
            long value3 = 2147483648L;
            byte[] value = null;
            BER.EncodeUnsignedInteger(os1, type2, value3);

            Assert.AreEqual(result.Length, os1.Length);
            value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
                Assert.AreEqual(result[i], value[i]);

            UnsignedInteger32 variable = new UnsignedInteger32(value3);
            Assert.AreEqual(result.Length, variable.BERLength);
        }

        public void TestEncodeUnsignedInt64()
        {   
            byte[] result = {
            0x46, 0x09, 0x00, (byte)0xC9, (byte)0xAC, (byte)0xC1, (byte)0x87,
                      0x4B, (byte)0xB1, (byte)0xE1, (byte)0xB9 };
            byte[] result3 = { 0x46, 0x01, 0x03 };
            byte[] result4 = { 0x46, 0x04, 0x01, 0x00, 0x00, 0x01 };
            MemoryStream os1 = new MemoryStream(11);
            byte type2 = 0x46;
            long value3 = -3914541189257109063L;// 14532202884452442553l;
            byte[] value = null;
            BER.EncodeUnsignedInt64(os1, type2, value3);

            Assert.AreEqual(result.Length, os1.Length);
            value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(result[i], value[i]);
            }

            Counter64 variable = new Counter64(value3);
            Assert.AreEqual(result.Length, variable.BERLength);

            os1 = new MemoryStream(3);
            BER.EncodeUnsignedInt64(os1, type2, 3);

            Assert.AreEqual(3, os1.Length);

            value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(result3[i], value[i]);
            }

            os1 = new MemoryStream(3);
            BER.EncodeUnsignedInt64(os1, type2, 16777217);

            Assert.AreEqual(6, os1.Length);
            value = os1.ToArray();
            for (int i = 0; i < os1.Length; i++)
            {
                Assert.AreEqual(result4[i], value[i]);
            }
        }

        public void TestDecodeLength()
        {
            MemoryStream os1 = new MemoryStream();
            int length = 7559605;
            BER.EncodeLength(os1, length);
            byte[] result = os1.ToArray();
            IOException ex = null;
            try
            {
                int decodedLength =
                    BER.DecodeLength(new BERInputStream(result));
                Assert.AreEqual(length, decodedLength);
            }
            catch (IOException iox)
            {
                ex = iox;
            }
            Assert.IsNotNull(ex);
        }

        public void TestDecodeInteger()
        {
            MemoryStream os1 = new MemoryStream();
            int integer = -1;
            BER.EncodeInteger(os1, (byte)0x02, integer);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            int decodedInteger =
                BER.DecodeInteger(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x02, b.Value);

            integer = 0x7FFFFFFF;
            os1 = new MemoryStream();
            BER.EncodeInteger(os1, (byte)0x02, integer);
            result = os1.ToArray();
            decodedInteger =
                BER.DecodeInteger(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x02, b.Value);
        }

        public void TestDecodeUnsignedInteger()
        {
            MemoryStream os1 = new MemoryStream();
            long integer = 0xFFFFFFFFL;
            BER.EncodeUnsignedInteger(os1, (byte)0x42, integer);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long decodedInteger =
                BER.DecodeUnsignedInteger(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x42, b.Value);

            integer = 0x7FFFFFFFL;
            os1 = new MemoryStream();
            BER.EncodeUnsignedInteger(os1, (byte)0x43, integer);
            result = os1.ToArray();
            decodedInteger =
                BER.DecodeUnsignedInteger(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x43, b.Value);
        }

        public void TestDecodeUnsignedInt64()
        {
            MemoryStream os1 = new MemoryStream();
            long integer = -1;
            BER.EncodeUnsignedInt64(os1, (byte)0x46, integer);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long decodedInteger =
                BER.DecodeUnsignedInt64(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x46, b.Value);

            integer = 0x7FFFFFFFFFFFFFFFL;
            os1 = new MemoryStream();
            BER.EncodeUnsignedInt64(os1, (byte)0x46, integer);
            result = os1.ToArray();
            decodedInteger =
                BER.DecodeUnsignedInt64(new BERInputStream(result), out b);
            Assert.AreEqual(integer, decodedInteger);
            Assert.AreEqual((byte)0x46, b.Value);
        }

        public void TestDecodeString()
        {
            MemoryStream os1 = new MemoryStream();
            string s = "Hello SNMP4J";
            BER.EncodeString(os1, (byte)0x04, Encoding.UTF8.GetBytes(s));
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            byte[] decodedString =
                BER.DecodeString(new BERInputStream(result), out b);
            for (int i = 0; i < decodedString.Length; i++)
                Assert.AreEqual(Encoding.UTF8.GetBytes(s)[i], decodedString[i]);
            Assert.AreEqual((byte)0x04, b.Value);
        }

        public void TestDecodeOID()
        {
            MemoryStream os1 = new MemoryStream();
            long[] s = { 1, 3, 6, 1, 4, 4976, 1, 0 };
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID0()
        {
            MemoryStream os1 = new MemoryStream();
            long[] s = { 0, 39, 6, 1, 4, 4976, 1, 0 };
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID11()
        {
            MemoryStream os1 = new MemoryStream();
            long[] s = { 1, 1, 6, 1, 4, 4976, 1, 0 };
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID10()
        {
            MemoryStream os1 = new MemoryStream();
            long[] s = { 1, 0, 6, 1, 4, 4976, 1, 0 };
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID139()
        {
            long[]
            s = { 1, 39, 6, 1, 4, 4976, 1, 0 };
            int oidLength = BER.GetOIDLength(s);
            Assert.AreEqual(8, oidLength);
            MemoryStream os1 = new MemoryStream(oidLength);
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID2()
        {
            long[]
            s = { 2, 2205, 6, 1, 4, 4976, 1, 0 };
            int oidLength = BER.GetOIDLength(s);
            Assert.AreEqual(9, oidLength);
            MemoryStream os1 = new MemoryStream(oidLength);
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        public void TestDecodeOID2Big()
        {
            long[]
            s = { 2, 1073741824, 6, 1, 4, 4976, 1, 0 };
            int oidLength = BER.GetOIDLength(s);
            Assert.AreEqual(12, oidLength);
            MemoryStream os1 = new MemoryStream(oidLength);
            BER.EncodeOID(os1, (byte)0x06, s);
            byte[] result = os1.ToArray();
            BER.MutableByte b = new BER.MutableByte();
            long[] decodedOID =
                BER.DecodeOID(new BERInputStream(result), out b);
            for (int i = 0; i < decodedOID.Length; i++)
                Assert.AreEqual(s[i], decodedOID[i]);
            Assert.AreEqual((byte)0x06, b.Value);
        }

        /*
        public void TestDecodeScopedPDU()  {
          OctetString scopedPDUString =
              OctetString.fromHexString("30:3f:02:01:03:30:12:02:04:04:44:59:05:02:04:00:00:ff:e2:04:01:04:02:01:03:04:10"+
                  ":30:0e:03:00:02:01:00:02:01:00:04:00:04:00:04:00:30:14:04:00:04:00:a0:0e:02:04:04:44:59:05:02:01:00:02:"+
                  "01:00:30:00");
          BERInputStream wholeMsg = new BERInputStream(ByteBuffer.wrap(scopedPDUString.GetValue()));
          BER.MutableByte type = new BER.MutableByte();
          int length = BER.DecodeHeader(wholeMsg, type);
          Assert.AreEqual(type.GetValue(),BER.SEQUENCE);
          long lengthOfLength = wholeMsg.getPosition();
          wholeMsg.reset();
          wholeMsg.mark(length);
          Assert.AreEqual(wholeMsg.skip(lengthOfLength), lengthOfLength);
          Integer32 snmpVersion = new Integer32();
          snmpVersion.decodeBER(wholeMsg);
          Assert.AreEqual(snmpVersion.GetValue(), SnmpConstants.version3);
          // decode SNMPv3 header
          MPv3.HeaderData header = new MPv3.HeaderData();
          header.decodeBER(wholeMsg);
          ScopedPDU scopedPDU = new ScopedPDU();
          scopedPDU.decodeBER(wholeMsg);
        }
        */
    }
}
