using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YaoiLib.Dangerous
{
    /// <summary>
    /// Only use this if you know what you are doing.
    /// </summary>
    public struct FixedStringBuilder(int size)
    {
        private readonly string _buffer = new string('\0', size);
        private int _pos = 0;

        private readonly void ThrowIfOutOfBounds(int length)
        {
            if (_buffer.Length - _pos < length)
                throw new InvalidOperationException();
        }

        private readonly void ThrowIfNotFilled()
        {
            if (_buffer.Length != _pos)
                throw new InvalidOperationException();
        }

        public override readonly string ToString()
        {
            ThrowIfNotFilled();
            return _buffer;
        }

        public void Append(string str)
        {
            ThrowIfOutOfBounds(str.Length);
            str.CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference<char>(_buffer), _pos), _buffer.Length - _pos));
            _pos += str.Length;
        }

        public void Append(ReadOnlySpan<char> chars)
        {
            ThrowIfOutOfBounds(chars.Length);
            chars.CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference<char>(_buffer), _pos), _buffer.Length - _pos));
            _pos += chars.Length;
        }

        public void Append(char c)
        {
            ThrowIfOutOfBounds(1);
            Unsafe.Add(ref MemoryMarshal.GetReference<char>(_buffer), _pos++) = c;
        }

        public void Append(int num)
        {
            int num2 = num.DigitCount();
            bool isNegative = num2 < 0;
            if (isNegative)
                ++num2;
            ThrowIfOutOfBounds(num2);

            _pos += num2;
            ref char c = ref Unsafe.Add(ref MemoryMarshal.GetReference<char>(_buffer), _pos);

            do
            {
                c = ref Unsafe.Subtract(ref c, 1);
                (num, num2) = Math.DivRem(num, 10);
                c = (char)('0' + num2);
            } while (num > 10);

            if (isNegative)
            {
                Unsafe.Subtract(ref c, 1) = '-';
            }
        }

        public void Append(uint num)
        {
            uint num2 = (uint)num.DigitCount();
            ThrowIfOutOfBounds((int)num2);

            _pos += (int)num2;
            ref char c = ref Unsafe.Add(ref MemoryMarshal.GetReference<char>(_buffer), _pos);

            do
            {
                c = ref Unsafe.Subtract(ref c, 1);
                (num, num2) = Math.DivRem(num, 10u);
                c = (char)('0' + num2);
            } while (num > 10);
        }
    }
}
