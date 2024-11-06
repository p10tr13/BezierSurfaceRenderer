using System.Runtime.Intrinsics;

namespace GK_Proj_2
{
    public class Triangle
    {
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public Vertex v3 { get; set; }

        public double MeanZ => (v1.pointAfter.Z + v2.pointAfter.Z + v3.pointAfter.Z)/3.0;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }
}
