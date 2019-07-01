using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Xml;
using ObjectFactory;
using System.Diagnostics;
using Cellbot_Map_Tool;

namespace VibotCommon
{
    public struct _LAYERBASEINFO
    {
        public string m_ImageName;
        public Rectangle m_Rect;
        public float m_Level;

        public _LAYERBASEINFO(String objectName)
        {
            int Width = 15;
            int Height = 15;

            switch(objectName)
            {
                case "BackGround1":
                    Width = Resource1.BackGround1.Width;
                    Height = Resource1.BackGround1.Height;
                    break;
                case "BackGround2":
                    Width = Resource1.BackGround2.Width;
                    Height = Resource1.BackGround2.Height;
                    break;
                case "BackGround3":
                    Width = Resource1.BackGround3.Width;
                    Height = Resource1.BackGround3.Height;
                    break;
            }
            m_ImageName = "";
            m_Rect = new Rectangle(0, 0, Width, Height); // Defalult
            m_Level = 1; // Defalult Level = 1 
        }
    }

    public struct _EnemyInfo
    {
        public int m_Hp;
        public int m_DistanceOfAttack;
        public int m_TypeOfAttack;
        public int m_Speed;
        public int m_TypeOfMovement;
        public bool m_Guided;
        public  int m_AliveTick;
        public _EnemyInfo(int iNumber)
        {
            m_Hp = 0;
            m_DistanceOfAttack = 0;
            m_TypeOfAttack = 0;
            m_Speed = 0;
            m_TypeOfMovement = 0;
            m_Guided = false;
            m_AliveTick = 0;
        }
    }

    public class CObjectSpawnInfo
    {
     
            public string m_ObjectName;
            public int m_Probability;

            public List<CObjectSpawnInfo> m_ObjectSpwanInfo = new List<CObjectSpawnInfo>();
    }

    public class CEnemySpawnInfo
    {
        public struct _SpawnInfo
        {
            public string m_EnemyName;
            public int m_Probability;
            public _SpawnInfo(string EnemyName, int Probability)
            {
                m_EnemyName = EnemyName.ToString();
                m_Probability = Probability;
            }
        }

        public int m_Interval = 0;
        public int m_AliveTime = 0;
        public List<_SpawnInfo> m_SpwanInfo = new List<_SpawnInfo>();
    }

    public class CObjectCommon
    {
         public _LAYERBASEINFO m_BaseInfo;
         public CObjectCommon(String objectName)
         {
             m_BaseInfo = new _LAYERBASEINFO(objectName);
         }
    }

    public abstract class CComponent : IObject
    {
        public override IObject Instance()
        {
            return null;
        }

        public abstract void AddObject(CObjectCommon Object);
        public abstract List<CObjectCommon> GetObjectList();
        public abstract CObjectCommon GetIntersectObject(Rectangle Rect);
        public abstract bool DeleteObject(CObjectCommon Object);
        public abstract void Clear();
        public virtual void AddSpwanInfo(CObjectCommon Object, CEnemySpawnInfo EnemyInfo) { }
        public virtual void AddObjectSpwanInfo(CObjectCommon Object, CObjectSpawnInfo ObjectInfo) { }
        public virtual CEnemySpawnInfo GetSpwanInfo(CObjectCommon Object) { return null; }
        public virtual CObjectSpawnInfo GetObjectSpwanInfo(CObjectCommon Object) { return null; }

        public abstract bool IsEmpty();
    }

    public class CObject : CComponent
    {
        public List<CObjectCommon> m_ObjectList = new List<CObjectCommon>();
        
        public override IObject Instance()
        {
            return new CObject();
        }
        public override void AddObject(CObjectCommon Object)
        {
            m_ObjectList.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_ObjectList;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_ObjectList)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                {
                    return Object;
                }
            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_ObjectList.Remove(Object);
        }

        public override void Clear()
        {
    
            m_ObjectList.Clear();
        }



        public override bool IsEmpty()
        {
            return m_ObjectList.Count > 0 ? false : true;
        }

   
    }

    public class CEnemy : CComponent
    {
        public List<CObjectCommon> m_EnemyList = new List<CObjectCommon>();
        public Dictionary<CObjectCommon, CEnemySpawnInfo> m_SpawnInfoMap = new Dictionary<CObjectCommon, CEnemySpawnInfo>();
        public override IObject Instance()
        {
            return new CEnemy();
        }
        public override void AddObject(CObjectCommon Object)
        {
            m_SpawnInfoMap.Add(Object, new CEnemySpawnInfo());
            m_EnemyList.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_EnemyList;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_EnemyList)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                    return Object;
      
            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            m_SpawnInfoMap.Remove(Object);
            return m_EnemyList.Remove(Object);
        }

        public override void  Clear()
        {
            m_SpawnInfoMap.Clear();
 	        m_EnemyList.Clear();
        }

        public override void AddSpwanInfo(CObjectCommon Object, CEnemySpawnInfo EnemyInfo)
        {
            if (m_SpawnInfoMap.ContainsKey(Object))
            {
                foreach (KeyValuePair<CObjectCommon, CEnemySpawnInfo> SpwnInfoPair in m_SpawnInfoMap)
                {
                    if (SpwnInfoPair.Key == Object)
                    {
                        m_SpawnInfoMap.Remove(SpwnInfoPair.Key);
                        break;
                    }
                }
            }
            m_SpawnInfoMap.Add(Object, EnemyInfo);
        }

        public override CEnemySpawnInfo GetSpwanInfo(CObjectCommon Object)
        {
            foreach (KeyValuePair<CObjectCommon, CEnemySpawnInfo> SpwnInfoPair in m_SpawnInfoMap)
                if (SpwnInfoPair.Key == Object)
                    return (CEnemySpawnInfo)SpwnInfoPair.Value;
                

            return null;
        }
        public override bool IsEmpty()
        {
            return m_EnemyList.Count > 0 ? false : true;
        }
    }

   

    public class CBackGround : CComponent
    {
        public List<CObjectCommon> m_Tiles = new List<CObjectCommon>();
        public override IObject Instance()
        {
            return new CBackGround();
        }

        public override void AddObject(CObjectCommon Object)
        {
            m_Tiles.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_Tiles;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_Tiles)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                    return Object;
           
            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_Tiles.Remove(Object);
        }

        public override void Clear()
        {
            m_Tiles.Clear();
        }

        public override bool IsEmpty()
        {
            return m_Tiles.Count > 0 ? false : true;
        }
    }


    public class CWhiteCell : CObject
    {
        public List<CObjectCommon> m_WhiteCell = new List<CObjectCommon>();

        public override IObject Instance()
        {
            return new CWhiteCell();
        }

        public override void AddObject(CObjectCommon Object)
        {
            m_WhiteCell.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_WhiteCell;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_WhiteCell)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                    return Object;

            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_WhiteCell.Remove(Object);
        }

        public override void Clear()
        {
            m_WhiteCell.Clear();
        }

        public override bool IsEmpty()
        {
            return m_WhiteCell.Count > 0 ? false : true;
        }
    }

    public class CGoal_Zone : CComponent
    {
        public List<CObjectCommon> m_Goal_Zone = new List<CObjectCommon>();
        public override IObject Instance()
        {
            return new CGoal_Zone();
        }

        public override void AddObject(CObjectCommon Object)
        {
            m_Goal_Zone.Clear();
            m_Goal_Zone.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_Goal_Zone;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_Goal_Zone)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                    return Object;

            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_Goal_Zone.Remove(Object);
        }

        public override void Clear()
        {
            m_Goal_Zone.Clear();
        }

        public override bool IsEmpty()
        {
            return m_Goal_Zone.Count > 0 ? false : true;
        }
    }

    public class CRedBlood : CComponent
    {
        public List<Point> m_Road = new List<Point>();
        public List<CObjectCommon> m_RedBloodList = new List<CObjectCommon>();
        public override IObject Instance()
        {
            return new CRedBlood();
        }

        public override void AddObject(CObjectCommon Object)
        {
            m_RedBloodList.Clear();
            m_RedBloodList.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_RedBloodList;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_RedBloodList)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                {
                    return Object;
                }
            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_RedBloodList.Remove(Object);
        }

        public override void Clear()
        {
            m_Road.Clear();
            m_RedBloodList.Clear();
        }

        public override bool IsEmpty()
        {
            return m_RedBloodList.Count > 0 ? false : true;
        }
    }

    public class CVibot : CComponent
    {
        public List<CObjectCommon> m_Vibot = new List<CObjectCommon>();
        public override IObject Instance()
        {
            return new CVibot();
        }

        public override void AddObject(CObjectCommon Object)
        {
            m_Vibot.Clear();
            m_Vibot.Add(Object);
        }

        public override List<CObjectCommon> GetObjectList()
        {
            return m_Vibot;
        }

        public override CObjectCommon GetIntersectObject(Rectangle Rect)
        {
            foreach (CObjectCommon Object in m_Vibot)
            {
                if (Object.m_BaseInfo.m_Rect.IntersectsWith(Rect))
                    return Object;
            }
            return null;
        }

        public override bool DeleteObject(CObjectCommon Object)
        {
            return m_Vibot.Remove(Object);
        }

        public override void Clear()
        {
            m_Vibot.Clear();
        }

        public override bool IsEmpty()
        {
            return m_Vibot.Count > 0 ? false : true;
        }
    }

    public class CVibotFactory : CObjectFactory
    {
        public override void RegisterObject()
        {
            CEnemy Object1 = new CEnemy();
            AddObjectClass("Enemy", Object1.Instance);

            CVibot Object2 = new CVibot();
            AddObjectClass("Vibot", Object2.Instance);

            CRedBlood Object3 = new CRedBlood();
            AddObjectClass("RedBlood", Object3.Instance);

            CBackGround Object4 = new CBackGround();
            AddObjectClass("BackGround1", Object4.Instance);

            CBackGround Object5 = new CBackGround();
            AddObjectClass("BackGround2", Object5.Instance);

            CBackGround Object6 = new CBackGround();
            AddObjectClass("BackGround3", Object6.Instance);

            CObject Object7 = new CObject();
            AddObjectClass("Object", Object7.Instance);

            CGoal_Zone Object8 = new CGoal_Zone();
            AddObjectClass("Goal_Zone", Object8.Instance);

            CWhiteCell Object9 = new CWhiteCell();
            AddObjectClass("WhiteCell", Object9.Instance);
        }
    }

   

    public class CStage
    {
        public Size m_Map = new Size(0, 0);
        public String m_ImageName = "MainTile";
      //  public Size m_Tile = new Size(0, 0);

        private CVibotFactory m_VibotFactory = new CVibotFactory();

        public Dictionary<string, CComponent> m_Components = new Dictionary<string, CComponent>();

        public CComponent GetComponent(string name)
        {
            foreach (KeyValuePair<string, CComponent> Component in m_Components)
            {      if (Component.Key == name)
                    return (CComponent)Component.Value;
             }
            return null;
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, CComponent> Component in m_Components)
                Component.Value.Clear();
     
        }



        public void LoadMapFile(string FullPath)
        {
            Clear();

            XmlDocument MapFile = new XmlDocument();

            MapFile.Load(FullPath);
            XmlElement Root = MapFile.DocumentElement;
            XmlNodeList ChildNodes = Root.ChildNodes;
            foreach (XmlNode Node in ChildNodes)
            {
                switch (Node.Name)
                {
                    case "MapInfo" :
                        {
                            LoadMapInfoFromXml(Node);
                        }
                        break;
                    case "BackGround": //여기있네
                            LoadBackGroundInfoFromXml(Node);
                        break;
                    case "Vibot":
                        {
                            LoadVibotInfoFromXml(Node);
                        }
                        break;

                    case "RedBlood":
                        {
                            LoadRedBloodInfoFromXml(Node);
                        }
                        break;
                    case "WhiteCellPoints":
                        {
                            LoadWhiteCellsSpawnInfoFromXml(Node);

                        }
                        break;
                
                    case "SpawnPoints":
                        {
                            LoadEnemysSpawnInfoFromXml(Node);
                        }
                        break;
                    case "ObjectPoints":
                        LoadObjectInfoFromXml(Node);
                            break;
                
                      
                }
            }
        }

        private void LoadMapInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                switch (ChildNode.Name)
                {
                    case "Map_Width":
                        m_Map.Width = int.Parse(ChildNode.InnerText);
                        break;
                    case "Map_Height":
                        m_Map.Height = int.Parse(ChildNode.InnerText);
                        break;
                    case "MainTile":
                        m_ImageName = ChildNode.InnerText.ToString();
                        break;
                    case "Goal_Zone":
                        {
                            CComponent Component = GetComponent("Goal_Zone");

                            CObjectCommon TempObject = new CObjectCommon("Goal_Zone");
                            TempObject.m_BaseInfo.m_ImageName = "Goal_Zone";

                            string[] Rect = ChildNode.InnerText.Split(' ');
                            TempObject.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                            TempObject.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                            TempObject.m_BaseInfo.m_Rect.Width = 128; //일단 고정 숫자
                            TempObject.m_BaseInfo.m_Rect.Height = 128;

                            Component.AddObject(TempObject);
                        }

                        break;


                }
            }


        }

        private void LoadBackGroundInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("BackGround");

            foreach (XmlNode ChildNode in ChildNodes)
            {
                CObjectCommon Object = new CObjectCommon("BackGround");
                switch (ChildNode.Name)
                {
                    case "Image":
                        Object.m_BaseInfo.m_ImageName = ChildNode.InnerText.ToString();
                        break;

                    case "Location":
                        {
                            string[] Rect = ChildNode.InnerText.Split(' ');
                            Object.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                            Object.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                            Object.m_BaseInfo.m_Rect.Width = int.Parse(Rect[2]);
                            Object.m_BaseInfo.m_Rect.Height = int.Parse(Rect[3]);
                        }
                        break;
                 
                }
                Component.AddObject(Object);
            }
        }

        private void LoadVibotInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("Vibot");
            CObjectCommon Object = new CObjectCommon("Vibot");
            foreach (XmlNode ChildNode in ChildNodes)
            {
                switch (ChildNode.Name)
                {
                    case "Image":
                        Object.m_BaseInfo.m_ImageName = ChildNode.InnerText.ToString();
                        break;

                    case "Location":
                        {
                            string[] Rect = ChildNode.InnerText.Split(' ');
                            Object.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                            Object.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                            Object.m_BaseInfo.m_Rect.Width = 60;
                            Object.m_BaseInfo.m_Rect.Height = 60;
                        }
                        break;
                    default:
                        break;
                }
            }
            Component.AddObject(Object);
        }

        private void LoadRedBloodInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("RedBlood");
            CRedBlood RedBlood = (CRedBlood)Component;
            CObjectCommon Object = new CObjectCommon("RedBlood");
            foreach (XmlNode ChildNode in ChildNodes)
            {
                switch (ChildNode.Name)
                {
                    case "Image":
                        {
                            Object.m_BaseInfo.m_ImageName = ChildNode.InnerText.ToString();
                        }
                        break;
                    case "Location":
                        {
                            string[] Rect = ChildNode.InnerText.Split(' ');
                            Object.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                            Object.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                            Object.m_BaseInfo.m_Rect.Width = 30;
                            Object.m_BaseInfo.m_Rect.Height = 30;

                        }
                        break;
                    case "Road":
                        {
                            XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
                            foreach (XmlNode ChildNode2 in ChildNodes2)
                            {
                                if (ChildNode2.Name == "Pos")
                                {
                                    string[] Rect = ChildNode2.InnerText.Split(' ');
                                    Point Pos = new Point(int.Parse(Rect[0]), int.Parse(Rect[1]));
                                    RedBlood.m_Road.Add(Pos);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            Component.AddObject(Object);
        }

        private void LoadEnemysSpawnInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("Enemy");
            CEnemy SpawnInfos = (CEnemy)Component;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "SpawnPoint")
                {
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
                    CObjectCommon Object = new CObjectCommon("SpawnPoint");
                    CEnemySpawnInfo SpawnInfo = new CEnemySpawnInfo();
                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {
                            case "Location":
                                {
                                    string[] Rect = ChildNode2.InnerText.Split(' ');
                                    Object.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                                    Object.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                                    Object.m_BaseInfo.m_Rect.Width = 45;
                                    Object.m_BaseInfo.m_Rect.Height = 45;
                                }
                                break;
                            case "SpawnTick":
                                {
                                    SpawnInfo.m_Interval = int.Parse(ChildNode2.InnerText);
                                }
                                break;
                            case "AliveTime":
                                {
                                    SpawnInfo.m_AliveTime = int.Parse(ChildNode2.InnerText);
                                }
                                break;
                            case "Enemys":
                                {
                                    XmlNodeList ChildNodes3 = ChildNode2.ChildNodes;
                                    foreach (XmlNode ChildNode3 in ChildNodes3)
                                    {
                                        if (ChildNode3.Name == "Enemy")
                                        {
                                            CEnemySpawnInfo._SpawnInfo Enemy = new CEnemySpawnInfo._SpawnInfo();
                                            XmlNodeList ChildNodes4 = ChildNode3.ChildNodes;
                                            foreach (XmlNode ChildNode4 in ChildNodes4)
                                            {
                                                switch (ChildNode4.Name)
                                                {
                                                    case "Name":
                                                        {
                                                            Enemy.m_EnemyName = ChildNode4.InnerText;
                                                        }
                                                        break;
                                                    case "Probability":
                                                        {
                                                            Enemy.m_Probability = int.Parse(ChildNode4.InnerText);
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            SpawnInfo.m_SpwanInfo.Add(Enemy);
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    Component.AddObject(Object);
                    Component.AddSpwanInfo(Object, SpawnInfo);
                }
            }
        }

        private void LoadWhiteCellsSpawnInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("WhiteCell");


            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "WhiteCell")
                {

                    CObjectCommon WhiteCell = new CObjectCommon("WhiteCell");
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;

                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {
                            case "Level":
                                WhiteCell.m_BaseInfo.m_Level = float.Parse(ChildNode2.InnerText);
                                break;

                            case "Location":
                                {
                                    string[] Rect = ChildNode2.InnerText.Split(' ');
                                    WhiteCell.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                                    WhiteCell.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                                    WhiteCell.m_BaseInfo.m_Rect.Width = 50;
                                    WhiteCell.m_BaseInfo.m_Rect.Height = 50;
                                }
                                break;
                        
                            
                        }

                    }
                    WhiteCell.m_BaseInfo.m_ImageName = "WhiteCell";
                    Component.AddObject(WhiteCell);
                }
            }
      

    }


        private void LoadObjectInfoFromXml(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            CComponent Component = GetComponent("Object");



                    foreach (XmlNode ChildNode in ChildNodes)
                    {

                        if (ChildNode.Name == "Object")
                        {
                            XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
                            CObjectCommon Object = new CObjectCommon("Object");

                            foreach (XmlNode ChildNode2 in ChildNodes2)
                            {
                                switch (ChildNode2.Name)
                                {

                                    case "Name":

                                        Object.m_BaseInfo.m_ImageName = ChildNode2.InnerText;
                                        break;

                                    case "Location":
                                        string[] Rect = ChildNode2.InnerText.Split(' ');
                                        Object.m_BaseInfo.m_Rect.X = int.Parse(Rect[0]);
                                        Object.m_BaseInfo.m_Rect.Y = int.Parse(Rect[1]);
                                        Object.m_BaseInfo.m_Rect.Width = 30;
                                        Object.m_BaseInfo.m_Rect.Height = 30;
                                        break;
                                }
                            }
                            Component.AddObject(Object);
                        }
            

            
            }

        }

        public void RegisterComponent()
        {
            m_VibotFactory.RegisterObject();
            m_Components.Add("BackGround1", (CComponent)m_VibotFactory.CreateObjectClass("BackGround1"));
            m_Components.Add("BackGround2", (CComponent)m_VibotFactory.CreateObjectClass("BackGround2"));
            m_Components.Add("BackGround3", (CComponent)m_VibotFactory.CreateObjectClass("BackGround3"));
            m_Components.Add("Enemy", (CComponent)m_VibotFactory.CreateObjectClass("Enemy"));
            m_Components.Add("RedBlood", (CComponent)m_VibotFactory.CreateObjectClass("RedBlood"));
            m_Components.Add("Vibot", (CComponent)m_VibotFactory.CreateObjectClass("Vibot"));
            m_Components.Add("Goal_Zone", (CComponent)m_VibotFactory.CreateObjectClass("Goal_Zone"));
            m_Components.Add("Object", (CComponent)m_VibotFactory.CreateObjectClass("Object"));
            m_Components.Add("WhiteCell", (CComponent)m_VibotFactory.CreateObjectClass("WhiteCell"));
        }
    }
}
