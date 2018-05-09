using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea
{
    public class Point_LatLng
    {
        public double Lat;
        public double Lng;
        /// <summary>
        /// 重数
        /// </summary>
        public int Num;
        public static double Sigma;
        public double oSigma;

        public Point_LatLng() { }
        public Point_LatLng(double lat , double lng,int num)
        {
            Lat = lat;
            Lng = lng;
            Num = num;
        }
        public Point_LatLng(double lat, double lng, int num,double sigma)
        {
            Lat = lat;
            Lng = lng;
            Num = num;
            oSigma = sigma;
        }
        public override string ToString()
        {
            return "(" + Lat.ToString("0.00") + "," + Lng.ToString("0.00") + ") Num " + Num;
        }


        public static double Distance(Point_LatLng pa,Point_LatLng pb)
        {
            double C = Math.Sin(pa.Lat) * Math.Sin(pb.Lat) * Math.Cos(pa.Lng - pb.Lng) + Math.Cos(pa.Lat) * Math.Cos(pb.Lat);
            double R = 6371.004;
            double sDistance = R * Math.Acos(C) * Math.PI / 180;
            return sDistance;
        }
        public double DistanceToPoint(Point_LatLng p)
        {
            double C = Math.Sin(this.Lat / 180 * Math.PI) * Math.Sin(p.Lat / 180 * Math.PI) + Math.Cos(this.Lat / 180 * Math.PI) *
                Math.Cos(p.Lat / 180 * Math.PI) * Math.Cos(this.Lng / 180 * Math.PI - p.Lng / 180 * Math.PI);
            double R = 6371004;
            double sDistance = R * Math.Acos(C)/* * Math.PI / 180*/;
            return sDistance;
        }
        public double AffectFunction(Point_LatLng p)
        {
            double d = DistanceToPoint(p);
            return (Math.Log(Num) + 1) * Math.Exp(-d * d / (2 * Sigma * Sigma));
        }

        public double AffectFunction_NoNum(Point_LatLng p)
        {
            double d = DistanceToPoint(p);
            return (1) * Math.Exp(-d * d / (2 * Sigma * Sigma));
        }

        public double AffectFunction_NoSigma(Point_LatLng p)
        {
            double d = DistanceToPoint(p);
            double sSigma = Sigma * (Math.Log(Num) + 1);
            return (1) * Math.Exp(-d * d / (2 * sSigma * sSigma));
        }

        public double AffectFunction_ScaleAdaptive(Point_LatLng p)
        {
            double d = DistanceToPoint(p);
            return (Math.Log(Num) + 1) * Math.Exp(-d * d / (2 * oSigma * oSigma));
        }

        public double InterclassRelationsFunction(Point_LatLng p,double AffectSigma)
        {
            double d = DistanceToPoint(p);
            return (Math.Log(Num) + 1) * Math.Exp(-d * d / (2 * AffectSigma * AffectSigma));
        }

        public static implicit operator PointLatLng (Point_LatLng p)
        {
            return new PointLatLng(p.Lat, p.Lng);
        }
        public static implicit operator Point_LatLng(PointLatLng p)
        {
            return new Point_LatLng(p.Lat, p.Lng, 1);
        }
    }
}
