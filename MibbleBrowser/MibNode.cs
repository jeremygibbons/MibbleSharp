

namespace MibbleBrowser
{
    using System.Windows.Forms;
    using MibbleSharp;
    using MibbleSharp.Snmp;
    using MibbleSharp.Value;


    /// <summary>
    /// 
    /// </summary>
    class MibNode : TreeNode
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MibNode"/> class.
        /// </summary>
        /// <param name="name">The node's name</param>
        /// <param name="value">The node's value</param>
        public MibNode(string name, ObjectIdentifierValue value)
            : base(name)
        {
            this.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectIdentifierValue Value
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Oid
        {
            get
            {
                if(this.Value == null)
                {
                    return string.Empty;
                }
                return Value.ToString();
            }
        }

        public MibValueSymbol Symbol
        {
            get
            {
                if(this.Value == null)
                {
                    return null;
                }
                return this.Value.Symbol;
            }
        }

        public SnmpObjectType SnmpObjType
        {
            get
            {
                MibValueSymbol symbol = this.Symbol;

                if(symbol == null)
                {
                    return null;
                }

                return symbol.Type as SnmpObjectType;
            }
        }

        public string Description
        {
            get
            {
                if (this.Value != null && this.Value.Symbol != null)
                {
                    return Value.Symbol.ToString();
                }

                return string.Empty;
            }
        }
    }
}
