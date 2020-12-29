using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace PoincareDiskModelConsolApp.Helpers
{
    class OpenGLHelper
    {
        GameWindow window;

        public OpenGLHelper(GameWindow window) 
        {
            this.window = window;
            Start();
        }

        void Start() 
        {
            window.Load += Loaded;
            window.Resize += Resize;
            window.RenderFrame += RenderFrame;
            window.Run(1/60);
        }

        void RenderFrame(object o, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(1, 1);
            GL.Vertex2(49, 1);
            GL.Vertex2(25, 49);
            GL.End();
            window.SwapBuffers();
        }

        void Loaded(object o, EventArgs e) 
        {
            GL.ClearColor(0, 0, 0, 0);
        }

        void Resize(object ob, EventArgs e) 
        {
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 50, 0, 50, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}
