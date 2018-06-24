namespace DianPingMarkerMaker
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonInitialFields = new System.Windows.Forms.Button();
            this.bttnDBSetting = new System.Windows.Forms.Button();
            this.textBoxDBInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bttnRandomShop = new System.Windows.Forms.Button();
            this.bttnRandomCom = new System.Windows.Forms.Button();
            this.bttnCurShopRndCom = new System.Windows.Forms.Button();
            this.comboBoxShopID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.listBoxShopInfo = new System.Windows.Forms.ListBox();
            this.buttonSelectShop = new System.Windows.Forms.Button();
            this.textBoxShopTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.bttnSave = new System.Windows.Forms.Button();
            this.bttnReset = new System.Windows.Forms.Button();
            this.specificMarker10 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker9 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker8 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker7 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker6 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker5 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker4 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker3 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker2 = new DianPingMarkerMaker.SpecificMarker();
            this.specificMarker1 = new DianPingMarkerMaker.SpecificMarker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonInitialFields);
            this.groupBox1.Controls.Add(this.bttnDBSetting);
            this.groupBox1.Controls.Add(this.textBoxDBInfo);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(842, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库连接信息";
            // 
            // buttonInitialFields
            // 
            this.buttonInitialFields.Location = new System.Drawing.Point(761, 18);
            this.buttonInitialFields.Name = "buttonInitialFields";
            this.buttonInitialFields.Size = new System.Drawing.Size(75, 23);
            this.buttonInitialFields.TabIndex = 2;
            this.buttonInitialFields.Text = "初始化字段";
            this.buttonInitialFields.UseVisualStyleBackColor = true;
            this.buttonInitialFields.Click += new System.EventHandler(this.buttonInitialFields_Click);
            // 
            // bttnDBSetting
            // 
            this.bttnDBSetting.Location = new System.Drawing.Point(653, 18);
            this.bttnDBSetting.Name = "bttnDBSetting";
            this.bttnDBSetting.Size = new System.Drawing.Size(75, 23);
            this.bttnDBSetting.TabIndex = 0;
            this.bttnDBSetting.Text = "配置";
            this.bttnDBSetting.UseVisualStyleBackColor = true;
            this.bttnDBSetting.Click += new System.EventHandler(this.bttnDBSetting_Click);
            // 
            // textBoxDBInfo
            // 
            this.textBoxDBInfo.Location = new System.Drawing.Point(6, 20);
            this.textBoxDBInfo.Name = "textBoxDBInfo";
            this.textBoxDBInfo.ReadOnly = true;
            this.textBoxDBInfo.Size = new System.Drawing.Size(614, 21);
            this.textBoxDBInfo.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.bttnRandomShop);
            this.groupBox2.Controls.Add(this.bttnRandomCom);
            this.groupBox2.Controls.Add(this.bttnCurShopRndCom);
            this.groupBox2.Controls.Add(this.comboBoxShopID);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Controls.Add(this.listBoxShopInfo);
            this.groupBox2.Controls.Add(this.buttonSelectShop);
            this.groupBox2.Controls.Add(this.textBoxShopTitle);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(818, 649);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "商铺与评论数据";
            // 
            // bttnRandomShop
            // 
            this.bttnRandomShop.Location = new System.Drawing.Point(384, 23);
            this.bttnRandomShop.Name = "bttnRandomShop";
            this.bttnRandomShop.Size = new System.Drawing.Size(75, 23);
            this.bttnRandomShop.TabIndex = 9;
            this.bttnRandomShop.Text = "随机商铺";
            this.bttnRandomShop.UseVisualStyleBackColor = true;
            this.bttnRandomShop.Click += new System.EventHandler(this.bttnRandomShop_Click);
            // 
            // bttnRandomCom
            // 
            this.bttnRandomCom.Location = new System.Drawing.Point(360, 149);
            this.bttnRandomCom.Name = "bttnRandomCom";
            this.bttnRandomCom.Size = new System.Drawing.Size(99, 23);
            this.bttnRandomCom.TabIndex = 8;
            this.bttnRandomCom.Text = "随机商铺评论";
            this.bttnRandomCom.UseVisualStyleBackColor = true;
            this.bttnRandomCom.Click += new System.EventHandler(this.bttnRandomCom_Click);
            // 
            // bttnCurShopRndCom
            // 
            this.bttnCurShopRndCom.Location = new System.Drawing.Point(223, 149);
            this.bttnCurShopRndCom.Name = "bttnCurShopRndCom";
            this.bttnCurShopRndCom.Size = new System.Drawing.Size(111, 23);
            this.bttnCurShopRndCom.TabIndex = 7;
            this.bttnCurShopRndCom.Text = "当前商铺随机评论";
            this.bttnCurShopRndCom.UseVisualStyleBackColor = true;
            this.bttnCurShopRndCom.Click += new System.EventHandler(this.bttnCurShopRndCom_Click);
            // 
            // comboBoxShopID
            // 
            this.comboBoxShopID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxShopID.FormattingEnabled = true;
            this.comboBoxShopID.Location = new System.Drawing.Point(64, 151);
            this.comboBoxShopID.Name = "comboBoxShopID";
            this.comboBoxShopID.Size = new System.Drawing.Size(121, 20);
            this.comboBoxShopID.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "ShopID";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(8, 184);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(804, 459);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // listBoxShopInfo
            // 
            this.listBoxShopInfo.FormattingEnabled = true;
            this.listBoxShopInfo.ItemHeight = 12;
            this.listBoxShopInfo.Location = new System.Drawing.Point(8, 52);
            this.listBoxShopInfo.Name = "listBoxShopInfo";
            this.listBoxShopInfo.Size = new System.Drawing.Size(451, 76);
            this.listBoxShopInfo.TabIndex = 3;
            // 
            // buttonSelectShop
            // 
            this.buttonSelectShop.Location = new System.Drawing.Point(291, 23);
            this.buttonSelectShop.Name = "buttonSelectShop";
            this.buttonSelectShop.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectShop.TabIndex = 2;
            this.buttonSelectShop.Text = "查询";
            this.buttonSelectShop.UseVisualStyleBackColor = true;
            this.buttonSelectShop.Click += new System.EventHandler(this.buttonSelectShop_Click);
            // 
            // textBoxShopTitle
            // 
            this.textBoxShopTitle.Location = new System.Drawing.Point(82, 25);
            this.textBoxShopTitle.Name = "textBoxShopTitle";
            this.textBoxShopTitle.Size = new System.Drawing.Size(194, 21);
            this.textBoxShopTitle.TabIndex = 1;
            this.textBoxShopTitle.Text = "拙政园";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ShopTitle";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.labelStatus);
            this.groupBox3.Controls.Add(this.bttnSave);
            this.groupBox3.Controls.Add(this.bttnReset);
            this.groupBox3.Controls.Add(this.specificMarker10);
            this.groupBox3.Controls.Add(this.specificMarker9);
            this.groupBox3.Controls.Add(this.specificMarker8);
            this.groupBox3.Controls.Add(this.specificMarker7);
            this.groupBox3.Controls.Add(this.specificMarker6);
            this.groupBox3.Controls.Add(this.specificMarker5);
            this.groupBox3.Controls.Add(this.specificMarker4);
            this.groupBox3.Controls.Add(this.specificMarker3);
            this.groupBox3.Controls.Add(this.specificMarker2);
            this.groupBox3.Controls.Add(this.specificMarker1);
            this.groupBox3.Location = new System.Drawing.Point(836, 80);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(349, 493);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "标注";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(194, 468);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(11, 12);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = " ";
            // 
            // bttnSave
            // 
            this.bttnSave.Location = new System.Drawing.Point(196, 433);
            this.bttnSave.Name = "bttnSave";
            this.bttnSave.Size = new System.Drawing.Size(75, 23);
            this.bttnSave.TabIndex = 11;
            this.bttnSave.Text = "保存";
            this.bttnSave.UseVisualStyleBackColor = true;
            this.bttnSave.Click += new System.EventHandler(this.bttnSave_Click);
            // 
            // bttnReset
            // 
            this.bttnReset.Location = new System.Drawing.Point(57, 433);
            this.bttnReset.Name = "bttnReset";
            this.bttnReset.Size = new System.Drawing.Size(75, 23);
            this.bttnReset.TabIndex = 10;
            this.bttnReset.Text = "重置";
            this.bttnReset.UseVisualStyleBackColor = true;
            this.bttnReset.Click += new System.EventHandler(this.bttnReset_Click);
            // 
            // specificMarker10
            // 
            this.specificMarker10.Location = new System.Drawing.Point(6, 389);
            this.specificMarker10.Name = "specificMarker10";
            this.specificMarker10.Size = new System.Drawing.Size(330, 35);
            this.specificMarker10.SpecificClassName = "食品卫生";
            this.specificMarker10.TabIndex = 9;
            // 
            // specificMarker9
            // 
            this.specificMarker9.Location = new System.Drawing.Point(6, 348);
            this.specificMarker9.Name = "specificMarker9";
            this.specificMarker9.Size = new System.Drawing.Size(330, 35);
            this.specificMarker9.SpecificClassName = "客流状态";
            this.specificMarker9.TabIndex = 8;
            // 
            // specificMarker8
            // 
            this.specificMarker8.Location = new System.Drawing.Point(6, 307);
            this.specificMarker8.Name = "specificMarker8";
            this.specificMarker8.Size = new System.Drawing.Size(330, 35);
            this.specificMarker8.SpecificClassName = "管理水平";
            this.specificMarker8.TabIndex = 7;
            // 
            // specificMarker7
            // 
            this.specificMarker7.Location = new System.Drawing.Point(6, 266);
            this.specificMarker7.Name = "specificMarker7";
            this.specificMarker7.Size = new System.Drawing.Size(330, 35);
            this.specificMarker7.SpecificClassName = "门票物价";
            this.specificMarker7.TabIndex = 6;
            // 
            // specificMarker6
            // 
            this.specificMarker6.Location = new System.Drawing.Point(6, 225);
            this.specificMarker6.Name = "specificMarker6";
            this.specificMarker6.Size = new System.Drawing.Size(330, 35);
            this.specificMarker6.SpecificClassName = "旅游交通";
            this.specificMarker6.TabIndex = 5;
            // 
            // specificMarker5
            // 
            this.specificMarker5.Location = new System.Drawing.Point(6, 184);
            this.specificMarker5.Name = "specificMarker5";
            this.specificMarker5.Size = new System.Drawing.Size(330, 35);
            this.specificMarker5.SpecificClassName = "清洁卫生";
            this.specificMarker5.TabIndex = 4;
            // 
            // specificMarker4
            // 
            this.specificMarker4.Location = new System.Drawing.Point(6, 143);
            this.specificMarker4.Name = "specificMarker4";
            this.specificMarker4.Size = new System.Drawing.Size(330, 35);
            this.specificMarker4.SpecificClassName = "服务设施";
            this.specificMarker4.TabIndex = 3;
            // 
            // specificMarker3
            // 
            this.specificMarker3.Location = new System.Drawing.Point(6, 102);
            this.specificMarker3.Name = "specificMarker3";
            this.specificMarker3.Size = new System.Drawing.Size(330, 35);
            this.specificMarker3.SpecificClassName = "服务质量";
            this.specificMarker3.TabIndex = 2;
            // 
            // specificMarker2
            // 
            this.specificMarker2.Location = new System.Drawing.Point(6, 61);
            this.specificMarker2.Name = "specificMarker2";
            this.specificMarker2.Size = new System.Drawing.Size(330, 35);
            this.specificMarker2.SpecificClassName = "景区特色";
            this.specificMarker2.TabIndex = 1;
            // 
            // specificMarker1
            // 
            this.specificMarker1.Location = new System.Drawing.Point(6, 20);
            this.specificMarker1.Name = "specificMarker1";
            this.specificMarker1.Size = new System.Drawing.Size(330, 35);
            this.specificMarker1.SpecificClassName = "景区景点";
            this.specificMarker1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1197, 741);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DianPing MarkerMaker";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.MainForm_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bttnDBSetting;
        private System.Windows.Forms.TextBox textBoxDBInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private SpecificMarker specificMarker1;
        private SpecificMarker specificMarker10;
        private SpecificMarker specificMarker9;
        private SpecificMarker specificMarker8;
        private SpecificMarker specificMarker7;
        private SpecificMarker specificMarker6;
        private SpecificMarker specificMarker5;
        private SpecificMarker specificMarker4;
        private SpecificMarker specificMarker3;
        private SpecificMarker specificMarker2;
        private System.Windows.Forms.Button bttnSave;
        private System.Windows.Forms.Button bttnReset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSelectShop;
        private System.Windows.Forms.TextBox textBoxShopTitle;
        private System.Windows.Forms.ListBox listBoxShopInfo;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button bttnRandomCom;
        private System.Windows.Forms.Button bttnCurShopRndCom;
        private System.Windows.Forms.ComboBox comboBoxShopID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonInitialFields;
        private System.Windows.Forms.Button bttnRandomShop;
        private System.Windows.Forms.Label labelStatus;
    }
}

