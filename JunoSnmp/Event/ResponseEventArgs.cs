// <copyright file="ResponseEvent.cs" company="None">
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
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Event
{
    using System;
    using JunoSnmp.SMI;

    public delegate void ResponseHandler(object o, ResponseEventArgs args);

    /**
 * <code>ResponseEvent</code> associates a request PDU with the corresponding
 * response and an optional user object.
 *
 * @author Frank Fock
 * @version 1.1
 */
    public class ResponseEventArgs : EventArgs
    {
        private IAddress peerAddress;
        private PDU request;
        private PDU response;
        private object userObject;
        private readonly Exception error;

        /**
         * Creates an <code>ResponseEvent</code> instance.
         * @param source
         *    the event source.
         * @param peerAddress
         *    the transport address of the entity that send the response.
         * @param request
         *    the request PDU (must not be <code>null</code>).
         * @param response
         *    the response PDU or <code>null</code> if the request timed out.
         * @param userObject
         *    an optional user object.
         */
        public ResponseEventArgs(
            IAddress peerAddress,
            PDU request,
            PDU response,
            object userObject)
        {
            this.PeerAddress = peerAddress;
            this.Request = request;
            this.Response = response;
            this.UserObject = userObject;
        }

        /**
         * Creates an <code>ResponseEvent</code> instance with an exception object
         * indicating a message processing error.
         * @param source
         *    the event source.
         * @param peerAddress
         *    the transport address of the entity that send the response.
         * @param request
         *    the request PDU (must not be <code>null</code>).
         * @param response
         *    the response PDU or <code>null</code> if the request timed out.
         * @param userObject
         *    an optional user object.
         * @param error
         *    an <code>Exception</code>.
         */
        public ResponseEventArgs(IAddress peerAddress,
                             PDU request, PDU response,
                             object userObject,
                             Exception error)
            : this(peerAddress, request, response, userObject)
        {
            this.error = error;
        }

        /**
         * Gets the request PDU.
         * @return
         *    a <code>PDU</code>.
         */

        public PDU Request
        {
            get
            {
                return request;
            }

            protected set
            {
                this.request = value;
            }
        }

        /**
         * Gets the response PDU.
         * @return
         *    a PDU instance if a response has been received. If the request
         *    timed out then <code>null</code> will be returned.
         */

        public PDU Response
        {
            get
            {
                return response;
            }

            protected set
            {
                this.response = value;
            }
        }

        /**
         * Gets the user object that has been supplied to the asynchronous request
         * {@link Session#send(PDU pdu, org.snmp4j.Target target, Object userHandle,
         * ResponseListener listener)}.
         * @return
         *    an Object.
         */

        public object UserObject
        {
            get
            {
                return userObject;
            }

            protected set
            {
                this.userObject = value;
            }
        }

        /**
         * Gets the exception object from the exception that has been generated
         * when the request processing has failed due to an error.
         * @return
         *    an <code>Exception</code> instance.
         */
        public Exception Error
        {
            get
            {
                return error;
            }
        }

        /**
         * Gets the transport address of the response sender.
         * @return
         *    the transport <code>Address</code> of the command responder that send
         *    this response, or <code>null</code> if no response has been received
         *    within the time-out interval or if an error occured (see
         *    {@link #getError()}).
         */

        public IAddress PeerAddress
        {
            get
            {
                return peerAddress;
            }

            protected set
            {
                this.peerAddress = value;
            }
        }
    }
}
