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
    using JunoSnmp.SMI;

    public class CounterIncrEventArgs : EventArgs
    {
        private OID oid;
        private IVariable currentValue = new Counter32();
        private long increment = 1;
        private Object index;

        public CounterIncrEventArgs(OID oid)
        {
            this.oid = oid;
        }

        public CounterIncrEventArgs(OID oid, long increment) : this(oid)
        {
            this.increment = increment;
        }

        public CounterIncrEventArgs(OID oid, long increment, object index) : this(oid, increment)
        {
            this.index = index;
        }

        public OID Oid
        {
            get
            {
                return this.oid;
            }
        }

        public long Increment
        {
            get
            {
                return this.increment;
            }

            set
            {
                this.increment = value;
            }
        }

        public object Index
        {
            get
            {
                return this.index;
            }
        }

        public IVariable CurrentValue
        {
            get
            {
                return this.currentValue;
            }

            set
            {
                this.currentValue = value;
            }
        }
    }
}
