using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public class Vertex
    {
        public Point3D point { get; set; }

        public Vector3D PuVec { get; set; }

        public Vector3D PvVec { get; set; }

        public Vector3D N => Vector3D.CrossProduct(PuVec, PvVec);

        public Vertex(Point3D point, Vector3D puVec, Vector3D pvVec)
        {
            this.point = point;
            PuVec = puVec;
            PvVec = pvVec;
        }
    }
}
