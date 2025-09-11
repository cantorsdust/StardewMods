using StardewModdingAPI;
using StardewValley;
using TimeSpeed.Framework.Messages;

namespace TimeSpeed.Framework;

/// <summary>Displays messages to the user in-game.</summary>
internal class Notifier
{
    /*********
    ** Fields
    *********/
    /// <summary>The multiplayer API with which to notify farmhands.</summary>
    private readonly IMultiplayerHelper Multiplayer;

    /// <summary>The mod IDs which should receive multiplayer notifications.</summary>
    private readonly string[] ModId;

    /// <summary>The monitor to which to log local info.</summary>
    private readonly IMonitor Monitor;


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="multiplayer"><inheritdoc cref="Multiplayer" path="/summary"/></param>
    /// <param name="modId"><inheritdoc cref="ModId" path="/summary"/></param>
    /// <param name="monitor"><inheritdoc cref="Monitor" path="/summary"/></param>
    public Notifier(IMultiplayerHelper multiplayer, string modId, IMonitor monitor)
    {
        this.Multiplayer = multiplayer;
        this.ModId = [modId];
        this.Monitor = monitor;
    }

    /// <summary>Send notifications when the tick interval changes.</summary>
    /// <param name="newInterval">The new tick interval.</param>
    public void OnSpeedChanged(int newInterval)
    {
        this.QuickNotify(I18n.Message_SpeedChanged(seconds: newInterval / 1000));

        this.Monitor.Log($"Tick length set to {newInterval / 1000d:0.##} seconds.", LogLevel.Info);

        this.SendMessageToFarmhands(new NotifyTickIntervalChangedMessage { NewInterval = newInterval });
    }

    /// <summary>Send notifications when time is frozen or unfrozen.</summary>
    /// <param name="frozen">Whether time is now frozen (true) or unfrozen (false).</param>
    /// <param name="logMessage">A log message which explains why the time freeze changed, or <c>null</c> for a generic time frozen/unfrozen message.</param>
    public void OnTimeFreezeToggled(bool frozen, string logMessage = null)
    {
        if (frozen)
        {
            this.QuickNotify(I18n.Message_TimeStopped());
            this.Monitor.Log(logMessage ?? "Time is frozen globally.", LogLevel.Info);
        }
        else
        {
            this.QuickNotify(I18n.Message_TimeResumed());
            this.Monitor.Log(logMessage ?? "Time has resumed.", LogLevel.Info);
        }

        this.SendMessageToFarmhands(new NotifyFreezeChangedMessage { IsFrozen = frozen });
    }

    /// <summary>Send notifications when the local player enters a location.</summary>
    /// <param name="isTimeFrozen">Whether time is currently frozen.</param>
    /// <param name="tickInterval">The current tick interval.</param>
    /// <param name="freezeReason">The freeze reason, if time is frozen.</param>
    public void OnLocalLocationChanged(bool isTimeFrozen, int tickInterval, AutoFreezeReason freezeReason)
    {
        switch (freezeReason)
        {
            case AutoFreezeReason.FrozenAtTime when isTimeFrozen:
                this.ShortNotify(I18n.Message_OnLocationChange_TimeStoppedGlobally());
                break;

            case AutoFreezeReason.FrozenForLocation when isTimeFrozen:
                this.ShortNotify(I18n.Message_OnLocationChange_TimeStoppedHere());
                break;

            default:
                this.ShortNotify(I18n.Message_OnLocationChange_TimeSpeedHere(seconds: tickInterval / 1000));
                break;
        }
    }

    /// <summary>Send notifications when the local configuration is reloaded.</summary>
    public void OnConfigReloaded()
    {
        this.ShortNotify(I18n.Message_ConfigReloaded());
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Display a message for one second.</summary>
    /// <param name="message">The message to display.</param>
    private void QuickNotify(string message)
    {
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 1000 });
    }

    /// <summary>Display a message for two seconds.</summary>
    /// <param name="message">The message to display.</param>
    private void ShortNotify(string message)
    {
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type) { timeLeft = 2000 });
    }

    /// <summary>Send a multiplayer message to all connected players if the local player is the host.</summary>
    /// <typeparam name="TMessage">The message type to send.</typeparam>
    /// <param name="message">The message to send.</param>
    private void SendMessageToFarmhands<TMessage>(TMessage message)
    {
        if (Context.IsMainPlayer)
        {
            string messageType = message.GetType().Name;

            this.Multiplayer.SendMessage(message, messageType, modIDs: this.ModId);
        }
    }
}
