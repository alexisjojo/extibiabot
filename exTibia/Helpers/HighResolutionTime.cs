using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class HighResolutionTime
    {
        #region Variables Declaration
        private static long mStartCounter;
        private static long mFrequency;
        #endregion

        #region Constuctors
        static HighResolutionTime()
        {
            NativeMethods.QueryPerformanceFrequency(out mFrequency);
        }
        #endregion

        #region Methods
        public static void Start()
        {
            NativeMethods.QueryPerformanceCounter(out mStartCounter);
        }

        public static double Time()
        {
            long endCounter;
            NativeMethods.QueryPerformanceCounter(out endCounter);
            long elapsed = endCounter - mStartCounter;
            return (double)elapsed / mFrequency;
        }
        #endregion
    }
}
