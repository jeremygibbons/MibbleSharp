using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Snmp
{
     /**
     * The base SNMP macro type. This is an abstract type, meaning there
     * only exist instances of subclasses. It exists to provide methods
     * that are valid across all SNMP macro types.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.5
     * @since    2.5
     */
    public abstract class SnmpType : MibType
    {

    /**
     * The type description.
     */
    private string description;

    /**
     * Returns a string with any unneeded indentation removed. This
     * method will decide the indentation level from the number of
     * spaces on the second line. It also replaces all tab characters
     * with 8 spaces.
     *
     * @param str            the string to process
     *
     * @return the processed string
     */
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
                builder.Append(removeIndent(str, indent));
                str = "";
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
                builder.Append(removeIndent(str.Substring(0, pos), -1));
                builder.Append("\n");
                str = str.Substring(pos + 1);
            }
            else
            {
                if (indent < 0)
                {
                    indent = 0;
                    for (int i = 0; isSpace(str[i]); i++)
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
                builder.Append(removeIndent(str.Substring(0, pos), indent));
                builder.Append("\n");
                str = str.Substring(pos + 1);
            }
        }
        return builder.ToString();
    }

    /**
     * Removes the specified number of leading spaces from a string.
     * All tab characters in the string will be converted to spaces
     * before processing. If the string contains fewer than the
     * specified number of leading spaces, all will be removed. If
     * the indentation count is less than zero (0), all leading
     * spaces in the input string will be removed.
     *
     * @param str            the input string
     * @param indent         the indentation space count
     *
     * @return the unindented string
     */
    private static string removeIndent(string str, int indent)
    {
        int pos = 0;

        str = replaceTabs(str);
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

    /**
     * Replaces any tab characters with 8 space characters.
     *
     * @param str            the input string
     *
     * @return the new string without tab characters
     */
    private static string replaceTabs(string str)
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

    /**
     * Checks if a character is a space character.
     *
     * @param ch             the character to check
     *
     * @return true if the character is a space character, or
     *         false otherwise
     */
    private static bool isSpace(char ch)
    {
        return ch == ' '
            || ch == '\t';
    }

    /**
     * Creates a new SNMP macro type instance. This constructor can
     * only be called by subclasses.
     *
     * @param name           the type name
     * @param description    the type description
     */
    protected SnmpType(string name, string description) : base(name, false)
    {
        this.description = description;
    }

    /**
     * Returns the type description. Any unneeded indentation will be
     * removed from the description, and it also replaces all tab
     * characters with 8 spaces.
     *
     * @return the type description, or
     *         null if no description has been set
     *
     * @see #getUnformattedDescription()
     */
    public string getDescription()
    {
        return RemoveIndent(description);
    }

    /**
     * Returns the unformatted type description. This method returns
     * the original MIB file description, without removing unneeded
     * indentation or similar.
     *
     * @return the unformatted type description, or
     *         null if no description has been set
     *
     * @see #getDescription()
     *
     * @since 2.5
     */
    public string getUnformattedDescription()
    {
        return description;
    }

    /**
     * Returns the type description indented with the specified
     * string. The first line will NOT be indented, but only the
     * following lines (if any).
     *
     * @param indent         the indentation string
     *
     * @return the indented type description, or
     *         null if no description has been set
     */
    protected string getDescription(string indent)
    {
        StringBuilder builder;
        string str = getDescription();
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
        else
        {
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
    }
}

}
