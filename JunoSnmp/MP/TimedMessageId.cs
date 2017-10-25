// <copyright file="TimedMessageID.cs" company="None">
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

    /// <summary>
    /// The <code>TimedMessageID</code> adds system time information to the message ID that allows
    /// to measure response times and detect lost messages with SNMPv3.
    /// </summary>
    public class TimedMessageID : SimpleMessageID
    {
        private readonly long creationNanoTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedMessageID"/> class.
        /// </summary>
        /// <param name="messageID">The message ID</param>
        public TimedMessageID(int messageID) : base(messageID)
        {

            this.creationNanoTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the <see cref="DateTime.Now.Ticks"/> value when this message ID object has been created.
        /// </summary>
        public long CreationNanoTime
        {
            get
            {
                return this.creationNanoTime;
            }
        }

        /// <summary>
        /// Gets a string representing this object
        /// </summary>
        /// <returns>A string representing this object</returns>
        public override string ToString()
        {
            return "TimedMessageID{" +
                "msgID=" + this.MessageId +
                ",creationNanoTime=" + this.creationNanoTime +
                "}";
        }
    }
}
