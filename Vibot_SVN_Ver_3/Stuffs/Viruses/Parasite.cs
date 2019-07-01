using System;
using System.Collections.Generic;
using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Vibot.Base;
using Vibot.Actors;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;


namespace Vibot.Stuffs
{
    public class Parasite : Stuff
    {
       Texture2D Bodys_Texture;
       Texture2D[] HitHeadTexture = new Texture2D[3]; //첫번째 충돌시 이미지

        public List<Body> _ParasiteBodies;
        const int segmentCount = 7;

        const float Maxium_Speed = 3f;
        private Vector2 Headorigin;
        private Vector2 Bodyorigin;
        private Body TempBody;

        public float TimetoMove = 0;
        private int randnum = 0;



        private float collTimerWithVibot = 0.0f; //봇과의 충돌 타이머
        private float minCollTimer = 1.0f;
        public int HitCnt;
        public const int MaxHP= 3;








        public Parasite(GraphicsDevice GraphicDevice, ContentManager ContentManager, SpriteBatch SpriteBatch, Vector2 Pos, float m)
            : base(GraphicDevice, ContentManager, SpriteBatch)
        {
            position = Pos;
            mass = m;
            m_GraphicDevice = GraphicDevice;
            m_ContentManager = ContentManager;
            m_SpriteBatch = SpriteBatch;

            HitCnt = 0;

            m_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Viruses\\parasite_head");
            Bodys_Texture = m_ContentManager.Load<Texture2D>("Sprites\\Viruses\\parasite_body");
            HitHeadTexture[0] = m_ContentManager.Load<Texture2D>("Sprites\\Viruses\\parasite_head1");

            Bodyorigin = new Vector2(Bodys_Texture.Width / 2, Bodys_Texture.Height / 2);

             Body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(m_Texture.Width), ConvertUnits.ToSimUnits(m_Texture.Height), mass, ConvertUnits.ToSimUnits(position));
             Body.BodyType = BodyType.Dynamic;
             Body.Friction = 10f;
             Body.CollisionCategories = Category.Cat20;
             Body.CollidesWith = Category.Cat2 | ~Category.All; //~Category.Cat20 | ~Category.Cat31; // | ~Category.All;

            _ParasiteBodies = new List<Body>(segmentCount);

            PolygonShape shape = new PolygonShape(1f);
            //   shape.SetAsBox(1.0f, 0.125f);
            shape.SetAsBox(1.0f, 0.125f);
            Body prevBody = Body;
            Headorigin = new Vector2(m_Texture.Width / 2, m_Texture.Height / 2);

            for (int i = 0; i < segmentCount; i++)
            {

                TempBody = new Body(world);
                TempBody.BodyType = BodyType.Dynamic;
                TempBody.Position = ConvertUnits.ToSimUnits(new Vector2(position.X + 100 + 100f * i, position.Y));
                TempBody.Mass = mass;
                TempBody.Friction = 10f;
             //   MiddleBody.CollisionCategories = Category.Cat20;
                TempBody.CollidesWith = Category.Cat2 | ~Category.All; // ~Category.Cat20 | ~Category.Cat31; 
                // MiddleBody.Position = ConvertUnits.ToSimUnits(new Vector2(position.X + m_Texture.Width + 100f * i, position.Y));
                Fixture fix = TempBody.CreateFixture(shape);
                fix.Friction = 0.5f;


                JointFactory.CreateRevoluteJoint(world, prevBody, TempBody, -Vector2.UnitX);
                prevBody = TempBody;
                _ParasiteBodies.Add(TempBody);
            }

        }


 

        public override void OnDraw(GameTime gameTime)
        {
            //   m_SpriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(body.Position) - cCamera.FollwingCamera,Color.White);
            //   m_SpriteBatch.Draw(m_Texture, ConvertUnits.ToDisplayUnits(_ParasiteBodies[0].Position) - cCamera.FollwingCamera, Color.White);

            if (CollisionToVibot())
            {
                m_SpriteBatch.Draw(HitHeadTexture[0], bodyViewPortPosition, null, Color.White, Body.Rotation, Headorigin, m_HP, SpriteEffects.None, 0f);
            
            }
            else
                m_SpriteBatch.Draw(m_Texture, bodyViewPortPosition,
                                               null,
                                               Color.White, Body.Rotation, Headorigin, m_HP,
                                               SpriteEffects.None, 0f);

            for (int i = 0; i < _ParasiteBodies.Count; i++)
                m_SpriteBatch.Draw(Bodys_Texture, cCamera.Transform(ConvertUnits.ToDisplayUnits(_ParasiteBodies[i].Position)),
                                               null,
                                               Color.White, _ParasiteBodies[i].Rotation, Bodyorigin, m_HP,
                                               SpriteEffects.None, 0f);
        }


        public override bool CollisionToVibot()
        {
            //////////////////////// VIBOT 충돌  후 사망 처리PART  ////////////////////////////////////

            if (HitCnt >= MaxHP)
                m_HP -= 0.05f;
    

            if (Actor_Vibot.Vibot_Laser_Element.m_Sphere.Intersects(new BoundingSphere(new Vector3(bodyWorldPosition.X, bodyWorldPosition.Y, 0), m_Texture.Width / 2.14f))) //
            {
                if (collTimerWithVibot >= minCollTimer)
                {
                  // Debug.WriteLine("충돌2");
                    collTimerWithVibot = 0.0f;
                    HitCnt++;
                }
                return true;
            }
            else
                return false;

        }

        bool TurnXDirection = false;
        bool TurnYDirection = false;

        public override void OnUpdate(GameTime gameTime)  //움직이기
        {
                Random WaveRand = new Random(   (int) (gameTime.TotalGameTime.Milliseconds*1000) );


            TimetoMove += (float)gameTime.ElapsedGameTime.TotalSeconds;
            collTimerWithVibot  += (float)gameTime.ElapsedGameTime.TotalSeconds;

          
          //      Body.ApplyLinearImpulse(new Vector2(Rand.Next(10, 20)));
          //      Body.ApplyLinearImpulse(new Vector2(Rand.Next(-20, -10), 0));
              if (bodyWorldPosition.X - 100 > cCamera.MapSize.X)
              {
                  Body.LinearVelocity = Vector2.Zero;
                  Body.Position = new Vector2(_ParasiteBodies[0].Position.X - 1.2f, Body.Position.Y);
                  Body.Rotation +=(float) Math.PI;
                  TurnXDirection = false;
              }
              else if (bodyWorldPosition.X + 100 < 100)
              {
                  Body.LinearVelocity = Vector2.Zero; 
                  Body.Position = new Vector2(_ParasiteBodies[_ParasiteBodies.Count - 1].Position.X+1.2f, Body.Position.Y);
                  Body.Rotation +=(float) Math.PI;
                  TurnXDirection = true;
              
              }


              if (bodyWorldPosition.Y - 100 > cCamera.MapSize.Y)
              {
                  Body.LinearVelocity = Vector2.Zero;
                  Body.Position = new Vector2(Body.Position.X, _ParasiteBodies[_ParasiteBodies.Count - 1].Position.Y - 1.2f);
                  Body.Rotation += (float)Math.PI / 2;
                  TurnYDirection = false;
              }
              else if (bodyWorldPosition.Y + 100 < 100)
              {
                  Body.LinearVelocity = Vector2.Zero;
                  Body.Position = new Vector2( Body.Position.Y, _ParasiteBodies[0].Position.X + 1.2f);
                  Body.Rotation += (float)Math.PI/2;
                  TurnYDirection = true;

              }
            //  if (bodyWorldPosition.Y < 100)
            //      Body.ApplyLinearImpulse(
            //                new Vector2(0, Rand.Next(10, 20)));
            //  else if (bodyWorldPosition.Y + 100 > cCamera.MapSize.Y)
            //      Body.ApplyLinearImpulse(
            //           new Vector2(0, Rand.Next(-20, -10)));



           
           if (TimetoMove > 1)
           {
               randnum = WaveRand.Next(0, segmentCount);
               if (TurnXDirection)
                   Body.ApplyLinearImpulse(new Vector2(100, 0));
               else
                   Body.ApplyLinearImpulse(new Vector2(-100, 0));
               
          //      switch (Rand.Next(0, 4))
          //      {
          //          case 0:
          //              for (int i = randnum; i >= 0; --i)    
          //              _ParasiteBodies[i].ApplyTorque(30);
          //              Body.ApplyLinearImpulse(new Vector2(-80, 0));
          //
          //
          //              break;
          //
          //          case 1:
          //              for (int i = randnum; i >= 0; --i)  
          //              _ParasiteBodies[i].ApplyTorque(-30); 
          //              Body.ApplyLinearImpulse(new Vector2(80, 0));
          //        //      Body.ApplyLinearImpulse(new Vector2(0, Rand.Next(-20, 20)));
          //              break;
          //
          //
          //          case 2:
          //              Body.ApplyLinearImpulse(new Vector2(-80, 0));
          //        //     Body.ApplyLinearImpulse(new Vector2(0, 50));
          //           
          //              break;
          //
          //          case 3:
          //              Body.ApplyLinearImpulse(new Vector2(80, 0));
          //      //       Body.ApplyLinearImpulse(new Vector2(0, -50));
          //              break;
          //      }
        //      _ParasiteBodies[Rand.Next(0, segmentCount)].ApplyForce(new Vector2(0, Rand.Next(-50, 50)));

       
                TimetoMove = 0;
            }

           for (int i = 0; i < _ParasiteBodies.Count; i++)
           {
               if (_ParasiteBodies[i].Rotation < -Math.PI / 16)
                   _ParasiteBodies[i].ApplyTorque(30);
               else if (_ParasiteBodies[i].Rotation > Math.PI / 16)
                   _ParasiteBodies[i].ApplyTorque(-30);
           }


        }


        //  public void HandleMessage(CMsg<eSystemMsg> Msg)
        //  {
        //      switch (Msg.m_Msg)
        //      {
        //
        //          case eSystemMsg.ACTORMSG_LOADMAP:
        //              {
        //                  XmlNode Node = (XmlNode)Msg.m_Arg1;
        //                  LoadDataFromMap(Node);
        //
        //              }
        //              break;
        //          default:
        //              break;
        //      }
        //
        //
        //  }
        //
        //
        //  public void LoadDataFromMap(XmlNode Node)
        //  {
        //      XmlNodeList ChildNodes = Node.ChildNodes;
        //      foreach (XmlNode ChildNode in ChildNodes)
        //      {
        //          if (ChildNode.Name == "Object")
        //          {
        //              XmlNodeList ChildNodes2 = ChildNode.ChildNodes;
        //
        //              foreach (XmlNode ChildNode2 in ChildNodes2)
        //              {
        //                  switch (ChildNode2.Name)
        //                  {
        //                      case "Location":
        //                          {
        //                              string[] Word = ChildNode2.InnerText.Split(' ');
        //                              position.X = int.Parse(Word[0]);
        //                              position.Y = int.Parse(Word[1]);
        //                          }
        //                          break;
        //
        //                      case "Name":
        //                          {
        //                           //   Name = ChildNode2.InnerText;
        //
        //                          }
        //                          break;
        //
        //
        //                      default:
        //                          break;
        //                  }
        //
        //              }
        //
        //
        //          }
        //      }
        //  }




    }
}
