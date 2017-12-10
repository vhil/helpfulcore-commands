namespace Helpfulcore.Commands.Pipelines.UiDuplicateItem
{
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Web.UI.Sheer;

    public class UpdateDataSources
    {
        private readonly DataSourceService service;

        public UpdateDataSources()
        {
            this.service = new DataSourceService();
        }

        public void Execute(ClientPipelineArgs args)
        {
            var database = Database.GetDatabase(args.Parameters["database"]);
            var language = LanguageManager.GetLanguage(args.Parameters["language"]);
            var originalItem = database.GetItem(new ID(args.Parameters["id"]), language);
            var duplicatedItem = database.GetItem(originalItem.Parent.Paths.FullPath + "/" + args.Parameters["name"], language);

            service.UpdateRelativeDataSourses(originalItem, duplicatedItem);

            args.AbortPipeline();
        }
    }
}
