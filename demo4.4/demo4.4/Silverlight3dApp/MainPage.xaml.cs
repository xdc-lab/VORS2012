using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Silverlight3dApp
{
    /// <summary>
    /// main app interface
    /// </summary>
    public partial class MainPage
    {
        #region fields
        Scene scene;
        XMLDataParse pubData, rteData, oeiData, schStuffData;
        DataImage pubMap, rteMap, oeiMap, schStuffMap;
        State state;
        GraphicsDevice Device { get { return GraphicsDeviceManager.Current.GraphicsDevice; } }
        #endregion

        public MainPage()
        {
            InitializeComponent();
        }

        void InitialiseUI()
        {
            //the loading indicator
            loading.Visibility = System.Windows.Visibility.Collapsed;

            //hide sub panel
            researchPanel.Visibility = System.Windows.Visibility.Collapsed;
            teachingPanel.Visibility = System.Windows.Visibility.Collapsed;
            enterprisingPanel.Visibility = System.Windows.Visibility.Collapsed;

            //init label tags
            labelResearchOutput.Tag = "inactive";
            labelResearchEnvironment.Tag = "inactive";
            labelResearchImpact.Tag = "inactive";

            //searchBox
            searchBox.KeyDown += new KeyEventHandler(searchBox_KeyDown);

            //stackpanels
            stackPanelSchools.Visibility = stackPanelResearchEnvironment.Visibility = stackPanelResearchImpact.Visibility = System.Windows.Visibility.Collapsed;

            //school connection strength slider
            schoolConnectionSlider.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Check if GPU is on
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware) MessageBox.Show("Please activate enableGPUAcceleration=true on your Silverlight plugin page.", "Warning", MessageBoxButton.OK);

            //initialise UI
            InitialiseUI();

            //Create scene
            scene = new Scene(myDrawingSurface);

            //load rte data
            rteData = new XMLDataParse("Data/RTEdata.xml");            
            rteData.ParseRTEData(); //-> rteData.AllRTEData
            rteMap = new DataImage(512, 512);
            //rteMap.GenerateHisFaces(rteData.AllRTEData);//-> rteMap.TextureImage
            rteMap.GenerateHisCircles(rteData.RTEAllRTEData);

            //update scene mesh
            scene.Update(rteMap.DepthImage, rteMap.TextureImage, state);

            //load research oei data
            oeiData = new XMLDataParse("Data/researchOEIdata.xml");
            oeiData.ParseOEIData(); //-> oeiData.AllOEIData
            oeiMap = new DataImage(512, 512);

            //load publication data
            pubData = new XMLDataParse("Data/pubsdata.xml");
            pubData.ParsePubData();
            pubMap = new DataImage(512, 512);

            //load school stuff data
            schStuffData = new XMLDataParse("Data/schStuffData.xml");
            schStuffData.ParseSchStuffData();
            schStuffMap = new DataImage(512, 512);

            //debug info
            //foreach (var ps in pubData.PubSchoolPubs){Debug.WriteLine("/schName:{0}\n\\pubcount:{1}",ps.schName, ps.schPubCount);}

            //debug
            //DebugShow(rteMap.TextureImage);
        }

        private void myDrawingSurface_Draw(object sender, DrawEventArgs e)
        {
            //show render info
            sysInfo.Dispatcher.BeginInvoke(() => { sysInfo.Text = string.Format("FPS:{0} Duratioin:{1}", (int)(1000 / e.DeltaTime.TotalMilliseconds), e.TotalTime); });

            //Render scene
            scene.Draw();

            e.InvalidateSurface();
        }

        //debug use - show the image of map
        void DebugShow(WriteableBitmap _showMap)
        {
            Image _imageControl = new Image();
            _imageControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            _imageControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            _imageControl.Width = 512;
            _imageControl.Height = 512;
            _imageControl.Source = _showMap;
            LayoutRoot.Children.Remove(_imageControl);
            LayoutRoot.Children.Add(_imageControl);
        }

        //debug use - using data 
        void temp()
        {
            foreach (string _schName in pubData.PubSchoolNames)
            {
                Random _r = new Random();

                for (int i = 0; i < 20; i++)
                {
                    Debug.WriteLine("<stuff>\n<name>demo name</name>\n<research>{0}</research>\n<teaching>{1}</teaching>\n<enterprising>{2}</enterprising>\n<school>{3}</school>\n</stuff>",
                        _r.NextDouble(),
                        _r.NextDouble(),
                        _r.NextDouble(),
                        _schName);
                }
            }
        }

        #region event handlers

        private void overallSlider_valueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            List<DataSet> _tempList = new List<DataSet>();

            foreach (DataSet ds in rteData.RTEAllRTEData)
            {
                DataSet _temp = new DataSet();

                _temp.y = ds.y;

                _temp.a1 = ds.a1 * researchWeight.Value;
                _temp.a2 = ds.a2 * researchWeight.Value;
                _temp.a3 = ds.a3 * researchWeight.Value;

                _temp.b1 = ds.b1 * teachingWeight.Value;
                _temp.b2 = ds.b2 * teachingWeight.Value;
                _temp.b3 = ds.b3 * teachingWeight.Value;

                _temp.c1 = ds.c1 * enterprisingWeight.Value;
                _temp.c2 = ds.c2 * enterprisingWeight.Value;
                _temp.c3 = ds.c3 * enterprisingWeight.Value;

                _tempList.Add(_temp);
            }

            //update data map
            DataImage _newDatamap = new DataImage();
            _newDatamap.GenerateHisCircles(_tempList);
            //_newDatamap.GenerateHisFaces(_tempList);//-> _newDatamap.TextureImage
            scene.Update(_newDatamap.DepthImage, _newDatamap.TextureImage, state);

            //disp info
            var _object = sender as Slider;
            textInfo.Text = _object.Value.ToString();
        }

        private void mainLabel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var _label = sender as TextBlock;
            //
            switch (_label.Name)
            {
                case "labelResearch":
                    //Debug.WriteLine("label research clicked.");
                    textInfo.Text = _label.Name;
                    if (researchPanel.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        //change UI
                        researchPanel.Visibility = System.Windows.Visibility.Visible;
                        labelTeaching.Text = "> TEACHING";
                        labelEnterprising.Text = "> ENTERPRISING";
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Collapsed;
                        teachingPanel.Visibility = enterprisingPanel.Visibility = System.Windows.Visibility.Collapsed;
                        researchOutputSlider.Visibility = researchEnvironmentSlider.Visibility = researchImpactSlider.Visibility = System.Windows.Visibility.Visible;
                        schoolConnectionSlider.Visibility = System.Windows.Visibility.Collapsed;
                        stackPanelSchools.Children.Clear();

                        //update data and map
                        //oeiMap.GenerateHisFaces(oeiData.OEIAllOEIData);//-> oeiMap.TextureImage
                        oeiMap.GenerateHisCircles(oeiData.OEIAllOEIData);
                        scene.Update(oeiMap.DepthImage, oeiMap.TextureImage, state);
                    }
                    else
                    {
                        //change UI
                        researchPanel.Visibility = System.Windows.Visibility.Collapsed;
                        labelTeaching.Text = "< TEACHING";
                        labelEnterprising.Text = "< ENTERPRISING";
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Visible;
                        schoolConnectionSlider.Visibility = System.Windows.Visibility.Collapsed;
                        stackPanelSchools.Children.Clear();

                        //restore startup mesh
                        rteMap.GenerateHisCircles(rteData.RTEAllRTEData); //rteMap.TextureImage
                        scene.Update(rteMap.DepthImage, rteMap.TextureImage, state);
                    }

                    break;
                case "labelTeaching":
                    textInfo.Text = _label.Name;
                    if (teachingPanel.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        teachingPanel.Visibility = System.Windows.Visibility.Visible;
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Collapsed;
                        researchPanel.Visibility = enterprisingPanel.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        teachingPanel.Visibility = System.Windows.Visibility.Collapsed;
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Visible;

                        //restore startup mesh
                        //restore startup mesh
                        rteMap.GenerateHisCircles(rteData.RTEAllRTEData); //rteMap.TextureImage
                        scene.Update(rteMap.DepthImage, rteMap.TextureImage, state);
                    }
                    break;

                case "labelEnterprising":
                    textInfo.Text = _label.Name;
                    if (enterprisingPanel.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        enterprisingPanel.Visibility = System.Windows.Visibility.Visible;
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Collapsed;
                        researchPanel.Visibility = teachingPanel.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        enterprisingPanel.Visibility = System.Windows.Visibility.Collapsed;
                        researchWeight.Visibility = teachingWeight.Visibility = enterprisingWeight.Visibility = System.Windows.Visibility.Visible;

                        //restore stratup mesh
                        //restore startup mesh
                        rteMap.GenerateHisCircles(rteData.RTEAllRTEData); //rteMap.TextureImage
                        scene.Update(rteMap.DepthImage, rteMap.TextureImage, state);
                    }
                    break;
            }
        }

        private void subPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var _subLabel = sender as TextBlock;
            //
            switch (_subLabel.Name)
            {
                case "labelResearchOutput":
                    textInfo.Text = _subLabel.Name;

                    //update UI
                    researchOutputSlider.Visibility = researchEnvironmentSlider.Visibility = researchImpactSlider.Visibility = System.Windows.Visibility.Collapsed;
                    stackPanelSchools.Visibility = System.Windows.Visibility.Visible;
                    if (labelResearchOutput.Tag == "inactive")
                    {
                        //show slider for connection strength
                        schoolConnectionSlider.Visibility = System.Windows.Visibility.Visible;

                        //show school textblocks
                        foreach (var sch in pubData.PubSchoolPubs)
                        {
                            TextBlock _school = new TextBlock();
                            _school.Height = 25; _school.Width = 80;
                            _school.TextAlignment = TextAlignment.Center;
                            _school.Tag = "active";
                            _school.Name = sch.schName;
                            _school.Text = sch.schName;
                            _school.MouseRightButtonDown += new MouseButtonEventHandler(_school_MouseRightButtonDown);
                            _school.MouseLeftButtonUp += new MouseButtonEventHandler(_school_MouseLeftButtonDown);

                            stackPanelSchools.Children.Add(_school);
                        }

                        //mark panel state
                        labelResearchOutput.Tag = "active";

                        //update data
                        //get active school list -> pubData.PubSchoolNames
                        //get active school data
                        List<SchoolPubsInYears> _schPubsInYears = new List<SchoolPubsInYears>();
                        foreach (string sch in pubData.PubSchoolNames)
                        {
                            SchoolPubsInYears _tempSchoolPubsInYears = new SchoolPubsInYears();
                            _tempSchoolPubsInYears.SchoolName = sch;

                            //
                            List<string> _tempYears = new List<string>();
                            List<int> _tempPubs = new List<int>();
                            foreach (string _year in pubData.PubYears)
                            {
                                _tempYears.Add(_year);
                                _tempPubs.Add(pubData.PubCountSchoolPubInYear(sch, _year));
                            }
                            _tempSchoolPubsInYears.Years = _tempYears;
                            _tempSchoolPubsInYears.Pubs = _tempPubs;

                            //add to _schPubsInYears
                            _schPubsInYears.Add(_tempSchoolPubsInYears);
                        }

                        //send data to dataimage
                        pubMap.GenerateTerrains(_schPubsInYears);
                        scene.Update(pubMap.DepthImage, pubMap.TextureImage, state);
                    }
                    else
                    {
                        schoolConnectionSlider.Visibility = System.Windows.Visibility.Collapsed;
                        stackPanelSchools.Children.Clear();
                        labelResearchOutput.Tag = "inactive";
                    }

                    break;
                case "labelResearchEnvironment":
                    textInfo.Text = _subLabel.Name;

                    //update UI
                    stackPanelSchools.Visibility = stackPanelResearchImpact.Visibility = System.Windows.Visibility.Collapsed;
                    stackPanelResearchEnvironment.Visibility = System.Windows.Visibility.Visible;

                    break;
                case "labelResearchImpact":
                    textInfo.Text = _subLabel.Name;

                    //update UI
                    stackPanelSchools.Visibility = stackPanelResearchEnvironment.Visibility = System.Windows.Visibility.Collapsed;
                    stackPanelResearchImpact.Visibility = System.Windows.Visibility.Visible;

                    break;
                case "labelTeachingCourse":
                    textInfo.Text = _subLabel.Name;
                    break;
                case "labelTeachingTime":
                    textInfo.Text = _subLabel.Name;
                    break;
                case "labelTeachingStudent":
                    textInfo.Text = _subLabel.Name;
                    break;
                case "labelEnterprisingOutput":
                    textInfo.Text = _subLabel.Name;
                    break;
                case "labelEnterprisingFund":
                    textInfo.Text = _subLabel.Name;
                    break;
                case "labelEnterprisingImpact":
                    textInfo.Text = _subLabel.Name;
                    break;
            }
        }

        private void researchSubLabel_valueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //get target object
            var _object = sender as Slider;

            //detect event objects
            if (_object.Parent == researchPanel)
            {
                //->update data
                List<DataSet> _tempList = new List<DataSet>();

                foreach (DataSet ds in oeiData.OEIAllOEIData)
                {
                    DataSet _temp = new DataSet();

                    _temp.y = ds.y;

                    _temp.a1 = ds.a1 * researchOutputSlider.Value;
                    _temp.a2 = ds.a2 * researchEnvironmentSlider.Value;
                    _temp.a3 = ds.a3 * researchImpactSlider.Value;

                    _temp.b1 = ds.b1 * researchOutputSlider.Value;
                    _temp.b2 = ds.b2 * researchEnvironmentSlider.Value;
                    _temp.b3 = ds.b3 * researchImpactSlider.Value;

                    _temp.c1 = ds.c1 * researchOutputSlider.Value;
                    _temp.c2 = ds.c2 * researchEnvironmentSlider.Value;
                    _temp.c3 = ds.c3 * researchImpactSlider.Value;

                    _tempList.Add(_temp);
                }

                //update data image
                DataImage _newDataMap = new DataImage();
                _newDataMap.GenerateHisCircles(_tempList);

                //update mesh
                scene.Update(_newDataMap.DepthImage, _newDataMap.TextureImage, state);
            }
            else if (_object.Parent == teachingPanel)
            {
            }
            else if (_object.Parent == enterprisingPanel)
            {
            }
            else
            {
            }
        }

        private void schoolConnection_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var _object = sender as Slider;

            //update dataimage
            //get active school names
            List<string> _activeSchoolNames = new List<string>();
            foreach (TextBlock _sch in stackPanelSchools.Children)
            {
                if (_sch.Tag == "active") _activeSchoolNames.Add(_sch.Name);
            }

            //get data
            List<SchoolPubsInYears> _schPubsInYears = new List<SchoolPubsInYears>();
            foreach (string sch in _activeSchoolNames)
            {
                SchoolPubsInYears _tempSchoolPubsInYears = new SchoolPubsInYears();
                _tempSchoolPubsInYears.SchoolName = sch;

                //
                List<string> _tempYears = new List<string>();
                List<int> _tempPubs = new List<int>();
                foreach (string _year in pubData.PubYears)
                {
                    _tempYears.Add(_year);
                    _tempPubs.Add(pubData.PubCountSchoolPubInYear(sch, _year));
                }
                _tempSchoolPubsInYears.Years = _tempYears;
                _tempSchoolPubsInYears.Pubs = _tempPubs;

                //add to _schPubsInYears
                _schPubsInYears.Add(_tempSchoolPubsInYears);
            }

            //send data to dataimage
            pubMap.GenerateTerrains(_schPubsInYears, (int)_object.Value);
            scene.Update(pubMap.DepthImage, pubMap.TextureImage, state);
        }

        void _school_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if ctrl button down
            ModifierKeys _keys = Keyboard.Modifiers;
            bool _ctrlkey = (_keys & ModifierKeys.Control) != 0;
            if (true == _ctrlkey)
            {
                //debug info
                Debug.WriteLine("!!!control key down.");

                //->get selected school's name
                var _object = sender as TextBlock;
                string _selectedSchoolName = _object.Name;

                //update UI
                schoolConnectionSlider.Visibility = System.Windows.Visibility.Collapsed;


                //->load this school's all stuffs with r t e data
                //schStuffData.StaffCountSchoolStuffs(_selectedSchoolName) -> list<dataset>
                schStuffMap.GenerateTerrains(schStuffData.StaffCountSchoolStuffs(_selectedSchoolName), 1);  //->schStuffMap.DepthImage schStuffMap.TextureImage
                scene.Update(schStuffMap.DepthImage, schStuffMap.TextureImage, state);
            }
            else
            {
                //loading
                loading.Visibility = System.Windows.Visibility.Visible;
                schoolConnectionSlider.Visibility = System.Windows.Visibility.Visible;

                //change color
                var _object = sender as TextBlock;
                if (_object.Tag == "active")
                {
                    _object.Foreground = new SolidColorBrush(Colors.Gray);
                    _object.Tag = "inactive";
                }
                else
                {
                    _object.Foreground = new SolidColorBrush(Colors.Black);
                    _object.Tag = "active";
                }

                //update date
                // get active school list
                List<TextBlock> _activeSchools = new List<TextBlock>();
                foreach (TextBlock _sch in stackPanelSchools.Children)
                {
                    if (_sch.Tag == "active")
                    {
                        _activeSchools.Add(_sch); //Debug.WriteLine(_sch.Name);
                    }
                }

                // get active schools' data
                List<SchoolPubsInYears> _schPubsInYears = new List<SchoolPubsInYears>();
                foreach (TextBlock _sch in _activeSchools)
                {
                    SchoolPubsInYears _tempSchPubsInYears = new SchoolPubsInYears();
                    _tempSchPubsInYears.SchoolName = _sch.Name;

                    //
                    List<string> _tempYears = new List<string>();
                    List<int> _tempPubs = new List<int>();
                    foreach (string _y in pubData.PubYears)
                    {
                        _tempYears.Add(_y);
                        _tempPubs.Add(pubData.PubCountSchoolPubInYear(_sch.Name, _y));

                        //debug info
                        //Debug.WriteLine("school:{0}, year:{1}, pubs:{2}", _sch.Name, _y.ToString(), pubData.PubCountSchoolPubInYear(_sch.Name, _y));
                    }
                    _tempSchPubsInYears.Years = _tempYears;
                    _tempSchPubsInYears.Pubs = _tempPubs;

                    //add to _schPubInYears
                    _schPubsInYears.Add(_tempSchPubsInYears);
                }

                //-> send data to dataimage
                pubMap.GenerateTerrains(_schPubsInYears, (int)schoolConnectionSlider.Value);//-> pubMap.TextureImage
                scene.Update(pubMap.DepthImage, pubMap.TextureImage, state);

                //debug info
                //DebugShow(pubMap.TextureImage);

                //loading                       
                loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void _school_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // get clicked school name
            var _object = sender as TextBlock;
            string _selectedSchoolName = _object.Name;

            // query this school's pubs in years
            SchoolPubsInYears _tempSchoolPubInYears = new SchoolPubsInYears();
            _tempSchoolPubInYears.SchoolName = _selectedSchoolName;
            foreach (string _year in pubData.PubYears)
            {
                _tempSchoolPubInYears.Years.Add(_year);
                _tempSchoolPubInYears.Pubs.Add(pubData.PubCountSchoolPubInYear(_selectedSchoolName, _year));
            }

            //-> generate data image
            pubMap.GenerateTerrains(_tempSchoolPubInYears);//-> pubMap.TextureImage
            scene.Update(pubMap.DepthImage, pubMap.TextureImage, state);

            //debug info
            //DebugShow(pubMap.TextureImage);
            Debug.WriteLine("!!!right click ->" + _object.Name);
        }

        void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            var _object = sender as TextBox;

            //check key value 
            if (e.Key == Key.Enter)
            {
                //->parse searchBox texts
                string _searchBoxContent = _object.Text;
                string[] _searchBoxContentSplit = _searchBoxContent.Split(new Char[] { ',', '.', ' ' });

                //add to year list _yearListFromSearchBox
                List<string> _yearListFromSearchBox = new List<string>();
                for (int i = 0; i < _searchBoxContentSplit.Length; i++)
                {
                    if (pubData.PubYears.Contains(_searchBoxContentSplit[i].Trim()) && !_yearListFromSearchBox.Contains(_searchBoxContentSplit[i].Trim()))
                    {
                        _yearListFromSearchBox.Add(_searchBoxContentSplit[i].Trim());
                        //Debug.WriteLine(_searchBoxContentSplit[i].Trim());
                    }
                }

                //get data of the years
                List<DataSet> _tempDataSetList = new List<DataSet>();
                foreach (DataSet _ds in rteData.RTEAllRTEData)
                {
                    if (_yearListFromSearchBox.Contains(_ds.y))
                    {
                        DataSet _temp = new DataSet();
                        _temp = _ds;
                        _tempDataSetList.Add(_temp);
                    }
                }


                //generate data image
                rteMap.GenerateHisCircles(_tempDataSetList);//-> rteMap.TextureImage
                scene.Update(rteMap.DepthImage, rteMap.TextureImage, state);
            }
        }

        #endregion

    }
}
