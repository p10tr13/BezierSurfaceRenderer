using System.Drawing;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace GK_Proj_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BezierSurface? bezierSurface { get; set; }

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
                Point3D[,] controlPoints = new Point3D[4,4];
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
                    MessageBox.Show("Wystąpił błąd podczas czytania pliku","Error",MessageBoxButton.OK, MessageBoxImage.Error);
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
            if(bezierSurface != null)
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

        private void DrawControlPoints()
        {
            if (bezierSurface == null)
                return;
            double zoom = ZoomSlider.Value;

            Point3D[,] points = bezierSurface.GetRotatedControlPoints((int)AlphaSlider.Value, (int)BetaSlider.Value);
            for (int i = 0; i < points.GetLength(0); i++)
            {
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.DarkGreen
                    };
                    Canvas.SetLeft(rect, points[i, j].X * zoom - rect.Width / 2);
                    Canvas.SetTop(rect, points[i, j].Y * zoom - rect.Width / 2);
                    MyCanvas.Children.Add(rect);
                }
            }

            for (int i = 0; i < points.GetLength(0); i++)
            {
                for (int j = 0; j < points.GetLength(1); j++)
                {
                    if (j != points.GetLength(1) - 1)
                    {
                        Line line = new Line
                        {
                            X1 = points[i, j].X * zoom,
                            Y1 = points[i, j].Y * zoom,
                            X2 = points[i, j + 1].X * zoom,
                            Y2 = points[i, j + 1].Y * zoom,
                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        };
                        MyCanvas.Children.Add(line);
                    }

                    if (i != points.GetLength(0) - 1)
                    {
                        Line line = new Line
                        {
                            X1 = points[i, j].X * zoom,
                            Y1 = points[i, j].Y * zoom,
                            X2 = points[i + 1, j].X * zoom,
                            Y2 = points[i + 1, j].Y * zoom,
                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        };
                        MyCanvas.Children.Add(line);
                    }
                }
            }

        }

        private void DrawTriangleGrid()
        {
            if (bezierSurface == null)
                return;
            double zoom = ZoomSlider.Value;

            foreach (Triangle tri in bezierSurface.Triangles)
            {
                Line line1 = new Line
                {
                    X1 = tri.v1.pointAfter.X * zoom,
                    Y1 = tri.v1.pointAfter.Y * zoom,
                    X2 = tri.v2.pointAfter.X * zoom,
                    Y2 = tri.v2.pointAfter.Y * zoom,
                    Stroke = Brushes.HotPink,
                    StrokeThickness = 1
                };
                MyCanvas.Children.Add(line1);

                Line line2 = new Line
                {
                    X1 = tri.v1.pointAfter.X * zoom,
                    Y1 = tri.v1.pointAfter.Y * zoom,
                    X2 = tri.v3.pointAfter.X * zoom,
                    Y2 = tri.v3.pointAfter.Y * zoom,
                    Stroke = Brushes.HotPink,
                    StrokeThickness = 1
                };
                MyCanvas.Children.Add(line2);

                Line line3 = new Line
                {
                    X1 = tri.v2.pointAfter.X * zoom,
                    Y1 = tri.v2.pointAfter.Y * zoom,
                    X2 = tri.v3.pointAfter.X * zoom,
                    Y2 = tri.v3.pointAfter.Y * zoom,
                    Stroke = Brushes.HotPink,
                    StrokeThickness = 1
                };
                MyCanvas.Children.Add(line3);
            }
        }

        private void DrawFilledTriangles()
        {
            if (bezierSurface == null)
                return;
            double zoom = ZoomSlider.Value;

            foreach (Triangle tri in bezierSurface.Triangles)
            {
                Polygon polygon = new Polygon()
                {
                    Stroke = Brushes.HotPink,
                    StrokeThickness = 1,
                    Fill = Brushes.LightPink
                };
                polygon.Points.Add(new System.Windows.Point(tri.v1.pointAfter.X * zoom, tri.v1.pointAfter.Y * zoom));
                polygon.Points.Add(new System.Windows.Point(tri.v2.pointAfter.X * zoom, tri.v2.pointAfter.Y * zoom));
                polygon.Points.Add(new System.Windows.Point(tri.v3.pointAfter.X * zoom, tri.v3.pointAfter.Y * zoom));
                MyCanvas.Children.Add(polygon);
            }
        }

        private void Draw()
        {
            MyCanvas.Children.Clear();
            //DrawTriangleGrid();
            DrawFilledTriangles();
            DrawControlPoints();
        }

        private void ChangeLightButton_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(RTextBox.Text,out int r) && int.TryParse(GTextBox.Text, out int g) && int.TryParse(BTextBox.Text, out int b) && 
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
    }
}