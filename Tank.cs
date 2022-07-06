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
    //note - range for tank is melee attack range, as he doesn't have projectiles
    class Tank : Player
    {
        public bool isEnemyInAttackRange;
        public float distanceToEnemy;
        float timeSinceAbThree;
        float abThreeCooldown;
        bool isAbThreeCoolingDown;
        float attackDelay;
        SoundEffect swoosh;
        SoundEffect earthquake;
        SoundEffect clash;
        SoundEffect shockwave;
        float abilityOneRange;
        float abilityTwoRange;

        public Tank(float x, float y)
        {
            position = new Vector2(x, y);
            drawPosition = new Vector2(x, y);
            health = 120;
            isCountingAbOne = false;

            timeSinceAbilityOne = 0;

            timeSinceAbilityTwo = 0;

            abilityOneCooldown = 45;

            abilityTwoCooldown = 10;

            isAbTwoCooldown = false;

            abTwoCooldownTime = 0;

            abTwoExpirationTime = 5;

            timeSinceBasicAttack = 0;
            basicAttackCooldown = 1.75f;
            isBasicAttackCoolingDown = false;

            timeSinceAbThree = 0;
            abThreeCooldown = 35;
            isAbThreeCoolingDown = false;

            exp = 0;

            lvl = 1;
            damage = 25;
            maxLvl = 4;
            maxExp = 100;
            range = 100;

            runningTime = 0;
            runningThreshold = 135;
            runningSourceRectangles = new Rectangle[5];
            runningSourceRectangles[0] = new Rectangle(66, 226, 94, 86);
            runningSourceRectangles[1] = new Rectangle(161, 222, 95, 90);
            runningSourceRectangles[2] = new Rectangle(273, 225, 73, 90);
            runningSourceRectangles[3] = new Rectangle(351, 225, 66, 90);
            runningSourceRectangles[4] = new Rectangle(436, 226, 52, 89);
            currentRunningAnimationIndex = 0;
            isRunning = false;

            attackAnimationTime = 0;
            attackAnimationThreshold = 135;
            attackSourceRectangles = new Rectangle[4];
            attackSourceRectangles[0] = new Rectangle(71, 354, 94, 86);
            attackSourceRectangles[1] = new Rectangle(183, 354, 102, 86);
            attackSourceRectangles[2] = new Rectangle(297, 323, 57, 117);
            attackSourceRectangles[3] = new Rectangle(376, 360, 98, 79);
            currentAttackAnimationIndex = 0;
            isAttacking = false;
            attackDelay = 1.35f;

            abilityTwoRange = 1200;
            abilityOneRange = 1200;

            isDead = false;

            deathThreshold = 200;
            deathTime = 0;
            deathSourceRectangles = new Rectangle[9];
            deathSourceRectangles[0] = new Rectangle(71, 478, 94, 86);
            deathSourceRectangles[1] = new Rectangle(184, 481, 100, 88);
            deathSourceRectangles[2] = new Rectangle(301, 472, 77, 87);
            deathSourceRectangles[3] = new Rectangle(399, 468, 80, 88);
            deathSourceRectangles[4] = new Rectangle(502, 506, 93, 44);
            deathSourceRectangles[5] = new Rectangle(502, 506, 93, 44);
            deathSourceRectangles[6] = new Rectangle(502, 506, 93, 44);
            deathSourceRectangles[7] = new Rectangle(502, 506, 93, 44);
            deathSourceRectangles[8] = new Rectangle(502, 506, 93, 44);

            deathIndex = 0;
            isDone = false;


        }

        public override void LoadContent(ContentManager c)
        {
            runningAnimation = c.Load<Texture2D>("brute");
            pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
            font = c.Load<SpriteFont>("littleHealth");
            swoosh = c.Load<SoundEffect>("woosh-1");
            earthquake = c.Load<SoundEffect>("shake");
            clash = c.Load<SoundEffect>("sword_clash.10");
            shockwave = c.Load<SoundEffect>("shockwaveSound");
        }

        public override void Update(GameTime gameTime, Vector2 ePos, List<Arrow> arrows)
        {
            if (isDead)
            {
                if (deathIndex < deathSourceRectangles.Length-1 && !isDone)
                {
                    if (deathTime > deathThreshold)
                    {
                        deathIndex += 1;
                        deathTime = 0;
                    }else
                    {
                        deathTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }
                else
                {
                    isDone = true;
                }
            }
            if (!isDead)
            {
                pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                distanceToEnemy = Vector2.Distance(position, ePos);
                isRunning = false;

                if (Keyboard.GetState().IsKeyDown(Keys.D) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)))
                {
                    position.X += 1f;
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft)))
                {
                    position.X -= 1f;
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickUp))
                {
                    position.Y -= 1f;
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown)))
                {
                    position.Y += 1f;
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }

                //running animation
                if (currentRunningAnimationIndex == runningSourceRectangles.Length - 1)
                {
                    currentRunningAnimationIndex = 0;
                }
                if (isRunning)
                {
                    if (runningTime > runningThreshold)
                    {
                        currentRunningAnimationIndex += 1;
                        runningTime = 0;
                    }
                    else
                    {
                        runningTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }

                if (currentAttackAnimationIndex == attackSourceRectangles.Length - 1)
                {
                    currentAttackAnimationIndex = 0;
                    isAttacking = false;
                }
                if (isAttacking)
                {
                    if (attackAnimationTime > attackAnimationThreshold)
                    {
                        currentAttackAnimationIndex += 1;
                        attackAnimationTime = 0;
                    }
                    else
                    {
                        attackAnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    }
                }
                //basic attack cooldown
                if (timeSinceBasicAttack >= attackDelay)
                {
                    isAttacking = false;
                }

                if (timeSinceBasicAttack >= basicAttackCooldown)
                {
                    isBasicAttackCoolingDown = false;
                    timeSinceBasicAttack = 0f;
                }
                if (isBasicAttackCoolingDown)
                {
                    timeSinceBasicAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //checks if ability one is on cooldown
                if (timeSinceAbilityOne >= abilityOneCooldown)
                {
                    isCountingAbOne = false;
                    timeSinceAbilityOne = 0f;
                }

                //updates ability one time
                if (isCountingAbOne)
                {
                    timeSinceAbilityOne += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //checks if running ability is expired, starts cooldown
                if (timeSinceAbilityTwo >= abTwoExpirationTime)
                {
                    rangeBoosted = false;
                    isAbTwoCooldown = true;
                    timeSinceAbilityTwo = 0f;
                }

                //checks if ability cooldown is over
                if (abTwoCooldownTime >= abilityTwoCooldown)
                {
                    rangeBoosted = false;
                    abTwoCooldownTime = 0f;
                }


                //tracks ability cooldown time
                if (rangeBoosted)
                {
                    abTwoCooldownTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //ability three cooldown
                if (timeSinceAbThree >= abThreeCooldown)
                {
                    isAbThreeCoolingDown = false;
                    timeSinceAbThree = 0f;
                }
                if (isAbThreeCoolingDown)
                {
                    timeSinceAbThree += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //update levels
                if (exp >= maxExp && lvl < maxLvl)
                {
                    lvl += 1;
                    exp = 0;
                }

                //update damage with levels
                if (lvl == 1)
                {
                    damage = 15;
                }
                else if (lvl == 2)
                {
                    damage = 25;
                }
                else if (lvl == 3)
                {
                    damage = 50;
                }
                else if (lvl == 4)
                {
                    damage = 100;
                }


                //actual update stuff

                //attack range
                if (distanceToEnemy <= range)
                {
                    isEnemyInAttackRange = true;
                }
                else
                {
                    isEnemyInAttackRange = false;
                }
            }
            
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            if (isDead)
            {
                sb.Draw(runningAnimation, drawPosition, deathSourceRectangles[deathIndex], Color.White);
            }
            else if (isAttacking)
            {
                sb.Draw(runningAnimation, drawPosition, attackSourceRectangles[currentAttackAnimationIndex], Color.White);

            }
            else if (isRunning)
            {
                sb.Draw(runningAnimation, drawPosition, runningSourceRectangles[currentRunningAnimationIndex], Color.White);
            }
            else
            {
                sb.Draw(runningAnimation, drawPosition, runningSourceRectangles[0], Color.White);

            }
            if (health >= 0)
            {
                sb.DrawString(font, "Health: " + health, new Vector2(25, 850), Color.Black);

            }else
            {
                sb.DrawString(font, "Health: 0", new Vector2(25, 850), Color.Black);
            }
            
            sb.DrawString(font, "Level: " + lvl, new Vector2(175, 850), Color.Black);
            sb.DrawString(font, "EXP: " + exp, new Vector2(275, 850), Color.Black);
            if (isCountingAbOne)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Earthquake Cooldown (X): " + (int)(abilityOneCooldown - timeSinceAbilityOne), new Vector2(375, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Earthquake Cooldown (Z): " + (int)(abilityOneCooldown - timeSinceAbilityOne), new Vector2(375, 850), Color.Black);

                }
            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Earthquake Cooldown (X): Ready!", new Vector2(375, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Earthquake Cooldown (Z): Ready!", new Vector2(375, 850), Color.Black);

                }

            }
            if (rangeBoosted)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Shockwave Cooldown (A): " + (int)(abilityTwoCooldown - abTwoCooldownTime), new Vector2(750, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Shockwave Cooldown (X): " + (int)(abilityTwoCooldown - abTwoCooldownTime), new Vector2(750, 850), Color.Black);

                }
            }
           
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Shockwave Cooldown (A): Ready!", new Vector2(715, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Shockwave Cooldown (X): Ready!", new Vector2(715, 850), Color.Black);

                }

            }
            if (isAbThreeCoolingDown)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Smash Cooldown (B): " + (int)(abThreeCooldown - timeSinceAbThree), new Vector2(1050, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Smash Cooldown (C): " + (int)(abThreeCooldown - timeSinceAbThree), new Vector2(1050, 850), Color.Black);

                }

            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Smash Cooldown (B): Ready!", new Vector2(1050, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Smash Cooldown (C): Ready!", new Vector2(1050, 850), Color.Black);

                }

            }
            sb.End();
        }

        public override void abilityOne(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, ContentManager c)
        {

            for(int i = 0; i<soldiers.Count; i++)
            {
                if(Vector2.Distance(soldiers[i].position, position) <= abilityOneRange)
                {

                    soldiers[i].immobilize(4);
                    soldiers[i].health -= 10;
                }
                
            }
            if (distanceToEnemy <= abilityOneRange)
            {
                e.immobilize(4);
                e.health -= 25;
            }
            earthquake.Play(1, 0, 0);

            isCountingAbOne = true;
        }

        public override void abilityTwo(Enemy e, List<Soldier> soldiers)
        {
            if (!rangeBoosted)
            {
                shockwave.Play(1, 0, 0);

                for (int i = 0; i < soldiers.Count; i++)
                {
                    if (Vector2.Distance(soldiers[i].position, position) <= abilityTwoRange)
                    {
                        soldiers[i].knockBack();
                        soldiers[i].health -= 20;
                    }

                        
                }

                if (Vector2.Distance(e.position, position) <= abilityTwoRange)
                {
                    e.knockBack();
                    e.health -= 20;
                }
            }
           
            rangeBoosted = true;
        }

        public override void abilityThree(Enemy e)
        {
            if (isEnemyInAttackRange && !isAbThreeCoolingDown)
            {
                e.health -= 60;
                isAbThreeCoolingDown = true;
                clash.Play(1, 0, 0);
                isAttacking = true;
            }
        }

        public override void basicAttack(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, int dir, ContentManager c)
        {
            if (!isBasicAttackCoolingDown)
            {
                isAttacking = true;
                isBasicAttackCoolingDown = true;

                swoosh.Play(1, 0, 0);
                if (isEnemyInAttackRange)
                {
                    e.health -= damage;


                }
                for (int i = 0; i < soldiers.Count; i++)
                {
                    if (soldiers[i].isInTankRange)
                    {
                        soldiers[i].health -= damage;

                    }
                }
            }
            
        }
    }
}
