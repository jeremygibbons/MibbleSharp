// <copyright file="TestPriv3DES.cs" company="None">
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
//    Original Java code from Snmp4J Copyright (C) 2003-2017 Frank Fock and 
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
    class TestPriv3DES
    {
        [TestCase]
        public void Test3DESKey()
        {
            SecurityProtocols protos = SecurityProtocols.GetInstance();
            protos.AddDefaultProtocols();
            protos.AddPrivacyProtocol(new Priv3DES());
            OctetString engineid = OctetString.FromHexString("00:00:00:00:00:00:00:00:00:00:00:02");
            OctetString password = new OctetString("maplesyrup");
            byte[] expectedKey =
                OctetString.FromHexString("52:6f:5e:ed:9f:cc:e2:6f:89:64:c2:93:07:87:d8:2b:79:ef:f4:4a:90:65:0e:e0:a3:a4:0a:bf:ac:5a:cc:12").ToByteArray();
            byte[] key = protos.PasswordToKey(Priv3DES.protocolOid, AuthMD5.Id, password, engineid.ToByteArray());

            for (int i = 0; i < expectedKey.Length; i++)
            {
                Assert.AreEqual(expectedKey[i], key[i]);
            }
        }

        [TestCase]
        public void Test3DESEncrypt()
        {

            Priv3DES pd = new Priv3DES();
            DecryptParams pp = new DecryptParams();
            byte[] key = System.Text.UTF8Encoding.UTF8.GetBytes("12345678901234561234567890123456");
            byte[] plaintext = System.Text.UTF8Encoding.UTF8.GetBytes(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            byte[] ciphertext = null;
            byte[] decrypted = null;
            int engine_boots = 1;
            int engine_time = 2;

            ciphertext = pd.Encrypt(plaintext, 0, plaintext.Length, key, engine_boots, engine_time, pp);
            decrypted = pd.Decrypt(ciphertext, 0, ciphertext.Length, key, engine_boots, engine_time, pp);

            for (int i = 0; i < plaintext.Length; i++)
            {
                Assert.AreEqual(plaintext[i], decrypted[i]);
            }

            Assert.AreEqual(8, pp.Length);
        }
    }
}
