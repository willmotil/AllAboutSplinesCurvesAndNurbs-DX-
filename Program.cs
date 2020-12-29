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
            //using (var game = new Game_HermiteSplines()) game.Run();
            //using (var game = new Game_BezierSplines()) game.Run();
            //using (var game = new Game_BezierSplinesWeighted()) game.Run();
            //using (var game = new Game_BezierSplinesWeightedTimed()) game.Run();
            //using (var game = new Game_Bezier06RecalculatingCps()) game.Run();
            //using (var game = new Game_MeshBsplineWeightedTimed()) game.Run();
            // work in progress
            using (var game = new Game_MeshBwtPrimitive()) game.Run();
            
        }
    }
}
