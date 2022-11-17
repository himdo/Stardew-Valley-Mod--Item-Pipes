using System;
using System.Xml.Serialization;
using ExampleMod1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace ExampleMod1
{
    [XmlType("Mods_himdo_Inserter")]
    public class InserterObject : SObject // must be public for the XML serializer
    {
        /*********
        ** Fields
        *********/
        private static readonly Texture2D Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/InserterUpToDown.png");


        /*********
        ** Accessors
        *********/

        public override string DisplayName
        {
            get => this.name;
            set { }
        }


        /*********
        ** Public methods
        *********/
        public InserterObject() { }

        public InserterObject(Vector2 placement)
        {
            this.name = this.loadDisplayName();
            this.DisplayName = this.loadDisplayName();
            this.bigCraftable.Value = true;
            this.Type = "Crafting"; // Makes performObjectDropInAction work for non-objects

            this.TileLocation = placement;
            this.boundingBox.Value = new Rectangle((int)placement.X * 64, (int)placement.Y * 64, 64, 64);
        }

        public override string getDescription()
        {
            return "This is a test description";
        }

        public override bool canStackWith(ISalable other)
        {
            return other is InserterObject;
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 placementTile = new Vector2(x / Game1.tileSize, y / Game1.tileSize);
            var i = new InserterObject(placementTile);
            location.Objects.Add(placementTile, i);
            location.playSound("woodyStep");
            return true;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t == null)
                return false;

            if (t is MeleeWeapon || !t.isHeavyHitter())
            {
                return false;
            }

                
            location.objects.Remove(this.TileLocation);
            this.DropItem(location, new InserterObject(Vector2.Zero));
            location.playSound("woodyStep");
            return false;
        }


        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            var tex = this.GetMainTexture();

            spriteBatch.Draw(tex, objectPosition, null, Color.White, 0, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && scaleSize > 0.3 && this.Stack != int.MaxValue;

            var tex = this.GetMainTexture();

            spriteBatch.Draw(tex, location + new Vector2(32f, 32f), null, color * transparency, 0f, new Vector2(8f, 16f), 4f * (scaleSize < 0.2 ? scaleSize : (scaleSize / 2f)), SpriteEffects.None, layerDepth);
            if (shouldDrawStackNumber)
            {
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize) + 3f * scaleSize, 64f - 18f * scaleSize + 2f), 3f * scaleSize, 1f, color);
            }
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            var tex = this.GetMainTexture();

            Vector2 scaleFactor = this.getScale();
            scaleFactor *= Game1.pixelZoom;
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile, yNonTile));
            //Rectangle destination = new Rectangle(
            //    x: (int)(position.X - scaleFactor.X / 2f) + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
            //    y: (int)(position.Y - scaleFactor.Y / 2f) + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
            //    width: (int)(Game1.tileSize + scaleFactor.X),
            //    height: (int)((Game1.tileSize * 2) + scaleFactor.Y / 2f)
            //);
            Rectangle destination = new Rectangle(
                x: (int)(position.X - scaleFactor.X / 2f),
                y: (int)(position.Y - scaleFactor.Y / 2f),
                width: (int)(Game1.tileSize + scaleFactor.X),
                height: (int)((Game1.tileSize * 2) + scaleFactor.Y / 2f)
            );
            spriteBatch.Draw(tex, destination, null, Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
            //spriteBatch.Draw(tex, destination, null, Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            // draw mannequin
            float drawLayer = Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f;
            this.draw(spriteBatch, xNonTile: x * Game1.tileSize, yNonTile: y * Game1.tileSize - Game1.tileSize, layerDepth: drawLayer, alpha: alpha);
        }
        public static string GetNameFromVariantKey(string variantKey)
        {
            return $"Inserter";
        }

        /// <summary>Drop an item onto the ground near the mannequin.</summary>
        /// <param name="location">The location containing the mannequin.</param>
        /// <param name="item">The item to drop.</param>
        private void DropItem(GameLocation location, Item item)
        {
            var position = new Vector2((this.TileLocation.X + 0.5f) * Game1.tileSize, (this.TileLocation.Y + 0.5f) * Game1.tileSize);
            location.debris.Add(new Debris(item, position));
        }

        ///*********
        //** Protected methods
        //*********/

        protected override string loadDisplayName()
        {
            return "Inserter";
        }

        /// <summary>Get the main mannequin texture to render.</summary>
        private Texture2D GetMainTexture()
        {
            return InserterObject.Tex;
        }
    }

    public class InserterRecipe : CustomCraftingRecipe // must be public for the XML serializer
    {
        public override string Description => "This is a test Description";

        public override string Name => "Inserter";

        public override Texture2D IconTexture => ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/InserterUpToDown.png");

        public override Rectangle? IconSubrect => null;

        public override IngredientMatcher[] Ingredients => new[] { new ObjectIngredientMatcher(SObject.wood, 1), new ObjectIngredientMatcher(SObject.stone, 1) };

        public override Item CreateResult()
        {
            return new InserterObject(Vector2.Zero);
        }
    }
}