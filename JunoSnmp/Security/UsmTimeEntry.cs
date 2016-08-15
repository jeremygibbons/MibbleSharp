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
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;
    /**
 * The <code>UsmTimeEntry</code> class represents time synchronization
 * information associated with an engine ID.
 *
 * @author Frank Fock
 * @version 1.0
 */
    public class UsmTimeEntry : ISerializable
    {

        private OctetString engineID;
        private int engineBoots;
        private int timeDiff;
        private int latestReceivedTime;

        /**
         * Creates a time entry with engine ID, engine boots and time.
         *
         * @param engineID
         *    the engine ID for which time synchronization information is created.
         * @param engineBoots
         *    the number of engine boots of the engine.
         * @param engineTime
         *    the time in seconds elapsed since the last reboot of the engine.
         */
        public UsmTimeEntry(OctetString engineID, int engineBoots, int engineTime)
        {
            this.engineID = engineID;
            this.engineBoots = engineBoots;
            this.EngineTime = engineTime;
        }

        public OctetString EngineID
        {
            get
            {
                return engineID;
            }
        }


        public int EngineBoots
        {
            get
            {
                return this.engineBoots;
            }

            set
            {
                this.engineBoots = value;
            }
        }

        public int TimeDiff
        {
            get
            {
                return this.timeDiff;
            }

            set
            {
                this.timeDiff = value;
            }
        }

        /**
         * Gets the time when a message has been received last from the associated
         * SNMP engine.
         * @return
         *    the engine time in seconds.
         */

        /**
         * Sets the time when a message has been received last from the associated
         * SNMP engine.
         * @param latestReceivedTime
         *    the engine time in seconds.
         */
        public int LatestReceivedTime
        {
            get
            {
                return this.latestReceivedTime;
            }

            set
            {
                this.latestReceivedTime = value;
            }
        }

        /**
         * Sets the engine time which also sets the last received engine time
         * to the supplied value.
         * @param engineTime
         *    the time in seconds elapsed since the last reboot of the engine.
         */
        public int EngineTime
        {
            set
            {
                this.latestReceivedTime = value;
                this.timeDiff = value - (int)(DateTime.Now.Ticks / UsmTimeTable.TIME_PRECISION);
            }   
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
