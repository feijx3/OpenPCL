using System;

namespace PCL
{
    public class MyLoadingStateSimulator : ILoadingTrigger
    {
        public MyLoadingStateSimulator()
        {
            this._LoadingState = MyLoading.MyLoadingState.Unloaded;
            this.IsLoader = false;
        }

        private MyLoading.MyLoadingState _LoadingState { get; set; }

        public MyLoading.MyLoadingState LoadingState
        {
            get
            {
                return this._LoadingState;
            }
            set
            {
                if (this._LoadingState != value)
                {
                    MyLoading.MyLoadingState loadingState = this._LoadingState;
                    this._LoadingState = value;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEvent = this.LoadingStateChangedEvent;
                    if (loadingStateChangedEvent != null)
                    {
                        loadingStateChangedEvent(value, loadingState);
                    }
                }
            }
        }

        public bool IsLoader { get; }

        public event ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChanged;

        public event ILoadingTrigger.ProgressChangedEventHandler ProgressChanged;
    }
}
