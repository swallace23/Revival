using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Revival
{
    class DyingRogue
    {
        Vector2 pos;
        Texture2D deathAnimation;
        float deathTime;
        float deathThreshold;
        int deathAnimationIndex;
        Rectangle[] deathSourceRectangles;
        public bool isDone;

        public DyingRogue(Vector2 position)
        {
            pos = position;
            deathTime = 0;
            deathThreshold = 200;
            deathAnimationIndex = 0;
            deathSourceRectangles = new Rectangle[10];
            deathSourceRectangles[0] = new Rectangle(44, 1019, 53, 86);
            deathSourceRectangles[1] = new Rectangle(188, 1028, 62, 77);
            deathSourceRectangles[2] = new Rectangle(338, 1034, 71, 71);
            deathSourceRectangles[3] = new Rectangle(488, 1037, 71, 68);
            deathSourceRectangles[4] = new Rectangle(620, 1067, 110, 38);
            deathSourceRectangles[5] = new Rectangle(620, 1067, 110, 38);
            deathSourceRectangles[6] = new Rectangle(620, 1067, 110, 38);
            deathSourceRectangles[7] = new Rectangle(620, 1067, 110, 38);
            deathSourceRectangles[8] = new Rectangle(620, 1067, 110, 38);
            deathSourceRectangles[9] = new Rectangle(620, 1067, 110, 38);

            isDone = false;
        }

        public void LoadContent(ContentManager c)
        {
            deathAnimation = c.Load<Texture2D>("main2.1");
        }

        public void Update(GameTime gameTime)
        {
            if (deathAnimationIndex == deathSourceRectangles.Length - 1)
            {
                isDone = true;
            }
            if (!isDone)
            {
                if (deathTime < deathThreshold)
                {
                    deathTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else
                {
                    deathAnimationIndex += 1;
                    deathTime = 0;
                }
            }
        }

        public void Draw(SpriteBatch sb, Vector2 playerPos, Vector2 playerDrawPos)
        {
            sb.Begin();
            sb.Draw(deathAnimation, new Rectangle((int)(pos.X - playerPos.X + playerDrawPos.X), (int)(pos.Y - playerPos.Y + playerDrawPos.Y), deathSourceRectangles[deathAnimationIndex].Width, deathSourceRectangles[deathAnimationIndex].Height), deathSourceRectangles[deathAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            sb.End();
        }
    }
}
