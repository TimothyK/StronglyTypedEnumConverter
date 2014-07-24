

Imports System.Collections.Generic
Imports System.Linq

Class CowboyType
    Private Sub New()
    End Sub

    Public Shared ReadOnly Good As New CowboyType()
    Public Shared ReadOnly Bad As New CowboyType()
    Public Shared ReadOnly Ugly As New CowboyType()

    Public Shared Iterator Function All() As IEnumerable(Of CowboyType)
        Yield Good
        Yield Bad
        Yield Ugly
    End Function

    Public Overrides Function ToString() As String
        Dim map = New Dictionary(Of CowboyType, String)() From {
            {Good, "Good"},
            {Bad, "Bad"},
            {Ugly, "Ugly"}
        }

        Return map(Me)
    End Function

    Public Shared Function FromString(value As String) As CowboyType
        If value Is Nothing Then
            Throw New ArgumentNullException("value")
        End If

        Dim result = All().FirstOrDefault(Function(x) x.ToString() = value)
        If result IsNot Nothing Then
            Return result
        End If

        Throw New ArgumentOutOfRangeException("value", value, "Invalid CowboyType")
    End Function

    Public Shared Narrowing Operator CType(value As CowboyType) As Integer
        Dim map = New Dictionary(Of CowboyType, Integer)() From {
            {Good, 0},
            {Bad, 1},
            {Ugly, 2}
        }

        Return map(value)
    End Operator

    Public Shared Narrowing Operator CType(value As Integer) As CowboyType
        Dim result = All().FirstOrDefault(Function(x) CInt(x) = value)
        If result IsNot Nothing Then
            Return result
        End If

        Throw New InvalidCastException("The value " & value & " is not a valid CowboyType")
    End Operator

End Class


