using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class ItemLocation
    {
        #region Fields

        private LocationType _locType;
        private Tile _tile;
        private int _x;
        private int _y;
        private int _z;
        private Location _location;
        private Container _container;
        private int _containerOrder;
        private Slot _slot;

        #endregion

        #region Properties

        public LocationType LocationType
        {
            get { return _locType; }
            set { _locType = value; }
        }

        public Tile Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public Container Container
        {
            get { return _container; }
            set { _container = value; }
        }

        public int ContainerOrder
        {
            get { return _containerOrder; }
            set { _containerOrder = value; }
        }

        public Slot Slot
        {
            get { return _slot; }
            set { _slot = value; }
        }

        #endregion

        #region Constructors

        public ItemLocation(Location location)
        {
            this.Location = location;
            this.LocationType = LocationType.Map;
        }
        public ItemLocation(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.LocationType = LocationType.Map;
        }
        public ItemLocation(Tile tile)
        {
            this.Tile = tile;
            this.LocationType = LocationType.Map;
        }
        public ItemLocation(Container container, int orderInContainer)
        {
            this.Container = container;
            this.ContainerOrder = orderInContainer;
            this.LocationType = LocationType.Container;
        }
        public ItemLocation(Slot slot)
        {
            this.Slot = slot;
            this.LocationType = LocationType.Slot;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (this.LocationType == LocationType.Container)
                return String.Format("{0}", Container.GuiName);
            if (this.LocationType == LocationType.Map)
                return String.Format("{0}", Location.ToString());
            if (this.LocationType == LocationType.Slot)
                return String.Format("{0}", 3);
            return base.ToString();
        }

        #endregion
    }
}
