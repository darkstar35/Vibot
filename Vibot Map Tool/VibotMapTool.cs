using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.Xml;
using System.IO;
using VibotCommon;
using ObjectFactory;
using System.Diagnostics;

namespace Cellbot_Map_Tool
{
    public partial class VibotMapTool : Form
    {
        enum eMode
        {
            MODE_NONE = 0,
            MODE_MAP_TILE = 1,
            MODE_MAP_VIBOTSTARTPOINT = 2,
            MODE_MAP_MAIN = 3,
            MODE_REDBLOOD_ROAD = 4,
            MODE_SELECTEDOBJECT_MOVE = 5,
        };

        private string[] ObjectComponentName =
        {
            "",
            "BackGround1",
            "BackGround2",
            "BackGround3",
            "Vibot",
            "RedBlood",
            "Enemy",
            "WhiteCell",
            "Object",
            "Object",
            "Goal_Zone",
        };

        private List<CObjectCommon> m_Object = new List<CObjectCommon>();
        private CStage m_Stage = new CStage();


        //BackBuffer Layer(Map)
        private Bitmap m_BackBuffer = null;
        private Graphics m_BackBufferGraphics = null;
        private Rectangle m_BackBufferRect = new Rectangle(0, 0, 0, 0);

        private Bitmap m_Map = null;
        private Graphics m_MapGraphics = null;

        private Bitmap m_SelectedImage = null;

        private Bitmap m_Border = null;
        private CObjectCommon m_SelectedObject = null;
        private string m_SelectedObjectName = "";
        private Bitmap m_SelectedImage_forMove = null;




        //BackBuffer2 Layer(MiniMap)
        private Bitmap m_BackBuffer2 = null;
        private Graphics m_BackBuffer2Graphics = null;

        //Pen
        private Pen m_RedPen = new Pen(Brushes.Red, 1);
        private Pen m_YellowPen = new Pen(Brushes.Yellow, 2);
        private Pen m_GreenPen = new Pen(Brushes.Green, 1);

        private bool m_bCreatedMap = false;

        private Bitmap m_CursorImage = null;

        private Point m_CursorPos = new Point(0, 0);

        private int iSelectedImage = 0;

        private eMode CurrentMode = eMode.MODE_NONE;

        public VibotMapTool()
        {
            InitializeComponent();
            m_Stage.RegisterComponent();
        }

        private void Draw()
        {
            DrawBackBuffer();
            DrawBackBuffer2();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            m_CursorPos.X = e.Location.X;
            m_CursorPos.Y = e.Location.Y;
            toolStripStatusLabel1.Text = "Mouse Pos(" + m_CursorPos.X + ", " + m_CursorPos.Y + ")";
            DrawBackBuffer();
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && m_bCreatedMap) // 맵에디터 상태에서  필드에 클릭시 배치됨 
            {
                switch (CurrentMode)
                {
                    case eMode.MODE_MAP_TILE:
                    case eMode.MODE_MAP_MAIN:
                    case eMode.MODE_MAP_VIBOTSTARTPOINT:
                        {
                            m_CursorPos.X = e.Location.X;
                            m_CursorPos.Y = e.Location.Y;
                            if (iSelectedImage == null)
                                return;
                            CreateObject(iSelectedImage);
                            Draw();
                        }
                        break;

                    case eMode.MODE_REDBLOOD_ROAD:
                            AddRedBloodRoad(e.Location);
                        break;

                    case eMode.MODE_NONE:
                        {
                            m_SelectedObject = null;
                            m_SelectedObjectName = "";
                            GetSelectedObject(e.Location);
                            if (m_Border != null)
                            {
                                m_Border.Dispose();
                                m_Border = null;
                            }
                            if (m_SelectedObject != null)
                            {

                                m_Border = new Bitmap(m_SelectedObject.m_BaseInfo.m_Rect.Width, m_SelectedObject.m_BaseInfo.m_Rect.Height);
                                Graphics G = Graphics.FromImage(m_Border);
                                G.DrawRectangle(m_YellowPen, 1, 1, m_SelectedObject.m_BaseInfo.m_Rect.Width - 1, m_SelectedObject.m_BaseInfo.m_Rect.Height - 1);
                            }

                        }
                        break;
                    case eMode.MODE_SELECTEDOBJECT_MOVE:
                        {
                            m_SelectedObject.m_BaseInfo.m_Rect.X = e.X;
                            m_SelectedObject.m_BaseInfo.m_Rect.Y = e.Y;
                            LOAD_ClearState();
                            DrawAllObject();
                            DrawBackBuffer();
                        }
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (CurrentMode == eMode.MODE_NONE)
                {
                    m_SelectedObject = null;
                    m_SelectedObjectName = "";
                    if (m_Border != null)
                    {
                        m_Border.Dispose();
                        m_Border = null;
                    }
                    GetSelectedObject(e.Location);
                    if (m_SelectedObject != null)
                    {
                        m_Border = new Bitmap(m_SelectedObject.m_BaseInfo.m_Rect.Width, m_SelectedObject.m_BaseInfo.m_Rect.Height);
                        Graphics G = Graphics.FromImage(m_Border);
                        G.DrawRectangle(m_YellowPen, 1, 1, m_SelectedObject.m_BaseInfo.m_Rect.Width - 1, m_SelectedObject.m_BaseInfo.m_Rect.Height - 1);
                    }
                    if (m_SelectedObjectName == "Enemy")
                    {
                        Point Pos = new Point();
                        Pos = pictureBox2.PointToScreen(e.Location);
                        contextMenuStrip2.Show(Pos);
                    }
            //    else if (m_SelectedObjectName == "WhiteCell")
            //    {
            //        Point Pos = new Point();
            //        Pos = pictureBox2.PointToScreen(e.Location);
            //        contextMenuStrip2.Show(Pos);
            //    }
            //    else if (m_SelectedObjectName == "Vibot")
            //    {
            //        Point Pos = new Point();
            //        Pos = pictureBox2.PointToScreen(e.Location);
            //        contextMenuStrip2.Show(Pos);
            //    }
            //    else if (m_SelectedObjectName == "RedBlood")
            //    {
            //        Point Pos = new Point();
            //        Pos = pictureBox2.PointToScreen(e.Location);
            //        contextMenuStrip2.Show(Pos);
            //    }
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!m_bCreatedMap) return;

            if (e.Node.ImageIndex != 0)
            {
                switch (e.Node.ImageIndex)
                {
                    case 1:
                    case 2:
                    case 3:
                        CurrentMode = eMode.MODE_MAP_TILE;
                        break;
                    case 4:
                        CurrentMode = eMode.MODE_MAP_VIBOTSTARTPOINT;
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        CurrentMode = eMode.MODE_MAP_MAIN;

                        break;
                }


                iSelectedImage = e.Node.ImageIndex;
 
                if (m_SelectedImage != null)
                    m_SelectedImage.Dispose();

                if (m_CursorImage != null)
                    m_CursorImage.Dispose();

                m_CursorImage = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject(e.Node.Text));
            }
        }

        private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e)
        {
            DrawBackBuffer2();
        }

        private void splitContainer1_Panel2_ClientSizeChanged(object sender, EventArgs e)
        {
            if (m_BackBuffer2Graphics != null)
                DrawBackBuffer2();
        }

        private void CellbotMapTool_KeyUp(object sender, KeyEventArgs e)
        {
           // if (CurrentMode == eMode.MODE_NONE) return;

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        LOAD_ClearState();
                        DrawBackBuffer();
                    }
                    break;
                case Keys.Delete:
                    {
                        if (m_SelectedObject != null)
                        {
                            CComponent Component = m_Stage.GetComponent(m_SelectedObjectName);
                            Component.DeleteObject(m_SelectedObject);

                            m_SelectedObject = null;
                            m_SelectedObjectName = "";
                            if (m_Border != null)
                            {
                                m_Border.Dispose();
                                m_Border = null;
                            }
                            DrawAllObject();
                            Draw();
                        }
                    }
                    break;
                default:
                    break;
            }
        }



        private void MakeTransparentSpwanImage()
        {
            float[][] ptsArray = 
                    {
                        new float[] {1.0f, 0, 0, 0, 0},
                        new float[] {0, 1.0f, 0, 0, 0},
                        new float[] {0, 0, 1.0f, 0, 0},
                        new float[] {0, 0, 0, 1.0f, 0},
                        new float[] {0.5f, 0, 0, 0, 0}
                    };

            ImageAttributes Attribute = new ImageAttributes();
            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            Attribute.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            m_SelectedImage_forMove = new Bitmap(59, 60);
            Graphics G = Graphics.FromImage(m_SelectedImage_forMove);
            Rectangle Rect = new Rectangle(0, 0, m_SelectedObject.m_BaseInfo.m_Rect.Width, m_SelectedObject.m_BaseInfo.m_Rect.Height);
            G.DrawImage(m_CursorImage, Rect, 0, 0, Rect.Width, Rect.Height, GraphicsUnit.Pixel, Attribute);
        }


        //Draw BackBuffer

        private void DrawBackBuffer()
        {
            if (!m_bCreatedMap)
                return;

            m_BackBufferGraphics.Clear(BackColor);
            m_BackBufferGraphics.DrawImage(m_Map, 0, 0);

            if (CurrentMode == eMode.MODE_MAP_TILE)
                DrawTileSection();

            DrawVirusSpawnPoints();

            if (m_Border != null)
                m_BackBufferGraphics.DrawImage(m_Border, m_SelectedObject.m_BaseInfo.m_Rect);

            DrawRedBloodRoad();


            if (m_SelectedImage_forMove != null)
                m_BackBufferGraphics.DrawImage(m_SelectedImage_forMove, m_SelectedObject.m_BaseInfo.m_Rect);

            if (m_CursorImage != null)
                m_BackBufferGraphics.DrawImage(m_CursorImage, m_CursorPos);

            pictureBox2.Refresh();
        }


        // Draw BackBuffer2
        private void DrawBackBuffer2()
        {
            if (!m_bCreatedMap)
                return;

            m_BackBuffer2Graphics.Clear(BackColor);

            Point Pos = new Point();
            Pos.X = -(splitContainer1.Panel2.AutoScrollPosition.X) / (pictureBox2.Width / pictureBox1.Width);
            Pos.Y = -(splitContainer1.Panel2.AutoScrollPosition.Y) / (pictureBox2.Height / pictureBox1.Height);

            int iWidth = (splitContainer1.Panel2.ClientSize.Width) / (pictureBox2.Width / pictureBox1.Width);
            int iHeight = (splitContainer1.Panel2.ClientSize.Height) / (pictureBox2.Height / pictureBox1.Height);

            m_BackBuffer2Graphics.DrawImage(m_Map, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            m_BackBuffer2Graphics.DrawRectangle(m_RedPen, new Rectangle(0, 0, iWidth, iHeight));
            pictureBox1.Refresh();
        }


        private void CreateObject(int iObjectIndex)
        {
            String objName = ObjectComponentName[iObjectIndex];

            CObjectCommon Object = new CObjectCommon(objName);
            if (ObjectComponentName[iObjectIndex] != "Enemy")
            {
                string[] Key = imageList1.Images.Keys[iObjectIndex].Split('.');
                Debug.WriteLine(Key[0]);
                Object.m_BaseInfo.m_ImageName = Key[0];
            }
            switch (ObjectComponentName[iObjectIndex])
            {
                case "Tile":
                    {
                        //    int iSectionWidth = m_CursorPos.X / m_Stage.m_Tile.Width;
                        //    int iSectionHeight = m_CursorPos.Y / m_Stage.m_Tile.Height;
                        //
                        //    Object.m_BaseInfo.m_Rect.X = iSectionWidth * m_Stage.m_Tile.Width;
                        //    Object.m_BaseInfo.m_Rect.Y = iSectionHeight * m_Stage.m_Tile.Height;
                        Object.m_BaseInfo.m_Rect.Width = m_SelectedImage.Width;
                        Object.m_BaseInfo.m_Rect.Height = m_SelectedImage.Height;
                    }
                    break;
                default:
                    {
                        Object.m_BaseInfo.m_Rect.X = m_CursorPos.X;
                        Object.m_BaseInfo.m_Rect.Y = m_CursorPos.Y;
                      // Object.m_BaseInfo.m_Rect.Width = m_SelectedImage.Width;
                      // Object.m_BaseInfo.m_Rect.Height = m_SelectedImage.Height;
                    }
                    break;
            }

            AddObject(Object, ObjectComponentName[iObjectIndex]);
            DrawAllObject();
        }

        private void DrawAllObject()
        {
            m_MapGraphics.Clear(BackColor);
            if (m_Stage.m_ImageName != null)
                m_MapGraphics.DrawImage((System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject(m_Stage.m_ImageName)), new Rectangle(0, 0, m_Map.Width, m_Map.Height));
            foreach (KeyValuePair<string, CComponent> Component in m_Stage.m_Components)
            {
                if (Component.Key != "Enemy")
                {
                    CComponent ObjectComponent = (CComponent)Component.Value;
                    foreach (CObjectCommon Object in ObjectComponent.GetObjectList())
                        DrawObject(Object);

                }
            }
        }

        private void DrawObject(CObjectCommon ObjectCommon)
        {
            Bitmap ObjectImage = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject(ObjectCommon.m_BaseInfo.m_ImageName));
            m_MapGraphics.DrawImage(ObjectImage, ObjectCommon.m_BaseInfo.m_Rect);
        }

        private void AddObject(CObjectCommon Object, string CompontName, string name = null)
        {
            Debug.WriteLine(CompontName);
            CComponent Component = m_Stage.GetComponent(CompontName);
            Component.AddObject(Object);
        }

        private void DrawTileSection()
        {
            //     int iSectionWidth = m_CursorPos.X / m_Stage.m_Tile.Width;
            //     int iSectionHeight = m_CursorPos.Y / m_Stage.m_Tile.Height;

            //       m_BackBufferGraphics.DrawRectangle(m_RedPen, new Rectangle(iSectionWidth * m_Stage.m_Tile.Width, iSectionHeight * m_Stage.m_Tile.Height, m_Stage.m_Tile.Width, m_Stage.m_Tile.Height));
        }

        private void 이동경로ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_bCreatedMap)
            {
                MessageBox.Show("맵을 생성해 주세요");
                return;
            }

            LOAD_ClearState();
            CurrentMode = eMode.MODE_REDBLOOD_ROAD;
        }

        private void DrawRedBloodRoad()
        {
            CComponent Component = m_Stage.GetComponent("RedBlood");

            if (Component == null)
                return;

            CRedBlood RedBlood = (CRedBlood)Component;
            Bitmap Flag = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject("flag"));
            Point OldPoint = new Point(0, 0);
            Point Gap = new Point(Flag.Width / 2, Flag.Height / 2);

            foreach (Point Road in RedBlood.m_Road)
            {
                if (OldPoint.X != 0 && OldPoint.Y != 0)
                {
                    m_BackBufferGraphics.DrawLine(m_GreenPen, OldPoint, Road);
                }
                OldPoint = Road;
            }

            foreach (Point Road in RedBlood.m_Road)
                m_BackBufferGraphics.DrawImage(Flag, Road.X - Gap.X, Road.Y - Gap.Y);

        }
        private void AddRedBloodRoad(Point Road)
        {
            CComponent Component = m_Stage.GetComponent("RedBlood");
            CRedBlood RedBlood = (CRedBlood)Component;

            RedBlood.m_Road.Add(Road);
        }

        private void DrawVirusSpawnPoints()
        {
            CComponent Component = m_Stage.GetComponent("Enemy");

            if (Component == null)
                return;


            CEnemy SpawnPoints = (CEnemy)Component;
            Bitmap SpawnPointImage = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject("SpawnPoint"));

            foreach (CObjectCommon Object in Component.GetObjectList())
                m_BackBufferGraphics.DrawImage(SpawnPointImage, Object.m_BaseInfo.m_Rect);

        }



        private void GetSelectedObject(Point pos) //클릭->선택
        {
            Rectangle Rect = new Rectangle(pos.X, pos.Y, 1, 1);
            CObjectCommon Object = null;
            Object = GetSelectedObjectFromComponent(Rect, "Vibot");
            if (Object != null)
            {
                m_SelectedObjectName = "Vibot";
                m_SelectedObject = Object;
                return;
            }

            Object = GetSelectedObjectFromComponent(Rect, "Enemy");
            if (Object != null)
            {
                m_SelectedObjectName = "Enemy";
                m_SelectedObject = Object;
                return;
            }
            Object = GetSelectedObjectFromComponent(Rect, "WhiteCell");
            if (Object != null)
            {
                m_SelectedObjectName = "WhiteCell";
                m_SelectedObject = Object;
                return;
            }

            Object = GetSelectedObjectFromComponent(Rect, "RedBlood");
            if (Object != null)
            {
                m_SelectedObjectName = "RedBlood";
                m_SelectedObject = Object;
                return;
            }

            Object = GetSelectedObjectFromComponent(Rect, "Object");
            if (Object != null)
            {
                m_SelectedObjectName = "Object";
                m_SelectedObject = Object;
                return;
            }

            Object = GetSelectedObjectFromComponent(Rect, "Tile");
            if (Object != null)
            {
                m_SelectedObjectName = "Tile";
                m_SelectedObject = Object;
                return;
            }
        }

        private CObjectCommon GetSelectedObjectFromComponent(Rectangle Rect, string name)
        {
            CComponent Component = m_Stage.GetComponent(name);
            if (Component != null)
            {
                CObjectCommon Object = Component.GetIntersectObject(Rect);
                if (Object != null)
                {
                    return Object;
                }
            }
            return null;
        }

        private void LOAD_ClearState()
        {
            CurrentMode = eMode.MODE_NONE;
            iSelectedImage = 0;
            if (m_CursorImage != null)
            {
                m_CursorImage.Dispose();
                m_CursorImage = null;
            }

            if (m_SelectedImage != null)
            {
                m_SelectedImage.Dispose();
                m_SelectedImage = null;
            }

            if (m_Border != null)
            {
                m_Border.Dispose();
                m_Border = null;
            }

            if (m_SelectedImage_forMove != null)
            {
                m_SelectedImage_forMove.Dispose();
                m_SelectedImage_forMove = null;
            }

            m_SelectedObject = null;
            m_SelectedObjectName = "";
            DrawBackBuffer();
        }

        private void CreateMapFile(string FullPath)  // save 
        {
            XmlTextWriter MapFile = new XmlTextWriter(FullPath, Encoding.UTF8);
            MapFile.Formatting = Formatting.Indented;



            MapFile.WriteStartDocument();
            MapFile.WriteStartElement("VibotMapInfo");

            MapFile.WriteStartElement("MapInfo");
            MapFile.WriteElementString("MainTile", m_Stage.m_ImageName);
            MapFile.WriteElementString("Map_Width", string.Format("{0}", m_Stage.m_Map.Width));
            MapFile.WriteElementString("Map_Height", string.Format("{0}", m_Stage.m_Map.Height));
            WriteGoal_ZoneToMap(MapFile);

            MapFile.WriteEndElement();

            WriteBackGroundInfoToMap(MapFile);
            WriteVibotToMap(MapFile);
            WriteRedBloodToMap(MapFile);
            WritesWhiteCellPointsToMap(MapFile);
            WritesObjectPointsToMap(MapFile);
            WriteEnemysToMap(MapFile);
            MapFile.WriteEndElement();
            MapFile.WriteEndDocument();

            MapFile.Close();
        }

        private void WriteBackGroundInfoToMap(XmlTextWriter MapFile)
        {
            MapFile.WriteStartElement("BackGroundLayer");
            CComponent Component = m_Stage.GetComponent("BackGround1");

            if (Component.GetObjectList().Count == 0)
            {
                MapFile.WriteEndElement();
                return;
            }

            foreach (CObjectCommon Tile in Component.GetObjectList()) // 존나 ㅂㅎㅇ같은 코드
            {
                MapFile.WriteElementString("Type", Tile.m_BaseInfo.m_ImageName);
                string Location = "";
                Location = string.Format("{0} {1} {2} {3}", Tile.m_BaseInfo.m_Rect.X, Tile.m_BaseInfo.m_Rect.Y, Tile.m_BaseInfo.m_Rect.Width, Tile.m_BaseInfo.m_Rect.Height);
                MapFile.WriteElementString("Rectangle", Location);
                MapFile.WriteElementString("Angle", "90");
            }

            Component = m_Stage.GetComponent("BackGround2");

            if (Component.GetObjectList().Count == 0)
            {
                MapFile.WriteEndElement();
                return;
            }

            foreach (CObjectCommon Tile in Component.GetObjectList()) // 존나 ㅂㅎㅇ같은 코드
            {
                MapFile.WriteElementString("Type", Tile.m_BaseInfo.m_ImageName);
                string Location = "";
                Location = string.Format("{0} {1} {2} {3}", Tile.m_BaseInfo.m_Rect.X, Tile.m_BaseInfo.m_Rect.Y, Tile.m_BaseInfo.m_Rect.Width, Tile.m_BaseInfo.m_Rect.Height);
                MapFile.WriteElementString("Rectangle", Location);
                MapFile.WriteElementString("Angle", "90");
            }

            Component = m_Stage.GetComponent("BackGround3");

            if (Component.GetObjectList().Count == 0)
            {
                MapFile.WriteEndElement();
                return;
            }

            foreach (CObjectCommon Tile in Component.GetObjectList()) // 존나 ㅂㅎㅇ같은 코드
            {
                MapFile.WriteElementString("Type", Tile.m_BaseInfo.m_ImageName);
                string Location = "";
                Location = string.Format("{0} {1} {2} {3}", Tile.m_BaseInfo.m_Rect.X, Tile.m_BaseInfo.m_Rect.Y, Tile.m_BaseInfo.m_Rect.Width, Tile.m_BaseInfo.m_Rect.Height);
                MapFile.WriteElementString("Rectangle", Location);
                MapFile.WriteElementString("Angle", "90");
            }

            MapFile.WriteEndElement();
        }

        private void WriteVibotToMap(XmlTextWriter MapFile)
        {
            CComponent Component = m_Stage.GetComponent("Vibot");

            if (Component.GetObjectList().Count == 0) return;

            MapFile.WriteStartElement("Vibot");
            foreach (CObjectCommon Vibot in Component.GetObjectList())
            {
                MapFile.WriteElementString("Image", Vibot.m_BaseInfo.m_ImageName);
                string Location = "";
                Location = string.Format("{0} {1} {2} {3}", Vibot.m_BaseInfo.m_Rect.X, Vibot.m_BaseInfo.m_Rect.Y, Vibot.m_BaseInfo.m_Rect.Width, Vibot.m_BaseInfo.m_Rect.Height);
                MapFile.WriteElementString("Location", Location);
            }
            MapFile.WriteEndElement();
        }

        private void WriteGoal_ZoneToMap(XmlTextWriter MapFile)
        {
            CComponent Component = m_Stage.GetComponent("Goal_Zone");

            if (Component.GetObjectList().Count == 0)
                return;

            foreach (CObjectCommon Goal_Zone_Position in Component.GetObjectList())
                MapFile.WriteElementString("Goal_Zone", string.Format("{0} {1}", Goal_Zone_Position.m_BaseInfo.m_Rect.X, Goal_Zone_Position.m_BaseInfo.m_Rect.Y));

        }

        private void WriteRedBloodToMap(XmlTextWriter MapFile)
        {
            CComponent Component = m_Stage.GetComponent("RedBlood");

            if (Component.GetObjectList().Count == 0) return;

            MapFile.WriteStartElement("RedBlood");
            foreach (CObjectCommon RedBlood in Component.GetObjectList())
            {
                MapFile.WriteElementString("Image", RedBlood.m_BaseInfo.m_ImageName);
                string Location = "";
                Location = string.Format("{0} {1}", RedBlood.m_BaseInfo.m_Rect.X, RedBlood.m_BaseInfo.m_Rect.Y);
                MapFile.WriteElementString("Location", Location);
            }
            MapFile.WriteStartElement("Road");
            CRedBlood RedBloods = (CRedBlood)Component;
            foreach (Point Pos in RedBloods.m_Road)
            {
                MapFile.WriteStartElement("Pos");
                string Location = "";
                Location = string.Format("{0} {1}", Pos.X, Pos.Y);
                MapFile.WriteString(Location);
                MapFile.WriteEndElement();
            }
            MapFile.WriteEndElement();
            MapFile.WriteEndElement();
        }

        private void WriteEnemysToMap(XmlTextWriter MapFile)
        {
            CComponent Component = m_Stage.GetComponent("Enemy");

            if (Component.GetObjectList().Count == 0)
                return;

            MapFile.WriteStartElement("SpawnPoints");

            CEnemy Enemys = (CEnemy)Component;
            foreach (CObjectCommon SpawnPoint in Component.GetObjectList())
            {
                CEnemySpawnInfo EnemySpawnInfo = Enemys.GetSpwanInfo(SpawnPoint);
                MapFile.WriteStartElement("SpawnPoint");
                MapFile.WriteStartElement("Location");
                string Location = "";
                Location = string.Format("{0} {1} {2} {3}", SpawnPoint.m_BaseInfo.m_Rect.X, SpawnPoint.m_BaseInfo.m_Rect.Y, SpawnPoint.m_BaseInfo.m_Rect.Width, SpawnPoint.m_BaseInfo.m_Rect.Height);
                MapFile.WriteString(Location);
                MapFile.WriteEndElement();

                MapFile.WriteStartElement("SpawnTick");
                MapFile.WriteValue(EnemySpawnInfo.m_Interval);
                MapFile.WriteEndElement();

                MapFile.WriteStartElement("AliveTime");
                MapFile.WriteValue(EnemySpawnInfo.m_AliveTime);
                MapFile.WriteEndElement();

                MapFile.WriteStartElement("Enemys");
                foreach (CEnemySpawnInfo._SpawnInfo SpawnInfo in EnemySpawnInfo.m_SpwanInfo)
                {
                    MapFile.WriteStartElement("Enemy");
                    MapFile.WriteElementString("Name", SpawnInfo.m_EnemyName);
                    MapFile.WriteStartElement("Probability");
                    MapFile.WriteValue(SpawnInfo.m_Probability);
                    MapFile.WriteEndElement();
                    MapFile.WriteEndElement();
                }
                MapFile.WriteEndElement();

                MapFile.WriteEndElement();

            }
            MapFile.WriteEndElement();
        }

        private void WritesObjectPointsToMap(XmlTextWriter MapFile)  //한단계 이전으로 가야 합니다 
        {
            CComponent Component = m_Stage.GetComponent("Object");

            if (Component.GetObjectList().Count == 0)
                return;

            MapFile.WriteStartElement("ObjectPoints"); // 쓴다 

            CObject Objects = (CObject)Component;
            foreach (CObjectCommon ObjectPoint in Component.GetObjectList())
            {
                MapFile.WriteStartElement("Object");

                MapFile.WriteElementString("Name", ObjectPoint.m_BaseInfo.m_ImageName);
                
                MapFile.WriteStartElement("Location");

                string Location = "";
                Location = string.Format("{0} {1}", ObjectPoint.m_BaseInfo.m_Rect.X, ObjectPoint.m_BaseInfo.m_Rect.Y);

                MapFile.WriteString(Location);
                MapFile.WriteEndElement();

          
                MapFile.WriteEndElement();

            }
            MapFile.WriteEndElement();
        }


        private void WritesWhiteCellPointsToMap(XmlTextWriter MapFile)
        {
            CComponent Component = m_Stage.GetComponent("WhiteCell");
            if (Component.GetObjectList().Count == 0)
                return;

            MapFile.WriteStartElement("WhiteCellPoints"); // 쓴다 

            foreach (CObjectCommon WhiteCell in Component.GetObjectList())
            {
                MapFile.WriteStartElement("WhiteCell"); // 쓴다 
                MapFile.WriteElementString("Level", string.Format("{0}", WhiteCell.m_BaseInfo.m_Level)); //임시로 레벨 1
                MapFile.WriteElementString("Location", string.Format("{0} {1}", WhiteCell.m_BaseInfo.m_Rect.X, WhiteCell.m_BaseInfo.m_Rect.Y));
                MapFile.WriteEndElement(); //닫는다
            }


            MapFile.WriteEndElement();
        }

        private void setSpawnInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEnemySpawn Dialog = new SetEnemySpawn();
            Dialog.ShowDialog();
        }

        public void SaveVirusSpwanInfo(CEnemySpawnInfo SpwanInfo)
        {
            CComponent Component = m_Stage.GetComponent("Enemy");
            Component.AddSpwanInfo(m_SelectedObject, SpwanInfo);
        }

        public CEnemySpawnInfo GetCurrentVirusSpwanInfo()
        {
            CComponent Component = m_Stage.GetComponent("Enemy");
            return Component.GetSpwanInfo(m_SelectedObject);

        }

        public void ClearMap()
        {
            m_Stage.Clear();
            LOAD_ClearState();
            DrawAllObject();
            Draw();
        }

        private bool IsEmptyMap()
        {
            foreach (KeyValuePair<string, CComponent> Component in m_Stage.m_Components)
            {
                CComponent TempComponent = (CComponent)Component.Value;
                if (!TempComponent.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }
        public void SetGoalPoint(Point Pos)
        {
            m_Stage.m_Map.Width = Width;

        }

        public void SetMapSize(int Width, int Height)
        {
            m_Stage.m_Map.Width = Width;
            m_Stage.m_Map.Height = Height;
        }

        public void SetTileSize(int Width, int Height)
        {
            //  m_Stage.m_Tile.Width = Width;
            //  m_Stage.m_Tile.Height = Height;
        }

        public void LOAD_InitalizeMap()
        {
            m_bCreatedMap = true;

            if (m_BackBuffer != null)
            {
                m_BackBuffer.Dispose();
                m_BackBuffer = null;
            }

            if (m_Map != null)
            {
                m_Map.Dispose();
                m_Map = null;
            }

            if (m_BackBuffer2 != null)
            {
                m_BackBuffer2.Dispose();
                m_BackBuffer2 = null;
            }

            m_BackBuffer = new Bitmap(m_Stage.m_Map.Width, m_Stage.m_Map.Height);
            m_BackBufferGraphics = Graphics.FromImage(m_BackBuffer);

            m_Map = new Bitmap(m_Stage.m_Map.Width, m_Stage.m_Map.Height);
            m_MapGraphics = Graphics.FromImage(m_Map);

            m_BackBuffer2 = new Bitmap(m_Stage.m_Map.Width / 10, m_Stage.m_Map.Height / 10);
            m_BackBuffer2Graphics = Graphics.FromImage(m_BackBuffer2);

            pictureBox2.Width = m_Stage.m_Map.Width;
            pictureBox2.Height = m_Stage.m_Map.Height;
            pictureBox2.Image = m_BackBuffer;


        
            pictureBox1.Image = m_BackBuffer2;
        }



        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void loadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = Directory.GetCurrentDirectory();
            Dialog.DefaultExt = "xml";
            Dialog.Filter = "xml files (*.xml)|*.xml";
            Dialog.FilterIndex = 2;
            Dialog.RestoreDirectory = true;
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                m_Stage.LoadMapFile(Dialog.FileName);
                LOAD_InitalizeMap();
                LOAD_ClearState();
                DrawAllObject();
                Draw();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_bCreatedMap)
            {
                MessageBox.Show("맵을 생성해 주세요");
                return;
            }

            SaveFileDialog Dialog = new SaveFileDialog();
            Dialog.InitialDirectory = Directory.GetCurrentDirectory();
            Dialog.DefaultExt = "xml";
            Dialog.Filter = "xml files (*.xml)|*.xml";
            Dialog.FilterIndex = 2;
            Dialog.RestoreDirectory = true;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                CreateMapFile(Dialog.FileName);
            }
        }

        private void enemyDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetEnemyDialog Dialog = new SetEnemyDialog();
            Dialog.ShowDialog();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsEmptyMap())
            {
                if (MessageBox.Show("정보를 저장하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveFileDialog Dialog = new SaveFileDialog();
                    Dialog.InitialDirectory = Directory.GetCurrentDirectory();
                    Dialog.DefaultExt = "xml";
                    Dialog.Filter = "xml files (*.xml)|*.xml";
                    Dialog.FilterIndex = 2;
                    Dialog.RestoreDirectory = true;

                    if (Dialog.ShowDialog() == DialogResult.OK)
                    {
                        CreateMapFile(Dialog.FileName);
                    }
                }
            }
            CSetMapInfoDialog SetMapInfoDialog = new CSetMapInfoDialog();
            SetMapInfoDialog.ShowDialog();
        }

        private void CellbotMapTool_SizeChanged(object sender, EventArgs e)
        {

        }
        private void MoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentMode = eMode.MODE_SELECTEDOBJECT_MOVE;
            m_CursorImage = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject("SpawnPoint"));
            MakeTransparentSpwanImage();
        }

        private void moveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CurrentMode = eMode.MODE_SELECTEDOBJECT_MOVE;
            m_CursorImage = (System.Drawing.Bitmap)(Resource1.ResourceManager.GetObject(m_SelectedObject.m_BaseInfo.m_ImageName));
            MakeTransparentSpwanImage();
        }

        private void VibotMapTool_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        LOAD_ClearState();
                        DrawBackBuffer();
                    }
                    break;
                case Keys.Delete:
                    {
                        if (m_SelectedObject != null)
                        {
                            CComponent Component = m_Stage.GetComponent(m_SelectedObjectName);
                            Component.DeleteObject(m_SelectedObject);

                            m_SelectedObject = null;
                            m_SelectedObjectName = "";
                            if (m_Border != null)
                            {
                                m_Border.Dispose();
                                m_Border = null;
                            }
                            DrawAllObject();
                            Draw();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
