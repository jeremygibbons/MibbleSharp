// <copyright file="SecurityModels.cs" company="None">
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
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
 
    /// <summary>
    /// The <c>SecurityModels</c> class is a collection of all
    /// supported security models of a SNMP entity.
    /// </summary>
    public class SecurityModels
    {
        private IDictionary<SecurityModel.SecurityModelID, SecurityModel> securityModels = new Dictionary<SecurityModel.SecurityModelID, SecurityModel>(3);

        private static SecurityModels instance = null;

        protected SecurityModels()
        {
        }

        /// <summary>
        /// Gets the security singleton instance.
        /// </summary>
        public static SecurityModels Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (SecurityModels.instance == null)
                {
                    SecurityModels.instance = new SecurityModels();
                }

                return SecurityModels.instance;
            }
        }
        
        /// <summary>
        /// Gets the <see cref="SecurityModel"/> referenced by the provided id
        /// </summary>
        /// <param name="id">The id of the Security Model</param>
        /// <returns>The requested Security Model, or null if it does not exist</returns>
        public SecurityModel this[SecurityModel.SecurityModelID id]
        {
            get
            {
                if(this.securityModels.ContainsKey(id))
                {
                    return this.securityModels[id];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the SecurityModels collection instance that contains the supplied
        /// <see cref="SecurityModel"/>s.
        /// </summary>
        /// <param name="models">An enumerable collection of <see cref="SecurityModel"/>s</param>
        /// <returns>
        /// A new instance of <see cref="SecurityModels"/> containing the supplied
        /// <see cref="SecurityModel"/>s.
        /// </returns>
        public static SecurityModels GetCollection(SecurityModel[] models)
        {
            SecurityModels smc = new SecurityModels();
            foreach (var model in models)
            {
                smc.AddSecurityModel(model);
            }

            return smc;
        }
        
        /// <summary>
        /// Adds a security model to the central repository of security models.
        /// </summary>
        /// <param name="model">
        /// A <c>SecurityModel</c>. If a security model with the same ID already exists, it is replaced
        /// </param>
        public void AddSecurityModel(SecurityModel model)
        {
            this.securityModels.Add(model.ID, model);
        }
        
        /// <summary>
        /// Removes a security model from the central repository of security models.
        /// </summary>
        /// <returns>
        /// The removed <see cref="SecurityModel"/>
        /// </returns>
        public SecurityModel RemoveSecurityModel(SecurityModel.SecurityModelID id)
        {
            SecurityModel sm = this.securityModels[id];
            this.securityModels.Remove(id);
            return sm;
        }
    }
}
