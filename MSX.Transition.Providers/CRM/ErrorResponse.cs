using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MSX.Transition.Providers.CRM
{

    [DataContract]
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        [DataMember]
        public Error? Error { get; set; }
    }

    [DataContract]
    public class Error
    {
        [JsonPropertyName("code")]
        [DataMember]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        [DataMember]
        public string? Message { get; set; }
    }
}
