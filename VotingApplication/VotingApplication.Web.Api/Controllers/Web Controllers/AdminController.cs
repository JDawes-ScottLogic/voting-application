﻿using System;
using System.Web.Mvc;

namespace VotingApplication.Web.Api.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Admin Panel";

            return View();
        }
    }
}
