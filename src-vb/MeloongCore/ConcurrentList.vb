Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    <NullableContext(1)>
    <Nullable(0)>
    Public Class ConcurrentList(Of T)
        Implements IList(Of T), ICollection(Of T), IEnumerable(Of T), IEnumerable, IList, ICollection

        Public Sub New()
        End Sub

        Public Sub New(data As IEnumerable(Of T))
            Me._items = New List(Of T)(data)
        End Sub

        Public Shared Widening Operator CType(data As List(Of T)) As ConcurrentList(Of T)
            Return New ConcurrentList(Of T)(data)
        End Operator

        Public Shared Widening Operator CType(data As ConcurrentList(Of T)) As List(Of T)
            Dim syncRoot As Object = data.SyncRoot
            Dim list As List(Of T)
            SyncLock syncRoot
                list = New List(Of T)(data._items)
            End SyncLock
            Return list
        End Operator

        Public Function GetEnumerator() As IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
            Dim syncRoot As Object = Me.SyncRoot
            Dim enumerator As IEnumerator(Of T)
            SyncLock syncRoot
                enumerator = Me._items.ToList().GetEnumerator()
            End SyncLock
            Return enumerator
        End Function

        Function GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Dim syncRoot As Object = Me.SyncRoot
            Dim enumerator As IEnumerator
            SyncLock syncRoot
                enumerator = Me._items.ToList().GetEnumerator()
            End SyncLock
            Return enumerator
        End Function

        Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count, System.Collections.ICollection.Count
            Get
                Dim syncRoot As Object = Me.SyncRoot
                Dim count As Integer
                SyncLock syncRoot
                    count = Me._items.Count
                End SyncLock
                Return count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly, System.Collections.IList.IsReadOnly
            Get
                Return False
            End Get
        End Property

        ReadOnly Property IsFixedSize As Boolean Implements System.Collections.IList.IsFixedSize
            Get
                Return False
            End Get
        End Property

        ReadOnly Property IsSynchronized As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return True
            End Get
        End Property

        ReadOnly Property SyncRoot As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return Me.SyncRoot
            End Get
        End Property

        Public Default Property Item(index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
            Get
                Dim syncRoot As Object = Me.SyncRoot
                Dim t As T
                SyncLock syncRoot
                    t = Me._items(index)
                End SyncLock
                Return t
            End Get
            Set(value As T)
                Dim syncRoot As Object = Me.SyncRoot
                SyncLock syncRoot
                    Me._items(index) = value
                End SyncLock
            End Set
        End Property

        <Nullable(2)>
        Default Property Item(index As Integer) As Object Implements System.Collections.IList.Item
            <NullableContext(2)>
            Get
                Return Me(index)
            End Get
            <NullableContext(2)>
            Set(value As Object)
                Dim syncRoot As Object = Me.SyncRoot
                SyncLock syncRoot
                    Me._items(index) = CType(CObj(value), T)
                End SyncLock
            End Set
        End Property

        Public Sub Add(item As T) Implements System.Collections.Generic.ICollection(Of T).Add
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.Add(item)
            End SyncLock
        End Sub

        <NullableContext(2)>
        Function Add(value As Object) As Integer Implements System.Collections.IList.Add
            Dim syncRoot As Object = Me.SyncRoot
            Dim num As Integer
            SyncLock syncRoot
                Me._items.Add(CType(CObj(value), T))
                num = Me._items.Count - 1
            End SyncLock
            Return num
        End Function

        Public Sub Insert(index As Integer, item As T) Implements System.Collections.Generic.IList(Of T).Insert
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.Insert(index, item)
            End SyncLock
        End Sub

        <NullableContext(2)>
        Sub Insert(index As Integer, value As Object) Implements System.Collections.IList.Insert
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.Insert(index, CType(CObj(value), T))
            End SyncLock
        End Sub

        Public Function Remove(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
            Dim syncRoot As Object = Me.SyncRoot
            Dim flag2 As Boolean
            SyncLock syncRoot
                flag2 = Me._items.Remove(item)
            End SyncLock
            Return flag2
        End Function

        <NullableContext(2)>
        Sub Remove(value As Object) Implements System.Collections.IList.Remove
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                If TypeOf value Is T Then
                    Dim t As T = CType(CObj(value), T)
                    Me._items.Remove(t)
                End If
            End SyncLock
        End Sub

        Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt, System.Collections.IList.RemoveAt
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.RemoveAt(index)
            End SyncLock
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear, System.Collections.IList.Clear
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.Clear()
            End SyncLock
        End Sub

        Public Function Contains(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
            Dim syncRoot As Object = Me.SyncRoot
            Dim flag2 As Boolean
            SyncLock syncRoot
                flag2 = Me._items.Contains(item)
            End SyncLock
            Return flag2
        End Function

        <NullableContext(2)>
        Function Contains(value As Object) As Boolean Implements System.Collections.IList.Contains
            Dim syncRoot As Object = Me.SyncRoot
            Dim flag3 As Boolean
            SyncLock syncRoot
                Dim flag2 As Boolean
                If TypeOf value Is T Then
                    Dim t As T = CType(CObj(value), T)
                    flag2 = Me._items.Contains(t)
                Else
                    flag2 = False
                End If
                flag3 = flag2
            End SyncLock
            Return flag3
        End Function

        Public Function IndexOf(item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
            Dim syncRoot As Object = Me.SyncRoot
            Dim num As Integer
            SyncLock syncRoot
                num = Me._items.IndexOf(item)
            End SyncLock
            Return num
        End Function

        <NullableContext(2)>
        Function IndexOf(value As Object) As Integer Implements System.Collections.IList.IndexOf
            Dim syncRoot As Object = Me.SyncRoot
            Dim num2 As Integer
            SyncLock syncRoot
                Dim num As Integer
                If TypeOf value Is T Then
                    Dim t As T = CType(CObj(value), T)
                    num = Me._items.IndexOf(t)
                Else
                    num = -1
                End If
                num2 = num
            End SyncLock
            Return num2
        End Function

        Public Sub CopyTo(array As T(), arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                Me._items.CopyTo(array, arrayIndex)
            End SyncLock
        End Sub

        Sub CopyTo(array As Array, index As Integer) Implements System.Collections.ICollection.CopyTo
            Dim syncRoot As Object = Me.SyncRoot
            SyncLock syncRoot
                CType(Me._items, ICollection).CopyTo(array, index)
            End SyncLock
        End Sub

        Private _items As List(Of T) = New List(Of T)()

        Public SyncRoot As Object = New Object()
    End Class
End Namespace
