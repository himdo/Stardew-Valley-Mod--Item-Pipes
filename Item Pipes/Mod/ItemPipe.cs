using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

using System.Collections.Generic;
using StardewModdingAPI;
using ItemPipes.ItemPipeUI;

namespace ItemPipes.ItemPipeObject
{

    public enum Directions
    {
        NorthToSouth = 0,
        EastToWest = 1,
        SouthToNorth = 2,
        WestToEast = 3,
    }


    [XmlType("Mods_himdo_ItemPipe")]
    public class ItemPipe : SObject // must be public for the XML serializer
    {
        /*********
        ** Fields
        *********/
        private Texture2D Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeUpToDown.png");
        public readonly NetInt FacingDirection = new NetInt((int)Directions.NorthToSouth);
        public List<Item> WhiteListItems = new List<Item>();


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
        public ItemPipe() { }

        public ItemPipe(Vector2 placement)
        {
            this.name = this.loadDisplayName();
            this.DisplayName = this.loadDisplayName();
            this.bigCraftable.Value = true;
            this.Type = "Crafting"; // Makes performObjectDropInAction work for non-objects
            this.Price = 1500;

            this.TileLocation = placement;
            this.boundingBox.Value = new Rectangle((int)placement.X * 64, (int)placement.Y * 64, 64, 64);
        }

        public ItemPipe(Directions direction, Vector2 placement)
        {
            this.name = this.loadDisplayName();
            this.DisplayName = this.loadDisplayName();
            this.bigCraftable.Value = true;
            this.Type = "Crafting"; // Makes performObjectDropInAction work for non-objects

            this.TileLocation = placement;
            this.boundingBox.Value = new Rectangle((int)placement.X * 64, (int)placement.Y * 64, 64, 64);
            ChangeDirection(direction);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            if (!environment.objects.TryGetValue(this.TileLocation - GetFromChestVector(), out SObject objFrom) || !(objFrom is Chest chestFrom))
            {
                return false;
            }
            if (!environment.objects.TryGetValue(this.TileLocation + GetFromChestVector(), out SObject objTo) || !(objTo is Chest chestTo))
            {
                return false;
            }

            chestFrom.clearNulls();
            for (int i = 0; i < minutes; i++)
            {
                bool movedItem = MoveOneItem(chestFrom, chestTo);
                if (!movedItem) break;

            }
            return false;
        }

        public override Item getOne()
        {
            var ret = new ItemPipe( Vector2.Zero);
            ret._GetOneFrom(this);
            return ret;
        }
        private bool MoveOneItem(Chest chestFrom, Chest chestTo)
        {
            chestFrom.clearNulls();
            for (int i = 0; i < chestFrom.items.Count; i++)
            {
                Item item = chestFrom.items[i];
                int itemStack = item.Stack;
                Item itemOne = item.getOne();
                bool canMoveItem = WhiteListItems.Count == 0;
                if (!canMoveItem)
                {
                    for (int itemCount = 0; itemCount < WhiteListItems.Count; itemCount++)
                    {
                        Item whiteListItem = WhiteListItems[itemCount];
                        if (itemOne.canStackWith(whiteListItem) || (itemOne.ParentSheetIndex == whiteListItem.ParentSheetIndex && (item as SObject).quality.Value == (whiteListItem as SObject).quality.Value))
                        {
                            canMoveItem = true;
                            break;
                        }
                    }
                }
                if (canMoveItem)
                {
                    Item movedTo = chestTo.addItem(itemOne);

                    if (movedTo == null)
                    {
                        if (itemStack == 1)
                        {
                            chestFrom.items.RemoveAt(i);
                        }
                        else
                        {
                            chestFrom.items.RemoveAt(i);
                            item.Stack = itemStack - 1;
                            chestFrom.items.Insert(i, item);
                        }

                        return true;
                    }
                }
                
            }
            return false;
        }

        private Vector2 GetFromChestVector()
        {
            Vector2 FromDirection = new Vector2();
            switch (FacingDirection)
            {
                case (int)Directions.NorthToSouth:
                    FromDirection = new Vector2(0, 1);
                    break;
                case (int)Directions.SouthToNorth:
                    FromDirection = new Vector2(0, -1);
                    break;
                case (int)Directions.EastToWest:
                    FromDirection = new Vector2(-1, 0);
                    break;
                case (int)Directions.WestToEast:
                    FromDirection = new Vector2(1, 0);
                    break;
            }
            return FromDirection;
        }

        public override string getDescription()
        {
            return "This Pipe moves items from one chest to another!";
        }

        public override bool canStackWith(ISalable other)
        {
            return other is ItemPipe;
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 placementTile = new Vector2(x / Game1.tileSize, y / Game1.tileSize);
            var i = new ItemPipe(placementTile);
            location.Objects.Add(placementTile, i);
            location.playSound("woodyStep");
            return true;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t == null)
            {
                return false;
            }

            if (t is MeleeWeapon || !t.isHeavyHitter())
            {
                return false;
            }

                
            location.objects.Remove(this.TileLocation);
            this.DropItem(location, new ItemPipe(Vector2.Zero));
            location.playSound("woodyStep");

            return false;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {

            if (justCheckingForActivity)
            {
                return true;
            }
            if (!Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
            {
                return false;
            }

            Game1.activeClickableMenu = new ItemPipeCustomUI(this);
            return true;
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
            Rectangle destination = new Rectangle(
                x: (int)(position.X - scaleFactor.X / 2f),
                y: (int)(position.Y - scaleFactor.Y / 2f),
                width: (int)(Game1.tileSize + scaleFactor.X),
                height: (int)((Game1.tileSize * 2) + scaleFactor.Y / 2f)
            );
            spriteBatch.Draw(tex, destination, null, Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            float drawLayer = Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f;
            this.draw(spriteBatch, xNonTile: x * Game1.tileSize, yNonTile: y * Game1.tileSize - Game1.tileSize, layerDepth: drawLayer, alpha: alpha);
        }
        public static string GetNameFromVariantKey(string variantKey)
        {
            return "Item Pipe";
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

        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddFields(this.FacingDirection);

            this.FacingDirection.fieldChangeEvent += this.OnNetFieldChanged;
        }

        private void OnNetFieldChanged<TNetField, TValue>(TNetField field, TValue oldValue, TValue newValue)
        {
            //this.FarmerForRenderingCache = null;
            //ModEntry._Monitor.Log($"${field} changed from ${oldValue}, to ${newValue}",LogLevel.Debug);
            this.SetTextureForDirection();
        }
        protected override string loadDisplayName()
        {
            return "Item Pipe";
        }

        /// <summary>Get the main mannequin texture to render.</summary>
        private Texture2D GetMainTexture()
        {
            return this.Tex;
        }
        public void ChangeDirection(Directions newDirection)
        {
            this.FacingDirection.Set((int) newDirection);
            this.SetTextureForDirection();
        }
        private void SetTextureForDirection()
        {
            switch (this.FacingDirection)
            {
                case (int)Directions.NorthToSouth:
                    Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeUpToDown.png");
                    break;
                case (int)Directions.SouthToNorth:
                    Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeDownToUp.png");
                    break;
                case (int)Directions.EastToWest:
                    Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeRightToLeft.png");
                    break;
                case (int)Directions.WestToEast:
                    Tex = ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeLeftToRight.png");
                    break;
                default:
                    break;
            }
        }
    }

    //public class PipeRecipe : CustomCraftingRecipe // must be public for the XML serializer
    //{
    //    public override string Description => "This is a test Description";

    //    public override string Name => "Pipe Recipe";

    //    public override Texture2D IconTexture => ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeUpToDown.png");

    //    public override Rectangle? IconSubrect => null;

    //    public override IngredientMatcher[] Ingredients => new[] { new ObjectIngredientMatcher(388, 1) }; // , new ObjectIngredientMatcher(SObject.stone, 1)

    //    //public CraftingRecipe NameWithoutLocale = new CraftingRecipe("Pipe", false);
    //    public override Item CreateResult()
    //    {
    //        return new ItemPipe(Vector2.Zero);
    //    }
    //}



    //public class PipeRecipe2 : CraftingRecipePackData // must be public for the XML serializer
    //{
    //    public string Description => "This is a test Description";

    //    public string Name => "Pipe Recipe";

    //    public Texture2D IconTexture => ModEntry.Instance.Helper.ModContent.Load<Texture2D>("assets/PipeUpToDown.png");

    //    public Rectangle? IconSubrect => null;

    //    public IngredientMatcher[] Ingredients => new[] { new ObjectIngredientMatcher(388, 1) }; // , new ObjectIngredientMatcher(SObject.stone, 1)

    //    public string ID = "997";

    //    //public CraftingRecipe NameWithoutLocale = new CraftingRecipe("Pipe", false);
    //    //public Item CreateResult()
    //    //{
    //    //    return new ItemPipe(Vector2.Zero);
    //    //}
    //}
}