using System;
using System.Collections.Generic;
using System.Text;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
 class NotesOnSplines
    {
        /*
         * Notes and rejected or attempted stuff that has maybe bits to save.
         * 
         */
         
         
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

            //var rate = 1f - (ratioc / ratioa);
            //var pAB = ((b - a) * rate) + a;
            //var result = b - pAB;
            //artificialTangentLine.Add(pAB);
            //artificialTangentLine.Add(b);

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

        ///// <summary>
        ///// +++++++++++   THIS is a circle as defined for a very specific case... and i wanted it to see the tangental relation.
        ///// 
        /////  However this works only for the special case.
        /////  
        ///// </summary>
        ///// <returns></returns>
        //public Vector3 WeightedPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float time)
        //{
        //    // for a circle  it is 
        //    // for Bezier curve with n segments the optimal distance to the control points, in the sense that the middle of the curve lies on the circle itself, is 
        //    // (4/3)*tan(pi/(2n))  were n is the number of control points in the curve.
        //    //
        //    // So for 4 points it is (4/3)*tan(pi/8) = 4*(sqrt(2)-1)/3 = 0.552284749831.
        //    //
        //    
        ////     var weightedDivisor = 3; // 3 is circular
        ////      4*(.414f)/3  ==  numberOfPoints* (.414f) /weightedDivisor
        ////     

        //    // another close approximation is  pi / (npoints *3) * distance(p2-p1);

        //    // or if i reverse the above ideas then. .... the diameter divided by the number of points * 3 gives the coefficient multiplier for surrounding point vector differences to the point in question.
        //    //
        //    // so in this case 2/ (4*3) * ( p[index -1] - p[index +1] )
        //    //
        //    // for v1's inner point  from (v2 - v0) * 0.27614237f we add v1 to get the new p1 we then set v1 to p0
        //    // for v2s inner point from (v1 - v3) * 0.27614237f we add v2 to get the new p2 we then set v2 to p3


        //    Vector3 p0 = v0; Vector3 p1 = v1; Vector3 p2 = v2; Vector3 p3 = v3;

        //    //float weight = 1.41f;

        //    var b1 = (v2 - v0) * 0.27614237f;
        //    p1 = v1 + b1;
        //    p0 = v1;

        //    var b2 = (v1 - v3) * 0.27614237f;
        //    p2 = v2 + b2;
        //    p3 = v2;

        //    float t = time;
        //    //float t = time * .33f + .33f;
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

        //    artificialCpLine.Add(p0);
        //    artificialCpLine.Add(p1);
        //    artificialCpLine.Add(p2);
        //    artificialCpLine.Add(p3);

        //    return result;
        //}


        ///// <summary>
        ///// This is getting close to what i want
        ///// </summary>
        ///// <returns></returns>
        //public Vector3 WeightedPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float time)
        //{

        //    Vector3 p0 = v0; Vector3 p1 = v1; Vector3 p2 = v2; Vector3 p3 = v3;

        //    //float weight = 1.41f;
        //    var n1 = GetIdealTangentVector(v0, v1, v2);
        //    n1.Normalize();
        //    var d1 = Vector3.Distance( (v2 - v1) * 0.27614237f * _weight , Vector3.Zero); 
        //    p1 = v1 + n1 * d1;
        //    p0 = v1;

        //    var n2= GetIdealTangentVector(v3, v2, v1);
        //    n2.Normalize();
        //    var d2 = Vector3.Distance((v2 - v1) * 0.27614237f * _weight, Vector3.Zero); 
        //    p2 = v2 + n2 * d2;
        //    p3 = v2;

        //    float t = time;
        //    //float t = time * .33f + .33f;
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

        //    artificialCpLine.Add(p0);
        //    artificialCpLine.Add(p1);
        //    artificialCpLine.Add(p2);
        //    artificialCpLine.Add(p3);

        //    return result;
        //}
         
                 //public Vector3 GetTangentIdealTangentVectorForCenterPoint(Vector3 a, Vector3 c, Vector3 b)
        //{
        //    float disCA = Vector3.Distance(a, c);
        //    float disCB = Vector3.Distance(b, c);
        //    Vector3 result;
        //    if (disCA < disCB)
        //    {
        //        var dir = b - c;
        //        dir.Normalize();
        //        var nb = dir * disCA;
        //        result = Vector3.Normalize(a - nb);
        //    }
        //    else
        //    {
        //        var dir = a - c;
        //        dir.Normalize();
        //        var na = dir * disCB;
        //        result = Vector3.Normalize(na - b);
        //    }
        //    return result;
        //}
         
         */

}
}
