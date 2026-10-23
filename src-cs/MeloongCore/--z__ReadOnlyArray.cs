using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>z__ReadOnlyArray<T> : IEnumerable, ICollection, IList, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection<T>, IList<T>
{
    public <>z__ReadOnlyArray(T[] items)
    {
        this._items = items;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._items.GetEnumerator();
    }

    int ICollection.Count
    {
        get
        {
            return this._items.Length;
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
        this._items.CopyTo(array, index);
    }

    object IList.this[int index]
    {
        get
        {
            return this._items[index];
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
        return this._items.Contains(value);
    }

    int IList.IndexOf(object value)
    {
        return this._items.IndexOf(value);
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
        return this._items.GetEnumerator();
    }

    int IReadOnlyCollection<T>.Count
    {
        get
        {
            return this._items.Length;
        }
    }

    T IReadOnlyList<T>.this[int index]
    {
        get
        {
            return this._items[index];
        }
    }

    int ICollection<T>.Count
    {
        get
        {
            return this._items.Length;
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
        return this._items.Contains(item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        this._items.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException();
    }

    T IList<T>.this[int index]
    {
        get
        {
            return this._items[index];
        }
        set
        {
            throw new NotSupportedException();
        }
    }

    int IList<T>.IndexOf(T item)
    {
        return this._items.IndexOf(item);
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
    private readonly T[] _items;
}
