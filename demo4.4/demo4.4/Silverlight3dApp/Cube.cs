using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silverlight3dApp
{
    public class Cube
    {
        #region Fields

        readonly Scene scene;
        readonly GraphicsDevice graphicsDevice;
        readonly VertexBuffer vertexBuffer;
        readonly IndexBuffer indexBuffer;
        readonly SilverlightEffect mySilverlightEffect;
        readonly SilverlightEffectParameter worldViewProjectionParameter;
        readonly SilverlightEffectParameter worldParameter;
        readonly SilverlightEffectParameter lightPositionParameter;

        #endregion

        #region Properties

        public int VerticesCount { get; private set; }
        public int FaceCount { get; private set; }

        public Matrix World
        {
            set
            {
                worldParameter.SetValue(value);
            }
        }

        public Matrix WorldViewProjection
        {
            set
            {
                worldViewProjectionParameter.SetValue(value);
            }
        }

        public Vector3 LightPosition
        {
            set
            {
                lightPositionParameter.SetValue(value);
            }
        }

        #endregion

        #region Creation

        public Cube(Scene scene, float size)
        {
            this.scene = scene;
            this.graphicsDevice = scene.GraphicsDevice;
            this.mySilverlightEffect = scene.ContentManager.Load<SilverlightEffect>("CustomEffect");

            // Cache effect parameters
            worldViewProjectionParameter = mySilverlightEffect.Parameters["WorldViewProjection"];
            worldParameter = mySilverlightEffect.Parameters["World"];
            lightPositionParameter = mySilverlightEffect.Parameters["LightPosition"];

            // Init static parameters
            this.LightPosition = new Vector3(1, 1, -10);

            // Temporary lists
            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            List<ushort> indices = new List<ushort>();

            vertices.Add(new VertexPositionNormalTexture(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector2(0, 0)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(5, 5, 0), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f)));
            vertices.Add(new VertexPositionNormalTexture(new Vector3(5, 0, 0), new Vector3(1, 0, 0), new Vector2(1, 0)));

            indices.Add((ushort)0);
            indices.Add((ushort)1);
            indices.Add((ushort)2);

            // Create a vertex buffer, and copy our vertex data into it.
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormal.VertexDeclaration, vertices.Count, BufferUsage.None);

            vertexBuffer.SetData(0, vertices.ToArray(), 0, vertices.Count, VertexPositionColorNormal.Stride);

            // Create an index buffer, and copy our index data into it.
            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.None);

            indexBuffer.SetData(0, indices.ToArray(), 0, indices.Count);

            // Statistics
            VerticesCount = vertices.Count;
            FaceCount = indices.Count / 3;
        }

        #endregion

        #region Methods

        public void Draw()
        {
            foreach (var pass in mySilverlightEffect.CurrentTechnique.Passes)
            {
                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                // Apply pass
                pass.Apply();

                // Set vertex buffer and index buffer
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.Indices = indexBuffer;

                // The shaders are already set so we can draw primitives
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesCount, 0, FaceCount);
            }
        }

        #endregion
    }
}
