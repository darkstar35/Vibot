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


namespace Vibot.Actors
{
    public class Chole_Object : Stuff
    {

        public Chole_Object(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 Position) :
            base(GraphicDevice, ContentManager, SpriteBatch)
        {

            m_Texture = ContentManager.Load<Texture2D>("Sprites\\Objects\\colestroll");
            radius = (float)m_Texture.Width/2.2f;
            mass = 30;

            SetUpPhysics(world, Position, radius, mass);     
        }
        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {
            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            body.Friction = 1;
       
        }


        public override void OnUpdate(GameTime gameTime)  //움직이기
        {


   


        }





    }
}
