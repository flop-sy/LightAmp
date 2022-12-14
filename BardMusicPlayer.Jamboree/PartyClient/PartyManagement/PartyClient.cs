#region

using System.Collections.Generic;
using BardMusicPlayer.Jamboree.PartyNetworking;

#endregion

namespace BardMusicPlayer.Jamboree.PartyClient.PartyManagement;

public sealed class PartyClientInfo
{
    private readonly Queue<NetworkPacket> _inPackets = new();

    /// <summary>
    ///     Is this session a (0) bard, (1) dancer
    /// </summary>
    public byte Performer_Type { get; set; } = 254;

    public string Performer_Name { get; set; } = "Unknown";

    public void AddPacket(NetworkPacket packet)
    {
        _inPackets.Enqueue(packet);
    }
}