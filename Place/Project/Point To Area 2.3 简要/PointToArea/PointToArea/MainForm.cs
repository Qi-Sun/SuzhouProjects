using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using System.IO;
using System.Threading;
using PointToArea.Para;

namespace PointToArea
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            gMapControl1.MouseMove += GMapControl1_MouseMove;
            gMapControl1.MouseClick += GMapControl1_MouseClick;
        }

        private void GMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mMouseClickPosition = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                mMarkerOverlay_MousePosition.Markers.Clear();
                mMarkerOverlay_MousePosition.Markers.Add(new GMarkerGoogle(mMouseClickPosition,GMarkerGoogleType.green_big_go));
                MessageBox.Show(mMouseClickPosition.Lat + "\r\n" + mMouseClickPosition.Lng);
            }
        }

        private void GMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        public GMapOverlay mMarkerOverlay_SuperiorTitle = new GMapOverlay("SuperiorTitle");
        public GMapOverlay mMarkerOverlay_Title = new GMapOverlay("Title");
        public GMapOverlay mMarkerOverlay_Annotation_Place_Title = new GMapOverlay("Annotation_Place_Title");
        public GMapOverlay mMarkerOverlay_TrainingSamples = new GMapOverlay("训练样本");
        public string mPoi_Title = "周庄";
        public string mPoi_SuperiorTitle = "周庄";
        public string mPoi_Annotation_Place_Title = "拙政园";
        public string mConnectSQL = String.Empty;
        public delegate string Function(string a, string b, string c);
        public POIArea sTrainingSamples = new POIArea();
        public PointLatLng mMouseClickPosition = new PointLatLng();
        public GMapOverlay mMarkerOverlay_MousePosition = new GMapOverlay("鼠标点击位置");
        public GMapOverlay mMarkerOverlay_TestData = new GMapOverlay("测试数据");
        public List<POIArea> mPoiAreaList = new List<POIArea>();
        //用以计算地图显示时的中心点
        List<double> mLatList = new List<double>();
        List<double> mLngList = new List<double>();
        //渲染
        private int[] sTypesCode = new int[10] { 3, 32, 9, 14, 6, 28, 7, 22, 36, 37 };
        private Color[] sColorCode = new Color[10] {Color.Blue,Color.Red,Color.Green,Color.Yellow,
            Color.Brown,Color.Purple,Color.Orange,Color.Gray,Color.Black,Color.White};
        public Bitmap mDensityMap = new Bitmap(1, 1);
        public double mDensityThreshold = 10;
        public double Parameter_k = 0.20;
        public List<TasktoCalculate> mTaskList = new List<TasktoCalculate>();
        //2017-06-20 整理添加
        //关键的四个参数
        public Database mDataBase = new Database("suzhou", "19950310");
        public DataDirPara mDataDirPara = new DataDirPara("E:\\Dissertation\\DATA\\Point2Place 2.3Beta\\");
        DbscanPara mDBscanPara = new DbscanPara(500, 6);
        P2PmethodPara mP2PmethodPara = new P2PmethodPara(0, 30456766, 10, 40000, 320000, 6, 0.20);

        public string mField_SuperiorTitleName = "super";
        public string mField_AnnotationPlacePoiid = "annotation_place_poiid";


        private void MainForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(Math.Sin(30.0/180*Math.PI).ToString());
            gMapControl1.Zoom = 15;
            gMapControl1.MapProvider = GMapProviders.GoogleChinaMap;
            gMapControl1.Manager.Mode = AccessMode.ServerAndCache;//地图加载模式
            gMapControl1.MinZoom = 1;   //最小比例
            gMapControl1.MaxZoom = 23; //最大比例
            gMapControl1.DragButton = System.Windows.Forms.MouseButtons.Left;//左键拖拽地图
            gMapControl1.Position = new PointLatLng(23, 113);
            gMapControl1.Overlays.Add(mMarkerOverlay_SuperiorTitle);
            gMapControl1.Overlays.Add(mMarkerOverlay_Title);
            gMapControl1.Overlays.Add(mMarkerOverlay_Annotation_Place_Title);
            mConnectSQL = mDataBase.GetConnectionString();
            //LoadData_SuperiorTitle();
            //POIArea sA = new POIArea("1", "2", null);
            gMapControl1.Overlays.Add(mMarkerOverlay_TrainingSamples);
            gMapControl1.Overlays.Add(mMarkerOverlay_TestData);
            gMapControl1.Overlays.Add(mMarkerOverlay_MousePosition);
            InitializeDataGrid();
            Point_LatLng.Sigma = 100;
            //mMarkerOverlay_SuperiorTitle.Markers.Add(new GMarkerGoogle
            //(new PointLatLng(31.3172692954, 120.680272763), GMarkerGoogleType.red_big_stop));

            //2017年8月13日17:33:47 计算新的苏州景区
            mDataBase.DBInfo.CheckinWeibos_Table = "travel_poi_checkin_weibos_suzhou";
            mDataBase.DBInfo.Poi_Table = "pois_suzhou_813";
            mDataBase.DBInfo.GeoWeibos_Table = "suzhou_weibos_sq2";
            mDataBase.DBInfo.UserInfo_Table = "travel_users_prcity";
            return;
        }

        public void Debug_CalculateDistance()
        {
            int sHeight = gMapControl1.Height;
            int sWidth = gMapControl1.Width;
            double LeftTop_Lat = gMapControl1.FromLocalToLatLng(0, 0).Lat;
            double LeftTop_Lng = gMapControl1.FromLocalToLatLng(0, 0).Lng;
            double RightBottom_Lat = gMapControl1.FromLocalToLatLng(sWidth, sHeight).Lat;
            double RightBottom_Lng = gMapControl1.FromLocalToLatLng(sWidth, sHeight).Lng;
            Point_LatLng p1 = new Point_LatLng(LeftTop_Lat, LeftTop_Lng, 1);
            Point_LatLng p2 = new Point_LatLng(LeftTop_Lat, RightBottom_Lng, 1);
            double sDistance = p1.DistanceToPoint(p2);
            MessageBox.Show(sDistance.ToString());
        }

        public void InitializeDataGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Row Name", "PoiName");
            dataGridView1.Columns.Add("AverageDistance", "AD");
            dataGridView1.Columns.Add("MedianDistance", "MD");
            dataGridView1.Columns.Add("AverageNumber", "AN");
            dataGridView1.Columns.Add("Method 1", "Normal");
            dataGridView1.Columns.Add("Method 2", "No Num");
            dataGridView1.Columns.Add("Method 3", "No Sigma");
            foreach (DataGridViewColumn item in dataGridView1.Columns)
            {
                item.Width = 90;
            }           
        }

        public void OverlayClear()
        {
            foreach (GMapOverlay item in gMapControl1.Overlays)
            {
                item.Markers.Clear();
            }
        }

        public void LoadData_SuperiorTitle()
        {
            
            string sStrSQL = SQLPattern.Extract_POI_by_Superior_Title(mDataBase.DBInfo.SchemaName,
                mDataBase.DBInfo.Poi_Table, mPoi_SuperiorTitle);
            
            DataTable sTable = Database.DataTable_ExecuteReader(mConnectSQL, sStrSQL);
            List<double> sLatList = new List<double>();
            List<double> sLngList = new List<double>();
            if (sTable != null && sTable.Rows.Count == 0)
            {
                MessageBox.Show("No data");
                return;
            }
            int sCount = Math.Min(sTable.Rows.Count, 100);
            OverlayClear();
            for (int i = 0; i < sCount; i++)
            {
                double lat = double.Parse(sTable.Rows[i]["latitude"].ToString());
                double lng = double.Parse(sTable.Rows[i]["longitude"].ToString());
                if (lat == 0 || lng == 0) continue;
                PointLatLng sPoint = new PointLatLng(lat,lng);
                sLatList.Add(lat);sLngList.Add(lng);
                GMapMarker sMarker = new GMarkerGoogle(sPoint,GMarkerGoogleType.blue_pushpin);
                mMarkerOverlay_SuperiorTitle.Markers.Add(sMarker);
            }
            
            PointLatLng sPointCenter = new PointLatLng(sLatList.Average(), sLngList.Average());
            gMapControl1.Position = sPointCenter;
            gMapControl1.Zoom = 15;
            gMapControl1.Refresh();
        }


        //B2094757D06EAAF84698 南锣鼓巷
        //B2094654D26FA1F5499D 颐和园
        //B2094654D26EAAFE499E 北京大学

        ///这是第一步
        private void bttnChkWb2File_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => { CheckinWeiboToFile(); });
        }

        /// 这是第二步，不要在意函数名有些奇怪
        private void TestBttn_Click(object sender, EventArgs e)
        {
            //TestFunction();
            #region  写入文件
            //Task.Factory.StartNew(() => { CheckinWeiboToFile(); });


            #endregion
            #region 展示数据
            /*
            OpenFileDialog sOpenFileDialog = new OpenFileDialog();
            sOpenFileDialog.InitialDirectory = "E:\\Dissertation\\DATA\\suzhou";
            if (sOpenFileDialog.ShowDialog() == DialogResult.OK) 
            {
                ShowCheckWeiboData(sOpenFileDialog.FileName);
            }*/
            #endregion
            //TestFunction_FromFile("拙政园");
            ReadData_ConvertDBScan_Brief(mDBscanPara, mP2PmethodPara);
            //ReadData_ConvertDBScan(500, 6);
        }

        /// 这是第三步，继续忽略奇怪的函数名
        private void bttnDebug_Click(object sender, EventArgs e)
        {
            //Debug_CalculateDistance();
            //Update_DatabaseTable_MultiFiles();
            //P2PmethodPara sP2P_Para = new P2PmethodPara(0, 30456766, 10, 40000, 320000, 6, 0.20);
            Update_DatabaseTable_MultiFiles_MultiTask(mP2PmethodPara);
            //Update_DatabaseTable();
        }

        /// <summary>
        /// 将签到微博提取到文件（方法的第一步）
        /// </summary>
        public void CheckinWeiboToFile()
        {
            string sConnectSQl = mDataBase.GetConnectionString();
            string sSelectSQl = SQLPattern.GetPoiSuperiorTitle(mDataBase.DBInfo.SchemaName
                , mDataBase.DBInfo.Poi_Table, mField_SuperiorTitleName);
            DataTable sTable = Database.DataTable_ExecuteReader(sConnectSQl, sSelectSQl);
            int sCount = sTable.Rows.Count;
            UpdateUI(() => { progressBar1.Maximum = sCount; lblBar1.Text = "0 /" + sCount; });
            for (int i = 0; i < sCount; i++)
            {
                string poiTitle = sTable.Rows[i]["title"].ToString();
                //TestFunction(sTable.Rows[i]["title"].ToString());
                //string sConnectSQl = Database.GetConnectionString();
                string ssSelectSQl = SQLPattern.GetCheckWeibo(mDataBase.DBInfo.SchemaName, mDataBase.DBInfo.CheckinWeibos_Table,
                    mDataBase.DBInfo.Poi_Table, poiTitle, mField_AnnotationPlacePoiid, mField_SuperiorTitleName);
                DataTable ssTable = Database.DataTable_ExecuteReader(sConnectSQl, ssSelectSQl);
                if (ssTable != null) 
                {
                    int sRowCount = ssTable.Rows.Count;
                    POIArea sCurPoiArea = POIArea.ReadFromDataTable(ssTable, poiTitle);
                    POIArea.WriteToFile(sCurPoiArea, mDataDirPara.SampleSetDirectory);
                }
               
                //mMarkerOverlay_TrainingSamples.Markers.Clear();
                UpdateUI(() => { progressBar1.Value++; lblBar1.Text = i + " /" + sCount; });
            }
        }

        /// <summary>
        /// 对样本进行密度聚类（方法的第二部）
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="minpts"></param>
        public void ReadData_ConvertDBScan_Brief(DbscanPara dbscanPara, P2PmethodPara p2pPara)
        {
            double radius = dbscanPara.Radius;
            int minpts = dbscanPara.Minpts;
            double para_k = p2pPara.Para_K;
            OpenFileDialog sOpenFileDialog = new OpenFileDialog();
            //sOpenFileDialog.InitialDirectory = "D:\\sq\\DATA2\\beijing_lz\\Post_DBSCAN";
            string srootPath = mDataDirPara.DbscanResultDirectory;
            string sSubDir = "Radius_" + (int)radius + "_Minpts_" + minpts + "_k_" + para_k + "\\";
            string sPath = srootPath + sSubDir;
            if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);
            sOpenFileDialog.Multiselect = true;
            DBScan sDBScan = new DBScan(radius, minpts);
            int index = 0;
            if (sOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                int sFilesCount = sOpenFileDialog.FileNames.Length;
                progressBar1.Maximum = sFilesCount;
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < sFilesCount; i++)
                    {
                        string sFilename = sOpenFileDialog.FileNames[i];
                        POIArea sCurPoi = POIArea.ReadFromFile(sFilename);
                        POIArea sProcessedPoiArea = sDBScan.ReClassPoiAreaWeibo(sCurPoi);
                        sProcessedPoiArea.CalculateModelParameter(para_k);
                        //TODO
                        List<double> qssLatList = new List<double>();
                        List<double> qssLonList = new List<double>();
                        List<double> qsDistanceToCenter = new List<double>();
                        List<double> qsNumber = new List<double>();
                        if (sProcessedPoiArea.ClassList.Count == 1 && sProcessedPoiArea.ClassList[0].ClassID == -1)
                        {
                            MessageBox.Show(sCurPoi.PoiName);
                        }
                        else
                        {
                            #region 旧代码
                            /*
                            foreach (ClusteringClass sClass in sProcessedPoiArea.ClassList)
                            {
                                if (sClass.ClassID == -1) continue;
                                foreach (Point_LatLng point in sClass.PointList)
                                {
                                    qssLatList.Add(point.Lat);
                                    qssLonList.Add(point.Lng);
                                    qsNumber.Add((point.Num));
                                }
                            }
                            double sCenterLat = qssLatList.Average();
                            double sCenterLng = qssLonList.Average();
                            Point_LatLng sAreaCenter = new Point_LatLng(sCenterLat, sCenterLng, 1);
                            foreach (ClusteringClass sClass in sProcessedPoiArea.ClassList)
                            {
                                if (sClass.ClassID == -1) continue;
                                foreach (Point_LatLng point in sClass.PointList)
                                {
                                    qsDistanceToCenter.Add(point.DistanceToPoint(sAreaCenter));
                                }
                            }
                            qsDistanceToCenter.Sort();
                            qsNumber.Sort();
                            double sSigma = qsDistanceToCenter.Average() * Parameter_k;
                            foreach (ClusteringClass sClass in sProcessedPoiArea.ClassList)
                            {
                                if (sClass.ClassID == -1) continue;
                                foreach (Point_LatLng point in sClass.PointList)
                                {
                                    point.oSigma = sSigma;
                                }
                            }
                        }*/
                            #endregion
                            POIArea.WriteToFile_WithSigma(sProcessedPoiArea, sPath);
                        }
                        UpdateUI(() =>
                        {
                            progressBar1.Value = i + 1;
                            lblBar1.Text = (i + 1) + " / " + sFilesCount;
                        });
                    }
                });

            }
        }

        /// <summary>
        /// 对全部数据进行计算，多文件多线程（方法的第三步）
        /// </summary>
        /// <param name="P2P_Para"></param>
        public void Update_DatabaseTable_MultiFiles_MultiTask(P2PmethodPara P2P_Para)
        {
            int sTaskCount = P2P_Para.TaskMaxNum;

            int sStartIndex = P2P_Para.StartIndex;
            int sEndIndex = P2P_Para.EndIndex;
            int sInterval = P2P_Para.EachTaskInterval;
            int sFileInterval = P2P_Para.EachFileInterval;

            string sField_Place = "Place_k" + (int)(P2P_Para.Para_K * 100);
            string sField_Value = "Value_k" + (int)(P2P_Para.Para_K * 100);
            string sField_Marked = "Marked";


            UpdateUI(() => { progressBar1.Maximum = sTaskCount; });

            int sCurIndex = sStartIndex;
            int sCount = sEndIndex - sStartIndex;
            double sDensityThreshold = P2P_Para.Density_Tao;
            progressBar2.Maximum = sCount;
            //准备数据
            OpenFileDialog sOpenFileDialog = new OpenFileDialog();
            //sOpenFileDialog.InitialDirectory = "E:\\Dissertation\\DATA\\suzhou_DBscan_500_6";
            sOpenFileDialog.Multiselect = true;

            if (sOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<POIArea> sqPoiAreaList = new List<POIArea>();
                //mPoiAreaList.Clear();
                foreach (string item in sOpenFileDialog.FileNames)
                {
                    POIArea sCurPoi = POIArea.ReadFromFile_WithSigma(item);
                    if (sCurPoi.ClassList.Count == 1 && sCurPoi.ClassList[0].ClassID == -1)
                    {
                        MessageBox.Show(sCurPoi.PoiName + "无有效数据");
                    }
                    else
                    {
                        sqPoiAreaList.Add(sCurPoi);
                        sCurPoi.CalculateModelParameter(P2P_Para.Para_K);
                    }
                }
                string sdbscanFileName = sOpenFileDialog.FileNames[0];

                string sRootDir = mDataDirPara.Point2PlaceDirectory;
                string sSubDir = "UpdateFiles_" + FetchSubDir(sdbscanFileName) + "k_" + P2P_Para.Para_K + "\\";
                string sFilePath = sRootDir + sSubDir;
                if (!Directory.Exists(sFilePath)) Directory.CreateDirectory(sFilePath);
                //模型建好了
                MessageBox.Show("Models Finished");

                for (int t = 0; t < sTaskCount; t++)
                {
                    int sStart = t * sInterval;
                    int sEnd = Math.Min(sStart + sInterval, sEndIndex);
                    TasktoCalculate sTask = new TasktoCalculate(t, sStart, sEnd, sFileInterval, sFilePath,
                        sDensityThreshold, sqPoiAreaList.ToArray(), mDataBase, sField_Place, sField_Value, sField_Marked);
                    sTask.UpdateProgress += this.UpdateProgress_;
                    mTaskList.Add(sTask);
                    Task.Factory.StartNew(() => { sTask.DoTask(); });
                    Thread.Sleep(2000);
                    UpdateUI(() => {
                        progressBar1.Value++;
                        int sCur = progressBar1.Value;
                        int sMax = progressBar1.Maximum;
                        lblBar1.Text= (sCur * 100.0 / sMax).ToString("0.0") + "%   " + sCur + " / " + sMax;
                    });
                }
            }

        }

        /// <summary>
        /// 生成子文件夹名称（在第三步有引用）
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public string FetchSubDir(string Filename)
        {
            string returnString = "";
            string[] sPlits = Filename.Split('\\');
            int sLength = sPlits.Length;
            string sSubDir = sPlits[sLength - 2];
            string[] ssPlits = sSubDir.Split('_');
            returnString += ssPlits[0] + "_";
            returnString += ssPlits[1] + "_";
            returnString += ssPlits[2] + "_";
            returnString += ssPlits[3] + "_";
            return returnString;
        }      

        public POIArea ShowSelectedData_withSigma(string fileName, GMarkerGoogleType type = GMarkerGoogleType.blue_small)
        {

            POIArea sCurPoi = POIArea.ReadFromFile_WithSigma(fileName);
            sCurPoi.CalculateModelParameter(Parameter_k);
            //mMarkerOverlay_TestData.Markers.Clear();
            //List<double> sLatList = new List<double>();
            //List<double> sLngList = new List<double>();
            int sClassCount = sCurPoi.ClassList.Count;
            for (int j = 0; j < sClassCount; j++)
            {
                int sRowCount = sCurPoi.ClassList[j].PointList.Count;
                ClusteringClass sClass = sCurPoi.ClassList[j];
                if (sClass.ClassID == -1) continue;
                for (int i = 0; i < sRowCount; i++)
                {
                    double slat = sClass.PointList[i].Lat;
                    double slng = sClass.PointList[i].Lng;
                    if (slat == 0 || slng == 0) continue;
                    mLatList.Add(slat); mLngList.Add(slng);
                    GMapMarker sMarker =
                        new GMarkerGoogle(new PointLatLng(slat, slng), type);
                    sMarker.ToolTipText = sCurPoi.PoiName + "\r\n" + sClass.PointList[i].Num.ToString()
                        + ":" + sClass.ClassID + "\r\n";
                    mMarkerOverlay_TestData.Markers.Add(sMarker);
                }
            }

            //PointLatLng sPointCenter = new PointLatLng(sLatList.Average(), sLngList.Average());
            //gMapControl1.Position = sPointCenter;
            //gMapControl1.Zoom = 15;
            gMapControl1.Refresh();
            return sCurPoi;
        }

        private void bttnReadData_Click(object sender, EventArgs e)
        {
            ReadData_WithSigma();
        }

        public void ReadData_WithSigma()
        {
            OpenFileDialog sOpenFileDialog = new OpenFileDialog();
            //sOpenFileDialog.InitialDirectory = "E:\\Dissertation\\DATA";
            sOpenFileDialog.Multiselect = true;

            int index = 0;
            if (sOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                mPoiAreaList.Clear();
                mMarkerOverlay_TestData.Markers.Clear();
                InitializeDataGrid();
                mLatList.Clear(); mLngList.Clear();
                foreach (string item in sOpenFileDialog.FileNames)
                {
                    POIArea sCurPoi =
                    ShowSelectedData_withSigma(item, (GMarkerGoogleType)sTypesCode[index % 10]);
                    sCurPoi.CalculateModelParameter(Parameter_k);
                    if (sCurPoi.ClassList.Count == 1 && sCurPoi.ClassList[0].ClassID == -1)
                    {
                        MessageBox.Show(sCurPoi.PoiName+ "无有效数据");
                    }
                    else
                    {
                        //计算平均距离
                        List<double> ssLatList = new List<double>();
                        List<double> ssLonList = new List<double>();
                        List<double> sDistanceToCenter = new List<double>();
                        List<double> sNumber = new List<double>();
                        foreach (ClusteringClass sClass in sCurPoi.ClassList)
                        {
                            if (sClass.ClassID == -1) continue;
                            foreach (Point_LatLng point in sClass.PointList)
                            {
                                ssLatList.Add(point.Lat);
                                ssLonList.Add(point.Lng);
                                sNumber.Add((point.Num));
                            }
                        }
                        double sCenterLat = ssLatList.Average();
                        double sCenterLng = ssLonList.Average();
                        Point_LatLng sAreaCenter = new Point_LatLng(sCenterLat, sCenterLng, 1);
                        foreach (ClusteringClass sClass in sCurPoi.ClassList)
                        {
                            if (sClass.ClassID == -1) continue;
                            foreach (Point_LatLng point in sClass.PointList)
                            {
                                sDistanceToCenter.Add(point.DistanceToPoint(sAreaCenter));
                            }
                        }
                        sDistanceToCenter.Sort();
                        sNumber.Sort();
                        //更新数据表
                        mPoiAreaList.Add(sCurPoi);
                        dataGridView1.Rows.Add();
                        dataGridView1[0, index].Value = sCurPoi.PoiName;
                        dataGridView1[1, index].Value = sDistanceToCenter.Average();
                        dataGridView1[2, index].Value = sDistanceToCenter[sDistanceToCenter.Count / 2];
                        dataGridView1[3, index].Value = sNumber.Average();
                        index++;
                    }
                    PointLatLng sPointCenter = new PointLatLng(mLatList.Average(), mLngList.Average());
                    gMapControl1.Position = sPointCenter;
                    mMouseClickPosition = sPointCenter;
                    gMapControl1.Zoom = 15;
                    gMapControl1.Refresh();
                }
            }
        }

        private void bttnCalculate_Click(object sender, EventArgs e)
        {
            int sAreaCount = mPoiAreaList.Count;
            Point_LatLng.Sigma = (double)numericUpDown1.Value;
            for (int i = 0; i < sAreaCount; i++)
            {
                dataGridView1[4, i].Value =
                    mPoiAreaList[i].ClassesAffectFunction(mMouseClickPosition).ToString("0.000");
                dataGridView1[5, i].Value =
                    mPoiAreaList[i].ClassesAffectFunction_NoNum(mMouseClickPosition).ToString("0.000");
                dataGridView1[6, i].Value =
                    mPoiAreaList[i].DiscriminantFunction_ScaleAdaptive(mMouseClickPosition).ToString("0.000");
            }
            dataGridView1.Refresh();
        }

        private void bttnDensity_Click(object sender, EventArgs e)
        {
            //Debug_3_Methods();
            //CreateAllDataFiles();
            Debug_1_Method();
        }

        public void Debug_1_Method()
        {
            labelStatus.Text = "Waiting...";
            mDensityThreshold = (double)numericUpDown4.Value;   //阈值
            int sHeight = gMapControl1.Height;
            int sWidth = gMapControl1.Width;
            double LeftTop_Lat = gMapControl1.FromLocalToLatLng(0, 0).Lat;
            double LeftTop_Lng = gMapControl1.FromLocalToLatLng(0, 0).Lng;
            double RightBottom_Lat = gMapControl1.FromLocalToLatLng(sWidth, sHeight).Lat;
            double RightBottom_Lng = gMapControl1.FromLocalToLatLng(sWidth, sHeight).Lng;
            int sScale = (int)numericUpDown2.Value;
            double sDelta_Lat = (RightBottom_Lat - LeftTop_Lat) * 1.0 * sScale / sHeight;
            double sDelta_Lng = (RightBottom_Lng - LeftTop_Lng) * 1.0 * sScale / sWidth;
            int sCount_X = sWidth / sScale;
            int sCount_Y = sHeight / sScale;
            double sOffset_Lat = LeftTop_Lat + sDelta_Lat / 2;
            double sOffset_Lng = LeftTop_Lng + sDelta_Lng / 2;
            Bitmap sBitmap = new Bitmap(sWidth, sHeight);
            Graphics g = Graphics.FromImage(sBitmap);
            double sThreshold = mDensityThreshold;
            List<double> sSamplesDensityList = new List<double>();
            if (mPoiAreaList.Count == 1)
            {
                double[,] sResult = new double[sCount_X, sCount_Y];
                POIArea sCurArea = mPoiAreaList[0];
                double sMax = -99999;
                double sMin = 99999;
                for (int i = 0; i < sCount_X; i++)
                {
                    for (int j = 0; j < sCount_Y; j++)
                    {
                        sResult[i, j] = sCurArea.DiscriminantFunction_ScaleAdaptive(new Point_LatLng(
                            sOffset_Lat + j * sDelta_Lat, sOffset_Lng + i * sDelta_Lng, 1));
                        sMax = Math.Max(sMax, sResult[i, j]);
                        sMin = Math.Min(sMin, sResult[i, j]);
                    }
                }
                for (int i = 0; i < sCount_X; i++)
                {
                    for (int j = 0; j < sCount_Y; j++)
                    {
                        double ssProb = sResult[i, j];
                        //ssProb = (sProb + sMax) / (sMax + sMax);
                        ssProb = (ssProb - sMin) / (sMax - sMin);
                        int sValue = 0;
                        //sValue = Math.Min(Math.Max((int)(255 * ssProb), 0), 255);
                        sValue = (int)(255 * ssProb);
                        Color sCurColor = Color.FromArgb(sValue, sValue, sValue);
                        Graphics gg = pictureBox1.CreateGraphics();
                        if (sResult[i, j] < sThreshold)
                        {
                            g.FillRectangle(new SolidBrush(Color.White), i * sScale, j * sScale, sScale, sScale);
                            gg.FillRectangle(new SolidBrush(Color.White), i * sScale, j * sScale, sScale, sScale);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(sCurColor), i * sScale, j * sScale, sScale, sScale);
                            gg.FillRectangle(new SolidBrush(sCurColor), i * sScale, j * sScale, sScale, sScale);
                        }
                    }
                }

            }
            else
            {
                int sAreaCount = mPoiAreaList.Count;
                int sMaxDensityIndex = -1;
                double sMaxDensityValue = -1;
                double sCurDensityValue = 0;
                Graphics gg = pictureBox1.CreateGraphics();
                for (int i = 0; i < sCount_X; i++)
                {
                    for (int j = 0; j < sCount_Y; j++)
                    {
                        sCurDensityValue = 0;
                        sMaxDensityValue = -1;
                        sMaxDensityIndex = -1;
                        for (int k = 0; k < sAreaCount; k++)
                        {
                            sCurDensityValue = mPoiAreaList[k].DiscriminantFunction_ScaleAdaptive(new Point_LatLng(
                            sOffset_Lat + j * sDelta_Lat, sOffset_Lng + i * sDelta_Lng, 1));
                            if (sMaxDensityValue < sCurDensityValue)
                            {
                                sMaxDensityValue = sCurDensityValue;
                                sMaxDensityIndex = k;
                            }
                        }
                        if (sMaxDensityValue < sThreshold)
                        {
                            g.FillRectangle(new SolidBrush(Color.White),
                            i * sScale, j * sScale, sScale, sScale);
                            gg.FillRectangle(new SolidBrush(Color.White),
                                i * sScale, j * sScale, sScale, sScale);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(sColorCode[sMaxDensityIndex % 10]),
                            i * sScale, j * sScale, sScale, sScale);
                            gg.FillRectangle(new SolidBrush(sColorCode[sMaxDensityIndex % 10]),
                                i * sScale, j * sScale, sScale, sScale);
                        }

                    }
                }
            }
            mDensityMap = sBitmap;
            pictureBox1.Refresh();
            labelStatus.Text = "Finished.";
        }

        public void UpdateProgress_()
        {
            UpdateUI(() =>
            {
                progressBar2.Value++;
                int sMax = progressBar2.Maximum;
                int sCur = progressBar2.Value;
                labelStatus.Text = (sCur * 100.0 / sMax).ToString("0.0") + "%   " + sCur + " / " + sMax;
            });
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(mDensityMap, 0, 0);
        }


        public void UpdateUI(Action uiAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(uiAction));
            }
            else
            {
                uiAction();
            }
        }


    }
}
