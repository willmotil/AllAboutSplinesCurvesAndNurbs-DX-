
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

// https://www.geogebra.org/m/WPHQ9rUt

// hermites next maybe https://www.youtube.com/watch?v=4vjBXh3xYB4
// bezier thru points https://www.youtube.com/watch?v=tXrkNfHFakg  , https://www.youtube.com/watch?v=sZITw1X2ctU

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Game_BezierSplines : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public Texture2D _dot;

        MouseState ms;

        #region camera variables.

        WaypointCamera _camera;
        bool _useDemoWaypoints = true;
        static Vector3 _testTarget = new Vector3(350, 300, -25);

        #endregion

        static Vector3 _wpOffset = new Vector3(350, 240, -25);
        Vector3[] _wayPoints = new Vector3[]
        {
            new Vector3(120, 120, -5) + _wpOffset, new Vector3(120, -120, -5) + _wpOffset, new Vector3(-120, -120, -5) + _wpOffset, new Vector3(-120, 120, -5) + _wpOffset,
        };

        CurveBezierSpline cspline;

        int numOfPoints = 40;
        float weight = 0f;

        string msg =
              $" " + "\n" +
              $" " + "\n"
              ;



        public Game_BezierSplines()
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

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("MgGenFont");
            _dot = CreateDotTexture(GraphicsDevice, Color.White);

            _camera = new WaypointCamera(GraphicsDevice, _spriteBatch, _dot, new Vector3(2, 2, 10), new Vector3(0, 0, 0), Vector3.UnitY, 0.1f, 10000f, 1f, true, false);
            _camera.TransformCamera(_camera.World.Translation, _testTarget, _camera.World.Up);
            _camera.Up = Vector3.Forward;
            _camera.WayPointCycleDurationInTotalSeconds = 20f;
            _camera.MovementSpeedPerSecond = 3f;
            _camera.SetWayPoints(_wayPoints, true, 100);

            //cspline = new CatMullSpline(_wayPoints);
            cspline = new CurveBezierSpline(_wayPoints, true, 0.5f);
        }


        float t = 0f;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                t += elapsed / 10f;
                if (t > 1f)
                    t = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                t -= elapsed / 10f;
                if (t < 0f)
                    t = 1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                weight += .01f;
                if (weight > 2f)
                    weight = 0f;
                cspline = new CurveBezierSpline(_wayPoints, true, weight);
            }

            ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                _wayPoints[0] = new Vector3(ms.Position.X, ms.Position.Y, 0);
                cspline = new CurveBezierSpline(_wayPoints, true, weight);
            }

            msg = $"Bezier " + "\n" + $"weight " + weight;


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                _useDemoWaypoints = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
                _useDemoWaypoints = true;

            _camera.Update(_testTarget, _useDemoWaypoints, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            cspline.DrawWithSpriteBatch(_spriteBatch, gameTime);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, msg, new Vector2(10, 20), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Texture2D CreateDotTexture(GraphicsDevice device, Color color)
        {
            Color[] data = new Color[1] { color };
            Texture2D tex = new Texture2D(device, 1, 1);
            tex.SetData<Color>(data);
            return tex;
        }

    }

}



