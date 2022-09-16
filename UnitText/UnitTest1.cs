using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitText
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]

        public void Page_SkipPagenumberTimesOne_ReturnChosenItems(int currentPageIndex)
        {
            List<string> lstr = new List<string> { "one", "two", "three"};
            var expect = lstr[currentPageIndex - 1];
            var result = lstr.Skip(1 * (currentPageIndex - 1)).Take(1).ElementAt(0);
            Assert.AreEqual(result, expect);
        }
    }
}
