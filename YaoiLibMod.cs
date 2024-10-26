using Terraria.ModLoader;

using YaoiLib.Terraria;

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
        }

        public override void Unload()
        {
            TimeHelper.Unload();
            RainHelper.Unload();
            EventHelper.Unload();
        }
    }
}
