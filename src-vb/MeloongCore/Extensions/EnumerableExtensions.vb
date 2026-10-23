Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace MeloongCore.Extensions
    <NullableContext(1)>
    <Nullable(0)>
    Public Module EnumerableExtensions
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function FirstOrDefault(Of TSource)(source As IEnumerable(Of TSource), ParamArray predicates As Func(Of TSource, Boolean)()) As <Nullable(2)> TSource
            For Each func As Func(Of TSource, Boolean) In predicates
                For Each tsource As TSource In source
                    If func(tsource) Then
                        Return tsource
                    End If
                Next
            Next
            Return Nothing
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Distinct(Of T)(list As IList(Of T), comparer As Func(Of T, T, Boolean)) As IEnumerable(Of T)
            Dim <Distinct>d__ As EnumerableExtensions.<Distinct>d__1(Of T) = New EnumerableExtensions.<Distinct>d__1(Of T)(-2)
            <Distinct>d__.<>3__list = list
            <Distinct>d__.<>3__comparer = comparer
            Return <Distinct>d__
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function DistinctBy(Of TSource, TKey)(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey)) As IEnumerable(Of TSource)
            Dim <DistinctBy>d__ As EnumerableExtensions.<DistinctBy>d__2(Of TSource, TKey) = New EnumerableExtensions.<DistinctBy>d__2(Of TSource, TKey)(-2)
            <DistinctBy>d__.<>3__source = source
            <DistinctBy>d__.<>3__selector = selector
            Return <DistinctBy>d__
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function DistinctBy(Of TSource, TKey)(source As ParallelQuery(Of TSource), selector As Func(Of TSource, TKey)) As ParallelQuery(Of TSource)
            Dim seen As ConcurrentDictionary(Of TKey, Boolean) = New ConcurrentDictionary(Of TKey, Boolean)()
            Return source.Where(Function(element As TSource) seen.TryAdd(selector(element), True))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MaxByAll(Of TSource, TKey As IComparable(Of TKey))(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey)) As List(Of TSource)
            Dim list As List(Of TSource) = New List(Of TSource)()
            Dim list2 As List(Of TSource)
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    list2 = list
                Else
                    Dim tsource As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource)
                    list.Add(tsource)
                    While enumerator.MoveNext()
                        Dim tsource2 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource2)
                        Dim num As Integer = tkey2.CompareTo(tkey)
                        If num > 0 Then
                            tkey = tkey2
                            list.Clear()
                            list.Add(tsource2)
                        ElseIf num = 0 Then
                            list.Add(tsource2)
                        End If
                    End While
                    list2 = list
                End If
            End Using
            Return list2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MaxByAll(Of TSource, TKey)(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey), comparer As IComparer(Of TKey)) As List(Of TSource)
            Dim list As List(Of TSource) = New List(Of TSource)()
            Dim list2 As List(Of TSource)
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    list2 = list
                Else
                    Dim tsource As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource)
                    list.Add(tsource)
                    While enumerator.MoveNext()
                        Dim tsource2 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource2)
                        Dim num As Integer = comparer.Compare(tkey2, tkey)
                        If num > 0 Then
                            tkey = tkey2
                            list.Clear()
                            list.Add(tsource2)
                        ElseIf num = 0 Then
                            list.Add(tsource2)
                        End If
                    End While
                    list2 = list
                End If
            End Using
            Return list2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MinByAll(Of TSource, TKey As IComparable(Of TKey))(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey)) As List(Of TSource)
            Dim list As List(Of TSource) = New List(Of TSource)()
            Dim list2 As List(Of TSource)
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    list2 = list
                Else
                    Dim tsource As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource)
                    list.Add(tsource)
                    While enumerator.MoveNext()
                        Dim tsource2 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource2)
                        Dim num As Integer = tkey2.CompareTo(tkey)
                        If num < 0 Then
                            tkey = tkey2
                            list.Clear()
                            list.Add(tsource2)
                        ElseIf num = 0 Then
                            list.Add(tsource2)
                        End If
                    End While
                    list2 = list
                End If
            End Using
            Return list2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MinByAll(Of TSource, TKey)(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey), comparer As IComparer(Of TKey)) As List(Of TSource)
            Dim list As List(Of TSource) = New List(Of TSource)()
            Dim list2 As List(Of TSource)
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    list2 = list
                Else
                    Dim tsource As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource)
                    list.Add(tsource)
                    While enumerator.MoveNext()
                        Dim tsource2 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource2)
                        Dim num As Integer = comparer.Compare(tkey2, tkey)
                        If num < 0 Then
                            tkey = tkey2
                            list.Clear()
                            list.Add(tsource2)
                        ElseIf num = 0 Then
                            list.Add(tsource2)
                        End If
                    End While
                    list2 = list
                End If
            End Using
            Return list2
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MaxBy(Of TSource, TKey As IComparable(Of TKey))(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey)) As <Nullable(2)> TSource
            Dim tsource As TSource
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    tsource = Nothing
                    tsource = tsource
                Else
                    Dim tsource2 As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource2)
                    While enumerator.MoveNext()
                        Dim tsource3 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource3)
                        If tkey2.CompareTo(tkey) > 0 Then
                            tsource2 = enumerator.Current
                            tkey = tkey2
                        End If
                    End While
                    tsource = tsource2
                End If
            End Using
            Return tsource
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MaxBy(Of TSource, TKey)(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey), comparer As IComparer(Of TKey)) As <Nullable(2)> TSource
            Dim tsource As TSource
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    tsource = Nothing
                    tsource = tsource
                Else
                    Dim tsource2 As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource2)
                    While enumerator.MoveNext()
                        Dim tsource3 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource3)
                        If comparer.Compare(tkey2, tkey) > 0 Then
                            tsource2 = enumerator.Current
                            tkey = tkey2
                        End If
                    End While
                    tsource = tsource2
                End If
            End Using
            Return tsource
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MinBy(Of TSource, TKey As IComparable(Of TKey))(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey)) As <Nullable(2)> TSource
            Dim tsource As TSource
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    tsource = Nothing
                    tsource = tsource
                Else
                    Dim tsource2 As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource2)
                    While enumerator.MoveNext()
                        Dim tsource3 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource3)
                        If tkey2.CompareTo(tkey) < 0 Then
                            tsource2 = enumerator.Current
                            tkey = tkey2
                        End If
                    End While
                    tsource = tsource2
                End If
            End Using
            Return tsource
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function MinBy(Of TSource, TKey)(source As IEnumerable(Of TSource), selector As Func(Of TSource, TKey), comparer As IComparer(Of TKey)) As <Nullable(2)> TSource
            Dim tsource As TSource
            Using enumerator As IEnumerator(Of TSource) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    tsource = Nothing
                    tsource = tsource
                Else
                    Dim tsource2 As TSource = enumerator.Current
                    Dim tkey As TKey = selector(tsource2)
                    While enumerator.MoveNext()
                        Dim tsource3 As TSource = enumerator.Current
                        Dim tkey2 As TKey = selector(tsource3)
                        If comparer.Compare(tkey2, tkey) < 0 Then
                            tsource2 = enumerator.Current
                            tkey = tkey2
                        End If
                    End While
                    tsource = tsource2
                End If
            End Using
            Return tsource
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Ordered(Of T As IComparable(Of T))(source As IEnumerable(Of T)) As IEnumerable(Of T)
            Dim <>9__12_ As Func(Of T, T) = EnumerableExtensions.<>c__12(Of T).<>9__12_0
            Dim func As Func(Of T, T) = <>9__12_
            If <>9__12_ Is Nothing Then
                Dim func2 As Func(Of T, T) = Function(x As T) x
                func = func2
                EnumerableExtensions.<>c__12(Of T).<>9__12_0 = func2
            End If
            Return source.OrderBy(func)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function OrderedDescending(Of T As IComparable(Of T))(source As IEnumerable(Of T)) As IEnumerable(Of T)
            Dim <>9__13_ As Func(Of T, T) = EnumerableExtensions.<>c__13(Of T).<>9__13_0
            Dim func As Func(Of T, T) = <>9__13_
            If <>9__13_ Is Nothing Then
                Dim func2 As Func(Of T, T) = Function(x As T) x
                func = func2
                EnumerableExtensions.<>c__13(Of T).<>9__13_0 = func2
            End If
            Return source.OrderByDescending(func)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Join(list As IEnumerable, split As Char) As String
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Dim flag As Boolean = True
            For Each obj As Object In list
                If flag Then
                    flag = False
                Else
                    stringBuilder.Append(split)
                End If
                If obj IsNot Nothing Then
                    stringBuilder.Append(obj.ToString())
                End If
            Next
            Return stringBuilder.ToString()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Join(list As IEnumerable, split As String) As String
            If split.IsSingle() Then
                Return list.Join(split(0))
            End If
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Dim flag As Boolean = True
            For Each obj As Object In list
                If flag Then
                    flag = False
                Else
                    stringBuilder.Append(split)
                End If
                If obj IsNot Nothing Then
                    stringBuilder.Append(obj)
                End If
            Next
            Return stringBuilder.ToString()
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Remove(Of TKey, TValue)(dict As ConcurrentDictionary(Of TKey, TValue), key As TKey) As Boolean
            Dim tvalue As TValue
            Return dict.TryRemove(key, tvalue)
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ToDictionary(Of T)(arr As T(,)) As Dictionary(Of T, T)
            Dim dictionary As Dictionary(Of T, T) = New Dictionary(Of T, T)()
            If arr.Length = 0 Then
                Return dictionary
            End If
            If arr.GetLength(1) <> 2 Then
                Throw New ArgumentException("数组必须为两列，第一列为 Key，第二列为 Value。")
            End If
            For i As Integer = 0 To arr.GetLength(0) - 1
                dictionary(arr(i, 0)) = arr(i, 1)
            Next
            Return dictionary
        End Function

        <NullableContext(2)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function GetOrDefault(Of TKey, TValue)(<Nullable(1)> dict As IDictionary(Of TKey, TValue), <Nullable(1)> key As TKey, Optional defaultValue As TValue = Nothing) As TValue
            Dim tvalue As TValue
            If Not dict.TryGetValue(key, tvalue) Then
                Return defaultValue
            End If
            Return tvalue
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub AddIntoValueCollection(Of TKey, TValue)(dict As IDictionary(Of TKey, List(Of TValue)), key As TKey, value As TValue)
            Dim list As List(Of TValue)
            If dict.TryGetValue(key, list) Then
                list.Add(value)
                Return
            End If
            dict.Add(key, New List(Of TValue)(1) From { value })
        End Sub

        <NullableContext(0)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Flags(Of T As{Structure, [Enum]})(value As T) As <Nullable(New Byte() { 1, 0 })> IEnumerable(Of T)
            Return EnumUtils.GetAllFlags(Of T)().Where(Function(element As T) value.HasFlag(element))
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub RemoveIf(Of T)(source As ICollection(Of T), predicate As Func(Of T, Boolean))
            For Each t As T In source.Where(predicate).ToList()
                source.Remove(t)
            Next
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub RemoveAll(Of T)(source As ICollection(Of T), itemsToRemove As IEnumerable(Of T))
            For Each t As T In itemsToRemove
                source.Remove(t)
            Next
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Sub KeepIf(Of T)(source As ICollection(Of T), predicate As Func(Of T, Boolean))
            For Each t As T In source.Where(Function(item As T) Not predicate(item)).ToList()
                source.Remove(t)
            Next
        End Sub

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function Except(Of T)(source As IEnumerable(Of T), element As T) As IEnumerable(Of T)
            Return source.Except(New <>z__ReadOnlySingleElementList(Of T)(element))
        End Function

        <NullableContext(2)>
        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function IsSingle(Of T)(<Nullable(New Byte() { 2, 1 })> source As IEnumerable(Of T)) As Boolean
            If source Is Nothing Then
                Return False
            End If
            Dim list As IList(Of T) = TryCast(source, IList(Of T))
            If list IsNot Nothing Then
                Return list.Count = 1
            End If
            Dim flag As Boolean
            Using enumerator As IEnumerator(Of T) = source.GetEnumerator()
                If Not enumerator.MoveNext() Then
                    flag = False
                ElseIf enumerator.MoveNext() Then
                    flag = False
                Else
                    flag = True
                End If
            End Using
            Return flag
        End Function

        <System.Runtime.CompilerServices.ExtensionAttribute()>
        Public Function ForAll(Of T)(source As IEnumerable(Of T), action As Action(Of T)) As IEnumerable(Of T)
            For Each t As T In source
                action(t)
            Next
            Return source
        End Function
    End Module
End Namespace
