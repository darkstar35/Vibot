using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vibot.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Vibot.Actors
{
    class Actor_UI : IActor
    {

        public static Rectangle Blur_BlockRectangle;

        private Texture2D m_BloodyMask;
        public Texture2D m_ShadowMask;
        private Texture2D InGameUI;
        private Texture2D UI_GaugeCount;
        private Texture2D UI_GaugeCount2;
        private Texture2D WpointUI;
        private Texture2D MouseCursor;
        private Texture2D[] Block_Blur;
        private Texture2D UI_ClickGauge;

        public float Cusor_Spin;
        private byte m_FadeAlpha = 100;


        double shockTime; // 붉은 충격 효과 보여주는 시간 



        public Actor_UI(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;

            Block_Blur = new Texture2D[9];


            m_BloodyMask = m_ContentManager.Load<Texture2D>("Sprites\\UI\\bloody_UI");
            m_ShadowMask = m_ContentManager.Load<Texture2D>("Sprites\\UI\\shadow_UI");
            InGameUI = m_ContentManager.Load<Texture2D>("Sprites\\UI\\InGameUI");
            MouseCursor = m_ContentManager.Load<Texture2D>("Sprites\\UI\\cursor");
            UI_GaugeCount = m_ContentManager.Load<Texture2D>("Sprites\\UI\\InGameUI_Gauge");
            UI_GaugeCount2 = m_ContentManager.Load<Texture2D>("Sprites\\UI\\InGameUI_Gauge2");
            SystemFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameUIFont");
            UI_ClickGauge = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\Tiles");
            WpointUI = m_ContentManager.Load<Texture2D>("Sprites\\UI\\W-pointUI");
            for (int i = 0; i < 9; i++)
                Block_Blur[i] = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Block_Blor_Effect" + i.ToString());


        }





        public void OnDraw(GameTime gameTime, Actor_RedBlood RedBlood, Actor_Vibot Vibot)
        {

            Cusor_Spin += 1f;
            byte AlphaValue = 30;
            int number = (int)(gameTime.TotalGameTime.TotalSeconds % Rand.Next(8, 9));
            int YellowCount = 0, GreenCount = 0, PupleCount = 0, WhiteCount = 0;             // 바이오게이지 현황 UI 표기 

            m_SpriteBatch.Draw(InGameUI, new Vector2(0, 0), Color.White);   // 게임 UI 표기
            m_SpriteBatch.Draw(m_ShadowMask, new Vector2(0, 0), Color.White);   // 게임 화면 마스크 표기
      

           
      //   if (Vibot.BioGauge_Inven.Count > 0)
      //   {
      //       Vector2 displayVector = new Vector2(480, 650); //장전된 총알의 위치
      //
      //
      //       switch (Vibot.BioGauge_Inven.ElementAt<BioGauge>(0).Grade)
      //       {
      //           case 0:
      //               m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(0, 0, 47, 47), Color.White);
      //               break;
      //
      //           case 1:
      //               m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(80, 0, 47, 47), Color.White);
      //
      //               break;
      //           case 2:
      //               m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(155, 0, 47, 47), Color.White);
      //               break;
      //           case 3:
      //
      //               m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(234, 0, 47, 47), Color.White);
      //               break;
      //
      //       }
      //               for (int i = 1; i < Vibot.BioGauge_Inven.Count; i++)
      //               {
      //                   switch (Vibot.BioGauge_Inven.ElementAt<BioGauge>(i).Grade)
      //                   {
      //
      //                       case 0:
      //                           m_SpriteBatch.Draw(UI_GaugeCount, new Vector2(532 + 55*i, 655), new Rectangle(0, 0, 47, 47), Color.White);
      //                           WhiteCount++;
      //                           break;
      //                       case 1:
      //                           m_SpriteBatch.Draw(UI_GaugeCount, new Vector2(532 + 55 * i, 655), new Rectangle(80, 0, 47, 47), Color.White);
      //                           YellowCount++;
      //                           break;
      //                       case 2:
      //                           m_SpriteBatch.Draw(UI_GaugeCount, new Vector2(532 + 55 * i, 655), new Rectangle(155, 0, 47, 47), Color.White);
      //                           GreenCount++;
      //                           break;
      //                       case 3:
      //                           m_SpriteBatch.Draw(UI_GaugeCount, new Vector2(532 + 55 * i, 655), new Rectangle(230, 0, 47, 47), Color.White);
      //                           PupleCount++;
      //                           break;
      //
      //                   }
      //               }
      //
      //
      //
      //       }
            if(Stage.Scene_DestroyMode.Current_stage < 3)
            m_SpriteBatch.Draw(UI_GaugeCount, new Rectangle(500, 672, UI_GaugeCount.Width, UI_GaugeCount.Height), null, Color.White);
            else
             m_SpriteBatch.Draw(UI_GaugeCount2, new Rectangle(500, 650, UI_GaugeCount2.Width, UI_GaugeCount2.Height), null, Color.White);

            if (Vibot.BioGauge_Inven.Count > 0)
            {

                for (int i = 0; i < Vibot.BioGauge_Inven.Count; i++)
                {
                    switch (Vibot.BioGauge_Inven.ElementAt<BioGauge>(i).Grade)
                    {
                        case 0:
                            WhiteCount++;
                            break;
                        case 1:
                            YellowCount++;
                            break;
                        case 2:
                            GreenCount++;
                            break;
                        case 3:
                            PupleCount++;
                            break;

                    }
                }

                Vector2 displayVector = new Vector2(611, 595);
                switch (Vibot.BioGauge_Inven.ElementAt<BioGauge>(0).Grade)
                {
                    case 0:
                        m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(0, 0, 47, 47), Color.White);
                        break;
                    case 1:
                        m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(80, 0, 47, 47), Color.White);
                        break;
                    case 2:
                        m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(155, 0, 47, 47), Color.White);
                        break;
                    case 3:
                        m_SpriteBatch.Draw(UI_GaugeCount, displayVector, new Rectangle(234, 0, 47, 47), Color.White);
                        break;

                }

                if (WhiteCount > 0)
                    m_SpriteBatch.DrawString(SystemFont, WhiteCount.ToString(), new Vector2(540, 675), Color.White);
                else if (WhiteCount == 10)     
                    m_SpriteBatch.DrawString(SystemFont, WhiteCount.ToString(), new Vector2(540, 675), Color.Red);
                if (GreenCount > 0)
                    m_SpriteBatch.DrawString(SystemFont, GreenCount.ToString(), new Vector2(540 + 35 + 50, 675), Color.LightGreen);
                if (PupleCount > 0)
                    m_SpriteBatch.DrawString(SystemFont, PupleCount.ToString(), new Vector2(540 + 105 + 50, 675), Color.Purple);
                if (YellowCount > 0)
                    m_SpriteBatch.DrawString(SystemFont, YellowCount.ToString(), new Vector2(540 + 175 + 50, 675), Color.Yellow);
  

            }

    

            if (Mouse.GetState().X < cCamera.SCREEN_WIDTH && Mouse.GetState().Y < cCamera.SCREEN_HEIGHT)
            {
                Blur_BlockRectangle = new Rectangle((int)(Mouse.GetState().X + cCamera.CameraPosition.X), (int)(Mouse.GetState().Y + cCamera.CameraPosition.Y), 70, 70);

                m_SpriteBatch.Draw(Block_Blur[number], new Vector2(Mouse.GetState().X - 15, Mouse.GetState().Y - 15), null, new Color(0, 0, 0, AlphaValue < 70 ? AlphaValue + 1 : 0), 0f, new Vector2(70 / 2, 70 / 2), 0.3f, SpriteEffects.None, 0);
                m_SpriteBatch.Draw(MouseCursor, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), null, Color.White, (float)Math.PI * Cusor_Spin, new Vector2(MouseCursor.Width / 2, MouseCursor.Height / 2), 1f, SpriteEffects.None, 0);

            }

            if (RedBlood.Life_Count != 0)
            {
                m_SpriteBatch.DrawString(SystemFont, "Current Life :", new Vector2(10, 10), Color.White,
                0.0f, new Vector2(0.0f, 0.0f), 0.6f, SpriteEffects.None, 0);
                m_SpriteBatch.DrawString(SystemFont, RedBlood.Life_Count.ToString(), new Vector2(140, 10), Color.Tomato,
                0.0f, new Vector2(0.0f, 0.0f), 0.6f, SpriteEffects.None, 0);
            }

            //   if (Actor_RedBlood.Damaged) //If RedBlood Damaged
            //       RedBlood.Damaged = true;


            if (RedBlood.Damaged != Vector2.Zero)
                shockTime += gameTime.ElapsedGameTime.TotalMilliseconds;


            // 백혈구 올기는 게이지 보여주기 dhfrlsms rpdlwsl qhduwnrl 
            m_SpriteBatch.Draw(WpointUI, new Vector2(390, 680), null, Color.White, 0f, Vector2.Zero, new Vector2(0.3f, 1f), SpriteEffects.None, 1.0f);

            m_SpriteBatch.DrawString(SystemFont, "White Gauge\n culturing", new Vector2(300, 680), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
     //    m_SpriteBatch.DrawString(SystemFont, "Completed Receive W-Gage", new Vector2(600, 20), Color.White, 0f, Vector2.Zero, 0.51f, SpriteEffects.None, 0f);
     //       if (Vibot.WhiteGauge_Value > 1f)
                m_SpriteBatch.Draw(UI_ClickGauge, new Vector2(390, 690), new Rectangle(320, 0, 3, 3), Color.White, 0f, Vector2.Zero, new Vector2(Vibot.WhiteGauge_Value, 2.5f), SpriteEffects.None, 1.0f);

 
            if (shockTime < 1300 && RedBlood.Damaged != Vector2.Zero)
                m_SpriteBatch.Draw(m_BloodyMask, new Vector2(0, 0), new Color(m_FadeAlpha < 255 ? m_FadeAlpha + 1 : 255, 0, 0, 255));
            else if (shockTime > 1300)
            {

                m_FadeAlpha = 90;
                shockTime = 0;
                RedBlood.Damaged = Vector2.Zero;
            }

        }




        public override void LoadDataFromMap(XmlNode Node)
        {

        }

        public override void Destory()
        {

            //    m_GraphicDevice = null;
            //    m_ContentManager = null;
            //    m_SpriteBatch = null;
            //    m_SceneManager = null;
            //    m_ObjectFactory = null;
        }


    }
}
