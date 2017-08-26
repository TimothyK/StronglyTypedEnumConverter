using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace StronglyTypedEnumConverter
{
    [TestClass]
    public class CowboyTypeTests
    {
        [TestMethod]
        public void CowboyType_FromInvalidString_ThrowArgRange()
        {
            try
            {
                CowboyType.FromString("Jolly");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");            
        }
        
        [TestMethod]
        public void CowboyType_FromNullString_ThrowArgNull()
        {
            try
            {
                CowboyType.FromString(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");            
        }

        [TestMethod]
        public void CowboyType_All_ReturnsAllThreeValues()
        {
            var types = CowboyType.All().ToArray();

            types.ShouldContain(CowboyType.Good);
            types.ShouldContain(CowboyType.Bad);
            types.ShouldContain(CowboyType.Ugly);
        }

        [TestMethod]
        public void CowboyType_StringValuesRoundTrip_ReturnsOriginalValue()
        {
            foreach (var type in CowboyType.All())
                type.ShouldBeSameAs(CowboyType.FromString(type.ToString()), type + " did not round trip successfully");
        }

        [TestMethod]
        public void CowboyType_CastToInt_ReturnsExpected()
        {
            ((int) CowboyType.Good).ShouldBe(0);
            ((int) CowboyType.Bad).ShouldBe(1);
            ((int) CowboyType.Ugly).ShouldBe(2);
        }
        
        [TestMethod]
        public void CowboyType_CastFromInt_ReturnsExpected()
        {
            ((CowboyType) 0).ShouldBe(CowboyType.Good);
            ((CowboyType) 1).ShouldBe(CowboyType.Bad);
            ((CowboyType) 2).ShouldBe(CowboyType.Ugly);
        }

        [TestMethod]
        public void CowboyType_FromInvalidInt_ThrowInvalidCast()
        {
            try
            {
                var dummy = (CowboyType) 4;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            Assert.Fail("Expected exception did not occur");
        }

    }
}
