using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    public interface IConfigProvider
    {
        [NullableContext(2)]
        void Set<T>([Nullable(1)] string key, T value, bool encrypted);

        void Remove(string key);

        [NullableContext(2)]
        T Read<T>([Nullable(1)] string key, T defaultValue, bool encrypted);

        bool HasValue(string key);

        void DiscardCache();
    }
}
