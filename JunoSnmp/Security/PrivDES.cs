// <copyright file="PrivDES.cs" company="None">
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
    using JunoSnmp.SMI;

    /// <summary>
    /// Privacy protocol class for DES.
    /// <para>
    /// This class uses DES in CBC mode to encrypt the data.The protocol
    /// is defined in the IETF standard "User-based Security Model (USM)
    /// for SNMPv3".
    /// </para>
    /// </summary>
    public class PrivDES : PrivacyGeneric
    {
        /// <summary>
        /// Unique ID of this privacy protocol.
        /// </summary>
        private static readonly OID protocolOid = new OID("1.3.6.1.6.3.10.1.2.2");

        private static readonly String PROTOCOL_ID = "DES/CBC/NoPadding";
        private static readonly String PROTOCOL_CLASS = "DES";
        private static readonly int DECRYPT_PARAMS_LENGTH = 8;
        private static readonly int INIT_VECTOR_LENGTH = 8;
        private static readonly int INPUT_KEY_LENGTH = 16;
        private static readonly int KEY_LENGTH = 8;

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PrivDES()
        {
            this.initVectorLength = INIT_VECTOR_LENGTH;
            this.protocolId = PROTOCOL_ID;
            this.protocolClass = PROTOCOL_CLASS;
            this.keyBytes = KEY_LENGTH;
            this.salt = Salt.GetInstance();
            this.cipherPool = new CipherPool();
        }

        public byte[] encrypt(byte[] unencryptedData,
                              int offset,
                              int length,
                              byte[] encryptionKey,
                              long engineBoots,
                              long engineTime,
                              DecryptParams decryptParams)
        {
            int mySalt = (int)salt.Next();

            if (encryptionKey.Length < INPUT_KEY_LENGTH)
            {
                log.Error("Wrong Key length: need at least 16 bytes, is " +
                             encryptionKey.Length +
                             " bytes.");
                throw new ArgumentException("encryptionKey has illegal length "
                                                   + encryptionKey.Length
                                                   + " (should be at least 16).");
            }

            if ((decryptParams.Array == null) || (decryptParams.Length < 8))
            {
                decryptParams.Array = new byte[8];
            }
            decryptParams.Length = 8;
            decryptParams.Offset = 0;

            // put salt in decryption_params (sent as priv params)
            if (log.IsDebugEnabled)
            {
                log.Debug("Preparing decrypt_params.");
            }
            for (int i = 0; i < 4; ++i)
            {
                decryptParams.Array[3 - i] = (byte)(0xFF & (engineBoots >> (8 * i)));
                decryptParams.Array[7 - i] = (byte)(0xFF & (mySalt >> (8 * i)));
            }

            byte[] iv = new byte[8];

            // last eight bytes of key xored with decrypt params are used as iv
            if (log.IsDebugEnabled)
            {
                log.Debug("Preparing iv for encryption.");
            }
            for (int i = 0; i < 8; ++i)
            {
                iv[i] = (byte)(encryptionKey[8 + i] ^ decryptParams.array[i]);
            }

            byte[] encryptedData = null;

            try
            {
                // now do CBC encryption of the plaintext
                Cipher alg = doInit(encryptionKey, iv);
                encryptedData = DoFinalWithPadding(unencryptedData, offset, length, alg);
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

            if (log.IsDebugEnabled)
            {
                log.Debug("Encryption finished.");
            }
            return encryptedData;
        }

        /**
         * Decrypts a message using a given decryption key, engine boots count, and
         * engine ID.
         *
         * @param cryptedData
         *    the data to decrypt.
         * @param offset
         *    the offset into <code>cryptedData</code> to start decryption.
         * @param length
         *    the length of the data to decrypt.
         * @param decryptionKey
         *    the decryption key.
         * @param engineBoots
         *    the engine boots counter.
         * @param engineTime
         *    the engine time value.
         * @return
         *    the decrypted data, or <code>null</code> if decryption failed.
         */
        public byte[] decrypt(byte[] cryptedData,
                              int offset,
                              int length,
                              byte[] decryptionKey,
                              long engineBoots,
                              long engineTime,
                              DecryptParams decryptParams)
        {
            if ((length % 8 != 0) ||
                (length < 8) ||
                (decryptParams.Length != 8))
            {
                throw new ArgumentException(
                    "Length (" + length +
                    ") is not multiple of 8 or decrypt params has not length 8 ("
                    + decryptParams.Length + ").");
            }
            if (decryptionKey.Length < INPUT_KEY_LENGTH)
            {
                log.Error("Wrong Key length: need at least 16 bytes, is " +
                             decryptionKey.Length +
                             " bytes.");
                throw new ArgumentException("decryptionKey has illegal length "
                                                   + decryptionKey.Length
                                                   + " (should be at least 16).");
            }

            byte[] iv = new byte[8];

            // last eight bytes of key xored with decrypt params are used as iv
            for (int i = 0; i < 8; ++i)
            {
                iv[i] = (byte)(decryptionKey[8 + i] ^ decryptParams.Array[i]);
            }

            byte[] decryptedData = doDecrypt(cryptedData, offset, length, decryptionKey, iv);
            return decryptedData;
        }

        /**
         * Gets the OID uniquely identifying the privacy protocol.
         * @return
         *    an <code>OID</code> instance.
         */
        public OID ID
        {
            get
            {
                return (OID)protocolOid.Clone();
            }
        }

        public bool IsSupported
        {
            get
            {
                Cipher alg;
                try
                {
                    alg = cipherPool.reuseCipher();
                    if (alg == null)
                    {
                        Cipher.getInstance("DESede/CBC/NoPadding");
                    }
                    return true;
                }
                catch (NoSuchPaddingException e)
                {
                    return false;
                }
                catch (NoSuchAlgorithmException e)
                {
                    return false;
                }
            }
        }

        public int GetEncryptedLength(int scopedPDULength)
        {
            if (scopedPDULength % 8 == 0)
            {
                return scopedPDULength;
            }
            return 8 * ((scopedPDULength / 8) + 1);
        }

        public int MinKeyLength
        {
            get
            {
                return 16;
            }
        }

        public int DecryptParamsLength
        {
            get
            {
                return DECRYPT_PARAMS_LENGTH;
            }
        }

        public int MaxKeyLength
        {
            get
            {
                return this.MinKeyLength;
            }
        }

        public byte[] ExtendShortKey(byte[] shortKey, OctetString password,
                                     byte[] engineID,
                                     IAuthenticationProtocol authProtocol)
        {
            return shortKey;
        }
    }
}
