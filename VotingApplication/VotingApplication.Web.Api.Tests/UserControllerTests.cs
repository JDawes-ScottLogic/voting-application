﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VotingApplication.Data;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using VotingApplication.Web.Api.Controllers;

namespace VotingApplication.Web.Api.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private UserController _controller;
        private User _bobUser;

        [TestInitialize]
        public void setup()
        {
            _bobUser = new User { Id = 1, Name = "Bob" };
            List<User> dummyUsers = new List<User>();
            dummyUsers.Add(_bobUser);

            var mockContextFactory = new Mock<IContextFactory>();
            var mockContext = new Mock<IVotingContext>();
            mockContextFactory.Setup(a => a.CreateContext()).Returns(mockContext.Object);
            mockContext.Setup(a => a.Users).Returns(dummyUsers);

            _controller = new UserController(mockContextFactory.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();
        }

        [TestMethod]
        public void GetReturnsAllUsers()
        {
            // Act
            var response = _controller.Get();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void GetReturnsNonNullUsers()
        {
            // Act
            var response = _controller.Get();
            List<User> responseUsers = ((ObjectContent)response.Content).Value as List<User>;

            // Assert
            Assert.IsNotNull(responseUsers);
        }

        [TestMethod]
        public void GetReturnsUsersFromTheDatabase()
        {
            // Act
            var response = _controller.Get();
            List<User> responseUsers = ((ObjectContent)response.Content).Value as List<User>;

            // Assert
            List<User> expectedUsers = new List<User>();
            expectedUsers.Add(_bobUser);
            CollectionAssert.AreEquivalent(expectedUsers, responseUsers);
        }
    }
}
