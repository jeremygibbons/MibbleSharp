// <copyright file="TransportStateReference.cs" company="None">
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
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>TransportStateReference</c> class holds information defined by
    /// RFC 5343 for the <c>tmStateReference</c> ASI elements. Objects of this
    /// class are cached by security aware <see cref="ITransportMapping{A}"/>s and
    /// transport aware <see cref="SecurityModel"/>s.
    /// </summary>
    public class TransportStateReference
    {
        private ITransportMapping<IAddress> transport;
        private IAddress address;
        private OctetString securityName;
        private SecurityLevel requestedSecurityLevel;
        private SecurityLevel transportSecurityLevel;
        private bool sameSecurity;
        private object sessionID;
        private ICertifiedIdentity certifiedIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStateReference"/> class.
        /// </summary>
        /// <param name="transport">The transport mapping</param>
        /// <param name="address">The address</param>
        /// <param name="securityName">The security name</param>
        /// <param name="requestedSecurityLevel">The requested <see cref="SecurityLevel"/></param>
        /// <param name="transportSecurityLevel">The transport <see cref="SecurityLevel"/></param>
        /// <param name="sameSecurity">A value indicating whether </param>
        /// <param name="sessionID">A session identifier</param>
        public TransportStateReference(
            ITransportMapping<IAddress> transport,
            IAddress address,
            OctetString securityName,
            SecurityLevel requestedSecurityLevel,
            SecurityLevel transportSecurityLevel,
            bool sameSecurity,
            object sessionID)
        {
            this.transport = transport;
            this.address = address;
            this.securityName = securityName;
            this.requestedSecurityLevel = requestedSecurityLevel;
            this.transportSecurityLevel = transportSecurityLevel;
            this.sameSecurity = sameSecurity;
            this.sessionID = sessionID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransportStateReference"/> class.
        /// </summary>
        /// <param name="transport">The transport mapping</param>
        /// <param name="address">The address</param>
        /// <param name="securityName">The security name</param>
        /// <param name="requestedSecurityLevel">The requested <see cref="SecurityLevel"/></param>
        /// <param name="transportSecurityLevel">The transport <see cref="SecurityLevel"/></param>
        /// <param name="sameSecurity">A value indicating whether </param>
        /// <param name="sessionID">A session identifier</param>
        /// <param name="certifiedIdentity">A certified identity</param>
        public TransportStateReference(
            ITransportMapping<IAddress> transport,
            IAddress address,
            OctetString securityName,
            SecurityLevel requestedSecurityLevel,
            SecurityLevel transportSecurityLevel,
            bool sameSecurity,
            object sessionID,
            ICertifiedIdentity certifiedIdentity)
            : this(transport, address, securityName, requestedSecurityLevel, transportSecurityLevel,
                  sameSecurity, sessionID)
        {
            this.certifiedIdentity = certifiedIdentity;
        }

        /// <summary>
        /// Gets the transport mapping
        /// </summary>
        public ITransportMapping<IAddress> Transport
        {
            get
            {
                return this.transport;
            }
        }

        /// <summary>
        /// Gets the address
        /// </summary>
        public IAddress Address
        {
            get
            {
                return this.address;
            }
        }

        /// <summary>
        /// Gets or sets the security name for this state reference
        /// </summary>
        public OctetString SecurityName
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

        /// <summary>
        /// Gets or sets the requested security level
        /// </summary>
        public SecurityLevel RequestedSecurityLevel
        {
            get
            {
                return requestedSecurityLevel;
            }

            set
            {
                this.requestedSecurityLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the transport security level
        /// </summary>
        public SecurityLevel TransportSecurityLevel
        {
            get
            {
                return this.transportSecurityLevel;
            }

            set
            {
                this.transportSecurityLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to use the same security
        /// </summary>
        public bool SameSecurity
        {
            get
            {
                return this.sameSecurity;
            }

            set
            {
                this.sameSecurity = value;
            }
        }

        /// <summary>
        /// Gets the session identifier
        /// </summary>
        public object SessionID
        {
            get
            {
                return this.sessionID;
            }
        }

        /// <summary>
        /// Gets the certified identity
        /// </summary>
        public ICertifiedIdentity CertifiedIdentity
        {
            get
            {
                return this.certifiedIdentity;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the transport security is valid, i.e. if transport, 
        /// address, securityName and transportSecurityLevel are not null.
        /// </summary>
        public bool IsTransportSecurityValid
        {
            get
            {
                return (transport != null) && (address != null) && (securityName != null);
            }
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "TransportStateReference[" +
                "transport=" + this.transport +
                ", address=" + this.address +
                ", securityName=" + this.securityName +
                ", requestedSecurityLevel=" + this.requestedSecurityLevel +
                ", transportSecurityLevel=" + this.transportSecurityLevel +
                ", sameSecurity=" + this.sameSecurity +
                ", sessionID=" + this.sessionID +
                ", certifiedIdentity=" + this.certifiedIdentity +
                ']';
        }
    }
}
