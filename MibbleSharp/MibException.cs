using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp
{
    /**
     * A MIB exception. This exception is used to report processing
     * errors for operations on MIB types and values.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.0
     * @since    2.0
     */
    public class MibException : Exception
    {

    /**
     * The file location.
     */
    private FileLocation location;

    /**
     * Creates a new MIB exception.
     *
     * @param location       the file location
     * @param message        the error message
     */
    public MibException(FileLocation location, string message) : base(message)
    {
        this.location = location;
    }

    /**
     * Creates a new MIB exception.
     *
     * @param file           the file containing the error
     * @param line           the line number containing the error
     * @param column         the column number containing the error
     * @param message        the error message
     */
    public MibException(string file, int line, int column, string message) : this(new FileLocation(file, line, column), message)
    {

    }

    /**
     * Returns the error location.
     *
     * @return the error location
     */
    public FileLocation getLocation()
    {
        return location;
    }
}

}
