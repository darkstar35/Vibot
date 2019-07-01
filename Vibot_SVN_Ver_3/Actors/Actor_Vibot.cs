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
using FarseerPhysics.Collision.Shapes;
using Vibot.Base;
using Vibot.Stuffs;
using System.Diagnostics;

namespace Vibot.Actors
{

    public class Actor_Vibot : IActor
    {
        public static CElement Vibot_Laser_Element;
 
        public Queue<BioGauge> BioGauge_Inven;
        public int KillCount = 0;

        private Texture2D UI_ReadytoBulid;
        private Texture2D Bulid_LineTexture;
        private Texture2D UI_Building_Texture;
        //대기
        private Texture2D DashEffect;

        bool LinedrawMode_ForBuilding;

        public double Mouse_angle;
        public float LaserLineLength;
        float LaserLineGrowingLength;

        private Vector2[] BioGauge_SpinPosition;

        const float Bioline_Maxmum_Length = 150f; //수정 120314

     
        public const int MaxWhiteGrow = 20;
        public const int MaxWhiteGauge = 5;

        const float ForceAmount = 2.0f;
        const float ForceAccelate = 3.5f; // 3.9f
        const float Maxium_Speed = 3.8f; // 3.9f
        const int ItemInven_Capacity = 3;

        public float WhiteGauge_Value;
        public bool WhiteGaugeGrow = true;
        public bool Dashon = false;
        float DashTime = 1.0f;
        float Laser_Spin = 0;

        public BioGauge ReloadBioGauge;
        public Actor_WhiteCellManager WhiteCells;


        public Actor_Vibot(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Actor_WhiteCellManager WhiteCells)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
            this.WhiteCells = WhiteCells;

        }
        

        public override void LoadDataFromMap(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            Vector2 position = Vector2.Zero;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                switch (ChildNode.Name)
                {

                    case "Location":
                        string[] Rect = ChildNode.InnerText.Split(' ');
                        position = new Vector2(float.Parse(Rect[0]), float.Parse(Rect[1]));

                        break;
                }
            }

            m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Vibot\\Vibot");
            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");

            BioGauge_Inven = new Queue<BioGauge>(ItemInven_Capacity);
            BioGauge_SpinPosition = new Vector2[ItemInven_Capacity];

            UI_ReadytoBulid = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Build_AreaUI");
            UI_Building_Texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Building");

            //대기
            DashEffect = m_ContentManager.Load<Texture2D>("Sprites\\Vibot\\dash");


            //  Bulid_LineTexture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\LineTocursor");

            Bulid_LineTexture = new Texture2D(m_GraphicDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] pixels = new Color[1];  // 적혈구 상태에 따라 라인 색갈을 변경할 수 있도록 고려한듯?
            pixels[0] = Color.YellowGreen;
            Bulid_LineTexture.SetData<Color>(pixels);

            //  m_DieSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Vibot\\Vibot_Dash");
            m_DamagedSound = m_ContentManager.Load<SoundEffect>("Audio\\IN_GAME\\Vibot\\Vibot_Dash");

            for (int i = 0; i < BioGauge_SpinPosition.Length; i++)
                BioGauge_SpinPosition[i] = Vector2.Zero;


            LaserLineGrowingLength = 0f;
            LinedrawMode_ForBuilding = false;

            this.width = 60; //(float)m_Texture.Width;
            this.height = 60; //float)m_Texture.Height;
            this.mass = 20.0f;
            SetUpPhysics(world, position, width, height, mass);

            Vibot_Laser_Element = new CElement(bodyWorldPosition.X, bodyWorldPosition.Y, (m_Texture.Width) / 2);
            Vibot_Laser_Element.m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Vibot\\combo_line");

        }

        public override void SetUpPhysics(World world, Vector2 Pos, float width, float height, float mass)
        {

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(width / 2.34), mass, ConvertUnits.ToSimUnits(Pos));
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0f;
            body.Friction = 5f;
            body.CollisionCategories = Category.Cat2;
        //    body.CollidesWith = Category.All | ~Category.Cat1 | ~Category.Cat4 | ~Category.Cat5;

        }

        public void SelectBiogage(KeyboardState KeyState, KeyboardState oldKeyState)
        {


            if (BioGauge_Inven.Count > 0)
            {
                Queue<BioGauge> BioGaugeInven_Temp = new Queue<BioGauge>(ItemInven_Capacity);

               
                if (KeyState.IsKeyDown(Keys.D1) && !oldKeyState.IsKeyDown(Keys.D1))
                {
                    if (BioGauge_Inven.Peek().Grade != 0)
                        for (int i = 0; i < BioGauge_Inven.Count; i++)
                        {
                            if (BioGauge_Inven.ElementAt<BioGauge>(i).Grade == 0) // loop를 돌면서 해당 grade가 있는지 찾는다 
                            {
                                BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(i));
                                int tempcount = i;

                                for (int j = 0; j < BioGauge_Inven.Count; j++)
                                {
                                    if (j == tempcount)
                                        continue;
                                    BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(j));   //바꿔치기 
                                }

                                BioGauge_Inven = BioGaugeInven_Temp;
                                break;
                            }
                        }
                }
                 //   인벤토리안에 Grade 1인 아이템을 dequeue 한다 
                else if (KeyState.IsKeyDown(Keys.D2) && !oldKeyState.IsKeyDown(Keys.D2))
                {
                    if (BioGauge_Inven.Peek().Grade != 2)
                        for (int i = 0; i < BioGauge_Inven.Count; i++)
                        {
                            if (BioGauge_Inven.ElementAt<BioGauge>(i).Grade == 2) // loop를 돌면서 해당 grade가 있는지 찾는다 
                            {
                                BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(i));
                                int tempcount = i;

                                for (int j = 0; j < BioGauge_Inven.Count; j++)
                                {
                                    if (j == tempcount)
                                        continue;
                                    BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(j));   //바꿔치기 
                                }

                                BioGauge_Inven = BioGaugeInven_Temp;
                                break;
                            }
                        }
                }
                else if (KeyState.IsKeyDown(Keys.D3) && !oldKeyState.IsKeyDown(Keys.D3))
                {
                    if (BioGauge_Inven.Peek().Grade != 3)
                        for (int i = 0; i < BioGauge_Inven.Count; i++)
                        {
                            if (BioGauge_Inven.ElementAt<BioGauge>(i).Grade == 3) // loop를 돌면서 해당 grade가 있는지 찾는다 
                            {
                                BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(i));
                                int tempcount = i;

                                for (int j = 0; j < BioGauge_Inven.Count; j++)
                                {
                                    if (j == tempcount)
                                        continue;
                                    BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(j));   //바꿔치기 
                                }

                                BioGauge_Inven = BioGaugeInven_Temp;
                                break;
                            }
                        }
                }
                else if (KeyState.IsKeyDown(Keys.D4) && !oldKeyState.IsKeyDown(Keys.D4))
                {
                    if (BioGauge_Inven.Peek().Grade != 1)
                        for (int i = 0; i < BioGauge_Inven.Count; i++)
                        {
                            if (BioGauge_Inven.ElementAt<BioGauge>(i).Grade == 1) // loop를 돌면서 해당 grade가 있는지 찾는다 
                            {
                                BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(i));
                                int tempcount = i;

                                for (int j = 0; j < BioGauge_Inven.Count; j++)
                                {
                                    if (j == tempcount)
                                        continue;
                                    BioGaugeInven_Temp.Enqueue(BioGauge_Inven.ElementAt<BioGauge>(j));   //바꿔치기 
                                }

                                BioGauge_Inven = BioGaugeInven_Temp;
                                break;
                            }
                        }
                }
         
                /*
              
                  queue 꺼내야 queue peek()
    for문으로 해당 grade item이 있는지 검사 
    있다면 해당 grade itemd를 peekque로 교체
    기존에 있던 peekqueue는 tempqueue로 이동
    기존 inven queue에 있던 남은 item들도 tempqueue로 이동 
    Temp queue와 기존 INVEN QUEUE를 교체 
              
                 */
            }

        }


        KeyboardState oldKeyState;
        MouseState oldMouseState;
     
        private float bioGaegeTimer = 0.0f; //바이오 게이지를 바로 안사라지게 하기 위한 타이머

        public void OnUpdate(GameTime gameTime)  //조작
        {
            Dashon = false;
            bioGaegeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState KeyState = Keyboard.GetState();
            MouseState MouseState = Mouse.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            

            ////////////////////////////////// 최대 속력 안에 조절 한다 ////////////////////////////////////
            if (body.LinearVelocity.Y >= Maxium_Speed || body.LinearVelocity.X >= Maxium_Speed || body.LinearVelocity.Y <= -Maxium_Speed || body.LinearVelocity.X <= -Maxium_Speed)
                body.LinearVelocity = Vector2.SmoothStep(body.LinearVelocity, Vector2.Zero, 0.3f); //0.3

            DashTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
            if (DashTime > 1.8f)
                Dashon = true;

             //대기
             //휠버튼에서 손을 떼도 1초 동안 살아있다.
                 if (bioGaegeTimer > 1.0f) //1초 지나면 죽인당 
                 {
                     Debug.WriteLine("죽을때가 됐넹");
                     ReloadBioGauge = null;
                     bioGaegeTimer = 0;
                 }

         //    if(gamePadState != null)
         //         HandleGamepadInput(gamePadState, gameTime);
            if(KeyState != null)
                  HandleKeyboardInput(KeyState, MouseState, gameTime);
           
            //  body.LinearVelocity.Normalize();
            //  body.LinearVelocity *= ForceAmount;   
            /////////////////////////////////////////////////////////////////////WHITECELL code //////////////////////////////////////////////////////////////////

            int WhiteGaugeCnt = 0;

            for (int i = BioGauge_Inven.Count - 1; i >= 0; i--)
                if (BioGauge_Inven.ElementAt<BioGauge>(i).Grade == 0)
                    WhiteGaugeCnt++;


            //MouseState.LeftButton != ButtonState.Pressed && 
            if (WhiteGaugeGrow && WhiteGauge_Value <= MaxWhiteGrow && WhiteGaugeCnt < MaxWhiteGauge) //13000 ->130
                WhiteGauge_Value += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (LaserLineGrowingLength < 0f)
                LaserLineGrowingLength = 0f;
            if (LaserLineLength > Bioline_Maxmum_Length)
                LaserLineLength = Bioline_Maxmum_Length;

            if (WhiteGauge_Value > MaxWhiteGrow && WhiteGaugeCnt < MaxWhiteGauge)
            {
                BioGauge PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 0);
                BioGauge_Inven.Enqueue(PlusItem);
                WhiteGauge_Value = 0;
            }

        }


        public bool IsCheckSelectWhiteCell(List<WhiteCell> WhiteCell_List)
        {
            bool IsHannom = false;


            for (int j = 0; j < WhiteCell_List.Count; j++)
                if (WhiteCells.WhiteCell_List[j].ISselectedcell == true)
                    IsHannom = true;


            if (IsHannom) // 한놈을 잡았다면 
                return true;
            else
                return false;

        }

        private void HandleGamepadInput(GamePadState gamePadState, GameTime gameTime)
        {

                this.body.LinearVelocity += new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y); 
            
            //new Vector2(0, ForceAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

            StopAccelate(gameTime, 6f, gamePadState);  //감속 메서드 2.5 3.8


            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
               
            }
        }

        float LaserLineGrowingFollwing = 0;
        private void HandleKeyboardInput(KeyboardState KeyState, MouseState MouseState, GameTime gameTime)
        {
            SelectBiogage(KeyState, oldKeyState);

            if (KeyState.IsKeyDown(Keys.W) || KeyState.IsKeyDown(Keys.Up))
                this.body.LinearVelocity -= new Vector2(0, ForceAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

            if (KeyState.IsKeyDown(Keys.S) || KeyState.IsKeyDown(Keys.Down))
                this.body.LinearVelocity += new Vector2(0, ForceAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

            if (KeyState.IsKeyDown(Keys.A) || KeyState.IsKeyDown(Keys.Left))
                this.body.LinearVelocity -= new Vector2(ForceAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);

            if (KeyState.IsKeyDown(Keys.D) || KeyState.IsKeyDown(Keys.Right))
                this.body.LinearVelocity += new Vector2(ForceAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);

            StopAccelate(gameTime, 6f, KeyState);  //감속 메서드 2.5 3.8


            if (KeyState.IsKeyDown(Keys.Space))
                Dash(KeyState, gameTime, 14.3f); //15면 강해 





            /////////////////////////////////////////////////////////////////////bioGauge shot  code //////////////////////////////////////////////////////////////////

            if ((MouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton != ButtonState.Pressed)
               || KeyState.IsKeyDown(Keys.F) && !oldKeyState.IsKeyDown(Keys.F)) // 안되면 위로 올릴것
            {
                if (BioGauge_Inven.Count > 0)
                    SelectBioGauge();
            }



            if (MouseState.MiddleButton == ButtonState.Pressed || KeyState.IsKeyDown(Keys.F))
            {
                if (ReloadBioGauge != null    && Vector2.Distance(bodyWorldPosition, ReloadBioGauge.Bullet_Position) < LaserLineLength) 
                {
                    LaserLineGrowingFollwing += (float)gameTime.ElapsedGameTime.Milliseconds / 10;
                    ReloadBioGauge.Bullet_Position = bodyWorldPosition + new Vector2((float)Math.Cos(Mouse_angle) * LaserLineGrowingFollwing, (float)Math.Sin(Mouse_angle) * LaserLineGrowingFollwing) * 2;
                 

                    //  if (TrapClickGametime > 0 && Vector2.Distance(cCamera.Transform(ReloadBioGauge.Bullet_Position), new Vector2(MouseState.X, MouseState.Y)) < 5)
                    //      ReloadBioGauge.Bullet_Position -= new Vector2((float)Math.Cos(Mouse_angle) * 3, (float)Math.Sin(Mouse_angle) * 3);

                }
                bioGaegeTimer = 0;
            }
            else
                LaserLineGrowingFollwing = 0;



                Vibot_Laser_Element.m_Sphere.Radius = (Vibot_Laser_Element.m_Texture.Width) / 2 + WhiteGauge_Value/5; // 반경 

                if (MouseState.LeftButton == ButtonState.Pressed && (Mouse.GetState().X < cCamera.SCREEN_WIDTH && Mouse.GetState().Y < cCamera.SCREEN_HEIGHT))
                {
                    LinedrawMode_ForBuilding = true; // 주변에 영역을 그리고  

       
                        if (LaserLineLength < Bioline_Maxmum_Length)
                            LaserLineGrowingLength += (float)gameTime.ElapsedGameTime.Milliseconds / 10;  // 레이저 증가선 길이 증가 
                        else
                            LaserLineGrowingLength -= (float)gameTime.ElapsedGameTime.Milliseconds / 10; // 레이저 증가선 길이 감소  
               


                    bool IsHannom = false;
                    ///
                    /// WhiteCell Update 부문 
                    ///
                    for (int i = WhiteCells.WhiteCell_List.Count - 1; i >= 0; i--) // Whtecell 선택 코드
                    {

                        for (int j = 0; j < WhiteCells.WhiteCell_List.Count; j++)
                            if (WhiteCells.WhiteCell_List[j].ISselectedcell == true)
                                IsHannom = true; // Whtecell 중복선택 방지 코드

                        if (IsHannom) // 한놈을 잡았다면 
                            break;          // 루프를 빠져나온다 
                    
                        else if (Vector2.Distance(WhiteCells.WhiteCell_List[i].bodyWorldPosition, bodyWorldPosition) < LaserLineLength)
                        {
                            if (Vector2.Distance(bodyWorldPosition + new Vector2((float)Math.Cos(Mouse_angle) * LaserLineGrowingLength, (float)Math.Sin(Mouse_angle) * LaserLineGrowingLength),     //LaserLineLength / 2
                                WhiteCells.WhiteCell_List[i].bodyWorldPosition) <= (LaserLineLength/2)  ) //방향성 체크 
                            {
                                WhiteCells.WhiteCell_List[i].ISselectedcell = true;
                                break;
                            }
                        }
                    }



                }
                else
                {
                    LinedrawMode_ForBuilding = false;
                    LaserLineGrowingLength -= (float)gameTime.ElapsedGameTime.Milliseconds / 10;
                }


                LaserLineLength = LaserLineGrowingLength + 45; //Vector2.Distance(VibotViewPortPosition, new Vector2(MouseState.X, MouseState.Y)) *


                for (int i = WhiteCells.WhiteCell_List.Count - 1; i >= 0; i--)
                {
                    if (WhiteCells.WhiteCell_List[i].ISselectedcell == true)
                    {
                        WhiteCells.WhiteCell_List[i].body.Position = Vector2.SmoothStep(WhiteCells.WhiteCell_List[i].body.Position, this.body.Position + ConvertUnits.ToSimUnits(new Vector2((float)Math.Cos(Mouse_angle) * LaserLineLength, (float)Math.Sin(Mouse_angle) * LaserLineLength)), 0.30f);

                        if (MouseState.RightButton == ButtonState.Pressed)  // 우측 버튼을 클릭하면 현재 선택 취소
                        {
                            WhiteCells.WhiteCell_List[i].ISselectedcell = false;
                            break;
                        }

                    }
                }
      


            /////////////////////////////////////////////////////////////////////WHITECELL 관련 코드 //////////////////////////////////////////////////////////////////

            //ZOOM IN/OUT        
            if (MouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue)
                cCamera.Zoom += 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds; // Zoom_in
            if (MouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue)
                cCamera.Zoom -= 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;// Zoom_out



            oldKeyState = KeyState;
            oldMouseState = MouseState;

        }

        private void Dash(KeyboardState keyState, GameTime gameTime, float dash_increase_value)  // 대쉬 
        {

            if (keyState.IsKeyDown(Keys.W) && Dashon)
            {
                body.LinearVelocity = Vector2.SmoothStep(body.LinearVelocity, body.LinearVelocity + new Vector2(0, -ForceAccelate), dash_increase_value);
                //  Force.Y -= (ForceAccelate + (float)gameTime.ElapsedGameTime.TotalSeconds);
                Dashon = false;
                DashTime = 0;
                m_DamagedSound.Play();
            }

            if (keyState.IsKeyDown(Keys.S) && Dashon)
            {
                body.LinearVelocity = Vector2.SmoothStep(body.LinearVelocity, body.LinearVelocity + new Vector2(0, ForceAccelate), dash_increase_value);
                Dashon = false;
                DashTime = 0;
                m_DamagedSound.Play();
            }

            if (keyState.IsKeyDown(Keys.A) && Dashon)
            {
                body.LinearVelocity = Vector2.SmoothStep(body.LinearVelocity, body.LinearVelocity + new Vector2(-ForceAccelate, 0), dash_increase_value);
                //  Force.X -= (ForceAccelate + (float)gameTime.ElapsedGameTime.TotalSeconds);
                Dashon = false;
                DashTime = 0;
                m_DamagedSound.Play();
            }
            if (keyState.IsKeyDown(Keys.D) && Dashon) //DashTime >= 2
            {
                body.LinearVelocity = Vector2.SmoothStep(body.LinearVelocity, body.LinearVelocity + new Vector2(ForceAccelate, 0), dash_increase_value);
                //  Force.X += (ForceAccelate + (float)gameTime.ElapsedGameTime.TotalSeconds);
                Dashon = false;
                DashTime = 0;
                m_DamagedSound.Play();
            }

        }



        public void StopAccelate(GameTime gameTime, float StopAmount, KeyboardState keyState)
        {
            //////////////////////////////// 일정 속력 감속 한다 //////////////////////////
            if (keyState.IsKeyUp(Keys.D) && keyState.IsKeyUp(Keys.A))
            {
                if (body.LinearVelocity.X > 0)
                {
                    this.body.LinearVelocity -= new Vector2(StopAmount * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    if(!keyState.IsKeyUp(Keys.D))
                       this.body.LinearVelocity -= new Vector2(StopAmount * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                }
                else if (body.LinearVelocity.X < 0)
                {
                    this.body.LinearVelocity += new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);
                    if(!keyState.IsKeyUp(Keys.A))
                      this.body.LinearVelocity += new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);
                }
            }
        

            if (keyState.IsKeyUp(Keys.W) && keyState.IsKeyUp(Keys.S))
            {
                if (body.LinearVelocity.Y > 0)
                {
                    this.body.LinearVelocity -= new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));
                    if (!keyState.IsKeyUp(Keys.S))
                        this.body.LinearVelocity -= new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));
                }
                else if (body.LinearVelocity.Y < 0)
                {
                    this.body.LinearVelocity += new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));
                    if (!keyState.IsKeyUp(Keys.W))
                        this.body.LinearVelocity += new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));
                }
            }
        }

        public void StopAccelate(GameTime gameTime, float StopAmount, GamePadState GamePad)
        {
            //////////////////////////////// 일정 속력 감속 한다 //////////////////////////
            if (GamePad.ThumbSticks.Left.X != 0)
            {
                if (body.LinearVelocity.X > 0)
                    this.body.LinearVelocity -= new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);
                else if (body.LinearVelocity.X < 0)
                    this.body.LinearVelocity += new Vector2(StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds), 0);

            }
            if (GamePad.ThumbSticks.Left.Y != 0)
            {
                if (body.LinearVelocity.Y > 0)
                    this.body.LinearVelocity -= new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

                else if (body.LinearVelocity.Y < 0)
                    this.body.LinearVelocity += new Vector2(0, StopAmount * ((float)gameTime.ElapsedGameTime.TotalSeconds));

            }
        }


        public int SelectBioGauge()
        {
            float posX = Vibot_Laser_Element.m_Sphere.Center.X + (float)Math.Cos(Mouse_angle);
            float posY = Vibot_Laser_Element.m_Sphere.Center.Y + (float)Math.Sin(Mouse_angle);
            Vector2 BioBallReloadPosition = new Vector2(posX, posY);


            switch (BioGauge_Inven.Peek().Grade)
            {

                case 0:   // 하얀색 게이지
                    ReloadBioGauge = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, BioBallReloadPosition, BioGaugeMODE.BULLET, 0);
                    ReloadBioGauge.bioGaugeTime = 0;
                    BioGauge_Inven.Dequeue();

                    return 0;


                case 1:   // 노란색 게이지
                    ReloadBioGauge = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, BioBallReloadPosition, BioGaugeMODE.BULLET, 1);
                    ReloadBioGauge.bioGaugeTime = 0;
                    BioGauge_Inven.Dequeue();

                    return 1;

                case 2: // 초록색 게이지 
                    ReloadBioGauge = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, BioBallReloadPosition, BioGaugeMODE.BULLET, 2);
                    ReloadBioGauge.bioGaugeTime = 0;
                    BioGauge_Inven.Dequeue();

                    return 2;

                case 3:    //보라색 게이지
                    ReloadBioGauge = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, BioBallReloadPosition, BioGaugeMODE.BULLET, 3);
                    ReloadBioGauge.bioGaugeTime = 0;
                    BioGauge_Inven.Dequeue();

                    return 3;

                default:
                    return -1;
            }

        }



        public void OnDraw(GameTime gameTime)
        {

            Mouse_angle = Math.Atan2((double)(Mouse.GetState().Y - bodyViewPortPosition.Y), (double)(Mouse.GetState().X - (bodyViewPortPosition.X)));
            // Mouse_angle = Mouse_angle*(1.1f)*cCamera.Zoom;

            if (LinedrawMode_ForBuilding) // 클릭했을때만 
                m_SpriteBatch.Draw(UI_ReadytoBulid, new Rectangle((int)(bodyViewPortPosition.X), (int)(bodyViewPortPosition.Y), 300, 300), null, Color.White, (float)Math.PI * Laser_Spin / 40f, new Vector2(UI_ReadytoBulid.Width / 2, UI_ReadytoBulid.Height / 2), SpriteEffects.None, 0);

            m_SpriteBatch.Draw(Bulid_LineTexture, bodyViewPortPosition, null, Color.White, (float)Mouse_angle, Vector2.Zero, new Vector2(LaserLineLength, 1.5f), SpriteEffects.None, 1.0f);



            if (ReloadBioGauge != null) // 현재 선택되있는 게이지가 있다면,
            {
   
               m_SpriteBatch.Draw(ReloadBioGauge.m_Texture, cCamera.Transform(new Vector2(ReloadBioGauge.Bullet_Position.X 
                   - (ReloadBioGauge.m_Texture.Width / 2), ReloadBioGauge.Bullet_Position.Y - (ReloadBioGauge.m_Texture.Height / 2))), null, Color.White, 0f, Vector2.Zero,
                   1f, SpriteEffects.None, 0f);

          //   m_SpriteBatch.Draw(ReloadBioGauge.m_Texture, cCamera.Transform(new Vector2(ReloadBioGauge.Bullet_Position.X,
          //       ReloadBioGauge.Bullet_Position.Y) ), null, Color.White, 0f, Vector2.Zero,
          //  1f, SpriteEffects.None, 0f);
          //
          //
          //   m_SpriteBatch.Draw(ReloadBioGauge.m_Texture, ReloadBioGauge.Bullet_Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

           //     m_SpriteBatch.Draw(ReloadBioGauge.m_Texture, ReloadBioGauge.Bullet_Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            }


            Vibot_Laser_Element.m_Sphere.Center.X = bodyWorldPosition.X;
            Vibot_Laser_Element.m_Sphere.Center.Y = bodyWorldPosition.Y;

            m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, 0f, m_TextureOrigin, 1f, SpriteEffects.None, 0f);
            m_SpriteBatch.Draw(Vibot_Laser_Element.m_Texture, bodyViewPortPosition, null, Color.White, (float)Math.PI * Laser_Spin, m_TextureOrigin, (Vibot_Laser_Element.m_Sphere.Radius / (Vibot_Laser_Element.m_Texture.Width / 2)),
                        SpriteEffects.None, 0);


            Laser_Spin += 0.7f;

            if (Dashon)
                m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition, null, Color.White, 0f, m_TextureOrigin, 1f, SpriteEffects.None, 0f);

            //Debug.WriteLine(this.body.LinearVelocity);

            //대쉬 블러 그리기
            //대기
            if (!Dashon)
            {
                float x = this.body.LinearVelocity.X;
                float y = this.body.LinearVelocity.Y;
                const int VIBOTSIZE = 40;
                //m_SpriteBatch.DrawString(BioGauge_Font, "Not DashOn", new Vector2(580, 200), Color.LightYellow);
                m_SpriteBatch.Draw(DashEffect, new Vector2(bodyViewPortPosition.X - (x * 4) - VIBOTSIZE, bodyViewPortPosition.Y - (y * 4) - VIBOTSIZE), null, Color.White * 0.5f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1f);
                m_SpriteBatch.Draw(DashEffect, new Vector2(bodyViewPortPosition.X - (x * 6) - VIBOTSIZE, bodyViewPortPosition.Y - (y * 6) - VIBOTSIZE), null, Color.White * 0.3f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1f);
                m_SpriteBatch.Draw(DashEffect, new Vector2(bodyViewPortPosition.X - (x * 8) - VIBOTSIZE, bodyViewPortPosition.Y - (y * 8) - VIBOTSIZE), null, Color.White * 0.1f, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1f);

            }

            //주변을 도는 바이오 게이지 그리기
            foreach (BioGauge IteminInven in BioGauge_Inven)            //    if (IteminInven != null)
                IteminInven.OnDraw(gameTime);

                m_SpriteBatch.DrawString(GameMsgFont, (WhiteGauge_Value * 5).ToString("##") + "%", new Vector2(420, 680), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            

            int degree = 0;
            List<double> GaugePosition_tempList = new List<double>();  // 곰동님 코드 바이오게이지 간의 좌표 충돌 예방

            foreach (BioGauge IteminInven in BioGauge_Inven)
                GaugePosition_tempList.Add(IteminInven.Gauge_Spin_OnVibot);

            while (true)
            {
                bool result = false;
                for (int i = 0; i < GaugePosition_tempList.Count; i++)
                {
                    if (degree >= GaugePosition_tempList[i] - i)
                    {
                        if (degree <= GaugePosition_tempList[i] + i)
                        {
                            result = true;
                        }
                        break;
                    }
                    else if (degree == GaugePosition_tempList[i])
                    {
                        result = true;
                        break;
                    }
                }

                if (result)
                {
                    degree = Rand.Next(0, 360);
                    break;
                }
                else
                    break;
            }


        }



        public int ItemPickup_HandleMessage(int grade)
        {

            BioGauge PlusItem;
            switch (grade)
            {

                case 1:
                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 1);
                    BioGauge_Inven.Enqueue(PlusItem);
                    return 1;

                case 2:

                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 2);
                    BioGauge_Inven.Enqueue(PlusItem);
                    return 2;


                case 3:
                    PlusItem = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 3);
                    BioGauge_Inven.Enqueue(PlusItem);
                    return 3;



                default:
                    return -1;

            }



        }

        public override void Destory()
        {

            world.RemoveBody(body);

            if (BioGauge_Inven.Count > 0)
                BioGauge_Inven.Clear();

            WhiteGauge_Value = 0f;
        }




    }
}
