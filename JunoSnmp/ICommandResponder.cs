// <copyright file="ICommandResponder.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp
{
    using JunoSnmp.Event;

    /// <summary>
    /// <c>ICommandResponder</c> implementations process incoming requests, report and
    /// notification PDUs. An event may only processed once. A command responder
    /// must therefore set the <c>processed</c> member of the supplied
    /// <c>CommandResponderEvent</c> object to <c>true</c> when it has
    /// processed the PDU.
    /// </summary>
    public interface ICommandResponder ////: EventListener
    {
        /// <summary>
        /// Process an incoming request, report or notification PDU.
        /// </summary>
        /// <param name="ev">
        /// a <c>CommandResponderEvent</c> instance containing the PDU to
        /// process and some additional information returned by the message
        /// processing model that decoded the SNMP message.
        /// </param>
        void ProcessPdu(object source, CommandResponderArgs ev);
    }
}