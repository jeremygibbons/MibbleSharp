// <copyright file="PrivAES.cs" company="None">
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
    using JunoSnmp.SMI;

    /// <summary>
    /// Base class for PrivAES128, PrivAES192 and PrivAES256.
    /// <para>
    /// This class uses AES in CFB mode to encrypt the data. The protocol
    /// is defined in draft-blumenthal-aes-usm-08.txt(AES128) and
    /// draft-blumenthal-aes-usm-04.txt(AES192 and AES256).
    /// </para>
    /// </summary>
    public abstract class PrivAES : PrivacyGeneric, IPrivacyProtocol
    {

        private static readonly string PROTOCOL_ID = "AES/CFB/NoPadding";
        private static readonly string PROTOCOL_CLASS = "AES";
        private static readonly int DECRYPT_PARAMS_LENGTH = 8;
        private static readonly int INIT_VECTOR_LENGTH = 16;

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /**
         * Constructor.
         *
         * @param keyBytes
         *    Length of key, must be 16, 24 or 32.
         * @throws IllegalArgumentException
         *    if keyBytes is illegal
         */
        protected PrivAES(int keyBytes)
        {
            this.initVectorLength = PrivAES.INIT_VECTOR_LENGTH;
            this.protocolId = PrivAES.PROTOCOL_ID;
            this.protocolClass = PrivAES.PROTOCOL_CLASS;
            if ((keyBytes != 16) && (keyBytes != 24) && (keyBytes != 32))
            {
                throw new ArgumentException(
                    "Only 128, 192 and 256 bit AES is allowed. Requested ("
                    + (8 * keyBytes) + ").");
            }
            this.keyBytes = keyBytes;
            this.salt = Salt.GetInstance();
        }

        public override byte[] Encrypt(byte[] unencryptedData, int offset, int length,
                              byte[] encryptionKey, long engineBoots,
                              long engineTime, DecryptParams decryptParams)
        {

            byte[] initVect = new byte[PrivAES.INIT_VECTOR_LENGTH];
            long my_salt = salt.Next();

            if (encryptionKey.Length != keyBytes)
            {
                throw new ArgumentException(
                    "Needed key length is " + keyBytes +
                    ". Got " + encryptionKey.Length +
                    ".");
            }

            if ((decryptParams.Array == null) ||
                (decryptParams.Length < PrivAES.DECRYPT_PARAMS_LENGTH))
            {
                decryptParams.Array = new byte[PrivAES.DECRYPT_PARAMS_LENGTH];
            }
            decryptParams.Length = PrivAES.DECRYPT_PARAMS_LENGTH;
            decryptParams.Offset = 0;

            /* Set IV as engine_boots + engine_time + salt */
            initVect[0] = (byte)((engineBoots >> 24) & 0xFF);
            initVect[1] = (byte)((engineBoots >> 16) & 0xFF);
            initVect[2] = (byte)((engineBoots >> 8) & 0xFF);
            initVect[3] = (byte)((engineBoots) & 0xFF);
            initVect[4] = (byte)((engineTime >> 24) & 0xFF);
            initVect[5] = (byte)((engineTime >> 16) & 0xFF);
            initVect[6] = (byte)((engineTime >> 8) & 0xFF);
            initVect[7] = (byte)((engineTime) & 0xFF);
            for (int i = 56, j = 8; i >= 0; i -= 8, j++)
            {
                initVect[j] = (byte)((my_salt >> i) & 0xFF);
            }
            System.Array.Copy(initVect, 8, decryptParams.Array, 0, 8);
            if (log.IsDebugEnabled)
            {
                log.Debug("initVect is " + AsHex(initVect));
            }

            // allocate space for encrypted text
            byte[] encryptedData = null;
            try
            {
                encryptedData = DoEncrypt(encryptionKey, initVect, unencryptedData, offset, length);

                if (log.IsDebugEnabled)
                {
                    log.Debug("aes encrypt: Data to encrypt " + PrivAES.AsHex(unencryptedData));

                    log.Debug("aes encrypt: used key " + PrivAES.AsHex(encryptionKey));

                    log.Debug("aes encrypt: created privacy_params " +
                                 PrivAES.AsHex(decryptParams.Array));

                    log.Debug("aes encrypt: encrypted Data  " +
                                 AsHex(encryptedData));
                }
            }
            catch (Exception e)
            {
                log.Error("Encrypt Exception " + e);
            }

            return encryptedData;
        }

        public override byte[] Decrypt(byte[] cryptedData, int offset, int length,
                              byte[] decryptionKey, long engineBoots, long engineTime,
                              DecryptParams decryptParams)
        {

            byte[] initVect = new byte[16];

            if (decryptionKey.Length != keyBytes)
            {
                throw new ArgumentException(
                    "Needed key length is " + keyBytes +
                    ". Got " + decryptionKey.Length +
                    ".");
            }

            /* Set IV as engine_boots + engine_time + decrypt params */
            initVect[0] = (byte)((engineBoots >> 24) & 0xFF);
            initVect[1] = (byte)((engineBoots >> 16) & 0xFF);
            initVect[2] = (byte)((engineBoots >> 8) & 0xFF);
            initVect[3] = (byte)((engineBoots) & 0xFF);
            initVect[4] = (byte)((engineTime >> 24) & 0xFF);
            initVect[5] = (byte)((engineTime >> 16) & 0xFF);
            initVect[6] = (byte)((engineTime >> 8) & 0xFF);
            initVect[7] = (byte)((engineTime) & 0xFF);
            System.Array.Copy(decryptParams.Array, decryptParams.Offset, initVect, 8, 8);
            if (log.IsDebugEnabled)
            {
                log.Debug("initVect is " + PrivAES.AsHex(initVect));
            }

            return DoDecrypt(cryptedData, offset, length, decryptionKey, initVect);
        }

        public override int GetEncryptedLength(int scopedPDULength)
        {
            return scopedPDULength;
        }

        /**
         * Turns array of bytes into string
         *
         * @param buf	Array of bytes to convert to hex string
         * @return	Generated hex string
         */
        public static string AsHex(byte[] buf)
        {
            return new OctetString(buf).ToHexString();
        }

        public abstract override OID ID
        {
            get;
        }

        public override int MinKeyLength
        {
            get
            {
                return this.keyBytes;
            }
        }

        public override int MaxKeyLength
        {
            get
            {
                return this.MinKeyLength;
            }
        }

        public override int DecryptParamsLength
        {
            get
            {
                return PrivAES.DECRYPT_PARAMS_LENGTH;
            }
        }

        public override byte[] ExtendShortKey(byte[] shortKey, OctetString password,
                                     byte[] engineID,
                                     IAuthenticationProtocol authProtocol)
        {
            // we have to extend the key, currently only the AES draft
            // defines this algorithm, so this may have to be changed for other
            // privacy protocols
            byte[] extKey = new byte[MinKeyLength];
            int length = shortKey.Length;
            System.Array.Copy(shortKey, 0, extKey, 0, length);

            while (length < extKey.Length)
            {
                byte[] hash = authProtocol.Hash(extKey, 0, length);

                if (hash == null)
                {
                    return null;
                }
                int bytesToCopy = extKey.Length - length;
                if (bytesToCopy > authProtocol.HashLength)
                {
                    bytesToCopy = authProtocol.HashLength;
                }
                System.Array.Copy(hash, 0, extKey, length, bytesToCopy);
                length += bytesToCopy;
            }
            return extKey;
        }

        public abstract override void GetObjectData(SerializationInfo info, StreamingContext ctx);
    }
}
