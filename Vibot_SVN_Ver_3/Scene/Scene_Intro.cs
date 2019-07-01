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



namespace Vibot.Scene
{
    class Scene_Intro : IScene
    {
        private Texture2D m_TeamLogo;
         Vector2 GG_Position;
         Vector2 HF_Position;
         Random rand;
  

         public Texture2D main_FadeTexture = null;


        float step_timing = 0;


         Rectangle[] Logo = new Rectangle[6];



         public override void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;




       
          m_FadeTexture = new Texture2D(m_GraphicDevice, 1280, 720, false, SurfaceFormat.Color);
          Color[] colors = new Color[1280 * 720];
          m_FadeAlpha = 255;
          Parameter_Alpha = 255;

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(Color.Black.ToVector3());
            }
            m_FadeTexture.SetData(colors);

            
            m_TeamLogo = m_ContentManager.Load<Texture2D>("Sprites\\MainMenu\\teamlogo");  //인트로화면

            Logo[0] = new Rectangle(115, 0, 209, 86); // I : GG
            Logo[1] = new Rectangle(115, 108, 281, 87); // You  : HF
            //   Logo[2] = new Rectangle(115, 108, 281, 85); // 
            Logo[3] = new Rectangle(188, 0, 134, 86); //  GG
            Logo[4] = new Rectangle(288, 109, 104, 85); //  HF
            Logo[5] = new Rectangle(60, 226, 302, 26); // Team Good Game Have Fun

           GG_Position = new Vector2(690 - 160, 360 - Logo[0].Height);
           HF_Position = new Vector2(690 - 160, 360 + 110 - Logo[1].Height);
           rand = new Random();

        }
         float endsecond = 4.0f;

         public override void OnUpdate(GameTime gameTime)
         {

             if (m_FadeState == eFADESTATE.FADE_NONE)
             {
                 step_timing += (float)gameTime.ElapsedGameTime.TotalSeconds;


                 if (step_timing > 2.3)
                 {
                     GG_Position = Vector2.SmoothStep(GG_Position, new Vector2(300, 360), 0.08f);
                     HF_Position = Vector2.SmoothStep(HF_Position, new Vector2(450, 360), 0.08f);

                     if (step_timing > endsecond) //&& 
                     {
                         m_FadeAlpha -= 3;
                         if (m_FadeAlpha < 0)
                         {
                             m_FadeAlpha = 255;
                             m_FadeState = eFADESTATE.FADE_OUT;
                         }
                     }
                 }

             }
         }


        public override void OnDraw(GameTime gameTime)
        {


            m_GraphicDevice.Clear(Color.Black);
            m_SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend); //.FrontToBack
            if (m_FadeState == eFADESTATE.FADE_NONE)
            {

                   if (step_timing > 0.5 && step_timing < 2.3)
                   {
                       m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(50 + 640 - 160, 360 - Logo[0].Height, 209, 86), Logo[0], new Color(255, 255, 255, (byte)Parameter_Alpha > 0 ? Parameter_Alpha - 5 : 5));

                       for (int bit = 1; bit <= 4; bit++)
                           m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(50 + 700, 350, 10, 10), new Rectangle((6 * ((int)gameTime.TotalGameTime.TotalMilliseconds % 500 * bit)), 0, 6, 6), new Color(255, 255, 255, (byte)Parameter_Alpha < 255 ? Parameter_Alpha + 1 : 255));

                       if (step_timing > 0.9 && step_timing < 2.3)
                       {
                           m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(50 + 675 - Logo[1].Width, 360 + 110 - Logo[1].Height, 250, 87), Logo[1], new Color(255, 255, 255, Parameter_Alpha > 0 ? Parameter_Alpha - 5 : 5)); //Parameter_Alpha > 0 ? Parameter_Alpha - 5 : 5
                           for (int bit = 1; bit <= 4; bit++)
                               m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(700, 450, 10, 10), new Rectangle((6 * ((int)gameTime.TotalGameTime.TotalMilliseconds % 500 * bit)), 0, 6, 6), new Color(255, 255, 255, (byte)Parameter_Alpha < 255 ? Parameter_Alpha + 1 : 255));
                       }
           

                   }


                   if (step_timing > 2.3)
                   {
                       if (step_timing < endsecond)
                       {
                           Parameter_Alpha += 5;
                           if (Parameter_Alpha >= 250)
                               Parameter_Alpha = 255;
                       }
                  //     m_SpriteBatch.Draw(m_TeamLogo, GG_Position, Logo[3], new Color(Parameter_Alpha, Parameter_Alpha, Parameter_Alpha, (byte)Parameter_Alpha));
                   //    m_SpriteBatch.Draw(m_TeamLogo, HF_Position, Logo[4], new Color(Parameter_Alpha, Parameter_Alpha, Parameter_Alpha, (byte)Parameter_Alpha));
                    //   m_SpriteBatch.Draw(m_TeamLogo, HF_Position, Logo[4], new Color(255, 255, 255, Parameter_Alpha )); // 이상하게 마지막에 이거 없으면 희미하게 지워짐;;
                       m_SpriteBatch.Draw(m_TeamLogo, GG_Position, Logo[3], new Color(255, 255, 255, Parameter_Alpha));
                       m_SpriteBatch.Draw(m_TeamLogo, HF_Position, Logo[4], new Color(255, 255, 255, Parameter_Alpha));

                       if ((step_timing > 2.6 && step_timing < 3.5) || (step_timing > 4.5 && step_timing < 5.5) || (step_timing > 6.5 && step_timing < 7.5)) // && Rand.Next(100,2000)%step_timing >= 0) 이 코드 안먹힘..
                       {
                        // m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(Logo[5].Width, 360 + 200 - Logo[5].Height, 303, 26), Logo[5], new Color(Parameter_Alpha, Parameter_Alpha, Parameter_Alpha, (byte)Parameter_Alpha));
                           m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(Logo[5].Width, 360 + 200 - Logo[5].Height, 303, 26), Logo[5], Color.White);
                          m_SpriteBatch.Draw(m_TeamLogo, new Rectangle(Logo[5].Width, 360 + 200 - Logo[5].Height, 303, 26), Logo[5], Color.White);
                       }

                  
                       
                   }

                   if (step_timing > endsecond) //  && m_FadeState == eFADESTATE.FADE_IN
                   {
                   
                       Parameter_Alpha -= 5;
                       if (Parameter_Alpha <= 30)
                           Parameter_Alpha = 0;

                       m_SpriteBatch.Draw(m_FadeTexture, new Vector2(0, 0), new Color(255, 255, 255, (byte)MathHelper.Clamp(m_FadeAlpha, 0, 255)));
               
                   }

        }
            m_SpriteBatch.End();
        }  


        public override void Destory()
        {
         
        }
    }
}
