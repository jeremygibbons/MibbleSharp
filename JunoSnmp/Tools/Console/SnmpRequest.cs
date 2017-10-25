// <copyright file="SnmpRequest.cs" company="None">
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

namespace JunoSnmp.Tools.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JunoSnmp;
    using JunoSnmp.ASN1;
    using JunoSnmp.Event;
    using JunoSnmp.MP;
    using JunoSnmp.Security;
    using JunoSnmp.SMI;
    using JunoSnmp.Transport;
    using JunoSnmp.Util;
    using JunoSnmp.Util.Tree;

    /**
 * The SnmpRequest application is an example implementation of most of the
 * SNMP4J features. It can be used to send SNMP requests to a target or to
 * listen for traps/notifications and inform requests.
 *
 * @author Frank Fock
 * @version 1.9
 */
    public class SnmpRequest : CommandResponder, IPDUFactory
    {

        // initialize Java logging

        public static readonly int DEFAULT = 0;
        public static readonly int WALK = 1;
        public static readonly int LISTEN = 2;
        public static readonly int TABLE = 3;
        public static readonly int CVS_TABLE = 4;
        public static readonly int TIME_BASED_CVS_TABLE = 5;
        public static readonly int SNAPSHOT_CREATION = 6;
        public static readonly int SNAPSHOT_DUMP = 7;

        public ITarget Target { get; set; }
        public IAddress Address { get; set; }
        public OID AuthProtocol { get; set; }
        public OID PrivProtocol { get; set; }
        public OctetString PrivPassphrase { get; set; }
        public OctetString AuthPassphrase { get; set; }
        public OctetString Community { get; set; } = new OctetString("public");
        public OctetString AuthoritativeEngineID { get; set; }
        public OctetString ContextEngineID { get; set; }
        public OctetString ContextName { get; set; } = new OctetString();
        public OctetString SecurityName { get; set; } = new OctetString();
        public OctetString LocalEngineID { get; set; } = new OctetString(MPv3.CreateLocalEngineID());

        public TimeTicks SysUpTime { get; set; } = new TimeTicks(0);
        public OID TrapOID { get; set; } = SnmpConstants.coldStart;

        PDUv1 v1TrapPDU = new PDUv1();

        public int Version { get; set; } = SnmpConstants.version3;
        public int EngineBootCount { get; set; } = 0;
        public int Retries { get; set; } = 1;
        public int Timeout { get; set; } = 1000;
        public int PduType { get; set; } = PDU.GETNEXT;
        public int MaxRepetitions { get; set; } = 10;
        public int NonRepeaters { get; set; } = 0;
        public int MaxSizeResponsePDU { get; set; } = 65535;
        public List<VariableBinding> VarBindings { get; set; } = new List<VariableBinding>();
        string snapshotFile;

        protected int Operation { get; set; } = DEFAULT;

        int numDispatcherThreads = 2;

        bool useDenseTableOperation = false;

        // table options
        OID lowerBoundIndex, upperBoundIndex;


        public SnmpRequest(string[] args)
        {
            BER.CheckSequenceLengthFlag = false;

            // Set the default counter listener to return proper USM and MP error
            // counters.
            CounterSupport.Instance.AddCounterListener(new DefaultCounterListener());

            this.VarBindings.Add(new VariableBinding(new OID("1.3.6")));
            int paramStart = ParseArgs(args);

            if (paramStart >= args.Length)
            {
                PrintUsage();
                return;
            }
            else if (this.Operation != SNAPSHOT_DUMP)
            {
                CheckOptions();
                this.Address = GetAddress(args[paramStart++]);
                List<VariableBinding> vbs = GetVariableBindings(args, paramStart).ToList();
                CheckTrapVariables(vbs);
                if (vbs.Count > 0)
                {
                    this.VarBindings = vbs;
                }
            }
        }
        
        public bool UseDenseTableOperation
        {
            get => useDenseTableOperation;
        }

        public OID UpperBoundIndex
        {
            get => upperBoundIndex;
        }

        public int NumDispatcherThreads
        {
            get => numDispatcherThreads;
        }

        public OID LowerBoundIndex
        {
            get => lowerBoundIndex;
        }
        
        private void CheckOptions()
        {
            if ((this.Operation == WALK) &&
                ((this.PduType != PDU.GETBULK) && (this.PduType != PDU.GETNEXT)))
            {
                throw new ArgumentException(
                    "Walk operation is not supported for PDU type: " +
                    PDU.GetTypeString(this.PduType));
            }
            else if ((this.Operation == WALK) && (this.VarBindings.Count != 1))
            {
                throw new ArgumentException(
                    "There must be exactly one OID supplied for walk operations");
            }
            if ((this.PduType == PDU.V1TRAP) && (this.Version != SnmpConstants.version1))
            {
                throw new ArgumentException(
                    "V1TRAP PDU type is only available for SNMP version 1");
            }
        }

        private void CheckTrapVariables(List<VariableBinding> vbs)
        {
            if ((this.PduType == PDU.INFORM) ||
                (this.PduType == PDU.TRAP))
            {
                if ((vbs.Count == 0) ||
                    ((vbs.Count > 1) &&
                     (!(vbs[0]).Oid.Equals(SnmpConstants.sysUpTime))))
                {
                    vbs.Insert(0, new VariableBinding(SnmpConstants.sysUpTime, this.SysUpTime));
                }

                if ((vbs.Count == 1) ||
                    ((vbs.Count > 2) &&
                     (!(vbs[1]).Oid.Equals(SnmpConstants.snmpTrapOID))))
                {
                    vbs.Insert(1, new VariableBinding(SnmpConstants.snmpTrapOID, this.TrapOID));
                }
            }
        }

        //synchronized
        public void Listen()
        {
            AbstractTransportMapping<IAddress> transport;
            if (this.Address is TcpAddress tcpaddr)
            {
                transport = new DefaultTcpTransportMapping(tcpaddr);
            }
            else if (this.Address is UdpAddress udpaddr)
            {
                transport = new DefaultUdpTransportMapping(udpaddr);
            }
            else
            {
                throw new ArgumentException("Invalid transport");
            }

            ThreadPool threadPool =
                ThreadPool.create("DispatcherPool", numDispatcherThreads);
            IMessageDispatcher mtDispatcher =
                new MultiThreadedMessageDispatcher(threadPool, new MessageDispatcherImpl());

            // add message processing models
            mtDispatcher.AddMessageProcessingModel(new MPv1());
            mtDispatcher.AddMessageProcessingModel(new MPv2c());
            mtDispatcher.AddMessageProcessingModel(new MPv3(localEngineID.GetValue()));

            // add all security protocols
            SecurityProtocols.GetInstance().AddDefaultProtocols();
            SecurityProtocols.GetInstance().AddPrivacyProtocol(new Priv3DES());

            Snmp snmp = new Snmp(mtDispatcher, transport);
            if (this.Version == SnmpConstants.version3)
            {
                USM usm = new USM(SecurityProtocols.GetInstance(), this.LocalEngineID, 0);
                SecurityModels.Instance.AddSecurityModel(usm);
                // Add the configured user to the USM
                AddUsmUser(snmp);
            }
            else
            {
                CommunityTarget target = new CommunityTarget();
                target.Community = this.Community;
                this.Target = target;
            }

            snmp.addCommandResponder(this);

            transport.listen();
            Console.WriteLine("Listening on " + this.Address);

            try
            {
                this.Wait();
            }
            catch (InterruptedException ex)
            {
                Thread.currentThread().interrupt();
            }
        }

        private void AddUsmUser(Snmp snmp)
        {
            snmp.getUSM().addUser(this.SecurityName, new UsmUser(this.SecurityName,
                                                            this.AuthProtocol,
                                                            this.AuthPassphrase,
                                                            this.PrivProtocol,
                                                            this.PrivPassphrase));
        }

        private Snmp CreateSnmpSession()
        {
            AbstractTransportMapping<IAddress> transport;
            if (this.Address is TlsAddress)
            {
                transport = new TLSTM();
            }
            else if (this.Address is TcpAddress)
            {
                transport = new DefaultTcpTransportMapping();
            }
            else
            {
                transport = new DefaultUdpTransportMapping();
            }
            // Could save some CPU cycles:
            // transport.setAsyncMsgProcessingSupported(false);
            Snmp snmp = new Snmp(transport);
            MPv3 mpv3 = (MPv3)snmp.getMessageProcessingModel(MPv3.MPId);
            mpv3.LocalEngineID = this.LocalEngineID.GetValue();
            mpv3.setCurrentMsgID(MPv3.RandomMsgID(this.EngineBootCount));

            if (this.Version == SnmpConstants.version3)
            {
                USM usm = new USM(SecurityProtocols.GetInstance(),
                                  this.LocalEngineID,
                                  this.EngineBootCount);
                SecurityModels.Instance.AddSecurityModel(usm);
                AddUsmUser(snmp);
                SecurityModels.Instance.AddSecurityModel(
                          new TSM(this.LocalEngineID, false));
            }
            return snmp;
        }

        private ITarget CreateTarget()
        {
            if (this.Version == SnmpConstants.version3)
            {
                UserTarget target = new UserTarget();
                if (this.AuthPassphrase != null)
                {
                    if (this.PrivPassphrase != null)
                    {
                        target.SecurityLevel = SecurityLevel.AuthPriv;
                    }
                    else
                    {
                        target.SecurityLevel = SecurityLevel.AuthNoPriv;
                    }
                }
                else
                {
                    target.SecurityLevel = SecurityLevel.NoAuthNoPriv;
                }

                target.SecurityName = this.SecurityName;
                if (this.AuthoritativeEngineID != null)
                {
                    target.AuthoritativeEngineID = this.AuthoritativeEngineID.GetValue();
                }

                if (this.Address is TlsAddress)
                {
                    target.SecurityModel = TSM.SECURITY_MODEL_TSM;
                }
                return target;
            }
            else
            {
                CommunityTarget target = new CommunityTarget();
                target.Community = this.Community;
                return target;
            }
        }

        public PDU send()
        {
            Snmp snmp = CreateSnmpSession();
            this.Target = CreateTarget();
            this.Target.Version = this.Version;
            this.Target.Address = this.Address;
            this.Target.Retries = this.Retries;
            this.Target.Timeout = this.Timeout;
            this.Target.MaxSizeRequestPDU = this.MaxSizeResponsePDU;
            snmp.listen();

            PDU request = CreatePDU(this.Target);
            if (request.Type == PDU.GETBULK)
            {
                request.MaxRepetitions = this.MaxRepetitions;
                request.NonRepeaters = this.NonRepeaters;
            }

            foreach (VariableBinding vb in this.VarBindings)
            {
                request.Add(vb);
            }

            PDU response = null;
            if ((this.Operation == WALK) || (this.Operation == SNAPSHOT_CREATION))
            {
                List<VariableBinding> snapshot = null;

                if (this.Operation == SNAPSHOT_CREATION)
                {
                    snapshot = new List<VariableBinding>();
                }

                Walk(snmp, request, this.Target, snapshot);
                if (snapshot != null)
                {
                    CreateSnapshot(snapshot);
                }
                return null;
            }
            else
            {
                ResponseEventArgs responseEvent;
                long startTime = System.DateTime.Now.Ticks;
                responseEvent = snmp.send(request, target);
                if (responseEvent != null)
                {
                    response = responseEvent.Response;
                    Console.WriteLine("Received response after " +
                                       (System.DateTime.Now.Ticks - startTime) / SnmpConstants.MILLISECOND_TO_NANOSECOND + " milliseconds");
                }
            }

            snmp.close();
            return response;
        }

        private PDU Walk(Snmp snmp, PDU request, ITarget target, List<VariableBinding> snapshot)
        {
            request.NonRepeaters = 0;
            OID rootOID = request[0].Oid;
            PDU response = null;
            
            
            TreeUtils treeUtils = new TreeUtils(snmp, this);

            WalkTreeListener treeListener = new WalkTreeListener(snapshot);

            lock (treeListener)
            {
                treeUtils.GetSubtree(target, rootOID, null, treeListener);
                try
                {
                    treeListener.wait();
                }
                catch (InterruptedException ex)
                {
                    Console.Error.WriteLine("Tree retrieval interrupted: " + ex.getMessage());
                    Thread.currentThread().interrupt();
                }
            }
            return response;
        }


        private static IEnumerable<VariableBinding> GetVariableBindings(string[] args, int position)
        {
            List<VariableBinding> v = new List<VariableBinding>(args.Length - position + 1);
            for (int i = position; i < args.Length; i++)
            {
                string oid = args[i];
                char type = 'i';
                string value = null;
                int equal = oid.IndexOf("={");
                if (equal > 0)
                {
                    oid = args[i].Substring(0, equal);
                    type = args[i][equal + 2];
                    value = args[i].Substring(args[i].IndexOf('}') + 1);
                }
                else if (oid.IndexOf('-') > 0)
                {
                    StringTokenizer st = new StringTokenizer(oid, "-");
                    if (st.countTokens() != 2)
                    {
                        throw new ArgumentException("Illegal OID range specified: '" +
                                                           oid);
                    }

                    oid = st.nextToken();
                    VariableBinding vbLower = new VariableBinding(new OID(oid));
                    v.Add(vbLower);
                    long last = long.Parse(st.nextToken());
                    long first = vbLower.Oid.Last;

                    for (long k = first + 1; k <= last; k++)
                    {
                        OID nextOID = new OID(vbLower.Oid.GetValue(), 0,
                                              vbLower.Oid.Size - 1);
                        nextOID.Append(k);
                        VariableBinding next = new VariableBinding(nextOID);
                        v.Add(next);
                    }

                    continue;
                }

                VariableBinding vb = new VariableBinding(new OID(oid));

                if (value != null)
                {
                    IVariable variable;
                    switch (type)
                    {
                        case 'i':
                            variable = new Integer32(int.Parse(value));
                            break;
                        case 'u':
                            variable = new UnsignedInteger32(long.Parse(value));
                            break;
                        case 's':
                            variable = new OctetString(value);
                            break;
                        case 'x':
                            variable = OctetString.FromString(value, ':', 16);
                            break;
                        case 'd':
                            variable = OctetString.FromString(value, '.', 10);
                            break;
                        case 'b':
                            variable = OctetString.FromString(value, ' ', 2);
                            break;
                        case 'n':
                            variable = new Null();
                            break;
                        case 'o':
                            variable = new OID(value);
                            break;
                        case 't':
                            variable = new TimeTicks(long.Parse(value));
                            break;
                        case 'a':
                            variable = new IpAddress(value);
                            break;
                        default:
                            throw new ArgumentException("Variable type " + type +
                                                               " not supported");
                    }

                    vb.Variable = variable;
                }

                v.Add(vb);
            }
            return v;
        }

        private static IAddress GetAddress(string transportAddress)
        {
            string transport = "udp";
            int colon = transportAddress.IndexOf(':');
            if (colon > 0)
            {
                transport = transportAddress.Substring(0, colon);
                transportAddress = transportAddress.Substring(colon + 1);
            }
            // set default port
            if (transportAddress.IndexOf('/') < 0)
            {
                transportAddress += "/161";
            }

            if (transport.Equals("udp", StringComparison.CurrentCultureIgnoreCase))
            {
                return new UdpAddress(transportAddress);
            }
            else if (transport.Equals("tcp", StringComparison.CurrentCultureIgnoreCase))
            {
                return new TcpAddress(transportAddress);
            }
            else if (transport.Equals("tls", StringComparison.CurrentCultureIgnoreCase))
            {
                return new TlsAddress(transportAddress);
            }

            throw new ArgumentException("Unknown transport " + transport);
        }

        private static string NextOption(string[] args, int position)
        {
            if (position + 1 >= args.Length)
            {
                throw new ArgumentException("Missing option value for " +
                                                   args[position]);
            }

            return args[position + 1];
        }

        private static OctetString CreateOctetString(string s)
        {
            OctetString octetString;

            if (s.StartsWith("0x"))
            {
                octetString = OctetString.FromHexString(s.Substring(2), ':');
            }
            else
            {
                octetString = new OctetString(s);
            }

            return octetString;
        }

        private int ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-a"))
                {
                    String s = NextOption(args, i++);
                    if (s.Equals("MD5"))
                    {
                        this.AuthProtocol = AuthMD5.ID;
                    }
                    else if (s.Equals("SHA"))
                    {
                        this.AuthProtocol = AuthSHA.ID;
                    }
                    else if (s.Equals("SHA256"))
                    {
                        this.AuthProtocol = AuthHMAC192SHA256.ID;
                    }
                    else if (s.Equals("SHA512"))
                    {
                        this.AuthProtocol = AuthHMAC384SHA512.ID;
                    }
                    else
                    {
                        throw new ArgumentException("Authentication protocol unsupported: " + s);
                    }
                }
                else if (args[i].Equals("-A"))
                {
                    this.AuthPassphrase = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-X") || args[i].Equals("-P"))
                {
                    this.PrivPassphrase = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-c"))
                {
                    this.Community = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-b"))
                {
                    this.EngineBootCount = Math.Max(Convert.ToInt32(NextOption(args, i++)), 0);
                }
                else if (args[i].Equals("-d"))
                {
                    String debugOption = NextOption(args, i++);
                    LogFactory.getLogFactory().getRootLogger().setLogLevel(
                        LogLevel.toLevel(debugOption));
                }
                else if (args[i].Equals("-l"))
                {
                    this.LocalEngineID = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-e"))
                {
                    this.AuthoritativeEngineID = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-E"))
                {
                    this.ContextEngineID = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-h"))
                {
                    PrintUsage();
                    Environment.Exit(0);
                }
                else if (args[i].Equals("-n"))
                {
                    this.ContextName = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-m"))
                {
                    this.MaxSizeResponsePDU = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-r"))
                {
                    this.Retries = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-t"))
                {
                    this.Timeout = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-u"))
                {
                    this.SecurityName = CreateOctetString(NextOption(args, i++));
                }
                else if (args[i].Equals("-V"))
                {
                    PrintVersion();
                    Environment.Exit(0);
                }
                else if (args[i].Equals("-Cr"))
                {
                    this.MaxRepetitions = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-Cn"))
                {
                    this.NonRepeaters = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-Ce"))
                {
                    v1TrapPDU.Enterprise = new OID(NextOption(args, i++));
                }
                else if (args[i].Equals("-Ct"))
                {
                    this.TrapOID = new OID(NextOption(args, i++));
                }
                else if (args[i].Equals("-Cg"))
                {
                    v1TrapPDU.GenericTrap = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-Cs"))
                {
                    v1TrapPDU.SpecificTrap = Convert.ToInt32(NextOption(args, i++));
                }
                else if (args[i].Equals("-Ca"))
                {
                    v1TrapPDU.AgentAddress = new IpAddress(NextOption(args, i++));
                }
                else if (args[i].Equals("-Cu"))
                {
                    String upTime = NextOption(args, i++);
                    v1TrapPDU.Timestamp = long.Parse(upTime);
                    this.SysUpTime.SetValue(long.Parse(upTime));
                }
                else if (args[i].Equals("-Ow"))
                {
                    this.Operation = WALK;
                }
                else if (args[i].Equals("-Ocs"))
                {
                    this.Operation = SNAPSHOT_CREATION;
                    snapshotFile = NextOption(args, i++);
                    if (!snapshotFile.canWrite() && snapshotFile.exists())
                    {
                        throw new ArgumentException("Snapshot file '" + snapshotFile +
                                                           "' cannot be written");
                    }
                }
                else if (args[i].Equals("-Ods"))
                {
                    this.Operation = SNAPSHOT_DUMP;
                    snapshotFile = NextOption(args, i++);
                    if (!snapshotFile.canRead())
                    {
                        throw new ArgumentException("Snapshot file '" + snapshotFile +
                                                           "' cannot be read");
                    }
                }
                else if (args[i].Equals("-Ol"))
                {
                    this.Operation = LISTEN;
                }
                else if (args[i].Equals("-OtCSV"))
                {
                    this.Operation = CVS_TABLE;
                }
                else if (args[i].Equals("-OttCSV"))
                {
                    this.Operation = TIME_BASED_CVS_TABLE;
                }
                else if (args[i].Equals("-Ot"))
                {
                    this.Operation = TABLE;
                }
                else if (args[i].Equals("-Otd"))
                {
                    this.Operation = TABLE;
                    useDenseTableOperation = true;
                }
                else if (args[i].Equals("-Cil"))
                {
                    lowerBoundIndex = new OID(NextOption(args, i++));
                }
                else if (args[i].Equals("-Ciu"))
                {
                    upperBoundIndex = new OID(NextOption(args, i++));
                }
                else if (args[i].Equals("-v"))
                {
                    string v = NextOption(args, i++);
                    if (v.Equals("1"))
                    {
                        this.Version = SnmpConstants.version1;
                    }
                    else if (v.Equals("2c"))
                    {
                        this.Version = SnmpConstants.version2c;
                    }
                    else if (v.Equals("3"))
                    {
                        this.Version = SnmpConstants.version3;
                    }
                    else
                    {
                        throw new ArgumentException("Version " + v + " not supported");
                    }
                }
                else if (args[i].Equals("-x"))
                {
                    string s = NextOption(args, i++);
                    if (s.Equals("DES"))
                    {
                        this.PrivProtocol = PrivDES.ID;
                    }
                    else if ((s.Equals("AES128")) || (s.Equals("AES")))
                    {
                        this.PrivProtocol = PrivAES128.ID;
                    }
                    else if (s.Equals("AES192"))
                    {
                        this.PrivProtocol = PrivAES192.ID;
                    }
                    else if (s.Equals("AES256"))
                    {
                        this.PrivProtocol = PrivAES256.ID;
                    }
                    else if ((s.Equals("3DES") || s.Equals("DESEDE", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        this.PrivProtocol = Priv3DES.ID;
                    }
                    else
                    {
                        throw new ArgumentException("Privacy protocol " + s +
                                                           " not supported");
                    }
                }
                else if (args[i].Equals("-p"))
                {
                    string s = NextOption(args, i++);
                    this.PduType = PDU.GetTypeFromString(s);
                    if (this.PduType == int.MinValue)
                    {
                        throw new ArgumentException("Unknown PDU type " + s);
                    }
                }
                else if (!args[i].StartsWith("-"))
                {
                    return i;
                }
                else
                {
                    throw new ArgumentException("Unknown option " + args[i]);
                }
            }

            return 0;
        }

        protected static void PrintVersion()
        {
            Console.WriteLine();
            Console.WriteLine("JunoSnmp Command Line Tool v" + VersionInfo.getVersion() +
                               " Copyright " + (char)0xA9 +
                               " 2004-2011, Frank Fock and Jochen Katz for SNMP4J, 2017 Jeremy Gibbons for JunoSnmp");
            Console.WriteLine("http://www.snmp4j.org");
            Console.WriteLine();
            Console.WriteLine("SNMP4J is licensed under the Apache License 2.0:");
            Console.WriteLine("http://www.apache.org/licenses/LICENSE-2.0.txt");
            Console.WriteLine();
        }

        /**
         * printUsage
         */
        protected static void PrintUsage()
        {
            string[] usage = new string[] {
        "",
        "Usage: SNMP4J [options] [transport:]address [OID[={type}value] ...]",
        "",
        "  -a  authProtocol      Sets the authentication protocol used to",
        "                        authenticate SNMPv3 messages. Valid values are",
        "                        MD5 and SHA.",
        "  -A  authPassphrase    Sets the authentication pass phrase for authenticated",
        "                        SNMPv3 messages.",
        "  -b  engineBootCount   Sets the engine boot count to the specified value",
        "                        greater or equal to zero. Default is zero.",
        "  -c  community         Sets the community for SNMPv1/v2c messages.",
        "  -Ca agentAddress      Sets the agent address field of a V1TRAP PDU.",
        "                        The default value is '0.0.0.0'.",
        "  -Cg genericID         Sets the generic ID for SNMPv1 TRAPs (V1TRAP).",
        "                        The default is 0 (coldStart).",
        "  -Ce enterpriseOID     Sets the enterprise OID field of a V1TRAP PDU.",
        "  -Cil lowerBoundIndex  Sets the lower bound index for TABLE operations.",
        "  -Ciu upperBoundIndex  Sets the upper bound index for TABLE operations.",
        "  -Cn non-repeaters     Sets  the  non-repeaters field for GETBULK PDUs.",
        "                        It specifies the number of supplied variables that",
        "                        should not be iterated over. The default is 0.",
        "  -Cr max-repetitions   Sets the max-repetitions field for GETBULK PDUs.",
        "                        This specifies the maximum number of iterations",
        "                        over the repeating variables. The default is 10.",
        "  -Cs specificID        Sets the specific ID for V1TRAP PDU. The default is 0.",
        "  -Ct trapOID           Sets the trapOID (1.3.6.1.6.3.1.1.4.1.0) of an INFORM",
        "                        or TRAP PDU. The default is 1.3.6.1.6.3.1.1.5.1.",
        "  -Cu upTime            Sets the sysUpTime field of an INFORM, TRAP, or",
        "                        V1TRAP PDU.",
        "  -d  debugLevel        Sets the global debug level for Log4J logging output.",
        "                        Valid values are OFF, ERROR, WARN, INFO, and DEBUG.",
        "  -e  engineID          Sets the authoritative engine ID of the command",
        "                        responder used for SNMPv3 request messages. If not",
        "                        supplied, the engine ID will be discovered.",
        "  -E  contextEngineID   Sets the context engine ID used for the SNMPv3 scoped",
        "                        PDU. The authoritative engine ID will be used for the",
        "                        context engine ID, if the latter is not specified.",
        "  -h                    Displays this message and then exits the application.",
        "  -l  localEngineID     Sets the local engine ID of the command generator",
        "                        and the notification receiver (thus this SNMP4J-Tool)",
        "                        used for SNMPv3 request messages. This option can be",
        "                        used to avoid engine ID clashes through duplicate IDs",
        "                        leading to usmStatsNotInTimeWindows reports.",
        "  -n  contextName       Sets the target context name for SNMPv3 messages. ",
        "                        Default is the empty string.",
        "  -m  maxSizeRespPDU    The maximum size of the response PDU in bytes.",
        "  -Ocs <file>           Same as -Ow except that the retrieved values are also",
        "                        written as a serialized ArrayList of VariableBinding",
        "                        instances to the specified file. The snapshot format",
        "                        can be read later by other applications, for example",
        "                        MIB Explorer Pro 2.2 or later. See also -Ods.",
        "  -Ods <file>           Reads the snapshot file and dumps its contents on",
        "                        standard out. No SNMP operation will be done.",
        "                        See also -Ocs.",
        "  -Ol                   Activates listen operation mode. In this mode, the",
        "                        application will listen for incoming TRAPs and INFORMs",
        "                        on the supplied address. Received request will be",
        "                        dumped to the console until the application is stopped.",
        "  -Ot                   Activates table operation mode. In this mode, the",
        "                        application receives tabular data from the column",
        "                        OIDs specified as parameters. The retrieved rows will",
        "                        be dumped to the console ordered by their index values.",
        "  -Otd                  Activates dense table operation mode. In this mode, the",
        "                        application receives tabular data from the column",
        "                        OIDs specified as parameters. The retrieved rows will",
        "                        be dumped to the console ordered by their index values.",
        "                        In contrast to -Ot this option must not be used with",
        "                        sparse tables. ",
        "  -OtCSV                Same as -Ot except that for each SNMP row received",
        "                        exactly one row of comma separated values will printed",
        "                        to the console where the first column contains the row",
        "                        index.",
        "  -OttCSV               Same as -OtCSV except that each row's first column",
        "                        will report the current time (millis after 1.1.1970)",
        "                        when the request has been sent.",
        "  -Ow                   Activates walk operation mode for GETNEXT and GETBULK",
        "                        PDUs. If activated, the GETNEXT and GETBULK operations",
        "                        will be repeated until all instances within the",
        "                        OID subtree of the supplied OID have been retrieved",
        "                        successfully or until an error occurred.",
        "  -p  pduType           Specifies the PDU type to be used for the message.",
        "                        Valid types are GET, GETNEXT, GETBULK (SNMPv2c/v3),",
        "                        SET, INFORM, TRAP, and V1TRAP (SNMPv1).",
        "  -P  privacyPassphrase Sets the privacy pass phrase for encrypted",
        "                        SNMPv3 messages (same as -X).",
        "  -r  retries           Sets the number of retries used for requests. A zero",
        "                        value will send out a request exactly once.",
        "                        Default is 1.",
        "  -t  timeout           Sets the timeout in milliseconds between retries.",
        "                        Default is 1000 milliseconds.",
        "  -u  securityName      Sets the security name for authenticated v3 messages.",
        "  -v  1|2c|3            Sets the SNMP protocol version to be used.",
        "                        Default is 3.",
        "  -V                    Displays version information and then exits.",
        "  -x  privacyProtocol   Sets the privacy protocol to be used to encrypt",
        "                        SNMPv3 messages. Valid values are DES, AES (AES128),",
        "                        AES192, AES256, and 3DES(DESEDE).",
        "  -X  privacyPassphrase Sets the privacy pass phrase for encrypted",
        "                        SNMPv3 messages (same as -P).",
        "",
        "The address of the target SNMP engine is parsed according to the",
        "specified <transport> selector (default selector is udp):",
        "",
        "  udp | tcp | tls       hostname[/port]",
        "                        ipv4Address[/port]",
        "                        ipv6Address[/port]",
        "",
        "The OIDs have to be specified in numerical form where strings may be"+
        "enclosed in single quotes ('), for example:",
        "  1.3.6.1.2.1.1.5.0  (which will return the sysName.0 instance with a GET)",
        "  1.3.6.1.6.3.16.1.2.1.3.2.6.'public'  (which will return the ",
        "    vacmGroupName.2.6.112.117.98.108.105.99 instance with a GET)",
        "To request multiple instances, add additional OIDs with a space as",
        "separator. For the last sub-identifier of a plain OID (without an assigned",
        "value) a range can be specified, for example '1.3.6.1.2.1.2.2.1-10' will",
        "has the same effect as enumerating all OIDs from '1.3.6.1.2.1.2.2.1' to",
        "'1.3.6.1.2.1.2.2.10'.",
        "For SET and INFORM request, you can specify a value for each OID by",
        "using the following form: OID={type}value where <type> is one of",
        "the following single characters enclosed by '{' and '}':",
        "  i                     Integer32",
        "  u                     UnsingedInteger32, Gauge32",
        "  s                     OCTET STRING",
        "  x                     OCTET STRING specified as hex string where",
        "                        bytes separated by colons (':').",
        "  d                     OCTET STRING specified as decimal string",
        "                        where bytes are separated by dots ('.').",
        "  n                     Null",
        "  o                     OBJECT IDENTIFIER",
        "  t                     TimeTicks",
        "  a                     IpAddress",
        "  b                     OCTET STRING specified as binary string where",
        "                        bytes are separated by spaces.",
        "",
        "An example for a complete SNMPv2c SET request to set sysName:",
        " SNMP4J -c private -v 2c -p SET udp:localhost/161 \"1.3.6.1.2.1.1.5.0={s}SNMP4J\"",
        "",
        "To walk the whole MIB tree with GETBULK and using SNMPv3 MD5 authentication:",
        " SNMP4J -a MD5 -A MD5UserAuthPassword -u MD5User -p GETBULK -Ow 127.0.0.1/161",
        "",
        "Listen for unauthenticated SNMPv3 INFORMs and TRAPs and all v1/v2c TRAPs:",
        " SNMP4J -u aSecurityName -Ol 0.0.0.0/162",
        "",
        "Send an unauthenticated SNMPv3 notification (trap):",
        " SNMP4J -p TRAP -v 3 -u aSecurityName 127.0.0.1/162 \"1.3.6.1.2.1.1.3.0={t}0\" \\",
        "  \"1.3.6.1.6.3.1.1.4.1.0={o}1.3.6.1.6.3.1.1.5.1\" \\",
        "  \"1.3.6.1.2.1.1.1.0={s}System XYZ, Version N.M\"",
        "Retrieve rows of the columnar objects ifDescr to ifInOctets and ifOutOctets:",
        " SNMP4J -c public -v 2c -Ot localhost 1.3.6.1.2.1.2.2.1.2-10\\",
        "  1.3.6.1.2.1.2.2.1.16",
        ""
    };

            foreach (string anUsage in usage)
            {
                Console.WriteLine(anUsage);
            }
        }


        protected static void PrintVariableBindings(PDU response)
        {
            for (int i = 0; i < response.Count; i++)
            {
                VariableBinding vb = response[i];
                Console.WriteLine(vb.ToString());
            }
        }

        protected static void PrintReport(PDU response)
        {
            if (response.Count < 1)
            {
                Console.WriteLine("REPORT PDU does not contain a variable binding.");
                return;
            }

            VariableBinding vb = response[0];
            OID oid = vb.Oid;
            if (SnmpConstants.usmStatsUnsupportedSecLevels.Equals(oid))
            {
                Console.Write("REPORT: Unsupported Security Level.");
            }
            else if (SnmpConstants.usmStatsNotInTimeWindows.Equals(oid))
            {
                Console.Write("REPORT: Message not within time window.");
            }
            else if (SnmpConstants.usmStatsUnknownUserNames.Equals(oid))
            {
                Console.Write("REPORT: Unknown user name.");
            }
            else if (SnmpConstants.usmStatsUnknownEngineIDs.Equals(oid))
            {
                Console.Write("REPORT: Unknown engine id.");
            }
            else if (SnmpConstants.usmStatsWrongDigests.Equals(oid))
            {
                Console.Write("REPORT: Wrong digest.");
            }
            else if (SnmpConstants.usmStatsDecryptionErrors.Equals(oid))
            {
                Console.Write("REPORT: Decryption error.");
            }
            else if (SnmpConstants.snmpUnknownSecurityModels.Equals(oid))
            {
                Console.Write("REPORT: Unknown security model.");
            }
            else if (SnmpConstants.snmpInvalidMsgs.Equals(oid))
            {
                Console.Write("REPORT: Invalid message.");
            }
            else if (SnmpConstants.snmpUnknownPDUHandlers.Equals(oid))
            {
                Console.Write("REPORT: Unknown PDU handler.");
            }
            else if (SnmpConstants.snmpUnavailableContexts.Equals(oid))
            {
                Console.Write("REPORT: Unavailable context.");
            }
            else if (SnmpConstants.snmpUnknownContexts.Equals(oid))
            {
                Console.Write("REPORT: Unknown context.");
            }
            else
            {
                Console.Write("REPORT contains unknown OID ("
                                   + oid.ToString() + ").");
            }
            Console.WriteLine(" Current counter value is " +
                               vb.Variable.ToString() + ".");
        }

        //synchronized
        public void processPdu(CommandResponderEventArgs e)
        {
            PDU command = e.PDU;
            if (command != null)
            {
                Console.WriteLine(command.ToString());
                if ((command.Type != PDU.TRAP) &&
                    (command.Type != PDU.V1TRAP) &&
                    (command.Type != PDU.REPORT) &&
                    (command.Type != PDU.RESPONSE))
                {
                    command.ErrorIndex = 0;
                    command.ErrorStatus = 0;
                    command.Type = PDU.RESPONSE;
                    StatusInformation statusInformation = new StatusInformation();
                    StateReference sref = e.StateReference;
                    try
                    {
                        e.MessageDispatcher.ReturnResponsePdu(e.MessageProcessingModel,
                                                                   e.SecurityModel,
                                                                   e.SecurityName,
                                                                   e.SecurityLevel,
                                                                   command,
                                                                   e.MaxSizeResponsePDU,
                                                                   sref,
                                                                   statusInformation);
                    }
                    catch (MessageException ex)
                    {
                        Console.Error.WriteLine("Error while sending response: " + ex.Message);
                        LogFactory.getLogger(typeof(SnmpRequest)).error(ex);
                    }
                }
            }
        }

        public PDU CreatePDU(ITarget target)
        {
            PDU request;
            if (target.Version == SnmpConstants.version3)
            {
                request = new ScopedPDU();
                ScopedPDU scopedPDU = (ScopedPDU)request;
                if (this.ContextEngineID != null)
                {
                    scopedPDU.ContextEngineID = this.ContextEngineID;
                }
                if (this.ContextName != null)
                {
                    scopedPDU.ContextName = this.ContextName;
                }
            }
            else
            {
                if (this.PduType == PDU.V1TRAP)
                {
                    request = v1TrapPDU;
                }
                else
                {
                    request = new PDU();
                }
            }

            request.Type = this.PduType;
            return request;
        }

        public PDU CreatePDU(MessageProcessingModel messageProcessingModel)
        {
            return createPDU((ITarget)null);
        }

        public void table()
        {
            Snmp snmp = CreateSnmpSession();
            this.Target = CreateTarget();
            this.Target.Version = this.Version;
            this.Target.Address = this.Address;
            this.Target.Retries = this.Retries;
            this.Target.Timeout = this.Timeout;
            snmp.listen();

            TableUtils tableUtils = new TableUtils(snmp, this);
            tableUtils.MaxNumRowsPerPDU = this.MaxRepetitions;
            Counter32 counter = new Counter32();

            OID[] columns = new OID[this.VarBindings.Count];
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = this.VarBindings[i].Oid;
            }

            long startTime = System.nanoTime();
            lock (counter)
            {

                TableListener listener;
                if (this.Operation == TABLE)
                {
                    listener = new TextTableListener();
                }
                else
                {
                    listener = new CVSTableListener(System.nanoTime());
                }

                if (useDenseTableOperation)
                {
                    tableUtils.GetDenseTable(this.Target, columns, listener, counter,
                                             lowerBoundIndex, upperBoundIndex);
                }
                else
                {
                    tableUtils.GetTable(this.Target, columns, listener, counter,
                                        lowerBoundIndex, upperBoundIndex);
                }

                try
                {
                    counter.Wait(timeout);
                }
                catch (InterruptedException ex)
                {
                    Thread.currentThread().interrupt();
                }
            }

            Console.WriteLine("Table received in " +
                                   (System.nanoTime() - startTime) / SnmpConstants.MILLISECOND_TO_NANOSECOND + " milliseconds.");
            snmp.close();
        }

        class WalkTreeListener
        {
            private bool finished;
            private readonly WalkCounts counts = new WalkCounts();
            private readonly IList<VariableBinding> snapshot;
            private readonly long startTime = System.DateTime.Now.Ticks;

            protected WalkTreeListener(IList<VariableBinding> snapshot)
            {
                this.snapshot = snapshot;
            }

            public bool Next(TreeEventArgs e)
            {
                counts.requests++;
                if (e.VariableBindings != null)
                {
                    VariableBinding[] vbs = e.VariableBindings;
                    counts.objects += vbs.Length;
                    foreach (VariableBinding vb in vbs)
                    {
                        if (snapshot != null)
                        {
                            snapshot.Add(vb);
                        }

                        Console.WriteLine(vb.ToString());
                    }
                }

                return true;
            }

            public void Finished(TreeEventArgs e)
            {
                if ((e.VariableBindings != null) &&
                    (e.VariableBindings.Length > 0))
                {
                    Next(e);
                }

                Console.WriteLine();
                Console.WriteLine("Total requests sent:    " + counts.requests);
                Console.WriteLine("Total objects received: " + counts.objects);
                Console.WriteLine("Total walk time:        " +
                                   (System.nanoTime() - startTime) / SnmpConstants.MILLISECOND_TO_NANOSECOND + " milliseconds");
                if (e.IsError)
                {
                    Console.Error.WriteLine("The following error occurred during walk:");
                    Console.Error.WriteLine(e.ErrorMessage);
                }
                finished = true;
                lock (this)
                {
                    this.notify();
                }
            }

            public bool IsFinished()
            {
                return finished;
            }
        }

        class CVSTableListener : TableListener
        {

            private long requestTime;
            private bool finished;
            private SnmpRequest request;

            public CVSTableListener(long time, SnmpRequest request)
            {
                this.requestTime = time;
                this.request = request;
            }

            public bool Next(TableEventArgs evt)
            {
                if (request.Operation == TIME_BASED_CVS_TABLE)
                {
                    Console.Write(requestTime);
                    Console.Write(",");
                }

                Console.Write("\"" + evt.Index + "\",");
                for (int i = 0; i < evt.Columns.Length; i++)
                {
                    IVariable v = evt.Columns[i].Variable;
                    string value = v.ToString();
                    switch (v.Syntax)
                    {
                        case SMIConstants.SyntaxOctetString:
                            {
                                StringBuffer escapedString = new StringBuffer(value.length());
                                StringTokenizer st = new StringTokenizer(value, "\"", true);
                                while (st.hasMoreTokens())
                                {
                                    string token = st.nextToken();
                                    escapedString.append(token);
                                    if (token.Equals("\""))
                                    {
                                        escapedString.append("\"");
                                    }
                                }
                            }
                        case SMIConstants.SyntaxIpAddress:
                        case SMIConstants.SyntaxObjectIdentifier:
                        case SMIConstants.SyntaxOpaque:
                            {
                                Console.Write("\"");
                                Console.Write(value);
                                Console.Write("\"");
                                break;
                            }
                        default:
                            {
                                Console.Write(value);
                            }
                    }
                    if (i + 1 < evt.getColumns().length)
                    {
                        Console.Write(",");
                    }
                }
                Console.WriteLine();
                return true;
            }

            public void Finished(TableEventArgs evt)
            {
                finished = true;
                lock (evt.UserObject)
                {
                    evt.UserObject.Notify();
                }
            }

            public bool IsFinished()
            {
                return finished;
            }

        }

        class TextTableListener : TableListener
        {

            private bool finished;

            public void Finished(TableEventArgs evt)
            {
                Console.WriteLine();
                Console.WriteLine("Table walk completed with status " + evt.getStatus() +
                           ". Received " +
                             evt.getUserObject() + " rows.");
                finished = true;
                lock (evt.UserObject)
                {
                    evt.UserObject.notify();
                }
            }

            public bool Next(TableEventArgs evt)
            {
                Console.WriteLine("Index = " + evt.Index + ":");
                for (int i = 0; i < evt.Columns.Length; i++)
                {
                    Console.WriteLine(evt.Columns[i]);
                }
                Console.WriteLine();
                ((Counter32)evt.UserObject).Increment();
                return true;
            }

            public bool IsFinished()
            {
                return finished;
            }

        }

        public static void main(String[] args)
        {
            try
            {
                /* Initialize Log4J logging:
                      if (System.getProperty("log4j.configuration") == null) {
                        BasicConfigurator.configure();
                      }
                      Logger.getRootLogger().setLevel(Level.OFF);
                */
                SnmpRequest snmpRequest = new SnmpRequest(args);
                if (snmpRequest.operation == SNAPSHOT_DUMP)
                {
                    snmpRequest.dumpSnapshot();
                }
                else
                {
                    try
                    {
                        if (snmpRequest.operation == LISTEN)
                        {
                            snmpRequest.Listen();
                        }
                        else if ((snmpRequest.operation == TABLE) ||
                                 (snmpRequest.operation == CVS_TABLE) ||
                                 (snmpRequest.operation == TIME_BASED_CVS_TABLE))
                        {
                            snmpRequest.table();
                        }
                        else
                        {
                            PDU response = snmpRequest.send();
                            if ((snmpRequest.PduType == PDU.TRAP) ||
                                (snmpRequest.PduType == PDU.REPORT) ||
                                (snmpRequest.PduType == PDU.V1TRAP) ||
                                (snmpRequest.PduType == PDU.RESPONSE))
                            {
                                Console.WriteLine(PDU.GetTypeString(snmpRequest.PduType) +
                                                   " sent successfully");
                            }
                            else if (response == null)
                            {
                                if (snmpRequest.operation != WALK)
                                {
                                    Console.WriteLine("Request timed out.");
                                }
                            }
                            else if (response.Type == PDU.REPORT)
                            {
                                printReport(response);
                            }
                            else if (snmpRequest.operation == DEFAULT)
                            {
                                Console.WriteLine("Response received with requestID=" +
                                                   response.RequestID +
                                                   ", errorIndex=" +
                                                   response.ErrorIndex + ", " +
                                                   "errorStatus=" + response.ErrorStatusText +
                                                   "(" + response.ErrorStatus + ")");
                                PrintVariableBindings(response);
                            }
                            else
                            {
                                Console.WriteLine("Received something strange: requestID=" +
                                                   response.RequestID +
                                                   ", errorIndex=" +
                                                   response.ErrorIndex + ", " +
                                                   "errorStatus=" + response.ErrorStatusText +
                                                   "(" + response.ErrorStatus + ")");
                                PrintVariableBindings(response);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine("Error while trying to send request: " +
                                           ex.getMessage());
                        ex.printStackTrace();
                    }
                }
            }
            catch (ArgumentException iaex)
            {
                Console.Error.WriteLine("Error: " + iaex.Message);
                Console.Error.Write(iaex.StackTrace);
            }
        }

        private void CreateSnapshot(List<VariableBinding> snapshot)
        {
            FileOutputStream fos = null;
            try
            {
                fos = new FileOutputStream(snapshotFile);
                ObjectOutputStream oos = new ObjectOutputStream(fos);
                oos.writeObject(snapshot);
                oos.flush();
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.StackTrace);
            }
            finally
            {
                if (fos != null)
                {
                    try
                    {
                        fos.close();
                    }
                    catch (IOException ex1)
                    {
                        ex1.printStackTrace();
                    }
                }
            }
        }

        private void DumpSnapshot()
        {
            FileInputStream fis = null;
            try
            {
                fis = new FileInputStream(snapshotFile);
                ObjectInputStream ois = new ObjectInputStream(fis);
                List l = (List)ois.readObject();
                int i = 1;
                Console.WriteLine("Dumping snapshot file '" + snapshotFile + "':");
                for (Iterator it = l.iterator(); it.hasNext(); i++)
                {
                    Console.WriteLine("" + i + ": " + it.next());
                }
                Console.WriteLine();
                Console.WriteLine("Dumped " + l.size() + " variable bindings.");
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.StackTrace);
            }
            finally
            {
                if (fis != null)
                {
                    try
                    {
                        fis.close();
                    }
                    catch (IOException ex1)
                    {
                    }
                }
            }
        }

        public void SetVbs(List<VariableBinding> vbs)
        {
            this.vbs = vbs;
        }

        public void SetUseDenseTableOperation(bool useDenseTableOperation)
        {
            this.useDenseTableOperation = useDenseTableOperation;
        }

        public void SetUpperBoundIndex(OID upperBoundIndex)
        {
            this.upperBoundIndex = upperBoundIndex;
        }

        public void SetNumDispatcherThreads(int numDispatcherThreads)
        {
            this.numDispatcherThreads = numDispatcherThreads;
        }

        public void SetLowerBoundIndex(OID lowerBoundIndex)
        {
            this.lowerBoundIndex = lowerBoundIndex;
        }

        class WalkCounts
        {
            public int requests;
            public int objects;
        }
    }
}
