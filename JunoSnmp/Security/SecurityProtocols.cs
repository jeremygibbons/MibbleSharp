// <copyright file="Salt.cs" company="None">
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

namespace JunoSnmp.Security
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>SecurityProtocols</code> class holds all authentication and
    /// privacy protocols for a SNMP entity.<para>
    /// To register security protocols other than the default, set the system
    /// property <see cref="SECURITY_PROTOCOLS_PROPERTIES"/> to a customized version
    /// of the <c>SecurityProtocols.properties</c> file. The path has to
    /// be specified relatively to this class.
    /// </para>
    /// </summary>
    public class SecurityProtocols : ISerializable
    {

        private readonly Dictionary<OID, IAuthenticationProtocol> authProtocols;
        private readonly Dictionary<OID, IPrivacyProtocol> privProtocols;

        public static readonly string SECURITY_PROTOCOLS_PROPERTIES =
          "org.junosnmp.securityProtocols";
        private static readonly string SECURITY_PROTOCOLS_PROPERTIES_DEFAULT =
            "SecurityProtocols.properties";
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SecurityProtocols instance = null;
        private int maxAuthDigestLength = 0;
        private int maxPrivDecryptParamsLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityProtocols"/> class. As the class
        /// is a singleton, this should not be called by other code.
        /// </summary>
        protected SecurityProtocols()
        {
            authProtocols = new Dictionary<OID, IAuthenticationProtocol>(5);
            privProtocols = new Dictionary<OID, IPrivacyProtocol>(5);
        }
        
        /// <summary>Gets the global instance of the SecurityProtocols singleton</summary>
        /// <returns>The global instance of the Security Protocols singleton</returns>
        public static SecurityProtocols GetInstance()
        {
            if (SecurityProtocols.instance == null)
            {
                SecurityProtocols.instance = new SecurityProtocols();
            }

            return SecurityProtocols.instance;
        }
        
        /// <summary>
        /// Set the <c>SecurityProtocols</c>
        /// </summary>
        /// <param name="securityProtocols">A <c>SecurityProtocols</c> object</param>
        public static void SetSecurityProtocols(SecurityProtocols securityProtocols)
        {
            SecurityProtocols.instance = securityProtocols;
        }
        
        /// <summary>
        /// Add the default SecurityProtocols.
        /// The names of the SecurityProtocols to add are read from a
        /// properties file.
        /// </summary>
        /// <returns>This SecurityProtocols instance, to aid in chaining</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public SecurityProtocols AddDefaultProtocols()
        {
            //if (JunoSnmpSettings.ExtensibilityEnabled)
            //{
            //    string secProtocols =
            //        System.getProperty(SECURITY_PROTOCOLS_PROPERTIES,
            //                           SECURITY_PROTOCOLS_PROPERTIES_DEFAULT);
            //    Stream ins =
            //        SecurityProtocols.getclass.getResourceAsStream(secProtocols);
            //    if (ins == null)
            //    {
            //        throw new InternalError("Could not read '" + secProtocols +
            //                                "' from classpath!");
            //    }
            //    Properties props = new Properties();
            //    try
            //    {
            //        props.load(ins);
            //        for (Enumeration en = props.propertyNames(); en.hasMoreElements();)
            //        {
            //            string className = en.nextElement().toString();
            //            string customOidString = props.getProperty(className);
            //            OID customOID = null;
            //            if (customOidString != null)
            //            {
            //                customOID = new OID(customOidString);
            //            }
            //            try
            //            {
            //                Class c = Class.forName(className);
            //                object proto = c.newInstance();
            //                if ((proto is NonStandardSecurityProtocol) && (customOID != null))
            //                {
            //                    if (logger.isInfoEnabled())
            //                    {
            //                        logger.info("Assigning custom ID '" + customOID + "' to security protocol " + className);
            //                    }
            //                  ((NonStandardSecurityProtocol)proto).setID(customOID);
            //                }
            //                if (proto is IAuthenticationProtocol)
            //                {
            //                    AddAuthenticationProtocol((IAuthenticationProtocol)proto);
            //                }
            //                else if (proto is IPrivacyProtocol)
            //                {
            //                    AddPrivacyProtocol((IPrivacyProtocol)proto);
            //                }
            //                else
            //                {
            //                    logger.error(
            //                        "Failed to register security protocol because it does " +
            //                        "not implement required interfaces: " + className);
            //                }
            //            }
            //            catch (Exception cnfe)
            //            {
            //                logger.error(cnfe);
            //                throw new InternalError(cnfe.Message);
            //            }
            //        }
            //    }
            //    catch (IOException iox)
            //    {
            //        string txt = "Could not read '" + secProtocols + "': " +
            //            iox.Message;
            //        logger.error(txt);
            //        throw new InternalError(txt);
            //    }
            //    finally
            //    {
            //        try
            //        {
            //            ins.Close();
            //        }
            //        catch (IOException ex)
            //        {
            //            // ignore
            //            logger.warn(ex);
            //        }
            //    }
            //}
            //else
            //{
                AddAuthenticationProtocol(new AuthMD5());
                AddAuthenticationProtocol(new AuthSHA());
                AddAuthenticationProtocol(new AuthHMAC192SHA256());
                AddAuthenticationProtocol(new AuthHMAC384SHA512());
                AddPrivacyProtocol(new PrivDES());
                AddPrivacyProtocol(new PrivAES128());
                AddPrivacyProtocol(new PrivAES192());
                AddPrivacyProtocol(new PrivAES256());
            //}

            return this;
        }

        /**
         * Add the given {@link AuthenticationProtocol}. If an authentication protocol
         * with the supplied ID already exists, the supplied authentication protocol
         * will not be added and the security protocols will not be unchang.
         *
         * @param auth
         *    the AuthenticationProtocol to add (an existing authentication protcol
         *    with <code>auth</code>'s ID remains unchanged).
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddAuthenticationProtocol(IAuthenticationProtocol auth)
        {
            if (authProtocols[auth.ID] == null)
            {
                authProtocols.Add(auth.ID, auth);
                if (auth.HashLength > maxAuthDigestLength)
                {
                    maxAuthDigestLength = auth.HashLength;
                }
            }
        }

        /**
         * Get the {@link AuthenticationProtocol} with the given ID.
         *
         * @param id
         *    The unique ID (specified as {@link OID}) of the AuthenticationProtocol.
         * @return
         *    the AuthenticationProtocol object if it was added before,
         *    or null if not.
         */
        public IAuthenticationProtocol GetAuthenticationProtocol(OID id)
        {
            if (id == null)
            {
                return null;
            }

            return authProtocols[id];
        }

        /**
         * Remove the given {@link AuthenticationProtocol}.
         *
         * @param auth The protocol to remove
         */
        public void RemoveAuthenticationProtocol(IAuthenticationProtocol auth)
        {
            authProtocols.Remove(auth.ID);
        }

        /**
         * Add the given {@link PrivacyProtocol}. If a privacy protocol
         * with the supplied ID already exists, the supplied privacy protocol
         * will not be added and the security protocols will not be changed.
         *
         * @param priv
         *    the PrivacyProtocol to add (an existing privacy protcol
         *    with <code>priv</code>'s ID remains unchanged).
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddPrivacyProtocol(IPrivacyProtocol priv)
        {
            if (privProtocols[priv.ID] == null)
            {
                privProtocols.Add(priv.ID, priv);
                if (priv.DecryptParamsLength > maxPrivDecryptParamsLength)
                {
                    maxPrivDecryptParamsLength = priv.DecryptParamsLength;
                }
            }
        }
        
        /// <summary>
        /// Get the <see cref="IPrivacyProtocol "/> with the given ID.
        /// </summary>
        /// <param name="id">The OID of the privacy protocol</param>
        /// <returns>
        /// The <see cref="IPrivacyProtocol"/> with the given ID if it exists,
        /// null otherwise.
        /// </returns>
        public IPrivacyProtocol GetPrivacyProtocol(OID id)
        {
            if (id == null)
            {
                return null;
            }

            return privProtocols[id];
        }

        /// <summary>Remove the given privacy protocol</summary>
        /// <param name="priv">The protocol to be removed</param>
        public void RemovePrivacyProtocol(IPrivacyProtocol priv)
        {
            privProtocols.Remove(priv.ID);
        }
        
        /// <summary>
        /// Generates the localized key for the given password and engine id for the
        /// authentication protocol specified by the supplied OID.
        /// </summary>
        /// <param name="authProtocolID">
        /// An OID identifying the authentication protocol to use.
        /// </param>
        /// <param name="engineID">The engine ID of the authoritative engine</param>
        /// <returns>The localized authentication key.</returns>
        public byte[] PasswordToKey(OID authProtocolID,
                                    OctetString passwordString,
                                    byte[] engineID)
        {

            IAuthenticationProtocol protocol = authProtocols[authProtocolID];
            if (protocol == null)
            {
                return null;
            }

            return protocol.PasswordToKey(passwordString, engineID);
        }
        
        /// <summary>
        /// Generates the localized key for the given password and engine id for the
        /// privacy protocol specified by the supplied OID.
        /// </summary>
        /// <param name="privProtocolID">
        /// An OID identifying the privacy protocol the key should be created for.
        /// </param>
        /// <param name="authProtocolID">
        /// An OID identifying the authentication protocol to use
        /// </param>
        /// <param name="passwordString">The authentication pass phrase</param>
        /// <param name="engineID">The engine ID of the authoritative engine.</param>
        public byte[] PasswordToKey(OID privProtocolID,
                                    OID authProtocolID,
                                    OctetString passwordString,
                                    byte[] engineID)
        {

            IAuthenticationProtocol authProtocol = authProtocols[authProtocolID];
            if (authProtocol == null)
            {
                return null;
            }

            IPrivacyProtocol privProtocol = privProtocols[privProtocolID];
            if (privProtocol == null)
            {
                return null;
            }

            byte[] key = authProtocol.PasswordToKey(passwordString, engineID);

            if (key == null)
            {
                return null;
            }

            if (key.Length >= privProtocol.MinKeyLength)
            {
                if (key.Length > privProtocol.MaxKeyLength)
                {
                    // truncate key
                    byte[] truncatedKey = new byte[privProtocol.MaxKeyLength];
                    System.Array.Copy(key, 0, truncatedKey, 0, privProtocol.MaxKeyLength);
                    return truncatedKey;
                }

                return key;
            }

            // extend key if necessary
            byte[] extKey = privProtocol.ExtendShortKey(key, passwordString, engineID,
                                                        authProtocol);
            return extKey;
        }
        
        /// <summary>
        /// Gets the maximum authentication key length of the all known
        /// authentication protocols in this <see cref="SecurityProtocols"/> object
        /// </summary>
        public int MaxAuthDigestLength
        {
            get
            {
                return this.maxAuthDigestLength;
            }
        }
        
        /// <summary>
        /// Gets the maximum privacy key length of the all known
        /// privacy protocols in this <see cref="SecurityProtocols"/> object
        /// </summary>
        public int MaxPrivDecryptParamsLength
        {
            get
            {
                return this.maxPrivDecryptParamsLength;
            }
        }
        
        /// <summary>
        /// Limits the supplied key value to the specified maximum length
        /// </summary>
        /// <param name="key">The key to truncate</param>
        /// <param name="maxKeyLength">The maximum length of the key</param>
        /// <returns>
        /// The truncated key with a length equal to the smaller of <c>key.Length</c>
        /// and <c>MaxKeyLength</c>
        /// </returns>
        public byte[] TruncateKey(byte[] key, int maxKeyLength)
        {
            byte[] truncatedNewKey = new byte[Math.Min(maxKeyLength, key.Length)];
            System.Array.Copy(key, 0, truncatedNewKey, 0, truncatedNewKey.Length);
            return truncatedNewKey;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
