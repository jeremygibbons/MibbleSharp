// <copyright file="CombineElement.cs" company="None">
//    <para>
//    This program is free software: you can redistribute it and/or
//    modify it under the terms of the BSD license.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.</para>
//    <para>
//    See the LICENSE.txt file for more details.</para>
//    Original code as generated by Grammatica 1.6 Copyright (c) 
//    2003-2015 Per Cederberg. All rights reserved.
//    Updates Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace PerCederberg.Grammatica.Runtime.RE
{
    using System.IO;
    
     /// <summary>
     /// A regular expression combination element. This element matches
     /// two consecutive elements.
     /// </summary>
    internal class CombineElement : Element
    {
        /// <summary>
        /// The first element.
        /// </summary>
        private Element elem1;

        /// <summary>
        /// The second element.
        /// </summary>
        private Element elem2;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombineElement"/> class.
        /// </summary>
        /// <param name="first">The first element</param>
        /// <param name="second">The second element</param>
        public CombineElement(Element first, Element second)
        {
            this.elem1 = first;
            this.elem2 = second;
        }

        /// <summary>
        /// Creates a copy of this element. The copy will be an
        /// instance of the same class matching the same strings.
        /// Copies of elements are necessary to allow elements to cache
        /// intermediate results while matching strings without
        /// interfering with other threads.
        /// </summary>
        /// <returns>A copy of this object</returns>
        public override object Clone()
        {
            return new CombineElement(this.elem1, this.elem2);
        }

        /// <summary>
        /// Returns the length of a matching string starting at the
        /// specified position.The number of matches to skip can also be
        /// specified, but numbers higher than zero (0) cause a failed
        /// match for any element that doesn't attempt to combine other
        /// elements.
        /// </summary>
        /// <param name="m">the matcher being used</param>
        /// <param name="buffer">the input character buffer to match</param>
        /// <param name="start">The starting position</param>
        /// <param name="skip">the number of matches to skip</param>
        /// <returns>
        /// the length of the matching string, or -1 if no match was found
        /// </returns>
        public override int Match(
            Matcher m,
            ReaderBuffer buffer,
            int start,
            int skip)
        {
            int  length1 = -1;
            int  length2 = 0;
            int  skip1 = 0;
            int  skip2 = 0;

            while (skip >= 0) {
                length1 = this.elem1.Match(m, buffer, start, skip1);
                if (length1 < 0)
                {
                    return -1;
                }

                length2 = this.elem2.Match(m, buffer, start + length1, skip2);
                if (length2 < 0)
                {
                    skip1++;
                    skip2 = 0;
                }
                else
                {
                    skip2++;
                    skip--;
                }
            }

            return length1 + length2;
        }

        /// <summary>
        /// Prints this element to the specified output stream.
        /// </summary>
        /// <param name="output">The output stream to be used</param>
        /// <param name="indent">The current indentation</param>
        public override void PrintTo(TextWriter output, string indent) {
            this.elem1.PrintTo(output, indent);
            this.elem2.PrintTo(output, indent);
        }
    }
}