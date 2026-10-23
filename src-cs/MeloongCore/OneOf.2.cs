using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public readonly struct OneOf<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>
    {
        [return: Nullable(new byte[] { 0, 1, 1, 1 })]
        public static implicit operator OneOf<T1, T2, T3>(T1 value)
        {
            return new OneOf<T1, T2, T3>(1, value, default(T2), default(T3));
        }

        [return: Nullable(new byte[] { 0, 1, 1, 1 })]
        public static implicit operator OneOf<T1, T2, T3>(T2 value)
        {
            return new OneOf<T1, T2, T3>(2, default(T1), value, default(T3));
        }

        [return: Nullable(new byte[] { 0, 1, 1, 1 })]
        public static implicit operator OneOf<T1, T2, T3>(T3 value)
        {
            return new OneOf<T1, T2, T3>(3, default(T1), default(T2), value);
        }

        public static implicit operator T1([Nullable(new byte[] { 0, 1, 1, 1 })] OneOf<T1, T2, T3> value)
        {
            return value.As<T1>();
        }

        public static implicit operator T2([Nullable(new byte[] { 0, 1, 1, 1 })] OneOf<T1, T2, T3> value)
        {
            return value.As<T2>();
        }

        public static implicit operator T3([Nullable(new byte[] { 0, 1, 1, 1 })] OneOf<T1, T2, T3> value)
        {
            return value.As<T3>();
        }

        [NullableContext(2)]
        private OneOf(int index, T1 value1, T2 value2, T3 value3)
        {
            this._index = -1;
            this._index = index;
            this._value1 = value1;
            this._value2 = value2;
            this._value3 = value3;
        }

        public TResult Switch<[Nullable(2)] TResult>(Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3)
        {
            TResult tresult;
            switch (this._index)
            {
            case 1:
                tresult = f1(this._value1);
                break;
            case 2:
                tresult = f2(this._value2);
                break;
            case 3:
                tresult = f3(this._value3);
                break;
            default:
                throw new InvalidOperationException();
            }
            return tresult;
        }

        public void Switch(Action<T1> f1, Action<T2> f2, Action<T3> f3)
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
            if (this._index == 3)
            {
                f3(this._value3);
                return;
            }
            throw new InvalidOperationException();
        }

        [NullableContext(2)]
        public bool Is<T>()
        {
            switch (this._index)
            {
            case 1:
                if (typeof(T) == typeof(T1))
                {
                    return true;
                }
                break;
            case 2:
                if (typeof(T) == typeof(T2))
                {
                    return true;
                }
                break;
            case 3:
                if (typeof(T) == typeof(T3))
                {
                    return true;
                }
                break;
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

        public bool IsT3()
        {
            return this._index == 3;
        }

        public T As<[Nullable(2)] T>()
        {
            switch (this._index)
            {
            case 1:
                if (typeof(T) == typeof(T1))
                {
                    return (T)((object)this._value1);
                }
                break;
            case 2:
                if (typeof(T) == typeof(T2))
                {
                    return (T)((object)this._value2);
                }
                break;
            case 3:
                if (typeof(T) == typeof(T3))
                {
                    return (T)((object)this._value3);
                }
                break;
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

        public T3 AsT3()
        {
            if (this._index == 3)
            {
                return this._value3;
            }
            throw new InvalidOperationException();
        }

        private readonly int _index;

        [Nullable(2)]
        private readonly T1 _value1;

        [Nullable(2)]
        private readonly T2 _value2;

        [Nullable(2)]
        private readonly T3 _value3;
    }
}
