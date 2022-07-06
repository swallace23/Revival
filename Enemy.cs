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
    public abstract class Enemy
    {
        public Vector2 position;
        public float speed;
        public Texture2D image;
        public bool isInMoveRange;
        public BoundingBox eBB;
        public bool isInAttackRange;
        public bool isBasicAttackCoolingDown;
        public bool hasAttacked;
        public bool isSneaking;
        public float health;
        public bool isDead;
        public float maxHealth;
        public Type enemyType;
        public bool isPlayerInZone;
        public float abilityTwoCooldownTime;

        public Enemy()
        {
            
    }

        abstract public void abilityOne(List<Arrow> arrows, ContentManager c);
        abstract public void abilityTwo();
        abstract public void basicAttack(Player p, List<Arrow> arrows, int dir, ContentManager c);
        abstract public void LoadContent(ContentManager c);
        abstract public void Draw(SpriteBatch sb, Vector2 playerDrawPos, Vector2 playerPos);
        abstract public void Update(Vector2 posP, GameTime gameTime);
        abstract public void immobilize(float it);
        abstract public void mobilize();
        abstract public void knockBack();
    }
}
