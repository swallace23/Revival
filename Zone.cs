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
    public class Zone
    {
        public Vector2 position;
        public Texture2D enemyPic;
        public Texture2D playerPic;
        Texture2D redFlash;
        public bool isPlayerInZone;
        public bool isEnemyInZone;
        public bool isSoldierInZone;
        public bool isCaptured;
        public bool isLastCaptured;
        public float amountCaptured;
        float zoneHealth;
        int health;
        Vector2 textLocation;
        public BoundingBox zBB;
        SpriteFont font;
        public float width;
        public float height;
        float flashTime;
        float flashThreshold;
        bool isFlashing;

        public Zone(float x, float y)
        {
            position = new Vector2(x,y);
            isPlayerInZone = false;
            isEnemyInZone = false;
            isSoldierInZone = false;
            isLastCaptured = false;
            isCaptured = false;
            zoneHealth = 15;
            health = (int) zoneHealth- (int) amountCaptured;
            width = 500;
            height = 650;
            isFlashing = false;
            flashTime = 0;
            flashThreshold = 1f;

        }

        public void LoadContent(ContentManager c)
        {
            enemyPic = c.Load<Texture2D>("darkPurple");
            playerPic = c.Load<Texture2D>("greenSquare");
            redFlash = c.Load<Texture2D>("purpleSquare");
            zBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + playerPic.Width, position.Y + playerPic.Height, 0));
            font = c.Load<SpriteFont>("health");
            textLocation = new Vector2(position.X + (playerPic.Width / 2), 50);

        }

        public void Update(GameTime gameTime)
        {
            health = (int)zoneHealth - (int)amountCaptured;

            textLocation = new Vector2(position.X + (playerPic.Width / 4), 50);

            width = playerPic.Width;
            height = playerPic.Height;

            if (amountCaptured >= zoneHealth)
            {
                isCaptured = true;
            }
            if (isPlayerInZone && !isCaptured && !isEnemyInZone && !isSoldierInZone && isLastCaptured)
            {
                amountCaptured += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (isPlayerInZone && !isEnemyInZone && !isSoldierInZone)
            {
                flashTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            else
            {
                flashTime = 0;
                isFlashing = false;
            }

            if (flashTime >= flashThreshold)
            {
                isFlashing = true;
            } 
            if(flashTime >= flashThreshold * 2)
            {
                isFlashing = false;
                flashTime = 0;
            }

            //player movement

        }

        public void Draw(SpriteBatch sb, Vector2 pDrawPos, Vector2 pPos)
        {
            if (isCaptured == false)
            {
                sb.Begin();
                if (!isFlashing)
                {
                    sb.Draw(enemyPic, position - pPos + pDrawPos, Color.White);
                }
                else
                {
                    sb.Draw(redFlash, position - pPos + pDrawPos, Color.White);
                }
                sb.DrawString(font, "Zone Health: " + health, textLocation -pPos + pDrawPos, Color.Black);

                sb.End();

            }
            else
            {
                sb.Begin();
                
                    sb.Draw(playerPic, position - pPos + pDrawPos, Color.White);
                
                //sb.DrawString(font, "Zone Health: " +  health, textLocation, Color.Black);
                sb.End();
            }
            
        }

        public void reset()
        {
            isPlayerInZone = false;
            isEnemyInZone = false;
            isSoldierInZone = false;
            isLastCaptured = false;
            isCaptured = false;
            zoneHealth = 15;
            amountCaptured = 0;
            health = (int)zoneHealth - (int)amountCaptured;
            isFlashing = false;
            flashTime = 0;
            flashThreshold = 1f;
        }
    }
}
