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
using System.Xml.Linq;
using SObject = StardewValley.Object;
using ItemPipes.ItemPipeObject;
using StardewValley.Characters;

namespace ItemPipes.ItemPipeUI
{
    public class CustomUIUserData
    {
        public int myID = -500;

        public int leftNeighborID = -1;

        public int rightNeighborID = -1;

        public int upNeighborID = -1;

        public int downNeighborID = -1;

        public CustomUIUserData(int myID, int leftNeighborID, int rightNeighborID, int upNeighborID, int downNeighborID)
        {
            this.myID = myID;
            this.leftNeighborID = leftNeighborID;
            this.rightNeighborID = rightNeighborID;
            this.upNeighborID = upNeighborID;
            this.downNeighborID = downNeighborID;
        }
    }

    public class ItemPipeCustomUI : MenuWithInventory
    {
        private RootElement ui;
        private Table table;
        private ItemPipe itemPipeInstance;
        private int heightOffset = -120;
        private bool interactingWithCustomUi = false;
        private int interactingWithCustomUiId = -1;
        private bool deBouncer = true;

        public ItemPipeCustomUI() : base(null, okButton: false, trashCan: false, 0,0)//12, 132)//: base((Game1.uiViewport.Width - 900) / 2, (Game1.uiViewport.Height - (Game1.uiViewport.Height - 100)) / 2, 900, (Game1.uiViewport.Height - 100))
        {
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
                String = ModEntry.helper.Translation.Get("ui.item-pipe.master-title.text"),
                Bold = true,
            };
            title.LocalPosition = new Vector2((width - title.Width) / 2, 10 + heightOffset);
            ui.AddChild(title);

            

            var DirectionText = new Label()
            {
                String = ModEntry.helper.Translation.Get("ui.item-pipe.direction-title.text"),
                Bold = true,
            };
            DirectionText.LocalPosition = new Vector2(15, 100 + heightOffset);
            ui.AddChild(DirectionText);

            int moveOverConstant = 300;
            var SouthToNorth = new Label()
            {
                String = ModEntry.helper.Translation.Get("ui.item-pipe.direction.up.text"),
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.SouthToNorth : false,
                Callback = (e) => SetPipeDirection(Directions.SouthToNorth),
            };
            SouthToNorth.LocalPosition = new Vector2((width - SouthToNorth.Width) / 2 - moveOverConstant, 150 + heightOffset);
            SouthToNorth.UserData = new CustomUIUserData(62003, 62001, 62002,-1,62001);
            ui.AddChild(SouthToNorth);

            var EastToWest = new Label()
            {
                String = ModEntry.helper.Translation.Get("ui.item-pipe.direction.left.text"),
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.EastToWest : false,
                Callback = (e) => SetPipeDirection(Directions.EastToWest),
            };
            EastToWest.LocalPosition = new Vector2((width - EastToWest.Width) / 2 - 70 - moveOverConstant, 200 + heightOffset);
            EastToWest.UserData = new CustomUIUserData(62001,-1,62002,62003,62000);
            ui.AddChild(EastToWest);
            var WestToEast = new Label()
            {
                String = ModEntry.helper.Translation.Get("ui.item-pipe.direction.right.text"),
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.WestToEast : false,
                Callback = (e) => SetPipeDirection(Directions.WestToEast),
            };
            WestToEast.LocalPosition = new Vector2((width - WestToEast.Width) / 2 + 70 - moveOverConstant, 200 + heightOffset);
            WestToEast.UserData = new CustomUIUserData(62002,62001,62100,62003,62000);
            ui.AddChild(WestToEast);

            var NorthToSouth = new Label()
            {

                String = ModEntry.helper.Translation.Get("ui.item-pipe.direction.down.text"),
                Bold = this.itemPipeInstance != null ? (int)this.itemPipeInstance.FacingDirection == (int)Directions.NorthToSouth : false,
                Callback = (e) => SetPipeDirection(Directions.NorthToSouth),
            };
            NorthToSouth.LocalPosition = new Vector2((width - NorthToSouth.Width) / 2 - moveOverConstant, 250 + heightOffset);

            NorthToSouth.UserData = new CustomUIUserData(62000,62001,62002,62001,0);
            ui.AddChild(NorthToSouth);
            var accept = new Label()
            {
                String = ModEntry.helper.Translation.Get("ui.item-pipe.button.accept.text"),
                Bold = true,
                Callback = (e) => Accept(),
            };

            accept.LocalPosition = new Vector2((width - accept.Width) - 150, height / 2 + 5 + heightOffset);
            accept.UserData = new CustomUIUserData(62010, -1,-1,62100,9);
            ui.AddChild(accept);
            var whitelistText = new Label()
            {
                String = (this.itemPipeInstance != null && this.itemPipeInstance.WhiteListMode == true) ? ModEntry.helper.Translation.Get("ui.item-pipe.whitelist-title.text") : ModEntry.helper.Translation.Get("ui.item-pipe.blacklist-title.text"),
                Bold = true,
            };
            whitelistText.LocalPosition = new Vector2((3*(width - whitelistText.Width)) / 4, 60 + heightOffset);
            ui.AddChild(whitelistText);


            var whitelistBlacklistToggle = new Checkbox()
            {
                Callback = (e) => ToggleButton(),
                Checked = (this.itemPipeInstance != null && this.itemPipeInstance.WhiteListMode == true)? true : false,
            };
            whitelistBlacklistToggle.LocalPosition = new Vector2(whitelistText.LocalPosition.X - 60, whitelistText.LocalPosition.Y);

            whitelistBlacklistToggle.UserData = new CustomUIUserData(62011, 62003,-1,-1, 62100);
            ui.AddChild(whitelistBlacklistToggle);

            table = new Table()
            {
                RowHeight = (128 - 16) / 8,
                Size = new Vector2(width/2 + 80, height/2 - 165),
                LocalPosition = new Vector2(width/2 - 120, 10),
            };

            if (this.itemPipeInstance != null)
            {
                List<Element> rowSlots = new List<Element>();
                int numberOfSlotsPerRow = 5;
                for (int i = 0; i < itemPipeInstance.WhiteListItems.Count + 1; i++)
                {
                    var itemSlot = new ItemSlot()
                    {
                        LocalPosition = new Vector2(10 + (100*(i% numberOfSlotsPerRow)), 150 * ((int)(Math.Ceiling((float) (i+1)/numberOfSlotsPerRow)))),
                        Callback = (e) => AddItem((ItemSlot)e),
                    };
                    if (i < itemPipeInstance.WhiteListItems.Count)
                    {
                        itemSlot.ItemDisplay = itemPipeInstance.WhiteListItems[i];
                    }
                    int currentId = 62100 + i;
                    itemSlot.UserData = new CustomUIUserData(
                        currentId, 
                        (i % numberOfSlotsPerRow == 0)? 62002: currentId-1, 
                        (i % numberOfSlotsPerRow == numberOfSlotsPerRow-1)? -1: (i < itemPipeInstance.WhiteListItems.Count) ? currentId + 1 : -1, 
                        (i < numberOfSlotsPerRow)? 62011 : currentId - numberOfSlotsPerRow, 
                        (i + numberOfSlotsPerRow <= itemPipeInstance.WhiteListItems.Count) ? currentId+numberOfSlotsPerRow: 62010);
                    rowSlots.Add(itemSlot);
                    if ((i+1)% numberOfSlotsPerRow == 0)
                    {
                        table.AddRow(rowSlots.ToArray());
                        // This appears to only put blank spaces
                        for (int j = 0; j < 2; ++j)
                            table.AddRow(new Element[0]);
                        rowSlots = new List<Element>();
                    }
                }
                if (rowSlots.Count > 0)
                {
                    table.AddRow(rowSlots.ToArray());
                    // This appears to only put blank spaces
                    for (int j = 0; j < 2; ++j)
                        table.AddRow(new Element[0]);
                    rowSlots = new List<Element>();
                }
            }
            ui.AddChild(table);
        }

        public ItemPipeCustomUI(ItemPipe instance): this()
        {
            itemPipeInstance = instance;
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

            if (ItemWithBorder.HoveredElement != null)
            {
                if (ItemWithBorder.HoveredElement is ItemSlot slot && slot.Item != null)
                {
                    drawToolTip(b, slot.Item.getDescription(), slot.Item.DisplayName, slot.Item);
                }
                else if (ItemWithBorder.HoveredElement.ItemDisplay != null)
                {
                    drawToolTip(b, ItemWithBorder.HoveredElement.ItemDisplay.getDescription(), ItemWithBorder.HoveredElement.ItemDisplay.DisplayName, ItemWithBorder.HoveredElement.ItemDisplay);
                }
            }
            else
            {
                var hover = base.inventory.hover(Game1.getMouseX(), Game1.getMouseY(), null);
                if (hover != null)
                {
                    drawToolTip(b, base.inventory.hoverText, base.inventory.hoverTitle, hover);
                }
            }

            drawMouse(b);
        }

        public override void update(GameTime time)
        {
            ui.Update();
        }
        protected override void cleanupBeforeExit()
        {
            if (base.heldItem != null)
            {
                Game1.player.addItemToInventory(base.heldItem);
                //base.heldItem = null;
            }
            itemPipeInstance.UIOpened.Set(false);
            base.cleanupBeforeExit();
        }
        public override void receiveKeyPress(Keys key)
        {
            //ModEntry._Monitor.Log($"receiveKeyPress ${key.ToSButton()}", LogLevel.Debug);
            if (this.currentlySnappedComponent == null)
            {
                if (base.inventory.inventory != null && base.inventory.inventory.Count > 0)
                {
                    if (base.inventory.allClickableComponents == null)
                    {
                        base.inventory.populateClickableComponentList();
                        this.allClickableComponents = base.inventory.allClickableComponents;
                    }
                    // ID 0 is the first slot in the inventory
                    var component = getComponentWithID(0);
                    this.currentlySnappedComponent = component;//base.inventory.allClickableComponents.First();
                    Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                }
            } else
            {
                if (interactingWithCustomUi == false)
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    {
                        int nextId = this.currentlySnappedComponent.leftNeighborID;
                        if (nextId != -1)
                        {
                            var component = getComponentWithID(nextId);
                            if (component != null)
                            {
                                this.currentlySnappedComponent = component;
                                Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    {
                        int nextId = this.currentlySnappedComponent.rightNeighborID;
                        if (nextId != -1)
                        {
                            var component = getComponentWithID(nextId);
                            if (component != null)
                            {
                                this.currentlySnappedComponent = component;
                                Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                    {
                        if (this.currentlySnappedComponent.myID >= 0 && this.currentlySnappedComponent.myID < 12) // This is the top row of the inventory
                        {
                            interactingWithCustomUi = true;
                            if (this.currentlySnappedComponent.myID >= 0 && this.currentlySnappedComponent.myID <= 4)
                            {
                                interactingWithCustomUiId = 62000;
                                moveMouseToCustomUIItem();
                            }
                            else if (this.currentlySnappedComponent.myID >= 5 && this.currentlySnappedComponent.myID <= 8)
                            {
                                interactingWithCustomUiId = 62100;
                                moveMouseToCustomUIItem();
                            }
                            else
                            {
                                interactingWithCustomUiId = 62010;
                                moveMouseToCustomUIItem();
                            }
                        }
                        else
                        {
                            int nextId = this.currentlySnappedComponent.upNeighborID;
                            if (nextId != -1)
                            {
                                var component = getComponentWithID(nextId);
                                if (component != null)
                                {
                                    this.currentlySnappedComponent = component;
                                    Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                                }
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    {
                        int nextId = this.currentlySnappedComponent.downNeighborID;
                        if (nextId != -1)
                        {
                            var component = getComponentWithID(nextId);

                            if (component != null)
                            {
                                this.currentlySnappedComponent = component;
                                Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                            }
                        }
                    }
                    //else if (Game1.options.doesInputListContain(Game1.options.actionButton, key))
                    //{
                    //    ModEntry._Monitor.Log($"Action Button Pressed In UI", LogLevel.Debug); // Pressed a on game pad
                    //}
                    //else if (Game1.options.doesInputListContain(Game1.options.useToolButton, key))
                    //{
                    //    ModEntry._Monitor.Log($"Use Tool Button Pressed In UI", LogLevel.Debug); // Pressed x on game pad
                    //}
                }
                else
                {
                    if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    {
                        var currentElement = GetElementById(interactingWithCustomUiId);
                        if (currentElement != null)
                        {
                            int nextId = (currentElement.UserData as CustomUIUserData).leftNeighborID;
                            if (nextId != -1)
                            {
                                if (nextId < 62000)
                                {
                                    interactingWithCustomUi = false;
                                    interactingWithCustomUiId = -1;
                                    var component = getComponentWithID(nextId);

                                    if (component != null)
                                    {
                                        this.currentlySnappedComponent = component;
                                        Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                                    }
                                } else
                                {
                                    interactingWithCustomUiId = nextId;
                                    moveMouseToCustomUIItem();
                                }
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    {
                        var currentElement = GetElementById(interactingWithCustomUiId);
                        if (currentElement != null)
                        {
                            int nextId = (currentElement.UserData as CustomUIUserData).rightNeighborID;
                            if (nextId != -1)
                            {
                                if (nextId < 62000)
                                {
                                    interactingWithCustomUi = false;
                                    interactingWithCustomUiId = -1;
                                    var component = getComponentWithID(nextId);

                                    if (component != null)
                                    {
                                        this.currentlySnappedComponent = component;
                                        Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                                    }

                                }
                                else
                                {
                                    interactingWithCustomUiId = nextId;
                                    moveMouseToCustomUIItem();
                                }
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                    {
                        var currentElement = GetElementById(interactingWithCustomUiId);
                        if (currentElement != null)
                        {
                            int nextId = (currentElement.UserData as CustomUIUserData).upNeighborID;
                            if (nextId != -1)
                            {
                                if (nextId < 62000)
                                {
                                    interactingWithCustomUi = false;
                                    interactingWithCustomUiId = -1;
                                    var component = getComponentWithID(nextId);

                                    if (component != null)
                                    {
                                        this.currentlySnappedComponent = component;
                                        Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                                    }

                                }
                                else
                                {
                                    interactingWithCustomUiId = nextId;
                                    moveMouseToCustomUIItem();
                                }
                            }
                        }
                    }
                    else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    {
                        var currentElement = GetElementById(interactingWithCustomUiId);
                        if (currentElement != null)
                        {
                            int nextId = (currentElement.UserData as CustomUIUserData).downNeighborID;
                            if (nextId != -1)
                            {
                                if (nextId < 62000)
                                {
                                    interactingWithCustomUi = false;
                                    interactingWithCustomUiId = -1;
                                    var component = getComponentWithID(nextId);

                                    if (component != null)
                                    {
                                        this.currentlySnappedComponent = component;
                                        Game1.setMousePosition(component.bounds.Right - component.bounds.Width / 4, component.bounds.Bottom - component.bounds.Height / 4, ui_scale: true);
                                    }

                                }
                                else
                                {
                                    interactingWithCustomUiId = nextId;
                                    moveMouseToCustomUIItem();
                                }
                            }
                        }
                    }
                    //else if (Game1.options.doesInputListContain(Game1.options.actionButton, key))
                    //{
                    //    ModEntry._Monitor.Log($"Action Button Pressed Custom UI", LogLevel.Debug);
                    //}
                    //else if (Game1.options.doesInputListContain(Game1.options.useToolButton, key))
                    //{
                    //    ModEntry._Monitor.Log($"Use Tool Button Pressed In Custom UI", LogLevel.Debug);
                    //}
                }
            }
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                Exit();
            }
        }

        private bool moveMouseToCustomUIItem()
        {
            return moveMouseToCustomUIItem(interactingWithCustomUiId);
        }
        private bool moveMouseToCustomUIItem(int id)
        {
            var element = GetElementById(id);
            if (element == null)
            {
                return false;
            }
            Game1.setMousePosition(element.Bounds.Right - element.Bounds.Width / 4, element.Bounds.Bottom - element.Bounds.Height / 4, ui_scale: true);
            return true;
        }

        private Element GetElementById(int id)
        {
            for (int i = 0; i < ui.Children.Count(); i++)
            {
                var child = ui.Children[i];
                if (child.UserData == null && (child as Table) == null)
                {
                    continue;
                }
                else
                {

                    if ((child as Table) == null)
                    {
                        CustomUIUserData userId = child.UserData as CustomUIUserData;
                        if (userId.myID == id)
                        {
                            return child;
                        }
                    } else
                    {
                        Table tableChild = child as Table;
                        for (int tableIndex = 0; tableIndex < tableChild.Children.Length; tableIndex++)
                        {
                            var tableChildInner = tableChild.Children[tableIndex];
                            var itemSlot = (tableChildInner as ItemSlot);
                            if (itemSlot != null && itemSlot.UserData != null)
                            {

                                CustomUIUserData userId = itemSlot.UserData as CustomUIUserData;
                                if (userId.myID == id)
                                {
                                    //ModEntry._Monitor.Log($"Table Neighbers: Left: ${userId.leftNeighborID}, Right: ${userId.rightNeighborID}, Up: ${userId.upNeighborID}, Down: ${userId.downNeighborID}, Whitelisted item count: ${itemPipeInstance.WhiteListItems.Count}", LogLevel.Debug);

                                    return itemSlot;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        
        private void Exit()
        {
            this.exitThisMenuNoSound();
        }

        private void SetPipeDirection(Directions direction)
        {
            if (this.itemPipeInstance != null)
            {
                this.itemPipeInstance.ChangeDirection(direction);
                ReCreateUI();
            }
        }
        private void Accept()
        {
            Exit();
        }

        private void ToggleButton()
        {
            if (deBouncer == true)
            {
                deBouncer = false;
                this.itemPipeInstance.ToggleWhiteListMode();
                ReCreateUI();
                DelayedAction.functionAfterDelay(AllowDebouncer, 500);
                
                switch (this.itemPipeInstance.FacingDirection)
                {
                    case (int)Directions.NorthToSouth:
                        interactingWithCustomUiId = 62000;
                        break;
                    case (int)Directions.SouthToNorth:
                        interactingWithCustomUiId = 62003;
                        break;
                    case (int)Directions.EastToWest:
                        interactingWithCustomUiId = 62001;
                        break;
                    case (int)Directions.WestToEast:
                        interactingWithCustomUiId = 62002;
                        break;
                }
                moveMouseToCustomUIItem();
            }
        }

        private void AllowDebouncer()
        {
            deBouncer = true;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            this.table.Scrollbar.ScrollBy(direction / -120);
        }

        private void AddItem(ItemSlot e)
        {
            if (base.heldItem == null)
            {
                itemPipeInstance.WhiteListItems.Remove(e.ItemDisplay);
                e.ItemDisplay = null;
                ReCreateUI();
            }
            else
            {
                bool foundCopy = false;
                for (int i = 0; i < itemPipeInstance.WhiteListItems.Count; i++)
                {
                    if (itemPipeInstance.WhiteListItems[i].ParentSheetIndex == base.heldItem.ParentSheetIndex && (itemPipeInstance.WhiteListItems[i] as SObject).quality.Value == (base.heldItem as SObject).quality.Value)
                    {
                        foundCopy = true;
                        break;
                    }
                }
                if (foundCopy == false)
                {
                    itemPipeInstance.WhiteListItems.Add(base.heldItem.getOne());
                    e.ItemDisplay = base.heldItem.getOne();
                    ReCreateUI();

                }
            }

        }
    }
}
