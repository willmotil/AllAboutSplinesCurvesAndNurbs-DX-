using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class CurveBezierSpline
    {
        // https://www.youtube.com/watch?v=tXrkNfHFakg  matrix thru points.
        // https://www.youtube.com/watch?v=sZITw1X2ctU si(x) = Ai + Bi(X-Xi)+Ci(X - Xi)^2 + Di(X- Xi)^3;
        // Cubic  https://www.youtube.com/watch?v=pnYccz1Ha34
        // i = (1-t);
        // C0(t) = i^3 * P0 + 3*i^2*t * P1+ 3i*t^2* P2 +t^3*P3
        // Thru the control points https://www.youtube.com/watch?v=2hL1LGMVnVM

        // https://www.youtube.com/watch?v=a55tOCUy9oI ok from this video at 1:35 i got a inspiration.
        // i need to generate interior artificial splines for a closed bezier based on any points surround segmental tangent to the accounted point and then the following the same
        // in order to generate the tangental artificial points for the interior 2 points of a sampled bezer  essentially a bezier will need to generate 6 points to get the g2 curvature between 
        // the original points p1 p2    of a 4 point bezier this means only a segment of the bezier is validly plotted and the entire bezier is closed in this context.
        // so for a closed 4 point bezier a additional 8 points are generated. All of them representing the interior knots between the original points.
        // that should give g2 continuity once the artificial position is determined properly.

        Vector3[] cp;
        Vector3[] curveLinePoints;
        public int _numOfVisualCurvatureSegmentPoints = 400;
        // Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        private float _weight = 0.5f;


        public CurveBezierSpline(Vector3[] controlPoints, bool useWikiVersion)
        {
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public CurveBezierSpline(Vector3[] controlPoints, bool useWikiVersion, float weight)
        {
            _weight = weight;
            CreateSpline(controlPoints, useWikiVersion);
        }

        /// <summary>
        /// Weight is the Parametric constant: 0.0 for the uniform spline, 0.5 for the centripetal spline, 1.0 for the chordal spline
        /// </summary>
        public CurveBezierSpline(Vector3[] controlPoints, bool useWikiVersion, float weight, int numOfVisualCurvatureSegmentPoints)
        {
            _weight = weight;
            _numOfVisualCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateSpline(controlPoints, useWikiVersion);
        }



        #region  


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
                    curveLinePoints[i] = GetSplinePoint(t);
                }
            }

        }

        private Vector3 GetSplinePoint(float Time)
        {
            var offset = cp.Length * Time;
            var index = (int)(offset);
            var fracTime = offset - (float)index;
            return GetSplinePoint(index, fracTime);
        }

        private Vector3 GetSplinePoint(int cpIndex, float fracTime)
        {
            // caluclate conditional cp indexs at the moments
            int index0 = EnsureIndexInRange(cpIndex - 1);
            int index1 = EnsureIndexInRange(cpIndex + 0);
            int index2 = EnsureIndexInRange(cpIndex + 1);
            int index3 = EnsureIndexInRange(cpIndex + 2);
            return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
        }

        /*
         public static Vector2 GetPointOnBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;

            Vector2 result =
                (u3) * p0 +
                (3f * u2 * t) * p1 +
                (3f * u * t2) * p2 +
                (t3) * p3;

            return result;
        }

        quadraticBezier: function(p0, p1, p2, t, pFinal) 
        {
		pFinal = pFinal || {};
		pFinal.x = Math.pow(1 - t, 2) * p0.x + 
				   (1 - t) * 2 * t * p1.x + 
				   t * t * p2.x;
		pFinal.y = Math.pow(1 - t, 2) * p0.y + 
				   (1 - t) * 2 * t * p1.y + 
				   t * t * p2.y;
		return pFinal;
	   },

        cubicBezier: function(p0, p1, p2, p3, t, pFinal) 
        {
		pFinal = pFinal || {};
		pFinal.x = Math.pow(1 - t, 3) * p0.x + 
				   Math.pow(1 - t, 2) * 3 * t * p1.x + 
				   (1 - t) * 3 * t * t * p2.x + 
				   t * t * t * p3.x;
		pFinal.y = Math.pow(1 - t, 3) * p0.y + 
				   Math.pow(1 - t, 2) * 3 * t * p1.y + 
				   (1 - t) * 3 * t * t * p2.y + 
				   t * t * t * p3.y;
		return pFinal;
	}

        multicurve: function(points, context) 
        {
		var p0, p1, midx, midy;

		context.moveTo(points[0].x, points[0].y);

		for(var i = 1; i < points.length - 2; i += 1) {
			p0 = points[i];
			p1 = points[i + 1];
			midx = (p0.x + p1.x) / 2;
			midy = (p0.y + p1.y) / 2;
			context.quadraticCurveTo(p0.x, p0.y, midx, midy);
		}
		p0 = points[points.length - 2];
		p1 = points[points.length - 1];
		context.quadraticCurveTo(p0.x, p0.y, p1.x, p1.y);
	}


            //Vector3 result = 
            //    MathF.Pow(i, 3) * p0 +
            //    MathF.Pow(i, 2) * 3 * t * p1 +
            //    i * 3 * t * t * p2 +
            //    t * t * t * p3;
            //return result;

         */

        /// <summary>
        /// Were time is now in this context is 0 to 1
        /// </summary>
        /// <returns></returns>
        public Vector3 WeightedPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float time)
        {
            Vector3 p0 = v0; Vector3 p1 = v1; Vector3 p2 = v2; Vector3 p3 = v3;

            float weight = 1.41f;
            float offsetTangentalPosition = .66f;

            var n = (v2 - v0) * offsetTangentalPosition + v0;
            var n1 = v1 - n;
            p1 = v1 + n1 * weight;

            n = (v1 - v3) * offsetTangentalPosition + v3; 
            var n2 = v2 - n;
            p2 = v2 + n2 * weight;

            float t = time;
            float i = 1f - t;

            float u = 1f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;
            Vector3 result =
                u3 * p0 +
                (3f * u2 * t) * p1 +
                (3f * u * t2) * p2 +
                t3 * p3;
            return result;
        }

        ///// <summary>
        ///// The weight value used in this function is typically from 0 to 1
        ///// </summary>
        //private float GetT(float t, Vector3 p0, Vector3 p1)
        //{
        //    // https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
        //    float a = MathF.Pow((p1.X - p0.X), 2.0f) + MathF.Pow((p1.Y - p0.Y), 2.0f) + MathF.Pow((p1.Z - p0.Z), 2.0f);
        //    float b = System.MathF.Pow(a, _weight * 0.5f);
        //    return (b + t);
        //}

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
