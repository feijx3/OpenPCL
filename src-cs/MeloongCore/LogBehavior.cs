using System;

namespace MeloongCore
{
    public enum LogBehavior
    {
        None,
        ToastIfDebug,
        Toast,
        Alert,
        AlertThenFeedback,
        AlertThenCrash
    }
}
