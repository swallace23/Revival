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
/// draw objects relative to player - glue player to side of screen and move objects towards it
/// don't worry about collision detection yet, but when you get to it:
///     Bounding boxes and bounding spheres - you can ask if theyre touching and it does all the calcs for you
///     you just have to create the box/sphere - always a rectangle or circle.
///     that code will come later
///     
/// there's a link at the top of resources about C# lists
/// 
/// DO NOT DO ANYTHING WITH THE SIZE OF THE WINDOW YET  - it is way harder than you think
/// </summary>

namespace Revival
{
    class Sniper : Player
    {

        float attackDelay;
        Arrow a;
        Arrow b;
        Arrow c;
        Arrow d;
        bool isAdded;
        bool isShot;
        SoundEffect shot;
        Texture2D deathAnimation;
        public Sniper(float x, float y)
        {

            position = new Vector2(x, y);
            drawPosition = new Vector2(x, y);
            rangeBoosted = false;
            range = 450f;
            health = 40;
            //all timer variable declarations
            isCountingAbOne = false;

            timeSinceAbilityOne = 0f;

            timeSinceAbilityTwo = 0f;

            abilityOneCooldown = 45.75f;

            abilityTwoCooldown = 55f;

            isAbTwoCooldown = false;

            abTwoCooldownTime = 0f;

            abTwoExpirationTime = 5f;

            timeSinceBasicAttack = 0f;
            basicAttackCooldown = 1.25f;
            isBasicAttackCoolingDown = false;

            exp = 0;
            
            lvl = 1;
            damage = 35;
            maxLvl = 4;
            maxExp = 100;

            // Set a default timer value.
            attackAnimationTime = 0;
            // Set an initial threshold of 250ms, you can change this to alter the speed of the animation (lower number = faster animation).
            attackAnimationThreshold = 75;
            attackSourceRectangles = new Rectangle[11];
            attackSourceRectangles[0] = new Rectangle(43, 31, 58, 76);
            attackSourceRectangles[1] = new Rectangle(175, 31, 56, 76);
            attackSourceRectangles[2] = new Rectangle(303, 21, 52, 86);
            attackSourceRectangles[3] = new Rectangle(431, 21, 52, 86);
            attackSourceRectangles[4] = new Rectangle(559, 12, 52, 95);
            attackSourceRectangles[5] = new Rectangle(687, 21, 67, 86);
            attackSourceRectangles[6] = new Rectangle(815, 21, 57, 86);
            attackSourceRectangles[7] = new Rectangle(943, 21, 52,86);
            attackSourceRectangles[8] = new Rectangle(47, 149, 52, 86);
            attackSourceRectangles[9] = new Rectangle(175, 149, 52, 86);
            attackSourceRectangles[10] = new Rectangle(303, 159, 56, 76);
            // This tells the animation to start on the left-side sprite.
            currentAttackAnimationIndex = 1;

            isRunning = false;
            isAttacking = false;

            attackDelay = 0.75f;

            isAdded = false;

            runningTime = 0;
            runningThreshold = 75;
            currentRunningAnimationIndex = 0;
            runningSourceRectangles = new Rectangle[8];
            runningSourceRectangles[0] = new Rectangle(42, 169, 65, 63);
            runningSourceRectangles[1] = new Rectangle(163, 169, 74, 63);
            runningSourceRectangles[2] = new Rectangle(291, 169, 72, 63);
            runningSourceRectangles[3] = new Rectangle(421, 169, 66, 63);
            runningSourceRectangles[4] = new Rectangle(549, 169, 58, 63);
            runningSourceRectangles[5] = new Rectangle(673, 169, 58, 63);
            runningSourceRectangles[6] = new Rectangle(803, 169, 62, 63);
            runningSourceRectangles[7] = new Rectangle(933, 169, 65, 63);

            deathTime = 0;
            deathThreshold = 200;
            deathIndex = 0;
            isDead = false;
            isDone = false;
            deathSourceRectangles = new Rectangle[13];
            deathSourceRectangles[0] = new Rectangle(44, 160, 56, 74);
            deathSourceRectangles[1] = new Rectangle(170, 168, 56, 66);
            deathSourceRectangles[2] = new Rectangle(298, 172, 56, 62);
            deathSourceRectangles[3] = new Rectangle(426, 172, 56, 62);
            deathSourceRectangles[4] = new Rectangle(552, 174, 56, 62);
            deathSourceRectangles[5] = new Rectangle(680, 174, 56, 62);
            deathSourceRectangles[6] = new Rectangle(808, 178, 58, 56);
            deathSourceRectangles[7] = new Rectangle(932, 204, 74, 30);
            deathSourceRectangles[8] = new Rectangle(34, 336, 84, 26);
            deathSourceRectangles[9] = new Rectangle(162, 340, 84, 22);
            deathSourceRectangles[10] = new Rectangle(162, 340, 84, 22);
            deathSourceRectangles[11] = new Rectangle(162, 340, 84, 22);
            deathSourceRectangles[12] = new Rectangle(162, 340, 84, 22);

        }


        public override void LoadContent(ContentManager c)
        {
            image = c.Load<Texture2D>("newarcher");
            pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + image.Width, position.Y + image.Height, 0));
            font = c.Load<SpriteFont>("health");
            attackAnimation = c.Load<Texture2D>("Normal Attack");
            runningAnimation = c.Load<Texture2D>("Idle and running");
            shot = c.Load<SoundEffect>("shoot");
            deathAnimation = c.Load<Texture2D>("death");

            
        }

        public override void Update(GameTime gameTime, Vector2 ePos, List<Arrow> arrows)
        {
            if (isDead)
            {
                if (deathIndex < deathSourceRectangles.Length - 1 && !isDone)
                {
                    if (deathTime > deathThreshold)
                    {
                        deathIndex += 1;
                        deathTime = 0;
                    }
                    else
                    {
                        deathTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }
                else
                {
                    isDone = true;
                }
            }
            else
            {
                pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));

                //animation stuff
                if (isAttacking == false)
                {
                    currentAttackAnimationIndex = 0;
                }
                if (isAttacking)
                {
                    if (attackAnimationTime > attackAnimationThreshold)
                    {

                        currentAttackAnimationIndex += 1;

                        // Reset the timer.
                        attackAnimationTime = 0;

                    }
                    // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
                    else
                    {
                        attackAnimationTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }




                //user movement
                isRunning = false;
                if ((Keyboard.GetState().IsKeyDown(Keys.D)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight)))
                {

                    position.X += 2f;
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft))
                {
                    if (position.X >= 2)
                    {
                        position.X -= 2f;
                    }
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if ((Keyboard.GetState().IsKeyDown(Keys.W)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickUp)))
                {
                    if (position.Y > 2)
                    {
                        position.Y -= 2f;
                    }
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }
                if ((Keyboard.GetState().IsKeyDown(Keys.S)) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickDown)))
                {
                    if (position.Y < 900)
                    {
                        position.Y += 2f;
                    }
                    pBB = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + runningSourceRectangles[currentRunningAnimationIndex].Width, position.Y + runningSourceRectangles[currentRunningAnimationIndex].Height, 0));
                    isRunning = true;
                }


                if (isRunning == false)
                {
                    currentRunningAnimationIndex = 0;
                }
                if (isRunning)
                {
                    if (currentRunningAnimationIndex == runningSourceRectangles.Length - 1)
                    {
                        currentRunningAnimationIndex = 0;
                    }
                    if (runningTime > runningThreshold)
                    {

                        currentRunningAnimationIndex += 1;



                        // Reset the timer.
                        runningTime = 0;

                    }
                    // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
                    else
                    {
                        runningTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }
                }




                //basic attack cooldown
                if (timeSinceBasicAttack >= attackDelay && !isAdded)
                {
                    a.position = position;
                    arrows.Add(a);
                    isAttacking = false;
                    isAdded = true;
                    shot.Play(1, 0, 0);
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
                if (timeSinceAbilityOne >= attackDelay && isShot == true)
                {
                    b.position = position;
                    arrows.Add(b);
                    c.position = position;
                    arrows.Add(c);
                    d.position = position;
                    arrows.Add(d);
                    isShot = false;
                    isAttacking = false;
                    shot.Play(1, 0, 0);


                }
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
                    isAbTwoCooldown = false;
                    abTwoCooldownTime = 0f;
                }

                //tracks time in ability
                if (rangeBoosted)
                {
                    timeSinceAbilityTwo += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //tracks ability cooldown time
                if (isAbTwoCooldown)
                {
                    abTwoCooldownTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //updates arrow range if ability 2 active
                //rangeBoosted doubles as a "is ability two active," may change name if convenient later
                if (rangeBoosted == true)
                {
                    range = 800f;
                    timeSinceAbilityTwo += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    range = 450f;
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
                    damage = 35;
                }
                else if (lvl == 2)
                {
                    damage = 40;
                }
                else if (lvl == 3)
                {
                    damage = 45;
                }
                else if (lvl == 4)
                {
                    damage = 50;
                }
            }
             
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Begin();
            //sb.Draw(image, drawPosition, Color.White);

            if (isDead)
            {
                sb.Draw(deathAnimation, drawPosition, deathSourceRectangles[deathIndex], Color.White);
            }
            else if(isAttacking)
            {
                sb.Draw(attackAnimation, drawPosition, attackSourceRectangles[currentAttackAnimationIndex], Color.White);
                //THIS IS THE CODE FOR FLIPPING THE DUDE
                //sb.Draw(attackAnimation, new Rectangle((int)drawPosition.X, (int)drawPosition.Y, attackSourceRectangles[currentAttackAnimationIndex].Width, attackSourceRectangles[currentAttackAnimationIndex].Height), attackSourceRectangles[currentAttackAnimationIndex], Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
            else if (isRunning)
            {
                sb.Draw(runningAnimation, drawPosition, runningSourceRectangles[currentRunningAnimationIndex], Color.White);
            }
            else
            {
                sb.Draw(attackAnimation, drawPosition, attackSourceRectangles[currentAttackAnimationIndex], Color.White);
            }

            if (health >= 0)
            {
                sb.DrawString(font, "Health: " + health, new Vector2(25, 850), Color.Black);
            }
            else
            {
                sb.DrawString(font, "Health: 0", new Vector2(25, 850), Color.Black);
            }
            sb.DrawString(font, "Level: " + lvl, new Vector2(175, 850), Color.Black);
            sb.DrawString(font, "EXP: " + exp, new Vector2(275, 850), Color.Black);
            if (isCountingAbOne)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Scattershot Cooldown (X): " + (int)(abilityOneCooldown - timeSinceAbilityOne), new Vector2(415, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Scattershot Cooldown (Z): " + (int)(abilityOneCooldown - timeSinceAbilityOne), new Vector2(415, 850), Color.Black);

                }
            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Scattershot Cooldown (X): Ready!", new Vector2(415, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Scattershot Cooldown (Z): Ready!", new Vector2(415, 850), Color.Black);

                }

            }
            if (isAbTwoCooldown)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Scope Cooldown (X): " + (int)(abilityTwoCooldown - abTwoCooldownTime), new Vector2(850, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Scope Cooldown (X): " + (int)(abilityTwoCooldown - abTwoCooldownTime), new Vector2(850, 850), Color.Black);

                }
            } else if (rangeBoosted)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Scope Cooldown (A): Boosted!", new Vector2(850, 850), Color.Black);

                }
                else
                {
                    sb.DrawString(font, "Scope Cooldown (X): Boosted!", new Vector2(850, 850), Color.Black);

                }
            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    sb.DrawString(font, "Scope Cooldown (A): Ready!", new Vector2(850, 850), Color.Black);

                } else
                {
                    sb.DrawString(font, "Scope Cooldown (X): Ready!", new Vector2(850, 850), Color.Black);

                }

            }
            sb.End();
        }

        //fires an arrow
        public override void basicAttack(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, int dir, ContentManager c)
        {
            
            a = new Arrow(position, dir, range);
            a.LoadContent(c);
            isBasicAttackCoolingDown = true;
            isAttacking = true;
            isAdded = false;
            

        }

        //shoots three arrows at once in a spray
        public override void abilityOne(Enemy e, List<Soldier> soldiers, List<Arrow> arrows, ContentManager content)
        {
            if (isCountingAbOne == false)
            {
                //creates new arrows and loads content for them
                 b = new Arrow(position, 1, range);
                b.LoadContent(content);
                 c = new Arrow(position, 5, range);
                c.LoadContent(content);
                 d = new Arrow(position, 6, range);
                d.LoadContent(content);
                //adds arrows to passed list of arrows
                
                //starts tracking cooldown time
                isCountingAbOne = true;
                isAttacking = true;
                isShot = true;
            }
           

        }
        //extends sniper basic attack range
        public override void abilityTwo(Enemy e, List<Soldier> soldiers)
        {
            if (isAbTwoCooldown == false)
            {
                rangeBoosted = true;
                timeSinceAbilityTwo = 0f;
               // isCountingAbTwo = true;
            }
           
        }

        public override void abilityThree(Enemy e)
        {
        }
    }
}
