using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace AllAboutSplinesCurvesAndNurbs_DX_
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        public static Texture2D _generatedTexture;
        public static Texture2D _dot;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        public static BasicEffect _basicEffect;
        List<VertexPositionColorTexture> _verticeList = new List<VertexPositionColorTexture>();
        VertexPositionColorTexture[] _vertices;

        // rectangle textures to draw.
        Rectangle r0 = new Rectangle(0, 0, 50, 50);
        Rectangle r1 = new Rectangle(0, 60, 50, 50);
        Rectangle r2 = new Rectangle(60, 0, 50, 50);
        Rectangle r3 = new Rectangle(100, 100, 50, 50);
        Rectangle r4 = new Rectangle(210, 200, 450, 260);

        #region camera variables.

        DemoCamera _camera;
        bool _useDemoWaypoints = true;
        static Vector3 _testTarget = new Vector3(350, 300, -25);

        static Vector3 _wpOffset = new Vector3(350, 240, -25);
        Vector3[] _wayPoints = new Vector3[]
        {
            new Vector3(-180, 10, -5) + _wpOffset, new Vector3(-150, 120, -5) + _wpOffset, new Vector3(-120, 30, -5) + _wpOffset, new Vector3(-90, 120, -5) + _wpOffset, new Vector3(-60, 120, -5) + _wpOffset, new Vector3(-30, 120, -5) + _wpOffset, 
            new Vector3(0, 200, -5) + _wpOffset, new Vector3(30, 120, -5) + _wpOffset, new Vector3(60, 30, -5) + _wpOffset, new Vector3(90, 120, -5) + _wpOffset, new Vector3(120, 120, -5) + _wpOffset, new Vector3(150, 120, -5) + _wpOffset, 
            new Vector3(180, 10, -5) + _wpOffset
        };
        //Vector3[] _wayPoints = new Vector3[]
        //{
        //    new Vector3(120, 120, -5) + _wpOffset, new Vector3(120, -120, -5) + _wpOffset, new Vector3(-120, -120, -5) + _wpOffset, new Vector3(-120, 120, -5) + _wpOffset,
        //};
        //Vector3[] _wayPoints = new Vector3[]
        //{
        //    new Vector3(120, 120, -5) + _wpOffset, new Vector3(120, -120, -5) + _wpOffset, new Vector3(-120, -120, -5) + _wpOffset, new Vector3(-120, 120, -5) + _wpOffset,
        //    new Vector3(250, 50, -5) + _wpOffset, new Vector3(250, -50, -5) + _wpOffset, new Vector3(-200, -200, -200) + _wpOffset, new Vector3(-150, 150, -5) + _wpOffset,
        //     new Vector3(50, 160, -5) + _wpOffset
        //};

        #endregion

        public Game1()
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
            _basicEffect = new BasicEffect(this.GraphicsDevice);
            _generatedTexture = CreateCheckerBoard(GraphicsDevice, 20,20, Color.White, Color.Red);
            _dot = CreateDotTexture(GraphicsDevice, Color.White);

            _camera = new DemoCamera(GraphicsDevice, _spriteBatch, _dot, new Vector3(2, 2, 10), new Vector3(0, 0, 0), Vector3.UnitY, 0.1f, 10000f, 1f, true, false);
            _camera.TransformCamera(_camera.World.Translation, _testTarget, _camera.World.Up);
            _camera.Up = Vector3.Forward;
            _camera.WayPointCycleDurationInTotalSeconds = 20f;
            _camera.MovementSpeedPerSecond = 3f;
            _camera.SetWayPoints(_wayPoints, true, 100);

            CreateTriangles();
        }

        public void CreateTriangles()
        {
            AddVertexRectangleToBuffer(r0);
            AddVertexRectangleToBuffer(r1);
            AddVertexRectangleToBuffer(r2);
            AddVertexRectangleToBuffer(r3);
            AddVertexRectangleToBuffer(r4);
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

            DrawPrimitiveGeometry(gameTime);

            DrawSpriteBatches(gameTime);

            base.Draw(gameTime);
        }

        public void DrawPrimitiveGeometry(GameTime gameTime)
        {
            _basicEffect.Texture = _generatedTexture;
            DrawTriangles(GraphicsDevice);
        }

        public void DrawSpriteBatches(GameTime gameTime)
        {
            _spriteBatch.Begin();

            DrawCameraSceneWithSpriteBatch(gameTime);

            _camera.DrawCurveThruWayPointsWithSpriteBatch(gameTime);

            _spriteBatch.End();
        }

        public void DrawCameraSceneWithSpriteBatch(GameTime gameTime)
        {
            // current 2d rectangle positions on the orthographic xy plane.

            DrawHelpers.DrawRectangleOutline(r0, 1, Color.Gray);
            DrawHelpers.DrawRectangleOutline(r1, 1, Color.Gray);
            DrawHelpers.DrawRectangleOutline(r2, 1, Color.Gray);
            DrawHelpers.DrawRectangleOutline(r3, 1, Color.Gray);
            DrawHelpers.DrawRectangleOutline(r4, 1, Color.Gray);

            string msg =
              $" " + "\n" +
              $" " + "\n"
              ;
            _spriteBatch.DrawString(_font, msg, new Vector2(10, 20), Color.White);
        }

        public void SetUpBasicEffect()
        {
            if(_basicEffect.Texture == null)
               _basicEffect.Texture = _generatedTexture;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.TextureEnabled = true;
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = _camera.View;
            _basicEffect.Projection = _camera.Projection;
        }
        public void SetStates()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }

        public void AddVertexRectangleToBuffer(Rectangle r)
        {
            if(GraphicsDevice.RasterizerState != RasterizerState.CullClockwise)
            {
                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f)));  // p1
                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Left, r.Bottom, 0f), Color.Red, new Vector2(0f, 1f))); // p0
                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f)));// p3

                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f)));// p3
                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Right, r.Top, 0f), Color.Blue, new Vector2(1f, 0f)));// p2
                _verticeList.Add(new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f))); // p1
            }
            _vertices = _verticeList.ToArray();
        }

        public void DrawTriangles(GraphicsDevice device)
        {
            if (_vertices != null)
            {
                SetStates();
                SetUpBasicEffect();
                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    int numberOfTriangles = _vertices.Length / 3;
                    this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, numberOfTriangles);
                }
            }
        }

        public static Texture2D CreateCheckerBoard(GraphicsDevice device, int w, int h, Color c0, Color c1)
        {
            Color[] data = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color c = c0;
                    if ((y % 2 == 0))
                    {
                        if ((x % 2 == 0))
                            c = c0;
                        else
                            c = c1;
                    }
                    else
                    {
                        if ((x % 2 == 0))
                            c = c1;
                        else
                            c = c0;
                    }
                    data[index] = c;
                }
            }
            Texture2D tex = new Texture2D(device, w, h);
            tex.SetData<Color>(data);
            return tex;
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
