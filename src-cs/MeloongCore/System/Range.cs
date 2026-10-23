using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
    [ExcludeFromCodeCoverage]
    internal readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }

        public Index End { get; }

        public Range(Index start, Index end)
        {
            this.Start = start;
            this.End = end;
        }

        [NullableContext(2)]
        public override bool Equals([NotNullWhen(true)] object value)
        {
            if (value is Range)
            {
                Range range = (Range)value;
                if (range.Start.Equals(this.Start))
                {
                    return range.End.Equals(this.End);
                }
            }
            return false;
        }

        public bool Equals(Range other)
        {
            return other.Start.Equals(this.Start) && other.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return Range.HashHelpers.Combine(this.Start.GetHashCode(), this.End.GetHashCode());
        }

        [NullableContext(1)]
        public override string ToString()
        {
            return this.Start.ToString() + ".." + this.End.ToString();
        }

        public static Range StartAt(Index start)
        {
            return new Range(start, Index.End);
        }

        public static Range EndAt(Index end)
        {
            return new Range(Index.Start, end);
        }

        public static Range All
        {
            get
            {
                return new Range(Index.Start, Index.End);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: TupleElementNames(new string[] { "Offset", "Length" })]
        public ValueTuple<int, int> GetOffsetAndLength(int length)
        {
            Index start = this.Start;
            int num;
            if (start.IsFromEnd)
            {
                num = length - start.Value;
            }
            else
            {
                num = start.Value;
            }
            Index end = this.End;
            int num2;
            if (end.IsFromEnd)
            {
                num2 = length - end.Value;
            }
            else
            {
                num2 = end.Value;
            }
            if (num2 > length || num > num2)
            {
                Range.ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            return new ValueTuple<int, int>(num, num2 - num);
        }

        private static class HashHelpers
        {
            public static int Combine(int h1, int h2)
            {
                return (((h1 << 5) | (int)((uint)h1 >> 27)) + h1) ^ h2;
            }
        }

        private static class ThrowHelper
        {
            [DoesNotReturn]
            public static void ThrowArgumentOutOfRangeException()
            {
                throw new ArgumentOutOfRangeException("length");
            }
        }
    }
}
