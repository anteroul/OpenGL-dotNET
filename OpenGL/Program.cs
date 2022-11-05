using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace OpenGL
{
    class Program
    {
        public const string rootDir = "../../../";

        static IWindow window;
        
        static GL gl;
        static BufferObject<float> vbo;
        static BufferObject<uint> ebo;
        static VertexArrayObject<float, uint> vao;
        static Shader shader;

        static readonly float[] vertices =
        {
             0.5f,  0.5f, 0.0f, 1, 0, 0, 1,
             0.5f, -0.5f, 0.0f, 0, 0, 0, 1,
            -0.5f, -0.5f, 0.0f, 0, 1, 0, 1,
            -0.5f,  0.5f, 0.5f, 0, 0, 0, 1
        };

        static readonly uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1280, 720);
            options.Title = "Hello Silk.NET";

            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;

            window.Run();
        }

        static unsafe void OnLoad()
        {
            // Initialize input
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }

            // Initialize OpenGL API
            gl = GL.GetApi(window);

            ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
            vbo = new BufferObject<float>(gl, vertices, BufferTargetARB.ArrayBuffer);
            vao = new VertexArrayObject<float, uint>(gl, vbo, ebo);

            vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 7, 0);
            vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 7, 3);

            shader = new Shader(gl, "shaders/shader.vs", "shaders/shader.fs");
        }

        static void OnUpdate(double obj)
        {
            // TODO: Update game
        }

        static unsafe void OnRender(double obj)
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit);
            vao.Bind();
            shader.Use();
            shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
            gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
        }

        static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }

        static void OnClose()
        {
            vbo.Dispose();
            ebo.Dispose();
            vao.Dispose();
            shader.Dispose();
        }
    }
}
