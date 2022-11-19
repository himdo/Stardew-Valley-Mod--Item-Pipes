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

        public InserterCustomUI() : base((Game1.uiViewport.Width - 900) / 2, (Game1.uiViewport.Height - (Game1.uiViewport.Height - 100)) / 2, 900, (Game1.uiViewport.Height - 100))
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

            table = new Table()
            {
                RowHeight = (128 - 16) / 8,
                Size = new Vector2(700, height - 200),
                LocalPosition = new Vector2(50, 100),
            };

            for (int i = 0; i < 3; i ++)
            {

                List<Element> rowSlots = new List<Element>();
                //table.AddRow(rowSlots.ToArray());
                var rowElement = new Label()
                {
                    String = $"This is a table Item ${i}",
                    Bold = false,
                };
                rowElement.LocalPosition = new Vector2(150, 150 * (1 + i));
                rowSlots.Add(rowElement);
                table.AddRow(rowSlots.ToArray());
                // This appears to only put blank spaces
                for (int j = 0; j < 2; ++j)
                    table.AddRow(new Element[0]);
            }
            ui.AddChild(table);
            var accept = new Label()
            {
                String = "Accept",
                Bold = true,
                LocalPosition = new Vector2(500, height - 50),
                Callback = (e) => Accept(),
            };
            ui.AddChild(accept);
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

        private void Accept()
        {
            ModEntry._Monitor.Log($"Accept was clicked", LogLevel.Debug);
            Exit();
        }
    }

}
