using System.Runtime.Intrinsics;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace GK_Proj_2
{
    public class Triangle
    {
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public Vertex v3 { get; set; }
        private static Random random = new Random();

        public double MeanZ => (v1.pointAfter.Z + v2.pointAfter.Z + v3.pointAfter.Z) / 3.0;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public void Fill(Canvas canvas, int zoom)
        {
            int width = (int)canvas.ActualWidth;
            int height = (int)canvas.ActualHeight;
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            int shift = (int)Math.Min(Math.Min(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            int minY = shift; // Dla czytelności algorytmu
            int maxY = (int)Math.Max(Math.Max(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            int size = maxY - shift;
            List<(int ymin, int ymax, int xmin, double m)>[] ET = new List<(int ymin, int ymax, int xmin, double m)>[size + 1];
            List<(int ymin, int ymax, int xmin, double m)> AET = new List<(int ymin, int ymax, int xmin, double m)>();
            var e1 = v1.GetEdgeAtrib(v2);
            var e2 = v1.GetEdgeAtrib(v3);
            var e3 = v2.GetEdgeAtrib(v3);

            ET[e1.ymin - shift] = [];
            ET[e2.ymin - shift] = [];
            ET[e3.ymin - shift] = [];
            ET[e1.ymin - shift].Add(e1);
            ET[e2.ymin - shift].Add(e2);
            ET[e3.ymin - shift].Add(e3);
            byte r = (byte)random.Next(0, 255);
            byte g = (byte)random.Next(0, 255);
            byte b = (byte)random.Next(0, 255);

            Color color = Color.FromRgb(r,g,b);
            for (int y = minY * zoom; y <= maxY * zoom; y++)
            {
                if (y % zoom == 0 && ET[y / zoom - shift] != null)
                {
                    AET.AddRange(ET[y / zoom - shift]);
                    int z = AET.Count;
                    AET.RemoveAll(e => e.ymax <= y / zoom);
                    int za = AET.Count;
                    if (z != za)
                        Console.WriteLine();
                    AET.Sort((e1, e2) => e1.xmin.CompareTo(e2.xmin));
                }

                for (int i = 0; i < AET.Count; i += 2)
                {
                    if (i + 1 >= AET.Count)
                        break;

                    var ed1 = AET[i];
                    var ed2 = AET[i + 1];

                    double startX = ed1.xmin * zoom + (y - ed1.ymin * zoom) * ed1.m;
                    double endX = ed2.xmin * zoom + (y - ed2.ymin * zoom) * ed2.m;
                    if (startX > endX)
                    {
                        double help = endX;
                        endX = startX;
                        startX = help;
                    }

                    for (int x = (int)Math.Ceiling(startX); x <= (int)endX; x++)
                    {
                        SetPixel(bitmap, x, y, color);
                    }
                }
                Image image = new Image { Source = bitmap };
                Canvas.SetLeft(image, -canvas.ActualWidth/2);
                Canvas.SetTop(image, canvas.ActualHeight/2);
                image.RenderTransform = new ScaleTransform(1, -1);
                canvas.Children.Add(image);
            }
        }

        public void Fill1(WriteableBitmap bitmap, int zoom)
        {
            int shift = (int)Math.Min(Math.Min(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            int minY = shift; // Dla czytelności algorytmu
            int maxY = (int)Math.Max(Math.Max(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            int size = maxY - shift;
            List<(int ymin, int ymax, int xmin, double m)>[] ET = new List<(int ymin, int ymax, int xmin, double m)>[size + 1];
            List<(int ymin, int ymax, int xmin, double m)> AET = new List<(int ymin, int ymax, int xmin, double m)>();
            var e1 = v1.GetEdgeAtrib(v2);
            var e2 = v1.GetEdgeAtrib(v3);
            var e3 = v2.GetEdgeAtrib(v3);

            ET[e1.ymin - shift] = [];
            ET[e2.ymin - shift] = [];
            ET[e3.ymin - shift] = [];
            ET[e1.ymin - shift].Add(e1);
            ET[e2.ymin - shift].Add(e2);
            ET[e3.ymin - shift].Add(e3);

            for (int y = minY * zoom; y <= maxY * zoom; y++)
            {
                if (y % zoom == 0 && ET[y / zoom - shift] != null)
                {
                    AET.AddRange(ET[y / zoom - shift]);
                    int z = AET.Count;
                    AET.RemoveAll(e => e.ymax <= y / zoom);
                    int za = AET.Count;
                    if (z != za)
                        Console.WriteLine();
                    AET.Sort((e1, e2) => e1.xmin.CompareTo(e2.xmin));
                }

                for (int i = 0; i < AET.Count; i += 2)
                {
                    if (i + 1 >= AET.Count)
                        break;

                    var ed1 = AET[i];
                    var ed2 = AET[i + 1];

                    double startX = ed1.xmin * zoom + (y - ed1.ymin * zoom) * ed1.m;
                    double endX = ed2.xmin * zoom + (y - ed2.ymin * zoom) * ed2.m;
                    if (startX > endX)
                    {
                        double help = endX;
                        endX = startX;
                        startX = help;
                    }

                    for (int x = (int)Math.Ceiling(startX); x <= (int)endX; x++)
                    {
                        SetPixel(bitmap, x, y, Color.FromRgb(255,105,180));
                    }
                }
            }
        }

        public void SetPixel(WriteableBitmap bitmap, int xbefore, int ybefore, Color color)
        {
            int scaleX = 1;
            int scaleY = -1;
            double translateX = bitmap.PixelWidth / 2;
            double translateY = bitmap.PixelHeight / 2;

            int x = xbefore * scaleX + (int)translateX;
            int y = ybefore * scaleY + (int)translateY;


            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return;

            bitmap.Lock();

            unsafe
            {
                IntPtr pBackBuffer = bitmap.BackBuffer;
                int stride = bitmap.BackBufferStride;
                int pixelOffset = y * stride + x * 4;

                byte* pPixel = (byte*)pBackBuffer + pixelOffset;
                pPixel[0] = color.B;
                pPixel[1] = color.G;
                pPixel[2] = color.R;
                pPixel[3] = color.A;
            }

            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            bitmap.Unlock();
        }
    }
}
