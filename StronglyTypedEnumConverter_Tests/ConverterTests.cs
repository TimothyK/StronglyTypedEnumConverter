using System;
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

    
    }
    
}
