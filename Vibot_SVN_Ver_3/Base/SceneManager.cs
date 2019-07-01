using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Vibot.Scene;
using Vibot.Actors;


namespace Vibot.Base
{
    public enum eFADESTATE
    {
        FADE_NONE = 0,
        FADE_OUT,
        FADE_IN,
    }


    public class SceneManager
    {
        
        private GraphicsDevice m_GraphicDevice = null;
        private ContentManager m_ContentManager = null;
        private SpriteBatch m_SpriteBatch = null;

        private IScene m_CurrentScene = null;
        private IScene m_PreviousScene = null;
        private CObjectFactory m_ObjectFactory = null;
    
        public eFADESTATE m_FadeStage = eFADESTATE.FADE_NONE;
        public int m_FadeAlpha = 0;

        private Texture2D m_FadeTexture = null;
        public bool m_Loading = false;
       

        public CellField CellField;
        private double backcellmovement;
        const float cellsParallaxPeriod = 28f;  // 
        const float cellsParallaxAmplitude = 1028f; // BackGround Cell 진폭

        public void SetObjectory(CObjectFactory ObjectFactory)
        {
            //m_ObjectFactory = ObjectFactory;
        }

        public void CreateScene(string SceneName)
        {
            if (m_ObjectFactory == null)
            {
                MessageBox.Show("ObjectFactory를 설정하세요");
                return;
            }

        
/*
            if (m_PreviousScene != null)
            {
              
                m_PreviousScene.Destory();
                
                m_PreviousScene = null;
            }
            */

                IObject Object = m_ObjectFactory.CreateObjectClass(SceneName);

                if (Object == null)
                {
                    MessageBox.Show("등록되지 않는 오브젝트입니다");
                    return;
                }
          
            m_CurrentScene = (IScene)Object;
            m_CurrentScene.OnInitalize(m_GraphicDevice, m_ContentManager, m_SpriteBatch, this);
            m_Loading = true;
            m_PreviousScene = m_CurrentScene;
        }

        public void ChangeScene(string SceneName)
        {
            if (m_ObjectFactory == null)
            {
                MessageBox.Show("ObjectFactory를 설정하세요");
                return;
            }
           // m_CurrentScene.Destory();///////////////
 
         //   m_CurrentScene = null;

            m_FadeStage = eFADESTATE.FADE_OUT;
            CreateScene(SceneName);
        }

        public void Update(GameTime gameTime)
        {

            if (m_FadeStage != eFADESTATE.FADE_NONE)
            {
                if (m_FadeStage == eFADESTATE.FADE_OUT)
                {
                    m_FadeAlpha += 3;
                    if (m_FadeAlpha > 255)
                    {
                        m_FadeStage = eFADESTATE.FADE_IN;
                        m_FadeAlpha = 255;
                    }
                }
                else if (m_FadeStage == eFADESTATE.FADE_IN)
                {
                    m_FadeAlpha -= 3;
                    if (m_FadeAlpha < 0)


                    {
                        m_FadeStage = eFADESTATE.FADE_NONE;
                        m_FadeAlpha = 0;
                    }

       
                    
                }
            }
            if (m_Loading && m_FadeStage == eFADESTATE.FADE_NONE)
                if (m_CurrentScene != null)
                m_CurrentScene.OnUpdate(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
        m_GraphicDevice.Clear(Color.Black);
        //   m_GraphicDevice.Clear(Color.White);
        
           backcellmovement += gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 position = Vector2.Multiply(new Vector2(
                    (float)Math.Cos(backcellmovement / cellsParallaxPeriod),
                    (float)Math.Sin(backcellmovement / cellsParallaxPeriod)),
                    cellsParallaxAmplitude);
            // draw the cells
            CellField.Draw(position);

       


            if (m_FadeStage == eFADESTATE.FADE_NONE || m_FadeStage == eFADESTATE.FADE_IN)
            {
                if(m_CurrentScene != null )
                    m_CurrentScene.OnDraw(gameTime);
             
            }
            else if (m_FadeStage == eFADESTATE.FADE_OUT)
            {
         //      if(m_PreviousScene != null)
                 

            }
        
            if (m_FadeStage != eFADESTATE.FADE_NONE)
            {
                m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); //.FrontToBack
                m_SpriteBatch.Draw(m_FadeTexture, new Vector2(0, 0), new Color(255, 255, 255, (byte)MathHelper.Clamp(m_FadeAlpha, 0, 255)));
              m_SpriteBatch.End();
            }
           
        }

 
        public SceneManager(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
            //m_ObjectFactory = ObjectFactory;

            m_FadeTexture = new Texture2D(m_GraphicDevice, 1280, 720, false, SurfaceFormat.Color);
            Color[] colors = new Color[1280 * 720];
      
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(Color.Black.ToVector3());
            }
            m_FadeTexture.SetData(colors);

            CellField = new CellField(Vector2.Multiply(new Vector2(
                    (float)Math.Cos(backcellmovement / cellsParallaxPeriod),
                    (float)Math.Sin(backcellmovement / cellsParallaxPeriod)),
                    cellsParallaxAmplitude), GraphicDevice, ContentManager);
            CellField.LoadContent();

        }

   
    }
   
    public class VibotObjectoryFactory : CObjectFactory
    {
        public override void RegisterObject()
        {
            IObject Object = new Scene_Intro();
            this.AddObjectClass("Scene_Intro", Object.Instance);


            Object = new Scene_Menu();
            this.AddObjectClass("Scene_Menu", Object.Instance);
 
            Object = new Scene_Tutorial();
            this.AddObjectClass("Scene_Tutorial", Object.Instance);

            Object = new Scene_Stage0();
            this.AddObjectClass("Scene_Stage0", Object.Instance);

            Object = new Scene_Stage1();
            this.AddObjectClass("Scene_Stage1", Object.Instance);
            /*
 Object = new Scene_Tutorial2();
 this.AddObjectClass("Scene_Tutorial2", Object.Instance);
 
 Object = new Scene_Tutorial3();
 this.AddObjectClass("Scene_Tutorial3", Object.Instance);
    
          
           
            Object = new Actor_UI();
            this.AddObjectClass("Actor_UI", Object.Instance);

            Object = new Actor_StageBackGround();
            this.AddObjectClass("Actor_StageBackGround", Object.Instance);

            Object = new Actor_Vibot();
            this.AddObjectClass("Actor_Vibot", Object.Instance);
         
            Object = new Actor_RedBlood();
            this.AddObjectClass("Actor_RedBlood", Object.Instance);

            Object = new Actor_Virus();
            this.AddObjectClass("Actor_Virus", Object.Instance);

            Object = new Actor_Objects();
            this.AddObjectClass("Actor_Objects", Object.Instance);

            Object = new Parasite();
            this.AddObjectClass("Parasite", Object.Instance);

            Object = new N7_NaturalCell();
            this.AddObjectClass("N7_NaturalCell", Object.Instance);

            Object = new N8_NaturalCell();
            this.AddObjectClass("N8_NaturalCell", Object.Instance);

            Object = new Chole_Object();
            this.AddObjectClass("Chole_Object", Object.Instance);

            Object = new H5();
            this.AddObjectClass("H5", Object.Instance);

            Object = new Flu();
            this.AddObjectClass("Flu", Object.Instance);

            Object = new Bacillus();
            this.AddObjectClass("Bacillus", Object.Instance); 


  
                    */

        }
    }
  
}
