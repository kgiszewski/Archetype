using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archetype.Extensions;
using Archetype.PropertyConverters;
using NUnit.Framework;

namespace Archetype.Tests.Extensions
{
	[TestFixture]
	class EnumerableExtensionsTests
	{
		[Test]
		public void Can_Get_List_In_Groups_Of_Three()
		{
			var items = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
			var groups = items.InGroupsOf(3);
			Assert.AreEqual(4, groups.Count());
			Assert.AreEqual("1, 2, 3", string.Join(", ", groups.First()));
			Assert.AreEqual("4, 5, 6", string.Join(", ", groups.Skip(1).First()));
			Assert.AreEqual("7, 8, 9", string.Join(", ", groups.Skip(2).First()));
			Assert.AreEqual("10", string.Join(", ", groups.Last()));
		}

		[Test]
		public void Can_Get_List_In_Groups_Of_Five()
		{
			var items = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
			var groups = items.InGroupsOf(5);
			Assert.AreEqual(2, groups.Count());
			Assert.AreEqual("1, 2, 3, 4, 5", string.Join(", ", groups.First()));
			Assert.AreEqual("6, 7, 8, 9, 10", string.Join(", ", groups.Last()));
		}

		[Test]
		public void Can_Get_List_In_Groups_Of_More_Than_It_Contains()
		{
			var items = new List<string> { "1", "2", "3", "4" };
			var groups = items.InGroupsOf(100);
			Assert.AreEqual(1, groups.Count());
			Assert.AreEqual("1, 2, 3, 4", string.Join(", ", groups.First()));
		}

		[Test]
		public void Can_Get_Empty_List_In_Groups_Of_Four()
		{
			var items = new List<string>();
			var groups = items.InGroupsOf(4);
			Assert.AreEqual(0, groups.Count());
		}

		[Test]
		public void Can_Get_Fieldsets_In_Groups_Of_Two()
		{
			var sampleJson = File.ReadAllText("..\\..\\Data\\sample-1.json");
			var converter = new ArchetypeValueConverter();
			var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, sampleJson, false);

			Assert.AreEqual(2, result.Fieldsets.Count());
			var groups = result.Fieldsets.InGroupsOf(2);
			Assert.AreEqual(1, groups.Count());
			Assert.IsTrue(groups.First().First() == result.Fieldsets.First());
			Assert.IsTrue(groups.First().Last() == result.Fieldsets.Last());
		}

		[Test]
		public void Can_Get_Properties_In_Groups_Of_Two()
		{
			var sampleJson = File.ReadAllText("..\\..\\Data\\sample-1.json");
			var converter = new ArchetypeValueConverter();
			var result = (Archetype.Models.ArchetypeModel)converter.ConvertDataToSource(null, sampleJson, false);
			var properties = result.Fieldsets.First().Properties.ToList();

			Assert.AreEqual(4, properties.Count());
			var groups = properties.InGroupsOf(2);
			Assert.AreEqual(2, groups.Count());

			Assert.AreEqual(properties[0], groups.First().First());
			Assert.AreEqual(properties[1], groups.First().Last());
			Assert.AreEqual(properties[2], groups.Last().First());
			Assert.AreEqual(properties[3], groups.Last().Last());
		}
	}
}
