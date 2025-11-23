using System;
using System.ComponentModel.DataAnnotations;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace BMC.Feature.Comments.Models
{
    public class CommentModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string AuthorEmail { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Comment")]
        public string Content { get; set; }

        public DateTime PostDate { get; set; }

        public ID BlogPostId { get; set; }

        public ID CommentId { get; set; }

        public bool IsApproved { get; set; }

        public Item ParentComment { get; set; }

        public CommentModel()
        {
            PostDate = DateTime.Now;
            IsApproved = false;
        }
    }
}