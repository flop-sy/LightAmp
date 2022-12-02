#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyClient.PartyManagement;
using ZeroTier.Sockets;

#endregion

namespace BardMusicPlayer.Jamboree.PartyNetworking.Server_Client
{
    public class NetworkPartyServer : IDisposable
    {
        private static readonly Lazy<NetworkPartyServer> lazy = new(() => new NetworkPartyServer());

        private NetworkPartyServer()
        {
        }

        public static NetworkPartyServer Instance => lazy.Value;

        private SocketServer svcWorker { get; set; }

        void IDisposable.Dispose()
        {
            svcWorker.Stop();
#if DEBUG
            Console.WriteLine("Dispose Called.");
#endif
            //GC.SuppressFinalize(this);
        }

        ~NetworkPartyServer()
        {
#if DEBUG
            Console.WriteLine("Destructor Called."); // Breakpoint here
#endif
        }

        public void StartServer(IPEndPoint iPEndPoint, byte type, string name)
        {
            var objWorkerServerDiscovery = new BackgroundWorker();
            objWorkerServerDiscovery.WorkerReportsProgress = true;
            objWorkerServerDiscovery.WorkerSupportsCancellation = true;

            svcWorker = new SocketServer(ref objWorkerServerDiscovery, iPEndPoint, type, name);
            objWorkerServerDiscovery.DoWork += svcWorker.Start;
            objWorkerServerDiscovery.ProgressChanged += logWorkers_ProgressChanged;
            objWorkerServerDiscovery.RunWorkerAsync();
        }

        public void Stop()
        {
            svcWorker.Stop();
        }

        private void logWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.UserState.ToString());
        }
    }

    public class SocketServer
    {
        private readonly PartyClientInfo _clientInfo = new();
        private readonly Dictionary<string, KeyValuePair<long, ZeroTierExtendedSocket>> _pushBacklist = new();
        public bool disposing;
        public IPEndPoint iPEndPoint;
        private ZeroTierExtendedSocket listener;
        private readonly List<NetworkSocket> removed_sessions = new();
        public int ServerPort = 0;

        private readonly List<NetworkSocket> sessions = new();
        private readonly BackgroundWorker worker;

        public SocketServer(ref BackgroundWorker w, IPEndPoint localEndPoint, byte type, string name)
        {
            worker = w;
            iPEndPoint = localEndPoint;
            worker.ReportProgress(1, "Server");

            _clientInfo.Performer_Type = type;
            _clientInfo.Performer_Name = name;

            FoundClients.Instance.OnNewAddress += Instance_Finished; //Triggered if a new IP was added
        }

        public void Instance_Finished(object sender, string ip)
        {
            KeyValuePair<long, ZeroTierExtendedSocket> val;
            if (!_pushBacklist.TryGetValue(ip, out val))
                return;

            var handler = val.Value;
            if (AddClient(handler)) _pushBacklist.Remove(ip);
        }

        private bool AddClient(ZeroTierExtendedSocket handler)
        {
            var remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
            if (!FoundClients.Instance.IsIpInList(remoteIpEndPoint.Address.ToString()))
            {
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Error Ip not in list\r\n"));
                return false;
            }

            var sockets = FoundClients.Instance.FindSocket(remoteIpEndPoint.Address.ToString());
            if (sockets != null)
            {
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Session added\r\n"));
                sockets.ListenSocket = handler;
                lock (sessions)
                {
                    sessions.Add(sockets);
                }

                return true;
            }

            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Error handshake sock null\r\n"));
            return false;
            //return true;
        }

        public void Start(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(3000);
            listener = new ZeroTierExtendedSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(iPEndPoint);
            listener.Listen(10);
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Started\r\n"));

            while (disposing == false)
            {
                var da = DateTimeOffset.Now.ToUnixTimeSeconds();

                //Only accept if a autodiscover was triggered
                if (listener.Poll(100, SelectMode.SelectRead))
                {
                    //Incomming connection
                    var handler = listener.Accept();
                    var isInList = false;
                    Parallel.ForEach(sessions, session =>
                    {
                        if (session.ListenSocket == handler)
                            isInList = true;
                    });
                    if (!isInList)
                    {
                        var remoteIpEndPoint = handler.RemoteEndPoint as IPEndPoint;
                        if (!AddClient(handler))
                        {
                            var val = new KeyValuePair<long, ZeroTierExtendedSocket>(
                                DateTimeOffset.Now.ToUnixTimeSeconds(), handler);
                            _pushBacklist.Add(remoteIpEndPoint.Address.ToString(), val);
                        }
                    }
                }

                lock (sessions)
                {
                    //Update the sessions
                    foreach (var session in sessions)
                        if (!session.Update())
                            removed_sessions.Add(session);

                    //Remove dead sessions
                    foreach (var session in removed_sessions) sessions.Remove(session);
                }

                //And clear the list
                removed_sessions.Clear();

                //Keep the pushback list clean
                var delPushlist = new List<string>();
                foreach (var data in _pushBacklist)
                {
                    var val = data.Value;
                    var currtime = DateTimeOffset.Now.ToUnixTimeSeconds();

                    if (val.Key + 60 <= currtime)
                    {
                        delPushlist.Add(data.Key);
                        val.Value.Close();
                    }
                }

                lock (_pushBacklist)
                {
                    foreach (var i in delPushlist)
                        _pushBacklist.Remove(i);
                }

                var db = DateTimeOffset.Now.ToUnixTimeSeconds();
                try
                {
                    Task.Delay((int)(10 - (db - da)));
                }
                catch
                {
                }
            }

            //Finished serving - close all
            foreach (var s in sessions)
                // Release the socket.
                s.CloseConnection();

            listener.KeepAlive = false;
            try
            {
                listener.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                listener.Close();
            }

            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Stopped\r\n"));
        }

        public void SendToAll(byte[] pck)
        {
            foreach (var session in sessions)
                session.SendPacket(pck);
        }

        public void Stop()
        {
            disposing = true;
        }
    }
}