//
// ParserLogException.cs
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PerCederberg.Grammatica.Runtime {

    /// 
    /// <summary>
    /// A parser log exception. This class contains a list of all the 
    /// parse errors encountered while parsing.
    /// </summary>
    /// 
    [Serializable]
    public class ParserLogException : Exception {

        private List<ParseException> errors = new List<ParseException>();

        /// 
        /// <summary>
        /// Create an empty Parser Log Exception object
        /// </summary>
        /// 
        public ParserLogException() {
        }

        /// 
        /// <summary>
        /// Deserialize a ParserLogException object
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public ParserLogException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            errors = (List<ParseException>)info.GetValue("Errors", typeof(List<ParseException>));
        }

        /// 
        /// <summary>
        /// The message property (read-only). This property contains the
        /// detailed exception error message.
        /// </summary>
        /// 
        public override string Message {
            get{
                return String.Join("\n", errors);
            }
        }

        /// 
        /// <summary>
        /// The number of errors in the log
        /// </summary>
        /// 
        public int Count {
            get {
                return errors.Count;
            }
        }

        /// 
        /// <summary>
        /// An enumeration of the ParseException objects in the log
        /// </summary>
        /// 
        public IEnumerable<ParseException> Errors
        {
            get
            {
                return errors;
            }
        }

        /// 
        /// <summary>
        /// Add a ParseException to the log
        /// </summary>
        /// <param name="e">The ParseException to be added</param>
        /// 
        public void AddError(ParseException e) {
            errors.Add(e);
        }

        /// 
        /// <summary>
        /// Serialize a ParserLogException
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Errors", errors);
            base.GetObjectData(info, context);
        }
    }
}
