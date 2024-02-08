using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MSX.Transition.Providers.CRM.Contract
{
    public class AuditHistoryCds
    {
        [JsonPropertyName("changedata")]
        [DataMember]
        public string? changedata { get; set; }
    }

}
