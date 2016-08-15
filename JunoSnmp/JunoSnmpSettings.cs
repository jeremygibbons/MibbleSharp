// <copyright file="JunoSnmpSettings.cs" company="None">
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
    using System;
    using System.Threading.Tasks;
    using JunoSnmp.MP;
    using JunoSnmp.Util;

    /// <summary>
    /// The <c>JunoSnmpSettings</c> class implements a central configuration
    /// class of the JunoSnmp framework.As a rule of thumb, changes to the default
    /// configuration should be made before any other classes of the JunoSnmp API are
    /// instantiated or referenced by the application code.
    /// </summary>
    public sealed class JunoSnmpSettings
    {
        /// <summary>
        /// The enterprise ID of JunoSnmp.
        /// </summary>
        public static readonly int JunoSnmpEnterpriseID = 48056;

        /// <summary>
        /// By default allow worth of ~2MB memory for engine ID cache.
        /// </summary>
        private static int maxEngineIdCacheSize = 50000;

        /// <summary>
        /// Specifies whether JunoSnmp can be extended by custom implementations of
        /// security protocols, transport mappings, address types, SMI syntaxes, etc.
        /// through property files defined via System properties.
        /// If set to <c>false</c> all classes JunoSnmp is aware of will be
        /// used hard coded which speeds up initialization and is required to use
        /// JunoSnmp in a secure environment where System properties are not available
        /// </summary>
        private static bool extensibilityEnabled = false;

        /// <summary>
        /// By default JunoSnmp catches runtime exceptions at thread
        /// boundaries of API controlled threads. In JunoSnmp such a thread runs in each
        /// <see cref="TransportMapping"/> and in each <see cref="Snmp"/> session object. To ensure
        /// robust runtime behavior, unexpected runtime exceptions are caught and
        /// logged. If you need to localize and debug such exceptions then set this
        /// value to <c>true</c>.
        /// </summary>
        private static volatile bool forwardRuntimeExceptions = false;

        /// <summary>
        /// By default JunoSnmp uses <see cref="Task"/> instances to run
        /// concurrent tasks. For environments with restricted thread management,
        /// a custom thread factory can be used.
        /// </summary>
        private static TaskFactory taskFactory = Task.Factory;

        /// <summary>
        /// By default JunoSnmp uses <see cref="System.Threading.Timer"/> instances to run
        /// timed tasks. For environments with restricted thread management,
        /// a custom timer factory can be used.
        /// </summary>
        private static TimerFactory timerFactory = new DefaultTimerFactory();

        /// <summary>
        /// By default SNMP4J uses the <see cref="SimpleOIDTextFormat"/> to convert
        /// <see cref="OID"/>s to/from a textual representation.
        /// </summary>
        private static OIDTextFormat oidTextFormat = new SimpleOIDTextFormat();

        /// <summary>
        /// By default SNMP4J uses the <see cref="SimpleVariableTextFormat"/> to convert
        /// <see cref="VariableBinding"/>s to/from a textual representation.
        /// </summary>        
        private static VariableTextFormat variableTextFormat =
            new SimpleVariableTextFormat();

        /// <summary>
        /// The default Thread join timeout, used for example by the
        /// <see cref="DefaultThreadFactory"/>, defines the maximum time to wait for a
        /// Thread running a worker task to end that task (end join the main thread
        /// if that Thread has been exclusively used for that task). The default value
        /// is 60 seconds (1 min.).
        /// </summary>
        private static long threadJoinTimeout = 60000;

        /// <summary>
        /// This setting can used to be compatible with some faulty SNMP implementations
        /// which send Counter64 objects with SNMP v1 PDUs.
        /// </summary>
        private static bool allowSNMPv2InV1 = false;

        /// <summary>
        /// Suppress GETBULK sending. Instead use GETNEXT. This option may be useful in environments
        /// with buggy devices which support SNMP v2c or v3 but do not support the mandatory GETBULK PDU type.
        /// </summary>
        private static bool noGetBulk = false;

        /// <summary><para>
        /// This flag enables or disables (default) whether REPORT PDUs should be send by
        /// a command responder with securityLevel <see cref="JunoSnmp.Security.SecurityLevel.NoAuthNoPriv"/>
        /// if otherwise the command generator would not able to receive the <see cref="PDU.REPORT"/>.
        /// RFC 3412 §7.6.2.3 requires that the securityLevel is the same as in the confirmed class
        /// PDU if not explicitly otherwise specified.
        /// </para><para>
        /// For <see cref="SnmpConstants.usmStatsUnsupportedSecLevels"/> reports, this would
        /// always render the command responder unable to return the report.
        /// </para><para>
        /// Setting this flag to <see cref="ReportSecurityLevelOption.noAuthNoPrivIfNeeded"/>
        /// reactivates the behavior of SNMP4J prior to v2.2 which sends out the reports with
        /// <see cref="JunoSnmp.Security.SecurityLevel.NoAuthNoPriv"/> if otherwise, the report would
        /// not be sent out.
        /// </para></summary>
        private static ReportSecurityLevelOption reportSecurityLevelStrategy = ReportSecurityLevelOption.standard;

        /// <summary>
        /// The enterprise ID that is used to build SNMP engine IDs and other enterprise
        /// specific OIDs.This value should be changed by API users from other enterprises (companies or
        /// organizations).
        /// </summary>
        private static int enterpriseID = JunoSnmpEnterpriseID;

        /// <summary>
        /// The snmp4jStatistics value defines the level of statistic values that are collected
        /// in extension to those collected for the SNMP standard.
        /// </summary>
        private static JunoSnmpStatistics junoSnmpStatistics = JunoSnmpStatistics.basic;

        /// <summary>
        /// The checkUsmUserPassphraseLength specifies whether the minimum USM passphrase length should be
        /// checked when creating UsmUser instances(RFC3414 §11.2). Default is yes.
        /// </summary>
        private static bool checkUsmUserPassphraseLength = true;

        /// <summary>
        /// Specifies the how the security level of retry requests after a REPORT PDU is
        /// set.
        /// </summary>
        public enum ReportSecurityLevelOption
        {
            /// <summary>
            /// This is the SNMP4J &lt; 2.2.0 default strategy. If the report receiver would not
            /// be able to process the REPORT, a lesser security level is used.
            /// </summary>
            noAuthNoPrivIfNeeded,

            /// <summary>
            /// The standard report strategy is conforming to RFC 3412 and 3414. Security level noAuthNoPriv is allowed
            /// for engine ID discovery and wrong username reports.
            /// </summary>
            standard,

            /// <summary>
            /// Never use noAuthNoPriv security level for reports.
            /// </summary>
            neverNoAuthNoPriv
        }

        /// <summary>
        /// Enumeration of values for the JunoSnmp statistics collection level
        /// </summary>
        public enum JunoSnmpStatistics
        {
            /// <summary>
            /// Do not collect any SNMP4J specific statistics.
            /// </summary>
            none,

            /// <summary>
            /// Collect only basic statistics as defined by the junoSnmpStatsBasicGroup.
            /// </summary>
            basic,

            /// <summary>
            /// Collect extended statistics that include per target stats.
            /// </summary>
            extended
        }

        /// <summary><para>
        /// Gets or sets a value indicating whether the extensibility feature of JunoSnmp is enabled.
        /// When enabled, JunoSnmp checks certain property files that describe which transport
        /// mappings, address types, SMI syntaxes, security protocols, etc.should be
        /// supported by JunoSnmp.
        /// </para><para>
        /// By default, the extensibility feature is disabled which provides a faster
        /// startup and since no system properties are read, it ensures that SNMP4J
        /// can be used also in secure environments like applets.
        /// </para></summary>
        public static bool ExtensibilityEnabled
        {
            get
            {
                return JunoSnmpSettings.extensibilityEnabled;
            }

            set
            {
                JunoSnmpSettings.extensibilityEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to forward runtime exceptions.
        /// If <c>true</c>, runtime exceptions are thrown on thread boundaries
        /// controlled by JunoSnmp and related APIs
        /// </summary>
        /// <remarks>Default is false</remarks>
        public static bool ForwardRuntimeExceptions
        {
            get
            {
                return JunoSnmpSettings.forwardRuntimeExceptions;
            }

            set
            {
                JunoSnmpSettings.forwardRuntimeExceptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the TaskFactory used to create new threads
        /// </summary>
        /// <remarks>
        /// The default value is the default .Net TaskFactory obtained
        /// via Task.Factory
        /// </remarks>
        public static TaskFactory TaskFactory
        {
            get
            {
                return JunoSnmpSettings.taskFactory;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                JunoSnmpSettings.taskFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the timer factory.
        /// </summary>
        public static TimerFactory TimerFactory
        {
            get
            {
                return JunoSnmpSettings.timerFactory;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                JunoSnmpSettings.timerFactory = value;
            }
        }

        /// <summary>
        /// Gets or sets the OID text format for textual representation of OIDs.
        /// </summary>
        public static OIDTextFormat OIDTextFormat
        {
            get
            {
                return JunoSnmpSettings.oidTextFormat;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                JunoSnmpSettings.oidTextFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the variable text format for textual representation of variable
        /// bindings.
        /// </summary>
        public static VariableTextFormat VariableTextFormat
        {
            get
            {
                return JunoSnmpSettings.variableTextFormat;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                JunoSnmpSettings.variableTextFormat = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the thread join timeout used to join threads if no explicit timeout
        /// is set.
        /// </summary>
        /// <remarks>
        /// Timeout value is given in milliseconds
        /// </remarks>
        public static long ThreadJoinTimeout
        {
            get
            {
                return JunoSnmpSettings.threadJoinTimeout;
            }

            set
            {
                JunoSnmpSettings.threadJoinTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to disable SNMP Standard conformity. This
        /// provides for Counter64 usage in SNMP v1 PDUs and
        /// the ability to decode SNMP v2 traps in SNMP v1 version PDUs.
        /// </summary>
        /// <remarks>Default is false (do not allow non-compliant behaviors). This setting is meant 
        /// to help deal with buggy Snmp implementations which use v2 features in v1 PDUs
        /// </remarks>
        public static bool AllowSNMPv2InV1
        {
            get
            {
                return JunoSnmpSettings.allowSNMPv2InV1;
            }

            set
            {
                JunoSnmpSettings.allowSNMPv2InV1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the ReportSecurityLevel strategy
        /// </summary>
        public static ReportSecurityLevelOption ReportSecurityLevelStrategy
        {
            get
            {
                return JunoSnmpSettings.reportSecurityLevelStrategy;
            }

            set
            {
                JunoSnmpSettings.reportSecurityLevelStrategy = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to suppress use of GetBulk. Instead use GETNEXT.
        /// This option may be useful in environments with buggy devices which support SNMP v2c 
        /// or v3 but do not support the mandatory GETBULK PDU type.
        /// </summary>
        /// <remarks>Default is False (use of GetBulk is allowed)</remarks>
        public static bool NoGetBulk
        {
            get
            {
                return JunoSnmpSettings.noGetBulk;
            }

            set
            {
                JunoSnmpSettings.noGetBulk = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the enterprise OID used for creating SNMP engine IDs and other enterprise
        /// specific identifiers.
        /// </summary>
        /// <remarks>
        /// A JunoSnmp specific default value is used (48056), but this value should be changed 
        /// for other enterprises (i.e., companies or organizations).
        /// </remarks>
        public static int EnterpriseID
        {
            get
            {
                return JunoSnmpSettings.enterpriseID;
            }

            set
            {
                JunoSnmpSettings.enterpriseID = value;
            }
        }        
        
        /// <summary>
        /// Gets or sets the maximum number of engine IDs to be hold in the cache of the <see cref="MPv3"/>.
        /// A upper limit is necessary to avoid DoS attacks with unconfirmed SNMP v3 PDUs. 
        /// </summary>
        /// <remarks>
        /// The default value is 50,000, or about 2MB of cached data
        /// </remarks>
        public static int MaxEngineIdCacheSize
        {
            get
            {
                return JunoSnmpSettings.maxEngineIdCacheSize;
            }

            set
            {
                JunoSnmpSettings.maxEngineIdCacheSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the JunoSnmp statistics level.
        /// </summary>
        public static JunoSnmpStatistics JunoSnmpStatisticsLevel
        {
            get
            {
                return JunoSnmpSettings.junoSnmpStatistics;
            }

            set
            {
                JunoSnmpSettings.junoSnmpStatistics = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the minimum USM passphrase length should be
        /// checked when creating UsmUser instances(RFC3414 §11.2).
        /// </summary>
        /// <remarks>Default is True.</remarks>
        public static bool CheckUsmUserPassphraseLength
        {
            get
            {
                return JunoSnmpSettings.checkUsmUserPassphraseLength;
            }

            set
            {
                JunoSnmpSettings.checkUsmUserPassphraseLength = value;
            }
        }
    }
}
