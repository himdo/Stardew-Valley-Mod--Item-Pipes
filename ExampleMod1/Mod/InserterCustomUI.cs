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

namespace ExampleMod1
{
    public class InserterCustomUI : IClickableMenu
    {
        private RootElement ui;
        private Table table;
        private InserterObject inserterInstance;

        public InserterCustomUI() : base((Game1.uiViewport.Width - 900) / 2, (Game1.uiViewport.Height - (Game1.uiViewport.Height - 100)) / 2, 900, (Game1.uiViewport.Height - 100))
        {
            ModEntry._Monitor.Log($"UI Set", LogLevel.Debug);

            ReCreateUI();
        }

        private void ReCreateUI()
        {

            ui = new RootElement();
            ui.LocalPosition = new Vector2(xPositionOnScreen, yPositionOnScreen);

            var title = new Label()
            {
                String = "This is a menu Title",
                Bold = true,
            };
            title.LocalPosition = new Vector2((width - title.Width) / 2, 10);
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
            DirectionText.LocalPosition = new Vector2(10, height - 250);
            ui.AddChild(DirectionText);

            int moveOverConstant = 300;
            var SouthToNorth = new Label()
            {
                String = "Up",
                //String = "↑",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.SouthToNorth : false,
                Callback = (e) => SetInserterDirection(Directions.SouthToNorth),
            };
            SouthToNorth.LocalPosition = new Vector2((width - SouthToNorth.Width) / 2 - moveOverConstant, height - 200);
            ui.AddChild(SouthToNorth);

            var EastToWest = new Label()
            {
                String = "Left",
                //String = "→",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.EastToWest : false,
                Callback = (e) => SetInserterDirection(Directions.EastToWest),
            };
            EastToWest.LocalPosition = new Vector2((width - EastToWest.Width) / 2 - 70 - moveOverConstant, height - 150);
            ui.AddChild(EastToWest);
            var WestToEast = new Label()
            {
                String = "Right",
                //String = "←",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.WestToEast : false,
                Callback = (e) => SetInserterDirection(Directions.WestToEast),
            };
            WestToEast.LocalPosition = new Vector2((width - WestToEast.Width) / 2 + 70 - moveOverConstant, height - 150);
            ui.AddChild(WestToEast);

            var NorthToSouth = new Label()
            {

                String = "Down",
                //String = "↓",
                Bold = this.inserterInstance != null ? (int)this.inserterInstance.FacingDirection == (int)Directions.NorthToSouth : false,
                Callback = (e) => SetInserterDirection(Directions.NorthToSouth),
            };
            NorthToSouth.LocalPosition = new Vector2((width - NorthToSouth.Width) / 2 - moveOverConstant, height - 100);
            ui.AddChild(NorthToSouth);
            var accept = new Label()
            {
                String = "Accept",
                Bold = true,
                LocalPosition = new Vector2((width - SouthToNorth.Width)-150, height - 50),
                Callback = (e) => Accept(),
            };
            ui.AddChild(accept);
        }

        public InserterCustomUI(InserterObject instance): this()
        {
            inserterInstance = instance;
            ModEntry._Monitor.Log($"InstanceSet", LogLevel.Debug);

            ReCreateUI();

        }
        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, xPositionOnScreen - 12, yPositionOnScreen - 12, width + 24, height + 24, Color.White);

            ui.Draw(b);

            drawMouse(b);
        }

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
