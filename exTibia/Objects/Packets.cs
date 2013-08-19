using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;
using System.Collections.ObjectModel;

namespace exTibia.Objects
{
    public class Packets
    {
        #region Singleton

        private static Packets _instance = new Packets();

        public static Packets Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Delegates & Events

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            if (PacketReceived != null)
            {
                lastRecievedPacket = e.Packet;
            }
        }

        #endregion

        #region Fields

        private Collection<Packet> _lastPackets = new Collection<Packet>();
        private Packet lastRecievedPacket = new Packet();

        #endregion

        #region Constructor

        public Packets()
        {
            PacketReceived += new EventHandler<PacketReceivedEventArgs>(Packets_PacketReceived);         
        }

        void Packets_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            lastRecievedPacket = e.Packet;         
        }

        #endregion

        #region Methods

        public void LogPacket(Packet p)
        {
            OnPacketReceived(new PacketReceivedEventArgs(p));
            if (p.Type == PacketType.ContainerOpen)
                Helpers.Debug.WriteLine("Container has been opened", ConsoleColor.Red);
            Helpers.Debug.WriteLine(string.Format("Lenght: {0} Packet: {1}", p.Length, p.Body.ToStringFast()), ConsoleColor.DarkBlue);
        }

        public bool WaitForPacket(PacketType packettype, int maxTime)
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();

            while (watcher.ElapsedMilliseconds < maxTime)
            {
                if (lastRecievedPacket.Type == packettype) return true; //quitting, and will return true
                Thread.Sleep(2);
            }
            return false;
        }

        #endregion
    }
}













        //if (System.Text.ASCIIEncoding.ASCII.GetString(packet).Contains("backpack"))
        //{
        //    lastRecievedPacket = packet;
        //}    

/*   
 * Container opened
 *   14-00  F4-07-5B-46  0E-00 6E-00   25-0B   03-00   62 61 67  08  01-01
 *   |size| | adler32 | |size| |type|  |bpid|  |size|  |b a g |  |am|                                                
 *   
 *   24 00  02-10-47-06 1A-00  6E-00   26-0B   08-00   62 61 63 6B 70 61 63 6B   14 00 04-17-0D-D2-1C-1C-E5-0D-25-0B-32-08
 *   |size| | adler32 | |size| |type|  |bpid|  |size|  |b a  c  k  p  a  c  k|  |am|
 * 
 *   24004C1012E91D00AA30A5020007004E6173747A6F72010001F57C1C7E0705007369656D
 * 
 * 	
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
