using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BMC.Feature.Hero.Models;
using Sitecore.Mvc.Presentation;

namespace BMC.Feature.Hero.Controllers
{
    public class HeroSliderController : Controller
    {
        public ActionResult Index()
        {
            var rendering = RenderingContext.CurrentOrNull;
            if (rendering == null)
                return Content("No rendering context");

            var datasource = rendering.Rendering.Item;
            var contextItem = Sitecore.Context.Item;

            var slideItems = GetSlideItems(datasource, contextItem);
            var slides = slideItems.Select(item => new HeroSlideModel(item)).ToList();

            if (!slides.Any())
                return Content(string.Empty);

            return View(slides);
        }

        private List<Sitecore.Data.Items.Item> GetSlideItems(Sitecore.Data.Items.Item datasource, Sitecore.Data.Items.Item contextItem)
        {
            var sourceItem = datasource ?? contextItem;
            if (sourceItem == null)
                return new List<Sitecore.Data.Items.Item>();

            var children = sourceItem.Children;
            if (children == null || children.Count == 0)
                return new List<Sitecore.Data.Items.Item>();

            var slideItems = children
                .Where(child => child.TemplateName == "Hero Slide" || child.TemplateName.Contains("Slide"))
                .ToList();

            return slideItems;
        }
    }
}