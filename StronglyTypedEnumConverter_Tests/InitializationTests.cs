using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void Converter_Initialize_ObjectCreated()
        {
            var converter = new Converter();
            Assert.IsNotNull(converter);
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

    }
}
