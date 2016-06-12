// <copyright file="SnmpSettingsConstants.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleBrowser.SnmpSettings
{
    /// <summary>
    /// The constants and enums that appear in Snmp settings
    /// </summary>
    class SnmpSettingsConstants
    {
        /// <summary>
        /// The version of the Snmp protocol
        /// </summary>
        /// <remarks>
        /// Versions v2 (RFC 1441) and v2u (RFC 1909) are not supported
        /// </remarks>
        public enum SNMPVersion
        {
            v1 = 0,
            v2c,
            v3
        }

        /// <summary>
        /// The various Snmp v3 security levels, per RFC 3411 item 3.4.3
        /// </summary>
        public enum v3SecurityLevel
        {
            NoAuthNoPriv = 0,
            AuthNoPriv,
            AuthPriv
        }

        /// <summary>
        /// The various SNMPv3 authentication protocols.
        /// MD5 and SHA1 usage are specified in RFC 3414.
        /// SHA2 usage is specified in RFC 7860
        /// </summary>
        public enum v3AuthProtocol
        {
            /// <summary>
            /// No authentication is used. This is the default value
            /// </summary>
            None = 0,

            /// <summary>
            /// Use HMAC-MD5-96, per RFC 3414
            /// </summary>
            MD5,

            /// <summary>
            /// Use HMAC-SHA-96, per RFC 3414
            /// </summary>
            SHA1,

            /// <summary>
            /// Use usmHMAC128SHA224AuthProtocol per RFC 7860.
            /// Uses SHA-224 and truncates the output to 128 bits
            /// </summary>
            SHA2_224,

            /// <summary>
            /// Use usmHMAC192SHA256AuthProtocol per RFC 7860.
            /// Uses SHA-256 and truncates the output to 192 bits
            /// </summary>
            SHA2_256,

            /// <summary>
            /// Use usmHMAC256SHA384AuthProtocol per RFC 7860.
            /// Uses SHA-384 and truncates the output to 256 bits
            /// </summary>
            SHA2_384,

            /// <summary>
            /// Use usmHMAC384SHA512AuthProtocol per RFC 7860.
            /// Uses SHA-512 and truncates the output to 384 bits
            /// </summary>
            SHA2_512
        }

        /// <summary>
        /// 
        /// </summary>
        public enum v3PrivProtocol
        {
            /// <summary>
            /// No privacy protocol
            /// </summary>
            None = 0,

            /// <summary>
            /// Use usmDESPrivProtocol, i.e. the standard CBC-DES encryption,
            /// per RFC 3414.
            /// </summary>
            DES,

            /// <summary>
            /// USe usmAesCfb128PrivProtocol, i.e. AES-128 encryption,
            /// per RFC 3826
            /// </summary>
            AES_128,

            /// <summary>
            /// Use cusm3DES168PrivProtocol, i.e. a Cisco-specific approach to
            /// support 3-DES for SNMPv3
            /// </summary>
            TripleDES,

            /// <summary>
            /// Use cusmAESCfb192PrivProtocol, i.e. a Cisco-specific approach to
            /// support AES 192 for SNMPv3
            /// </summary>
            AES_192,

            /// <summary>
            /// Use cusmAESCfb256PrivProtocol, i.e. a Cisco-specific approach to
            /// support AES 256 for SNMPv3
            /// </summary
            AES_256
        }
    }
}
