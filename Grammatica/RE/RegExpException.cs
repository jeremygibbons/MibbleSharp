/*
 * RegExpException.cs
 *
 * This program is free software: you can redistribute it and/or
 * modify it under the terms of the BSD license.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * LICENSE.txt file for more details.
 *
 * Copyright (c) 2003-2015 Per Cederberg. All rights reserved.
 * C# conversion and modifications Copyright (c) 2016 Jeremy Gibbons
 * All rights reserved.
 */

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PerCederberg.Grammatica.Runtime.RE
{

    /// <summary>
    /// A regular expression exception. This exception is thrown if a 
    /// regular expression couldn't be processed (or "compiled") properly.
    /// </summary>
    [Serializable]
    public class RegExpException : Exception
    {

        /// <summary>
        /// ErrorType enumeration
        /// </summary>
        public enum ErrorType
        {

            /// 
            /// <summary>
            /// The unexpected character error constant. This error is
            /// used when a character was read that didn't match the
            /// allowed set of characters at the given position.
            /// </summary>
            ///
            UNEXPECTED_CHARACTER,

            /// 
            /// <summary>
            /// The unterminated pattern error constant. This error is
            /// used when more characters were expected in the pattern.
            /// </summary>
            ///
            UNTERMINATED_PATTERN,

            ///
            /// <summary>
            /// The unsupported special character error constant. This
            /// error is used when special regular expression
            /// characters are used in the pattern, but not supported
            /// in this implementation.
            /// </summary>
            ///
            UNSUPPORTED_SPECIAL_CHARACTER,

            ///
            /// <summary>
            /// The unsupported escape character error constant. This
            /// error is used when an escape character construct is
            /// used in the pattern, but not supported in this
            /// implementation.
            /// </summary>
            ///
            UNSUPPORTED_ESCAPE_CHARACTER,

            ///
            /// <summary>The invalid repeat count error constant. This error is
            /// used when a repetition count of zero is specified, or
            /// when the minimum exceeds the maximum.
            /// </summary>
            ///
            INVALID_REPEAT_COUNT
        }

        private ErrorType type;

        private int position;

        private string pattern;

        /// 
        /// <summary>
        /// Creates a new regular expression exception.
        /// </summary>
        /// <param name="type">The error type</param>
        /// <param name="pos">The error position</param>
        /// <param name="pattern">The regexp pattern</param>
        /// 
        public RegExpException(ErrorType type, int pos, string pattern)
        {
            this.type = type;
            this.position = pos;
            this.pattern = pattern;
        }

        /// 
        /// <summary>
        /// Deserialize a RegExpException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected RegExpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.position = info.GetInt32("Position");
            this.pattern = info.GetString("Pattern");
            this.type = (RegExpException.ErrorType) info.GetInt32("Type");
        }

        /// 
        /// <summary>
        /// The message property. This property contains the detailed 
        /// exception error message.
        /// </summary>
        /// 
        public override string Message
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                // Append error type name
                switch (type)
                {
                    case ErrorType.UNEXPECTED_CHARACTER:
                        buffer.Append("unexpected character");
                        break;
                    case ErrorType.UNTERMINATED_PATTERN:
                        buffer.Append("unterminated pattern");
                        break;
                    case ErrorType.UNSUPPORTED_SPECIAL_CHARACTER:
                        buffer.Append("unsupported character");
                        break;
                    case ErrorType.UNSUPPORTED_ESCAPE_CHARACTER:
                        buffer.Append("unsupported escape character");
                        break;
                    case ErrorType.INVALID_REPEAT_COUNT:
                        buffer.Append("invalid repeat count");
                        break;
                    default:
                        buffer.Append("internal error");
                        break;
                }

                // Append erroneous character
                buffer.Append(": ");
                if (position < pattern.Length)
                {
                    buffer.Append('\'');
                    buffer.Append(pattern.Substring(position));
                    buffer.Append('\'');
                }
                else
                {
                    buffer.Append("<end of pattern>");
                }

                // Append position
                buffer.Append(" at position ");
                buffer.Append(position);

                return buffer.ToString();
            }            
        }

        /// <summary>
        /// Serialize a RegExpException object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Type", type);
            info.AddValue("Position", position);
            info.AddValue("Pattern", pattern);
            base.GetObjectData(info, context);
        }
    }
}
