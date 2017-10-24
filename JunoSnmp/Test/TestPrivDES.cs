// <copyright file="TestPrivDES.cs" company="None">
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
    using NUnit.Framework;

    [TestFixture]
    class TestPrivDES
    {
        [TestCase]
        public void TestEncrypt()
        {
            PrivDES pd = new PrivDES();
            DecryptParams pp = new DecryptParams();
            byte[] key = System.Text.UTF8Encoding.UTF8.GetBytes("1234567890123456");
            
            byte[] plaintext = System.Text.UTF8Encoding.UTF8.GetBytes(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
                );
            byte[] ciphertext = null;
            byte[] decrypted = null;
            int engine_boots = 1;
            int engine_time = 2;

            //cat.debug("Cleartext: " + asHex(plaintext));
            ciphertext = pd.Encrypt(plaintext, 0, plaintext.Length, key, engine_boots, engine_time, pp);
            //cat.debug("Encrypted: " + asHex(ciphertext));
            decrypted = pd.Decrypt(ciphertext, 0, ciphertext.Length, key, engine_boots, engine_time, pp);
            //cat.debug("Cleartext: " + asHex(decrypted));

            for (int i = 0; i < plaintext.Length; i++)
            {
                Assert.AreEqual(plaintext[i], decrypted[i]);
            }
            //cat.info("pp length is: " + pp.length);
            Assert.AreEqual(8, pp.Length);
        }
    }
}
