namespace TimeSpeed.Framework;

/// <summary>The reasons for automated time freezes.</summary>
internal enum AutoFreezeReason
{
    /// <summary>No freeze currently applies.</summary>
    None,

    /// <summary>Time was automatically frozen based on the location.</summary>
    FrozenForLocation,

    /// <summary>Time was automatically frozen based on the time of day.</summary>
    FrozenAtTime,

    /// <summary>Time was automatically frozen before the player passed out.</summary>
    FrozenBeforePassOut
}
