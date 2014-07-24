
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> _
    Public Class CowboyTypeTests
    <TestMethod> _
    Public Sub CowboyType_FromInvalidString_ThrowArgRange()
        Try
            CowboyType.FromString("Jolly")
        Catch ex As ArgumentOutOfRangeException
            Console.WriteLine(ex.ToString())
            Return
        End Try

        Assert.Fail("Expected exception did not occur")

    End Sub

    <TestMethod> _
    Public Sub CowboyType_FromNullString_ThrowArgNull()
        Try
            CowboyType.FromString(Nothing)
        Catch ex As ArgumentNullException
            Console.WriteLine(ex.ToString())
            Return
        End Try

        Assert.Fail("Expected exception did not occur")

    End Sub

    <TestMethod> _
    Public Sub CowboyType_All_ReturnsAllThreeValues()
        Dim types = CowboyType.All().ToArray()

        Assert.IsTrue(types.Contains(CowboyType.Good))
        Assert.IsTrue(types.Contains(CowboyType.Bad))
        Assert.IsTrue(types.Contains(CowboyType.Ugly))
    End Sub

    <TestMethod> _
    Public Sub CowboyType_StringValuesRoundTrip_ReturnsOriginalValue()
        For Each type In CowboyType.All()
            Assert.AreSame(type, CowboyType.FromString(type.ToString()), type.ToString() & " did not round trip successfully")
        Next
    End Sub

    <TestMethod> _
    Public Sub CowboyType_CastToInt_ReturnsExpected()
        Assert.AreEqual(0, CInt(CowboyType.Good))
        Assert.AreEqual(1, CInt(CowboyType.Bad))
        Assert.AreEqual(2, CInt(CowboyType.Ugly))
    End Sub

    <TestMethod> _
    Public Sub CowboyType_CastFromInt_ReturnsExpected()
        Assert.AreEqual(CowboyType.Good, CType(0, CowboyType))
        Assert.AreEqual(CowboyType.Bad, CType(1, CowboyType))
        Assert.AreEqual(CowboyType.Ugly, CType(2, CowboyType))
    End Sub

    <TestMethod> _
    Public Sub CowboyType_FromInvalidInt_ThrowInvalidCast()
        Try
            Dim dummy = CType(4, CowboyType)
        Catch ex As InvalidCastException
            Console.WriteLine(ex.ToString())
            Return
        End Try

        Assert.Fail("Expected exception did not occur")

    End Sub

End Class

