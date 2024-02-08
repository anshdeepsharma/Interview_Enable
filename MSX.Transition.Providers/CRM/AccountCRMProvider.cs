using Microsoft.Extensions.Logging;
using MSX.Common.Models.Accounts;
using MSX.Common.Models.Enums;
using MSX.Transition.Providers.Contracts.v1.Provider;
using MSX.Transition.Providers.CRM.Contract;
using System.Web;

namespace MSX.Transition.Providers.CRM
{
    public class AccountCRMProvider : IAccountCRMProvider
    {
        private readonly ILogger<IAccountCRMProvider> _logger;
        private readonly ICRMProvider _crmProvider;
        private readonly ICRMXMLHandler _crmXMLHandler;

        private static string AccountFetchXML = XMLHandler.ReadXMLFile(Constants.FetchXMLPath + Constants.AccountFetchXMLFile);

        public AccountCRMProvider(ILogger<IAccountCRMProvider> logger
            , ICRMProvider crmProvider
            , ICRMXMLHandler crmXMLHandler)
        {
            _logger = logger;
            _crmProvider = crmProvider;
            _crmXMLHandler = crmXMLHandler;
        }
        public async Task<Account?> GetAccountByCRMIdAsync(string crmAccountId)
        {
            Data search = new() { CrmAccountId = HttpUtility.UrlEncode(crmAccountId) };

            string query = _crmXMLHandler.ConstructQuery("accounts"
                , AccountFetchXML
                , Entity.Account.ToString()
                , Constants.AccountSelectAttributes
                , new Dictionary<string, string>()
                    {
                        { "Id", "accountid" },
                        { "CrmAccountId", "msp_accountnumber" }
                    }
                , search);

            var accountCdsList = await _crmProvider.ExecuteGetRequestAsync<ResponseValueArray<AccountCds>>(query);
            return MapAccountCds(accountCdsList).FirstOrDefault();
        }

        private List<Account> MapAccountCds(ResponseValueArray<AccountCds>? accountCdsList)
        {
            var accountList = new List<Account>();

            if (accountCdsList?.value?.Count > 0)
            {
                for (int index = 0; index < accountCdsList.value.Count; index++)
                {
                    var accountCds = accountCdsList.value[index];

                    var transformedAccounts = new Account()
                    {
                        Data = new Data()
                        {
                            Id = accountCds.Id.ToString()
                        }
                    };

                    accountCds.MapToAccount(transformedAccounts);
                    accountList.Add(transformedAccounts);
                }
            }
            return accountList;
        }
    }
}