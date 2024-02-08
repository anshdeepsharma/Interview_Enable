namespace MSX.Transition.Providers.CRM.Contract
{
    public static class AccountCdsExtension
    {
        public static void MapToAccount(this AccountCds accountCds, MSX.Common.Models.Accounts.Account account)
        {
            if (accountCds.CID != null)
            {
                account.Data.SalesAccountId = accountCds.CID;

            }
            if (accountCds.Name != null)
            {
                account.Data.SalesAccountName = accountCds.Name;

            }

            if (accountCds.ManagedStatusDisplay != null)
            {
                account.Data.IsManaged = "Managed".Equals(accountCds.ManagedStatusDisplay, StringComparison.InvariantCultureIgnoreCase).ToString();
            }

            if (accountCds.GlobalCrmId != null)
            {
                account.Data.CrmAccountId = accountCds.GlobalCrmId;
            }
            if (accountCds.MsSalesId != null)
            {
                account.Data.MSSalesAccountId = accountCds.MsSalesId;
            }
            if (accountCds.MsSalesTpId != null)
            {
                account.Data.ParentMSSalesAccountId = accountCds.MsSalesTpId;
            }

            if (accountCds.GpId != null)
            {
                account.Data.GlobalParentAccountId = accountCds.GpId;
            }
            if (accountCds.GpName != null)
            {
                account.Data.GlobalParentAccountName = accountCds.GpName;
            }
            if (accountCds.IsGlobalHQ != null)
            {
                account.Data.IsGlobalHQ = accountCds.IsGlobalHQ;
            }

            if (accountCds.SalesTerritoryId != null)
            {
                account.Data.SalesTerritory_Id = accountCds.SalesTerritoryId;
            }
            if (accountCds.SalesTerritoryName != null)
            {
                account.Data.SalesTerritory = accountCds.SalesTerritoryName;
            }
            if (accountCds.SubsidiaryId != null)
            {
                account.Data.SubsidiaryId = accountCds.SubsidiaryId.ToString();
            }
            if (accountCds.SubsidiaryDisplay != null)
            {
                account.Data.Subsidiary = accountCds.SubsidiaryDisplay;
            }
            if (accountCds.AreaId != null)
            {
                account.Data.AreaId = accountCds.AreaId.ToString();
            }
            if (accountCds.AreaDisplay != null)
            {
                account.Data.Area = accountCds.AreaDisplay;
            }

            /* Segment */
            if (accountCds.SegmentGroupDisplay != null)
            {
                account.Data.SegmentGroup = accountCds.SegmentGroupDisplay;
            }
            if (accountCds.SegmentDisplay != null)
            {
                account.Data.Segment = accountCds.SegmentDisplay;
            }
            if (accountCds.SubSegmentDisplay != null)
            {
                account.Data.SubSegment = accountCds.SubSegmentDisplay;
            }

            if (accountCds.ParentingLevelDisplay != null)
            {
                account.Data.ParentLevel = accountCds.ParentingLevelDisplay;
            }

            if (accountCds.AccountOwnerId != null)
            {
                account.Data.CRMOwnerId = accountCds.AccountOwnerId.ToString();
                account.Data.OwnerAlias = accountCds.OwnerAlias != null ? accountCds.OwnerAlias.Split("@")[0] : accountCds.OwnerAlias;
            }
        }
    }
}
