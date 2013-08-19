using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class Queues
    {
        #region Fields

        private static PriorityQueue _queueSync = new PriorityQueue();
        private static PriorityQueue _queueASync = new PriorityQueue();
        private static object _locker = new object();

        #endregion

        #region Properties

        public static PriorityQueue QueueSync
        {
            get { return _queueSync; }
        }

        public static PriorityQueue QueueASync
        {
            get { return _queueASync; }
        }
    
        public static object Locker
        {
            get { return Queues._locker; }
            set { Queues._locker = value; }
        }

        #endregion

        #region Methods

        public static void Add(Task task, Action action, int priority, int lifetime, bool async, bool overwrite)
        {

            KeyValuePair<int, QueueItem> item = new KeyValuePair<int, QueueItem>(priority,
                new QueueItem(task, action, lifetime, async, overwrite));

            lock (_locker)
            {
                if (async)
                    QueueASync.Enqueue(item);
                else
                    QueueSync.Enqueue(item);
            }
        }

        public static void Add(Action action, int priority, int lifetime, bool async, bool overwrite)
        {
            KeyValuePair<int, QueueItem> item = new KeyValuePair<int, QueueItem>(priority,
                new QueueItem(action, lifetime, async, overwrite));

            lock (_locker)
            {
                if (async)
                    QueueASync.Enqueue(item);
                else
                    QueueSync.Enqueue(item);
            }
        }

        public static bool HasTask(Task task)
        {

            if (QueueASync.HasTask(task))
                return true;
            else
                if (QueueSync.HasTask(task))
                    return true;
            return false;
        }

        #endregion
    }
}
