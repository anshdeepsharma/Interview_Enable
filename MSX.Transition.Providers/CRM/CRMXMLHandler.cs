using MSX.Common.Models.Requests.v1;
using MSX.Transition.Providers.Contracts.v1.Provider;
using System.Reflection;
using System.Text;

namespace MSX.Transition.Providers.CRM
{
    public class CRMXMLHandler : ICRMXMLHandler
    {
        public string ConstructQuery<T>(string entity, string fetchXML, string mappingKey, string selectAttributes, Dictionary<string, string> attributeMapping, T data)
        {
            fetchXML = fetchXML.Replace("<" + mappingKey.ToLower() + "_columnlist/>", GetColumnsXML(selectAttributes));

            string condition = ConstructCondition<T>(mappingKey, data, attributeMapping);
            string fetchXml = fetchXML.Replace("<MORECONDITIONS/>", condition);

            fetchXml = fetchXml.Replace("GSXPAGEDETAIL=\"\"", GetFetchPageAttribute(null));

            string queryString = entity + Constants.FetchXMLQuery;
            string query = string.Format(queryString, fetchXml);

            return query;
        }

        private string? GetColumnsXML(string columns)
        {
            if (columns == null)
                return columns;

            StringBuilder stringBuilder = new StringBuilder(50);
            string format = "<attribute name= \"{0}\" /> \n";
            string[]? columnsArray = columns.Split(",");
            if (columnsArray != null)
            {
                foreach (string columnName in columnsArray)
                {
                    stringBuilder = stringBuilder.Append(string.Format(format, columnName.ToLower()));
                }
            }
            return stringBuilder.ToString();
        }
        private string ConstructCondition<T>(string key, T data, Dictionary<string, string> attributeMapping)
        {
            StringBuilder stringBuilder = new(50);
            PropertyInfo[] properties = data.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (attributeMapping.ContainsKey(propertyInfo.Name))
                {
                    string targetAttribute = attributeMapping[propertyInfo.Name];
                    if (targetAttribute != null)
                    {
                        object? value = propertyInfo.GetValue(data);
                        if (value != null)
                        {
                            stringBuilder.Append(string.Format("<condition attribute=\"{0}\" operator=\"eq\" value=\"{1}\" />", targetAttribute, value.ToString()));
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private string GetFetchPageAttribute(AccountSearchRequest request)
        {
            int count = 5000, page = 1;
            int maxPageSize = 5000;
            if (request != null)
            {
                if (request.PageNumber.HasValue)
                {
                    page = request.PageNumber.Value;
                    if (page < 1)
                        page = 1;
                }
                if (request.NumberOfRecords.HasValue && request.NumberOfRecords.Value > 0)
                {
                    count = request.NumberOfRecords.Value;
                    if (count > maxPageSize)
                        count = maxPageSize;
                }
            }
            return string.Format("page=\"{0}\" count=\"{1}\"", page, count);
        }
    }
}
