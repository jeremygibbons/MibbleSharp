using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp
{
    /**
     * A MIB macro symbol. This class holds information relevant to a MIB
     * macro definition, i.e. a defined macro name.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.6
     */
    public class MibMacroSymbol : MibSymbol
    {

        /**
         * Creates a new macro symbol
         *
         * @param location       the symbol location
         * @param mib            the symbol MIB file
         * @param name           the symbol name
         */
        public MibMacroSymbol(FileLocation location, Mib mib, string name) :
            base(location, mib, name)
        {
        }

        /**
         * Initializes the MIB symbol. This will remove all levels of
         * indirection present, such as references to types or values. No
         * information is lost by this operation. This method may modify
         * this object as a side-effect.<p>
         *
         * <strong>NOTE:</strong> This is an internal method that should
         * only be called by the MIB loader.
         *
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public override void Initialize(MibLoaderLog log)
        {
            // Nothing to be initialized
        }

        /**
         * Clears and prepares this MIB symbol for garbage collection.
         * This method will recursively clear any associated types or
         * values, making sure that no data structures references this
         * symbol.
         */
        public override void Clear()
        {
            // Nothing to clear
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("MACRO ");
            builder.Append(getName());
            return builder.ToString();
        }
    }

}
