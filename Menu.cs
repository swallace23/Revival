using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Revival
{
    class Menu
    {
        Texture2D startScreen;
        Texture2D endScreen;
        Texture2D lossScreen;
        Texture2D revival;
        Vector2 position;
        SpriteFont font;
        SpriteFont bigFont;
        public Menu()
        {
            position = new Vector2(350, 200);
        }
        public void LoadContent(ContentManager c)
        {
            startScreen = c.Load<Texture2D>("startpic");
            endScreen = c.Load<Texture2D>("win");
            lossScreen = c.Load<Texture2D>("lscreen");
            font = c.Load<SpriteFont>("health");
            bigFont = c.Load<SpriteFont>("bigHealth");
            revival = c.Load<Texture2D>("REVIVAL");
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X -= 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X += 2;
            }
        }
        public void Draw(SpriteBatch sb, int gameMode)
        {
            sb.Begin();
            if (gameMode == 0)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(bigFont, "WELCOME TO REVIVAL", new Vector2(275, 30), Color.White);
                    sb.DrawString(font, "Controls:", new Vector2(250, 150), Color.White);
                    sb.DrawString(font, "Left thumbstick to move", new Vector2(250, 200), Color.White);
                    sb.DrawString(font, "Right Thumbstick + Right Trigger to attack", new Vector2(250, 250), Color.White);
                    sb.DrawString(font, "A/B/Y for abilities", new Vector2(250, 300), Color.White);
                    sb.DrawString(font, "Menu to pause", new Vector2(250, 350), Color.White);
                    sb.DrawString(font, "A to start", new Vector2(250, 400), Color.White);

                    sb.DrawString(font, "Directions:", new Vector2(800, 150), Color.White);
                    sb.DrawString(font, "Kill enemies and capture red zones to win!", new Vector2(800, 200), Color.White);
                    sb.DrawString(font, "If enemies get too close, they will hurt you!", new Vector2(800, 250), Color.White);
                    sb.DrawString(font, "Beware! the enemy leader can dash and sneak", new Vector2(800, 300), Color.White);
                    sb.DrawString(font, "around the map", new Vector2(800, 330), Color.White);

                    sb.Draw(revival, new Vector2(465, 375), Color.White);
                } else
                {
                    sb.DrawString(bigFont, "WELCOME TO REVIVAL", new Vector2(275, 30), Color.White);
                    sb.DrawString(font, "Controls:", new Vector2(250, 150), Color.White);
                    sb.DrawString(font, "WASD to move", new Vector2(250, 200), Color.White);
                    sb.DrawString(font, "Arrow keys to attack", new Vector2(250, 250), Color.White);
                    sb.DrawString(font, "Z/X/C for abilities", new Vector2(250, 300), Color.White);
                    sb.DrawString(font, "P to pause", new Vector2(250, 350), Color.White);

                    sb.DrawString(font, "Space to start", new Vector2(250, 400), Color.White);


                    sb.DrawString(font, "Directions:", new Vector2(800, 150), Color.White);
                    sb.DrawString(font, "Kill enemies and capture red zones to win!", new Vector2(800, 200), Color.White);
                    sb.DrawString(font, "If enemies get too close, they will hurt you!", new Vector2(800, 250), Color.White);
                    sb.DrawString(font, "Beware! the enemy leader can dash and sneak", new Vector2(800, 300), Color.White);
                    sb.DrawString(font, "around the map", new Vector2(800, 330), Color.White);

                    sb.Draw(revival, new Vector2(465, 375), Color.White);
                }
                
            }
            else if (gameMode == 2)
            {
                sb.DrawString(bigFont, "YOU LOSE", new Vector2(500, 200), Color.Black);
                //sb.Draw(endScreen, new Vector2(250,200), Color.Black);
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Press Left Shoulder to restart.", new Vector2(515, 350), Color.Black);

                } else
                {
                    sb.DrawString(font, "Press Enter to restart.", new Vector2(515, 350), Color.Black);

                }
            }
            else if (gameMode == 3)
            {
                sb.DrawString(bigFont, "YOU WIN!", new Vector2(500, 200), Color.Black);
                //sb.Draw(endScreen, new Vector2(250,200), Color.Black);
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Press Left Shoulder to restart!", new Vector2(515, 350), Color.Black);

                } else
                {
                    sb.DrawString(font, "Press Enter to restart!", new Vector2(515, 350), Color.Black);

                }
            }
            sb.End();
        }


    }
}
