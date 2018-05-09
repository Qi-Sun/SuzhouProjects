using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.WindowsForms;
using System.IO;
using System.Data;
using System.Collections;

namespace PointToArea
{
    public class POIArea
    {
        public string PoiName;
        public string PoiID;
        public Point_LatLng sCenterPoint;
        public double sAverageDistance;
        public double sAffectSigma;
        public List<ClusteringClass> ClassList = new List<ClusteringClass>();

        public POIArea() { }
        public POIArea(string poiname,string poiid, ClusteringClass[] list)
        {
            PoiName = poiname;
            PoiID = poiid;
            if (list != null)
            {
                ClassList.AddRange(list);
            } 
        }

        public double InterclassRelationsFunction(Point_LatLng p)
        {
            int sClassCount = ClassList.Count;
            List<double> sValues = new List<double>();
            if (sClassCount < 0)
                return 0;
            else
            {
                foreach (ClusteringClass sClass in ClassList)
                {
                    if (sClass.ClassID == -1) continue;
                    sValues.Add(sClass.InterclassRelationsFunction(p, sAffectSigma));
                }
            }
            return sValues.Max();
        }

        public double ClassesAffectFunction(Point_LatLng p)
        {
            //double value = 0;
            int Count = ClassList.Count;
            if (Count == 0)
            {
                return -1;
            }
            double[] values = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                if (ClassList[i].ClassID == -1)
                    values[i] = 0;
                else
                    values[i] = ClassList[i].ClassAffectFunction(p);
            }
            return values.Max();
        }

        public double ClassesAffectFunction_NoNum(Point_LatLng p)
        {
            //double value = 0;
            int Count = ClassList.Count;
            if (Count == 0)
            {
                return -1;
            }
            double[] values = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                if (ClassList[i].ClassID == -1)
                    values[i] = 0;
                else
                    values[i] = ClassList[i].ClassAffectFunction_NoNum(p);
            }
            return values.Max();
        }

        public double ClassesAffectFunction_NoSigma(Point_LatLng p)
        {
            //double value = 0;
            int Count = ClassList.Count;
            if (Count == 0)
            {
                return -1;
            }
            double[] values = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                if (ClassList[i].ClassID == -1)
                    values[i] = 0;
                else
                    values[i] = ClassList[i].ClassAffectFunction_NoSigma(p);
            }
            return values.Max();
        }

        public double ClassesAffectFunction_ScaleAdaptive(Point_LatLng p)
        {
            //double value = 0;
            int Count = ClassList.Count;
            if (Count == 0)
            {
                return 0;
            }
            double[] values = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                if (ClassList[i].ClassID == -1)
                    values[i] = 0;
                else
                    values[i] = ClassList[i].ClassAffectFunction_ScaleAdaptive(p);
            }
            return values.Max();
        }

        public double DiscriminantFunction_ScaleAdaptive(Point_LatLng p)
        {
            return/* ClassesAffectFunction_ScaleAdaptive(p) + */
                InterclassRelationsFunction(p);
        }

        public static void WriteToFile(POIArea p, string path)
        {
            string sFormat = "{0},{1},{2},{3}";
            string sFilename = p.PoiName + "_" + p.PoiID + ".txt";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            StreamWriter sw = new StreamWriter(path + sFilename);
            int sCount = p.ClassList.Count;
            for (int i = 0; i < sCount; i++)
            {
                int ssCount = p.ClassList[i].PointList.Count;
                for (int j = 0; j < ssCount; j++)
                {
                    Point_LatLng sP = p.ClassList[i].PointList[j];
                    if (sP.Lat == 0 || sP.Lng == 0) continue;
                    sw.WriteLine(sFormat, i, sP.Lat, sP.Lng, sP.Num);
                }
            }
            sw.Close();
            return;
        }

        public static void WriteToFile_WithSigma(POIArea p, string path)
        {
            string sFormat = "{0},{1},{2},{3},{4}";
            string sFilename = p.PoiName + "_" + p.PoiID + ".txt";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            StreamWriter sw = new StreamWriter(path + sFilename);
            int sCount = p.ClassList.Count;
            for (int i = 0; i < sCount; i++)
            {
                int ssCount = p.ClassList[i].PointList.Count;
                for (int j = 0; j < ssCount; j++)
                {
                    Point_LatLng sP = p.ClassList[i].PointList[j];
                    if (sP.Lat == 0 || sP.Lng == 0) continue;
                    sw.WriteLine(sFormat, i, sP.Lat, sP.Lng, sP.Num, sP.oSigma);
                }
            }
            sw.Close();
            return;
        }

        public static POIArea ReadFromFile(string fileName)
        {
            FileInfo sFileInfo = new FileInfo(fileName);
            string ShortName = sFileInfo.Name;
            string poiname = ShortName.Split('.')[0].Split('_')[0];
            string poiid= ShortName.Split('.')[0].Split('_')[1];
            StreamReader sr = new StreamReader(fileName);
            POIArea sPoiArea = new POIArea(poiname, poiid, null);
            while (sr.EndOfStream == false)
            {
                string[] sRecords = sr.ReadLine().Split(',');
                int classId = int.Parse(sRecords[0]);
                if (classId <= sPoiArea.ClassList.Count - 1) 
                {
                    sPoiArea.ClassList[classId].PointList.Add(
                        new Point_LatLng(double.Parse(sRecords[1]),
                        double.Parse(sRecords[2]), int.Parse(sRecords[3])));
                }
                else
                {
                    sPoiArea.ClassList.Add(new ClusteringClass());
                    sPoiArea.ClassList[classId].PointList.Add(
                        new Point_LatLng(double.Parse(sRecords[1]),
                        double.Parse(sRecords[2]), int.Parse(sRecords[3])));
                }
            }
            return sPoiArea;
        }

        public static POIArea ReadFromFile_WithSigma(string fileName)
        {
            FileInfo sFileInfo = new FileInfo(fileName);
            string ShortName = sFileInfo.Name;
            string poiname = ShortName.Split('.')[0].Split('_')[0];
            string poiid = ShortName.Split('.')[0].Split('_')[1];
            StreamReader sr = new StreamReader(fileName);
            POIArea sPoiArea = new POIArea(poiname, poiid, null);
            while (sr.EndOfStream == false)
            {
                string[] sRecords = sr.ReadLine().Split(',');
                int classId = int.Parse(sRecords[0]);
                if (classId <= sPoiArea.ClassList.Count - 1)
                {
                    sPoiArea.ClassList[classId].PointList.Add(
                        new Point_LatLng(double.Parse(sRecords[1]),
                        double.Parse(sRecords[2]), int.Parse(sRecords[3]),double.Parse(sRecords[4])));
                }
                else
                {
                    sPoiArea.ClassList.Add(new ClusteringClass());
                    sPoiArea.ClassList[classId].PointList.Add(
                        new Point_LatLng(double.Parse(sRecords[1]),
                        double.Parse(sRecords[2]), int.Parse(sRecords[3]), double.Parse(sRecords[4])));
                }
                if (double.Parse(sRecords[4]) == 0)
                    sPoiArea.ClassList[classId].ClassID = -1;
                else
                    sPoiArea.ClassList[classId].ClassID = classId;
            }
            return sPoiArea;
        }

        public static POIArea ReadFromDataTable(DataTable table,string Name)
        {
            int sRowCount = table.Rows.Count;
            POIArea sPoiArea = new POIArea(Name, "NotUnique", null);
            ClusteringClass sClass = new ClusteringClass();           
            for (int i = 0; i < sRowCount; i++)
            {
                double slat = double.Parse(table.Rows[i]["latitude"].ToString());
                double slng = double.Parse(table.Rows[i]["longitude"].ToString());
                if (slat == 0 || slng == 0 || slat == -1 || slng == -1) continue;

                sClass.PointList.Add(new Point_LatLng(slat, slng,
                    int.Parse(table.Rows[i]["num"].ToString())));
            }
            sPoiArea.ClassList.Add(sClass);
            return sPoiArea;
        }

        public void CalculateModelParameter(double Para_k)
        {
            List<double> sLatList = new List<double>();
            List<double> sLngList = new List<double>();
            List<double> sDistanceList = new List<double>();
            if (ClassList.Count == 0 )
            {
                sAverageDistance = 0; return;
            }
            foreach (ClusteringClass sClass in ClassList)
            {
                if (sClass.ClassID == -1) continue;
                foreach (Point_LatLng point in sClass.PointList)
                {
                    sLatList.Add(point.Lat);
                    sLngList.Add(point.Lng);
                }
            }
            if (sLatList.Count != 0)
            {
                sCenterPoint = new Point_LatLng(sLatList.Average(), sLngList.Average(), 1);
                foreach (ClusteringClass sClass in ClassList)
                {
                    if (sClass.ClassID == -1) continue;
                    foreach (Point_LatLng point in sClass.PointList)
                    {
                        sDistanceList.Add(sCenterPoint.DistanceToPoint(point));
                    }
                }
                sAverageDistance = sDistanceList.Average();
                if (sAverageDistance < 200) sAverageDistance = 200;
            }
            else
                sAverageDistance = 0;
            sAffectSigma = sAverageDistance * Para_k;
            //计算oSigma并赋值
            foreach (ClusteringClass sClass in ClassList)
            {
                if (sClass.ClassID == -1) continue;
                sClass.CalculateModelParameter();
                double sAveDis = sClass.sAverageDistance;
                foreach (Point_LatLng sPoint in sClass.PointList)
                {
                    sPoint.oSigma = sAveDis * Para_k;
                }
            }
        }
    }

    public enum MarkerType
    {
        none = 0,
        blue_small = 3,
        red_small = 32,
        green_small = 9,
        yellow_small = 14,
        brown_small = 6,
        purple_small = 28,
        gray_small = 7,
        orange_small = 22,
        black_small = 36,
        white_small = 37
    }

    public class PredictPlace : IComparable<PredictPlace>
    {
        public string sPlaceName;
        public double sPredictValue;
        public PredictPlace(string name,double value)
        {
            sPlaceName = name;
            sPredictValue = value;
        }

        public int CompareTo(PredictPlace other)
        {
            return -sPredictValue.CompareTo(other.sPredictValue);
        }
    }
}
