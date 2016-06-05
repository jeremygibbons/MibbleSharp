// <copyright file="Program.cs" company="None">
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

namespace MibbleValidator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using MibbleSharp;

    /// <summary>
    /// A test program for validating MIBs
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The command-line help output.
        /// </summary>
        private static readonly string CommandHelp =
        "Validates a set of SNMP MIB files. This program comes with\n" +
        "ABSOLUTELY NO WARRANTY; for details see the LICENSE.txt file.\n" +
        "\n" +
        "Syntax: MibbleValidator <file(s) or URL(s)>";

        /// <summary>
        /// The main program function
        /// </summary>
        /// <param name="args">The array of program arguments</param>
        public static void Main(string[] args)
        {
            MibLoader loader = new MibLoader();
            Mib mib = null;
            System.Collections.ArrayList queue = new System.Collections.ArrayList();
            string file;
            int warnings = 0;
            int errors = 0;

            // Check command-line arguments
            if (args.Length < 1)
            {
                PrintHelp("No file(s) specified");
                return;
            }

            foreach (string arg in args)
            {
                try
                {
                    if (arg.Contains(":"))
                    {
                        queue.Add(new Uri(arg));
                    }
                    else
                    {
                        if (Directory.Exists(arg))
                        {
                            AddMibs(arg, queue);
                            continue;
                        }

                        if (!File.Exists(arg))
                        {
                            Console.Out.WriteLine("Warning: Skipping " + arg +
                                               ": file not found");
                            continue;
                        }

                        queue.Add(arg);
                    }
                }
                catch (UriFormatException e)
                {
                    System.Console.Out.WriteLine(
                        "Warning: Skipping " + arg + ": " + e.Message);
                }
            }

            // Parse MIB files
            int i = 0;
            foreach (var src in queue)
            {
                Console.Out.Write(i);
                Console.Out.Write("/");
                Console.Out.Write(queue.Count);
                Console.Out.Write(": Reading " + src + "... ");
                Console.Out.Flush();
                try
                {
                    loader.UnloadAll();
                    if (src is Uri)
                    {
                        loader.RemoveAllDirs();
                        mib = loader.Load((Uri)src);
                    }
                    else
                    {
                        file = (string)src;
                        if (!loader.HasDir(Directory.GetParent(file).FullName))
                        {
                            loader.RemoveAllDirs();
                            loader.AddDir(Directory.GetParent(file).FullName);
                        }

                        using (StreamReader sr = new StreamReader(file))
                        {
                            mib = loader.Load(sr);
                        }
                    }

                    Console.Out.WriteLine("[OK]");

                    if (mib.Log.WarningCount > 0)
                    {
                        mib.Log.PrintTo(Console.Out);
                        warnings++;
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.Out.WriteLine("[FAILED]");
                    PrintError(src.ToString(), e);
                    errors++;
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine("[FAILED]");
                    PrintError(src.ToString(), e);
                    errors++;
                }
                catch (MibLoaderException e)
                {
                    Console.Out.WriteLine("[FAILED]");
                    e.Log.PrintTo(Console.Out);
                    errors++;
                }

                i++;
            }

            // Print error count
            System.Console.Out.WriteLine();
            Console.Out.WriteLine("Files processed:  " + queue.Count);
            Console.Out.WriteLine("  with errors:    " + errors);
            Console.Out.WriteLine("  with warnings:  " + warnings);
            if (errors > 0)
            {
                Console.Error.WriteLine("Error: validation errors were encountered");
            }

            Console.ReadLine();

            // Return error count
            return;
        }

        /// <summary>
        /// Prints command line help information
        /// </summary>
        /// <param name="error">An optional error message</param>
        private static void PrintHelp(string error)
        {
            Console.Error.WriteLine(CommandHelp);
            Console.Error.WriteLine();
            if (error != null)
            {
                Console.Error.Write("Error: ");
                Console.Error.WriteLine(error);
                Console.Error.WriteLine();
            }
        }

        /// <summary>
        /// Prints a URL not found error message
        /// </summary>
        /// <param name="url">The URL that was not found</param>
        /// <param name="e">The detailed exception</param>
        private static void PrintError(string url, IOException e)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Error: couldn't open URL:");
            builder.Append("\n    ");
            builder.Append(url);
            Console.Out.WriteLine(builder.ToString());
        }

        /// <summary>
        /// Adds all MIB files from a directory to the
        /// specified queue
        /// </summary>
        /// <param name="dir">The directory to check</param>
        /// <param name="queue">The queue to add the files to</param>
        private static void AddMibs(string dir, ArrayList queue)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(dir);

            foreach (string file in files)
            {
                if (File.GetAttributes(file).HasFlag(FileAttributes.Hidden))
                {
                    // Hidden file or directories are ignored
                }
                else if (Directory.Exists(file))
                {
                    AddMibs(file, queue);
                }
                else if (IsMib(file))
                {
                    queue.Add(file);
                }
            }
        }

        /// <summary>
        /// Checks if a file is likely a MIB by looking at the first few lines 
        /// of the file and searching for the beginnings of a MIB definition.
        /// </summary>
        /// <param name="file">The file to check</param>
        /// <returns>True if the file is likely a MIB, false if not.</returns>
        private static bool IsMib(string file)
        {
            StringBuilder buffer = new StringBuilder();

            if (!File.Exists(file))
            {
                return false;
            }

            try
            {
                using (TextReader reader = File.OpenText(file))
                {
                    buffer.Append(reader.ReadToEnd());
                }
            }
            catch (FileNotFoundException)
            {
                // Do nothing
            }
            catch (IOException)
            {
                // Do nothing
            }

            string result = buffer.ToString();

            return result.IndexOf("DEFINITIONS") > 0 &&
                   result.IndexOf("::=") > 0 &&
                   result.IndexOf("BEGIN") > 0;
        }
    }
}
