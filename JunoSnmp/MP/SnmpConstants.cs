// <copyright file="SnmpConstants.cs" company="None">
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

namespace JunoSnmp.MP
{
    using System;
    using JunoSnmp.SMI;

    /// <summary>
    /// The <code>SnmpConstants</code> class holds constants, ObjectIDs and
    /// Message strings used within the library.
    /// </summary>
    public sealed class SnmpConstants
    {
        public static readonly int DEFAULT_COMMAND_RESPONDER_PORT = 161;
        public static readonly int DEFAULT_NOTIFICATION_RECEIVER_PORT = 162;

        public static readonly int MIN_PDU_LENGTH = 484;

        public const int MILLISECOND_TO_NANOSECOND = 1000000;
        public const int HUNDREDTHS_TO_NANOSECOND = 10000000;

        public const int version1 = 0;
        public const int version2c = 1;
        public const int version3 = 3;

        // SNMP error conditions defined (indirectly) by the SNMP standards:
        /** Command responders did not respond within specified timeout interval. */
        public static readonly int SNMP_ERROR_TIMEOUT = -1;
        /** OIDs returned from a GETNEXT or GETBULK are less or equal than the requested one (which is not allowed by SNMP). */
        public static readonly int SNMP_ERROR_LEXICOGRAPHIC_ORDER = -2;
        /** A unresolvable REPORT message was received while processing a request. */
        public static readonly int SNMP_ERROR_REPORT = -3;
        /** An IOException occurred during request processing. */
        public static readonly int SNMP_ERROR_IO = -4;

        // SNMP error codes defined by the protocol:
        public static readonly int SNMP_ERROR_SUCCESS = 0;
        public static readonly int SNMP_ERROR_TOO_BIG = 1;
        public static readonly int SNMP_ERROR_NO_SUCH_NAME = 2;
        public static readonly int SNMP_ERROR_BAD_VALUE = 3;
        public static readonly int SNMP_ERROR_READ_ONLY = 4;
        public static readonly int SNMP_ERROR_GENERAL_ERROR = 5;
        public static readonly int SNMP_ERROR_NO_ACCESS = 6;
        public static readonly int SNMP_ERROR_WRONG_TYPE = 7;
        public static readonly int SNMP_ERROR_WRONG_LENGTH = 8;
        public static readonly int SNMP_ERROR_WRONG_ENCODING = 9;
        public static readonly int SNMP_ERROR_WRONG_VALUE = 10;
        public static readonly int SNMP_ERROR_NO_CREATION = 11;
        public static readonly int SNMP_ERROR_INCONSISTENT_VALUE = 12;
        public static readonly int SNMP_ERROR_RESOURCE_UNAVAILABLE = 13;
        public static readonly int SNMP_ERROR_COMMIT_FAILED = 14;
        public static readonly int SNMP_ERROR_UNDO_FAILED = 15;
        public static readonly int SNMP_ERROR_AUTHORIZATION_ERROR = 16;
        public static readonly int SNMP_ERROR_NOT_WRITEABLE = 17;
        public static readonly int SNMP_ERROR_INCONSISTENT_NAME = 18;

        public const int SNMP_MP_OK = 0;
        public const int SNMP_MP_ERROR = -1400;
        public const int SNMP_MP_UNSUPPORTED_SECURITY_MODEL = -1402;
        public const int SNMP_MP_NOT_IN_TIME_WINDOW = -1403;
        public const int SNMP_MP_DOUBLED_MESSAGE = -1404;
        public const int SNMP_MP_INVALID_MESSAGE = -1405;
        public const int SNMP_MP_INVALID_ENGINEID = -1406;
        public const int SNMP_MP_NOT_INITIALIZED = -1407;
        public const int SNMP_MP_PARSE_ERROR = -1408;
        public const int SNMP_MP_UNKNOWN_MSGID = -1409;
        public const int SNMP_MP_MATCH_ERROR = -1410;
        public const int SNMP_MP_COMMUNITY_ERROR = -1411;
        public const int SNMP_MP_WRONG_USER_NAME = -1412;
        public const int SNMP_MP_BUILD_ERROR = -1413;
        public const int SNMP_MP_USM_ERROR = -1414;
        public const int SNMP_MP_UNKNOWN_PDU_HANDLERS = -1415;
        public const int SNMP_MP_UNAVAILABLE_CONTEXT = -1416;
        public const int SNMP_MP_UNKNOWN_CONTEXT = -1417;
        public const int SNMP_MP_REPORT_SENT = -1418;

        public const int SNMPv1v2c_CSM_OK = 0;
        public const int SNMPv1v2c_CSM_BAD_COMMUNITY_NAME = 1501;
        public const int SNMPv1v2c_CSM_BAD_COMMUNITY_USE = 1502;


        public const int SNMPv3_USM_OK = 0;
        public const int SNMPv3_USM_ERROR = 1401;
        public const int SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL = 1403;
        public const int SNMPv3_USM_UNKNOWN_SECURITY_NAME = 1404;
        public const int SNMPv3_USM_ENCRYPTION_ERROR = 1405;
        public const int SNMPv3_USM_DECRYPTION_ERROR = 1406;
        public const int SNMPv3_USM_AUTHENTICATION_ERROR = 1407;
        public const int SNMPv3_USM_AUTHENTICATION_FAILURE = 1408;
        public const int SNMPv3_USM_PARSE_ERROR = 1409;
        public const int SNMPv3_USM_UNKNOWN_ENGINEID = 1410;
        public const int SNMPv3_USM_NOT_IN_TIME_WINDOW = 1411;
        public const int SNMPv3_USM_UNSUPPORTED_AUTHPROTOCOL = 1412;
        public const int SNMPv3_USM_UNSUPPORTED_PRIVPROTOCOL = 1413;
        public const int SNMPv3_USM_ADDRESS_ERROR = 1414;
        public const int SNMPv3_USM_ENGINE_ID_TOO_LONG = 1415;
        public const int SNMPv3_USM_SECURITY_NAME_TOO_LONG = 1416;

        public const int SNMPv3_TSM_OK = 0;
        public const int SNMPv3_TSM_UNKNOWN_PREFIXES = 1601;
        public const int SNMPv3_TSM_INVALID_CACHES = 1602;
        public const int SNMPv3_TSM_INADEQUATE_SECURITY_LEVELS = 1603;

        public const int SNMP_MD_OK = 0;
        public const int SNMP_MD_ERROR = 1701;
        public const int SNMP_MD_UNSUPPORTED_MP_MODEL = 1702;
        public const int SNMP_MD_UNSUPPORTED_ADDRESS_CLASS = 1703;
        public const int SNMP_MD_UNSUPPORTED_SNMP_VERSION = 1704;


        // USM security protocol OIDs
        public static readonly OID usmNoAuthProtocol =
      new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 1, 1 });
        public static readonly OID usmHMACMD5AuthProtocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 1, 2 });
        public static readonly OID usmHMACSHAAuthProtocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 1, 3 });
        public static readonly OID usmNoPrivProtocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 2, 1 });
        public static readonly OID usmDESPrivProtocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 2, 2 });
        public static readonly OID usm3DESEDEPrivProtocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 2, 3 });
        public static readonly OID usmAesCfb128Protocol =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 1, 2, 4 });

        // SNMP4J security protocol OIDs
        public static readonly OID oosnmpUsmAesCfb192Protocol =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 2, 2, 1, 1, 1 });
        public static readonly OID oosnmpUsmAesCfb256Protocol =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 2, 2, 1, 1, 2 });
        public static readonly OID oosnmpUsmAesCfb192ProtocolWith3DESKeyExtension =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 2, 2, 1, 2, 1 });
        public static readonly OID oosnmpUsmAesCfb256ProtocolWith3DESKeyExtension =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 2, 2, 1, 2, 2 });

        public static readonly OID usmStatsUnsupportedSecLevels =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 1, 0 });
        public static readonly OID usmStatsNotInTimeWindows =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 2, 0 });
        public static readonly OID usmStatsUnknownUserNames =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 3, 0 });
        public static readonly OID usmStatsUnknownEngineIDs =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 4, 0 });
        public static readonly OID usmStatsWrongDigests =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 5, 0 });
        public static readonly OID usmStatsDecryptionErrors =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 15, 1, 1, 6, 0 });

        public static readonly OID snmpEngineID =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 10, 2, 1, 1, 0 });

        public static readonly OID snmpUnknownSecurityModels =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 1, 0 });
        public static readonly OID snmpInvalidMsgs =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 2, 0 });
        public static readonly OID snmpUnknownPDUHandlers =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 11, 2, 1, 3, 0 });

        // SNMP counters (obsoleted counters are not listed)
        public static readonly OID snmpInPkts =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 1, 0 });
        public static readonly OID snmpInBadVersions =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 3, 0 });
        public static readonly OID snmpInBadCommunityNames =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 4, 0 });
        public static readonly OID snmpInBadCommunityUses =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 5, 0 });
        public static readonly OID snmpInASNParseErrs =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 6, 0 });
        public static readonly OID snmpSilentDrops =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 31, 0 });
        public static readonly OID snmpProxyDrops =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 11, 32, 0 });

        public static readonly OID snmpTrapOID =
           new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 4, 1, 0 });
        public static readonly OID snmpTrapEnterprise =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 4, 3, 0 });

        // generic trap prefix
        public static readonly OID snmpTraps =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5 });
        // standard traps
        public static readonly OID coldStart =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5, 1 });
        public static readonly OID warmStart =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5, 2 });
        public static readonly OID authenticationFailure =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5, 5 });
        public static readonly OID linkDown =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5, 3 });
        public static readonly OID linkUp =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 5, 4 });

        // most important system group OIDs
        public static readonly OID sysDescr =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 1, 0 });
        public static readonly OID sysObjectID =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 2, 0 });
        public static readonly OID sysUpTime =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 3, 0 });
        public static readonly OID sysContact =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 4, 0 });
        public static readonly OID sysName =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 5, 0 });
        public static readonly OID sysLocation =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 6, 0 });
        public static readonly OID sysServices =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 7, 0 });
        public static readonly OID sysOREntry =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 1, 9, 1 });

        // contexts
        public static readonly OID snmpUnavailableContexts =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 12, 1, 4, 0 });
        public static readonly OID snmpUnknownContexts =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 12, 1, 5, 0 });

        // coexistance
        public static readonly OID snmpTrapAddress =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 18, 1, 3, 0 });
        public static readonly OID snmpTrapCommunity =
          new OID(new long[] { 1, 3, 6, 1, 6, 3, 18, 1, 4, 0 });

        public static readonly OID zeroDotZero = new OID(new long[] { 0, 0 });

        // SNMP-TSM-MIB
        public static readonly OID snmpTsmInvalidCaches =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 190, 1, 1, 1, 0 });
        public static readonly OID snmpTsmInadequateSecurityLevels =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 190, 1, 1, 2, 0 });
        public static readonly OID snmpTsmUnknownPrefixes =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 190, 1, 1, 3, 0 });
        public static readonly OID snmpTsmInvalidPrefixes =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 190, 1, 1, 4, 0 });
        public static readonly OID snmpTsmConfigurationUsePrefix =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 190, 1, 2, 1, 0 });

        // SNMP-TLS-TM-MIB
        public static readonly OID snmpTlstmSessionOpens =
        new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 1, 0 });
        public static readonly OID snmpTlstmSessionClientCloses =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 2, 0 });
        public static readonly OID snmpTlstmSessionOpenErrors =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 3, 0 });
        public static readonly OID snmpTlstmSessionAccepts =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 4, 0 });
        public static readonly OID snmpTlstmSessionServerCloses =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 5, 0 });
        public static readonly OID snmpTlstmSessionNoSessions =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 6, 0 });
        public static readonly OID snmpTlstmSessionInvalidClientCertificates =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 7, 0 });
        public static readonly OID snmpTlstmSessionUnknownServerCertificate =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 8, 0 });
        public static readonly OID snmpTlstmSessionInvalidServerCertificates =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 9, 0 });
        public static readonly OID snmpTlstmSessionInvalidCaches =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 198, 2, 1, 10, 0 });

        // SNMP-SSH-TM-MIB
        public static readonly OID snmpSshtmSessionOpens =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 1, 0 });
        public static readonly OID snmpSshtmSessionCloses =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 2, 0 });
        public static readonly OID snmpSshtmSessionOpenErrors =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 3, 0 });
        public static readonly OID snmpSshtmSessionUserAuthFailures =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 4, 0 });
        public static readonly OID snmpSshtmSessionNoChannels =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 5, 0 });
        public static readonly OID snmpSshtmSessionNoSubsystems =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 6, 0 });
        public static readonly OID snmpSshtmSessionNoSessions =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 7, 0 });
        public static readonly OID snmpSshtmSessionInvalidCaches =
          new OID(new long[] { 1, 3, 6, 1, 2, 1, 189, 1, 1, 8, 0 });

        // SNMP4J-STATISTICS-MIB
        /**
         * The total number of requests that timed out (Counter32).
         */
        public static readonly OID snmp4jStatsRequestTimeouts =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 1, 0 });
        /**
         * The total number of retries sent on behalf of
         * requests. The first message, thus the request
         * itself is not counted (Counter32).
         */
        public static readonly OID snmp4jStatsRequestRetries =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 2, 0 });
        /**
         * The total number of milliseconds this SNMP
         * entity spend waiting for responses on its own
         * requests (Counter64).
         */
        public static readonly OID snmp4jStatsRequestWaitTime =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 3, 0 });
        /**
         * The number of milliseconds a successful request took
         * from sending the request to receiving the corresponding response
         * with the same msgID. Note, for community based SNMP version, only
         * the same request ID is used to correlate request and response.
         * Thus, only for SNMPv3 the counter can distinguish which retry
         * has been successfully responded.
         */
        public static readonly OID snmp4jStatsRequestRuntime =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 4, 0 });

        /**
         * The total number of requests that timed out for this target (Counter32).
         */
        public static readonly OID snmp4jStatsReqTableTimeouts =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 10, 3, 1, 4 });
        /**
         * The total number of retries sent on behalf of
         * requests to this target. The first message, thus the request
         * itself is not counted.
         */
        public static readonly OID snmp4jStatsReqTableRetries =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 10, 3, 1, 5 });
        /**
         * The total number of milliseconds this SNMP
         * entity spend waiting for responses on its own
         * requests to this target.
         */
        public static readonly OID snmp4jStatsReqTableWaitTime =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 10, 3, 1, 6 });
        /**
         * The number of milliseconds a successful request took
         * from sending the request to receiving the corresponding response
         * with the same msgID for this target.
         * Note, for community based SNMP version, only the same request ID
         * is used to correlate request and response. Thus, only for SNMPv3
         * the counter can distinguish which retry has been successfully responded.
         */
        public static readonly OID snmp4jStatsReqTableRuntime =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 1, 10, 3, 1, 7 });


        /**
         * The number of response processings that ended
         * due to an internal timeout before that maximum
         * number of response variables (GETBULK) has been
         * reached. For other request types than GETBULK,
         * an internal timeout would return a SNMP error
         * (e.g. genErr) to the command sender.
         */
        public static readonly OID snmp4jStatsResponseTimeouts =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 2, 1, 0 });
        /**
         * The total number of retries ignored by the command
         * responder while processing requests.
         */
        public static readonly OID snmp4jStatsResponseIgnoredRetries =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 2, 2, 0 });
        /**
         * The total number of milliseconds the command
         * responder took to process a request.
         */
        public static readonly OID snmp4jStatsResponseProcessTime =
            new OID(new long[] { 1, 3, 6, 1, 4, 1, 4976, 10, 1, 1, 4, 1, 2, 3, 0 });

        // SNMP framework
        public static readonly OID snmpSetSerialNo =
            new OID(new long[] { 1, 3, 6, 1, 6, 3, 1, 1, 6, 1, 0 });

        public static readonly string[] SNMP_TP_ERROR_MESSAGES = {
      "Request timed out"
  };

        public static readonly string[] SNMP_ERROR_MESSAGES = {
      "Success",
      "PDU encoding too big",
      "No such name",
      "Bad value",
      "Variable is read-only",
      "General variable binding error",
      "No access",
      "Wrong type",
      "Variable binding data with incorrect length",
      "Variable binding data with wrong encoding",
      "Wrong value",
      "Unable to create object",
      "Inconsistent value",
      "Resource unavailable",
      "Commit failed",
      "Undo failed",
      "Authorization error",
      "Not writable",
      "Inconsistent naming used"
  };
        public static string[][] MD_ERROR_MESSAGES = new string[][] {
      new string [] { string.Empty + SNMP_MD_ERROR, "Message Dispatcher error" },
      new string [] { string.Empty + SNMP_MD_UNSUPPORTED_MP_MODEL, "Unsupported message processing model" },
      new string [] { string.Empty + SNMP_MD_UNSUPPORTED_ADDRESS_CLASS, "Unsupported address class" },
      new string [] { string.Empty + SNMP_MD_UNSUPPORTED_SNMP_VERSION, "Unsupported address class" }
  };

        public static string[][] MP_ERROR_MESSAGES = new string[][] {
      new string [] { string.Empty + SNMP_MP_ERROR, "MP error" },
      new string [] { string.Empty + SNMP_MP_UNSUPPORTED_SECURITY_MODEL, "Unsupported security model" },
      new string [] { string.Empty + SNMP_MP_NOT_IN_TIME_WINDOW, "Message not in time window"},
      new string [] { string.Empty + SNMP_MP_DOUBLED_MESSAGE, "Doubled message" },
      new string [] { string.Empty + SNMP_MP_INVALID_MESSAGE, "Invalid message" },
      new string [] { string.Empty + SNMP_MP_INVALID_ENGINEID, "Invalid engine ID" },
      new string [] { string.Empty + SNMP_MP_NOT_INITIALIZED, "MP not initialized" },
      new string [] { string.Empty + SNMP_MP_PARSE_ERROR, "MP parse error"},
      new string [] { string.Empty + SNMP_MP_UNKNOWN_MSGID, "Unknown message ID"},
      new string [] { string.Empty + SNMP_MP_MATCH_ERROR, "MP match error"},
      new string [] { string.Empty + SNMP_MP_COMMUNITY_ERROR, "MP community error"},
      new string [] { string.Empty + SNMP_MP_WRONG_USER_NAME, "Wrong user name"},
      new string [] { string.Empty + SNMP_MP_BUILD_ERROR, "MP build error"},
      new string [] { string.Empty + SNMP_MP_USM_ERROR, "USM error"},
      new string [] { string.Empty + SNMP_MP_UNKNOWN_PDU_HANDLERS, "Unknown PDU handles"},
      new string [] { string.Empty + SNMP_MP_UNAVAILABLE_CONTEXT, "Unavailable context"},
      new string [] { string.Empty + SNMP_MP_UNKNOWN_CONTEXT, "Unknown context"},
      new string [] { string.Empty + SNMP_MP_REPORT_SENT, "Report sent"}
  };

        public static String[][] USM_ERROR_MESSAGES = new string[][]
        {
      new string [] { string.Empty + SNMPv3_USM_OK, "USM OK" },
      new string [] { string.Empty + SNMPv3_USM_ERROR, "USM error" },
      new string [] { string.Empty + SNMPv3_USM_UNSUPPORTED_SECURITY_LEVEL, "Unsupported security level" },
      new string [] { string.Empty + SNMPv3_USM_UNKNOWN_SECURITY_NAME, "Unknown security name"},
      new string [] { string.Empty + SNMPv3_USM_ENCRYPTION_ERROR, "Encryption error"},
      new string [] { string.Empty + SNMPv3_USM_DECRYPTION_ERROR, "Decryption error"},
      new string [] { string.Empty + SNMPv3_USM_AUTHENTICATION_ERROR, "Authentication error"},
      new string [] { string.Empty + SNMPv3_USM_AUTHENTICATION_FAILURE, "Authentication failure"},
      new string [] { string.Empty + SNMPv3_USM_PARSE_ERROR, "USM parse error"},
      new string [] { string.Empty + SNMPv3_USM_UNKNOWN_ENGINEID, "Unknown engine ID"},
      new string [] { string.Empty + SNMPv3_USM_NOT_IN_TIME_WINDOW, "Not in time window"},
      new string [] { string.Empty + SNMPv3_USM_UNSUPPORTED_AUTHPROTOCOL, "Unsupported authentication protocol"},
      new string [] { string.Empty + SNMPv3_USM_UNSUPPORTED_PRIVPROTOCOL, "Unsupported privacy protocol"},
      new string [] { string.Empty + SNMPv3_USM_ADDRESS_ERROR, "Address error"},
      new string [] { string.Empty + SNMPv3_USM_ENGINE_ID_TOO_LONG, "Engine ID too long"},
      new string [] { string.Empty + SNMPv3_USM_SECURITY_NAME_TOO_LONG, "Security name too long"}
  };

        public static string MpErrorMessage(int status)
        {
            string s = string.Empty + status;
            foreach (string[] MP_ERROR_MESSAGE in MP_ERROR_MESSAGES)
            {
                if (s.Equals(MP_ERROR_MESSAGE[0]))
                {
                    return MP_ERROR_MESSAGE[1];
                }
            }

            // MPv3 uses USM so scan the USM error messages too:
            s = SnmpConstants.ErrorMessage(status, USM_ERROR_MESSAGES);
            if (s == null)
            {
                s = SnmpConstants.ErrorMessage(status, MD_ERROR_MESSAGES);
                return (s == null) ? "" + status : s;
            }
            return s;
        }

        public static string usmErrorMessage(int status)
        {
            string s = SnmpConstants.ErrorMessage(status, USM_ERROR_MESSAGES);
            return (s == null) ? string.Empty + status : s;
        }

        private static string ErrorMessage(int status, string[][] errorMessages)
        {
            string s = string.Empty + status;
            foreach (string[] errorMessage in errorMessages)
            {
                if (s.Equals(errorMessage[0]))
                {
                    return errorMessage[1];
                }
            }
            return null;
        }

        /**
         * Gets the generic trap ID from a notification OID.
         * @param oid
         *    an OID.
         * @return
         *    -1 if the supplied OID is not a generic trap, otherwise a value >= 0
         *    will be returned that denotes the generic trap ID.
         */
        public static long GetGenericTrapID(OID oid)
        {
            if ((oid == null) || (oid.Size != snmpTraps.Size + 1))
            {
                return -1;
            }
            if (oid.LeftMostCompare(snmpTraps.Size, snmpTraps) == 0)
            {
                return oid[oid.Size - 1] - 1;
            }
            return -1;
        }

        public static OID GetTrapOID(OID enterprise, int genericID, int specificID)
        {
            OID oid;

            if (genericID != 6)
            {
                oid = new OID(snmpTraps);
                oid.Append(genericID + 1);
            }
            else
            {
                oid = new OID(enterprise);
                oid.Append(0);
                oid.Append(specificID);
            }

            return oid;
        }
    }
}
