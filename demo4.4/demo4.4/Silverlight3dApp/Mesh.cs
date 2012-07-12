using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Graphics;
using System.Diagnostics;

namespace Silverlight3dApp
{
    public class Mesh
    {
        #region fields
        GraphicsDevice graphicsDevice;
        VertexPositionNormalTexture[] vertices;
        ushort[] indices;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        float[,] heightData;
        int mapWidth, mapHeight;
        Texture2D texture;

        Scene scene;
        SilverlightEffect mySilverlightEffect;
        SilverlightEffectParameter worldViewProjectionParameter;
        SilverlightEffectParameter worldParameter;
        SilverlightEffectParameter lightPositionParameter;

        public Microsoft.Xna.Framework.Matrix World { set { worldParameter.SetValue(value); } }
        public Microsoft.Xna.Framework.Matrix WorldViewProjection { set { worldViewProjectionParameter.SetValue(value); } }
        public Vector3 LightPosition { set { lightPositionParameter.SetValue(value); } }
        #endregion

        #region init
        public Mesh(Scene _scene)
        {
            this.scene = _scene;
            this.graphicsDevice = _scene.GraphicsDevice;
            this.mySilverlightEffect = _scene.ContentManager.Load<SilverlightEffect>("CustomEffect");

            //init map for mesh
            mapWidth = 128;
            mapHeight = 128;

            //cache effect parameters
            worldViewProjectionParameter = mySilverlightEffect.Parameters["WorldViewProjection"];
            worldParameter = mySilverlightEffect.Parameters["World"];
            lightPositionParameter = mySilverlightEffect.Parameters["LightPosition"];
            this.LightPosition = new Vector3(0, 10, 0);

            //init vertices/indices
            vertices = new VertexPositionNormalTexture[mapWidth * mapHeight];
            indices = new ushort[(mapWidth - 1) * (mapHeight - 1) * 6];
            Canvas _canvas = new Canvas();
            _canvas.Width = _canvas.Height = 128;
            _canvas.Background = new SolidColorBrush(Colors.Blue);
            WriteableBitmap _map = new WriteableBitmap(_canvas, null);
            heightData = LoadHeightDataFromMap(_map);
            texture = new Texture2D(graphicsDevice, _map.PixelWidth, _map.PixelHeight);
            texture.SetData(_map.Pixels);

            //
            SetupVertices();
            SetupIndices();
            CalculateNormals();
        }
        #endregion

        #region methods
        public void Draw()
        {
            foreach (var pass in mySilverlightEffect.CurrentTechnique.Passes)
            {
                graphicsDevice.Textures[0] = texture;
                graphicsDevice.RasterizerState = new RasterizerState { 
                    CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None, 
                    FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid //.WireFrame 
                };
                // Apply pass
                pass.Apply();

                // Set vertex buffer and index buffer
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.Indices = indexBuffer;

                // The shaders are already set so we can draw primitives
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }
        }

        public void UpdateMesh(WriteableBitmap _newDepthMap, WriteableBitmap _newTextureMap)
        {   
            //update texture map
            texture = new Texture2D(graphicsDevice, _newTextureMap.PixelWidth, _newTextureMap.PixelHeight);
            texture.SetData(_newTextureMap.Pixels);

            //check and resize depth map
            if (_newDepthMap.PixelWidth > 128 || _newDepthMap.PixelHeight > 128)
            {     
                //blur depth map
                _newDepthMap = BlurMap(_newDepthMap, 15);

                //resize depth map for mesh
                Image _img = new Image();
                _img.Width = _newDepthMap.PixelWidth;
                _img.Height = _newDepthMap.PixelHeight;
                _img.Source = _newDepthMap;                
                WriteableBitmap _resizedNewMap = new WriteableBitmap(128, 128);
                _resizedNewMap = new WriteableBitmap(_img, new ScaleTransform() { ScaleX = ((double)128 / _newDepthMap.PixelWidth), ScaleY = ((double)128 / _newDepthMap.PixelHeight) });
                        //Debug.WriteLine((((double)128 / _newMap.PixelWidth)).ToString());      

                //update height data
                heightData = LoadHeightDataFromMap(_resizedNewMap);
            }
            else
            {
                //blur depth map
                _newDepthMap = BlurMap(_newDepthMap, 4);

                //update height data
                heightData = LoadHeightDataFromMap(_newDepthMap);
            }

            //update vertices, indices and normals
            SetupVertices();
            SetupIndices();
            CalculateNormals();
        }

        void SetupVertices()
        {
            vertices = new VertexPositionNormalTexture[mapWidth * mapHeight];
            int counter = 0;

            //
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    //
                    vertices[counter] = new VertexPositionNormalTexture(
                        new Vector3(x - 64, heightData[x, y], y - 64), //x-64/y-64 is to set the object in the center of scene
                        new Vector3(0, 0, 0),
                        new Vector2((float)x / (mapWidth - 1), (float)y / (mapHeight - 1))
                        );

                    counter++;
                }
            }

            //set vertexbuffer
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        void SetupIndices()
        {
            indices = new ushort[(mapWidth - 1) * (mapHeight - 1) * 6];
            int i = 0;

            //
            for (int y = 0; y < (mapHeight - 1); y++)
            {
                for (int x = 0; x < (mapWidth - 1); x++)
                {
                    indices[i * 6] = (ushort)(x + y * mapWidth);
                    indices[i * 6 + 1] = (ushort)((x + 1) + (y + 1) * mapWidth);
                    indices[i * 6 + 2] = (ushort)(x + (y + 1) * mapWidth);
                    indices[i * 6 + 3] = (ushort)(x + y * mapWidth);
                    indices[i * 6 + 4] = (ushort)((x + 1) + y * mapWidth);
                    indices[i * 6 + 5] = (ushort)((x + 1) + (y + 1) * mapWidth);

                    i++;
                }
            }

            //set indexbuffer
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Length, BufferUsage.None);
            indexBuffer.SetData<ushort>(indices);
        }

        void CalculateNormals()
        {
            //calculate average normals
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            //normalize all normals
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Normal.Normalize();
            }
        }

        float[,] LoadHeightDataFromMap(WriteableBitmap _sourceMap)
        {
            //load height data from map
            WriteableBitmap _heightmap = _sourceMap;
            float[,] HeightmapArray = new float[_heightmap.PixelWidth, _heightmap.PixelHeight];

            for (int y = 0; y < _heightmap.PixelHeight; y++)
            {
                for (int x = 0; x < _heightmap.PixelWidth; x++)
                {
                    int index = _heightmap.PixelWidth * y + x;
                    int pixel = _heightmap.Pixels[index];
                    byte[] bytes = BitConverter.GetBytes(pixel);

                    // darker colour for lower height, lighter colour for higher height, max 10
                    HeightmapArray[x, y] = ((float)(bytes[0] + bytes[1] + bytes[2]) / 3 / 255 * 60);
                }
            }
            //return the heightmap data
            heightData = HeightmapArray;
            return HeightmapArray;
        }

        WriteableBitmap BlurMap(WriteableBitmap _inputMap, double _blurRadius)
        {
            Image _img = new Image();
            _img.Width = _inputMap.PixelWidth;
            _img.Height = _inputMap.PixelHeight;
            _img.Source = _inputMap;

            //blur effect
            BlurEffect _blurEffect = new BlurEffect() { Radius = _blurRadius};

            _img.Effect = _blurEffect;

            WriteableBitmap _outputMap = new WriteableBitmap(_img, null);
            return _outputMap;
        }

        
        #endregion

    }
}
