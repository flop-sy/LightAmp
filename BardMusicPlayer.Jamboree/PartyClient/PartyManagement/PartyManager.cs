#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace BardMusicPlayer.Jamboree.PartyClient.PartyManagement
{
    internal sealed class PartyManager
    {
        private readonly List<PartyClientInfo> _partyClients = new();

        public List<PartyClientInfo> GetPartyMembers()
        {
            return _partyClients;
        }

        public void Add(PartyClientInfo client)
        {
            if (_partyClients.Any(info => info.Performer_Name == client.Performer_Name)) return;

            _partyClients.Add(client);
        }

        public void Remove(PartyClientInfo client)
        {
            _partyClients.Remove(client);
        }

        #region Instance Constructor/Destructor

        private static readonly Lazy<PartyManager> LazyInstance = new(static () => new PartyManager());

        private PartyManager()
        {
            _partyClients.Clear();
        }

        public static PartyManager Instance => LazyInstance.Value;

        ~PartyManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}