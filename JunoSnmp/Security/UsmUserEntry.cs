// <copyright file="UsmUserEntry.cs" company="None">
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
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /**
 * The <code>UsmUserEntry</code> class represents a user in the
 * Local Configuration Datastore (LCD).
 *
 * @author Frank Fock
 * @version 1.1
 */
    public class UsmUserEntry : ISerializable, IComparable
    {
        private OctetString engineID;
        private OctetString userName;
        private UsmUser usmUser;
        private byte[] authenticationKey;
        private byte[] privacyKey;

        /**
         * Creates a new user entry with empty engine ID and empty user.
         */
        public UsmUserEntry()
        {
            engineID = new OctetString();
            userName = new OctetString();
            usmUser = new UsmUser(new OctetString(), null, null, null, null);
        }

        /**
         * Creates a user with user name and associated {@link UsmUser}.
         * @param userName
         *    the user name of the new entry.
         * @param user
         *    the <code>UsmUser</code> representing the security information of the
         *    user.
         */
        public UsmUserEntry(OctetString userName, UsmUser user)
        {
            this.userName = userName;
            this.usmUser = user;
            if (user.IsLocalized)
            {
                this.engineID = user.LocalizationEngineID;
                if ((user.AuthenticationProtocol != null) &&
                    (user.AuthenticationPassphrase != null))
                {
                    authenticationKey = user.AuthenticationPassphrase.GetValue();
                    if ((user.PrivacyProtocol != null) &&
                        (user.PrivacyPassphrase != null))
                    {
                        privacyKey = user.PrivacyPassphrase.GetValue();
                    }
                }
            }
        }

        /**
         * Creates a user with user name and associated {@link UsmUser}.
         * @param userName
         *    the user name of the new entry.
         * @param engineID
         *    the authoritative engine ID associated with the user.
         * @param user
         *    the <code>UsmUser</code> representing the security information of the
         *    user.
         */
        public UsmUserEntry(OctetString userName, OctetString engineID, UsmUser user) 
            : this(userName, user)
        {
            this.engineID = engineID;
        }

        /**
         * Creates a localized user entry.
         * @param engineID
         *    the engine ID for which the users has bee localized.
         * @param securityName
         *    the user and security name of the new entry.
         * @param authProtocol
         *    the authentication protocol ID.
         * @param authKey
         *    the authentication key.
         * @param privProtocol
         *    the privacy protocol ID.
         * @param privKey
         *    the privacy key.
         */
        public UsmUserEntry(byte[] engineID, OctetString securityName,
                            OID authProtocol, byte[] authKey,
                            OID privProtocol, byte[] privKey)
        {
            this.engineID = (engineID == null) ? null : new OctetString(engineID);
            this.userName = securityName;
            this.authenticationKey = authKey;
            this.privacyKey = privKey;
            this.usmUser = new UsmUser(
                userName, 
                authProtocol,
                ((authenticationKey != null) ? new OctetString(authenticationKey) : null),
                privProtocol,
                ((privacyKey != null) ? new OctetString(privacyKey) : null),
                this.engineID);
        }


        public OctetString EngineID
        {
            get
            {
                return this.engineID;
            }

            set
            {
                this.engineID = value;
            }
        }

        public OctetString UserName
        {
            get
            {
                return userName;
            }

            set
            {
                this.userName = value;
            }
        }

        public UsmUser UsmUser
        {
            get
            {
                return usmUser;
            }

            set
            {
                this.usmUser = value;
            }
        }

        public byte[] AuthenticationKey
        {
            get
            {
                return authenticationKey;
            }

            set
            {
                this.authenticationKey = value;
            }
        }

        public byte[] PrivacyKey
        {
            get
            {
                return privacyKey;
            }

            set
            {
                this.privacyKey = value;
            }
        }

        /**
         * Compares this user entry with another one by engine ID then by their user
         * names.
         *
         * @param o
         *    a <code>UsmUserEntry</code> instance.
         * @return
         *    a negative integer, zero, or a positive integer as this object is
         *    less than, equal to, or greater than the specified object.
         */
        public int CompareTo(Object o)
        {
            UsmUserEntry other = (UsmUserEntry)o;
            int result = 0;
            if ((engineID != null) && (other.engineID != null))
            {
                result = engineID.CompareTo(other.engineID);
            }
            else if ((engineID != null) && (other.engineID == null))
            {
                result = 1;
            }
            else if ((engineID == null) && (other.engineID != null))
            {
                result = -1;
            }
            if (result == 0)
            {
                result = userName.CompareTo(other.userName);
                if (result == 0)
                {
                    result = usmUser.CompareTo(other.usmUser);
                }
            }
            return result;
        }

        public override string ToString()
        {
            return "UsmUserEntry[userName=" + userName + ",usmUser=" + usmUser + "]";
        }
    }
}
