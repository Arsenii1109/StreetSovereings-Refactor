using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StreetSovereings_.src.objects;

namespace StreetSovereings_.src
{
    internal class Renderer
    {
        private static NativeWindowSettings _settings = new NativeWindowSettings
        {
            Title = "Street Sovereigns",
            MinimumClientSize = new Vector2i(800, 600), 
            ClientSize = new Vector2i(1000, 800) 
        };

        public class Game : GameWindow
        {
            private enum GameState
            {
                Playing 
            }

            private GameState _currentState = GameState.Playing;

            private readonly CubeManager _cubeManager = new CubeManager();
            private readonly PlaneManager _planeManager = new PlaneManager();

            private readonly float[] _vertices =
            {
                // Positions         
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
            };

            private readonly uint[] _indices =
            {
                0, 1, 2, 2, 3, 0,
                4, 5, 6, 6, 7, 4,
                0, 1, 5, 5, 4, 0,
                2, 3, 7, 7, 6, 2,
                0, 3, 7, 7, 4, 0,
                1, 2, 6, 6, 5, 1,
            };

            private int _vao;
            private int _vbo;
            private int _ebo;
            private int _shaderProgram;
            private int _frameShaderProgram;

            private Sounds _sounds;

            private bool _debugShowCoordinates = true;

            public Vector3 _cameraPosition = new Vector3(1.5f, 1.5f, 3f);
            private bool _leftControlPressed = false;

            private float _rotation = 0.0f;

            float speed = 0.001f;
            float _initialSpeed;

            public Game() : base(GameWindowSettings.Default, _settings)
            {
                _sounds = new Sounds();
            }

            protected override void OnLoad()
            {
                base.OnLoad();
                GL.ClearColor(Color4.Black);

                _sounds.InitializeAudio();

                InitializeBuffers();
                InitializeShaders();

                GL.Enable(EnableCap.DepthTest);

                AddPlane(0.0f, -1.0f, 0.0f, 10.0f, 0.1f, 10.0f, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); 
            }

            private void InitializeBuffers()
            {
                _vao = GL.GenVertexArray();
                GL.BindVertexArray(_vao);

                _vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            protected override void OnResize(ResizeEventArgs e)
            {
                base.OnResize(e);

                GL.Viewport(0, 0, e.Width, e.Height);

                var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), e.Width / (float)e.Height, 0.1f, 100.0f);
                GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref projection);
            }

            private void InitializeShaders()
            {
                _shaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);
                _frameShaderProgram = CreateShaderProgram(frameVertexShaderSource, frameFragmentShaderSource);
            }


            protected override void OnUpdateFrame(FrameEventArgs args)
            {
                base.OnUpdateFrame(args);

                var input = KeyboardState;

                UpdateGame(input);
            }

            private void ShowDebugCoordinates()
            {
                if (_debugShowCoordinates)
                {
                    Console.WriteLine(_cameraPosition);
                }
            }

            private void UpdateGame(KeyboardState input)
            {
                if (_currentState == GameState.Playing)
                {
                    var movement = new Vector3(
                        input.IsKeyDown(Keys.D) ? speed : input.IsKeyDown(Keys.A) ? -speed : 0,
                        input.IsKeyDown(Keys.Space) ? speed : input.IsKeyDown(Keys.LeftShift) ? -speed : 0,
                        input.IsKeyDown(Keys.W) ? -speed : input.IsKeyDown(Keys.S) ? speed : 0
                    );
                    _cameraPosition += movement;

                    if (movement != Vector3.Zero)
                    {
                        _sounds.StartWalkingSound();
                        ShowDebugCoordinates();
                    }

                    if (input.IsKeyPressed(Keys.LeftControl) && !_leftControlPressed)
                    {
                        _initialSpeed = speed;
                        speed *= 2;
                        _leftControlPressed = true;
                    }
                    else if (input.IsKeyReleased(Keys.LeftControl) && _leftControlPressed)
                    {
                        _leftControlPressed = false;
                        speed = _initialSpeed;
                    }

                    if (input.IsKeyDown(Keys.Escape) && input.IsKeyDown(Keys.LeftAlt))
                    {
                        Close();
                    }
                }
            }

            protected override void OnRenderFrame(FrameEventArgs args)
            {
                base.OnRenderFrame(args);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                RenderGame();

                SwapBuffers();
            }

            private void RenderGame()
            {
                GL.UseProgram(_shaderProgram);
                GL.ClearColor(Color4.CornflowerBlue);

                var view = Matrix4.LookAt(_cameraPosition, Vector3.Zero, Vector3.UnitY);
                var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

                GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "view"), false, ref view);
                GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "projection"), false, ref projection);

                GL.BindVertexArray(_vao);

                foreach (var cube in _cubeManager.GetCubes())
                {
                    var model = Matrix4.CreateTranslation(cube.Position) * Matrix4.CreateRotationY(_rotation) * Matrix4.CreateRotationX(_rotation);
                    GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref model);
                    GL.Uniform4(GL.GetUniformLocation(_shaderProgram, "ourColor"), cube.Color);
                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                }

                foreach (var plane in _planeManager.GetPlanes())
                {
                    var model = Matrix4.CreateScale(plane.Size) * Matrix4.CreateTranslation(plane.Position);
                    GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "model"), false, ref model);
                    GL.Uniform4(GL.GetUniformLocation(_shaderProgram, "ourColor"), plane.Color);
                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                }

                GL.BindVertexArray(0);
            }

            private int CreateShaderProgram(string vertexShaderSource, string fragmentShaderSource)
            {
                int vertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShader, vertexShaderSource);
                GL.CompileShader(vertexShader);
                CheckShaderCompileStatus(vertexShader);

                int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShader, fragmentShaderSource);
                GL.CompileShader(fragmentShader);
                CheckShaderCompileStatus(fragmentShader);

                int shaderProgram = GL.CreateProgram();
                GL.AttachShader(shaderProgram, vertexShader);
                GL.AttachShader(shaderProgram, fragmentShader);
                GL.LinkProgram(shaderProgram);
                CheckProgramLinkStatus(shaderProgram);

                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);

                return shaderProgram;
            }

            private void CheckShaderCompileStatus(int shader)
            {
                GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
                if (status != (int)All.True)
                {
                    string infoLog = GL.GetShaderInfoLog(shader);
                    throw new Exception($"Shader compilation failed: {infoLog}");
                }
            }

            private void CheckProgramLinkStatus(int program)
            {
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
                if (status != (int)All.True)
                {
                    string infoLog = GL.GetProgramInfoLog(program);
                    throw new Exception($"Program link failed: {infoLog}");
                }
            }

            public void AddCube(float x, float y, float z, Vector4 rgba, float mass)
            {
                _cubeManager.AddCube(x, y, z, rgba, mass);
            }

            public void AddPlane(float x, float y, float z, float sizeX, float sizeY, float sizeZ, Vector4 rgba)
            {
                _planeManager.AddPlane(x, y, z, sizeX, sizeY, sizeZ, rgba);
            }

            private Vector2 ScreenToNormalizedDeviceCoordinates(Vector2 screenCoordinates, Vector2i screenSize)
            {
                return new Vector2(
                    (2.0f * screenCoordinates.X) / screenSize.X - 1.0f,
                    1.0f - (2.0f * screenCoordinates.Y) / screenSize.Y
                );
            }

            private const string vertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                }";

            private const string fragmentShaderSource = @"
                #version 330 core
                uniform vec4 ourColor;
                out vec4 color;

                void main()
                {
                    color = ourColor;
                }";

            private const string frameVertexShaderSource = @"
                #version 330 core
                layout (location = 0) in vec2 aPosition;

                void main()
                {
                    gl_Position = vec4(aPosition, 0.0, 1.0);
                }";

            private const string frameFragmentShaderSource = @"
                #version 330 core
                out vec4 color;

                void main()
                {
                    color = vec4(1.0, 1.0, 1.0, 1.0); // White color for the frame
                }";
        }
    }
}