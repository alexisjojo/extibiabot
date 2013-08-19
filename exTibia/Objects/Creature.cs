using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{

    public class Creature
    {
        #region Fields

        private int _creatureAddress;

        #endregion

        #region Constructor

        public Creature()
        {

        }

        public Creature(int creatureAddress)
        {
            this._creatureAddress = creatureAddress;
        }

        public Creature(uint creatureAddress)
        {
            this._creatureAddress = (int)creatureAddress;
        }

        #endregion

        #region Properties

        public int Address
        {
            get { return _creatureAddress; }
        }

        public int ID
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Id); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Id, value); }
        }

        public CreatureType Type
        {
            get { return (CreatureType)Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Type); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Type, (int)value); }
        }

        public string Name
        {
            get { return Memory.ReadString(this._creatureAddress + Addresses.Creature.Name); }
            set { Memory.WriteString(this._creatureAddress + Addresses.Creature.Name, value); }
        }

        public int X
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.X); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.X, value); }
        }

        public int Y
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Y); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Y, value); }
        }

        public int Z
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Z); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Z, value); }
        }

        public Location Location
        {
            get { return new Location(X, Y, Z); }
        }

        public int OffsetHoriz
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.OffsetHoriz); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.OffsetHoriz, value); }
        }

        public int OffsetVert
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.OffsetVert); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.OffsetVert, value); }
        }

        public bool IsWalking
        {
            get { return Convert.ToBoolean(Memory.ReadInt32(this._creatureAddress + Addresses.Creature.IsWalking)); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.IsWalking, Convert.ToByte(value)); }
        }

        public Directions Direction
        {
            get { return (Directions)Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Direction); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Direction, (int)value); }
        }

        public uint NextCreature
        {
            get { return Memory.ReadUInt32(this._creatureAddress + Addresses.Creature.IndexNextCreature); }
            set { Memory.WriteUInt32(this._creatureAddress + Addresses.Creature.IndexNextCreature, value); }
        }

        public int Outfit
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Outfit); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Outfit, value); }
        }

        public int ColorHead
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.ColorHead); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.ColorHead, value); }
        }

        public int ColorBody
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.ColorBody); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.ColorBody, value); }
        }

        public int ColorLegs
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.ColorLegs); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.ColorLegs, value); }
        }

        public int ColorFeet
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.ColorFeet); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.ColorFeet, value); }
        }

        public int Addon
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Addon); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Addon, value); }
        }

        public int MountId
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.MountId); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.MountId, value); }
        }

        public int Light
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Light); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Light, value); }
        }

        public int LightColor
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.LightColor); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.LightColor, value); }
        }

        public int BlackSquare
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.BlackSquare); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.BlackSquare, value); }
        }

        public int HPBar
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.HPBar); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.HPBar, value); }
        }

        public int WalkSpeed
        {
            get { return Memory.ReadInt32(this._creatureAddress + Addresses.Creature.WalkSpeed); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.WalkSpeed, value); }
        }

        public bool IsVisible
        {
            get { return Convert.ToBoolean(Memory.ReadInt32(this._creatureAddress + Addresses.Creature.IsVisible)); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.IsVisible, Convert.ToByte(value)); }
        }

        public Skull Skull
        {
            get { return (Skull)Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Skull); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Skull, (int)value); }
        }

        public Party Party
        {
            get { return (Party)Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Party); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Party, (int)value); }
        }

        public WarIcon WarIcon
        {
            get { return (WarIcon)Memory.ReadInt32(this._creatureAddress + Addresses.Creature.WarIcon); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.WarIcon, (int)value); }
        }

        public bool Active
        {
            get { return Convert.ToBoolean(Memory.ReadInt32(this._creatureAddress + Addresses.Creature.Active)); }
            set { Memory.WriteInt32(this._creatureAddress + Addresses.Creature.Active, Convert.ToByte(value)); }
        }

        public int Danger
        {
            get
            {
                if (Targeting.Instance.TargetingRules.Where(r => r.MonsterName.ToLower() == Name.ToLower()).Count() > 0)
                    return Targeting.Instance.TargetingRules.Where(r => r.MonsterName.ToLower() == Name.ToLower()).First().Settings[0].Danger;
                else
                    return -1;
            }
        }

        public int ListOrder
        {
            get
            {
                if (Targeting.Instance.TargetingRules.Where(r => r.MonsterName.ToLower() == Name.ToLower()).Count() > 0)
                    return Targeting.Instance.TargetingRules.IndexOf(Targeting.Instance.TargetingRules.Where(r => r.MonsterName.ToLower() == Name.ToLower()).First());
                else
                    return -1;
            }
        }

        #endregion

        #region Methods

        public bool IsAlive()
        {
            return this.HPBar > 0;
        }

        #endregion

        #region Static methods

        public static Collection<Creature> Creatures()
        {
            try
            {
                Collection<Creature> _list = new Collection<Creature>();

                for (int i = Addresses.Creature.Start; i < Addresses.Creature.End; i += Addresses.Creature.StepCreatures)
                {
                    if (Memory.ReadInt32(i + Addresses.Creature.Z) == Player.Z)
                    {
                        _list.Add(new Creature(i));
                    }
                }

                return _list;
            }
            catch (InvalidOperationException e)
            {
                Debug.Report(e);
                return null;
            }
            finally
            {

            }
        }

        
        public static Collection<Creature> Monsters()
        {
            try
            {
                Collection<Creature> _list = new Collection<Creature>();
                for (int i = Addresses.Creature.Start; i < Addresses.Creature.End; i += Addresses.Creature.StepCreatures)
                {
                    if (Memory.ReadInt32(i + Addresses.Creature.Z) == Player.Z)
                    {
                        if (Memory.ReadInt32(i + Addresses.Creature.Id) > 0x40000000)
                        {
                            _list.Add(new Creature(i));
                        }
                    }
                }

                return _list;
            }
            catch (InvalidOperationException e)
            {
                Debug.Report(e);
                return null;
            }
        }

        public static Collection<Creature> CreaturesInBw()
        {
            Collection<Creature> creatures = new Collection<Creature>();

            uint indexCreature = Memory.ReadUInt32(Addresses.Creature.FirstCreature);

            while (indexCreature < uint.MaxValue)
            {
                Creature creature = CreatureAt((int)indexCreature);
                indexCreature = creature.NextCreature;
                creatures.Add(creature);
            }

            return creatures;
        }

        public static Creature CreatureAt(int index)
        {
            return new Creature(Addresses.Creature.Start + (index * Addresses.Creature.StepCreatures));
        }

        #endregion
    }

    #region Comparers

    public class CreatureSortProximity : IComparer<Creature>
    {
        public int Compare(Creature x, Creature y)
        {
            if (x.Location.DistanceTo(Player.Location) < y.Location.DistanceTo(Player.Location)) return -1;
            else if (x.Location.DistanceTo(Player.Location) > y.Location.DistanceTo(Player.Location)) return 1;
            else return 0;
        }
    }

    public class CreatureSortByHealth : IComparer<Creature>
    {
        public int Compare(Creature x, Creature y)
        {
            if (x.HPBar > y.HPBar) return 1;
            else if (x.HPBar < y.HPBar) return -1;
            else return 0;
        }
    }

    public class CreatureSortByDanger : IComparer<Creature>
    {
        public int Compare(Creature x, Creature y)
        {
            if (x.Danger < y.Danger) return 1;
            else if (x.Danger > y.Danger) return -1;
            else return 0;
        }
    }

    public class CreatureSortByListOrder : IComparer<Creature>
    {
        public int Compare(Creature x, Creature y)
        {
            if (x.ListOrder < y.ListOrder) return 1;
            else if (x.ListOrder > y.ListOrder) return -1;
            else return 0;
        }
    }

    #endregion
}
