// <copyright file="UsmSecurityStateReference.cs" company="None">
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
    /// <summary>
    /// The <code>UsmSecurityStateReference</code> holds cached security data
    /// for the <see cref="USM"/> security model.
    /// </summary>
    public class UsmSecurityStateReference : ISecurityStateReference
    {
        private byte[] userName;
        private byte[] securityName;
        private byte[] securityEngineID;
        private IAuthenticationProtocol authenticationProtocol;
        private IPrivacyProtocol privacyProtocol;
        private byte[] authenticationKey;
        private byte[] privacyKey;
        private int securityLevel;

        public UsmSecurityStateReference()
        {
        }

        public byte[] UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.userName = value;
            }
        }

        public byte[] SecurityName
        {
            get
            {
                return this.securityName;
            }

            set
            {
                this.securityName = value;
            }
        }

        public byte[] SecurityEngineID
        {
            get
            {
                return this.securityEngineID;
            }

            set
            {
                this.securityEngineID = value;
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

        public int SecurityLevel
        {
            get
            {
                return this.securityLevel;
            }

            set
            {
                this.securityLevel = value;
            }
        }
    }
}
