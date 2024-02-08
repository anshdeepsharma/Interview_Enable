namespace MSX.Transition.Providers
{
    public static class Constants
    {
        public const string FetchXMLPath = "\\FetchXML\\";

        public const string FetchXMLQuery = "?fetchXml={0}";

        public const string ATURolePlayed = "ATU Account Executive";

        public const string MapperPath = "\\Mapper\\";
        public const string TransitionFieldMapperFile = "TransitionFieldMapper.json";
        public const string AccountFetchXMLFile = "Account.GetAccount.xml";
        public const string OpportunityByUserFetchXMLFile = "Opportunity.UserIsOwner.xml";
        public const string OpportunityByUserInTeamFetchXMLFile = "Opportunity.UserIsInTeam.xml";
        public const string TransitionFetchXMLFile = "Transition.GetTransition.xml";
        public const string UserFieldMapperFile = "UserFieldMapper.json";
        public const string AccountSelectAttributes = "accountid,name,msp_accountnumber,msp_endcustomersegmentcode,msp_endcustomersubsegmentcode,msp_parentinglevelcode,msp_mstopparentid,msp_gpid,msp_hq,ownerid";
        public const string OpportunitySelectAttributes = "msp_opportunitynumber,owninguser,owningteam";
        public const string TransitionSelectAttributes = "msp_relationshipmanagementid,msp_name,msp_accounttransitiiontype,msp_accountid,msp_previoussubsegment,msp_previoussegment,msp_currentsegment,msp_currentsubsegment,msp_activitystatuscode,msp_previousaccountowner,msp_incomingats,msp_outgoingats,msp_incomingssp,msp_outgoingssp,msp_manager,msp_transitionsolutionarea,ownerid,createdon";
        public const string DomainName = "@microsoft.com";
    }
    public static class ErrorMessage
    {
        public const string AccountTransitionFailed = "Unable to {0} account transition: {1}, Reason: {2}";
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
        public const string TransitionProcessSuccess = "Transition Record processed successfully";
    }

    public static class Status
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Conflict = "Conflict";
    }
}
