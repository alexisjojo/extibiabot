using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Location
    {

        #region Fields

        private int _x = -1;
        private int _y = -1;
        private int _z = -1;
        private int _offsetX = -1;
        private int _offsetY = -1;

        #endregion

        #region Constructors

        public Location()
        {

        }
        public Location(int x, int y, int z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        public Location(uint x, uint y, uint z)
        {
            this._x = (int)x;
            this._y = (int)y;
            this._z = (int)z;
        }

        public Location(int x, int y, int z, int offsetX, int offsetY)
        {
            this._x = x;
            this._y = y;
            this._z = z;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
        }

        #endregion

        #region Properties

        public int X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public int Z
        {
            get { return this._z; }
            set { this._z = value; }
        }

        public int OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        public int OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }

        #endregion

        #region Methods

        public static Collection<Location> Locations()
        {
            Collection<Location> locations = new Collection<Location>();

            for ( int x=0; x < 15; x++ )
            {
                for ( int y=0; y < 11; y++ )
                {
                    locations.Add(new Location(Player.X - 7 + x, Player.Y - 5 + y, Player.Z, x, y));
                }
            }

            return locations;
        }

        public static Collection<Location> GetLocationsDimension(int left, int top, int right, int down)
        {
            Collection<Location> result = new Collection<Location>();
            Location InitialPosition = Player.Location;

            for (int i = 0; i < left; i++)
                result.Add(new Location(InitialPosition.X - left + i, InitialPosition.Y, InitialPosition.Z));

            for (int i = 0; i <= right; i++)
                result.Add(new Location(InitialPosition.X + i, InitialPosition.Y, InitialPosition.Z));

            for (int i = 0; i < top; i++)
                result.Add(new Location(InitialPosition.X, InitialPosition.Y - top + i, InitialPosition.Z));

            for (int i = 0; i <= down; i++)
                result.Add(new Location(InitialPosition.X, InitialPosition.Y + i, InitialPosition.Z));

            return result;
        }

        public static Collection<Location> GetLocationsDimension(Location baseLocation, int left, int top, int right, int down)
        {
            Collection<Location> result = new Collection<Location>();
            Location InitialPosition = baseLocation;

            for (int i = 0; i < left; i++)
                result.Add(new Location(InitialPosition.X - left + i, InitialPosition.Y, InitialPosition.Z));

            for (int i = 0; i <= right; i++)
                result.Add(new Location(InitialPosition.X + i, InitialPosition.Y, InitialPosition.Z));

            for (int i = 0; i < top; i++)
                result.Add(new Location(InitialPosition.X, InitialPosition.Y - top + i, InitialPosition.Z));

            for (int i = 0; i <= down; i++)
                result.Add(new Location(InitialPosition.X, InitialPosition.Y + i, InitialPosition.Z));

            return result;
        }

        public static Location LocationDir(LocDirection direction, Location baselocation)
        {
            switch (direction)
            {
                case LocDirection.Up:
                    return new Location(baselocation.X, baselocation.Y - 1, baselocation.Z);
                case LocDirection.Down:
                    return new Location(baselocation.X, baselocation.Y + 1, baselocation.Z);
                case LocDirection.Right:
                    return new Location(baselocation.X + 1, baselocation.Y, baselocation.Z);
                case LocDirection.Left:
                    return new Location(baselocation.X - 1, baselocation.Y, baselocation.Z);
            }
            return new Location();
        }

        public bool IsValid()
        {
            return X >= 0 && Y >= 0 && Z >= 0;
        }

        public bool IsAdjacentTo( Location loc )
        {
            return loc.Z == Z && Math.Max(Math.Abs(X - loc.X), Math.Abs(Y - loc.Y)) <= 1;
        }

        public bool IsOnScreen()
        {
            return ((Math.Abs(X - Player.X) <= 7) && (Math.Abs(Y - Player.Y) <= 5) && Z == Player.Z) ? true : false;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", X, Y, Z);
        }

        public double DistanceTo( Location l )
        {
            int xDist = X - l.X;
            int yDist = Y - l.Y;
            int x = (int)Math.Sqrt(xDist * xDist + yDist * yDist);
            return x;
        }

        public override bool Equals( object other )
        {
            return other is Location && Equals((Location)other);
        }

        public bool Equals( Location other )
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }

        public static bool operator ==( Location me, Location other )
        {
            return me.Equals(other);
        }

        public static bool operator !=( Location me, Location other )
        {
            return !me.Equals(other);
        }

        public Location Offset( int x, int y, int z )
        {
            return new Location(X + x, Y + y, Z + z);
        }

        public override int GetHashCode()
        {
            ushort shortX = (ushort)X;
            ushort shortY = (ushort)Y;
            byte byteZ = (byte)Z;
            return ((shortX << 3) + (shortY << 1) + byteZ).GetHashCode();
        }

        #endregion
    }
}
