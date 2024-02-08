using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSX.Common.Models;
using MSX.Common.Models.ApplicationException;
using MSX.Common.Models.Assignments;
using MSX.Common.Models.Responses.v1;
using MSX.Common.Models.Transitions;
using MSX.Transition.Business.Services;

namespace MSX.Transition.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "STS")]
    public class TransitionController : ControllerBase
    {
        private readonly ILogger<TransitionController> _logger;
        private readonly ITransitionService _transitionService;
        private readonly ITransitionTeamService _transitionTeamService;
        private readonly MSXRequestContext _msxRequestContext;

        public TransitionController(ILogger<TransitionController> logger
            , ITransitionService transitionService
            , ITransitionTeamService transitionTeamService
            , MSXRequestContext msxRequestContext)
        {
            _logger = logger;
            _transitionService = transitionService;
            _msxRequestContext = msxRequestContext;
            _transitionTeamService = transitionTeamService;
        }

        [HttpPost("CreateTransitions")]
        public async Task<List<Response<AccountTeam>>> CreateTransitions(List<AccountTeam> accountTeamAssignment)
        {
            _logger.LogInformation(Messages.RequestReceived, "CreateTransition", _msxRequestContext.CorrelationId);

            if (accountTeamAssignment == null || !accountTeamAssignment.Any())
            {
                throw new DomainException("No assignments to process to create/update transitions");
            }

            return await _transitionService.CreateTransitionsAsync(accountTeamAssignment);
        }

        [HttpPost("CreateTransitionTeam")]
        public async Task<List<Response<TransitionTeam>>> CreateTransitionTeam(List<TransitionTeam> transitionTeam)
        {
            _logger.LogInformation(Messages.RequestReceived, "CreateTransitionTeam", _msxRequestContext.CorrelationId);

            return await _transitionTeamService.CreateTransitionTeamsAsync(transitionTeam);
        }
    }
}
