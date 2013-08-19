using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class PipeServer
    {
        #region Singleton

        private static PipeServer _instance = new PipeServer();

        public static PipeServer Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private NamedPipeServerStream _pipe;
        private byte[] _buffer = new byte[10000];
        private string _pipeName = "";

        #endregion

        #region Properties

        public NamedPipeServerStream Pipe
        {
            get { return _pipe; }
            set { _pipe = value; }
        }
        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }
        public string PipeName
        {
            get { return _pipeName; }
            set { _pipeName = value; }
        }

        #endregion

        #region Delegates and events

        public delegate void PipeNotification();
        public delegate void PipeListener(NetworkMessage msg);

        public event PipeNotification OnConnected;
        public event PipeListener OnReceive;

        #endregion

        #region Constructor

        public PipeServer()
        {
            PipeName = string.Format("exTibiaS{0}", GameClient.Process.Id);
            Pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            OnReceive += new PipeListener(PipeServer_OnReceive);
            Pipe.BeginWaitForConnection(new AsyncCallback(BeginWaitForConnection), null);
        }

        #endregion

        #region Methods

        void PipeServer_OnReceive(NetworkMessage msg)
        {
            Packets.Instance.LogPacket(new Packet(msg.GetBuffer()));
        }

        public void Init()
        {
            Debug.WriteLine("Instance of PipeServer() has been created.", ConsoleColor.Yellow);
        }

        private void BeginWaitForConnection(IAsyncResult ar)
        {
            Pipe.EndWaitForConnection(ar);
           
            if (Pipe.IsConnected)
            {
                if (OnConnected != null)
                    OnConnected.BeginInvoke(null, null);
                Pipe.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(BeginRead), null);
            }
        }

        private void BeginRead(IAsyncResult ar)
        {
            try
            {
                int read = Pipe.EndRead(ar);

                if (read == 0)
                    return;

                byte[] received = new byte[read];
                Array.Copy(Buffer, received, read);

                if (OnReceive != null)
                    OnReceive.BeginInvoke(new NetworkMessage(received, read), null, null);
                Pipe.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(BeginRead), null);
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion
    }
}
