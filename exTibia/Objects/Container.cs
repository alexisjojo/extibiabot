using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using System.Drawing;

namespace exTibia.Objects
{
    public class Container
    {
        #region Fields

        /* Memory part */
        private int _address;
        private int _index;
        private int _guiPointer;

        /* GUI part */
        private string _nameGui;
        private int _idGui;
        private int _positionX = 0;
        private int _positionY = 0;
        private int _width;
        private int _height;
        private int _usedSlots;
        private int _maxSlots;
        private int _heightBar;
        private int _positionBar;
        private int _visibleArea;
        private int _valor;
        private bool _isMinimized;

        #endregion

        #region Properties

        /* Memory part */
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        public int Id
        {
            get { return Memory.ReadInt32(Address + Addresses.Container.Id); }
        }
        public string Name
        {
            get { return Memory.ReadString(Address + Addresses.Container.Name); }
        }
        public int Volume
        {
            get { return Memory.ReadInt32(Address + Addresses.Container.Volume); }
        }
        public int Amount
        {
            get { return Memory.ReadInt32(Address + Addresses.Container.Amount); }
        }
        public bool IsOpen
        {
            get { return (Memory.ReadInt32(Address + Addresses.Container.IsOpen) == 1); }
        }
        public bool HasParent
        {
            get { return (Memory.ReadInt32(Address + Addresses.Container.HasParent) == 1); }
        }
        public int GuiPointer
        {
            get { return _guiPointer; }
            set { _guiPointer = value; }
        }

        /* GUI part */
        public string GuiName
        {
            get { return _nameGui; }
            set { _nameGui = value; }
        }
        public int GuiId
        {
            get { return _idGui; }
            set { _idGui = value; }
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
        public int UsedSlots
        {
            get { return _usedSlots; }
            set { _usedSlots = value; }
        }
        public int MaxSlots
        {
            get { return _maxSlots; }
            set { _maxSlots = value; }
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
        public bool IsMinimized
        {
            get { return _isMinimized; }
            set { _isMinimized = value; }
        }
        public Point CloseButton
        {
            get
            {
                return new Point(PositionX + 0xa6, this.PositionY + 8);
            }
        }
        public Point MinimiseButton
        {
            get
            {
                return new Point(PositionX + 154, this.PositionY + 9);
            }
        }
        public Point ScrollUpButton
        {
            get
            {
                return new Point(PositionX + 166, this.PositionY + 20);
            }
        }
        public Point ScrollDownButton
        {
            get
            {
                return new Point(PositionX + 166, this.PositionY + Height - 10);
            }
        }
        public Point ScrollButton
        {
            get
            {
                return new Point(PositionX + 166, this.PositionY + 36 + PositionBar);
            }
        }
        public Point Bottom
        {
            get
            {
                return new Point(PositionX + 85, this.PositionY + Height - 2);
            }
        }

        #endregion

        #region Constructors

        public Container() { }

        public Container(int address)
        {
            this.Address = address;
        }

        public Container(int address, int index)
        {
            this.Address = address;
            this.Index = index;
        }

        #endregion

        #region Methods

        public Collection<ItemContainer> GetItems()
        {
            Collection<ItemContainer> items = new Collection<ItemContainer>();
            int orderInBp = 0;
            for (int i = Address; i < Address + Addresses.Container.StepSlot * Amount; i += Addresses.Container.StepSlot)
            {
                int positionX = orderInBp % 4;
                int positionY = orderInBp / 4;

                items.Add(new ItemContainer(
                    Memory.ReadInt32(i + Addresses.Container.ItemId),
                    (Memory.ReadInt32(i + Addresses.Container.ItemCount) == 0) ? 1 : Memory.ReadInt32(i + Addresses.Container.ItemCount),
                    new ItemLocation(this, orderInBp),
                    positionX,
                    positionY));
                orderInBp++;
            }

            return items;
        }

        public Point Item(int x, int y)
        {
            try
            {
                UpdateGUI();

                Point result = new Point(PositionX + 30 + 37 * x, 0);

                int positionStartY = PositionY + 36;
                int positionYstep = 37; 
                int clickedTimes = 0;

                while ((positionYstep * y < PositionBar || positionYstep * y > PositionBar + VisibleArea - 28) && clickedTimes < 20)
                {
                    clickedTimes++;
                    if (positionYstep * y < PositionBar && positionYstep * y < PositionBar + VisibleArea - 55)
                        InputControl.Instance.LeftClickPoint(ScrollUpButton);

                    if (positionYstep * y > PositionBar + VisibleArea - 28)
                        InputControl.Instance.LeftClickPoint(ScrollDownButton);

                    UpdateGUI();
                }
                result.Y = positionStartY + positionYstep * y - PositionBar;
                return result;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return new Point();
        }

        public void UpdateGUI()
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

            for (int containerIndex = 0; containerIndex < 20; containerIndex++)
            {
                int containerId = Memory.ReadInt32(basePointer + 0x2C);
                if (containerId >= 0x40 && containerId - 0x40 == Index)
                {
                    GuiPointer = basePointer;
                    PositionX += Memory.ReadInt32(basePointer + 0x14);
                    PositionY += Memory.ReadInt32(basePointer + 0x18);
                    Width = Memory.ReadInt32(basePointer + 0x1C);
                    Height = Memory.ReadInt32(basePointer + 0x20);
                    IsMinimized = (Memory.ReadInt32(basePointer + 0x64) == 1);
                    int pointerToId = Memory.ReadInt32(basePointer + 96);
                    int pointerToName = Memory.ReadInt32(pointerToId + 36);
                    GuiName = Memory.ReadString(pointerToName);
                    GuiId = Memory.ReadInt32(pointerToId + 76);
                    int pointerToSlots = Memory.ReadInt32(basePointer + 68);
                    UsedSlots = Memory.ReadInt32(pointerToSlots + 56);
                    MaxSlots = Memory.ReadInt32(pointerToSlots + 60);
                    int pointerToPtrBars = Memory.ReadInt32(pointerToSlots + 36);
                    int PointerToBars = Memory.ReadInt32(pointerToPtrBars + 36);
                    HeightBar = Memory.ReadInt32(PointerToBars + 44);
                    PositionBar = Memory.ReadInt32(PointerToBars + 48);
                    VisibleArea = Memory.ReadInt32(PointerToBars + 52);
                    Valor = Memory.ReadInt32(PointerToBars + 60);
                    break;
                }
                basePointer = Memory.ReadInt32(basePointer + 16);
            }
        }

        public void Close()
        {
            UpdateGUI();
            InputControl.Instance.LeftClickPoint(CloseButton);
        }

        #endregion
    }
}