// <copyright file="PDUv1.cs" company="None">
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
    using System.IO;
    using System.Linq;
    using System.Text;
    using JunoSnmp.ASN1;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <see cref="PDUv1"/> represents SNMP v1 PDUs. The behavior of this class
    /// is identical to its superclass <see cref="PDU"/> for the PDU type <see cref="PDU.GET"/>,
    /// <see cref="PDU.GETNEXT"/>, and <see cref="PDU.SET"/>. The other SNMP v2 PDU types
    /// implemented by <see cref="PDU"/> are not supported. In contrast to its super
    /// class, <c>PDUv1</c> implements the <see cref="PDU.TRAP"/> type.
    /// </para><para>
    /// To support this type, access methods are provided to get and set the
    /// enterprise <c>OID</c>, generic, specific, and timestamp of a SNMP v1 trap PDU.
    /// </para><para>
    /// The constants defined for generic SNMP v1 traps are included in this class.
    /// The descriptions are taken from the <c>SNMPv2-MIB</c> (RFC 3418). The corresponding
    /// OIDs are defined in <see cref="JunoSnmp.MP.SnmpConstants"/>.
    /// </summary>
    public class PDUv1 : PDU
    {

        /// <summary>
        /// A coldStart (0) trap signifies that the SNMP entity,
        /// supporting a notification originator application, is
        /// reinitializing itself and that its configuration may
        /// have been altered.
        /// </summary>
        public static readonly int COLDSTART = 0;

        /// <summary>
        /// A warmStart (1) trap signifies that the SNMP entity,
        /// supporting a notification originator application,
        /// is reinitializing itself such that its configuration
        /// is unaltered.
        /// </summary>
        public static readonly int WARMSTART = 1;

        /// <summary>
        /// A linkDown (2) trap signifies that the SNMP entity, acting in
        /// an agent role, has detected that the <c>ifOperStatus</c> object for
        /// one of its communication links is about to enter the down
        /// state from some other state (but not from the <c>notPresent</c>
        /// state).  This other state is indicated by the included value
        /// of <c>ifOperStatus</c>.
        /// </summary>
        public static readonly int LINKDOWN = 2;

        /// <summary>
        /// A linkUp (3) trap signifies that the SNMP entity, acting in an
        /// agent role, has detected that the <c>ifOperStatus</c> object for
        /// one of its communication links left the down state and
        /// transitioned into some other state (but not into the
        /// <c>notPresent</c> state).  This other state is indicated by the
        /// included value of <c>ifOperStatus</c>.
        /// </summary>
        public static readonly int LINKUP = 3;

        /// <summary>
        /// An <c>authenticationFailure</c> (4) trap signifies that the SNMP
        /// entity has received a protocol message that is not
        /// properly authenticated. While all implementations
        /// of SNMP entities MAY be capable of generating this
        /// trap, the <c>snmpEnableAuthenTraps</c> object indicates
        /// whether this trap will be generated.
        /// </summary>
        public static readonly int AUTHENTICATIONFAILURE = 4;

        /// <summary>
        /// If the generic trap identifier is <c>EnterpriseSpecific</c>(6), then
        /// the enterprise specific trap ID is given by the <c>specificTrap</c> member field.
        /// </summary>
        public static readonly int EnterpriseSpecific = 6;

        private static string OperationNotSupported = "Operation not supported for SNMPv1 PDUs";

        private OID enterprise = new OID();
        private IpAddress agentAddress = new IpAddress("0.0.0.0");
        private Integer32 genericTrap = new Integer32(0);
        private Integer32 specificTrap = new Integer32(0);
        private TimeTicks timestamp = new TimeTicks(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="PDUv1"/>
        /// </summary>
        public PDUv1()
        {
            this.Type = PDU.V1TRAP;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PDUv1"/>, copying its values from the 
        /// <c>PDUv1</c> object passed as an argument.
        /// </summary>
        public PDUv1(PDUv1 other) : base(other)
        {
            this.enterprise = (OID)other.enterprise.Clone();
            this.agentAddress = (IpAddress)other.agentAddress.Clone();
            this.genericTrap = (Integer32)other.genericTrap.Clone();
            this.specificTrap = (Integer32)other.specificTrap.Clone();
            this.timestamp = (TimeTicks)other.timestamp.Clone();
        }

        /// <summary>
        /// Creates a new copy of this <c>PDUv1</c> object.
        /// </summary>
        /// <returns>A fresh copy of this <c>PDUv1</c> object</returns>
        public override object Clone()
        {
            return new PDUv1(this);
        }

        /// <summary>
        /// Decodes a <c>Variable</c> from an <c>InputStream</c>.
        /// </summary>
        /// <param name="inputStream">An <see cref="BERInputStream"/> to read from</param>
        public override void DecodeBER(BERInputStream inputStream)
        {
            byte pduType;
            int length = BER.DecodeHeader(inputStream, out pduType);
            int pduStartPos = (int)inputStream.Position;

            switch (pduType)
            {
                case PDU.SET:
                case PDU.GET:
                case PDU.GETNEXT:
                case PDU.V1TRAP:
                case PDU.RESPONSE:
                    break;
                // The following PDU types are not supported by the SNMPv1 standard!
                case PDU.NOTIFICATION:
                case PDU.INFORM:
                    if (JunoSnmpSettings.AllowSNMPv2InV1)
                    {
                        break;
                    }

                    throw new ArgumentException("Unsupported PDU type: " + pduType);
                default:
                    throw new ArgumentException("Unsupported PDU type: " + pduType);
            }

            this.Type = pduType;

            if (this.Type == PDU.V1TRAP)
            {
                this.enterprise.DecodeBER(inputStream);
                this.agentAddress.DecodeBER(inputStream);
                this.genericTrap.DecodeBER(inputStream);
                this.specificTrap.DecodeBER(inputStream);
                this.timestamp.DecodeBER(inputStream);
            }
            else
            {
                this.requestID.DecodeBER(inputStream);
                this.errorStatus.DecodeBER(inputStream);
                this.errorIndex.DecodeBER(inputStream);
            }

            // reusing pduType here to save memory ;-)
            int vbLength = BER.DecodeHeader(inputStream, out pduType);

            if (pduType != BER.SEQUENCE)
            {
                throw new IOException("Encountered invalid tag, SEQUENCE expected: " + pduType);
            }

            // rest read count
            int startPos = (int)inputStream.Position;
            this.variableBindings = new List<VariableBinding>();
            while (inputStream.Position - startPos < vbLength)
            {
                VariableBinding vb = new VariableBinding();
                vb.DecodeBER(inputStream);

                if (!this.IsVariableV1(vb.Variable))
                {
                    throw new MessageException("Counter64 encountered in SNMPv1 PDU (RFC 2576 §4.1.2.1)");
                }

                variableBindings.Add(vb);
            }

            if (BER.CheckSequenceLengthFlag)
            {
                BER.CheckSequenceLength(vbLength, (int)inputStream.Position - startPos, this);
                BER.CheckSequenceLength(length, (int)inputStream.Position - pduStartPos, this);
            }
        }

        /// <summary>
        /// Encodes a variable to an output stream.
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeHeader(outputStream, type, this.BERPayloadLength);

            if (type == PDU.V1TRAP)
            {
                this.enterprise.EncodeBER(outputStream);
                this.agentAddress.EncodeBER(outputStream);
                this.genericTrap.EncodeBER(outputStream);
                this.specificTrap.EncodeBER(outputStream);
                this.timestamp.EncodeBER(outputStream);
            }
            else
            {
                this.requestID.EncodeBER(outputStream);
                this.errorStatus.EncodeBER(outputStream);
                this.errorIndex.EncodeBER(outputStream);
            }

            int vbLength = 0;
            vbLength += variableBindings.Select(vb => vb.BERLength).Sum();

            BER.EncodeHeader(outputStream, BER.SEQUENCE, vbLength);
            for (int i = 0; i < variableBindings.Count; i++)
            {
                VariableBinding vb = variableBindings[i];
                if (!this.IsVariableV1(vb.Variable))
                {
                    throw new IOException("Cannot encode Counter64 into a SNMPv1 PDU");
                }

                vb.EncodeBER(outputStream);
            }
        }

        /// <summary>
        /// Check if the given variable can be encoded into a SNMP v1 PDU.
        /// </summary>
        /// <param name="v">A variable value (must not be null)</param>
        /// <returns>
        /// True if the variabe is SNMP v1 compatible, or if <see cref="JunoSnmpSettings.AllowSNMPv2InV1"/>
        /// is true, false otherwise, for example if v is an instance of <see cref="Counter64"/>
        /// </returns>
        protected bool IsVariableV1(IVariable v)
        {
            return !(v is Counter64) || JunoSnmpSettings.AllowSNMPv2InV1;
        }

        /// <summary>
        /// Gets the BER length of the PDU payload
        /// </summary>
        protected override int BERPayloadLengthPDU
        {
            get
            {
                if (this.Type != PDU.V1TRAP)
                {
                    return base.BERPayloadLengthPDU;
                }
                else
                {
                    int length = 0;

                    length = VariableBindings.Select(vb => vb.BERLength).Sum();

                    length += BER.GetBERLengthOfLength(length) + 1;
                    length += agentAddress.BERLength;
                    length += enterprise.BERLength;
                    length += genericTrap.BERLength;
                    length += specificTrap.BERLength;
                    length += timestamp.BERLength;
                    return length;
                }
            }
        }

        /// <summary>
        /// Sets the maximum repetitions of repeatable variable bindings in GETBULK
        /// requests.
        /// </summary>
        /// <remarks>Not supported in SNMP v1 PDUs</remarks>
        public override int MaxRepetitions
        {
            get
            {
                throw new NotSupportedException(OperationNotSupported);
            }

            set
            {
                throw new NotSupportedException(OperationNotSupported);
            }
        }

        /// <summary>
        /// This method is not supported for SNMPv1 PDUs and will throw a
        /// <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="maxSizeScopedPDU">The maximum size of the Scoped PDU</param>
        public void SetMaxSizeScopedPDU(int maxSizeScopedPDU)
        {
            throw new NotSupportedException(OperationNotSupported);
        }

        /// <summary>
        /// This method is not supported for SNMPv1 PDUs and will throw a
        /// <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="nonRepeaters">The non repeaters parameter</param>
        public void SetNonRepeaters(int nonRepeaters)
        {
            throw new NotSupportedException(OperationNotSupported);
        }

        /// <summary>
        /// Checks whether this is an SNMP version 1 trap.
        /// </summary>
        private void CheckV1TRAP()
        {
            if (this.Type != PDU.V1TRAP)
            {
                throw new NotSupportedException(
                    "Operation is only supported for SNMPv1 trap PDUs (V1TRAP)");
            }
        }

        /// <summary>
        /// Gets or sets the "enterprise" OID of the SNMP v1 trap. 
        /// </summary>
        /// <remarks>
        /// Although the name of this property might lead to the assumption that the
        /// enterprise OID has to be an OID under the
        /// <c>iso(1).org(3).dod(6).internet(1).private(4).enterprises(1)</c> node,
        /// it is actually allowed to be any valid OID.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the PDU is not an SNMP v1 trap</exception> 
        public OID Enterprise
        {
            get
            {
                this.CheckV1TRAP();
                return enterprise;
            }

            set
            {
                this.CheckV1TRAP();
                this.CheckNull(value);
                this.enterprise = (OID)value.Clone();
            }
        }
        
        /// <summary>
        /// Gets or sets the IP address of the originator system of this SNMP v1 trap.
        /// If this value is <c>0.0.0.0</c> (the recommended default), then the address
        /// of the peer SNMP entity should be extracted from the <see cref="ITarget"/>
        /// object associated with this PDU.
        /// </summary>
        /// <remarks>
        /// The default value should be overriden only in special cases, such as
        /// when forwarding SNMP v1 traps through a SNMP proxy.
        /// </remarks>
        /// <exception cref="NotSupportedException">If this is not an SNMP v1 Trap PDU</exception>
        public JunoSnmp.SMI.IpAddress AgentAddress
        {
            get
            {
                this.CheckV1TRAP();
                return this.agentAddress;
            }

            set
            {
                this.CheckV1TRAP();
                this.CheckNull(value);
                this.agentAddress = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the generic trap ID. If this value is <c>ENTERPRISE_SPECIFIC(6)</c>
        /// then <see cref="SpecificTrap"/> must be used to set the trap ID of the enterprise
        /// specific trap.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If the type of this PDU is not <see cref="PDU.V1TRAP"/>
        /// </exception>
        public int GenericTrap
        {
            get
            {
                this.CheckV1TRAP();
                return this.genericTrap.GetValue();
            }

            set
            {
                this.CheckV1TRAP();
                this.genericTrap.SetValue(value);
            }
        }
        
        /// <summary>
        /// Gets or sets the specific trap ID. If this value is set,
        /// <see cref="GenericTrap"/> must return <c>ENTERPRISE_SPECIFIC(6)</c>
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If the type of this PDU is not <see cref="PDU.V1TRAP"/>
        /// </exception>
        public int SpecificTrap
        {
            get
            {
                this.CheckV1TRAP();
                return specificTrap.GetValue();
            }

            set
            {
                this.CheckV1TRAP();
                this.specificTrap.SetValue(value);
            }
        }
        
        /// <summary>
        /// Gets or sets the <c>TimeTicks</c> value of the trap sender's notion of
        /// its sysUpTime value when this trap has been generated.
        /// </summary>
        public long Timestamp
        {
            get
            {
                this.CheckV1TRAP();
                return timestamp.GetValue();
            }

            set
            {
                this.CheckV1TRAP();
                this.timestamp.SetValue(value);
            }
        }
        
        /// <summary>
        /// Checks for null parameters.
        /// </summary>
        /// <param name="parameter">An IVariable instance</param>
        /// <exception cref="ArgumentNullException">If <c>parameter</c> is null.</exception>
        protected void CheckNull(IVariable parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("Members of PDUv1 must not be null");
            }
        }

        /// <summary>
        /// Gets a text representation of this object
        /// </summary>
        /// <returns>A text representation of this object</returns>
        public override string ToString()
        {
            if (type == PDU.V1TRAP)
            {
                StringBuilder buf = new StringBuilder();
                buf.Append(GetTypeString(type));
                buf.Append("[requestID=");
                buf.Append(requestID);
                buf.Append(",timestamp=");
                buf.Append(timestamp);
                buf.Append(",enterprise=");
                buf.Append(enterprise);
                buf.Append(",genericTrap=");
                buf.Append(genericTrap);
                buf.Append(",specificTrap=");
                buf.Append(specificTrap);
                buf.Append(", VBS[");

                for (int i = 0; i < variableBindings.Count; i++)
                {
                    buf.Append(variableBindings[i]);
                    if (i + 1 < variableBindings.Count)
                    {
                        buf.Append("; ");
                    }
                }

                buf.Append("]]");
                return buf.ToString();
            }

            return base.ToString();
        }

        /// <summary>
        /// Checks this object for equality with another object
        /// </summary>
        /// <param name="obj">Another objet to compare against</param>
        /// <returns>
        /// True if the objects are equal, i.e. all their properties are equal. False if not
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is PDUv1)
            {
                PDUv1 o = (PDUv1)obj;
                return base.Equals(obj) &&
                  AbstractVariable.Equal(enterprise, o.enterprise) &&
                  AbstractVariable.Equal(agentAddress, o.agentAddress) &&
                  AbstractVariable.Equal(genericTrap, o.genericTrap) &&
                  AbstractVariable.Equal(specificTrap, o.specificTrap) &&
                  AbstractVariable.Equal(timestamp, o.timestamp);
            }

            return base.Equals(obj);    //To change body of overridden methods use File | Settings | File Templates.
        }
        
        /// <summary>
        /// Gets a hash code for this object
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
