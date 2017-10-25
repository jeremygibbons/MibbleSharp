// <copyright file="RetrievalEventArgs.cs" company="None">
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
    using JunoSnmp.MP;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>RetrievalEvent</code> is an abstract class representing the result
    /// of one or more GET/GETNEXT/GETBULK requests.
    /// </summary>
    public abstract class RetrievalEventArgs : EventArgs
    {
        /// <summary>
        /// Retrieval operation was successful.
        /// </summary>
        public const int STATUS_OK = SnmpConstants.SNMP_ERROR_SUCCESS;

        /// <summary>
        /// A request to the agent timed out.
        /// </summary>
        public const int STATUS_TIMEOUT = SnmpConstants.SNMP_ERROR_TIMEOUT;

        /// <summary>
        /// The agent failed to return the objects in lexicographic order.
        /// </summary>
        public const int STATUS_WRONG_ORDER = SnmpConstants.SNMP_ERROR_LEXICOGRAPHIC_ORDER;

        /// <summary>
        /// A report has been received from the agent.
        /// </summary>
        /// <see cref="ReportPDU"/>
        public const int STATUS_REPORT = SnmpConstants.SNMP_ERROR_REPORT;

        /// <summary>
        /// An exception occurred during retrieval operation.
        /// </summary>
        /// <see cref="Exception"/>
        public const int STATUS_EXCEPTION = -4;

        protected VariableBinding[] vbs;
        protected int status = STATUS_OK;
        protected object userObject;
        protected Exception exception;
        protected PDU reportPDU;

        protected RetrievalEventArgs(object userObject) : base()
        {
            this.userObject = userObject;
        }

        /**
         * Creates a retrieval event with a status.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param status
         *    one of the status constants defined for this object.
         */
        protected RetrievalEventArgs(object userObject, int status) : this(userObject)
        {
            this.status = status;
        }

        /**
         * Creates a retrieval event with an exception.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param exception
         *    an exception instance.
         */
        protected RetrievalEventArgs(object userObject, Exception exception) : this(userObject)
        {
            this.exception = exception;
            this.status = STATUS_EXCEPTION;
        }

        /**
         * Creates a retrieval event with a report PDU.
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param report
         *    a PDU of type {@link PDU#REPORT}.
         */
        protected RetrievalEventArgs(object userObject, PDU report) : this(userObject)
        {
            this.reportPDU = report;
            this.status = STATUS_REPORT;
        }

        /**
         * Creates a retrieval event with row data.
         *
         * @param source
         *    the source of the event.
         * @param userObject
         *    the user object or <code>null</code>.
         * @param variableBindings
         *    an array of <code>VariableBinding</code> instances.
         */
        protected RetrievalEventArgs(Object userObject,
                              VariableBinding[] variableBindings) : this(userObject)
        {
            this.vbs = variableBindings;
        }

        /**
         * Gets the status of the table operation.
         * @return
         *    one of the status constants defined for this object.
         *    {@link #STATUS_OK} indicates success, all other values indicate
         *    failure of the operation which corresponds to a SNMP error status
         *    as defined by {@link org.snmp4j.PDU#getErrorStatus()}.
         */
        public int Status
        {
            get => status;
        }

        /**
         * Indicates whether the event reports an error or not.
         * @return
         *    <code>true</code> if the operation failed with an error.
         */
        public bool IsError
        {
            get => (status != STATUS_OK);
        }

        /**
         * Gets the user object that has been specified by the user when the retrieval
         * operation that fired this event has been requested.
         * @return
         *    an object instance if an user object has been specified or
         *    <code>null</code> otherwise.
         */
        public object UserObject
        {
            get => this.userObject;
        }

        /**
         * Gets the exception associated with this event.
         * @return
         *    an Exception instance if there has been an exception instance
         *    associated with this event ({@link #getStatus()} returns
         *    {@link #STATUS_EXCEPTION}), or <code>null</code> otherwise.
         */
        public Exception Exception
        {
            get => this.exception;
        }

        /**
         * Gets the report PDU associated with this event.
         * @return
         *    a <code>ScopedPDU</code> instance if there has been a report PDU
         *    instance associated with this event ({@link #getStatus()} returns
         *    {@link #STATUS_REPORT}), or <code>null</code> otherwise.
         */
        public PDU ReportPDU
        {
            get => this.reportPDU;
        }

        /**
         * Returns a textual error message for the error.
         * @return
         *    an error message or an empty string if no error occurred.
         */
        public string ErrorMessage
        {
            get
            {
                switch (status)
                {
                    case STATUS_EXCEPTION:
                        {
                            return exception.Message;
                        }
                    case STATUS_REPORT:
                        {
                            return "Report: " + reportPDU[0];
                        }
                    case STATUS_TIMEOUT:
                        {
                            return "Request timed out.";
                        }
                    case STATUS_WRONG_ORDER:
                        {
                            return "Agent did not return variable bindings in lexicographic order.";
                        }
                    default:
                        {
                            return PDU.ToErrorStatusText(status);
                        }
                }
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + "[vbs=" + string.Join(", ", vbs.ToList())
                + ",status=" + status + ",exception=" +
                exception + ",report=" + reportPDU + "]";
        }

    }

}
