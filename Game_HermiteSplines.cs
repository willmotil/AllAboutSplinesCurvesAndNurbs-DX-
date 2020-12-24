



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// https://www.geogebra.org/m/WPHQ9rUt

// hermites next maybe https://www.youtube.com/watch?v=4vjBXh3xYB4
// bezier thru points https://www.youtube.com/watch?v=tXrkNfHFakg

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Game_HermiteSplines : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static Texture2D _dot;

        MouseState ms;

        #region camera variables.

        WaypointCamera _camera;
        bool _useDemoWaypoints = true;
        static Vector3 _testTarget = new Vector3(350, 300, -25);

        static Vector3 _wpOffset = new Vector3(350, 240, -25);
        Vector3[] _wayPoints = new Vector3[]
        {
            new Vector3(120, 120, -5) + _wpOffset, new Vector3(120, -120, -5) + _wpOffset, new Vector3(-120, -120, -5) + _wpOffset, new Vector3(-120, 120, -5) + _wpOffset,
        };

        CurveHermiteSpline cspline;

        int numOfPoints = 40;
        float weight = 0f;
        int selectedCp = 0;

        string msg =
              $" " + "\n" +
              $" " + "\n"
              ;

        #endregion

        public Game_HermiteSplines()
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
            _dot = CreateDotTexture(GraphicsDevice, Color.White);

            _camera = new WaypointCamera(GraphicsDevice, _spriteBatch, _dot, new Vector3(2, 2, 10), new Vector3(0, 0, 0), Vector3.UnitY, 0.1f, 10000f, 1f, true, false);
            _camera.TransformCamera(_camera.World.Translation, _testTarget, _camera.World.Up);
            _camera.Up = Vector3.Forward;
            _camera.WayPointCycleDurationInTotalSeconds = 20f;
            _camera.MovementSpeedPerSecond = 3f;
            _camera.SetWayPoints(_wayPoints, true, 100);

            cspline = new CurveHermiteSpline(_wayPoints, true, 0.5f);
        }


        float t = 0f;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (IsPressedWithDelay(Keys.R, gameTime))
            {
                var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                t += elapsed / 10f;
                if (t > 1f)
                    t = 0;
            }

            if (IsPressedWithDelay(Keys.T, gameTime))
            {
                var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                t -= elapsed / 10f;
                if (t < 0f)
                    t = 1f;
            }

            if (IsPressedWithDelay(Keys.Up, gameTime))
            {
                weight += .1f;
                if (weight > 2f)
                    weight = 0f;
                cspline = new CurveHermiteSpline(_wayPoints, true, weight);
            }

            if (IsPressedWithDelay(Keys.Down, gameTime))
            {
                weight -= .1f;
                if (weight < -1f)
                    weight = 2f;
                cspline = new CurveHermiteSpline(_wayPoints, true, weight);
            }

            if (IsPressedWithDelay(Keys.Tab, gameTime))
            {
                selectedCp += 1;
                if (selectedCp > 3)
                    selectedCp = 0;
            }

            ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                _wayPoints[selectedCp] = new Vector3(ms.Position.X, ms.Position.Y, 0);
                cspline = new CurveHermiteSpline(_wayPoints, true, weight);
            }

            msg = 
                $"Hermite " + 
                $"\n" + $"weight " + weight +
                $"\n" + $"selectedCp " + selectedCp
                ;


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

            _spriteBatch.Begin();
            cspline.DrawWithSpriteBatch(_spriteBatch, _font, gameTime);
            _spriteBatch.DrawString(_font, msg, new Vector2(10, 20), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsPressedWithDelay(Keys key, GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(key) && IsUnDelayed(gameTime))
                return true;
            else
                return false;
        }

        float delay = 0f;
        bool IsUnDelayed(GameTime gametime)
        {
            if (delay < 0)
            {
                delay = .25f;
                return true;
            }
            else
            {
                delay -= (float)gametime.ElapsedGameTime.TotalSeconds;
                return false;
            }
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



