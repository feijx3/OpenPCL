using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MeloongCore
{
    [NullableContext(2)]
    public interface IWorker
    {
        void Start(CancellationToken cancellationToken = default(CancellationToken), ProgressProvider progressOverride = null);

        void Cancel();

        bool WaitIfRunning(int millisecondsTimeout = -1, CancellationToken cancellationToken = default(CancellationToken));

        [NullableContext(1)]
        Task<bool> WaitIfRunningAsync(int millisecondsTimeout = -1, CancellationToken cancellationToken = default(CancellationToken));

        event Action Started;

        event Action Stopped;

        event Action Succeeded;

        [Nullable(new byte[] { 2, 1 })]
        event Action<Exception> Failed;

        event Action Canceled;

        bool Running { get; }

        bool HasSucceeded { get; }

        bool LastSucceeded { get; }

        [Nullable(1)]
        ProgressProvider Progress
        {
            [NullableContext(1)]
            get;
        }
    }
}
