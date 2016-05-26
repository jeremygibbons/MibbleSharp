// <copyright file="SnmpType.cs" company="None">
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

namespace MibbleSharp.Snmp
{
    using System.Text;

    /// <summary>
    /// The base SNMP macro type. This is an abstract type, meaning there
    /// only exist instances of subclasses. It exists to provide methods
    /// that are valid across all SNMP macro types.
    /// </summary>
    public abstract class SnmpType : MibType
    {
        /// <summary>
        /// The type description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpType">SNMP macro type</see> class. This constructor can
        /// only be called by subclasses.
        /// </summary>
        /// <param name="name">The type name</param>
        /// <param name="description">The type description</param>    
        protected SnmpType(string name, string description) : base(name, false)
        {
            this.description = description;
        }

        /// <summary>
        /// Gets the type description. Any unneeded indentation will be
        /// removed from the description, and it also replaces all tab
        /// characters with 8 spaces.
        /// </summary>
        /// <see cref="getUnformattedDescription"/>
        public string Description
        {
            get
            {
                return RemoveIndent(this.description);
            }
        }

        /// <summary>
        /// Gets the unformatted type description. This method returns
        /// the original MIB file description, without removing unneeded
        /// indentation or similar.
        /// </summary>
        /// <see cref="Description"/>
        public string UnformattedDescription
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Returns a string with any unneeded indentation removed. This
        /// method will decide the indentation level from the number of
        /// spaces on the second line. It also replaces all tab characters
        /// with 8 spaces.
        /// </summary>
        /// <param name="str">The string to be de-indented</param>
        /// <returns>The input string with indentation removed</returns>
        public static string RemoveIndent(string str)
        {
            StringBuilder builder = new StringBuilder();
            int indent = -1;
            int pos;

            if (str == null)
            {
                return null;
            }

            while (str.Length > 0)
            {
                pos = str.IndexOf("\n");
                if (pos < 0)
                {
                    builder.Append(RemoveIndent(str, indent));
                    str = string.Empty;
                }
                else if (pos == 0)
                {
                    builder.Append("\n");
                    str = str.Substring(1);
                }
                else if (str.Substring(0, pos).Trim().Length == 0)
                {
                    builder.Append("\n");
                    str = str.Substring(pos + 1);
                }
                else if (builder.Length == 0)
                {
                    builder.Append(RemoveIndent(str.Substring(0, pos), -1));
                    builder.Append("\n");
                    str = str.Substring(pos + 1);
                }
                else
                {
                    if (indent < 0)
                    {
                        indent = 0;
                        for (int i = 0; IsSpace(str[i]); i++)
                        {
                            if (str[i] == '\t')
                            {
                                indent += 8;
                            }
                            else
                            {
                                indent++;
                            }
                        }
                    }

                    builder.Append(RemoveIndent(str.Substring(0, pos), indent));
                    builder.Append("\n");
                    str = str.Substring(pos + 1);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the type description indented with the specified
        /// string. The first line will NOT be indented, but only the
        /// following lines (if any).
        /// </summary>
        /// <param name="indent">The indentation string</param>
        /// <returns>The indented type description</returns>
        protected string GetDescription(string indent)
        {
            StringBuilder builder;
            string str = this.Description;
            int pos;

            if (str == null)
            {
                return null;
            }

            pos = str.IndexOf("\n");
            if (pos < 0)
            {
                return str;
            }

            builder = new StringBuilder();
            builder.Append(str.Substring(0, pos + 1));
            str = str.Substring(pos + 1);
            while (pos >= 0)
            {
                builder.Append(indent);
                pos = str.IndexOf("\n");
                if (pos < 0)
                {
                    builder.Append(str);
                }
                else
                {
                    builder.Append(str.Substring(0, pos + 1));
                    str = str.Substring(pos + 1);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Removes the specified number of leading spaces from a string.
        /// All tab characters in the string will be converted to spaces
        /// before processing. If the string contains fewer than the
        /// specified number of leading spaces, all will be removed. If
        /// the indentation count is less than zero (0), all leading
        /// spaces in the input string will be removed.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="indent">The indentation space count</param>
        /// <returns>The un-indented string</returns>
        private static string RemoveIndent(string str, int indent)
        {
            int pos = 0;

            str = ReplaceTabs(str);
            if (indent < 0)
            {
                return str.Trim();
            }

            for (pos = 0; pos < str.Length && pos < indent; pos++)
            {
                if (str[pos] != ' ')
                {
                    break;
                }
            }

            return str.Substring(pos);
        }

        /// <summary>
        /// Replaces any tab characters with 8 space characters.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>The input string with all tab characters replaced</returns>
        private static string ReplaceTabs(string str)
        {
            StringBuilder builder;

            if (str.IndexOf('\t') < 0)
            {
                return str;
            }
            else
            {
                builder = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '\t')
                    {
                        builder.Append("        ");
                    }
                    else
                    {
                        builder.Append(str[i]);
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Checks if a character is a space character.
        /// </summary>
        /// <param name="ch">The input character</param>
        /// <returns>True if the character is a space or tab, false if not</returns>
        private static bool IsSpace(char ch)
        {
            return ch == ' '
                || ch == '\t';
        }
    }
}
