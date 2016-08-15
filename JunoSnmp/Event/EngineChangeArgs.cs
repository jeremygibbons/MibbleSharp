// <copyright file="EngineChangeArg.cs" company="None">
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

    /// <summary>
    /// A class describing the arguments passed when an engine is added, removed or ignored in
    /// a Message Processing model.
    /// </summary>
    public class EngineChangeArgs : EventArgs
    {
        private OctetString engineID;
        private IAddress engineAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineChangeArgs"/> class.
        /// </summary>
        /// <param name="engineID">The Engine ID of the engine</param>
        /// <param name="engineAddress">The Engine Address of the engine</param>
        public EngineChangeArgs(OctetString engineID, IAddress engineAddress)
        {
            this.engineAddress = engineAddress;
            this.engineID = engineID;
        }

        /// <summary>
        /// Gets the Engine ID for this event
        /// </summary>
        public OctetString EngineID
        {
            get
            {
                return this.EngineID;
            }
        }

        /// <summary>
        /// Gets the Engine Address for this event.
        /// </summary>
        public IAddress EngineAddress
        {
            get
            {
                return this.engineAddress;
            }
        }
        
    }
}
