using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
    [ExcludeFromCodeCoverage]
    internal readonly struct Index : IEquatable<Index>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
            {
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }
            if (fromEnd)
            {
                this._value = ~value;
                return;
            }
            this._value = value;
        }

        private Index(int value)
        {
            this._value = value;
        }

        public static Index Start
        {
            get
            {
                return new Index(0);
            }
        }

        public static Index End
        {
            get
            {
                return new Index(-1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromStart(int value)
        {
            if (value < 0)
            {
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }
            return new Index(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromEnd(int value)
        {
            if (value < 0)
            {
                Index.ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
            }
            return new Index(~value);
        }

        public int Value
        {
            get
            {
                if (this._value < 0)
                {
                    return ~this._value;
                }
                return this._value;
            }
        }

        public bool IsFromEnd
        {
            get
            {
                return this._value < 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length)
        {
            int num = this._value;
            if (this.IsFromEnd)
            {
                num += length + 1;
            }
            return num;
        }

        [NullableContext(2)]
        public override bool Equals([NotNullWhen(true)] object value)
        {
            return value is Index && this._value == ((Index)value)._value;
        }

        public bool Equals(Index other)
        {
            return this._value == other._value;
        }

        public override int GetHashCode()
        {
            return this._value;
        }

        public static implicit operator Index(int value)
        {
            return Index.FromStart(value);
        }

        [NullableContext(1)]
        public override string ToString()
        {
            if (this.IsFromEnd)
            {
                return this.ToStringFromEnd();
            }
            return ((uint)this.Value).ToString();
        }

        [NullableContext(1)]
        private string ToStringFromEnd()
        {
            return "^" + this.Value.ToString();
        }

        private readonly int _value;

        private static class ThrowHelper
        {
            [DoesNotReturn]
            public static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
            {
                throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
            }
        }
    }
}
