using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
/// <summary>
/// TODO:
/// change UI depending on if controller is connected
/// playtest through everything a couple more times!
/// almost there!

/// Notes on controls:
/// WASD for movement
/// Arrow keys for basic attacks
/// Z, X, and C for abilities
/// </summary>
namespace Revival
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player p;
        Enemy e;
        DyingRogue dr;
        List<Arrow> arrows;
        List<Soldier> soldiers;
        List<DyingSoldier> deadSoldiers;
        KeyboardState prevKeyboard;
        GamePadState prevGamePad;
        Type eType;
        Type pType;

        Menu menu;
        List<Zone> zones;
        SpriteFont font;
        SpriteFont bigFont;
        Song introSong;
        Song fightSong;
        

        //timer stuff
        float timeSinceLastWave;
        float waveCooldown;
        bool isWaveCoolingDown;

        Vector2 PlayerDrawPosition;

        List<Background> backgrounds;

        float freezeTime;
        float freezeThreshold;

        Texture2D tankThumbnail;
        Texture2D archerThumbnail;

        bool isController;

 
        enum GameState
        {
            
            START,
            CHOICE,
            PLAY,
            LOSS,
            WIN,
            RESTART,
            PAUSE,
            FREEZE
        }
        GameState gameState = GameState.START;

        bool isPlaying;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            PlayerDrawPosition = new Vector2(100, 300);
            
            //p = new Tank(100,300);
            //this was +1250 originally
            e = new EnemyRogue(new Vector2(PlayerDrawPosition.X+1700, PlayerDrawPosition.Y), PlayerDrawPosition);
            menu = new Menu();
            arrows = new List<Arrow>();
            soldiers = new List<Soldier>();
            zones = new List<Zone>();
            //first originally 250
            zones.Add(new Zone(PlayerDrawPosition.X+600, 150));
            //900
            zones.Add(new Zone(PlayerDrawPosition.X+1250, 150));
            //1550
            zones.Add(new Zone(PlayerDrawPosition.X+1800, 150));

            backgrounds = new List<Background>();
            backgrounds.Add(new Background(0, 0));
            backgrounds.Add(new Background(1600, 0));
            backgrounds.Add(new Background(3200, 0));
            backgrounds.Add(new Background(0, -900));
            backgrounds.Add(new Background(0, 900));
            backgrounds.Add(new Background(1600, -900));
            backgrounds.Add(new Background(1600, 900));
            backgrounds.Add(new Background(3200, -900));
            backgrounds.Add(new Background(3200, 900));

            deadSoldiers = new List<DyingSoldier>();
            eType = e.GetType();
           // pType = p.GetType();

            //timer stuff
            timeSinceLastWave = 0f;
            waveCooldown = 20;
            isWaveCoolingDown = false;

            //making first zone capturable
            zones[0].isLastCaptured = true;

            isPlaying = false;

            freezeTime = 0;
            freezeThreshold = 1250;

            isController = false;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            prevKeyboard = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
 
            menu.LoadContent(Content);


            fightSong = Content.Load<Song>("Karstenholymoly_-_The_Invisible_Enemy_(feat._bangcorrupt)");
            
            introSong = Content.Load<Song>("mactonite_-_Requiem_for_the_Corrupt");

            tankThumbnail = Content.Load<Texture2D>("tankThumbnail");
            archerThumbnail = Content.Load<Texture2D>("archerThumbnail");
            
            foreach(Background b in backgrounds)
            {
                b.LoadContent(Content);
            }

            if (dr != null)
            {
                dr.LoadContent(Content);
            }
            foreach(DyingSoldier d in deadSoldiers)
            {
                d.LoadContent(Content);
            }
            

            //p.LoadContent(Content);
            e.LoadContent(Content);
            font = Content.Load<SpriteFont>("health");
            bigFont = Content.Load<SpriteFont>("bigHealth");
            Soldier.LoadContent(Content);
            
            
                
            foreach (Arrow a in arrows)
            {
                a.LoadContent(Content);

            }
            foreach(Zone z in zones)
            {
                z.LoadContent(Content);
            }
            
            

            // TODO: use this.Content to load your game content here
        }

        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            
            KeyboardState state = Keyboard.GetState();
            GamePadState gState = GamePad.GetState(PlayerIndex.One);

            
            
            if (gameState == GameState.START)
            {
                
                
                if (!isPlaying)
                {
                    MediaPlayer.Play(introSong);
                    MediaPlayer.IsRepeating = true;
                    isPlaying = true;
                }
                
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                {
                    gameState = GameState.CHOICE;
                }

            }
            else if (gameState == GameState.CHOICE)
            {
                
                if (Keyboard.GetState().IsKeyDown(Keys.T) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X))
                {
                    p = new Tank(PlayerDrawPosition.X, PlayerDrawPosition.Y);
                    pType = p.GetType();
                    p.LoadContent(Content);
                    gameState = GameState.PLAY;
                    MediaPlayer.Stop();
                    isPlaying = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Y) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
                {
                    p = new Sniper(PlayerDrawPosition.X, PlayerDrawPosition.Y);
                    pType = p.GetType();
                    p.LoadContent(Content);
                    gameState = GameState.PLAY;
                    MediaPlayer.Stop();
                    isPlaying = false;
                }
            } else if (gameState == GameState.RESTART)
            {
                gameState = GameState.START;
                //soldier restart
                soldiers.Clear();
                timeSinceLastWave = 0f;
                isWaveCoolingDown = false;
                //arrow restart
                arrows.Clear();
                //player restart
                p = null;
                //enemy restart
                e = new EnemyRogue(new Vector2(PlayerDrawPosition.X + 1700, PlayerDrawPosition.Y), PlayerDrawPosition);
                e.LoadContent(Content);
                //zone restart
                for (int i = 0; i < zones.Count; i++)
                {
                    zones[i].reset();
                }
                zones[0].isLastCaptured = true;

                MediaPlayer.Stop();
                isPlaying = false;

            }
            else if(gameState == GameState.PAUSE)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.O) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder))
                {
                    gameState = GameState.PLAY;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                {
                    this.Exit();
                }
            } else if (gameState == GameState.FREEZE)
            {
                if (freezeTime <= freezeThreshold)
                {
                    freezeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                } else
                {
                    gameState = GameState.WIN;
                }
            } else if(gameState == GameState.WIN || gameState == GameState.LOSS)
            {
                if (!isPlaying)
                {
                    MediaPlayer.Play(introSong);
                    isPlaying = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder))
                {
                    gameState = GameState.RESTART;
                }
            }
            else if (gameState == GameState.PLAY) {
                //restart if enter/left shoulder is pressed
                if (!isPlaying)
                {
                    MediaPlayer.Volume = 0.2f;
                    MediaPlayer.Play(fightSong);
                    MediaPlayer.IsRepeating = true;
                    isPlaying = true;
                }
                
                
                if(Keyboard.GetState().IsKeyDown(Keys.P) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
                {
                    gameState = GameState.PAUSE;
                    return;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder))
                {
                    gameState = GameState.RESTART;
                }

                //check win condition
                bool isAllCaptured = true; 
                for(int i = 0; i<zones.Count; i++)
                {
                    if (zones[i].isCaptured == false)
                    {
                        isAllCaptured = false;
                        break;
                    }
                }

                //timers
                if (timeSinceLastWave >= waveCooldown)
                {
                    isWaveCoolingDown = false;
                    timeSinceLastWave = 0f;
                }
                if (isWaveCoolingDown)
                {
                    timeSinceLastWave += (float)gameTime.ElapsedGameTime.TotalSeconds;

                }

              

                //win condition
                if (isAllCaptured == true)
                {
                    gameState = GameState.FREEZE;
                }
                //loss condition
                if (p.isDone)
                {
                    gameState = GameState.LOSS;
                    MediaPlayer.Stop();
                    isPlaying = false;
                }
                //check health/distance
                if (p.health <= 0)
                {
                    p.isDead = true;
                   //gameState=GameState.LOSS;
                }

                if (e.health <= 0)
                {
                    dr = new DyingRogue(e.position);
                    dr.LoadContent(Content);
                    e.isDead = true;
                    p.exp += 35;
                }
                if (soldiers.Count > 0)
                {
                    for (int i = soldiers.Count - 1; i >= 0; i--)
                    {

                        if (soldiers[i].health <= 0)
                        {
                            DyingSoldier d = new DyingSoldier(soldiers[i].position);
                            deadSoldiers.Add(d);
                            d.LoadContent(Content);

                            soldiers.Remove(soldiers[i]);
                            p.exp += 15;

                        }
                    }
                }

                if (arrows.Count > 0)
                {
                    for (int i = arrows.Count - 1; i >= 0; i--)
                    {

                        if (arrows[i].distanceTraveled >= arrows[i].range)
                        {
                            arrows.Remove(arrows[i]);
                        }
                    }
                }

                if (!isWaveCoolingDown)
                {
                    if (!zones[1].isCaptured)
                    {
                        Soldier s = new Soldier(p.position.X + 2000, 150, zones[0], 1);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 2000, 325, zones[0], 2);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 2000, 500, zones[0], 3);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 2000, 675, zones[0], 4);
                        soldiers.Add(s);
                        isWaveCoolingDown = true;
                    }
                    else
                    {
                        Soldier s = new Soldier(p.position.X + 1000, 150, zones[0], 1);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 1000, 325, zones[0], 2);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 1000, 500, zones[0], 3);
                        soldiers.Add(s);
                        s = new Soldier(p.position.X + 1000, 675, zones[0], 4);
                        soldiers.Add(s);
                        isWaveCoolingDown = true;
                    }

                    
                }

                //check zone captures
                for(int i =0; i<zones.Count; i++)
                {
                    if (zones[i].isCaptured == true && i<zones.Count-1)
                    {
                        zones[i + 1].isLastCaptured = true;
                        continue;
                    }

                    if (p.pBB.Intersects(zones[i].zBB))
                    {
                        zones[i].isPlayerInZone = true;
                        
                    }
                    else
                    {
                        zones[i].isPlayerInZone = false;
                        
                    }
                    if (e.eBB.Intersects(zones[i].zBB))
                    {
                        zones[i].isEnemyInZone = true;
                    }
                    else
                    {
                        zones[i].isEnemyInZone = false;
                    }

                    if (soldiers.Count == 0)
                    {
                        zones[i].isSoldierInZone = false;
                    }
                        for (int j = 0; j < soldiers.Count; j++)
                        {
                            if (soldiers[j].isInZone)
                            {
                                zones[i].isSoldierInZone = true;
                                break;
                            }
                            else
                            {
                                zones[i].isSoldierInZone = false;
                            }

                        }
                    
                    


                }
 

                //check if player is in zone for enemy AI
                e.isPlayerInZone = false;
                for(int i = 0; i < zones.Count; i++)
                {
                    if (p.pBB.Intersects(zones[i].zBB))
                    {
                        e.isPlayerInZone = true;
                        break;
                    }
                }

                //update first zone for soldier AI
                
                    for (int i = 0; i < zones.Count; i++)
                    {
                        if (zones[i].isCaptured)
                        {
                            for (int j = 0; j < soldiers.Count; j++)
                            {
                            if (i + 1 < zones.Count)
                            {
                                soldiers[j].firstZone = zones[i + 1];

                            }



                        }
                        }
                    }
                
                    //removing dying soldiers
                for(int i = 0; i< deadSoldiers.Count; i++)
                {
                    if (deadSoldiers[i].isDone)
                    {
                        deadSoldiers.Remove(deadSoldiers[i]);
                    }
                }

                if(dr != null)
                {
                    if (dr.isDone)
                    {
                        dr = null;
                    }
                }
                
                

                //update objects

                p.Update(gameTime, e.position, arrows);

                
                e.Update(p.position, gameTime);

                

                foreach(DyingSoldier d in deadSoldiers)
                {
                    d.Update(gameTime);
                }
                if (dr != null)
                {
                    dr.Update(gameTime);
                }

                //soldier update

                
                    foreach (Soldier s in soldiers)
                    {
                        s.Update(p, gameTime);
                    }
 
                foreach (Arrow a in arrows)
                {
                    a.Update();

                }
                foreach(Zone z in zones)
                {
                    z.Update(gameTime);
                }

                for(int i = 0; i<soldiers.Count; i++)
                {
                    if (soldiers[i].soldierBB.Intersects(soldiers[i].firstZone.zBB))
                    {
                        soldiers[i].isInZone = true;
                    }
                    else
                    {
                        soldiers[i].isInZone = false;
                    }
                }

                //if x is pressed, ability 2 (this is first because of sniper ab 2)
                if ((Keyboard.GetState().IsKeyDown(Keys.X) && prevKeyboard.IsKeyDown(Keys.X) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) && prevGamePad.IsButtonDown(Buttons.A) == false))
                {
                    p.abilityTwo(e, soldiers);
                }
                //if z is pressed, ability one
                if ((Keyboard.GetState().IsKeyDown(Keys.Z) && prevKeyboard.IsKeyDown(Keys.Z) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X) && prevGamePad.IsButtonDown(Buttons.X) == false))
                {
                    p.abilityOne(e, soldiers, arrows, Content);

                }
                if (pType.Equals(typeof(Tank)))
                {
                    if ((Keyboard.GetState().IsKeyDown(Keys.C) && prevKeyboard.IsKeyDown(Keys.C) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B) && prevGamePad.IsButtonDown(Buttons.B) == false))
                    {
                        p.abilityThree(e);
                    }
                }

                //if arrow keys pressed, basic attack
                if (pType.Equals(typeof(Tank)))
                {
                    if ((Keyboard.GetState().IsKeyDown(Keys.Right) && prevKeyboard.IsKeyDown(Keys.Right) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger) && prevGamePad.IsButtonDown(Buttons.RightTrigger) == false)){
                        if (!p.isBasicAttackCoolingDown)
                        {
                            p.basicAttack(e, soldiers, arrows, 1, Content);
                        }
                    }

                }
                if (pType.Equals(typeof(Sniper)))
                {
                    if ((Keyboard.GetState().IsKeyDown(Keys.Right) && prevKeyboard.IsKeyDown(Keys.Right) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightThumbstickRight) && GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger) && prevGamePad.IsButtonDown(Buttons.RightTrigger) == false))
                    {
                        if (!p.isBasicAttackCoolingDown)
                        {
                            p.basicAttack(e, soldiers, arrows, 1, Content);

                        }


                    }
                    else if ((Keyboard.GetState().IsKeyDown(Keys.Up) && prevKeyboard.IsKeyDown(Keys.Up) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightThumbstickUp) && GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger) && prevGamePad.IsButtonDown(Buttons.RightTrigger) == false))
                    {
                        if (!p.isBasicAttackCoolingDown)
                        {
                            p.basicAttack(e, soldiers, arrows, 2, Content);
                        }

                    }
                    else if ((Keyboard.GetState().IsKeyDown(Keys.Left) && prevKeyboard.IsKeyDown(Keys.Left) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightThumbstickLeft) && GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger) && prevGamePad.IsButtonDown(Buttons.RightTrigger) == false))
                    {
                        if (!p.isBasicAttackCoolingDown)
                        {
                            p.basicAttack(e, soldiers, arrows, 3, Content);

                        }

                    }
                    else if ((Keyboard.GetState().IsKeyDown(Keys.Down) && prevKeyboard.IsKeyDown(Keys.Down) == false) || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightThumbstickDown) && GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger) && prevGamePad.IsButtonDown(Buttons.RightTrigger) == false))
                    {
                        if (!p.isBasicAttackCoolingDown)
                        {
                            p.basicAttack(e, soldiers, arrows, 4, Content);

                        }

                    }
                }
                
                
                
                //check collision between arrows and soldiers and enemies
                for (int i = arrows.Count-1; i >= 0; i--) {
                    
                    for (int j = 0; j < soldiers.Count; j++)
                    {
                        if (arrows[i].arrowBB.Intersects(soldiers[j].soldierBB))
                        {
                            soldiers[j].health -= p.damage;
                            arrows.Remove(arrows[i]);
                            break;
                        }
                    }

                }

                //enemy-arrow collision

                for (int i = arrows.Count-1; i>=0; i--)
                {
                    if (arrows[i].arrowBB.Intersects(e.eBB))
                    {
                        e.health = e.health - p.damage;
                        arrows.Remove(arrows[i]);
                        
                    }
                }

                //soldier player distance
                if (pType.Equals(typeof(Tank))){
                    for(int i = 0; i < soldiers.Count; i++)
                    {
                        if(Vector2.Distance(soldiers[i].position, p.position)<=p.range)
                        {
                            soldiers[i].isInTankRange = true;
                        }
                        else
                        {
                            soldiers[i].isInTankRange = false;
                        }
                    }
                }

                //enemy basic attack
                
                if (e.isBasicAttackCoolingDown == false && e.hasAttacked == true && e.isInAttackRange==true)
                {
                    e.basicAttack(p, arrows, 1, Content);
                }

                //if enemy is rogue type and within player range and under half health, dashes back
                //doesn't dash if player about to die or if is sneaking
                if (eType.Equals(typeof(EnemyRogue))){
                    if (Vector2.Distance(e.position, p.position) < p.range && e.health <= e.maxHealth / 2 && p.health>35 && !e.isSneaking)
                    {
                        e.abilityOne(arrows, Content);
                    }
                }
                
                

                

                //ability two for rogue type - when close and above half health
                if (eType.Equals(typeof(EnemyRogue)))
                {
                    if(Vector2.Distance(e.position, p.position) < 400 && e.health >= e.maxHealth * 0.5)
                    {
                        if (!e.isSneaking)
                        {
                            e.abilityTwo();
                            
                        }

                    }
                }


            }
            prevKeyboard = state;
            prevGamePad = gState;
        
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            //start menu
            if (gameState == GameState.START)
            {
                GraphicsDevice.Clear(Color.Black);
                menu.Draw(spriteBatch, 0);
 
            }

            else if (gameState == GameState.CHOICE)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    spriteBatch.DrawString(font, "Press X for Tank", new Vector2(350, 150), Color.White);
                    spriteBatch.DrawString(font, "Press B for Sniper", new Vector2(700, 150), Color.White);
                } else
                {
                    spriteBatch.DrawString(font, "Press T for Tank", new Vector2(350, 150), Color.White);
                    spriteBatch.DrawString(font, "Press Y for Sniper", new Vector2(700, 150), Color.White);
                }
                
                spriteBatch.Draw(tankThumbnail, new Vector2(360, 200), Color.White);
                spriteBatch.Draw(archerThumbnail, new Vector2(710, 220), Color.White);
                spriteBatch.End();
            } else if (gameState == GameState.PAUSE)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Begin();
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    spriteBatch.DrawString(bigFont, "PAUSED", new Vector2(500, 30), Color.White);
                    spriteBatch.DrawString(font, "Controls:", new Vector2(250, 150), Color.White);
                    spriteBatch.DrawString(font, "Left thumbstick to move", new Vector2(250, 200), Color.White);
                    spriteBatch.DrawString(font, "Right Thumbstick + Right Trigger to attack", new Vector2(250, 250), Color.White);
                    spriteBatch.DrawString(font, "A/B/Y for abilities", new Vector2(250, 300), Color.White);
                    spriteBatch.DrawString(font, "Right Shoulder to resume", new Vector2(250, 350), Color.White);
                    spriteBatch.DrawString(font, "Left Shoulder to restart", new Vector2(250, 400), Color.White);
                    spriteBatch.DrawString(font, "Left menu to restart", new Vector2(250, 450), Color.White);

                    spriteBatch.DrawString(font, "Directions:", new Vector2(800, 150), Color.White);
                    spriteBatch.DrawString(font, "Kill enemies and capture red zones to win!", new Vector2(800, 200), Color.White);
                    spriteBatch.DrawString(font, "If enemies get too close, they will hurt you!", new Vector2(800, 250), Color.White);
                    spriteBatch.DrawString(font, "Beware! the enemy leader can dash and sneak", new Vector2(800, 300), Color.White);
                    spriteBatch.DrawString(font, "around the map", new Vector2(800, 330), Color.White);
                } else
                {
                    spriteBatch.DrawString(bigFont, "PAUSED", new Vector2(500, 30), Color.White);
                    spriteBatch.DrawString(font, "Controls:", new Vector2(250, 150), Color.White);
                    spriteBatch.DrawString(font, "WASD to move", new Vector2(250, 200), Color.White);
                    spriteBatch.DrawString(font, "Arrow keys to attack", new Vector2(250, 250), Color.White);
                    spriteBatch.DrawString(font, "Z/X/Y for abilities", new Vector2(250, 300), Color.White);
                    spriteBatch.DrawString(font, "O to resume", new Vector2(250, 350), Color.White);
                    spriteBatch.DrawString(font, "Enter to restart", new Vector2(250, 400), Color.White);
                    spriteBatch.DrawString(font, "Q to quit", new Vector2(250, 450), Color.White);

                    spriteBatch.DrawString(font, "Directions:", new Vector2(800, 150), Color.White);
                    spriteBatch.DrawString(font, "Kill enemies and capture red zones to win!", new Vector2(800, 200), Color.White);
                    spriteBatch.DrawString(font, "If enemies get too close, they will hurt you!", new Vector2(800, 250), Color.White);
                    spriteBatch.DrawString(font, "Beware! the enemy leader can dash and sneak", new Vector2(800, 300), Color.White);
                    spriteBatch.DrawString(font, "around the map", new Vector2(800, 330), Color.White);
                }
                

                spriteBatch.End();
            }

            //regular gameplay
            else if (gameState == GameState.PLAY) { 
                GraphicsDevice.Clear(Color.White);
                
                foreach(Background b in backgrounds)
                {
                    b.Draw(spriteBatch, p.drawPosition, p.position);
                }

                
                foreach (Zone z in zones)
                {
                    z.Draw(spriteBatch, p.drawPosition, p.position);
                }

                p.Draw(spriteBatch);
                e.Draw(spriteBatch, p.drawPosition, p.position);

               

                if (dr != null)
                {
                    dr.Draw(spriteBatch, p.position, p.drawPosition);
                }
                foreach(DyingSoldier d in deadSoldiers)
                {
                    d.Draw(spriteBatch, p.drawPosition, p.position);
                }
                foreach (Soldier s in soldiers)
                {
                    s.Draw(spriteBatch, p.drawPosition, p.position);
                }
                
                foreach (Arrow a in arrows)
                {
                    a.Draw(spriteBatch, p.drawPosition, p.position);
                }

                
                
            }
            //loss screen
            else if (gameState == GameState.LOSS)
            {
                GraphicsDevice.Clear(Color.White);
                menu.Draw(spriteBatch, 2);
               
            } else if(gameState == GameState.FREEZE)
            {
                GraphicsDevice.Clear(Color.White);

                foreach (Background b in backgrounds)
                {
                    b.Draw(spriteBatch, p.drawPosition, p.position);
                }


                foreach (Zone z in zones)
                {
                    z.Draw(spriteBatch, p.drawPosition, p.position);
                }

                p.Draw(spriteBatch);
                e.Draw(spriteBatch, p.drawPosition, p.position);



                if (dr != null)
                {
                    dr.Draw(spriteBatch, p.position, p.drawPosition);
                }
                foreach (DyingSoldier d in deadSoldiers)
                {
                    d.Draw(spriteBatch, p.drawPosition, p.position);
                }
                foreach (Soldier s in soldiers)
                {
                    s.Draw(spriteBatch, p.drawPosition, p.position);
                }

                foreach (Arrow a in arrows)
                {
                    a.Draw(spriteBatch, p.drawPosition, p.position);
                }
            }
            //win screen
            else if (gameState == GameState.WIN)
            {
                GraphicsDevice.Clear(Color.White);
                menu.Draw(spriteBatch, 3);
            }
            //for the restart frame
            else if(gameState == GameState.RESTART)
            {
                GraphicsDevice.Clear(Color.Black);
            }
            else
            {
                //this is just to make sure nothing weird is going on with gamemodes
                GraphicsDevice.Clear(Color.Red);
            }
        }
    }
}
