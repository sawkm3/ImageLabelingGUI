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
        private List<KeyValuePair<int, string>> files;
        private int id;

        private readonly Color[] backColors = new Color[] { System.Drawing.SystemColors.Control, Color.LightSkyBlue, Color.LightPink };

        public Form1()
        {
            InitializeComponent();

            // set files info
            files = new List<KeyValuePair<int,string>>();

            // test images
            files.Add(new KeyValuePair<int, string>(1, "../../../../Image/1.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/2.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/3.jpg"));
            files.Add(new KeyValuePair<int, string>(0, "../../../../Image/4.jpg"));
            files.Add(new KeyValuePair<int, string>(2, "../../../../Image/5.jpg"));
            files.Add(new KeyValuePair<int, string>(1, "../../../../Image/6.jpg"));
            files.Add(new KeyValuePair<int, string>(0, "../../../../Image/7.jpg"));

            this.markerThumbnailPanel1.Settings(files.ToArray());
            this.markerThumbnailPanel1.ThumbnailSelected += Form1_ThumbnailSelected;
        }

        public void Form1_ThumbnailSelected(int id)
        {
            this.id = id;

            this.pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap selectedImage = new Bitmap(files[id].Value);

            Size newSize = MarkerThumbnailPanel.ChangeSizeWhileKeepThumbnailAspectRatio(selectedImage.Size, pictureBox1.Size - new Size(50, 50));
            Bitmap resizedImage = new Bitmap(selectedImage, newSize);

            Graphics g = e.Graphics;

            // draw test
            g.Clear(backColors[files[id].Key]);
            g.DrawImage(resizedImage, (this.pictureBox1.Width - resizedImage.Width) / 2, (this.pictureBox1.Height - resizedImage.Height) / 2);

            resizedImage.Dispose();
            selectedImage.Dispose();
        }


    }
}
