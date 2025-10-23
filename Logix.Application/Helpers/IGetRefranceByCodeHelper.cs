using Logix.Application.Common;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Helpers.Acc
{
    public interface IGetRefranceByCodeHelper
    {
        Task<long> GetRefranceByCode(long AccountType, string Code = "", long FacilityId = 0);
    }

    public class GetRefranceByCodeHelper : IGetRefranceByCodeHelper
    {
        private readonly IMainServiceManager serviceManager;
        private readonly ICurrentData _session;
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IHrRepositoryManager hrRepositoryManager;

        public GetRefranceByCodeHelper(IMainServiceManager serviceManager, ICurrentData session, IAccRepositoryManager accRepositoryManager, IMainRepositoryManager mainRepositoryManager, IHrRepositoryManager hrRepositoryManager)
        {
            this.serviceManager = serviceManager;
            _session = session;
            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this.hrRepositoryManager = hrRepositoryManager;
        }

        public async Task<long> GetRefranceByCode(long AccountType, string Code, long FacilityId = 0)
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
                                x => x.IsSub == false && x.IsActive == true && x.FacilityId == _session.FacilityId && x.AccAccountCode == Code);
                            break;
                        }
                    case 2: // 'عميل
                        {
                            accAccountId = await GetCustomerId(2, Code, facilityId);
                            break;
                        }
                    case 3: // 'مورد
                        {
                            accAccountId = await GetCustomerId(1, Code, facilityId);
                            break;
                        }
                    case 4: // '' مشروع
                        {
                            accAccountId = 0;
                            break;
                        }
                    case 5: //''أصول ثابتة
                        {
                            accAccountId = 0;
                            break;
                        }

                    case 6: //'بنوك
                        {
                            accAccountId = await accRepositoryManager.AccBankRepository.GetOne(s => s.BankId,
                                x => x.IsDeleted == false && x.FacilityId == _session.FacilityId && x.Code == Code);
                            break;
                        }

                    case 7: //' 'صناديق
                        {
                            accAccountId = await accRepositoryManager.AccCashOnHandRepository.GetOne(s => s.AccAccountId ?? 0,
                                x => x.IsDeleted == false && x.FacilityId == _session.FacilityId && x.Code == int.Parse(Code));
                            break;
                        }
                    case 8: // 'مستحقات الموظفين
                        {
                            accAccountId = await hrRepositoryManager.HrEmployeeRepository.GetOne(s => s.Id,
                                x => x.IsDeleted == false && x.EmpId == Code);
                            break;
                        }
                    case 18: // 'الفرص
                        {
                            accAccountId = await GetCustomerId(4, Code, facilityId);
                            break;
                        }
                    case 19: // 'الأعضاء
                        {
                            accAccountId = await GetCustomerId(6, Code, facilityId);
                            break;
                        }
                    case 20: // 'مقاول
                        {
                            accAccountId = await GetCustomerId(3, Code, facilityId);
                            break;
                        }
                    case 50: // ''طلاب
                        {
                            accAccountId = await GetCustomerId(10, Code, facilityId);
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


        private async Task<long> GetCustomerId(int CusTypeId, string Code, long FacilityId)
        {
            // this function converted from [Sys_Customer_SP] when @CMDTYPE = 5

            long Id = 0;
            Id = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id,
                                    x => x.CusTypeId == CusTypeId && x.Code == Code && x.FacilityId == FacilityId && x.IsDeleted == false);
            return Id;
        }
    }
}