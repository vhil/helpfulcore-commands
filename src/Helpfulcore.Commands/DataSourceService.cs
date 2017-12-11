namespace Helpfulcore.Commands
{
    using System.Collections.Generic;
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Layouts;

    public class DataSourceService
    {
        public void UpdateRelativeDataSourses(Item originalItem, Item destinationItem)
        {
            if (originalItem == null || destinationItem == null) return;

            var items = this.GetAllItemsReccursively(originalItem, destinationItem);

            foreach (var item in items)
            {
                var original = item.Key;
                var destination = item.Value;

                using (new EditContext(destination))
                {
                    destination[FieldIDs.LayoutField] = this.UpdateRenderingDataSourses(original, destination, FieldIDs.LayoutField);
                }

                var languages = LanguageManager.GetLanguages(destination.Database);

                foreach (var language in languages)
                {
                    original = original.Database.GetItem(original.ID, language);
                    destination = originalItem.Database.GetItem(destination.ID, language);
                    using (new EditContext(destination))
                    {
                        destination[FieldIDs.FinalLayoutField] = this.UpdateRenderingDataSourses(original, destination, FieldIDs.FinalLayoutField);
                    }
                }
            }
        }

        protected string UpdateRenderingDataSourses(Item originalItem, Item destinationItem, ID layoutFieldId)
        {
            var layoutString = destinationItem[layoutFieldId];
            var currentLayoutXml = LayoutField.GetFieldValue(destinationItem.Fields[layoutFieldId]);
            if (string.IsNullOrEmpty(currentLayoutXml) || string.IsNullOrEmpty(layoutString)) return string.Empty;

            var layout = LayoutDefinition.Parse(currentLayoutXml);
            var replacememts = new Dictionary<string, string>();

            // loop over devices in the rendering
            for (var deviceIndex = layout.Devices.Count - 1; deviceIndex >= 0; deviceIndex--)
            {
                var device = layout.Devices[deviceIndex] as DeviceDefinition;

                if (device == null) continue;

                // loop over renderings within the device
                for (var renderingIndex = device.Renderings.Count - 1; renderingIndex >= 0; renderingIndex--)
                {
                    var rendering = device.Renderings[renderingIndex] as RenderingDefinition;

                    if (!string.IsNullOrWhiteSpace(rendering?.Datasource))
                    {
                        ID dataSource;
                        if (ID.TryParse(rendering.Datasource, out dataSource))
                        {
                            if (!replacememts.ContainsKey(rendering.Datasource))
                            {
                                replacememts.Add(rendering.Datasource, this.EnsureRelativeDatasourse(originalItem, destinationItem, dataSource));
                            }
                        }
                    }
                }
            }

            foreach (var replacement in replacememts)
            {
                layoutString = layoutString.Replace(replacement.Key, replacement.Value);
            }

            return layoutString;
        }

        protected string EnsureRelativeDatasourse(Item originalItem, Item destinationItem, ID dataSource)
        {
            var datasourceItem = destinationItem.Database.GetItem(dataSource, destinationItem.Language);

            var relativeDatasourceItem = this.GetDestinationRelativeItem(datasourceItem, originalItem, destinationItem);

            return relativeDatasourceItem?.ID.ToString() ?? dataSource.ToString();
        }

        protected IDictionary<Item, Item> GetAllItemsReccursively(Item originalItem, Item destinationItem)
        {
            var ret = new Dictionary<Item, Item>
            {
                {originalItem, destinationItem}
            };

            var allOriginals = this.GetChildrenReccursively(originalItem);

            foreach (var original in allOriginals)
            {
                var destination = this.GetDestinationRelativeItem(original, originalItem, destinationItem);
                if (destination != null)
                {
                    ret.Add(original, destination);
                }
            }

            return ret;
        }

        private Item GetDestinationRelativeItem(Item original, Item originalItem, Item destinationItem)
        {
            if (original.Paths.FullPath.StartsWith(originalItem.Paths.FullPath))
            {
                var relativePath = original.Paths.FullPath.Replace(originalItem.Paths.FullPath, string.Empty);
                var relativeDatasourceItemPath = destinationItem.Paths.FullPath + relativePath;
                var relativeItem = destinationItem.Database.GetItem(relativeDatasourceItemPath, destinationItem.Language);

                if (relativeItem != null)
                {
                    return relativeItem;
                }
            }

            return null;
        }

        private IEnumerable<Item> GetChildrenReccursively(Item item)
        {
            var result = new List<Item>();

            foreach (Item child in item.GetChildren(ChildListOptions.IgnoreSecurity))
            {
                result.Add(child);

                result.AddRange(this.GetChildrenReccursively(child));
            }

            return result;
        }
    }
}