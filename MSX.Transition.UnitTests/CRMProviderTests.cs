using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.Responses.v1;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MSX.Transition.Providers.CRM;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace MSX.Transition.UnitTests
{
    public class CRMProviderTests
    {
        private CRMProvider _crmProvider;
        private Mock<IMemoryCache> _memoryCacheMock;
        private Mock<ILogger<ICRMProvider>> _loggerMock;
        private Mock<ICRMClient> _crmClientMock;
        private Mock<ICRMTranslator> _crmTranslatorMock;
        private Mock<ICRMXMLHandler> _crmXMLHandlerMock;
        private Mock<IOptions<CRMConfig>> _crmConfigMock;

        [SetUp]
        public void Setup()
        {
            _memoryCacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<ICRMProvider>>();
            _crmClientMock = new Mock<ICRMClient>();
            _crmTranslatorMock = new Mock<ICRMTranslator>();
            _crmConfigMock = new Mock<IOptions<CRMConfig>>();
            var options = Options.Create(new MemoryCacheOptions());
            var memoryCache = new MemoryCache(options);

            _crmConfigMock.Setup(config => config.Value).Returns(new CRMConfig
            {
                EntityName = "Test",
                OrgURL = "https://example.com",
                APIBaseURL = "/api/",
                MemoryCacheDurationInDays = 1,
            });

            _crmProvider = new CRMProvider(
                _crmConfigMock.Object,
                memoryCache,
                _loggerMock.Object,
                _crmClientMock.Object,
                _crmTranslatorMock.Object
            ); 
        }

        [Test]
        public async Task CreateBatchRequest_ReturnsBatchRequest()
        {
            // Arrange
            var httpMethod = HttpMethod.Get;
            var endpoint = "/api/endpoint";
            var payload = "payload";
            var contentId = "contentId";

            // Act
            var result = _crmProvider.CreateBatchRequest(httpMethod, endpoint, payload, contentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(httpMethod.ToString(), result.HttpMethod);
            Assert.AreEqual(_crmConfigMock.Object.Value?.OrgURL + _crmConfigMock.Object.Value?.APIBaseURL + endpoint, result.URL);
            Assert.AreEqual(payload, result.Payload);
        }

        [Test]
        public async Task ExecuteGetRequest_ThrowsException_WhenResponseIsNotSuccessful()
        {
            // Arrange
            var strApiEndPoint = "/api/endpoint";
            var response = new HttpResponseMessage
            {
                Content = new StringContent("error"),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            _crmClientMock.Setup(c => c.ExecuteGetAPI(strApiEndPoint)).ReturnsAsync(response);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _crmProvider.ExecuteGetRequestAsync<string>(strApiEndPoint));
        }

        [Test]
        public async Task ResolveObjectId_ReturnsCachedResult_WhenResultExistsInMemoryCache()
        {
            // Arrange
            var objInput = new object();
            var inMemoryCachekey = "cacheKey";
            var fieldMapper = new FieldMapper();
            var expectedResult = Guid.NewGuid();
            JObject validJObject = new JObject();
            validJObject.Add("id", "("+expectedResult.ToString()+")");
            _crmTranslatorMock.Setup(t => t.Translate(objInput, fieldMapper)).ReturnsAsync(validJObject);
            // Act
            var result = await _crmProvider.ResolveObjectIdAsync(objInput, inMemoryCachekey, fieldMapper);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task ResolveObjectId_ReturnsNull_WhenResultDoesNotExistInMemoryCache()
        {
            // Arrange
            var objInput = new object();
            var inMemoryCachekey = "cacheKey";
            var fieldMapper = new FieldMapper();
            JObject emptyJObject = new JObject();
            _crmTranslatorMock.Setup(t => t.Translate(objInput, fieldMapper)).ReturnsAsync(emptyJObject);

            // Act
            var result = await _crmProvider.ResolveObjectIdAsync(objInput, inMemoryCachekey, fieldMapper);

            // Assert
            Assert.IsNull(result);
        }
    }
}
