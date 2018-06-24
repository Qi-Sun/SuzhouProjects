using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DianPingMarkerMaker
{    

    public partial class SpecificMarker : UserControl
    {
        public string SpecificClassName { get; set; }
        public SpecificMarker()
        {
            InitializeComponent();
        }
        public SpecificMarker(string  classname)
        {
            InitializeComponent();
            SpecificClassName = classname;
        }

        private void SpecificMarker_Load(object sender, EventArgs e)
        {
            lblControlName.Text = SpecificClassName;
        }

        public void ResetTheRadioButton()
        {
            this.rB_neutral.Checked = true;
        }

        public int GetMarkerValue()
        {
            if (this.rB_Positive.Checked == true)
                return 1;
            else if (this.rB_negative.Checked == true)
                return -1;
            else return 0;
        }
    }
}
