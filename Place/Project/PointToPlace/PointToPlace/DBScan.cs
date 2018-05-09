using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToPoint
{
    public class DBScan
    {
        private double sRadius;
        private int sMinPts;

        public DBScan(double radius, int minPts)
        {
            this.sRadius = radius;
            this.sMinPts = minPts;
        }

        public double GetPointNumber(List<DBScanDataPoint> points)
        {
            double sSum = 0;
            foreach (DBScanDataPoint point in points)
            {
                sSum += (Math.Log(point.sDataPoint.Num) + 1);
            }
            return sSum;
        }

        public void Process(List<DBScanDataPoint> DBScanDataPoints)
        {
            int size = DBScanDataPoints.Count;
            int idx = 0;
            int cluster = 0;
            while (idx < size)
            {
                DBScanDataPoint p = DBScanDataPoints[idx++];
                //choose an unvisited DBScanDataPoint
                if (p.sIsVisit == false)
                {
                    p.sIsVisit = (true);//set visited
                    List<DBScanDataPoint> adjacentDBScanDataPoints = getAdjacentDBScanDataPoints(p, DBScanDataPoints);
                    //set the DBScanDataPoint which adjacent DBScanDataPoints less than minPts noised
                    if (adjacentDBScanDataPoints != null && GetPointNumber(adjacentDBScanDataPoints) < sMinPts)
                    {
                        p.sIsNoised = true;
                    }
                    else
                    {
                        p.sCluster = (cluster);
                        for (int i = 0; i < adjacentDBScanDataPoints.Count; i++)
                        {
                            DBScanDataPoint adjacentDBScanDataPoint = adjacentDBScanDataPoints[i];
                            //only check unvisited DBScanDataPoint, cause only unvisited have the chance to add new adjacent DBScanDataPoints
                            if (adjacentDBScanDataPoint.sIsVisit == false)
                            {
                                adjacentDBScanDataPoint.sIsVisit = (true);
                                List<DBScanDataPoint> adjacentAdjacentDBScanDataPoints = getAdjacentDBScanDataPoints(adjacentDBScanDataPoint, DBScanDataPoints);
                                //add DBScanDataPoint which adjacent DBScanDataPoints not less than minPts noised
                                if (adjacentAdjacentDBScanDataPoints != null && GetPointNumber(adjacentAdjacentDBScanDataPoints) >= sMinPts)
                                {
                                    adjacentDBScanDataPoints.AddRange(adjacentAdjacentDBScanDataPoints);
                                }
                            }
                            //add DBScanDataPoint which doest not belong to any cluster
                            if (adjacentDBScanDataPoint.sCluster == -1)
                            {
                                adjacentDBScanDataPoint.sCluster = (cluster);
                                //set DBScanDataPoint which marked noised before non-noised
                                if (adjacentDBScanDataPoint.sIsNoised == true)
                                {
                                    adjacentDBScanDataPoint.sIsNoised = (false);
                                }
                            }
                        }
                        cluster++;
                    }
                }
            }
        }

        private List<DBScanDataPoint> getAdjacentDBScanDataPoints(DBScanDataPoint centerDBScanDataPoint, List<DBScanDataPoint> DBScanDataPoints)
        {
            List<DBScanDataPoint> adjacentDBScanDataPoints = new List<DBScanDataPoint>();
            foreach (DBScanDataPoint p in DBScanDataPoints)
            {
                //include centerDBScanDataPoint itself
                double distance = centerDBScanDataPoint.GetDistance(p);
                if (distance <= sRadius)
                {
                    adjacentDBScanDataPoints.Add(p);
                }
            }
            return adjacentDBScanDataPoints;
        }

        public POIArea ReClassPoiAreaWeibo(POIArea sOldPoiArea)
        {
            POIArea sNewPoiArea = new POIArea(sOldPoiArea.PoiName, sOldPoiArea.PoiID, null);
            List<DBScanDataPoint> sRawDataPoint = new List<DBScanDataPoint>();
            List<DBScanDataPoint> sNewDataPoint = new List<DBScanDataPoint>();
            foreach (ClusteringClass sClass in sOldPoiArea.ClassList)
            {
                foreach (Point_LatLng sPoint in sClass.PointList)
                {
                    sRawDataPoint.Add(new DBScanDataPoint(sPoint));
                }
            }
            Process(sRawDataPoint);
            sNewDataPoint.AddRange(sRawDataPoint);
            int sCount = sNewDataPoint.Count;
            int sClusterCount = -1;
            foreach (DBScanDataPoint point in sNewDataPoint)
            {
                sClusterCount = Math.Max(sClusterCount, point.sCluster);
            }
            //噪音类
            ClusteringClass sNoiseClass = new ClusteringClass();
            sNoiseClass.ClassID = -1;
            //全是噪音
            if (sClusterCount == -1)
            {
                for (int i = 0; i < sCount; i++)
                {
                    sNoiseClass.PointList.Add(sNewDataPoint[i].sDataPoint);
                }
                sNewPoiArea.ClassList.Add(sNoiseClass);
            }
            //不全是噪音
            else
            {
                ClusteringClass[] sClasses = new ClusteringClass[sClusterCount + 1];
                for (int i = 0; i < sClusterCount + 1; i++)
                {
                    sClasses[i] = new ClusteringClass();
                    sClasses[i].ClassID = i;
                }
                sNewPoiArea.ClassList.AddRange(sClasses);
                for (int i = 0; i < sCount; i++)
                {
                    if (sNewDataPoint[i].sCluster == -1)
                        sNoiseClass.PointList.Add(sNewDataPoint[i].sDataPoint);
                    else
                    {
                        sNewPoiArea.ClassList[sNewDataPoint[i].sCluster].PointList.Add(sNewDataPoint[i].sDataPoint);
                    }
                }
                sNewPoiArea.ClassList.Add(sNoiseClass);
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
