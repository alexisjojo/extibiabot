using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Worker
    {
        #region Singleton

        private static Worker _instance = new Worker();

        public static Worker Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private Thread _workerThreadSynchrous;
        private Thread _workerThreadAsynchrous;
        private bool _exitSynchrous = false;

        #endregion

        #region Properties

        public bool ExitSynchrous
        {
            get { return _exitSynchrous; }
            set { _exitSynchrous = value; }
        }

        #endregion

        #region Constructor

        public Worker()
        {
            _workerThreadSynchrous = new Thread(RunSynchrously);
            _workerThreadAsynchrous = new Thread(RunAsynchrously);

            _workerThreadSynchrous.SetApartmentState(ApartmentState.STA);
            _workerThreadAsynchrous.SetApartmentState(ApartmentState.STA);

            _workerThreadSynchrous.Start();
            _workerThreadAsynchrous.Start();
        }

        #endregion

        #region Methods

        public void Init()
        {
            Debug.WriteLine("Instance of Worker() has been created.", ConsoleColor.Yellow);
        }

        public void RunAsynchrously()
        {
            {
            execute:
                try
                {
                    new Thread(() =>
                    {
                        KeyValuePair<int, QueueItem> dequeued;
                        IAsyncResult asyncResult;

                        while (true)
                        {
                            dequeued = new KeyValuePair<int, QueueItem>();

                            if (Queues.QueueASync.TryDequeue(out dequeued))
                            {
                                if (dequeued.Value == null)
                                    continue;

                                if (!dequeued.Value.IsAlive)
                                {
                                    Debug.WriteLine(String.Format("Dequeued task is not more active. Skipping..."), ConsoleColor.Red);
                                    continue;
                                }

                                Debug.WriteLine(String.Format("Dequeued asynchrous task - Priority: {0}  Method: {1}", 100 - dequeued.Key, dequeued.Value.TaskType), ConsoleColor.White);
                                Debug.WriteLine(String.Format("Executing dequeued task asynchrously. Task: {0}", dequeued.Value.TaskType), ConsoleColor.White);

                                asyncResult = dequeued.Value.Action.BeginInvoke(null, null);
                                dequeued.Value.Action.EndInvoke(asyncResult);
                            }

                            Thread.Sleep(10);
                        }
                    }).Start();
                }
                catch (InvalidOperationException ex)
                {
                    Helpers.Debug.Report(ex);
                    goto execute;
                }
            }

        }

        public void RunSynchrously()
        {
        execute:
            try
            {
                new Thread(() =>
                {
                    KeyValuePair<int, QueueItem> dequeued;
                    while (true)
                    {
                        dequeued = new KeyValuePair<int, QueueItem>();

                        if (Queues.QueueSync.TryDequeue(out dequeued))
                        {
                            if (dequeued.Value == null)
                                continue;

                            if (!dequeued.Value.IsAlive)
                            {
                                Debug.WriteLine(String.Format("Dequeued task is not more active. Skipping..."), ConsoleColor.Red);
                                continue;
                            }

                            Debug.WriteLine(String.Format("Dequeued task - Priority: {0}  Method: {1}", 100 - dequeued.Key,dequeued.Value.TaskType), ConsoleColor.White);
                            
                            Debug.WriteLine(String.Format("Executing dequeued task synchrously. Task: {0}", dequeued.Value.TaskType), ConsoleColor.White);
                            
                            dequeued.Value.Action();

                            Debug.WriteLine(String.Format("Items in the queue list: {0}",Queues.QueueSync.Count), ConsoleColor.White);

                        }

                        Thread.Sleep(10);
                    }                        
                }).Start();
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                goto execute;
            }
        }

        #endregion
    }
}
