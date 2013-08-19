using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Security.Permissions;

using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    #region Delegates

    public delegate bool KeyPressed();
    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    #endregion

    public static class KeyboardHook
    {
        #region Fields

        private static HookProc hookproc = new HookProc(Filter);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private static IntPtr hHook = IntPtr.Zero;
        private static bool Control = false;
        private static bool Shift = false;
        private static bool Alt = false;
        private static bool Win = false;
        private static Dictionary<Keys, KeyPressed> handledKeysDown = new Dictionary<Keys, KeyPressed>();
        private static Dictionary<Keys, KeyPressed> handledKeysUp = new Dictionary<Keys, KeyPressed>();
        private delegate bool KeyboardHookHandler(Keys key);
        private static KeyboardHookHandler KeyDown;
        private static bool Enabled;

        #endregion

        #region Methods

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static bool Enable()
        {
            if (Enabled == false)
            {
                try
                {
                    using (Process curProcess = Process.GetCurrentProcess())
                    using (ProcessModule curModule = curProcess.MainModule)
                        hHook = NativeMethods.SetWindowsHookEx((int)NativeMethods.HookType.WH_KEYBOARD_LL, hookproc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
                    Enabled = true;
                    return true;
                }
                catch(InvalidOperationException)
                {
                    Enabled = false;
                    return false;
                }
            }
            else
                return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static bool Disable()
        {
            if (Enabled == true)
            {
                try
                {
                    NativeMethods.UnhookWindowsHookEx(hHook);
                    Enabled = false;
                    return true;
                }
                catch(InvalidOperationException)
                {
                    Enabled = true;
                    return false;
                }
            }
            else
                return false;
        }

        private static IntPtr Filter(int nCode, IntPtr wParam, IntPtr lParam)
        {
            bool result = true;

            if (nCode >= 0)
            {
                if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN
                    || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if ((Keys)vkCode == Keys.LControlKey ||
                        (Keys)vkCode == Keys.RControlKey)
                        Control = true;
                    else if ((Keys)vkCode == Keys.LShiftKey ||
                        (Keys)vkCode == Keys.RShiftKey)
                        Shift = true;
                    else if ((Keys)vkCode == Keys.RMenu ||
                        (Keys)vkCode == Keys.LMenu)
                        Alt = true;
                    else if ((Keys)vkCode == Keys.RWin ||
                        (Keys)vkCode == Keys.LWin)
                        Win = true;
                    else
                        result = OnKeyDown((Keys)vkCode);
                }
                else if (wParam == (IntPtr)NativeMethods.WM_KEYUP
                    || wParam == (IntPtr)NativeMethods.WM_SYSKEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    if ((Keys)vkCode == Keys.LControlKey ||
                        (Keys)vkCode == Keys.RControlKey)
                        Control = false;
                    else if ((Keys)vkCode == Keys.LShiftKey ||
                        (Keys)vkCode == Keys.RShiftKey)
                        Shift = false;
                    else if ((Keys)vkCode == Keys.RMenu ||
                        (Keys)vkCode == Keys.LMenu)
                        Alt = false;
                    else if ((Keys)vkCode == Keys.RWin ||
                        (Keys)vkCode == Keys.LWin)
                        Win = false;
                    else
                        result = OnKeyUp((Keys)vkCode);
                }
            }

            return result ? NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam) : new IntPtr(1);
        }

        public static bool AddKeyDown(Keys key, KeyPressed callback)
        {
            KeyDown = null;
            if (!handledKeysDown.ContainsKey(key))
            {
                handledKeysDown.Add(key, callback);
                return true;
            }
            else
                return false;
        }

        public static bool AddKeyUp(Keys key, KeyPressed callback)
        {
            if (!handledKeysUp.ContainsKey(key))
            {
                handledKeysUp.Add(key, callback);
                return true;
            }
            else
                return false;
        }

        public static bool Add(Keys key, KeyPressed callback)
        {
            return AddKeyDown(key, callback);
        }

        public static bool RemoveDown(Keys key)
        {
            return handledKeysDown.Remove(key);
        }

        public static bool RemoveUp(Keys key)
        {
            return handledKeysUp.Remove(key);
        }

        public static bool Remove(Keys key)
        {
            return RemoveDown(key);
        }

        private static bool OnKeyDown(Keys key)
        {
            if (KeyDown != null)
                return KeyDown(key);
            if (handledKeysDown.ContainsKey(key))
                return handledKeysDown[key]();
            else
                return true;
        }

        private static bool OnKeyUp(Keys key)
        {
            if (handledKeysUp.ContainsKey(key))
                return handledKeysUp[key]();
            else
                return true;
        }

        public static string KeyToString(Keys key)
        {
            return (KeyboardHook.Control ? "Ctrl + " : "") +
                            (KeyboardHook.Alt ? "Alt + " : "") +
                            (KeyboardHook.Shift ? "Shift + " : "") +
                            (KeyboardHook.Win ? "Win + " : "") +
                            key.ToString();
        }

        #endregion
    }
}
