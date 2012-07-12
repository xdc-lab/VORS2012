using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Resources;

using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Silverlight3dApp
{
    /// <summary>
    /// generate writeablebitmap for heightmap use
    /// </summary>
    public class DataImage
    {
        #region fields
        int mapWidth;
        int mapHeight;        
        Color defaultColor = Color.FromArgb(255, 37, 37, 107);
        SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        WriteableBitmap textureImage;
        WriteableBitmap depthImage;
        Canvas canvas;

        //public property
        public WriteableBitmap TextureImage { get { return textureImage; } }
        public WriteableBitmap DepthImage { get { return depthImage; } }
        #endregion

        #region initialise dataimage
        public DataImage()
            :this(512,512)
        {}

        //override initialise function
        public DataImage(int _width, int _height)
        {
            mapWidth = _width;
            mapHeight = _height;
            //
            textureImage = new WriteableBitmap(mapWidth, mapHeight);
            depthImage = new WriteableBitmap(mapWidth, mapHeight);
            canvas = new Canvas();
            canvas.Background = transparent;
            canvas.Width = mapWidth;
            canvas.Height = mapHeight;
        }
        #endregion

        #region method: generate historical circles
        public void GenerateHisCircles(List<DataSet> _dataList)
        {
            double _sweepAngleRO, _sweepAngleRE, _sweepAngleRI, _sweepAngleTC, _sweepAngleTT,
                    _sweepAngleTS, _sweepAngleEO, _sweepAngleEF, _sweepAngleEI, _dataSum;
            
            //
            canvas.Children.Clear();

            //
            int i = 0;
            foreach (DataSet _data in _dataList)
            {
                System.Windows.Shapes.Path path, path2, path3, path4, path5, path6, path7, path8, path9;
                i++;

                //get each segment angle
                _dataSum = _data.Sum();
                _sweepAngleRO = _data.a1 / _dataSum * 360;
                _sweepAngleRE = _data.a2 / _dataSum * 360;
                _sweepAngleRI = _data.a3 / _dataSum * 360;

                _sweepAngleTC = _data.b1 / _dataSum * 360;
                _sweepAngleTT = _data.b2 / _dataSum * 360;
                _sweepAngleTS = _data.b3 / _dataSum * 360;

                _sweepAngleEO = _data.c1 / _dataSum * 360;
                _sweepAngleEF = _data.c2 / _dataSum * 360;
                _sweepAngleEI = _data.c3 / _dataSum * 360;

                //draw arcs
                path = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), 
                    new SolidColorBrush(Colors.Transparent), 
                    (mapWidth / _dataList.Count) / 2.5,
                    0, 
                    _sweepAngleRO,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path2 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 200, 0, 0)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO, 
                    _sweepAngleRE,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path3 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 150, 0, 0)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE, 
                    _sweepAngleRI,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path4 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI, 
                    _sweepAngleTC,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path5 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 200, 0)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI + _sweepAngleTC, 
                    _sweepAngleTT,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path6 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 150, 0)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI + _sweepAngleTC + _sweepAngleTT, 
                    _sweepAngleTS,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path7 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI + _sweepAngleTC + _sweepAngleTT + _sweepAngleTS, 
                    _sweepAngleEO,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path8 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 0, 200)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI + _sweepAngleTC + _sweepAngleTT + _sweepAngleTS + _sweepAngleEO, 
                    _sweepAngleEF,
                    (i * (mapWidth / _dataList.Count) + 4) / 2,
                    new Point(mapWidth / 2, mapHeight / 2));
                path9 = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, 0, 0, 150)), 
                    new SolidColorBrush(Colors.Transparent),
                    (mapWidth / _dataList.Count) / 2.5,
                    _sweepAngleRO + _sweepAngleRE + _sweepAngleRI + _sweepAngleTC + _sweepAngleTT + _sweepAngleTS + _sweepAngleEO + _sweepAngleEF, 
                    _sweepAngleEI, 
                    (i * (mapWidth / _dataList.Count) + 10) / 2, 
                    new Point(mapWidth / 2, mapHeight / 2));

                //add paths to canvas
                canvas.Children.Add(path);
                canvas.Children.Add(path2);
                canvas.Children.Add(path3);
                canvas.Children.Add(path4);
                canvas.Children.Add(path5);
                canvas.Children.Add(path6);
                canvas.Children.Add(path7);
                canvas.Children.Add(path8);
                canvas.Children.Add(path9);
            }

            //return texture/depth image
            depthImage = new WriteableBitmap(canvas, null);            
            textureImage = depthImage;
        }

        private System.Windows.Shapes.Path DrawArcPaths(SolidColorBrush strok, SolidColorBrush fill, double thickness, double startAngle, double sweepAngle, double radius, Point center)
        {
            System.Windows.Shapes.Path _output = new System.Windows.Shapes.Path();

            //
            _output.Stroke = strok;
            _output.StrokeThickness = thickness;
            _output.Fill = fill;

            //
            ArcSegment _arc = new ArcSegment();
            PathGeometry pGeometry = new PathGeometry();
            PathFigureCollection pfCollection = new PathFigureCollection();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(center.X + Math.Sin(Math.PI * startAngle / 180) * radius,
                center.Y + -Math.Cos(Math.PI * startAngle / 180) * radius);
            PathSegmentCollection psCollection = new PathSegmentCollection();
            _arc.SweepDirection = SweepDirection.Clockwise;
            _arc.Size = new Size(radius, radius);
            _arc.IsLargeArc = (sweepAngle > 180);
            _arc.Point = new Point(
                center.X + Math.Sin(Math.PI * (startAngle + sweepAngle) / 180) * radius,
                center.Y + -Math.Cos(Math.PI * (startAngle + sweepAngle) / 180) * radius);
            psCollection.Add(_arc);
            pFigure.Segments = psCollection;
            pfCollection.Add(pFigure);
            pGeometry.Figures = pfCollection;
            _output.Data = pGeometry;

            //
            return _output;
        }
        #endregion

        #region method: generate historical faces
        public void GenerateHisFaces(List<DataSet> _dataSource)
        {
            //set up unit size
            int unitWidth = 128 / _dataSource.Count;
            int unitHeight = 128 / 3;

            //set up canvas
            canvas.Children.Clear();
            canvas.Background = transparent;

            //draw data map
            int i = 0;
            foreach (DataSet _inputData in _dataSource)
            {
                //first rect
                Rectangle _rect = new Rectangle();
                _rect.Width = unitWidth;
                _rect.Height = unitHeight;
                _rect.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(_inputData.a1 * 255), 0, 0)); //Debug.WriteLine("!_blue {0}", _inputData.RO * 255);
                _rect.Margin = new Thickness(i * unitWidth, 0, 0, 0);
                canvas.Children.Add(_rect);

                //second rect
                Rectangle _rect2 = new Rectangle();
                _rect2.Width = unitWidth;
                _rect2.Height = unitHeight;
                _rect2.Fill = new SolidColorBrush(Color.FromArgb(255, 0, Convert.ToByte(_inputData.b2 * 255), 0));
                _rect2.Margin = new Thickness(i * unitWidth, unitHeight, 0, 0);
                canvas.Children.Add(_rect2);

                //third rect
                Rectangle _rect3 = new Rectangle();
                _rect3.Width = unitWidth;
                _rect3.Height = unitHeight;
                _rect3.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, Convert.ToByte(_inputData.c1 * 255)));
                _rect3.Margin = new Thickness(i * unitWidth, unitHeight * 2, 0, 0);
                canvas.Children.Add(_rect3);

                //
                i++;
            }

            //generate writeableBitmap
            depthImage = new WriteableBitmap(canvas, null);
            textureImage = depthImage;
        }
        #endregion

        #region method: generate terrain
        public void GenerateTerrains(List<DataSet> _inputData, int _connectionStrength)
        {
            //setup canvas
            canvas.Children.Clear();
            canvas.Background = transparent;

            //draw data 
            int i = 0;
            int _totalBumps = _inputData.Count(); //Debug.WriteLine(Math.Ceiling( Math.Sqrt(_totalBumps)));
            int _unitNum = (int)Math.Ceiling(Math.Sqrt(_totalBumps)); //bump number in a row
            int _unitDist = (int)Math.Floor(mapWidth / _unitNum);
            foreach (DataSet _ds in _inputData)
            {
                int _pX = _unitDist / 2 + _unitDist * (i % _unitNum);
                int _pY = _unitDist / 2 + _unitDist * (i / _unitNum);
                System.Windows.Shapes.Path _circle = DrawArcPaths(
                    new SolidColorBrush(Colors.Orange),
                    transparent,
                    (double)(_unitDist - 10) / 4,
                    0,
                    360 * (_ds.a1 / _ds.Sum()),
                    (double)(_unitDist - 30) / 2,
                    new Point(_pX, _pY)
                    );
                canvas.Children.Add(_circle);
                System.Windows.Shapes.Path _circle2 = DrawArcPaths(
                    new SolidColorBrush(Colors.Gray),
                    transparent,
                    (double)(_unitDist - 10) / 4,
                    360 * (_ds.a1 / _ds.Sum()),
                    360 * (_ds.b1 / _ds.Sum()),
                    (double)(_unitDist - 30) / 2,
                    new Point(_pX, _pY)
                    );
                canvas.Children.Add(_circle2);
                System.Windows.Shapes.Path _circle3 = DrawArcPaths(
                    new SolidColorBrush(Colors.Blue),
                    transparent,
                    (double)(_unitDist - 10) / 4,
                    360 * ((_ds.a1 + _ds.b1) / _ds.Sum()),
                    360 * (_ds.c1 / _ds.Sum()),
                    (double)(_unitDist - 30) / 2,
                    new Point(_pX, _pY)
                    );
                canvas.Children.Add(_circle3);

                i++;
            }

            //generate depth map
            depthImage = new WriteableBitmap(canvas, null);
            textureImage = depthImage;
        }

        public void GenerateTerrains(List<SchoolPubsInYears> _inputData, int _schoolConnectionStrength)
        {
            //setup _canvas
            canvas.Children.Clear();
            canvas.Background = transparent;

            //draw data shapes - terrain
            int i = 0;
            int _totalBumps = _inputData.Count;
            int _unitNum = (int)Math.Ceiling(Math.Sqrt(_totalBumps)); //bump number in a row
            int _unitDist = (int)Math.Floor(mapWidth / _unitNum);
            foreach (SchoolPubsInYears _schPubsInYears in _inputData)
            {
                //setup center point
                int _pX = _unitDist / 2 + _unitDist * (i % _unitNum);
                int _pY = _unitDist / 2 + _unitDist * (i / _unitNum);

                //draw circles
                int _max = _schPubsInYears.Pubs.Max();
                for (int n = 0; n < _schPubsInYears.Years.Count; n++)
                {
                    byte _r = (byte)(((float)_schPubsInYears.Pubs[n] / _max) * 255); //Debug.WriteLine("_r: "+_r.ToString());                    
                    byte _b = (byte)(255 - _r);
                    System.Windows.Shapes.Path _circle = DrawArcPaths(
                        new SolidColorBrush(Color.FromArgb(255, _r, _r, _b)),
                        transparent,
                        (_unitDist / _schPubsInYears.Years.Count) / 2,
                        0,
                        359.999d,
                        n * (_unitDist / _schPubsInYears.Years.Count) / 2 + 5,
                        new Point(_pX, _pY)
                        );
                    
                    canvas.Children.Add(_circle);
                }

                i++;
            }

            //generate depth map
            depthImage = new WriteableBitmap(canvas, null);

            //draw connections   
            //get all bumps' center points
            List<Point> _centerPoints = new List<Point>();
            for (int c = 0; c < _inputData.Count; c++)
            {
                _centerPoints.Add(new Point(_unitDist / 2 + _unitDist * (c % _unitNum), _unitDist / 2 + _unitDist * (c / _unitNum)));
            }

            //draw connections between points
            for (int n = 0; n < _centerPoints.Count; n++)
            {
                for(int m = n; m< _centerPoints.Count; m++)
                {
                    Line _connectionLine = new Line()
                    {
                        Stroke = new SolidColorBrush(Colors.Orange),
                        StrokeThickness = _schoolConnectionStrength,
                        X1 = _centerPoints[n].X,
                        Y1 = _centerPoints[n].Y,
                        X2 = _centerPoints[m].X,
                        Y2 = _centerPoints[m].Y };

                    canvas.Children.Add(_connectionLine);
                }
            }

            //generate texture map
            textureImage = new WriteableBitmap(canvas, null);            

        }

        public void GenerateTerrains(List<SchoolPubsInYears> _inputData)
        {
            //set up _canvas
            canvas.Children.Clear();
            canvas.Background = transparent;

            //draw data shapes - terrain
            int i = 0;
            foreach (SchoolPubsInYears _schPubsInYears in _inputData)
            {
                int _max = _schPubsInYears.Pubs.Max();
                for (int n = 0; n < _schPubsInYears.Years.Count; n++)
                {
                    byte _r = (byte)(((float)_schPubsInYears.Pubs[n] / _max) * 255); //Debug.WriteLine("_r: "+_r.ToString());                    
                    byte _b = (byte)(255 - _r);
                    System.Windows.Shapes.Path _circle = DrawArcPaths(
                        new SolidColorBrush(Color.FromArgb(255, _r, _r, _b)),
                        transparent,
                        4,
                        0,
                        359.999d,
                        n * 9 / 2,
                        new Point(64 + 128 * (i % 4), 64 + 128 * (i / 4))
                        );
                    
                    canvas.Children.Add(_circle);
                }

                //
                i++;
            }

            //generate depth map
            depthImage = new WriteableBitmap(canvas, null);

            //draw connections   
            //get all bumps' center points
            List<Point> _centerPoints = new List<Point>();
            for (int c = 0; c < _inputData.Count; c++)
            {
                _centerPoints.Add(new Point(64 + 128 * (c % 4), 64 + 128 * (c / 4)));
            }

            //draw connections between points
            for (int n = 0; n < _centerPoints.Count; n++)
            {
                for(int m = n; m< _centerPoints.Count; m++)
                {
                    Line _connectionLine = new Line()
                    {
                        Stroke = new SolidColorBrush(Colors.Orange),
                        StrokeThickness = 0,
                        X1 = _centerPoints[n].X,
                        Y1 = _centerPoints[n].Y,
                        X2 = _centerPoints[m].X,
                        Y2 = _centerPoints[m].Y };

                    canvas.Children.Add(_connectionLine);
                }
            }

            //generate texture map
            textureImage = new WriteableBitmap(canvas, null);            
        }

        public void GenerateTerrains(SchoolPubsInYears _inputData)
        {
            //set up canvas
            canvas.Children.Clear();
            canvas.Background = transparent;

            //draw data shapes - terrain
            int _max = _inputData.Pubs.Max();
            for (int i = 0; i < _inputData.Years.Count; i++)
            {
                byte _r = (byte)(((float)_inputData.Pubs[i] / _max) * 255);
                byte _g = _r;
                byte _b = (byte)(255 - _r);
                System.Windows.Shapes.Path _circle = DrawArcPaths(
                    new SolidColorBrush(Color.FromArgb(255, _r, _g, _b)),
                    transparent,
                    15,
                    0,
                    359.999d,
                    (i * 30 + 10) / 2,
                    new Point(256, 256)
                    );

                canvas.Children.Add(_circle);
            }

            //generate writeablebitmap
            textureImage = new WriteableBitmap(canvas, null);
            depthImage = textureImage;
        }

        #endregion
    }
}
