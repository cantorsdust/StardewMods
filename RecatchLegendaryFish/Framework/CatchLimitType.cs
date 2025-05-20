namespace RecatchLegendaryFish.Framework;

/// <summary>How often each legendary fish can be caught.</summary>
internal enum CatchLimitType
{
    /// <summary>Any number can be caught.</summary>
    Unlimited,

    /// <summary>One can be caught each day.</summary>
    OnePerDay,

    /// <summary>One can be caught each week.</summary>
    OnePerWeek,

    /// <summary>One can be caught each season.</summary>
    OnePerSeason,

    /// <summary>One can be caught each year.</summary>
    OnePerYear
}
