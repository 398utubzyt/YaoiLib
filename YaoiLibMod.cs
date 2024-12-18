using Terraria.ModLoader;

using YaoiLib.Terraria;
using YaoiLib.Terraria.Graphics;

namespace YaoiLib
{
	public class YaoiLibMod : Mod
	{
        public override void Load()
        {
            TimeHelper.Load();
            RainHelper.Load();
            EventHelper.Load();
            MusicHelper.Load();
            NpcHelper.Load();
        }

        public override void Unload()
        {
            NpcHelper.Unload();
            TimeHelper.Unload();
            RainHelper.Unload();
            EventHelper.Unload();
        }
    }
}
