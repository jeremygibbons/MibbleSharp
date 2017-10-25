// <copyright file="CommunityTarget.cs" company="None">
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
    using System;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /// <summary>
    /// A <code>CommunityTarget</code> represents SNMP target properties for
    /// community based message processing models(SNMPv1 and SNMPv2c).
    /// </summary>
    public class CommunityTarget : AbstractTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityTarget"/> class.
        /// </summary>
        public CommunityTarget()
        {
            this.Version = SnmpConstants.version1;
            this.SecurityLevel = Security.SecurityLevel.NoAuthNoPriv;
            this.SecurityModel = Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityTarget"/> class,
        /// fully specified.
        /// </summary>
        /// <param name="address">The transport <c>IAddress</c> of the target</param>
        /// <param name="community">The community to be used for the target</param>
        public CommunityTarget(IAddress address, OctetString community)
            : base(address, community)
        {
            this.Version = SnmpConstants.version1;
            this.SecurityLevel = Security.SecurityLevel.NoAuthNoPriv;
            this.SecurityModel = Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1;
        }

        /// <summary>
        /// Gets or sets the community octet sting. This is a convenience method to set the
        /// security name for community based SNMP(v1 and v2c). It basically checks
        /// that the community is not <c>null</c> and then sets the security name property.
        /// with the supplied parameter.
        /// </summary>
        public OctetString Community
        {
            get
            {
                return this.SecurityName;
            }

            set
            {
                this.SecurityName = value ?? throw new ArgumentException("Community must not be null");
            }
        }

        /// <summary>
        /// Gets or sets the security model.
        /// </summary>
        public override Security.SecurityModel.SecurityModelID SecurityModel
        {
            get
            {
                switch (this.Version)
                {
                    case SnmpConstants.version1:
                        return Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1;
                    default:
                        return Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv2c;
                }
            }

            set
            {
                switch (value)
                {
                    case Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv1:
                        base.SecurityModel = value;
                        base.Version = SnmpConstants.version1;
                        break;
                    case Security.SecurityModel.SecurityModelID.SECURITY_MODEL_SNMPv2c:
                        base.SecurityModel = value;
                        base.Version = SnmpConstants.version2c;
                        break;
                    default:
                        throw new NotSupportedException("To set security model for a CommunityTarget, use setVersion");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Security Level.
        /// </summary>
        public override Security.SecurityLevel SecurityLevel
        {
            get
            {
                return base.SecurityLevel;
            }

            set
            {
                if (value != Security.SecurityLevel.NoAuthNoPriv)
                {
                    throw new ArgumentException("CommunityTarget only supports SecurityLevel.NOAUTH_NOPRIV");
                }

                base.SecurityLevel = value;
            }
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "CommunityTarget[" + ToStringAbstractTarget() + ']';
        }
    }
}
