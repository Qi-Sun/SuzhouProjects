using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PointToPoint
{
    public class Database
    {
        public DatabaseInfo DBInfo = new DatabaseInfo();
        private string DBpwd;

        public Database(string cityName,string pwd)
        {
            DBInfo.ProduceDefaultDatabaseInfo(cityName);
            DBpwd = pwd;
        }

        public string GetConnectionString()
        {
            //return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\My Course\\数据库\\Student.mdb";
            return "Data Source=127.0.0.1;Database=" + DBInfo.SchemaName + ";User ID=root;Password=" + DBpwd;
            //return "Data Source=127.0.0.1;Database=huangshandata;User ID=root;Password=19950310;";
            //return "Provider=SQLOLEDB;Password=zhangyi;Persist Security Info=True;User ID=sa;Initial Catalog=com;Data Source=(local);Connect Timeout=15";
        }

        public static string GetConnectionString(string Schema,string pwd)
        {
            //return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\\My Course\\数据库\\Student.mdb";
            return "Data Source=127.0.0.1;Database=" + Schema + ";User ID=root;Password=" + pwd;
            //return "Data Source=127.0.0.1;Database=huangshandata;User ID=root;Password=19950310;";
            //return "Provider=SQLOLEDB;Password=zhangyi;Persist Security Info=True;User ID=sa;Initial Catalog=com;Data Source=(local);Connect Timeout=15";
        }

        //没用到过
        public static MySqlDataReader DataReader_ExecuteReader(string connectionStringStr, string sqlStr)
        {
            MySqlConnection cnn = null;
            MySqlCommand cmd = null;
            MySqlDataReader rst = null;

            try
            {
                cnn = new MySqlConnection(connectionStringStr);
                cnn.Open();

                cmd = new MySqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sqlStr;
                cmd.CommandTimeout = 1000;
                rst = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("ex!\n");
            }
            return rst;
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
