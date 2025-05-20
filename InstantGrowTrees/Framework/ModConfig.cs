using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;

namespace InstantGrowTrees.Framework;

/// <summary>The mod configuration model.</summary>
internal class ModConfig
{
    /*********
    ** Accessors
    *********/
    /// <summary>The configuration for fruit trees.</summary>
    public FruitTreeConfig FruitTrees { get; set; } = new();

    /// <summary>The configuration for non-fruit trees.</summary>
    public RegularTreeConfig NonFruitTrees { get; set; } = new();


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
        this.FruitTrees ??= new FruitTreeConfig();
        this.NonFruitTrees ??= new RegularTreeConfig();
    }
}
