using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    public static class DrawHelpers
    {
        static SpriteBatch spriteBatch;
        static Texture2D dot;

        /// <summary>
        /// Flips atan direction to xna spritebatch rotational alignment defaults to true.
        /// </summary>
        public static bool SpriteBatchAtan2 = true;

        public static void Initialize(GraphicsDevice device, SpriteBatch spriteBatch, Texture2D dot)
        {
            DrawHelpers.spriteBatch = spriteBatch;
            if (DrawHelpers.dot == null)
                DrawHelpers.dot = dot;
            if (DrawHelpers.dot == null)
                DrawHelpers.dot = CreateDotTexture(device, Color.White);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Texture2D CreateDotTexture(GraphicsDevice device, Color color)
        {
            Color[] data = new Color[1] { color };
            Texture2D tex = new Texture2D(device, 1, 1);
            tex.SetData<Color>(data);
            return tex;
        }

        public static void DrawRectangleOutline(Rectangle r, int lineThickness, Color c)
        {
            DrawSquareBorder(r, lineThickness, c);
        }

        public static void DrawSquareBorder(Rectangle r, int lineThickness, Color c)
        {
            Rectangle TLtoR = new Rectangle(r.Left, r.Top, r.Width, lineThickness);
            Rectangle BLtoR = new Rectangle(r.Left, r.Bottom - lineThickness, r.Width, lineThickness);
            Rectangle LTtoB = new Rectangle(r.Left, r.Top, lineThickness, r.Height);
            Rectangle RTtoB = new Rectangle(r.Right - lineThickness, r.Top, lineThickness, r.Height);
            spriteBatch.Draw(dot, TLtoR, c);
            spriteBatch.Draw(dot, BLtoR, c);
            spriteBatch.Draw(dot, LTtoB, c);
            spriteBatch.Draw(dot, RTtoB, c);
        }

        public static void DrawCrossHair(Vector2 position, float radius, Color color)
        {
            DrawHelpers.DrawBasicLine(new Vector2(-radius, 0) + position, new Vector2(0 + radius, 0) + position, 1, color);
            DrawHelpers.DrawBasicLine(new Vector2(0, 0 - radius) + position, new Vector2(0, radius) + position, 1, color);
        }

        public static void DrawBasicLine(Vector2 s, Vector2 e, int thickness, Color linecolor)
        {
            spriteBatch.Draw(dot, new Rectangle((int)s.X, (int)s.Y, thickness, (int)Vector2.Distance(e, s)), new Rectangle(0, 0, 1, 1), linecolor, (float)Atan2Xna(e.X - s.X, e.Y - s.Y), Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawBasicPoint(Vector2 p, int size, Color c)
        {
            spriteBatch.Draw(dot, new Rectangle((int)p.X - size / 2, (int)p.Y - size / 2, 1 + size, 1 + size), new Rectangle(0, 0, 1, 1), c, 0.0f, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawBasicPoint(Vector2 p, Color c)
        {
            spriteBatch.Draw(dot, new Rectangle((int)p.X, (int)p.Y, 2, 2), new Rectangle(0, 0, 1, 1), c, 0.0f, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static float Atan2Xna(float difx, float dify)
        {
            if (SpriteBatchAtan2)
                return (float)System.Math.Atan2(difx, dify) * -1f;
            else
                return (float)System.Math.Atan2(difx, dify);
        }
    }
}
