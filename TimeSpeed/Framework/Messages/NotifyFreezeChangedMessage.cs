namespace TimeSpeed.Framework.Messages;

/// <summary>A multiplayer message which indicates that the host froze or unfroze time.</summary>
internal class NotifyFreezeChangedMessage
{
    /// <summary>Whether the time is now frozen (true) or unfrozen (false).</summary>
    public bool IsFrozen { get; set; }

    /// <summary>The player who requested the change.</summary>
    public long FromPlayerId { get; set; }
}
