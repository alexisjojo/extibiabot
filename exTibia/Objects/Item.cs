using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public interface IItem
    {
        int ItemId
        {
            get;
            set;
        }
        int ItemCount
        {
            get;
            set;
        }
        string ItemName
        {
            get;
            set;
        }
        ItemLocation ItemLocation
        {
            get;
            set;
        }

        void Use();
        void UseOnItem(IItem item, int amount = -1);
        void UseOnMap(Location location, int amount = -1);
        void UseOnMap(Tile tile, int amount = -1);
        void MoveToContainer(Container containerObject, int amount = -1);
        void MoveToContainer(string containerName, int amount = -1);
        void MoveToContainer(int containerGuiIndex, int amount = -1);
        void MoveToSlot(Slot slot, int amount = -1);
        void MoveToGround(Location location, int amount = -1);
        void MoveToGround(Tile tyle, int amount = -1);
        void Look();
    }
}
