using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using MibbleSharp.Value;

namespace MibbleSharp.Type
{
    /**
     * A MIB type value range constraint. This class represents a value
     * range in a set of value constraints.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    public class ValueRangeConstraint : Constraint
    {

        /**
         * The constraint location. This value is reset to null once the
         * constraint has been initialized. 
         */
        private FileLocation location;

        /**
         * The lower bound value.
         */
        private NumberValue lower;

        /**
         * The upper bound value.
         */
        private NumberValue upper;

        /**
         * The strict lower bound flag.
         */
        private bool strictLower;

        /**
         * The strict upper bound flag.
         */
        private bool strictUpper;

        /**
         * Creates a new value range constraint.
         *
         * @param location       the constraint location
         * @param lower          the lower bound, or null for minimum
         * @param strictLower    the strict lower bound (less than) flag
         * @param upper          the upper bound, or null for maximum
         * @param strictUpper    the strict upper bound (greater than) flag
         */
        public ValueRangeConstraint(FileLocation location,
                                    NumberValue lower,
                                    bool strictLower,
                                    NumberValue upper,
                                    bool strictUpper)
        {

            this.location = location;
            this.lower = lower;
            this.upper = upper;
            this.strictLower = strictLower;
            this.strictUpper = strictUpper;
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

            string message;

            if (lower != null)
            {
                lower.Initialize(log, type);
            }
            if (upper != null)
            {
                upper.Initialize(log, type);
            }
            if (location != null && !IsCompatible(type))
            {
                message = "Value range constraint not compatible with " +
                          "this type";
                log.addWarning(location, message);
            }
            location = null;
        }

        /**
         * Checks if the specified type is compatible with this
         * constraint.
         *
         * @param type            the type to check
         *
         * @return true if the type is compatible, or
         *         false otherwise
         */
        public bool IsCompatible(MibType type)
        {
            return (type == null || lower == null || type.IsCompatible(lower))
                && (type == null || upper == null || type.IsCompatible(upper));
        }

        /**
         * Checks if the specified value is compatible with this
         * constraint.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         */
        public bool IsCompatible(MibValue value)
        {
            return (lower == null || isLessThan(strictLower, lower, value))
                && (upper == null || isLessThan(strictUpper, value, upper));
        }

        /**
         * Checks if the specified value is compatible with this
         * constraint.
         *
         * @param value          the value to check
         *
         * @return true if the value is compatible, or
         *         false otherwise
         *
         * @since 2.9
         */
        public bool IsCompatible(BigInteger value)
        {
            return (lower == null || isLessThan(strictLower, lower.value, value))
                && (upper == null || isLessThan(strictUpper, value, upper.value));
        }

        /**
         * Checks if one MIB value is less than another.
         *
         * @param strict         the strict less than flag
         * @param value1         the first value
         * @param value2         the second value
         *
         * @return true if the first value is less than the second, or
         *         false otherwise
         */
        private bool isLessThan(bool strict,
                                   MibValue value1,
                                   MibValue value2)
        {
            NumberValue nv1 = value1 as NumberValue;
            NumberValue nv2 = value2 as NumberValue;

            if (value1 != null && value2 != null)
            {

                return isLessThan(strict,
                                  nv1.value,
                                  nv2.value);
            }
            StringValue s1 = value1 as StringValue;
            StringValue s2 = value2 as StringValue;

            if (s1 != null && s2 != null)
            {

                return isLessThan(strict,
                                  s1.ToString(),
                                  s2.ToString());
            }
            else
            {
                return false;
            }
        }

        /**
         * Checks if a number is less than another.
         *
         * @param strict         the strict less than flag
         * @param value1         the first number
         * @param value2         the second number
         *
         * @return true if the first number is less than the second, or
         *         false otherwise
         */
        private bool isLessThan(bool strict,
                                   BigInteger value1,
                                   BigInteger value2)
        {

            return strict ? (value1 < value2) : (value1 <= value2);
        }

        /**
         * Checks if a string is less than another.
         *
         * @param strict         the strict less than flag
         * @param value1         the first string
         * @param value2         the second string
         *
         * @return true if the first string is less than the second, or
         *         false otherwise
         */
        private bool isLessThan(bool strict,
                                   string value1,
                                   string value2)
        {

            if (strict)
            {
                return value1.CompareTo(value2) < 0;
            }
            else
            {
                return value1.CompareTo(value2) <= 0;
            }
        }

        /**
         * Returns the lower bound value.
         *
         * @return the lower bound value, or null for minimum
         */
        public MibValue getLowerBound()
        {
            return lower;
        }

        /**
         * Returns the upper bound value.
         *
         * @return the upper bound value, or null for maximum
         */
        public MibValue getUpperBound()
        {
            return upper;
        }

        /**
         * Returns a string representation of this object.
         *
         * @return a string representation of this object
         */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (lower == null)
            {
                builder.Append("MIN");
            }
            else
            {
                builder.Append(lower);
            }
            if (strictLower)
            {
                builder.Append("<");
            }
            builder.Append("..");
            if (strictUpper)
            {
                builder.Append("<");
            }
            if (upper == null)
            {
                builder.Append("MAX");
            }
            else
            {
                builder.Append(upper);
            }

            return builder.ToString();
        }
    }

}
