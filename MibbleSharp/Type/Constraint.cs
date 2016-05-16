
namespace MibbleSharp.Type
{
   
    /**
     * A MIB type constraint.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public interface Constraint
    {

        /**
         * Initializes the constraint. This will remove all levels of
         * indirection present, such as references to types or values. No
         * constraint information is lost by this operation. This method
         * may modify this object as a side-effect, and will be called by
         * the MIB loader.
         *
         * @param type           the type to constrain
         * @param log            the MIB loader log
         *
         * @throws MibException if an error was encountered during the
         *             initialization
         */
        void Initialize(MibType type, MibLoaderLog log);

        /**
         * Checks if the specified type is compatible with this
         * constraint.
         *
         * @param type            the type to check
         *
         * @return true if the type is compatible, or
         *         false otherwise
         */
        bool IsCompatible(MibType type);

        /**
         * Checks if the specified value is compatible with this
         * constraint.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        bool IsCompatible(MibValue value);
    }

}
