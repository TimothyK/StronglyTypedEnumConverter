
Imports System.Reflection
Imports System.CodeDom.Compiler

<TestClass>
Public Class BasicEnumTests
    Private Shared _type As Type

    <ClassInitialize>
    Public Shared Sub ClassInit(context As TestContext)
        _type = ConvertBasicCowboyTypeEnum()
    End Sub

    Private Const CowboyTypeEnumDef As String = "Enum CowboyType" & vbNewLine _
                                                & "    Good" & vbNewLine _
                                                & "    Bad" & vbNewLine _
                                                & "    Ugly" & vbNewLine _
                                                & "End Enum"

    Private Shared Function ConvertBasicCowboyTypeEnum() As Type
        Dim converter = New Converter()
        Dim sourceCode = converter.Convert(CowboyTypeEnumDef)
        Console.WriteLine(sourceCode)

        Dim assembly = CompileCode(sourceCode)

        Dim type = assembly _
            .GetTypes() _
            .SingleOrDefault(Function(t) t.Name = "CowboyType")
        Return type
    End Function

    Private Shared Function CompileCode(sourceCode As String) As Assembly
        Dim compiler = New Microsoft.VisualBasic.VBCodeProvider()
        Dim parameters = New CompilerParameters() With { _
            .GenerateInMemory = True, _
            .GenerateExecutable = False _
        }
        parameters.ReferencedAssemblies.Add("System.dll")
        parameters.ReferencedAssemblies.Add("System.Core.dll")

        Dim compilerOut = compiler.CompileAssemblyFromSource(parameters, sourceCode)

        If compilerOut.Errors.Count = 0 Then
            Return compilerOut.CompiledAssembly
        End If

        For Each compilerError In compilerOut.Errors
            Console.Error.WriteLine(compilerError.ToString())
        Next
        Throw New ApplicationException("Could Not Compile Code")
    End Function

    ''' <summary>
    ''' Returns the public static readonly fields that represent the Enum members
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function GetEnumMembers() As IEnumerable(Of FieldInfo)
        Return _type.GetFields(BindingFlags.Static Or BindingFlags.Public).Where(Function(f) f.IsInitOnly)
    End Function

    Private Shared Function GetEnumValues() As Object()
        Return GetEnumMembers().Select(Function(f) f.GetValue(Nothing)).Where(Function(x) x IsNot Nothing).ToArray()
    End Function

    <TestMethod> _
    Public Sub Class_SameNameAsEnum()
        Assert.AreEqual("CowboyType", _type.Name)
    End Sub

    <TestMethod> _
    Public Sub Class_HasPrivateConstructor()
        Assert.IsTrue(_type.GetConstructors(BindingFlags.NonPublic Or BindingFlags.Instance).Any())
    End Sub

    <TestMethod> _
    Public Sub Class_HasNoPublicConstructor()
        Assert.IsFalse(_type.GetConstructors(BindingFlags.Public Or BindingFlags.Instance).Any())
    End Sub

    <TestMethod> _
    Public Sub Members_HasThreeStaticReadOnlyFields()
        Dim fieldNames = GetEnumMembers().Select(Function(f) f.Name).ToArray()

        Assert.IsTrue(fieldNames.Contains("Good"))
        Assert.IsTrue(fieldNames.Contains("Bad"))
        Assert.IsTrue(fieldNames.Contains("Ugly"))
    End Sub

    <TestMethod> _
    Public Sub Members_HaveUniqueValues()
        Dim values = GetEnumValues()

        Assert.AreEqual(values.Length, 3)
        Assert.AreEqual(values.Distinct().Count(), 3)
    End Sub

    <TestMethod> _
    Public Sub All_MethodExists()
        Dim hasAllMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Any(Function(f) f.Name = "All")

        Assert.IsTrue(hasAllMethod)
    End Sub

    <TestMethod> _
    Public Sub All_ReturnsAllValues()
        Dim expected = GetEnumValues()

        Dim allMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Single(Function(f) f.Name = "All")

        Dim actual = DirectCast(allMethod.Invoke(Nothing, Nothing), IEnumerable(Of Object)).ToArray()

        Assert.AreEqual(expected.Length, actual.Length)
        For Each item In expected
            Assert.IsTrue(actual.Contains(item))
        Next
    End Sub

    <TestMethod> _
    Public Sub ToString_ReturnsExpected()
        Dim fields = GetEnumMembers().ToArray()

        Dim fieldNamesAndValues = fields.ToDictionary(Function(f) f.Name, Function(f) f.GetValue(Nothing))
        For Each kvp In fieldNamesAndValues
            Assert.AreEqual(kvp.Key, kvp.Value.ToString())
        Next
    End Sub

    <TestMethod> _
    Public Sub FromString_MethodExists()
        Dim hasFromStringMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Any(Function(f) f.Name = "FromString")

        Assert.IsTrue(hasFromStringMethod)
    End Sub

    <TestMethod> _
    Public Sub FromString_ValidInputs_ReturnsValidValues()
        Dim strings = New String() {"Good", "Bad", "Ugly"}

        Dim fromStringMethod = _type.GetMethods(BindingFlags.Static Or BindingFlags.Public).First(Function(f) f.Name = "FromString")

        Dim fields = GetEnumMembers()

        Dim map = strings.ToDictionary(Function(value) fields.First(Function(f) f.Name = value).GetValue(Nothing), Function(value) fromStringMethod.Invoke(Nothing, New Object() {value}))

        For Each kvp In map
            Assert.AreSame(kvp.Key, kvp.Value)
        Next
    End Sub

    <TestMethod> _
    Public Sub FromString_NullInput_ThrowsArgNullException()
        Dim fromStringMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .First(Function(f) f.Name = "FromString")

        Try
            fromStringMethod.Invoke(Nothing, New Object() {Nothing})
        Catch ex As TargetInvocationException
            Assert.IsInstanceOfType(ex.InnerException, GetType(ArgumentNullException))
            Return
        End Try

        Assert.Fail("Expected exception did not occur")

    End Sub

    <TestMethod> _
    Public Sub FromString_GarbageInput_ThrowsArgRangeException()
        Dim fromStringMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .First(Function(f) f.Name = "FromString")

        Try
            fromStringMethod.Invoke(Nothing, New Object() {"Garbage"})
        Catch ex As TargetInvocationException
            Assert.IsInstanceOfType(ex.InnerException, GetType(ArgumentOutOfRangeException))
            StringAssert.Contains(ex.InnerException.Message, "Garbage")
            Return
        End Try

        Assert.Fail("Expected exception did not occur")
    End Sub

    <TestMethod> _
    Public Sub ExplicitToInt_HasMethod()
        Dim opExplicitMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Where(Function(f) f.Name = "op_Explicit") _
            .Where(Function(f) f.ReturnType = GetType(Integer)) _
            .SingleOrDefault(Function(f) f.GetParameters().Single().ParameterType = _type)

        Assert.IsNotNull(opExplicitMethod)
    End Sub

    <TestMethod> _
    Public Sub ExplicitToInt_Values_CastCorrectly()
        Dim fields = GetEnumMembers().ToArray()

        Dim opExplicitMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Where(Function(f) f.Name = "op_Explicit") _
            .Where(Function(f) f.ReturnType = GetType(Integer)) _
            .Single(Function(f) f.GetParameters().Single().ParameterType = _type)

        Dim map = New Dictionary(Of String, Integer)() From { _
            {"Good", 0}, _
            {"Bad", 1}, _
            {"Ugly", 2} _
        }

        For Each kvp In map
            Dim field = fields.First(Function(f) f.Name = kvp.Key)
            Dim enumValue = field.GetValue(Nothing)
            Dim actual = opExplicitMethod.Invoke(Nothing, New Object() {enumValue})
            Assert.AreEqual(kvp.Value, actual, "Value of " + kvp.Key + " has incorrect integer mapping")
        Next

    End Sub

    <TestMethod> _
    Public Sub ExplicitFromInt_HasMethod()
        Dim opExplicitMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Where(Function(f) f.Name = "op_Explicit") _
            .Where(Function(f) f.ReturnType = _type) _
            .SingleOrDefault(Function(f) f.GetParameters().Single().ParameterType = GetType(Integer))

        Assert.IsNotNull(opExplicitMethod)
    End Sub

    <TestMethod> _
    Public Sub ExplicitFromInt_Values_CastCorrectly()
        Dim fields = GetEnumMembers().ToArray()

        Dim opExplicitMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Where(Function(f) f.Name = "op_Explicit") _
            .Where(Function(f) f.ReturnType = _type) _
            .Single(Function(f) f.GetParameters().Single().ParameterType = GetType(Integer))

        Dim map = New Dictionary(Of String, Integer)() From { _
            {"Good", 0}, _
            {"Bad", 1}, _
            {"Ugly", 2} _
        }

        For Each kvp In map
            Dim field = fields.First(Function(f) f.Name = kvp.Key)
            Dim expected = field.GetValue(Nothing)
            Dim actual = opExplicitMethod.Invoke(Nothing, New Object() {kvp.Value})
            Assert.AreEqual(expected, actual, "Value of " + kvp.Key + " has incorrect integer mapping")
        Next
    End Sub

    <TestMethod> _
    Public Sub ExplicitFromInt_InvalidValue_ThrowsInvalidCastException()
        Dim opExplicitMethod = _type _
            .GetMethods(BindingFlags.Static Or BindingFlags.Public) _
            .Where(Function(f) f.Name = "op_Explicit") _
            .Where(Function(f) f.ReturnType = _type) _
            .Single(Function(f) f.GetParameters().Single().ParameterType = GetType(Integer))

        Try
            Dim input As Integer = 3
            opExplicitMethod.Invoke(Nothing, New Object() {input})
        Catch ex As TargetInvocationException
            Assert.IsInstanceOfType(ex.InnerException, GetType(InvalidCastException))
            StringAssert.Contains(ex.InnerException.Message, "3")
            Return
        End Try

        Assert.Fail("Expected exception did not occur")
    End Sub


End Class


