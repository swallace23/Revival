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
    public class Soldier
    {
        public Vector2 position;
        int ID;
        public Vector2 velocity;
        public int health;
        public String healthString;
        public BoundingBox soldierBB;
        public Zone firstZone;
        public bool isInZone;
        public bool isPInZone;
        Vector2 zoneCenter;
        static SpriteFont font;
        float moveRange;
        bool isInMoveRange;
        bool isInAttackRange;
        float attackRange;
        Vector2 wanderDestination;
        BoundingBox wanderBB;

        float timeSinceBasicAttack;
        float basicAttackCooldown;
        bool isBasicAttackCoolingDown;

        float timeSinceImmobilization;
        float immobilizeTime;
        bool isImmobilized;


        float distanceToPlayer;
        static Random r;

        int speed;

        public bool isInTankRange;

        Vector2 knockGoal;
        bool isKnocking;

        static Texture2D runningAnimation;
        float runningTime;
        float runningThreshold;
        int runningAnimationIndex;
        Rectangle[] runningSourceRectangles;

         



        public Soldier(float x, float y, Zone z, int id)
        {
            position = new Vector2(x, y);
            //health starts at 100
            health = 30;
            firstZone = z;
            velocity = new Vector2(0, 0);
            zoneCenter = new Vector2(firstZone.position.X + 250, position.Y);

            timeSinceBasicAttack = 0f;
            basicAttackCooldown = 3f;
            isBasicAttackCoolingDown = false;
            timeSinceImmobilization = 0f;
            isImmobilized = false;
            distanceToPlayer = 0f;
            attackRange = 100f;
            moveRange = 300f;
            ID = id;
            isPInZone = false;
            wanderDestination = wander();
            wanderBB = new BoundingBox(new Vector3(wanderDestination.X, wanderDestination.Y, 0), new Vector3(wanderDestination.X + 1, wanderDestination.Y + 1, 0));
            speed = 1;
            isInTankRange = false;

            runningTime = 0;
            runningThreshold = 75;
            runningAnimationIndex = 0;
            runningSourceRectangles = new Rectangle[10];
            runningSourceRectangles[0] = new Rectangle(12, 9, 52, 84);
            runningSourceRectangles[1] = new Rectangle(84, 8, 57, 86);
            runningSourceRectangles[2] = new Rectangle(158, 8, 60, 85);
            runningSourceRectangles[3] = new Rectangle(235, 8, 60, 84);
            runningSourceRectangles[4] = new Rectangle(316, 8, 57, 85);
            runningSourceRectangles[5] = new Rectangle(395, 9, 56, 83);
            runningSourceRectangles[6] = new Rectangle(478, 8, 50, 86);
            runningSourceRectangles[7] = new Rectangle(554, 8, 50, 86);
            runningSourceRectangles[8] = new Rectangle(632, 8, 51, 85);
            runningSourceRectangles[9] = new Rectangle(708, 8, 51, 85);

            

        }

        public static void LoadContent(ContentManager c)
        {
            runningAnimation = c.Load<Texture2D>("zombies");
            font = c.Load<SpriteFont>("health");
            r = new Random();

        }

        public void Update(Player p, GameTime gameTime)
        {
            soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

            healthString = health.ToString();

            zoneCenter = new Vector2(firstZone.position.X + 250, 300);

            distanceToPlayer = Vector2.Distance(position, p.position);
            //speed it up when far away
            /* if (distanceToPlayer > 800)
             {
                 speed = 3;
             }
             else
             {
                 speed = 1;
             }*/

            //attack cooldowns
            if (timeSinceBasicAttack >= basicAttackCooldown)
            {
                isBasicAttackCoolingDown = false;
                timeSinceBasicAttack = 0f;
            }
            if (isBasicAttackCoolingDown)
            {
                timeSinceBasicAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (timeSinceImmobilization >= immobilizeTime)
            {
                mobilize();
            }

            if (isImmobilized)
            {
                timeSinceImmobilization += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (distanceToPlayer <= attackRange)
            {
                isInAttackRange = true;
            }
            else
            {
                isInAttackRange = false;
            }

            if (distanceToPlayer <= moveRange && !isInAttackRange)
            {
                isInMoveRange = true;
            }
            else
            {
                isInMoveRange = false;
            }

            //animation stuff
            if (runningAnimationIndex == runningSourceRectangles.Length - 1)
            {
                runningAnimationIndex = 0;
                runningTime = 0;
            }
            if (runningTime >= runningThreshold)
            {
                runningAnimationIndex += 1;
                runningTime = 0;
            }
            else
            {
                runningTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            

            //update wander destination 
            if (!wanderBB.Intersects(firstZone.zBB))
            {
                wanderDestination = wander();
                wanderBB = new BoundingBox(new Vector3(wanderDestination.X, wanderDestination.Y, 0), new Vector3(wanderDestination.X + 1, wanderDestination.Y + 1, 0));
            }

            //AI movement

            move(firstZone.position, p.position);
            if (Vector2.Distance(position, wanderDestination) < 5)
            {
                wanderDestination = wander();
                wanderBB = new BoundingBox(new Vector3(wanderDestination.X, wanderDestination.Y, 0), new Vector3(wanderDestination.X + 1, wanderDestination.Y + 1, 0));

            }

            if (Vector2.Distance(position, knockGoal) < 15)
            {
                isKnocking = false;
            }

            if (p.pBB.Intersects(firstZone.zBB))
            {
                isPInZone = true;
            }
            else
            {
                isPInZone = false;
            }


            soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

            //attack stuff
            if (isInAttackRange && !isBasicAttackCoolingDown)
            {
                attack(p);
                isBasicAttackCoolingDown = true;
            }
        }

        public void Draw(SpriteBatch sb, Vector2 pDrawPos, Vector2 pPos)
        {
            sb.Begin();
            sb.Draw(runningAnimation, new Rectangle((int)(position.X - pPos.X + pDrawPos.X), (int)(position.Y - pPos.Y + pDrawPos.Y), runningSourceRectangles[runningAnimationIndex].Width, runningSourceRectangles[runningAnimationIndex].Height), runningSourceRectangles[runningAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

            sb.DrawString(font, "" + health, new Vector2(position.X + 25, position.Y - 25) - pPos + pDrawPos, Color.Black);
            sb.End();
        }

        public void move(Vector2 zonePos, Vector2 pPos)
        {
            if (!isInAttackRange)
            {
                if(isKnocking){
                    velocity = knockGoal - position;
                    velocity.Normalize();
                    velocity *= 7;
                    position += velocity;
                    soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

                }
                else if (isInMoveRange || (isPInZone && isInZone))
                {
                    velocity = pPos - position;
                    velocity.Normalize();
                    velocity *= speed;
                    position += velocity;
                    soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));
                }
                else if (!isInZone)
                {
                    velocity = new Vector2(zonePos.X, position.Y) - position;
                    velocity.Normalize();
                    velocity *= speed;
                    position += velocity;
                    soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));
                }

                else
                {

                    velocity = wanderDestination - position;
                    velocity.Normalize();
                    velocity *= speed;
                    position += velocity;
                    soldierBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

                }

            }

        }

        public void attack(Player p)
        {
            if (!isImmobilized)
            {
                p.health -= 5;
            }
        }

        public Vector2 wander()
        {
            //Random r = new Random();

            float xDestination = r.Next((int)firstZone.position.X, (int)(firstZone.position.X + firstZone.width));
            float yDestination = r.Next((int)firstZone.position.Y, (int)(firstZone.position.Y + firstZone.height));

            return new Vector2(xDestination, yDestination);
        }

        public void immobilize(float it)
        {
            speed = 0;
            isImmobilized = true;
            immobilizeTime = it;
        }

        public void mobilize()
        {
            speed = 1;
            isImmobilized = false;
            timeSinceImmobilization = 0;
        }

        public void knockBack()
        {
            knockGoal = new Vector2(position.X + 200, position.Y);
            isKnocking = true;
        }

    }
}
