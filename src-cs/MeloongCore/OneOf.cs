using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public readonly struct OneOf<[Nullable(2)] T1, [Nullable(2)] T2>
    {
        [return: Nullable(new byte[] { 0, 1, 1 })]
        public static implicit operator OneOf<T1, T2>(T1 value)
        {
            return new OneOf<T1, T2>(1, value, default(T2));
        }

        [return: Nullable(new byte[] { 0, 1, 1 })]
        public static implicit operator OneOf<T1, T2>(T2 value)
        {
            return new OneOf<T1, T2>(2, default(T1), value);
        }

        public static implicit operator T1([Nullable(new byte[] { 0, 1, 1 })] OneOf<T1, T2> value)
        {
            return value.As<T1>();
        }

        public static implicit operator T2([Nullable(new byte[] { 0, 1, 1 })] OneOf<T1, T2> value)
        {
            return value.As<T2>();
        }

        [NullableContext(2)]
        private OneOf(int index, T1 value1, T2 value2)
        {
            this._index = -1;
            this._index = index;
            this._value1 = value1;
            this._value2 = value2;
        }

        public TResult Switch<[Nullable(2)] TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2)
        {
            int index = this._index;
            TResult tresult;
            if (index != 1)
            {
                if (index != 2)
                {
                    throw new InvalidOperationException();
                }
                tresult = f2(this._value2);
            }
            else
            {
                tresult = f1(this._value1);
            }
            return tresult;
        }

        public void Switch(Action<T1> f1, Action<T2> f2)
        {
            if (this._index == 1)
            {
                f1(this._value1);
                return;
            }
            if (this._index == 2)
            {
                f2(this._value2);
                return;
            }
            throw new InvalidOperationException();
        }

        [NullableContext(2)]
        public bool Is<T>()
        {
            int index = this._index;
            if (index != 1)
            {
                if (index == 2)
                {
                    if (typeof(T) == typeof(T2))
                    {
                        return true;
                    }
                }
            }
            else if (typeof(T) == typeof(T1))
            {
                return true;
            }
            return false;
        }

        public bool IsT1()
        {
            return this._index == 1;
        }

        public bool IsT2()
        {
            return this._index == 2;
        }

        public T As<[Nullable(2)] T>()
        {
            int index = this._index;
            if (index != 1)
            {
                if (index == 2)
                {
                    if (typeof(T) == typeof(T2))
                    {
                        return (T)((object)this._value2);
                    }
                }
            }
            else if (typeof(T) == typeof(T1))
            {
                return (T)((object)this._value1);
            }
            throw new InvalidOperationException();
        }

        public T1 AsT1()
        {
            if (this._index == 1)
            {
                return this._value1;
            }
            throw new InvalidOperationException();
        }

        public T2 AsT2()
        {
            if (this._index == 2)
            {
                return this._value2;
            }
            throw new InvalidOperationException();
        }

        private readonly int _index;

        [Nullable(2)]
        private readonly T1 _value1;

        [Nullable(2)]
        private readonly T2 _value2;
    }
}
