using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Linq;

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

        [TestMethod]
        public void CowboyType_Sort_ValuesAreSorted()
        {
            //Arrange
            var unsorted = new[] {CowboyType.Bad, CowboyType.Ugly, CowboyType.Good};

            //Act
            var sorted = unsorted.OrderBy(x => x).ToArray();

            //Assert
            sorted[0].ShouldBe(CowboyType.Good);
            sorted[1].ShouldBe(CowboyType.Bad);
            sorted[2].ShouldBe(CowboyType.Ugly);
        }

        [TestMethod] public void CowboyType_GoodLessThanBad_True() => (CowboyType.Good < CowboyType.Bad).ShouldBeTrue();
        [TestMethod] public void CowboyType_GoodLessThanOrEqualBad_True() => (CowboyType.Good <= CowboyType.Bad).ShouldBeTrue();
        [TestMethod] public void CowboyType_GoodGreaterThanBad_False() => (CowboyType.Good > CowboyType.Bad).ShouldBeFalse();
        [TestMethod] public void CowboyType_GoodGreaterThanOrEqualThanBad_False() => (CowboyType.Good >= CowboyType.Bad).ShouldBeFalse();


        // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
        [TestMethod] public void CowboyType_GoodLessThanGood_False () => (CowboyType.Good < CowboyType.Good).ShouldBeFalse();
        [TestMethod] public void CowboyType_GoodLessThanOrEqualGood_True() => (CowboyType.Good <= CowboyType.Good).ShouldBeTrue();
        [TestMethod] public void CowboyType_GoodGreaterThanGood_False() => (CowboyType.Good > CowboyType.Good).ShouldBeFalse();
        [TestMethod] public void CowboyType_GoodGreaterThanOrEqualThanGood_True() => (CowboyType.Good >= CowboyType.Good).ShouldBeTrue();
#pragma warning restore CS1718 // Comparison made to same variable
        // ReSharper restore EqualExpressionComparison

    }
}
