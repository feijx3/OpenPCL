using System;

namespace MeloongCore
{
    public enum RateLimitMode
    {
        ImmediateThenMerge,
        ImmediateThenIgnore,
        WaitUnillNoNewInvoke
    }
}
