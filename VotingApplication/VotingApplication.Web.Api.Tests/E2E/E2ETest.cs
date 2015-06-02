﻿using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Protractor;
using System;
using System.Threading;
using VotingApplication.Web.Api.Models;
using VotingApplication.Web.Api.Tests.E2E.Helpers;

namespace VotingApplication.Web.Api.Tests.E2E
{
    [TestClass]
    public class E2ETest
    {
        protected const string ChromeDriverDir = @"..\..\";
        protected const string SiteBaseUri = @"http://localhost:64205/";

        protected const string NewUserId = "8558135A-6E0A-477A-9FB5-44F3A73335E0";
        protected const string NewUserEmail = "bob@example.com";
        protected const string NewUserPassword = "ASooperSecurePassword";

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

            using (var identityContext = new TestIdentityDbContext())
            {
                identityContext.ClearUsers();
            }
        }

        public IWebDriver Driver
        {
            get
            {
                var driver = new NgWebDriver(new ChromeDriver(ChromeDriverDir));
                driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(10));

                return driver;
            }
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

        public void CreateNewUser()
        {
            var hasher = new PasswordHasher();
            string hash = hasher.HashPassword(NewUserPassword);

            var newUser = new ApplicationUser()
            {
                Id = NewUserId,
                UserName = NewUserEmail,
                Email = NewUserEmail,
                PasswordHash = hash,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            using (var dbContext = new TestIdentityDbContext())
            {
                dbContext.Users.Add(newUser);

                dbContext.SaveChanges();
            }
        }
    }
}