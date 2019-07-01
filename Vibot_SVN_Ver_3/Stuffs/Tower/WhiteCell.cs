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


namespace Vibot.Stuffs
{
    public class WhiteCell : Stuff
    {

        public Texture2D Tower_shield_texture;
        protected Texture2D Display_Selected_Postion;

        Vector2 DistanceVector = Vector2.Zero;
        private const float Maximum_HP = 2f;
        private bool isBloodCotainW = false;

        public SoundEffect m_Create;

        public float HP
        {
            get
            {
                return m_HP;
            }
            set
            {
                m_HP = MathHelper.Clamp(value, 0, Maximum_HP);
            }
        }

        int Grade = 0;
        int Level = 0;
        protected float spin = 0f;
        public bool ISselectedcell;
 //       public bool CopyAvailable;

        //Sound
        private SoundEffect Created_Complete_Sound;



        public WhiteCell(int level, Vector2 position, GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch) :
            base(GraphicDevice, ContentManager, SpriteBatch)
        {
            radius = 0f;

            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;

            this.Level = level;

            if (level == 0)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell0");
            else if (level == 1)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell0");
            else if (level == 2)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell0");

            Tower_shield_texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell_shield");
            Display_Selected_Postion = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Hexagon");

           // m_DieSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\tele");
            Created_Complete_Sound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Tower\\tower_build_complete");  

            Rand = new Random();
            spin = Rand.Next(0, 360);
            m_HP = 1f; // defalt HP

            SetUpPhysics(world, position, m_Texture.Width / 2.14f, 100f);

            Created_Complete_Sound.Play();
        }

        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Static;
            body.CollisionCategories = Category.Cat1 | Category.Cat3 | ~Category.All;
            body.Restitution = 0f;
            //body.Friction = 59.5f;

        }




        public virtual void OnUpdate(GameTime gameTime, Actor_RedBlood RedBlood)
        {


            if (!isBloodCotainW || ISselectedcell)
                DistanceVector = bodyWorldPosition - RedBlood.Blood_Core_Position;

            if (new Rectangle((int)RedBlood.Blood_Core_Position.X - RedBlood.RedBloodRectangleSize / 2, (int)RedBlood.Blood_Core_Position.Y - RedBlood.RedBloodRectangleSize / 2, RedBlood.RedBloodRectangleSize, RedBlood.RedBloodRectangleSize).Contains(
                new Rectangle((int)bodyWorldPosition.X - m_Texture.Width / 2, (int)bodyWorldPosition.Y - m_Texture.Height / 2, m_Texture.Width, m_Texture.Height))
                )
            {
                isBloodCotainW = true;
                this.body.Position = Vector2.SmoothStep(body.Position, ConvertUnits.ToSimUnits(DistanceVector * 0.9f + RedBlood.Blood_Core_Position), 0.1f);
            }
            else
                isBloodCotainW = false;

            if (isBloodCotainW == true)  //아아 이런 코드좋지않아 
            {// 경환 : 현재 디펜스 영역 안에 있다면 

                if (!(this is WhiteCell_PointDamType) && !(this is WhiteCell_SplashDamType) && !(this is WhiteCell_DefenseType))
                    HP += (float)gameTime.ElapsedGameTime.TotalSeconds / 20; // 체력을 회복시켜 준다 
            
            }
        }




        public override void OnDraw(GameTime gameTime)
        {

            m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, 0f, m_TextureOrigin, HP, SpriteEffects.None, 0f);
            m_SpriteBatch.Draw(Tower_shield_texture, bodyViewPortPosition, null, Color.White, spin, new Vector2(Tower_shield_texture.Width / 2, Tower_shield_texture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (ISselectedcell)
                m_SpriteBatch.Draw(Display_Selected_Postion, bodyViewPortPosition - new Vector2(50, 50), null, Color.OrangeRed, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            //  if (CopyAvailable == true)
            //      m_SpriteBatch.DrawString(GameMsgFont, "Reday To CellDivision", bodyViewPortPosition + new Vector2(20,10), Color.GreenYellow, 0, Vector2.Zero,
            //          0.5f, SpriteEffects.None, 0f);

            spin += 0.1f;


        }

        public virtual void Powerup()
        {
            m_HP += 1f;
        }

    }
}
