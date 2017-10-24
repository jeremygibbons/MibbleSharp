
namespace JunoSnmp.Test
{
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestPrivAES
    {
        [TestCase]
        public static void TestCrypt()
        {
            PrivAES128 pd = new PrivAES128();
            DecryptParams pp = new DecryptParams();
            byte[] key = {
                (byte) 0x66, (byte) 0x95, (byte) 0xfe, (byte) 0xbc,
                (byte) 0x92, (byte) 0x88, (byte) 0xe3, (byte) 0x62,
                (byte) 0x82, (byte) 0x23, (byte) 0x5f, (byte) 0xc7,
                (byte) 0x15, (byte) 0x1f, (byte) 0x12, (byte) 0x84//,
                /*
                (byte) 0x97, (byte) 0xb3, (byte) 0x8f, (byte) 0x3f,
                (byte) 0x50, (byte) 0x5E, (byte) 0x07, (byte) 0xEB,
                (byte) 0x9A, (byte) 0xF2, (byte) 0x55, (byte) 0x68,
                (byte) 0xFA, (byte) 0x1F, (byte) 0x5D, (byte) 0xBE*/
            };
            byte[] plaintext = System.Text.UTF8Encoding.UTF8.GetBytes("This is a secret message, nobody is allowed to read it!");
            byte[] ciphertext = null;
            byte[] decrypted = null;
            int engine_boots = unchecked((int)0xdeadc0de);
            int engine_time = unchecked((int)0xbeefdede);

            ciphertext = pd.Encrypt(plaintext, 0, plaintext.Length, key, engine_boots,
                                    engine_time, pp);
            decrypted = pd.Decrypt(ciphertext, 0, ciphertext.Length, key, engine_boots, engine_time, pp);

            Assert.AreEqual(AsHex(plaintext), AsHex(decrypted));

            Assert.AreEqual(8, pp.Length);
        }

        [TestCase]
        public void TestAesKeyExtension()
        {
            SecurityProtocols.GetInstance().AddAuthenticationProtocol(new AuthSHA());
            SecurityProtocols.GetInstance().AddPrivacyProtocol(new PrivAES256());
            byte[] key =
              SecurityProtocols.GetInstance().PasswordToKey(PrivAES256.Oid, AuthSHA.Id, new OctetString("maplesyrup"),
                new byte[] {(byte) 0, (byte) 0, (byte) 0, (byte)0,
                    (byte) 0, (byte) 0, (byte) 0, (byte)0,
                    (byte) 0, (byte) 0, (byte) 0, (byte)2 });
            Assert.AreEqual("66:95:fe:bc:92:88:e3:62:82:23:5f:c7:15:1f:12:84:97:b3:8f:3f:50:5e:07:eb:9a:f2:55:68:fa:1f:5d:be",
                new OctetString(key).ToHexString());
        }

        [TestCase]
        public void TestSecurityProtocolsAddDefaultProtocols()
        {
            JunoSnmpSettings.ExtensibilityEnabled = true;
            //System.setProperty(SecurityProtocols.SECURITY_PROTOCOLS_PROPERTIES, "SecurityProtocolsTest.properties");
            SecurityProtocols.GetInstance().AddDefaultProtocols();
            OID aes192AGENTppID = new OID("1.3.6.1.6.3.10.1.2.20");
            OID aes256AGENTppID = new OID("1.3.6.1.6.3.10.1.2.21");
            Assert.NotNull(SecurityProtocols.GetInstance().GetPrivacyProtocol(aes192AGENTppID));
            Assert.NotNull(SecurityProtocols.GetInstance().GetPrivacyProtocol(aes256AGENTppID));
            Assert.AreEqual(aes192AGENTppID, SecurityProtocols.GetInstance().GetPrivacyProtocol(aes192AGENTppID).ID);
            Assert.AreEqual(aes256AGENTppID, SecurityProtocols.GetInstance().GetPrivacyProtocol(aes256AGENTppID).ID);
        }

        private static string AsHex(byte[] buf)
        {
            return new OctetString(buf).ToHexString();
        }
    }
}
