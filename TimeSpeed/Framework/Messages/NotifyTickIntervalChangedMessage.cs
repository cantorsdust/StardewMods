namespace TimeSpeed.Framework.Messages;

/// <summary>A multiplayer message which indicates that the host changed the time speed.</summary>
internal class NotifyTickIntervalChangedMessage
{
    /// <summary>The new tick interval.</summary>
    public int NewInterval { get; set; }

    /// <summary>The player who requested the change.</summary>
    public long FromPlayerId { get; set; }
}
