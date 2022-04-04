using Microsoft.VisualStudio.TestTools.UnitTesting;
using Villeon;

namespace VilleonTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ResultShouldBe40()
        {
            MathBoi boi = new MathBoi();
            int result = boi.Add(20, 20);
            Assert.AreEqual(40, result);
        }
    }
}