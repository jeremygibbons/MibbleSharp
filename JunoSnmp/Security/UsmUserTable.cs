// <copyright file="UsmUserTable.cs" company="None">
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
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>UsmUserTable</c> class stores USM user
    /// information as part of the Local Configuration Datastore(LCD).
    /// </summary>
    public class UsmUserTable : ISerializable
    {
        private static readonly log4net.ILog log = log4net.LogManager
          .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDictionary<UsmUserKey, UsmUserEntry> table = new Dictionary<UsmUserKey, UsmUserEntry>();

        public UsmUserTable()
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public UsmUserEntry addUser(UsmUserEntry user)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Adding user " + user.UserName + " = " + user.UsmUser);
            }
            UsmUserKey key = new UsmUserKey(user);
            UsmUserEntry entry;
            bool wasOldValue = table.TryGetValue(key, out entry);
            table.Add(key, user);
            return wasOldValue ? entry : null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void setUsers(IEnumerable<UsmUserEntry> c)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Setting users to " + c);
            }

            table.Clear();

            foreach (UsmUserEntry user in c)
            {
                table.Add(new UsmUserKey(user), user);
            }
        }

        /**
         * Gets all user entries with the supplied user name.
         * @param userName
         *    an <code>OctetString</code> denoting the user name.
         * @return
         *    a possibly empty <code>List</code> containing all user entries with
         *    the specified <code>userName</code>.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<UsmUserEntry> getUserEntries(OctetString userName)
        {
            IList<UsmUserEntry> users = table.Values.Where(u => u.UserName.Equals(userName)).ToList();

            if (log.IsDebugEnabled)
            {
                log.Debug("Returning user entries for " + userName + " = " + users);
            }

            return users;
        }

        public IList<UsmUserEntry> UserEntries
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return this.table.Values.ToList();
            }
        
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<UsmUserEntry> RemoveAllUsers(OctetString securityName, OctetString engineID)
        {
            if (engineID == null)
            {
                IList<UsmUserKey> deletedK = new List<UsmUserKey>();
                IList<UsmUserEntry> deletedV = new List<UsmUserEntry>();
                foreach (UsmUserKey key in table.Keys)
                {
                    UsmUserEntry userEntry = table[key];
                    if (securityName.Equals(userEntry.UsmUser.SecurityName))
                    {
                        deletedK.Add(key);
                    }
                }

                foreach(var key in deletedK)
                {
                    UsmUserEntry userEntry = table[key];
                    table.Remove(key);
                    deletedV.Add(userEntry);
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Removed user " + table[key]);
                    }
                }
                
                return deletedV;
            }

            UsmUserKey userKey = new UsmUserKey(engineID, securityName);
            UsmUserEntry entry = table[userKey];
            table.Remove(userKey);

            if (log.IsDebugEnabled)
            {
                log.Debug("Removed user with secName=" + securityName +
                    " and engineID=" + engineID);
            }

            return (entry != null) ? new List<UsmUserEntry>() { entry } : new List<UsmUserEntry>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public UsmUserEntry RemoveUser(OctetString engineID,
                                                OctetString securityName)
        {
            UsmUserKey key = new UsmUserKey(engineID, securityName);
            UsmUserEntry entry = table[key];
            table.Remove(key);

            if (log.IsDebugEnabled)
            {
                log.Debug("Removed user with secName=" + securityName +
                             " and engineID=" + engineID);
            }

            return entry;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public UsmUserEntry GetUser(OctetString engineID,
                                             OctetString securityName)
        {
            return table[new UsmUserKey(engineID, securityName)];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public UsmUserEntry GetUser(OctetString securityName)
        {
            return table[new UsmUserKey(new OctetString(), securityName)];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            table.Clear();

            if (log.IsDebugEnabled)
            {
                log.Debug("Cleared UsmUserTable");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public class UsmUserKey : IComparable
        {
            OctetString engineID;
            OctetString securityName;

            public UsmUserKey(UsmUserEntry entry)
            {
                this.EngineID = entry.EngineID;
                this.securityName = entry.UsmUser.SecurityName;
            }

            public UsmUserKey(OctetString engineID, OctetString securityName)
            {
                this.EngineID = engineID;
                this.securityName = securityName;
            }

            private OctetString EngineID
            {
                set
                {
                    if (engineID == null)
                    {
                        this.engineID = new OctetString();
                    }
                    else
                    {
                        this.engineID = value;
                    }
                }
            }

            public override int GetHashCode()
            {
                return engineID.GetHashCode() ^ 2 + securityName.GetHashCode();
            }

            public override bool Equals(Object o)
            {
                if ((o is UsmUserEntry) || (o is UsmUserKey))
                {
                    return (this.CompareTo(o) == 0);
                }

                return false;
            }

            public int CompareTo(Object o)
            {
                if (o is UsmUserEntry)
                {
                    return CompareTo(new UsmUserKey((UsmUserEntry)o));
                }

                UsmUserKey other = (UsmUserKey)o;
                int result = 0;

                if ((engineID != null) && (other.engineID != null))
                {
                    result = engineID.CompareTo(other.engineID);
                }
                else if (engineID != null)
                {
                    result = 1;
                }
                else if (other.engineID != null)
                {
                    result = -1;
                }

                if (result == 0)
                {
                    result = securityName.CompareTo(other.securityName);
                }

                return result;
            }
        }
    }
}
