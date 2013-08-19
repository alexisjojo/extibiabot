using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;
using System.Windows.Forms;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class GameClient
    {
        #region Fields

        private static Process tibia;
        private static IntPtr handle;
        private static NativeMethods.RECT tibiaWindow = new NativeMethods.RECT();

        #endregion

        #region Constructor

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public GameClient(TibiaClient TibiaClient)
        {
            Tibia = TibiaClient.Process;

            AssignProcess();
        }

        #endregion

        #region Properties

        public static Process Tibia
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return tibia; }
            set { tibia = value; }
        }

        public static IntPtr Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        internal static NativeMethods.RECT TibiaWindow
        {
            get { return tibiaWindow; }
            set { tibiaWindow = value; }
        }

        public static Process Process
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return Tibia; }
            set { Tibia = value; }
        }

        public static int ProcessID
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return Process.Id; }
        }

        #endregion

        #region Methods

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void AssignProcess()
        {
            try
            {
                Tibia.WaitForInputIdle();

                while (Tibia.MainWindowHandle == IntPtr.Zero)
                {
                    Tibia.Refresh();
                    System.Threading.Thread.Sleep(5);
                }

                Handle = NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, 0, (uint)Tibia.Id);

                AdjustTibiaWindow();
            }
            catch (InvalidOperationException e)
            {
                exTibia.Helpers.Debug.Report(e);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void AdjustTibiaWindow()
        {
            try
            {
                NativeMethods.GetWindowRect((IntPtr)Tibia.MainWindowHandle, out tibiaWindow);

                tibiaWindow.Top += SystemInformation.CaptionHeight + SystemInformation.BorderSize.Height + 3;
                tibiaWindow.Left += SystemInformation.BorderSize.Width + 3;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }
}
