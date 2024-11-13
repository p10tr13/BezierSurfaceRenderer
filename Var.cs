using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public static class Var
    {
        public static int RLightColor = 255;

        public static int GLightColor = 255;

        public static int BLightColor = 255;

        public static byte TriangleRColor = 248;

        public static byte TriangleGColor = 24;

        public static byte TriangleBColor = 148;

        public static double kd = 0.5;

        public static double ks = 0.5;

        public static double m = 50;

        public static Point3D SunPosition = new Point3D(0, 0, 300);

        public static int controlEdgeWidth = 2;

        public static int controlPointRectSize = 10;

        public static int triangleEdgeWidth = 1;

        public static SolidColorBrush triangleStroke = Brushes.HotPink;

        public static bool GridDrawingMode = true;

        public static double SunSpiralRadius = 300.0;

        public static double SunSpiralHeight = 300.0;

        public static double SunSpiralCycleSpeed = 4.0;

        public static double SunAngle = 0;

        public static bool OwnNormals = false;

        public static bool OwnTexture = false;

        public static BitmapSource? normalMap = null;

        public static byte[,,]? normalMapTable = null;

        public static BitmapSource? textureMap = null;

        public static byte[,,]? textureMapTable = null;

        public static readonly object bitmapLock = new object();
    }
}
