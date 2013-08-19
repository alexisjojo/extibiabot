using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class PipeClient
    {

        #region Singleton

        static readonly PipeClient _instance = new PipeClient();

        public static PipeClient Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private NamedPipeClientStream _pipe;
        private string _pipeName = "";

        #endregion

        #region Properties

        public NamedPipeClientStream Pipe
        {
            get { return _pipe; }
            set { _pipe = value; }
        }
        public string PipeName
        {
            get { return _pipeName; }
            set { _pipeName = value; }
        }

        #endregion

        #region Constructor

        public PipeClient()
        {

            PipeName = string.Format("exTibiaC{0}", GameClient.Process.Id);
            Pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
        }

        #endregion

        #region Methods

        public bool Connect()
        {
            try
            {
                Pipe.Connect(10000);
                if (Pipe.IsConnected)
                    return true;
                return false;
            }
            catch (TimeoutException te)
            {
                Helpers.Debug.Report(te);
                return false;
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
                return false;
            }
        }

        public void Write(byte[] data)
        {
            if (Pipe.IsConnected && Pipe.CanWrite)
            {
                try
                {
                    Pipe.Write(data, 0, data.Length);
                }
                catch(Exception ex)
                {
                    Helpers.Debug.Report(ex);
                }
            }
        }

        public void Send(NetworkMessage message)
        {
            Write(message.Data);
        }

        #endregion
    }
}
