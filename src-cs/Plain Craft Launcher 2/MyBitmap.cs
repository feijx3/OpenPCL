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
using PCL.My;

namespace PCL
{
    public class MyBitmap
    {
        public static implicit operator MyBitmap(Image Image)
        {
            MyBitmap myBitmap;
            if (Image == null)
            {
                myBitmap = null;
            }
            else
            {
                myBitmap = new MyBitmap(Image);
            }
            return myBitmap;
        }

        public static implicit operator Image(MyBitmap Image)
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

        public static implicit operator MyBitmap(ImageSource Image)
        {
            MyBitmap myBitmap;
            if (Image == null)
            {
                myBitmap = null;
            }
            else
            {
                myBitmap = new MyBitmap(Image);
            }
            return myBitmap;
        }

        public static implicit operator ImageSource(MyBitmap Image)
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

        public static implicit operator MyBitmap(Bitmap Image)
        {
            MyBitmap myBitmap;
            if (Image == null)
            {
                myBitmap = null;
            }
            else
            {
                myBitmap = new MyBitmap(Image);
            }
            return myBitmap;
        }

        public static implicit operator Bitmap(MyBitmap Image)
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

        public static implicit operator MyBitmap(ImageBrush Image)
        {
            MyBitmap myBitmap;
            if (Image == null)
            {
                myBitmap = null;
            }
            else
            {
                myBitmap = new MyBitmap(Image);
            }
            return myBitmap;
        }

        public static implicit operator ImageBrush(MyBitmap Image)
        {
            ImageBrush imageBrush;
            if (Image == null)
            {
                imageBrush = null;
            }
            else
            {
                imageBrush = new ImageBrush(new MyBitmap(Image.Pic));
            }
            return imageBrush;
        }

        public MyBitmap()
        {
        }

        public MyBitmap(string FilePathOrResourceName)
        {
            try
            {
                FilePathOrResourceName = FilePathOrResourceName.Replace("pack://application:,,,/images/", ModBase.PathImage);
                if (FilePathOrResourceName.StartsWithF(ModBase.PathImage, false))
                {
                    if (this.$STATIC$.ctor$2011E$Cache$Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$.ctor$2011E$Cache$Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag = false;
                    try
                    {
                        Monitor.Enter(this.$STATIC$.ctor$2011E$Cache$Init, ref flag);
                        if (this.$STATIC$.ctor$2011E$Cache$Init.State == 0)
                        {
                            this.$STATIC$.ctor$2011E$Cache$Init.State = 2;
                            this.$STATIC$.ctor$2011E$Cache = new ConcurrentDictionary<string, MyBitmap>();
                        }
                        else if (this.$STATIC$.ctor$2011E$Cache$Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this.$STATIC$.ctor$2011E$Cache$Init.State = 1;
                        if (flag)
                        {
                            Monitor.Exit(this.$STATIC$.ctor$2011E$Cache$Init);
                        }
                    }
                    if (this.$STATIC$.ctor$2011E$Cache.ContainsKey(FilePathOrResourceName))
                    {
                        this.Pic = this.$STATIC$.ctor$2011E$Cache[FilePathOrResourceName].Pic;
                    }
                    else
                    {
                        this.Pic = new MyBitmap((ImageSource)new ImageSourceConverter().ConvertFromString(FilePathOrResourceName));
                        this.$STATIC$.ctor$2011E$Cache.TryAdd(FilePathOrResourceName, this.Pic);
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
                            this.Pic = MyBitmap.WebPDecoder.DecodeFromBytes(array2);
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
                this.Pic = (Bitmap)MyWpfExtension.Application.TryFindResource(FilePathOrResourceName);
                if (this.Pic == null)
                {
                    this.Pic = new Bitmap(1, 1);
                    if (ex is ArgumentException)
                    {
                        throw new Exception(string.Format("图片格式不支持，或图片文件损坏（{0}）", FilePathOrResourceName), ex);
                    }
                    throw new Exception(string.Format("加载 MyBitmap 意外失败（{0}）", FilePathOrResourceName), ex);
                }
                else
                {
                    Logger.Warn(ex, string.Format("指定类型有误的 MyBitmap 加载（{0}）", FilePathOrResourceName), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\MyBitmap.vb");
                }
            }
        }

        public MyBitmap(ImageSource Image)
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

        public MyBitmap(Image Image)
        {
            this.Pic = (Bitmap)Image;
        }

        public MyBitmap(Bitmap Image)
        {
            this.Pic = Image;
        }

        public MyBitmap(ImageBrush Image)
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

        public MyBitmap Clip(int X, int Y, int Width, int Height)
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

        public MyBitmap RotateFlip(RotateFlipType Type)
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

        private ConcurrentDictionary<string, MyBitmap> $STATIC$.ctor$2011E$Cache;

        private StaticLocalInitFlag $STATIC$.ctor$2011E$Cache$Init;

        private class WebPDecoder
        {
            public static Bitmap DecodeFromBytes(byte[] Bytes)
            {
                return new SimpleDecoder().DecodeFromBytes(Bytes, (long)Bytes.Length);
            }
        }
    }
}
