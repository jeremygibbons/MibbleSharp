//
// MibLoaderException.cs
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

namespace MibbleSharp
{
    /**
     * A MIB loader exception. This exception is thrown when a MIB file
     * couldn't be loaded properly, normally due to syntactical or
     * semantical errors in the file.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.3
     * @since    2.0
     */
    [Serializable]
    public class MibLoaderException : Exception
    {

    /**
     * The MIB loader log.
     */
    private MibLoaderLog log;

    /**
     * Creates a new MIB loader exception.
     *
     * @param log            the MIB loader log
     */
    public MibLoaderException(MibLoaderLog log)
    {
        this.log = log;
    }

    /**
     * Creates a new MIB loader exception. The specified message will
     * be added to a new MIB loader log as an error.
     *
     * @param file           the file containg the error
     * @param message        the error message
     *
     * @since 2.3
     */
    public MibLoaderException(string file, string message)
    {
        log = new MibLoaderLog();
        log.AddError(file, -1, -1, message);
    }

    /**
     * Returns the MIB loader log.
     *
     * @return the MIB loader log
     */
    public MibLoaderLog getLog()
    {
        return log;
    }

    /**
     * Returns a error summary message.
     *
     * @return a error summary message
     */
    public string getMessage()
    {
        return "found " + log.ErrorCount + " MIB loader errors";
    }
}

}
