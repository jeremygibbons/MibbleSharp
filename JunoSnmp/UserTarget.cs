// <copyright file="UserTarget.cs" company="None">
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
    /// User based target for SNMPv3 or later.
    /// </summary>
    public class UserTarget : SecureTarget, IEquatable<UserTarget>
    {
        private readonly OctetString authoritativeEngineID = new OctetString();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTarget"/> class,
        /// creating a target for a user based security model target.
        /// </summary>
        public UserTarget()
        {
            this.SecurityModel = Security.SecurityModel.SecurityModelID.SECURITY_MODEL_USM;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTarget"/> class,
        /// creating a target for a user based security model target with
        /// security level noAuthNoPriv and a one second timeout without retries
        /// </summary>
        /// <param name="address">The transport <see cref="IAddress"/> of the target</param>
        /// <param name="securityName">The USM security name to be used to access the target</param>
        /// <param name="authoritativeEngineID">
        /// The authoritative engine ID as a possibly zero length
        /// byte array which must not be null
        /// </param>
        public UserTarget(IAddress address, OctetString securityName,
                          byte[] authoritativeEngineID)
            : base(address, securityName)
        {
            this.AuthoritativeEngineID = authoritativeEngineID;
            this.SecurityModel = Security.SecurityModel.SecurityModelID.SECURITY_MODEL_USM;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTarget"/> class,
        /// creating a target for a user based security model target with
        /// the supplied security level and a one second timeout without retries
        /// </summary>
        /// <param name="address">The transport <see cref="IAddress"/> of the target</param>
        /// <param name="securityName">The USM security name to be used to access the target</param>
        /// <param name="authoritativeEngineID">
        /// The authoritative engine ID as a possibly zero length
        /// byte array which must not be null
        /// </param>
        /// <param name="securityLevel">The <see cref="SecurityLevel"/> to be used</param>
        public UserTarget(
            IAddress address, 
            OctetString securityName,
            byte[] authoritativeEngineID, 
            SecurityLevel securityLevel)
            : base(address, securityName)
        {
            this.AuthoritativeEngineID = authoritativeEngineID;
            this.SecurityLevel = securityLevel;
            this.SecurityModel = Security.SecurityModel.SecurityModelID.SECURITY_MODEL_USM;
        }
        
        /// <summary>
        /// Gets or sets the autoritative engine ID of this target. The value must
        /// not be null, but may be zero length.
        /// </summary>
        public byte[] AuthoritativeEngineID
        {
            get
            {
                return this.authoritativeEngineID.GetValue();
            }

            set
            {
                this.authoritativeEngineID.SetValue(value);
            }
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return "UserTarget[" + this.ToStringAbstractTarget() +
                ", authoritativeEngineID=" + this.AuthoritativeEngineID +
                ']';
        }

        /// <summary>
        /// Checks whether this object is equal to another
        /// </summary>
        /// <param name="o">The object to be compared with</param>
        /// <returns>True if the objects are equal, false if not</returns>
        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            if (!base.Equals(o)) return false;

            UserTarget that = (UserTarget)o;

            if (authoritativeEngineID != null ? !authoritativeEngineID.Equals(that.authoritativeEngineID) : that.authoritativeEngineID != null)
                return false;

            return true;
        }

        public bool Equals(UserTarget ut)
        {
            if (ut == null || this.GetType() != ut.GetType()) return false;
            if (!base.Equals(ut)) return false;

            if (this.authoritativeEngineID != null ? !authoritativeEngineID.Equals(ut.authoritativeEngineID) : ut.authoritativeEngineID != null)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a hash value for this object
        /// </summary>
        /// <returns>A hash value for this object</returns>
        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 31 * result + (authoritativeEngineID != null ? authoritativeEngineID.GetHashCode() : 0);
            return result;
        }
    }
}