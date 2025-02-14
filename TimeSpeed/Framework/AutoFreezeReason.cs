using StardewValley;

namespace TimeSpeed.Framework;

/// <summary>The reasons for automated time freezes.</summary>
internal enum AutoFreezeReason
{
    /// <summary>No freeze currently applies.</summary>
    None,

    /// <summary>Time was automatically frozen based on the location per <see cref="ModConfig.ShouldFreeze(GameLocation)"/>.</summary>
    FrozenForLocation,

    /// <summary>Time was automatically frozen per <see cref="ModConfig.ShouldFreeze(int)"/>.</summary>
    FrozenAtTime,

    /// <summary>Time was automatically frozen per <see cref="ModConfig.ShouldFreezeBeforePassOut(int)"/>.</summary>
    FrozenBeforePassOut
}
