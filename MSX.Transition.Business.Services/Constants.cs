namespace MSX.Transition.Business.Services
{
    public static class Constants
    {
        public const string ATURolePlayed = "ATU Account Executive";
        public const string TransitionCRMEntity = "msp_relationshipmanagements";
        public const string SSPRole = "Specialist Sales";
        public const string SSPManagerRoleRummary = "Solution Area Specialists M";
        public const string SSPRoleSummary = "Solution Area Specialists IC";
        public const string ATUManagerRole = "ATU Manager";
        public const string ATSRole = "ATU Account Technology Strategist";
        public const string CSAMRoleSummary = "Customer Success Account Mgmt IC";

        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string ACTIVE = "ACTIVE";
        public const string EXPIRED = "EXPIRED";

        public const string STUM = "STU M";
        public const string SSP = "SSP";

    }
    public static class ErrorMessage
    {
        public const string AccountTransitionFailed = "Unable to {0} create account transition: {1}, Reason: {2}";
        public const string TranslateToCRMObjectFailed = "Unable to translate the object into CRM object : {0}";
        public const string UnknownError = "An error occurred : {0}";
        public const string NoAssignmentsError = "No assignments Requests found to be executed.";
        public const string DuplicateEventIdError = "Failure; All Requests must have event Ids; Duplicate Key:  {0}. ";
        public const string RequestValidationError = "Valid Entity is Expected; ";
        public const string SubjectValidationMessage = "Valid assignemnt Subject is Expected (Allowed Values 'CREATE', 'DELETE', 'UPDATE'); ";
        public const string AliasValidationMessage = "Valid Alias is Expected; ";
        public const string CRMIDValidationMessage = "Valid CRM ID is Expected; ";
        public const string SalesTerritoryValidationMessage = "Valid AccountSalesTerritoryGuid is Expected;";
        public const string CRMIDNotFound = "Account with CRMID {0} not found; ";
        public const string UserAliasNotFound = "User with Alias {0} not found in MSX or is in disabled state; ";
        public const string MorethanOneAssignmentsExistsOnAccount = "User {0} is having more than one assignment on Account {1}";


    }
    public static class Message
    {
        public const string TransitionProcessSuccess = "Assignment Record processed successfully";
    }

    public static class Status
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Conflict = "Conflict";
    }

    public static class Area
    {
        public const string UnitedStates = "United States";
        public const string Canada = "Canada";
        public const string Latam = "Latam";
    }

}
