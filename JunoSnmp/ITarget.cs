// <copyright file="ITarget.cs" company="None">
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
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;

    /// <summary>
    /// An <c>ITarget</c> interface defines an abstract representation of a
    /// remote SNMP entity.It represents a target with an <see cref="IAddress"/> object, 
    /// as well protocol parameters such as retransmission and timeout policy.
    /// </summary>
    public interface ITarget : ISerializable, ICloneable
    {
        /// <summary>
        /// Gets or sets the address of the target
        /// </summary>
        IAddress Address { get; set; }
        
        /// <summary>
        /// Gets or sets the SNMP version (SNMP message processing model) of the target.
        /// </summary>
        /// <see cref="JunoSnmp.MP.SnmpConstants.version1"/>
        /// <see cref="JunoSnmp.MP.SnmpConstants.version2c"/>
        /// <see cref="JunoSnmp.MP.SnmpConstants.version3"/>
        int Version { get; set; }
        
        /// <summary>
        /// Gets or sets the number of retries to be performed before a request is timed out,
        /// as a positive integer.
        /// </summary>
        /// <remarks>
        /// If the number of retries is set to 0, then the request is sent
        /// exactly once
        /// </remarks>
        int Retries { get; set; }
        
        /// <summary>
        /// Gets or sets the timeout for a target
        /// </summary>
        /// <remarks>The value represents the number of milliseconds before a confirmed request
        /// is resent or timed out
        /// </remarks>
        long Timeout { get; set; }

        /**
         * Gets the maximum size of request PDUs that this target is able to respond
         * to. The default is 65535.
         * @return
         *    the maximum PDU size of request PDUs for this target. Which is always
         *    greater than 484.
         */

        /**
         * Sets the maximum size of request PDUs that this target is able to receive.
         * @param maxSizeRequestPDU
         *    the maximum PDU (SNMP message) size this session will be able to
         *    process.
         */
        int MaxSizeRequestPDU { get; set; }

        /**
         * Gets the prioritised list of transport mappings to be used for this
         * target. The first mapping in the list that matches the target address
         * is chosen for sending new requests.
         *
         * @return
         *    an ordered list of {@link TransportMapping} instances.
         * @since 2.0
         */
        IList<ITransportMapping<IAddress>> PreferredTransports { get; }

        /**
         * Gets the security model associated with this target.
         * @return
         *    an <code>int</code> value as defined in the {@link org.snmp4j.security.SecurityModel}
         *    interface or any third party subclass thereof.
         */

        /**
         * Sets the security model for this target.
         * @param securityModel
         *    an <code>int</code> value as defined in the {@link org.snmp4j.security.SecurityModel}
         *    interface or any third party subclass thereof.
         */
        JunoSnmp.Security.SecurityModel.SecurityModelID SecurityModel { get; set; }

        /**
         * Gets the security name associated with this target. The security name
         * is used by the security model to lookup further parameters like
         * authentication and privacy protocol settings from the security model
         * dependent internal storage.
         * @return
         *   an <code>OctetString</code> instance (never <code>null</code>).
         */

        /**
         * Sets the security name to be used with this target.
         * @param securityName
         *    an <code>OctetString</code> instance (must not be <code>null</code>).
         * @see #getSecurityName()
         */
        OctetString SecurityName { get; set; }

        /**
         * Gets the security level associated with this target.
         * @return
         *   one of
         *   <P><UL>
         *   <LI>{@link org.snmp4j.security.SecurityLevel#NOAUTH_NOPRIV}
         *   <LI>{@link org.snmp4j.security.SecurityLevel#AUTH_NOPRIV}
         *   <LI>{@link org.snmp4j.security.SecurityLevel#AUTH_PRIV}
         *   </UL></P>
         */

        /**
         * Sets the security level for this target. The supplied security level must
         * be supported by the security model dependent information associated with
         * the security name set for this target.
         * @param securityLevel
         *   one of
         *   <P><UL>
         *   <LI>{@link org.snmp4j.security.SecurityLevel#NOAUTH_NOPRIV}
         *   <LI>{@link org.snmp4j.security.SecurityLevel#AUTH_NOPRIV}
         *   <LI>{@link org.snmp4j.security.SecurityLevel#AUTH_PRIV}
         *   </UL></P>
         */
        SecurityLevel SecurityLevel { get; set; }
    }
}
