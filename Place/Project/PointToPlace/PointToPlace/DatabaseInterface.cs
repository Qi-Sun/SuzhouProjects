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

        private string DatabaseIP;
        private string DatabaseSchema;
        private string DatabaseUser;
        private string DatabasePwd;

        public DatabaseInterface()
        {
            PoiTable = "final_poi_suzhou";
            UserTable = "all_user_info";
            CityWeiboTable = "suzhou_weibos_sq";
            CheckinTable = "travel_poi_checkin_weibos_suzhou";
            AllWeiboTable = "travel_poi_users_weibodata_suzhou";

            DatabaseIP = "127.0.0.1";
            DatabaseSchema = "suzhou";
            DatabaseUser = "root";
            DatabasePwd = "19950310";
        }
        public DatabaseInterface(string poiT,string userT,string cityweiboT,string checkinT,string allweiboT,
            string ip ,string  schema,string user ,string pwd)
        {
            PoiTable = poiT;
            UserTable = userT;
            CityWeiboTable = cityweiboT;
            CheckinTable = checkinT;
            AllWeiboTable = allweiboT;
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


    }
}
