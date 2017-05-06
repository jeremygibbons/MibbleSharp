// <copyright file="VariableBinding.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using JunoSnmp.ASN1;
    using JunoSnmp.Util;

    /// <summary>
    /// A <c>VariableBinding</c> is an association of a object instance
    /// identifier(<see cref="OID"/>) and the instance's value (<see cref="IVariable"/>).
    /// </summary>
    public class VariableBinding : ISerializable, IBERSerializable, ICloneable, IEquatable<VariableBinding>
    {
        private OID oid;
        private IVariable variable;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableBinding"/> class 
        /// with a zero length OID and a <see cref="Null"/> value.
        /// </summary>
        public VariableBinding()
        {
            oid = new OID();
            this.variable = Null.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableBinding"/> class 
        /// with thhe supplied <see cref="OID"/> and a <see cref="Null"/> value.
        /// </summary>
        /// <param name="oid">The <see cref="OID"/> for the new variable binding</param>
        public VariableBinding(OID oid)
        {
            Oid = oid;
            this.variable = Null.Instance;
        }

        /**
         * Creates a variable binding with the supplied OID and value.
         * @param oid
         *    the OID for the new variable binding (must not be <code>null</code>).
         * @param variable
         *    the value for the new variable binding (must not be <code>null</code>).
         */
        public VariableBinding(OID oid, IVariable variable)
        {
            Oid = oid;
            Variable = variable;
        }

        /**
         * Creates a variable binding with the supplied OID and a text value.
         * The text value is parsed based on MIB information to a Variable
         * using the {@link VariableTextFormat} set by
         * {@link SNMP4JSettings#setVariableTextFormat(org.snmp4j.util.VariableTextFormat)}.
         * The default {@link org.snmp4j.util.SimpleVariableTextFormat} does not support
         * this operation. To be able to use this constructor, use the <code>SmiManager</code>
         * of SNMP4J-SMI instead.
         *
         * @param oid
         *    the {@link OID} of the new variable binding.
         * @param variableText
         *    the textual representation of a {@link Variable} of the syntax defined for <code>oid</code>
         *    by a MIB specification.
         * @throws ParseException
         *    if the <code>variableText</code> cannot be parsed or <code>oid</code>'s syntax is unknown.
         * @since 2.2
         */
        public VariableBinding(OID oid, string variableText) : this(oid, JunoSnmpSettings.VariableTextFormat.Parse(oid, variableText))
        {

        }

        /// <summary>
        /// Gets or sets the object instance identifier of the variable binding.
        /// </summary>
        public OID Oid
        {
            get
            {
                return this.oid;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException(
                        "OID of a VariableBinding must not be null");
                }
                this.oid = (OID)value.Clone();
            }
        }

        /// <summary>
        /// Gets or set the value of the variable binding.
        /// </summary>
        public IVariable Variable
        {
            get
            {
                return this.variable;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException(
                        "Variable of a VariableBinding must not be null");
                }

                this.variable = (IVariable)value.Clone();
            }
        }

        /**
         * Gets the syntax of the variable bindings value.
         * @return
         *   a SMI syntax identifier (see {@link SMIConstants}).
         */
        public int Syntax
        {
            get
            {
                return this.variable.Syntax;
            }
        }

        /**
         * Returns whether the variable bindings value has an exception syntax.
         * @see Variable
         * @return
         *    <code>true</code> if the syntax of this variable is an instance of
         *    <code>Null</code> and its syntax equals one of the following:
         *    <UL>
         *    <LI>{@link SMIConstants#EXCEPTION_NO_SUCH_OBJECT}</LI>
         *    <LI>{@link SMIConstants#EXCEPTION_NO_SUCH_INSTANCE}</LI>
         *    <LI>{@link SMIConstants#EXCEPTION_END_OF_MIB_VIEW}</LI>
         *    </UL>
         */
        public bool IsException
        {
            get
            {
                return this.variable.IsException;
            }

        }

        public int BERPayloadLength
        {
            get
            {
                return this.oid.BERLength + this.variable.BERLength;
            }

        }

        public int BERLength
        {
            get
            {
                int length = this.BERPayloadLength;
                // add type byte and length of length
                length += BER.GetBERLengthOfLength(length) + 1;
                return length;
            }

        }

        public void DecodeBER(BERInputStream inputStream)
        {
            int length = BER.DecodeHeader(inputStream, out byte type);
            long startPos = inputStream.Position;
            if (type != BER.SEQUENCE)
            {
                throw new IOException("Invalid sequence encoding: " + type);
            }

            this.oid.DecodeBER(inputStream);
            this.variable = AbstractVariable.CreateFromBER(inputStream);
            if (BER.CheckSequenceLengthFlag)
            {
                BER.CheckSequenceLength(length,
                                        (int)(inputStream.Position - startPos),
                                        this);
            }
        }

        public void EncodeBER(Stream outputStream)
        {
            int length = this.BERPayloadLength;
            BER.EncodeHeader(outputStream, BER.SEQUENCE,
                             length);
            this.oid.EncodeBER(outputStream);
            this.variable.EncodeBER(outputStream);
        }

        /**
         * Gets a string representation of this variable binding using the
         * {@link VariableTextFormat} configured by {@link SNMP4JSettings}.
         * @return
         *    a string of the form <code>&lt;OID&gt; = &lt;Variable&gt;</code>.
         */
        public override string ToString()
        {
            IVariableTextFormat varFormat = JunoSnmpSettings.VariableTextFormat;
            return varFormat.Format(this.oid, this.variable, true);
        }

        /**
         * Gets a string representation of this variable binding's value using the
         * {@link VariableTextFormat} configured by {@link SNMP4JSettings}.
         * @return
         *    a string of the form <code>&lt;Variable&gt;</code>.
         * @since 1.10
         */
        public string ToValueString()
        {
            IVariableTextFormat varFormat = JunoSnmpSettings.VariableTextFormat;
            return varFormat.Format(this.oid, this.variable, false);
        }

        public object Clone()
        {
            return new VariableBinding(this.oid, this.variable);
        }

        public override int GetHashCode()
        {
            return this.oid.GetHashCode();
        }

        public override bool Equals(Object o)
        {
            if (o is VariableBinding vb)
            {
                return this.Equals(vb);
            }

            return false;
        }

        public bool Equals(VariableBinding o)
        {
            return this.oid.Equals(o.Oid) && this.variable.Equals(o.Variable);
        }

        /**
         * Create an array of {@link VariableBinding}s based on the
         * provided OIDs.
         * @param oids
         *    an array of OIDs (must not be null).
         * @return
         *    an array of {@link VariableBinding}s where each
         *    the n-th binding's OID is the n-th OID out of
         *    <code>oids</code>.
         * @since 2.1
         */
        public static VariableBinding[] CreateFromOIDs(OID[] oids)
        {
            VariableBinding[] variableBindings = new VariableBinding[oids.Length];
            for (int i = 0; i < oids.Length; i++)
            {
                variableBindings[i] = new VariableBinding(oids[i]);
            }

            return variableBindings;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
