using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace YaoiLib
{
    public ref struct CallArgumentList(ReadOnlySpan<object> args)
    {
        private readonly ReadOnlySpan<object> _args = args;
        private int _index = 0;

        public bool TryGetNext<T>([MaybeNullWhen(false)] out T value)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            bool result = _index < _args.Length && _args[_index] is T t;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            Unsafe.SkipInit(out t); // No-op because intellisense is stupid.
            
            if (result)
            {
                value = t;
                ++_index;
            } else
            {
                value = default;
            }

            return result;
        }

        public bool TryGetNextOptional<T>([MaybeNullWhen(false)] out T value)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            bool result = _index < _args.Length && _args[_index++] is T t;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            Unsafe.SkipInit(out t); // No-op because intellisense is stupid.

            value = result ? t : default;

            return result;
        }
    }
}
