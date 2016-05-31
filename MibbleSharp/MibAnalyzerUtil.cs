// <copyright file="MibAnalyzerUtil.cs" company="None">
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
    using System.Collections.Generic;
    using System.Text;
    using MibbleSharp.Asn1;
    using PerCederberg.Grammatica.Runtime;

    /// <summary>
    /// Helper and utility functions for the MIB file analyzer.
    /// </summary>
    public class MibAnalyzerUtil
    {
        /// <summary>
        /// An internal hash map containing all the used comment tokens.
        /// When a comment string is returned by the getComments() method,
        /// the corresponding tokens will be added to this hash map and
        /// not returned on subsequent calls.
        /// </summary>
        private static HashSet<Token> commentTokens = new HashSet<Token>();

        /// <summary>
        /// Checks if a node corresponds to a bit value. This method is
        /// used to distinguish between bit values and object identifier
        /// values during the analysis.
        /// </summary>
        /// <param name="node">The parse tree node to check</param>
        /// <returns>True if the node contains a bit value, false if not</returns>
        public static bool IsBitValue(Node node)
        {
            if (node.Id == (int)Asn1Constants.COMMA)
            {
                return true;
            }
            else if (node.Id == (int)Asn1Constants.NAME_VALUE_LIST
                && node.ChildCount < 4)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < node.ChildCount; i++)
                {
                    if (IsBitValue(node[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns all the comments associated with a specified node. If
        /// there are multiple comment lines, these will be concatenated
        /// into a single string. This method handles comments before,
        /// inside and after (starting on the same line) as the specified
        /// node. It also updates the comment token cache to avoid
        /// returning the same comments twice.
        /// </summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The comment string, or null if no comments were found </returns>
        public static string GetComments(Node node)
        {
            string comment = string.Empty;
            string str;
            Token token;

            str = GetCommentsBefore(node);
            if (str != null)
            {
                comment = str;
            }

            str = GetCommentsInside(node);
            if (str != null)
            {
                if (comment.Length > 0)
                {
                    comment += "\n\n";
                }

                comment += str;
            }

            token = GetCommentTokenSameLine(node);
            if (token != null)
            {
                if (comment.Length > 0)
                {
                    comment += "\n\n";
                }

                token = token.GetPreviousToken();
                comment += GetCommentsAfter(token);
            }

            return comment.Length <= 0 ? null : comment;
        }

        /// <summary>
        /// Returns all the footer comments after the specified node. This
        /// method also clears the comment cache and should be called
        /// exactly once after each MIB file parsed, in order to free
        /// memory used by the comment token cache.
        /// </summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The comment string, or null if no comments were found</returns>
        public static string GetCommentsFooter(Node node)
        {
            string comment = GetCommentsAfter(node);

            commentTokens.Clear();
            return comment;
        }

        /// <summary>
        /// Returns all the comments before the specified node. If
        /// there are multiple comment lines possibly separated by
        /// whitespace, they will be concatenated into one string.
        /// </summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The comment string, or null if none was found</returns>
        private static string GetCommentsBefore(Node node)
        {
            Token token = GetFirstToken(node);
            List<string> comments = new List<string>();
            StringBuilder builder = new StringBuilder();
            string res = string.Empty;

            if (token != null)
            {
                token = token.GetPreviousToken();
            }

            while (token != null)
            {
                if (token.Id == (int)Asn1Constants.WHITESPACE)
                {
                    comments.Add(GetLineBreaks(token.GetImage()));
                }
                else if (token.Id == (int)Asn1Constants.COMMENT &&
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

        /// <summary>
        /// Returns all the comments after the specified node. If
        /// there are multiple comment lines possibly separated by
        /// whitespace, they will be concatenated into one string.
        /// </summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The comment string, or null if none was found</returns>
        private static string GetCommentsAfter(Node node)
        {
            Token token = GetLastToken(node);
            StringBuilder comment = new StringBuilder();
            string res;

            if (token != null)
            {
                token = token.GetNextToken();
            }

            while (token != null)
            {
                if (token.Id == (int)Asn1Constants.WHITESPACE)
                {
                    comment.Append(GetLineBreaks(token.GetImage()));
                }
                else if (token.Id == (int)Asn1Constants.COMMENT &&
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

        /// <summary>
        /// Returns all the unhandled comments inside the specified node.
        /// Note that only comment tokens not present in the token cache
        /// will be returned by this method.
        /// </summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The comment string or null if none was found</returns>
        private static string GetCommentsInside(Node node)
        {
            Token token = GetFirstToken(node);
            Token last = GetLastToken(node);
            StringBuilder comment = new StringBuilder();
            string res;

            while (token != null && token != last)
            {
                if (token.Id == (int)Asn1Constants.COMMENT &&
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

        /// <summary>Returns the first comment token on the same line.</summary>
        /// <param name="node">The production node</param>
        /// <returns>The first comment token on the same line</returns>
        /// @return the first comment token on the same line
        private static Token GetCommentTokenSameLine(Node node)
        {
            Token last = GetLastToken(node);
            Token token;

            if (last == null)
            {
                return null;
            }

            token = last.GetNextToken();
            while (token != null)
            {
                switch ((Asn1Constants)token.Id)
                {
                    case Asn1Constants.WHITESPACE:
                    case Asn1Constants.COMMA:
                        // Skip to next
                        break;
                    case Asn1Constants.COMMENT:
                        if (last.EndLine == token.StartLine)
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

        /// <summary>Returns the first token in a production.</summary>
        /// <param name="node">The production or node token</param>
        /// <returns>The first token in the production, or null if none was found</returns>
        private static Token GetFirstToken(Node node)
        {
            while (node is Production)
            {
                node = node[0];
            }

            return (Token)node;
        }

        /// <summary>Returns the last token in a production.</summary>
        /// <param name="node">The production or token node</param>
        /// <returns>The last token in the production, or null if none was found</returns>
        private static Token GetLastToken(Node node)
        {
            while (node is Production)
            {
                node = node[node.ChildCount - 1];
            }

            return (Token)node;
        }

        /// <summary>
        /// Returns a string containing the line breaks of an input string.
        /// </summary>
        /// <param name="str">The string to be processed</param>
        /// <returns>A string containing zero or more line breaks.</returns>
        private static string GetLineBreaks(string str)
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
