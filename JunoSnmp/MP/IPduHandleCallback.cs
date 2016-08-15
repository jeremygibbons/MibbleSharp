// <copyright file="IPduHandleCallback.cs" company="None">
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
    /// <summary>
    /// The <c>PduHandleCallback</c> can be used to get informed about a
    /// <c>PduHandle</c> creation before a request is actually sent out.
    /// </summary>
    /// <typeparam name="P">The PDU type</typeparam>
    public interface IPduHandleCallback<P>
    {
        /**
         * A new PduHandle has been created for a PDU. This event callback
         * notification can be used to get informed about a new PduHandle
         * (just) before a PDU has been sent out.
         *
         * @param handle
         *   a <code>PduHandle</code> instance that uniquely identifies a request -
         *   thus in most cases the request ID.
         * @param pdu
         *    the request PDU for which the handle has been created.
         */
        void PduHandleAssigned(PduHandle handle, P pdu);
    }
}
