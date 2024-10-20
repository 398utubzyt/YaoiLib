using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Terraria.ModLoader;
using YaoiLib.Dangerous;

namespace YaoiLib
{
    public static class TooltipLineListExtensions
    {
        public static bool TryFindVanillaTooltip(this List<TooltipLine> self, string name, [MaybeNullWhen(false)] out TooltipLine line)
        {
            for (int i = 0; i < self.Count; ++i)
            {
                line = self[i];
                if (line.Mod.Equals("Terraria", StringComparison.Ordinal))
                {
                    if (line.Name.Equals(name, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            line = null;
            return false;
        }

        public static bool TryFindVanillaTooltipTooltip(this List<TooltipLine> self, int number, [MaybeNullWhen(false)] out TooltipLine line)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(number, 0, nameof(number));
            FixedStringBuilder sb = new FixedStringBuilder(7 + number.DigitCount());

            sb.Append("Tooltip");
            // Cast to avoid negative checks.
            sb.Append((uint)number);

            string key = sb.ToString();

            for (int i = 0; i < self.Count; ++i)
            {
                line = self[i];
                if (line.Mod.Equals("Terraria", StringComparison.Ordinal))
                {
                    if (line.Name.Equals(key, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            line = null;
            return false;
        }
        public static bool TryFindModdedTooltip(this List<TooltipLine> self, Mod mod, string name, [MaybeNullWhen(false)] out TooltipLine line)
        {
            for (int i = 0; i < self.Count; ++i)
            {
                line = self[i];
                if (line.Mod.Equals(mod.Name, StringComparison.Ordinal))
                {
                    if (line.Name.Equals(name, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            line = null;
            return false;
        }
    }
}
