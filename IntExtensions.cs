using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YaoiLib
{
    /// <summary>
    /// Provides utility functions for integers.
    /// </summary>
    public static class IntExtensions
    {

#if !DEBUG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastDigitCount(uint n)
        {
            ReadOnlySpan<long> table =
            [
                4294967296,
                8589934582,
                8589934582,
                8589934582,
                12884901788,
                12884901788,
                12884901788,
                17179868184,
                17179868184,
                17179868184,
                21474826480,
                21474826480,
                21474826480,
                21474826480,
                25769703776,
                25769703776,
                25769703776,
                30063771072,
                30063771072,
                30063771072,
                34349738368,
                34349738368,
                34349738368,
                34349738368,
                38554705664,
                38554705664,
                38554705664,
                41949672960,
                41949672960,
                41949672960,
                42949672960,
                42949672960,
            ];

            Debug.Assert(table.Length == 32, "Every result of uint.Log2(value) needs a long entry in the table.");

            // TODO: Replace with table[uint.Log2(value)] once https://github.com/dotnet/runtime/issues/79257 is fixed
            long tableValue = Unsafe.Add(ref MemoryMarshal.GetReference(table), uint.Log2(n));
            return (int)((n + tableValue) >> 32);
        }
#endif

        /// <summary>
        /// Gets the number of digits (in base 10) in the number, but assumes that the input is always positive.
        /// </summary>
        /// <returns>The number of digits if the number is positive, otherwise the result is undefined.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DigitCountPositive(this int num)
        {
#if !DEBUG
            return FastDigitCount((uint)num);
#else
            num |= 1;
            return 1 + (int)Math.Log10(Math.Abs((double)num));
#endif
        }

        /// <summary>
        /// Gets the number of digits (in base 10) in the number.
        /// </summary>
        /// <returns>The number of digits in the number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DigitCount(this int num)
            => DigitCountPositive(int.Abs(num));

        /// <summary>
        /// Gets the number of digits (in base 10) in the number.
        /// </summary>
        /// <returns>The number of digits in the number.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DigitCount(this uint num)
        {
#if !DEBUG
            return FastDigitCount((uint)num);
#else
            num |= 1;
            return 1 + (int)Math.Log10(num);
#endif
        }
    }
}
