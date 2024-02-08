using MSX.Assignment.Common.Infra.Clients;
using MSX.Common.Models.Enums;
using MSX.Transition.Providers.Contracts.v1.Provider;
using Transitions = MSX.Common.Models.Transitions;

namespace MSX.Transition.Business.Services
{
    public class AuditHistoryHandler : TransitionHandler
    {
        private readonly IAuditHistoryCRMProvider _auditHistoryCRMProvider;
        private readonly IPreviousSegmentGetter _previousSegmentGetter;
        private readonly IPreviousSubsegmentGetter _previousSubsegmentGetter;
        private readonly IAccountOwnerGetter _accountOwnerGetter;
        private readonly ICRMProvider _crmProvider;
        private readonly ITaxonomyServiceClient _taxonomyServiceClient;

        public AuditHistoryHandler(IAuditHistoryCRMProvider auditHistoryCRMProvider,
            IPreviousSegmentGetter previousSegmentGetter,
            IPreviousSubsegmentGetter previousSubsegmentGetter,
            IAccountOwnerGetter accountOwnerGetter,
            ICRMProvider crmProvider,
            ITaxonomyServiceClient taxonomyServiceClient)
        {
            _auditHistoryCRMProvider = auditHistoryCRMProvider;
            _previousSegmentGetter = previousSegmentGetter;
            _previousSubsegmentGetter = previousSubsegmentGetter;
            _accountOwnerGetter = accountOwnerGetter;
            _crmProvider = crmProvider;
            _taxonomyServiceClient = taxonomyServiceClient;
        }

        public override async Task ExecuteAsync(TransitionDataModel transitionDataModel, Transitions.Transition transition)
        {
            var accountAuditHistory = await _auditHistoryCRMProvider.GetAccountAuditHistoryAsync(transition.AccountId);

            transition.PreviousSegment = _previousSegmentGetter.Get(accountAuditHistory);
            if (transition.PreviousSegment != null && transition.PreviousSegment != string.Empty)
            {
                List<MSX.Common.Models.Taxonomy>? segments = await _taxonomyServiceClient.GetTaxonomy(TaxonomyType.Segment.ToString());

                if (segments != null)
                {
                    MSX.Common.Models.Taxonomy? segment = segments.Where(segment => segment.Key.Equals(transition.PreviousSegment)).FirstOrDefault();
                    if (segment != null)
                        transition.PreviousSegment = segment.Value;
                }
            }
            else
            {
                transition.PreviousSegment = transition.CurrentSegment;
            }

            transition.PreviousSubsegment = _previousSubsegmentGetter.Get(accountAuditHistory);
            if (transition.PreviousSubsegment != null && transition.PreviousSubsegment != string.Empty)
            {
                List<MSX.Common.Models.Taxonomy>? subSegments = await _taxonomyServiceClient.GetTaxonomy(TaxonomyType.SubSegment.ToString());

                if (subSegments != null)
                {
                    MSX.Common.Models.Taxonomy? subSegment = subSegments.Where(subSegment => subSegment.Key.Equals(transition.PreviousSubsegment)).FirstOrDefault();
                    if (subSegment != null)
                        transition.PreviousSubsegment = subSegment.Value;
                }
            }
            else
            {
                transition.PreviousSubsegment = transition.CurrentSubsegment;
            }

            transition.PreviousAccountOwner = _accountOwnerGetter.Get(accountAuditHistory);

            if (_nextHandler != null)
            {
                await _nextHandler.ExecuteAsync(transitionDataModel, transition);
            }
        }
    }
}
