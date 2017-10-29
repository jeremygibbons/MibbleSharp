// <copyright file="Priv3DES.cs" company="None">
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
    using System.Runtime.Serialization;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;

    /**
 * Privacy protocol class for Triple DES (DESEDE).
 *
 * This class uses DES-EDE in CBC mode to encrypt the data. The protocol
 * is defined by the Internet Draft 'Extension to the User-Based Security
 * Model (USM) to Support Triple-DES EDE in "Outside" CBC Mode'.
 *
 * @author Frank Fock, Jochen Katz
 * @version 2.2.2
 * @since 1.9
 */
    public class Priv3DES : PrivacyGeneric
    {

        /**
         * Unique ID of this privacy protocol.
         */
        public static readonly OID protocolOid = new OID(SnmpConstants.usm3DESEDEPrivProtocol);

        private static readonly string PROTOCOL_ID = "TripleDES";
        private static readonly string PROTOCOL_CLASS = "DESede";
        private static readonly int DECRYPT_PARAMS_LENGTH = 8;
        private static readonly int INIT_VECTOR_LENGTH = 8;
        private static readonly int INPUT_KEY_LENGTH = 32;
        private static readonly int KEY_LENGTH = 24;

        private static readonly log4net.ILog log = log4net.LogManager
        .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Priv3DES()
        {
            base.initVectorLength = INIT_VECTOR_LENGTH;
            base.protocolId = PROTOCOL_ID;
            base.protocolClass = PROTOCOL_CLASS;
            this.cipherMode = System.Security.Cryptography.CipherMode.CBC;
            this.paddingMode = System.Security.Cryptography.PaddingMode.None;
            base.keyBytes = KEY_LENGTH;
            this.salt = Salt.GetInstance();
        }

        public override byte[] Encrypt(byte[] unencryptedData,
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
                log.Error("Wrong Key length: need at least 32 bytes, is " +
                             encryptionKey.Length +
                             " bytes.");
                throw new ArgumentException("encryptionKey has illegal length "
                                                   + encryptionKey.Length
                                                   + " (should be at least 32).");
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

            byte[] iv = new byte[INIT_VECTOR_LENGTH];

            // last eight bytes of key xored with decrypt params are used as iv
            if (log.IsDebugEnabled)
            {
                log.Debug("Preparing iv for encryption.");
            }

            for (int i = 0; i < 8; ++i)
            {
                iv[i] = (byte)(encryptionKey[KEY_LENGTH + i] ^ decryptParams.Array[i]);
            }

            byte[] encryptedData = null;

            // Use only first 24 bytes of key
            byte[] key = new byte[24];
            Buffer.BlockCopy(encryptionKey, 0, key, 0, key.Length);

            try
            {
                // now do CBC encryption of the plaintext
                if ((length % 8) == 0)
                {
                    encryptedData = DoEncrypt(key, iv, unencryptedData, offset, length);
                }
                else
                {
                    byte[] paddedData = new byte[8 * ((length / 8) + 1)];
                    Buffer.BlockCopy(unencryptedData, offset, paddedData, 0, length);
                    encryptedData = DoEncrypt(key, iv, paddedData, offset, paddedData.Length);
                }
                    
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

        public override byte[] Decrypt(byte[] cryptedData,
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
                throw new ArgumentException("Length (" + length +
                                                   ") is not multiple of 8 or decrypt " +
                                                   "params has not length 8 ("
                                                   + decryptParams.Length + ").");
            }
            if (decryptionKey.Length < INPUT_KEY_LENGTH)
            {
                log.Error("Wrong Key length: need at least 32 bytes, is " +
                             decryptionKey.Length +
                             " bytes.");
                throw new ArgumentException("decryptionKey has illegal length "
                                                   + decryptionKey.Length
                                                   + " (should be at least 32).");
            }

            byte[] iv = new byte[8];

            // last eight bytes of key xored with decrypt params are used as iv
            for (int i = 0; i < 8; ++i)
            {
                iv[i] = (byte)(decryptionKey[KEY_LENGTH + i] ^ decryptParams.Array[i]);
            }

            // Use only first 24 bytes of key
            byte[] key = new byte[24];
            Buffer.BlockCopy(decryptionKey, 0, key, 0, key.Length);

            byte[] decryptedData = DoDecrypt(cryptedData, offset, length, key, iv);
            return decryptedData;
        }

        /**
         * Gets the OID uniquely identifying the privacy protocol.
         * @return
         *    an <code>OID</code> instance.
         */
        public override OID ID
        {
            get
            {
                return (OID)protocolOid.Clone();
            }
        }

        public override int GetEncryptedLength(int scopedPDULength)
        {
            if (scopedPDULength % 8 == 0)
            {
                return scopedPDULength;
            }
            return 8 * ((scopedPDULength / 8) + 1);
        }

        public override int MinKeyLength
        {
            get
            {
                return INPUT_KEY_LENGTH;
            }
        }

        public override int DecryptParamsLength
        {
            get
            {
                return DECRYPT_PARAMS_LENGTH;
            }
        }

        public override int MaxKeyLength
        {
            get
            {
                return MinKeyLength;
            }
        }

        public override byte[] ExtendShortKey(byte[] shortKey, OctetString password,
                                 byte[] engineID,
                                 IAuthenticationProtocol authProtocol)
        {
            int length = shortKey.Length;
            byte[] extendedKey = new byte[MinKeyLength];
            System.Array.Copy(shortKey, 0, extendedKey, 0, shortKey.Length);

            byte[] key = new byte[MinKeyLength];
            System.Array.Copy(shortKey, 0, key, 0, shortKey.Length);

            while (length < MinKeyLength)
            {
                key = authProtocol.PasswordToKey(new OctetString(key, 0, length), engineID);
                int copyBytes = Math.Min(MinKeyLength - length,
                                         authProtocol.HashLength);
                System.Array.Copy(key, 0, extendedKey, length, copyBytes);
                length += copyBytes;
            }

            return extendedKey;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }

}
