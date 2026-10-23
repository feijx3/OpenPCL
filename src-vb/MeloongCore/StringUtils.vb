Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports MeloongCore.Extensions

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Module StringUtils
        Public Function XmlEscape(str As String) As String
            If str.StartsWithF("{", False) Then
                str = "{}" + str
            End If
            Return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("""", "&quot;").ReplaceLineEndings("&#xa;", False)
        End Function

        Public Function LikePatternEscape(input As String) As String
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Dim i As Integer = 0
            While i < input.Length
                Dim c As Char = input(i)
                If c <= "*"c Then
                    If c <> "#"c AndAlso c <> "*"c Then
                        GoTo IL_0039
                    End If
                    GoTo IL_0034
                Else
                    If c = "?"c OrElse c = "["c OrElse c = "]"c Then
                        GoTo IL_0034
                    End If
                    GoTo IL_0039
                End If
                IL_003C:
                Dim flag As Boolean
                If flag Then
                    stringBuilder.Append("["c).Append(c).Append("]"c)
                Else
                    stringBuilder.Append(c)
                End If
                i += 1
                Continue While
                IL_0034:
                flag = True
                GoTo IL_003C
                IL_0039:
                flag = False
                GoTo IL_003C
            End While
            Return stringBuilder.ToString()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function RegexEscape(value As String) As String
            Return Regex.Escape(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function RegexUnescape(value As String) As String
            Return Regex.Unescape(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function UrlEscape(value As String) As String
            Return Uri.EscapeDataString(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function UrlUnescape(value As String) As String
            Return Uri.UnescapeDataString(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FormUrlEscape(value As String) As String
            Return WebUtility.UrlEncode(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FormUrlUnescape(value As String) As String
            Return WebUtility.UrlDecode(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HtmlEscape(value As String) As String
            Return WebUtility.HtmlEncode(value)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HtmlUnescape(value As String) As String
            Return WebUtility.HtmlDecode(value)
        End Function

        Public Function FormatByteSize(byteSize As Long) As String
            Dim text As String = If((byteSize < 0L), "-", "")
            If byteSize < 0L Then
                byteSize *= -1L
            End If
            If byteSize < 1000L Then
                Return String.Format("{0}{1} B", text, byteSize)
            End If
            If byteSize < 1024000L Then
                Dim text2 As String = Math.Round(CDbl(byteSize) / 1024.0).ToString(CultureInfo.InvariantCulture)
                Return String.Format("{0}{1} K", text, Math.Round(CDbl(byteSize) / 1024.0, (3 - text2.Length).Clamp(0, 2)))
            End If
            If byteSize < 1048576000L Then
                Dim text3 As String = Math.Round(CDbl(byteSize) / 1024.0 / 1024.0).ToString(CultureInfo.InvariantCulture)
                Return String.Format("{0}{1} M", text, Math.Round(CDbl(byteSize) / 1024.0 / 1024.0, (3 - text3.Length).Clamp(0, 2)))
            End If
            Dim text4 As String = Math.Round(CDbl(byteSize) / 1024.0 / 1024.0 / 1024.0).ToString(CultureInfo.InvariantCulture)
            Return String.Format("{0}{1} G", text, Math.Round(CDbl(byteSize) / 1024.0 / 1024.0 / 1024.0, (3 - text4.Length).Clamp(0, 2)))
        End Function

        Public Function FormatTimeSpan(span As TimeSpan, isShort As Boolean) As String
            Dim text As String = If((span.TotalMilliseconds > 0.0), "后", "前")
            If span.TotalMilliseconds < 0.0 Then
                span = -span
            End If
            Dim num As Integer = CInt(Math.Floor(CDbl(span.Days) / 30.0))
            If isShort Then
                If num >= 12 Then
                    Return String.Format("{0} 年{1}", Math.Floor(CDbl(num) / 12.0), text)
                End If
                If num >= 2 Then
                    Return String.Format("{0} 个月{1}", num, text)
                End If
                If span.TotalDays >= 2.0 Then
                    Return String.Format("{0} 天{1}", span.Days, text)
                End If
                If span.TotalHours >= 1.0 Then
                    Return String.Format("{0} 小时{1}", span.Hours, text)
                End If
                If span.TotalMinutes >= 1.0 Then
                    Return String.Format("{0} 分钟{1}", span.Minutes, text)
                End If
                If span.TotalSeconds >= 1.0 Then
                    Return String.Format("{0} 秒{1}", span.Seconds, text)
                End If
                Return "1 秒" + text
            Else
                If num >= 61 Then
                    Return String.Format("{0} 年{1}", Math.Floor(CDbl(num) / 12.0), text)
                End If
                If num >= 12 Then
                    Return String.Format("{0} 年{1}{2}", Math.Floor(CDbl(num) / 12.0), If((num Mod 12 > 0), String.Format(" {0} 个月", num Mod 12), ""), text)
                End If
                If num >= 4 Then
                    Return String.Format("{0} 个月{1}", num, text)
                End If
                If num >= 1 Then
                    Return String.Format("{0} 月{1}{2}", num, If((span.Days Mod 30 > 0), String.Format(" {0} 天", span.Days Mod 30), ""), text)
                End If
                If span.TotalDays >= 4.0 Then
                    Return String.Format("{0} 天{1}", span.Days, text)
                End If
                If span.TotalDays >= 1.0 Then
                    Return String.Format("{0} 天{1}{2}", span.Days, If((span.Hours > 0), String.Format(" {0} 小时", span.Hours), ""), text)
                End If
                If span.TotalHours >= 10.0 Then
                    Return String.Format("{0} 小时{1}", span.Hours, text)
                End If
                If span.TotalHours >= 1.0 Then
                    Return String.Format("{0} 小时{1}{2}", span.Hours, If((span.Minutes > 0), String.Format(" {0} 分钟", span.Minutes), ""), text)
                End If
                If span.TotalMinutes >= 10.0 Then
                    Return String.Format("{0} 分钟{1}", span.Minutes, text)
                End If
                If span.TotalMinutes >= 1.0 Then
                    Return String.Format("{0} 分{1}{2}", span.Minutes, If((span.Seconds > 0), String.Format(" {0} 秒", span.Seconds), ""), text)
                End If
                If span.TotalSeconds >= 1.0 Then
                    Return String.Format("{0} 秒{1}", span.Seconds, text)
                End If
                Return "1 秒" + text
            End If
        End Function

        Public Function ParseVersionWithDefaults(value As String) As Version
            While True
                Dim enumerable As IEnumerable(Of Char) = value
                Dim <>9__12_ As Func(Of Char, Boolean) = StringUtils.<>c.<>9__12_0
                Dim func As Func(Of Char, Boolean) = <>9__12_
                If <>9__12_ Is Nothing Then
                    Dim func2 As Func(Of Char, Boolean) = Function(c As Char) c = "."c
                    func = func2
                    StringUtils.<>c.<>9__12_0 = func2
                End If
                If enumerable.Count(func) >= 3 Then
                    Exit For
                End If
                value += ".0"
            End While
            Return Version.Parse(value)
        End Function
    End Module
End Namespace
