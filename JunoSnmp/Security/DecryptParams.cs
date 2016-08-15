// <copyright file="DecryptParams.cs" company="None">
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
    /// <summary>
    /// Parameter class for encrypt and decrypt methods of <see cref="ISecurityProtocol"/>.
    /// </summary>
    public class DecryptParams
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptParams"/> class with the given value.
        /// </summary>
        /// <param name="array">The array as received on the wire</param>
        /// <param name="offset">The offset within the array</param>
        /// <param name="length">The length of the decryption parameters</param>
        public DecryptParams(byte[] array, int offset, int length)
        {
            this.Array = array;
            this.Offset = offset;
            this.Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecryptParams"/> class with the given value.
        /// </summary>
        public DecryptParams()
        {
            this.Array = null;
            this.Offset = 0;
            this.Length = 0;
        }

        /// <summary>
        /// Gets or sets the Array property
        /// </summary>
        public byte[] Array
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the offset within the array
        /// </summary>
        public int Offset
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the length of the decryption parameters
        /// </summary>
        public int Length
        {
            get; set;
        }
        
        /// <summary>
        /// Initialize the <see cref="DecryptParams"/> with the given values
        /// </summary>
        /// <param name="array">The array as received on the wire</param>
        /// <param name="offset">The offset within the array</param>
        /// <param name="length">The length of the decryption parameters</param>
        public void SetValues(byte[] array, int offset, int length)
        {
            this.Array = array;
            this.Offset = offset;
            this.Length = length;
        }
    }
}
