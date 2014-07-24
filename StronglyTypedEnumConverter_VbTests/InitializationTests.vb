
Imports Microsoft.VisualStudio.TestTools.UnitTesting


<TestClass> _
Public Class InitializationTests
    <TestMethod> _
    Public Sub Converter_Initialize_ObjectCreated()
        Dim converter = New Converter()
        Assert.IsNotNull(converter)
    End Sub


    <TestMethod> _
    Public Sub Converter_GarbageIn_ThrowsArgumentException()
        Dim converter = New Converter()
        Try
            converter.Convert("End Enum " & vbNewLine & "Public Enum garbage ")
        Catch ex As ArgumentException
            Console.WriteLine(ex)
            Return
        End Try

        Assert.Fail("Expected exception did not occur")
    End Sub

End Class


