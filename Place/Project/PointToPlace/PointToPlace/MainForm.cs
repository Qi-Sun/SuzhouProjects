using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointToPlace
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            gMapControl1.MouseMove += gMapControl1_MouseMove;
            gMapControl1.MouseClick += gMapControl1_MouseClick;
        }

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

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
