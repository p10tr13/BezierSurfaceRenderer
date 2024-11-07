using System.Drawing;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using Rectangle = System.Windows.Shapes.Rectangle;

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
                        Fill = Brushes.DarkGreen
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
                        Stroke = Brushes.Black,
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
                    Fill = Brushes.DarkGreen
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
                    Stroke = Brushes.Black,
                    StrokeThickness = Var.controlEdgeWidth
                };
                MyCanvas.Children.Add(line);
                e++;
            }
        }

        private void DrawFill()
        {
            MyCanvas.Children.Clear();

            if (bezierSurface == null)
                return;

            double zoom = ZoomSlider.Value;

            int width = (int)MyCanvas.ActualWidth;
            int height = (int)MyCanvas.ActualHeight;
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            for (int i = 0; i < bezierSurface.Triangles.Count; i++)
            {
                Triangle tri = bezierSurface.Triangles[i];

                tri.Fill(bitmap, (int)zoom);
            }

            Image image = new Image { Source = bitmap };
            Canvas.SetLeft(image, -MyCanvas.ActualWidth / 2);
            Canvas.SetTop(image, MyCanvas.ActualHeight / 2);
            image.RenderTransform = new ScaleTransform(1, -1);
            MyCanvas.Children.Add(image);
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
            if (timer !=  null)
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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(40);
            timer.Tick += UpdateSunPosition;
            Var.SunAngle = 0.0;
            timer.Start();
        }

        private void StopSunAnimation(object sender, RoutedEventArgs e) 
        { 
            if(timer != null)
                timer.Stop(); 
        }

        private void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Var.SunSpiralHeight = zSlider.Value;
            Draw();
        }
    }
}