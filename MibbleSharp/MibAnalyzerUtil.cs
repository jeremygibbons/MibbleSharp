using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerCederberg.Grammatica.Runtime;
using MibbleSharp.Asn1;

namespace MibbleSharp
{
    /**
     * Helper and utility functions for the MIB file analyzer.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.9
     */
    class MibAnalyzerUtil
    {

        /**
         * An internal hash map containing all the used comment tokens.
         * When a comment string is returned by the getComments() method,
         * the corresponding tokens will be added to this hash map and
         * not returned on subsequent calls.
         */
        private static HashSet<Token> commentTokens = new HashSet<Token>();

        /**
         * Checks if a node corresponds to a bit value. This method is
         * used to distinguish between bit values and object identifier
         * values during the analysis.
         *
         * @param node           the parse tree node to check
         *
         * @return true if the node contains a bit value, or
         *         false otherwise
         */
        public static bool IsBitValue(Node node)
        {
            if (node.GetId() == (int)Asn1Constants.COMMA)
            {
                return true;
            }
            else if (node.GetId() == (int)Asn1Constants.NAME_VALUE_LIST
                  && node.GetChildCount() < 4)
            {

                return true;
            }
            else
            {
                for (int i = 0; i < node.GetChildCount(); i++)
                {
                    if (IsBitValue(node.GetChildAt(i)))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /**
         * Returns all the comments associated with a specified node. If
         * there are multiple comment lines, these will be concatenated
         * into a single string. This method handles comments before,
         * inside and after (starting on the same line) as the specified
         * node. It also updates the comment token cache to avoid
         * returning the same comments twice.
         *
         * @param node           the production or token node
         *
         * @return the comment string, or
         *         null if no comments were found
         */
        public static string GetComments(Node node)
        {
            string comment = "";
            string str;
            Token token;

            str = getCommentsBefore(node);
            if (str != null)
            {
                comment = str;
            }
            str = getCommentsInside(node);
            if (str != null)
            {
                if (comment.Length > 0)
                {
                    comment += "\n\n";
                }
                comment += str;
            }
            token = getCommentTokenSameLine(node);
            if (token != null)
            {
                if (comment.Length > 0)
                {
                    comment += "\n\n";
                }
                token = token.GetPreviousToken();
                comment += getCommentsAfter(token);
            }
            return comment.Length <= 0 ? null : comment;
        }

        /**
         * Returns all the footer comments after the specified node. This
         * method also clears the comment cache and should be called
         * exactly once after each MIB file parsed, in order to free
         * memory used by the comment token cache.
         *
         * @param node           the production or token node
         *
         * @return the comment string, or
         *         null if no comments were found
         */
        public static string getCommentsFooter(Node node)
        {
            string comment = getCommentsAfter(node);

            commentTokens.Clear();
            return comment;
        }

        /**
         * Returns all the comments before the specified node. If
         * there are multiple comment lines possibly separated by
         * whitespace, they will be concatenated into one string.
         *
         * @param node          the production or token node
         *
         * @return the comment string, or
         *         null if no comments were found
         */
        private static string getCommentsBefore(Node node)
        {
            Token token = getFirstToken(node);
            List<string> comments = new List<string>();
            StringBuilder builder = new StringBuilder();
            string res = "";

            if (token != null)
            {
                token = token.GetPreviousToken();
            }
            while (token != null)
            {
                if (token.GetId() == (int) Asn1Constants.WHITESPACE)
                {
                    comments.Add(getLineBreaks(token.GetImage()));
                }
                else if (token.GetId() == (int)Asn1Constants.COMMENT &&
                         !commentTokens.Contains(token))
                {

                    commentTokens.Add(token);
                    comments.Add(token.GetImage().Substring(2).Trim());
                }
                else
                {
                    break;
                }
                token = token.GetPreviousToken();
            }
            for (int i = comments.Count - 1; i >= 0; i--)
            {
                builder.Append(comments[i]);
            }
            res = builder.ToString().Trim();
            return res.Length <= 0 ? null : res;
        }

        /**
         * Returns all the comments after the specified node. If
         * there are multiple comment lines possibly separated by
         * whitespace, they will be concatenated into one string.
         *
         * @param node           the production or token node
         *
         * @return the comment string, or
         *         null if no comments were found
         */
        private static string getCommentsAfter(Node node)
        {
            Token token = getLastToken(node);
            StringBuilder comment = new StringBuilder();
            string res;

            if (token != null)
            {
                token = token.GetNextToken();
            }
            while (token != null)
            {
                if (token.GetId() == (int) Asn1Constants.WHITESPACE)
                {
                    comment.Append(getLineBreaks(token.GetImage()));
                }
                else if (token.GetId() == (int) Asn1Constants.COMMENT &&
                         !commentTokens.Contains(token))
                {

                    commentTokens.Add(token);
                    comment.Append(token.GetImage().Substring(2).Trim());
                }
                else
                {
                    break;
                }
                token = token.GetNextToken();
            }
            res = comment.ToString().Trim();
            return res.Length <= 0 ? null : res;
        }

        /**
         * Returns all the unhandled comments inside the specified node.
         * Note that only comment tokens not present in the token cache
         * will be returned by this method.
         *
         * @param node           the production or token node
         *
         * @return the comment string, or
         *         null if no comments were found
         */
        private static string getCommentsInside(Node node)
        {
            Token token = getFirstToken(node);
            Token last = getLastToken(node);
            StringBuilder comment = new StringBuilder();
            string res;

            while (token != null && token != last)
            {
                if (token.GetId() == (int) Asn1Constants.COMMENT &&
                    !commentTokens.Contains(token))
                {

                    commentTokens.Add(token);
                    comment.Append(token.GetImage().Substring(2).Trim());
                    comment.Append("\n");
                }
                token = token.GetNextToken();
            }
            res = comment.ToString().Trim();
            return res.Length <= 0 ? null : res;
        }

        /**
         * Returns the first comment token on the same line.
         *
         * @param node           the production node
         *
         * @return the first comment token on the same line
         */
        private static Token getCommentTokenSameLine(Node node)
        {
            Token last = getLastToken(node);
            Token token;

            if (last == null)
            {
                return null;
            }
            token = last.GetNextToken();
            while (token != null)
            {
                switch ((Asn1Constants) token.GetId())
                {
                    case Asn1Constants.WHITESPACE:
                    case Asn1Constants.COMMA:
                        // Skip to next
                        break;
                    case Asn1Constants.COMMENT:
                        if (last.GetEndLine() == token.GetStartLine())
                        {
                            return token;
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }
                token = token.GetNextToken();
            }
            return null;
        }

        /**
         * Returns the first token in a production.
         *
         * @param node           the production or token node
         *
         * @return the first token in the production, or
         *         null if none was found
         */
        private static Token getFirstToken(Node node)
        {
            while (node is Production) {
                node = node.GetChildAt(0);
            }
            return (Token)node;
        }

        /**
         * Returns the last token in a production.
         *
         * @param node           the production or token node
         *
         * @return the last token in the production, or
         *         null if none was found
         */
        private static Token getLastToken(Node node)
        {
            while (node is Production) {
                node = node.GetChildAt(node.GetChildCount() - 1);
            }
            return (Token)node;
        }

        /**
         * Returns a string containing the line breaks of an input
         * string.
         *
         * @param str            the input string
         *
         * @return a string containing zero or more line breaks
         */
        private static string getLineBreaks(string str)
        {
            StringBuilder res = new StringBuilder();

            if (str == null)
            {
                return null;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    res.Append('\n');
                }
            }
            return res.ToString();
        }
    }
}
