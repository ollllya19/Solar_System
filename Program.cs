using System;
using OpenTK;


namespace GLProject
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindow window = new GameWindow(800,800);
            GLWindow mainWindow = new GLWindow(window);
        }
       
    }
}
