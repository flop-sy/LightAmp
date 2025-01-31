﻿#region

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using BardMusicPlayer.Jamboree.Events;
using BardMusicPlayer.Jamboree.PartyClient.PartyManagement;
using ZeroTier.Sockets;
using SocketException = ZeroTier.Sockets.SocketException;

#endregion

namespace BardMusicPlayer.Jamboree.PartyNetworking;

public sealed class NetworkSocket
{
    private bool _await_pong;
    private bool _close;
    private string _remoteIP = "";

    private Timer _timer;

    public NetworkSocket(string IP)
    {
        _ = ConnectTo(IP).ConfigureAwait(false);
    }

    internal NetworkSocket(ZeroTierExtendedSocket socket)
    {
        ListenSocket = socket;
        PartyManager.Instance.Add(PartyClient);
    }

    public PartyClientInfo PartyClient { get; } = new();

    public ZeroTierExtendedSocket ListenSocket { get; set; }
    public ZeroTierExtendedSocket ConnectorSocket { get; set; }

    public async Task<bool> ConnectTo(string IP)
    {
        _remoteIP = IP;
        var localEndPoint = new IPEndPoint(IPAddress.Parse(IP), 12345);
        var bytes = new byte[1024];
        ConnectorSocket =
            new ZeroTierExtendedSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect to the server
        ConnectorSocket.Connect(localEndPoint);
        //Wait til connected
        while (!ConnectorSocket.Connected) await Task.Delay(1);
        //Inform we are connected
        BmpJamboree.Instance.PublishEvent(
            new PartyConnectionChangedEvent(PartyConnectionChangedEvent.ResponseCode.OK, "Connected"));

        BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[NetworkSocket]: Send handshake\r\n"));
        SendPacket(ZeroTierPacketBuilder.MSG_JOIN_PARTY(FoundClients.Instance.Type, FoundClients.Instance.OwnName));

        _timer = new Timer();
        _timer.Interval = 10000;
        _timer.Elapsed += _timer_Elapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;

        return false;
    }

    private void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (_await_pong)
        {
            _close = true;
            _timer.Enabled = false;
            return;
        }

        var buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.PING);
        SendPacket(buffer.GetData());
        _await_pong = true;
        _timer.Interval = 3000;
    }

    public bool Update()
    {
        var bytes = new byte[60000];
        if (_close)
        {
            CloseConnection();
            return false;
        }

        if (ListenSocket.Poll(0, SelectMode.SelectError))
        {
            CloseConnection();
            return false;
        }

        if (ListenSocket.Available == -1)
            return false;

        if (!ListenSocket.Poll(100, SelectMode.SelectRead)) return true;

        try
        {
            var bytesRec = ListenSocket.Receive(bytes);
            if (bytesRec == -1)
            {
                CloseConnection();
                return false;
            }

            OpcodeHandling(bytes, bytesRec);
        }
        catch (SocketException err)
        {
            Console.WriteLine(
                "ServiceErrorCode={0} SocketErrorCode={1}",
                err.ServiceErrorCode,
                err.SocketErrorCode);
            return false;
        }

        return true;
    }

    public void SendPacket(byte[] pck)
    {
        if (ConnectorSocket.Available == -1) _close = true;

        if (!ConnectorSocket.Connected) _close = true;

        try
        {
            if (ConnectorSocket.Send(pck) == -1) _close = true;
        }
        catch
        {
            _close = true;
        }

        _close = false;
    }

    private void OpcodeHandling(byte[] bytes, int bytesRec)
    {
        var packet = new NetworkPacket(bytes);
        switch (packet.Opcode)
        {
            case NetworkOpcodes.OpcodeEnum.PING:
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent("[SocketServer]: Ping \r\n"));
                var buffer = new NetworkPacket(NetworkOpcodes.OpcodeEnum.PONG);
                SendPacket(buffer.GetData());
                break;
            case NetworkOpcodes.OpcodeEnum.PONG:
                _await_pong = false;
                _timer.Interval = 30000;
                break;
            case NetworkOpcodes.OpcodeEnum.MSG_JOIN_PARTY:
                PartyClient.Performer_Type = packet.ReadUInt8();
                PartyClient.Performer_Name = packet.ReadCString();
                BmpJamboree.Instance.PublishEvent(new PartyDebugLogEvent(
                    "[SocketServer]: Received handshake from " + PartyClient.Performer_Name + "\r\n"));
                break;
            case NetworkOpcodes.OpcodeEnum.MSG_PLAY:
                BmpJamboree.Instance.PublishEvent(new PerformanceStartEvent(packet.ReadInt64(), true));
                break;
            case NetworkOpcodes.OpcodeEnum.MSG_STOP:
                BmpJamboree.Instance.PublishEvent(new PerformanceStartEvent(packet.ReadInt64(), false));
                break;
            case NetworkOpcodes.OpcodeEnum.MSG_SONG_DATA:
                Debug.WriteLine("");
                break;
        }
    }

    public void CloseConnection()
    {
        _await_pong = false;
        _timer.Enabled = false;
        ListenSocket.LingerState = new LingerOption(false, 10);
        try
        {
            ListenSocket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            ListenSocket.Close();
        }

        ListenSocket.LingerState = new LingerOption(false, 10);
        try
        {
            ConnectorSocket.Shutdown(SocketShutdown.Both);
        }
        finally
        {
            ConnectorSocket.Close();
        }

        FoundClients.Instance.Remove(_remoteIP);
    }
}