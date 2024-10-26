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

        /// <summary>
        /// Calculates the player's current pickaxe power using the best currently available pickaxe.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="power">The highest possible pickaxe power of the player.</param>
        /// <returns><see langword"true"/> if the player has a pickaxe; otherwise, <see langword="false"/>.</returns>
        public static bool CalculatePickaxePower(this Player player, out int power)
        {
            Item pickaxe = player.GetBestPickaxe();
            bool result = pickaxe != null;

            if (result)
                power = pickaxe.pick;
            else
                power = 0;

            return result;
        }
    }
}
