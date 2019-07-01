using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
    public class Bacillus : Stuff
    {
        const float Maxium_Speed = 1.0f;

        public Texture2D bacil2; //첫번째 충돌시 이미지
        public Texture2D bacil3; //두번째 충돌시 이미지
        public Texture2D bacilDie; //두번째 충돌시 이미지
        public SoundEffect collSound; //충돌시 효과음
       

        
        private float collTimerWithVibot = 0.0f; //봇과의 충돌 타이머
        private float collTimerWithTower = 0.0f; //타워와의 충돌 타이머
        private float minCollTimer = 1.0f;
        private int hitCnt = 0;


        public Bacillus(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 position, Vector2 direcitonvector)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {


            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\virus_die2");
            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Bacillus");

            //리소스 가져오기
            bacil2 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Bacillus2");
            bacil3 = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Bacillus3");
            bacilDie = ContentManager.Load<Texture2D>("Sprites\\Viruses\\BacillusDie");
            collSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Virus\\Bacilcoil\\bacilColl");

            DirectionVector = direcitonvector;
            this.position = position;
            m_HP = 2;
            width = (float)m_Texture.Width;
            height = (float)m_Texture.Height;
            mass = 160f;
            radius = width / 2;
            SetUpPhysics(world, position, radius, mass);

        }
        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {
            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0f;
            body.Friction = 1f;
            this.CollisionCategories = Category.Cat14;
            //this.CollidesWith = ~Category.Cat31 & ~Category.Cat1 & ~Category.Cat2 & ~Category.Cat3;
            this.CollidesWith = Category.Cat2 | Category.Cat6 | Category.Cat14 | Category.Cat3;

            Debug.WriteLine(this.CollidesWith);
        }

    
        public override bool CollisionToVibot()
        {
            //////////////////////// VIBOT 충돌  후 사망 처리PART  ////////////////////////////////////

            if (hitCnt >= 3)
                m_HP -= 0.05f;


            if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2))) //
            {
                if (collTimerWithVibot >= minCollTimer)
                {
                   // Debug.WriteLine("충돌2");
                    collTimerWithVibot = 0.0f;
                    hitCnt++;

                    if (hitCnt == 1)
                    {
                        collSound.Play();
                        m_Texture = bacil2;
                    }
                    else if (hitCnt == 2)
                    {
                        collSound.Play(1.0f, 0.0f, 0.0f);
                        m_Texture = bacil3;
                    }
                    else if (hitCnt == 3)
                    {
                        m_Texture = bacilDie;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CollisionToTower(List<WhiteCell> Whitecell_List)
        {

            if (hitCnt >= 3)
                m_HP -= 0.05f;
        
            foreach (WhiteCell whitecell in Whitecell_List)
            {
                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), 20).Intersects
                    (new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), Stuff.TowerAbsorbRange)))
                {
                    SetHoming(whitecell.body.Position, 0.1f);
                    if (collTimerWithTower >= minCollTimer)
                    {
                        //Debug.WriteLine("충돌2");
                        collTimerWithTower = 0.0f;
                        hitCnt++;

                        if (hitCnt == 1)
                        {
                            collSound.Play(1.0f, 0.0f, 0.0f);
                            m_Texture = bacil2;
                        }
                        else if (hitCnt == 2)
                        {
                            collSound.Play(1.0f, 0.0f, 0.0f);
                            m_Texture = bacil3;
                        }
                        else if (hitCnt == 3)
                        {
                            m_Texture = bacilDie;
                        }
                    }

                    return true;
                }

            }

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
                    m_HP -= 0.3f;
                    Actor_RedBlood.m_HP[i] -= 1f;
              
                    Actor_RedBlood.Damaged = Actor_RedBlood.Blood_BodyList[i].Position;
                    break;
                }
            }

            return false;
        }
        public override void SetHoming(Vector2 target)
        {
            rotation = (float)Math.Atan2((target.Y - position.Y), (target.X - position.X));
            DirectionVector = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            DirectionVector *= 300;
        }

        private float DirectionTimer = 0.0f;

        public override void OnUpdate(GameTime gameTime)  //움직이기
        {

            //////////////////////// 움직이기 처리 //////////////////////
            Vector2 force = Vector2.Zero;
            ForceAmount = 1 + (float)Rand.NextDouble();

            collTimerWithVibot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            collTimerWithTower += (float)gameTime.ElapsedGameTime.TotalSeconds;
            DirectionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (DirectionVector != null)
                body.ApplyForce(DirectionVector * (float)gameTime.ElapsedGameTime.TotalSeconds, this.body.Position);

            StopBodyAccelate(gameTime, 0.4f, Maxium_Speed); // 최대 속도 제한 



          base.OnUpdate(gameTime);

        }




    }
}
