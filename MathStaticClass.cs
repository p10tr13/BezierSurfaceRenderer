using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
