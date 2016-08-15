// <copyright file="PduHandle.cs" company="None">
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

    /**
 * The <code>PduHandle</code> class represents an unique key for a SNMP PDU.
 * It uses an unique transaction ID (request ID) to identify the PDUs.
 *
 * @author Frank Fock
 * @version 1.0.3
 * @since 1.0
 */
    public class PduHandle : ISerializable
    {
        public static readonly int NONE = int.MinValue;
        private int transactionID = NONE;

        /**
         * Creates a <code>PduHandle</code> with a transaction ID set to {@link #NONE}.
         */
        public PduHandle()
        {
        }

        /**
         * Creates a <code>PduHandle</code> for the supplied transaction ID.
         * @param transactionID
         *    an unqiue transaction ID.
         */
        public PduHandle(int transactionID)
        {
            TransactionID = transactionID;
        }

        /**
         * Gets the transaction ID of this handle.
         * @return
         *    the transaction ID.
         */

        /**
         * Sets the transaction ID which is typically the request ID of the PDU.
         * @param transactionID
         *    an unqiue transaction ID.
         */
        public int TransactionID
        {
            get
            {
                return this.transactionID;
            }

            set
            {
                this.transactionID = value;
            }
        }

        /**
         * Copy all members from the supplied <code>PduHandle</code>.
         * @param other
         *    a PduHandle.
         */
        public void CopyFrom(PduHandle other)
        {
            this.TransactionID = other.transactionID;
        }

        /**
         * Indicates whether some other object is "equal to" this one.
         *
         * @param obj the reference object with which to compare.
         * @return <code>true</code> if this object is the same as the obj argument;
         *   <code>false</code> otherwise.
         */
        public override bool Equals(object obj)
        {
            return (obj is PduHandle) && (transactionID == ((PduHandle)obj).transactionID);
        }

        /**
         * Returns a hash code value for the object.
         *
         * @return a hash code value for this object.
         */
        public override int GetHashCode()
        {
            return this.transactionID;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "PduHandle[" + this.transactionID + "]";
        }
    }
}
