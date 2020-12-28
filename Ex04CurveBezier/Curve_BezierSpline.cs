using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Curve_BezierSpline
    {
        // A entire treaties on beziers it looks to be ... https://pomax.github.io/bezierinfo/index.html

        // https://www.youtube.com/watch?v=tXrkNfHFakg  matrix thru points.
        // https://www.youtube.com/watch?v=sZITw1X2ctU si(x) = Ai + Bi(X-Xi)+Ci(X - Xi)^2 + Di(X- Xi)^3;
        // Cubic  https://www.youtube.com/watch?v=pnYccz1Ha34
        // i = (1-t);
        // C0(t) = i^3 * P0 + 3*i^2*t * P1+ 3i*t^2* P2 +t^3*P3
        // Thru the control points https://www.youtube.com/watch?v=2hL1LGMVnVM
        // excellent answer for were to position extra cp 's to induce a circle from 4 beziers.
        // https://stackoverflow.com/questions/1734745/how-to-create-circle-with-b%c3%a9zier-curves#27863181

        // https://www.youtube.com/watch?v=a55tOCUy9oI ok from this video at 1:35 i got a inspiration.
        // i need to generate interior artificial splines for a closed bezier based on any points surround segmental tangent to the accounted point and then the following the same
        // in order to generate the tangental artificial points for the interior 2 points of a sampled bezer  essentially a bezier will need to generate 6 points to get the g2 curvature between 
        // the original points p1 p2    of a 4 point bezier this means only a segment of the bezier is validly plotted and the entire bezier is closed in this context.
        // so for a closed 4 point bezier a additional 8 points are generated. All of them representing the interior knots between the original points.
        // that should give c1 at least maybe c2 continuity with the curve intersecting the cp's once all the artificial positions are determined properly.

        //  https://www.cs.drexel.edu/~david/Classes/CS536/Lectures/L-04_BSplines_NURBS.6.pdf

        Vector3[] cp;
        Vector3[] curveLinePoints;
        public int _numOfVisualCurvatureSegmentPoints = 100;
        // Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        private float _weight = 0.5f;
        List<Vector3> artificialCpLine = new List<Vector3>();
        List<Vector3> artificialTangentLine = new List<Vector3>();

        public Curve_BezierSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public Curve_BezierSpline(Vector3[] controlPoints, bool useWikiVersion, float weight)
        {
            _weight = weight;
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public Curve_BezierSpline(Vector3[] controlPoints, bool useWikiVersion, float weight, int numOfVisualCurvatureSegmentPoints)
        {
            _weight = weight;
            _numOfVisualCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateSpline(controlPoints, useWikiVersion);
        }



        #region  

        private void CreateSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            artificialCpLine.Clear();
            artificialTangentLine.Clear();
            cp = new Vector3[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);

            curveLinePoints = new Vector3[_numOfVisualCurvatureSegmentPoints];

            var loopCount = _numOfVisualCurvatureSegmentPoints;
            var divisor = loopCount - 1;

            for (int i = 0; i < loopCount; i++)
            {
                float t = (float)(i) / (float)(divisor);
                curveLinePoints[i] = GetSplinePoint(t);
            }
        }

        private Vector3 GetSplinePoint(float Time)
        {
            var plotRange = cp.Length;
            var offset = plotRange * Time;
            var index = (int)(offset);
            var fracTime = offset - (float)index;
            return DetermineSplines(index, fracTime);
            //return GetSplinePoint(1, fracTime);
        }

        private Vector3 DetermineSplines(int cpIndex, float fracTime)
        {
            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpIndex - 1);
            int index1 = EnsureIndexInRange(cpIndex + 0);
            int index2 = EnsureIndexInRange(cpIndex + 1);
            int index3 = EnsureIndexInRange(cpIndex + 2);
            return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
        }

        public int EnsureIndexInRange(int i)
        {
            while (i < 0)
                i = i + (cp.Length);
            while (i > (cp.Length - 1))
                i = i - (cp.Length);
            return i;
        }


        /// <summary>
        /// Here we go this one is only for closed curves open ones are easy just append the end points to the control point list
        /// </summary>
        /// <returns></returns>
        public Vector3 WeightedPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float time)
        {
            Vector3 p0 = v0; Vector3 p1 = v1; Vector3 p2 = v2; Vector3 p3 = v3;

            var segmentDistance = Vector3.Distance(v2 , v1) * 0.35355339f * _weight;

            var n1 = GetIdealTangentVector(v0, v1, v2);
            n1.Normalize();
            var d1 = segmentDistance;
            p1 = v1 + n1 * d1;
            p0 = v1;

            var n2 = GetIdealTangentVector(v3, v2, v1);
            n2.Normalize();
            var d2 = segmentDistance;
            p2 = v2 + n2 * d2;
            p3 = v2;

            //float t = time * .33f + .33f;
            float t = time;
            float t2 = t * t;
            float t3 = t2 * t;
            float i = 1f - t;
            float i2 = i * i;
            float i3 = i2 * i;

            Vector3 result =
                (i3) * 1f * p0 +
                (i2 * t) * 3f * p1 +
                (i * t2) * 3f * p2 +
                (t3) * 1f * p3;

            artificialCpLine.Add(p0);  // visualization stuff.
            artificialCpLine.Add(p1);
            artificialCpLine.Add(p2);
            artificialCpLine.Add(p3);

            return result;
        }

        public Vector3 GetIdealTangentVector(Vector3 a, Vector3 b, Vector3 c)
        {
            float disa = Vector3.Distance(a, b);
            float disc = Vector3.Distance(b, c);
            float total = disa + disc;
            float ratioa = disa / total;
            float ratioc = disc / total;

            var pAB = ((b - a) * ratioa) + a;
            var pBC = ((c - b) * ratioa) + b;
            var result = pBC - pAB;

            artificialTangentLine.Add(pAB); // visualization stuff.
            artificialTangentLine.Add(pBC);

            return result;
        }


        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, SpriteFont font, GameTime gameTime)
        {
            DrawWithSpriteBatch(_spriteBatch, gameTime);

            for (int i = 0; i < cp.Length; i++)
            {
                _spriteBatch.DrawString(font, $" CP[{i}]", new Vector2(cp[i].X, cp[i].Y), Color.Black);
            }
        }

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, GameTime gameTime)
        {

            bool flip = false;
            for (int i = 0; i < curveLinePoints.Length - 1; i++)
            {
                if (flip)
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Green);
                else
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Black);

                if (i < 2)
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Yellow);

                flip = !flip;
            }

            for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
            {
                DrawHelpers.DrawBasicLine(new Vector2(artificialCpLine[i].X, artificialCpLine[i].Y), new Vector2(artificialCpLine[i + 1].X, artificialCpLine[i + 1].Y), 1, Color.Purple);
            }

            for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
            {
                DrawHelpers.DrawBasicLine(new Vector2(artificialTangentLine[i].X, artificialTangentLine[i].Y), new Vector2(artificialTangentLine[i + 1].X, artificialTangentLine[i + 1].Y), 1, Color.Pink);
            }

            for (int i = 0; i < cp.Length; i++)
            {
                DrawHelpers.DrawBasicPoint(new Vector2(cp[i].X, cp[i].Y), Color.Red);
            }
        }

        public Vector3 MidPoint(Vector3 a, Vector3 b)
        {
            return (a + b) / 2;
        }
        public Vector3 MidPoint(Vector3 a, Vector3 b , Vector3 c)
        {
            return (a + b + c) / 3;
        }

        public Vector3 ToVector3(Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public Vector2 ToVector2(Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public Vector2 ToVector2(Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }

    }
}
