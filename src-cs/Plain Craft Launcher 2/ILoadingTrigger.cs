using System;

namespace PCL
{
    public interface ILoadingTrigger
    {
        bool IsLoader { get; }

        MyLoading.MyLoadingState LoadingState { get; set; }

        event ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChanged;

        event ILoadingTrigger.ProgressChangedEventHandler ProgressChanged;

        public delegate void LoadingStateChangedEventHandler(MyLoading.MyLoadingState NewState, MyLoading.MyLoadingState OldState);

        public delegate void ProgressChangedEventHandler(double NewProgress, double OldProgress);
    }
}
