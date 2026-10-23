Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

<CompilerGenerated()>
Friend NotInheritable Class <>z__ReadOnlySingleElementList(Of T)
    Implements IEnumerable, ICollection, IList, IEnumerable(Of T), IReadOnlyCollection(Of T), IReadOnlyList(Of T), ICollection(Of T), IList(Of T)

    Public Sub New(item As T)
        Me._item = item
    End Sub

    Function GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return New <>z__ReadOnlySingleElementList(Of T).Enumerator(Me._item)
    End Function

    ReadOnly Property Count As Integer Implements System.Collections.ICollection.Count
        Get
            Return 1
        End Get
    End Property

    ReadOnly Property IsSynchronized As Boolean Implements System.Collections.ICollection.IsSynchronized
        Get
            Return False
        End Get
    End Property

    ReadOnly Property SyncRoot As Object Implements System.Collections.ICollection.SyncRoot
        Get
            Return Me
        End Get
    End Property

    Sub CopyTo(array As Array, index As Integer) Implements System.Collections.ICollection.CopyTo
        array.SetValue(Me._item, index)
    End Sub

    Default Property Item(index As Integer) As Object Implements System.Collections.IList.Item
        Get
            If index <> 0 Then
                Throw New IndexOutOfRangeException()
            End If
            Return Me._item
        End Get
        Set(value As Object)
            Throw New NotSupportedException()
        End Set
    End Property

    ReadOnly Property IsFixedSize As Boolean Implements System.Collections.IList.IsFixedSize
        Get
            Return True
        End Get
    End Property

    ReadOnly Property IsReadOnly As Boolean Implements System.Collections.IList.IsReadOnly
        Get
            Return True
        End Get
    End Property

    Function Add(value As Object) As Integer Implements System.Collections.IList.Add
        Throw New NotSupportedException()
    End Function

    Sub Clear() Implements System.Collections.IList.Clear
        Throw New NotSupportedException()
    End Sub

    Function Contains(value As Object) As Boolean Implements System.Collections.IList.Contains
        Return EqualityComparer(Of T).[Default].Equals(Me._item, CType(CObj(value), T))
    End Function

    Function IndexOf(value As Object) As Integer Implements System.Collections.IList.IndexOf
        If Not EqualityComparer(Of T).[Default].Equals(Me._item, CType(CObj(value), T)) Then
            Return -1
        End If
        Return 0
    End Function

    Sub Insert(index As Integer, value As Object) Implements System.Collections.IList.Insert
        Throw New NotSupportedException()
    End Sub

    Sub Remove(value As Object) Implements System.Collections.IList.Remove
        Throw New NotSupportedException()
    End Sub

    Sub RemoveAt(index As Integer) Implements System.Collections.IList.RemoveAt
        Throw New NotSupportedException()
    End Sub

    Function GetEnumerator() As IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
        Return New <>z__ReadOnlySingleElementList(Of T).Enumerator(Me._item)
    End Function

    ReadOnly Property Count As Integer Implements System.Collections.Generic.IReadOnlyCollection(Of T).Count
        Get
            Return 1
        End Get
    End Property

    ReadOnly Default Property Item(index As Integer) As T Implements System.Collections.Generic.IReadOnlyList(Of T).Item
        Get
            If index <> 0 Then
                Throw New IndexOutOfRangeException()
            End If
            Return Me._item
        End Get
    End Property

    ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Return 1
        End Get
    End Property

    ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
        Get
            Return True
        End Get
    End Property

    Sub Add(item As T) Implements System.Collections.Generic.ICollection(Of T).Add
        Throw New NotSupportedException()
    End Sub

    Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
        Throw New NotSupportedException()
    End Sub

    Function Contains(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
        Return EqualityComparer(Of T).[Default].Equals(Me._item, item)
    End Function

    Sub CopyTo(array As T(), arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
        array(arrayIndex) = Me._item
    End Sub

    Function Remove(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
        Throw New NotSupportedException()
    End Function

    Default Property Item(index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
        Get
            If index <> 0 Then
                Throw New IndexOutOfRangeException()
            End If
            Return Me._item
        End Get
        Set(value As T)
            Throw New NotSupportedException()
        End Set
    End Property

    Function IndexOf(item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
        If Not EqualityComparer(Of T).[Default].Equals(Me._item, item) Then
            Return -1
        End If
        Return 0
    End Function

    Sub Insert(index As Integer, item As T) Implements System.Collections.Generic.IList(Of T).Insert
        Throw New NotSupportedException()
    End Sub

    Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt
        Throw New NotSupportedException()
    End Sub

    <CompilerGenerated()>
    Private _item As T

    Private NotInheritable Class Enumerator
        Implements IDisposable, IEnumerator, IEnumerator(Of T)

        Public Sub New(item As T)
            Me.System.Collections.Generic.IEnumerator<T>.Current = item
        End Sub

        ReadOnly Property Current As Object Implements System.Collections.IEnumerator.Current
            Get
                Return Me._item
            End Get
        End Property

        ReadOnly Property Current As T Implements System.Collections.Generic.IEnumerator(Of T).Current
            Get
                Return Me._item
            End Get
        End Property

        Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            If Not Me._moveNextCalled Then
                Dim num As Integer = 1
                Dim flag As Boolean = num <> 0
                Me._moveNextCalled = num <> 0
                Return flag
            End If
            Return False
        End Function

        Sub Reset() Implements System.Collections.IEnumerator.Reset
            Me._moveNextCalled = False
        End Sub

        Sub Dispose() Implements System.IDisposable.Dispose
        End Sub

        <CompilerGenerated()>
        Private _item As T

        <CompilerGenerated()>
        Private _moveNextCalled As Boolean
    End Class
End Class
