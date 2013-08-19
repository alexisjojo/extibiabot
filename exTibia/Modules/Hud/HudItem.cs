using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class HudItem : INotifyPropertyChanged
    {
        private string _textToPrint;
        private uint _r;
        private uint _g;
        private uint _b;
        private uint _posx;
        private uint _posy;
        private uint _font;
        private string _textName;

        public string TextToPrint
        {
            get { return _textToPrint; }
            set { _textToPrint = value; }
        }
        public uint R
        {
            get { return _r; }
            set { _r = value; }
        }
        public uint G
        {
            get { return _g; }
            set { _g = value; }
        }
        public uint B
        {
            get { return _b; }
            set { _b = value; }
        }
        public uint PosX
        {
            get { return _posx; }
            set { _posx = value; }
        }
        public uint PosY
        {
            get { return _posy; }
            set { _posy = value; }
        }
        public uint FontType
        {
            get { return _font; }
            set { _font = value; }
        }
        public string TextName
        {
            get 
            {
                OnPropertyChanged("TextName");
                return _textName; 
            }
            set 
            {
                OnPropertyChanged("TextName"); 
                _textName = value;
            }        
        }


        public HudItem()
        {

        }

        public HudItem(uint x, uint y, uint r, uint g, uint b, uint font, string textToPrint)
        {
            TextName = string.Format("{0}{1}{2}{3}{4}", x, y, r, g, b);
            R = r;
            G = g;
            B = b;
            PosX = x;
            PosY = y;
            FontType = font;
            TextToPrint = textToPrint;
        }

        public HudItem(string textName, uint x, uint y, uint r, uint g, uint b, uint font, string textToPrint)
        {
            TextName = textName;
            R = r;
            G = g;
            B = b;
            PosX = x;
            PosY = y;
            FontType = font;
            TextToPrint = textToPrint;
        }

        public NetworkMessage ToNetworkMessage()
        {
            NetworkMessage message = new NetworkMessage();
            message.AddByte(0x04);
            message.AddString(TextName);
            message.AddUInt32(PosX);
            message.AddUInt32(PosY);
            message.AddUInt32(R);
            message.AddUInt32(G);
            message.AddUInt32(B);
            message.AddUInt32(FontType);
            message.AddString(TextToPrint);
            message.Position = 8;
            return message;
        }

        #region PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
