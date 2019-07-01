using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using Vibot.Base;
using Vibot.Actors;
using Vibot.Stuffs;
using System.Diagnostics;


namespace Vibot.Base
{


    public enum ActPattern
    {
        Wander, Intelligent, Derive
    }

    public enum MoveType
    {
        Linear, SCurve, Circle
    }


    public class Stuff : IObject
    {
        const float Maxium_Speed = 3.0f;


        protected float radius;

        public float Radius
        {
            get
            {
                return radius;
            }

        }



        protected float ForceAmount; // 속도

        public const float TowerAbsorbRange = 50f;

        public float rotation;
        public float Head_degree;
        public Vector2 DirectionVector;
        public Vector2 position;
        public Vector2 velocity;
  


        public Body Body
        {
            get { return body; }
            set { body = value; }
        }



        public Stuff(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
     
            //       GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");

        }


        /////////////////////////  RedBlood과 Virus충돌?      ///////////////////////////
        public virtual bool CollisionToBlood(Actor_RedBlood Actor_RedBlood, GameTime gametime)
        {

            for (int i = 0; i < Actor_RedBlood.Blood_BodyList.Count; i++)
            {
                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0f), (float)(m_Texture.Width / 2.14))
                                             .Intersects(new BoundingSphere(new Vector3(ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.X), ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.Y), 0f), (float)(38 / 2.14)))
                      )
                {
                    m_HP -= 1.0f;
                    Actor_RedBlood.m_HP[i] -= 0.5f;
                    // m_SpriteBatch.Draw(DamagedBlood_texture, ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position) - cCamera.CameraPosition, new Rectangle(47 * (int)gametime.TotalGameTime.TotalMilliseconds % 3, 0, 47, 47), Color.White);
                    Actor_RedBlood.Damaged = Actor_RedBlood.Blood_BodyList[i].Position;

                    return true;

                }
                else
                    Actor_RedBlood.Damaged = Vector2.Zero;
            }

            return false;

        }


        public virtual bool CollisionToTower(List<WhiteCell> Whitecell_list)
        {

            foreach(WhiteCell whitecell in Whitecell_list)
            {
            if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), 20).Intersects(
          new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), TowerAbsorbRange))
            )
                SetHoming(whitecell.body.Position, 0.2f);


            if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2).Intersects(
                     new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), whitecell.m_Texture.Width))
              )
            {
                m_HP -= 0.09f;
             
                if(!whitecell.ISselectedcell)
                whitecell.m_HP -= 0.01f;
                return true;

            }
            }
            return false;



        }

        public virtual bool CollisionToVibot()
        {
            //////////////////////// VIBOT 충돌  후 사망 처리PART  ////////////////////////////////////

            if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2))) //
            {
                m_HP -= 0.20f;
                return true;
            }
            else
                return false;
        }

        public virtual void SetHoming(Vector2 target)
        {

        }

        public virtual void SetHoming(Vector2? target, float followingspeed)
        {
            if (target.HasValue)
                body.Position = Vector2.SmoothStep(body.Position, target.Value, followingspeed);
        }




        //    public virtual bool CollisionToVibot(Actor_Vibot Vibot)
        //    {
        //        //////////////////////// VIBOT 충돌  후 사망 처리PART  ////////////////////////////////////
        //
        //        if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2))) //
        //        {
        //            m_HP -= 0.20f;
        //            return true;
        //        }
        //        else
        //            return false;
        //    }

        public virtual bool CollisionToEndofWorld()
        {
            // -------- 맵에 끝에 가면 자동 사망 처리 ----------------//

            if (bodyWorldPosition.X + 40 < 0 || bodyWorldPosition.X - 40 > cCamera.MapSize.X || bodyWorldPosition.Y + 40 < 0 || bodyWorldPosition.Y + 40 > cCamera.MapSize.Y)
                return true;
            else
                return false;
        }




        public virtual void OnUpdate(GameTime gameTime)
        {
        }

        public virtual void StopBodyAccelate(GameTime gameTime, float StopAmount, float MaxSpeed)
        {
            //////////////////////////////// 일정 속력 감속 한다 //////////////////////////
            if (body.LinearVelocity.X > MaxSpeed)
            {
                if (body.LinearVelocity.X > 0)
                    this.body.LinearVelocity -= new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);

                else if (body.LinearVelocity.X < 0)
                    this.body.LinearVelocity += new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);

            }
            if (body.LinearVelocity.Y > MaxSpeed)
            {
                if (body.LinearVelocity.Y > 0)
                    this.body.LinearVelocity -= new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

                else if (body.LinearVelocity.Y < 0)
                    this.body.LinearVelocity += new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

            }
        }


        public virtual void OnDraw(GameTime gameTime)
        {
            if (m_HP > 0 && m_Texture != null)
                m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, body.Rotation, m_TextureOrigin, m_HP, SpriteEffects.None, 0f);

        }



    }
}
