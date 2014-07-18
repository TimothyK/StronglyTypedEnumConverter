﻿using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void Converter_Initialize_ObjectCreated()
        {
            var converter = new Converter();
            Assert.IsNotNull(converter);
        }


        private const string CowboyTypeEnumDef = "enum CowboyType {Good,Bad,Ugly};";        

        [TestMethod]
        public void Converter_BasicEnum_ReturnsNonEmptyValue()
        {
            var converter = new Converter();
            var output = converter.Convert(CowboyTypeEnumDef);
            Assert.IsNotNull(output);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsCodeThatCompiles()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            
            var assembly = CompileCode(sourceCode);
            Assert.IsNotNull(assembly);
        }

        private static Assembly CompileCode(string sourceCode)
        {
            var compiler = new Microsoft.CSharp.CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            var compilerOut = compiler.CompileAssemblyFromSource(parameters, sourceCode);

            if (compilerOut.Errors.Count == 0) 
                return compilerOut.CompiledAssembly;

            foreach (var error in compilerOut.Errors)
                Console.Error.WriteLine(error.ToString());
            throw new ApplicationException("Could Not Compile Code");
        }

        [TestMethod]
        public void Converter_GarbageIn_ThrowsArgumentException()
        {
            var converter = new Converter();
            try
            {
                converter.Convert("garbage }{");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                return;
            }

            Assert.Fail("Expected exception did not occur");            
        }


        [TestMethod]
        public void Converter_BasicEnum_ReturnsCodeWithOneClass()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);

            var assembly = CompileCode(sourceCode);
            
            Assert.AreEqual(1, assembly.GetTypes().Length);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsClassWithSameNameAsEnum()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            Console.WriteLine(sourceCode);

            var assembly = CompileCode(sourceCode);

            var type = assembly.GetTypes()[0];
            
            Assert.AreEqual("CowboyType", type.Name);
        }
        
        [TestMethod]
        public void Converter_BasicEnum_ReturnsHasThreeStaticFields()
        {
            var converter = new Converter();
            var sourceCode = converter.Convert(CowboyTypeEnumDef);
            Console.WriteLine(sourceCode);

            var assembly = CompileCode(sourceCode);

            var type = assembly.GetTypes()[0];

            var fieldNames = type.GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(f => f.Name)
                .ToArray();

            Assert.IsTrue(fieldNames.Contains("Good"));     
            Assert.IsTrue(fieldNames.Contains("Bad"));     
            Assert.IsTrue(fieldNames.Contains("Ugly"));     
        }

    }
    
}
