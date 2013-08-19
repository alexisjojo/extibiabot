using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Player
    {
        #region Constructor

        public Player()
        {
            
        }

        #endregion

        #region Properties

        public static string Name
        {
            get { return Creature.Creatures().First(c => c.ID == Player.ID).Name; }           
        }

        public static int ID
        {
            get { return Memory.ReadInt32(Addresses.Player.Id); }
        }

        public static int Health
        {
            get { return Memory.ReadInt32(Addresses.Player.XOR) ^ Memory.ReadInt32(Addresses.Player.Health); }
        }

        public static int HealthMax
        {
            get { return Memory.ReadInt32(Addresses.Player.XOR) ^ Memory.ReadInt32(Addresses.Player.HealthMax); }
        }

        public static int HealthPercent
        {
            get { return (int)((decimal)Health / (decimal)HealthMax * 100); }
        }

        public static int Mana
        {
            get { return Memory.ReadInt32(Addresses.Player.XOR) ^ Memory.ReadInt32(Addresses.Player.Mana); }
        }

        public static int ManaMax
        {
            get { return Memory.ReadInt32(Addresses.Player.XOR) ^ Memory.ReadInt32(Addresses.Player.ManaMax); }
        }

        public static int ManaPercent
        {
            get { return (int)((decimal)Mana / (decimal)ManaMax * 100); }
        }

        public static int Cap
        {
            get { return (Memory.ReadInt32(Addresses.Player.XOR) ^ Memory.ReadInt32(Addresses.Player.Cap)) / 100; }
        }

        public static int X
        {
            get { return Memory.ReadInt32(Addresses.Player.X); }
        }

        public static int Y
        {
            get { return Memory.ReadInt32(Addresses.Player.Y); }
        }

        public static int Z
        {
            get { return Memory.ReadInt32(Addresses.Player.Z); }
        }

        public static Location Location
        {
            get { return new Location(X, Y, Z); }
        }

        public static int Soul
        {
            get { return Memory.ReadInt32(Addresses.Player.Soul); }
        }

        public static int Level
        {
            get { return Memory.ReadInt32(Addresses.Player.Level); }
        }

        public static int MagicLevel
        {
            get { return Memory.ReadInt32(Addresses.Player.MagicLevel); }
        }

        public static int Experience
        {
            get { return Memory.ReadInt32(Addresses.Player.Experience); }
        }

        public static int Flags
        {
            get { return Memory.ReadInt32(Addresses.Player.Flags); }
        }

        public static int GreenSquare
        {
            get { return Memory.ReadInt32(Addresses.Player.GreenSquare); }
        }

        public static int RedSquare
        {
            get { return Memory.ReadInt32(Addresses.Player.RedSquare); }
        }

        public static int Fist
        {
            get { return Memory.ReadInt32(Addresses.Player.Fist); }
        }

        public static int Club
        {
            get { return Memory.ReadInt32(Addresses.Player.Club); }
        }

        public static int Sword
        {
            get { return Memory.ReadInt32(Addresses.Player.Sword); }
        }

        public static int Axe
        {
            get { return Memory.ReadInt32(Addresses.Player.Axe); }
        }

        public static int Distance
        {
            get { return Memory.ReadInt32(Addresses.Player.Distance); }
        }

        public static int Shielding
        {
            get { return Memory.ReadInt32(Addresses.Player.Shielding); }
        }

        public static int Fishing
        {
            get { return Memory.ReadInt32(Addresses.Player.Fishing); }
        }

        public static int FistPercent
        {
            get { return Memory.ReadInt32(Addresses.Player.FistPercent); }
        }

        public static int ClubPercent
        {
            get { return Memory.ReadInt32(Addresses.Player.ClubPercent); }
        }

        public static int SwordPercent
        {
            get { return Memory.ReadInt32(Addresses.Player.SwordPercent); }
        }

        public static int AxePercent
        {
            get { return Memory.ReadInt32(Addresses.Player.AxePercent); }
        }

        public static int DistancePercent
        {
            get { return Memory.ReadInt32(Addresses.Player.DistancePercent); }
        }

        public static int ShieldingPercent
        {
            get { return Memory.ReadInt32(Addresses.Player.ShieldingPercent); }
        }

        public static int FishingPercent
        {
            get { return Memory.ReadInt32(Addresses.Player.FishingPercent); }
        }

        public static int Arrow
        {
            get { return Memory.ReadInt32(Addresses.Player.Arrow); }
        }

        public static int Chest
        {
            get { return Memory.ReadInt32(Addresses.Player.Chest); }
        }

        public static int RightHandCount
        {
            get { return Memory.ReadInt32(Addresses.Player.RightHand_count); }
        }

        public static int ArrowCount
        {
            get { return Memory.ReadInt32(Addresses.Player.Arrow_count); }
        }

        public static int Shoes
        {
            get { return Memory.ReadInt32(Addresses.Player.Shoes); }
        }

        public static int PRing
        {
            get { return Memory.ReadInt32(Addresses.Player.PRing); }
        }

        public static int Legs
        {
            get { return Memory.ReadInt32(Addresses.Player.Legs); }
        }

        public static int LeftHand
        {
            get { return Memory.ReadInt32(Addresses.Player.LeftHand); }
        }

        public static int RightHand
        {
            get { return Memory.ReadInt32(Addresses.Player.RightHand); }
        }

        public static int Amulet
        {
            get { return Memory.ReadInt32(Addresses.Player.Amulet); }
        }

        public static int Backpack
        {
            get { return Memory.ReadInt32(Addresses.Player.Backpack); }
        }

        public static int Head
        {
            get { return Memory.ReadInt32(Addresses.Player.Head); }
        }

        public static bool IsPoisoned
        {
            get { return (1 != (Flags & 1) ? false : true); }
        }

        public static bool IsBurned
        {
            get { return (2 != (Flags & 2) ? false : true); }
        }

        public static bool IsEnergized
        {
            get { return (4 != (Flags & 4) ? false : true); }
        }

        public static bool IsDrunken
        {
            get { return (8 != (Flags & 8) ? false : true); }
        }

        public static bool IsManaShield
        {
            get { return (16 != (Flags & 16) ? false : true); }
        }

        public static bool IsParalyzed
        {
            get { return (32 != (Flags & 32) ? false : true); }
        }

        public static bool IsHasted
        {
            get { return (64 != (Flags & 64) ? false : true); }
        }

        public static bool IsStrengthened
        {
            get { return (4096 != (Flags & 4096) ? false : true); }
        }

        public static bool IsPvpSigned
        {
            get { return (8192 != (Flags & 8192) ? false : true); }
        }

        public static bool IsPz
        {
            get { return (16384 != (Flags & 16384) ? false : true); }
        }

        #endregion
    }
}
