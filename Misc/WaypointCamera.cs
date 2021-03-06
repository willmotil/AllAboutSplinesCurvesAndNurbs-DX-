﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    // Todo.       
    // I should later fix this up later to just take a set of waypoints and allow for the camera to generate a new uniformed set from them. 
    // To allow for the motion to be proportioned smoothly, that may not always be desired though.

    public class WaypointCamera
    {
        Vector3 _camPos = Vector3.Zero;
        Vector3 _targetLookAtPos = Vector3.One;
        Vector3 _forward = Vector3.Zero;
        Vector3 _lastForward = Vector3.Zero;
        Vector3 _camUp = Vector3.Zero;
        Matrix _camera = Matrix.Identity;
        float _near = 1f;
        float _far = 1000f;
        float inv = 1f;
        Matrix _projection = Matrix.Identity;
        float _durationElapsed = 0f;
        float _durationInSeconds = 1f;

        private Vector3[] wayPointReference;
        CurveMyImbalancedSpline wayPointCurvature;

        public Matrix Projection { get { return _projection; } set { _projection = value; } }
        public Matrix View { get { return Matrix.Invert(_camera); } }
        public Matrix World { get { return _camera; } }
        public Vector3 Position { get { return _camera.Translation; } }
        public Vector3 Forward { get { return _camera.Forward; } }
        public Vector3 Up { get { return _camera.Up; } set { _camera.Up = value; _camUp = value; }  }
        public Vector3 Right { get { return _camera.Right; } }
        public float WayPointCycleDurationInTotalSeconds { get { return _durationInSeconds; } set { _durationInSeconds = value; } }
        public float LookAtSpeedPerSecond { get; set; } = 1f;
        public float MovementSpeedPerSecond { get; set; } = 1f;
        public void SetWayPoints(Vector3[] waypoints, bool connectEnds, int numberOfSegments)
        {
            wayPointReference = waypoints;
            wayPointCurvature.SetWayPoints(waypoints, numberOfSegments, connectEnds);
        }


        /// <summary>
        /// This is a cinematic styled fixed camera it uses way points to traverse thru the world.
        /// </summary>
        public WaypointCamera(GraphicsDevice device, SpriteBatch spriteBatch, Texture2D dot, Vector3 pos, Vector3 target, Vector3 up, float nearClipPlane, float farClipPlane, float fieldOfView, bool perspective, bool inverseProjection)
        {
            DrawHelpers.Initialize(device, spriteBatch, dot);
            wayPointCurvature = new CurveMyImbalancedSpline();
            TransformCamera(pos, target, up);
            SetProjection(device, nearClipPlane, farClipPlane, fieldOfView, perspective, inverseProjection);
        }

        /// <summary>
        /// If waypoints are present then and automatedCameraMotion is set to true the cinematic camera will execute.
        /// </summary>
        public void Update(Vector3 targetPosition, bool automatedCameraMotion, GameTime gameTime)
        {
            if (automatedCameraMotion && wayPointReference != null)
                CurveThruWayPoints(targetPosition, wayPointReference, gameTime);
            else
                UpdateCameraUsingDefaultKeyboardCommands(gameTime);
        }

        public void UpdateCameraUsingDefaultKeyboardCommands(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // look
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                LookLeftLocally(LookAtSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                LookRightLocally(LookAtSpeedPerSecond * elapsed);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                LookUpLocally(LookAtSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                LookDownLocally(LookAtSpeedPerSecond * elapsed);

            // move
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                MoveForwardLocally(MovementSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                MoveBackLocally(MovementSpeedPerSecond * elapsed);

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                MoveUpLocally(MovementSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                MoveDownLocally(MovementSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                MoveLeftLocally(MovementSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                MoveRightLocally(MovementSpeedPerSecond * elapsed);

            // roll
            if (Keyboard.GetState().IsKeyDown(Keys.C))
                RollClockwise(MovementSpeedPerSecond * elapsed);
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                RollCounterClockwise(MovementSpeedPerSecond * elapsed);

            // transform
            TransformCamera(_camera.Translation, _camera.Forward + _camera.Translation, _camera.Up);
        }

        public void TransformCamera(Vector3 pos, Vector3 target, Vector3 up)
        {
            _targetLookAtPos = target;
            _camPos = pos;
            _camUp = up;
            _forward = _targetLookAtPos - _camPos;

            if (_forward.X == 0 && _forward.Y == 0 && _forward.Z == 0)
                _forward = _lastForward;
            else
                _lastForward = _forward;

            // TODO handle up down vector gimble lock astetic under fixed camera.
            // ...

            // ...

            _camera = Matrix.CreateWorld(_camPos, _forward, _camUp);
        }

        public void SetProjection(GraphicsDevice device, float nearClipPlane, float farClipPlane, float fieldOfView, bool perspective, bool inverseProjection)
        {
            _near = nearClipPlane;
            _far = farClipPlane;

            // Allows a change to a spritebatch style orthagraphic or inverse styled persepective, e.g. a viewer imagining a forward z positive depth going into the screen.
            inv = 1f;
            if (inverseProjection)
                inv *= -1f;

            if (perspective)
                _projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, device.Viewport.AspectRatio, _near, inv * _far);
            else
                _projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, _near, inv * _far);
        }

        public void CurveThruWayPoints(Vector3 targetPosition, Vector3[] waypoints, GameTime gameTime)
        {
            _durationElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_durationElapsed >= WayPointCycleDurationInTotalSeconds)
                _durationElapsed -= WayPointCycleDurationInTotalSeconds;

            float timeOnCurve = _durationElapsed / WayPointCycleDurationInTotalSeconds;
            var p = wayPointCurvature.GetPointOnCurveAtTime(timeOnCurve);

            TransformCamera(p, targetPosition, _camUp);
        }

        /// <summary>
        /// Moves the camera thru paths in straight lines from point to point.
        /// </summary>
        public void InterpolateThruWayPoints(Vector3 targetPosition, Vector3[] waypoints, bool useSmoothStep, GameTime gameTime)
        {
            _durationElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_durationElapsed >= WayPointCycleDurationInTotalSeconds)
                _durationElapsed -= WayPointCycleDurationInTotalSeconds;

            var interpolation = _durationElapsed / WayPointCycleDurationInTotalSeconds;
            float coeff = 1f / (float)waypoints.Length;
            int index = (int)(interpolation / coeff);
            int index2 = index + 1;
            if (index2 >= waypoints.Length)
            {
                index2 = 0;
            }
            float adjustedInterpolator = (interpolation - (coeff * index)) / coeff;
            if (useSmoothStep)
                TransformCamera(Vector3.SmoothStep(waypoints[index], waypoints[index2], adjustedInterpolator), targetPosition, _camUp);
            else
                TransformCamera(Vector3.Lerp(waypoints[index], waypoints[index2], adjustedInterpolator), targetPosition, _camUp);
        }

        public void DrawCurveThruWayPointsWithSpriteBatch(GameTime gameTime)
        {
            if (wayPointCurvature != null)
            {
                // current 2d camera position and forward on the orthographic xy plane.
                var camPosition = _camera.Translation;
                var camForward = _camera.Forward;
                var drawnCamDepthAdjustment = new Vector2(camPosition.Z / 4, camPosition.Z);
                var drawnCamTargetPos = new Vector2(_targetLookAtPos.X, _targetLookAtPos.Y);
                var drawnCamPos = new Vector2(camPosition.X, camPosition.Y);
                var drawnCamIteratedPos = new Vector2(drawnCamPos.X, drawnCamPos.Y);
                var drawnCamForawrdRay = new Vector2(camForward.X, camForward.Y) * 25;
                var drawnCamIteratedOffsetPos = drawnCamIteratedPos + drawnCamDepthAdjustment;
                var drawCamCrossHairLeft = new Vector2(-15, 0) + drawnCamIteratedOffsetPos;
                var drawCamCrossHairRight = new Vector2(0 + 15, 0) + drawnCamIteratedOffsetPos;
                var drawCamCrossHairUp = new Vector2(0, 0 - 15) + drawnCamIteratedOffsetPos;
                var drawCamCrossHairDown = new Vector2(0, 15) + drawnCamIteratedOffsetPos;

                // draw curved segmented output.
                var curveLineSegments = wayPointCurvature.GetCurveLineSegments;
                for (int i = 0; i < curveLineSegments.Count; i++)
                {
                    var segment = curveLineSegments[i];
                    if (i % 2 == 0)
                        DrawHelpers.DrawBasicLine(new Vector2(segment.Start.X, segment.Start.Y), new Vector2(segment.End.X, segment.End.Y), 1, Color.Black);
                    else
                        DrawHelpers.DrawBasicLine(new Vector2(segment.Start.X, segment.Start.Y), new Vector2(segment.End.X, segment.End.Y), 1, Color.Blue);
                }

                // Draw forward camera direction
                DrawHelpers.DrawBasicLine(drawnCamPos, drawnCamForawrdRay + drawnCamPos, 1, Color.Yellow);
                // Draw camera crosshairs forward to target.
                DrawHelpers.DrawBasicLine(drawnCamTargetPos, drawnCamIteratedOffsetPos, 1, Color.Yellow);
                // Draw a line from camera crosshairs to current position on way point curve.
                DrawHelpers.DrawBasicLine(drawnCamIteratedPos, drawnCamIteratedOffsetPos, 1, Color.White);
                // Draw cross hair for camera position
                DrawHelpers.DrawBasicLine(drawCamCrossHairLeft, drawCamCrossHairRight, 1, Color.Blue);
                DrawHelpers.DrawBasicLine(drawCamCrossHairUp, drawCamCrossHairDown, 1, Color.Blue);
                // Draw current 2d waypoint positions on the orthographic xy plane.
                foreach (var p in wayPointReference)
                {
                    DrawHelpers.DrawBasicPoint(new Vector2(p.X, p.Y), Color.White);
                }
            }
        }

        public void MoveForwardLocally(float amount)
        {
            _camera.Translation += _camera.Forward * amount;
        }
        public void MoveBackLocally(float amount)
        {
            _camera.Translation += _camera.Backward * amount;
        }
        public void MoveLeftLocally(float amount)
        {
            _camera.Translation += _camera.Left * amount;
        }
        public void MoveRightLocally(float amount)
        {
            _camera.Translation += _camera.Right * amount;
        }
        public void MoveUpLocally(float amount)
        {
            _camera.Translation += _camera.Up * amount;
        }
        public void MoveDownLocally(float amount)
        {
            _camera.Translation += _camera.Down * amount;
        }

        public void LookLeftLocally(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Up, amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookRightLocally(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Up, -amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookUpLocally(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Right, amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookDownLocally(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Right, -amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }

        public void RollClockwise(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Forward, amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void RollCounterClockwise(float amountInRadians)
        {
            var m = Matrix.CreateFromAxisAngle(_camera.Forward, -amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }

        public void LookLeftSystem(float amountInRadians)
        {
            var m = Matrix.CreateRotationY(amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookRightSystem(float amountInRadians)
        {
            var m = Matrix.CreateRotationY(-amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookUpSystem(float amountInRadians)
        {
            var m = Matrix.CreateRotationX(amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }
        public void LookDownSystem(float amountInRadians)
        {
            var m = Matrix.CreateRotationX(-amountInRadians);
            var t = _camera.Translation;
            _camera *= m;
            _camera.Translation = t;
        }

    }

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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
        public List<LineSegment> GetCurveLineSegments { get { return curveLineSegments; } }
        public List<LineSegment> GetCurveLineUniformedSegments { get { return curveUniformedLineSegments; } }

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
                curveLineSegmentsLength = segmentCount + 1;  //controlPointsLen;
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


    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



    //public static class DrawHelpers
    //{
    //    static SpriteBatch spriteBatch;
    //    static Texture2D dot;

    //    /// <summary>
    //    /// Flips atan direction to xna spritebatch rotational alignment defaults to true.
    //    /// </summary>
    //    public static bool SpriteBatchAtan2 = true;

    //    public static void Initialize(SpriteBatch spriteBatch, Texture2D dot)
    //    {
    //        DrawHelpers.spriteBatch = spriteBatch;
    //        if(DrawHelpers.dot == null)
    //            DrawHelpers.dot = dot;
    //    }

    //    public static void DrawRectangleOutline(Rectangle r, int lineThickness, Color c)
    //    {
    //        DrawSquareBorder(r, lineThickness, c);
    //    }
    //    public static void DrawSquareBorder(Rectangle r, int lineThickness, Color c)
    //    {
    //        Rectangle TLtoR = new Rectangle(r.Left, r.Top, r.Width, lineThickness);
    //        Rectangle BLtoR = new Rectangle(r.Left, r.Bottom - lineThickness, r.Width, lineThickness);
    //        Rectangle LTtoB = new Rectangle(r.Left, r.Top, lineThickness, r.Height);
    //        Rectangle RTtoB = new Rectangle(r.Right - lineThickness, r.Top, lineThickness, r.Height);
    //        spriteBatch.Draw(dot, TLtoR, c);
    //        spriteBatch.Draw(dot, BLtoR, c);
    //        spriteBatch.Draw(dot, LTtoB, c);
    //        spriteBatch.Draw(dot, RTtoB, c);
    //    }
    //    public static void DrawBasicLine(Vector2 s, Vector2 e, int thickness, Color linecolor)
    //    {
    //        Rectangle screendrawrect = new Rectangle((int)s.X, (int)s.Y, thickness, (int)Vector2.Distance(e, s));
    //        float rot = (float)Atan2Xna(e.X - s.X, e.Y - s.Y);
    //        spriteBatch.Draw(dot, screendrawrect, new Rectangle(0, 0, 1, 1), linecolor, rot, Vector2.Zero, SpriteEffects.None, 0);
    //    }
    //    public static void DrawBasicPoint(Vector2 p, Color c)
    //    {
    //        Rectangle screendrawrect = new Rectangle((int)p.X, (int)p.Y, 2, 2);
    //        spriteBatch.Draw(dot, screendrawrect, new Rectangle(0, 0, 1, 1), c, 0.0f, Vector2.One, SpriteEffects.None, 0);
    //    }
    //    public static float Atan2Xna(float difx, float dify)
    //    {
    //        if (SpriteBatchAtan2)
    //            return (float)System.Math.Atan2(difx, dify) * -1f;
    //        else
    //            return (float)System.Math.Atan2(difx, dify);
    //    }
    //}

}

