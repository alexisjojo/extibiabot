using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exTibia.Objects
{
    public class ItemContainer : IItem
    {
        private int _itemId;
        private int _itemCount;
        private string _itemName;
        private ItemLocation _itemLocation;
        private int _indexX;
        private int _indexY;

        public int ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }
        public int ItemCount
        {
            get { return _itemCount; }
            set { _itemCount = value; }
        }
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        public ItemLocation ItemLocation
        {
            get { return _itemLocation; }
            set { _itemLocation = value; }
        }
        public int IndexX
        {
            get { return _indexX; }
            set { _indexX = value; }
        }
        public int IndexY
        {
            get { return _indexY; }
            set { _indexY = value; }
        }

        public ItemContainer() { }

        public ItemContainer(int itemId, int itemCount, ItemLocation itemLocation, int indexX, int indexY)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            ItemLocation = itemLocation;
            IndexX = indexX;
            IndexY = indexY;
        }

        public void Use() { }
        public void UseOnItem(IItem item, int amount = -1) { }
        public void UseOnMap(Location location, int amount = -1) { }
        public void UseOnMap(Tile tile, int amount = -1) { }
        public void MoveToContainer(Container containerObject, int amount = -1) { }
        public void MoveToContainer(string containerName, int amount = -1) { }
        public void MoveToContainer(int containerGuiIndex, int amount = -1) { }
        public void MoveToSlot(Slot slot, int amount = -1) { }
        public void MoveToGround(Location location, int amount = -1) { }
        public void MoveToGround(Tile tyle, int amount = -1) { }
        public void Look() { }
        
    }
}
