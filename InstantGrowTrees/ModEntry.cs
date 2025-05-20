using cantorsdust.Common;
using InstantGrowTrees.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace InstantGrowTrees;

/// <summary>The entry class called by SMAPI.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Properties
    *********/
    /// <summary>The mod configuration.</summary>
    private ModConfig Config = null!; // set in Entry

    /// <summary>The number of days after a tree is fully grown until it reaches iridium quality.</summary>
    /// <remarks>Fruit trees increase in quality once per year, so iridium is 30 days * 4 seasons * 3 quality increases.</remarks>
    private const int FruitTreeIridiumDays = 360;


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        CommonHelper.RemoveObsoleteFiles(this, "InstantGrowTrees.pdb");

        this.Config = helper.ReadConfig<ModConfig>();
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
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
        GenericModConfigMenuIntegration.Register(this.ModManifest, this.Helper.ModRegistry, this.Monitor,
            getConfig: () => this.Config,
            reset: () => this.Config = new(),
            save: () => this.Helper.WriteConfig(this.Config)
        );
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        // When the player loads a saved game, or after the overnight save,
        // check for any trees that should be grown.
        this.GrowTrees();
    }

    /****
    ** Methods
    ****/
    /// <summary>Grow all trees eligible for growth.</summary>
    private void GrowTrees()
    {
        // collect config
        bool ageFruitTrees = this.Config.FruitTrees.InstantlyAge;
        bool growFruitTrees = this.Config.FruitTrees.InstantlyGrow;
        bool growOtherTrees = this.Config.NonFruitTrees.InstantlyGrow;
        if (!ageFruitTrees && !growFruitTrees && !growOtherTrees)
            return;

        // apply
        Utility.ForEachLocation(location =>
        {
            foreach (TerrainFeature feature in location.terrainFeatures.Values)
            {
                switch (feature)
                {
                    case FruitTree fruitTree:
                        if (growFruitTrees)
                            this.GrowFruitTree(fruitTree);
                        if (ageFruitTrees)
                            this.AgeFruitTree(fruitTree);
                        break;

                    case Tree tree:
                        if (growOtherTrees)
                            this.GrowTree(tree);
                        break;
                }
            }

            return true;
        });
    }

    /// <summary>Grow a tree if it's eligible for growth.</summary>
    /// <param name="tree">The tree to grow.</param>
    private void GrowTree(Tree tree)
    {
        // skip if already grown
        if (tree.growthStage.Value >= Tree.treeStage)
            return;

        // get max size here
        RegularTreeConfig config = this.Config.NonFruitTrees;
        int maxSize = !config.InstantlyGrowWhenInvalid
            ? tree.GetMaxSizeHere(config.InstantlyGrowInWinter)
            : Tree.treeStage;

        // grow tree
        if (maxSize > tree.growthStage.Value)
            tree.growthStage.Value = maxSize;
    }

    /// <summary>Grow a fruit tree if it's eligible for growth.</summary>
    /// <param name="tree">The tree to grow.</param>
    private void GrowFruitTree(FruitTree tree)
    {
        FruitTreeConfig config = this.Config.FruitTrees;

        // skip if already grown
        bool isGrown = tree.growthStage.Value >= FruitTree.treeStage;
        bool isMature = tree.daysUntilMature.Value <= 0;
        if (isGrown && isMature)
            return;

        // grow tree of not blocked
        if (config.InstantlyGrowWhenInvalid || !FruitTree.IsGrowthBlocked(tree.Tile, tree.Location))
        {
            if (!isGrown)
                tree.growthStage.Value = FruitTree.treeStage;
            if (!isMature)
                tree.daysUntilMature.Value = 0;
        }
    }

    /// <summary>Age a fruit tree if it's eligible for aging.</summary>
    /// <param name="tree">The tree to grow.</param>
    private void AgeFruitTree(FruitTree tree)
    {
        FruitTreeConfig config = this.Config.FruitTrees;

        // skip if can't be aged
        if (!config.InstantlyAge || tree.growthStage.Value < FruitTree.treeStage || tree.daysUntilMature.Value <= -ModEntry.FruitTreeIridiumDays)
            return;

        // age tree
        tree.daysUntilMature.Value = -ModEntry.FruitTreeIridiumDays;
    }
}
