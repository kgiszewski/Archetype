using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Enums;
using Umbraco.Courier.Core.Helpers;
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
			// No longer need to extract the DataType (int) Ids as they now reference the Guid [LK]
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
					var config = JsonConvert.DeserializeObject<Archetype.Models.ArchetypePreValue>(prevalue.Value);

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
				// just look at the amount of dancing around we have to do in order to fake a `PublishedPropertyType`?!
				var dataTypeId = PersistenceManager.Default.GetNodeId(propertyData.DataType, NodeObjectTypes.DataType);
				var fakePropertyType = this.CreateFakePropertyType(dataTypeId, this.EditorAlias);

				var converter = new Archetype.PropertyConverters.ArchetypeValueConverter();
				var archetype = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(fakePropertyType, propertyData.Value, false);

				if (archetype != null)
				{
					// create a 'fake' provider, as ultimately only the 'Packaging' enum will be referenced.
					var fakeItemProvider = new PropertyItemProvider();

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
									DataType = PersistenceManager.Default.GetUniqueId(property.DataTypeId, NodeObjectTypes.DataType),
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
								ResolutionManager.Instance.PackagingItem(fakeItem, fakeItemProvider);
							}
							catch (Exception ex)
							{
								LogHelper.Error<ArchetypeDataResolver>(string.Concat("Error resolving data value: ", fakeItem.Name), ex);
							}

							// pass up the dependencies and resources
							item.Dependencies.AddRange(fakeItem.Dependencies);
							item.Resources.AddRange(fakeItem.Resources);

							if (fakeItem.Data != null && fakeItem.Data.Any())
							{
								var firstDataType = fakeItem.Data.FirstOrDefault();
								if (firstDataType != null)
								{
									// add a dependency for the property's data-type
									property.DataTypeGuid = firstDataType.ToString();
									item.Dependencies.Add(property.DataTypeGuid, ProviderIDCollection.dataTypeItemProviderGuid);
								}
							}
						}
						else if (direction == Direction.Extracting)
						{
							// run the 'fake' item through Courier's data resolvers
							ResolutionManager.Instance.ExtractingItem(fakeItem, fakeItemProvider);

							// resolve the property's data-type Id
							int identifier;
							if (int.TryParse(Dependencies.ConvertIdentifier(property.DataTypeGuid, IdentifierReplaceDirection.FromGuidToNodeId), out identifier))
								property.DataTypeId = dataTypeId;
						}

						if (fakeItem.Data != null && fakeItem.Data.Any())
						{
							var firstDataType = fakeItem.Data.FirstOrDefault();
							if (firstDataType != null)
							{
								// set the resolved property data value
								property.Value = firstDataType.Value;
							}
						}
					}

					if (item.Name.Contains(string.Concat(this.EditorAlias, ": Nested")))
					{
						// if the Archetype is nested, then we only want to return the object itself - not a serialized string
						propertyData.Value = archetype;
					}
					else
					{
						// if the Archetype is the root/container, then we can serialize it to a string
						propertyData.Value = JsonConvert.SerializeObject(archetype, Formatting.None);
					}
				}
			}
		}

		private PublishedPropertyType CreateFakePropertyType(int dataTypeId, string propertyEditorAlias)
		{
			return new PublishedPropertyType(null, new PropertyType(new DataTypeDefinition(-1, propertyEditorAlias) { Id = dataTypeId }));
		}
	}
}