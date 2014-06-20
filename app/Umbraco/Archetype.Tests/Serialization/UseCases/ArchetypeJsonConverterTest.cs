using System;
using Archetype.Tests.Serialization.Base;
using Archetype.PropertyConverters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Archetype.Tests.Serialization.UseCases
{
    [TestFixture]
    public class ArchetypeJsonConverterTest : ArchetypeJsonConverterTestBase
    {
        private ContactDetails _contactDetails;
        private CompanyDetails _companyDetails;
        private AnnualStatement _annualStatement;
        private MergerDetails _mergerDetails;
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

            _mergerDetails = new MergerDetails
            {
                MergerDate = new DateTime(2014,6,7,8,9,10),
                MergerValue = 12345676890.12m
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
        public void Convert_MergerDetailsModel_To_ArchetypeJson()
        {
            var result = ConvertModelToArchetypeJson(_mergerDetails, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._MERGER_DETAILS_JSON, result);
        }        
        
        [Test]
        public void Convert_ContactDetailsModel_To_ArchetypeJson()
        {
            var result = ConvertModelToArchetypeJson(_contactDetails, Formatting.Indented);
            Assert.AreEqual(JsonTestStrings._CONTACT_DETAILS_JSON, result);
        }

        [Test]
        public void Convert_CompanyDetailsModel_To_ArchetypeJson()
        {
            var result = ConvertModelToArchetypeJson(_companyDetails, Formatting.Indented);

            Assert.AreEqual(JsonTestStrings._COMPANY_DETAILS_JSON, result);

        }

        [Test]
        public void Convert_AnnualStatementModel_To_ArchetypeJson()
        {
            var result = ConvertModelToArchetypeJson(_annualStatement, Formatting.Indented);

            Assert.AreEqual(JsonTestStrings._ANNUAL_STATEMENT_JSON, result);

        }

        [Test]
        public void NullValues_Serialize_To_Empty_String()
        {
            var result = ConvertModelToArchetypeJson(new ContactDetails());

            Assert.AreEqual(JsonTestStrings._NULL_VALUES_JSON, result);

        }

        [Test]
        public void Convert_ContactDetailsModel_To_Archetype()
        {
            var model = new ContactDetails
            {
                Address = "addr",
                Email = "email"
            };

            Assert.NotNull(ConvertModelToArchetype(model));
        }

        [Test]
        public void Convert_AnnualStatementModel_To_Archetype()
        {
            Assert.NotNull(ConvertModelToArchetype(_annualStatement));
        }

        [Test]
        public void Convert_AllContactDetailsModel_To_Archetype()
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

            Assert.NotNull(ConvertModelToArchetype(model));
        }

        [Test]
        public void Convert_ContactDetailsListModel_To_Archetype()
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

            Assert.NotNull(ConvertModelToArchetype(model));
        }

        [Test]
        public void Convert_CompanyDetailsModel_To_Archetype()
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

            Assert.NotNull(ConvertModelToArchetype(model));
        }

        #endregion

        #region deserialization tests

        [Test]
        public void Convert_ArchetypeJson_To_ContactDetailsModel()
        {
            var result = ConvertArchetypeJsonToModel<ContactDetails>(JsonTestStrings._CONTACT_DETAILS_JSON);

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
        public void Convert_ArchetypeJson_To_AnnualStatementModel()
        {
            var result = ConvertArchetypeJsonToModel<AnnualStatement>(JsonTestStrings._ANNUAL_STATEMENT_JSON);

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
        public void Convert_ArchetypeJson_To_AnnualStatementModel_NullableDate_HasValue()
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

            var result = ConvertModelToArchetypeAndBack(annualStatement);

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
        public void Convert_ArchetypeJson_To_AllContactDetailsModel()
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

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.NotNull(result);
            Assert.IsInstanceOf<AllContactDetails>(result);

            Assert.AreEqual(item1.Address, result.UserDetails.Address);
            Assert.AreEqual(item1.Email, result.UserDetails.Email);
            Assert.AreEqual(item2.Address, result.AdminDetails.Address);
            Assert.AreEqual(item2.Name, result.AdminDetails.Name);
        }

        [Test]
        public void Convert_ArchetypeJson_To_ContactDetailsListModel()
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

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.NotNull(result);
            Assert.IsInstanceOf<ContactDetailsList>(result);
            Assert.AreEqual(result.Count, 2);

            Assert.AreEqual(item1.Address, result[0].Address);
            Assert.AreEqual(item1.Email, result[0].Email);
            Assert.AreEqual(item2.Address, result[1].Address);
            Assert.AreEqual(item2.Name, result[1].Name);
        }

        [Test]
        public void Convert_ArchetypeJson_To_CompanyDetailsModel()
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

            var result = ConvertModelToArchetypeAndBack(model);

            Assert.NotNull(result);
            Assert.IsInstanceOf<CompanyDetails>(result);

            Assert.AreEqual(model.Region, result.Region);
            Assert.AreEqual(model.ContactDetails.Address, result.ContactDetails.Address);
            Assert.AreEqual(model.ContactDetails.Name, result.ContactDetails.Name);
        }

        #endregion

    }
}
