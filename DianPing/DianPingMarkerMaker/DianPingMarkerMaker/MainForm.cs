using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DianPingMarkerMaker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public string DBIP { get; private set; }
        public string DBSchema { get; private set; }
        public string DBUser { get; private set; }
        public string DBPwd { get; private set; }
        public string DBTScenic { get; private set; }
        public string DBTScenicComment { get; private set; }
        public string ConnettingString;
        public string CurCommentID = null;
        public int PicOrderID = 0;


        public List<SpecificMarker> SpecificMarkerList = new List<SpecificMarker>();
        public Dictionary<string, int> SentimentMarkerCode = new Dictionary<string, int>() { };

        private void MainForm_Load(object sender, EventArgs e)
        {
            DBIP = ConfigHelper.GetAppConfig("Data Source");
            DBSchema = ConfigHelper.GetAppConfig("DataBase");
            DBUser = ConfigHelper.GetAppConfig("User ID");
            DBPwd = ConfigHelper.GetAppConfig("Password");
            DBTScenic = ConfigHelper.GetAppConfig("Scenic Table");
            DBTScenicComment = ConfigHelper.GetAppConfig("ScenicComment Table");
            textBoxDBInfo.Text = string.Format("{0}   {1}   {2}   {3}   {4}", DBIP, DBSchema, DBUser, DBTScenic, DBTScenicComment);
            //写入控件序列
            SpecificMarkerList.Add(this.specificMarker1);
            SpecificMarkerList.Add(this.specificMarker2);
            SpecificMarkerList.Add(this.specificMarker3);
            SpecificMarkerList.Add(this.specificMarker4);
            SpecificMarkerList.Add(this.specificMarker5);
            SpecificMarkerList.Add(this.specificMarker6);
            SpecificMarkerList.Add(this.specificMarker7);
            SpecificMarkerList.Add(this.specificMarker8);
            SpecificMarkerList.Add(this.specificMarker9);
            SpecificMarkerList.Add(this.specificMarker10);
            //录入标记的代码
            SentimentMarkerCode.Add("景区景点", 1);
            SentimentMarkerCode.Add("景区特色", 2);
            SentimentMarkerCode.Add("服务质量", 3);
            SentimentMarkerCode.Add("服务设施", 4);
            SentimentMarkerCode.Add("清洁卫生", 5);
            SentimentMarkerCode.Add("旅游交通", 6);
            SentimentMarkerCode.Add("门票物价", 7);
            SentimentMarkerCode.Add("管理水平", 8);
            SentimentMarkerCode.Add("客流状态", 9);
            SentimentMarkerCode.Add("食品卫生", 10);
            ConnettingString = DatabaseAccess.GetConnectionString(DBIP, DBSchema, DBUser, DBPwd);
            //清除缓存
            string directoryPath = "tempPic/";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            try
            {
                Directory.GetFiles(directoryPath).ToList().ForEach(File.Delete);
            }
            catch { }
        }

        private void bttnDBSetting_Click(object sender, EventArgs e)
        {
            DbServerInfoForm dbinfo = new DbServerInfoForm(DBIP, DBSchema, DBUser, DBPwd, DBTScenic, DBTScenicComment);
            if (dbinfo.ShowDialog(this) == DialogResult.OK)
            {
                DBIP = dbinfo.Ip;
                DBSchema = dbinfo.Schema;
                DBUser = dbinfo.User;
                DBPwd = dbinfo.Pwd;
                DBTScenic = dbinfo.Table_Scenic;
                DBTScenicComment = dbinfo.Table_ScenicComment;
                ConfigHelper.UpdateAppConfig("Data Source", DBIP);
                ConfigHelper.UpdateAppConfig("DataBase", DBSchema);
                ConfigHelper.UpdateAppConfig("User ID", DBUser);
                ConfigHelper.UpdateAppConfig("Password", DBPwd);
                ConfigHelper.UpdateAppConfig("Scenic Table", DBTScenic);
                ConfigHelper.UpdateAppConfig("ScenicComment Table", DBTScenicComment);
                textBoxDBInfo.Text = string.Format("{0}   {1}   {2}   {3}   {4}", DBIP, DBSchema, DBUser, DBTScenic, DBTScenicComment);
                ConnettingString = DatabaseAccess.GetConnectionString(DBIP, DBSchema, DBUser, DBPwd);
            }
        }

        private void specificMarker1_Load(object sender, EventArgs e)
        {

        }

        private void bttnReset_Click(object sender, EventArgs e)
        {
            foreach (var marker in SpecificMarkerList)
            {
                marker.ResetTheRadioButton();
            }
        }

        private void bttnSave_Click(object sender, EventArgs e)
        {
            string positiveClasses = "";
            string positiveCodes = " ";
            string neutralClasses = "";
            string neutralCodes = " ";
            string negativeClasses = "";
            string negativeCodes = " ";
            foreach (var marker in SpecificMarkerList)
            {
                switch (marker.GetMarkerValue())
                {
                    case 1:
                        positiveClasses += string.Format("{0};", marker.SpecificClassName);
                        positiveCodes += string.Format("{0},", SentimentMarkerCode[marker.SpecificClassName]);
                        break;
                    case -1:
                        negativeClasses += string.Format("{0};", marker.SpecificClassName);
                        negativeCodes += string.Format("{0},", SentimentMarkerCode[marker.SpecificClassName]);
                        break;
                    default:
                        neutralClasses += string.Format("{0};", marker.SpecificClassName);
                        neutralCodes += string.Format("{0},", SentimentMarkerCode[marker.SpecificClassName]);
                        break;
                }
            }
            positiveCodes = positiveCodes.Remove(positiveCodes.Length - 1);
            negativeCodes = negativeCodes.Remove(negativeCodes.Length - 1);
            neutralCodes = neutralCodes.Remove(neutralCodes.Length - 1);
            string UpdateSQL_pattern = @"
UPDATE {0}.{1}
SET
    PositiveAspects = '{2}',
    NeutralAspects = '{3}',
    NegativeAspects = '{4}',
    PositiveCodes = '{5}',
    NeutralCodes = '{6}',
    NegativeCodes = '{7}',
    Marked = Marked + 1
WHERE
    com_id = '{8}';
";
            string UpdateSQL = string.Format(UpdateSQL_pattern, this.DBSchema, this.DBTScenicComment,
                positiveClasses, neutralClasses, negativeClasses,
                positiveCodes, neutralCodes, negativeCodes, this.CurCommentID);
            try
            {
                DatabaseAccess.Execute_NonQuery(this.ConnettingString, UpdateSQL);
                labelStatus.Text = "已保存！";
            }
            catch { labelStatus.Text = "出错了…"; }

        }

        private void buttonSelectShop_Click(object sender, EventArgs e)
        {
            string shopTitle = textBoxShopTitle.Text;
            string _SelectShopSQL_Pattern = @"Select * From {0}.{1} WHERE shop_title = '{2}';";
            DataTable shopInfo = DatabaseAccess.DataTable_ExecuteReader(this.ConnettingString,
                string.Format(_SelectShopSQL_Pattern, this.DBSchema, this.DBTScenic, shopTitle));
            listBoxShopInfo.Items.Clear();
            comboBoxShopID.Items.Clear();
            if (shopInfo == null)
            {
                listBoxShopInfo.Items.Add("无当前查询的商铺名称");
                bttnCurShopRndCom.Enabled = false;
                CurCommentID = null;
            }
            else if (shopInfo.Rows.Count > 0)
            {
                bttnCurShopRndCom.Enabled = true;
                for (int i = 0; i < shopInfo.Rows.Count; i++)
                {
                    listBoxShopInfo.Items.Add(string.Format("{0}:{1}", "Shop ID", shopInfo.Rows[i]["shop_id"]));
                    comboBoxShopID.Items.Add(shopInfo.Rows[i]["shop_id"]);
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Type", shopInfo.Rows[i]["shop_type"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Com Count", shopInfo.Rows[i]["com_count"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Mean Price", shopInfo.Rows[i]["mean_price"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Score", shopInfo.Rows[i]["score_each"]));
                    listBoxShopInfo.Items.Add("\r\n");
                }
                comboBoxShopID.SelectedIndex = 0;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bttnCurShopRndCom_Click(object sender, EventArgs e)
        {
            if (comboBoxShopID.SelectedItem == null)
            {
                MessageBox.Show("未选择商铺");
                return;
            }
            PicOrderID++;
            labelStatus.Text = "";
            string shopId = comboBoxShopID.SelectedItem.ToString();
            string _SelectSQL_pattern = @"Select * from {0}.{1} where shop_id = {2} order by rand() limit 1;";
            string selectSQL = string.Format(_SelectSQL_pattern, DBSchema, DBTScenicComment, shopId);
            DataTable comInfo = DatabaseAccess.DataTable_ExecuteReader(this.ConnettingString, selectSQL);
            richTextBox1.Clear();
            if (comInfo == null)
            {
                richTextBox1.AppendText("当前查询无评论");
                CurCommentID = null;
            }
            else
            {
                CurCommentID = comInfo.Rows[0]["com_id"].ToString();
                string comText = "\t" + comInfo.Rows[0]["com_text"].ToString() + "\r\n\r\n";
                string scoreText = "\t" + comInfo.Rows[0]["score"].ToString() + "\r\n\r\n";
                string timeText = "\t" + comInfo.Rows[0]["post_time"].ToString() + "\r\n\r\n";
                string picUrls = comInfo.Rows[0]["com_pic"].ToString();
                string[] picUrlList = picUrls.Split(',');
                string comHeader = "评论内容:\r\n";
                string scoreHeader = "商铺得分：\r\n";
                string timeHeader = "评论时间:";
                string picHeader = "评论图片：\t\n";

                richTextBox1.Text += (timeHeader + timeText);
                richTextBox1.Text += (scoreHeader + scoreText);
                richTextBox1.Text += (comHeader + comText);
                richTextBox1.Text += picHeader;

                richTextBox1.SelectionStart = richTextBox1.Text.Length;

                for (int i = 0; i < picUrlList.Length; i++)
                {
                    if (picUrlList[i].Length < 5)
                        continue;
                    saveImage(picUrlList[i], i);
                    Clipboard.Clear();   //清空剪贴板
                    Bitmap bmp = new Bitmap("tempPic/" + string.Format("temp_{0}_{1}.jpg", PicOrderID, i)); //创建Bitmap类对象
                    Clipboard.SetImage(ZoomBitmap(bmp, richTextBox1.Width - 10));  //将Bitmap类对象写入剪贴板
                    richTextBox1.Paste();   //将剪贴板中的对象粘贴到RichTextBox1
                }

            }
        }

        private void bttnRandomCom_Click(object sender, EventArgs e)
        {
            PicOrderID++;
            labelStatus.Text = "";
            string _SelectSQL_pattern = @"Select * from {0}.{1} order by rand() limit 1;";
            string selectSQL = string.Format(_SelectSQL_pattern, DBSchema, DBTScenicComment);
            DataTable comInfo = DatabaseAccess.DataTable_ExecuteReader(this.ConnettingString, selectSQL);
            richTextBox1.Clear();
            if (comInfo == null)
            {
                richTextBox1.AppendText("当前查询无评论");
                CurCommentID = null;
            }
            else
            {
                CurCommentID = comInfo.Rows[0]["com_id"].ToString();
                string shopId = comInfo.Rows[0]["shop_id"].ToString();
                try
                {
                    string _SelectShopSQL_Pattern = @"Select * From {0}.{1} WHERE shop_id = '{2}';";
                    DataTable shopInfo = DatabaseAccess.DataTable_ExecuteReader(this.ConnettingString,
                        string.Format(_SelectShopSQL_Pattern, this.DBSchema, this.DBTScenic, shopId));
                    listBoxShopInfo.Items.Clear();
                    comboBoxShopID.Items.Clear();
                    if (shopInfo == null)
                    {
                        listBoxShopInfo.Items.Add("无当前查询的商铺名称");
                        bttnCurShopRndCom.Enabled = false;
                        CurCommentID = null;
                    }
                    else if (shopInfo.Rows.Count > 0)
                    {
                        bttnCurShopRndCom.Enabled = true;
                        textBoxShopTitle.Text = shopInfo.Rows[0]["shop_title"].ToString();
                        listBoxShopInfo.Items.Add(string.Format("{0}:{1}", "Shop ID", shopInfo.Rows[0]["shop_id"]));
                        comboBoxShopID.Items.Add(shopInfo.Rows[0]["shop_id"]);
                        listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Type", shopInfo.Rows[0]["shop_type"]));
                        listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Com Count", shopInfo.Rows[0]["com_count"]));
                        listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Mean Price", shopInfo.Rows[0]["mean_price"]));
                        listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Score", shopInfo.Rows[0]["score_each"]));
                        listBoxShopInfo.Items.Add("\r\n");

                        comboBoxShopID.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                string comText = "\t" + comInfo.Rows[0]["com_text"].ToString() + "\r\n\r\n";
                string scoreText = "\t" + comInfo.Rows[0]["score"].ToString() + "\r\n\r\n";
                string timeText = "\t" + comInfo.Rows[0]["post_time"].ToString() + "\r\n\r\n";
                string picUrls = comInfo.Rows[0]["com_pic"].ToString();
                string[] picUrlList = picUrls.Split(',');
                string comHeader = "评论内容:\r\n";
                string scoreHeader = "商铺得分：\r\n";
                string timeHeader = "评论时间:";
                string picHeader = "评论图片：\t\n";

                richTextBox1.Text += (timeHeader + timeText);
                richTextBox1.Text += (scoreHeader + scoreText);
                richTextBox1.Text += (comHeader + comText);
                richTextBox1.Text += picHeader;

                richTextBox1.SelectionStart = richTextBox1.Text.Length;

                for (int i = 0; i < picUrlList.Length; i++)
                {
                    if (picUrlList[i].Length < 5)
                        continue;
                    saveImage(picUrlList[i], i);
                    Clipboard.Clear();   //清空剪贴板
                    Bitmap bmp = new Bitmap("tempPic/" + string.Format("temp_{0}_{1}.jpg", PicOrderID, i)); //创建Bitmap类对象
                    Clipboard.SetImage(ZoomBitmap(bmp, richTextBox1.Width - 10));  //将Bitmap类对象写入剪贴板
                    richTextBox1.Paste();   //将剪贴板中的对象粘贴到RichTextBox1
                }

            }
        }

        private Bitmap ZoomBitmap(Bitmap bitmap, int destWidth)
        {
            System.Drawing.Image sourImage = bitmap;//按比例缩放             
            int sourWidth = sourImage.Width;
            int sourHeight = sourImage.Height;
            if (destWidth < sourWidth)
            {
                int destHeight = (sourHeight * destWidth / sourWidth);
                Bitmap destBitmap = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);
                //设置画布的描绘质量           
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(sourImage, new Rectangle(0, 0, destWidth, destHeight), new Rectangle(0, 0, sourWidth, sourHeight), GraphicsUnit.Pixel);
                g.Dispose();
                //设置压缩质量       
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
                long[] quality = new long[1];
                quality[0] = 100;
                System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                encoderParams.Param[0] = encoderParam;
                sourImage.Dispose();
                return destBitmap;
            }
            else return bitmap;
        }

        public void saveImage(string sUrl, int index)
        {
            WebClient mywebclient = new WebClient();
            string url = sUrl;
            string newfilename = string.Format("temp_{0}_{1}.jpg", PicOrderID, index);
            string filepath = @"tempPic/" + newfilename;
            try
            {
                mywebclient.DownloadFile(url, filepath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonInitialFields_Click(object sender, EventArgs e)
        {
            try
            {
                string alterSQL = @"
ALTER TABLE {0}.{1}
ADD COLUMN `PositiveAspects` VARCHAR(64) NULL COMMENT '正面评价的方面' AFTER `com_pic`,
ADD COLUMN `NeutralAspects` VARCHAR(64) NULL COMMENT '中立评价的方面' AFTER `PositiveAspects`,
ADD COLUMN `NegativeAspects` VARCHAR(64) NULL COMMENT '负面评价的方面' AFTER `NeutralAspects`,
ADD COLUMN `PositiveCodes` VARCHAR(32) NULL COMMENT '正面评价的代码' AFTER `NegativeAspects`,
ADD COLUMN `NeutralCodes` VARCHAR(32) NULL COMMENT '中性评价的代码' AFTER `PositiveCodes`,
ADD COLUMN `NegativeCodes` VARCHAR(32) NULL COMMENT '负面评价的代码' AFTER `NeutralCodes`,
ADD COLUMN `Marked` INT(8) NULL COMMENT '标注的次数' AFTER `NegativeCodes`;
";
                DatabaseAccess.Execute_NonQuery(this.ConnettingString, string.Format(alterSQL, DBSchema, DBTScenicComment));
                string updateSQL = @"
update {0}.{1}
set 
    Marked = 0;
";
                DatabaseAccess.Execute_NonQuery(this.ConnettingString, string.Format(updateSQL, DBSchema, DBTScenicComment));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string directoryPath = "tempPic/";
            try
            {
                Directory.GetFiles(directoryPath).ToList().ForEach(File.Delete);
            }
            catch { }
        }

        private void bttnRandomShop_Click(object sender, EventArgs e)
        {
            string _SelectShopSQL_Pattern = @"Select * From {0}.{1} order by rand() limit 1;";
            DataTable shopInfo = DatabaseAccess.DataTable_ExecuteReader(this.ConnettingString,
                string.Format(_SelectShopSQL_Pattern, this.DBSchema, this.DBTScenic));
            listBoxShopInfo.Items.Clear();
            comboBoxShopID.Items.Clear();
            if (shopInfo == null)
            {
                listBoxShopInfo.Items.Add("无当前查询的商铺名称");
                bttnCurShopRndCom.Enabled = false;
                CurCommentID = null;
            }
            else if (shopInfo.Rows.Count > 0)
            {
                bttnCurShopRndCom.Enabled = true;
                textBoxShopTitle.Text = shopInfo.Rows[0]["shop_title"].ToString();
                for (int i = 0; i < shopInfo.Rows.Count; i++)
                {
                    listBoxShopInfo.Items.Add(string.Format("{0}:{1}", "Shop ID", shopInfo.Rows[i]["shop_id"]));
                    comboBoxShopID.Items.Add(shopInfo.Rows[i]["shop_id"]);
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Type", shopInfo.Rows[i]["shop_type"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Com Count", shopInfo.Rows[i]["com_count"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Mean Price", shopInfo.Rows[i]["mean_price"]));
                    listBoxShopInfo.Items.Add(string.Format("\t {0}:\t{1}", "Shop Score", shopInfo.Rows[i]["score_each"]));
                    listBoxShopInfo.Items.Add("\r\n");
                }
                comboBoxShopID.SelectedIndex = 0;
            }
        }

        private void MainForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string directoryPath = "tempPic/";
            try
            {
                Directory.GetFiles(directoryPath).ToList().ForEach(File.Delete);
            }
            catch { }
        }
    }
}
