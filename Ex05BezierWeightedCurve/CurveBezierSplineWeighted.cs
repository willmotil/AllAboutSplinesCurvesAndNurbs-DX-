using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{

    // This one will take vector4s and they will have the weights added to make sharp edges and stuff.


    public class CurveBezierSplineWeighted
    {
        Vector3[] cp;
        float[] cpWeight;
        Vector3[] curveLinePoints;
        public bool _closedControlPoints = true;
        public int _numOfCurvatureSegmentPoints = 100;
        private float _globalWeight = 1.0f;
        // for visual display only.
        List<Vector3> artificialCpLine = new List<Vector3>();
        List<Vector3> artificialTangentLine = new List<Vector3>();

        int index0 = 0;
        int index1 = 0;
        int index2 = 0;
        int index3 = 0;

        /// <summary>
        /// </summary>
        public CurveBezierSplineWeighted(Vector4[] controlPoints, bool closedControlPoints)
        {
            _closedControlPoints = closedControlPoints;
            CreateSpline(controlPoints);
        }

        /// <summary>
        /// </summary>
        public CurveBezierSplineWeighted(Vector4[] controlPoints, float globalWeight, bool closedControlPoints)
        {
            _closedControlPoints = closedControlPoints;
            _globalWeight = globalWeight;
            CreateSpline(controlPoints);
        }

        /// <summary>
        /// </summary>
        public CurveBezierSplineWeighted(Vector4[] controlPoints, float globalWeight, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints)
        {
            _closedControlPoints = closedControlPoints;
            _globalWeight = globalWeight;
            _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateSpline(controlPoints);
        }



        #region  

        private void CreateSpline(Vector4[] controlPoints)
        {

            artificialCpLine.Clear();
            artificialTangentLine.Clear();
            cp = new Vector3[controlPoints.Length];
            cpWeight = new float[controlPoints.Length];
            for (int i = 0; i < controlPoints.Length; i++)
            {
                cp[i] = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);
                cpWeight[i] = controlPoints[i].W;
            }

            curveLinePoints = new Vector3[_numOfCurvatureSegmentPoints];

            var loopCount = _numOfCurvatureSegmentPoints;
            var divisor = loopCount - 1;

            for (int i = 0; i < loopCount; i++)
            {
                float t = (float)(i) / (float)(divisor);
                curveLinePoints[i] = GetSplinePoint(t);
            }
        }

        private Vector3 GetSplinePoint(float Time)
        {
            if (_closedControlPoints)
            {
                var plotRange = cp.Length;
                var offset = plotRange * Time;
                var index = (int)(offset);
                var fracTime = offset - (float)index;
                return DetermineSplines(index, fracTime);
            }
            else
            {
                var plotRange = cp.Length -1;
                var offset = plotRange * Time;
                var index = (int)(offset);
                var fracTime = offset - (float)index;
                return DetermineSplines(index, fracTime);
            }
        }

        private Vector3 DetermineSplines(int cpIndex, float fracTime)
        {
            if (_closedControlPoints || (cpIndex > 0 && cpIndex < cp.Length - 2) )
            {
                // caluclate conditional cp indexs at the moments
                index0 = EnsureIndexInRange(cpIndex - 1);
                index1 = EnsureIndexInRange(cpIndex + 0);
                index2 = EnsureIndexInRange(cpIndex + 1);
                index3 = EnsureIndexInRange(cpIndex + 2);
                //return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
            }
            else
            {
                if (cpIndex == 0)
                {
                    index0 = EnsureIndexInRange(cpIndex + 0);
                    index1 = EnsureIndexInRange(cpIndex + 0);
                    index2 = EnsureIndexInRange(cpIndex + 1);
                    index3 = EnsureIndexInRange(cpIndex + 2);
                    //return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
                }
                if (cpIndex >= cp.Length - 2)
                {
                    index0 = EnsureIndexInRange(cpIndex - 1);
                    index1 = EnsureIndexInRange(cpIndex + 0); // <<
                    index2 = EnsureIndexInRange(cpIndex + 1);
                    index3 = EnsureIndexInRange(cpIndex + 1);
                    //return WeightedPoint(cp[index0], cp[index1], cp[index2], cp[index3], fracTime);
                }
            }
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

            var segmentDistance = Vector3.Distance(v2, v1) * 0.35355339f; // * _weight;

            var n1 = Vector3.Normalize(GetIdealTangentVector(v0, v1, v2));
            p1 = v1 + n1 * segmentDistance * cpWeight[index1];
            p0 = v1;

            var n2 = Vector3.Normalize(GetIdealTangentVector(v3, v2, v1));
            p2 = v2 + n2 * segmentDistance * cpWeight[index2];
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
            float ratioa = disa / (disa + disc);
            var pAB = ((b - a) * ratioa) + a;
            var pBC = ((c - b) * ratioa) + b;
            var result = pBC - pAB;
            // prevent nan later on.
            if (result == Vector3.Zero)
                result = c - a;

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
                _spriteBatch.DrawString(font, $" CP[{i}] \n w: {cpWeight[i].ToString("0.000")}", new Vector2(cp[i].X, cp[i].Y), Color.Black);
            }
        }

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, GameTime gameTime)
        {

            bool flip = false;
            int lineThickness = 2;
            for (int i = 0; i < curveLinePoints.Length - 1; i++)
            {
                if (flip)
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), lineThickness, Color.Green);
                else
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), lineThickness, Color.Black);

                if (i < 2)
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[i]), ToVector2(curveLinePoints[i + 1]), lineThickness, Color.Yellow);

                flip = !flip;
            }

            for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
            {
                DrawHelpers.DrawBasicLine(new Vector2(artificialCpLine[i].X, artificialCpLine[i].Y), new Vector2(artificialCpLine[i + 1].X, artificialCpLine[i + 1].Y), lineThickness, Color.Purple);
            }

            for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
            {
                DrawHelpers.DrawBasicLine(new Vector2(artificialTangentLine[i].X, artificialTangentLine[i].Y), new Vector2(artificialTangentLine[i + 1].X, artificialTangentLine[i + 1].Y), lineThickness, Color.Pink);
            }

            for (int i = 0; i < cp.Length; i++)
            {
                DrawHelpers.DrawBasicPoint(new Vector2(cp[i].X, cp[i].Y), 4, Color.Red);
            }
        }

        public Vector3 MidPoint(Vector3 a, Vector3 b)
        {
            return (a + b) / 2;
        }
        public Vector3 MidPoint(Vector3 a, Vector3 b, Vector3 c)
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
