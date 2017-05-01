﻿// <copyright file="SimpleMessageID.cs" company="None">
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

    /// <summary>
    /// The <c>SimpleMessageID</c> implements the simplest possible {@link MessageID} with
    /// a minimum memory footprint.
    /// </summary>
    public class SimpleMessageID : MessageID, ISerializable, IEquatable<SimpleMessageID>
    {

        private readonly int messageID;

        public SimpleMessageID(int messageID)
        {
            this.messageID = messageID;
        }

        public int MessageID
        {
            get
            {
                return this.messageID;
            }
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || this.GetType() != o.GetType()) return false;

            SimpleMessageID that = (SimpleMessageID)o;

            if (messageID != that.messageID) return false;

            return true;
        }


        public override int GetHashCode()
        {
            return messageID;
        }

        public override string ToString()
        {
            return messageID.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public bool Equals(SimpleMessageID other)
        {
            return this.messageID == other.messageID;
        }
    }
}
