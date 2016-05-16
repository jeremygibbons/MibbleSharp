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
        log.addError(file, -1, -1, message);
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
        return "found " + log.errorCount() + " MIB loader errors";
    }
}

}
