Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Newtonsoft.Json

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Class McInstallManifest
        Implements IEquatable(Of McInstallManifest)

        Public Sub New(SchemaVersion As Integer, Components As McComponents, LastModifiedUtc As DateTime, ManagedFiles As IReadOnlyList(Of McInstallManifest.ManagedFile))
            Me.SchemaVersion = SchemaVersion
            Me.Components = Components
            Me.LastModifiedUtc = LastModifiedUtc
            Me.ManagedFiles = ManagedFiles
            MyBase..ctor()
        End Sub

        <CompilerGenerated()>
        Protected Overridable ReadOnly Property EqualityContract As Type
            <CompilerGenerated()>
            Get
                Return GetType(McInstallManifest)
            End Get
        End Property

        Public Property SchemaVersion As Integer

        Public Property Components As McComponents

        Public Property LastModifiedUtc As DateTime

        Public Property ManagedFiles As IReadOnlyList(Of McInstallManifest.ManagedFile)

        Public Function RefreshModComponents(gameFolder As String) As McInstallManifest
            Dim invalidFiles As List(Of McInstallManifest.ManagedFile) = New List(Of McInstallManifest.ManagedFile)()
            For Each managedFile As McInstallManifest.ManagedFile In Me.ManagedFiles
                If managedFile.Component IsNot Nothing Then
                    Dim fileInfo As FileInfo = New FileInfo(Path.Combine(gameFolder, managedFile.RelativePath))
                    If Not fileInfo.Exists OrElse fileInfo.LastWriteTimeUtc <> managedFile.LastWriteTimeUtc.ToUniversalTime() Then
                        invalidFiles.Add(managedFile)
                    End If
                End If
            Next
            If Not invalidFiles.Any() Then
                Return Me
            End If
            Dim mcInstallManifest As McInstallManifest = Me.<Clone>$()
            Dim mcComponents As McComponents = Me.Components.<Clone>$()
            Dim invalidFiles4 As IEnumerable(Of McInstallManifest.ManagedFile) = invalidFiles
            Dim <>9__19_ As Func(Of McInstallManifest.ManagedFile, Boolean) = McInstallManifest.<>c.<>9__19_1
            Dim func As Func(Of McInstallManifest.ManagedFile, Boolean) = <>9__19_
            If <>9__19_ Is Nothing Then
                Dim func2 As Func(Of McInstallManifest.ManagedFile, Boolean) = Function(file As McInstallManifest.ManagedFile) file.Component = "OptiFine"
                func = func2
                McInstallManifest.<>c.<>9__19_1 = func2
            End If
            mcComponents.OptiFineEntry = If(invalidFiles4.Any(func), Nothing, Me.Components.OptiFineEntry)
            Dim invalidFiles2 As IEnumerable(Of McInstallManifest.ManagedFile) = invalidFiles
            Dim <>9__19_2 As Func(Of McInstallManifest.ManagedFile, Boolean) = McInstallManifest.<>c.<>9__19_2
            Dim func3 As Func(Of McInstallManifest.ManagedFile, Boolean) = <>9__19_2
            If <>9__19_2 Is Nothing Then
                Dim func4 As Func(Of McInstallManifest.ManagedFile, Boolean) = Function(file As McInstallManifest.ManagedFile) file.Component = "FabricApi"
                func3 = func4
                McInstallManifest.<>c.<>9__19_2 = func4
            End If
            mcComponents.FabricApiVersion = If(invalidFiles2.Any(func3), Nothing, Me.Components.FabricApiVersion)
            Dim invalidFiles3 As IEnumerable(Of McInstallManifest.ManagedFile) = invalidFiles
            Dim <>9__19_3 As Func(Of McInstallManifest.ManagedFile, Boolean) = McInstallManifest.<>c.<>9__19_3
            Dim func5 As Func(Of McInstallManifest.ManagedFile, Boolean) = <>9__19_3
            If <>9__19_3 Is Nothing Then
                Dim func6 As Func(Of McInstallManifest.ManagedFile, Boolean) = Function(file As McInstallManifest.ManagedFile) file.Component = "OptiFabric"
                func5 = func6
                McInstallManifest.<>c.<>9__19_3 = func6
            End If
            mcComponents.OptiFabricVersion = If(invalidFiles3.Any(func5), Nothing, Me.Components.OptiFabricVersion)
            mcInstallManifest.Components = mcComponents
            mcInstallManifest.ManagedFiles = Me.ManagedFiles.Where(Function(file As McInstallManifest.ManagedFile) Not invalidFiles.Contains(file)).ToList()
            mcInstallManifest.LastModifiedUtc = DateTime.UtcNow
            Return mcInstallManifest
        End Function

        <CompilerGenerated()>
        Public Overrides Function ToString() As String
            Dim stringBuilder As StringBuilder = New StringBuilder()
            stringBuilder.Append("McInstallManifest")
            stringBuilder.Append(" { ")
            If Me.PrintMembers(stringBuilder) Then
                stringBuilder.Append(" "c)
            End If
            stringBuilder.Append("}"c)
            Return stringBuilder.ToString()
        End Function

        <CompilerGenerated()>
        Protected Overridable Function PrintMembers(builder As StringBuilder) As Boolean
            RuntimeHelpers.EnsureSufficientExecutionStack()
            builder.Append("SchemaVersion = ")
            builder.Append(Me.SchemaVersion.ToString())
            builder.Append(", Components = ")
            builder.Append(Me.Components)
            builder.Append(", LastModifiedUtc = ")
            builder.Append(Me.LastModifiedUtc.ToString())
            builder.Append(", ManagedFiles = ")
            builder.Append(Me.ManagedFiles)
            Return True
        End Function

        <NullableContext(2)>
        <CompilerGenerated()>
        Public Shared Operator <>(left As McInstallManifest, right As McInstallManifest) As Boolean
            Return Not(left Is right)
        End Operator

        <NullableContext(2)>
        <CompilerGenerated()>
        Public Shared Operator =(left As McInstallManifest, right As McInstallManifest) As Boolean
            Return left Is right OrElse (left IsNot Nothing AndAlso left.Equals(right))
        End Operator

        <CompilerGenerated()>
        Public Overrides Function GetHashCode() As Integer
            Return(((EqualityComparer(Of Type).[Default].GetHashCode(Me.EqualityContract) * -1521134295 + EqualityComparer(Of Integer).[Default].GetHashCode(Me.<SchemaVersion>k__BackingField)) * -1521134295 + EqualityComparer(Of McComponents).[Default].GetHashCode(Me.<Components>k__BackingField)) * -1521134295 + EqualityComparer(Of DateTime).[Default].GetHashCode(Me.<LastModifiedUtc>k__BackingField)) * -1521134295 + EqualityComparer(Of IReadOnlyList(Of McInstallManifest.ManagedFile)).[Default].GetHashCode(Me.<ManagedFiles>k__BackingField)
        End Function

        <NullableContext(2)>
        <CompilerGenerated()>
        Public Overrides Overloads Function Equals(obj As Object) As Boolean
            Return Me.Equals(TryCast(obj, McInstallManifest))
        End Function

        <NullableContext(2)>
        <CompilerGenerated()>
        Public Overridable Overloads Function Equals(other As McInstallManifest) As Boolean Implements System.IEquatable(Of PCLCS.McInstallManifest).Equals
            Return Me Is other OrElse (other IsNot Nothing AndAlso Me.EqualityContract Is other.EqualityContract AndAlso EqualityComparer(Of Integer).[Default].Equals(Me.<SchemaVersion>k__BackingField, other.<SchemaVersion>k__BackingField) AndAlso EqualityComparer(Of McComponents).[Default].Equals(Me.<Components>k__BackingField, other.<Components>k__BackingField) AndAlso EqualityComparer(Of DateTime).[Default].Equals(Me.<LastModifiedUtc>k__BackingField, other.<LastModifiedUtc>k__BackingField) AndAlso EqualityComparer(Of IReadOnlyList(Of McInstallManifest.ManagedFile)).[Default].Equals(Me.<ManagedFiles>k__BackingField, other.<ManagedFiles>k__BackingField))
        End Function

        <CompilerGenerated()>
        Protected Sub New(original As McInstallManifest)
            Me.SchemaVersion = original.<SchemaVersion>k__BackingField
            Me.Components = original.<Components>k__BackingField
            Me.LastModifiedUtc = original.<LastModifiedUtc>k__BackingField
            Me.ManagedFiles = original.<ManagedFiles>k__BackingField
        End Sub

        <CompilerGenerated()>
        Public Sub Deconstruct(<System.Runtime.InteropServices.OutAttribute()> ByRef SchemaVersion As Integer, <System.Runtime.InteropServices.OutAttribute()> ByRef Components As McComponents, <System.Runtime.InteropServices.OutAttribute()> ByRef LastModifiedUtc As DateTime, <System.Runtime.InteropServices.OutAttribute()> ByRef ManagedFiles As IReadOnlyList(Of McInstallManifest.ManagedFile))
            SchemaVersion = Me.SchemaVersion
            Components = Me.Components
            LastModifiedUtc = Me.LastModifiedUtc
            ManagedFiles = Me.ManagedFiles
        End Sub

        <Nullable(0)>
        Public Class ManagedFile
            Implements IEquatable(Of McInstallManifest.ManagedFile)

            Public Sub New(RelativePath As String, Sha1 As String, LastWriteTimeUtc As DateTime, <Nullable(2)> Optional Component As String = Nothing)
                Me.RelativePath = RelativePath
                Me.Sha1 = Sha1
                Me.LastWriteTimeUtc = LastWriteTimeUtc
                Me.Component = Component
                MyBase..ctor()
            End Sub

            <CompilerGenerated()>
            Protected Overridable ReadOnly Property EqualityContract As Type
                <CompilerGenerated()>
                Get
                    Return GetType(McInstallManifest.ManagedFile)
                End Get
            End Property

            Public Property RelativePath As String

            Public Property Sha1 As String

            Public Property LastWriteTimeUtc As DateTime

            <Nullable(2)>
            <JsonProperty(NullValueHandling := 1)>
            Public Property Component As String

            <CompilerGenerated()>
            Public Overrides Function ToString() As String
                Dim stringBuilder As StringBuilder = New StringBuilder()
                stringBuilder.Append("ManagedFile")
                stringBuilder.Append(" { ")
                If Me.PrintMembers(stringBuilder) Then
                    stringBuilder.Append(" "c)
                End If
                stringBuilder.Append("}"c)
                Return stringBuilder.ToString()
            End Function

            <CompilerGenerated()>
            Protected Overridable Function PrintMembers(builder As StringBuilder) As Boolean
                RuntimeHelpers.EnsureSufficientExecutionStack()
                builder.Append("RelativePath = ")
                builder.Append(Me.RelativePath)
                builder.Append(", Sha1 = ")
                builder.Append(Me.Sha1)
                builder.Append(", LastWriteTimeUtc = ")
                builder.Append(Me.LastWriteTimeUtc.ToString())
                builder.Append(", Component = ")
                builder.Append(Me.Component)
                Return True
            End Function

            <NullableContext(2)>
            <CompilerGenerated()>
            Public Shared Operator <>(left As McInstallManifest.ManagedFile, right As McInstallManifest.ManagedFile) As Boolean
                Return Not(left Is right)
            End Operator

            <NullableContext(2)>
            <CompilerGenerated()>
            Public Shared Operator =(left As McInstallManifest.ManagedFile, right As McInstallManifest.ManagedFile) As Boolean
                Return left Is right OrElse (left IsNot Nothing AndAlso left.Equals(right))
            End Operator

            <CompilerGenerated()>
            Public Overrides Function GetHashCode() As Integer
                Return(((EqualityComparer(Of Type).[Default].GetHashCode(Me.EqualityContract) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<RelativePath>k__BackingField)) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<Sha1>k__BackingField)) * -1521134295 + EqualityComparer(Of DateTime).[Default].GetHashCode(Me.<LastWriteTimeUtc>k__BackingField)) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<Component>k__BackingField)
            End Function

            <NullableContext(2)>
            <CompilerGenerated()>
            Public Overrides Overloads Function Equals(obj As Object) As Boolean
                Return Me.Equals(TryCast(obj, McInstallManifest.ManagedFile))
            End Function

            <NullableContext(2)>
            <CompilerGenerated()>
            Public Overridable Overloads Function Equals(other As McInstallManifest.ManagedFile) As Boolean Implements System.IEquatable(Of PCLCS.McInstallManifest.ManagedFile).Equals
                Return Me Is other OrElse (other IsNot Nothing AndAlso Me.EqualityContract Is other.EqualityContract AndAlso EqualityComparer(Of String).[Default].Equals(Me.<RelativePath>k__BackingField, other.<RelativePath>k__BackingField) AndAlso EqualityComparer(Of String).[Default].Equals(Me.<Sha1>k__BackingField, other.<Sha1>k__BackingField) AndAlso EqualityComparer(Of DateTime).[Default].Equals(Me.<LastWriteTimeUtc>k__BackingField, other.<LastWriteTimeUtc>k__BackingField) AndAlso EqualityComparer(Of String).[Default].Equals(Me.<Component>k__BackingField, other.<Component>k__BackingField))
            End Function

            <CompilerGenerated()>
            Protected Sub New(original As McInstallManifest.ManagedFile)
                Me.RelativePath = original.<RelativePath>k__BackingField
                Me.Sha1 = original.<Sha1>k__BackingField
                Me.LastWriteTimeUtc = original.<LastWriteTimeUtc>k__BackingField
                Me.Component = original.<Component>k__BackingField
            End Sub

            <CompilerGenerated()>
            Public Sub Deconstruct(<System.Runtime.InteropServices.OutAttribute()> ByRef RelativePath As String, <System.Runtime.InteropServices.OutAttribute()> ByRef Sha1 As String, <System.Runtime.InteropServices.OutAttribute()> ByRef LastWriteTimeUtc As DateTime, <Nullable(2)> <System.Runtime.InteropServices.OutAttribute()> ByRef Component As String)
                RelativePath = Me.RelativePath
                Sha1 = Me.Sha1
                LastWriteTimeUtc = Me.LastWriteTimeUtc
                Component = Me.Component
            End Sub
        End Class
    End Class
End Namespace
