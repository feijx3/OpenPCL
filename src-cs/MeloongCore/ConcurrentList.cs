using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class ConcurrentList<[Nullable(2)] T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
    {
        public ConcurrentList()
        {
        }

        public ConcurrentList(IEnumerable<T> data)
        {
            this._items = new List<T>(data);
        }

        public static implicit operator ConcurrentList<T>(List<T> data)
        {
            return new ConcurrentList<T>(data);
        }

        public static implicit operator List<T>(ConcurrentList<T> data)
        {
            object syncRoot = data.SyncRoot;
            List<T> list;
            lock (syncRoot)
            {
                list = new List<T>(data._items);
            }
            return list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            object syncRoot = this.SyncRoot;
            IEnumerator<T> enumerator;
            lock (syncRoot)
            {
                enumerator = this._items.ToList<T>().GetEnumerator();
            }
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            object syncRoot = this.SyncRoot;
            IEnumerator enumerator;
            lock (syncRoot)
            {
                enumerator = this._items.ToList<T>().GetEnumerator();
            }
            return enumerator;
        }

        public int Count
        {
            get
            {
                object syncRoot = this.SyncRoot;
                int count;
                lock (syncRoot)
                {
                    count = this._items.Count;
                }
                return count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.SyncRoot;
            }
        }

        public T this[int index]
        {
            get
            {
                object syncRoot = this.SyncRoot;
                T t;
                lock (syncRoot)
                {
                    t = this._items[index];
                }
                return t;
            }
            set
            {
                object syncRoot = this.SyncRoot;
                lock (syncRoot)
                {
                    this._items[index] = value;
                }
            }
        }

        [Nullable(2)]
        object IList.this[int index]
        {
            [NullableContext(2)]
            get
            {
                return this[index];
            }
            [NullableContext(2)]
            set
            {
                object syncRoot = this.SyncRoot;
                lock (syncRoot)
                {
                    this._items[index] = (T)((object)value);
                }
            }
        }

        public void Add(T item)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.Add(item);
            }
        }

        [NullableContext(2)]
        int IList.Add(object value)
        {
            object syncRoot = this.SyncRoot;
            int num;
            lock (syncRoot)
            {
                this._items.Add((T)((object)value));
                num = this._items.Count - 1;
            }
            return num;
        }

        public void Insert(int index, T item)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.Insert(index, item);
            }
        }

        [NullableContext(2)]
        void IList.Insert(int index, object value)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.Insert(index, (T)((object)value));
            }
        }

        public bool Remove(T item)
        {
            object syncRoot = this.SyncRoot;
            bool flag2;
            lock (syncRoot)
            {
                flag2 = this._items.Remove(item);
            }
            return flag2;
        }

        [NullableContext(2)]
        void IList.Remove(object value)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                if (value is T)
                {
                    T t = (T)((object)value);
                    this._items.Remove(t);
                }
            }
        }

        public void RemoveAt(int index)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.RemoveAt(index);
            }
        }

        public void Clear()
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.Clear();
            }
        }

        public bool Contains(T item)
        {
            object syncRoot = this.SyncRoot;
            bool flag2;
            lock (syncRoot)
            {
                flag2 = this._items.Contains(item);
            }
            return flag2;
        }

        [NullableContext(2)]
        bool IList.Contains(object value)
        {
            object syncRoot = this.SyncRoot;
            bool flag3;
            lock (syncRoot)
            {
                bool flag2;
                if (value is T)
                {
                    T t = (T)((object)value);
                    flag2 = this._items.Contains(t);
                }
                else
                {
                    flag2 = false;
                }
                flag3 = flag2;
            }
            return flag3;
        }

        public int IndexOf(T item)
        {
            object syncRoot = this.SyncRoot;
            int num;
            lock (syncRoot)
            {
                num = this._items.IndexOf(item);
            }
            return num;
        }

        [NullableContext(2)]
        int IList.IndexOf(object value)
        {
            object syncRoot = this.SyncRoot;
            int num2;
            lock (syncRoot)
            {
                int num;
                if (value is T)
                {
                    T t = (T)((object)value);
                    num = this._items.IndexOf(t);
                }
                else
                {
                    num = -1;
                }
                num2 = num;
            }
            return num2;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                this._items.CopyTo(array, arrayIndex);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            object syncRoot = this.SyncRoot;
            lock (syncRoot)
            {
                ((ICollection)this._items).CopyTo(array, index);
            }
        }

        private readonly List<T> _items = new List<T>();

        public readonly object SyncRoot = new object();
    }
}
