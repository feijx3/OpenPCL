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
    public class MyImage : Image
    {
        public MyImage()
        {
            base.Initialized += delegate(object a0, EventArgs a1)
            {
                this.Load();
            };
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
                return Conversions.ToBoolean(base.GetValue(MyImage.EnableCacheProperty));
            }
            set
            {
                base.SetValue(MyImage.EnableCacheProperty, value);
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
                        ImageSource Bitmap = ((value == null) ? null : new MyBitmap(value));
                        ModBase.RunInUi(delegate
                        {
                            this.Source = Bitmap;
                        }, false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("加载图片失败（{0}）", value), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyImage.vb");
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
            bool EnableCache = this.EnableCache;
            string TempPath = string.Format("{0}MyImage\\{1}.png{2}", ModBase.PathTemp, this.Source.GetStableHashCode(), RuntimeHelpers.GetObjectValue(EnableCache ? "" : ModBase.GetUuid()));
            FileInfo info = FileUtils.GetInfo(TempPath);
            if (EnableCache && info.Exists)
            {
                this.ActualSource = TempPath;
                if (DateTime.Now - info.LastWriteTime < this.FileCacheExpiredTime)
                {
                    return;
                }
            }
            else
            {
                this.ActualSource = this.LoadingSource;
            }
            ModBase.RunInNewThread(delegate
            {
                bool flag = !string.IsNullOrEmpty(this.FallbackSource) && !this.FallbackSource.StartsWithF("http", false);
                try
                {
                    string[] array;
                    if (this.FallbackSource == null || !this.FallbackSource.StartsWithF("http", false) || Operators.CompareString(this.Source, this.FallbackSource, false) == 0)
                    {
                        (array = new string[1])[0] = this.Source;
                    }
                    else
                    {
                        string[] array2 = new string[2];
                        array2[0] = this.Source;
                        array = array2;
                        array2[1] = this.FallbackSource;
                    }
                    ModNet.NetDownloadByLoader(array, TempPath, null, null, true);
                    this.ActualSource = TempPath;
                    if (!EnableCache)
                    {
                        FileUtils.Delete(TempPath, false);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (TempPath != null)
                        {
                            FileUtils.Delete(TempPath, false);
                        }
                    }
                    catch (Exception ex2)
                    {
                    }
                    if (flag)
                    {
                        try
                        {
                            this.ActualSource = this.FallbackSource;
                            Logger.Warn(ex, string.Format("下载图片失败，使用本地备用图片（{0}）", this.Source), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyImage.vb");
                            ProjectData.ClearProjectError();
                            return;
                        }
                        catch (Exception ex3)
                        {
                            Logger.Error(ex, string.Format("下载图片失败（{0}）", this.Source), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyImage.vb");
                            Logger.Error(ex3, string.Format("加载备用图片失败（{0}）", this.FallbackSource), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyImage.vb");
                            goto IL_01D0;
                        }
                    }
                    Logger.Error(ex, string.Format("下载图片失败（{0}，备用：{1}）", this.Source, this.FallbackSource), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyImage.vb");
                    IL_01D0:;
                }
            }, "MyImage PicLoader " + Conversions.ToString(ModBase.GetUuid()) + "#", ThreadPriority.BelowNormal);
        }

        public TimeSpan FileCacheExpiredTime;

        public static readonly DependencyProperty EnableCacheProperty = DependencyProperty.Register("EnableCache", typeof(bool), typeof(MyImage), new PropertyMetadata(true));

        private string _Source;

        public new static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(MyImage), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                ((MyImage)sender).Source = e.NewValue.ToString();
            }
        }));

        private string _FallbackSource;

        private string _LoadingSource;

        private string _ActualSource;
    }
}
