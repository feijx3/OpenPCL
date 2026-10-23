using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class DynamicConfigEntry<[Nullable(2)] T>
    {
        public DynamicConfigEntry(string key, [Nullable(2)] T defaultValue, bool encrypted = false)
        {
        }

        [Nullable(2)]
        [method: NullableContext(2)]
        [field: Nullable(2)]
        public event Action<T> Changed;

        [Nullable(new byte[] { 2, 1, 1, 1 })]
        [field: Nullable(new byte[] { 2, 1, 1, 1 })]
        public event Action<PreviewGetEventArgs<T>, IConfigProvider> PreviewGet;

        public void Edit([Nullable(new byte[] { 1, 2, 2 })] Func<T, T> editFunc, IConfigProvider provider)
        {
            this.Set(editFunc(this.Get(provider)), provider);
        }

        public void Edit([Nullable(new byte[] { 1, 0 })] DynamicConfigEntry<T>.RefAction editAction, IConfigProvider provider)
        {
            T t = this.Get(provider);
            editAction(ref t);
            this.Set(t, provider);
        }

        public void Set([Nullable(2)] T value, IConfigProvider provider)
        {
            provider.Set<T>(this.<key>P, value, this.<encrypted>P);
            Action<T> changed = this.Changed;
            if (changed == null)
            {
                return;
            }
            changed(value);
        }

        public void Reset(IConfigProvider provider)
        {
            if (!provider.HasValue(this.<key>P))
            {
                return;
            }
            provider.Remove(this.<key>P);
            Action<T> changed = this.Changed;
            if (changed == null)
            {
                return;
            }
            changed(this.<defaultValue>P);
        }

        [return: Nullable(2)]
        public T Get(IConfigProvider provider)
        {
            PreviewGetEventArgs<T> previewGetEventArgs = new PreviewGetEventArgs<T>(provider.Read<T>(this.<key>P, this.<defaultValue>P, this.<encrypted>P));
            Action<PreviewGetEventArgs<T>, IConfigProvider> previewGet = this.PreviewGet;
            if (previewGet != null)
            {
                previewGet(previewGetEventArgs, provider);
            }
            return previewGetEventArgs.Value;
        }

        public bool HasValue(IConfigProvider provider)
        {
            return provider.HasValue(this.<key>P);
        }

        [CompilerGenerated]
        private string <key>P = key;

        [Nullable(2)]
        [CompilerGenerated]
        private T <defaultValue>P = defaultValue;

        [CompilerGenerated]
        private bool <encrypted>P = encrypted;

        [NullableContext(0)]
        public delegate void RefAction(ref T value);
    }
}
