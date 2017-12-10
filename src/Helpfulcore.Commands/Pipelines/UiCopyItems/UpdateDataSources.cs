namespace Helpfulcore.Commands.Pipelines.UiCopyItems
{
    using System.Linq;
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Shell.Framework.Pipelines;

    public class UpdateDataSources
    {
        private readonly DataSourceService service;

        public UpdateDataSources()
        {
            this.service = new DataSourceService();
        }

        public void Execute(CopyItemsArgs args)
        {
            var database = Database.GetDatabase(args.Parameters["database"]);
            var language = LanguageManager.GetLanguage(args.Parameters["language"]);
            var originalItem = database.GetItem(new ID(args.Parameters["items"]), language);
            var copiedItem = args.Copies.FirstOrDefault();

            this.service.UpdateRelativeDataSourses(originalItem, copiedItem);
        }
    }
}
