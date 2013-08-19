using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Drawing;

using exTibia.Helpers;
using exTibia.Modules;


namespace exTibia.Objects
{
    #region Structs

    public struct TileItem
    {
        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        private int _data;

        public int data
        {
            get { return _data; }
            set { _data = value; }
        }
        private int _dataEx;

        public int dataEx
        {
            get { return _dataEx; }
            set { _dataEx = value; }
        }
    }

    public struct TileLocation
    {
        private int _x;

        public int x
        {
            get { return _x; }
            set { _x = value; }
        }
        private int _y;

        public int y
        {
            get { return _y; }
            set { _y = value; }
        }
        private int _z;

        public int z
        {
            get { return _z; }
            set { _z = value; }
        }
    }

    #endregion

    public class Tile
    {
        #region Constructors

        public Tile()
        {
        }

        public Tile(int index, TileItem[] items, int itemcount, TileLocation global, TileLocation local, int[] ord)
        {
            this._index = index;
            this._itemCount = itemcount;
            this._globalLocation = global;
            this._items = items;
            this._memoryLocation = local;
            this._order = ord;
        }

        #endregion

        #region Variables

        private int _index;
        private TileItem[] _items;
        private int _itemCount;
        private int[] _order;
        private TileLocation _globalLocation;
        private TileLocation _memoryLocation;
        private Point _pathfinderPoint = new Point();
        private exTibia.Objects.Location _location;

        #endregion

        #region Constants

        public const int num_trappers = 8;
        public const int UNDERGROUND_LAYER = 2;
        public const int GROUND_LAYER = 7;
        public const int MAPSIZE_w = 10;
        public const int MAPSIZE_X = 18;
        public const int MAPSIZE_Y = 14;
        public const int MAPSIZE_Z = 8;
        public const int MAP_MAX_Z = 15;

        #endregion

        #region Properties

        public int Index
        {
            get { return this._index; }
            set { this._index = value; }
        }

        public int[] GetOrder()
        {
            return (int[])_order;
        }

        public int ItemCount
        {
            get { return this._itemCount; }
            set { this._itemCount = value; }
        }

        public TileItem[] GetItems()
        {        
            return (TileItem[])_items;
        }

        public TileLocation GlobalLocation
        {
            get { return this._globalLocation; }
            set { this._globalLocation = value; }
        }

        public TileLocation MemoryLocation
        {
            get { return this._memoryLocation; }
            set { this._memoryLocation = value; }
        }

        public exTibia.Objects.Location LocationOnMap
        {
            get
            {
                return _location;
            }
            set { this._location = value; }
        }

        public Point PathfinderPoint
        {
            get { return _pathfinderPoint; }
            set { this._pathfinderPoint = value; }
        }

        public bool IsBlockingPath
        {
            get { return GetItems().Any(i => DatItems.GetFlag(i.id, ItemFlags.BlocksPath)); }
        }

        public bool HasField
        {
            get { return HasFire || HasPoison || HasEnergy; }
        }

        public bool HasFire
        {
            get { return GetItems().Any(i => Consts.Fire_IDs.Where(j => j == i.id).Count() > 0); }
        }

        public bool HasPoison
        {
            get { return GetItems().Any(i => Consts.Poision_IDs.Where(j => j == i.id).Count() > 0); }
        }

        public bool HasEnergy
        {
            get { return GetItems().Any(i => Consts.Energy_IDs.Where(j => j == i.id).Count() > 0); }
        }

        public bool HasCreature
        {
            get { return GetItems().Any(i => i.id == 99); }
        }

        public bool HasPlayer
        {
            get { return GetItems().Any(i => i.id == 99 && i.data < 0x40000000); }
        }

        public Collection<int> IdList()
        {
            Collection<int> result = new Collection<int>();
            GetItems().ToList().ForEach(t =>
            {
                result.Add(t.id);
            });
            return result;
        }

        #endregion

        #region Methods

        public static int TileToAddress(int index, int address)
        {
            try
            {
                return address + ((int)Addresses.Map.StepTile * index);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public static int TileToAddress(int index)
        {
            try
            {
                int addr = Memory.ReadInt32(Addresses.Map.Pointer);
                return addr + ((int)Addresses.Map.StepTile * index);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public static int ToTileNumber(TileLocation local)
        {
            try
            {
                return local.x + local.y * 18 + local.z * 14 * 18;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public static TileLocation tileToGlobal(Tile tile, Tile PlayerTile)
        {
            try
            {
                TileLocation result = new TileLocation();
                TileLocation loc = new TileLocation();
                TileLocation playerMemLoc, playerGloLoc;

                loc = tile.MemoryLocation;
                playerMemLoc = PlayerTile.MemoryLocation;

                int diffX = 0;
                int diffY = 0;
                int maxX = 0;
                int maxY = 0;

                diffX = 8 - playerMemLoc.x;
                diffY = 6 - playerMemLoc.y;
                loc.x = loc.x + diffX;
                loc.y = loc.y + diffY;

                maxY = (int)Addresses.Map.MaxY + 1;
                maxX = (int)Addresses.Map.MaxX;

                if (loc.x > maxX)
                {
                    loc.x = loc.x - (int)Addresses.Map.MaxX;
                    //loc.y = loc.y + 1;
                }
                else if (loc.x < 0)
                {
                    loc.x = loc.x + (int)Addresses.Map.MaxX;
                    //loc.y = loc.y - 1;
                }
                else if (loc.y > maxY)
                {
                    loc.y = loc.y - (int)Addresses.Map.MaxY;
                }
                else if (loc.y < 0)
                {
                    loc.y = loc.y + (int)Addresses.Map.MaxY;
                }

                playerGloLoc = PlayerTile.GlobalLocation;

                result.x = playerGloLoc.x + (loc.x - 8);
                result.y = playerGloLoc.y + (loc.y - 6);
                result.z = playerGloLoc.z + (loc.z - playerMemLoc.z);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new TileLocation();
        }

        public static TileLocation tileToLocal(int index)
        {
            try
            {
                TileLocation result = new TileLocation();

                result.z = Convert.ToInt32(index / (14 * 18));
                result.y = Convert.ToInt32(index - result.z * 14 * 18) / 18;
                result.x = Convert.ToInt32(index - result.z * 14 * 18) - result.y * 18;
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new TileLocation();
        }

        public static TileLocation PlayerLocation()
        {
            try
            {
                TileLocation loc = new TileLocation();
                loc.x = (int)Player.X;
                loc.y = (int)Player.Y;
                loc.z = (int)Player.Z;
                return loc;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new TileLocation();
        }

        public static TileLocation globalToLocal(TileLocation loc)
        {
            try
            {
                TileLocation result = new TileLocation();
                TileLocation playerLoc;
                int zz, PlayerZPlane;

                playerLoc = PlayerLocation();

                if (playerLoc.z <= GROUND_LAYER)
                {
                    PlayerZPlane = (MAPSIZE_Z - 1) - playerLoc.z;
                }
                else
                {
                    PlayerZPlane = UNDERGROUND_LAYER;
                }

                zz = playerLoc.z - loc.z;

                result.x = loc.x - (playerLoc.x - 8) - zz;
                result.y = loc.y - (playerLoc.y - 6) - zz;
                result.z = PlayerZPlane + zz;

                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new TileLocation();
        }

        public static int TileObjectID(int tileNumber, int stack)
        {
            try
            {
                return Memory.ReadInt32(TileToAddress(tileNumber) + (stack * Addresses.Map.StepTileObject) + Addresses.Map.DistanceObjectId);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                return -1;
            }
        }

        public static int TileObjectData(int tileNumber, int stack)
        {
            try
            {
                return Memory.ReadInt32(TileToAddress(tileNumber) + (stack * Addresses.Map.StepTileObject) + Addresses.Map.DistanceObjectData);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                return -1;
            }
        }

        public static int TileObjectDataEx(int tileNumber, int stack)
        {
            try
            {
                return Memory.ReadInt32(TileToAddress(tileNumber) + (stack * Addresses.Map.StepTileObject) + Addresses.Map.DistanceObjectDataEx);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                return -1;
            }
        }

        #endregion

        #region Static methods

        public static IEnumerable<Tile> Tiles()
        {
            List<exTibia.Objects.Location> locations = new List<exTibia.Objects.Location>();
            exTibia.Objects.Location temp = new exTibia.Objects.Location();
            locations = exTibia.Objects.Location.Locations().ToList();

            List<Tile> result = new List<Tile>();
            Tile t = new Tile();
            Tile playerTile = GetPlayerTile();
            int address = Memory.ReadInt32(Addresses.Map.Pointer);
            List<Tile> list = new List<Tile>();

            for (int x = 0; x <= Addresses.Map.MaxTiles - 1; x++)
            {
                t = GetTile(x, address);

                if (t != null)
                {
                    if (t.ItemCount > 0)
                    {
                        t.MemoryLocation = Tile.tileToLocal(x);
                        t.GlobalLocation = Tile.tileToGlobal(t, playerTile);

                        temp.X = t.GlobalLocation.x;
                        temp.Y = t.GlobalLocation.y;
                        temp.Z = t.GlobalLocation.z;

                        yield return new Tile(x, t.GetItems(), t.ItemCount, t.GlobalLocation, t.MemoryLocation, t.GetOrder());
                    }
                }
            }
        }

        #endregion

        #region Help methods

        public static Tile GetTile(int index)
        {
            return GetTile(index, 0);
        }

        public static Tile GetTile(int index, int address)
        {
            Tile result = new Tile();

            try
            {                
                int addr, addr2;

                if (address == 0)
                {
                    addr = Tile.TileToAddress(index);
                }
                else
                {
                    addr = Tile.TileToAddress(index, address);
                }

                result.ItemCount = Memory.ReadInt32(addr + Addresses.Map.DistanceTileObjectCount);
                result._items = new TileItem[result.ItemCount];
                result._order = new int[result.ItemCount];

                addr2 = addr + (int)Addresses.Map.TileOrder;

                for (int z = 0; z <= result.ItemCount -1; z++)
                {
                    result.GetOrder()[z] = Memory.ReadInt32(addr2);
                    addr2 = addr2 + (int)Addresses.Map.TileOrder;
                }

                addr = addr + (int)Addresses.Map.DistanceTileObjects;

                for (int a = 0; a <= result.ItemCount - 1; a++)
                {
                    result.GetItems()[a].id = Memory.ReadInt32(addr + Addresses.Map.DistanceObjectId);
                    result.GetItems()[a].data = Memory.ReadInt32(addr + Addresses.Map.DistanceObjectData);
                    result.GetItems()[a].dataEx = Memory.ReadInt32(addr + Addresses.Map.DistanceObjectDataEx);
                    addr = addr + (int)Addresses.Map.StepTileObject;
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static Tile GetTile(TileLocation location)
        {
            try
            {
                TileLocation local;
                int num, minFloor, maxFloor;
                Tile PlayerTile;

                local = Tile.globalToLocal(location);
                num = Tile.ToTileNumber(local);
                PlayerTile = GetPlayerTile();

                minFloor = 0;
                maxFloor = 0;

                for (int i = 0; i <= 7; i++)
                {
                    if (PlayerTile.Index >= Addresses.Map.MaxTiles * i && PlayerTile.Index <= Addresses.Map.MaxTiles * (i + 1))
                    {
                        minFloor = (int)Addresses.Map.MaxTiles * i;
                        maxFloor = (int)Addresses.Map.MaxTiles * (i + 1) - 1;
                        break;
                    }
                }

                if (num > maxFloor)
                {
                    num = num - maxFloor + minFloor - 1;
                }

                if (num < minFloor)
                {
                    num = maxFloor - minFloor + num + 1;
                }

                return GetTile(num);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static Tile GetPlayerTile()
        {
            return GetPlayerTile(0);
        }

        public static Tile GetPlayerTileExtended()
        {
            try
            {
                int PlayerId = Player.ID;
                int addr = Memory.ReadInt32(Addresses.Map.TilePointer);
                Tile t = new Tile();
                Tile result = new Tile();
                for (int i = 0; i < Addresses.Map.MaxZ; i++)
                {
                    int tileNumber = Memory.ReadInt32(addr + (14 * 18 * i * 4) + 116 * 4);

                    t = GetTile(tileNumber);

                    if (t.ItemCount > 0)
                    {
                        for (int y = 0; y <= t.ItemCount - 1; y++)
                        {
                            if (t.GetItems()[y].id == 0x63 && t.GetItems()[y].data == PlayerId)
                            {
                                result = t;
                                result.Index = tileNumber;
                                result.GlobalLocation = Tile.PlayerLocation();
                                result.MemoryLocation = Tile.tileToLocal(tileNumber);
                                return result;
                            }

                        }
                    }

                }
                return null;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }

            return null;
        }

        public static Tile GetPlayerTile(int address)
        {
            try
            {
                Tile result = new Tile();
                int playerId;
                Tile t = new Tile();
                int found = 0;
                if (address == 0)
                {
                    address = Memory.ReadInt32(Addresses.Map.Pointer);
                }

                playerId = (int)Player.ID;

                for (int x = 0; x <= Addresses.Map.MaxTiles - 1; x++)
                {
                    t = GetTile(x, address);

                    if (t.ItemCount > 0)
                    {
                        for (int y = 0; y <= t.ItemCount - 1; y++)
                        {
                            if (t.GetItems()[y].id == 0x63 && t.GetItems()[y].data == playerId)
                            {
                                result = t;
                                result.Index = x;
                                result.GlobalLocation = Tile.PlayerLocation();
                                result.MemoryLocation = Tile.tileToLocal(x);
                                found++;
                            }

                        }
                    }
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static int PlayerTileIndex()
        {
            try
            {
                int address = Memory.ReadInt32(Addresses.Map.Pointer);

                int tileAddress;
                int itemCount;

                for (int x = 0; x <= Addresses.Map.MaxTiles - 1; x++)
                {
                    tileAddress = Tile.TileToAddress(x);
                    itemCount = Memory.ReadInt32(tileAddress + Addresses.Map.DistanceTileObjectCount);

                    tileAddress = tileAddress + (int)Addresses.Map.DistanceTileObjects;

                    if (itemCount > 0)
                    {
                        for (int i = 0; i <= itemCount - 1; i++)
                        {
                            if (Memory.ReadInt32(tileAddress + Addresses.Map.DistanceObjectId + i * Addresses.Map.StepTileObject) == 0x63)
                                if (Memory.ReadInt32(tileAddress + Addresses.Map.DistanceObjectData + i * Addresses.Map.StepTileObject) == Player.ID)
                                    return x;

                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return -1;
        }

        public static Collection<Tile> TilesFromScreen()
        {
            try
            {
                int playerTile = PlayerTileIndex();
                Collection<Tile> result = new Collection<Tile>();
                Tile tile = new Tile();

                Collection<Objects.Location> locations = new Collection<Objects.Location>();
                locations = Objects.Location.Locations();

                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X - 256, Player.Y - 256, Player.Z));

                foreach (Objects.Location location in locations)
                {
                    tile = Tile.GetTile(Tile.GetTileNumByPosition(location.X, location.Y, location.Z, playerTile));
                    tile.LocationOnMap = location;
                    tile.PathfinderPoint = new Point(location.X - maploc.X,location.Y- maploc.Y);
                    if (tile.ItemCount > 0)
                        result.Add(tile);
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static Collection<Tile> TilesFromScreenExtended()
        {
            try
            {
                int playerTile = GetPlayerTileExtended().Index;
                Collection<Tile> result = new Collection<Tile>();
                Tile tile = new Tile();

                Collection<Objects.Location> locations = new Collection<Objects.Location>();
                locations = Objects.Location.Locations();

                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X - 256, Player.Y - 256, Player.Z));

                foreach (Objects.Location location in locations)
                {
                    tile = Tile.GetTile(Tile.GetTileNumByPosition(location.X, location.Y, location.Z, playerTile));
                    tile.LocationOnMap = location;
                    tile.PathfinderPoint = new Point(location.X - maploc.X, location.Y - maploc.Y);
                    if (tile.ItemCount > 0)
                        result.Add(tile);
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static Tile GetDistTile(exTibia.Objects.Location location)
        {
            return GetDistTile(location.X, location.Y, location.Z);
        }

        public static Tile GetDistTile(int X, int Y, int Z)
        {
            try
            {
                Collection<Tile> tiles = new Collection<Tile>();
                tiles = TilesFromScreen();
                return tiles.FirstOrDefault(t => t.LocationOnMap == new exTibia.Objects.Location(X, Y, Z));
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static bool CompareTiles(Tile first, Tile second)
        {
            try
            {
                if (first == null || second == null)
                    return false;

                if (first.ItemCount == second.ItemCount)
                {
                    return HelpMethods.ArraysEqual<TileItem>(first.GetItems(), second.GetItems());
                }
                else
                    return false;


            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        public static int GetTileNumByPosition(int px, int py, int pz, int PlayerTile)
        {
            int sx, sy, sz;
            int nx, ny, nz;

            int xdiff, ydiff;

            int PlayerTileNum = PlayerTile;

            if (pz != Player.Z)
            {
                return -1;
            }

            if (PlayerTileNum == -1)
            {
                return -1;
            }

            xdiff = px - Player.X;
            ydiff = py - Player.Y;

            sz = PlayerTileNum / (14 * 18);
            sy = (PlayerTileNum - (sz * 14 * 18)) / 18;
            sx = PlayerTileNum - (sz * 14 * 18) - (sy * 18);

            nx = sx + xdiff;
            ny = sy + ydiff;
            nz = sz;

            if (nx > 17) { nx -= 18; }
            if (nx < 0) { nx += 18; }

            if (ny > 13) { ny -= 14; }
            if (ny < 0) { ny += 14; }

            return (nx + ny * 18 + nz * 14 * 18);
        }

        public static bool CheckTileHasItem(exTibia.Objects.Location location, Collection<int> items)
        {
            try
            {
                Tile t = new Tile();
                t = GetDistTile(location);

                if (t.GetItems().Any(i => items.Any(it => i.id == it)))
                    return true;
                return false;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;       
        }

        #endregion
    }
}
