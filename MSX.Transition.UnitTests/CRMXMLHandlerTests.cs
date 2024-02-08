using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MSX.Common.Models.Enums;
using MSX.Common.Models.Requests.v1;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using System.Reflection;
using MSX.Transition.Providers.CRM;

namespace MSX.Transition.UnitTests
{
    [TestFixture]
    public class CRMXMLHandlerTests
    {
        private CRMXMLHandler _crmXmlHandler;

        [SetUp]
        public void Setup()
        {
            _crmXmlHandler = new CRMXMLHandler();
        }

        [Test]
        public void ConstructQuery_ValidData_ReturnsQuery()
        {
            // Arrange
            string entity = "Account";
            string fetchXML = "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\" count=\"50\" page=\"1\">\r\n  <entity name=\"account\">\r\n    <attribute name=\"accountid\" />\r\n    <attribute name=\"name\" />\r\n    <attribute name=\"telephone1\" />\r\n    <attribute name=\"websiteurl\" />\r\n    <attribute name=\"description\" />\r\n    <attribute name=\"address1_line1\" />\r\n    <attribute name=\"address1_line2\" />\r\n    <attribute name=\"address1_line3\" />\r\n    <attribute name=\"address1_city\" />\r\n    <attribute name=\"address1_stateorprovince\" />\r\n    <attribute name=\"address1_postalcode\" />\r\n    <attribute name=\"address1_country\" />\r\n    <attribute name=\"createdon\" />\r\n    <attribute name=\"modifiedon\" />\r\n    <order attribute=\"name\" descending=\"false\" />\r\n    <filter type=\"and\">\r\n      <condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />\r\n    </filter>\r\n  </entity>\r\n</fetch>";
            string mappingKey = "Account";
            string selectAttributes = "accountid,name,telephone1,websiteurl,description,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,createdon,modifiedon";
            AccountSearchRequest data = new AccountSearchRequest()
            {
                SearchValues = new Common.Models.Accounts.Account()
                {
                    Data = new Common.Models.Accounts.Data()
                    {
                        CrmAccountId = "123"
                    }
                }
            };
            string expectedQuery = "Account?fetchXml=<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\" count=\"50\" page=\"1\">\r\n  <entity name=\"account\">\r\n    <attribute name=\"accountid\" />\r\n    <attribute name=\"name\" />\r\n    <attribute name=\"telephone1\" />\r\n    <attribute name=\"websiteurl\" />\r\n    <attribute name=\"description\" />\r\n    <attribute name=\"address1_line1\" />\r\n    <attribute name=\"address1_line2\" />\r\n    <attribute name=\"address1_line3\" />\r\n    <attribute name=\"address1_city\" />\r\n    <attribute name=\"address1_stateorprovince\" />\r\n    <attribute name=\"address1_postalcode\" />\r\n    <attribute name=\"address1_country\" />\r\n    <attribute name=\"createdon\" />\r\n    <attribute name=\"modifiedon\" />\r\n    <order attribute=\"name\" descending=\"false\" />\r\n    <filter type=\"and\">\r\n      <condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />\r\n    </filter>\r\n  </entity>\r\n</fetch>";

            // Act
            string query = _crmXmlHandler.ConstructQuery(entity, fetchXML, mappingKey, selectAttributes, new Dictionary<string,string>(),data);

            // Assert
            Assert.AreEqual(expectedQuery, query);
        }

        /* [Test]
         public void GetColumnsXML_ValidColumns_ReturnsColumnsXML()
         {
             // Arrange
             string columns = "accountid,name,telephone1,websiteurl,description,address1_line1,address1_line2,address1_line3,address1_city,address1_stateorprovince,address1_postalcode,address1_country,createdon,modifiedon";
             string expectedColumnsXML = "<attribute name= \"accountid\" /> \n<attribute name= \"name\" /> \n<attribute name= \"telephone1\" /> \n<attribute name= \"websiteurl\" /> \n<attribute name= \"description\" /> \n<attribute name= \"address1_line1\" /> \n<attribute name= \"address1_line2\" /> \n<attribute name= \"address1_line3\" /> \n<attribute name= \"address1_city\" /> \n<attribute name= \"address1_stateorprovince\" /> \n<attribute name= \"address1_postalcode\" /> \n<attribute name= \"address1_country\" /> \n<attribute name= \"createdon\" /> \n<attribute name= \"modifiedon\" /> \n";

             // Act
             string columnsXML = _crmXmlHandler.GetColumnsXML(columns);

             // Assert
             Assert.AreEqual(expectedColumnsXML, columnsXML);
         }*/

        /* [Test]
         public void ConstructCondition_ValidData_ReturnsCondition()
         {
             // Arrange
             string key = "Account";
             AccountSearchRequest data = new AccountSearchRequest()
             {
                 StateCode = 0
             };
             string expectedCondition = "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";

             // Act
             string condition = _crmXmlHandler.ConstructCondition<AccountSearchRequest>(key, data);

             // Assert
             Assert.AreEqual(expectedCondition, condition);
         }*/

       /* [Test]
        public void GetFetchPageAttribute_ValidRequest_ReturnsFetchPageAttribute()
        {
            // Arrange
            AccountSearchRequest request = new AccountSearchRequest()
            {
                PageNumber = 1,
                NumberOfRecords = 5000
            };
            string expectedFetchPageAttribute = "page=\"1\" count=\"5000\"";

            // Act
            string fetchPageAttribute = _crmXmlHandler.GetFetchPageAttribute(request);

            // Assert
            Assert.AreEqual(expectedFetchPageAttribute, fetchPageAttribute);
        }*/
    }
}
