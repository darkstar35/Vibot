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
    class WhiteCell_PointDamType : WhiteCell
    {
        public bool OpenFire;
        public double Mouse_angle;

        Dictionary<float, Stuff> DistancetoEnemyDictionary = new Dictionary<float, Stuff>();

        public float DurationTick = 0;
        private float _shotinterval = 300;
        public float DamValue = 0.1f;

        public float ShotInterval
        {
            set { _shotinterval = MathHelper.Clamp(value, 180, 300); }
            get { return _shotinterval; }
        }

        const float Max_Range = 250; //300 

        public SoundEffect m_shotSound;


        public WhiteCell_PointDamType(int level, Vector2 pos, GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
            : base(level, pos,
                GraphicDevice, ContentManager, SpriteBatch)
        {
            HP = 1;

            if (level == 0)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell2");
            else if (level == 1)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell2");
            else if (level == 2)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell2");

            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");
            m_shotSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Tower\\bulletshot");  

        }



        public override void OnUpdate(GameTime gameTime, Actor_RedBlood RedBlood)
        {
            if (OpenFire)
            {
                DurationTick += gameTime.ElapsedGameTime.Milliseconds;
                if (DurationTick > ShotInterval)
                {

                 //   CElement PointBullet = new CElement((int)bodyWorldPosition.X, (int)bodyWorldPosition.Y, (int)BulletRange, 0) { m_Type = 3, m_angle = Mouse_angle };

                    Actor_GameFieldManager.FieldofBullets.Add(new CElement(bodyWorldPosition.X, bodyWorldPosition.Y, 5) { m_Type = 2, m_angle = Mouse_angle, DamValue = this.DamValue });
                    m_shotSound.Play(0.5f, 0.0f, 0f);

                    DurationTick = 0;
                }
            }

            base.OnUpdate(gameTime, RedBlood);
        }



        public virtual void ShottoVirus(List<Stuff> Virus_List)
        {
            for (int i = Virus_List.Count - 1; i >= 0; i--)
            {
                // 자기 영역안에서 발견한다면 
                if (Vector2.Distance(bodyWorldPosition, Virus_List[i].bodyWorldPosition) < Max_Range)
                {
                    OpenFire = true;   // OpenFireMode
                    if (DistancetoEnemyDictionary.ContainsKey(Vector2.Distance(bodyWorldPosition, Virus_List[i].bodyWorldPosition)))
                        continue;

                    DistancetoEnemyDictionary.Add(Vector2.Distance(bodyWorldPosition, Virus_List[i].bodyWorldPosition), Virus_List[i]);
                }

            }
            if (Virus_List.Count == 0 || DistancetoEnemyDictionary.Count == 0)
                OpenFire = false;


            if (DistancetoEnemyDictionary.Count > 0 && OpenFire)
            {

                Stuff TempVirus = DistancetoEnemyDictionary[DistancetoEnemyDictionary.Keys.Min()];
                // 그놈 쪽 방향으로 총부리를 겨눈다 -> 각도값을 얻어온다              

                Mouse_angle = Math.Atan2((double)(TempVirus.bodyWorldPosition.Y - bodyWorldPosition.Y),
                                                                       (double)(TempVirus.bodyWorldPosition.X - bodyWorldPosition.X));
                if (DistancetoEnemyDictionary.Count > 50) // 근방 등록한 놈이 50마리가 넘는다면 
                    DistancetoEnemyDictionary.Clear();     // 한번 클리어 해주고 초기화 해준다 
                if (Vector2.Distance(bodyWorldPosition, TempVirus.bodyWorldPosition) > Max_Range)  // 사거리안에 적이 있다면 오픈 화이어 
                    OpenFire = false;

            }


        }

        public override void OnDraw(GameTime gameTime)
        {

            m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, 0f, m_TextureOrigin, HP, SpriteEffects.None, 0f);
            m_SpriteBatch.Draw(Tower_shield_texture, bodyViewPortPosition, null, Color.White, spin, new Vector2(Tower_shield_texture.Width / 2, Tower_shield_texture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (ISselectedcell)
                m_SpriteBatch.Draw(Display_Selected_Postion, bodyViewPortPosition - new Vector2(50, 50), null, Color.OrangeRed, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            spin += 0.1f;
        }


        public override void Powerup()
        {
            ShotInterval -= 10;   // 아이템을 추가 주입하면  연사가 빨라진다
            DamValue += 0.03f;
            HP += 0.5f;
        }

    }
}
