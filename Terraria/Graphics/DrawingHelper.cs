using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace YaoiLib.Terraria.Graphics
{
    public static class DrawingHelper
    {
        private static readonly int[] _pixelTexData = [ 1 ];
        private static Texture2D _pixelTex;

        internal static void Load()
        {
            _pixelTex = new(Main.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixelTex.SetData(_pixelTexData);
        }

        internal static void Unload()
        {
            _pixelTex.Dispose();
        }

        /// <summary>
        /// Draws a rectangle using screen-space coordinates.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        /// <param name="rect">The rectangle size and position.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRect(SpriteBatch batch, Rectangle rect, Color color)
        {
            if (_pixelTex != null)
                batch.Draw(_pixelTex, rect, color);
        }
    }
}
