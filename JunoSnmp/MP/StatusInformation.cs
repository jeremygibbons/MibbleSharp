// <copyright file="StatusInformation.cs" company="None">
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

namespace JunoSnmp.MP
{
    using System;
    using System.Runtime.Serialization;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>StatusInformation</c> class represents status information
    /// of a SNMPv3 message that is needed to return a report message.
    /// </summary>
    public class StatusInformation : ISerializable
    {
        private VariableBinding errorIndication;
        private byte[] contextName;
        private byte[] contextEngineID;
        private Integer32 securityLevel;

        public StatusInformation()
        {
        }

        public StatusInformation(
            VariableBinding errorIndication,
            byte[] contextName,
            byte[] contextEngineID,
            Integer32 securityLevel)
        {
            this.errorIndication = errorIndication;
            this.contextName = contextName;
            this.contextEngineID = contextEngineID;
            this.securityLevel = securityLevel;
        }

        public VariableBinding ErrorIndication
        {
            get
            {
                return this.errorIndication;
            }

            set
            {
                this.errorIndication = value;
            }
        }

        public byte[] ContextName
        {
            get
            {
                return this.contextName;
            }

            set
            {
                this.contextName = value;
            }
        }

        public byte[] ContextEngineID
        {
            get
            {
                return this.contextEngineID;
            }

            set
            {
                this.contextEngineID = value;
            }
        }

        public Integer32 SecurityLevel
        {
            get
            {
                return this.securityLevel;
            }

            set
            {
                this.securityLevel = value;
            }
        }

        public override string ToString()
        {
            if (this.errorIndication == null)
            {
                return "noError";
            }

            return this.errorIndication.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
