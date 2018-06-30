using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SFarmer = StardewValley.Farmer;

namespace AllProfessions
{
    /// <summary>The entry class called by SMAPI.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>Professions to gain for each level. Each entry represents the skill, level requirement, and profession IDs.</summary>
        private readonly Tuple<Skill, int, int[]>[] ProfessionsToGain =
        {
            Tuple.Create(Skill.Farming, 5, new[] { SFarmer.rancher, SFarmer.tiller }),
            Tuple.Create(Skill.Farming, 10, new[] { SFarmer.butcher/*actually coopmaster*/, SFarmer.shepherd, SFarmer.artisan, SFarmer.agriculturist }),
            Tuple.Create(Skill.Fishing, 5, new[] { SFarmer.fisher, SFarmer.trapper }),
            Tuple.Create(Skill.Fishing, 10, new[] { SFarmer.angler, SFarmer.pirate, SFarmer.baitmaster, SFarmer.mariner }),
            Tuple.Create(Skill.Foraging, 5, new[] { SFarmer.forester, SFarmer.gatherer }),
            Tuple.Create(Skill.Foraging, 10, new[] { SFarmer.lumberjack, SFarmer.tapper, SFarmer.botanist, SFarmer.tracker }),
            Tuple.Create(Skill.Mining, 5, new[] { SFarmer.miner, SFarmer.geologist }),
            Tuple.Create(Skill.Mining, 10, new[] { SFarmer.blacksmith, SFarmer.burrower/*actually prospector*/, SFarmer.excavator, SFarmer.gemologist }),
            Tuple.Create(Skill.Combat, 5, new[] { SFarmer.fighter, SFarmer.scout }),
            Tuple.Create(Skill.Combat, 10, new[] { SFarmer.brute, SFarmer.defender, SFarmer.acrobat, SFarmer.desperado })
        };


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += this.ReceiveAfterDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReceiveAfterDayStarted(object sender, EventArgs e)
        {
            // When the player loads a saved game, or after the overnight level screen,
            // add any professions the player should have but doesn't.
            this.AddMissingProfessions();
        }

        /****
        ** Methods
        ****/
        /// <summary>Add all missing professions.</summary>
        private void AddMissingProfessions()
        {
            // get missing professions
            List<int> expectedProfessions = new List<int>();
            foreach (var entry in this.ProfessionsToGain)
            {
                Skill skill = entry.Item1;
                int level = entry.Item2;
                int[] professions = entry.Item3;

                if (Game1.player.getEffectiveSkillLevel((int)skill) >= level)
                    expectedProfessions.AddRange(professions);
            }

            // add professions
            foreach (int professionID in expectedProfessions.Distinct().Except(Game1.player.professions))
            {
                // add profession
                Game1.player.professions.Add(professionID);

                // add health bonuses that are a special case of LevelUpMenu.getImmediateProfessionPerk
                switch (professionID)
                {
                    // fighter
                    case 24:
                        Game1.player.maxHealth += 15;
                        break;

                    // defender
                    case 27:
                        Game1.player.maxHealth += 25;
                        break;
                }
            }
        }
    }
}
