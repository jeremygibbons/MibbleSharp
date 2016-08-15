// <copyright file="IAssignableFrom.cs" company="None">
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
//    Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//    </para>
// </copyright>

namespace JunoSnmp.SMI
{
    /// <summary>
    /// This interface is applied to classes which can have their value assigned from another type.
    /// For example, IAssignableFrom&lt;string&gt; indicates that the class has a method SetValue(string val)
    /// which will allow it to determine its value from the passed string.
    /// </summary>
    /// <typeparam name="T">The type a value is assignable by</typeparam>
    public interface IAssignableFrom<T>
    {
        /// <summary>
        /// Sets the value of the object from a argument of type T
        /// </summary>
        /// <param name="val">The <c>T</c> value to assign from</param>
        void SetValue(T val);
    }
}
