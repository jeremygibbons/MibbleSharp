// <copyright file="SimpleVariableTextFormat.cs" company="None">
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

namespace JunoSnmp.Util
{
    using System;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <c>SimpleVariableTextFormat</c> implements a simple textual
    /// representation for SNMP variables based on their type only.
    /// No MIB information is used (or in fact can be used).
    /// </summary>
    public class SimpleVariableTextFormat : IVariableTextFormat
    {
        /// <summary>
        /// Creates a simple variable text format.
        /// </summary>
        public SimpleVariableTextFormat()
        {
        }
        
        /// <summary>
        /// Returns a textual representation of the supplied variable against the
        /// optionally supplied instance OID.
        /// </summary>
        /// <param name="instanceOID"> the instance OID <c>variable</c> is associated
        /// with.If <c>null</c> the formatting cannot take any MIB specification of the 
        /// variable into account and has to format it based on its type only.
        /// </param>
        /// <param name="variable">The variable to format</param>
        /// <param name="withOID">
        /// if <c>true</c> the <code>instanceOID</code> should be included
        /// in the textual representation to form a <see cref="VariableBinding"/>
        /// representation.
        /// </param>
        /// <returns>The textual representation</returns>
        public string Format(OID instanceOID, IVariable variable, bool withOID)
        {
            return (withOID) ?
                JunoSnmpSettings.OIDTextFormat.Format(instanceOID.GetValue()) +
                " = " + variable
                : variable.ToString();
        }
        
        /// <summary>
        /// This operation is not supported by <see cref="SimpleVariableTextFormat"/>.
        /// </summary>
        /// <param name="smiSyntax">
        /// The SMI syntax identifier identifying the target <c>IVariable</c>
        /// </param>
        /// <param name="text">A textual representation of the variable</param>
        /// <returns>The new <c>IVariable</c> instance</returns>
        /// <exception cref="ParseException">
        /// If the variable cannot be parsed successfully
        /// </exception>
        public IVariable Parse(int smiSyntax, string text)
        {
            IVariable v = AbstractVariable.CreateFromSyntax(smiSyntax);
            if (v is IAssignableFrom<string> s)
            {
                s.SetValue(text);
            }

            return v;
        }

        /// <summary>
        /// This operation is not supported by <see cref="SimpleVariableTextFormat"/>.
        /// </summary>
        /// <param name="classOrInstanceOID">
        /// the instance OID <c>variable</c> is associated with. Must not
        /// be <c>null</c>.
        /// </param>
        /// <param name="text">A textual representation of the variable</param>
        /// <returns>The new <c>IVariable</c> instance</returns>
        /// <exception cref="ParseException">If the variable cannot be parsed successfully</exception>
        public IVariable Parse(OID classOrInstanceOID, string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Parses a textual representation of a variable binding.
        /// </summary>
        /// <param name="text">A textual representation of the variable binding</param>
        /// <returns>The new <see cref="VariableBinding"/> instance</returns>
        /// <exception cref="ParseException">If the variable binding could not be parsed successfully</exception>
        public VariableBinding ParseVariableBinding(string text)
        {
            int assignmentPos = text.IndexOf(" = ", StringComparison.InvariantCulture);
            if (assignmentPos <= 0)
            {
                throw new ParseException(0, "Could not locate assignment ' = ' string in '" + text);
            }

            OID oid = new OID(
                JunoSnmpSettings.OIDTextFormat.Parse(text.Substring(0, assignmentPos)));
            IVariable var = this.Parse(oid, text.Substring(assignmentPos + 3));
            return new VariableBinding(oid, var);
        }
    }

}
