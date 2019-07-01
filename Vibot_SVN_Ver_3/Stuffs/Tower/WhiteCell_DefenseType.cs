using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Vibot.Actors;
using Vibot.Stuffs;
using Vibot.Base;
namespace Vibot.Stuffs
{
    class WhiteCell_DefenseType : WhiteCell
    {

        private const float Maximum_HP = 10f;
     //   public string PointDamPower = "P";
        public float Treat_Range = 500;
        public bool DoTreat;
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
        Dictionary<float, WhiteCell> DistancetoWhiteCellDictionary = new Dictionary<float, WhiteCell>();


        public WhiteCell_DefenseType(int level, Vector2 pos, GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
            : base(level, pos,
                GraphicDevice, ContentManager, SpriteBatch)
        {
            HP = 5;  //초기체력
            if (level == 0)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell1");
         //  else if (level == 1)
         //      m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell1");
         //  else if (level == 2)
         //      m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell1");


            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");
       
        }
        

        public override void OnUpdate(GameTime gameTime, Actor_RedBlood RedBlood)
        {

            base.OnUpdate(gameTime, RedBlood);
          
        }

        public override void OnDraw(GameTime gameTime)
        {

            m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, 0f, m_TextureOrigin, HP / 3, SpriteEffects.None, 0f);
            m_SpriteBatch.Draw(Tower_shield_texture, bodyViewPortPosition, null, Color.White, spin, new Vector2(Tower_shield_texture.Width / 2, Tower_shield_texture.Height / 2), 1f, SpriteEffects.None, 0f); ;
            
            
            if (ISselectedcell)
                m_SpriteBatch.Draw(Display_Selected_Postion, bodyViewPortPosition - new Vector2(50, 50), null, Color.OrangeRed, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            spin += 0.1f;


        }


        public virtual void ShottoVirus(List<WhiteCell> WhiteCell_List)
        {
            for (int i = WhiteCell_List.Count - 1; i >= 0; i--)
            {
                // 자기 영역안에서 발견한다면 
                if (Vector2.Distance(bodyWorldPosition, WhiteCell_List[i].bodyWorldPosition) < Treat_Range)
                {
                    DoTreat = true;   // DoTreatMode
                    if (DistancetoWhiteCellDictionary.ContainsKey(Vector2.Distance(bodyWorldPosition, WhiteCell_List[i].bodyWorldPosition)))
                        continue;

                    DistancetoWhiteCellDictionary.Add(Vector2.Distance(bodyWorldPosition, bodyWorldPosition), WhiteCell_List[i]);
                }

            }

            //    if (DistancetoWhiteCellDictionary.Count > 0 && DoTreat)
            //    {
            //
            //        Stuff TempVirus = DistancetoWhiteCellDictionary[DistancetoWhiteCellDictionary.Keys.Min()];
            //        // 그놈 쪽 방향으로 총부리를 겨눈다 -> 각도값을 얻어온다              
            //
            //        Mouse_angle = Math.Atan2((double)(TempVirus.bodyWorldPosition.Y - bodyWorldPosition.Y),
            //                                                               (double)(TempVirus.bodyWorldPosition.X - bodyWorldPosition.X));
            //        if (DistancetoWhiteCellDictionary.Count > 50)
            //            DistancetoWhiteCellDictionary.Clear();
            //        if (Vector2.Distance(bodyWorldPosition, TempVirus.bodyWorldPosition) > Treat_Range)
            //            DoTreat = false;
            //
            //    }

        }

        public override void Powerup()
        {
            m_HP += 1f;
        }

    }
}
