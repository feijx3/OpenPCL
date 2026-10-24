Imports System
Imports System.Collections.Concurrent
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Threading
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Imazen.WebP
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices
Imports PCL.My

Namespace PCL
    Public Class OurBitmap
        Public Shared Widening Operator CType(Image As Image) As OurBitmap
            Dim myBitmap As OurBitmap
            If Image Is Nothing Then
                myBitmap = Nothing
            Else
                myBitmap = New OurBitmap(Image)
            End If
            Return myBitmap
        End Operator

        Public Shared Widening Operator CType(Image As OurBitmap) As Image
            Dim image As Image
            If Image Is Nothing Then
                image = Nothing
            Else
                image = Image.Pic
            End If
            Return image
        End Operator

        Public Shared Widening Operator CType(Image As ImageSource) As OurBitmap
            Dim myBitmap As OurBitmap
            If Image Is Nothing Then
                myBitmap = Nothing
            Else
                myBitmap = New OurBitmap(Image)
            End If
            Return myBitmap
        End Operator

        Public Shared Widening Operator CType(Image As OurBitmap) As ImageSource
            Dim imageSource As ImageSource
            If Image Is Nothing Then
                imageSource = Nothing
            Else
                Dim pic As Bitmap = Image.Pic
                Dim rectangle As Rectangle = New Rectangle(0, 0, pic.Width, pic.Height)
                Dim bitmapData As BitmapData = pic.LockBits(rectangle, ImageLockMode.ReadWrite, Global.System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                Try
                    ' The following expression was wrapped in a checked-expression
                    Dim bitmapSource As BitmapSource = BitmapSource.Create(pic.Width, pic.Height, CDbl(pic.HorizontalResolution), CDbl(pic.VerticalResolution), PixelFormats.Bgra32, Nothing, bitmapData.Scan0, rectangle.Width * rectangle.Height * 4, bitmapData.Stride)
                    bitmapSource.Freeze()
                    imageSource = bitmapSource
                Finally
                    pic.UnlockBits(bitmapData)
                End Try
            End If
            Return imageSource
        End Operator

        Public Shared Widening Operator CType(Image As Bitmap) As OurBitmap
            Dim myBitmap As OurBitmap
            If Image Is Nothing Then
                myBitmap = Nothing
            Else
                myBitmap = New OurBitmap(Image)
            End If
            Return myBitmap
        End Operator

        Public Shared Widening Operator CType(Image As OurBitmap) As Bitmap
            Dim bitmap As Bitmap
            If Image Is Nothing Then
                bitmap = Nothing
            Else
                bitmap = Image.Pic
            End If
            Return bitmap
        End Operator

        Public Shared Widening Operator CType(Image As ImageBrush) As OurBitmap
            Dim myBitmap As OurBitmap
            If Image Is Nothing Then
                myBitmap = Nothing
            Else
                myBitmap = New OurBitmap(Image)
            End If
            Return myBitmap
        End Operator

        Public Shared Widening Operator CType(Image As OurBitmap) As ImageBrush
            Dim imageBrush As ImageBrush
            If Image Is Nothing Then
                imageBrush = Nothing
            Else
                imageBrush = New ImageBrush(New OurBitmap(Image.Pic))
            End If
            Return imageBrush
        End Operator

        Public Sub New()
        End Sub

        Public Sub New(FilePathOrResourceName As String)
            Try
                FilePathOrResourceName = FilePathOrResourceName.Replace("pack://application:,,,/images/", ModBase.PathImage)
                If FilePathOrResourceName.StartsWithF(ModBase.PathImage, False) Then
                    If Me.$STATIC$.ctor$2011E$Cache$Init Is Nothing Then
                        Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$.ctor$2011E$Cache$Init, New StaticLocalInitFlag(), Nothing)
                    End If
                    Dim flag As Boolean = False
                    Try
                        Monitor.Enter(Me.$STATIC$.ctor$2011E$Cache$Init, flag)
                        If Me.$STATIC$.ctor$2011E$Cache$Init.State = 0S Then
                            Me.$STATIC$.ctor$2011E$Cache$Init.State = 2S
                            Me.$STATIC$.ctor$2011E$Cache = New ConcurrentDictionary(Of String, OurBitmap)()
                        ElseIf Me.$STATIC$.ctor$2011E$Cache$Init.State = 2S Then
                            Throw New IncompleteInitialization()
                        End If
                    Finally
                        Me.$STATIC$.ctor$2011E$Cache$Init.State = 1S
                        If flag Then
                            Monitor.[Exit](Me.$STATIC$.ctor$2011E$Cache$Init)
                        End If
                    End Try
                    If Me.$STATIC$.ctor$2011E$Cache.ContainsKey(FilePathOrResourceName) Then
                        Me.Pic = Me.$STATIC$.ctor$2011E$Cache(FilePathOrResourceName).Pic
                    Else
                        Me.Pic = New OurBitmap(CType(New ImageSourceConverter().ConvertFromString(FilePathOrResourceName), ImageSource))
                        Me.$STATIC$.ctor$2011E$Cache.TryAdd(FilePathOrResourceName, Me.Pic)
                    End If
                Else
                    Using stream As Stream = FileUtils.ReadAsStream(FilePathOrResourceName, Nothing)
                        Dim array As Byte() = New Byte(1) {}
                        stream.Read(array, 0, 2)
                        stream.Seek(0L, SeekOrigin.Begin)
                        If array(0) = 82 AndAlso array(1) = 73 Then
                            ' The following expression was wrapped in a checked-expression
                            Dim array2 As Byte() = New Byte(CInt((stream.Length - 1L)) + 1 - 1) {}
                            stream.Read(array2, 0, array2.Length)
                            Me.Pic = OurBitmap.WebPDecoder.DecodeFromBytes(array2)
                        Else
                            Me.Pic = New Bitmap(stream)
                        End If
                    End Using
                End If
            Catch ex As Exception
                Me.Pic = CType(MyWpfExtension.Application.TryFindResource(FilePathOrResourceName), Bitmap)
                If Me.Pic Is Nothing Then
                    Me.Pic = New Bitmap(1, 1)
                    If TypeOf ex Is ArgumentException Then
                        Throw New Exception(String.Format("图片格式不支持，或图片文件损坏（{0}）", FilePathOrResourceName), ex)
                    End If
                    Throw New Exception(String.Format("加载 OurBitmap 意外失败（{0}）", FilePathOrResourceName), ex)
                Else
                    Logger.Warn(ex, String.Format("指定类型有误的 OurBitmap 加载（{0}）", FilePathOrResourceName), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\OpenPCL\Modules\Base\OurBitmap.vb")
                End If
            End Try
        End Sub

        Public Sub New(Image As ImageSource)
            Using memoryStream As MemoryStream = New MemoryStream()
                New PngBitmapEncoder() With { .Frames = { BitmapFrame.Create(CType(Image, BitmapSource)) } }.Save(memoryStream)
                Me.Pic = New Bitmap(memoryStream)
            End Using
        End Sub

        Public Sub New(Image As Image)
            Me.Pic = CType(Image, Bitmap)
        End Sub

        Public Sub New(Image As Bitmap)
            Me.Pic = Image
        End Sub

        Public Sub New(Image As ImageBrush)
            Using memoryStream As MemoryStream = New MemoryStream()
                New BmpBitmapEncoder() With { .Frames = { BitmapFrame.Create(CType(Image.ImageSource, BitmapSource)) } }.Save(memoryStream)
                Me.Pic = New Bitmap(memoryStream)
            End Using
        End Sub

        Public Function Clip(X As Integer, Y As Integer, Width As Integer, Height As Integer) As OurBitmap
            Dim bitmap As Bitmap = New Bitmap(Width, Height, Me.Pic.PixelFormat)
            bitmap.SetResolution(Me.Pic.HorizontalResolution, Me.Pic.VerticalResolution)
            Using graphics As Graphics = Graphics.FromImage(bitmap)
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor
                graphics.TranslateTransform(CSng((0 - X)), CSng((0 - Y)))
                graphics.DrawImage(Me.Pic, New Rectangle(0, 0, Me.Pic.Width, Me.Pic.Height))
            End Using
            Return bitmap
        End Function

        Public Function RotateFlip(Type As RotateFlipType) As OurBitmap
            Dim bitmap As Bitmap = New Bitmap(Me.Pic)
            bitmap.SetResolution(Me.Pic.HorizontalResolution, Me.Pic.VerticalResolution)
            bitmap.RotateFlip(Type)
            Return bitmap
        End Function

        Public Sub Save(FilePath As String)
            Dim bitmapEncoder As BitmapEncoder = New PngBitmapEncoder()
            bitmapEncoder.Frames.Add(BitmapFrame.Create(CType(Me, BitmapSource)))
            Using fileStream As FileStream = FileUtils.CreateAsStream(FilePath)
                bitmapEncoder.Save(fileStream)
            End Using
        End Sub

        Public Pic As Bitmap

        Private $STATIC$.ctor$2011E$Cache As ConcurrentDictionary(Of String, OurBitmap)

        Private $STATIC$.ctor$2011E$Cache$Init As StaticLocalInitFlag

        Private Class WebPDecoder
            Public Shared Function DecodeFromBytes(Bytes As Byte()) As Bitmap
                Return New SimpleDecoder().DecodeFromBytes(Bytes, CLng(Bytes.Length))
            End Function
        End Class
    End Class
End Namespace
