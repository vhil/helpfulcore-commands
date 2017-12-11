namespace Helpfulcore.Commands.Pipelines.UiCopyItems
{
    using System.Linq;
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Shell.Framework.Pipelines;
	using System;
	using Sitecore.Diagnostics;

	public class UpdateDataSources
    {
        private readonly DataSourceService service;

        public UpdateDataSources()
        {
            this.service = new DataSourceService();
        }

        public void Execute(CopyItemsArgs args)
        {
	        try
	        {
		        var database = Database.GetDatabase(args.Parameters["database"]);
		        var language = LanguageManager.GetLanguage(args.Parameters["language"]);

		        if (database == null || language == null) return;

		        var originalItem = database.GetItem(new ID(args.Parameters["items"]), language);
		        var copiedItem = args.Copies.FirstOrDefault();

		        if (originalItem == null || copiedItem == null) return;

		        this.service.UpdateRelativeDataSourses(originalItem, copiedItem);
	        }
	        catch (Exception ex)
	        {
				Log.Error($"Error while updating relative datasources.", ex, this);
	        }
        }
    }
}
