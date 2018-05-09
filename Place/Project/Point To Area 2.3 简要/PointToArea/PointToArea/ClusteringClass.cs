using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea
{
    public class ClusteringClass
    {
        public int ClassID;
        public Point_LatLng sCenterPoint;
        public double sAverageDistance;
        public List<Point_LatLng> PointList = new List<Point_LatLng>();

        public void CalculateModelParameter()
        {
            List<double> sLatList = new List<double>();
            List<double> sLngList = new List<double>();
            List<double> sDistanceList = new List<double>();
            if (PointList.Count == 0 || ClassID == -1) 
            {
                sAverageDistance = 0;return;
            }
            foreach (Point_LatLng point in PointList)
            {
                sLatList.Add(point.Lat);
                sLngList.Add(point.Lng);
            }
            sCenterPoint = new Point_LatLng(sLatList.Average(), sLngList.Average(), 1);
            foreach (Point_LatLng point in PointList)
            {
                sDistanceList.Add(sCenterPoint.DistanceToPoint(point));
            }
            sAverageDistance = sDistanceList.Average();
            if (sAverageDistance < 1E-5) sAverageDistance = 200;
        }

        public double ClassAffectFunction(Point_LatLng p)
        {
            double value = 0;
            int sCount = PointList.Count;
            if (sCount == 0)
            {
                return 0;
            }
            for (int i = 0; i < sCount; i++)
            {
                value += PointList[i].AffectFunction(p);
            }
            return value;
        }

        public double ClassAffectFunction_NoNum(Point_LatLng p)
        {
            double value = 0;
            int sCount = PointList.Count;
            if (sCount == 0)
            {
                return 0;
            }
            for (int i = 0; i < sCount; i++)
            {
                value += PointList[i].AffectFunction_NoNum(p);
            }
            return value;
        }

        public double ClassAffectFunction_NoSigma(Point_LatLng p)
        {
            double value = 0;
            int sCount = PointList.Count;
            if (sCount == 0)
            {
                return -0;
            }
            for (int i = 0; i < sCount; i++)
            {
                value += PointList[i].AffectFunction_NoSigma(p);
            }
            return value;
        }

        public double ClassAffectFunction_ScaleAdaptive(Point_LatLng p)
        {
            double value = 0;
            int sCount = PointList.Count;
            if (sCount == 0)
            {
                return 0;
            }
            for (int i = 0; i < sCount; i++)
            {
                value += PointList[i].AffectFunction_ScaleAdaptive(p);
            }
            return value;
        }

        public double InterclassRelationsFunction(Point_LatLng p, double AffectSigma)
        {
            double value = 0;
            int sCount = PointList.Count;
            if (sCount == 0)
            {
                return 0;
            }
            for (int i = 0; i < sCount; i++)
            {
                value += PointList[i].InterclassRelationsFunction(p, AffectSigma);
            }
            return value;
        }

    }
}
