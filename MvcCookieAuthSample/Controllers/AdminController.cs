using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models;

namespace MvcCookieAuthSample.Controllers
{
    [Authorize]
    public class AdminController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
