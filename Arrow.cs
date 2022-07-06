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
    public class Arrow
    {
        public Vector2 position;
        Vector2 prevPosition;
        Vector2 testPos;
        Vector2 toTest;
        Texture2D image;
        public BoundingBox arrowBB;
        public float distanceTraveled;
        public float range;
        float rotation;
        

        public Arrow(Vector2 pPos, int direction, float r)
        {
            position = new Vector2(pPos.X, pPos.Y);
            prevPosition = position;

            distanceTraveled = 0;
            range = r;
            //point to where arrow travels
            //1=right, 2=up, 3=left, 4=down, 5 and 6 are diagonal
            if (direction == 1) {
            testPos = new Vector2(pPos.X + 1000, pPos.Y+46);
            } else if (direction == 2)
            {
                testPos = new Vector2(pPos.X+37, pPos.Y - 1000);
            } else if(direction == 3)
            {
                testPos = new Vector2(pPos.X - 1000, pPos.Y+46);
            } else if (direction == 4)
            {
                testPos = new Vector2(pPos.X+37, pPos.Y + 1000);
            } else if (direction == 5)
            {
                testPos = new Vector2(pPos.X + 1000, pPos.Y + 250);
            } else if (direction == 6)
            {
                testPos = new Vector2(pPos.X + 1000, pPos.Y - 250);
            }
            else
            {
                throw new ArgumentException("Arrow direction must be 1 2 3 4 5 or 6");
            }
            //determines rotation for image
            rotation = (float)Math.Atan2((double)testPos.X, (double)testPos.Y);

            //moves arrow
            toTest = testPos - position;
            toTest.Normalize();
        }

        public void LoadContent(ContentManager c)
        {
            image = c.Load<Texture2D>("arrow");
            arrowBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + image.Width, position.Y + image.Height, 0));

        }

        public void Update()
        {
            //moves the arrow, updates distance traveled
            distanceTraveled += Vector2.Distance(prevPosition, position);
            prevPosition = position;
            position += (toTest*5);

            
            //updates arrow bounding box
            arrowBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + image.Width, position.Y + image.Height, 0));

        }

        public void Draw(SpriteBatch sb, Vector2 pDrawPos, Vector2 pPos)
        {
            sb.Begin();
            sb.Draw(image, position - pPos + pDrawPos, Color.White);
            sb.End();
        }

    }
}
