using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class HelpMethods
    {
        public static bool RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeSpan)
        {
            bool success = false;
            int elapsed = 0;
            while ((!success) && (elapsed < timeSpan.TotalMilliseconds))
            {
                Thread.Sleep(1000);
                elapsed += 1000;
                success = task();
            }
            return success;
        }

        public static bool CheckValueInRange(int aStart, int aEnd, int aValueToTest)
        {
            bool ValueInRange = Enumerable.Range(aStart, aEnd).Contains(aValueToTest);
            return ValueInRange;
        }

        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        public static void AllocateConsole()
        {
            NativeMethods.AllocConsole();
            Console.Title = "ExTibia Debug Window";
            IntPtr hwnd = NativeMethods.FindWindow(null,"ExTibia Debug Window");
        }

        public static string GetEnumDesc(Enum value)
        {
            var type = value.GetType();

            var fi = type.GetField(value.ToString());

            var descriptions = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return descriptions.Length > 0 ? descriptions[0].Description : value.ToString();
        }
    }
}
