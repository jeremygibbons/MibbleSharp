// <copyright file="OIDTextFormat.cs" company="None">
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
    /// <summary>
    /// The <code>OIDTextFormat</code> provides a textual representation of a raw
    /// object ID.
    /// </summary>
    public interface OIDTextFormat
    {
        /// <summary>
        /// Returns a textual representation of a raw object ID, for example as
        /// dotted string ("1.3.6.1.4") or object name("ifDescr") depending on the
        /// formats representation rules.
        /// </summary>
        /// <param name="value">The OID value to format</param>
        /// <returns>The textual representation</returns>
        string Format(long[] value);
        
        /// <summary>
        /// Returns a textual representation of a raw object ID, for example as
        /// dotted string ("1.3.6.1.4"), object name plus numerical index("ifDescr.0"),
        /// or other formats that can be parsed again with {@link #parse(String)} to a
        /// the same OID value.
        /// </summary>
        /// <param name="value">The OID value to format</param>
        /// <returns>The textual representation</returns>
        string FormatForRoundTrip(long[] value);
        
        /// <summary>
        /// Parses a textual representation of an object ID and returns its raw value.
        /// </summary>
        /// <param name="text">A textual representation of an OID</param>
        /// <returns>The raw OID value</returns>
        /// <exception cref="ParseException">If the OID cannot be parsed succesfully</exception>
        long[] Parse(string text);
    }
}
