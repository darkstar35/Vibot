﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vibot.Base;
using Vibot.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Vibot.Scene
{
    class Scene_Gameover : IScene
    {
        private Texture2D m_OverLogo; 


        private eFADESTATE m_FadeState = eFADESTATE.FADE_NONE;
      //  private int m_FadeAlpha = 255;
        
        public override void SetClassName(string ClassName) { }

        public override IObject Instance() { return (IObject)new Scene_Gameover(); }

        public override void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, SceneManager SceneManager, CObjectFactory ObjectFactory)
        {
            base.OnInitalize(GraphicDevice, ContentManager, SpriteBatch, SceneManager, ObjectFactory);

            m_FadeTexture = new Texture2D(m_GraphicDevice, 1280, 720, false, SurfaceFormat.Color);
            Color[] colors = new Color[1280 * 720];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(Color.Black.ToVector3());
            }
            m_FadeTexture.SetData(colors);

            m_OverLogo = m_ContentManager.Load<Texture2D>("Sprites\\fail");  //Game Over 화면

            m_FadeState = eFADESTATE.FADE_IN;
        }
        public override void OnUpdate(GameTime gameTime)
        {
            if (m_FadeState != eFADESTATE.FADE_NONE)
            {
                if (m_FadeState == eFADESTATE.FADE_IN)
                {
                    m_FadeAlpha -= 1;
                    if (m_FadeAlpha < 0)
                    {
                        m_FadeState = eFADESTATE.FADE_OUT;
                        m_FadeAlpha = 0;
                    }
                }
                else if (m_FadeState == eFADESTATE.FADE_OUT)
                {
                    m_FadeAlpha += 1;
                    if (m_FadeAlpha > 255)
                    {
                        Actor_RedBlood.Life_Count = 7;
                        m_FadeState = eFADESTATE.FADE_NONE;
                        m_FadeAlpha = 255;
                        m_SceneManager.ChangeScene("Scene_Menu");
                      

                    }
                }
            }
        }
        public override void OnDraw(GameTime gameTime)
        {
          

            if (m_FadeState != eFADESTATE.FADE_NONE)
            {

                m_SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                m_SpriteBatch.Draw(m_OverLogo, new Vector2(0, 0), Color.White);
                m_SpriteBatch.End();
                m_SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                m_SpriteBatch.Draw(m_FadeTexture, new Vector2(0, 0), new Color(255, 255, 255, (byte)MathHelper.Clamp(m_FadeAlpha, 0, 255)));
                m_SpriteBatch.End();

            }
        }
        public override void HandleMessage()
        {

        }
        public override void Destory()
        {

        }
    }
}
