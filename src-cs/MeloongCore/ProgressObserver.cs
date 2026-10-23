using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class ProgressObserver : RateLimitedWorker
    {
        public ProgressObserver(ProgressProvider provider, Action<double> updateAction, double minimalIntervalMs = 70.0)
            : base(delegate
            {
                ProgressObserver.Observe(provider, updateAction);
            }, minimalIntervalMs, RateLimitMode.ImmediateThenMerge)
        {
            this.provider = provider;
            provider.ProgressChanged += this.StartObserver;
        }

        public override void Dispose()
        {
            this.provider.ProgressChanged -= this.StartObserver;
            base.Dispose();
        }

        [NullableContext(0)]
        private void StartObserver([TupleElementNames(new string[] { "actual", "skiped" })] ValueTuple<double, double> raw)
        {
            base.Start();
        }

        private static void Observe(ProgressProvider provider, Action<double> updateAction)
        {
            double increasing = provider.GetIncreasing();
            updateAction(increasing);
        }

        private readonly ProgressProvider provider;
    }
}
