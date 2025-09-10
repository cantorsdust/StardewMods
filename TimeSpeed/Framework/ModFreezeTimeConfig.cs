using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;
using StardewValley;
using StardewValley.Locations;

namespace TimeSpeed.Framework;

/// <summary>The mod configuration for where or when time should be frozen.</summary>
internal class ModFreezeTimeConfig
{
    /*********
    ** Accessors
    *********/
    /// <summary>The time at which to freeze time everywhere (or <c>null</c> to disable this). This should be 24-hour military time (e.g. 800 for 8am, 1600 for 8pm, etc).</summary>
    public int? AnywhereAtTime { get; set; }

    /// <summary>Whether to freeze time before the player passes out (i.e. at 1:50am).</summary>
    public bool PassOut { get; set; } = false;

    /// <summary>Whether to freeze time indoors.</summary>
    public bool Indoors { get; set; } = false;

    /// <summary>Whether to freeze time outdoors.</summary>
    public bool Outdoors { get; set; } = false;

    /// <summary>Whether to freeze time in the mines.</summary>
    public bool Mines { get; set; } = false;

    /// <summary>Whether to freeze time in the Skull Cavern.</summary>
    public bool SkullCavern { get; set; } = false;

    /// <summary>Whether to freeze time in the Volcano Dungeon.</summary>
    public bool VolcanoDungeon { get; set; } = false;

    /// <summary>The names of custom locations in which to freeze time.</summary>
    /// <remarks>Location names can be seen in-game using the <a href="https://www.nexusmods.com/stardewvalley/mods/679">Debug Mode</a> mod.</remarks>
    public HashSet<string> ByLocationName { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The names of custom locations in which time shouldn't be frozen regardless of the previous settings.</summary>
    /// <remarks>See remarks on <see cref="ByLocationName"/>.</remarks>
    public HashSet<string> ExceptLocationNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);


    /*********
    ** Public methods
    *********/
    /// <summary>Get whether time should be frozen in the given location.</summary>
    /// <param name="location">The location to check.</param>
    public bool ShouldFreeze(GameLocation? location)
    {
        if (location == null || this.ExceptLocationNames.Contains(location.Name))
            return false;

        // by location name
        if (this.ByLocationName.Contains(location.Name))
            return true;

        // by location name (Deep Woods mod)
        if ((location.Name == "DeepWoods" || location.Name.StartsWith("DeepWoods_")) && this.ByLocationName.Contains("DeepWoods"))
            return true;

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
        this.ByLocationName = new HashSet<string>(this.ByLocationName ?? [], StringComparer.OrdinalIgnoreCase);
        this.ByLocationName.Remove(null!);

        this.ExceptLocationNames = new HashSet<string>(this.ExceptLocationNames ?? [], StringComparer.OrdinalIgnoreCase);
        this.ExceptLocationNames.Remove(null!);
    }
}
