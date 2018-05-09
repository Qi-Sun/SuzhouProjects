using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointToArea
{
    /// <summary>
    /// 数据库信息
    /// </summary>
    public class DatabaseInfo
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string SchemaName = "SomeCity_Data";
        /// <summary>
        /// 签到微博表名称
        /// </summary>
        public string CheckinWeibos_Table = "SomeCity_CheckinWeibo";
        /// <summary>
        /// 城市内POI表名称
        /// </summary>
        public string Poi_Table = "SomeCity_Poi";
        /// <summary>
        /// 地理微博表名称
        /// </summary>
        public string GeoWeibos_Table = " SomeCity_GeoWeibo";
        /// <summary>
        /// 用户信息表名称
        /// </summary>
        public string UserInfo_Table = " SomeCity_Userinfo";

        /// <summary>
        /// 生成默认数据库表信息
        /// </summary>
        /// <param name="TargetCityName">目标城市名称</param>
        public void ProduceDefaultDatabaseInfo(string TargetCityName)
        {
            TargetCityName = TargetCityName.Replace(' ', '_');
            SchemaName = TargetCityName + "_Data";
            Poi_Table = TargetCityName + "_Poi";
            CheckinWeibos_Table = TargetCityName + "_CheckinWeibo";
            UserInfo_Table = TargetCityName + "_Userinfo";
            GeoWeibos_Table = TargetCityName + "_GeoWeibo";
            return;
        }


    }
}
