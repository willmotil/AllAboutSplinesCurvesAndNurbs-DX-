

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class CurveHermiteSpline
    {


        Vector3[] cp;
        Vector3[] curveLinePoints;
        public int _numOfVisualCurvatureSegmentPoints = 80;
        // Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        private float _weight = 0.5f;
        List<Vector3> artificialCpLine = new List<Vector3>();

        public CurveHermiteSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public CurveHermiteSpline(Vector3[] controlPoints, bool useWikiVersion, float weight)
        {
            _weight = weight;
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public CurveHermiteSpline(Vector3[] controlPoints, bool useWikiVersion, float weight, int numOfVisualCurvatureSegmentPoints)
        {
            _weight = weight;
            _numOfVisualCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateSpline(controlPoints, useWikiVersion);
        }



        #region  


        private void CreateSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            artificialCpLine.Clear();
            cp = new Vector3[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
                cp[i] = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);

            curveLinePoints = new Vector3[_numOfVisualCurvatureSegmentPoints];

            var loopCount = _numOfVisualCurvatureSegmentPoints;
            var divisor = loopCount -1;

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

        /*
        Hermite basis functions
        h1(t) = 2t^3 - 3t^2 +1
        h2(t) = -2t^3 +3t^2
        h3(t) = t^3 - 2t^2 + t
        h4(t) = t^3 - t^2

        each interval uses the 4 basis functions like so 
        m1 h3(t) + y1 h1(t) + y2 h2(t) + m2 h4(t)
        were m1 and m2 are the tangent values

        https://codeplea.com/public/content/splines_hermite_basis.png


         */

        public Vector3 WeightedPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            // for points p1 p2  we need to find the tangents of them using p0 p3 as well.
            var t1 = (p2 - p0) * _weight;  // when weight is about .707 should get circular continuity.
            var t2 = (p3 - p1) * _weight;

            artificialCpLine.Add(t1 + p1);
            artificialCpLine.Add( p1);
            artificialCpLine.Add(t2 + p2);
            artificialCpLine.Add(p2);

            return Vector3.Hermite(p1, t1, p2, t2, time); //just used monogames since it's already there.
        }

        //public Vector3 WeightedPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        //{
        //    float invtime = 1f - time;
        //    // for points p1 p2  we need to find the tangents of them using p0 p3 as well.
        //    var n1 = (p2 - p0);
        //    var n2 = (p3 - p1);

        //    n1.Normalize();
        //    n2.Normalize();

        //    var cos = .99f - Vector3.Dot(n1, n2);
        //    var hsqrt = (float)Math.Sqrt(cos / 2f);

        //    //var w1 = MathHelper.Lerp(hsqrt, _weight, time);
        //    //var w2 = MathHelper.Lerp(hsqrt, _weight, invtime);

        //    var w1 = hsqrt * _weight;
        //    var w2 = hsqrt * _weight;

        //    var t1 = (p2 - p0) * w1;
        //    var t2 = (p3 - p1) * w2;

        //    return Vector3.Hermite(p1, t1, p2, t2, time);
        //}



        /*
        public Vector3 WeightedPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            // for points p1 p2  we need to find the tangents of them using p0 p3 as well.
            var t1 = (p2 - p0) * _weight;  // when weight is about .707 should get circular continuity.
            var t2 = (p3 - p1) * _weight;

            return Vector3.Hermite(p1, t1, p2, t2, time); //just used monogames since it's already there.
        } 
        
        public Vector3 WeightedPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float time)
        {
            if(time > 1f || time < 0f)
            {
                throw new Exception("Time out of range");
            }

            Vector3 p0 = v0; Vector3 p1 = v1; Vector3 p2 = v2; Vector3 p3 = v3;

            //var c = (v0 + v1 + v2 + v3) / 4;
            //var t1 = v1 - c;
            //var t2 = v2 - c;

            var n1 = p1 - p0;
            var n2 = p3 - p2;

            var t1 = Vector3.Normalize(n1) + n1 * _weight;
            var t2 = Vector3.Normalize(n2) + n2 * _weight;

            return Vector3.Hermite(p1, t1, p2, t2, time);

            // for points p1 p2  we need to find the tangents of them using p0 p3 as well.

        }
         */

        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, SpriteFont font, GameTime gameTime)
        {
            DrawWithSpriteBatch(_spriteBatch, gameTime);

            for (int i = 0; i < cp.Length; i++)
            {
                _spriteBatch.DrawString(font, $" CP[{i}]" , new Vector2(cp[i].X, cp[i].Y), Color.Black);
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

                if(i <3 )
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), 1, Color.Yellow);

                flip = !flip;
            }

            for (int i = 0; i < artificialCpLine.Count - 1; i+=2)
            {
                DrawHelpers.DrawBasicLine(new Vector2(artificialCpLine[i].X, artificialCpLine[i].Y), new Vector2(artificialCpLine[i + 1].X, artificialCpLine[i + 1].Y), 1, Color.Purple);
            }

            for (int i = 0; i < cp.Length; i++)
            {
                DrawHelpers.DrawBasicPoint(new Vector2(cp[i].X, cp[i].Y), Color.Red);
            }
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
