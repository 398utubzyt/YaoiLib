using Terraria;

namespace YaoiLib
{
    public static class PlayerExtensions
    {
        /// <summary>
        /// Gets if the player belongs to the current game instance.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns><see langword"true"/> if the player is a local player; otherwise, <see langword="false"/>.</returns>
        public static bool IsLocalPlayer(this Player player)
            => player.whoAmI == Main.myPlayer;
    }
}
