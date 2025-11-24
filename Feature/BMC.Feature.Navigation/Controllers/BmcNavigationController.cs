using System.Web.Mvc;
using BMC.Feature.Navigation.Models;
using BMC.Feature.Navigation.Repositories;

namespace BMC.Feature.Navigation.Controllers
{
    public class BmcNavigationController : Controller
    {
        private readonly NavigationRepository _repository;

        public BmcNavigationController()
        {
            _repository = new NavigationRepository();
        }

        public ActionResult Header()
        {
            var siteRoot = GetSiteRoot();
            if (siteRoot == null)
                return Content(string.Empty);

            var navigationItems = NavigationItemModel.BuildNavigationTree(siteRoot);

            return View(navigationItems);
        }

        public ActionResult Footer()
        {
            var siteRoot = GetSiteRoot();
            if (siteRoot == null)
                return Content(string.Empty);

            var navigationItems = NavigationItemModel.BuildNavigationTree(siteRoot);

            return View(navigationItems);
        }

        public ActionResult Breadcrumb()
        {
            var breadcrumbItems = _repository.GetBreadcrumbItems();
            var breadcrumbModels = new System.Collections.Generic.List<NavigationItemModel>();

            foreach (var item in breadcrumbItems)
            {
                breadcrumbModels.Add(new NavigationItemModel(item));
            }

            return View(breadcrumbModels);
        }

        private Sitecore.Data.Items.Item GetSiteRoot()
        {
            var site = Sitecore.Context.Site;
            if (site == null)
                return null;

            var database = Sitecore.Context.Database;
            if (database == null)
                return null;

            var rootPath = site.RootPath + site.StartItem;
            var rootItem = database.GetItem(rootPath);

            return rootItem;
        }
    }
}