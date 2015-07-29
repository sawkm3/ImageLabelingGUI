//------------------------------------
// Project Name : ImageLabelingGUI
// File Name : MarkerThumbnailPanel.cs
// User Name : sawkm3 (GitHubID)
// Created Date : 2015/07/26(Sun)
//
// Memo : C++で画像管理ライブラリを作成し，ここで呼び出して使う予定
//------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ImageLabelingGUI
{
    public partial class MarkerThumbnailPanel : UserControl
    {
        // file path array
        private KeyValuePair<int, string>[] files;

        // image buffering
        Bitmap[] thumbnailBuffer;

        // scroll marker bitmap
        private Bitmap scrollMarker;

        //thumbnail settings
        private double aspectRatio = 0.75;
        private Rectangle thumbnailRect;

        // maximum number of display images
        private int displayNum;
        // visible top of image number
        private int topImageNumber;
        // thumbnail tile height
        private int thumbnailTileWidth;
        private int thumbnailTileHeight;

        // selected infomation
        int selectedNumber;

        // marker line width
        public readonly int markerLineWidth = 10;
        public readonly int offsetThumbnail = 10;

        // scroll width
        private readonly int scrollWidth = 17;

        // thumbnail selected delegate
        public delegate void ThumbnailSelectedFunc(int id);
        public ThumbnailSelectedFunc ThumbnailSelected;

        // marker color
        private Brush[] markerBrushes = new Brush[] {Brushes.LightGray, Brushes.CornflowerBlue, Brushes.IndianRed};

        public MarkerThumbnailPanel()
        {
            InitializeComponent();

            // mouse wheel event
            this.MouseWheel += MarkerThumbnailPanel_MouseWheel;
        }

        public void Settings(KeyValuePair<int, string>[] filePathArray)
        {
            // set file path
            files = filePathArray;

            Reset();
        }

        private void Reset()
        {
            ResetComponentsBounds();

            // reset scroll value
            this.VerticalScroll.Value = 0;

            // reset selected number
            selectedNumber = 0;
        }

        private void ResetComponentsBounds()
        {
            if (files == null) return;

            int thumbnailNum = files.Length;

            // thumbnailPanel width
            pictureBox1.Width = this.Width - scrollWidth;

            // thumbnailRect
            thumbnailRect.X = offsetThumbnail;
            thumbnailRect.Y = offsetThumbnail;
            thumbnailRect.Width = pictureBox1.Width - markerLineWidth * 2 - thumbnailRect.X * 2;
            thumbnailRect.Height = (int)(thumbnailRect.Width * aspectRatio);

            // set thumbnailTile size
            thumbnailTileWidth = thumbnailRect.Width + thumbnailRect.X * 2;
            thumbnailTileHeight = thumbnailRect.Height + thumbnailRect.Y * 2;

            // thumbnailPanel height
            int newHeight = thumbnailTileHeight * thumbnailNum;
            pictureBox1.Height = (newHeight < this.Height) ? this.Height : newHeight;

            RecalculateDisplay();

            // set scroll margin
            this.VerticalScroll.SmallChange = this.VerticalScroll.Maximum / thumbnailNum;
            this.VerticalScroll.LargeChange = this.VerticalScroll.SmallChange * (displayNum - 2);

            // create marker
            CreateScrollMarker();
        }

        private void RecalculateDisplay()
        {
            int thumbnailNum = files.Length;

            int oldTopImageNumber = topImageNumber;

            // change topImageNumber by scroll value ratio
            topImageNumber = (int)(files.Length * this.VerticalScroll.Value / (double)this.VerticalScroll.Maximum);

            int oldDisplayNum = displayNum;

            // num of visible thumbnail files
            displayNum = this.Height / thumbnailTileHeight + 2;
            if (thumbnailNum < topImageNumber + displayNum) displayNum = thumbnailNum - topImageNumber;

            // create thumbnail image
            CreateThumbnailImages();
        }

        private void CreateThumbnailImages()
        {
            Bitmap loadImage;

            // free memory
            if (thumbnailBuffer != null)
            {
                for (int i = 0; i < thumbnailBuffer.Length; i++)
                {
                    thumbnailBuffer[i].Dispose();
                }
            }

            // create buffer resource
            thumbnailBuffer = new Bitmap[displayNum];

            for (int i = 0; i < displayNum; i++)
            {
                // load image
                loadImage = new Bitmap(files[topImageNumber + i].Value);
                thumbnailBuffer[i] = new Bitmap(loadImage, ChangeSizeWhileKeepThumbnailAspectRatio(loadImage.Size, thumbnailRect.Size));

                loadImage.Dispose();
            }
        }

        public static Size ChangeSizeWhileKeepThumbnailAspectRatio(Size oldSize, Size targetSize)
        {
            double widthRatio = (double)oldSize.Width / targetSize.Width;
            double heightRatio = (double)oldSize.Height / targetSize.Height;

            if (widthRatio < heightRatio) return new Size((int)(oldSize.Width / heightRatio), (int)(oldSize.Height / heightRatio));
            else return new Size((int)(oldSize.Width / widthRatio), (int)(oldSize.Height / widthRatio));
        }

        private void CreateScrollMarker()
        {
            // calculate scroll marker image height in consideration of scroll width
            int height = this.Height - scrollWidth * 2;

            // create bitmap when null or need update
            if (scrollMarker != null && scrollMarker.Height == height) return;

            // create bitmap
            scrollMarker = new Bitmap(markerLineWidth, height);

            Graphics g = Graphics.FromImage(scrollMarker);

            // calculate marker size
            int thumbnailNum = files.Length;
            decimal stepHeight = (decimal)height / thumbnailNum;
            int beforeHeight, afterHeight;

            // draw marker
            for (int i = 0; i < thumbnailNum; i++)
            {
                beforeHeight = (int)(stepHeight * i);
                afterHeight = (int)(stepHeight * (i + 1));
                g.FillRectangle(markerBrushes[files[i].Key], 0, beforeHeight, markerLineWidth, afterHeight);
            }

            g.Dispose();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Bitmap drawBitmap;
            int height;
            int halfOffset = offsetThumbnail / 2;
            int x, y;

            // clear background
            g.Clear(Color.White);

            // draw looking thumbnails
            for (int i = topImageNumber; i < topImageNumber + displayNum; i++)
            {
                height = thumbnailTileHeight * i;

                // fill background color
                if (i == selectedNumber) g.FillRectangle(Brushes.MidnightBlue, 0, height + 1, markerLineWidth + thumbnailTileWidth, thumbnailTileHeight - 1);
                else g.FillRectangle(Brushes.White, 0, height, markerLineWidth + thumbnailTileWidth, thumbnailTileHeight);

                // draw thumbnail marker
                g.FillRectangle(markerBrushes[files[i].Key], 0, thumbnailRect.Y + height, markerLineWidth, thumbnailRect.Height);

                // draw image
                drawBitmap = thumbnailBuffer[i - topImageNumber];
                x = markerLineWidth + thumbnailRect.X + (thumbnailRect.Width - drawBitmap.Size.Width) / 2;
                y = thumbnailRect.Y + height + (thumbnailRect.Height - drawBitmap.Size.Height) / 2;
                g.DrawImage(drawBitmap, x, y);
                //g.FillRectangle(Brushes.Green, markerLineWidth + thumbnailRect.X, thumbnailRect.Y + height, thumbnailRect.Width, thumbnailRect.Height);

                // draw line
                if (i != topImageNumber)
                    g.DrawLine(Pens.LightGray, markerLineWidth + halfOffset, height, thumbnailTileWidth + halfOffset, height);
            }

            // draw scroll marker
            g.DrawImage(scrollMarker, thumbnailTileWidth + markerLineWidth, scrollWidth + this.VerticalScroll.Value);
        }

        private void MarkerThumbnailPanel_SizeChanged(object sender, EventArgs e)
        {
            // if this size changed, refresh this and childs bounds
            ResetComponentsBounds();

            pictureBox1.Refresh();
        }

        private void MarkerThumbnailPanel_Scroll(object sender, ScrollEventArgs e)
        {
            // moved bertival scroll
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (this.VerticalScroll.Maximum < 0) return;

                RecalculateDisplay();

                pictureBox1.Refresh();
            }
        }

        private void MarkerThumbnailPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            // when the scroll bar move by mouse wheel, why the scroll event is not called ?
            if (e.Delta != 0)
            {
                RecalculateDisplay();

                // change selectedNumber
                if (0 < e.Delta) selectedNumber--;
                else selectedNumber++;

                ThreasholdSelectedNumber();

                ThumbnailSelected(selectedNumber);

               pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = e.Location;

            if (p.X < markerLineWidth + thumbnailTileWidth)
            {
                selectedNumber = p.Y / thumbnailTileHeight;

                ThreasholdSelectedNumber();

                ThumbnailSelected(selectedNumber);

                pictureBox1.Refresh();
            }
        }

        private void ThreasholdSelectedNumber()
        {
            Threashold(ref selectedNumber, 0, files.Length);
        }

        private static void Threashold(ref int number, int min, int max)
        {
            if (number < min) number = min;
            if (max <= number) number = max - 1;
        }
    }
}
