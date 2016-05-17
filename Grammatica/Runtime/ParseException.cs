/*
 * ParseException.cs
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
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PerCederberg.Grammatica.Runtime
{

    /// <summary>
    /// A parse exception
    /// </summary>
    [Serializable]
    public class ParseException : Exception
    {

        /// 
        /// <summary>
        /// The ErrorType enumeration
        /// </summary>
        /// 
        public enum ErrorType
        {

            /// 
            /// <summary>
            /// The internal error type is only used to signal an error
            /// that is a result of a bug in the parser or tokenizer
            /// code.
            /// </summary>
            /// 
            INTERNAL,

            /// 
            /// <summary>
            /// The I/O error type is used for stream I/O errors.
            /// </summary>
            /// 
            IO,

            /// 
            /// <summary>
            /// The unexpected end of file error type is used when end
            /// of file is encountered instead of a valid token.
            /// </summary>
            /// 
            UNEXPECTED_EOF,

            /// 
            /// <summary>
            /// The unexpected character error type is used when a
            /// character is read that isn't handled by one of the
            /// token patterns.
            /// </summary>
            /// 
            UNEXPECTED_CHAR,

            /// 
            /// <summary>
            /// The unexpected token error type is used when another
            /// token than the expected one is encountered.
            /// </summary>
            /// 
            UNEXPECTED_TOKEN,
            
            /// 
            /// <summary>
            /// The invalid token error type is used when a token
            /// pattern with an error message is matched. The
            /// additional information provided should contain the
            /// error message.
            /// </summary>
            /// 
            INVALID_TOKEN,

            /// 
            /// <summary>
            /// The analysis error type is used when an error is
            /// encountered in the analysis. The additional information
            /// provided should contain the error message.
            /// </summary>
            /// 
            ANALYSIS
        }


        private ErrorType type;

        private string info;

        private IList<string> details;

        private int line;

        private int column;

        /// 
        /// <summary>
        ///  Creates a new parse exception.
        /// </summary>
        /// <param name="type">The parse error type</param>
        /// <param name="info">The additional error information</param>
        /// <param name="line">The line number, or -1 if unknown</param>
        /// <param name="column">The column number, or -1 if unknown</param>
        /// 
        public ParseException(ErrorType type,
                              string info,
                              int line,
                              int column)
            : this(type, info, null, line, column)
        {
        }

        /// 
        /// <summary>
        ///  Creates a new parse exception. This constructor is only
        ///  used to supply the detailed information array, which is
        ///  only used for expected token errors. The list then contains
        ///  descriptions of the expected tokens.
        /// </summary>
        /// <param name="type">The parse error type</param>
        /// <param name="info">The additional error information</param>
        /// <param name="details">The detailed error information</param>
        /// <param name="line">The line number, or -1 if unknown</param>
        /// <param name="column">The column number, or -1 if unknown</param>
        /// 
        public ParseException(ErrorType type,
                              string info,
                              IList<string> details,
                              int line,
                              int column)
        {

            this.type = type;
            this.info = info;
            this.details = details;
            this.line = line;
            this.column = column;
        }

        /// 
        /// <summary>
        /// Deserialize a ParseException object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            line = info.GetInt32("Line");
            column = info.GetInt32("Column");
            details = (IList<string>)info.GetValue("Details", typeof(IList<string>));
            this.info = info.GetString("Info");
            type = (ErrorType)info.GetInt32("Type");
        }

        /// 
        /// <summary>
        /// Error type property
        /// </summary>
        /// <remarks>Readonly</remarks>
        /// 
        public ErrorType Type
        {
            get
            {
                return type;
            }
        }

        /// 
        /// <summary>
        /// Additional error information property
        /// </summary>
        /// <remarks>Readonly</remarks>
        /// 
        public string Info
        {
            get
            {
                return info;
            }
        }

        /// 
        /// <summary>
        /// Detailed error information property
        /// </summary>
        /// <remarks>Readonly</remarks>
        /// 
        public IEnumerable<string> Details
        {
            get
            {
                return details;
            }
        }

        /// 
        /// <summary>
        /// The line number where the error occured. Returns -1 if unknown
        /// </summary>
        /// 
        public int Line
        {
            get
            {
                return line;
            }
        }

        /// 
        /// <summary>
        /// The column number where the error occured. Returns -1 if unknown
        /// </summary>
        /// 
        public int Column
        {
            get
            {
                return column;
            }
        }

        /// 
        /// <summary>
        /// The message property (read-only). This property contains
        /// the detailed exception error message, including line and
        /// column numbers when available.
        /// </summary>
        /// <see cref="ErrorMessage"/>
        /// 
        public override string Message
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                // Add error description
                buffer.Append(ErrorMessage);

                // Add line and column
                if (line > 0 && column > 0)
                {
                    buffer.Append(", on line: ");
                    buffer.Append(line);
                    buffer.Append(" column: ");
                    buffer.Append(column);
                }

                return buffer.ToString();
            }
        }

        /// 
        /// <summary>
        /// Error message property, containing all available error info
        /// except the line and column
        /// </summary>
        /// <remarks>Readonly</remarks>
        /// 
        public string ErrorMessage
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                // Add type and info
                switch (type)
                {
                    case ErrorType.IO:
                        buffer.Append("I/O error: ");
                        buffer.Append(info);
                        break;
                    case ErrorType.UNEXPECTED_EOF:
                        buffer.Append("unexpected end of file");
                        break;
                    case ErrorType.UNEXPECTED_CHAR:
                        buffer.Append("unexpected character '");
                        buffer.Append(info);
                        buffer.Append("'");
                        break;
                    case ErrorType.UNEXPECTED_TOKEN:
                        buffer.Append("unexpected token ");
                        buffer.Append(info);
                        if (details != null)
                        {
                            buffer.Append(", expected ");
                            if (details.Count > 1)
                            {
                                buffer.Append("one of ");
                            }
                            buffer.Append(MessageDetails);
                        }
                        break;
                    case ErrorType.INVALID_TOKEN:
                        buffer.Append(info);
                        break;
                    case ErrorType.ANALYSIS:
                        buffer.Append(info);
                        break;
                    default:
                        buffer.Append("internal error");
                        if (info != null)
                        {
                            buffer.Append(": ");
                            buffer.Append(info);
                        }
                        break;
                }

                return buffer.ToString();
            }
        }

        ///
        /// <summary>
        /// Returns a string containing all the detailed information in
        /// a list. The elements are separated with the CurrentCulture's
        /// ListSeparator string
        /// </summary>
        /// 
        private string MessageDetails
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                for (int i = 0; i < details.Count; i++)
                {
                    if (i > 0)
                    {
                        buffer.Append(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ");
                        if (i + 1 == details.Count)
                        {
                            buffer.Append("or ");
                        }
                    }
                    buffer.Append(details[i]);
                }

                return buffer.ToString();
            }
        }

        /// 
        /// <summary>
        /// Serialize a ParseException object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Line", line);
            info.AddValue("Column", column);
            info.AddValue("Details", details);
            info.AddValue("Info", info);
            info.AddValue("Type", type);

            base.GetObjectData(info, context);
        }
    }
}
