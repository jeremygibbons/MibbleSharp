// <copyright file="VariantVariable.cs" company="None">
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
    using System.IO;
    using System.Runtime.CompilerServices;
    using JunoSnmp.ASN1;

    /// <summary><para>
    /// The <see cref="VariantVariable"/> provides a decorator for any type
    /// of <see cref="IVariable"/> instance, to be able to intercept or monitor variable
    /// value modification by using a <see cref="IVariantVariableCallback"/>.
    /// </para><para>
    /// This class will work for implementations that use <see cref="Syntax"/>
    /// method to determine the variables syntax. However "instanceof" will not
    /// work.
    /// </para><para>
    /// In contrast to the native <code>IVariable</code> implementations,
    /// <code>VariantVariable</code> can be modified dynamically (i.e. while
    /// a PDU is being BER encoded where this variable has been added to) without
    /// causing BER encoding errors.
    /// </para></summary>
    public class VariantVariable :
        AbstractVariable,
        IAssignableFrom<int>,
        IAssignableFrom<long>,
        IAssignableFrom<string>,
        IAssignableFrom<byte[]>,
        IEquatable<VariantVariable>
    {

        private readonly IVariable variable;
        private readonly IVariantVariableCallback callback;

        /// <summary>
        /// Creates a variant variable wrapping the specified value.
        /// </summary>
        /// <param name="initialVariable">An <see cref="IVariable"/></param>
        public VariantVariable(IVariable initialVariable)
        {
            if (initialVariable == null)
            {
                throw new ArgumentNullException("initialVariable");
            }

            this.variable = initialVariable;
        }

        /// <summary>
        /// Creates a variant variable wrapping the specified value and a callback
        /// that monitors value modifications.
        /// </summary>
        /// <param name="initialVariable">An <see cref="IVariable"/></param>
        /// <param name="callback">
        /// A callback handler that is called before the value is to be modified
        /// and after it has been modified
        public VariantVariable(IVariable initialVariable,
                               IVariantVariableCallback callback)
            : this(initialVariable)
        {
            this.callback = callback;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int CompareTo(IVariable o)
        {
            this.UpdateVariable();
            return this.variable.CompareTo(o);
        }

        protected void UpdateVariable()
        {
            if (callback != null)
            {
                callback.UpdateVariable(this);
            }
        }

        protected void VariableUpdated()
        {
            if (callback != null)
            {
                callback.VariableUpdated(this);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void DecodeBER(BERInputStream inputStream)
        {
            variable.DecodeBER(inputStream);
            this.VariableUpdated();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void EncodeBER(Stream outputStream)
        {
            this.UpdateVariable();
            variable.EncodeBER(outputStream);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            this.variable.FromSubIndex(subIndex, impliedLength);
            this.VariableUpdated();
        }

        public override int BERLength
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                this.UpdateVariable();
                return this.variable.BERLength;
            }
        }

        public override int Syntax
        {
            get
            {
                return this.variable.Syntax;
            }

        }


        public override int IntValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                this.UpdateVariable();
                return this.variable.IntValue;
            }
        }


        public override long LongValue
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                this.UpdateVariable();
                return this.variable.LongValue;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override OID ToSubIndex(bool impliedLength)
        {
            this.UpdateVariable();
            return this.variable.ToSubIndex(impliedLength);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Equals(object o)
        {
            this.UpdateVariable();
            return this.variable.Equals(o);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Equals(VariantVariable other)
        {
            this.UpdateVariable();
            return this.variable.Equals(other);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int GetHashCode()
        {
            this.UpdateVariable();
            return this.variable.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override string ToString()
        {
            this.UpdateVariable();
            return this.variable.ToString();
        }

        public override object Clone()
        {
            this.UpdateVariable();
            return new VariantVariable((IVariable)this.variable.Clone());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetValue(int val)
        {
            if (variable is IAssignableFrom<int> intvar)
            {
                intvar.SetValue(val);
            }
            else
            {
                throw new InvalidCastException("An integer value cannot be assigned to " +
                                             variable);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetValue(long val)
        {
            if (variable is IAssignableFrom<long> longvar)
            {
                longvar.SetValue(val);
            }
            else
            {
                throw new InvalidCastException("A long value cannot be assigned to " +
                                             variable);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetValue(OctetString value)
        {
            if (variable is IAssignableFrom<byte[]> byt)
            {
                byt.SetValue(value.GetValue());
            }
            else
            {
                throw new InvalidCastException("An OctetString value cannot be assigned to " +
                                             variable);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetValue(byte[] val)
        {
            if (variable is IAssignableFrom<byte[]> byt)
            {
                 byt.SetValue(val);
            }
            else
            {
                throw new InvalidCastException("A byte array value cannot be assigned to " +
                                             variable);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetValue(string val)
        {
            if (variable is IAssignableFrom<string> str)
            {
                str.SetValue(val);
            }
            else
            {
                throw new InvalidCastException("A string value cannot be assigned to " +
                                             variable);
            }
        }

        /// <summary>
        /// Returns the typed variable that holds the wrapped value.
        /// @return
        ///    a Variable instance.

        public IVariable Variable
        {
            get
            {
                return variable;
            }
        }

        public override bool IsDynamic
        {
            get
            {
                return true;
            }
        }
    }
}
