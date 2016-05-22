// <copyright file="MibException.cs" company="None">
//    <para>
//    This work is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published
//    by the Free Software Foundation; either version 2 of the License,
//    or (at your option) any later version.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    General Public License for more details.</para>
//    <para>
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// A MIB exception. This exception is used to report processing
    /// errors for operations on MIB types and values.
    /// </summary>
    [Serializable]
    public class MibException : Exception
    {
        /// <summary>
        /// The FileLocation where the exception was raised
        /// </summary>
        private FileLocation location;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibException"/> class.
        /// </summary>
        /// <param name="location">The FileLocation where the exception is being raised</param>
        /// <param name="message">The detailed exception error message</param>
        public MibException(FileLocation location, string message) : base(message)
        {
            this.location = location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MibException"/> class
        /// </summary>
        /// <param name="file">The filename where the exception is being raised</param>
        /// <param name="line">The line within the file where the exception was raised</param>
        /// <param name="column">The column within the file where the exception was raised</param>
        /// <param name="message">The detailed exception error message</param>
        public MibException(string file, int line, int column, string message) : this(new FileLocation(file, line, column), message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MibException"/> class, during de-serialization
        /// </summary>
        /// <param name="info">The SerializationInfo object</param>
        /// <param name="context">The StreamingContext object</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected MibException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.location = (FileLocation)info.GetValue("Location", typeof(FileLocation));
        }

        /// <summary>
        /// Gets the FileLocation where this exception was raised
        /// </summary>
        public FileLocation Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Return object data for serialization
        /// </summary>
        /// <param name="info">The SerializationInfo object</param>
        /// <param name="context">The StreamingContext object</param>
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
