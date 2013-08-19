using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using exTibia.Modules;
using exTibia.Objects;
using System.Diagnostics;

namespace exTibia.Helpers
{
    public class Injector
    {

        #region Singleton

        private static Injector _instance = new Injector();

        public static Injector Instance
        {
            get { return _instance; }
        }

        #endregion

         Process clientProcess;
         IntPtr ProcessHandle;

        public void Inject()
        {
            clientProcess = GameClient.Process;
            string dllPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath.ToString(), "exCore.dll");
            ProcessHandle = NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, 0, (uint)GameClient.Process.Id);
            Inject(dllPath);
        }

        public bool Inject(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Dll to inject does not exist: " + filename);
            }

            IntPtr remoteAddress = NativeMethods.VirtualAllocEx(
                ProcessHandle,
                IntPtr.Zero,
                (uint)filename.Length,
                NativeMethods.AllocationType.Commit | NativeMethods.AllocationType.Reserve,
                NativeMethods.MemoryProtection.ExecuteReadWrite);

            Memory.WriteStringNoEncoding(ProcessHandle, remoteAddress.ToInt32(), filename);
            
            IntPtr thread = NativeMethods.CreateRemoteThread(
                ProcessHandle, IntPtr.Zero, 0,
                NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("Kernel32"), "LoadLibraryA"),
                remoteAddress, 0, IntPtr.Zero);

            NativeMethods.WaitForSingleObject(thread, 0xFFFFFFFF);

            NativeMethods.VirtualFreeEx(
                ProcessHandle,
                remoteAddress,
                (uint)filename.Length,
                NativeMethods.AllocationType.Release);

            return thread.ToInt32() > 0 && remoteAddress.ToInt32() > 0;
        }

    }
}
