// <copyright file="Salt.cs" company="None">
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
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;

    /// <summary>
    /// Class that holds a 64 bit salt value for crypto operations.
    /// This class tries to use the .Net RNGCryptoServiceProvider class to initialize
    /// the salt value. If the RNG CSP is not available the non-secure System.Random
    /// class is used.
    /// </summary>
    /// <remarks>The Class is implemented as a singleton</remarks>
    public class Salt
    {
        private long salt;

        private static Salt instance = null;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="Salt"/> class. This default
        /// constructor initializes the salt to a random value.
        /// </summary>
        protected Salt()
        {
            byte[] rnd = new byte[8];

            try
            {
                Salt.rngCsp.GetBytes(rnd);
            }
            catch (CryptographicException)
            {
                log.Warn("Cryptographic service provider could not be acquired. Using System.Random instead.");
                Random r = new Random();
                r.NextBytes(rnd);
            }

            this.salt = rnd[0];

            for (int i = 0; i < 7; i++)
            {
                this.salt = (this.salt * 256) + ((int)rnd[i]) + 128;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("Initialized Salt to " + Convert.ToString(this.salt, 16) + ".");
            }
        }

        /// <summary>
        /// Get an initialized Salt instance
        /// </summary>
        /// <returns>The Salt singleton instance</returns>
        public static Salt GetInstance()
        {
            if (Salt.instance == null)
            {
                Salt.instance = new Salt();
            }

            return Salt.instance;
        }

        /// <summary>
        /// Get the next value of the salt by adding one to its current value.
        /// This might result in a predictable salt value if it is not combined with
        /// other somehow unpredictable(random) data.
        /// </summary>
        /// <returns>Previous value increased by one.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long Next()
        {
            return this.salt++;
        }
    }
}
