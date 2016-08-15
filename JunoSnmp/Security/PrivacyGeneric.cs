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
        protected CipherPool cipherPool;
        protected int initVectorLength;


        protected Cipher DoInit(byte[] encryptionKey, byte[] initVect)
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
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }

        protected byte[] DoFinal(byte[] unencryptedData, int offset, int length, Cipher alg)
        {
            return alg.doFinal(unencryptedData, offset, length);
        }

        protected byte[] DoFinalWithPadding(byte[] unencryptedData, int offset, int length, Cipher alg)
        {
            byte[] encryptedData;

            if (length % 8 == 0)
            {
                encryptedData = alg.doFinal(unencryptedData, offset, length);
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Using padding.");
                }

                encryptedData = new byte[8 * ((length / 8) + 1)];
                byte[] tmp = new byte[8];

                int encryptedLength = alg.update(unencryptedData, offset, length,
                    encryptedData);
                alg.doFinal(tmp, 0, 8 - (length % 8), encryptedData, encryptedLength);
            }

            return encryptedData;
        }

        protected byte[] DoDecrypt(byte[] cryptedData, int offset, int length, byte[] decryptionKey, byte[] iv)
        {
            byte[] decryptedData = null;

            try
            {
                Cipher alg = cipherPool.reuseCipher();
                if (alg == null)
                {
                    alg = Cipher.getInstance(protocolId);
                }
                SecretKeySpec key = new SecretKeySpec(decryptionKey, 0, keyBytes, protocolClass);
                IvParameterSpec ivSpec = new IvParameterSpec(iv);
                alg.init(Cipher.DECRYPT_MODE, key, ivSpec);
                decryptedData = alg.doFinal(cryptedData, offset, length);
                cipherPool.offerCipher(alg);
            }
            catch (Exception e)
            {
                log.Error(e);
                if (log.IsDebugEnabled)
                {
                    log.Debug(e.StackTrace);
                }
            }
            return decryptedData;
        }

        public abstract byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] encryptionKey, long engineBoots, long engineTime, DecryptParams decryptParams);
        public abstract byte[] Decrypt(byte[] cryptedData, int offset, int length, byte[] decryptionKey, long engineBoots, long engineTime, DecryptParams decryptParams);
        public abstract int GetEncryptedLength(int scopedPDULength);
        public abstract byte[] ExtendShortKey(byte[] shortKey, OctetString password, byte[] engineID, IAuthenticationProtocol authProtocol);
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        public bool Supported
        {
            get
            {
                Cipher alg;

                try
                {
                    alg = cipherPool.reuseCipher();

                    if (alg == null)
                    {
                        alg = Cipher.getInstance(protocolId);
                    }

                    byte[] initVect = new byte[initVectorLength];
                    byte[] encryptionKey = new byte[keyBytes];
                    SecretKeySpec key = new SecretKeySpec(encryptionKey, 0, keyBytes, protocolClass);
                    IvParameterSpec ivSpec = new IvParameterSpec(initVect);
                    alg.init(Cipher.ENCRYPT_MODE, key, ivSpec);
                    return true;
                }
                catch (NoSuchPaddingException e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(protocolClass + " privacy not available without padding");
                    }

                    return false;
                }
                catch (NoSuchAlgorithmException e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(protocolClass + " privacy not available");
                    }

                    return false;
                }
                catch (InvalidAlgorithmParameterException e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(protocolClass + " privacy not available due to invalid parameter: " + e.getMessage());
                    }

                    return false;
                }
                catch (InvalidKeyException e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(protocolClass + " privacy with key length " + keyBytes + " not supported");
                    }

                    return false;
                }
            }
        }

        public abstract int MinKeyLength { get; }
        public abstract int MaxKeyLength { get; }
        public abstract int DecryptParamsLength { get; }
        public abstract OID ID { get; }
    }
}
