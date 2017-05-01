// <copyright file="GenericAddress.cs" company="None">
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
    using System.Runtime.CompilerServices;
    using JunoSnmp.ASN1;
    /// <summary>
    /// The <code>GenericAddress</code> implements the decorator and factory
    /// design pattern to provide a generic address type.
    /// To register address types other than the default, set the system property
    /// <see cref="ADDRESS_TYPES_PROPERTIES"/> before calling the <see cref="Parse"/> method
    /// for the first time.
    /// </summary>
    public class GenericAddress : SMIAddress, IEquatable<GenericAddress>
    {
        /// <summary>
        /// Default address type identifier for an UpdAddress.
        /// </summary>
        public static readonly string TYPE_UDP = "udp";

        /// <summary>
        /// Default address type identifier for a TcpAddress.
        /// </summary>
        public static readonly string TYPE_TCP = "tcp";

        /// <summary>
        /// Default address type identifier for an IpAddress.
        /// </summary>
        public static readonly string TYPE_IP = "ip";

        /// <summary>
        /// Default address type identifier for an TlsAddress.
        /// </summary>
        public static readonly string TYPE_TLS = "tls";

        public static readonly string ADDRESS_TYPES_PROPERTIES =
            "org.junosnmp.addresses";

        private static readonly string ADDRESS_TYPES_PROPERTIES_DEFAULT =
            "address.properties";

        private static readonly log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SMIAddress address;
        private static Dictionary<string, Func<IAddress>> knownAddressTypeGenerators = new Dictionary<string, Func<IAddress>>();
        private static Dictionary<string, Type> knownAddressTypes = null;

        public GenericAddress()
        {
            RegisterAddressTypes();
        }

        public GenericAddress(SMIAddress address)
        {
            RegisterAddressTypes();
            this.address = address;
        }

        public override int Syntax
        {
            get
            {
                return this.address.Syntax;
            }
        }

        public override bool IsValid
        {
            get
            {
                return (this.address != null) && this.address.IsValid;
            }
        }

        public override string ToString()
        {
            return this.address.ToString();
        }

        public override int GetHashCode()
        {
            return this.address.GetHashCode();
        }

        public override int CompareTo(IVariable o)
        {
            return this.address.CompareTo(o);
        }

        public override bool Equals(object o)
        {
            return this.address.Equals(o);
        }

        public bool Equals(GenericAddress g)
        {
            return this.address.Equals(g);
        }

        public override void DecodeBER(BERInputStream inputStream)
        {
            throw new NotSupportedException();
        }
        public override void EncodeBER(Stream outputStream)
        {
            this.address.EncodeBER(outputStream);
        }

        public override int BERLength
        {
            get
            {
                return this.address.BERLength;
            }
        }

        public IAddress Address
        {
            get
            {
                return this.address;
            }

            set
            {
                SMIAddress sa = value as SMIAddress;
                if (sa == null)
                {
                    throw new ArgumentException("Argument is not an SMIAddress instance: " + value);
                }

                this.address = sa;
            }
        }
        /**
         * Register Address classes from a properties file. The registered
         * address types are used by the {@link GenericAddress#parse(String)}
         * method to type-safe instantiate sub-classes from <code>Address</code>
         * from a <code>String</code>.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void RegisterAddressTypes()
        {
            /*
            if (JunoSnmpSettings.ExtensibilityEnabled)
            {
                String addresses = System.getProperty(ADDRESS_TYPES_PROPERTIES,
                                                      ADDRESS_TYPES_PROPERTIES_DEFAULT);
                InputStream ins = Variable.getclass.getResourceAsStream(addresses);
                if (ins == null)
                {
                    throw new InternalError("Could not read '" + addresses +
                                            "' from classpath!");
                }
                Properties props = new Properties();
                try
                {
                    props.load(ins);
                    Dictionary<string, IAddress> h = new TreeMap<String, Class<? extends Address>>();
                    for (Enumeration en = props.propertyNames(); en.hasMoreElements();)
                    {
                        String id = en.nextElement().toString();
                        String className = props.getProperty(id);
                        try
                        {
                            IAddress c = (IAddress)Class.forName(className);
                            h.put(id, c);
                        }
                        catch (ClassNotFoundException cnfe)
                        {
                            logger.error(cnfe);
                        }
                        catch (ClassCastException ccex)
                        {
                            logger.error("Class name '" + className + "' is not a subclass of " + Address.getclass.getName());
                        }
                    }
                    knownAddressTypes = h;
                }
                catch (IOException iox)
                {
                    String txt = "Could not read '" + addresses + "': " + iox.getMessage();
                    logger.error(txt);
                    throw new InternalError(txt);
                }
                finally
                {
                    try
                    {
                        ins.close();
                    }
                    catch (IOException ex)
                    {
                        // ignore
                        logger.warn(ex);
                    }
                }
            }
            else
            {
                Dictionary<string, Type> h = new Dictionary<string, Type>();
                h.Add(TYPE_UDP, typeof(UdpAddress));
                h.Add(TYPE_TCP, typeof(TcpAddress));
                h.Add(TYPE_IP, typeof(IpAddress));
                h.Add(TYPE_TLS, typeof(TlsAddress));
                knownAddressTypes = h;
            }
            */
            GenericAddress.knownAddressTypes = new Dictionary<string, Type>();

            GenericAddress.knownAddressTypeGenerators.Add(TYPE_UDP, () => new UdpAddress());
            GenericAddress.knownAddressTypeGenerators.Add(TYPE_TCP, () => new TcpAddress());
            GenericAddress.knownAddressTypeGenerators.Add(TYPE_IP, () => new IpAddress());
            GenericAddress.knownAddressTypeGenerators.Add(TYPE_TLS, () => new TlsAddress());

            GenericAddress.knownAddressTypes.Add(TYPE_UDP, typeof(UdpAddress));
            GenericAddress.knownAddressTypes.Add(TYPE_TCP, typeof(TcpAddress));
            GenericAddress.knownAddressTypes.Add(TYPE_IP, typeof(IpAddress));
            GenericAddress.knownAddressTypes.Add(TYPE_TLS, typeof(TlsAddress));
        }

        /**
         * Parses a given transport protocol dependent address string into an
         * <code>Address</code> instance that is subsumed by this
         * <code>GenericAddress</code> object.
         *
         * @param address
         *    an address string with a leading type specifier as defined in the
         *    "address.properties". The format is <code>"type:address"</code> where
         *    the format of <code>address</code> depends on <code>type</code>.
         *    Valid values for <code>type</code> are, for example, "udp" and "tcp".
         * @return
         *    a <code>Address</code> instance of the address classes specified
         *    in "address.properties" whose type ID matched the specified ID in
         *    <code>address</code>. If <code>address</code> cannot be parsed,
         *    <code>null</code> is returned.
         * @throws IllegalArgumentException
         *    if the address type indicator supplied is not know.
         */
        public static IAddress Parse(string address)
        {
            string type = TYPE_UDP;
            int sep = address.IndexOf(':');
            if (sep > 0)
            {
                type = address.Substring(0, sep);
                address = address.Substring(sep + 1);
            }

            type = type.ToLowerInvariant();

            if(knownAddressTypeGenerators.ContainsKey(type) == false)
            {
                throw new ArgumentException("Address type " + type + " unknown");
            }

            IAddress addr = knownAddressTypeGenerators[type].Invoke();

            if (addr.ParseAddress(address))
            {
                return addr;
            }

            return null;
        }

        /**
         * Parse an address form the supplied string.
         * @param address
         *    an address string known by the GenericAddress.
         * @return boolean
         * @see #parse(String address)
         */
        public override bool ParseAddress(string address)
        {
            IAddress addr = GenericAddress.Parse(address);
            if (addr is SMIAddress)
            {
                this.Address = addr;
                return true;
            }

            return false;
        }

        public override void SetValue(byte[] rawAddress)
        {
            this.address.SetValue(rawAddress);
        }

        public override object Clone()
        {
            return new GenericAddress(address);
        }

        public override int IntValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long LongValue
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override OID ToSubIndex(bool impliedLength)
        {
            throw new NotSupportedException();
        }

        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            throw new NotSupportedException();
        }

        public override void SetValue(string value)
        {
            if (!ParseAddress(value))
            {
                throw new ArgumentException(value + " cannot be parsed by " +
                                                   this.GetType().Name);
            }
        }

        /**
         * Gets the transport domain prefix string (lowercase) for a supplied
         * {@link Address} class.
         * @param addressClass
         *    an implementation class of {@link Address}.
         * @return
         *    the corresponding transport domain prefix as defined by the
         *    IANA registry "SNMP Transport Domains" if the <code>addressClass</code>
         *    has been registered with a domain prefix, <code>null</code> otherwise.
         * @since 2.0
         */
        public static string GetTDomainPrefix(Type addressType)
        {
            if (GenericAddress.knownAddressTypes == null)
            {
                GenericAddress.RegisterAddressTypes();
            }

            foreach (string key in knownAddressTypes.Keys)
            {
                if(GenericAddress.knownAddressTypes[key] == addressType)
                {
                    return key;
                }
            }

            return null;
        }
    }
}
