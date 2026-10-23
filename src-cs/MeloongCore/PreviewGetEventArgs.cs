using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(2)]
    [Nullable(0)]
    public class PreviewGetEventArgs<T> : EventArgs
    {
        public PreviewGetEventArgs(T value)
        {
        }

        public T Value { get; set; } = value;
    }
}
