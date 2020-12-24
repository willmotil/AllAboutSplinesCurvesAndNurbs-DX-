using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework
{
    public class CurveMyImbalancedSpline
    {
        int order = 2;
        int curveSegmentsEndIndex = 0;
        int curveLineSegmentsLength = 0;
        int cpEndIndex = 0;
        float segmentsSummedDistance = 0f;
        Vector4[] cp;
        List<LineSegment> curveLineSegments = new List<LineSegment>();
        List<LineSegment> curveUniformedLineSegments = new List<LineSegment>();
        List<float> curveLineSegmentLength = new List<float>();

        bool ConnectedEnds { get; set; } = false;
        float DefaultWeight { get; set; } = 2.0f;
        public List<LineSegment> GetCurveLineSegments {   get { return curveLineSegments; } }
        public List<LineSegment> GetCurveLineUniformedSegments {   get { return curveUniformedLineSegments; }}

        public void SetWayPoints(Vector3[] controlPoints, int segmentCount, bool connectedEnds)
        {
            cp = new Vector4[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector4(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z, 1.0f);
            SetCommon(controlPoints.Length, segmentCount, connectedEnds);
        }
        public void SetWayPoints(Vector4[] controlPoints, int segmentCount, bool connectedEnds)
        {
            cp = new Vector4[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector4(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z, controlPoints[i].W);
            SetCommon(controlPoints.Length, segmentCount, connectedEnds);
        }

        private void SetCommon(int controlPointsLen, int segmentCount, bool connectedEnds)
        {
            order = 2;
            ConnectedEnds = connectedEnds;
            if (ConnectedEnds)
            {
                curveSegmentsEndIndex = segmentCount;
                curveLineSegmentsLength = segmentCount +1;  //controlPointsLen;
                cpEndIndex = controlPointsLen; // we can do this provided we Ensure Wrapped Index adustments occur per index.
            }
            else
            {
                curveSegmentsEndIndex = segmentCount - 1;
                curveLineSegmentsLength = segmentCount;  //controlPointsLen;
                cpEndIndex = controlPointsLen - 1;
            }
            BuildCurve();
        }

        public Vector3 GetPointOnCurveAtTime(float timeOnCurve)
        {
            return CalculatePointOnCurveAtTime(timeOnCurve);
        }

        public void BuildCurve()
        {
            Vector3[] curve = new Vector3[curveLineSegmentsLength];
            for (int i = 0; i < curveLineSegmentsLength; i++)
            {
                float timeOnLine = (float)(i) / (float)(curveSegmentsEndIndex);
                curve[i] = CalculatePointOnCurveAtTime(timeOnLine);
            }
            // calculate segment lengths
            for (int i = 0; i < curveSegmentsEndIndex; i++)
            {
                var dist = Vector3.Distance(curve[i], curve[i + 1]);
                segmentsSummedDistance += dist;
                curveLineSegmentLength.Add(dist);
            }
            // set the vertexs
            for (int i = 0; i < curveSegmentsEndIndex; i++)
            {
                curveLineSegments.Add(new LineSegment(curve[i], curve[i + 1]));
            }
        }

        private Vector3 CalculatePointOnCurveAtTime(float interpolationAmountOnCurve)
        {
            //int order = 2;
            //int segLei = curveLineLodCount - 1;
            //int cpLei = cp.Length - 1;
            float i = interpolationAmountOnCurve * (curveSegmentsEndIndex);

            // calculate curvature moments on the line.
            float t = (float)(i) / (float)((float)(curveSegmentsEndIndex) + 0.00001f);
            float cpit = (float)(cpEndIndex) * t; // cp index time.
            float cpt = Frac(cpit); // cp fractional time
            int cpi = (int)(cpit); // cp primary index.

            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpi - 1);
            int index1 = EnsureIndexInRange(cpi + 0);
            int index2 = EnsureIndexInRange(cpi + 1);
            int index3 = EnsureIndexInRange(cpi + 2);

            Vector3 plot = new Vector3();
            if ((cpi <= (cpEndIndex - order) && cpi >= 1) || ConnectedEnds) // middle
                plot = ToVector3(CalculateInnerCurvePoint(cp[index0], cp[index1], cp[index2], cp[index3], cpt));
            else
            {
                if (cpi < 1) // begining
                    plot = ToVector3(CalculateBeginingCurvePoint(cp[index1], cp[index2], cp[index3], cpt));
                else // if (cpi > (cpLei - order)) // end
                    plot = ToVector3(CalculateEndingCurvePoint(cp[index0], cp[index1], cp[index2], cpt));
            }
            return plot;
        }

        public int EnsureIndexInRange(int i)
        {
            while (i < 0)
                i = (cp.Length) + i;
            while (i > (cp.Length - 1))
                i = i - (cp.Length);
            return i;
        }

        Vector4 CalculateBeginingCurvePoint(Vector4 a0, Vector4 a1, Vector4 a2, float time)
        {
            return GetPointAtTimeOn2ndDegreePolynominalCurve(a0, a1, a2, (float)(time * .5f));
        }
        // ending segment
        Vector4 CalculateEndingCurvePoint(Vector4 a0, Vector4 a1, Vector4 a2, float time)
        {
            return GetPointAtTimeOn2ndDegreePolynominalCurve(a0, a1, a2, (float)(time * .5f + .5f));
        }
        // middle segments
        Vector4 CalculateInnerCurvePoint(Vector4 a0, Vector4 a1, Vector4 a2, Vector4 a3, float time)
        {
            Vector4 b0 = a1; Vector4 b1 = a2; Vector4 b2 = a3;
            Vector4 a = GetPointAtTimeOn2ndDegreePolynominalCurve(a0, a1, a2, (float)(time * .5f + .5f));
            Vector4 b = GetPointAtTimeOn2ndDegreePolynominalCurve(b0, b1, b2, (float)(time * .5f));
            return (a * (1f - time) + b * time);
        }

        /* primary calculations */

        /// <summary>
        /// This is a specialized imbalanced function based on a 2nd degree polynominal function.
        /// </summary>
        Vector4 GetPointAtTimeOn2ndDegreePolynominalCurve(Vector4 A, Vector4 B, Vector4 C, float t)
        {
            //Calculate Artificial Spline Point
            var S = (((B - C) + B) + ((B - A) + B)) * .5f;
            //var S = CalculateProportionalArtificialSplinePoint(A, B, C); // original
            //var S = CalculateNormalizedArtificialSplinePoint(A, B, C);
            float i = 1.0f - t;
            Vector4 plot = A * (i * i) + S * 2f * (i * t) + C * (t * t);
            // linear
            Vector4 plot2 = new Vector4();
            if (t <= .5f)
                plot2 = A + (B - A) * (t * 2f);
            else
                plot2 = B + (C - B) * ((t - .5f) * 2f);
            // below 1 the curve begins to straighten.
            plot.W = DefaultWeight;
            Vector4 finalPlot = (plot * (plot2.W)) + (plot2 * (1f - plot2.W));
            return finalPlot;
        }

        //Vector4 CalculateProportionalArtificialSplinePoint(Vector4 A, Vector4 B, Vector4 C)
        //{
        //    return (((B - C) + B) + ((B - A) + B)) * .5f;
        //}

        //// testing method
        //Vector4 CalculateNormalizedArtificialSplinePoint(Vector4 A, Vector4 B, Vector4 C)
        //{
        //    Vector3 p0 = new Vector3(A.X, A.Y, A.Z);
        //    Vector3 p1 = new Vector3(B.X, B.Y, B.Z);
        //    Vector3 p2 = new Vector3(C.X, C.Y, C.Z);
        //    //
        //    Vector3 invtemp0 = (p1 - p2);// + p1;
        //    Vector3 invtemp2 = (p1 - p0);// + p1;
        //    float invdist0 = invtemp0.Length();
        //    float invdist2 = invtemp2.Length();
        //    float avgdist = (invdist0 + invdist2) * .5f;
        //    invtemp0.Normalize();
        //    invtemp2.Normalize();
        //    //
        //    invtemp0 = invtemp0 * (avgdist);
        //    invtemp2 = invtemp2 * (avgdist);
        //    Vector3 g = (invtemp0 + invtemp2) * .5f + p1;
        //    return new Vector4(g.X, g.Y, g.Z, B.W);
        //}

        float Interpolate(float v0, float v1, float timeX)
        {
            return ((v1 - v0) * timeX + v0);
        }
        float Frac(float n)
        {
            var i = (int)(n);
            return n - (float)(i);
        }
        public Vector3 ToVector3(Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public class LineSegment
        {
            public Vector3 Start { get; set; }
            public Vector3 End { get; set; }
            public LineSegment(Vector3 start, Vector3 end)
            {
                Start = start;
                End = end;
            }
        }
    }
}
