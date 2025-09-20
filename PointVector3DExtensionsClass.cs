using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public static class PointVector3DExtensions
    {
        public static Point3D Add(this Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3D Multiply(this Point3D p, double d)
        {
            return new Point3D(p.X * d, p.Y * d, p.Z * d);
        }
    }
}
