// <copyright file="CounterIncrArgs.cs" company="None">
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

namespace JunoSnmp.Event
{
    using System;
    using JunoSnmp.Security;

    public class UsmUserChangeArgs : EventArgs
    {
        /// <summary>
        /// Constant: a new user was created.
        /// </summary>
        public static readonly int USER_ADDED = 1;

        /// <summary>
        /// Constant: a user was deleted.
        /// </summary>
        public static readonly int USER_REMOVED = 2;

        /// <summary>
        /// Constant: a user was changed (but not deleted).
        /// </summary>
        public static readonly int USER_CHANGED = 3;

        private UsmUserEntry user;
        private int type;

        /**
 * Construct a UsmUserEvent.
 *
 * @param source
 *    the object that emitts this event
 * @param changedEntry
 *    the changed entry
 * @param type
 *    can be USER_ADDED, USER_REMOVED or USER_CHANGED.
 */
        public UsmUserChangeArgs(UsmUserEntry changedEntry, int type) : base()
        {
            this.user = changedEntry;
            this.type = type;
        }

        public UsmUserEntry User
        {
            get
            {
                return this.user;
            }

            set
            {
                this.user = value;
            }
        }

        public int Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }
    }
}
