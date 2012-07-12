using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
    /// parse xml data file
    /// </summary> 
    public class XMLDataParse
    {
        #region fields
        XElement root;

        //pub data fields
        public struct School
        {
            public string schName;
            public int schPubCount;
            public List<string> Connection;
        }
        List<string> schoolNames;
        public List<string> PubSchoolNames { get { return schoolNames; } }
        List<string> pubYears;
        public List<string> PubYears { get { return pubYears; } }
        List<School> schoolPubs;
        public List<School> PubSchoolPubs { get { return schoolPubs; } }

        //rte data fields
        List<string> rteYears;
        public List<string> RTEYears { get { return rteYears; } }
        List<DataSet> allRTEData;
        public List<DataSet> RTEAllRTEData { get { return allRTEData; } }

        //oei data fields
        List<DataSet> allOEIData;
        public List<DataSet> OEIAllOEIData { get { return allOEIData; } }

        //school stuff fields
        List<DataSet> allStuffsInSchools;
        public List<DataSet> AllStuffsInSchools { get { return AllStuffsInSchools; } }
        #endregion

        #region initialise xmldataparse
        public XMLDataParse() 
            : this("Data/pubsdata.xml")
        {
        }

        public XMLDataParse(string dataFile) 
        {
            root = XElement.Load(dataFile);
        }
        #endregion

        #region parse publication data
        public void ParsePubData()
        {
            //init lists
            pubYears = new List<string>();
            schoolNames = new List<string>();
            schoolPubs = new List<School>();

            var pubs =
                from pub in root.Elements("publication")
                where (string)pub.Element("cfAcro") != null
                && (string)pub.Element("refereed") != null
                && (string)pub.Element("eprintid") != null
                && (string)pub.Element("i_type") != null
                && (string)pub.Element("p_date") != null
                
                select pub;

            //count
            foreach (var _pub in pubs)
            {
                //count pub years
                if (_pub.Element("p_date") != null)
                {
                    string _year = _pub.Element("p_date").Value;
                    _year = _year.Substring(0, 4);
                    if (!pubYears.Contains(_year)) 
                    { 
                        pubYears.Add(_year);
                    }
                }

                //count school names, schoolpubs
                //get school names
                string _schoolName = _pub.Element("cfAcro").Value;

                if (!schoolNames.Contains(_schoolName))
                {
                    schoolNames.Add(_schoolName);

                    //count schoolPubs
                    School _tempSchool = new School();
                    _tempSchool.schName = _schoolName;
                    _tempSchool.schPubCount = 1;
                    schoolPubs.Add(_tempSchool);
                }
                else
                {
                    //convert list to array
                    School[] _schoolPubs = schoolPubs.ToArray();

                    //find school name and update pub num
                    for (int i = 0; i < _schoolPubs.Length; i++)
                    {
                        if (_schoolPubs[i].schName == _schoolName)
                        {
                            _schoolPubs[i].schPubCount++;
                        }
                    }

                    //convert array back to list
                    schoolPubs = _schoolPubs.ToList();
                }
            }
        }

        public int PubCountSchoolPubInYear(string _schoolName, string _year)
        {
            //xml query
            int count = 
                (from schPub in root.Elements("publication")
                where (string)schPub.Element("cfAcro") == _schoolName
                && (string)schPub.Element("refereed") != null
                && (string)schPub.Element("eprintid") != null
                && (string)schPub.Element("i_type") != null
                && ((string)schPub.Element("p_date")).Substring(0,4) == _year

                select schPub).Count();

            //return value
            return count;
        }
        
        #endregion

        #region parse rte data 
        public void ParseRTEData()
        {
            allRTEData = new List<DataSet>();

            //
            var rtes =
                from rte in root.Elements("record")
                where (string)rte.Element("year") != null
                && (string)rte.Element("researchOutput") != null
                && (string)rte.Element("researchEnv") != null
                && (string)rte.Element("researchImpact") != null
                && (string)rte.Element("teachingCourse") != null
                && (string)rte.Element("teachingTime") != null
                && (string)rte.Element("teachingStudent") != null
                && (string)rte.Element("enterprisingOutput") != null
                && (string)rte.Element("enterprisingFund") != null
                && (string)rte.Element("enterprisingImpact") != null

                select rte;           
 
            //parse data sets
            foreach (XElement record in rtes)
            {
                DataSet _temp = new DataSet();

                //
                _temp.y = record.Element("year").Value;
                _temp.a1 = double.Parse(record.Element("researchOutput").Value);
                _temp.a2 = double.Parse(record.Element("researchEnv").Value);
                _temp.a3 = double.Parse(record.Element("researchImpact").Value);
                //
                _temp.b1 = double.Parse(record.Element("teachingCourse").Value);
                _temp.b2 = double.Parse(record.Element("teachingTime").Value);
                _temp.b3 = double.Parse(record.Element("teachingStudent").Value);
                //
                _temp.c1 = double.Parse(record.Element("enterprisingOutput").Value);
                _temp.c2 = double.Parse(record.Element("enterprisingFund").Value);
                _temp.c3 = double.Parse(record.Element("enterprisingImpact").Value);
                //
                allRTEData.Add(_temp);
            }
            allRTEData.Sort();

            //count years
            rteYears = new List<string>();
            foreach (var year in rtes)
            {
                if (!rteYears.Contains(year.Element("year").Value))
                    rteYears.Add(year.Element("year").Value);
            }
        }
        #endregion

        #region parse oei data
        public void ParseOEIData()
        {
            allOEIData = new List<DataSet>();

            var oeis =
                from oei in root.Elements("record")
                where (string) oei.Element("year") != null
                && (string)oei.Element("researchOut") != null
                && (string)oei.Element("researchEnv") != null
                && (string)oei.Element("researchImp") != null

                select oei;

            //
            foreach (var oei in oeis)
            {
                DataSet _temp = new DataSet();

                _temp.y = oei.Element("year").Value;

                _temp.a1 = double.Parse(oei.Element("researchOut").Value);
                _temp.a2 = double.Parse(oei.Element("researchEnv").Value);
                _temp.a3 = double.Parse(oei.Element("researchImp").Value);

                _temp.b1 = double.Parse(oei.Element("researchOut").Value);
                _temp.b2 = double.Parse(oei.Element("researchEnv").Value);
                _temp.b3 = double.Parse(oei.Element("researchImp").Value);

                _temp.c1 = double.Parse(oei.Element("researchOut").Value);
                _temp.c2 = double.Parse(oei.Element("researchEnv").Value);
                _temp.c3 = double.Parse(oei.Element("researchImp").Value);

                allOEIData.Add(_temp);
            }
        }
        #endregion

        #region parse school staff data
        public void ParseSchStuffData()
        {
            allStuffsInSchools = new List<DataSet>();

            var stuffs =
                from stuff in root.Elements("stuff")
                where (string)stuff.Element("name") != null
                && (string)stuff.Element("research") != null
                && (string)stuff.Element("teaching") != null
                && (string)stuff.Element("enterprising") != null
                && (string)stuff.Element("school") != null

                select stuff;

            //read data
            foreach (var _stuff in stuffs)
            {
                DataSet _temp = new DataSet();

                _temp.tag = _stuff.Element("name").Value;
                _temp.tag2 = _stuff.Element("school").Value;

                _temp.a1 = double.Parse(_stuff.Element("research").Value);
                _temp.b1 = double.Parse(_stuff.Element("teaching").Value);
                _temp.c1 = double.Parse(_stuff.Element("enterprising").Value);

                allStuffsInSchools.Add(_temp);
            }
        }

        public List<DataSet> StaffCountSchoolStuffs(string _targetSchoolName)
        {
            //check input parameter
            if (_targetSchoolName.Length > 0)
            {
                List<DataSet> _output = new List<DataSet>();

                //check state
                if (allStuffsInSchools.Count <= 0) ParseSchStuffData();

                //count stuffs in school
                foreach (DataSet _ds in allStuffsInSchools)
                {
                    if (_ds.tag2 == _targetSchoolName)
                    {
                        _output.Add(_ds);
                    }
                }

                //return value
                return _output;
            }
            else
            {
                throw new System.ArgumentException();
            }
        }
        #endregion
    }
}
