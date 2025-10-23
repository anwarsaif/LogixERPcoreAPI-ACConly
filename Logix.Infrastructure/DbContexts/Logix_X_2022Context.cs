using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

/*namespace Logix.Infrastructure.DbContexts
{
    public partial class Logix_X_2022Context : DbContext
    {
        public Logix_X_2022Context()
        {
        }

        public Logix_X_2022Context(DbContextOptions<Logix_X_2022Context> options)
            : base(options)
        {
        }

        public virtual DbSet<AccAccount> AccAccounts { get; set; } = null!;
        public virtual DbSet<AccAccountStatementVw> AccAccountStatementVws { get; set; } = null!;
        public virtual DbSet<AccAccountsCloseType> AccAccountsCloseTypes { get; set; } = null!;
        public virtual DbSet<AccAccountsCostcenter> AccAccountsCostcenters { get; set; } = null!;
        public virtual DbSet<AccAccountsCostcenterVw> AccAccountsCostcenterVws { get; set; } = null!;
        public virtual DbSet<AccAccountsGroupsFinalVw> AccAccountsGroupsFinalVws { get; set; } = null!;
        public virtual DbSet<AccAccountsLevel> AccAccountsLevels { get; set; } = null!;
        public virtual DbSet<AccAccountsRefrancesVw> AccAccountsRefrancesVws { get; set; } = null!;
        public virtual DbSet<AccAccountsReportsVw> AccAccountsReportsVws { get; set; } = null!;
        public virtual DbSet<AccAccountsSubHelpeVw> AccAccountsSubHelpeVws { get; set; } = null!;
        public virtual DbSet<AccAccountsSubVw> AccAccountsSubVws { get; set; } = null!;
        public virtual DbSet<AccAccountsVw> AccAccountsVws { get; set; } = null!;
        public virtual DbSet<AccActivitesVw> AccActivitesVws { get; set; } = null!;
        public virtual DbSet<AccBalanceSheet> AccBalanceSheets { get; set; } = null!;
        public virtual DbSet<AccBalanceSheet2Vw> AccBalanceSheet2Vws { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetContractor> AccBalanceSheetContractors { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetCostCenterVw> AccBalanceSheetCostCenterVws { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetCustomer> AccBalanceSheetCustomers { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetDonor> AccBalanceSheetDonors { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetEmployee> AccBalanceSheetEmployees { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetPostOrNot> AccBalanceSheetPostOrNots { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetStudent> AccBalanceSheetStudents { get; set; } = null!;
        public virtual DbSet<AccBalanceSheetSupplier> AccBalanceSheetSuppliers { get; set; } = null!;
        public virtual DbSet<AccBank> AccBanks { get; set; } = null!;
        public virtual DbSet<AccBankVw> AccBankVws { get; set; } = null!;
        public virtual DbSet<AccBranchAccount> AccBranchAccounts { get; set; } = null!;
        public virtual DbSet<AccBranchAccountType> AccBranchAccountTypes { get; set; } = null!;
        public virtual DbSet<AccBranchAccountsVw> AccBranchAccountsVws { get; set; } = null!;
        public virtual DbSet<AccBranchVw> AccBranchVws { get; set; } = null!;
        public virtual DbSet<AccBudgetEstimateBalanceVw> AccBudgetEstimateBalanceVws { get; set; } = null!;
        public virtual DbSet<AccBudgetEstimateDetaile> AccBudgetEstimateDetailes { get; set; } = null!;
        public virtual DbSet<AccBudgetEstimateDetailesVw> AccBudgetEstimateDetailesVws { get; set; } = null!;
        public virtual DbSet<AccBudgetEstimateMaster> AccBudgetEstimateMasters { get; set; } = null!;
        public virtual DbSet<AccBudgetEstimateVw> AccBudgetEstimateVws { get; set; } = null!;
        public virtual DbSet<AccCashOnHand> AccCashOnHands { get; set; } = null!;
        public virtual DbSet<AccCashOnHandListVw> AccCashOnHandListVws { get; set; } = null!;
        public virtual DbSet<AccCashOnHandVw> AccCashOnHandVws { get; set; } = null!;
        public virtual DbSet<AccCheque> AccCheques { get; set; } = null!;
        public virtual DbSet<AccChequeReturn> AccChequeReturns { get; set; } = null!;
        public virtual DbSet<AccChequeReturnVw> AccChequeReturnVws { get; set; } = null!;
        public virtual DbSet<AccChequesNotesVw> AccChequesNotesVws { get; set; } = null!;
        public virtual DbSet<AccChequesStatus> AccChequesStatuses { get; set; } = null!;
        public virtual DbSet<AccChequesStatusVw> AccChequesStatusVws { get; set; } = null!;
        public virtual DbSet<AccChequesVw> AccChequesVws { get; set; } = null!;
        public virtual DbSet<AccCostCenteHelpVw> AccCostCenteHelpVws { get; set; } = null!;
        public virtual DbSet<AccCostCenter> AccCostCenters { get; set; } = null!;
        public virtual DbSet<AccCostCenterListVw> AccCostCenterListVws { get; set; } = null!;
        public virtual DbSet<AccCostCenterVw> AccCostCenterVws { get; set; } = null!;
        public virtual DbSet<AccDebtAge> AccDebtAges { get; set; } = null!;
        public virtual DbSet<AccDocumentType> AccDocumentTypes { get; set; } = null!;
        public virtual DbSet<AccDocumentTypeListVw> AccDocumentTypeListVws { get; set; } = null!;
        public virtual DbSet<AccExpensesIncome> AccExpensesIncomes { get; set; } = null!;
        public virtual DbSet<AccFacilitiesVw> AccFacilitiesVws { get; set; } = null!;
        public virtual DbSet<AccFacility> AccFacilities { get; set; } = null!;
        public virtual DbSet<AccFinancialPositionVw> AccFinancialPositionVws { get; set; } = null!;
        public virtual DbSet<AccFinancialYear> AccFinancialYears { get; set; } = null!;
        public virtual DbSet<AccFinancialYearVw> AccFinancialYearVws { get; set; } = null!;
        public virtual DbSet<AccGroup> AccGroups { get; set; } = null!;
        public virtual DbSet<AccGroupVw> AccGroupVws { get; set; } = null!;
        public virtual DbSet<AccGuarantee> AccGuarantees { get; set; } = null!;
        public virtual DbSet<AccJournalComment> AccJournalComments { get; set; } = null!;
        public virtual DbSet<AccJournalCommentsVw> AccJournalCommentsVws { get; set; } = null!;
        public virtual DbSet<AccJournalDetaile> AccJournalDetailes { get; set; } = null!;
        public virtual DbSet<AccJournalDetailesCostcenter> AccJournalDetailesCostcenters { get; set; } = null!;
        public virtual DbSet<AccJournalDetailesCostcenterVw> AccJournalDetailesCostcenterVws { get; set; } = null!;
        public virtual DbSet<AccJournalDetailesVw> AccJournalDetailesVws { get; set; } = null!;
        public virtual DbSet<AccJournalMaster> AccJournalMasters { get; set; } = null!;
        public virtual DbSet<AccJournalMasterExportVw> AccJournalMasterExportVws { get; set; } = null!;
        public virtual DbSet<AccJournalMasterFile> AccJournalMasterFiles { get; set; } = null!;
        public virtual DbSet<AccJournalMasterFilesVw> AccJournalMasterFilesVws { get; set; } = null!;
        public virtual DbSet<AccJournalMasterStatus> AccJournalMasterStatuses { get; set; } = null!;
        public virtual DbSet<AccJournalMasterVw> AccJournalMasterVws { get; set; } = null!;
        public virtual DbSet<AccJournalSignatureVw> AccJournalSignatureVws { get; set; } = null!;
        public virtual DbSet<AccPaymentType> AccPaymentTypes { get; set; } = null!;
        public virtual DbSet<AccPeriod> AccPeriods { get; set; } = null!;
        public virtual DbSet<AccPeriodDateVw> AccPeriodDateVws { get; set; } = null!;
        public virtual DbSet<AccPeriodsVw> AccPeriodsVws { get; set; } = null!;
        public virtual DbSet<AccPettyCash> AccPettyCashes { get; set; } = null!;
        public virtual DbSet<AccPettyCashD> AccPettyCashDs { get; set; } = null!;
        public virtual DbSet<AccPettyCashDVw> AccPettyCashDVws { get; set; } = null!;
        public virtual DbSet<AccPettyCashExpenseVw> AccPettyCashExpenseVws { get; set; } = null!;
        public virtual DbSet<AccPettyCashExpensesType> AccPettyCashExpensesTypes { get; set; } = null!;
        public virtual DbSet<AccPettyCashExpensesTypeVw> AccPettyCashExpensesTypeVws { get; set; } = null!;
        public virtual DbSet<AccPettyCashTemp> AccPettyCashTemps { get; set; } = null!;
        public virtual DbSet<AccPettyCashTempVw> AccPettyCashTempVws { get; set; } = null!;
        public virtual DbSet<AccPettyCashVw> AccPettyCashVws { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayable> AccReceivablesPayables { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesAccount> AccReceivablesPayablesAccounts { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesAccountsVw> AccReceivablesPayablesAccountsVws { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesD> AccReceivablesPayablesDs { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesDVw> AccReceivablesPayablesDVws { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesStatus> AccReceivablesPayablesStatuses { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesTransaction> AccReceivablesPayablesTransactions { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesTransactionD> AccReceivablesPayablesTransactionDs { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesTransactionDVw> AccReceivablesPayablesTransactionDVws { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesTransactionVw> AccReceivablesPayablesTransactionVws { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesType> AccReceivablesPayablesTypes { get; set; } = null!;
        public virtual DbSet<AccReceivablesPayablesVw> AccReceivablesPayablesVws { get; set; } = null!;
        public virtual DbSet<AccReconciliation> AccReconciliations { get; set; } = null!;
        public virtual DbSet<AccReconciliationDetaile> AccReconciliationDetailes { get; set; } = null!;
        public virtual DbSet<AccReconciliationDetailesVw> AccReconciliationDetailesVws { get; set; } = null!;
        public virtual DbSet<AccReconciliationVw> AccReconciliationVws { get; set; } = null!;
        public virtual DbSet<AccReferenceType> AccReferenceTypes { get; set; } = null!;
        public virtual DbSet<AccReferenceTypeVw> AccReferenceTypeVws { get; set; } = null!;
        public virtual DbSet<AccRequest> AccRequests { get; set; } = null!;
        public virtual DbSet<AccRequestBalanceStatusVw> AccRequestBalanceStatusVws { get; set; } = null!;
        public virtual DbSet<AccRequestExchangeStatusVw> AccRequestExchangeStatusVws { get; set; } = null!;
        public virtual DbSet<AccRequestHasCreditVw> AccRequestHasCreditVws { get; set; } = null!;
        public virtual DbSet<AccRequestJournalVw> AccRequestJournalVws { get; set; } = null!;
        public virtual DbSet<AccRequestVw> AccRequestVws { get; set; } = null!;
        public virtual DbSet<AccSettlementInstallment> AccSettlementInstallments { get; set; } = null!;
        public virtual DbSet<AccSettlementInstallmentsVw> AccSettlementInstallmentsVws { get; set; } = null!;
        public virtual DbSet<AccSettlementSchedule> AccSettlementSchedules { get; set; } = null!;
        public virtual DbSet<AccSettlementScheduleD> AccSettlementScheduleDs { get; set; } = null!;
        public virtual DbSet<AccSettlementScheduleDVw> AccSettlementScheduleDVws { get; set; } = null!;
        public virtual DbSet<AccSettlementScheduleVw> AccSettlementScheduleVws { get; set; } = null!;
        public virtual DbSet<AccountIdPosList> AccountIdPosLists { get; set; } = null!;
        public virtual DbSet<AllowancesOvertimeBonusesVw> AllowancesOvertimeBonusesVws { get; set; } = null!;
        public virtual DbSet<BudgAccount> BudgAccounts { get; set; } = null!;
        public virtual DbSet<BudgAccountsSubHelpeVw> BudgAccountsSubHelpeVws { get; set; } = null!;
        public virtual DbSet<BudgAccountsVw> BudgAccountsVws { get; set; } = null!;
        public virtual DbSet<BudgBalanceSheet> BudgBalanceSheets { get; set; } = null!;
        public virtual DbSet<BudgCreditsFinancialYear> BudgCreditsFinancialYears { get; set; } = null!;
        public virtual DbSet<BudgCreditsFinancialYearVw> BudgCreditsFinancialYearVws { get; set; } = null!;
        public virtual DbSet<BudgDocType> BudgDocTypes { get; set; } = null!;
        public virtual DbSet<BudgGroup> BudgGroups { get; set; } = null!;
        public virtual DbSet<BudgGroupVw> BudgGroupVws { get; set; } = null!;
        public virtual DbSet<BudgRequest> BudgRequests { get; set; } = null!;
        public virtual DbSet<BudgTransaction> BudgTransactions { get; set; } = null!;
        public virtual DbSet<BudgTransactionDetaile> BudgTransactionDetailes { get; set; } = null!;
        public virtual DbSet<BudgTransactionDetailesVw> BudgTransactionDetailesVws { get; set; } = null!;
        public virtual DbSet<BudgTransactionFile> BudgTransactionFiles { get; set; } = null!;
        public virtual DbSet<BudgTransactionVw> BudgTransactionVws { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<Cite> Cites { get; set; } = null!;
        public virtual DbSet<CrmActionTypeVw> CrmActionTypeVws { get; set; } = null!;
        public virtual DbSet<CrmAdvertisingCampaign> CrmAdvertisingCampaigns { get; set; } = null!;
        public virtual DbSet<CrmAdvertisingCampaignsDetail> CrmAdvertisingCampaignsDetails { get; set; } = null!;
        public virtual DbSet<CrmAdvertisingCampaignsDetailsVw> CrmAdvertisingCampaignsDetailsVws { get; set; } = null!;
        public virtual DbSet<CrmAdvertisingCampaignsLink> CrmAdvertisingCampaignsLinks { get; set; } = null!;
        public virtual DbSet<CrmAdvertisingCampaignsVw> CrmAdvertisingCampaignsVws { get; set; } = null!;
        public virtual DbSet<CrmCampaignTypeVw> CrmCampaignTypeVws { get; set; } = null!;
        public virtual DbSet<CrmEmailTemplate> CrmEmailTemplates { get; set; } = null!;
        public virtual DbSet<CrmEmailTemplateAttach> CrmEmailTemplateAttaches { get; set; } = null!;
        public virtual DbSet<CrmMeeting> CrmMeetings { get; set; } = null!;
        public virtual DbSet<CrmMeetingsAgendum> CrmMeetingsAgenda { get; set; } = null!;
        public virtual DbSet<CrmMeetingsStaff> CrmMeetingsStaffs { get; set; } = null!;
        public virtual DbSet<CrmMeetingsStaffVw> CrmMeetingsStaffVws { get; set; } = null!;
        public virtual DbSet<CrmMeetingsVw> CrmMeetingsVws { get; set; } = null!;
        public virtual DbSet<CrmOpportunitiesPayment> CrmOpportunitiesPayments { get; set; } = null!;
        public virtual DbSet<CrmOpportunitiesPaymentVw> CrmOpportunitiesPaymentVws { get; set; } = null!;
        public virtual DbSet<CrmOpportunitiesVw> CrmOpportunitiesVws { get; set; } = null!;
        public virtual DbSet<CrmOpportunity> CrmOpportunities { get; set; } = null!;
        public virtual DbSet<DrvDocument> DrvDocuments { get; set; } = null!;
        public virtual DbSet<DrvDocumentVw> DrvDocumentVws { get; set; } = null!;
        public virtual DbSet<DrvFolder> DrvFolders { get; set; } = null!;
        public virtual DbSet<DrvHistoryDownload> DrvHistoryDownloads { get; set; } = null!;
        public virtual DbSet<DrvHistoryDownloadVw> DrvHistoryDownloadVws { get; set; } = null!;
        public virtual DbSet<EmergencyTransaction> EmergencyTransactions { get; set; } = null!;
        public virtual DbSet<EmergencyTransactionsD> EmergencyTransactionsDs { get; set; } = null!;
        public virtual DbSet<EmergencyTransactionsDVw> EmergencyTransactionsDVws { get; set; } = null!;
        public virtual DbSet<EmergencyTransactionsVw> EmergencyTransactionsVws { get; set; } = null!;
        public virtual DbSet<EquAsset> EquAssets { get; set; } = null!;
        public virtual DbSet<EquAssetsFile> EquAssetsFiles { get; set; } = null!;
        public virtual DbSet<EquAssetsInsurance> EquAssetsInsurances { get; set; } = null!;
        public virtual DbSet<EquAssetsInsuranceVw> EquAssetsInsuranceVws { get; set; } = null!;
        public virtual DbSet<EquAssetsMetering> EquAssetsMeterings { get; set; } = null!;
        public virtual DbSet<EquAssetsMeteringVw> EquAssetsMeteringVws { get; set; } = null!;
        public virtual DbSet<EquAssetsNote> EquAssetsNotes { get; set; } = null!;
        public virtual DbSet<EquAssetsNoteVw> EquAssetsNoteVws { get; set; } = null!;
        public virtual DbSet<EquAssetsPart> EquAssetsParts { get; set; } = null!;
        public virtual DbSet<EquAssetsPartsVw> EquAssetsPartsVws { get; set; } = null!;
        public virtual DbSet<EquAssetsStatus> EquAssetsStatuses { get; set; } = null!;
        public virtual DbSet<EquAssetsStatusVw> EquAssetsStatusVws { get; set; } = null!;
        public virtual DbSet<EquAssetsTransfer> EquAssetsTransfers { get; set; } = null!;
        public virtual DbSet<EquAssetsTransferVw> EquAssetsTransferVws { get; set; } = null!;
        public virtual DbSet<EquAssetsType> EquAssetsTypes { get; set; } = null!;
        public virtual DbSet<EquAssetsTypeVw> EquAssetsTypeVws { get; set; } = null!;
        public virtual DbSet<EquAssetsUser> EquAssetsUsers { get; set; } = null!;
        public virtual DbSet<EquAssetsUsersVw> EquAssetsUsersVws { get; set; } = null!;
        public virtual DbSet<EquAssetsVw> EquAssetsVws { get; set; } = null!;
        public virtual DbSet<EquAssetsWarrantiesVw> EquAssetsWarrantiesVws { get; set; } = null!;
        public virtual DbSet<EquAssetsWarranty> EquAssetsWarranties { get; set; } = null!;
        public virtual DbSet<EquCheckItem> EquCheckItems { get; set; } = null!;
        public virtual DbSet<EquCheckItemsPiece> EquCheckItemsPieces { get; set; } = null!;
        public virtual DbSet<EquCheckItemsPieceVw> EquCheckItemsPieceVws { get; set; } = null!;
        public virtual DbSet<EquContract> EquContracts { get; set; } = null!;
        public virtual DbSet<EquContractInstallment> EquContractInstallments { get; set; } = null!;
        public virtual DbSet<EquContractInstallmentVw> EquContractInstallmentVws { get; set; } = null!;
        public virtual DbSet<EquContractVw> EquContractVws { get; set; } = null!;
        public virtual DbSet<EquEmployeesWorkshop> EquEmployeesWorkshops { get; set; } = null!;
        public virtual DbSet<EquEmployeesWorkshopVw> EquEmployeesWorkshopVws { get; set; } = null!;
        public virtual DbSet<EquEquipmentNoteTypeVw> EquEquipmentNoteTypeVws { get; set; } = null!;
        public virtual DbSet<EquFormsCheckItem> EquFormsCheckItems { get; set; } = null!;
        public virtual DbSet<EquFormsCheckItemsVw> EquFormsCheckItemsVws { get; set; } = null!;
        public virtual DbSet<EquFreeMonth> EquFreeMonths { get; set; } = null!;
        public virtual DbSet<EquFreeMonthsVw> EquFreeMonthsVws { get; set; } = null!;
        public virtual DbSet<EquGap> EquGaps { get; set; } = null!;
        public virtual DbSet<EquGapStatus> EquGapStatuses { get; set; } = null!;
        public virtual DbSet<EquGapVw> EquGapVws { get; set; } = null!;
        public virtual DbSet<EquGroupsForm> EquGroupsForms { get; set; } = null!;
        public virtual DbSet<EquGroupsFormsVw> EquGroupsFormsVws { get; set; } = null!;
        public virtual DbSet<EquIncome> EquIncomes { get; set; } = null!;
        public virtual DbSet<EquIncomeVw> EquIncomeVws { get; set; } = null!;
        public virtual DbSet<EquPayInstallment> EquPayInstallments { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenance> EquPreventiveMaintenances { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceForm> EquPreventiveMaintenanceForms { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceFormsVw> EquPreventiveMaintenanceFormsVws { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceGroup> EquPreventiveMaintenanceGroups { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceGroups2Vw> EquPreventiveMaintenanceGroups2Vws { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceGroupsVw> EquPreventiveMaintenanceGroupsVws { get; set; } = null!;
        public virtual DbSet<EquPreventiveMaintenanceVw> EquPreventiveMaintenanceVws { get; set; } = null!;
        public virtual DbSet<EquStatus> EquStatuses { get; set; } = null!;
        public virtual DbSet<EquSupplierIdWarranty> EquSupplierIdWarranties { get; set; } = null!;
        public virtual DbSet<EquTransactionsType> EquTransactionsTypes { get; set; } = null!;
        public virtual DbSet<EquTypeVw> EquTypeVws { get; set; } = null!;
        public virtual DbSet<EquTypesPayment> EquTypesPayments { get; set; } = null!;
        public virtual DbSet<EquWarrantyUsageTermType> EquWarrantyUsageTermTypes { get; set; } = null!;
        public virtual DbSet<EquWorkOrder> EquWorkOrders { get; set; } = null!;
        public virtual DbSet<EquWorkOrderCompletion> EquWorkOrderCompletions { get; set; } = null!;
        public virtual DbSet<EquWorkOrderCost> EquWorkOrderCosts { get; set; } = null!;
        public virtual DbSet<EquWorkOrderCostVw> EquWorkOrderCostVws { get; set; } = null!;
        public virtual DbSet<EquWorkOrderFile> EquWorkOrderFiles { get; set; } = null!;
        public virtual DbSet<EquWorkOrderMeterReading> EquWorkOrderMeterReadings { get; set; } = null!;
        public virtual DbSet<EquWorkOrderPart> EquWorkOrderParts { get; set; } = null!;
        public virtual DbSet<EquWorkOrderPartVw> EquWorkOrderPartVws { get; set; } = null!;
        public virtual DbSet<EquWorkOrderTask> EquWorkOrderTasks { get; set; } = null!;
        public virtual DbSet<EquWorkOrderTaskVw> EquWorkOrderTaskVws { get; set; } = null!;
        public virtual DbSet<EquWorkOrderVw> EquWorkOrderVws { get; set; } = null!;
        public virtual DbSet<EquWorkshop> EquWorkshops { get; set; } = null!;
        public virtual DbSet<EquWorkshopTask> EquWorkshopTasks { get; set; } = null!;
        public virtual DbSet<FollowDistributionTransactionsSupervisor> FollowDistributionTransactionsSupervisors { get; set; } = null!;
        public virtual DbSet<FollowDistributionTransactionsSupervisorsLocation> FollowDistributionTransactionsSupervisorsLocations { get; set; } = null!;
        public virtual DbSet<FollowDistributionTransactionsSupervisorsLocationVw> FollowDistributionTransactionsSupervisorsLocationVws { get; set; } = null!;
        public virtual DbSet<FollowDistributionTransactionsSupervisorsVw> FollowDistributionTransactionsSupervisorsVws { get; set; } = null!;
        public virtual DbSet<FollowLocationSupervisor> FollowLocationSupervisors { get; set; } = null!;
        public virtual DbSet<FollowLocationSupervisorVw> FollowLocationSupervisorVws { get; set; } = null!;
        public virtual DbSet<FollowTransaction> FollowTransactions { get; set; } = null!;
        public virtual DbSet<FollowTransactionDetail> FollowTransactionDetails { get; set; } = null!;
        public virtual DbSet<FollowViolation> FollowViolations { get; set; } = null!;
        public virtual DbSet<FollowViolationType> FollowViolationTypes { get; set; } = null!;
        public virtual DbSet<FoodicsBank> FoodicsBanks { get; set; } = null!;
        public virtual DbSet<FoodicsBanksVw> FoodicsBanksVws { get; set; } = null!;
        public virtual DbSet<FoodicsBranch> FoodicsBranches { get; set; } = null!;
        public virtual DbSet<FoodicsBranchesVw> FoodicsBranchesVws { get; set; } = null!;
        public virtual DbSet<FoodicsCloser> FoodicsClosers { get; set; } = null!;
        public virtual DbSet<FoodicsClosersVw> FoodicsClosersVws { get; set; } = null!;
        public virtual DbSet<FoodicsJournalLog> FoodicsJournalLogs { get; set; } = null!;
        public virtual DbSet<FoodicsOrder> FoodicsOrders { get; set; } = null!;
        public virtual DbSet<FoodicsOrderPayment> FoodicsOrderPayments { get; set; } = null!;
        public virtual DbSet<FoodicsOrderPaymentsVw> FoodicsOrderPaymentsVws { get; set; } = null!;
        public virtual DbSet<FoodicsOrdersVw> FoodicsOrdersVws { get; set; } = null!;
        public virtual DbSet<FoodicsPaymentMapping> FoodicsPaymentMappings { get; set; } = null!;
        public virtual DbSet<FxaDepreciationMethod> FxaDepreciationMethods { get; set; } = null!;
        public virtual DbSet<FxaFixedAsset> FxaFixedAssets { get; set; } = null!;
        public virtual DbSet<FxaFixedAssetTransfer> FxaFixedAssetTransfers { get; set; } = null!;
        public virtual DbSet<FxaFixedAssetTransferVw> FxaFixedAssetTransferVws { get; set; } = null!;
        public virtual DbSet<FxaFixedAssetType> FxaFixedAssetTypes { get; set; } = null!;
        public virtual DbSet<FxaFixedAssetTypeVw> FxaFixedAssetTypeVws { get; set; } = null!;
        public virtual DbSet<FxaFixedAssetVw> FxaFixedAssetVws { get; set; } = null!;
        public virtual DbSet<FxaTransaction> FxaTransactions { get; set; } = null!;
        public virtual DbSet<FxaTransactionsAssest> FxaTransactionsAssests { get; set; } = null!;
        public virtual DbSet<FxaTransactionsAssestVw> FxaTransactionsAssestVws { get; set; } = null!;
        public virtual DbSet<FxaTransactionsProduct> FxaTransactionsProducts { get; set; } = null!;
        public virtual DbSet<FxaTransactionsProductsVw> FxaTransactionsProductsVws { get; set; } = null!;
        public virtual DbSet<FxaTransactionsRevaluation> FxaTransactionsRevaluations { get; set; } = null!;
        public virtual DbSet<FxaTransactionsRevaluationVw> FxaTransactionsRevaluationVws { get; set; } = null!;
        public virtual DbSet<FxaTransactionsType> FxaTransactionsTypes { get; set; } = null!;
        public virtual DbSet<FxaTransactionsVw> FxaTransactionsVws { get; set; } = null!;
        public virtual DbSet<HdTickect> HdTickects { get; set; } = null!;
        public virtual DbSet<HdTickectPriorityVw> HdTickectPriorityVws { get; set; } = null!;
        public virtual DbSet<HdTickectQuickreplyVw> HdTickectQuickreplyVws { get; set; } = null!;
        public virtual DbSet<HdTickectStatusVw> HdTickectStatusVws { get; set; } = null!;
        public virtual DbSet<HdTickectsAssigin> HdTickectsAssigins { get; set; } = null!;
        public virtual DbSet<HdTickectsFile> HdTickectsFiles { get; set; } = null!;
        public virtual DbSet<HdTickectsReply> HdTickectsReplies { get; set; } = null!;
        public virtual DbSet<HdTickectsReplyRpVw> HdTickectsReplyRpVws { get; set; } = null!;
        public virtual DbSet<HdTickectsReplyVw> HdTickectsReplyVws { get; set; } = null!;
        public virtual DbSet<HdTickectsVw> HdTickectsVws { get; set; } = null!;
        public virtual DbSet<HdTicketStaff> HdTicketStaffs { get; set; } = null!;
        public virtual DbSet<HotFloor> HotFloors { get; set; } = null!;
        public virtual DbSet<HotFloorsVw> HotFloorsVws { get; set; } = null!;
        public virtual DbSet<HotGroup> HotGroups { get; set; } = null!;
        public virtual DbSet<HotGroupsVw> HotGroupsVws { get; set; } = null!;
        public virtual DbSet<HotRoom> HotRooms { get; set; } = null!;
        public virtual DbSet<HotRoomAsset> HotRoomAssets { get; set; } = null!;
        public virtual DbSet<HotRoomAssetsVw> HotRoomAssetsVws { get; set; } = null!;
        public virtual DbSet<HotRoomService> HotRoomServices { get; set; } = null!;
        public virtual DbSet<HotRoomVw> HotRoomVws { get; set; } = null!;
        public virtual DbSet<HotService> HotServices { get; set; } = null!;
        public virtual DbSet<HotServicesVw> HotServicesVws { get; set; } = null!;
        public virtual DbSet<HotTransaction> HotTransactions { get; set; } = null!;
        public virtual DbSet<HotTransactionsCompanion> HotTransactionsCompanions { get; set; } = null!;
        public virtual DbSet<HotTransactionsCompanionVw> HotTransactionsCompanionVws { get; set; } = null!;
        public virtual DbSet<HotTransactionsPayment> HotTransactionsPayments { get; set; } = null!;
        public virtual DbSet<HotTransactionsRoom> HotTransactionsRooms { get; set; } = null!;
        public virtual DbSet<HotTransactionsRoomVw> HotTransactionsRoomVws { get; set; } = null!;
        public virtual DbSet<HotTransactionsService> HotTransactionsServices { get; set; } = null!;
        public virtual DbSet<HotTransactionsServicesVw> HotTransactionsServicesVws { get; set; } = null!;
        public virtual DbSet<HotTransactionsStatus> HotTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<HotTransactionsType> HotTransactionsTypes { get; set; } = null!;
        public virtual DbSet<HotTransactionsVw> HotTransactionsVws { get; set; } = null!;
        public virtual DbSet<HotTypeRoom> HotTypeRooms { get; set; } = null!;
        public virtual DbSet<HrAbsence> HrAbsences { get; set; } = null!;
        public virtual DbSet<HrAbsenceVw> HrAbsenceVws { get; set; } = null!;
        public virtual DbSet<HrActualAttendance> HrActualAttendances { get; set; } = null!;
        public virtual DbSet<HrActualAttendanceVw> HrActualAttendanceVws { get; set; } = null!;
        public virtual DbSet<HrAllowanceDeduction> HrAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrAllowanceDeductionM> HrAllowanceDeductionMs { get; set; } = null!;
        public virtual DbSet<HrAllowanceDeductionTempOrFix> HrAllowanceDeductionTempOrFixes { get; set; } = null!;
        public virtual DbSet<HrAllowanceDeductionVw> HrAllowanceDeductionVws { get; set; } = null!;
        public virtual DbSet<HrAllowanceVw> HrAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrArchiveFilesDetail> HrArchiveFilesDetails { get; set; } = null!;
        public virtual DbSet<HrArchiveFilesDetailsVw> HrArchiveFilesDetailsVws { get; set; } = null!;
        public virtual DbSet<HrArchivesFile> HrArchivesFiles { get; set; } = null!;
        public virtual DbSet<HrArchivesFilesVw> HrArchivesFilesVws { get; set; } = null!;
        public virtual DbSet<HrAssignman> HrAssignmen { get; set; } = null!;
        public virtual DbSet<HrAssignmenVw> HrAssignmenVws { get; set; } = null!;
        public virtual DbSet<HrAttAction> HrAttActions { get; set; } = null!;
        public virtual DbSet<HrAttCheckShiftEmployeeVw> HrAttCheckShiftEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrAttDay> HrAttDays { get; set; } = null!;
        public virtual DbSet<HrAttLocation> HrAttLocations { get; set; } = null!;
        public virtual DbSet<HrAttLocationEmployee> HrAttLocationEmployees { get; set; } = null!;
        public virtual DbSet<HrAttLocationEmployeeVw> HrAttLocationEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrAttShift> HrAttShifts { get; set; } = null!;
        public virtual DbSet<HrAttShiftClose> HrAttShiftCloses { get; set; } = null!;
        public virtual DbSet<HrAttShiftCloseD> HrAttShiftCloseDs { get; set; } = null!;
        public virtual DbSet<HrAttShiftCloseVw> HrAttShiftCloseVws { get; set; } = null!;
        public virtual DbSet<HrAttShiftEmployee> HrAttShiftEmployees { get; set; } = null!;
        public virtual DbSet<HrAttShiftEmployeeMVw> HrAttShiftEmployeeMVws { get; set; } = null!;
        public virtual DbSet<HrAttShiftEmployeeVw> HrAttShiftEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrAttShiftTimeTable> HrAttShiftTimeTables { get; set; } = null!;
        public virtual DbSet<HrAttShiftTimeTableEmpVw> HrAttShiftTimeTableEmpVws { get; set; } = null!;
        public virtual DbSet<HrAttShiftTimeTableVw> HrAttShiftTimeTableVws { get; set; } = null!;
        public virtual DbSet<HrAttTimeTable> HrAttTimeTables { get; set; } = null!;
        public virtual DbSet<HrAttTimeTableDay> HrAttTimeTableDays { get; set; } = null!;
        public virtual DbSet<HrAttTimeTableVw> HrAttTimeTableVws { get; set; } = null!;
        public virtual DbSet<HrAttendance> HrAttendances { get; set; } = null!;
        public virtual DbSet<HrAttendanceSoftwareVw> HrAttendanceSoftwareVws { get; set; } = null!;
        public virtual DbSet<HrAttendanceTypeVw> HrAttendanceTypeVws { get; set; } = null!;
        public virtual DbSet<HrAttendancesVw> HrAttendancesVws { get; set; } = null!;
        public virtual DbSet<HrAttendancesVw2> HrAttendancesVw2s { get; set; } = null!;
        public virtual DbSet<HrAuthorization> HrAuthorizations { get; set; } = null!;
        public virtual DbSet<HrAuthorizationVw> HrAuthorizationVws { get; set; } = null!;
        public virtual DbSet<HrCardTemplate> HrCardTemplates { get; set; } = null!;
        public virtual DbSet<HrCheckInOut> HrCheckInOuts { get; set; } = null!;
        public virtual DbSet<HrCheckInOutVw> HrCheckInOutVws { get; set; } = null!;
        public virtual DbSet<HrClearance> HrClearances { get; set; } = null!;
        public virtual DbSet<HrClearanceMonth> HrClearanceMonths { get; set; } = null!;
        public virtual DbSet<HrClearanceMonthsVw> HrClearanceMonthsVws { get; set; } = null!;
        public virtual DbSet<HrClearanceType> HrClearanceTypes { get; set; } = null!;
        public virtual DbSet<HrClearanceTypeVw> HrClearanceTypeVws { get; set; } = null!;
        public virtual DbSet<HrClearanceVw> HrClearanceVws { get; set; } = null!;
        public virtual DbSet<HrCompensatoryVacation> HrCompensatoryVacations { get; set; } = null!;
        public virtual DbSet<HrCompensatoryVacationsVw> HrCompensatoryVacationsVws { get; set; } = null!;
        public virtual DbSet<HrCompetence> HrCompetences { get; set; } = null!;
        public virtual DbSet<HrCompetencesCatagory> HrCompetencesCatagories { get; set; } = null!;
        public virtual DbSet<HrCompetencesVw> HrCompetencesVws { get; set; } = null!;
        public virtual DbSet<HrContracte> HrContractes { get; set; } = null!;
        public virtual DbSet<HrContractesVw> HrContractesVws { get; set; } = null!;
        public virtual DbSet<HrCostType> HrCostTypes { get; set; } = null!;
        public virtual DbSet<HrCostTypeVw> HrCostTypeVws { get; set; } = null!;
        public virtual DbSet<HrCustody> HrCustodies { get; set; } = null!;
        public virtual DbSet<HrCustodyItem> HrCustodyItems { get; set; } = null!;
        public virtual DbSet<HrCustodyItemsProperty> HrCustodyItemsProperties { get; set; } = null!;
        public virtual DbSet<HrCustodyItemsVw> HrCustodyItemsVws { get; set; } = null!;
        public virtual DbSet<HrCustodyMDVw> HrCustodyMDVws { get; set; } = null!;
        public virtual DbSet<HrCustodyRefranceType> HrCustodyRefranceTypes { get; set; } = null!;
        public virtual DbSet<HrCustodyType> HrCustodyTypes { get; set; } = null!;
        public virtual DbSet<HrCustodyVw> HrCustodyVws { get; set; } = null!;
        public virtual DbSet<HrDecision> HrDecisions { get; set; } = null!;
        public virtual DbSet<HrDecisionsTypeVw> HrDecisionsTypeVws { get; set; } = null!;
        public virtual DbSet<HrDecisionsVw> HrDecisionsVws { get; set; } = null!;
        public virtual DbSet<HrDeductionVw> HrDeductionVws { get; set; } = null!;
        public virtual DbSet<HrDefinitionSalaryVw> HrDefinitionSalaryVws { get; set; } = null!;
        public virtual DbSet<HrDelay> HrDelays { get; set; } = null!;
        public virtual DbSet<HrDelayVw> HrDelayVws { get; set; } = null!;
        public virtual DbSet<HrDependent> HrDependents { get; set; } = null!;
        public virtual DbSet<HrDependentsVw> HrDependentsVws { get; set; } = null!;
        public virtual DbSet<HrDirectJob> HrDirectJobs { get; set; } = null!;
        public virtual DbSet<HrDirectJobVw> HrDirectJobVws { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryActionType> HrDisciplinaryActionTypes { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryCase> HrDisciplinaryCases { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryCaseAction> HrDisciplinaryCaseActions { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryCaseActionVw> HrDisciplinaryCaseActionVws { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryRule> HrDisciplinaryRules { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryRuleVw> HrDisciplinaryRuleVws { get; set; } = null!;
        public virtual DbSet<HrDisciplinaryTemplate> HrDisciplinaryTemplates { get; set; } = null!;
        public virtual DbSet<HrEducation> HrEducations { get; set; } = null!;
        public virtual DbSet<HrEducationVw> HrEducationVws { get; set; } = null!;
        public virtual DbSet<HrEffectiveDateNotice> HrEffectiveDateNotices { get; set; } = null!;
        public virtual DbSet<HrEmpLeaveVw> HrEmpLeaveVws { get; set; } = null!;
        public virtual DbSet<HrEmpTicketVw> HrEmpTicketVws { get; set; } = null!;
        public virtual DbSet<HrEmpVacationVw> HrEmpVacationVws { get; set; } = null!;
        public virtual DbSet<HrEmpWarn> HrEmpWarns { get; set; } = null!;
        public virtual DbSet<HrEmpWarnVw> HrEmpWarnVws { get; set; } = null!;
        public virtual DbSet<HrEmpWorkTime> HrEmpWorkTimes { get; set; } = null!;
        public virtual DbSet<HrEmpWorkTimeVw> HrEmpWorkTimeVws { get; set; } = null!;
        public virtual DbSet<HrEmployee> HrEmployees { get; set; } = null!;
        public virtual DbSet<HrEmployeeCost> HrEmployeeCosts { get; set; } = null!;
        public virtual DbSet<HrEmployeeCostVw> HrEmployeeCostVws { get; set; } = null!;
        public virtual DbSet<HrEmployeeCostcenter> HrEmployeeCostcenters { get; set; } = null!;
        public virtual DbSet<HrEmployeeShiftReportVw> HrEmployeeShiftReportVws { get; set; } = null!;
        public virtual DbSet<HrEmployeeVw> HrEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrEmployeesListVw> HrEmployeesListVws { get; set; } = null!;
        public virtual DbSet<HrEvaluationAnnualIncreaseConfig> HrEvaluationAnnualIncreaseConfigs { get; set; } = null!;
        public virtual DbSet<HrFile> HrFiles { get; set; } = null!;
        public virtual DbSet<HrFlexibleWorking> HrFlexibleWorkings { get; set; } = null!;
        public virtual DbSet<HrFlexibleWorkingMaster> HrFlexibleWorkingMasters { get; set; } = null!;
        public virtual DbSet<HrFlexibleWorkingVw> HrFlexibleWorkingVws { get; set; } = null!;
        public virtual DbSet<HrGoal> HrGoals { get; set; } = null!;
        public virtual DbSet<HrGoalPlanType> HrGoalPlanTypes { get; set; } = null!;
        public virtual DbSet<HrGoalsKpi> HrGoalsKpis { get; set; } = null!;
        public virtual DbSet<HrGoalsKpiVw> HrGoalsKpiVws { get; set; } = null!;
        public virtual DbSet<HrGoalsMetric> HrGoalsMetrics { get; set; } = null!;
        public virtual DbSet<HrGoalsNote> HrGoalsNotes { get; set; } = null!;
        public virtual DbSet<HrGoalsNoteVw> HrGoalsNoteVws { get; set; } = null!;
        public virtual DbSet<HrGoalsPlan> HrGoalsPlans { get; set; } = null!;
        public virtual DbSet<HrGoalsPlansVw> HrGoalsPlansVws { get; set; } = null!;
        public virtual DbSet<HrGoalsReply> HrGoalsReplies { get; set; } = null!;
        public virtual DbSet<HrGoalsStatus> HrGoalsStatuses { get; set; } = null!;
        public virtual DbSet<HrGoalsUpdate> HrGoalsUpdates { get; set; } = null!;
        public virtual DbSet<HrGoalsUpdatesVw> HrGoalsUpdatesVws { get; set; } = null!;
        public virtual DbSet<HrGoalsVw> HrGoalsVws { get; set; } = null!;
        public virtual DbSet<HrGosi> HrGosis { get; set; } = null!;
        public virtual DbSet<HrGosiEmployee> HrGosiEmployees { get; set; } = null!;
        public virtual DbSet<HrGosiEmployeeAccVw> HrGosiEmployeeAccVws { get; set; } = null!;
        public virtual DbSet<HrGosiEmployeeVw> HrGosiEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrGosiTypeVw> HrGosiTypeVws { get; set; } = null!;
        public virtual DbSet<HrGosiVw> HrGosiVws { get; set; } = null!;
        public virtual DbSet<HrHoliday> HrHolidays { get; set; } = null!;
        public virtual DbSet<HrHolidayVw> HrHolidayVws { get; set; } = null!;
        public virtual DbSet<HrIncrement> HrIncrements { get; set; } = null!;
        public virtual DbSet<HrIncrementType> HrIncrementTypes { get; set; } = null!;
        public virtual DbSet<HrIncrementsAllowanceDeduction> HrIncrementsAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrIncrementsAllowanceVw> HrIncrementsAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrIncrementsDeductionVw> HrIncrementsDeductionVws { get; set; } = null!;
        public virtual DbSet<HrIncrementsVw> HrIncrementsVws { get; set; } = null!;
        public virtual DbSet<HrInsurance> HrInsurances { get; set; } = null!;
        public virtual DbSet<HrInsuranceCategoryVw> HrInsuranceCategoryVws { get; set; } = null!;
        public virtual DbSet<HrInsuranceEmp> HrInsuranceEmps { get; set; } = null!;
        public virtual DbSet<HrInsuranceEmpVw> HrInsuranceEmpVws { get; set; } = null!;
        public virtual DbSet<HrInsurancePolicy> HrInsurancePolicies { get; set; } = null!;
        public virtual DbSet<HrInsuranceTransactionType> HrInsuranceTransactionTypes { get; set; } = null!;
        public virtual DbSet<HrInsuranceVw> HrInsuranceVws { get; set; } = null!;
        public virtual DbSet<HrJob> HrJobs { get; set; } = null!;
        public virtual DbSet<HrJobCategoriesVw> HrJobCategoriesVws { get; set; } = null!;
        public virtual DbSet<HrJobDescription> HrJobDescriptions { get; set; } = null!;
        public virtual DbSet<HrJobEmployeeVw> HrJobEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrJobGrade> HrJobGrades { get; set; } = null!;
        public virtual DbSet<HrJobGradeVw> HrJobGradeVws { get; set; } = null!;
        public virtual DbSet<HrJobLevel> HrJobLevels { get; set; } = null!;
        public virtual DbSet<HrJobLevelsAllowanceDeduction> HrJobLevelsAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrJobOffer> HrJobOffers { get; set; } = null!;
        public virtual DbSet<HrJobOfferAdvantage> HrJobOfferAdvantages { get; set; } = null!;
        public virtual DbSet<HrJobOfferLookupDataVw> HrJobOfferLookupDataVws { get; set; } = null!;
        public virtual DbSet<HrJobOfferVw> HrJobOfferVws { get; set; } = null!;
        public virtual DbSet<HrJobProgramVw> HrJobProgramVws { get; set; } = null!;
        public virtual DbSet<HrJobStatusVw> HrJobStatusVws { get; set; } = null!;
        public virtual DbSet<HrJobTypeVw> HrJobTypeVws { get; set; } = null!;
        public virtual DbSet<HrJobVw> HrJobVws { get; set; } = null!;
        public virtual DbSet<HrKpi> HrKpis { get; set; } = null!;
        public virtual DbSet<HrKpiDetaile> HrKpiDetailes { get; set; } = null!;
        public virtual DbSet<HrKpiDetailesVw> HrKpiDetailesVws { get; set; } = null!;
        public virtual DbSet<HrKpiNote> HrKpiNotes { get; set; } = null!;
        public virtual DbSet<HrKpiNoteVw> HrKpiNoteVws { get; set; } = null!;
        public virtual DbSet<HrKpiProjectManager> HrKpiProjectManagers { get; set; } = null!;
        public virtual DbSet<HrKpiProjectManagerDetaile> HrKpiProjectManagerDetailes { get; set; } = null!;
        public virtual DbSet<HrKpiProjectManagerDetailesVw> HrKpiProjectManagerDetailesVws { get; set; } = null!;
        public virtual DbSet<HrKpiProjectManagerVw> HrKpiProjectManagerVws { get; set; } = null!;
        public virtual DbSet<HrKpiRequest> HrKpiRequests { get; set; } = null!;
        public virtual DbSet<HrKpiTemplate> HrKpiTemplates { get; set; } = null!;
        public virtual DbSet<HrKpiTemplatesCompetence> HrKpiTemplatesCompetences { get; set; } = null!;
        public virtual DbSet<HrKpiTemplatesCompetencesSub> HrKpiTemplatesCompetencesSubs { get; set; } = null!;
        public virtual DbSet<HrKpiTemplatesCompetencesVw> HrKpiTemplatesCompetencesVws { get; set; } = null!;
        public virtual DbSet<HrKpiTemplatesVw> HrKpiTemplatesVws { get; set; } = null!;
        public virtual DbSet<HrKpiType> HrKpiTypes { get; set; } = null!;
        public virtual DbSet<HrKpiVw> HrKpiVws { get; set; } = null!;
        public virtual DbSet<HrLanguage> HrLanguages { get; set; } = null!;
        public virtual DbSet<HrLanguagesVw> HrLanguagesVws { get; set; } = null!;
        public virtual DbSet<HrLeave> HrLeaves { get; set; } = null!;
        public virtual DbSet<HrLeaveType> HrLeaveTypes { get; set; } = null!;
        public virtual DbSet<HrLeaveTypeVw> HrLeaveTypeVws { get; set; } = null!;
        public virtual DbSet<HrLeaveVw> HrLeaveVws { get; set; } = null!;
        public virtual DbSet<HrLicense> HrLicenses { get; set; } = null!;
        public virtual DbSet<HrLicensesVw> HrLicensesVws { get; set; } = null!;
        public virtual DbSet<HrLoan> HrLoans { get; set; } = null!;
        public virtual DbSet<HrLoanInstallment> HrLoanInstallments { get; set; } = null!;
        public virtual DbSet<HrLoanInstallmentPayment> HrLoanInstallmentPayments { get; set; } = null!;
        public virtual DbSet<HrLoanInstallmentPaymentVw> HrLoanInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<HrLoanInstallmentVw> HrLoanInstallmentVws { get; set; } = null!;
        public virtual DbSet<HrLoanPayment> HrLoanPayments { get; set; } = null!;
        public virtual DbSet<HrLoanPaymentVw> HrLoanPaymentVws { get; set; } = null!;
        public virtual DbSet<HrLoanVw> HrLoanVws { get; set; } = null!;
        public virtual DbSet<HrLocationNoteTypeVw> HrLocationNoteTypeVws { get; set; } = null!;
        public virtual DbSet<HrMandate> HrMandates { get; set; } = null!;
        public virtual DbSet<HrMandateVw> HrMandateVws { get; set; } = null!;
        public virtual DbSet<HrMaritalStatusVw> HrMaritalStatusVws { get; set; } = null!;
        public virtual DbSet<HrMembership> HrMemberships { get; set; } = null!;
        public virtual DbSet<HrNote> HrNotes { get; set; } = null!;
        public virtual DbSet<HrNoteVw> HrNoteVws { get; set; } = null!;
        public virtual DbSet<HrNotification> HrNotifications { get; set; } = null!;
        public virtual DbSet<HrNotificationsReply> HrNotificationsReplies { get; set; } = null!;
        public virtual DbSet<HrNotificationsSetting> HrNotificationsSettings { get; set; } = null!;
        public virtual DbSet<HrNotificationsSettingVw> HrNotificationsSettingVws { get; set; } = null!;
        public virtual DbSet<HrNotificationsType> HrNotificationsTypes { get; set; } = null!;
        public virtual DbSet<HrNotificationsTypeVw> HrNotificationsTypeVws { get; set; } = null!;
        public virtual DbSet<HrNotificationsVw> HrNotificationsVws { get; set; } = null!;
        public virtual DbSet<HrOhad> HrOhads { get; set; } = null!;
        public virtual DbSet<HrOhadDetail> HrOhadDetails { get; set; } = null!;
        public virtual DbSet<HrOhadDetailsVw> HrOhadDetailsVws { get; set; } = null!;
        public virtual DbSet<HrOhadStatusVw> HrOhadStatusVws { get; set; } = null!;
        public virtual DbSet<HrOhadTransactionTypeVw> HrOhadTransactionTypeVws { get; set; } = null!;
        public virtual DbSet<HrOhadVw> HrOhadVws { get; set; } = null!;
        public virtual DbSet<HrOpeningBalance> HrOpeningBalances { get; set; } = null!;
        public virtual DbSet<HrOpeningBalanceType> HrOpeningBalanceTypes { get; set; } = null!;
        public virtual DbSet<HrOpeningBalanceVw> HrOpeningBalanceVws { get; set; } = null!;
        public virtual DbSet<HrOverTimeD> HrOverTimeDs { get; set; } = null!;
        public virtual DbSet<HrOverTimeD2Vw> HrOverTimeD2Vws { get; set; } = null!;
        public virtual DbSet<HrOverTimeDExportVw> HrOverTimeDExportVws { get; set; } = null!;
        public virtual DbSet<HrOverTimeDVw> HrOverTimeDVws { get; set; } = null!;
        public virtual DbSet<HrOverTimeM> HrOverTimeMs { get; set; } = null!;
        public virtual DbSet<HrOverTimeMVw> HrOverTimeMVws { get; set; } = null!;
        public virtual DbSet<HrPayroll> HrPayrolls { get; set; } = null!;
        public virtual DbSet<HrPayrollAllowanceAccountsVw> HrPayrollAllowanceAccountsVws { get; set; } = null!;
        public virtual DbSet<HrPayrollAllowanceDeduction> HrPayrollAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrPayrollAllowanceDeductionVw> HrPayrollAllowanceDeductionVws { get; set; } = null!;
        public virtual DbSet<HrPayrollAllowanceVw> HrPayrollAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrPayrollCostcenter> HrPayrollCostcenters { get; set; } = null!;
        public virtual DbSet<HrPayrollCostcenterVw> HrPayrollCostcenterVws { get; set; } = null!;
        public virtual DbSet<HrPayrollD> HrPayrollDs { get; set; } = null!;
        public virtual DbSet<HrPayrollDAllowanceVw> HrPayrollDAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrPayrollDPayment> HrPayrollDPayments { get; set; } = null!;
        public virtual DbSet<HrPayrollDPaymentVw> HrPayrollDPaymentVws { get; set; } = null!;
        public virtual DbSet<HrPayrollDVw> HrPayrollDVws { get; set; } = null!;
        public virtual DbSet<HrPayrollDeductionAccountsVw> HrPayrollDeductionAccountsVws { get; set; } = null!;
        public virtual DbSet<HrPayrollDeductionVw> HrPayrollDeductionVws { get; set; } = null!;
        public virtual DbSet<HrPayrollNote> HrPayrollNotes { get; set; } = null!;
        public virtual DbSet<HrPayrollNoteVw> HrPayrollNoteVws { get; set; } = null!;
        public virtual DbSet<HrPayrollPaymentTypeVw> HrPayrollPaymentTypeVws { get; set; } = null!;
        public virtual DbSet<HrPayrollStatus> HrPayrollStatuses { get; set; } = null!;
        public virtual DbSet<HrPayrollType> HrPayrollTypes { get; set; } = null!;
        public virtual DbSet<HrPayrollVw> HrPayrollVws { get; set; } = null!;
        public virtual DbSet<HrPerformance> HrPerformances { get; set; } = null!;
        public virtual DbSet<HrPerformanceForVw> HrPerformanceForVws { get; set; } = null!;
        public virtual DbSet<HrPerformanceStatusVw> HrPerformanceStatusVws { get; set; } = null!;
        public virtual DbSet<HrPerformanceTypeVw> HrPerformanceTypeVws { get; set; } = null!;
        public virtual DbSet<HrPerformanceVw> HrPerformanceVws { get; set; } = null!;
        public virtual DbSet<HrPermission> HrPermissions { get; set; } = null!;
        public virtual DbSet<HrPermissionReasonVw> HrPermissionReasonVws { get; set; } = null!;
        public virtual DbSet<HrPermissionTypeVw> HrPermissionTypeVws { get; set; } = null!;
        public virtual DbSet<HrPermissionsVw> HrPermissionsVws { get; set; } = null!;
        public virtual DbSet<HrPoliciesType> HrPoliciesTypes { get; set; } = null!;
        public virtual DbSet<HrPoliciesVw> HrPoliciesVws { get; set; } = null!;
        public virtual DbSet<HrPolicy> HrPolicies { get; set; } = null!;
        public virtual DbSet<HrPreparationSalariesVw> HrPreparationSalariesVws { get; set; } = null!;
        public virtual DbSet<HrPreparationSalary> HrPreparationSalaries { get; set; } = null!;
        public virtual DbSet<HrProvision> HrProvisions { get; set; } = null!;
        public virtual DbSet<HrProvisionsEmployee> HrProvisionsEmployees { get; set; } = null!;
        public virtual DbSet<HrProvisionsEmployeeAccVw> HrProvisionsEmployeeAccVws { get; set; } = null!;
        public virtual DbSet<HrProvisionsEmployeeVw> HrProvisionsEmployeeVws { get; set; } = null!;
        public virtual DbSet<HrProvisionsType> HrProvisionsTypes { get; set; } = null!;
        public virtual DbSet<HrProvisionsVw> HrProvisionsVws { get; set; } = null!;
        public virtual DbSet<HrPsAllowanceDeduction> HrPsAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrPsAllowanceVw> HrPsAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrPsDeductionVw> HrPsDeductionVws { get; set; } = null!;
        public virtual DbSet<HrQualificationVw> HrQualificationVws { get; set; } = null!;
        public virtual DbSet<HrRateType> HrRateTypes { get; set; } = null!;
        public virtual DbSet<HrRateTypeVw> HrRateTypeVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentAllowanceDeduction> HrRecruitmentAllowanceDeductions { get; set; } = null!;
        public virtual DbSet<HrRecruitmentAllowanceVw> HrRecruitmentAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentApplicantKpiDVw> HrRecruitmentApplicantKpiDVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentApplicantKpiVw> HrRecruitmentApplicantKpiVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentApplication> HrRecruitmentApplications { get; set; } = null!;
        public virtual DbSet<HrRecruitmentApplicationVw> HrRecruitmentApplicationVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidate> HrRecruitmentCandidates { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateApplicationVw> HrRecruitmentCandidateApplicationVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateKpi> HrRecruitmentCandidateKpis { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateKpiD> HrRecruitmentCandidateKpiDs { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateKpiDVw> HrRecruitmentCandidateKpiDVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateKpiVw> HrRecruitmentCandidateKpiVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentCandidateVw> HrRecruitmentCandidateVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentDeductionVw> HrRecruitmentDeductionVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentEvaluationMember> HrRecruitmentEvaluationMembers { get; set; } = null!;
        public virtual DbSet<HrRecruitmentInterview> HrRecruitmentInterviews { get; set; } = null!;
        public virtual DbSet<HrRecruitmentInterviewVw> HrRecruitmentInterviewVws { get; set; } = null!;
        public virtual DbSet<HrRecruitmentVacancy> HrRecruitmentVacancies { get; set; } = null!;
        public virtual DbSet<HrRecruitmentVacancyVw> HrRecruitmentVacancyVws { get; set; } = null!;
        public virtual DbSet<HrRequest> HrRequests { get; set; } = null!;
        public virtual DbSet<HrRequestDetaile> HrRequestDetailes { get; set; } = null!;
        public virtual DbSet<HrRequestDetailesVw> HrRequestDetailesVws { get; set; } = null!;
        public virtual DbSet<HrRequestType> HrRequestTypes { get; set; } = null!;
        public virtual DbSet<HrRequestVw> HrRequestVws { get; set; } = null!;
        public virtual DbSet<HrSalaryGroup> HrSalaryGroups { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupAccount> HrSalaryGroupAccounts { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupAllowanceVw> HrSalaryGroupAllowanceVws { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupDeductionVw> HrSalaryGroupDeductionVws { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupRefrance> HrSalaryGroupRefrances { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupRefranceVw> HrSalaryGroupRefranceVws { get; set; } = null!;
        public virtual DbSet<HrSalaryGroupVw> HrSalaryGroupVws { get; set; } = null!;
        public virtual DbSet<HrSetting> HrSettings { get; set; } = null!;
        public virtual DbSet<HrSkill> HrSkills { get; set; } = null!;
        public virtual DbSet<HrSkillsVw> HrSkillsVws { get; set; } = null!;
        public virtual DbSet<HrSpecializationVw> HrSpecializationVws { get; set; } = null!;
        public virtual DbSet<HrSponserVw> HrSponserVws { get; set; } = null!;
        public virtual DbSet<HrSupportHrdf> HrSupportHrdfs { get; set; } = null!;
        public virtual DbSet<HrTicket> HrTickets { get; set; } = null!;
        public virtual DbSet<HrTicketVw> HrTicketVws { get; set; } = null!;
        public virtual DbSet<HrTrainingBag> HrTrainingBags { get; set; } = null!;
        public virtual DbSet<HrTrainingBagVw> HrTrainingBagVws { get; set; } = null!;
        public virtual DbSet<HrTransaction> HrTransactions { get; set; } = null!;
        public virtual DbSet<HrTransactionType> HrTransactionTypes { get; set; } = null!;
        public virtual DbSet<HrTransactionVw> HrTransactionVws { get; set; } = null!;
        public virtual DbSet<HrTransfer> HrTransfers { get; set; } = null!;
        public virtual DbSet<HrTransfersVw> HrTransfersVws { get; set; } = null!;
        public virtual DbSet<HrUpgradeVw> HrUpgradeVws { get; set; } = null!;
        public virtual DbSet<HrVacancyStatusVw> HrVacancyStatusVws { get; set; } = null!;
        public virtual DbSet<HrVacation> HrVacations { get; set; } = null!;
        public virtual DbSet<HrVacationBalance> HrVacationBalances { get; set; } = null!;
        public virtual DbSet<HrVacationBalanceVw> HrVacationBalanceVws { get; set; } = null!;
        public virtual DbSet<HrVacationDue> HrVacationDues { get; set; } = null!;
        public virtual DbSet<HrVacationRequest> HrVacationRequests { get; set; } = null!;
        public virtual DbSet<HrVacationRequestVw> HrVacationRequestVws { get; set; } = null!;
        public virtual DbSet<HrVacationSetting> HrVacationSettings { get; set; } = null!;
        public virtual DbSet<HrVacationStatus> HrVacationStatuses { get; set; } = null!;
        public virtual DbSet<HrVacationsCatagory> HrVacationsCatagories { get; set; } = null!;
        public virtual DbSet<HrVacationsNote> HrVacationsNotes { get; set; } = null!;
        public virtual DbSet<HrVacationsType> HrVacationsTypes { get; set; } = null!;
        public virtual DbSet<HrVacationsTypeVw> HrVacationsTypeVws { get; set; } = null!;
        public virtual DbSet<HrVacationsVw> HrVacationsVws { get; set; } = null!;
        public virtual DbSet<HrVisa> HrVisas { get; set; } = null!;
        public virtual DbSet<HrVisaVw> HrVisaVws { get; set; } = null!;
        public virtual DbSet<HrVisitLocationAttendance> HrVisitLocationAttendances { get; set; } = null!;
        public virtual DbSet<HrVisitLocationAttendanceResource> HrVisitLocationAttendanceResources { get; set; } = null!;
        public virtual DbSet<HrVisitLocationAttendanceResourcesVw> HrVisitLocationAttendanceResourcesVws { get; set; } = null!;
        public virtual DbSet<HrVisitLocationAttendanceVw> HrVisitLocationAttendanceVws { get; set; } = null!;
        public virtual DbSet<HrVisitLocationNote> HrVisitLocationNotes { get; set; } = null!;
        public virtual DbSet<HrVisitLocationNoteVw> HrVisitLocationNoteVws { get; set; } = null!;
        public virtual DbSet<HrVisitSchedule> HrVisitSchedules { get; set; } = null!;
        public virtual DbSet<HrVisitScheduleLocation> HrVisitScheduleLocations { get; set; } = null!;
        public virtual DbSet<HrVisitScheduleLocationStep> HrVisitScheduleLocationSteps { get; set; } = null!;
        public virtual DbSet<HrVisitScheduleLocationStepsVw> HrVisitScheduleLocationStepsVws { get; set; } = null!;
        public virtual DbSet<HrVisitScheduleLocationVw> HrVisitScheduleLocationVws { get; set; } = null!;
        public virtual DbSet<HrVisitStatusVw> HrVisitStatusVws { get; set; } = null!;
        public virtual DbSet<HrVisitStep> HrVisitSteps { get; set; } = null!;
        public virtual DbSet<HrWagesProtectionVw> HrWagesProtectionVws { get; set; } = null!;
        public virtual DbSet<HrWeekend> HrWeekends { get; set; } = null!;
        public virtual DbSet<HrWeekendVw> HrWeekendVws { get; set; } = null!;
        public virtual DbSet<HrWorkExperience> HrWorkExperiences { get; set; } = null!;
        public virtual DbSet<IntegraField> IntegraFields { get; set; } = null!;
        public virtual DbSet<IntegraPropertiesVw> IntegraPropertiesVws { get; set; } = null!;
        public virtual DbSet<IntegraProperty> IntegraProperties { get; set; } = null!;
        public virtual DbSet<IntegraPropertyValue> IntegraPropertyValues { get; set; } = null!;
        public virtual DbSet<IntegraPropertyValuesVw> IntegraPropertyValuesVws { get; set; } = null!;
        public virtual DbSet<IntegraSsoAuth> IntegraSsoAuths { get; set; } = null!;
        public virtual DbSet<IntegraSystem> IntegraSystems { get; set; } = null!;
        public virtual DbSet<IntegraTable> IntegraTables { get; set; } = null!;
        public virtual DbSet<IntegraTableValue> IntegraTableValues { get; set; } = null!;
        public virtual DbSet<InvestBranch> InvestBranches { get; set; } = null!;
        public virtual DbSet<InvestBranchVw> InvestBranchVws { get; set; } = null!;
        public virtual DbSet<InvestEmployee> InvestEmployees { get; set; } = null!;
        public virtual DbSet<InvestEmployeeVvw> InvestEmployeeVvws { get; set; } = null!;
        public virtual DbSet<InvestMonth> InvestMonths { get; set; } = null!;
        public virtual DbSet<LgxmgAppSubscription> LgxmgAppSubscriptions { get; set; } = null!;
        public virtual DbSet<LgxmgAppSubscriptionsHistory> LgxmgAppSubscriptionsHistories { get; set; } = null!;
        public virtual DbSet<LgxmgAppSubscriptionsHistoryVw> LgxmgAppSubscriptionsHistoryVws { get; set; } = null!;
        public virtual DbSet<LgxmgAppSubscriptionsVw> LgxmgAppSubscriptionsVws { get; set; } = null!;
        public virtual DbSet<LgxmgCompany> LgxmgCompanies { get; set; } = null!;
        public virtual DbSet<LgxmgCustomersVw> LgxmgCustomersVws { get; set; } = null!;
        public virtual DbSet<LgxmgDomain> LgxmgDomains { get; set; } = null!;
        public virtual DbSet<LgxmgDomainFacilitiesVw> LgxmgDomainFacilitiesVws { get; set; } = null!;
        public virtual DbSet<LgxmgDomainFacility> LgxmgDomainFacilities { get; set; } = null!;
        public virtual DbSet<LgxmgDomainUpdate> LgxmgDomainUpdates { get; set; } = null!;
        public virtual DbSet<LgxmgDomainUpdatesVw> LgxmgDomainUpdatesVws { get; set; } = null!;
        public virtual DbSet<LgxmgDomainsHistory> LgxmgDomainsHistories { get; set; } = null!;
        public virtual DbSet<LgxmgDomainsVw> LgxmgDomainsVws { get; set; } = null!;
        public virtual DbSet<LgxmgLocation> LgxmgLocations { get; set; } = null!;
        public virtual DbSet<LgxmgMobileApp> LgxmgMobileApps { get; set; } = null!;
        public virtual DbSet<LgxmgPleskSubscription> LgxmgPleskSubscriptions { get; set; } = null!;
        public virtual DbSet<LgxmgPleskSubscriptionsHistory> LgxmgPleskSubscriptionsHistories { get; set; } = null!;
        public virtual DbSet<LgxmgServer> LgxmgServers { get; set; } = null!;
        public virtual DbSet<LgxmgServersHistory> LgxmgServersHistories { get; set; } = null!;
        public virtual DbSet<LgxmgServersVw> LgxmgServersVws { get; set; } = null!;
        public virtual DbSet<LgxmgSslCertificate> LgxmgSslCertificates { get; set; } = null!;
        public virtual DbSet<LgxmgSslCertificatesHistory> LgxmgSslCertificatesHistories { get; set; } = null!;
        public virtual DbSet<LgxmgSslCertificatesVw> LgxmgSslCertificatesVws { get; set; } = null!;
        public virtual DbSet<LgxmgSupportSubscription> LgxmgSupportSubscriptions { get; set; } = null!;
        public virtual DbSet<LgxmgSupportSubscriptionsHistory> LgxmgSupportSubscriptionsHistories { get; set; } = null!;
        public virtual DbSet<LgxmgSupportSubscriptionsVw> LgxmgSupportSubscriptionsVws { get; set; } = null!;
        public virtual DbSet<LgxmgUpdate> LgxmgUpdates { get; set; } = null!;
        public virtual DbSet<LookupDataServiceTypeVw> LookupDataServiceTypeVws { get; set; } = null!;
        public virtual DbSet<MaintChangeStatusComment> MaintChangeStatusComments { get; set; } = null!;
        public virtual DbSet<MaintChangeStatusCommentsVw> MaintChangeStatusCommentsVws { get; set; } = null!;
        public virtual DbSet<MaintContract> MaintContracts { get; set; } = null!;
        public virtual DbSet<MaintContractInstallment> MaintContractInstallments { get; set; } = null!;
        public virtual DbSet<MaintContractInstallmentPayment> MaintContractInstallmentPayments { get; set; } = null!;
        public virtual DbSet<MaintContractInstallmentPaymentVw> MaintContractInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<MaintContractInstallmentVw> MaintContractInstallmentVws { get; set; } = null!;
        public virtual DbSet<MaintContractRenew> MaintContractRenews { get; set; } = null!;
        public virtual DbSet<MaintContractVw> MaintContractVws { get; set; } = null!;
        public virtual DbSet<MaintMaintenanceAssetsVw> MaintMaintenanceAssetsVws { get; set; } = null!;
        public virtual DbSet<MaintMaintenanceType> MaintMaintenanceTypes { get; set; } = null!;
        public virtual DbSet<MaintRequest> MaintRequests { get; set; } = null!;
        public virtual DbSet<MaintRequestStatus> MaintRequestStatuses { get; set; } = null!;
        public virtual DbSet<MaintRequestsAsset> MaintRequestsAssets { get; set; } = null!;
        public virtual DbSet<MaintRequestsAssetsVw> MaintRequestsAssetsVws { get; set; } = null!;
        public virtual DbSet<MaintRequestsChangeStatus> MaintRequestsChangeStatuses { get; set; } = null!;
        public virtual DbSet<MaintRequestsChangeStatusVw> MaintRequestsChangeStatusVws { get; set; } = null!;
        public virtual DbSet<MaintRequestsItem> MaintRequestsItems { get; set; } = null!;
        public virtual DbSet<MaintRequestsItemsVw> MaintRequestsItemsVws { get; set; } = null!;
        public virtual DbSet<MaintRequestsVw> MaintRequestsVws { get; set; } = null!;
        public virtual DbSet<MaintStatus> MaintStatuses { get; set; } = null!;
        public virtual DbSet<MaintTicket> MaintTickets { get; set; } = null!;
        public virtual DbSet<MaintTicketsComment> MaintTicketsComments { get; set; } = null!;
        public virtual DbSet<MaintTicketsDamageCustomer> MaintTicketsDamageCustomers { get; set; } = null!;
        public virtual DbSet<MaintTicketsDevice> MaintTicketsDevices { get; set; } = null!;
        public virtual DbSet<MaintTicketsPart> MaintTicketsParts { get; set; } = null!;
        public virtual DbSet<MaintTicketsPartsVw> MaintTicketsPartsVws { get; set; } = null!;
        public virtual DbSet<MaintTicketsReferral> MaintTicketsReferrals { get; set; } = null!;
        public virtual DbSet<MaintTicketsReferralsVw> MaintTicketsReferralsVws { get; set; } = null!;
        public virtual DbSet<MaintTicketsRepaier> MaintTicketsRepaiers { get; set; } = null!;
        public virtual DbSet<MaintTicketsRepaierVw> MaintTicketsRepaierVws { get; set; } = null!;
        public virtual DbSet<MaintTicketsVw> MaintTicketsVws { get; set; } = null!;
        public virtual DbSet<MaintTransaction> MaintTransactions { get; set; } = null!;
        public virtual DbSet<MaintTransactionsInstallment> MaintTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<MaintTransactionsInstallmentsVw> MaintTransactionsInstallmentsVws { get; set; } = null!;
        public virtual DbSet<MaintTransactionsVw> MaintTransactionsVws { get; set; } = null!;
        public virtual DbSet<MaintType> MaintTypes { get; set; } = null!;
        public virtual DbSet<MaintTypeDamageSupVw> MaintTypeDamageSupVws { get; set; } = null!;
        public virtual DbSet<MaintTypeDamageVw> MaintTypeDamageVws { get; set; } = null!;
        public virtual DbSet<MaintTypeVw> MaintTypeVws { get; set; } = null!;
        public virtual DbSet<MaintVisit> MaintVisits { get; set; } = null!;
        public virtual DbSet<MaintVisitVw> MaintVisitVws { get; set; } = null!;
        public virtual DbSet<MonthDay> MonthDays { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterial> MrpBillOfMaterials { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsComponent> MrpBillOfMaterialsComponents { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsComponentsVw> MrpBillOfMaterialsComponentsVws { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsExpense> MrpBillOfMaterialsExpenses { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsExpensesVw> MrpBillOfMaterialsExpensesVws { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsListVw> MrpBillOfMaterialsListVws { get; set; } = null!;
        public virtual DbSet<MrpBillOfMaterialsVw> MrpBillOfMaterialsVws { get; set; } = null!;
        public virtual DbSet<MrpExpense> MrpExpenses { get; set; } = null!;
        public virtual DbSet<MrpExpensesList> MrpExpensesLists { get; set; } = null!;
        public virtual DbSet<MrpExpensesVw> MrpExpensesVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingEqu> MrpManufacturingEqus { get; set; } = null!;
        public virtual DbSet<MrpManufacturingEquVw> MrpManufacturingEquVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingExpense> MrpManufacturingExpenses { get; set; } = null!;
        public virtual DbSet<MrpManufacturingExpensesVw> MrpManufacturingExpensesVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrder> MrpManufacturingOrders { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderCost> MrpManufacturingOrderCosts { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderCostVw> MrpManufacturingOrderCostVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderProduct> MrpManufacturingOrderProducts { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderProductVw> MrpManufacturingOrderProductVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderStatus> MrpManufacturingOrderStatuses { get; set; } = null!;
        public virtual DbSet<MrpManufacturingOrderVw> MrpManufacturingOrderVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingResource> MrpManufacturingResources { get; set; } = null!;
        public virtual DbSet<MrpManufacturingResourcesVw> MrpManufacturingResourcesVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStaff> MrpManufacturingStaffs { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStaffVw> MrpManufacturingStaffVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStep> MrpManufacturingSteps { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStepsDetaile> MrpManufacturingStepsDetailes { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStepsDetailesVw> MrpManufacturingStepsDetailesVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStepsVw> MrpManufacturingStepsVws { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStoppingResuming> MrpManufacturingStoppingResumings { get; set; } = null!;
        public virtual DbSet<MrpManufacturingStoppingResumingVw> MrpManufacturingStoppingResumingVws { get; set; } = null!;
        public virtual DbSet<MrpProduction> MrpProductions { get; set; } = null!;
        public virtual DbSet<MrpProductionsDamaged> MrpProductionsDamageds { get; set; } = null!;
        public virtual DbSet<MrpProductionsDamagedItem> MrpProductionsDamagedItems { get; set; } = null!;
        public virtual DbSet<MrpProductionsDamagedItemsVw> MrpProductionsDamagedItemsVws { get; set; } = null!;
        public virtual DbSet<MrpProductionsDamagedVw> MrpProductionsDamagedVws { get; set; } = null!;
        public virtual DbSet<MrpProductionsReceiving> MrpProductionsReceivings { get; set; } = null!;
        public virtual DbSet<MrpProductionsReceivingVw> MrpProductionsReceivingVws { get; set; } = null!;
        public virtual DbSet<MrpProductionsVw> MrpProductionsVws { get; set; } = null!;
        public virtual DbSet<MrpStep> MrpSteps { get; set; } = null!;
        public virtual DbSet<MrpTransactionsType> MrpTransactionsTypes { get; set; } = null!;
        public virtual DbSet<MuqeemTransaction> MuqeemTransactions { get; set; } = null!;
        public virtual DbSet<MuqeemTransactionsType> MuqeemTransactionsTypes { get; set; } = null!;
        public virtual DbSet<MuqeemTransactionsVw> MuqeemTransactionsVws { get; set; } = null!;
        public virtual DbSet<PmBudgetType> PmBudgetTypes { get; set; } = null!;
        public virtual DbSet<PmChangeRequest> PmChangeRequests { get; set; } = null!;
        public virtual DbSet<PmChangeRequestVw> PmChangeRequestVws { get; set; } = null!;
        public virtual DbSet<PmCrewStaff> PmCrewStaffs { get; set; } = null!;
        public virtual DbSet<PmCrewStaffVw> PmCrewStaffVws { get; set; } = null!;
        public virtual DbSet<PmDeliverableTransaction> PmDeliverableTransactions { get; set; } = null!;
        public virtual DbSet<PmDeliverableTransactionsDetail> PmDeliverableTransactionsDetails { get; set; } = null!;
        public virtual DbSet<PmDeliverableTransactionsDetailsVw> PmDeliverableTransactionsDetailsVws { get; set; } = null!;
        public virtual DbSet<PmDeliverableTransactionsVw> PmDeliverableTransactionsVws { get; set; } = null!;
        public virtual DbSet<PmDurationTypeVw> PmDurationTypeVws { get; set; } = null!;
        public virtual DbSet<PmDynamicAttribute> PmDynamicAttributes { get; set; } = null!;
        public virtual DbSet<PmDynamicAttributesTable> PmDynamicAttributesTables { get; set; } = null!;
        public virtual DbSet<PmDynamicValue> PmDynamicValues { get; set; } = null!;
        public virtual DbSet<PmExtractAdditionalType> PmExtractAdditionalTypes { get; set; } = null!;
        public virtual DbSet<PmExtractAdditionalTypeListVw> PmExtractAdditionalTypeListVws { get; set; } = null!;
        public virtual DbSet<PmExtractAdditionalTypeVw> PmExtractAdditionalTypeVws { get; set; } = null!;
        public virtual DbSet<PmExtractPaymentType> PmExtractPaymentTypes { get; set; } = null!;
        public virtual DbSet<PmExtractTemporary> PmExtractTemporarys { get; set; } = null!;
        public virtual DbSet<PmExtractTemporaryProduct> PmExtractTemporaryProducts { get; set; } = null!;
        public virtual DbSet<PmExtractTemporarysVw> PmExtractTemporarysVws { get; set; } = null!;
        public virtual DbSet<PmExtractTransaction> PmExtractTransactions { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsAdditional> PmExtractTransactionsAdditionals { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsAdditionalVw> PmExtractTransactionsAdditionalVws { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsChangeStatus> PmExtractTransactionsChangeStatuses { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsDiscount> PmExtractTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsDiscountVw> PmExtractTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsPayment> PmExtractTransactionsPayments { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsProduct> PmExtractTransactionsProducts { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsProductsVw> PmExtractTransactionsProductsVws { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsStatus> PmExtractTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsType> PmExtractTransactionsTypes { get; set; } = null!;
        public virtual DbSet<PmExtractTransactionsVw> PmExtractTransactionsVws { get; set; } = null!;
        public virtual DbSet<PmGroup> PmGroups { get; set; } = null!;
        public virtual DbSet<PmGroupStaff> PmGroupStaffs { get; set; } = null!;
        public virtual DbSet<PmGroupStaffVw> PmGroupStaffVws { get; set; } = null!;
        public virtual DbSet<PmGroupVw> PmGroupVws { get; set; } = null!;
        public virtual DbSet<PmHumanRight> PmHumanRights { get; set; } = null!;
        public virtual DbSet<PmInvestigationQuestion> PmInvestigationQuestions { get; set; } = null!;
        public virtual DbSet<PmInvestigationRequest> PmInvestigationRequests { get; set; } = null!;
        public virtual DbSet<PmInvestigationRequestVw> PmInvestigationRequestVws { get; set; } = null!;
        public virtual DbSet<PmInvestigationStaff> PmInvestigationStaffs { get; set; } = null!;
        public virtual DbSet<PmJobRole> PmJobRoles { get; set; } = null!;
        public virtual DbSet<PmJobRolesVw> PmJobRolesVws { get; set; } = null!;
        public virtual DbSet<PmJobsSalary> PmJobsSalaries { get; set; } = null!;
        public virtual DbSet<PmJobsSalaryVw> PmJobsSalaryVws { get; set; } = null!;
        public virtual DbSet<PmJudicialAuthorityVw> PmJudicialAuthorityVws { get; set; } = null!;
        public virtual DbSet<PmKickOff> PmKickOffs { get; set; } = null!;
        public virtual DbSet<PmKickOffVw> PmKickOffVws { get; set; } = null!;
        public virtual DbSet<PmLookUpCatagory> PmLookUpCatagories { get; set; } = null!;
        public virtual DbSet<PmLookUpDatum> PmLookUpData { get; set; } = null!;
        public virtual DbSet<PmMemosPreparation> PmMemosPreparations { get; set; } = null!;
        public virtual DbSet<PmMemosPreparationVw> PmMemosPreparationVws { get; set; } = null!;
        public virtual DbSet<PmOperationalControl> PmOperationalControls { get; set; } = null!;
        public virtual DbSet<PmOperationalControlsVw> PmOperationalControlsVws { get; set; } = null!;
        public virtual DbSet<PmProject> PmProjects { get; set; } = null!;
        public virtual DbSet<PmProjectAccount> PmProjectAccounts { get; set; } = null!;
        public virtual DbSet<PmProjectCommPlan> PmProjectCommPlans { get; set; } = null!;
        public virtual DbSet<PmProjectEntry> PmProjectEntries { get; set; } = null!;
        public virtual DbSet<PmProjectEntryVw> PmProjectEntryVws { get; set; } = null!;
        public virtual DbSet<PmProjectEscalation> PmProjectEscalations { get; set; } = null!;
        public virtual DbSet<PmProjectEscalationVw> PmProjectEscalationVws { get; set; } = null!;
        public virtual DbSet<PmProjectKpi> PmProjectKpis { get; set; } = null!;
        public virtual DbSet<PmProjectKpiD> PmProjectKpiDs { get; set; } = null!;
        public virtual DbSet<PmProjectKpiDVw> PmProjectKpiDVws { get; set; } = null!;
        public virtual DbSet<PmProjectKpiVw> PmProjectKpiVws { get; set; } = null!;
        public virtual DbSet<PmProjectPlan> PmProjectPlans { get; set; } = null!;
        public virtual DbSet<PmProjectPlansVw> PmProjectPlansVws { get; set; } = null!;
        public virtual DbSet<PmProjectReactive> PmProjectReactives { get; set; } = null!;
        public virtual DbSet<PmProjectReactiveVw> PmProjectReactiveVws { get; set; } = null!;
        public virtual DbSet<PmProjectResource> PmProjectResources { get; set; } = null!;
        public virtual DbSet<PmProjectResourcesRole> PmProjectResourcesRoles { get; set; } = null!;
        public virtual DbSet<PmProjectResourcesRolesVw> PmProjectResourcesRolesVws { get; set; } = null!;
        public virtual DbSet<PmProjectResourcesVw> PmProjectResourcesVws { get; set; } = null!;
        public virtual DbSet<PmProjectStakeholderTypeVw> PmProjectStakeholderTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectStatusVw> PmProjectStatusVws { get; set; } = null!;
        public virtual DbSet<PmProjectStepsVw> PmProjectStepsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsAddDeduc> PmProjectsAddDeducs { get; set; } = null!;
        public virtual DbSet<PmProjectsAddDeducVw> PmProjectsAddDeducVws { get; set; } = null!;
        public virtual DbSet<PmProjectsAppVw> PmProjectsAppVws { get; set; } = null!;
        public virtual DbSet<PmProjectsAssumption> PmProjectsAssumptions { get; set; } = null!;
        public virtual DbSet<PmProjectsBudget> PmProjectsBudgets { get; set; } = null!;
        public virtual DbSet<PmProjectsBudgetItem> PmProjectsBudgetItems { get; set; } = null!;
        public virtual DbSet<PmProjectsBudgetItemsVw> PmProjectsBudgetItemsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsBudgetType> PmProjectsBudgetTypes { get; set; } = null!;
        public virtual DbSet<PmProjectsBudgetTypeVw> PmProjectsBudgetTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsBudgetVw> PmProjectsBudgetVws { get; set; } = null!;
        public virtual DbSet<PmProjectsClosing> PmProjectsClosings { get; set; } = null!;
        public virtual DbSet<PmProjectsClosingVw> PmProjectsClosingVws { get; set; } = null!;
        public virtual DbSet<PmProjectsD> PmProjectsDs { get; set; } = null!;
        public virtual DbSet<PmProjectsDeliverable> PmProjectsDeliverables { get; set; } = null!;
        public virtual DbSet<PmProjectsDeliverableAcceptCriteriaVw> PmProjectsDeliverableAcceptCriteriaVws { get; set; } = null!;
        public virtual DbSet<PmProjectsDeliverableAcceptCriterion> PmProjectsDeliverableAcceptCriteria { get; set; } = null!;
        public virtual DbSet<PmProjectsDeliverableVw> PmProjectsDeliverableVws { get; set; } = null!;
        public virtual DbSet<PmProjectsEditVw> PmProjectsEditVws { get; set; } = null!;
        public virtual DbSet<PmProjectsExternalStaff> PmProjectsExternalStaffs { get; set; } = null!;
        public virtual DbSet<PmProjectsExternalStaffVw> PmProjectsExternalStaffVws { get; set; } = null!;
        public virtual DbSet<PmProjectsFile> PmProjectsFiles { get; set; } = null!;
        public virtual DbSet<PmProjectsFilesVw> PmProjectsFilesVws { get; set; } = null!;
        public virtual DbSet<PmProjectsFinancialCost> PmProjectsFinancialCosts { get; set; } = null!;
        public virtual DbSet<PmProjectsFinancialCostsVw> PmProjectsFinancialCostsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsGovernance> PmProjectsGovernances { get; set; } = null!;
        public virtual DbSet<PmProjectsGuarantee> PmProjectsGuarantees { get; set; } = null!;
        public virtual DbSet<PmProjectsGuaranteeVw> PmProjectsGuaranteeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsInstallment> PmProjectsInstallments { get; set; } = null!;
        public virtual DbSet<PmProjectsInstallmentPayment> PmProjectsInstallmentPayments { get; set; } = null!;
        public virtual DbSet<PmProjectsInstallmentPaymentVw> PmProjectsInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<PmProjectsInstallmentVw> PmProjectsInstallmentVws { get; set; } = null!;
        public virtual DbSet<PmProjectsInterconnection> PmProjectsInterconnections { get; set; } = null!;
        public virtual DbSet<PmProjectsInterconnectionVw> PmProjectsInterconnectionVws { get; set; } = null!;
        public virtual DbSet<PmProjectsItem> PmProjectsItems { get; set; } = null!;
        public virtual DbSet<PmProjectsItemsVw> PmProjectsItemsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsKpiVw> PmProjectsKpiVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsFollowApplyTypeVw> PmProjectsLessonsFollowApplyTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsImpactTypeVw> PmProjectsLessonsImpactTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsLearnedDetail> PmProjectsLessonsLearnedDetails { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsLearnedDetailsVw> PmProjectsLessonsLearnedDetailsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsLearnedMaster> PmProjectsLessonsLearnedMasters { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsLearnedMasterVw> PmProjectsLessonsLearnedMasterVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsLessonLearnedCatsVw> PmProjectsLessonsLessonLearnedCatsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsLessonsProcedureTypeVw> PmProjectsLessonsProcedureTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsListVw> PmProjectsListVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReport> PmProjectsMonthlyReports { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportAllProject> PmProjectsMonthlyReportAllProjects { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportAllProjectsVw> PmProjectsMonthlyReportAllProjectsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportClosed> PmProjectsMonthlyReportCloseds { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportClosedVw> PmProjectsMonthlyReportClosedVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportDelayed> PmProjectsMonthlyReportDelayeds { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportDelayedVw> PmProjectsMonthlyReportDelayedVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportRecommandation> PmProjectsMonthlyReportRecommandations { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportRisk> PmProjectsMonthlyReportRisks { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportRisksVw> PmProjectsMonthlyReportRisksVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportSupport> PmProjectsMonthlyReportSupports { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportSupportVw> PmProjectsMonthlyReportSupportVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportsSectionsVw> PmProjectsMonthlyReportsSectionsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsMonthlyReportsVw> PmProjectsMonthlyReportsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsNote> PmProjectsNotes { get; set; } = null!;
        public virtual DbSet<PmProjectsObjective> PmProjectsObjectives { get; set; } = null!;
        public virtual DbSet<PmProjectsPaymentPlan> PmProjectsPaymentPlans { get; set; } = null!;
        public virtual DbSet<PmProjectsPerformanceIndicator> PmProjectsPerformanceIndicators { get; set; } = null!;
        public virtual DbSet<PmProjectsRequiredSupport> PmProjectsRequiredSupports { get; set; } = null!;
        public virtual DbSet<PmProjectsRequiredSupportVw> PmProjectsRequiredSupportVws { get; set; } = null!;
        public virtual DbSet<PmProjectsResource> PmProjectsResources { get; set; } = null!;
        public virtual DbSet<PmProjectsResourcesVw> PmProjectsResourcesVws { get; set; } = null!;
        public virtual DbSet<PmProjectsRisk> PmProjectsRisks { get; set; } = null!;
        public virtual DbSet<PmProjectsRisksVw> PmProjectsRisksVws { get; set; } = null!;
        public virtual DbSet<PmProjectsRisksVw2> PmProjectsRisksVw2s { get; set; } = null!;
        public virtual DbSet<PmProjectsStaff> PmProjectsStaffs { get; set; } = null!;
        public virtual DbSet<PmProjectsStaffType> PmProjectsStaffTypes { get; set; } = null!;
        public virtual DbSet<PmProjectsStaffVw> PmProjectsStaffVws { get; set; } = null!;
        public virtual DbSet<PmProjectsStatementRequest> PmProjectsStatementRequests { get; set; } = null!;
        public virtual DbSet<PmProjectsStatementRequestVw> PmProjectsStatementRequestVws { get; set; } = null!;
        public virtual DbSet<PmProjectsStatus> PmProjectsStatuses { get; set; } = null!;
        public virtual DbSet<PmProjectsStatusVw> PmProjectsStatusVws { get; set; } = null!;
        public virtual DbSet<PmProjectsStokeholder> PmProjectsStokeholders { get; set; } = null!;
        public virtual DbSet<PmProjectsStokeholderVw> PmProjectsStokeholderVws { get; set; } = null!;
        public virtual DbSet<PmProjectsStrategicLink> PmProjectsStrategicLinks { get; set; } = null!;
        public virtual DbSet<PmProjectsStrategicLinkVw> PmProjectsStrategicLinkVws { get; set; } = null!;
        public virtual DbSet<PmProjectsTransaction> PmProjectsTransactions { get; set; } = null!;
        public virtual DbSet<PmProjectsTransactionType> PmProjectsTransactionTypes { get; set; } = null!;
        public virtual DbSet<PmProjectsTransactionsActivity> PmProjectsTransactionsActivities { get; set; } = null!;
        public virtual DbSet<PmProjectsTransactionsItem> PmProjectsTransactionsItems { get; set; } = null!;
        public virtual DbSet<PmProjectsTransactionsItemsVw> PmProjectsTransactionsItemsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsTransactionsVw> PmProjectsTransactionsVws { get; set; } = null!;
        public virtual DbSet<PmProjectsTreePart> PmProjectsTreeParts { get; set; } = null!;
        public virtual DbSet<PmProjectsType> PmProjectsTypes { get; set; } = null!;
        public virtual DbSet<PmProjectsTypeTable> PmProjectsTypeTables { get; set; } = null!;
        public virtual DbSet<PmProjectsTypeVw> PmProjectsTypeVws { get; set; } = null!;
        public virtual DbSet<PmProjectsUpdate> PmProjectsUpdates { get; set; } = null!;
        public virtual DbSet<PmProjectsVw> PmProjectsVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrder> PmPurchaseOrders { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderExstantion> PmPurchaseOrderExstantions { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderExstantionVw> PmPurchaseOrderExstantionVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderItem> PmPurchaseOrderItems { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderItemsListVw> PmPurchaseOrderItemsListVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderItemsVw> PmPurchaseOrderItemsVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderStatus> PmPurchaseOrderStatuses { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderSupply> PmPurchaseOrderSupplies { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderSupplyVw> PmPurchaseOrderSupplyVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderTypeIdVw> PmPurchaseOrderTypeIdVws { get; set; } = null!;
        public virtual DbSet<PmPurchaseOrderVw> PmPurchaseOrderVws { get; set; } = null!;
        public virtual DbSet<PmPurchasingPlan> PmPurchasingPlans { get; set; } = null!;
        public virtual DbSet<PmRegulationsSystem> PmRegulationsSystems { get; set; } = null!;
        public virtual DbSet<PmRegulationsSystemsType> PmRegulationsSystemsTypes { get; set; } = null!;
        public virtual DbSet<PmRegulationsSystemsVw> PmRegulationsSystemsVws { get; set; } = null!;
        public virtual DbSet<PmReport> PmReports { get; set; } = null!;
        public virtual DbSet<PmReportsVw> PmReportsVws { get; set; } = null!;
        public virtual DbSet<PmResourcesAssignment> PmResourcesAssignments { get; set; } = null!;
        public virtual DbSet<PmResourcesAssignmentsVw> PmResourcesAssignmentsVws { get; set; } = null!;
        public virtual DbSet<PmResumption> PmResumptions { get; set; } = null!;
        public virtual DbSet<PmResumptionVw> PmResumptionVws { get; set; } = null!;
        public virtual DbSet<PmRiskEffect> PmRiskEffects { get; set; } = null!;
        public virtual DbSet<PmRiskImpact> PmRiskImpacts { get; set; } = null!;
        public virtual DbSet<PmSecuritySituation> PmSecuritySituations { get; set; } = null!;
        public virtual DbSet<PmSecuritySituationReply> PmSecuritySituationReplies { get; set; } = null!;
        public virtual DbSet<PmSecuritySituationVw> PmSecuritySituationVws { get; set; } = null!;
        public virtual DbSet<PmSecurityStatusVw> PmSecurityStatusVws { get; set; } = null!;
        public virtual DbSet<PmSession> PmSessions { get; set; } = null!;
        public virtual DbSet<PmSessionDay> PmSessionDays { get; set; } = null!;
        public virtual DbSet<PmSessionVw> PmSessionVws { get; set; } = null!;
        public virtual DbSet<PmSetting> PmSettings { get; set; } = null!;
        public virtual DbSet<PmTimeSheetTarget> PmTimeSheetTargets { get; set; } = null!;
        public virtual DbSet<PmTimeSheetTargetVw> PmTimeSheetTargetVws { get; set; } = null!;
        public virtual DbSet<PmTimesheetDetaile> PmTimesheetDetailes { get; set; } = null!;
        public virtual DbSet<PmTimesheetDetailes2Vw> PmTimesheetDetailes2Vws { get; set; } = null!;
        public virtual DbSet<PmTimesheetDetailesVw> PmTimesheetDetailesVws { get; set; } = null!;
        public virtual DbSet<PmTimesheetMaster> PmTimesheetMasters { get; set; } = null!;
        public virtual DbSet<PmTimesheetMasterVw> PmTimesheetMasterVws { get; set; } = null!;
        public virtual DbSet<PmTransaction> PmTransactions { get; set; } = null!;
        public virtual DbSet<PmTransactionTimeSheet> PmTransactionTimeSheets { get; set; } = null!;
        public virtual DbSet<PmTransactionTimeSheetVw> PmTransactionTimeSheetVws { get; set; } = null!;
        public virtual DbSet<PmTransactionsD> PmTransactionsDs { get; set; } = null!;
        public virtual DbSet<PmTransactionsDVw> PmTransactionsDVws { get; set; } = null!;
        public virtual DbSet<PmTransactionsInstallment> PmTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<PmTransactionsInstallmentsVw> PmTransactionsInstallmentsVws { get; set; } = null!;
        public virtual DbSet<PmTransactionsPoVw> PmTransactionsPoVws { get; set; } = null!;
        public virtual DbSet<PmTransactionsStatus> PmTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<PmTransactionsStatusVw> PmTransactionsStatusVws { get; set; } = null!;
        public virtual DbSet<PmTransactionsVw> PmTransactionsVws { get; set; } = null!;
        public virtual DbSet<PmTypeAddVw> PmTypeAddVws { get; set; } = null!;
        public virtual DbSet<PmTypeDeducVw> PmTypeDeducVws { get; set; } = null!;
        public virtual DbSet<PmVerdict> PmVerdicts { get; set; } = null!;
        public virtual DbSet<PmVerdictFinancialCostsVw> PmVerdictFinancialCostsVws { get; set; } = null!;
        public virtual DbSet<PmVerdictTypeVw> PmVerdictTypeVws { get; set; } = null!;
        public virtual DbSet<PmVerdictVw> PmVerdictVws { get; set; } = null!;
        public virtual DbSet<PmViolationType> PmViolationTypes { get; set; } = null!;
        public virtual DbSet<PmViolationTypeVw> PmViolationTypeVws { get; set; } = null!;
        public virtual DbSet<PmWorkType> PmWorkTypes { get; set; } = null!;
        public virtual DbSet<PmWorkTypeVw> PmWorkTypeVws { get; set; } = null!;
        public virtual DbSet<PrtlVisitorCounter> PrtlVisitorCounters { get; set; } = null!;
        public virtual DbSet<PurCommittee> PurCommittees { get; set; } = null!;
        public virtual DbSet<PurCommitteeDecision> PurCommitteeDecisions { get; set; } = null!;
        public virtual DbSet<PurCommitteeDecisionsVw> PurCommitteeDecisionsVws { get; set; } = null!;
        public virtual DbSet<PurCommitteeMember> PurCommitteeMembers { get; set; } = null!;
        public virtual DbSet<PurCommitteeMembersType> PurCommitteeMembersTypes { get; set; } = null!;
        public virtual DbSet<PurCommitteeMembersVw> PurCommitteeMembersVws { get; set; } = null!;
        public virtual DbSet<PurCommitteesVw> PurCommitteesVws { get; set; } = null!;
        public virtual DbSet<PurComparison> PurComparisons { get; set; } = null!;
        public virtual DbSet<PurComparisonQuotation> PurComparisonQuotations { get; set; } = null!;
        public virtual DbSet<PurComparisonQuotationVw> PurComparisonQuotationVws { get; set; } = null!;
        public virtual DbSet<PurComparisonSupplier> PurComparisonSuppliers { get; set; } = null!;
        public virtual DbSet<PurComparisonSuppliersVw> PurComparisonSuppliersVws { get; set; } = null!;
        public virtual DbSet<PurComparisonType> PurComparisonTypes { get; set; } = null!;
        public virtual DbSet<PurComparisonVw> PurComparisonVws { get; set; } = null!;
        public virtual DbSet<PurDiscountByAmount> PurDiscountByAmounts { get; set; } = null!;
        public virtual DbSet<PurDiscountByQty> PurDiscountByQties { get; set; } = null!;
        public virtual DbSet<PurDiscountCatalog> PurDiscountCatalogs { get; set; } = null!;
        public virtual DbSet<PurDiscountCatalogAllVw> PurDiscountCatalogAllVws { get; set; } = null!;
        public virtual DbSet<PurDiscountCatalogVw> PurDiscountCatalogVws { get; set; } = null!;
        public virtual DbSet<PurDiscountProduct> PurDiscountProducts { get; set; } = null!;
        public virtual DbSet<PurDiscountProductsVw> PurDiscountProductsVws { get; set; } = null!;
        public virtual DbSet<PurDiscountType> PurDiscountTypes { get; set; } = null!;
        public virtual DbSet<PurExpense> PurExpenses { get; set; } = null!;
        public virtual DbSet<PurExpensesList> PurExpensesLists { get; set; } = null!;
        public virtual DbSet<PurExpensesVw> PurExpensesVws { get; set; } = null!;
        public virtual DbSet<PurOfficialPaper> PurOfficialPapers { get; set; } = null!;
        public virtual DbSet<PurOfficialPapersVw> PurOfficialPapersVws { get; set; } = null!;
        public virtual DbSet<PurPaymentTerm> PurPaymentTerms { get; set; } = null!;
        public virtual DbSet<PurQuotationConformity> PurQuotationConformities { get; set; } = null!;
        public virtual DbSet<PurQuotationConformityVw> PurQuotationConformityVws { get; set; } = null!;
        public virtual DbSet<PurQuotationDocumentType> PurQuotationDocumentTypes { get; set; } = null!;
        public virtual DbSet<PurQuotationDocumentTypeCategory> PurQuotationDocumentTypeCategories { get; set; } = null!;
        public virtual DbSet<PurRfqEvaluationBossVw> PurRfqEvaluationBossVws { get; set; } = null!;
        public virtual DbSet<PurRfqEvaluationMember> PurRfqEvaluationMembers { get; set; } = null!;
        public virtual DbSet<PurRfqStatus> PurRfqStatuses { get; set; } = null!;
        public virtual DbSet<PurRfqStatusVw> PurRfqStatusVws { get; set; } = null!;
        public virtual DbSet<PurRfqTrackingStatus> PurRfqTrackingStatuses { get; set; } = null!;
        public virtual DbSet<PurRfqTrackingStatusVw> PurRfqTrackingStatusVws { get; set; } = null!;
        public virtual DbSet<PurSubmitInvoicesItem> PurSubmitInvoicesItems { get; set; } = null!;
        public virtual DbSet<PurSubmitInvoicesItemsVw> PurSubmitInvoicesItemsVws { get; set; } = null!;
        public virtual DbSet<PurSubmitInvoicesPayment> PurSubmitInvoicesPayments { get; set; } = null!;
        public virtual DbSet<PurSubmitInvoicesType> PurSubmitInvoicesTypes { get; set; } = null!;
        public virtual DbSet<PurSupplierActivity> PurSupplierActivities { get; set; } = null!;
        public virtual DbSet<PurTransaction> PurTransactions { get; set; } = null!;
        public virtual DbSet<PurTransactionsDiscount> PurTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<PurTransactionsDiscountVw> PurTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsExpense> PurTransactionsExpenses { get; set; } = null!;
        public virtual DbSet<PurTransactionsExpensesVw> PurTransactionsExpensesVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsInstallment> PurTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<PurTransactionsNote> PurTransactionsNotes { get; set; } = null!;
        public virtual DbSet<PurTransactionsNoteVw> PurTransactionsNoteVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsPayment> PurTransactionsPayments { get; set; } = null!;
        public virtual DbSet<PurTransactionsPaymentVw> PurTransactionsPaymentVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsProduct> PurTransactionsProducts { get; set; } = null!;
        public virtual DbSet<PurTransactionsProductsVw> PurTransactionsProductsVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsStatus> PurTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<PurTransactionsSubmitInvoice> PurTransactionsSubmitInvoices { get; set; } = null!;
        public virtual DbSet<PurTransactionsSubmitInvoicesVw> PurTransactionsSubmitInvoicesVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsSupplier> PurTransactionsSuppliers { get; set; } = null!;
        public virtual DbSet<PurTransactionsSupplierVw> PurTransactionsSupplierVws { get; set; } = null!;
        public virtual DbSet<PurTransactionsType> PurTransactionsTypes { get; set; } = null!;
        public virtual DbSet<PurTransactionsVw> PurTransactionsVws { get; set; } = null!;
        public virtual DbSet<ReAccruedRevenue> ReAccruedRevenues { get; set; } = null!;
        public virtual DbSet<ReAccruedRevenueContract> ReAccruedRevenueContracts { get; set; } = null!;
        public virtual DbSet<ReAccruedRevenueContractsVw> ReAccruedRevenueContractsVws { get; set; } = null!;
        public virtual DbSet<ReAccruedRevenueRealestate> ReAccruedRevenueRealestates { get; set; } = null!;
        public virtual DbSet<ReAccruedRevenueVw> ReAccruedRevenueVws { get; set; } = null!;
        public virtual DbSet<ReBalanceSheetVw> ReBalanceSheetVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallment> ReContractInstallments { get; set; } = null!;
        public virtual DbSet<ReContractInstallment2Vw> ReContractInstallment2Vws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentFollowUp> ReContractInstallmentFollowUps { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentFollowUpVw> ReContractInstallmentFollowUpVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentNotInvoiceVw> ReContractInstallmentNotInvoiceVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentNotPaidVw> ReContractInstallmentNotPaidVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentPayment> ReContractInstallmentPayments { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentPaymentVw> ReContractInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentServicesVw> ReContractInstallmentServicesVws { get; set; } = null!;
        public virtual DbSet<ReContractInstallmentVw> ReContractInstallmentVws { get; set; } = null!;
        public virtual DbSet<ReContractInsurance> ReContractInsurances { get; set; } = null!;
        public virtual DbSet<ReContractInsuranceVw> ReContractInsuranceVws { get; set; } = null!;
        public virtual DbSet<ReContractLease> ReContractLeases { get; set; } = null!;
        public virtual DbSet<ReContractLease2Vw> ReContractLease2Vws { get; set; } = null!;
        public virtual DbSet<ReContractLeaseRealestate> ReContractLeaseRealestates { get; set; } = null!;
        public virtual DbSet<ReContractLeaseRealestate2Vw> ReContractLeaseRealestate2Vws { get; set; } = null!;
        public virtual DbSet<ReContractLeaseRealestateVw> ReContractLeaseRealestateVws { get; set; } = null!;
        public virtual DbSet<ReContractLeaseRenew> ReContractLeaseRenews { get; set; } = null!;
        public virtual DbSet<ReContractLeaseService> ReContractLeaseServices { get; set; } = null!;
        public virtual DbSet<ReContractLeaseServicesVw> ReContractLeaseServicesVws { get; set; } = null!;
        public virtual DbSet<ReContractLeaseVw> ReContractLeaseVws { get; set; } = null!;
        public virtual DbSet<ReContractLeaseYear> ReContractLeaseYears { get; set; } = null!;
        public virtual DbSet<ReContractLeaving> ReContractLeavings { get; set; } = null!;
        public virtual DbSet<ReContractLeavingVw> ReContractLeavingVws { get; set; } = null!;
        public virtual DbSet<ReContractProperty> ReContractProperties { get; set; } = null!;
        public virtual DbSet<ReContractSale> ReContractSales { get; set; } = null!;
        public virtual DbSet<ReContractSaleRealestate> ReContractSaleRealestates { get; set; } = null!;
        public virtual DbSet<ReContractStatusVw> ReContractStatusVws { get; set; } = null!;
        public virtual DbSet<ReContractType> ReContractTypes { get; set; } = null!;
        public virtual DbSet<ReDynamicAttribute> ReDynamicAttributes { get; set; } = null!;
        public virtual DbSet<ReDynamicAttributesVw> ReDynamicAttributesVws { get; set; } = null!;
        public virtual DbSet<ReDynamicValue> ReDynamicValues { get; set; } = null!;
        public virtual DbSet<ReExpensesReceipt> ReExpensesReceipts { get; set; } = null!;
        public virtual DbSet<ReExpensesReceiptsPaymentInstallment> ReExpensesReceiptsPaymentInstallments { get; set; } = null!;
        public virtual DbSet<ReNotification> ReNotifications { get; set; } = null!;
        public virtual DbSet<ReNotificationVw> ReNotificationVws { get; set; } = null!;
        public virtual DbSet<ReNotificationsType> ReNotificationsTypes { get; set; } = null!;
        public virtual DbSet<ReRealEstate> ReRealEstates { get; set; } = null!;
        public virtual DbSet<ReRealEstateAsset> ReRealEstateAssets { get; set; } = null!;
        public virtual DbSet<ReRealEstateAssetsVw> ReRealEstateAssetsVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenance> ReRealEstateMaintenances { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenanceItem> ReRealEstateMaintenanceItems { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenanceItemsVw> ReRealEstateMaintenanceItemsVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenancePart> ReRealEstateMaintenanceParts { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenancePartVw> ReRealEstateMaintenancePartVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenanceStaff> ReRealEstateMaintenanceStaffs { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenanceStaffVw> ReRealEstateMaintenanceStaffVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateMaintenanceVw> ReRealEstateMaintenanceVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateOwnerRatio> ReRealEstateOwnerRatios { get; set; } = null!;
        public virtual DbSet<ReRealEstateOwnerRatioVw> ReRealEstateOwnerRatioVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateParentVw> ReRealEstateParentVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateStatus> ReRealEstateStatuses { get; set; } = null!;
        public virtual DbSet<ReRealEstateType> ReRealEstateTypes { get; set; } = null!;
        public virtual DbSet<ReRealEstateTypeVw> ReRealEstateTypeVws { get; set; } = null!;
        public virtual DbSet<ReRealEstateVw> ReRealEstateVws { get; set; } = null!;
        public virtual DbSet<ReRealestatOwnersVw> ReRealestatOwnersVws { get; set; } = null!;
        public virtual DbSet<ReReceipt> ReReceipts { get; set; } = null!;
        public virtual DbSet<ReReceiptsVw> ReReceiptsVws { get; set; } = null!;
        public virtual DbSet<ReServicesType> ReServicesTypes { get; set; } = null!;
        public virtual DbSet<ReServicesTypesVw> ReServicesTypesVws { get; set; } = null!;
        public virtual DbSet<ReTransaction> ReTransactions { get; set; } = null!;
        public virtual DbSet<ReTransactionsDiscount> ReTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<ReTransactionsDiscountVw> ReTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<ReTransactionsInstallment> ReTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<ReTransactionsInstallmentsVw> ReTransactionsInstallmentsVws { get; set; } = null!;
        public virtual DbSet<ReTransactionsRealestate> ReTransactionsRealestates { get; set; } = null!;
        public virtual DbSet<ReTransactionsRealestateVw> ReTransactionsRealestateVws { get; set; } = null!;
        public virtual DbSet<ReTransactionsService> ReTransactionsServices { get; set; } = null!;
        public virtual DbSet<ReTransactionsServicesVw> ReTransactionsServicesVws { get; set; } = null!;
        public virtual DbSet<ReTransactionsType> ReTransactionsTypes { get; set; } = null!;
        public virtual DbSet<ReTransactionsVw> ReTransactionsVws { get; set; } = null!;
        public virtual DbSet<RealEstateReceiptD> RealEstateReceiptDs { get; set; } = null!;
        public virtual DbSet<RealEstateReceiptDVw> RealEstateReceiptDVws { get; set; } = null!;
        public virtual DbSet<RealEstateReceiptM> RealEstateReceiptMs { get; set; } = null!;
        public virtual DbSet<RealEstateReceiptMVw> RealEstateReceiptMVws { get; set; } = null!;
        public virtual DbSet<RevlBank> RevlBanks { get; set; } = null!;
        public virtual DbSet<RevlCitiesNeighborhood> RevlCitiesNeighborhoods { get; set; } = null!;
        public virtual DbSet<RevlCitiesNeighborhoodsVw> RevlCitiesNeighborhoodsVws { get; set; } = null!;
        public virtual DbSet<RevlCitiesVw> RevlCitiesVws { get; set; } = null!;
        public virtual DbSet<RevlEmpDataEntryVw> RevlEmpDataEntryVws { get; set; } = null!;
        public virtual DbSet<RevlEmpValuerVw> RevlEmpValuerVws { get; set; } = null!;
        public virtual DbSet<RevlEvaluationManagerVw> RevlEvaluationManagerVws { get; set; } = null!;
        public virtual DbSet<RevlEvaluationSupervisorVw> RevlEvaluationSupervisorVws { get; set; } = null!;
        public virtual DbSet<RevlGeometric> RevlGeometrics { get; set; } = null!;
        public virtual DbSet<RevlPropertiesOld> RevlPropertiesOlds { get; set; } = null!;
        public virtual DbSet<RevlPropertiesRent> RevlPropertiesRents { get; set; } = null!;
        public virtual DbSet<RevlProperty> RevlProperties { get; set; } = null!;
        public virtual DbSet<RevlRealEstate> RevlRealEstates { get; set; } = null!;
        public virtual DbSet<RevlRealEstateArea> RevlRealEstateAreas { get; set; } = null!;
        public virtual DbSet<RevlRealEstateComparison> RevlRealEstateComparisons { get; set; } = null!;
        public virtual DbSet<RevlRealEstateComparisonsVw> RevlRealEstateComparisonsVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateContract> RevlRealEstateContracts { get; set; } = null!;
        public virtual DbSet<RevlRealEstateContractVw> RevlRealEstateContractVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateFile> RevlRealEstateFiles { get; set; } = null!;
        public virtual DbSet<RevlRealEstateFilesVw> RevlRealEstateFilesVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateGeometric> RevlRealEstateGeometrics { get; set; } = null!;
        public virtual DbSet<RevlRealEstateImag> RevlRealEstateImags { get; set; } = null!;
        public virtual DbSet<RevlRealEstateImagsVw> RevlRealEstateImagsVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateRasmala> RevlRealEstateRasmalas { get; set; } = null!;
        public virtual DbSet<RevlRealEstateRasmalaVw> RevlRealEstateRasmalaVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateRent> RevlRealEstateRents { get; set; } = null!;
        public virtual DbSet<RevlRealEstateStatus> RevlRealEstateStatuses { get; set; } = null!;
        public virtual DbSet<RevlRealEstateStatusVw> RevlRealEstateStatusVws { get; set; } = null!;
        public virtual DbSet<RevlRealEstateType> RevlRealEstateTypes { get; set; } = null!;
        public virtual DbSet<RevlRealEstateVw> RevlRealEstateVws { get; set; } = null!;
        public virtual DbSet<RevlSetting> RevlSettings { get; set; } = null!;
        public virtual DbSet<RevlStatus> RevlStatuses { get; set; } = null!;
        public virtual DbSet<RevlStep> RevlSteps { get; set; } = null!;
        public virtual DbSet<RevlValuationRatio> RevlValuationRatios { get; set; } = null!;
        public virtual DbSet<RevlZoneVw> RevlZoneVws { get; set; } = null!;
        public virtual DbSet<RptCustomReport> RptCustomReports { get; set; } = null!;
        public virtual DbSet<RptField> RptFields { get; set; } = null!;
        public virtual DbSet<RptFieldsVw> RptFieldsVws { get; set; } = null!;
        public virtual DbSet<RptOperator> RptOperators { get; set; } = null!;
        public virtual DbSet<RptReport> RptReports { get; set; } = null!;
        public virtual DbSet<RptReportsField> RptReportsFields { get; set; } = null!;
        public virtual DbSet<RptReportsFieldsVw> RptReportsFieldsVws { get; set; } = null!;
        public virtual DbSet<RptReportsGroupBy> RptReportsGroupBies { get; set; } = null!;
        public virtual DbSet<RptReportsGroupByVw> RptReportsGroupByVws { get; set; } = null!;
        public virtual DbSet<RptReportsOrderBy> RptReportsOrderBies { get; set; } = null!;
        public virtual DbSet<RptReportsOrderByVw> RptReportsOrderByVws { get; set; } = null!;
        public virtual DbSet<RptReportsVw> RptReportsVws { get; set; } = null!;
        public virtual DbSet<RptTable> RptTables { get; set; } = null!;
        public virtual DbSet<SalAdditionalType> SalAdditionalTypes { get; set; } = null!;
        public virtual DbSet<SalAdditionalTypeVw> SalAdditionalTypeVws { get; set; } = null!;
        public virtual DbSet<SalAgeCredit> SalAgeCredits { get; set; } = null!;
        public virtual DbSet<SalAgeCreditVw> SalAgeCreditVws { get; set; } = null!;
        public virtual DbSet<SalCommission> SalCommissions { get; set; } = null!;
        public virtual DbSet<SalCommissionBranch> SalCommissionBranches { get; set; } = null!;
        public virtual DbSet<SalCommissionBranchVw> SalCommissionBranchVws { get; set; } = null!;
        public virtual DbSet<SalCommissionCalculation> SalCommissionCalculations { get; set; } = null!;
        public virtual DbSet<SalCommissionDetaile> SalCommissionDetailes { get; set; } = null!;
        public virtual DbSet<SalCommissionDetailesVw> SalCommissionDetailesVws { get; set; } = null!;
        public virtual DbSet<SalCommissionPay> SalCommissionPays { get; set; } = null!;
        public virtual DbSet<SalCommissionVw> SalCommissionVws { get; set; } = null!;
        public virtual DbSet<SalCustomerPoint> SalCustomerPoints { get; set; } = null!;
        public virtual DbSet<SalCustomerPointsRefrance> SalCustomerPointsRefrances { get; set; } = null!;
        public virtual DbSet<SalCustomerPointsVw> SalCustomerPointsVws { get; set; } = null!;
        public virtual DbSet<SalDiscountByAmount> SalDiscountByAmounts { get; set; } = null!;
        public virtual DbSet<SalDiscountByQty> SalDiscountByQties { get; set; } = null!;
        public virtual DbSet<SalDiscountCatalog> SalDiscountCatalogs { get; set; } = null!;
        public virtual DbSet<SalDiscountCatalogAllVw> SalDiscountCatalogAllVws { get; set; } = null!;
        public virtual DbSet<SalDiscountCatalogDetailVw> SalDiscountCatalogDetailVws { get; set; } = null!;
        public virtual DbSet<SalDiscountCatalogVw> SalDiscountCatalogVws { get; set; } = null!;
        public virtual DbSet<SalDiscountProduct> SalDiscountProducts { get; set; } = null!;
        public virtual DbSet<SalDiscountProductsVw> SalDiscountProductsVws { get; set; } = null!;
        public virtual DbSet<SalDiscountType> SalDiscountTypes { get; set; } = null!;
        public virtual DbSet<SalInvoiceDetail> SalInvoiceDetails { get; set; } = null!;
        public virtual DbSet<SalInvoiceDetailsVw> SalInvoiceDetailsVws { get; set; } = null!;
        public virtual DbSet<SalItemsPriceD> SalItemsPriceDs { get; set; } = null!;
        public virtual DbSet<SalItemsPriceDVw> SalItemsPriceDVws { get; set; } = null!;
        public virtual DbSet<SalItemsPriceList> SalItemsPriceLists { get; set; } = null!;
        public virtual DbSet<SalItemsPriceM> SalItemsPriceMs { get; set; } = null!;
        public virtual DbSet<SalItemsPriceMVw> SalItemsPriceMVws { get; set; } = null!;
        public virtual DbSet<SalListItemPriceVw> SalListItemPriceVws { get; set; } = null!;
        public virtual DbSet<SalOrderMake> SalOrderMakes { get; set; } = null!;
        public virtual DbSet<SalOrderMakeVw> SalOrderMakeVws { get; set; } = null!;
        public virtual DbSet<SalPaymentTerm> SalPaymentTerms { get; set; } = null!;
        public virtual DbSet<SalPaymentTermsVw> SalPaymentTermsVws { get; set; } = null!;
        public virtual DbSet<SalPointByAmount> SalPointByAmounts { get; set; } = null!;
        public virtual DbSet<SalPointByAmountVw> SalPointByAmountVws { get; set; } = null!;
        public virtual DbSet<SalPointCatalog> SalPointCatalogs { get; set; } = null!;
        public virtual DbSet<SalPointType> SalPointTypes { get; set; } = null!;
        public virtual DbSet<SalPosCloseCash> SalPosCloseCashes { get; set; } = null!;
        public virtual DbSet<SalPosSetting> SalPosSettings { get; set; } = null!;
        public virtual DbSet<SalPosSettingVw> SalPosSettingVws { get; set; } = null!;
        public virtual DbSet<SalPosUser> SalPosUsers { get; set; } = null!;
        public virtual DbSet<SalPosUsersVw> SalPosUsersVws { get; set; } = null!;
        public virtual DbSet<SalProductsSearching> SalProductsSearchings { get; set; } = null!;
        public virtual DbSet<SalProductsSearchingVw> SalProductsSearchingVws { get; set; } = null!;
        public virtual DbSet<SalReceiptingInvoice> SalReceiptingInvoices { get; set; } = null!;
        public virtual DbSet<SalReceiptingInvoicesSection> SalReceiptingInvoicesSections { get; set; } = null!;
        public virtual DbSet<SalReceiptingInvoicesSectionsVw> SalReceiptingInvoicesSectionsVws { get; set; } = null!;
        public virtual DbSet<SalReceiptingInvoicesVw> SalReceiptingInvoicesVws { get; set; } = null!;
        public virtual DbSet<SalScaleSetting> SalScaleSettings { get; set; } = null!;
        public virtual DbSet<SalScaleSettingVw> SalScaleSettingVws { get; set; } = null!;
        public virtual DbSet<SalSetting> SalSettings { get; set; } = null!;
        public virtual DbSet<SalTailorDesign> SalTailorDesigns { get; set; } = null!;
        public virtual DbSet<SalTailorDesignCatagory> SalTailorDesignCatagories { get; set; } = null!;
        public virtual DbSet<SalTailorDesignVw> SalTailorDesignVws { get; set; } = null!;
        public virtual DbSet<SalTransaction> SalTransactions { get; set; } = null!;
        public virtual DbSet<SalTransactionCommisionVw> SalTransactionCommisionVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsAdditional> SalTransactionsAdditionals { get; set; } = null!;
        public virtual DbSet<SalTransactionsAdditionalVw> SalTransactionsAdditionalVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsBranchReportVw> SalTransactionsBranchReportVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsCommission> SalTransactionsCommissions { get; set; } = null!;
        public virtual DbSet<SalTransactionsCommissionVw> SalTransactionsCommissionVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsDiscount> SalTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<SalTransactionsDiscountVw> SalTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsInstallment> SalTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<SalTransactionsNote> SalTransactionsNotes { get; set; } = null!;
        public virtual DbSet<SalTransactionsNoteVw> SalTransactionsNoteVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsPayment> SalTransactionsPayments { get; set; } = null!;
        public virtual DbSet<SalTransactionsPaymentVw> SalTransactionsPaymentVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProduct> SalTransactionsProducts { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductTypeVw> SalTransactionsProductTypeVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProducts6Vw> SalTransactionsProducts6Vws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsExportVw> SalTransactionsProductsExportVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsPrintVw> SalTransactionsProductsPrintVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsPropertiy> SalTransactionsProductsPropertiys { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsVw> SalTransactionsProductsVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsVw2> SalTransactionsProductsVw2s { get; set; } = null!;
        public virtual DbSet<SalTransactionsProductsVw3> SalTransactionsProductsVw3s { get; set; } = null!;
        public virtual DbSet<SalTransactionsStatus> SalTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<SalTransactionsTicket> SalTransactionsTickets { get; set; } = null!;
        public virtual DbSet<SalTransactionsTicketVw> SalTransactionsTicketVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsTransPortVw> SalTransactionsTransPortVws { get; set; } = null!;
        public virtual DbSet<SalTransactionsType> SalTransactionsTypes { get; set; } = null!;
        public virtual DbSet<SalTransactionsVw> SalTransactionsVws { get; set; } = null!;
        public virtual DbSet<SchAdmission> SchAdmissions { get; set; } = null!;
        public virtual DbSet<SchAdmissionResponse> SchAdmissionResponses { get; set; } = null!;
        public virtual DbSet<SchAdmissionResponseVw> SchAdmissionResponseVws { get; set; } = null!;
        public virtual DbSet<SchAdmissionVw> SchAdmissionVws { get; set; } = null!;
        public virtual DbSet<SchClassRoom> SchClassRooms { get; set; } = null!;
        public virtual DbSet<SchClassRoomsVw> SchClassRoomsVws { get; set; } = null!;
        public virtual DbSet<SchDiscountCatalog> SchDiscountCatalogs { get; set; } = null!;
        public virtual DbSet<SchDiscountCatalogVw> SchDiscountCatalogVws { get; set; } = null!;
        public virtual DbSet<SchDiscountFee> SchDiscountFees { get; set; } = null!;
        public virtual DbSet<SchDiscountFeesVw> SchDiscountFeesVws { get; set; } = null!;
        public virtual DbSet<SchDiscountType> SchDiscountTypes { get; set; } = null!;
        public virtual DbSet<SchGrade> SchGrades { get; set; } = null!;
        public virtual DbSet<SchGradesVw> SchGradesVws { get; set; } = null!;
        public virtual DbSet<SchLevel> SchLevels { get; set; } = null!;
        public virtual DbSet<SchLevelsDiscount> SchLevelsDiscounts { get; set; } = null!;
        public virtual DbSet<SchLevelsDiscountDetail> SchLevelsDiscountDetails { get; set; } = null!;
        public virtual DbSet<SchLevelsDiscountVw> SchLevelsDiscountVws { get; set; } = null!;
        public virtual DbSet<SchMethodDistributionFee> SchMethodDistributionFees { get; set; } = null!;
        public virtual DbSet<SchMethodDistributionFeesDetailed> SchMethodDistributionFeesDetaileds { get; set; } = null!;
        public virtual DbSet<SchQuittance> SchQuittances { get; set; } = null!;
        public virtual DbSet<SchQuittanceVw> SchQuittanceVws { get; set; } = null!;
        public virtual DbSet<SchRegistration> SchRegistrations { get; set; } = null!;
        public virtual DbSet<SchRegistrationInstallment> SchRegistrationInstallments { get; set; } = null!;
        public virtual DbSet<SchRegistrationInstallmentPayment> SchRegistrationInstallmentPayments { get; set; } = null!;
        public virtual DbSet<SchRegistrationInstallmentPaymentVw> SchRegistrationInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<SchRegistrationInstallmentVw> SchRegistrationInstallmentVws { get; set; } = null!;
        public virtual DbSet<SchRegistrationVw> SchRegistrationVws { get; set; } = null!;
        public virtual DbSet<SchRoute> SchRoutes { get; set; } = null!;
        public virtual DbSet<SchRouteDetailed> SchRouteDetaileds { get; set; } = null!;
        public virtual DbSet<SchRouteDetailedVw> SchRouteDetailedVws { get; set; } = null!;
        public virtual DbSet<SchRouteLink> SchRouteLinks { get; set; } = null!;
        public virtual DbSet<SchRouteLinkVw> SchRouteLinkVws { get; set; } = null!;
        public virtual DbSet<SchRouteVw> SchRouteVws { get; set; } = null!;
        public virtual DbSet<SchSchool> SchSchools { get; set; } = null!;
        public virtual DbSet<SchSchoolLevel> SchSchoolLevels { get; set; } = null!;
        public virtual DbSet<SchSchoolLevelsVw> SchSchoolLevelsVws { get; set; } = null!;
        public virtual DbSet<SchSchoolTerm> SchSchoolTerms { get; set; } = null!;
        public virtual DbSet<SchSchoolTermsVw> SchSchoolTermsVws { get; set; } = null!;
        public virtual DbSet<SchSchoolYear> SchSchoolYears { get; set; } = null!;
        public virtual DbSet<SchSchoolYearsVw> SchSchoolYearsVws { get; set; } = null!;
        public virtual DbSet<SchSchoolsVw> SchSchoolsVws { get; set; } = null!;
        public virtual DbSet<SchStudentClass> SchStudentClasses { get; set; } = null!;
        public virtual DbSet<SchStudentClassesVw> SchStudentClassesVws { get; set; } = null!;
        public virtual DbSet<SchStudentStatusVw> SchStudentStatusVws { get; set; } = null!;
        public virtual DbSet<SchStudentTransfer> SchStudentTransfers { get; set; } = null!;
        public virtual DbSet<SchStudentVw> SchStudentVws { get; set; } = null!;
        public virtual DbSet<SchStudyFee> SchStudyFees { get; set; } = null!;
        public virtual DbSet<SchStudyFeesGroup> SchStudyFeesGroups { get; set; } = null!;
        public virtual DbSet<SchStudyFeesGroupVw> SchStudyFeesGroupVws { get; set; } = null!;
        public virtual DbSet<SchStudyFeesVw> SchStudyFeesVws { get; set; } = null!;
        public virtual DbSet<SchTeachersVw> SchTeachersVws { get; set; } = null!;
        public virtual DbSet<SchTransaction> SchTransactions { get; set; } = null!;
        public virtual DbSet<SchTransactionsDiscount> SchTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<SchTransactionsDiscountVw> SchTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsInstallment> SchTransactionsInstallments { get; set; } = null!;
        public virtual DbSet<SchTransactionsInstallmentsVw> SchTransactionsInstallmentsVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsPayment> SchTransactionsPayments { get; set; } = null!;
        public virtual DbSet<SchTransactionsPaymentVw> SchTransactionsPaymentVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsPrintVw> SchTransactionsPrintVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsTransportationInstallmentsVw> SchTransactionsTransportationInstallmentsVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsTransportationPrintVw> SchTransactionsTransportationPrintVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsTransportationVw> SchTransactionsTransportationVws { get; set; } = null!;
        public virtual DbSet<SchTransactionsVw> SchTransactionsVws { get; set; } = null!;
        public virtual DbSet<SchTransportation> SchTransportations { get; set; } = null!;
        public virtual DbSet<SchTransportationInstallment> SchTransportationInstallments { get; set; } = null!;
        public virtual DbSet<SchTransportationInstallmentPayment> SchTransportationInstallmentPayments { get; set; } = null!;
        public virtual DbSet<SchTransportationInstallmentPaymentVw> SchTransportationInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<SchTransportationInstallmentVw> SchTransportationInstallmentVws { get; set; } = null!;
        public virtual DbSet<SchTransportationVw> SchTransportationVws { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<SubCategoriesVw> SubCategoriesVws { get; set; } = null!;
        public virtual DbSet<SubDiscountCatalog> SubDiscountCatalogs { get; set; } = null!;
        public virtual DbSet<SubDiscountCatalogVw> SubDiscountCatalogVws { get; set; } = null!;
        public virtual DbSet<SubDiscountPackage> SubDiscountPackages { get; set; } = null!;
        public virtual DbSet<SubDiscountPackageVw> SubDiscountPackageVws { get; set; } = null!;
        public virtual DbSet<SubDurationsVw> SubDurationsVws { get; set; } = null!;
        public virtual DbSet<SubLoginMember> SubLoginMembers { get; set; } = null!;
        public virtual DbSet<SubLoginMemberVw> SubLoginMemberVws { get; set; } = null!;
        public virtual DbSet<SubMembersVw> SubMembersVws { get; set; } = null!;
        public virtual DbSet<SubPackage> SubPackages { get; set; } = null!;
        public virtual DbSet<SubPackageVw> SubPackageVws { get; set; } = null!;
        public virtual DbSet<SubStatusVw> SubStatusVws { get; set; } = null!;
        public virtual DbSet<SubSubscription> SubSubscriptions { get; set; } = null!;
        public virtual DbSet<SubSubscriptionPayment> SubSubscriptionPayments { get; set; } = null!;
        public virtual DbSet<SubSubscriptionPaymentVw> SubSubscriptionPaymentVws { get; set; } = null!;
        public virtual DbSet<SubSubscriptionsReactive> SubSubscriptionsReactives { get; set; } = null!;
        public virtual DbSet<SubSubscriptionsSuspend> SubSubscriptionsSuspends { get; set; } = null!;
        public virtual DbSet<SubSubscriptionsSuspendVw> SubSubscriptionsSuspendVws { get; set; } = null!;
        public virtual DbSet<SubSubscriptionsType> SubSubscriptionsTypes { get; set; } = null!;
        public virtual DbSet<SubSubscriptionsVw> SubSubscriptionsVws { get; set; } = null!;
        public virtual DbSet<SubTransaction> SubTransactions { get; set; } = null!;
        public virtual DbSet<SysActionTypeVw> SysActionTypeVws { get; set; } = null!;
        public virtual DbSet<SysActivityLog> SysActivityLogs { get; set; } = null!;
        public virtual DbSet<SysActivityLogField> SysActivityLogFields { get; set; } = null!;
        public virtual DbSet<SysActivityLogM2Vw> SysActivityLogM2Vws { get; set; } = null!;
        public virtual DbSet<SysActivityLogMVw> SysActivityLogMVws { get; set; } = null!;
        public virtual DbSet<SysActivityLogVw> SysActivityLogVws { get; set; } = null!;
        public virtual DbSet<SysActivityType> SysActivityTypes { get; set; } = null!;
        public virtual DbSet<SysActivityVw> SysActivityVws { get; set; } = null!;
        public virtual DbSet<SysAgeTypeVw> SysAgeTypeVws { get; set; } = null!;
        public virtual DbSet<SysAnnouncement> SysAnnouncements { get; set; } = null!;
        public virtual DbSet<SysAnnouncementLocationVw> SysAnnouncementLocationVws { get; set; } = null!;
        public virtual DbSet<SysAnnouncementTypeVw> SysAnnouncementTypeVws { get; set; } = null!;
        public virtual DbSet<SysAnnouncementVw> SysAnnouncementVws { get; set; } = null!;
        public virtual DbSet<SysApplicationsType> SysApplicationsTypes { get; set; } = null!;
        public virtual DbSet<SysAppointment> SysAppointments { get; set; } = null!;
        public virtual DbSet<SysAppointmentVw> SysAppointmentVws { get; set; } = null!;
        public virtual DbSet<SysArchiveFile> SysArchiveFiles { get; set; } = null!;
        public virtual DbSet<SysArchiveMessage> SysArchiveMessages { get; set; } = null!;
        public virtual DbSet<SysBanksVw> SysBanksVws { get; set; } = null!;
        public virtual DbSet<SysBranch> SysBranches { get; set; } = null!;
        public virtual DbSet<SysBranchVw> SysBranchVws { get; set; } = null!;
        public virtual DbSet<SysCalendar> SysCalendars { get; set; } = null!;
        public virtual DbSet<SysCite> SysCites { get; set; } = null!;
        public virtual DbSet<SysCitesVw> SysCitesVws { get; set; } = null!;
        public virtual DbSet<SysCityVw> SysCityVws { get; set; } = null!;
        public virtual DbSet<SysCountryVw> SysCountryVws { get; set; } = null!;
        public virtual DbSet<SysCreateUserRequst> SysCreateUserRequsts { get; set; } = null!;
        public virtual DbSet<SysCreateUserRequstVw> SysCreateUserRequstVws { get; set; } = null!;
        public virtual DbSet<SysCurrency> SysCurrencies { get; set; } = null!;
        public virtual DbSet<SysCurrencyListVw> SysCurrencyListVws { get; set; } = null!;
        public virtual DbSet<SysCustomer> SysCustomers { get; set; } = null!;
        public virtual DbSet<SysCustomerAssigin> SysCustomerAssigins { get; set; } = null!;
        public virtual DbSet<SysCustomerBranch> SysCustomerBranches { get; set; } = null!;
        public virtual DbSet<SysCustomerBranchVw> SysCustomerBranchVws { get; set; } = null!;
        public virtual DbSet<SysCustomerCoType> SysCustomerCoTypes { get; set; } = null!;
        public virtual DbSet<SysCustomerCondition> SysCustomerConditions { get; set; } = null!;
        public virtual DbSet<SysCustomerContact> SysCustomerContacts { get; set; } = null!;
        public virtual DbSet<SysCustomerContactVw> SysCustomerContactVws { get; set; } = null!;
        public virtual DbSet<SysCustomerDdlVw> SysCustomerDdlVws { get; set; } = null!;
        public virtual DbSet<SysCustomerDomain> SysCustomerDomains { get; set; } = null!;
        public virtual DbSet<SysCustomerDomainsVw> SysCustomerDomainsVws { get; set; } = null!;
        public virtual DbSet<SysCustomerExperience> SysCustomerExperiences { get; set; } = null!;
        public virtual DbSet<SysCustomerFile> SysCustomerFiles { get; set; } = null!;
        public virtual DbSet<SysCustomerFilesVw> SysCustomerFilesVws { get; set; } = null!;
        public virtual DbSet<SysCustomerGroup> SysCustomerGroups { get; set; } = null!;
        public virtual DbSet<SysCustomerGroupAccount> SysCustomerGroupAccounts { get; set; } = null!;
        public virtual DbSet<SysCustomerGroupAccountsVw> SysCustomerGroupAccountsVws { get; set; } = null!;
        public virtual DbSet<SysCustomerIndustryVw> SysCustomerIndustryVws { get; set; } = null!;
        public virtual DbSet<SysCustomerListVw> SysCustomerListVws { get; set; } = null!;
        public virtual DbSet<SysCustomerNote> SysCustomerNotes { get; set; } = null!;
        public virtual DbSet<SysCustomerNoteVw> SysCustomerNoteVws { get; set; } = null!;
        public virtual DbSet<SysCustomerService> SysCustomerServices { get; set; } = null!;
        public virtual DbSet<SysCustomerServicesVw> SysCustomerServicesVws { get; set; } = null!;
        public virtual DbSet<SysCustomerSize> SysCustomerSizes { get; set; } = null!;
        public virtual DbSet<SysCustomerSizeVw> SysCustomerSizeVws { get; set; } = null!;
        public virtual DbSet<SysCustomerSizeVw1> SysCustomerSizeVw1s { get; set; } = null!;
        public virtual DbSet<SysCustomerSizeVw2> SysCustomerSizeVw2s { get; set; } = null!;
        public virtual DbSet<SysCustomerStatusVw> SysCustomerStatusVws { get; set; } = null!;
        public virtual DbSet<SysCustomerTransfer> SysCustomerTransfers { get; set; } = null!;
        public virtual DbSet<SysCustomerType> SysCustomerTypes { get; set; } = null!;
        public virtual DbSet<SysCustomerVw> SysCustomerVws { get; set; } = null!;
        public virtual DbSet<SysDailyJobSendByEmail> SysDailyJobSendByEmails { get; set; } = null!;
        public virtual DbSet<SysDepartment> SysDepartments { get; set; } = null!;
        public virtual DbSet<SysDepartmentCatagory> SysDepartmentCatagories { get; set; } = null!;
        public virtual DbSet<SysDepartmentVw> SysDepartmentVws { get; set; } = null!;
        public virtual DbSet<SysDocument> SysDocuments { get; set; } = null!;
        public virtual DbSet<SysDocumentValue> SysDocumentValues { get; set; } = null!;
        public virtual DbSet<SysDocumentValuesVw> SysDocumentValuesVws { get; set; } = null!;
        public virtual DbSet<SysDocumentsVw> SysDocumentsVws { get; set; } = null!;
        public virtual DbSet<SysDynamicAttribute> SysDynamicAttributes { get; set; } = null!;
        public virtual DbSet<SysDynamicAttributeDataType> SysDynamicAttributeDataTypes { get; set; } = null!;
        public virtual DbSet<SysDynamicAttributesVw> SysDynamicAttributesVws { get; set; } = null!;
        public virtual DbSet<SysDynamicValue> SysDynamicValues { get; set; } = null!;
        public virtual DbSet<SysEmailsAllVw> SysEmailsAllVws { get; set; } = null!;
        public virtual DbSet<SysExchangeRate> SysExchangeRates { get; set; } = null!;
        public virtual DbSet<SysExchangeRateListVw> SysExchangeRateListVws { get; set; } = null!;
        public virtual DbSet<SysExchangeRateVw> SysExchangeRateVws { get; set; } = null!;
        public virtual DbSet<SysFavMenu> SysFavMenus { get; set; } = null!;
        public virtual DbSet<SysFile> SysFiles { get; set; } = null!;
        public virtual DbSet<SysFilesAllVw> SysFilesAllVws { get; set; } = null!;
        public virtual DbSet<SysFilesDocument> SysFilesDocuments { get; set; } = null!;
        public virtual DbSet<SysFilesDocumentVw> SysFilesDocumentVws { get; set; } = null!;
        public virtual DbSet<SysForm> SysForms { get; set; } = null!;
        public virtual DbSet<SysFormsVw> SysFormsVws { get; set; } = null!;
        public virtual DbSet<SysGroup> SysGroups { get; set; } = null!;
        public virtual DbSet<SysGroupVw> SysGroupVws { get; set; } = null!;
        public virtual DbSet<SysInformation> SysInformations { get; set; } = null!;
        public virtual DbSet<SysLeadStaff> SysLeadStaffs { get; set; } = null!;
        public virtual DbSet<SysLibraryFile> SysLibraryFiles { get; set; } = null!;
        public virtual DbSet<SysLibraryFilesVw> SysLibraryFilesVws { get; set; } = null!;
        public virtual DbSet<SysLicense> SysLicenses { get; set; } = null!;
        public virtual DbSet<SysLicenseFile> SysLicenseFiles { get; set; } = null!;
        public virtual DbSet<SysLicenseFilesVw> SysLicenseFilesVws { get; set; } = null!;
        public virtual DbSet<SysLicensesVw> SysLicensesVws { get; set; } = null!;
        public virtual DbSet<SysLocationVw> SysLocationVws { get; set; } = null!;
        public virtual DbSet<SysLogHistory> SysLogHistories { get; set; } = null!;
        public virtual DbSet<SysLookUpCatagory> SysLookUpCatagories { get; set; } = null!;
        public virtual DbSet<SysLookupColor> SysLookupColors { get; set; } = null!;
        public virtual DbSet<SysLookupDataGenderVw> SysLookupDataGenderVws { get; set; } = null!;
        public virtual DbSet<SysLookupDataVw> SysLookupDataVws { get; set; } = null!;
        public virtual DbSet<SysLookupDatum> SysLookupData { get; set; } = null!;
        public virtual DbSet<SysMailServer> SysMailServers { get; set; } = null!;
        public virtual DbSet<SysMailServerListVw> SysMailServerListVws { get; set; } = null!;
        public virtual DbSet<SysMessage> SysMessages { get; set; } = null!;
        public virtual DbSet<SysMessageVw> SysMessageVws { get; set; } = null!;
        public virtual DbSet<SysMobileMember> SysMobileMembers { get; set; } = null!;
        public virtual DbSet<SysModule> SysModules { get; set; } = null!;
        public virtual DbSet<SysMonthVw> SysMonthVws { get; set; } = null!;
        public virtual DbSet<SysNameAllVw> SysNameAllVws { get; set; } = null!;
        public virtual DbSet<SysNationalityTypeVw> SysNationalityTypeVws { get; set; } = null!;
        public virtual DbSet<SysNationalityVw> SysNationalityVws { get; set; } = null!;
        public virtual DbSet<SysNotification> SysNotifications { get; set; } = null!;
        public virtual DbSet<SysNotificationsMang> SysNotificationsMangs { get; set; } = null!;
        public virtual DbSet<SysNotificationsMangVw> SysNotificationsMangVws { get; set; } = null!;
        public virtual DbSet<SysNotificationsSetting> SysNotificationsSettings { get; set; } = null!;
        public virtual DbSet<SysNotificationsSettingVw> SysNotificationsSettingVws { get; set; } = null!;
        public virtual DbSet<SysNotificationsVw> SysNotificationsVws { get; set; } = null!;
        public virtual DbSet<SysOwnersListVw> SysOwnersListVws { get; set; } = null!;
        public virtual DbSet<SysPoliciesProcedure> SysPoliciesProcedures { get; set; } = null!;
        public virtual DbSet<SysPoliciesProceduresTypeVw> SysPoliciesProceduresTypeVws { get; set; } = null!;
        public virtual DbSet<SysPoliciesProceduresVw> SysPoliciesProceduresVws { get; set; } = null!;
        public virtual DbSet<SysPropertiesVw> SysPropertiesVws { get; set; } = null!;
        public virtual DbSet<SysProperty> SysProperties { get; set; } = null!;
        public virtual DbSet<SysPropertyValue> SysPropertyValues { get; set; } = null!;
        public virtual DbSet<SysPropertyValuesVw> SysPropertyValuesVws { get; set; } = null!;
        public virtual DbSet<SysResetPassword> SysResetPasswords { get; set; } = null!;
        public virtual DbSet<SysScreen> SysScreens { get; set; } = null!;
        public virtual DbSet<SysScreenInstalled> SysScreenInstalleds { get; set; } = null!;
        public virtual DbSet<SysScreenInstalledVw> SysScreenInstalledVws { get; set; } = null!;
        public virtual DbSet<SysScreenPermission> SysScreenPermissions { get; set; } = null!;
        public virtual DbSet<SysScreenPermissionPropertiesVw> SysScreenPermissionPropertiesVws { get; set; } = null!;
        public virtual DbSet<SysScreenPermissionProperty> SysScreenPermissionProperties { get; set; } = null!;
        public virtual DbSet<SysScreenPermissionVw> SysScreenPermissionVws { get; set; } = null!;
        public virtual DbSet<SysScreenProperty> SysScreenProperties { get; set; } = null!;
        public virtual DbSet<SysScreenSystem> SysScreenSystems { get; set; } = null!;
        public virtual DbSet<SysScreenVw> SysScreenVws { get; set; } = null!;
        public virtual DbSet<SysScreenWorkflow> SysScreenWorkflows { get; set; } = null!;
        public virtual DbSet<SysSettingExport> SysSettingExports { get; set; } = null!;
        public virtual DbSet<SysSettingExportVw> SysSettingExportVws { get; set; } = null!;
        public virtual DbSet<SysStrategicPlan> SysStrategicPlans { get; set; } = null!;
        public virtual DbSet<SysStrategicPlanVw> SysStrategicPlanVws { get; set; } = null!;
        public virtual DbSet<SysSupplierVw> SysSupplierVws { get; set; } = null!;
        public virtual DbSet<SysSystem> SysSystems { get; set; } = null!;
        public virtual DbSet<SysTable> SysTables { get; set; } = null!;
        public virtual DbSet<SysTableField> SysTableFields { get; set; } = null!;
        public virtual DbSet<SysTargetEmployee> SysTargetEmployees { get; set; } = null!;
        public virtual DbSet<SysTargetEmployeeVw> SysTargetEmployeeVws { get; set; } = null!;
        public virtual DbSet<SysTargetMaster> SysTargetMasters { get; set; } = null!;
        public virtual DbSet<SysTargetMasterVw> SysTargetMasterVws { get; set; } = null!;
        public virtual DbSet<SysTargetPeriod> SysTargetPeriods { get; set; } = null!;
        public virtual DbSet<SysTargetPeriodVw> SysTargetPeriodVws { get; set; } = null!;
        public virtual DbSet<SysTargetQty> SysTargetQties { get; set; } = null!;
        public virtual DbSet<SysTargetQtyVw> SysTargetQtyVws { get; set; } = null!;
        public virtual DbSet<SysTask> SysTasks { get; set; } = null!;
        public virtual DbSet<SysTaskStatusVw> SysTaskStatusVws { get; set; } = null!;
        public virtual DbSet<SysTasksResponse> SysTasksResponses { get; set; } = null!;
        public virtual DbSet<SysTasksResponseVw> SysTasksResponseVws { get; set; } = null!;
        public virtual DbSet<SysTasksScheduler> SysTasksSchedulers { get; set; } = null!;
        public virtual DbSet<SysTasksSchedulerVw> SysTasksSchedulerVws { get; set; } = null!;
        public virtual DbSet<SysTasksType> SysTasksTypes { get; set; } = null!;
        public virtual DbSet<SysTasksVw> SysTasksVws { get; set; } = null!;
        public virtual DbSet<SysTemplate> SysTemplates { get; set; } = null!;
        public virtual DbSet<SysTemplateVw> SysTemplateVws { get; set; } = null!;
        public virtual DbSet<SysTitleVw> SysTitleVws { get; set; } = null!;
        public virtual DbSet<SysUpdate> SysUpdates { get; set; } = null!;
        public virtual DbSet<SysUpdates2019> SysUpdates2019s { get; set; } = null!;
        public virtual DbSet<SysUpdates2020> SysUpdates2020s { get; set; } = null!;
        public virtual DbSet<SysUpdates2021> SysUpdates2021s { get; set; } = null!;
        public virtual DbSet<SysUpdates2022> SysUpdates2022s { get; set; } = null!;
        public virtual DbSet<SysUpdatesInstalled> SysUpdatesInstalleds { get; set; } = null!;
        public virtual DbSet<SysUpdatesInstalledVw> SysUpdatesInstalledVws { get; set; } = null!;
        public virtual DbSet<SysUser> SysUsers { get; set; } = null!;
        public virtual DbSet<SysUserDevice> SysUserDevices { get; set; } = null!;
        public virtual DbSet<SysUserLogTime> SysUserLogTimes { get; set; } = null!;
        public virtual DbSet<SysUserLogTimeVw> SysUserLogTimeVws { get; set; } = null!;
        public virtual DbSet<SysUserManual> SysUserManuals { get; set; } = null!;
        public virtual DbSet<SysUserTracking> SysUserTrackings { get; set; } = null!;
        public virtual DbSet<SysUserTrackingVw> SysUserTrackingVws { get; set; } = null!;
        public virtual DbSet<SysUserType> SysUserTypes { get; set; } = null!;
        public virtual DbSet<SysUserVw> SysUserVws { get; set; } = null!;
        public virtual DbSet<SysVatGroup> SysVatGroups { get; set; } = null!;
        public virtual DbSet<SysVatGroupVw> SysVatGroupVws { get; set; } = null!;
        public virtual DbSet<SysVatReportsVw> SysVatReportsVws { get; set; } = null!;
        public virtual DbSet<SysVersion> SysVersions { get; set; } = null!;
        public virtual DbSet<TndTender> TndTenders { get; set; } = null!;
        public virtual DbSet<TndTenderGuarantee> TndTenderGuarantees { get; set; } = null!;
        public virtual DbSet<TndTenderGuaranteesVw> TndTenderGuaranteesVws { get; set; } = null!;
        public virtual DbSet<TndTenderVw> TndTenderVws { get; set; } = null!;
        public virtual DbSet<TndTendersItem> TndTendersItems { get; set; } = null!;
        public virtual DbSet<TndTendersItemsApprove> TndTendersItemsApproves { get; set; } = null!;
        public virtual DbSet<TndTendersItemsApproveVw> TndTendersItemsApproveVws { get; set; } = null!;
        public virtual DbSet<TndTendersItemsPrice> TndTendersItemsPrices { get; set; } = null!;
        public virtual DbSet<TndTendersItemsPriceVw> TndTendersItemsPriceVws { get; set; } = null!;
        public virtual DbSet<TransCar> TransCars { get; set; } = null!;
        public virtual DbSet<TransCarDriver> TransCarDrivers { get; set; } = null!;
        public virtual DbSet<TransCarsType> TransCarsTypes { get; set; } = null!;
        public virtual DbSet<TransCarsTypeVw> TransCarsTypeVws { get; set; } = null!;
        public virtual DbSet<TransCarsVw> TransCarsVws { get; set; } = null!;
        public virtual DbSet<TransContainer> TransContainers { get; set; } = null!;
        public virtual DbSet<TransContainerSize> TransContainerSizes { get; set; } = null!;
        public virtual DbSet<TransContainersState> TransContainersStates { get; set; } = null!;
        public virtual DbSet<TransContainersStateVw> TransContainersStateVws { get; set; } = null!;
        public virtual DbSet<TransContainersVw> TransContainersVws { get; set; } = null!;
        public virtual DbSet<TransContract> TransContracts { get; set; } = null!;
        public virtual DbSet<TransContractContainer> TransContractContainers { get; set; } = null!;
        public virtual DbSet<TransContractContainersVw> TransContractContainersVws { get; set; } = null!;
        public virtual DbSet<TransContractCustomer> TransContractCustomers { get; set; } = null!;
        public virtual DbSet<TransContractCustomersVw> TransContractCustomersVws { get; set; } = null!;
        public virtual DbSet<TransContractInstallment> TransContractInstallments { get; set; } = null!;
        public virtual DbSet<TransContractInstallmentListVw> TransContractInstallmentListVws { get; set; } = null!;
        public virtual DbSet<TransContractInstallmentPayment> TransContractInstallmentPayments { get; set; } = null!;
        public virtual DbSet<TransContractInstallmentPaymentVw> TransContractInstallmentPaymentVws { get; set; } = null!;
        public virtual DbSet<TransContractInstallmentVw> TransContractInstallmentVws { get; set; } = null!;
        public virtual DbSet<TransContractPrintVw> TransContractPrintVws { get; set; } = null!;
        public virtual DbSet<TransContractTicketVw> TransContractTicketVws { get; set; } = null!;
        public virtual DbSet<TransContractType> TransContractTypes { get; set; } = null!;
        public virtual DbSet<TransContractVisit> TransContractVisits { get; set; } = null!;
        public virtual DbSet<TransContractVisitVw> TransContractVisitVws { get; set; } = null!;
        public virtual DbSet<TransContractVw> TransContractVws { get; set; } = null!;
        public virtual DbSet<TransDriversCarsRef> TransDriversCarsRefs { get; set; } = null!;
        public virtual DbSet<TransDriversCarsVw> TransDriversCarsVws { get; set; } = null!;
        public virtual DbSet<TransInvoice> TransInvoices { get; set; } = null!;
        public virtual DbSet<TransInvoicesInstallment> TransInvoicesInstallments { get; set; } = null!;
        public virtual DbSet<TransInvoicesInstallmentVw> TransInvoicesInstallmentVws { get; set; } = null!;
        public virtual DbSet<TransInvoicesType> TransInvoicesTypes { get; set; } = null!;
        public virtual DbSet<TransInvoicesVw> TransInvoicesVws { get; set; } = null!;
        public virtual DbSet<TransKiloMeterProfit> TransKiloMeterProfits { get; set; } = null!;
        public virtual DbSet<TransLine> TransLines { get; set; } = null!;
        public virtual DbSet<TransLinePrice> TransLinePrices { get; set; } = null!;
        public virtual DbSet<TransLinesPrice> TransLinesPrices { get; set; } = null!;
        public virtual DbSet<TransRenewContract> TransRenewContracts { get; set; } = null!;
        public virtual DbSet<TransRoute> TransRoutes { get; set; } = null!;
        public virtual DbSet<TransRouteCity> TransRouteCities { get; set; } = null!;
        public virtual DbSet<TransRouteCityVw> TransRouteCityVws { get; set; } = null!;
        public virtual DbSet<TransRouteVw> TransRouteVws { get; set; } = null!;
        public virtual DbSet<TransService> TransServices { get; set; } = null!;
        public virtual DbSet<TransServicesType> TransServicesTypes { get; set; } = null!;
        public virtual DbSet<TransServicesTypesVw> TransServicesTypesVws { get; set; } = null!;
        public virtual DbSet<TransSettingsLicensesType> TransSettingsLicensesTypes { get; set; } = null!;
        public virtual DbSet<TransSmsSetting> TransSmsSettings { get; set; } = null!;
        public virtual DbSet<TransTicket> TransTickets { get; set; } = null!;
        public virtual DbSet<TransTicketVw> TransTicketVws { get; set; } = null!;
        public virtual DbSet<TransTransaction> TransTransactions { get; set; } = null!;
        public virtual DbSet<TransTransactionsDetail> TransTransactionsDetails { get; set; } = null!;
        public virtual DbSet<TransTransactionsDetailsVw> TransTransactionsDetailsVws { get; set; } = null!;
        public virtual DbSet<TransTransactionsDiscount> TransTransactionsDiscounts { get; set; } = null!;
        public virtual DbSet<TransTransactionsDiscountVw> TransTransactionsDiscountVws { get; set; } = null!;
        public virtual DbSet<TransTransactionsPayment> TransTransactionsPayments { get; set; } = null!;
        public virtual DbSet<TransTransactionsPaymentVw> TransTransactionsPaymentVws { get; set; } = null!;
        public virtual DbSet<TransTransactionsPrintVw> TransTransactionsPrintVws { get; set; } = null!;
        public virtual DbSet<TransTransactionsType> TransTransactionsTypes { get; set; } = null!;
        public virtual DbSet<TransTransactionsTypeVw> TransTransactionsTypeVws { get; set; } = null!;
        public virtual DbSet<TransTransactionsVw> TransTransactionsVws { get; set; } = null!;
        public virtual DbSet<TransTrip> TransTrips { get; set; } = null!;
        public virtual DbSet<TransTripsDetaile> TransTripsDetailes { get; set; } = null!;
        public virtual DbSet<TransTripsDetailesDeliveredVw> TransTripsDetailesDeliveredVws { get; set; } = null!;
        public virtual DbSet<TransTripsDetailesVw> TransTripsDetailesVws { get; set; } = null!;
        public virtual DbSet<TransTripsVw> TransTripsVws { get; set; } = null!;
        public virtual DbSet<TransVisit> TransVisits { get; set; } = null!;
        public virtual DbSet<TransVisitsVw> TransVisitsVws { get; set; } = null!;
        public virtual DbSet<TransactionEmp2Vw> TransactionEmp2Vws { get; set; } = null!;
        public virtual DbSet<TransactionEmpVw> TransactionEmpVws { get; set; } = null!;
        public virtual DbSet<TrnCourse> TrnCourses { get; set; } = null!;
        public virtual DbSet<TrnCourseDegree> TrnCourseDegrees { get; set; } = null!;
        public virtual DbSet<TrnCourseDegreeVw> TrnCourseDegreeVws { get; set; } = null!;
        public virtual DbSet<TrnExpensesDetail> TrnExpensesDetails { get; set; } = null!;
        public virtual DbSet<TrnExpensesDetailsVw> TrnExpensesDetailsVws { get; set; } = null!;
        public virtual DbSet<TrnExpensesM> TrnExpensesMs { get; set; } = null!;
        public virtual DbSet<TrnExpensesMAllVw> TrnExpensesMAllVws { get; set; } = null!;
        public virtual DbSet<TrnExpensesMVw> TrnExpensesMVws { get; set; } = null!;
        public virtual DbSet<TrnExpensesType> TrnExpensesTypes { get; set; } = null!;
        public virtual DbSet<TrnExpensesTypeVw> TrnExpensesTypeVws { get; set; } = null!;
        public virtual DbSet<TrnMethodDistributionFee> TrnMethodDistributionFees { get; set; } = null!;
        public virtual DbSet<TrnMethodDistributionFeesDetailed> TrnMethodDistributionFeesDetaileds { get; set; } = null!;
        public virtual DbSet<TrnProgram> TrnPrograms { get; set; } = null!;
        public virtual DbSet<TrnProgramInstallVw> TrnProgramInstallVws { get; set; } = null!;
        public virtual DbSet<TrnProgramInstallVw2> TrnProgramInstallVw2s { get; set; } = null!;
        public virtual DbSet<TrnProgramShortInstallPrintVw> TrnProgramShortInstallPrintVws { get; set; } = null!;
        public virtual DbSet<TrnProgramShortInstallVw> TrnProgramShortInstallVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTrainee> TrnProgramTrainees { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineePayment> TrnProgramTraineePayments { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineePaymentD> TrnProgramTraineePaymentDs { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineePaymentDVw> TrnProgramTraineePaymentDVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineePaymentVw> TrnProgramTraineePaymentVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineesInstall> TrnProgramTraineesInstalls { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineesInstallAllVw> TrnProgramTraineesInstallAllVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineesInstallVw> TrnProgramTraineesInstallVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTraineesVw> TrnProgramTraineesVws { get; set; } = null!;
        public virtual DbSet<TrnProgramTrainer> TrnProgramTrainers { get; set; } = null!;
        public virtual DbSet<TrnProgramTrainerPayment> TrnProgramTrainerPayments { get; set; } = null!;
        public virtual DbSet<TrnProgramTrainersVw> TrnProgramTrainersVws { get; set; } = null!;
        public virtual DbSet<TrnProgramsPrintVw> TrnProgramsPrintVws { get; set; } = null!;
        public virtual DbSet<TrnProgramsType> TrnProgramsTypes { get; set; } = null!;
        public virtual DbSet<TrnProgramsTypeVw> TrnProgramsTypeVws { get; set; } = null!;
        public virtual DbSet<TrnProgramsVw> TrnProgramsVws { get; set; } = null!;
        public virtual DbSet<TrnStudyPlan> TrnStudyPlans { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanDegree> TrnStudyPlanDegrees { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanDegreeVw> TrnStudyPlanDegreeVws { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanDetail> TrnStudyPlanDetails { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanDetailsDegreeVw> TrnStudyPlanDetailsDegreeVws { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanDetailsVw> TrnStudyPlanDetailsVws { get; set; } = null!;
        public virtual DbSet<TrnStudyPlanVw> TrnStudyPlanVws { get; set; } = null!;
        public virtual DbSet<TrnTrainee> TrnTrainees { get; set; } = null!;
        public virtual DbSet<TrnTraineesVw> TrnTraineesVws { get; set; } = null!;
        public virtual DbSet<TrnTrainer> TrnTrainers { get; set; } = null!;
        public virtual DbSet<TrnTrainerType> TrnTrainerTypes { get; set; } = null!;
        public virtual DbSet<TrnTrainersVw> TrnTrainersVws { get; set; } = null!;
        public virtual DbSet<TrnType> TrnTypes { get; set; } = null!;
        public virtual DbSet<WebMenu> WebMenus { get; set; } = null!;
        public virtual DbSet<WfAppGroup> WfAppGroups { get; set; } = null!;
        public virtual DbSet<WfAppGroupsVw> WfAppGroupsVws { get; set; } = null!;
        public virtual DbSet<WfAppType> WfAppTypes { get; set; } = null!;
        public virtual DbSet<WfAppTypeTable> WfAppTypeTables { get; set; } = null!;
        public virtual DbSet<WfAppTypeVw> WfAppTypeVws { get; set; } = null!;
        public virtual DbSet<WfApplication> WfApplications { get; set; } = null!;
        public virtual DbSet<WfApplicationsAssigne> WfApplicationsAssignes { get; set; } = null!;
        public virtual DbSet<WfApplicationsAssignesReply> WfApplicationsAssignesReplies { get; set; } = null!;
        public virtual DbSet<WfApplicationsAssignesReplyVw> WfApplicationsAssignesReplyVws { get; set; } = null!;
        public virtual DbSet<WfApplicationsAssignesVw> WfApplicationsAssignesVws { get; set; } = null!;
        public virtual DbSet<WfApplicationsComment> WfApplicationsComments { get; set; } = null!;
        public virtual DbSet<WfApplicationsCommentsVw> WfApplicationsCommentsVws { get; set; } = null!;
        public virtual DbSet<WfApplicationsStatus> WfApplicationsStatuses { get; set; } = null!;
        public virtual DbSet<WfApplicationsStatusVw> WfApplicationsStatusVws { get; set; } = null!;
        public virtual DbSet<WfApplicationsVw> WfApplicationsVws { get; set; } = null!;
        public virtual DbSet<WfDynamicAttribute> WfDynamicAttributes { get; set; } = null!;
        public virtual DbSet<WfDynamicAttributeDataType> WfDynamicAttributeDataTypes { get; set; } = null!;
        public virtual DbSet<WfDynamicAttributesTable> WfDynamicAttributesTables { get; set; } = null!;
        public virtual DbSet<WfDynamicAttributesTableVw> WfDynamicAttributesTableVws { get; set; } = null!;
        public virtual DbSet<WfDynamicAttributesVw> WfDynamicAttributesVws { get; set; } = null!;
        public virtual DbSet<WfDynamicTableValue> WfDynamicTableValues { get; set; } = null!;
        public virtual DbSet<WfDynamicValue> WfDynamicValues { get; set; } = null!;
        public virtual DbSet<WfEscalation> WfEscalations { get; set; } = null!;
        public virtual DbSet<WfEscalationVw> WfEscalationVws { get; set; } = null!;
        public virtual DbSet<WfLookUpCatagory> WfLookUpCatagories { get; set; } = null!;
        public virtual DbSet<WfLookupDataVw> WfLookupDataVws { get; set; } = null!;
        public virtual DbSet<WfLookupDatum> WfLookupData { get; set; } = null!;
        public virtual DbSet<WfLookupType> WfLookupTypes { get; set; } = null!;
        public virtual DbSet<WfStatus> WfStatuses { get; set; } = null!;
        public virtual DbSet<WfStatusVw> WfStatusVws { get; set; } = null!;
        public virtual DbSet<WfStep> WfSteps { get; set; } = null!;
        public virtual DbSet<WfStepLevel> WfStepLevels { get; set; } = null!;
        public virtual DbSet<WfStepsNotification> WfStepsNotifications { get; set; } = null!;
        public virtual DbSet<WfStepsTransaction> WfStepsTransactions { get; set; } = null!;
        public virtual DbSet<WfStepsTransactionsVw> WfStepsTransactionsVws { get; set; } = null!;
        public virtual DbSet<WfStepsType> WfStepsTypes { get; set; } = null!;
        public virtual DbSet<WfStepsVw> WfStepsVws { get; set; } = null!;
        public virtual DbSet<WhAccountType> WhAccountTypes { get; set; } = null!;
        public virtual DbSet<WhActualInventorySeriale> WhActualInventorySeriales { get; set; } = null!;
        public virtual DbSet<WhActualInventorySerialesVw> WhActualInventorySerialesVws { get; set; } = null!;
        public virtual DbSet<WhBalanceSheet> WhBalanceSheets { get; set; } = null!;
        public virtual DbSet<WhBalanceSheetD> WhBalanceSheetDs { get; set; } = null!;
        public virtual DbSet<WhBarcodeType> WhBarcodeTypes { get; set; } = null!;
        public virtual DbSet<WhCloseInventory> WhCloseInventories { get; set; } = null!;
        public virtual DbSet<WhCloseInventoryVw> WhCloseInventoryVws { get; set; } = null!;
        public virtual DbSet<WhInventoriesDetaile> WhInventoriesDetailes { get; set; } = null!;
        public virtual DbSet<WhInventoriesDetailesVw> WhInventoriesDetailesVws { get; set; } = null!;
        public virtual DbSet<WhInventoriesMaster> WhInventoriesMasters { get; set; } = null!;
        public virtual DbSet<WhInventoriesMasterVw> WhInventoriesMasterVws { get; set; } = null!;
        public virtual DbSet<WhInventoriesStatus> WhInventoriesStatuses { get; set; } = null!;
        public virtual DbSet<WhInventoriesTransactionsType> WhInventoriesTransactionsTypes { get; set; } = null!;
        public virtual DbSet<WhInventoriesVw> WhInventoriesVws { get; set; } = null!;
        public virtual DbSet<WhInventory> WhInventories { get; set; } = null!;
        public virtual DbSet<WhInventorySection> WhInventorySections { get; set; } = null!;
        public virtual DbSet<WhInventorySectionsEmp> WhInventorySectionsEmps { get; set; } = null!;
        public virtual DbSet<WhInventorySectionsEmpVw> WhInventorySectionsEmpVws { get; set; } = null!;
        public virtual DbSet<WhInventorySectionsVw> WhInventorySectionsVws { get; set; } = null!;
        public virtual DbSet<WhItem> WhItems { get; set; } = null!;
        public virtual DbSet<WhItemProperty> WhItemProperties { get; set; } = null!;
        public virtual DbSet<WhItemPropertyVw> WhItemPropertyVws { get; set; } = null!;
        public virtual DbSet<WhItemTemplate> WhItemTemplates { get; set; } = null!;
        public virtual DbSet<WhItemTemplateVw> WhItemTemplateVws { get; set; } = null!;
        public virtual DbSet<WhItemType2Vw> WhItemType2Vws { get; set; } = null!;
        public virtual DbSet<WhItemValuationCosting> WhItemValuationCostings { get; set; } = null!;
        public virtual DbSet<WhItemValuationCostingVw> WhItemValuationCostingVws { get; set; } = null!;
        public virtual DbSet<WhItemsActionsVw> WhItemsActionsVws { get; set; } = null!;
        public virtual DbSet<WhItemsBalance> WhItemsBalances { get; set; } = null!;
        public virtual DbSet<WhItemsBatch> WhItemsBatches { get; set; } = null!;
        public virtual DbSet<WhItemsBatchListVw> WhItemsBatchListVws { get; set; } = null!;
        public virtual DbSet<WhItemsCatagoriesVw> WhItemsCatagoriesVws { get; set; } = null!;
        public virtual DbSet<WhItemsCatagory> WhItemsCatagories { get; set; } = null!;
        public virtual DbSet<WhItemsColorVw> WhItemsColorVws { get; set; } = null!;
        public virtual DbSet<WhItemsComponent> WhItemsComponents { get; set; } = null!;
        public virtual DbSet<WhItemsComponentsVw> WhItemsComponentsVws { get; set; } = null!;
        public virtual DbSet<WhItemsCostingChange> WhItemsCostingChanges { get; set; } = null!;
        public virtual DbSet<WhItemsInventory> WhItemsInventories { get; set; } = null!;
        public virtual DbSet<WhItemsInventoryVw> WhItemsInventoryVws { get; set; } = null!;
        public virtual DbSet<WhItemsListVw> WhItemsListVws { get; set; } = null!;
        public virtual DbSet<WhItemsNotificationsVw> WhItemsNotificationsVws { get; set; } = null!;
        public virtual DbSet<WhItemsSection> WhItemsSections { get; set; } = null!;
        public virtual DbSet<WhItemsSectionsVw> WhItemsSectionsVws { get; set; } = null!;
        public virtual DbSet<WhItemsSerial> WhItemsSerials { get; set; } = null!;
        public virtual DbSet<WhItemsSerialsVw> WhItemsSerialsVws { get; set; } = null!;
        public virtual DbSet<WhItemsStatusVw> WhItemsStatusVws { get; set; } = null!;
        public virtual DbSet<WhItemsSupplier> WhItemsSuppliers { get; set; } = null!;
        public virtual DbSet<WhItemsSupplierVw> WhItemsSupplierVws { get; set; } = null!;
        public virtual DbSet<WhItemsType> WhItemsTypes { get; set; } = null!;
        public virtual DbSet<WhItemsUnit> WhItemsUnits { get; set; } = null!;
        public virtual DbSet<WhItemsUnitListVw> WhItemsUnitListVws { get; set; } = null!;
        public virtual DbSet<WhItemsUnitVw> WhItemsUnitVws { get; set; } = null!;
        public virtual DbSet<WhItemsVw> WhItemsVws { get; set; } = null!;
        public virtual DbSet<WhListTemplateVw> WhListTemplateVws { get; set; } = null!;
        public virtual DbSet<WhManufactureCountryVw> WhManufactureCountryVws { get; set; } = null!;
        public virtual DbSet<WhManufacturingYearVw> WhManufacturingYearVws { get; set; } = null!;
        public virtual DbSet<WhOrder> WhOrders { get; set; } = null!;
        public virtual DbSet<WhOrderDetail> WhOrderDetails { get; set; } = null!;
        public virtual DbSet<WhOrderDetailsVw> WhOrderDetailsVws { get; set; } = null!;
        public virtual DbSet<WhOrdersVw> WhOrdersVws { get; set; } = null!;
        public virtual DbSet<WhPaymentType> WhPaymentTypes { get; set; } = null!;
        public virtual DbSet<WhSetting> WhSettings { get; set; } = null!;
        public virtual DbSet<WhTemplate> WhTemplates { get; set; } = null!;
        public virtual DbSet<WhTransactionsCarVw> WhTransactionsCarVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsDetaile> WhTransactionsDetailes { get; set; } = null!;
        public virtual DbSet<WhTransactionsDetailes3Vw> WhTransactionsDetailes3Vws { get; set; } = null!;
        public virtual DbSet<WhTransactionsDetailesVw> WhTransactionsDetailesVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsExpense> WhTransactionsExpenses { get; set; } = null!;
        public virtual DbSet<WhTransactionsMaster> WhTransactionsMasters { get; set; } = null!;
        public virtual DbSet<WhTransactionsMasterVw> WhTransactionsMasterVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsReferenceType> WhTransactionsReferenceTypes { get; set; } = null!;
        public virtual DbSet<WhTransactionsReferenceTypeVw> WhTransactionsReferenceTypeVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsReservationVw> WhTransactionsReservationVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsSalesVw> WhTransactionsSalesVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsScheduling> WhTransactionsSchedulings { get; set; } = null!;
        public virtual DbSet<WhTransactionsSchedulingVw> WhTransactionsSchedulingVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsSeriale> WhTransactionsSeriales { get; set; } = null!;
        public virtual DbSet<WhTransactionsSerialesVw> WhTransactionsSerialesVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsStaff> WhTransactionsStaffs { get; set; } = null!;
        public virtual DbSet<WhTransactionsStaffVw> WhTransactionsStaffVws { get; set; } = null!;
        public virtual DbSet<WhTransactionsStatus> WhTransactionsStatuses { get; set; } = null!;
        public virtual DbSet<WhTransactionsType> WhTransactionsTypes { get; set; } = null!;
        public virtual DbSet<WhTransactionsTypeVw> WhTransactionsTypeVws { get; set; } = null!;
        public virtual DbSet<WhTransferMethodVw> WhTransferMethodVws { get; set; } = null!;
        public virtual DbSet<WhTransferTypeVw> WhTransferTypeVws { get; set; } = null!;
        public virtual DbSet<WhUnit> WhUnits { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=PC13\\SQLEXPRESS19;database=Logix_X_2022;Trusted_Connection=yes;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccAccount>(entity =>
            {
                entity.HasIndex(e => new { e.AccAccountCode, e.FlagDelete, e.FacilityId }, "Acc_Code")
                    .IsUnique()
                    .HasFilter("([FlagDelete]=(0))");

                entity.Property(e => e.CcId).HasComment("مركز التكلفة الافتراضي");

                entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsHelpAccount).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccAccountStatementVw>(entity =>
            {
                entity.ToView("Acc_Account_statement_VW");

                entity.Property(e => e.JDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<AccAccountsCostcenter>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccAccountsCostcenterVw>(entity =>
            {
                entity.ToView("Acc_Accounts_Costcenter_VW");
            });

            modelBuilder.Entity<AccAccountsGroupsFinalVw>(entity =>
            {
                entity.ToView("ACC_Accounts_Groups_Final_VW");

                entity.Property(e => e.AccAccountId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccAccountsLevel>(entity =>
            {
                entity.Property(e => e.LevelId).ValueGeneratedNever();
            });

            modelBuilder.Entity<AccAccountsRefrancesVw>(entity =>
            {
                entity.ToView("Acc_Accounts_Refrances_VW");
            });

            modelBuilder.Entity<AccAccountsReportsVw>(entity =>
            {
                entity.ToView("ACC_Accounts_Reports_VW");
            });

            modelBuilder.Entity<AccAccountsSubHelpeVw>(entity =>
            {
                entity.ToView("ACC_Accounts_Sub_helpe_VW");

                entity.Property(e => e.AccAccountId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccAccountsSubVw>(entity =>
            {
                entity.ToView("ACC_Accounts_Sub_VW");

                entity.Property(e => e.AccAccountId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccAccountsVw>(entity =>
            {
                entity.ToView("ACC_Accounts_VW");
            });

            modelBuilder.Entity<AccActivitesVw>(entity =>
            {
                entity.ToView("Acc_Activites_VW");
            });

            modelBuilder.Entity<AccBalanceSheet>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheet2Vw>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet2_VW");

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetContractor>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Contractors");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetCostCenterVw>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_CostCenter_VW");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetCustomer>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Customers");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetDonor>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Donor");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetEmployee>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Employee");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetPostOrNot>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_PostOrNot");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetStudent>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Student");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBalanceSheetSupplier>(entity =>
            {
                entity.ToView("ACC_Balance_Sheet_Supplier");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccBank>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccBankVw>(entity =>
            {
                entity.ToView("ACC_Bank_VW");
            });

            modelBuilder.Entity<AccBranchAccount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccBranchAccountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccBranchAccountsVw>(entity =>
            {
                entity.ToView("Acc_Branch_Accounts_VW");
            });

            modelBuilder.Entity<AccBranchVw>(entity =>
            {
                entity.ToView("ACC_Branch_VW");

                entity.Property(e => e.BranchId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccBudgetEstimateBalanceVw>(entity =>
            {
                entity.ToView("Acc_Budget_Estimate_Balance_VW");
            });

            modelBuilder.Entity<AccBudgetEstimateDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReferenceNo).HasComment("رقم المرجع في نظام التقسيط");
            });

            modelBuilder.Entity<AccBudgetEstimateDetailesVw>(entity =>
            {
                entity.ToView("Acc_Budget_Estimate_Detailes_VW");
            });

            modelBuilder.Entity<AccBudgetEstimateMaster>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccBudgetEstimateVw>(entity =>
            {
                entity.ToView("Acc_Budget_Estimate_VW");
            });

            modelBuilder.Entity<AccCashOnHand>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccCashOnHandListVw>(entity =>
            {
                entity.ToView("Acc_Cash_on_hand_List_VW");
            });

            modelBuilder.Entity<AccCashOnHandVw>(entity =>
            {
                entity.ToView("Acc_Cash_on_hand_VW");
            });

            modelBuilder.Entity<AccCheque>(entity =>
            {
                entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

                entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChequDate).IsFixedLength();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccChequeReturn>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccChequeReturnVw>(entity =>
            {
                entity.ToView("ACC_Cheque_Return_VW");
            });

            modelBuilder.Entity<AccChequesNotesVw>(entity =>
            {
                entity.ToView("Acc_Cheques_Notes_VW");
            });

            modelBuilder.Entity<AccChequesStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccChequesStatusVw>(entity =>
            {
                entity.ToView("ACC_Cheques_Status_VW");
            });

            modelBuilder.Entity<AccChequesVw>(entity =>
            {
                entity.ToView("ACC_Cheques_VW");

                entity.Property(e => e.ChequDate).IsFixedLength();
            });

            modelBuilder.Entity<AccCostCenteHelpVw>(entity =>
            {
                entity.ToView("ACC_CostCente_Help_VW");

                entity.Property(e => e.CcId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccCostCenter>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<AccCostCenterListVw>(entity =>
            {
                entity.ToView("ACC_CostCenter_List_VW");

                entity.Property(e => e.CcId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccCostCenterVw>(entity =>
            {
                entity.ToView("ACC_CostCenter_VW");
            });

            modelBuilder.Entity<AccDocumentType>(entity =>
            {
                entity.Property(e => e.DocTypeId).ValueGeneratedNever();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccDocumentTypeListVw>(entity =>
            {
                entity.ToView("ACC_Document_Type_List_VW");
            });

            modelBuilder.Entity<AccExpensesIncome>(entity =>
            {
                entity.ToView("ACC_Expenses_Income");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccFacilitiesVw>(entity =>
            {
                entity.ToView("ACC_Facilities_VW");
            });

            modelBuilder.Entity<AccFacility>(entity =>
            {
                entity.HasKey(e => e.FacilityId)
                    .HasName("PK_Facilities");

                entity.Property(e => e.AccountBranches).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountCash).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountCashSales).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountChequ).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountChequUnderCollection).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountContractors).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountCostGoodsSold).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountCostSales).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountCustomer).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountFeeManage).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountInstallmentsUnderCollection).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountInventoryTransit).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountInvestor).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountInvestorProfits).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountMerchandiseInventory).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountProfitInstallment).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountProfitInstallmentDeferred).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountReceivablesSales).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountSales).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountSalesProfits).HasDefaultValueSql("((0))");

                entity.Property(e => e.AccountSupplier).HasDefaultValueSql("((0))");

                entity.Property(e => e.CcIdItems).HasDefaultValueSql("((0))");

                entity.Property(e => e.CcIdProjects).HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountAccountId).HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountCreditAccountId).HasDefaultValueSql("((0))");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.GroupAssets).HasDefaultValueSql("((0))");

                entity.Property(e => e.GroupCopyrights).HasDefaultValueSql("((0))");

                entity.Property(e => e.GroupExpenses).HasDefaultValueSql("((0))");

                entity.Property(e => e.GroupIncame).HasDefaultValueSql("((0))");

                entity.Property(e => e.GroupLiabilities).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Posting).HasDefaultValueSql("((1))");

                entity.Property(e => e.SalesAccountType).HasDefaultValueSql("((1))");

                entity.Property(e => e.SeparateAccountCustomer).HasDefaultValueSql("((2))");

                entity.Property(e => e.SeparateAccountSupplier).HasDefaultValueSql("((2))");

                entity.Property(e => e.UsingPurchaseAccount).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccFinancialPositionVw>(entity =>
            {
                entity.ToView("ACC_Financial_Position_VW");

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccFinancialYear>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccFinancialYearVw>(entity =>
            {
                entity.ToView("ACC_Financial_Year_VW");

                entity.Property(e => e.FinYear).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccGroup>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccGroupVw>(entity =>
            {
                entity.ToView("ACC_Group_VW");

                entity.Property(e => e.AccGroupId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccGuarantee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccJournalCommentsVw>(entity =>
            {
                entity.ToView("ACC_Journal_Comments_VW");
            });

            modelBuilder.Entity<AccJournalDetaile>(entity =>
            {
                entity.Property(e => e.Auto).HasDefaultValueSql("((0))");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReferenceNo).HasComment("رقم المرجع في نظام التقسيط");
            });

            modelBuilder.Entity<AccJournalDetailesCostcenter>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccJournalDetailesCostcenterVw>(entity =>
            {
                entity.ToView("ACC_Journal_Detailes_Costcenter_VW");
            });

            modelBuilder.Entity<AccJournalDetailesVw>(entity =>
            {
                entity.ToView("ACC_Journal_Detailes_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();

                entity.Property(e => e.MJDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<AccJournalMaster>(entity =>
            {
                entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

                entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();

                entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccJournalMasterExportVw>(entity =>
            {
                entity.ToView("ACC_Journal_Master_Export_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.JDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<AccJournalMasterFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccJournalMasterFilesVw>(entity =>
            {
                entity.ToView("ACC_Journal_Master_Files_VW");

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccJournalMasterStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<AccJournalMasterVw>(entity =>
            {
                entity.ToView("ACC_Journal_Master_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.JDateGregorian).IsFixedLength();

                entity.Property(e => e.JDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccJournalSignatureVw>(entity =>
            {
                entity.ToView("ACC_Journal_Signature_VW");
            });

            modelBuilder.Entity<AccPaymentType>(entity =>
            {
                entity.Property(e => e.PaymentTypeId).ValueGeneratedNever();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccPeriod>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.PeriodEndDateGregorian).IsFixedLength();

                entity.Property(e => e.PeriodEndDateHijri).IsFixedLength();

                entity.Property(e => e.PeriodStartDateGregorian).IsFixedLength();

                entity.Property(e => e.PeriodStartDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccPeriodDateVw>(entity =>
            {
                entity.ToView("ACC_Period_Date_VW");

                entity.Property(e => e.PeriodId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccPeriodsVw>(entity =>
            {
                entity.ToView("ACC_Periods_VW");

                entity.Property(e => e.PeriodEndDateGregorian).IsFixedLength();

                entity.Property(e => e.PeriodEndDateHijri).IsFixedLength();

                entity.Property(e => e.PeriodStartDateGregorian).IsFixedLength();

                entity.Property(e => e.PeriodStartDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccPettyCash>(entity =>
            {
                entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccPettyCashD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccPettyCashDVw>(entity =>
            {
                entity.ToView("Acc_Petty_Cash_D_VW");
            });

            modelBuilder.Entity<AccPettyCashExpenseVw>(entity =>
            {
                entity.ToView("Acc_Petty_Cash_Expense_VW");
            });

            modelBuilder.Entity<AccPettyCashExpensesType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccPettyCashExpensesTypeVw>(entity =>
            {
                entity.ToView("ACC_Petty_Cash_Expenses_Type_VW");
            });

            modelBuilder.Entity<AccPettyCashTemp>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccPettyCashTempVw>(entity =>
            {
                entity.ToView("Acc_Petty_Cash_Temp_VW");
            });

            modelBuilder.Entity<AccPettyCashVw>(entity =>
            {
                entity.ToView("Acc_Petty_Cash_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<AccReceivablesPayable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReceivablesPayablesAccount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReceivablesPayablesAccountsVw>(entity =>
            {
                entity.ToView("Acc_Receivables_Payables_Accounts_VW");
            });

            modelBuilder.Entity<AccReceivablesPayablesD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReceivablesPayablesDVw>(entity =>
            {
                entity.ToView("Acc_Receivables_Payables_D_VW");
            });

            modelBuilder.Entity<AccReceivablesPayablesTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReceivablesPayablesTransactionDVw>(entity =>
            {
                entity.ToView("Acc_Receivables_Payables_Transaction_D_VW");
            });

            modelBuilder.Entity<AccReceivablesPayablesTransactionVw>(entity =>
            {
                entity.ToView("Acc_Receivables_Payables_Transaction_VW");
            });

            modelBuilder.Entity<AccReceivablesPayablesType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<AccReceivablesPayablesVw>(entity =>
            {
                entity.ToView("Acc_Receivables_Payables_VW");
            });

            modelBuilder.Entity<AccReconciliation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReconciliationDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccReconciliationDetailesVw>(entity =>
            {
                entity.ToView("ACC_Reconciliation_Detailes_VW");
            });

            modelBuilder.Entity<AccReconciliationVw>(entity =>
            {
                entity.ToView("ACC_Reconciliation_VW");
            });

            modelBuilder.Entity<AccReferenceType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccReferenceTypeVw>(entity =>
            {
                entity.ToView("ACC_Reference_Type_VW");
            });

            modelBuilder.Entity<AccRequest>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

                entity.Property(e => e.ExchangeRate).HasDefaultValueSql("((1))");

                entity.Property(e => e.JId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AccRequestBalanceStatusVw>(entity =>
            {
                entity.ToView("Acc_Request_Balance_Status_VW");
            });

            modelBuilder.Entity<AccRequestExchangeStatusVw>(entity =>
            {
                entity.ToView("Acc_Request_Exchange_Status_VW");
            });

            modelBuilder.Entity<AccRequestHasCreditVw>(entity =>
            {
                entity.ToView("Acc_Request_Has_Credit_VW");
            });

            modelBuilder.Entity<AccRequestJournalVw>(entity =>
            {
                entity.ToView("Acc_Request_Journal_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.JDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<AccRequestVw>(entity =>
            {
                entity.ToView("Acc_Request_VW");
            });

            modelBuilder.Entity<AccSettlementInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccSettlementInstallmentsVw>(entity =>
            {
                entity.ToView("Acc_Settlement_Installments_VW");
            });

            modelBuilder.Entity<AccSettlementSchedule>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccSettlementScheduleD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<AccSettlementScheduleDVw>(entity =>
            {
                entity.ToView("Acc_Settlement_Schedule_D_VW");
            });

            modelBuilder.Entity<AccSettlementScheduleVw>(entity =>
            {
                entity.ToView("Acc_Settlement_Schedule_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AccountIdPosList>(entity =>
            {
                entity.ToView("Account_ID_POS_List");
            });

            modelBuilder.Entity<AllowancesOvertimeBonusesVw>(entity =>
            {
                entity.ToView("Allowances_Overtime_Bonuses_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgAccount>(entity =>
            {
                entity.Property(e => e.CcId).HasComment("مركز التكلفة الافتراضي");

                entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsHelpAccount).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BudgAccountsSubHelpeVw>(entity =>
            {
                entity.ToView("Budg_Accounts_Sub_helpe_VW");

                entity.Property(e => e.AccAccountId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgAccountsVw>(entity =>
            {
                entity.ToView("Budg_Accounts_VW");
            });

            modelBuilder.Entity<BudgBalanceSheet>(entity =>
            {
                entity.ToView("Budg_Balance_Sheet");

                entity.Property(e => e.DateGregorian).IsFixedLength();

                entity.Property(e => e.DateHijri).IsFixedLength();
            });

            modelBuilder.Entity<BudgCreditsFinancialYear>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BudgCreditsFinancialYearVw>(entity =>
            {
                entity.ToView("Budg_Credits_Financial_Year_VW");
            });

            modelBuilder.Entity<BudgDocType>(entity =>
            {
                entity.HasKey(e => e.DocTypeId)
                    .HasName("PK_ACC_Budget_Doc_Type");

                entity.Property(e => e.DocTypeId).ValueGeneratedNever();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeId).HasComment("هل تقديري ام فعلي");
            });

            modelBuilder.Entity<BudgGroup>(entity =>
            {
                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<BudgGroupVw>(entity =>
            {
                entity.ToView("Budg_Group_VW");

                entity.Property(e => e.BudgGroupId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgRequest>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<BudgTransaction>(entity =>
            {
                entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

                entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.DateGregorian).IsFixedLength();

                entity.Property(e => e.DateHijri).IsFixedLength();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BudgTransactionDetaile>(entity =>
            {
                entity.Property(e => e.Auto).HasDefaultValueSql("((0))");

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReferenceNo).HasComment("رقم المرجع في نظام التقسيط");
            });

            modelBuilder.Entity<BudgTransactionDetailesVw>(entity =>
            {
                entity.ToView("Budg_Transaction_Detailes_VW");

                entity.Property(e => e.Expr2).IsFixedLength();
            });

            modelBuilder.Entity<BudgTransactionFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<BudgTransactionVw>(entity =>
            {
                entity.ToView("Budg_Transaction_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.DateGregorian).IsFixedLength();

                entity.Property(e => e.DateHijri).IsFixedLength();
            });

            modelBuilder.Entity<CrmActionTypeVw>(entity =>
            {
                entity.ToView("CRM_Action_Type_VW");
            });

            modelBuilder.Entity<CrmAdvertisingCampaign>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmAdvertisingCampaignsDetail>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmAdvertisingCampaignsDetailsVw>(entity =>
            {
                entity.ToView("CRM_Advertising_campaigns_Details_VW");
            });

            modelBuilder.Entity<CrmAdvertisingCampaignsLink>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmAdvertisingCampaignsVw>(entity =>
            {
                entity.ToView("CRM_Advertising_campaigns_VW");
            });

            modelBuilder.Entity<CrmCampaignTypeVw>(entity =>
            {
                entity.ToView("CRM_Campaign_Type_VW");
            });

            modelBuilder.Entity<CrmEmailTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CrmEmailTemplateAttach>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CrmMeeting>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmMeetingsAgendum>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmMeetingsStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CrmMeetingsStaffVw>(entity =>
            {
                entity.ToView("CRM_Meetings_Staff_VW");
            });

            modelBuilder.Entity<CrmMeetingsVw>(entity =>
            {
                entity.ToView("CRM_Meetings_VW");
            });

            modelBuilder.Entity<CrmOpportunitiesPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<CrmOpportunitiesPaymentVw>(entity =>
            {
                entity.ToView("CRM_Opportunities_Payment_VW");

                entity.Property(e => e.PaymentDate).IsFixedLength();
            });

            modelBuilder.Entity<CrmOpportunitiesVw>(entity =>
            {
                entity.ToView("CRM_Opportunities_VW");
            });

            modelBuilder.Entity<CrmOpportunity>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<DrvDocument>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DrvDocumentVw>(entity =>
            {
                entity.ToView("DRV_Document_VW");
            });

            modelBuilder.Entity<DrvFolder>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DrvHistoryDownload>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DrvHistoryDownloadVw>(entity =>
            {
                entity.ToView("DRV_History_Download_VW");
            });

            modelBuilder.Entity<EmergencyTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EmergencyTransactionsD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EmergencyTransactionsDVw>(entity =>
            {
                entity.ToView("Emergency_Transactions_D_VW");
            });

            modelBuilder.Entity<EmergencyTransactionsVw>(entity =>
            {
                entity.ToView("Emergency_Transactions_VW");
            });

            modelBuilder.Entity<EquAsset>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PurchasedFrom).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsInsurance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsInsuranceVw>(entity =>
            {
                entity.ToView("EQU_Assets_Insurance_VW");
            });

            modelBuilder.Entity<EquAssetsMetering>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsMeteringVw>(entity =>
            {
                entity.ToView("EQU_Assets_Metering_VW");
            });

            modelBuilder.Entity<EquAssetsNote>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsNoteVw>(entity =>
            {
                entity.ToView("EQU_Assets_Note_VW");
            });

            modelBuilder.Entity<EquAssetsPart>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsPartsVw>(entity =>
            {
                entity.ToView("EQU_Assets_Parts_VW");
            });

            modelBuilder.Entity<EquAssetsStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsStatusVw>(entity =>
            {
                entity.ToView("EQU_Assets_Status_VW");
            });

            modelBuilder.Entity<EquAssetsTransfer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsTransferVw>(entity =>
            {
                entity.ToView("EQU_Assets_Transfer_VW");
            });

            modelBuilder.Entity<EquAssetsType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsTypeVw>(entity =>
            {
                entity.ToView("EQU_Assets_Type_VW");
            });

            modelBuilder.Entity<EquAssetsUser>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquAssetsUsersVw>(entity =>
            {
                entity.ToView("EQU_Assets_Users_VW");
            });

            modelBuilder.Entity<EquAssetsVw>(entity =>
            {
                entity.ToView("EQU_Assets_VW");
            });

            modelBuilder.Entity<EquAssetsWarrantiesVw>(entity =>
            {
                entity.ToView("EQU_Assets_Warranties_VW");
            });

            modelBuilder.Entity<EquAssetsWarranty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquCheckItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquCheckItemsPiece>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquCheckItemsPieceVw>(entity =>
            {
                entity.ToView("EQU_Check_Items_Piece_VW");
            });

            modelBuilder.Entity<EquContract>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquContractInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquContractInstallmentVw>(entity =>
            {
                entity.ToView("EQU_Contract_Installment_VW");
            });

            modelBuilder.Entity<EquContractVw>(entity =>
            {
                entity.ToView("EQU_Contract_VW");
            });

            modelBuilder.Entity<EquEmployeesWorkshop>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquEmployeesWorkshopVw>(entity =>
            {
                entity.ToView("EQU_Employees_Workshop_VW");
            });

            modelBuilder.Entity<EquEquipmentNoteTypeVw>(entity =>
            {
                entity.ToView("EQU_Equipment_Note_Type_VW");
            });

            modelBuilder.Entity<EquFormsCheckItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquFormsCheckItemsVw>(entity =>
            {
                entity.ToView("EQU_Forms_Check_Items_VW");
            });

            modelBuilder.Entity<EquFreeMonth>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquFreeMonthsVw>(entity =>
            {
                entity.ToView("EQU_Free_Months_VW");
            });

            modelBuilder.Entity<EquGap>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquGapStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EquGapVw>(entity =>
            {
                entity.ToView("EQU_Gap_VW");
            });

            modelBuilder.Entity<EquGroupsForm>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquGroupsFormsVw>(entity =>
            {
                entity.ToView("EQU_Groups_Forms_VW");
            });

            modelBuilder.Entity<EquIncome>(entity =>
            {
                entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

                entity.Property(e => e.BankId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.DateGregorian).IsFixedLength();

                entity.Property(e => e.DateHijri).IsFixedLength();

                entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentTypeId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquIncomeVw>(entity =>
            {
                entity.ToView("EQU_Income_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.DateGregorian).IsFixedLength();

                entity.Property(e => e.DateHijri).IsFixedLength();
            });

            modelBuilder.Entity<EquPayInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquPreventiveMaintenance>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceForm>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceFormsVw>(entity =>
            {
                entity.ToView("EQU_Preventive_Maintenance_Forms_VW");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceGroup>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceGroups2Vw>(entity =>
            {
                entity.ToView("EQU_Preventive_Maintenance_Groups2_VW");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceGroupsVw>(entity =>
            {
                entity.ToView("EQU_Preventive_Maintenance_Groups_VW");
            });

            modelBuilder.Entity<EquPreventiveMaintenanceVw>(entity =>
            {
                entity.ToView("EQU_Preventive_Maintenance_VW");
            });

            modelBuilder.Entity<EquStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquSupplierIdWarranty>(entity =>
            {
                entity.ToView("EQU_Supplier_ID_warranty");
            });

            modelBuilder.Entity<EquTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<EquTypeVw>(entity =>
            {
                entity.ToView("EQU_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EquWarrantyUsageTermType>(entity =>
            {
                entity.ToView("EQU_Warranty_Usage_Term_Type");
            });

            modelBuilder.Entity<EquWorkOrder>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderCompletion>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderCost>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderCostVw>(entity =>
            {
                entity.ToView("EQU_Work_Order_Cost_VW");
            });

            modelBuilder.Entity<EquWorkOrderFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderPart>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderPartVw>(entity =>
            {
                entity.ToView("EQU_Work_Order_Part_VW");
            });

            modelBuilder.Entity<EquWorkOrderTask>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkOrderTaskVw>(entity =>
            {
                entity.ToView("EQU_Work_Order_Task_VW");
            });

            modelBuilder.Entity<EquWorkOrderVw>(entity =>
            {
                entity.ToView("EQU_Work_Order_VW");
            });

            modelBuilder.Entity<EquWorkshop>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<EquWorkshopTask>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FollowDistributionTransactionsSupervisor>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FollowDistributionTransactionsSupervisorsLocation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FollowDistributionTransactionsSupervisorsLocationVw>(entity =>
            {
                entity.ToView("Follow_Distribution_Transactions_Supervisors_Location_VW");
            });

            modelBuilder.Entity<FollowDistributionTransactionsSupervisorsVw>(entity =>
            {
                entity.ToView("Follow_Distribution_Transactions_Supervisors_VW");
            });

            modelBuilder.Entity<FollowLocationSupervisor>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FollowLocationSupervisorVw>(entity =>
            {
                entity.ToView("Follow_Location_Supervisor_VW");
            });

            modelBuilder.Entity<FollowTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FollowTransactionDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FollowViolation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FollowViolationType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FoodicsBanksVw>(entity =>
            {
                entity.ToView("FoodicsBanks_VW");
            });

            modelBuilder.Entity<FoodicsBranchesVw>(entity =>
            {
                entity.ToView("FoodicsBranches_VW");
            });

            modelBuilder.Entity<FoodicsClosersVw>(entity =>
            {
                entity.ToView("FoodicsClosers_VW");
            });

            modelBuilder.Entity<FoodicsJournalLog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<FoodicsOrderPaymentsVw>(entity =>
            {
                entity.ToView("Foodics_Order_Payments_VW");
            });

            modelBuilder.Entity<FoodicsOrdersVw>(entity =>
            {
                entity.ToView("Foodics_Orders_VW");
            });

            modelBuilder.Entity<FxaDepreciationMethod>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaFixedAsset>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaFixedAssetTransfer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<FxaFixedAssetTransferVw>(entity =>
            {
                entity.ToView("FXA_FixedAsset_Transfer_VW");
            });

            modelBuilder.Entity<FxaFixedAssetType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaFixedAssetTypeVw>(entity =>
            {
                entity.ToView("FXA_FixedAsset_Type_VW");
            });

            modelBuilder.Entity<FxaFixedAssetVw>(entity =>
            {
                entity.ToView("FXA_FixedAsset_VW");
            });

            modelBuilder.Entity<FxaTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaTransactionsAssest>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaTransactionsAssestVw>(entity =>
            {
                entity.ToView("FXA_Transactions_Assest_VW");
            });

            modelBuilder.Entity<FxaTransactionsProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaTransactionsProductsVw>(entity =>
            {
                entity.ToView("FXA_Transactions_Products_VW");
            });

            modelBuilder.Entity<FxaTransactionsRevaluation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<FxaTransactionsRevaluationVw>(entity =>
            {
                entity.ToView("FXA_Transactions_Revaluation_VW");
            });

            modelBuilder.Entity<FxaTransactionsVw>(entity =>
            {
                entity.ToView("FXA_Transactions_VW");
            });

            modelBuilder.Entity<HdTickect>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HdTickectPriorityVw>(entity =>
            {
                entity.ToView("HD_Tickect_Priority_VW");
            });

            modelBuilder.Entity<HdTickectQuickreplyVw>(entity =>
            {
                entity.ToView("HD_Tickect_Quickreply_VW");
            });

            modelBuilder.Entity<HdTickectStatusVw>(entity =>
            {
                entity.ToView("HD_Tickect_Status_VW");
            });

            modelBuilder.Entity<HdTickectsAssigin>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HdTickectsFile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HdTickectsReply>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HdTickectsReplyRpVw>(entity =>
            {
                entity.ToView("HD_Tickects_Reply_RP_VW");
            });

            modelBuilder.Entity<HdTickectsReplyVw>(entity =>
            {
                entity.ToView("HD_Tickects_Reply_VW");
            });

            modelBuilder.Entity<HdTickectsVw>(entity =>
            {
                entity.ToView("HD_Tickects_VW");
            });

            modelBuilder.Entity<HotFloor>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotFloorsVw>(entity =>
            {
                entity.ToView("HOT_Floors_VW");
            });

            modelBuilder.Entity<HotGroup>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotGroupsVw>(entity =>
            {
                entity.ToView("HOT_Groups_VW");
            });

            modelBuilder.Entity<HotRoom>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotRoomAsset>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotRoomAssetsVw>(entity =>
            {
                entity.ToView("HOT_Room_Assets_VW");
            });

            modelBuilder.Entity<HotRoomService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotRoomVw>(entity =>
            {
                entity.ToView("HOT_Room_VW");
            });

            modelBuilder.Entity<HotService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotServicesVw>(entity =>
            {
                entity.ToView("HOT_Services_VW");
            });

            modelBuilder.Entity<HotTransaction>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotTransactionsCompanion>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotTransactionsCompanionVw>(entity =>
            {
                entity.ToView("HOT_Transactions_Companion_VW");
            });

            modelBuilder.Entity<HotTransactionsPayment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<HotTransactionsRoom>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotTransactionsRoomVw>(entity =>
            {
                entity.ToView("HOT_Transactions_Room_VW");
            });

            modelBuilder.Entity<HotTransactionsService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HotTransactionsServicesVw>(entity =>
            {
                entity.ToView("HOT_Transactions_Services_VW");
            });

            modelBuilder.Entity<HotTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HotTransactionsVw>(entity =>
            {
                entity.ToView("HOT_Transactions_VW");
            });

            modelBuilder.Entity<HotTypeRoom>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrAbsence>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type).IsFixedLength();
            });

            modelBuilder.Entity<HrAbsenceVw>(entity =>
            {
                entity.ToView("HR_Absence_VW");

                entity.Property(e => e.Type).IsFixedLength();
            });

            modelBuilder.Entity<HrActualAttendance>(entity =>
            {
                entity.Property(e => e.Date).IsFixedLength();
            });

            modelBuilder.Entity<HrActualAttendanceVw>(entity =>
            {
                entity.ToView("HR_ActualAttendance_VW");

                entity.Property(e => e.Date).IsFixedLength();
            });

            modelBuilder.Entity<HrAllowanceDeduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PreparationSalariesId).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrAllowanceDeductionM>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PreparationSalariesId).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrAllowanceDeductionTempOrFix>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrAllowanceDeductionVw>(entity =>
            {
                entity.ToView("HR_Allowance_Deduction_VW");
            });

            modelBuilder.Entity<HrAllowanceVw>(entity =>
            {
                entity.ToView("HR_Allowance_VW");
            });

            modelBuilder.Entity<HrArchiveFilesDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrArchiveFilesDetailsVw>(entity =>
            {
                entity.ToView("HR_Archive_FilesDetails_VW");
            });

            modelBuilder.Entity<HrArchivesFile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrArchivesFilesVw>(entity =>
            {
                entity.ToView("HR_Archives_Files_VW");
            });

            modelBuilder.Entity<HrAssignman>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrAssignmenVw>(entity =>
            {
                entity.ToView("HR_Assignmen_VW");
            });

            modelBuilder.Entity<HrAttAction>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Time })
                    .HasName("USERCHECKTIME");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Time).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmpId).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsManual).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeId).HasDefaultValueSql("('I')");

                entity.Property(e => e.UserExtFmt).HasDefaultValueSql("((0))");

                entity.Property(e => e.WorkCode).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrAttCheckShiftEmployeeVw>(entity =>
            {
                entity.ToView("HR_Att_Check_Shift_Employee_VW");
            });

            modelBuilder.Entity<HrAttLocation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttLocationEmployee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttLocationEmployeeVw>(entity =>
            {
                entity.ToView("HR_Att_Location_Employee_VW");
            });

            modelBuilder.Entity<HrAttShift>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttShiftClose>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrAttShiftCloseD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttShiftCloseVw>(entity =>
            {
                entity.ToView("HR_Att_Shift_Close_Vw");
            });

            modelBuilder.Entity<HrAttShiftEmployee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttShiftEmployeeMVw>(entity =>
            {
                entity.ToView("HR_Att_Shift_EmployeeM_VW");
            });

            modelBuilder.Entity<HrAttShiftEmployeeVw>(entity =>
            {
                entity.ToView("HR_Att_Shift_Employee_VW");
            });

            modelBuilder.Entity<HrAttShiftTimeTable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttShiftTimeTableEmpVw>(entity =>
            {
                entity.ToView("HR_Att_Shift_TimeTable_Emp_VW");
            });

            modelBuilder.Entity<HrAttShiftTimeTableVw>(entity =>
            {
                entity.ToView("HR_Att_Shift_TimeTable_VW");
            });

            modelBuilder.Entity<HrAttTimeTable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExitOnNextDate).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrAttTimeTableDay>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAttTimeTableVw>(entity =>
            {
                entity.ToView("HR_Att_TimeTable_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrAttendance>(entity =>
            {
                entity.Property(e => e.AllowTimeIn).HasDefaultValueSql("((0))");

                entity.Property(e => e.AllowTimeOut).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrAttendanceSoftwareVw>(entity =>
            {
                entity.ToView("HR_Attendance_Software_VW");
            });

            modelBuilder.Entity<HrAttendanceTypeVw>(entity =>
            {
                entity.ToView("HR_Attendance_Type_VW");
            });

            modelBuilder.Entity<HrAttendancesVw>(entity =>
            {
                entity.ToView("HR_Attendances_VW");
            });

            modelBuilder.Entity<HrAttendancesVw2>(entity =>
            {
                entity.ToView("HR_Attendances_VW2");
            });

            modelBuilder.Entity<HrAuthorization>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrAuthorizationVw>(entity =>
            {
                entity.ToView("HR_Authorization_VW");
            });

            modelBuilder.Entity<HrCardTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCheckInOut>(entity =>
            {
                entity.Property(e => e.IsSend).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCheckInOutVw>(entity =>
            {
                entity.ToView("HR_CheckInOut_VW");
            });

            modelBuilder.Entity<HrClearance>(entity =>
            {
                entity.Property(e => e.BankId).IsFixedLength();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrClearanceMonth>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MsMonth).IsFixedLength();

                entity.Property(e => e.PayrollTypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrClearanceMonthsVw>(entity =>
            {
                entity.ToView("HR_Clearance_Months_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrClearanceType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrClearanceTypeVw>(entity =>
            {
                entity.ToView("HR_Clearance_Type_VW");
            });

            modelBuilder.Entity<HrClearanceVw>(entity =>
            {
                entity.ToView("HR_Clearance_VW");

                entity.Property(e => e.BankId).IsFixedLength();
            });

            modelBuilder.Entity<HrCompensatoryVacation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCompensatoryVacationsVw>(entity =>
            {
                entity.ToView("HR_Compensatory_Vacations_VW");
            });

            modelBuilder.Entity<HrCompetence>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCompetencesCatagory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCompetencesVw>(entity =>
            {
                entity.ToView("HR_Competences_VW");
            });

            modelBuilder.Entity<HrContracte>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrContractesVw>(entity =>
            {
                entity.ToView("HR_Contractes_VW");
            });

            modelBuilder.Entity<HrCostType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrCostTypeVw>(entity =>
            {
                entity.ToView("HR_Cost_Type_VW");
            });

            modelBuilder.Entity<HrCustody>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrCustodyItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrCustodyItemsProperty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrCustodyItemsVw>(entity =>
            {
                entity.ToView("HR_Custody_Items_VW");
            });

            modelBuilder.Entity<HrCustodyMDVw>(entity =>
            {
                entity.ToView("HR_Custody_M_D_VW");
            });

            modelBuilder.Entity<HrCustodyRefranceType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrCustodyType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrCustodyVw>(entity =>
            {
                entity.ToView("HR_Custody_VW");
            });

            modelBuilder.Entity<HrDecision>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrDecisionsTypeVw>(entity =>
            {
                entity.ToView("HR_Decisions_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrDecisionsVw>(entity =>
            {
                entity.ToView("HR_Decisions_VW");
            });

            modelBuilder.Entity<HrDeductionVw>(entity =>
            {
                entity.ToView("HR_Deduction_VW");
            });

            modelBuilder.Entity<HrDefinitionSalaryVw>(entity =>
            {
                entity.ToView("HR_Definition_Salary_VW");
            });

            modelBuilder.Entity<HrDelay>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrDelayVw>(entity =>
            {
                entity.ToView("HR_Delay_VW");
            });

            modelBuilder.Entity<HrDependent>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrDependentsVw>(entity =>
            {
                entity.ToView("HR_Dependents_VW");
            });

            modelBuilder.Entity<HrDirectJob>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrDirectJobVw>(entity =>
            {
                entity.ToView("HR_Direct_Job_VW");
            });

            modelBuilder.Entity<HrDisciplinaryActionType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrDisciplinaryCase>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrDisciplinaryCaseAction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrDisciplinaryCaseActionVw>(entity =>
            {
                entity.ToView("HR_Disciplinary_Case_Action_VW");
            });

            modelBuilder.Entity<HrDisciplinaryRule>(entity =>
            {
                entity.Property(e => e.DeductedLate).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrDisciplinaryRuleVw>(entity =>
            {
                entity.ToView("HR_Disciplinary_Rule_VW");
            });

            modelBuilder.Entity<HrDisciplinaryTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrEducation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrEducationVw>(entity =>
            {
                entity.ToView("HR_Education_VW");
            });

            modelBuilder.Entity<HrEmpLeaveVw>(entity =>
            {
                entity.ToView("HR_Emp_Leave_VW");
            });

            modelBuilder.Entity<HrEmpTicketVw>(entity =>
            {
                entity.ToView("HR_Emp_Ticket_VW");
            });

            modelBuilder.Entity<HrEmpVacationVw>(entity =>
            {
                entity.ToView("HR_Emp_Vacation_VW");
            });

            modelBuilder.Entity<HrEmpWarn>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrEmpWarnVw>(entity =>
            {
                entity.ToView("HR_Emp_Warn_VW");
            });

            modelBuilder.Entity<HrEmpWorkTime>(entity =>
            {
                entity.HasKey(e => e.EmpWorkId)
                    .HasName("PK_HR_Emp_Word_Time");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrEmpWorkTimeVw>(entity =>
            {
                entity.ToView("HR_Emp_Work_Time_VW");
            });

            modelBuilder.Entity<HrEmployee>(entity =>
            {
                entity.ToView("HR_Employee");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrEmployeeCost>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrEmployeeCostVw>(entity =>
            {
                entity.ToView("HR_Employee_Cost_VW");
            });

            modelBuilder.Entity<HrEmployeeCostcenter>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrEmployeeShiftReportVw>(entity =>
            {
                entity.ToView("HR_Employee_Shift_Report_VW");
            });

            modelBuilder.Entity<HrEmployeeVw>(entity =>
            {
                entity.ToView("HR_Employee_VW");
            });

            modelBuilder.Entity<HrEmployeesListVw>(entity =>
            {
                entity.ToView("HR_EmployeesList_VW");
            });

            modelBuilder.Entity<HrEvaluationAnnualIncreaseConfig>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrFile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrFlexibleWorking>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrFlexibleWorkingMaster>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrFlexibleWorkingVw>(entity =>
            {
                entity.ToView("HR_Flexible_Working_VW");
            });

            modelBuilder.Entity<HrGoal>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Sent).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrGoalsKpi>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGoalsKpiVw>(entity =>
            {
                entity.ToView("HR_Goals_KPI_VW");
            });

            modelBuilder.Entity<HrGoalsNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGoalsNoteVw>(entity =>
            {
                entity.ToView("HR_Goals_Note_VW");
            });

            modelBuilder.Entity<HrGoalsPlan>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGoalsPlansVw>(entity =>
            {
                entity.ToView("HR_Goals_Plans_VW");
            });

            modelBuilder.Entity<HrGoalsReply>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGoalsUpdate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGoalsUpdatesVw>(entity =>
            {
                entity.ToView("HR_Goals_Updates_VW");
            });

            modelBuilder.Entity<HrGoalsVw>(entity =>
            {
                entity.ToView("HR_Goals_VW");
            });

            modelBuilder.Entity<HrGosi>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGosiEmployee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrGosiEmployeeAccVw>(entity =>
            {
                entity.ToView("HR_GOSI_Employee_Acc_VW");
            });

            modelBuilder.Entity<HrGosiEmployeeVw>(entity =>
            {
                entity.ToView("HR_GOSI_Employee_VW");
            });

            modelBuilder.Entity<HrGosiTypeVw>(entity =>
            {
                entity.ToView("HR_Gosi_Type_VW");
            });

            modelBuilder.Entity<HrGosiVw>(entity =>
            {
                entity.ToView("HR_GOSI_VW");
            });

            modelBuilder.Entity<HrHoliday>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrHolidayVw>(entity =>
            {
                entity.ToView("HR_Holiday_VW");

                entity.Property(e => e.HolidayId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrIncrement>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrIncrementType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrIncrementsAllowanceDeduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrIncrementsAllowanceVw>(entity =>
            {
                entity.ToView("HR_Increments_Allowance_VW");
            });

            modelBuilder.Entity<HrIncrementsDeductionVw>(entity =>
            {
                entity.ToView("HR_Increments_Deduction_VW");
            });

            modelBuilder.Entity<HrIncrementsVw>(entity =>
            {
                entity.ToView("HR_Increments_VW");
            });

            modelBuilder.Entity<HrInsurance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrInsuranceCategoryVw>(entity =>
            {
                entity.ToView("HR_Insurance_Category_VW");
            });

            modelBuilder.Entity<HrInsuranceEmp>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrInsuranceEmpVw>(entity =>
            {
                entity.ToView("HR_Insurance_Emp_VW");
            });

            modelBuilder.Entity<HrInsurancePolicy>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrInsuranceVw>(entity =>
            {
                entity.ToView("HR_Insurance_VW");
            });

            modelBuilder.Entity<HrJob>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrJobCategoriesVw>(entity =>
            {
                entity.ToView("HR_Job_Categories_VW");
            });

            modelBuilder.Entity<HrJobDescription>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrJobEmployeeVw>(entity =>
            {
                entity.ToView("HR_Job_Employee_VW");
            });

            modelBuilder.Entity<HrJobGrade>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrJobGradeVw>(entity =>
            {
                entity.ToView("HR_Job_Grade_VW");
            });

            modelBuilder.Entity<HrJobLevel>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrJobLevelsAllowanceDeduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrJobOfferAdvantage>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrJobOfferLookupDataVw>(entity =>
            {
                entity.ToView("HR_Job_Offer_lookup_Data_VW");
            });

            modelBuilder.Entity<HrJobOfferVw>(entity =>
            {
                entity.ToView("HR_Job_Offer_VW");
            });

            modelBuilder.Entity<HrJobProgramVw>(entity =>
            {
                entity.ToView("HR_Job_Program_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrJobStatusVw>(entity =>
            {
                entity.ToView("HR_Job_Status_VW");
            });

            modelBuilder.Entity<HrJobTypeVw>(entity =>
            {
                entity.ToView("HR_Job_Type_VW");
            });

            modelBuilder.Entity<HrJobVw>(entity =>
            {
                entity.ToView("HR_Job_VW");
            });

            modelBuilder.Entity<HrKpi>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrKpiDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrKpiDetailesVw>(entity =>
            {
                entity.ToView("HR_KPI_Detailes_VW");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrKpiNote>(entity =>
            {
                entity.Property(e => e.CreateIn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrKpiNoteVw>(entity =>
            {
                entity.ToView("HR_KPI_Note_VW");
            });

            modelBuilder.Entity<HrKpiProjectManager>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrKpiProjectManagerDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrKpiProjectManagerDetailesVw>(entity =>
            {
                entity.ToView("HR_KPI_Project_Manager_Detailes_VW");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrKpiProjectManagerVw>(entity =>
            {
                entity.ToView("HR_KPI_Project_Manager_VW");
            });

            modelBuilder.Entity<HrKpiTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrKpiTemplatesCompetence>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrKpiTemplatesCompetencesSub>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrKpiTemplatesCompetencesVw>(entity =>
            {
                entity.ToView("HR_KPI_Templates_Competences_VW");
            });

            modelBuilder.Entity<HrKpiTemplatesVw>(entity =>
            {
                entity.ToView("HR_KPI_Templates_VW");
            });

            modelBuilder.Entity<HrKpiType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Isdeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrKpiVw>(entity =>
            {
                entity.ToView("HR_KPI_VW");
            });

            modelBuilder.Entity<HrLanguage>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLanguagesVw>(entity =>
            {
                entity.ToView("HR_Languages_VW");
            });

            modelBuilder.Entity<HrLeave>(entity =>
            {
                entity.Property(e => e.BankId).IsFixedLength();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrLeaveType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLeaveTypeVw>(entity =>
            {
                entity.ToView("HR_Leave_Type_VW");
            });

            modelBuilder.Entity<HrLeaveVw>(entity =>
            {
                entity.ToView("HR_Leave_VW");

                entity.Property(e => e.BankId).IsFixedLength();
            });

            modelBuilder.Entity<HrLicense>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLicensesVw>(entity =>
            {
                entity.ToView("HR_Licenses_VW");
            });

            modelBuilder.Entity<HrLoan>(entity =>
            {
                entity.Property(e => e.CreateInstallment).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Guarantor2EmpId).IsFixedLength();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLoanInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsPaid).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLoanInstallmentPayment>(entity =>
            {
                entity.Property(e => e.AmountPaid).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoanPaymentId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrLoanInstallmentPaymentVw>(entity =>
            {
                entity.ToView("HR_Loan_Installment_Payment_VW");
            });

            modelBuilder.Entity<HrLoanInstallmentVw>(entity =>
            {
                entity.ToView("HR_Loan_Installment_VW");
            });

            modelBuilder.Entity<HrLoanPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrLoanPaymentVw>(entity =>
            {
                entity.ToView("HR_Loan_Payment_VW");
            });

            modelBuilder.Entity<HrLoanVw>(entity =>
            {
                entity.ToView("HR_Loan_VW");
            });

            modelBuilder.Entity<HrLocationNoteTypeVw>(entity =>
            {
                entity.ToView("HR_Location_Note_Type_VW");
            });

            modelBuilder.Entity<HrMandate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrMandateVw>(entity =>
            {
                entity.ToView("HR_Mandate_VW");
            });

            modelBuilder.Entity<HrMaritalStatusVw>(entity =>
            {
                entity.ToView("HR_Marital_Status_VW");
            });

            modelBuilder.Entity<HrMembership>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNoteVw>(entity =>
            {
                entity.ToView("HR_Note_VW");
            });

            modelBuilder.Entity<HrNotification>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNotificationsReply>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNotificationsSetting>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNotificationsSettingVw>(entity =>
            {
                entity.ToView("HR_Notifications_Setting_VW");
            });

            modelBuilder.Entity<HrNotificationsType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrNotificationsTypeVw>(entity =>
            {
                entity.ToView("HR_Notifications_Type_VW");
            });

            modelBuilder.Entity<HrNotificationsVw>(entity =>
            {
                entity.ToView("HR_Notifications_VW");
            });

            modelBuilder.Entity<HrOhad>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrOhadDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrOhadDetailsVw>(entity =>
            {
                entity.ToView("HR_OhadDetails_VW");
            });

            modelBuilder.Entity<HrOhadStatusVw>(entity =>
            {
                entity.ToView("HR_Ohad_Status_VW");
            });

            modelBuilder.Entity<HrOhadTransactionTypeVw>(entity =>
            {
                entity.ToView("HR_Ohad_Transaction_Type_VW");
            });

            modelBuilder.Entity<HrOhadVw>(entity =>
            {
                entity.ToView("HR_Ohad_VW");
            });

            modelBuilder.Entity<HrOpeningBalance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrOpeningBalanceType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrOpeningBalanceVw>(entity =>
            {
                entity.ToView("HR_Opening_Balance_VW");
            });

            modelBuilder.Entity<HrOverTimeD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrOverTimeD2Vw>(entity =>
            {
                entity.ToView("HR_OverTime_D2_VW");
            });

            modelBuilder.Entity<HrOverTimeDExportVw>(entity =>
            {
                entity.ToView("HR_OverTime_D_Export_VW");
            });

            modelBuilder.Entity<HrOverTimeDVw>(entity =>
            {
                entity.ToView("HR_OverTime_D_VW");
            });

            modelBuilder.Entity<HrOverTimeM>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrOverTimeMVw>(entity =>
            {
                entity.ToView("HR_OverTime_M_VW");
            });

            modelBuilder.Entity<HrPayroll>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MsMonth).IsFixedLength();

                entity.Property(e => e.PayrollTypeId).HasDefaultValueSql("((1))");

                entity.Property(e => e.State).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrPayrollAllowanceAccountsVw>(entity =>
            {
                entity.ToView("HR_Payroll_Allowance_Accounts_VW");
            });

            modelBuilder.Entity<HrPayrollAllowanceDeduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollAllowanceDeductionVw>(entity =>
            {
                entity.ToView("HR_Payroll_allowance_Deduction_VW");
            });

            modelBuilder.Entity<HrPayrollAllowanceVw>(entity =>
            {
                entity.ToView("HR_Payroll_Allowance_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPayrollCostcenter>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollCostcenterVw>(entity =>
            {
                entity.ToView("HR_Payroll_Costcenter_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPayrollD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollDAllowanceVw>(entity =>
            {
                entity.ToView("HR_Payroll_D_Allowance_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPayrollDPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollDPaymentVw>(entity =>
            {
                entity.ToView("HR_Payroll_D_Payment_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<HrPayrollDVw>(entity =>
            {
                entity.ToView("HR_Payroll_D_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPayrollDeductionAccountsVw>(entity =>
            {
                entity.ToView("HR_Payroll_Deduction_Accounts_VW");
            });

            modelBuilder.Entity<HrPayrollDeductionVw>(entity =>
            {
                entity.ToView("HR_Payroll_Deduction_VW");
            });

            modelBuilder.Entity<HrPayrollNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollNoteVw>(entity =>
            {
                entity.ToView("HR_Payroll_Note_VW");
            });

            modelBuilder.Entity<HrPayrollPaymentTypeVw>(entity =>
            {
                entity.ToView("HR_Payroll_Payment_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrPayrollStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrPayrollType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPayrollVw>(entity =>
            {
                entity.ToView("HR_Payroll_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPerformance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPerformanceForVw>(entity =>
            {
                entity.ToView("HR_Performance_For_VW");
            });

            modelBuilder.Entity<HrPerformanceStatusVw>(entity =>
            {
                entity.ToView("HR_Performance_Status_VW");
            });

            modelBuilder.Entity<HrPerformanceTypeVw>(entity =>
            {
                entity.ToView("HR_Performance_Type_VW");
            });

            modelBuilder.Entity<HrPerformanceVw>(entity =>
            {
                entity.ToView("HR_Performance_VW");
            });

            modelBuilder.Entity<HrPermission>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPermissionReasonVw>(entity =>
            {
                entity.ToView("HR_Permission_Reason_VW");
            });

            modelBuilder.Entity<HrPermissionTypeVw>(entity =>
            {
                entity.ToView("HR_Permission_Type_VW");
            });

            modelBuilder.Entity<HrPermissionsVw>(entity =>
            {
                entity.ToView("HR_Permissions_VW");
            });

            modelBuilder.Entity<HrPoliciesType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_HR_Policies_Type_1");

                entity.Property(e => e.TypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrPoliciesVw>(entity =>
            {
                entity.ToView("HR_Policies_VW");
            });

            modelBuilder.Entity<HrPolicy>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPreparationSalariesVw>(entity =>
            {
                entity.ToView("HR_Preparation_Salaries_VW");

                entity.Property(e => e.MsMonth).IsFixedLength();
            });

            modelBuilder.Entity<HrPreparationSalary>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MsMonth).IsFixedLength();

                entity.Property(e => e.PayrollTypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrProvision>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrProvisionsEmployee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrProvisionsEmployeeAccVw>(entity =>
            {
                entity.ToView("HR_Provisions_Employee_Acc_VW");
            });

            modelBuilder.Entity<HrProvisionsEmployeeVw>(entity =>
            {
                entity.ToView("HR_Provisions_Employee_VW");
            });

            modelBuilder.Entity<HrProvisionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrProvisionsVw>(entity =>
            {
                entity.ToView("HR_Provisions_VW");
            });

            modelBuilder.Entity<HrPsAllowanceDeduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrPsAllowanceVw>(entity =>
            {
                entity.ToView("HR_PS_Allowance_VW");
            });

            modelBuilder.Entity<HrPsDeductionVw>(entity =>
            {
                entity.ToView("HR_PS_Deduction_VW");
            });

            modelBuilder.Entity<HrQualificationVw>(entity =>
            {
                entity.ToView("HR_Qualification_VW");
            });

            modelBuilder.Entity<HrRateType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrRateTypeVw>(entity =>
            {
                entity.ToView("HR_Rate_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrRecruitmentAllowanceVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Allowance_VW");
            });

            modelBuilder.Entity<HrRecruitmentApplicantKpiDVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Applicant_KPI_D_VW");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrRecruitmentApplicantKpiVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Applicant_KPI_VW");
            });

            modelBuilder.Entity<HrRecruitmentApplication>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrRecruitmentApplicationVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Application_VW");
            });

            modelBuilder.Entity<HrRecruitmentCandidate>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRecruitmentCandidateApplicationVw>(entity =>
            {
                entity.ToView("HR_Recruitment_CandidateApplication_VW");
            });

            modelBuilder.Entity<HrRecruitmentCandidateKpi>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrRecruitmentCandidateKpiD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrRecruitmentCandidateKpiDVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Candidate_KPI_D_VW");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<HrRecruitmentCandidateKpiVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Candidate_KPI_VW");
            });

            modelBuilder.Entity<HrRecruitmentCandidateVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Candidate_VW");
            });

            modelBuilder.Entity<HrRecruitmentDeductionVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Deduction_VW");
            });

            modelBuilder.Entity<HrRecruitmentEvaluationMember>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FinalDegree).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsEvaluation).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRecruitmentInterview>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrRecruitmentInterviewVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Interview_VW");
            });

            modelBuilder.Entity<HrRecruitmentVacancy>(entity =>
            {
                entity.Property(e => e.DeptId).HasDefaultValueSql("((0))");

                entity.Property(e => e.FacilityId).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LocationId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRecruitmentVacancyVw>(entity =>
            {
                entity.ToView("HR_Recruitment_Vacancy_VW");
            });

            modelBuilder.Entity<HrRequest>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRequestDetaile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRequestDetailesVw>(entity =>
            {
                entity.ToView("HR_Request_Detailes_VW");
            });

            modelBuilder.Entity<HrRequestType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrRequestVw>(entity =>
            {
                entity.ToView("HR_Request_VW");
            });

            modelBuilder.Entity<HrSalaryGroup>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrSalaryGroupAccount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrSalaryGroupAllowanceVw>(entity =>
            {
                entity.ToView("HR_Salary_Group_Allowance_VW");
            });

            modelBuilder.Entity<HrSalaryGroupDeductionVw>(entity =>
            {
                entity.ToView("HR_Salary_Group_Deduction_VW");
            });

            modelBuilder.Entity<HrSalaryGroupRefrance>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrSalaryGroupRefranceVw>(entity =>
            {
                entity.ToView("HR_Salary_Group_Refrance_VW");
            });

            modelBuilder.Entity<HrSalaryGroupVw>(entity =>
            {
                entity.ToView("HR_Salary_Group_VW");
            });

            modelBuilder.Entity<HrSetting>(entity =>
            {
                entity.Property(e => e.MonthEndDay).IsFixedLength();

                entity.Property(e => e.MonthStartDay).IsFixedLength();
            });

            modelBuilder.Entity<HrSkill>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrSkillsVw>(entity =>
            {
                entity.ToView("HR_Skills_VW");
            });

            modelBuilder.Entity<HrSpecializationVw>(entity =>
            {
                entity.ToView("HR_Specialization_VW");
            });

            modelBuilder.Entity<HrSponserVw>(entity =>
            {
                entity.ToView("HR_Sponser_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrSupportHrdf>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrTicket>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrTicketVw>(entity =>
            {
                entity.ToView("HR_Ticket_VW");
            });

            modelBuilder.Entity<HrTrainingBagVw>(entity =>
            {
                entity.ToView("HR_Training_bag_VW");
            });

            modelBuilder.Entity<HrTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrTransactionType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<HrTransactionVw>(entity =>
            {
                entity.ToView("HR_Transaction_VW");
            });

            modelBuilder.Entity<HrTransfer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrTransfersVw>(entity =>
            {
                entity.ToView("HR_Transfers_VW");
            });

            modelBuilder.Entity<HrUpgradeVw>(entity =>
            {
                entity.ToView("HR_Upgrade_VW");
            });

            modelBuilder.Entity<HrVacancyStatusVw>(entity =>
            {
                entity.ToView("HR_Vacancy_Status_VW");
            });

            modelBuilder.Entity<HrVacation>(entity =>
            {
                entity.Property(e => e.Approve).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HrVdtId).HasComment("نوع القرار");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSalary).HasDefaultValueSql("((0))");

                entity.Property(e => e.NeedJoinRequest).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationBalance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isacitve).HasDefaultValueSql("((1))");

                entity.Property(e => e.VacationTypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<HrVacationBalanceVw>(entity =>
            {
                entity.ToView("HR_VacationBalance_VW");
            });

            modelBuilder.Entity<HrVacationDue>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationRequest>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSalary).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationRequestVw>(entity =>
            {
                entity.ToView("HR_VacationRequest_VW");
            });

            modelBuilder.Entity<HrVacationSetting>(entity =>
            {
                entity.Property(e => e.VacSettingId).ValueGeneratedNever();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationsCatagory>(entity =>
            {
                entity.Property(e => e.CatId).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationsNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVacationsType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrVacationsTypeVw>(entity =>
            {
                entity.ToView("HR_Vacations_Type_VW");

                entity.Property(e => e.VacationTypeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrVacationsVw>(entity =>
            {
                entity.ToView("HR_Vacations_VW");
            });

            modelBuilder.Entity<HrVisa>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrVisaVw>(entity =>
            {
                entity.ToView("HR_Visa_VW");
            });

            modelBuilder.Entity<HrVisitLocationAttendance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrVisitLocationAttendanceResource>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<HrVisitLocationAttendanceResourcesVw>(entity =>
            {
                entity.ToView("HR_Visit_Location_Attendance_Resources_VW");
            });

            modelBuilder.Entity<HrVisitLocationAttendanceVw>(entity =>
            {
                entity.ToView("HR_Visit_Location_Attendance_VW");
            });

            modelBuilder.Entity<HrVisitLocationNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HrVisitLocationNoteVw>(entity =>
            {
                entity.ToView("HR_Visit_Location_Note_VW");
            });

            modelBuilder.Entity<HrVisitSchedule>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVisitScheduleLocation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVisitScheduleLocationStep>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrVisitScheduleLocationStepsVw>(entity =>
            {
                entity.ToView("HR_Visit_Schedule_Location_Steps_VW");
            });

            modelBuilder.Entity<HrVisitScheduleLocationVw>(entity =>
            {
                entity.ToView("HR_Visit_Schedule_Location_VW");
            });

            modelBuilder.Entity<HrVisitStatusVw>(entity =>
            {
                entity.ToView("HR_Visit_Status_VW");
            });

            modelBuilder.Entity<HrVisitStep>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<HrWagesProtectionVw>(entity =>
            {
                entity.ToView("HR_Wages_Protection_VW");
            });

            modelBuilder.Entity<HrWeekend>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<HrWeekendVw>(entity =>
            {
                entity.ToView("HR_Weekend_VW");
            });

            modelBuilder.Entity<HrWorkExperience>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<IntegraField>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<IntegraPropertiesVw>(entity =>
            {
                entity.ToView("Integra_Properties_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<IntegraProperty>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<IntegraPropertyValuesVw>(entity =>
            {
                entity.ToView("Integra_Property_Values_VW");
            });

            modelBuilder.Entity<IntegraSystem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<IntegraTable>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<InvestBranch>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<InvestBranchVw>(entity =>
            {
                entity.ToView("INVEST_BRANCH_VW");
            });

            modelBuilder.Entity<InvestEmployee>(entity =>
            {
                entity.Property(e => e.DeptId).HasDefaultValueSql("((0))");

                entity.Property(e => e.DirectDeposit).HasDefaultValueSql("((0))");

                entity.Property(e => e.ExcludeAttend).HasDefaultValueSql("((0))");

                entity.Property(e => e.HaveBankLoan).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSub).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");

                entity.Property(e => e.Location).HasDefaultValueSql("((0))");

                entity.Property(e => e.ParentId).HasDefaultValueSql("((0))");

                entity.Property(e => e.StopSalary).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<InvestEmployeeVvw>(entity =>
            {
                entity.ToView("INVEST_Employee_VVW");
            });

            modelBuilder.Entity<LgxmgAppSubscription>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgAppSubscriptionsHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgAppSubscriptionsHistoryVw>(entity =>
            {
                entity.ToView("LGXMG_App_Subscriptions_History_VW");
            });

            modelBuilder.Entity<LgxmgAppSubscriptionsVw>(entity =>
            {
                entity.ToView("LGXMG_App_Subscriptions_VW");
            });

            modelBuilder.Entity<LgxmgCompany>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgCustomersVw>(entity =>
            {
                entity.ToView("LGXMG_Customers_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<LgxmgDomain>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgDomainFacilitiesVw>(entity =>
            {
                entity.ToView("LGXMG_Domain_Facilities_VW");
            });

            modelBuilder.Entity<LgxmgDomainFacility>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgDomainUpdate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgDomainUpdatesVw>(entity =>
            {
                entity.ToView("LGXMG_Domain_Updates_VW");
            });

            modelBuilder.Entity<LgxmgDomainsHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgDomainsVw>(entity =>
            {
                entity.ToView("LGXMG_Domains_VW");
            });

            modelBuilder.Entity<LgxmgLocation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgPleskSubscription>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgPleskSubscriptionsHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgServer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgServersHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgServersVw>(entity =>
            {
                entity.ToView("LGXMG_Servers_VW");
            });

            modelBuilder.Entity<LgxmgSslCertificate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgSslCertificatesHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgSslCertificatesVw>(entity =>
            {
                entity.ToView("LGXMG_SSL_Certificates_VW");
            });

            modelBuilder.Entity<LgxmgSupportSubscription>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgSupportSubscriptionsHistory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LgxmgSupportSubscriptionsVw>(entity =>
            {
                entity.ToView("LGXMG_Support_Subscriptions_VW");
            });

            modelBuilder.Entity<LgxmgUpdate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<LookupDataServiceTypeVw>(entity =>
            {
                entity.ToView("Lookup_Data_ServiceType_VW");
            });

            modelBuilder.Entity<MaintChangeStatusComment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintChangeStatusCommentsVw>(entity =>
            {
                entity.ToView("Maint_ChangeStatus_Comments_VW");
            });

            modelBuilder.Entity<MaintContract>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintContractInstallment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintContractInstallmentPaymentVw>(entity =>
            {
                entity.ToView("Maint_Contract_Installment_Payment_VW");
            });

            modelBuilder.Entity<MaintContractInstallmentVw>(entity =>
            {
                entity.ToView("Maint_Contract_Installment_VW");
            });

            modelBuilder.Entity<MaintContractRenew>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintContractVw>(entity =>
            {
                entity.ToView("Maint_Contract_VW");
            });

            modelBuilder.Entity<MaintMaintenanceAssetsVw>(entity =>
            {
                entity.ToView("Maint_Maintenance_Assets_VW");
            });

            modelBuilder.Entity<MaintRequest>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintRequestsAsset>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintRequestsAssetsVw>(entity =>
            {
                entity.ToView("Maint_Requests_Assets_VW");
            });

            modelBuilder.Entity<MaintRequestsChangeStatusVw>(entity =>
            {
                entity.ToView("Maint_Requests_ChangeStatus_VW");
            });

            modelBuilder.Entity<MaintRequestsItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintRequestsItemsVw>(entity =>
            {
                entity.ToView("Maint_Requests_Items_VW");
            });

            modelBuilder.Entity<MaintRequestsVw>(entity =>
            {
                entity.ToView("Maint_Requests_VW");
            });

            modelBuilder.Entity<MaintTicket>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.HasIndex(e => new { e.SerialId, e.SerialIdNew }, "ClusteredIndex-20190421-211331")
                    .IsClustered();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsComment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsDamageCustomer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsDevice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FacilityId).HasDefaultValueSql("((1))");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsPart>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsPartsVw>(entity =>
            {
                entity.ToView("Maint_Tickets_Parts_VW");
            });

            modelBuilder.Entity<MaintTicketsReferral>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsRead).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsReferralsVw>(entity =>
            {
                entity.ToView("Maint_Tickets_Referrals_VW");
            });

            modelBuilder.Entity<MaintTicketsRepaier>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTicketsRepaierVw>(entity =>
            {
                entity.ToView("Maint_Tickets_Repaier_VW");
            });

            modelBuilder.Entity<MaintTicketsVw>(entity =>
            {
                entity.ToView("Maint_Tickets_VW");
            });

            modelBuilder.Entity<MaintTransaction>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MaintTransactionsInstallmentsVw>(entity =>
            {
                entity.ToView("Maint_Transactions_Installments_VW");
            });

            modelBuilder.Entity<MaintTransactionsVw>(entity =>
            {
                entity.ToView("Maint_Transactions_VW");
            });

            modelBuilder.Entity<MaintTypeDamageSupVw>(entity =>
            {
                entity.ToView("Maint_Type_Damage_Sup_VW");
            });

            modelBuilder.Entity<MaintTypeDamageVw>(entity =>
            {
                entity.ToView("Maint_Type_Damage_VW");
            });

            modelBuilder.Entity<MaintTypeVw>(entity =>
            {
                entity.ToView("Maint_Type_VW");
            });

            modelBuilder.Entity<MaintVisit>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.VisitDate).IsFixedLength();
            });

            modelBuilder.Entity<MaintVisitVw>(entity =>
            {
                entity.ToView("Maint_Visit_VW");

                entity.Property(e => e.VisitDate).IsFixedLength();
            });

            modelBuilder.Entity<MrpBillOfMaterial>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.RefranceId).HasComment("رقم المرجع لعرض الاسعار او اوامر البيع");
            });

            modelBuilder.Entity<MrpBillOfMaterialsComponent>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpBillOfMaterialsComponentsVw>(entity =>
            {
                entity.ToView("MRP_Bill_Of_Materials_Components_VW");
            });

            modelBuilder.Entity<MrpBillOfMaterialsExpense>(entity =>
            {
                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpBillOfMaterialsExpensesVw>(entity =>
            {
                entity.ToView("MRP_Bill_Of_Materials_Expenses_VW");
            });

            modelBuilder.Entity<MrpBillOfMaterialsListVw>(entity =>
            {
                entity.ToView("MRP_Bill_Of_Materials_List_VW");
            });

            modelBuilder.Entity<MrpBillOfMaterialsVw>(entity =>
            {
                entity.ToView("MRP_Bill_Of_Materials_VW");
            });

            modelBuilder.Entity<MrpExpense>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpExpensesList>(entity =>
            {
                entity.ToView("MRP_Expenses_List");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<MrpExpensesVw>(entity =>
            {
                entity.ToView("MRP_Expenses_VW");
            });

            modelBuilder.Entity<MrpManufacturingEqu>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingEquVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_EQU_VW");
            });

            modelBuilder.Entity<MrpManufacturingExpense>(entity =>
            {
                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingExpensesVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Expenses_VW");
            });

            modelBuilder.Entity<MrpManufacturingOrder>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingOrderCost>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingOrderCostVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Order_Cost_VW");
            });

            modelBuilder.Entity<MrpManufacturingOrderProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingOrderProductVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Order_Product_VW");
            });

            modelBuilder.Entity<MrpManufacturingOrderVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Order_VW");
            });

            modelBuilder.Entity<MrpManufacturingResource>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingResourcesVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Resources_VW");
            });

            modelBuilder.Entity<MrpManufacturingStaff>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingStaffVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Staff_VW");
            });

            modelBuilder.Entity<MrpManufacturingStep>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingStepsDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingStepsDetailesVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Steps_Detailes_VW");
            });

            modelBuilder.Entity<MrpManufacturingStepsVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Steps_VW");
            });

            modelBuilder.Entity<MrpManufacturingStoppingResuming>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpManufacturingStoppingResumingVw>(entity =>
            {
                entity.ToView("MRP_Manufacturing_Stopping_Resuming_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<MrpProduction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpProductionsDamaged>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpProductionsDamagedItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpProductionsDamagedItemsVw>(entity =>
            {
                entity.ToView("MRP_Productions_Damaged_Items_VW");
            });

            modelBuilder.Entity<MrpProductionsDamagedVw>(entity =>
            {
                entity.ToView("MRP_Productions_Damaged_VW");
            });

            modelBuilder.Entity<MrpProductionsReceiving>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MrpProductionsReceivingVw>(entity =>
            {
                entity.ToView("MRP_Productions_Receiving_VW");
            });

            modelBuilder.Entity<MrpProductionsVw>(entity =>
            {
                entity.ToView("MRP_Productions_VW");
            });

            modelBuilder.Entity<MrpStep>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<MuqeemTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<MuqeemTransactionsVw>(entity =>
            {
                entity.ToView("Muqeem_Transactions_VW");
            });

            modelBuilder.Entity<PmBudgetType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<PmChangeRequest>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmChangeRequestVw>(entity =>
            {
                entity.ToView("PM_ChangeRequest_VW");
            });

            modelBuilder.Entity<PmCrewStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmCrewStaffVw>(entity =>
            {
                entity.ToView("PM_Crew_Staff_VW");
            });

            modelBuilder.Entity<PmDeliverableTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).IsFixedLength();
            });

            modelBuilder.Entity<PmDeliverableTransactionsDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmDeliverableTransactionsDetailsVw>(entity =>
            {
                entity.ToView("PM_Deliverable_Transactions_Details_VW");
            });

            modelBuilder.Entity<PmDeliverableTransactionsVw>(entity =>
            {
                entity.ToView("PM_Deliverable_Transactions_VW");

                entity.Property(e => e.Note).IsFixedLength();
            });

            modelBuilder.Entity<PmDurationTypeVw>(entity =>
            {
                entity.ToView("PM_Duration_Type_VW");
            });

            modelBuilder.Entity<PmDynamicAttribute>(entity =>
            {
                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmDynamicAttributesTable>(entity =>
            {
                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmDynamicValue>(entity =>
            {
                entity.HasKey(e => e.DynamicValueId)
                    .HasName("PK_PM_DynamicValues_1");

                entity.Property(e => e.DynamicValueId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<PmExtractAdditionalType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractAdditionalTypeListVw>(entity =>
            {
                entity.ToView("PM_Extract_Additional_Type_list_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmExtractAdditionalTypeVw>(entity =>
            {
                entity.ToView("PM_Extract_Additional_Type_VW");
            });

            modelBuilder.Entity<PmExtractPaymentType>(entity =>
            {
                entity.ToView("PM_Extract_Payment_Type");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmExtractTemporary>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractTemporaryProduct>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractTemporarysVw>(entity =>
            {
                entity.ToView("PM_Extract_Temporarys_VW");
            });

            modelBuilder.Entity<PmExtractTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.RefranceId).HasComment("رقم المرجع لعرض الاسعار او اوامر البيع");
            });

            modelBuilder.Entity<PmExtractTransactionsAdditional>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractTransactionsAdditionalVw>(entity =>
            {
                entity.ToView("PM_Extract_Transactions_Additional_VW");
            });

            modelBuilder.Entity<PmExtractTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractTransactionsDiscountVw>(entity =>
            {
                entity.ToView("PM_Extract_Transactions_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<PmExtractTransactionsPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<PmExtractTransactionsProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmExtractTransactionsProductsVw>(entity =>
            {
                entity.ToView("PM_Extract_Transactions_Products_VW");
            });

            modelBuilder.Entity<PmExtractTransactionsVw>(entity =>
            {
                entity.ToView("PM_Extract_Transactions_VW");
            });

            modelBuilder.Entity<PmGroup>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmGroupStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmGroupStaffVw>(entity =>
            {
                entity.ToView("PM_Group_Staff_VW");
            });

            modelBuilder.Entity<PmGroupVw>(entity =>
            {
                entity.ToView("PM_Group_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmHumanRight>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmInvestigationRequest>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmInvestigationRequestVw>(entity =>
            {
                entity.ToView("PM_InvestigationRequest_VW");
            });

            modelBuilder.Entity<PmInvestigationStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmJobRole>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmJobRolesVw>(entity =>
            {
                entity.ToView("PM_Job_Roles_VW");
            });

            modelBuilder.Entity<PmJobsSalary>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PmJobsSalaryVw>(entity =>
            {
                entity.ToView("PM_Jobs_Salary_VW");
            });

            modelBuilder.Entity<PmJudicialAuthorityVw>(entity =>
            {
                entity.ToView("PM_Judicial_Authority_VW");
            });

            modelBuilder.Entity<PmKickOff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmKickOffVw>(entity =>
            {
                entity.ToView("PM_Kick_Off_VW");
            });

            modelBuilder.Entity<PmLookUpCatagory>(entity =>
            {
                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmLookUpDatum>(entity =>
            {
                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmMemosPreparation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmMemosPreparationVw>(entity =>
            {
                entity.ToView("PM_Memos_Preparation_VW");
            });

            modelBuilder.Entity<PmOperationalControl>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PmOperationalControlsVw>(entity =>
            {
                entity.ToView("PM_Operational_Controls_VW");
            });

            modelBuilder.Entity<PmProject>(entity =>
            {
                entity.Property(e => e.DeductionValue).HasDefaultValueSql("((0))");

                entity.Property(e => e.FacilityId).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.StepId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<PmProjectAccount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectEntry>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmProjectEntryVw>(entity =>
            {
                entity.ToView("PM_Project_Entry_VW");
            });

            modelBuilder.Entity<PmProjectEscalation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectEscalationVw>(entity =>
            {
                entity.ToView("PM_ProjectEscalation_VW");
            });

            modelBuilder.Entity<PmProjectKpiD>(entity =>
            {
                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<PmProjectKpiDVw>(entity =>
            {
                entity.ToView("PM_Project_KPI_D_VW");

                entity.Property(e => e.KpiTemComId).IsFixedLength();
            });

            modelBuilder.Entity<PmProjectKpiVw>(entity =>
            {
                entity.ToView("PM_Project_KPI_VW");
            });

            modelBuilder.Entity<PmProjectPlan>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectPlansVw>(entity =>
            {
                entity.ToView("PM_Project_Plans_VW");
            });

            modelBuilder.Entity<PmProjectReactive>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmProjectReactiveVw>(entity =>
            {
                entity.ToView("PM_Project_Reactive_VW");
            });

            modelBuilder.Entity<PmProjectResource>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectResourcesRole>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectResourcesRolesVw>(entity =>
            {
                entity.ToView("PM_Project_Resources_Roles_VW");
            });

            modelBuilder.Entity<PmProjectResourcesVw>(entity =>
            {
                entity.ToView("PM_Project_Resources_VW");
            });

            modelBuilder.Entity<PmProjectStakeholderTypeVw>(entity =>
            {
                entity.ToView("PM_Project_Stakeholder_Type_VW");
            });

            modelBuilder.Entity<PmProjectStatusVw>(entity =>
            {
                entity.ToView("PM_Project_Status_VW");
            });

            modelBuilder.Entity<PmProjectStepsVw>(entity =>
            {
                entity.ToView("PM_Project_Steps_VW");
            });

            modelBuilder.Entity<PmProjectsAddDeduc>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsAddDeducVw>(entity =>
            {
                entity.ToView("PM_Projects_Add_Deduc_VW");
            });

            modelBuilder.Entity<PmProjectsAppVw>(entity =>
            {
                entity.ToView("PM_Projects_App_VW");
            });

            modelBuilder.Entity<PmProjectsAssumption>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsBudget>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsBudgetItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsBudgetItemsVw>(entity =>
            {
                entity.ToView("PM_Projects_Budget_Items_VW");
            });

            modelBuilder.Entity<PmProjectsBudgetType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsBudgetTypeVw>(entity =>
            {
                entity.ToView("PM_Projects_Budget_Type_VW");
            });

            modelBuilder.Entity<PmProjectsBudgetVw>(entity =>
            {
                entity.ToView("PM_Projects_Budget_VW");
            });

            modelBuilder.Entity<PmProjectsClosing>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsClosingVw>(entity =>
            {
                entity.ToView("PM_Projects_Closing_VW");
            });

            modelBuilder.Entity<PmProjectsDeliverable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsDeliverableAcceptCriteriaVw>(entity =>
            {
                entity.ToView("PM_Projects_Deliverable_Accept_Criteria_VW");
            });

            modelBuilder.Entity<PmProjectsDeliverableAcceptCriterion>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsDeliverableVw>(entity =>
            {
                entity.ToView("PM_Projects_Deliverable_VW");
            });

            modelBuilder.Entity<PmProjectsEditVw>(entity =>
            {
                entity.ToView("PM_Projects_EDIT_VW");
            });

            modelBuilder.Entity<PmProjectsExternalStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsExternalStaffVw>(entity =>
            {
                entity.ToView("PM_Projects_ExternalStaff_VW");
            });

            modelBuilder.Entity<PmProjectsFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TaskId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsFilesVw>(entity =>
            {
                entity.ToView("PM_Projects_Files_VW");
            });

            modelBuilder.Entity<PmProjectsFinancialCost>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsFinancialCostsVw>(entity =>
            {
                entity.ToView("PM_Projects_FinancialCosts_VW");
            });

            modelBuilder.Entity<PmProjectsGovernance>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsGuarantee>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsGuaranteeVw>(entity =>
            {
                entity.ToView("PM_Projects_Guarantee_VW");
            });

            modelBuilder.Entity<PmProjectsInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsInstallmentPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmProjectsInstallmentPaymentVw>(entity =>
            {
                entity.ToView("PM_Projects_Installment_Payment_VW");
            });

            modelBuilder.Entity<PmProjectsInstallmentVw>(entity =>
            {
                entity.ToView("PM_Projects_Installment_VW");
            });

            modelBuilder.Entity<PmProjectsInterconnection>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsInterconnectionVw>(entity =>
            {
                entity.ToView("PM_Projects_Interconnection_VW");
            });

            modelBuilder.Entity<PmProjectsItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsItemsVw>(entity =>
            {
                entity.ToView("PM_Projects_Items_VW");
            });

            modelBuilder.Entity<PmProjectsKpiVw>(entity =>
            {
                entity.ToView("PM_Projects_KPI_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsFollowApplyTypeVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Follow_Apply_Type_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsImpactTypeVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Impact_Type_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsLearnedDetail>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsLessonsLearnedDetailsVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Learned_Details_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsLearnedMaster>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsLessonsLearnedMasterVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Learned_Master_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsLessonLearnedCatsVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Lesson_Learned_Cats_VW");
            });

            modelBuilder.Entity<PmProjectsLessonsProcedureTypeVw>(entity =>
            {
                entity.ToView("PM_Projects_Lessons_Procedure_Type_VW");
            });

            modelBuilder.Entity<PmProjectsListVw>(entity =>
            {
                entity.ToView("PM_Projects_list_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmProjectsMonthlyReport>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportAllProject>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportAllProjectsVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReport_All_Projects_VW");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportClosed>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportClosedVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReport_Closed_VW");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportDelayed>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportDelayedVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReport_Delayed_VW");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportRecommandation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportRisk>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportRisksVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReport_Risks_VW");

                entity.Property(e => e.EffectValue).IsFixedLength();

                entity.Property(e => e.ImpactValue).IsFixedLength();
            });

            modelBuilder.Entity<PmProjectsMonthlyReportSupport>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportSupportVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReport_Support_VW");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportsSectionsVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReports_Sections_VW");
            });

            modelBuilder.Entity<PmProjectsMonthlyReportsVw>(entity =>
            {
                entity.ToView("PM_Projects_MonthlyReports_VW");
            });

            modelBuilder.Entity<PmProjectsNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PmProjectsObjective>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsPaymentPlan>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsPerformanceIndicator>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsRequiredSupport>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsRequiredSupportVw>(entity =>
            {
                entity.ToView("PM_Projects_Required_Support_VW");
            });

            modelBuilder.Entity<PmProjectsResource>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsResourcesVw>(entity =>
            {
                entity.ToView("PM_Projects_Resources_VW");
            });

            modelBuilder.Entity<PmProjectsRisk>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsRisksVw>(entity =>
            {
                entity.ToView("PM_Projects_Risks_VW");

                entity.Property(e => e.EffectValue).IsFixedLength();

                entity.Property(e => e.ImpactValue).IsFixedLength();
            });

            modelBuilder.Entity<PmProjectsRisksVw2>(entity =>
            {
                entity.ToView("PM_Projects_Risks_VW2");
            });

            modelBuilder.Entity<PmProjectsStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStaffType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStaffVw>(entity =>
            {
                entity.ToView("PM_Projects_Staff_VW");
            });

            modelBuilder.Entity<PmProjectsStatementRequest>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStatementRequestVw>(entity =>
            {
                entity.ToView("PM_Projects_Statement_Request_VW");
            });

            modelBuilder.Entity<PmProjectsStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStatusVw>(entity =>
            {
                entity.ToView("PM_Projects_Status_VW");
            });

            modelBuilder.Entity<PmProjectsStokeholder>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStokeholderVw>(entity =>
            {
                entity.ToView("PM_Projects_Stokeholder_VW");
            });

            modelBuilder.Entity<PmProjectsStrategicLink>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsStrategicLinkVw>(entity =>
            {
                entity.ToView("PM_Projects_Strategic_Link_VW");
            });

            modelBuilder.Entity<PmProjectsTransaction>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsTransactionType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<PmProjectsTransactionsActivity>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsTransactionsItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsTransactionsItemsVw>(entity =>
            {
                entity.ToView("PM_Projects_Transactions_Items_VW");
            });

            modelBuilder.Entity<PmProjectsTransactionsVw>(entity =>
            {
                entity.ToView("PM_Projects_Transactions_VW");
            });

            modelBuilder.Entity<PmProjectsTreePart>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<PmProjectsType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsTypeTable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsTypeVw>(entity =>
            {
                entity.ToView("PM_Projects_Type_VW");
            });

            modelBuilder.Entity<PmProjectsUpdate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmProjectsVw>(entity =>
            {
                entity.ToView("PM_Projects_VW");
            });

            modelBuilder.Entity<PmPurchaseOrder>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmPurchaseOrderExstantion>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmPurchaseOrderExstantionVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_Exstantion_VW");
            });

            modelBuilder.Entity<PmPurchaseOrderItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmPurchaseOrderItemsListVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_Items_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmPurchaseOrderItemsVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_Items_VW");
            });

            modelBuilder.Entity<PmPurchaseOrderStatus>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmPurchaseOrderSupply>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmPurchaseOrderSupplyVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_Supply_VW");
            });

            modelBuilder.Entity<PmPurchaseOrderTypeIdVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_Type_ID_VW");
            });

            modelBuilder.Entity<PmPurchaseOrderVw>(entity =>
            {
                entity.ToView("PM_Purchase_Order_VW");
            });

            modelBuilder.Entity<PmPurchasingPlan>(entity =>
            {
                entity.Property(e => e.DeliveryStartDate).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.OfferingDate).IsFixedLength();
            });

            modelBuilder.Entity<PmRegulationsSystem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmRegulationsSystemsType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmRegulationsSystemsVw>(entity =>
            {
                entity.ToView("PM_Regulations_Systems_VW");
            });

            modelBuilder.Entity<PmReport>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmReportsVw>(entity =>
            {
                entity.ToView("PM_Reports_VW");
            });

            modelBuilder.Entity<PmResourcesAssignment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmResourcesAssignmentsVw>(entity =>
            {
                entity.ToView("PM_Resources_Assignments_VW");
            });

            modelBuilder.Entity<PmResumption>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmResumptionVw>(entity =>
            {
                entity.ToView("PM_Resumption_VW");
            });

            modelBuilder.Entity<PmRiskEffect>(entity =>
            {
                entity.Property(e => e.Value).IsFixedLength();
            });

            modelBuilder.Entity<PmRiskImpact>(entity =>
            {
                entity.Property(e => e.Value).IsFixedLength();
            });

            modelBuilder.Entity<PmSecuritySituation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PmSecuritySituationReply>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<PmSecuritySituationVw>(entity =>
            {
                entity.ToView("PM_Security_Situation_VW");
            });

            modelBuilder.Entity<PmSecurityStatusVw>(entity =>
            {
                entity.ToView("PM_Security_Status_VW");
            });

            modelBuilder.Entity<PmSession>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmSessionDay>(entity =>
            {
                entity.ToView("PM_Session_Days");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmSessionVw>(entity =>
            {
                entity.ToView("PM_Session_VW");
            });

            modelBuilder.Entity<PmSetting>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTimeSheetTarget>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTimeSheetTargetVw>(entity =>
            {
                entity.ToView("PM_TimeSheet_Target_VW");
            });

            modelBuilder.Entity<PmTimesheetDetaile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTimesheetDetailes2Vw>(entity =>
            {
                entity.ToView("PM_Timesheet_Detailes2_VW");
            });

            modelBuilder.Entity<PmTimesheetDetailesVw>(entity =>
            {
                entity.ToView("PM_Timesheet_Detailes_VW");
            });

            modelBuilder.Entity<PmTimesheetMaster>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTimesheetMasterVw>(entity =>
            {
                entity.ToView("PM_Timesheet_Master_VW");
            });

            modelBuilder.Entity<PmTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTransactionTimeSheet>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTransactionTimeSheetVw>(entity =>
            {
                entity.ToView("PM_Transaction_TimeSheet_VW");
            });

            modelBuilder.Entity<PmTransactionsD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTransactionsDVw>(entity =>
            {
                entity.ToView("PM_Transactions_D_VW");
            });

            modelBuilder.Entity<PmTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTransactionsInstallmentsVw>(entity =>
            {
                entity.ToView("PM_Transactions_Installments_VW");
            });

            modelBuilder.Entity<PmTransactionsPoVw>(entity =>
            {
                entity.ToView("PM_Transactions_PO_VW");
            });

            modelBuilder.Entity<PmTransactionsStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmTransactionsStatusVw>(entity =>
            {
                entity.ToView("PM_Transactions_Status_VW");
            });

            modelBuilder.Entity<PmTransactionsVw>(entity =>
            {
                entity.ToView("PM_Transactions_VW");
            });

            modelBuilder.Entity<PmTypeAddVw>(entity =>
            {
                entity.ToView("PM_Type_Add_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmTypeDeducVw>(entity =>
            {
                entity.ToView("PM_Type_Deduc_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmVerdict>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmVerdictFinancialCostsVw>(entity =>
            {
                entity.ToView("PM_Verdict_FinancialCosts_VW");
            });

            modelBuilder.Entity<PmVerdictTypeVw>(entity =>
            {
                entity.ToView("PM_Verdict_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PmVerdictVw>(entity =>
            {
                entity.ToView("PM_Verdict_VW");
            });

            modelBuilder.Entity<PmViolationType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmViolationTypeVw>(entity =>
            {
                entity.ToView("PM_Violation_Type_VW");
            });

            modelBuilder.Entity<PmWorkType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PmWorkTypeVw>(entity =>
            {
                entity.ToView("PM_Work_Type_VW");
            });

            modelBuilder.Entity<PrtlVisitorCounter>(entity =>
            {
                entity.Property(e => e.VisitCount).ValueGeneratedNever();
            });

            modelBuilder.Entity<PurCommittee>(entity =>
            {
                entity.Property(e => e.Code).IsFixedLength();
            });

            modelBuilder.Entity<PurCommitteeDecision>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurCommitteeDecisionsVw>(entity =>
            {
                entity.ToView("PUR_Committee_Decisions_VW");
            });

            modelBuilder.Entity<PurCommitteeMembersType>(entity =>
            {
                entity.ToView("PUR_Committee_Members_Type");
            });

            modelBuilder.Entity<PurCommitteeMembersVw>(entity =>
            {
                entity.ToView("PUR_Committee_Members_VW");
            });

            modelBuilder.Entity<PurCommitteesVw>(entity =>
            {
                entity.ToView("PUR_Committees_VW");

                entity.Property(e => e.Code).IsFixedLength();
            });

            modelBuilder.Entity<PurComparison>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurComparisonQuotation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurComparisonQuotationVw>(entity =>
            {
                entity.ToView("Pur_Comparison_Quotation_VW");
            });

            modelBuilder.Entity<PurComparisonSupplier>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurComparisonSuppliersVw>(entity =>
            {
                entity.ToView("PUR_Comparison_suppliers_VW");
            });

            modelBuilder.Entity<PurComparisonType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurComparisonVw>(entity =>
            {
                entity.ToView("PUR_Comparison_VW");
            });

            modelBuilder.Entity<PurDiscountByAmount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurDiscountByQty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurDiscountCatalog>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurDiscountCatalogAllVw>(entity =>
            {
                entity.ToView("PUR_Discount_Catalog_All_VW");
            });

            modelBuilder.Entity<PurDiscountCatalogVw>(entity =>
            {
                entity.ToView("PUR_Discount_Catalog_VW");
            });

            modelBuilder.Entity<PurDiscountProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurDiscountProductsVw>(entity =>
            {
                entity.ToView("PUR_Discount_Products_VW");
            });

            modelBuilder.Entity<PurDiscountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<PurExpense>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurExpensesList>(entity =>
            {
                entity.ToView("PUR_Expenses_List");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PurExpensesVw>(entity =>
            {
                entity.ToView("PUR_Expenses_VW");
            });

            modelBuilder.Entity<PurOfficialPaper>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurOfficialPapersVw>(entity =>
            {
                entity.ToView("PUR_OfficialPapers_VW");
            });

            modelBuilder.Entity<PurQuotationConformity>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurQuotationConformityVw>(entity =>
            {
                entity.ToView("PUR_Quotation_Conformity_VW");
            });

            modelBuilder.Entity<PurRfqEvaluationBossVw>(entity =>
            {
                entity.ToView("PUR_RFQ_EvaluationBoss_VW");
            });

            modelBuilder.Entity<PurRfqStatusVw>(entity =>
            {
                entity.ToView("PUR_RFQ_Status_VW");
            });

            modelBuilder.Entity<PurRfqTrackingStatusVw>(entity =>
            {
                entity.ToView("PUR_RFQ_Tracking_StatusVW");
            });

            modelBuilder.Entity<PurSubmitInvoicesItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurSubmitInvoicesItemsVw>(entity =>
            {
                entity.ToView("PUR_Submit_Invoices_Items_VW");
            });

            modelBuilder.Entity<PurSubmitInvoicesPayment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurSubmitInvoicesType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<PurTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.RefranceId).HasComment("رقم المرجع لعرض الاسعار او اوامر الشراء");
            });

            modelBuilder.Entity<PurTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsDiscountVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<PurTransactionsExpense>(entity =>
            {
                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsExpensesVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Expenses_VW");
            });

            modelBuilder.Entity<PurTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsNoteVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Note_VW");
            });

            modelBuilder.Entity<PurTransactionsPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<PurTransactionsPaymentVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Payment_VW");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<PurTransactionsProduct>(entity =>
            {
                entity.Property(e => e.AccountId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Equivalent).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReferenceNo).HasComment("رقم المرجع في نظام التقسيط");
            });

            modelBuilder.Entity<PurTransactionsProductsVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Products_VW");
            });

            modelBuilder.Entity<PurTransactionsSubmitInvoice>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsSubmitInvoicesVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Submit_Invoices_VW");
            });

            modelBuilder.Entity<PurTransactionsSupplier>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<PurTransactionsSupplierVw>(entity =>
            {
                entity.ToView("PUR_Transactions_Supplier_VW");
            });

            modelBuilder.Entity<PurTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<PurTransactionsVw>(entity =>
            {
                entity.ToView("PUR_Transactions_VW");
            });

            modelBuilder.Entity<ReAccruedRevenue>(entity =>
            {
                entity.Property(e => e.DueDate).IsFixedLength();

                entity.Property(e => e.EndDate).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.StartDate).IsFixedLength();
            });

            modelBuilder.Entity<ReAccruedRevenueContract>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReAccruedRevenueContractsVw>(entity =>
            {
                entity.ToView("RE_Accrued_Revenue_Contracts_VW");

                entity.Property(e => e.DueDate).IsFixedLength();

                entity.Property(e => e.EndDate).IsFixedLength();

                entity.Property(e => e.StartDate).IsFixedLength();
            });

            modelBuilder.Entity<ReAccruedRevenueRealestate>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReAccruedRevenueVw>(entity =>
            {
                entity.ToView("RE_Accrued_Revenue_VW");

                entity.Property(e => e.DueDate).IsFixedLength();

                entity.Property(e => e.EndDate).IsFixedLength();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.StartDate).IsFixedLength();
            });

            modelBuilder.Entity<ReBalanceSheetVw>(entity =>
            {
                entity.ToView("RE_Balance_Sheet_VW");
            });

            modelBuilder.Entity<ReContractInstallment>(entity =>
            {
                entity.Property(e => e.AmountPaid).HasDefaultValueSql("((0))");

                entity.Property(e => e.CollectingUserId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.PaidBefore).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractInstallment2Vw>(entity =>
            {
                entity.ToView("RE_Contract_Installment2_VW");
            });

            modelBuilder.Entity<ReContractInstallmentFollowUp>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractInstallmentFollowUpVw>(entity =>
            {
                entity.ToView("RE_Contract_Installment_Follow_up_VW");
            });

            modelBuilder.Entity<ReContractInstallmentNotInvoiceVw>(entity =>
            {
                entity.ToView("RE_Contract_InstallmentNotInvoice_VW");
            });

            modelBuilder.Entity<ReContractInstallmentNotPaidVw>(entity =>
            {
                entity.ToView("RE_Contract_InstallmentNotPaid_VW");
            });

            modelBuilder.Entity<ReContractInstallmentPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ReContractInstallmentPaymentVw>(entity =>
            {
                entity.ToView("RE_Contract_Installment_Payment_VW");
            });

            modelBuilder.Entity<ReContractInstallmentServicesVw>(entity =>
            {
                entity.ToView("RE_Contract_Installment_Services_VW");
            });

            modelBuilder.Entity<ReContractInstallmentVw>(entity =>
            {
                entity.ToView("RE_Contract_Installment_VW");
            });

            modelBuilder.Entity<ReContractInsurance>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractInsuranceVw>(entity =>
            {
                entity.ToView("RE_Contract_Insurance_VW");
            });

            modelBuilder.Entity<ReContractLease>(entity =>
            {
                entity.Property(e => e.AutoInvoices).HasDefaultValueSql("((0))");

                entity.Property(e => e.AutoSentToCustomer).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentType2).HasComment("طريقة دفع مبلغ الخدمات");
            });

            modelBuilder.Entity<ReContractLease2Vw>(entity =>
            {
                entity.ToView("RE_Contract_Lease2_VW");
            });

            modelBuilder.Entity<ReContractLeaseRealestate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractLeaseRealestate2Vw>(entity =>
            {
                entity.ToView("RE_Contract_Lease_Realestate2_VW");
            });

            modelBuilder.Entity<ReContractLeaseRealestateVw>(entity =>
            {
                entity.ToView("RE_Contract_Lease_Realestate_VW");
            });

            modelBuilder.Entity<ReContractLeaseRenew>(entity =>
            {
                entity.Property(e => e.Amount).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.InstallmentValue).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractLeaseService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractLeaseServicesVw>(entity =>
            {
                entity.ToView("RE_Contract_Lease_Services_vw");
            });

            modelBuilder.Entity<ReContractLeaseVw>(entity =>
            {
                entity.ToView("RE_Contract_Lease_VW");
            });

            modelBuilder.Entity<ReContractLeaseYear>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractLeaving>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ReContractLeavingVw>(entity =>
            {
                entity.ToView("RE_Contract_Leaving_VW");
            });

            modelBuilder.Entity<ReContractProperty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractSale>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentType2).HasComment("طريقة دفع مبلغ الخدمات");
            });

            modelBuilder.Entity<ReContractSaleRealestate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReContractStatusVw>(entity =>
            {
                entity.ToView("RE_Contract_Status_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ReContractType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReDynamicAttribute>(entity =>
            {
                entity.HasKey(e => e.DynamicAttributeId)
                    .HasName("PK_RE_DynamicAttributes_1");

                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReDynamicAttributesVw>(entity =>
            {
                entity.ToView("RE_DynamicAttributes_VW");
            });

            modelBuilder.Entity<ReDynamicValue>(entity =>
            {
                entity.Property(e => e.DynamicValueId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<ReExpensesReceipt>(entity =>
            {
                entity.Property(e => e.AccAccountId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CollectingEmpId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ContractLeaseId).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeBondId).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeId).HasComment("1 قبض 2 صرف");
            });

            modelBuilder.Entity<ReExpensesReceiptsPaymentInstallment>(entity =>
            {
                entity.HasKey(e => e.PaymentInstallId)
                    .HasName("PK_INVEST_PAYMENT_INSTALLMENT");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReNotification>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReNotificationVw>(entity =>
            {
                entity.ToView("RE_Notification_VW");
            });

            modelBuilder.Entity<ReNotificationsType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstate>(entity =>
            {
                entity.Property(e => e.Cc2Id).HasDefaultValueSql("((0))");

                entity.Property(e => e.Cc3Id).HasDefaultValueSql("((0))");

                entity.Property(e => e.Cc4Id).HasDefaultValueSql("((0))");

                entity.Property(e => e.Cc5Id).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ReRealEstateAsset>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateAssetsVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Assets_VW");
            });

            modelBuilder.Entity<ReRealEstateMaintenance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateMaintenanceItem>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateMaintenanceItemsVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Maintenance_Items_VW");
            });

            modelBuilder.Entity<ReRealEstateMaintenancePart>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateMaintenancePartVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Maintenance_Part_VW");
            });

            modelBuilder.Entity<ReRealEstateMaintenanceStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateMaintenanceStaffVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Maintenance_Staff_VW");
            });

            modelBuilder.Entity<ReRealEstateMaintenanceVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Maintenance_VW");
            });

            modelBuilder.Entity<ReRealEstateOwnerRatio>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateOwnerRatioVw>(entity =>
            {
                entity.ToView("RE_Real_estate_OwnerRatio_VW");
            });

            modelBuilder.Entity<ReRealEstateParentVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Parent_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ReRealEstateStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReRealEstateTypeVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_Type_VW");
            });

            modelBuilder.Entity<ReRealEstateVw>(entity =>
            {
                entity.ToView("RE_Real_Estate_VW");
            });

            modelBuilder.Entity<ReRealestatOwnersVw>(entity =>
            {
                entity.ToView("Re_Realestat_Owners_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ReReceipt>(entity =>
            {
                entity.Property(e => e.EmpId).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReReceiptsVw>(entity =>
            {
                entity.ToView("RE_Receipts_VW");

                entity.Property(e => e.EmpId).IsFixedLength();
            });

            modelBuilder.Entity<ReServicesType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReServicesTypesVw>(entity =>
            {
                entity.ToView("RE_Services_Types_VW");
            });

            modelBuilder.Entity<ReTransaction>(entity =>
            {
                entity.Property(e => e.AutoCreate).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.SendIsDone).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReTransactionsDiscountVw>(entity =>
            {
                entity.ToView("RE_Transactions_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<ReTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReTransactionsInstallmentsVw>(entity =>
            {
                entity.ToView("RE_Transactions_Installments_VW");
            });

            modelBuilder.Entity<ReTransactionsRealestate>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReTransactionsRealestateVw>(entity =>
            {
                entity.ToView("RE_Transactions_Realestate_VW");
            });

            modelBuilder.Entity<ReTransactionsService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ReTransactionsServicesVw>(entity =>
            {
                entity.ToView("RE_Transactions_Services_VW");
            });

            modelBuilder.Entity<ReTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ReTransactionsVw>(entity =>
            {
                entity.ToView("RE_Transactions_VW");
            });

            modelBuilder.Entity<RealEstateReceiptD>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RealEstateReceiptDVw>(entity =>
            {
                entity.ToView("Real_Estate_Receipt_D_VW");
            });

            modelBuilder.Entity<RealEstateReceiptM>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RealEstateReceiptMVw>(entity =>
            {
                entity.ToView("Real_Estate_Receipt_M_VW");
            });

            modelBuilder.Entity<RevlBank>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlCitiesNeighborhood>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlCitiesNeighborhoodsVw>(entity =>
            {
                entity.ToView("REVL_Cities_Neighborhoods_VW");
            });

            modelBuilder.Entity<RevlCitiesVw>(entity =>
            {
                entity.ToView("REVL_Cities_VW");

                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RevlEmpDataEntryVw>(entity =>
            {
                entity.ToView("REVL_Emp_DataEntry_VW");
            });

            modelBuilder.Entity<RevlEmpValuerVw>(entity =>
            {
                entity.ToView("REVL_Emp_Valuer_VW");
            });

            modelBuilder.Entity<RevlEvaluationManagerVw>(entity =>
            {
                entity.ToView("REVL_Evaluation_Manager_VW");
            });

            modelBuilder.Entity<RevlEvaluationSupervisorVw>(entity =>
            {
                entity.ToView("REVL_Evaluation_Supervisor_VW");
            });

            modelBuilder.Entity<RevlGeometric>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlPropertiesOld>(entity =>
            {
                entity.HasKey(e => e.PropertyId)
                    .HasName("PK_REVL_Properties");

                entity.Property(e => e.Sort).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlPropertiesRent>(entity =>
            {
                entity.HasKey(e => e.PropertyId)
                    .HasName("PK_REVL_Rent");
            });

            modelBuilder.Entity<RevlProperty>(entity =>
            {
                entity.HasKey(e => e.PropertyId)
                    .HasName("PK_REVL_Properties_2");
            });

            modelBuilder.Entity<RevlRealEstate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MarketValueNo).HasDefaultValueSql("((0))");

                entity.Property(e => e.ServicesElectric).HasDefaultValueSql("((0))");

                entity.Property(e => e.ServicesPhone).HasDefaultValueSql("((0))");

                entity.Property(e => e.ServicesSanitation).HasDefaultValueSql("((0))");

                entity.Property(e => e.ServicesWater).HasDefaultValueSql("((0))");

                entity.Property(e => e.Type2Id).HasComment("نوع العقار");

                entity.Property(e => e.TypeId).HasComment("نوع العقار");

                entity.Property(e => e.WhiteEarthBit).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateArea>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateComparison>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateComparisonsVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Comparisons_VW");
            });

            modelBuilder.Entity<RevlRealEstateContract>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateContractVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Contract_VW");
            });

            modelBuilder.Entity<RevlRealEstateFile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateFilesVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Files_VW");
            });

            modelBuilder.Entity<RevlRealEstateGeometric>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateImag>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateImagsVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Imags_VW");
            });

            modelBuilder.Entity<RevlRealEstateRasmala>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateRasmalaVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Rasmala_VW");
            });

            modelBuilder.Entity<RevlRealEstateRent>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<RevlRealEstateStatusVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_Status_VW");
            });

            modelBuilder.Entity<RevlRealEstateType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlRealEstateVw>(entity =>
            {
                entity.ToView("REVL_Real_Estate_VW");
            });

            modelBuilder.Entity<RevlStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlStep>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlValuationRatio>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RevlZoneVw>(entity =>
            {
                entity.ToView("REVL_Zone_VW");

                entity.Property(e => e.ZoneId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RptCustomReport>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptField>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptFieldsVw>(entity =>
            {
                entity.ToView("RPT_Fields_VW");
            });

            modelBuilder.Entity<RptOperator>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptReport>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<RptReportsField>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptReportsFieldsVw>(entity =>
            {
                entity.ToView("RPT_Reports_Fields_VW");
            });

            modelBuilder.Entity<RptReportsGroupBy>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptReportsGroupByVw>(entity =>
            {
                entity.ToView("RPT_Reports_GroupBy_VW");
            });

            modelBuilder.Entity<RptReportsOrderBy>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RptReportsOrderByVw>(entity =>
            {
                entity.ToView("RPT_Reports_OrderBy_VW");
            });

            modelBuilder.Entity<RptReportsVw>(entity =>
            {
                entity.ToView("RPT_Reports_VW");
            });

            modelBuilder.Entity<RptTable>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalAdditionalType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalAdditionalTypeVw>(entity =>
            {
                entity.ToView("SAL_Additional_Type_VW");
            });

            modelBuilder.Entity<SalAgeCreditVw>(entity =>
            {
                entity.ToView("SAL_AgeCredit_VW");
            });

            modelBuilder.Entity<SalCommission>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalCommissionBranch>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalCommissionBranchVw>(entity =>
            {
                entity.ToView("SAL_Commission_Branch_VW");
            });

            modelBuilder.Entity<SalCommissionCalculation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SalCommissionDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalCommissionDetailesVw>(entity =>
            {
                entity.ToView("SAL_Commission_Detailes_VW");
            });

            modelBuilder.Entity<SalCommissionVw>(entity =>
            {
                entity.ToView("SAL_Commission_VW");

                entity.Property(e => e.Name).IsFixedLength();

                entity.Property(e => e.Name2).IsFixedLength();
            });

            modelBuilder.Entity<SalCustomerPoint>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalCustomerPointsRefrance>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SalCustomerPointsVw>(entity =>
            {
                entity.ToView("SAL_Customer_Points_VW");
            });

            modelBuilder.Entity<SalDiscountByAmount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalDiscountByQty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalDiscountCatalog>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalDiscountCatalogAllVw>(entity =>
            {
                entity.ToView("SAL_Discount_Catalog_All_VW");
            });

            modelBuilder.Entity<SalDiscountCatalogDetailVw>(entity =>
            {
                entity.ToView("SAL_Discount_Catalog_Detail_VW");
            });

            modelBuilder.Entity<SalDiscountCatalogVw>(entity =>
            {
                entity.ToView("SAL_Discount_Catalog_VW");
            });

            modelBuilder.Entity<SalDiscountProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalDiscountProductsVw>(entity =>
            {
                entity.ToView("SAL_Discount_Products_VW");
            });

            modelBuilder.Entity<SalDiscountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SalInvoiceDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SalInvoiceDetailsVw>(entity =>
            {
                entity.ToView("SAL_Invoice_Details_VW");
            });

            modelBuilder.Entity<SalItemsPriceD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalItemsPriceDVw>(entity =>
            {
                entity.ToView("SAL_Items_Price_D_VW");
            });

            modelBuilder.Entity<SalItemsPriceList>(entity =>
            {
                entity.ToView("SAL_Items_Price_List");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SalItemsPriceM>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalItemsPriceMVw>(entity =>
            {
                entity.ToView("SAL_Items_Price_M_VW");
            });

            modelBuilder.Entity<SalListItemPriceVw>(entity =>
            {
                entity.ToView("SAL_List_Item_Price_VW");
            });

            modelBuilder.Entity<SalOrderMake>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalOrderMakeVw>(entity =>
            {
                entity.ToView("SAL_Order_Make_VW");
            });

            modelBuilder.Entity<SalPaymentTerm>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalPaymentTermsVw>(entity =>
            {
                entity.ToView("SAL_Payment_Terms_VW");
            });

            modelBuilder.Entity<SalPointByAmount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalPointByAmountVw>(entity =>
            {
                entity.ToView("SAL_Point_By_Amount_VW");
            });

            modelBuilder.Entity<SalPointCatalog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalPointType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SalPosCloseCash>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalPosSetting>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Online).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SalPosSettingVw>(entity =>
            {
                entity.ToView("SAL_POS_Setting_VW");
            });

            modelBuilder.Entity<SalPosUser>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalPosUsersVw>(entity =>
            {
                entity.ToView("SAL_POS_Users_VW");
            });

            modelBuilder.Entity<SalProductsSearching>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalProductsSearchingVw>(entity =>
            {
                entity.ToView("SAL_Products_Searching_VW");
            });

            modelBuilder.Entity<SalReceiptingInvoice>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalReceiptingInvoicesSection>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalReceiptingInvoicesSectionsVw>(entity =>
            {
                entity.ToView("SAL_Receipting_Invoices_Sections_VW");
            });

            modelBuilder.Entity<SalReceiptingInvoicesVw>(entity =>
            {
                entity.ToView("SAL_Receipting_Invoices_VW");
            });

            modelBuilder.Entity<SalScaleSetting>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalScaleSettingVw>(entity =>
            {
                entity.ToView("SAL_Scale_Setting_VW");
            });

            modelBuilder.Entity<SalSetting>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTailorDesign>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTailorDesignCatagory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTailorDesignVw>(entity =>
            {
                entity.ToView("SAL_Tailor_Design_VW");
            });

            modelBuilder.Entity<SalTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HasReservation).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isposted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Points).HasDefaultValueSql("((0))");

                entity.Property(e => e.RefranceId).HasComment("رقم المرجع لعرض الاسعار او اوامر البيع");
            });

            modelBuilder.Entity<SalTransactionCommisionVw>(entity =>
            {
                entity.ToView("SAL_Transaction_Commision_VW");
            });

            modelBuilder.Entity<SalTransactionsAdditional>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsAdditionalVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Additional_VW");
            });

            modelBuilder.Entity<SalTransactionsBranchReportVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Branch_Report_VW");
            });

            modelBuilder.Entity<SalTransactionsCommission>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsCommissionVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Commission_VW");
            });

            modelBuilder.Entity<SalTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsDiscountVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<SalTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsNoteVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Note_VW");
            });

            modelBuilder.Entity<SalTransactionsPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<SalTransactionsPaymentVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Payment_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<SalTransactionsProduct>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.UnitCost).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsProductTypeVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Product_Type_VW");
            });

            modelBuilder.Entity<SalTransactionsProducts6Vw>(entity =>
            {
                entity.ToView("SAL_Transactions_Products6_VW");
            });

            modelBuilder.Entity<SalTransactionsProductsExportVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Products_Export_VW");
            });

            modelBuilder.Entity<SalTransactionsProductsPrintVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Products_Print_VW");
            });

            modelBuilder.Entity<SalTransactionsProductsPropertiy>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsProductsVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Products_VW");
            });

            modelBuilder.Entity<SalTransactionsProductsVw2>(entity =>
            {
                entity.ToView("SAL_Transactions_Products_VW2");
            });

            modelBuilder.Entity<SalTransactionsProductsVw3>(entity =>
            {
                entity.ToView("SAL_Transactions_Products_VW3");
            });

            modelBuilder.Entity<SalTransactionsTicket>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SalTransactionsTicketVw>(entity =>
            {
                entity.ToView("SAL_Transactions_Ticket_VW");
            });

            modelBuilder.Entity<SalTransactionsTransPortVw>(entity =>
            {
                entity.ToView("SAL_Transactions_TransPort_VW");
            });

            modelBuilder.Entity<SalTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SalTransactionsVw>(entity =>
            {
                entity.ToView("SAL_Transactions_VW");
            });

            modelBuilder.Entity<SchAdmission>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchAdmissionResponseVw>(entity =>
            {
                entity.ToView("Sch_Admission_Response_VW");
            });

            modelBuilder.Entity<SchAdmissionVw>(entity =>
            {
                entity.ToView("Sch_Admission_VW");
            });

            modelBuilder.Entity<SchClassRoom>(entity =>
            {
                entity.Property(e => e.Capacity).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SchClassRoomsVw>(entity =>
            {
                entity.ToView("Sch_ClassRooms_VW");
            });

            modelBuilder.Entity<SchDiscountCatalog>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchDiscountCatalogVw>(entity =>
            {
                entity.ToView("SCH_Discount_Catalog_VW");
            });

            modelBuilder.Entity<SchDiscountFee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchDiscountFeesVw>(entity =>
            {
                entity.ToView("Sch_Discount_Fees_VW");
            });

            modelBuilder.Entity<SchDiscountType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchGrade>(entity =>
            {
                entity.Property(e => e.Capacity).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SchGradesVw>(entity =>
            {
                entity.ToView("Sch_Grades_VW");
            });

            modelBuilder.Entity<SchLevel>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SchLevelsDiscount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchLevelsDiscountDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchLevelsDiscountVw>(entity =>
            {
                entity.ToView("SCH_Levels_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();
            });

            modelBuilder.Entity<SchMethodDistributionFee>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchMethodDistributionFeesDetailed>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchQuittance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SchQuittanceVw>(entity =>
            {
                entity.ToView("Sch_Quittance_VW");
            });

            modelBuilder.Entity<SchRegistration>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchRegistrationInstallment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchRegistrationInstallmentPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SchRegistrationInstallmentPaymentVw>(entity =>
            {
                entity.ToView("Sch_Registration_Installment_Payment_VW");
            });

            modelBuilder.Entity<SchRegistrationInstallmentVw>(entity =>
            {
                entity.ToView("Sch_Registration_Installment_VW");
            });

            modelBuilder.Entity<SchRegistrationVw>(entity =>
            {
                entity.ToView("Sch_Registration_VW");
            });

            modelBuilder.Entity<SchRoute>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchRouteDetailed>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchRouteDetailedVw>(entity =>
            {
                entity.ToView("Sch_Route_Detailed_VW");
            });

            modelBuilder.Entity<SchRouteLink>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchRouteLinkVw>(entity =>
            {
                entity.ToView("Sch_Route_Link_VW");
            });

            modelBuilder.Entity<SchRouteVw>(entity =>
            {
                entity.ToView("Sch_Route_VW");
            });

            modelBuilder.Entity<SchSchool>(entity =>
            {
                entity.ToView("Sch_Schools");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SchSchoolLevel>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchSchoolLevelsVw>(entity =>
            {
                entity.ToView("Sch_SchoolLevels_VW");
            });

            modelBuilder.Entity<SchSchoolTerm>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SchSchoolTermsVw>(entity =>
            {
                entity.ToView("Sch_SchoolTerms_VW");
            });

            modelBuilder.Entity<SchSchoolYear>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SchSchoolYearsVw>(entity =>
            {
                entity.ToView("Sch_SchoolYears_VW");
            });

            modelBuilder.Entity<SchSchoolsVw>(entity =>
            {
                entity.ToView("Sch_Schools_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SchStudentClass>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SchStudentClassesVw>(entity =>
            {
                entity.ToView("Sch_StudentClasses_VW");
            });

            modelBuilder.Entity<SchStudentStatusVw>(entity =>
            {
                entity.ToView("SCH_Student_Status_VW");
            });

            modelBuilder.Entity<SchStudentTransfer>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchStudentVw>(entity =>
            {
                entity.ToView("Sch_Student_VW");
            });

            modelBuilder.Entity<SchStudyFee>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchStudyFeesGroup>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchStudyFeesGroupVw>(entity =>
            {
                entity.ToView("Sch_StudyFees_Group_VW");
            });

            modelBuilder.Entity<SchStudyFeesVw>(entity =>
            {
                entity.ToView("Sch_Study_fees_VW");
            });

            modelBuilder.Entity<SchTeachersVw>(entity =>
            {
                entity.ToView("Sch_Teachers_VW");
            });

            modelBuilder.Entity<SchTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchTransactionsDiscountVw>(entity =>
            {
                entity.ToView("Sch_Transactions_Discount_VW");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<SchTransactionsInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchTransactionsInstallmentsVw>(entity =>
            {
                entity.ToView("Sch_Transactions_Installments_VW");
            });

            modelBuilder.Entity<SchTransactionsPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<SchTransactionsPaymentVw>(entity =>
            {
                entity.ToView("SCH_Transactions_Payment_VW");

                entity.Property(e => e.PaymentDate).IsFixedLength();
            });

            modelBuilder.Entity<SchTransactionsPrintVw>(entity =>
            {
                entity.ToView("Sch_Transactions_Print_VW");
            });

            modelBuilder.Entity<SchTransactionsTransportationInstallmentsVw>(entity =>
            {
                entity.ToView("SCH_Transactions_Transportation_Installments_VW");
            });

            modelBuilder.Entity<SchTransactionsTransportationPrintVw>(entity =>
            {
                entity.ToView("Sch_Transactions_Transportation_Print_VW");
            });

            modelBuilder.Entity<SchTransactionsTransportationVw>(entity =>
            {
                entity.ToView("SCH_Transactions_Transportation_VW");
            });

            modelBuilder.Entity<SchTransactionsVw>(entity =>
            {
                entity.ToView("Sch_Transactions_VW");
            });

            modelBuilder.Entity<SchTransportation>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchTransportationInstallment>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SchTransportationInstallmentPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SchTransportationInstallmentPaymentVw>(entity =>
            {
                entity.ToView("Sch_Transportation_Installment_Payment_VW");
            });

            modelBuilder.Entity<SchTransportationInstallmentVw>(entity =>
            {
                entity.ToView("Sch_Transportation_Installment_VW");
            });

            modelBuilder.Entity<SchTransportationVw>(entity =>
            {
                entity.ToView("Sch_Transportation_VW");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToView("size");
            });

            modelBuilder.Entity<SubCategoriesVw>(entity =>
            {
                entity.ToView("Sub_Categories_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SubDiscountCatalog>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SubDiscountCatalogVw>(entity =>
            {
                entity.ToView("Sub_Discount_Catalog_VW");
            });

            modelBuilder.Entity<SubDiscountPackage>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SubDiscountPackageVw>(entity =>
            {
                entity.ToView("Sub_Discount_Package_VW");
            });

            modelBuilder.Entity<SubDurationsVw>(entity =>
            {
                entity.ToView("Sub_Durations_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SubLoginMember>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SubLoginMemberVw>(entity =>
            {
                entity.ToView("Sub_Login_Member_VW");
            });

            modelBuilder.Entity<SubMembersVw>(entity =>
            {
                entity.ToView("Sub_Members_VW");
            });

            modelBuilder.Entity<SubPackage>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SubPackageVw>(entity =>
            {
                entity.ToView("Sub_Package_VW");
            });

            modelBuilder.Entity<SubStatusVw>(entity =>
            {
                entity.ToView("Sub_Status_VW");
            });

            modelBuilder.Entity<SubSubscription>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SubSubscriptionPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<SubSubscriptionPaymentVw>(entity =>
            {
                entity.ToView("Sub_Subscription_Payment_vw");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<SubSubscriptionsReactive>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SubSubscriptionsSuspend>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SubSubscriptionsSuspendVw>(entity =>
            {
                entity.ToView("Sub_Subscriptions_Suspend_VW");
            });

            modelBuilder.Entity<SubSubscriptionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SubSubscriptionsVw>(entity =>
            {
                entity.ToView("Sub_Subscriptions_VW");

                entity.Property(e => e.PaymentDate).IsFixedLength();
            });

            modelBuilder.Entity<SysActionTypeVw>(entity =>
            {
                entity.ToView("Sys_Action_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysActivityLogM2Vw>(entity =>
            {
                entity.ToView("Sys_ActivityLogM2_VW");
            });

            modelBuilder.Entity<SysActivityLogMVw>(entity =>
            {
                entity.ToView("Sys_ActivityLogM_VW");
            });

            modelBuilder.Entity<SysActivityLogVw>(entity =>
            {
                entity.ToView("Sys_ActivityLog_VW");
            });

            modelBuilder.Entity<SysActivityVw>(entity =>
            {
                entity.ToView("Sys_Activity_VW");
            });

            modelBuilder.Entity<SysAgeTypeVw>(entity =>
            {
                entity.ToView("Sys_Age_Type_VW");
            });

            modelBuilder.Entity<SysAnnouncement>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysAnnouncementLocationVw>(entity =>
            {
                entity.ToView("Sys_Announcement_Location_VW");
            });

            modelBuilder.Entity<SysAnnouncementTypeVw>(entity =>
            {
                entity.ToView("Sys_Announcement_Type_VW");
            });

            modelBuilder.Entity<SysAnnouncementVw>(entity =>
            {
                entity.ToView("Sys_Announcement_VW");
            });

            modelBuilder.Entity<SysApplicationsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysAppointment>(entity =>
            {
                entity.HasKey(e => e.AppId)
                    .HasName("PK_Sys_Event");

                entity.Property(e => e.AllDay).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysAppointmentVw>(entity =>
            {
                entity.ToView("Sys_Appointment_VW");
            });

            modelBuilder.Entity<SysArchiveFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysBanksVw>(entity =>
            {
                entity.ToView("Sys_Banks_VW");
            });

            modelBuilder.Entity<SysBranch>(entity =>
            {
                entity.ToView("Sys_BRANCH");

                entity.Property(e => e.BranchId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysBranchVw>(entity =>
            {
                entity.ToView("SYS_BRANCH_VW");
            });

            modelBuilder.Entity<SysCite>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCitesVw>(entity =>
            {
                entity.ToView("Sys_Cites_vw");

                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCityVw>(entity =>
            {
                entity.ToView("Sys_City_VW");

                entity.Property(e => e.CityId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCountryVw>(entity =>
            {
                entity.ToView("Sys_Country_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCreateUserRequst>(entity =>
            {
                entity.Property(e => e.Approve).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCreateUserRequstVw>(entity =>
            {
                entity.ToView("Sys_CreateUser_Requst_VW");
            });

            modelBuilder.Entity<SysCurrency>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCurrencyListVw>(entity =>
            {
                entity.ToView("Sys_Currency_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCustomer>(entity =>
            {
                entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Iscompleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.OwnerProperty).HasDefaultValueSql("((0))");

                entity.Property(e => e.RateFileCompletion).HasDefaultValueSql("((0))");

                entity.Property(e => e.TitleId).HasComment("اللقب");

                entity.Property(e => e.Veneration).HasComment("التبجيل");
            });

            modelBuilder.Entity<SysCustomerAssigin>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerBranch>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerBranchVw>(entity =>
            {
                entity.ToView("Sys_Customer_Branch_VW");
            });

            modelBuilder.Entity<SysCustomerCoType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysCustomerCondition>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysCustomerContact>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerContactVw>(entity =>
            {
                entity.ToView("Sys_Customer_Contact_VW");
            });

            modelBuilder.Entity<SysCustomerDdlVw>(entity =>
            {
                entity.ToView("Sys_Customer_DDL_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCustomerDomain>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerDomainsVw>(entity =>
            {
                entity.ToView("Sys_Customer_Domains_VW");
            });

            modelBuilder.Entity<SysCustomerExperience>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerFilesVw>(entity =>
            {
                entity.ToView("Sys_Customer_Files_VW");
            });

            modelBuilder.Entity<SysCustomerGroup>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerGroupAccount>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerGroupAccountsVw>(entity =>
            {
                entity.ToView("Sys_Customer_Group_Accounts_VW");
            });

            modelBuilder.Entity<SysCustomerIndustryVw>(entity =>
            {
                entity.ToView("Sys_Customer_Industry_VW");
            });

            modelBuilder.Entity<SysCustomerListVw>(entity =>
            {
                entity.ToView("Sys_Customer_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysCustomerNote>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerNoteVw>(entity =>
            {
                entity.ToView("Sys_Customer_Note_VW");
            });

            modelBuilder.Entity<SysCustomerService>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerServicesVw>(entity =>
            {
                entity.ToView("Sys_Customer_Services_VW");
            });

            modelBuilder.Entity<SysCustomerSize>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<SysCustomerSizeVw>(entity =>
            {
                entity.ToView("Sys_Customer_Size_VW");
            });

            modelBuilder.Entity<SysCustomerSizeVw1>(entity =>
            {
                entity.ToView("Sys_Customer_Size_VW1");
            });

            modelBuilder.Entity<SysCustomerSizeVw2>(entity =>
            {
                entity.ToView("Sys_Customer_Size_VW2");
            });

            modelBuilder.Entity<SysCustomerStatusVw>(entity =>
            {
                entity.ToView("Sys_Customer_Status_VW");
            });

            modelBuilder.Entity<SysCustomerTransfer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysCustomerType>(entity =>
            {
                entity.Property(e => e.TypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysCustomerVw>(entity =>
            {
                entity.ToView("Sys_Customer_VW");
            });

            modelBuilder.Entity<SysDepartment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysDepartmentCatagory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysDepartmentVw>(entity =>
            {
                entity.ToView("Sys_Department_VW");
            });

            modelBuilder.Entity<SysDocument>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysDocumentValuesVw>(entity =>
            {
                entity.ToView("Sys_Document_Values_VW");
            });

            modelBuilder.Entity<SysDocumentsVw>(entity =>
            {
                entity.ToView("Sys_Documents_VW");
            });

            modelBuilder.Entity<SysDynamicAttribute>(entity =>
            {
                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysDynamicAttributesVw>(entity =>
            {
                entity.ToView("Sys_DynamicAttributes_VW");
            });

            modelBuilder.Entity<SysDynamicValue>(entity =>
            {
                entity.HasKey(e => e.DynamicValueId)
                    .HasName("PK_Sys_DynamicValues_1");

                entity.Property(e => e.DynamicValueId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysEmailsAllVw>(entity =>
            {
                entity.ToView("Sys_Emails_All_VW");
            });

            modelBuilder.Entity<SysExchangeRate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysExchangeRateListVw>(entity =>
            {
                entity.ToView("Sys_Exchange_Rate_List_VW");
            });

            modelBuilder.Entity<SysExchangeRateVw>(entity =>
            {
                entity.ToView("Sys_Exchange_Rate_VW");
            });

            modelBuilder.Entity<SysFile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysFilesAllVw>(entity =>
            {
                entity.ToView("Sys_Files_All_VW");
            });

            modelBuilder.Entity<SysFilesDocument>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysFilesDocumentVw>(entity =>
            {
                entity.ToView("Sys_Files_Document_VW");
            });

            modelBuilder.Entity<SysForm>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysFormsVw>(entity =>
            {
                entity.ToView("Sys_Forms_VW");
            });

            modelBuilder.Entity<SysGroup>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysGroupVw>(entity =>
            {
                entity.ToView("Sys_Group_VW");
            });

            modelBuilder.Entity<SysInformation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysLibraryFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysLibraryFilesVw>(entity =>
            {
                entity.ToView("Sys_Library_Files_VW");
            });

            modelBuilder.Entity<SysLicense>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysLicenseFile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysLicenseFilesVw>(entity =>
            {
                entity.ToView("Sys_License_Files_VW");
            });

            modelBuilder.Entity<SysLicensesVw>(entity =>
            {
                entity.ToView("Sys_Licenses_VW");
            });

            modelBuilder.Entity<SysLocationVw>(entity =>
            {
                entity.ToView("Sys_Location_VW");
            });

            modelBuilder.Entity<SysLookUpCatagory>(entity =>
            {
                entity.Property(e => e.CatagoriesId).ValueGeneratedNever();

                entity.Property(e => e.IsDeletable).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsEditable).HasDefaultValueSql("((1))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysLookupColor>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysLookupDataGenderVw>(entity =>
            {
                entity.ToView("Sys_lookup_Data_Gender_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysLookupDataVw>(entity =>
            {
                entity.ToView("Sys_lookup_Data_VW");
            });

            modelBuilder.Entity<SysLookupDatum>(entity =>
            {
                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysMailServer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysMailServerListVw>(entity =>
            {
                entity.ToView("Sys_MailServer_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysMessage>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysMessageVw>(entity =>
            {
                entity.ToView("Sys_Message_VW");
            });

            modelBuilder.Entity<SysMobileMember>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<SysModule>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysMonthVw>(entity =>
            {
                entity.ToView("Sys_Month_VW");

                entity.Property(e => e.MonthId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysNameAllVw>(entity =>
            {
                entity.ToView("Sys_Name_All_VW");
            });

            modelBuilder.Entity<SysNationalityTypeVw>(entity =>
            {
                entity.ToView("Sys_Nationality_Type_VW");
            });

            modelBuilder.Entity<SysNationalityVw>(entity =>
            {
                entity.ToView("Sys_Nationality_VW");
            });

            modelBuilder.Entity<SysNotification>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsRead).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysNotificationsMang>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysNotificationsMangVw>(entity =>
            {
                entity.ToView("Sys_Notifications_Mang_VW");
            });

            modelBuilder.Entity<SysNotificationsSetting>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysNotificationsSettingVw>(entity =>
            {
                entity.ToView("Sys_Notifications_Setting_VW");
            });

            modelBuilder.Entity<SysNotificationsVw>(entity =>
            {
                entity.ToView("Sys_Notifications_VW");
            });

            modelBuilder.Entity<SysOwnersListVw>(entity =>
            {
                entity.ToView("Sys_Owners_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysPoliciesProcedure>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysPoliciesProceduresTypeVw>(entity =>
            {
                entity.ToView("Sys_Policies_Procedures_Type_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysPoliciesProceduresVw>(entity =>
            {
                entity.ToView("Sys_Policies_Procedures_VW");
            });

            modelBuilder.Entity<SysPropertiesVw>(entity =>
            {
                entity.ToView("Sys_Properties_VW");
            });

            modelBuilder.Entity<SysProperty>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysPropertyValuesVw>(entity =>
            {
                entity.ToView("Sys_Property_Values_VW");
            });

            modelBuilder.Entity<SysResetPassword>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsUpdate).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysScreen>(entity =>
            {
                entity.HasKey(e => e.ScreenId)
                    .HasName("PK_Sys_SCREEN");

                entity.Property(e => e.ScreenId).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysScreenInstalled>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysScreenInstalledVw>(entity =>
            {
                entity.ToView("Sys_Screen_Installed_VW");
            });

            modelBuilder.Entity<SysScreenPermission>(entity =>
            {
                entity.HasKey(e => e.PriveId)
                    .HasName("PK_Sys_SCREEN_PERMISSION2");
            });

            modelBuilder.Entity<SysScreenPermissionPropertiesVw>(entity =>
            {
                entity.ToView("Sys_Screen_Permission_Properties_VW");
            });

            modelBuilder.Entity<SysScreenPermissionProperty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysScreenPermissionVw>(entity =>
            {
                entity.ToView("Sys_SCREEN_PERMISSION_VW");
            });

            modelBuilder.Entity<SysScreenProperty>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PropertyType).HasComment("نوع الخاصية هل checkbox Or Value");
            });

            modelBuilder.Entity<SysScreenVw>(entity =>
            {
                entity.ToView("Sys_Screen_VW");
            });

            modelBuilder.Entity<SysScreenWorkflow>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysSettingExport>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysSettingExportVw>(entity =>
            {
                entity.ToView("Sys_Setting_Export_VW");
            });

            modelBuilder.Entity<SysStrategicPlan>(entity =>
            {
                entity.Property(e => e.BasedId).HasComment("المرتكز");

                entity.Property(e => e.InsertDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysStrategicPlanVw>(entity =>
            {
                entity.ToView("Sys_Strategic_Plan_VW");
            });

            modelBuilder.Entity<SysSupplierVw>(entity =>
            {
                entity.ToView("Sys_Supplier_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SysSystem>(entity =>
            {
                entity.Property(e => e.SystemId).ValueGeneratedNever();

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTable>(entity =>
            {
                entity.Property(e => e.TableId).ValueGeneratedNever();
            });

            modelBuilder.Entity<SysTargetEmployee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTargetEmployeeVw>(entity =>
            {
                entity.ToView("Sys_Target_Employee_VW");
            });

            modelBuilder.Entity<SysTargetMaster>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTargetMasterVw>(entity =>
            {
                entity.ToView("Sys_Target_Master_VW");
            });

            modelBuilder.Entity<SysTargetPeriod>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTargetPeriodVw>(entity =>
            {
                entity.ToView("Sys_Target_Period_VW");
            });

            modelBuilder.Entity<SysTargetQty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TargetQty).IsFixedLength();
            });

            modelBuilder.Entity<SysTargetQtyVw>(entity =>
            {
                entity.ToView("Sys_Target_Qty_vw");

                entity.Property(e => e.TargetQty).IsFixedLength();
            });

            modelBuilder.Entity<SysTask>(entity =>
            {
                entity.Property(e => e.DoneOn).HasComment("تاريخ اغلاق المهمة");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTaskStatusVw>(entity =>
            {
                entity.ToView("Sys_Task_Status_VW");
            });

            modelBuilder.Entity<SysTasksResponse>(entity =>
            {
                entity.Property(e => e.ExractReport).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTasksResponseVw>(entity =>
            {
                entity.ToView("Sys_Tasks_Response_VW");
            });

            modelBuilder.Entity<SysTasksScheduler>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DoneOn).HasComment("تاريخ اغلاق المهمة");
            });

            modelBuilder.Entity<SysTasksSchedulerVw>(entity =>
            {
                entity.ToView("Sys_Tasks_Scheduler_VW");
            });

            modelBuilder.Entity<SysTasksType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTasksVw>(entity =>
            {
                entity.ToView("Sys_Tasks_VW");
            });

            modelBuilder.Entity<SysTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysTemplateVw>(entity =>
            {
                entity.ToView("Sys_Template_VW");
            });

            modelBuilder.Entity<SysTitleVw>(entity =>
            {
                entity.ToView("Sys_Title_VW");
            });

            modelBuilder.Entity<SysUpdate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUpdates2019>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUpdates2020>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUpdates2021>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUpdates2022>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUpdatesInstalled>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SysUpdatesInstalledVw>(entity =>
            {
                entity.ToView("Sys_Updates_Installed_VW");
            });

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .IsClustered(false);

                entity.HasIndex(e => e.UserId, "ID")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Enable).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsAgree).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.Isupdate).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUserDevice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysUserLogTimeVw>(entity =>
            {
                entity.ToView("SYS_USER_LogTime_VW");
            });

            modelBuilder.Entity<SysUserTrackingVw>(entity =>
            {
                entity.ToView("Sys_User_Tracking_VW");
            });

            modelBuilder.Entity<SysUserVw>(entity =>
            {
                entity.ToView("SYS_USER_VW");
            });

            modelBuilder.Entity<SysVatGroup>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<SysVatGroupVw>(entity =>
            {
                entity.ToView("Sys_VAT_Group_VW");

                entity.Property(e => e.VatName).IsFixedLength();
            });

            modelBuilder.Entity<SysVatReportsVw>(entity =>
            {
                entity.ToView("Sys_VAT_Reports_VW");
            });

            modelBuilder.Entity<TndTender>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TndTenderGuarantee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TndTenderGuaranteesVw>(entity =>
            {
                entity.ToView("TND_Tender_Guarantees_VW");
            });

            modelBuilder.Entity<TndTenderVw>(entity =>
            {
                entity.ToView("TND_Tender_VW");
            });

            modelBuilder.Entity<TndTendersItem>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TndTendersItemsApprove>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TndTendersItemsApproveVw>(entity =>
            {
                entity.ToView("TND_Tenders_Items_Approve_VW");
            });

            modelBuilder.Entity<TndTendersItemsPrice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TndTendersItemsPriceVw>(entity =>
            {
                entity.ToView("TND_Tenders_Items_Price_VW");
            });

            modelBuilder.Entity<TransCar>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransCarDriver>(entity =>
            {
                entity.ToView("Trans_Car_Drivers");
            });

            modelBuilder.Entity<TransCarsType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransCarsTypeVw>(entity =>
            {
                entity.ToView("Trans_Cars_Type_VW");
            });

            modelBuilder.Entity<TransCarsVw>(entity =>
            {
                entity.ToView("Trans_Cars_VW");
            });

            modelBuilder.Entity<TransContainer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContainerSize>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContainersState>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContainersStateVw>(entity =>
            {
                entity.ToView("Trans_Containers_State_VW");
            });

            modelBuilder.Entity<TransContainersVw>(entity =>
            {
                entity.ToView("Trans_Containers_VW");
            });

            modelBuilder.Entity<TransContract>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReturnTrip).HasDefaultValueSql("((0))");

                entity.Property(e => e.TripTime).IsFixedLength();
            });

            modelBuilder.Entity<TransContractContainer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContractContainersVw>(entity =>
            {
                entity.ToView("Trans_Contract_Containers_VW");
            });

            modelBuilder.Entity<TransContractCustomer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContractCustomersVw>(entity =>
            {
                entity.ToView("Trans_Contract_Customers_VW");
            });

            modelBuilder.Entity<TransContractInstallment>(entity =>
            {
                entity.Property(e => e.AmountPaid).HasDefaultValueSql("((0))");

                entity.Property(e => e.CollectingUserId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaidBefore).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContractInstallmentListVw>(entity =>
            {
                entity.ToView("Trans_Contract_Installment_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TransContractInstallmentPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TransContractInstallmentPaymentVw>(entity =>
            {
                entity.ToView("Trans_Contract_Installment_Payment_VW");
            });

            modelBuilder.Entity<TransContractInstallmentVw>(entity =>
            {
                entity.ToView("Trans_Contract_Installment_VW");
            });

            modelBuilder.Entity<TransContractPrintVw>(entity =>
            {
                entity.ToView("Trans_Contract_Print_VW");

                entity.Property(e => e.TripTime).IsFixedLength();
            });

            modelBuilder.Entity<TransContractTicketVw>(entity =>
            {
                entity.ToView("Trans_Contract_Ticket_VW");
            });

            modelBuilder.Entity<TransContractType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<TransContractVisit>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransContractVisitVw>(entity =>
            {
                entity.ToView("Trans_Contract_Visit_VW");
            });

            modelBuilder.Entity<TransContractVw>(entity =>
            {
                entity.ToView("Trans_Contract_VW");

                entity.Property(e => e.TripTime).IsFixedLength();
            });

            modelBuilder.Entity<TransDriversCarsRef>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransDriversCarsVw>(entity =>
            {
                entity.ToView("Trans_Drivers_Cars_VW");
            });

            modelBuilder.Entity<TransInvoice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransInvoicesInstallment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransInvoicesInstallmentVw>(entity =>
            {
                entity.ToView("Trans_Invoices_Installment_VW");
            });

            modelBuilder.Entity<TransInvoicesType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<TransInvoicesVw>(entity =>
            {
                entity.ToView("Trans_Invoices_VW");
            });

            modelBuilder.Entity<TransKiloMeterProfit>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransLine>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK_Trans_Cars_Line");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransLinePrice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransLinesPrice>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransRoute>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransRouteCity>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransRouteCityVw>(entity =>
            {
                entity.ToView("Trans_Route_City_VW");
            });

            modelBuilder.Entity<TransRouteVw>(entity =>
            {
                entity.ToView("Trans_Route_VW");
            });

            modelBuilder.Entity<TransService>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransServicesType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransServicesTypesVw>(entity =>
            {
                entity.ToView("Trans_Services_Types_VW");
            });

            modelBuilder.Entity<TransSettingsLicensesType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransSmsSetting>(entity =>
            {
                entity.HasKey(e => e.IdMaseg)
                    .HasName("PK_Sys_maseg");
            });

            modelBuilder.Entity<TransTicket>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTicketVw>(entity =>
            {
                entity.ToView("Trans_Ticket_VW");
            });

            modelBuilder.Entity<TransTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTransactionsDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTransactionsDetailsVw>(entity =>
            {
                entity.ToView("Trans_Transactions_Details_VW");
            });

            modelBuilder.Entity<TransTransactionsDiscount>(entity =>
            {
                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTransactionsDiscountVw>(entity =>
            {
                entity.ToView("Trans_Transactions_Discount_vw");

                entity.Property(e => e.DiscountDate).IsFixedLength();

                entity.Property(e => e.DiscountDate2).IsFixedLength();
            });

            modelBuilder.Entity<TransTransactionsPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<TransTransactionsPaymentVw>(entity =>
            {
                entity.ToView("Trans_Transactions_Payment_VW");

                entity.Property(e => e.PaymentDate).IsFixedLength();

                entity.Property(e => e.PaymentDate2).IsFixedLength();
            });

            modelBuilder.Entity<TransTransactionsPrintVw>(entity =>
            {
                entity.ToView("Trans_Transactions_Print_VW");
            });

            modelBuilder.Entity<TransTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<TransTransactionsTypeVw>(entity =>
            {
                entity.ToView("Trans_Transactions_Type_vw");
            });

            modelBuilder.Entity<TransTransactionsVw>(entity =>
            {
                entity.ToView("Trans_Transactions_VW");
            });

            modelBuilder.Entity<TransTrip>(entity =>
            {
                entity.Property(e => e.IsClose).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTripsDetaile>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransTripsDetailesDeliveredVw>(entity =>
            {
                entity.ToView("Trans_Trips_Detailes_Delivered_VW");
            });

            modelBuilder.Entity<TransTripsDetailesVw>(entity =>
            {
                entity.ToView("Trans_Trips_Detailes_VW");
            });

            modelBuilder.Entity<TransTripsVw>(entity =>
            {
                entity.ToView("Trans_Trips_VW");
            });

            modelBuilder.Entity<TransVisit>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TransVisitsVw>(entity =>
            {
                entity.ToView("Trans_Visits_VW");
            });

            modelBuilder.Entity<TransactionEmp2Vw>(entity =>
            {
                entity.ToView("Transaction_Emp_2_VW");
            });

            modelBuilder.Entity<TransactionEmpVw>(entity =>
            {
                entity.ToView("Transaction_Emp_VW");
            });

            modelBuilder.Entity<TrnCourse>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnCourseDegree>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnCourseDegreeVw>(entity =>
            {
                entity.ToView("TRN_Course_Degree_VW");
            });

            modelBuilder.Entity<TrnExpensesDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnExpensesDetailsVw>(entity =>
            {
                entity.ToView("TRN_Expenses_Details_VW");
            });

            modelBuilder.Entity<TrnExpensesM>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnExpensesMAllVw>(entity =>
            {
                entity.ToView("TRN_Expenses_M_All_Vw");
            });

            modelBuilder.Entity<TrnExpensesMVw>(entity =>
            {
                entity.ToView("TRN_Expenses_M_VW");
            });

            modelBuilder.Entity<TrnExpensesType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnExpensesTypeVw>(entity =>
            {
                entity.ToView("TRN_Expenses_Type_VW");
            });

            modelBuilder.Entity<TrnMethodDistributionFee>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnMethodDistributionFeesDetailed>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgram>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramInstallVw>(entity =>
            {
                entity.ToView("TRN_Program_Install_VW");
            });

            modelBuilder.Entity<TrnProgramInstallVw2>(entity =>
            {
                entity.ToView("TRN_Program_Install_VW2");
            });

            modelBuilder.Entity<TrnProgramShortInstallPrintVw>(entity =>
            {
                entity.ToView("TRN_Program_Short_Install_Print_VW");
            });

            modelBuilder.Entity<TrnProgramShortInstallVw>(entity =>
            {
                entity.ToView("TRN_Program_Short_Install_VW");
            });

            modelBuilder.Entity<TrnProgramTrainee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTraineePayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTraineePaymentD>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTraineePaymentDVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainee_Payment_D_VW");
            });

            modelBuilder.Entity<TrnProgramTraineePaymentVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainee_Payment_VW");
            });

            modelBuilder.Entity<TrnProgramTraineesInstall>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTraineesInstallAllVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainees_Install_All_VW");
            });

            modelBuilder.Entity<TrnProgramTraineesInstallVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainees_Install_VW");
            });

            modelBuilder.Entity<TrnProgramTraineesVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainees_VW");
            });

            modelBuilder.Entity<TrnProgramTrainer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTrainerPayment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramTrainersVw>(entity =>
            {
                entity.ToView("TRN_Program_Trainers_VW");

                entity.Property(e => e.TrainerTypeName).IsFixedLength();
            });

            modelBuilder.Entity<TrnProgramsPrintVw>(entity =>
            {
                entity.ToView("TRN_Programs_Print_VW");

                entity.Property(e => e.TypeTrainingName).IsFixedLength();
            });

            modelBuilder.Entity<TrnProgramsType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnProgramsTypeVw>(entity =>
            {
                entity.ToView("TRN_Programs_Type_VW");
            });

            modelBuilder.Entity<TrnProgramsVw>(entity =>
            {
                entity.ToView("TRN_Programs_VW");

                entity.Property(e => e.TypeTrainingName).IsFixedLength();
            });

            modelBuilder.Entity<TrnStudyPlan>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnStudyPlanDegree>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnStudyPlanDegreeVw>(entity =>
            {
                entity.ToView("TRN_Study_Plan_Degree_VW");
            });

            modelBuilder.Entity<TrnStudyPlanDetail>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnStudyPlanDetailsDegreeVw>(entity =>
            {
                entity.ToView("TRN_Study_Plan_Details_Degree_VW");
            });

            modelBuilder.Entity<TrnStudyPlanDetailsVw>(entity =>
            {
                entity.ToView("TRN_Study_Plan_Details_VW");
            });

            modelBuilder.Entity<TrnStudyPlanVw>(entity =>
            {
                entity.ToView("TRN_Study_Plan_VW");
            });

            modelBuilder.Entity<TrnTrainee>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnTraineesVw>(entity =>
            {
                entity.ToView("TRN_Trainees_VW");
            });

            modelBuilder.Entity<TrnTrainer>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TrnTrainerType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TrainerTypeName).IsFixedLength();
            });

            modelBuilder.Entity<TrnTrainersVw>(entity =>
            {
                entity.ToView("TRN_Trainers_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TrnType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TypeName).IsFixedLength();
            });

            modelBuilder.Entity<WebMenu>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.PageId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfAppGroup>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfAppGroupsVw>(entity =>
            {
                entity.ToView("WF_App_Groups_VW");
            });

            modelBuilder.Entity<WfAppType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfAppTypeTable>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfAppTypeVw>(entity =>
            {
                entity.ToView("WF_App_Type_VW");
            });

            modelBuilder.Entity<WfApplication>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfApplicationsAssigne>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfApplicationsAssignesReply>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfApplicationsAssignesReplyVw>(entity =>
            {
                entity.ToView("WF_Applications_Assignes_Reply_VW");
            });

            modelBuilder.Entity<WfApplicationsAssignesVw>(entity =>
            {
                entity.ToView("WF_Applications_Assignes_VW");
            });

            modelBuilder.Entity<WfApplicationsComment>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfApplicationsCommentsVw>(entity =>
            {
                entity.ToView("WF_Applications_Comments_VW");
            });

            modelBuilder.Entity<WfApplicationsStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<WfApplicationsStatusVw>(entity =>
            {
                entity.ToView("WF_Applications_Status_VW");
            });

            modelBuilder.Entity<WfApplicationsVw>(entity =>
            {
                entity.ToView("WF_Applications_VW");
            });

            modelBuilder.Entity<WfDynamicAttribute>(entity =>
            {
                entity.HasKey(e => e.DynamicAttributeId)
                    .HasName("PK_RE_DynamicAttributes");

                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReadOnly).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfDynamicAttributesTable>(entity =>
            {
                entity.Property(e => e.DynamicAttributeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReadOnly).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfDynamicAttributesTableVw>(entity =>
            {
                entity.ToView("WF_DynamicAttributes_Table_VW");
            });

            modelBuilder.Entity<WfDynamicAttributesVw>(entity =>
            {
                entity.ToView("WF_DynamicAttributes_VW");
            });

            modelBuilder.Entity<WfDynamicTableValue>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfDynamicValue>(entity =>
            {
                entity.HasKey(e => e.DynamicValueId)
                    .HasName("PK_RE_DynamicValues_1");

                entity.Property(e => e.DynamicValueId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WfEscalation>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EscalationStatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfEscalationVw>(entity =>
            {
                entity.ToView("WF_Escalation_VW");
            });

            modelBuilder.Entity<WfLookUpCatagory>(entity =>
            {
                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfLookupDataVw>(entity =>
            {
                entity.ToView("WF_lookup_Data_VW");
            });

            modelBuilder.Entity<WfLookupDatum>(entity =>
            {
                entity.Property(e => e.Isdel).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfLookupType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<WfStatus>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfStatusVw>(entity =>
            {
                entity.ToView("WF_Status_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WfStep>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfStepLevel>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<WfStepsNotification>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfStepsTransaction>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WfStepsTransactionsVw>(entity =>
            {
                entity.ToView("WF_Steps_Transactions_VW");
            });

            modelBuilder.Entity<WfStepsVw>(entity =>
            {
                entity.ToView("WF_Steps_VW");
            });

            modelBuilder.Entity<WhAccountType>(entity =>
            {
                entity.Property(e => e.AccountTypeId).ValueGeneratedNever();
            });

            modelBuilder.Entity<WhActualInventorySeriale>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhActualInventorySerialesVw>(entity =>
            {
                entity.ToView("WH_ActualInventory_Seriales_VW");
            });

            modelBuilder.Entity<WhBalanceSheet>(entity =>
            {
                entity.ToView("WH_Balance_Sheet");

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<WhBalanceSheetD>(entity =>
            {
                entity.ToView("WH_Balance_Sheet_D");

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<WhBarcodeType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<WhCloseInventory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhCloseInventoryVw>(entity =>
            {
                entity.ToView("WH_Close_Inventory_VW");
            });

            modelBuilder.Entity<WhInventoriesDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhInventoriesDetailesVw>(entity =>
            {
                entity.ToView("WH_Inventories_Detailes_VW");
            });

            modelBuilder.Entity<WhInventoriesMaster>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhInventoriesMasterVw>(entity =>
            {
                entity.ToView("WH_Inventories_Master_VW");
            });

            modelBuilder.Entity<WhInventoriesStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<WhInventoriesVw>(entity =>
            {
                entity.ToView("WH_Inventories_VW");
            });

            modelBuilder.Entity<WhInventory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<WhInventorySection>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhInventorySectionsEmp>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhInventorySectionsEmpVw>(entity =>
            {
                entity.ToView("WH_Inventory_Sections_Emp_VW");
            });

            modelBuilder.Entity<WhInventorySectionsVw>(entity =>
            {
                entity.ToView("WH_Inventory_Sections_VW");
            });

            modelBuilder.Entity<WhItem>(entity =>
            {
                entity.Property(e => e.AccountId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FacilityId).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.ItemType).HasComment("1 products 2 services");

                entity.Property(e => e.ItemType2).HasComment("1 products 2 services");

                entity.Property(e => e.PriceIncludeVat).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemProperty>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.LookUpCatagoriesId).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemPropertyVw>(entity =>
            {
                entity.ToView("WH_Item_Property_VW");
            });

            modelBuilder.Entity<WhItemTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemTemplateVw>(entity =>
            {
                entity.ToView("WH_Item_Template_VW");
            });

            modelBuilder.Entity<WhItemType2Vw>(entity =>
            {
                entity.ToView("Wh_Item_Type2_VW");
            });

            modelBuilder.Entity<WhItemValuationCosting>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemValuationCostingVw>(entity =>
            {
                entity.ToView("WH_Item_Valuation_Costing_VW");

                entity.Property(e => e.TDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<WhItemsActionsVw>(entity =>
            {
                entity.ToView("WH_Items_Actions_VW");

                entity.Property(e => e.TDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<WhItemsBalance>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsBatch>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsBatchListVw>(entity =>
            {
                entity.ToView("Wh_Items_Batch_List_VW");
            });

            modelBuilder.Entity<WhItemsCatagoriesVw>(entity =>
            {
                entity.ToView("WH_Items_Catagories_VW");
            });

            modelBuilder.Entity<WhItemsCatagory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsColorVw>(entity =>
            {
                entity.ToView("Wh_Items_Color_VW");
            });

            modelBuilder.Entity<WhItemsComponent>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsComponentsVw>(entity =>
            {
                entity.ToView("Wh_Items_Components_VW");
            });

            modelBuilder.Entity<WhItemsInventory>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsInventoryVw>(entity =>
            {
                entity.ToView("WH_Items_Inventory_VW");
            });

            modelBuilder.Entity<WhItemsListVw>(entity =>
            {
                entity.ToView("WH_Items_List_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WhItemsNotificationsVw>(entity =>
            {
                entity.ToView("Wh_Items_Notifications_VW");
            });

            modelBuilder.Entity<WhItemsSection>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsSectionsVw>(entity =>
            {
                entity.ToView("Wh_Items_Sections_VW");
            });

            modelBuilder.Entity<WhItemsSerial>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsSerialsVw>(entity =>
            {
                entity.ToView("WH_Items_Serials_VW");
            });

            modelBuilder.Entity<WhItemsStatusVw>(entity =>
            {
                entity.ToView("Wh_Items_Status_VW");
            });

            modelBuilder.Entity<WhItemsSupplier>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsSupplierVw>(entity =>
            {
                entity.ToView("Wh_Items_Supplier_VW");
            });

            modelBuilder.Entity<WhItemsUnit>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FixedOrVariable).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhItemsUnitListVw>(entity =>
            {
                entity.ToView("Wh_Items_Unit_List_VW");
            });

            modelBuilder.Entity<WhItemsUnitVw>(entity =>
            {
                entity.ToView("Wh_Items_Unit_VW");
            });

            modelBuilder.Entity<WhItemsVw>(entity =>
            {
                entity.ToView("Wh_Items_VW");
            });

            modelBuilder.Entity<WhListTemplateVw>(entity =>
            {
                entity.ToView("WH_List_Template_VW");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WhManufactureCountryVw>(entity =>
            {
                entity.ToView("Wh_Manufacture_Country_VW");
            });

            modelBuilder.Entity<WhManufacturingYearVw>(entity =>
            {
                entity.ToView("Wh_Manufacturing_Year_VW");
            });

            modelBuilder.Entity<WhOrder>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhOrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDId).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhOrderDetailsVw>(entity =>
            {
                entity.ToView("WH_OrderDetails_VW");
            });

            modelBuilder.Entity<WhOrdersVw>(entity =>
            {
                entity.ToView("Wh_Orders_VW");

                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<WhPaymentType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTemplate>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsCarVw>(entity =>
            {
                entity.ToView("Wh_Transactions_Car_VW");
            });

            modelBuilder.Entity<WhTransactionsDetaile>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Equivalent).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsDetailes3Vw>(entity =>
            {
                entity.ToView("WH_Transactions_Detailes3_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsDetailesVw>(entity =>
            {
                entity.ToView("WH_Transactions_Detailes_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsExpense>(entity =>
            {
                entity.Property(e => e.CcId).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsMaster>(entity =>
            {
                entity.HasKey(e => e.TId)
                    .HasName("PK_WH_Transaction_Master");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DirectTransfer).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();

                entity.Property(e => e.TDateHijri1).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsMasterVw>(entity =>
            {
                entity.ToView("WH_Transactions_Master_VW");

                entity.Property(e => e.ChequDateHijri).IsFixedLength();

                entity.Property(e => e.TDateGregorian).IsFixedLength();

                entity.Property(e => e.TDateHijri).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsReferenceType>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsReferenceTypeVw>(entity =>
            {
                entity.ToView("WH_Transactions_Reference_Type_VW");
            });

            modelBuilder.Entity<WhTransactionsReservationVw>(entity =>
            {
                entity.ToView("WH_Transactions_Reservation_VW");

                entity.Property(e => e.TDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsSalesVw>(entity =>
            {
                entity.ToView("WH_Transactions_Sales_VW");
            });

            modelBuilder.Entity<WhTransactionsScheduling>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<WhTransactionsSchedulingVw>(entity =>
            {
                entity.ToView("WH_Transactions_Scheduling_VW");
            });

            modelBuilder.Entity<WhTransactionsSeriale>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsSerialesVw>(entity =>
            {
                entity.ToView("WH_Transactions_Seriales_VW");

                entity.Property(e => e.TDateGregorian).IsFixedLength();
            });

            modelBuilder.Entity<WhTransactionsStaff>(entity =>
            {
                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsStaffVw>(entity =>
            {
                entity.ToView("WH_Transactions_Staff_VW");
            });

            modelBuilder.Entity<WhTransactionsStatus>(entity =>
            {
                entity.Property(e => e.StatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<WhTransactionsType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<WhTransactionsTypeVw>(entity =>
            {
                entity.ToView("WH_Transactions_Type_VW");
            });

            modelBuilder.Entity<WhTransferMethodVw>(entity =>
            {
                entity.ToView("Wh_Transfer_Method_VW");
            });

            modelBuilder.Entity<WhTransferTypeVw>(entity =>
            {
                entity.ToView("Wh_Transfer_Type_VW");
            });

            modelBuilder.Entity<WhUnit>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}*/
