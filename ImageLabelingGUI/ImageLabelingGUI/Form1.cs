//------------------------------------
// Project Name : ImageLabelingGUI
// File Name : Form1.cs
// User Name : sawkm3 (GitHubID)
// Created Date : 2015/07/29(Wed)
//
// Memo : C++で画像管理ライブラリを作成し，ここで呼び出して使う予定
//------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageLabelingGUI
{
    public partial class Form1 : Form
    {
        private readonly Color[] backColors = new Color[] { System.Drawing.SystemColors.Control, Color.LightSkyBlue, Color.LightPink, Color.Gray };

        public Form1()
        {
            InitializeComponent();

            // set minimum
            this.MinimumSize = this.Size - this.ClientSize + new Size(400, 400);

            // create files info
            List<KeyValuePair<int, string>> files = new List<KeyValuePair<int, string>>();

            // test images
            files.Add(new KeyValuePair<int, string>(1, "../../../../Image/1.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/2.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/3.jpg"));
            files.Add(new KeyValuePair<int, string>(0, "../../../../Image/4.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/5.jpg"));
            files.Add(new KeyValuePair<int, string>(1, "../../../../Image/6.jpg"));
            files.Add(new KeyValuePair<int, string>(0, "../../../../Image/7.jpg"));
            files.Add(new KeyValuePair<int, string>(0, "../../../../Image/8.png"));
            //files.Add(new KeyValuePair<int, string>(1, "../../../../Image/1.png"));
            //files.Add(new KeyValuePair<int, string>(2, "../../../../Image/2.png"));
            //files.Add(new KeyValuePair<int, string>(2, "../../../../Image/3.png"));
            //files.Add(new KeyValuePair<int, string>(0, "../../../../Image/4.png"));
            //files.Add(new KeyValuePair<int, string>(2, "../../../../Image/5.png"));
            //files.Add(new KeyValuePair<int, string>(1, "../../../../Image/6.png"));

            this.markerThumbnailPanel1.Settings(files.ToArray());
            this.markerThumbnailPanel1.ThumbnailSelected += Form1_ThumbnailSelected;

            // change status bar text
            label2.Text = files.Count + "枚";


            RefreshComponentsPlace();
        }

        public void Form1_ThumbnailSelected()
        {
            this.pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap selectedImage;
            try
            {
                selectedImage = new Bitmap(markerThumbnailPanel1.Value);
            }
            catch (Exception)
            {
                selectedImage = new Bitmap(markerThumbnailPanel1.ErrorImagePath);
            }
            

            Size newSize = MarkerThumbnailPanel.ChangeSizeWhileKeepThumbnailAspectRatio(selectedImage.Size, pictureBox1.Size - new Size(50, 50));
            Bitmap resizedImage = new Bitmap(selectedImage, newSize);

            Graphics g = e.Graphics;

            // draw test
            g.Clear(backColors[markerThumbnailPanel1.Key]);
            g.DrawImage(resizedImage, (this.pictureBox1.Width - resizedImage.Width) / 2, (this.pictureBox1.Height - resizedImage.Height) / 2);

            resizedImage.Dispose();
            selectedImage.Dispose();
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            RefreshComponentsPlace();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            RefreshComponentsPlace();
        }

        private void RefreshComponentsPlace()
        {
            button1.Width = panel1.Width / 2;
            markerThumbnailPanel1.Refresh();
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            markerThumbnailPanel1.SetCategory(1);
            markerThumbnailPanel1.Next();
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            markerThumbnailPanel1.SetCategory(2);
            markerThumbnailPanel1.Next();
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // no entry
            markerThumbnailPanel1.SetCategory(3);
            markerThumbnailPanel1.Next();
            pictureBox1.Refresh();
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void カテゴリ別ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            カテゴリ別ToolStripMenuItem.Checked = !カテゴリ別ToolStripMenuItem.Checked;

            // processing

        }

        private void 保存順ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            保存順ToolStripMenuItem.Checked = true;
            画像ID順ToolStripMenuItem.Checked = false;
            ユーザID順ToolStripMenuItem.Checked = false;
            label3.Text = "保存順";

            // processing

        }

        private void 画像ID順ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            保存順ToolStripMenuItem.Checked = false;
            画像ID順ToolStripMenuItem.Checked = true;
            ユーザID順ToolStripMenuItem.Checked = false;
            label3.Text = "画像ID順";

            // processing

        }

        private void ユーザID順ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            保存順ToolStripMenuItem.Checked = false;
            画像ID順ToolStripMenuItem.Checked = false;
            ユーザID順ToolStripMenuItem.Checked = true;
            label3.Text = "ユーザID順";

            // processing

        }
    }
}
