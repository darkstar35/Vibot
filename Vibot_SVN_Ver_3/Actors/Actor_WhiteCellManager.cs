using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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


using Vibot.Base;
using Vibot.Stuffs;

namespace Vibot.Actors
{
  public class Actor_WhiteCellManager : IActor
    {
        public List<WhiteCell> WhiteCell_List = new List<WhiteCell>();

        private Actor_RedBlood RedBlood;

        public Actor_WhiteCellManager(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Actor_RedBlood RedBlood)
        {
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;
            this.RedBlood = RedBlood;
        }


        public void OnUpdate(GameTime gameTime)
        {

         //   if (WhiteCell_List.Count != 0)
                for (int i = WhiteCell_List.Count - 1; i >= 0; i--)
                {
                    WhiteCell_List[i].OnUpdate(gameTime, RedBlood);
                    if (WhiteCell_List[i].m_HP <= 0 )
                    {

                        world.RemoveBody(WhiteCell_List[i].body);
                        WhiteCell_List.RemoveAt(i);
                        break;
                    }
                }

        }


        public void OnDraw(GameTime gameTime)
        {
           // if (WhiteCell_List.Count != 0)
                for (int i = WhiteCell_List.Count - 1; i >= 0; i--)
                    WhiteCell_List[i].OnDraw(gameTime);

        }

        public override void Destory()
        {
            if (WhiteCell_List.Count > 0)
            {
                for (int i = WhiteCell_List.Count - 1; i >= 0; i--)
                    world.RemoveBody(WhiteCell_List[i].body);
              

                WhiteCell_List.Clear();
            }
        }


        public override void LoadDataFromMap(XmlNode Node)
        {
            Vector2 Position = Vector2.Zero;
            int Level = 0;


            XmlNodeList ChildNodes = Node.ChildNodes;
            foreach (XmlNode ChildNode in ChildNodes)
            {
                if (ChildNode.Name == "WhiteCell")
                {
                    XmlNodeList ChildNodes2 = ChildNode.ChildNodes;

                    foreach (XmlNode ChildNode2 in ChildNodes2)
                    {
                        switch (ChildNode2.Name)
                        {
                            case "Level":
                                Level = int.Parse(ChildNode2.InnerText);
                                break;

                            case "Location":
                                string[] Word = ChildNode2.InnerText.Split(' ');
                                Position.X = float.Parse(Word[0]);
                                Position.Y = float.Parse(Word[1]);
                                break;
                        }



                    }
                    WhiteCell WhiteCell = new WhiteCell(Level, Position, m_GraphicDevice, m_ContentManager, m_SpriteBatch);
                    WhiteCell_List.Add(WhiteCell);
                }
            }
        }


    }
}
