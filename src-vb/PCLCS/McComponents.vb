Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports MeloongCore.Extensions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace PCLCS
    <NullableContext(2)>
    <Nullable(0)>
    Public Class McComponents
        Implements IEquatable(Of McComponents)

        <JsonConstructor()>
        Public Sub New(<Nullable(1)> MinecraftVersion As String, <Nullable(1)> ModLoaderVersions As Dictionary(Of ModLoaders, String), Optional OptiFineEntry As JObject = Nothing, Optional FabricApiVersion As String = Nothing, Optional OptiFabricVersion As String = Nothing)
            Me.MinecraftVersion = MinecraftVersion
            Me.ModLoaderVersions = ModLoaderVersions
            Me.OptiFineEntry = OptiFineEntry
            Me.FabricApiVersion = FabricApiVersion
            Me.OptiFabricVersion = OptiFabricVersion
            MyBase..ctor()
        End Sub

        <Nullable(1)>
        <CompilerGenerated()>
        Protected Overridable ReadOnly Property EqualityContract As Type
            <NullableContext(1)>
            <CompilerGenerated()>
            Get
                Return GetType(McComponents)
            End Get
        End Property

        <Nullable(1)>
        Public Property MinecraftVersion As String

        <Nullable(1)>
        Public Property ModLoaderVersions As Dictionary(Of ModLoaders, String)

        <JsonProperty(NullValueHandling := 1)>
        Public Property OptiFineEntry As JObject

        <JsonProperty(NullValueHandling := 1)>
        Public Property FabricApiVersion As String

        <JsonProperty(NullValueHandling := 1)>
        Public Property OptiFabricVersion As String

        <JsonIgnore()>
        Public ReadOnly Property OptiFineVersion As String
            Get
                Dim optiFineEntry As JObject = Me.OptiFineEntry
                If optiFineEntry Is Nothing Then
                    Return Nothing
                End If
                Dim jtoken As JToken = optiFineEntry("InstanceName")
                If jtoken Is Nothing Then
                    Return Nothing
                End If
                Dim text As String = Extensions.Value(Of String)(jtoken)
                If text Is Nothing Then
                    Return Nothing
                End If
                Return text.AfterLast("-OptiFine_", False)
            End Get
        End Property

        Public Sub New(<Nullable(1)> minecraftVersion As String, optiFineEntry As JObject, forgeVersion As String, neoForgeVersion As String, fabricVersion As String, fabricApiVersion As String, optiFabricVersion As String, liteLoaderVersion As String)
            Me.New(minecraftVersion, New Dictionary(Of ModLoaders, String)(), optiFineEntry, fabricApiVersion, optiFabricVersion)
            If forgeVersion IsNot Nothing Then
                Me.ModLoaderVersions(ModLoaders.Forge) = forgeVersion
            End If
            If neoForgeVersion IsNot Nothing Then
                Me.ModLoaderVersions(ModLoaders.NeoForge) = neoForgeVersion
            End If
            If fabricVersion IsNot Nothing Then
                Me.ModLoaderVersions(ModLoaders.Fabric) = fabricVersion
            End If
            If liteLoaderVersion IsNot Nothing Then
                Me.ModLoaderVersions(ModLoaders.LiteLoader) = liteLoaderVersion
            End If
        End Sub

        <NullableContext(1)>
        Public Function IsEquivalentTo(other As McComponents) As Boolean
            Return Me.MinecraftVersion = other.MinecraftVersion AndAlso Me.OptiFineVersion = other.OptiFineVersion AndAlso Me.FabricApiVersion = other.FabricApiVersion AndAlso Me.OptiFabricVersion = other.OptiFabricVersion AndAlso Me.ModLoaderVersions.Count = other.ModLoaderVersions.Count AndAlso Me.ModLoaderVersions.All(Function(entry As KeyValuePair(Of ModLoaders, String))
                Dim text As String
                Return other.ModLoaderVersions.TryGetValue(entry.Key, text) AndAlso text = entry.Value
            End Function)
        End Function

        <NullableContext(1)>
        <CompilerGenerated()>
        Public Overrides Function ToString() As String
            Dim stringBuilder As StringBuilder = New StringBuilder()
            stringBuilder.Append("McComponents")
            stringBuilder.Append(" { ")
            If Me.PrintMembers(stringBuilder) Then
                stringBuilder.Append(" "c)
            End If
            stringBuilder.Append("}"c)
            Return stringBuilder.ToString()
        End Function

        <NullableContext(1)>
        <CompilerGenerated()>
        Protected Overridable Function PrintMembers(builder As StringBuilder) As Boolean
            RuntimeHelpers.EnsureSufficientExecutionStack()
            builder.Append("MinecraftVersion = ")
            builder.Append(Me.MinecraftVersion)
            builder.Append(", ModLoaderVersions = ")
            builder.Append(Me.ModLoaderVersions)
            builder.Append(", OptiFineEntry = ")
            builder.Append(Me.OptiFineEntry)
            builder.Append(", FabricApiVersion = ")
            builder.Append(Me.FabricApiVersion)
            builder.Append(", OptiFabricVersion = ")
            builder.Append(Me.OptiFabricVersion)
            builder.Append(", OptiFineVersion = ")
            builder.Append(Me.OptiFineVersion)
            Return True
        End Function

        <CompilerGenerated()>
        Public Shared Operator <>(left As McComponents, right As McComponents) As Boolean
            Return Not(left Is right)
        End Operator

        <CompilerGenerated()>
        Public Shared Operator =(left As McComponents, right As McComponents) As Boolean
            Return left Is right OrElse (left IsNot Nothing AndAlso left.Equals(right))
        End Operator

        <CompilerGenerated()>
        Public Overrides Function GetHashCode() As Integer
            Return((((EqualityComparer(Of Type).[Default].GetHashCode(Me.EqualityContract) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<MinecraftVersion>k__BackingField)) * -1521134295 + EqualityComparer(Of Dictionary(Of ModLoaders, String)).[Default].GetHashCode(Me.<ModLoaderVersions>k__BackingField)) * -1521134295 + EqualityComparer(Of JObject).[Default].GetHashCode(Me.<OptiFineEntry>k__BackingField)) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<FabricApiVersion>k__BackingField)) * -1521134295 + EqualityComparer(Of String).[Default].GetHashCode(Me.<OptiFabricVersion>k__BackingField)
        End Function

        <CompilerGenerated()>
        Public Overrides Overloads Function Equals(obj As Object) As Boolean
            Return Me.Equals(TryCast(obj, McComponents))
        End Function

        <CompilerGenerated()>
        Public Overridable Overloads Function Equals(other As McComponents) As Boolean Implements System.IEquatable(Of PCLCS.McComponents).Equals
            Return Me Is other OrElse (other IsNot Nothing AndAlso Me.EqualityContract Is other.EqualityContract AndAlso EqualityComparer(Of String).[Default].Equals(Me.<MinecraftVersion>k__BackingField, other.<MinecraftVersion>k__BackingField) AndAlso EqualityComparer(Of Dictionary(Of ModLoaders, String)).[Default].Equals(Me.<ModLoaderVersions>k__BackingField, other.<ModLoaderVersions>k__BackingField) AndAlso EqualityComparer(Of JObject).[Default].Equals(Me.<OptiFineEntry>k__BackingField, other.<OptiFineEntry>k__BackingField) AndAlso EqualityComparer(Of String).[Default].Equals(Me.<FabricApiVersion>k__BackingField, other.<FabricApiVersion>k__BackingField) AndAlso EqualityComparer(Of String).[Default].Equals(Me.<OptiFabricVersion>k__BackingField, other.<OptiFabricVersion>k__BackingField))
        End Function

        <CompilerGenerated()>
        Protected Sub New(<Nullable(1)> original As McComponents)
            Me.MinecraftVersion = original.<MinecraftVersion>k__BackingField
            Me.ModLoaderVersions = original.<ModLoaderVersions>k__BackingField
            Me.OptiFineEntry = original.<OptiFineEntry>k__BackingField
            Me.FabricApiVersion = original.<FabricApiVersion>k__BackingField
            Me.OptiFabricVersion = original.<OptiFabricVersion>k__BackingField
        End Sub

        <CompilerGenerated()>
        Public Sub Deconstruct(<Nullable(1)> <System.Runtime.InteropServices.OutAttribute()> ByRef MinecraftVersion As String, <Nullable(1)> <System.Runtime.InteropServices.OutAttribute()> ByRef ModLoaderVersions As Dictionary(Of ModLoaders, String), <System.Runtime.InteropServices.OutAttribute()> ByRef OptiFineEntry As JObject, <System.Runtime.InteropServices.OutAttribute()> ByRef FabricApiVersion As String, <System.Runtime.InteropServices.OutAttribute()> ByRef OptiFabricVersion As String)
            MinecraftVersion = Me.MinecraftVersion
            ModLoaderVersions = Me.ModLoaderVersions
            OptiFineEntry = Me.OptiFineEntry
            FabricApiVersion = Me.FabricApiVersion
            OptiFabricVersion = Me.OptiFabricVersion
        End Sub
    End Class
End Namespace
