using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(new byte[] { 0, 1 })]
    public class RedoableWorker<[Nullable(2)] TOut> : RedoableWorkerBase<TOut>
    {
        public RedoableWorker([Nullable(new byte[] { 1, 2, 1 })] Func<CancellationToken?, ProgressProvider, TOut> workload, [CallerMemberName] string creatorMemberName = "")
            : base(workload, null, creatorMemberName)
        {
        }

        public RedoableWorker(Func<CancellationToken?, TOut> workload, [CallerMemberName] string creatorMemberName = "")
            : base((CancellationToken? c, [Nullable(2)] ProgressProvider _) => workload(c), null, creatorMemberName)
        {
        }

        public RedoableWorker(Func<TOut> workload, [CallerMemberName] string creatorMemberName = "")
            : base((CancellationToken? _, [Nullable(2)] ProgressProvider _) => workload(), null, creatorMemberName)
        {
        }
    }
}
