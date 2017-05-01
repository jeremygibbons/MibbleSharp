// <copyright file="TransportMappings.cs" company="None">
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

namespace JunoSnmp.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using JunoSnmp.SMI;
    /// <summary>
    /// The <code>TransportMappings</code> factory can be used to create a transport
    /// mapping for an address class.
    /// </summary>
    public class TransportMappings
    {

        private static readonly log4net.ILog log = log4net.LogManager
                  .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string TRANSPORT_MAPPINGS = "org.snmp4j.transportMappings";
        private static readonly string TRANSPORT_MAPPINGS_DEFAULT = "transports.properties";

        private static TransportMappings instance = null;
        private Dictionary<string, Type> transportMappings = null;

        protected TransportMappings()
        {
        }

        /**
         * Returns the <code>TransportMappings</code> singleton.
         * @return
         *    the <code>TransportMappings</code> instance.
         */
        public static TransportMappings GetInstance()
        {
            if (instance == null)
            {
                instance = new TransportMappings();
            }

            return instance;
        }

        /**
         * Returns a <code>TransportMapping</code> instance that is initialized with
         * the supplied transport address.
         * If no such mapping exists, <code>null</code> is returned. To register
         * third party transport mappings, please set the system property
         * {@link #TRANSPORT_MAPPINGS} to a transport mappings registration file,
         * before calling this method for the first time.
         *
         * @param transportAddress
         *   an <code>Address</code> instance that the transport mapping to lookup
         *   has to support.
         * @return
         *   a <code>TransportMapping</code> that supports the specified
         *   <code>transportAddress</code> or <code>null</code> if such a mapping
         *   cannot be found.
         */
        public ITransportMapping<IAddress> CreateTransportMapping(IAddress transportAddress)
        {
            if (transportMappings == null)
            {
                RegisterTransportMappings();
            }
            Type c = transportMappings[transportAddress.GetType().Name];

            if (c == null)
            {
                return null;
            }

            try
            {
                return (ITransportMapping<IAddress>)Activator.CreateInstance(c, transportAddress.GetType());
            }
            catch(Exception)
            {
                return null;
            }

            //Class[] prms = new Class[1];
            //prms[0] = transportAddress.getClass();
            //Constructor <? extends TransportMapping > constructor;
            //try
            //{
            //    try
            //    {
            //        constructor = c.getConstructor(prms);
            //        return constructor.newInstance(transportAddress);
            //    }
            //    catch (NoSuchMethodException nsme)
            //    {
            //        Constructor <? extends TransportMapping >[] cs = (Constructor <? extends TransportMapping >[]) c.getConstructors();
            //        foreach (Constructor<? extends TransportMapping> cons in cs)
            //        {
            //            Class[] params2 = cons.getParameterTypes();
            //            if ((params2.length == 1) && (params2[0].isAssignableFrom(prms[0])))
            //            {
            //                return cons.newInstance(transportAddress);
            //            }
            //        }
            //        logger.error("NoSuchMethodException while instantiating " + c.getName(), nsme);
            //        return null;
            //    }
            //}
            //catch (InvocationTargetException ite)
            //{
            //    if (logger.isDebugEnabled())
            //    {
            //        ite.printStackTrace();
            //    }
            //    logger.error(ite);
            //    throw new RuntimeException(ite.getTargetException());
            //}
            //catch (Exception ex)
            //{
            //    if (logger.isDebugEnabled())
            //    {
            //        ex.printStackTrace();
            //    }
            //    logger.error(ex);
            //    return null;
            //}
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void RegisterTransportMappings()
        {
            if (JunoSnmpSettings.ExtensibilityEnabled)
            {
                //                String transports =
                //                    System.getProperty(TRANSPORT_MAPPINGS, TRANSPORT_MAPPINGS_DEFAULT);
                //                InputStream is = TransportMappings.class.getResourceAsStream(transports);
                //      if (is == null) {
                //        throw new InternalError("Could not read '" + transports +
                //                                "' from classpath!");
                //    }
                //    Properties props = new Properties();
                //      try {
                //        props.load(is);
                //        Hashtable<String, Class<? extends TransportMapping>> t =
                //            new Hashtable<String, Class<? extends TransportMapping>>(props.size());
                //        for (Enumeration en = props.propertyNames(); en.hasMoreElements(); ) {
                //          String addressClassName = en.nextElement().toString();
                //    String className = props.getProperty(addressClassName);
                //          try {
                //            Class<? extends TransportMapping> c = (Class <? extends TransportMapping >)Class.forName(className);
                //            t.put(addressClassName, c);
                //          }
                //          catch (ClassNotFoundException cnfe) {
                //            logger.error(cnfe);
                //          }
                //        }
                //        // atomic syntax registration
                //        transportMappings = t;
                //      }
                //      catch (IOException iox) {
                //        String txt = "Could not read '" + transports + "': " +
                //            iox.getMessage();
                //logger.error(txt);
                //        throw new InternalError(txt);
                //      }
                //      finally {
                //        try {
                //          is.close();
                //        }
                //        catch (IOException ex) {
                //          logger.warn(ex);
                //        }
                //      }
            }
            else
            {
                Dictionary<string, Type> t = new Dictionary<string, Type>();

                t[nameof(UdpAddress)] = typeof(DefaultUdpTransportMapping);
                //t[nameof(TcpAddress)] = typeof(DefaultTcpTransportMapping);
                //TODO: uncomment this line to re-enable TLS
                //t[nameof(TlsAddress)] = typeof(TLSTM);
                transportMappings = t;
            }
        }

    }

}
