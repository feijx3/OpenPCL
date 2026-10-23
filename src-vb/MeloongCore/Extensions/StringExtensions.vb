Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace MeloongCore.Extensions
    <NullableContext(1)>
    <Nullable(0)>
    Public Module StringExtensions
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function StartsWithF(<Nullable(2)> value As String, prefix As String, Optional ignoreCase As Boolean = False) As Boolean
            Return value IsNot Nothing AndAlso value.StartsWith(prefix, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <NullableContext(2)>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function StartsWithF(value As String, prefix As Char, Optional ignoreCase As Boolean = False) As Boolean
            Return value IsNot Nothing AndAlso value.StartsWith(prefix.ToString(), If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function EndsWithF(<Nullable(2)> value As String, suffix As String, Optional ignoreCase As Boolean = False) As Boolean
            Return value IsNot Nothing AndAlso value.EndsWith(suffix, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <NullableContext(2)>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function EndsWithF(value As String, suffix As Char, Optional ignoreCase As Boolean = False) As Boolean
            Return value IsNot Nothing AndAlso value.EndsWith(suffix.ToString(), If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ContainsIgnoreCase(value As String, subString As String) As Boolean
            Return value.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IndexOfF(value As String, subString As String, Optional ignoreCase As Boolean = False) As Integer
            Return value.IndexOf(subString, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IndexOfF(value As String, subString As String, startIndex As Integer, Optional ignoreCase As Boolean = False) As Integer
            Return value.IndexOf(subString, startIndex, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function LastIndexOfF(value As String, subString As String, Optional ignoreCase As Boolean = False) As Integer
            Return value.LastIndexOf(subString, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function LastIndexOfF(value As String, subString As String, startIndex As Integer, Optional ignoreCase As Boolean = False) As Integer
            Return value.LastIndexOf(subString, startIndex, If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Lower(str As String) As String
            Return str.ToLowerInvariant()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Upper(str As String) As String
            Return str.ToUpperInvariant()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Split(fullStr As String, splitStr As String, Optional removeEmptyEntries As Boolean = False) As String()
            If splitStr.IsSingle() Then
                Return fullStr.Split(splitStr(0), removeEmptyEntries)
            End If
            Return fullStr.Split(New String() { splitStr }, removeEmptyEntries)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Split(fullStr As String, splitChar As Char, Optional removeEmptyEntries As Boolean = False) As String()
            Return fullStr.Split(New Char() { splitChar }, removeEmptyEntries)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Split(fullStr As String, splitStrings As String(), Optional removeEmptyEntries As Boolean = False) As String()
            Return fullStr.Split(splitStrings, If(removeEmptyEntries, StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.None))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Split(fullStr As String, splitChars As Char(), Optional removeEmptyEntries As Boolean = False) As String()
            Return fullStr.Split(splitChars, If(removeEmptyEntries, StringSplitOptions.RemoveEmptyEntries, StringSplitOptions.None))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function SplitLines(input As String, Optional skipEmptyLines As Boolean = False) As String()
            Return input.ReplaceLineEndings(vbLf, False).Split(vbLf, skipEmptyLines)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function BeforeFirst(str As String, text As String, Optional ignoreCase As Boolean = False) As String
            Dim num As Integer = If(String.IsNullOrEmpty(text), (-1), str.IndexOfF(text, ignoreCase))
            If num < 0 Then
                Return str
            End If
            Return str.Substring(0, num)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function BeforeFirstOfAny(str As String, <Nullable(New Byte() { 2, 1 })> texts As IEnumerable(Of String), Optional ignoreCase As Boolean = False) As String
            If texts Is Nothing Then
                Return str
            End If
            Dim num As Integer = -1
            For Each text As String In texts
                If Not String.IsNullOrEmpty(text) Then
                    Dim num2 As Integer = str.IndexOfF(text, ignoreCase)
                    If num2 >= 0 AndAlso (num < 0 OrElse num > num2) Then
                        num = num2
                    End If
                End If
            Next
            If num < 0 Then
                Return str
            End If
            Return str.Substring(0, num)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function BeforeLast(str As String, text As String, Optional ignoreCase As Boolean = False) As String
            Dim num As Integer = If(String.IsNullOrEmpty(text), (-1), str.LastIndexOfF(text, ignoreCase))
            If num < 0 Then
                Return str
            End If
            Return str.Substring(0, num)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function BeforeLastOfAny(str As String, <Nullable(New Byte() { 2, 1 })> texts As IEnumerable(Of String), Optional ignoreCase As Boolean = False) As String
            If texts Is Nothing Then
                Return str
            End If
            Dim num As Integer = -1
            For Each text As String In texts
                If Not String.IsNullOrEmpty(text) Then
                    Dim num2 As Integer = str.LastIndexOfF(text, ignoreCase)
                    If num2 >= 0 AndAlso (num < 0 OrElse num < num2) Then
                        num = num2
                    End If
                End If
            Next
            If num < 0 Then
                Return str
            End If
            Return str.Substring(0, num)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function AfterFirst(str As String, <Nullable(2)> text As String, Optional ignoreCase As Boolean = False) As String
            Dim num As Integer = If(String.IsNullOrEmpty(text), (-1), str.IndexOfF(text, ignoreCase))
            If num < 0 Then
                Return str
            End If
            Return str.Substring(num + text.Length)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function AfterFirstOfAny(str As String, <Nullable(New Byte() { 2, 1 })> texts As IEnumerable(Of String), Optional ignoreCase As Boolean = False) As String
            If texts Is Nothing Then
                Return str
            End If
            Dim num As Integer = -1
            Dim num2 As Integer = 0
            For Each text As String In texts
                If Not String.IsNullOrEmpty(text) Then
                    Dim num3 As Integer = str.IndexOfF(text, ignoreCase)
                    If num3 >= 0 AndAlso (num < 0 OrElse num > num3) Then
                        num = num3
                        num2 = text.Length
                    End If
                End If
            Next
            If num < 0 Then
                Return str
            End If
            Return str.Substring(num + num2)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function AfterLast(str As String, <Nullable(2)> text As String, Optional ignoreCase As Boolean = False) As String
            Dim num As Integer = If(String.IsNullOrEmpty(text), (-1), str.LastIndexOfF(text, ignoreCase))
            If num < 0 Then
                Return str
            End If
            Return str.Substring(num + text.Length)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function AfterLastOfAny(str As String, <Nullable(New Byte() { 2, 1 })> texts As IEnumerable(Of String), Optional ignoreCase As Boolean = False) As String
            If texts Is Nothing Then
                Return str
            End If
            Dim num As Integer = -1
            Dim num2 As Integer = 0
            For Each text As String In texts
                If Not String.IsNullOrEmpty(text) Then
                    Dim num3 As Integer = str.LastIndexOfF(text, ignoreCase)
                    If num3 >= 0 AndAlso (num < 0 OrElse num < num3) Then
                        num = num3
                        num2 = text.Length
                    End If
                End If
            Next
            If num < 0 Then
                Return str
            End If
            Return str.Substring(num + num2)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Between(str As String, after As String, before As String, Optional ignoreCase As Boolean = False) As String
            Dim num As Integer = If(String.IsNullOrEmpty(after), (-1), str.LastIndexOfF(after, ignoreCase))
            If num >= 0 Then
                num += after.Length
            Else
                num = 0
            End If
            Dim num2 As Integer = If(String.IsNullOrEmpty(before), (-1), str.IndexOfF(before, num, ignoreCase))
            If num2 >= 0 Then
                Dim num3 As Integer = num
                Return str.Substring(num3, num2 - num3)
            End If
            If num > 0 Then
                Return str.Substring(num)
            End If
            Return str
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Replace(str As String, oldValue As String, newValueGetter As Func(Of String)) As String
            If Not str.Contains(oldValue) Then
                Return str
            End If
            Return str.Replace(oldValue, newValueGetter())
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ReplaceLineEndings(input As String, newValue As String, Optional mergeMultiple As Boolean = False) As String
            Return If(mergeMultiple, StringExtensions.regexLineEndingAndMerge, StringExtensions.regexLineEnding).Replace(input, newValue.Replace("$", "$$"))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RegexSearch(str As String, pattern As String, Optional options As RegexOptions = RegexOptions.None) As IEnumerable(Of String)
            Dim enumerable2 As IEnumerable(Of String)
            Try
                Dim enumerable As IEnumerable(Of Match) = Regex.Matches(str, pattern, options).Cast()
                Dim <>9__29_ As Func(Of Match, String) = StringExtensions.<>c.<>9__29_0
                Dim func As Func(Of Match, String) = <>9__29_
                If <>9__29_ Is Nothing Then
                    Dim func2 As Func(Of Match, String) = Function(m As Match) m.Value
                    func = func2
                    StringExtensions.<>c.<>9__29_0 = func2
                End If
                enumerable2 = enumerable.[Select](func)
            Catch ex As Exception
                Logger.Warn(ex, "RegexSearch 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Extensions\StringExtensions.cs")
                enumerable2 = Array.Empty(Of String)()
            End Try
            Return enumerable2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RegexSeek(str As String, pattern As String, Optional options As RegexOptions = RegexOptions.None) As <Nullable(2)> String
            Dim text As String
            Try
                Dim match As Match = Regex.Match(str, pattern, options)
                text = If(match.Success, match.Value, Nothing)
            Catch ex As Exception
                Logger.Warn(ex, "RegexSeek 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Extensions\StringExtensions.cs")
                text = Nothing
            End Try
            Return text
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RegexCheck(str As String, pattern As String, Optional options As RegexOptions = RegexOptions.None) As Boolean
            Dim flag As Boolean
            Try
                flag = Regex.IsMatch(str, pattern, options)
            Catch ex As Exception
                Logger.Warn(ex, "RegexCheck 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\Projects\PCL\本体代码\MeloongCore\Shared\Extensions\StringExtensions.cs")
                flag = False
            End Try
            Return flag
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RegexReplace(allContents As String, searchRegex As String, replaceTo As String, Optional options As RegexOptions = RegexOptions.None) As String
            Return Regex.Replace(allContents, searchRegex, replaceTo, options)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function RegexReplace(allContents As String, searchRegex As String, replaceTo As MatchEvaluator, Optional options As RegexOptions = RegexOptions.None) As String
            Return Regex.Replace(allContents, searchRegex, replaceTo, options)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ReadString(stream As Stream, <Nullable(2)> Optional encoding As Encoding = Nothing) As String
            Dim [string] As String
            Using memoryStream As MemoryStream = New MemoryStream()
                If stream.CanSeek AndAlso stream.Position <> 0L Then
                    stream.Seek(0L, SeekOrigin.Begin)
                End If
                stream.CopyTo(memoryStream)
                [string] = memoryStream.ToArray().GetString(encoding)
            End Using
            Return [string]
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function GetString(bytes As Byte(), <Nullable(2)> Optional encoding As Encoding = Nothing) As String
            If encoding IsNot Nothing Then
                Return encoding.GetString(bytes)
            End If
            Dim num As Integer = bytes.Length
            If num >= 3 AndAlso bytes(0) = 239 AndAlso bytes(1) = 187 AndAlso bytes(2) = 191 Then
                Return Encoding.UTF8.GetString(bytes, 3, num - 3)
            End If
            If num >= 2 AndAlso bytes(0) = 254 AndAlso bytes(1) = 255 Then
                Return Encoding.BigEndianUnicode.GetString(bytes, 2, num - 2)
            End If
            If num >= 2 AndAlso bytes(0) = 255 AndAlso bytes(1) = 254 Then
                Return Encoding.Unicode.GetString(bytes, 2, num - 2)
            End If
            Dim text As String
            Try
                text = New UTF8Encoding(False, True).GetString(bytes)
            Catch ex As DecoderFallbackException
                text = Encoding.GetEncoding("GB18030").GetString(bytes)
            End Try
            Return text
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Capitalize(word As String) As String
            If String.IsNullOrEmpty(word) Then
                Return word
            End If
            Return word.Substring(0, 1).Upper() + word.Substring(1).Lower()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function EnsureLength(<Nullable(2)> str As String, code As Char, length As Integer) As String
            If str Is Nothing Then
                str = ""
            End If
            If str.Length <= length Then
                Return str.PadLeft(length, code)
            End If
            Return str.Substring(0, length)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsAsciiOnly(input As String) As Boolean
            Dim <>9__38_ As Func(Of Char, Boolean) = StringExtensions.<>c.<>9__38_0
            Dim func As Func(Of Char, Boolean) = <>9__38_
            If <>9__38_ Is Nothing Then
                Dim func2 As Func(Of Char, Boolean) = Function(c As Char) c < ChrW(128)
                func = func2
                StringExtensions.<>c.<>9__38_0 = func2
            End If
            Return input.All(func)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function GetStableHashCode(str As String) As ULong
            Dim num As ULong = 5381UL
            For Each c As Char In str
                num = (num << 5) Xor num Xor CULng(c)
            Next
            Return num Xor 12218072394304324399UL
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function DeserializeJson(data As String) As <Nullable(2)> JToken
            Dim jtoken As JToken
            Try
                jtoken = JsonConvert.DeserializeObject(Of JToken)(data, New JsonSerializerSettings() With { .DateTimeZoneHandling = 0 })
            Catch ex As Exception
                Dim length As Integer = (If(data, "")).Length
                Throw New FormatException("JSON deserialize failed: " + If((length > 2000), String.Format("{0}...(total {1} characters)...{2}", data.Substring(0, 500), length, data.Substring(data.Length - 500)), (If(data, ""))), ex)
            End Try
            Return jtoken
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function DeserializeJson(Of T As JToken)(data As String) As <Nullable(2)> T
            Return CType(CObj(data.DeserializeJson()), T)
        End Function

        Private regexLineEndingAndMerge As Regex = New Regex("(?:\r\n|[\n\r\f\u0085\u2028\u2029])+", RegexOptions.Compiled)

        Private regexLineEnding As Regex = New Regex("\r\n|[\n\r\f\u0085\u2028\u2029]", RegexOptions.Compiled)
    End Module
End Namespace
