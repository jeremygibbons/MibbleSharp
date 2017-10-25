// <copyright file="PrivacyGeneric.cs" company="None">
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


namespace JunoSnmp.Security
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp.SMI;

    /// <summary>
    /// The PrivacyGeneric abstract class implements common functionality of privacy protocols.
    /// </summary>
    public abstract class PrivacyGeneric : IPrivacyProtocol
    {
        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected string protocolId;
        protected string protocolClass;
        protected int keyBytes;
        protected Salt salt;
        protected int initVectorLength;

        public bool Supported
        {
            get
            {
                try
                {


                    using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(protocolId))
                    {
                        sa.Key = new byte[keyBytes]; ;
                        sa.IV = new byte[initVectorLength]; ;

                        // Create an encryptor to perform the stream transform.
                        sa.CreateEncryptor(sa.Key, sa.IV);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(protocolClass + " privacy not available. Error: " + e.Message);
                    }

                    return false;
                }
            }
        }

        public abstract int MinKeyLength { get; }
        public abstract int MaxKeyLength { get; }
        public abstract int DecryptParamsLength { get; }
        public abstract OID ID { get; }

        protected byte[] DoEncrypt(byte[] encryptionKey, byte[] initVect, byte[] unencryptedData, int offset, int length)
        {
            using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(protocolId))
            {
                sa.Key = encryptionKey;
                sa.IV = initVect;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = sa.CreateEncryptor(sa.Key, sa.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(unencryptedData, offset, length);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }



        protected byte[] DoEncryptWithPadding(byte[] encryptionKey, byte[] initVect, byte[] unencryptedData, int offset, int length)
        {
            if (length % 8 == 0)
            {
                return DoEncrypt(encryptionKey, initVect, unencryptedData, offset, length);
            }

            using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(protocolId))
            {
                sa.Key = encryptionKey;
                sa.IV = initVect;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = sa.CreateEncryptor(sa.Key, sa.IV);

                if (log.IsDebugEnabled)
                {
                    log.Debug("Using padding.");
                }

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(unencryptedData, offset, length);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        protected byte[] DoDecrypt(byte[] cryptedData, int offset, int length, byte[] decryptionKey, byte[] iv)
        {
            byte[] decryptedData = null;

            using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(protocolId))
            {
                byte[] key = GetValidSizedKey(sa, decryptionKey);
                sa.Key = key;
                sa.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = sa.CreateDecryptor(sa.Key, sa.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        decryptedData = new byte[cryptedData.Length];
                        csDecrypt.Read(decryptedData, 0, decryptedData.Length);
                        return decryptedData;
                    }
                }
            }
        }

        protected byte[] GetValidSizedKey(SymmetricAlgorithm sa, byte[] inputKey)
        {
            int keySizeBytes = 0;
            int decKeySizeBits = inputKey.Length * 8;
            KeySizes[] ks = sa.LegalKeySizes;
            foreach (KeySizes k in ks)
            {
                if (decKeySizeBits == k.MinSize
                    || decKeySizeBits == k.MaxSize
                    || (decKeySizeBits > k.MinSize
                        && decKeySizeBits < k.MaxSize
                        && (decKeySizeBits - k.MinSize) % k.SkipSize == 0))
                {
                    keySizeBytes = inputKey.Length;
                }
            }

            if (keySizeBytes == 0)
            {
                //Key is too long, trim it.
                //TODO: This needs to be reworked...
                decKeySizeBits = ks[0].MinSize;
                keySizeBytes = decKeySizeBits / 8;
            }

            byte[] key = new byte[keySizeBytes];
            Array.Copy(inputKey, key, keySizeBytes);
            return key;
        }

        public abstract byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] encryptionKey, long engineBoots, long engineTime, DecryptParams decryptParams);
        public abstract byte[] Decrypt(byte[] cryptedData, int offset, int length, byte[] decryptionKey, long engineBoots, long engineTime, DecryptParams decryptParams);
        public abstract int GetEncryptedLength(int scopedPDULength);
        public abstract byte[] ExtendShortKey(byte[] shortKey, OctetString password, byte[] engineID, IAuthenticationProtocol authProtocol);
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

    }
}
