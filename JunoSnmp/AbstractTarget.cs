// <copyright file="AbstractTarget.cs" company="None">
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

namespace JunoSnmp
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /// <summary>
    /// An <see cref="AbstractTarget"/> class is an abstract representation of a remote
    /// SNMP entity.It represents a target with an Address object, as well protocol
    /// parameters such as retransmission and timeout policy.Implementers of the
    /// <see cref="ITarget"/> interface can subclass <c>AbstractTarget</c> to
    /// take advantage of the implementation of common <c>Target</c>
    //// properties.
    /// </summary>
    public abstract class AbstractTarget : ITarget, IEquatable<AbstractTarget>
    {
        public IAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the SNMP version (and thus the SNMP message processing model) of the target
        /// </summary>
        /// <see cref="SnmpConstants.version1"/>
        /// <see cref="SnmpConstants.version2c"/>
        /// <see cref="SnmpConstants.version3"/>
        public int Version { get; set; } = SnmpConstants.version3;

        private int retries = 0;

        /// <summary>
        /// Gets or sets the timeout for the target, in milliseconds
        /// </summary>
        public long Timeout { get; set;} = 1000;
        private int maxSizeRequestPDU = 65535;

        /// <summary>
        /// Gets or sets the prioritised list of transport mappings to be used for this
        /// target. The first mapping in the list that matches the target address
        /// will be chosen for sending new requests.If the value is set to
        /// <c>null</c> (default), the appropriate <see cref="ITransportMapping{A}"/>
        /// will be chosen by the supplied address of the target.
        /// If an entity supports more than one transport mapping for
        /// an <see cref="IAddress"/> class, then the results are not defined.
        /// This situation can be controlled by setting this preferredTransports list.
        /// </summary>
        public IList<ITransportMapping<IAddress>> PreferredTransports { get; set; }

        /// <summary>
        /// Gets or sets the security level for this target. The supplied security level must
        /// be supported by the security model dependent information associated with
        /// the security name set for this target.
        /// </summary>
        /// <see cref="Security.SecurityLevel.NoAuthNoPriv"/>
        /// <see cref="Security.SecurityLevel.AuthNoPriv"/>
        /// <see cref="Security.SecurityLevel.AuthPriv"/>
        public virtual SecurityLevel SecurityLevel { get; set; } = SecurityLevel.NoAuthNoPriv;

        /// <summary>
        /// Sets the security model for this target.
        /// </summary>
        public virtual JunoSnmp.Security.SecurityModel.SecurityModelID SecurityModel { get; set; } = JunoSnmp.Security.SecurityModel.SecurityModelID.SECURITY_MODEL_USM;

        /// <summary>
        /// Gets or sets the securityname used to authenticate with this target
        /// </summary>
        public OctetString SecurityName { get; set; } = new OctetString();

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTarget"/> class.
        /// </summary>
        protected AbstractTarget()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTarget"/> class, as a
        /// SNMP v3 target with no retries and a 1 second timeout.
        /// </summary>
        /// <param name="address">An <see cref="IAddress"/> instance</param>
        protected AbstractTarget(IAddress address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTarget"/> class, as a
        /// SNMP v3 target with no retries and a 1 second timeout.
        /// </summary>
        /// <param name="address">An <see cref="IAddress"/> instance</param>
        /// <param name="securityName">The security name to be used</param>
        protected AbstractTarget(IAddress address, OctetString securityName) : this(address)
        {
            this.SecurityName = securityName;
        }
        
        /// <summary>
        /// Gets or sets the number of retries to be performed before a request is timed out.
        /// </summary>
        /// <remarks>
        /// If the value is 0, then the request is sent exactly once, with no retries
        /// </remarks>
        public int Retries
        {
            get
            {
                return this.retries;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Number of retries < 0");
                }

                this.retries = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum size in bytes of the request PDUs that this target can respond to.
        /// The default is 65535, and the value must be greater than 484.
        /// </summary>
        public int MaxSizeRequestPDU
        {
            get
            {
                return this.maxSizeRequestPDU;
            }

            set
            {
                if (value < SnmpConstants.MIN_PDU_LENGTH)
                {
                    throw new ArgumentException("The minimum PDU length is: " +
                                                       SnmpConstants.MIN_PDU_LENGTH);
                }

                this.maxSizeRequestPDU = value;
            }
        }

        /// <summary>
        /// Gets a string representation of this target
        /// </summary>
        /// <returns>A string representation of this target</returns>
        public override string ToString()
        {
            return this.GetType().Name + "[" + this.ToStringAbstractTarget() + "]";
        }

        /// <summary>
        /// Gets a fresh copy of this object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new NotSupportedException();
        }

        /// <summary>
        /// Tests this object for equality with another object
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>True if the objects are of the same value, false if not</returns>
        public override bool Equals(object o)
        {
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            if (o is AbstractTarget that)
            {
                return this.Equals(that);
            }

            return false;
        }

        /// <summary>
        /// Tests this object for equality with another object
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>True if the objects are of the same value, false if not</returns>
        public bool Equals(AbstractTarget that)
        {
            if (that == null
                || (this.Version != that.Version)
                || (this.Retries != that.Retries)
                || (this.Timeout != that.Timeout)
                || (this.maxSizeRequestPDU != that.maxSizeRequestPDU)
                || (this.SecurityLevel != that.SecurityLevel)
                || (this.SecurityModel != that.SecurityModel)
                || (!this.Address.Equals(that.Address))
                || (this.PreferredTransports != null ? !this.PreferredTransports.Equals(that.PreferredTransports) : that.PreferredTransports != null)
                || (!this.SecurityName.Equals(that.SecurityName)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public override int GetHashCode()
        {
            int result = this.Address.GetHashCode();
            result = 31 * result + this.Version;
            result = 31 * result + this.SecurityName.GetHashCode();
            return result;
        }

        /// <summary>
        /// Serializes this object to a serialization stream
        /// </summary>
        /// <param name="info">The <c>SerializationInfo</c> object</param>
        /// <param name="context">The <c>StreamingContext</c> object</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a string representation of this target.
        /// </summary>
        /// <returns>A string representation of this target</returns>
        protected string ToStringAbstractTarget()
        {
            return "address=" + this.Address + ",version=" + this.Version +
                ",timeout=" + this.Timeout + ",retries=" + this.retries +
                ",securityLevel=" + this.SecurityLevel +
                ",securityModel=" + this.SecurityModel +
                ",securityName=" + this.SecurityName +
                ",preferredTransports=" + this.PreferredTransports;
        }
    }
}
