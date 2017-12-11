namespace Helpfulcore.Commands.Pipelines.UiDuplicateItem
{
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Web.UI.Sheer;
	using System;
	using Sitecore.Diagnostics;

	public class UpdateDataSources
    {
        private readonly DataSourceService service;

        public UpdateDataSources()
        {
            this.service = new DataSourceService();
        }

        public void Execute(ClientPipelineArgs args)
        {
			try
			{ 
				var database = Database.GetDatabase(args.Parameters["database"]);
				var language = LanguageManager.GetLanguage(args.Parameters["language"]);

				if (database == null || language == null)
				{
					args.AbortPipeline();
					return;
				}

				var originalItem = database.GetItem(new ID(args.Parameters["id"]), language);

				if (originalItem == null)
				{
					args.AbortPipeline();
					return;
				}

				var duplicatedItem = database.GetItem(originalItem.Parent.Paths.FullPath + "/" + args.Parameters["name"], language);

				if (duplicatedItem == null)
				{
					args.AbortPipeline();
					return;
				}

				this.service.UpdateRelativeDataSourses(originalItem, duplicatedItem);

				args.AbortPipeline();
			}
			catch (Exception ex)
			{
				Log.Error($"Error while updating relative datasources.", ex, this);
			}
		}
    }
}
