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
    ////using System.Runtime.CompilerServices;
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

        private static readonly object[][] SYNTAX_NAME_MAPPING = new object[][]
        {
            new object[] { "Integer32", (int) BER.INTEGER32},
            new object[] { "BIT STRING", (int) BER.BITSTRING},
            new object[] { "OCTET STRING", (int) BER.OCTETSTRING},
            new object[] { "OBJECT IDENTIFIER", (int) BER.OID},
            new object[] { "TimeTicks", (int) BER.TIMETICKS},
            new object[] { "Counter", (int) BER.COUNTER},
            new object[] { "Counter64", (int) BER.COUNTER64},
            new object[] { "EndOfMibView", BER.EndOfMibView},
            new object[] { "Gauge", (int) BER.GAUGE32},
            new object[] { "Unsigned32", (int) BER.GAUGE32},
            new object[] { "IpAddress", (int) BER.IPADDRESS},
            new object[] { "NoSuchInstance", BER.NoSuchInstance},
            new object[] { "NoSuchObject", BER.NoSuchObject},
            new object[] { "Null", (int) BER.NULL},
            new object[] { "Opaque", (int) BER.OPAQUE}
        };

        private static Dictionary<int, IVariable> registeredSyntaxes = null;

        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The abstract <code>Variable</code> class serves as the base class for all
        /// specific SNMP syntax types.
        /// </summary>
        public AbstractVariable()
        {
        }

        public abstract override bool Equals(object o);

        public abstract int CompareTo(IVariable o);

        public abstract override int GetHashCode();

        /// <summary>
        /// Gets the length of this <c>IVariable</c> in bytes when encoded
        /// according to the Basic Encoding Rules(BER).
        /// </summary>
        public abstract int BERLength { get; }


        public virtual int BERPayloadLength
        {
            get
            {
                return this.BERLength;
            }
        }

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

        /**
         * Encodes a <code>Variable</code> to an <code>OutputStream</code>.
         * @param outputStream
         *    an <code>OutputStream</code>.
         * @throws IOException
         *    if an error occurs while writing to the stream.
         */
        public abstract void EncodeBER(Stream outputStream);

        /**
         * Creates a <code>Variable</code> from a BER encoded <code>InputStream</code>.
         * Subclasses of <code>Variable</code> are registered using the properties file
         * <code>smisyntaxes.properties</code> in this package. The properties are
         * read when this method is called first.
         *
         * @param inputStream
         *    an <code>BERInputStream</code> containing a BER encoded byte stream.
         * @return
         *    an instance of a subclass of <code>Variable</code>.
         * @throws IOException
         *    if the <code>inputStream</code> is not properly BER encoded.
         */
        public static IVariable CreateFromBER(BERInputStream inputStream)
        {
            ////if (JunoSnmpSettings.ExtensibilityEnabled &&
            ////    (registeredSyntaxes == null))
            ////{
            ////AbstractVariable.RegisterSyntaxes();
            ////}

            long startPos = inputStream.Position;
            int type = inputStream.ReadByte();
            IVariable variable;
            ////if (JunoSnmpSettings.ExtensibilityEnabled)
            ////{
            ////IVariable c = registeredSyntaxes[type];
            //// if (c == null)
            ////{
            ////throw new IOException("Encountered unsupported variable syntax: " +
            ////type);
            ////}
            ////
            ////try
            ////{

            ////variable = c.NewInstance();
            ////}
            ////catch (IllegalAccessException aex)
            ////{
            ////throw new IOException("Could not access variable syntax class for: " +
            ////c.getName());
            ////}
            ////catch (InstantiationException iex)
            ////{
            ////throw new IOException(
            ////"Could not instantiate variable syntax class for: " +
            ////c.getName);
            ////}
            ////}
            ////else
            ////{
            variable = CreateVariable(type);
            ////}
            inputStream.Position = startPos;
            variable.DecodeBER(inputStream);
            return variable;
        }

        private static IVariable CreateVariable(int smiSyntax)
        {
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

        /**
         * Creates a <code>Variable</code> from the supplied SMI syntax identifier.
         * Subclasses of <code>Variable</code> are registered using the properties
         * file <code>smisyntaxes.properties</code> in this package. The properties
         * are read when this method is called for the first time.
         *
         * @param smiSyntax
         *    an SMI syntax identifier of the registered types, which is typically
         *    defined by {@link SMIConstants}.
         * @return
         *    a <code>Variable</code> variable instance of the supplied SMI syntax.
         */
        public static IVariable CreateFromSyntax(int smiSyntax)
        {
            ////if (!JunoSnmpSettings.ExtensibilityEnabled)
            ////{
            return CreateVariable(smiSyntax);
            ////}
            ////if (registeredSyntaxes == null)
            ////{
            ////RegisterSyntaxes();
            ////}
            ///Class <? extends Variable > c = registeredSyntaxes.get(new Integer(smiSyntax));
            ////if (c == null)
            ////{
            //// throw new ArgumentException("Unsupported variable syntax: " +
            ////smiSyntax);
            ////}
            ////try
            ////{
            ////IVariable variable = c.newInstance();
            ////return variable;
            ////}
            ////catch (IllegalAccessException aex)
            ////{
            ////throw new RuntimeException("Could not access variable syntax class for: " +
            ////c.getName());
            ////}
            ////catch (InstantiationException iex)
            ////{
            ////throw new RuntimeException(
            ////"Could not instantiate variable syntax class for: " +
            ////c.getName());
            ////}
        }

        /**
         * Register SNMP syntax classes from a properties file. The registered
         * syntaxes are used by the {@link #createFromBER} method to type-safe
         * instantiate sub-classes from <code>Variable</code> from an BER encoded
         * <code>InputStream</code>.
         */
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private static void RegisterSyntaxes()
        //{
        //    string syntaxes = System.getProperty(SMISyntaxesProperties,
        //                                         SMISyntaxesPropertiesDefault);
        //    InputStream ins = Variable.getclass.getResourceAsStream(syntaxes);
        //    if (ins == null)
        //    {
        //        throw new InternalError("Could not read '" + syntaxes +
        //                                "' from classpath!");
        //    }
        //    Properties props = new Properties();
        //    try
        //    {
        //        props.load(ins);
        //        ///Hashtable<Integer, Class<? extends Variable>> regSyntaxes = new Hashtable<Integer, Class<? extends Variable>>(props.size());
        //        for (Enumeration en = props.propertyNames(); en.hasMoreElements();)
        //        {
        //            string id = en.nextElement().toString();
        //            string className = props.getProperty(id);
        //            try
        //            {
        //                //Class<? extends Variable> c = (Class <? extends Variable >) Class.forName(className);
        //                regSyntaxes.put(new Integer(id), c);
        //            }
        //            catch (ClassNotFoundException cnfe)
        //            {
        //                logger.error(cnfe);
        //            }
        //            catch (ClassCastException ccex)
        //            {
        //                log.Error(ccex);
        //            }
        //        }
        //        // atomic syntax registration
        //        registeredSyntaxes = regSyntaxes;
        //    }
        //    catch (IOException iox)
        //    {
        //        string txt = "Could not read '" + syntaxes + "': " +
        //            iox.Message;
        //        log.Error(txt);
        //        throw new InternalError(txt);
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            ins.close();
        //        }
        //        catch (IOException ex)
        //        {
        //            log.Warn(ex);
        //        }
        //    }
        //}

        /**
         * Gets the ASN.1 syntax identifier value of this SNMP variable.
         * @return
         *    an integer value < 128 for regular SMI objects and a value >= 128
         *    for exception values like noSuchObject, noSuchInstance, and
         *    endOfMibView.
         */
        public abstract int Syntax { get; }

        /**
         * Checks whether this variable represents an exception like
         * noSuchObject, noSuchInstance, and endOfMibView.
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
                return Null.IsExceptionSyntax(this.Syntax);
            }
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

        /**
         * Returns an integer representation of this variable if
         * such a representation exists.
         * @return
         *    an integer value (if the native representation of this variable
         *    would be a long, then the long value will be casted to int).
         * @throws UnsupportedOperationException if an integer representation
         * does not exists for this Variable.
         * @since 1.7
         */
        public abstract int IntValue { get; }

        /**
         * Returns a long representation of this variable if
         * such a representation exists.
         * @return
         *    a long value.
         * @throws UnsupportedOperationException if a long representation
         * does not exists for this Variable.
         * @since 1.7
         */
        public abstract long LongValue { get; }

        public abstract object Clone();

        /**
         * Gets a textual description of the supplied syntax type.
         * @param syntax
         *    the BER code of the syntax.
         * @return
         *    a textual description like 'Integer32' for <code>syntax</code>
         *    as used in the Structure of Management Information (SMI) modules.
         *    '?' is returned if the supplied syntax is unknown.
         */
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

        /**
         * Gets a textual description of this Variable.
         * @return
         *    a textual description like 'Integer32'
         *    as used in the Structure of Management Information (SMI) modules.
         *    '?' is returned if the syntax is unknown.
         * @since 1.7
         */
        public string SyntaxString
        {
            get
            {
                return AbstractVariable.GetSyntaxString(this.Syntax);
            }
            
        }

        /**
         * Returns the BER syntax ID for the supplied syntax string (as returned
         * by {@link #getSyntaxString(int)}).
         * @param syntaxString
         *    the textual representation of the syntax.
         * @return
         *    the corresponding BER ID.
         * @since 1.6
         */
        public static int GetSyntaxFromString(string syntaxString)
        {
            foreach (object[] aSYNTAX_NAME_MAPPING in SYNTAX_NAME_MAPPING)
            {
                if (aSYNTAX_NAME_MAPPING[0].Equals(syntaxString))
                {
                    return (int)aSYNTAX_NAME_MAPPING[1];
                }
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

        /**
         * Indicates whether this variable is dynamic, which means that it might
         * change its value while it is being (BER) serialized. If a variable is
         * dynamic, it will be cloned on-the-fly when it is added to a {@link PDU}
         * with {@link PDU#add(VariableBinding)}. By cloning the value, it is
         * ensured that there are no inconsistent changes between determining the
         * length with {@link #getBERLength()} for encoding enclosing SEQUENCES and
         * the actual encoding of the Variable itself with {@link #encodeBER}.
         *
         * @return
         *    <code>false</code> by default. Derived classes may override this
         *    if implementing dynamic {@link Variable} instances.
         * @since 1.8
         */
        public virtual bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        /**
         * Tests if two variables have the same value.
         * @param a
         *   a variable.
         * @param b
         *   another variable.
         * @return
         *   <code>true</code> if
         *   <code>a == null) ?  (b == null) : a.equals(b)</code>.
         *   @since 2.0
         */
        public static bool Equal(AbstractVariable a, AbstractVariable b)
        {
            return (a == null) ? (b == null) : a.Equals(b);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
