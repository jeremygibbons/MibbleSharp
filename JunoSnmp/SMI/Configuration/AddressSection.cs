using System.Configuration;

namespace JunoSnmp.SMI.Configuration
{
    class AddressSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection addressProperties;

        private static ConfigurationProperty udp;
        private static ConfigurationProperty tcp;
        private static ConfigurationProperty ip;
        private static ConfigurationProperty tls;
        private static ConfigurationProperty ssh;

        static AddressSection()
        {
            AddressSection.addressProperties = new ConfigurationPropertyCollection();

            AddressSection.udp = new ConfigurationProperty(
                "udp",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            AddressSection.tcp = new ConfigurationProperty(
                "tcp",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            AddressSection.ip = new ConfigurationProperty(
                "ip",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            AddressSection.tls = new ConfigurationProperty(
                "tls",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            AddressSection.ssh = new ConfigurationProperty(
                "ssh",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            AddressSection.addressProperties.Add(AddressSection.udp);
            AddressSection.addressProperties.Add(AddressSection.tcp);
            AddressSection.addressProperties.Add(AddressSection.ip);
            AddressSection.addressProperties.Add(AddressSection.tls);
            AddressSection.addressProperties.Add(AddressSection.ssh);
        }

        /// <summary>
        /// Gets the udp setting.
        /// </summary>
        [ConfigurationProperty("udp", IsRequired = true)]
        public string Udp
        {
            get { return (string)base[AddressSection.udp]; }
        }

        /// <summary>
        /// Gets the tcp setting.
        /// </summary>
        [ConfigurationProperty("tcp", IsRequired = true)]
        public string Tcp
        {
            get { return (string)base[AddressSection.tcp]; }
        }

        /// <summary>
        /// Gets the ip setting.
        /// </summary>
        [ConfigurationProperty("ip", IsRequired = true)]
        public string Ip
        {
            get { return (string)base[AddressSection.ip]; }
        }

        /// <summary>
        /// Gets the tls setting.
        /// </summary>
        [ConfigurationProperty("tls", IsRequired = true)]
        public string Tls
        {
            get { return (string)base[AddressSection.tls]; }
        }

        /// <summary>
        /// Gets the ssh setting.
        /// </summary>
        [ConfigurationProperty("ssh", IsRequired = true)]
        public string Ssh
        {
            get { return (string)base[AddressSection.ssh]; }
        }

        /// <summary>
        /// Gets this custom property collection.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return AddressSection.addressProperties; }
        }
    }
}
