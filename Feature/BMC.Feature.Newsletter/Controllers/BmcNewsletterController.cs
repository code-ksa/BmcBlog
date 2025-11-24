using System.Web.Mvc;
using BMC.Feature.Newsletter.Models;
using BMC.Feature.Newsletter.Services;

namespace BMC.Feature.Newsletter.Controllers
{
    public class BmcNewsletterController : Controller
    {
        private readonly NewsletterService _newsletterService;

        public BmcNewsletterController()
        {
            _newsletterService = new NewsletterService();
        }

        [HttpGet]
        public ActionResult Subscribe()
        {
            return View(new NewsletterSubscriptionModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(NewsletterSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isAlreadySubscribed = _newsletterService.IsSubscribed(model.Email);
            if (isAlreadySubscribed)
            {
                TempData["ErrorMessage"] = "This email is already subscribed to our newsletter.";
                return View(model);
            }

            var success = _newsletterService.Subscribe(model.Email, model.Name);

            if (success)
            {
                _newsletterService.SendConfirmationEmail(model.Email);
                TempData["SuccessMessage"] = "Thank you for subscribing! Please check your email to confirm your subscription.";
                return RedirectToAction("Subscribe");
            }

            TempData["ErrorMessage"] = "Unable to process your subscription. Please try again later.";
            return View(model);
        }

        [HttpGet]
        public ActionResult Unsubscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid email address.";
                return RedirectToAction("Subscribe");
            }

            var success = _newsletterService.Unsubscribe(email);

            if (success)
            {
                TempData["SuccessMessage"] = "You have been successfully unsubscribed from our newsletter.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to unsubscribe. Email not found.";
            }

            return RedirectToAction("Subscribe");
        }

        [HttpGet]
        public ActionResult ConfirmSubscription(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid confirmation token.";
                return RedirectToAction("Subscribe");
            }

            // TODO: Implement token validation logic
            TempData["SuccessMessage"] = "Your subscription has been confirmed. Thank you!";
            return RedirectToAction("Subscribe");
        }
    }
}