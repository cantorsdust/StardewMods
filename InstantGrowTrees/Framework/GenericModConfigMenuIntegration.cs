using System;
using cantorsdust.Common.Integrations;
using StardewModdingAPI;

namespace InstantGrowTrees.Framework;

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

        // fruit tree section
        api.AddSectionTitle(manifest, I18n.Config_FruitTrees);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FruitTrees_InstantlyAge_Name,
            tooltip: I18n.Config_FruitTrees_InstantlyAge_Desc,
            getValue: () => getConfig().FruitTrees.InstantlyAge,
            setValue: value => getConfig().FruitTrees.InstantlyAge = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FruitTrees_InstantlyGrow_Name,
            tooltip: I18n.Config_FruitTrees_InstantlyGrow_Desc,
            getValue: () => getConfig().FruitTrees.InstantlyGrow,
            setValue: value => getConfig().FruitTrees.InstantlyGrow = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FruitTrees_InstantlyGrowInWinter_Name,
            tooltip: I18n.Config_FruitTrees_InstantlyGrowInWinter_Desc,
            getValue: () => getConfig().FruitTrees.InstantlyGrowInWinter,
            setValue: value => getConfig().FruitTrees.InstantlyGrowInWinter = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_FruitTrees_InstantlyGrowWhenInvalid_Name,
            tooltip: I18n.Config_FruitTrees_InstantlyGrowWhenInvalid_Desc,
            getValue: () => getConfig().FruitTrees.InstantlyGrowWhenInvalid,
            setValue: value => getConfig().FruitTrees.InstantlyGrowWhenInvalid = value
        );

        // non-fruit tree section
        api.AddSectionTitle(manifest, I18n.Config_Trees);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Trees_InstantlyGrow_Name,
            tooltip: I18n.Config_Trees_InstantlyGrow_Desc,
            getValue: () => getConfig().NonFruitTrees.InstantlyGrow,
            setValue: value => getConfig().NonFruitTrees.InstantlyGrow = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Trees_InstantlyGrowInWinter_Name,
            tooltip: I18n.Config_Trees_InstantlyGrowInWinter_Desc,
            getValue: () => getConfig().NonFruitTrees.InstantlyGrowInWinter,
            setValue: value => getConfig().NonFruitTrees.InstantlyGrowInWinter = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Trees_InstantlyGrowWhenInvalid_Name,
            tooltip: I18n.Config_Trees_InstantlyGrowWhenInvalid_Desc,
            getValue: () => getConfig().NonFruitTrees.InstantlyGrowWhenInvalid,
            setValue: value => getConfig().NonFruitTrees.InstantlyGrowWhenInvalid = value
        );
    }
}
