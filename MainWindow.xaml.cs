using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using Rectangle = System.Windows.Shapes.Rectangle;
using System.Drawing.Imaging;

namespace GK_Proj_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BezierSurface? bezierSurface { get; set; }

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            // Wczytanie przykładowych map
            string basePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));

            string surfaceMapPath = System.IO.Path.Combine(basePath, "Surfaces/111.txt");
            Point3D[,] controlPoints = new Point3D[4, 4];
            using StreamReader reader = new StreamReader(surfaceMapPath);
            string line = reader.ReadLine();
            for (int i = 0; i < 16; i++)
            {
                string[] cords = line.Split(" ");
                double x, y, z;
                if (double.TryParse(cords[0], out x) && double.TryParse(cords[1], out y) && double.TryParse(cords[2], out z))
                {
                    int j = i % 4;
                    controlPoints[(i - j) / 4, j] = new Point3D(x, y, z);
                }
                else
                    throw new Exception();

                line = reader.ReadLine();
            }
            bezierSurface = new BezierSurface(controlPoints, (int)PrecisionSlider.Value);
            reader.Close();
            Draw();

            string normalMapPath = System.IO.Path.Combine(basePath, "NormalMaps/Sci-Fi_Wall_015_normal.png");
            Var.normalMap = new BitmapImage(new Uri(normalMapPath, UriKind.RelativeOrAbsolute));
            FillNormalsMapTable();

            string baseColorMapPath = System.IO.Path.Combine(basePath, "TextureMaps/Sci-Fi_Wall_015_basecolor.png");
            Var.textureMap = new BitmapImage(new Uri(baseColorMapPath, UriKind.Relative));
            FillTextureMapTable();
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                Point3D[,] controlPoints = new Point3D[4, 4];
                try
                {
                    using StreamReader reader = new StreamReader(filename);
                    string line = reader.ReadLine();
                    for (int i = 0; i < 16; i++)
                    {
                        string[] cords = line.Split(" ");
                        double x, y, z;
                        if (double.TryParse(cords[0], out x) && double.TryParse(cords[1], out y) && double.TryParse(cords[2], out z))
                        {
                            int j = i % 4;
                            controlPoints[(i - j) / 4, j] = new Point3D(x, y, z);
                        }
                        else
                            throw new Exception();

                        line = reader.ReadLine();
                    }
                    bezierSurface = new BezierSurface(controlPoints, (int)PrecisionSlider.Value);
                    reader.Close();
                    Draw();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas czytania pliku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PrecisionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bezierSurface != null && bezierSurface.steps != (int)PrecisionSlider.Value)
            {
                bezierSurface.ChangeSteps((int)PrecisionSlider.Value);
                if (AlphaSlider.Value != 0 || BetaSlider.Value != 0)
                    bezierSurface.Rotate((int)AlphaSlider.Value, (int)BetaSlider.Value);
                Draw();
            }
        }

        private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bezierSurface != null)
            {
                bezierSurface.Rotate((int)AlphaSlider.Value, (int)BetaSlider.Value);
                Draw();
            }
        }

        private void BetaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bezierSurface != null)
            {
                bezierSurface.Rotate((int)AlphaSlider.Value, (int)BetaSlider.Value);
                bezierSurface.Triangles = bezierSurface.Triangles.OrderBy(tri => tri.MeanZ).ToList();
                Draw();
            }
        }

        private void MyCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            ScaleTransform scaleTransform = new ScaleTransform(1, -1);
            TranslateTransform translateTransform = new TranslateTransform(MyCanvas.ActualWidth / 2, MyCanvas.ActualHeight / 2);
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            MyCanvas.RenderTransform = transformGroup;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Draw();
        }

        // Rysowanie siatki z efektem głębi (sortowanie po współrzędnej z podczas obortu beta)
        // Aby zobaczyć efekt można włączyć proste wypełnianie trójkątów przy rysowaniu Polygon w tej funkcji
        private void DrawGrid()
        {
            MyCanvas.Children.Clear();

            if (bezierSurface == null)
                return;

            double zoom = ZoomSlider.Value;

            Point3D[,] points = bezierSurface.GetRotatedControlPoints((int)AlphaSlider.Value, (int)BetaSlider.Value);

            // Stworzenie posortowanej listy punktów kontrolnych
            List<Point3D> pointList = [.. points];
            pointList = pointList.OrderBy(point => point.Z).ToList();
            int p = 0;

            // Stworzenie posortowanej listy połączeń pomiędzy punktami kontrolnymi
            List<(double MeanZ, double x1, double y1, double x2, double y2)> edgeList = [];
            for (int i = 0; i < points.GetLength(0); i++)
            {
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    if (j != points.GetLength(1) - 1)
                    {
                        edgeList.Add(((points[i, j].Z + points[i, j + 1].Z) / 2.0, points[i, j].X * zoom, points[i, j].Y * zoom,
                            points[i, j + 1].X * zoom, points[i, j + 1].Y * zoom));
                    }

                    if (i != points.GetLength(0) - 1)
                    {
                        edgeList.Add(((points[i, j].Z + points[i + 1, j].Z) / 2.0, points[i, j].X * zoom, points[i, j].Y * zoom,
                            points[i + 1, j].X * zoom, points[i + 1, j].Y * zoom));
                    }
                }
            }
            edgeList = edgeList.OrderBy(edge => edge.MeanZ).ToList();
            int e = 0;

            for (int i = 0; i < bezierSurface.Triangles.Count; i++)
            {
                Triangle tri = bezierSurface.Triangles[i];

                while (p != pointList.Count && tri.MeanZ >= pointList[p].Z)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = Var.controlPointRectSize,
                        Height = Var.controlPointRectSize,
                        Fill = System.Windows.Media.Brushes.DarkGreen
                    };
                    Canvas.SetLeft(rect, pointList[p].X * zoom - rect.Width / 2);
                    Canvas.SetTop(rect, pointList[p].Y * zoom - rect.Width / 2);
                    MyCanvas.Children.Add(rect);
                    p++;
                }


                while (e != edgeList.Count && tri.MeanZ >= edgeList[e].MeanZ)
                {
                    Line line = new Line
                    {
                        X1 = edgeList[e].x1,
                        Y1 = edgeList[e].y1,
                        X2 = edgeList[e].x2,
                        Y2 = edgeList[e].y2,
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = Var.controlEdgeWidth
                    };
                    MyCanvas.Children.Add(line);
                    e++;
                }

                Polygon polygon = new Polygon()
                {
                    Stroke = Var.triangleStroke,
                    StrokeThickness = Var.triangleEdgeWidth
                };
                polygon.Points.Add(new System.Windows.Point(tri.v1.pointAfter.X * zoom, tri.v1.pointAfter.Y * zoom));
                polygon.Points.Add(new System.Windows.Point(tri.v2.pointAfter.X * zoom, tri.v2.pointAfter.Y * zoom));
                polygon.Points.Add(new System.Windows.Point(tri.v3.pointAfter.X * zoom, tri.v3.pointAfter.Y * zoom));
                MyCanvas.Children.Add(polygon);
            }

            while (p != pointList.Count)
            {
                Rectangle rect = new Rectangle
                {
                    Width = Var.controlPointRectSize,
                    Height = Var.controlPointRectSize,
                    Fill = System.Windows.Media.Brushes.DarkGreen
                };
                Canvas.SetLeft(rect, pointList[p].X * zoom - rect.Width / 2);
                Canvas.SetTop(rect, pointList[p].Y * zoom - rect.Width / 2);
                MyCanvas.Children.Add(rect);
                p++;
            }

            while (e != edgeList.Count)
            {
                Line line = new Line
                {
                    X1 = edgeList[e].x1,
                    Y1 = edgeList[e].y1,
                    X2 = edgeList[e].x2,
                    Y2 = edgeList[e].y2,
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = Var.controlEdgeWidth
                };
                MyCanvas.Children.Add(line);
                e++;
            }
        }

        // Rysowanie wypełnienia trójkątów
        private void DrawFill()
        {
            MyCanvas.Children.Clear();

            if (bezierSurface == null)
                return;

            int width = (int)MyCanvas.ActualWidth;
            int height = (int)MyCanvas.ActualHeight;
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.White);
            }

            for (int i = 0; i < bezierSurface.Triangles.Count; i++)
            {
                Triangle tri = bezierSurface.Triangles[i];

                tri.Fill(bitmap);
            }

            // Za dużo jest zmiennych dzielonych, które można by niby przekazać w wywołaniu funckji,
            // ale zyski w wydajności nie były na tyle duże, aby był sens tak robić (według mnie).
            //Parallel.ForEach(bezierSurface.Triangles, tri =>
            //{
            //    tri.Fill(bitmap);
            //});

            BitmapSource source = ConvertBitmapToBitmapSource(bitmap);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image { Source = source, Width = bitmap.Width, Height = bitmap.Height };
            Canvas.SetLeft(image, -MyCanvas.ActualWidth / 2);
            Canvas.SetTop(image, MyCanvas.ActualHeight / 2);
            image.RenderTransform = new ScaleTransform(1, -1);
            MyCanvas.Children.Add(image);

            // Elipsa ukazująca pozycje światła dla sprawdzenia, czy ok świeci
            Ellipse sun = new Ellipse();
            sun.Stroke = System.Windows.Media.Brushes.Green;
            sun.Width = 10;
            sun.Height = 10;
            Canvas.SetLeft(sun, Var.SunPosition.X - 5);
            Canvas.SetTop(sun, Var.SunPosition.Y - 5);
            MyCanvas.Children.Add(sun);
        }

        private void Draw()
        {
            if (Var.GridDrawingMode)
                DrawGrid();
            else DrawFill();
        }

        private void ChangeLightButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RTextBox.Text, out int r) && int.TryParse(GTextBox.Text, out int g) && int.TryParse(BTextBox.Text, out int b) &&
                r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255)
            {
                Var.RLightColor = r;
                Var.GLightColor = g;
                Var.BLightColor = b;
                Draw();
            }
            else
                MessageBox.Show("Złe kolory", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void kdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Var.kd = kdSlider.Value;
            Draw();
        }

        private void ksSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Var.ks = ksSlider.Value;
            Draw();
        }

        private void mSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Var.m = mSlider.Value;
            Draw();
        }

        private void GridRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Var.GridDrawingMode = true;
            if (timer != null)
                timer.Stop();
            Draw();
        }

        private void FillRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Var.GridDrawingMode = false;
            Draw();
        }

        private void ChangeObjButton_Click(object sender, RoutedEventArgs e)
        {
            if (byte.TryParse(RObjTextBox.Text, out byte r) && byte.TryParse(GObjTextBox.Text, out byte g) && byte.TryParse(BObjTextBox.Text, out byte b) &&
                r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255)
            {
                Var.TriangleRColor = r;
                Var.TriangleGColor = g;
                Var.TriangleBColor = b;
                Draw();
            }
            else
                MessageBox.Show("Złe kolory", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UpdateSunPosition(object sender, EventArgs e)
        {
            Var.SunAngle += Var.SunSpiralCycleSpeed * timer.Interval.TotalSeconds;

            double sunX = Var.SunSpiralRadius * Math.Cos(Var.SunAngle);
            double sunY = Var.SunSpiralRadius * Math.Sin(Var.SunAngle);
            Var.SunPosition = new Point3D(sunX, sunY, Var.SunSpiralHeight);
            Draw();
        }

        private void StartSunAnimation(object sender, RoutedEventArgs e)
        {
            if (timer != null && timer.IsEnabled)
                return;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(40);
            timer.Tick += UpdateSunPosition;
            Var.SunAngle = 0.0;
            timer.Start();
        }

        private void StopSunAnimation(object sender, RoutedEventArgs e)
        {
            if (timer != null)
                timer.Stop();
        }

        private void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer == null || !timer.IsEnabled)
            {
                Var.SunPosition.Z = (double)zSlider.Value;
                Draw();
                return;
            }
            Var.SunSpiralHeight = zSlider.Value;
            Draw();
        }

        private void NormalsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                try
                {
                    Var.normalMap = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
                    FillNormalsMapTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas czytania pliku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillNormalsMapTable()
        {
            if (Var.normalMap == null)
                return;

            Var.normalMapTable = new byte[Var.normalMap.PixelWidth, Var.normalMap.PixelHeight, 3];

            for (int i = 0; i < Var.normalMap.PixelWidth; i++)
            {
                for (int j = 0; j < Var.normalMap.PixelHeight; j++)
                {
                    byte[] pixels = new byte[4];
                    Var.normalMap.CopyPixels(new Int32Rect(i, j, 1, 1), pixels, 4, 0);
                    Var.normalMapTable[i, j, 0] = pixels[0];
                    Var.normalMapTable[i, j, 1] = pixels[1];
                    Var.normalMapTable[i, j, 2] = pixels[2];
                }
            }
        }

        private void NormalsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Var.normalMap != null)
            {
                Var.OwnNormals = true;
                Draw();
            }
            else
            {
                MessageBox.Show("Brak mapy wektorów normalnych", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TextureCheckBox.IsChecked = false;
            }
        }

        private void NormalsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Var.OwnNormals = false;
            Draw();
        }

        private void TextureButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;
                try
                {
                    Var.textureMap = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
                    FillTextureMapTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas czytania pliku", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FillTextureMapTable()
        {
            if (Var.textureMap == null)
                return;

            Var.textureMapTable = new byte[Var.textureMap.PixelWidth, Var.textureMap.PixelHeight, 3];

            for (int i = 0; i < Var.textureMap.PixelWidth; i++)
            {
                for (int j = 0; j < Var.textureMap.PixelHeight; j++)
                {
                    byte[] pixels = new byte[4];
                    Var.textureMap.CopyPixels(new Int32Rect(i, j, 1, 1), pixels, 4, 0);
                    Var.textureMapTable[i, j, 0] = pixels[0];
                    Var.textureMapTable[i, j, 1] = pixels[1];
                    Var.textureMapTable[i, j, 2] = pixels[2];
                }
            }
        }

        private void TextureCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Var.textureMap != null)
            {
                Var.OwnTexture = true;
                Draw();
            }
            else
            {
                MessageBox.Show("Brak mapy wektorów normalnych", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TextureCheckBox.IsChecked = false;
            }
        }

        private void TextureCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Var.OwnTexture = false;
            Draw();
        }

        public BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}