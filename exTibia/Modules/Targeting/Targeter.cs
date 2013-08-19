using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Targeter
    {
        #region Singleton

        static readonly Targeter _instance = new Targeter();

        public static Targeter Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private Creature _target = new Creature();
        private TargetSetting _rule = new TargetSetting();

        private bool _isattacking = false;
        private Thread _targeterThread;
        private bool _pause = false;
        private TargeterState _targeterState;

        public TargeterState TargetState
        {
            get { return _targeterState; }
            set { _targeterState = value; }
        }


        private Location _initialLocation = new Location();

        #endregion

        #region Properties

        public Location InitialLocation
        {
            get { return _initialLocation; }
            set { _initialLocation = value; }
        }

        public bool Pause
        {
            get { return _pause; }
            set { _pause = value; }
        }

        public TargetSetting Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        public Creature Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Thread TargeterThread
        {
            get { return _targeterThread; }
            set { _targeterThread = value; }
        }

        public bool IsAttacking
        {
            get { return _isattacking; }
            set { _isattacking = value; }
        }

        #endregion

        #region Constructor

        public Targeter()
        {
            TargeterThread = new Thread(Attack);
        }

        #endregion

        #region Methods for targeting

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Attack()
        {
            try
            {
                Targeting.Instance.RuleExecute();

                InitialLocation = Target.Location;

                while (Target.Active && Target.HPBar > Rule.HpRangeMin && Target.HPBar <= Rule.HpRangeMax && Target.IsVisible)
                {
                    Thread.Sleep(50);

                    if (!Targeting.Instance.State)
                        break;

                    if (!CheckAndMark())
                        break;
                    else
                        IsAttacking = true;

                    if (!Queues.HasTask(Task.TargeterStance))
                        Stance();

                    if (!Queues.HasTask(Task.TargeterCastSpell))
                        CastSpells();
                }

            }
            finally
            {
                IsAttacking = false;
                Targeting.Instance.RuleExecuted();
            }
        }

        #region Marking a monster

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool CheckAndMark()
        {
            //monster is already marked
            if (Player.RedSquare == Target.ID)
                return true;

            //monster is dead
            if (!Target.IsAlive())
                return false;

            //monster is not on screen
            if (!Target.Active)
                return false;

            //monster should be attacked
            return GuiBattle.Instance.MarkMonsterInBattle(Target);

        }

        #endregion

        #region Casting spells

        public void CastSpells()
        {
            try
            {
                foreach (SpellSetting spell in Rule.Spells.Where(s=> s != null))
                {
                    if (spell == null)
                        continue;

                    if (CanCast(spell.Spell) && Player.Location.DistanceTo(Target.Location) < 5)
                    {
                        if (spell.Synchronize)
                        {
                            int oldHP = Target.HPBar;
                            int interval = 0;

                            while (oldHP == Target.HPBar && interval < 1000)
                            {
                                Thread.Sleep(50);
                                interval += 50;
                            }
                        }

                        Queues.Add(Task.TargeterCastSpell, new Action(() =>
                        {
                            Cast(spell.Spell);
                        }), 95, 100, true, true);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        public bool CanCast(string spell)
        {
            try
            {
                TargetState = TargeterState.CheckingCastingSpell;
                Spell spellObject = Spells.Instance.FindSpell(spell);

                return Cooldown.Instance.CanCast(spell) && (Player.Location.DistanceTo(Target.Location) <= spellObject.Range);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Cast(string spell)
        {
            try
            {
                TargetState = TargeterState.CastingSpell;
                InputControl.UseHot(HotkeysGame.FindSpell(spell)); 
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion

        #region Stancing

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool Stance()
        {
            try
            {
                switch (Rule.DesiredStance)
                {
                    case DesiredStance.MeleeApproach:
                        MeleeApproach();
                        break;
                    case DesiredStance.MeleeCircle:
                        MeleeCircle();
                        break;
                    case DesiredStance.MeleeParry:
                        MeleeParry();
                        break;
                    case DesiredStance.MeleeStrike:
                        MeleeStrike();
                        break;
                    case DesiredStance.DistAway:
                        DistAway();
                        break;
                    case DesiredStance.DistStraight:
                        DistStraight();
                        break;
                    case DesiredStance.DistWait:
                        DistWait();
                        break;
                    case DesiredStance.DistWaitStraight:
                        DistWaitStraight();
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool MeleeApproach()
        {
            try
            {
                Location destination = new Location(-1, -1, -1);

                if (Player.Location.DistanceTo(Target.Location) > 1)
                {
                    destination = Target.Location;
                }
                else
                {

                    #region Refreshing map

                    byte[] map = new byte[65536];
                    Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                    Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                    #endregion

                    #region Player.X == Target.X

                    if (Player.X == Target.X)
                    {
                        Thread.Sleep(100);

                        if (new Random().Next(1, 2) == 1)
                        {
                            if (Walker.Instance.LocIsWalkable(map,maploc,Location.LocationDir(LocDirection.Left, Player.Location),true))
                                destination = Location.LocationDir(LocDirection.Left, Player.Location);
                            else
                                if (Walker.Instance.LocIsWalkable(map,maploc,Location.LocationDir(LocDirection.Right, Player.Location),true))
                                    destination = Location.LocationDir(LocDirection.Right, Player.Location);
                        }
                        else
                        {
                            if (Walker.Instance.LocIsWalkable(map,maploc,Location.LocationDir(LocDirection.Right, Player.Location),true))
                                destination = Location.LocationDir(LocDirection.Right, Player.Location);
                            else
                                if (Walker.Instance.LocIsWalkable(map, maploc, Location.LocationDir(LocDirection.Left, Player.Location), true))
                                    destination = Location.LocationDir(LocDirection.Left, Player.Location);
                        }
                    }

                    #endregion

                    #region Player.Y == Target.Y

                    if (Player.Y == Target.Y)
                    {
                        Thread.Sleep(100);

                        if (new Random().Next(1, 2) == 1)
                        {
                            if (Walker.Instance.LocIsWalkable(map,maploc,Location.LocationDir(LocDirection.Up, Player.Location)))
                                destination = Location.LocationDir(LocDirection.Up, Player.Location);
                            else
                                if (Walker.Instance.LocIsWalkable(map, maploc, Location.LocationDir(LocDirection.Down, Player.Location)))
                                    destination = Location.LocationDir(LocDirection.Down, Player.Location);
                        }
                        else
                        {
                            if (Walker.Instance.LocIsWalkable(map, maploc, Location.LocationDir(LocDirection.Down, Player.Location)))
                                destination = Location.LocationDir(LocDirection.Down, Player.Location);
                            else
                                if (Walker.Instance.LocIsWalkable(map, maploc, Location.LocationDir(LocDirection.Up, Player.Location)))
                                    destination = Location.LocationDir(LocDirection.Up, Player.Location);
                        }
                    }

                    #endregion
                }

                #region Walking

                if (destination.IsValid())
                {


                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;



                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool MeleeStrike()
        {
            try
            {
                Location destination = new Location(-1, -1, -1);

                if (Player.Location.DistanceTo(Target.Location) > 1)
                {
                    Thread.Sleep(250);
                    return true;
                }
                else
                {
                    #region Refreshing map

                    byte[] map = new byte[65536];
                    Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                    Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                    #endregion

                    #region Player.X != Target.X ( and Player.Y == Target.Y )

                    if ((Player.X != Target.X) && (Player.Y != Target.Y))
                    {
                        destination = Location.GetLocationsDimension(2, 2, 2, 2).Where(l =>
                            (l.X == Target.X || l.Y == Target.Y) && Walker.Instance.LocIsWalkable(map, maploc, l, true))
                                .OrderBy(l => Player.Location.DistanceTo(l)).FirstOrDefault();
                    }

                    #endregion
                }

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;
                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }

                #endregion

                #region Turn to monster

                if ((Player.X == Target.X) && (Player.Y == Target.Y - 1))
                    InputControl.Turn(2);

                if ((Player.X == Target.X) && (Player.Y == Target.Y + 1))
                    InputControl.Turn(8);

                if ((Player.X == Target.X + 1) && (Player.Y == Target.Y))
                    InputControl.Turn(4);

                if ((Player.X == Target.X - 1) && (Player.Y == Target.Y))
                    InputControl.Turn(6);

                #endregion

                
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool MeleeCircle()
        {
            try
            {
                Location destination = new Location(-1, -1, -1);

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination

                destination = Location.Locations().Where(l =>
                (l.DistanceTo(Target.Location) == 2) &&
                (Walker.Instance.LocIsWalkable(map, maploc, l, true)))
                .OrderByDescending(l => Math.Abs(l.X - Target.X))
                .OrderByDescending(l => Math.Abs(l.Y - Target.Y))
                .OrderBy(l => Player.Location.DistanceTo(l))
                .FirstOrDefault();

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool MeleeParry()
        {
            try
            {              
                #region Checking if we must move or no
                
                List<Creature> creatures = Creature.CreaturesInBw().Where(c =>
                   Targeting.Instance.InTargeting(c.Name) &&
                   c.Location.IsAdjacentTo(Player.Location) &&
                   c.ID != Target.ID).ToList();

                System.Diagnostics.Debug.WriteLine(String.Format("creatures.Count: {0}", creatures.Count));

                if (creatures.Count == 0)
                {
                    Thread.Sleep(new Random().Next(300,600));
                    return true;
                }

                #endregion

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination

                Location destination = new Location(-1, -1, -1);

                List<Location> locationsWithMonster = new List<Location>();

                if (creatures.Count > 0)
                {
                    creatures.ForEach(c =>
                        {
                            locationsWithMonster.Add(c.Location);
                        });

                    locationsWithMonster = locationsWithMonster.OrderBy(l => l.DistanceTo(Player.Location)).ToList();
                }

                destination = Location.Locations().Where(l =>
                (l.DistanceTo(Target.Location) == 1) &&
                (!l.Equals(Player.Location)) &&
                (!l.Equals(Target.Location)))
                .OrderByDescending(l => l.DistanceTo(locationsWithMonster.ElementAt(0)))
                .FirstOrDefault();

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool DistAway()
        {
            try
            {
                #region Checking if we must move or no

                if (Player.Location.DistanceTo(Target.Location) == Rule.Distance)
                {
                    Thread.Sleep(new Random().Next(20, 50));
                    return true;
                }

                #endregion

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination  

                Location destination = new Location(-1, -1, -1);
                IEnumerable<Location> possibleDestinations = new List<Location>();

                for (int i = 0; i < Rule.Distance; i++)
                {
                    possibleDestinations = Location.Locations().Where(l =>
                    (l.DistanceTo(Target.Location) == (Rule.Distance - i)) &&
                    (!l.Equals(Player.Location)) &&
                    (!l.Equals(Target.Location)) &&
                    (Walker.Instance.LocIsWalkable(map, maploc, l, true)))
                    .OrderByDescending(l => Math.Abs((Math.Abs(l.X - Target.X) + Math.Abs(l.Y - Target.Y))))
                    .OrderBy(l => l.DistanceTo(Player.Location));

                    if (possibleDestinations.Count() > 0)
                    {
                        destination = possibleDestinations.ElementAt(0);
                        break;
                    }
                }

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool DistStraight()
        {
            try
            {
                #region Checking if we must move or no

                if (Player.Location.DistanceTo(Target.Location) >= Rule.Distance)
                {
                    Thread.Sleep(new Random().Next(30, 60));
                    return true;
                }

                #endregion

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination

                Location destination = new Location(-1, -1, -1);
                IEnumerable<Location> possibleDestinations = new List<Location>();

                for (int i = 0; i < Rule.Distance; i++)
                {
                    possibleDestinations = Location.Locations().Where(l =>
                    (l.DistanceTo(Target.Location) == (Rule.Distance - i)) &&
                    (!l.Equals(Player.Location)) &&
                    (!l.Equals(Target.Location)) &&
                    (Walker.Instance.LocIsWalkable(map,maploc,l,true)))
                    .OrderBy(l => Math.Abs((Math.Abs(l.X - Target.X) + Math.Abs(l.Y - Target.Y))))
                    .OrderBy(l => l.DistanceTo(Player.Location));

                    if (possibleDestinations.Count() > 0)
                    {
                        destination = possibleDestinations.FirstOrDefault();
                        break;
                    }
                }

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool DistWait()
        {
            try
            {
                #region Checking if we must move or no

                if (Player.Location.DistanceTo(Target.Location) >= Rule.Distance)
                {
                    Thread.Sleep(new Random().Next(300, 600));
                    return true;
                }

                #endregion

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination

                Location destination = new Location(-1, -1, -1);
                IEnumerable<Location> possibleDestinations = new List<Location>();

                for (int i = 0; i < Rule.Distance; i++)
                {
                    possibleDestinations = Location.Locations().Where(l =>
                    (l.DistanceTo(Target.Location) == (Rule.Distance - i)) &&
                    (!l.Equals(Player.Location)) &&
                    (!l.Equals(Target.Location)) &&
                    (Walker.Instance.LocIsWalkable(map, maploc, l, true)))
                    .OrderByDescending(l => Math.Abs((Math.Abs(l.X - Target.X) + Math.Abs(l.Y - Target.Y))))
                    .OrderBy(l => l.DistanceTo(Player.Location));

                    if (possibleDestinations.Count() > 0)
                    {
                        destination = possibleDestinations.FirstOrDefault();
                        break;
                    }
                }

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        bool DistWaitStraight()
        {
            try
            {
                #region Checking if we must move or no

                if (Player.Location.DistanceTo(Target.Location) >= Rule.Distance)
                {
                    Thread.Sleep(new Random().Next(300, 600));
                    return true;
                }

                #endregion

                #region Refreshing map

                byte[] map = new byte[65536];
                Array.Copy(Map.MapFromMem(), 65536, map, 0, 65536);
                Location maploc = Map.MapToLocation(Map.MapFileName(Player.X, Player.Y, Player.Z));

                #endregion

                #region Choosing a destination

                Location destination = new Location(-1, -1, -1);
                IEnumerable<Location> possibleDestinations = new List<Location>();

                for (int i = 0; i < Rule.Distance; i++)
                {
                    possibleDestinations = Location.Locations().Where(l =>
                    (l.DistanceTo(Target.Location) == (Rule.Distance - i)) &&
                    (!l.Equals(Player.Location)) &&
                    (!l.Equals(Target.Location)) &&
                    (Walker.Instance.LocIsWalkable(map, maploc, l, true)))
                    .OrderByDescending(l => Math.Abs((Math.Abs(l.X - Target.X) + Math.Abs(l.Y - Target.Y))))
                    .OrderBy(l => l.DistanceTo(Player.Location));

                    if (possibleDestinations.Count() > 0)
                    {
                        destination = possibleDestinations.FirstOrDefault();
                        break;
                    }
                }

                #endregion

                #region Walking

                if (destination.IsValid())
                {
                    Walker.Instance.Walksender = WalkSender.Targeting;
                    Walker.Instance.SkipNode = false;
                    Walker.Instance.WalkingMethod = CaveBot.Instance.WalkingMethod;

                    Walker.Instance.SpecialAreas.Clear();

                    foreach (SpecialArea area in CaveBot.Instance.SpecialAreasList.Where(a => a.Active && a.Consideration == WalkSender.Targeting))
                        Walker.Instance.SpecialAreas.Add(area);

                    Walker.Instance.Start = Player.Location;

                    if (!destination.IsValid())
                        return true;

                    Walker.Instance.End = destination;

                    bool result = false;

                    Queues.Add(Task.TargeterStance, new Action(() =>
                    {
                        result = Walker.Instance.Walk();
                    }), 90, 100, true, true);

                    if (destination != Target.Location)
                        Thread.Sleep(50);

                    return result;
                }
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (NullReferenceException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return true;
        }

        #endregion
        
        #endregion

        #region Help methods

        public void StopAttack()
        {
            Pause = true;
        }

        #endregion

    }
}
