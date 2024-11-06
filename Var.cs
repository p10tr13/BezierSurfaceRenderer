using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public static class Var
    {
        public static int RLightColor = 1;

        public static int GLightColor = 1;

        public static int BLightColor = 1;

        public static int RectangleRColor = 248;

        public static int RectangleGColor = 24;

        public static int RectangleBColor = 148;

        public static double kd = 0.5;

        public static double ks = 0.5;

        public static double m = 50;

        public static Point3D SunPosition = new Point3D(0, 1000, 0);

        public static int controlEdgeWidth = 2;

        public static int controlPointRectSize = 10;

        public static int triangleEdgeWidth = 1;

        public static SolidColorBrush triangleStroke = Brushes.HotPink;
    }
}
