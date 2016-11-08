namespace gTronic
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.chkEnablePreview = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.chkDebuggingMode = new System.Windows.Forms.CheckBox();
            this.chkAutoPlay = new System.Windows.Forms.CheckBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnSaveXY = new System.Windows.Forms.Button();
            this.chkFullLog = new System.Windows.Forms.CheckBox();
            this.chkHidePicture = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(9, 501);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(59, 30);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(280, 390);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(74, 502);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(67, 30);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(220, 503);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(74, 29);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkEnablePreview
            // 
            this.chkEnablePreview.AutoSize = true;
            this.chkEnablePreview.Location = new System.Drawing.Point(12, 410);
            this.chkEnablePreview.Name = "chkEnablePreview";
            this.chkEnablePreview.Size = new System.Drawing.Size(100, 17);
            this.chkEnablePreview.TabIndex = 4;
            this.chkEnablePreview.Text = "Enable Preview";
            this.chkEnablePreview.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(123, 454);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(57, 42);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            // 
            // chkDebuggingMode
            // 
            this.chkDebuggingMode.AutoSize = true;
            this.chkDebuggingMode.Location = new System.Drawing.Point(12, 433);
            this.chkDebuggingMode.Name = "chkDebuggingMode";
            this.chkDebuggingMode.Size = new System.Drawing.Size(108, 17);
            this.chkDebuggingMode.TabIndex = 7;
            this.chkDebuggingMode.Text = "Debugging Mode";
            this.chkDebuggingMode.UseVisualStyleBackColor = true;
            // 
            // chkAutoPlay
            // 
            this.chkAutoPlay.AutoSize = true;
            this.chkAutoPlay.Location = new System.Drawing.Point(12, 456);
            this.chkAutoPlay.Name = "chkAutoPlay";
            this.chkAutoPlay.Size = new System.Drawing.Size(67, 17);
            this.chkAutoPlay.TabIndex = 8;
            this.chkAutoPlay.Text = "Autoplay";
            this.chkAutoPlay.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(229, 415);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(33, 30);
            this.btnUp.TabIndex = 9;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(229, 459);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(33, 30);
            this.btnDown.TabIndex = 10;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(196, 436);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(33, 30);
            this.btnLeft.TabIndex = 11;
            this.btnLeft.Text = "←";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(262, 435);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(33, 30);
            this.btnRight.TabIndex = 12;
            this.btnRight.Text = "→";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnSaveXY
            // 
            this.btnSaveXY.Location = new System.Drawing.Point(147, 503);
            this.btnSaveXY.Name = "btnSaveXY";
            this.btnSaveXY.Size = new System.Drawing.Size(67, 30);
            this.btnSaveXY.TabIndex = 13;
            this.btnSaveXY.Text = "Save XY";
            this.btnSaveXY.UseVisualStyleBackColor = true;
            this.btnSaveXY.Click += new System.EventHandler(this.btnSaveXY_Click);
            // 
            // chkFullLog
            // 
            this.chkFullLog.AutoSize = true;
            this.chkFullLog.Location = new System.Drawing.Point(12, 479);
            this.chkFullLog.Name = "chkFullLog";
            this.chkFullLog.Size = new System.Drawing.Size(63, 17);
            this.chkFullLog.TabIndex = 14;
            this.chkFullLog.Text = "Full Log";
            this.chkFullLog.UseVisualStyleBackColor = true;
            // 
            // chkHidePicture
            // 
            this.chkHidePicture.AutoSize = true;
            this.chkHidePicture.Location = new System.Drawing.Point(118, 409);
            this.chkHidePicture.Name = "chkHidePicture";
            this.chkHidePicture.Size = new System.Drawing.Size(84, 17);
            this.chkHidePicture.TabIndex = 15;
            this.chkHidePicture.Text = "Hide Picture";
            this.chkHidePicture.UseVisualStyleBackColor = true;
            this.chkHidePicture.CheckedChanged += new System.EventHandler(this.chkHidePicture_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 538);
            this.Controls.Add(this.chkHidePicture);
            this.Controls.Add(this.chkFullLog);
            this.Controls.Add(this.btnSaveXY);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.chkAutoPlay);
            this.Controls.Add(this.chkDebuggingMode);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.chkEnablePreview);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmMain";
            this.Text = "gTronic v0.1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Move += new System.EventHandler(this.frmMain_Move);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkEnablePreview;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox chkDebuggingMode;
        private System.Windows.Forms.CheckBox chkAutoPlay;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnSaveXY;
        private System.Windows.Forms.CheckBox chkFullLog;
        private System.Windows.Forms.CheckBox chkHidePicture;
    }
}

