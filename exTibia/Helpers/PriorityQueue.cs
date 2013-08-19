using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Linq;
using System;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{

    public class PriorityQueue
    {
        #region Fields

        private List<KeyValuePair<int, QueueItem>> list = new List<KeyValuePair<int, QueueItem>>();
        private static readonly Object _locker = new Object();

        #endregion

        #region Methods

        public int Count
        {
            get { return list.Count; }
        }

        public void Enqueue(KeyValuePair<int, QueueItem> value)
        {
            lock (_locker)
            {
                CleanUp();

                list.Add(value);
            }
        }

        public bool TryDequeue(out KeyValuePair<int, QueueItem> result)
        {
            result = new KeyValuePair<int, QueueItem>();

            lock (_locker)
            {
                if (list.Count > 0)
                {
                    CleanUp();

                    result = list.OrderByDescending(i => i.Key).FirstOrDefault();

                    if (result.Value != null)
                        return true;
                    return false;  
                }
                return false;
            }
        }

        public void CleanUp()
        {
            int removed = 0;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].Value.IsAlive)
                {
                    list.RemoveAt(i);
                    removed++;
                }
            }
        }

        public bool HasTask(Task task)
        {
            lock (_locker)
            {
                return list.Where(i => i.Value.TaskType == task).Count() > 0 ? true : false;
            }
        }

        #endregion
    }
}