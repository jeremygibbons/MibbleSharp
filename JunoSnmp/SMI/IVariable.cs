// <copyright file="IVariable.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using JunoSnmp.ASN1;

    /// <summary>
    ///  The <code>Variable</code> interface defines common attributes of all SNMP
    ///  variables.
    /// </summary>
    /// <remarks>
    /// <see cref="ICloneable"/> is implemented as cloning can be used by the JunoSnmp API to better
    /// support concurrency by creating a clone for internal processing.
    /// The content of this object is independent to the content of the clone.
    /// Thus, changes to the clone will have no effect to this object. 
    /// </remarks>
    public interface IVariable : IComparable<IVariable>, IBERSerializable, ICloneable
    {
        /// <summary>
        /// Gets the ASN.1 syntax identifier value of this SNMP variable. The value is
        /// an integer value &lt; 128 for regular SMI objects and a value >= 128
        /// for exception values like <c>noSuchObject</c>, <c>noSuchInstance</c>,
        /// and <c>endOfMibView</c>.
        /// </summary>
        int Syntax { get; }

        /// <summary>
        /// <para>Gets a value indicating whether this variable represents an exception like
        /// <c>noSuchObject</c>, <c>noSuchInstance</c>, and <c>endOfMibView</c>.</para><para>
        /// Returns <c>True</c> if the syntax of this variable is an instance of
        /// <c>Null</c> and its syntax equals one of the following:
        /// - <see cref="SMIConstants.EXCEPTION_NO_SUCH_OBJECT"/>
        /// - <see cref="SMIConstants.EXCEPTION_NO_SUCH_INSTANCE"/>
        /// - <see cref="SMIConstants.EXCEPTION_END_OF_MIB_VIEW"/>
        /// </para>
        /// </summary>
        bool IsException { get; }

        /// <summary>
        /// Gets an integer representation of this variable if
        /// such a representation exists. If the native representation of
        /// the variable is a long, the value is down-casted to <c>int</c>
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If no meaningful integer representation exists
        /// </exception>
        int IntValue { get; }

        /// <summary>
        /// Gets a long representation of this variable if
        /// such a representation exists.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// If no meaningful long representation exists
        /// </exception>
        long LongValue { get; }

        /// <summary>
        /// Gets a textual description of this Variable, such as 'Integer32', as
        /// used in the SMI modules. '?' i returned if the syntax is unknown
        /// </summary>
        string SyntaxString { get; }

        /// <summary><para>
        /// Gets a value indicating whether this variable is dynamic. If a variable is dynamic,
        /// precautions have to be taken when a Variable is serialized using BER
        /// encoding, because between determining the length with
        /// <see cref="BERLength"/>} for encoding enclosing SEQUENCES and the actual
        /// encoding of the Variable itself with <see cref="EncodeBER"/> changes to the
        /// value need to be blocked by synchronization.
        /// </para><para>
        /// In order to ensure proper synchronization if an <c>IVariable</c> is
        /// dynamic, modifications of the variable's content need to synchronize on
        /// the <c>IVariable</c> instance. This can be achieved for the standard
        /// SMI Variable implementations for example by
        /// </para><para>
        /// <c>
        ///    public static ModifyVariable(Integer32 variable, int value)
        ///      synchronize(variable) {
        ///        variable.Value = value;
        ///      }
        ///    }
        /// </c>
        /// </para><para>
        /// Returns <c>True</c> if the variable might change its value between
        /// two calls to <see cref="BERLength"/> and <see cref="EncodeBER"/> and
        /// <c>False</c> if the value is immutable or if its value does
        /// not change while serialization because of measures taken by the
        /// implementing class (i.e. variable cloning).
        /// </para>
        /// </summary>
        bool IsDynamic { get; }

        /// <summary>
        /// Converts the value of this <code>Variable</code> to a (sub-)index
        /// value.
        /// </summary>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only (e.g. {@link OctetString} and
        /// <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <returns>An OID that represents this value as an (sub-)index</returns>
        /// <exception cref="NotSupportedException">
        /// If the variable cannot be used in an index
        /// </exception>
        OID ToSubIndex(bool impliedLength);

        /// <summary>
        /// Sets the value of this <c>Variable</c> from the supplied (sub-)index.
        /// </summary>
        /// <param name="subIndex">The sub-index OID.</param>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only (e.g. {@link OctetString} and
        /// <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If the variable cannot be used in an index
        /// </exception>
        void FromSubIndex(OID subIndex, bool impliedLength);
    }
}
