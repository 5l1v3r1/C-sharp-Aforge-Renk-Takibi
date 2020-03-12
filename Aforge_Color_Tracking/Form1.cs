using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//Aforge ile Uzaylar
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision;
using AForge.Vision.Motion;

namespace Aforge_Color_Tracking
{
    public partial class Form1 : Form
    {
        //Siniflarin nesneleri tanimlaniyor
        private VideoCaptureDevice FinalVideoSource;
        private FilterInfoCollection VideoCaptuerDevices;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Sisteme bagli olan Cam listesini aliyoruz
            VideoCaptuerDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo VideoCaptureDevice in VideoCaptuerDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name); // WebCamlarin hepsi ComboBox'da listeleniyor
                comboBox1.SelectedIndex = 0;  //1.User im Combobox wird markiert.
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalVideoSource = new VideoCaptureDevice(VideoCaptuerDevices[comboBox1.SelectedIndex].MonikerString);//1.User ist gewählt
            FinalVideoSource.NewFrame += new NewFrameEventHandler(FinalVideoSource_NewFrame);

            FinalVideoSource.Start();
        }

        void FinalVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;

            try
            {

                //kırmızı
                if (radioButton1.Checked)
                {
                    //ColorFilter'in yapildigi yer
                    ColorFiltering filter = new ColorFiltering();
                    int a1 = int.Parse(textBox1.Text);
                    int a2 = int.Parse(textBox2.Text);
                    int a3 = int.Parse(textBox3.Text);
                    int b1 = int.Parse(textBox4.Text);
                    int b2 = int.Parse(textBox5.Text);
                    int b3 = int.Parse(textBox6.Text);
                    filter.Red = new IntRange(a1, b1);
                    filter.Green = new IntRange(a2, b2);
                    filter.Blue = new IntRange(a3, b3);
                    filter.ApplyInPlace(image1);

                    //Görüntü üzerinde algilanan renk kare icine aliniyor
                    cevresiniciz(image1);

                }

                //mavi
                if (radioButton2.Checked)
                {
                    int a1 = int.Parse(textBox7.Text);
                    int a2 = int.Parse(textBox8.Text);
                    int a3 = int.Parse(textBox9.Text);
                    // Euclidean Color Filterin yapildigi yer
                    EuclideanColorFiltering filter = new EuclideanColorFiltering();
                    // Algilanacak renk belirleniyor ve orta noktasi belirleniyor
                    filter.CenterColor = new RGB(Color.FromArgb(a1, a2, a3));
                    filter.Radius = 50;
                    // filtre calistiriliyor
                    filter.ApplyInPlace(image1);

                    //Görüntü üzerinde algilanan rengi cevrcevelemek veya hedeflemek icin gerekli Methodlar
                    cevresiniciz(image1);
                    //hedefal(image);
                }
            }
            catch { }

        }
        /////////Burdan sonra ekrandaki color etrafina Dikdörtgen cizdiriyoruz/////////////
        public void cevresiniciz(Bitmap image)
        {

            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 2;
            blobCounter.MinHeight = 2;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);

            blobCounter.ProcessImage(grayImage);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            foreach (Rectangle recs in rects)
            {

                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];
                    //Graphics g = Graphics.FromImage(image);
                    Graphics g = pictureBox1.CreateGraphics();
                    using (Pen pen = new Pen(Color.FromArgb(0, 0, 0), 2))
                    {

                        g.DrawRectangle(pen, objectRect);
                    }

                    //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                    int objectX = objectRect.X + (objectRect.Width / 2);
                    int objectY = objectRect.Y + (objectRect.Height / 2);
                    //int objectX = objectRect.X;
                    //int objectY = objectRect.Y;
                    g.DrawString(objectX.ToString() + "X" + objectY.ToString(), new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(250, 1));
                    g.Dispose();

                }
            }
        }
        ///////////////////// Buraya kadar //////////////////////////////////////////

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Webcam beim Schließen des Programms wieder freigeben
            if (FinalVideoSource != null && FinalVideoSource.IsRunning)
            {
                FinalVideoSource.SignalToStop();
                FinalVideoSource = null;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Webcam beim Schließen des Programms wieder freigeben
            if (FinalVideoSource != null && FinalVideoSource.IsRunning)
            {
                FinalVideoSource.SignalToStop();
                FinalVideoSource = null;

            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
