// <copyright file="DefaultTimeoutModel.cs" company="None">
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
    /// <summary>
    /// The <code>DefaultTimeoutModel</code> implements a timeout model that uses
    /// constant timeouts between retries.<para>
    /// The total time waited before a request is timed out is therefore:
    /// <c>(totalNumberOfRetries + 1) * targetTimeout</c> where each (re)try
    /// is timed out after <c>targetTimeout</c> milliseconds.</para>
    /// </summary>
    public class DefaultTimeoutModel : ITimeoutModel
    {
        /// <summary>
        /// Gets a new instance of the <see cref="DefaultTimeoutModel"/> class.
        /// </summary>
        public DefaultTimeoutModel()
        {
        }

        /// <summary>
        /// Gets the timeout for the specified retry (a zero value for
        /// <c>retryCount</c> specifies the first request).
        /// </summary>
        /// <param name="retryCount">The number of retries already performed for the target</param>
        /// <param name="totalNumberOfRetries">The total number of retries configured for the target</param>
        /// <param name="targetTimeout">The timeout as specified for the target in milliseconds</param>
        /// <returns>The timeout duration in milliseconds for the supplied entry</returns>
        public long GetRetryTimeout(int retryCount, int totalNumberOfRetries, long targetTimeout)
        {
            return targetTimeout;
        }

        /// <summary>
        /// Gets the timeout for all retries, which is defined as the sum of
        /// <see cref="GetRetryTimeout(int retryCount, int totalNumberOfRetries, long targetTimeout)"/> 
        /// for all <c>retryCount</c> in <c>0 &lt;= retryCount &lt; totalNumberOfRetries</c>.
        /// </summary>
        /// <param name="totalNumberOfRetries">The total number of retries configured for the target</param>
        /// <param name="targetTimeout">The timeout as specified for the target in milliseconds</param>
        /// <returns>The time in milliseconds when the request will be timed out finally.</returns>
        public long GetRequestTimeout(int totalNumberOfRetries, long targetTimeout)
        {
            return (totalNumberOfRetries + 1) * targetTimeout;
        }
    }
}
