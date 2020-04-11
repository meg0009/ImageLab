using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace ImageLab {
    enum Morf {
        Dilation,
        Erosion
    }

    class MathMorfology : Filters {
        protected static float[,] kernel = null;
        protected Morf type;
        public static void createMathMorfKernel(float[,] k = null) {
            if (k == null) {
                kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            }
            else if(k.GetLength(0)%2 ==0 || k.GetLength(1)%2 == 0) {
                MessageBox.Show("Не правильный структурный элемент", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                kernel = k;
            }
        }
        public static float[,] Kernel {
            get {
                return kernel;
            }
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int max = 0;
            int min = 256;
            Color resultColor = Color.Black;
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            if (type == Morf.Dilation) {
                for (int l = -radiusY; l <= radiusY; l++) {
                    for (int k = -radiusX; k <= radiusX; k++) {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                        Color c = sourceImage.GetPixel(idX, idY);
                        int Intensity = (int)(0.299f * (float)c.R + 0.587f * (float)c.G + 0.114f * (float)c.B);
                        if (kernel[k + radiusX, l + radiusY] == 1 && Intensity > max) {
                            max = Intensity;
                            resultColor = c;
                        }
                    }
                }
            }
            else {
                for (int l = -radiusY; l <= radiusY; l++) {
                    for (int k = -radiusX; k <= radiusX; k++) {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                        Color c = sourceImage.GetPixel(idX, idY);
                        int Intensity = (int)(0.299f * (float)c.R + 0.587f * (float)c.G + 0.114f * (float)c.B);
                        if (kernel[k + radiusX, l + radiusY] == 1 && Intensity < min) {
                            min = Intensity;
                            resultColor = c;
                        }
                    }
                }
            }
            return resultColor;
        }
    }

    //операция расширения
    class Dilation : MathMorfology {
        public Dilation() {
            type = Morf.Dilation;
        }
    }


    //операция сужения
    class Erosion : MathMorfology {
        public Erosion() {
            type = Morf.Erosion;
        }
    }

    //операция раскрытия
    class Opening : MathMorfology {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            Bitmap tmp = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            type = Morf.Erosion;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    tmp.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            type = Morf.Dilation;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50 + 50));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(tmp, i, j));
                }
            }
            return resultImage;
        }
    }

    //операция закрытия
    class Closing : MathMorfology {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            Bitmap tmp = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            type = Morf.Dilation;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    tmp.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            type = Morf.Erosion;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 50 + 50));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(tmp, i, j));
                }
            }
            return resultImage;
        }
    }

    //операция морфологического градиента
    class Grad : MathMorfology {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            Bitmap tmp1 = new Bitmap(sourceImage.Width, sourceImage.Height);
            Bitmap tmp2 = new Bitmap(sourceImage.Width, sourceImage.Height);
            type = Morf.Dilation;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / tmp1.Width * 40));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    tmp1.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            type = Morf.Erosion;
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / tmp2.Width * 40 + 40));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    tmp2.SetPixel(i, j, calculateNewPixelColor(tmp1, i, j));
                }
            }
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 20 + 80));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    Color c1 = tmp1.GetPixel(i, j);
                    Color c2 = tmp2.GetPixel(i, j);
                    resultImage.SetPixel(i, j, Color.FromArgb(
                        Clamp(c1.R - c2.R, 0, 255),
                        Clamp(c1.G - c2.G, 0, 255),
                        Clamp(c1.B - c2.B, 0, 255)
                        ));
                }
            }
            return resultImage;
        }
    }
}
