using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurImage : Image
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurImage()
        {
        }

        public OurImage()
        {
            base.Initialized += this._Lambda___R1_1;
            this.FileCacheExpiredTime = new TimeSpan(7, 0, 0, 0);
            this._Source = "";
            this._FallbackSource = null;
            this._LoadingSource = "pack://application:,,,/images/Icons/NoIcon.png";
            this._ActualSource = null;
        }

        public bool EnableCache
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(OurImage.EnableCacheProperty));
            }
            set
            {
                base.SetValue(OurImage.EnableCacheProperty, value);
            }
        }

        public new string Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                if (Operators.CompareString(value, "", false) == 0)
                {
                    value = null;
                }
                if (Operators.CompareString(this._Source, value, false) != 0)
                {
                    this._Source = value;
                    if (base.IsInitialized)
                    {
                        this.Load();
                    }
                }
            }
        }

        public string FallbackSource
        {
            get
            {
                return this._FallbackSource;
            }
            set
            {
                this._FallbackSource = value;
            }
        }

        public string LoadingSource
        {
            get
            {
                return this._LoadingSource;
            }
            set
            {
                this._LoadingSource = value;
            }
        }

        public string ActualSource
        {
            get
            {
                return this._ActualSource;
            }
            set
            {
                if (Operators.CompareString(value, "", false) == 0)
                {
                    value = null;
                }
                if (Operators.CompareString(this._ActualSource, value, false) != 0)
                {
                    this._ActualSource = value;
                    try
                    {
                        OurImage._Closure___22_0 CS_locals1 = new OurImage._Closure___22_0();
                        CS_locals1._VB_Me = this;
                        CS_locals1._VB_Local_Bitmap = ((value == null) ? null : new OurBitmap(value));
                        ModBase.RunInUi(new Action(CS_locals1._Lambda___0), false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("加载图片失败（{0}）", value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurImage.vb");
                        try
                        {
                            if (value.StartsWithF(ModBase.PathTemp, true))
                            {
                                FileUtils.Delete(value, false);
                            }
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                }
            }
        }

        private void Load()
        {
            OurImage._Closure___24_0 CS_locals1 = new OurImage._Closure___24_0();
            CS_locals1._VB_Me = this;
            if (this.Source == null)
            {
                this.ActualSource = null;
                return;
            }
            if (!this.Source.StartsWithF("http", false))
            {
                this.ActualSource = this.Source;
                return;
            }
            CS_locals1._VB_Local_EnableCache = this.EnableCache;
            CS_locals1._VB_Local_TempPath = string.Format("{0}OurImage\\{1}.png{2}", ModBase.PathTemp, this.Source.GetStableHashCode(), RuntimeHelpers.GetObjectValue(CS_locals1._VB_Local_EnableCache ? "" : ModBase.GetUuid()));
            FileInfo info = FileUtils.GetInfo(CS_locals1._VB_Local_TempPath);
            if (CS_locals1._VB_Local_EnableCache && info.Exists)
            {
                this.ActualSource = CS_locals1._VB_Local_TempPath;
                if (DateTime.Now - info.LastWriteTime < this.FileCacheExpiredTime)
                {
                    return;
                }
            }
            else
            {
                this.ActualSource = this.LoadingSource;
            }
            ModBase.RunInNewThread(new Action(CS_locals1._Lambda___0), "OurImage PicLoader " + Conversions.ToString(ModBase.GetUuid()) + "#", ThreadPriority.BelowNormal);
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, EventArgs a1)
        {
            this.Load();
        }

        public TimeSpan FileCacheExpiredTime;

        public static readonly DependencyProperty EnableCacheProperty = DependencyProperty.Register("EnableCache", typeof(bool), typeof(OurImage), new PropertyMetadata(true));

        private string _Source;

        public new static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(OurImage), new PropertyMetadata(new PropertyChangedCallback(OurImage._Closure___._I._Lambda___0_0)));

        private string _FallbackSource;

        private string _LoadingSource;

        private string _ActualSource;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal void _Lambda___0_0(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    ((OurImage)sender).Source = e.NewValue.ToString();
                }
            }

            public static readonly OurImage._Closure___ _I = new OurImage._Closure___();
        }

        [CompilerGenerated]
        private sealed class _Closure___22_0
        {
            public _Closure___22_0()
            {
            }

            internal void _Lambda___0()
            {
                ((System.Windows.Controls.Image)this._VB_Me).Source = this._VB_Local_Bitmap;
            }

            public ImageSource _VB_Local_Bitmap;

            public OurImage _VB_Me;
        }

        [CompilerGenerated]
        private sealed class _Closure___24_0
        {
            public _Closure___24_0()
            {
            }

            internal void _Lambda___0()
            {
                bool flag = !string.IsNullOrEmpty(this._VB_Me.FallbackSource) && !this._VB_Me.FallbackSource.StartsWithF("http", false);
                try
                {
                    string[] array;
                    if (this._VB_Me.FallbackSource == null || !this._VB_Me.FallbackSource.StartsWithF("http", false) || Operators.CompareString(this._VB_Me.Source, this._VB_Me.FallbackSource, false) == 0)
                    {
                        (array = new string[1])[0] = this._VB_Me.Source;
                    }
                    else
                    {
                        string[] array2 = new string[2];
                        array2[0] = this._VB_Me.Source;
                        array = array2;
                        array2[1] = this._VB_Me.FallbackSource;
                    }
                    ModNet.NetDownloadByLoader(array, this._VB_Local_TempPath, null, null, true);
                    this._VB_Me.ActualSource = this._VB_Local_TempPath;
                    if (!this._VB_Local_EnableCache)
                    {
                        FileUtils.Delete(this._VB_Local_TempPath, false);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (this._VB_Local_TempPath != null)
                        {
                            FileUtils.Delete(this._VB_Local_TempPath, false);
                        }
                    }
                    catch (Exception ex2)
                    {
                    }
                    if (flag)
                    {
                        try
                        {
                            this._VB_Me.ActualSource = this._VB_Me.FallbackSource;
                            Logger.Warn(ex, string.Format("下载图片失败，使用本地备用图片（{0}）", this._VB_Me.Source), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurImage.vb");
                            ProjectData.ClearProjectError();
                            return;
                        }
                        catch (Exception ex3)
                        {
                            Logger.Error(ex, string.Format("下载图片失败（{0}）", this._VB_Me.Source), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurImage.vb");
                            Logger.Error(ex3, string.Format("加载备用图片失败（{0}）", this._VB_Me.FallbackSource), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurImage.vb");
                            goto IL_01D0;
                        }
                    }
                    Logger.Error(ex, string.Format("下载图片失败（{0}，备用：{1}）", this._VB_Me.Source, this._VB_Me.FallbackSource), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurImage.vb");
                    IL_01D0:;
                }
            }

            public string _VB_Local_TempPath;

            public bool _VB_Local_EnableCache;

            public OurImage _VB_Me;
        }
    }
}
