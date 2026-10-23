Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace MeloongCore
    Public Class ProgressProvider
        <TupleElementNames(New String() { "actual", "skiped" })>
        <Nullable(New Byte() { 2, 0 })>
        <TupleElementNames(New String() { "actual", "skiped" })>
        <Nullable(New Byte() { 2, 0 })>
        Public Event ProgressChanged As Action(Of ValueTuple(Of Double, Double))

        <Nullable(2)>
        <NullableContext(2)>
        <Nullable(2)>
        Private Event ChildProgressChanged As Action

        Private Sub InvokeProgressChanged()
            Dim progressChanged As Action(Of ValueTuple(Of Double, Double)) = Me.ProgressChanged
            If progressChanged IsNot Nothing Then
                progressChanged(Me.GetRaw())
            End If
            Dim childProgressChanged As Action = Me.ChildProgressChanged
            If childProgressChanged Is Nothing Then
                Return
            End If
            childProgressChanged()
        End Sub

        Private ReadOnly Property progressSum As Double
            Get
                Return Me.progressParts.Item1 + Me.progressParts.Item2 + Me.progressParts.Item3
            End Get
        End Property

        Private Function _Set(value As Double, skiped As Boolean) As Boolean
            value = value.Clamp(0.0, 1.0)
            Dim num As Double = value - Me.progressSum
            If num < 0.0 Then
                Me.progressParts = New ValueTuple(Of Double, Double, Double)(Me.progressParts.Item1 * value / Me.progressSum, Me.progressParts.Item2 * value / Me.progressSum, Me.progressParts.Item3 * value / Me.progressSum)
            ElseIf skiped Then
                Me.progressParts.Item2 = Me.progressParts.Item2 + num
            Else
                Me.progressParts.Item1 = Me.progressParts.Item1 + num
            End If
            Return num <> 0.0
        End Function

        Public Sub [Set](value As Double, Optional skiped As Boolean = False, Optional action As ProgressProvider.ChildrenAction = ProgressProvider.ChildrenAction.Finished)
            Dim flag2 As Boolean
            SyncLock Me
                flag2 = Me._Set(value, skiped)
                flag2 = flag2 Or Me.AssumeChildrenAs(action)
            End SyncLock
            If flag2 Then
                Me.InvokeProgressChanged()
            End If
        End Sub

        Public Sub Add(value As Double, Optional skiped As Boolean = False, Optional action As ProgressProvider.ChildrenAction = ProgressProvider.ChildrenAction.Finished)
            Dim flag2 As Boolean
            SyncLock Me
                flag2 = Me._Set(value + Me.progressSum, skiped)
                flag2 = flag2 Or Me.AssumeChildrenAs(action)
            End SyncLock
            If flag2 Then
                Me.InvokeProgressChanged()
            End If
        End Sub

        Public Sub Finish(Optional action As ProgressProvider.ChildrenAction = ProgressProvider.ChildrenAction.Finished)
            Me.[Set](1.0, False, action)
        End Sub

        Public Sub Skip(Optional action As ProgressProvider.ChildrenAction = ProgressProvider.ChildrenAction.Skiped)
            Me.[Set](1.0, True, action)
        End Sub

        Public Sub Reset()
            Dim flag2 As Boolean
            SyncLock Me
                flag2 = Me.progressSum > 0.0
                Me.progressParts = New ValueTuple(Of Double, Double, Double)(0.0, 0.0, 0.0)
                Me.childrens.ForEach(Sub(<TupleElementNames(New String() { "percentage", "child" })> <Nullable(New Byte() { 0, 1 })> c As ValueTuple(Of Double, ProgressProvider))
                    RemoveHandler c.Item2.ChildProgressChanged, AddressOf Me.InvokeProgressChanged
                End Sub)
                Me.childrens.Clear()
            End SyncLock
            If flag2 Then
                Me.InvokeProgressChanged()
            End If
        End Sub

        <NullableContext(1)>
        Public Function SplitTo(value As Double) As ProgressProvider
            Dim progressProvider As ProgressProvider
            SyncLock Me
                progressProvider = Me.SplitBy(New Double() { value - Me.progressSum }).First()
            End SyncLock
            Return progressProvider
        End Function

        <NullableContext(1)>
        Public Function SplitBy(ParamArray percentages As Double()) As List(Of ProgressProvider)
            Dim list As List(Of ProgressProvider)
            SyncLock Me
                Dim <>9__18_ As Func(Of Double, Boolean) = ProgressProvider.<>c.<>9__18_0
                Dim func As Func(Of Double, Boolean) = <>9__18_
                If <>9__18_ Is Nothing Then
                    Dim func2 As Func(Of Double, Boolean) = Function(p As Double) p <= 0.0
                    func = func2
                    ProgressProvider.<>c.<>9__18_0 = func2
                End If
                If percentages.Any(func) Then
                    Throw New ArgumentException("子项进度必须为正数且")
                End If
                Me.progressParts.Item3 = Me.progressParts.Item3 + ((percentages.Sum() + Me.progressSum).Clamp(0.0, 1.0) - Me.progressSum)
                list = percentages.[Select](Function(percentage As Double)
                    Dim progressProvider As ProgressProvider = New ProgressProvider()
                    AddHandler progressProvider.ChildProgressChanged, AddressOf Me.InvokeProgressChanged
                    Me.childrens.Add(New ValueTuple(Of Double, ProgressProvider)(percentage, progressProvider))
                    Return progressProvider
                End Function).ToList()
            End SyncLock
            Return list
        End Function

        Private Function AssumeChildrenAs(action As ProgressProvider.ChildrenAction) As Boolean
            Dim CS$<>8__locals1 As ProgressProvider.<>c__DisplayClass20_0 = New ProgressProvider.<>c__DisplayClass20_0()
            CS$<>8__locals1.<>4__this = Me
            CS$<>8__locals1.action = action
            If CS$<>8__locals1.action = ProgressProvider.ChildrenAction.None Then
                Return False
            End If
            Dim changed As Boolean
            SyncLock Me
                CS$<>8__locals1.changed = False
                Dim enumerable As IEnumerable(Of ValueTuple(Of Double, ProgressProvider)) = Me.childrens
                Dim <>9__20_ As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = ProgressProvider.<>c.<>9__20_0
                Dim func As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = <>9__20_
                If <>9__20_ Is Nothing Then
                    Dim func2 As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = Function(<TupleElementNames(New String() { "percentage", "child" })> c As ValueTuple(Of Double, ProgressProvider)) c.Item1 > 0.0
                    func = func2
                    ProgressProvider.<>c.<>9__20_0 = func2
                End If
                If enumerable.Any(func) Then
                    Dim CS$<>8__locals2 As ProgressProvider.<>c__DisplayClass20_0 = CS$<>8__locals1
                    Dim item As Double = Me.progressParts.Item3
                    Dim enumerable2 As IEnumerable(Of ValueTuple(Of Double, ProgressProvider)) = Me.childrens
                    Dim <>9__20_2 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = ProgressProvider.<>c.<>9__20_1
                    Dim func3 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = <>9__20_2
                    If <>9__20_2 Is Nothing Then
                        Dim func4 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = Function(<TupleElementNames(New String() { "percentage", "child" })> c As ValueTuple(Of Double, ProgressProvider)) c.Item1
                        func3 = func4
                        ProgressProvider.<>c.<>9__20_1 = func4
                    End If
                    CS$<>8__locals2.mult = item / enumerable2.Sum(func3)
                    Me.childrens.ForEach(Sub(<TupleElementNames(New String() { "percentage", "child" })> <Nullable(New Byte() { 0, 1 })> c As ValueTuple(Of Double, ProgressProvider))
                        Dim raw As ValueTuple(Of Double, Double) = c.Item2.GetRaw()
                        Dim item2 As Double = raw.Item1
                        Dim item3 As Double = raw.Item2
                        Dim <>4__this As ProgressProvider = CS$<>8__locals1.<>4__this
                        <>4__this.progressParts.Item1 = <>4__this.progressParts.Item1 + CS$<>8__locals1.mult * c.Item1 * If((CS$<>8__locals1.action = ProgressProvider.ChildrenAction.Finished), (1.0 - item3), item2)
                        Dim <>4__this2 As ProgressProvider = CS$<>8__locals1.<>4__this
                        <>4__this2.progressParts.Item2 = <>4__this2.progressParts.Item2 + CS$<>8__locals1.mult * c.Item1 * If((CS$<>8__locals1.action = ProgressProvider.ChildrenAction.Skiped), (1.0 - item2), item3)
                        RemoveHandler c.Item2.ChildProgressChanged, AddressOf CS$<>8__locals1.<>4__this.InvokeProgressChanged
                        If item2 + item3 < 0.9999999 Then
                            CS$<>8__locals1.changed = True
                        End If
                    End Sub)
                End If
                Me.progressParts.Item3 = 0.0
                Me.childrens.Clear()
                changed = CS$<>8__locals1.changed
            End SyncLock
            Return changed
        End Function

        Public Function GetRaw() As <TupleElementNames(New String() { "actual", "skiped" })> ValueTuple(Of Double, Double)
            Dim current As ValueTuple(Of Double, Double)
            SyncLock Me
                Dim CS$<>8__locals1 As ProgressProvider.<>c__DisplayClass21_0 = New ProgressProvider.<>c__DisplayClass21_0()
                CS$<>8__locals1.current = New ValueTuple(Of Double, Double)(Me.progressParts.Item1, Me.progressParts.Item2)
                Dim enumerable As IEnumerable(Of ValueTuple(Of Double, ProgressProvider)) = Me.childrens
                Dim <>9__21_ As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = ProgressProvider.<>c.<>9__21_0
                Dim func As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = <>9__21_
                If <>9__21_ Is Nothing Then
                    Dim func2 As Func(Of ValueTuple(Of Double, ProgressProvider), Boolean) = Function(<TupleElementNames(New String() { "percentage", "child" })> c As ValueTuple(Of Double, ProgressProvider)) c.Item1 > 0.0
                    func = func2
                    ProgressProvider.<>c.<>9__21_0 = func2
                End If
                If enumerable.Any(func) Then
                    Dim CS$<>8__locals2 As ProgressProvider.<>c__DisplayClass21_0 = CS$<>8__locals1
                    Dim item As Double = Me.progressParts.Item3
                    Dim enumerable2 As IEnumerable(Of ValueTuple(Of Double, ProgressProvider)) = Me.childrens
                    Dim <>9__21_2 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = ProgressProvider.<>c.<>9__21_1
                    Dim func3 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = <>9__21_2
                    If <>9__21_2 Is Nothing Then
                        Dim func4 As Func(Of ValueTuple(Of Double, ProgressProvider), Double) = Function(<TupleElementNames(New String() { "percentage", "child" })> c As ValueTuple(Of Double, ProgressProvider)) c.Item1
                        func3 = func4
                        ProgressProvider.<>c.<>9__21_1 = func4
                    End If
                    CS$<>8__locals2.mult = item / enumerable2.Sum(func3)
                    Me.childrens.ForEach(Sub(<TupleElementNames(New String() { "percentage", "child" })> <Nullable(New Byte() { 0, 1 })> c As ValueTuple(Of Double, ProgressProvider))
                        Dim raw As ValueTuple(Of Double, Double) = c.Item2.GetRaw()
                        Dim item2 As Double = raw.Item1
                        Dim item3 As Double = raw.Item2
                        CS$<>8__locals1.current.Item1 = CS$<>8__locals1.current.Item1 + CS$<>8__locals1.mult * c.Item1 * item2
                        CS$<>8__locals1.current.Item2 = CS$<>8__locals1.current.Item2 + CS$<>8__locals1.mult * c.Item1 * item3
                    End Sub)
                End If
                current = CS$<>8__locals1.current
            End SyncLock
            Return current
        End Function

        Public ReadOnly Property IsFinished As Boolean
            Get
                Return Me.GetRaw().Item1 + Me.GetRaw().Item2 > 0.9999999
            End Get
        End Property

        Public Function GetIncreasing() As Double
            Dim num As Double
            SyncLock Me
                Dim raw As ValueTuple(Of Double, Double) = Me.GetRaw()
                Dim valueTuple As ValueTuple(Of Double, Double) = raw
                Dim valueTuple2 As ValueTuple(Of Double, Double) = Me.observedProgress
                If valueTuple.Item1 = valueTuple2.Item1 AndAlso valueTuple.Item2 = valueTuple2.Item2 Then
                    num = Me.incrementProgress
                Else
                    If Me.IsFinished Then
                        Me.incrementProgress = 1.0
                    ElseIf raw.Item1 >= Me.observedProgress.Item1 Then
                        Me.incrementProgress += (1.0 - Me.incrementProgress) * (raw.Item1 - Me.observedProgress.Item1) / (1.0 - Me.observedProgress.Item1 - raw.Item2)
                    End If
                    Me.observedProgress = raw
                    num = Me.incrementProgress
                End If
            End SyncLock
            Return num
        End Function

        <TupleElementNames(New String() { "actual", "skiped", "splited" })>
        Private progressParts As ValueTuple(Of Double, Double, Double) = New ValueTuple(Of Double, Double, Double)(0.0, 0.0, 0.0)

        <TupleElementNames(New String() { "percentage", "child" })>
        <Nullable(New Byte() { 1, 0, 1 })>
        Private childrens As List(Of ValueTuple(Of Double, ProgressProvider)) = New List(Of ValueTuple(Of Double, ProgressProvider))()

        <TupleElementNames(New String() { "actual", "skiped" })>
        Private observedProgress As ValueTuple(Of Double, Double) = New ValueTuple(Of Double, Double)(0.0, 0.0)

        Private incrementProgress As Double

        Public Enum ChildrenAction
            None
            Finished
            Skiped
        End Enum
    End Class
End Namespace
