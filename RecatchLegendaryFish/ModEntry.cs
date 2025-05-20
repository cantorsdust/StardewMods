using System;
using System.Collections.Generic;
using System.Text.Json;
using cantorsdust.Common;
using RecatchLegendaryFish.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Locations;

namespace RecatchLegendaryFish;

/// <summary>The entry class called by SMAPI.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Properties
    *********/
    /// <summary>The mod configuration.</summary>
    private ModConfig Config = null!; // set in Entry

    /// <summary>Whether the mod is currently enabled.</summary>
    private bool IsEnabled = true;

    /// <summary>The last player for which changes were applied.</summary>
    private readonly PerScreen<Farmer> LastPlayer = new();

    /// <summary>When each legendary fish was last caught.</summary>
    /// <remarks>This has the <see cref="WorldDate.TotalDays"/> indexed by qualified fish ID.</remarks>
    private readonly PerScreen<Dictionary<string, int>> LastCaughtPerFish = new(() => new());


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        CommonHelper.RemoveObsoleteFiles(this, "RecatchLegendaryFish.pdb");

        this.Config = helper.ReadConfig<ModConfig>();

        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
    }


    /*********
    ** Private methods
    *********/
    /****
    ** Event handlers
    ****/
    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (this.IsEnabled && e.Name.IsEquivalentTo("Data/Locations"))
        {
            Dictionary<string, int> lastCaughtPerFish = this.LastCaughtPerFish.Value;
            int? startOfLimitPeriod = this.GetStartOfLimitPeriod()?.TotalDays;

            e.Edit(
                asset =>
                {
                    foreach (LocationData location in asset.AsDictionary<string, LocationData>().Data.Values)
                    {
                        if (location.Fish is null)
                            continue;

                        foreach (SpawnFishData? fish in location.Fish)
                        {
                            if (fish is null)
                                continue;

                            // Known limitation: there's no good way to handle ItemId being an item query instead
                            // of an item ID, but all vanilla legendary fish (and likely most modded ones) use an
                            // item ID.
                            if (fish.CatchLimit == 1 && ItemContextTagManager.HasBaseTag(fish.ItemId, "fish_legendary"))
                            {
                                // unlimited
                                if (!startOfLimitPeriod.HasValue)
                                {
                                    fish.CatchLimit = -1;
                                    continue;
                                }

                                // else apply limit
                                string qualifiedId = ItemRegistry.QualifyItemId(fish.ItemId);
                                if (qualifiedId != null)
                                {
                                    bool caughtThisPeriod = lastCaughtPerFish.TryGetValue(qualifiedId, out int lastCaught) && lastCaught >= startOfLimitPeriod;
                                    fish.CatchLimit = caughtThisPeriod ? 0 : 1;
                                }
                            }
                        }
                    }
                },
                AssetEditPriority.Late // handle new legendary fish added by mods
            );
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(this.ModManifest, this.Helper.ModRegistry, this.Monitor,
            getConfig: () => this.Config,
            reset: () => this.Config = new(),
            save: this.OnConfigChanged
        );
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Context.IsPlayerFree && this.Config.ToggleKey.JustPressed())
            this.OnToggle();
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // load data
        this.LastCaughtPerFish.Value = this.LoadFishLastCaughtData();

        // hook into fish-caught event
        if (this.LastPlayer.Value != null)
            this.LastPlayer.Value.fishCaught.OnValueAdded -= this.OnFishCaught;

        Game1.player.fishCaught.OnValueAdded += this.OnFishCaught;
        this.LastPlayer.Value = Game1.player;

        // apply catch limit
        this.RebuildContentChanges();
    }

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.SaveFishCaughtData(this.LastCaughtPerFish.Value);
    }

    /// <summary>Handle a fish getting caught by the player.</summary>
    /// <param name="fishId">The qualified item ID of the fish that was caught.</param>
    /// <param name="countAndSize">An array containing (0) the total number of this fish caught by the player and (1) and the largest size caught.</param>
    private void OnFishCaught(string fishId, int[] countAndSize)
    {
        fishId = ItemRegistry.QualifyItemId(fishId);
        if (fishId is null || !ItemContextTagManager.HasBaseTag(fishId, "fish_legendary"))
            return;

        this.LastCaughtPerFish.Value[fishId] = Game1.Date.TotalDays;
    }

    /// <summary>Update when the mod settings are changed through Generic Mod Config Menu.</summary>
    private void OnConfigChanged()
    {
        this.Helper.WriteConfig(this.Config);
        this.RebuildContentChanges();
    }

    /// <summary>Handle the toggle key.</summary>
    private void OnToggle()
    {
        this.IsEnabled = !this.IsEnabled;
        this.RebuildContentChanges();

        string? key = this.Config.ToggleKey.GetKeybindCurrentlyDown()?.ToString();
        string message = this.IsEnabled
            ? I18n.Message_Enabled(key: key)
            : I18n.Message_Disabled(key: key);
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 2500 });
    }

    /// <summary>Load the fish-last-caught data for the current player.</summary>
    private Dictionary<string, int> LoadFishLastCaughtData()
    {
        string rawFishData = Game1.player.modData.GetValueOrDefault(this.GetFishCaughtKey());
        if (rawFishData is null)
            return new();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, int>>(rawFishData) ?? new();
        }
        catch (Exception ex)
        {
            this.Monitor.Log("Couldn't read fish-last-caught dates from the save file; resetting dates.", LogLevel.Error);
            this.Monitor.Log(ex.ToString());
            return new();
        }
    }

    /// <summary>Save the fish-last-caught data to the current player.</summary>
    /// <param name="data">The data to save.</param>
    private void SaveFishCaughtData(Dictionary<string, int> data)
    {
        Game1.player.modData[this.GetFishCaughtKey()] = JsonSerializer.Serialize(data);
    }

    /// <summary>Invalidate the <c>Data/Locations</c> asset to reapply the fish catch limits.</summary>
    private void RebuildContentChanges()
    {
        this.Helper.GameContent.InvalidateCache("Data/Locations");
    }

    /// <summary>Get the unique key in <see cref="Character.modData"/> which persists the <see cref="LastCaughtPerFish"/> data.</summary>
    private string GetFishCaughtKey()
    {
        return $"{this.ModManifest.UniqueID}_DateLastCaught";
    }

    /// <summary>Get the minimum date for which the catch limit applies, or <c>null</c> if it's unlimited.</summary>
    private WorldDate? GetStartOfLimitPeriod()
    {
        WorldDate today = WorldDate.Now();

        switch (this.Config.CatchLimit)
        {
            // today
            case CatchLimitType.OnePerDay:
                return today;

            // last Monday
            case CatchLimitType.OnePerWeek:
                {
                    int daysSinceMonday = today.DayOfWeek == DayOfWeek.Sunday // sunday is 0
                        ? 6
                        : (int)today.DayOfWeek - 1;
                    return WorldDate.ForDaysPlayed(today.TotalDays - daysSinceMonday);
                }

            // start of season
            case CatchLimitType.OnePerSeason:
                return WorldDate.ForDaysPlayed(today.TotalDays - today.DayOfMonth + 1);

            // start of year
            case CatchLimitType.OnePerYear:
                return WorldDate.ForDaysPlayed(today.TotalDays - (today.SeasonIndex * WorldDate.DaysPerMonth) - today.DayOfMonth + 1);

            // no limit
            default:
                return null;
        }
    }
}
