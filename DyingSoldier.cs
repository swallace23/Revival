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
    class DyingSoldier
    {
        Vector2 pos;
        Texture2D deathAnimation;
        float deathTime;
        float deathThreshold;
        int deathAnimationIndex;
        Rectangle[] deathSourceRectangles;
        public bool isDone;

        public DyingSoldier(Vector2 position)
        {
            pos = position;
            deathTime = 0;
            deathThreshold = 200;
            deathAnimationIndex = 0;
            deathSourceRectangles = new Rectangle[8];
            deathSourceRectangles[0] = new Rectangle(0, 88, 59, 65);
            deathSourceRectangles[1] = new Rectangle(100, 86, 62, 70);
            deathSourceRectangles[2] = new Rectangle(203, 87, 59, 66);
            deathSourceRectangles[3] = new Rectangle(304, 99, 65, 55);
            deathSourceRectangles[4] = new Rectangle(305, 191, 75, 44);
            deathSourceRectangles[5] = new Rectangle(305, 191, 75, 44);
            deathSourceRectangles[6] = new Rectangle(305, 191, 75, 44);
            deathSourceRectangles[7] = new Rectangle(305, 191, 75, 44);

            isDone = false;
        }

        public void LoadContent(ContentManager c)
        {
            deathAnimation = c.Load<Texture2D>("zombieDeath");
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
                    deathTime += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
                } else
                {
                    deathAnimationIndex += 1;
                    deathTime = 0;
                }
            }
        }

        public void Draw(SpriteBatch sb, Vector2 pDrawPos, Vector2 pPos)
        {
            sb.Begin();
            sb.Draw(deathAnimation, new Rectangle((int)(pos.X - pPos.X + pDrawPos.X), (int)(pos.Y - pPos.Y + pDrawPos.Y), deathSourceRectangles[deathAnimationIndex].Width, deathSourceRectangles[deathAnimationIndex].Height), deathSourceRectangles[deathAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            sb.End();
        }
    }
}
