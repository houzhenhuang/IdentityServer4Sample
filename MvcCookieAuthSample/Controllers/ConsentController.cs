using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models;
using MvcCookieAuthSample.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcCookieAuthSample.Services;

namespace MvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentServices;
        public ConsentController(ConsentService consentServices)
        {
            this._consentServices = consentServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentServices.BuildConsentViewModel(returnUrl);
            if (model is null)
            {
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result = await _consentServices.ProcessConsent(viewModel);
            if (result.IsRedirect)
            {
                return Redirect(result.ReturnUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(result.ConsentViewModel);
        }
    }
}
