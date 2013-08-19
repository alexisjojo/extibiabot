using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Security.Permissions;
using System.Security;

using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{

    #region Enums

    public enum InputType
    {
        Simulate,
        Control
    }

    #endregion

    public class InputControl
    {
        #region Singleton

        private static InputControl _instance = new InputControl();

        public static InputControl Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Variables

        private int _clickTimeStart  = 40;
        private int _clickTimeEnd    = 60;
        private int _pressTimeStart  = 40;
        private int _pressTimeEnd    = 60;
        private int _mouseMoveTime = 300; //w ms
        private InputType _inputType = InputType.Simulate;

        public TimeSpan MouseMoveTime
        {
            get { return new TimeSpan(0, 0, 0, 0, this._mouseMoveTime); }
        }

        #endregion

        #region Constants

        private const int WM_CHAR = 0x102;
        private const int WM_SETTEXT = 0xC;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_CLOSE = 0x10;
        private const int WM_COMMAND = 0x111;
        private const int WM_CLEAR = 0x303;
        private const int WM_DESTROY = 0x2;
        private const int WM_GETTEXT = 0xD;
        private const int WM_GETTEXTLENGTH = 0xE;
        private const int WM_LBUTTONDBLCLK = 0x203;

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }


        #endregion

        #region Methods (to set properties of Input)

        public void SetInputMode(InputType type)
        {
            this._inputType = type;
        }

        public void SetClickTime(int x, int y)
        {
            _clickTimeStart = x;
            _clickTimeEnd = y;
        }

        public void SetPressTime(int x, int y)
        {
            _pressTimeStart = x;
            _pressTimeEnd = y;
        }

        public void SetMouseTime(int x)
        {
            _mouseMoveTime = x;
        }

        public void MouseMove(Point newPosition)
        {
            newPosition.X = newPosition.X + GameClient.TibiaWindow.Left;
            newPosition.Y = newPosition.Y + GameClient.TibiaWindow.Top;
            Point start = Cursor.Position;
            int sleep = 10;
            double deltaX = newPosition.X - start.X;
            double deltaY = newPosition.Y - start.Y;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double timeFraction = 0.0;
            do
            {
                timeFraction = (double)stopwatch.Elapsed.Ticks / MouseMoveTime.Ticks;
                if (timeFraction > 1.0)
                    timeFraction = 1.0;
                PointF curPoint = new PointF((float)(start.X + timeFraction * deltaX), (float)(start.Y + timeFraction * deltaY));
                if (NativeMethods.SetCursorPos(Point.Round(curPoint).X, Point.Round(curPoint).Y) == 0)
                {
                    throw new InvalidOperationException("Error");
                }
                Thread.Sleep(sleep);
            } while (timeFraction < 1.0);
        }

        #endregion

        #region Methods (Controlling and simulating)

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void LeftClickXY(int x, int y)
        {
            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0201, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0202, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    break;

                case InputType.Control:
                    MouseMove(new Point(x, y));
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void LeftClickPoint(Point p)
        {
            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0201, (IntPtr)0, (IntPtr)MakeLParam(p.X, p.Y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0202, (IntPtr)0, (IntPtr)MakeLParam(p.X, p.Y));
                    break;

                case InputType.Control:
                    MouseMove(p);
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, p.X + GameClient.TibiaWindow.Left, p.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, p.X + GameClient.TibiaWindow.Left, p.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void LeftClickLoc(Location l)
        {
            int x = GuiGameScreen.Instance.GetXfromLoc(l.X);
            int y = GuiGameScreen.Instance.GetYfromLoc(l.Y);

            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0201, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0202, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    break;

                case InputType.Control:
                    MouseMove(new Point(x, y));
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void RightClickXY(int x, int y)
        {
            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0204, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0205, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    break;

                case InputType.Control:
                    MouseMove(new Point(x, y));
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void RightClickPoint(Point p)
        {
            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0204, (IntPtr)0, (IntPtr)MakeLParam(p.X, p.Y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0205, (IntPtr)0, (IntPtr)MakeLParam(p.X, p.Y));
                    break;

                case InputType.Control:
                    MouseMove(p);
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X + GameClient.TibiaWindow.Left, p.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, p.X + GameClient.TibiaWindow.Left, p.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void RightClickLoc(Location l)
        {
            int x = GuiGameScreen.Instance.GetXfromLoc(l.X);
            int y = GuiGameScreen.Instance.GetYfromLoc(l.Y);

            switch (_inputType)
            {
                case InputType.Simulate:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0204, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0205, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                    break;

                case InputType.Control:
                    MouseMove(new Point(x, y));
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTDOWN, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    Thread.Sleep(Utils.RandomNumber(_clickTimeStart, _clickTimeEnd));
                    NativeMethods.mouse_event(MOUSEEVENTF_RIGHTUP, x + GameClient.TibiaWindow.Left, y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Drag(Point from, Point to)
        {
            switch (_inputType)
            {
                case InputType.Simulate:
                    if (from.X > 0 && to.X > 0)
                    {
                        int x = new Random().Next(2, 4);
                        int lpara = MakeLParam((int)from.X, (int)from.Y);
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, (int)0x201, (IntPtr)0, (IntPtr)lpara);
                        Cursor.Position = new Point(Cursor.Position.X - x, Cursor.Position.Y - x);
                        Thread.Sleep(Utils.RandomNumber(40, 75));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, (int)0x200, (IntPtr)0, (IntPtr)lpara);
                        Cursor.Position = new Point(Cursor.Position.X + x, Cursor.Position.Y + x);
                        Thread.Sleep(Utils.RandomNumber(40, 75));
                        lpara = MakeLParam((int)to.X, (int)to.Y);
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, (int)0x200, (IntPtr)0, (IntPtr)lpara);
                        Cursor.Position = new Point(Cursor.Position.X - x, Cursor.Position.Y - x);
                        Thread.Sleep(Utils.RandomNumber(40, 75));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, (int)0x202, (IntPtr)0x0001, (IntPtr)lpara);
                        Cursor.Position = new Point(Cursor.Position.X + x, Cursor.Position.Y + x);
                        Thread.Sleep(Utils.RandomNumber(40, 75));
                    }
                    break;

                case InputType.Control:
                    MouseMove(from);
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, from.X + GameClient.TibiaWindow.Left, from.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    MouseMove(to);
                    NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, to.X + GameClient.TibiaWindow.Left, to.Y + GameClient.TibiaWindow.Top, 0, (IntPtr)0);
                    break;
            }
        }

        #endregion

        #region Methods (Functions for bot)

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void UseHot(int hot)
        {
            bool ctrl = false, shift = false;

            if (hot >= 13 && hot <= 24)
            {
                shift = true;
            }
            else if (hot >= 25 && hot <= 36)
            {
                ctrl = true;
            }

            switch (hot)
            {
                case 1: hot = 0x70; break;
                case 2: hot = 0x71; break;
                case 3: hot = 0x72; break;
                case 4: hot = 0x73; break;
                case 5: hot = 0x74; break;
                case 6: hot = 0x75; break;
                case 7: hot = 0x76; break;
                case 8: hot = 0x77; break;
                case 9: hot = 0x78; break;
                case 10: hot = 0x79; break;
                case 11: hot = 0x7A; break;
                case 12: hot = 0x7B; break;
            }

            if (ctrl)
            {
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x11, (IntPtr)0);
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)hot, (IntPtr)0);
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x11, (IntPtr)0);
                Thread.Sleep(Utils.RandomNumber(100, 200));
            }
            else if (shift)
            {
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x10, (IntPtr)0);
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x69 + hot, (IntPtr)0);
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x10, (IntPtr)0);
                Thread.Sleep(Utils.RandomNumber(100, 200));
            }
            else
            {
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)hot, (IntPtr)0);
                NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)hot, (IntPtr)0);
                Thread.Sleep(Utils.RandomNumber(100, 200));
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void Turn(int dir)
        {

            switch (dir)
            {
                case 2:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x11, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x28, (IntPtr)0);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x28, (IntPtr)0);
                    Thread.Sleep(30); 
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x11, (IntPtr)0);
                    break;

                case 4:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x11, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x25, (IntPtr)0);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x25, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x11, (IntPtr)0);
                    break;

                case 6:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x11, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x27, (IntPtr)0);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x27, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x11, (IntPtr)0);
                    break;

                case 8:
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x11, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)0x26, (IntPtr)0);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x26, (IntPtr)0);
                    Thread.Sleep(30);
                    NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)0x11, (IntPtr)0);
                    break;
            }




        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static bool GoLocation(Location loc)
        {
            switch (Instance._inputType)
            {
                case InputType.Simulate:
                    if (Player.X == loc.X && Player.Y == loc.Y + 1) // up
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x48, 1), (IntPtr)0x1480001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x48, 1), (IntPtr)0x1480001);
                    }
                    if (Player.X == loc.X && Player.Y == loc.Y - 1) // down
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x50, 1), (IntPtr)0x1500001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x50, 1), (IntPtr)0x1500001);
                    }
                    if (Player.X == loc.X + 1 && Player.Y == loc.Y) // left
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4B, 1), (IntPtr)0x14b0001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4B, 1), (IntPtr)0x14b0001);
                    }
                    if (Player.X == loc.X - 1 && Player.Y == loc.Y) // right
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4D, 1), (IntPtr)0x14d0001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4D, 1), (IntPtr)0x14d0001);
                    }

                    if (Player.X == loc.X + 1 && Player.Y == loc.Y + 1) // up - left
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x47, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x47, 1), (IntPtr)0);
                    }
                    if (Player.X == loc.X - 1 && Player.Y == loc.Y + 1) // up - right
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x49, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x49, 1), (IntPtr)0);
                    }

                    if (Player.X == loc.X + 1 && Player.Y == loc.Y - 1) // down - left
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4f, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4f, 1), (IntPtr)0);
                    }
                    if (Player.X == loc.X - 1 && Player.Y == loc.Y - 1) // down - right
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x51, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x51, 1), (IntPtr)0);
                    }
                    break;

                case InputType.Control:
                    //MouseMove(new Point(x, y), MouseMoveTime);
                    //mouse_event(MOUSEEVENTF_LEFTDOWN, x + Client.TibiaWindow.Left, y + Client.TibiaWindow.Top, 0, 0);
                    Thread.Sleep(Utils.RandomNumber(Instance._clickTimeStart, Instance._clickTimeEnd));
                    //mouse_event(MOUSEEVENTF_LEFTUP, x + Client.TibiaWindow.Left, y + Client.TibiaWindow.Top, 0, 0);
                    break;

            }
            return Player.Location == loc;
            }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static bool GoLocation(Directions direction )
        {
            switch (Instance._inputType)
            {
                case InputType.Simulate:
                    if (direction == Directions.North) // up
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x48, 1), (IntPtr)0x1480001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x48, 1), (IntPtr)0x1480001);
                    }
                    if (direction == Directions.South) // down
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x50, 1), (IntPtr)0x1500001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x50, 1), (IntPtr)0x1500001);
                    }
                    if (direction == Directions.West) // left
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4B, 1), (IntPtr)0x14b0001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4B, 1), (IntPtr)0x14b0001);
                    }
                    if (direction == Directions.East) // right
                    {
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4D, 1), (IntPtr)0x14d0001);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.SendMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4D, 1), (IntPtr)0x14d0001);
                    }

                    if (direction == Directions.NorthWest) // up - left
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x47, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x47, 1), (IntPtr)0);
                    }
                    if (direction == Directions.NorthEast) // up - right
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x49, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x49, 1), (IntPtr)0);
                    }

                    if (direction == Directions.SouthWest) // down - left
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x4f, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x4f, 1), (IntPtr)0);
                    }
                    if (direction == Directions.SouthEast) // down - right
                    {
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0100, (IntPtr)NativeMethods.MapVirtualKey(0x51, 1), (IntPtr)0);
                        Thread.Sleep(Utils.RandomNumber(Instance._pressTimeStart, Instance._pressTimeEnd));
                        NativeMethods.PostMessage(GameClient.Tibia.MainWindowHandle, 0x0101, (IntPtr)NativeMethods.MapVirtualKey(0x51, 1), (IntPtr)0);
                    }
                    break;

                case InputType.Control:
                    //MouseMove(new Point(x, y), MouseMoveTime);
                    //mouse_event(MOUSEEVENTF_LEFTDOWN, x + Client.TibiaWindow.Left, y + Client.TibiaWindow.Top, 0, 0);
                    Thread.Sleep(Utils.RandomNumber(Instance._clickTimeStart, Instance._clickTimeEnd));
                    //mouse_event(MOUSEEVENTF_LEFTUP, x + Client.TibiaWindow.Left, y + Client.TibiaWindow.Top, 0, 0);
                    break;

            }
            return true;
        }

        #endregion
    }
}
