using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

using Vibot.Base;
using Vibot.Actors;

namespace Vibot.Stuffs
{
    public class Preinvasive : Stuff
    {
        const float Maxium_Speed = 5f;


        public Preinvasive(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 position, Vector2 direcitonvector)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {
            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Viruses\\Preinvasive");
            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\virus_die3");
          //  ForceAmount = 3f;


            DirectionVector = direcitonvector;
            this.position = position;
            m_HP = 5f;
            width = (float)m_Texture.Width;
            height = (float)m_Texture.Height;
            mass = 10f;
            radius = width / 2.14f;
            SetUpPhysics(world, position, radius, mass);     
        }

        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
 
            body.CollisionCategories = Category.Cat8;
            body.CollidesWith = Category.Cat8 | ~Category.All;


        }
        public override bool CollisionToBlood(Actor_RedBlood Actor_RedBlood, GameTime gametime)
        {
            for (int i = 0; i < Actor_RedBlood.Blood_BodyList.Count; i++)
            {
                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0f), (float)(m_Texture.Width / 2.14))
                                             .Intersects(new BoundingSphere(new Vector3(ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.X), ConvertUnits.ToDisplayUnits(Actor_RedBlood.Blood_BodyList[i].Position.Y), 0f), (float)(38 / 2.14)))
                      )
                {
                    m_HP -= 0.5f;
                    Actor_RedBlood.m_HP[i] -= 0.5f;
                    Actor_RedBlood.Damaged = Actor_RedBlood.Blood_BodyList[i].Position;
                    break;
                }
            }

            return false;
        }

        public override bool CollisionToTower(List<WhiteCell> Whitecell_list)
        {
            foreach (WhiteCell whitecell in Whitecell_list)
            {
         
                if (new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2).Intersects(
                         new BoundingSphere(new Vector3(whitecell.bodyWorldPosition.X, whitecell.bodyWorldPosition.Y, 0), whitecell.m_Texture.Width)))
                {
                    m_HP -= 0.05f;
                    whitecell.m_HP -= 0.05f;
                    return true;

                }
            }
            return false;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            //////////////////////// 움직이기 처리 //////////////////////
         
            ForceAmount = 10 + (float)Rand.NextDouble();

            if (DirectionVector != null)
                body.ApplyForce( (ForceAmount *DirectionVector) * (float)gameTime.ElapsedGameTime.TotalSeconds, body.Position);

            StopBodyAccelate(gameTime, 1f, Maxium_Speed); // 최대 속도 제한 

        }

        public override void OnDraw(GameTime gameTime)
        {
            if (m_HP > 0 && m_Texture != null)
                m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, body.Rotation, m_TextureOrigin, m_HP/3, SpriteEffects.None, 0f);
        }


    }
}
