using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ImageLab {
    class MatrixFilter : Filters {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel) {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for(int l = -radiusY; l <= radiusY; l++) {
                for(int k = -radiusX; k <= radiusX; k++) {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    }

    //фильтр размытия
    class BlurFilter : MatrixFilter {
        public BlurFilter() {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++) {
                for (int j = 0; j < sizeY; j++) {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }

    //фильтр Гаусса
    class GaussianFilter : MatrixFilter {
        public void createGaussianKernel(int radius, float sigma) {
            //определяем размер ядра
            int size = 2 * radius + 1;
            //создаем ядро фильтра
            kernel = new float[size, size];
            //коэффициент нормировки ядра
            float norm = 0;
            //рассчитывем ядро линейного фильтра
            for (int i = -radius; i <= radius; i++) {
                for (int j = -radius; j <= radius; j++) {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            //нормируем ядро
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    kernel[i, j] /= norm;
                }
            }
        }
        public GaussianFilter() {
            createGaussianKernel(3, 2);
        }
    }

    //фильтр Тиснение
    class LetteringFilter : MatrixFilter {
        public void createLetteringKernel() {
            kernel = new float[3, 3] { { 0, 1, 0 }, { 1, 0, -1 }, { 0, -1, 0 } };
        }
        public LetteringFilter() {
            createLetteringKernel();
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float Intensity = 0;
            for (int l = -radiusY; l <= radiusY; l++) {
                for (int k = -radiusX; k <= radiusX; k++) {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    float neighborIntensity = 0.299f * (float)neighborColor.R + 0.587f * (float)neighborColor.G + 0.114f * (float)neighborColor.B;
                    Intensity += neighborIntensity * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp(((int)Intensity + 255) / 2, 0, 255),
                Clamp(((int)Intensity + 255) / 2, 0, 255),
                Clamp(((int)Intensity + 255) / 2, 0, 255)
                );
        }
    }

    //медианный фильтр
    class MedianFilter : MatrixFilter {
        int size;
        public MedianFilter(int s = 3) {
            size = s;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int radius = size / 2;
            int s = size * size;
            Color[] sourceColors = new Color[s];
            int[] R = new int[s];
            int[] G = new int[s];
            int[] B = new int[s];
            for (int i = -radius, k = 0; i <= radius; i++) {
                for (int j = -radius; j <= radius; j++) {
                    int idX = Clamp(x + i, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + j, 0, sourceImage.Height - 1);
                    sourceColors[k++] = sourceImage.GetPixel(idX, idY);
                }
            }
            for (int k = 0; k < s; k++) {
                R[k] = sourceColors[k].R;
                G[k] = sourceColors[k].G;
                B[k] = sourceColors[k].B;
            }
            Array.Sort(R);
            Array.Sort(G);
            Array.Sort(B);
            return Color.FromArgb(R[s / 2], G[s / 2], B[s / 2]);
        }
    }

    //фильтр Motion Blur
    class MotionBlurFilter : MatrixFilter {
        public void createMotionBlurKernel() {
            int size = 5;
            kernel = new float[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    if (i == j) {
                        kernel[i, j] = 1f / (float)size;
                    }
                    else {
                        kernel[i, j] = 0;
                    }
                }
            }
        }
        public MotionBlurFilter() {
            createMotionBlurKernel();
        }
    }

    //фильтр Собеля
    class SobelFilter : MatrixFilter {
        protected float[,] Ox = null;
        protected float[,] Oy = null;
        public SobelFilter() {
            Ox = new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            Oy = new float[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            kernel = Ox;
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float tmpR1 = 0;
            float tmpG1 = 0;
            float tmpB1 = 0;
            for (int l = -radiusY; l <= radiusY; l++) {
                for (int k = -radiusX; k <= radiusX; k++) {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    tmpR1 += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    tmpG1 += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    tmpB1 += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            kernel = Oy;
            float tmpR2 = 0;
            float tmpG2 = 0;
            float tmpB2 = 0;
            for (int l = -radiusY; l <= radiusY; l++) {
                for (int k = -radiusX; k <= radiusX; k++) {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    tmpR2 += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    tmpG2 += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    tmpB2 += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            float resultR = (float)Math.Sqrt(tmpR1 * tmpR1 + tmpR2 * tmpR2);
            float resultG = (float)Math.Sqrt(tmpG1 * tmpG1 + tmpG2 * tmpG2);
            float resultB = (float)Math.Sqrt(tmpB1 * tmpB1 + tmpB2 * tmpB2);
            float Intensity = 0.299f * (float)resultR + 0.587f * (float)resultG + 0.114f * (float)resultB;
            return Color.FromArgb(
                Clamp((int)Intensity, 0, 255),
                Clamp((int)Intensity, 0, 255),
                Clamp((int)Intensity, 0, 255)
                );
        }
    }

    //фильтр повышения резкости
    class UpSharpnessFilter : MatrixFilter {
        public UpSharpnessFilter() {
            kernel = new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    }
}
