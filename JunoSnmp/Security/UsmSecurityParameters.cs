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

        private static readonly int MAX_BER_LENGTH_WITHOU_SEC_PARAMS = 32 + 2 + 6 + 6 + 32 + 2;

        private readonly OctetString authoritativeEngineID = new OctetString();
        private readonly Integer32 authoritativeEngineBoots = new Integer32();
        private readonly Integer32 authoritativeEngineTime = new Integer32();

        public OctetString UserName { get; set; } = new OctetString();
        public IAuthenticationProtocol AuthenticationProtocol { get; set; }
        public IPrivacyProtocol PrivacyProtocol { get; set; }
        public byte[] AuthenticationKey { get; set; }
        public byte[] PrivacyKey {get; set; }
        public OctetString PrivacyParameters { get; set; } = new OctetString();
        public OctetString AuthenticationParameters { get; set; } = new OctetString();
        public int SecurityParametersPosition { get; set; }  = -1;

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
            this.PrivacyProtocol = privacyProtocol;
            this.UserName = userName;
            this.AuthenticationProtocol = authenticationProtocol;
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
            byte type;
            int octetLength = BER.DecodeHeader(inputStream, out type);
            long startPos = inputStream.Position;

            if (type != BER.OCTETSTRING)
            {
                string txt =
                    "BER decoding error: Expected BER OCTETSTRING but found: " +
                    type;
                log.Warn(txt);
                throw new IOException(txt);
            }

            sequencePosition = (int)inputStream.Position;
            int length = BER.DecodeHeader(inputStream, out type);
            long startPosSeq = inputStream.Position;

            if (type != BER.SEQUENCE)
            {
                string txt =
                    "BER decoding error: Expected BER SEQUENCE but found: " +
                    type;
                log.Warn(txt);
                throw new IOException(txt);
            }

            authoritativeEngineID.DecodeBER(inputStream);
            authoritativeEngineBoots.DecodeBER(inputStream);
            authoritativeEngineTime.DecodeBER(inputStream);
            UserName.DecodeBER(inputStream);
            this.authParametersPosition = (int)(inputStream.Position - pos);
            pos = (int)inputStream.Position;
            this.AuthenticationParameters.DecodeBER(inputStream);
            this.authParametersPosition +=
                ((int)inputStream.Position - pos) - this.AuthenticationParameters.BERPayloadLength;

            PrivacyParameters.DecodeBER(inputStream);
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
                    (this.AuthenticationParameters.BERPayloadLength +
                     PrivacyParameters.BERLength);
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
            UserName.EncodeBER(outputStream);
            this.AuthenticationParameters.EncodeBER(outputStream);
            PrivacyParameters.EncodeBER(outputStream);
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
                length += UserName.BERLength;
                length += this.AuthenticationParameters.BERLength;
                length += PrivacyParameters.BERLength;
                return length;
            }
        }

        public int GetBERMaxLength(int securityLevel)
        {
            SecurityProtocols secProtocol = SecurityProtocols.GetInstance();

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
