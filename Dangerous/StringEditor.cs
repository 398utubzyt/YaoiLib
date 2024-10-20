using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YaoiLib.Dangerous
{
    /// <summary>
    /// Only use this if you know what you are doing.
    /// </summary>
    public static class StringEditor
    {
        public static void Edit(string str, int at, char c)
        {
            ArgumentNullException.ThrowIfNull(str, nameof(str));
            ArgumentOutOfRangeException.ThrowIfLessThan(at, 0, nameof(at));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(at, str.Length, nameof(at));

            Unsafe.Add(ref MemoryMarshal.GetReference<char>(str), at) = c;
        }
    }
}
