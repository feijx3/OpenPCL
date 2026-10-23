Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    Public Module EnumUtils
        Private Function GetAllValues(Of T As{Structure, [Enum]})() As <Nullable(New Byte() { 1, 0 })> IEnumerable(Of T)
            Return [Enum].GetValues(GetType(T)).Cast()
        End Function

        Public Function FromString(Of T As{Structure, [Enum]})(<Nullable(2)> value As String) As T
            If String.IsNullOrEmpty(value) Then
                Return CType(CObj([Enum].ToObject(GetType(T), 0)), T)
            End If
            Dim num As Double
            If Double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, num) Then
                Return CType(CObj([Enum].ToObject(GetType(T), Convert.ToInt64(num))), T)
            End If
            Return CType(CObj([Enum].Parse(GetType(T), value, True)), T)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function HasFlagF(Of T As{Structure, [Enum]})(value As T, flag As T) As Boolean
            Return value.HasFlag(flag) AndAlso Not EqualityComparer(Of T).[Default].Equals(flag, Nothing)
        End Function

        Public Function GetAllFlags(Of T As{Structure, [Enum]})() As <Nullable(New Byte() { 1, 0 })> List(Of T)
            Dim list As List(Of [Enum])
            If EnumUtils.allFlagsCache.TryGetValue(GetType(T), list) Then
                Return list.Cast().ToList()
            End If
            If Not GetType(T).IsDefined(GetType(FlagsAttribute), False) Then
                Throw New ArgumentException("枚举 " + GetType(T).FullName + " 没有定义 FlagsAttribute。")
            End If
            Dim list2 As List(Of T) = New List(Of T)()
            For Each t As T In EnumUtils.GetAllValues(Of T)().Distinct()
                Dim num As Long = Convert.ToInt64(t, CultureInfo.InvariantCulture)
                If num <> 0L AndAlso (num And (num - 1L)) = 0L Then
                    list2.Add(t)
                End If
            Next
            EnumUtils.allFlagsCache(GetType(T)) = list2.Cast().ToList()
            Return list2
        End Function

        <Nullable(1)>
        Private allFlagsCache As ConcurrentDictionary(Of Type, List(Of [Enum])) = New ConcurrentDictionary(Of Type, List(Of [Enum]))()
    End Module
End Namespace
