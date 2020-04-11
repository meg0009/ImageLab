using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace ImageLab {
    abstract class Filters {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for(int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending) {
                    return null;
                }
                for(int j = 0; j < sourceImage.Height; j++) {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
        public int Clamp(int value, int min, int max) {
            if(value < min) {
                return min;
            }
            if (value > max) {
                return max;
            }
            return value;
        }
    }

    //фильтр эффект "стекла"
    class GlassFilter : Filters {
        Random rnd = new Random();
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int xx = x + (int)((rnd.NextDouble() - 0.5) * 10.0);
            int yy = y + (int)((rnd.NextDouble() - 0.5) * 10.0);
            return sourceImage.GetPixel(Clamp(xx, 0, sourceImage.Width - 1), Clamp(yy, 0, sourceImage.Height - 1));
        }
    }

    //фильтр отенков серого
    class GrayScaleFilter : Filters {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            float Intensity = 0.299f * (float)sourceColor.R + 0.587f * (float)sourceColor.G + 0.114f * (float)sourceColor.B;
            return Color.FromArgb(
                (int)Intensity,
                (int)Intensity,
                (int)Intensity
                );
        }
    }

    //фильтр "Серый мир"
    class GrayWorldFilter : Filters {
        public int R_;
        public int G_;
        public int B_;
        public int Avg;

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            return Color.FromArgb(
                Clamp((int)sourceColor.R * Avg / R_, 0, 255),
                Clamp((int)sourceColor.G * Avg / G_, 0, 255),
                Clamp((int)sourceColor.B * Avg / B_, 0, 255)
                );
        }
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            for (int i = 0; i < sourceImage.Width; i++) {
                for (int j = 0; j < sourceImage.Height; j++) {
                    if (worker.CancellationPending) {
                        return null;
                    }
                    R_ += sourceImage.GetPixel(i, j).R;
                    G_ += sourceImage.GetPixel(i, j).G;
                    B_ += sourceImage.GetPixel(i, j).B;
                }
            }
            R_ /= sourceImage.Width * sourceImage.Height;
            G_ /= sourceImage.Width * sourceImage.Height;
            B_ /= sourceImage.Width * sourceImage.Height;
            Avg = (R_ + G_ + B_) / 3;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }

    //фильтр Инверсия
    class InvertFilter : Filters {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                255 - sourceColor.G,
                255 - sourceColor.B);
            return resultColor;
        }
    }

    //фильтр линейного растяжения гистограммы
    class LinearDilationFilter : Filters {
        //минимальная яркость пикселей на исходном изображении
        public float Ymin = 1;
        //максимальная яркость
        public float Ymax;
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            float tmp = 255 * (sourceColor.GetBrightness() - Ymin) / (Ymax - Ymin);
            return Color.FromArgb(
                (int)tmp,
                (int)tmp,
                (int)tmp
                );
        }
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker) {
            for (int i = 0; i < sourceImage.Width; i++) {
                for (int j = 0; j < sourceImage.Height; j++) {
                    if (worker.CancellationPending) {
                        return null;
                    }
                    if (sourceImage.GetPixel(i, j).GetBrightness() < Ymin) {
                        Ymin = sourceImage.GetPixel(i, j).GetBrightness();
                    }
                    if (sourceImage.GetPixel(i, j).GetBrightness() > Ymax) {
                        Ymax = sourceImage.GetPixel(i, j).GetBrightness();
                    }
                }
            }
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++) {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending) {
                    return null;
                }
                for (int j = 0; j < sourceImage.Height; j++) {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }


    //фильтр Сепия
    class SepiaFilter : Filters {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            float Intensity = 0.299f * (float)sourceColor.R + 0.587f * (float)sourceColor.G + 0.114f * (float)sourceColor.B;
            float k = 35;
            return Color.FromArgb(
                Clamp((int)(Intensity + 2.0f * k), 0, 255),
                Clamp((int)(Intensity + 0.5f * k), 0, 255),
                Clamp((int)(Intensity - k), 0, 255)
                );
        }
    }

    //фильтр повышения яркости
    class UpBrightnessFilter : Filters {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 50;
            return Color.FromArgb(
                Clamp(sourceColor.R + k, 0, 255),
                Clamp(sourceColor.G + k, 0, 255),
                Clamp(sourceColor.B + k, 0, 255)
                );
        }
    }

    //фильтр Волны
    class WavesFilter : Filters {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y) {
            int xx = x + (int)(20.0 * Math.Sin(2.0 * Math.PI * (Double)y / 30.0));
            int yy = y;
            return sourceImage.GetPixel(Clamp(xx, 0, sourceImage.Width - 1), Clamp(yy, 0, sourceImage.Height - 1));
        }
    }
}
