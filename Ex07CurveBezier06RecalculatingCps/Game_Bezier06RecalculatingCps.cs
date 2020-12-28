
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{

    public class Game_Bezier06RecalculatingCps : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static Texture2D _dot;
        MouseState ms;

        public int currentScrollWheelvalue = 0;
        CurveBezier06RecalculatingCps curve;
        float maxSelectableWeight = 9f;
        float selectedWeight = 1f;
        int numOfPoints = 50;
        int selectedCp = 0;
        bool isMouseNearCp = false;
        int mouseNearCpNumber = 0;
        bool isCurveClosed = false;
        bool isUniformedUsed = true;
        bool showGeneratedTangentsPositions = false;
        float cycledTime = 0f;
        float gameTimeInSeconds = 0;
        Vector4 positionWeightAtCycledTime = Vector4.Zero;
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

        //Vector4[] _wayPoints = new Vector4[]
        //{
        //     new Vector4(-100, -80,  0, 1f) + _wpOffset, new Vector4(-50, 80,  0, 1f) + _wpOffset, new Vector4(0, -80,  0, 1f) + _wpOffset, new Vector4(50, 80,  0, 1f) + _wpOffset, new Vector4(100, -80, 0, 1f) + _wpOffset, new Vector4(150, 80, 0, 1f) + _wpOffset
        //};

        Vector4[] _wayPoints = new Vector4[]
        {
             new Vector4(100, 150,  0, 1f) , new Vector4(150, 200,  0, 1f) , new Vector4(250, 250,  0, 1f) , new Vector4(500, 150,  0, 1f) , new Vector4(600, 200, 0, 1f)
        };


        public Game_Bezier06RecalculatingCps()
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
            DrawHelpers.Initialize(GraphicsDevice, _spriteBatch, null);

            curve = new CurveBezier06RecalculatingCps(_wayPoints, numOfPoints, isCurveClosed, isUniformedUsed);
            curve._showTangents = showGeneratedTangentsPositions;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ms = Mouse.GetState();
            bool redoCurve = false;
            CurveIterationTimer(gameTime, 10f);


            // show extra position and tangent lines.
            if (IsPressedWithDelay(Keys.F1, gameTime))
            {
                showGeneratedTangentsPositions = !showGeneratedTangentsPositions;
                curve._showTangents = showGeneratedTangentsPositions;
            }

            // switch to a uniformed or non uniformed curve.
            if (IsPressedWithDelay(Keys.Space, gameTime))
            {
                isUniformedUsed = !isUniformedUsed;
                redoCurve = true;
            }

            // switch curve open or closed.
            if (IsPressedWithDelay(Keys.Tab, gameTime))
            {
                isCurveClosed = !isCurveClosed;
                redoCurve = true;
            }

            // adjust weight
            if (IsPressedWithDelay(Keys.Up, gameTime) || ms.ScrollWheelValue > currentScrollWheelvalue)
            {
                selectedWeight += .05f;
                if (selectedWeight > maxSelectableWeight)
                    selectedWeight = -1f;
                _wayPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalue = ms.ScrollWheelValue;
                redoCurve = true;
            }

            // adjust weight
            if (IsPressedWithDelay(Keys.Down, gameTime) || ms.ScrollWheelValue < currentScrollWheelvalue)
            {
                selectedWeight -= .05f;
                if (selectedWeight < -1f)
                    selectedWeight = maxSelectableWeight;
                _wayPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalue = ms.ScrollWheelValue;
                redoCurve = true;
            }

            // adjust control point position.
            if (ms.LeftButton == ButtonState.Pressed)
            {
                _wayPoints[selectedCp] = new Vector4(ms.Position.X, ms.Position.Y, 0, _wayPoints[selectedCp].W);
                redoCurve = true;
            }

            // next control point
            if (IsPressedWithDelay(Keys.Right, gameTime))
            {
                selectedCp += 1;
                if (selectedCp > _wayPoints.Length - 1)
                    selectedCp = 0;
            }

            // check or select a control point.
            CheckPointSelected();

            if (redoCurve)
                curve = new CurveBezier06RecalculatingCps(_wayPoints, numOfPoints, isCurveClosed, isUniformedUsed);

            string msg2 = "Open";
            if (isCurveClosed)
                msg2 = "Closed";
            string msg3 = "NonUniform";
            if (isUniformedUsed)
                msg3 = "Uniform";
            msg =
                $"GameTime In Seconds {gameTimeInSeconds.ToString("#########0.00")}  Cycle time over curve: {cycledTime.ToString("###0.00")}" +
                $"\n" + $"Bezier " +
                $"\n" + $"Total Distance: {curve.TotalCurveDistance.ToString("#########0.000")}" +
                $"\n" + $"Press Tab ....... Curve is {msg2}" +
                $"\n" + $"Press Space .. Curve is {msg3}" +
                $"\n" + $"Left Click ........ Move selectedCp " +
                $"\n" + $"Right Click ...... Selected Cp " + selectedCp +
                $"\n" + $"Mouse Scroll ... Alter Weight " + selectedWeight
                ;

            base.Update(gameTime);
        }

        public void CurveIterationTimer(GameTime gameTime, float cycleTimeInSeconds)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            cycledTime += elapsed / cycleTimeInSeconds;
            if (cycledTime > 1f)
                cycledTime = 0;
            gameTimeInSeconds += elapsed;
            if (isUniformedUsed)
                positionWeightAtCycledTime = curve.GetUniformSplinePoint(cycledTime);
            else
                positionWeightAtCycledTime = curve.GetNonUniformSplinePoint(cycledTime);
        }

        public void CheckPointSelected()
        {
            int size = 10;
            var checkedRect = new Rectangle(ms.Position.X - size / 2, ms.Position.Y - size / 2, size, size);
            isMouseNearCp = false;
            for (int i = 0; i < _wayPoints.Length; i++)
            {
                if (checkedRect.Contains(new Vector2(_wayPoints[i].X, _wayPoints[i].Y)))
                {
                    isMouseNearCp = true;
                    mouseNearCpNumber = i;
                    if (ms.RightButton == ButtonState.Pressed)
                        selectedCp = i;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            curve.DrawWithSpriteBatch(_spriteBatch, _font, gameTime);

            if (isMouseNearCp)
            {
                var col = Color.Blue;
                if(selectedCp == mouseNearCpNumber)
                    col = Color.Red;
                DrawHelpers.DrawCrossHair(new Vector2(_wayPoints[mouseNearCpNumber].X, _wayPoints[mouseNearCpNumber].Y), 15, col);
            }

            var p = new Vector2(positionWeightAtCycledTime.X, positionWeightAtCycledTime.Y);
            DrawHelpers.DrawCrossHair(p, 5, Color.White);
            _spriteBatch.DrawString(_font, $" pos X:{p.X.ToString("###0.00")}, Y:{p.Y.ToString("###0.00")}\n weight: {positionWeightAtCycledTime.W.ToString("####0.000")}", p + new Vector2(0, 20), Color.White);

            _spriteBatch.DrawString(_font, msg, new Vector2(10, 5), Color.White);

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

