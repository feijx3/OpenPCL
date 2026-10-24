using System;

namespace PCL
{
    public interface IOurRadio
    {
        event IOurRadio.CheckEventHandler Check;

        event IOurRadio.ChangedEventHandler Changed;

        public delegate void CheckEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void ChangedEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}
