using System.Collections.Generic;

namespace BMC.Feature.Comments.Models
{
    public class CommentListViewModel
    {
        public List<CommentModel> Comments { get; set; }
        public int TotalComments { get; set; }
        public bool AllowComments { get; set; }

        public CommentListViewModel()
        {
            Comments = new List<CommentModel>();
            TotalComments = 0;
            AllowComments = true;
        }
    }
}