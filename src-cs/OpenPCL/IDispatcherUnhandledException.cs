using System;
using System.Windows.Threading;

namespace PCL
{
    public interface IDispatcherUnhandledException
    {
        void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e);
    }
}
