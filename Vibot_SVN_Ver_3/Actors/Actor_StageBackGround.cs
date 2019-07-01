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
    class Actor_StageBackGround : IActor
    {

        public static Rectangle Goal_Zone;
        public Texture2D Goal_Zone_Texture;

        public List<CElement> BackGroundLayers = new List<CElement>();
        public List<CElement> TileBlockList = new List<CElement>();

        public Rectangle BackGroundRect; // 뒷배경 화면 
        Texture2D BackGround_Texture;
        CellField CellField;

        private double backcellmovement;
        private const float cellsParallaxPeriod = 28f;  // 
        private const float cellsParallaxAmplitude = 1028f; // BackGround Cell 진폭


        public override void LoadDataFromMap(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            foreach (XmlNode ChildNode in ChildNodes)
            {

                switch (ChildNode.Name)
                {
                    case "MainTile":
                        {
                            string path = "Sprites\\Stages\\" + ChildNode.InnerText.ToString();
                            BackGround_Texture = m_ContentManager.Load<Texture2D>(path);
                        }
                        break;

                    case "Rectangle":
                        {
                            string[] sizes = ChildNode.InnerText.Split(' ');
                            BackGroundRect = new Rectangle(int.Parse(sizes[0]), int.Parse(sizes[1]),
                                int.Parse(sizes[2]), int.Parse(sizes[3]));
                        }
                        break;

                    case "Goal_Zone":
                        string[] strings = ChildNode.InnerText.Split(' ');
                        Goal_Zone = new Rectangle(int.Parse(strings[0]), int.Parse(strings[1]),
                            int.Parse(strings[2]), int.Parse(strings[3]));

                        break;

                    case "BackGroundLayers":
                        XmlNodeList InnerChildNodes = ChildNode.ChildNodes;

                        foreach (XmlNode InnerChildNode in InnerChildNodes)
                        {
                            CElement BackLayer = new CElement();

                            if (InnerChildNode.Name == "BackGroundLayer")
                            {
                                XmlNodeList InnerInnerChildNodes = InnerChildNode.ChildNodes;

                                foreach (XmlNode InnerInnerChildNode in InnerInnerChildNodes)
                                {

                                    switch (InnerInnerChildNode.Name)
                                    {
                                        case "Type":
                                            string path = "Sprites\\Stages\\BackGroundLayers\\" + InnerInnerChildNode.InnerText.ToString();
                                            BackLayer.m_Texture = m_ContentManager.Load<Texture2D>(path);
                                            break;

                                        case "Rectangle":
                                            {
                                                string[] Rectangle = InnerInnerChildNode.InnerText.Split(' ');
                                                BackLayer.m_Rect.X = int.Parse(Rectangle[0]);
                                                BackLayer.m_Rect.Y = int.Parse(Rectangle[1]);
                                                BackLayer.m_Rect.Width = int.Parse(Rectangle[2]);
                                                BackLayer.m_Rect.Height = int.Parse(Rectangle[3]);
                                                break;
                                            }

                                        case "Angle":
                                            {
                                                BackLayer.m_angle = MathHelper.ToRadians(float.Parse(InnerInnerChildNode.InnerText));
                                                break;
                                            }

                                    }

                                }
                                BackGroundLayers.Add(BackLayer);


                            }
                        }
                        break;

                    //TileBlock 만들기 
                    case "TileBlocks":
                        {
                            XmlNodeList InnerChildNodes2 = ChildNode.ChildNodes;

                            foreach (XmlNode InnerChildNode in InnerChildNodes2)
                            {
                                CElement TileBlock = new CElement();

                                if (InnerChildNode.Name == "TileBlock")
                                {
                                    XmlNodeList InnerInnerChildNodes = InnerChildNode.ChildNodes;

                                    foreach (XmlNode InnerInnerChildNode in InnerInnerChildNodes)
                                    {

                                        switch (InnerInnerChildNode.Name)
                                        {
                                            case "Type":
                                                TileBlock.m_Type = int.Parse(InnerInnerChildNode.InnerText);
                                                string path = "Sprites\\Stages\\TileBlock\\TileBlock" + TileBlock.m_Type.ToString();
                                                TileBlock.m_Texture = m_ContentManager.Load<Texture2D>(path);
                                                break;

                                            case "Location":
                                                {
                                                    string[] Rectangle = InnerInnerChildNode.InnerText.Split(' ');
                                                    TileBlock.m_Rect.X = int.Parse(Rectangle[0]);
                                                    TileBlock.m_Rect.Y = int.Parse(Rectangle[1]);
                                                    if (TileBlock.m_Type == 1)
                                                    {
                                                        TileBlock.m_Rect.Width = 128;
                                                        TileBlock.m_Rect.Height = 128;
                                                    }
                                                    else if (TileBlock.m_Type == 2)
                                                    {
                                                        TileBlock.m_Rect.Width = 300;
                                                        TileBlock.m_Rect.Height = 300;
                                                    }

                                                    break;
                                                }


                                        }
                                    }

                                    TileBlock.SetUpPhysics(world, new Vector2(TileBlock.m_Rect.Center.X, TileBlock.m_Rect.Center.Y), TileBlock.m_Rect.Width,
                                         TileBlock.m_Rect.Height, 1);
                                    TileBlock.body.BodyType = BodyType.Static;
                                    TileBlock.body.CollidesWith = Category.Cat2 | ~Category.All; //Category.Cat1 |
                                    TileBlockList.Add(TileBlock);


                                }
                            }
                            break;
                        }

                }
            }
            m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\Tiles");
            Goal_Zone_Texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\Goal_Zone");



        }



        public Actor_StageBackGround(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;


            CellField = new CellField(Vector2.Multiply(new Vector2(
                (float)Math.Cos(backcellmovement / cellsParallaxPeriod),
                (float)Math.Sin(backcellmovement / cellsParallaxPeriod)),
                cellsParallaxAmplitude), m_GraphicDevice, m_ContentManager);

            CellField.LoadContent();
        }



        public void OnDraw(GameTime gameTime)
        {

            // draw the cells
            backcellmovement += gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 position = Vector2.Multiply(new Vector2(
                    (float)Math.Cos(backcellmovement / cellsParallaxPeriod),
                    (float)Math.Sin(backcellmovement / cellsParallaxPeriod)),
                    cellsParallaxAmplitude);
            CellField.Draw(position, m_SpriteBatch);

            m_SpriteBatch.Draw(BackGround_Texture, cCamera.Transform(BackGroundRect), Color.White);
            // cCamera.Transform 해야하나요? ㅇㅇ

            m_SpriteBatch.Draw(m_Texture, new Rectangle(0, 0, 10, (int)BackGroundRect.Y), new Rectangle(318 - 64, 0, 64, 64), Color.White);
            m_SpriteBatch.Draw(m_Texture, new Rectangle((int)BackGroundRect.X - 10, 0, 10, (int)BackGroundRect.Y), new Rectangle(318 - 64, 0, 64, 64), Color.White);
            m_SpriteBatch.Draw(m_Texture, new Rectangle(0, 0, (int)BackGroundRect.X, 10), new Rectangle(318 - 64, 0, 64, 64), Color.White);
            m_SpriteBatch.Draw(m_Texture, new Rectangle(0, (int)BackGroundRect.Y - 10, (int)BackGroundRect.X, 10), new Rectangle(318 - 64, 0, 64, 64), Color.White);

            CellField.Draw(position, m_SpriteBatch);

            if (BackGroundLayers.Count > 0)
                foreach (CElement BackGroundLayer in BackGroundLayers)
                {
                    // m_SpriteBatch.Draw(BackGroundLayer.m_Texture, BackGroundLayer.ViewPortRectangle, null,
                    //         Color.White);
                    m_SpriteBatch.Draw(BackGroundLayer.m_Texture, BackGroundLayer.ViewPortRectangle, null,
                        Color.White,
                        (float)BackGroundLayer.m_angle, BackGroundLayer.m_TextureOrigin, SpriteEffects.None, 0f);
                }


            if (TileBlockList.Count > 0)  //TileBlockList 그리기
                foreach (CElement TileBlock in TileBlockList)
                    m_SpriteBatch.Draw(TileBlock.m_Texture, TileBlock.ViewPortRectangle, null,
                        Color.White);


            // 목표 포인트
            m_SpriteBatch.Draw(Goal_Zone_Texture, cCamera.Transform(Goal_Zone), Color.White);
        }




        public override void Destory()
        {

           if(BackGroundLayers.Count > 0)
            BackGroundLayers.Clear();

           if (TileBlockList.Count > 0)
           {
               for (int i = TileBlockList.Count - 1; i >= 0; i--)
                   world.RemoveBody(TileBlockList[i].body);

                   TileBlockList.Clear();
           }

        }





    }
}

