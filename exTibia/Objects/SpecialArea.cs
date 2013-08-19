using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class SpecialArea
    {
        #region Fields

        private int _dimensionTop;
        private int _dimensionLeft;
        private int _dimensionRight;
        private int _dimensionDown;
        private Collection<Location> _locations;
        private Location _initialLocation;
        private bool _active = false;
        private string _name;
        private WalkSender _consideration;
        #endregion

        #region Properties

        public string SpecialName
        {
            get 
            { 
                if (!Active)
                    return String.Format("{0}", this._name); 
                else
                    return String.Format("-> {0}", this._name); 

            }
        }

        public string SpecialCords
        {
            get { return String.Format("L:{0} T:{1} R:{2} D:{3}", _dimensionLeft, _dimensionTop, _dimensionRight, _dimensionDown); }
        }

        public WalkSender Consideration
        {
            get { return this._consideration; }
            set { _consideration = value; }
        }

        public int DimensionTop
        {
            get { return this._dimensionTop; }
            set { _dimensionTop = value; }
        }

        public int DimensionLeft
        {
            get { return this._dimensionLeft; }
            set { _dimensionLeft = value; }
        }

        public int DimensionRight
        {
            get { return this._dimensionRight; }
            set { _dimensionRight = value; }
        }

        public int DimensionDown
        {
            get { return this._dimensionDown; }
            set { _dimensionDown = value; }
        }

        public Collection<Location> Locations
        {
            get { return this._locations; }
        }

        public Location InitialLocation
        {
            get { return this._initialLocation; }
            set { _initialLocation = value; }
        }

        public bool Active
        {
            get { return this._active; }
            set { _active = value; }
        }

        public string Name
        {
            get { return this._name; }
            set { _name = value; }
        }

        public BitmapSource Image
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return ConvertToBitmapSource(RenderImage()); }
        }

        #endregion

        #region Constructor

        public SpecialArea(string name, Collection<Location> locations, WalkSender consideration)
        {
            this._name = name;
            this._consideration = consideration;
            this._locations = locations;
            this._initialLocation = Player.Location;
        }

        public SpecialArea(string name, int left, int top, int right, int down, WalkSender consideration)
        {
            this._name = name;
            this._dimensionLeft = left;
            this._dimensionDown = down;
            this._dimensionRight = right;
            this._dimensionTop = top;
            this._consideration = consideration;
            this._locations = Location.GetLocationsDimension(DimensionLeft,DimensionTop,DimensionRight,DimensionDown);
            this._initialLocation = Player.Location;
        }

        public SpecialArea(string name, int left, int top, int right, int down, WalkSender consideration, Location baseLocation)
        {
            this._name = name;
            this._dimensionLeft = left;
            this._dimensionDown = down;
            this._dimensionRight = right;
            this._dimensionTop = top;
            this._consideration = consideration;
            this._locations = Location.GetLocationsDimension(baseLocation,DimensionLeft, DimensionTop, DimensionRight, DimensionDown);
            this._initialLocation = Player.Location;
        }

        #endregion

        #region Methods

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public BitmapSource UpdatedImage()
        {
            try
            {
                return ConvertToBitmapSource(RenderImage());
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                return null;

            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public Bitmap RenderImage()
        {
            Bitmap bitmap = new Bitmap(315,250);
            Bitmap player = global::exTibia.Properties.Resources.Tibia;

            this._locations = Location.GetLocationsDimension(DimensionLeft, DimensionTop, DimensionRight, DimensionDown);

            try
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    Location current;

                    using (Pen pen = new Pen(Brushes.Black))
                    {
                        for (int x = 0; x < 15; x++)
                            for (int y = 0; y < 11; y++)
                            {
                                current = new Location(InitialLocation.X - 7 + x, InitialLocation.Y - 5 + y, InitialLocation.Z);
                                if (x == 7 && y == 5)
                                    g.DrawImage(player, x * 20 + 1, y * 20 + 1, 15, 15);
                                else
                                {
                                    if (this.Locations.Contains(current))
                                        g.FillEllipse(Brushes.Green, x * 20 + 1, y * 20 + 1, 15, 15);
                                    else
                                        g.DrawRectangle(pen, x * 20 + 1, y * 20 + 1, 15, 15);
                                }
                            }
                    }

                }
                return bitmap;
            }
            catch (InvalidOperationException ex)
            {
                bitmap.Dispose();
                Helpers.Debug.Report(ex);
                return null;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static BitmapSource ConvertToBitmapSource(System.Drawing.Bitmap gdiPlusBitmap)
        {
            IntPtr hBitmap = gdiPlusBitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(this.Name);
        }

        #endregion
    }
}
