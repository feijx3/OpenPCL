Imports System
Imports System.Threading
Imports MeloongCore
Imports MeloongCore.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class PclLogger
        Inherits FileLogger

        Public Overrides Function Format(Text As String, Level As LogLevel, FilePath As String, Ex As Exception) As String
            Text = MyBase.Format(Text, Level, FilePath, Ex)
            Text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(Text, "*"c), "*"c)
            Return Text
        End Function

        Public Overrides Sub HandleBehavior(RawMessage As String, FormattedMessage As String, Behavior As LogBehavior, Ex As Exception)
            If Not ModBase.IsProgramEnding Then
                MyBase.HandleBehavior(RawMessage, FormattedMessage, Behavior, Ex)
                Dim text As String = If((Ex Is Nothing), RawMessage, (If((RawMessage Is Nothing), "", String.Format("{0}：", RawMessage)) + Ex.GetDisplay(False)))
                text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text, "*"c), "*"c)
                Dim text2 As String = If((Ex Is Nothing), RawMessage, (If((RawMessage Is Nothing), "", String.Format("{0}：", RawMessage)) + Ex.GetDisplay(True)))
                text2 = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text2, "*"c), "*"c)
                Select Case Behavior
                    Case LogBehavior.ToastIfDebug
                        If ModBase.ModeDebug Then
                            ModMain.Hint("[调试模式] " + text, ModMain.HintType.Blue, False)
                        End If
                    Case LogBehavior.Toast
                        ModMain.Hint(text, ModMain.HintType.Red, False)
                    Case LogBehavior.Alert
                        ModMain.OurMsgBox(text2, "错误", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                    Case LogBehavior.AlertThenFeedback
                        If PageSetupSystem.IsLauncherNewest().GetValueOrDefault(True) Then
                            If ModMain.OurMsgBox(text2 + vbCrLf & vbCrLf & "是否反馈此问题？如果不反馈，这个问题可能永远无法得到解决！", "错误", "反馈", "取消", "", True, True, False, Nothing, Nothing, Nothing) = 1 Then
                                ModMain.Feedback(False, True)
                            End If
                        Else
                            ModMain.OurMsgBox(text2 + vbCrLf & vbCrLf & "将 PCL 更新至最新版或许可以解决这个问题……", "错误", "确定", "", "", True, True, False, Nothing, Nothing, Nothing)
                        End If
                    Case LogBehavior.AlertThenCrash
                        If Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init Is Nothing Then
                            Interlocked.CompareExchange(Of StaticLocalInitFlag)(Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init, New StaticLocalInitFlag(), Nothing)
                        End If
                        Dim flag As Boolean = False
                        Try
                            Monitor.Enter(Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init, flag)
                            If Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 0S Then
                                Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 2S
                                Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger = True
                            ElseIf Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 2S Then
                                Throw New IncompleteInitialization()
                            End If
                        Finally
                            Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 1S
                            If flag Then
                                Monitor.[Exit](Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init)
                            End If
                        End Try
                        If Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger Then
                            Me.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger = False
                            If PageSetupSystem.IsLauncherNewest().GetValueOrDefault(True) Then
                                If Interaction.MsgBox(text2 + vbCrLf & vbCrLf & "是否反馈此问题？如果不反馈，这个问题可能永远无法得到解决！", MsgBoxStyle.YesNo Or MsgBoxStyle.Critical, "错误") = MsgBoxResult.Yes Then
                                    ModMain.Feedback(False, True)
                                End If
                            Else
                                Interaction.MsgBox(text2 + vbCrLf & vbCrLf & "将 PCL 更新至最新版或许可以解决这个问题……", MsgBoxStyle.Critical, "错误")
                            End If
                        Else
                            Thread.Sleep(2000)
                        End If
                        FormMain.EndProgramForce(ModBase.ProcessReturnValues.Exception)
                End Select
                If Behavior >= LogBehavior.Toast Then
                    ModMain.Telemetry("错误日志", New String() { "Exception", text2 })
                End If
            End If
        End Sub

        Private $STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger As Boolean

        Private $STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init As StaticLocalInitFlag
    End Class
End Namespace
