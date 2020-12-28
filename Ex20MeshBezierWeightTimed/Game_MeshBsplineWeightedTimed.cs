
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{

    public class Game_MeshBsplineWeightedTimed : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static Texture2D _dot;
        MouseState ms;

        Mesh_BsplineWeightedTimed mesh;
        float maxSelectableWeight = 9f;
        float selectedWeight = 1f;
        int numOfPoints = 40;
        int selectedCp = 0;
        bool isCurveClosed = false;
        bool isUniformedUsed = true;
        bool showGeneratedTangentsPositions = false;
        float cycledTime = 0f;
        float gameTimeInSeconds = 0;
        public int currentScrollWheelvalueForWeight = 0;
        Vector3 positionAtCycledTime = Vector3.Zero;
        string msg =
              $" " + "\n" +
              $" " + "\n"
              ;

        static Vector4 _wpOffset = new Vector4(350, 240, +5, 0);

        int cpWidth = 4;
        int cpHeight = 6;
        Vector4[] meshPoints = new Vector4[]
        {
             new Vector4(100, 200,  0, 1f) , new Vector4(300, 200,  0, 1f) , new Vector4(500, 200,  0, 1f) , new Vector4(700, 200,  0, 1f) ,
             new Vector4(100, 300,  0, 1f) , new Vector4(300, 300,  0, 1f) , new Vector4(500, 300,  0, 1f) , new Vector4(700, 300,  0, 1f) ,
             new Vector4(100, 400,  0, 1f) , new Vector4(300, 400,  0, 1f) , new Vector4(500, 400,  0, 1f) , new Vector4(700, 400,  0, 1f) ,
             new Vector4(100, 500,  0, 1f) , new Vector4(300, 500,  0, 1f) , new Vector4(500, 500,  0, 1f) , new Vector4(700, 500,  0, 1f),
             new Vector4(100, 600,  0, 1f) , new Vector4(300, 600,  0, 1f) , new Vector4(500, 600,  0, 1f) , new Vector4(700, 600,  0, 1f),
             new Vector4(100, 700,  0, 1f) , new Vector4(300, 700,  0, 1f) , new Vector4(500, 700,  0, 1f) , new Vector4(700, 700,  0, 1f)
        };


        public Game_MeshBsplineWeightedTimed()
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

            mesh = new Mesh_BsplineWeightedTimed(meshPoints, cpWidth, cpHeight, numOfPoints, isCurveClosed, isUniformedUsed);
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
                //curve._showTangents = showGeneratedTangentsPositions;
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
            if (IsPressedWithDelay(Keys.Up, gameTime) || ms.ScrollWheelValue > currentScrollWheelvalueForWeight)
            {
                selectedWeight += .05f;
                if (selectedWeight > maxSelectableWeight)
                    selectedWeight = -1f;
                meshPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalueForWeight = ms.ScrollWheelValue;
                redoCurve = true;
            }

            // adjust weight
            if (IsPressedWithDelay(Keys.Down, gameTime) || ms.ScrollWheelValue < currentScrollWheelvalueForWeight)
            {
                selectedWeight -= .05f;
                if (selectedWeight < -1f)
                    selectedWeight = maxSelectableWeight;
                meshPoints[selectedCp].W = selectedWeight;
                currentScrollWheelvalueForWeight = ms.ScrollWheelValue;
                redoCurve = true;
            }

            // adjust control point position.
            if (ms.LeftButton == ButtonState.Pressed)
            {
                meshPoints[selectedCp] = new Vector4(ms.Position.X, ms.Position.Y, 0, meshPoints[selectedCp].W);
                redoCurve = true;
            }

            // select a control point.
            if (ms.RightButton == ButtonState.Pressed)
                CheckPointSelected();

            if (redoCurve)
                mesh = new Mesh_BsplineWeightedTimed(meshPoints, cpWidth, cpHeight, numOfPoints, isCurveClosed, isUniformedUsed);

            string msg2 = "Open";
            if (isCurveClosed)
                msg2 = "Closed";
            string msg3 = "NonUniform";
            if (isUniformedUsed)
                msg3 = "Uniform";
            msg =
                $"GameTime In Seconds {gameTimeInSeconds.ToString("#########0.00")}  Cycle time over curve: {cycledTime.ToString("###0.00")}" +
                $"\n" + $"Bezier Mesh" +
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
            //if (isUniformedUsed)
            //    positionAtCycledTime = curve.GetUniformSplinePoint(cycledTime);
            //else
            //    positionAtCycledTime = curve.GetNonUniformSplinePoint(cycledTime);
        }

        public void CheckPointSelected()
        {
            int size = 10;
            var checkedRect = new Rectangle(ms.Position.X - size / 2, ms.Position.Y - size / 2, size, size);
            for (int i = 0; i < meshPoints.Length; i++)
                if (checkedRect.Contains(new Vector2(meshPoints[i].X, meshPoints[i].Y)))
                    selectedCp = i;
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            mesh.DrawWithSpriteBatch(_spriteBatch, _font, gameTime);
            DrawHelpers.DrawCrossHair(positionAtCycledTime.ToVector2(), 5, Color.White);
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


