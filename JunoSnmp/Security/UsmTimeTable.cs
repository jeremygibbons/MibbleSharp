// <copyright file="UsmTimeEntry.cs" company="None">
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
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>UsmTimeTable</c> class is a singleton that stores USM user
    /// information as part of the Local Configuration Datastore(LCD).
    /// </summary>
    public class UsmTimeTable : ISerializable
    {

        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly long TIME_PRECISION = 1000000000L;

        private IDictionary<IVariable, UsmTimeEntry> table = new Dictionary<IVariable, UsmTimeEntry>(10);
        private long lastLocalTimeChange = System.nanoTime();
        private UsmTimeEntry localTime;

        public UsmTimeTable(OctetString localEngineID, int engineBoots)
        {
            setLocalTime(new UsmTimeEntry(localEngineID, engineBoots, 0));
        }

        public void AddEntry(UsmTimeEntry entry)
        {
            table.Add(entry.EngineID, entry);
        }

        public UsmTimeEntry GetEntry(OctetString engineID)
        {
            return table[engineID];
        }

        public UsmTimeEntry getLocalTime()
        {
            UsmTimeEntry entry = new UsmTimeEntry(localTime.EngineID,
                                                  localTime.EngineBoots,
                                                  getEngineTime());
            entry.TimeDiff = entry.TimeDiff * (-1) + localTime.TimeDiff;
            return entry;
        }

        public void setLocalTime(UsmTimeEntry localTime)
        {
            this.localTime = localTime;
            lastLocalTimeChange = System.nanoTime();
        }

        /**
         * Sets the number of engine boots.
         * @param engineBoots
         *    the number of engine boots.
         * @since 1.2
         */


        /**
         * Returns the number of seconds since the value of
         * the engineBoots object last changed. When incrementing this object's value
         * would cause it to exceed its maximum, engineBoots is incremented as if a
         * re-initialization had occurred, and this
         * object's value consequently reverts to zero.
         *
         * @return
         *    a positive integer value denoting the number of seconds since
         *    the engineBoots value has been changed.
         * @since 1.2
         */
        public int EngineTime
        {
            get
            {
                return (int)((((System.nanoTime() - lastLocalTimeChange) / TIME_PRECISION) +
                  localTime.LatestReceivedTime) %
                         2147483648L);
            }
        }

        /**
         * The number of times that the SNMP engine has (re-)initialized itself
         * since snmpEngineID was last configured.
         * @return
         *    the number of SNMP engine reboots.
         */
        public int EngineBoots
        {
            get
            {
                return localTime.EngineBoots;
            }

            set
            {
                this.localTime.EngineBoots = value;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public UsmTimeEntry getTime(OctetString engineID)
        {
            if (localTime.EngineID.Equals(engineID))
            {
                return getLocalTime();
            }

            UsmTimeEntry found = table[engineID];

            if (found == null)
            {
                return null;
            }

            return new UsmTimeEntry(engineID, found.EngineBoots,
                                    found.TimeDiff +
                                    (int)(System.nanoTime() / TIME_PRECISION));
        }

        /**
         * Removes the specified engine ID from the time cache.
         * @param engineID
         *    the engine ID of the remote SNMP engine to remove from this  time cache.
         */
        public void RemoveEntry(OctetString engineID)
        {
            table.Remove(engineID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int CheckEngineID(
            OctetString engineID,
            bool discoveryAllowed,
            int engineBoots,
            int engineTime)
        {
            if (table[engineID] != null)
            {
                return SnmpConstants.SNMPv3_USM_OK;
            }
            else if (discoveryAllowed)
            {
                AddEntry(new UsmTimeEntry(engineID, engineBoots, engineTime));
                return SnmpConstants.SNMPv3_USM_OK;
            }
            return SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int CheckTime(UsmTimeEntry entry)
        {
            int now = (int)(System.nanoTime() / TIME_PRECISION);
            if (localTime.EngineID.Equals(entry.EngineID))
            {
                /* Entry found, we are authoritative */
                if ((localTime.EngineBoots == 2147483647)
                    || (localTime.EngineBoots != entry.EngineBoots)
                    || (Math.Abs(now + localTime.TimeDiff - entry.LatestReceivedTime) > 150))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(
                            "CheckTime: received message outside time window (authoritative):" +
                            ((localTime.EngineBoots !=
                              entry.EngineBoots) ? "engineBoots differ " + localTime.EngineBoots + "!=" + entry.EngineBoots :
                             "" + (Math.Abs(now + localTime.TimeDiff -
                                          entry.LatestReceivedTime)) + " > 150"));
                    }

                    return SnmpConstants.SNMPv3_USM_NOT_IN_TIME_WINDOW;
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("CheckTime: time ok (authoritative)");
                    }
                    return SnmpConstants.SNMPv3_USM_OK;
                }
            }
            else
            {
                UsmTimeEntry time = table[entry.EngineID];
                if (time == null)
                {
                    return SnmpConstants.SNMPv3_USM_UNKNOWN_ENGINEID;
                }
                // RFC 3414 section 3.2.7 b) 1):
                if ((entry.EngineBoots > time.EngineBoots) ||
                    ((entry.EngineBoots == time.EngineBoots) &&
                     (entry.LatestReceivedTime > time.LatestReceivedTime)))
                {
                    /* time ok, update values */
                    time.EngineBoots = entry.EngineBoots;
                    time.LatestReceivedTime = entry.LatestReceivedTime;
                    time.TimeDiff = entry.LatestReceivedTime - now;
                }
                // RFC 3414 section 3.2.7 b) 2):
                if ((entry.EngineBoots < time.EngineBoots)
                    || ((entry.EngineBoots == time.EngineBoots) &&
                     (time.LatestReceivedTime > entry.LatestReceivedTime + 150))
                     || (time.EngineBoots == 2147483647))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug(
                            "CheckTime: received message outside time window (non authoritative)");
                    }

                    return SnmpConstants.SNMPv3_USM_NOT_IN_TIME_WINDOW;
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("CheckTime: time ok (non authoritative)");
                    }

                    return SnmpConstants.SNMPv3_USM_OK;
                }
            }
        }

        public void Reset()
        {
        }
    }
}
