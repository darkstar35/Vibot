using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using System.Xml;
using Vibot.Base;
using Vibot.Stuffs;
using System.Diagnostics;


namespace Vibot.Actors
{


    public class CSecurePoint : CElement
    {
        public const float MaxSecuringTime = 14f;
        public float SecureSecond = 0;

        public CSecurePoint(int x, int y, int width, int height)
            : base(x, y, width, height)
        {


        }

        public SecureState SecureState
        {
            get
            {
                //원활한 시큐어 게이지 그리기를 위해 10초로 바꿈
                //백분율 사용을 위함
                if (SecureSecond > MaxSecuringTime)
                    return SecureState.SECURE;
                else if (SecureSecond > 0f)
                    return SecureState.SECURING;

                return SecureState.UNSECURE;
            }
        }



    }



    class Actor_GameFieldManager : IActor
    {
        Actor_Vibot Vibot;
        public Actor_Objects Objects;
        Actor_RedBlood RedBlood;
        Actor_SpawnPointManager SpawnPoints;
        Actor_WhiteCellManager WhiteCells;

        public bool ItemDrop = false;
        List<BioGauge> DropGaugeList = new List<BioGauge>();
        public List<BioTrap> BioTrapList = new List<BioTrap>();

        private Texture2D SecureBefore;
        private Texture2D secureAfter;
        private Texture2D secureGauge; //시큐어 게이지

        private Texture2D SplashBulletTex;
        private Texture2D PointBulletTex;
        private Texture2D DefenseTrapTex;

        public List<CSecurePoint> SecurePoint_List = new List<CSecurePoint>();
        public static List<CElement> FieldofBullets = new List<CElement>();

        Queue<CMsg<eSystemMsg>> System_MsgQueue = new Queue<CMsg<eSystemMsg>>();


        public Actor_GameFieldManager(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Actor_Vibot Actor_Vibot, Actor_RedBlood Actor_RedBlood, Actor_SpawnPointManager Actor_SpawnPoint, Actor_Objects Actor_Objects, Actor_WhiteCellManager WhiteCells)
        {
            SystemFont = ContentManager.Load<SpriteFont>("Fonts\\prologue");
            SecureBefore = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\SecurePointBefore");
            secureAfter = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\SecurePointAfter");
            secureGauge = ContentManager.Load<Texture2D>("Sprites\\MainMenu\\secureGauge");
            SplashBulletTex = ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\Splash");
            PointBulletTex = ContentManager.Load<Texture2D>("Sprites\\WhiteCell\\Bullet2");
            DefenseTrapTex = ContentManager.Load<Texture2D>("Sprites\\Trap\\CellWall");

            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;

            this.Vibot = Actor_Vibot;
            this.Objects = Actor_Objects;
            this.RedBlood = Actor_RedBlood;
            this.SpawnPoints = Actor_SpawnPoint;
            this.Objects = Actor_Objects;
            this.WhiteCells = WhiteCells;
            m_DieSound = ContentManager.Load<SoundEffect>("Audio\\MAINMENU\\start");

        }

        //  public Actor_GameFieldManager( 차후, 중보스 기생충을 추가하는 생성자 매개변수를 넣어야 할듯      
        public void OnDraw(GameTime gameTime)
        {
            //Draw SecurePoints
            foreach (CSecurePoint SecurePoint in SecurePoint_List)
            {
                if (SecurePoint.SecureState == SecureState.SECURE)
                {
                    m_SpriteBatch.Draw(secureAfter, SecurePoint.ViewPortRectangle, new Rectangle(0, 0, 300, 300), Color.White);
                    //    m_SpriteBatch.DrawString(SystemFont, "" + (SecurePoint.ViewPortRectangle.X + (SecurePoint.ViewPortRectangle.Width / 2)).ToString()+","+(SecurePoint.ViewPortRectangle.Y + (SecurePoint.ViewPortRectangle.Height / 2)).ToString()+"", new Vector2(SecurePoint.ViewPortRectangle.X + (SecurePoint.ViewPortRectangle.Width / 2), SecurePoint.ViewPortRectangle.Y + (SecurePoint.ViewPortRectangle.Height / 2)), Color.White);
                }
                else if (SecurePoint.SecureState == SecureState.UNSECURE)
                {
                    m_SpriteBatch.Draw(SecureBefore, SecurePoint.ViewPortRectangle, new Rectangle(0, 0, 300, 300), Color.White);
                    // m_SpriteBatch.DrawString(SystemFont, "" + (SecurePoint.ViewPortRectangle.X + (SecurePoint.ViewPortRectangle.Width / 2)).ToString() + "," + (SecurePoint.ViewPortRectangle.Y + (SecurePoint.ViewPortRectangle.Height / 2)).ToString() + "", new Vector2(SecurePoint.ViewPortRectangle.X + (SecurePoint.ViewPortRectangle.Width / 2), SecurePoint.ViewPortRectangle.Y + (SecurePoint.ViewPortRectangle.Height / 2)), Color.White);
                }
                else if (SecurePoint.SecureState == SecureState.SECURING)
                {
                    //대기 
                    //시큐어 게이지 그리기 플러드 컨트롤 참조

                    float GaugeHeight = 300 * (SecurePoint.SecureSecond / CSecurePoint.MaxSecuringTime);
                    Debug.WriteLine(GaugeHeight);

                    m_SpriteBatch.Draw(
                        secureGauge,
                        new Rectangle(
                            (int)SecurePoint.ViewPortRectangle.X,
                            (int)SecurePoint.ViewPortRectangle.Y + (300 - (int)GaugeHeight),
                            300,
                            (int)GaugeHeight),
                        new Rectangle(
                            0,
                            0 + (300 - (int)GaugeHeight),
                            300,
                            (int)GaugeHeight),
                        Color.White);

                    m_SpriteBatch.Draw(SecureBefore, SecurePoint.ViewPortRectangle, new Rectangle(0, 0, 300, 300), Color.White);
                }


            }


            // Trap 관련 코드 
            for (int i = BioTrapList.Count - 1; i >= 0; i--)
            {
                if (BioTrapList[i].BioTrapMODE == BioTrapMODE.RUNNING)
                {

                    switch (BioTrapList[i].m_Type)
                    {
                        case 3:  // Splash Dam Trap
                            m_SpriteBatch.Draw(SplashBulletTex, BioTrapList[i].ViewPortRectangle, null,
                                Color.White, (float)BioTrapList[i].m_angle, new Vector2(SplashBulletTex.Width / 2, SplashBulletTex.Height / 2),
                                          SpriteEffects.None, 0f);


                            break;

                        case 2:// Point DamTrap
                            m_SpriteBatch.Draw(PointBulletTex, BioTrapList[i].ViewPortRectangle, null,
                                Color.Green, (float)BioTrapList[i].m_angle, new Vector2(PointBulletTex.Width / 2, PointBulletTex.Height / 2),
                                SpriteEffects.None, 0f);

                            break;

                        case 1: // Defense Type
                            m_SpriteBatch.Draw(DefenseTrapTex, BioTrapList[i].bodyViewPortPosition, null, Color.White, (float)BioTrapList[i].m_angle
                                , new Vector2(DefenseTrapTex.Width / 2, DefenseTrapTex.Height / 2), 1f, SpriteEffects.None, 0f);

                            break;

                    }
                }

            }




            // 필드에 나온 총알들 
            for (int i = FieldofBullets.Count - 1; i >= 0; i--)
            {

                switch (FieldofBullets[i].m_Type)// FieldofBullets[i] code
                {

                    case 3:  // Splash Dam Type   
                        m_SpriteBatch.Draw(SplashBulletTex, FieldofBullets[i].ViewPortRectangle,
                        null, Color.White, (float)FieldofBullets[i].m_angle, new Vector2(SplashBulletTex.Width / 2, SplashBulletTex.Height / 2),
                        SpriteEffects.None, 0f);
                        break;

                    case 2:// Point Dam Type
                        m_SpriteBatch.Draw(PointBulletTex, cCamera.Transform(new Vector2(FieldofBullets[i].m_Sphere.Center.X, FieldofBullets[i].m_Sphere.Center.Y)),
                        Color.Orange);
                        break;


                    case 1:
                        //   m_SpriteBatch.Draw(PointBulletTex, new Vector2(FieldofBullets[i].m_Sphere.Center.X, FieldofBullets[i].m_Sphere.Center.Y) - cCamera.CameraPosition,
                        //       new Rectangle(64 * 5, 0, 5, 5), Color.Orange);
                        break;

                }

            }



            foreach (Stuff Virus in SpawnPoints.AllofVirusList)
                Virus.OnDraw(gameTime);  //     if (RedBlood != null && RedBlood.Blood_BodyList.Count > 0) // Test 코드  


            foreach (BioGauge DropedItem in DropGaugeList)    // if (DropedItem != null && DropedItem.Body != null)
                DropedItem.OnDraw(gameTime);

        }



        private Texture2D UI_Building_Texture;

        public void OnUpdate(GameTime gameTime)
        {

            //  if (FieldofBullets.Count > 0)         // 필드에 나온 총알들 
            for (int i = FieldofBullets.Count - 1; i >= 0; i--)
            {
                FieldofBullets[i].m_LifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (FieldofBullets[i].m_LifeTime > 3 || FieldofBullets[i].m_HP <= 0)
                {
                    FieldofBullets.RemoveAt(i);
                    break;
                }

                switch (FieldofBullets[i].m_Type)// FieldofBullets[i] code
                {

                    case 3:  // Splash Dam Type
                        FieldofBullets[i].m_Rect.X += (int)(Math.Cos(FieldofBullets[i].m_angle) * 10);
                        FieldofBullets[i].m_Rect.Y += (int)(Math.Sin(FieldofBullets[i].m_angle) * 10);

                        foreach (Stuff Virus in SpawnPoints.AllofVirusList)
                        {
                            //대기
                            if (Virus is Malignant)
                            {
                                Malignant tempMal = Virus as Malignant;
                                if (tempMal.m_HP <= 0)
                                {
                                    world.RemoveBody(tempMal.ShieldBody);
                                    world.RemoveBody(tempMal.Body);
                                    SpawnPoints.AllofVirusList.Remove(tempMal);
                                    break;

                                }
                                if (FieldofBullets[i].ViewPortRectangle.Intersects(new Rectangle((int)tempMal.Defense_Sphere.Center.X, (int)tempMal.Defense_Sphere.Center.Y, 40, 10)))    //이
                                {
                                    //  Debug.WriteLine("스플래시 공격 방패!!");
                                    FieldofBullets.RemoveAt(i);
                                    break;
                                }

                            }
                            else if (FieldofBullets[i].m_Rect.Intersects(new Rectangle((int)Virus.bodyWorldPosition.X, (int)Virus.bodyWorldPosition.Y, Virus.m_Texture.Width, Virus.m_Texture.Height)))    //이
                            {
                                Virus.m_HP -= FieldofBullets[i].DamValue; // 스플래시 데미지 
                                //FieldofBullets[i].m_HP--;
                            }

                        }

                        break;

                    case 2:// Point Dam Type
                        FieldofBullets[i].m_Sphere.Center.X += (float)(Math.Cos(FieldofBullets[i].m_angle) * 5);
                        FieldofBullets[i].m_Sphere.Center.Y += (float)(Math.Sin(FieldofBullets[i].m_angle) * 5);


                        foreach (Stuff Virus in SpawnPoints.AllofVirusList)
                        {
                            if (Virus is Malignant)
                            {
                                Malignant tempMal = Virus as Malignant;
                                if (tempMal.m_HP <= 0)
                                {
                                    world.RemoveBody(tempMal.ShieldBody);
                                    world.RemoveBody(tempMal.Body);
                                    SpawnPoints.AllofVirusList.Remove(tempMal);
                                }
                                if (FieldofBullets[i].ViewPortRectangle.Intersects(new Rectangle((int)tempMal.Defense_Sphere.Center.X, (int)tempMal.Defense_Sphere.Center.Y, 40, 10)))    //이
                                {
                                    Debug.WriteLine("포인트 공격 방패!!");
                                    FieldofBullets.RemoveAt(i);
                                    break;
                                }

                            }
                            else if (FieldofBullets[i].m_Sphere.Intersects(new BoundingSphere(new Vector3(Virus.bodyWorldPosition.X, Virus.bodyWorldPosition.Y, 0), ((Virus.Radius != 0) ? Virus.Radius : 10))))
                            {
                                Virus.m_HP -= FieldofBullets[i].DamValue; // 포인트 데미지 
                                FieldofBullets[i].m_HP -= 0.01f;
                            }
                        }
                        break;



                    case 1:
                        FieldofBullets[i].m_Sphere.Center.X += (float)Math.Cos(FieldofBullets[i].m_angle) * 10;
                        FieldofBullets[i].m_Sphere.Center.Y += (float)Math.Sin(FieldofBullets[i].m_angle) * 10;

                        break;

                }




            }

            for (int i = BioTrapList.Count - 1; i >= 0; i--)
            {
                BioTrapList[i].OnUpdate(gameTime);

                if (BioTrapList[i].m_LifeTime > 2)// || BioTrapList[i].m_HP <= 0)
                {
                    if (BioTrapList[i].body != null)
                        world.RemoveBody(BioTrapList[i].body);

                    BioTrapList.RemoveAt(i);
                    break;
                }


                foreach (Stuff Virus in SpawnPoints.AllofVirusList)
                {

                    //대기
                    if (Virus is Malignant)
                    {
                        Malignant tempMal = Virus as Malignant;
                        if (tempMal.m_HP <= 0)
                        {
                            world.RemoveBody(tempMal.ShieldBody); world.RemoveBody(tempMal.Body);
                            SpawnPoints.AllofVirusList.Remove(tempMal);
                        }
                        if (BioTrapList[i].ViewPortRectangle.Intersects(new Rectangle((int)tempMal.Defense_Sphere.Center.X, (int)tempMal.Defense_Sphere.Center.Y, 40, 10)))    //이
                        {
                            //  Debug.WriteLine("공격 방패!!");
                            BioTrapList.RemoveAt(i);
                            break;
                        }

                    }
                    else if (BioTrapList[i].m_Rect.Intersects(new Rectangle((int)Virus.bodyWorldPosition.X, (int)Virus.bodyWorldPosition.Y, Virus.m_Texture.Width, Virus.m_Texture.Height)))    //이
                    {
                        if (BioTrapList[i].m_Type == 3)
                            Virus.m_HP -= 0.5f; // 스플래시 데미지 
                        else if (BioTrapList[i].m_Type == 2)
                            Virus.m_HP -= 1f; // 포인트 데미지 

                    }


                }




            }
            //      }


            ///
            /// WhiteCell Update 부문 

            if (Vibot.ReloadBioGauge != null) // 현재 Vibot이 장전된 bioGauge을 가지고 있고,
            {

                //      bool checkSelectWhiteCell = false;

                if (Vibot.ReloadBioGauge.Grade == 0) //타워 업그레이드 !checkSelectWhiteCell &&
                {
                    if (Vibot.IsCheckSelectWhiteCell(WhiteCells.WhiteCell_List))
                        for (int i = WhiteCells.WhiteCell_List.Count - 1; i >= 0; --i)
                            WhiteCells.WhiteCell_List[i].ISselectedcell = false;

                    Vibot.ReloadBioGauge = null;
                    WhiteCell whitecell = new WhiteCell(0, Vibot.bodyWorldPosition + new Vector2((float)Math.Cos(Vibot.Mouse_angle),
                        (float)Math.Sin(Vibot.Mouse_angle)) * Vibot.LaserLineLength, m_GraphicDevice, m_ContentManager, m_SpriteBatch);
                    WhiteCells.WhiteCell_List.Add(whitecell);

                }

            }
            for (int i = WhiteCells.WhiteCell_List.Count - 1; i >= 0; i--)
            {


                if (WhiteCells.WhiteCell_List[i] is WhiteCell_PointDamType)
                {
                    WhiteCell_PointDamType WhiteCell_PointDamType;
                    WhiteCell_PointDamType = WhiteCells.WhiteCell_List[i] as WhiteCell_PointDamType;
                    WhiteCell_PointDamType.ShottoVirus(SpawnPoints.AllofVirusList); // 나중에 SpawnPoint2와 Boss급 바이러스는 어떻게 처리?
                }
                else if (WhiteCells.WhiteCell_List[i] is WhiteCell_SplashDamType)
                {
                    WhiteCell_SplashDamType WhiteCell_SplashDamType;
                    WhiteCell_SplashDamType = WhiteCells.WhiteCell_List[i] as WhiteCell_SplashDamType;
                    WhiteCell_SplashDamType.ShottoVirus(SpawnPoints.AllofVirusList); // ShottheVirus
                }




                if (Vibot.ReloadBioGauge != null) // 현재 Vibot이 장전된 bioGauge을 가지고 있고,
                {

                    if (new BoundingSphere(new Vector3(WhiteCells.WhiteCell_List[i].bodyWorldPosition.X, WhiteCells.WhiteCell_List[i].bodyWorldPosition.Y, 0), 20).Intersects
                (new BoundingSphere(new Vector3(Vibot.ReloadBioGauge.Bullet_Position.X, Vibot.ReloadBioGauge.Bullet_Position.Y, 0), Vibot.ReloadBioGauge.Radius + 3)))   // bioGauge을  쏴서 맞추면,
                    {

                        switch (Vibot.ReloadBioGauge.Grade) //타워 업그레이드 
                        {
                            case 3: //스플래시 타입 
                                if (WhiteCells.WhiteCell_List[i] is WhiteCell_SplashDamType)
                                    WhiteCells.WhiteCell_List[i].Powerup();
                                else
                                {
                                    WhiteCells.WhiteCell_List[i] = new WhiteCell_SplashDamType(0, WhiteCells.WhiteCell_List[i].bodyWorldPosition, m_GraphicDevice, m_ContentManager, m_SpriteBatch);

                                }

                                break;

                            case 2: //포인트 데미지 타입
                                if (WhiteCells.WhiteCell_List[i] is WhiteCell_PointDamType)
                                    WhiteCells.WhiteCell_List[i].Powerup();
                                else
                                    WhiteCells.WhiteCell_List[i] = new WhiteCell_PointDamType(0, WhiteCells.WhiteCell_List[i].bodyWorldPosition, m_GraphicDevice, m_ContentManager, m_SpriteBatch);


                                break;


                            case 1: //방패 타입 
                                if (WhiteCells.WhiteCell_List[i] is WhiteCell_DefenseType)
                                    WhiteCells.WhiteCell_List[i].Powerup();
                                else
                                {
                                    WhiteCells.WhiteCell_List[i] = new WhiteCell_DefenseType(0, WhiteCells.WhiteCell_List[i].bodyWorldPosition, m_GraphicDevice, m_ContentManager, m_SpriteBatch);
                                    //  WhiteCells.WhiteCell_List.Add(WhiteCell_DefenseType);
                                    //  WhiteCells.WhiteCell_List.Remove();
                                }
                                break;

                        }

                        Vibot.ReloadBioGauge = null;
                    }


                }
            }

            //Virus Damage 처리 AI처리 

            for (int i = SpawnPoints.AllofVirusList.Count - 1; i >= 0; --i)
            {

                bool CheckKilledByVibot = false;


                SpawnPoints.AllofVirusList[i].OnUpdate(gameTime);

                if (SpawnPoints.AllofVirusList[i].CollisionToVibot() || SpawnPoints.AllofVirusList[i].CollisionToTower(WhiteCells.WhiteCell_List))        // SpawnPoints.AllofVirusList[i]가 vibot나 tower에게 죽었다면 
                    CheckKilledByVibot = true;


                SpawnPoints.AllofVirusList[i].CollisionToBlood(RedBlood, gameTime);


                if (SpawnPoints.AllofVirusList[i] is Bacillus)
                    SpawnPoints.AllofVirusList[i].SetHoming(RedBlood.Blood_Core_Position);
                else if (SpawnPoints.AllofVirusList[i] is Malignant)
                {
                    if (SpawnPoints.AllofVirusList[i].m_HP < 0.3f)
                        SpawnPoints.AllofVirusList[i].m_HP = 0;
                }


                if (SpawnPoints.AllofVirusList[i].m_HP <= 0) // SpawnPoints.AllofVirusList[i] 사망 처리 
                {
                    if (CheckKilledByVibot)
                    {
                        if (SpawnPoints.AllofVirusList[i] is H5)
                            Vibot.WhiteGauge_Value += 0.5f;
                        else if (SpawnPoints.AllofVirusList[i] is Flu)

                            Vibot.WhiteGauge_Value += 1f;
                        else if (SpawnPoints.AllofVirusList[i] is Malignant)
                        {
                            Malignant Malignant = SpawnPoints.AllofVirusList[i] as Malignant;
                            Malignant.collSound.Play(0.5f, 0.0f, 0.0f);
                            world.RemoveBody(Malignant.ShieldBody);

                            Vibot.WhiteGauge_Value += 1;

                        }
                        else if (SpawnPoints.AllofVirusList[i] is Bacillus)
                            Vibot.WhiteGauge_Value += 2;
                        else if (SpawnPoints.AllofVirusList[i] is Preinvasive)
                            Vibot.WhiteGauge_Value += 2;


                        if (ItemDrop)
                            System_MsgQueue.Enqueue(new CMsg<eSystemMsg>(eSystemMsg.SYSMSG_VIRUS_ITEMDROP, SpawnPoints.AllofVirusList[i]));
                        // CMsg<eSystemMsg> Msg = new CMsg<eSystemMsg>(eSystemMsg.SYSMSG_VIRUS_ITEMDROP, SpawnPoints.AllofVirusList[i]);



                    }

                    SpawnPoints.AllofVirusList[i].m_DieSound.Play();
                    world.RemoveBody(SpawnPoints.AllofVirusList[i].body);
                    SpawnPoints.AllofVirusList.Remove(SpawnPoints.AllofVirusList[i]);


                    break;

                }
                else if (SpawnPoints.AllofVirusList[i].CollisionToEndofWorld())
                {
                    world.RemoveBody(SpawnPoints.AllofVirusList[i].body);
                    SpawnPoints.AllofVirusList.Remove(SpawnPoints.AllofVirusList[i]);
                    break;
                }

            }


            //      //Virus Damage 처리 AI처리 
            //      foreach (Stuff Virus in SpawnPoints.AllofVirusList)
            //      {
            //
            //          bool CheckKilledByVibot = false;
            //
            //     //    if (!Virus.body.IsDisposed) //죽지 않았으면 
            //     //    {
            //              Virus.OnUpdate(gameTime);
            //
            //              if (Virus.CollisionToVibot() || Virus.CollisionToTower(WhiteCells.WhiteCell_List))        // Virus가 vibot나 tower에게 죽었다면 
            //                  CheckKilledByVibot = true;
            //
            //
            //              Virus.CollisionToBlood(RedBlood, gameTime);
            //
            //
            //              if (Virus is Bacillus)
            //                  Virus.SetHoming(RedBlood.Blood_Core_Position);
            //              else if (Virus is Malignant)
            //              {
            //                  if (Virus.m_HP < 0.3f)
            //                      Virus.m_HP = 0;
            //              }
            //       //   }
            //
            //          if (Virus.m_HP <= 0) // Virus 사망 처리 
            //          {
            //              if (CheckKilledByVibot)
            //              {
            //                  if (Virus is H5)
            //                      Vibot.WhiteGauge_Value += 0.5f;
            //                  else if (Virus is Flu)
            //
            //                      Vibot.WhiteGauge_Value += 1f;
            //                  else if (Virus is Malignant)
            //                  {
            //                      Malignant Malignant = Virus as Malignant;
            //                      Malignant.collSound.Play(0.5f, 0.0f, 0.0f);
            //                      world.RemoveBody(Malignant.ShieldBody);
            //
            //                      Vibot.WhiteGauge_Value += 1;
            //
            //                  }
            //                  else if (Virus is Bacillus)
            //                      Vibot.WhiteGauge_Value += 2;
            //
            //                  if (ItemDrop)
            //                      System_MsgQueue.Enqueue(new CMsg<eSystemMsg>(eSystemMsg.SYSMSG_VIRUS_ITEMDROP, Virus));
            //                  // CMsg<eSystemMsg> Msg = new CMsg<eSystemMsg>(eSystemMsg.SYSMSG_VIRUS_ITEMDROP, Virus);
            //
            //
            //
            //              }
            //
            //              Virus.m_DieSound.Play();
            //              world.RemoveBody(Virus.body);
            //              SpawnPoints.AllofVirusList.Remove(Virus);
            //
            //
            //              break;
            //
            //          }
            //          else if (Virus.CollisionToEndofWorld())
            //          {
            //              world.RemoveBody(body);
            //              SpawnPoints.AllofVirusList.Remove(Virus);
            //              break;
            //          }
            //
            //      }


            foreach (BioGauge BioGauge in DropGaugeList)
            {
                BioGauge.CollisionToEndofWorld();

                //대기 필드의 바이오 게이지 8초 이상 되면 죽는 코드

                //바이오 게이지 타이머 갱신
                BioGauge.bioGaugeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //8초 이상 되면
                if (BioGauge.bioGaugeTime > BioGauge.minBioGaugeTime)
                {
                    //죽어랏
                    world.RemoveBody(BioGauge.Body);
                    DropGaugeList.Remove(BioGauge);
                    break;
                }
                if (BioGauge.CollisionToVibot())
                {
                    Vibot.ItemPickup_HandleMessage(BioGauge.Grade);
                    world.RemoveBody(BioGauge.Body);
                    DropGaugeList.Remove(BioGauge);
                    break;
                }
            }




            foreach (CSecurePoint SecurePoint in SecurePoint_List)  //SecurePoint 관련 코드
            {
                //대기---------------------------------
                int x = SecurePoint.m_Rect.X + ((SecurePoint.m_Rect.Width / 2) - (SecurePoint.m_Rect.Width / 8));
                int y = SecurePoint.m_Rect.Y + ((SecurePoint.m_Rect.Height / 2) - (SecurePoint.m_Rect.Height / 8));
                int width = SecurePoint.m_Rect.Width / 4;
                int height = SecurePoint.m_Rect.Height / 4;
                Rectangle temp = new Rectangle(x, y, width, height);

                //      if (SecurePoint.m_Rect.Contains(new Point((int)RedBlood.Blood_Core_Position.X, (int)RedBlood.Blood_Core_Position.Y)))
                if (temp.Contains(new Point((int)RedBlood.Blood_Core_Position.X, (int)RedBlood.Blood_Core_Position.Y)))
                {
                    SecurePoint.SecureSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (SecurePoint.SecureState == SecureState.SECURING)
                        RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP;
                    else if (SecurePoint.SecureState == SecureState.SECURE)
                    {

                        RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_MOVE;
                        // m_DieSound.Play(0.5f, 0f, 0f);
                        // break;
                    }

                }
                //대기----------------------------------------------------
            }


            foreach (CMsg<eSystemMsg> Msg in System_MsgQueue)
                HandleMessage(Msg);

            System_MsgQueue.Clear();


        }





        public override void Destory()
        {


            Objects.Destory();
            WhiteCells.Destory();

            if (SecurePoint_List.Count > 0)
                SecurePoint_List.Clear();

            //   if (SpawnPoints.AllofVirusList.Count > 0)
            //   {
            //       foreach (Stuff Virus in SpawnPoints.AllofVirusList)
            //           Virus.Body.Dispose();
            //       SpawnPoints.AllofVirusList.Clear();
            //   }
            if (SpawnPoints.AllofVirusList.Count > 0)
            {

                for (int i = SpawnPoints.AllofVirusList.Count - 1; i >= 0; --i)
                    world.RemoveBody(SpawnPoints.AllofVirusList[i].Body);

                SpawnPoints.AllofVirusList.Clear();
            }


            SpawnPoints.Destory();
            //      if (RedBlood.Blood_BodyList.Count > 0)
            RedBlood.Destory();

            if (DropGaugeList.Count > 0)
            {

                for (int i = DropGaugeList.Count - 1; i >= 0; --i)
                    world.RemoveBody(DropGaugeList[i].body);

                DropGaugeList.Clear();

            }
            if (FieldofBullets.Count > 0)
                FieldofBullets.Clear();
            if (BioTrapList.Count > 0)
                BioTrapList.Clear();

        }


        public override void LoadDataFromMap(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "Secure_Points")
                {
                    XmlNodeList InnerChildNodes = ChildNode.ChildNodes;

                    foreach (XmlNode InnerChildNode in InnerChildNodes)
                    {
                        if (InnerChildNode.Name == "Secure_Point")
                        {
                            string[] SecurePoint_strings = InnerChildNode.InnerText.Split(' ');

                            CSecurePoint SecurePoint =
                                new CSecurePoint(int.Parse(SecurePoint_strings[0]), int.Parse(SecurePoint_strings[1]), int.Parse(SecurePoint_strings[2]), int.Parse(SecurePoint_strings[3]));
                            //      { m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\SecurePoint") }; // 경환: 최적화 필요?

                            SecurePoint_List.Add(SecurePoint);
                        }

                    }
                }
                break;
            }

            m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\Tiles");
            UI_Building_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Trap\\building");
        }

        public void dropGaugeonFiled(int grade, Vector2 DropPosition)
        {
            BioGauge PlusItem = null;


            switch (grade)
            {
                case 2:
                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, DropPosition, BioGaugeMODE.DROPTEM, 2);
                    break;
                case 3:
                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, DropPosition, BioGaugeMODE.DROPTEM, 3);
                    //    DropGaugeList.Add(PlusItem);
                    break;
                case 1:
                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, DropPosition, BioGaugeMODE.DROPTEM, 1);
                    //    DropGaugeList.Add(PlusItem);
                    break;

            }
            if (PlusItem != null)
                DropGaugeList.Add(PlusItem);

        }


        public override void HandleMessage(CMsg<eSystemMsg> Msg)
        {

            switch (Msg.m_Msg)
            {

                case eSystemMsg.SYSMSG_ITEM_PICKUP:
                    {
                        BioGauge Item = (BioGauge)Msg.m_Arg1;
                        DropGaugeList.Remove(Item);

                    }
                    break;


                case eSystemMsg.SYSMSG_VIRUS_ITEMDROP:
                    {

                        Stuff Virus = (Stuff)Msg.m_Arg1;


                        switch (Msg.m_Arg1.ToString())  // 아이템드랍 / 아이템 확률표
                        {
                            case "Vibot.Stuffs.H5":
                                {
                                    float RandomNumber = Rand.Next(0, 100);

                                    if (0 < RandomNumber && RandomNumber <= 2)
                                    {
                                        dropGaugeonFiled(2, Virus.bodyWorldPosition);
                                        //딜레이 코드 추가 필요   
                                        break;
                                    }
                                    else if (2 < RandomNumber && RandomNumber <= 4)
                                    {
                                        dropGaugeonFiled(3, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    //   else if (6 < RandomNumber && RandomNumber <= 9)
                                    //   {
                                    //       PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Virus.bodyWorldPosition, BioGaugeMODE.DROPTEM, 1);
                                    //       DropGaugeList.Add(PlusItem);
                                    //       break;
                                    //   }
                                    break;
                                }

                            case "Vibot.Stuffs.Flu":
                                {
                                    float RandomNumber = Rand.Next(0, 100);

                                    if (0 < RandomNumber && RandomNumber <= 5)
                                    {
                                        //딜레이 코드 추가 필요   
                                        dropGaugeonFiled(3, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (5 < RandomNumber && RandomNumber <= 8)
                                    {
                                        dropGaugeonFiled(2, Virus.bodyWorldPosition);
                                        break;
                                    }

                                    //      else
                                    //      {
                                    //          PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Virus.bodyWorldPosition, BioGaugeMODE.DROPTEM, 1);
                                    //          DropGaugeList.Add(PlusItem);
                                    //          break;
                                    //      }
                                    break;
                                }

                            case "Vibot.Stuffs.Bacillus":
                                {
                                    float RandomNumber = Rand.Next(0, 100);

                                    if (0 < RandomNumber && RandomNumber <= 10)
                                    {
                                        //딜레이 코드 추가 필요   
                                        dropGaugeonFiled(2, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (10 < RandomNumber && RandomNumber <= 16)
                                    {
                                        dropGaugeonFiled(3, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (16 < RandomNumber && RandomNumber <= 18)
                                    {
                                        dropGaugeonFiled(1, Virus.bodyWorldPosition);
                                        break;
                                    }

                                    //  else
                                    //  {
                                    //      PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Virus.bodyWorldPosition, BioGaugeMODE.DROPTEM, 1);
                                    //      DropGaugeList.Add(PlusItem);
                                    //      break;
                                    //  }
                                    break;
                                }


                            case "Vibot.Stuffs.Malignant":
                                {
                                    float RandomNumber = Rand.Next(0, 100);

                                    if (0 < RandomNumber && RandomNumber <= 15)
                                    {
                                        //딜레이 코드 추가 필요   
                                        dropGaugeonFiled(3, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (15 < RandomNumber && RandomNumber <= 27)
                                    {
                                        dropGaugeonFiled(2, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (27 < RandomNumber && RandomNumber <= 30)
                                    {
                                        dropGaugeonFiled(1, Virus.bodyWorldPosition);
                                        break;
                                    }

                                    break;

                                }
                            case "Vibot.Stuffs.Preinvasive":
                                {
                                    float RandomNumber = Rand.Next(0, 100);

                                    if (0 < RandomNumber && RandomNumber <= 15)
                                    {
                                        //딜레이 코드 추가 필요   
                                        dropGaugeonFiled(3, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (15 < RandomNumber && RandomNumber <= 30)
                                    {
                                        dropGaugeonFiled(2, Virus.bodyWorldPosition);
                                        break;
                                    }
                                    else if (30 < RandomNumber && RandomNumber <= 35)
                                    {
                                        dropGaugeonFiled(1, Virus.bodyWorldPosition);
                                        break;
                                    }

                                    break;

                                }
                        }
                    }
                    break;
            }
        }

        public virtual bool VibotCollideSphere(IObject Object, float Radius)
        {
            if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(
                new BoundingSphere(new Vector3(Object.bodyWorldPosition.X, Object.bodyWorldPosition.Y, 0), Radius))
                )
                return true;

            else
                return false;
        }




    }
}
