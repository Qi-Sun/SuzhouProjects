using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointToPlace
{
    public class DbscanMethod
    {
        private double Radius;
        private int MinPts;

        public DbscanMethod(double radius,int minpts)
        {
            Radius = radius;
            MinPts = minpts;
        }

        private List<DbscanPoint> getAdjacentDBScanDataPoints(DbscanPoint centerDBScanDataPoint, List<DbscanPoint> DBScanDataPoints)
        {
            List<DbscanPoint> adjacentDBScanDataPoints = new List<DbscanPoint>();
            foreach (DbscanPoint p in DBScanDataPoints)
            {
                //include centerDBScanDataPoint itself
                double distance = centerDBScanDataPoint.GetDistance(p);
                if (distance <= Radius)
                {
                    adjacentDBScanDataPoints.Add(p);
                }
            }
            return adjacentDBScanDataPoints;
        }

        public double GetPointNumber(List<DbscanPoint> points)
        {
            double sSum = 0;
            foreach (DbscanPoint point in points)
            {
                sSum += (Math.Log(point.DataPoint.Num) + 1);
            }
            return sSum;
        }

        public void Process(List<DbscanPoint> DBScanDataPoints)
        {
            int size = DBScanDataPoints.Count;
            int idx = 0;
            int cluster = 0;
            while (idx < size)
            {
                DbscanPoint p = DBScanDataPoints[idx++];
                //choose an unvisited DBScanDataPoint
                if (p.IsVisit == false)
                {
                    p.IsVisit = (true);//set visited
                    List<DbscanPoint> adjacentDBScanDataPoints = getAdjacentDBScanDataPoints(p, DBScanDataPoints);
                    //set the DBScanDataPoint which adjacent DBScanDataPoints less than minPts noised
                    if (adjacentDBScanDataPoints != null && GetPointNumber(adjacentDBScanDataPoints) < MinPts)
                    {
                        p.IsNoise = true;
                    }
                    else
                    {
                        p.ClusterID = (cluster);
                        for (int i = 0; i < adjacentDBScanDataPoints.Count; i++)
                        {
                            DbscanPoint adjacentDBScanDataPoint = adjacentDBScanDataPoints[i];
                            //only check unvisited DBScanDataPoint, cause only unvisited have the chance to add new adjacent DBScanDataPoints
                            if (adjacentDBScanDataPoint.IsVisit == false)
                            {
                                adjacentDBScanDataPoint.IsVisit = (true);
                                List<DbscanPoint> adjacentAdjacentDBScanDataPoints = getAdjacentDBScanDataPoints(adjacentDBScanDataPoint, DBScanDataPoints);
                                //add DBScanDataPoint which adjacent DBScanDataPoints not less than minPts noised
                                if (adjacentAdjacentDBScanDataPoints != null && GetPointNumber(adjacentAdjacentDBScanDataPoints) >= MinPts)
                                {
                                    adjacentDBScanDataPoints.AddRange(adjacentAdjacentDBScanDataPoints);
                                }
                            }
                            //add DBScanDataPoint which doest not belong to any cluster
                            if (adjacentDBScanDataPoint.ClusterID == -1)
                            {
                                adjacentDBScanDataPoint.ClusterID = (cluster);
                                //set DBScanDataPoint which marked noised before non-noised
                                if (adjacentDBScanDataPoint.IsNoise == true)
                                {
                                    adjacentDBScanDataPoint.IsNoise = (false);
                                }
                            }
                        }
                        cluster++;
                    }
                }
            }
        }

        public ScenicPlace ReClassPoiAreaWeibo(ScenicPlace ScenicArea)
        {
            ScenicPlace sNewPoiArea = new ScenicPlace(ScenicArea.ScenicName, ScenicArea.ScenicID, 
                ScenicArea.PoiNum, ScenicArea.CheckinNum, ScenicArea.ScenicType,null);
            List<DbscanPoint> sRawDataPoints = new List<DbscanPoint>();
            List<DbscanPoint> sNewDataPoints = new List<DbscanPoint>();
            foreach (DbscanCluster sClass in ScenicArea.ClusterList)
            {
                foreach (GeoPoint sPoint in sClass.PointList)
                {
                    sRawDataPoints.Add(new DbscanPoint(sPoint));
                }
            }
            Process(sRawDataPoints);
            sNewDataPoints.AddRange(sRawDataPoints);
            int sCount = sNewDataPoints.Count;
            int sClusterCount = -1;
            foreach (DbscanPoint point in sNewDataPoints)
            {
                sClusterCount = Math.Max(sClusterCount, point.ClusterID);
            }
            //噪音类
            DbscanCluster sNoiseClass = new DbscanCluster();
            sNoiseClass.ClusterID = -1;
            //全是噪音
            if (sClusterCount == -1)
            {
                for (int i = 0; i < sCount; i++)
                {
                    sNoiseClass.PointList.Add(sNewDataPoints[i].DataPoint);
                }
                sNewPoiArea.ClusterList.Add(sNoiseClass);
            }
            //不全是噪音
            else
            {
                DbscanCluster[] sClasses = new DbscanCluster[sClusterCount + 1];
                for (int i = 0; i < sClusterCount + 1; i++)
                {
                    sClasses[i] = new DbscanCluster();
                    sClasses[i].ClusterID = i;
                }
                sNewPoiArea.ClusterList.AddRange(sClasses);
                for (int i = 0; i < sCount; i++)
                {
                    if (sNewDataPoints[i].ClusterID == -1)
                        sNoiseClass.PointList.Add(sNewDataPoints[i].DataPoint);
                    else
                    {
                        sNewPoiArea.ClusterList[sNewDataPoints[i].ClusterID].PointList.Add(sNewDataPoints[i].DataPoint);
                    }
                }
                sNewPoiArea.ClusterList.Add(sNoiseClass);
            }
            /* //计算类的空间尺度
            foreach (ClusteringClass sClass in sNewPoiArea.ClassList)
            {
                sClass.CalculateModelParameter();
            }*/
            return sNewPoiArea;
        }
    }
}
