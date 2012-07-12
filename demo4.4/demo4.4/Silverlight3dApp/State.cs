using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Silverlight3dApp
{
    /// <summary>
    /// class for containing data for scene camera parameters
    /// </summary>
    public class State
    {
        //w v p parameters
        public Matrix World = Matrix.Identity;
        public Matrix View = Matrix.Identity;
        public Matrix Projection = Matrix.Identity;

        //camera parameters
        public Vector3 CameraPosition = Vector3.Zero;
        public Vector3 CameraTarget = Vector3.Zero;
    }


    /// <summary>
    /// class for containing data for pubs
    /// </summary>
    public class SchoolPubsInYears :IComparable<SchoolPubsInYears>
    {
        public string SchoolName = "";
        public List<string> Years = new List<string>();
        public List<int> Pubs = new List<int>();

        public int CompareTo(SchoolPubsInYears _schPubsInYears)
        {
            return this.SchoolName.CompareTo(_schPubsInYears.SchoolName);
        }
    }


    /// <summary>
    /// class for containing data for overall research/teaching/enterprising
    /// </summary>
    #region dataset
    public class DataSet : IComparable<DataSet>
    {
        /// <summary>
        /// research outpt/env/impact, teaching course/time/student/, and enterprising output/fund/impact
        /// </summary>
        public string y = "";
        public string tag = "";
        public string tag2 = "";

        public double a1 = 0;
        public double a2 = 0;
        public double a3 = 0;

        public double b1 = 0;
        public double b2 = 0;
        public double b3 = 0;

        public double c1 = 0;
        public double c2 = 0;
        public double c3 = 0;

        public double Sum()
        {
            double _output;
            _output = a1 + a2 + a3 + b1 + b2 + b3 + c1 + c2 + c3;
            return _output;
        }

        public int CompareTo(DataSet _dataSet)
        {
            return this.y.CompareTo(_dataSet.y);
        }
    }
    #endregion

}
