using Sitecore.Data.Items;

namespace BMC.Feature.Blog.Models
{
    public class TagModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int PostCount { get; set; }

        public TagModel(Item item)
        {
            if (item == null)
                return;

            Name = item.Name;
            DisplayName = item.Fields["Display Name"]?.Value ?? item.Name;
            
            var postCountField = item.Fields["Post Count"];
            if (postCountField != null && !string.IsNullOrEmpty(postCountField.Value))
            {
                int.TryParse(postCountField.Value, out int count);
                PostCount = count;
            }
        }
    }
}
