using System;
using System.Reflection;

using Terraria.ModLoader;

namespace YaoiLib
{
    public static class ModExtensions
    {
        /// <summary>
        /// Gets if the mod is currently loading.
        /// </summary>
        /// <param name="mod">The mod to check.</param>
        /// <returns><see langword="true"/> if the mod is loading; otherwise, <see langword="false"/>.</returns>
        public static bool IsLoading(this Mod mod)
        {
            return typeof(Mod).GetField("loading", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(mod) as bool? ?? false;
        }
    }
}
