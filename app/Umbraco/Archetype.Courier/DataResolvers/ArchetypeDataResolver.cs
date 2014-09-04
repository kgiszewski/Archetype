using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Models;
using Archetype.PropertyConverters;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.DataResolvers;
using Umbraco.Courier.ItemProviders;

namespace Archetype.Courier.DataResolvers
{
	public class ArchetypeDataResolver : PropertyDataResolverProvider
	{
		private enum Direction
		{
			Extracting,
			Packaging
		}

		public override string EditorAlias
		{
			get
			{
				return Archetype.Constants.PropertyEditorAlias;
			}
		}

		public override void ExtractingDataType(DataType item)
		{
			// No longer need to extract the DataType (int) Ids as Archetype now references the Guid [LK]
		}

		public override void ExtractingProperty(Item item, ContentProperty propertyData)
		{
			ReplacePropertyDataIds(item, propertyData, Direction.Extracting);
		}

		public override void PackagingDataType(DataType item)
		{
			AddDataTypeDependencies(item);
		}

		public override void PackagingProperty(Item item, ContentProperty propertyData)
		{
			ReplacePropertyDataIds(item, propertyData, Direction.Packaging);
		}

		private void AddDataTypeDependencies(DataType item)
		{
			if (item.Prevalues != null && item.Prevalues.Count > 0)
			{
				var prevalue = item.Prevalues[0];
				if (prevalue.Alias.InvariantEquals(Archetype.Constants.PreValueAlias) && !string.IsNullOrWhiteSpace(prevalue.Value))
				{
					var config = JsonConvert.DeserializeObject<ArchetypePreValue>(prevalue.Value);

					if (config != null && config.Fieldsets != null)
					{
						foreach (var property in config.Fieldsets.SelectMany(x => x.Properties))
						{
							item.Dependencies.Add(property.DataTypeGuid.ToString(), ProviderIDCollection.dataTypeItemProviderGuid);
						}

						item.Prevalues[0].Value = JsonConvert.SerializeObject(config, Formatting.None);
					}
				}
			}
		}

		private void ReplacePropertyDataIds(Item item, ContentProperty propertyData, Direction direction)
		{
			if (propertyData != null && propertyData.Value != null)
			{
				var dataTypeId = ExecutionContext.DatabasePersistence.GetNodeId(propertyData.DataType, NodeObjectTypes.DataType);
				var fakePropertyType = this.CreateDummyPropertyType(dataTypeId, this.EditorAlias);

				var converter = new ArchetypeValueConverter();
				var archetype = (ArchetypeModel)converter.ConvertDataToSource(fakePropertyType, propertyData.Value, false);

				if (archetype != null)
				{
					// get the `PropertyItemProvider` from the collection.
					var propertyItemProvider = ItemProviderCollection.Instance.GetProvider(ProviderIDCollection.propertyDataItemProviderGuid, this.ExecutionContext);

					foreach (var property in archetype.Fieldsets.SelectMany(x => x.Properties))
					{
						if (property == null || string.IsNullOrWhiteSpace(property.PropertyEditorAlias))
							continue;

						// create a 'fake' item for Courier to process
						var fakeItem = new ContentPropertyData()
						{
							ItemId = item.ItemId,
							Name = string.Format("{0} [{1}: Nested {2} ({3})]", new[] { item.Name, this.EditorAlias, property.PropertyEditorAlias, property.Alias }),
							Data = new List<ContentProperty>
							{
								new ContentProperty
								{
									Alias = property.Alias,
									DataType = ExecutionContext.DatabasePersistence.GetUniqueId(property.DataTypeId, NodeObjectTypes.DataType),
									PropertyEditorAlias = property.PropertyEditorAlias,
									Value = property.Value
								}
							}
						};

						if (direction == Direction.Packaging)
						{
							try
							{
								// run the 'fake' item through Courier's data resolvers
								ResolutionManager.Instance.PackagingItem(fakeItem, propertyItemProvider);
							}
							catch (Exception ex)
							{
								LogHelper.Error<ArchetypeDataResolver>(string.Concat("Error packaging data value: ", fakeItem.Name), ex);
							}

							// pass up the dependencies and resources
							item.Dependencies.AddRange(fakeItem.Dependencies);
							item.Resources.AddRange(fakeItem.Resources);
						}
						else if (direction == Direction.Extracting)
						{
							try
							{
								// run the 'fake' item through Courier's data resolvers
								ResolutionManager.Instance.ExtractingItem(fakeItem, propertyItemProvider);
							}
							catch (Exception ex)
							{
								LogHelper.Error<ArchetypeDataResolver>(string.Concat("Error extracting data value: ", fakeItem.Name), ex);
							}
						}

						if (fakeItem.Data != null && fakeItem.Data.Any())
						{
							var firstDataType = fakeItem.Data.FirstOrDefault();
							if (firstDataType != null)
							{
								// set the resolved property data value
								property.Value = firstDataType.Value;

								// (if packaging) add a dependency for the property's data-type
								if (direction == Direction.Packaging)
									item.Dependencies.Add(firstDataType.DataType.ToString(), ProviderIDCollection.dataTypeItemProviderGuid);
							}
						}
					}

					// serialize the Archetype back to a string
					propertyData.Value = archetype.SerializeForPersistence();
				}
			}
		}

		private PublishedPropertyType CreateDummyPropertyType(int dataTypeId, string propertyEditorAlias)
		{
			return new PublishedPropertyType(null, new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias) { Id = dataTypeId }));
		}
	}
}