using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointToPlace
{
    public class DatabaseInterface
    {
        /// <summary>
        /// Poi数据表
        /// </summary>
        public string PoiTable { private set; get; }
        /// <summary>
        /// 用户信息表
        /// </summary>
        public string UserTable { get; private set; }
        /// <summary>
        /// 研究城市内微博记录表
        /// </summary>
        public string CityWeiboTable { get; private set; }
        /// <summary>
        /// 用户在全国内的微博记录表
        /// </summary>
        public string AllWeiboTable { get; private set; }
        /// <summary>
        /// 用户在研究城市内的签到微博记录表
        /// </summary>
        public string CheckinTable { get; private set; }
        /// <summary>
        /// 研究城市中的景区信息表
        /// </summary>
        public string ScenicAreaTable { get; private set; }
        /// <summary>
        /// 数据库IP地址
        /// </summary>
        private string DatabaseIP;
        /// <summary>
        /// 数据库
        /// </summary>
        private string DatabaseSchema;
        /// <summary>
        /// 用户
        /// </summary>
        private string DatabaseUser;
        /// <summary>
        /// 登陆密码
        /// </summary>
        private string DatabasePwd;

        public DatabaseInterface()
        {
            PoiTable = "final_poi_suzhou";
            UserTable = "all_user_info";
            CityWeiboTable = "suzhou_weibos_sq";
            CheckinTable = "travel_poi_checkin_weibos_suzhou";
            AllWeiboTable = "travel_poi_users_weibodata_suzhou";
            ScenicAreaTable = "Scenic_Area_Info";


            DatabaseIP = "127.0.0.1";
            DatabaseSchema = "suzhou";
            DatabaseUser = "root";
            DatabasePwd = "19950310";
        }
        public DatabaseInterface(string poiT,string userT,string cityweiboT,string checkinT,string allweiboT,string scenicT,
            string ip ,string  schema,string user ,string pwd)
        {
            PoiTable = poiT;
            UserTable = userT;
            CityWeiboTable = cityweiboT;
            CheckinTable = checkinT;
            AllWeiboTable = allweiboT;
            ScenicAreaTable = scenicT;
            DatabaseIP = ip;
            DatabaseSchema = schema;
            DatabaseUser = user;
            DatabasePwd = pwd;
        }


        public DataTable DataTable_ExecuteReade(string sqlStr)
        {
            string connectionString = string.Format("Data Source={0};Database={1};User ID={2};Password={3}", 
                DatabaseIP, DatabaseSchema, DatabaseUser, DatabasePwd);
            return DataTable_ExecuteReader(connectionString,sqlStr);
        }

        public static DataTable DataTable_ExecuteReader(string connectionStringStr, string sqlStr)
        {
            DataTable dt = null;
            try
            {
                using (var conn = new MySqlConnection(connectionStringStr))
                {
                    conn.Open();
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlStr;
                        cmd.CommandTimeout = 1000;
                        using (MySqlDataAdapter reader = new MySqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();

                            if (reader.Fill(ds) > 0)
                                dt = ds.Tables[0];
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {
                System.Console.WriteLine("ex!\n");
                throw;
            }
            return dt;
        }

        public void Execute_NonQuery(string sqlStr)
        {
            string connectionString = string.Format("Data Source={0};Database={1};User ID={2};Password={3}",
                DatabaseIP, DatabaseSchema, DatabaseUser, DatabasePwd);
            Execute_NonQuery(connectionString, sqlStr);
        }

        public static void Execute_NonQuery(string connStr, string sqlStr)
        {
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlStr;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                    }
                    conn.Close();
                }
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex!\n" + ex.Message);
                throw;
            }
            // AH! MUCH BETTER! NICE, CLEAN, EFFICIENT, HIGH FUNCTIONING CODE!
            // USE THIS APPROACH - I WHOLEHEARTEDLY ENDORSE THIS CODE! :-)
            //string connStr = "server=localhost;user=root;database=world;port=3306;password=******;";
        }

        private string _GetScenicArea = @"SELECT * from {0}.{1} {2};";
        /// <summary>
        /// 获取景区列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public string GetScenicAreaList(int start = 0  , int limit = 0)
        {
            string limitStr = "";
            if (limit != 0)
                limitStr = string.Format(" limit {0},{1}", start, limit);
            return string.Format(_GetScenicArea, this.DatabaseSchema, this.ScenicAreaTable, limitStr);
        }

        private string _GetScenicAreaPOI = @"SELECT * from {0}.{1} where super = {2} {3};";
        /// <summary>
        /// 获取某一景区关联的POI信息
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="limit"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public string GetPoisByScenicArea(string areaName,int limit = 0, int start = 0)
        {
            string limitStr = "";
            if (limit != 0)
                limitStr = string.Format(" limit {0},{1}", start, limit);
            return string.Format(_GetScenicAreaPOI, this.DatabaseSchema, this.PoiTable, areaName, limitStr);
        }

        private string _GetScenicAreaCheckin = @"SELECT * FROM {0}.{1} WHERE annotation_place_poiid in 
(SELECT poiid FROM {0}.{2} WHERE super = {3} ) {4} ;";
        /// <summary>
        /// 获取某一景区关联的签到信息
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="limit"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public string GetCheckinWbByScenicArea(string areaName, int limit = 0, int start = 0)
        {
            string limitStr = "";
            if (limit != 0)
                limitStr = string.Format(" limit {0},{1}", start, limit);
            return string.Format(_GetScenicAreaCheckin, this.DatabaseSchema, this.CheckinTable, this.PoiTable, limitStr);
        }

        private string _GetSomeGeoWeibo = @"SELECT * from {0}.{1} {2};";
        /// <summary>
        /// 获取研究城市内一部分的微博
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public string GetSomeGeoWeibo(int limit = 0, int start = 0)
        {
            string limitStr = "";
            if (limit != 0)
                limitStr = string.Format(" limit {0},{1}", start, limit);
            return string.Format(_GetSomeGeoWeibo, this.DatabaseSchema, this.CityWeiboTable, limitStr);
        }

    }
}
