// <copyright file="TimeTicks.cs" company="None">
//    <para>
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at</para>
//    <para>
//    http://www.apache.org/licenses/LICENSE-2.0
//    </para><para>
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.</para>
//    <para>
//    Original Java code from Snmp4J Copyright (C) 2003-2016 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.SMI
{
    using System;
    using System.IO;
    using JunoSnmp.ASN1;

    /// <summary>
    /// The <see cref="TimeTicks"/> class represents the time in 1/100 seconds
    /// since some epoch (which should be have been defined in the
    /// corresponding MIB specification).
    /// </summary>
    public class TimeTicks : UnsignedInteger32
    {

        private static readonly string FormatPattern =
          "{0,choice,0#|1#1 day, |1<{0,number,integer} days, }" +
          "{1,number,integer}:{2,number,00}:{3,number,00}.{4,number,00}";
        private static readonly int[] FormatFactors = { 24 * 60 * 60 * 100, 60 * 60 * 100, 60 * 100, 100, 1 };

        public TimeTicks()
        {
        }

        /**
         * Copy constructor.
         * @param other
         *    a TimeTicks instance.
         * @since 1.7
         */
        public TimeTicks(TimeTicks other)
        {
            this.value = other.value;
        }

        public TimeTicks(long value) : base(value)
        {
        }

        public override object Clone()
        {
            return new TimeTicks(value);
        }

        public override int Syntax
        {
            get
            {
                return SMIConstants.SyntaxTimeTicks;
            }
        }

        public override void EncodeBER(Stream os)
        {
            BER.EncodeUnsignedInteger(os, BER.TIMETICKS, base.GetValue());
        }

        public override void DecodeBER(BERInputStream inputStream)
        {
            BER.MutableByte type = new BER.MutableByte();
            long newValue = BER.DecodeUnsignedInteger(inputStream, out type);
            if (type.Value != BER.TIMETICKS)
            {
                throw new IOException("Wrong type encountered when decoding TimeTicks: " + type.Value);
            }

            this.SetValue(newValue);
        }

        /**
         * Returns string with the value of this <code>TimeTicks</code> object as
         * "[days,]hh:mm:ss.hh".
         *
         * @return
         *    a <code>String</code> representation of this object.
         */
        public override string ToString()
        {
            return ToString(TimeTicks.FormatPattern);
        }

        /**
         * Sets the value of this TimeTicks instance from a string.
         *
         * @param value
         *    a string representation of this value, which is
         *    (a) is either an unsigned number or
         *    (b) matches the format {@link FORMAT_PATTERN}.
         * @since 2.1.2
         */
        public override void SetValue(string value)
        {
            try
            {
                long v = long.Parse(value);
                this.SetValue(v);
            }
            catch (FormatException)
            {
                long v = 0;
                string[] num = value.Split(new string[] { "days :", "\\." }, StringSplitOptions.RemoveEmptyEntries);
                int i = 0;
                foreach (string n in num)
                {
                    if (n.Length > 0)
                    {
                        long f = TimeTicks.FormatFactors[i++];
                        v += long.Parse(n) * f;
                    }
                }

                this.SetValue(v);
            }
        }


        /**
         * Formats the content of this <code>TimeTicks</code> object according to
         * a supplied <code>MessageFormat</code> pattern.
         * @param pattern
         *    a <code>MessageFormat</code> pattern that takes up to five parameters
         *    which are: days, hours, minutes, seconds, and 1/100 seconds.
         * @return
         *    the formatted string representation.
         */
        public string ToString(string pattern)
        {
            long hseconds, seconds, minutes, hours, days;
            long tt = this.GetValue();

            days = tt / 8640000;
            tt %= 8640000;

            hours = tt / 360000;
            tt %= 360000;

            minutes = tt / 6000;
            tt %= 6000;

            seconds = tt / 100;
            tt %= 100;

            hseconds = tt;

            long[] values = new long[5];
            values[0] = days;
            values[1] = hours;
            values[2] = minutes;
            values[3] = seconds;
            values[4] = hseconds;

            return string.Format(pattern, values);
        }

        /**
         * Returns the timeticks value as milliseconds (instead 1/100 seconds).
         * @return
         *    <code>getValue()*10</code>.
         * @since 1.7
         */
        public long ToMilliseconds()
        {
            return value * 10;
        }

        /**
         * Sets the timeticks value by milliseconds.
         * @param millis
         *    sets the value as <code>setValue(millis/10)</code>.
         * @since 1.7
         */
        public void FromMilliseconds(long millis)
        {
            this.SetValue(millis / 10);
        }
    }


}
