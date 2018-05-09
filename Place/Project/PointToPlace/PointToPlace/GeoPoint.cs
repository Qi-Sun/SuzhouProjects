using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace PointToPlace
{
    public class GeoPoint
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; private set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; private set; }
        /// <summary>
        /// 重数
        /// </summary>
        public int Num { get; private set; }
        /// <summary>
        /// 全局Sigma系数
        /// </summary>
        public static double GlobalSigma { get;  set; }
        /// <summary>
        /// 局部Sigma系数
        /// </summary>
        public double LocalSigma { get; private set; }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GeoPoint() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        /// <param name="num">重数</param>
        public GeoPoint(double lat, double lng, int num)
        {
            this.Lat = lat;
            this.Lng = lng;
            this.Num = num;
        }
        /// <summary>
        /// 带Sigma参数的构造函数
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lng">经度</param>
        /// <param name="num">重数</param>
        /// <param name="localsigma">局部Sigma参数</param>
        public GeoPoint(double lat, double lng, int num,double localsigma)
        {
            this.Lat = lat;
            this.Lng = lng;
            this.Num = num;
            this.LocalSigma = localsigma;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Point:({0},{1})\tNum:{2}", Lat.ToString("0.00") , Lng.ToString("0.00"),this.Num);
        }

        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="pa">点A</param>
        /// <param name="pb">点B</param>
        /// <returns></returns>
        public static double Distance(GeoPoint pa, GeoPoint pb)
        {
            double C = Math.Sin(pa.Lat) * Math.Sin(pb.Lat) * Math.Cos(pa.Lng - pb.Lng) + Math.Cos(pa.Lat) * Math.Cos(pb.Lat);
            double R = 6371.004;
            double sDistance = R * Math.Acos(C) * Math.PI / 180;
            return sDistance;
        }

        /// <summary>
        /// 计算该点到另一点的距离
        /// </summary>
        /// <param name="p">另一点</param>
        /// <returns></returns>
        public double DistanceToPoint(GeoPoint p)
        {
            double C = Math.Sin(this.Lat / 180 * Math.PI) * Math.Sin(p.Lat / 180 * Math.PI) + Math.Cos(this.Lat / 180 * Math.PI) *
                Math.Cos(p.Lat / 180 * Math.PI) * Math.Cos(this.Lng / 180 * Math.PI - p.Lng / 180 * Math.PI);
            double R = 6371004;
            double sDistance = R * Math.Acos(C)/* * Math.PI / 180*/;
            return sDistance;
        }
      
        public static implicit operator PointLatLng(GeoPoint p)
        {
            return new PointLatLng(p.Lat, p.Lng);
        }
        public static implicit operator GeoPoint(PointLatLng p)
        {
            return new GeoPoint(p.Lat, p.Lng, 1);
        }
    }
}
