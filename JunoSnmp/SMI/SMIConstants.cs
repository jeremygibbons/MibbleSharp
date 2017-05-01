// <copyright file="SMIConstants.cs" company="None">
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

namespace JunoSnmp.SMI
{
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <c>SMIConstants</c> defines the tag values for SMI syntax types.
    /// </summary>
    public static class SMIConstants
    {
        public const int SyntaxInteger = BER.Asn1Integer;
        public const int SyntaxOctetString= BER.Asn1OctetString;
        public const int SyntaxNull = BER.Asn1Null;
        public const int SyntaxObjectIdentifier = BER.Asn1ObjectIdentifier;

        public const int SyntaxIpAddress = BER.IPADDRESS;
        public const int SyntaxInteger32 = BER.Asn1Integer;
        public const int SyntaxCounter32 = BER.COUNTER32;
        public const int SyntaxGauge32 = BER.GAUGE32;
        public const int SyntaxUnsignedInteger32 = BER.GAUGE32;
        public const int SyntaxTimeTicks = BER.TIMETICKS;
        public const int SyntaxOpaque = BER.OPAQUE;
        public const int SyntaxCounter64 = BER.COUNTER64;

        public const int SyntaxBits = SyntaxOctetString;

        public const int ExceptionNoSuchObject = BER.NoSuchObject;
        public const int ExceptionNoSuchInstance = BER.NoSuchInstance;
        public const int ExceptionEndOfMibView = BER.EndOfMibView;
    }
}
