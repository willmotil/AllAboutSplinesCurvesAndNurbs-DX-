using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{

    #region Class Extension Helpers 

    public static class MgExt
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector4 ToVector4(this Vector3 v)
        {
            return new Vector4(v.X, v.Y, v.Z, 1f);
        }
        public static Vector4 ToVector4(this Vector3 v, float w)
        {
            return new Vector4(v.X, v.Y, v.Z, w);
        }

        public static string VectorToString(this Vector4 v, string message)
        {
            string f = "+###0.0;-###0.0";
            return "\n " + message + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f) + "  " + v.Z.ToString(f) + "  " + v.W.ToString(f);
        }
        public static string VectorToString(this Vector3 v, string message)
        {
            string f = "+###0.0;-###0.0";
            return "\n " + message + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f) + "  " + v.Z.ToString(f);
        }
        public static string VectorToString(this Vector2 v, string message)
        {
            string f = "+###0.0;-###0.0";
            return "\n " + message + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f);
        }
        public static string VectorToString(this Vector4 v)
        {
            string f = "+###0.0;-###0.0";
            return " " + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f) + "  " + v.Z.ToString(f) + "  " + v.W.ToString(f);
        }
        public static string VectorToString(this Vector3 v)
        {
            string f = "+###0.0;-###0.0";
            return " " + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f) + "  " + v.Z.ToString(f);
        }
        public static string VectorToString(this Vector2 v)
        {
            string f = "+###0.0;-###0.0";
            return " " + "  " + v.X.ToString(f) + "  " + v.Y.ToString(f);
        }

        /// <summary>
        /// Allows a position to be inflected against a unit normal and any position on its surface plane. 
        /// This is useful in mirroring positions across a plane 
        /// When for example, you want to find in a water reflection cameras inflected position.
        /// </summary>
        public static Vector3 InflectPositionFromPlane(this Vector3 theCameraPostion, Vector3 thePlanesSurfaceNormal, Vector3 anyPositionOnThatSurfacePlane)
        {
            // the dot product also gives the length. 
            // when placed againsts a unit normal so any unit n * a distance is the distance to that normals plane no matter the normals direction. 
            // i didn't know that relation was so straight forward.
            float camToPlaneDist = Vector3.Dot(thePlanesSurfaceNormal, theCameraPostion - anyPositionOnThatSurfacePlane);
            return theCameraPostion - thePlanesSurfaceNormal * camToPlaneDist * 2;
        }

        /// <summary>
        /// Creates a world with a target.
        /// </summary>
        public static Matrix CreateWorldToTarget(this Matrix m, Vector3 position, Vector3 targetPosition, Vector3 up)
        {
            return Matrix.CreateWorld(position, targetPosition - position, up);
        }

        /// <summary>
        /// Display matrix
        /// </summary>
        public static string DisplayMatrix(this Matrix m, string name)
        {
            string f = "##0.###"; //"+000.000;-000.000";
            return name +=
                "\n { " + m.M11.ToString(f) + ", " + m.M12.ToString(f) + ", " + m.M13.ToString(f) + ", " + m.M14.ToString(f) + " }" +
                "\n { " + m.M21.ToString(f) + ", " + m.M22.ToString(f) + ", " + m.M23.ToString(f) + ", " + m.M24.ToString(f) + " }" +
                "\n { " + m.M31.ToString(f) + ", " + m.M32.ToString(f) + ", " + m.M33.ToString(f) + ", " + m.M34.ToString(f) + " }" +
                "\n { " + m.M41.ToString(f) + ", " + m.M42.ToString(f) + ", " + m.M43.ToString(f) + ", " + m.M44.ToString(f) + " }";
        }

        /// <summary>
        /// Creates a opposite monogame createlookat version or a left handed matrix.
        /// This returns a matrix suitable for a render target cube.
        /// </summary>
        public static Matrix CreateLhLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = -Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(-vector, vector2);
            Matrix result = Matrix.Identity;
            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = 0f;
            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = 0f;
            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = 0f;
            result.M41 = -Vector3.Dot(vector2, cameraPosition);
            result.M42 = -Vector3.Dot(vector3, cameraPosition);
            result.M43 = -Vector3.Dot(vector, cameraPosition);
            result.M44 = 1f;
            return result;
        }

        /// <summary>
        /// This returns a matrix suitable for a render target cube.
        /// </summary>
        public static Matrix CreateCubeFaceLookAtViewMatrix(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = -Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(-vector, vector2);
            Matrix result = Matrix.Identity;
            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = 0f;
            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = 0f;
            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = 0f;
            result.M41 = -Vector3.Dot(vector2, cameraPosition);
            result.M42 = -Vector3.Dot(vector3, cameraPosition);
            result.M43 = -Vector3.Dot(vector, cameraPosition);
            result.M44 = 1f;
            return result;
        }

        /// <summary>
        /// This returns a perspective projection matrix suitable for a rendertarget cube
        /// </summary>
        public static Matrix GetRenderTargetCubeProjectionMatrix(float near, float far)
        {
            return Matrix.CreatePerspectiveFieldOfView((float)MathHelper.Pi * .5f, 1, near, far);
        }

        /// <summary>
        /// Takes a screen position Point and reurns a ray in world space using viewport . unproject(...) , 
        /// The near and far are the z plane depth values used and found in your projection matrix.
        /// </summary>
        public static Ray GetScreenPointAsRayInto3dWorld(this Point screenPosition, Matrix projectionMatrix, Matrix viewMatrix, Matrix world, float near, float far, GraphicsDevice device)
        {
            return GetScreenVector2AsRayInto3dWorld(screenPosition.ToVector2(), projectionMatrix, viewMatrix, world, near, far, device);
        }

        /// <summary>
        /// Or not ?
        /// Takes a screen position Vector2 and reurns a ray in world space using viewport . unproject(...) , 
        /// The near and far are the z plane depth values used and found in your projection matrix.
        /// </summary>
        public static Ray GetScreenVector2AsRayInto3dWorld(this Vector2 screenPosition, Matrix projectionMatrix, Matrix viewMatrix, Matrix world, float near, float far, GraphicsDevice device)
        {
            Vector3 farScreenPoint = new Vector3(screenPosition.X, screenPosition.Y, far); // the projection matrice's far plane value.
            Vector3 nearScreenPoint = new Vector3(screenPosition.X, screenPosition.Y, near); // must be more then zero.
            Vector3 nearWorldPoint = device.Viewport.Unproject(nearScreenPoint, projectionMatrix, viewMatrix, world);
            Vector3 farWorldPoint = device.Viewport.Unproject(farScreenPoint, projectionMatrix, viewMatrix, world);
            Vector3 worldRaysNormal = Vector3.Normalize(farWorldPoint - nearWorldPoint);

            return new Ray(nearWorldPoint, worldRaysNormal);
        }

    }

    #endregion

    /// <summary>
    /// will motil 2018 , quick demo and testing helper class
    /// </summary>
    public static class BasicStuff
    {
        static BasicFps fps;
        public static CyclePerSeconds oscillator;
        public static BasicCamera camera;
        /// <summary>
        /// Basic stuff i get sick of typing over and over.
        /// </summary>
        public static void GameBasics(Game pass_this)
        {
            var g = pass_this;
            GameWindow w = g.Window;
            g.IsMouseVisible = true;
            w.AllowUserResizing = true;
        }
        // extensions
        public static SpriteBatch LoadBasics(this Game pthis) { return Load("LoadBasics MonoGame Test", pthis); }
        public static SpriteBatch LoadBasics(this Game pthis, string gameName) { return Load(gameName, pthis); }

        /// <summary>
        /// Basic stuff to avoid repitition.
        /// </summary>
        public static SpriteBatch Load(string gameName, Game pthis)
        {
            var g = pthis;
            GameWindow w = g.Window;
            BasicTextures.Load(pthis.GraphicsDevice);
            if (gameName != null && gameName.Length > 0)
                w.Title = gameName;
            else
                w.Title = "MonoGame Example";
            g.IsMouseVisible = true;
            w.AllowUserResizing = true;
            fps = new BasicFps();
            oscillator = new CyclePerSeconds(1f);
            camera = new BasicCamera();
            return new SpriteBatch(g.GraphicsDevice);
        }
        public static void UnloadBasics(this Game pthis)
        {
            BasicTextures.Dispose();
        }

        /// <summary>
        /// Updates basic stuff returns a camera view matrix if desired.
        /// </summary>
        public static Matrix Update(GameTime gameTime)
        {
            fps.Update(gameTime);
            oscillator.Update(gameTime);
            camera.UpdateUiActions(gameTime);
            return camera.View;
        }
        /// <summary>
        /// Note this is the simple fps class it will generate garbage.
        /// At least without my stringbuilder wrapper.
        /// </summary>
        public static void Draw(SpriteBatch sb, SpriteFont font)
        {
            fps.DrawFps(sb, font, Vector2.Zero, Color.MonoGameOrange);
        }
        static RasterizerState rs = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
        public static void DrawBeginEnd(GraphicsDevice gd, SpriteBatch sb, SpriteFont font)
        {
            gd.RasterizerState = rs;
            sb.Begin(SpriteSortMode.Immediate, null, null, null, rs, null, null);
            fps.DrawFps(sb, font, Vector2.Zero, Color.MonoGameOrange);
            sb.End();
            gd.RasterizerState = rs;
        }
    }

    public static class BasicTextures
    {
        public static Texture2D dotTexture;
        public static Texture2D red;
        public static Texture2D blue;
        public static Texture2D green;
        public static Texture2D yellow;
        public static Texture2D orange;
        public static Texture2D moccasin;
        public static Texture2D cornflowerblue;
        public static Texture2D white;
        public static Texture2D black;
        public static Texture2D aqua;
        public static Texture2D grid;
        public static Texture2D checkerBoard;

        public static bool wasDisposed = true;

        /// <summary>
        /// Don't forget to call dispose() probably in unload.
        /// </summary>
        public static void Load(GraphicsDevice gd)
        {
            CreateDotTextures(gd);
        }

        /// <summary>
        /// Don't forget to call dispose() on this class.
        /// </summary>
        public static void CreateDotTextures(GraphicsDevice device)
        {
            wasDisposed = false;
            if (dotTexture == null)
                dotTexture = TextureDotCreate(device, Color.MonoGameOrange);
            if (checkerBoard == null)
                checkerBoard = CreateCheckerBoard(device, 64, 64, Color.Moccasin, Color.CornflowerBlue);
            if (grid == null)
                grid = CreateGrid(device, 196, 196, Color.LightSkyBlue, Color.Green, Color.DarkGray);
            if (red == null)
            {
                red = TextureDotCreate(device, Color.Red);
                blue = TextureDotCreate(device, Color.Blue);
                green = TextureDotCreate(device, Color.Green);
                yellow = TextureDotCreate(device, Color.Yellow);
                orange = TextureDotCreate(device, Color.MonoGameOrange);
                moccasin = TextureDotCreate(device, Color.Moccasin);
                cornflowerblue = TextureDotCreate(device, Color.CornflowerBlue);
                white = TextureDotCreate(device, Color.White);
                black = TextureDotCreate(device, Color.Black);
                aqua = TextureDotCreate(device, Color.Aqua);
            }
        }

        public static Texture2D TextureDotCreate(GraphicsDevice device, Color c)
        {
            Color[] data = new Color[1];
            data[0] = c;

            return TextureFromColorArray(device, data, 1, 1);
        }

        public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
        {
            Texture2D tex = new Texture2D(device, width, height);
            tex.SetData<Color>(data);
            return tex;
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
            return TextureFromColorArray(device, data, w, h);
        }

        public static Texture2D CreateGrid(GraphicsDevice device, int w, int h, Color c0, Color c1, Color c2)
        {
            Color[] data = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color c = c2;
                    if ((y % 4 == 0))
                        c = c0;
                    if ((x % 4 == 0))
                        c = c1;
                    if ((x % 4 == 0) && (y % 4 == 0))
                    {
                        var r = (c0.R + c1.R) / 2;
                        var g = (c0.R + c1.R) / 2;
                        var b = (c0.R + c1.R) / 2;
                        c = new Color(r, g, b, 255);
                    }
                    data[index] = c;
                }
            }
            return TextureFromColorArray(device, data, w, h);
        }

        public static void Dispose()
        {
            if (wasDisposed == false)
            {
                if (dotTexture != null)
                {
                    if (dotTexture.IsDisposed == false)
                    {
                        dotTexture.Dispose();
                    }
                }
                if (red != null)
                {
                    if (red.IsDisposed == false)
                    {
                        red.Dispose();
                        green.Dispose();
                        blue.Dispose();
                        yellow.Dispose();
                        orange.Dispose();
                        moccasin.Dispose();
                        cornflowerblue.Dispose();
                        white.Dispose();
                        black.Dispose();
                        aqua.Dispose();
                    }
                }
                if (checkerBoard != null)
                {
                    if (checkerBoard.IsDisposed == false)
                    {
                        checkerBoard.Dispose();
                    }
                }
                if (grid != null)
                {
                    if (grid.IsDisposed == false)
                    {
                        grid.Dispose();
                    }
                }
            }
        }
    }

    public class BasicFps
    {
        private double frames = 0;
        private double updates = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        public double msgFrequency = 1.0f;
        public string msg = "";

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Short Answer can do it like this time elapsed per frame. 
            // This is delta time to keep things straight for movement.
            // elapsedThisFrame = (double)(gameTime.ElapsedGameTime.TotalSeconds); 

            // You can do this if your adding up elapsed time to do something like a timer or countdown or up till you display a msg.
            // elapsedTimeCumulative += (double)(gameTime.ElapsedGameTime.TotalSeconds); 

            // I do this just because i usually want to get the time now as well.
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = (double)(now - last);
            if (elapsed > msgFrequency)
            {
                msg = " Fps: " + (frames / elapsed).ToString() + "\n Elapsed time: " + elapsed.ToString() + "\n Updates: " + updates.ToString() + "\n Frames: " + frames.ToString();
                elapsed = 0;
                frames = 0;
                updates = 0;
                last = now;
            }
            updates++;
        }

        public void DrawFps(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            spriteBatch.DrawString(font, msg, fpsDisplayPosition, fpsTextColor);
            frames++;
        }
    }

    public class BasicCamera
    {
        Vector3 position = new Vector3(.1f, .1f, 1f);
        Vector3 forward = Vector3.Forward;
        Matrix camera = Matrix.Identity;
        Matrix cameraTarget = Matrix.Identity;
        Matrix view = Matrix.Identity;
        float elapsed = 0f;

        float movespeed = 2f;
        float rotspeed = (float)(Math.PI) * .5f;

        float pi = (float)Math.PI;
        float axisHeightOrYrot = 0.0f;
        float distanceToTarget = 0.7233f;
        int cameraTypeOption = 0;

        float orbitalRotationAmount = 0f;
        float designatedAxisXrot = 0.01f;
        float designatedAxisYrot = 0.01f;

        public BasicCamera()
        {
            CameraType(0);
            camera = Matrix.CreateWorld(new Vector3(.1f, .1f, 1f), Vector3.Forward, Vector3.Up);
            cameraTarget = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }

        public const int CAMOPTION_MOVEMENT_FIXEDUP = 0;
        public const int CAMOPTION_MOVEMENT_FREE = 1;
        public const int CAMOPTION_CYLINDRICAL_HEIGHT = 2;
        public const int CAMOPTION_POLAR = 3;
        public void CameraType(int options)
        {
            cameraTypeOption = options;
        }

        public Vector3 Position
        {
            set
            {
                position = value;
                camera.Translation = position;
            }
        }
        public void SetTarget(Vector3 position, float distance, float height)
        {
            cameraTarget = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }
        // todo
        public void SetTargetAxis(Vector3 rotationAxis, Vector3 position, float distance, float height)
        {
            cameraTarget = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }
        public void SetTargetRotationDistanceHeight(float rotation, float distance, float height)
        {
            distanceToTarget = distance;
        }

        public void UpdateUiActions(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (cameraTypeOption > 1 && cameraTypeOption < 4)
            {
                UpdateOtherCams(gameTime);
            }
            else
            {
                UpdateMovementCam(gameTime);
            }
        }

        void UpdateMovementCam(GameTime gameTime)
        {
            // key presses

            Vector3 rotationalMoment = Vector3.Zero;

            //if (Keyboard.GetState().IsKeyDown(Keys.D1))
            //    movementSphericalorCylindricalCamera = 1;
            //if (Keyboard.GetState().IsKeyDown(Keys.D2))
            //    movementSphericalorCylindricalCamera = 2;
            //if (Keyboard.GetState().IsKeyDown(Keys.D3))
            //    movementSphericalorCylindricalCamera = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                rotationalMoment.Y += rotspeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                rotationalMoment.Y += -rotspeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                rotationalMoment.X += rotspeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                rotationalMoment.X += -rotspeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.C))
                rotationalMoment.Z += rotspeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                rotationalMoment.Z += -rotspeed * elapsed;

            if (Keyboard.GetState().IsKeyDown(Keys.E))
                position += camera.Forward * movespeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                position += camera.Forward * -movespeed * elapsed;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                position += camera.Up * movespeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                position += camera.Up * -movespeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                position += camera.Right * -movespeed * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                position += camera.Right * +movespeed * elapsed;

            // free camera
            camera *=
                            Matrix.CreateFromAxisAngle(camera.Forward, rotationalMoment.Z) *
                            Matrix.CreateFromAxisAngle(camera.Up, rotationalMoment.X) *
                            Matrix.CreateFromAxisAngle(camera.Right, rotationalMoment.Y)
                            ;
            forward = camera.Forward;
            camera.Translation = position;
            if (cameraTypeOption == 0)
                camera = Matrix.CreateWorld(position, forward, Vector3.Up);
            else
                camera = Matrix.CreateWorld(position, forward, camera.Up);
            view = Matrix.CreateLookAt(camera.Translation, camera.Translation + camera.Forward, camera.Up);
        }

        //  ToDo... this isnt ready yet...
        void UpdateOtherCams(GameTime gameTime)
        {
            // key presses

            //if (Keyboard.GetState().IsKeyDown(Keys.D1))
            //    movementSphericalorCylindricalCamera = 1;
            //if (Keyboard.GetState().IsKeyDown(Keys.D2))
            //    movementSphericalorCylindricalCamera = 2;
            //if (Keyboard.GetState().IsKeyDown(Keys.D3))
            //    movementSphericalorCylindricalCamera = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                axisHeightOrYrot += .005f;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                axisHeightOrYrot += -.005f;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                orbitalRotationAmount += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                orbitalRotationAmount += -.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                distanceToTarget += .005f;
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                distanceToTarget += -.005f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                designatedAxisXrot += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                designatedAxisXrot += -.01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                designatedAxisYrot += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                designatedAxisYrot += -.01f;

            if (cameraTypeOption == 1)
                camera = Matrix.CreateScale(.1f) * OrbitTargetForwardAtHeight(cameraTarget, designatedAxisYrot * pi, distanceToTarget, designatedAxisXrot);
            if (cameraTypeOption == 2)
                camera = OrbitTargetAxis(cameraTarget, designatedAxisYrot * pi, designatedAxisXrot * pi, distanceToTarget * 2f);
            view = Matrix.CreateLookAt(camera.Translation, camera.Translation + camera.Forward, camera.Up);
        }
        public Matrix World
        {
            get { return camera; }
            set { camera = value; position = camera.Translation; forward = camera.Forward; }
        }
        public Matrix View
        {
            get { return view; }
        }
        public Matrix ObitalTarget
        {
            get { return cameraTarget; }
            set { cameraTarget = value; }
        }

        Matrix OrbitTargetForwardAtHeight(Matrix target, float ForwardOrbitallRotation, float DistanceFromTarget, float zHeightAboveTarget)
        {
            float flip = -1; // quick fix think my model is upside down
            var targetAxisHeightOffset = target.Forward * zHeightAboveTarget;
            var targetPos = target.Translation; // yes this is intentional, see position.
            Matrix axisTransformation = target * Matrix.CreateFromAxisAngle(target.Forward, ForwardOrbitallRotation);
            // i don't think this can happen anymore now so its probably redundant.
            float proximityToGimblePointRight = Math.Abs(Vector3.Dot(axisTransformation.Right, target.Forward));
            var positionalDistanceOffset = Vector3.Lerp(axisTransformation.Right, axisTransformation.Up, proximityToGimblePointRight) * DistanceFromTarget;
            var position = targetPos + positionalDistanceOffset + targetAxisHeightOffset;
            var gimbleSmothingLookAtOffset = Vector3.Zero;
            if (DistanceFromTarget < .5f)
                gimbleSmothingLookAtOffset = (1f * flip * target.Forward + 2f * target.Left + 1f * target.Up) * .1f * (.5f - DistanceFromTarget);
            var toTarget = (targetPos + gimbleSmothingLookAtOffset) - position;
            var lookUp = flip * target.Forward;
            return Matrix.CreateWorld(position, toTarget, lookUp);
        }

        Matrix OrbitTargetAxis(Matrix target, float ForwardOrbitallRotation, float UpAxisRotation, float DistanceFromTarget)
        {
            float flip = -1; // quick fix
            var Target = target * Matrix.CreateFromAxisAngle(target.Up, UpAxisRotation); // order matters its not commutative.
            var targetPos = Target.Translation; // yes this is intentional, see position.
            Matrix axisTransformation = Target * Matrix.CreateFromAxisAngle(Target.Forward, ForwardOrbitallRotation);
            float proximityToGimblePointRight = Math.Abs(Vector3.Dot(axisTransformation.Right, Target.Forward));
            var positionalDistanceOffset = Vector3.Lerp(axisTransformation.Right, axisTransformation.Up, proximityToGimblePointRight) * DistanceFromTarget;
            var position = targetPos + positionalDistanceOffset;
            var gimbleSmothingLookAtOffset = Vector3.Zero;
            if (DistanceFromTarget < .5f)
                gimbleSmothingLookAtOffset = (1f * flip * Target.Forward + 2f * Target.Left + 1f * Target.Up) * .1f * (.5f - DistanceFromTarget);
            var toTarget = (targetPos + gimbleSmothingLookAtOffset) - position;
            var lookUp = flip * Target.Forward;
            return Matrix.CreateWorld(position, toTarget, lookUp);
        }

        Matrix OrbitTargetAxisWithHeightOffset(Matrix target, float ForwardOrbitallRotation, float UpAxisRotation, float DistanceFromTarget, float zHeightAboveTarget)
        {
            float flip = -1; // quick fix
            var Target = target * Matrix.CreateFromAxisAngle(target.Up, UpAxisRotation); // order matters its not commutative.
            var targetAxisHeightOffset = target.Forward * zHeightAboveTarget;
            var targetPos = Target.Translation; // yes this is intentional, see position.
            Matrix axisTransformation = Target * Matrix.CreateFromAxisAngle(Target.Forward, ForwardOrbitallRotation);
            float proximityToGimblePointRight = Math.Abs(Vector3.Dot(axisTransformation.Right, Target.Forward));
            var positionalDistanceOffset = Vector3.Lerp(axisTransformation.Right, axisTransformation.Up, proximityToGimblePointRight) * DistanceFromTarget;
            var position = targetPos + positionalDistanceOffset + targetAxisHeightOffset;
            var gimbleSmothingLookAtOffset = Vector3.Zero;
            if (DistanceFromTarget < .5f)
                gimbleSmothingLookAtOffset = (1f * flip * Target.Forward + 2f * Target.Left + 1f * Target.Up) * .1f * (.5f - DistanceFromTarget);
            var toTarget = (targetPos + gimbleSmothingLookAtOffset) - position;
            var lookUp = flip * Target.Forward;
            return Matrix.CreateWorld(position, toTarget, lookUp);
        }
    }

    public class CyclePerSeconds
    {
        double last = 0;
        double now = 0;
        double elapsed = 0;
        double frequency = 1.0f;
        double PI2 = Math.PI * 2d;

        public CyclePerSeconds(float timeInSecondsToCompleteAFullCycle)
        {
            frequency = timeInSecondsToCompleteAFullCycle;
        }
        public void Update(GameTime gameTime)
        {
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = (double)(now - last);
            if (elapsed >= frequency)
            {
                elapsed = elapsed - frequency;
                last = now;
            }
        }
        /// <summary>
        /// Returns the completion percentage of a cycle for the seconds. 
        /// This value ranges from 0 to 1.
        /// </summary>
        public float GetFullCycleCompletionPercentage
        {
            get
            {
                return (float)(elapsed / frequency);
            }
        }
        /// <summary>
        /// Returns a oscillation value that spans from 0 to 1 to 0 for a cycle in the alloted seconds. 
        /// </summary>
        public float GetOscillation
        {
            get
            {
                var radians = (elapsed / frequency) * PI2;
                return (float)(Math.Cos(radians)) * .5f + .5f;
            }
        }
    }

    public class HardCodedSpriteFont
    {

        int width = 128;
        int height = 96;
        char defaultChar = Char.Parse("*");
        int lineHeightSpaceing = 19;
        float spaceing = 0;

        public SpriteFont LoadHardCodeSpriteFont(GraphicsDevice device)
        {
            Texture2D t = DecodeToTexture(device, rleByteData, width, height);
            return new SpriteFont(t, bounds, croppings, chars, lineHeightSpaceing, spaceing, kernings, defaultChar);
        }

        private Texture2D DecodeToTexture(GraphicsDevice device, List<byte> rleByteData, int _width, int _height)
        {
            Color[] colData = DecodeDataRLE(rleByteData);
            Texture2D tex = new Texture2D(device, _width, _height);
            tex.SetData<Color>(colData);
            return tex;
        }

        private Color[] DecodeDataRLE(List<byte> rleByteData)
        {
            List<Color> colAry = new List<Color>();
            for (int i = 0; i < rleByteData.Count; i++)
            {
                var val = (rleByteData[i] & 0x7F) * 2;
                if (val > 252)
                    val = 255;
                Color color = new Color();
                if (val > 0)
                    color = new Color(val, val, val, val);
                if ((rleByteData[i] & 0x80) > 0)
                {
                    var runlen = rleByteData[i + 1];
                    for (int j = 0; j < runlen; j++)
                        colAry.Add(color);
                    i += 1;
                }
                colAry.Add(color);
            }
            return colAry.ToArray();
        }


        // Item count = 95
        List<char> chars = new List<char>
        {
        (char)32,(char)33,(char)34,(char)35,(char)36,(char)37,(char)38,(char)39,(char)40,(char)41,(char)42,(char)43,(char)44,(char)45,(char)46,(char)47,(char)48,(char)49,(char)50,(char)51,(char)52,(char)53,(char)54,(char)55,(char)56,(char)57,(char)58,(char)59,(char)60,(char)61,(char)62,(char)63,(char)64,(char)65,(char)66,(char)67,(char)68,(char)69,(char)70,(char)71,(char)72,(char)73,(char)74,(char)75,(char)76,(char)77,(char)78,(char)79,(char)80,(char)81,
        (char)82,(char)83,(char)84,(char)85,(char)86,(char)87,(char)88,(char)89,(char)90,(char)91,(char)92,(char)93,(char)94,(char)95,(char)96,(char)97,(char)98,(char)99,(char)100,(char)101,(char)102,(char)103,(char)104,(char)105,(char)106,(char)107,(char)108,(char)109,(char)110,(char)111,(char)112,(char)113,(char)114,(char)115,(char)116,(char)117,(char)118,(char)119,(char)120,(char)121,(char)122,(char)123,(char)124,(char)125,(char)126
        };

        List<Rectangle> bounds = new List<Rectangle>
        {
        new Rectangle(125,84,1,1),new Rectangle(110,73,1,12),new Rectangle(63,75,4,5),new Rectangle(47,19,11,12),new Rectangle(77,1,7,15),new Rectangle(1,19,14,12),new Rectangle(107,16,11,13),new Rectangle(91,60,1,5),new Rectangle(35,1,4,16),new Rectangle(41,1,4,16),
        new Rectangle(9,86,7,7),new Rectangle(1,75,9,9),new Rectangle(30,86,3,5),new Rectangle(63,82,5,1),new Rectangle(91,67,1,2),new Rectangle(19,1,6,16),new Rectangle(82,18,8,12),new Rectangle(91,73,5,12),new Rectangle(1,61,7,12),new Rectangle(10,61,7,12),new Rectangle(74,46,8,12),
        new Rectangle(19,61,7,12),new Rectangle(84,46,8,12),new Rectangle(28,61,7,12),new Rectangle(1,47,8,12),new Rectangle(11,47,8,12),new Rectangle(71,47,1,9),new Rectangle(124,59,3,12),new Rectangle(95,32,8,9),new Rectangle(47,86,9,4),new Rectangle(23,75,8,9),new Rectangle(55,61,6,12),
        new Rectangle(92,1,13,14),new Rectangle(107,1,12,13),new Rectangle(21,47,8,12),new Rectangle(14,33,10,12),new Rectangle(26,33,10,12),new Rectangle(31,47,8,12),new Rectangle(37,61,7,12),new Rectangle(105,31,11,12),new Rectangle(38,33,10,12),new Rectangle(105,73,3,12),new Rectangle(63,61,6,12),
        new Rectangle(41,47,8,12),new Rectangle(46,61,7,12),new Rectangle(82,32,11,12),new Rectangle(50,33,10,12),new Rectangle(17,19,13,12),new Rectangle(51,47,8,12),new Rectangle(62,1,13,15),new Rectangle(118,31,9,12),new Rectangle(61,47,8,12),new Rectangle(95,45,9,12),new Rectangle(62,33,10,12),
        new Rectangle(1,33,11,12),new Rectangle(32,19,13,12),new Rectangle(94,59,8,12),new Rectangle(106,45,9,12),new Rectangle(104,59,8,12),new Rectangle(47,1,4,16),new Rectangle(27,1,6,16),new Rectangle(53,1,4,16),new Rectangle(18,86,10,6),new Rectangle(89,87,9,1),new Rectangle(58,86,3,3),
        new Rectangle(116,84,7,9),new Rectangle(92,17,8,13),new Rectangle(74,33,6,9),new Rectangle(62,18,8,13),new Rectangle(63,85,7,9),new Rectangle(121,1,5,13),new Rectangle(114,59,8,12),new Rectangle(72,18,8,13),new Rectangle(113,73,1,12),new Rectangle(86,1,4,15),new Rectangle(120,16,7,13),
        new Rectangle(102,17,1,13),new Rectangle(116,73,11,9),new Rectangle(33,75,8,9),new Rectangle(43,75,8,9),new Rectangle(71,60,8,12),new Rectangle(81,60,8,12),new Rectangle(84,74,5,9),new Rectangle(81,85,6,9),new Rectangle(98,73,5,12),new Rectangle(53,75,8,9),new Rectangle(12,75,9,9),
        new Rectangle(71,74,11,9),new Rectangle(72,85,7,9),new Rectangle(117,45,9,12),new Rectangle(1,86,6,9),new Rectangle(1,1,7,16),new Rectangle(59,1,1,16),new Rectangle(10,1,7,16),new Rectangle(35,86,10,4)
        };

        List<Rectangle> croppings = new List<Rectangle>
        {
        new Rectangle(0,34,5,19),new Rectangle(0,4,4,19),new Rectangle(0,3,6,19),new Rectangle(0,4,12,19),new Rectangle(0,4,9,19),new Rectangle(0,4,16,19),new Rectangle(0,4,11,19),new Rectangle(0,3,3,19),new Rectangle(0,3,6,19),new Rectangle(0,3,6,19),
        new Rectangle(0,3,9,19),new Rectangle(0,6,12,19),new Rectangle(0,14,5,19),new Rectangle(0,10,6,19),new Rectangle(0,14,5,19),new Rectangle(0,3,6,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),
        new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,4,9,19),new Rectangle(0,7,6,19),new Rectangle(0,7,6,19),new Rectangle(0,6,12,19),new Rectangle(0,9,12,19),new Rectangle(0,6,12,19),new Rectangle(0,4,8,19),
        new Rectangle(0,4,15,19),new Rectangle(0,3,11,19),new Rectangle(0,4,9,19),new Rectangle(0,4,10,19),new Rectangle(0,4,11,19),new Rectangle(0,4,9,19),new Rectangle(0,4,8,19),new Rectangle(0,4,11,19),new Rectangle(0,4,11,19),new Rectangle(0,4,6,19),new Rectangle(0,4,7,19),
        new Rectangle(0,4,9,19),new Rectangle(0,4,8,19),new Rectangle(0,4,12,19),new Rectangle(0,4,11,19),new Rectangle(0,4,12,19),new Rectangle(0,4,9,19),new Rectangle(0,4,12,19),new Rectangle(0,4,10,19),new Rectangle(0,4,9,19),new Rectangle(0,4,10,19),new Rectangle(0,4,11,19),
        new Rectangle(0,4,10,19),new Rectangle(0,4,14,19),new Rectangle(0,4,9,19),new Rectangle(0,4,10,19),new Rectangle(0,4,9,19),new Rectangle(0,3,6,19),new Rectangle(0,3,6,19),new Rectangle(0,3,6,19),new Rectangle(0,4,12,19),new Rectangle(0,17,9,19),new Rectangle(0,3,9,19),
        new Rectangle(0,7,8,19),new Rectangle(0,3,9,19),new Rectangle(0,7,7,19),new Rectangle(0,3,9,19),new Rectangle(0,7,8,19),new Rectangle(0,3,5,19),new Rectangle(0,7,9,19),new Rectangle(0,3,9,19),new Rectangle(0,4,4,19),new Rectangle(0,4,5,19),new Rectangle(0,3,8,19),
        new Rectangle(0,3,4,19),new Rectangle(0,7,14,19),new Rectangle(0,7,9,19),new Rectangle(0,7,9,19),new Rectangle(0,7,9,19),new Rectangle(0,7,9,19),new Rectangle(0,7,6,19),new Rectangle(0,7,7,19),new Rectangle(0,4,5,19),new Rectangle(0,7,9,19),new Rectangle(0,7,8,19),
        new Rectangle(0,7,12,19),new Rectangle(0,7,8,19),new Rectangle(0,7,8,19),new Rectangle(0,7,7,19),new Rectangle(0,3,8,19),new Rectangle(0,3,6,19),new Rectangle(0,3,8,19),new Rectangle(0,9,12,19)
        };

        List<Vector3> kernings = new List<Vector3>
        {
        new Vector3(0,0,5),new Vector3(1,1,2),new Vector3(1,4,1),new Vector3(0,11,1),new Vector3(1,7,1),new Vector3(1,14,1),new Vector3(0,11,0),new Vector3(1,1,1),new Vector3(1,4,1),new Vector3(1,4,1),
        new Vector3(1,7,1),new Vector3(1,9,2),new Vector3(1,3,1),new Vector3(0,5,1),new Vector3(2,1,2),new Vector3(0,6,0),new Vector3(0,8,1),new Vector3(2,5,2),new Vector3(1,7,1),new Vector3(1,7,1),new Vector3(0,8,1),
        new Vector3(1,7,1),new Vector3(0,8,1),new Vector3(1,7,1),new Vector3(0,8,1),new Vector3(0,8,1),new Vector3(2,1,3),new Vector3(1,3,2),new Vector3(2,8,2),new Vector3(2,9,1),new Vector3(2,8,2),new Vector3(1,6,1),
        new Vector3(1,13,1),new Vector3(-1,12,0),new Vector3(0,8,1),new Vector3(-1,10,1),new Vector3(0,10,1),new Vector3(0,8,1),new Vector3(0,7,1),new Vector3(-1,11,1),new Vector3(0,10,1),new Vector3(1,3,2),new Vector3(0,6,1),
        new Vector3(0,8,1),new Vector3(0,7,1),new Vector3(0,11,1),new Vector3(0,10,1),new Vector3(-1,13,0),new Vector3(0,8,1),new Vector3(-1,13,0),new Vector3(0,9,1),new Vector3(0,8,1),new Vector3(0,9,1),new Vector3(0,10,1),
        new Vector3(-1,11,0),new Vector3(0,13,1),new Vector3(0,8,1),new Vector3(0,9,1),new Vector3(0,8,1),new Vector3(1,4,1),new Vector3(0,6,0),new Vector3(1,4,1),new Vector3(1,10,1),new Vector3(0,9,0),new Vector3(2,3,4),
        new Vector3(0,7,1),new Vector3(0,8,1),new Vector3(0,6,1),new Vector3(0,8,1),new Vector3(0,7,1),new Vector3(0,5,0),new Vector3(0,8,1),new Vector3(0,8,1),new Vector3(1,1,2),new Vector3(0,4,1),new Vector3(0,7,1),
        new Vector3(1,1,2),new Vector3(1,11,2),new Vector3(0,8,1),new Vector3(0,8,1),new Vector3(0,8,1),new Vector3(0,8,1),new Vector3(0,5,1),new Vector3(0,6,1),new Vector3(0,5,0),new Vector3(0,8,1),new Vector3(-1,9,0),
        new Vector3(0,11,1),new Vector3(0,7,1),new Vector3(-1,9,0),new Vector3(0,6,1),new Vector3(0,7,1),new Vector3(2,1,3),new Vector3(0,7,1),new Vector3(1,10,1)
        };

        // pixelsCompressed: 12288 bytesTallied: 49152 byteDataCount: 6203

        List<byte> rleByteData = new List<byte>
        {
        128,131,9,80,117,127,128,1,127,117,80,9,128,8,18,107,128,1,106,20,128,7,24,71,128,1,68,24,128,3,255,3,128,1,255,3,128,1,127,128,4,30,85,115,125,117,89,36,128,7,127,128,7,127,128,4,24,80,112,124,118,96,51,2,128,8,1,128,8,8,87,122,121,128,5,82,68,10,128,3,10,69,81,128,8,58,67,128,1,63,60,128,6,19,100,5,128,1,5,101,18,128,2,127,128,7,127,128,1,127,128,3,59,113,49,13,2,11,45,107,74,128,6,127,128,7,127,128,3,64,89,39,12,2,9,36,92,99,9,128,6,21,95,100,21,128,6,73,70,5,128,6,118,10,128,5,11,117,128,8,98,27,128,1,20,99,128,6,102,40,128,3,46,100,128,2,127,128,7,127,128,1,127,128,2,32,113,11,128,4,6,103,47,128,2,2,64,112,127,116,
        71,42,128,8,62,49,128,6,69,92,128,6,68,50,54,68,128,6,111,13,128,7,127,128,7,127,128,7,11,113,128,3,104,12,128,4,36,108,128,4,1,108,34,128,1,127,128,7,127,128,1,127,128,2,92,47,128,6,30,106,128,2,72,61,9,127,14,46,103,128,2,255,2,128,1,21,83,128,1,26,94,123,121,98,127,1,101,37,128,4,1,110,8,10,111,1,128,5,124,1,128,6,5,125,128,7,125,4,128,6,50,75,128,3,62,52,128,4,72,58,128,5,57,70,128,1,127,128,7,127,128,1,127,128,1,2,122,7,128,7,117,12,128,1,118,5,0,127,128,7,127,128,1,76,38,0,27,111,40,6,9,41,127,0,47,85,128,4,34,88,128,1,90,33,128,4,255,3,128,5,32,104,128,7,104,30,128,6,91,35,128,3,19,91,128,4,101,28,128,5,28,
        100,128,1,127,128,7,127,128,1,127,128,1,11,116,128,8,100,27,128,1,113,38,0,127,128,7,127,128,1,109,13,0,94,42,128,3,127,0,16,112,128,4,81,43,128,1,45,80,128,5,127,128,5,2,30,105,35,128,7,35,104,30,2,128,3,6,116,2,128,4,101,7,128,3,115,15,128,5,15,114,128,1,127,128,7,127,128,1,127,128,1,11,116,128,8,100,24,128,1,44,123,88,127,24,128,6,127,128,1,123,3,0,122,10,128,3,127,0,3,124,128,3,5,115,4,128,1,5,115,5,128,4,127,128,5,255,1,37,128,9,37,255,1,128,3,43,83,128,5,61,44,128,3,123,5,128,5,5,123,128,1,127,128,7,127,128,1,127,128,1,2,122,6,128,7,117,8,128,2,9,45,127,111,110,27,128,4,127,128,1,123,3,0,120,6,128,3,127,0,5,123,
        128,3,46,80,128,3,81,46,128,4,127,128,5,3,35,107,24,128,7,24,107,34,3,128,3,83,42,128,5,18,83,128,3,123,4,128,5,5,123,128,1,127,128,7,127,128,1,127,128,2,92,45,128,6,30,102,128,5,127,0,50,108,128,4,127,128,1,110,14,0,100,31,128,3,127,0,15,107,128,3,93,255,5,93,128,4,127,128,7,41,92,128,7,93,40,128,4,2,116,6,128,6,96,3,128,2,115,13,128,5,15,114,128,1,127,128,7,127,128,1,127,128,2,33,113,12,128,4,6,104,43,128,5,127,0,3,118,128,4,127,128,1,80,41,0,44,103,23,3,24,83,127,0,40,73,128,2,13,115,2,128,3,1,114,13,128,3,127,128,7,9,120,128,7,120,7,128,4,35,90,128,7,60,36,128,2,102,25,128,5,28,101,128,1,127,128,7,127,128,1,127,
        128,3,61,114,51,13,2,11,44,107,64,128,3,101,42,13,127,8,40,68,128,4,127,128,1,29,91,128,1,50,107,124,109,49,118,255,1,25,128,2,59,73,128,5,72,58,128,3,127,128,8,127,128,7,127,128,5,75,50,128,7,17,75,128,2,73,56,128,5,58,72,128,1,127,128,7,127,128,1,127,128,4,31,87,116,126,127,68,19,128,4,53,86,120,127,105,55,1,128,4,127,128,2,79,55,128,12,105,28,128,5,27,105,128,3,127,128,8,126,1,128,5,1,126,128,5,113,11,128,8,89,128,2,37,107,128,4,1,108,36,128,1,127,128,7,127,128,1,127,128,8,114,18,128,8,127,128,6,9,117,128,2,3,84,87,34,8,128,8,24,110,128,7,110,24,128,2,127,128,8,111,14,128,5,12,111,128,4,28,98,128,9,59,28,128,1,1,102,45,128,3,
        45,102,128,2,127,128,7,127,128,1,127,128,8,71,67,8,128,7,127,128,5,8,70,82,128,4,40,91,117,126,122,109,128,30,196,1,11,128,3,13,67,68,128,4,68,58,128,9,16,67,128,2,19,101,4,128,1,4,101,19,128,2,127,128,7,127,128,1,127,128,8,4,72,115,126,117,128,5,127,128,4,124,119,84,9,128,41,5,74,116,127,128,1,127,116,74,5,128,4,107,18,128,10,81,128,3,24,68,128,1,68,24,128,3,255,3,128,1,255,3,128,1,127,128,47,6,74,114,119,86,12,128,5,127,128,98,127,128,8,127,128,4,80,68,8,10,54,94,128,5,127,128,75,127,128,1,127,128,9,22,92,249,1,91,20,128,2,127,128,8,127,128,4,120,5,128,1,5,122,128,5,127,128,7,6,77,224,1,78,6,128,2,37,90,128,7,31,87,116,125,116,
        87,31,128,4,113,15,128,2,40,127,45,128,2,15,113,128,4,16,115,128,2,20,112,128,11,127,128,1,127,128,8,12,113,48,134,1,48,112,10,128,1,127,128,8,127,128,4,112,29,128,1,36,94,128,5,127,128,7,72,60,128,1,60,71,128,1,3,105,18,128,6,61,112,48,12,2,13,49,112,60,128,3,83,43,128,2,67,117,73,128,2,43,84,128,4,48,84,128,2,52,80,128,11,127,128,1,127,128,8,65,63,128,3,60,62,128,1,127,128,8,127,128,4,47,114,167,1,96,15,128,5,127,128,3,66,58,128,1,112,12,128,1,12,112,128,1,58,69,128,6,32,112,11,128,4,11,112,32,128,2,54,71,128,2,95,61,101,128,2,70,55,128,4,80,52,128,2,84,48,128,11,127,128,1,127,128,8,99,23,128,3,22,98,128,1,127,19,84,119,123,100,
        34,128,2,127,128,4,11,102,127,90,1,0,2,127,128,3,127,128,2,66,48,128,2,124,2,128,1,2,123,0,12,108,6,128,6,92,45,128,6,46,90,128,2,24,99,128,1,1,117,7,121,4,128,1,97,27,128,2,255,2,254,3,255,2,128,4,5,67,113,124,112,75,127,128,1,127,16,75,114,123,105,47,128,2,118,7,128,3,11,117,128,1,127,118,57,12,7,38,114,29,128,1,127,128,3,20,105,41,32,123,62,0,10,114,128,3,127,128,1,65,39,128,3,112,12,128,1,12,111,0,80,47,128,6,2,122,6,128,6,7,121,1,128,1,1,117,3,0,22,97,0,98,29,0,2,118,2,128,3,17,112,128,2,18,110,128,5,8,105,66,18,2,9,58,127,128,1,127,105,54,15,2,15,89,50,128,1,125,1,128,3,3,125,128,1,127,14,128,3,47,90,
        128,1,127,128,3,93,36,128,1,35,124,62,45,93,128,3,127,0,69,31,128,4,73,58,128,1,60,71,27,99,7,77,224,1,78,6,128,1,11,116,128,8,116,11,128,2,93,27,0,49,67,0,66,57,0,24,97,128,4,48,81,128,2,50,79,128,5,72,68,128,4,127,128,1,127,1,128,3,19,108,128,1,125,1,128,3,2,125,128,1,127,128,4,13,118,128,1,127,128,3,122,5,128,2,39,125,122,48,128,3,127,39,87,59,128,4,7,79,224,1,78,7,99,27,72,60,128,1,60,71,128,1,11,116,128,8,116,11,128,2,64,55,0,77,37,0,35,85,0,51,68,128,4,79,48,128,2,82,48,128,5,113,19,128,4,127,128,1,127,128,4,3,126,128,1,118,6,128,3,9,117,128,1,127,128,4,4,125,128,1,127,128,3,112,37,128,3,58,127,62,128,
        3,127,3,5,103,33,128,8,48,79,0,112,12,128,1,12,112,128,1,2,122,6,128,6,6,121,1,128,2,34,83,0,103,8,0,6,110,0,79,39,128,4,110,16,128,2,113,17,128,5,125,4,128,4,127,128,1,127,128,5,127,128,1,99,21,128,3,26,98,128,1,127,128,4,19,111,128,1,127,128,3,53,115,41,7,8,46,112,72,127,56,128,2,127,128,1,16,107,15,128,6,6,108,12,0,124,2,128,1,2,123,128,2,92,45,128,6,45,91,128,3,6,109,6,104,128,2,101,12,106,10,128,2,255,2,254,3,255,2,128,4,117,13,128,4,127,128,1,127,128,5,127,128,1,64,63,128,3,192,1,128,1,127,128,4,64,71,128,1,127,128,4,41,100,121,119,87,23,0,55,127,51,128,1,127,128,2,32,99,3,128,5,69,58,128,1,112,12,128,1,12,
        111,128,2,34,112,11,128,4,11,112,33,128,4,103,43,76,128,2,70,47,108,128,4,48,84,128,2,52,80,128,6,87,48,128,3,11,127,128,1,127,128,5,127,128,1,11,113,48,134,1,46,112,10,128,1,127,45,21,5,16,61,105,8,128,1,127,128,12,3,11,128,1,127,128,3,54,77,128,4,19,105,2,128,1,73,58,128,1,60,71,128,3,61,113,49,12,2,12,49,113,59,128,5,74,98,46,128,2,39,101,81,128,4,80,52,128,2,84,48,128,6,25,113,40,8,11,52,114,127,128,1,127,128,5,127,128,2,22,92,122,121,92,21,128,2,125,70,110,124,115,75,9,128,2,127,128,28,90,37,128,2,7,79,224,1,78,6,128,4,32,88,117,125,116,88,32,128,6,44,127,16,128,2,8,126,52,128,4,112,20,128,2,115,16,128,7,30,97,123,121,89,
        26,127,128,1,127,128,5,127,128,155,28,81,112,124,123,111,89,56,128,1,255,2,125,110,61,128,85,127,106,1,128,4,7,118,127,128,7,33,96,128,3,69,102,38,8,3,12,32,62,108,128,1,127,128,1,4,23,77,63,128,3,12,118,2,128,4,2,118,12,128,4,35,91,118,125,117,95,60,128,1,255,2,124,116,95,55,5,128,3,127,128,7,127,128,1,127,61,128,6,127,128,1,127,128,7,127,128,2,33,100,123,110,69,128,1,127,81,46,128,4,59,80,127,128,5,34,97,100,37,128,2,51,95,3,128,8,127,128,3,19,117,128,4,96,35,128,4,36,96,128,4,62,106,38,5,6,20,53,104,128,1,127,128,1,4,18,47,99,107,17,128,2,127,128,7,127,128,1,127,108,30,128,5,127,128,1,127,128,7,127,128,1,26,116,37,5,27,92,128,1,127,
        22,108,1,128,2,3,114,21,127,128,3,36,99,98,36,128,3,1,114,18,128,9,127,128,3,7,122,128,4,54,76,128,4,77,54,128,3,33,108,7,128,7,127,128,5,64,106,4,128,1,127,128,7,127,128,1,127,24,109,9,128,4,127,128,1,127,128,7,127,128,1,86,52,128,5,127,0,89,48,128,2,48,87,0,127,128,1,37,100,97,34,128,5,26,101,128,10,127,128,3,38,97,128,4,13,114,1,128,2,1,116,12,128,3,93,42,128,8,127,128,6,93,49,128,1,127,128,7,127,128,1,127,0,48,89,128,4,127,128,1,127,128,7,127,128,1,117,14,128,5,127,0,29,110,2,128,1,106,27,0,127,128,1,127,64,128,7,43,83,128,10,127,128,1,9,36,109,30,128,5,97,30,128,2,32,97,128,3,2,122,5,128,8,127,128,6,42,89,128,1,127,
        128,7,127,128,1,127,128,1,77,57,128,3,127,128,1,127,128,7,127,128,1,126,3,128,5,127,128,1,98,49,0,37,95,128,1,127,128,1,37,100,97,33,128,5,43,85,128,3,255,4,128,1,255,3,119,22,128,6,55,71,128,2,73,55,128,3,11,116,128,9,127,128,6,22,105,128,1,255,9,128,1,127,128,1,3,99,27,128,2,127,128,1,127,128,7,127,128,1,118,13,128,5,127,128,1,38,111,2,95,34,128,1,127,128,3,36,99,98,35,128,3,28,105,128,7,127,128,1,127,128,2,73,58,128,6,13,110,128,2,113,13,128,3,11,116,128,9,127,128,6,22,104,128,1,127,128,7,127,128,1,127,128,2,16,102,7,128,1,127,128,1,127,128,7,127,128,1,90,49,128,5,127,128,2,105,75,102,128,2,127,128,5,35,98,100,37,128,1,2,119,25,
        128,6,127,128,1,127,128,2,2,95,32,128,6,98,24,0,27,98,128,4,2,122,5,128,8,127,128,6,40,87,128,1,127,128,7,127,128,1,127,128,3,37,85,128,1,127,128,1,126,128,7,125,128,1,32,115,34,4,25,93,128,1,127,128,2,46,127,42,128,2,127,128,7,33,96,128,2,60,106,10,128,5,127,128,1,127,128,3,11,103,14,128,5,56,65,0,69,56,128,5,95,43,128,8,127,128,6,88,45,128,1,127,128,7,127,128,1,127,128,4,65,53,0,127,128,1,115,9,128,5,9,115,128,2,39,104,125,113,69,128,1,127,128,8,127,128,12,1,80,114,53,16,2,5,15,38,127,128,1,127,128,4,28,97,3,128,4,14,105,0,109,14,128,5,38,109,7,128,7,127,128,5,58,101,2,128,1,127,128,7,127,128,1,127,128,4,1,89,24,127,128,
        1,83,52,128,5,53,82,128,9,127,128,8,127,128,14,37,87,114,125,121,102,68,25,128,1,127,128,5,51,77,128,5,99,43,99,128,7,69,105,37,133,1,20,52,103,128,1,127,128,1,4,15,42,94,106,15,128,2,127,128,7,127,128,1,127,128,5,10,94,127,128,1,17,114,62,19,131,1,20,64,114,18,128,9,127,128,8,127,128,39,57,116,57,128,8,39,94,119,126,117,94,59,128,1,255,2,125,118,98,56,5,128,3,127,128,7,127,128,1,127,128,6,27,124,128,2,13,74,110,252,1,110,75,13,128,151,255,8,128,1,90,53,128,4,54,89,128,1,18,115,2,128,2,2,115,18,128,80,52,127,128,4,20,79,113,125,121,128,7,127,128,5,19,111,4,128,2,5,111,17,128,2,94,41,128,2,40,94,128,4,29,93,118,122,93,27,128,3,16,87,122,
        120,85,14,128,2,255,2,126,117,84,11,128,2,255,7,128,1,127,128,4,50,66,128,1,255,2,125,115,81,22,128,3,26,88,114,124,111,86,64,128,1,127,128,5,43,83,127,128,3,38,104,40,13,128,9,127,128,6,69,57,128,2,61,64,128,3,42,91,128,2,89,42,128,3,45,96,30,6,7,32,97,31,128,1,15,112,45,8,13,63,114,12,128,1,127,128,1,2,12,58,91,128,2,127,128,8,127,128,3,51,72,128,2,127,128,1,3,19,61,118,29,128,1,42,108,36,7,5,21,54,104,128,1,127,128,4,35,89,3,127,128,2,11,103,7,128,11,127,128,6,7,105,6,0,8,103,4,128,3,2,114,14,0,12,113,2,128,3,114,11,128,3,14,110,128,1,83,48,128,3,62,69,128,1,127,128,3,8,123,128,2,127,128,8,127,128,2,51,78,128,3,
        127,128,4,44,97,128,1,110,25,128,12,27,94,5,0,127,128,2,62,48,128,12,127,128,7,48,62,0,68,39,128,5,67,63,0,59,66,128,4,117,26,128,3,23,109,128,1,119,11,128,3,24,105,128,1,127,128,3,16,111,128,2,127,128,8,127,128,1,51,84,1,128,3,127,128,4,5,122,128,1,122,7,128,11,21,97,9,128,1,127,128,2,99,62,99,120,119,97,33,128,7,127,128,8,92,21,85,128,6,16,111,1,106,15,128,4,53,118,56,8,0,32,104,34,128,1,123,4,128,3,6,121,128,1,127,128,1,1,20,90,47,128,2,127,128,8,127,0,52,89,2,128,4,127,128,4,15,117,128,1,95,61,128,10,15,98,14,128,2,127,128,2,118,73,27,135,1,39,112,37,128,6,127,128,8,27,117,18,128,7,91,64,91,128,6,42,127,125,111,108,20,
        128,2,102,33,128,3,1,126,128,1,255,4,95,21,128,2,255,7,128,1,127,52,97,4,128,5,127,128,4,59,74,128,1,21,113,99,67,61,53,11,128,5,97,19,128,3,127,128,2,125,128,4,36,101,128,6,127,128,9,127,128,8,40,127,39,128,5,24,113,48,14,48,50,101,16,128,1,38,112,38,134,1,27,73,123,128,1,127,128,2,6,35,101,39,128,1,127,128,8,127,94,111,15,128,5,127,128,1,5,20,64,104,6,128,2,5,44,62,71,100,122,32,128,4,255,7,128,1,121,6,128,3,4,123,128,6,127,128,9,127,128,8,17,113,1,128,5,97,40,128,3,59,95,128,2,34,97,120,121,99,61,112,128,1,127,128,4,20,107,128,1,127,128,8,127,2,38,104,9,128,4,255,2,121,102,60,5,128,8,59,104,128,1,127,128,7,127,128,2,104,24,
        128,3,12,119,128,6,127,128,9,127,128,8,69,63,128,6,123,6,128,3,6,123,128,7,44,81,128,1,127,128,4,3,123,128,1,127,128,8,127,128,1,37,96,5,128,3,127,128,14,6,124,128,1,127,128,7,127,128,2,68,59,128,3,48,86,128,6,127,128,9,127,128,7,5,117,13,128,6,109,35,128,3,24,107,128,6,4,102,17,128,1,127,128,4,31,101,128,1,127,128,8,127,128,2,35,87,2,128,2,127,128,8,4,128,4,25,104,128,10,127,128,2,11,110,64,13,8,44,112,21,128,6,127,128,9,127,128,7,54,87,128,7,46,116,45,9,7,35,107,40,128,4,12,37,101,38,128,2,127,128,1,2,14,48,114,34,128,1,127,128,8,127,128,3,34,76,128,2,127,128,8,118,68,25,5,6,32,105,33,128,10,127,128,3,17,87,120,122,92,23,128,
        38,34,98,123,118,93,29,128,3,122,125,115,83,22,128,3,255,2,126,118,90,27,128,2,255,7,128,1,127,128,4,33,65,128,1,127,128,8,60,96,118,126,122,95,28,128,153,90,37,128,3,37,90,128,1,255,7,128,2,8,71,113,124,111,74,125,128,2,127,128,72,127,20,84,118,123,100,33,128,3,9,73,113,124,111,72,125,128,1,123,128,1,19,105,2,128,1,2,105,18,128,7,43,84,128,1,7,104,66,17,5,21,22,127,128,2,127,128,2,44,73,113,121,101,43,128,2,51,80,115,123,107,29,128,2,255,6,128,1,255,6,128,1,255,6,128,1,127,128,7,47,82,122,112,51,128,4,255,3,128,1,127,102,44,9,7,40,115,28,128,1,9,106,64,17,4,16,49,127,128,1,113,128,2,69,57,128,1,58,69,128,7,14,104,8,128,1,69,70,128,4,
        127,128,6,102,48,16,3,21,95,47,128,1,101,48,16,3,17,62,11,128,1,127,128,12,10,109,128,1,127,128,7,127,128,7,95,34,5,13,82,41,128,6,127,128,1,127,128,4,49,90,128,1,71,66,128,4,127,128,1,104,128,2,6,108,139,1,108,6,128,7,93,32,128,2,111,20,128,4,127,128,11,17,107,128,6,11,118,128,1,127,128,12,70,54,128,1,127,128,7,127,128,11,10,107,128,6,127,128,1,127,128,4,13,119,128,1,112,18,128,4,127,128,1,94,128,3,48,78,79,47,128,7,58,69,128,3,125,4,128,4,127,128,11,7,122,128,6,24,111,128,1,127,128,11,13,108,3,128,1,127,128,7,127,128,11,10,122,128,6,127,128,1,127,128,4,4,125,128,1,125,4,128,4,127,128,1,84,128,4,227,1,128,7,23,100,3,128,3,119,12,128,
        4,127,128,11,40,102,128,4,10,41,108,41,128,1,127,124,126,118,91,29,128,6,75,49,128,2,127,128,7,127,128,11,68,88,128,6,127,128,1,127,128,4,19,110,128,1,118,14,128,4,127,128,7,227,1,128,6,4,102,21,128,4,90,47,128,3,4,127,128,10,5,106,45,128,3,255,1,118,35,128,5,10,42,111,38,128,4,17,106,2,128,2,255,6,128,1,127,128,10,44,116,17,128,6,127,128,1,127,128,4,68,70,128,1,88,51,128,4,127,128,6,48,79,80,47,128,5,72,55,128,5,29,114,39,7,9,43,106,127,128,2,113,16,128,5,80,54,128,5,7,31,97,37,128,6,34,103,128,4,81,44,128,3,127,128,7,127,128,9,59,100,18,128,7,127,128,1,127,48,14,3,16,64,106,8,128,1,26,115,42,8,7,39,101,127,128,1,127,128,2,6,
        108,140,1,109,6,128,3,35,91,128,7,32,98,251,1,99,40,120,128,1,17,113,128,5,57,5,128,8,18,104,128,6,3,124,128,3,21,103,128,4,127,128,7,127,128,9,127,2,128,8,127,128,1,127,76,113,124,115,74,9,128,3,30,97,123,117,95,42,127,128,1,127,128,2,69,58,128,1,59,69,128,2,9,105,12,128,12,16,96,128,1,49,81,128,4,72,34,128,9,2,123,128,6,12,119,128,3,86,39,128,4,127,128,7,127,128,9,127,128,8,1,127,128,1,127,128,15,127,128,4,19,105,2,128,1,3,106,18,128,1,87,41,128,8,100,45,10,2,22,92,37,128,1,80,50,128,2,2,77,43,128,10,32,105,128,6,47,84,128,2,25,99,128,5,127,128,7,127,128,19,12,120,128,1,127,128,15,127,128,4,90,37,128,3,37,90,128,1,255,7,128,
        2,49,76,249,1,97,37,128,2,112,19,128,2,77,27,128,6,101,45,12,4,31,110,36,128,1,102,49,14,5,34,110,18,128,2,92,33,128,5,127,128,7,127,128,9,127,128,6,1,14,52,90,128,1,127,128,15,127,128,39,255,6,128,1,59,92,120,124,101,40,128,2,56,87,119,122,93,20,128,2,30,95,128,6,127,128,7,255,6,128,3,127,128,4,116,125,124,113,80,14,128,151,115,128,4,127,128,4,255,2,128,1,126,128,1,127,128,1,127,30,103,123,96,14,30,102,122,92,13,128,71,112,19,128,2,87,128,2,16,112,128,1,127,9,70,114,126,128,1,7,57,127,128,4,127,128,5,127,128,2,122,128,1,127,128,1,127,110,31,6,63,105,106,31,6,63,83,128,5,127,128,5,18,115,2,128,2,2,115,18,128,1,96,32,128,7,127,16,75,114,123,
        105,47,128,3,24,91,121,122,92,24,128,2,127,128,5,127,128,1,122,128,1,122,128,3,79,49,128,1,30,127,28,128,1,45,82,128,1,127,88,36,5,128,2,255,2,128,4,127,128,5,127,128,2,118,128,4,127,14,128,1,11,126,12,128,1,11,120,128,5,127,128,6,95,38,128,2,39,94,128,2,37,100,97,34,128,5,127,105,54,15,2,15,89,50,128,1,21,116,50,11,10,50,116,21,128,1,127,128,5,127,128,1,112,128,1,112,128,3,47,80,128,1,78,84,80,128,1,74,52,128,1,127,1,128,6,127,128,3,255,4,128,2,127,128,2,114,128,1,127,128,1,127,128,2,1,127,128,2,1,127,128,5,127,128,6,44,87,128,2,87,43,128,4,36,227,1,35,128,3,127,1,128,3,19,108,128,1,85,56,128,3,56,84,128,1,127,128,5,127,128,1,
        102,128,1,102,128,3,14,111,0,5,111,6,113,8,0,102,22,128,1,127,128,7,127,128,4,127,128,5,127,128,2,110,128,1,127,128,1,127,128,3,127,128,3,127,128,5,127,128,6,3,115,10,0,10,114,3,128,6,34,97,100,37,128,1,127,128,4,3,126,128,1,117,16,128,3,16,116,128,1,127,128,5,127,128,1,91,128,1,91,128,4,110,14,47,71,0,71,54,5,116,128,2,127,128,7,127,128,4,127,128,5,127,128,2,107,128,1,127,128,1,127,128,3,127,128,3,127,128,1,255,8,128,3,70,56,0,56,68,128,9,64,127,128,1,127,128,5,127,128,1,126,4,128,3,4,125,128,1,127,128,5,127,128,1,81,128,1,81,128,4,77,44,95,22,0,21,105,32,89,128,2,127,128,7,127,128,4,127,128,5,127,128,2,103,128,1,127,128,1,127,128,
        3,127,128,3,127,128,5,127,128,7,19,104,0,104,18,128,7,34,97,100,37,128,1,127,128,5,127,128,1,116,16,128,3,16,116,128,1,127,1,128,4,127,128,10,45,91,100,128,2,100,90,59,128,2,127,128,7,127,128,4,127,128,5,127,128,2,99,128,1,127,128,1,127,128,3,127,128,3,127,128,5,127,128,8,96,51,94,128,6,35,227,1,36,128,3,127,128,5,127,128,1,85,58,128,3,57,85,128,1,114,16,128,3,13,127,128,10,12,127,51,128,2,50,127,29,128,2,127,128,7,127,128,4,127,128,5,127,128,2,95,128,1,127,128,1,127,128,3,127,128,3,127,128,5,127,128,8,45,123,42,128,4,37,100,97,34,128,5,127,128,5,127,128,1,22,117,51,11,10,50,116,22,128,1,33,75,13,2,11,36,102,127,128,1,255,4,128,4,103,7,128,
        2,6,118,3,128,2,127,128,7,127,128,4,120,3,128,4,127,128,5,127,128,18,127,128,8,3,106,2,128,4,96,33,128,7,127,128,5,127,128,2,26,93,250,1,93,25,128,3,36,109,124,114,76,15,127,128,31,127,128,4,95,52,3,128,3,127,128,2,127,128,1,127,128,104,255,4,128,2,22,102,125,117,128,1,255,2,128,1,127,128,1,127,128,2,13,87,123,119,80,9,128,68,28,98,124,115,61,128,2,85,42,128,2,41,84,128,1,7,73,113,123,99,64,128,29,87,39,5,8,66,84,128,5,255,5,128,4,125,128,7,5,100,101,5,128,5,113,45,128,2,47,118,112,50,128,2,6,120,128,1,255,8,128,1,60,117,3,128,1,22,116,42,7,24,106,43,128,1,10,105,10,0,10,105,10,128,1,83,66,135,1,38,100,128,33,6,122,128,9,52,71,
        128,1,84,23,0,121,0,22,84,128,4,81,42,46,81,128,4,17,126,9,128,1,51,86,10,50,126,54,128,1,25,101,128,12,4,112,47,128,1,84,53,128,2,34,101,128,2,41,85,0,84,41,128,2,121,5,128,5,255,8,128,18,16,67,96,110,122,127,128,8,18,100,3,128,1,44,117,32,116,33,117,44,128,3,49,75,128,1,78,49,128,3,49,97,128,2,101,26,128,1,55,126,51,10,86,52,128,13,44,101,128,1,116,12,128,2,8,122,128,3,83,80,84,128,3,107,61,4,128,31,26,115,65,34,18,5,127,128,7,2,97,24,128,3,65,249,1,122,64,128,3,21,99,4,128,1,4,100,22,128,2,80,60,128,2,120,5,128,2,50,113,120,55,128,2,255,8,128,6,125,255,5,128,3,19,127,21,128,3,30,112,125,123,107,28,128,28,82,28,128,3,
        127,128,7,63,60,128,3,44,117,33,117,33,117,43,128,1,5,100,19,128,3,20,101,5,128,1,112,22,128,30,118,9,128,8,85,79,85,128,4,1,4,11,68,108,128,28,88,6,128,2,14,127,128,6,26,96,1,128,3,84,23,0,121,0,22,83,128,1,80,45,128,5,46,79,128,34,87,44,128,7,42,84,0,85,43,128,6,4,120,128,28,50,72,9,6,45,114,127,128,5,4,101,17,128,7,126,128,49,24,112,43,8,6,37,95,128,1,10,105,10,0,10,106,10,128,1,96,39,8,7,59,83,128,29,64,115,124,93,26,127,128,5,74,50,128,60,26,91,120,124,103,59,128,1,85,40,128,2,41,85,128,1,60,100,124,119,76,8,128,41,255,5,128,248
        };


    }

    #region VertexPrimitive Grids Orientation Arrows Cube Sphere line Circle Quads ect. mostly 3d 

    public class OrientationArrows
    {
        VertexPositionColorTexture[] vertices;
        int[] indices;

        public OrientationArrows()
        {
            CreateOrientationArrows();
        }

        private void CreateOrientationArrows()
        {
            //float z = 0.0f;

            Vector3 center = new Vector3(0, 0, 0);
            //
            Vector3 endForward_fromcenter = new Vector3(0, 0, -1f);
            Vector3 endUp_fromcenter = new Vector3(0, 1f, 0);
            Vector3 endRight_fromcenter = new Vector3(1f, 0, 0);
            //
            Vector3 offCenterForward = new Vector3(0, 0, -.2f);
            Vector3 offCenterRight = new Vector3(.2f, 0, 0);
            Vector3 offCenterUp = new Vector3(0, .2f, 0);
            //
            Vector3 endForward_fromoffcenter = offCenterRight + endForward_fromcenter;
            Vector3 endUp_fromoffcenter = offCenterRight + endUp_fromcenter;
            Vector3 endRight_fromoffcenter = offCenterUp + endRight_fromcenter;


            // order is  0,1,2 ,2,1,3  of vertices 0,1,2,3   0 will always be center

            vertices = new VertexPositionColorTexture[12];

            // forward
            vertices[0].Position = center; vertices[0].Color = Color.White; vertices[0].TextureCoordinate = new Vector2(0f, 0f);
            vertices[1].Position = endForward_fromcenter; vertices[1].Color = Color.White; vertices[1].TextureCoordinate = new Vector2(1f, 0f);
            vertices[2].Position = offCenterRight; vertices[2].Color = Color.White; vertices[2].TextureCoordinate = new Vector2(0f, .33f);
            vertices[3].Position = endForward_fromoffcenter; vertices[3].Color = Color.White; vertices[3].TextureCoordinate = new Vector2(1f, .33f);

            // right
            vertices[4].Position = center; vertices[4].Color = Color.White; vertices[4].TextureCoordinate = new Vector2(0f, .66f);
            vertices[5].Position = endRight_fromcenter; vertices[5].Color = Color.White; vertices[5].TextureCoordinate = new Vector2(1f, .66f);
            vertices[6].Position = offCenterUp; vertices[6].Color = Color.White; vertices[6].TextureCoordinate = new Vector2(0f, .33f);
            vertices[7].Position = endRight_fromoffcenter; vertices[7].Color = Color.White; vertices[7].TextureCoordinate = new Vector2(1f, .33f);

            // up square
            vertices[8].Position = center; vertices[8].Color = Color.White; vertices[8].TextureCoordinate = new Vector2(0f, .66f);
            vertices[9].Position = endUp_fromcenter; vertices[9].Color = Color.White; vertices[9].TextureCoordinate = new Vector2(1f, .66f);
            vertices[10].Position = offCenterRight; vertices[10].Color = Color.White; vertices[10].TextureCoordinate = new Vector2(0f, 1f);
            vertices[11].Position = endUp_fromoffcenter; vertices[11].Color = Color.White; vertices[11].TextureCoordinate = new Vector2(1f, 1f);

            indices = new int[18];
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 2; indices[4] = 1; indices[5] = 3;

            indices[6] = 4; indices[7] = 5; indices[8] = 6;
            indices[9] = 6; indices[10] = 5; indices[11] = 7;

            indices[12] = 8; indices[13] = 9; indices[14] = 10;
            indices[15] = 10; indices[16] = 9; indices[17] = 11;
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColorTexture.VertexDeclaration);
            }
        }
    }

    /// <summary>
    /// This is a sphere or a sky sphere. A face resolution of 2 is also a cube or sky cube.
    /// It can use 6 seperate images on 6 faces or a cross or blender block type texture..
    /// Both Sphere and skyShere Uses CCW culling in regular operation.
    /// It generates positions normals texture and tangents for normal maping.
    /// It tesselates face points into sphereical coordinates on creation.
    /// It can also switch tangent or normal directions or u v that shouldn't be needed though.
    /// </summary>
    public class SpherePNTT
    {
        bool changeToSkySphere = false;
        bool changeToSingleImageTexture = true;
        bool blenderStyleElseCross = false;
        bool flipTangentSign = false;
        bool flipNormalDirection = false;
        bool flipU = false;
        bool flipV = false;
        int verticeFaceResolution = 3;
        float scale = 1f;

        int verticeFaceDrawOffset = 0;
        int indiceFaceDrawOffset = 0;
        int verticesPerFace = 0;
        int indicesPerFace = 0;
        int primitivesPerFace = 0;

        // face identifiers
        const int FaceFront = 0;
        const int FaceBack = 1;
        const int FaceLeft = 2;
        const int FaceRight = 3;
        const int FaceTop = 4;
        const int FaceBottom = 5;

        VertexPositionNormalTextureTangent[] vertices = new VertexPositionNormalTextureTangent[24];
        int[] indices = new int[36];

        /// <summary>
        /// Defaults to a seperate image hexahedron. 
        /// Use the other overloads if you want something more specific like a sphere.
        /// The spheres are counter clockwise wound.
        /// The skySphere is clockwise wound.
        /// </summary>
        public SpherePNTT()
        {
            CreateSixFaceSphere(true, false, false, false, false, false, false, verticeFaceResolution, scale);
        }
        // seperate faces
        public SpherePNTT(bool changeToSkySphere)
        {
            CreateSixFaceSphere(changeToSkySphere, false, false, false, false, false, false, verticeFaceResolution, scale);
        }
        // seperate faces at resolution
        public SpherePNTT(bool changeToSkySphere, int vertexResolutionPerFace, float scale)
        {
            CreateSixFaceSphere(changeToSkySphere, false, false, false, false, false, false, vertexResolutionPerFace, scale);
        }
        /// <summary>
        /// Set the type, if the faces are in a single image or six seperate images and if the single image is a cross or blender type image.
        /// Additionally specify the number of vertices per face this value is squared as it is used for rows and columns.
        /// </summary>
        public SpherePNTT(bool changeToSkySphere, bool changeToSingleImageTexture, bool blenderStyleSkyBox, int vertexResolutionPerFace, float scale)
        {
            CreateSixFaceSphere(changeToSkySphere, changeToSingleImageTexture, blenderStyleSkyBox, false, false, false, false, vertexResolutionPerFace, scale);
        }
        public SpherePNTT(bool changeToSkySphere, bool changeToSingleImageTexture, bool blenderStyleSkyBox, bool flipNormalDirection, bool flipTangentDirection, bool flipTextureDirectionU, bool flipTextureDirectionV, int vertexResolutionPerFace, float scale)
        {
            CreateSixFaceSphere(changeToSkySphere, changeToSingleImageTexture, blenderStyleSkyBox, flipNormalDirection, flipTangentDirection, flipTextureDirectionU, flipTextureDirectionV, vertexResolutionPerFace, scale);
        }

        void CreateSixFaceSphere(bool changeToSkySphere, bool changeToSingleImageTexture, bool blenderStyleElseCross, bool flipNormalDirection, bool flipTangentDirection, bool flipU, bool flipV, int vertexResolutionPerFace, float scale)
        {
            this.scale = scale;
            this.changeToSkySphere = changeToSkySphere;
            this.changeToSingleImageTexture = changeToSingleImageTexture;
            this.blenderStyleElseCross = blenderStyleElseCross;
            this.flipNormalDirection = flipNormalDirection;
            this.flipTangentSign = flipTangentDirection;
            this.flipU = flipU;
            this.flipV = flipV;
            if (vertexResolutionPerFace < 2)
                vertexResolutionPerFace = 2;
            this.verticeFaceResolution = vertexResolutionPerFace;
            Vector3 offset = new Vector3(.5f, .5f, .5f);
            // 8 vertice points ill label them, then reassign them for clarity.
            Vector3 LT_f = new Vector3(0, 1, 0) - offset; Vector3 A = LT_f * scale;
            Vector3 LB_f = new Vector3(0, 0, 0) - offset; Vector3 B = LB_f * scale;
            Vector3 RT_f = new Vector3(1, 1, 0) - offset; Vector3 C = RT_f * scale;
            Vector3 RB_f = new Vector3(1, 0, 0) - offset; Vector3 D = RB_f * scale;
            Vector3 LT_b = new Vector3(0, 1, 1) - offset; Vector3 E = LT_b * scale;
            Vector3 LB_b = new Vector3(0, 0, 1) - offset; Vector3 F = LB_b * scale;
            Vector3 RT_b = new Vector3(1, 1, 1) - offset; Vector3 G = RT_b * scale;
            Vector3 RB_b = new Vector3(1, 0, 1) - offset; Vector3 H = RB_b * scale;

            // Six faces to a cube or sphere
            // each face of the cube wont actually share vertices as each will use its own texture.
            // unless it is actually using single skybox texture

            // we will need to precalculate the grids size now
            int vw = vertexResolutionPerFace;
            int vh = vertexResolutionPerFace;
            int vlen = vw * vh * 6; // the extra six here is the number of faces
            int iw = vw - 1;
            int ih = vh - 1;
            int ilen = iw * ih * 6 * 6; // the extra six here is the number of faces
            vertices = new VertexPositionNormalTextureTangent[vlen];
            indices = new int[ilen];
            verticeFaceDrawOffset = vlen = vw * vh;
            indiceFaceDrawOffset = ilen = iw * ih * 6;
            verticesPerFace = vertexResolutionPerFace * vertexResolutionPerFace;
            indicesPerFace = iw * ih * 6;
            primitivesPerFace = iw * ih * 2; // 2 triangles per quad

            if (changeToSkySphere)
            {
                // passed uv texture coordinates.
                Vector2 uv0 = new Vector2(1f, 1f);
                Vector2 uv1 = new Vector2(0f, 1f);
                Vector2 uv2 = new Vector2(1f, 0f);
                Vector2 uv3 = new Vector2(0f, 0f);
                SetFaceGrid(FaceFront, D, B, C, A, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceBack, F, H, E, G, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceLeft, B, F, A, E, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceRight, H, D, G, C, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceTop, C, A, G, E, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceBottom, H, F, D, B, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
            }
            else // regular cube 
            {
                Vector2 uv0 = new Vector2(0f, 0f);
                Vector2 uv1 = new Vector2(0f, 1f);
                Vector2 uv2 = new Vector2(1f, 0f);
                Vector2 uv3 = new Vector2(1f, 1f);
                SetFaceGrid(FaceFront, A, B, C, D, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceBack, G, H, E, F, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceLeft, E, F, A, B, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceRight, C, D, G, H, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceTop, E, A, G, C, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
                SetFaceGrid(FaceBottom, B, F, D, H, uv0, uv1, uv2, uv3, vertexResolutionPerFace);
            }
        }

        void SetFaceGrid(int faceMultiplier, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3, int vertexResolution)
        {
            if (changeToSingleImageTexture)
                UvSkyTextureReassignment(faceMultiplier, ref uv0, ref uv1, ref uv2, ref uv3);
            int vw = vertexResolution;
            int vh = vertexResolution;
            int vlen = vw * vh;
            int iw = vw - 1;
            int ih = vh - 1;
            int ilen = iw * ih * 6;
            // actual start index's
            int vIndex = faceMultiplier * vlen;
            int iIndex = faceMultiplier * ilen;
            // we now must build the grid/
            float ratio = 1f / (float)(vertexResolution - 1);
            // well do it all simultaneously no point in spliting it up
            for (int y = 0; y < vertexResolution; y++)
            {
                float ratioY = (float)y * ratio;
                for (int x = 0; x < vertexResolution; x++)
                {
                    // index
                    int index = vIndex + (y * vertexResolution + x);
                    float ratioX = (float)x * ratio;
                    // calculate uv_n_p tangent comes later
                    var uv = InterpolateUv(uv0, uv1, uv2, uv3, ratioX, ratioY);
                    var n = InterpolateToNormal(v0, v1, v2, v3, ratioX, ratioY);
                    var p = n * .5f; // displace to distance
                    if (changeToSkySphere)
                        n = -n;
                    if (flipNormalDirection)
                        n = -n;
                    // handle u v fliping if its desired.
                    if (flipU)
                        uv.X = 1.0f - uv.X;
                    if (flipV)
                        uv.Y = 1.0f - uv.Y;
                    // assign
                    vertices[index].Position = p;
                    vertices[index].TextureCoordinate = uv;
                    vertices[index].Normal = n;
                }
            }

            // ToDo... 
            // We could loop all the vertices which are nearly the exact same and make sure they are the same place but seperate.
            // sort of redundant but floating point errors happen under interpolation, well get back to that later on.
            // not sure i really need to it looks pretty spot on.

            // ok so now we have are positions our normal and uv per vertice we need to loop again and handle the tangents
            for (int y = 0; y < (vertexResolution - 1); y++)
            {
                for (int x = 0; x < (vertexResolution - 1); x++)
                {
                    //
                    int indexV0 = vIndex + (y * vertexResolution + x);
                    int indexV1 = vIndex + ((y + 1) * vertexResolution + x);
                    int indexV2 = vIndex + (y * vertexResolution + (x + 1));
                    int indexV3 = vIndex + ((y + 1) * vertexResolution + (x + 1));
                    var p0 = vertices[indexV0].Position;
                    var p1 = vertices[indexV1].Position;
                    var p2 = vertices[indexV2].Position;
                    var p3 = vertices[indexV3].Position;
                    var t = -(p0 - p1);
                    if (changeToSkySphere)
                        t = -t;
                    t.Normalize();
                    if (flipTangentSign)
                        t = -t;
                    vertices[indexV0].Tangent = t; vertices[indexV1].Tangent = t; vertices[indexV2].Tangent = t; vertices[indexV3].Tangent = t;
                    //
                    // set our indices while were at it.
                    int indexI = iIndex + ((y * (vertexResolution - 1) + x) * 6);
                    int via = indexV0, vib = indexV1, vic = indexV2, vid = indexV3;
                    indices[indexI + 0] = via; indices[indexI + 1] = vib; indices[indexI + 2] = vic;
                    indices[indexI + 3] = vic; indices[indexI + 4] = vib; indices[indexI + 5] = vid;
                }
            }
        }

        // this allows for the use of a single texture skybox.
        void UvSkyTextureReassignment(int faceMultiplier, ref Vector2 uv0, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3)
        {
            if (changeToSingleImageTexture)
            {
                Vector2 tupeBuvwh = new Vector2(.250000000f, .333333333f); // this is a 8 square left sided skybox
                Vector2 tupeAuvwh = new Vector2(.333333333f, .500000000f); // this is a 6 square blender type skybox
                Vector2 currentuvWH = tupeBuvwh;
                Vector2 uvStart = Vector2.Zero;
                Vector2 uvEnd = Vector2.Zero;

                // crossstyle
                if (blenderStyleElseCross == false)
                {
                    currentuvWH = tupeBuvwh;
                    switch (faceMultiplier)
                    {
                        case FaceFront:
                            uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceBack:
                            uvStart = new Vector2(currentuvWH.X * 3f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceRight:
                            uvStart = new Vector2(currentuvWH.X * 2f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceLeft:
                            uvStart = new Vector2(currentuvWH.X * 0f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            //uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 0f);
                            //uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceTop:
                            uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 0f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceBottom:
                            uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 2f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                    }
                    if (changeToSkySphere)
                    {
                        uv0 = new Vector2(uvEnd.X, uvEnd.Y); uv1 = new Vector2(uvStart.X, uvEnd.Y); uv2 = new Vector2(uvEnd.X, uvStart.Y); uv3 = new Vector2(uvStart.X, uvStart.Y);
                    }
                    else
                    {
                        uv0 = new Vector2(uvStart.X, uvStart.Y); uv1 = new Vector2(uvStart.X, uvEnd.Y); uv2 = new Vector2(uvEnd.X, uvStart.Y); uv3 = new Vector2(uvEnd.X, uvEnd.Y);
                    }
                }
                else
                {
                    currentuvWH = tupeAuvwh;
                    switch (faceMultiplier)
                    {
                        case FaceLeft:
                            uvStart = new Vector2(currentuvWH.X * 0f, currentuvWH.Y * 0f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceBack:
                            uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 0f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceRight:
                            uvStart = new Vector2(currentuvWH.X * 2f, currentuvWH.Y * 0f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceBottom:
                            uvStart = new Vector2(currentuvWH.X * 0f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceTop:
                            uvStart = new Vector2(currentuvWH.X * 1f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                        case FaceFront:
                            uvStart = new Vector2(currentuvWH.X * 2f, currentuvWH.Y * 1f);
                            uvEnd = uvStart + currentuvWH;
                            break;
                    }
                    if (changeToSkySphere)
                    {
                        uv0 = new Vector2(uvEnd.X, uvEnd.Y); uv2 = new Vector2(uvEnd.X, uvStart.Y); uv1 = new Vector2(uvStart.X, uvEnd.Y); uv3 = new Vector2(uvStart.X, uvStart.Y);
                    }
                    else
                    {
                        uv0 = new Vector2(uvStart.X, uvStart.Y); uv1 = new Vector2(uvStart.X, uvEnd.Y); uv2 = new Vector2(uvEnd.X, uvStart.Y); uv3 = new Vector2(uvEnd.X, uvEnd.Y);
                    }
                }
            }
        }

        Vector3 InterpolateToNormal(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float timeX, float timeY)
        {
            var y0 = ((v1 - v0) * timeY + v0);
            var y1 = ((v3 - v2) * timeY + v2);
            var n = ((y1 - y0) * timeX + y0) * 10f; // * 10f ensure its sufficiently denormalized.
            n.Normalize();
            return n;
        }
        Vector2 InterpolateUv(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float timeX, float timeY)
        {
            var y0 = ((v1 - v0) * timeY + v0);
            var y1 = ((v3 - v2) * timeY + v2);
            return ((y1 - y0) * timeX + y0);
        }

        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTextureTangent.VertexDeclaration);
            }
        }

        /// <summary>
        /// Seperate faced cube or sphere or sky
        /// This method is pretty dependant on being able to pass to textureA not good but....
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect, Texture2D front, Texture2D back, Texture2D left, Texture2D right, Texture2D top, Texture2D bottom)
        {
            int FaceFront = 0;
            int FaceBack = 1;
            int FaceLeft = 2;
            int FaceRight = 3;
            int FaceTop = 4;
            int FaceBottom = 5;
            for (int t = 0; t < 6; t++)
            {
                if (t == FaceFront) effect.Parameters["TextureA"].SetValue(front);
                if (t == FaceBack) effect.Parameters["TextureA"].SetValue(back);
                if (t == FaceLeft) effect.Parameters["TextureA"].SetValue(left);
                if (t == FaceRight) effect.Parameters["TextureA"].SetValue(right);
                if (t == FaceTop) effect.Parameters["TextureA"].SetValue(top);
                if (t == FaceBottom) effect.Parameters["TextureA"].SetValue(bottom);
                int ifoffset = t * indicesPerFace;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, ifoffset, primitivesPerFace, VertexPositionNormalTextureTangent.VertexDeclaration);
                }
            }
        }

        /// <summary>
        /// Single texture multi faced cube or sphere or sky
        /// This method is pretty dependant on being able to pass to textureA not good but....
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect, Texture2D cubeTexture)
        {
            effect.Parameters["TextureA"].SetValue(cubeTexture);
            for (int t = 0; t < 6; t++)
            {
                int ifoffset = t * indicesPerFace;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, ifoffset, primitivesPerFace, VertexPositionNormalTextureTangent.VertexDeclaration);
                }
            }
        }

        /// <summary>
        /// This method is pretty dependant on being able to pass to textureA not good but....
        /// </summary>
        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect, Texture2D front, Texture2D back, Texture2D left, Texture2D right, Texture2D top, Texture2D bottom)
        {
            int FaceFront = 0;
            int FaceBack = 1;
            int FaceLeft = 2;
            int FaceRight = 3;
            int FaceTop = 4;
            int FaceBottom = 5;
            for (int t = 0; t < 6; t++)
            {
                if (t == FaceFront) effect.Texture = front;
                if (t == FaceBack) effect.Texture = back;
                if (t == FaceLeft) effect.Texture = left;
                if (t == FaceRight) effect.Texture = right;
                if (t == FaceTop) effect.Texture = top;
                if (t == FaceBottom) effect.Texture = bottom;
                int ifoffset = t * indicesPerFace;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, ifoffset, primitivesPerFace, VertexPositionNormalTextureTangent.VertexDeclaration);
                }
            }
        }

        /// <summary>
        /// Single texture multi faced cube or sphere or sky
        /// This method is pretty dependant on being able to pass to textureA not good but....
        /// </summary>
        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect, Texture2D cubeTexture)
        {
            effect.Texture = cubeTexture;
            for (int t = 0; t < 6; t++)
            {
                int ifoffset = t * indicesPerFace;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, ifoffset, primitivesPerFace, VertexPositionNormalTextureTangent.VertexDeclaration);
                }
            }
        }

        public Vector3 Norm(Vector3 n)
        {
            return Vector3.Normalize(n);
        }

        /// <summary>
        /// Positional cross product, Counter Clock wise positive.
        /// </summary>
        public static Vector3 CrossVectors3d(Vector3 a, Vector3 b, Vector3 c)
        {
            // no point in doing reassignments the calculation is straight forward.
            return new Vector3
                (
                ((b.Y - a.Y) * (c.Z - b.Z)) - ((c.Y - b.Y) * (b.Z - a.Z)),
                ((b.Z - a.Z) * (c.X - b.X)) - ((c.Z - b.Z) * (b.X - a.X)),
                ((b.X - a.X) * (c.Y - b.Y)) - ((c.X - b.X) * (b.Y - a.Y))
                );
        }

        /// <summary>
        /// use the vector3 cross
        /// </summary>
        public static Vector3 CrossXna(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = a - b;
            var v2 = c - b;

            return Vector3.Cross(v1, v2);
        }

        // vertex structure data.
        public struct VertexPositionNormalTextureTangent : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Vector3 Tangent;

            public static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                  new VertexElement(VertexElementByteOffset.PositionStartOffset(), VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector2(), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 1)
            );
            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        }
        /// <summary>
        /// This is a helper struct for tallying byte offsets
        /// </summary>
        public struct VertexElementByteOffset
        {
            public static int currentByteSize = 0;
            [STAThread]
            public static int PositionStartOffset() { currentByteSize = 0; var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(float n) { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector2 n) { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Color n) { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector3 n) { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector4 n) { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }

            public static int OffsetFloat() { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
            public static int OffsetColor() { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector2() { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector3() { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector4() { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }
        }
    }

    public class Grid3dOrientation
    {
        public Grid3d gridForward;
        public Grid3d gridRight;
        public Grid3d gridUp;

        /// <summary>
        /// Draws 3 3d grids, linewith should be very small like .001
        /// </summary>
        public Grid3dOrientation(int x, int y, float lineWidth)
        {
            gridForward = new Grid3d(x, y, lineWidth, true, 0);
            gridRight = new Grid3d(x, y, lineWidth, true, 1);
            gridUp = new Grid3d(x, y, lineWidth, true, 2);
        }

        /// <summary>
        /// Draws this world grid with basic effect.
        /// </summary>
        public void DrawWithBasicEffect(GraphicsDevice gd, BasicEffect effect, Matrix world, float scale, Texture2D forwardTexture, Texture2D upTexture, Texture2D rightTexture)
        {
            // Draw a 3d full orientation grid
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            effect.World = Matrix.CreateScale(scale) * world;
            bool isLighting = effect.LightingEnabled;
            effect.LightingEnabled = false;
            effect.Texture = upTexture;
            gridForward.Draw(gd, effect);
            effect.Texture = forwardTexture;
            gridRight.Draw(gd, effect);
            effect.Texture = rightTexture;
            gridUp.Draw(gd, effect);
            if (isLighting)
                effect.LightingEnabled = true;
        }

        /// <summary>
        /// The method expects that the shader can accept a parameter named TextureA.
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect, Texture2D forwardTexture, Texture2D upTexture, Texture2D rightTexture)
        {
            // Draw a 3d full orientation grid
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            effect.Parameters["TextureA"].SetValue(upTexture);
            gridForward.Draw(gd, effect);
            effect.Parameters["TextureA"].SetValue(forwardTexture);
            gridRight.Draw(gd, effect);
            effect.Parameters["TextureA"].SetValue(rightTexture);
            gridUp.Draw(gd, effect);
        }

        public void Draw(GraphicsDevice gd, Effect effect, int part0to2)
        {
            if (part0to2 == 0)
            {
                gridForward.Draw(gd, effect);
            }
            else
            {
                if (part0to2 == 1)
                    gridRight.Draw(gd, effect);
                else
                    gridUp.Draw(gd, effect);
            }
        }
    }

    public class OrientationLines
    {
        VertexPositionColor[] vertices;
        int[] indices;

        public OrientationLines()
        {
            CreateOrientationLines(.1f, 1.0f);
        }
        public OrientationLines(float linewidth, float lineDistance)
        {
            CreateOrientationLines(linewidth, lineDistance);
        }

        private void CreateOrientationLines(float linewidth, float lineDistance)
        {
            var center = new Vector3(0, 0, 0);
            var scaledup = Vector3.Up * linewidth;
            var scaledforward = Vector3.Forward * linewidth;
            var forward = Vector3.Forward * lineDistance;
            var right = Vector3.Right * lineDistance;
            var up = Vector3.Up * lineDistance;

            var r = new Color(1.0f, 0.0f, 0.0f, .8f);
            var g = new Color(0.0f, 1.0f, 0.0f, .8f);
            var b = new Color(0.0f, 0.0f, 1.0f, .8f);

            vertices = new VertexPositionColor[9];
            indices = new int[18];

            // forward
            vertices[0].Position = forward; vertices[0].Color = g;
            vertices[1].Position = scaledup; vertices[1].Color = g;
            vertices[2].Position = center; vertices[2].Color = g;

            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 0; indices[4] = 2; indices[5] = 1;

            // right
            vertices[4].Position = right; vertices[3].Color = b;
            vertices[5].Position = scaledup; vertices[4].Color = b;
            vertices[6].Position = center; vertices[5].Color = b;

            indices[6] = 3; indices[7] = 4; indices[8] = 5;
            indices[9] = 3; indices[10] = 5; indices[11] = 4;

            // up square
            vertices[8].Position = up; vertices[6].Color = r;
            vertices[9].Position = center; vertices[7].Color = r;
            vertices[10].Position = scaledforward; vertices[8].Color = r;

            indices[12] = 6; indices[13] = 7; indices[14] = 8;
            indices[15] = 6; indices[16] = 8; indices[17] = 7;
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColor.VertexDeclaration);
            }
        }
    }

    public class NavOrientation3d
    {
        CircleNav3d navCircle3dA;
        CircleNav3d navCircle3dB;
        CircleNav3d navCircle3dC;

        public NavOrientation3d()
        {
            navCircle3dA = new CircleNav3d(30, .05f, 24, 6, 0);
            navCircle3dB = new CircleNav3d(30, .05f, 24, 24, 1);
            navCircle3dC = new CircleNav3d(30, .05f, 24, 24, 2);
        }
        public NavOrientation3d(int segments, int navSegments, int largeSegmentModulator, float lineThickness0to1)
        {
            navCircle3dA = new CircleNav3d(segments, lineThickness0to1, navSegments, largeSegmentModulator, 0);
            navCircle3dB = new CircleNav3d(segments, lineThickness0to1, navSegments, largeSegmentModulator, 1);
            navCircle3dC = new CircleNav3d(segments, lineThickness0to1, navSegments, largeSegmentModulator, 2);
        }
        public NavOrientation3d(int segments, int navSegments, int largeSegmentModulator, float lineThickness0to1, float navSize0to1)
        {
            navCircle3dA = new CircleNav3d(segments, navSegments, largeSegmentModulator, lineThickness0to1, navSize0to1, true, 0);
            navCircle3dB = new CircleNav3d(segments, navSegments, largeSegmentModulator, lineThickness0to1, navSize0to1, true, 1);
            navCircle3dC = new CircleNav3d(segments, navSegments, largeSegmentModulator, lineThickness0to1, navSize0to1, true, 2);
        }

        public void DrawNavOrientation3DWithBasicEffect(GraphicsDevice gd, BasicEffect beffect, Matrix world, Texture2D ta, Texture2D tb, Texture2D tc)
        {
            bool isLighting = beffect.LightingEnabled;
            beffect.LightingEnabled = false;
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            beffect.World = world;
            beffect.Texture = ta;
            navCircle3dA.Draw(gd, beffect);
            beffect.Texture = tb;
            navCircle3dB.Draw(gd, beffect);
            beffect.Texture = tc;
            navCircle3dC.Draw(gd, beffect);
            beffect.LightingEnabled = isLighting;
        }
        public void DrawNavOrientation3DWithBasicEffect(GraphicsDevice gd, BasicEffect beffect, Vector3 position, float scale, Texture2D ta, Texture2D tb, Texture2D tc)
        {
            Matrix world = Matrix.CreateScale(scale) * Matrix.Identity;
            world.Translation = position;
            bool isLighting = beffect.LightingEnabled;
            beffect.LightingEnabled = false;
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            beffect.World = world;
            beffect.Texture = ta;
            navCircle3dA.Draw(gd, beffect);
            beffect.Texture = tb;
            navCircle3dB.Draw(gd, beffect);
            beffect.Texture = tc;
            navCircle3dC.Draw(gd, beffect);
            beffect.LightingEnabled = isLighting;
        }
        public void DrawNavOrientation3DToTargetWithBasicEffect(GraphicsDevice gd, BasicEffect beffect, Vector3 position, float scale, Matrix lookAtTargetsMatrix, Texture2D ta, Texture2D tb, Texture2D tc)
        {
            var totarget = lookAtTargetsMatrix.Translation - position;
            Matrix target = Matrix.CreateScale(scale * .6f) * Matrix.CreateWorld(position, -totarget, lookAtTargetsMatrix.Up);
            Matrix world = Matrix.CreateScale(scale) * Matrix.Identity;
            world.Translation = position;
            bool isLighting = beffect.LightingEnabled;
            beffect.LightingEnabled = false;
            gd.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };
            beffect.World = world;
            beffect.Texture = ta;
            navCircle3dA.Draw(gd, beffect);
            beffect.World = target;
            beffect.Texture = tb;
            navCircle3dB.Draw(gd, beffect);
            beffect.World = world;
            beffect.Texture = tc;
            navCircle3dC.Draw(gd, beffect);
            beffect.LightingEnabled = isLighting;
        }
    }

    public class Quad
    {
        public VertexPositionNormalTexture[] vertices;
        public int[] indices;
        public bool changeToXz = true;

        float z = 0.0f;
        float adjustmentX = 0f; // .5
        float adjustmentY = 0f; // -.5
        float scale = 2f; // scale 2 and matrix identity passed straight thru is litterally orthographic

        public Quad()
        {
            CreateQuad();
        }
        public Quad(float scale)
        {
            this.scale = scale;
            CreateQuad();
        }
        public Quad(float scale, float z, float adjustmentX, float adjustmentY)
        {
            this.scale = scale;
            this.adjustmentX = adjustmentX;
            this.adjustmentY = adjustmentY;
            this.z = z;
            CreateQuad();
        }

        private void CreateQuad()
        {
            //    
            //    //    0          2 
            //    //   LT ------ RT
            //    //   |          |  
            //    //   |1         |3 
            //    //   LB ------ RB
            //
            indices = new int[6];
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            indices[3] = 2; indices[4] = 1; indices[5] = 3;


            vertices = new VertexPositionNormalTexture[4];
            vertices[0].Position = new Vector3((adjustmentX - 0.5f) * scale, (adjustmentY - 0.5f) * scale, z);
            vertices[0].Normal = Vector3.Backward;
            //vertices_Quad[0].Color = Color.White;
            vertices[0].TextureCoordinate = new Vector2(0f, 1f);

            vertices[1].Position = new Vector3((adjustmentX + 0.5f) * scale, (adjustmentY - 0.5f) * scale, z);
            vertices[1].Normal = Vector3.Backward;
            //vertices_Quad[1].Color = Color.White;
            vertices[1].TextureCoordinate = new Vector2(0f, 0f);

            vertices[2].Position = new Vector3((adjustmentX - 0.5f) * scale, (adjustmentY + 0.5f) * scale, z);
            vertices[2].Normal = Vector3.Backward;
            //vertices_Quad[2].Color = Color.White;
            vertices[2].TextureCoordinate = new Vector2(1f, 1f);

            vertices[3].Position = new Vector3((adjustmentX + 0.5f) * scale, (adjustmentY + 0.5f) * scale, z);
            vertices[3].Normal = Vector3.Backward;
            //vertices_Quad[3].Color = Color.White;
            vertices[3].TextureCoordinate = new Vector2(1f, 0f);

            if (changeToXz)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 n = vertices[i].Position;
                    vertices[i].Position = new Vector3(n.X, n.Z, n.Y);
                }
             }

            // ok finding a bi tanget or what ever its called a perpendicular is straightforward.
            // provided im using a quad. even if im not i can use some simple averages on the triangle and figure out the up and or right.
            //Vector3 parrallelPlaneNormalUp = vertices[0].Position - vertices[1].Position;
            // this is for all the vertices ^^^ as long as the quad is flat to itself which it should be.
            //
            //vertices[0].Tangent = Vector3.Normalize(vertices[0].Position - vertices[1].Position);
            //vertices[1].Tangent = Vector3.Normalize(vertices[0].Position - vertices[1].Position);
            //vertices[2].Tangent = Vector3.Normalize(vertices[0].Position - vertices[1].Position);
            //vertices[3].Tangent = Vector3.Normalize(vertices[0].Position - vertices[1].Position);

        }

        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTexture.VertexDeclaration);
            }
        }
    }

    public class Grid3d
    {
        int width;
        int height;
        public VertexPositionTexture[] vertices;
        public int[] indices;

        /// <summary>
        /// Creates a grid for 3d modelspace.
        /// The Width Height is doubled into negative and positive.
        /// linesize should be a very small value less then 1;
        /// flip options range from 0 to 2
        /// </summary>
        public Grid3d(int rows, int columns, float lineSize, bool centered, int flipOption)
        {
            rows *= 2;
            columns *= 2;
            Vector3 centerOffset = Vector3.Zero;
            if (centered)
                centerOffset = new Vector3(-.5f, -.5f, 0f);
            width = rows;
            height = columns;
            int len = width * 4 + height * 4;
            float xratio = 1f / width;
            float yratio = 1f / height;
            vertices = new VertexPositionTexture[len];
            indices = new int[(width * 6 + height * 6) * 2];
            int vIndex = 0;
            int iIndex = 0;
            for (int x = 0; x < width; x++)
            {
                int svIndex = vIndex;
                Vector3 xpos = new Vector3(xratio * x, 0f, 0f);
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(0f, 0f, 0f) + xpos + centerOffset,
                    new Vector2(0f, 0f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(0f, 1f, 0f) + xpos + centerOffset,
                    new Vector2(0f, 1f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(lineSize, 0f, 0f) + xpos + centerOffset,
                    new Vector2(1f, 0f));
                vIndex++;
                vertices[vIndex] = new VertexPositionTexture(
                    new Vector3(lineSize, 1f, 0f) + xpos + centerOffset,
                    new Vector2(1f, 1f));
                vIndex++;
                // triangle 1
                indices[iIndex + 0] = svIndex + 0; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 2;
                // triangle 2
                indices[iIndex + 3] = svIndex + 2; indices[iIndex + 4] = svIndex + 1; indices[iIndex + 5] = svIndex + 3;
                // triangle 3 backface
                indices[iIndex + 0] = svIndex + 2; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 0;
                // triangle 4 backface
                indices[iIndex + 3] = svIndex + 3; indices[iIndex + 4] = svIndex + 2; indices[iIndex + 5] = svIndex + 1;
                iIndex += 6 *2;
            }
            for (int y = 0; y < height; y++)
            {
                int svIndex = vIndex;
                Vector3 ypos = new Vector3(0f, yratio * y, 0f);
                vertices[vIndex] = new VertexPositionTexture(new Vector3(0f, 0f, 0f) + ypos + centerOffset, new Vector2(0f, 0f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(0f, lineSize, 0f) + ypos + centerOffset, new Vector2(0f, 1f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(1f, 0f, 0f) + ypos + centerOffset, new Vector2(1f, 0f)); vIndex++;
                vertices[vIndex] = new VertexPositionTexture(new Vector3(1f, lineSize, 0f) + ypos + centerOffset, new Vector2(1f, 1f)); vIndex++;
                // triangle 1
                indices[iIndex + 0] = svIndex + 0; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 2;
                // triangle 2
                indices[iIndex + 3] = svIndex + 2; indices[iIndex + 4] = svIndex + 1; indices[iIndex + 5] = svIndex + 3;
                // triangle 3 backface
                indices[iIndex + 0] = svIndex + 2; indices[iIndex + 1] = svIndex + 1; indices[iIndex + 2] = svIndex + 0;
                // triangle 4 backface
                indices[iIndex + 3] = svIndex + 3; indices[iIndex + 4] = svIndex + 2; indices[iIndex + 5] = svIndex + 1;
                iIndex += 6 *2;
            }
            Flip(flipOption);
        }

        void Flip(int flipOption)
        {
            if (flipOption == 1)
            {
                int index = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(0f, p.X, p.Y);
                        index++;
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(0f, p.X, p.Y);
                        index++;
                    }
                }
            }
            if (flipOption == 2)
            {
                int index = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(p.Y, 0f, p.X);
                        index++;
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var p = vertices[index].Position;
                        vertices[index].Position = new Vector3(p.Y, 0f, p.X);
                        index++;
                    }
                }
            }
        }

        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionTexture.VertexDeclaration);
            }
        }
    }

    public class Circle3d
    {
        bool centered = true;
        public VertexPositionTexture[] vertices;
        public int[] indices;

        static int OrientationOptionRightUpForward
        {
            get;
            set;
        }

        public Circle3d(int segments)
        {
            CreateCircle(segments, .01f, true, 2);
        }
        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public Circle3d(int segments, float lineSize)
        {
            CreateCircle(segments, lineSize, true, 2);
        }
        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public Circle3d(int segments, float lineSize0to1, bool centerIt, int orientation012)
        {
            CreateCircle(segments, lineSize0to1, centerIt, orientation012);
        }
        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public void CreateCircle(int segments, float lineSize0to1, bool centerIt, int orientation012)
        {
            centered = centerIt;
            float centering = .5f;
            if (centered)
                centering = 0.0f;
            float offset = 1f - lineSize0to1;
            vertices = new VertexPositionTexture[segments * 2];
            indices = new int[segments * 6];
            float pi2 = (float)(Math.PI * 2d);
            float mult = 1f / (float)(segments);
            int index = 0;
            int v_index = 0;
            int i_index = 0;
            for (index = 0; index < segments; index++)
            {
                var u = (float)(index) * mult;
                double radians = u * pi2;
                float x = ((float)(Math.Sin(radians)) * .5f) + centering;
                float y = ((float)(Math.Cos(radians)) * .5f) + centering;
                vertices[v_index + 0] = new VertexPositionTexture(ReOrient(new Vector3(x, y, 0)), new Vector2(u, 0f));
                vertices[v_index + 1] = new VertexPositionTexture(ReOrient(new Vector3(x * offset, y * offset, 0)), new Vector2(u, 1f));
                if (index < segments - 1)
                {
                    indices[i_index + 0] = v_index + 0; indices[i_index + 1] = v_index + 1; indices[i_index + 2] = v_index + 2;
                    indices[i_index + 3] = v_index + 2; indices[i_index + 4] = v_index + 1; indices[i_index + 5] = v_index + 3;
                }
                else
                {
                    // connect the last one directly to the front
                    indices[i_index + 0] = v_index + 0; indices[i_index + 1] = v_index + 1; indices[i_index + 2] = 0;
                    indices[i_index + 3] = 0; indices[i_index + 4] = v_index + 1; indices[i_index + 5] = 1;
                }
                v_index += 2;
                i_index += 6;
            }
        }
        Vector3 ReOrient(Vector3 v)
        {
            if (OrientationOptionRightUpForward == 1)
                v = new Vector3(v.Z, v.X, v.Y);
            if (OrientationOptionRightUpForward == 2)
                v = new Vector3(v.X, v.Z, v.Y);
            return v;
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionTexture.VertexDeclaration);
            }
        }
    }

    public class CircleNav3d
    {
        bool centered = true;
        float navSmallLargeRatio = .40f;
        float zerodegreelinethickener = 4.0f;

        public VertexPositionTexture[] vertices;
        public int[] indices;
        static int OrientationOptionRightUpForward
        {
            get;
            set;
        }

        public CircleNav3d(int segments)
        {
            CreateCircleNav3d(segments, 4, 2, .01f, .35f, true, 2);
        }
        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public CircleNav3d(int segments, float lineSize)
        {
            CreateCircleNav3d(segments, 4, 2, lineSize, .35f, true, 2);
        }
        public CircleNav3d(int segments, float lineSize, int navsegments, int largeSegmentModulator)
        {
            CreateCircleNav3d(segments, navsegments, largeSegmentModulator, lineSize, .35f, true, 2);
        }
        public CircleNav3d(int segments, float lineSize, int navsegments, int largeSegmentModulator, int orientation012)
        {
            CreateCircleNav3d(segments, navsegments, largeSegmentModulator, lineSize, .35f, true, orientation012);
        }
        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public CircleNav3d(int segments, int navsegments, int largeSegmentModulator, float lineSize0to1, float navSize0to1, bool centerIt, int orientation012)
        {
            CreateCircleNav3d(segments, navsegments, largeSegmentModulator, lineSize0to1, navSize0to1, centerIt, orientation012);
        }

        /// <summary>
        /// Create a circle default orientation is 2 forward
        /// </summary>
        public void CreateCircleNav3d(int segments, int navSegments, int largeSegmentModulator, float lineSize0to1, float navSize0to1, bool centerIt, int orientation012)
        {
            OrientationOptionRightUpForward = orientation012;
            int circlesegmentVertexs = segments * 2;
            int circlesegmentIndices = segments * 6;
            int navsegmentVertexs = navSegments * 4;
            int navsegmentIndices = navSegments * 6;
            vertices = new VertexPositionTexture[circlesegmentVertexs + navsegmentVertexs];
            indices = new int[circlesegmentIndices + navsegmentIndices];

            centered = centerIt;
            float centering = .5f;
            if (centered)
                centering = 0.0f;
            float offset = 1f - lineSize0to1;
            float pi2 = (float)(Math.PI * 2d);
            float steppercentage = 1f / (float)(segments);
            float u = 0f, radians = 0f, x = 0f, y = 0f;
            int index = 0, v_index = 0, i_index = 0;
            for (index = 0; index < segments; index++)
            {
                u = (float)(index) * steppercentage;
                radians = u * pi2;
                x = ((float)(Math.Sin(radians)) * .5f) + centering;
                y = ((float)(Math.Cos(radians)) * .5f) + centering;
                vertices[v_index + 0] = new VertexPositionTexture(ReOrient(new Vector3(x, y, 0)), new Vector2(u, 0f));
                vertices[v_index + 1] = new VertexPositionTexture(ReOrient(new Vector3(x * offset, y * offset, 0)), new Vector2(u, 1f));
                if (index < segments - 1)
                {
                    indices[i_index + 0] = v_index + 0; indices[i_index + 1] = v_index + 1; indices[i_index + 2] = v_index + 2;
                    indices[i_index + 3] = v_index + 2; indices[i_index + 4] = v_index + 1; indices[i_index + 5] = v_index + 3;
                }
                else
                {
                    // connect the last one directly to the front
                    indices[i_index + 0] = v_index + 0; indices[i_index + 1] = v_index + 1; indices[i_index + 2] = 0;
                    indices[i_index + 3] = 0; indices[i_index + 4] = v_index + 1; indices[i_index + 5] = 1;
                }
                v_index += 2;
                i_index += 6;
            }
            //CreateNavLines
            var offsetOuter = offset;
            var offsetInner = 1f - (navSize0to1);
            steppercentage = 1f / (float)(navSegments);
            for (index = 0; index < navSegments; index++)
            {
                if (index % largeSegmentModulator == 0)
                    offsetInner = 1f - (navSize0to1);
                else
                    offsetInner = 1f - (navSize0to1 * navSmallLargeRatio);
                // first set of vertices
                u = (float)(index) * steppercentage;
                radians = u * pi2;
                x = ((float)(Math.Sin(radians)) * .5f) + centering;
                y = ((float)(Math.Cos(radians)) * .5f) + centering;
                vertices[v_index + 0] = new VertexPositionTexture(ReOrient(new Vector3(x * offsetOuter, y * offsetOuter, 0)), new Vector2(u, 0f));
                vertices[v_index + 1] = new VertexPositionTexture(ReOrient(new Vector3(x * offsetInner, y * offsetInner, 0)), new Vector2(u, 1f));
                // second set of vertices
                u = (float)(index + lineSize0to1) * steppercentage;
                // just make the 0 line slightly larger as its special.
                if (index == 0)
                    u = (float)(index + lineSize0to1 * zerodegreelinethickener) * steppercentage;
                radians = u * pi2;
                x = ((float)(Math.Sin(radians)) * .5f) + centering;
                y = ((float)(Math.Cos(radians)) * .5f) + centering;
                vertices[v_index + 2] = new VertexPositionTexture(ReOrient(new Vector3(x * offsetOuter, y * offsetOuter, 0)), new Vector2(u, 0f));
                vertices[v_index + 3] = new VertexPositionTexture(ReOrient(new Vector3(x * offsetInner, y * offsetInner, 0)), new Vector2(u, 1f));
                // indices
                indices[i_index + 0] = v_index + 0; indices[i_index + 1] = v_index + 1; indices[i_index + 2] = v_index + 2;
                indices[i_index + 3] = v_index + 2; indices[i_index + 4] = v_index + 1; indices[i_index + 5] = v_index + 3;
                v_index += 4;
                i_index += 6;
            }

        }

        Vector3 ReOrient(Vector3 v)
        {
            if (OrientationOptionRightUpForward == 1)
                v = new Vector3(v.Z, v.X, v.Y);
            if (OrientationOptionRightUpForward == 2)
                v = new Vector3(v.X, v.Z, v.Y);
            return v;
        }

        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionTexture.VertexDeclaration);
            }
        }
    }

    public class Line
    {
        VertexPositionColor[] vertices;
        int[] indices;

        public Vector3 camUp = Vector3.Up;

        public Line(float linewidth, Color c, Vector3 start, Vector3 end)
        {
            CreateLine(linewidth, c, start, end);
        }

        private void CreateLine(float linewidth, Color c, Vector3 start, Vector3 end)
        {
            var a = end -start;
            a.Normalize();
            var b = Vector3.Up;
            float n = Vector3.Dot(a, b);
            if (n*n >.95f)
                b = Vector3.Right;
            var su = Vector3.Cross(a, b);
            var sr = Vector3.Cross(a, su);
            var offsetup = su * linewidth;
            var offsetright = sr * linewidth;

            Vector3 s0 = start + offsetright - offsetup;
            Vector3 s1 = start - offsetright - offsetup;
            Vector3 s2 = start + offsetup;

            Vector3 e0 = end + offsetright - offsetup;
            Vector3 e1 = end - offsetright - offsetup;
            Vector3 e2 = end + offsetup;

            var cs = c * .4f;
            cs.A = c.A;

            vertices = new VertexPositionColor[12];
            indices = new int[18];

            int v = 0;
            int i = 0;
            // q1
            vertices[v].Position = s0; vertices[v].Color = cs; v++;
            vertices[v].Position = s1; vertices[v].Color = cs; v++;
            vertices[v].Position = e0; vertices[v].Color = c; v++;
            vertices[v].Position = e1; vertices[v].Color = c; v++;

            var vi = v - 4;
            indices[i + 0] = vi + 0; indices[i+1] = vi + 1; indices[i+2] = vi + 2; 
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;

            // q2
            vertices[v].Position = s1; vertices[v].Color = cs; v++;
            vertices[v].Position = s2; vertices[v].Color = cs; v++;
            vertices[v].Position = e1; vertices[v].Color = c; v++;
            vertices[v].Position = e2; vertices[v].Color = c; v++;

             vi = v - 4;
            indices[i + 0] = vi + 0; indices[i + 1] = vi + 1; indices[i + 2] = vi + 2;
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;

            //q3
            vertices[v].Position = s2; vertices[v].Color = cs; v++;
            vertices[v].Position = s0; vertices[v].Color = cs; v++;
            vertices[v].Position = e2; vertices[v].Color = c; v++;
            vertices[v].Position = e0; vertices[v].Color = c; v++;

            vi = v - 4;
            indices[i + 0] = vi + 0; indices[i + 1] = vi + 1; indices[i + 2] = vi + 2;
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColor.VertexDeclaration);
            }
        }
    }

    public class LinePCT
    {
        VertexPositionColorTexture[] vertices;
        int[] indices;

        public Vector3 camUp = Vector3.Up;

        public LinePCT(float linewidth, Color c, Vector3 start, Vector3 end)
        {
            CreateLine(linewidth, c, c, start, end);
        }
        public LinePCT(float linewidth, Color colorStart, Color colorEnd, Vector3 start, Vector3 end)
        {
            CreateLine(linewidth, colorStart, colorEnd, start, end);
        }

        private void CreateLine(float linewidth, Color cs, Color ce, Vector3 start, Vector3 end)
        {
            var a = end - start;
            a.Normalize();
            var b = Vector3.Up;
            float n = Vector3.Dot(a, b);
            if (n * n > .95f)
                b = Vector3.Right;
            var su = Vector3.Cross(a, b);
            var sr = Vector3.Cross(a, su);
            var offsetup = su * linewidth;
            var offsetright = sr * linewidth;

            Vector3 s0 = start + offsetright - offsetup;
            Vector3 s1 = start - offsetright - offsetup;
            Vector3 s2 = start + offsetup;

            Vector3 e0 = end + offsetright - offsetup;
            Vector3 e1 = end - offsetright - offsetup;
            Vector3 e2 = end + offsetup;

            Vector2 uv0 = new Vector2(0f, 1f);
            Vector2 uv1 = new Vector2(0f, 0f);
            Vector2 uv2 = new Vector2(1f, 0f);
            Vector2 uv3 = new Vector2(1f, 1f);

            vertices = new VertexPositionColorTexture[12];
            indices = new int[18];

            int v = 0;
            int i = 0;
            // q1
            vertices[v].Position = s0; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv0; v++;
            vertices[v].Position = s1; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv1; v++;
            vertices[v].Position = e0; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv2; v++;
            vertices[v].Position = e1; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv3; v++;

            var vi = v - 4;
            indices[i + 0] = vi + 0; indices[i + 1] = vi + 1; indices[i + 2] = vi + 2;
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;

            // q2
            vertices[v].Position = s1; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv0; v++;
            vertices[v].Position = s2; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv1; v++;
            vertices[v].Position = e1; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv2; v++;
            vertices[v].Position = e2; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv3; v++;

            vi = v - 4;
            indices[i + 0] = vi + 0; indices[i + 1] = vi + 1; indices[i + 2] = vi + 2;
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;

            //q3
            vertices[v].Position = s2; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv0; v++;
            vertices[v].Position = s0; vertices[v].Color = cs; vertices[v].TextureCoordinate = uv1; v++;
            vertices[v].Position = e2; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv2; v++;
            vertices[v].Position = e0; vertices[v].Color = ce; vertices[v].TextureCoordinate = uv3; v++;

            vi = v - 4;
            indices[i + 0] = vi + 0; indices[i + 1] = vi + 1; indices[i + 2] = vi + 2;
            indices[i + 3] = vi + 2; indices[i + 4] = vi + 1; indices[i + 5] = vi + 3;
            i += 6;
        }
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionColorTexture.VertexDeclaration);
            }
        }
    }

    #endregion
}
