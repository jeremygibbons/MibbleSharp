// <copyright file="IRequestStatistics.cs" company="None">
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
    /// The <c>RequestStatistics</c> interface defines statistic values about request processing.
    /// </summary>
    public interface IRequestStatistics
    {
        /**
         * Gets the total number of messages that have been sent on behalf of this request.
         * @return
         *    the number of messages sent (number of retries plus one).
         */

        /**
         * Sets the total number of messages that have been sent on behalf of this request.
         * @param totalMessagesSent
         *    the total message count for this request.
         */
        int TotalMessagesSent { get; set; }

        /**
         * Gets the index of the message that has been responded.
         * @return
         *    0 if the initial message has been responded by the command responder.
         *    A value greater than zero indicates, that a retry message has been responded.
         */

        /**
         * Sets the index of the message that has been responded.
         * @param indexOfMessageResponded
         *    the zero-based index of the message for which the response had been received.
         */
        int IndexOfMessageResponded { get; set; }

        /**
         * Gets the time elapsed between the sending of the message and receiving its response.
         * @return
         *    the runtime of the successful request and response message pair in nanoseconds.
         */

        /**
         * Sets the time elapsed between the sending of the message and receiving its response.
         * @param responseRuntimeNanos
         *    the runtime of the successful request and response message pair in nanoseconds.
         */
        long ResponseRuntimeNanos { get; set; }
    }

}
