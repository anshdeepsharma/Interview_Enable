using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.Responses.v1;

namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface ICRMProvider
    {
        Task<T> ExecuteGetRequestAsync<T>(string strApiEndPoint);
        Task<List<Response<BatchRequest>>> ExecuteBatchRequestAsync(List<BatchRequest> batchRequests);
        BatchRequest CreateBatchRequest(HttpMethod httpMethod
            , string endpoint
            , string payload
            , string contentId);
        Task<Guid?> ResolveObjectIdAsync(object? objInput
            , string inMemoryCachekey
            , FieldMapper? fieldMapper);
    }
}
