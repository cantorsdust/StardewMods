using System;
using System.Collections.Generic;
using cantorsdust.Common;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TimeSpeed.Framework;
using TimeSpeed.Framework.Messages;

namespace TimeSpeed;

/// <summary>The entry class called by SMAPI.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Properties
    *********/
    /// <summary>Provides helper methods for tracking time flow.</summary>
    private readonly TimeHelper TimeHelper = new();

    /// <summary>Displays messages to the user.</summary>
    private Notifier Notifier = null!; // set in Entry

    /// <summary>The mod configuration.</summary>
    private ModConfig Config = null!; // set in Entry

    /// <summary>Whether the player has manually frozen time.</summary>
    private bool ManualFreeze;

    /// <summary>The reason time would be frozen automatically if applicable, regardless of <see cref="ManualFreeze"/>.</summary>
    private AutoFreezeReason AutoFreeze = AutoFreezeReason.None;

    /// <summary>The current auto-freeze reasons which the player has temporarily suspended until the relevant context changes.</summary>
    private readonly HashSet<AutoFreezeReason> SuspendAutoFreezes = [];

    /// <summary>Whether time should be frozen.</summary>
    private bool IsTimeFrozen =>
        this.ManualFreeze
        || (this.AutoFreeze != AutoFreezeReason.None && !this.SuspendAutoFreezes.Contains(this.AutoFreeze));

    /// <summary>Whether the flow of time should be adjusted.</summary>
    private bool AdjustTime;

    /// <summary>Backing field for <see cref="TickInterval"/>.</summary>
    private int _tickInterval;

    /// <summary>The number of milliseconds per 10-game-minutes to apply.</summary>
    private int TickInterval
    {
        get => this._tickInterval;
        set => this._tickInterval = Math.Max(value, 0);
    }


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // init
        I18n.Init(helper.Translation);
        CommonHelper.RemoveObsoleteFiles(this, "TimeSpeed.pdb");
        this.Notifier = new Notifier(this.Helper.Multiplayer, this.ModManifest.UniqueID, this.Monitor);

        // read config
        this.Config = helper.ReadConfig<ModConfig>();

        // add time events
        this.TimeHelper.WhenTickProgressChanged(this.OnTickProgressed);
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
        helper.Events.Player.Warped += this.OnWarped;

        // add time freeze/unfreeze notification
        {
            bool wasPaused = false;
            helper.Events.Display.RenderingHud += (_, _) =>
            {
                wasPaused = Game1.paused;
                if (this.IsTimeFrozen)
                    Game1.paused = true;
            };

            helper.Events.Display.RenderedHud += (_, _) =>
            {
                Game1.paused = wasPaused;
            };
        }
    }


    /*********
    ** Private methods
    *********/
    /****
    ** Event handlers
    ****/
    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.RegisterConfigMenu();
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.RegisterConfigMenu();
    }

    /// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != this.ModManifest.UniqueID || e.FromPlayerID == Game1.player.UniqueMultiplayerID)
            return;

        switch (e.Type)
        {
            // from farmhand: request to (un)freeze time
            case nameof(ToggleFreezeMessage):
                if (Context.IsMainPlayer)
                {
                    if (!this.Config.LetFarmhandsManageTime)
                        this.RejectRequestFromFarmhand("toggle time freeze", e.FromPlayerID);
                    else
                        this.ToggleFreeze(fromPlayerId: e.FromPlayerID);
                }

                break;

            // from farmhand: request to change time speed
            case nameof(ChangeTickIntervalMessage):
                if (Context.IsMainPlayer)
                {
                    if (!this.Config.LetFarmhandsManageTime)
                        this.RejectRequestFromFarmhand("change time speed", e.FromPlayerID);
                    else
                    {
                        var message = e.ReadAs<ChangeTickIntervalMessage>();
                        this.ChangeTickInterval(message.Increase, message.Change, fromPlayerId: e.FromPlayerID);
                    }
                }
                break;

            // from host: access denied
            case nameof(RequestDeniedMessage):
                this.Notifier.OnAccessDeniedFromHost(I18n.Message_HostAccessDenied());
                break;

            // from host: time speed changed
            case nameof(NotifyTickIntervalChangedMessage):
                if (!Context.IsMainPlayer)
                {
                    var message = e.ReadAs<NotifyTickIntervalChangedMessage>();
                    this.Notifier.OnSpeedChanged(message.NewInterval, fromPlayerId: message.FromPlayerId);
                }
                break;

            // from host: time (un)frozen
            case nameof(NotifyFreezeChangedMessage):
                if (!Context.IsMainPlayer)
                {
                    var message = e.ReadAs<NotifyFreezeChangedMessage>();
                    this.Notifier.OnTimeFreezeToggled(frozen: message.IsFrozen, fromPlayerId: message.FromPlayerId);
                }
                break;
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!this.ShouldEnable())
            return;

        this.UpdateScaleForDay(Game1.season, Game1.dayOfMonth);
        this.UpdateTimeFreeze(clearPreviousOverrides: true);
        this.UpdateSettingsForLocation(Game1.currentLocation);
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!this.ShouldEnable(forInput: true))
            return;

        if (this.Config.Keys.FreezeTime.JustPressed())
            this.ToggleFreeze();
        else if (this.Config.Keys.IncreaseTickInterval.JustPressed())
            this.ChangeTickInterval(increase: true);
        else if (this.Config.Keys.DecreaseTickInterval.JustPressed())
            this.ChangeTickInterval(increase: false);
        else if (this.Config.Keys.ReloadConfig.JustPressed())
            this.ReloadConfig();
    }

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (!this.ShouldEnable() || !e.IsLocalPlayer)
            return;

        this.UpdateSettingsForLocation(e.NewLocation);
    }

    /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (!this.ShouldEnable())
            return;

        this.UpdateFreezeForTime();
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!this.ShouldEnable())
            return;

        this.TimeHelper.Update();

        if (e.IsOneSecond && this.Monitor.IsVerbose)
        {
            string? timeFrozenLabel;
            if (this.ManualFreeze)
                timeFrozenLabel = ", frozen manually";
            else if (this.SuspendAutoFreezes.Contains(this.AutoFreeze))
                timeFrozenLabel = ", resumed manually";
            else if (this.IsTimeFrozen)
                timeFrozenLabel = $", frozen per {this.AutoFreeze}";
            else
                timeFrozenLabel = null;

            this.Monitor.Log($"Time is {Game1.timeOfDay}; {this.TimeHelper.TickProgress:P} towards {Utility.ModifyTime(Game1.timeOfDay, 10)} (tick interval: {this.TimeHelper.CurrentDefaultTickInterval}, {this.TickInterval / 10_000m:0.##}s/min{timeFrozenLabel})");
        }
    }

    /// <summary>Raised after the <see cref="TimeHelper.TickProgress"/> value changes.</summary>
    private void OnTickProgressed(object? sender, TickProgressChangedEventArgs e)
    {
        if (!this.ShouldEnable())
            return;

        if (this.IsTimeFrozen)
            this.TimeHelper.TickProgress = e.TimeChanged ? 0 : e.PreviousProgress;
        else
        {
            if (!this.AdjustTime)
                return;
            if (this.TickInterval == 0)
                this.TickInterval = 1000;

            if (e.TimeChanged)
                this.TimeHelper.TickProgress = this.ScaleTickProgress(this.TimeHelper.TickProgress, this.TickInterval);
            else
                this.TimeHelper.TickProgress = e.PreviousProgress + this.ScaleTickProgress(e.NewProgress - e.PreviousProgress, this.TickInterval);
        }
    }

    /****
    ** Methods
    ****/
    /// <summary>Get whether time features should be enabled.</summary>
    /// <param name="forInput">Whether to check for input handling.</param>
    private bool ShouldEnable(bool forInput = false)
    {
        // save is loaded
        if (!Context.IsWorldReady)
            return false;

        // must be host player to directly control time
        if (!Context.IsMainPlayer && !forInput)
            return false;

        // check restrictions for input
        if (forInput)
        {
            // don't handle input when player isn't free (except in events)
            if (!Context.IsPlayerFree && !Game1.eventUp)
                return false;

            // ignore input if a textbox is active
            if (Game1.keyboardDispatcher.Subscriber is not null)
                return false;
        }

        return true;
    }

    /// <summary>Reload <see cref="Config"/> from the config file.</summary>
    private void ReloadConfig()
    {
        this.Config = this.Helper.ReadConfig<ModConfig>();
        this.UpdateScaleForDay(Game1.season, Game1.dayOfMonth);
        this.UpdateSettingsForLocation(Game1.currentLocation);
        this.Notifier.OnConfigReloaded();
    }

    /// <summary>Register or update the config menu with Generic Mod Config Menu.</summary>
    private void RegisterConfigMenu()
    {
        GenericModConfigMenuIntegration.Register(this.ModManifest, this.Helper.ModRegistry, this.Monitor,
            getConfig: () => this.Config,
            reset: () => this.Config = new ModConfig(),
            save: () =>
            {
                this.Helper.WriteConfig(this.Config);
                if (this.ShouldEnable())
                    this.UpdateSettingsForLocation(Game1.currentLocation);
            }
        );
    }

    /// <summary>Increment or decrement the tick interval, taking into account the held modifier key if applicable.</summary>
    /// <param name="increase">Whether to increment the tick interval; else decrement.</param>
    /// <param name="amount">The absolute amount by which to change the tick interval, or <c>null</c> to get the default amount based on the local pressed keys.</param>
    /// <param name="fromPlayerId">The player which requested the change, if applicable.</param>
    private void ChangeTickInterval(bool increase, int? amount = null, long? fromPlayerId = null)
    {
        // get offset to apply
        int change = amount ?? 1000;
        if (!amount.HasValue)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.LeftControl))
                change *= 100;
            else if (state.IsKeyDown(Keys.LeftShift))
                change *= 10;
            else if (state.IsKeyDown(Keys.LeftAlt))
                change /= 10;
        }

        // ask host to change the tick interval if needed
        if (!Context.IsMainPlayer)
        {
            this.SendMessageToHost(new ChangeTickIntervalMessage { Change = change, Increase = increase });
            return;
        }

        // update tick interval
        if (!increase)
        {
            int minAllowed = Math.Min(this.TickInterval, change);
            this.TickInterval = Math.Max(minAllowed, this.TickInterval - change);
        }
        else
            this.TickInterval += change;

        // log change
        this.Notifier.OnSpeedChanged(this.TickInterval, fromPlayerId);
    }

    /// <summary>Toggle whether time is frozen.</summary>
    /// <param name="fromPlayerId">The player which requested the change, if applicable.</param>
    private void ToggleFreeze(long? fromPlayerId = null)
    {
        // ask host to toggle freeze if needed
        if (!Context.IsMainPlayer)
        {
            this.SendMessageToHost(new ToggleFreezeMessage());
            return;
        }

        // apply
        bool freeze = !this.IsTimeFrozen;
        this.UpdateTimeFreeze(manualOverride: freeze);
        this.Notifier.OnTimeFreezeToggled(frozen: freeze, fromPlayerId: fromPlayerId);
    }

    /// <summary>Update the time freeze settings for the given time of day.</summary>
    private void UpdateFreezeForTime()
    {
        bool wasFrozen = this.IsTimeFrozen;
        this.UpdateTimeFreeze();

        if (!wasFrozen && this.IsTimeFrozen)
            this.Notifier.OnTimeFreezeToggled(frozen: true, logMessage: $"Time automatically set to frozen at {Game1.timeOfDay}.");
    }

    /// <summary>Update the time settings for the given location.</summary>
    /// <param name="location">The game location.</param>
    private void UpdateSettingsForLocation(GameLocation? location)
    {
        if (location == null)
            return;

        // update time settings
        this.SuspendAutoFreezes.Remove(AutoFreezeReason.FrozenForLocation);
        this.UpdateTimeFreeze();
        this.TickInterval = this.Config.GetMillisecondsPerMinute(location) * 10;

        // notify player
        if (this.Config.LocationNotify)
            this.Notifier.OnLocalLocationChanged(this.IsTimeFrozen, this.TickInterval, this.AutoFreeze);
    }

    /// <summary>Update the <see cref="AutoFreeze"/> and <see cref="ManualFreeze"/> flags based on the current context.</summary>
    /// <param name="manualOverride">An explicit freeze (<c>true</c>) or unfreeze (<c>false</c>) requested by the player, if applicable.</param>
    /// <param name="clearPreviousOverrides">Whether to clear any previous explicit overrides.</param>
    private void UpdateTimeFreeze(bool? manualOverride = null, bool clearPreviousOverrides = false)
    {
        bool wasManualFreeze = this.ManualFreeze;
        AutoFreezeReason wasAutoFreeze = this.AutoFreeze;

        // update auto freeze
        this.AutoFreeze = this.GetAutoFreezeType();
        bool isAutoFrozen = this.AutoFreeze != AutoFreezeReason.None;

        // update manual freeze
        if (manualOverride.HasValue)
            this.ManualFreeze = manualOverride.Value;

        // update overrides
        if (clearPreviousOverrides || !isAutoFrozen)
            this.SuspendAutoFreezes.Clear();
        if (manualOverride == false && isAutoFrozen)
            this.SuspendAutoFreezes.Add(this.AutoFreeze);

        // log change
        if (wasAutoFreeze != this.AutoFreeze)
            this.Monitor.Log($"Auto freeze changed from {wasAutoFreeze} to {this.AutoFreeze}.");
        if (wasManualFreeze != this.ManualFreeze)
            this.Monitor.Log($"Manual freeze changed from {wasManualFreeze} to {this.ManualFreeze}.");
    }

    /// <summary>Update the time settings for the given date.</summary>
    /// <param name="season">The current season.</param>
    /// <param name="dayOfMonth">The current day of month.</param>
    private void UpdateScaleForDay(Season season, int dayOfMonth)
    {
        this.AdjustTime = this.Config.ShouldScale(season, dayOfMonth);
    }

    /// <summary>Get the adjusted progress towards the next 10-game-minute tick.</summary>
    /// <param name="progress">The percentage of the clock tick interval (i.e. the interval between time changes) that elapsed since the last update tick.</param>
    /// <param name="newTickInterval">The clock tick interval to which to apply the progress.</param>
    private double ScaleTickProgress(double progress, int newTickInterval)
    {
        double ratio = this.TimeHelper.CurrentDefaultTickInterval / (newTickInterval * 1d); // ratio between the game's normal interval (e.g. 7000) and the player's custom interval
        return progress * ratio;
    }

    /// <summary>Get the freeze type which applies for the current context, ignoring overrides by the player.</summary>
    private AutoFreezeReason GetAutoFreezeType()
    {
        if (this.Config.ShouldFreeze(Game1.currentLocation))
            return AutoFreezeReason.FrozenForLocation;

        if (this.Config.ShouldFreezeBeforePassingOut(Game1.timeOfDay))
            return AutoFreezeReason.FrozenBeforePassOut;

        if (this.Config.ShouldFreeze(Game1.timeOfDay))
            return AutoFreezeReason.FrozenAtTime;

        return AutoFreezeReason.None;
    }

    /// <summary>Send a multiplayer message to the host player.</summary>
    /// <typeparam name="TMessage">The message type to send.</typeparam>
    /// <param name="message">The message to send.</param>
    private void SendMessageToHost<TMessage>(TMessage message)
        where TMessage : notnull
    {
        // check host info
        long hostPlayerId = Game1.MasterPlayer.UniqueMultiplayerID;
        IMultiplayerPeerMod? hostMod = this.Helper.Multiplayer.GetConnectedPlayer(hostPlayerId)?.GetMod(this.ModManifest.UniqueID);
        if (hostMod is null || hostMod.Version.IsOlderThan("2.8.0"))
        {
            this.Notifier.OnAccessDeniedFromHost(I18n.Message_HostMissingMod());
            return;
        }

        // send message
        string messageType = message.GetType().Name;
        this.Helper.Multiplayer.SendMessage(message, messageType, modIDs: [this.ModManifest.UniqueID], playerIDs: [hostPlayerId]);
    }

    /// <summary>Reject a request to control the time from a farmhand.</summary>
    /// <param name="actionLabel">A human-readable label for the attempted change (like 'toggle time freeze') for logged messages.</param>
    /// <param name="farmhandId">The farmhand who requested the change.</param>
    private void RejectRequestFromFarmhand(string actionLabel, long farmhandId)
    {
        string farmhandName = Game1.GetPlayer(farmhandId)?.Name ?? farmhandId.ToString();

        this.Monitor.Log($"Rejected request from {farmhandName} to {actionLabel}, because you disabled that in the mod options.", LogLevel.Info);

        this.Helper.Multiplayer.SendMessage(new RequestDeniedMessage(), nameof(RequestDeniedMessage), modIDs: [this.ModManifest.UniqueID], playerIDs: [farmhandId]);
    }
}
