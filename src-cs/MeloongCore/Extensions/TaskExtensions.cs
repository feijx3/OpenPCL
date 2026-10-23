using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MeloongCore.Extensions
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class TaskExtensions
    {
        public static void Run(this Task task)
        {
            task.GetAwaiter().GetResult();
        }

        public static T Run<[Nullable(2)] T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }

        public static ConfiguredTaskAwaitable NoCapture(this Task task)
        {
            return task.ConfigureAwait(false);
        }

        [return: Nullable(new byte[] { 0, 1 })]
        public static ConfiguredTaskAwaitable<T> NoCapture<[Nullable(2)] T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
