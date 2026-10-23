Imports System
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    Public Module GeneralUtils
        <NullableContext(1)>
        Public Function IsGenericInstanceOf(<Nullable(2)> instance As Object, genericTypeDefinition As Type) As Boolean
            If instance Is Nothing Then
                Return False
            End If
            Dim type As Type = instance.[GetType]()
            While type IsNot Nothing
                If type.IsGenericType AndAlso type.GetGenericTypeDefinition() Is genericTypeDefinition Then
                    Return True
                End If
                type = type.BaseType
            End While
            Return False
        End Function
    End Module
End Namespace
