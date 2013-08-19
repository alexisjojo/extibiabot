using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Objects
{
    public class MapGrid
    {
        #region Constructors

        public MapGrid()
        {

        }

        public MapGrid(int x, int y, bool walkable)
        {
            _x = x;
            _y = y;
            _walkable = walkable;
        }

        public MapGrid(int x, int y, bool walkable, Objects.Location location)
        {
            _x = x;
            _y = y;
            _walkable = walkable;
            _location = location;
        }

        public MapGrid(Location loc, bool walkable, Tile tile)
        {
            _location = loc;
            _walkable = walkable;
            _tile = tile;
        }

        #endregion

        #region Variables

        private int _x;
        private int _y;
        private bool _walkable;
        private string _mapfile;
        private Tile _tile;
        private Location _location;

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

        public bool Walkable
        {
            get { return this._walkable; }
            set { this._walkable = value; }
        }

        public string MapFile
        {
            get { return this._mapfile; }
            set { this._mapfile = value; }
        }

        public Tile Tile
        {
            get { return this._tile; }
            set { this._tile = value; }
        }

        public Location Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        #endregion
    }
}
