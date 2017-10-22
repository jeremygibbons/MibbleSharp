// <copyright file="AuthGeneric.cs" company="None">
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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using JunoSnmp.SMI;
    /// <summary>
    /// The abstract class AuthGeneric implements common operations for
    /// SNMP authentication protocols, such as MD5 and SHA.
    /// </summary>
    public abstract class AuthGeneric : IAuthenticationProtocol
    {
        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int HmacBlockSize = 64;
        private static int DefaultAuthenticationCodeLength = 12;

        private int hmacBlockSize;
        private int authenticationCodeLength;
        private int hashLength;
        private string protoName;
        
        /// <summary>
        /// Creates an authentication protocol with the specified name (ID) and digest length and using the
        /// <see cref="DefaultAuthenticationCodeLength"/> default code length.
        /// </summary>
        /// <param name="protoName">
        /// The name (ID) of the authentication protocol. Only names that are supported by
        /// the underlying platform may be used
        /// </param>
        /// <param name="hashLength">The hash length in bytes</param>
        public AuthGeneric(string protoName, int hashLength)
        {
            this.hmacBlockSize = HmacBlockSize;
            this.authenticationCodeLength = DefaultAuthenticationCodeLength;
            this.protoName = protoName;
            this.hashLength = hashLength;
        }

        /**
         * Creates an authentication protocol with the specified name (ID) and digest length and using the
         * {@link #DEFAULT_AUTHENTICATION_CODE_LENGTH} default code length.
         * @param protoName
         *   the name (ID) of the authentication protocol. Only names that are supported by the used
         *   security provider can be used.
         * @param digestLength
         *   the digest length.
         * @param authenticationCodeLength
         *   the length of the hash output (i.e., the authentication code length).
         * @since 2.4.0
         */
        public AuthGeneric(string protoName, int digestLength, int authenticationCodeLength)
                : this(protoName, digestLength)
        {
            this.authenticationCodeLength = authenticationCodeLength;
        }

        /// <summary>
        /// Gets the length of the message digest used by this authentication protocol.
        /// </summary>
        public int HashLength
        {
            get
            {
                return this.hashLength;
            }
        }

        /// <summary>
        /// Gets the length of the authentication code (the hashing output length) in octets.
        /// </summary>
        public int AuthenticationCodeLength
        {
            get
            {
                return this.authenticationCodeLength;
            }
        }
        
        /// <summary>
        /// Get a fresh HashAlgorithm object of for the hashing method specified in the
        /// constructor.
        /// </summary>
        /// <returns>A new HashAlgorithm object</returns>
        /// <exception cref="ArgumentException">
        /// If the specified algorithm is not supported
        /// </exception>
        protected HashAlgorithm CreateHashAlgorithm()
        {
            HashAlgorithm ha = HashAlgorithm.Create(this.protoName);
            
            if(ha == null)
            {
                throw new ArgumentException(this.protoName + " not supported");
            }

            return ha;
        }

        public bool Supported
        {
            get
            {
                try
                {
                    this.CreateHashAlgorithm();
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }
        }

        public abstract OID ID { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationKey"></param>
        /// <param name="message"></param>
        /// <param name="messageOffset"></param>
        /// <param name="messageLength"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool Authenticate(
            byte[] authenticationKey,
            byte[] message,
            int messageOffset,
            int messageLength,
            ByteArrayWindow hash)
        {
            byte[] authKey = authenticationKey;

            byte[] newHash;
            byte[] k_ipad = new byte[this.hmacBlockSize]; // inner padding - key XORd with ipad
            byte[] k_opad = new byte[this.hmacBlockSize]; // outer padding - key XORd with opad

            // clear the bytes for the digest
            for (int i = 0; i < authenticationCodeLength; ++i)
            {
                hash[i] = 0;
            }

            HashAlgorithm ha = this.CreateHashAlgorithm();

            if (authKey.Length > hmacBlockSize)
            {
                authKey = ha.ComputeHash(authenticationKey);
            }

            /*
             * the HMAC_MD transform looks like:
             *
             * MD(K XOR opad, MD(K XOR ipad, msg))
             *
             * where K is an n byte key
             * ipad is the byte 0x36 repeated 64 times
             * opad is the byte 0x5c repeated 64 times
             * and text is the data being protected
             */

            
            // start out by storing key, ipad and opad in pads
            for (int i = 0; i < authKey.Length; ++i)
            {
                k_ipad[i] = (byte)(authKey[i] ^ 0x36);
                k_opad[i] = (byte)(authKey[i] ^ 0x5c);
            }

            for (int i = authKey.Length; i < hmacBlockSize; ++i)
            {
                k_ipad[i] = 0x36;
                k_opad[i] = 0x5c;
            }

            ha = this.CreateHashAlgorithm();
            /* perform inner MD */
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, ha, CryptoStreamMode.Write))
                {
                    cs.Write(k_ipad, 0, k_ipad.Length);/* start with inner pad      */
                    cs.Write(message, messageOffset, messageLength); /* then text of msg  */
                    cs.FlushFinalBlock();
                    newHash = ha.Hash; /* finish up 1st pass        */
                }
            }

            ha = this.CreateHashAlgorithm();
            /* perform outer MD */
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, ha, CryptoStreamMode.Write))
                {
                    cs.Write(k_opad, 0, k_opad.Length);/* start with outer pad */
                    cs.Write(newHash, 0, newHash.Length); /* then results of 1st hash */
                    cs.FlushFinalBlock();
                    newHash = ha.Hash; /* finish up 2nd pass */
                }
            }
            
            // copy the digest into the message (authenticationCodeLength bytes only!)
            for (int i = 0; i < authenticationCodeLength; ++i)
            {
                hash[i] = newHash[i];
            }

            return true;
        }

        public bool IsAuthentic(
            byte[] authenticationKey,
            byte[] message,
            int messageOffset,
            int messageLength,
            ByteArrayWindow hash)
        {
            // copy digest from message
            ByteArrayWindow origHash = new ByteArrayWindow(
                new byte[authenticationCodeLength],
                0,
                authenticationCodeLength);

            System.Array.Copy(
                hash.Value, 
                hash.Offset,
                origHash.Value, 
                0,
                authenticationCodeLength);

            // use the authenticate() method to recalculate the digest
            if (!Authenticate(
                authenticationKey, 
                message, 
                messageOffset,
                messageLength, 
                hash))
            {
                return false;
            }

            return hash.Equals(origHash, authenticationCodeLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public byte[] ChangeDelta(byte[] oldKey,
                                  byte[] newKey,
                                  byte[] random)
        {
            // algorithm according to USM-document textual convention KeyChange
            // works with SHA and MD5
            // modifications needed to support variable length keys
            // algorithm according to USM-document textual convention KeyChange
            HashAlgorithm ha = this.CreateHashAlgorithm();

            int hashLength = ha.HashSize / 8; // Note: HashSize is in bits

            if (log.IsDebugEnabled)
            {
                log.Debug(protoName + "oldKey: " +
                             new OctetString(oldKey).ToHexString());
                log.Debug(protoName + "newKey: " +
                             new OctetString(newKey).ToHexString());
                log.Debug(protoName + "random: " +
                             new OctetString(random).ToHexString());
            }

            int iterations = (oldKey.Length - 1) / hashLength;

            OctetString tmp = new OctetString(oldKey);
            OctetString delta = new OctetString();

            for (int k = 0; k < iterations; k++)
            {
                tmp.Append(random);
                tmp.SetValue(ha.ComputeHash(tmp.GetValue()));
                delta.Append(new byte[hashLength]);
                for (int kk = 0; kk < hashLength; kk++)
                {
                    delta[k * hashLength + kk] = (byte)(tmp[kk] ^ newKey[k * hashLength + kk]);
                }
            }

            tmp.Append(random);

            tmp = new OctetString(ha.ComputeHash(tmp.GetValue()), 0, oldKey.Length - delta.Length);
            
            for (int j = 0; j < tmp.Length; j++)
            {
                tmp[j] = (byte)(tmp[j] ^ newKey[iterations * hashLength + j]);
            }

            byte[] keyChange = new byte[random.Length + delta.Length + tmp.Length];

            System.Array.Copy(random, 0, keyChange, 0, random.Length);

            System.Array.Copy(delta.GetValue(), 0, keyChange,
                             random.Length, delta.Length);

            System.Array.Copy(tmp.GetValue(), 0, keyChange,
                             random.Length + delta.Length, tmp.Length);

            if (log.IsDebugEnabled)
            {
                log.Debug(protoName + "keyChange:" +
                             new OctetString(keyChange).ToHexString());
            }

            return keyChange;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordString"></param>
        /// <param name="engineID"></param>
        /// <returns></returns>
        public byte[] PasswordToKey(OctetString passwordString, byte[] engineID)
        {

            HashAlgorithm ha = this.CreateHashAlgorithm();

            byte[] hash;
            byte[] buf = new byte[hmacBlockSize];
            int password_index = 0;
            int count = 0;
            byte[] password = passwordString.GetValue();

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, ha, CryptoStreamMode.Write))
                {
                    /* Use while loop until we've done 1 Megabyte */
                    while (count < 1048576)
                    {
                        for (int i = 0; i < hmacBlockSize; ++i)
                        {
                            /* Take the next octet of the password, wrapping */
                            /* to the beginning of the password as necessary.*/
                            buf[i] = password[password_index++ % password.Length];
                        }
                        cs.Write(buf, 0, buf.Length);
                        count += hmacBlockSize;
                    }

                    cs.FlushFinalBlock();
                    hash = ha.Hash;
                }
            }
                    

            if (log.IsDebugEnabled)
            {
                log.Debug(protoName + "First digest: " +
                             new OctetString(hash).ToHexString());
            }

            /*****************************************************/
            /* Now localize the key with the engine_id and pass  */
            /* through Hash Algorithm to produce final key       */
            /*****************************************************/
            ha = this.CreateHashAlgorithm();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, ha, CryptoStreamMode.Write))
                {
                    cs.Write(hash, 0, hash.Length);
                    cs.Write(engineID, 0, engineID.Length);
                    cs.Write(hash, 0, hash.Length);

                    cs.FlushFinalBlock();
                }

                hash = ha.Hash;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug(protoName + "localized key: " +
                             new OctetString(hash).ToHexString());
            }

            return hash;
        }

        public byte[] Hash(byte[] data)
        {
            HashAlgorithm ha = this.CreateHashAlgorithm();
            return ha.ComputeHash(data);
        }

        public byte[] Hash(byte[] data, int offset, int length)
        {
            HashAlgorithm ha = this.CreateHashAlgorithm();
            return ha.ComputeHash(data, offset, length);
        }

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}
