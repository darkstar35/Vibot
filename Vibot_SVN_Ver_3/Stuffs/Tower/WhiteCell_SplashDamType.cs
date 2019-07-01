
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
    class WhiteCell_SplashDamType : WhiteCell
    {
        public bool OpenFire;
        public double Mouse_angle;


        Dictionary<float, Stuff> DistancetoEnemyDictionary = new Dictionary<float, Stuff>();

        public float DurationTick = 0;
        public const float Interval = 800;
        const float Max_Range = 250;
        private float _bulletrange = 100;
        private const float Maximum_HP = 2f;

        public float BulletRange
        {
            get { return _bulletrange; }
            set { _bulletrange = MathHelper.Clamp(value, 100, 200); }

        }

        float DamValue = 0.03f;

        public SoundEffect m_shotSound;

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


        public WhiteCell_SplashDamType(int level, Vector2 pos, GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
            : base(level, pos,
                GraphicDevice, ContentManager, SpriteBatch)
        {

            HP = 1;  //초기체력
            if (level == 0)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell3");
            else if (level == 1)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell3");
            else if (level == 2)
                m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\whitecell3");

            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");

            m_shotSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Tower\\splashshot");  

        }



        public override void OnUpdate(GameTime gameTime, Actor_RedBlood RedBlood)
        {

            if (OpenFire)
            {
                DurationTick += gameTime.ElapsedGameTime.Milliseconds;
                if (DurationTick > Interval)
                {


                      Actor_GameFieldManager.FieldofBullets.Add(new CElement((int)bodyWorldPosition.X, (int)bodyWorldPosition.Y,25, (int)BulletRange) { m_Type = 3, m_angle = Mouse_angle, DamValue = this.DamValue});          
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


         //  if (CopyAvailable == true)
         //      m_SpriteBatch.DrawString(GameMsgFont, "Reday To CellDivision", bodyViewPortPosition + new Vector2(100, 50), Color.GreenYellow);

            spin += 0.1f;
        }


        public override void Powerup()
        {
            m_HP += 0.5f;
            BulletRange += 3;
            DamValue += 0.01f;
        }

    }
}
