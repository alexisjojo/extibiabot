using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class CaveBot : INotifyPropertyChanged
    {
        #region Singleton

        static readonly CaveBot _instance = new CaveBot();

        public static CaveBot Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Delegates & Events

        public event EventHandler<CaveBotEventArgs> WaypointProcessed;
        public event EventHandler<CaveBotEventArgs> NextWaypoint;
        public event PropertyChangedEventHandler PropertyChanged;

        void CaveBot_NextWaypoint(object sender, CaveBotEventArgs e)
        {
            e.Waypoint.AttemptToRun = 0;
            CurrentID++;            
        }

        void CaveBot_WaypointProcessed(object sender, CaveBotEventArgs e)
        {
            e.Waypoint.Active = false;         
            CanProcessNextWpt = true;
        }

        protected virtual void OnWaypointProcessed(CaveBotEventArgs e)
        {
            if (WaypointProcessed != null)
            {
                WaypointProcessed(this, e);
            }
        }

        protected virtual void OnNextWaypoint(CaveBotEventArgs e)
        {
            if (NextWaypoint != null)
            {
                NextWaypoint(this, e);
            }
        }

        #endregion

        #region Fields

        private ObservableCollection<Waypoint> _waypointList = new ObservableCollection<Waypoint>();
        private ObservableCollection<SpecialArea> _specialAreaList = new ObservableCollection<SpecialArea>();
        private Thread _caveBotThread;
        private bool _state = false;
        private Waypoint _currentWaypoint;
        private int _currentID = 0;

        private WalkingMethod _walkingMethod = new WalkingMethod();

        private int _skipNodes = 3;
        private int _maxAttempts = 3;

        private bool _walkByPlayers = false;
        private bool _walkByFields = false;

        private bool _drawMap = true;
        private bool _drawWay = false;
        private bool _drawItems = true;

        private int _drawMapInterval = 500;
        private int _drawWayInterval = 250;
        private int _drawItemsInterval = 750;

        private bool _requestedPause = false;
        private Tile _tile = new Tile();
        private bool _CanProcessNextWpt = true;
        private CbDrawMap _drawMapWindow = new CbDrawMap();

        private ObservableCollection<WalkItemID> _walkableIds = new ObservableCollection<WalkItemID>() { new WalkItemID(12), new WalkItemID(13) };
        private ObservableCollection<WalkItemID> _nonWalkableIds = new ObservableCollection<WalkItemID>() { new WalkItemID(62), new WalkItemID(63) };


        #endregion

        #region Properties

        public ObservableCollection<Waypoint> WaypointList
        {
            get {  return _waypointList; }
        }

        public ObservableCollection<SpecialArea> SpecialAreasList
        {
            get { return _specialAreaList; }
        }

        public ObservableCollection<WalkItemID> WalkableIds
        {
            get { return _walkableIds; }
        }

        public ObservableCollection<WalkItemID> NonWalkableIds
        {
            get { return _nonWalkableIds; }
        }

        public Thread CaveBotThread
        {
            get { return _caveBotThread; }
            set { _caveBotThread = value; }
        }

        public WalkingMethod WalkingMethod
        {
            get { return _walkingMethod; }
            set { _walkingMethod = value; }

        }

        public bool State
        {
            get { return _state; }
            set { _state = value; }
        }

        public bool RequestedPause
        {
            get { return _requestedPause; }
            set { _requestedPause = value; }
        }
   
        public Waypoint CurrentWaypoint
        {
            get { return _currentWaypoint; }
            set { _currentWaypoint = value; }
        }

        public int CurrentID
        {
            get { return _currentID; }
            set { _currentID = value; }
        }

        public int MaxAttempts
        {
            get { return _maxAttempts; }
            set { _maxAttempts = value; }
        }

        public int SkipNodes
        {
            get { return _skipNodes; }
            set { _skipNodes = value; }
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

        public CbDrawMap DrawMapWindow
        {
            get { return _drawMapWindow; }
            set { _drawMapWindow = value; }
        }

        public bool DrawMap
        {
            get { return _drawMap; }
            set 
            { 
                _drawMap = value;
                OnPropertyChanged("DrawMap");
            }
        }

        public bool DrawWay
        {
            get { return _drawWay; }
            set 
            { 
                _drawWay = value;
                OnPropertyChanged("DrawWay");
            }
        }

        public bool DrawItems
        {
            get { return _drawItems; }
            set 
            { 
                _drawItems = value;
                OnPropertyChanged("DrawWay");
            }
        }

        public int DrawWayInterval
        {
            get { return _drawWayInterval; }
            set { _drawWayInterval = value; }
        }

        public int DrawMapInterval
        {
            get { return _drawMapInterval; }
            set { _drawMapInterval = value; }
        }

        public int DrawItemsInterval
        {
            get { return _drawItemsInterval; }
            set { _drawItemsInterval = value; }
        }

        public Tile Tile
        {
            get { return _tile; }
            set { _tile = value; }
        }

        public bool CanProcessNextWpt
        {
            get { return _CanProcessNextWpt; }
            set { _CanProcessNextWpt = value; }
        }

        #endregion

        #region Constructor

        public CaveBot()
        {
            WalkingMethod = exTibia.Helpers.WalkingMethod.ArrowKeys;

            WaypointProcessed += new EventHandler<CaveBotEventArgs>(CaveBot_WaypointProcessed);
            NextWaypoint += new EventHandler<CaveBotEventArgs>(CaveBot_NextWaypoint);

            CaveBotThread = new Thread(RunCaveBot);
            CaveBotThread.Start();
        }

        #endregion

        #region Main methods

        void RunCaveBot()
        {            
            while (true)
            {
                while (State)
                {
                    Thread.Sleep(250);

                    if (WaypointList.Count() > 0)
                    {
                        if (!Queues.HasTask(Task.CaveBotProcessWaypoint))
                            CanProcessNextWpt = true;

                        if (!CanProcessNextWpt)
                            continue;

                        if (CurrentID >= WaypointList.Count())
                            CurrentID = 0;

                        CurrentWaypoint = (CurrentID <= WaypointList.Count()) ? WaypointList[CurrentID] : WaypointList[0];

                        CanProcessNextWpt = false;

                        Queues.Add(Task.CaveBotProcessWaypoint, new Action(() =>
                        {
                            WaypointList[WaypointList.IndexOf(CurrentWaypoint)].Active = true;
                            ProcessWaypoint(CurrentWaypoint);
                        }), 80, 10, false, false);
                    }
                }
                Thread.Sleep(250);
            }
        }

        public void Init()
        {
            Helpers.Debug.WriteLine("Instance of CaveBot() has been created.", ConsoleColor.Yellow);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region HelpMehods

        private void ProcessWaypoint(Waypoint waypoint)
        {
            #region Increasing an amount of attempts to process waypoint

            waypoint.AttemptToRun++;

            if (!(waypoint.AttemptToRun < CaveBot.Instance.MaxAttempts))
                OnNextWaypoint(new CaveBotEventArgs(waypoint));

            #endregion

            try
            {
                switch (waypoint.WaypointType)
                {
                    case WaypointType.Action:
                        DoAction(waypoint);
                        break;
                    case WaypointType.Ladder:
                        DoLadder(waypoint);
                        break;
                    case WaypointType.Node:
                        DoNode(waypoint);
                        break;
                    case WaypointType.Rope:
                        DoRope(waypoint);
                        break;
                    case WaypointType.Shovel:
                        DoShovel(waypoint);
                        break;
                    case WaypointType.Stand:
                        DoStand(waypoint);
                        break;
                    case WaypointType.Use:
                        DoUse(waypoint);
                        break;
                    case WaypointType.Walk:
                        DoWalk(waypoint);
                        break;
                }
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }  

            WaypointProcessed(null, new CaveBotEventArgs(waypoint));
        }

        public static void Stop()
        {
            Walker.Instance.PauseWalking();
        }

        #endregion

        #region Tools for walking

        private static List<String> ListRopes = new List<string>() { "Rope", "Elvenhair Rope", "Sneaky Stabber Of Eliteness", "Squeezing Gear Of Girlpower", "Whacking Driller Of Fate" };
        private static List<String> ListShovel = new List<string>() { "Shovel", "Light Shovel", "Sneaky Stabber Of Eliteness", "Squeezing Gear Of Girlpower", "Whacking Driller Of Fate" };

        #endregion

        #region Processing Waypoint

        private void DoStand(Waypoint waypoint)
        {
            Walker.Instance.WalkByFields = WalkByFields;
            Walker.Instance.WalkByPlayers = WalkByPlayers;
            Walker.Instance.Walksender = WalkSender.Walking;
            Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;
            Walker.Instance.SkipNode = false;

            Walker.Instance.SpecialAreas.Clear();

            foreach (SpecialArea area in SpecialAreasList)
                Walker.Instance.SpecialAreas.Add(area);

            Walker.Instance.Start = Player.Location;
            Walker.Instance.End = waypoint.Location;

            Walker.Instance.Walk();

            Thread.Sleep(new Random().Next(250,500));

            if (Player.Location.Equals(waypoint.Location))
                NextWaypoint(null, new CaveBotEventArgs(waypoint));
            else
                if (Player.Location.Z != waypoint.Location.Z)
                    NextWaypoint(null, new CaveBotEventArgs(waypoint));
        }

        private void DoWalk(Waypoint waypoint)
        {
            Walker.Instance.Walksender = WalkSender.Walking;
            Walker.Instance.SkipNode = false;
            Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

            Walker.Instance.SpecialAreas.Clear();

            foreach (SpecialArea area in SpecialAreasList)
                Walker.Instance.SpecialAreas.Add(area);

            Walker.Instance.Start = Player.Location;
            Walker.Instance.End = waypoint.Location;

            Walker.Instance.Walk();

            if (Player.Location.DistanceTo(waypoint.Location) <= 2)
                NextWaypoint(null, new CaveBotEventArgs(waypoint));
        }

        private void DoNode(Waypoint waypoint)
        {
            Walker.Instance.Walksender = WalkSender.Walking;
            Walker.Instance.SkipNode = true;
            Walker.Instance.SkipNodeAmount = SkipNodes;
            Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

            Walker.Instance.SpecialAreas.Clear();

            foreach (SpecialArea area in SpecialAreasList)
                Walker.Instance.SpecialAreas.Add(area);

            Walker.Instance.Start = Player.Location;
            Walker.Instance.End = waypoint.Location;

            Walker.Instance.Walk();

            if (Player.Location.DistanceTo(waypoint.Location) <= CaveBot.Instance.SkipNodes)
                NextWaypoint(null, new CaveBotEventArgs(waypoint));
        }

        private void DoAction(Waypoint waypoint)
        {
            NextWaypoint(null, new CaveBotEventArgs(waypoint));
        }

        private void DoLadder(Waypoint waypoint)
        {
            NextWaypoint(null, new CaveBotEventArgs(waypoint));
        }

        private void DoUse(Waypoint waypoint)
        {
            try
            {
                if (!Player.Location.IsAdjacentTo(waypoint.Location)) DoStand(waypoint);
                InputControl.Instance.RightClickLoc(waypoint.Location);
                NextWaypoint(null, new CaveBotEventArgs(waypoint));
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }  
        }

        private void DoRope(Waypoint waypoint)
        {
            try
            {
                if (waypoint == null)
                    throw new ArgumentNullException("waypoint");

                if (!Player.Location.IsAdjacentTo(waypoint.Location)) DoStand(waypoint);

                Collection<Objects.Container> containers = new Collection<Objects.Container>();
                containers = Inventory.Instance.GetContainers();

                int idfound = -1;
                Objects.Container cfound = null;
                string found = "";

                foreach (string s in ListRopes)
                {
                    idfound = HotkeysGame.FindHotkey(s);
                    if (idfound != 0)
                    {
                        found = s;
                        break;
                    }
                }

                if (idfound == -1)
                {
                    foreach (string s in ListRopes)
                    {
                        foreach (Objects.Container c in containers)
                        {
                            if (c.GetItems().Any(i => i.ItemName == s))
                            {
                                found = s;
                                cfound = c;
                                break;
                            }
                        }
                        if (cfound != null) break;
                    }
                }

                if (idfound == -1 && cfound == null) return;

                if (idfound != -1)
                {
                    InputControl.UseHot(idfound);
                    Walker.Instance.PauseWalking();
                    Thread.Sleep(Utils.RandomNumber(250, 500));
                    InputControl.Instance.LeftClickLoc(waypoint.Location);
                }
                else
                {
                    cfound.GetItems().First(i => i.ItemId == Items.FindByName(found).ItemID).UseOnMap(waypoint.Location);
                }

                Thread.Sleep(Utils.RandomNumber(500, 750));

                NextWaypoint(null, new CaveBotEventArgs(waypoint));
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }           
        }

        private void DoShovel(Waypoint waypoint)
        {
            try
            {
                if (waypoint == null)
                    throw new ArgumentNullException("waypoint");

                if (!Player.Location.IsAdjacentTo(waypoint.Location)) DoStand(waypoint);
                Helpers.Debug.WriteLine("executing shovel waypoint...", ConsoleColor.Red);
                Collection<Objects.Container> containers = new Collection<Objects.Container>();
                containers = Inventory.Instance.GetContainers();

                int idfound = -1;
                Objects.Container cfound = null;
                string found = "";

                foreach (string s in ListShovel)
                {
                    idfound = HotkeysGame.FindHotkey(s);
                    if (idfound != 0)
                    {
                        found = s;
                        break;
                    }
                }

                if (idfound == -1)
                {
                    foreach (string s in ListShovel)
                    {
                        foreach (Objects.Container c in containers)
                        {
                            if (c.GetItems().Any(i => i.ItemName == s))
                            {
                                found = s;
                                cfound = c;
                                break;
                            }
                        }
                        if (cfound != null) break;
                    }
                }

                if (idfound == -1 && cfound == null) return;

                if (idfound != -1)
                {
                    InputControl.UseHot(idfound);
                    Thread.Sleep(Utils.RandomNumber(250, 500));
                    InputControl.Instance.LeftClickLoc(waypoint.Location);
                }
                else
                {
                    cfound.GetItems().First(i => i.ItemId == Items.FindByName(found).ItemID).UseOnMap(waypoint.Location);
                }

                Thread.Sleep(Utils.RandomNumber(500, 750));

                NextWaypoint(null, new CaveBotEventArgs(waypoint));
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }
}
