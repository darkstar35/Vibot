using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vibot.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;


namespace Vibot.Actors
{
    class Actor_Items : IActor
    {
       // public static CElement[] Item_Element = new CElement[10];

     //   public override IObject Instance() { return (IObject)new Actor_Items(); }
        private List<IStuff> Drop_BioGageList = new List<IStuff>();

        public Texture2D[] Ball_Image = new Texture2D[3];
        public Point Position;
        public String Name;

        public float DropTime { get; set; }

        public Actor_Items()
        {

        }


        public Actor_Items(Vector2 position, int itemnum)
        {
            if(itemnum == 0)
                SetUpPhysics(world, position, Ball_Image[itemnum].Width, Ball_Image[itemnum].Height, 10f);
            else  if(itemnum == 1)
                SetUpPhysics(world, position, Ball_Image[itemnum].Width, Ball_Image[itemnum].Height, 10);
            else  if(itemnum == 2)
                SetUpPhysics(world, position, Ball_Image[itemnum].Width, Ball_Image[itemnum].Height, 10f);
        }

  
        public override void OnInitalize(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, SceneManager SceneManager, CObjectFactory ObjectFactory)
        {
                     base.OnInitalize(GraphicDevice, ContentManager, SpriteBatch, SceneManager, ObjectFactory);
        }


   
    
        public override void OnUpdate(GameTime gameTime)  //움직이기
        {

            foreach (IStuff Item in Drop_BioGageList)
            {
                if (Item != null)
                {
                    Item.OnUpdate(gameTime);
                    Item.PickUp_Item();
                }
            }

        }

        public override void Destory()
        {
            throw new NotImplementedException();
        }

        public override void OnDraw(GameTime gameTime)
        {

            foreach (CMsg<eSystemMsg> Msg in System_MsgQueue)
            {
                DropTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                HandleMessage(Msg);
            }
            System_MsgQueue.Clear();
        



            foreach (IStuff Item in Drop_BioGageList)
            {
                if (Item != null)
                    Item.OnDraw(gameTime);
            }


        }


        public override void HandleMessage(CMsg<eSystemMsg> Msg) //
        {
            switch (Msg.m_Msg)
            {
                case eSystemMsg.ACTORMSG_ITEM_PICKUP:
                    {
                        BioGage Item = (BioGage)Msg.m_Arg1;
                        Drop_BioGageList.Remove(Item);
                    }
                    break;
         
                case eSystemMsg.ACTORMSG_VIRUS_DIE:
                    {/*
                        IObject Object = m_ObjectFactory.CreateObjectClass("Testball");

                        if (Object != null)
                        {

                            IStuff Item = (IStuff)Object;
                            Item.OnInitalize(m_GraphicDevice, m_ContentManager, m_SpriteBatch, m_ObjectFactory, (IActor)this);
                            Item.HandleMessage(new CMsg<eStuffMsg>(eStuffMsg.ItemMSG_INITALIZE, Position));
                            Item.CollisionCategories = Category.Cat4;
                            Item.CollidesWith = Category.Cat4;
                            m_ItemList.Add(Item);


                        } 

                        */
                    }
                    break;
  
                default:
                    break;
            }
        }

        private void LoadSetFromXML(XmlNode Node, CMsg<eSystemMsg> Msg)
        {
        
            /*
            XmlNodeList ChildNodes = Node.ChildNodes;
            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "Items")
                {
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;

                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {
                            case "Location":
                                {
                                    string[] Word = ChildNode2.InnerText.Split(' ');
                                    Position.X = int.Parse(Word[0]);
                                    Position.Y = int.Parse(Word[1]);
                                }
                                break;

                            case "Name":
                                {
                                    Name = ChildNode2.InnerText;

                                }
                                break;
        
                            default:
                                break;
                        }

                    }
          */
            
    
                  
      
                }




            }
        }
