Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports MeloongCore
Imports MeloongCore.Extensions

Namespace PCLCS
    <NullableContext(1)>
    <Nullable(0)>
    Public Class WikiEntry
        Public Overrides Function ToString() As String
            Return String.Format("{0}&{1}|{2}|{3}，浏览量 {4}", New Object() { Me.Slugs.GetOrDefault(ResourcePlatforms.CurseForge, Nothing), Me.Slugs.GetOrDefault(ResourcePlatforms.Modrinth, Nothing), Me.Id, Me.ChineseName, Me.Popularity })
        End Function

        Public Id As Integer

        <Nullable(2)>
        Public ChineseName As String

        Public Slugs As Dictionary(Of ResourcePlatforms, String) = New Dictionary(Of ResourcePlatforms, String)()

        Public Popularity As Integer

        Public Shared All As Lazy(Of List(Of WikiEntry)) = New Lazy(Of List(Of WikiEntry))(Function()
            Dim list As List(Of String) = FileUtils.ReadAsLines("Resource\WikiEntries.txt", False, Nothing, GetType(WikiEntry)).ToList()
            Dim queue As Queue(Of Integer) = New Queue(Of Integer)()
            Dim num As Integer = 0
            While True
                Dim num2 As Integer = num
                Dim list2 As List(Of String) = list
                If num2 >= list2(list2.Count - 1).Length Then
                    Exit For
                End If
                Dim queue2 As Queue(Of Integer) = queue
                Dim list3 As List(Of String) = list
                queue2.Enqueue(Integer.Parse(list3(list3.Count - 1).Substring(num, 3).ConvertRadix(86, 10)))
                num += 3
            End While
            list.RemoveAt(list.Count - 1)
            Dim list4 As List(Of WikiEntry) = New List(Of WikiEntry)()
            Dim num3 As Integer = 0
            For Each text As String In list
                num3 += 1
                If Not(text = "") Then
                    Dim num4 As Integer = queue.Dequeue()
                    For Each text2 As String In text.Split(New Char() { "¨"c })
                        Dim wikiEntry As WikiEntry = New WikiEntry()
                        Dim array2 As String() = text2.Split(New Char() { "|"c })
                        Dim text3 As String = array2(0)
                        If text3.StartsWithF("@", False) Then
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, text3.Replace("@", ""))
                        ElseIf text3.EndsWithF("@", False) Then
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, text3.TrimEnd(New Char() { "@"c }))
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, wikiEntry.Slugs(ResourcePlatforms.CurseForge))
                        ElseIf text3.Contains("@") Then
                            Dim array3 As String() = text3.Split(New Char() { "@"c })
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, array3(0))
                            wikiEntry.Slugs.Add(ResourcePlatforms.Modrinth, array3(1))
                        Else
                            wikiEntry.Slugs.Add(ResourcePlatforms.CurseForge, text3)
                        End If
                        wikiEntry.Id = num3
                        wikiEntry.Popularity = num4
                        If array2.Length >= 2 Then
                            Dim wikiEntry2 As WikiEntry = wikiEntry
                            Dim array4 As String() = array2
                            wikiEntry2.ChineseName = array4(array4.Length - 1)
                            If wikiEntry.ChineseName.Contains("*") Then
                                Dim text4 As String = wikiEntry.Slugs.Values.First().Replace("-", " ").Capitalize()
                                wikiEntry.ChineseName = wikiEntry.ChineseName.Replace("*", " (" + text4 + ")")
                            End If
                        End If
                        list4.Add(wikiEntry)
                    Next
                End If
            Next
            Return list4
        End Function)
    End Class
End Namespace
