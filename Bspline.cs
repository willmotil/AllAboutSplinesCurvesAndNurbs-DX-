using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/
// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/NURBS/NURBS-motiv.html
// https://pages.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/NURBS/RB-circles.html

namespace Microsoft.Xna.Framework
{
    public class Bspline
    {
        Vector4[] cp;

        public Vector4[] solved = new Vector4[10] { new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4(), new Vector4()};

        public Bspline(Vector3[] controlPoints)
        {
            cp = new Vector4[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector4(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z, 1.0f);
        }

        public Vector3 GetSplinePoint( int cpIndex, float fracTime )
        {
            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpIndex - 1);
            int index1 = EnsureIndexInRange(cpIndex + 0);
            int index2 = EnsureIndexInRange(cpIndex + 1);
            int index3 = EnsureIndexInRange(cpIndex + 2);
            return CalculateBspline(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
        }

        /// <summary>
        /// Simple subdivision b spline.
        /// https://www.geogebra.org/m/WPHQ9rUt
        /// </summary>
        public Vector3 CalculateBspline(Vector4 a0, Vector4 a1, Vector4 a2, Vector4 a3, float time)
        {
            var a4 = (a1 - a0) * time + a0;
            var a5 = (a2 - a1) * time + a1;
            var a6 = (a3 - a2) * time + a2;
            var a7 = (a5 - a4) * time + a4;
            var a8 = (a6 - a5) * time + a5;
            var a9 = (a8 - a7) * time + a7;
            solved[0] = a0;
            solved[1] = a1;
            solved[2] = a2;
            solved[3] = a3;
            solved[4] = a4;
            solved[5] = a5;
            solved[6] = a6;
            solved[7] = a7;
            solved[8] = a8;
            solved[9] = a9;
            return ToVector3(a9);
        }
        public Vector3 ToVector3(Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public Vector3 BiCubic(Vector3 a0, Vector3 a1, Vector3 a2, Vector3 a3, float time)
        {
            return (((((a3 - a2) * time + a2) - ((a2 - a1) * time + a1)) * time + ((a2 - a1) * time + a1)) - ((((a2 - a1) * time + a1) - ((a1 - a0) * time + a0)) * time + ((a1 - a0) * time + a0))) * time + ((((a2 - a1) * time + a1) - ((a1 - a0) * time + a0)) * time + ((a1 - a0) * time + a0));
        }

            public int EnsureIndexInRange(int i)
        {
            while (i < 0)
                i = (cp.Length) + i;
            while (i > (cp.Length - 1))
                i = i - (cp.Length);
            return i;
        }
    }
}
