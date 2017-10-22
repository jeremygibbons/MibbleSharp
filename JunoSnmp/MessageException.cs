// <copyright file="MessageException.cs" company="None">
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

namespace JunoSnmp
{
    using System;
    using System.IO;
    using JunoSnmp.MP;

    /// <summary>
    /// The <c>MessageException</c> represents information about an exception
    /// occurred during message processing.The associated
    /// <c>StatusInformation</c> object provides(if present) detailed
    /// information about the error that occurred and the status of the processed
    /// message.
    /// </summary>
    public class MessageException : IOException
    {
        private readonly int junoSnmpErrorStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class.
        /// </summary>
        public MessageException()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class from a
        /// <see cref="StatusInformation"/> object.
        /// </summary>
        /// <param name="status">A <see cref="StatusInformation"/> instance</param>
        public MessageException(StatusInformation status) : base(string.Empty + status.ErrorIndication)
        {
            StatusInformation = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class with a given message
        /// </summary>
        /// <param name="message">The error message, as a string</param>
        public MessageException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class with an error message
        /// and JunoSnmp specific error status (see <see cref="JunoSnmpErrorStatus"/> for details)
        /// </summary>
        /// <param name="message">An error message</param>
        /// <param name="junoSnmpErrorStatus">
        /// A <see cref="MessageProcessingModel"/> or <see cref="SecurityModel"/> specific error status as defined by
        /// <see cref="SnmpConstants"/>
        /// </param>
        public MessageException(string message, int junoSnmpErrorStatus) : base(message)
        {
            this.junoSnmpErrorStatus = junoSnmpErrorStatus;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageException"/> class with an error message
        /// and JunoSnmp specific error status (see <see cref="JunoSnmpErrorStatus"/> for details)
        /// </summary>
        /// <param name="message">An error message</param>
        /// <param name="junoSnmpErrorStatus">
        /// A <see cref="MessageProcessingModel"/> or <see cref="SecurityModel"/> specific error status as defined by
        /// <see cref="SnmpConstants"/>
        /// </param>
        /// <param name="innerException">The root cause represented by an inner Exception object</param>
        public MessageException(string message, int junoSnmpErrorStatus, Exception innerException) : base(message, innerException)
        {   
            this.junoSnmpErrorStatus = junoSnmpErrorStatus;
        }

        /// <summary>
        /// Gets or sets the status information for this exception
        /// </summary>
        public StatusInformation StatusInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the JunoSnmp specific error status associated with this exception. Returns a 
        /// <see cref="MessageProcessingModel"/> or <see cref="SecurityModel"/> specific error status 
        /// as defined by <see cref="SnmpConstants"/>
        /// </summary>
        public int JunoSnmpErrorStatus
        {
            get
            {
                return this.junoSnmpErrorStatus;
            }
        }
    }
}
