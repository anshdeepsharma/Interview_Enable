using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MSX.Transition.Providers.CRM.Contract
{
    public class TransitionCds
    {
        [JsonPropertyName("msp_relationshipmanagementid")]
        [DataMember]
        public string? Id { get; set; }

        [JsonPropertyName("msp_previoussubsegment")]
        [DataMember]
        public string? PreviousSubsegment { get; set; }

        [JsonPropertyName("msp_previoussegment")]
        [DataMember]
        public string? PreviousSegment { get; set; }

        [JsonPropertyName("_msp_accountid_value")]
        [DataMember]
        public string? AccountId { get; set; }

        [JsonPropertyName("msp_accounttransitiiontype@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? AccountTransitiionType { get; set; }

        [JsonPropertyName("msp_activitystatuscode@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? ActivityStatusCode { get; set; }

        [JsonPropertyName("msp_currentsegment")]
        [DataMember]
        public string? CurrentSegment { get; set; }

        [JsonPropertyName("msp_currentsubsegment")]
        [DataMember]
        public string? CurrentSubsegment { get; set; }

        [JsonPropertyName("msp_name")]
        [DataMember]
        public string? Name { get; set; }

        [JsonPropertyName("msp_previousaccountowner")]
        [DataMember]
        public string? PreviousAccountOwner { get; set; }

        [JsonPropertyName("_msp_incomingats_value")]
        [DataMember]
        public string? IncomingATS { get; set; }

        [JsonPropertyName("_msp_outgoingats_value")]
        [DataMember]
        public string? OutgoingATS { get; set; }

        [JsonPropertyName("_msp_incomingssp_value")]
        [DataMember]
        public string? IncomingSSP { get; set; }

        [JsonPropertyName("_msp_outgoingssp_value")]
        [DataMember]
        public string? OutgoingSSP { get; set; }

        [JsonPropertyName("msp_manager")]
        [DataMember]
        public string? Manager { get; set; }

        [JsonPropertyName("msp_transitionsolutionarea")]
        [DataMember]
        public string? SolutionArea { get; set; }

        [JsonPropertyName("_ownerid_value")]
        [DataMember]
        public string? OwnerId { get; set; }

        [JsonPropertyName("createdon")]
        [DataMember]
        public string? CreatedOn { get; set; }

        // Transition Team attributes

        [JsonPropertyName("team.activityid")]
        [DataMember]
        public string? TeamId { get; set; }

        [JsonPropertyName("team.msp_role@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? TeamRole { get; set; }

        [JsonPropertyName("team.ownerid")]
        [DataMember]
        public string? TeamOwnerId { get; set; }

        [JsonPropertyName("team.msp_memberid")]
        [DataMember]
        public string? TeamMemberId { get; set; }

        [JsonPropertyName("team.msp_transitionmode@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? TeamTransitionMode { get; set; }

        [JsonPropertyName("team.msp_solutionarea")]
        [DataMember]
        public string? TeamSolutionArea { get; set; }

    }

}
