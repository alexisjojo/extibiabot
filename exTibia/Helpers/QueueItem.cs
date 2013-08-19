using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public class QueueItem
    {
        #region Fields

        private Action _action;
        private bool _running = false;
        private bool _async = false;
        private int _lifetime;
        private long _timeadded;
        private bool _overwrite;
        private Task _task;
        #endregion

        #region Properties

        public int Lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }
        public Action Action
        {
            get { return _action; }
            set { _action = value; }
        }
        public bool Overwrite
        {
            get { return _overwrite; }
            set { _overwrite = value; }
        }
        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }
        public bool Async
        {
            get { return _async; }
            set { _async = value; }
        }

        public Task TaskType
        {
            get { return _task; }
            set { _task = value; }
        }

        public bool IsAlive
        {
            get
            {
                return (TimeSpan.FromTicks(DateTime.Now.Ticks - _timeadded).TotalMilliseconds < _lifetime);
            }
        }

        #endregion

        #region Constructor

        public QueueItem(Action action, int lifetime)
        {
            this._action = action;
            this._lifetime = lifetime;
            this._timeadded = DateTime.Now.Ticks; 
        }

        public QueueItem(Action action, int lifetime, bool async)
        {
            this._action = action;
            this._lifetime = lifetime;
            this._async = async;
            this._timeadded = DateTime.Now.Ticks; 
        }

        public QueueItem(Action action, int lifetime, bool async, bool overwrite)
        {
            this._action = action;
            this._lifetime = lifetime;
            this._async = async;
            this._timeadded = DateTime.Now.Ticks;
            this._overwrite = overwrite;
        }

        public QueueItem(Task task, Action action, int lifetime, bool async, bool overwrite)
        {
            this._action = action;
            this._lifetime = lifetime;
            this._async = async;
            this._timeadded = DateTime.Now.Ticks;
            this._overwrite = overwrite;
            this._task = task;
        }

        #endregion
    }
}
