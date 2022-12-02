#region

using System;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core.Enums;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public bool CanGetPerformance()
        {
            return Scanner.Locations.ContainsKey(Signatures.PerformanceStatusKey);
        }

        public Instrument GetPerformance()
        {
            var result = Instrument.None;
            if (!CanGetPerformance() || !MemoryHandler.IsAttached) return result;

            try
            {
                var performanceData = MemoryHandler.GetByteArray(Scanner.Locations[Signatures.PerformanceStatusKey],
                    MemoryHandler.Structures.PerformanceInfo.SourceSize);

                var status = (Performance.Status)performanceData[MemoryHandler.Structures.PerformanceInfo.Status];
                var instrument = Instrument.Parse(performanceData[MemoryHandler.Structures.PerformanceInfo.Instrument]);

                switch (status)
                {
                    case Performance.Status.Closed:
                    case Performance.Status.Loading:
                        return Instrument.None;

                    case Performance.Status.Opened:
                    case Performance.Status.SwitchingNote:
                    case Performance.Status.HoldingNote:
                        return instrument > Instrument.None ? instrument : Instrument.None;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                MemoryHandler?.RaiseException(ex);
            }

            return result;
        }
    }
}