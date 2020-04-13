using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLab {
    public partial class Form1 : Form {

        Bitmap image;

        public Form1() {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*";
            if(dialog.ShowDialog() == DialogResult.OK) {}
            image = new Bitmap(dialog.FileName);
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e) {
            InvertFilter filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if(backgroundWorker1.CancellationPending != true) {
                image = newImage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (!e.Cancelled) {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e) {
            backgroundWorker1.CancelAsync();
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e) {
            BlurFilter filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e) {
            GaussianFilter filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void оттенкиСерогоToolStripMenuItem_Click(object sender, EventArgs e) {
            GrayScaleFilter filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e) {
            SepiaFilter filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличитьЯркостьToolStripMenuItem_Click(object sender, EventArgs e) {
            UpBrightnessFilter filter = new UpBrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void повыситьРезкостьToolStripMenuItem_Click(object sender, EventArgs e) {
            UpSharpnessFilter filter = new UpSharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e) {
            GrayWorldFilter filter = new GrayWorldFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void линейноеРастяжениеToolStripMenuItem_Click(object sender, EventArgs e) {
            LinearDilationFilter filter = new LinearDilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e) {
            GlassFilter filter = new GlassFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волныToolStripMenuItem_Click(object sender, EventArgs e) {
            WavesFilter filter = new WavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e) {
            Dilation filter = new Dilation();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e) {
            Erosion filter = new Erosion();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e) {
            Opening filter = new Opening();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e) {
            Closing filter = new Closing();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gradToolStripMenuItem_Click(object sender, EventArgs e) {
            Grad filter = new Grad();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйФильтрToolStripMenuItem_Click(object sender, EventArgs e) {
            MedianFilter filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e) {
            LetteringFilter filter = new LetteringFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e) {
            MotionBlurFilter filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void Form1_Load(object sender, EventArgs e) {
            MathMorfology.createMathMorfKernel();
        }

        private void задатьСруктурныйЭлементToolStripMenuItem_Click(object sender, EventArgs e) {
            Form fm2 = new Form2();
            fm2.ShowDialog();
        }

        private void сохронитьToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image files(*.png)|*.png|Image files(*.jpg)|*.jpg|Image files(*.bmp)|*.bmp|All files(*.*)|*.*";
            if(dialog.ShowDialog() == DialogResult.OK) { }
            if(dialog.FileName != "") {
                System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
                image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e) {
            SobelFilter filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
