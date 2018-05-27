using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllCropsAllSeasons.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace AllCropsAllSeasons
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod, IAssetEditor
    {

        /*********
        ** Properties
        *********/
        /// <summary>The crop tiles which should be saved for the next day.</summary>
        private CropTileState[] SavedCrops = new CropTileState[0];

        /// <summary>Configuration payload.</summary>
        private ModConfig Config;

        /// <summary>All Game locations.</summary>
        private GameLocation[] Locations;
        private bool IsLoaded => Context.IsWorldReady && this.Locations != null;



        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            PlayerEvents.Warped += this.PlayerEvents_Warped;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals("Data/Crops") // change crop seasons
                || asset.AssetNameEquals("TerrainFeatures/hoeDirtSnow"); // change dirt texture
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // change crop seasons; based on user config
            StringBuilder cropseason = new StringBuilder();
            if (asset.AssetNameEquals("Data/Crops"))
            {
                asset
                    .AsDictionary<int, string>()
                    .Set((id, data) =>
                    {
                        string[] fields = data.Split('/');
                        cropseason.Clear();
                        if (fields[1].Contains("spring") || this.Config.CropGrowSpring)
                            cropseason.Append("spring");
                        if (fields[1].Contains("summer") || this.Config.CropGrowSummer)
                            cropseason.Append(" summer");
                        if (fields[1].Contains("fall") || this.Config.CropGrowFall)
                            cropseason.Append(" fall");
                        if (fields[1].Contains("winter") || this.Config.CropGrowWinter)
                            cropseason.Append(" winter");
                        fields[1] = cropseason.ToString();
                        fields[1] = fields[1].Trim();
                        return string.Join("/", fields);
                    });
            }

            // change dirt texture
            else if (asset.AssetNameEquals("TerrainFeatures/hoeDirtSnow") && this.Config.WinterHoeSnow) //Allows users to set plowed snow or dirt in winter
                asset.ReplaceWith(this.Helper.Content.Load<Texture2D>("TerrainFeatures/hoeDirt", ContentSource.GameContent));
        }


        /// <summary>Water All Fields.</summary> //Thanks CJB!
        public void WaterAllFields(GameLocation[] locations)
        {
            foreach (GameLocation location in locations)
            {
                if (!location.IsFarm && !location.Name.Contains("Greenhouse"))
                    continue;

                foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
                {
                    if (terrainFeature is HoeDirt dirt)
                        dirt.state.Value = HoeDirt.watered;
                }


            }
        }

        /// <summary>Get all game locations.</summary> //Thanks CJB!
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            foreach (GameLocation location in Game1.locations)
            {
                // current location
                yield return location;
            }
        }

        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/

        /// <summary>The method called after the day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            this.Locations = GetAllLocations().ToArray();
            if (this.Config.WaterCropSnow && Game1.isSnowing)
                this.WaterAllFields(Locations);
        }

        /// <summary>The method called when the player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerEvents_Warped(object sender, EventArgsPlayerWarped e)
        {
            // when player enters farmhouse (including on new day), back up crops in case they're
            // about to end the day
            if (e.NewLocation is FarmHouse)
                this.StashCrops();
        }

        /// <summary>The method called when the game is writing to the save file.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            //If winter is disabled by user, do not restore crops thus game acting natively
            //If not winter, mod acts normal
            if (this.Config.CropGrowWinter || Game1.currentSeason != "winter")
                // before save (but after tomorrow's day updates), fix any crops that died due to the day update
                this.RestoreCrops();
        }

        /****
        ** Methods
        ****/
        /// <summary>Take a snapshot of the current crops.</summary>
        private void StashCrops()
        {
            Farm farm = Game1.getFarm();
            this.SavedCrops = this.GetCropTiles(farm).ToArray();
        }

        /// <summary>Restore the temporarily-saved crops.</summary>
        private void RestoreCrops()
        {
            // get data
            CropTileState[] crops = this.SavedCrops;
            if (!crops.Any())
                return;
            Farm farm = Game1.getFarm();
            GameLocation greenhouse = Game1.getLocationFromName("Greenhouse");

            // ignore crops converted into giant crops
            {
                HashSet<Vector2> coveredByGiantCrop = new HashSet<Vector2>(this.GetGiantCropTiles(farm));
                crops = crops.Where(crop => !coveredByGiantCrop.Contains(crop.Tile)).ToArray();
            }

            // restore crops
            foreach (CropTileState saved in crops)
            {
                // get actual tile
                if (!farm.terrainFeatures.ContainsKey(saved.Tile) || !(farm.terrainFeatures[saved.Tile] is HoeDirt))
                    farm.terrainFeatures[saved.Tile] = new HoeDirt(); // reset dirt tile if needed (e.g. to clear debris)
                HoeDirt dirt = (HoeDirt)farm.terrainFeatures[saved.Tile];

                // reset crop tile if needed
                if (dirt.crop == null || dirt.crop.dead.Value)
                {
                    // reset values changed by day update
                    if (dirt.state.Value != HoeDirt.watered)
                        dirt.state.Value = saved.State;
                    dirt.fertilizer.Value = saved.Fertilizer;
                    dirt.crop = saved.Crop;
                    dirt.crop.dead.Value = false;
                    dirt.dayUpdate(greenhouse, saved.Tile);
                }
            }
        }

        /// <summary>Get all tiles on the farm with a live crop.</summary>
        /// <param name="farm">The farm to search.</param>
        private IEnumerable<CropTileState> GetCropTiles(Farm farm)
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> entry in farm.terrainFeatures.Pairs)
            {
                Vector2 tile = entry.Key;
                HoeDirt dirt = entry.Value as HoeDirt;
                Crop crop = dirt?.crop;
                if (crop != null && !crop.dead.Value)
                    yield return new CropTileState(tile, crop, dirt.state.Value, dirt.fertilizer.Value);
            }
        }

        /// <summary>Get all tiles on the farm with a giant crop.</summary>
        /// <param name="farm">The farm to search.</param>
        private IEnumerable<Vector2> GetGiantCropTiles(Farm farm)
        {
            foreach (GiantCrop giantCrop in farm.resourceClumps.OfType<GiantCrop>())
            {
                Vector2 tile = giantCrop.tile.Value;

                yield return tile; // top left tile
                yield return tile + new Vector2(1, 0);
                yield return tile + new Vector2(0, 1);
                yield return tile + new Vector2(1, 1);
            }
        }
    }
}
