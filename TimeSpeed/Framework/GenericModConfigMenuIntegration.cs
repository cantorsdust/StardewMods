using System;
using System.Linq;
using cantorsdust.Common.Integrations;
using StardewModdingAPI;

namespace TimeSpeed.Framework;

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
    /// <param name="getConfig">Get the current mod configuration.</param>
    /// <param name="reset">Reset the config to its default values.</param>
    /// <param name="save">Save the current config to the <c>config.json</c> file.</param>
    public static void Register(IManifest manifest, IModRegistry modRegistry, IMonitor monitor, Func<ModConfig> getConfig, Action reset, Action save)
    {
        // get API
        IGenericModConfigMenuApi api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, monitor);
        if (api == null)
            return;

        // register config UI
        api.Unregister(manifest);
        api.Register(manifest, reset, save);

        // add warning if not host player
        if (!Context.IsMainPlayer)
            api.AddParagraph(manifest, I18n.Config_FarmhandWarning);

        // general options
        const float minSecondsPerMinute = 0.1f;
        const float maxSecondsPerMinute = 15f;
        api.AddSectionTitle(manifest, I18n.Config_GeneralOptions);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_EnableOnFestivalDays_Name,
            tooltip: I18n.Config_EnableOnFestivalDays_Desc,
            getValue: () => getConfig().EnableOnFestivalDays,
            setValue: value => getConfig().EnableOnFestivalDays = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_LocationNotify_Name,
            tooltip: I18n.Config_LocationNotify_Desc,
            getValue: () => getConfig().LocationNotify,
            setValue: value => getConfig().LocationNotify = value
        );

        // seconds per minute section
        api.AddSectionTitle(manifest, I18n.Config_SecondsPerMinute);
        api.AddNumberOption(
            manifest,
            name: I18n.Config_IndoorsSpeed_Name,
            tooltip: I18n.Config_IndoorsSpeed_Desc,
            getValue: () => (float)getConfig().SecondsPerMinute.Indoors,
            setValue: value => getConfig().SecondsPerMinute.Indoors = Math.Round(value, 2),
            min: minSecondsPerMinute,
            max: maxSecondsPerMinute,
            interval: 0.1f
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_OutdoorsSpeed_Name,
            tooltip: I18n.Config_OutdoorsSpeed_Desc,
            getValue: () => (float)getConfig().SecondsPerMinute.Outdoors,
            setValue: value => getConfig().SecondsPerMinute.Outdoors = Math.Round(value, 2),
            min: minSecondsPerMinute,
            max: maxSecondsPerMinute,
            interval: 0.1f
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_MineSpeed_Name,
            tooltip: I18n.Config_MineSpeed_Desc,
            getValue: () => (float)getConfig().SecondsPerMinute.Mines,
            setValue: value => getConfig().SecondsPerMinute.Mines = Math.Round(value, 2),
            min: minSecondsPerMinute,
            max: maxSecondsPerMinute,
            interval: 0.1f
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_SkullCavernSpeed_Name,
            tooltip: I18n.Config_SkullCavernSpeed_Desc,
            getValue: () => (float)getConfig().SecondsPerMinute.SkullCavern,
            setValue: value => getConfig().SecondsPerMinute.SkullCavern = Math.Round(value, 2),
            min: minSecondsPerMinute,
            max: maxSecondsPerMinute,
            interval: 0.1f
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_VolcanoDungeonSpeed_Name,
            tooltip: I18n.Config_VolcanoDungeonSpeed_Desc,
            getValue: () => (float)getConfig().SecondsPerMinute.VolcanoDungeon,
            setValue: value => getConfig().SecondsPerMinute.VolcanoDungeon = Math.Round(value, 2),
            min: minSecondsPerMinute,
            max: maxSecondsPerMinute,
            interval: 0.1f
        );

        // freeze time
        api.AddSectionTitle(manifest, I18n.Config_FreezeTime);
        api.AddNumberOption(
            manifest,
            name: I18n.Config_AnywhereAtTime_Name,
            tooltip: I18n.Config_AnywhereAtTime_Desc,
            getValue: () => getConfig().FreezeTime.AnywhereAtTime ?? 2600,
            setValue: value => getConfig().FreezeTime.AnywhereAtTime = (value == 2600 ? null : value),
            min: 600,
            max: 2600
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FreezeTimeIndoors_Name,
            tooltip: I18n.Config_FreezeTimeIndoors_Desc,
            getValue: () => getConfig().FreezeTime.Indoors,
            setValue: value => getConfig().FreezeTime.Indoors = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FreezeTimeOutdoors_Name,
            tooltip: I18n.Config_FreezeTimeOutdoors_Desc,
            getValue: () => getConfig().FreezeTime.Outdoors,
            setValue: value => getConfig().FreezeTime.Outdoors = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FreezeTimeMine_Name,
            tooltip: I18n.Config_FreezeTimeMine_Desc,
            getValue: () => getConfig().FreezeTime.Mines,
            setValue: value => getConfig().FreezeTime.Mines = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FreezeTimeSkullCavern_Name,
            tooltip: I18n.Config_FreezeTimeSkullCavern_Desc,
            getValue: () => getConfig().FreezeTime.SkullCavern,
            setValue: value => getConfig().FreezeTime.SkullCavern = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FreezeTimeVolcanoDungeon_Name,
            tooltip: I18n.Config_FreezeTimeVolcanoDungeon_Desc,
            getValue: () => getConfig().FreezeTime.VolcanoDungeon,
            setValue: value => getConfig().FreezeTime.VolcanoDungeon = value
        );
        api.AddTextOption(
            manifest,
            name: I18n.Config_FreezeTimeFreezeNames_Name,
            tooltip: I18n.Config_FreezeTimeFreezeNames_Desc,
            getValue: () => string.Join(", ", getConfig().FreezeTime.ByLocationName),
            setValue: value => getConfig().FreezeTime.ByLocationName = new(
                value
                    .Split(",")
                    .Select(p => p.Trim())
                    .Where(p => p != string.Empty)
            )
        );
        api.AddTextOption(
            manifest,
            name: I18n.Config_FreezeTimeDontFreezeNames_Name,
            tooltip: I18n.Config_FreezeTimeDontFreezeNames_Desc,
            getValue: () => string.Join(", ", getConfig().FreezeTime.ExceptLocationNames),
            setValue: value => getConfig().FreezeTime.ExceptLocationNames = new(
                value
                    .Split(",")
                    .Select(p => p.Trim())
                    .Where(p => p != string.Empty)
            )
        );

        // controls
        api.AddSectionTitle(manifest, I18n.Config_Controls);
        api.AddKeybindList(
            manifest,
            name: I18n.Config_FreezeTimeKey_Name,
            tooltip: I18n.Config_FreezeTimeKey_Desc,
            getValue: () => getConfig().Keys.FreezeTime,
            setValue: value => getConfig().Keys.FreezeTime = value
        );
        api.AddKeybindList(
            manifest,
            name: I18n.Config_SlowTimeKey_Name,
            tooltip: I18n.Config_SlowTimeKey_Desc,
            getValue: () => getConfig().Keys.IncreaseTickInterval,
            setValue: value => getConfig().Keys.IncreaseTickInterval = value
        );
        api.AddKeybindList(
            manifest,
            name: I18n.Config_SpeedUpTimeKey_Name,
            tooltip: I18n.Config_SpeedUpTimeKey_Desc,
            getValue: () => getConfig().Keys.DecreaseTickInterval,
            setValue: value => getConfig().Keys.DecreaseTickInterval = value
        );
        api.AddKeybindList(
            manifest,
            name: I18n.Config_ReloadKey_Name,
            tooltip: I18n.Config_ReloadKey_Desc,
            getValue: () => getConfig().Keys.ReloadConfig,
            setValue: value => getConfig().Keys.ReloadConfig = value
        );
    }
}
