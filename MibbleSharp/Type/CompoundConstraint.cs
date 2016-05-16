using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MibbleSharp.Type
{
    /**
     * A compound MIB type constraint. This class holds two constraints,
     * either one that must be compatible for this constraint to return
     * true. Effectively this class represents an OR composition of the
     * two constraints.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.6
     * @since    2.0
     */
    public class CompoundConstraint : Constraint
    {

    /**
     * The first constraint.
     */
    private Constraint first;

    /**
     * The second constraint.
     */
    private Constraint second;

    /**
     * Creates a new compound constraint.
     *
     * @param first          the first constraint
     * @param second         the second constraint
     */
    public CompoundConstraint(Constraint first, Constraint second)
    {
        this.first = first;
        this.second = second;
    }

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
    public void Initialize(MibType type, MibLoaderLog log)
    {

        first.Initialize(type, log);
        second.Initialize(type, log);
    }

    /**
     * Checks if the specified type is compatible with this
     * constraint. The type is considered compatible if it is
     * compatible with both constraints.
     *
     * @param type            the type to check
     *
     * @return true if the type is compatible, or
     *         false otherwise
     */
    public bool IsCompatible(MibType type)
    {
        return first.IsCompatible(type) && second.IsCompatible(type);
    }

    /**
     * Checks if the specified value is compatible with this
     * constraint set. The value is considered compatible if it is
     * compatible with either of the two constraints.
     *
     * @param value          the value to check
     *
     * @return true if the value is compatible, or
     *         false otherwise
     */
    public bool IsCompatible(MibValue value)
    {
        return first.IsCompatible(value) || second.IsCompatible(value);
    }

    /**
     * Returns a list of the constraints in this compound. All
     * compound constraints will be flattened out and their contents
     * will be added to the list.
     *
     * @return a list of the base constraints in the compound
     */
    public IList<Constraint> getConstraintList()
    {
            List<Constraint> list = new List<Constraint>();

        if (first is CompoundConstraint) {
            list.AddRange(((CompoundConstraint)first).getConstraintList());
        } else {
            list.Add(first);
        }
        if (second is CompoundConstraint) {
            list.AddRange(((CompoundConstraint)second).getConstraintList());
        } else {
            list.Add(second);
        }
        return list;
    }

    /**
     * Returns a string representation of this object.
     *
     * @return a string representation of this object
     */
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append(first.ToString());
        builder.Append(" | ");
        builder.Append(second.ToString());

        return builder.ToString();
    }
}

}
