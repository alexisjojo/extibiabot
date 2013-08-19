using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Map
    {
        #region Constructor

        public Map()
        {

        }

        #endregion

        #region Variables

        private byte[,] _maptopath = new byte[1024,1024];

        #endregion

        #region Properties

        public byte[,] GetMapPathfinder()
        {
            return (byte[,])_maptopath;
        } 

        #endregion

        #region Methods

        public byte[,] GetMap(bool all)
        {
            Collection<Tile> tiles = new Collection<Tile>();
            return GetMap(all, out tiles);
        }

        public byte[,] GetMap(bool all, out Collection<Tile> tiles)
        {
            tiles = new Collection<Tile>();
            try
            {
                if (all)
                {
                    UpdateMap(false, 1, out tiles);
                    return GetMapPathfinder();
                }
                else
                {
                    UpdateMap(true, 1, out tiles);
                    return GetMapPathfinder();
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        private void UpdateMap(bool center, int mode, out Collection<Tile> tilesFromScreen)
        {
            tilesFromScreen = new Collection<Tile>();

            try
            {
                #region Vars and reading map

                List<string> paths = new List<string>();
                List<byte[]> arrays = new List<byte[]>();
                string playermap = MapFileName(Player.X, Player.Y, Player.Z);

                if (center)
                    paths.Add(playermap);
                else
                {
                    for (int pi = 0; pi < 3; pi++)
                        for (int pj = 0; pj < 3; pj++)
                            paths.Add(MapFileName((Player.X - 256 + 256 * pj), (Player.Y - 256 + 256 * pi), Player.Z));
                }

                paths.ForEach(s =>
                {
                    if (File.Exists(s))
                    {
                        FileInfo file = new FileInfo(s);
                        byte[] tab = new byte[file.Length];

                        if (s == MapFileName(Player.X, Player.Y, Player.Z))
                        {
                            tab = MapFromMem();
                        }
                        else 
                        {
                            tab = File.ReadAllBytes(s); 
                        }
                        arrays.Add(tab);
                    }
                });

                #endregion

                #region Processing map files

                int i = 0;

                List<Int32> blocked_mode1 = new List<int>() { 0xFF, 0xFA };
                List<Int32> blocked_mode2 = new List<int>() { 0x72, 0x28, 0xB3, 0xC0, 0x1e, 0x56, 0xBA, 0xD2, 0x0 };
                
                Location l = (center) ? MapToLocation(paths[0]) : MapToLocation(paths[i]);

                for (int a = 0; a < 3; a++)
                    for (int b = 0; b < 3; b++)
                    {
                        if (center)
                            if (a != 1) continue;
                        if (center)
                            if (b != 1) continue;

                        if (i > arrays.Count-1)
                            i = arrays.Count - 1;

                        byte[] array = arrays[i];
                        int index = 65536;

                        for (int x = 0; x < 256; x++)
                        {
                            for (int y = 0; y < 256; y++)
                            {
                                int ax = (center) ? x : x + b * 256;
                                int ay = (center) ? y : y + a * 256;

                                switch (mode)
                                {
                                    #region Mode 1
                                case 1:
                                    switch (array[index])
                                    {
                                        case 0xFF:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0xFA:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        default:
                                            GetMapPathfinder()[ax, ay] = 4;
                                            break;
                                    }
                                    break;
                                #endregion

                                    #region Mode 2
                                case 2:
                                    switch (array[index - 65536])
                                    {
                                        case 0x72:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0x28:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0xB3:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0xC0:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0x1E:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0x56:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0xBA:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0xD2:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        case 0x0:
                                            GetMapPathfinder()[ax, ay] = 0;
                                            break;
                                        default:
                                            GetMapPathfinder()[ax, ay] = 4;
                                            break;
                                    }
                                    break;
                                #endregion
                                }
                                index++;
                            }
                        }
                        i++;
                    }

                #endregion

                #region Processing screen tiles

                Collection<Tile> tiles = new Collection<Tile>(Tile.TilesFromScreenExtended());
                
                tilesFromScreen = tiles;

                foreach (Tile t in tiles)
                {
                    bool tileWalkable = false;

                    if (t.ItemCount > 1)
                    {
                        if (t.GetItems()[1].id == 99)
                        {
                            if (t.GetItems()[0].id == 10146) //depot spot with player, unwalkable
                                goto setWalkable;

                            if (t.GetItems()[1].data > 0x70000000) //summon, unwalkable
                                goto setWalkable;

                            else if (t.GetItems()[1].data < 0x40000000) //player, walkable/non depends from setting
                            {
                                if (CaveBot.Instance.WalkByPlayers)
                                    tileWalkable = true;
                                else
                                {
                                    tileWalkable = false;
                                    goto setWalkable;
                                }
                            }
                            else if (t.GetItems()[1].data > 0x40000000 || t.GetItems()[1].data > 0x70000000) //monster, unwalkable
                            {
                                tileWalkable = false;
                                goto setWalkable;
                            }
                        }
                    }

                    tileWalkable = !t.GetItems().Any(item =>
                    {
                        bool hasBlockingFlag = (DatItems.GetFlag(item.id, ItemFlags.Blocking) || DatItems.GetFlag(item.id, ItemFlags.BlocksPath));
                        return hasBlockingFlag;
                    });




                setWalkable:

                    if (center)
                    {
                        if (t.PathfinderPoint.X < 256 || t.PathfinderPoint.Y < 256)
                            continue;
                        if (t.PathfinderPoint.X > 1024 || t.PathfinderPoint.Y > 1024)
                            continue;
                    }

                    if (center)
                        GetMapPathfinder()[t.PathfinderPoint.X - 256, t.PathfinderPoint.Y - 256] = tileWalkable ? (byte)4 : (byte)0;
                    else
                        GetMapPathfinder()[t.PathfinderPoint.X, t.PathfinderPoint.Y] = tileWalkable ? (byte)4 : (byte)0;
                }

                #endregion
            }
            catch (ArgumentException ex)
            {
                Helpers.Debug.Report(ex);
            } 
            catch (FileNotFoundException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion

        #region Help Methods

        public static string MapFileName(int x, int y, int z)
        {
            try
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tibia\\Automap\\" +
                    (x / 256).ToString("000") +
                    (y / 256).ToString("000") +
                    z.ToString("00") + ".map";
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return "";
        }

        public static Location MapToLocation(string fileName)
        {
            try
            {
                Location l = new Location(0, 0, 0);
                fileName = fileName.Substring(fileName.Count() - 12, 8);
                if (fileName.Length == 12 || fileName.Length == 8)
                {
                    l.X = Int32.Parse(fileName.Substring(0, 3)) * 256;
                    l.Y = Int32.Parse(fileName.Substring(3, 3)) * 256;
                    l.Z = Int32.Parse(fileName.Substring(6, 2));
                }
                return l;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new Location();
        }

        public static byte[] MapFromMem()
        {
            try
            {
                string path = MapFileName(Player.X, Player.Y, Player.Z);
                Location loc = MapToLocation(path);
                int start = (int)Addresses.Map.Memory;
                int offset = (int)Addresses.Map.MemoryOffset;
                int good = 0;


                byte[] result = new byte[(uint)65536 * 2];

                for (int i = 0; i <= 10; i++)
                {
                    int memX = Memory.ReadByte(start);
                    int memY = Memory.ReadByte(start + 4);
                    int memZ = Memory.ReadByte(start + 8);

                    if (memX == (loc.X / 256) && memY == (loc.Y / 256) && memZ == loc.Z)
                    {
                        good = start;
                        good += 20;
                        break;
                    }
                    start += offset;
                }

                if (good != 0)
                {
                    result = Memory.ReadBytes(good, (uint)65536 * 2);
                }
                return result;
            }
            catch (InvalidOperationException e)
            {
                Helpers.Debug.Report(e);
            }
            return null;
        }

        #endregion
    }
}
