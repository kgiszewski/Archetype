using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Extensions;
using Archetype.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Logging;
using Umbraco.Courier.Core.ProviderModel;
using Umbraco.Courier.DataResolvers;
using Umbraco.Courier.ItemProviders;

namespace Archetype.Courier.DataResolvers
{
    /// <summary>
    /// Lee Kelleher's implementation of Courier's PropertyDataResolverProvider for Archetype.
    /// </summary>
	public class ArchetypeDataResolver : PropertyDataResolverProvider
	{
		private enum Direction
		{
			Extracting,
			Packaging
		}

        /// <summary>
        /// Gets the editor alias.
        /// </summary>
        /// <value>
        /// The editor alias.
        /// </value>
		public override string EditorAlias
		{
			get
			{
				return Constants.PropertyEditorAlias;
			}
		}

        /// <summary>
        /// Extractings the type of the data.
        /// </summary>
        /// <param name="item">The item.</param>
		public override void ExtractingDataType(DataType item)
		{
			// No longer need to extract the DataType (int) Ids as Archetype now references the Guid [LK]
		}

        /// <summary>
        /// Extractings the property.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propertyData">The property data.</param>
		public override void ExtractingProperty(Item item, ContentProperty propertyData)
		{
			ReplacePropertyDataIds(item, propertyData, Direction.Extracting);
		}

        /// <summary>
        /// Packagings the type of the data.
        /// </summary>
        /// <param name="item">The item.</param>
		public override void PackagingDataType(DataType item)
		{
			AddDataTypeDependencies(item);
		}

        /// <summary>
        /// Packages the property.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propertyData">The property data.</param>
		public override void PackagingProperty(Item item, ContentProperty propertyData)
		{
			ReplacePropertyDataIds(item, propertyData, Direction.Packaging);
		}

        /// <summary>
        /// Adds the datatype dependencies.
        /// </summary>
        /// <param name="item">The item.</param>
		private void AddDataTypeDependencies(DataType item)
		{
			if (item.Prevalues != null && item.Prevalues.Count > 0)
			{
				var prevalue = item.Prevalues[0];
				if (prevalue.Alias.InvariantEquals(Constants.PreValueAlias) && !string.IsNullOrWhiteSpace(prevalue.Value))
				{
					var config = JsonConvert.DeserializeObject<ArchetypePreValue>(prevalue.Value);

					if (config != null && config.Fieldsets != null)
					{
						foreach (var property in config.Fieldsets.SelectMany(x => x.Properties))
						{
							item.Dependencies.Add(property.DataTypeGuid.ToString(), ItemProviderIds.dataTypeItemProviderGuid);
						}

						item.Prevalues[0].Value = JsonConvert.SerializeObject(config, Formatting.None);
					}
				}
			}
		}

        /// <summary>
        /// Replaces the property data Ids.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="propertyData">The property data.</param>
        /// <param name="direction">The direction.</param>
		private void ReplacePropertyDataIds(Item item, ContentProperty propertyData, Direction direction)
		{
			if (propertyData != null && propertyData.Value != null)
			{
				var dataType = ExecutionContext.DatabasePersistence.RetrieveItem<DataType>(new ItemIdentifier(propertyData.DataType.ToString(),
					ItemProviderIds.dataTypeItemProviderGuid));

				//Fetch the Prevalues for the current Property's DataType (if its an 'Archetype config')
				var prevalue = dataType.Prevalues.FirstOrDefault(x => x.Alias.ToLowerInvariant().Equals("archetypeconfig"));
				var archetypePreValue = prevalue == null
					? null
					: JsonConvert.DeserializeObject<ArchetypePreValue>(prevalue.Value,
						ArchetypeHelper.Instance.JsonSerializerSettings);
				RetrieveAdditionalProperties(ref archetypePreValue);

				//Deserialize the value of the current Property to an ArchetypeModel and set additional properties before converting values
				var sourceJson = propertyData.Value.ToString();
				var archetype = JsonConvert.DeserializeObject<ArchetypeModel>(sourceJson, ArchetypeHelper.Instance.JsonSerializerSettings);
				RetrieveAdditionalProperties(ref archetype, archetypePreValue);

				if (archetype != null)
				{
					// get the `PropertyItemProvider` from the collection.
					var propertyItemProvider = ItemProviderCollection.Instance.GetProvider(ItemProviderIds.propertyDataItemProviderGuid, ExecutionContext);

					foreach (var property in archetype.Fieldsets.SelectMany(x => x.Properties))
					{
						if (property == null || string.IsNullOrWhiteSpace(property.PropertyEditorAlias))
							continue;

						// create a 'fake' item for Courier to process
						var fakeItem = new ContentPropertyData
						{
							ItemId = item.ItemId,
							Name = string.Format("{0} [{1}: Nested {2} ({3})]", new[] { item.Name, EditorAlias, property.PropertyEditorAlias, property.Alias }),
							Data = new List<ContentProperty>
							{
								new ContentProperty
								{
									Alias = property.Alias,
									DataType = ExecutionContext.DatabasePersistence.GetUniqueId(property.DataTypeId, UmbracoNodeObjectTypeIds.DataType),
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
								CourierLogHelper.Error<ArchetypeDataResolver>(string.Concat("Error packaging data value: ", fakeItem.Name), ex);
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
								CourierLogHelper.Error<ArchetypeDataResolver>(string.Concat("Error extracting data value: ", fakeItem.Name), ex);
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
									item.Dependencies.Add(firstDataType.DataType.ToString(), ItemProviderIds.dataTypeItemProviderGuid);
							}
						}
					}

					// serialize the Archetype back to a string
					propertyData.Value = archetype.SerializeForPersistence();
				}
			}
		}

        /// <summary>
        /// Retrieves the additional properties.
        /// </summary>
        /// <param name="preValue">The pre value.</param>
		private void RetrieveAdditionalProperties(ref ArchetypePreValue preValue)
		{
			if (preValue == null)
				return;

			foreach (var fieldset in preValue.Fieldsets)
			{
				foreach (var property in fieldset.Properties)
				{
					var dataType = ExecutionContext.DatabasePersistence.RetrieveItem<DataType>(
						new ItemIdentifier(property.DataTypeGuid.ToString(),
							ItemProviderIds.dataTypeItemProviderGuid));

					if (dataType == null)
						continue;

					property.PropertyEditorAlias = dataType.PropertyEditorAlias;
				}
			}
		}

        /// <summary>
        /// Retrieves the additional properties.
        /// </summary>
        /// <param name="archetype">The archetype.</param>
        /// <param name="preValue">The pre value.</param>
		private void RetrieveAdditionalProperties(ref ArchetypeModel archetype, ArchetypePreValue preValue)
		{
			foreach (var fieldset in preValue.Fieldsets)
			{
				var fieldsetAlias = fieldset.Alias;
				foreach (var fieldsetInst in archetype.Fieldsets.Where(x => x.Alias == fieldsetAlias))
				{
					foreach (var property in fieldset.Properties)
					{
						var propertyAlias = property.Alias;
						foreach (var propertyInst in fieldsetInst.Properties.Where(x => x.Alias == propertyAlias))
						{
							propertyInst.DataTypeGuid = property.DataTypeGuid.ToString();
							propertyInst.DataTypeId = ExecutionContext.DatabasePersistence.GetNodeId(
								property.DataTypeGuid, UmbracoNodeObjectTypeIds.DataType);
							propertyInst.PropertyEditorAlias = property.PropertyEditorAlias;
						}
					}
				}
			}
		}
	}
}