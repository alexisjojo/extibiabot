using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    public partial class HudWindow : Window
    {
        private Thread _hudDrawer;
        Canvas canvasObj = new Canvas();

        public HudWindow()
        {
            InitializeComponent();
            this.Top = GameClient.TibiaWindow.Top + GuiGameScreen.Instance.Height;
            this.Left = GameClient.TibiaWindow.Left + GuiGameScreen.Instance.Width;
            this.Content = canvasObj;
            this.Background = null;
            _hudDrawer = new Thread(() => Refresh());
            _hudDrawer.Start();
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
       
        public void Refresh()
        {
            while (true)
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => 
                {
                    if (NativeMethods.GetForegroundWindow() != GameClient.Tibia.MainWindowHandle)
                        this.Visibility = System.Windows.Visibility.Hidden;
                    else
                    {
                        this.Visibility = System.Windows.Visibility.Visible;
                        Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { InitChart(); }));
                    }
                }));
                
                Thread.Sleep(500);
            }
        }

        public void InitChart()
        {
            canvasObj.Children.Clear();

            /*
            Text(20.0, 10.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 25.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(20.0, 40.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.LimeGreen);
            Text(10.0, 60.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 85.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Goldenrod);
            Text(10.0, 100.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.AntiqueWhite);
            Text(20.0, 128.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Chartreuse);
            Text(10.0, 145.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Coral);
            Text(20.0, 160.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.DeepPink);
            Text(10.0, 180.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 205.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.LightGreen);
            Text(10.0, 220.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 240.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.LightSeaGreen);
            Text(10.0, 265.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(20.0, 280.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 300.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 325.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(20.0, 340.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 360.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 385.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(10.0, 400.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 428.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 445.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(20.0, 460.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 480.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(20.0, 505.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(10.0, 520.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Plum);
            Text(20.0, 540.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.White);
            Text(10.0, 565.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            Text(20.0, 580.0, "dfsafsdfsdfsdfsdfsdfsdfsdfsdfsfsfsdffsdfsdfsdfsdfsfsfs", Colors.Red);
            */

            if (CaveBot.Instance.DrawMap || CaveBot.Instance.DrawItems)
                DrawMap();
            if (CaveBot.Instance.DrawWay)
                DrawLocations(Walker.Instance.WayPathFinderNodes, false);

            //canvasObj.InvalidateArrange();
        }

        
       
        public void Text(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvasObj.Children.Add(textBlock);
        }

        public void DrawMap(bool onlyscreen = false)
        {
            DrawMap(null, false);
        }

        public void DrawMap(Collection<Location> locations, bool onlyscreen = false)
        {
            GuiGameScreen.Instance.UpdateGUI();

            Rectangle rect;

            Location baseloc = new Location();
            baseloc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));
            Collection<Tile> tiles = new Collection<Tile>();

            byte[,] map = new byte[1024, 1024];

            map = Walker.Instance.Map.GetMap(true, out tiles);


            foreach (Tile tile in tiles)
            {
                int x = tile.LocationOnMap.OffsetX;
                int y = tile.LocationOnMap.OffsetY;

                Location location = new Location(x, y, (int)Player.Z);

                int a = onlyscreen ? x : x + 256;
                int b = onlyscreen ? y : y + 256;

                bool walkable = map[tile.PathfinderPoint.X, tile.PathfinderPoint.Y] == (byte)4 ? true : false;

                rect = new Rectangle
                {
                    Width = (GuiGameScreen.Instance.Width / 15),
                    Height = (GuiGameScreen.Instance.Height / 11)
                };

                double cx = x * rect.Width + 2 + GuiGameScreen.Instance.PositionX;
                double cy = y * rect.Height + 2 + GuiGameScreen.Instance.PositionY;

                rect.RadiusX = 3;
                rect.RadiusY = 3;

                rect.SnapsToDevicePixels = true;

                rect.Stroke = Brushes.Green;

                int yspan = 0;

                if (walkable)
                    rect.Stroke = Brushes.Green;
                else
                    rect.Stroke = Brushes.Red;

                if (CaveBot.Instance.DrawItems)
                {
                    foreach (TileItem item in tile.GetItems())
                    {
                        if (item.id != 0)
                        {
                            //PipeClient.Instance.Send(new HudItem(string.Format("{0}{1}{2}", cx, cy, yspan), (uint)cx, (uint)cy + (uint)yspan, 255, 255, 255, 2, item.id.ToString()).ToNetworkMessage());
                            Text(cx, cy + yspan, item.id.ToString(), Colors.White);
                            yspan += 10;
                        }
                    }
                }


                Canvas.SetLeft(rect, cx);
                Canvas.SetTop(rect, cy);

                if (CaveBot.Instance.DrawMap)
                    canvasObj.Children.Add(rect);
            }
        }

        public void DrawLocations(Collection<Location> locations, bool onlyscreen = false)
        {
            GuiGameScreen.Instance.UpdateGUI();

            Rectangle rect;

            Location baseloc = new Location();
            baseloc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

            Location location;

            for (int x = 0; x < 15; x++)
                for (int y = 0; y < 11; y++)
                {
                    location = new Location((int)Player.X - 7 + x, (int)Player.Y - 5 + y, (int)Player.Z);

                    if (locations != null)
                        if (!locations.Contains(location))
                            continue;

                    int a = onlyscreen ? (int)Player.X - baseloc.X - 7 + x : (int)Player.X - baseloc.X - 7 + x + 256;
                    int b = onlyscreen ? (int)Player.Y - baseloc.Y - 5 + y : (int)Player.Y - baseloc.Y - 5 + y + 256;

                    rect = new Rectangle
                    {
                        Width = (GuiGameScreen.Instance.Width / 15),
                        Height = (GuiGameScreen.Instance.Height / 11)
                    };

                    double cx = x * rect.Width + 2 + GuiGameScreen.Instance.PositionX;
                    double cy = y * rect.Height + 2 + GuiGameScreen.Instance.PositionY;

                    rect.RadiusX = 3;
                    rect.RadiusY = 3;

                    rect.SnapsToDevicePixels = true;

                    Canvas.SetLeft(rect, cx);
                    Canvas.SetTop(rect, cy);

                    if (locations == null)
                    {

                    }
                    else
                    {
                        rect.Stroke = Brushes.Yellow;
                        Text(cx + 10, cy + 10, "Way", Colors.Yellow);
                    }

                    canvasObj.Children.Add(rect);
                }
        }
  }
    
}

