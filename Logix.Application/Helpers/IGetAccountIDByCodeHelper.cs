using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Domain.ACC;
using Logix.Domain.HR;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Helpers.Acc
{
    public interface IGetAccountIDByCodeHelper
    {
        Task<long> GetAccountIDByCode(long AccountType, string Code = "", long FacilityId = 0);
        Task<(string, string)> GetAccountNameByID(long AccountType, long Code = 0);
    }

    public class GetAccountIDByCodeHelper : IGetAccountIDByCodeHelper
    {
        private readonly ICurrentData _session;
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IHrRepositoryManager hrRepositoryManager;
        private readonly IFxaRepositoryManager fxaRepositoryManager;

        public GetAccountIDByCodeHelper(ICurrentData session,
            IAccRepositoryManager accRepositoryManager,
            IMainRepositoryManager mainRepositoryManager,
            IHrRepositoryManager hrRepositoryManager,
            IFxaRepositoryManager fxaRepositoryManager)
        {
            _session = session;
            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this.hrRepositoryManager = hrRepositoryManager;
            this.fxaRepositoryManager = fxaRepositoryManager;
        }

        public async Task<long> GetAccountIDByCode(long AccountType, string Code, long FacilityId = 0)
        {
            try
            {
                long facilityId = FacilityId == 0 ? _session.FacilityId : FacilityId;
                long accAccountId = 0;
                var parentId = await accRepositoryManager.AccReferenceTypeRepository.GetOne(s => s.ParentId, x => x.ReferenceTypeId == AccountType);
                switch (parentId)
                {
                    case 1: // حساب
                        {
                            accAccountId = await accRepositoryManager.AccAccountsSubHelpeVwRepository.GetOne(s => s.AccAccountId,
                                x => x.IsSub == false && x.IsActive == true && x.FacilityId == facilityId && x.AccAccountCode == Code);
                            break;
                        }
                    case 2: // 'عميل
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(2, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(2, Code, facilityId, AccountType);
                            break;
                        }
                    case 3: // 'مورد
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(1, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(1, Code, facilityId, AccountType);

                            break;
                        }
                    case 5: //'حساب الأصل
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfFixedAsset(Code, facilityId);
                            else if (AccountType == 12)
                                accAccountId = await GetAccountIdOfFixedAsset(Code, facilityId);
                            else if (AccountType == 13)
                                accAccountId = await GetAccountId2OfFixedAsset(Code, facilityId);
                            else if (AccountType == 14)
                                accAccountId = await GetAccountId3OfFixedAsset(Code, facilityId);

                            break;
                        }
                    case 6: //' 'بنوك
                        {
                            accAccountId = await accRepositoryManager.AccBankRepository.GetOne(s => s.AccAccountId ?? 0,
                                x => x.IsDeleted == false && x.FacilityId == facilityId && x.Code.Equals(Code));

                            break;
                        }
                    case 7: //' 'صناديق
                        {
                            accAccountId = await accRepositoryManager.AccCashOnHandRepository.GetOne(s => s.AccAccountId ?? 0,
                                x => x.IsDeleted == false && x.FacilityId == facilityId && x.Code.Equals(Code));
                            break;
                        }
                    case 8: // 'موظف
                        {
                            //AccAccountID = await accRepositoryManager.AccCashOnHandRepository.GetOne(s => s.AccAccountId ?? 0, x => x.IsDeleted == false && x.FacilityId == facilityId && x.Code == Code);
                            break;
                        }
                    case 9: // 'مستحقات الموظفين
                        {
                            accAccountId = await hrRepositoryManager.HrEmployeeRepository.GetOne(s => s.AccountId ?? 0,
                                x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId == Code);
                            break;
                        }
                    case 10: // ''سلف الموظفين
                        {
                            var empData = await hrRepositoryManager.HrEmployeeRepository.GetOneVw(x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId.Equals(Code));

                            accAccountId = empData.AccountLoanId ?? 0;
                            break;
                        }
                    case 11: // 'عهدة
                        {
                            var empData = await hrRepositoryManager.HrEmployeeRepository.GetOneVw(x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId.Equals(Code));

                            accAccountId = empData.AccountOhadId ?? 0;
                            break;
                        }
                    case 15: // 'نهاية خدمة الموظفين
                        {
                            //select Account_End_Service_ID from HR_Employee_VW where Emp_ID = @Emp_ID and IsDeleted = 0 and Facility_ID = @Facility_ID
                            var empData = await hrRepositoryManager.HrEmployeeRepository.GetOneVw(x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId.Equals(Code));

                            accAccountId = empData.AccountEndServiceId ?? 0;
                            break;
                        }
                    case 16: // 'نهاية مستحق بدل إجازة الموظفين
                        {
                            var empData = await hrRepositoryManager.HrEmployeeRepository.GetOneVw(x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId.Equals(Code));

                            accAccountId = empData.AccountVacationSalaryId ?? 0;
                            break;
                        }
                    case 17: // 'نهاية مستحق تذاكر الموظفين
                        {
                            var empData = await hrRepositoryManager.HrEmployeeRepository.GetOneVw(x => x.IsDeleted == false && x.FacilityId == facilityId && x.EmpId.Equals(Code));

                            accAccountId = empData.AccountTicketsId ?? 0;
                            break;
                        }
                    case 18: // 'الفرص
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(4, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(4, Code, facilityId, AccountType);

                            break;
                        }
                    case 19: // 'الاعضاء
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(6, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(6, Code, facilityId, AccountType);

                            break;
                        }
                    case 20: // 'المقاول
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(3, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(3, Code, facilityId, AccountType);

                            break;
                        }
                    case 50: // 'طلاب
                        {
                            if (parentId == AccountType)
                                accAccountId = await GetAccountIdOfCustomer(10, Code, facilityId);
                            else
                                accAccountId = await GetAccountIdOfCustomerGroup(10, Code, facilityId, AccountType);

                            break;
                        }
                    case 51: // 'مدرب
                        {
                            accAccountId = await GetAccountIdOfCustomer(11, Code, facilityId);
                            break;
                        }
                };
                return accAccountId;
            }
            catch (Exception exp)
            {
                Console.WriteLine($"=== Exp in get of {GetType}, Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")} .");
                throw exp;
            }
        }

        public async Task<(string, string)> GetAccountNameByID(long AccountType, long Code)
        {
            try
            {
                long facilityId = _session.FacilityId;
                string AccountName = "";
                string AccountCode = "";
                var ParentID = await accRepositoryManager.AccReferenceTypeRepository.GetOne(s => s.ParentId, x => x.ReferenceTypeId == AccountType);
                switch (ParentID)
                {
                    case 1: // حساب
                        {
                            var AccAccount = await accRepositoryManager.AccAccountsSubHelpeVwRepository.GetOne(x => x.IsSub == false && x.IsActive == true && x.FacilityId == facilityId && x.AccAccountId == Code);
                            AccountName = AccAccount.AccAccountName ?? "0";
                            AccountCode = AccAccount.AccAccountCode ?? "";

                            break;
                        }
                    case 2: // 'عميل
                        {
                            if (ParentID == AccountType)
                            {
                                var AccAccount = await mainRepositoryManager.SysCustomerRepository.GetOne(x => x.IsDeleted == false && x.FacilityId == facilityId && x.CusTypeId == 2 && x.Id == Code);
                                AccountName = AccAccount.Name ?? "";
                                AccountCode = AccAccount.Code ?? "";
                            }
                            else
                            {

                            }
                            break;
                        }
                    case 3: // 'مورد
                        {
                            if (ParentID == AccountType)
                            {
                                var AccAccount = await mainRepositoryManager.SysCustomerRepository.GetOne(x => x.IsDeleted == false && x.FacilityId == facilityId && x.CusTypeId == 1 && x.Id == Code);
                                AccountName = AccAccount.Name ?? "";
                                AccountCode = AccAccount.Code ?? "";
                            }
                            else
                            {

                            }
                            break;
                        }

                    case 5: //'حساب الأصل
                        {
                            if (ParentID == AccountType)

                            {
                                AccountName = "";
                                AccountCode = "";

                            }
                            else
                            {
                                AccountName = "";
                                AccountCode = "";
                            }
                            break;
                        }

                    case 6: //' 'بنوك
                        {
                            var AccAccount = await accRepositoryManager.AccBankRepository.GetOne(x => x.IsDeleted == false && x.FacilityId == facilityId && x.BankId == Code);
                            AccountName = AccAccount.BankName ?? "";
                            AccountCode = AccAccount.Code ?? "";
                            break;
                        }

                    case 7: //' 'صناديق
                        {
                            //AccAccountID = await accRepositoryManager.AccCashOnHandRepository.GetOne(s => s.AccAccountId ?? 0, x => x.IsDeleted == false && x.FacilityId == facilityId && x.Code == Code);
                            break;
                        }

                    case 8: // 'موظف
                        {
                            //AccAccountID = await accRepositoryManager.AccCashOnHandRepository.GetOne(s => s.AccAccountId ?? 0, x => x.IsDeleted == false && x.FacilityId == facilityId && x.Code == Code);
                            break;
                        }
                    case 9: // 'مستحقات الموظفين
                        {
                            var AccAccount = await hrRepositoryManager.HrEmployeeRepository.GetOne(x => x.IsDeleted == false && x.FacilityId == facilityId && x.Id == Code);
                            AccountName = AccAccount.EmpName ?? "";
                            AccountCode = AccAccount.EmpId;
                            break;
                        }
                    case 10: // ''سلف الموظفين
                        {
                            AccountName = "";
                            AccountCode = "";
                            break;
                        }
                    case 11: // 'عهدة
                        {
                            AccountName = "";
                            AccountCode = "";
                            break;
                        }
                    case 15: // 'نهاية خدمة الموظفين
                        {
                            AccountName = "";
                            AccountCode = "";
                            break;
                        }
                }

                return (AccountCode ?? "", AccountName ?? "");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"=== Exp in get of {GetType}, Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")} .");
                throw exp;
            }
        }



        private async Task<long> GetAccountIdOfCustomer(int CusTypeId, string Code, long FacilityId)
        {
            // this function converted from [Sys_Customer_SP] when @CMDTYPE = 12

            long accountId = 0;
            accountId = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.AccAccountId ?? 0,
                                    x => x.IsDeleted == false && x.FacilityId == FacilityId && x.CusTypeId == CusTypeId && x.Code == Code);
            return accountId;
        }

        private async Task<long> GetAccountIdOfCustomerGroup(int CusTypeId, string Code, long FacilityId, long AccountType)
        {
            // this function converted from [Sys_Customer_SP] when @CMDTYPE = 24

            long accountId = 0;
            var groupId = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.GroupId ?? 0,
                                    x => x.IsDeleted == false && x.FacilityId == FacilityId && x.CusTypeId == CusTypeId && x.Code == Code);

            accountId = await mainRepositoryManager.SysCustomerGroupAccountRepository.GetOne(a => a.AccountId ?? 0,
                a => a.Id == groupId && a.ReferenceTypeId == AccountType && a.IsDeleted == false);
            return accountId;
        }


        private async Task<long> GetAccountIdOfFixedAsset(string Code, long FacilityId)
        {
            // this function converted from [FXA_FixedAsset_SP] when @CMDTYPE = 10

            long accountId = 0;
            accountId = await fxaRepositoryManager.FxaFixedAssetRepository.GetOne(a => a.AccountId ?? 0,
                a => a.No.ToString() == Code && a.FacilityId == FacilityId && a.IsDeleted == false);
            return accountId;
        }

        private async Task<long> GetAccountId2OfFixedAsset(string Code, long FacilityId)
        {
            // this function converted from [FXA_FixedAsset_SP] when @CMDTYPE = 10

            long accountId = 0;
            accountId = await fxaRepositoryManager.FxaFixedAssetRepository.GetOne(a => a.Account2Id ?? 0,
                a => a.No.ToString() == Code && a.FacilityId == FacilityId && a.IsDeleted == false);
            return accountId;
        }

        private async Task<long> GetAccountId3OfFixedAsset(string Code, long FacilityId)
        {
            // this function converted from [FXA_FixedAsset_SP] when @CMDTYPE = 10

            long accountId = 0;
            accountId = await fxaRepositoryManager.FxaFixedAssetRepository.GetOne(a => a.Account3Id ?? 0,
                a => a.No.ToString() == Code && a.FacilityId == FacilityId && a.IsDeleted == false);
            return accountId;
        }

    }
}