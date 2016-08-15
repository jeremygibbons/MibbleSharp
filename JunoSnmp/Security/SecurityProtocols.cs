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
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /**
 * The <code>SecurityProtocols</code> class holds all authentication and
 * privacy protocols for a SNMP entity.
 * <p>
 * To register security protocols other than the default, set the system
 * property {@link #SECURITY_PROTOCOLS_PROPERTIES} to a customized version
 * of the <code>SecurityProtocols.properties</code> file. The path has to
 * be specified relatively to this class.
 *
 * @author Jochen Katz & Frank Fock
 * @version 1.9
 */
    public class SecurityProtocols : ISerializable
    {

        private Dictionary<OID, IAuthenticationProtocol> authProtocols;
        private Dictionary<OID, IPrivacyProtocol> privProtocols;

        public static readonly string SECURITY_PROTOCOLS_PROPERTIES =
          "org.snmp4j.securityProtocols";
        private static readonly string SECURITY_PROTOCOLS_PROPERTIES_DEFAULT =
            "SecurityProtocols.properties";
        private static readonly LogAdapter logger = LogFactory.getLogger(SecurityProtocols.getclass);

  private static SecurityProtocols instance = null;
        private int maxAuthDigestLength = 0;
        private int maxPrivDecryptParamsLength = 0;

        protected SecurityProtocols()
        {
            authProtocols = new Dictionary<OID, IAuthenticationProtocol>(5);
            privProtocols = new Dictionary<OID, IPrivacyProtocol>(5);
        }

        /**
         * Get an instance of class SecurityProtocols.
         *
         * @return the globally used SecurityProtocols object.
         */
        public static SecurityProtocols GetInstance()
        {
            if (SecurityProtocols.instance == null)
            {
                SecurityProtocols.instance = new SecurityProtocols();
            }

            return SecurityProtocols.instance;
        }

        /**
         * Set the <code>SecurityProtocols</code>
         * @param securityProtocols SecurityProtocols
         */
        public static void setSecurityProtocols(SecurityProtocols securityProtocols)
        {
            SecurityProtocols.instance = securityProtocols;
        }

        /**
         * Add the default SecurityProtocols.
         *
         * The names of the SecurityProtocols to add are read from a
         * properties file.
         *
         * @return
         *    this SecurityProtocols instance for chaining configuration.
         *
         * @throws InternalError if {@link SNMP4JSettings#isExtensibilityEnabled()} is <code>true</code>
         * and corresponding properties file with the security protocols configuration cannot be opened/read.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public SecurityProtocols AddDefaultProtocols()
        {
            if (JunoSnmpSettings.ExtensibilityEnabled)
            {
                string secProtocols =
                    System.getProperty(SECURITY_PROTOCOLS_PROPERTIES,
                                       SECURITY_PROTOCOLS_PROPERTIES_DEFAULT);
                Stream ins =
                    SecurityProtocols.getclass.getResourceAsStream(secProtocols);
                if (ins == null)
                {
                    throw new InternalError("Could not read '" + secProtocols +
                                            "' from classpath!");
                }
                Properties props = new Properties();
                try
                {
                    props.load(ins);
                    for (Enumeration en = props.propertyNames(); en.hasMoreElements();)
                    {
                        string className = en.nextElement().toString();
                        string customOidString = props.getProperty(className);
                        OID customOID = null;
                        if (customOidString != null)
                        {
                            customOID = new OID(customOidString);
                        }
                        try
                        {
                            Class c = Class.forName(className);
                            object proto = c.newInstance();
                            if ((proto is NonStandardSecurityProtocol) && (customOID != null))
                            {
                                if (logger.isInfoEnabled())
                                {
                                    logger.info("Assigning custom ID '" + customOID + "' to security protocol " + className);
                                }
                              ((NonStandardSecurityProtocol)proto).setID(customOID);
                            }
                            if (proto is IAuthenticationProtocol)
                            {
                                AddAuthenticationProtocol((IAuthenticationProtocol)proto);
                            }
                            else if (proto is IPrivacyProtocol)
                            {
                                AddPrivacyProtocol((IPrivacyProtocol)proto);
                            }
                            else
                            {
                                logger.error(
                                    "Failed to register security protocol because it does " +
                                    "not implement required interfaces: " + className);
                            }
                        }
                        catch (Exception cnfe)
                        {
                            logger.error(cnfe);
                            throw new InternalError(cnfe.Message);
                        }
                    }
                }
                catch (IOException iox)
                {
                    string txt = "Could not read '" + secProtocols + "': " +
                        iox.Message;
                    logger.error(txt);
                    throw new InternalError(txt);
                }
                finally
                {
                    try
                    {
                        ins.Close();
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
                AddAuthenticationProtocol(new AuthMD5());
                AddAuthenticationProtocol(new AuthSHA());
                AddAuthenticationProtocol(new AuthHMAC192SHA256());
                AddAuthenticationProtocol(new AuthHMAC384SHA512());
                AddPrivacyProtocol(new PrivDES());
                AddPrivacyProtocol(new PrivAES128());
                AddPrivacyProtocol(new PrivAES192());
                AddPrivacyProtocol(new PrivAES256());
            }

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
                if (auth.DigestLength > maxAuthDigestLength)
                {
                    maxAuthDigestLength = auth.DigestLength;
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

        /**
         * Get the PrivacyProtocol with the given ID.
         *
         * @param id
         *    The unique ID (specified as {@link OID}) of the PrivacyProtocol.
         * @return
         *    the {@link PrivacyProtocol} object if it was added before,
         *    or null if not.
         */
        public IPrivacyProtocol GetPrivacyProtocol(OID id)
        {
            if (id == null)
            {
                return null;
            }

            return privProtocols[id];
        }

        /**
         * Remove the given {@link PrivacyProtocol}.
         *
         * @param priv The protocol to remove
         */
        public void RemovePrivacyProtocol(IPrivacyProtocol priv)
        {
            privProtocols.Remove(priv.ID);
        }


        /**
         * Generates the localized key for the given password and engine id for the
         * authentication protocol specified by the supplied OID.
         *
         * @param authProtocolID
         *    an <code>OID</code> identifying the authentication protocol to
         *    use.
         * @param passwordString
         *    the authentication pass phrase.
         * @param engineID
         *    the engine ID of the authoritative engine.
         * @return
         *    the localized authentication key.
         */
        public byte[] PasswordToKey(OID authProtocolID,
                                    OctetString passwordString,
                                    byte[] engineID)
        {

            IAuthenticationProtocol protocol =
                    authProtocols[authProtocolID];
            if (protocol == null)
            {
                return null;
            }
            return protocol.PasswordToKey(passwordString, engineID);
        }

        /**
         * Generates the localized key for the given password and engine id for the
         * privacy protocol specified by the supplied OID.
         *
         * @param privProtocolID
         *    an <code>OID</code> identifying the privacy protocol the key should
         *    be created for.
         * @param authProtocolID
         *    an <code>OID</code> identifying the authentication protocol to use.
         * @param passwordString
         *    the authentication pass phrase.
         * @param engineID
         *    the engine ID of the authoritative engine.
         * @return
         *    the localized privacy key.
         */
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

        /**
         * Gets the maximum authentication key length of the all known
         * authentication protocols.
         * @return
         *    the maximum authentication key length of all authentication protocols
         *    that have been added to this <code>SecurityProtocols</code>
         *    instance.
         */
        public int MaxAuthDigestLength
        {
            get
            {
                return this.maxAuthDigestLength;
            }
        }

        /**
         * Gets the maximum privacy key length of the currently known
         * privacy protocols.
         * @return
         *    the maximum privacy key length of all privacy protocols
         *    that have been added to this <code>SecurityProtocols</code>
         *    instance.
         */
        public int MaxPrivDecryptParamsLength
        {
            get
            {
                return this.maxPrivDecryptParamsLength;
            }
        }

        /**
         * Limits the supplied key value to the specified maximum length
         * @param key
         *    the key to truncate.
         * @param maxKeyLength
         *    the maximum length of the returned key.
         * @return
         *    the truncated key with a length of
         *    <code>min(key.length, maxKeyLength)</code>.
         * @since 1.9
         */
        public byte[] TruncateKey(byte[] key, int maxKeyLength)
        {
            byte[] truncatedNewKey = new byte[Math.Min(maxKeyLength, key.Length)];
            System.Array.Copy(key, 0, truncatedNewKey, 0, truncatedNewKey.Length);
            return truncatedNewKey;
        }
    }
}
