using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace HelloSilk
{
    class Program
    {
        static IWindow window;
        static GL gl;

        static uint vbo;
        static uint ebo;
        static uint vao;
        static uint shader;

        static readonly string vss = @"
            #version 330 core
            layout (location = 0) in vec4 vPos;

            void main()
            {
                gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
            }
        ";

        static readonly string fss = @"
            #version 330 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(0.0f, 1.0f, 0.0f, 1.0f);
            }
        ";

        static readonly float[] vertices =
        {
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
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

            // Create vertex array
            vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            // Initialize the vertex buffer containing the vertex data
            vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* v = &vertices[0])
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
            }

            // Initialize the element buffer containing the vertex data
            ebo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
            fixed (void* i = &indices[0])
            {
                gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            }

            // Initialize the vertex shader
            uint vs = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vs, vss);
            gl.CompileShader(vs);

            // Initialize the fragment shader
            uint fs = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fs, fss);
            gl.CompileShader(fs);

            // Linking the shader
            shader = gl.CreateProgram();
            gl.AttachShader(shader, vs);
            gl.AttachShader(shader, fs);
            gl.LinkProgram(shader);

            // Discard the no longer useful individual shaders
            gl.DetachShader(shader, vs);
            gl.DetachShader(shader, fs);
            gl.DeleteShader(vs);
            gl.DeleteShader(fs);

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            gl.EnableVertexAttribArray(0);
        }

        static void OnUpdate(double obj)
        {
            // TODO: Update game
        }

        static unsafe void OnRender(double obj)
        {
            gl.Clear((uint)ClearBufferMask.ColorBufferBit);
            gl.BindVertexArray(vao);
            gl.UseProgram(shader);
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
            gl.DeleteBuffer(vbo);
            gl.DeleteBuffer(ebo);
            gl.DeleteVertexArray(vao);
            gl.DeleteProgram(shader);
        }
    }
}
