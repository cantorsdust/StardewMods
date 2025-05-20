using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TimeSpeed.Framework;

/// <summary>The keyboard bindings used to control the flow of time. See available keys at <a href="https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx" />. Set a key to null to disable it.</summary>
internal class ModControlsConfig
{
    /*********
    ** Accessors
    *********/
    /// <summary>Freeze or unfreeze time. Freezing time will stay in effect until you unfreeze it; unfreezing time will stay in effect until you enter a new location with time settings.</summary>
    public KeybindList FreezeTime { get; set; } = new(SButton.N);

    /// <summary>Slow down time by one second per 10-game-minutes. Combine with Control to increase by 100 seconds, Shift to increase by 10 seconds, or Alt to increase by 0.1 seconds.</summary>
    public KeybindList IncreaseTickInterval { get; set; } = new(SButton.OemPeriod);

    /// <summary>Speed up time by one second per 10-game-minutes. Combine with Control to decrease by 100 seconds, Shift to decrease by 10 seconds, or Alt to decrease by 0.1 seconds.</summary>
    public KeybindList DecreaseTickInterval { get; set; } = new(SButton.OemComma);

    /// <summary>Reload all values from the config file and apply them immediately. Time will stay frozen if it was frozen via keybind.</summary>
    public KeybindList ReloadConfig { get; set; } = new(SButton.B);


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
        this.FreezeTime ??= new KeybindList();
        this.IncreaseTickInterval ??= new KeybindList();
        this.DecreaseTickInterval ??= new KeybindList();
        this.ReloadConfig ??= new KeybindList();
    }
}
