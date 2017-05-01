// <copyright file="ISession.cs" company="None">
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

namespace JunoSnmp
{
    using JunoSnmp.Event;
    using JunoSnmp.SMI;

    /// <summary>
    /// Session</code> defines a common interface for all classes that
    /// implement SNMP protocol operations based on JunoSnmp.
    /// </summary>
    public interface ISession
    {

        /**
         * Closes the session and frees any allocated resources, i.e. sockets.
         * After a <code>Session</code> has been closed it must
         * be used.
         * @throws IOException
         *    if the session could not free all resources.
         */
        void Close();

        /**
         * Sends a <code>PDU</code> to the given target and returns the received
         * response <code>PDU</code>.
         * @param pdu
         *    the <code>PDU</code> to send.
         * @param target
         *    the <code>Target</code> instance that specifies how and where to send
         *    the PDU.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>. If the sent <code>pdu</code>
         *    is an unconfirmed PDU (notification, response, or report), then
         *    <code>null</code> will be returned.
         * @throws IOException
         *    if the message could not be send.
         */
        ResponseEvent Send(PDU pdu, ITarget target);

        /**
         * Asynchronously sends a <code>PDU</code> to the given target. The response
         * is then returned by calling the supplied <code>ResponseListener</code>
         * instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the message could not be send.
         */
        void Send(PDU pdu, ITarget target, object userHandle,
                         ResponseListener listener);

        /**
         * Sends a <code>PDU</code> to the given target and returns the received
         * response <code>PDU</code> encapsulated in a <code>ResponseEvent</code>
         * object that also includes:
         * <ul>
         * <li>the transport address of the response sending peer,
         * <li>the <code>Target</code> information of the target,
         * <li>the request <code>PDU</code>,
         * <li>the response <code>PDU</code> (if any).
         * </ul>
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param transport
         *    specifies the <code>TransportMapping</code> to be used when sending
         *    the PDU. If <code>transport</code> is <code>null</code>, the associated
         *    message dispatcher will try to determine the transport mapping by the
         *    <code>target</code>'s address.
         * @return
         *    the received response encapsulated in a <code>ResponseEvent</code>
         *    instance. To obtain the received response <code>PDU</code> call
         *    {@link ResponseEvent#getResponse()}. If the request timed out,
         *    that method will return <code>null</code>. If the sent <code>pdu</code>
         *    is an unconfirmed PDU (notification, response, or report), then
         *    <code>null</code> will be returned.
         * @throws IOException
         *    if the message could not be send.
         */
        ResponseEvent Send(PDU pdu, ITarget target,
                                  ITransportMapping<IAddress> transport);

        /**
         * Asynchronously sends a <code>PDU</code> to the given target. The response
         * is then returned by calling the supplied <code>ResponseListener</code>
         * instance.
         *
         * @param pdu
         *    the PDU instance to send.
         * @param target
         *    the Target instance representing the target SNMP engine where to send
         *    the <code>pdu</code>.
         * @param transport
         *    specifies the <code>TransportMapping</code> to be used when sending
         *    the PDU. If <code>transport</code> is <code>null</code>, the associated
         *    message dispatcher will try to determine the transport mapping by the
         *    <code>target</code>'s address.
         * @param userHandle
         *    an user defined handle that is returned when the request is returned
         *    via the <code>listener</code> object.
         * @param listener
         *    a <code>ResponseListener</code> instance that is called when
         *    <code>pdu</code> is a confirmed PDU and the request is either answered
         *    or timed out.
         * @throws IOException
         *    if the message could not be send.
         */
        void Send(PDU pdu, ITarget target, ITransportMapping<IAddress> transport,
                         object userHandle,
                         ResponseListener listener);

        /**
         * Cancels an asynchronous request. Any asynchronous request must be canceled
         * when the supplied response listener is being called, even if the
         * <code>ResponseEvent</code> indicates an error.
         * @param request
         *    a request PDU as sent via {@link #send(PDU pdu, Target target,
         *    Object userHandle, ResponseListener listener)} or any .
         * @param listener
         *    a ResponseListener instance.
         */
        void Cancel(PDU request, ResponseListener listener);
    }
}
