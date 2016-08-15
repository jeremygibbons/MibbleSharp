// <copyright file="BERInputStream.cs" company="None">
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

namespace JunoSnmp.ASN1
{
    using System;
    using System.IO;

    /// <summary>
    /// The class wraps is a memory-backed byte-oriented read-only stream
    /// </summary>
    public class BERInputStream : MemoryStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BERInputStream"/> class. 
        /// </summary>
        /// <param name="buf">The array of bytes to use as a source for the stream</param>
        public BERInputStream(byte[] buf) : base(buf)
        {
        }
        
        /// <summary>
        /// Gets the number of bytes that can be read (or skipped over) from this
        /// input stream without blocking by the next caller of a method for this input
        /// stream.
        /// </summary>
        public long Available
        {
            get
            {
                return this.Length - this.Position;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a string with the current position in the stream.
        /// </summary>
        /// <remarks>Used for generating exception messages</remarks>
        public string PositionMessage
        {
            get
            {
                return " at position " + this.Position;
            }
        }

        /// <summary>
        /// Skips over and discards <code>n</code> bytes of data from this input stream.
        /// </summary>
        /// <param name="n">The number of bytes to be skipped</param>
        /// <returns>The actual number of bytes skipped</returns>
        /// <exception cref="IOException">If an I/O error occurs</exception>
        public long Skip(long n)
        {
            long skipped = Math.Min(this.Available, n);
            this.Position += skipped;
            return skipped;
        }
    }
}
