using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Vibot.Actors;
using Vibot.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


//using System.Drawing;

namespace Vibot
{

    public class cCamera
    {

        public enum eCameraEvent
        {
            FOCUS_OBJECT = 0, // 이동하고 그쪽으로 줌인 (줌 인/아웃  조금씩 하면서)
            WRTING_PROLOG_WORD,
            MOVE_TO_THERE, //  해당 지점으로 카메라 이동
            WRTING_GAMEMSG,

        }


  
        public static readonly float SCREEN_BOUNDARY_WIDTH = 100 * (float)Math.Sqrt(2);  // 흔들리는거 말하는듯.. 폴더에서 따온거..
        public static Vector2 CameraPosition;

        public static float SCREEN_WIDTH = 1280;
        public static float SCREEN_HEIGHT = 720;
        public static readonly Vector2 ScreenCenter = new Vector2(SCREEN_WIDTH / 2,
      SCREEN_HEIGHT / 2);

        private static float _currentZoom = 1.0f;
        private static float _minZoom = 0.88f;
        private static float _maxZoom = 1.00f;

       public static Vector2 MapSize;

    //    private Vector2 m_LimitPos;


        public cCamera(Vector2 Map_Size)
        {
            MapSize = Map_Size;  // 전체 맵 사이즈
           
         //   m_LimitPos.X = m_MapSize.X - GraphicsDeviceManager.DefaultBackBufferWidth/2;
         //   m_LimitPos.Y = m_MapSize.Y - SCREEN_HEIGHT/2;         //SCREEN_HEIGHT;
        }



        public void InitalizeCamera(Vector2 positon) // 필요없는 메소드 같은데??
        {
            CameraPosition.X = positon.X;
            CameraPosition.Y = positon.Y;

        }

        public static Vector2 Transform(Vector2 Position)
        {
            return Position - CameraPosition;

        }

        public static Rectangle Transform(Rectangle rectangle)
        {
            return new Rectangle((int)(rectangle.X - CameraPosition.X),
                (int)(rectangle.Y - CameraPosition.Y),
                rectangle.Width,
                rectangle.Height);
        }


        public static float Zoom
        {
            get { return _currentZoom; }
            set
            {  
             //   _currentZoom = value;
              _currentZoom = MathHelper.Clamp(value, _minZoom, _maxZoom);  //currentZoom, min과 max에서만 둔다 

             
            }

        }



 public void Update(Vector2? target)
 {

     if(target.HasValue)
     {
         CameraPosition = Vector2.SmoothStep(CameraPosition, target.Value, 0.1f); // 화면이 따라가는건가??

         if (CameraPosition.Y >= 0 || CameraPosition.Y <= MapSize.Y - SCREEN_HEIGHT) //위        
         {
             if (CameraPosition.Y < 0)
                 CameraPosition.Y = 0;
             if (CameraPosition.Y > MapSize.Y - SCREEN_HEIGHT)
                 CameraPosition.Y = MapSize.Y - SCREEN_HEIGHT;// SCREEN_HEIGHT / 2 - 55;
         }
         if (CameraPosition.X >= 0 || CameraPosition.X <= MapSize.X - SCREEN_WIDTH) // 왼쪽
         {
             if (CameraPosition.X < 0)  // 맵한계치 에 도달할 시, 처리
                 CameraPosition.X = 0;

             if (CameraPosition.X > MapSize.X - SCREEN_WIDTH)
                 CameraPosition.X = MapSize.X - SCREEN_WIDTH; // 화면이 움직인다
         }    


    

     }
 }
 public static void boundCamera() // 카메라 흔들기
 {

 }

  
    }
}
