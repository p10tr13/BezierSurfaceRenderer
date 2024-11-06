using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Numerics;

namespace GK_Proj_2
{
    public class Vertex
    {
        public Point3D point { get; set; }

        public Vector3D PuVec { get; set; }

        public Vector3D PvVec { get; set; }

        public Vector3D N => Vector3D.CrossProduct(PuVec, PvVec);

        public Point3D pointAfter { get; set; }

        public Vector3D PuVecAfter { get; set; }

        public Vector3D PvVecAfter { get; set; }

        public Vector3D NAfter => Vector3D.CrossProduct(PuVecAfter, PvVecAfter);

        public double U { get; set; }

        public double V { get; set; }

        public Vertex(Point3D point, Vector3D puVec, Vector3D pvVec, double u, double v)
        {
            this.point = point;
            PuVec = puVec;
            PvVec = pvVec;
            pointAfter = new Point3D(point.X, point.Y, point.Z);
            PuVecAfter = new Vector3D(PuVec.X, PuVec.Y, PvVec.Z);
            PvVecAfter = new Vector3D(PvVec.X, PvVec.Y, PvVec.Z);
            this.U = u;
            this.V = v;
        }

        public void Rotate (Matrix3D matrix)
        {
            pointAfter = matrix.Transform(point);
            PuVecAfter = matrix.Transform(PvVec);
            PvVecAfter = matrix.Transform(PvVec);
        }

        public (int ymin, int ymax, int xmin, double m) GetEdgeAtrib(Vertex vother)
        {
            int xmin, ymin, ymax, xmax;
            double m;
            if (vother.pointAfter.Y > pointAfter.Y)
            {
                ymin = (int)pointAfter.Y;
                xmin = (int)pointAfter.X;
                ymax = (int)vother.pointAfter.Y;
                xmax = (int)vother.pointAfter.X;
            }
            else
            {
                ymin = (int)vother.pointAfter.Y;
                xmin = (int)vother.pointAfter.X;
                ymax = (int)pointAfter.Y;
                xmax = (int)pointAfter.X;
            }

            m = ymax == ymin ? 0 : (double)(xmax - xmin) / (double)(ymax - ymin);

            return (ymin, ymax, xmin, m);
        }
    }
}
