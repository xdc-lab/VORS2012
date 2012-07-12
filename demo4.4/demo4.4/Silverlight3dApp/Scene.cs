using System.Windows.Controls;
using System;
using System.Windows.Graphics;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Input;
using System.Diagnostics;

namespace Silverlight3dApp
{
    public class Scene : IDisposable
    {
        #region Fields
        readonly DrawingSurface drawingSurface;
        readonly ContentManager contentManager;
        readonly Mesh mesh;
        float aspectRatio;
        float rotationAngleY, rotationAngleX;
        double offsetX, offsetY;
        bool isMouseDown;
        Vector3 cameraPosition, cameraTarget;

        public State State;
        public float RotationAngle { set { aspectRatio = value; } }
        public ContentManager ContentManager { get { return contentManager; } }
        public GraphicsDevice GraphicsDevice { get { return GraphicsDeviceManager.Current.GraphicsDevice; } }
        #endregion

        #region init
        public Scene(DrawingSurface _drawingSurface)
        {
            // regist events
            drawingSurface = _drawingSurface;
            drawingSurface.SizeChanged += _drawingSurface_SizeChanged;
            drawingSurface.MouseWheel+=new System.Windows.Input.MouseWheelEventHandler(drawingSurface_MouseWheel);
            drawingSurface.MouseLeftButtonDown +=new MouseButtonEventHandler(drawingSurface_MouseLeftButtonDown);
            drawingSurface.MouseLeftButtonUp+=new MouseButtonEventHandler(drawingSurface_MouseLeftButtonUp);
            drawingSurface.MouseMove+=new MouseEventHandler(drawingSurface_MouseMove);
            drawingSurface.MouseLeave+=new MouseEventHandler(drawingSurface_MouseLeave);

            // init contentmanager
            contentManager = new ContentManager(null) { RootDirectory = "Content" };

            //set up camera
            cameraPosition = new Vector3(0, 120, 120);
            cameraTarget = Vector3.Zero;

            // Init variables
            mesh = new Mesh(this);
        }
        #endregion

        #region draw
        public void Draw()
        {
            // Clear drawingsurface
            GraphicsDeviceManager.Current.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);

            // Compute matrices
            Matrix world = Matrix.CreateRotationX(rotationAngleX) * Matrix.CreateRotationY(rotationAngleY);
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitY);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(0.85f, aspectRatio, 0.01f, 1000.0f);

            //draw mesh
            mesh.World = world;
            mesh.WorldViewProjection = world * view * projection;
            mesh.Draw();

            // Animate rotation
            //rotationAngleY += 0.00f;
        }

        public void Update(WriteableBitmap _newDepthMap, WriteableBitmap _newTextureMap, State _state)
        {
            mesh.UpdateMesh(_newDepthMap, _newTextureMap);
        }
        #endregion

        #region event handlers
        void _drawingSurface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            aspectRatio = (float)(drawingSurface.ActualWidth / drawingSurface.ActualHeight); //Debug.WriteLine("!!!size change detected.");
        }

        void drawingSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double _delta = e.Delta;
            if (_delta > 0)
            {
                //wheel rolling forward
                if(cameraPosition.Z > 0) cameraPosition += new Vector3(-5, -5, -5);
            }
            else
            {
                //wheel rolling backward
                if(cameraPosition.Z < 256) cameraPosition += new Vector3(5, 5, 5);
            }
        }

        void drawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //update mouse state
            isMouseDown = true;

            //get position
            double xPosition = e.GetPosition(null).X; 
            double yPosition = e.GetPosition(null).Y; //Debug.WriteLine("pointer pos X<{0}> Y<{1}>", xPosition, yPosition);
            offsetX = xPosition;
            offsetY = yPosition;
        }

        void drawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        void drawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            //mouse move
            if (isMouseDown == true)
            {
                //get current position
                double xPosition = e.GetPosition(null).X; 
                double yPosition = e.GetPosition(null).Y;

                //rotation
                rotationAngleX += (float)((yPosition - offsetY) * 0.002);
                rotationAngleY += (float)((xPosition - offsetX) * 0.002);

                //update offset position
                offsetX = xPosition;
                offsetY = yPosition;
            }
        }

        void drawingSurface_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public void Dispose()
        {
            drawingSurface.SizeChanged -= _drawingSurface_SizeChanged;
        }
        #endregion

    }
}
