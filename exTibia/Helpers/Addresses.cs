using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class Addresses
    {
        #region Base Address of Tibia Client

        public readonly static int baseAdress = GameClient.Tibia.MainModule.BaseAddress.ToInt32() - 0x400000;

        #endregion

        #region Tibia Client addresses and offsets

        internal static class Client
        {
            public readonly static int GUI = 0x7B7520 + baseAdress;
            public readonly static int StatusBarText = 0x802948 + baseAdress;
            public readonly static int StatusbarTime = 0x802948 - 4 + baseAdress;
            public readonly static int IsConnected = 0x7C0CF8 + baseAdress;
            public readonly static int AttackMode = 0x7C0AB6 + baseAdress;
            public readonly static int ChaseType = 0x7C0AC0 + baseAdress;
            public readonly static int Dialog = 0x7B7DC0 + baseAdress;
            public readonly static int DatPointer = 0x7BE5EC + baseAdress;
            public readonly static int DatStepItems = 136;
            public readonly static int DatFlagsOffset = 68;
            public readonly static int TibiaTime = 0x9DB8B4 + baseAdress;
            public readonly static int Cooldown = 0x804854 + baseAdress;
            public readonly static int CoolDownCategoryStart = Cooldown;
            public readonly static int CoolDownItemsStart = Cooldown + 0xC;
            public readonly static int CoolDownItems = Cooldown + 0x10;
            public readonly static int LastPacket = 0x7B7DC0 + baseAdress;
            public readonly static int Width = 0x7C0CD8 + baseAdress;
            public readonly static int Height = 0x7C0D40 + baseAdress; 
        }

        #endregion

        #region Player Addresses //UPDATED to 9.83

        internal static class Player
        {
            public readonly static int XOR = 0x7BA090 + baseAdress;
            public readonly static int Id = 0x987EA4 + baseAdress;
            public readonly static int Health = 0x950000 + baseAdress;
            public readonly static int HealthMax = 0x987E9C + baseAdress;
            public readonly static int Mana = 0x7BA0E4 + baseAdress;
            public readonly static int ManaMax = 0x7BA094 + baseAdress;
            public readonly static int Cap = 0x987E94 + baseAdress;
            public readonly static int Z = 0x987EB0 + baseAdress;
            public readonly static int X = Z - 8;
            public readonly static int Y = Z - 4;
            public readonly static int Soul = 0x7BA0D0 + baseAdress;
            public readonly static int Level = 0x7BA0CC + baseAdress;
            public readonly static int MagicLevel = 0x7BA0D4 + baseAdress;
            public readonly static int Experience = 0x7BA0A0 + baseAdress;
            public readonly static int Flags = 0x7BA054 + baseAdress;
            public readonly static int GreenSquare = 0x7BA088 + baseAdress;
            public readonly static int RedSquare = 0x7BA0E0 + baseAdress;


            public readonly static int Fist = 0x987E78 + baseAdress;
            public readonly static int Club = Fist + 4;
            public readonly static int Sword = Fist + 8;
            public readonly static int Axe = Fist + 12;
            public readonly static int Distance = Fist + 16;
            public readonly static int Shielding = Fist + 20;
            public readonly static int Fishing = Fist + 24;

            public readonly static int FistPercent = 0x7B70BC + baseAdress;
            public readonly static int ClubPercent = FistPercent + 4;
            public readonly static int SwordPercent = FistPercent + 8;
            public readonly static int AxePercent = FistPercent + 12;
            public readonly static int DistancePercent = FistPercent + 16;
            public readonly static int ShieldingPercent = FistPercent + 20;
            public readonly static int FishingPercent = FistPercent + 24;

            public readonly static int Head = 0x9DE690 + baseAdress;
            public readonly static int Backpack = Head + 24;
            public readonly static int Amulet = Head + 12;
            public readonly static int RightHand = Head + 48;
            public readonly static int LeftHand = Head + 60;
            public readonly static int Legs = Head + 72;
            public readonly static int PRing = Head + 96;
            public readonly static int Shoes = Head + 84;
            public readonly static int Arrow = Head + 108;
            public readonly static int Chest = Head + 36;
            public readonly static int RightHand_count = Head + RightHand + 4;
            public readonly static int Arrow_count = Head + Arrow + 4;
        }

        #endregion

        #region Creature addresses and offsets //UPDATED to 9.83

        internal static class Creature
        {
            public readonly static int Start = 0x950008 + baseAdress;
            public readonly static int StepCreatures = 0xB0;
            public readonly static int MaxCreatures = 1300;
            public readonly static int End = Start + (StepCreatures * MaxCreatures);
            public readonly static int FirstCreature = 0x7BA0A8 + baseAdress;

            public readonly static int Id = 0;
            public readonly static int Type = 3;
            public readonly static int Name = 4;
            public readonly static int X = 44;
            public readonly static int Y = 40;
            public readonly static int Z = 36;
            public readonly static int OffsetHoriz = 48;
            public readonly static int OffsetVert = 52;
            public readonly static int IsWalking = 80;
            public readonly static int Direction = 56;
            public readonly static int IndexNextCreature = 92;
            public readonly static int Outfit = 96;
            public readonly static int ColorHead = 100;
            public readonly static int ColorBody = 104;
            public readonly static int ColorLegs = 108;
            public readonly static int ColorFeet = 112;
            public readonly static int Addon = 116;
            public readonly static int MountId = 120;
            public readonly static int Light = 124;
            public readonly static int LightColor = 128;
            public readonly static int BlackSquare = 136;
            public readonly static int HPBar = 140;
            public readonly static int WalkSpeed = 144;
            public readonly static int IsVisible = 148;
            public readonly static int Skull = 152;
            public readonly static int Party = 156;
            public readonly static int WarIcon = 168;
            public readonly static int Active = 172;
        }

        #endregion

        #region Container addresses and offsets  //UPDATED to 9.83

        internal static class Container
        {
            public readonly static int Start = 0x809580 + baseAdress;
            public readonly static int MaxContainers = 16;
            public readonly static int StepContainer = 492;
            public readonly static int End = Start + (MaxContainers * StepContainer);

            public readonly static int HasParent = 0;
            public readonly static int Id = 12;
            public readonly static int Name = 16;
            public readonly static int Amount = 48;
            public readonly static int IsOpen = 52;
            public readonly static int Volume = 56;
            public readonly static int ItemCount = 64;
            public readonly static int ItemId = 68;
            public readonly static int StepSlot = 12;

        }

        #endregion

        #region Hotkey addresses and offsets //UPDATED to 9.83

        internal static class Hotkey
        {
            public readonly static int TextStart = 0x7C1838 + baseAdress;
            public readonly static int ObjectStart = 0x7C3DD0 + baseAdress;
            public readonly static int TextStep = 0x100;
            public readonly static int ObjectStep = 0x4;
        }

        #endregion

        #region Map addresses and offsets //UPDATED to 9.83

        internal static class Map
        {
            public readonly static int Pointer = 0x9DE6AC + baseAdress;
            public readonly static int Memory = 0x80BDF8 + baseAdress;
            public readonly static int TilePointer = 0x9DE6C8 + baseAdress;
            public readonly static int MemoryOffset = 0x200A8;

            public readonly static int MaxX = 18;
            public readonly static int MaxY = 14;
            public readonly static int MaxZ = 8;
            public readonly static int MaxTiles = 2016;

            public readonly static int StepTile = 168;
            public readonly static int StepTileObject = 12;
            public readonly static int DistanceTileObjectCount = 0;
            public readonly static int DistanceTileObjects = 48;
            public readonly static int DistanceObjectId = 4;
            public readonly static int DistanceObjectData = 0;
            public readonly static int DistanceObjectDataEx = 8;
            public readonly static int TileOrder = 4;

        }

        #endregion
    }
}
