using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// https://www.geogebra.org/m/WPHQ9rUt

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Game_SubdivisionSpline : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static Texture2D _generatedTexture;
        public static Texture2D _dot;

        #region camera variables.

        WaypointCamera _camera;
        bool _useDemoWaypoints = true;
        static Vector3 _testTarget = new Vector3(350, 300, -25);

        static Vector3 _wpOffset = new Vector3(350, 240, -25);
        Vector3[] _wayPoints = new Vector3[]
        {
            new Vector3(120, 120, -5) + _wpOffset, new Vector3(120, -120, -5) + _wpOffset, new Vector3(-120, -120, -5) + _wpOffset, new Vector3(-120, 120, -5) + _wpOffset,
        };

        CurveSubdivisionSpline bspline;

        Vector3[] bezierCurveLines;

        string msg =
         $" " + "\n" +
         $" " + "\n"
        ;

        #endregion

        public Game_SubdivisionSpline()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            this.Window.AllowAltF4 = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("MgGenFont");
            //_generatedTexture = CreateCheckerBoard(GraphicsDevice, 20, 20, Color.White, Color.Red);
            _dot = CreateDotTexture(GraphicsDevice, Color.White);

            _camera = new WaypointCamera(GraphicsDevice, _spriteBatch, _dot, new Vector3(2, 2, 10), new Vector3(0, 0, 0), Vector3.UnitY, 0.1f, 10000f, 1f, true, false);
            _camera.TransformCamera(_camera.World.Translation, _testTarget, _camera.World.Up);
            _camera.Up = Vector3.Forward;
            _camera.WayPointCycleDurationInTotalSeconds = 20f;
            _camera.MovementSpeedPerSecond = 3f;
            _camera.SetWayPoints(_wayPoints, true, 100);

            bspline = new CurveSubdivisionSpline(_wayPoints);
            bezierCurveLines = new Vector3[20];
            for (int i =0; i < 20; i++)
            {
                float t = (float)(i) / (float)(19);
                bezierCurveLines[i] = bspline.GetSplinePoint(1, t);
            }
        }

        float t = 0f;
        protected override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            t += elapsed / 5f;
            if (t > 1f)
                t = 0;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                _useDemoWaypoints = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
                _useDemoWaypoints = true;

            msg = $"Subdivision Spline ?? de Casteljau's algorithm " + "\n" + $" ";

            bspline.GetSplinePoint(1, t);

            _camera.Update(_testTarget, _useDemoWaypoints, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawBsplineWithSpriteBatch(gameTime);

            base.Draw(gameTime);
        }

        public void DrawBsplineWithSpriteBatch(GameTime gameTime)
        {
            _spriteBatch.Begin();

            for (int i = 0; i < bezierCurveLines.Length -1; i++)
            {
                DrawHelpers.DrawBasicLine(ToVector2(bezierCurveLines[i]), ToVector2(bezierCurveLines[i+1]), 1, Color.Green);
            }

            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[0]), ToVector2(bspline.solved[1]), 1, Color.Blue);
            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[1]), ToVector2(bspline.solved[2]), 1, Color.Blue);
            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[2]), ToVector2(bspline.solved[3]), 1, Color.Blue);

            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[4]), ToVector2(bspline.solved[5]), 1, Color.Yellow);
            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[5]), ToVector2(bspline.solved[6]), 1, Color.Yellow);

            DrawHelpers.DrawBasicLine(ToVector2(bspline.solved[7]), ToVector2(bspline.solved[8]), 1, Color.Orange);

            DrawHelpers.DrawBasicPoint(ToVector2(bspline.solved[9]), Color.Red);

            _spriteBatch.DrawString(_font, msg, new Vector2(10, 20), Color.White);

            _spriteBatch.End();
        }

        public static Texture2D CreateDotTexture(GraphicsDevice device, Color color)
        {
            Color[] data = new Color[1] { color };
            Texture2D tex = new Texture2D(device, 1, 1);
            tex.SetData<Color>(data);
            return tex;
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

