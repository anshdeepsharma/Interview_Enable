using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MSX.Transition.Providers.CRM.Contract
{
    public class OpportunityCds
    {
        [JsonPropertyName("msp_opportunitynumber")]
        [DataMember]
        public string? OpportunityNumber { get; set; }
    }

}
