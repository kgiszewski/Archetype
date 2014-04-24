using System;
using System.Collections.Generic;
using Archetype.Umbraco.PropertyConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization
{
    [TestFixture]
    public class ArchetypeJsonConverterTest
    {
        private const string _ANNUAL_STATEMENT_JSON =
@"{
  ""fieldsets"": [
    {
      ""alias"": ""annualStatement"",
      ""properties"": [
        {
          ""alias"": ""FiscalYearStart"",
          ""value"": ""01/09/2013 00:00:00""
        },
        {
          ""alias"": ""FiscalYearEnd"",
          ""value"": ""31/08/2014 00:00:00""
        },
        {
          ""alias"": ""DividendPaymentDate"",
          ""value"": """"
        },
        {
          ""alias"": ""TotalShares"",
          ""value"": ""345678""
        },
        {
          ""alias"": ""Sales"",
          ""value"": ""123456700.89""
        },
        {
          ""alias"": ""Profit"",
          ""value"": ""1123456.78""
        }
      ]
    }
  ]
}";

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
          ""value"": {
            ""fieldsets"": [
              {
                ""alias"": ""urlPicker"",
                ""properties"": [
                  {
                    ""alias"": ""title"",
                    ""value"": ""The Test Company""
                  },
                  {
                    ""alias"": ""url"",
                    ""value"": ""http://test.com""
                  },
                  {
                    ""alias"": ""content"",
                    ""value"": """"
                  },
                  {
                    ""alias"": ""media"",
                    ""value"": """"
                  },
                  {
                    ""alias"": ""openInNewWindow"",
                    ""value"": ""1""
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

        private const string _COMPANY_DETAILS_JSON =
@"{
  ""fieldsets"": [
    {
      ""alias"": ""companyDetails"",
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
                    ""value"": {
                      ""fieldsets"": [
                        {
                          ""alias"": ""urlPicker"",
                          ""properties"": [
                            {
                              ""alias"": ""title"",
                              ""value"": ""The Test Company""
                            },
                            {
                              ""alias"": ""url"",
                              ""value"": ""http://test.com""
                            },
                            {
                              ""alias"": ""content"",
                              ""value"": """"
                            },
                            {
                              ""alias"": ""media"",
                              ""value"": """"
                            },
                            {
                              ""alias"": ""openInNewWindow"",
                              ""value"": ""1""
                            }
                          ]
                        }
                      ]
                    }
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
        private AnnualStatement _annualStatement;
        private UrlPicker _webSite;

        [SetUp]
        public void SetUp()
        {
            _webSite = new UrlPicker
            {
                Url = "http://test.com",
                Title = "The Test Company",
                OpenInNewWindow = true
            };
            
            _contactDetails = new ContactDetails
            {
                Address = "Test Address",
                Email = "test@test.com",
                Fax = "000",
                Mobile = "000",
                Name = "Test",
                Telephone = "111",
                WebSite = _webSite
            };

            _companyDetails = new CompanyDetails
            {
                Region = "Test Region",
                ContactDetails = _contactDetails
            };

            _annualStatement = new AnnualStatement
            {
                FiscalYearStart = new DateTime(2013, 9, 1),
                FiscalYearEnd = new DateTime(2014, 8, 31),
                TotalShares = 345678,
                Sales = 123456700.89,
                Profit = 1123456.78m
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
        public void CompanyDetailsModel_Fieldset_Serializes_As_Expected()
        {
            var result = JsonConvert.SerializeObject(_companyDetails, Formatting.Indented);

            Assert.AreEqual(_COMPANY_DETAILS_JSON, result);

        }

        [Test]
        public void AnnualStatementModel_Fieldset_Serializes_As_Expected()
        {
            var result = JsonConvert.SerializeObject(_annualStatement, Formatting.Indented);

            Assert.AreEqual(_ANNUAL_STATEMENT_JSON, result);

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
        public void ConvertNumericAndDateModelToArchetype()
        {
            var converter = new ArchetypeValueConverter();
            var json = JsonConvert.SerializeObject(_annualStatement);
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
        public void DeserializeModelFromArchetype()
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
            Assert.AreEqual(_webSite.Content, result.WebSite.Content);
            Assert.AreEqual(_webSite.Media, result.WebSite.Media);
            Assert.AreEqual(_webSite.OpenInNewWindow, result.WebSite.OpenInNewWindow);
            Assert.AreEqual(_webSite.Title, result.WebSite.Title);
            Assert.AreEqual(_webSite.Url, result.WebSite.Url);
        }

        [Test]
        public void DeserializeNumericAndDateModelFromArchetype()
        {
            var result = JsonConvert.DeserializeObject<AnnualStatement>(_ANNUAL_STATEMENT_JSON);

            Assert.NotNull(result);
            Assert.IsInstanceOf<AnnualStatement>(result);

            Assert.AreEqual(new DateTime(2013, 9, 1), result.FiscalYearStart);
            Assert.AreEqual(new DateTime(2014, 8, 31), result.FiscalYearEnd);
            Assert.IsNull(result.DividendPaymentDate);
            Assert.AreEqual(345678, result.TotalShares);
            Assert.AreEqual(123456700.89, result.Sales);
            Assert.AreEqual(1123456.78m, result.Profit);
        }

        [Test]
        public void DeserializeNumericAndDateModel_NullableDate_HasValue_FromArchetype()
        {
            var annualStatement = new AnnualStatement
            {
                FiscalYearStart = new DateTime(2013, 9, 1),
                FiscalYearEnd = new DateTime(2014, 8, 31),
                DividendPaymentDate = new DateTime(2014,9,15,10,15,30),
                TotalShares = 345678,
                Sales = 123456700.89,
                Profit = 1123456.78m
            };

            var json = JsonConvert.SerializeObject(annualStatement);
            var result = JsonConvert.DeserializeObject<AnnualStatement>(json);

            Assert.NotNull(result);
            Assert.IsInstanceOf<AnnualStatement>(result);

            Assert.AreEqual(annualStatement.FiscalYearStart, result.FiscalYearStart);
            Assert.AreEqual(annualStatement.FiscalYearEnd, result.FiscalYearEnd);
            Assert.AreEqual(annualStatement.DividendPaymentDate, result.DividendPaymentDate);
            Assert.AreEqual(annualStatement.TotalShares, result.TotalShares);
            Assert.AreEqual(annualStatement.Sales, result.Sales);
            Assert.AreEqual(annualStatement.Profit, result.Profit);
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
