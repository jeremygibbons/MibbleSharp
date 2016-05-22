// <copyright file="MibLoaderLog.cs" company="None">
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
    using System.Text;
    using PerCederberg.Grammatica.Runtime;

    /// <summary>
    ///  A MIB loader log. This class contains error and warning messages
    /// from loading a MIB file and all imports not previously loaded.
    /// </summary>
    public class MibLoaderLog
    {
        /// <summary>
        /// The log entries.
        /// </summary>
        private IList<LogEntry> entries = new List<LogEntry>();

        /// <summary>
        /// The log error count.
        /// </summary>
        private int errors = 0;

        /// <summary>
        /// The log warning count.
        /// </summary>
        private int warnings = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibLoaderLog"/> class.
        /// It is initially empty.
        /// </summary>
        public MibLoaderLog()
        {
            // No initialization needed
        }

        /// <summary>
        /// Gets an iterator with all the log entries. The iterator
        /// will only return LogEntry instances.
        /// </summary>
        public IEnumerator<LogEntry> Entries
        {
            get
            {
                return this.entries.GetEnumerator();
            }
        }
        
        /// <summary>
        /// Gets the number of errors in the log
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets the number of warnings in the log.
        /// </summary>
        public int WarningCount
        {
            get
            {
                return this.warnings;
            }
        }

        /// <summary>
        /// Adds a log entry to this log.
        /// </summary>
        /// <param name="entry">The log entry to be added</param>
        public void Add(LogEntry entry)
        {
            if (entry.IsError)
            {
                this.errors++;
            }

            if (entry.IsWarning)
            {
                this.warnings++;
            }

            this.entries.Add(entry);
        }

        /// <summary>
        /// Adds an internal error message to the log. Internal errors are 
        /// only issued when possible bugs are encountered.They are
        /// counted as errors.
        /// </summary>
        /// <param name="location">The file location</param>
        /// <param name="message">The log message</param>
        public void AddInternalError(FileLocation location, string message)
        {
            this.Add(new LogEntry(LogEntry.InternalError, location, message));
        }

        /// <summary>
        /// Adds an internal error message to the log. Internal errors are 
        /// only issued when possible bugs are encountered.They are
        /// counted as errors.
        /// </summary>
        /// <param name="file">The filename</param>
        /// <param name="message">The log message</param>
        public void AddInternalError(string file, string message)
        {
            this.AddInternalError(new FileLocation(file), message);
        }

        /// <summary>
        /// Adds an error message to the log.
        /// </summary>
        /// <param name="location">The file location</param>
        /// <param name="message">The log message</param>
        public void AddError(FileLocation location, string message)
        {
            this.Add(new LogEntry(LogEntry.Error, location, message));
        }

        /// <summary>
        /// Adds an error message to the log.
        /// </summary>
        /// <param name="file">The affected file</param>
        /// <param name="line">The line number</param>
        /// <param name="column">The column number</param>
        /// <param name="message">The log message</param>
        public void AddError(string file, int line, int column, string message)
        {
            this.AddError(new FileLocation(file, line, column), message);
        }

        /// <summary>
        /// Adds a warning message to the log.
        /// </summary>
        /// <param name="location">The file location</param>
        /// <param name="message">The log message</param>
        public void AddWarning(FileLocation location, string message)
        {
            this.Add(new LogEntry(LogEntry.Warning, location, message));
        }

        /// <summary>
        /// Adds a warning message to the log.
        /// </summary>
        /// <param name="file">The affected file</param>
        /// <param name="line">The line number</param>
        /// <param name="column">The column number</param>
        /// <param name="message">The log message</param>
        public void AddWarning(string file, int line, int column, string message)
        {
            this.AddWarning(new FileLocation(file, line, column), message);
        }

        /// <summary>
        /// Adds all log entries from another log.
        /// </summary>
        /// <param name="log">The log to be read from</param>
        public void AddAll(MibLoaderLog log)
        {
            foreach (var entry in log.entries)
            {
                this.Add(entry);
            }
        }

        /// <summary>
        /// Adds all errors from a parser log exception.
        /// </summary>
        /// <param name="file">The affected file</param>
        /// <param name="log">The Parser Log Exception</param>
        public void AddAll(string file, ParserLogException log)
        {
            foreach (var err in log.Errors)
            {
                this.AddError(file, err.Line, err.Column, err.ErrorMessage);
            }
        }

        /// <summary>
        /// Prints all log entries to the specified output stream, with a default margin
        /// </summary>
        /// <param name="output">The output stream to use</param>
        public void PrintTo(TextWriter output)
        {
            this.PrintTo(output, 70);
        }

        /// <summary>
        /// Prints all log entries to the specified output stream.
        /// </summary>
        /// <param name="output">The output stream to use</param>
        /// <param name="margin">The margin</param>
        public void PrintTo(TextWriter output, int margin)
        {
            string str;
            foreach (var entry in this.entries)
            {
                StringBuilder buffer = new StringBuilder();
                switch (entry.Type)
                {
                    case LogEntry.Error:
                        buffer.Append("Error: ");
                        break;
                    case LogEntry.Warning:
                        buffer.Append("Warning: ");
                        break;
                    default:
                        buffer.Append("Internal Error: ");
                        break;
                }

                buffer.Append("in ");
                buffer.Append(MibLoaderLog.RelativeFilename(entry.File));
                if (entry.LineNumber > 0)
                {
                    buffer.Append(": line ");
                    buffer.Append(entry.LineNumber);
                }

                buffer.Append(":\n");
                str = MibLoaderLog.LinebreakString(entry.Message, "    ", margin);
                buffer.Append(str);
                str = entry.ReadLine();
                if (str != null && str.Length >= entry.ColumnNumber)
                {
                    buffer.Append("\n\n");
                    buffer.Append(str);
                    buffer.Append("\n");
                    for (int j = 1; j < entry.ColumnNumber; j++)
                    {
                        if (str[j - 1] == '\t')
                        {
                            buffer.Append("\t");
                        }
                        else
                        {
                            buffer.Append(" ");
                        }
                    }

                    buffer.Append("^");
                }

                output.WriteLine(buffer.ToString());
            }

            output.Flush();
        }

        /// <summary>
        /// Creates a relative file name from a file. This method will
        /// return the absolute file name if the file unless the current
        /// directory is a parent to the file.
        /// </summary>
        /// <param name="file">The file to calculate relative name for</param>
        /// <returns>
        /// The relative name if found, or the absolute name otherwise
        /// </returns>
        private static string RelativeFilename(string file)
        {
            string currentPath;
            string filePath;

            if (file == null)
            {
                return "<unknown file>";
            }

            try
            {
                currentPath = new Uri(".").LocalPath;
                filePath = new Uri(file).LocalPath;
                if (filePath.StartsWith(currentPath))
                {
                    filePath = filePath.Substring(currentPath.Length);
                    if (filePath[0] == '/' || filePath[0] == '\\')
                    {
                        return filePath.Substring(1);
                    }
                    else
                    {
                        return filePath;
                    }
                }
            }
            catch (UriFormatException)
            {
                // Do nothing
            }

            return file.ToString();
        }

        /// <summary>
        /// Breaks a string into multiple lines.This method will also add
        /// a prefix to each line in the resulting string. The prefix
        /// length will be taken into account when breaking the line.Line
        /// breaks will only be inserted as replacements for space
        /// characters.
        /// </summary>
        /// <param name="str">The string to line break</param>
        /// <param name="prefix">The prefix to add to each line</param>
        /// <param name="length">The maximum line length</param>
        /// <returns>The new formatted string</returns>
        private static string LinebreakString(string str, string prefix, int length)
        {
            StringBuilder buffer = new StringBuilder();
            int pos;

            while (str.Length + prefix.Length > length)
            {
                pos = str.LastIndexOf(' ', length - prefix.Length);
                if (pos < 0)
                {
                    pos = str.IndexOf(' ');
                    if (pos < 0)
                    {
                        break;
                    }
                }

                buffer.Append(prefix);
                buffer.Append(str.Substring(0, pos));
                str = str.Substring(pos + 1);
                buffer.Append("\n");
            }

            buffer.Append(prefix);
            buffer.Append(str);
            return buffer.ToString();
        }

        /// <summary>
        /// A log entry. This class holds all the details in an error or a
        /// warning log entry.
        /// </summary>
        public class LogEntry
        {
            /// <summary>
            /// The internal error log entry type constant.
            /// </summary>
            public const int InternalError = 1;

            /// <summary>
            /// The error log entry type constant.
            /// </summary>
            public const int Error = 2;

            /// <summary>
            /// The warning log entry type constant.
            /// </summary>
            public const int Warning = 3;

            /// <summary>
            /// The log entry type.
            /// </summary>
            private int type;

            /// <summary>
            /// The log entry file reference.
            /// </summary>
            private FileLocation location;

            /// <summary>
            /// The log entry message.
            /// </summary>
            private string message;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogEntry"/> class.
            /// </summary>
            /// <param name="type">The log entry type</param>
            /// <param name="location">The log entry file reference</param>
            /// <param name="message">The log entry message</param>
            public LogEntry(int type, FileLocation location, string message)
            {
                this.type = type;
                if (location == null || location.File == null)
                {
                    this.location = new FileLocation("<unknown file>");
                }
                else
                {
                    this.location = location;
                }

                this.message = message;
            }

            /// <summary>
            /// Gets a value indicating whether this is an error log entry.
            /// </summary>
            public bool IsError
            {
                get
                {
                    return this.type == InternalError || this.type == Error;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this is a warning log entry.
            /// </summary>
            public bool IsWarning
            {
                get
                {
                    return this.type == Warning;
                }
            }

            /// <summary>
            /// Gets the log entry type
            /// </summary>
            /// <see cref="InternalError"/>
            /// <see cref="Error"/>
            /// <see cref="Warning"/>
            public int Type
            {
                get
                {
                    return this.type;
                }
            }

            /// <summary>
            /// Gets the file this entry applies to.
            /// </summary>
            public string File
            {
                get
                {
                    return this.location.File;
                }
            }

            /// <summary>
            /// Gets the line number.
            /// </summary>
            public int LineNumber
            {
                get
                {
                    return this.location.LineNumber;
                }
            }

            /// <summary>
            /// Gets the column number.
            /// </summary>
            public int ColumnNumber
            {
                get
                {
                    return this.location.ColumnNumber;
                }
            }

            /// <summary>
            /// Gets the log entry message.
            /// </summary>
            public string Message
            {
                get
                {
                    return this.message;
                }
            }

            /// <summary>
            /// Reads the line from the referenced file. If the file couldn't
            /// be opened or read correctly, null will be returned.The line
            /// will NOT contain the terminating '\n' character.
            /// </summary>
            /// <returns>The line read from the file, or null if not found</returns>
            public string ReadLine()
            {
                return this.location.ReadLine();
            }
        }
    }
}
