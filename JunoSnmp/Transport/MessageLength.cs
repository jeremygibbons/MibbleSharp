﻿// <copyright file="MessageLength.cs" company="None">
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

namespace JunoSnmp.Transport
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The <code>MessageLength</code> object contains information about the
    /// length of a message and the length of its header.
    /// </summary>
    public class MessageLength : ISerializable
    {
        private int payloadLength;
        private int headerLength;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageLength"/> class.
        /// </summary>
        /// <param name="headerLength">The length of the message header, in bytes</param>
        /// <param name="payloadLength">The length of the payload in bytes</param>
        public MessageLength(int headerLength, int payloadLength)
        {
            this.payloadLength = payloadLength;
            this.headerLength = headerLength;
        }

        /// <summary>
        /// Gets the length of the message payload, in bytes
        /// </summary>
        public int PayloadLength
        {
            get
            {
                return payloadLength;
            }
        }

        /// <summary>
        /// Gets the length of the message header, in bytes
        /// </summary>
        public int HeaderLength
        {
            get
            {
                return headerLength;
            }
        }

        /// <summary>
        /// Gets the total length of the message, in bytes
        /// </summary>
        public int Length
        {
            get
            {
                return headerLength + payloadLength;
            }
        }

        /// <summary>
        /// Returns a text representation of this object
        /// </summary>
        /// <returns>A text representation of this object</returns>
        public override string ToString()
        {
            return nameof(MessageLength) +
        "[headerLength=" + headerLength + ",payloadLength=" + payloadLength + "]";
        }

        /// <summary>
        /// Writes this object to a stream for serialization purposes
        /// </summary>
        /// <param name="info">The serialization info object</param>
        /// <param name="context">The streaming context object</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}