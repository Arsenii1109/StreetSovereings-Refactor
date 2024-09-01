using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace StreetSovereings_.src.objects
{
    internal class AddModel
    {
        private string _modelName;
        private string _modelPath;
        private int _vao;
        private int _vbo;
        private int _ebo;
        private int _vertexCount;

        public AddModel(string modelName, string modelPath)
        {
            _modelName = modelName;
            _modelPath = modelPath;
        }

        public void LoadModel()
        {
            // Load the 3D model from the .obj file
            string[] lines = File.ReadAllLines(_modelPath);
            List<Vector3> vertices = new List<Vector3>();
            List<uint> indices = new List<uint>();

            foreach (string line in lines)
            {
                if (line.StartsWith("v "))
                {
                    string[] vertexData = line.Split(' ');
                    vertices.Add(new Vector3(float.Parse(vertexData[1]), float.Parse(vertexData[2]), float.Parse(vertexData[3])));
                }
                else if (line.StartsWith("f "))
                {
                    string[] faceData = line.Split(' ');
                    indices.Add(uint.Parse(faceData[1].Split('/')[0]));
                    indices.Add(uint.Parse(faceData[2].Split('/')[0]));
                    indices.Add(uint.Parse(faceData[3].Split('/')[0]));
                }
            }

            _vertexCount = vertices.Count;

            // Create VAO, VBO, and EBO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float) * 3, vertices.ToArray(), BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            // Vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void RenderModel()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}