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
    public abstract class Player
    {
        public Vector2 position;
        public Vector2 drawPosition;
        public Texture2D image;
        public Texture2D attackAnimation;
        public Texture2D runningAnimation;
        public bool rangeBoosted;
        public float health;
        public float range;
        public BoundingBox pBB;
        public bool isBasicAttackCoolingDown;
        public int exp;
        public int maxExp;
        public int lvl;
        public int damage;
        public int maxLvl;
        public SpriteFont font;
        public float timeSinceAbilityTwo;
        public float abilityTwoCooldown;
        public float timeSinceAbilityOne;
        public float abilityOneCooldown;
        public bool isCountingAbOne;
        public bool isAbTwoCooldown;
        public float abTwoCooldownTime;
        public float abTwoExpirationTime;

        //timer stuff
        public float timeSinceBasicAttack;
        public float basicAttackCooldown;

        public float attackAnimationTime;
        public int attackAnimationThreshold;
        public Rectangle[] attackSourceRectangles;
        public byte currentAttackAnimationIndex;

        public float runningTime;
        public int runningThreshold;
        public Rectangle[] runningSourceRectangles;
        public byte currentRunningAnimationIndex;

        public float idleTime;
        public int idleThreshold;
        public Rectangle[] idleSourceRectangles;
        public byte idleAnimationIndex;

        public bool isAttacking;
        public bool isRunning;
        public bool isDead;
        public bool isDone;

        public float deathTime;
        public float deathThreshold;
        public Rectangle[] deathSourceRectangles;
        public byte deathIndex;


        public Player()
        {


        }

        abstract public void abilityOne(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, ContentManager c);
        abstract public void abilityTwo(Enemy e, List<Soldier> soldiers);
        abstract public void abilityThree(Enemy e);
        abstract public void basicAttack(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, int dir, ContentManager c);
        abstract public void LoadContent(ContentManager c);
        abstract public void Draw(SpriteBatch sb);
        abstract public void Update(GameTime gameTime, Vector2 ePos, List<Arrow> arrows);
    }
}
