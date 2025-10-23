using Castle.MicroKernel.Registration;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.SAL;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.SAL;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Logix.Application.Helpers.Acc
{
    public interface IGetAccJournaCodeHelper
    {
        Task<string> GetAccJournCode<T>(AccJournalVM Entity) where T : class;
        Task<bool> CheckDate(string curDate);
    }

    public class GetAccJournaCodeHelper : IGetAccJournaCodeHelper
    {
        private readonly IMainServiceManager serviceManager;
        private readonly ICurrentData _session;
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IHrRepositoryManager hrRepositoryManager;

        public GetAccJournaCodeHelper(IMainServiceManager serviceManager, ICurrentData session, IAccRepositoryManager accRepositoryManager, IMainRepositoryManager mainRepositoryManager, IHrRepositoryManager hrRepositoryManager)
        {
            this.serviceManager = serviceManager;
            _session = session;
            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this.hrRepositoryManager = hrRepositoryManager;
        }

        public async Task<string> GetAccJournCode<T>(AccJournalVM Entity) where T : class
        {
            try
            {
                string Code = null;

                //--------------------------------'--ترقيم القيود حسب النوع 
                string NumberByDocType = "0";
                var DocType = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(91, _session.FacilityId);
                if (DocType != null)
                {
                    NumberByDocType = DocType.PropertyValue;
                }
                if (NumberByDocType == "0")
                {
                    //return await Result<T>.FailAsync("السنة المالية تختلف على الفترة المحاسبية المحددة.");

                    throw new Exception("Please Adjust Numbering from configuration for journal entries if you want by document type or not #91.");

                }

                //------------------------------------ترقيم القيود حسب الفترة المحاسبية
                string NumberByPeriod = "0";
                var ByPeriod = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(197, _session.FacilityId);
                if (ByPeriod != null)
                {
                    NumberByPeriod = ByPeriod.PropertyValue;
                }

                //------------------------------------ترقيم القيود حسب الفرع
                string NumberByBranch = "0";
                var ByBranch = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(92, _session.FacilityId);
                if (ByBranch != null)
                {
                    NumberByBranch = ByBranch.PropertyValue;
                }
                if (NumberByBranch == "0")
                {
                    throw new Exception("Please Adjust Numbering from configuration for journal entries if you want by branch or not #92.");
                }

                //------------------------------------ترقيم السندات حسب الفرع
                string NumberDocByBranch = "0";
                var DocByBranch = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(93, _session.FacilityId);
                if (DocByBranch != null)
                {
                    NumberDocByBranch = DocByBranch.PropertyValue;
                }
                if (NumberDocByBranch == "0")
                {
                    throw new Exception("Please Adjust Numbering from configuration for documents if you want by branch or not #93.");
                }

                //--التحقق من الفترة المحاسبية والتاريخ والسنة المالية
                var Period_ID = await accRepositoryManager.AccPeriodsRepository.GetAll(x => x.FinYear == _session.FinYear && x.PeriodId == Entity.PeriodId && x.FlagDelete == false);
                if (Period_ID.Count() == 0)
                {
                    throw new Exception("The financial year differs from the specified accounting period.");
                }

                //-----------------------هل تاريخ القيد خلال السنة ام لا
                var FinancialYear2 = await accRepositoryManager.AccFinancialYearRepository.GetAll(x => x.FinYear == _session.FinYear && x.IsDeleted == false);
                DateTime parsedDate = DateTime.ParseExact(Entity.Date1, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                if (FinancialYear2 != null)
                {
                    var FinancialYear = FinancialYear2.Where(x =>
                     parsedDate >= DateTime.ParseExact(x.StartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                        && parsedDate <= DateTime.ParseExact(x.EndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture));
                    if (FinancialYear.Count() == 0)
                    {
                        throw new Exception("السنة المالية تختلف على الفترة المحاسبية المحددة.");

                    }

                }

                //------------هل تاريخ القيد خلال الفترة ام لا

                var PeriodID2 = await accRepositoryManager.AccPeriodsRepository.GetAll(x => x.FinYear == _session.FinYear && x.FlagDelete == false);

                if (PeriodID2 != null)
                {
                    var PeriodID = PeriodID2.Where(x =>
                     parsedDate >= DateTime.ParseExact(x.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                        && parsedDate <= DateTime.ParseExact(x.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture));
                    if (PeriodID.Count() == 0)
                    {
                        throw new Exception("التاريخ للقيد خارج إطار تاريخ الفترة.");
                        //return await Result<SalTransactionDto>.FailAsync("التاريخ للقيد خارج إطار تاريخ الفترة.");

                    }

                }
                //-------------التشييك على حالة السنة المالية
                var Finyearstate = await accRepositoryManager.AccFinancialYearRepository.GetAll(x => x.FinYear == _session.FinYear && x.IsDeleted == false && x.FinState == 2);

                if (Finyearstate.Count() > 0)
                {
                    throw new Exception("حالة السنة المالية مغلقة ولايمكن العمل عليها.");

                    //return await Result<SalTransactionDto>.FailAsync("حالة السنة المالية مغلقة ولايمكن العمل عليها.");

                }
                //----------------التشييك على الفرع
                if (Entity.BranchId == 0)
                {
                    throw new Exception("الفرع غير محدد التسجيل.");

                    //return await Result<SalTransactionDto>.FailAsync("الفرع غير محدد التسجيل.");

                }
                //------------------التشييك على التعادل
                if (Entity.ExchangeRate == 0)
                {
                    throw new Exception("التعادل يجب ان يكون اكبر من الصفر");
                    //return await Result<SalTransactionDto>.FailAsync("التعادل يجب ان يكون اكبر من الصفر");

                }

                //--------------------التشييك على حالة الفترة
                var Periodstate = await accRepositoryManager.AccPeriodsRepository.GetAll(x => x.PeriodId == Entity.PeriodId && x.FlagDelete == false && x.PeriodState == 2);
                if (Periodstate.Count() > 0)
                {
                    throw new Exception("حالة السنة المالية مغلقة ولايمكن العمل عليها.");

                    //return await Result<SalTransactionDto>.FailAsync("حالة السنة المالية مغلقة ولايمكن العمل عليها.");

                }
                //--------------------------------' تفعيل مراكز التكلفة في الدائن في فاتورة  المبيعات 
                string ActiveCCIDCredit = "0";
                var get = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(57, _session.FacilityId);
                if (get != null)
                {
                    ActiveCCIDCredit = get.PropertyValue;
                }
                if (string.IsNullOrEmpty(ActiveCCIDCredit))
                {
                    ActiveCCIDCredit = "0";
                }

                //------------------------------توليد الكود
                var items22 = await accRepositoryManager.AccJournalMasterRepository.GetAll();
                long codeAut = 0;
                long ReferenceNo;
                if (items22.Any())
                {
                    var filteredItems = items22.Where(x => x.DocTypeId == (NumberByDocType == "1" ? Entity.DocTypeId : x.DocTypeId)
                    && x.CcId == (NumberByBranch == "1" ? Entity.BranchId : x.CcId)
                    && x.PeriodId == (NumberByPeriod == "1" ? Entity.PeriodId : x.PeriodId)
                    && x.FacilityId == _session.FacilityId && x.FinYear == _session.FinYear
                    );
                    codeAut = filteredItems.Select(t => !string.IsNullOrEmpty(t.JCode) ? Convert.ToInt64(t.JCode) : 0).DefaultIfEmpty().Max() + 1;
                }
                //------------------------ترقيم تسلسلي للسندات القبض والصرف
                if (Entity.DocTypeId == 1 || Entity.DocTypeId == 2 || Entity.DocTypeId == 3)
                {
                    var ReferenceNoAll = await accRepositoryManager.AccJournalMasterRepository.GetAll();
                    if (ReferenceNoAll.Any())
                    {
                        var filteredItems = items22.Where(x => x.DocTypeId == Entity.DocTypeId && x.PeriodId == Entity.PeriodId
                        && x.FacilityId == _session.FacilityId && x.FinYear == _session.FinYear
                         && x.CcId == (NumberByBranch == "1" ? Entity.BranchId : x.CcId)
                        );
                        ReferenceNo = filteredItems.Select(t => !string.IsNullOrEmpty(t.ReferenceNo.ToString()) ? Convert.ToInt64(t.ReferenceNo) : 0).DefaultIfEmpty().Max() + 1;
                    }

                }
                if (codeAut.ToString().Length <= 5)
                {
                    string paddedValue = "0000" + codeAut;
                    string formattedValue = paddedValue.Substring(paddedValue.Length - 5);
                    Code = formattedValue;
                }
                else
                {
                    Code = codeAut.ToString();
                }

                return Code;
            }
            catch (Exception exp)
            {
                Console.WriteLine($"=== Exp in get of {GetType()}, Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")} .");
                throw exp;
            }
        }
        //public async Task<IResult<TDto>> GetAccJournCode<T>(AccJournalVM Entity) where T : class
        //{
        //    try
        //    {
        //        string Code = null;

        //        //-------------------------------- '--ترقيم القيود حسب النوع 
        //        string NumberByDocType = "0";
        //        var DocType = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(91, _session.FacilityId);
        //        if (DocType != null)
        //        {
        //            NumberByDocType = DocType.PropertyValue;
        //        }
        //        if (NumberByDocType == "0")
        //        {
        //            return await Result<TDto>.FailAsync("السنة المالية تختلف على الفترة المحاسبية المحددة.");
        //        }

        //        // الأكواد الأخرى هنا...

        //        return await Result<TDto>.SuccessAsync(default(TDto), "");
        //    }
        //    catch (Exception exp)
        //    {
        //        Console.WriteLine($"=== Exp in get of {GetType()}, Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")} .");
        //        throw exp;
        //    }
        //}
        public async Task<bool> CheckDate(string curDate)
        {
            bool ret = false;
            try
            {
                //------------------------------------نوع التقويم المعتمد
                string CalendarType = "0";
                var Calendar = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(19,_session.FacilityId);
                if (Calendar != null)
                {
                    CalendarType = Calendar.PropertyValue;
                }
                

                int year = int.Parse(curDate.Substring(0, 4));
                if (CalendarType == "1")
                {
                    if (year >= 1900 && year <= 2100)
                        ret = true;
                    else
                        return false;
                }
                else
                {
                    if (year >= 1300 && year <= 1500)
                        ret = true;
                    else
                        return false;
                }

                int month = int.Parse(curDate.Substring(5, 2));
                if (month < 1 || month > 12)
                    return false;

                int day = int.Parse(curDate.Substring(8, 2));
                if (day < 1 || day > 31)
                    return false;

                if (curDate[4] != '/' || curDate[7] != '/')
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid date specified", ex);
            }

            return ret;
        }
    }
}