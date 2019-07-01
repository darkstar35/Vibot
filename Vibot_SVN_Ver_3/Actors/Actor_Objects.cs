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

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace Vibot.Actors
{
    public class Actor_Objects : IActor
    {

        public List<Stuff> NaturalObjectList = new List<Stuff>();

        private Vector2 Object_Spwan_Position;
        protected String Name;



        public Actor_Objects(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
        }


        public void OnDraw(GameTime gameTime)
        {

            foreach (Stuff Object in NaturalObjectList)      //        if (Object != null)
                Object.OnDraw(gameTime);


        }


        public void OnUpdate(GameTime gameTime)
        {

            foreach (Stuff Objects in NaturalObjectList)       //      if (Objects != null)
            {
                Objects.OnUpdate(gameTime);
                if (Objects.CollisionToEndofWorld())
                {
                    world.RemoveBody(Objects.body);
                    NaturalObjectList.Remove(Objects);
                    break;
                }
            }
        }




        public override void Destory()
        {

            if (NaturalObjectList.Count > 0)
            {
                for (int i = NaturalObjectList.Count-1; i >=0 ; i--)
                    world.RemoveBody(NaturalObjectList[i].body);

                NaturalObjectList.Clear();
            }
        }

        public override void LoadDataFromMap(XmlNode Node)
        {
            XmlNodeList ChildNodes = Node.ChildNodes;
            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "Object")
                {
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;

                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {

                            case "Name":
                                Name = ChildNode2.InnerText;
                   
                                break;
                            case "Location":
                                {
                                    string[] Word = ChildNode2.InnerText.Split(' ');
                                    Object_Spwan_Position.X = float.Parse(Word[0]);
                                    Object_Spwan_Position.Y = float.Parse(Word[1]);
                                }
                                break;


              

                        }
            
                    }
                    switch (Name)
                    {
                        case "Cole":

                            Chole_Object Stuff = new Chole_Object(m_GraphicDevice, m_ContentManager, m_SpriteBatch, Object_Spwan_Position);

                            Stuff.CollisionCategories = Category.Cat5;
                            Stuff.CollidesWith = Category.All | ~Category.Cat31;
                            NaturalObjectList.Add(Stuff);
                            break;

                    }



                }
            }
        }

    }
}
