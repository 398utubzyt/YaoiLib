using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ReLogic.Content;

using Terraria;
using Terraria.GameContent;

namespace YaoiLib.Terraria.Graphics
{
    public static class DrawingHelper
    {
        /// <summary>
        /// Draws a rectangle using screen-space coordinates.
        /// </summary>
        /// <param name="batch">The sprite batch to draw with.</param>
        /// <param name="rect">The rectangle size and position.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRect(SpriteBatch batch, Rectangle rect, Color color)
        {
            if (TextureAssets.MagicPixel != null)
                batch.Draw(TextureAssets.MagicPixel.Value, rect, color);
        }
    }
}
