using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea
{
    public class DBScanDataPoint
    {
        public Point_LatLng sDataPoint;
        public bool sIsVisit { get; set; }
        public int sCluster { get; set; }
        public bool sIsNoised { get; set; }

        public DBScanDataPoint(Point_LatLng p)
        {
            sDataPoint = p;
            sIsVisit = false;
            sCluster = -1;
            sIsNoised = false;
        }

        public double GetDistance(DBScanDataPoint p)
        {
            return this.sDataPoint.DistanceToPoint(p.sDataPoint);
        }

        public override string ToString()
        {
            return sDataPoint.ToString() + " Cluster " + sCluster + " IsNoised " + (sIsNoised ? 1 : 0);
        }
    }
}
