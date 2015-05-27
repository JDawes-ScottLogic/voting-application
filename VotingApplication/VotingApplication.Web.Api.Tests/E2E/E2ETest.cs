﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Protractor;
using System;
using System.Threading;
using VotingApplication.Web.Api.Tests.E2E.Helpers;

namespace VotingApplication.Web.Api.Tests.E2E
{
    [TestClass]
    public class E2ETest
    {
        [TestInitialize]
        public void EnsureDbIsEmptyBeforeTest()
        {
            ClearDatabase();
        }

        [TestCleanup]
        public void EnsureDbIsEmptyAfterTest()
        {
            ClearDatabase();
        }

        private static void ClearDatabase()
        {
            using (var dbContext = new TestVotingContext())
            {
                dbContext.ClearDatabase();
            }
        }

        protected const string ChromeDriverDir = @"..\..\";
        protected const string SiteBaseUri = @"http://localhost:64205/";

        public NgWebDriver NgDriver
        {
            get
            {
                var driver = new NgWebDriver(new ChromeDriver(ChromeDriverDir));
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                return driver;
            }
        }

        public IWebDriver Driver
        {
            get { return NgDriver; }
        }

        public static void GoToBaseUri(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(SiteBaseUri);
        }

        public static IWebElement FindElementById(IWebDriver driver, string elementId)
        {
            return driver.FindElement(By.Id(elementId));
        }

        public void WaitForPageChange()
        {
            Thread.Sleep(1000);
        }
    }
}
