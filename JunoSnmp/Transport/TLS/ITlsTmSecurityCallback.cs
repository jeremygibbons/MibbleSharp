// <copyright file="ITlsTmSecurityCallback.cs" company="None">
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

namespace JunoSnmp.Transport.TLS
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using JunoSnmp.SMI;

    /**
 * The <code>TlsTmSecurityCallback</code> is implemented by the
 * SnmpTlsMib (of SNMP4J-Agent), for example, to resolve (lookup) the
 * <code>tmSecurityName</code> for incoming requests.
 *
 * @author Frank Fock
 * @version 2.0
 * @since 2.0
 */
    public interface ITlsTmSecurityCallback<C> where C : X509Certificate2
    {

        /**
         * Gets the tmSecurityName (see RFC 5953) from the certificate chain
         * of the communication peer that needs to be authenticated.
         *
         * @param peerCertificateChain
         *    an array of {@link Certificate}s with the peer's own certificate
         *    first followed by any CA authorities.
         * @return
         *    the tmSecurityName as defined by RFC 5953.
         */
        OctetString GetSecurityName(C[] peerCertificateChain);

        /**
         * Check if the supplied peer end certificate is accepted as client.
         * @param peerEndCertificate
         *    a client Certificate instance to check acceptance for.
         * @return
         *    <tt>true</tt> if the certificate is accepted.
         */
        bool IsClientCertificateAccepted(C peerEndCertificate);

        /**
         * Check if the supplied peer certificate chain is accepted as server.
         * @param peerCertificateChain
         *    a server Certificate chain to check acceptance for.
         * @return
         *    <tt>true</tt> if the certificate chain is accepted.
         */
        bool IsServerCertificateAccepted(C[] peerCertificateChain);

        /**
         * Check if the supplied issuer certificate is accepted as server.
         * @param issuerCertificate
         *    an issuer Certificate instance to check acceptance for.
         * @return
         *    <tt>true</tt> if the certificate is accepted.
         */
        bool IsAcceptedIssuer(C issuerCertificate);

        /**
         * Gets the local certificate alias to be used for the supplied
         * target address.
         * @param targetAddress
         *    a target address or <tt>null</tt> if the default local
         *    certificate alias needs to be retrieved.
         * @return
         *    the requested local certificate alias, if known.
         *    Otherwise <tt>null</tt> is returned which could cause
         *    a protocol violation if the local key store contains more
         *    than one certificate.
         */
        string GetLocalCertificateAlias(IAddress targetAddress);

    }

}
