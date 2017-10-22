// <copyright file="TestAuthSHA1.cs" company="None">
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
    class TestAuthSHA1
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
                (byte) 0x66, (byte) 0x95, (byte) 0xfe, (byte) 0xbc,
                (byte) 0x92, (byte) 0x88, (byte) 0xe3, (byte) 0x62,
                (byte) 0x82, (byte) 0x23, (byte) 0x5f, (byte) 0xc7,
                (byte) 0x15, (byte) 0x1f, (byte) 0x12, (byte) 0x84,
                (byte) 0x97, (byte) 0xb3, (byte) 0x8f, (byte) 0x3f};

            AuthSHA auth = new AuthSHA();
            
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
                (byte) 0x78, (byte) 0xe2, (byte) 0xdc, (byte) 0xce,
                (byte) 0x79, (byte) 0xd5, (byte) 0x94, (byte) 0x03,
                (byte) 0xb5, (byte) 0x8c, (byte) 0x1b, (byte) 0xba,
                (byte) 0xa5, (byte) 0xbf, (byte) 0xf4, (byte) 0x63,
                (byte) 0x91, (byte) 0xf1, (byte) 0xcd, (byte) 0x25};

            AuthSHA auth = new AuthSHA();
            byte[] key = auth.PasswordToKey(new OctetString(password), engineId);
            Assert.AreEqual(expectedKey.Length, key.Length);
            for (int i = 0; i < key.Length; i++)
            {
                Assert.AreEqual(expectedKey[i], key[i]);
            }
        }

        [TestCase]
        public void testChangeDelta()
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
                (byte) 0x9c, (byte) 0x10, (byte) 0x17, (byte) 0xf4,
                (byte) 0xfd, (byte) 0x48, (byte) 0x3d, (byte) 0x2d,
                (byte) 0xe8, (byte) 0xd5, (byte) 0xfa, (byte) 0xdb,
                (byte) 0xf8, (byte) 0x43, (byte) 0x92, (byte) 0xcb,
                (byte) 0x06, (byte) 0x45, (byte) 0x70, (byte) 0x51};

            AuthSHA auth = new AuthSHA();
            oldKey = auth.PasswordToKey(new OctetString(oldPass), engineId);
            newKey = auth.PasswordToKey(new OctetString(newPass), engineId);
            byte[] delta = auth.ChangeDelta(oldKey, newKey, random);
            Assert.AreEqual(expectedDelta.Length, delta.Length);
            for (int i = 0; i < delta.Length; i++)
            {
                Assert.AreEqual(delta[i], expectedDelta[i]);
            }
        }
    }
}
