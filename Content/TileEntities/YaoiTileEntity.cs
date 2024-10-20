using Terraria;
using Terraria.ModLoader;

namespace YaoiLib.Content.TileEntities
{
    public abstract class YaoiTileEntity : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && IsValidTile(ref tile);
        }

        protected abstract bool IsValidTile(ref readonly Tile tile);
    }
}
