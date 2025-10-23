using Logix.Application.DTOs.HR;
using Logix.Domain.ACC;
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
using Logix.Domain.WF;
using Logix.Domain.WH;
using Microsoft.EntityFrameworkCore;

namespace Logix.Application.Common
{
    public interface IApplicationDbContext
    {


        #region ============== Main ======================
        DbSet<SysSystem> SysSystems { get; }
        DbSet<SysScreen> SysScreens { get; }
        DbSet<SysAnnouncement> SysAnnouncements { get; }
        DbSet<SysAnnouncementVw> SysAnnouncementVws { get; }
        DbSet<SysAnnouncementLocationVw> SysAnnouncementLocationVws { get; }
        DbSet<SysLookupCategory> SysLookupCategories { get; }
        DbSet<SysLookupData> SysLookupData { get; }
        DbSet<SysLookupDataVw> SysLookupDataVws { get; }
        DbSet<SysDepartment> SysDepartments { get; }
        DbSet<SysDepartmentVw> SysDepartmentVws { get; }
        DbSet<SysDepartmentCatagory> SysDepartmentCatagories { get; }
        DbSet<SysGroup> SysGroups { get; }
        DbSet<SysGroupVw> SysGroupVws { get; }
        DbSet<SysScreenPermission> SysScreenPermissions { get; }
        DbSet<SysScreenPermissionVw> SysScreenPermissionVws { get; }
        DbSet<SysBranchVw> SysBranchVws { get; }
        DbSet<SysNotification> SysNotifications { get; }
        DbSet<SysNotificationsVw> SysNotificationsVws { get; }
        DbSet<SysProperty> SysProperties { get; }
        DbSet<SysPropertiesVw> SysPropertiesVws { get; }
        DbSet<SysPropertyValue> SysPropertyValues { get; }
        DbSet<SysPropertyValuesVw> SysPropertyValuesVws { get; }
        DbSet<SysScreenProperty> SysScreenProperties { get; }
        DbSet<SysScreenPermissionProperty> SysScreenPermissionProperties { get; }
        DbSet<SysScreenPermissionPropertiesVw> SysScreenPermissionPropertiesVws { get; }
        DbSet<SysCustomerType> SysCustomerTypes { get; }
        DbSet<SysCustomerGroup> SysCustomerGroups { get; }
        DbSet<SysCustomerGroupAccount> SysCustomerGroupAccounts { get; }
        DbSet<SysCustomerGroupAccountsVw> SysCustomerGroupAccountsVws { get; }
        DbSet<SysLicense> SysLicenses { get; }
        DbSet<SysLicensesVw> SysLicensesVws { get; }
        DbSet<SysFavMenu> SysFavMenus { get; }
        DbSet<SysFile> SysFiles { get; }

        DbSet<InvestBranch> InvestBranches { get; }
        DbSet<InvestBranchVw> InvestBranceshVws { get; }

        DbSet<SysCurrency> SysCurrency { get; }
        DbSet<SysCurrencyListVw> SysCurrencyListVws { get; }

        DbSet<SysExchangeRate> SysExchangeRates { get; }
        DbSet<SysExchangeRateVw> SysExchangeRatesVws { get; }
        DbSet<SysExchangeRateListVW> SysExchangeRateListsVws { get; }

        DbSet<SysUser> SysUsers { get; }
        DbSet<SysUserVw> SysUserVws { get; }
        DbSet<SysScreenInstalled> SysScreenInstalleds { get; }
        DbSet<SysScreenInstalledVw> SysScreenInstalledVws { get; }
        DbSet<SysCites> SysCites { get; }

        DbSet<SysCustomerBranch> SysCustomerBranches { get; }
        DbSet<SysCustomerBranchVw> SysCustomerBranchVws { get; }
        DbSet<SysCustomerCoType> SysCustomerCoTypes { get; }

        DbSet<SysPoliciesProcedure> SysPoliciesProcedures { get; }
        DbSet<SysPoliciesProceduresVw> SysPoliciesProceduresVws { get; }

        DbSet<SysVatGroup> SysVatGroups { get; }
        DbSet<SysCustomerVw> SysCustomerVws { get; }
        DbSet<SysCustomerContact> SysCustomerContacts { get; }
        DbSet<SysCustomerContactVw> SysCustomerContactVws { get; }
        DbSet<SysCustomerFile> SysCustomerFiles { get; }
        DbSet<SysCustomerFilesVw> SysCustomerFilesVws { get; }

        DbSet<SysTemplate> SysTemplates { get; }
        DbSet<SysTemplateVw> SysTemplateVws { get; }

        DbSet<SysForm> SysForms { get; }
        DbSet<SysFormsVw> SysFormsVws { get; }

        DbSet<SysSettingExport> SysSettingExports { get; }
        DbSet<SysSettingExportVw> SysSettingExportVws { get; }
        DbSet<SysActivityLog> SysActivityLogs { get; }
        DbSet<SysActivityLogVw> SysActivityLogVws { get; }
        DbSet<SysUserLogTime> SysUserLogTimes { get; }
        DbSet<SysUserLogTimeVw> SysUserLogTimeVws { get; }
        DbSet<SysUserTracking> SysUserTrackings { get; }
        DbSet<SysUserTrackingVw> SysUserTrackingVws { get; }

        DbSet<Domain.Main.InvestEmployee> InvestEmployees { get; }
        DbSet<InvestEmployeeVvw> InvestEmployeeVvws { get; }

        DbSet<SysUserType> SysUserTypes { get; }
        DbSet<SysUserType2> SysUserTypes2 { get; }

        DbSet<SysVersion> SysVersions { get; }

        DbSet<SysDynamicAttribute> SysDynamicAttributes { get; }
        DbSet<SysDynamicAttributesVw> SysDynamicAttributesVws { get; }
        DbSet<SysDynamicAttributeDataType> SysDynamicAttributeDataTypes { get; }
        DbSet<SysScreenVw> SysScreenVws { get; }
        DbSet<SysVatGroupVw> SysVatGroupVws { get; }
        DbSet<SysMailServer> SysMailServer { get; }
        DbSet<SysCountry> SysCountrys { get; }
        DbSet<SysCountryVw> SysCountryVws { get; }

        DbSet<SysNotificationsMang> SysNotificationsMangs { get; }
        DbSet<SysNotificationsMangVw> SysNotificationsMangVws { get; }

        DbSet<SysTable> SysTables { get; }
        DbSet<SysTableField> SysTableFields { get; }

        DbSet<SysActivityType> SysActivityTypes { get; }
        DbSet<SysPackagesPropertyValue> SysPackagesPropertyValue { get; }
        DbSet<SysPackage> SysPackages { get; }

        DbSet<SysSms> SysSms { get; }


        DbSet<SysWebHook> SysWebHooks { get; }
        DbSet<SysWebHookVw> SysWebHookVws { get; }

        DbSet<SysFilesDocument> SysFilesDocuments { get; }
        DbSet<SysFilesDocumentVw> SysFilesDocumentVws { get; }
        DbSet<SysRecordWebhook> SysRecordWebhooks { get; }
        DbSet<SysRecordWebhookVw> SysRecordWebhookVws { get; }
        DbSet<SysDynamicValue> SysDynamicValues { get; }
        DbSet<SysScreenWorkflow> SysScreenWorkflows { get; }
        DbSet<SysPeriod> SysPeriods { get; }

        DbSet<SysLibraryFile> SysLibraryFiles { get; }
        DbSet<SysLibraryFilesVw> SysLibraryFilesVws { get; }
        DbSet<SysResetPassword> SysResetPasswords { get; }
        DbSet<SysCreateUserRequst> SysCreateUserRequsts { get; }
        DbSet<SysCreateUserRequstVw> SysCreateUserRequstVws { get; }
        DbSet<ChatMessage> ChatMessages { get; }
        DbSet<SysMethodTypeApi> SysMethodTypeApis { get; }
        DbSet<SysProcessScreenWebHook> SysProcessScreenWebHooks { get; }
        DbSet<SysWebHookAuth> SysWebHookAuths { get; }
        DbSet<SysRecordWebhookAuth> SysRecordWebhookAuths { get; }
        DbSet<SysRecordWebhookAuthVw> SysRecordWebhookAuthVws { get; }
        DbSet<SysWebHookAuthVw> SysWebHookAuthVws { get; }

        DbSet<SysInvoiceAccordingType> SysInvoiceAccordingTypes { get; }
        DbSet<SysZatcaInvoiceType> SysZatcaInvoiceTypes { get; }

        DbSet<SysZatcaInvoiceTransaction> SysZatcaInvoiceTransactions { get; }
        DbSet<SysZatcaInvoiceTransactionsSimulation> SysZatcaInvoiceTransactionsSimulations { get; }
        DbSet<SysZatcaReportingResult> SysZatcaReportingResults { get; }
        DbSet<SysZatcaReportingResultsSimulation> SysZatcaReportingResultsSimulations { get; }
        DbSet<SysZatcaSignedXml> SysZatcaSignedXmls { get; }
        DbSet<SysZatcaSignedXmlSimulation> SysZatcaSignedXmlSimulations { get; }
        DbSet<ZatcaCreditDebitNote> ZatcaCreditDebitNotes { get; }
        DbSet<ZatcaVatcategoriesReason> ZatcaVatcategoriesReasons { get; }
        DbSet<SysPropertyClassification> SysPropertyClassifications { get; }

        DbSet<SysCustomersFilesSetting> SysCustomersFilesSettings { get; }
        DbSet<SysCustomersFilesSettingsVw> SysCustomersFilesSettingsVws { get; }

        #endregion

        #region ======= HR ========================
        DbSet<HrEmployee> HrEmployees { get; }
        DbSet<HrEmployeeVw> HrEmployeeVws { get; }
        DbSet<HrAttDay> HrAttDays { get; }
        DbSet<HrEvaluationAnnualIncreaseConfig> HrEvaluationAnnualIncreaseConfigs { get; }
        DbSet<HrNotification> HrNotifications { get; }
        DbSet<HrNotificationsVw> HrNotificationsVws { get; }
        DbSet<HrCompetence> HrCompetences { get; }
        DbSet<HrCompetencesVw> HrCompetencesVws { get; }
        DbSet<HrCompetencesCatagory> HrCompetencesCatagorys { get; }
        DbSet<HrKpiTemplatesCompetence> HrKpiTemplatesCompetences { get; }

        DbSet<HrTrainingBag> HrTrainingBags { get; }
        DbSet<HrTrainingBagVw> HrTrainingBagVws { get; }
        DbSet<HrPolicy> HrPolicys { get; }
        DbSet<HrPoliciesVw> HrPoliciesVws { get; }
        DbSet<HrPoliciesType> HrPoliciesTypes { get; }
        DbSet<HrKpiTemplate> HrKpiTemplates { get; }
        DbSet<HrKpiTemplatesVw> HrKpiTemplatesVws { get; }
        DbSet<HrSetting> HrSettings { get; }
        DbSet<HrSalaryGroup> HrSalaryGroups { get; }
        DbSet<HrSalaryGroupVw> HrSalaryGroupVws { get; }
        DbSet<HrVacationsType> HrVacationsTypes { get; }
        DbSet<HrVacationsTypeVw> HrVacationsTypeVws { get; }
        DbSet<HrDisciplinaryCase> HrDisciplinaryCases { get; }
        DbSet<HrAbsence> HrAbsences { get; }
        DbSet<HrVacation> HrVacations { get; }
        DbSet<HrDelay> HrDelays { get; }
        //DbSet<HrDisciplinaryCase> HrDisciplinaryCases { get; } 
        DbSet<HrCardTemplate> HrCardTemplates { get; }
        DbSet<InvestMonth> investMonths { get; }
        DbSet<HrOverTimeD> hrOverTimeDs { get; }
        DbSet<HrOverTimeDVw> hrOverTimeDVws { get; }
        DbSet<HrPayrollType> HrPayrollTypes { get; }

        DbSet<HrAttShift> HrAttShifts { get; }
        DbSet<HrJobProgramVw> HrJobProgramVws { get; }
        DbSet<HrJobVw> HrJobVws { get; }

        DbSet<HrDirectJob> HrDirectJobs { get; }
        DbSet<HrDirectJobVw> HrDirectJobVws { get; }

        DbSet<HrNote> HrNotes { get; }
        DbSet<HrNoteVw> HrNoteVws { get; }

        DbSet<HrAllowanceDeductionM> HrAllowanceDeductionMs { get; }
        DbSet<HrAllowanceDeductionTempOrFix> HrAllowanceDeductionTempOrFixs { get; }
        DbSet<HrArchiveFilesDetail> HrArchiveFilesDetails { get; }
        DbSet<HrArchiveFilesDetailsVw> HrArchiveFilesDetailsVws { get; }
        DbSet<HrAssignman> HrAssignmans { get; }
        DbSet<HrAssignmenVw> HrAssignmenVws { get; }
        DbSet<HrAttAction> HrAttActions { get; }
        DbSet<HrAttLocationEmployee> HrAttLocationEmployees { get; }
        DbSet<HrAttLocationEmployeeVw> HrAttLocationEmployeeVws { get; }
        DbSet<HrAttShiftClose> HrAttShiftCloses { get; }
        DbSet<HrAttShiftCloseVw> HrAttShiftCloseVws { get; }
        DbSet<HrAttShiftCloseD> HrAttShiftCloseDs { get; }
        DbSet<HrAuthorization> HrAuthorizations { get; }
        DbSet<HrAuthorizationVw> HrAuthorizationVws { get; }
        DbSet<HrAttendanceBioTime> HrAttendanceBioTimes { get; }
        DbSet<HrCheckInOut> HrCheckInOuts { get; }
        DbSet<HrCheckInOutVw> HrCheckInOutVws { get; }
        DbSet<HrClearance> HrClearances { get; }
        DbSet<HrClearanceVw> HrClearanceVws { get; }
        DbSet<HrClearanceType> HrClearanceTypes { get; }
        DbSet<HrClearanceTypeVw> HrClearanceTypeVws { get; }
        DbSet<HrCompensatoryVacation> HrCompensatoryVacations { get; }
        DbSet<HrCompensatoryVacationsVw> HrCompensatoryVacationsVws { get; }
        DbSet<HrContracte> HrContractes { get; }
        DbSet<HrContractesVw> HrContractesVws { get; }
        DbSet<HrClearanceMonth> HrClearanceMonths { get; }
        DbSet<HrClearanceMonthsVw> HrClearanceMonthsVws { get; }
        DbSet<HrCostType> HrCostTypes { get; }
        DbSet<HrCostTypeVw> HrCostTypeVws { get; }
        DbSet<HrCustody> HrCustodys { get; }
        DbSet<HrCustodyVw> HrCustodyVws { get; }
        DbSet<HrCustodyItem> HrCustodyItems { get; }
        DbSet<HrCustodyItemsVw> HrCustodyItemsVws { get; }
        DbSet<HrCustodyItemsProperty> HrCustodyItemsPropertys { get; }
        DbSet<HrCustodyRefranceType> HrCustodyRefranceTypes { get; }
        DbSet<HrCustodyType> HrCustodyTypes { get; }
        DbSet<HrDecision> HrDecisions { get; }
        DbSet<HrDecisionsEmployee> HrDecisionsEmployees { get; }
        DbSet<HrDecisionsEmployeeVw> HrDecisionsEmployeeVws { get; }
        DbSet<HrHoliday> HrHolidays { get; }
        DbSet<HrHolidayVw> HrHolidayVws { get; }
        DbSet<HrPermission> HrPermissions { get; }
        DbSet<HrPermissionsVw> HrPermissionsVws { get; }
        DbSet<HrAttShiftTimeTable> HrAttShiftTimeTables { get; }
        DbSet<HrAttShiftTimeTableVw> HrAttShiftTimeTableVws { get; }
        DbSet<HrEmployeeCost> HrEmployeeCosts { get; }
        DbSet<HrInsurancePolicy> HrInsurancePolicys { get; }
        DbSet<HrInsurance> HrInsurances { get; }
        DbSet<HrInsuranceEmp> HrInsuranceEmps { get; }
        DbSet<HrInsuranceEmpVw> HrInsuranceEmpVws { get; }

        DbSet<HrEmployeeCostVw> HrEmployeeCostVws { get; }
        DbSet<HrJob> HrJobs { get; }
        DbSet<HrJobDescription> HrJobDescriptions { get; }
        DbSet<HrJobEmployeeVw> HrJobEmployeeVws { get; }
        DbSet<HrJobLevel> HrJobLevels { get; }
        DbSet<HrRecruitmentVacancy> HrRecruitmentVacancys { get; }
        DbSet<HrRecruitmentVacancyVw> HrRecruitmentVacancyVws { get; }
        DbSet<HrRecruitmentApplication> HrRecruitmentApplications { get; }
        DbSet<HrRecruitmentApplicationVw> HrRecruitmentApplicationVws { get; }
        DbSet<HrRecruitmentCandidate> HrRecruitmentCandidates { get; }
        DbSet<HrRecruitmentCandidateVw> HrRecruitmentCandidateVws { get; }
        DbSet<HrVacancyStatusVw> HrVacancyStatusVws { get; }
        DbSet<HrJobGrade> HrJobGrades { get; }
        DbSet<HrJobGradeVw> HrJobGradeVws { get; }

        DbSet<HrRecruitmentCandidateKpi> HrRecruitmentCandidateKpis { get; }
        DbSet<HrRecruitmentCandidateKpiVw> HrRecruitmentCandidateKpiVws { get; }
        DbSet<HrRecruitmentCandidateKpiD> HrRecruitmentCandidateKpiDs { get; }
        DbSet<HrRecruitmentCandidateKpiDVw> HrRecruitmentCandidateKpiDVws { get; }
        DbSet<HrPayroll> HrPayrolls { get; }
        DbSet<HrPayrollVw> HrPayrollVws { get; }
        DbSet<HrTicket> HrTickets { get; }
        DbSet<HrTicketVw> HrTicketVws { get; }
        DbSet<HrVisaVw> HrVisaVws { get; }
        DbSet<HrVisa> HrVisas { get; }
        DbSet<HrFixingEmployeeSalary> HrFixingEmployeeSalarys { get; }
        DbSet<HrFixingEmployeeSalaryVw> HrFixingEmployeeSalaryVws { get; }
        DbSet<HrLeaveType> HrLeaveTypes { get; }
        DbSet<HrLeaveTypeVw> HrLeaveTypeVws { get; }
        DbSet<HrPayrollAllowanceDeduction> HrPayrollAllowanceDeductions { get; }
        DbSet<HrPayrollAllowanceDeductionVw> HrPayrollAllowanceDeductionVws { get; }
        DbSet<HrLoanInstallmentPayment> HrLoanInstallmentPayments { get; }
        DbSet<HrLoanInstallmentPaymentVw> HrLoanInstallmentPaymentVws { get; }
        DbSet<HrLoanInstallment> HrLoanInstallments { get; }
        DbSet<HrLoanInstallmentVw> HrLoanInstallmentVws { get; }
        DbSet<HrPayrollNote> HrPayrollNotes { get; }
        DbSet<HrPayrollNoteVw> HrPayrollNoteVws { get; }
        DbSet<HrDecisionsType> HrDecisionsTypes { get; }
        DbSet<HrDecisionsTypeVw> HrDecisionsTypeVws { get; }
        DbSet<HrDecisionsTypeEmployee> HrDecisionsTypeEmployees { get; }
        DbSet<HrDecisionsTypeEmployeeVw> HrDecisionsTypeEmployeeVws { get; }

        DbSet<HrRequestVw> HrRequestVws { get; }
        DbSet<HrEmployeeLocationVw> HrEmployeeLocationVws { get; }
        DbSet<HrAttShiftEmployee> HrAttShiftEmployees { get; }
        DbSet<HrAttShiftEmployeeVw> HrAttShiftEmployeeVws { get; }
        DbSet<HrAttShiftEmployeeMVw> HrAttShiftEmployeeMVws { get; }
        DbSet<HrAttCheckShiftEmployeeVw> HrAttCheckShiftEmployeeVws { get; }
        DbSet<HRAttendanceReportDto> HRAttendanceReportDtos { get; }
        DbSet<HRAttendanceReport5Dto> HRAttendanceReport5Dtos { get; }
        DbSet<HRAttendanceReport4Dto> HRAttendanceReport4Dtos { get; }
        DbSet<HRAddMultiAttendanceDto> HRAddMultiAttendanceDtos { get; }
        DbSet<HrOpeningBalance> HrOpeningBalances { get; }
        DbSet<HrOpeningBalanceVw> HrOpeningBalanceVws { get; }
        DbSet<HrOpeningBalanceType> HrOpeningBalanceTypes { get; }
        DbSet<HrPayrollAllowanceVw> HrPayrollAllowanceVws { get; }
        DbSet<HRPayrollCreate2SpDto> HRPayrollCreate2SpDtos { get; }
        DbSet<HrPsAllowanceDeduction> HrPsAllowanceDeductions { get; }
        DbSet<HrPsAllowanceDeductionVw> HrPsAllowanceDeductionVws { get; }
        DbSet<HrPreparationSalariesVw> HrPreparationSalariesVws { get; }
        DbSet<HrPreparationSalary> HrPreparationSalaries { get; }
        public DbSet<HrPayrollDeductionAccountsVw> HrPayrollDeductionAccountsVws { get; }
        public DbSet<HrPayrollCostcenterVw> HrPayrollCostcenterVws { get; }
        public DbSet<HrPayrollCostcenter> HrPayrollCostcenters { get; }
        public DbSet<HrPayrollAllowanceAccountsVw> HrPayrollAllowanceAccountsVws { get; }
        public DbSet<HrNotificationsReply> hrNotificationsReplies { get; }
        public DbSet<HrLoanPaymentVw> HrLoanPaymentVws { get; }
        public DbSet<HrLoanPayment> HrLoanPayments { get; }
        public DbSet<HrPermissionReasonVw> HrPermissionReasonVws { get; }
        public DbSet<HrPermissionTypeVw> HrPermissionTypeVws { get; }
        public DbSet<HrEmpStatusHistoryVw> HrEmpStatusHistoryVws { get; }
        public DbSet<HrEmpStatusHistory> HrEmpStatusHistorys { get; }




        public DbSet<HrEducation> HrEducations { get; }
        public DbSet<HrEducationVw> HrEducationVws { get; }
        public DbSet<HrSkill> HrSkills { get; }
        public DbSet<HrSkillsVw> HrSkillsVws { get; }
        public DbSet<HrLanguage> HrLanguages { get; }
        public DbSet<HrLanguagesVw> HrLanguagesVws { get; }
        public DbSet<HrFile> HrFiles { get; }
        public DbSet<HrWorkExperience> HrWorkExperiences { get; }
        public DbSet<HrGosiEmployee> HrGosiEmployees { get; }
        public DbSet<HrGosiEmployeeVw> HrGosiEmployeeVws { get; }
        public DbSet<HrGosi> HrGosis { get; }
        public DbSet<HrGosiVw> HrGosiVws { get; }
        public DbSet<HrGosiEmployeeAccVw> HrGosiEmployeeAccVws { get; }
        public DbSet<EmployeeGosiDto> EmployeeGosiDtos { get; }
        public DbSet<HrVacationsDayType> HrVacationsDayTypeS { get; }
        public DbSet<HrEmployeeLeaveResultDto> HrEmployeeLeaveResultDtos { get; }
        public DbSet<HRAttendanceTotalReportDto> HRAttendanceTotalReportDtos { get; }
        public DbSet<HrIncrementType> HrIncrementTypes { get; }
        public DbSet<HrIncrementsAllowanceDeductionVw> HrIncrementsAllowanceDeductionVws { get; }
        public DbSet<HrEmployeeIncremenResultDto> HrEmployeeIncremenResulttDtos { get; }
        public DbSet<HRAttendanceTotalReportNewSPDto> HRAttendanceTotalReportNewSPDtos { get; }
        public DbSet<HRAttendanceReport6SP> HRAttendanceReport6SPs { get; }
        public DbSet<HrPerformanceVw> HrPerformanceVws { get; }
        public DbSet<HrPerformance> HrPerformances { get; }
        public DbSet<HrPerformanceTypeVw> HrPerformanceTypeVws { get; }
        public DbSet<HrPerformanceStatusVw> HrPerformanceStatusVws { get; }
        public DbSet<HrPerformanceForVw> HrPerformanceForVws { get; }
        public DbSet<HrKpiTemplatesJob> HrKpiTemplatesJobs { get; }
        public DbSet<HrKpiTemplatesJobsVw> HrKpiTemplatesJobsVws { get; }
        public DbSet<HrEmpGoalIndicatorsVw> HrEmpGoalIndicatorsVws { get; }
        public DbSet<HrEmpGoalIndicator> HrEmpGoalIndicators { get; }
        public DbSet<HrEmpGoalIndicatorsEmployee> HrEmpGoalIndicatorsEmployees { get; }
        public DbSet<HrEmpGoalIndicatorsCompetence> HrEmpGoalIndicatorsCompetences { get; }
        public DbSet<HrDefinitionSalaryEmp> HrDefinitionSalaryEmps { get; }
        public DbSet<HREmpClearanceSpDto> HREmpClearanceSpDtos { get; }
        public DbSet<HrActualAttendanceVw> HrActualAttendanceVws { get; }
        public DbSet<HrActualAttendance> HrActualAttendances { get; }
        public DbSet<HrPayrollDeductionVw> HrPayrollDeductionVws { get; }
        public DbSet<HrGosiTypeVw> HrGosiTypeVws { get; }
        public DbSet<HrFlexibleWorkingVw> HrFlexibleWorkingVws { get; }
        public DbSet<HrFlexibleWorking> HrFlexibleWorkings { get; }
        public DbSet<HrFlexibleWorkingMaster> HrFlexibleWorkingMasters { get; }
        public DbSet<HrMandateLocationDetailesVw> HrMandateLocationDetailesVws { get; }
        public DbSet<HrMandateLocationDetaile> HrMandateLocationDetailes { get; }
        public DbSet<HrMandateLocationMaster> HrMandateLocationMasters { get; }
        public DbSet<HrMandateLocationMasterVw> HrMandateLocationMasterVws { get; }
        public DbSet<HrMandateRequestsMasterVw> HrMandateRequestsMasterVws { get; }
        public DbSet<HrMandateRequestsMaster> HrMandateRequestsMasters { get; }
        public DbSet<HrMandateRequestsDetaile> HrMandateRequestsDetailes { get; }
        public DbSet<HrMandateRequestsDetailesVw> HrMandateRequestsDetailesVws { get; }
        public DbSet<HrExpensesType> HrExpensesTypes { get; }
        public DbSet<HrExpensesTypeVw> HrExpensesTypeVws { get; }
        public DbSet<HrJobOffer> HrJobOffers { get; }
        public DbSet<HrJobOfferVw> HrJobOfferVws { get; }
        public DbSet<HrExpense> HrExpenses { get; }
        public DbSet<HrExpensesVw> HrExpensesVws { get; }
        public DbSet<HrExpensesEmployeesVw> HrExpensesEmployeesVws { get; }
        public DbSet<HrExpensesEmployee> HrExpensesEmployees { get; }
        public DbSet<HrJobOfferAdvantage> HrJobOfferAdvantages { get; }
        public DbSet<HrProvision> HrProvisions { get; }
        public DbSet<HrProvisionsVw> HrProvisionsVws { get; }
        public DbSet<HrProvisionsEmployee> HrProvisionsEmployees { get; }
        public DbSet<HrProvisionsEmployeeVw> HrProvisionsEmployeeVws { get; }
        public DbSet<HrProvisionsEmployeeAccVw> HrProvisionsEmployeeAccVws { get; }
        public DbSet<HRPayrollCreateAdvancedSpDto> HRPayrollCreateAdvancedSpDtos { get; }
        public DbSet<HrIncomeTaxVw> HrIncomeTaxVws { get; }
        public DbSet<HrIncomeTax> HrIncomeTaxs { get; }
        public DbSet<HrIncomeTaxSlide> HrIncomeTaxSlides { get; }

        public DbSet<HrIncomeTaxPeriod> HrIncomeTaxPeriods { get; }
        public DbSet<HRPayrollManuallCreateSpDto> HRPayrollManuallCreateSpDtos { get; }
        public DbSet<HrEmpGoalIndicatorsEmployeeVw> HrEmpGoalIndicatorsEmployeeVws { get; }
        DbSet<HrTimeZone> HrTimeZones { get; }

        public DbSet<HrRequestGoalsEmployeeDetail> HrRequestGoalsEmployeeDetails { get; }
        public DbSet<HrRequestGoalsEmployeeDetailsVw> HrRequestGoalsEmployeeDetailsVws { get; }
        DbSet<HrExpensesSchedule> HrExpensesSchedules { get; }
        DbSet<HrExpensesPayment> HrExpensesPayments { get; }
        DbSet<HrStructure> HrStructures { get; }
        DbSet<HrStructureVw> HrStructureVws { get; }
        DbSet<HrPayrollTransactionTypeValue> HrPayrollTransactionTypeValues { get; }
        DbSet<HrPayrollTransactionTypeValuesVw> HrPayrollTransactionTypeValuesVws { get; }
        DbSet<HrPayrollTransactionType> HrPayrollTransactionTypes { get; }

        DbSet<HrVisitScheduleLocation> HrVisitScheduleLocations { get; }
        DbSet<HrVisitScheduleLocationVw> HrVisitScheduleLocationVws { get; }
        DbSet<HrVisitStep> HrVisitSteps { get; }
        DbSet<HRAttendanceTotalReportSPDto> HRAttendanceTotalReportSPDtos { get; }
        DbSet<HrPsAllowanceVw> HrPsAllowanceVws { get; }
        DbSet<HrPsDeductionVw> HrPsDeductionVws { get; }
        DbSet<HrJobCategory> HrJobCategorys { get; }
        DbSet<HrJobCategoriesVw> HrJobCategoriesVw { get; }
        DbSet<HrJobGroupsVw> HrJobGroupsVw { get; }
        DbSet<HrJobGroups> HrJobGroups { get; }
        DbSet<HrIncrementsAllowanceVw> HrIncrementsAllowanceVws { get; }
        DbSet<HrIncrementsDeductionVw> HrIncrementsDeductionVws { get; }
        DbSet<HrSector> HrSectors { get; }
        DbSet<HrJobAllowanceDeduction> HrJobAllowanceDeductions { get; }
        DbSet<HrJobAllowanceDeductionVw> HrJobAllowanceDeductionVw { get; }
        DbSet<HrJobLevelsAllowanceDeduction> HrJobLevelsAllowanceDeductions { get; }
        DbSet<HrJobLevelsAllowanceDeductionVw> HrJobLevelsAllowanceDeductionVw { get; }
        DbSet<HrJobLevelsVw> HrJobLevelsVw { get; }
        DbSet<HrPayrollDVw> HrPayrollDVw { get; }
        DbSet<HrLeaveAllowanceDeduction> HrLeaveAllowanceDeduction { get; }
        DbSet<HrLeaveAllowanceVw> HrLeaveAllowanceVw { get; }
        DbSet<HrProvisionsMedicalInsurance> HrProvisionsMedicalInsurances { get; }
        DbSet<HrProvisionsMedicalInsuranceVw> HrProvisionsMedicalInsuranceVw { get; }
        DbSet<HrProvisionsMedicalInsuranceEmployee> HrProvisionsMedicalInsuranceEmployees { get; }
        DbSet<HrProvisionsMedicalInsuranceEmployeeVw> HrProvisionsMedicalInsuranceEmployeeVw { get; }
        DbSet<HrProvisionsMedicalInsuranceEmployeeAccVw> HrProvisionsMedicalInsuranceEmployeeAccVw { get; }
        DbSet<HrContractsAllowanceDeduction> HrContractsAllowanceDeduction { get; }
        DbSet<HrClearanceAllowanceDeduction> HrClearanceAllowanceDeductions { get; }
        DbSet<HrClearanceAllowanceVw> HrClearanceAllowanceVws { get; }
        DbSet<HrContractsDeductionVw> HrContractsDeductionVw { get; }
        DbSet<HrContractsAllowanceVw> HrContractsAllowanceVw { get; }
        #endregion

        #region ============ ACC ==============================================================================

        DbSet<AccAccount> AccAccounts { get; }
        DbSet<AccFinancialYear> AccFinancialYears { get; }
        DbSet<AccFacility> AccFacilities { get; }
        DbSet<AccFacilitiesVw> AccFacilitiesVws { get; }
        DbSet<AccGroup> AccGroup { get; }
        DbSet<AccPeriods> AccPeriods { get; }
        DbSet<AccCostCenterVws> AccCostCenterVws { get; }
        DbSet<AccJournalMaster> AccJournalMasters { get; }
        DbSet<AccJournalMasterVw> AccJournalMasterVws { get; }
        DbSet<AccCostCenter> AccCostCenter { get; }

        DbSet<AccReferenceType> AccReferenceTypes { get; }
        DbSet<AccAccountsVw> AccAccountsVw { get; }

        DbSet<AccBranchAccount> AccBranchAccounts { get; }
        DbSet<AccBranchAccountsVw> AccBranchAccountsVws { get; }
        DbSet<AccBranchAccountType> AccBranchAccountTypes { get; }

        DbSet<AccPeriodDateVws> AccPeriodDateVws { get; }
        DbSet<AccJournalDetaile> AccJournalDetailes { get; }
        DbSet<AccJournalDetailesVw> AccJournalDetailesVws { get; }
        DbSet<AccAccountsCloseType> AccAccountsCloseTypes { get; }
        DbSet<AccBank> AccBanks { get; }
        DbSet<AccCashOnHand> AccCashOnHands { get; }
        DbSet<AccRequest> AccRequests { get; }
        DbSet<AccRequestVw> AccRequestVws { get; }
        DbSet<AccDocumentTypeListVw> AccDocumentTypeListVws { get; }
        DbSet<AccCashOnHandListVw> AccCashonhandListVWs { get; }
        DbSet<AccAccountsSubHelpeVw> AccAccountsSubHelpeVws { get; }
        DbSet<AccCostCenteHelpVw> AccCostCenteHelpVws { get; }
        DbSet<AccJournalSignatureVw> AccJournalSignatureVws { get; }
        DbSet<AccJournalMasterExportVw> AccJournalMasterExportVws { get; }
        DbSet<AccBankVw> AccBankVws { get; }
        DbSet<AccAccountsLevel> AccAccountsLevels { get; }
        DbSet<AccPettyCashExpensesType> AccPettyCashExpensesTypes { get; }
        DbSet<AccPettyCashExpensesTypeVw> AccPettyCashExpensesTypeVws { get; }
        DbSet<AccBankChequeBook> AccBankChequeBooks { get; }
        DbSet<AccCashOnHandVw> AccCashOnHandVws { get; }
        DbSet<AccAccountsGroupsFinalVw> AccAccountsGroupsFinalVws { get; }
        DbSet<AccAccountsCostcenterVw> AccAccountsCostcenterVws { get; }
        DbSet<AccReferenceTypeVw> AccReferenceTypeVws { get; }
        DbSet<AccJournalMasterFile> AccJournalMasterFiles { get; }
        DbSet<AccJournalMasterFilesVw> AccJournalMasterFilesVws { get; }
        DbSet<AccAccountsCostcenter> AccAccountsCostcenters { get; }
        DbSet<AccRequestBalanceStatusVw> AccRequestBalanceStatusVws { get; }
        DbSet<AccRequestHasCreditVw> AccRequestHasCreditVw { get; }
        DbSet<AccRequestExchangeStatusVw> AccRequestExchangeStatusVw { get; }

        DbSet<AccBalanceSheet> AccBalanceSheets { get; }
        DbSet<AccPaymentType> AccPaymentType { get; }
        DbSet<AccJournalComment> AccJournalComment { get; }
        DbSet<AccSettlementSchedule> AccSettlementSchedule { get; }
        DbSet<AccPettyCash> AccPettyCash { get; }
        DbSet<AccPettyCashVw> AccPettyCashVw { get; }

        DbSet<AccSettlementScheduleD> AccSettlementScheduleD { get; }

        DbSet<AccSettlementInstallment> AccSettlementInstallment { get; }
        DbSet<AccSettlementScheduleDVw> AccSettlementScheduleDVw { get; }
        DbSet<AccPettyCashD> AccPettyCashD { get; }
        DbSet<AccPettyCashDVw> AccPettyCashDVw { get; }
        DbSet<AccPettyCashTempVw> AccPettyCashTempVw { get; }
        DbSet<AccSettlementInstallmentsVw> AccSettlementInstallmentsVw { get; }
        DbSet<AccJournalDetailesCostcenter> AccJournalDetailesCostcenter { get; }

        DbSet<AccBalanceSheetCostCenterVw> AccBalanceSheetCostCenterVw { get; }
        DbSet<AccAccountsRefrancesVw> AccAccountsRefrancesVw { get; }

        DbSet<AccAccountsReportsVw> AccAccountsReportsVW { get; }
        DbSet<AccBalanceSheetPostOrNot> AccBalanceSheetPostOrNot { get; }
        DbSet<AccReceivablesPayablesTransactionD> AccReceivablesPayablesTransactionDs { get; }
        DbSet<AccReceivablesPayablesTransactionDVw> AccReceivablesPayablesTransactionDVws { get; }

        DbSet<AccCertificateSetting> AccCertificateSettings { get; }
        DbSet<AccCertificateSettingsSimulation> AccCertificateSettingsSimulations { get; }
        DbSet<AccCostCentersLevel> AccCostCentersLevel { get; }
        DbSet<AccRequestEmployeeVw> AccRequestEmployeeVw { get; }

        #endregion --------- End ACC --------------------------------------------------------------------------

        #region ======= PM ========================



        DbSet<PMProjects> PMProjects { get; }
        DbSet<PmProjectsVw> PmProjectsVws { get; }
        DbSet<PmProjectsType> PmProjectsTypes { get; }

        //==propsPattern==

        DbSet<PmProjectsTypeVw> PmProjectsTypeVws { get; }
        DbSet<PmExtractAdditionalType> PmExtractAdditionalTypes { get; }
        DbSet<PmExtractAdditionalTypeVw> PmExtractAdditionalTypeVws { get; }
        DbSet<PmProjectsStage> PmProjectsStages { get; }
        DbSet<PmProjectsStagesVw> PmProjectsStagesVws { get; }
        DbSet<PMProjectsStaff> PmProjectsStaffs { get; }
        DbSet<PmProjectsStaffVw> PmProjectsStaffVws { get; }
        DbSet<PmProjectsStatusVw> PmProjectsStatusVws { get; }
        DbSet<PmProjectsStatus> PmProjectsStatuses { get; }

        DbSet<PmProjectStatusVw> PmProjectStatusVws { get; }
        DbSet<PmDurationTypeVw> PmDurationTypeVws { get; }
        DbSet<PmSetting> PmSettings { get; }


        //from here was comment
        //DbSet<PmProjectsVw> PmProjectsVws { get; }

        DbSet<PmProjectPlan> PmProjectPlans { get; }
        DbSet<PmProjectPlansVw> PmProjectPlansVws { get; }
        DbSet<PmProjectsAddDeduc> PmProjectsAddDeducs { get; }
        DbSet<PmProjectsAddDeducVw> PmProjectsAddDeducVws { get; }
        DbSet<PmProjectsFile> PmProjectsFiles { get; }
        DbSet<PmProjectsFilesVw> PmProjectsFilesVws { get; }
        DbSet<PmProjectsInstallment> PmProjectsInstallments { get; }
        DbSet<PmProjectsInstallmentVw> PmProjectsInstallmentVws { get; }
        DbSet<PmProjectsInstallmentAction> PmProjectsInstallmentActions { get; }
        DbSet<PmProjectsInstallmentActionVw> PmProjectsInstallmentActionVws { get; }
        DbSet<PmProjectsInstallmentPayment> PmProjectsInstallmentPayments { get; }
        DbSet<PmProjectsInstallmentPaymentVw> PmProjectsInstallmentPaymentVws { get; }
        DbSet<PmProjectsItem> PmProjectsItems { get; }
        DbSet<PMProjectsItemsVw> PmProjectsItemsVws { get; }
        DbSet<PmProjectsRisk> PmProjectsRisks { get; }
        DbSet<PmProjectsRisksVw> PmProjectsRiskVws { get; }
        DbSet<PmProjectsRisksVw2> PmProjectsRisksVw2s { get; }

        DbSet<PmProjectsStaffType> PmProjectsStaffTypes { get; }

        DbSet<PmProjectsStokeholder> PmProjectsStokeholders { get; }
        DbSet<PmProjectsStokeholderVw> PmProjectsStokeholderVws { get; }
        DbSet<PmExtractTransactionsAdditional> PmExtractTransactionsAdditionals { get; }
        DbSet<PmExtractTransactionsPayment> PmExtractTransactionsPayments { get; }
        DbSet<PmExtractTransactionsPaymentVw> PmExtractTransactionsPaymentVws { get; }


        //DbSet<PmProjectStatusVw> PmProjectStatusVws { get; }


        DbSet<PmExtractTransactionsChangeStatus> PmExtractTransactionsChangeStatus { get; }
        DbSet<PmExtractTransactionsChangeStatusVw> PmExtractTransactionsChangeStatusVws { get; }

        DbSet<PmExtractTransaction> PmExtractTransactions { get; }
        DbSet<PmExtractTransactionsVw> PmExtractTransactionsVws { get; }

        DbSet<PmExtractTransactionsProduct> PmExtractTransactionsProducts { get; }
        DbSet<PmExtractTransactionsProductsVw> PmExtractTransactionsProductsVws { get; }
        DbSet<PmExtractTransactionsType> PmExtractTransactionsTypes { get; }
        DbSet<PmProjectsEditVw> PmProjectsEditVws { get; }
        DbSet<PmExtractTransactionsStatus> PmExtractTransactionsStatuses { get; }
        DbSet<PmRiskImpact> PmRiskImpacts { get; }
        DbSet<PmRiskEffect> PmRiskEffects { get; }
        DbSet<PmProjectsBudgetItem> PmProjectsBudgetItems { get; }
        DbSet<PmProjectsBudgetItemsVw> PmProjectsBudgetItemsVws { get; }
        DbSet<PmProjectsObjective> PmProjectsObjectives { get; }
        DbSet<PmProjectsStrategicLink> PmProjectsStrategicLinks { get; }
        DbSet<PmProjectsStrategicLinkVw> PmProjectsStrategicLinkVws { get; }
        DbSet<PmProjectsInterconnection> PmProjectsInterconnections { get; }
        DbSet<PmProjectsInterconnectionVw> PmProjectsInterconnectionVws { get; }
        DbSet<PmProjectsPerformanceIndicator> PmProjectsPerformanceIndicators { get; }
        DbSet<PmDeliverableTransactionsDetailsVw> PmDeliverableTransactionsDetailsVws { get; }
        DbSet<PmDeliverableTransactionsDetail> PmDeliverableTransactionsDetails { get; }
        DbSet<PmProjectsDeliverable> PmProjectsDeliverables { get; }
        DbSet<PmProjectsDeliverableVw> PmProjectsDeliverableVws { get; }
        DbSet<PmProjectsAssumption> PmProjectsAssumptions { get; }
        DbSet<PmProjectsGovernance> PmProjectsGovernances { get; }
        DbSet<PmProjectsResource> PmProjectsResources { get; }
        DbSet<PmProjectsResourcesVw> PmProjectsResourcesVws { get; }
        DbSet<PmProjectsDeliverableAcceptCriteriaVw> PmProjectsDeliverableAcceptCriteriaVws { get; }
        DbSet<PmProjectsDeliverableAcceptCriterion> PmProjectsDeliverableAcceptCriterions { get; }
        DbSet<PmProjectStepsVw> PmProjectStepsVws { get; }
        DbSet<PmChangeRequest> PmChangeRequests { get; }
        DbSet<PmChangeRequestVw> PmChangeRequestVws { get; }
        DbSet<PmChangeRequestItem> PmChangeRequestItems { get; }
        DbSet<PmChangeRequestItemsVw> PmChangeRequestItemsVws { get; }

        DbSet<PmDeliverableTransaction> PmDeliverableTransactions { get; }
        DbSet<PmDeliverableTransactionsVw> PmDeliverableTransactionsVw { get; }
        DbSet<PmProjectsLessonsLearnedDetail> PmProjectsLessonsLearnedDetails { get; }
        DbSet<PmProjectsLessonsLearnedDetailsVw> PmProjectsLessonsLearnedDetailsVws { get; }
        DbSet<PmProjectsLessonsLearnedMaster> PmProjectsLessonsLearnedMasters { get; }
        DbSet<PmProjectsLessonsLearnedMasterVw> PmProjectsLessonsLearnedMasterVws { get; }
        DbSet<PmProjectsClosingVw> PmProjectsClosingVws { get; }
        DbSet<PmProjectsClosing> PmProjectsClosings { get; }
        DbSet<PmDeliverablesTrackingStatus> PmDeliverablesTrackingStatuses { get; }
        DbSet<PmDeliverablesTrackingStatusVw> PmDeliverablesTrackingStatusVws { get; }
        DbSet<PmKickOff> PmKickOffs { get; }
        DbSet<PmKickOffVw> PmKickOffVws { get; }
        DbSet<PmProjectsStatementRequest> PmProjectsStatementRequests { get; }
        DbSet<PmProjectsStatementRequestVw> PmProjectsStatementRequestVws { get; }
        DbSet<PmExtractTransactionsDiscount> PmExtractTransactionsDiscounts { get; }
        DbSet<PmExtractTransactionsDiscountVw> PmExtractTransactionsDiscountVws { get; }
        DbSet<PmProjectsGuarantee> PmProjectsGuarantees { get; }
        DbSet<PmProjectsGuaranteeVw> PmProjectsGuaranteeVws { get; }
        DbSet<PmProjectsListVw> PmProjectsListVws { get; }

        DbSet<PmTransactionsInstallment> PmTransactionsInstallments { get; }
        DbSet<PmTransactionsInstallmentsVw> PmTransactionsInstallmentsVws { get; }

        DbSet<PmTransaction> PmTransactions { get; }
        DbSet<PmTransactionsVw> PmTransactionsVws { get; }



        #endregion ======= PM ========================

        #region ============ RPT ==================
        DbSet<RptReport> RptReports { get; }
        DbSet<RptCustomReport> RptCustomReports { get; }

        DbSet<RptField> RptFields { get; }
        DbSet<RptFieldsVw> RptFieldsVws { get; }
        DbSet<RptTable> RptTables { get; }
        DbSet<RptReportsField> RptReportsField { get; }
        DbSet<RptReportsFieldsVw> RptReportsFieldsVw { get; }
        DbSet<RptOperator> RptOperator { get; }
        DbSet<DynamicQueryResult> DynamicQueryResults { get; }
        DbSet<RptReportsVw> RptReportsVw { get; }
        DbSet<RptReportsOrderByVw> RptReportsOrderByVw { get; }
        DbSet<RptPowerBiconfig> RptPowerBiconfig { get; }

        #endregion --------- End RPT ----------------

        #region ==========SAL==========================================
        DbSet<SalTransaction> SalTransactions { get; }
        DbSet<SalTransactionsVw> SalTransactionsVws { get; }

        DbSet<SalItemsPriceM> SalItemsPriceMs { get; }
        DbSet<SalItemsPriceMVw> SalItemsPriceMVws { get; }

        DbSet<SalPosSetting> SalPosSettings { get; }
        DbSet<SalPosSettingVw> SalPosSettingVws { get; }

        DbSet<SalPosUser> SalPosUsers { get; }
        DbSet<SalPosUsersVw> SalPosUsersVws { get; }
        DbSet<SalPaymentTerm> SalPaymentTerms { get; }
        DbSet<SalSetting> SalSetting { get; }
        DbSet<SalTransactionsType> SalTransactionsType { get; }
        DbSet<SalTransactionsCommission> SalTransactionsCommissions { get; }
        DbSet<SalTransactionsCommissionVw> SalTransactionsCommissionVws { get; }

        DbSet<SalTransactionsProduct> SalTransactionsProducts { get; }
        DbSet<SalTransactionsProductsVw> SalTransactionsProductsVws { get; }

        DbSet<SalTransactionsDiscount> SalTransactionsDiscounts { get; }
        DbSet<SalTransactionsDiscountVw> SalTransactionsDiscountVws { get; }

        #endregion ==========End SAL===================================

        #region =======OPM=====================================
        DbSet<OpmContractLocation> OpmContractLocations { get; }
        DbSet<OpmContract> OpmContracts { get; }
        DbSet<OpmContractVw> OpmContractVws { get; }
        DbSet<OpmTransactionsItem> OpmTransactionsItems { get; }
        DbSet<OpmTransactionsLocation> OpmTransactionsLocations { get; }
        DbSet<OpmPolicy> OpmPolicies { get; }
        DbSet<OpmContractItem> OpmContractItems { get; }
        DbSet<OPMTransactionsDetailsVw> oPMTransactionsDetailsVws { get; }

        DbSet<OpmContarctEmp> OpmContarctEmps { get; }
        DbSet<OpmContarctAssign> OpmContarctAssigns { get; }
        DbSet<OpmContractVw> OpmContractVw { get; }

        DbSet<OpmContractReplaceEmp> OpmContractReplaceEmps { get; }
        DbSet<OpmContarctEmpVw> OpmContarctEmpVws { get; }
        DbSet<OpmContractItemsVw> OpmContractItemsVws { get; }
        DbSet<OPMPayrollD> OPMPayrollDs { get; }
        DbSet<OpmPURTransactionsDetails> OpmPURTransactionsDetails { get; }


        DbSet<OpmContractReplaceEmpVw> OpmContractReplaceEmpVws { get; }
        DbSet<OPMPayrollDVW> OPMPayrollDVWs { get; }
        DbSet<OPMContractLocationVW> oPMContractLocationVWs { get; }
        DbSet<OpmServicesTypesVW> opmServicesTypesVWs { get; }
        #endregion

        #region =======PUR=====================================
        //DbSet<PurTransactionsType> purTransactionsTypes { get; }
        DbSet<PurTransactionsPayment> purTransactionsPayments { get; }
        /// <summary>
        /// ///////////////
        /// </summary>
        DbSet<PurExpense> PurExpenses { get; }
        DbSet<PurExpensesVw> PurExpensesVws { get; }
        DbSet<PurDiscountCatalog> PurDiscountCatalogs { get; }
        DbSet<PurDiscountCatalogVw> PurDiscountCatalogVws { get; }
        DbSet<PurDiscountCatalogAllVw> PurDiscountCatalogAllVws { get; }
        DbSet<PurDiscountByAmount> PurDiscountByAmounts { get; }
        DbSet<PurDiscountByQty> PurDiscountByQties { get; }
        DbSet<PurDiscountProduct> PurDiscountProducts { get; }
        DbSet<PurDiscountProductsVw> PurDiscountProductsVws { get; }
        DbSet<PurDiscountType> PurDiscountTypes { get; }
        DbSet<PurAdditionalType> PurAdditionalTypes { get; }
        DbSet<PurAdditionalTypeVw> PurAdditionalTypeVws { get; }
        DbSet<PurItemsPriceM> PurItemsPriceMs { get; }
        DbSet<PurItemsPriceMVw> PurItemsPriceMVws { get; }
        DbSet<PurItemsPriceD> PurItemsPriceDs { get; }
        DbSet<PurItemsPriceDVw> PurItemsPriceDVws { get; }
        DbSet<PurTransaction> PurTransactions { get; }
        DbSet<PurTransactionsVw> PurTransactionsVws { get; }
        DbSet<PurTransactionsSupplier> PurTransactionsSuppliers { get; }
        DbSet<PurTransactionsSupplierVw> PurTransactionsSupplierVws { get; }
        DbSet<PurTransactionsProduct> PurTransactionsProducts { get; }
        DbSet<PurTransactionsProductsVw> PurTransactionsProductsVws { get; }
        DbSet<PurTransactionsType> PurTransactionsTypes { get; }
        DbSet<PurTransactionsDiscount> PurTransactionsDiscounts { get; }
        DbSet<PurTransactionsDiscountVw> PurTransactionsDiscountVws { get; }
        DbSet<PurRqfWorkFlowEvaluation> PurRqfWorkFlowEvaluations { get; }
        DbSet<PurRqfWorkFlowEvaluationVw> PurRqfWorkFlowEvaluationVws { get; }
        DbSet<PurPaymentTerm> PurPaymentTerms { get; }

        #endregion

        #region ============================== WF ========================================
        DbSet<WfAppTypeTable> WfAppTypeTables { get; }
        DbSet<WfDynamicAttributeDataType> WfDynamicAttributeDataTypes { get; }
        DbSet<WfDynamicValue> WfDynamicValues { get; }
        DbSet<WfDynamicTableValue> WfDynamicTableValues { get; }
        DbSet<WfLookUpCatagory> WfLookUpCatagories { get; }
        DbSet<WfLookupType> WfLookupTypes { get; }
        DbSet<WfStepLevel> WfStepLevels { get; }
        DbSet<WfStepsNotification> WfStepsNotifications { get; }
        DbSet<WfStepsType> WfStepsTypes { get; }


        DbSet<WfAppGroup> WfAppGroups { get; }
        DbSet<WfAppGroupsVw> WfAppGroupsVws { get; }

        DbSet<WfAppType> WfAppTypes { get; }
        DbSet<WfAppTypeVw> WfAppTypeVws { get; }

        DbSet<WfApplication> WfApplications { get; }
        DbSet<WfApplicationsVw> WfApplicationsVws { get; }

        DbSet<WfApplicationsAssigne> WfApplicationsAssignes { get; }
        DbSet<WfApplicationsAssignesVw> WfApplicationsAssignesVws { get; }

        DbSet<WfApplicationsAssignesReply> WfApplicationsAssignesReplies { get; }
        DbSet<WfApplicationsAssignesReplyVw> WfApplicationsAssignesReplyVws { get; }

        DbSet<WfApplicationsComment> WfApplicationsComments { get; }
        DbSet<WfApplicationsCommentsVw> WfApplicationsCommentsVws { get; }

        DbSet<WfApplicationsStatus> WfApplicationsStatus { get; }
        DbSet<WfApplicationsStatusVw> WfApplicationsStatusVws { get; }

        DbSet<WfDynamicAttribute> WfDynamicAttributes { get; }
        DbSet<WfDynamicAttributesVw> WfDynamicAttributesVws { get; }

        DbSet<WfDynamicAttributesTable> WfDynamicAttributesTables { get; }
        DbSet<WfDynamicAttributesTableVw> WfDynamicAttributesTableVws { get; }

        DbSet<WfEscalation> WfEscalations { get; }
        DbSet<WfEscalationVw> WfEscalationVws { get; }

        DbSet<WfLookupData> WfLookupData { get; }
        DbSet<WfLookupDataVw> WfLookupDataVws { get; }

        DbSet<WfStatus> WfStatus { get; }
        DbSet<WfStatusVw> WfStatusVws { get; }

        DbSet<WfStep> WfSteps { get; }
        DbSet<WfStepsVw> WfStepsVws { get; }

        DbSet<WfStepsTransaction> WfStepsTransactions { get; }
        DbSet<WfStepsTransactionsVw> WfStepsTransactionsVws { get; }

        DbSet<WfLayoutAttribute> WfLayoutAttributes { get; }

        DbSet<WfAppCommittee> WfAppCommittees { get; }
        DbSet<WfAppCommitteesVw> WfAppCommitteesVws { get; }

        DbSet<WfActionsCommittee> WfActionsCommittees { get; }

        DbSet<WfAppCommitteesMember> WfAppCommitteesMembers { get; }
        DbSet<WfAppCommitteesMembersVw> WfAppCommitteesMembersVws { get; }

        DbSet<WfAppMember> WfAppMembers { get; }
        DbSet<WfAppMembersVw> WfAppMembersVws { get; }

        DbSet<WfStepsCommittee> WfStepsCommittees { get; }
        DbSet<WfStepsCommitteesVw> WfStepsCommitteesVws { get; }
        DbSet<WFFormsControls> WFFormsControls { get; }

        #endregion

        #region ======= WH ========================

        DbSet<WhUnit> WhUnits { get; }
        DbSet<WhItemsUnitListVw> WhItemsUnitListVw { get; }
        DbSet<WhItemsCatagory> whItemsCatagories { get; }
        DbSet<WhAccountType> WhAccountTypes { get; }
        DbSet<WhItem> WhItems { get; }
        DbSet<WhItemsVw> WhItemsVws { get; }
        DbSet<WhInventory> WhInventorys { get; }
        DbSet<WhInventoriesVw> WhInventoriesVws { get; }
        DbSet<WhTransactionsMaster> WhTransactionsMasters { get; }
        DbSet<WhTransactionsMasterVw> WhTransactionsMasterVws { get; }
        DbSet<WhTransactionsDetaile> WhTransactionsDetailes { get; }
        DbSet<WhTransactionsDetailesVw> WhTransactionsDetailesVws { get; }
        DbSet<WhItemsComponent> WhItemsComponents { get; }
        DbSet<WhItemsComponentsVw> WhItemsComponentsVws { get; }
        DbSet<WhItemsSerial> WhItemsSerials { get; }
        DbSet<WhItemsSerialsVw> WhItemsSerialsVws { get; }
        DbSet<WhActualInventorySeriale> WhActualInventorySeriales { get; }
        DbSet<WhActualInventorySerialesVw> WhActualInventorySerialesVws { get; }
        DbSet<WhItemsCatagoriesVw> WhItemsCatagoriesVw { get; }
        DbSet<WhItemsSection> WhItemsSections { get; }
        DbSet<WhItemsSectionsVw> WhItemsSectionsVw { get; }
        DbSet<WhInventorySection> WhInventorySections { get; }
        DbSet<WhItemTemplate> WhItemTemplates { get; }
        DbSet<WhItemTemplateVw> WhItemTemplateVw { get; }
        DbSet<WhItemsBatch> WhItemsBatchs { get; }
        DbSet<WhItemsBatchListVw> WhItemsBatchListVw { get; }
        DbSet<WhTemplate> WhTemplates { get; }
        DbSet<WhTransactionsType> WhTransactionsTypes { get; }
        DbSet<WhTransactionsTypeVw> WhTransactionsTypeVw { get; }
        #endregion ======= WH ========================

        #region =============== FXA ===========================
        DbSet<FxaAdditionsExclusion> FxaAdditionsExclusions { get; }
        DbSet<FxaAdditionsExclusionVw> FxaAdditionsExclusionVws { get; }

        DbSet<FxaAdditionsExclusionType> FxaAdditionsExclusionTypes { get; }
        DbSet<FxaDepreciationMethod> FxaDepreciationMethods { get; }

        DbSet<FxaFixedAsset> FxaFixedAssets { get; }
        DbSet<FxaFixedAssetVw> FxaFixedAssetVws { get; }
        DbSet<FxaFixedAssetVw2> FxaFixedAssetVw2s { get; }

        DbSet<FxaFixedAssetTransfer> FxaFixedAssetTransfers { get; }
        DbSet<FxaFixedAssetTransferVw> FxaFixedAssetTransferVws { get; }

        DbSet<FxaFixedAssetType> FxaFixedAssetTypes { get; }
        DbSet<FxaFixedAssetTypeVw> FxaFixedAssetTypeVws { get; }

        DbSet<FxaTransaction> FxaTransactions { get; }
        DbSet<FxaTransactionsVw> FxaTransactionsVws { get; }

        DbSet<FxaTransactionsAsset> FxaTransactionsAssets { get; }
        DbSet<FxaTransactionsAssetsVw> FxaTransactionsAssetsVws { get; }

        DbSet<FxaTransactionsPayment> FxaTransactionsPayments { get; }

        DbSet<FxaTransactionsType> FxaTransactionsTypes { get; }

        DbSet<FxaTransactionsProduct> FxaTransactionsProducts { get; }
        DbSet<FxaTransactionsProductsVw> FxaTransactionsProductsVws { get; }

        DbSet<FxaTransactionsRevaluation> FxaTransactionsRevaluations { get; }
        DbSet<FxaTransactionsRevaluationVw> FxaTransactionsRevaluationVws { get; }
        #endregion

        #region =============== CRM ===========================

        DbSet<CrmEmailTemplateAttach> CrmEmailTemplateAttachs { get; }
        DbSet<CrmEmailTemplate> CrmEmailTemplates { get; }

        #endregion

        #region =============HD==========================
        DbSet<HdTickect> HdTickects { get; }
        #endregion


        #region =============TS==========================

        DbSet<TsTask> TsTasks { get; }
        DbSet<TsTasksVw> TsTasksVws { get; }
        DbSet<TsTaskStatusVw> TsTaskStatusVws { get; }
        DbSet<TsTasksResponse> TsTasksResponses { get; }
        DbSet<TsTasksResponseVw> TsTasksResponseVws { get; }
        DbSet<TsAppointment> TsAppointments { get; }
        DbSet<TsAppointmentVw> TsAppointmentVws { get; }
        DbSet<TsTasksScheduler> TsTasksSchedulers { get; }
        DbSet<TsTasksSchedulerVw> TsTasksSchedulerVws { get; }

        #endregion

        #region ========= Hot ==========
        DbSet<HotFloor> HotFloors { get; }
        DbSet<HotFloorsVw> HotFloorsVws { get; }
        DbSet<HotGroup> HotGroups { get; }
        DbSet<HotGroupsVw> HotGroupsVws { get; }
        DbSet<HotRoom> HotRooms { get; }
        DbSet<HotRoomAsset> HotRoomAssets { get; }
        DbSet<HotRoomAssetsVw> HotRoomAssetsVws { get; }
        DbSet<HotRoomService> HotRoomServices { get; }
        DbSet<HotRoomVw> HotRoomVws { get; }
        DbSet<HotServicesVw> HotServicesVws { get; }
        DbSet<HotTransaction> HotTransactions { get; }
        DbSet<HotTransactionsCompanion> HotTransactionsCompanions { get; }
        DbSet<HotTransactionsCompanionVw> HotTransactionsCompanionVws { get; }
        DbSet<HotTransactionsPayment> HotTransactionsPayments { get; }
        DbSet<HotTransactionsRoom> HotTransactionsRooms { get; }
        DbSet<HotTransactionsRoomVw> HotTransactionsRoomVws { get; }
        DbSet<HotTransactionsService> HotTransactionsServices { get; }
        DbSet<HotTransactionsServicesVw> HotTransactionsServicesVws { get; }
        DbSet<HotTransactionsStatus> HotTransactionsStatuses { get; }
        DbSet<HotTransactionsType> HotTransactionsTypes { get; }

        DbSet<HotTransactionsStatus> HotTransactionsStatuss { get; }
        DbSet<HotTransactionsVw> HotTransactionsVws { get; }

        DbSet<HotTypeRoom> HotTypeRooms { get; }

        DbSet<HotService> HotServices { get; }
        #endregion

        #region ========= Integra ==========
        DbSet<IntegraSystem> IntegraSystems { get; }
        DbSet<IntegraProperty> IntegraPropertys { get; }
        DbSet<IntegraPropertiesVw> IntegraPropertiesVws { get; }
        DbSet<IntegraPropertyValue> IntegraPropertyValues { get; }
        DbSet<IntegraPropertyValuesVw> IntegraPropertyValuesVws { get; }
        DbSet<IntegraTable> IntegraTables { get; }
        DbSet<IntegraField> IntegraFields { get; }
        DbSet<IntegraTableValue> IntegraTableValues { get; }
        #endregion

        DbSet<BudgTransaction> BudgTransactions { get; }
        DbSet<BudgTransactionVw> BudgTransactionVws { get; }
        DbSet<BudgTransactionDetaile> BudgTransactionDetaile { get; }
        DbSet<BudgTransactionDetailesVw> BudgTransactionDetailesVws { get; }
        DbSet<BudgExpensesLinks> BudgExpensesLinks { get; }
        DbSet<BudgExpensesLinksVW> budgExpensesLinksVWs { get; }
        DbSet<OPMPayroll> oPMPayrolls { get; }
        DbSet<OpmPayrollVw> opmPayrollVws { get; }
        DbSet<OpmServicesTypes> OpmServicesTypes { get; }
        DbSet<OPMTransactionsDetails> OPMTransactionsDetails { get; }
        DbSet<BudgDocType> BudgDocType { get; }

        #region =============== RE ===========================
        DbSet<ReTransactionsInstallment> ReTransactionsInstallments { get; }
        DbSet<ReTransactionsInstallmentsVw> ReTransactionsInstallmentsVws { get; }
        DbSet<ReTransaction> ReTransactions { get; }
        DbSet<ReTransactionsVw> ReTransactionsVws { get; }
        #endregion

        #region =============== Sch ===========================
        DbSet<SchTransactionsInstallment> SchTransactionsInstallments { get; }
        DbSet<SchTransactionsInstallmentsVw> SchTransactionsInstallmentsVws { get; }

        DbSet<SchTransactionsTransportationVw> SchTransactionsTransportationVws { get; }
        DbSet<SchTransactionsTransportationInstallmentsVw> SchTransactionsTransportationInstallmentsVws { get; }
        DbSet<SchTransactionsTransportationPrintVw> SchTransactionsTransportationPrintVws { get; }
        #endregion

        #region =============== Maintenance ===========================
        DbSet<MaintTransactionsInstallment> MaintTransactionsInstallments { get; }
        DbSet<MaintTransactionsInstallmentsVw> MaintTransactionsInstallmentsVws { get; }
        #endregion

        #region =============== Trans ===========================
        DbSet<TransTransaction> TransTransactions { get; }
        DbSet<TransTransactionsVw> TransTransactionsVws { get; }
        #endregion

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}