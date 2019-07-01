using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


namespace Vibot.Base
{


    public abstract class IObject : Object
    {

        protected Random Rand = new Random();

        //physics properties
        protected float width;
        protected float height;
        protected float mass;

        //Font
        protected SpriteFont SystemFont;
        protected SpriteFont GameMsgFont;

        public Texture2D m_Texture;
        public Vector2 m_TextureOrigin
        {
            get
            {
                return new Vector2(m_Texture.Width / 2, m_Texture.Height / 2);
            }
            set
            {
                m_TextureOrigin = value;
            }
   
        }
        public Body body;
        public float m_HP = 1f;
       
        


        //Sound
        public SoundEffect m_DieSound;
        public SoundEffect m_DamagedSound;

        //Manager
        protected GraphicsDevice m_GraphicDevice = null;
        protected ContentManager m_ContentManager = null;
        protected SpriteBatch m_SpriteBatch = null;

        public static World world = new World(new Vector2(0, 0));

        protected Category _collidesWith;
        protected Category _collisionCategories;


        public Category CollisionCategories
        {
            get { return _collisionCategories; }
            set
            {
                _collisionCategories = value;
                body.CollisionCategories = value;
            }
        }

        public Category CollidesWith
        {
            get { return _collidesWith; }
            set
            {
                _collidesWith = value;
                body.CollidesWith = value;
            }
        }

        public Vector2 bodyWorldPosition
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(body.Position);
            }

            set
            {
                body.Position = ConvertUnits.ToSimUnits(value);
            }
        }
        public Vector2 bodyViewPortPosition
        {
            get
            {
                return cCamera.Transform(bodyWorldPosition);
            }
        }

        public float Slope_Atan(IObject a, IObject b)
        {

            return (float)Math.Atan2(a.bodyWorldPosition.Y - b.bodyWorldPosition.Y,
            a.bodyWorldPosition.X - b.bodyWorldPosition.X);
        }
        public float Slope_Atan(IObject a, Vector2 b)
        {

            return (float)Math.Atan2(b.Y - a.bodyWorldPosition.Y,
             b.X - a.bodyWorldPosition.X);
        }
        
        public virtual void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {
            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;

        }

        public virtual void SetUpPhysics(World world, Vector2 position, float width, float height, float mass)
        {
            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 1f;
            body.Friction = 1f;

        }


    }
}
