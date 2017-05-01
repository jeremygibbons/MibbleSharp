// <copyright file="SNMPv3SecurityModel.cs" company="None">
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
    using System.IO;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;

    /// <summary>
    /// The abstract class <c>SNMPv3SecurityModel</c> implements
    /// common methods and fields for security models for the SNMPv3
    /// message processing model.
    /// </summary>
    public abstract class SNMPv3SecurityModel : SecurityModel
    {

        protected OctetString localEngineID;
        
        /// <summary>
        /// Gets the local engine ID.
        /// </summary>
        public OctetString LocalEngineID
        {
            get
            {
                return this.localEngineID;
            }
        }

        protected static byte[] BuildWholeMessage(
            Integer32 snmpVersion,
            byte[] scopedPdu,
            byte[] globalData,
            ISecurityParameters securityParameters)
        {
            int length =
                snmpVersion.BERLength +
                globalData.Length +
                securityParameters.BERLength +
                scopedPdu.Length;
            int totalLength = BER.GetBERLengthOfLength(length) + length + 1;

            using (MemoryStream os = new MemoryStream(totalLength))
            {
                BER.EncodeHeader(os, BER.SEQUENCE, length);
                snmpVersion.EncodeBER(os);
                os.Write(globalData, 0, globalData.Length);
                securityParameters.EncodeBER(os);
                os.Write(scopedPdu, 0, scopedPdu.Length);
                int secParamsPos = 1 + snmpVersion.BERLength +
                    BER.GetBERLengthOfLength(length) + globalData.Length;
                securityParameters.SecurityParametersPosition = secParamsPos;
                return os.ToArray();
            }
        }

        protected static byte[] BuildMessageBuffer(BERInputStream scopedPDU)
        {
            long pos = scopedPDU.Position;
            int readLengthBytes = (int)scopedPDU.Position;
            byte type;
            int length = BER.DecodeHeader(scopedPDU, out type);
            readLengthBytes = (int)scopedPDU.Position - readLengthBytes;
            byte[] buf = new byte[length + readLengthBytes];
            scopedPDU.Position = pos;

            int offset = 0;
            long avail = scopedPDU.Available;
            while ((offset < buf.Length) && (avail > 0))
            {
                int read = scopedPDU.Read(buf, offset, buf.Length - offset);
                if (read < 0)
                {
                    break;
                }

                offset += read;
            }

            return buf;
        }
    }
}
