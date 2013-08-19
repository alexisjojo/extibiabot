using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Inventory
    {
        #region Singleton

        private static Inventory _instance = new Inventory();

        public static Inventory Instance
        {
            get { return _instance; }
        }

        #endregion

        public Collection<Container> GetContainers()
        {
            try
            {
                int containerID = 0;
                Collection<Container> containers = new Collection<Container>();

                for (int address = Addresses.Container.Start; address < Addresses.Container.End; address += Addresses.Container.StepContainer)
                {
                    if (Memory.ReadInt32(address + Addresses.Container.IsOpen) == 1)
                    {
                        containers.Add(new Container(address, containerID));
                        containers[containerID].UpdateGUI();
                        containerID++;
                    }
                }
                return containers;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public Container GetContainer(int index)
        {
            return new Container(Addresses.Container.Start + index * Addresses.Container.StepContainer, index);
        }

        public Collection<ItemContainer> GetContainersItems()
        {
            Collection<ItemContainer> items = new Collection<ItemContainer>();
            foreach (Container container in GetContainers())
                foreach (ItemContainer item in container.GetItems())
                    items.Add(item);
            return items;
        }
    }
}
