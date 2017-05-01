// <copyright file="ByteArrayWindow.cs" company="None">
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

namespace JunoSnmp.Security
{
    using System;

    /// <summary>
    /// The <code>ByteArrayWindow</code> provides windowed access to a sub-array
    /// of a byte array.
    /// </summary>
    public class ByteArrayWindow : IEquatable<ByteArrayWindow>
    {
        private byte[] value;
        private readonly int offset;
        private readonly int length;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayWindow"/> class, 
        /// that provides access to the bytes in the supplied array
        /// starting at the supplied offset.
        /// </summary>
        /// <param name="value">The underlying byte array</param>
        /// <param name="offset">The starting position of the created window</param>
        /// <param name="length">The length of the window</param>
        public ByteArrayWindow(byte[] value, int offset, int length)
        {
            this.value = value;
            this.offset = offset;
            this.length = length;
        }

        /// <summary>
        /// Gets or sets the backing byte array
        /// </summary>
        public byte[] Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }
        
        /// <summary>
        /// Gets the offset
        /// </summary>
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }
        
        /// <summary>
        /// Gets the length of the window
        /// </summary>
        public int Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Gets or sets the value at index i in the window
        /// </summary>
        /// <param name="i">The index to operate on</param>
        /// <returns>The element at index i in the window</returns>
        public byte this[int i]
        {
            get
            {
                if (i >= this.length)
                {
                    throw new ArgumentOutOfRangeException(string.Empty + i + " >= " + this.length);
                }

                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(string.Empty + i);
                }

                return this.value[i + this.offset];
            }

            set
            {
                if (i >= this.length)
                {
                    throw new ArgumentOutOfRangeException(string.Empty + i + " >= " + this.length);
                }

                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException(string.Empty + i);
                }

                this.value[i + this.offset] = value;
            }
        }

        /// <summary>
        /// Indicates whether some other object is "equal to" this one.
        /// </summary>
        /// <param name="obj">The reference object with which to compare.</param>
        /// <returns>
        /// <c>true</c> if this object is the same as the argument, <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ByteArrayWindow b)
            {
                return this.Equals(b);
            }

            return false;
        }

        public bool Equals(ByteArrayWindow other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Length != this.Length)
            {
                return false;
            }

            for (int i = 0; i < this.Length; i++)
            {
                if (other.Value[i] != this.Value[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hashcode for the current object
        /// </summary>
        /// <returns>A hashcode for the current object</returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// Compares this ByteArrayWindow with another, over at most
        /// <c>maxBytesToCompare</c> bytes
        /// </summary>
        /// <param name="other">The ByteArrayWindow to compare to</param>
        /// <param name="maxBytesToCompare">The maximum number of bytes to examine</param>
        /// <returns>
        /// True if the first <c>maxBytesToCompare</c> in the window are equal.
        /// False if some are different, or if <c>maxBytesToCompare</c> is greater
        /// than the length of either of the window buffers.
        /// </returns>
        public bool Equals(ByteArrayWindow other, int maxBytesToCompare)
        {
            if ((other.length < maxBytesToCompare) ||
                (this.length < maxBytesToCompare))
            {
                return false;
            }

            for (int i = 0; i < maxBytesToCompare; i++)
            {
                if (this.value[this.offset + i] != other.value[other.offset + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
