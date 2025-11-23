using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace BMC.Foundation.SitecoreExtensions.Pipelines
{
    public class CustomResolveRenderingDatasource : GetRendererProcessor
    {
        public override void Process(GetRendererArgs args)
        {
            if (args == null || args.Result != null)
                return;

            var rendering = args.Rendering;
            if (rendering == null)
                return;

            // Custom datasource resolution logic
            var datasource = rendering.DataSource;
            
            if (string.IsNullOrEmpty(datasource))
            {
                // Use context item if no datasource is specified
                rendering.Item = Sitecore.Context.Item;
            }
            else
            {
                // Resolve datasource item
                var datasourceItem = rendering.Item?.Database?.GetItem(datasource);
                if (datasourceItem != null)
                {
                    rendering.Item = datasourceItem;
                }
            }
        }
    }
}
