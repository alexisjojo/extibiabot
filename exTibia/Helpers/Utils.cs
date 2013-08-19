using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;
using System.Windows.Forms;

using exTibia.Objects;
using exTibia.Modules;
using System.Threading;

namespace exTibia.Helpers
{
    public static class Utils
    {

        internal static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        internal static Thread sayT;

        internal static void Say(string words)
        {
            sayT = new Thread(() => SayT(words));
            sayT.Start();
            sayT.Join();
        }

        internal static void SayT(string words)
        {
            int size = words.Count();

            char[] arr = new char[size];

            for (int i = 0; i < size; i++)
            {
                arr[i] = words[i];
            }

            foreach (char c in arr)
            {
                NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0102, (IntPtr)Convert.ToInt32(c), (IntPtr)0);
                Thread.Sleep(Utils.RandomNumber(100, 200));
            }
            NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0102, (IntPtr)0x0D, (IntPtr)0);
            sayT.Abort();
        }

        internal static int Positive(int value)
        {
            int num = value;
            if (value < 0)
                num = 0;
            return num;
        }

        internal static string ByteToString(byte[] value, int start, int count)
        {
            string str = new ASCIIEncoding().GetString(value, start, count);
            if (str.IndexOf("\0") != -1)
                str = str.Substring(0, str.IndexOf("\0"));
            return str;
        }
    }
}
