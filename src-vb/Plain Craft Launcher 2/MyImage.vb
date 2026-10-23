Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyImage
        Inherits Image

        Public Sub New()
            AddHandler MyBase.Initialized, Sub(a0 As Object, a1 As EventArgs)
                Me.Load()
            End Sub
            Me.FileCacheExpiredTime = New TimeSpan(7, 0, 0, 0)
            Me._Source = ""
            Me._FallbackSource = Nothing
            Me._LoadingSource = "pack://application:,,,/images/Icons/NoIcon.png"
            Me._ActualSource = Nothing
        End Sub

        Public Property EnableCache As Boolean
            Get
                Return Conversions.ToBoolean(MyBase.GetValue(MyImage.EnableCacheProperty))
            End Get
            Set(value As Boolean)
                MyBase.SetValue(MyImage.EnableCacheProperty, value)
            End Set
        End Property

        Public Property Source As String
            Get
                Return Me._Source
            End Get
            Set(value As String)
                If Operators.CompareString(value, "", False) = 0 Then
                    value = Nothing
                End If
                If Operators.CompareString(Me._Source, value, False) <> 0 Then
                    Me._Source = value
                    If MyBase.IsInitialized Then
                        Me.Load()
                    End If
                End If
            End Set
        End Property

        Public Property FallbackSource As String
            Get
                Return Me._FallbackSource
            End Get
            Set(value As String)
                Me._FallbackSource = value
            End Set
        End Property

        Public Property LoadingSource As String
            Get
                Return Me._LoadingSource
            End Get
            Set(value As String)
                Me._LoadingSource = value
            End Set
        End Property

        Public Property ActualSource As String
            Get
                Return Me._ActualSource
            End Get
            Set(value As String)
                If Operators.CompareString(value, "", False) = 0 Then
                    value = Nothing
                End If
                If Operators.CompareString(Me._ActualSource, value, False) <> 0 Then
                    Me._ActualSource = value
                    Try
                        Dim Bitmap As ImageSource = If((value Is Nothing), Nothing, New MyBitmap(value))
                        ModBase.RunInUi(Sub()
                            Me.Source = Bitmap
                        End Sub, False)
                    Catch ex As Exception
                        Logger.Warn(ex, String.Format("加载图片失败（{0}）", value), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyImage.vb")
                        Try
                            If value.StartsWithF(ModBase.PathTemp, True) Then
                                FileUtils.Delete(value, False)
                            End If
                        Catch ex2 As Exception
                        End Try
                    End Try
                End If
            End Set
        End Property

        Private Sub Load()
            If Me.Source Is Nothing Then
                Me.ActualSource = Nothing
                Return
            End If
            If Not Me.Source.StartsWithF("http", False) Then
                Me.ActualSource = Me.Source
                Return
            End If
            Dim EnableCache As Boolean = Me.EnableCache
            Dim TempPath As String = String.Format("{0}MyImage\{1}.png{2}", ModBase.PathTemp, Me.Source.GetStableHashCode(), RuntimeHelpers.GetObjectValue(If(EnableCache, "", ModBase.GetUuid())))
            Dim info As FileInfo = FileUtils.GetInfo(TempPath)
            If EnableCache AndAlso info.Exists Then
                Me.ActualSource = TempPath
                If DateTime.Now - info.LastWriteTime < Me.FileCacheExpiredTime Then
                    Return
                End If
            Else
                Me.ActualSource = Me.LoadingSource
            End If
            ModBase.RunInNewThread(Sub()
                Dim flag As Boolean = Not String.IsNullOrEmpty(Me.FallbackSource) AndAlso Not Me.FallbackSource.StartsWithF("http", False)
                Try
                    Dim enumerable As IEnumerable(Of String)
                    If Me.FallbackSource Is Nothing OrElse Not Me.FallbackSource.StartsWithF("http", False) OrElse Operators.CompareString(Me.Source, Me.FallbackSource, False) = 0 Then
                        Dim array As String() = New String(0) {}
                        enumerable = array
                        array(0) = Me.Source
                    Else
                        Dim array2 As String() = New String(1) {}
                        array2(0) = Me.Source
                        enumerable = array2
                        array2(1) = Me.FallbackSource
                    End If
                    ModNet.NetDownloadByLoader(enumerable, TempPath, Nothing, Nothing, True)
                    Me.ActualSource = TempPath
                    If Not EnableCache Then
                        FileUtils.Delete(TempPath, False)
                    End If
                Catch ex As Exception
                    Try
                        If TempPath IsNot Nothing Then
                            FileUtils.Delete(TempPath, False)
                        End If
                    Catch ex2 As Exception
                    End Try
                    If flag Then
                        Try
                            Me.ActualSource = Me.FallbackSource
                            Logger.Warn(ex, String.Format("下载图片失败，使用本地备用图片（{0}）", Me.Source), LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyImage.vb")
                            ProjectData.ClearProjectError()
                            Return
                        Catch ex3 As Exception
                            Logger.[Error](ex, String.Format("下载图片失败（{0}）", Me.Source), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyImage.vb")
                            Logger.[Error](ex3, String.Format("加载备用图片失败（{0}）", Me.FallbackSource), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyImage.vb")
                            GoTo IL_01D0
                        End Try
                    End If
                    Logger.[Error](ex, String.Format("下载图片失败（{0}，备用：{1}）", Me.Source, Me.FallbackSource), LogBehavior.Toast, "E:\Projects\PCL\本体代码\Plain Craft Launcher 2\Controls\MyImage.vb")
                    IL_01D0:
                End Try
            End Sub, "MyImage PicLoader " + Conversions.ToString(ModBase.GetUuid()) + "#", ThreadPriority.BelowNormal)
        End Sub

        Public FileCacheExpiredTime As TimeSpan

        Public Shared EnableCacheProperty As DependencyProperty = DependencyProperty.Register("EnableCache", GetType(Boolean), GetType(MyImage), New PropertyMetadata(True))

        Private _Source As String

        Public Shared SourceProperty As DependencyProperty = DependencyProperty.Register("Source", GetType(String), GetType(MyImage), New PropertyMetadata(Sub(sender As DependencyObject, e As DependencyPropertyChangedEventArgs)
            If sender IsNot Nothing Then
                CType(sender, MyImage).Source = e.NewValue.ToString()
            End If
        End Sub))

        Private _FallbackSource As String

        Private _LoadingSource As String

        Private _ActualSource As String
    End Class
End Namespace
