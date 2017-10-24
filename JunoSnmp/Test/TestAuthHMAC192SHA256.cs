// <copyright file="TestAuthHMAC192SHA256.cs" company="None">
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
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Test
{
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestAuthHMAC192SHA256
    {
        [TestCase]
        public void TestPasswordToKey1()
        {
            string password = "maplesyrup";
            byte[] engineId = {
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x02};
            byte[] expectedKey = {
                (byte) 0x89, (byte) 0x82, (byte) 0xe0, (byte) 0xe5,
                (byte) 0x49, (byte) 0xe8, (byte) 0x66, (byte) 0xdb,
                (byte) 0x36, (byte) 0x1a, (byte) 0x6b, (byte) 0x62,
                (byte) 0x5d, (byte) 0x84, (byte) 0xcc, (byte) 0xcc,
                (byte) 0x11, (byte) 0x16, (byte) 0x2d, (byte) 0x45,
                (byte) 0x3e, (byte) 0xe8, (byte) 0xce, (byte) 0x3a,
                (byte) 0x64, (byte) 0x45, (byte) 0xc2, (byte) 0xd6,
                (byte) 0x77, (byte) 0x6f, (byte) 0x0f, (byte) 0x8b};

            AuthHMAC192SHA256 auth = new AuthHMAC192SHA256();
            
            byte[] key = auth.PasswordToKey(new OctetString(password), engineId);
            Assert.AreEqual(expectedKey.Length, key.Length);
            for (int i = 0; i < key.Length; i++)
            {
                Assert.AreEqual(key[i], expectedKey[i]);
            }
        }

        [TestCase]
        public void TestPasswordToKey2()
        {
            string password = "newsyrup";
            byte[] engineId = {
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x02};
            byte[] expectedKey = {
                (byte) 0xb5, (byte) 0xa4, (byte) 0x13, (byte) 0x46,
                (byte) 0xa9, (byte) 0xe3, (byte) 0x88, (byte) 0x80,
                (byte) 0x82, (byte) 0x80, (byte) 0x1f, (byte) 0xa6,
                (byte) 0xc5, (byte) 0x2b, (byte) 0x8c, (byte) 0xcc,
                (byte) 0x75, (byte) 0x04, (byte) 0x36, (byte) 0x2e,
                (byte) 0x67, (byte) 0x9a, (byte) 0x64, (byte) 0x8e,
                (byte) 0x69, (byte) 0x5a, (byte) 0x2b, (byte) 0x49,
                (byte) 0x81, (byte) 0xe9, (byte) 0x46, (byte) 0x28};

            AuthHMAC192SHA256 auth = new AuthHMAC192SHA256();
            byte[] key = auth.PasswordToKey(new OctetString(password), engineId);
            Assert.AreEqual(expectedKey.Length, key.Length);
            for (int i = 0; i < key.Length; i++)
            {
                Assert.AreEqual(expectedKey[i], key[i]);
            }
        }

        [TestCase]
        public void TestChangeDelta()
        {
            string oldPass = "maplesyrup";
            string newPass = "newsyrup";
            byte[] oldKey;
            byte[] newKey;
            byte[] random = {
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00};
            byte[] engineId = {
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x02};
            byte[] expectedDelta = {
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00,
                (byte) 0xe6, (byte) 0xfb, (byte) 0xe8, (byte) 0x99,
                (byte) 0x88, (byte) 0x5a, (byte) 0x4b, (byte) 0x3f,
                (byte) 0x7d, (byte) 0x59, (byte) 0x22, (byte) 0x38,
                (byte) 0x7d, (byte) 0x78, (byte) 0x64, (byte) 0xef,
                (byte) 0xde, (byte) 0x9f, (byte) 0xcc, (byte) 0x27,
                (byte) 0xdf, (byte) 0xc3, (byte) 0xa5, (byte) 0xe7,
                (byte) 0x13, (byte) 0xea, (byte) 0xc6, (byte) 0x06,
                (byte) 0xae, (byte) 0xe6, (byte) 0xdd, (byte) 0x68
            };

            AuthHMAC192SHA256 auth = new AuthHMAC192SHA256();
            oldKey = auth.PasswordToKey(new OctetString(oldPass), engineId);
            newKey = auth.PasswordToKey(new OctetString(newPass), engineId);
            byte[] delta = auth.ChangeDelta(oldKey, newKey, random);
            System.Console.Out.WriteLine("DELTA=" + new OctetString(delta).ToHexString());
            Assert.AreEqual(expectedDelta.Length, delta.Length);
            for (int i = 0; i < delta.Length; i++)
            {
                Assert.AreEqual(delta[i], expectedDelta[i]);
            }
        }
    }
}
