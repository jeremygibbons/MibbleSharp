// <copyright file="PDU.cs" company="None">
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
    using System.Runtime.Serialization;
    using System.Text;
    using JunoSnmp.ASN1;
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>PDU</code> class represents a SNMP protocol data unit. The PDU
    /// version supported by the BER decoding and encoding methods of this class
    /// is v2.The default PDU type is GET.
    /// </summary>
    /// <seealso cref="PDUv1"/>
    /// <seealso cref="ScopedPDU"/>
    public class PDU : IBERSerializable, ISerializable
    {
        /**
         * Denotes a get PDU.
         */
        public const int GET = (BER.Asn1Context | BER.Asn1Constructor | 0x0);
        /**
         * Denotes a getnext (search) PDU.
         */
        public const int GETNEXT = (BER.Asn1Context | BER.Asn1Constructor | 0x1);
        /**
         * Denotes a response PDU.
         */
        public const int RESPONSE = (BER.Asn1Context | BER.Asn1Constructor | 0x2);
        /**
         * Denotes a set PDU.
         */
        public const int SET = (BER.Asn1Context | BER.Asn1Constructor | 0x3);
        /**
         * Denotes a SNMPv1 trap PDU. This type can only be used with instances of the
         * {@link PDUv1} class.
         */
        public const int V1TRAP = (BER.Asn1Context | BER.Asn1Constructor | 0x4);
        /**
         * Denotes a SNMPv2c/v3 getbulk PDU.
         */
        public const int GETBULK = (BER.Asn1Context | BER.Asn1Constructor | 0x5);
        /**
         * Denotes a SNMPv2c/v3 inform PDU (unprecisely also known as a confirmed
         * notification).
         */
        public const int INFORM = (BER.Asn1Context | BER.Asn1Constructor | 0x6);
        /**
         * Denotes a SNMPv2c/v3 notification PDU (undistinguishable from
         * {@link #TRAP}).
         */
        public const int TRAP = (BER.Asn1Context | BER.Asn1Constructor | 0x7);
        /**
         * Denotes a SNMPv2c/v3 notification PDU (undistinguishable from
         * {@link #NOTIFICATION}).
         */
        public const int NOTIFICATION = TRAP;
        /**
         * Denotes a SNMPv3 report PDU.
         */
        public const int REPORT = (BER.Asn1Context | BER.Asn1Constructor | 0x8);


        // Error status constants

        /**
         * Operation success (no error).
         */
        public static readonly int noError = SnmpConstants.SNMP_ERROR_SUCCESS;

        /**
         * PDU encoding is too big for the transport used.
         */
        public static readonly int tooBig = SnmpConstants.SNMP_ERROR_TOO_BIG;

        /**
         * No such variable binding name, see error index.
         */
        public static readonly int noSuchName = SnmpConstants.SNMP_ERROR_NO_SUCH_NAME;

        /**
         * Bad value in variable binding, see error index.
         */
        public static readonly int badValue = SnmpConstants.SNMP_ERROR_BAD_VALUE;

        /**
         * The variable binding is read-only, see error index.
         */
        public static readonly int readOnly = SnmpConstants.SNMP_ERROR_READ_ONLY;

        /**
         * An unspecific error caused by a variable binding, see error index.
         */
        public static readonly int genErr = SnmpConstants.SNMP_ERROR_GENERAL_ERROR;

        /**
         * The variable binding is not accessible by the current MIB view, see error
         * index.
         */
        public static readonly int noAccess = SnmpConstants.SNMP_ERROR_NO_ACCESS;

        /**
         * The variable binding's value has the wrong type, see error index.
         */
        public static readonly int wrongType = SnmpConstants.SNMP_ERROR_WRONG_TYPE;

        /**
         * The variable binding's value has the wrong length, see error index.
         */
        public static readonly int wrongLength = SnmpConstants.SNMP_ERROR_WRONG_LENGTH;

        /**
         * The variable binding's value has a value that could under no circumstances
         * be assigned, see error index.
         */
        public static readonly int wrongValue = SnmpConstants.SNMP_ERROR_WRONG_VALUE;

        /**
         * The variable binding's value has the wrong encoding, see error index.
         */
        public static readonly int wrongEncoding = SnmpConstants.SNMP_ERROR_WRONG_ENCODING;

        /**
         * The specified object does not exists and cannot be created,
         * see error index.
         */
        public static readonly int noCreation = SnmpConstants.SNMP_ERROR_NO_CREATION;

        /**
         * The variable binding's value is presently inconsistent with the current
         * state of the target object, see error index.
         */
        public static readonly int inconsistentValue = SnmpConstants.SNMP_ERROR_INCONSISTENT_VALUE;

        /**
         * The resource needed to assign a variable binding's value is presently
         * unavailable, see error index.
         */
        public static readonly int resourceUnavailable = SnmpConstants.SNMP_ERROR_RESOURCE_UNAVAILABLE;

        /**
         * Unable to commit a value, see error index.
         */
        public static readonly int commitFailed = SnmpConstants.SNMP_ERROR_COMMIT_FAILED;

        /**
         * Unable to undo a committed value, see error index.
         */
        public static readonly int undoFailed = SnmpConstants.SNMP_ERROR_UNDO_FAILED;

        /**
         * Unauthorized access, see error index.
         */
        public static readonly int authorizationError =
            SnmpConstants.SNMP_ERROR_AUTHORIZATION_ERROR;

        /**
         * The variable's value cannot be modified, see error index.
         */
        public static readonly int notWritable = SnmpConstants.SNMP_ERROR_NOT_WRITEABLE;

        /**
         * The specified object does not exists and presently it cannot be created,
         * see error index.
         */
        public static readonly int inconsistentName = SnmpConstants.SNMP_ERROR_INCONSISTENT_NAME;

        protected List<VariableBinding> variableBindings = new List<VariableBinding>();
        protected Integer32 errorStatus = new Integer32();
        protected Integer32 errorIndex = new Integer32();
        protected Integer32 requestID = new Integer32();
        protected int type = GET;

        /**
         * Default constructor.
         */
        public PDU()
        {
        }

        /**
         * Copy constructor which creates a deep copy (clone) of the
         * other PDU.
         * @param other
         *    the <code>PDU</code> to copy from.
         */
        public PDU(PDU other)
        {
            this.variableBindings = new List<VariableBinding>();
            foreach (VariableBinding vb in other.variableBindings)
            {
                this.variableBindings.Add((VariableBinding)vb.Clone());
            }

            this.errorIndex = (Integer32)other.errorIndex.Clone();
            this.errorStatus = (Integer32)other.errorStatus.Clone();
            this.type = other.type;

            if (other.requestID != null)
            {
                this.requestID = (Integer32)other.requestID.Clone();
            }
        }

        /**
         * Constructs a new PDU from a type and a list of {@link VariableBinding} instances.
         * The list will not be referenced, instead a deep copy of the variable bindings
         * is executed (each variable binding will be cloned).
         *
         * @param pduType
         *    the PDU type.
         * @param vbs
         *    the variable bindings.
         * @since 2.2.4
         */
        public PDU(int pduType, List<VariableBinding> vbs)
        {
            this.type = pduType;
            this.variableBindings = new List<VariableBinding>(vbs.Count);
            foreach (VariableBinding vb in vbs)
            {
                this.variableBindings.Add((VariableBinding)vb.Clone());
            }
        }

        /**
         * Adds a variable binding to this PDU. A <code>NullPointerException</code>
         * is thrown if <code>VariableBinding</code> or its <code>Variable</code> is
         * <code>null</code>.
         * @param vb
         *   a <code>VariableBinding</code> instance.
         */
        public void Add(VariableBinding vb)
        {
            this.variableBindings.Add(vb);
        }

        /**
         * Adds a new variable binding to this PDU by using the OID of the supplied
         * <code>VariableBinding</code>. The value portion is thus set to
         * <code>null</code>.
         * <p>
         * This method should be used for GET type requests. For SET, TRAP and INFORM
         * requests, the {@link #add} method should be used instead.
         * @param vb
         *   a <code>VariableBinding</code> instance.
         * @since 1.8
         */
        public void AddOID(VariableBinding vb)
        {
            VariableBinding cvb = new VariableBinding(vb.Oid);
            this.variableBindings.Add(cvb);
        }

        /// <summary>
        /// Adds a set of <see cref="VariableBinding"/>s to this PDU.
        /// </summary>
        /// <param name="vbs">An <c>IEnumerable&lt;VariableBinding&gt;></c> instance</param>
        public void AddAll(IEnumerable<VariableBinding> vbs)
        {
            this.variableBindings.AddRange(vbs);
        }



        /**
         * Adds new <code>VariableBindings</code> each with the OID of the
         * corresponding variable binding of the supplied array to this PDU (see
         * {@link #addOID(VariableBinding vb)}).
         * @param vbs
         *   an array of <code>VariableBinding</code> instances. For each instance
         *   in the supplied array, a new VariableBinding created by
         *   <code>new VariableBinding(OID)</code> will be appended to the current
         *   list of variable bindings in the PDU.
         * @since 1.8
         */
        public void AddAllOIDs(IEnumerable<VariableBinding> vbs)
        {
            foreach (var vb in vbs)
            {
                this.AddOID(vb);
            }
        }

        /**
         * Gets the variable binding at the specified position.
         * @param index
         *    a zero based positive integer (<code>0 <= index < {@link #size()}</code>)
         * @return
         *    a VariableBinding instance. If <code>index</code> is out of bounds
         *    an exception is thrown.
         */
        public VariableBinding this[int index]
        {
            get
            {
                return variableBindings[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Variable binding must not be null");
                }

                this.variableBindings[index] = value;
            }
        }

        /**
         * Gets the first variable whose OID starts with the specified OID.
         * @param prefix
         *    the search {@link OID}.
         * @return
         *    the {@link Variable} of the first {@link VariableBinding}
         *    whose prefix matches <code>oid</code>. If no such element
         *    could be found, <code>null</code> is returned.
         * @since 2.0
         */
        public IVariable GetVariable(OID prefix)
        {
            return this.variableBindings.FirstOrDefault(v => v.Oid.StartsWith(prefix)).Variable;
        }

        /**
         * Gets a list of {@link VariableBinding}s whose OID prefix
         * matches the supplied prefix.
         * @param prefix
         *    the search {@link OID}.
         * @return
         *    a List of all {@link VariableBinding}s
         *    whose prefix matches <code>oid</code>. If no such element
         *    could be found, an empty List is returned.
         */
        public IList<VariableBinding> GetBindingList(OID prefix)
        {
            return this.variableBindings.Where(v => v.Oid.StartsWith(prefix)).ToList();
        }

        /**
         * Removes the variable binding at the supplied position.
         * @param index
         *    a position >= 0 and < {@link #size()}.
         */
        public void RemoveAt(int index)
        {
            variableBindings.RemoveAt(index);
        }

        /**
         * Gets the number of variable bindings in the PDU.
         * @return
         *    the size of the PDU.
         */
        public int Count
        {
            get
            {
                return this.variableBindings.Count;
            }
        }

        /**
         * Gets the variable binding vector.
         * @return
         *    the internal <code>Vector</code> containing the PDU's variable bindings.
         */
        public IList<VariableBinding> VariableBindings
        {
            get
            {
                return variableBindings;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.variableBindings = new List<VariableBinding>(value);
            }
        }

        /**
         * Remove the last variable binding from the PDU, if such an element exists.
         */
        public void Trim()
        {
            if (this.variableBindings.Count > 0)
            {
                this.variableBindings.RemoveAt(this.variableBindings.Count - 1);
            }
        }

        /**
         * Gets the error status of the PDU.
         * @return
         *    a SNMP error status.
         * @see SnmpConstants
         */

        /**
         * Sets the error status of the PDU.
         * @param errorStatus
         *    a SNMP error status.
         * @see SnmpConstants
         */
        public int ErrorStatus
        {
            get
            {
                return this.errorStatus.GetValue();
            }

            set
            {
                this.errorStatus.SetValue(value);
            }
        }

        /**
         * Gets a textual description of the error status.
         * @return
         *    a String containing an element of the
         *    {@link SnmpConstants#SNMP_ERROR_MESSAGES} array for a valid error status.
         *    "Unknown error: <errorStatusNumber>" is returned for any other value.
         */
        public string ErrorStatusText
        {
            get
            {
                return PDU.ToErrorStatusText(errorStatus.GetValue());
            }
        }

        /**
         * Returns textual description for the supplied error status value.
         * @param errorStatus
         *    an error status.
         * @return
         *    a String containing an element of the
         *    {@link SnmpConstants#SNMP_ERROR_MESSAGES} array for a valid error status.
         *    "Unknown error: <errorStatusNumber>" is returned for any other value.
         * @since 1.7
         */
        public static string ToErrorStatusText(int errorStatus)
        {
            try
            {
                if (errorStatus < 0)
                {
                    return SnmpConstants.SNMP_TP_ERROR_MESSAGES[Math.Abs(errorStatus) - 1];
                }

                return SnmpConstants.SNMP_ERROR_MESSAGES[errorStatus];
            }
            catch (IndexOutOfRangeException)
            {
                return "Unknown error: " + errorStatus;
            }
        }

        /**
         * Gets the error index.
         * @return
         *   an integer value >= 0 where 1 denotes the first variable binding.
         */

        /**
         * Sets the error index.
         * @param errorIndex
         *    an integer value >= 0 where 1 denotes the first variable binding.
         */
        public int ErrorIndex
        {
            get
            {
                return errorIndex.GetValue();
            }

            set
            {
                this.errorIndex.SetValue(value);
            }
        }

        /**
         * Checks whether this PDU is a confirmed class PDU.
         * @return boolean
         */
        public bool IsConfirmedPdu
        {
            get
            {
                return ((type != PDU.REPORT) && (type != PDU.RESPONSE) &&
                    (type != PDU.TRAP) && (type != PDU.V1TRAP));
            }
        }

        /**
         * Checks whether this PDU is a {@link PDU#RESPONSE} or [@link PDU#REPORT}.
         * @return
         *    <code>true</code> if {@link #getType()} returns {@link PDU#RESPONSE} or [@link PDU#REPORT} and
         *    <code>false</code> otherwise.
         * @since 2.4.1
         */
        public bool IsResponsePdu
        {
            get
            {
                return ((type == PDU.RESPONSE) || (type == PDU.REPORT));
            }
        }

        public virtual int BERLength
        {
            get
            {
                // header for data_pdu
                int length = this.BERPayloadLengthPDU;
                length += BER.GetBERLengthOfLength(length) + 1;
                // assume maximum length here
                return length;
            }
        }

        public virtual int BERPayloadLength
        {
            get
            {
                return this.BERPayloadLengthPDU;
            }
        }

        public virtual void DecodeBER(BERInputStream inputStream)
        {
            byte pduType;
            int length = BER.DecodeHeader(inputStream, out pduType);
            int pduStartPos = (int)inputStream.Position;
            switch ((int)pduType)
            {
                case PDU.SET:
                case PDU.GET:
                case PDU.GETNEXT:
                case PDU.GETBULK:
                case PDU.INFORM:
                case PDU.REPORT:
                case PDU.TRAP:
                case PDU.RESPONSE:
                    break;
                default:
                    throw new IOException("Unsupported PDU type: " + pduType);
            }

            this.type = pduType;
            requestID.DecodeBER(inputStream);
            errorStatus.DecodeBER(inputStream);
            errorIndex.DecodeBER(inputStream);

            int vbLength = BER.DecodeHeader(inputStream, out pduType);
            if (pduType != BER.SEQUENCE)
            {
                throw new ArgumentException("Encountered invalid tag, SEQUENCE expected: " +
                                      pduType);
            }

            // rest read count
            int startPos = (int)inputStream.Position;
            variableBindings = new List<VariableBinding>();
            while (inputStream.Position - startPos < vbLength)
            {
                VariableBinding vb = new VariableBinding();
                vb.DecodeBER(inputStream);
                variableBindings.Add(vb);
            }

            if (inputStream.Position - startPos != vbLength)
            {
                throw new IOException("Length of VB sequence (" + vbLength +
                                      ") does not match real length: " +
                                      ((int)inputStream.Position - startPos));
            }

            if (BER.CheckSequenceLengthFlag)
            {
                BER.CheckSequenceLength(length,
                                        (int)inputStream.Position - pduStartPos,
                                        this);
            }
        }

        /**
         * Computes the length in bytes of the BER encoded variable bindings without
         * including the length of BER sequence length.
         * @param variableBindings
         *    a list of variable bindings.
         */
        public static int GetBERLength(IList<VariableBinding> variableBindings)
        {
            int length = 0;
            // length for all vbs
            foreach (VariableBinding variableBinding in variableBindings)
            {
                length += variableBinding.BERLength;
            }

            return length;
        }

        protected virtual int BERPayloadLengthPDU
        {
            get
            {
                int length = GetBERLength(variableBindings);
                length += BER.GetBERLengthOfLength(length) + 1;

                // req id, error status, error index
                Integer32 i32 = new Integer32(requestID.GetValue());
                length += i32.BERLength;
                i32 = errorStatus;
                length += i32.BERLength;
                i32 = errorIndex;
                length += i32.BERLength;
                i32 = null;
                return length;
            }
        }

        public virtual void EncodeBER(Stream outputStream)
        {
            BER.EncodeHeader(outputStream, type, this.BERPayloadLengthPDU);

            requestID.EncodeBER(outputStream);
            errorStatus.EncodeBER(outputStream);
            errorIndex.EncodeBER(outputStream);

            int vbLength = 0;
            foreach (VariableBinding vb in variableBindings)
            {
                vbLength += vb.BERLength;
            }

            BER.EncodeHeader(outputStream, BER.SEQUENCE, vbLength);
            foreach (VariableBinding vb in variableBindings)
            {
                vb.EncodeBER(outputStream);
            }
        }

        /**
         * Removes all variable bindings from the PDU and sets the request ID to zero.
         * This can be used to reuse a PDU for another request.
         */
        public void Clear()
        {
            variableBindings.Clear();
            this.RequestID = new Integer32(0);
        }

        /**
         * Gets the PDU type. The default is {@link PDU#GETNEXT}.
         * @return
         *    the PDU's type.
         */

        /**
         * Sets the PDU type.
         * @param type
         *    the type of the PDU (e.g. GETNEXT, SET, etc.)
         */
        public int Type
        {
            get
            {
                return type;
            }

            set
            {
                this.type = value;
            }
        }

        public virtual object Clone()
        {
            return new PDU(this);
        }

        /**
         * Gets the request ID associated with this PDU.
         * @return
         *    an <code>Integer32</code> instance.
         */

        /**
         * Sets the request ID for this PDU. When the request ID is not set or set to
         * zero, the message processing model will generate a unique request ID for
         * the <code>PDU</code> when sent.
         * @param requestID
         *    a unique request ID.
         */
        public Integer32 RequestID
        {
            get
            {
                return this.requestID;
            }

            set
            {
                this.requestID = value;
            }
        }

        /**
         * Gets a string representation of the supplied PDU type.
         * @param type
         *    a PDU type.
         * @return
         *    a string representation of <code>type</code>, for example "GET".
         */
        public static string GetTypeString(int type)
        {
            switch (type)
            {
                case PDU.GET:
                    return "GET";
                case PDU.SET:
                    return "SET";
                case PDU.GETNEXT:
                    return "GETNEXT";
                case PDU.GETBULK:
                    return "GETBULK";
                case PDU.INFORM:
                    return "INFORM";
                case PDU.RESPONSE:
                    return "RESPONSE";
                case PDU.REPORT:
                    return "REPORT";
                case PDU.TRAP:
                    return "TRAP";
                case PDU.V1TRAP:
                    return "V1TRAP";
            }

            return "unknown";
        }

        /**
         * Gets the PDU type identifier for a string representation of the type.
         * @param type
         *    the string representation of a PDU type: <code>GET, GETNEXT, GETBULK,
         *    SET, INFORM, RESPONSE, REPORT, TRAP, V1TRAP)</code>.
         * @return
         *    the corresponding PDU type constant, or <code>Integer.MIN_VALUE</code>
         *    of the supplied type is unknown.
         */
        public static int getTypeFromString(String type)
        {
            if (type.Equals("GET"))
            {
                return PDU.GET;
            }
            else if (type.Equals("SET"))
            {
                return PDU.SET;
            }
            else if (type.Equals("GETNEXT"))
            {
                return PDU.GETNEXT;
            }
            else if (type.Equals("GETBULK"))
            {
                return PDU.GETBULK;
            }
            else if (type.Equals("INFORM"))
            {
                return PDU.INFORM;
            }
            else if (type.Equals("RESPONSE"))
            {
                return PDU.RESPONSE;
            }
            else if (type.Equals("TRAP"))
            {
                return PDU.TRAP;
            }
            else if (type.Equals("V1TRAP"))
            {
                return PDU.V1TRAP;
            }
            else if (type.Equals("REPORT"))
            {
                return PDU.REPORT;
            }

            return int.MinValue;
        }

        /**
         * Returns a string representation of the object.
         *
         * @return a string representation of the object.
         */
        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(PDU.GetTypeString(type));
            buf.Append("[requestID=");
            buf.Append(requestID);
            buf.Append(", errorStatus=");
            buf.Append(this.ErrorStatusText + "(" + errorStatus + ")");
            buf.Append(", errorIndex=");
            buf.Append(errorIndex);
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

        /**
         * Gets the maximum repetitions of repeatable variable bindings in GETBULK
         * requests.
         * @return
         *    an integer value >= 0.
         */

        /**
         * Sets the maximum repetitions of repeatable variable bindings in GETBULK
         * requests.
         * @param maxRepetitions
         *    an integer value >= 0.
         */
        public virtual int MaxRepetitions
        {
            get
            {
                return errorIndex.GetValue();
            }

            set
            {
                this.errorIndex.SetValue(value);
            }
        }

        /**
         * Gets the number of non repeater variable bindings in a GETBULK PDU.
         * @return
         *    an integer value >= 0 and <= {@link #size()}
         */

        /**
         * Sets the number of non repeater variable bindings in a GETBULK PDU.
         * @param nonRepeaters
         *    an integer value >= 0 and <= {@link #size()}
         */
        public int NonRepeaters
        {
            get
            {
                return errorStatus.GetValue();
            }

            set
            {
                this.errorStatus.SetValue(value);
            }
        }

        /**
         * Returns an array with the variable bindings of this PDU.
         * @return
         *    an array of <code>VariableBinding</code> instances of this PDU in the
         *    same order as in the PDU.
         */
        public VariableBinding[] ToArray()
        {
            return this.variableBindings.ToArray();
        }

        public override int GetHashCode()
        {
            // Returning the hasCode() of the request ID is not a good idea, as
            // this might change during sending a request.
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is PDU)
            {
                PDU o = (PDU)obj;
                return (type == o.type) &&
                    (AbstractVariable.Equal(requestID, o.requestID)) &&
                    (AbstractVariable.Equal(errorStatus, o.errorStatus)) &&
                    (AbstractVariable.Equal(errorIndex, o.errorIndex)) &&
                    variableBindings.Equals(o.variableBindings);
            }
            return false;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
