using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLProject
{
    class GLWindow
    {
        private GameWindow mainWindow;
        private double rotationAngle;
        float[] lightPosition;
        float[] lightColor;
        private Sun[] objects;

        public GLWindow(GameWindow window)
        {
            this.mainWindow = window;
            rotationAngle = 0;
            lightPosition = new float[] { -30, 30, 30, 0 };
            lightColor = new float[] { 1, 1, 1, 0 };
            objects = new Sun[9] { new Sun(), new Mercury(), new Venus(), new Earth(), new Mars(),
                                   new Jupiter(), new Saturn(),new Uranus(),new Neptune()};

            //overloading methods of GameWindow object
            mainWindow.Load += Loaded;
            mainWindow.Resize += Resized;
            mainWindow.RenderFrame += Rendered;
            mainWindow.KeyPress += KeyManaged;
            mainWindow.Run(1 / 45.0);
        }
        private void KeyManaged(object obj, KeyPressEventArgs e)
        {
            //переделать в switch
            if (e.KeyChar == 's')
            {
                GL.Rotate(rotationAngle, new Vector3d(1, 0, 0));
                rotationAngle += 0.1;
            }
            if (e.KeyChar == 'w')
            {
                GL.Rotate(rotationAngle, new Vector3d(-1, 0, 0));
                rotationAngle += 0.1;
            }
            if (e.KeyChar == 'a')
            {
                GL.Rotate(rotationAngle, new Vector3d(0, 1, 0));
                rotationAngle += 0.1;
            }
            if (e.KeyChar == 'd')
            {
                GL.Rotate(rotationAngle, new Vector3d(0, -1, 0));
                rotationAngle += 0.1;
            }
        }
        private void Loaded(object obj, EventArgs e)
        {
            GL.ClearColor(0, 0, 0, 0);
            GL.Enable(EnableCap.DepthTest);           //overlapping distant objects
        }
        protected void Resized(object obj, EventArgs e)
        {
            GL.Viewport(0, 0, mainWindow.Width, mainWindow.Height);
            GL.MatrixMode(MatrixMode.Projection);     //matrix of projection
            GL.LoadIdentity();                        //cleaning our matrix
            GL.Ortho(-650.0, 650.0, -650.0, 650.0, -650.0, 650.0);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        protected void Rendered(object obj, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CreateLighting();
            for (int i = 0; i < objects.Length; ++i)
            {
                GL.PushMatrix();

                objects[i].DrawOrbit();
                Point tempPoint = objects[i].RotateAroundCenter();
                GL.Translate(tempPoint.X, tempPoint.Y, tempPoint.Z);
                objects[i].Material();
                objects[i].DrawSphere();
                GL.PopMatrix();
            }
            mainWindow.SwapBuffers();
        }
        //lighting
        private void CreateLighting()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightColor);
            GL.Enable(EnableCap.Light0); // Включаем в уравнение освещенности источник GL_LIGHT0
        }

    }
    class Point
    {
        private int x, y, z;
        public Point()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public int X
        {
            get => x;
            set => x = value;
        }
        public int Y
        {
            get => y;
            set => y = value;
        }
        public int Z
        {
            get => z;
            set => z = value;
        }
    }


    //              Calculations
    //-----------------------------------------
    //Radius:          Orbit (a.e)  Orbit(km)
    //-----------------------------------------
    //Sun: 696 000     |    -     |     -
    //Merk - 2 439     |   0,39   |  58,343169  (6)
    //Jupit - 69 911   |   5,20   |  777,908927
    //Saturn - 58 232  |   9,54   |  1427,163686
    //Uran - 25 362    |   19,22  |  2875,271074
    //Neptun - 24 622  |   30,06  |  4496,911992
    //Earth - 6 371    |   1,00   |  149,597870
    //Venerus - 6 051  |   0,72   |  107,710466
    //Msrs - 3 390     |   1,52   |  227,388763


    //classes of planets and sun
    class Sun
    {
        protected Point point0;
        protected double radius, orbitalRadius;
        protected double rotationAngle, rotationSpeed;
        protected float[] color;

        public Sun()
        {
            point0 = new Point();
            radius = 70;
            orbitalRadius = 0;
            rotationAngle = 0;
            rotationSpeed = 0.05;
            color = new float[] { 0.05f, 0.02f, 0.001f, 0 };
        }
        public void DrawSphere()
        {
            int nx = 24;
            int ny = 24;
            double x, y, z, sy, cy, sy1, cy1, sx, cx, piy, pix, ay, ay1, ax, tx, ty, ty1, dnx, dny, diy;
            dnx = 1.0 / (double)nx;
            dny = 1.0 / (double)ny;

            GL.Begin(PrimitiveType.QuadStrip);
            piy = Math.PI * dny;
            pix = Math.PI * dnx;
            for (int iy = 0; iy < ny; iy++)
            {
                diy = (double)iy;
                ay = diy * piy;
                sy = Math.Sin(ay);
                cy = Math.Cos(ay);
                ty = diy * dny;
                ay1 = ay + piy;
                sy1 = Math.Sin(ay1);
                cy1 = Math.Cos(ay1);
                ty1 = ty + dny;
                for (int ix = 0; ix <= nx; ix++)
                {
                    ax = 2.0 * ix * pix;
                    sx = Math.Sin(ax);
                    cx = Math.Cos(ax);
                    x = radius * sy * cx;
                    y = radius * sy * sx;
                    z = radius * cy;
                    tx = (double)ix * dnx;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tx, ty);
                    GL.Vertex3(x, y, z);
                    x = radius * sy1 * cx;
                    y = radius * sy1 * sx;
                    z = radius * cy1;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tx, ty1);
                    GL.Vertex3(x, y, z);
                }
            }
            GL.End();
        }
        public void Material()
        {
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, color);
        }
        public void DrawOrbit()
        {
            GL.Begin(BeginMode.Points);
            GL.Color3(1, 0, 0);
            GL.Vertex2(0, 0);
            for (int i = 0; i < 720; i++)
                GL.Vertex2(Math.Cos(i) * orbitalRadius, Math.Sin(i) * orbitalRadius);
            GL.End();
        }
        public virtual Point RotateAroundCenter()
        {
            point0.X = (int)(orbitalRadius * Math.Cos(rotationAngle));
            point0.Y = (int)(orbitalRadius * Math.Sin(rotationAngle));
            rotationAngle += rotationSpeed;
            return point0;
        }
    }
    class Mercury : Sun
    {
        public Mercury()
        {
            rotationSpeed = 0.015;
            radius = 9;
            orbitalRadius = 80;
            color = new float[] { 0.98f, 0.625f, 0.12f, 0 };
        }
    }
    class Venus : Sun
    {
        public Venus()
        {
            rotationSpeed = 0.008;
            radius = 12;
            orbitalRadius = 100;
            color = new float[] { 0.05f, 0.002f, 0.001f, 0 };
        }
    }
    class Earth : Sun
    {
        public Earth()
        {
            rotationSpeed = 0.005;
            radius = 14;
            orbitalRadius = 125;
            color = new float[] { 0, 0, 1, 0 };
        }
    }
    class Mars : Sun
    {
        public Mars()
        {
            rotationSpeed = 0.005;
            radius = 10;
            orbitalRadius = 165;
            color = new float[] { 0.05f, 0.002f, 0.001f, 0 };
        }
    }
    class Jupiter : Sun
    {
        public Jupiter()
        {
            rotationSpeed = 0.003;
            radius = 50;
            orbitalRadius = 250;
            color = new float[] { 0.05f, 0.02f, 0.01f, 0};
        }
    }
    class Saturn : Sun
    {
        public Saturn()
        {
            rotationSpeed = 0.001;
            radius = 45;
            orbitalRadius = 370;
            color = new float[] { 0.005f, 0.001f, 0.003f, 0 };
        }
    }
    class Uranus : Sun
    {
        public Uranus()
        {
            rotationSpeed = 0.004;
            radius = 20;
            orbitalRadius = 490;
            color = new float[] { 0, 0, 0.06f, 0 };
        }
    }
    class Neptune : Sun
    {
        public Neptune()
        {
            rotationSpeed = 0.003;
            radius = 20;
            orbitalRadius = 600;
            color = new float[] { 0.05f, 0.02f, 0.1f, 0 };
        }
    }
}
