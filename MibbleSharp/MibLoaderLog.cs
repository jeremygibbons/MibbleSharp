//
// MibLoaderLog.cs
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
using System.Text;
using PerCederberg.Grammatica.Runtime;

namespace MibbleSharp
{
    /**
     * A MIB loader log. This class contains error and warning messages
     * from loading a MIB file and all imports not previously loaded.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public class MibLoaderLog
    {

        /**
         * The log entries.
         */
        private IList<LogEntry> entries = new List<LogEntry>();

        /**
         * The log error count.
         */
        private int errors = 0;

        /**
         * The log warning count.
         */
        private int warnings = 0;

        /**
         * Creates a new loader log without entries.
         */
        public MibLoaderLog()
        {
            // No initialization needed
        }

        /// <summary>
        /// The number of errors in the log
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return errors;
            }
        }

        /**
         * Returns the number of warnings in the log.
         *
         * @return the number of warnings in the log
         */
        public int WarningCount
        {
            get
            {
                return warnings;
            }
        }

        /**
         * Adds a log entry to this log.
         *
         * @param entry          the log entry to add
         */
        public void Add(LogEntry entry)
        {
            if (entry.IsError)
            {
                errors++;
            }
            if (entry.IsWarning)
            {
                warnings++;
            }
            entries.Add(entry);
        }

        /**
         * Adds an internal error message to the log. Internal errors are
         * only issued when possible bugs are encountered. They are
         * counted as errors.
         *
         * @param location       the file location
         * @param message        the error message
         */
        public void AddInternalError(FileLocation location, string message)
        {
            Add(new LogEntry(LogEntry.INTERNAL_ERROR, location, message));
        }

        /**
         * Adds an internal error message to the log. Internal errors are
         * only issued when possible bugs are encountered. They are
         * counted as errors.
         *
         * @param file           the file affected
         * @param message        the error message
         */
        public void AddInternalError(string file, string message)
        {
            AddInternalError(new FileLocation(file), message);
        }

        /**
         * Adds an error message to the log.
         *
         * @param location       the file location
         * @param message        the error message
         */
        public void AddError(FileLocation location, string message)
        {
            Add(new LogEntry(LogEntry.ERROR, location, message));
        }

        /**
         * Adds an error message to the log.
         *
         * @param file           the file affected
         * @param line           the line number
         * @param column         the column number
         * @param message        the error message
         */
        public void AddError(string file, int line, int column, string message)
        {
            AddError(new FileLocation(file, line, column), message);
        }

        /**
         * Adds a warning message to the log.
         *
         * @param location       the file location
         * @param message        the warning message
         */
        public void AddWarning(FileLocation location, string message)
        {
            Add(new LogEntry(LogEntry.WARNING, location, message));
        }

        /**
         * Adds a warning message to the log.
         *
         * @param file           the file affected
         * @param line           the line number
         * @param column         the column number
         * @param message        the warning message
         */
        public void AddWarning(string file, int line, int column, string message)
        {
            AddWarning(new FileLocation(file, line, column), message);
        }

        /**
         * Adds all log entries from another log.
         *
         * @param log            the MIB loader log
         */
        public void AddAll(MibLoaderLog log)
        {
            for (int i = 0; i < log.entries.Count; i++)
            {
                Add(log.entries[i]);
            }
        }

        /**
         * Adds all errors from a parser log exception.
         *
         * @param file           the file affected
         * @param log            the parser log exception
         */
        public void AddAll(string file, ParserLogException log)
        {
            foreach(var err in log.Errors)
            {
                AddError(file, err.Line, err.Column, err.ErrorMessage);
            }
        }

        /**
         * Returns an iterator with all the log entries. The iterator
         * will only return LogEntry instances.
         *
         * @return an iterator with all the log entries
         *
         * @see LogEntry
         *
         * @since 2.2
         */
        public IEnumerator<LogEntry> Entries
        {
            get
            {
                return entries.GetEnumerator();
            }
        }

        /**
         * Prints all log entries to the specified output stream.
         *
         * @param output         the output stream to use
         */
        public void PrintTo(TextWriter output)
        {
            PrintTo(output, 70);
        }

        /**
         * Prints all log entries to the specified output stream.
         *
         * @param output         the output stream to use
         * @param margin         the print margin
         *
         * @since 2.2
         */
        public void PrintTo(TextWriter output, int margin)
        {
            LogEntry entry;
            string str;

            for (int i = 0; i < entries.Count; i++)
            {
                entry = entries[i];
                StringBuilder buffer = new StringBuilder();
                switch (entry.Type)
                {
                    case LogEntry.ERROR:
                        buffer.Append("Error: ");
                        break;
                    case LogEntry.WARNING:
                        buffer.Append("Warning: ");
                        break;
                    default:
                        buffer.Append("Internal Error: ");
                        break;
                }
                buffer.Append("in ");
                buffer.Append(RelativeFilename(entry.File));
                if (entry.LineNumber > 0)
                {
                    buffer.Append(": line ");
                    buffer.Append(entry.LineNumber);
                }
                buffer.Append(":\n");
                str = LinebreakString(entry.Message, "    ", margin);
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

        /**
         * Creates a relative file name from a file. This method will
         * return the absolute file name if the file unless the current
         * directory is a parent to the file.
         *
         * @param file           the file to calculate relative name for
         *
         * @return the relative name if found, or
         *         the absolute name otherwise
         */
        private string RelativeFilename(string file)
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
                    if (filePath[0] == '/'
                     || filePath[0] == '\\')
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

        /**
         * Breaks a string into multiple lines. This method will also add
         * a prefix to each line in the resulting string. The prefix
         * length will be taken into account when breaking the line. Line
         * breaks will only be inserted as replacements for space
         * characters.
         *
         * @param str            the string to line break
         * @param prefix         the prefix to add to each line
         * @param length         the maximum line length
         *
         * @return the new formatted string
         */
        private string LinebreakString(string str, string prefix, int length)
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



        /**
         * A log entry. This class holds all the details in an error or a
         * warning log entry.
         *
         * @author   Per Cederberg, <per at percederberg dot net>
         * @version  2.2
         * @since    2.2
         */
        public class LogEntry
        {

            /**
             * The internal error log entry type constant.
             */
            public const int INTERNAL_ERROR = 1;

            /**
             * The error log entry type constant.
             */
            public const int ERROR = 2;

            /**
             * The warning log entry type constant.
             */
            public const int WARNING = 3;

            /**
             * The log entry type.
             */
            private int type;

            /**
             * The log entry file reference.
             */
            private FileLocation location;

            /**
             * The log entry message.
             */
            private string message;

            /**
             * Creates a new log entry.
             *
             * @param type           the log entry type
             * @param location       the log entry file reference
             * @param message        the log entry message
             */
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

            /**
             * Checks if this is an error log entry.
             *
             * @return true if this is an error log entry, or
             *         false otherwise
             */
            public bool IsError
            {
                get
                {
                    return type == INTERNAL_ERROR || type == ERROR;
                }
            }

            /**
             * Checks if this is a warning log entry.
             *
             * @return true if this is a warning log entry, or
             *         false otherwise
             */
            public bool IsWarning
            {
                get
                {
                    return type == WARNING;
                }
            }

            /**
             * Returns the log entry type.
             *
             * @return the log entry type
             *
             * @see #INTERNAL_ERROR
             * @see #ERROR
             * @see #WARNING
             */
            public int Type
            {
                get
                {
                    return type;
                }
            }

            /**
             * Returns the file this entry applies to.
             *
             * @return the file affected
             */
            public string File
            {
                get
                {
                    return location.File;
                }
            }

            /**
             * Returns the line number.
             *
             * @return the line number
             */
            public int LineNumber
            {
                get
                {
                    return location.LineNumber;
                }
            }

            /**
             * Returns the column number.
             *
             * @return the column number
             */
            public int ColumnNumber
            {
                get
                {
                    return location.ColumnNumber;
                }
            }

            /**
             * Returns the log entry message.
             *
             * @return the log entry message
             */
            public string Message
            {
                get
                {
                    return message;
                }
            }

            /**
             * Reads the line from the referenced file. If the file couldn't
             * be opened or read correctly, null will be returned. The line
             * will NOT contain the terminating '\n' character.
             *
             * @return the line read, or
             *         null if not found
             */
            public string ReadLine()
            {
                return location.ReadLine();
            }
        }
    }

}
