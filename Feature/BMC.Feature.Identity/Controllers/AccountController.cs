using System.Web.Mvc;
using BMC.Feature.Identity.Models;
using BMC.Feature.Identity.Services;
using Sitecore.Security.Authentication;

namespace BMC.Feature.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isValid = _userService.ValidateUser(model.Username, model.Password);
            if (isValid)
            {
                var username = "extranet\\" + model.Username;
                var user = Sitecore.Security.Accounts.User.FromName(username, false);

                if (user != null && user.IsAuthenticated)
                {
                    // FIX: استخدم user.Name بدلاً من user مباشرة
                    AuthenticationManager.Login(user.Name, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = _userService.CreateUser(model.Username, model.Email, model.Password);
            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Unable to create user. Username may already exist.");
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            AuthenticationManager.Logout();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = _userService.SendPasswordResetEmail(model.Email);
            if (success)
            {
                TempData["SuccessMessage"] = "Password reset instructions have been sent to your email.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Unable to process password reset request.");
            return View(model);
        }
    }
}