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
using Microsoft.Xna.Framework.Media;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using System.Diagnostics;


namespace Vibot
{

    public enum BioTrapMODE { WAIT, RUNNING, READY };

    public class BioTrap : CElement
    {
        public const float BoomTrapTime = 3.0f; //바이오 트랩 터지는 타이머, 3초
        public float TrapTime = 0.0f;

        public Vector2 Bullet_Position = Vector2.Zero;

        public Texture2D ReadyTexture;
        public BioTrapMODE BioTrapMODE = BioTrapMODE.READY;



        public float ShotSpeed = 1;



        public BioTrap(Vector2 position, int Type, double shotangle) :
            base()
        {

            m_Type = Type;

            switch (m_Type)
            {
                case 1:
                    width = 30;
                    height = 200;
                    mass = 100;
                    m_Rect = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);

                    SetUpPhysics(world, position, width, height, mass);
                    this.body.Friction = 30;
                    this.body.Restitution = 30f;

                    break;

                case 2:
                    m_Rect = new Rectangle((int)position.X, (int)position.Y, 400, 5);
                    break;
                case 3:
                    m_Rect = new Rectangle((int)position.X, (int)position.Y, 10, 250);
                    break;

            }

            m_angle = shotangle;

        }

        public BioTrap(Vector2 postion, int Type) :
            base()
        {

            m_Rect.X = (int)postion.X;
            m_Rect.Y = (int)postion.Y;
            m_Type = Type;

        }

        public void OnUpdate(GameTime gameTime)
        {
            TrapTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TrapTime < BoomTrapTime)
            {
                BioTrapMODE = Vibot.BioTrapMODE.RUNNING;

                m_LifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


                switch (m_Type)
                {
                    case 1:
                      //  this.bodyWorldPosition += new Vector2((float)Math.Cos(m_angle), (float)Math.Sin(m_angle)) * 8;

                        break;

                    case 2:
                        m_Rect.X += (int)(Math.Cos(m_angle) * 30);
                        m_Rect.Y += (int)(Math.Sin(m_angle) * 30);
                        break;

                    case 3:
                        m_Rect.X += (int)(Math.Cos(m_angle) * 10);
                        m_Rect.Y += (int)(Math.Sin(m_angle) * 10);
                        // Boom(해당방향으로 )
                        break;

                }


            }



            switch (BioTrapMODE)
            {
                case Vibot.BioTrapMODE.READY:

                    break;

                case Vibot.BioTrapMODE.WAIT:

                    break;

                case Vibot.BioTrapMODE.RUNNING:

                    break;

            }




        }






    }
}


