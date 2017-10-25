// <copyright file="TreeEventArgs.cs" company="None">
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
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Event
{
    using System;
    using JunoSnmp.SMI;
    /**
 * The <code>TreeEvent</code> class reports events in a tree retrieval
 * operation.
 *
 * @author Frank Fock
 * @version 1.8
 * @since 1.8
 * @see TreeUtils
 */
    public class TreeEventArgs : RetrievalEventArgs
    {

        public TreeEventArgs(object userObject, VariableBinding[] vbs) : base(userObject, vbs)
        {

        }

        public TreeEventArgs(object userObject, int status) : base(userObject, status)
        {

        }

        public TreeEventArgs(object userObject, PDU report) : base(userObject, report)
        {

        }

        public TreeEventArgs(object userObject, Exception exception) : base(userObject, exception)
        {

        }

        /**
         * Gets the variable bindings retrieved in depth first order from the
         * (sub-)tree.
         *
         * @return VariableBinding[]
         *    a possibly empty or <code>null</code> array of
         *    <code>VariableBindings</code>.
         */
        public VariableBinding[] VariableBindings
        {
            get => this.vbs;
        }
    }
}
