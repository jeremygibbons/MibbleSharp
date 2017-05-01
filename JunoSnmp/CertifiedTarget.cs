// <copyright file="CertifiedTarget.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp
{
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>CertifiedTarget</c> class implements a <see cref="SecureTarget"/>
    /// for use with <see cref="Security.SecurityModel"/>s
    /// that support secured connections using client and server certificates.
    /// </summary>
    public class CertifiedTarget : SecureTarget, ICertifiedIdentity, ISerializable
    {
        private OctetString serverFingerprint;
        private OctetString clientFingerprint;

        public CertifiedTarget(OctetString identity) : base(new TlsAddress(), identity)
        {
        }

        public CertifiedTarget(IAddress address, OctetString identity,
                               OctetString serverFingerprint, OctetString clientFingerprint)
            : base(address, identity)
        {
            this.serverFingerprint = serverFingerprint;
            this.clientFingerprint = clientFingerprint;
        }

        public OctetString ServerFingerprint
        {
            get
            {
                return serverFingerprint;
            }
        }

        public OctetString ClientFingerprint
        {
            get
            {
                return clientFingerprint;
            }
        }

        public OctetString Identity
        {
            get
            {
                return base.SecurityName;
            }
        }

        public override string ToString()
        {
            return "CertifiedTarget[" + ToStringAbstractTarget() +
                ", serverFingerprint=" + serverFingerprint +
                ", clientFingerprint=" + clientFingerprint +
                ']';
        }
    }
}
