// <copyright file="TestTimeTicks.cs" company="None">
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
//    Original Java code from Snmp4J Copyright (C) 2003-2017 Frank Fock and 
//    Jochen Katz (SNMP4J.org). All rights reserved.
//    </para><para>
//    C# conversion Copyright (c) 2017 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.Test
{
    using JunoSnmp.SMI;
    using NUnit.Framework;

    [TestFixture]
    class TestTimeTicks
    {
        public TestTimeTicks()
        {
        }

        [TestCase]
        public void TestToString()
        {
            TimeTicks timeticks = new TimeTicks();
            string stringRet = timeticks.ToString();
            Assert.AreEqual("0:00:00.00", stringRet);
        }

        [TestCase]
        public void TestToMaxValue()
        {
            TimeTicks timeticks = new TimeTicks(4294967295L);
            string stringRet = timeticks.ToString();
            Assert.AreEqual("497 days, 2:27:52.95", stringRet);
        }
    }
}
