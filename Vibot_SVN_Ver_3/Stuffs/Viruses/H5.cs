using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using Vibot.Base;
using Vibot.Actors;

namespace Vibot.Stuffs
{
    public class H5 : Stuff
    {
        const float Maxium_Speed = 1.0f;
        
  

        public H5(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 position, Vector2 direcitonvector)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {


            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\virus_die2");
          //  DamagedBlood_texture = ContentManager.Load<Texture2D>("Sprites\\RedBlood\\RedBlood_damaged");
            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Viruses\\H5");


            DirectionVector = direcitonvector;
            this.position = position;
            m_HP = 1;
            width = (float)m_Texture.Width;
            height = (float)m_Texture.Height;
            mass = 2f;
            radius = width / 2;
            SetUpPhysics(world, position, radius, mass);

        }
        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        { 

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.5f;
            body.Friction = 0.5f;
            body.CollisionCategories = Category.Cat6;
            body.CollidesWith = ~Category.Cat31 & ~Category.Cat1 & ~Category.Cat2 & ~Category.Cat3 & ~Category.Cat7;


        }

      
        public override bool CollisionToVibot()
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

        public override bool CollisionToTower(List<WhiteCell> Whitecell_List)
        {

            foreach (WhiteCell whitecell in Whitecell_List)
            {


                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), 20).Intersects(
              new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), Stuff.TowerAbsorbRange))
                    )
                    SetHoming(whitecell.body.Position, 0.1f);



                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2).Intersects(
                         new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), whitecell.m_Texture.Width))
                  )
                {
                    m_HP -= 0.09f;
                    if (!whitecell.ISselectedcell) 
                    whitecell.m_HP -= 0.01f;
                    return true;
                }
            }

            return false;
        }

        public override void OnUpdate(GameTime gameTime)  //움직이기
        {

            //////////////////////// 움직이기 처리 //////////////////////
            Vector2 force = Vector2.Zero;
            ForceAmount = 1 + (float)Rand.NextDouble();
                 

        // if(DirectionVector != null)
          body.ApplyForce(DirectionVector* (float)gameTime.ElapsedGameTime.TotalSeconds, this.body.Position);
               
            StopBodyAccelate(gameTime, 0.4f, Maxium_Speed); // 최대 속도 제한 

    
   

        
       //  base.OnUpdate(gameTime);
         
        }
     
      


    }
}
