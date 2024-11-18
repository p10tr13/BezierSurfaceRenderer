using System.Runtime.Intrinsics;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Drawing;
using FastBitmapLib;

namespace GK_Proj_2
{
    public class Triangle
    {
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public Vertex v3 { get; set; }

        public double MeanZ => (v1.pointAfter.Z + v2.pointAfter.Z + v3.pointAfter.Z) / 3.0;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        // Ta fukcja pozwala na rysowanie na Canvie bitmap każdego trójkąta oddzielnie i można przez to uzyskać efekt odpowiedniej kolejności z
        // punktami kontrolnymi i krawędziami, ale jest wolniejsze i trzeba trochę przerobić więc nie używam jej
        //public void FillOnCanva(Canvas canvas, int zoom)
        //{
        //    int width = (int)canvas.ActualWidth;
        //    int height = (int)canvas.ActualHeight;
        //    WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        //
        //    int shift = (int)Math.Min(Math.Min(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
        //    int minY = shift; // Dla czytelności algorytmu
        //    int maxY = (int)Math.Max(Math.Max(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
        //    int size = maxY - shift;
        //    List<(int ymin, int ymax, int xmin, double m)>[] ET = new List<(int ymin, int ymax, int xmin, double m)>[size + 1];
        //    List<(int ymin, int ymax, int xmin, double m)> AET = new List<(int ymin, int ymax, int xmin, double m)>();
        //    var e1 = v1.GetEdgeAtrib(v2);
        //    var e2 = v1.GetEdgeAtrib(v3);
        //    var e3 = v2.GetEdgeAtrib(v3);
        //
        //    ET[e1.ymin - shift] = [];
        //    ET[e2.ymin - shift] = [];
        //    ET[e3.ymin - shift] = [];
        //    ET[e1.ymin - shift].Add(e1);
        //    ET[e2.ymin - shift].Add(e2);
        //    ET[e3.ymin - shift].Add(e3);
        //
        //    for (int y = minY * zoom; y <= maxY * zoom; y++)
        //    {
        //        if (y % zoom == 0 && ET[y / zoom - shift] != null)
        //      {
        //          AET.AddRange(ET[y / zoom - shift]);
        //          int z = AET.Count;
        //          AET.RemoveAll(e => e.ymax <= y / zoom);
        //          int za = AET.Count;
        //          if (z != za)
        //              Console.WriteLine();
        //          AET.Sort((e1, e2) => e1.xmin.CompareTo(e2.xmin));
        //      }
        //
        //        for (int i = 0; i < AET.Count; i += 2)
        //        {
        //            if (i + 1 >= AET.Count)
        //                break;
        //
        //            var ed1 = AET[i];
        //            var ed2 = AET[i + 1];
        //
        //            double startX = ed1.xmin * zoom + (y - ed1.ymin * zoom) * ed1.m;
        //            double endX = ed2.xmin * zoom + (y - ed2.ymin * zoom) * ed2.m;
        //            if (startX > endX)
        //          {
        //              double help = endX;
        //              endX = startX;
        //              startX = help;
        //          }
        //
        //            for (int x = (int)Math.Ceiling(startX); x <= (int)endX; x++)
        //            {
        //                SetPixel(bitmap, x, y, Color.FromRgb(255, 105, 180));
        //            }
        //        }
        //        Image image = new Image { Source = bitmap };
        //        Canvas.SetLeft(image, -canvas.ActualWidth / 2);
        //        Canvas.SetTop(image, canvas.ActualHeight / 2);
        //        image.RenderTransform = new ScaleTransform(1, -1);
        //        canvas.Children.Add(image);
        //    }
        //}
        //
        //public void FillWithZoom(WriteableBitmap bitmap, int zoom)
        //{
        //    int shift = (int)Math.Min(Math.Min(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
        //    int minY = shift; // Dla czytelności algorytmu
        //    int maxY = (int)Math.Max(Math.Max(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
        //    int size = maxY - shift;
        //    List<(int ymin, int ymax, int xmin, double m)>[] ET = new List<(int ymin, int ymax, int xmin, double m)>[size + 1];
        //    List<(int ymin, int ymax, int xmin, double m)> AET = new List<(int ymin, int ymax, int xmin, double m)>();
        //    var e1 = v1.GetEdgeAtrib(v2);
        //    var e2 = v1.GetEdgeAtrib(v3);
        //    var e3 = v2.GetEdgeAtrib(v3);
        //
        //    ET[e1.ymin - shift] = [];
        //    ET[e2.ymin - shift] = [];
        //    ET[e3.ymin - shift] = [];
        //    ET[e1.ymin - shift].Add(e1);
        //    ET[e2.ymin - shift].Add(e2);
        //    ET[e3.ymin - shift].Add(e3);
        //
        //    for (int y = minY * zoom; y <= maxY * zoom; y++)
        //    {
        //        if (y % zoom == 0 && ET[y / zoom - shift] != null)
        //        {
        //            AET.AddRange(ET[y / zoom - shift]);
        //            int z = AET.Count;
        //            AET.RemoveAll(e => e.ymax <= y / zoom);
        //            int za = AET.Count;
        //            if (z != za)
        //                Console.WriteLine();
        //            AET.Sort((e1, e2) => e1.xmin.CompareTo(e2.xmin));
        //        }
        //
        //        for (int i = 0; i < AET.Count; i += 2)
        //        {
        //            if (i + 1 >= AET.Count)
        //                break;
        //
        //            var ed1 = AET[i];
        //            var ed2 = AET[i + 1];
        //
        //            double startX = ed1.xmin * zoom + (y - ed1.ymin * zoom) * ed1.m;
        //            double endX = ed2.xmin * zoom + (y - ed2.ymin * zoom) * ed2.m;
        //            if (startX > endX)
        //            {
        //                double help = endX;
        //                endX = startX;
        //                startX = help;
        //            }
        //
        //            for (int x = (int)Math.Ceiling(startX); x <= (int)endX; x++)
        //            {
        //                SetPixel(bitmap, x, y, CalculateColorWithZoom(x, y, zoom));
        //            }
        //        }
        //    }
        //}

        public System.Windows.Media.Color CalculateColorWithZoom(int x, int y, int zoom)
        {
            double r = (double)Var.TriangleRColor;
            double g = (double)Var.TriangleGColor;
            double b = (double)Var.TriangleBColor;

            double rl = (double)Var.RLightColor;
            double gl = (double)Var.GLightColor;
            double bl = (double)Var.BLightColor;

            Vector3D V = new Vector3D(0, 0, 1);

            double x1 = v1.pointAfter.X, y1 = v1.pointAfter.Y, z1 = v1.pointAfter.Z;
            double x2 = v2.pointAfter.X, y2 = v2.pointAfter.Y, z2 = v2.pointAfter.Z;
            double x3 = v3.pointAfter.X, y3 = v3.pointAfter.Y, z3 = v3.pointAfter.Z;
            Vector3D n1 = v1.NAfter, n2 = v2.NAfter, n3 = v3.NAfter;
            n1.Normalize();
            n2.Normalize();
            n3.Normalize();

            // Wartości barycentryczne do dalszych obliczeń
            double denom = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            double alpha = ((y2 - y3) * (x / zoom - x3) + (x3 - x2) * (y / zoom - y3)) / denom;
            double beta = ((y3 - y1) * (x / zoom - x3) + (x1 - x3) * (y / zoom - y3)) / denom;
            double gamma = 1.0 - alpha - beta;

            double z = alpha * z1 + beta * z2 + gamma * z3;
            Point3D pixPos = new Point3D(x / zoom, y / zoom, z);

            // Wektor do źródła światła
            Vector3D lightV = Var.SunPosition - pixPos;
            lightV.Normalize();

            // Interpolowanie wektora normalnego
            double nx = alpha * n1.X + beta * n2.X + gamma * n3.X;
            double ny = alpha * n1.Y + beta * n2.Y + gamma * n3.Y;
            double nz = alpha * n1.Z + beta * n2.Z + gamma * n3.Z;

            Vector3D intN = new Vector3D(nx, ny, nz);
            intN.Normalize();

            if (Var.OwnTexture && Var.normalMap != null)
            {
                // Nowe u i v
                double u = alpha * v1.U + beta * v2.U + gamma * v3.U;
                double v = alpha * v1.V + beta * v2.V + gamma * v3.V;
                // Nowe Pu'
                double pux = alpha * v1.PuVecAfter.X + beta * v2.PuVecAfter.X + gamma * v3.PuVecAfter.X;
                double puy = alpha * v1.PuVecAfter.Y + beta * v2.PuVecAfter.Y + gamma * v3.PuVecAfter.Y;
                double puz = alpha * v1.PuVecAfter.Z + beta * v2.PuVecAfter.Z + gamma * v3.PuVecAfter.Z;
                // Nowe Pv'
                double pvx = alpha * v1.PvVecAfter.X + beta * v2.PvVecAfter.X + gamma * v3.PvVecAfter.X;
                double pvy = alpha * v1.PvVecAfter.Y + beta * v2.PvVecAfter.Y + gamma * v3.PvVecAfter.Y;
                double pvz = alpha * v1.PvVecAfter.Z + beta * v2.PvVecAfter.Z + gamma * v3.PvVecAfter.Z;

                int xp = (int)(u * (Var.normalMap.PixelWidth - 1));
                int yp = (int)(v * (Var.normalMap.PixelHeight - 1));

                if (xp == Var.normalMap.PixelWidth)
                    xp -= 1;
                if (yp == Var.normalMap.PixelHeight)
                    yp -= 1;

                byte[] pixels = new byte[4];
                Var.normalMap.CopyPixels(new Int32Rect(xp, yp, 1, 1), pixels, 4, 0);
                double npx = (pixels[2] / 255.0) * 2 - 1;
                double npy = (pixels[1] / 255.0) * 2 - 1;
                double npz = (pixels[0] / 255.0) * 2 - 1;

                Vector3D pixelV = new Vector3D(npx, npy, npz);
                pixelV.Normalize();

                Matrix3D Matrix = new Matrix3D(pux, pvx, intN.X, 0,
                                               puy, pvy, intN.Y, 0,
                                               puz, pvz, intN.Z, 0,
                                               0, 0, 0, 1);
                intN = Matrix.Transform(pixelV);
                intN.Normalize();
            }

            Vector3D reflectionV = 2 * Vector3D.DotProduct(intN, lightV) * intN - lightV;
            reflectionV.Normalize();

            // składowa zwierciadlana bez uwzględniania kolorów
            double zwierciadlana = Var.ks * Math.Pow(Math.Max(Vector3D.DotProduct(reflectionV, V), 0), Var.m);

            // składowa rozproszona rmodelu oświetlenie bez kolorów
            double cos = Vector3D.DotProduct(intN, lightV);
            double rmodel = Var.kd * Math.Max(Vector3D.DotProduct(intN, lightV), 0);

            double kr = r * rl * (zwierciadlana + rmodel);
            double kg = g * gl * (zwierciadlana + rmodel);
            double kb = b * bl * (zwierciadlana + rmodel);

            kr = Math.Min(kr, 255);
            kg = Math.Min(kg, 255);
            kb = Math.Min(kb, 255);

            return System.Windows.Media.Color.FromRgb((byte)kr, (byte)kg, (byte)kb);
        }

        // Funkcja wypełniania tego trójkąta z sortowaniem kubełkowym
        public void Fill(Bitmap bitmap)
        {
            int minY = (int)Math.Min(Math.Min(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            int maxY = (int)Math.Max(Math.Max(v1.pointAfter.Y, v2.pointAfter.Y), v3.pointAfter.Y);
            Dictionary<int, List<(double ymin, double ymax, double xmin, double m)>> ET = new Dictionary<int, List<(double ymin, double ymax, double xmin, double m)>>();
            List<(double ymin, double ymax, double xmin, double m)> AET = new List<(double ymin, double ymax, double xmin, double m)>();
            var e1 = v1.GetEdgeAtrib(v2);
            var e2 = v1.GetEdgeAtrib(v3);
            var e3 = v2.GetEdgeAtrib(v3);

            ET[(int)e1.ymin] = [];
            ET[(int)e2.ymin] = [];
            ET[(int)e3.ymin] = [];
            ET[(int)e1.ymin].Add(e1);
            ET[(int)e2.ymin].Add(e2);
            ET[(int)e3.ymin].Add(e3);

            for (int y = minY; y <= maxY; y++)
            {
                if (ET.ContainsKey(y))
                {
                    if (ET.TryGetValue(y, out var list))
                    {
                        AET.AddRange(list);
                        AET.RemoveAll(e => (int)e.ymax <= y);
                        AET.Sort((e1, e2) =>
                        {
                            int result = e1.xmin.CompareTo(e2.xmin);
                            if (result == 0)
                            {
                                return e1.m.CompareTo(e2.m);
                            }
                            return result;
                        });
                    }
                }

                for (int i = 0; i < AET.Count; i += 2)
                {
                    if (i + 1 >= AET.Count)
                        break;

                    var ed1 = AET[i];
                    var ed2 = AET[i + 1];

                    double startX = ed1.xmin;
                    double endX = ed2.xmin;
                    if ((int)Math.Ceiling(startX) > (int)endX)
                    {
                        double help = endX;
                        endX = startX;
                        startX = help;
                    }

                    for (int x = (int)Math.Ceiling(startX); x <= (int)endX; x++)
                    {
                        SetPixel(bitmap, x, y, CalculateColor(x, y));
                    }
                }

                for (int i = 0; i < AET.Count; i++)
                {
                    var ed = AET[i];
                    ed.xmin += ed.m;
                    AET[i] = ed;
                }
            }
        }

        // Funkcja oblicza odpowiedni kolor dla danego położenia piksela (x,y)
        public System.Drawing.Color CalculateColor(int x, int y)
        {
            double r = (double)Var.TriangleRColor;
            double g = (double)Var.TriangleGColor;
            double b = (double)Var.TriangleBColor;

            double rl = (double)Var.RLightColor;
            double gl = (double)Var.GLightColor;
            double bl = (double)Var.BLightColor;

            Vector3D V = new Vector3D(0, 0, 1);

            double x1 = v1.pointAfter.X, y1 = v1.pointAfter.Y, z1 = v1.pointAfter.Z;
            double x2 = v2.pointAfter.X, y2 = v2.pointAfter.Y, z2 = v2.pointAfter.Z;
            double x3 = v3.pointAfter.X, y3 = v3.pointAfter.Y, z3 = v3.pointAfter.Z;
            Vector3D n1 = v1.NAfter, n2 = v2.NAfter, n3 = v3.NAfter;
            n1.Normalize();
            n2.Normalize();
            n3.Normalize();

            // Wartości barycentryczne do dalszych obliczeń
            double denom = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
            double alpha = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / denom;
            double beta = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / denom;
            double gamma = 1.0 - alpha - beta;

            double z = alpha * z1 + beta * z2 + gamma * z3;
            Point3D pixPos = new Point3D(x, y, z);

            // Wektor do źródła światła
            Vector3D lightV = Var.SunPosition - pixPos;
            lightV.Normalize();

            // Interpolowanie wektora normalnego
            double nx = alpha * n1.X + beta * n2.X + gamma * n3.X;
            double ny = alpha * n1.Y + beta * n2.Y + gamma * n3.Y;
            double nz = alpha * n1.Z + beta * n2.Z + gamma * n3.Z;

            Vector3D intN = new Vector3D(nx, ny, nz);
            intN.Normalize();

            if (Var.OwnTexture && Var.textureMap !=  null && Var.textureMapTable != null)
            {
                // Nowe u i v
                double u = alpha * v1.U + beta * v2.U + gamma * v3.U;
                double v = alpha * v1.V + beta * v2.V + gamma * v3.V;

                int xp = (int)(v * (Var.textureMap.PixelWidth - 1));
                int yp = (int)(u * (Var.textureMap.PixelHeight - 1));

                xp = Math.Clamp(xp, 0, Var.textureMap.PixelWidth - 1);
                yp = Math.Clamp(yp, 0, Var.textureMap.PixelHeight - 1);

                r = Var.textureMapTable[xp, yp, 2];
                g = Var.textureMapTable[xp, yp, 1];
                b = Var.textureMapTable[xp, yp, 0];
            }

            if (Var.OwnNormals && Var.normalMap != null && Var.normalMapTable != null)
            {
                // Nowe u i v
                double u = alpha * v1.U + beta * v2.U + gamma * v3.U;
                double v = alpha * v1.V + beta * v2.V + gamma * v3.V;

                // Nowe Pu'
                double pux = alpha * v1.PuVecAfter.X + beta * v2.PuVecAfter.X + gamma * v3.PuVecAfter.X;
                double puy = alpha * v1.PuVecAfter.Y + beta * v2.PuVecAfter.Y + gamma * v3.PuVecAfter.Y;
                double puz = alpha * v1.PuVecAfter.Z + beta * v2.PuVecAfter.Z + gamma * v3.PuVecAfter.Z;
                Vector3D pun = new Vector3D(pux, puy, puz);
                pun.Normalize();

                // Nowe Pv'
                double pvx = alpha * v1.PvVecAfter.X + beta * v2.PvVecAfter.X + gamma * v3.PvVecAfter.X;
                double pvy = alpha * v1.PvVecAfter.Y + beta * v2.PvVecAfter.Y + gamma * v3.PvVecAfter.Y;
                double pvz = alpha * v1.PvVecAfter.Z + beta * v2.PvVecAfter.Z + gamma * v3.PvVecAfter.Z;
                Vector3D pvn = new Vector3D(pvx, pvy, pvz);
                pvn.Normalize();

                int xp = (int)(v * (Var.normalMap.PixelWidth - 1));
                int yp = (int)(u * (Var.normalMap.PixelHeight - 1));

                xp = Math.Clamp(xp, 0, Var.normalMap.PixelWidth - 1);
                yp = Math.Clamp(yp, 0, Var.normalMap.PixelHeight - 1);

                double npx = (Var.normalMapTable[xp, yp, 2] / 255.0) * 2 - 1;
                double npy = (Var.normalMapTable[xp, yp, 1] / 255.0) * 2 - 1;
                double npz = (Var.normalMapTable[xp, yp, 0] / 255.0) * 2 - 1;
                //npz = Math.Clamp(npz, 0, 1);

                Vector3D pixelV = new Vector3D(npx, npy, npz);
                pixelV.Normalize();

                Matrix3D Matrix = new Matrix3D(pun.X, pvn.X, intN.X, 0,
                                               pun.Y, pvn.Y, intN.Y, 0,
                                               pun.Z, pvn.Z, intN.Z, 0,
                                               0, 0, 0, 1);
                intN = Matrix.Transform(pixelV);
                intN.Normalize();
            }

            Vector3D reflectionV = 2 * Vector3D.DotProduct(intN, lightV) * intN - lightV;
            reflectionV.Normalize();

            // składowa zwierciadlana bez uwzględniania kolorów
            double zwierciadlana = Var.ks * Math.Pow(Math.Max(Vector3D.DotProduct(reflectionV, V), 0), Var.m);

            // składowa rozproszona rmodelu oświetlenie bez kolorów
            double rmodel = Var.kd * Math.Max(Vector3D.DotProduct(intN, lightV), 0);

            double kr = r * rl / 255 * (zwierciadlana + rmodel);
            double kg = g * gl / 255 * (zwierciadlana + rmodel);
            double kb = b * bl / 255 * (zwierciadlana + rmodel);

            kr = Math.Min(kr, 255);
            kg = Math.Min(kg, 255);
            kb = Math.Min(kb, 255);

            return System.Drawing.Color.FromArgb((byte)kr, (byte)kg, (byte)kb);
        }

        public void SetPixel(Bitmap bitmap, int xbefore, int ybefore, System.Drawing.Color color)
        {
            int scaleX = 1;
            int scaleY = -1;

            lock (Var.bitmapLock)
            {
                using (var fastBitmap = bitmap.FastLock())
                {
                    double translateX = bitmap.Width / 2;
                    double translateY = bitmap.Height / 2;

                    int x = xbefore * scaleX + (int)translateX;
                    int y = ybefore * scaleY + (int)translateY;


                    if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height)
                        return;

                    fastBitmap.SetPixel(x, y, color);
                }
            }   
        }
    }
}
