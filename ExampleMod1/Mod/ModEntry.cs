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

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.GameLaunchedHandler;
            this.Helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;

        }


        /*********
        ** Private methods
        *********/
        private void GameLaunchedHandler(object sender, GameLaunchedEventArgs e)
        {
            var sc = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(InserterObject));
            CustomCraftingRecipe.CraftingRecipes.Add("Inserter", new InserterRecipe());
        }


        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
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
    }
}
