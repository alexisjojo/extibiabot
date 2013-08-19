using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Walker
    {
        #region Singleton

        static readonly Walker _instance = new Walker();

        public static Walker Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Consctructor

        public Walker()
        {

        }

        public Walker(Location start, Location end)
        {
            _start = start;
            _end = end;

        }

        public Walker(Location start, Location end, WalkSender sender)
        {
            _start = start;
            _end = end;
            _walkSender = sender;
        }

        public Walker(Location start, Location end, ObservableCollection<SpecialArea> specialAreas)
        {
            _start = start;
            _end = end;
            _specialAreas = specialAreas;
        }

        public Walker(Location start, Location end, ObservableCollection<SpecialArea> specialAreas, WalkSender sender)
        {
            _start = start;
            _end = end;
            _walkSender = sender;
            _specialAreas = specialAreas;

        }

        #endregion

        #region Variables

        private PathFinder _pathfinder;
        private WalkSender _walkSender;
        private Collection<Location> _wayPathFinderNodes = new Collection<Location>();
        private Collection<Location> _wayLocation = new Collection<Location>();
        private Collection<Location> _elapesedwayPathFinderNodes = new Collection<Location>();
        private Stopwatch _pathTimer = new Stopwatch();
        private int _wayLimit = 250;
        private Map _map = new Map();
        private Location _start;
        private Location _end;
        private bool _diagonals = true;
        private bool _heavydiagonals = true;
        private bool _break;
        private bool _iswalking;
        private int _pause_time = -1;
        private bool _skipnodebool = false;
        private int _skipnodeamount = 3;
        private ObservableCollection<SpecialArea> _specialAreas = new ObservableCollection<SpecialArea>();
        private Collection<int> _idstoAvoid = new Collection<int>();
        private WalkingMethod _walkingMethod = new WalkingMethod();
        private bool _walkByPlayers = false;
        private bool _walkByFields = false;
        private bool _drawMap = false;
        private bool _drawWay = false;
        private byte[,] _walkerMap = new byte[1024,1024];
        private byte[] _memMapa = new byte[65536];

        #endregion

        #region Properties

        public byte[,] WalkerMap()
        {
            return _walkerMap;
        }

        public byte[] MemMapa()
        {
            return _memMapa;
        }

        public PathFinder Pathfinder
        {
            get { return this._pathfinder; }
            set { this._pathfinder = value; }
        }

        public WalkingMethod WalkingMethod
        {
            get { return this._walkingMethod; }
            set { this._walkingMethod = value; }
        }

        public WalkSender Walksender
        {
            get { return this._walkSender; }
            set { this._walkSender = value; }
        }

        public Collection<Location> WayPathFinderNodes
        {
            get { return this._wayPathFinderNodes; }
            private set { this._wayPathFinderNodes = value; }
        }

        public Collection<Location> ElapesedwayPathFinderNodes
        {
            get { return this._elapesedwayPathFinderNodes; }
            private set { this._elapesedwayPathFinderNodes = value; }
        }

        public Collection<Location> WayLocation
        {
            get { return this._wayLocation; }
            private set { this._wayLocation = value; }
        }

        public Map Map
        {
            get { return this._map; }
            set { this._map = value; }
        }

        public Location Start
        {
            get { return this._start; }
            set { this._start = value; }
        }

        public Location End
        {
            get { return this._end; }
            set { this._end = value; }
        }

        public bool Pause
        {
            get { return this._break; }
            set { this._break = value; }
        }

        public int WaySteps
        {
            get
            {
                return WayPathFinderNodes.Count;
            }
        }

        public int WayLimit
        {
            get { return this._wayLimit; }
            set { this._wayLimit = value; }
        }

        public bool IsWalking
        {
            get { return this._iswalking; }
            set { this._iswalking = value; }
        }

        public int PauseTime
        {
            get { return this._pause_time; }
            set { this._pause_time = value; }
        }

        public bool SkipNode
        {
            get { return this._skipnodebool; }
            set { this._skipnodebool = value; }
        }

        public int SkipNodeAmount
        {
            get { return this._skipnodeamount; }
            set { this._skipnodeamount = value; }
        }

        public bool Diagonals
        {
            get { return this._diagonals; }
            set { this._diagonals = value; }
        }

        public bool Heavydiagonals
        {
            get { return this._heavydiagonals; }
            set { this._heavydiagonals = value; }
        }

        public ObservableCollection<SpecialArea> SpecialAreas
        {
            get { return this._specialAreas; }
        }

        public Collection<int> IDsToAvoid
        {
            get { return this._idstoAvoid; }
            private set { this._idstoAvoid = value; }
        }

        public bool WalkByPlayers
        {
            get { return _walkByPlayers; }
            set { _walkByPlayers = value; }
        }

        public bool WalkByFields
        {
            get { return _walkByFields; }
            set { _walkByFields = value; }
        }

        public bool DrawMap
        {
            get { return _drawMap; }
            set { _drawMap = value; }
        }

        public bool DrawWay
        {
            get { return _drawWay; }
            set { _drawWay = value; }
        }

        public Stopwatch PathTimer
        {
            get { return _pathTimer; }
            set { _pathTimer = value; }
        }

        #endregion

        #region Methods

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool Walk()
        {
            try
            {
                PathTimer.Restart();
                bool loadedAllMaps = false;
                byte[,] mapSmall = new byte[256, 256];
                mapSmall = Map.GetMap(false);
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 256; j++)
                        WalkerMap()[i + 256, j + 256] = mapSmall[i, j];

                foreach (SpecialArea area in SpecialAreas.Where(a => a.Active && a.Consideration == this.Walksender))
                    foreach (Location l in area.Locations)
                    {
                        Point p = LocToPoint(l);
                        WalkerMap()[p.X, p.Y] = 0;
                    }

                WalkerMap()[LocToPoint(Start).X,LocToPoint(Start).Y] = 4;
                WalkerMap()[LocToPoint(End).X,LocToPoint(End).Y] = 4;

            calculatePath:                
                Pathfinder = new PathFinder(WalkerMap());
                Pathfinder.Formula = HeuristicFormula.Custom1;
                Pathfinder.HeuristicEstimate = 12;
                Pathfinder.Diagonals = Diagonals;
                Pathfinder.HeavyDiagonals = Heavydiagonals;
                WayPathFinderNodes = Pathfinder.FindPath(Start, End);

                PathTimer.Stop();

                if (WayPathFinderNodes == null)
                {
                    if (!loadedAllMaps)
                    {
                        Array.Copy(Map.GetMap(true), 0, WalkerMap(), 0, 1024 * 1024);
                        loadedAllMaps = true;
                        goto calculatePath;
                    }

                    Helpers.Debug.WriteLine(String.Format("Could not find path to location: {0}.", End.ToString()), ConsoleColor.Red);
                    return false;
                }

                WayPathFinderNodes.RemoveAt(0);

                if (WayLimit < WaySteps) return false;

                Helpers.Debug.WriteLine(String.Format("Path (Steps {0}) to waypoint {1} calculated in {2:0.00} ms.", WayPathFinderNodes.Count(), End.ToString(), PathTimer.ElapsedMilliseconds), ConsoleColor.Green);

                #region Walking (depends from method)

                foreach (Location location in WayPathFinderNodes)
                {
                    #region Breaking a walking

                    if (Worker.Instance.ExitSynchrous) { Worker.Instance.ExitSynchrous = false; return false; }

                    #endregion

                    #region Pausing a walking

                    if (PauseTime != -1)
                    {
                        IsWalking = false;
                        Thread.Sleep(PauseTime);
                        PauseTime = -1;
                    }

                    if (Pause) { Pause = false; return false; };
                    if (SkipNode) { if (location.DistanceTo(End) < SkipNodeAmount) return true; }

                    #endregion

                    #region Handling targeting way

                    if (Walksender == WalkSender.Targeting)
                    {
                        if (Targeter.Instance.Rule.DesiredStance == DesiredStance.MeleeApproach)
                        {
                            if (WayPathFinderNodes.Count > 1)
                            {
                                if (Player.Location.DistanceTo(Targeter.Instance.Target.Location) <= 2)
                                    return true;
                            }
                        }
                    }

                    #endregion

                    #region Making steps

                    IsWalking = true;

                    Location oldLocation = Player.Location;

                    if (Player.Location == location)
                        break;

                    int elapsed = 0;

                    while (oldLocation == Player.Location && location.DistanceTo(Player.Location) < 2)
                    {
                        switch (WalkingMethod)
                        {
                            case exTibia.Helpers.WalkingMethod.ArrowKeys:
                                #region Arrow keys
                            arrowkeys:
                                InputControl.GoLocation(location);
                                #endregion
                                break;
                            case exTibia.Helpers.WalkingMethod.MapClicks:
                                #region Map Clicks
                            mapclicks:
                                InputControl.Instance.LeftClickLoc(location);
                                #endregion
                                break;
                            case exTibia.Helpers.WalkingMethod.Mixed:
                                #region Mixed
                                if (new Random().Next(1, 2) == 1)
                                    goto arrowkeys;
                                else
                                    goto mapclicks;
                                #endregion
                        }

                        Thread.Sleep(25);

                        elapsed += 25;
                        if (elapsed > 200)
                            return false;

                        if (WayPathFinderNodes.Count == 1)
                        {
                            Thread.Sleep(new Random().Next(300, 600));
                            break;
                        }
                        
                    }
                    IsWalking = false;

                    #endregion
                }

                #endregion

                return Start == End;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        public void PauseWalking()
        {
            Pause = true;
        }

        public void PauseFor(int i)
        {
            PauseTime = i;
        }

        #endregion

        #region HelpMethods

        public static Location PointToLoc(int x, int y)
        {
            try
            {
                Location l = new Location();
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X - 256, Player.Y - 256, Player.Z));

                l.X = maploc.X + x;
                l.Y = maploc.Y + y;
                l.Z = (int)Player.Z;

                return l;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new Location();
        }

        public static Point LocToPoint(Location l)
        {
            try
            {
                Point p = new Point();
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X - 256, Player.Y - 256, Player.Z));

                p.X = l.X - maploc.X;
                p.Y = l.Y - maploc.Y;

                return p;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new Point();
        }

        public bool LocIsWalkable(Location location)
        {
            try
            {

                Array.Clear(MemMapa(), 0, MemMapa().Length);

                Array.Copy(Map.MapFromMem(), 65536, MemMapa(), 0, 65536);

                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                int dimensionX = location.X - maploc.X;
                int dimensionY = location.Y - maploc.Y;

                byte[][] mapTransformed = new byte[256][];
                for (int i = 0; i < 256; i++)
                    mapTransformed[i] = new byte[256];


                int counter = 0;

                for (int x = 0; x < 256; x++)
                    for (int y = 0; y < 256; y++)
                    {
                        mapTransformed[x][y] = MemMapa()[counter];
                        counter++;
                    }

                if (dimensionX < 0 || dimensionX > 255)
                    return false;
                if (dimensionY < 0 || dimensionY > 255)
                    return false;

                if (mapTransformed[dimensionX][dimensionY] != 0xFF)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        public bool LocIsWalkable(byte[] map, Location maploc, Location location, bool senderTargeting = false)
        {
            try
            {
                int dimensionX = location.X - maploc.X;
                int dimensionY = location.Y - maploc.Y;

                byte[][] mapTransformed = new byte[256][];
                for (int i = 0; i < 256; i++)
                    mapTransformed[i] = new byte[256];

                int counter = 0;

                for (int x = 0; x < 256; x++)
                    for (int y = 0; y < 256; y++)
                    {
                        mapTransformed[x][y] = map[counter];
                        counter++;
                    }

                if (dimensionX < 0 || dimensionX > 255)
                    return false;
                if (dimensionY < 0 || dimensionY > 255)
                    return false;

                if (senderTargeting)
                {
                    foreach (SpecialArea area in SpecialAreas.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        foreach (Location l in area.Locations)
                        {
                            Point p = LocToPoint(l);
                            mapTransformed[dimensionX][dimensionY] = 0xFF;
                        }
                }

                if (mapTransformed[dimensionX][dimensionY] != 0xFF && mapTransformed[dimensionX][dimensionY] != 0xFA)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        #endregion
    }

        #region Struct

    public struct WalkerSetting
    {
        public WalkerSetting(bool a, bool b, bool c, bool d)
        {
            avoidFire = a;
            avoidEnergy = b;
            avoidPoison = c;
            WalkByPlayers = d;
        }

        bool avoidFire;
        bool avoidEnergy;
        bool avoidPoison;
        bool WalkByPlayers;
    }

    #endregion
}
