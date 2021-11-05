using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Owls.Flight
{
    public static class FlightBezier
    {
        // factorial is capped at 16
        private static readonly float[] Factorial = new float[]
        {
            1.0f,
            1.0f,
            2.0f,
            6.0f,
            24.0f,
            120.0f,
            720.0f,
            5040.0f,
            40320.0f,
            362880.0f,
            3628800.0f,
            39916800.0f,
            479001600.0f,
            6227020800.0f,
            87178291200.0f,
            1307674368000.0f,
            20922789888000.0f,
        };

        // Calculates a point on the curve at interval t. t = 0 means start point, t = 1 is end point.
        public static Vector3 Point2(float t, List<Vector2> controlPoints)
        {
            int N = controlPoints.Count - 1;
            controlPoints = CheckPointRange(N, controlPoints);

            if (t <= 0) { return controlPoints[0]; }
            if (t >= 1) { return controlPoints[controlPoints.Count - 1]; }

            Vector3 p = new Vector2();

            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }

            return p;
        }

        public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval = 0.01f)
        {
            int N = controlPoints.Count - 1;
            controlPoints = CheckPointRange(N, controlPoints);

            List<Vector2> points = new List<Vector2>();
            for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
            {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            points.Add(p);
            }

            return points;
        }

        private static List<Vector2> CheckPointRange(int N, List<Vector2> controlPoints)
        {
            if (N > 16)
            {
                Debug.LogError("Maximum points for the flight curve 16.");
                controlPoints.RemoveRange(16, controlPoints.Count - 16);
            }

            return controlPoints;
        }

        private static float Bernstein(int n, int i, float t)
        {
            float t_i = Mathf.Pow(t, i);
            float t_n_minus_i = Mathf.Pow((1 - t), (n - i));

            float basis = Binomial(n, i) * t_i * t_n_minus_i;
            return basis;
        }

        private static float Binomial(int n, int i)
        {
            float ni;
            float a1 = Factorial[n];
            float a2 = Factorial[i];
            float a3 = Factorial[n - i];
            ni = a1 / (a2 * a3);
            return ni;
        }
    }
}
