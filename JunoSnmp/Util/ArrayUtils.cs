using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JunoSnmp.Util
{
    static class ArrayUtils
    {
        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static void Populate<T>(this T[] arr, T value, int offset, int length)
        {
            if(offset + length > arr.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for(int i = offset; i < offset + length; i++)
            {
                arr[i] = value;
            }
        }
    }
}
