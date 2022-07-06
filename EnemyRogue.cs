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


/// <summary>
/// general idea for movement - have separate move() function that takes a vector2 and moves towards it
/// in game1 pass the move function the player's position when it's within a certain distance
/// </summary>

namespace Revival
{
    class EnemyRogue : Enemy
    {
        Vector2 PlayerPosition;
        Vector2 destination;
        Vector2 toDest;
        SpriteFont font;
        SoundEffect woosh;
        
        float moveRange;
        float attackRange;
        float distanceToPlayer;

        //timer stuff
        float timeSinceBasicAttack;
        float basicAttackCooldown;

        float timeSinceAbilityTwo;
        float abilityTwoDuration;
        bool isAbilityTwoCoolingDown;
        float abilityTwoCooldown;

        float timeSinceAbilityOne;
        float abilityOneCooldown;
        bool isAbilityOneCoolingDown;

        float timeSinceDeath;
        float deathDuration;
        Vector2 poofPos;

        float timeSinceImmobilization;
        float immobilizeTime;
        bool isImmobilized;

        Vector2 knockGoal;
        bool isKnocking;
        Vector2 startingPos;

        //animation stuff
        Texture2D runningAnimation;
        float runningTime;
        float runningThreshold;
        public Rectangle[] runningSourceRectangles;
        public byte runningAnimationIndex;

        bool isAttacking;
        float attackTime;
        float attackThreshold;
        public Rectangle[] attackSourceRectangles;
        public byte attackAnimationIndex;

        Texture2D redSquare;


        public EnemyRogue(Vector2 start, Vector2 pPos)
        {
            position = start;
            destination = start;
            startingPos = start;
            isInMoveRange = false;
            moveRange = 300f;
            attackRange = 100f;
            PlayerPosition = pPos;
            speed = 1f;

            timeSinceBasicAttack = 0f;
            basicAttackCooldown = 2f;
            hasAttacked = false;

            isSneaking = false;

            timeSinceAbilityTwo = 0f;
            abilityTwoDuration = 4f;
            isAbilityTwoCoolingDown = false;
            abilityTwoCooldown = 10f;
            abilityTwoCooldownTime = 0f;

            timeSinceAbilityOne = 0f;
            abilityOneCooldown = 8f;
            isAbilityOneCoolingDown = false;

            health = 100;
            maxHealth = health;


            isDead = false;
            timeSinceDeath = 0f;
            deathDuration = 5f;
            
            enemyType = typeof(EnemyRogue);

            isPlayerInZone = false;

            timeSinceImmobilization = 0f;
            isImmobilized = false;

            runningTime = 0f;
            runningThreshold = 75;
            runningSourceRectangles = new Rectangle[6];
            runningSourceRectangles[0] = new Rectangle(37, 130, 73, 88);
            runningSourceRectangles[1] = new Rectangle(181, 133, 79, 85);
            runningSourceRectangles[2] = new Rectangle(328, 139, 82, 79);
            runningSourceRectangles[3] = new Rectangle(478, 130, 79, 88);
            runningSourceRectangles[4] = new Rectangle(628, 133, 82, 85);
            runningSourceRectangles[5] = new Rectangle(778, 139, 82, 79);
            runningAnimationIndex = 0;

            isAttacking = false;
            attackTime = 0f;
            attackThreshold = 100;
            attackSourceRectangles = new Rectangle[10];
            attackSourceRectangles[0] = new Rectangle(34, 469, 64, 82);
            attackSourceRectangles[1] = new Rectangle(191, 466, 63, 85);
            attackSourceRectangles[2] = new Rectangle(331, 449, 73, 102);
            attackSourceRectangles[3] = new Rectangle(469, 461, 123, 89);
            attackSourceRectangles[4] = new Rectangle(599, 484, 113, 67);
            attackSourceRectangles[5] = new Rectangle(755, 481, 96, 70);
            attackSourceRectangles[6] = new Rectangle(931, 560, 73, 102);
            attackSourceRectangles[7] = new Rectangle(1090, 574, 64, 88);
            attackSourceRectangles[8] = new Rectangle(1231, 574, 67, 88);
            attackSourceRectangles[9] = new Rectangle(1390, 571, 58, 91);
            attackAnimationIndex = 0;
            

        }

        public override void LoadContent(ContentManager c)
        {
            eBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[0].Width, position.Y + runningSourceRectangles[0].Height, 0));
            font = c.Load<SpriteFont>("health");
            runningAnimation = c.Load<Texture2D>("Fullmain");
            woosh = c.Load<SoundEffect>("woosh-2");
            redSquare = c.Load<Texture2D>("bigRed");
        }

        public override void Update(Vector2 posP, GameTime gameTime)
        {
            eBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

            if (isDead)
            {
                health = 100;
                mobilize();
                position = new Vector2(1000000, 100000);
                timeSinceAbilityOne = 0;
                timeSinceAbilityTwo = 0;
                isBasicAttackCoolingDown = false;
                isAbilityOneCoolingDown = false;
                isAbilityTwoCoolingDown = false;
                abilityTwoCooldownTime = 0;
                isSneaking = false;
                eBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));
                if (timeSinceDeath >= deathDuration)
                {
                    isDead = false;
                    position = new Vector2(posP.X+1700, posP.Y); 
                    timeSinceDeath = 0f;
                }
                else
                {
                    timeSinceDeath += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            if (!isDead)
            {
                PlayerPosition = posP;
                distanceToPlayer = Vector2.Distance(position, posP);

                //cooldown stuff
                if (timeSinceBasicAttack >= basicAttackCooldown)
                {
                    isBasicAttackCoolingDown = false;
                    timeSinceBasicAttack = 0;
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

                if (timeSinceAbilityOne >= abilityOneCooldown)
                {
                    isAbilityOneCoolingDown = false;
                    timeSinceAbilityOne = 0;
                }

                if (isAbilityOneCoolingDown)
                {
                    timeSinceAbilityOne += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //ability two cooldown
                if (abilityTwoCooldownTime >= abilityTwoCooldown)
                {
                    isAbilityTwoCoolingDown = false;
                    abilityTwoCooldownTime = 0;
                }
                if (isAbilityTwoCoolingDown)
                {
                    abilityTwoCooldownTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //ability two duration
                if (timeSinceAbilityTwo >= abilityTwoDuration)
                {
                    isSneaking = false;
                    isAbilityTwoCoolingDown = true;
                    timeSinceAbilityTwo = 0f;
                }

                if (isSneaking)
                {
                    timeSinceAbilityTwo += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //animation stuff
                if (runningAnimationIndex == runningSourceRectangles.Length - 1)
                {
                    runningAnimationIndex = 0;
                }
                else
                {
                    if (runningTime >= runningThreshold)
                    {
                        runningAnimationIndex += 1;
                        runningTime = 0;
                    }
                    else
                    {
                        runningTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }
                if (isAttacking)
                {
                    if (attackAnimationIndex == attackSourceRectangles.Length - 1)
                    {
                        attackAnimationIndex =0;
                        attackTime = 0;
                        isAttacking = false;
                    }
                    else if (attackTime >= attackThreshold)
                    {
                        attackAnimationIndex += 1;
                        attackTime = 0;
                    }
                    else
                    {
                        attackTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    }
                }

                //ends knockback
                if (Vector2.Distance(position, knockGoal) < 15)
                {
                    isKnocking = false;
                }

                //checks move range for AI
                if (distanceToPlayer < moveRange)
                {
                    isInMoveRange = true;
                }
                else
                {
                    isInMoveRange = false;
                }

                if (distanceToPlayer <= attackRange)
                {
                    isInAttackRange = true;
                    hasAttacked = true;
                }
                else
                {
                    isInAttackRange = false;
                }
                //update speed for when sneaking
                if (isSneaking)
                {
                    speed = 1.5f;
                }else
                {
                    speed = 1f;
                }

                //moves rogue

                move(posP);
                
            }
            eBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));


        }

        public override void Draw(SpriteBatch sb, Vector2 playerDrawPos, Vector2 playerPos )
        {
            sb.Begin();
            if (!isSneaking && !isDead)
            {

                if (isAttacking)
                {
                    sb.Draw(runningAnimation, new Rectangle((int)(position.X - playerPos.X + playerDrawPos.X), (int)(position.Y - playerPos.Y + playerDrawPos.Y), attackSourceRectangles[attackAnimationIndex].Width, attackSourceRectangles[attackAnimationIndex].Height), attackSourceRectangles[attackAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                }else if (isInAttackRange)
                {
                    sb.Draw(runningAnimation, new Rectangle((int)(position.X - playerPos.X + playerDrawPos.X), (int)(position.Y - playerPos.Y + playerDrawPos.Y), attackSourceRectangles[0].Width, attackSourceRectangles[0].Height), attackSourceRectangles[0], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

                }
                else
                {
                    sb.Draw(runningAnimation, new Rectangle((int)(position.X - playerPos.X + playerDrawPos.X), (int)(position.Y - playerPos.Y + playerDrawPos.Y), runningSourceRectangles[runningAnimationIndex].Width, runningSourceRectangles[runningAnimationIndex].Height), runningSourceRectangles[runningAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                }

                sb.DrawString(font, ""+ health, new Vector2(position.X-playerPos.X+playerDrawPos.X + 25, position.Y-playerPos.Y+playerDrawPos.Y - 25), Color.Black);
            } else if (isSneaking)
            {
                sb.Draw(redSquare, new Vector2(-100, -100), Color.White * 0.2f);
            }

            sb.End();
        }

        public void move(Vector2 pPos)
        {
            if (isKnocking)
            {
                toDest = knockGoal - position;
                toDest.Normalize();
                toDest *= 7;
                position += toDest;
                eBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[runningAnimationIndex].Width, position.Y + runningSourceRectangles[runningAnimationIndex].Height, 0));

            } else if (!isImmobilized)
            {
                if (!isInMoveRange && !isInAttackRange && !isPlayerInZone)
                {
                    position.X -= speed;
                }
                else if (!isInMoveRange && !isInAttackRange && isPlayerInZone)
                {
                    toDest = PlayerPosition - position;
                    toDest.Normalize();
                    position += (toDest * speed);
                }
                else if (isInMoveRange && isInAttackRange == false)
                {
                    toDest = PlayerPosition - position;
                    toDest.Normalize();
                    position += (toDest * speed);
                }
            }
             
        }
        public override void basicAttack(Player p, List<Arrow> arrows, int dir, ContentManager c)
        {
            if (!isDead && !isImmobilized)
            {
                p.health = p.health - 20;
                isBasicAttackCoolingDown = true;
                hasAttacked = true;
                isAttacking = true;
                woosh.Play(1, 0, 0);
            }
            
        }

        public override void abilityOne(List<Arrow> arrows, ContentManager c)
        {
            if (!isAbilityOneCoolingDown)
            {
                toDest = position - PlayerPosition;
                toDest.Normalize();
                position += (toDest * 150);
                woosh.Play(1, 0, 0);
                isAbilityOneCoolingDown = true;
            }
                
            
            
        }

        public override void abilityTwo()
        {
            if (!isAbilityTwoCoolingDown)
            {
                isSneaking = true;
                poofPos = position;
                woosh.Play(1, 0, 0);
            }
        }
       

        public override void immobilize(float it)
        {
            isImmobilized = true;
            immobilizeTime = it;
        }

        public override void mobilize()
        {
            isImmobilized = false;
            timeSinceImmobilization = 0;
        }

        public override void knockBack()
        {
            knockGoal = new Vector2(position.X + 200, position.Y);
            isKnocking = true;
        }

    }
}
