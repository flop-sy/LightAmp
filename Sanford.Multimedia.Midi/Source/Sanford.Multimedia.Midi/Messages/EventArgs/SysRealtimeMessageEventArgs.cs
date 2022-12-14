#region

using System;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed class SysRealtimeMessageEventArgs : EventArgs
{
    public static readonly SysRealtimeMessageEventArgs Start = new(SysRealtimeMessage.StartMessage);

    public static readonly SysRealtimeMessageEventArgs Continue = new(SysRealtimeMessage.ContinueMessage);

    public static readonly SysRealtimeMessageEventArgs Stop = new(SysRealtimeMessage.StopMessage);

    public static readonly SysRealtimeMessageEventArgs Clock = new(SysRealtimeMessage.ClockMessage);

    public static readonly SysRealtimeMessageEventArgs Tick = new(SysRealtimeMessage.TickMessage);

    public static readonly SysRealtimeMessageEventArgs ActiveSense = new(SysRealtimeMessage.ActiveSenseMessage);

    public static readonly SysRealtimeMessageEventArgs Reset = new(SysRealtimeMessage.ResetMessage);

    private SysRealtimeMessageEventArgs(SysRealtimeMessage message)
    {
        Message = message;
    }

    public SysRealtimeMessage Message { get; }
}