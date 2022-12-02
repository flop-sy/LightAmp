#region

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BardMusicPlayer.Jamboree.Events;
using ZeroTier.Sockets;

#endregion

namespace BardMusicPlayer.Jamboree.PartyNetworking
{
    /// <summary>
    ///     The autodiscover, to get the client IP and version
    /// </summary>
    internal class Autodiscover : IDisposable
    {
        private static readonly Lazy<Autodiscover> lazy = new(() => new Autodiscover());

        private Autodiscover()
        {
        }

        public static Autodiscover Instance => lazy.Value;

        private SocketRx svcRx { get; set; }

        void IDisposable.Dispose()
        {
            svcRx.Stop();
#if DEBUG
            Console.WriteLine("Dispose Called.");
#endif
            //GC.SuppressFinalize(this);
        }

        ~Autodiscover()
        {
            svcRx.Stop();
#if DEBUG
            Console.WriteLine("Destructor Called.");
#endif
        }

        public void StartAutodiscover(string address, string version)
        {
            var objWorkerServerDiscoveryRx = new BackgroundWorker();
            objWorkerServerDiscoveryRx.WorkerReportsProgress = true;
            objWorkerServerDiscoveryRx.WorkerSupportsCancellation = true;

            svcRx = new SocketRx(ref objWorkerServerDiscoveryRx, address, version);

            objWorkerServerDiscoveryRx.DoWork += svcRx.Start;
            objWorkerServerDiscoveryRx.ProgressChanged += logWorkers_ProgressChanged;
            objWorkerServerDiscoveryRx.RunWorkerAsync();
        }

        private void logWorkers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine(e.UserState.ToString());
        }

        public void Stop()
        {
            svcRx.Stop();
        }
    }

    public class SocketRx
    {
        public string Address = "";
        public string BCAddress = "";
        private readonly byte[] bytes = new byte[255];
        public bool disposing;
        public IPEndPoint iPEndPoint;
        public int ServerPort = 0;
        public string version = "";

        private readonly BackgroundWorker worker;

        public SocketRx(ref BackgroundWorker w, string address, string ver)
        {
            Address = address;
            BCAddress = address.Split('.')[0] + "." + address.Split('.')[1] + "." + address.Split('.')[2] + ".255";
            version = ver;
            worker = w;
            worker.ReportProgress(1, "Server");
        }

        public void Start(object sender, DoWorkEventArgs e)
        {
            var listener = new ZeroTierExtendedSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var transmitter =
                new ZeroTierExtendedSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var r = listener.SetBroadcast();
            r = transmitter.SetBroadcast();
            iPEndPoint = new IPEndPoint(IPAddress.Parse(BCAddress), 5555);
            listener.ReceiveTimeout = 10;
            listener.BSD_Bind(iPEndPoint);
            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[Autodiscover]: Started\r\n"));

            while (disposing == false)
            {
                var bytesRec = listener.ReceiveFrom(bytes);
                if (bytesRec > 0)
                {
                    var all = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    var f = all.Split(' ')[0]; //Get the init
                    if (f.Equals("XIVAmp"))
                    {
                        var ip = all.Split(' ')[1]; //the IP
                        var version = all.Split(' ')[2]; //the version number
                        //Add the client
                        FoundClients.Instance.Add(ip, version);
                    }
                }

                if (!disposing)
                {
                    var t = "XIVAmp " + Address + " " + version; //Send the init ip and version
                    var p = transmitter.SendTo(iPEndPoint, Encoding.ASCII.GetBytes(t));
                    Thread.Sleep(3000);
                }
            }

            try
            {
                transmitter.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                transmitter.Close();
            }

            try
            {
                listener.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                listener.Close();
            }

            BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[Autodiscover]: Stopped\r\n"));
        }

        public void Stop()
        {
            disposing = true;
        }
    }
}