using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DianPingMarkerMaker
{
    public partial class DbServerInfoForm : Form
    {
        public string Ip;
        public string Schema;
        public string User;
        public string Pwd;
        public string Table_Scenic;
        public string Table_ScenicComment;

        public DbServerInfoForm(string ip,string schema,string user,string pwd,string table_scenic,string table_scenicComment)
        {
            InitializeComponent();
            Ip = ip;
            Schema = schema;
            User = user;
            Pwd = pwd;
            Table_Scenic = table_scenic;
            Table_ScenicComment = table_scenicComment;
        }

        private void DbServerInfoForm_Load(object sender, EventArgs e)
        {
            textBoxIP.Text = Ip;
            textBoxSchema.Text = Schema;
            textBoxUser.Text = User;
            textBoxPasswd.Text = Pwd;
            textBoxScencicTable.Text = Table_Scenic;
            textBoxScenicCommentTable.Text = Table_ScenicComment;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Ip =textBoxIP.Text ;
            Schema =textBoxSchema.Text ;
            User =textBoxUser.Text  ;
            Pwd = textBoxPasswd.Text;
            Table_Scenic = textBoxScencicTable.Text ;
            Table_ScenicComment = textBoxScenicCommentTable.Text ;
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
