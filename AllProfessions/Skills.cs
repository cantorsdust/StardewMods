using SFarmer = StardewValley.Farmer;

namespace AllProfessions
{
    /// <summary>Represents the player skill types.</summary>
    public enum Skill
    {
        /// <summary>The farming skill.</summary>
        Farming = SFarmer.farmingSkill,

        /// <summary>The fishing skill.</summary>
        Fishing = SFarmer.fishingSkill,

        /// <summary>The foraging skill.</summary>
        Foraging = SFarmer.foragingSkill,

        /// <summary>The mining skill.</summary>
        Mining = SFarmer.miningSkill,

        /// <summary>The combat skill.</summary>
        Combat = SFarmer.combatSkill,

        /// <summary>The (disabled) luck skill.</summary>
        Luck = SFarmer.luckSkill
    }
}
