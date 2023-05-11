namespace CloudMusicConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FilePanel = new System.Windows.Forms.Panel();
            this.LabelTotal = new System.Windows.Forms.Label();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.LabelFilename = new System.Windows.Forms.Label();
            this.BtnOpen = new System.Windows.Forms.Button();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.LbErrMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.LblWebsite = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // FilePanel
            // 
            this.FilePanel.AllowDrop = true;
            this.FilePanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.FilePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("FilePanel.BackgroundImage")));
            this.FilePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.FilePanel.Location = new System.Drawing.Point(12, 12);
            this.FilePanel.Name = "FilePanel";
            this.FilePanel.Size = new System.Drawing.Size(300, 47);
            this.FilePanel.TabIndex = 1;
            this.FilePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.FilePanel_DragDrop);
            this.FilePanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.FilePanel_DragEnter);
            // 
            // LabelTotal
            // 
            this.LabelTotal.BackColor = System.Drawing.SystemColors.Control;
            this.LabelTotal.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabelTotal.Location = new System.Drawing.Point(12, 70);
            this.LabelTotal.Name = "LabelTotal";
            this.LabelTotal.Size = new System.Drawing.Size(300, 22);
            this.LabelTotal.TabIndex = 2;
            this.LabelTotal.Text = "共0个文件，正在转换第0个";
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(13, 135);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 23);
            this.BtnStart.TabIndex = 3;
            this.BtnStart.Text = "开始转换";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Location = new System.Drawing.Point(94, 135);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(75, 23);
            this.BtnStop.TabIndex = 4;
            this.BtnStop.Text = "停止转换";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // LabelFilename
            // 
            this.LabelFilename.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabelFilename.Location = new System.Drawing.Point(12, 95);
            this.LabelFilename.MaximumSize = new System.Drawing.Size(300, 22);
            this.LabelFilename.Name = "LabelFilename";
            this.LabelFilename.Size = new System.Drawing.Size(300, 22);
            this.LabelFilename.TabIndex = 5;
            this.LabelFilename.Text = "歌名";
            // 
            // BtnOpen
            // 
            this.BtnOpen.Location = new System.Drawing.Point(237, 135);
            this.BtnOpen.Name = "BtnOpen";
            this.BtnOpen.Size = new System.Drawing.Size(75, 23);
            this.BtnOpen.TabIndex = 6;
            this.BtnOpen.Text = "打开";
            this.BtnOpen.UseVisualStyleBackColor = true;
            this.BtnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LbErrMsg,
            this.LblWebsite});
            this.StatusBar.Location = new System.Drawing.Point(0, 172);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(324, 22);
            this.StatusBar.SizingGrip = false;
            this.StatusBar.TabIndex = 7;
            this.StatusBar.Text = "statusStrip1";
            // 
            // LbErrMsg
            // 
            this.LbErrMsg.BackColor = System.Drawing.SystemColors.Control;
            this.LbErrMsg.Name = "LbErrMsg";
            this.LbErrMsg.Size = new System.Drawing.Size(210, 17);
            this.LbErrMsg.Spring = true;
            this.LbErrMsg.Text = "error message";
            this.LbErrMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblWebsite
            // 
            this.LblWebsite.IsLink = true;
            this.LblWebsite.LinkVisited = true;
            this.LblWebsite.Name = "LblWebsite";
            this.LblWebsite.Size = new System.Drawing.Size(99, 17);
            this.LblWebsite.Text = "timetickme.com";
            this.LblWebsite.Click += new System.EventHandler(this.LblWebsite_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 194);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.BtnOpen);
            this.Controls.Add(this.LabelFilename);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.LabelTotal);
            this.Controls.Add(this.FilePanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网易云音乐格式转换器";
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel FilePanel;
        private System.Windows.Forms.Label LabelTotal;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Label LabelFilename;
        private System.Windows.Forms.Button BtnOpen;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel LbErrMsg;
        private System.Windows.Forms.ToolStripStatusLabel LblWebsite;
    }
}

