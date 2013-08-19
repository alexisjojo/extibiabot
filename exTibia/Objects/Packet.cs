using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Objects
{
    public class Packet
    {
        #region Fields

        private PacketType _type;
        private byte[] _body;
        private int _length;

        #endregion

        #region Properties

        public PacketType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public byte[] Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        #endregion

        #region Constructors

        public Packet() { }

        public Packet(byte[] bytes)
        {
            Type = (PacketType)bytes[0];
            Length = bytes.Count();
            Body = new byte[Length];
            Array.Copy(bytes, 0, Body, 0, Length); 
        }

        #endregion
    }
}
