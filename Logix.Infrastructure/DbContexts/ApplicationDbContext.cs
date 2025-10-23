using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Domain.ACC;
using Logix.Domain.Base;
using Logix.Domain.CRM;
using Logix.Domain.FXA;
using Logix.Domain.Gb;
using Logix.Domain.GB;
using Logix.Domain.HD;
using Logix.Domain.HOT;
using Logix.Domain.Hr;
using Logix.Domain.HR;
using Logix.Domain.Integra;
using Logix.Domain.Main;
using Logix.Domain.Maintenance;
using Logix.Domain.OPM;
using Logix.Domain.PM;
using Logix.Domain.PUR;
using Logix.Domain.RE;
using Logix.Domain.RPT;
using Logix.Domain.SAL;
using Logix.Domain.Sch;
using Logix.Domain.Trans;
using Logix.Domain.TS;
using Logix.Domain.WA;
using Logix.Domain.WF;
using Logix.Domain.WH;
using Logix.Infrastructure.EntityConfigurations.ACC;
using Logix.Infrastructure.EntityConfigurations.CRM;
using Logix.Infrastructure.EntityConfigurations.FXA;
using Logix.Infrastructure.EntityConfigurations.GB;
using Logix.Infrastructure.EntityConfigurations.HD;
using Logix.Infrastructure.EntityConfigurations.HOT;
using Logix.Infrastructure.EntityConfigurations.HR;
using Logix.Infrastructure.EntityConfigurations.Integra;
using Logix.Infrastructure.EntityConfigurations.Main;
using Logix.Infrastructure.EntityConfigurations.Maintenance;
using Logix.Infrastructure.EntityConfigurations.OPM;
using Logix.Infrastructure.EntityConfigurations.PM;
using Logix.Infrastructure.EntityConfigurations.PUR;
using Logix.Infrastructure.EntityConfigurations.RE;
using Logix.Infrastructure.EntityConfigurations.RPT;
using Logix.Infrastructure.EntityConfigurations.SAL;
using Logix.Infrastructure.EntityConfigurations.Sch;
using Logix.Infrastructure.EntityConfigurations.Trans;
using Logix.Infrastructure.EntityConfigurations.TS;
using Logix.Infrastructure.EntityConfigurations.WA;
using Logix.Infrastructure.EntityConfigurations.WF;
using Logix.Infrastructure.EntityConfigurations.WH;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.DbContexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentData currentData;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentData currentData) : base(options)
        {
            this.currentData = currentData;
        }


        /*   Each region is for one system entities   */

        #region ============ Main ===========================================================================
        public DbSet<SysCalendar> SysCalendars { get; private set; }
        public DbSet<SysNotificationsSetting> SysNotificationsSettings { get; private set; }
        public DbSet<SysNotificationsSettingVw> SysNotificationsSettingVws { get; private set; }
        public DbSet<SysSystem> SysSystems { get; private set; }
        public DbSet<SysScreen> SysScreens { get; private set; }
        public DbSet<SysAnnouncement> SysAnnouncements { get; private set; }
        public DbSet<SysAnnouncementVw> SysAnnouncementVws { get; private set; }
        public DbSet<SysAnnouncementLocationVw> SysAnnouncementLocationVws { get; private set; }
        public DbSet<SysLookupCategory> SysLookupCategories { get; private set; }
        public DbSet<SysLookupData> SysLookupData { get; private set; }
        public DbSet<SysLookupDataVw> SysLookupDataVws { get; private set; }
        public DbSet<SysDepartment> SysDepartments { get; private set; }
        public DbSet<SysDepartmentVw> SysDepartmentVws { get; private set; }
        public DbSet<SysDepartmentCatagory> SysDepartmentCatagories { get; private set; }
        public DbSet<SysGroup> SysGroups { get; private set; }
        public DbSet<SysGroupVw> SysGroupVws { get; private set; }
        public DbSet<SysScreenPermission> SysScreenPermissions { get; private set; }
        public DbSet<SysScreenPermissionVw> SysScreenPermissionVws { get; private set; }
        public DbSet<MainListDto> MainListDtos { get; private set; }
        public DbSet<SubListDto> SubListDtos { get; private set; }
        public DbSet<SysBranchVw> SysBranchVws { get; private set; }
        public DbSet<SysNotification> SysNotifications { get; private set; }
        public DbSet<SysNotificationsVw> SysNotificationsVws { get; private set; }
        public DbSet<SysProperty> SysProperties { get; private set; }
        public DbSet<SysPropertiesVw> SysPropertiesVws { get; private set; }
        public DbSet<SysPropertyValue> SysPropertyValues { get; private set; }
        public DbSet<SysPropertyValuesVw> SysPropertyValuesVws { get; private set; }
        public DbSet<SysScreenProperty> SysScreenProperties { get; private set; }
        public DbSet<SysScreenPermissionProperty> SysScreenPermissionProperties { get; private set; }
        public DbSet<SysScreenPermissionPropertiesVw> SysScreenPermissionPropertiesVws { get; private set; }

        public DbSet<SysCustomerType> SysCustomerTypes { get; private set; }
        public DbSet<SysCustomerGroup> SysCustomerGroups { get; private set; }
        public DbSet<SysCustomerGroupAccount> SysCustomerGroupAccounts { get; private set; }
        public DbSet<SysCustomerGroupAccountsVw> SysCustomerGroupAccountsVws { get; private set; }
        public DbSet<SysLicense> SysLicenses { get; private set; }
        public DbSet<SysLicensesVw> SysLicensesVws { get; private set; }
        public DbSet<SysFavMenu> SysFavMenus { get; private set; }
        public DbSet<SysFile> SysFiles { get; private set; }
        public DbSet<SysCustomer> SysCustomer { get; private set; }

        public DbSet<InvestBranch> InvestBranches { get; set; } = null!;
        public DbSet<InvestBranchVw> InvestBranceshVws { get; set; } = null!;
        public DbSet<SysCurrency> SysCurrency { get; private set; } = null!;
        public DbSet<SysCurrencyListVw> SysCurrencyListVws { get; private set; } = null!;

        public DbSet<SysExchangeRate> SysExchangeRates { get; private set; } = null!;
        public DbSet<SysExchangeRateVw> SysExchangeRatesVws { get; private set; } = null!;
        public DbSet<SysExchangeRateListVW> SysExchangeRateListsVws { get; private set; } = null!;

        public DbSet<SysUser> SysUsers { get; private set; }
        public DbSet<SysUserVw> SysUserVws { get; private set; }
        public DbSet<SysScreenInstalled> SysScreenInstalleds { get; private set; }
        public DbSet<SysScreenInstalledVw> SysScreenInstalledVws { get; private set; }


        public DbSet<SysCustomerBranch> SysCustomerBranches { get; private set; }
        public DbSet<SysCustomerBranchVw> SysCustomerBranchVws { get; private set; }
        public DbSet<SysCites> SysCites { get; private set; }
        public DbSet<SysCustomerCoType> SysCustomerCoTypes { get; private set; }

        public DbSet<SysPoliciesProcedure> SysPoliciesProcedures { get; private set; }
        public DbSet<SysPoliciesProceduresVw> SysPoliciesProceduresVws { get; private set; }
        public DbSet<SysVatGroup> SysVatGroups { get; private set; }
        public DbSet<SysCustomerVw> SysCustomerVws { get; private set; }

        public DbSet<SysCustomerContact> SysCustomerContacts { get; private set; }
        public DbSet<SysCustomerContactVw> SysCustomerContactVws { get; private set; }
        public DbSet<SysCustomerFile> SysCustomerFiles { get; private set; }
        public DbSet<SysCustomerFilesVw> SysCustomerFilesVws { get; private set; }

        public DbSet<SysTemplate> SysTemplates { get; private set; }
        public DbSet<SysTemplateVw> SysTemplateVws { get; private set; }
        public DbSet<SysForm> SysForms { get; private set; }
        public DbSet<SysFormsVw> SysFormsVws { get; private set; }

        public DbSet<SysSettingExport> SysSettingExports { get; private set; }
        public DbSet<SysSettingExportVw> SysSettingExportVws { get; private set; }

        public DbSet<SysActivityLog> SysActivityLogs { get; private set; }
        public DbSet<SysActivityLogVw> SysActivityLogVws { get; private set; }

        public DbSet<Domain.Main.InvestEmployee> InvestEmployees { get; private set; }
        public DbSet<InvestEmployeeVvw> InvestEmployeeVvws { get; private set; }
        public DbSet<SysUserLogTime> SysUserLogTimes { get; private set; }
        public DbSet<SysUserLogTimeVw> SysUserLogTimeVws { get; private set; }

        public DbSet<SysUserTracking> SysUserTrackings { get; private set; }
        public DbSet<SysUserTrackingVw> SysUserTrackingVws { get; private set; }

        public DbSet<SysUserType> SysUserTypes { get; private set; }
        public DbSet<SysUserType2> SysUserTypes2 { get; private set; }

        public DbSet<SysVersion> SysVersions { get; private set; }

        public DbSet<SysDynamicAttribute> SysDynamicAttributes { get; private set; }
        public DbSet<SysDynamicAttributesVw> SysDynamicAttributesVws { get; private set; }
        public DbSet<SysDynamicAttributeDataType> SysDynamicAttributeDataTypes { get; private set; }
        public DbSet<SysScreenVw> SysScreenVws { get; private set; }
        public DbSet<SysVatGroupVw> SysVatGroupVws { get; private set; }
        public DbSet<MonthDay> MonthDays { get; private set; }
        public DbSet<SysMailServer> SysMailServer { get; private set; }
        public DbSet<SysCountry> SysCountrys { get; private set; }
        public DbSet<SysCountryVw> SysCountryVws { get; private set; }

        public DbSet<SysNotificationsMang> SysNotificationsMangs { get; private set; }
        public DbSet<SysNotificationsMangVw> SysNotificationsMangVws { get; private set; }

        public DbSet<SysTable> SysTables { get; private set; }
        public DbSet<SysTableField> SysTableFields { get; private set; }

        public DbSet<SysActivityType> SysActivityTypes { get; private set; }
        public DbSet<SysPackagesPropertyValue> SysPackagesPropertyValue { get; private set; }
        public DbSet<SysPackage> SysPackages { get; private set; }

        public DbSet<SysSms> SysSms { get; private set; }

        public DbSet<SysWebHook> SysWebHooks { get; private set; }
        public DbSet<SysWebHookVw> SysWebHookVws { get; private set; }

        public DbSet<SysFilesDocument> SysFilesDocuments { get; private set; }
        public DbSet<SysFilesDocumentVw> SysFilesDocumentVws { get; private set; }
        public DbSet<SysRecordWebhookVw> SysRecordWebhookVws { get; private set; }
        public DbSet<SysRecordWebhook> SysRecordWebhooks { get; private set; }
        public DbSet<SysDynamicValue> SysDynamicValues { get; private set; }
        public DbSet<SysScreenWorkflow> SysScreenWorkflows { get; private set; }
        public DbSet<SysPeriod> SysPeriods { get; private set; }
        public DbSet<SysLibraryFile> SysLibraryFiles { get; private set; }
        public DbSet<SysLibraryFilesVw> SysLibraryFilesVws { get; private set; }
        public DbSet<SysResetPassword> SysResetPasswords { get; private set; }
        public DbSet<SysCreateUserRequst> SysCreateUserRequsts { get; private set; }
        public DbSet<SysCreateUserRequstVw> SysCreateUserRequstVws { get; private set; }
        public DbSet<ChatMessage> ChatMessages { get; private set; }
        public DbSet<SysMethodTypeApi> SysMethodTypeApis { get; private set; }
        public DbSet<SysProcessScreenWebHook> SysProcessScreenWebHooks { get; private set; }
        public DbSet<SysWebHookAuth> SysWebHookAuths { get; private set; }
        public DbSet<SysRecordWebhookAuth> SysRecordWebhookAuths { get; private set; }
        public DbSet<SysRecordWebhookAuthVw> SysRecordWebhookAuthVws { get; private set; }
        public DbSet<SysWebHookAuthVw> SysWebHookAuthVws { get; private set; }

        public DbSet<SysInvoiceAccordingType> SysInvoiceAccordingTypes { get; private set; }
        public DbSet<SysZatcaInvoiceType> SysZatcaInvoiceTypes { get; private set; }

        public DbSet<SysZatcaInvoiceTransaction> SysZatcaInvoiceTransactions { get; private set; }
        public DbSet<SysZatcaInvoiceTransactionsSimulation> SysZatcaInvoiceTransactionsSimulations { get; private set; }
        public DbSet<SysZatcaReportingResult> SysZatcaReportingResults { get; private set; }
        public DbSet<SysZatcaReportingResultsSimulation> SysZatcaReportingResultsSimulations { get; private set; }
        public DbSet<SysZatcaSignedXml> SysZatcaSignedXmls { get; private set; }
        public DbSet<SysZatcaSignedXmlSimulation> SysZatcaSignedXmlSimulations { get; private set; }
        public DbSet<ZatcaCreditDebitNote> ZatcaCreditDebitNotes { get; private set; }
        public DbSet<ZatcaVatcategoriesReason> ZatcaVatcategoriesReasons { get; private set; }
        public DbSet<SysPropertyClassification> SysPropertyClassifications { get; private set; }
        public DbSet<SysCustomersFilesSetting> SysCustomersFilesSettings { get; private set; }
        public DbSet<SysCustomersFilesSettingsVw> SysCustomersFilesSettingsVws { get; private set; }

        #endregion ------------------ End Main ---------------------------------------------------------------

        #region ======= PM ========================

        //==propsPattern==
        public DbSet<PmJobsSalary> pmJobsSalaries { get; private set; } = null!;
        public DbSet<PmJobsSalaryVw> PmJobsSalaryVws { get; private set; } = null!;
        public DbSet<PmOperationalControl> PmOperationalControls { get; private set; } = null!;
        public DbSet<PmOperationalControlsVw> PmOperationalControlsVws { get; private set; } = null!;
        public DbSet<PMProjects> PMProjects { get; private set; } = null!;
        public DbSet<PmProjectsVw> PmProjectsVws { get; private set; }
        public DbSet<PmExtractAdditionalType> PmExtractAdditionalTypes { get; private set; }
        public DbSet<PmExtractAdditionalTypeVw> PmExtractAdditionalTypeVws { get; private set; }

        public DbSet<PmProjectsStage> PmProjectsStages { get; private set; }
        public DbSet<PmProjectsStagesVw> PmProjectsStagesVws { get; private set; }
        public DbSet<PMProjectsStaff> PmProjectsStaffs { get; private set; }
        public DbSet<PmProjectsStaffVw> PmProjectsStaffVws { get; private set; }

        public DbSet<PmProjectPlan> PmProjectPlans { get; private set; }
        public DbSet<PmProjectPlansVw> PmProjectPlansVws { get; private set; }
        public DbSet<PmProjectsAddDeduc> PmProjectsAddDeducs { get; private set; }
        public DbSet<PmProjectsAddDeducVw> PmProjectsAddDeducVws { get; private set; }
        public DbSet<PmProjectsFile> PmProjectsFiles { get; private set; }
        public DbSet<PmProjectsFilesVw> PmProjectsFilesVws { get; private set; }
        public DbSet<PmProjectsInstallment> PmProjectsInstallments { get; private set; }
        public DbSet<PmProjectsInstallmentVw> PmProjectsInstallmentVws { get; private set; }
        public DbSet<PmProjectsInstallmentAction> PmProjectsInstallmentActions { get; private set; }
        public DbSet<PmProjectsInstallmentActionVw> PmProjectsInstallmentActionVws { get; private set; }
        public DbSet<PmProjectsInstallmentPayment> PmProjectsInstallmentPayments { get; private set; }
        public DbSet<PmProjectsInstallmentPaymentVw> PmProjectsInstallmentPaymentVws { get; private set; }
        public DbSet<PmProjectsItem> PmProjectsItems { get; private set; }
        public DbSet<PMProjectsItemsVw> PmProjectsItemsVws { get; private set; }
        public DbSet<PmProjectsRisk> PmProjectsRisks { get; private set; }
        public DbSet<PmProjectsRisksVw> PmProjectsRiskVws { get; private set; }
        public DbSet<PmProjectsRisksVw2> PmProjectsRisksVw2s { get; private set; }

        public DbSet<PmProjectsStaffType> PmProjectsStaffTypes { get; private set; }

        public DbSet<PmProjectsStokeholder> PmProjectsStokeholders { get; private set; }
        public DbSet<PmProjectsStokeholderVw> PmProjectsStokeholderVws { get; private set; }
        public DbSet<PmProjectsType> PmProjectsTypes { get; private set; }
        public DbSet<PmProjectsTypeVw> PmProjectsTypeVws { get; private set; }
        public DbSet<PmProjectsStatusVw> PmProjectsStatusVws { get; private set; }
        public DbSet<PmProjectsStatus> PmProjectsStatuses { get; private set; }

        public DbSet<PmProjectStatusVw> PmProjectStatusVws { get; private set; }
        public DbSet<PmDurationTypeVw> PmDurationTypeVws { get; private set; }

        public DbSet<PmSetting> PmSettings { get; private set; }


        public DbSet<PmExtractTransactionsChangeStatus> PmExtractTransactionsChangeStatus { get; private set; }
        public DbSet<PmExtractTransactionsChangeStatusVw> PmExtractTransactionsChangeStatusVws { get; private set; }
        public DbSet<PmExtractTransaction> PmExtractTransactions { get; private set; }
        public DbSet<PmExtractTransactionsVw> PmExtractTransactionsVws { get; private set; }

        public DbSet<PmExtractTransactionsProduct> PmExtractTransactionsProducts { get; private set; }
        public DbSet<PmExtractTransactionsProductsVw> PmExtractTransactionsProductsVws { get; private set; }
        public DbSet<PmExtractTransactionsType> PmExtractTransactionsTypes { get; private set; }
        public DbSet<PmExtractTransactionsAdditional> PmExtractTransactionsAdditionals { get; private set; }
        public DbSet<PmExtractTransactionsPayment> PmExtractTransactionsPayments { get; private set; }
        public DbSet<PmExtractTransactionsPaymentVw> PmExtractTransactionsPaymentVws { get; private set; }
        public DbSet<PmProjectsEditVw> PmProjectsEditVws { get; private set; }
        public DbSet<PmExtractTransactionsStatus> PmExtractTransactionsStatuses { get; private set; }
        public DbSet<PmRiskImpact> PmRiskImpacts { get; private set; }
        public DbSet<PmRiskEffect> PmRiskEffects { get; private set; }
        public DbSet<PmProjectsBudgetItem> PmProjectsBudgetItems { get; private set; }
        public DbSet<PmProjectsBudgetItemsVw> PmProjectsBudgetItemsVws { get; private set; }
        public DbSet<PmProjectsObjective> PmProjectsObjectives { get; private set; }
        public DbSet<PmProjectsStrategicLink> PmProjectsStrategicLinks { get; private set; }
        public DbSet<PmProjectsStrategicLinkVw> PmProjectsStrategicLinkVws { get; private set; }
        public DbSet<PmProjectsInterconnection> PmProjectsInterconnections { get; private set; }
        public DbSet<PmProjectsInterconnectionVw> PmProjectsInterconnectionVws { get; private set; }
        public DbSet<PmProjectsPerformanceIndicator> PmProjectsPerformanceIndicators { get; private set; }
        public DbSet<PmDeliverableTransactionsDetailsVw> PmDeliverableTransactionsDetailsVws { get; private set; }
        public DbSet<PmDeliverableTransactionsDetail> PmDeliverableTransactionsDetails { get; private set; }
        public DbSet<PmProjectsDeliverable> PmProjectsDeliverables { get; private set; }
        public DbSet<PmProjectsDeliverableVw> PmProjectsDeliverableVws { get; private set; }
        public DbSet<PmProjectsAssumption> PmProjectsAssumptions { get; private set; }
        public DbSet<PmProjectsGovernance> PmProjectsGovernances { get; private set; }
        public DbSet<PmProjectsResource> PmProjectsResources { get; private set; }
        public DbSet<PmProjectsResourcesVw> PmProjectsResourcesVws { get; private set; }
        public DbSet<PmProjectsDeliverableAcceptCriteriaVw> PmProjectsDeliverableAcceptCriteriaVws { get; private set; }
        public DbSet<PmProjectsDeliverableAcceptCriterion> PmProjectsDeliverableAcceptCriterions { get; private set; }
        public DbSet<PmProjectStepsVw> PmProjectStepsVws { get; private set; }
        public DbSet<PmChangeRequest> PmChangeRequests { get; private set; }
        public DbSet<PmChangeRequestVw> PmChangeRequestVws { get; private set; }
        public DbSet<PmChangeRequestItem> PmChangeRequestItems { get; private set; }
        public DbSet<PmChangeRequestItemsVw> PmChangeRequestItemsVws { get; private set; }
        public DbSet<PmDeliverableTransaction> PmDeliverableTransactions { get; private set; }
        public DbSet<PmDeliverableTransactionsVw> PmDeliverableTransactionsVw { get; private set; }
        public DbSet<PmProjectsLessonsLearnedDetail> PmProjectsLessonsLearnedDetails { get; private set; }
        public DbSet<PmProjectsLessonsLearnedDetailsVw> PmProjectsLessonsLearnedDetailsVws { get; private set; }
        public DbSet<PmProjectsLessonsLearnedMaster> PmProjectsLessonsLearnedMasters { get; private set; }
        public DbSet<PmProjectsLessonsLearnedMasterVw> PmProjectsLessonsLearnedMasterVws { get; private set; }
        public DbSet<PmProjectsClosingVw> PmProjectsClosingVws { get; private set; }
        public DbSet<PmProjectsClosing> PmProjectsClosings { get; private set; }
        public DbSet<PmDeliverablesTrackingStatus> PmDeliverablesTrackingStatuses { get; private set; }
        public DbSet<PmDeliverablesTrackingStatusVw> PmDeliverablesTrackingStatusVws { get; private set; }
        public DbSet<PmKickOff> PmKickOffs { get; private set; }
        public DbSet<PmKickOffVw> PmKickOffVws { get; private set; }
        public DbSet<PmProjectsStatementRequest> PmProjectsStatementRequests { get; private set; }
        public DbSet<PmProjectsStatementRequestVw> PmProjectsStatementRequestVws { get; private set; }
        public DbSet<PmExtractTransactionsDiscount> PmExtractTransactionsDiscounts { get; private set; }
        public DbSet<PmExtractTransactionsDiscountVw> PmExtractTransactionsDiscountVws { get; private set; }
        public DbSet<PmProjectsGuarantee> PmProjectsGuarantees { get; private set; }
        public DbSet<PmProjectsGuaranteeVw> PmProjectsGuaranteeVws { get; private set; }
        public DbSet<PmProjectsListVw> PmProjectsListVws { get; private set; }

        public DbSet<PmTransactionsInstallment> PmTransactionsInstallments { get; private set; }
        public DbSet<PmTransactionsInstallmentsVw> PmTransactionsInstallmentsVws { get; private set; }

        public DbSet<PmTransaction> PmTransactions { get; private set; }
        public DbSet<PmTransactionsVw> PmTransactionsVws { get; private set; }



        #endregion ======= PM ========================

        #region ============ RPT ==================
        public DbSet<RptReport> RptReports { get; private set; }
        public DbSet<RptCustomReport> RptCustomReports { get; private set; }

        public DbSet<RptField> RptFields { get; private set; }
        public DbSet<RptFieldsVw> RptFieldsVws { get; private set; }
        public DbSet<RptTable> RptTables { get; private set; }
        public DbSet<RptReportsField> RptReportsField { get; private set; }
        public DbSet<RptReportsFieldsVw> RptReportsFieldsVw { get; private set; }
        public DbSet<RptOperator> RptOperator { get; private set; }
        public DbSet<DynamicQueryResult> DynamicQueryResults { get; private set; }
        public DbSet<RptReportsVw> RptReportsVw { get; private set; }
        public DbSet<RptReportsOrderByVw> RptReportsOrderByVw { get; private set; }
        public DbSet<RptPowerBiconfig> RptPowerBiconfig { get; private set; }

        #endregion --------- End RPT ----------------

        #region ============ HR ==============================================================================

        public DbSet<HrAllowanceVw> HrAllowanceVws { get; private set; }
        public DbSet<HrDeductionVw> HrDeductionVws { get; private set; }
        public DbSet<HrDisciplinaryActionType> HrDisciplinaryActionTypes { get; private set; }
        public DbSet<HrAttTimeTableDay> HrAttTimeTableDays { get; private set; }
        public DbSet<HrAttLocation> HrAttLocations { get; private set; }
        public DbSet<HrEmployee> HrEmployees { get; private set; }
        public DbSet<HrEmployeeVw> HrEmployeeVws { get; private set; }
        public DbSet<HrAttDay> HrAttDays { get; private set; }
        public DbSet<HrEvaluationAnnualIncreaseConfig> HrEvaluationAnnualIncreaseConfigs { get; private set; }
        public DbSet<HrNotification> HrNotifications { get; private set; }
        public DbSet<HrNotificationsVw> HrNotificationsVws { get; private set; }
        public DbSet<HrCompetence> HrCompetences { get; private set; }
        public DbSet<HrCompetencesVw> HrCompetencesVws { get; private set; }
        public DbSet<HrCompetencesCatagory> HrCompetencesCatagorys { get; private set; }
        public DbSet<HrKpiTemplatesCompetence> HrKpiTemplatesCompetences { get; private set; }
        public DbSet<HrTrainingBag> HrTrainingBags { get; private set; }
        public DbSet<HrTrainingBagVw> HrTrainingBagVws { get; private set; }

        public DbSet<HrPolicy> HrPolicys { get; private set; }
        public DbSet<HrPoliciesVw> HrPoliciesVws { get; private set; }

        public DbSet<HrPoliciesType> HrPoliciesTypes { get; private set; }

        public DbSet<HrKpiTemplate> HrKpiTemplates { get; private set; }
        public DbSet<HrKpiTemplatesVw> HrKpiTemplatesVws { get; private set; }
        public DbSet<HrSetting> HrSettings { get; private set; }
        public DbSet<HrSalaryGroup> HrSalaryGroups { get; private set; }
        public DbSet<HrSalaryGroupVw> HrSalaryGroupVws { get; private set; }
        public DbSet<HrVacationsType> HrVacationsTypes { get; private set; }
        public DbSet<HrVacationsTypeVw> HrVacationsTypeVws { get; private set; }
        public DbSet<HrDisciplinaryCase> HrDisciplinaryCases { get; private set; }
        public DbSet<HrAbsence> HrAbsences { get; private set; }
        public DbSet<HrDelay> HrDelays { get; private set; }
        public DbSet<HrNote> HrNotes { get; private set; }
        public DbSet<HrNoteVw> HrNoteVws { get; private set; }


        //public DbSet<HrDisciplinaryCase> HrDisciplinaryCases { get; private set; } 
        public DbSet<HrCardTemplate> HrCardTemplates { get; private set; }
        public DbSet<InvestMonth> investMonths { get; private set; }
        public DbSet<HrOverTimeD> hrOverTimeDs { get; private set; }
        public DbSet<HrOverTimeDVw> hrOverTimeDVws { get; private set; }

        public DbSet<HrAttShift> HrAttShifts { get; private set; }
        public DbSet<HrJobProgramVw> HrJobProgramVws { get; private set; }
        public DbSet<HrJobVw> HrJobVws { get; private set; }

        public DbSet<HrAttendanceReportDto> HrAttendanceReportDto { get; private set; }
        public DbSet<HrVacation> HrVacations { get; private set; }
        public DbSet<HrVacationsVw> HrVacationsVws { get; private set; }
        public DbSet<HrAttendance> HrAttendances { get; private set; }
        public DbSet<HrAttendancesVw> HrAttendancesVws { get; private set; }
        public DbSet<HrAttTimeTable> HrAttTimeTables { get; private set; }
        public DbSet<HrAttTimeTableVw> HrAttTimeTableVws { get; private set; }
        public DbSet<HrAttShiftEmployee> HrAttShiftEmployees { get; private set; }
        public DbSet<HrAttShiftEmployeeVw> HrAttShiftEmployeeVws { get; private set; }
        public DbSet<HrMandate> HrMandates { get; private set; }
        public DbSet<HrMandateVw> HrMandateVws { get; private set; }
        public DbSet<HrKpiType> HrKpiTypes { get; private set; }
        public DbSet<HrKpiTemplatesCompetencesVw> HrKpiTemplatesCompetencesVws { get; private set; }
        public DbSet<HrDisciplinaryCaseAction> HrDisciplinaryCaseActions { get; private set; }
        public DbSet<HrDisciplinaryCaseActionVw> HrDisciplinaryCaseActionVws { get; private set; }
        public DbSet<HrDisciplinaryRule> HrDisciplinaryRules { get; private set; }
        public DbSet<HrDisciplinaryRuleVw> HrDisciplinaryRuleVws { get; private set; }
        public DbSet<HrRateType> HrRateTypes { get; private set; }
        public DbSet<HrRateTypeVw> HrRateTypeVws { get; private set; }
        public DbSet<HrVacationsCatagory> HrVacationsCatagories { get; private set; }
        public DbSet<HrAllowanceDeduction> HrAllowanceDeductions { get; private set; }
        public DbSet<HrAllowanceDeductionVw> HrAllowanceDeductionVws { get; private set; }
        public DbSet<HrLoanVw> HrLoanVws { get; private set; }
        public DbSet<HrLoan> HrLoans { get; private set; }
        public DbSet<HrAbsenceVw> HrAbsenceVw { get; private set; }
        public DbSet<HrPayrollDVw> HrPayrollDVw { get; private set; }
        public DbSet<HrPayrollD> HrPayrollDs { get; private set; }
        public DbSet<HrArchivesFile> HrArchivesFiles { get; private set; }
        public DbSet<HrArchivesFilesVw> HrArchivesFilesVws { get; private set; }
        public DbSet<HrLicense> HrLicenses { get; private set; }
        public DbSet<HrLicensesVw> HrLicensesVws { get; private set; }
        public DbSet<HrTransfer> HrTransfers { get; private set; }
        public DbSet<HrTransfersVw> HrTransfersVws { get; private set; }
        public DbSet<HrOverTimeM> HrOverTimeMs { get; private set; }
        public DbSet<HrOverTimeMVw> HrOverTimeMVws { get; private set; }
        public DbSet<HrOhadDetail> HrOhadDetails { get; private set; }
        public DbSet<HrOhadDetailsVw> HrOhadDetailsVws { get; private set; }
        public DbSet<HrEmpWarn> HrEmpWarns { get; private set; }
        public DbSet<HrEmpWarnVw> HrEmpWarnVws { get; private set; }
        public DbSet<HrVacationBalance> HrVacationBalances { get; private set; }
        public DbSet<HrVacationBalanceVw> HrVacationBalanceVws { get; private set; }
        public DbSet<HrDependent> HrDependents { get; private set; }
        public DbSet<HrDependentsVw> HrDependentsVws { get; private set; }
        public DbSet<HrDirectJob> HrDirectJobs { get; private set; }
        public DbSet<HrDirectJobVw> HrDirectJobVws { get; private set; }
        public DbSet<HrIncrement> HrIncrements { get; private set; }
        public DbSet<HrIncrementsVw> HrIncrementsVws { get; private set; }
        public DbSet<HrLeave> HrLeaves { get; private set; }
        public DbSet<HrLeaveVw> HrLeaveVws { get; private set; }
        public DbSet<HrKpi> HrKpis { get; private set; }
        public DbSet<HrKpiVw> HrKpiVws { get; private set; }
        public DbSet<HrKpiDetaile> HrKpiDetailes { get; private set; }
        public DbSet<HrKpiDetailesVw> HrKpiDetailesVws { get; private set; }
        public DbSet<HrEmpWorkTime> HrEmpWorkTimes { get; private set; }
        public DbSet<HrEmpWorkTimeVw> HrEmpWorkTimeVws { get; private set; }
        public DbSet<HrSalaryGroupAccount> HrSalaryGroupAccounts { get; private set; }
        public DbSet<HrSalaryGroupRefrance> HrSalaryGroupRefrances { get; private set; }
        public DbSet<HrSalaryGroupRefranceVw> HrSalaryGroupRefranceVws { get; private set; }
        public DbSet<HrSalaryGroupAllowanceVw> HrSalaryGroupAllowanceVws { get; private set; }
        public DbSet<HrSalaryGroupDeductionVw> HrSalaryGroupDeductionVws { get; private set; }
        public DbSet<HrNotificationsTypeVw> HrNotificationsTypeVws { get; private set; }
        public DbSet<HrNotificationsType> HrNotificationsTypes { get; private set; }
        public DbSet<HrNotificationsSetting> HrNotificationsSettings { get; private set; }
        public DbSet<HrNotificationsSettingVw> HrNotificationsSettingVws { get; private set; }


        public DbSet<HrAllowanceDeductionM> HrAllowanceDeductionMs { get; private set; }
        public DbSet<HrAllowanceDeductionTempOrFix> HrAllowanceDeductionTempOrFixs { get; private set; }
        public DbSet<HrArchiveFilesDetail> HrArchiveFilesDetails { get; private set; }
        public DbSet<HrArchiveFilesDetailsVw> HrArchiveFilesDetailsVws { get; private set; }
        public DbSet<HrAssignman> HrAssignmans { get; private set; }
        public DbSet<HrAssignmenVw> HrAssignmenVws { get; private set; }
        public DbSet<HrAttAction> HrAttActions { get; private set; }
        public DbSet<HrAttLocationEmployee> HrAttLocationEmployees { get; private set; }
        public DbSet<HrAttLocationEmployeeVw> HrAttLocationEmployeeVws { get; private set; }
        public DbSet<HrAttShiftClose> HrAttShiftCloses { get; private set; }
        public DbSet<HrAttShiftCloseVw> HrAttShiftCloseVws { get; private set; }
        public DbSet<HrAttShiftCloseD> HrAttShiftCloseDs { get; private set; }
        public DbSet<HrAuthorization> HrAuthorizations { get; private set; }
        public DbSet<HrAuthorizationVw> HrAuthorizationVws { get; private set; }
        public DbSet<HrAttendanceBioTime> HrAttendanceBioTimes { get; private set; }
        public DbSet<HrCheckInOut> HrCheckInOuts { get; private set; }
        public DbSet<HrCheckInOutVw> HrCheckInOutVws { get; private set; }

        public DbSet<HrClearance> HrClearances { get; private set; }
        public DbSet<HrClearanceVw> HrClearanceVws { get; private set; }
        public DbSet<HrClearanceType> HrClearanceTypes { get; private set; }
        public DbSet<HrClearanceTypeVw> HrClearanceTypeVws { get; private set; }

        public DbSet<HrCompensatoryVacation> HrCompensatoryVacations { get; private set; }
        public DbSet<HrCompensatoryVacationsVw> HrCompensatoryVacationsVws { get; private set; }


        public DbSet<HrOhad> HrOhads { get; private set; }
        public DbSet<HrOhadVw> HrOhadVws { get; private set; }
        public DbSet<HrContracte> HrContractes { get; private set; }
        public DbSet<HrContractesVw> HrContractesVws { get; private set; }
        public DbSet<HrClearanceMonth> HrClearanceMonths { get; private set; }
        public DbSet<HrClearanceMonthsVw> HrClearanceMonthsVws { get; private set; }

        public DbSet<HrRequest> HrRequests { get; private set; }
        public DbSet<HrRequestVw> HrRequestVws { get; private set; }
        public DbSet<HrRequestDetaile> HrRequestDetailes { get; private set; }
        public DbSet<HrRequestDetailesVw> HrRequestDetailesVws { get; private set; }
        public DbSet<HrRequestType> HrRequestTypes { get; private set; }



        public DbSet<HrCostType> HrCostTypes { get; private set; }
        public DbSet<HrCostTypeVw> HrCostTypeVws { get; private set; }
        public DbSet<HrCustody> HrCustodys { get; private set; }
        public DbSet<HrCustodyVw> HrCustodyVws { get; private set; }
        public DbSet<HrCustodyItem> HrCustodyItems { get; private set; }
        public DbSet<HrCustodyItemsVw> HrCustodyItemsVws { get; private set; }
        public DbSet<HrCustodyItemsProperty> HrCustodyItemsPropertys { get; private set; }
        public DbSet<HrCustodyRefranceType> HrCustodyRefranceTypes { get; private set; }
        public DbSet<HrCustodyType> HrCustodyTypes { get; private set; }
        public DbSet<HrDecision> HrDecisions { get; private set; }
        public DbSet<HrDecisionsVw> HrDecisionsVws { get; private set; }
        public DbSet<HrDecisionsEmployee> HrDecisionsEmployees { get; private set; }
        public DbSet<HrDecisionsEmployeeVw> HrDecisionsEmployeeVws { get; private set; }
        public DbSet<HrIncrementsAllowanceDeduction> HrIncrementsAllowanceDeductions { get; private set; }
        public DbSet<HrDelayVw> HrDelayVws { get; private set; }
        public DbSet<HrHoliday> HrHolidays { get; private set; }
        public DbSet<HrHolidayVw> HrHolidayVws { get; private set; }
        public DbSet<HrPermission> HrPermissions { get; private set; }
        public DbSet<HrPermissionsVw> HrPermissionsVws { get; private set; }

        public DbSet<HrAttShiftTimeTable> HrAttShiftTimeTables { get; private set; }
        public DbSet<HrAttShiftTimeTableVw> HrAttShiftTimeTableVws { get; private set; }
        public DbSet<HrEmployeeCost> HrEmployeeCosts { get; private set; }
        public DbSet<HrEmployeeCostVw> HrEmployeeCostVws { get; private set; }

        public DbSet<HrInsurancePolicy> HrInsurancePolicys { get; private set; }
        public DbSet<HrInsurance> HrInsurances { get; private set; }
        public DbSet<HrInsuranceEmp> HrInsuranceEmps { get; private set; }
        public DbSet<HrInsuranceEmpVw> HrInsuranceEmpVws { get; private set; }

        public DbSet<HrJob> HrJobs { get; private set; }
        public DbSet<HrJobDescription> HrJobDescriptions { get; private set; }
        public DbSet<HrJobEmployeeVw> HrJobEmployeeVws { get; private set; }
        public DbSet<HrJobLevel> HrJobLevels { get; private set; }
        public DbSet<HrRecruitmentVacancy> HrRecruitmentVacancys { get; private set; }
        public DbSet<HrRecruitmentVacancyVw> HrRecruitmentVacancyVws { get; private set; }
        public DbSet<HrRecruitmentApplication> HrRecruitmentApplications { get; private set; }
        public DbSet<HrRecruitmentApplicationVw> HrRecruitmentApplicationVws { get; private set; }
        public DbSet<HrRecruitmentCandidate> HrRecruitmentCandidates { get; private set; }
        public DbSet<HrRecruitmentCandidateVw> HrRecruitmentCandidateVws { get; private set; }


        public DbSet<HrVacancyStatusVw> HrVacancyStatusVws { get; private set; }
        public DbSet<HrJobGrade> HrJobGrades { get; private set; }
        public DbSet<HrJobGradeVw> HrJobGradeVws { get; private set; }
        public DbSet<HrRecruitmentCandidateKpi> HrRecruitmentCandidateKpis { get; private set; }
        public DbSet<HrRecruitmentCandidateKpiVw> HrRecruitmentCandidateKpiVws { get; private set; }
        public DbSet<HrRecruitmentCandidateKpiD> HrRecruitmentCandidateKpiDs { get; private set; }
        public DbSet<HrRecruitmentCandidateKpiDVw> HrRecruitmentCandidateKpiDVws { get; private set; }
        public DbSet<HrPayroll> HrPayrolls { get; private set; }
        public DbSet<HrPayrollVw> HrPayrollVws { get; private set; }
        public DbSet<HrTicketVw> HrTicketVws { get; private set; }
        public DbSet<HrTicket> HrTickets { get; private set; }
        public DbSet<HrVisaVw> HrVisaVws { get; private set; }
        public DbSet<HrVisa> HrVisas { get; private set; }
        public DbSet<HrVacationEmpBalanceDto> HrVacationEmpBalanceDtos { get; private set; }
        public DbSet<HrVacationBalanceALLFilterDto> HrVacationBalanceALLFilterDtos { get; private set; }
        public DbSet<HrFixingEmployeeSalary> HrFixingEmployeeSalarys { get; private set; }
        public DbSet<HrFixingEmployeeSalaryVw> HrFixingEmployeeSalaryVws { get; private set; }
        public DbSet<HrLeaveType> HrLeaveTypes { get; private set; }
        public DbSet<HrLeaveTypeVw> HrLeaveTypeVws { get; private set; }
        public DbSet<HrPayrollAllowanceDeduction> HrPayrollAllowanceDeductions { get; private set; }
        public DbSet<HrPayrollAllowanceDeductionVw> HrPayrollAllowanceDeductionVws { get; private set; }
        public DbSet<HrLoanInstallmentPayment> HrLoanInstallmentPayments { get; private set; }
        public DbSet<HrLoanInstallmentPaymentVw> HrLoanInstallmentPaymentVws { get; private set; }
        public DbSet<HrLoanInstallment> HrLoanInstallments { get; private set; }
        public DbSet<HrLoanInstallmentVw> HrLoanInstallmentVws { get; private set; }

        public DbSet<HrPayrollNote> HrPayrollNotes { get; private set; }

        public DbSet<HrPayrollNoteVw> HrPayrollNoteVws { get; private set; }
        public DbSet<HrDecisionsTypeVw> HrDecisionsTypeVws { get; private set; }
        public DbSet<HrDecisionsType> HrDecisionsTypes { get; private set; }
        public DbSet<HrDecisionsTypeEmployee> HrDecisionsTypeEmployees { get; private set; }
        public DbSet<HrDecisionsTypeEmployeeVw> HrDecisionsTypeEmployeeVws { get; private set; }
        public DbSet<HrEmployeeLocationVw> HrEmployeeLocationVws { get; private set; }
        public DbSet<HrAttShiftEmployeeMVw> HrAttShiftEmployeeMVws { get; private set; }
        public DbSet<HrAttCheckShiftEmployeeVw> HrAttCheckShiftEmployeeVws { get; private set; }

        public DbSet<HRAttendanceReportDto> HRAttendanceReportDtos { get; private set; }
        public DbSet<HRAttendanceReport5Dto> HRAttendanceReport5Dtos { get; private set; }
        public DbSet<HRAttendanceReport4Dto> HRAttendanceReport4Dtos { get; private set; }
        public DbSet<HRAddMultiAttendanceDto> HRAddMultiAttendanceDtos { get; private set; }
        public DbSet<HrOpeningBalance> HrOpeningBalances { get; private set; }
        public DbSet<HrOpeningBalanceVw> HrOpeningBalanceVws { get; private set; }
        public DbSet<HrOpeningBalanceType> HrOpeningBalanceTypes { get; private set; }
        public DbSet<HrPayrollAllowanceVw> HrPayrollAllowanceVws { get; private set; }

        public DbSet<HRPayrollCreate2SpDto> HRPayrollCreate2SpDtos { get; private set; }
        public DbSet<HrPsAllowanceDeduction> HrPsAllowanceDeductions { get; private set; }
        public DbSet<HrPsAllowanceDeductionVw> HrPsAllowanceDeductionVws { get; private set; }
        public DbSet<HrPreparationSalariesVw> HrPreparationSalariesVws { get; private set; }
        public DbSet<HrPreparationSalary> HrPreparationSalaries { get; private set; }

        public DbSet<HRPreparationSalariesLoanDto> HRPreparationSalariesLoanDtos { get; private set; }
        public DbSet<HrPayrollDeductionAccountsVw> HrPayrollDeductionAccountsVws { get; private set; }
        public DbSet<HrPayrollCostcenter> HrPayrollCostcenters { get; private set; }
        public DbSet<HrPayrollCostcenterVw> HrPayrollCostcenterVws { get; private set; }
        public DbSet<HrPayrollAllowanceAccountsVw> HrPayrollAllowanceAccountsVws { get; private set; }
        public DbSet<HrNotificationsReply> hrNotificationsReplies { get; private set; }
        public DbSet<HrLoanPaymentVw> HrLoanPaymentVws { get; private set; }
        public DbSet<HrLoanPayment> HrLoanPayments { get; private set; }
        public DbSet<HrPermissionReasonVw> HrPermissionReasonVws { get; private set; }
        public DbSet<HrPermissionTypeVw> HrPermissionTypeVws { get; private set; }
        public DbSet<HrEmpStatusHistoryVw> HrEmpStatusHistoryVws { get; private set; }
        public DbSet<HrEmpStatusHistory> HrEmpStatusHistorys { get; private set; }

        public DbSet<HrEducation> HrEducations { get; private set; }
        public DbSet<HrEducationVw> HrEducationVws { get; private set; }
        public DbSet<HrSkill> HrSkills { get; private set; }
        public DbSet<HrSkillsVw> HrSkillsVws { get; private set; }
        public DbSet<HrLanguage> HrLanguages { get; private set; }
        public DbSet<HrLanguagesVw> HrLanguagesVws { get; private set; }
        public DbSet<HrFile> HrFiles { get; private set; }
        public DbSet<HrWorkExperience> HrWorkExperiences { get; private set; }
        public DbSet<HrGosiEmployee> HrGosiEmployees { get; private set; }
        public DbSet<HrGosiEmployeeVw> HrGosiEmployeeVws { get; private set; }
        public DbSet<HrGosi> HrGosis { get; private set; }
        public DbSet<HrGosiVw> HrGosiVws { get; private set; }
        public DbSet<HrGosiEmployeeAccVw> HrGosiEmployeeAccVws { get; private set; }
        public DbSet<EmployeeGosiDto> EmployeeGosiDtos { get; private set; }

        public DbSet<HrVacationsDayType> HrVacationsDayTypeS { get; private set; }
        public DbSet<HrEmployeeLeaveResultDto> HrEmployeeLeaveResultDtos { get; private set; }
        public DbSet<HRAttendanceTotalReportDto> HRAttendanceTotalReportDtos { get; private set; }
        public DbSet<HrIncrementType> HrIncrementTypes { get; private set; }
        public DbSet<HrIncrementsAllowanceDeductionVw> HrIncrementsAllowanceDeductionVws { get; private set; }
        public DbSet<HrEmployeeIncremenResultDto> HrEmployeeIncremenResulttDtos { get; private set; }
        public DbSet<HRAttendanceTotalReportNewSPDto> HRAttendanceTotalReportNewSPDtos { get; private set; }
        public DbSet<HRAttendanceReport6SP> HRAttendanceReport6SPs { get; private set; }
        public DbSet<HrPerformanceVw> HrPerformanceVws { get; private set; }
        public DbSet<HrPerformance> HrPerformances { get; private set; }
        public DbSet<HrPerformanceTypeVw> HrPerformanceTypeVws { get; private set; }
        public DbSet<HrPerformanceStatusVw> HrPerformanceStatusVws { get; private set; }
        public DbSet<HrPerformanceForVw> HrPerformanceForVws { get; private set; }
        public DbSet<HrKpiTemplatesJob> HrKpiTemplatesJobs { get; private set; }
        public DbSet<HrKpiTemplatesJobsVw> HrKpiTemplatesJobsVws { get; private set; }
        public DbSet<HrEmpGoalIndicatorsVw> HrEmpGoalIndicatorsVws { get; private set; }
        public DbSet<HrEmpGoalIndicator> HrEmpGoalIndicators { get; private set; }

        public DbSet<HrEmpGoalIndicatorsEmployee> HrEmpGoalIndicatorsEmployees { get; private set; }
        public DbSet<HrEmpGoalIndicatorsCompetence> HrEmpGoalIndicatorsCompetences { get; private set; }
        public DbSet<HrDefinitionSalaryEmp> HrDefinitionSalaryEmps { get; private set; }
        public DbSet<HREmpClearanceSpDto> HREmpClearanceSpDtos { get; private set; }
        public DbSet<HrActualAttendanceVw> HrActualAttendanceVws { get; private set; }
        public DbSet<HrActualAttendance> HrActualAttendances { get; private set; }
        public DbSet<HrPayrollDeductionVw> HrPayrollDeductionVws { get; private set; }
        public DbSet<HrGosiTypeVw> HrGosiTypeVws { get; private set; }
        public DbSet<HrFlexibleWorkingVw> HrFlexibleWorkingVws { get; private set; }
        public DbSet<HrFlexibleWorking> HrFlexibleWorkings { get; private set; }
        public DbSet<HrFlexibleWorkingMaster> HrFlexibleWorkingMasters { get; private set; }
        public DbSet<HrMandateLocationDetailesVw> HrMandateLocationDetailesVws { get; private set; }
        public DbSet<HrMandateLocationDetaile> HrMandateLocationDetailes { get; private set; }
        public DbSet<HrMandateLocationMaster> HrMandateLocationMasters { get; private set; }
        public DbSet<HrMandateLocationMasterVw> HrMandateLocationMasterVws { get; private set; }
        public DbSet<HrMandateRequestsMasterVw> HrMandateRequestsMasterVws { get; private set; }
        public DbSet<HrMandateRequestsMaster> HrMandateRequestsMasters { get; private set; }

        public DbSet<HrMandateRequestsDetaile> HrMandateRequestsDetailes { get; private set; }
        public DbSet<HrMandateRequestsDetailesVw> HrMandateRequestsDetailesVws { get; private set; }

        public DbSet<HrExpensesType> HrExpensesTypes { get; private set; }
        public DbSet<HrExpensesTypeVw> HrExpensesTypeVws { get; private set; }
        public DbSet<HrJobOffer> HrJobOffers { get; private set; }
        public DbSet<HrJobOfferVw> HrJobOfferVws { get; private set; }
        public DbSet<HrExpense> HrExpenses { get; private set; }
        public DbSet<HrExpensesVw> HrExpensesVws { get; private set; }
        public DbSet<HrExpensesEmployeesVw> HrExpensesEmployeesVws { get; private set; }
        public DbSet<HrExpensesEmployee> HrExpensesEmployees { get; private set; }

        public DbSet<HrJobOfferAdvantage> HrJobOfferAdvantages { get; private set; }

        public DbSet<HrProvision> HrProvisions { get; private set; }
        public DbSet<HrProvisionsVw> HrProvisionsVws { get; private set; }
        public DbSet<HrProvisionsEmployee> HrProvisionsEmployees { get; private set; }
        public DbSet<HrProvisionsEmployeeVw> HrProvisionsEmployeeVws { get; private set; }
        public DbSet<HrProvisionsEmployeeAccVw> HrProvisionsEmployeeAccVws { get; private set; }
        public DbSet<HRPayrollCreateAdvancedSpDto> HRPayrollCreateAdvancedSpDtos { get; private set; }
        public DbSet<HrIncomeTaxVw> HrIncomeTaxVws { get; private set; }
        public DbSet<HrIncomeTax> HrIncomeTaxs { get; private set; }
        public DbSet<HrIncomeTaxSlide> HrIncomeTaxSlides { get; private set; }
        public DbSet<HrIncomeTaxPeriod> HrIncomeTaxPeriods { get; private set; }
        public DbSet<HRPayrollManuallCreateSpDto> HRPayrollManuallCreateSpDtos { get; private set; }

        public DbSet<HrEmpGoalIndicatorsEmployeeVw> HrEmpGoalIndicatorsEmployeeVws { get; private set; }
        public DbSet<HrTimeZone> HrTimeZones { get; private set; }

        public DbSet<HrRequestGoalsEmployeeDetail> HrRequestGoalsEmployeeDetails { get; private set; }
        public DbSet<HrRequestGoalsEmployeeDetailsVw> HrRequestGoalsEmployeeDetailsVws { get; private set; }
        public DbSet<HrExpensesSchedule> HrExpensesSchedules { get; private set; }
        public DbSet<HrExpensesPayment> HrExpensesPayments { get; private set; }
        public DbSet<HrStructure> HrStructures { get; private set; }
        public DbSet<HrStructureVw> HrStructureVws { get; private set; }
        public DbSet<HrPayrollTransactionTypeValue> HrPayrollTransactionTypeValues { get; private set; }
        public DbSet<HrPayrollTransactionTypeValuesVw> HrPayrollTransactionTypeValuesVws { get; private set; }
        public DbSet<HrPayrollTransactionType> HrPayrollTransactionTypes { get; private set; }

        public DbSet<HrVisitScheduleLocation> HrVisitScheduleLocations { get; private set; }
        public DbSet<HrVisitScheduleLocationVw> HrVisitScheduleLocationVws { get; private set; }
        public DbSet<HrVisitStep> HrVisitSteps { get; private set; }
        public DbSet<HRAttendanceTotalReportSPDto> HRAttendanceTotalReportSPDtos { get; private set; }
        public DbSet<HRAttendanceTotalReportSPDto> HRAttendanceTotalReportSPDto { get; private set; }
        public DbSet<HrPsAllowanceVw> HrPsAllowanceVws { get; private set; }
        public DbSet<HrPsDeductionVw> HrPsDeductionVws { get; private set; }
        public DbSet<HrJobCategoriesVw> HrJobCategoriesVw { get; private set; }
        public DbSet<HrJobCategory> HrJobCategorys { get; private set; }
        public DbSet<HrJobGroupsVw> HrJobGroupsVw { get; private set; }
        public DbSet<HrJobGroups> HrJobGroups { get; private set; }
        public DbSet<HrIncrementsAllowanceVw> HrIncrementsAllowanceVws { get; private set; }
        public DbSet<HrIncrementsDeductionVw> HrIncrementsDeductionVws { get; private set; }
        public DbSet<HrSector> HrSectors { get; private set; }
        public DbSet<HrJobAllowanceDeduction> HrJobAllowanceDeductions { get; private set; }
        public DbSet<HrJobAllowanceDeductionVw> HrJobAllowanceDeductionVw { get; private set; }
        public DbSet<HrJobLevelsAllowanceDeduction> HrJobLevelsAllowanceDeductions { get; private set; }
        public DbSet<HrJobLevelsAllowanceDeductionVw> HrJobLevelsAllowanceDeductionVw { get; private set; }
        public DbSet<HrJobLevelsVw> HrJobLevelsVw { get; private set; }
        public DbSet<HrLeaveAllowanceDeduction> HrLeaveAllowanceDeduction { get; private set; }
        public DbSet<HrLeaveAllowanceVw> HrLeaveAllowanceVw { get; private set; }
        public DbSet<HrProvisionsMedicalInsurance> HrProvisionsMedicalInsurances { get; private set; }
        public DbSet<HrProvisionsMedicalInsuranceVw> HrProvisionsMedicalInsuranceVw { get; private set; }
        public DbSet<HrProvisionsMedicalInsuranceEmployee> HrProvisionsMedicalInsuranceEmployees { get; private set; }
        public DbSet<HrProvisionsMedicalInsuranceEmployeeVw> HrProvisionsMedicalInsuranceEmployeeVw { get; private set; }
        public DbSet<HrProvisionsMedicalInsuranceEmployeeAccVw> HrProvisionsMedicalInsuranceEmployeeAccVw { get; private set; }
        public DbSet<HrContractsAllowanceDeduction> HrContractsAllowanceDeduction { get; private set; }
        public DbSet<HrClearanceAllowanceDeduction> HrClearanceAllowanceDeductions { get; private set; }
        public DbSet<HrClearanceAllowanceVw> HrClearanceAllowanceVws { get; private set; }
        public DbSet<HrContractsDeductionVw> HrContractsDeductionVw { get; private set; }
        public DbSet<HrContractsAllowanceVw> HrContractsAllowanceVw { get; private set; }

        #endregion --------- End HR --------------------------------------------------------------------------

        #region ============ ACC ==============================================================================

        public DbSet<AccAccount> AccAccounts { get; private set; }
        public DbSet<AccFinancialYear> AccFinancialYears { get; private set; }
        public DbSet<AccFacility> AccFacilities { get; private set; }
        public DbSet<AccFacilitiesVw> AccFacilitiesVws { get; private set; }
        public DbSet<AccGroup> AccGroup { get; private set; }
        public DbSet<AccPeriods> AccPeriods { get; private set; }
        public DbSet<AccCostCenterVws> AccCostCenterVws { get; private set; }
        public DbSet<AccJournalMaster> AccJournalMasters { get; private set; }
        public DbSet<AccJournalMasterVw> AccJournalMasterVws { get; private set; }
        public DbSet<AccCostCenter> AccCostCenter { get; private set; }

        public DbSet<AccReferenceType> AccReferenceTypes { get; private set; }
        public DbSet<AccAccountsVw> AccAccountsVw { get; private set; }

        public DbSet<AccBranchAccount> AccBranchAccounts { get; private set; }
        public DbSet<AccBranchAccountsVw> AccBranchAccountsVws { get; private set; }
        public DbSet<AccBranchAccountType> AccBranchAccountTypes { get; private set; }

        public DbSet<AccPeriodDateVws> AccPeriodDateVws { get; private set; }
        public DbSet<AccJournalDetaile> AccJournalDetailes { get; private set; }
        public DbSet<AccJournalDetailesVw> AccJournalDetailesVws { get; private set; }
        public DbSet<AccAccountsCloseType> AccAccountsCloseTypes { get; private set; }
        public DbSet<AccBank> AccBanks { get; private set; }
        public DbSet<AccCashOnHand> AccCashOnHands { get; private set; }
        public DbSet<AccRequest> AccRequests { get; private set; }
        public DbSet<AccRequestVw> AccRequestVws { get; private set; }
        public DbSet<AccDocumentTypeListVw> AccDocumentTypeListVws { get; private set; }
        public DbSet<AccCashOnHandListVw> AccCashonhandListVWs { get; private set; }
        public DbSet<AccAccountsSubHelpeVw> AccAccountsSubHelpeVws { get; private set; }
        public DbSet<AccCostCenteHelpVw> AccCostCenteHelpVws { get; private set; }
        public DbSet<AccJournalSignatureVw> AccJournalSignatureVws { get; private set; }
        public DbSet<AccJournalMasterExportVw> AccJournalMasterExportVws { get; private set; }
        public DbSet<AccBankVw> AccBankVws { get; private set; }
        public DbSet<AccAccountsLevel> AccAccountsLevels { get; private set; }
        public DbSet<AccPettyCashExpensesType> AccPettyCashExpensesTypes { get; private set; }
        public DbSet<AccPettyCashExpensesTypeVw> AccPettyCashExpensesTypeVws { get; private set; }
        public DbSet<AccBankChequeBook> AccBankChequeBooks { get; private set; }
        public DbSet<AccCashOnHandVw> AccCashOnHandVws { get; private set; }
        public DbSet<AccAccountsGroupsFinalVw> AccAccountsGroupsFinalVws { get; private set; }
        public DbSet<AccAccountsCostcenterVw> AccAccountsCostcenterVws { get; private set; }
        public DbSet<AccReferenceTypeVw> AccReferenceTypeVws { get; private set; }
        public DbSet<AccJournalMasterFile> AccJournalMasterFiles { get; private set; }
        public DbSet<AccJournalMasterFilesVw> AccJournalMasterFilesVws { get; private set; }
        public DbSet<AccAccountsCostcenter> AccAccountsCostcenters { get; private set; }
        public DbSet<AccRequestBalanceStatusVw> AccRequestBalanceStatusVws { get; private set; }
        public DbSet<AccRequestHasCreditVw> AccRequestHasCreditVw { get; private set; }
        public DbSet<AccRequestExchangeStatusVw> AccRequestExchangeStatusVw { get; private set; }
        public DbSet<AccBalanceSheet> AccBalanceSheets { get; private set; }
        public DbSet<AccPaymentType> AccPaymentType { get; private set; }
        public DbSet<AccJournalComment> AccJournalComment { get; private set; }

        public DbSet<AccSettlementSchedule> AccSettlementSchedule { get; private set; }
        public DbSet<AccPettyCash> AccPettyCash { get; private set; }
        public DbSet<AccPettyCashVw> AccPettyCashVw { get; private set; }
        public DbSet<AccSettlementScheduleD> AccSettlementScheduleD { get; private set; }

        public DbSet<AccSettlementInstallment> AccSettlementInstallment { get; private set; }
        public DbSet<AccSettlementScheduleDVw> AccSettlementScheduleDVw { get; private set; }
        public DbSet<AccPettyCashD> AccPettyCashD { get; private set; }
        public DbSet<AccPettyCashDVw> AccPettyCashDVw { get; private set; }
        public DbSet<AccPettyCashTempVw> AccPettyCashTempVw { get; private set; }
        public DbSet<AccSettlementInstallmentsVw> AccSettlementInstallmentsVw { get; private set; }

        public DbSet<AccJournalDetailesCostcenter> AccJournalDetailesCostcenter { get; private set; }
        public DbSet<AccBalanceSheetCostCenterVw> AccBalanceSheetCostCenterVw { get; private set; }
        public DbSet<AccAccountsRefrancesVw> AccAccountsRefrancesVw { get; private set; }
        public DbSet<AccAccountsReportsVw> AccAccountsReportsVW { get; private set; }
        public DbSet<AccBalanceSheetPostOrNot> AccBalanceSheetPostOrNot { get; private set; }
        public DbSet<AccReceivablesPayablesTransactionD> AccReceivablesPayablesTransactionDs { get; private set; }
        public DbSet<AccReceivablesPayablesTransactionDVw> AccReceivablesPayablesTransactionDVws { get; private set; }

        public DbSet<AccCertificateSetting> AccCertificateSettings { get; private set; }
        public DbSet<AccCertificateSettingsSimulation> AccCertificateSettingsSimulations { get; private set; }
        //public DbSet<AccBank> AccBanks { get; private set; }
        public DbSet<AccCostCentersLevel> AccCostCentersLevel { get; private set; }
        public DbSet<AccRequestEmployeeVw> AccRequestEmployeeVw { get; private set; }
        #endregion --------- End ACC --------------------------------------------------------------------------

        #region ============ WhatsApp ==============================================================================
        public DbSet<WaWhatsappSetting> WaWhatsappSettings { get; private set; }
        public DbSet<WaTemplateMessageValue> WaTemplateMessageValues { get; }
        public DbSet<WaTemplateMessage> WaTemplateMessages { get; }
        public DbSet<WaDirectMessage> WaDirectMessages { get; }
        #endregion --------- End WhatsApp --------------------------------------------------------------------------

        #region //=============================Gb======================

        public DbSet<BudgTransaction> BudgTransactions { get; private set; }
        public DbSet<BudgTransactionVw> BudgTransactionVws { get; private set; }
        public DbSet<BudgTransactionDetaile> BudgTransactionDetaile { get; private set; }
        public DbSet<BudgTransactionDetailesVw> BudgTransactionDetailesVws { get; private set; }
        public DbSet<BudgExpensesLinks> BudgExpensesLinks { get; private set; }
        public DbSet<BudgExpensesLinksVW> budgExpensesLinksVWs { get; private set; }
        public DbSet<BudgDocType> BudgDocType { get; private set; }


        #endregion==================End GB====================

        #region ================SAL=====================================================
        public DbSet<SalTransaction> SalTransactions { get; private set; }
        public DbSet<SalTransactionsVw> SalTransactionsVws { get; private set; }

        public DbSet<SalItemsPriceM> SalItemsPriceMs { get; private set; }
        public DbSet<SalItemsPriceMVw> SalItemsPriceMVws { get; private set; }

        public DbSet<SalPosSetting> SalPosSettings { get; private set; }
        public DbSet<SalPosSettingVw> SalPosSettingVws { get; private set; }

        public DbSet<SalPosUser> SalPosUsers { get; private set; }
        public DbSet<SalPosUsersVw> SalPosUsersVws { get; private set; }
        public DbSet<SalPaymentTerm> SalPaymentTerms { get; private set; }
        public DbSet<SalSetting> SalSetting { get; private set; }
        public DbSet<SalTransactionsType> SalTransactionsType { get; private set; }
        public DbSet<SalTransactionsCommission> SalTransactionsCommissions { get; private set; }
        public DbSet<SalTransactionsCommissionVw> SalTransactionsCommissionVws { get; private set; }

        public DbSet<SalTransactionsProduct> SalTransactionsProducts { get; private set; }
        public DbSet<SalTransactionsProductsVw> SalTransactionsProductsVws { get; private set; }

        public DbSet<SalTransactionsDiscount> SalTransactionsDiscounts { get; private set; }
        public DbSet<SalTransactionsDiscountVw> SalTransactionsDiscountVws { get; private set; }

        #endregion =============End SAL=================================================

        #region ================OPM=====================================================
        public DbSet<OpmContractLocation> OpmContractLocations { get; private set; }
        public DbSet<OpmContract> OpmContracts { get; private set; }
        public DbSet<OpmContractVw> OpmContractVws { get; private set; }
        public DbSet<OpmTransactionsItem> OpmTransactionsItems { get; private set; }
        public DbSet<OpmTransactionsLocation> OpmTransactionsLocations { get; private set; }
        public DbSet<OpmPolicy> OpmPolicies { get; private set; }

        public DbSet<OpmContractItem> OpmContractItems { get; private set; }
        public DbSet<OpmContarctEmp> OpmContarctEmps { get; private set; }
        public DbSet<OpmContarctAssign> OpmContarctAssigns { get; private set; }
        public DbSet<OpmContractVw> OpmContractVw { get; private set; }

        public DbSet<OpmContractReplaceEmp> OpmContractReplaceEmps { get; private set; }
        public DbSet<OpmContarctEmpVw> OpmContarctEmpVws { get; private set; }
        public DbSet<OpmContractItemsVw> OpmContractItemsVws { get; }
        public DbSet<OPMPayrollD> OPMPayrollDs { get; private set; }
        public DbSet<OPMPayroll> oPMPayrolls { get; private set; }
        public DbSet<OpmPayrollVw> opmPayrollVws { get; private set; }
        public DbSet<HrPayrollType> HrPayrollTypes { get; private set; }
        public DbSet<OPMTransactionsDetails> OPMTransactionsDetails { get; private set; }
        public DbSet<OpmPURTransactionsDetails> OpmPURTransactionsDetails { get; private set; }

        public DbSet<OpmContractReplaceEmpVw> OpmContractReplaceEmpVws { get; private set; }
        public DbSet<OPMPayrollDVW> OPMPayrollDVWs { get; private set; }
        public DbSet<OPMContractLocationVW> oPMContractLocationVWs { get; private set; }
        public DbSet<OpmServicesTypes> OpmServicesTypes { get; private set; }
        public DbSet<OpmServicesTypesVW> opmServicesTypesVWs { get; private set; }
        public DbSet<OPMTransactionsDetailsVw> oPMTransactionsDetailsVws { get; private set; }
        #endregion

        #region =======PUR=====================================

        public DbSet<PurTransactionsPayment> purTransactionsPayments { get; private set; }
        public DbSet<PurExpense> PurExpenses { get; private set; }
        public DbSet<PurExpensesVw> PurExpensesVws { get; private set; }

        public DbSet<PurDiscountCatalog> PurDiscountCatalogs { get; private set; }
        public DbSet<PurDiscountCatalogVw> PurDiscountCatalogVws { get; private set; }
        public DbSet<PurDiscountCatalogAllVw> PurDiscountCatalogAllVws { get; private set; }
        public DbSet<PurDiscountByAmount> PurDiscountByAmounts { get; private set; }
        public DbSet<PurDiscountByQty> PurDiscountByQties { get; private set; }
        public DbSet<PurDiscountProduct> PurDiscountProducts { get; private set; }
        public DbSet<PurDiscountProductsVw> PurDiscountProductsVws { get; private set; }
        public DbSet<PurDiscountType> PurDiscountTypes { get; private set; }
        public DbSet<PurAdditionalType> PurAdditionalTypes { get; private set; }
        public DbSet<PurAdditionalTypeVw> PurAdditionalTypeVws { get; private set; }
        public DbSet<PurItemsPriceM> PurItemsPriceMs { get; private set; }
        public DbSet<PurItemsPriceMVw> PurItemsPriceMVws { get; private set; }
        public DbSet<PurItemsPriceD> PurItemsPriceDs { get; private set; }
        public DbSet<PurItemsPriceDVw> PurItemsPriceDVws { get; private set; }
        public DbSet<PurTransaction> PurTransactions { get; private set; }
        public DbSet<PurTransactionsVw> PurTransactionsVws { get; private set; }
        public DbSet<PurTransactionsSupplier> PurTransactionsSuppliers { get; private set; }
        public DbSet<PurTransactionsSupplierVw> PurTransactionsSupplierVws { get; private set; }
        public DbSet<PurTransactionsProduct> PurTransactionsProducts { get; private set; }
        public DbSet<PurTransactionsProductsVw> PurTransactionsProductsVws { get; private set; }
        public DbSet<PurTransactionsType> PurTransactionsTypes { get; private set; }
        public DbSet<PurTransactionsDiscount> PurTransactionsDiscounts { get; private set; }
        public DbSet<PurTransactionsDiscountVw> PurTransactionsDiscountVws { get; private set; }
        public DbSet<PurRqfWorkFlowEvaluation> PurRqfWorkFlowEvaluations { get; private set; }
        public DbSet<PurRqfWorkFlowEvaluationVw> PurRqfWorkFlowEvaluationVws { get; private set; }
        public DbSet<PurPaymentTerm> PurPaymentTerms { get; private set; }


        #endregion {

        #region ============================== WF ========================================
        public DbSet<WfAppTypeTable> WfAppTypeTables { get; private set; }
        public DbSet<WfDynamicAttributeDataType> WfDynamicAttributeDataTypes { get; private set; }
        public DbSet<WfDynamicValue> WfDynamicValues { get; private set; }
        public DbSet<WfDynamicTableValue> WfDynamicTableValues { get; private set; }
        public DbSet<WfLookUpCatagory> WfLookUpCatagories { get; private set; }
        public DbSet<WfLookupType> WfLookupTypes { get; private set; }
        public DbSet<WfStepLevel> WfStepLevels { get; private set; }
        public DbSet<WfStepsNotification> WfStepsNotifications { get; private set; }
        public DbSet<WfStepsType> WfStepsTypes { get; private set; }


        public DbSet<WfAppGroup> WfAppGroups { get; private set; }
        public DbSet<WfAppGroupsVw> WfAppGroupsVws { get; private set; }

        public DbSet<WfAppType> WfAppTypes { get; private set; }
        public DbSet<WfAppTypeVw> WfAppTypeVws { get; private set; }

        public DbSet<WfApplication> WfApplications { get; private set; }
        public DbSet<WfApplicationsVw> WfApplicationsVws { get; private set; }

        public DbSet<WfApplicationsAssigne> WfApplicationsAssignes { get; private set; }
        public DbSet<WfApplicationsAssignesVw> WfApplicationsAssignesVws { get; private set; }

        public DbSet<WfApplicationsAssignesReply> WfApplicationsAssignesReplies { get; private set; }
        public DbSet<WfApplicationsAssignesReplyVw> WfApplicationsAssignesReplyVws { get; private set; }

        public DbSet<WfApplicationsComment> WfApplicationsComments { get; private set; }
        public DbSet<WfApplicationsCommentsVw> WfApplicationsCommentsVws { get; private set; }

        public DbSet<WfApplicationsStatus> WfApplicationsStatus { get; private set; }
        public DbSet<WfApplicationsStatusVw> WfApplicationsStatusVws { get; private set; }

        public DbSet<WfDynamicAttribute> WfDynamicAttributes { get; private set; }
        public DbSet<WfDynamicAttributesVw> WfDynamicAttributesVws { get; private set; }

        public DbSet<WfDynamicAttributesTable> WfDynamicAttributesTables { get; private set; }
        public DbSet<WfDynamicAttributesTableVw> WfDynamicAttributesTableVws { get; private set; }

        public DbSet<WfEscalation> WfEscalations { get; private set; }
        public DbSet<WfEscalationVw> WfEscalationVws { get; private set; }

        public DbSet<WfLookupData> WfLookupData { get; private set; }
        public DbSet<WfLookupDataVw> WfLookupDataVws { get; private set; }

        public DbSet<WfStatus> WfStatus { get; private set; }
        public DbSet<WfStatusVw> WfStatusVws { get; private set; }

        public DbSet<WfStep> WfSteps { get; private set; }
        public DbSet<WfStepsVw> WfStepsVws { get; private set; }

        public DbSet<WfStepsTransactionsVw> WfStepsTransactionsVws { get; private set; }
        public DbSet<WfStepsTransaction> WfStepsTransactions { get; private set; }

        public DbSet<WfLayoutAttribute> WfLayoutAttributes { get; private set; }

        public DbSet<WfAppCommittee> WfAppCommittees { get; private set; }
        public DbSet<WfAppCommitteesVw> WfAppCommitteesVws { get; private set; }

        public DbSet<WfActionsCommittee> WfActionsCommittees { get; private set; }

        public DbSet<WfAppCommitteesMember> WfAppCommitteesMembers { get; private set; }
        public DbSet<WfAppCommitteesMembersVw> WfAppCommitteesMembersVws { get; private set; }

        public DbSet<WfAppMember> WfAppMembers { get; private set; }
        public DbSet<WfAppMembersVw> WfAppMembersVws { get; private set; }

        public DbSet<WfStepsCommittee> WfStepsCommittees { get; private set; }
        public DbSet<WfStepsCommitteesVw> WfStepsCommitteesVws { get; private set; }
        public DbSet<WFFormsControls> WFFormsControls { get; private set; }
        #endregion

        #region ========= Hot ==========
        public DbSet<HotFloor> HotFloors { get; private set; }
        public DbSet<HotFloorsVw> HotFloorsVws { get; private set; }
        public DbSet<HotGroup> HotGroups { get; private set; }
        public DbSet<HotGroupsVw> HotGroupsVws { get; private set; }
        public DbSet<HotRoom> HotRooms { get; private set; }
        public DbSet<HotRoomAsset> HotRoomAssets { get; private set; }
        public DbSet<HotRoomAssetsVw> HotRoomAssetsVws { get; private set; }
        public DbSet<HotRoomService> HotRoomServices { get; private set; }
        public DbSet<HotRoomVw> HotRoomVws { get; private set; }
        public DbSet<HotServicesVw> HotServicesVws { get; private set; }
        public DbSet<HotTransaction> HotTransactions { get; private set; }
        public DbSet<HotTransactionsCompanion> HotTransactionsCompanions { get; private set; }
        public DbSet<HotTransactionsCompanionVw> HotTransactionsCompanionVws { get; private set; }
        public DbSet<HotTransactionsPayment> HotTransactionsPayments { get; private set; }
        public DbSet<HotTransactionsRoom> HotTransactionsRooms { get; private set; }
        public DbSet<HotTransactionsRoomVw> HotTransactionsRoomVws { get; private set; }
        public DbSet<HotTransactionsService> HotTransactionsServices { get; private set; }
        public DbSet<HotTransactionsServicesVw> HotTransactionsServicesVws { get; private set; }
        public DbSet<HotTransactionsStatus> HotTransactionsStatuses { get; private set; }
        public DbSet<HotTransactionsType> HotTransactionsTypes { get; private set; }

        public DbSet<HotTransactionsStatus> HotTransactionsStatuss { get; private set; }
        public DbSet<HotTransactionsVw> HotTransactionsVws { get; private set; }

        public DbSet<HotTypeRoom> HotTypeRooms { get; private set; }

        public DbSet<HotService> HotServices { get; private set; }
        #endregion

        #region ======= WH ========================

        public DbSet<WhUnit> WhUnits { get; private set; }
        public DbSet<WhItemsUnitListVw> WhItemsUnitListVw { get; private set; }
        public DbSet<WhItemsCatagory> whItemsCatagories { get; private set; }
        public DbSet<WhAccountType> WhAccountTypes { get; private set; }
        public DbSet<WhItemsVw> WhItemsVws { get; private set; }
        public DbSet<WhItem> WhItems { get; private set; }
        public DbSet<WhInventory> WhInventorys { get; private set; }
        public DbSet<WhInventoriesVw> WhInventoriesVws { get; private set; }
        public DbSet<WhTransactionsMaster> WhTransactionsMasters { get; private set; }
        public DbSet<WhTransactionsMasterVw> WhTransactionsMasterVws { get; private set; }
        public DbSet<WhTransactionsDetaile> WhTransactionsDetailes { get; private set; }
        public DbSet<WhTransactionsDetailesVw> WhTransactionsDetailesVws { get; private set; }
        public DbSet<WhItemsComponent> WhItemsComponents { get; private set; }
        public DbSet<WhItemsComponentsVw> WhItemsComponentsVws { get; private set; }
        public DbSet<WhItemsSerial> WhItemsSerials { get; private set; }
        public DbSet<WhItemsSerialsVw> WhItemsSerialsVws { get; private set; }
        public DbSet<WhActualInventorySeriale> WhActualInventorySeriales { get; private set; }
        public DbSet<WhActualInventorySerialesVw> WhActualInventorySerialesVws { get; private set; }
        public DbSet<WhItemsCatagoriesVw> WhItemsCatagoriesVw { get; private set; }
        public DbSet<WhItemsSection> WhItemsSections { get; private set; }
        public DbSet<WhItemsSectionsVw> WhItemsSectionsVw { get; private set; }
        public DbSet<WhInventorySection> WhInventorySections { get; private set; }
        public DbSet<WhItemTemplate> WhItemTemplates { get; private set; }
        public DbSet<WhItemTemplateVw> WhItemTemplateVw { get; private set; }
        public DbSet<WhTemplate> WhTemplates { get; private set; }
        public DbSet<WhItemsBatch> WhItemsBatchs { get; private set; }
        public DbSet<WhItemsBatchListVw> WhItemsBatchListVw { get; private set; }
        public DbSet<WhTransactionsType> WhTransactionsTypes { get; private set; }
        public DbSet<WhTransactionsTypeVw> WhTransactionsTypeVw { get; private set; }

        #endregion ======= WH ========================

        #region =============== FXA ===========================
        public DbSet<FxaAdditionsExclusion> FxaAdditionsExclusions { get; private set; }
        public DbSet<FxaAdditionsExclusionVw> FxaAdditionsExclusionVws { get; private set; }

        public DbSet<FxaAdditionsExclusionType> FxaAdditionsExclusionTypes { get; private set; }
        public DbSet<FxaDepreciationMethod> FxaDepreciationMethods { get; private set; }

        public DbSet<FxaFixedAsset> FxaFixedAssets { get; private set; }
        public DbSet<FxaFixedAssetVw> FxaFixedAssetVws { get; private set; }
        public DbSet<FxaFixedAssetVw2> FxaFixedAssetVw2s { get; private set; }

        public DbSet<FxaFixedAssetTransfer> FxaFixedAssetTransfers { get; private set; }
        public DbSet<FxaFixedAssetTransferVw> FxaFixedAssetTransferVws { get; private set; }

        public DbSet<FxaFixedAssetType> FxaFixedAssetTypes { get; private set; }
        public DbSet<FxaFixedAssetTypeVw> FxaFixedAssetTypeVws { get; private set; }

        public DbSet<FxaTransaction> FxaTransactions { get; private set; }
        public DbSet<FxaTransactionsVw> FxaTransactionsVws { get; private set; }

        public DbSet<FxaTransactionsAsset> FxaTransactionsAssets { get; private set; }
        public DbSet<FxaTransactionsAssetsVw> FxaTransactionsAssetsVws { get; private set; }

        public DbSet<FxaTransactionsPayment> FxaTransactionsPayments { get; private set; }

        public DbSet<FxaTransactionsType> FxaTransactionsTypes { get; private set; }

        public DbSet<FxaTransactionsProduct> FxaTransactionsProducts { get; private set; }
        public DbSet<FxaTransactionsProductsVw> FxaTransactionsProductsVws { get; private set; }

        public DbSet<FxaTransactionsRevaluation> FxaTransactionsRevaluations { get; private set; }
        public DbSet<FxaTransactionsRevaluationVw> FxaTransactionsRevaluationVws { get; private set; }
        #endregion

        #region =============== CRM ===========================

        public DbSet<CrmEmailTemplate> CrmEmailTemplates { get; private set; }
        public DbSet<CrmEmailTemplateAttach> CrmEmailTemplateAttachs { get; private set; }

        #endregion



        #region =============HD==========================
        public DbSet<HdTickect> HdTickects { get; private set; }
        #endregion


        #region =============TS==========================

        public DbSet<TsTask> TsTasks { get; private set; }
        public DbSet<TsTasksVw> TsTasksVws { get; private set; }
        public DbSet<TsTaskStatusVw> TsTaskStatusVws { get; private set; }
        public DbSet<TsTasksResponse> TsTasksResponses { get; private set; }
        public DbSet<TsTasksResponseVw> TsTasksResponseVws { get; private set; }
        public DbSet<TsAppointment> TsAppointments { get; private set; }
        public DbSet<TsAppointmentVw> TsAppointmentVws { get; private set; }
        public DbSet<TsTasksScheduler> TsTasksSchedulers { get; private set; }
        public DbSet<TsTasksSchedulerVw> TsTasksSchedulerVws { get; private set; }

        #endregion

        #region =============Integra==========================

        public DbSet<IntegraSystem> IntegraSystems { get; private set; }
        public DbSet<IntegraProperty> IntegraPropertys { get; private set; }
        public DbSet<IntegraPropertiesVw> IntegraPropertiesVws { get; private set; }
        public DbSet<IntegraPropertyValue> IntegraPropertyValues { get; private set; }
        public DbSet<IntegraPropertyValuesVw> IntegraPropertyValuesVws { get; private set; }
        public DbSet<IntegraTable> IntegraTables { get; private set; }
        public DbSet<IntegraField> IntegraFields { get; private set; }
        public DbSet<IntegraTableValue> IntegraTableValues { get; private set; }

        #endregion

        #region =============== RE ===========================
        public DbSet<ReTransactionsInstallment> ReTransactionsInstallments { get; private set; }
        public DbSet<ReTransactionsInstallmentsVw> ReTransactionsInstallmentsVws { get; private set; }
        public DbSet<ReTransaction> ReTransactions { get; private set; }
        public DbSet<ReTransactionsVw> ReTransactionsVws { get; private set; }
        #endregion

        #region =============== Sch ===========================
        public DbSet<SchTransactionsInstallment> SchTransactionsInstallments { get; private set; }
        public DbSet<SchTransactionsInstallmentsVw> SchTransactionsInstallmentsVws { get; private set; }

        public DbSet<SchTransactionsTransportationVw> SchTransactionsTransportationVws { get; private set; }
        public DbSet<SchTransactionsTransportationInstallmentsVw> SchTransactionsTransportationInstallmentsVws { get; private set; }
        public DbSet<SchTransactionsTransportationPrintVw> SchTransactionsTransportationPrintVws { get; private set; }
        #endregion

        #region =============== Maintenance ===========================
        public DbSet<MaintTransactionsInstallment> MaintTransactionsInstallments { get; private set; }
        public DbSet<MaintTransactionsInstallmentsVw> MaintTransactionsInstallmentsVws { get; private set; }
        #endregion

        #region =============== Trans ===========================
        public DbSet<TransTransaction> TransTransactions { get; private set; }
        public DbSet<TransTransactionsVw> TransTransactionsVws { get; private set; }
        #endregion

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<TraceEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentData.UserId;
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.ModifiedBy = 0;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = currentData.UserId;
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.ModifiedBy = currentData.UserId;
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        entry.Entity.IsDeleted = true;
                        break;
                }

            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region ========= Main ======================================================================
            modelBuilder.ApplyConfiguration(new SysCalendarConfig());
            modelBuilder.ApplyConfiguration(new SysSystemConfig());
            modelBuilder.ApplyConfiguration(new SysScreenConfig());
            modelBuilder.ApplyConfiguration(new SysAnnouncementConfig());
            modelBuilder.ApplyConfiguration(new SysAnnouncementVwConfig());
            modelBuilder.ApplyConfiguration(new SysAnnouncementLocationVwConfig());
            modelBuilder.ApplyConfiguration(new SysLookupDataConfig());
            modelBuilder.ApplyConfiguration(new SysLookupDataVwConfig());
            modelBuilder.ApplyConfiguration(new SysLookupCategoryConfig());
            modelBuilder.ApplyConfiguration(new SysDepartmentConfig());
            modelBuilder.ApplyConfiguration(new SysDepartmentVwConfig());
            modelBuilder.ApplyConfiguration(new SysDepartmentCatagoryConfig());
            modelBuilder.ApplyConfiguration(new SysUserConfig());
            modelBuilder.ApplyConfiguration(new SysGroupConfig());
            modelBuilder.ApplyConfiguration(new SysGroupVwConfig());
            modelBuilder.ApplyConfiguration(new SysScreenPermissionConfig());
            modelBuilder.ApplyConfiguration(new SysScreenPermissionVwConfig());
            modelBuilder.ApplyConfiguration(new SysBranchVwConfig());
            modelBuilder.ApplyConfiguration(new SysNotificationConfig());
            modelBuilder.ApplyConfiguration(new SysNotificationsVwConfig());
            modelBuilder.ApplyConfiguration(new SysPropertyConfig());
            modelBuilder.ApplyConfiguration(new SysPropertiesVwConfig());
            modelBuilder.ApplyConfiguration(new SysPropertyValuesVwConfig());
            modelBuilder.ApplyConfiguration(new SysScreenPropertyConfig());
            modelBuilder.ApplyConfiguration(new SysScreenPermissionPropertyConfig());
            modelBuilder.ApplyConfiguration(new SysScreenPermissionPropertiesVwConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerGroupConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerTypeConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerGroupAccountConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerGroupAccountsVwConfig());
            modelBuilder.ApplyConfiguration(new SysLicenseConfig());
            modelBuilder.ApplyConfiguration(new SysLicenseVwConfig());

            modelBuilder.ApplyConfiguration(new SysFavMenuConfig());
            modelBuilder.ApplyConfiguration(new SysFileConfig());

            modelBuilder.ApplyConfiguration(new InvestBranchConfig());
            modelBuilder.ApplyConfiguration(new InvestBranchVwConfig());

            modelBuilder.ApplyConfiguration(new SysCustomerConfig());
            modelBuilder.ApplyConfiguration(new SysCurrencyConfig());
            modelBuilder.ApplyConfiguration(new SysCurrencyListVwConfig());

            modelBuilder.ApplyConfiguration(new SysExchangeRateConfig());
            modelBuilder.ApplyConfiguration(new SysExchangeRateVwConfig());
            modelBuilder.ApplyConfiguration(new SysExchangeRateListVWConfig());

            modelBuilder.ApplyConfiguration(new SysScreenInstalledConfig());
            modelBuilder.ApplyConfiguration(new SysScreenInstalledVwConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerBranchConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerBranchVwConfig());
            modelBuilder.ApplyConfiguration(new SysCitesConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerCoTypeConfig());
            modelBuilder.ApplyConfiguration(new SysUserVwConfig());
            modelBuilder.ApplyConfiguration(new SysVatGroupConfig());

            modelBuilder.ApplyConfiguration(new SysPoliciesProcedureConfig());
            modelBuilder.ApplyConfiguration(new SysPoliciesProceduresVwConfig());


            modelBuilder.ApplyConfiguration(new SysCustomerVwConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerContactConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerContactVwConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerFileConfig());
            modelBuilder.ApplyConfiguration(new SysCustomerFilesVwConfig());

            modelBuilder.ApplyConfiguration(new SysTemplateConfig());
            modelBuilder.ApplyConfiguration(new SysTemplateVwConfig());

            modelBuilder.ApplyConfiguration(new SysFormConfig());
            modelBuilder.ApplyConfiguration(new SysFormVwConfig());
            modelBuilder.ApplyConfiguration(new SysFormVwConfig());

            modelBuilder.ApplyConfiguration(new SysSettingExportConfig());
            modelBuilder.ApplyConfiguration(new SysSettingExportVwConfig());

            modelBuilder.ApplyConfiguration(new SysActivityLogConfig());
            modelBuilder.ApplyConfiguration(new SysActivityLogVwConfig());

            modelBuilder.ApplyConfiguration(new InvestEmployeeConfig());
            modelBuilder.ApplyConfiguration(new InvestEmployeeVvwConfig());

            modelBuilder.ApplyConfiguration(new SysUserLogTimeConfig());
            modelBuilder.ApplyConfiguration(new SysUserLogTimeVwConfig());

            modelBuilder.ApplyConfiguration(new SysUserTrackingConfig());
            modelBuilder.ApplyConfiguration(new SysUserTrackingVwConfig());

            modelBuilder.ApplyConfiguration(new SysUserTypeConfig());
            modelBuilder.ApplyConfiguration(new SysUserType2Config());

            modelBuilder.ApplyConfiguration(new SysDynamicAttributeConfig());
            modelBuilder.ApplyConfiguration(new SysDynamicAttributesVwConfig());
            modelBuilder.ApplyConfiguration(new SysDynamicAttributeDataTypeConfig());
            modelBuilder.ApplyConfiguration(new SysScreenVwConfig());
            modelBuilder.ApplyConfiguration(new MonthDayConfig());
            modelBuilder.ApplyConfiguration(new SysVatGroupVwConfig());

            modelBuilder.ApplyConfiguration(new SysNotificationsSettingConfig());
            modelBuilder.ApplyConfiguration(new SysNotificationsSettingVwConfig());
            modelBuilder.ApplyConfiguration(new SysMailServerConfig());
            modelBuilder.ApplyConfiguration(new SysCountryConfig());
            modelBuilder.ApplyConfiguration(new SysCountryVwConfig());

            modelBuilder.ApplyConfiguration(new SysNotificationsMangConfig());
            modelBuilder.ApplyConfiguration(new SysNotificationsMangVwConfig());

            modelBuilder.ApplyConfiguration(new SysTableConfig());
            modelBuilder.ApplyConfiguration(new SysTableFieldConfig());

            modelBuilder.ApplyConfiguration(new SysPackagesPropertyValueConfig());
            modelBuilder.ApplyConfiguration(new SysPackageConfig());

            modelBuilder.ApplyConfiguration(new SysSmsConfig());

            modelBuilder.ApplyConfiguration(new SysWebHookConfig());
            modelBuilder.ApplyConfiguration(new SysWebHookVwConfig());

            modelBuilder.ApplyConfiguration(new SysFilesDocumentConfig());
            modelBuilder.ApplyConfiguration(new SysFilesDocumentVwConfig());
            modelBuilder.ApplyConfiguration(new SysRecordWebhookConfig());
            modelBuilder.ApplyConfiguration(new SysRecordWebhookVwConfig());
            modelBuilder.ApplyConfiguration(new SysDynamicValueConfig());
            modelBuilder.ApplyConfiguration(new SysPeriodConfig());
            modelBuilder.ApplyConfiguration(new SysLibraryFileConfig());
            modelBuilder.ApplyConfiguration(new SysLibraryFilesVwConfig());
            modelBuilder.ApplyConfiguration(new SysResetPasswordConfig());
            modelBuilder.ApplyConfiguration(new SysCreateUserRequstConfig());
            modelBuilder.ApplyConfiguration(new SysCreateUserRequstVwConfig());
            modelBuilder.ApplyConfiguration(new ChatMessageConfig());
            modelBuilder.ApplyConfiguration(new SysMethodTypeApiConfig());
            modelBuilder.ApplyConfiguration(new SysProcessScreenWebHookConfig());
            modelBuilder.ApplyConfiguration(new SysWebHookAuthConfig());
            modelBuilder.ApplyConfiguration(new SysRecordWebhookAuthConfig());
            modelBuilder.ApplyConfiguration(new SysRecordWebhookAuthVwConfig());
            modelBuilder.ApplyConfiguration(new SysWebHookAuthVwConfig());

            modelBuilder.ApplyConfiguration(new SysInvoiceAccordingTypeConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaInvoiceTypeConfig());

            modelBuilder.ApplyConfiguration(new SysZatcaInvoiceTransactionConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaInvoiceTransactionsSimulationConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaReportingResultConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaReportingResultsSimulationConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaSignedXmlConfig());
            modelBuilder.ApplyConfiguration(new SysZatcaSignedXmlSimulationConfig());
            modelBuilder.ApplyConfiguration(new ZatcaCreditDebitNoteConfig());
            modelBuilder.ApplyConfiguration(new ZatcaVatcategoriesReasonConfig());
            modelBuilder.ApplyConfiguration(new SysPropertyClassificationConfig());

            modelBuilder.ApplyConfiguration(new SysCustomersFilesSettingConfig());
            modelBuilder.ApplyConfiguration(new SysCustomersFilesSettingsVwConfig());

            #endregion ------- End Main -------------------------------------------------------------------

            #region ========= HR ======================================================================
            modelBuilder.ApplyConfiguration(new HrAttLocationConfig());
            modelBuilder.ApplyConfiguration(new HrEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrEmployeeVwConfig());

            modelBuilder.ApplyConfiguration(new HrEvaluationAnnualIncreaseConfigConfig());

            modelBuilder.ApplyConfiguration(new HrNotificationConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsVwConfig());

            modelBuilder.ApplyConfiguration(new HrCompetenceConfig());
            modelBuilder.ApplyConfiguration(new HrCompetencesVwConfig());

            modelBuilder.ApplyConfiguration(new HrCompetencesCatagoryConfig());

            modelBuilder.ApplyConfiguration(new HrTrainingBagConfig());
            modelBuilder.ApplyConfiguration(new HrTrainingBagVwConfig());

            modelBuilder.ApplyConfiguration(new HrPolicyConfig());
            modelBuilder.ApplyConfiguration(new HrPoliciesVwConfig());

            modelBuilder.ApplyConfiguration(new HrPoliciesTypeConfig());

            modelBuilder.ApplyConfiguration(new HrKpiTemplatesVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTemplateConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTemplatesCompetenceConfig());
            modelBuilder.ApplyConfiguration(new HrSettingConfig());

            modelBuilder.ApplyConfiguration(new HrSalaryGroupConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupVwConfig());

            modelBuilder.ApplyConfiguration(new HrVacationsTypeConfig());
            modelBuilder.ApplyConfiguration(new HrVacationsTypeVwConfig());

            modelBuilder.ApplyConfiguration(new HrDisciplinaryCaseConfig());
            modelBuilder.ApplyConfiguration(new HrCardTemplateConfig());
            modelBuilder.ApplyConfiguration(new hrOverTimeDVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollTypeConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftConfig());
            modelBuilder.ApplyConfiguration(new HrJobVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobProgramVwConfig());
            modelBuilder.ApplyConfiguration(new HrAbsenceConfig());

            modelBuilder.ApplyConfiguration(new HrVacationsConfig());
            modelBuilder.ApplyConfiguration(new HrVacationsVwConfig());
            modelBuilder.ApplyConfiguration(new HrMandateConfig());
            modelBuilder.ApplyConfiguration(new HrMandateVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttendanceConfig());
            modelBuilder.ApplyConfiguration(new HrAttendancesVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttTimeTableConfig());
            modelBuilder.ApplyConfiguration(new HrAttTimeTableVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTypeConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTemplatesCompetencesVwConfig());
            modelBuilder.ApplyConfiguration(new HrDisciplinaryRuleConfig());
            modelBuilder.ApplyConfiguration(new HrDisciplinaryRuleVwConfig());
            modelBuilder.ApplyConfiguration(new HrDisciplinaryCaseActionConfig());
            modelBuilder.ApplyConfiguration(new HrDisciplinaryCaseActionVwConfig());
            modelBuilder.ApplyConfiguration(new HrRateTypeConfig());
            modelBuilder.ApplyConfiguration(new HrRateTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrVacationsCatagoryConfig());
            modelBuilder.ApplyConfiguration(new HrAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrAllowanceDeductionVWConfig());
            modelBuilder.ApplyConfiguration(new HrLoanConfig());
            modelBuilder.ApplyConfiguration(new HrLoanVwConfig());
            modelBuilder.ApplyConfiguration(new HrAbsenceVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollDVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollDConfig());
            //       2023/12/31
            modelBuilder.ApplyConfiguration(new HrArchivesFileConfig());
            modelBuilder.ApplyConfiguration(new HrArchivesFilesVwConfig());

            modelBuilder.ApplyConfiguration(new HrLicenseConfig());
            modelBuilder.ApplyConfiguration(new HrLicensesVwConfig());

            modelBuilder.ApplyConfiguration(new HrTransferConfig());
            modelBuilder.ApplyConfiguration(new HrTransfersVwConfig());

            modelBuilder.ApplyConfiguration(new HrOverTimeMConfig());
            modelBuilder.ApplyConfiguration(new HrOverTimeMVwConfig());

            modelBuilder.ApplyConfiguration(new HrOhadDetailConfig());
            modelBuilder.ApplyConfiguration(new HrOhadDetailsVwConfig());

            modelBuilder.ApplyConfiguration(new HrEmpWarnConfig());
            modelBuilder.ApplyConfiguration(new HrEmpWarnVwConfig());

            modelBuilder.ApplyConfiguration(new HrVacationBalanceConfig());
            modelBuilder.ApplyConfiguration(new HrVacationBalanceVwConfig());

            modelBuilder.ApplyConfiguration(new HrDependentConfig());
            modelBuilder.ApplyConfiguration(new HrDependentsVwConfig());

            modelBuilder.ApplyConfiguration(new HrDirectJobConfig());
            modelBuilder.ApplyConfiguration(new HrDirectJobVwConfig());

            modelBuilder.ApplyConfiguration(new HrIncrementConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementsVwConfig());

            modelBuilder.ApplyConfiguration(new HrLeaveConfig());
            modelBuilder.ApplyConfiguration(new HrLeaveVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiConfig());
            modelBuilder.ApplyConfiguration(new HrKpiVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiDetaileConfig());
            modelBuilder.ApplyConfiguration(new HrKpiDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new HrEmpWorkTimeConfig());
            modelBuilder.ApplyConfiguration(new HrEmpWorkTimeVwConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupAccountConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupRefranceConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupRefranceVwConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupAllowanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrSalaryGroupDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsTypeConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsSettingConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsSettingVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttTimeTableDayConfig());
            modelBuilder.ApplyConfiguration(new HrDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrAllowanceVwConfig());

            modelBuilder.ApplyConfiguration(new HrAllowanceDeductionMConfig());

            modelBuilder.ApplyConfiguration(new HrAllowanceDeductionTempOrFixConfig());

            modelBuilder.ApplyConfiguration(new HrArchiveFilesDetailsConfig());
            modelBuilder.ApplyConfiguration(new HrArchiveFilesDetailsVwConfig());

            modelBuilder.ApplyConfiguration(new HrAttActionConfig());
            modelBuilder.ApplyConfiguration(new HrAssignmenConfig());
            modelBuilder.ApplyConfiguration(new HrAssignmenVwConfig());

            modelBuilder.ApplyConfiguration(new HrAttLocationEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrAttLocationEmployeeVwConfig());

            modelBuilder.ApplyConfiguration(new HrAttShiftCloseConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftCloseVwConfig());

            modelBuilder.ApplyConfiguration(new HrAttShiftCloseDConfig());

            modelBuilder.ApplyConfiguration(new HrAuthorizationConfig());
            modelBuilder.ApplyConfiguration(new HrAuthorizationVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttendanceBioTimeConfig());

            modelBuilder.ApplyConfiguration(new HrCheckInOutConfig());
            modelBuilder.ApplyConfiguration(new HrCheckInOutVwConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceTypeConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceTypeVwConfig());

            modelBuilder.ApplyConfiguration(new HrCompensatoryVacationConfig());
            modelBuilder.ApplyConfiguration(new HrCompensatoryVacationsVwConfig());


            modelBuilder.ApplyConfiguration(new HrContracteConfig());
            modelBuilder.ApplyConfiguration(new HrContractesVwConfig());

            modelBuilder.ApplyConfiguration(new HrClearanceMonthConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceMonthsVwConfig());
            modelBuilder.ApplyConfiguration(new HrCostTypeConfig());
            modelBuilder.ApplyConfiguration(new HrCostTypeVwConfig());

            modelBuilder.ApplyConfiguration(new HrCustodyConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyVwConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyItemConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyItemsVwConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyItemsPropertyConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyRefranceTypeConfig());
            modelBuilder.ApplyConfiguration(new HrCustodyTypeConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsVwConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrOhadConfig());
            modelBuilder.ApplyConfiguration(new HrOhadVwConfig());
            modelBuilder.ApplyConfiguration(new HrNoteConfig());
            modelBuilder.ApplyConfiguration(new HrNoteVwConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementsAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrDelayVwConfig());
            modelBuilder.ApplyConfiguration(new HrHolidayConfig());
            modelBuilder.ApplyConfiguration(new HrHolidayVwConfig());
            modelBuilder.ApplyConfiguration(new HrPermissionConfig());
            modelBuilder.ApplyConfiguration(new HrPermissionsVwConfig());


            modelBuilder.ApplyConfiguration(new HrAttShiftTimeTableConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftTimeTableVwConfig());
            modelBuilder.ApplyConfiguration(new HrEmployeeCostConfig());
            modelBuilder.ApplyConfiguration(new HrEmployeeCostVwConfig());

            modelBuilder.ApplyConfiguration(new HrInsurancePolicyConfig());
            modelBuilder.ApplyConfiguration(new HrInsuranceConfig());
            modelBuilder.ApplyConfiguration(new HrInsuranceEmpConfig());
            modelBuilder.ApplyConfiguration(new HrInsuranceEmpVwConfig());

            modelBuilder.ApplyConfiguration(new HrJobEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobDescriptionConfig());
            modelBuilder.ApplyConfiguration(new HrJobConfig());
            modelBuilder.ApplyConfiguration(new HrJobLevelConfig());


            modelBuilder.ApplyConfiguration(new HrRecruitmentApplicationVwConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentApplicationConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentVacancyConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentVacancyVwConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateVwConfig());




            modelBuilder.ApplyConfiguration(new HrVacancyStatusVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobGradeConfig());
            modelBuilder.ApplyConfiguration(new HrJobGradeVwConfig());


            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateKpiDConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateKpiDVwConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateKpiConfig());
            modelBuilder.ApplyConfiguration(new HrRecruitmentCandidateKpiVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollVwConfig());
            modelBuilder.ApplyConfiguration(new HrTicketConfig());
            modelBuilder.ApplyConfiguration(new HrTicketVwConfig());
            modelBuilder.ApplyConfiguration(new HrVisaVwConfig());
            modelBuilder.ApplyConfiguration(new HrVisaConfig());
            modelBuilder.ApplyConfiguration(new HrFixingEmployeeSalaryConfig());
            modelBuilder.ApplyConfiguration(new HrFixingEmployeeSalaryVwConfig());
            modelBuilder.ApplyConfiguration(new HrLeaveTypeConfig());
            modelBuilder.ApplyConfiguration(new HrLeaveTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollAllowanceDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrLoanInstallmentPaymentConfig());
            modelBuilder.ApplyConfiguration(new HrLoanInstallmentPaymentVwConfig());
            modelBuilder.ApplyConfiguration(new HrLoanInstallmentConfig());
            modelBuilder.ApplyConfiguration(new HrLoanInstallmentVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollNoteConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollNoteVwConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsTypeConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsTypeEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrDecisionsTypeEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrEmployeeLocationVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttShiftEmployeeMVwConfig());
            modelBuilder.ApplyConfiguration(new HrAttCheckShiftEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrOpeningBalanceConfig());
            modelBuilder.ApplyConfiguration(new HrOpeningBalanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrOpeningBalanceTypeConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollAllowanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrPsAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrPsAllowanceDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrPreparationSalaryConfig());
            modelBuilder.ApplyConfiguration(new HrPreparationSalariesVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollDeductionAccountsVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollCostcenterConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollCostcenterVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollAllowanceAccountsVwConfig());
            modelBuilder.ApplyConfiguration(new HrNotificationsReplyConfig());
            modelBuilder.ApplyConfiguration(new HrLoanPaymentVwConfig());
            modelBuilder.ApplyConfiguration(new HrRequestConfig());
            modelBuilder.ApplyConfiguration(new HrRequestVwConfig());
            modelBuilder.ApplyConfiguration(new HrRequestDetaileConfig());
            modelBuilder.ApplyConfiguration(new HrRequestDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new HrRequestTypeConfig());
            modelBuilder.ApplyConfiguration(new HrPermissionTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrPermissionReasonVwConfig());
            modelBuilder.ApplyConfiguration(new HrEmpStatusHistoryConfig());
            modelBuilder.ApplyConfiguration(new HrEmpStatusHistoryVwConfig());


            modelBuilder.ApplyConfiguration(new HrLanguageConfig());
            modelBuilder.ApplyConfiguration(new HrLanguagesVwConfig());
            modelBuilder.ApplyConfiguration(new HrFileConfig());
            modelBuilder.ApplyConfiguration(new HrSkillConfig());
            modelBuilder.ApplyConfiguration(new HrSkillsVwConfig());
            modelBuilder.ApplyConfiguration(new HrEducationConfig());
            modelBuilder.ApplyConfiguration(new HrEducationVwConfig());
            modelBuilder.ApplyConfiguration(new HrWorkExperienceConfig());
            modelBuilder.ApplyConfiguration(new HrGosiEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrGosiEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrGosiConfig());
            modelBuilder.ApplyConfiguration(new HrGosiVwConfig());
            modelBuilder.ApplyConfiguration(new HrGosiEmployeeAccVwConfig());
            modelBuilder.ApplyConfiguration(new HrVacationsDayTypeConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementTypeConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementsAllowanceDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrPerformanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrPerformanceConfig());
            modelBuilder.ApplyConfiguration(new HrPerformanceStatusVwConfig());
            modelBuilder.ApplyConfiguration(new HrPerformanceTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrPerformanceForVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTemplatesJobsVwConfig());
            modelBuilder.ApplyConfiguration(new HrKpiTemplatesJobsConfig());
            modelBuilder.ApplyConfiguration(new HrEmpGoalIndicatorsVwConfig());
            modelBuilder.ApplyConfiguration(new HrEmpGoalIndicatorConfig());
            modelBuilder.ApplyConfiguration(new HrEmpGoalIndicatorsEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrEmpGoalIndicatorsCompetenceConfig());
            modelBuilder.ApplyConfiguration(new HrDefinitionSalaryEmpConfig());
            modelBuilder.ApplyConfiguration(new HrActualAttendanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrActualAttendanceConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrGosiTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrFlexibleWorkingVwConfig());
            modelBuilder.ApplyConfiguration(new HrFlexibleWorkingConfig());
            modelBuilder.ApplyConfiguration(new HrFlexibleWorkingMasterConfig());
            modelBuilder.ApplyConfiguration(new HrMandateLocationDetaileConfig());
            modelBuilder.ApplyConfiguration(new HrMandateLocationDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new HrMandateLocationMasterConfig());
            modelBuilder.ApplyConfiguration(new HrMandateLocationMasterVwConfig());
            modelBuilder.ApplyConfiguration(new HrMandateRequestsMasterConfig());
            modelBuilder.ApplyConfiguration(new HrMandateRequestsMasterVwConfig());
            modelBuilder.ApplyConfiguration(new HrMandateRequestsDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new HrMandateRequestsDetaileConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesTypeConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesTypeVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobOfferVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobOfferConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesEmployeesVwConfig());
            modelBuilder.ApplyConfiguration(new HrExpenseConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobOfferAdvantageConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsVwConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsEmployeeAccVwConfig());
            modelBuilder.ApplyConfiguration(new HrIncomeTaxConfig());
            modelBuilder.ApplyConfiguration(new HrIncomeTaxVwConfig());
            modelBuilder.ApplyConfiguration(new HrIncomeTaxSlideConfig());
            modelBuilder.ApplyConfiguration(new HrIncomeTaxPeriodConfig());
            modelBuilder.ApplyConfiguration(new HrEmpGoalIndicatorsEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrTimeZoneConfig());

            modelBuilder.ApplyConfiguration(new HrRequestGoalsEmployeeDetailConfig());
            modelBuilder.ApplyConfiguration(new HrRequestGoalsEmployeeDetailsVwConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesScheduleConfig());
            modelBuilder.ApplyConfiguration(new HrExpensesPaymentConfig());
            modelBuilder.ApplyConfiguration(new HrStructureConfig());
            modelBuilder.ApplyConfiguration(new HrStructureVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollTransactionTypeValueConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollTransactionTypeValuesVwConfig());
            modelBuilder.ApplyConfiguration(new HrPayrollTransactionTypeConfig());

            modelBuilder.ApplyConfiguration(new HrVisitScheduleLocationConfig());
            modelBuilder.ApplyConfiguration(new HrVisitScheduleLocationVwConfig());
            modelBuilder.ApplyConfiguration(new HrVisitStepConfig());
            modelBuilder.ApplyConfiguration(new HrPsDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobCategoryConfig());
            modelBuilder.ApplyConfiguration(new HrJobCategoriesVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobGroupsConfig());
            modelBuilder.ApplyConfiguration(new HrJobGroupsVwConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementsAllowanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrIncrementsDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrSectorConfig());
            modelBuilder.ApplyConfiguration(new HrJobAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrJobAllowanceDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobLevelsAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrJobLevelsAllowanceDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrJobLevelsVwConfig());
            modelBuilder.ApplyConfiguration(new HrLeaveAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrLeaveAllowanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsMedicalInsuranceConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsMedicalInsuranceVwConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsMedicalInsuranceEmployeeConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsMedicalInsuranceEmployeeVwConfig());
            modelBuilder.ApplyConfiguration(new HrProvisionsMedicalInsuranceEmployeeAccVwConfig());
            modelBuilder.ApplyConfiguration(new HrContractsAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceAllowanceDeductionConfig());
            modelBuilder.ApplyConfiguration(new HrClearanceAllowanceVwConfig());
            modelBuilder.ApplyConfiguration(new HrContractsDeductionVwConfig());
            modelBuilder.ApplyConfiguration(new HrContractsAllowanceVwConfig());

            #endregion ------- End HR -------------------------------------------------------------------

            #region ========= ACC ======================================================================
            modelBuilder.ApplyConfiguration(new AccAccountConfig());
            modelBuilder.ApplyConfiguration(new AccFacilityConfig());
            modelBuilder.ApplyConfiguration(new AccFacilitiesVwConfig());
            modelBuilder.ApplyConfiguration(new AccFinancialYearConfig());
            modelBuilder.ApplyConfiguration(new AccGroupConfig());
            modelBuilder.ApplyConfiguration(new AccPeriodsConfig());
            modelBuilder.ApplyConfiguration(new AccCostCenterConfig());
            modelBuilder.ApplyConfiguration(new AccCostCenterVwsConfig());
            modelBuilder.ApplyConfiguration(new AccJournalMasterConfig());
            modelBuilder.ApplyConfiguration(new AccJournalMasterVwConfig());
            //modelBuilder.ApplyConfiguration(new AccCostCenterVwsConfig());
            modelBuilder.ApplyConfiguration(new WhItemsVwConfig());
            modelBuilder.ApplyConfiguration(new AccReferenceTypeConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsVwConfig());
            modelBuilder.ApplyConfiguration(new AccBranchAccountConfig());
            modelBuilder.ApplyConfiguration(new AccBranchAccountsVwConfig());
            modelBuilder.ApplyConfiguration(new AccBranchAccountTypeConfig());
            modelBuilder.ApplyConfiguration(new AccPeriodDateVwsConfig());
            modelBuilder.ApplyConfiguration(new AccJournalDetaileConfig());
            modelBuilder.ApplyConfiguration(new AccRequestConfig());
            modelBuilder.ApplyConfiguration(new AccRequestVwConfig());
            modelBuilder.ApplyConfiguration(new AccJournalDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new AccCashonhandListVWConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsSubHelpeVwConfig());
            modelBuilder.ApplyConfiguration(new AccCostCenteHelpVwConfig());
            modelBuilder.ApplyConfiguration(new AccJournalSignatureVwConfig());
            modelBuilder.ApplyConfiguration(new AccJournalMasterExportVwConfig());
            modelBuilder.ApplyConfiguration(new AccBankVwConfig());
            modelBuilder.ApplyConfiguration(new AccBanksConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsLevelConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashExpensesTypeConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashExpensesTypeVwConfig());
            modelBuilder.ApplyConfiguration(new AccBankChequeBookConfig());
            modelBuilder.ApplyConfiguration(new AccCashOnHandVwConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsGroupsFinalVwConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsCostcenterVwConfig());
            modelBuilder.ApplyConfiguration(new AccDocumentTypeListVwConfig());
            modelBuilder.ApplyConfiguration(new AccReferenceTypeVwConfig());
            modelBuilder.ApplyConfiguration(new AccJournalMasterFilesConfig());
            modelBuilder.ApplyConfiguration(new AccJournalMasterFilesVwConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsCostcenterConfig());
            modelBuilder.ApplyConfiguration(new AccRequestBalanceStatusVwConfig());
            modelBuilder.ApplyConfiguration(new AccRequestExchangeStatusVwConfig());
            modelBuilder.ApplyConfiguration(new AccRequestHasCreditVwConfig());
            modelBuilder.ApplyConfiguration(new AccBalanceSheetConfig());
            modelBuilder.ApplyConfiguration(new AccPaymentTypeConfig());
            modelBuilder.ApplyConfiguration(new AccJournalCommentConfig());
            modelBuilder.ApplyConfiguration(new AccSettlementScheduleConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashVwConfig());
            modelBuilder.ApplyConfiguration(new AccSettlementInstallmentConfig());
            modelBuilder.ApplyConfiguration(new AccSettlementScheduleDConfig());
            modelBuilder.ApplyConfiguration(new AccSettlementScheduleDVwConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashDConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashDVwConfig());
            modelBuilder.ApplyConfiguration(new AccPettyCashTempVwConfig());
            modelBuilder.ApplyConfiguration(new AccSettlementInstallmentsVwConfig());
            modelBuilder.ApplyConfiguration(new AccJournalDetailesCostcenterConfig());
            modelBuilder.ApplyConfiguration(new AccBalanceSheetCostCenterVwConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsRefrancesVwConfig());
            modelBuilder.ApplyConfiguration(new AccAccountsReportsVWConfig());
            modelBuilder.ApplyConfiguration(new AccBalanceSheetPostOrNotConfig());
            modelBuilder.ApplyConfiguration(new AccReceivablesPayablesTransactionDConfig());
            modelBuilder.ApplyConfiguration(new AccReceivablesPayablesTransactionDVwConfig());

            modelBuilder.ApplyConfiguration(new AccCertificateSettingConfig());
            modelBuilder.ApplyConfiguration(new AccCertificateSettingsSimulationConfig());
            modelBuilder.ApplyConfiguration(new AccRequestEmployeeVwConfig());
            #endregion ------- End ACC -------------------------------------------------------------------

            #region ======= PM ========================

            //==configPattern==

            modelBuilder.ApplyConfiguration(new PmProjectsTypeConfig());
            modelBuilder.ApplyConfiguration(new PmJobsSalaryConfig());
            modelBuilder.ApplyConfiguration(new PmJobsSalaryVwConfig());
            modelBuilder.ApplyConfiguration(new PmOperationalControlConfig());
            modelBuilder.ApplyConfiguration(new PmOperationalControlsVwConfig());

            modelBuilder.ApplyConfiguration(new PmExtractAdditionalTypeConfig());
            modelBuilder.ApplyConfiguration(new PmExtractAdditionalTypeVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectPlanConfig());
            modelBuilder.ApplyConfiguration(new PmProjectPlansVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsAddDeducConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsAddDeducVwConfig());
            modelBuilder.ApplyConfiguration(new PMProjectsConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsFileConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsFilesVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentActionConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentActionVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentPaymentConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentPaymentVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInstallmentVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsItemConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsItemsVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsRiskConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsRisksVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsRisksVw2Config());
            modelBuilder.ApplyConfiguration(new PmProjectsStaffConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStaffTypeConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStaffVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStageConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStagesVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStokeholderConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStokeholderVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsTypeVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStatusConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsVwConfig());

            modelBuilder.ApplyConfiguration(new PmProjectStatusVwConfig());
            modelBuilder.ApplyConfiguration(new PmDurationTypeVwConfig());
            modelBuilder.ApplyConfiguration(new PmSettingConfig());

            modelBuilder.ApplyConfiguration(new HrDisciplinaryActionTypeConfig());

            modelBuilder.ApplyConfiguration(new PmExtractTransactionsChangeStatusConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsChangeStatusVwConfig());

            modelBuilder.ApplyConfiguration(new PmExtractTransactionConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsVwConfig());

            modelBuilder.ApplyConfiguration(new PmExtractTransactionsProductConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsProductsVwConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsTypeConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsAdditionalConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsEditVwConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsStatusConfig());
            modelBuilder.ApplyConfiguration(new PmRiskImpactConfig());
            modelBuilder.ApplyConfiguration(new PmRiskEffectConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsBudgetItemsVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsBudgetItemConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsObjectiveConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStrategicLinkConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStrategicLinkVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInterconnectionConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsInterconnectionVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsPerformanceIndicatorConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverableTransactionsDetailConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverableTransactionsDetailsVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsDeliverableConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsDeliverableVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsResourceConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsResourcesVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsGovernanceConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsAssumptionConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsDeliverableAcceptCriterionConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsDeliverableAcceptCriteriaVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectStepsVwConfig());
            modelBuilder.ApplyConfiguration(new PmChangeRequestConfig());
            modelBuilder.ApplyConfiguration(new PmChangeRequestVwConfig());
            modelBuilder.ApplyConfiguration(new PmChangeRequestItemConfig());
            modelBuilder.ApplyConfiguration(new PmChangeRequestItemsVwConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverableTransactionConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverableTransactionsVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsLessonsLearnedDetailConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsLessonsLearnedDetailsVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsLessonsLearnedMasterConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsLessonsLearnedMasterVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsClosingConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsClosingVwConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverablesTrackingStatusConfig());
            modelBuilder.ApplyConfiguration(new PmDeliverablesTrackingStatusVwConfig());
            modelBuilder.ApplyConfiguration(new PmKickOffConfig());
            modelBuilder.ApplyConfiguration(new PmKickOffVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStatementRequestConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsStatementRequestVwConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsDiscountConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsDiscountVwConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsPaymentConfig());
            modelBuilder.ApplyConfiguration(new PmExtractTransactionsPaymentVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsGuaranteeConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsGuaranteeVwConfig());
            modelBuilder.ApplyConfiguration(new PmProjectsListVwConfig());

            modelBuilder.ApplyConfiguration(new PmTransactionsInstallmentConfig());
            modelBuilder.ApplyConfiguration(new PmTransactionsInstallmentsVwConfig());

            modelBuilder.ApplyConfiguration(new PmTransactionConfig());
            modelBuilder.ApplyConfiguration(new PmTransactionsVwConfig());
            #endregion ======= PM ========================

            #region ========= RPT ======================================================================
            modelBuilder.ApplyConfiguration(new RptReportConfig());
            modelBuilder.ApplyConfiguration(new RptCustomReportConfig());

            modelBuilder.ApplyConfiguration(new RptFieldConfig());
            modelBuilder.ApplyConfiguration(new RptFieldsVwConfig());
            modelBuilder.ApplyConfiguration(new RptTableConfig());
            modelBuilder.ApplyConfiguration(new RptReportsFieldsVwConfig());
            modelBuilder.ApplyConfiguration(new RptReportsFieldConfig());
            modelBuilder.ApplyConfiguration(new RptOperatorConfig());
            modelBuilder.ApplyConfiguration(new RptReportsVwConfig());
            modelBuilder.ApplyConfiguration(new RptReportsOrderByVwConfig());
            modelBuilder.ApplyConfiguration(new RptReportsOrderByVwConfig());
            modelBuilder.ApplyConfiguration(new RptPowerBiconfigConfig());
            #endregion ------- End RPT -------------------------------------------------------------------

            #region ========= WhatsApp ======================================================================
            modelBuilder.ApplyConfiguration(new WaWhatsappSettingConfig());
            modelBuilder.ApplyConfiguration(new WaTemplateMessageValueConfig());
            modelBuilder.ApplyConfiguration(new WaTemplateMessageConfig());
            modelBuilder.ApplyConfiguration(new WaDirectMessageConfig());


            #endregion ------- End WhatsApp -------------------------------------------------------------------

            #region ======================= GB ======================
            modelBuilder.ApplyConfiguration(new BudgTransactionConfig());
            modelBuilder.ApplyConfiguration(new BudgTransactionVwConfig());
            modelBuilder.ApplyConfiguration(new BudgTransactionDetaileConfig());
            modelBuilder.ApplyConfiguration(new BudgTransactionDetailesVwConfig());
            modelBuilder.ApplyConfiguration(new BudgExpensesLinksConfig());
            modelBuilder.ApplyConfiguration(new BudgExpensesLinksVWConfig());
            modelBuilder.ApplyConfiguration(new BudgDocTypeConfig());
            #endregion================== End GB ====================

            #region =============SAL===================================================================
            modelBuilder.ApplyConfiguration(new SalTransactionConfig());
            modelBuilder.ApplyConfiguration(new SalTransactionVwConfig());

            modelBuilder.ApplyConfiguration(new SalItemsPriceMConfig());
            modelBuilder.ApplyConfiguration(new SalItemsPriceMVwConfig());

            modelBuilder.ApplyConfiguration(new SalPosSettingConfig());
            modelBuilder.ApplyConfiguration(new SalPosSettingVwConfig());

            modelBuilder.ApplyConfiguration(new SalPosUserConfig());
            modelBuilder.ApplyConfiguration(new SalPosUsersVwConfig());
            modelBuilder.ApplyConfiguration(new SalTransactionsTypeConfig());

            modelBuilder.ApplyConfiguration(new SalTransactionsCommissionConfig());
            modelBuilder.ApplyConfiguration(new SalTransactionsCommissionVwConfig());

            modelBuilder.ApplyConfiguration(new SalTransactionsProductConfig());
            modelBuilder.ApplyConfiguration(new SalTransactionsProductsVwConfig());

            modelBuilder.ApplyConfiguration(new SalTransactionsDiscountConfig());
            modelBuilder.ApplyConfiguration(new SalTransactionsDiscountVwConfig());
            #endregion =========End SAL===============================================================

            #region =============OPM==================================================================
            modelBuilder.ApplyConfiguration(new OpmContractConfig());
            modelBuilder.ApplyConfiguration(new OpmContractVwConfig());
            modelBuilder.ApplyConfiguration(new OpmContractLocationConfig());
            modelBuilder.ApplyConfiguration(new OpmTransactionsItemConfig());
            modelBuilder.ApplyConfiguration(new OpmTransactionsLocationConfig());
            modelBuilder.ApplyConfiguration(new OpmPolicyConfig());

            modelBuilder.ApplyConfiguration(new OpmContractItemConfig());
            modelBuilder.ApplyConfiguration(new OpmContarctEmpConfig());
            modelBuilder.ApplyConfiguration(new OpmContarctAssignConfig());
            modelBuilder.ApplyConfiguration(new OpmContractVwConfig());

            modelBuilder.ApplyConfiguration(new OpmContractReplaceEmpConfig());
            modelBuilder.ApplyConfiguration(new OpmContarctEmpVwConfig());

            modelBuilder.ApplyConfiguration(new OPMPayrollConfig());
            modelBuilder.ApplyConfiguration(new OpmPayrollVwConfig());

            modelBuilder.ApplyConfiguration(new OpmContractReplaceEmpVwConfig());
            modelBuilder.ApplyConfiguration(new OpmContractItemsVwConfig());
            modelBuilder.ApplyConfiguration(new OPMPayrollDVWConfig());
            modelBuilder.ApplyConfiguration(new OPMContractLocationVWConfig());
            modelBuilder.ApplyConfiguration(new OpmServicesTypesVWConfig());
            modelBuilder.ApplyConfiguration(new OPMTransactionsDetailsVwConfig());

            #endregion==================End OPM====================

            #region =============PUR=====================================
            modelBuilder.ApplyConfiguration(new PurTransactionsPaymentConfig());
            modelBuilder.ApplyConfiguration(new PurExpenseConfig());
            modelBuilder.ApplyConfiguration(new PurExpensesVwConfig());

            modelBuilder.ApplyConfiguration(new PurDiscountCatalogConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountCatalogVwConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountCatalogAllVwConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountByAmountConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountByQtyConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountProductConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountProductsVwConfig());
            modelBuilder.ApplyConfiguration(new PurDiscountTypeConfig());

            modelBuilder.ApplyConfiguration(new PurAdditionalTypeConfig());
            modelBuilder.ApplyConfiguration(new PurAdditionalTypeVwConfig());
            modelBuilder.ApplyConfiguration(new PurItemsPriceMConfig());
            modelBuilder.ApplyConfiguration(new PurItemsPriceMVwConfig());
            modelBuilder.ApplyConfiguration(new PurItemsPriceDConfig());
            modelBuilder.ApplyConfiguration(new PurItemsPriceDVwConfig());

            modelBuilder.ApplyConfiguration(new PurTransactionConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsVwConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsSupplierConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsSupplierVwConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsProductConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsProductsVwConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsTypeConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsDiscountConfig());
            modelBuilder.ApplyConfiguration(new PurTransactionsDiscountVwConfig());
            modelBuilder.ApplyConfiguration(new PurRqfWorkFlowEvaluationConfig());
            modelBuilder.ApplyConfiguration(new PurRqfWorkFlowEvaluationVwConfig());
            modelBuilder.ApplyConfiguration(new PurPaymentTermConfig());
            #endregion

            #region ======================== WF ==============================================
            modelBuilder.ApplyConfiguration(new WfAppTypeTableConfig());
            modelBuilder.ApplyConfiguration(new WfDynamicAttributeDataTypeConfig());
            modelBuilder.ApplyConfiguration(new WfDynamicValueConfig());
            modelBuilder.ApplyConfiguration(new WfDynamicTableValueConfig());
            modelBuilder.ApplyConfiguration(new WfLookUpCatagoryConfig());
            modelBuilder.ApplyConfiguration(new WfLookupTypeConfig());
            modelBuilder.ApplyConfiguration(new WfStepLevelConfig());
            modelBuilder.ApplyConfiguration(new WfStepsNotificationConfig());
            modelBuilder.ApplyConfiguration(new WfStepsTypeConfig());

            modelBuilder.ApplyConfiguration(new WfAppGroupConfig());
            modelBuilder.ApplyConfiguration(new WfAppGroupsVwConfig());

            modelBuilder.ApplyConfiguration(new WfAppTypeConfig());
            modelBuilder.ApplyConfiguration(new WfAppTypeVwConfig());

            modelBuilder.ApplyConfiguration(new WfApplicationConfig());
            modelBuilder.ApplyConfiguration(new WfApplicationsVwConfig());

            modelBuilder.ApplyConfiguration(new WfApplicationsAssigneConfig());
            modelBuilder.ApplyConfiguration(new WfApplicationsAssignesVwConfig());

            modelBuilder.ApplyConfiguration(new WfApplicationsAssignesReplyConfig());
            modelBuilder.ApplyConfiguration(new WfApplicationsAssignesReplyVwConfig());

            modelBuilder.ApplyConfiguration(new WfApplicationsCommentConfig());
            modelBuilder.ApplyConfiguration(new WfApplicationsCommentsVwConfig());

            modelBuilder.ApplyConfiguration(new WfApplicationsStatusConfig());
            modelBuilder.ApplyConfiguration(new WfApplicationsStatusVwConfig());

            modelBuilder.ApplyConfiguration(new WfDynamicAttributeConfig());
            modelBuilder.ApplyConfiguration(new WfDynamicAttributesVwConfig());

            modelBuilder.ApplyConfiguration(new WfDynamicAttributesTableConfig());
            modelBuilder.ApplyConfiguration(new WfDynamicAttributesTableVwConfig());

            modelBuilder.ApplyConfiguration(new WfEscalationConfig());
            modelBuilder.ApplyConfiguration(new WfEscalationVwConfig());

            modelBuilder.ApplyConfiguration(new WfLookupDataConfig());
            modelBuilder.ApplyConfiguration(new WfLookupDataVwConfig());

            modelBuilder.ApplyConfiguration(new WfStatusConfig());
            modelBuilder.ApplyConfiguration(new WfStatusVwConfig());

            modelBuilder.ApplyConfiguration(new WfStepConfig());
            modelBuilder.ApplyConfiguration(new WfStepsVwConfig());

            modelBuilder.ApplyConfiguration(new WfStepsTransactionConfig());
            modelBuilder.ApplyConfiguration(new WfStepsTransactionsVwConfig());

            modelBuilder.ApplyConfiguration(new WfLayoutAttributeConfig());

            modelBuilder.ApplyConfiguration(new WfAppCommitteeConfig());
            modelBuilder.ApplyConfiguration(new WfAppCommitteesVwConfig());

            modelBuilder.ApplyConfiguration(new WfActionsCommitteeConfig());

            modelBuilder.ApplyConfiguration(new WfAppCommitteesMemberConfig());
            modelBuilder.ApplyConfiguration(new WfAppCommitteesMembersVwConfig());

            modelBuilder.ApplyConfiguration(new WfAppMemberConfig());
            modelBuilder.ApplyConfiguration(new WfAppMembersVwConfig());

            modelBuilder.ApplyConfiguration(new WfStepsCommitteeConfig());
            modelBuilder.ApplyConfiguration(new WfStepsCommitteesVwConfig());

            #endregion ======================= End WF ========================================

            #region ======== HOT============
            modelBuilder.ApplyConfiguration(new EntityConfigurations.HOT.HotFloorConfig());
            modelBuilder.ApplyConfiguration(new HotFloorsVwConfig());
            modelBuilder.ApplyConfiguration(new HotGroupConfig());
            modelBuilder.ApplyConfiguration(new HotRoomAssetConfig());
            modelBuilder.ApplyConfiguration(new HotRoomAssetsVwConfig());
            modelBuilder.ApplyConfiguration(new HotRoomConfig());
            modelBuilder.ApplyConfiguration(new HotRoomServiceConfig());
            modelBuilder.ApplyConfiguration(new HotServiceConfig());
            modelBuilder.ApplyConfiguration(new HotServicesVwConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsCompanionConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsCompanionVwConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsPaymentConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsRoomConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsRoomVwConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsServiceConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsServicesVwConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsStatusConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsTypeConfig());
            modelBuilder.ApplyConfiguration(new HotTransactionsVwConfig());
            modelBuilder.ApplyConfiguration(new HotTypeRoomConfig());



            #endregion

            #region ======= WH ========================
            modelBuilder.ApplyConfiguration(new WhUnitConfig());
            modelBuilder.ApplyConfiguration(new WhItemsUnitListVwConfig());
            modelBuilder.ApplyConfiguration(new WhItemsCatagoryConfig());
            modelBuilder.ApplyConfiguration(new WhAccountTypeConfig());
            modelBuilder.ApplyConfiguration(new WhItemConfig());
            modelBuilder.ApplyConfiguration(new WhItemsVwConfig());
            modelBuilder.ApplyConfiguration(new WhInventoryConfig());
            modelBuilder.ApplyConfiguration(new WhInventoriesVwConfig());
            modelBuilder.ApplyConfiguration(new WhTransactionsMasterConfig());
            modelBuilder.ApplyConfiguration(new WhTransactionsMasterVwConfig());
            modelBuilder.ApplyConfiguration(new WhItemsComponentConfig());
            modelBuilder.ApplyConfiguration(new WhItemsComponentsVwConfig());
            modelBuilder.ApplyConfiguration(new WhItemsSerialConfig());
            modelBuilder.ApplyConfiguration(new WhItemsSerialsVwConfig());
            modelBuilder.ApplyConfiguration(new WhActualInventorySerialeConfig());
            modelBuilder.ApplyConfiguration(new WhActualInventorySerialesVwConfig());
            modelBuilder.ApplyConfiguration(new WhItemsCatagoriesVwConfig());
            modelBuilder.ApplyConfiguration(new WhItemsSectionConfig());
            modelBuilder.ApplyConfiguration(new WhItemsSectionsVwConfig());
            modelBuilder.ApplyConfiguration(new WhInventorySectionConfig());
            modelBuilder.ApplyConfiguration(new WhItemTemplateConfig());
            modelBuilder.ApplyConfiguration(new WhItemTemplateVwConfig());
            modelBuilder.ApplyConfiguration(new WhTemplateConfig());
            modelBuilder.ApplyConfiguration(new WhItemsBatchConfig());
            modelBuilder.ApplyConfiguration(new WhItemsBatchListVwConfig());
            modelBuilder.ApplyConfiguration(new WhTransactionsTypeConfig());
            modelBuilder.ApplyConfiguration(new WhTransactionsTypeVwConfig());


            #endregion ======= WH ========================

            #region =============== FXA ===========================
            modelBuilder.ApplyConfiguration(new FxaAdditionsExclusionConfig());
            modelBuilder.ApplyConfiguration(new FxaAdditionsExclusionVwConfig());

            modelBuilder.ApplyConfiguration(new FxaAdditionsExclusionTypeConfig());
            modelBuilder.ApplyConfiguration(new FxaDepreciationMethodConfig());

            modelBuilder.ApplyConfiguration(new FxaFixedAssetConfig());
            modelBuilder.ApplyConfiguration(new FxaFixedAssetVwConfig());
            modelBuilder.ApplyConfiguration(new FxaFixedAssetVw2Config());

            modelBuilder.ApplyConfiguration(new FxaFixedAssetTransferConfig());
            modelBuilder.ApplyConfiguration(new FxaFixedAssetTransferVwConfig());

            modelBuilder.ApplyConfiguration(new FxaFixedAssetTypeConfig());
            modelBuilder.ApplyConfiguration(new FxaFixedAssetTypeVwConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionConfig());
            modelBuilder.ApplyConfiguration(new FxaTransactionsVwConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionsAssetConfig());
            modelBuilder.ApplyConfiguration(new FxaTransactionsAssetsVwConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionsPaymentConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionsTypeConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionsProductConfig());
            modelBuilder.ApplyConfiguration(new FxaTransactionsProductsVwConfig());

            modelBuilder.ApplyConfiguration(new FxaTransactionsRevaluationConfig());
            modelBuilder.ApplyConfiguration(new FxaTransactionsRevaluationVwConfig());
            #endregion

            #region =============== CRM ===========================

            modelBuilder.ApplyConfiguration(new CrmEmailTemplateAttachConfig());
            modelBuilder.ApplyConfiguration(new CrmEmailTemplateConfig());

            #endregion

            #region =============HD==========================
            modelBuilder.ApplyConfiguration(new HdTickectConfig());
            #endregion

            #region =============TS==========================

            modelBuilder.ApplyConfiguration(new TsTaskConfig());
            modelBuilder.ApplyConfiguration(new TsTasksVwConfig());
            modelBuilder.ApplyConfiguration(new TsTaskStatusVwConfig());
            modelBuilder.ApplyConfiguration(new TsTasksResponseConfig());
            modelBuilder.ApplyConfiguration(new TsTasksResponseVwConfig());
            modelBuilder.ApplyConfiguration(new TsAppointmentConfig());
            modelBuilder.ApplyConfiguration(new TsAppointmentVwConfig());
            modelBuilder.ApplyConfiguration(new TsTasksSchedulerConfig());
            modelBuilder.ApplyConfiguration(new TsTasksSchedulerVwConfig());
            #endregion

            #region =============Integra==========================

            modelBuilder.ApplyConfiguration(new IntegraSystemConfig());
            modelBuilder.ApplyConfiguration(new IntegraPropertyConfig());
            modelBuilder.ApplyConfiguration(new IntegraPropertiesVwConfig());
            modelBuilder.ApplyConfiguration(new IntegraPropertyValueConfig());
            modelBuilder.ApplyConfiguration(new IntegraPropertyValuesVwConfig());
            modelBuilder.ApplyConfiguration(new IntegraTableConfig());
            modelBuilder.ApplyConfiguration(new IntegraFieldConfig());
            #endregion

            #region =============== RE ===========================
            modelBuilder.ApplyConfiguration(new ReTransactionsInstallmentConfig());
            modelBuilder.ApplyConfiguration(new ReTransactionsInstallmentsVwConfig());
            modelBuilder.ApplyConfiguration(new ReTransactionConfig());
            modelBuilder.ApplyConfiguration(new ReTransactionsVwConfig());
            #endregion

            #region =============== Sch ===========================
            modelBuilder.ApplyConfiguration(new SchTransactionsInstallmentConfig());
            modelBuilder.ApplyConfiguration(new SchTransactionsInstallmentsVwConfig());

            modelBuilder.ApplyConfiguration(new SchTransactionsTransportationVwConfig());
            modelBuilder.ApplyConfiguration(new SchTransactionsTransportationInstallmentsVwConfig());
            modelBuilder.ApplyConfiguration(new SchTransactionsTransportationPrintVwConfig());
            #endregion

            #region =============== Maintenance ===========================
            modelBuilder.ApplyConfiguration(new MaintTransactionsInstallmentConfig());
            modelBuilder.ApplyConfiguration(new MaintTransactionsInstallmentsVwConfig());
            #endregion

            #region =============== Trans ===========================
            modelBuilder.ApplyConfiguration(new TransTransactionConfig());
            modelBuilder.ApplyConfiguration(new TransTransactionsVwConfig());
            #endregion
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }
        //   partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
