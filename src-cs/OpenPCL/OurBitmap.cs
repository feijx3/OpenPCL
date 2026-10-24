using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Imazen.WebP;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using PCL.Our;

namespace PCL
{
    public class OurBitmap
    {
        public static implicit operator OurBitmap(Image Image)
        {
            OurBitmap ourBitmap;
            if (Image == null)
            {
                ourBitmap = null;
            }
            else
            {
                ourBitmap = new OurBitmap(Image);
            }
            return ourBitmap;
        }

        public static implicit operator Image(OurBitmap Image)
        {
            Image image;
            if (Image == null)
            {
                image = null;
            }
            else
            {
                image = Image.Pic;
            }
            return image;
        }

        public static implicit operator OurBitmap(ImageSource Image)
        {
            OurBitmap ourBitmap;
            if (Image == null)
            {
                ourBitmap = null;
            }
            else
            {
                ourBitmap = new OurBitmap(Image);
            }
            return ourBitmap;
        }

        public static implicit operator ImageSource(OurBitmap Image)
        {
            ImageSource imageSource;
            if (Image == null)
            {
                imageSource = null;
            }
            else
            {
                Bitmap pic = Image.Pic;
                Rectangle rectangle = new Rectangle(0, 0, pic.Width, pic.Height);
                BitmapData bitmapData = pic.LockBits(rectangle, ImageLockMode.ReadWrite, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                try
                {
                    BitmapSource bitmapSource = BitmapSource.Create(pic.Width, pic.Height, (double)pic.HorizontalResolution, (double)pic.VerticalResolution, PixelFormats.Bgra32, null, bitmapData.Scan0, checked(rectangle.Width * rectangle.Height * 4), bitmapData.Stride);
                    bitmapSource.Freeze();
                    imageSource = bitmapSource;
                }
                finally
                {
                    pic.UnlockBits(bitmapData);
                }
            }
            return imageSource;
        }

        public static implicit operator OurBitmap(Bitmap Image)
        {
            OurBitmap ourBitmap;
            if (Image == null)
            {
                ourBitmap = null;
            }
            else
            {
                ourBitmap = new OurBitmap(Image);
            }
            return ourBitmap;
        }

        public static implicit operator Bitmap(OurBitmap Image)
        {
            Bitmap bitmap;
            if (Image == null)
            {
                bitmap = null;
            }
            else
            {
                bitmap = Image.Pic;
            }
            return bitmap;
        }

        public static implicit operator OurBitmap(ImageBrush Image)
        {
            OurBitmap ourBitmap;
            if (Image == null)
            {
                ourBitmap = null;
            }
            else
            {
                ourBitmap = new OurBitmap(Image);
            }
            return ourBitmap;
        }

        public static implicit operator ImageBrush(OurBitmap Image)
        {
            ImageBrush imageBrush;
            if (Image == null)
            {
                imageBrush = null;
            }
            else
            {
                imageBrush = new ImageBrush(new OurBitmap(Image.Pic));
            }
            return imageBrush;
        }

        public OurBitmap()
        {
        }

        public OurBitmap(string FilePathOrResourceName)
        {
            try
            {
                FilePathOrResourceName = FilePathOrResourceName.Replace("pack://application:,,,/images/", ModBase.PathImage);
                if (FilePathOrResourceName.StartsWithF(ModBase.PathImage, false))
                {
                    if (this._STATIC_ctor_2011E_Cache_Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this._STATIC_ctor_2011E_Cache_Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag = false;
                    try
                    {
                        Monitor.Enter(this._STATIC_ctor_2011E_Cache_Init, ref flag);
                        if (this._STATIC_ctor_2011E_Cache_Init.State == 0)
                        {
                            this._STATIC_ctor_2011E_Cache_Init.State = 2;
                            this._STATIC_ctor_2011E_Cache = new ConcurrentDictionary<string, OurBitmap>();
                        }
                        else if (this._STATIC_ctor_2011E_Cache_Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this._STATIC_ctor_2011E_Cache_Init.State = 1;
                        if (flag)
                        {
                            Monitor.Exit(this._STATIC_ctor_2011E_Cache_Init);
                        }
                    }
                    if (this._STATIC_ctor_2011E_Cache.ContainsKey(FilePathOrResourceName))
                    {
                        this.Pic = this._STATIC_ctor_2011E_Cache[FilePathOrResourceName].Pic;
                    }
                    else
                    {
                        this.Pic = new OurBitmap((ImageSource)new ImageSourceConverter().ConvertFromString(FilePathOrResourceName));
                        this._STATIC_ctor_2011E_Cache.TryAdd(FilePathOrResourceName, this.Pic);
                    }
                }
                else
                {
                    using (Stream stream = FileUtils.ReadAsStream(FilePathOrResourceName, null))
                    {
                        byte[] array = new byte[2];
                        stream.Read(array, 0, 2);
                        stream.Seek(0L, SeekOrigin.Begin);
                        if (array[0] == 82 && array[1] == 73)
                        {
                            byte[] array2 = new byte[checked((int)(stream.Length - 1L) + 1)];
                            stream.Read(array2, 0, array2.Length);
                            this.Pic = OurBitmap.WebPDecoder.DecodeFromBytes(array2);
                        }
                        else
                        {
                            this.Pic = new Bitmap(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Pic = (Bitmap)OurWpfExtension.Application.TryFindResource(FilePathOrResourceName);
                if (this.Pic == null)
                {
                    this.Pic = new Bitmap(1, 1);
                    if (ex is ArgumentException)
                    {
                        throw new Exception(string.Format("图片格式不支持，或图片文件损坏（{0}）", FilePathOrResourceName), ex);
                    }
                    throw new Exception(string.Format("加载 OurBitmap 意外失败（{0}）", FilePathOrResourceName), ex);
                }
                else
                {
                    Logger.Warn(ex, string.Format("指定类型有误的 OurBitmap 加载（{0}）", FilePathOrResourceName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\OurBitmap.vb");
                }
            }
        }

        public OurBitmap(ImageSource Image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new PngBitmapEncoder
                {
                    Frames = { BitmapFrame.Create((BitmapSource)Image) }
                }.Save(memoryStream);
                this.Pic = new Bitmap(memoryStream);
            }
        }

        public OurBitmap(Image Image)
        {
            this.Pic = (Bitmap)Image;
        }

        public OurBitmap(Bitmap Image)
        {
            this.Pic = Image;
        }

        public OurBitmap(ImageBrush Image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                new BmpBitmapEncoder
                {
                    Frames = { BitmapFrame.Create((BitmapSource)Image.ImageSource) }
                }.Save(memoryStream);
                this.Pic = new Bitmap(memoryStream);
            }
        }

        public OurBitmap Clip(int X, int Y, int Width, int Height)
        {
            Bitmap bitmap = new Bitmap(Width, Height, this.Pic.PixelFormat);
            bitmap.SetResolution(this.Pic.HorizontalResolution, this.Pic.VerticalResolution);
            checked
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.TranslateTransform((float)(0 - X), (float)(0 - Y));
                    graphics.DrawImage(this.Pic, new Rectangle(0, 0, this.Pic.Width, this.Pic.Height));
                }
                return bitmap;
            }
        }

        public OurBitmap RotateFlip(RotateFlipType Type)
        {
            Bitmap bitmap = new Bitmap(this.Pic);
            bitmap.SetResolution(this.Pic.HorizontalResolution, this.Pic.VerticalResolution);
            bitmap.RotateFlip(Type);
            return bitmap;
        }

        public void Save(string FilePath)
        {
            BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)this));
            using (FileStream fileStream = FileUtils.CreateAsStream(FilePath))
            {
                bitmapEncoder.Save(fileStream);
            }
        }

        public Bitmap Pic;

        private ConcurrentDictionary<string, OurBitmap> _STATIC_ctor_2011E_Cache;

        private StaticLocalInitFlag _STATIC_ctor_2011E_Cache_Init;

        private class WebPDecoder
        {
            public WebPDecoder()
            {
            }

            public static Bitmap DecodeFromBytes(byte[] Bytes)
            {
                return new SimpleDecoder().DecodeFromBytes(Bytes, (long)Bytes.Length);
            }
        }
    }
}
