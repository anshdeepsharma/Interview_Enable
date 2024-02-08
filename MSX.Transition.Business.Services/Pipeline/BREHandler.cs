using Microsoft.Extensions.Logging;
using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.ApplicationException;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class BREHandler : TransitionHandler
    {
        private readonly IBREServiceClient _breServiceClient;

        private readonly ILogger<BREHandler> _logger;
        public BREHandler(ILogger<BREHandler> logger
            , IBREServiceClient breServiceClient)
        {
            _logger = logger;
            _breServiceClient = breServiceClient;
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel
            , Transitions.Transition transition)
        {
            transitionDataModel.BREData = await _breServiceClient.Execute(transitionDataModel.BRERequest);

            var ruleName = transitionDataModel.CheckFlags.RuleName;
            var ruleOutput = transitionDataModel.BREData?.Response.Policies.Where(x => x.PolicyName == "Account Transition")
                .FirstOrDefault()?
                .Rules.Where(x => x.RuleName == ruleName)?.FirstOrDefault()?.RuleOutput;

            var areas = ruleOutput?.Area.Split('，');
            var previousSegments = ruleOutput?.Segment.Split('，');
            var currentSegments = ruleOutput?.FyTransitionSegment.Split('，');

            if (transitionDataModel.CheckFlags.CheckArea
                && areas != null
                && areas.Any()
                && !areas.Contains(transitionDataModel.Account?.Area))
            {
                throw new DomainException($"BRE rule check failed for Area: {transitionDataModel.Account?.Area}", System.Net.HttpStatusCode.NoContent);
            }

            if (transitionDataModel.CheckFlags.CheckPreviousSegment
                && previousSegments != null
                && previousSegments.Any()
                && !previousSegments.Contains(transition.PreviousSegment))
            {
                throw new DomainException($"BRE rule check failed for Previous Segment: {transition.PreviousSegment}", System.Net.HttpStatusCode.NoContent);
            }

            if (transitionDataModel.CheckFlags.CheckSegment
                && currentSegments != null
                && currentSegments.Any()
                && !currentSegments.Contains(transition.CurrentSegment))
            {
                throw new DomainException($"BRE rule check failed for Current Segment: {transition.CurrentSegment}", System.Net.HttpStatusCode.NoContent);
            }

            _logger.LogInformation("BRE rule check passed");

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
