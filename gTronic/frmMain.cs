using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using static gTronic.gTronic;

namespace gTronic
{
    public partial class frmMain : Form
    {
        private Form _frmPreview = new Form();

        private gTronic _gTronic = new gTronic();

        /// <summary>
        /// This is necessary for the TopMost = true to always work
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 8;  // Turn on WS_EX_TOPMOST
                return cp;
            }
        }

        public frmMain()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            btnStop.Enabled = false;

            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            if (_gTronic.gLocationX != 0 && _gTronic.gLocationY != 0)
            {
                this.Location = new Point(_gTronic.gLocationX, _gTronic.gLocationY);
            }

            PictureBox pic1 = new PictureBox();
            Bitmap bmp1 = new Bitmap(BioBitmapWidth, BioBitmapHeight);
            Label lab1 = new Label();
            Label lab2 = new Label();
            _frmPreview.Controls.Add(pic1);
            _frmPreview.Controls.Add(lab1);
            _frmPreview.Controls.Add(lab2);
            _frmPreview.Text = this.Text + " - Preview Window";
            _frmPreview.Width = this.Width;
            _frmPreview.Height = this.Height;
            _frmPreview.StartPosition = FormStartPosition.Manual;
            _frmPreview.TopMost = true;
            pic1.Width = bmp1.Width;
            pic1.Height = bmp1.Height;
            pic1.Image = bmp1;
            pic1.Location = pictureBox1.Location;
            lab1.Width = pic1.Width;
            lab1.Height = 20;
            lab1.Location = new Point(pic1.Location.X, pic1.Location.Y + pic1.Height + 10);
            lab2.Width = pic1.Width;
            lab2.Height = 20;
            lab2.Location = new Point(pic1.Location.X, pic1.Location.Y + pic1.Height + 20 + lab1.Height);
        }

        //Start Button
        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            _gTronic.IsStarted = true;

            //Check Full Log
            if (chkFullLog.Checked)
            {
                _gTronic.gLog = new StreamWriter(String.Format("{0}_gTronicDump.txt", DateTime.Now.ToString("[yyyy-MM-dd][HH-mm-ss]")), false);
            }

            int BorderWidth = (this.Width - this.ClientSize.Width) / 2;
            int TitlebarHeight = this.Height - this.ClientSize.Height - BorderWidth;
            int x = this.Location.X + this.Width;
            int y = this.Location.Y + TitlebarHeight + pictureBox1.Location.Y;

            int count_loops = 0;

            while (_gTronic.IsStarted)
            {
                try
                {
                    Application.DoEvents();
                    _gTronic.BioBmp = CaptureScreen(BioBitmapWidth, BioBitmapHeight, x, y);

                    if (count_loops % 100 == 0)
                    {
                        //Clean Memory
                        GC.Collect();
                        //Check for Hide Picture
                        if (!chkHidePicture.Checked)
                        {
                            pictureBox1.Image = _gTronic.BioBmp;
                        }
                    }

                    count_loops++;

                    if (count_loops == 10000)
                    {
                        count_loops = 0;
                    }

                    _gTronic.CountUnknown = 0;
                    _gTronic.FillBioMatrix(chkFullLog.Checked);


                    List<BioMove> BioMoves = null;
                    if (_gTronic.CountUnknown < UNKNOWN_THRESHOLD)
                    {
                        BioMoves = _gTronic.DetectAvailableMoves(_gTronic.BioMatrix, chkFullLog.Checked);
                    }
                    else
                    {
                        continue;
                    }

                    //Check Preview
                    if (chkEnablePreview.Checked)
                    {
                        _gTronic.PreviewBioMatrix(((PictureBox)_frmPreview.Controls[0]).Image);
                        _gTronic.PreviewMoves(((PictureBox)_frmPreview.Controls[0]).Image, BioMoves);
                        ((Label)_frmPreview.Controls[1]).Text = String.Format("Available Moves : {0} Unknown Colors : {1}", BioMoves.Count, _gTronic.CountUnknown);
                        _frmPreview.Location = new Point(this.Location.X + this.Width + pictureBox1.Width, this.Location.Y);
                        _frmPreview.Refresh();
                        _frmPreview.Show();
                    }

                    //Check Autoplay
                    if (!chkDebuggingMode.Checked && chkAutoPlay.Checked)
                    {
                        if (BioMoves.Count > 0)
                        {
                            BioMove selMove = _gTronic.SelectMove(BioMoves);
                            //Check for Preview Enabled
                            if (chkEnablePreview.Checked)
                            {
                                ((Label)_frmPreview.Controls[2]).Text = String.Format("Current Move Combos : {0}", selMove.TotalCombos);
                            }

                            int start_x = 2 * BorderWidth + this.Location.X + this.Width + BioWidthOffset + selMove.StartBlock.Row * (BioWidth + BioSpace) + BioWidth / 2;
                            int start_y = TitlebarHeight + this.Location.Y + pictureBox1.Location.Y + BioHeightOffset + selMove.StartBlock.Column * (BioHeight + BioSpace) + BioHeight / 2;

                            int end_x = 2 * BorderWidth + this.Location.X + this.Width + BioWidthOffset + selMove.EndBlock.Row * (BioWidth + BioSpace) + BioWidth / 2;
                            int end_y = TitlebarHeight + this.Location.Y + pictureBox1.Location.Y + BioHeightOffset + selMove.EndBlock.Column * (BioHeight + BioSpace) + BioHeight / 2;

                            _gTronic.PlayMouseMove(selMove, BorderWidth, TitlebarHeight, start_x, start_y, end_x, end_y, chkFullLog.Checked);
                            //Random rnd = new Random();
                            //System.Threading.Thread.Sleep(rnd.Next(MIN_DELAY_BETWEEN_MOVES, MAX_DELAY_BETWEEN_MOVES + 1));
                            //System.Threading.Thread.Sleep(MIN_DELAY_BETWEEN_MOVES + MAX_DELAY_BETWEEN_MOVES / 2);
                        }
                    }

                    //Check Debug
                    if (chkDebuggingMode.Checked)
                    {
                        _gTronic.WriteDebugDump(pictureBox1.Image);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error!\r\n" + ex.Message, "btnStart");
                }
            }
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            int BorderWidth = (this.Width - this.ClientSize.Width) / 2;
            int TitlebarHeight = this.Height - this.ClientSize.Height - BorderWidth;
            int x = this.Location.X + this.Width;
            int y = this.Location.Y + TitlebarHeight + pictureBox1.Location.Y;
            pictureBox1.Image = CaptureScreen(BioBitmapWidth, BioBitmapHeight, x, y);
            pictureBox1.Refresh();
        }

        //Exit Button
        private void btnExit_Click(object sender, EventArgs e)
        {
            _gTronic.IsStarted = false;
            Application.Exit();
        }

        //Stop Button
        private void btnStop_Click(object sender, EventArgs e)
        {
            _gTronic.IsStarted = false;
            //Check Full Log
            if (chkFullLog.Checked)
            {
                if (_gTronic.gLog != null)
                {
                    _gTronic.gLog.Close();
                }
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _gTronic.IsStarted = false;
        }

        //Move Up 1 pixel
        private void btnUp_Click(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X, this.Location.Y - 1);
        }

        //Move Down 1 pixel
        private void btnDown_Click(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X, this.Location.Y + 1);
        }

        //Move Left 1 pixel
        private void btnLeft_Click(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X - 1, this.Location.Y);
        }

        //Move Right 1 pixel
        private void btnRight_Click(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X + 1, this.Location.Y);
        }

        //Save to file, Form Position
        private void btnSaveXY_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(gTronicIniFile, false))
            {
                sw.WriteLine(String.Format("x={0}", this.Location.X));
                sw.WriteLine(String.Format("y={0}", this.Location.Y));
            }
        }

        private void chkHidePicture_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Visible = !chkHidePicture.Checked;
        }
    }
}