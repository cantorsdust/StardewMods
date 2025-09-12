using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using cantorsdust.Common;

namespace AllProfessions.Framework;

/// <summary>A set of professions to gain for a skill level.</summary>
internal class ModDataProfessions
{
    /// <summary>The skill to check.</summary>
    public Skill Skill { get; set; }

    /// <summary>The minimum skill level to gain the professions.</summary>
    public int Level { get; set; }

    /// <summary>The professions to gain.</summary>
    public Profession[] Professions { get; set; } = [];


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
        this.Professions ??= [];
    }
}
