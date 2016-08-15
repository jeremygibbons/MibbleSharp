// <copyright file="ParseException.cs" company="None">
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

namespace JunoSnmp.Util
{
    using System;

    public class ParseException : FormatException
    {
        int line = -1;
        int col = -1;
        int offset = -1;

        public ParseException() : base()
        {
        }

        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ParseException(int offset) : this(offset, string.Empty)
        {
        }

        public ParseException(int offset, string message) : this(offset, message, null)
        {
        }

        public ParseException(int offset, string message, Exception innerException) : base(message, innerException)
        {
            this.offset = offset;
        }

        public ParseException(int line, int col) : this(line, col, string.Empty)
        {
        }

        public ParseException(int line, int col, string message) : this(line, col, message, null)
        {
        }

        public ParseException(int line, int col, string message, Exception innerException) : base(message, innerException)
        {
            this.line = line;
            this.col = col;
        }

        public override string ToString()
        {
            if(offset != -1)
            {
                string msg = "ParseException at offset " + offset;
                if(string.IsNullOrEmpty(this.Message) == false)
                {
                    msg += ": " + this.Message;
                }
                return msg;
            }

            if (line != -1)
            {
                string msg = "ParseException at line " + this.line + " and column " + this.col;
                if (string.IsNullOrEmpty(this.Message) == false)
                {
                    msg += ": " + this.Message;
                }
                return msg;
            }

            return "ParseException: " + this.Message;
        }
    }
}
