using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(new byte[] { 0, 2 })]
    public class RedoableWorker : RedoableWorkerBase<NoType>
    {
        public RedoableWorker([Nullable(new byte[] { 1, 2 })] Action<CancellationToken?, ProgressProvider> workload, [CallerMemberName] string creatorMemberName = "")
            : base(delegate(CancellationToken? c, ProgressProvider p)
            {
                workload(c, p);
                return null;
            }, null, creatorMemberName)
        {
        }

        public RedoableWorker(Action<CancellationToken?> workload, [CallerMemberName] string creatorMemberName = "")
            : base(delegate(CancellationToken? c, ProgressProvider _)
            {
                workload(c);
                return null;
            }, null, creatorMemberName)
        {
        }

        public RedoableWorker(Action workload, [CallerMemberName] string creatorMemberName = "")
            : base(delegate(CancellationToken? _, ProgressProvider _)
            {
                workload();
                return null;
            }, null, creatorMemberName)
        {
        }
    }
}
