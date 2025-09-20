using System.Windows.Media.Media3D;

namespace GK_Proj_2
{
    static class MathStatic
    {
        public static double BernsteinPolynomial(int n, int k, double t)
        {
            return BinomialCoefficient(n, k) * Math.Pow(t, k) * Math.Pow(1 - t, n - k);
        }

        public static int BinomialCoefficient(int n, int k)
        {
            int r = 1;
            int d;
            if (k > n) return 0;

            for (d = 1; d <= k; d++)
            {
                r *= n--;
                r /= d;
            }
            return r;
        }

        public static Matrix3D GetRotationMatrix(int alpha, int beta)
        {
            double a = (Math.PI / 180) * (double)alpha;
            double b = (Math.PI / 180) * (double)beta;
            Matrix3D alphaMatrix = new Matrix3D() { M11 = Math.Cos(a), M12 = (-1) * Math.Sin(a), M21 = Math.Sin(a), M22 = Math.Cos(a), M33 = 1, M44 = 1 };
            Matrix3D betaMatrix = new Matrix3D() { M11 = 1, M22 = Math.Cos(b), M23 = (-1) * Math.Sin(b), M32 = Math.Sin(b), M33 = Math.Cos(b), M44 = 1 };
            return alphaMatrix * betaMatrix;
        }
    }
}
