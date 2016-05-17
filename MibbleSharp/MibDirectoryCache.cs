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
using System.Diagnostics.CodeAnalysis;

namespace MibbleSharp
{
    /// <summary>
    /// A MIB search directory cache. This class attempts to map MIB names
    /// to files for a single directory.It keeps two internal caches; one
    /// based on the file name similarity with MIB names, and one based on
    /// the content of the first few lines in each file.Each of these
    /// caches are created upon request and the content cache is normally
    /// used only as a secondary alternative due to the performance
    /// penalty of its creation.
    /// </summary>
    class MibDirectoryCache
    {

        /// 
        /// <summary>
        /// The regex pattern for a MIB name
        /// </summary>
        /// 
        private static readonly Regex NAME = new Regex(@"[a-zA-Z][a-zA-Z0-9-_]*");

        /// 
        /// <summary>
        /// The directory to search in
        /// </summary>
        /// 
        private string dir;

        /// 
        /// <summary>
        /// The file name cache. This cache is indexed by upper-case MIB
        /// name and links to the directory file.
        /// </summary>
        /// 
        private Dictionary<string, string> nameCache = null;

        /// 
        /// <summary>
        /// The content cache. This cache is indexed by the actual MIB
        /// name read from the file and links to the directory file.
        /// </summary>
        /// 
        private Dictionary<string, string> contentCache = null;

        /// 
        /// <summary>
        /// Creates a new MIB search directory cache.
        /// </summary>
        /// <param name="dir">The directory to search in</param>
        /// 
        public MibDirectoryCache(string dir)
        {
            this.dir = dir;
        }

        /// <summary>
        /// The directory indexed by this cache
        /// </summary>
        public string Dir
        {
            get
            {
                return dir;
            }
        }

        /// 
        /// <summary>
        /// Searches for a named MIB in the directory file name cache.
        /// Note that there are no guarantees that the returned file is
        /// indeed a MIB file.
        /// </summary>
        /// <param name="mibName">The MIB name to be searched for</param>
        /// <returns>The filename, or null if not found</returns>
        /// 
        public string FindByName(string mibName)
        {
            if (nameCache == null)
            {
                InitNameCache();
            }
            string mibNameUpper = mibName.ToUpper();
            if(nameCache.ContainsKey(mibNameUpper))
                return nameCache[mibNameUpper];
            return null;
        }

        /// 
        /// <summary>
        /// Initialize the name cache. Enumerates all files in directory dir.
        /// If the name is compatible with a MIB file name, the file is added 
        /// to the cache.
        /// </summary>
        /// 
        private void InitNameCache()
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

        /// 
        /// <summary>
        /// Searches for a named MIB in the directory content cache. This
        /// cache is costly to initialize, but since it is based on the
        /// actual content of the first few lines in each file it is more
        /// accurate.
        /// </summary>
        /// <param name="mibName">The name to be searched for</param>
        /// <returns>
        /// The first matching MIB file, null if no match was found
        /// </returns>
        /// 
        public string FindByContent(string mibName)
        {
            if (contentCache == null)
            {
                InitContentCache();
            }
            if(contentCache.ContainsKey(mibName))
                return contentCache[mibName];
            return null;
        }

        /// 
        /// <summary>
        /// Initialize the content cache. Enumerates the files in the directory
        /// and calls ReadMibName on each one to attempt to read a MIB name
        /// </summary>
        /// 
        private void InitContentCache()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dir);
            string name;

            contentCache = new Dictionary<string, string>();
            foreach(string file in files)
            {
                name = ReadMibName(file);
                if (name != null)
                {
                    contentCache[name] = file;
                }
            }
        }

        /// 
        /// <summary>
        /// Reads the initial lines of a supposed text file attempting to
        /// extract a MIB name.
        /// </summary>
        /// <param name="file">The presumed MIB file to be read</param>
        /// <returns>the MIB name found, or null if no name was found</returns>
        /// 
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Correct IDisposable implementation will work with multiple calls")]
        private string ReadMibName(string file)
        {
            if (File.Exists(file) == false)
            {
                return null;
            }

            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
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
            }
            catch (Exception)
            {
                // Do nothing
            }
            
            return null;
        }
    }

}
