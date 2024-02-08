using MSX.Transition.Business.Services;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM;

namespace MSX.Transition.API
{
    public static class DIContainter
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICRMProvider, CRMProvider>();
            services.AddSingleton<ICRMXMLHandler, CRMXMLHandler>();
            services.AddScoped<IAccountCRMProvider, AccountCRMProvider>();
            services.AddScoped<IOpportunityCRMProvider, OpportunityCRMProvider>();
            services.AddScoped<IAssignmentCRMProvider, AssignmentCRMProvider>();
            services.AddScoped<IAccountPlanCRMProvider, AccountPlanCRMProvider>();
            services.AddScoped<IAuditHistoryCRMProvider, AuditHistoryCRMProvider>();
            services.AddScoped<IUserCRMProvider, UserCRMProvider>();
            services.AddScoped<ITransitionTeamCRMProvider, TransitionTeamCRMProvider>();
            services.AddScoped<IGrantAccessCRMProvider, GrantAccessCRMProvider>();
            services.AddScoped<ITransitionCRMProvider, TransitionCRMProvider>();

            services.AddScoped<ITransitionService, TransitionService>();
            services.AddScoped<ITransitionTeamService, TransitionTeamService>();
            services.AddScoped<IGrantAccessService, GrantAccessService>();
            services.AddScoped<IPreviousSegmentGetter, PreviousSegmentGetter>();
            services.AddScoped<IPreviousSubsegmentGetter, PreviousSubsegmentGetter>();
            services.AddScoped<IAccountOwnerGetter, AccountOwnerGetter>();



            //transition types
            services.AddScoped<ITransitionFactory, TransitionFactory>();

            services.AddScoped<ATSTransitionService>()
                        .AddScoped<ITransitionTypeService, ATSTransitionService>(s => s.GetService<ATSTransitionService>());

            services.AddScoped<SSPTransitionService>()
                        .AddScoped<ITransitionTypeService, SSPTransitionService>(s => s.GetService<SSPTransitionService>());

            //Chain of reponsibility
            services.AddScoped<AccountHandler>();
            services.AddScoped<OpportunityHandler>();
            services.AddScoped<AssignmentHandler>();
            services.AddScoped<IAssignmentEventandler, AssignmentEventHandler>();
            services.AddScoped<CreateAssignmentEventHandler>();
            services.AddScoped<DeleteAssignmentEventHandler>();
            services.AddScoped<AuditHistoryHandler>();
            services.AddScoped<BREHandler>();
            services.AddScoped<ITranstionHandler, TransitionHandler>();
            services.AddScoped<ExistingTransitionsHandler>();
        
        }
    }
}
