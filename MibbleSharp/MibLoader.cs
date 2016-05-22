// <copyright file="MibLoader.cs" company="None">
//    <para>
//    This work is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published
//    by the Free Software Foundation; either version 2 of the License,
//    or (at your option) any later version.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    General Public License for more details.</para>
//    <para>
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MibbleSharp.Asn1;
    using MibbleSharp.Value;
    using PerCederberg.Grammatica.Runtime;

    /// <summary>
    /// <para>
    /// A MIB loader. This class contains a search path for locating MIB
    /// files, and also holds a reference to previously loaded MIB files
    /// to avoid loading the same file multiple times. The MIB search path
    /// consists of directories with MIB files that can be imported into
    /// other MIBs. The search path directories can either be normal file
    /// system directories or resource directories. By default the search
    /// path contains resource directories containing standard IANA and
    /// IETF MIB files (packaged in the Mibble JAR file).
    /// </para><para>
    /// The MIB loader searches for MIB files in a specific order. First,
    /// the file system directories in the search path are scanned for
    /// files with the same name as the MIB module being imported. This
    /// search ignores any file name extensions and compares the base file
    /// name in case-insensitive mode. If this search fails, the resource
    /// directories are searched for a file having the exact name of the
    /// MIB module being imported (case sensitive). The last step, if both
    /// the previous ones have failed, is to open the files in the search
    /// path one by one to check the MIB module names specified inside.
    /// Note that this may be slow for large directories with many files,
    /// and it is therefore recommended to always name the MIB files
    /// according to their module name.
    /// </para><para>
    /// The MIB loader is not thread-safe, i.e. it cannot be used
    /// concurrently in multiple threads.
    /// </para>
    /// </summary>
    public class MibLoader
    {
        /// <summary>
        /// The MIB file directory caches. This is also a list of the MIB
        /// file search path, as each directory on the path has its own
        /// cache. If a MIB isn't found among these directories, the
        /// resource directories will be attempted.
        /// </summary>
        private IList<MibDirectoryCache> dirCaches = new List<MibDirectoryCache>();

        /// <summary>
        /// The MIB files that have been loaded. This list contains all MIB file loaded
        /// with this loader, in order to avoid loading some MIB files
        /// multiple times (and thus duplicating import symbols).
        /// </summary>
        private IList<Mib> mibs = new List<Mib>();

        /// <summary>
        /// The queue of MIB files to load. This queue contains
        /// MibSource objects.
        /// </summary>
        private IList<MibSource> sourceQueue = new List<MibSource>();
        
        /// <summary>
        /// The queue of MIB files to load. This queue contains
        /// MIB module names.
        /// </summary>
        private IList<string> nameQueue = new List<string>();

        /// <summary>
        /// The default MIB context.
        /// </summary>
        private DefaultContext context = new DefaultContext();

        /// <summary>
        /// Initializes a new instance of the <see cref="MibLoader"/> class.
        /// </summary>
        public MibLoader()
        {
            this.AddDir("mibs/iana");
            this.AddDir("mibs/ietf");
        }

        /// <summary>
        /// Gets all the directories in the MIB search path. If a tree
        /// of directories has been added, all the individual directories
        /// will be returned by this method.
        /// </summary>
        public IList<string> Dirs
        {
            get
            {
                return this.dirCaches.Select(d => d.Dir).ToList();
            }
        }

        /// <summary>
        /// Gets the default MIB context. This context contains the
        /// symbols that are predefined for all MIB:s (such as 'iso').
        /// </summary>
        public IMibContext DefaultContext
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>Gets the root object identifier value (OID). This OID is
        /// the "iso" symbol.
        /// </summary>
        public ObjectIdentifierValue RootOid
        {
            get
            {
                MibSymbol symbol;
                MibValue value;

                symbol = this.context.FindSymbol(MibbleSharp.DefaultContext.ISO, false);
                value = ((MibValueSymbol)symbol).Value;
                return (ObjectIdentifierValue)value;
            }
        }

        /// <summary>
        /// Gets all previously loaded MIB files. If no MIB files have
        /// been loaded an empty array will be returned.
        /// </summary>
        public IEnumerable<Mib> AllMibs
        {
            get
            {
                return this.mibs;
            }
        }

        /// <summary>
        /// Checks if a directory is in the MIB search path. If a file is
        /// specified instead of a directory, this method checks if the
        /// parent directory is in the MIB search path.
        /// </summary>
        /// <param name="dir">The directory or file to check</param>
        /// <returns>
        /// True if the directory is in the MIB search path, false if not
        /// </returns>
        public bool HasDir(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }
            else if (File.Exists(dir))
            {
                // It's a file, so get its containing directory
                dir = Directory.GetParent(dir).FullName;
            }

            foreach (MibDirectoryCache cache in this.dirCaches)
            {
                if (cache.Dir.Equals(dir))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a directory to the MIB search path. If the directory
        /// specified is null, the current working directory will be added.
        /// </summary>
        /// <param name="dir">The directory to add</param>
        public void AddDir(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }

            if (!this.HasDir(dir) && Directory.Exists(dir))
            {
                this.dirCaches.Add(new MibDirectoryCache(dir));
            }
        }

        /// <summary>
        /// Adds directories to the MIB search path.
        /// </summary>
        /// <param name="dirs">The directories to add</param>
        public void AddDirs(IEnumerable<string> dirs)
        {
            foreach (string d in dirs)
            {
                this.AddDir(d);
            }
        }

        /// <summary>
        /// Adds a directory and all subdirectories to the MIB search path.
        /// If the directory specified is null, the current working
        /// directory (and subdirectories) will be added.
        /// </summary>
        /// <param name="dir">The directory to add</param>
        public void AddAllDirs(string dir)
        {
            if (dir == null)
            {
                dir = Directory.GetCurrentDirectory();
            }

            this.AddDir(dir);

            IEnumerable<string> subDirs = Directory.EnumerateDirectories(dir);
            foreach (string subDir in subDirs)
            {
                this.AddAllDirs(subDir);
            }
        }

        /// <summary>
        /// Removes a directory from the MIB search path.
        /// </summary>
        /// <param name="dir">The directory to remove</param>
        public void RemoveDir(string dir)
        {
            foreach (var cache in this.dirCaches)
            {
                if (cache.Dir.Equals(dir))
                {
                    this.dirCaches.Remove(cache);
                }
            }
        }

        /// <summary>
        /// Removes all directories from the MIB search path.
        /// </summary>
        public void RemoveAllDirs()
        {
            this.dirCaches.Clear();
        }

        /// <summary><para>
        /// Resets this loader. This means that all references to previous
        /// MIB files will be removed, forcing a reload of any imported
        /// MIB.</para>
        /// <para>
        /// Note that this is not the same operation as unloadAll, since
        /// the MIB files previously loaded will be unaffected by this
        /// this method (i.e. they remain possible to use). If the purpose
        /// is to free all memory used by the loaded MIB files, use the
        /// unloadAll() method instead.</para>
        /// </summary>
        /// <see cref="UnloadAll"/>
        public void Reset()
        {
            this.mibs.Clear();
            this.sourceQueue.Clear();
            this.nameQueue.Clear();
            this.context = new DefaultContext();
        }

        /// <summary>
        /// Returns a previously loaded MIB file. If the MIB file hasn't
        /// been loaded, null will be returned. The MIB is identified by
        /// it's MIB name (i.e. the module name).
        /// </summary>
        /// <param name="name">The MIB (module) name</param>
        /// <returns>The MIB module if found, null if not</returns>
        public Mib GetMib(string name)
        {
            return this.mibs.Where(m => m.Equals(name)).FirstOrDefault();
        }

        /// <summary>Loads a MIB file with the specified base name. The file is
        /// searched for in the MIB search path. The MIB is identified by
        /// it's MIB name (i.e. the module name). This method will also
        /// load all imported MIB:s if not previously loaded by this
        /// loader. If a MIB with the same name has already been loaded, it
        /// will be returned directly instead of reloading it.
        /// </summary>
        /// <param name="name">The MIB name (filename without extension)</param>
        /// <returns>The MIB module that was loaded</returns>
        /// <exception cref="IOException">
        /// If the MIB file couldn't be found in the MIB search path
        /// </exception>
        /// <exception cref="MibLoaderException">
        /// If the MIB file couldn't be loaded correctly
        /// </exception>
        public Mib Load(string name)
        {
            MibSource src;
            Mib mib;

            mib = this.GetMib(name);
            if (mib == null)
            {
                src = this.Locate(name);
                if (src == null)
                {
                    throw new FileNotFoundException(
                        "couldn't locate MIB: '" +
                        name + "'");
                }

                mib = this.Load(src);
            }
            else
            {
                mib.Loaded = true;
            }

            return mib;
        }

        /// <summary>
        /// Loads a MIB file from the specified URL. This method will also
        /// load all imported MIB:s if not previously loaded by this
        /// loader. Note that if the URL data contains several MIB modules,
        /// this method will only return the first one (although all are
        /// loaded).
        /// </summary>
        /// <param name="url">the URL containing the MIB</param>
        /// <returns>The first MIB module loaded</returns>
        /// <exception cref="IOException">
        /// If the MIB URL couldn't be read
        /// </exception>
        /// <exception cref="MibLoaderException">
        /// If the MIB file couldn't be loaded correctly
        /// </exception>
        public Mib Load(Uri url)
        {
            return this.Load(new MibSource(url));
        }

        /// <summary>
        /// Loads a MIB file from the specified input reader. This method
        /// will also load all imported MIB:s if not previously loaded by
        /// this loader. Note that if the input data contains several MIB
        /// modules, this method will only return the first one (although
        /// all are loaded).
        /// </summary>
        /// <param name="input">The input stream containing the MIB</param>
        /// <returns>The first MIB module loaded</returns>
        /// <exception cref="IOException">If the input stream couldn't be read</exception>
        /// <exception cref="MibLoaderException">If the MIB file couldn't be loaded correctly</exception>
        public Mib Load(StreamReader input)
        {
            return this.Load(new MibSource(input));
        }

        /// <summary>
        /// Unloads a MIB. This method will remove the loader reference to
        /// a previously loaded MIB if no other MIBs are depending on it.
        /// This method attempts to free the memory used by the MIB, as it
        /// clears both the loader and internal MIB references to the data
        /// structures (thereby allowing the garbage collector to recover
        /// the memory used if no other references exist). Other MIB:s
        /// should be unaffected by this operation.
        /// </summary>
        /// <param name="name">The MIB name</param>
        /// <exception cref="MibLoaderException">
        /// If the MIB couldn't be unloaded due to dependencies from 
        /// other loaded MIBs
        /// </exception>
        /// <see cref="Reset"/>
        public void Unload(string name)
        {
            foreach (Mib mib in this.mibs)
            {
                if (mib.Equals(name))
                {
                    this.Unload(mib);
                    return;
                }
            }
        }

        /// <summary>
        /// Unloads a MIB. This method will remove the loader reference to
        /// a previously loaded MIB if no other MIBs are depending on it.
        /// This method attempts to free the memory used by the MIB, as it
        /// clears both the loader and internal MIB references to the data
        /// structures (thereby allowing the garbage collector to recover
        /// the memory used if no other references exist). Other MIB:s
        /// should be unaffected by this operation.
        /// </summary>
        /// <param name="mib">The MIB to be unloaded</param>
        /// <exception cref="MibLoaderException">
        /// If the MIB couldn't be unloaded due to dependencies from 
        /// other loaded MIBs
        /// </exception>
        /// <see cref="Reset"/>
        public void Unload(Mib mib)
        {
            if (this.mibs.Contains(mib))
            {
                var referers = mib.ImportingMibs;
                if (referers.Count > 0)
                {
                    string message = "cannot be unloaded due to reference in " +
                              referers[0];
                    throw new MibLoaderException(mib.File, message);
                }

                this.mibs.Remove(mib);
                mib.Clear();
            }
        }

        /// <summary><para>
        /// Unloads all MIBs loaded by this loaded (since the last reset).
        /// This method attempts to free all the memory used by the MIBs,
        /// as it clears both the loader and internal MIB references to
        /// the data structures (thereby allowing the garbage collector to
        /// recover the memory used if no other references exist). Note
        /// that no previous MIBs returned by this loader should be
        /// accessed after this method has been called.</para>
        /// <para>
        /// In order to just reset the MIB loader to force re-loading of
        /// MIB files, use the reset() method instead which will leave the
        /// MIBs unaffected.</para>
        /// </summary>
        /// <see cref="Reset"/>
        public void UnloadAll()
        {
            foreach (Mib mib in this.mibs)
            {
                mib.Clear();
            }

            this.Reset();
        }

        /// <summary>
        /// Schedules the loading of a MIB file. The file is added to the
        /// queue of MIB files to be loaded, unless it is already loaded
        /// or in the queue. The MIB file search is postponed until the
        /// MIB is to be loaded, avoiding loading if the MIB name was
        /// defined in another MIB file in the queue.
        /// </summary>
        /// <param name="name">The MIB name (filename without extension)</param>
        public void ScheduleLoad(string name)
        {
            if (this.GetMib(name) == null && !this.nameQueue.Contains(name))
            {
                this.nameQueue.Add(name);
            }
        }

        /// <summary>
        /// Loads all MIB files in the loader queue. New entries may be
        /// added to the queue while loading a MIB, as a result of
        /// importing other MIB files. This method will either load all
        /// MIB files in the queue or none (if errors were encountered).
        /// </summary>
        /// <returns>The loader log for the whole queue</returns>
        /// <exception cref="MibLoaderException">
        /// If the MIB file in the queue couldn't be
        /// found
        /// </exception>
        private MibLoaderLog LoadQueue()
        {
            MibLoaderLog log = new MibLoaderLog();
            IList<Mib> processed = new List<Mib>();

            IList<Mib> list;

            foreach (var msrc in this.sourceQueue)
            {
                if (this.GetMib(msrc.File) == null)
                {
                    list = msrc.ParseMib(this, log);
                    foreach (var mib in list)
                    {
                        mib.Loaded = true;
                    }

                    this.mibs = this.mibs.Concat(list).ToList();
                    processed = processed.Concat(list).ToList();
                }
            }

            this.sourceQueue.Clear();

            foreach (var name in this.nameQueue)
            {
                MibSource src = this.Locate(name);
                if (src == null)
                {
                    continue;
                }

                if (this.GetMib(src.File) == null)
                {
                    list = src.ParseMib(this, log);
                    foreach (var mib in list)
                    {
                        mib.Loaded = false;
                    }

                    this.mibs = this.mibs.Concat(list).ToList();
                    processed = processed.Concat(list).ToList();
                }
            }

            this.nameQueue.Clear();
            
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
                    mib.Validate();
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
                    this.mibs.Remove(mib);
                }
            }

            return log;
        }

        /// <summary>
        /// Searches for a MIB in the search path. The name specified
        /// should be the MIB name. If a matching file name isn't found in
        /// the directory search path, the contents in the base resource
        /// path are also tested. Finally, if no MIB file has been found,
        /// the files in the search path will be opened regardless of file
        /// name to perform a small heuristic test for the MIB in question.
        /// </summary>
        /// <param name="name">The MIB name</param>
        /// <returns>The MIB found, or null if none was found</returns>
        private MibSource Locate(string name)
        {
            string file;

            foreach (var dir in this.dirCaches)
            {
                file = dir.FindByName(name);
                if (file != null)
                {
                    return new MibSource(file);
                }

                file = dir.FindByContent(name);

                if (file != null)
                {
                    return new MibSource(file);
                }
            }

            return null;
        }

        /// <summary>
        /// Loads a MIB. This method will also load all imported MIB:s if
        /// not previously loaded by this loader. Note that if the source
        /// contains several MIB modules, this method will only return the
        /// first one (although all are loaded).
        /// </summary>
        /// <param name="src">The MIB source</param>
        /// <returns>The first MIB Module loaded</returns>
        /// <exception cref="IOException">If the MIB couldn't be found</exception>
        /// <exception cref="MibLoaderException">
        /// If the MIB file couldn't be loaded correctly
        /// </exception>
        private Mib Load(MibSource src)
        {
            MibLoaderLog log;

            this.sourceQueue.Clear();
            this.sourceQueue.Add(src);

            log = this.LoadQueue();
            if (log.ErrorCount > 0)
            {
                throw new MibLoaderException(log);
            }

            return this.mibs.Last();
        }

        /// <summary>
        /// A MIB input source. This class encapsulates the two different
        /// ways of locating a MIB file, either through a file or a URL.
        /// </summary>
        private class MibSource
        {
            /// <summary>
            /// The singleton ASN.1 parser used by all MIB sources.
            /// </summary>
            private static Asn1Parser parser = null;

            /// <summary>
            /// The MIB file. This variable is only set if the MIB is read
            /// from file, or if the MIB name is known.
            /// </summary>
            private string file = null;

            /// <summary>
            /// The MIB URL location. This variable is only set if the MIB
            /// is read from a URL.
            /// </summary>
            private Uri url = null;

            /// <summary>
            /// The MIB reader. This variable is only set if the MIB
            /// is read from an input stream.
            /// </summary>
            private System.IO.TextReader input = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="MibSource"/> class.
            /// The MIB will be read from the specified file.
            /// </summary>
            /// <param name="file">The file to read from</param>
            public MibSource(string file)
            {
                this.file = file;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MibSource"/> class.
            /// The MIB will be read from the specified URL.
            /// </summary>
            /// <param name="url">The URL to read from</param>
            public MibSource(Uri url)
            {
                this.url = url;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MibSource"/> class.
            /// The MIB will be read from the specified URL. This method
            /// also create a default file from the specified MIB name in 
            /// order to improve possible error messages.
            /// </summary>
            /// <param name="name">The MIB name</param>
            /// <param name="url">The URL to read from</param>
            public MibSource(string name, Uri url) : this(url)
            {
                this.file = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MibSource"/> class.
            /// The MIB will be read from the specified input reader. The 
            /// input reader will be closed after reading the MIB.
            /// </summary>
            /// <param name="input">The input stream to read from</param>
            public MibSource(TextReader input)
            {
                this.input = input;
            }

            /// <summary>
            /// Gets  the MIB file. If the MIB is loaded from URL this
            /// file does not actually exist, but is used for providing a
            /// unique reference to the MIB.
            /// </summary>
            /// <returns>The MIB file</returns>
            public string File
            {
                get
                {
                    return this.file;
                }
            }

            /// <summary>
            /// Checks if this object is equal to another. This method
            /// will only return true for another mib source object with
            /// the same input source.
            /// </summary>
            /// <param name="obj">The object to compare with</param>
            /// <returns>True if the objects are equal, false if not</returns>
            public override bool Equals(object obj)
            {
                MibSource src = obj as MibSource;

                if (src == null)
                {
                    return false;
                }

                if (this.url != null)
                {
                    return this.url.Equals(src.url);
                }
                else if (this.file != null)
                {
                    return this.file.Equals(src.file);
                }

                return false;
            }

            /// <summary>
            /// Returns the hash code value for the object. This method is
            /// re-implemented to fulfill the contract of returning the same
            /// hash code for objects that are considered equal.
            /// </summary>
            /// <returns>The hash code value for the object</returns>
            public override int GetHashCode()
            {
                if (this.url != null)
                {
                    return this.url.GetHashCode();
                }
                else if (this.file != null)
                {
                    return this.file.GetHashCode();
                }
                else
                {
                    return base.GetHashCode();
                }
            }

            /// <summary>
            /// Parses the MIB input source and returns the MIB modules
            /// found. This method will read the MIB either from file, URL
            /// or input stream.
            /// </summary>
            /// <param name="loader">The MIB loader to use for imports</param>
            /// <param name="log">The MIB log to use for errors</param>
            /// <returns>The list of MIB modules created</returns>
            /// <exception cref="IOException">If the MIB couldn't be found</exception>
            /// <exception cref="MibLoaderException">
            /// If the MIB couldn't be parsed correctly
            /// </exception>
            public IList<Mib> ParseMib(MibLoader loader, MibLoaderLog log)
            {
                MibAnalyzer analyzer;
                string msg;
                string result = string.Empty;

                // Open input stream
                if (this.input != null)
                {
                    result = this.input.ReadToEnd();
                }
                else if (this.url != null)
                {
                    using (var client = new System.Net.WebClient())
                    {
                        result = client.DownloadString(this.url);
                    }
                }
                else
                {
                    using (TextReader reader = System.IO.File.OpenText(this.file))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                using (this.input = new StringReader(result))
                {
                    // Parse input stream
                    analyzer = new MibAnalyzer(this.file, loader, log);
                    try
                    {
                        if (parser == null)
                        {
                            Asn1Tokenizer tokenizer = new Asn1Tokenizer(this.input);
                            parser = new Asn1Parser(tokenizer, analyzer);
                            parser.Tokenizer.UseTokenList = true;
                        }
                        else
                        {
                            parser.Reset(this.input, analyzer);
                        }

                        parser.Parse();
                        return analyzer.Mibs.ToList();
                    }
                    catch (ParserCreationException e)
                    {
                        msg = "parser creation error in ASN.1 parser: " +
                              e.Message;
                        log.AddInternalError(this.file, msg);
                        analyzer.Reset();
                        throw new MibLoaderException(log);
                    }
                    catch (ParserLogException e)
                    {
                        log.AddAll(this.file, e);
                        analyzer.Reset();
                        throw new MibLoaderException(log);
                    }
                }
            }
        }
    }
}
