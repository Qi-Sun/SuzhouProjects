using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea.Para
{
    public class DbscanPara
    {
        public double Radius;
        public int Minpts;
        public DbscanPara(double r,int n)
        {
            Radius = r;
            Minpts = n;
        }
    }
}
