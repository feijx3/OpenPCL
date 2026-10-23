using System;
using System.Runtime.CompilerServices;
using ThrottleDebounce;

namespace MeloongCore
{
    public class RateLimitedWorker : IDisposable
    {
        [NullableContext(1)]
        public RateLimitedWorker(Action workload, double minimalIntervalMs, RateLimitMode mode)
        {
            RateLimitedAction rateLimitedAction;
            if (mode != RateLimitMode.ImmediateThenMerge)
            {
                if (mode != RateLimitMode.WaitUnillNoNewInvoke)
                {
                    rateLimitedAction = Debouncer.Debounce(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), true, false);
                }
                else
                {
                    rateLimitedAction = Debouncer.Debounce(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), false, true);
                }
            }
            else
            {
                rateLimitedAction = Throttler.Throttle(workload, TimeSpan.FromMilliseconds(minimalIntervalMs), true, true);
            }
            this.action = rateLimitedAction;
            base..ctor();
        }

        public void Start()
        {
            if (!this.disposed)
            {
                this.action.Invoke();
            }
        }

        public virtual void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;
            this.action.Dispose();
        }

        [Nullable(1)]
        private readonly RateLimitedAction action;

        private bool disposed;
    }
}
