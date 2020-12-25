
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{

    public class Game_BezierSplinesWeighted : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static Texture2D _dot;
        MouseState ms;
        public int currentScrollWheelvalue = 0;
        CurveBezierSplineWeighted curve;
        int numOfPoints = 100;
        float selectedWeight = 1f;
        int selectedCp = 0;
        float maxSelectableWeight = 9f;
        float cycledTime = 0f;
        Vector3 positionAtCycledTime = Vector3.Zero;
        string msg =
              $" " + "\n" +
              $" " + "\n"
              ;

        static Vector4 _wpOffset = new Vector4(350, 240, +5, 0);

        //Vector4[] _wayPoints = new Vector4[]
        //{
        //    new Vector4(80, 80, -5, 1f) + _wpOffset, new Vector4(80, -80, -5, 1) + _wpOffset, new Vector4(-80, -80, -5, 1f) + _wpOffset, new Vector4(-80, 80, -5, 1f) + _wpOffset,
        //};

        //Vector4[] _wayPoints = new Vector4[]
        //{
        //    new Vector4(80, 0, -5, 1f) + _wpOffset, new Vector4(0, -80, -5, 1f) + _wpOffset, new Vector4(-80, 0, -5, 1f) + _wpOffset, new Vector4(0, 80, -5, 1f) + _wpOffset,
        //};

        Vector4[] _wayPoints = new Vector4[]
        {
             new Vector4(-100, -80,  0, 1f) + _wpOffset, new Vector4(-50, 80,  0, 1f) + _wpOffset, new Vector4(0, -80,  0, 1f) + _wpOffset, new Vector4(50, 80,  0, 1f) + _wpOffset, new Vector4(100, -80, 0, 1f) + _wpOffset, new Vector4(150, 80, 0, 1f) + _wpOffset
        };


        public Game_BezierSplinesWeighted()
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
            curve = new CurveBezierSplineWeighted(_wayPoints, 0.5f, false);
            DrawHelpers.Initialize(GraphicsDevice, _spriteBatch, null);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            ms = Mouse.GetState();
            bool redoCurve = false;

            if (IsPressedWithDelay(Keys.Tab, gameTime))
            {
                selectedCp += 1;
                if (selectedCp > _wayPoints.Length - 1)
                    selectedCp = 0;
            }

            if (IsPressedWithDelay(Keys.Up, gameTime) || ms.ScrollWheelValue > currentScrollWheelvalue)
            {
                selectedWeight += .05f;
                if (selectedWeight > maxSelectableWeight)
                    selectedWeight = -1f;
                _wayPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalue = ms.ScrollWheelValue;
                redoCurve = true;
            }

            if (IsPressedWithDelay(Keys.Down, gameTime) || ms.ScrollWheelValue < currentScrollWheelvalue)
            {
                selectedWeight -= .05f;
                if (selectedWeight < -1f)
                    selectedWeight = maxSelectableWeight;
                _wayPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalue = ms.ScrollWheelValue;
                redoCurve = true;
            }

            if (ms.LeftButton == ButtonState.Pressed)
            {
                _wayPoints[selectedCp] = new Vector4(ms.Position.X, ms.Position.Y, 0, _wayPoints[selectedCp].W);
                redoCurve = true;
            }

            if (ms.RightButton == ButtonState.Pressed)
                CheckPointSelected();

            if (redoCurve)
                curve = new CurveBezierSplineWeighted(_wayPoints, false);

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            cycledTime += elapsed / 10f;
            if (cycledTime > 1f)
                cycledTime = 0;
            positionAtCycledTime = curve.GetSplinePoint(cycledTime);

            msg =
                $"Bezier " +
                $"\n" + $"Left Click ........ Move selectedCp " +
                $"\n" + $"Right Click ...... Selected Cp " + selectedCp +
                $"\n" + $"Mouse Scroll ... Alter Weight " + selectedWeight
                ;

            base.Update(gameTime);
        }

        public void CheckPointSelected()
        {
            int size = 10;
            var checkedRect = new Rectangle(ms.Position.X - size/2, ms.Position.Y - size/2, size, size);
            for (int i =0; i < _wayPoints.Length; i++)
                if(checkedRect.Contains( new Vector2(_wayPoints[i].X, _wayPoints[i].Y)  ))
                    selectedCp = i;
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            curve.DrawWithSpriteBatch(_spriteBatch, _font, gameTime);
            DrawHelpers.DrawCrossHair(positionAtCycledTime.ToVector2(), 5, Color.White);
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


/*

        #region camera variables.

        //WaypointCamera _camera;
        //bool _useDemoWaypoints = true;
        //static Vector3 _testTarget = new Vector3(350, 300, -25);

        #endregion

            //_camera = new WaypointCamera(GraphicsDevice, _spriteBatch, _dot, new Vector3(2, 2, 10), new Vector3(0, 0, 0), Vector3.UnitY, 0.1f, 10000f, 1f, true, false);
            //_camera.TransformCamera(_camera.World.Translation, _testTarget, _camera.World.Up);
            //_camera.Up = Vector3.Forward;
            //_camera.WayPointCycleDurationInTotalSeconds = 20f;
            //_camera.MovementSpeedPerSecond = 3f;
            //_camera.SetWayPoints(_wayPoints, true, 100);


             //if (IsPressedWithDelay(Keys.R, gameTime))
            //{
            //    var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    t += elapsed / 10f;
            //    if (t > 1f)
            //        t = 0;
            //}

            //if (IsPressedWithDelay(Keys.T, gameTime))
            //{
            //    var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    t -= elapsed / 10f;
            //    if (t < 0f)
            //        t = 1f;
            //}

            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //    _useDemoWaypoints = false;
            //if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            //    _useDemoWaypoints = true;

            //_camera.Update(_testTarget, _useDemoWaypoints, gameTime);
 */
