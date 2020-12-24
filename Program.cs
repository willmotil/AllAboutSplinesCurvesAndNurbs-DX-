using System;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            //using (var game = new Game_SubdivisionSpline()) game.Run();
            //using (var game = new Game_CatMullRomSplines()) game.Run();
            using (var game = new Game_HermiteSplines()) game.Run();
            //using (var game = new Game_BezierSplines()) game.Run();

            //using (var game = new Game_ImbalancedCurveWayPointCamera()) game.Run();
        }
    }
}
