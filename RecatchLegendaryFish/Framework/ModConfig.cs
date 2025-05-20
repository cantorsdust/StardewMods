using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;
using StardewModdingAPI.Utilities;

namespace RecatchLegendaryFish.Framework;

/// <summary>The mod configuration model.</summary>
internal class ModConfig
{
    /*********
    ** Accessors
    *********/
    /// <summary>A keybind which toggles whether the player can recatch fish.</summary>
    public KeybindList ToggleKey { get; set; } = new();


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
        this.ToggleKey ??= new KeybindList();
    }
}
