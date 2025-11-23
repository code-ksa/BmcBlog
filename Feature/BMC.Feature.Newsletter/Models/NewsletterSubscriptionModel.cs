using System.ComponentModel.DataAnnotations;

namespace BMC.Feature.Newsletter.Models
{
    public class NewsletterSubscriptionModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must accept the terms and conditions")]
        [Display(Name = "I accept the terms and conditions")]
        public bool AcceptTerms { get; set; }
    }
}