using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;
using StardewValley;
using StardewValley.Locations;

namespace TimeSpeed.Framework;

/// <summary>The time speed for in-game locations, measured in seconds per 10-game-minutes.</summary>
internal class ModSecondsPerMinuteConfig
{
    /*********
    ** Accessors
    *********/
    /// <summary>The time speed when indoors.</summary>
    public double Indoors { get; set; } = 1.4;

    /// <summary>The time speed when outdoors.</summary>
    public double Outdoors { get; set; } = 0.7;

    /// <summary>The time speed in the mines.</summary>
    public double Mines { get; set; } = 0.7;

    /// <summary>The time speed in the Skull Cavern.</summary>
    public double SkullCavern { get; set; } = 0.9;

    /// <summary>The time speed in the Volcano Dungeon.</summary>
    public double VolcanoDungeon { get; set; } = 0.9;

    /// <summary>The time speed for custom location names.</summary>
    /// <remarks>Location names can be seen in-game using the <a href="https://www.nexusmods.com/stardewvalley/mods/679">Debug Mode</a> mod.</remarks>
    public Dictionary<string, double> ByLocationName { get; set; } = new(StringComparer.OrdinalIgnoreCase);


    /*********
    ** Public methods
    *********/
    /// <summary>Get the number of seconds per in-game minute for a given location.</summary>
    /// <param name="location">The location to check.</param>
    public double GetSecondsPerMinute(GameLocation? location)
    {
        if (location == null)
            return this.Outdoors;

        // by location name
        if (this.ByLocationName.TryGetValue(location.Name, out double tickLength))
            return tickLength;

        // by location name (Deep Woods mod)
        if ((location.Name == "DeepWoods" || location.Name.StartsWith("DeepWoods_")) && this.ByLocationName.TryGetValue("DeepWoods", out tickLength))
            return tickLength;

        // mines / Skull Cavern
        if (location is MineShaft shaft)
        {
            return shaft.mineLevel <= 120
                ? this.Mines
                : this.SkullCavern;
        }

        // volcano dungeon
        if (location is VolcanoDungeon)
            return this.VolcanoDungeon;

        // indoors or outdoors
        return location.IsOutdoors
            ? this.Outdoors
            : this.Indoors;
    }


    /*********
    ** Private methods
    *********/
    /// <summary>The method called after the config file is deserialized.</summary>
    /// <param name="context">The deserialization context.</param>
    [OnDeserialized]
    [SuppressMessage("ReSharper", "NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract", Justification = SuppressReasons.ValidatesNullability)]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = SuppressReasons.UsedViaReflection)]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = SuppressReasons.UsedViaReflection)]
    private void OnDeserializedMethod(StreamingContext context)
    {
        this.ByLocationName = new Dictionary<string, double>(this.ByLocationName ?? [], StringComparer.OrdinalIgnoreCase);
    }
}
