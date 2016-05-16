
/**
 * A MIB symbol context. This interface is implemented by all objects
 * that contains multiple named references to MIB symbols.<p>
 *
 * <strong>Note:</strong> This interface is internal to Mibble and
 * only exposed to make it available in different packages.
 *
 * @author   Per Cederberg, <per at percederberg dot net>
 * @version  2.4
 * @since    2.0
 */

namespace MibbleSharp
{
    public interface MibContext
    {
        /**
        * Searches for a named MIB symbol. This method may search outside
        * the normal (or strict) scope, thereby allowing a form of
        * relaxed search. Note that the results from the normal and
        * expanded search may not be identical, due to the context
        * chaining and the same symbol name appearing in various
        * contexts.<p>
        *
        * <strong>NOTE:</strong> This is an internal method that should
        * only be called by the MIB loader.
        *
        * @param name           the symbol name
        * @param expanded       the expanded scope flag
        *
        * @return the MIB symbol, or null if not found
        *
        * @since 2.4
        */
        MibSymbol findSymbol(string name, bool expanded);
    }
}
