using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

using Vibot.Base;
using Vibot.Stuffs;

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


namespace Vibot.Actors
{

    class Actor_SpawnPointManager : IActor
    {

        public List<VirusSpawnPoint> AllofSpawnList = new List<VirusSpawnPoint>();
        public List<Stuff> AllofVirusList = new List<Stuff>();

        public Texture2D[] Spawn_image;
        public Texture2D Arrow_Texture;


        public Vector2 NearSecurying = Vector2.Zero;


        public override void LoadDataFromMap(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            Vector2 SpawnPostion = Vector2.Zero;

            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "SpawnPoint")
                {
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
                  
                    VirusSpawnPoint Spawn = new VirusSpawnPoint(m_GraphicDevice,m_ContentManager, m_SpriteBatch);
                                                                                 
                                                                     
                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {
                        
                            case "Level":
                                Spawn.m_Grade = int.Parse(ChildNode2.InnerText);
                                break;

                            case "Location":
                                {
                                    string[] Rect = ChildNode2.InnerText.Split(' ');
                                    SpawnPostion.X = float.Parse(Rect[0]);
                                    SpawnPostion.Y = float.Parse(Rect[1]);
                                }
                                break;
                            case "SpawnTick":
                                Spawn.m_Interval = int.Parse(ChildNode2.InnerText);
                                break;
                            case "AliveTime":
                                Spawn.m_AliveTime = int.Parse(ChildNode2.InnerText);
                                break;
                            case "SpawnStart":
                                Spawn.m_StartTimeTick = int.Parse(ChildNode2.InnerText);
                                break;
                            case "HP":
                                Spawn.m_HP = int.Parse(ChildNode2.InnerText);
                                break;

                            case "Virus":
                                {
                                    string Name = "";
                                    int Probability = 0;
                                  

                                    XmlNodeList ChildNodes3 = ChildNode2.ChildNodes;

                                    foreach (XmlNode ChildNode3 in ChildNodes3)
                                    {
                                        switch (ChildNode3.Name)
                                        {
                                            case "Name":
                                                Name = ChildNode3.InnerText;
                                                break;
                                            case "Probability":
                                                Probability = int.Parse(ChildNode3.InnerText);
                                                break;
                                            case "Direction":
                                                Spawn.m_Degree = float.Parse(ChildNode3.InnerText);
                                                break;
                                        }

                                        float DirectionX = (float)Math.Cos(MathHelper.ToRadians(Spawn.m_Degree));
                                        float DirectionY = (float)Math.Sin(MathHelper.ToRadians(Spawn.m_Degree));

                                        Vector2 DirectionVector = new Vector2(DirectionX, DirectionY) * -1;


                                        if (Name == "대장균")
                                        {
                                            Spawn.m_VirusInfoList.Add(new VirusInfo(Name, Probability, DirectionVector));
                                        }
                                        else
                                        {
                                            Spawn.m_VirusInfoList.Add(new VirusInfo(Name, Probability, DirectionVector));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                 Spawn.SetUpPhysics(world, SpawnPostion, 30, 5);  // original radius is 100
                 AllofSpawnList.Add(Spawn);
                }
            }
        }


        Actor_RedBlood RedBlood;

        public Actor_SpawnPointManager(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Actor_RedBlood RedBlood)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
            this.RedBlood = RedBlood;

            Spawn_image = new Texture2D[5];
            Spawn_image[0] = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\virusspwan0");
            Spawn_image[1] = m_ContentManager.Load<Texture2D>("Sprites\\Stages\\virusspwan1");
            Arrow_Texture = m_ContentManager.Load<Texture2D>("Sprites\\UI\\arrow");

        }



       public void OnDraw(GameTime gameTime, Actor_RedBlood Actor_RedBlood)
       {
      
           foreach (VirusSpawnPoint VirusSpawn in AllofSpawnList)
           {
               m_SpriteBatch.Draw(Spawn_image[VirusSpawn.m_Grade], VirusSpawn.bodyViewPortPosition, null, Color.White,
                   0f,
                   new Vector2(Spawn_image[VirusSpawn.m_Grade].Width / 2, Spawn_image[VirusSpawn.m_Grade].Height / 2)
                  , VirusSpawn.m_HP/100, SpriteEffects.None, 0f);

         
                   m_SpriteBatch.Draw(Arrow_Texture, VirusSpawn.bodyViewPortPosition, null, Color.White,
                           MathHelper.ToRadians(VirusSpawn.m_Degree - 90),  // 맵툴에서 시작점이 80도에서 시작하기 떄문에 이레해야된다 // 2012 04 22        
                  new Vector2(Arrow_Texture.Width/2,Arrow_Texture.Height/2), 1f, SpriteEffects.None, 0f);
           }
          
       }


       public void OnUpdate(GameTime gameTime)
       {
           IntervalSpawn(gameTime);
       }

       public void IntervalSpawn(GameTime gameTime)
        {
            foreach (VirusSpawnPoint Spawn in AllofSpawnList)
            {
                if (Spawn.m_StartTimeTick > 0)
                    Spawn.m_StartTimeTick -= gameTime.TotalGameTime.Milliseconds;


                else if (Spawn.m_StartTimeTick <= 0)
                {
                    Spawn.m_AliveTick += gameTime.TotalGameTime.Milliseconds;
                    Spawn.m_IntervalTick += gameTime.TotalGameTime.Milliseconds;

                    if (Spawn.m_IntervalTick > Spawn.m_Interval)
                    {
                        if (RedBlood.BloodCore_State == Actor_RedBlood.eREDBLOOD_State.REDBLOOD_STOP) //어그로 코드 
                        {
                            if (Vector2.Distance(Spawn.bodyWorldPosition, RedBlood.Blood_Core_Position) < 800)   // 800px 안에 REDBLOOD가 STOP중이면 

                                Spawn.SpawnVirus(AllofVirusList, (RedBlood.Blood_Core_Position - Spawn.bodyWorldPosition) /     // REDBLOOD를 향해 공격~~!
                              (RedBlood.Blood_Core_Position - Spawn.bodyWorldPosition).Length()
                              );
                        }
                        else
                            Spawn.SpawnVirus(AllofVirusList);

                        Spawn.m_IntervalTick = 0;
                    }
                    if (Spawn.m_AliveTick > Spawn.m_AliveTime || Spawn.m_HP < 0)
                    {
                        world.RemoveBody(Spawn.body);
                        AllofSpawnList.Remove(Spawn);
              
                        break;
                    }
                }
            }
        }

        public void IntervalSpawn(GameTime gameTime, Vector2 NearSecurying)
        {

            foreach (VirusSpawnPoint Spawn in AllofSpawnList)
            {
                if (Spawn.m_StartTimeTick > 0)
                    Spawn.m_StartTimeTick -= gameTime.TotalGameTime.Milliseconds;


                else if (Spawn.m_StartTimeTick <= 0)
                {
                    Spawn.m_AliveTick += gameTime.TotalGameTime.Milliseconds;
                    Spawn.m_IntervalTick += gameTime.TotalGameTime.Milliseconds;

                    if (Spawn.m_IntervalTick > Spawn.m_Interval)
                    {
  
                        Spawn.SpawnVirus(AllofVirusList, NearSecurying);
                        Spawn.m_IntervalTick = 0;

                    }
                    if (Spawn.m_AliveTick > Spawn.m_AliveTime || Spawn.m_HP < 0)
                    {
                        world.RemoveBody(Spawn.body);
                        AllofSpawnList.Remove(Spawn);
                   
                        break;
                    }
                }
            }
        }


        public override void Destory()
        {
            if (AllofSpawnList.Count > 0)
            {
            for (int i = AllofSpawnList.Count -1 ; i >= 0 ; --i)
                world.RemoveBody(AllofSpawnList[i].body);
  
            AllofSpawnList.Clear();  
            }
        }

    }

    public class VirusInfo
    {

        public string m_Name;
        public int m_Probability;      
        public Vector2 m_DirectionVector;

        public VirusInfo(string Name, int Probability, Vector2 DirectionVector)
        {
            m_Name = Name;
            m_Probability = Probability;
            m_DirectionVector = DirectionVector;
        }

    }

    public class VirusSpawnPoint : IObject
    {

        public List<VirusInfo> m_VirusInfoList = new List<VirusInfo>();

        public int m_Grade = 0;
        public int m_Interval = 0;
        public int m_AliveTime = 0;
        public float m_Degree = 0f;
        public int m_AliveTick = 0;
        public int m_IntervalTick = 0;

        public int m_StartTimeTick = 0;
        public int m_StartTime = 0;

        public bool FocusOnSecurying = false;

        public VirusSpawnPoint(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            // TODO: Complete member initialization
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
        }

        public override void SetUpPhysics(World world, Vector2 position, float radius, float mass)
        {

            body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(radius), mass, ConvertUnits.ToSimUnits(position));
            body.BodyType = BodyType.Static;
            body.Restitution = 0f;
            body.CollisionCategories = Category.Cat7;
            body.CollidesWith = ~Category.All;


        }


        public void SpawnVirus(List<Stuff> m_VirusList)
        {
            int RandomNumber = Rand.Next(100);
            int Min = 0;
            int Max = 0;


            foreach (VirusInfo VirusInfo in m_VirusInfoList)
            {
                Max += VirusInfo.m_Probability;

                if (RandomNumber >= Min && RandomNumber < Max)
                {
                    switch (VirusInfo.m_Name)
                    {
                        case "H5":
                            H5 h5 = new H5(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(h5);
                            break;

                        case "Flu":
                            Flu flu = new Flu(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(flu);
                            break;

                        case "대장균":
                            Bacillus bacil = new Bacillus(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(bacil);
                            break;

                        case "악성세포":
                            Malignant mali = new Malignant(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(mali);
                            break;

                         case "Prein":
                            Preinvasive Prein = new Preinvasive(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(Prein);
                            break;
                    }


                }
                Min += VirusInfo.m_Probability;
            }
        }

        public void SpawnVirus(List<Stuff> m_VirusList, Vector2 NearSecuryingDic)
        {
            int RandomNumber = Rand.Next(100);
            int Min = 0;
            int Max = 0;


            foreach (VirusInfo VirusInfo in m_VirusInfoList)
            {
                Max += VirusInfo.m_Probability;

                if (RandomNumber >= Min && RandomNumber < Max)
                {

                    switch (VirusInfo.m_Name)
                    {
                        case "H5":
                            H5 h5 = new H5(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, NearSecuryingDic);
                            m_VirusList.Add(h5);
                            break;

                        case "Flu":
                            Flu flu = new Flu(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, NearSecuryingDic);
                            m_VirusList.Add(flu);
                            break;

                        case "대장균":
                            Bacillus bacil = new Bacillus(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, NearSecuryingDic);
                            m_VirusList.Add(bacil);
                            break;

                        case "악성세포":
                            Malignant mali = new Malignant(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, NearSecuryingDic);
                            m_VirusList.Add(mali);
                            break;

                        case "Prein":
                            Preinvasive Prein = new Preinvasive(m_GraphicDevice, m_ContentManager, m_SpriteBatch, bodyWorldPosition, VirusInfo.m_DirectionVector);
                            m_VirusList.Add(Prein);
                            break;
                    }


                }
                Min += VirusInfo.m_Probability;
            }
        }

    }

}
