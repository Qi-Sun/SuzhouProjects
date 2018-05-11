using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointToPlace
{
    public class DbscanPoint
    {
        public GeoPoint DataPoint { get; private set; }
        public bool IsVisit { get;  set; }
        public int ClusterID { get;  set; }
        public bool IsNoise { get;  set; }

        public DbscanPoint(GeoPoint p)
        {
            DataPoint = p;
            IsVisit = false;
            ClusterID = -1;
            IsNoise = false;
        }

        public double GetDistance(DbscanPoint p)
        {
            return this.DataPoint.DistanceToPoint(p.DataPoint);
        }

        public override string ToString()
        {
            return DataPoint.ToString() + " Cluster " + ClusterID + " IsNoised " + (IsNoise ? 1 : 0);
        }
    }
}
