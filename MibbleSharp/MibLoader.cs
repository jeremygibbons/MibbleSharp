//
// MibLoader.cs
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
using System.IO;
using System.Linq;
using MibbleSharp.Value;
using MibbleSharp.Asn1;
using PerCederberg.Grammatica.Runtime;

namespace MibbleSharp
{

    /**
     * A MIB loader. This class contains a search path for locating MIB
     * files, and also holds a refererence to previously loaded MIB files
     * to avoid loading the same file multiple times. The MIB search path
     * consists of directories with MIB files that can be imported into
     * other MIBs. The search path directories can either be normal file
     * system directories or resource directories. By default the search
     * path contains resource directories containing standard IANA and
     * IETF MIB files (packaged in the Mibble JAR file).<p>
     *
     * The MIB loader searches for MIB files in a specific order. First,
     * the file system directories in the search path are scanned for
     * files with the same name as the MIB module being imported. This
     * search ignores any file name extensions and compares the base file
     * name in case-insensitive mode. If this search fails, the resource
     * directories are searched for a file having the exact name of the
     * MIB module being imported (case sensitive). The last step, if both
     * the previous ones have failed, is to open the files in the search
     * path one by one to check the MIB module names specified inside.
     * Note that this may be slow for large directories with many files,
     * and it is therefore recommended to always name the MIB files
     * according to their module name.<p>
     *
     * The MIB loader is not thread-safe, i.e. it cannot be used
     * concurrently in multiple threads.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    public class MibLoader
    {

        /**
         * The MIB file directory caches. This is also a list of the MIB
         * file search path, as each directory on the path has its own
         * cache. If a MIB isn't found among these directories, the
         * resource directories will be attempted.
         */
        private IList<MibDirectoryCache> dirCaches = new List<MibDirectoryCache>();

        /**
         * The MIB files loaded. This list contains all MIB file loaded
         * with this loader, in order to avoid loading some MIB files
         * multiple times (and thus duplicating import symbols).
         */
        private IList<Mib> mibs = new List<Mib>();

        /**
         * The queue of MIB files to load. This queue contains either
         * MIB module names or MibSource objects.
         */
        private IList<MibSource> sourceQueue = new List<MibSource>();
        private IList<string> nameQueue = new List<string>();
        /**
         * The default MIB context.
         */
        private DefaultContext context = new DefaultContext();

        /**
         * Creates a new MIB loader.
         */
        public MibLoader()
        {
            addDir("mibs/iana");
            addDir("mibs/ietf");
        }

        /**
         * Checks if a directory is in the MIB search path. If a file is
         * specified instead of a directory, this method checks if the
         * parent directory is in the MIB search path.
         *
         * @param dir            the directory or file to check
         *
         * @return true if the directory is in the MIB search path, or
         *         false otherwise
         *
         * @since 2.9
         */
        public bool hasDir(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }
            else if (File.Exists(dir))
            {
                //it's a file, so get it's directory
                dir = Directory.GetParent(dir).FullName;
            }

            foreach (MibDirectoryCache cache in dirCaches)
            {
                if (cache.Dir.Equals(dir))
                {
                    return true;
                }
            }
            return false;
        }

        /**
         * Returns all the directories in the MIB search path. If a tree
         * of directories has been added, all the individual directories
         * will be returned by this method.
         *
         * @return the directories in the MIB search path
         *
         * @since 2.9
         */
        public IList<string> getDirs()
        {
            return dirCaches.Select(d => d.Dir).ToList();
        }

        /**
         * Adds a directory to the MIB search path. If the directory
         * specified is null, the current working directory will be added.
         *
         * @param dir            the directory to add
         */
        public void addDir(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }
            if (!hasDir(dir) && Directory.Exists(dir))
            {
                dirCaches.Add(new MibDirectoryCache(dir));
            }
        }

        /**
         * Adds directories to the MIB search path.
         *
         * @param dirs           the directories to add
         */
        public void addDirs(IEnumerable<string> dirs)
        {
            foreach (string d in dirs)
            {
                addDir(d);
            }
        }

        /**
         * Adds a directory and all subdirectories to the MIB search path.
         * If the directory specified is null, the current working
         * directory (and subdirectories) will be added.
         *
         * @param dir            the directory to add
         */
        public void addAllDirs(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }
            addDir(dir);
            IEnumerable<string> subDirs = Directory.EnumerateDirectories(dir);
            foreach (string subDir in subDirs)
            {
                addAllDirs(subDir);
            }

        }

        /**
         * Removes a directory from the MIB search path.
         *
         * @param dir            the directory to remove
         */
        public void removeDir(string dir)
        {
            MibDirectoryCache cache;

            for (int i = 0; i < dirCaches.Count; i++)
            {
                cache = dirCaches[i];
                if (cache.Dir.Equals(dir))
                {
                    dirCaches.Remove(cache);
                }
            }
        }

        /**
         * Removes all directories from the MIB search path.
         */
        public void removeAllDirs()
        {
            dirCaches.Clear();
        }


        /**
         * Resets this loader. This means that all references to previuos
         * MIB files will be removed, forcing a reload of any imported
         * MIB.<p>
         *
         * Note that this is not the same operation as unloadAll, since
         * the MIB files previously loaded will be unaffected by this
         * this method (i.e. they remain possible to use). If the purpose
         * is to free all memory used by the loaded MIB files, use the
         * unloadAll() method instead.
         *
         * @see #unloadAll()
         */
        public void reset()
        {
            mibs.Clear();
            sourceQueue.Clear();
            nameQueue.Clear();
            context = new DefaultContext();
        }

        /**
         * Returns the default MIB context. This context contains the
         * symbols that are predefined for all MIB:s (such as 'iso').
         *
         * @return the default MIB context
         */
        public MibContext getDefaultContext()
        {
            return context;
        }

        /**
         * Returns the root object identifier value (OID). This OID is
         * the "iso" symbol.
         *
         * @return the root object identifier value
         *
         * @since 2.7
         */
        public ObjectIdentifierValue getRootOid()
        {
            MibSymbol symbol;
            MibValue value;

            symbol = context.FindSymbol(DefaultContext.ISO, false);
            value = ((MibValueSymbol)symbol).getValue();
            return (ObjectIdentifierValue)value;
        }

        /**
         * Returns a previously loaded MIB file. If the MIB file hasn't
         * been loaded, null will be returned. The MIB is identified by
         * it's MIB name (i.e. the module name).
         *
         * @param name           the MIB (module) name
         *
         * @return the MIB module if found, or
         *         null otherwise
         */
        public Mib getMib(string name)
        {
            foreach (Mib mib in mibs)
            {
                if (mib.Equals(name))
                {
                    return mib;
                }
            }
            return null;
        }

        /**
         * Returns all previously loaded MIB files. If no MIB files have
         * been loaded an empty array will be returned.
         *
         * @return an array with all loaded MIB files
         *
         * @since 2.2
         */
        public IEnumerable<Mib> getAllMibs()
        {
            return mibs;
        }

        /**
         * Loads a MIB file with the specified base name. The file is
         * searched for in the MIB search path. The MIB is identified by
         * it's MIB name (i.e. the module name). This method will also
         * load all imported MIB:s if not previously loaded by this
         * loader. If a MIB with the same name has already been loaded, it
         * will be returned directly instead of reloading it.
         *
         * @param name           the MIB name (filename without extension)
         *
         * @return the MIB module loaded
         *
         * @throws IOException if the MIB file couldn't be found in the
         *             MIB search path
         * @throws MibLoaderException if the MIB file couldn't be loaded
         *             correctly
         */
        public Mib Load(string name)
        {
            MibSource src;
            Mib mib;

            mib = getMib(name);
            if (mib == null)
            {
                src = locate(name);
                if (src == null)
                {
                    throw new FileNotFoundException("couldn't locate MIB: '" +
                                                    name + "'");
                }
                mib = load(src);
            }
            else
            {
                mib.Loaded = true;
            }
            return mib;
        }



        /**
         * Loads a MIB file from the specified URL. This method will also
         * load all imported MIB:s if not previously loaded by this
         * loader. Note that if the URL data contains several MIB modules,
         * this method will only return the first one (although all are
         * loaded).
         *
         * @param url            the URL containing the MIB
         *
         * @return the first MIB module loaded
         *
         * @throws IOException if the MIB URL couldn't be read
         * @throws MibLoaderException if the MIB file couldn't be loaded
         *             correctly
         *
         * @since 2.3
         */
        public Mib load(Uri url)
        {
            return load(new MibSource(url));
        }

        /**
         * Loads a MIB file from the specified input reader. This method
         * will also load all imported MIB:s if not previously loaded by
         * this loader. Note that if the input data contains several MIB
         * modules, this method will only return the first one (although
         * all are loaded).
         *
         * @param input          the input stream containing the MIB
         *
         * @return the first MIB module loaded
         *
         * @throws IOException if the input stream couldn't be read
         * @throws MibLoaderException if the MIB file couldn't be loaded
         *             correctly
         *
         * @since 2.3
         */
        public Mib load(StreamReader input)
        {
            return load(new MibSource(input));
        }

        /**
         * Loads a MIB. This method will also load all imported MIB:s if
         * not previously loaded by this loader. Note that if the source
         * contains several MIB modules, this method will only return the
         * first one (although all are loaded).
         *
         * @param src            the MIB source
         *
         * @return the first MIB module loaded
         *
         * @throws IOException if the MIB couldn't be found
         * @throws MibLoaderException if the MIB couldn't be loaded
         *             correctly
         */
        private Mib load(MibSource src)
        {
            MibLoaderLog log;

            sourceQueue.Clear();
            sourceQueue.Add(src);

            log = loadQueue();
            if (log.ErrorCount > 0)
            {
                throw new MibLoaderException(log);
            }

            return mibs.Last();
        }

        /**
         * Unloads a MIB. This method will remove the loader reference to
         * a previously loaded MIB if no other MIBs are depending on it.
         * This method attempts to free the memory used by the MIB, as it
         * clears both the loader and internal MIB references to the data
         * structures (thereby allowing the garbage collector to recover
         * the memory used if no other references exist). Other MIB:s
         * should be unaffected by this operation.
         *
         * @param name           the MIB name
         *
         * @throws MibLoaderException if the MIB couldn't be unloaded
         *             due to dependencies from other loaded MIBs
         *
         * @see #reset
         *
         * @since 2.3
         */
        public void unload(string name)
        {
            foreach (Mib mib in mibs)
            {
                if (mib.Equals(name))
                {
                    unload(mib);
                    return;
                }
            }
        }

        /**
         * Unloads a MIB. This method will remove the loader reference to
         * a previously loaded MIB if no other MIBs are depending on it.
         * This method attempts to free the memory used by the MIB, as it
         * clears both the loader and internal MIB references to the data
         * structures (thereby allowing the garbage collector to recover
         * the memory used if no other references exist). Other MIB:s
         * should be unaffected by this operation.
         *
         * @param mib            the MIB
         *
         * @throws MibLoaderException if the MIB couldn't be unloaded
         *             due to dependencies from other loaded MIBs
         *
         * @see #reset
         *
         * @since 2.3
         */
        public void unload(Mib mib)
        {
            string message;

            if (mibs.Contains(mib))
            {

                var referers = mib.getImportingMibs();
                if (referers.Count > 0)
                {
                    message = "cannot be unloaded due to reference in " +
                              referers[0];
                    throw new MibLoaderException(mib.getFile(), message);
                }
                mibs.Remove(mib);
                mib.Clear();
            }
        }

        /**
         * Unloads all MIBs loaded by this loaded (since the last reset).
         * This method attempts to free all the memory used by the MIBs,
         * as it clears both the loader and internal MIB references to
         * the data structures (thereby allowing the garbage collector to
         * recover the memory used if no other references exist). Note
         * that no previous MIBs returned by this loader should be
         * accessed after this method has been called.<p>
         *
         * In order to just reset the MIB loader to force re-loading of
         * MIB files, use the reset() method instead which will leave the
         * MIBs unaffected.
         *
         * @see #reset()
         * @since 2.9
         * 
         */
        public void unloadAll()
        {
            foreach (Mib mib in mibs)
                mib.Clear();

            reset();
        }

        /**
         * Schedules the loading of a MIB file. The file is added to the
         * queue of MIB files to be loaded, unless it is already loaded
         * or in the queue. The MIB file search is postponed until the
         * MIB is to be loaded, avoiding loading if the MIB name was
         * defined in another MIB file in the queue.
         *
         * @param name           the MIB name (filename without extension)
         */
        public void scheduleLoad(string name)
        {
            if (getMib(name) == null && !nameQueue.Contains(name))
            {
                nameQueue.Add(name);
            }
        }

        /**
         * Loads all MIB files in the loader queue. New entries may be
         * added to the queue while loading a MIB, as a result of
         * importing other MIB files. This method will either load all
         * MIB files in the queue or none (if errors were encountered).
         *
         * @return the loader log for the whole queue
         *
         * @throws IOException if the MIB file in the queue couldn't be
         *             found
         */
        private MibLoaderLog loadQueue()
        {
            MibLoaderLog log = new MibLoaderLog();
            IList<Mib> processed = new List<Mib>();

            IList<Mib> list;

            foreach (var msrc in sourceQueue)
            {
                if (getMib(msrc.getFile()) == null)
                {
                    list = msrc.parseMib(this, log);
                    foreach (var mib in list)
                    {
                        mib.Loaded = true;
                    }
                    mibs = mibs.Concat(list).ToList();
                    processed = processed.Concat(list).ToList();
                }
            }
            sourceQueue.Clear();

            foreach (var name in nameQueue)
            {
                MibSource src = locate(name);
                if (src == null)
                    continue;

                if (getMib(src.getFile()) == null)
                {
                    list = src.parseMib(this, log);
                    foreach (var mib in list)
                    {
                        mib.Loaded = false;
                    }
                    mibs = mibs.Concat(list).ToList();
                    processed = processed.Concat(list).ToList();
                }
            }
            nameQueue.Clear();



            // Initialize all parsed MIB files in reverse order
            foreach (var mib in processed.Reverse())
            {
                try
                {
                    mib.Initialize();
                }
                catch (MibLoaderException)
                {
                    // Do nothing, errors are already in the log
                }
            }

            // Validate all parsed MIB files in reverse order
            foreach (var mib in processed.Reverse())
            {
                try
                {
                    mib.validate();
                }
                catch (MibLoaderException)
                {
                    // Do nothing, errors are already in the log
                }
            }

            // Handle errors
            if (log.ErrorCount > 0)
            {
                foreach (var mib in processed)
                {
                    mibs.Remove(mib);
                }
            }

            return log;
        }

        /**
         * Searches for a MIB in the search path. The name specified
         * should be the MIB name. If a matching file name isn't found in
         * the directory search path, the contents in the base resource
         * path are also tested. Finally, if no MIB file has been found,
         * the files in the search path will be opened regardless of file
         * name to perform a small heuristic test for the MIB in question.
         *
         * @param name           the MIB name
         *
         * @return the MIB found, or
         *         null if no MIB was found
         */
        private MibSource locate(string name)
        {
            string file;

            foreach (var dir in dirCaches)
            {
                file = dir.FindByName(name);
                if (file != null)
                    return new MibSource(file);

                file = dir.FindByContent(name);
                if (file != null)
                    return new MibSource(file);
            }

            return null;
        }

        /**
         * A MIB input source. This class encapsulates the two different
         * ways of locating a MIB file, either through a file or a URL.
         */
        private class MibSource
        {

            /**
             * The singleton ASN.1 parser used by all MIB sources.
             */
            private static Asn1Parser parser = null;

            /**
             * The MIB file. This variable is only set if the MIB is read
             * from file, or if the MIB name is known.
             */
            private string file = null;

            /**
             * The MIB URL location. This variable is only set if the MIB
             * is read from a URL.
             */
            private Uri url = null;

            /**
             * The MIB reader. This variable is only set if the MIB
             * is read from an input stream.
             */
            private System.IO.TextReader input = null;

            /**
             * Creates a new MIB input source. The MIB will be read from
             * the specified file.
             *
             * @param file           the file to read from
             */
            public MibSource(string file)
            {
                this.file = file;
            }

            /**
             * Creates a new MIB input source. The MIB will be read from
             * the specified URL.
             *
             * @param url            the URL to read from
             */
            public MibSource(Uri url)
            {
                this.url = url;
            }

            /**
             * Creates a new MIB input source. The MIB will be read from
             * the specified URL. This method also create a default file
             * from the specified MIB name in order to improve possible
             * error messages.
             *
             * @param name           the MIB name
             * @param url            the URL to read from
             */
            public MibSource(string name, Uri url) : this(url)
            {
                this.file = name;
            }

            /**
             * Creates a new MIB input source. The MIB will be read from
             * the specified input reader. The input reader will be closed
             * after reading the MIB.
             *
             * @param input          the input stream to read from
             */
            public MibSource(TextReader input)
            {
                this.input = input;
            }

            /**
             * Checks if this object is equal to another. This method
             * will only return true for another mib source object with
             * the same input source.
             *
             * @param obj            the object to compare with
             *
             * @return true if the object is equal to this, or
             *         false otherwise
             */
            public override bool Equals(Object obj)
            {
                MibSource src = obj as MibSource;

                if (src == null)
                    return false;

                if (url != null)
                {
                    return url.Equals(src.url);
                }
                else if (file != null)
                {
                    return file.Equals(src.file);
                }

                return false;
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
                if (url != null)
                {
                    return url.GetHashCode();
                }
                else if (file != null)
                {
                    return file.GetHashCode();
                }
                else
                {
                    return base.GetHashCode();
                }
            }

            /**
             * Returns the MIB file. If the MIB is loaded from URL this
             * file does not actually exist, but is used for providing a
             * unique reference to the MIB.
             *
             * @return the MIB file
             */
            public string getFile()
            {
                return file;
            }

            /**
             * Parses the MIB input source and returns the MIB modules
             * found. This method will read the MIB either from file, URL
             * or input stream.
             *
             * @param loader         the MIB loader to use for imports
             * @param log            the MIB log to use for errors
             *
             * @return the list of MIB modules created
             *
             * @throws IOException if the MIB couldn't be found
             * @throws MibLoaderException if the MIB couldn't be parsed
             *             or analyzed correctly
             */
            public IList<Mib> parseMib(MibLoader loader, MibLoaderLog log)
            {

                MibAnalyzer analyzer;
                string msg;
                string result = "";
                // Open input stream
                if (input != null)
                {
                    result = input.ReadToEnd();
                }
                else if (url != null)
                {
                    using (var client = new System.Net.WebClient())
                    {
                        result = client.DownloadString(url);
                    }
                }
                else
                {
                    using (TextReader reader = File.OpenText(file))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                using (input = new StringReader(result))
                {
                    // Parse input stream
                    analyzer = new MibAnalyzer(file, loader, log);
                    try
                    {
                        if (parser == null)
                        {
                            parser = new Asn1Parser(input, analyzer);
                            parser.GetTokenizer().UseTokenList = true;
                        }
                        else
                        {
                            parser.Reset(input, analyzer);
                        }
                        parser.Parse();
                        return analyzer.getMibs().ToList();
                    }
                    catch (ParserCreationException e)
                    {
                        msg = "parser creation error in ASN.1 parser: " +
                              e.Message;
                        log.AddInternalError(file, msg);
                        analyzer.Reset();
                        throw new MibLoaderException(log);
                    }
                    catch (ParserLogException e)
                    {
                        log.AddAll(file, e);
                        analyzer.Reset();
                        throw new MibLoaderException(log);
                    }
                }
            }
        }
    }
}
