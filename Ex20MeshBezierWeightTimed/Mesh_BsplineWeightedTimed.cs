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
    public class Mesh_BsplineWeightedTimed
    {

        ///// <summary>
        ///// This holds the information relating to the control points given.
        ///// </summary>
        Curve_ForMeshBsplineWeightTimed[] curves;

        ///// <summary>
        ///// This holds the finished point on the mesh.
        ///// </summary>
        Curve_ForMeshBsplineWeightTimed finalMeshCurve;

        /// <summary>
        /// cp width 
        /// </summary>
        public int cpWidth = 4;
        /// <summary>
        /// cp height
        /// </summary>
        public int cpHeight = 4;

        ///// <summary>
        ///// control point ref.
        ///// </summary>
        Vector4[] cps;

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

        float totaldist = 0;
        /// <summary>
        /// the calculated total integrated distance of the curve.
        /// </summary>
        public float TotalCurveDistance { get { return totaldist; } }



        //#region  non requisite optional astetic visual values.
        //List<Vector3> artificialCpLine = new List<Vector3>();
        //List<Vector3> artificialTangentLine = new List<Vector3>();
        //#endregion

        //#region  temporary tracking values used thruout many methods as the curves are processed.
        //int currentCpXindex = 0;
        //int index0 = 0;
        //int index1 = 0;
        //int index2 = 0;
        //int index3 = 0;
        //#endregion


        #region constructors

        /// <summary>
        /// </summary>
        public Mesh_BsplineWeightedTimed(Vector4[] controlPoints, int width, int height)
        {
            CreateMesh(controlPoints, width, height);
        }

        /// <summary>
        /// </summary>
        public Mesh_BsplineWeightedTimed(Vector4[] controlPoints, int width, int height, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve)
        {
            _closedControlPoints = closedControlPoints;
            _uniformedCurve = uniformedCurve;
            _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateMesh(controlPoints, width, height);
        }

        /// <summary> 
        /// </summary>
        public Mesh_BsplineWeightedTimed(Vector4[] controlPoints, int width, int height, int numOfVisualCurvatureSegmentPoints, bool closedControlPoints, bool uniformedCurve, float globalWeight, bool showTangents)
        {
            _closedControlPoints = closedControlPoints;
            _uniformedCurve = uniformedCurve;
            _numOfCurvatureSegmentPoints = numOfVisualCurvatureSegmentPoints;
            CreateMesh(controlPoints, width, height);
        }

        #endregion

        //#region curve calculation methods

        private void CreateMesh(Vector4[] controlPoints, int width, int height)
        {
            this.cpWidth = width;
            this.cpHeight = height;
            CreateSpline(controlPoints);
        }

        private void CreateSpline(Vector4[] controlPoints)
        {
            cps = controlPoints;
            curves = new Curve_ForMeshBsplineWeightTimed[cpHeight];
            for (int y = 0; y < cpHeight; y++)
            {
                List<Vector4> pw = new List<Vector4>();
                for (int x = 0; x < cpWidth; x++)
                {
                    var v = controlPoints[GetCpIndex(x, y)];
                    pw.Add(v);
                }
                curves[y] = new Curve_ForMeshBsplineWeightTimed(pw.ToArray(), _numOfCurvatureSegmentPoints, _closedControlPoints, _uniformedCurve);
            }

            curveLinePoints = new Vector3[_numOfCurvatureSegmentPoints * _numOfCurvatureSegmentPoints];

            var loopCount = _numOfCurvatureSegmentPoints;
            var divisor = loopCount - 1;

            // Create the curve either uniformed or non uniformed.
            if (_uniformedCurve)
            {
                for (int y = 0; y < loopCount; y++)
                {
                    float ty = (float)(y) / (float)(divisor);
                    for (int x = 0; x < loopCount; x++)
                    {
                        float tx = (float)(x) / (float)(divisor);
                        int index = loopCount * y + x;
                        curveLinePoints[index] = ToVector3(GetUniformMeshPoint(tx, ty));
                    }
                }
            }
            else
            {
                for (int y = 0; y < loopCount; y++)
                {
                    float ty = (float)(y) / (float)(divisor);
                    for (int x = 0; x < loopCount; x++)
                    {
                        float tx = (float)(x) / (float)(divisor);
                        int index = loopCount * y + x;
                        curveLinePoints[index] = ToVector3(GetNonUniformMeshPoint(tx, ty));
                    }
                }
            }
        }

        public Vector4 GetNonUniformMeshPoint(float timeX, float timeY)
        {
            List<Vector4> pw = new List<Vector4>();
            for (int y = 0; y < cpHeight; y++)
                pw.Add(curves[y].GetNonUniformSplinePoint(timeX));
            // calculationally extraspensive argg.
            finalMeshCurve = new Curve_ForMeshBsplineWeightTimed(pw.ToArray(),_numOfCurvatureSegmentPoints,_closedControlPoints,_uniformedCurve);
            return finalMeshCurve.GetNonUniformSplinePoint(timeY);
        }
        public Vector4 GetUniformMeshPoint(float timeX, float timeY)
        {
            List<Vector4> pw = new List<Vector4>();
            for (int y = 0; y < cpHeight; y++)
                pw.Add(curves[y].GetUniformSplinePoint(timeX));
            // calculationally extraspensive argg.
            finalMeshCurve = new Curve_ForMeshBsplineWeightTimed(pw.ToArray(), _numOfCurvatureSegmentPoints, _closedControlPoints, _uniformedCurve);
            return finalMeshCurve.GetUniformSplinePoint(timeY);
        }


        int GetCpIndex(int x, int y)
        {
            return y * cpWidth + x;
        }

        Point GetCpIndexXy(int index)
        {
            int y = (int)(index / cpWidth);
            int x = index - y * cpWidth;
            return new Point(x, y);
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

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, SpriteFont font, GameTime gameTime)
        {

            //for(int y =0; y < curves.Length; y++)
            //    curves[y].DrawWithSpriteBatch(_spriteBatch, font, gameTime);

            //finalMeshCurve.DrawWithSpriteBatch(_spriteBatch, font, gameTime);

            DrawWithSpriteBatch(_spriteBatch, gameTime);

            //for (int i = 0; i < cps.Length; i++)
            //{
            //    string msg =
            //        $" CP[{i}]  w: {cps[i].weight.ToString("0.000")}" +
            //        $"\n startDist: {cps[i].startDistance.ToString("###0.00")}" +
            //        $"\n segDist: {cps[i].distanceToNextCp.ToString("###0.00")}"
            //        ;
            //    _spriteBatch.DrawString(font, msg, new Vector2(cps[i].position.X, cps[i].position.Y), Color.Black);
            //}
        }

        public void DrawWithSpriteBatch(SpriteBatch _spriteBatch, GameTime gameTime)
        {

            bool flip = false;
            int lineThickness = 2;

            // result point rows.
            for (int y = 0; y < _numOfCurvatureSegmentPoints - 1; y++)
            {
                for (int x = 0; x < _numOfCurvatureSegmentPoints - 1; x++)
                {
                    var index = _numOfCurvatureSegmentPoints * y + x;
                    var index2 = _numOfCurvatureSegmentPoints * y + x + 1;
                    var index3 = _numOfCurvatureSegmentPoints * y + x + _numOfCurvatureSegmentPoints;
                    var col = Color.Black;
                    if (flip)
                        col = Color.Green;
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[index]), ToVector2(curveLinePoints[index2]), lineThickness, col);
                    DrawHelpers.DrawBasicLine(ToVector2(curveLinePoints[index]), ToVector2(curveLinePoints[index3]), lineThickness, col);
                    flip = !flip;
                }
            }

            for (int y = 0; y < cpHeight; y++)
            {
                for (int x = 0; x < cpWidth; x++)
                {
                    int index = y * cpWidth + x;
                    DrawHelpers.DrawBasicPoint(new Vector2(cps[index].X, cps[index].Y), 4, Color.Red);
                }
            }
        }


    }
}








    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //    // supporting class.
    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++


    //    public class ControlPointRowColumn
    //    {
    //        public int cpWidth;
    //        public int cpHeight;
    //        public List<ControlPointsX> yListOfRows = new List<ControlPointsX>();

    //        public ControlPoint GetCp(int x, int y)
    //        {
    //            return yListOfRows[y].xRowItems[x];
    //        }

    //        public void SetCp(ControlPoint cp, int index)
    //        {
    //            var p = GetCpIndexXy(index);
    //            SetCp(cp, p.X, p.Y);
    //        }

    //        public void SetCp(ControlPoint cp, int x, int y)
    //        {
    //            while (y >= yListOfRows.Count)
    //                yListOfRows.Add(new ControlPointsX());
    //            while (x >= yListOfRows[y].xRowItems.Count)
    //                yListOfRows[y].xRowItems.Add(new ControlPoint());
    //            // set
    //            yListOfRows[y].xRowItems[x] = cp;
    //        }

    //        int WrapCpIndexXy(ref int x, ref int y)
    //        {
    //            if (x >= cpWidth)
    //                x = x - cpWidth;
    //            if (x < 0)
    //                x = x + cpWidth;
    //            if (y >= cpHeight)
    //                y = y - cpHeight;
    //            if (y < 0)
    //                y = y + cpHeight;
    //            int index = y * cpWidth + x;
    //            return index;
    //        }

    //        int GetCpIndex(int x, int y)
    //        {
    //            return y * cpWidth + x;
    //        }

    //        Point GetCpIndexXy(int index)
    //        {
    //            int y = (int)(index / cpWidth);
    //            int x = index - y * cpWidth;
    //            return new Point(x, y);
    //        }
    //    }
    //    public class ControlPointsX
    //    {
    //        public List<ControlPoint> xRowItems = new List<ControlPoint>();
    //    }

    //    public class ControlPoint
    //    {
    //        public Vector3 position = Vector3.Zero;
    //        public float weight = 0;
    //        public float distanceToNextCp = 0;
    //        public float startDistance = 0;
    //        public float ratio = 0;
    //        public int cpIndex = 0;
    //    }





    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //    //++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //}
//}


//public int EnsureIndexWidthInRange(int x)
//{
//    while (x < 0)
//        x = x + (cps.cpWidth);
//    while (x > (cps.cpWidth - 1))
//        x = x - (cps.cpWidth);
//    return x;
//}

//public int EnsureIndexHeightInRange(int y)
//{
//    while (y < 0)
//        y = y + (cps.cpHeight);
//    while (y > (cps.cpHeight - 1))
//        y = y - (cps.cpHeight);
//    return y;
//}

///// <summary>
///// Here we go this one is only for closed curves open ones are easy just append the end points to the control point list
///// </summary>
///// <returns></returns>
//public Vector3 WeightedPoint(ControlPoint c0, ControlPoint c1, ControlPoint c2, ControlPoint c3, float time)
//{
//    Vector3 v0 = c0.position; Vector3 v1 = c1.position; Vector3 v2 = c2.position; Vector3 v3 = c3.position;
//    Vector3 p0 = c0.position; Vector3 p1 = c1.position; Vector3 p2 = c2.position; Vector3 p3 = c3.position;

//    var segmentDistance = Vector3.Distance(v2, v1) * 0.35355339f; // * _weight;

//    var n1 = Vector3.Normalize(GetIdealTangentVector(v0, v1, v2));
//    p1 = v1 + n1 * segmentDistance * c1.weight;
//    p0 = v1;

//    var n2 = Vector3.Normalize(GetIdealTangentVector(v3, v2, v1));
//    p2 = v2 + n2 * segmentDistance * c2.weight;
//    p3 = v2;

//    //float t = time * .33f + .33f;
//    float t = time;
//    float t2 = t * t;
//    float t3 = t2 * t;
//    float i = 1f - t;
//    float i2 = i * i;
//    float i3 = i2 * i;

//    Vector3 result =
//        (i3) * 1f * p0 +
//        (i2 * t) * 3f * p1 +
//        (i * t2) * 3f * p2 +
//        (t3) * 1f * p3;

//    //artificialCpLine.Add(p0);  // visualization stuff.
//    //artificialCpLine.Add(p1);
//    //artificialCpLine.Add(p2);
//    //artificialCpLine.Add(p3);

//    return result;
//}

//public Vector3 GetIdealTangentVector(Vector3 a, Vector3 b, Vector3 c)
//{
//    float disa = Vector3.Distance(a, b);
//    float disc = Vector3.Distance(b, c);
//    float ratioa = disa / (disa + disc);
//    var pAB = ((b - a) * ratioa) + a;
//    var pBC = ((c - b) * ratioa) + b;
//    var result = pBC - pAB;
//    // prevent nan later on.
//    if (result == Vector3.Zero)
//        result = c - a;

//    //artificialTangentLine.Add(pAB); // visualization stuff.
//    //artificialTangentLine.Add(pBC);

//    return result;
//}


//int GetCpIndex(int x, int y)
//{
//    return y * cpWidth + x;
//}
//Point GetCpIndexXy(int index)
//{
//    int y = (int)(index / cpWidth);
//    int x = index - y * cpWidth;
//    return new Point(x, y);
//}

//int GetCurveIndex(int x, int y)
//{
//    return y * _numOfCurvatureSegmentPoints + x;
//}
//Point GetCurveIndexXY(int index)
//{
//    int y = (int)(index / _numOfCurvatureSegmentPoints);
//    int x = index - y * _numOfCurvatureSegmentPoints;
//    return new Point(x, y);
//}



//#endregion
