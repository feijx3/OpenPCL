using System;

namespace PCL
{
    public interface IMyRadio
    {
        event IMyRadio.CheckEventHandler Check;

        event IMyRadio.ChangedEventHandler Changed;

        public delegate void CheckEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void ChangedEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
