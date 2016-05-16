//
// MibDirectoryCache.cs
// 
// This work is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; either version 2 of the License,
// or (at your option) any later version.
//
// This work is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
// USA
// 
// Original Java code Copyright (c) 2004-2016 Per Cederberg. All
// rights reserved.
// C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace MibbleSharp
{
    /**
     * A MIB search directory cache. This class attempts to map MIB names
     * to files for a single directory. It keeps two internal caches; one
     * based on the file name similarity with MIB names, and one based on
     * the content of the first few lines in each file. Each of these
     * caches are created upon request and the content cache is normally
     * used only as a secondary alternative due to the performance
     * penalty of its creation.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.9
     */
    class MibDirectoryCache
    {

        /**
         * The MIB name regular expression pattern.
         */
        private static readonly Regex NAME = new Regex(@"[a-zA-Z][a-zA-Z0-9-_]*");

    /**
     * The directory to search.
     */
    private string dir;

        /**
         * The file name cache. This cache is indexed by upper-case MIB
         * name and links to the directory file.
         */
        private Dictionary<string, string> nameCache = null;

        /**
         * The content cache. This cache is indexed by the actual MIB
         * name read from the file and links to the directory file.
         */
        private Dictionary<string, string> contentCache = null;

        /**
         * Creates a new MIB search directory cache.
         *
         * @param dir            the directory to index
         */
        public MibDirectoryCache(string dir)
        {
            this.dir = dir;
        }

        /**
         * Returns the directory indexed by this cache.
         *
         * @return the directory indexed by this cache
         */
        public string Dir
        {
            get
            {
                return dir;
            }
        }

        /**
         * Searches for a named MIB in the directory file name cache.
         * Note that there are no guarantees that the returned file is
         * indeed a MIB file.
         *
         * @param mibName        the MIB name
         *
         * @return the first matching MIB file,
         *         null if no match was found
         */
        public string findByName(string mibName)
        {
            if (nameCache == null)
            {
                initNameCache();
            }
            string mibNameUpper = mibName.ToUpper();
            if(nameCache.ContainsKey(mibNameUpper))
                return nameCache[mibNameUpper];
            return null;
        }

        /**
         * Initializes the name cache.
         */
        private void initNameCache()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dir);
            string name;

            nameCache = new Dictionary<string, string>();

            foreach(string file in files)
            {
                name = Path.GetFileName(file);
                Match m = NAME.Match(name);
                if (m.Success)
                {
                    nameCache[m.Value.ToUpper()] = file;
                }
            }
        }

        /**
         * Searches for a named MIB in the directory content cache. This
         * cache is costly to initialize, but since it is based on the
         * actual content of the first few lines in each file it is more
         * accurate.
         *
         * @param mibName        the MIB name
         *
         * @return the first matching MIB file,
         *         null if no match was found
         */
        public string findByContent(string mibName)
        {
            if (contentCache == null)
            {
                initContentCache();
            }
            if(contentCache.ContainsKey(mibName))
                return contentCache[mibName];
            return null;
        }

        /**
         * Initializes the content cache.
         */
        private void initContentCache()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dir);
            string name;

            contentCache = new Dictionary<string, string>();
            foreach(string file in files)
            {
                name = readMibName(file);
                if (name != null)
                {
                    contentCache[name] = file;
                }
            }
        }

        /**
         * Reads the initial lines of a supposed text file attempting to
         * extract a MIB name.
         *
         * @param file           the file to read
         *
         * @return the MIB name found, or
         *         null if no name was found
         */
        private string readMibName(string file)
        {
            //BufferedReader  in = null;
            //string str;
            //Matcher m;

            if (File.Exists(file) == false)
            {
                return null;
            }


            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (!line.Equals("") && !line.StartsWith("--"))
                        {
                            Match m = NAME.Match(line);
                            return m.Success ? m.Value : null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Do nothing
            }
            
            return null;
        }
    }

}
