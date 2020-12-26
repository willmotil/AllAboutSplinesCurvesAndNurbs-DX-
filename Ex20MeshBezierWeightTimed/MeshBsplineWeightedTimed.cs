using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{

    /// <summary>
    ///  This version will add more functionality to the curve in order to allow it to use uniformly spaced segments this will increase the runtime computation slightly of getting a point on the curve. 
    ///  However this will allow for a smooth uniform distribution of points along the curve so that the speed of traversal is constant about it.
    ///  This is also important for uv calculations to smoothly map texture positions upon meshes that we will later generate from these curves in another example.
    ///  So this example is all about uniformity which is a important quality that doesn't come for free from this type of piecewise spline... well have to do more work to get it.
    /// </summary>
    public class MeshBsplineWeightedTimed
    {

        BezierSplineWeightedTimedForMesh curve;

        /// <summary>
        /// when set to true or closed this will loop the last point to curve round to the first and the curve will be a loop.
        /// when set to false the end points will be doubled in the algorithm to calculate the clamping for the first and last curve ending segments.
        /// </summary>
        public bool _closedControlPoints = true;
        /// <summary>
        /// when uniformed is true the resulting curve will be made with uniformly spaced positions and the timed traversal rate across the curve will be smooth.
        /// </summary>
        public bool _uniformedCurve = true;
        /// <summary>
        /// the number of timed points generated along the entire curve
        /// </summary>
        public int _numOfCurvatureSegmentPoints = 100;
        /// <summary>
        /// the higher this number the more refined the timing along the curve will be and the more time it will take to calculate it.
        /// </summary>
        public int _numberOfIntigrationStepsPerSegment = 100;

        #region constructors

        /// <summary>
        /// </summary>
        public MeshBsplineWeightedTimed(Vector4[] controlPoints, int width, int height)
        {
            CreateMesh(controlPoints, width, height);
        }

        /// <summary>
        /// </summary>
        public MeshBsplineWeightedTimed(Vector4[] controlPoints, int width, int height, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve)
        {
            _closedControlPoints = closedControlPoints;
            _uniformedCurve = uniformedCurve;
            _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateMesh(controlPoints, width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        public MeshBsplineWeightedTimed(Vector4[] controlPoints, int width, int height, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve, float globalWeight, bool showTangents)
        {
            _closedControlPoints = closedControlPoints;
            _uniformedCurve = uniformedCurve;
            _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateMesh(controlPoints, width, height);
        }

        #endregion

        #region curve calculation methods

        private void CreateMesh(Vector4[] controlPoints, int width, int height)
        {

            //CreateSpline
        }

        #endregion


        /// <summary>
        ///  This version will add more functionality to the curve in order to allow it to use uniformly spaced segments this will increase the runtime computation slightly of getting a point on the curve. 
        ///  However this will allow for a smooth uniform distribution of points along the curve so that the speed of traversal is constant about it.
        ///  This is also important for uv calculations to smoothly map texture positions upon meshes that we will later generate from these curves in another example.
        ///  So this example is all about uniformity which is a important quality that doesn't come for free from this type of piecewise spline... well have to do more work to get it.
        /// </summary>
        public class BezierSplineWeightedTimedForMesh
        {
            public bool _showTangents = false;

            #region  non requisite optional astetic visual values.
            List<Vector3> artificialCpLine = new List<Vector3>();
            List<Vector3> artificialTangentLine = new List<Vector3>();
            #endregion

            #region  temporary tracking values used thruout many methods as the curves are processed.
            int currentCpIndex = 0;
            int index0 = 0;
            int index1 = 0;
            int index2 = 0;
            int index3 = 0;
            #endregion


            /// <summary>
            /// This holds the information relating to the control points given or that will be calculated.
            /// </summary>
            ControlPoint[] cps;
            /// <summary>
            /// these are the reslting generated NonUniform timed or Uniformly timed line points.
            /// </summary>
            Vector3[] curveLinePoints;
            /// <summary>
            /// when set to true or closed this will loop the last point to curve round to the first and the curve will be a loop.
            /// when set to false the end points will be doubled in the algorithm to calculate the clamping for the first and last curve ending segments.
            /// </summary>
            public bool _closedControlPoints = true;
            /// <summary>
            /// when uniformed is true the resulting curve will be made with uniformly spaced positions and the timed traversal rate across the curve will be smooth.
            /// </summary>
            public bool _uniformedCurve = true;
            /// <summary>
            /// the number of timed points generated along the entire curve
            /// </summary>
            public int _numOfCurvatureSegmentPoints = 100;
            /// <summary>
            /// the higher this number the more refined the timing along the curve will be and the more time it will take to calculate it.
            /// </summary>
            public int _numberOfIntigrationStepsPerSegment = 100;
            /// <summary>
            /// Not yet implemented ... this value acts as a scalar on all weights.
            /// </summary>
            private float _globalWeight = 1.0f;

            float totaldist = 0;

            /// <summary>
            /// the calculated total integrated distance of the curve.
            /// </summary>
            public float TotalCurveDistance { get { return totaldist; } }



            #region constructors

            /// <summary>
            /// </summary>
            public BezierSplineWeightedTimedForMesh(Vector4[] controlPoints)
            {
                CreateSpline(controlPoints);
            }

            /// <summary>
            /// </summary>
            public BezierSplineWeightedTimedForMesh(Vector4[] controlPoints, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve)
            {
                _closedControlPoints = closedControlPoints;
                _uniformedCurve = uniformedCurve;
                _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
                CreateSpline(controlPoints);
            }

            /// <summary>
            /// 
            /// </summary>
            public BezierSplineWeightedTimedForMesh(Vector4[] controlPoints, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve, float globalWeight, bool showTangents)
            {
                _closedControlPoints = closedControlPoints;
                _uniformedCurve = uniformedCurve;
                _globalWeight = globalWeight;
                _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
                _showTangents = showTangents;
                CreateSpline(controlPoints);
            }

            #endregion

            #region curve calculation methods

            private void CreateSpline(Vector4[] controlPoints)
            {
                artificialCpLine.Clear();
                artificialTangentLine.Clear();
                cps = new ControlPoint[controlPoints.Length];
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    var cpInstance = new ControlPoint();
                    cpInstance.position = new Vector3(controlPoints[i].X, controlPoints[i].Y, controlPoints[i].Z);
                    cpInstance.weight = controlPoints[i].W;
                    cpInstance.cpIndex = i;
                    cps[i] = cpInstance;
                }

                FindCpLengthsAndIntegratedTotalCurveLength();

                curveLinePoints = new Vector3[_numOfCurvatureSegmentPoints];

                var loopCount = _numOfCurvatureSegmentPoints;
                var divisor = loopCount - 1;

                // Create the curve either uniformed or non uniformed.
                if (_uniformedCurve)
                {
                    for (int i = 0; i < loopCount; i++)
                    {
                        float t = (float)(i) / (float)(divisor);
                        curveLinePoints[i] = GetUniformSplinePoint(t);
                    }
                }
                else
                {
                    for (int i = 0; i < loopCount; i++)
                    {
                        float t = (float)(i) / (float)(divisor);
                        curveLinePoints[i] = GetNonUniformSplinePoint(t);
                    }
                }
            }

            public void FindCpLengthsAndIntegratedTotalCurveLength()
            {
                var loopCount = cps.Length;
                var divisor = loopCount - 1;

                var lastPos = cps[0].position;
                totaldist = 0;
                float prevTotalDist = 0;

                var integrateStepAmount = 1f / _numberOfIntigrationStepsPerSegment;
                for (int cptomeasure = 0; cptomeasure < loopCount; cptomeasure++)
                {
                    float cpToNextCpDistance = 0;
                    for (float time = 0f; time < integrateStepAmount + 000001f; time += integrateStepAmount)
                    {
                        var nowPosition = DetermineSplines(cptomeasure, time);
                        if (time > 0f)
                        {
                            var dist = Vector3.Distance(nowPosition, lastPos);
                            if (nowPosition != lastPos)
                                cpToNextCpDistance += dist;
                        }
                        lastPos = nowPosition;
                    }

                    prevTotalDist = totaldist;
                    if (_closedControlPoints == false && cptomeasure == loopCount - 1)
                        cpToNextCpDistance = 0;
                    else
                        totaldist += cpToNextCpDistance;

                    cps[cptomeasure].startDistance = prevTotalDist;
                    cps[cptomeasure].distanceToNextCp = cpToNextCpDistance;
                }
            }

            public Vector3 GetUniformSplinePoint(float time)
            {
                int resultIndex = 0;
                float fracTime = 0;
                float currentDistance = time * totaldist;
                if (_closedControlPoints)
                {
                    int i = 0;
                    while (i < cps.Length)
                    {
                        var start = cps[i].startDistance;
                        var end = start + cps[i].distanceToNextCp;
                        if (currentDistance >= start && currentDistance <= end)
                        {
                            resultIndex = i;
                            var len = end - start;
                            fracTime = (currentDistance - start) / len;
                            if (fracTime > 1f)
                                fracTime = 1f;
                            i = cps.Length; // break
                        }
                        i++;
                    }
                }
                else
                {
                    int i = 0;
                    while (i < cps.Length)
                    {
                        var start = cps[i].startDistance;
                        var end = start + cps[i].distanceToNextCp;
                        if (currentDistance >= start && currentDistance <= end)
                        {
                            resultIndex = i;
                            var len = end - start;
                            fracTime = (currentDistance - start) / len;
                            if (fracTime > 1f)
                                fracTime = 1f;
                            i = cps.Length; // break
                        }
                        i++;
                    }
                }
                return DetermineSplines(resultIndex, fracTime);
            }

            public Vector3 GetNonUniformSplinePoint(float Time)
            {
                if (_closedControlPoints)
                {
                    var plotRange = cps.Length;
                    var offset = plotRange * Time;
                    var index = (int)(offset);
                    var fractionalTime = offset - (float)index;
                    return DetermineSplines(index, fractionalTime);
                }
                else
                {
                    var plotRange = cps.Length - 1;
                    var offset = plotRange * Time;
                    var index = (int)(offset);
                    var fractionalTime = offset - (float)index;
                    return DetermineSplines(index, fractionalTime);
                }
            }

            private Vector3 DetermineSplines(int cpIndex, float fracTime)
            {
                if (_closedControlPoints || (cpIndex > 0 && cpIndex < cps.Length - 2))
                {
                    // caluclate conditional cp indexs at the moments
                    index0 = EnsureIndexInRange(cpIndex - 1);
                    index1 = EnsureIndexInRange(cpIndex + 0);//<
                    index2 = EnsureIndexInRange(cpIndex + 1);
                    index3 = EnsureIndexInRange(cpIndex + 2);
                }
                else
                {
                    if (cpIndex == 0)
                    {
                        index0 = EnsureIndexInRange(cpIndex + 0);
                        index1 = EnsureIndexInRange(cpIndex + 0);//<<
                        index2 = EnsureIndexInRange(cpIndex + 1);
                        index3 = EnsureIndexInRange(cpIndex + 2);
                    }
                    if (cpIndex >= cps.Length - 2)
                    {
                        index0 = EnsureIndexInRange(cpIndex - 1);
                        index1 = EnsureIndexInRange(cpIndex + 0); // <<
                        index2 = EnsureIndexInRange(cpIndex + 1);
                        index3 = EnsureIndexInRange(cpIndex + 1);
                    }
                }
                currentCpIndex = index1;
                return WeightedPoint(cps[index0].position, cps[index1].position, cps[index2].position, cps[index3].position, fracTime);
            }

            public int EnsureIndexInRange(int i)
            {
                while (i < 0)
                    i = i + (cps.Length);
                while (i > (cps.Length - 1))
                    i = i - (cps.Length);
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
                p1 = v1 + n1 * segmentDistance * cps[index1].weight;
                p0 = v1;

                var n2 = Vector3.Normalize(GetIdealTangentVector(v3, v2, v1));
                p2 = v2 + n2 * segmentDistance * cps[index2].weight;
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

                for (int i = 0; i < cps.Length; i++)
                {
                    string msg =
                        $" CP[{i}]  w: {cps[i].weight.ToString("0.000")}" +
                        $"\n startDist: {cps[i].startDistance.ToString("###0.00")}" +
                        $"\n segDist: {cps[i].distanceToNextCp.ToString("###0.00")}"
                        ;
                    _spriteBatch.DrawString(font, msg, new Vector2(cps[i].position.X, cps[i].position.Y), Color.Black);
                }
            }

            public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, GameTime gameTime)
            {

                bool flip = false;
                int lineThickness = 2;

                if (_showTangents)
                {
                    for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
                    {
                        DrawHelpers.DrawBasicLine(new Vector2(artificialCpLine[i].X, artificialCpLine[i].Y), new Vector2(artificialCpLine[i + 1].X, artificialCpLine[i + 1].Y), 1, Color.Purple);
                    }

                    for (int i = 0; i < artificialCpLine.Count - 1; i += 2)
                    {
                        DrawHelpers.DrawBasicLine(new Vector2(artificialTangentLine[i].X, artificialTangentLine[i].Y), new Vector2(artificialTangentLine[i + 1].X, artificialTangentLine[i + 1].Y), 1, Color.Pink);
                    }
                }

                for (int i = 0; i < cps.Length; i++)
                {
                    DrawHelpers.DrawBasicPoint(new Vector2(cps[i].position.X, cps[i].position.Y), 4, Color.Red);
                }

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

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // supporting class.
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++

            public class ControlPoint
            {
                public Vector3 position = Vector3.Zero;
                public float weight = 0;
                public float distanceToNextCp = 0;
                public float startDistance = 0;
                public float ratio = 0;
                public int cpIndex = 0;
            }

        }
    }
}
