using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace BMC.Feature.Navigation.Models
{
    public class NavigationItemModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public List<NavigationItemModel> Children { get; set; }

        public NavigationItemModel()
        {
            Children = new List<NavigationItemModel>();
        }

        public NavigationItemModel(Item item)
        {
            Children = new List<NavigationItemModel>();

            if (item == null)
                return;

            Title = item.Fields["Navigation Title"]?.Value ?? item.Fields["Title"]?.Value ?? item.DisplayName;
            Url = LinkManager.GetItemUrl(item);
            IsActive = Sitecore.Context.Item != null && Sitecore.Context.Item.ID == item.ID;
        }

        public static List<NavigationItemModel> BuildNavigationTree(Item rootItem)
        {
            if (rootItem == null)
                return new List<NavigationItemModel>();

            var navigationItems = new List<NavigationItemModel>();

            foreach (Item child in rootItem.Children)
            {
                if (child == null)
                    continue;

                var hideInNavigationField = child.Fields["Hide In Navigation"];
                if (hideInNavigationField != null && hideInNavigationField.Value == "1")
                    continue;

                var navItem = new NavigationItemModel(child);

                if (child.HasChildren)
                {
                    navItem.Children = BuildNavigationTree(child);
                }

                navigationItems.Add(navItem);
            }

            return navigationItems;
        }
    }
}