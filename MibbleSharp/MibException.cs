//
// MibException.cs
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
// Original Java code Copyright (c) 2004-2016 Per Cederberg. All
// rights reserved.
// C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MibbleSharp
{
    ///
    /// <summary>
    /// A MIB exception. This exception is used to report processing
    /// errors for operations on MIB types and values.
    /// </summary>
    ///
    [Serializable]
    public class MibException : Exception
    {
        /// <summary>
        /// The FileLocation where the exception was raised
        /// </summary>
        private FileLocation location;

        /// 
        /// <summary>
        /// Create a new MibException
        /// </summary>
        /// <param name="location">The FileLocation where the exception is being raised</param>
        /// <param name="message">The detailed exception error message</param>
        /// 
        public MibException(FileLocation location, string message) : base(message)
        {
            this.location = location;
        }

        /// 
        /// <summary>
        /// Create a new MibException
        /// </summary>
        /// <param name="file">The filename where the exception is being raised</param>
        /// <param name="line">The line within the file where the exception was raised</param>
        /// <param name="column">The column within the file where the exception was raised</param>
        /// <param name="message">The detailed exception error message</param>
        /// 
        public MibException(string file, int line, int column, string message) : this(new FileLocation(file, line, column), message)
        {

        }

        /// 
        /// <summary>
        /// Deserialize a MibException 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        protected MibException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.location = (FileLocation) info.GetValue("Location", typeof(FileLocation));
        }

        /// 
        /// <summary>
        /// The FileLocation where this exception was raised
        /// </summary>
        /// 
        public FileLocation Location
        {
            get
            {
                return location;
            }
        }

        /// 
        /// <summary>
        /// Return object data for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// 
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("Location", this.Location);
            
            // Call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
