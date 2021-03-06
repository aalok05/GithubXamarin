﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GithubXamarin.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GithubXamarin.Core.UnitTests
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void HtmlUrlToRepositoryNameConverterTestForDefaultValues()
        {
            var result = HtmlUrlToRepositoryNameConverter.Convert("https://github.com/prajjwaldimri/GithubXamarin/issues/50", "prajjwaldimri");
            Assert.AreEqual("GithubXamarin", result);
        }

        [TestMethod]
        public void HtmlUrlToRepositoryNameConverterTestForNullValues()
        {
            var result = HtmlUrlToRepositoryNameConverter.Convert(null, null);
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void HtmlUrlToRepositoryNameConverterTestForEmptyValues()
        {
            var result = HtmlUrlToRepositoryNameConverter.Convert("        ", "   ");
            Assert.AreEqual(string.Empty, result);
        }
    }
}
