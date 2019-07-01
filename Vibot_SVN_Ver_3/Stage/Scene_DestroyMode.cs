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
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Vibot.Actors;
using Vibot.Base;
using Vibot.Stuffs;

//게임진행
namespace Vibot.Stage
{

    public enum eStageState
    {
        STAGE_INTIALIZE,
        STAGE_PLAYING,
        STAGE_CLEAR,
        STAGE_FAIL,

    }

    class Scene_DestroyMode : IScene
    {
        public enum ePrologueEvent
        {
            FadeOut = 0,
            Beginning = 1, // 이동하고 그쪽으로 줌인 (줌 인/아웃  조금씩 하면서)
            Development1,
            Development2,
            Development3,
            Climax1, //  해당 지점으로 카메라 이동
            Climax2,
            Conclusion,
            GameStart

        }

        public eStageState StageState;
        public static int Current_stage = 1;
        const int LastStage = 7;

        public int Before_stage_index;


        ePrologueEvent NowPrologue;

        protected Body[] Map_outline_Body;
        private Texture2D ShadowMask;
        public Texture2D UI_StageClear_texture;
        public Texture2D UI_StageFail_texture;

        public Texture2D[] Display_Object_Postion;
        private Texture2D UI_Building;
        private Texture2D UI_Hexagon;
        private Texture2D UI_SelectGage;
        private Texture2D UI_Ending;

        public Texture2D Spwan_token;
        public Rectangle[] tocollisionrect;
        public Color[] BoxColor;
        float sendsecond = 0f;
        int temp_count = 0;
        bool Stage3TryFlag = false;
        
        List<string> WelcomeMsg = new List<string>(); // 그냥 시간별로 자동으로 넘어가는 메시지 
        List<string> InGameMsg = new List<string>(); // 키를 눌러야 다음 으로 넘어가는 메시지

        public float EventSecond;

        KeyboardState pre_keystate;
        KeyboardState oldKeyState;
        cCamera Camera;
        Actor_GameFieldManager GameFieldManager;
        Actor_StageBackGround StageBackGround;
        Actor_Vibot Vibot;
        Actor_Objects Objects;
        Actor_WhiteCellManager WhiteCells;
        Actor_UI UI;
        Actor_RedBlood RedBlood;
        Actor_SpawnPointManager Spawnpoints;
        Parasite Parasite_Object;


        public Scene_DestroyMode(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
        }

        public override void OnInitalize()
        {
            ShadowMask = m_ContentManager.Load<Texture2D>("Sprites\\UI\\shadow_UI");
            UI_StageClear_texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\StageClear");  //성공
            UI_StageFail_texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\StageFail");  // 스테이지 실패        
            UI_Building = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Building");
            UI_Hexagon = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Hexagon");
            UI_SelectGage = m_ContentManager.Load<Texture2D>("Sprites\\UI\\SelectGage");
            UI_Ending = m_ContentManager.Load<Texture2D>("Sprites\\MainMenu\\ending");

            UI_tooltip_Arrows = m_ContentManager.Load<Texture2D>("Sprites\\UI\\arrow");
            Spwan_token = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Pause_mask");
            SystemFont = m_ContentManager.Load<SpriteFont>("Fonts\\prologue");
            GameMsgFont = m_ContentManager.Load<SpriteFont>("Fonts\\GameFont");
            m_FadeTexture = new Texture2D(m_GraphicDevice, 1280, 720, false, SurfaceFormat.Color);

            Color[] pixels = new Color[1280 * 720];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color(Color.Black.ToVector3());

            m_FadeTexture.SetData(pixels);

            Map_outline_Body = new Body[4];



            StageBackGround = new Actor_StageBackGround(m_GraphicDevice, m_ContentManager, m_SpriteBatch);
            Objects = new Actor_Objects(m_GraphicDevice, m_ContentManager, m_SpriteBatch);
            UI = new Actors.Actor_UI(m_GraphicDevice, m_ContentManager, m_SpriteBatch);
            RedBlood = new Actors.Actor_RedBlood(m_GraphicDevice, m_ContentManager, m_SpriteBatch);
            WhiteCells = new Actor_WhiteCellManager(m_GraphicDevice, m_ContentManager, m_SpriteBatch, RedBlood);
            Spawnpoints = new Actors.Actor_SpawnPointManager(m_GraphicDevice, m_ContentManager, m_SpriteBatch, RedBlood);

            Vibot = new Actors.Actor_Vibot(m_GraphicDevice, m_ContentManager, m_SpriteBatch, WhiteCells);
            GameFieldManager = new Actor_GameFieldManager(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Vibot, RedBlood, Spawnpoints, Objects, WhiteCells);



            world.Gravity = Vector2.Zero;
            NowPrologue = ePrologueEvent.FadeOut;
            m_FadeState = eFADESTATE.FADE_OUT;
            m_FadeAlpha = 255;
        }



        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            KeyboardState key = Keyboard.GetState();

            switch (StageState)
            {
                case eStageState.STAGE_PLAYING:
                    world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));

                    if (RedBlood.Blood_Goalin)
                        StageState = eStageState.STAGE_CLEAR;

                    if (!oldKeyState.IsKeyDown(Keys.J) && key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.J))
                        NowPrologue++;
                    if (!oldKeyState.IsKeyDown(Keys.K) && key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.K))
                        NowPrologue--;
                    if (!oldKeyState.IsKeyDown(Keys.H) && key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H))
                        StageState = eStageState.STAGE_CLEAR;
                    else if (!oldKeyState.IsKeyDown(Keys.G) && key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G))
                        StageState = eStageState.STAGE_FAIL;

                    break;

                case eStageState.STAGE_CLEAR:
                    world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));


                    if (!oldKeyState.IsKeyDown(Keys.Space) && key.IsKeyDown(Keys.Space))
                    {
                        Destory();
                        Before_stage_index = Current_stage;
                        Current_stage++;

                        if(Current_stage <= LastStage )
                        StageState = eStageState.STAGE_INTIALIZE;
                        else
                            StageState = eStageState.STAGE_PLAYING;
                        break;
                    }
                    break;

                case eStageState.STAGE_FAIL:
                    world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));
                    if (!oldKeyState.IsKeyDown(Keys.Space) && key.IsKeyDown(Keys.Space))
                    {
                        Destory();
                        StageState = eStageState.STAGE_INTIALIZE;
                        break;
                    }
                    break;

                case eStageState.STAGE_INTIALIZE:
                    {

                        LoadMap("Content\\stages\\stage" + Current_stage.ToString() + ".xml");

                        if (Current_stage == 0) //Stage Level 0일때, Level전용 오브젝트 셋팅
                        {
                            BoxColor = new Color[3];
                            BoxColor[0] = Color.OrangeRed;
                            BoxColor[1] = Color.OrangeRed;
                            BoxColor[2] = Color.OrangeRed;
                        }
                        if (!(Current_stage == 1 || Current_stage == 2))
                            GameFieldManager.ItemDrop = true;


                        Map_outline_Body = new Body[4];
                        Map_outline_Body[0] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(StageBackGround.BackGroundRect.Width * 2),
                            ConvertUnits.ToSimUnits(30), 30f, ConvertUnits.ToSimUnits(new Vector2(0, 10))); // 상 

                        Map_outline_Body[1] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(StageBackGround.BackGroundRect.Width * 2), ConvertUnits.ToSimUnits(30), 30f,
                            ConvertUnits.ToSimUnits(new Vector2(10, StageBackGround.BackGroundRect.Height - 10)));   // 하 

                        Map_outline_Body[2] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(30), ConvertUnits.ToSimUnits(StageBackGround.BackGroundRect.Height * 2),
                            30f, ConvertUnits.ToSimUnits(new Vector2(0, 30)));     // 좌 

                        Map_outline_Body[3] = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(30), ConvertUnits.ToSimUnits(StageBackGround.BackGroundRect.Height * 2),
                            30f, ConvertUnits.ToSimUnits(new Vector2(StageBackGround.BackGroundRect.Width - 10, 30)));
                        // 우 

                        for (int i = 0; i < Map_outline_Body.Length; i++)
                        {
                            Map_outline_Body[i].BodyType = BodyType.Static;
                            Map_outline_Body[i].Restitution = 0.3f;
                            Map_outline_Body[i].CollisionCategories = Category.Cat31;
                            Map_outline_Body[i].CollidesWith = Category.Cat2 | ~Category.All;
                            //   | ~Category.Cat31 | ~Category.Cat1 | ~Category.Cat6 | ~Category.Cat10;
                        }

                        // BGM 추가해야함!                       
                        MediaPlayer.Play(m_BGM);
                        MediaPlayer.IsRepeating = true;
                        StageState = eStageState.STAGE_PLAYING;
                    }
                    break;

            }

            oldKeyState = key;
        }



        public override void OnDraw(GameTime gameTime)
        {
            m_GraphicDevice.Clear(Color.RosyBrown);

            switch (Current_stage)
            {
                case 0:
                    Stage_Stage0(gameTime);
                    break;

                case 1:
                    Stage_Stage1(gameTime);
                    break;

                case 2:
                    Stage_Stage2(gameTime);
                    break;

                case 3:
                    Stage_Stage3(gameTime);
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    Stage_Stage(gameTime);
                    break;

            default:
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(UI_Ending, Vector2.Zero, null, Color.White);
                    m_SpriteBatch.End();
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Escape) )
                    {
                        Current_stage = 0;
                        Scene_Menu.GameState = eGameState.MAINMENU;
                        StageState = eStageState.STAGE_INTIALIZE;
                    }
                        break;
            }

            if (StageState == eStageState.STAGE_INTIALIZE)
            {
                m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                m_SpriteBatch.DrawString(GameMsgFont, StageState.ToString() + "Loding.....", new Vector2(300, 300), Color.IndianRed);
                m_SpriteBatch.End();
            }

        }


        public override bool ShowGameMsg(SpriteFont SystemFont, float EventSecond, float MaxSecond, string Msg, Vector2 Position)
        {
            if (EventSecond < MaxSecond)
            {
                if (m_FadeAlpha < 250)
                    m_FadeAlpha += 3;
                //   SpriteBatch.End();
            }
            else if (EventSecond > MaxSecond)
            {
                //    SpriteBatch.DrawString(SystemFont, Msg, Position, new Color(m_FadeAlpha, m_FadeAlpha, m_FadeAlpha, m_FadeAlpha));  //와 스트링은 페이드 적용이 왜 안되나 미치겠네...     
                m_FadeAlpha -= 3;

                if (m_FadeAlpha <= 30)
                {
                    EventSecond = 0;
                    return true;
                }
            }
            m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            m_SpriteBatch.DrawString(SystemFont, Msg, Position, new Color(m_FadeAlpha, m_FadeAlpha, m_FadeAlpha, m_FadeAlpha));   // alpha 값이 높을수록 보인다
            m_SpriteBatch.End();

            return false;
        }


        public void DrawClearOrFail()
        {
            if (StageState == eStageState.STAGE_CLEAR)
            {
                m_SpriteBatch.Draw(UI_StageClear_texture, Vector2.Zero, Color.White);
                m_SpriteBatch.DrawString(SystemFont, "You can continue to Next Stage \n Press the", new Vector2(550, 390), Color.White);
                m_SpriteBatch.DrawString(SystemFont, "[Space]", new Vector2(720, 430), Color.LightSeaGreen);

            }
            if (StageState == eStageState.STAGE_FAIL)
            {
                m_SpriteBatch.Draw(UI_StageFail_texture, Vector2.Zero, Color.White);
                m_SpriteBatch.DrawString(SystemFont, "All of RedBlood Are contaminated", new Vector2(550, 390), Color.PaleVioletRed);
                m_SpriteBatch.DrawString(SystemFont, "Press the [Space] to retry", new Vector2(600, 430), Color.FloralWhite);
            }
        }


        private bool LoadMap(string name) //XML 파일을 읽는 부분
        {
            XmlDocument MapFile = new XmlDocument();
            MapFile.Load(name);
            XmlElement Root = MapFile.DocumentElement;
            XmlNodeList ChildNodes = Root.ChildNodes;


            foreach (XmlNode ChildNode in ChildNodes)
            {

                switch (ChildNode.Name)
                {

                    case "BackgroundMusic":
                        {
                            string path = "Audio\\BGM\\" + ChildNode.InnerText.ToString();
                            m_BGM = m_ContentManager.Load<Song>(path);
                        }
                        break;
                    case "EventScript":
                        {
                            XmlNodeList InnerChildNodes = ChildNode.ChildNodes;

                            foreach (XmlNode InnerChildNode in InnerChildNodes)
                            {

                                if (InnerChildNode.Name == "WelcomeMsg")
                                {
                                    string tempScript = null;
                                    string[] result = InnerChildNode.InnerText.Split(';');
                                    for (int i = 0; i < result.Length; i++)
                                    {
                                        tempScript += result[i];
                                        tempScript += '\n';
                                    }

                                    WelcomeMsg.Add(tempScript);

                                }
                                if (InnerChildNode.Name == "InGameMsg")
                                {

                                    string tempScript = null;
                                    string[] result = InnerChildNode.InnerText.Split(';');
                                    for (int i = 0; i < result.Length; i++)
                                    {
                                        tempScript += result[i];
                                        tempScript += '\n';
                                    }

                                    InGameMsg.Add(tempScript);
                                }
                            }
                        }
                        break;

                    case "MapInfo":
                        StageBackGround.LoadDataFromMap(ChildNode);

                        Camera = new cCamera(new Vector2(StageBackGround.BackGroundRect.Width,
                                                StageBackGround.BackGroundRect.Height)); // 카메라 맵 초기화
                        break;

                    case "GameField":
                        GameFieldManager.LoadDataFromMap(ChildNode);
                        break;

                    case "RedBlood":
                        RedBlood.LoadDataFromMap(ChildNode);
                        break;

                    case "Vibot":
                        Vibot.LoadDataFromMap(ChildNode);
                        break;

                    case "ObjectPoints":
                        Objects.LoadDataFromMap(ChildNode);
                        break;

                    case "WhiteCellPoints":
                        WhiteCells.LoadDataFromMap(ChildNode);

                        break;
                    case "SpawnPoints":
                        Spawnpoints.LoadDataFromMap(ChildNode);
                        break;
                }
            }
            if (Current_stage == LastStage)
                Parasite_Object = new Parasite(m_GraphicDevice, m_ContentManager, m_SpriteBatch, new Vector2(1000, 1500), 500f); // 이게 기생충 ;;

            return true;
        }


        #region Stage0
        private void Stage_Stage0(GameTime gameTime)
        {
            Display_Object_Postion = new Texture2D[3];

            for (int i = 0; i < Display_Object_Postion.Length; i++)
                Display_Object_Postion[i] = m_ContentManager.Load<Texture2D>("Sprites\\UI\\hexagon");

            tocollisionrect = new Rectangle[3];
            tocollisionrect[0] = new Rectangle(700, 450, 100, 100);
            tocollisionrect[1] = new Rectangle(450, 700, 100, 100);
            tocollisionrect[2] = new Rectangle(850, 700, 100, 100);

            if (Vibot.WhiteGauge_Value > 0)
                Vibot.WhiteGauge_Value = 0;

            switch (NowPrologue)
            {
                case ePrologueEvent.FadeOut:
                    if (Fade_INorOUT())
                    {
                        NowPrologue = ePrologueEvent.Beginning;
                        Camera.InitalizeCamera(new Vector2(100, 150));
                        GameFieldManager.ItemDrop = false;
                    }
                    break;

                case ePrologueEvent.Beginning:
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);

                    m_SpriteBatch.End();

                    if (ShowGameMsg(SystemFont, EventSecond, 2.1f, WelcomeMsg[0], cCamera.ScreenCenter))
                    {
                        // Camera.Update(new Vector2(150, 150));
                        EventSecond = 0;
                        m_FadeAlpha = 200;
                        NowPrologue = ePrologueEvent.Development1;

                    }

                    break;


                case ePrologueEvent.Development1:  //Vibot  위치로 신속히 이동

                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(2.3f));
                    StageBackGround.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    m_SpriteBatch.End();


                    if (ShowGameMsg(SystemFont, EventSecond, 2.1f, WelcomeMsg[1], cCamera.ScreenCenter))
                    {
                        EventSecond = 0;
                        m_FadeAlpha = 0;
                        NowPrologue = ePrologueEvent.Development2;
                    }

                    break;

                case ePrologueEvent.Development2: // 소개
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);

                    Vibot.OnUpdate(gameTime);
                    Vibot.OnDraw(gameTime);


                   if (EventSecond > 1.0f && EventSecond < 1.5f)
                       m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyWorldPosition.X - 25, (int)Vibot.bodyWorldPosition.Y - 80, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);   //상
                 
                   if (EventSecond > 1.5f && EventSecond < 1.9f)
                       m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyWorldPosition.X + 80, (int)Vibot.bodyWorldPosition.Y - 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //우
                 
                   if (EventSecond > 2.5f && EventSecond < 3.0f)
                       m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyWorldPosition.X - 25, (int)Vibot.bodyWorldPosition.Y + 50, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0); // 하
                 
                   if (EventSecond > 3.0f && EventSecond < 4.0f)
                       m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyWorldPosition.X - 80, (int)Vibot.bodyWorldPosition.Y + 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)-Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //좌 
                 
                    m_SpriteBatch.End();


                    Camera.Update((Vibot.bodyWorldPosition) - cCamera.ScreenCenter);


                    if (ShowGameMsg(SystemFont, EventSecond, 5.0f, WelcomeMsg[2], cCamera.ScreenCenter))
                    {
                        NowPrologue = ePrologueEvent.Development3;
                        EventSecond = 0;
                    }
                    break;

                case ePrologueEvent.Development3:  // Virus(object) 를 격퇴햐야된다
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);


                    Vibot.OnDraw(gameTime);
                    Vibot.OnUpdate(gameTime);
                    Objects.OnDraw(gameTime);

                    if ((int)(EventSecond % 2) == 0)
                    {
                        m_SpriteBatch.Draw(Display_Object_Postion[0], new Vector2(700, 450) - cCamera.CameraPosition, null, BoxColor[0], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.Draw(Display_Object_Postion[1], new Vector2(450, 700) - cCamera.CameraPosition, null, BoxColor[1], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.Draw(Display_Object_Postion[2], new Vector2(850, 700) - cCamera.CameraPosition, null, BoxColor[2], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }

                    m_SpriteBatch.End();

                    if (EventSecond > 1.0f && EventSecond < 3.5f)
                        Camera.Update(new Vector2(150, 200)); // followingCamera  0 을 향해간다 

                    if (ShowGameMsg(SystemFont, EventSecond, 5.0f, WelcomeMsg[3], new Vector2(50, 500)))
                    {

                        NowPrologue = ePrologueEvent.GameStart;
                        EventSecond = 0;
                        m_FadeAlpha = 0;
                    }

                    break;

                case ePrologueEvent.GameStart:



                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    if (Scene_Menu.GameState == eGameState.GAME_PLAYING) // 게임 Playing 중일때만 Update 한다 
                    {
                        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Vibot.OnUpdate(gameTime);
                        Objects.OnUpdate(gameTime);
                    }


                    StageBackGround.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);
                    UI.OnDraw(gameTime, RedBlood, Vibot);

                    Camera.Update(Vibot.bodyWorldPosition - cCamera.ScreenCenter);


                    DrawClearOrFail();

                    if (EventSecond > 10) // 메인 목표 메시지 
                        m_SpriteBatch.DrawString(GameMsgFont, "Move in all of pentagons once \n", new Vector2(30, 70), Color.MediumPurple, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

                    m_SpriteBatch.Draw(Display_Object_Postion[0], cCamera.Transform(new Vector2(700, 450)), null, BoxColor[0], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    m_SpriteBatch.Draw(Display_Object_Postion[1], cCamera.Transform(new Vector2(450, 700)), null, BoxColor[1], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    m_SpriteBatch.Draw(Display_Object_Postion[2], cCamera.Transform(new Vector2(850, 700)), null, BoxColor[2], 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);

                    if (EventSecond > 10)
                    {
                        if ((int)(EventSecond % 4) == 0)
                            m_SpriteBatch.DrawString(SystemFont, InGameMsg[2], new Vector2(200, 520), new Color(255, 255, 255, Parameter_Alpha < 255 ? Parameter_Alpha + 5 : 255));
                        else
                            m_SpriteBatch.DrawString(SystemFont, InGameMsg[1], new Vector2(200, 520), new Color(255, 255, 255, Parameter_Alpha < 255 ? Parameter_Alpha + 5 : 255));

                    }
                    m_SpriteBatch.End();

                    if (EventSecond > 5 && EventSecond < 10) //&& EventSecond < 10
                    {
                        if (ShowGameMsg(SystemFont, EventSecond, 11, InGameMsg[0], new Vector2(300, 520)))
                            m_FadeAlpha = 0;
                        if (EventSecond > 8 && EventSecond < 10)
                            m_FadeAlpha -= 3;
                    }


                    if (BoxColor[0] == Color.LightGreen && BoxColor[1] == Color.LightGreen && BoxColor[2] == Color.LightGreen)
                        StageState = eStageState.STAGE_CLEAR;
                    else
                    {

                        if (tocollisionrect[0].Contains(new Rectangle((int)Vibot.bodyWorldPosition.X,
                              (int)Vibot.bodyWorldPosition.Y, 10, 10))
                            )
                            BoxColor[0] = Color.LightGreen;

                        if (tocollisionrect[1].Contains(new Rectangle((int)Vibot.bodyWorldPosition.X,
                               (int)Vibot.bodyWorldPosition.Y, 10, 10))
                             )
                            BoxColor[1] = Color.LightGreen;

                        if (tocollisionrect[2].Contains(new Rectangle((int)Vibot.bodyWorldPosition.X,
                                (int)Vibot.bodyWorldPosition.Y, 10, 10))
                              )
                            BoxColor[2] = Color.LightGreen;
                    }
                    break;

            }

            switch (NowPrologue)
            {
                case ePrologueEvent.Beginning:
                case ePrologueEvent.Development1:
                case ePrologueEvent.Development2:
                case ePrologueEvent.Development3:
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(ShadowMask, new Vector2(0, 0), Color.White);   // 게임 UI 표기
                    m_SpriteBatch.End();
                    break;

            }



        }
        #endregion

        #region Stage1
        bool ohshit = false;


        private void Stage_Stage1(GameTime gameTime)
        {

            if (Vibot.WhiteGauge_Value > 0)
                Vibot.WhiteGauge_Value = 0;


            switch (NowPrologue)
            {
                case ePrologueEvent.FadeOut:
                    if (Fade_INorOUT())
                    {
                        NowPrologue = ePrologueEvent.Beginning;
                        Camera.InitalizeCamera(new Vector2(50, 100));
                    }

                    break;

                case ePrologueEvent.Beginning:
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);

                    m_SpriteBatch.End();

                    if (ShowGameMsg(SystemFont, EventSecond, 2.1f, WelcomeMsg[0], cCamera.ScreenCenter))
                    {
                        EventSecond = 0;
                        m_FadeAlpha = 200;
                        NowPrologue = ePrologueEvent.Development1;
                    }

                    break;


                case ePrologueEvent.Development1:  // Red Blood 소개란 
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    Camera.Update(RedBlood.Blood_Core_Position - cCamera.ScreenCenter);

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(cCamera.Zoom));
                    StageBackGround.OnDraw(gameTime);

                    RedBlood.OnUpdate(gameTime);
                    RedBlood.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);

                    m_SpriteBatch.End();


                    if (EventSecond > 1.0 && EventSecond < 2)
                        cCamera.Zoom += 0.003f;

                    if (ShowGameMsg(SystemFont, EventSecond, 5.0f, WelcomeMsg[1], cCamera.ScreenCenter))
                    {
                        cCamera.Zoom = 1.0f;
                        NowPrologue = ePrologueEvent.Development2;
                        EventSecond = 0;
                        m_FadeAlpha = 0;
                    }

                    break;
                case ePrologueEvent.Development2:  // SecurePoint 소개 
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (EventSecond > 1.0 && EventSecond < 2.4)
                        Camera.Update(new Vector2(50, 0));

                    RedBlood.OnUpdate(gameTime);
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);

                    Vibot.OnDraw(gameTime);
                    RedBlood.OnDraw(gameTime);


                    Texture2D tempSecurePoint = m_ContentManager.Load<Texture2D>("Sprites\\MainMenu\\SecurePointBefore");

                    if ((int)(EventSecond % 2) == 0)
                        m_SpriteBatch.Draw(tempSecurePoint,
                          cCamera.Transform(new Vector2(GameFieldManager.SecurePoint_List[0].m_Rect.X, GameFieldManager.SecurePoint_List[0].m_Rect.Y)),
                            Color.White);
                    //  m_SpriteBatch.Draw(tempSecurePoint,
                    //      cCamera.Transform(new Rectangle(GameFieldManager.SecurePoint_List[0].m_Rect.X, GameFieldManager.SecurePoint_List[0].m_Rect.Y, 180, 180)),
                    //      new Rectangle(0, 0, 64, 64), Color.White);



                    m_SpriteBatch.End();


                    if (ShowGameMsg(SystemFont, EventSecond, 4.0f, WelcomeMsg[2], new Vector2(150, 530)))
                    {
                        NowPrologue = ePrologueEvent.Development3;
                        EventSecond = 0;
                        m_FadeAlpha = 0;
                    }

                    break;


                case ePrologueEvent.Development3:  // GoalZonE 소개 
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (EventSecond > 1.0 && EventSecond < 2.4)
                        Camera.Update(new Vector2(0, 200));

                    RedBlood.OnUpdate(gameTime);
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);
                    m_SpriteBatch.Draw(m_ContentManager.Load<Texture2D>("Sprites\\MainMenu\\SecurePointBefore"),
                           cCamera.Transform(new Vector2(GameFieldManager.SecurePoint_List[0].m_Rect.X, GameFieldManager.SecurePoint_List[0].m_Rect.Y)),
                            Color.White);
                    Vibot.OnDraw(gameTime);
                    RedBlood.OnDraw(gameTime);

                    if ((int)(EventSecond % 2) == 0)
                        m_SpriteBatch.Draw(StageBackGround.Goal_Zone_Texture,
                            cCamera.Transform(Actor_StageBackGround.Goal_Zone),
                          Color.Red);

                    m_SpriteBatch.End();


                    if (ShowGameMsg(SystemFont, EventSecond, 3.5f, WelcomeMsg[3], new Vector2(150, 530)))
                    {
                        NowPrologue = ePrologueEvent.GameStart;
                        EventSecond = 0;
                        m_FadeAlpha = 0;
                    }

                    break;

                case ePrologueEvent.GameStart:

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(cCamera.Zoom));

                    if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
                    {
                        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Spawnpoints.OnUpdate(gameTime);
                        GameFieldManager.OnUpdate(gameTime);
                        RedBlood.OnUpdate(gameTime);
                        Vibot.OnUpdate(gameTime);
                        Objects.OnUpdate(gameTime);
                    }

                    StageBackGround.OnDraw(gameTime);
                    Spawnpoints.OnDraw(gameTime, RedBlood);
                    GameFieldManager.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    RedBlood.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);

                    Camera.Update(Vibot.bodyWorldPosition - cCamera.ScreenCenter);

                    if (!RedBlood.Blood_Goalin && RedBlood.Life_Count <= 0)
                        StageState = eStageState.STAGE_FAIL;

                    KeyboardState keystate = Keyboard.GetState();

                    if (GameFieldManager.SecurePoint_List[0].SecureState == SecureState.SECURING && temp_count == 0)     // 1 STEP
                    {
                        Scene_Menu.GameState = eGameState.GAME_PAUSE; // Game temp pause
                        m_SpriteBatch.Draw(Spwan_token, cCamera.Transform(new Rectangle(500, 190, 200, 200)), null, Color.Green, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[0], cCamera.Transform(new Vector2(480, 360)), Color.OrangeRed, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", cCamera.Transform(new Vector2(480, 400)), Color.White, 0.1f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                            temp_count = 1;


                    }

                    else if (temp_count == 1)     // 3 STEP
                    {
                        Camera.Update(new Vector2(50, 80));
                        Scene_Menu.GameState = eGameState.GAME_PAUSE; // Game temp pause

                        m_SpriteBatch.Draw(UI_Hexagon, cCamera.Transform(new Vector2(540, 550)), null, Color.Red, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[1], cCamera.Transform(new Vector2(320, 360)), Color.BlueViolet, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", cCamera.Transform(new Vector2(330, 420)), Color.White, 0.1f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                            temp_count = 2;


                    }
                    else if (temp_count == 2)
                    {

                        Scene_Menu.GameState = eGameState.GAME_PLAYING;
                        m_SpriteBatch.Draw(UI_Hexagon, cCamera.Transform(new Vector2(560, 450)), null, Color.Orange, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[2], cCamera.Transform(new Vector2(320, 360)), Color.BlueViolet, 0.07f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);

                        if (EventSecond > 17)
                            temp_count = 3;
                    }

                    else if (temp_count == 3 && GameFieldManager.SecurePoint_List[0].SecureState == SecureState.SECURE)
                    {
                        Camera.Update(new Vector2(100, 0));
                        // EventSecond -= (float)gameTime.ElapsedGameTime.TotalSeconds; // stop flow eventSecond
                        Scene_Menu.GameState = eGameState.GAME_PAUSE; // Game temp pause
                        m_SpriteBatch.Draw(UI_Hexagon, cCamera.Transform(new Vector2(910, 200)), null, Color.Orange, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[3], cCamera.Transform(new Vector2(870, 340)), Color.BlueViolet, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", cCamera.Transform(new Vector2(820, 390)), Color.White, 0.1f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            temp_count = 4;
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                        }
                    }
                    else if (temp_count == 4)
                    {

                        m_SpriteBatch.DrawString(GameMsgFont, "Protect RedBlood during that go to G-ZONE\n", new Vector2(40, 70), Color.BlueViolet, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);

                        //    if (!new Rectangle(880, 200, 150, 150).Contains((int)Vibot.bodyWorldPosition.X, (int)Vibot.bodyWorldPosition.Y))
                        m_SpriteBatch.Draw(UI_Hexagon, cCamera.Transform(new Vector2(910, 180)), null, Color.Orange, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);


                    }




                    if (RedBlood.Damaged != Vector2.Zero)
                        ohshit = true;
                    if (ohshit)
                        sendsecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (sendsecond > 0 && sendsecond < 3)
                    {
                        m_SpriteBatch.DrawString(SystemFont, "RedBloods under Acttack!\n Go to Redblood around for Protect", new Vector2(220, 520), new Color(255, 105, 105, (byte)Parameter_Alpha > 0 ? Parameter_Alpha - 5 : 0));
                        if (sendsecond >= 1.95f)
                        {
                            ohshit = false;
                            Parameter_Alpha = 255;
                            sendsecond = 0f;
                        }
                    }

                    m_SpriteBatch.End();


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    UI.OnDraw(gameTime, RedBlood, Vibot);
                    DrawClearOrFail();
                    m_SpriteBatch.End();

                    pre_keystate = keystate;


                    break;
            }


            switch (NowPrologue)
            {
                case ePrologueEvent.Beginning:
                case ePrologueEvent.Development1:
                case ePrologueEvent.Development2:
                case ePrologueEvent.Development3:
                case ePrologueEvent.Climax1:
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(ShadowMask, new Vector2(0, 0), Color.White);   // 게임 UI 표기
                    m_SpriteBatch.End();
                    break;

            }
        }
        #endregion

        #region Stage2
        private void Stage_Stage2(GameTime gameTime)
        {
            switch (NowPrologue)
            {
                case ePrologueEvent.FadeOut:
                    if (Fade_INorOUT())
                    {
                        NowPrologue = ePrologueEvent.Beginning;
                        Vibot.body.Position = ConvertUnits.ToSimUnits(new Vector2(1000, 448));
                        Vibot.WhiteGaugeGrow = false;
                    }
                    break;


                case ePrologueEvent.Beginning: //대시소개
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);

                    Vibot.OnDraw(gameTime);
                    Vibot.OnUpdate(gameTime);

                    if (EventSecond > 1.0f && EventSecond < 1.5f)
                    {
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y - 80, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);   //상
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y - 130, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);   //상
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y - 210, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);   //상
                    }
                    if (EventSecond > 1.5f && EventSecond < 2.0f)
                    {
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X + 80, (int)Vibot.bodyViewPortPosition.Y - 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //우
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X + 130, (int)Vibot.bodyViewPortPosition.Y - 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //우
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X + 210, (int)Vibot.bodyViewPortPosition.Y - 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //우
                    }
                    if (EventSecond > 2.5f && EventSecond < 3.0f)
                    {
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y + 50, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0); // 하
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y + 100, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0); // 하
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 25, (int)Vibot.bodyViewPortPosition.Y + 180, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0); // 하
                    }
                    if (EventSecond > 3.0f && EventSecond < 4.0f)
                    {
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 80, (int)Vibot.bodyViewPortPosition.Y + 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)-Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //좌 
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 130, (int)Vibot.bodyViewPortPosition.Y + 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)-Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //좌 
                        m_SpriteBatch.Draw(UI_tooltip_Arrows, new Rectangle((int)Vibot.bodyViewPortPosition.X - 210, (int)Vibot.bodyViewPortPosition.Y + 20, UI_tooltip_Arrows.Width, UI_tooltip_Arrows.Height), null, Color.White, (float)-Math.PI / 2, Vector2.Zero, SpriteEffects.None, 0); //좌 
                    }

                    m_SpriteBatch.End();


                    Camera.Update(cCamera.Transform(Vibot.bodyWorldPosition) - cCamera.ScreenCenter);

                    if (ShowGameMsg(SystemFont, EventSecond, 5.0f, WelcomeMsg[0], new Vector2(400, 530)))
                    {
                        NowPrologue = ePrologueEvent.GameStart;
                        EventSecond = 0;
                        Vibot.body.Position = ConvertUnits.ToSimUnits(new Vector2(158, 448));
                    }

                    break;

                case ePrologueEvent.GameStart:

                    if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
                        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    KeyboardState keystate = Keyboard.GetState();

                    if (!RedBlood.Blood_Goalin && RedBlood.Life_Count <= 0)
                        StageState = eStageState.STAGE_FAIL;


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(cCamera.Zoom));
                    StageBackGround.OnDraw(gameTime);

                    Spawnpoints.OnDraw(gameTime, RedBlood);
                    GameFieldManager.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    RedBlood.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);
                    WhiteCells.OnDraw(gameTime);


                    if (EventSecond > 1 && temp_count == 0)
                    {

                        Scene_Menu.GameState = eGameState.GAME_PAUSE; // Game temp pause   

                        Camera.Update(new Vector2(150, 0));

                        Rectangle DashArea = new Rectangle(300, 190, 300, 400);

                        m_SpriteBatch.Draw(Spwan_token, cCamera.Transform(DashArea), null, Color.Green, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[0], cCamera.Transform(new Vector2(480, 400)), Color.OrangeRed, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", cCamera.Transform(new Vector2(480, 440)), Color.White, 0.1f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP;
                            temp_count = 1;                
                        }
                    }
                    else if (temp_count == 1)    // 2 STEP
                    {
           
                        Vibot.OnUpdate(gameTime);
                        RedBlood.OnUpdate(gameTime);
                        Objects.OnUpdate(gameTime);

                        Rectangle DashArea = new Rectangle(300, 190, 300, 400);
                        m_SpriteBatch.Draw(Spwan_token, cCamera.Transform(DashArea), null, Color.Green, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[1], cCamera.Transform(new Vector2(480, 360)), Color.OrangeRed, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);


                        if (DashArea.Contains(new Rectangle((int)Vibot.bodyViewPortPosition.X, (int)Vibot.bodyViewPortPosition.Y, Vibot.m_Texture.Width, Vibot.m_Texture.Height)) &&
                            !Vibot.Dashon)
                        {
                            temp_count = 2;
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_MOVE;
                            Vibot.WhiteGaugeGrow = true;
                        }
                    }
                    else if (temp_count == 2)     // 3 STEP
                    {

                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[2], cCamera.Transform(new Vector2(480, 360)), Color.OrangeRed, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                        GameFieldManager.OnUpdate(gameTime);
                        Vibot.OnUpdate(gameTime);
                        RedBlood.OnUpdate(gameTime);
                        Objects.OnUpdate(gameTime);

                        if (Vibot.BioGauge_Inven.Count > 0)
                        {
                            Scene_Menu.GameState = eGameState.GAME_PAUSE;
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP;
                            temp_count = 3;

                        }
                    }
                    else if (temp_count == 3) // W게이지 가졌꾸나 w게이지는 이런거임
                    {
                        // UI mark 
                        m_SpriteBatch.Draw(Spwan_token, Vibot.BioGauge_Inven.ElementAt<BioGauge>(0).Gauge_Rect, null, Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.Draw(Spwan_token, new Rectangle(490, 660, 70, 60), null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[3], new Vector2(320, 470), Color.Wheat, 0.1f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", new Vector2(290, 540), Color.White, 0.08f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                            temp_count = 4;
           
                        }
                    }
                    else if (temp_count == 4) // 자 이제 저곳에 클릭을 해봐 
                    {
                   
                       GameFieldManager.OnUpdate(gameTime);
                       Vibot.OnUpdate(gameTime);
                       RedBlood.OnUpdate(gameTime);
                       WhiteCells.OnUpdate(gameTime);
                    //    Objects.OnUpdate(gameTime);

                       // UI mark 
                       m_SpriteBatch.Draw(UI_SelectGage, Vibot.bodyViewPortPosition + new Vector2(-100, 10), null, Color.OrangeRed, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                       m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[4], Vibot.bodyViewPortPosition + new Vector2(-130 , 40), Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                    
                        if (WhiteCells.WhiteCell_List.Count > 0)
                        {
                            Scene_Menu.GameState = eGameState.GAME_PAUSE;              
                            temp_count = 5;
                        }
  
                          
                 
                    }
                    else if (temp_count == 5)
                    {
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[5], WhiteCells.WhiteCell_List[0].bodyViewPortPosition, Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", WhiteCells.WhiteCell_List[0].bodyViewPortPosition + new Vector2(0, 50), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_MOVE;
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                            temp_count = 6;
                        }
                      
                    }

                    else if (temp_count == 6)
                    {
                    //    m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[6], cCamera.Transform(RedBlood.Blood_Core_Position + new Vector2(0, 150)), Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Laser line Select:\n Select cancel :\nMove Object!:", cCamera.Transform(RedBlood.Blood_Core_Position + new Vector2(-50, 150)), Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Click Position  \n Right Mouse Click \n Select and Drag:", cCamera.Transform(RedBlood.Blood_Core_Position + new Vector2(100, 150)), Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
    

                     //   m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[6], WhiteCells.WhiteCell_List[0].bodyViewPortPosition, Color.BlueViolet, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                        Spawnpoints.OnUpdate(gameTime);
                        GameFieldManager.OnUpdate(gameTime);
                        Vibot.OnUpdate(gameTime);
                        RedBlood.OnUpdate(gameTime);
                        Objects.OnUpdate(gameTime);
                        WhiteCells.OnUpdate(gameTime);

                    }
                    m_SpriteBatch.End();

                    Camera.Update((Vibot.bodyWorldPosition) - cCamera.ScreenCenter); // followingCamera  0 을 향해간다 


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    UI.OnDraw(gameTime, RedBlood, Vibot);
                    DrawClearOrFail();
                    m_SpriteBatch.End();
                    pre_keystate = keystate;



                    break;


            }

            switch (NowPrologue)
            {
                case ePrologueEvent.Beginning:
                case ePrologueEvent.Development1:
                case ePrologueEvent.Development2:
                case ePrologueEvent.Development3:
                case ePrologueEvent.Climax1:


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(ShadowMask, new Vector2(0, 0), Color.White);   // 게임 UI 표기
                    m_SpriteBatch.End();
                    break;

            }

        }
        #endregion


        #region Stage3

        BioGauge biogauge_Temp = null;  // 임시아이템 
        private void Stage_Stage3(GameTime gameTime)
        {

            switch (NowPrologue)
            {
                case ePrologueEvent.FadeOut:
                    if (Fade_INorOUT())
                        NowPrologue = ePrologueEvent.Beginning;
                    
                    Camera.InitalizeCamera(new Vector2(50, 50));
                    break;

                case ePrologueEvent.Beginning:
                    EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    StageBackGround.OnDraw(gameTime);
                    Spawnpoints.OnDraw(gameTime, RedBlood);
                    GameFieldManager.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);
                    WhiteCells.OnDraw(gameTime);
                    m_SpriteBatch.End();
                 
                    if (ShowGameMsg(SystemFont, EventSecond, 2.1f, WelcomeMsg[0], cCamera.ScreenCenter))
                    {
                        EventSecond = 0;
                        m_FadeAlpha = 200;
                        NowPrologue = ePrologueEvent.GameStart;
                        biogauge_Temp = new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, new Vector2(490, 160), BioGaugeMODE.ONVIBOT, 2);
                        Vibot.WhiteGaugeGrow = false;
                    }

                    break;


                case ePrologueEvent.GameStart:
                //    if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
                //        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    KeyboardState keystate = Keyboard.GetState();

                    if (!RedBlood.Blood_Goalin && RedBlood.Life_Count <= 0)
                        StageState = eStageState.STAGE_FAIL;

                    if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
                    {
                        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;

                        Objects.OnUpdate(gameTime);
                        Vibot.OnUpdate(gameTime);
                        GameFieldManager.OnUpdate(gameTime);
                        WhiteCells.OnUpdate(gameTime);
 
                    }


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(cCamera.Zoom));
                    StageBackGround.OnDraw(gameTime);
                    Spawnpoints.OnDraw(gameTime, RedBlood);
                    GameFieldManager.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);
                    WhiteCells.OnDraw(gameTime);



                    if (EventSecond > 1 && temp_count == 0)
                    {

                        Scene_Menu.GameState = eGameState.GAME_PAUSE; // Game temp pause   


                        m_SpriteBatch.Draw(biogauge_Temp.m_Texture, biogauge_Temp.position, Color.White);

                        m_SpriteBatch.Draw(Spwan_token, cCamera.Transform(new Rectangle(480, 150, 50, 50)), null, Color.Green, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[0], cCamera.Transform(new Vector2(400, 190)), Color.LawnGreen, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", cCamera.Transform(new Vector2(430, 250)), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            temp_count = 1;
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP;
                        }


                    }
                    else if (temp_count == 1)
                    {
                        m_SpriteBatch.Draw(biogauge_Temp.m_Texture, cCamera.Transform(biogauge_Temp.position), Color.White);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[1], cCamera.Transform(new Vector2(480, 170)), Color.OrangeRed, 0.1f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);

                        if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(biogauge_Temp.position.X, biogauge_Temp.position.Y, 0), biogauge_Temp.m_Texture.Width / 2)))
                        {
                            Vibot.BioGauge_Inven.Enqueue(new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Vibot.bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 2));
                            biogauge_Temp = null;
                            temp_count = 2;
                            Scene_Menu.GameState = eGameState.GAME_PAUSE;
                        }
                    }
                    else if (temp_count == 2)
                    {
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[2], new Vector2(400, 250), Color.OrangeRed, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Continue to 'SPACE' KEY", new Vector2(400, 300), Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        // biogauge 설명 여기에 게이지가 있게 된다 
                        if (keystate.IsKeyDown(Keys.Space) && !pre_keystate.IsKeyDown(Keys.Space))
                        {
                            Scene_Menu.GameState = eGameState.GAME_PLAYING;
                            temp_count = 3;
                     
                        }
                    }
                    else if (temp_count == 3)
                    {
                        if(Stage3TryFlag == true)
                        m_SpriteBatch.DrawString(GameMsgFont, "Try Again!", new Vector2(420, 220), Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[3], cCamera.Transform(new Vector2(440, 250)), Color.OrangeRed, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
                        m_SpriteBatch.Draw(UI_SelectGage, WhiteCells.WhiteCell_List[0].bodyViewPortPosition - new Vector2(45,45), null, Color.OrangeRed, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f); 
                        // white tower에 inject 시켜봐라 
            
                        WhiteCells.OnDraw(gameTime);

                  
                        if (WhiteCells.WhiteCell_List[0] is WhiteCell_PointDamType)
                           temp_count = 4;
                        if (!(WhiteCells.WhiteCell_List[0] is WhiteCell_PointDamType) && Vibot.BioGauge_Inven.Count < 1 && Vibot.ReloadBioGauge == null)
                        {
                            Vibot.BioGauge_Inven.Enqueue(new BioGauge(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Vibot.bodyViewPortPosition, BioGaugeMODE.ONVIBOT, 2));
                            Stage3TryFlag = true;
                        }
                    }
                    else if (temp_count == 4)
                    {

                        if (GameFieldManager.BioTrapList.Count == 0)
                        {
                            RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_MOVE;
                            Vibot.WhiteGaugeGrow = true;
                            GameFieldManager.ItemDrop = true; // 이 때부터는 몹으로부터 아이템 드랍됨.
                            temp_count = 5;
                    
                        }
                    }
                    else if (temp_count == 5)
                    {
                        m_SpriteBatch.DrawString(GameMsgFont, InGameMsg[4], cCamera.Transform(new Vector2(400, 200)), Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
          
                        RedBlood.OnUpdate(gameTime);
                        RedBlood.OnDraw(gameTime);
                        Spawnpoints.OnUpdate(gameTime);

                        m_SpriteBatch.DrawString(GameMsgFont, "Laser line Select:\n Select cancel :\nMove Object!:", cCamera.Transform(RedBlood.Blood_Core_Position + new Vector2(-50, 150)), Color.Wheat, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        m_SpriteBatch.DrawString(GameMsgFont, "Click Position  \n Right Mouse Click \n Select and Drag:", cCamera.Transform(RedBlood.Blood_Core_Position + new Vector2(100, 150)), Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                    }


                    pre_keystate = keystate;

                    m_SpriteBatch.End();
                    Camera.Update(Vibot.bodyWorldPosition - cCamera.ScreenCenter); // followingCamera  0 을 향해간다 


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    UI.OnDraw(gameTime, RedBlood, Vibot);
                    DrawClearOrFail();
                    m_SpriteBatch.End();


                    break;


            }

            switch (NowPrologue)
            {
                case ePrologueEvent.Beginning:
                case ePrologueEvent.Development1:
                case ePrologueEvent.Development2:
                case ePrologueEvent.Development3:
                case ePrologueEvent.Climax1:


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(ShadowMask, new Vector2(0, 0), Color.White);   // 게임 UI 표기
                    m_SpriteBatch.End();
                    break;

            }

        }
        #endregion



        //일반적인 게임 모드 
        #region Stage
        private void Stage_Stage(GameTime gameTime)
        {


            switch (NowPrologue)
            {
                case ePrologueEvent.FadeOut:
                    if (Fade_INorOUT())
                    {
                        NowPrologue = ePrologueEvent.GameStart;
                        Vibot.WhiteGaugeGrow = true;
                    }
                        break;

                case ePrologueEvent.GameStart:

                    if (!RedBlood.Blood_Goalin && RedBlood.Life_Count <= 0)
                        StageState = eStageState.STAGE_FAIL;

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(cCamera.Zoom));
                  
                    
                    if (Scene_Menu.GameState == eGameState.GAME_PLAYING)
                    {
                        EventSecond += (float)gameTime.ElapsedGameTime.TotalSeconds;
                  
                        Objects.OnUpdate(gameTime);
                        RedBlood.OnUpdate(gameTime);
                        Vibot.OnUpdate(gameTime);
                        GameFieldManager.OnUpdate(gameTime);
                        Spawnpoints.OnUpdate(gameTime);
                        WhiteCells.OnUpdate(gameTime);
                   
                    }

                    StageBackGround.OnDraw(gameTime);
                    Spawnpoints.OnDraw(gameTime, RedBlood);
                    GameFieldManager.OnDraw(gameTime);
                    Vibot.OnDraw(gameTime);
                    RedBlood.OnDraw(gameTime);
                    Objects.OnDraw(gameTime);
                    WhiteCells.OnDraw(gameTime);
                 //  m_SpriteBatch.DrawString(GameMsgFont, EventSecond.ToString(), new Vector2(100, 100), Color.White);
                 //  m_SpriteBatch.DrawString(GameMsgFont, gameTime.ElapsedGameTime.TotalMilliseconds.ToString(), new Vector2(100, 150), Color.White);
                 //  m_SpriteBatch.DrawString(GameMsgFont, gameTime.ElapsedGameTime.Milliseconds.ToString(), new Vector2(100, 200), Color.White);
                 //  m_SpriteBatch.DrawString(GameMsgFont, gameTime.TotalGameTime.Milliseconds.ToString(), new Vector2(100, 250), Color.White);
                 //  m_SpriteBatch.DrawString(GameMsgFont, gameTime.TotalGameTime.TotalMilliseconds.ToString(), new Vector2(100, 300), Color.White); 

                    if (Current_stage == LastStage)
                    {
                        if (Parasite_Object.HitCnt != -1 || Parasite_Object.HitCnt < Parasite.MaxHP)
                        {    
                            Parasite_Object.OnUpdate(gameTime);
                            Parasite_Object.OnDraw(gameTime);
                        }
                        else if (Parasite_Object.HitCnt >= Parasite.MaxHP)
                        {
                            world.RemoveBody(Parasite_Object.Body);

                            for (int i = 0; i < Parasite_Object._ParasiteBodies.Count; i++)
                                world.RemoveBody(Parasite_Object._ParasiteBodies[i]);

                            Parasite_Object._ParasiteBodies.Clear();

                            Parasite_Object.HitCnt = -1;
                        }
      
                    }




                    m_SpriteBatch.End();

                    Camera.Update(Vibot.bodyWorldPosition - cCamera.ScreenCenter); // followingCamera  0 을 향해간다 


                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    UI.OnDraw(gameTime, RedBlood, Vibot);
                    DrawClearOrFail();
                    m_SpriteBatch.End();


                    break;


            }

            switch (NowPrologue)
            {
                case ePrologueEvent.Beginning:
                case ePrologueEvent.Development1:
                case ePrologueEvent.Development2:
                case ePrologueEvent.Development3:
                case ePrologueEvent.Climax1:

                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    m_SpriteBatch.Draw(ShadowMask, new Vector2(0, 0), Color.White);   // 게임 UI 표기
                    m_SpriteBatch.End();
                    break;

            }

        }
        #endregion

        public override void Destory()
        {
            Stage3TryFlag = false; // 없애야 하나 120910

            Vibot.Destory();
            StageBackGround.Destory();
            RedBlood.Blood_Goalin = false;

            if (Current_stage == 0)
            {
                if (BoxColor != null)
                    for (int i = 0; i < BoxColor.Length; i++)
                        BoxColor[i] = Color.OrangeRed;
                GameFieldManager.Objects.Destory();
            }
            else
                GameFieldManager.Destory();

            if (Current_stage == LastStage)
            {
                world.RemoveBody(Parasite_Object.Body);
 
                    for (int i = 0; i < Parasite_Object._ParasiteBodies.Count; i++)
                        world.RemoveBody(Parasite_Object._ParasiteBodies[i]);
                  
                Parasite_Object._ParasiteBodies.Clear();
            }

            if (RedBlood.BloodCore_State == Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP)
                RedBlood.BloodCore_State = Actor_RedBlood.eREDBLOOD_State.REDBLOOD_START;

            cCamera.Zoom = 1.0f;

            StageState = eStageState.STAGE_INTIALIZE;
            m_FadeState = eFADESTATE.FADE_IN;
            world.Gravity = Vector2.Zero;
            NowPrologue = ePrologueEvent.FadeOut;

            EventSecond = 0;
            m_FadeAlpha = 0;
            if (temp_count > 0)
                temp_count = 0;

            Fade_INorOUT();

            MediaPlayer.Stop();

            for (int i = Map_outline_Body.Length - 1; i >= 0; --i)
                world.RemoveBody(Map_outline_Body[i]);

            if (BoxColor != null)
                for (int i = 0; i < BoxColor.Length; i++)
                    BoxColor[i] = Color.OrangeRed;

            InGameMsg.Clear();
            WelcomeMsg.Clear();

        }




    }
}
