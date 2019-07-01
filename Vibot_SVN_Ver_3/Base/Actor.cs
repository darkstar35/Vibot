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
using FarseerPhysics.Collision.Shapes;

namespace Vibot.Base
{
    public enum eSystemMsg
    {
        SYSMSG_ITEM_PICKUP,
        SYSMSG_VIRUS_ITEMDROP,
        SYSMSG_VIRUS_DAMAGED,
        SYSMSG_BLOODBODY_DIE,
        SYSMSG_OBJECT_REMOVE
    }
    public enum SecureState
    {
        UNSECURE, SECURE, SECURING
    };


    public enum eItemPickUpMsg
    {
        GREENGauge_INITALIZE, YELLOWGauge_INITALIZE, VIOLETGauge_INITALIZE
    }
    public class CElement : IObject  // Vibot Laser과 기생충 , BackGround Layer, 오브젝트들의 기본적인 텍스쳐 소스와 위치
    {

        private Vector2 origin;
        public Rectangle m_Rect;
        public BoundingSphere m_Sphere;
        public double m_angle;
        public int m_Type = 0;           // 경환: 바꿔야 하는 코드 Temp Code  
        public float m_LifeTime = 0;     // 바꿔야 하는 코드 Temp Code 경환 
        public float DamValue;

    //    private Vector2 rectTovecposition;
    //
    //    public Vector2 RectToVecPosition
    //    {
    //        get
    //        {  // if(m_Rect != null)  //나중에 만들기 
    //            rectvecposition = new Vector2(m_Rect.X, m_Rect.Y);
    //            return rectvecposition;
    //        }
    //        set
    //        {
    //            if (rectvecposition != null)
    //                rectvecposition += value;
    //        }
    //    
    //    }
    //    public Vector2 RectVecPosition
    //    {
    //        get
    //        {             
    //            return rectvecposition;
    //        }
    //        set
    //        {
    //            if (rectvecposition != null)
    //                rectvecposition += value;
    //        }
    //
    //    }
        public Vector2 RectOrigin 
        {
            get
            {
                return origin = new Vector2(m_Rect.Width/2, m_Rect.Height/2);
            }
      

        }
        public Rectangle ViewPortRectangle
        {
            get
            {
                return cCamera.Transform(m_Rect);
            }

        }
        public CElement()
        {

        }

        public CElement(float x, float y, float radius)
        {
            m_Sphere = new BoundingSphere(new Vector3(x, y, 0), radius);
        }
        public CElement(int x, int y, int width, int height)
        {
            m_Rect = new Rectangle(x, y, width, height);

        }

    }

    public abstract class IActor : IObject
    {

        //    public virtual void OnUpdate(GameTime gameTime) { }
        //    public virtual void OnDraw(GameTime gameTime){}
        public virtual void HandleMessage(CMsg<eSystemMsg> Msg) { }
        public abstract void LoadDataFromMap(XmlNode Node);
        public abstract void Destory();

    }

}
