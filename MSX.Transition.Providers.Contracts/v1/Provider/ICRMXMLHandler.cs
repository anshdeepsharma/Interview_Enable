
namespace MSX.Transition.Providers.Contracts.v1.Provider
{
    public interface ICRMXMLHandler
    {
        string ConstructQuery<T>(string entity, string fetchXML, string mappingKey, string selectAttributes, Dictionary<string, string> attributeMapping, T data);
    }
}
