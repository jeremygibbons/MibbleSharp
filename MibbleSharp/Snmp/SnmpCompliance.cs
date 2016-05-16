/*
 * SnmpCompliance.cs
 *
 * This work is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 *
 * This work is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
 * USA
 *
 * Original Java code Copyright (c) 2004-2016 Per Cederberg. All
 * rights reserved.
 * C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights
 * reserved
 */

using System;
using System.Text;

namespace MibbleSharp.Snmp
{

    /**
     * An SNMP module compliance value. This declaration is used inside a
     * module declaration for both the GROUP and OBJECT compliance parts.
     *
     * @see SnmpModule
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    public class SnmpCompliance
    {

        /**
         * The compliance group flag. 
         */
        private bool group;

        /**
         * The compliance value.
         */
        private MibValue value;

        /**
         * The value syntax.
         */
        private MibType syntax;

        /**
         * The value write syntax.
         */
        private MibType writeSyntax;

        /**
         * The access mode.
         */
        private SnmpAccess access;

        /**
         * The compliance description.
         */
        private string description;

        /**
         * The compliance comment.
         */
        private string comment = null;

        /**
         * Creates a new SNMP module compliance declaration.
         *
         * @param group          the group compliance flag
         * @param value          the compliance value
         * @param syntax         the value syntax, or null
         * @param writeSyntax    the value write syntax, or null
         * @param access         the access mode, or null
         * @param description    the compliance description
         */
        public SnmpCompliance(bool group,
                              MibValue value,
                              MibType syntax,
                              MibType writeSyntax,
                              SnmpAccess access,
                              string description)
        {

            this.group = group;
            this.value = value;
            this.syntax = syntax;
            this.writeSyntax = writeSyntax;
            this.access = access;
            this.description = description;
        }

        /**
         * Initializes this object. This will remove all levels of
         * indirection present, such as references to other types, and
         * returns the basic type. No type information is lost by this
         * operation. This method may modify this object as a
         * side-effect, and will be called by the MIB loader.
         *
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        public void Initialize(MibLoaderLog log)
        {

            value = value.Initialize(log, null);
        if (syntax != null) {
                syntax = syntax.Initialize(null, log);
            }
        if (writeSyntax != null) {
                writeSyntax = writeSyntax.Initialize(null, log);
            }
        }

        /**
         * Checks if this is a group compliance.
         *
         * @return true if this is a group compliance, or
         *         false otherwise
         *
         * @since 2.6
         */
        public bool isGroup()
        {
            return group;
        }

        /**
         * Checks if this is an object compliance.
         *
         * @return true if this is an object compliance, or
         *         false otherwise
         *
         * @since 2.6
         */
        public bool isObject()
        {
            return !group;
        }

        /**
         * Returns the value.
         *
         * @return the value
         */
        public MibValue getValue()
        {
            return value;
        }

        /**
         * Returns the value syntax.
         *
         * @return the value syntax, or
         *         null if not set
         */
        public MibType getSyntax()
        {
            return syntax;
        }

        /**
         * Returns the value write syntax.
         *
         * @return the value write syntax, or
         *         null if not set
         */
        public MibType getWriteSyntax()
        {
            return writeSyntax;
        }

        /**
         * Returns the access mode.
         *
         * @return the access mode, or
         *         null if not set
         */
        public SnmpAccess getAccess()
        {
            return access;
        }

        /**
         * Returns the compliance description. Any unneeded indentation
         * will be removed from the description, and it also replaces all
         * tab characters with 8 spaces.
         *
         * @return the compliance description
         *
         * @see #getUnformattedDescription()
         */
        public string getDescription()
        {
            return SnmpType.RemoveIndent(description);
        }

        /**
         * Returns the unformatted compliance description. This method
         * returns the original MIB file text, without removing unneeded
         * indentation or similar.
         *
         * @return the unformatted compliance description, or
         *         null if no description has been set
         *
         * @see #getDescription()
         *
         * @since 2.6
         */
        public string getUnformattedDescription()
        {
            return description;
        }

        /**
         * Returns the compliance comment.
         *
         * @return the compliance comment, or
         *         null if no comment was set
         *
         * @since 2.9
         */
        public string getComment()
        {
            return comment;
        }

        /**
         * Sets the compliance comment.
         *
         * @param comment        the compliance comment
         *
         * @since 2.9
         */
        public void setComment(string comment)
        {
            this.comment = comment;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(value);
            if (syntax != null)
            {
                builder.Append("\n      Syntax: ");
                builder.Append(syntax);
            }
            if (writeSyntax != null)
            {
                builder.Append("\n      Write-Syntax: ");
                builder.Append(writeSyntax);
            }
            if (access != null)
            {
                builder.Append("\n      Access: ");
                builder.Append(access);
            }
            builder.Append("\n      Description: ");
            builder.Append(description);
            return builder.ToString();
        }
    }

}
