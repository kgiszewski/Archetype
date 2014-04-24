using System.Collections.Generic;
using Archetype.Umbraco.PropertyConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization
{
    [TestFixture]
    public class ArchetypeJsonConverterTest
    {
        private const string _CONTACT_DETAILS_JSON =
@"{
  ""fieldsets"": [
    {
      ""alias"": ""contactDetails"",
      ""properties"": [
        {
          ""alias"": ""name"",
          ""value"": ""Test""
        },
        {
          ""alias"": ""address"",
          ""value"": ""Test Address""
        },
        {
          ""alias"": ""telephone"",
          ""value"": ""111""
        },
        {
          ""alias"": ""mobile"",
          ""value"": ""000""
        },
        {
          ""alias"": ""fax"",
          ""value"": ""000""
        },
        {
          ""alias"": ""email"",
          ""value"": ""test@test.com""
        },
        {
          ""alias"": ""webSite"",
          ""value"": ""http://test.com""
        }
      ]
    }
  ]
}";

        private const string _HUB_DETAILS_JSON =
@"{
  ""fieldsets"": [
    {
      ""alias"": ""regionalHub"",
      ""properties"": [
        {
          ""alias"": ""region"",
          ""value"": ""Test Region""
        },
        {
          ""alias"": ""contactDetails"",
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""contactDetails"",
                ""properties"": [
                  {
                    ""alias"": ""name"",
                    ""value"": ""Test""
                  },
                  {
                    ""alias"": ""address"",
                    ""value"": ""Test Address""
                  },
                  {
                    ""alias"": ""telephone"",
                    ""value"": ""111""
                  },
                  {
                    ""alias"": ""mobile"",
                    ""value"": ""000""
                  },
                  {
                    ""alias"": ""fax"",
                    ""value"": ""000""
                  },
                  {
                    ""alias"": ""email"",
                    ""value"": ""test@test.com""
                  },
                  {
                    ""alias"": ""webSite"",
                    ""value"": ""http://test.com""
                  }
                ]
              }
            ]
          }
        }
      ]
    }
  ]
}";

        private const string _NULL_VALUES_JSON = @"{""fieldsets"":[{""alias"":""contactDetails"",""properties"":[{""alias"":""name"",""value"":""""},{""alias"":""address"",""value"":""""},{""alias"":""telephone"",""value"":""""},{""alias"":""mobile"",""value"":""""},{""alias"":""fax"",""value"":""""},{""alias"":""email"",""value"":""""},{""alias"":""webSite"",""value"":""""}]}]}";

        private ContactDetails _contactDetails;
        private CompanyDetails _companyDetails;

        [SetUp]
        public void SetUp()
        {
            _contactDetails = new ContactDetails
            {
                Address = "Test Address",
                Email = "test@test.com",
                Fax = "000",
                Mobile = "000",
                Name = "Test",
                Telephone = "111",
                WebSite = new UrlPicker
                {
                    Url = "http://test.com",
                    Title = "The Test Company",
                    OpenInNewWindow = true
                }
            };

            _companyDetails = new CompanyDetails
            {
                Region = "Test Region",
                ContactDetails = _contactDetails
            };
        }

        #region serialization tests

        [Test]
        public void ContactDetailsModel_Serializes_To_Archetype_Property()
        {
            var result = JsonConvert.SerializeObject(_contactDetails, Formatting.Indented);
            Assert.AreEqual(_CONTACT_DETAILS_JSON, result);
        }

        [Test]
        public void RegionalModel_Fieldset_Serializes_As_Expected()
        {
            var result = JsonConvert.SerializeObject(_companyDetails, Formatting.Indented);

            Assert.AreEqual(_HUB_DETAILS_JSON, result);

        }

        [Test]
        public void NullValues_Serialize_To_Empty_String()
        {
            var result = JsonConvert.SerializeObject(new ContactDetails());

            Assert.AreEqual(_NULL_VALUES_JSON, result);

        }

        [Test]
        public void ConvertModelToArchetype()
        {
            var model = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model);
            var archetype = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void ConvertCompoundModelToArchetype()
        {
            var item1 = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            var item2 = new ContactDetails
            {
                Address = "addr1",
                Name = "email2"
            };

            var model = new AllContactDetails
            {
                UserDetails = item1,
                AdminDetails = item2
            };

            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var archetype = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void ConvertEnumerableModelToArchetype()
        {
            var item1 = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            var item2 = new ContactDetails
            {
                Address = "addr1",
                Name = "email2"
            };

            var model = new ContactDetailsList
            {
                item1,
                item2
            };

            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var archetype = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        [Test]
        public void ConvertNestedModelToArchetype()
        {
            var model = new CompanyDetails
            {
                Region = "Test Region",
                ContactDetails = new ContactDetails
                {
                    Address = "addr1",
                    Name = "email2"
                }
            };

            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var archetype = (Archetype.Umbraco.Models.Archetype)converter.ConvertDataToSource(null, json, false);

            Assert.NotNull(archetype);
        }

        #endregion

        #region deserialization tests

        [Test]
        public void ConvertArchetypeToModel()
        {
            var result = JsonConvert.DeserializeObject<ContactDetails>(_CONTACT_DETAILS_JSON);

            Assert.NotNull(result);
            Assert.IsInstanceOf<ContactDetails>(result);

            Assert.AreEqual("Test Address", result.Address);
            Assert.AreEqual("test@test.com", result.Email);
            Assert.AreEqual("000", result.Fax);
            Assert.AreEqual("000", result.Mobile);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual("111", result.Telephone);
            Assert.AreEqual("http://test.com", result.WebSite);
        }

        [Test]
        public void DeserializeCompoundModelFromArchetype()
        {
            var item1 = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            var item2 = new ContactDetails
            {
                Address = "addr1",
                Name = "email2"
            };

            var model = new AllContactDetails
            {
                UserDetails = item1,
                AdminDetails = item2
            };

            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<AllContactDetails>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<AllContactDetails>(result);

            Assert.AreEqual(item1.Address, result.UserDetails.Address);
            Assert.AreEqual(item1.Email, result.UserDetails.Email);
            Assert.AreEqual(item2.Address, result.AdminDetails.Address);
            Assert.AreEqual(item2.Name, result.AdminDetails.Name);
        }

        [Test]
        public void DeserializeEnumerableModelFromArchetype()
        {
            var item1 = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            var item2 = new ContactDetails
            {
                Address = "addr1",
                Name = "email2"
            };

            var model = new ContactDetailsList
            {
                item1,
                item2
            };

            var json = JsonConvert.SerializeObject(model, Formatting.Indented);

            var result = JsonConvert.DeserializeObject<ContactDetailsList>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<ContactDetailsList>(result);
            Assert.AreEqual(result.Count, 2);

            Assert.AreEqual(item1.Address, result[0].Address);
            Assert.AreEqual(item1.Email, result[0].Email);
            Assert.AreEqual(item2.Address, result[1].Address);
            Assert.AreEqual(item2.Name, result[1].Name);
        }

        [Test]
        public void DeserializeNestedModelFromArchetype()
        {
            var model = new CompanyDetails
            {
                Region = "Test Region",
                ContactDetails = new ContactDetails
                {
                    Address = "addr1",
                    Name = "email2"
                }
            };

            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var result = JsonConvert.DeserializeObject<CompanyDetails>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<CompanyDetails>(result);

            Assert.AreEqual(model.Region, result.Region);
            Assert.AreEqual(model.ContactDetails.Address, result.ContactDetails.Address);
            Assert.AreEqual(model.ContactDetails.Name, result.ContactDetails.Name);
        }

        #endregion

    }
}
