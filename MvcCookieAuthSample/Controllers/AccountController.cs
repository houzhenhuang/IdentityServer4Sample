using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Test;
using System.Threading.Tasks;
using MvcCookieAuthSample.ViewModels;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;
using IdentityServer4.Services;

namespace MvcCookieAuthSample.Controllers
{
    public class AccountController : Controller
    {
        //private readonly TestUserStore _users;
        //public AccountController(TestUserStore users)
        //{
        //    this._users = users;
        //}
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IIdentityServerInteractionService _identityServerInteractionService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IIdentityServerInteractionService identityServerInteractionService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._identityServerInteractionService = identityServerInteractionService;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user is null)
                {
                    ModelState.AddModelError(nameof(loginViewModel.Email), "Email is not exists");
                }
                else
                {
                    if (await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
                    {
                        AuthenticationProperties props = null;
                        if (loginViewModel.RememberMe)
                        {
                            props = new AuthenticationProperties
                            {
                                IsPersistent = loginViewModel.RememberMe,
                                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                            };
                        }
                        await _signInManager.SignInAsync(user, props);
                        if (_identityServerInteractionService.IsValidReturnUrl(returnUrl))
                        {
                            return RedirectToLoacl(returnUrl);
                        }
                        return Redirect("~/");
                    }
                    ModelState.AddModelError(nameof(loginViewModel.Password), "Wrong Password");
                }
            }
            return View();
        }
        //public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ViewData["ReturnUrl"] = returnUrl;
        //        var user = _users.FindByUsername(loginViewModel.UserName);
        //        if (user is null)
        //        {
        //            ModelState.AddModelError(nameof(loginViewModel.UserName), "UserName is not exists");
        //        }
        //        else
        //        {
        //            if (_users.ValidateCredentials(loginViewModel.UserName, loginViewModel.Password))
        //            {
        //                var props = new AuthenticationProperties
        //                {
        //                    IsPersistent = true,
        //                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
        //                };
        //                await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
        //                    HttpContext,
        //                    user.SubjectId,
        //                    user.Username,
        //                    props
        //                );
        //                return RedirectToLoacl(returnUrl);
        //            }
        //            ModelState.AddModelError(nameof(loginViewModel.Password), "Wrong Password");
        //        }
        //    }
        //    return View();
        //}
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                var identityUser = new ApplicationUser
                {
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.Email,
                    NormalizedUserName = registerViewModel.Email
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerViewModel.Password);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(identityUser, new AuthenticationProperties { IsPersistent = true });

                    return RedirectToLoacl(returnUrl);
                }
                else
                {
                    AddErrors(identityResult);
                }
            }
            return View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        private IActionResult RedirectToLoacl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}