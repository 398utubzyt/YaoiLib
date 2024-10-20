using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace YaoiLib
{
    public static class ListExtensions
    {
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire <see cref="List{T}"/> through <paramref name="result"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements within the <see cref="List{T}"/>.</typeparam>
        /// <param name="self">The <see cref="List{T}"/> to search.</param>
        /// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <param name="result">
        /// The first element that matches the conditions defined by the specified predicate, if found;
        /// otherwise, it is undefined.
        /// </param>
        /// <returns><see langword="true"/> if an element was found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static bool TryFind<T>(this List<T> self, Predicate<T> predicate, [MaybeNullWhen(false)] out T result)
        {
            ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
            Unsafe.SkipInit(out result);
            
            for (int i = 0; i < self.Count; i++)
            {
                if (predicate(self[i]))
                {
                    result = self[i];
                    return true;
                }
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                result = default;
            }
            return false;
        }
    }
}
