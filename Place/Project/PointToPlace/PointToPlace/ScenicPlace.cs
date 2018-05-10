using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointToPlace
{
    public enum PlaceType
    {
        //封闭的单核
        ClosedSingleCore = 1,
        //开放的单核
        OpenSingleCore = 2,
        //封闭的多核
        ClosedMultiCores = 3,
        //开放的多核（默认）
        OpenMultiCores = 4,
    }


    public class ScenicPlace
    {
        public PlaceType ScenicType;
        public string ScenicName;
        public List<DbscanCluster> ClusterList;
        public int PoiNum;
        public int CheckinNum;
    

    }
}
