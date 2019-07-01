using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using Vibot.Base;
using Vibot.Actors;
using System.Diagnostics;

namespace Vibot.Stuffs
{
    public class Malignant : Stuff
    {
        public Texture2D shield0; //처음 이미지

        public SoundEffect collSound; //충돌시 효과음

        public Texture2D MalignantDie; //바이러스 죽을 때 이미지

        public Texture2D m_Texture0; //바이러스 기본 이미지
        public Texture2D m_Texture1; //바이러스 기본 이미지
        public Texture2D m_Texture2; //바이러스 기본 이미지
        public Texture2D m_Texture3; //바이러스 기본 이미지 
        public Texture2D m_Texture4; //바이러스 기본 이미지 
        public Texture2D m_Texture5; //바이러스 기본 이미지 

        public Texture2D[] VirusTexture = new Texture2D[6];


        const float Maxium_Speed = 2.0f;
        public Body ShieldBody;


        public Malignant(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 position, Vector2 direcitonvector)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {

            GameMsgFont = ContentManager.Load<SpriteFont>("Fonts\\prologue");
            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\virus_die2");
            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant");

            shield0 = ContentManager.Load<Texture2D>("Sprites\\Stages\\Tiles");
            MalignantDie = ContentManager.Load<Texture2D>("Sprites\\Viruses\\MalignantDie");

            m_Texture0 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant");
            m_Texture1 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant1");
            m_Texture2 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant2");
            m_Texture3 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant3");
            m_Texture4 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant4");
            m_Texture5 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Malignant5");

            collSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Virus\\malig\\malig");

        
            this.position = position;
            m_HP = 1;
            width = (float)m_Texture.Width;
            height = (float)m_Texture.Height;
            mass = 20f;
            radius = width / 2;
            DirectionVector = direcitonvector *(mass);

            
            SetUpPhysics(world, position, radius, mass);
            

        }
        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.5f;
            body.Friction = 0.5f;
            body.CollisionCategories = Category.Cat14;
            body.CollidesWith = ~Category.All;

          //  ShieldBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(30), ConvertUnits.ToSimUnits(5), 100);
            ShieldBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(30), 50);
            ShieldBody.BodyType = BodyType.Static;
            ShieldBody.Restitution = 0.1f;
            ShieldBody.CollisionCategories = Category.Cat10;
            ShieldBody.CollidesWith = ~Category.All | Category.Cat1 | Category.Cat2;

        }

        private float collTimerWithVibot = 0.0f; //봇과의 충돌 타이머
        private float collTimerWithTower = 0.0f; //타워와의 충돌 타이머
        private float minCollTimer = 1.0f;


        public override bool CollisionToVibot()
        {
            //////////////////////// VIBOT 충돌  후 사망 처리PART  ////////////////////////////////////


          if (!Defense_Sphere.Intersects(Actor_Vibot.Vibot_Laser_Element.m_Sphere) // 너무 안좋은 코드 

                && Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), radius))) //
            {
           //
           //   Debug.WriteLine("충돌2");
           //   collTimerWithVibot = 0.0f;
           //
              m_HP -= 0.3f;
              m_Texture = MalignantDie;
          
                return true;
            }
            else
                return false;
       
        }

        public override bool CollisionToTower(List<WhiteCell> Whitecell_List)
        {
         
         
           foreach (WhiteCell whitecell in Whitecell_List)
           {
               if (new BoundingSphere(
                       new Vector3(
                           bodyWorldPosition.X, 
                           bodyWorldPosition.Y, 0), m_Texture.Width / 2).Intersects(
                           new BoundingSphere(
                               new Vector3(
                                   whitecell.bodyWorldPosition.X,
                                   whitecell.bodyWorldPosition.Y, 0), 25)))
               {
                
                   collTimerWithTower = 0.0f;
                   m_HP -= 0.2f;
                   whitecell.m_HP -= 0.02f;

                   m_Texture = MalignantDie;
                   return true;
               }
         
           }
           return false;

        }

        public override bool CollisionToEndofWorld()
        {
            if (bodyWorldPosition.X + 40 < 0 || bodyWorldPosition.X - 40 > cCamera.MapSize.X || bodyWorldPosition.Y + 40 < 0 || bodyWorldPosition.Y + 40 > cCamera.MapSize.Y)
            {

                world.RemoveBody(ShieldBody);
                return true;

            }
            else
                return false;
        }

        public override bool CollisionToBlood(Actor_RedBlood Actor_RedBlood, GameTime gametime)
        {
            for (int i = 0; i < Actor_RedBlood.Blood_BodyList.Count; i++)
            {
                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0f), (float)(m_Texture.Width / 2.14))
                                             .Intersects(new BoundingSphere(new Vector3(ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.X), ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.Y), 0f), (float)(38 / 2.14)))
                      )
                {
                    m_HP -= 1.0f;
                    Actor_RedBlood.m_HP[i] -= 1f;


                    Actor_RedBlood.Damaged = Actor_RedBlood.Blood_BodyList[i].Position;
                    break;
                }
            }

            return false;
        }


   
        public override void OnUpdate(GameTime gameTime)  //움직이기
        {


            //////////////////////// 움직이기 처리 //////////////////////
            Vector2 force = Vector2.Zero;

            collTimerWithVibot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            collTimerWithTower += (float)gameTime.ElapsedGameTime.TotalSeconds;
            textureChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

             if (DirectionVector != null)
              body.ApplyForce(DirectionVector * (float)gameTime.ElapsedGameTime.TotalSeconds, this.body.Position);
    

              StopBodyAccelate(gameTime, 0.3f, Maxium_Speed); // 최대 속도 제한 


        }


        public double Barrier_Spin_OnMalignant = 0;
        public Vector2 Spin_Position = Vector2.Zero;
        public Vector2 Bullet_Position = Vector2.Zero;


        public int imageChangeCount = 0;
        private float textureChangeTimer = 0.0f;
        private float minTextureChangeTimer = 0.1f;


        public override void OnDraw(GameTime gameTime)
        {
            if (textureChangeTimer > minTextureChangeTimer)
            {
                switch (imageChangeCount % 6)
                {
                    case 0:
                        m_Texture = m_Texture0;
                        break;
                    case 1:
                        m_Texture = m_Texture1;
                        break;
                    case 2:
                        m_Texture = m_Texture2;
                        break;
                    case 3:
                        m_Texture = m_Texture3;
                        break;
                    case 4:
                        m_Texture = m_Texture4;
                        break;
                    case 5:
                        m_Texture = m_Texture5;
                        imageChangeCount = 5;
                        break;
                }
                imageChangeCount++;
                textureChangeTimer = 0.0f;
            }

            if (m_HP > 0 && m_Texture != null) // 
            {
                m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, body.Rotation, m_TextureOrigin, m_HP, SpriteEffects.None, 0f);

                //대기
            //    m_SpriteBatch.DrawString(font, "" + bodyViewPortPosition.X.ToString() + "," + bodyViewPortPosition.Y.ToString(), bodyViewPortPosition, Color.White,0.0f, new Vector2(0.0f, 0.0f), 0.5f, SpriteEffects.None, 0);
                Barrier_Spin_OnMalignant++;

                if (Barrier_Spin_OnMalignant > 360)
                    Barrier_Spin_OnMalignant = 0;

                Spin_Position.X = (float)(40 * (Math.Cos(2 * Math.PI / 360 * Barrier_Spin_OnMalignant)));
                Spin_Position.Y = (float)(40 * (Math.Sin(2 * Math.PI / 360 * Barrier_Spin_OnMalignant))); // 38


                ShieldBody.Position = ConvertUnits.ToSimUnits(bodyWorldPosition + Spin_Position);


                Defense_Sphere = new BoundingSphere(new Vector3(
                    (ConvertUnits.ToDisplayUnits(ShieldBody.Position.X)),
                     (ConvertUnits.ToDisplayUnits(ShieldBody.Position.Y)), 0), 15);

                m_SpriteBatch.Draw(shield0, cCamera.Transform(new Vector2(Defense_Sphere.Center.X, Defense_Sphere.Center.Y)),
                    new Rectangle(320, 0, 40, 20), Color.DarkCyan);
            }
          
        
            //대기
        //    if (Defense_Sphere != null)
        //    {
        //        m_SpriteBatch.DrawString(font, "" + Defense_Sphere.Center.X.ToString() + "," + Defense_Sphere.Center.Y.ToString(), new Vector2(Defense_Sphere.Center.X, Defense_Sphere.Center.Y), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 0.5f, SpriteEffects.None, 0);
        //    }

          //       m_SpriteBatch.Draw(shield0, Defense_Sphere, new Rectangle(320, 0, 30, 10), Color.DarkCyan, Block_Spin,
          //  new Vector2(30 / 2, 10 / 2), SpriteEffects.None, 0f);
          //  m_SpriteBatch.Draw(shield0, shieldPositon, new Rectangle(320, 0, 30, 10), Color.DarkCyan, Block_Spin,
          //  new Vector2(30 / 2, 10 / 2), 1f, SpriteEffects.None, 0f);
          //    m_SpriteBatch.Draw(shield0, Defense_Sphere, new Rectangle(320, 0, 30, 10), Color.DarkCyan, (float)Math.PI * Block_Spin,
          //   new Vector2(Defense_Sphere.Width / 2, Defense_Sphere.Height / 2), SpriteEffects.None, 0f);
            Block_Spin += 0.7f;
        }

        public float Block_Spin = 0;
        public BoundingSphere Defense_Sphere;
    }

}
