using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace Vibot.Base
{

    public abstract class IScene : IObject
    {


        public Texture2D UI_tooltip_Arrows;
        public eFADESTATE m_FadeState = eFADESTATE.FADE_NONE;
        public int m_FadeAlpha;
        public int Parameter_Alpha = 255;


        protected Song m_BGM;
        protected Texture2D m_FadeTexture;

        


        public virtual void OnInitalize()
        {
           
        }


        public virtual void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
   
            SystemFont = m_ContentManager.Load<SpriteFont>("Fonts\\prologue");
            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");

            SystemFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameUIFont");


            m_FadeTexture = new Texture2D(m_GraphicDevice, 1280, 720, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1280 * 720];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color(Color.Black.ToVector3());
   
            m_FadeTexture.SetData(pixels);
     
        }

   
        public bool Fade_INorOUT() 
        {

            if (m_FadeState == eFADESTATE.FADE_OUT)
            {
                m_FadeAlpha -= 3;
                if (m_FadeAlpha <= 0)
                {
                    m_FadeAlpha = 0;
                   m_FadeState = eFADESTATE.FADE_NONE;
                     return true;
                }
            }
            else if (m_FadeState == eFADESTATE.FADE_IN)
            {
          
                m_FadeAlpha += 3;
                if (m_FadeAlpha >= 255)
                {
                    m_FadeAlpha = 255;
                    m_FadeState = eFADESTATE.FADE_OUT;
            
                }
            }
            

            if (m_FadeState != eFADESTATE.FADE_NONE) //main_Loading &&== eFADESTATE.FADE_NONE
            {
                 m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                m_SpriteBatch.Draw(m_FadeTexture, Vector2.Zero, new Color(255, 255, 255, (byte)MathHelper.Clamp(m_FadeAlpha, 0, 255)));
                m_SpriteBatch.End();
            }
            return false;
        }

        public virtual void OnUpdate(GameTime gameTime)
        {

   
        }
        public virtual void OnDraw(GameTime gameTime)
        {
        }

        public virtual bool ShowGameMsg(SpriteFont SystemFont,  float EventTime, float MaxSecond, String Msg, Vector2 Position)
        {
        

            if (EventTime < MaxSecond)       
            {
                if(m_FadeAlpha < 250)
                m_FadeAlpha += 3;
             //   SpriteBatch.End();
       
            }
            else if (EventTime > MaxSecond)
            {
            //    SpriteBatch.DrawString(SystemFont, Msg, Position, new Color(m_FadeAlpha, m_FadeAlpha, m_FadeAlpha, m_FadeAlpha));  //와 스트링은 페이드 적용이 왜 안되나 미치겠네...     
                m_FadeAlpha -= 3;

                if (m_FadeAlpha <= 30)
                {
                    EventTime = 0;
                    return true;

                
                }
            }
            m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            m_SpriteBatch.DrawString(SystemFont, Msg, Position, new Color(m_FadeAlpha, m_FadeAlpha, m_FadeAlpha, m_FadeAlpha));   // alpha 값이 높을수록 보인다
            m_SpriteBatch.End();

            return false;
        }   

        public abstract void Destory();

    }
}
