using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PCL
{
    public class OurLoadingStateSimulator : ILoadingTrigger
    {
        public OurLoadingStateSimulator()
        {
            this._LoadingState = OurLoading.OurLoadingState.Unloaded;
            this._IsLoader = false;
        }

        private OurLoading.OurLoadingState _LoadingState
        {
            [CompilerGenerated]
            get
            {
                return this.__LoadingState;
            }
            [CompilerGenerated]
            set
            {
                this.__LoadingState = value;
            }
        }

        public OurLoading.OurLoadingState LoadingState
        {
            get
            {
                return this._LoadingState;
            }
            set
            {
                if (this._LoadingState != value)
                {
                    OurLoading.OurLoadingState loadingState = this._LoadingState;
                    this._LoadingState = value;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEvent = this.LoadingStateChangedEvent;
                    if (loadingStateChangedEvent != null)
                    {
                        loadingStateChangedEvent(value, loadingState);
                    }
                }
            }
        }

        public bool IsLoader
        {
            [CompilerGenerated]
            get
            {
                return this._IsLoader;
            }
        }

        public event ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChanged
        {
            [CompilerGenerated]
            add
            {
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = this.LoadingStateChangedEvent;
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler2;
                do
                {
                    loadingStateChangedEventHandler2 = loadingStateChangedEventHandler;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler3 = (ILoadingTrigger.LoadingStateChangedEventHandler)Delegate.Combine(loadingStateChangedEventHandler2, value);
                    loadingStateChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.LoadingStateChangedEventHandler>(ref this.LoadingStateChangedEvent, loadingStateChangedEventHandler3, loadingStateChangedEventHandler2);
                }
                while (loadingStateChangedEventHandler != loadingStateChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler = this.LoadingStateChangedEvent;
                ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler2;
                do
                {
                    loadingStateChangedEventHandler2 = loadingStateChangedEventHandler;
                    ILoadingTrigger.LoadingStateChangedEventHandler loadingStateChangedEventHandler3 = (ILoadingTrigger.LoadingStateChangedEventHandler)Delegate.Remove(loadingStateChangedEventHandler2, value);
                    loadingStateChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.LoadingStateChangedEventHandler>(ref this.LoadingStateChangedEvent, loadingStateChangedEventHandler3, loadingStateChangedEventHandler2);
                }
                while (loadingStateChangedEventHandler != loadingStateChangedEventHandler2);
            }
        }

        public event ILoadingTrigger.ProgressChangedEventHandler ProgressChanged
        {
            [CompilerGenerated]
            add
            {
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = this.ProgressChangedEvent;
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler2;
                do
                {
                    progressChangedEventHandler2 = progressChangedEventHandler;
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler3 = (ILoadingTrigger.ProgressChangedEventHandler)Delegate.Combine(progressChangedEventHandler2, value);
                    progressChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.ProgressChangedEventHandler>(ref this.ProgressChangedEvent, progressChangedEventHandler3, progressChangedEventHandler2);
                }
                while (progressChangedEventHandler != progressChangedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler = this.ProgressChangedEvent;
                ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler2;
                do
                {
                    progressChangedEventHandler2 = progressChangedEventHandler;
                    ILoadingTrigger.ProgressChangedEventHandler progressChangedEventHandler3 = (ILoadingTrigger.ProgressChangedEventHandler)Delegate.Remove(progressChangedEventHandler2, value);
                    progressChangedEventHandler = Interlocked.CompareExchange<ILoadingTrigger.ProgressChangedEventHandler>(ref this.ProgressChangedEvent, progressChangedEventHandler3, progressChangedEventHandler2);
                }
                while (progressChangedEventHandler != progressChangedEventHandler2);
            }
        }

        [CompilerGenerated]
        private OurLoading.OurLoadingState __LoadingState;

        [CompilerGenerated]
        private bool _IsLoader;

        [CompilerGenerated]
        private ILoadingTrigger.LoadingStateChangedEventHandler LoadingStateChangedEvent;

        [CompilerGenerated]
        private ILoadingTrigger.ProgressChangedEventHandler ProgressChangedEvent;
    }
}
