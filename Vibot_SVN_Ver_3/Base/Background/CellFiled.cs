
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Vibot
{
    /// <summary>
    /// The starfield that renders behind the game, including a parallax effect.
    /// </summary>
    public class CellField : IDisposable
    {


        Random rand;
        /// <summary>
        /// The number of stars in the starfield.
        /// </summary>
        const int numberOfCells = 100; //개수
        /// <summary>
        /// The number of layers in the starfield.
        /// </summary>
        const int numberOfLayers = 8; //원래 8

        /// <summary>
        /// The colors for each layer of stars.
        /// </summary>
        /// 


        readonly Color[] layerColors = new Color[numberOfLayers]  //        static
            { 
              //  new Color(255, 255, 255, 255), 
              //  new Color(255, 255, 255, 216), 
              //  new Color(255, 255, 255, 192), 
             //   new Color(255, 255, 255, 160), 
                new Color(255, 255, 255, 128), 

             

                new Color(255, 255, 255, 96), 
                new Color(255, 255, 255, 64), 
                new Color(255, 255, 255, 32),

                new Color(255, 255, 255, 64), 
                new Color(255, 255, 255, 96), 
                new Color(255, 255, 255, 64), 
                new Color(255, 255, 255, 32) 
            };
        
        /// <summary>
        /// The backcellmovement factor for each layer of stars, used in the parallax effect.
        /// </summary>
        /// 

        readonly float[] backcellmovementFactors = new float[numberOfLayers] //static
            {
                0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f
            };

        /// <summary>
        /// The maximum amount of backcellmovement allowed per update.
        /// </summary>
        /// <remarks>
        /// Any per-update backcellmovement values that exceed this will trigger a 
        /// starfield reset.
        /// </remarks>
        const float maximumbackcellmovementPerUpdate = 50.0f; 

        /// <summary>
        /// The background color of the starfield.
        /// </summary>
        readonly Color backgroundColor = new Color(255, 100, 0); //   static



        /// <summary>
        /// The last position, used for the parallax effect.
        /// </summary>
        private Vector2 lastPosition;
        /// <summary>
        /// The current position, used for the parallax effect.
        /// </summary>
        private Vector2 position;
    
       private Vector2[] Cells;
       //      private Rectangle[] Cells;






        /// <summary>
        /// The graphics device used to render the starfield.
        /// </summary>
        public GraphicsDevice graphicsDevice;

        /// <summary>
        /// The content manager used to manage the m_Textures in the starfield.
        /// </summary>
        public ContentManager contentManager;

        /// <summary>
        /// The SpriteBatch used to render the starfield.
        /// </summary>
        public SpriteBatch m_spriteBatch;

        /// <summary>
        /// The m_Texture used for each star, typically one white pixel.
        /// </summary>
        public Texture2D[] CellTexture;
        public Texture2D[] CellLineTexture;



        GameTime gametime;





        /// <summary>
        /// Create a new Starfield object.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="graphicsDevice">The graphics device used to render.</param>
        /// <param name="contentManager">The content manager for this object.</param>
        public CellField(Vector2 position, GraphicsDevice graphicsDevice,
            ContentManager contentManager)
        {
            // safety-check the parameters, as they must be valid
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            if (contentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            // assign the parameters
            this.graphicsDevice = graphicsDevice;
            this.contentManager = contentManager;
   

            CellLineTexture = new Texture2D[3];
            CellTexture = new Texture2D[3];

 
           Cells = new Vector2[numberOfCells];

            Reset(position);
        }


        /// <summary>
        /// Load graphics data from the system.
        /// </summary>

        public void LoadContent()
        {
             gametime = new GameTime();
            // load the cloud m_Texture\

            for (int i = 0; i < CellTexture.Length; i++)
                CellTexture[i] = contentManager.Load<Texture2D>("Sprites\\Particles\\bubble"+i);

            rand = new Random();
            // create the SpriteBatch object
   
        }


        /// <summary>
        /// Release graphics data.
        /// </summary>
        public void UnloadContent()
        {

         
            for (int i = 0; i < CellTexture.Length; i++)
			{
                if (CellTexture != null)
                {
                        CellTexture[i].Dispose();
                        CellTexture[i] = null;
                }
			}
       

        }


        /// <summary>
        /// Reset the stars and the parallax effect.
        /// </summary>
        /// <param name="position">The new origin point for the parallax effect.</param>
        public void Reset(Vector2 position)
        {
            // recreate the stars
            int viewportWidth = graphicsDevice.Viewport.Width;
            int viewportHeight = graphicsDevice.Viewport.Height;
            for (int i = 0; i < Cells.Length; ++i)
            {
                Cells[i] = new Vector2(RandomMath.Random.Next(0, viewportWidth),
                    RandomMath.Random.Next(0, viewportHeight));
            }

            // reset the position
            this.lastPosition = this.position = position;
        }







        /// <summary>
        /// Update and draw the starfield.
        /// </summary>
        /// <remarks>
        /// This function updates and draws the starfield, 
        /// so that the per-star loop is only run once per frame.
        /// </remarks>
        /// <param name="position">The new position for the parallax effect.</param>



        public void Draw(Vector2 position, SpriteBatch m_spriteBatch)
        {
            // update the current position
            this.lastPosition = this.position;
            this.position = position;


            for (int i = 0; i < Cells.Length/3; i++)
               DrawCell(CellTexture[0], i, m_spriteBatch);
           for (int i = Cells.Length / 3; i < Cells.Length*2/3; i++)
               DrawCell(CellTexture[2], i, m_spriteBatch);
           for (int i = Cells.Length * 2 / 3; i < Cells.Length; i++)
               DrawCell(CellTexture[1], i, m_spriteBatch);

     
        }


        void DrawCell(Texture2D celltexture, int count, SpriteBatch m_spriteBatch)
        {
            Vector2 backcellmovement = -1.0f * (position - lastPosition);  //속도

            // create a rectangle representing the screen dimensions of the starfield
            Rectangle fieldRectangle = new Rectangle(0, 0,
                graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            // if we've moved too far, then reset, as the stars will be moving too fast
            if (backcellmovement.Length() > maximumbackcellmovementPerUpdate)
            {
                Reset(position);
                return;
            }
        

            int depth = count % backcellmovementFactors.Length;
            Cells[count] += backcellmovement * backcellmovementFactors[depth];

                // wrap the stars around
                if (Cells[count].X < fieldRectangle.X)
                {
                    Cells[count].X = fieldRectangle.X + fieldRectangle.Width;
                    Cells[count].Y = fieldRectangle.Y +
                        RandomMath.Random.Next(fieldRectangle.Height);
              
                }
                if (Cells[count].X > (fieldRectangle.X + fieldRectangle.Width))
                {
                    Cells[count].X = fieldRectangle.X;
                    Cells[count].Y = fieldRectangle.Y +
                        RandomMath.Random.Next(fieldRectangle.Height);

                }
                if (Cells[count].Y < fieldRectangle.Y)
                {
                    Cells[count].X = fieldRectangle.X +
                        RandomMath.Random.Next(fieldRectangle.Width);
                    Cells[count].Y = fieldRectangle.Y + fieldRectangle.Height;

                }
                if (Cells[count].Y >
                    (fieldRectangle.Y + graphicsDevice.Viewport.Height))
                {
                    Cells[count].X = fieldRectangle.X +
                        RandomMath.Random.Next(fieldRectangle.Width);
                    Cells[count].Y = fieldRectangle.Y;
        
                }


                // draw the cells
             m_spriteBatch.Draw(celltexture, new Vector2((int)Cells[count].X, (int)Cells[count].Y), null, layerColors[depth]);


        }







        /// <summary>
        /// Finalizes the Starfield object, calls Dispose(false)
        /// </summary>


        /// <summary>
        /// Disposes the Starfield object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">
        /// True if this method was called as part of the Dispose method.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    for (int i = 0; i < CellTexture.Length; i++)
                    {
                        if (CellTexture[i] != null)
                        {
                            CellTexture[i].Dispose();
                            CellTexture[i] = null;
                        }
                    }
                  
                    if (m_spriteBatch != null)
                    {
                        m_spriteBatch.Dispose();
                        m_spriteBatch = null;
                    }
                }
            }
        }



    }
}

