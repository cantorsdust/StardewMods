using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using cantorsdust.Common;

namespace AllProfessions.Framework;

/// <summary>The mod configuration model.</summary>
internal class ModData
{
    /// <summary>The professions to gain for each level.</summary>
    public ModDataProfessions[] ProfessionsToGain { get; set; } = [];


    /*********
    ** Private methods
    *********/
    /// <summary>The method called after the config file is deserialized.</summary>
    /// <param name="context">The deserialization context.</param>
    [OnDeserialized]
    [SuppressMessage("ReSharper", "NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract", Justification = SuppressReasons.ValidatesNullability)]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = SuppressReasons.ValidatesNullability)]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = SuppressReasons.UsedViaReflection)]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = SuppressReasons.UsedViaReflection)]
    private void OnDeserializedMethod(StreamingContext context)
    {
        this.ProfessionsToGain ??= [];

        if (this.ProfessionsToGain.Any(p => p is null))
            this.ProfessionsToGain = this.ProfessionsToGain.Where(p => p is not null).ToArray();
    }
}
