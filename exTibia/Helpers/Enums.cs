using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace exTibia.Helpers
{
    class Enums
    {
    }


    public enum PipeCommand : byte
    {
        Unknown = 0x00,
        UnInject = 0x01,
        EnableHooks = 0x02,
        DisableHooks = 0x03,
        PrintText = 0x04,
        UnPrintText = 0x05,
        PrintItem = 0x06,
        UnPrintItem = 0x07
    }

    public enum PacketType : byte
    {
        SelfAppear = 0x0A,
        GMAction = 0x0B,
        ErrorMessage = 0x14,
        FyiMessage = 0x15,
        WaitingList = 0x16,
        Ping = 0x1E,
        Death = 0x28,
        CanReportBugs = 0x32,
        MapDescription = 0x64,
        MoveNorth = 0x65,
        MoveEast = 0x66,
        MoveSouth = 0x67,
        MoveWest = 0x68,
        TileUpdate = 0x69,
        TileAddThing = 0x6A,
        TileTransformThing = 0x6B,
        TileRemoveThing = 0x6C,
        CreatureMove = 0x6D,
        ContainerOpen = 0x6E,
        ContainerClose = 0x6F,
        ContainerAddItem = 0x70,
        ContainerUpdateItem = 0x71,
        ContainerRemoveItem = 0x72,
        InventorySetSlot = 0x78,
        InventoryResetSlot = 0x79,
        ShopWindowOpen = 0x7A,
        ShopSaleGoldCount = 0x7B,
        ShopWindowClose = 0x7C,
        SafeTradeRequestAck = 0x7D,
        SafeTradeRequestNoAck = 0x7E,
        SafeTradeClose = 0x7F,
        WorldLight = 0x82,
        MagicEffect = 0x83,
        AnimatedText = 0x84,
        Projectile = 0x85,
        CreatureSquare = 0x86,
        CreatureHealth = 0x8C,
        CreatureLight = 0x8D,
        CreatureOutfit = 0x8E,
        CreatureSpeed = 0x8F,
        CreatureSkull = 0x90,
        CreatureShield = 0x91,
        ItemTextWindow = 0x96,
        HouseTextWindow = 0x97,
        PlayerStatus = 0xA0,
        PlayerSkillsUpdate = 0xA1,
        PlayerFlags = 0xA2,
        CancelTarget = 0xA3,
        CreatureSpeech = 0xAA,
        ChannelList = 0xAB,
        ChannelOpen = 0xAC,
        ChannelOpenPrivate = 0xAD,
        RuleViolationOpen = 0xAE,
        RuleViolationRemove = 0xAF,
        RuleViolationCancel = 0xB0,
        RuleViolationLock = 0xB1,
        PrivateChannelCreate = 0xB2,
        ChannelClosePrivate = 0xB3,
        TextMessage = 0xB4,
        PlayerWalkCancel = 0xB5,
        FloorChangeUp = 0xBE,
        FloorChangeDown = 0xBF,
        OutfitWindow = 0xC8,
        VipState = 0xD2,
        VipLogin = 0xD3,
        VipLogout = 0xD4,
        QuestList = 0xF0,
        QuestPartList = 0xF1,
        ShowTutorial = 0xDC,
        AddMapMarker = 0xDD,
    }

    public enum Messagetype
    {
        STATUS,
        DEFAULT,
        WHISPER,
        YELL,
        NPCTOPLAYER = 5,
        PRIVATE,
        CHANNEL,
        REDALERT = 15,
        RAID_ADVANCE,
        WELCOME,
        STATUSLOG,
        INFO,
        SEND = 21
    }

    public enum LocationType
    {
        Container,
        Map,
        Slot
    }

    public enum AlertAction
    {
        PlaySound,
        FocusClient,
        PauseBot,
        Disconnect
    }

    public enum AlertType
    {
        PlayerOnScreen,
        PlayerAttacking,
        DefaultMessage,
        PrivateMessage,
        Disconnected,
        CrashedFroze
    }

    public enum Task
    {
        CaveBotProcessWaypoint,
        TargetingExecuteRule,
        TargeterCastSpell,
        TargeterStance
    }

    public enum Directions
    {
        North = 0,
        NorthEast = 5,
        NorthWest = 8,
        South = 2,
        SouthEast = 6,
        SouthWest = 7,
        East = 1,
        West = 3,
        Center = 9
    }

    public enum WalkingMethod
    {
        [Description("Arrow Keys")]
        ArrowKeys,
        [Description("Map Clicks")]
        MapClicks,
        [Description("Mixed")]
        Mixed
    }

    [FlagsAttribute]
    public enum CreatureType : int
    {
        None = 0x0,
        Target = 0x1,
        NPC = 0x40
    }

    public enum ToolsRope
    {
        [Description("Rope")]
        Rope,
        [Description("Elvenhair Rope")]
        ElvenhairRope,
        [Description("Sneaky Stabber Of Eliteness")]
        SneakyStabberOfEliteness,
        [Description("Squeezing Gear Of Girlpower")]
        SqueezingGearOfGirlpower,
        [Description("Whacking Driller Of Fate")]
        WhackingDrillerOfFate
    }

    public enum ToolsShovel
    {
        [Description("Shovel")]
        Shovel,
        [Description("Light Shovel")]
        LightShovel,
        [Description("Sneaky Stabber Of Eliteness")]
        SneakyStabberOfEliteness,
        [Description("Squeezing Gear Of Girlpower")]
        SqueezingGearOfGirlpower,
        [Description("Whacking Driller Of Fate")]
        WhackingDrillerOfFate
    }

    [FlagsAttribute]
    public enum Skull : int
    {
        None = 0,
        Yellow = 1,
        Green = 2,
        White = 3,
        Red = 4,
        Black = 5
    }

    public enum Priority
    {
        Realtime = 1,
        High = 2,
        AboveNormal = 3,
        Normal = 4,
        BelowNormal = 5,
        Low = 6,
        None = 0
    }

    public enum WarIcon
    {
        None = 0,
        Green = 1,
        Red = 2,
        Blue = 3
    }

    public enum Party
    {
        None = 0,
        Host = 1,
        Inviter = 1,
        Guest = 2,
        Invitee = 2,
        Member = 3,
        Leader = 4,
        MemberSharedExp = 5,
        LeaderSharedExp = 6,
        MemberSharedExpInactive = 7,
        LeaderSharedExpInactive = 8
    }

    public enum WaypointType
    {
        Stand,
        Node,
        Walk,
        Rope,
        Shovel,
        Ladder,
        Use,
        Action
    }

    public enum WalkSender
    {
        Walking,
        Targeting,
        Looting
    }

    public enum HealerState
    {
        CheckingRule,
        ExecuteRule,
    }

    public enum HealerRangeType
    {
        Exact,
        Percent
    }

    public enum HealerAdditional
    {
        Pvpsigned,
        Poisoned,
        Burned,
        Energized,
        Drunken,
        ManaShield,
        Paralyzed,
        Hasted,
        InsidePz,
        Strengthened
    }

    public enum TargetingState
    {
        Running,
        CheckingRule,
    }

    public enum TargetPriority
    {
        ListOrder,
        Health,
        Proximity,
        Danger
    }

    [FlagsAttribute]
    public enum ItemFlags
    {
        [Description("None")]
        None = 0,
        [Description("IsGround")]
        IsGround = 1,
        [Description("TopOrder1")]
        TopOrder1 = 2,
        [Description("TopOrder2")]
        TopOrder2 = 4,
        [Description("TopOrder3")]
        TopOrder3 = 8,
        [Description("IsContainer")]
        IsContainer = 16,
        [Description("IsStackable")]
        IsStackable = 32,
        [Description("IsCorpse")]
        IsCorpse = 64,
        [Description("IsUsable")]
        IsUsable = 128,
        [Description("IsWritable")]
        IsWritable = 256,
        [Description("IsReadable")]
        IsReadable = 512,
        [Description("IsFluidContainer")]
        IsFluidContainer = 1024,
        [Description("IsSplash")]
        IsSplash = 2048,
        [Description("Blocking")]
        Blocking = 4096,
        [Description("IsImmovable")]
        IsImmovable = 8192,
        [Description("BlocksMissiles")]
        BlocksMissiles = 16384,
        [Description("BlocksPath")]
        BlocksPath = 32768,
        [Description("IsPickupable")]
        IsPickupable = 65536,
        [Description("IsHangable")]
        IsHangable = 131072,
        [Description("IsHangableHorizontal")]
        IsHangableHorizontal = 262144,
        [Description("IsHangableVertical")]
        IsHangableVertical = 524288,
        [Description("IsRotatable")]
        IsRotatable = 1048576,
        [Description("IsLightSource")]
        IsLightSource = 2097152,
        [Description("Floorchange")]
        Floorchange = 4194304,
        [Description("IsShifted")]
        IsShifted = 8388608,
        [Description("HasHeight")]
        HasHeight = 16777216,
        [Description("IsLayer")]
        IsLayer = 33554432,
        [Description("IsIdleAnimation")]
        IsIdleAnimation = 67108864,
        [Description("HasAutoMapColor")]
        HasAutoMapColor = 134217728,
        [Description("HasHelpLens")]
        HasHelpLens = 268435456,
        [Description("Unknown")]
        Unknown = 536870912,
        [Description("IgnoreStackpos")]
        IgnoreStackpos = 1073741824,
    }

    public enum LocDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public enum SpellCategory
    {
        Attack,
        Healing,
        Summon,
        Supply,
        Support,
        Potion,
        Special,
    }

    public enum SpellType
    {
        Instant,
        ItemTransformation,
        Creation,
        Missile,
        Strike,
        Area,
        Wave,
        Beam,
    }

    public enum TargeterState
    {
        CheckingCastingSpell,
        CastingSpell,
    }

    public enum MonsterAttack
    {
        DontAvoid,
        AvoidWave,
        AvoidBeam
    }

    public enum DesiredStance
    {
        MeleeParry,
        MeleeStrike,
        MeleeApproach,
        MeleeCircle,
        DistAway,
        DistWait,
        DistStraight,
        DistWaitStraight,
        LoseTarget
    }

    public enum DesiredAttack
    {
        Attack,
        Follow
    }

    public enum AttackMode
    {
        Offensive,
        Balanced,
        Defensive
    }
}
