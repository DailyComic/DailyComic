using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DailyComic.HtmlUtils.Tests
{
    [TestClass]
    public class TestUrlHelperCombine
    {
        [TestMethod]
        public void TwoParts_OnlyTrailingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "baz/");

            Assert.AreEqual("http://foo.bar/baz", result);
        }

        [TestMethod]
        public void TwoParts_OnlyLeadingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar", "/baz");

            Assert.AreEqual("http://foo.bar/baz", result);
        }

        [TestMethod]
        public void TwoParts_BothLeadingAndTrailingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "/baz/");

            Assert.AreEqual("http://foo.bar/baz", result);
        }

        [TestMethod]
        public void ThreeParts_OnlyTrailingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "baz/", "bang/");

            Assert.AreEqual("http://foo.bar/baz/bang", result);
        }

        [TestMethod]
        public void ThreeParts_OnlyLeadingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar", "/baz", "/bang");

            Assert.AreEqual("http://foo.bar/baz/bang", result);
        }

        [TestMethod]
        public void ThreeParts_BothLeadingAndTrailingSlashes()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "/baz/", "/bang/");

            Assert.AreEqual("http://foo.bar/baz/bang", result);
        }

        [TestMethod]
        public void ThreeParts_SecondIsNull()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", null, "/bang/");

            Assert.AreEqual("http://foo.bar/bang", result);
        }

        [TestMethod]
        public void ThreeParts_ThirdIsNull()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "/baz/", null);

            Assert.AreEqual("http://foo.bar/baz", result);
        }

        [TestMethod]
        public void ThreeParts_SecondIsEmpty()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "", "/bang/");

            Assert.AreEqual("http://foo.bar/bang", result);
        }

        [TestMethod]
        public void ThreeParts_ThirdIsEmpty()
        {
            var result = UrlHelper.CombineUrls("http://foo.bar/", "/baz/", "");

            Assert.AreEqual("http://foo.bar/baz", result);
        }
    }
}
