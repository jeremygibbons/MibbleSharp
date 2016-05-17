//
// ParserCreationException.cs
// 
// This work is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; either version 2 of the License,
// or (at your option) any later version.
//
// This work is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
// USA
// 
// Original Java code Copyright (c) 2003-2015 Per Cederberg. All
// rights reserved.
// C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PerCederberg.Grammatica.Runtime {

    /// 
    /// <summary>
    /// A parser creation exception. This exception is used for signalling
    /// an error in the token or production patterns, making it impossible
    /// to create a working parser or tokenizer.
    /// </summary>
    /// 
    [Serializable]
    public class ParserCreationException : Exception {

        ///
        /// The error type enumeration.
        ///
        public enum ErrorType {

            ///
            /// <summary>
            /// The internal error type is only used to signal an
            /// error that is a result of a bug in the parser or
            /// tokenizer code.
            /// </summary>
            ///
            INTERNAL,

            ////
            /// <summary>
            /// The invalid parser error type is used when the parser
            /// as such is invalid. This error is typically caused by
            /// using a parser without any patterns.
            /// </summary>
            ///
            INVALID_PARSER,

            ///
            /// <summary>
            /// The invalid token error type is used when a token
            /// pattern is erroneous. This error is typically caused
            /// by an invalid pattern type or an erroneous regular
            /// expression.
            /// </summary>
            ///
            INVALID_TOKEN,

            ///
            /// <summary>
            /// The invalid production error type is used when a
            /// production pattern is erroneous. This error is
            /// typically caused by referencing undeclared productions,
            /// or violating some other production pattern constraint.
            /// </summary>
            ///
            INVALID_PRODUCTION,

            ///
            /// <summary>
            /// The infinite loop error type is used when an infinite
            /// loop has been detected in the grammar. One of the
            /// productions in the loop will be reported.
            /// </summary>
            ///
            INFINITE_LOOP,

            ///
            /// <summary>
            /// The inherent ambiguity error type is used when the set
            /// of production patterns (i.e. the grammar) contains
            /// ambiguities that cannot be resolved.
            /// </summary>
            ///
            INHERENT_AMBIGUITY
        }

        private ErrorType type;

        ///
        /// <summary>
        /// The token or production pattern name. This variable is only
        /// set for some error types.
        /// </summary>
        ///
        private string name;

        ///
        /// <summary>
        /// The additional error information string. This variable is only
        /// set for some error types.
        /// </summary>
        ///
        private string info;

        ///
        /// <summary>
        /// The error details list. This variable is only set for some
        /// error types.
        /// </summary>
        ///
        private IList<string> details;

        ///
        /// <summary>
        /// Creates a new parser creation exception.
        /// </summary>
        /// <param name="type">The parse error type</param>
        /// <param name="info">The additional error information</param>
        public ParserCreationException(ErrorType type, string info)
            : this(type, null, info) {
        }

        ///
        /// <summary>
        /// Creates a new parser creation exception.
        /// </summary>
        /// <param name="type">The parse error type</param>
        /// <param name="name">The token or production pattern name</param>
        /// <param name="info">The additional error information</param>
        /// 
        public ParserCreationException(ErrorType type,
                                       string name,
                                       string info)
            : this(type, name, info, null) {
        }

        ///
        /// <summary>
        /// Creates a new parser creation exception.
        /// </summary>
        /// <param name="type">The parse error type</param>
        /// <param name="name">The token or production pattern name</param>
        /// <param name="info">The additional error information</param>
        /// <param name="details">The details of the errors</param>
        /// 
        public ParserCreationException(ErrorType type,
                                       string name,
                                       string info,
                                       IList<string> details) {

            this.type = type;
            this.name = name;
            this.info = info;
            this.details = details;
        }

        /// 
        /// <summary>
        /// Deserialize a ParserCreationException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public ParserCreationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.name = info.GetString("Name");
            this.type = (ErrorType) info.GetInt32("Type");
            this.info = info.GetString("Info");
            this.details = (IList<string>) info.GetValue("Details", typeof(IList<string>));

        }

        /// 
        /// <summary>
        /// The specific ErrorType
        /// </summary>
        /// 
        public ErrorType Type {
            get {
                return type;
            }
        }

        /// 
        /// <summary>
        /// The token or production name
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// 
        /// <summary>
        /// The additional error information property (read-only).
        /// </summary>
        /// 
        public string Info {
            get {
                return info;
            }
        }

        /// 
        /// <summary>
        /// The detailed error information property (read-only).
        /// </summary>
        /// 
        public string Details {
            get {
                StringBuilder  buffer = new StringBuilder();

                if (details == null) {
                    return null;
                }
                for (int i = 0; i < details.Count; i++) {
                    if (i > 0) {
                        buffer.Append(", ");
                        if (i + 1 == details.Count) {
                            buffer.Append("and ");
                        }
                    }
                    buffer.Append(details[i]);
                }

                return buffer.ToString();
            }
        }


        /// 
        /// <summary>
        /// The message property (read-only). This property contains the
        /// detailed exception error message.
        /// </summary>
        /// 
        public override string Message {
            get{
                StringBuilder  buffer = new StringBuilder();

                switch (type) {
                case ErrorType.INVALID_PARSER:
                    buffer.Append("parser is invalid, as ");
                    buffer.Append(info);
                    break;
                case ErrorType.INVALID_TOKEN:
                    buffer.Append("token '");
                    buffer.Append(name);
                    buffer.Append("' is invalid, as ");
                    buffer.Append(info);
                    break;
                case ErrorType.INVALID_PRODUCTION:
                    buffer.Append("production '");
                    buffer.Append(name);
                    buffer.Append("' is invalid, as ");
                    buffer.Append(info);
                    break;
                case ErrorType.INFINITE_LOOP:
                    buffer.Append("infinite loop found in production pattern '");
                    buffer.Append(name);
                    buffer.Append("'");
                    break;
                case ErrorType.INHERENT_AMBIGUITY:
                    buffer.Append("inherent ambiguity in production '");
                    buffer.Append(name);
                    buffer.Append("'");
                    if (info != null) {
                        buffer.Append(" ");
                        buffer.Append(info);
                    }
                    if (details != null) {
                        buffer.Append(" starting with ");
                        if (details.Count > 1) {
                            buffer.Append("tokens ");
                        } else {
                            buffer.Append("token ");
                        }
                        buffer.Append(Details);
                    }
                    break;
                default:
                    buffer.Append("internal error");
                    break;
                }
                return buffer.ToString();
            }
        }

        /// 
        /// <summary>
        /// Serialize a ParserCreationException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
