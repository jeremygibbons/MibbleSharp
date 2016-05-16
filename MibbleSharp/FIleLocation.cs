//
// FileLocation.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp
{
    public class FileLocation
    {
        /**
        * The file name.
        */
        private string file;

        /**
         * The line number.
         */
        private int line;

        /**
         * The column number.
         */
        private int column;

        /**
         * Creates a new file location without an exact line or column
         * reference.
         *
         * @param file           the file name
         */
        public FileLocation(string file) : this(file, -1, -1)
        {
            
        }

        /**
         * Creates a new file location.
         *
         * @param file           the file name
         * @param line           the line number
         * @param column         the column number
         */
        public FileLocation(string file, int line, int column)
        {
            this.file = file;
            this.line = line;
            this.column = column;
        }

        /**
         * Returns the file name.
         *
         * @return the file name
         */
        public string File()
        {
            return file;
        }

        /**
         * Returns the line number.
         *
         * @return the line number
         */
        public int LineNumber()
        {
            return line;
        }

        /**
         * Returns the column number.
         *
         * @return the column number
         */
        public int ColumnNumber()
        {
            return column;
        }

        /**
         * Reads the specified line from the file. If the file couldn't
         * be opened or read correctly, null will be returned. The line
         * will NOT contain the terminating '\n' character. This method
         * takes special care to only count the linefeed (LF, 0x0A)
         * character as a valid newline.
         *
         * @return the line read, or
         *         null if not found
         */
        public string ReadLine()
        {
            string str = null;
            int count = 1;
            int ch;

            if (file == null || line < 0)
            {
                return null;
            }
            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                // Only count line-feed characters in files with invalid line
                // termination sequences. The default readLine() method doesn't
                // quite do the right thing in those cases... (bug #16252)
                while (count < line && (ch = sr.Read()) >= 0)
                {
                    if (ch == '\n')
                    {
                        count++;
                    }
                }
                str = sr.ReadLine();
            }
            return str;
        }
    }
}
