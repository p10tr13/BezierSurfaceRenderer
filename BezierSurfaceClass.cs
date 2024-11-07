using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    public class BezierSurface
    {
        public BezierSurface(Point3D[,] controlPoints, int steps)
        {
            this.controlPoints = controlPoints;
            this.steps = steps;
            Triangles = CreateTriangles();
        }

        public Point3D[,] controlPoints { get; set; }

        public int steps { get; set; }

        public List<Vertex> Grid { get; set; }

        public List<Triangle> Triangles { get; set; }

        public List<Vertex> CreateGrid()
        {
            List<Vertex> grid = new List<Vertex>();
            for (int i = 0; i<= steps; i++)
            {
                double u = (double)i / steps;
                for (int j = 0; j <= steps; j++)
                {
                    double v = (double)j / steps;
                    grid.Add(CalculateBezierSurfaceVertex(u,v));
                }
            }
            return grid;
        }

        public Vertex CalculateBezierSurfaceVertex(double u, double v)
        {
            int n = controlPoints.GetLength(0) - 1;
            int m = controlPoints.GetLength(1) - 1;

            Point3D p = new Point3D(0, 0, 0);
            Vector3D pu = new Vector3D(0, 0, 0), pv = new Vector3D(0, 0, 0);

            for (int i = 0; i <= n; i++)
            {
                for (int j = 0;j <= m; j++)
                {
                    double Bi = MathStatic.BernsteinPolynomial(n, i, u);
                    double Bj = MathStatic.BernsteinPolynomial(m, j, v);
                    p = p.Add(controlPoints[i, j].Multiply(Bi*Bj));

                    if (i != n)
                    {
                        double Bim1 = MathStatic.BernsteinPolynomial(n - 1, i, u);
                        pu += Point3D.Subtract(controlPoints[i + 1, j], controlPoints[i, j]) * Bim1 * Bj;
                    }

                    if (j != m)
                    {
                        double Bjm1 = MathStatic.BernsteinPolynomial(m - 1, j, v);
                        pv += Point3D.Subtract(controlPoints[i, j + 1], controlPoints[i, j]) * Bjm1 * Bi;
                    }
                }
            }
            pu = pu * n;
            pv = pv * m;

            return new Vertex(p,pu,pv, u, v);
        }

        public List<Triangle> CreateTriangles()
        {
            Grid = CreateGrid();
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    int ind = i * (steps + 1) + j;
                    Vertex v1, v2, v3, v4;
                    v1 = Grid[ind];
                    v2 = Grid[ind + 1];
                    v3 = Grid[ind + steps + 1];
                    v4 = Grid[ind + steps + 2];

                    triangles.Add(new Triangle(v1, v2, v3));
                    triangles.Add(new Triangle(v2, v3, v4));
                }
            }
            return triangles;
        }

        public void ChangeSteps(int steps)
        {
            this.steps = steps;
            Triangles = CreateTriangles();
        }

        public void Rotate(int alpha, int beta)
        {            
            Matrix3D matrix = MathStatic.GetRotationMatrix(alpha, beta);
            foreach (Vertex v in Grid)
            {
                v.Rotate(matrix);
            }
        }

        public Point3D[,] GetRotatedControlPoints(int alpha, int beta)
        {
            Point3D[,] result = new Point3D[controlPoints.GetLength(0), controlPoints.GetLength(1)];
            Matrix3D matrix = MathStatic.GetRotationMatrix(alpha, beta);
            for (int i = 0; i < controlPoints.GetLength(0); i++)
            {
                for (int j = 0; j < controlPoints.GetLength(1); j++)
                {
                    result[i,j] = matrix.Transform(controlPoints[i,j]);
                }
            }
            return result;
        }
    }
}
