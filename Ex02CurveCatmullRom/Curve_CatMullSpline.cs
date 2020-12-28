using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Curve_CatMullSpline
    {

        Vector3[] cp;
        Vector3[] curveLinePoints;
        public int _numOfVisualCurvatureSegmentPoints = 40;
        // Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        private float _weight = 0.5f;


        public Curve_CatMullSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public Curve_CatMullSpline(Vector3[] controlPoints, bool useWikiVersion, float weight)
        {
            _weight = weight;
            CreateSpline( controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public Curve_CatMullSpline(Vector3[] controlPoints, bool useWikiVersion, float weight, int numOfVisualCurvatureSegmentPoints)
        {
            _weight = weight;
            _numOfVisualCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateSpline(controlPoints, useWikiVersion);
        }



        #region  Wiki version modifyed for moments. https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline


        private void CreateSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            cp = new Vector3[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);

            curveLinePoints = new Vector3[_numOfVisualCurvatureSegmentPoints];
            if (useWikiVersion)
            {
                for (int i = 0; i < _numOfVisualCurvatureSegmentPoints; i++)
                {
                    float t = (float)(i) / (float)(_numOfVisualCurvatureSegmentPoints - 1);
                    curveLinePoints[i] = GetPoint(t);
                }
            }
            else
            {
                for (int i = 0; i < _numOfVisualCurvatureSegmentPoints; i++)
                {
                    float t = (float)(i) / (float)(_numOfVisualCurvatureSegmentPoints - 1);
                    curveLinePoints[i] = GetAltPoint(t);
                }
            }
        }

        private Vector3 GetPoint(float Time)
        {
            var offset = cp.Length * Time;
            var index = (int)(offset);
            var fracTime = offset - (float)index;
            return GetPoint(index, fracTime);
        }

        private Vector3 GetPoint(int cpIndex, float fracTime)
        {
            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpIndex - 1);
            int index1 = EnsureIndexInRange(cpIndex + 0);
            int index2 = EnsureIndexInRange(cpIndex + 1);
            int index3 = EnsureIndexInRange(cpIndex + 2);
            return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
        }

        /// <summary>
        /// Were time is now in this context is 0 to 1
        /// </summary>
        /// <returns></returns>
        public Vector3 WeightedPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            float t0 = 0f;
            float t1 = GetT(t0, p0, p1);
            float t2 = GetT(t1, p1, p2);
            float t3 = GetT(t2, p2, p3);
            float t = t1;
            t = (t2 - t1) * time + t1;
            Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;
            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;
            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;
            return C;
        }

        /// <summary>
        /// The weight value used in this function is typically from 0 to 1
        /// </summary>
        private float GetT(float t, Vector3 p0, Vector3 p1)
        {
            // https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
            float a = MathF.Pow((p1.X - p0.X), 2.0f) + MathF.Pow((p1.Y - p0.Y), 2.0f) + MathF.Pow((p1.Z - p0.Z), 2.0f);
            float b = System.MathF.Pow(a, _weight * 0.5f);
            return (b + t);
        }

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region guys version  https://www.youtube.com/watch?v=9_aJGUTePYo

        //public CurveCatMullSpline(Vector3[] controlPoints)
        //{
        //    cp = new Vector3[controlPoints.Length];
        //    for (int i = 0; i < controlPoints.Length; i++)
        //        cp[i] = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);

        //    curveLinePoints = new Vector3[_numOfVisualCurvatureSegmentPoints];
        //    for (int i = 0; i < _numOfVisualCurvatureSegmentPoints; i++)
        //    {
        //        float t = (float)(i) / (float)(_numOfVisualCurvatureSegmentPoints - 1);
        //        curveLinePoints[i] = GetCatMullRomPoint(t);
        //    }
        //}

        private Vector3 GetAltPoint(float Time)
        {
            var offset = cp.Length * Time;
            var index = (int)(offset);
            var fracTime = offset - (float)index;
            return GetAltPoint(index, fracTime);
        }

        private Vector3 GetAltPoint(int cpIndex, float fracTime)
        {
            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpIndex - 1);
            int index1 = EnsureIndexInRange(cpIndex + 0);
            int index2 = EnsureIndexInRange(cpIndex + 1);
            int index3 = EnsureIndexInRange(cpIndex + 2);
            return GetAltPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
        }

        private Vector3 GetAltPoint(Vector3 a0, Vector3 a1, Vector3 a2, Vector3 a3, float time)
        {
            float t = time;
            float tt = t * t;
            float ttt = tt * t;

            float q1 = -ttt + 2f * tt - t;
            float q2 = 3f * ttt - 5f * tt + 2f;
            float q3 = -3f * ttt + 4f * tt + t;
            float q4 = ttt - tt;

            var v = (a0 * q1 + a1 * q2 + a2 * q3 + a3 * q4) * 0.5f;

            return v;
        }

        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, GameTime gameTime)
        {
            _spriteBatch.Begin();

            bool flip = false;
            for (int i = 0; i < curveLinePoints.Length - 1; i++)
            {
                if (flip)
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Green);
                else
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Black);
                flip = !flip;
            }

            for (int i = 0; i < cp.Length; i++)
            {
                DrawHelpers.DrawBasicPoint(new Vector2(cp[i].X, cp[i].Y), Color.Red);
            }

            //_spriteBatch.DrawString(_font, " time " + t, new Vector2(10, 10), Color.Black);

            _spriteBatch.End();
        }

        public int EnsureIndexInRange(int i)
        {
            while (i < 0)
                i = i + (cp.Length);
            while (i > (cp.Length - 1))
                i = i - (cp.Length);
            return i;
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
