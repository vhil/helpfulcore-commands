using Sitecore.Shell.Framework.Pipelines;

namespace Helpfulcore.Commands.Pipelines.UiCopyItems
{
    public class UpdateDataSources
    {
        public void Execute(CopyItemsArgs args)
        {
            var destination = args.Parameters["destination"];
            var items = args.Copies;
        }
    }
}
