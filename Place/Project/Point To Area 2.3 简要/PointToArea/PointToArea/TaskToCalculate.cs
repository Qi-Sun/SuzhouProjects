using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace PointToArea
{
    public class TasktoCalculate
    {
        public int sStartIndex;
        public int sEndIndex;
        public int sInterval;
        public int sCount;
        public string sFPath;
        public double sDensityThreshold;
        public int ID;
        public Database sDatabase;
        public List<POIArea> sPoiAreaList = new List<POIArea>();

        public string sField_Place;
        public string sField_Value;
        public string sField_Marked;

        public TasktoCalculate(int id,int start, int end, int interval, string path, double sDThreshold, POIArea[] pois
            , Database database,string field_Place, string field_Value, string field_Marked)
        {
            ID = id;
            sStartIndex = start;
            sEndIndex = end;
            sInterval = interval;
            sCount = end - start;
            sPoiAreaList.AddRange(pois);
            sFPath = path;
            sDensityThreshold = sDThreshold;
            sDatabase = database;
            sField_Marked = field_Marked;
            sField_Place = field_Place;
            sField_Value = field_Value;

        }

        public delegate void UpdateProgressHandle();
        public event UpdateProgressHandle UpdateProgress;

        public void DoTask()
        {
            string sConnectSQL = sDatabase.GetConnectionString();
            int sPoiAreaCount = sPoiAreaList.Count;
            int sGroupCount = sCount / sInterval + 1;
            int sssssssStartIndex = sStartIndex;
            int eeeeeeeEndIndex = sEndIndex;
            //UpdateUI(() => { progressBar1.Maximum = sGroupCount; progressBar1.Value = 0; });
            for (int g = 0; g < sGroupCount; g++)
            {
                int cStartIndex = sssssssStartIndex;
                int cEndIndex = Math.Min(cStartIndex + sInterval, eeeeeeeEndIndex);
                string sCurPath = sFPath + "Task_"+ID+"_Index_" + cStartIndex + "_" + cEndIndex + ".txt";
                int cInterval = cEndIndex - cStartIndex;
                string sSelectSQL = SQLPattern.Get_CityWeibos(
                    sDatabase.DBInfo.SchemaName, sDatabase.DBInfo.GeoWeibos_Table,
                    cStartIndex, cInterval);
                DataTable sTable = Database.DataTable_ExecuteReader(sConnectSQL, sSelectSQL);
                /*
                UpdateUI(() =>
                {
                    progressBar1.Value++;
                    int sMax = progressBar1.Maximum;
                    int sCur = progressBar1.Value;
                    lblBar1.Text = "  " + sCur + " / " + sMax;
                });*/
                StreamWriter sw = new StreamWriter(sCurPath);
                for (int i = 0; i < cInterval; i++)
                {
                    string sWeiboId = sTable.Rows[i]["id"].ToString();
                    double sWeibo_Lat = double.Parse(sTable.Rows[i]["latitude"].ToString());
                    double sWeibo_Lng = double.Parse(sTable.Rows[i]["longitude"].ToString());
                    Point_LatLng sWeiboPoint = new Point_LatLng(sWeibo_Lat, sWeibo_Lng, 1);
                    List<PredictPlace> sPredictPlaceList = new List<PredictPlace>();
                    for (int j = 0; j < sPoiAreaCount; j++)
                    {
                        string sName = sPoiAreaList[j].PoiName;
                        double sValue = sPoiAreaList[j].DiscriminantFunction_ScaleAdaptive(sWeiboPoint);
                        sPredictPlaceList.Add(new PredictPlace(sName, sValue));
                    }
                    sPredictPlaceList.Sort();
                    if (sPredictPlaceList[0].sPredictValue < sDensityThreshold)
                    {
                        //未分类
                        string sUpdateSQL = SQLPattern.Get__UpdateTable_PredictPlace_OnlyOne(
                            sDatabase.DBInfo.SchemaName, sDatabase.DBInfo.GeoWeibos_Table, sWeiboId,
                            sField_Place, "null", sField_Value, "null", sField_Marked);
                        //Database.Execute_NonQuery(sConnectSQL, sUpdateSQL);
                        sw.WriteLine(sUpdateSQL); sw.Flush();
                    }
                    else if (sPredictPlaceList[0].sPredictValue >= sDensityThreshold)
                    {
                        //仅有一类符合条件
                        string sUpdateSQL = SQLPattern.Get__UpdateTable_PredictPlace_OnlyOne(
                            sDatabase.DBInfo.SchemaName, sDatabase.DBInfo.GeoWeibos_Table, sWeiboId,
                               sField_Place, "'" + sPredictPlaceList[0].sPlaceName + "'",
                                sField_Value, sPredictPlaceList[0].sPredictValue.ToString(), sField_Marked);
                        //Database.Execute_NonQuery(sConnectSQL, sUpdateSQL);
                        sw.WriteLine(sUpdateSQL); sw.Flush();

                    }
                    /*
                    UpdateUI(() =>
                    {
                        progressBar2.Value++;
                        int sMax = progressBar2.Maximum;
                        int sCur = progressBar2.Value;
                        labelStatus.Text = (sCur * 100.0 / sMax).ToString("0.0") + "%   " + sCur + " / " + sMax;
                    });*/
                    sssssssStartIndex = cEndIndex;
                    UpdateProgress();/*
                            StreamWriter sw2 = new StreamWriter("./Log.txt");
                            sw2.WriteLine(i + cStartIndex);
                            sw2.Close();*/
                }
            }
        }
    }
}
