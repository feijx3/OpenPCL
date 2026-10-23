Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

<CompilerGenerated()>
Friend NotInheritable Class <>z__ReadOnlyArray(Of T)
    Implements IEnumerable, ICollection, IList, IEnumerable(Of T), IReadOnlyCollection(Of T), IReadOnlyList(Of T), ICollection(Of T), IList(Of T)

    Public Sub New(items As T())
        Me._items = items
    End Sub

    Function GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return Me._items.GetEnumerator()
    End Function

    ReadOnly Property Count As Integer Implements System.Collections.ICollection.Count
        Get
            Return Me._items.Length
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
        Me._items.CopyTo(array, index)
    End Sub

    Default Property Item(index As Integer) As Object Implements System.Collections.IList.Item
        Get
            Return Me._items(index)
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
        Return Me._items.Contains(value)
    End Function

    Function IndexOf(value As Object) As Integer Implements System.Collections.IList.IndexOf
        Return Me._items.IndexOf(value)
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
        Return Me._items.GetEnumerator()
    End Function

    ReadOnly Property Count As Integer Implements System.Collections.Generic.IReadOnlyCollection(Of T).Count
        Get
            Return Me._items.Length
        End Get
    End Property

    ReadOnly Default Property Item(index As Integer) As T Implements System.Collections.Generic.IReadOnlyList(Of T).Item
        Get
            Return Me._items(index)
        End Get
    End Property

    ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of T).Count
        Get
            Return Me._items.Length
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
        Return Me._items.Contains(item)
    End Function

    Sub CopyTo(array As T(), arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
        Me._items.CopyTo(array, arrayIndex)
    End Sub

    Function Remove(item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
        Throw New NotSupportedException()
    End Function

    Default Property Item(index As Integer) As T Implements System.Collections.Generic.IList(Of T).Item
        Get
            Return Me._items(index)
        End Get
        Set(value As T)
            Throw New NotSupportedException()
        End Set
    End Property

    Function IndexOf(item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
        Return Me._items.IndexOf(item)
    End Function

    Sub Insert(index As Integer, item As T) Implements System.Collections.Generic.IList(Of T).Insert
        Throw New NotSupportedException()
    End Sub

    Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of T).RemoveAt
        Throw New NotSupportedException()
    End Sub

    <CompilerGenerated()>
    Private _items As T()
End Class
