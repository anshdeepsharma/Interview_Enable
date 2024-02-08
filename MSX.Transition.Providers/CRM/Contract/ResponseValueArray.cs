using System.Runtime.Serialization;

namespace MSX.Transition.Providers.CRM.Contract
{
    [DataContract]
    public class ResponseValueArray<T>
    {
        [DataMember(IsRequired = false, Name = "value")]
        public List<T> value { get; set; }
    }
}
