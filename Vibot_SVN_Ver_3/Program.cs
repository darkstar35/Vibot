using System;


namespace Vibot
{
    static class Program
    {
#if WINDOWS
        [STAThread]
#endif
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            
            using (MainGame game = new MainGame())
            { 
			//	try
			//	{
					game.Run(); 
			//	}
			/*	catch (Exception e)
					MessageBox.Show("Error : " + e.Message);
		    */
            }
        }
    }
}

