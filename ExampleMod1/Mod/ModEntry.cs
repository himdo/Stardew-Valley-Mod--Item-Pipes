using System;
using System.IO;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common.Integrations.JsonAssets;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using SpaceShared;
using SpaceShared.APIs;
using StardewValley.Objects;
//using Pathoschild.Stardew.Automate.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using SpaceCore;
//using Pathoschild.Stardew.Automate;

namespace ExampleMod1
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        //private static IJsonAssetsApi Ja;
        public static Mod Instance;
        public static IMonitor _Monitor;

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

            //helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.GameLaunchedHandler;
            this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameLaunchedHandler(object sender, GameLaunchedEventArgs e)
        {
            //IAutomateAPI automate = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            //Ja = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            //Ja.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
            var sc = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(InserterObject));
            CustomCraftingRecipe.CraftingRecipes.Add("Inserter", new InserterRecipe());
        }


        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            //ModEntry.AddDefaultRecipes();
            _Monitor.Log($"${Game1.player.craftingRecipes}");
            foreach (string key in Game1.player.craftingRecipes.Keys)
            {
                _Monitor.Log($"${key}", LogLevel.Debug);
            }
            _Monitor.Log($"Adding Recipe to {Game1.player.Name}.", LogLevel.Debug);
            // TODO Figure out why this doesn't work
            string RecipeName = "Inserter";
            //string RecipeName = "Stone Chest";
            bool isAlreadyKnown = Game1.player.craftingRecipes.ContainsKey(RecipeName);
            if (!isAlreadyKnown)
            {
                Game1.player.craftingRecipes.Add(RecipeName, 0);
            }
            _Monitor.Log($"Finished Adding Recipe to {Game1.player.Name}.", LogLevel.Debug);
            foreach (string key in Game1.player.craftingRecipes.Keys)
            {
                _Monitor.Log($"${key}", LogLevel.Debug);
            }

            foreach (string key in Game1.player.craftingRecipes.Keys)
            {
                _Monitor.Log($"${key}", LogLevel.Debug);
            }
        }

        public static void AddDefaultRecipes()
        {
            //List<string> recipesToAdd = new List<string>();

            //int[] eventsSeen = Game1.player.eventsSeen.ToArray();
            //string precondition = $"{ModEntry.EventRootId}/{ModEntry.EventData[0]["Conditions"]}";
            //int rootEventReady = Game1.getFarm().checkEventPrecondition(precondition);
            //bool hasOrWillSeeRootEvent = eventsSeen.Contains(ModEntry.EventRootId) || rootEventReady != -1;
            //for (int i = 0; i < ModEntry.ItemDefinitions.Count; ++i)
            //{
            //    string variantKey = ModEntry.ItemDefinitions.Keys.ElementAt(i);
            //    string craftingRecipeName = OutdoorPot.GetNameFromVariantKey(variantKey: variantKey);
            //    bool isAlreadyKnown = Game1.player.craftingRecipes.ContainsKey(craftingRecipeName);
            //    bool isDefaultRecipe = ModEntry.ItemDefinitions[variantKey].RecipeIsDefault;
            //    bool isInitialEventRecipe = string.IsNullOrEmpty(ModEntry.ItemDefinitions[variantKey].RecipeConditions);
            //    bool shouldAdd = ModEntry.Config.RecipesAlwaysAvailable || isDefaultRecipe || (hasOrWillSeeRootEvent && isInitialEventRecipe);

            //    if (!isAlreadyKnown && shouldAdd)
            //    {
            //        recipesToAdd.Add(craftingRecipeName);
            //    }
            //}

            //string craftingRecipeName = InserterObject.GetNameFromVariantKey("");
            //bool isAlreadyKnown = Game1.player.craftingRecipes.ContainsKey(craftingRecipeName);
            //if (!isAlreadyKnown)
            //{
            //    recipesToAdd.Add(craftingRecipeName);
            //}

            //if (recipesToAdd.Count > 0)
            //{
            //    //_Monitor.Log($"Adding {recipesToAdd.Count} default recipes:{recipesToAdd.Aggregate(string.Empty, (str, s) => $"{str}{Environment.NewLine}{s}")}", LogLevel.Debug);

            //    for (int i = 0; i < recipesToAdd.Count; ++i)
            //    {
            //        Game1.player.craftingRecipes.Add(recipesToAdd[i], 0);
            //    }
            //}
        }


        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            if (e.Button.ToString().ToLower() == "n")
            {
                Game1.player.addItemToInventory(new InserterObject(Vector2.Zero));
            }
            // print button presses to the console window
            _Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        //private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        //{
        //    // ignore if player hasn't loaded a save yet
        //    if (!Context.IsWorldReady)
        //        return;
        //    // print button presses to the console window
        //    this.Monitor.Log($"{Game1.player.Name} has changed Inventory: {e.QuantityChanged}", LogLevel.Debug);

        //    foreach (Item item in e.Removed)
        //    {
        //        this.Monitor.Log($"{Game1.player.Name} Removed: {item.Name} x {item.Stack}");
        //    }
        //    foreach (Item item in e.Added)
        //    {
        //        this.Monitor.Log($"{Game1.player.Name} Added: {item.Name} x {item.Stack}", LogLevel.Debug);
        //    }
        //}
    }
}
