namespace Helpfulcore.Commands.Pipelines.UiDuplicateItem
{
    using Sitecore.Buckets.Pipelines.UI;
    using System.Reflection;
    using Sitecore.Web.UI.Sheer;

    public class BucketsItemDuplicate : ItemDuplicate
    {
        public new void Execute(ClientPipelineArgs args)
        {
            base.Execute(args);
            Resume(args);
        }

        private static void Resume(ClientPipelineArgs args)
        {
            const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var method = args.GetType().GetMethod("Resume", bindFlags);
            method?.Invoke(args, new object[0]);
        }
    }
}