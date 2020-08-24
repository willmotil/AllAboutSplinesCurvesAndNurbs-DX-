using System;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            //using (var game = new Game1()) game.Run();
            using (var game = new Game2_Bspline()) game.Run();

        }
    }
}
