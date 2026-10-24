using System;

namespace PCL
{
    public interface ILoadingTrigger
    {
        bool IsLoader { get; }

        OurLoading.OurLoadingState LoadingState { get; set; }

        event ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChanged;

        event ILoadingTrigger.ProgressChangedEventHandler ProgressChanged;

        public delegate void LoadingStateChangedEventHandler(OurLoading.OurLoadingState NewState, OurLoading.OurLoadingState OldState);

        public delegate void ProgressChangedEventHandler(double NewProgress, double OldProgress);
    }
}
