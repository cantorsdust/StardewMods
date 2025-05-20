using System;
using System.Collections.Generic;
using System.Linq;
using cantorsdust.Common.Integrations;
using StardewModdingAPI;
using StardewValley;

namespace AllProfessions.Framework;

/// <summary>Configures the integration with Generic Mod Config Menu.</summary>
internal static class GenericModConfigMenuIntegration
{
    /*********
    ** Public methods
    *********/
    /// <summary>Add a config UI to Generic Mod Config Menu if it's installed.</summary>
    /// <param name="manifest">The mod manifest.</param>
    /// <param name="modRegistry">The mod registry from which to get the API.</param>
    /// <param name="monitor">The monitor with which to log errors.</param>
    /// <param name="professionData">The professions by skill and level requirement.</param>
    /// <param name="getConfig">Get the current mod configuration.</param>
    /// <param name="reset">Reset the config to its default values.</param>
    /// <param name="save">Save the current config to the <c>config.json</c> file.</param>
    public static void Register(IManifest manifest, IModRegistry modRegistry, IMonitor monitor, ModDataProfessions[] professionData, Func<ModConfig> getConfig, Action reset, Action save)
    {
        // get API
        IGenericModConfigMenuApi? api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, monitor);
        if (api == null)
            return;

        // add config UI based on profession map
        api.Register(manifest, reset, save);
        api.AddParagraph(manifest, I18n.Config_Intro);
        foreach ((Skill skill, int level, Profession[] professions) in GenericModConfigMenuIntegration.GetProfessionMappings(professionData))
        {
            api.AddSectionTitle(manifest, () => I18n.Config_SkillLevel(skillName: GetDisplayName(skill), level: level));
            foreach (Profession profession in professions)
            {
                api.AddBoolOption(
                    manifest,
                    name: () => GetDisplayName(profession),
                    tooltip: () => GetTooltip(skill, level, profession),
                    getValue: () => !getConfig().ShouldIgnore(profession),
                    setValue: shouldAdd =>
                    {
                        var config = getConfig();

                        config.IgnoreProfessions.Remove(profession.ToString());
                        config.IgnoreProfessions.Remove(((int)profession).ToString());
                        if (!shouldAdd)
                            config.IgnoreProfessions.Add(profession.ToString());
                    }
                );
            }
        }
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Get the professions to configure by skill and level.</summary>
    /// <param name="professions">The configured profession mappings.</param>
    private static IEnumerable<ProfessionMapping> GetProfessionMappings(ModDataProfessions[] professions)
    {
        foreach (var skillGroup in professions.GroupBy(p => p.Skill))
        {
            foreach (var levelGroup in skillGroup.GroupBy(p => p.Level).OrderBy(p => p.Key))
            {
                yield return new ProfessionMapping(
                    Skill: skillGroup.Key,
                    Level: levelGroup.Key,
                    Professions: levelGroup.SelectMany(p => p.Professions).Distinct().ToArray()
                );
            }
        }
    }

    /// <summary>Get the translated display name for a skill.</summary>
    /// <param name="skill">The skill to translate.</param>
    private static string GetDisplayName(Skill skill)
    {
        return skill switch
        {
            Skill.Farming => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604"),
            Skill.Mining => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605"),
            Skill.Foraging => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606"),
            Skill.Fishing => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607"),
            Skill.Combat => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608"),
            Skill.Luck => Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609"),
            _ => skill.ToString()
        };
    }

    /// <summary>Get the translated display name for a profession.</summary>
    /// <param name="profession">The profession to translate.</param>
    private static string GetDisplayName(Profession profession)
    {
        string key = $"Strings\\UI:LevelUp_ProfessionName_{profession}";
        string displayName = Game1.content.LoadString(key);

        return displayName != key
            ? displayName
            : profession.ToString();
    }

    /// <summary>Get the translated tooltip for a profession option.</summary>
    /// <param name="skill">The linked skill.</param>
    /// <param name="level">The skill level requirement.</param>
    /// <param name="profession">The profession to translate.</param>
    private static string GetTooltip(Skill skill, int level, Profession profession)
    {
        // generic tooltip
        string tooltip = I18n.Config_ToggleSkill_Desc(professionName: GetDisplayName(profession), skillName: GetDisplayName(skill), level: level);

        // add profession description if available
        {
            string key = $"Strings\\UI:LevelUp_ProfessionDescription_{profession}";
            string description = Game1.content.LoadString(key);
            if (description != key)
                tooltip += "\n\n" + description;
        }

        return tooltip;
    }


    /*********
    ** Private types
    *********/
    /// <summary>The professions associated with a given skill and skill level.</summary>
    /// <param name="Skill">The skill.</param>
    /// <param name="Level">The skill level requirement.</param>
    /// <param name="Professions">The linked professions.</param>
    private record ProfessionMapping(Skill Skill, int Level, Profession[] Professions);
}
