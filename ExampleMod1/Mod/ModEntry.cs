
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceShared;
using SpaceShared.APIs;
using StardewValley;
using SpaceCore;
using SpaceCore.Framework;
//using CustomCraftingRecipeFramework = SpaceCore.Framework.CustomCraftingRecipe;
using CustomCraftingRecipeCore = SpaceCore.CustomCraftingRecipe;
using System.Collections.Generic;
using StardewValley.Menus;

namespace ExampleMod1
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        //private static IJsonAssetsApi Ja;
        public static Mod Instance;
        public static IMonitor _Monitor;
        internal static IJsonAssetsApi Ja;
        internal static List<CustomCraftingRecipeCore> customCraftingRecipes;

        //internal static Dictionary<string, ItemDefinition> ItemDefinitions = null;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            ModEntry._Monitor = this.Monitor;
            helper.Events.GameLoop.GameLaunched += this.GameLaunchedHandler;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            //helper.Events.Content.AssetRequested += this.OnAssetRequested;

        }


        /*********
        ** Private methods
        *********/
        private void GameLaunchedHandler(object sender, GameLaunchedEventArgs e)
        {
            //_Monitor.Log($"ModEntry.Ja is starting INIT", LogLevel.Debug);
            //ModEntry.Ja = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            //_Monitor.Log($"ModEntry.Ja is INIT", LogLevel.Debug);

            var sc = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(InserterObject));
            //customCraftingRecipes.Add(new InserterRecipe());
            //CustomCraftingRecipeCore.CraftingRecipes.Add("Inserter", new InserterRecipe());
            //CustomCraftingRecipeCore.CraftingRecipes.Add("Inserter", new CustomCraftingRecipe("Inserter", false,new InserterRecipe()));

            //CraftingRecipe.craftingRecipes = content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
        }


        /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shop)
            {
                if (shop.portraitPerson?.Name == "Robin")
                {
                    var inserter = new InserterObject(Vector2.Zero);
                    shop.forSale.Add(inserter);
                    shop.itemPriceAndStock.Add(inserter, new[] { 1500, int.MaxValue });
                }
            }
        }


        //private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        //{

        //    _Monitor.Log($"In AssetRequested Locale After: ${e.NameWithoutLocale}", LogLevel.Debug);
        //    if (e.NameWithoutLocale.IsEquivalentTo("Data\\CraftingRecipes"))
        //    {
        //        _Monitor.Log($"e.NameWithoutLocale.IsEquivalentTo", LogLevel.Debug);
        //        //e.Edit(static asset =>
        //        //e.Edit(asset =>
        //        //{
        //        //    var dict = asset.AsDictionary<string, string>().Data;

        //        //    _Monitor.Log($" e.Edit(asset: ${dict}", LogLevel.Debug);
        //        //    //dict.Add($"0 1/meow/0 1/true/{null} {0}/asdsad");
        //        //    //dict.Add("Inserter", $"{InserterObject} 5/Field/434/false/null/Inserter");
        //        //    //dict.Add("Inserter Recipe", $"{ModEntry.Ja.GetInserterObjectId("Frosty Stardrop Piece")} 5/Field/434/false/null/{I18n.Recipe_FrostyStardrop_Name()}");

        //        //});

        //        e.Edit((asset) =>
        //        {
        //            CraftingRecipePackData test = new InserterRecipe2();

        //            var crecipe = new DGACustomCraftingRecipe(test);
        //            _Monitor.Log($"e.NameWithoutLocale.IsEquivalentTo: ${crecipe.data.CraftingDataKey}", LogLevel.Debug);
        //            var dict = asset.AsDictionary<string, string>().Data;
        //            dict.Add(crecipe.data.CraftingDataKey, crecipe.data.CraftingDataValue);
        //            //dict.Add(new CustomCraftingRecipeFramework("Inserter",false,null);

        //            //var dict = asset.AsDictionary<string, string>().Data;
        //            //int i = 0;
        //            //foreach (var crecipe in ModEntry.customCraftingRecipes)
        //            //{
        //            //    if (crecipe.data.Enabled && !crecipe.data.IsCooking)
        //            //    {
        //            //        dict.Add(crecipe.data.CraftingDataKey, crecipe.data.CraftingDataValue);
        //            //        ++i;
        //            //    }
        //            //}
        //        });
        //    }

        //    //_Monitor.Log($"Finished AssetRequested", LogLevel.Debug);

        //}


        //private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        //{
        // TODO Figure out why this doesn't work
        //string RecipeName = "Inserter";
        //string RecipeName = "Stone Chest";
        //bool isAlreadyKnown = Game1.player.craftingRecipes.ContainsKey(RecipeName);
        //if (!isAlreadyKnown)
        //{
        //    Game1.player.craftingRecipes.Add(RecipeName, 0);
        //}
        //}


        ///// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        ///// <param name="sender">The event sender.</param>
        ///// <param name="e">The event data.</param>
        //private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        //{
        //    // ignore if player hasn't loaded a save yet
        //    if (!Context.IsWorldReady)
        //        return;
        //    if (e.Button.ToString().ToLower() == "n")
        //    {
        //        Game1.player.addItemToInventory(new InserterObject(Vector2.Zero));
        //    }
        //    // print button presses to the console window
        //    _Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        //}
    }
}
