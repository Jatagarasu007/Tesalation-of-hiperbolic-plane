using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Diagnostics;

namespace PoincareDiskModelConsolApp.Helpers
{
    public class OpenTKHelper: GameWindow
    {
        GameWindow window;
        Shader shader;
        int p;
        int q;
        int iteration;

        // For sending vertex data into
        int VBO1;
        int EBO1;
        int VAO1;

        int VBO2;
        int VAO2;

        List<PolygonSide> tesalationPolygonPoints = new List<PolygonSide>();

        MathHelper mathHelper = new MathHelper(); 

        /*
        float[] triangle1 = {
                        -0.5f, -0.5f, 0.0f, //Bottom-left vertex 0
                        1.0f, 0.0f, 0.0f, 0.0f, //color
                        0.5f, -0.5f, 0.0f, //Bottom-right vertex 1
                        1.0f, 0.0f, 0.0f, 0.0f, //color
                        -0.5f,  0.5f, 0.0f, //Top-left vertex 2
                        1.0f, 0.5f, 0.2f, 1.0f, //color
                        0.5f,  0.5f, 0.0f, //Top-right vertex 3
                        1.0f, 0.5f, 0.2f, 1.0f //color
                            };
                            };
        int [] indices = 
                        {
                         0, 1, 2,
                         1, 2, 3
                        };

            */

        float[] poencareDisk = { };


        public OpenTKHelper(int width, int height, string title, int p, int q, int iteration) : base(width, height, GraphicsMode.Default, title) 
        {
            this.p = p;
            this.q = q;
            this.iteration = iteration;
        }

        protected override void OnLoad(EventArgs e)
        {

            GL.ClearColor(0f, 0f, 0f, 0f);
            List<float> red = new List<float>{ 1.0f, 0.0f, 0.0f, 0.0f };
            List<float> orange = new List<float> { 1.0f, 0.5f, 0.2f, 1.0f };

            poencareDisk = mathHelper.GenerateCircle(new Point(0, 0), 1, red).ToArray();
            
            mathHelper.tGetInitialRegularPolygon(ref tesalationPolygonPoints, p , q, iteration);

            List<float> tesalationOpenGL = mathHelper.tTransformTesalationSetOpenGL(tesalationPolygonPoints, orange);

            VBO1 = GL.GenBuffer();
            EBO1 = GL.GenBuffer();

            // GL.GenVertexArrays gde možeš da incijalizuješ više VAO u isto vreme
            VAO1 = GL.GenVertexArray();

            GL.BindVertexArray(VAO1);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO1);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO1);
            GL.BufferData(BufferTarget.ArrayBuffer, poencareDisk.Length * sizeof(float), poencareDisk, BufferUsageHint.StaticDraw);
           

            // Absolute or relative path
            shader = new Shader("..\\..\\Shaders\\shader.vert", "..\\..\\Shaders\\shader.frag");

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float) + 16, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 3 * sizeof(float) + 16, 12);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);


            
            VBO2 = GL.GenBuffer();
            // GL.GenVertexArrays gde možeš da incijalizuješ više VAO u isto vreme
            VAO2 = GL.GenVertexArray();

            GL.BindVertexArray(VAO2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO2);
            GL.BufferData(BufferTarget.ArrayBuffer, tesalationOpenGL.ToArray().Length * sizeof(float), tesalationOpenGL.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float) + 16, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 3 * sizeof(float) + 16, 12);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here.
            shader.Use();

            GL.BindVertexArray(VAO1);
            GL.DrawArrays(PrimitiveType.Points, 0, poencareDisk.Length/ 3 * sizeof(float));

            GL.BindVertexArray(VAO2);
            GL.DrawArrays(PrimitiveType.Points, 0, 10010100);

            /*
            GL.BindVertexArray(VAO1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            */

            /*
            GL.BindVertexArray(VAO1);
            GL.DrawElements(BeginMode.Polygon, 3 , DrawElementsType.UnsignedInt, 0);
            */

            /*
            GL.BindVertexArray(VAO2);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            */

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VBO1);
            GL.DeleteBuffer(VBO2);
            shader.Dispose();
            base.OnUnload(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }


    }
}
