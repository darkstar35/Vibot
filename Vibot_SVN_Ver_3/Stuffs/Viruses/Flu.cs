using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vibot.Base;

using Microsoft.Xna.Framework.Audio;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace Vibot.Stuffs
{
    public class Flu : Stuff  //Flu 바이러스(2단계)
    {

        const float Maxium_Speed  = 1.0f;


        public Flu(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 position, Vector2 direcitonvector)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {
          
            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Flu");
            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\virus_die2");
         //   m_DamagedSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\heart");

            ForceAmount = 3.7f;


            DirectionVector = direcitonvector;
            this.position = position;
            m_HP = 1.2f;
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
            body.Friction = 1.5f;
            body.CollisionCategories = Category.Cat6;
            body.CollidesWith = ~Category.Cat31 & ~Category.Cat1 & ~Category.Cat2 & ~Category.Cat3 & ~Category.Cat7;


        }

     //  public override void SetUpPhysics(World world, Vector2 position, float width, float height, float mass)
     //  {
     //      mass = 3f;
     //
     //      body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(width / 2), mass, ConvertUnits.ToSimUnits(position));
     //      body.BodyType = BodyType.Dynamic;
     //      body.Restitution = 0.3f;
     //      body.Friction = 1.5f;
     //      this.CollisionCategories = Category.Cat6;
     //      body.CollidesWith = ~Category.Cat31 & ~Category.Cat1 & ~Category.Cat2 & ~Category.Cat3 & ~Category.Cat7;
     //
     //  }

        public override void OnUpdate(GameTime gameTime)
        {
         
            //////////////////////// 움직이기 처리 //////////////////////
            Vector2 force = Vector2.Zero;
            ForceAmount = 1 + (float)Rand.NextDouble();


            if (DirectionVector != null)
                body.ApplyForce(DirectionVector * (float)gameTime.ElapsedGameTime.TotalSeconds, this.body.Position);

            StopBodyAccelate(gameTime, 0.5f, Maxium_Speed); // 최대 속도 제한 



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
                    m_HP -= 0.05f;
                    if (!whitecell.ISselectedcell)   
                    whitecell.m_HP -= 0.02f;
                    return true;
                }
            }

            return false;

        }

   
   

      


    }
}
