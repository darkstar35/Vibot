using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Vibot.Base;


namespace Vibot
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

 
    public enum eFADESTATE
    {
        FADE_NONE = 0,
        FADE_OUT,
        FADE_IN,
    }


    public partial class MainGame : Microsoft.Xna.Framework.Game
    {

        private GraphicsDeviceManager graphics;
        private SpriteBatch SpriteBatch;
        SpriteFont MainGameFont;
 
        Stage.Scene_DestroyMode Scene_DestroyMode;
        Scene.Scene_Intro Scene_Intro;
        Scene_Menu Scene_Menu;
        

        private Random main_rand = new Random();
        
        private const string FILE_NAME = @"Content\\Stage_Saved.data";
        FileStream fs;


        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 720;
           // graphics.PreferredBackBufferWidth = 853;
           // graphics.PreferredBackBufferHeight = 480;
                       
            graphics.IsFullScreen = false;

            Content.RootDirectory = "Content";
      //      this.Components.Add(new GamerServicesComponent(this));
     
            
        }

    

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            Scene_Intro =  new Scene.Scene_Intro();
            Scene_Menu = new  Scene_Menu(); 
            Scene_DestroyMode = new Stage.Scene_DestroyMode(GraphicsDevice, Content, SpriteBatch);

            MainGameFont = Content.Load<SpriteFont>("Fonts\\GameFont");


       }
        Byte[] CurrentClearedStages = new Byte[5];


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            if (!File.Exists(FILE_NAME)) // 존재하지않으면
            {
                fs = new FileStream(FILE_NAME, FileMode.CreateNew, FileAccess.Write);
                fs.Write(Scene_Menu.CurrentClearedStages, 0, 1);
                fs.Close();
            }
            else //존재 하면 기존에 있는것을  읽는다 
            {
                fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
                fs.Read(CurrentClearedStages, 0, 1);
                fs.Close();
                Scene_Menu.CurrentClearedStages[0] = CurrentClearedStages[0];
            }
            Scene_Intro.OnInitalize(GraphicsDevice, Content, SpriteBatch);
            Scene_Menu.OnInitalize(GraphicsDevice, Content, SpriteBatch);
            Scene_DestroyMode.OnInitalize();

        }


        protected override void UnloadContent()
        {

        }


        KeyboardState previousKeyboard;
        bool sound_off = true;

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
           KeyboardState keyboard = Keyboard.GetState();

           sound_off = true;
           if (keyboard.IsKeyDown(Keys.O) && !previousKeyboard.IsKeyDown(Keys.O))
           {
               if (sound_off)
               {
                   MediaPlayer.Pause();
                   sound_off = false;
               }
               else
               {
                   MediaPlayer.Resume();
                   sound_off = true;
               }
           }
     
           previousKeyboard = keyboard;

           base.Update(gameTime);
     
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.White);

            switch (Scene_Menu.GameState)
            {
                case eGameState.GAMEINTRO:

                    Scene_Intro.OnUpdate(gameTime);
                    Scene_Intro.OnDraw(gameTime);


                 //  if (IScene.m_FadeState == eFADESTATE.FADE_OUT)
                   if(Scene_Intro.m_FadeState == eFADESTATE.FADE_OUT)
                        Scene_Menu.GameState = eGameState.MAINMENU;
               
                    break;
                    
                    //대기------------------------------------------
                case eGameState.CREDIT:
                    Scene_Menu.OnUpdate(gameTime);
                    Scene_Menu.OnDraw(gameTime);
                    break;
                    //대기------------------------------------------
       

                case eGameState.MAINMENU:
                    Scene_Menu.OnUpdate(gameTime);
                    Scene_Menu.OnDraw(gameTime);
                  
                    break;

                case eGameState.GAME_MENU: // 게임중 중단 메뉴 
                    
                    Scene_DestroyMode.OnDraw(gameTime); // 그려주기만 한다 
                  
                    Scene_Menu.OnUpdate(gameTime);
                    Scene_Menu.OnDraw(gameTime);

                    if (Scene_Menu.ISResetStage) // 중간 빠져서 나오고 다시 실행하면 리셋시켜준다 
                    {
                        Scene_DestroyMode.Destory();
                        Scene_Menu.ISResetStage = false;
                    }
                    break;


                case eGameState.GAME_PAUSE:
                    Scene_DestroyMode.OnDraw(gameTime);

                   break;

                case eGameState.GAME_PLAYING:


                    Scene_DestroyMode.OnUpdate(gameTime);
                    if (Scene_DestroyMode.StageState == Stage.eStageState.STAGE_INTIALIZE)
                    {
                        CurrentClearedStages[0] = (byte)Stage.Scene_DestroyMode.Current_stage;

                        //대기-----------------------------------------------------------
                        if (Scene_Menu.CurrentClearedStages[0] < CurrentClearedStages[0])
                            Scene_Menu.CurrentClearedStages[0] = CurrentClearedStages[0];
                   
                        //---------------------------------------------------------------
                    }
                    
                    Scene_DestroyMode.OnDraw(gameTime);
                    Scene_Menu.OnUpdate(gameTime);
                   
                    if(Scene_Menu.GameState != eGameState.GAME_PLAYING)
                    Scene_Menu.OnDraw(gameTime);

                    break;

                case eGameState.GAMEEXIT:
                    fs = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write);
                    fs.Write(Scene_Menu.CurrentClearedStages, 0, 1);
                    fs.Close();

                    //test_fs = new FileStream(TEST_NAME, FileMode.Create, FileAccess.Write);
                    //test_fs.Write(Scene_Menu.CurrentClearedStages, 0, 1);
                    //test_fs.Close();

                    this.Exit();
                    break;

            }
      
          base.Draw(gameTime);
       
        }



        


    }
}

