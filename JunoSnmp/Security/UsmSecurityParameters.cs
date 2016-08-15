// <copyright file="UsmSecurityParameters.cs" company="None">
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
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;

    public class UsmSecurityParameters : ISecurityParameters
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int MAX_BER_LENGTH_WITHOU_SEC_PARAMS =
      32 + 2 + 6 + 6 + 32 + 2;

        private OctetString authoritativeEngineID = new OctetString();
        private Integer32 authoritativeEngineBoots = new Integer32();
        private Integer32 authoritativeEngineTime = new Integer32();
        private OctetString userName = new OctetString();
        private IAuthenticationProtocol authenticationProtocol = null;
        private IPrivacyProtocol privacyProtocol = null;
        private byte[] authenticationKey;
        private byte[] privacyKey;
        private OctetString privacyParameters = new OctetString();
        private OctetString authenticationParameters = new OctetString();
        private int securityParametersPosition = -1;
        private int authParametersPosition = -1;
        private int decodedLength = -1;
        private int sequencePosition = -1;

        public UsmSecurityParameters()
        {
        }

        public UsmSecurityParameters(OctetString authoritativeEngineID,
                                     Integer32 authoritativeEngineBoots,
                                     Integer32 authoritativeEngineTime,
                                     OctetString userName,
                                     IAuthenticationProtocol authenticationProtocol,
                                     IPrivacyProtocol privacyProtocol)
        {
            this.authoritativeEngineID = authoritativeEngineID;
            this.authoritativeEngineBoots = authoritativeEngineBoots;
            this.authoritativeEngineTime = authoritativeEngineTime;
            this.privacyProtocol = privacyProtocol;
            this.userName = userName;
            this.authenticationProtocol = authenticationProtocol;
        }


        public byte[] AuthoritativeEngineID
        {
            get
            {
                return this.authoritativeEngineID.GetValue();
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Authoritative engine ID must not be null");
                }

                this.authoritativeEngineID.SetValue(value);
            }
        }

        public int AuthoritativeEngineBoots
        {
            get
            {
                return this.authoritativeEngineBoots.GetValue();
            }

            set
            {
                this.authoritativeEngineBoots.SetValue(value);
            }
        }

        public int AuthoritativeEngineTime
        {
            get
            {
                return this.authoritativeEngineTime.GetValue();
            }

            set
            {
                this.authoritativeEngineTime.SetValue(value);
            }
        }

        public OctetString UserName
        {
            get
            {
                return userName;
            }

            set
            {
                this.userName = value;
            }
        }

        public IAuthenticationProtocol AuthenticationProtocol
        {
            get
            {
                return this.authenticationProtocol;
            }

            set
            {
                this.authenticationProtocol = value;
            }
        }

        public IPrivacyProtocol PrivacyProtocol
        {
            get
            {
                return this.privacyProtocol;
            }

            set
            {
                this.privacyProtocol = value;
            }
        }

        public int BERLength
        {
            get
            {
                int length = BERPayloadLength;
                return length + BER.GetBERLengthOfLength(length) + 1;
            }
        }

        public int BERPayloadLength
        {
            get
            {
                int length = this.BERUsmPayloadLength;
                length += BER.GetBERLengthOfLength(length) + 1;
                return length;
            }
        }

        public void DecodeBER(BERInputStream inputStream)
        {
            int pos = (int)inputStream.Position;
            this.decodedLength = pos;
            BER.MutableByte mutableByte = new BER.MutableByte();
            int octetLength = BER.DecodeHeader(inputStream, out mutableByte);
            long startPos = inputStream.Position;
            if (mutableByte.Value != BER.OCTETSTRING)
            {
                string txt =
                    "BER decoding error: Expected BER OCTETSTRING but found: " +
                    mutableByte.Value;
                log.Warn(txt);
                throw new IOException(txt);
            }
            sequencePosition = (int)inputStream.Position;
            int length = BER.DecodeHeader(inputStream, out mutableByte);
            long startPosSeq = inputStream.Position;
            if (mutableByte.Value != BER.SEQUENCE)
            {
                string txt =
                    "BER decoding error: Expected BER SEQUENCE but found: " +
                    mutableByte.Value;
                log.Warn(txt);
                throw new IOException(txt);
            }
            authoritativeEngineID.DecodeBER(inputStream);
            authoritativeEngineBoots.DecodeBER(inputStream);
            authoritativeEngineTime.DecodeBER(inputStream);
            userName.DecodeBER(inputStream);
            this.authParametersPosition = (int)(inputStream.Position - pos);
            pos = (int)inputStream.Position;
            authenticationParameters.DecodeBER(inputStream);
            this.authParametersPosition +=
                ((int)inputStream.Position - pos) - authenticationParameters.BERPayloadLength;

            privacyParameters.DecodeBER(inputStream);
            this.decodedLength = (int)(inputStream.Position - decodedLength);
            if (BER.CheckSequenceLengthFlag)
            {
                // check length
                BER.CheckSequenceLength(length,
                                        (int)(inputStream.Position - startPosSeq),
                                        this);
                BER.CheckSequenceLength(octetLength,
                                        (int)(inputStream.Position - startPos),
                                        this);
            }
        }

        private int BEREncodedAuthParamsPosition
        {
            get
            {
                return this.BERLength -
                    (this.authenticationParameters.BERPayloadLength +
                     privacyParameters.BERLength);
            }
        }

        public int SequencePosition
        {
            get
            {
                return sequencePosition;
            }
        }

        public void EncodeBER(Stream outputStream)
        {
            BER.EncodeHeader(outputStream, BER.OCTETSTRING, this.BERPayloadLength);
            BER.EncodeHeader(outputStream, BER.SEQUENCE, BERUsmPayloadLength);
            authoritativeEngineID.EncodeBER(outputStream);
            authoritativeEngineBoots.EncodeBER(outputStream);
            authoritativeEngineTime.EncodeBER(outputStream);
            userName.EncodeBER(outputStream);
            authenticationParameters.EncodeBER(outputStream);
            privacyParameters.EncodeBER(outputStream);
        }

        /**
         * getBERUsmPayloadLength
         *
         * @return int
         */
        public int BERUsmPayloadLength
        {
            get
            {
                int length = authoritativeEngineID.BERLength;
                length += authoritativeEngineBoots.BERLength;
                length += authoritativeEngineTime.BERLength;
                length += userName.BERLength;
                length += authenticationParameters.BERLength;
                length += privacyParameters.BERLength;
                return length;
            }
        }

        public int GetBERMaxLength(int securityLevel)
        {
            SecurityProtocols secProtocol = SecurityProtocols.Instance;
            int securityParamsLength = 2;
            if (securityLevel > (int)SecurityLevel.NoAuthNoPriv)
            {
                securityParamsLength = secProtocol.MaxAuthDigestLength +
                    BER.GetBERLengthOfLength(secProtocol.MaxAuthDigestLength) + 1;

                if (securityLevel == (int)SecurityLevel.AuthPriv)
                {
                    securityParamsLength += secProtocol.MaxPrivDecryptParamsLength +
                        BER.GetBERLengthOfLength(secProtocol.MaxPrivDecryptParamsLength)
                        + 1;
                }
            }
            return MAX_BER_LENGTH_WITHOU_SEC_PARAMS + securityParamsLength +
                BER.GetBERLengthOfLength(MAX_BER_LENGTH_WITHOU_SEC_PARAMS +
                                         securityParamsLength) + 1;
        }

        public byte[] AuthenticationKey
        {
            get
            {
                return this.authenticationKey;
            }

            set
            {
                this.authenticationKey = value;
            }
        }

        public byte[] PrivacyKey
        {
            get
            {
                return this.privacyKey;
            }

            set
            {
                this.privacyKey = value;
            }
        }

        public OctetString PrivacyParameters
        {
            get
            {
                return this.privacyParameters;
            }

            set
            {
                this.privacyParameters = value;
            }
        }

        public OctetString AuthenticationParameters
        {
            get
            {
                return this.authenticationParameters;
            }

            set
            {
                this.authenticationParameters = value;
            }
        }

        public int SecurityParametersPosition
        {
            get
            {
                return this.securityParametersPosition;
            }

            set
            {
                this.securityParametersPosition = value;
            }
        }

        public int AuthParametersPosition
        {
            get
            {
                if (this.authParametersPosition >= 0)
                {
                    return this.authParametersPosition;
                }
                else
                {
                    return this.BEREncodedAuthParamsPosition;
                }
            }
        }

        /**
         * getScopedPduPosition
         *
         * @return int
         */
        public int ScopedPduPosition
        {
            get
            {
                if (this.decodedLength >= 0)
                {
                    return this.decodedLength + this.SecurityParametersPosition;
                }
                else
                {
                    return this.SecurityParametersPosition + this.BERLength;
                }
            }
        }
    }
}
