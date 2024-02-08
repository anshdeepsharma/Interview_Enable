using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.Responses.v1;
using MSX.Transition.Providers.Contracts.v1.Provider;
using System.Net;
using System.Text.Json;

namespace MSX.Transition.Providers.CRM
{
    public class CRMProvider : ICRMProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ICRMProvider> _logger;
        private readonly ICRMClient _crmClient;
        private readonly ICRMTranslator _crmTranslator;
        private readonly CRMConfig _crmConfig;

        public CRMProvider(IOptions<CRMConfig> crmConfig
            , IMemoryCache memoryCache
            , ILogger<ICRMProvider> logger
            , ICRMClient crmClient
            , ICRMTranslator crmTranslator)
        {
            _crmConfig = crmConfig.Value;
            _memoryCache = memoryCache;
            _logger = logger;
            _crmClient = crmClient;
            _crmTranslator = crmTranslator;
        }


        public BatchRequest CreateBatchRequest(HttpMethod httpMethod
            , string endpoint
            , string payload
            , string? contentId)
        {
            var batchRequest = new BatchRequest
            {
                BatchRequestId = Guid.NewGuid().ToString(),
                HttpMethod = httpMethod.ToString(),
                URL = _crmConfig.OrgURL + _crmConfig.APIBaseURL + endpoint,
                ContentId = contentId ?? string.Empty,
                Payload = payload
            };

            return batchRequest;
        }

        public async Task<T> ExecuteGetRequestAsync<T>(string strApiEndPoint)
        {
            HttpResponseMessage response = await _crmClient.ExecuteGetAPI(strApiEndPoint);
            string strResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(strResponse);
            }

            try
            {
                return JsonSerializer.Deserialize<T>(strResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deserializing response from CRM API. Response: {strResponse}");
                throw;
            }
        }
        public async Task<List<Response<BatchRequest>>> ExecuteBatchRequestAsync(List<BatchRequest> batchRequests)
        {

            List<BatchProcessResponse> batchProcessingResponses = await _crmClient.ProcessBatchRequests(batchRequests);

            List<Response<BatchRequest>> responses = new();
            int index = 0;

            foreach (BatchProcessResponse batchProcessResponse in batchProcessingResponses)
            {
                Response<BatchRequest> response = new(batchRequests[index]);
                if (batchProcessResponse.StatusCode >= 200 && batchProcessResponse.StatusCode < 300)
                {
                    string? id = batchProcessResponse.Response.Split('(', ')').Length > 1 ? batchProcessResponse.Response.Split('(', ')')[1] : null;

                    response.id = id ?? string.Empty;
                    response.Message = string.Empty;
                    response.SuccessMessage = Message.TransitionProcessSuccess;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Status = Status.Success;
                    _logger.LogInformation($"{Message.TransitionProcessSuccess} {response.SuccessMessage}");
                }
                else
                {
                    try
                    {
                        ErrorResponse? errorResponse = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(batchProcessResponse.Response);
                        if (errorResponse != null)
                        {
                            response.Message = string.Format(ErrorMessage.AccountTransitionFailed, "CREATE", response.Obj?.ContentId, errorResponse.Error?.Message);
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            response.Status = Status.Failed;

                            _logger.LogError($"{ErrorMessage.AccountTransitionFailed} {response.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = string.Format(ErrorMessage.AccountTransitionFailed, "CREATE", response.Obj?.ContentId, batchProcessResponse.Status + ex.Message);
                        _logger.LogError(ex, $"{ErrorMessage.AccountTransitionFailed} {response.Message}");
                    }
                }

                responses.Add(response);
                index++;
            }

            return responses;
        }

        public async Task<Guid?> ResolveObjectIdAsync(object? objInput
            , string inMemoryCachekey
            , FieldMapper? fieldMapper)
        {
            var result = (Guid?)_memoryCache.Get(inMemoryCachekey);
            if (result == null)
            {
                Guid? id = await ResolveObjectId(objInput, fieldMapper);

                if (id != null)
                {
                    result = id;
                    _memoryCache.Set(inMemoryCachekey, id, TimeSpan.FromDays(_crmConfig.MemoryCacheDurationInDays));
                }
            }
            return result;
        }

        private async Task<Guid?> ResolveObjectId(object? objInput, FieldMapper? fieldMapper)
        {
            if (objInput != null)
            {
                object Object = await _crmTranslator.Translate(objInput, fieldMapper);
                if (Object != null && Object.ToString()?.Split('(', ')').Length > 1)
                {
                    if (Guid.TryParse(Object.ToString()?.Split('(', ')')[1], out Guid id))
                        return id;
                }
            }

            return null;
        }
    }
}