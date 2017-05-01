// <copyright file="AbstractVariable.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <code>Variable</code> abstract class is the base class for all SNMP
    /// variables.
    /// All derived classes need to be registered with their SMI BER type in the
    /// <c>smisyntaxes.properties</c> so that the
    /// <see cref="CreateFromBER(BERInputStream inputStream)"/> method
    /// is able to decode a variable from a BER encoded stream.
    /// To register additional syntaxes, set the system property <see cref="SMISyntaxesProperties"/> 
    /// before decoding an IVariable for the first
    /// time. The path of the property file must be accessible from the classpath
    /// and it has to be specified relative to the <c>IVariable</c> class.
    /// </summary>
    public abstract class AbstractVariable : IVariable, ISerializable
    {

        public static readonly string SMISyntaxesProperties =
          "org.snmp4j.smisyntaxes";
        private static readonly string SMISyntaxesPropertiesDefault =
            "smisyntaxes.properties";

        private static readonly Dictionary<string, int> SYNTAX_NAME_MAPPING = new Dictionary<string, int>()
        {
            { "Integer32", (int) BER.INTEGER32},
            { "BIT STRING", (int) BER.BITSTRING},
            { "OCTET STRING", (int) BER.OCTETSTRING},
            { "OBJECT IDENTIFIER", (int) BER.OID},
            { "TimeTicks", (int) BER.TIMETICKS},
            { "Counter", (int) BER.COUNTER},
            { "Counter64", (int) BER.COUNTER64},
            { "EndOfMibView", BER.EndOfMibView},
            { "Gauge", (int) BER.GAUGE32},
            { "Unsigned32", (int) BER.GAUGE32},
            { "IpAddress", (int) BER.IPADDRESS},
            { "NoSuchInstance", BER.NoSuchInstance},
            { "NoSuchObject", BER.NoSuchObject},
            { "Null", (int) BER.NULL},
            { "Opaque", (int) BER.OPAQUE}
        };

        private static Dictionary<int, IVariable> registeredSyntaxes = null;

        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The abstract <code>Variable</code> class serves as the base class for all
        /// specific SNMP syntax types.
        /// </summary>
        protected AbstractVariable()
        {
        }

        /// <summary>
        /// Gets the length of this <c>IVariable</c> in bytes when encoded
        /// according to the Basic Encoding Rules(BER).
        /// </summary>
        public abstract int BERLength { get; }

        /// <summary>
        /// Gets the payload length of this <c>IVariable</c> in bytes when 
        /// encoded according to the Basic Encoding Rules(BER).
        /// </summary>
        public virtual int BERPayloadLength
        {
            get
            {
                return this.BERLength;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this variable is dynamic, which means
        /// that it might change its value while it is being(BER) serialized.
        /// If a variable is dynamic, it will be cloned on-the-fly when it is added 
        /// to a <see cref="PDU"/> with <see cref="VariableBinding"/>. By cloning the 
        /// value, it is ensured that there are no inconsistent changes between 
        /// determining the length with <see cref="BERLength"/> for encoding 
        /// enclosing SEQUENCES and the actual encoding of the Variable itself 
        /// with <see cref="EncodeBER(Stream)"/>.
        /// </summary>
        /// <remarks>
        /// False by default. Derived classes may override this if implementating dynamic
        /// <see cref="IVariable"/> instances.
        /// </remarks>
        public virtual bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a textual description of this Variable, such as "Integer32", as used
        /// in the Structure of Management Information (SMI) modules.
        /// '?' is returned if the syntax is unknown.
        /// </summary>
        public string SyntaxString
        {
            get
            {
                return AbstractVariable.GetSyntaxString(this.Syntax);
            }

        }

        /// <summary>
        /// Gets the ASN.1 syntax identifier value of this SNMP variable,
        /// i.e. an integer value &lt; 128 for regular SMI objects and a value
        /// &gt;= for exception values like <see cref="SMIConstants.ExceptionNoSuchObject"/>,
        /// <see cref="SMIConstants.ExceptionNoSuchInstance"/> or 
        /// <see cref="SMIConstants.ExceptionEndOfMibView"/>
        /// </summary>
        public abstract int Syntax { get; }
        
        /// <summary>
        /// Checks whether this variable represents an exception like 
        /// <see cref="SMIConstants.ExceptionNoSuchObject"/>,
        /// <see cref="SMIConstants.ExceptionNoSuchInstance"/> or 
        /// <see cref="SMIConstants.ExceptionEndOfMibView"/>, i.e. if the variable is
        /// an instance of null and its syntax is one of those exception syntaxes.
        /// </summary>
        public bool IsException
        {
            get
            {
                return Null.IsExceptionSyntax(this.Syntax);
            }
        }
        
        /// <summary>
        /// Returns an integer representation of this variable if
        /// such a representation exists.
        /// </summary>
        /// <remarks>
        /// If the native representation of this variable is a long, it will be
        /// downcast to int here.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// If no integer representation exists for this variable
        /// </exception>
        public abstract int IntValue { get; }
        
        /// <summary>
        /// Returns a long representation of this variable if
        /// such a representation exists.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If no long representation exists for this variable
        /// </exception>
        public abstract long LongValue { get; }
        
        /// <summary>
        /// Decodes an <c>IVariable</c> from an <c>InputStream</c>.
        /// </summary>
        /// <param name="inputStream">
        /// An input stream containing a BER encoded byte stream
        /// </param>
        /// <exception cref="IOException">
        /// If the stream could not be decoded according to BER rules
        /// </exception>
        public abstract void DecodeBER(BERInputStream inputStream);
        
        /// <summary>
        /// Encodes an <c>IVariable</c> to an output <c>Stream</c>.
        /// </summary>
        /// <param name="outputStream">The stream to write to</param>
        /// <exception cref="IOException">
        /// If an error occurs while writing to the stream
        /// </exception>
        public abstract void EncodeBER(Stream outputStream);
        
        /// <summary>
        /// Creates an <c>IVariable</c> from a BER encoded <see cref="BERInputStream"/>.
        /// Implementations of <c>IVariable</c> are registered using the properties file
        /// <code>smisyntaxes.properties</code> in this package.The properties are
        /// read when this method is called first.
        /// </summary>
        /// <param name="inputStream">The stream to be read from</param>
        /// <returns>An <see cref="IVariable"/> instance</returns>
        public static IVariable CreateFromBER(BERInputStream inputStream)
        {
            long startPos = inputStream.Position;
            int type = inputStream.ReadByte();
            IVariable variable;

            variable = CreateVariable(type);
            inputStream.Position = startPos;
            variable.DecodeBER(inputStream);
            return variable;
        }

        /// <summary>
        /// Creates a <code>Variable</code> from the supplied SMI syntax identifier.
        /// </summary>
        public static IVariable CreateFromSyntax(int smiSyntax)
        {
            return CreateVariable(smiSyntax);
        }
     
        /// <summary>
        /// Gets a string representation of the variable.
        /// </summary>
        /// <remarks>
        /// Declaring this method abstract forces subclasses to implement ToString explicitely
        /// </remarks>
        /// <returns>
        /// A string representation of the variable's value
        /// </returns>
        public abstract override string ToString();
        
        /// <summary>
        /// Creates a fresh copy of this object with identical values
        /// </summary>
        /// <returns>A fresh copy of this object</returns>
        public abstract object Clone();
        
        /// <summary>
        /// Gets a textual description of the supplied syntax type.
        /// </summary>
        /// <param name="syntax">The BER code of the syntax</param>
        /// <returns>
        /// a textual description like 'Integer32' for <code>syntax</code> 
        /// as used in the Structure of Management Information(SMI) modules.
        /// '?' is returned if the supplied syntax is unknown.
        /// </returns>
        public static string GetSyntaxString(int syntax)
        {
            switch (syntax)
            {
                case BER.INTEGER:
                    return "Integer32";
                case BER.BITSTRING:
                    return "BIT STRING";
                case BER.OCTETSTRING:
                    return "OCTET STRING";
                case BER.OID:
                    return "OBJECT IDENTIFIER";
                case BER.TIMETICKS:
                    return "TimeTicks";
                case BER.COUNTER:
                    return "Counter";
                case BER.COUNTER64:
                    return "Counter64";
                case BER.EndOfMibView:
                    return "EndOfMibView";
                case BER.GAUGE32:
                    return "Gauge";
                case BER.IPADDRESS:
                    return "IpAddress";
                case BER.NoSuchInstance:
                    return "NoSuchInstance";
                case BER.NoSuchObject:
                    return "NoSuchObject";
                case BER.NULL:
                    return "Null";
                case BER.OPAQUE:
                    return "Opaque";
            }
            return "?";
        }
                
        /// <summary>
        /// Returns the BER syntax ID for the supplied syntax string (as returned
        /// by <see cref="GetSyntaxString(int)"/>
        /// </summary>
        /// <param name="syntaxString">The textual representation of the syntax</param>
        /// <returns>The corresponding BER ID.</returns>
        public static int GetSyntaxFromString(string syntaxString)
        {
            int syn ;
            if(SYNTAX_NAME_MAPPING.TryGetValue(syntaxString, out syn))
            {
                return syn;
            }
            return BER.NULL;
        }
        
        /// <summary>
        /// Converts the value of this <c>Variable</c> to a (sub-)index value
        /// </summary>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. { @link OctetString}
        /// and <see cref="OID"/>. For other variables it has no effect.
        /// </param>
        /// <returns>An OID that represents this value as a sub index</returns>
        /// <exception cref="NotSupportedException">If this variable cannot be used as a sub index</exception>
        public abstract OID ToSubIndex(bool impliedLength);

        /// <summary>
        /// Sets the value of this <c>IVariable</c> from the supplied (sub-)index.
        /// </summary>
        /// <param name="subIndex">The sub-index OID</param>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. <see cref="OctetString"/>
        /// and <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <exception cref="NotSupportedException">If this variable cannot be set from a sub index</exception>
        public abstract void FromSubIndex(OID subIndex, bool impliedLength);

        /// <summary>
        /// Compares this object with another object
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>True if both objects are of equal value, false if not</returns>
        public abstract override bool Equals(object o);

        /// <summary>
        /// Compares this object with another object
        /// </summary>
        /// <param name="o">The object to compare against</param>
        /// <returns>0 if both objects are of equal value, a non-zero value if not</returns>
        public abstract int CompareTo(IVariable o);

        /// <summary>
        /// Gets a hashcode for this object
        /// </summary>
        /// <returns>A hashcode for this object</returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Tests if two variables have the same value
        /// </summary>
        /// <param name="a">A variable</param>
        /// <param name="b">Another variable</param>
        /// <returns>
        /// <c>True</c> if <c>a == null ? (b == null) : a.Equals(b)</c>
        /// </returns>
        public static bool Equal(AbstractVariable a, AbstractVariable b)
        {
            return (a == null) ? (b == null) : a.Equals(b);
        }

        /// <summary>
        /// Writes this objects data to a stream for serialization
        /// </summary>
        /// <param name="info">The SerializationInfo object</param>
        /// <param name="context">The streaming context</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Factory method for new IVariable instances
        /// </summary>
        /// <param name="smiSyntax">The syntax for which a variable should be created</param>
        /// <returns>A new instance of the designated IVariable type</returns>
        private static IVariable CreateVariable(int smiSyntax)
        {
            //TODO: This seems like a very ugly way of doing this
            switch (smiSyntax)
            {
                case SMIConstants.SyntaxObjectIdentifier:
                    {
                        return new OID();
                    }
                case SMIConstants.SyntaxInteger:
                    {
                        return new Integer32();
                    }
                case SMIConstants.SyntaxOctetString:
                    {
                        return new OctetString();
                    }
                case SMIConstants.SyntaxGauge32:
                    {
                        return new Gauge32();
                    }
                case SMIConstants.SyntaxCounter32:
                    {
                        return new Counter32();
                    }
                case SMIConstants.SyntaxCounter64:
                    {
                        return new Counter64();
                    }
                case SMIConstants.SyntaxNull:
                    {
                        return new Null();
                    }
                case SMIConstants.SyntaxTimeTicks:
                    {
                        return new TimeTicks();
                    }
                case SMIConstants.ExceptionEndOfMibView:
                    {
                        return new Null(SMIConstants.ExceptionEndOfMibView);
                    }
                case SMIConstants.ExceptionNoSuchInstance:
                    {
                        return new Null(SMIConstants.ExceptionNoSuchInstance);
                    }
                case SMIConstants.ExceptionNoSuchObject:
                    {
                        return new Null(SMIConstants.ExceptionNoSuchObject);
                    }
                case SMIConstants.SyntaxOpaque:
                    {
                        return new Opaque();
                    }
                case SMIConstants.SyntaxIpAddress:
                    {
                        return new IpAddress();
                    }
                default:
                    {
                        throw new ArgumentException("Unsupported variable syntax: " +
                                                           smiSyntax);
                    }
            }
        }
    }
}
