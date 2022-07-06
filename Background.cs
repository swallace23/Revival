using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Revival
{
    class Background
    {
        Vector2 position;
        Texture2D image;
        public Background(int x, int y)
        {
            position = new Vector2(x, y);
        }

        public void LoadContent(ContentManager c)
        {
            image = c.Load<Texture2D>("grass_template2");
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch sb, Vector2 pDrawPos, Vector2 pPos)
        {
            sb.Begin();
            sb.Draw(image, position - pPos + pDrawPos, Color.White);
            sb.End();
        }

    }
}
