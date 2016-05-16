using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp
{
    /**
     * An SNMP MIB module. This class contains all the information
     * from a single MIB module, including all defined types and values.
     * Note that a single MIB file may contain several such modules,
     * although that is not very common. MIB files are loaded through a
     * {@link MibLoader MIB loader}.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.7
     * @since    2.0
     *
     * @see <a href="http://www.ietf.org/rfc/rfc3411.txt">RFC 3411 - An
     *      Architecture for Describing SNMP Management Frameworks</a>
     */
    public class Mib : MibContext
    {

        /**
         * The MIB file.
         */
        private string file;

        /**
         * The loader used for this MIB.
         */
        private MibLoader loader;

        /**
         * The loader log used for loading this MIB.
         */
        private MibLoaderLog log;

        /**
         * The explicitly loaded flag. This flag is set when a MIB is
         * loaded by a direct call to the MibLoader, in contrast to when
         * it is loaded as the result of an import.
         */
        private bool loaded = false;

        /**
         * The MIB name.
         */
        private string name = null;

        /**
         * The SMI version.
         */
        private int smiVersion = 1;

        /**
         * The MIB file header comment.
         */
        private string headerComment = null;

        /**
         * The MIB file footer comment.
         */
        private string footerComment = null;

        /**
         * The references to imported MIB files.
         */
        private IList<MibImport> imports = new List<MibImport>();

        /**
         * The MIB symbol list. This list contains the MIB symbol objects
         * in the order they were added (i.e. present in the file).
         */
        private IList<MibSymbol> symbolList = new List<MibSymbol>();

        /**
         * The MIB symbol name map. This maps the symbol names to their
         * respective MIB symbol objects.
         */
        private Dictionary<string, MibSymbol> symbolNameMap = new Dictionary<string, MibSymbol>();

        /**
         * The MIB symbol value map. This maps the symbol values to their
         * respective MIB symbol objects. Only the value symbols with
         * either a number or an object identifier value is present in
         * this map.
         */
        private Dictionary<string, MibValueSymbol> symbolValueMap = new Dictionary<string, MibValueSymbol>();

        /**
         * Creates a new MIB module. This will NOT read the actual MIB
         * file, but only creates an empty container. The symbols are
         * then added during the first analysis pass (of two), leaving
         * symbols in the MIB possibly containing unresolved references.
         * A separate call to initialize() must be made once all
         * referenced MIB modules have also been loaded.
         *
         * @param file           the MIB file name
         * @param loader         the MIB loader to use for imports
         * @param log            the MIB log to use for errors
         *
         * @see #initialize()
         */
        public Mib(string file, MibLoader loader, MibLoaderLog log)
        {
            this.file = file;
            this.loader = loader;
            this.log = log;
        }

        /**
         * Initializes the MIB file. This will resolve all imported MIB
         * file references. Note that this method shouldn't be called
         * until all referenced MIB files (and their respective
         * references) have been loaded.
         *
         * @throws MibLoaderException if the MIB file couldn't be
         *             analyzed correctly
         *
         * @see #validate()
         */
        public void Initialize()
        {
            int errors = log.errorCount();

            // Resolve imported MIB files
            foreach (MibImport imp in imports)
            {
                try
                {
                    imp.Initialize(log);
                }
                catch (MibException e)
                {
                    log.addError(e.getLocation(), e.Message);
                }
            }

            // Check for errors
            if (errors != log.errorCount())
            {
                throw new MibLoaderException(log);
            }
        }

        /**
         * Validates the MIB file. This will resolve all type and value
         * references in the MIB symbols, while also validating them for
         * consistency. Note that this method shouldn't be called until
         * all referenced MIB files (and their respective references)
         * have been initialized.
         *
         * @throws MibLoaderException if the MIB file couldn't be
         *             analyzed correctly
         *
         * @see #initialize()
         */
        public void validate()
        {

            int errors = log.errorCount();

            // Validate all symbols
            foreach (MibSymbol symbol in symbolList)
            {
                try
                {
                    symbol.Initialize(log);
                }
                catch (MibException e)
                {
                    log.addError(e.getLocation(), e.Message);
                }

                MibValueSymbol value = symbol as MibValueSymbol;
                if (value != null && (value.getValue() is NumberValue || value.getValue() is ObjectIdentifierValue))
                {
                    symbolValueMap.Add(value.getValue().ToString(), value);
                }
            }

            // Check for errors
            if (errors != log.errorCount())
            {
                throw new MibLoaderException(log);
            }
        }

        /**
         * Clears and prepares this MIB for garbage collection. This method
         * will recursively clear all associated symbols, making sure that
         * no data structures references symbols from this MIB. Obviously,
         * this method shouldn't be called unless all dependant MIBs have
         * been cleared first.
         */
        public void Clear()
        {
            loader = null;
            log = null;
            if (imports != null)
            {
                imports.Clear();
            }
            imports = null;
            if (symbolList != null)
            {
                foreach (MibSymbol symbol in symbolList)
                {
                    symbol.Clear();
                }
                symbolList.Clear();
            }
            symbolList = null;
            if (symbolNameMap != null)
            {
                symbolNameMap.Clear();
            }
            symbolNameMap = null;
            if (symbolValueMap != null)
            {
                symbolValueMap.Clear();
            }
            symbolValueMap = null;
        }

        /**
         * Compares this MIB to another object. This method will return
         * true if the object is a string containing the MIB name, a file
         * containing the MIB file, or a Mib having the same name.
         *
         * @param obj            the object to compare with
         *
         * @return true if the objects are equal, or
         *         false otherwise
         */
        public override bool Equals(Object obj)
        {
            if (obj is string)
            {
                return name.Equals(obj);
            }
            else if (obj is Mib)
            {
                return obj.Equals(name);
            }
            else
            {
                return false;
            }
        }

        /**
         * Returns the hash code value for the object. This method is
         * reimplemented to fulfil the contract of returning the same
         * hash code for objects that are considered equal.
         *
         * @return the hash code value for the object
         *
         * @since 2.6
         */
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /**
         * Checks if this MIB module has been explicitly loaded. A MIB
         * module is considered explicitly loaded if the file or resource
         * containing the MIB definition was loaded by a direct call to
         * the MIB loader. Implictly loaded MIB modules are loaded as a
         * result of import statements in explicitly loaded MIBs.
         *
         * @return true if this MIB module was explicitly loaded, or
         *         false otherwise
         *
         * @since 2.7
         */
        public bool Loaded
        {
            get
            {
                return loaded;
            }
            set
            {
                loaded = value;
            }
        }

        /**
         * Returns the MIB name. This is sometimes also referred to as
         * the MIB module name.
         *
         * @return the MIB name
         */
        public string getName()
        {
            return name;
        }

        /**
         * Changes the MIB name. This method should only be called by
         * the MIB analysis classes.
         *
         * @param name           the MIB name
         */
        public void setName(string name)
        {
            this.name = name;
            if (file == null)
            {
                file = name;
            }
        }

        /**
         * Returns the MIB file.
         *
         * @return the MIB file
         */
        public string getFile()
        {
            return file;
        }

        /**
         * Returns the MIB loader used when loading this MIB.
         *
         * @return the loader used
         */
        public MibLoader getLoader()
        {
            return loader;
        }

        /**
         * Returns the loader log used when loading this MIB.
         *
         * @return the loader log used
         */
        public MibLoaderLog getLog()
        {
            return log;
        }

        /**
         * Returns the SMI version used for defining this MIB. This
         * number can be either 1 (for SMIv1) or 2 (for SMIv2). It is set
         * based on which macros are used in the MIB file.
         *
         * @return the SMI version used for defining the MIB
         *
         * @since 2.6
         */
        public int getSmiVersion()
        {
            return smiVersion;
        }

        /**
         * Sets the SMI version used for defining this MIB. This method
         * should only be called by the MIB analysis classes.
         *
         * @param version        the new SMI version
         *
         * @since 2.6
         */
        public void SetSmiVersion(int version)
        {
            this.smiVersion = version;
        }

        /**
         * Returns the MIB file header comment.
         *
         * @return the MIB file header comment, or
         *         null if no comment was present
         *
         * @since 2.6
         */
        public string getHeaderComment()
        {
            return headerComment;
        }

        /**
         * Sets the MIB file header comment.
         *
         * @param comment        the MIB header comment
         *
         * @since 2.6
         */
        public void setHeaderComment(string comment)
        {
            this.headerComment = comment;
        }

        /**
         * Returns the MIB file footer comment.
         *
         * @return the MIB file footer comment, or
         *         null if no comment was present
         *
         * @since 2.6
         */
        public string getFooterComment()
        {
            return footerComment;
        }

        /**
         * Sets the MIB file footer comment.
         *
         * @param comment        the MIB footer comment
         *
         * @since 2.6
         */
        public void setFooterComment(string comment)
        {
            this.footerComment = comment;
        }

        /**
         * Returns all MIB import references.
         *
         * @return a collection of all imports
         *
         * @see MibImport
         *
         * @since 2.6
         */
        public IList<MibImport> getAllImports()
        {
            IList<MibImport> res = new List<MibImport>();

            foreach (MibImport imp in imports)
            {
                if (imp.HasSymbols())
                {
                    res.Add(imp);
                }
            }
            return res;
        }

        /**
         * Returns a MIB import reference.
         *
         * @param name           the imported MIB name
         *
         * @return the MIB import reference, or
         *         null if not found
         */
        public MibImport getImport(string name)
        {
            foreach (MibImport imp in imports)
            {
                if (imp.Name.Equals(name))
                {
                    return imp;
                }
            }
            return null;
        }

        /**
         * Adds a reference to an imported MIB file.
         *
         * @param ref            the reference to add
         */
        public void addImport(MibImport impRef)
        {
            imports.Add(impRef);
        }

        /**
         * Finds all MIB:s that are dependant on this one. The search
         * will iterate through all loaded MIB:s and return those that
         * import this one.
         *
         * @return the array of MIB:s importing this one
         *
         * @see MibLoader
         *
         * @since 2.7
         */
        public IList<Mib> getImportingMibs()
        {
            return loader.getAllMibs()
                .Where(m => m != this && m.getImport(name) != null)
                .ToList();
        }

        /**
         * Returns all symbols in this MIB.
         *
         * @return a collection of the MIB symbols
         *
         * @see MibSymbol
         */
        public IList<MibSymbol> getAllSymbols()
        {
            return symbolList;
        }

        /**
         * Returns a symbol from this MIB.
         *
         * @param name           the symbol name
         *
         * @return the MIB symbol, or null if not found
         */
        public MibSymbol getSymbol(string name)
        {
            MibSymbol ms;
            symbolNameMap.TryGetValue(name, out ms);
            return ms;
        }

        /**
         * Returns a value symbol from this MIB.
         *
         * @param value          the symbol value
         *
         * @return the MIB value symbol, or null if not found
         */
        public MibValueSymbol getSymbolByValue(string value)
        {
            return symbolValueMap[value];
        }

        /**
         * Returns a value symbol from this MIB.
         *
         * @param value          the symbol value
         *
         * @return the MIB value symbol, or null if not found
         */
        public MibValueSymbol getSymbolByValue(MibValue value)
        {
            return symbolValueMap[value.ToString()];
        }

        /**
         * Returns a value symbol from this MIB. The search is performed
         * by using the strictly numerical OID value specified. Differing
         * from the getSymbolByValue() methods, this method may return a
         * symbol with only a partial OID match. If an exact match for
         * the OID is present in the MIB, this method will always return
         * the same result as getSymbolByValue(). Otherwise, the symbol
         * with the longest matching OID will be returned, making it
         * possible to identify a MIB symbol from an OID containing table
         * row indices or similar.
         *
         * @param oid            the numeric OID value
         *
         * @return the MIB value symbol, or null if not found
         *
         * @since 2.5
         */
        public MibValueSymbol getSymbolByOid(string oid)
        {
            MibValueSymbol sym;
            int pos;

            do
            {
                sym = getSymbolByValue(oid);
                if (sym != null)
                {
                    return sym;
                }
                pos = oid.LastIndexOf(".");
                if (pos > 0)
                {
                    oid = oid.Substring(0, pos);
                }
            } while (pos > 0);
            return null;
        }

        /**
         * Returns the root MIB value symbol. This value symbol is
         * normally the module identifier (in SMIv2), but may also be
         * just the base object identifier in the MIB.
         *
         * @return the root MIB value symbol
         *
         * @since 2.6
         */
        public MibValueSymbol getRootSymbol()
        {
            MibValueSymbol root = null;
            MibValueSymbol parent;

            foreach(MibSymbol m in symbolList)
            {
                root = m as MibValueSymbol;
                if(root != null)
                {
                    break;
                }
            }

            while (root != null && (parent = root.getParent()) != null)
            {
                if (!root.getMib().Equals(parent.getMib()))
                {
                    break;
                }
                root = parent;
            }
            return root;
        }

        /**
         * Adds a symbol to this MIB.
         *
         * @param symbol         the symbol to add
         */
        public void addSymbol(MibSymbol symbol)
        {
            symbolList.Add(symbol);
            symbolNameMap.Add(symbol.getName(), symbol);
        }

        /**
         * Searches for a named MIB symbol. This method is required to
         * implement the MibContext interface but returns the same results
         * as getSymbol(String).<p>
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
        public MibSymbol findSymbol(string name, bool expanded)
        {
            return getSymbol(name);
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            return getName();
        }
    }

}
