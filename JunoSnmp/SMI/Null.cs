// <copyright file="Null.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <code>Null</code> class represents SMI Null and the derived
    /// SMIv2 exception syntaxes.
    /// </summary>
    public class Null : AbstractVariable
    {

        private int syntax = SMIConstants.SyntaxNull;

        public static readonly Null NoSuchObject = new Null(SMIConstants.ExceptionNoSuchObject);
        public static readonly Null NoSuchInstance = new Null(SMIConstants.ExceptionNoSuchInstance);
        public static readonly Null EndOfMibView = new Null(SMIConstants.ExceptionEndOfMibView);
        public static readonly Null Instance = new Null(SMIConstants.SyntaxNull);

        public Null()
        {
        }

        public Null(int exceptionSyntax)
        {
            this.SetSyntax(exceptionSyntax);
        }

        public override void DecodeBER(BERInputStream inputStream)
        {
            BER.MutableByte type = new BER.MutableByte();
            BER.DecodeNull(inputStream, out type);
            this.syntax = type.Value & 0xFF;
        }

        public override int Syntax
        {
            get
            {
                return this.syntax;
            }
        }

        public void SetSyntax(int syntax)
        {
            this.syntax = syntax;
        }

        public override int GetHashCode()
        {
            return this.Syntax;
        }

        public override int BERLength
        {
            get
            {
                return 2;
            }
        }

        public override bool Equals(object o)
        {
            return (o is Null) && (((Null)o).Syntax == this.Syntax);
        }

        public override int CompareTo(IVariable o)
        {
            return (this.Syntax - o.Syntax);
        }

        public override string ToString()
        {
            switch (this.Syntax)
            {
                case SMIConstants.ExceptionNoSuchObject:
                    return "noSuchObject";
                case SMIConstants.ExceptionNoSuchInstance:
                    return "noSuchInstance";
                case SMIConstants.ExceptionEndOfMibView:
                    return "endOfMibView";
            }
            return "Null";
        }

        public override void EncodeBER(Stream outputStream)
        {
            BER.EncodeHeader(outputStream, (byte)this.Syntax, 0);
        }

        public override object Clone()
        {
            return new Null(this.syntax);
        }

        public static bool IsExceptionSyntax(int syntax)
        {
            switch (syntax)
            {
                case SMIConstants.ExceptionNoSuchObject:
                case SMIConstants.ExceptionNoSuchInstance:
                case SMIConstants.ExceptionEndOfMibView:
                    return true;
            }

            return false;
        }

        /**
         * Returns the syntax of this Null variable.
         * @return
         *    {@link SMIConstants#SYNTAX_NULL} or one of the exception syntaxes
         *    {@link SMIConstants#EXCEPTION_NO_SUCH_OBJECT},
         *    {@link SMIConstants#EXCEPTION_NO_SUCH_INSTANCE}, or
         *    {@link SMIConstants#EXCEPTION_END_OF_MIB_VIEW}
         * @since 1.7
         */
        public override int IntValue
        {
            get
            {
                return this.Syntax;
            }
        }

        /**
         * Returns the syntax of this Null variable.
         * @return
         *    {@link SMIConstants#SYNTAX_NULL} or one of the exception syntaxes
         *    {@link SMIConstants#EXCEPTION_NO_SUCH_OBJECT},
         *    {@link SMIConstants#EXCEPTION_NO_SUCH_INSTANCE}, or
         *    {@link SMIConstants#EXCEPTION_END_OF_MIB_VIEW}
         * @since 1.7
         */
        public override long LongValue
        {
            get
            {
                return this.Syntax;
            }       
        }

        /// <summary>
        /// Converts the value of this <c>Variable</c> to a (sub-)index value
        /// </summary>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. { @link OctetString}
        /// and <see cref="OID"/>. For other variables it has no effect.
        /// </param>
        /// <returns>An OID that represents this value as a sub index</returns>
        /// <exception cref="NotSupportedException">If this variable cannot be used as a sub index</exception>
        public override OID ToSubIndex(bool impliedLength)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the value of this <c>IVariable</c> from the supplied (sub-)index.
        /// </summary>
        /// <param name="subIndex">The sub-index OID</param>
        /// <param name="impliedLength">
        /// Specifies if the sub-index has an implied length. This parameter applies
        /// to variable length variables only(e.g. <see cref="OctetString"/>
        /// and <see cref="OID"/>). For other variables it has no effect.
        /// </param>
        /// <exception cref="NotSupportedException">If this variable cannot be set from a sub index</exception>
        public override void FromSubIndex(OID subIndex, bool impliedLength)
        {
            throw new NotSupportedException();
        }
    }
}
