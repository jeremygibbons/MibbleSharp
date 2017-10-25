// <copyright file="TableEventArgs.cs" company="None">
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
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Event
{
    using System;
    using System.Linq;
    using JunoSnmp.SMI;
   
    /// <summary>
    /// The TableEventArgs class reports events in a table retrieval operation
    /// </summary>
    public class TableEventArgs : RetrievalEventArgs
    {
        private readonly OID index;

        protected TableEventArgs(Object userObject) : base(userObject)
        {
            this.userObject = userObject;
        }

        /**
         * Creates a table event with a status.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param status
         *    one of the status constants defined for this object.
         */
        public TableEventArgs(Object userObject, int status) : this(userObject)
        {
            this.status = status;
        }

        /**
         * Creates a table event with an exception.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param exception
         *    an exception instance.
         */
        public TableEventArgs(Object userObject, Exception exception) : this(userObject)
        {
            this.exception = exception;
            this.status = STATUS_EXCEPTION;
        }

        /**
         * Creates a table event with a report PDU.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param report
         *    a PDU of type {@link PDU#REPORT}.
         */
        public TableEventArgs(Object userObject, PDU report) : this(userObject)
        {
            this.reportPDU = report;
            this.status = STATUS_REPORT;
        }

        /**
         * Creates a table event with row data.
         *
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param index
         *    the index OID of the row.
         * @param cols
         *    an array of <code>VariableBinding</code> instances containing the
         *    elements of the row. The array may contain <code>null</code> elements
         *    which indicates that the agent does not return an instance for that
         *    column and row index. If an element is not <code>null</code>, then
         *    the <code>OID</code> of the variable binding contains the full instance
         *    <code>OID</code> (i.e., table OID + column ID + row index) of the variable.
         */
        public TableEventArgs(Object userObject, OID index, VariableBinding[] cols) : base(userObject, cols)
        {
            this.index = index;
        }

        /// <summary>
        /// Gets the row index OID, or null if <see cref="IsError"/> returns <code>true</code>
        /// </summary>
        public OID Index
        {
            get => this.index;
        }

        /// <summary>
        /// Gets the columnar objects of the row, returning an array of
        /// <code>VariableBinding</code> instances containing the
        /// elements of the row.The array may contain<code>null</code> elements
        /// which indicates that the agent does not return an instance for that
        /// column and row index.If an element is not<code>null</code>, then
        /// the <code>OID</code> of the variable binding contains the full instance
        /// <code>OID</code> of the variable.<p>
        /// If <see cref="IsError"/> returns <code>true</code>, <code>null</code>
        /// will be returned.
        /// </summary>
        public VariableBinding[] Columns
        {
            get => vbs;
        }

        public override string ToString()
        {
            return this.GetType().Name + "[index=" + index + ",vbs=" +
                ((vbs == null) ? "null" : "" + string.Join(", ", vbs.ToList())) +
                ",status=" + status + ",exception=" +
                exception + ",report=" + reportPDU + "]";
        }
    }
}
