using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public class Vertex
    {
        public Point3D point { get; set; }

        public Vector3D PuVec { get; set; }

        public Vector3D PvVec { get; set; }

        public Vector3D N => Vector3D.CrossProduct(PuVec, PvVec); // Not normalized yet

        public Point3D pointAfter { get; set; }

        public Vector3D PuVecAfter { get; set; }

        public Vector3D PvVecAfter { get; set; }

        public Vector3D NAfter => Vector3D.CrossProduct(PuVecAfter, PvVecAfter); // Not normalized yet

        public double U { get; set; }

        public double V { get; set; }

        public Vertex(Point3D point, Vector3D puVec, Vector3D pvVec, double u, double v)
        {
            this.point = point;
            PuVec = puVec;
            PvVec = pvVec;
            pointAfter = new Point3D(point.X, point.Y, point.Z);
            PuVecAfter = new Vector3D(PuVec.X, PuVec.Y, PuVec.Z);
            PvVecAfter = new Vector3D(PvVec.X, PvVec.Y, PvVec.Z);
            this.U = u;
            this.V = v;
        }

        public void Rotate (Matrix3D matrix)
        {
            pointAfter = matrix.Transform(point);
            PuVecAfter = matrix.Transform(PuVec);
            PvVecAfter.Normalize();
            PvVecAfter = matrix.Transform(PvVec);
            PvVecAfter.Normalize();
        }

        public (double ymin, double ymax, double xmin, double m) GetEdgeAtrib(Vertex vother)
        {
            double xmin, ymin, ymax, xmax;
            double m;
            if (vother.pointAfter.Y > pointAfter.Y)
            {
                ymin = pointAfter.Y;
                xmin = pointAfter.X;
                ymax = vother.pointAfter.Y;
                xmax = vother.pointAfter.X;
            }
            else
            {
                ymin = vother.pointAfter.Y;
                xmin = vother.pointAfter.X;
                ymax = pointAfter.Y;
                xmax = pointAfter.X;
            }

            m = ymax == ymin ? 0 : (xmax - xmin) / (ymax - ymin);

            return (ymin, ymax, xmin, m);
        }
    }
}
