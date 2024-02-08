using Microsoft.Extensions.Logging;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Common;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Models;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;
using MSX.Transition.Providers.Contracts.v1.Provider;
using System.Net;

namespace MSX.Transition.Providers.CRM
{
    public class TransitionTeamCRMProvider : ITransitionTeamCRMProvider
    {
        private readonly ILogger<ITransitionTeamCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;
        private readonly ICRMTranslator _crmTranslator;

        const string map = "\\Mapper\\";
        private static readonly FieldMapper? TransitionTeamFieldMapper = System.Text.Json.JsonSerializer
            .Deserialize<FieldMapper>(XMLHandler.ReadXMLFile(map + "TransitionTeamFieldMapper.json"));


        public TransitionTeamCRMProvider(ILogger<ITransitionTeamCRMProvider> logger,
            ICRMProvider crmProvider
            , ICRMTranslator cRMTranslator)
        {
            _logger = logger;
            _crmProvider = crmProvider;
            _crmTranslator = cRMTranslator;
        }

        private async Task<BatchRequest> CreateBatchRequest(TransitionTeam transitionTeam)
        {
            HttpMethod method = HttpMethod.Post;
            string endpoint = "msp_transitioningteams";

            var obj = await _crmTranslator.Translate(transitionTeam, TransitionTeamFieldMapper);
            var payload = obj.ToString();
            payload = payload.Replace("msp_associatedtransition@odata.bind", "msp_AssociatedTransition_msp_transitioningteam@odata.bind");
            payload = payload.Replace("msp_memberid@odata.bind", "msp_memberId_msp_transitioningteam@odata.bind");

            if (transitionTeam?.Id != null && transitionTeam.Id != Guid.Empty)
            {
                method = HttpMethod.Patch;
                endpoint += "(" + transitionTeam?.Id.ToString() + ")";
            }

            return _crmProvider.CreateBatchRequest(method, endpoint, payload, transitionTeam.EventId + transitionTeam.TransitionId + transitionTeam.MemberId);
        }

        public async Task<List<Response<TransitionTeam>>> CreateTransitionTeamsAsync(List<TransitionTeam> transitionTeams)
        {
            List<Task<BatchRequest>> tasks = new List<Task<BatchRequest>>();
            Response<TransitionTeam>[] responses = new Response<TransitionTeam>[transitionTeams.Count];
            int index = 0;

            foreach (var team in transitionTeams)
            {
                Response<TransitionTeam> response = new Response<TransitionTeam>(team);
                responses[index++] = response;

                tasks.Add(CreateBatchRequest(team));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some transition team batch request tasks have exceptions");
            }

            List<BatchRequest> batchRequests = new List<BatchRequest>();
            index = 0;

            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (task.Result != null)
                        batchRequests.Add(task.Result);
                }
                else
                {
                    responses[index].Message = task.Exception?.InnerException?.Message!;

                    if (task.Exception?.InnerException != null && task.Exception?.InnerException is DomainException)
                        responses[index].StatusCode = (int)((DomainException)task.Exception?.InnerException!).StatusCode;
                    else
                        responses[index].StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                index++;
            }

            List<Response<BatchRequest>> crmResponses = await _crmProvider.ExecuteBatchRequestAsync(batchRequests);

            foreach (Response<BatchRequest> crmResponse in crmResponses)
            {
                if (crmResponse.Obj?.ContentId != null)
                {
                    TransitionTeam? transitionTeam = transitionTeams.Where(t => (t.EventId + t.TransitionId + t.MemberId).Equals(crmResponse.Obj?.ContentId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (transitionTeam != null)
                    {
                        if (crmResponse.id != null)
                            transitionTeam.Id = Guid.Parse(crmResponse.id);


                        Response<TransitionTeam>? res = responses.Where(r => transitionTeam.EventId.Equals(r.Obj?.EventId, StringComparison.InvariantCultureIgnoreCase)
                                                                            && transitionTeam.TransitionId.Equals(r.Obj.TransitionId, StringComparison.InvariantCultureIgnoreCase)
                                                                            && transitionTeam.MemberId.Equals(r.Obj.MemberId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        if (res != null)
                        {
                            res.SuccessMessage = crmResponse.SuccessMessage;
                            res.Message = crmResponse.Message;
                            res.StatusCode = crmResponse.StatusCode;
                            res.Status = crmResponse.Status;
                        }
                    }
                }
            }

            return responses.ToList();
        }
    }
}