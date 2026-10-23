using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(2)]
    [Nullable(0)]
    public class ConfigEntry<T>
    {
        public ConfigEntry([Nullable(1)] string key, T defaultValue, bool encrypted = false, IConfigProvider provider = null)
        {
        }

        [Nullable(1)]
        public IConfigProvider Provider
        {
            [NullableContext(1)]
            get;
        } = provider ?? ConfigProviders.Default;

        public event Action<T> Changed;

        [Nullable(new byte[] { 2, 1, 1, 1 })]
        [field: Nullable(new byte[] { 2, 1, 1, 1 })]
        public event Action<PreviewGetEventArgs<T>, IConfigProvider> PreviewGet;

        public void Edit([Nullable(new byte[] { 1, 2, 2 })] Func<T, T> editFunc)
        {
            this.Set(editFunc(this.Get()));
        }

        public void Edit([Nullable(new byte[] { 1, 0 })] ConfigEntry<T>.RefAction editAction)
        {
            T t = this.Get();
            editAction(ref t);
            this.Set(t);
        }

        public void Set(T value)
        {
            this.Provider.Set<T>(this.<key>P, value, this.<encrypted>P);
            Action<T> changed = this.Changed;
            if (changed == null)
            {
                return;
            }
            changed(value);
        }

        public void Reset()
        {
            if (!this.Provider.HasValue(this.<key>P))
            {
                return;
            }
            this.Provider.Remove(this.<key>P);
            Action<T> changed = this.Changed;
            if (changed == null)
            {
                return;
            }
            changed(this.<defaultValue>P);
        }

        public T Get()
        {
            PreviewGetEventArgs<T> previewGetEventArgs = new PreviewGetEventArgs<T>(this.Provider.Read<T>(this.<key>P, this.<defaultValue>P, this.<encrypted>P));
            Action<PreviewGetEventArgs<T>, IConfigProvider> previewGet = this.PreviewGet;
            if (previewGet != null)
            {
                previewGet(previewGetEventArgs, this.Provider);
            }
            return previewGetEventArgs.Value;
        }

        public bool HasValue()
        {
            return this.Provider.HasValue(this.<key>P);
        }

        [Nullable(1)]
        [CompilerGenerated]
        private string <key>P = key;

        [CompilerGenerated]
        private T <defaultValue>P = defaultValue;

        [CompilerGenerated]
        private bool <encrypted>P = encrypted;

        [NullableContext(0)]
        public delegate void RefAction(ref T value);
    }
}
