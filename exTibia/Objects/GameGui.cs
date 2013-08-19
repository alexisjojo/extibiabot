using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Security.Permissions;
using System.Security;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{

    public class GuiBattle
    {
        #region Singleton

        private static GuiBattle _instance = new GuiBattle();

        public static GuiBattle Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields
   
        private bool _isOpened;
        private int _positionX;
        private int _positionY;
        private int _width;
        private int _height;
        private int _maxItems;
        private int _heightBar;
        private int _positionBar;
        private int _areaVision;
        private int _valor;
        private bool _minimized;

        #endregion

        #region Properties

        public bool IsOpened
{
  get { return _isOpened; }
  set { _isOpened = value; }
}
        public int PositionX
        {
          get { return _positionX; }
          set { _positionX = value; }
        }
        public int PositionY
        {
          get { return _positionY; }
          set { _positionY = value; }
        }
        public int Width
        {
          get { return _width; }
          set { _width = value; }
        }
        public int Height
        {
          get { return _height; }
          set { _height = value; }
        }
        public int MaxItems
        {
          get { return _maxItems; }
          set { _maxItems = value; }
        }
        public int HeightBar
        {
          get { return _heightBar; }
          set { _heightBar = value; }
        }
        public int PositionBar
        {
          get { return _positionBar; }
          set { _positionBar = value; }
        }
        public int AreaVision
{
  get { return _areaVision; }
  set { _areaVision = value; }
}
        public int Valor
{
  get { return _valor; }
  set { _valor = value; }
}
        public bool Minimized
{
  get { return _minimized; }
  set { _minimized = value; }
}
        public Point MinimiseButton
       {
           get
           {
               return new Point(PositionX + 154, PositionY + 8);
           }
       }
        public Point CloseButton
       {
           get
           {
               return new Point(PositionX + 166, PositionY + 8);
           }
       }
        public Point ScrollUpButton
       {
           get
           {
               return new Point(PositionX + 166, PositionY + 0x14);
           }
       }
        public Point ScrollDownButton
       {
           get
           {
               return new Point(PositionX + 166, PositionY + Height - 10);
           }
       }
        public Point ScrollButton
       {
           get
           {
               return new Point(PositionX + 166, PositionY + 0x24 + PositionBar);
           }
       }
        public Point Bottom
       {
           get
           {
               return new Point(PositionX + 85, PositionY + Height - 2);
           }
       }

        #endregion

        #region Methods

        public void UpdateGUI()
        {
            try
            {
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                PositionX = 0;
                PositionY = 0;
                PositionX += Memory.ReadInt32(gp + 0x14);
                PositionY += Memory.ReadInt32(gp + 0x18);

                int pointerToPosition = Memory.ReadInt32(gp + 0x38);

                PositionX += Memory.ReadInt32(pointerToPosition + 0x14);
                PositionY += Memory.ReadInt32(pointerToPosition + 0x18);

                int basePointer = Memory.ReadInt32(pointerToPosition + 0x24);
                IsOpened = false;
                for (int window = 0; window < 20; window++)
                {
                    int windowId = Memory.ReadInt32(basePointer + 0x2C);

                    if (windowId == 0x7)
                    {
                        IsOpened = true;
                        PositionX += Memory.ReadInt32(basePointer + 0x14);
                        PositionY += Memory.ReadInt32(basePointer + 0x18);
                        Width = Memory.ReadInt32(basePointer + 0x1C);
                        Height = Memory.ReadInt32(basePointer + 0x20);
                        Minimized = (Memory.ReadInt32(basePointer + 0x64) == 1);

                        int pointerToMaxItems = Memory.ReadInt32(basePointer + 0x44);
                        MaxItems = Memory.ReadInt32(pointerToMaxItems + 0x34);

                        int pointerUnknown01 = Memory.ReadInt32(pointerToMaxItems + 0x24);

                        int pointerToBars = Memory.ReadInt32(pointerUnknown01 + 0x24);
                        HeightBar = Memory.ReadInt32(pointerToBars + 0x2C);
                        PositionBar = Memory.ReadInt32(pointerToBars + 0x30);
                        AreaVision = Memory.ReadInt32(pointerToBars + 0x34);
                        Valor = Memory.ReadInt32(pointerToBars + 0x3C);
                        return;
                    }
                    basePointer = Memory.ReadInt32(basePointer + 0x10);
                }
                IsOpened = false;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public Point GetCreatureCords(int index)
        {
            Random random = new Random();
            Point result = new Point(PositionX + random.Next(35, 70), 0);

            UpdateGUI();

            try
            {
                int startY = PositionY + 22; // num
                int battleItemHeight = 22; //num2
                int clicked = 0; //num3

                while((battleItemHeight * index < PositionBar || battleItemHeight * index > PositionBar + AreaVision - 15) && clicked < 20)
                {
                    clicked++;

                    if (battleItemHeight * index < PositionBar && battleItemHeight * index < PositionBar + AreaVision - 15)
                        InputControl.Instance.LeftClickPoint(ScrollUpButton);
                    if (battleItemHeight * index > PositionBar + AreaVision - 15)
                        InputControl.Instance.LeftClickPoint(ScrollDownButton);
                    UpdateGUI();
                }

                result.Y = startY + battleItemHeight * index - PositionBar;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return result;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void ResizeBattleList(int index)
        {
            UpdateGUI();

            try
            {
                Point start = Bottom;
                Point end = new Point();
                
                int minHeight = 38;
                int newHeight = 38 + (index - 1) * 22;
                if (newHeight < 38)
                {
                    newHeight = minHeight;
                }
                end.X = Bottom.X;
                end.Y = Bottom.Y + newHeight;

                InputControl.Instance.Drag(start,end);
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }

            UpdateGUI();
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool MarkMonsterInBattle(Creature c)
        {
            try
            {
                UpdateGUI();

                Collection<Creature> creaturesInBattle = Creature.CreaturesInBw();

                int MonsterID = -1;

                if (creaturesInBattle.Where(a => a.ID == c.ID).Count() > 0)
                    MonsterID = creaturesInBattle.IndexOf(creaturesInBattle.Where(a => a.ID == c.ID).First());
                else
                    return false;
                InputControl.Instance.LeftClickPoint(GetCreatureCords(MonsterID++));
            }
            catch
            {

            }
            return true;
        }

        #endregion
    }

    public class GuiButtons
    {
        #region Singleton

        private static GuiButtons _instance = new GuiButtons();

        public static GuiButtons Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private int _positionX;
        private int _positionY;
        private int _width;
        private int _height;
        private bool _skills;
        private bool _battle;
        private bool _vip;

        #endregion

        #region Properties

        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public bool Skills
        {
            get { return _skills; }
            set { _skills = value; }
        }
        public bool Battle
        {
            get { return _battle; }
            set { _battle = value; }
        }
        public bool Vip
        {
            get { return _vip; }
            set { _vip = value; }
        }
        public Point SkillsButton
        {
            get
            {
                return new Point(PositionX + 26, PositionY + 13);
            }
        }
        public Point BattleButton
        {
            get
            {
                return new Point(PositionX + 0x40, PositionY + 13);
            }
        }
        public Point VIPButton
        {
            get
            {
                return new Point(PositionX + 102, PositionY + 13);
            }
        }
        public Point LogOutButton
        {
            get
            {
                return new Point(PositionX + 147, PositionY + 13);
            }
        }

        #endregion

        #region Methods

        public void UpdateGUI()
        {
            try
            {
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                PositionX = 0;
                PositionY = 0;
                PositionX += Memory.ReadInt32(gp + 0x14);
                PositionY += Memory.ReadInt32(gp + 0x18);

                int pointerToPosition = Memory.ReadInt32(gp + 0x38);

                PositionX += Memory.ReadInt32(pointerToPosition + 0x14);
                PositionY += Memory.ReadInt32(pointerToPosition + 0x18);

                int basePointer = Memory.ReadInt32(pointerToPosition + 0x24);

                for (int window = 0; window < 4; window++)
                {
                    int windowId = Memory.ReadInt32(basePointer + 0x2C);

                    if (windowId == 0x5)
                    {
                        PositionX += Memory.ReadInt32(basePointer + 0x14);
                        PositionY += Memory.ReadInt32(basePointer + 0x18);
                        Width = Memory.ReadInt32(basePointer + 0x1C);
                        Height = Memory.ReadInt32(basePointer + 0x20);

                        int pointerToBtns = Memory.ReadInt32(basePointer + 0x24);

                        int pointerToVip = Memory.ReadInt32(pointerToBtns + 0x10);
                        Vip = (Memory.ReadInt32(pointerToVip + 0x7C) != 0);

                        int pointerToBattle = Memory.ReadInt32(pointerToVip + 0x10);
                        Battle = (Memory.ReadInt32(pointerToBattle + 0x7C) != 0);

                        int pointerToSkills = Memory.ReadInt32(pointerToBattle + 0x10);
                        Skills = (Memory.ReadInt32(pointerToSkills + 0x7C) != 0);

                        break;
                    }
                    basePointer = Memory.ReadInt32(basePointer + 0x10);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

    public class GuiChannel
    {
        #region Fields

        private string _name;
        private int _pointer;
        private bool _selected;
        private int _maxLines;
        private int _firstPointer;
        private bool _open;
        private int _lastMsgPointer;
        private Collection<GuiMessage> _messages = new Collection<GuiMessage>();

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Pointer
        {
            get { return _pointer; }
            set { _pointer = value; }
        }
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }
        public int FirstPointer
        {
            get { return _firstPointer; }
            set { _firstPointer = value; }
        }
        public bool Open
        {
            get { return _open; }
            set { _open = value; }
        }
        public int LastMsgPointer
        {
            get { return _lastMsgPointer; }
            set { _lastMsgPointer = value; }
        }
        public Collection<GuiMessage> Messages
        {
            get { return _messages; }
        }

        #endregion

        #region Constructor

        public GuiChannel()
        {
            Selected = false;
            Pointer = 0;
            MaxLines = 0;
            Open = false;
            LastMsgPointer = 0;
        }

        #endregion

        #region Methods

        public void RemoveOldMessages()
        {
            try
            {
                double timeNow = (double)DateTime.Now.Ticks;
                double maxTime = 120000.0;

                Collection<GuiMessage> messages = new Collection<GuiMessage>();

                foreach (GuiMessage message in Messages)
                {
                    /*int x1 = (int)(timeNow - message.Tick / 10000.0);*/
                    double messageTime = (double)((int)((timeNow - message.Tick) / 10000.0));
                    if (messageTime > maxTime)
                        messages.Add(message);
                }
                foreach (GuiMessage message in Messages)
                    Messages.Remove(message);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        public void UpdateGUI()
        {
            try
            {
                bool flag = true;
                int pointerToMsg = 0;
                bool usedLoot = false;

                int ptrTocurrentPointer = Memory.ReadInt32(Pointer + 36);
                int currentPointer = Memory.ReadInt32(ptrTocurrentPointer + 16);

                int firstPointer = FirstPointer;

                if (currentPointer != firstPointer)
                {
                    if (LastMsgPointer == 0)
                    {
                        usedLoot = true;
                        pointerToMsg = Memory.ReadInt32(ptrTocurrentPointer + 16);
                    }
                    else
                    {
                        pointerToMsg = LastMsgPointer;
                        flag = false;
                    }
                }
                while (pointerToMsg != 0)
                {
                    if (!flag)
                    {
                        flag = true;
                        pointerToMsg = Memory.ReadInt32(pointerToMsg + 92);
                    }
                    if (pointerToMsg != 0)
                    {
                        GuiMessage message = new GuiMessage();
                        message.Pointer = Memory.ReadInt32(pointerToMsg + 76);
                        message.Type = Memory.ReadInt32(pointerToMsg + 4);
                        message.Time = Memory.ReadString(pointerToMsg + 8);
                        message.Sender = Memory.ReadString(pointerToMsg + 20);
                        message.Text = Memory.ReadString(message.Pointer);
                        message.Tick = (double)DateTime.Now.Ticks;
                        message.UsedLoot = usedLoot;
                        Messages.Add(message);

                        LastMsgPointer = pointerToMsg;
                        pointerToMsg = Memory.ReadInt32(pointerToMsg + 92);
                    }
                }

                int currentPtr = Memory.ReadInt32(ptrTocurrentPointer + 20);

                if (LastMsgPointer != currentPtr)
                {
                    LastMsgPointer = 0;
                    Messages.Clear();
                }
                    
                
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

    public class GuiChannels
    {
        #region Singleton

        private static GuiChannels _instance = new GuiChannels();

        public static GuiChannels Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private readonly object _lock = new object();
        private string _chatText;
        private int _positionX;
        private int _positionY;
        private bool _private;
        private bool _advanced;
        private bool _raid;
        private bool _safeBank;
        private bool _healCreature;
        private bool _playerPk;
        private bool _unjust;
        private bool _yourAttack;
        private Collection<GuiChannel> _channelList = new Collection<GuiChannel>();

        #endregion

        #region Properties

        public string ChatText
        {
            get
            {
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                int ptrTopointerToMsg = Memory.ReadInt32(gp + 64);
                int pointerToMsg = Memory.ReadInt32(ptrTopointerToMsg + 68);
                pointerToMsg = Memory.ReadInt32(pointerToMsg + 44);
                _chatText = Memory.ReadString(pointerToMsg);
                return _chatText;
            }
            set { _chatText = value; }
        }
        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public bool Private
        {
            get { return _private; }
            set { _private = value; }
        }
        public bool Advanced
        {
            get { return _advanced; }
            set { _advanced = value; }
        }
        public bool Raid
        {
            get { return _raid; }
            set { _raid = value; }
        }
        public bool SafeBank
        {
            get { return _safeBank; }
            set { _safeBank = value; }
        }
        public bool HealCreature
        {
            get { return _healCreature; }
            set { _healCreature = value; }
        }
        public bool PlayerPK
        {
            get { return _playerPk; }
            set { _playerPk = value; }
        }
        public bool Unjust
        {
            get { return _unjust; }
            set { _unjust = value; }
        }
        public bool YourAttack
        {
            get { return _yourAttack; }
            set { _yourAttack = value; }
        }
        public Collection<GuiChannel> ChannelList
        {
            get { return _channelList; }
        }

        #endregion

        #region Constructor

        public GuiChannels()
        {

        }

        #endregion

        #region Methods

        public Collection<GuiChannel> GetChannels()
        {
            try
            {
                UpdateGUI();

                foreach (GuiChannel channel in ChannelList)
                    channel.UpdateGUI();

                return ChannelList;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public void UpdateGUI()
        {
            try
            {
                lock (_lock)
                {
                    TurnClosedAllChannels();

                    int gp = Memory.ReadInt32(Addresses.Client.GUI); //num
                    PositionX = 0;
                    PositionY = 0;
                    PositionX += Memory.ReadInt32(gp + 20);
                    PositionY += Memory.ReadInt32(gp + 24);

                    int pointerToPosition = Memory.ReadInt32(gp + 64); //num2
                    PositionX += Memory.ReadInt32(pointerToPosition + 20);
                    PositionY += Memory.ReadInt32(pointerToPosition + 24);

                    int pointerToChatText = Memory.ReadInt32(pointerToPosition + 68); //num3
                    pointerToChatText = Memory.ReadInt32(pointerToChatText + 44);
                    ChatText = Memory.ReadString(pointerToChatText);

                    int pointerToChannels = Memory.ReadInt32(pointerToPosition + 48); //num4

                    for (int i = Memory.ReadInt32(pointerToChannels + 36); i != 0; i = Memory.ReadInt32(i + 16))
                    {
                        int pointer = i;
                        int pointerToText = Memory.ReadInt32(i + 48);
                        string text = Memory.ReadString(pointerToText);
                        bool selected = (Memory.ReadInt32(i + 68) == 1);
                        int maxLines = Memory.ReadInt32(i + 40);

                        bool channelAlreadFound = false;
                        GuiChannel channel = new GuiChannel();
                        if (ChannelList.Count > 0)
                            foreach (GuiChannel channelTemp in ChannelList)
                            {
                                if (channelTemp.Name == text)
                                {
                                    channel = channelTemp;
                                    channelAlreadFound = true;
                                    break;
                                }
                            }

                        channel.Name = text;
                        channel.Pointer = pointer;
                        channel.Selected = selected;
                        channel.MaxLines = maxLines;
                        channel.Open = true;
                        channel.UpdateGUI();

                        if (!channelAlreadFound)
                            ChannelList.Add(channel);
                    }
                    RemoveClosedChannels();
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        public int FindChannelByName(string channelName)
        {
            try
            {
                int result;
                lock(_lock)
                {
                    int index = 0;
                    foreach (GuiChannel current in ChannelList)
                    {
                        if (current.Name.Trim().ToLower() == channelName.Trim().ToLower())
                        {
                            result = index;
                            return result;
                        }
                        index++;
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return -1;
        }

        private void TurnClosedAllChannels()
        {
            try
            {
                foreach (GuiChannel channel in ChannelList)
                {
                    channel.Open = false;
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void RemoveClosedChannels()
        {
            try
            {
                Collection<GuiChannel> list = new Collection<GuiChannel>();
                foreach (GuiChannel channel in this.ChannelList)
                {
                    if (!channel.Open)
                    {
                        list.Add(channel);
                    }
                }
                foreach (GuiChannel current in list)
                {
                    ChannelList.Remove(current);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

    public class GuiEquipment
    {
        #region Singleton

        private static GuiEquipment _instance = new GuiEquipment();

        public static GuiEquipment Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private int _positionX;
        private int _positionY;
        private int _width;
        private int _height;
        private bool _minimized;
        private int _fightMode;
        private int _chaseMode;
        private bool _safeMode;

        #endregion

        #region Properties

        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public bool Minimized
        {
            get { return _minimized; }
            set { _minimized = value; }
        }
        public int FightMode
        {
            get { return _fightMode; }
            set { _fightMode = value; }
        }
        public int ChaseMode
        {
            get { return _chaseMode; }
            set { _chaseMode = value; }
        }
        public bool SafeMode
        {
            get { return _safeMode; }
            set { _safeMode = value; }
        }
        public Point MinimiseButton
        {
            get
            {
                return new Point(PositionX + 14, PositionY + 10);
            }
        }
        public Point AmuletSlot
        {
            get
            {
                return new Point(PositionX + 25, PositionY + 35);
            }
        }
        public Point WeaponSlot
        {
            get
            {
                return new Point(PositionX + 25, PositionY + 73);
            }
        }
        public Point RingSlot
        {
            get
            {
                return new Point(PositionX + 25, PositionY + 111);
            }
        }
        public Point HelmetSlot
        {
            get
            {
                return new Point(PositionX + 63, PositionY + 0x10);
            }
        }
        public Point ArmorSlot
        {
            get
            {
                return new Point(PositionX + 63, PositionY + 54);
            }
        }
        public Point LegsSlot
        {
            get
            {
                return new Point(PositionX + 63, PositionY + 92);
            }
        }
        public Point BootsSlot
        {
            get
            {
                return new Point(PositionX + 63, PositionY + 130);
            }
        }
        public Point BackPackSlot
        {
            get
            {
                return new Point(PositionX + 101, PositionY + 35);
            }
        }
        public Point ShieldSlot
        {
            get
            {
                return new Point(PositionX + 101, PositionY + 73);
            }
        }
        public Point ArrowSlot
        {
            get
            {
                return new Point(PositionX + 101, PositionY + 111);
            }
        }
        public Point StopButton
        {
            get
            {
                return new Point(PositionX + 147, PositionY + 88);
            }
        }
        public Point OffensiveButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 67, PositionY + 13);
                }
                else
                {
                    result = new Point(PositionX + 135, PositionY + 0x1C);
                }
                return result;
            }
        }
        public Point BalancedButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 87, PositionY + 13);
                }
                else
                {
                    result = new Point(PositionX + 135, PositionY + 0x30);
                }
                return result;
            }
        }
        public Point DefensiveButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 107, PositionY + 13);
                }
                else
                {
                    result = new Point(PositionX + 135, PositionY + 0x44);
                }
                return result;
            }
        }
        public Point StandingButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 67, PositionY + 35);
                }
                else
                {
                    result = new Point(PositionX + 157, PositionY + 0x1C);
                }
                return result;
            }
        }
        public Point ChaseButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 87, PositionY + 35);
                }
                else
                {
                    result = new Point(PositionX + 157, PositionY + 0x30);
                }
                return result;
            }
        }
        public Point SafeButton
        {
            get
            {
                Point result;
                if (Minimized)
                {
                    result = new Point(PositionX + 107, PositionY + 35);
                }
                else
                {
                    result = new Point(PositionX + 157, PositionY + 0x44);
                }
                return result;
            }
        }

        #endregion

        #region Methods

        public void UpdateGUI()
        {
            try
            {
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                PositionX = 0;
                PositionY = 0;
                PositionX += Memory.ReadInt32(gp + 0x14);
                PositionY += Memory.ReadInt32(gp + 0x18);

                int pointerToPosition = Memory.ReadInt32(gp + 0x38);

                PositionX += Memory.ReadInt32(pointerToPosition + 0x14);
                PositionY += Memory.ReadInt32(pointerToPosition + 0x18);

                int basePointer = Memory.ReadInt32(pointerToPosition + 0x24);

                for (int window = 0; window < 4; window++)
                {
                    int windowId = Memory.ReadInt32(basePointer + 0x2C);

                    if (windowId == 0x1 || windowId == 0x2)
                    {
                        PositionX += Memory.ReadInt32(basePointer + 0x14);
                        PositionY += Memory.ReadInt32(basePointer + 0x18);
                        Width = Memory.ReadInt32(basePointer + 0x1C);
                        Height = Memory.ReadInt32(basePointer + 0x20);
                        Minimized = (windowId == 1) ? false : true;
                        FightMode = Client.Instance.AttackMode;
                        ChaseMode = Client.Instance.ChaseType;

                        int pointerToPtrSafe = Memory.ReadInt32(basePointer + 0x24);

                        int pointerToSafe = Memory.ReadInt32(pointerToPtrSafe + 0x40);
                        SafeMode = (Memory.ReadInt32(pointerToSafe + 0x48) == 0);

                    }
                    basePointer = Memory.ReadInt32(basePointer + 0x10);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

    public class GuiGameScreen
    {
        #region Singleton

        private static GuiGameScreen _instance = new GuiGameScreen();

        public static GuiGameScreen Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private int _positionX = 0;
        private int _positionY = 0;
        private int _width;
        private int _height;

        #endregion

        #region Properties

        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public int TileWidth
        {
            get { return (int)(Width / 15);}
        }
        public int TileHeight
        {
            get { return (int)(Height / 11); }
        }

        #endregion

        #region Methods

        public void UpdateGUI()
        {
            int gp = Memory.ReadInt32(Addresses.Client.GUI);
            int pointerToPosition = Memory.ReadInt32(gp + 0x30);
            PositionX = 0;
            PositionY = 0;
            PositionX = Memory.ReadInt32(pointerToPosition + 0x34);
            PositionY = Memory.ReadInt32(pointerToPosition + 0x30);
            Width = Memory.ReadInt32(pointerToPosition + 0x38);
            Height = Memory.ReadInt32(pointerToPosition + 0x3C);
        }

        public Point GetPointFromLocation(Location location)
        {
            return new Point(GetXfromLoc(location.X), GetYfromLoc(location.Y));
        }

        public int GetTileOffsetX(int x, bool AbsoluteX)
        {
            return GetTileScreenX(x + 8, AbsoluteX);
        }

        public int GetTileOffsetY(int y, bool AbsoluteY)
        {
            return GetTileScreenY(y + 6, AbsoluteY);
        }

        public int GetTileScreenX(int x, bool AbsoluteX)
        {
            UpdateGUI();

            double tilesize;
            tilesize = Width / 15;
            double xres = (tilesize * x) - (tilesize / 2);
            if (AbsoluteX)
            {
                xres += PositionX;
                return (int)(xres);
            }
            return (int)xres;
        }

        public int GetTileScreenY(int y, bool AbsoluteY)
        {
            UpdateGUI();

            double tilesize;
            tilesize = Height / 11;
            double xres = (tilesize * y) - (tilesize / 2);
            if (AbsoluteY)
            {
                xres += PositionY;
                return (int)xres;
            }
            return (int)xres;
        }

        public int GetXfromLoc(int x)
        {
            UpdateGUI();

            int playerx = (int)Player.X;
            int res;
            if (playerx > x)
            {
                res = x - playerx;
                return GetTileOffsetX(res, true);
            }

            if (playerx < x)
            {
                res = x - playerx;
                return GetTileOffsetX(res, true);
            }

            if (playerx == x)
            {
                return GetTileOffsetX(0, true);
            }

            return 0;
        }

        public int GetYfromLoc(int y)
        {
            UpdateGUI();

            int playery = (int)Player.Y;
            int res;
            if (playery > y)
            {
                res = y - playery;
                return GetTileOffsetY(res, true);
            }

            if (playery < y)
            {
                res = y - playery;
                return GetTileOffsetY(res, true);
            }

            if (playery == y)
            {
                return GetTileOffsetY(0, true);
            }

            return 0;
        }

        #endregion
    }

    public class GuiMessage
    {
        #region Fields

        private int _type;
        private string _time;
        private string _sender;
        private string _text;
        private double _tick;
        private int _pointer;
        private bool _usedLoot;

        #endregion

        #region Properties

        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public double Tick
        {
            get { return _tick; }
            set { _tick = value; }
        }
        public int Pointer
        {
            get { return _pointer; }
            set { _pointer = value; }
        }
        public bool UsedLoot
        {
            get { return _usedLoot; }
            set { _usedLoot = value; }
        }

        #endregion

        #region Constructor

        public GuiMessage()
        {
            Type = -1;
            Time = "";
            Text = "";
            Tick = 0.0;
            Pointer = 0;
            Sender = "";
            UsedLoot = false;
        }

        #endregion
    }

    public class GuiMinimap
    {
        #region Singleton

        private static GuiMinimap _instance = new GuiMinimap();

        public static GuiMinimap Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private int _positionX;
        private int _positionY;
        private int _width;
        private int _height;
        private int _zoom;
        private bool _center;
        private int _xPos;
        private int _yPos;
        private int _zPos;

        #endregion

        #region Properties

        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public int Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        public bool Center
        {
            get { return _center; }
            set { _center = value; }
        }
        public int XPos
        {
            get { return _xPos; }
            set { _xPos = value; }
        }
        public int YPos
        {
            get { return _yPos; }
            set { _yPos = value; }
        }
        public int ZPos
        {
            get { return _zPos; }
            set { _zPos = value; }
        }
        public Point PlusZoomButton
        {
            get
            {
                return new Point(PositionX + 122, PositionY + 74);
            }
        }
        public Point MinusZoomButton
        {
            get
            {
                return new Point(PositionX + 122, PositionY + 54);
            }
        }
        public Point CentreZoomButton
        {
            get
            {
                return new Point(PositionX + 135, PositionY + 95);
            }
        }

        #endregion

        #region Methods 

        public void UpdateGUI()
        {
            try
            {
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                PositionX = 9;
                PositionY = 5;
                PositionX += Memory.ReadInt32(gp + 0x14);
                PositionY += Memory.ReadInt32(gp + 0x18);

                int pointerToPosition = Memory.ReadInt32(gp + 0x38);

                PositionX += Memory.ReadInt32(pointerToPosition + 0x14);
                PositionY += Memory.ReadInt32(pointerToPosition + 0x18);

                int basePointer = Memory.ReadInt32(pointerToPosition + 0x24);

                for (int window = 0; window < 20; window++)
                {
                    int windowId = Memory.ReadInt32(basePointer + 0x2C);

                    if (windowId == 0x3)
                    {

                        PositionX += Memory.ReadInt32(basePointer + 0x14);
                        PositionY += Memory.ReadInt32(basePointer + 0x18);
                        Width = Memory.ReadInt32(basePointer + 0x1C);
                        Height = Memory.ReadInt32(basePointer + 0x20);
                        Zoom = Memory.ReadInt32(basePointer + 0x7C);
                        XPos = Memory.ReadInt32(basePointer + 0x64);
                        YPos = Memory.ReadInt32(basePointer + 0x68);
                        ZPos = Memory.ReadInt32(basePointer + 0x6C);

                        int isCentered = Memory.ReadInt32(basePointer + 0x61);
                        Center = (isCentered != 0);

                        break;
                    }
                    basePointer = Memory.ReadInt32(basePointer + 0x10);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

    public class GuiTrade
    {
        #region Singleton

        private static GuiTrade _instance = new GuiTrade();

        public static GuiTrade Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields
        private readonly object _lock = new object();
        private bool _isOpened = false;
        private bool _isMinimized = false;
        private int _positionX = 0;
        private int _positionY = 0;
        private int _width;
        private int _height;
        private Collection<TradeItem> _tradeItems = new Collection<TradeItem>();
        private int _itemListPositionX;
        private int _itemListPositionY;
        private int _itemListHeight;
        private int _itemListWidth;
        private int _typeItems;
        private int _selectedItem;
        private int _heightBar;
        private int _positionBar;
        private int _visibleArea;
        private int _valor;
        private int _tradeType;
        private int _itemsAmount;
        private int _heightAmountBar;
        private int _valorAmountBar;
        private int _maxAmount;
        private int _maxItems;
        private int _playerMoney;

        #endregion

        #region Properties

        public bool IsOpened
        {
            get { return _isOpened; }
            set { _isOpened = value; }
        }
        public bool IsMinimized
        {
            get { return _isMinimized; }
            set { _isMinimized = value; }
        }
        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }
        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public Point BuyButton
        {
            get
            {
                return new Point(this.PositionX + 0x64, PositionY + 30);
            }
        }
        public Point SellButton
        {
            get
            {
                return new Point(this.PositionX + 150, PositionY + 30);
            }
        }
        public Point MinimiseButton
        {
            get
            {
                return new Point(PositionX + 154, PositionY + 8);
            }
        }
        public Point CloseButton
        {
            get
            {
                return new Point(this.PositionX + 166, PositionY + 8);
            }
        }
        public Point OKButton
        {
            get
            {
                return new Point(this.PositionX + 150, PositionY + Height - 15);
            }
        }
        public Point Bottom
        {
            get
            {
                return new Point(this.PositionX + 85, PositionY + Height - 2);
            }
        }
        public Point ScrollUpButton
        {
            get
            {
                return new Point(this.PositionX + 166, PositionY + 0x34);
            }
        }
        public Point ScrollDownButton
        {
            get
            {
                return new Point(PositionX + 166, PositionY + Height - 78);
            }
        }
        public Point ScrollLeftAmountButton
        {
            get
            {
                return new Point(PositionX + 14, PositionY + Height - 60);
            }
        }
        public Point ScrollRightAmountButton
        {
            get
            {
                return new Point(PositionX + 116, PositionY + this.Height - 60);
            }
        }

        public Collection<TradeItem> TradeItems
        {
            get { return _tradeItems; }
        }
        public int ItemListPositionX
        {
            get { return _itemListPositionX; }
            set { _itemListPositionX = value; }
        }
        public int ItemListPositionY
        {
            get { return _itemListPositionY; }
            set { _itemListPositionY = value; }
        }
        public int ItemListHeight
        {
            get { return _itemListHeight; }
            set { _itemListHeight = value; }
        }
        public int ItemListWidth
        {
            get { return _itemListWidth; }
            set { _itemListWidth = value; }
        }
        public int TypeItems
        {
            get { return _typeItems; }
            set { _typeItems = value; }
        }
        public int SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; }
        }
        public int HeightBar
        {
            get { return _heightBar; }
            set { _heightBar = value; }
        }
        public int PositionBar
        {
            get { return _positionBar; }
            set { _positionBar = value; }
        }
        public int VisibleArea
        {
            get { return _visibleArea; }
            set { _visibleArea = value; }
        }
        public int Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }
        public int TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }
        public int ItemsAmount
        {
            get { return _itemsAmount; }
            set { _itemsAmount = value; }
        }
        public int HeightAmountBar
        {
            get { return _heightAmountBar; }
            set { _heightAmountBar = value; }
        }
        public int ValorAmountBar
        {
            get { return _valorAmountBar; }
            set { _valorAmountBar = value; }
        }
        public int MaxAmount
        {
            get { return _maxAmount; }
            set { _maxAmount = value; }
        }
        public int MaxItems
        {
            get { return _maxItems; }
            set { _maxItems = value; }
        }
        public int PlayerMoney
        {
            get { return _playerMoney; }
            set { _playerMoney = value; }
        }

        #endregion

        #region Methods

        public void UpdateGUI()
        {
            try
            {
                bool foundTrade = false;
                PositionX = 0;
                PositionY = 0;
                int gp = Memory.ReadInt32(Addresses.Client.GUI);
                PositionX += Memory.ReadInt32(gp + 0x14);
                PositionY += Memory.ReadInt32(gp + 0x18);

                int pointerToPosition = Memory.ReadInt32(gp + 0x38);

                PositionX += Memory.ReadInt32(pointerToPosition + 0x14);
                PositionY += Memory.ReadInt32(pointerToPosition + 0x18);

                int basePointer = Memory.ReadInt32(pointerToPosition + 0x24);

                for (int window = 0; window < 20; window++)
                {
                    if (foundTrade)
                        return;

                    int windowId = Memory.ReadInt32(basePointer + 0x2C);

                    #region Reading info about trade window

                    if (windowId == 0xA)
                    {
                        foundTrade = true;

                        IsOpened = true;
                        PositionX += Memory.ReadInt32(basePointer + 0x14);
                        PositionY += Memory.ReadInt32(basePointer + 0x18);
                        Width = Memory.ReadInt32(basePointer + 0x1C);
                        Height = Memory.ReadInt32(basePointer + 0x20);
                        IsMinimized = (Memory.ReadInt32(basePointer + 0x64) == 1);

                        int pointerToListBox = Memory.ReadInt32(basePointer + 0x44);
                        ItemListPositionX = PositionX + Memory.ReadInt32(pointerToListBox + 0x14);
                        ItemListPositionY = PositionY + Memory.ReadInt32(pointerToListBox + 0x18);
                        ItemListWidth = PositionX + Memory.ReadInt32(basePointer + 0x1C);
                        ItemListHeight = PositionX + Memory.ReadInt32(basePointer + 0x20);

                        int pointerUnknown01 = Memory.ReadInt32(pointerToListBox + 0x34);
                        int pointerUnknown02 = Memory.ReadInt32(pointerUnknown01 + 0x24);
                        int pointerUnknown03 = Memory.ReadInt32(pointerUnknown02 + 0x24);

                        TypeItems = Memory.ReadInt32(pointerUnknown02 + 0x2C);
                        SelectedItem = Memory.ReadInt32(pointerUnknown02 + 0x54);

                        int pointerToBars = Memory.ReadInt32(pointerUnknown03 + 0x24);
                        HeightBar = Memory.ReadInt32(pointerToBars + 0x2C);
                        Valor = Memory.ReadInt32(pointerToBars + 0x30);
                        PositionBar = Memory.ReadInt32(pointerToBars + 0x34);
                        VisibleArea = Memory.ReadInt32(pointerToBars + 0x3C);

                        int pointerToTradeType = Memory.ReadInt32(pointerToListBox + 0x30);
                        TradeType = Memory.ReadInt32(pointerToTradeType + 0x34);

                        int pointerUnknown04 = Memory.ReadInt32(pointerToListBox + 0x38);
                        int pointerUnknown05 = Memory.ReadInt32(pointerUnknown04 + 0x34);
                        int pointerUnknown06 = Memory.ReadInt32(pointerUnknown05 + 0x24);

                        ItemsAmount = Memory.ReadInt32(pointerUnknown06 + 0x30);
                        HeightAmountBar = Memory.ReadInt32(pointerUnknown06 + 0x40);
                        ValorAmountBar = Memory.ReadInt32(pointerUnknown06 + 0x44);
                        MaxAmount = Memory.ReadInt32(pointerUnknown06 + 0x2C);

                        int pointerUnknown07 = Memory.ReadInt32(pointerToListBox + 0x38);
                        int pointerUnknown08 = Memory.ReadInt32(pointerUnknown07 + 0x10);
                        int pointerUnknown09 = Memory.ReadInt32(pointerUnknown08 + 0x38);

                        MaxItems = Memory.ReadInt32(pointerUnknown08 + 0x34);
                        PlayerMoney = Memory.ReadInt32(pointerUnknown08 + 0x3C);

                        TradeItems.Clear();

                        for (int j = 0; j < MaxItems; j++)
                        {
                            TradeItem tradeItem = new TradeItem();
                            tradeItem.Id = Memory.ReadInt32(pointerUnknown09 + j * 0x40);
                            tradeItem.Name = Memory.ReadString(pointerUnknown09 + 8 + j * 0x40);
                            tradeItem.Weight = Memory.ReadInt32(pointerUnknown09 + 0x2C + j * 0x40);
                            tradeItem.Buy = Memory.ReadInt32(pointerUnknown09 + 0x30 + j * 0x40);
                            tradeItem.Sell = Memory.ReadInt32(pointerUnknown09 + 0x34 + j * 0x40);
                            tradeItem.Qtd = Memory.ReadInt32(pointerUnknown09 + 0x38 + j * 0x40);
                            tradeItem.Index = Memory.ReadInt32(pointerUnknown09 + 0x3C + j * 0x40);
                            if (tradeItem.Index != 1)
                                TradeItems.Add(tradeItem);
                        }
                        break;
                    }

                    #endregion

                    basePointer = Memory.ReadInt32(basePointer + 0x10);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        public Collection<TradeItem> GetTradeItems()
        {
            try
            {
                UpdateGUI();
                return TradeItems;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public TradeItem FindItem(string item)
        {
            TradeItem tradeItem = new TradeItem();
            tradeItem.Id = -1;
            int id = -1;

            if (!int.TryParse(item, out id))
                id = TradeItems.Where(i => i.Name == item).First().Id;

            UpdateGUI();

            try
            {
                lock (_lock)
                {
                    foreach (TradeItem titem in TradeItems)
                        if (titem.Id == id || titem.Name.ToLower().Trim() == item.ToLower().Trim())
                            return titem;
                    return null;

                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public Point Item(int index)
        {
            Random random = new Random();
            UpdateGUI();
            Point result = new Point(PositionX + random.Next(20, 120), -1);

            try
            {
                int startY = PositionY + 0x34;
                int tradeItemHeight = 14;
                int x = (ItemListHeight - 119) / 14;

                while (IsOpened)
                {
                    UpdateGUI();

                    if (index >= Valor && index <= Valor + x - 1)
                    {
                        result.Y = startY + (index - Valor) * tradeItemHeight;
                        break;
                    }

                    if (index > Valor + x - 1)
                        InputControl.Instance.LeftClickPoint(ScrollDownButton);
                    if (index < Valor)
                        InputControl.Instance.LeftClickPoint(ScrollUpButton);

                    Thread.Sleep(100);
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return result;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public bool SelectItem(int id, int index)
        {
            try
            {
                bool result = false;
                Point point = Item(index);
                InputControl.Instance.LeftClickPoint(point);
                this.UpdateGUI();
                if (index == SelectedItem)
                {
                    result = true;
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void SetAmount(int value)
        {
            try
            {
                if (value > MaxAmount)
                    value = MaxAmount;
                int i = 0;

                while (i < 200)
                {
                    i++;
                    UpdateGUI();
                    if (ItemsAmount == value)
                        break;
                    if (ItemsAmount < value)
                        InputControl.Instance.LeftClickPoint(ScrollRightAmountButton);
                    if (ItemsAmount > value)
                        InputControl.Instance.LeftClickPoint(ScrollLeftAmountButton);
                    Thread.Sleep(15);
                }
            }
            catch (InvalidCastException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void SetBuy()
        {
            try
            {
                UpdateGUI();
                while (TradeType != 1 && IsOpened)
                {
                    InputControl.Instance.LeftClickPoint(BuyButton);
                    Thread.Sleep(300);
                    UpdateGUI();
                }
            }
            catch (InvalidCastException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void SetSell()
        {
            try
            {
                UpdateGUI();
                while (TradeType != 0 && IsOpened)
                {
                    InputControl.Instance.LeftClickPoint(SellButton);
                    Thread.Sleep(300);
                    UpdateGUI();
                }
            }
            catch (InvalidCastException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void BuyItems(string item, int value)
        {
            try
            {
                if (value > 0)
                {
                    if (value > 100)
                        value = 100;

                    if (IsOpened)
                    {
                        SetBuy();
                        TradeItem tradeItem = FindItem(item);
                        if (tradeItem.Id != -1 && SelectItem(tradeItem.Id, tradeItem.Index) && IsOpened)
                        {
                            int ratio = PlayerMoney / tradeItem.Buy;
                            if (ratio <= 100)
                            {
                                SetAmount(value);
                                InputControl.Instance.LeftClickPoint(OKButton);
                                Thread.Sleep(300);
                                UpdateGUI();
                            }
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void SellItems(string item, int value)
        {
            try
            {
                if (value > 0)
                {
                    if (value > 100)
                        value = 100;

                    if (IsOpened)
                    {
                        SetSell();
                        TradeItem tradeItem = FindItem(item);
                        if (tradeItem.Id != -1 && tradeItem.Qtd > 0 && SelectItem(tradeItem.Id, tradeItem.Index) && IsOpened)
                        {
                            SetAmount(value);
                            InputControl.Instance.LeftClickPoint(OKButton);
                            Thread.Sleep(300);
                            UpdateGUI();
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }

}
