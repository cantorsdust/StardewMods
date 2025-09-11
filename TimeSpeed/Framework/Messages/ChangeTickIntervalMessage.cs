namespace TimeSpeed.Framework.Messages;

/// <summary>A multiplayer message from a farmhand requesting a change to the tick interval.</summary>
internal class ChangeTickIntervalMessage
{
    /// <summary>Whether to increment the tick interval; else decrement.</summary>
    public bool Increase { get; set; }

    /// <summary>The absolute amount by which to change the tick interval.</summary>
    public int Change { get; set; }
}
