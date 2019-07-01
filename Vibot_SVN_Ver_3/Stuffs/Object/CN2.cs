using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vibot.Base;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


namespace Vibot.Actors
{
    public class CN2 : IEnemy  //중립세포
    {
        private int m_Direction = 0;
        public override IObject Instance() { return (IObject)new CN2(); } // XML에 등록하기위한 인스탄스
        private SoundEffect m_DieSound;

  public  CN2()
    {
        height = 60;
        width = 60;
        mass = 10;
    }

    ~CN2()
    {


    }

        public override void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, CObjectFactory ObjectFactory, IActor Actor)
        {
            base.OnInitalize(GraphicDevice, ContentManager, SpriteBatch, ObjectFactory, Actor);
            m_Texture = m_ContentManager.Load<Texture2D>("Sprites/N2");
            m_DieSound = m_ContentManager.Load<SoundEffect>(@"Audio\\virus_die");

            Random Rand = new Random();
            m_Direction = Rand.Next(10);

        }


        public override void OnUpdate(GameTime gameTime)
        {

            enemy_count++;

            switch (m_Direction)
            {
                case 0:
                    {
                        m_Pos.X -= m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 1:
                    {
                        m_Pos.X -= m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 2:
                    {
                        m_Pos.X += m_cellspeed;
                        m_Pos.Y += m_cellspeed;

                    }
                    break;
                case 3:
                    {

                        m_Pos.Y += m_cellspeed;
                        m_Pos.X -= m_cellspeed;
                    }
                    break;
                case 4:
                    {
                        m_Pos.X -= m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 5:
                    {
                        m_Pos.X += m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 6:
                    {
                        m_Pos.X -= m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 7:
                    {
                        m_Pos.X += m_cellspeed;
                        m_Pos.Y -= m_cellspeed;
                    }
                    break;
                case 8:
                    {
                        m_Pos.X -= m_cellspeed;
                        m_Pos.Y += m_cellspeed;
                    }
                    break;
                case 9:
                    {
                        m_Pos.X += m_cellspeed;
                        m_Pos.Y -= m_cellspeed;
                    }
                    break;
            }

   
      



        }


        public override void HandleMessage(CMsg<eEnemyMsg> Msg)
        {
            switch (Msg.m_Msg)
            {
                default:
                    base.HandleMessage(Msg);
                    break;
            }
        }
        public override void Destory()
        {
        }
    }
}
