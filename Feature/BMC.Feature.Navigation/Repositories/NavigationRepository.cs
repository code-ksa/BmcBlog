using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;

namespace BMC.Feature.Navigation.Repositories
{
    public class NavigationRepository
    {
        public List<Item> GetNavigationItems(Item rootItem)
        {
            if (rootItem == null)
                return new List<Item>();

            var items = rootItem.Children
                .Where(child => child != null)
                .Where(child => 
                {
                    var hideField = child.Fields["Hide In Navigation"];
                    return hideField == null || hideField.Value != "1";
                })
                .ToList();

            return items;
        }

        public List<Item> GetBreadcrumbItems()
        {
            var currentItem = Sitecore.Context.Item;
            if (currentItem == null)
                return new List<Item>();

            var breadcrumbItems = new List<Item>();
            var item = currentItem;

            while (item != null && !IsHomeItem(item))
            {
                breadcrumbItems.Insert(0, item);
                item = item.Parent;
            }

            if (item != null && IsHomeItem(item))
            {
                breadcrumbItems.Insert(0, item);
            }

            return breadcrumbItems;
        }

        public bool IsCurrentItem(Item item)
        {
            if (item == null || Sitecore.Context.Item == null)
                return false;

            return item.ID == Sitecore.Context.Item.ID;
        }

        private bool IsHomeItem(Item item)
        {
            if (item == null)
                return false;

            var site = Sitecore.Context.Site;
            if (site == null)
                return false;

            var homeItemPath = site.RootPath + site.StartItem;
            return item.Paths.FullPath.Equals(homeItemPath, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}