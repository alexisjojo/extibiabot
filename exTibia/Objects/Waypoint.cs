using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Objects
{
    public class Waypoint : INotifyPropertyChanged
    {

        #region Fields

        private WaypointType _waypointType;
        private Location _location;
        private string _action;
        private int _waypointId;
        private bool _active;
        private int _attemptToRun = 0;

        #endregion

        #region Properties

        public string ListBoxItem
        {
            get 
            { 
                if (Active)
                    return String.Format("-> {0:D3} {1} {2}", CaveBot.Instance.WaypointList.IndexOf(this) + 1, _waypointType, _location); 
                else
                    return String.Format("{0:D3} {1} {2}", CaveBot.Instance.WaypointList.IndexOf(this) + 1, _waypointType, _location); 
            }
        }

        public WaypointType WaypointType
        {
            get { return _waypointType; }
            set 
            { 
                _waypointType = value; 
                RaisePropertyChanged("WaypointType");
            }
        }

        public Location Location
        {
            get { return _location; }
            set 
            { 
                _location = value; 
                RaisePropertyChanged("Location");
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                RaisePropertyChanged("Active");
                RaisePropertyChanged("ListBoxItem");
            }
        }

        public string ScriptAction
        {
            get { return _action; }
            set 
            { 
                _action = value; 
                RaisePropertyChanged("ScriptAction");
            }
        }

        public int WaypointID
        {
            get { return _waypointId; }
            set 
            { 
                _waypointId = value; 
                RaisePropertyChanged("WaypointID");
            }
        }

        public int AttemptToRun
        {
            get { return _attemptToRun; }
            set 
            { 
                _attemptToRun = value; 
                RaisePropertyChanged("AttemptToRun");
            }

        }

        #endregion

        #region Constructors

        public Waypoint()
        {

        }

        public Waypoint(WaypointType waypointType, Location location, string action)
        {
            _waypointType = waypointType;
            _location = location;
            _action = action;
        }

        public Waypoint(int waypointID, WaypointType waypointType, Location location, string action)
        {
            _waypointId = waypointID;
            _waypointType = waypointType;
            _location = location;
            _action = action;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
