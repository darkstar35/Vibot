using System;
using System.Collections.Generic;
using System.Collections;
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
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using Vibot.Base;
using Vibot.Stuffs;

namespace Vibot.Actors
{
    public class Actor_RedBlood : IActor
    {
        public enum eREDBLOOD_State
        {
            REDBLOOD_START,
            REDBLOOD_MOVE,
            REDBLOOD_STOP,
        }

        public enum eDirection
        {
            DIRECTION_LEFTTOP,
            DIRECTION_LEFTBOTTOM,
            DIRECTION_RIGHTTOP,
            DIRECTION_RIGHTBOTTOM,
            DIRECTION_TOP,
            DIRECTION_BOTTOM,
            DIRECTION_LEFT,
            DIRECTION_RIGHT,
            DIRECTION_NONE,
        }


        public Vector2 Damaged = Vector2.Zero;
     
        public int Life_Count;   //생명 개수
        public int Current_GoalInPoint_Count;   //골인 개수
        public int GoalInPoint;
        public bool Blood_Goalin = false;

        public float Blood_Core_Velocity;  // 적혈구코어 속도
        public Vector2 Blood_Core_Position;
        public Vector2 m_Force;


        public eREDBLOOD_State BloodCore_State = eREDBLOOD_State.REDBLOOD_START;
        private Texture2D BloodCore_Texture;

        public List<float> m_HP;
        public List<Body> Blood_BodyList;
        public Body CollidesWith_forRedBlood;
        Vector2 Blood_Temp_Pos;
        Vector2 Blood_Road_Distance;
        public List<Vector2> Blood_RoadList;


        private int Blood_CurrentPos;
        private float Previous_X, After_X, Previous_Y, After_Y;

        private Texture2D DamagedRedBlood;
        private Texture2D DEFENSE_AREA_T;
        private Texture2D Road_CurveLineTexture;
        private Texture2D BloodRoad_LineTexture = null;
        public Texture2D DamagedBlood_texture;
        private Vector2 BloodRoad_Line_Start = new Vector2(0, 0);
    
        public Rectangle DEFENSE_AREA;
        public int RedBloodRectangleSize = 400;
        public const float RedBloodSize = 35;
        public const int RedBloodOriginalSize = 47;

        //sound
        public SoundEffect m_GoalinSound;



        public Actor_RedBlood(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch) //, Actor_StageBackGround StageBackGround)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;

            m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\RedBlood\\RedBlood");
            DamagedRedBlood = m_ContentManager.Load<Texture2D>("Sprites\\RedBlood\\RedBlood_damaged");
            BloodCore_Texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Pause_mask");
            DEFENSE_AREA_T = m_ContentManager.Load<Texture2D>("Sprites\\UI\\bloody_UI");
            Road_CurveLineTexture = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\BloodLine_Pixel");

            m_DamagedSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Redblood\\heart");
            m_DieSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Redblood\\heart");
            m_GoalinSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Redblood\\goalin");
            DamagedBlood_texture = ContentManager.Load<Texture2D>("Sprites\\RedBlood\\RedBlood_damaged");
            BloodRoad_LineTexture = new Texture2D(m_GraphicDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];  // 적혈구 상태에 따라 라인 색갈을 변경할 수 있도록 고려한듯?
            pixels[0] = Color.Purple;
            BloodRoad_LineTexture.SetData<Color>(pixels);

        }



        public override void LoadDataFromMap(XmlNode Node)
        {

          //  CollidesWith_forRedBlood = new Body[2]; // 충돌 카타고리 

            Blood_RoadList = new List<Vector2>();
            Blood_CurrentPos = 1;



            XmlNodeList ChildNodes = Node.ChildNodes;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                switch (ChildNode.Name)
                {

                    case "CoreVelocity":
                        Blood_Core_Velocity = float.Parse(ChildNode.InnerText);
                        break;
                    case "LifeNumber":
                        Life_Count = int.Parse(ChildNode.InnerText);
                        Blood_BodyList = new List<Body>(Life_Count);
                        m_HP = new List<float>(Life_Count);

                        for (int i = 0; i < Life_Count; i++)
                            m_HP.Add(1f);
                        break;

                    case "Location":
                        string[] Location = ChildNode.InnerText.Split(' ');
                        Blood_Core_Position = new Vector2(float.Parse(Location[0]), float.Parse(Location[1]));

                        break;
                    case "Road":
                        {
                            XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
                            foreach (XmlNode ChildNode2 in ChildNodes2)
                            {
                                if (ChildNode2.Name == "Pos")
                                {
                                    string[] Pos = ChildNode2.InnerText.Split(' ');

                                    Vector2 newRoad_Pos = new Vector2(int.Parse(Pos[0]), int.Parse(Pos[1]));

                                    Blood_RoadList.Add(newRoad_Pos);
                                }
                            }

                        }
                        break;

                }
            }


            SetUpPhysics(world, Blood_Core_Position, RedBloodSize / 2.14f, 120.0f);

        
                for (int i = 0; i < Blood_BodyList.Count; i++)
                {
                    Blood_BodyList[i].CollisionCategories = Category.Cat1;
                    Blood_BodyList[i].CollidesWith = Category.Cat1 | Category.Cat2 | Category.Cat30 | Category.Cat5 | ~Category.All;
                //        | ~Category.Cat18  /*Parasite*/      | ~Category.Cat3  /*Tower*/| ~Category.Cat4 | ~Category.Cat6;
                }
        


            CollidesWith_forRedBlood = BodyFactory.CreateCircle(world, 0.5f, 0.1f, ConvertUnits.ToSimUnits(Blood_Core_Position));
            CollidesWith_forRedBlood.BodyType = BodyType.Dynamic;
      //      CollidesWith_forRedBlood.CollisionCategories = Category.Cat30;
            CollidesWith_forRedBlood.CollidesWith = Category.Cat1 |  ~Category.Cat2  /*Vibot*/ 
                |~Category.Cat3  /*Tower*/| ~Category.Cat4 | ~Category.Cat6 | ~Category.Cat7 | ~Category.All;


        }
        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {
            // Body body;
            for (int count = 0; count < Life_Count; count++)
            {
                Body body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position)); //ConvertUnits.ToSimUnits(position)
                body.BodyType = BodyType.Dynamic;
                Blood_BodyList.Add(body);

            }
        }

 

        public void OnUpdate(GameTime gameTime)
        {


            DEFENSE_AREA = new Rectangle(((int)Blood_Core_Position.X - RedBloodRectangleSize / 2) - (int)cCamera.CameraPosition.X, (int)((int)Blood_Core_Position.Y - RedBloodRectangleSize / 2) - (int)cCamera.CameraPosition.Y,
                RedBloodRectangleSize, RedBloodRectangleSize);


            if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
            {
                MoveRedBloodCore(); // Core 의 이동

                m_Force.X = MathHelper.Clamp(m_Force.X, 0f, 2.5f);
                m_Force.Y = MathHelper.Clamp(m_Force.Y, 0f, 2.5f);


                if (Blood_BodyList.Count == 0)
                {
                    BloodCore_State = eREDBLOOD_State.REDBLOOD_STOP; // 멈추고 
                    Damaged = Vector2.Zero;
                }

                for (int i = 0; i < Blood_BodyList.Count; i++)
                {

                    Blood_BodyList[i].Position = Vector2.SmoothStep(Blood_BodyList[i].Position, ConvertUnits.ToSimUnits(Blood_Core_Position), 0.11f);  // 이게 한곳으로 모아주는겁니다
                    Blood_BodyList[i].ApplyForce(m_Force, Blood_BodyList[i].Position); // Blood 알갱이들의  따라감

                    if (Actor_StageBackGround.Goal_Zone.Contains((int)ConvertUnits.ToDisplayUnits(Blood_BodyList[i].Position).X,
                        (int)ConvertUnits.ToDisplayUnits(Blood_BodyList[i].Position).Y))
                    {
                        m_HP.RemoveAt(i);
                        
                        world.RemoveBody(Blood_BodyList[i]);

                        Blood_BodyList.RemoveAt(i);
                        Blood_Goalin = true;
                        Current_GoalInPoint_Count++;
                        m_GoalinSound.Play();
                        break;
                    }
                    else if (m_HP[i] <= 0f)
                    {
                            
                        m_HP.RemoveAt(i);

                        world.RemoveBody(Blood_BodyList[i]);

                        Blood_BodyList.RemoveAt(i);
        
                        m_DieSound.Play();
                        break;
                    }


                }


                Life_Count = Blood_BodyList.Count;

            }

        }






        public void OnDraw(GameTime gameTime)
        {
            Draw_BloodRoad_Line();
            m_SpriteBatch.Draw(DEFENSE_AREA_T, DEFENSE_AREA, Color.White);

            for (int i = 0; i < Blood_BodyList.Count; i++)
            {
                Vector2 BloodPosition = ConvertUnits.ToDisplayUnits(Blood_BodyList[i].Position) - cCamera.CameraPosition;
 

                m_SpriteBatch.Draw(m_Texture, cCamera.Transform(ConvertUnits.ToDisplayUnits(Blood_BodyList[i].Position)),        // 야매 코드 임
                     new Rectangle((RedBloodOriginalSize * ((int)gameTime.TotalGameTime.TotalMilliseconds % 3)), 0, RedBloodOriginalSize, RedBloodOriginalSize),  // 애니메이션
                      Color.White, Blood_BodyList[i].Rotation, new Vector2(RedBloodOriginalSize / 2, RedBloodOriginalSize / 2), (RedBloodSize / (float)RedBloodOriginalSize), SpriteEffects.None, 0f);

            }
            if (Damaged != Vector2.Zero)
                m_SpriteBatch.Draw(DamagedBlood_texture, ConvertUnits.ToDisplayUnits(Damaged) - cCamera.CameraPosition, 
                new Rectangle(47 * (int)gameTime.TotalGameTime.TotalMilliseconds % 3, 0, 47, 47), Color.White);


          

        }


        public override void Destory()
        {

            if (Blood_BodyList.Count > 0)
            {
                for (int i = 0; i < Blood_BodyList.Count; i++)
                    world.RemoveBody(Blood_BodyList[i]);

                world.RemoveBody(CollidesWith_forRedBlood);

                Blood_BodyList.Clear();
           
            }


            if (Blood_RoadList.Count > 0)
                Blood_RoadList.Clear();

            Life_Count = 0;   //생명 개수
            Current_GoalInPoint_Count = 0;   //골인 개수
            GoalInPoint = 0;

        }


        private eDirection Check_Blood_Direction()
        {


            if (Previous_X > After_X && Previous_Y > After_Y)
                return eDirection.DIRECTION_LEFTTOP;
            if (Previous_X > After_X && Previous_Y < After_Y)
                return eDirection.DIRECTION_LEFTBOTTOM;

            if (Previous_X < After_X && Previous_Y > After_Y)
                return eDirection.DIRECTION_RIGHTTOP;
            if (Previous_X < After_X && Previous_Y < After_Y)
                return eDirection.DIRECTION_RIGHTBOTTOM;

            if (Previous_Y > After_Y)
                return eDirection.DIRECTION_TOP;
            else if (Previous_Y < After_Y)
                return eDirection.DIRECTION_BOTTOM;
            if (Previous_X > After_X)
                return eDirection.DIRECTION_LEFT;
            else if (Previous_X < After_X)
                return eDirection.DIRECTION_RIGHT;


            return eDirection.DIRECTION_NONE;
        }



        public void MoveRedBloodCore()
        {

            if (Blood_RoadList.Count != 0)
            {

                if (BloodCore_State == eREDBLOOD_State.REDBLOOD_START)
                {
                    BloodCore_State = eREDBLOOD_State.REDBLOOD_MOVE;
                }
                else if (BloodCore_State == eREDBLOOD_State.REDBLOOD_MOVE)
                {
                    Previous_X = Blood_Core_Position.X;
                    Previous_Y = Blood_Core_Position.Y;

                    After_X = Blood_RoadList[Blood_CurrentPos].X;
                    After_Y = Blood_RoadList[Blood_CurrentPos].Y;

                    Blood_Temp_Pos.X = Previous_X;
                    Blood_Temp_Pos.Y = Previous_Y;

                    Blood_Road_Distance.X = After_X - Previous_X;
                    Blood_Road_Distance.Y = After_Y - Previous_Y;
                    Blood_Road_Distance.Normalize();

                    bool NeedtoChange = false;

                    Blood_Temp_Pos.X += Blood_Road_Distance.X * Blood_Core_Velocity;
                    Blood_Temp_Pos.Y += Blood_Road_Distance.Y * Blood_Core_Velocity;
     

                    switch (Check_Blood_Direction())
                    {
                        case eDirection.DIRECTION_TOP:
                            if ((int)Blood_Temp_Pos.Y <= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_BOTTOM:
                            if ((int)Blood_Temp_Pos.Y >= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_LEFT:
                            if ((int)Blood_Temp_Pos.X <= Blood_RoadList[Blood_CurrentPos].X)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_RIGHT:
                            if ((int)Blood_Temp_Pos.X >= Blood_RoadList[Blood_CurrentPos].X)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_LEFTTOP:
                            if ((int)Blood_Temp_Pos.X <= Blood_RoadList[Blood_CurrentPos].X && (int)Blood_Temp_Pos.Y <= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;

                        case eDirection.DIRECTION_LEFTBOTTOM:
                            if ((int)Blood_Temp_Pos.X <= Blood_RoadList[Blood_CurrentPos].X && (int)Blood_Temp_Pos.Y >= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_RIGHTTOP:
                            if ((int)Blood_Temp_Pos.X >= Blood_RoadList[Blood_CurrentPos].X && (int)Blood_Temp_Pos.Y <= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_RIGHTBOTTOM:
                            if ((int)Blood_Temp_Pos.X >= Blood_RoadList[Blood_CurrentPos].X && (int)Blood_Temp_Pos.Y >= Blood_RoadList[Blood_CurrentPos].Y)
                                NeedtoChange = true;
                            break;
                        case eDirection.DIRECTION_NONE:
                            break;
                    }



                    if (NeedtoChange)
                    {
                        if (Blood_CurrentPos == Blood_RoadList.Count - 1) // CorePosition이 정해진 루트에 다 오게 되었다면   
                            BloodCore_State = eREDBLOOD_State.REDBLOOD_STOP;
                        else
                        {
                            Blood_CurrentPos++;
                            BloodCore_State = eREDBLOOD_State.REDBLOOD_START;
                        }

                    }
                    else
                        Blood_Core_Position = Blood_Temp_Pos;


                }
            }

        }
        public Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {

            float cube = t * t * t;
            float square = t * t;

            float ax = 3 * (p1.X - p0.X);
            float ay = 3 * (p1.Y - p0.Y);

            float bx = 3 * (p2.X - p1.X) - ax;
            float by = 3 * (p2.Y - p1.Y) - ay;

            float cx = p3.X - p0.X - ax - bx;
            float cy = p3.Y - p0.Y - ay - by;


            float x = (cx * cube) + (bx * square) + (ax * t) + p0.X;
            float y = (cy * cube) + (by * square) + (ay * t) + p0.Y;

            return new Vector2(x, y);
        }


        public Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {

            //  float cube = t * t * t;
            float square = t * t;

            float ax = 3 * (p1.X - p0.X);
            float ay = 3 * (p1.Y - p0.Y);

            float bx = 3 * (p2.X - p1.X) - ax;
            float by = 3 * (p2.Y - p1.Y) - ay;

            // float cx = p3.X - p0.X - ax - bx;
            // float cy = p3.Y - p0.Y - ay - by;


            float x = (bx * square) + (ax * t) + p0.X;   //  (cx * cube) +
            float y = (by * square) + (ay * t) + p0.Y;   //  (cy * cube) +

            return new Vector2(x, y);
        }

        private float Rotate(Vector2 Location, Vector2 TurnTowards, float CurrentAngle, float TurnSpeed)
        {
            float x = TurnTowards.X - Location.X;
            float y = TurnTowards.Y - Location.Y;

            float DesiredAngle = (float)Math.Atan2(y, x);
            float Difference = Wrap(DesiredAngle - CurrentAngle);

            Difference = MathHelper.Clamp(Difference, -TurnSpeed, TurnSpeed);

            return Wrap(CurrentAngle + Difference);
        }

        private float Wrap(float Radians)
        {
            while (Radians < -MathHelper.Pi)
                Radians += MathHelper.TwoPi;

            while (Radians > MathHelper.Pi)
                Radians -= MathHelper.TwoPi;

            return Radians;
        }
        List<Vector2> PointsInCurve = new List<Vector2>(); // 베이저 알고리즘
        private void Draw_BloodRoad_Line()
        {

            /*
            for (int i = 0; i < Blood_RoadList.Count; i++)
            {

              if(i > 0)
              {
                  if (i % 3 == 0)
                  {
                     for (float j = 0.0f; j < 1.0f; j += 0.001f)
                         PointsInCurve.Add(GetPoint(j, Blood_RoadList[i - 3], Blood_RoadList[i - 2], Blood_RoadList[i - 1], Blood_RoadList[i - 0]));

                      foreach (Vector2 PixelPoint in PointsInCurve)
                          m_SpriteBatch.Draw(Road_CurveLineTexture, cCamera.Transform(PixelPoint), null, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

                      if (PointsInCurve.Count > 0)
                          PointsInCurve.Clear();
                  }
        
              // else
              // {
              //    for (float j = 0.0f; j < 1.0f; j += 0.0005f)
              //         PointsInCurve.Add(GetPoint(j, Blood_RoadList[Blood_RoadList.Count - 3], Blood_RoadList[Blood_RoadList.Count - 2], Blood_RoadList[Blood_RoadList.Count - 1]));
              //     foreach (Vector2 PixelPoint in PointsInCurve)
              //         m_SpriteBatch.Draw(Road_CurveLineTexture, cCamera.Transform(PixelPoint), null, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
              //
              //     if (PointsInCurve.Count > 0)
              //         PointsInCurve.Clear();
              // }
              }
            }
                  */


            /////////////////////////////그냥 라인 그리는것 부분 
            float distance, angle;

            Vector2 BloodRoad_Line_Start;
            Vector2 BloodRoad_Line_End;

            for (int count = 0; count < Blood_RoadList.Count - 1; ++count)
            {
                BloodRoad_Line_Start.X = Blood_RoadList[count].X;
                BloodRoad_Line_Start.Y = Blood_RoadList[count].Y;

                BloodRoad_Line_End.X = Blood_RoadList[count + 1].X;
                BloodRoad_Line_End.Y = Blood_RoadList[count + 1].Y;

                distance = Vector2.Distance(BloodRoad_Line_Start, BloodRoad_Line_End);
                angle = (float)Math.Atan2((double)(BloodRoad_Line_End.Y - BloodRoad_Line_Start.Y), (double)(BloodRoad_Line_End.X - BloodRoad_Line_Start.X));

                m_SpriteBatch.Draw(BloodRoad_LineTexture, new Vector2(BloodRoad_Line_Start.X - cCamera.CameraPosition.X, BloodRoad_Line_Start.Y - cCamera.CameraPosition.Y), null, Color.White, angle, Vector2.Zero, new Vector2(distance, 1), SpriteEffects.None, 1.0f);

            }
        }

      



    }
}
