using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public sealed class ValueRange<[Nullable(0)] T> : IEquatable<ValueRange<T>> where T : IComparable<T>
    {
        [NullableContext(2)]
        public ValueRange(T lower, T upper, bool isLowerInclusive, bool isUpperInclusive, bool hasLower, bool hasUpper)
        {
            if (hasLower && lower == null)
            {
                throw new ArgumentNullException("lower");
            }
            this.Lower = lower;
            if (!hasUpper || upper != null)
            {
                this.Upper = upper;
                this.HasLower = hasLower;
                this.HasUpper = hasUpper;
                this.IsLowerInclusive = isLowerInclusive;
                this.IsUpperInclusive = isUpperInclusive;
                base..ctor();
                return;
            }
            throw new ArgumentNullException("upper");
        }

        [Nullable(2)]
        public T Lower
        {
            [NullableContext(2)]
            get;
            [NullableContext(2)]
            private set;
        }

        [Nullable(2)]
        public T Upper
        {
            [NullableContext(2)]
            get;
            [NullableContext(2)]
            private set;
        }

        public bool HasLower { get; private set; }

        public bool HasUpper { get; private set; }

        public void ClearLower()
        {
            this.Lower = default(T);
            this.HasLower = false;
        }

        public void ClearUpper()
        {
            this.Upper = default(T);
            this.HasUpper = false;
        }

        public void SetLower(T lower, bool isInclusive)
        {
            this.Lower = lower;
            this.HasLower = true;
            this.IsLowerInclusive = isInclusive;
        }

        public void SetUpper(T upper, bool isInclusive)
        {
            this.Upper = upper;
            this.HasUpper = true;
            this.IsUpperInclusive = isInclusive;
        }

        public static ValueRange<T> Closed(T lower, T upper)
        {
            return new ValueRange<T>(lower, upper, true, true, true, true);
        }

        public static ValueRange<T> ClosedOpen(T lower, T upper)
        {
            return new ValueRange<T>(lower, upper, true, false, true, true);
        }

        public static ValueRange<T> OpenClosed(T lower, T upper)
        {
            return new ValueRange<T>(lower, upper, false, true, true, true);
        }

        public static ValueRange<T> Open(T lower, T upper)
        {
            return new ValueRange<T>(lower, upper, false, false, true, true);
        }

        public static ValueRange<T> Exactly(T value)
        {
            return new ValueRange<T>(value, value, true, true, true, true);
        }

        public static ValueRange<T> AtLeast(T lower)
        {
            return new ValueRange<T>(lower, default(T), true, true, true, false);
        }

        public static ValueRange<T> GreaterThan(T lower)
        {
            return new ValueRange<T>(lower, default(T), false, true, true, false);
        }

        public static ValueRange<T> AtMost(T upper)
        {
            return new ValueRange<T>(default(T), upper, true, true, false, true);
        }

        public static ValueRange<T> LessThan(T upper)
        {
            return new ValueRange<T>(default(T), upper, true, false, false, true);
        }

        public static ValueRange<T> All()
        {
            return new ValueRange<T>(default(T), default(T), true, true, false, false);
        }

        public static ValueRange<T> FromString(string str, [Nullable(new byte[] { 2, 1, 1 })] Func<string, T> parseFunc = null)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("不能为空");
            }
            bool flag = str[0] == '[';
            bool flag2 = str[str.Length - 1] == ']';
            if (!flag && str[0] != '(')
            {
                throw new ArgumentException("开头必须是 [ 或 (");
            }
            if (!flag2 && str[str.Length - 1] != ')')
            {
                throw new ArgumentException("结尾必须是 ] 或 )");
            }
            int num = str.IndexOfAny(new char[] { ',', '，' });
            if (num == -1)
            {
                throw new ArgumentException("需要用逗号分隔两个值");
            }
            string text = str.Substring(1, num - 1).Trim();
            int num2 = num + 1;
            string text2 = str.Substring(num2, str.Length - 1 - num2).Trim();
            T t = default(T);
            T t2 = default(T);
            bool flag3 = false;
            bool flag4 = false;
            if ((text == null || text.Length != 0) && !(text == "-∞"))
            {
                t = ((parseFunc != null) ? parseFunc(text) : ((T)((object)Convert.ChangeType(text, typeof(T)))));
                flag3 = true;
            }
            if ((text2 == null || text2.Length != 0) && !(text2 == "+∞") && !(text2 == "∞"))
            {
                t2 = ((parseFunc != null) ? parseFunc(text2) : ((T)((object)Convert.ChangeType(text2, typeof(T)))));
                flag4 = true;
            }
            return new ValueRange<T>(t, t2, flag, flag2, flag3, flag4);
        }

        public override string ToString()
        {
            string[] array = new string[5];
            array[0] = ((this.HasLower && this.IsLowerInclusive) ? "[" : "(");
            int num = 1;
            string text;
            if (!this.HasLower)
            {
                text = "-∞";
            }
            else
            {
                T t = this.Lower;
                text = t.ToString();
            }
            array[num] = text;
            array[2] = ", ";
            int num2 = 3;
            string text2;
            if (!this.HasUpper)
            {
                text2 = "+∞";
            }
            else
            {
                T t = this.Upper;
                text2 = t.ToString();
            }
            array[num2] = text2;
            array[4] = ((this.HasUpper && this.IsUpperInclusive) ? "]" : ")");
            return string.Concat(array);
        }

        public bool Contains(T value)
        {
            if (this.HasLower)
            {
                ref T ptr = ref value;
                if (default(T) == null)
                {
                    T t = value;
                    ptr = ref t;
                }
                int num = ptr.CompareTo(this.Lower);
                if (num < 0)
                {
                    return false;
                }
                if (num == 0 && !this.IsLowerInclusive)
                {
                    return false;
                }
            }
            if (this.HasUpper)
            {
                ref T ptr2 = ref value;
                if (default(T) == null)
                {
                    T t = value;
                    ptr2 = ref t;
                }
                int num2 = ptr2.CompareTo(this.Upper);
                if (num2 > 0)
                {
                    return false;
                }
                if (num2 == 0 && !this.IsUpperInclusive)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEmpty()
        {
            if (!this.HasLower || !this.HasUpper)
            {
                return false;
            }
            T lower = this.Lower;
            int num = lower.CompareTo(this.Upper);
            return num > 0 || (num == 0 && (!this.IsLowerInclusive || !this.IsUpperInclusive));
        }

        public ValueRange<T> Intersect(ValueRange<T> other)
        {
            T t = default(T);
            T t2 = default(T);
            bool flag = true;
            bool flag2 = true;
            if (this.HasLower && other.HasLower)
            {
                T t3 = this.Lower;
                int num = t3.CompareTo(other.Lower);
                if (num <= 0)
                {
                    if (num >= 0)
                    {
                        t = this.Lower;
                        flag = this.IsLowerInclusive && other.IsLowerInclusive;
                    }
                    else
                    {
                        t = other.Lower;
                        flag = other.IsLowerInclusive;
                    }
                }
                else
                {
                    t = this.Lower;
                    flag = this.IsLowerInclusive;
                }
            }
            else if (this.HasLower)
            {
                t = this.Lower;
                flag = this.IsLowerInclusive;
            }
            else if (other.HasLower)
            {
                t = other.Lower;
                flag = other.IsLowerInclusive;
            }
            if (this.HasUpper && other.HasUpper)
            {
                T t3 = this.Upper;
                int num = t3.CompareTo(other.Upper);
                if (num >= 0)
                {
                    if (num <= 0)
                    {
                        t2 = this.Upper;
                        flag2 = this.IsUpperInclusive && other.IsUpperInclusive;
                    }
                    else
                    {
                        t2 = other.Upper;
                        flag2 = other.IsUpperInclusive;
                    }
                }
                else
                {
                    t2 = this.Upper;
                    flag2 = this.IsUpperInclusive;
                }
            }
            else if (this.HasUpper)
            {
                t2 = this.Upper;
                flag2 = this.IsUpperInclusive;
            }
            else if (other.HasUpper)
            {
                t2 = other.Upper;
                flag2 = other.IsUpperInclusive;
            }
            bool flag3 = this.HasLower || other.HasLower;
            bool flag4 = this.HasUpper || other.HasUpper;
            return new ValueRange<T>(t, t2, flag, flag2, flag3, flag4);
        }

        [NullableContext(2)]
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ValueRange<T>);
        }

        public bool Equals([Nullable(new byte[] { 2, 1 })] ValueRange<T> other)
        {
            return other != null && EqualityComparer<T>.Default.Equals(this.Lower, other.Lower) && EqualityComparer<T>.Default.Equals(this.Upper, other.Upper) && this.HasLower == other.HasLower && this.HasUpper == other.HasUpper && this.IsLowerInclusive == other.IsLowerInclusive && this.IsUpperInclusive == other.IsUpperInclusive;
        }

        public override int GetHashCode()
        {
            return (((((169504101 * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.Lower)) * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.Upper)) * -1521134295 + this.HasLower.GetHashCode()) * -1521134295 + this.HasUpper.GetHashCode()) * -1521134295 + this.IsLowerInclusive.GetHashCode()) * -1521134295 + this.IsUpperInclusive.GetHashCode();
        }

        public static bool operator ==([Nullable(new byte[] { 2, 1 })] ValueRange<T> left, [Nullable(new byte[] { 2, 1 })] ValueRange<T> right)
        {
            return EqualityComparer<ValueRange<T>>.Default.Equals(left, right);
        }

        public static bool operator !=([Nullable(new byte[] { 2, 1 })] ValueRange<T> left, [Nullable(new byte[] { 2, 1 })] ValueRange<T> right)
        {
            return !(left == right);
        }

        public bool IsLowerInclusive;

        public bool IsUpperInclusive;
    }
}
