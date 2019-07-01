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


using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using System.Diagnostics;


namespace Vibot.Actors
{
    enum BioGaugeType { YELLOW, VIOLET, GREEN };
    public enum BioGaugeMODE { DROPTEM, BULLET, ONVIBOT };

    public class BioGauge : Stuff
    {
        public float minBioGaugeTime = 8.0f; //바이오 게이지 죽는 타이머, 8초
        public float bioGaugeTime = 0.0f;

        bool SpinOnVibot = false;
        public int Grade = -1;
        public float Spincount = 0.9f;

        public Rectangle Gauge_Rect;
        public BioGaugeMODE Gauge_Mode = BioGaugeMODE.DROPTEM;

        public double Gauge_Spin_OnVibot = 0;


        public Vector2 Spin_Position = Vector2.Zero;
        public Vector2 Bullet_Position = Vector2.Zero;

        public float ShotSpeed = 1;
        double ShotAngle = 0f;



        public BioGauge(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 Position, BioGaugeMODE Gauge_mode, int grade) :
            base(GraphicDevice, ContentManager, SpriteBatch)
        {

            Gauge_Mode = Gauge_mode;
            Grade = grade;

            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Vibot\\combo" + Grade);
            mass = 5f;
            radius = 4;
            position = Position; 



            if (Gauge_Mode == BioGaugeMODE.DROPTEM)
                SetUpPhysics(world, position, radius, mass);
            else if (Gauge_Mode == BioGaugeMODE.ONVIBOT)
                Spincount = (float)Rand.NextDouble();
            else if (Gauge_Mode == BioGaugeMODE.BULLET)
                 Bullet_Position = Position; 
        
        }

   

        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {
            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));

            body.CollisionCategories = Category.Cat4;
            body.CollidesWith = Category.Cat4 | ~Category.Cat6;
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.3f;
            body.Friction = 3f;
        }



        public override bool CollisionToVibot()
        {

            //////////////////////// VIBOT 충돌  후 아이템 제거 
            if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), radius+30)))
            {
                m_HP -= 1f;
                return true;
            }
            else
                return false;


        }




        public override void OnUpdate(GameTime gameTime)
        {
          
            switch (Gauge_Mode)
            {

                case BioGaugeMODE.ONVIBOT:

             
                    break;
                case BioGaugeMODE.BULLET:
                    //    ShotSpeed++;

                    //       Vector2 Base_Position = new Vector2(posX, posY);

                    Bullet_Position += new Vector2((float)Math.Cos(ShotAngle), (float)Math.Sin(ShotAngle)) * ShotSpeed;
                    break;

                case BioGaugeMODE.DROPTEM:
                    base.OnUpdate(gameTime);
                  
                    
                    //if (!SpinOnVibot && whatTimeISit > 5f)
                    //{
                    //    m_Texture.Dispose();
                    //    body.Dispose();
                    //    // 필드에 있을떄 어느정도 되면 스스로 자멸하되 되는 코드
                    //}
                    break;

            }

        }

        public override void OnDraw(GameTime gameTime)
        {

            switch (Gauge_Mode)
            {
                case BioGaugeMODE.ONVIBOT:

                    Gauge_Spin_OnVibot++;

                    if (Gauge_Spin_OnVibot > 360)
                        Gauge_Spin_OnVibot = 0;

                    Spin_Position.X = (float)(40 * (Math.Cos(2 * Math.PI / 360 * Gauge_Spin_OnVibot)));
                    Spin_Position.Y = (float)(40 * (Math.Sin(2 * Math.PI / 360 * Gauge_Spin_OnVibot))); // 38

                    float posX = Actor_Vibot.Vibot_Laser_Element.m_Sphere.Center.X + Spin_Position.X - cCamera.CameraPosition.X;
                    float posY = Actor_Vibot.Vibot_Laser_Element.m_Sphere.Center.Y + Spin_Position.Y - cCamera.CameraPosition.Y;

                    Gauge_Rect = new Rectangle((int)posX - 15, (int)posY - 20, m_Texture.Width, m_Texture.Height);
                    m_SpriteBatch.Draw(m_Texture, Gauge_Rect, Color.White);

                    break;
                case BioGaugeMODE.DROPTEM:

                //  if (!SpinOnVibot)
                //      whatTimeISit += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_HP > 0) // || body != null)
                        m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition , null, Color.White, body.Rotation, m_TextureOrigin, m_HP, SpriteEffects.None, 0f);

                    break;

                case BioGaugeMODE.BULLET:
                    m_SpriteBatch.Draw(m_Texture, cCamera.Transform(Bullet_Position), null, Color.White,
                           0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                    break;


            }
        }




    }
}


