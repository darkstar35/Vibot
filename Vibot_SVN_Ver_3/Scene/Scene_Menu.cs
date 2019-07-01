using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vibot.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using System.Diagnostics;

namespace Vibot
{
    public enum eGameState
    {
        GAMEINTRO,
        MAINMENU,
        //GAME_NEWSTART,
        GAME_OPTION,
        GAME_PLAYING,
        GAME_PAUSE,
        GAME_MENU,
        GAMEEXIT,
        CREDIT,

    }

    public class Scene_Menu : IScene
    {
        public static eGameState GameState = eGameState.GAME_PLAYING;

        //Images
        private Texture2D m_Background;
        public Texture2D[] img_GMBttns = new Texture2D[3];

        private Texture2D credits;
    
        public Texture2D MouseCusor;
        public Rectangle MouseCusor_Rectangle;


        public List<Rectangle> cellbuttons_RectList;
        public SpriteFont main_SystemFont;

        public int MenuPoint = 0;
        public Byte[] CurrentClearedStages = new Byte[5];
        
        //Music
        private Song m_Opening;
        private SoundEffect m_StartSound;

       
        public bool main_Loading = false;

        private KeyboardState preKeyState;
        private Texture2D CellTexture;
        public bool ISResetStage;


        public override void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {

            base.OnInitalize(GraphicDevice, ContentManager, SpriteBatch);

            CellTexture = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\menucell");

            cellbuttons_RectList = new List<Rectangle>();
        
            cellbuttons_RectList.Add(new Rectangle(350, 500, 200, 100)); // Option
            cellbuttons_RectList.Add(new Rectangle(550, 501, 200, 100));  // Credit
            cellbuttons_RectList.Add(new Rectangle(750, 510, 200, 100));  //  Exit
            cellbuttons_RectList.Add(new Rectangle(300, 350, 100, 90));  // Stage0

            for (int i = 0; i < (int)CurrentClearedStages[0]; i++)
                cellbuttons_RectList.Add(new Rectangle(450 + i * 150, 350, 100, 90));

           MouseCusor_Rectangle = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 15, 15);

           MouseCusor = ContentManager.Load<Texture2D>("Sprites\\UI\\cursor");
            
            main_SystemFont = ContentManager.Load<SpriteFont>("Fonts\\prologue");
            m_Background = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\mainmenu_background");

            credits = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\credits");

        //    m_Title = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\M_title");

            for (int i = 0; i < 3; ++i)
             img_GMBttns[i] = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\gm" + i);

            m_Opening = ContentManager.Load<Song>("Audio\\BGM\\opening"); 
            m_StartSound = ContentManager.Load<SoundEffect>("Audio\\MAINMENU\\start");

           MediaPlayer.IsRepeating = true;
           MediaPlayer.Play(m_Opening);
      
  
        }


        public override void OnUpdate(GameTime gameTime)
        {
          KeyboardState keyState = Keyboard.GetState();
          MouseCusor_Rectangle.X = Mouse.GetState().X;
          MouseCusor_Rectangle.Y = Mouse.GetState().Y;

        switch(GameState)
         {
            case eGameState.GAME_PLAYING:
                    if (keyState.IsKeyDown(Keys.Escape) && !preKeyState.IsKeyDown(Keys.Escape))
                    {
                          
                        MenuPoint = 0;
                        GameState = eGameState.GAME_MENU;
                    }
                    break;
          
            
            case eGameState.GAME_MENU:


                    if (keyState.IsKeyDown(Keys.Up) && preKeyState.IsKeyUp(Keys.Up))
                        MenuPoint = MenuPoint == 0 ? 2 : MenuPoint - 1;         
                    else if (keyState.IsKeyDown(Keys.Down) && preKeyState.IsKeyUp(Keys.Down))
                        MenuPoint = MenuPoint == 2 ? 0 : MenuPoint + 1;
                    else if (keyState.IsKeyDown(Keys.Escape) && preKeyState.IsKeyUp(Keys.Escape))
                        GameState = eGameState.GAME_PLAYING;
                    else if (keyState.IsKeyDown(Keys.Enter) && preKeyState.IsKeyUp(Keys.Enter))
                    {
                        Fade_INorOUT();
                        switch (MenuPoint)
                        {
                            case 0:
                                GameState = eGameState.GAME_PLAYING;
                                break;
                            case 1:
                                ISResetStage = true;
                                GameState = eGameState.GAME_PLAYING;
                                break;
                            case 2:
                                
                                MenuPoint = 0;
                                ISResetStage = true;
                                GameState = eGameState.MAINMENU;
                                break;
                        }
                    }


                    break;

            case eGameState.CREDIT:
                    MouseState MOUSE_state1 = Mouse.GetState();
                    KeyboardState keyboard = Keyboard.GetState();

                    //if (MOUSE_state1.LeftButton == ButtonState.Pressed)
                    //{
                    //    if (cellbuttons_RectList[0].Contains(new Point(MouseCusor_Rectangle.X, MouseCusor_Rectangle.Y)))
                    //    {
                    //        Debug.WriteLine("크레딧,크레딧");
                    //        GameState = eGameState.MAINMENU; // CREDIT
                    //        break;
                    //    }
                    //}

                    if (keyboard.IsKeyDown(Keys.Escape))
                    {
                        Debug.WriteLine("크레딧,크레딧");
                        GameState = eGameState.MAINMENU; // CREDIT
                        break;
                    }

                    break;

            case eGameState.MAINMENU:
               
                
                MouseState MOUSE_state2 = Mouse.GetState();

                if(MOUSE_state2.LeftButton == ButtonState.Pressed)
                {
                    if (cellbuttons_RectList[0].Contains(new Point(MouseCusor_Rectangle.X, MouseCusor_Rectangle.Y)))
                    {
                     //   GameState = eGameState.GAME_OPTION;
                    }
                    else if (cellbuttons_RectList[1].Contains(new Point(MouseCusor_Rectangle.X, MouseCusor_Rectangle.Y)))
                    {
                        Debug.WriteLine("크레딧,메인메뉴");
                        GameState = eGameState.CREDIT; // CREDIT
                    }
                    else if (cellbuttons_RectList[2].Contains(new Point(MouseCusor_Rectangle.X, MouseCusor_Rectangle.Y)))
                        GameState = eGameState.GAMEEXIT;
                    else
                    {
                        for (int i = 3; i < cellbuttons_RectList.Count; i++)
                            if (cellbuttons_RectList[i].Contains(new Point(MouseCusor_Rectangle.X, MouseCusor_Rectangle.Y)))
                            {
                                Stage.Scene_DestroyMode.Current_stage = i - 3;
                                GameState = eGameState.GAME_PLAYING;
                                break;
                            }
                    }
                    
                }
        
           
           
            break;

      


        }
            preKeyState = keyState;

        }

 
     

        public override void OnDraw(GameTime gameTime)
        {
            Debug.WriteLine("GameState: "+GameState);
            
            if (GameState == eGameState.MAINMENU)
            {
                //대기-----------------------------------------------------------------
                cellbuttons_RectList = new List<Rectangle>();

                cellbuttons_RectList.Add(new Rectangle(350, 500, 200, 100)); // Option
                cellbuttons_RectList.Add(new Rectangle(550, 501, 200, 100));  // Credit
                cellbuttons_RectList.Add(new Rectangle(750, 510, 200, 100));  //  Exit
                cellbuttons_RectList.Add(new Rectangle(300, 350, 100, 90));  // Stage0

                for (int i = 0; i < (int)CurrentClearedStages[0]; i++)
                    cellbuttons_RectList.Add(new Rectangle(450 + i * 150, 350, 100, 90));
                //----------------------------------------------------------------------

                m_GraphicDevice.Clear(Color.AliceBlue);

                m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); //.FrontToBack

                m_SpriteBatch.Draw(m_Background, new Vector2(0, 0), Color.White);

                m_SpriteBatch.Draw(MouseCusor, MouseCusor_Rectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                m_SpriteBatch.Draw(CellTexture, cellbuttons_RectList[0], new Rectangle(0, 0, 150, 100), Color.BlueViolet, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                m_SpriteBatch.DrawString(main_SystemFont, "Option", new Vector2(363, 525), Color.BlueViolet, 0f, Vector2.Zero, 0.70f, SpriteEffects.None, 0f);

                m_SpriteBatch.Draw(CellTexture, cellbuttons_RectList[1], new Rectangle(0, 0, 150, 100), Color.Chartreuse, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                m_SpriteBatch.DrawString(main_SystemFont, "Credit", new Vector2(569, 526), Color.Chartreuse, 0f, Vector2.Zero, 0.71f, SpriteEffects.None, 0f);

                m_SpriteBatch.Draw(CellTexture, cellbuttons_RectList[2], new Rectangle(0, 0, 150, 100), Color.DarkGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                m_SpriteBatch.DrawString(main_SystemFont, "Exit", new Vector2(775, 530), Color.Gray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

                for (int i = 3; i < cellbuttons_RectList.Count; i++)
                {
                    m_SpriteBatch.Draw(CellTexture, cellbuttons_RectList[i], null, Color.White); //new Vector2(300 + (i - 3) * 150, 350)
                    m_SpriteBatch.DrawString(main_SystemFont, (i - 3).ToString(), new Vector2(335 + (i - 3) * 150, 370), Color.White);
                }

                m_SpriteBatch.End();
                Fade_INorOUT();
            }
            
            //대기 ---------------------------------------------------------------------------------------------------------------------------------------------
            else if (GameState == eGameState.CREDIT || GameState == eGameState.GAME_OPTION)
            {
                m_GraphicDevice.Clear(Color.Black);

                m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); //.FrontToBack

                //m_SpriteBatch.Draw(m_Background, new Vector2(0, 0), Color.White);

                //m_SpriteBatch.Draw(MouseCusor, MouseCusor_Rectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                //m_SpriteBatch.Draw(CellTexture, cellbuttons_RectList[0], new Rectangle(0, 0, 150, 100), Color.BlueViolet, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                m_SpriteBatch.Draw(credits, new Vector2(0, 0), Color.White);
                //5tm_SpriteBatch.DrawString(main_SystemFont, "Press 'Escape' to Exit.", new Vector2(50, 500), Color.BlueViolet, 0f, Vector2.Zero, 0.70f, SpriteEffects.None, 0f);

                m_SpriteBatch.End();
                Fade_INorOUT();
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------------

            else if (GameState == eGameState.GAME_MENU)
            {
                m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); 
                m_SpriteBatch.DrawString(main_SystemFont, "Pause Menu..", new Vector2(70, 20), Color.IndianRed);
                 for (int i = 0; i < 3; ++i) 
                   m_SpriteBatch.Draw(img_GMBttns[i], new Vector2(0, i == MenuPoint ? 50 + i * 70 + ((int)(5 * Math.Sin((float)gameTime.TotalGameTime.Milliseconds / 100.0f))) :
                        50 + i * 70), Color.White);
      
                m_SpriteBatch.End();

            }
           

        }

    
     
        public override void Destory()
        {
            //throw new NotImplementedException();
        }
        
           

    }
}
