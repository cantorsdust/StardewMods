using System;
using cantorsdust.Common.Integrations;
using StardewModdingAPI;
using StardewValley;

namespace RecatchLegendaryFish.Framework;

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
        IGenericModConfigMenuApi? api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, monitor);
        if (api == null)
            return;

        // register config UI
        api.Register(manifest, reset, save);

        // add options
        api.AddSectionTitle(manifest, I18n.Config_Options);
        api.AddTextOption(
            manifest,
            name: I18n.Config_CatchLimit_Name,
            tooltip: I18n.Config_CatchLimit_Desc,
            getValue: () => getConfig().CatchLimit.ToString(),
            setValue: value => getConfig().CatchLimit = Utility.TryParseEnum(value, out CatchLimitType limit)
                ? limit
                : CatchLimitType.Unlimited,
            allowedValues: Enum.GetNames<CatchLimitType>(),
            formatAllowedValue: value => I18n.GetByKey($"config.catch-limit.{value}").Default(value.ToString())
        );

        api.AddSectionTitle(manifest, I18n.Config_Controls);
        api.AddKeybindList(
            manifest,
            name: I18n.Config_ToggleKey_Name,
            tooltip: I18n.Config_ToggleKey_Desc,
            getValue: () => getConfig().ToggleKey,
            setValue: value => getConfig().ToggleKey = value
        );
    }
}
