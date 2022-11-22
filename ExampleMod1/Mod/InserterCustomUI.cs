using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Minigames;
using System.Reflection;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Tools;

namespace ExampleMod1
{
    public class InserterCustomUI : MenuWithInventory
    {
        private RootElement ui;
        private Table table;
        private InserterObject inserterInstance;
        IList<Item> WhiteListItems;
        private int heightOffset = -120;

        public InserterCustomUI() : base(null, okButton: false, trashCan: false, 0,0)//12, 132)//: base((Game1.uiViewport.Width - 900) / 2, (Game1.uiViewport.Height - (Game1.uiViewport.Height - 100)) / 2, 900, (Game1.uiViewport.Height - 100))
        {
            WhiteListItems = new List<Item>();
            //int num2 = 0;
            //int actualCapacity = WhiteListItems.Count + 1;
            //int rows = 3;
            //ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num2 / 2, yPositionOnScreen + 64, playerInventory: false, WhiteListItems, InventoryMenu.highlightAllItems, actualCapacity, rows);
            ReCreateUI();
        }
        
        private void ReCreateUI()
        {
            ui = new RootElement()
            {
                LocalPosition = new Vector2(this.xPositionOnScreen, this.yPositionOnScreen)
            };

            var title = new Label()
            {
                String = "Inserter Configuration",
                Bold = true,
            };
            title.LocalPosition = new Vector2((width - title.Width) / 2, 10 + heightOffset);
            ui.AddChild(title);

            //table = new Table()
            //{
            //    RowHeight = (128 - 16) / 8,
            //    Size = new Vector2(700, height - 200),
            //    LocalPosition = new Vector2(50, 100),
            //};

            //for (int i = 0; i < 3; i ++)
            //{

            //    List<Element> rowSlots = new List<Element>();
            //    //table.AddRow(rowSlots.ToArray());
            //    var rowElement = new Label()
            //    {
            //        String = $"This is a table Item ${i}",
            //        Bold = false,
            //    };
            //    rowElement.LocalPosition = new Vector2(150, 150 * (1 + i));
            //    rowSlots.Add(rowElement);
            //    table.AddRow(rowSlots.ToArray());
            //    // This appears to only put blank spaces
            //    for (int j = 0; j < 2; ++j)
            //        table.AddRow(new Element[0]);
            //}
            //ui.AddChild(table);

            var DirectionText = new Label()
            {
                String = "Direction:",
                Bold = true,
            };
            DirectionText.LocalPosition = new Vector2(15, 100 + heightOffset);
            ui.AddChild(DirectionText);

            int moveOverConstant = 300;
            var SouthToNorth = new Label()
            {
                String = "Up",
                //String = "↑",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.SouthToNorth : false,
                Callback = (e) => SetInserterDirection(Directions.SouthToNorth),
            };
            SouthToNorth.LocalPosition = new Vector2((width - SouthToNorth.Width) / 2 - moveOverConstant, 150 + heightOffset);
            ui.AddChild(SouthToNorth);

            var EastToWest = new Label()
            {
                String = "Left",
                //String = "→",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.EastToWest : false,
                Callback = (e) => SetInserterDirection(Directions.EastToWest),
            };
            EastToWest.LocalPosition = new Vector2((width - EastToWest.Width) / 2 - 70 - moveOverConstant, 200 + heightOffset);
            ui.AddChild(EastToWest);
            var WestToEast = new Label()
            {
                String = "Right",
                //String = "←",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.WestToEast : false,
                Callback = (e) => SetInserterDirection(Directions.WestToEast),
            };
            WestToEast.LocalPosition = new Vector2((width - WestToEast.Width) / 2 + 70 - moveOverConstant, 200 + heightOffset);
            ui.AddChild(WestToEast);

            var NorthToSouth = new Label()
            {

                String = "Down",
                //String = "↓",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.NorthToSouth : false,
                Callback = (e) => SetInserterDirection(Directions.NorthToSouth),
            };
            NorthToSouth.LocalPosition = new Vector2((width - NorthToSouth.Width) / 2 - moveOverConstant, 250 + heightOffset);
            ui.AddChild(NorthToSouth);
            var accept = new Label()
            {
                String = "Accept",
                Bold = true,
                Callback = (e) => Accept(),
            };

            accept.LocalPosition = new Vector2((width - accept.Width) - 150, height / 2 + 5 + heightOffset);
            ui.AddChild(accept);

            var itemSlot = new ItemSlot()
            {
                LocalPosition = new Vector2(150, height / 2 - 50 + heightOffset),
            };
            ui.AddChild(itemSlot);

        }

        public InserterCustomUI(InserterObject instance): this()
        {
            inserterInstance = instance;
            ReCreateUI();

        }
        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen + heightOffset, width, height / 2, Color.White);
            IClickableMenu.drawTextureBox(b, xPositionOnScreen + width-310, yPositionOnScreen + height / 2 - 130, 170,75, Color.White);
            base.draw(b, false, false);

            ui.Draw(b);
            if (base.heldItem != null)
            {
                base.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            }

            drawMouse(b);
        }

        //public override void receiveLeftClick(int x, int y, bool playSound = true)
        //{
        //    if (!okButton.containsPoint(x, y))
        //        base.receiveLeftClick(x, y, playSound);

        //    Item held = heldItem;
            //if (mainSpot.containsPoint(x, y))
            //{
            //    if (secondarySpot.item == null)
            //    {
            //        var tmp = mainSpot.item;
            //        mainSpot.item = heldItem;
            //        heldItem = tmp;
            //    }
            //    else
            //    {
            //        Game1.playSound("cancel");
            //    }
            //}
            //else if (secondarySpot.containsPoint(x, y) && mainSpot.item != null)
            //{
            //    bool doIt = false;
            //    if (mainSpot.item is MeleeWeapon mw)
            //    {
            //        if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, StardewValley.Object.prismaticShardIndex))
            //            doIt = true;
            //        else if (heldItem is IDGAItem dgai && dgai.FullId == ItemIds.SoulSapphire && (mw.InitialParentTileIndex == 62 || mw.InitialParentTileIndex == 63 || mw.InitialParentTileIndex == 64))
            //            doIt = true;
            //    }
            //    else if (mainSpot.item is Tool)
            //    {
            //        if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, StardewValley.Object.prismaticShardIndex))
            //            doIt = true;
            //    }
            //    else
            //    {
            //        if (heldItem is IDGAItem dgai && dgai.FullId == ItemIds.PersistiumDust)
            //            doIt = true;
            //    }

            //    if (doIt)
            //    {
            //        var tmp = secondarySpot.item;
            //        secondarySpot.item = heldItem;
            //        heldItem = tmp;
            //    }
            //    else
            //    {
            //        Game1.playSound("cancel");
            //    }
            //}
            //else if (okButton.containsPoint(x, y))
            //{
            //    if (mainSpot.item != null && secondarySpot.item != null && Game1.player.hasItemInInventory(ItemIds.StellarEssence.GetDeterministicHashCode(), 25))
            //    {
            //        doingStars = 0;
            //    }
            //    else
            //    {
            //        Game1.playSound("cancel");
            //    }
            //}
        //}

        public override void update(GameTime time)
        {
            ui.Update();
        }
        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                Exit();
            }
        }

        private void Exit()
        {
            this.exitThisMenuNoSound();
        }

        private void SetInserterDirection(Directions direction)
        {
            if (this.inserterInstance != null)
            {
                this.inserterInstance.ChangeDirection(direction);
                ReCreateUI();
            }
        }
        private void Accept()
        {
            ModEntry._Monitor.Log($"Accept was clicked", LogLevel.Debug);
            Exit();
        }
    }

}
