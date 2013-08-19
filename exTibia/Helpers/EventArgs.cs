using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public class CaveBotEventArgs : EventArgs
    {
        private Waypoint _waypoint;

        public Waypoint Waypoint
        {
            get { return _waypoint; }
            set { _waypoint = value; }
        }

        public CaveBotEventArgs() { }

        public CaveBotEventArgs(Waypoint waypoint)
        {
            this.Waypoint = waypoint;
        }

    }

    public class TargetingEventArgs : EventArgs
    {
        private TargetinRule _targetingRule;

        public TargetinRule TargetingRule
        {
            get { return _targetingRule; }
            set { _targetingRule = value; }
        }
        private Creature _creature;

        public Creature Creature
        {
            get { return _creature; }
            set { _creature = value; }
        }

        private Collection<Creature> _creatures;

        public Collection<Creature> Creatures
        {
            get { return _creatures; }
        }


        public TargetingEventArgs()
        {

        }

        public TargetingEventArgs(TargetinRule TargetingRule, Creature Creature)
        {
            _targetingRule = TargetingRule;
            _creature = Creature;
        }

        public TargetingEventArgs(TargetinRule TargetingRule, Collection<Creature> Creatures)
        {
            _targetingRule = TargetingRule;
            _creatures = Creatures;
        }

        public TargetingEventArgs(TargetinRule TargetingRule, Creature Creature, Collection<Creature> Creatures)
        {
            _targetingRule = TargetingRule;
            _creature = Creature;
            _creatures = Creatures;
        }
    }

    public class PacketReceivedEventArgs : EventArgs
    {
        private Packet _packet;

        public Packet Packet
        {
            get { return _packet; }
            set { _packet = value; }
        }

        public PacketReceivedEventArgs() { }

        public PacketReceivedEventArgs(Packet waypoint)
        {
            this._packet = waypoint;
        }

    }
}
