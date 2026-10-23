using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>z__ReadOnlySingleElementList<T> : IEnumerable, ICollection, IList, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection<T>, IList<T>
{
    public <>z__ReadOnlySingleElementList(T item)
    {
        this._item = item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new <>z__ReadOnlySingleElementList<T>.Enumerator(this._item);
    }

    int ICollection.Count
    {
        get
        {
            return 1;
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            return false;
        }
    }

    object ICollection.SyncRoot
    {
        get
        {
            return this;
        }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        array.SetValue(this._item, index);
    }

    object IList.this[int index]
    {
        get
        {
            if (index != 0)
            {
                throw new IndexOutOfRangeException();
            }
            return this._item;
        }
        set
        {
            throw new NotSupportedException();
        }
    }

    bool IList.IsFixedSize
    {
        get
        {
            return true;
        }
    }

    bool IList.IsReadOnly
    {
        get
        {
            return true;
        }
    }

    int IList.Add(object value)
    {
        throw new NotSupportedException();
    }

    void IList.Clear()
    {
        throw new NotSupportedException();
    }

    bool IList.Contains(object value)
    {
        return EqualityComparer<T>.Default.Equals(this._item, (T)((object)value));
    }

    int IList.IndexOf(object value)
    {
        if (!EqualityComparer<T>.Default.Equals(this._item, (T)((object)value)))
        {
            return -1;
        }
        return 0;
    }

    void IList.Insert(int index, object value)
    {
        throw new NotSupportedException();
    }

    void IList.Remove(object value)
    {
        throw new NotSupportedException();
    }

    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new <>z__ReadOnlySingleElementList<T>.Enumerator(this._item);
    }

    int IReadOnlyCollection<T>.Count
    {
        get
        {
            return 1;
        }
    }

    T IReadOnlyList<T>.this[int index]
    {
        get
        {
            if (index != 0)
            {
                throw new IndexOutOfRangeException();
            }
            return this._item;
        }
    }

    int ICollection<T>.Count
    {
        get
        {
            return 1;
        }
    }

    bool ICollection<T>.IsReadOnly
    {
        get
        {
            return true;
        }
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T item)
    {
        return EqualityComparer<T>.Default.Equals(this._item, item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        array[arrayIndex] = this._item;
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException();
    }

    T IList<T>.this[int index]
    {
        get
        {
            if (index != 0)
            {
                throw new IndexOutOfRangeException();
            }
            return this._item;
        }
        set
        {
            throw new NotSupportedException();
        }
    }

    int IList<T>.IndexOf(T item)
    {
        if (!EqualityComparer<T>.Default.Equals(this._item, item))
        {
            return -1;
        }
        return 0;
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new NotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    [CompilerGenerated]
    private readonly T _item;

    private sealed class Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        public Enumerator(T item)
        {
            this.System.Collections.Generic.IEnumerator<T>.Current = item;
        }

        object IEnumerator.Current
        {
            get
            {
                return this._item;
            }
        }

        T IEnumerator<T>.Current
        {
            get
            {
                return this._item;
            }
        }

        bool IEnumerator.MoveNext()
        {
            return !this._moveNextCalled && (this._moveNextCalled = true);
        }

        void IEnumerator.Reset()
        {
            this._moveNextCalled = false;
        }

        void IDisposable.Dispose()
        {
        }

        [CompilerGenerated]
        private readonly T _item;

        [CompilerGenerated]
        private bool _moveNextCalled;
    }
}
