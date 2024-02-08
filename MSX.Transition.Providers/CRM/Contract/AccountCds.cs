using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MSX.Transition.Providers.CRM.Contract
{
    public class AccountCds
    {
        [JsonPropertyName("accountid")]
        [DataMember]
        public Guid Id { get; set; }

        [JsonPropertyName("msp_gpid")]
        [DataMember]
        public string? GpId { get; set; }

        [JsonPropertyName("msp_gpname")]
        [DataMember]
        public string? GpName { get; set; }

        [JsonPropertyName("msp_hq@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? IsGlobalHQ { get; set; }

        [JsonPropertyName("msp_mssalesid")]
        [DataMember]
        public string? MsSalesId { get; set; }

        [JsonPropertyName("msp_mstopparentid")]
        [DataMember]
        public string? MsSalesTpId { get; set; }

        [JsonPropertyName("name")]
        [DataMember]
        public string Name { get; set; }

        [JsonPropertyName("parent.msp_accountnumber")]
        [DataMember]
        public string? ParentCrmId { get; set; }
        [JsonPropertyName("_ownerid_value")]
        [DataMember]
        public Guid? AccountOwnerId { get; set; }

        [JsonPropertyName("user.domainname")]
        [DataMember]
        public string? OwnerAlias { get; set; }

        [JsonPropertyName("_transactioncurrencyid_value")]
        [DataMember]
        public string? CurrencyId { get; set; }

        [JsonPropertyName("telephone1")]
        [DataMember]
        public string? MainPhone { get; set; }

        [JsonPropertyName("websiteurl")]
        [DataMember]
        public string? Website { get; set; }

        [JsonPropertyName("msp_servicesengagement")]
        [DataMember]
        public bool? ServicesEngagement { get; set; }

        [JsonPropertyName("msp_mcscoveragemodel")]
        [DataMember]
        public string? MCSCoverageModel { get; set; }

        [JsonPropertyName("msp_endcustomersubsegmentcode")]
        [DataMember]
        public int? SubSegmentId { get; set; }

        [JsonPropertyName("numberofemployees")]
        [DataMember]
        public int? NumberOfEmployees { get; set; }

        [JsonPropertyName("msp_pccount")]
        [DataMember]
        public int? PcCount { get; set; }

        [JsonPropertyName("msp_servercount")]
        [DataMember]
        public int? ServerCount { get; set; }

        [JsonPropertyName("msp_tabletcount")]
        [DataMember]
        public int? TabletCount { get; set; }

        [JsonPropertyName("msp_smartphonecount")]
        [DataMember]
        public int? SmartphoneCount { get; set; }

        [JsonPropertyName("fax")]
        [DataMember]
        public string? Fax { get; set; }

        [JsonPropertyName("msp_verticalcode")]
        [DataMember]
        public int? VerticalId { get; set; }

        [JsonPropertyName("msp_subverticalcode")]
        [DataMember]
        public int? SubVerticalId { get; set; }

        [JsonPropertyName("msp_verticalcategorycode")]
        [DataMember]
        public int? VerticalCategoryId { get; set; }

        [JsonPropertyName("msp_segmentgroup")]
        [DataMember]
        public int? SegmentGroupId { get; set; }

        [JsonPropertyName("msp_endcustomersegmentcode")]
        [DataMember]
        public int? SegmentId { get; set; }

        [JsonPropertyName("msp_managedstatuscode")]
        [DataMember]
        public int? ManagedStatusId { get; set; }

        [JsonPropertyName("address1_city")]
        [DataMember]
        public string? Address1City { get; set; }

        [JsonPropertyName("address1_country")]
        [DataMember]
        public string? Address1Country { get; set; }

        [JsonPropertyName("address1_line1")]
        [DataMember]
        public string? Address1Line1 { get; set; }

        [JsonPropertyName("address1_line2")]
        [DataMember]
        public string? Address1Line2 { get; set; }

        [JsonPropertyName("address1_line3")]
        [DataMember]
        public string? Address1Line3 { get; set; }

        [JsonPropertyName("address1_postalcode")]
        [DataMember]
        public string? Address1PostalCode { get; set; }

        [JsonPropertyName("address1_stateorprovince")]
        [DataMember]
        public string? Address1StateOrProvince { get; set; }

        [JsonPropertyName("msp_parentinglevelcode")]
        [DataMember]
        public int? ParentingLevelId { get; set; }

        [JsonPropertyName("msp_mdmorganizationid")]
        [DataMember]
        public string? MDMOrganizationId { get; set; }
        [JsonPropertyName("msp_dunsid")]
        [DataMember]
        public string? DunsId { get; set; }

        [JsonPropertyName("msp_cid")]
        [DataMember]
        public string? CID { get; set; }

        [JsonPropertyName("msp_taxid")]
        [DataMember]
        public string? TaxId { get; set; }

        [JsonPropertyName("description")]
        [DataMember]
        public string? Description { get; set; }

        [JsonPropertyName("msp_oemorganizatontype")]
        [DataMember]
        public int? OemOrganizationType { get; set; }

        [JsonPropertyName("msp_oemsegmenttype")]
        [DataMember]
        public int? OemSummarySegment { get; set; }

        [JsonPropertyName("msp_oemmanaged")]
        [DataMember]
        public bool? OemManaged { get; set; }

        [JsonPropertyName("msp_clanumber")]
        [DataMember]
        public string? ClaId { get; set; }

        [DataMember]
        [JsonPropertyName("msp_digitalnativeidentifier")]
        public string? Tags { get; set; }

        [JsonPropertyName("_ownerid_value@OData.Community.Display.V1.FormattedValue")]
        [DataMember]
        public string? AccountOwnerDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("_transactioncurrencyid_value@OData.Community.Display.V1.FormattedValue")]
        public string? CurrencyDisplay { get; set; }

        [JsonPropertyName("currency.isocurrencycode")]
        [DataMember]
        public string? CurrencyCode { get; set; }

        [DataMember]
        [JsonPropertyName("msp_endcustomersubsegmentcode@OData.Community.Display.V1.FormattedValue")]
        public string? SubSegmentDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_verticalcode@OData.Community.Display.V1.FormattedValue")]
        public string? VerticalDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_subverticalcode@OData.Community.Display.V1.FormattedValue")]
        public string? SubVerticalDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_verticalcategorycode@OData.Community.Display.V1.FormattedValue")]
        public string? VerticalCategoryDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_segmentgroup@OData.Community.Display.V1.FormattedValue")]
        public string? SegmentGroupDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_endcustomersegmentcode@OData.Community.Display.V1.FormattedValue")]
        public string? SegmentDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_managedstatuscode@OData.Community.Display.V1.FormattedValue")]
        public string? ManagedStatusDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_parentinglevelcode@OData.Community.Display.V1.FormattedValue")]
        public string? ParentingLevelDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_oemorganizatontype@OData.Community.Display.V1.FormattedValue")]
        public string? OemOrganizationTypeDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_oemsegmenttype@OData.Community.Display.V1.FormattedValue")]
        public string? OemSummarySegmentDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("_msp_accountcreatedby_value@OData.Community.Display.V1.FormattedValue")]
        public string? CreatedOnBehalfByDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("_msp_accountmodifiedby_value@OData.Community.Display.V1.FormattedValue")]
        public string? ModifiedOnBehalfByDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("msp_digitalnativeidentifier@OData.Community.Display.V1.FormattedValue")]
        public string? TagsDisplay { get; set; }

        // Linked Entity attributes
        [JsonPropertyName("territory.territoryid")]
        [DataMember]
        public string? SalesTerritoryId { get; set; }

        [JsonPropertyName("territory.name")]
        [DataMember]
        public string? SalesTerritoryName { get; set; }

        [JsonPropertyName("territory.msp_subsidiarycode")]
        [DataMember]
        public int? SubsidiaryId { get; set; }

        [DataMember]
        [JsonPropertyName("territory.msp_subsidiarycode@OData.Community.Display.V1.FormattedValue")]
        public string? SubsidiaryDisplay { get; set; }

        [JsonPropertyName("territory.msp_salesgeographyareacode")]
        [DataMember]
        public int? AreaId { get; set; }

        [DataMember]
        [JsonPropertyName("territory.msp_salesgeographyareacode@OData.Community.Display.V1.FormattedValue")]
        public string? AreaDisplay { get; set; }


        [JsonPropertyName("_msp_executivesponsor_value")]
        [DataMember]
        public Guid? ExecutiveSponsorId { get; set; }

        [JsonPropertyName("executivesponsor.domainname")]
        [DataMember]
        public string? ExecutiveSponsorAlias { get; set; }

        [DataMember]
        [JsonPropertyName("msp_accountnumber")]
        public string? GlobalCrmId { get; set; }

        [DataMember]
        [JsonPropertyName("createdon")]
        public DateTime? CreatedOn { get; set; }

        [DataMember]
        [JsonPropertyName("modifiedon")]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        [JsonPropertyName("_createdby_value")]
        public string? CreatedBy { get; set; }

        [DataMember]
        [JsonPropertyName("_modifiedby_value")]
        public string? ModifiedBy { get; set; }

        [DataMember]
        [JsonPropertyName("_createdby_value@OData.Community.Display.V1.FormattedValue")]
        public string? CreatedByDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("_createdby_value@Microsoft.Dynamics.CRM.lookuplogicalname")]
        public string? CreatedByTypeName { get; set; }

        [DataMember]
        [JsonPropertyName("_modifiedby_value@OData.Community.Display.V1.FormattedValue")]
        public string? ModifiedByDisplay { get; set; }

        [DataMember]
        [JsonPropertyName("_modifiedby_value@Microsoft.Dynamics.CRM.lookuplogicalname")]
        public string? ModifiedByTypeName { get; set; }
    }

}
