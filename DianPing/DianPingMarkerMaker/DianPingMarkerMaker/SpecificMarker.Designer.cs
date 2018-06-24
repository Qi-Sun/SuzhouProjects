namespace DianPingMarkerMaker
{
    partial class SpecificMarker
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblControlName = new System.Windows.Forms.Label();
            this.rB_Positive = new System.Windows.Forms.RadioButton();
            this.rB_neutral = new System.Windows.Forms.RadioButton();
            this.rB_negative = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblControlName
            // 
            this.lblControlName.AutoSize = true;
            this.lblControlName.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblControlName.Location = new System.Drawing.Point(15, 11);
            this.lblControlName.Name = "lblControlName";
            this.lblControlName.Size = new System.Drawing.Size(67, 14);
            this.lblControlName.TabIndex = 0;
            this.lblControlName.Text = "类别名称";
            // 
            // rB_Positive
            // 
            this.rB_Positive.AutoSize = true;
            this.rB_Positive.Location = new System.Drawing.Point(94, 11);
            this.rB_Positive.Name = "rB_Positive";
            this.rB_Positive.Size = new System.Drawing.Size(47, 16);
            this.rB_Positive.TabIndex = 1;
            this.rB_Positive.Text = "正面";
            this.rB_Positive.UseVisualStyleBackColor = true;
            // 
            // rB_neutral
            // 
            this.rB_neutral.AutoSize = true;
            this.rB_neutral.Checked = true;
            this.rB_neutral.Location = new System.Drawing.Point(178, 11);
            this.rB_neutral.Name = "rB_neutral";
            this.rB_neutral.Size = new System.Drawing.Size(47, 16);
            this.rB_neutral.TabIndex = 2;
            this.rB_neutral.TabStop = true;
            this.rB_neutral.Text = "中性";
            this.rB_neutral.UseVisualStyleBackColor = true;
            // 
            // rB_negative
            // 
            this.rB_negative.AutoSize = true;
            this.rB_negative.Location = new System.Drawing.Point(258, 11);
            this.rB_negative.Name = "rB_negative";
            this.rB_negative.Size = new System.Drawing.Size(47, 16);
            this.rB_negative.TabIndex = 3;
            this.rB_negative.Text = "负面";
            this.rB_negative.UseVisualStyleBackColor = true;
            // 
            // SpecificMarker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rB_negative);
            this.Controls.Add(this.rB_neutral);
            this.Controls.Add(this.rB_Positive);
            this.Controls.Add(this.lblControlName);
            this.Name = "SpecificMarker";
            this.Size = new System.Drawing.Size(330, 35);
            this.Load += new System.EventHandler(this.SpecificMarker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblControlName;
        private System.Windows.Forms.RadioButton rB_Positive;
        private System.Windows.Forms.RadioButton rB_neutral;
        private System.Windows.Forms.RadioButton rB_negative;
    }
}
