using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccJournalMasterRepository : GenericRepository<AccJournalMaster, AccJournalMasterVw>, IAccJournalMasterRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData session;
        private readonly IMapper _mapper;
        private readonly ILocalizationService localization;


        public AccJournalMasterRepository(ApplicationDbContext context, ICurrentData session, IMapper mapper, ILocalizationService localization) : base(context)
        {
            this.context = context;
            this.session = session;
            this._mapper = mapper;
            this.localization = localization;
        }

        public async Task<IEnumerable<AccJournalMasterVw>> GetAllAccJournalMasterVw()
        {
            return await context.AccJournalMasterVws.ToListAsync();
        }

        public int GetCount(Expression<Func<AccJournalMasterVw, bool>> expression)
        {
            return context.AccJournalMasterVws.Count(expression);
        }

        public async Task<int?> GetJournalMasterStatuse(long ReferenceNo, int DocTypeID)
        {
            try
            {
                if (DocTypeID > 0)
                {
                    return await context.AccJournalMasters.Where(X => X.ReferenceNo == ReferenceNo && X.DocTypeId == DocTypeID && X.FlagDelete == false).Select(x => x.StatusId).SingleOrDefaultAsync();
                }
                else
                {
                    return await context.AccJournalMasters.Where(X => X.ReferenceNo == ReferenceNo && (X.DocTypeId == 1 || X.DocTypeId == 2) && X.FlagDelete == false).Select(x => x.StatusId).SingleOrDefaultAsync();
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<int?> GetPostingStatuse(long facilityId)
        {
            try
            {
                if (facilityId > 0)
                {
                    return await context.AccFacilities.Where(X => X.FacilityId == facilityId).Select(x => x.Posting).SingleOrDefaultAsync();
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<bool> DeleteJournalWithDetailsbyReference(long ReferenceNo, int DocTypeId)
        {
            try
            {
                var masterJournal = await context.AccJournalMasters
                    .Where(x => x.ReferenceNo == ReferenceNo && x.DocTypeId == DocTypeId && x.FlagDelete == false).SingleOrDefaultAsync();

                if (masterJournal != null)
                {
                    masterJournal.FlagDelete = true;
                    masterJournal.DeleteUserId = (int?)session.UserId;
                    masterJournal.DeleteDate = DateTime.Now;

                    context.AccJournalMasters.Update(masterJournal);

                    //delete details
                    var journalDetails = await context.AccJournalDetailes
                    .Where(x => x.JId == masterJournal.JId && x.IsDeleted == false)
                    .ToListAsync();

                    foreach (var detail in journalDetails)
                    {
                        detail.IsDeleted = true;
                        detail.ModifiedBy = session.UserId;
                        detail.ModifiedOn = DateTime.Now;

                        detail.DeleteUserId = (int?)session.UserId;
                        detail.DeleteDate = DateTime.Now;
                    }

                    if (journalDetails.Any())
                    {
                        context.AccJournalDetailes.UpdateRange(journalDetails);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        private async Task<string> GetPropertyValue(long propertyId)
        {
            try
            {
                string? propertyValue;
                propertyValue = await context.SysPropertyValues.Where(p => p.PropertyId == propertyId && p.FacilityId == session.FacilityId).Select(p => p.PropertyValue).FirstOrDefaultAsync();

                return propertyValue ?? "";

            }
            catch
            {
                return "";
            }
        }
        public async Task<bool> CheckDateInFinancialYear(long FinYear, string Date)
        {
            var finYears = await context.Set<AccFinancialYear>().Where(f => f.FinYear == FinYear && f.IsDeleted == false).AsNoTracking().ToListAsync();
            var finYearRes = finYears.Where(f => !string.IsNullOrEmpty(f.StartDateGregorian) && !string.IsNullOrEmpty(f.EndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(f.StartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(f.EndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             );

            return finYearRes.Any(); //true if contains any financial year, otherWise false
        }
        public async Task<bool> CheckDateInPeriodByYear(long FinYear, string Date)
        {
            var periods = await context.Set<AccPeriods>().Where(p => p.FinYear == FinYear && p.FlagDelete == false).AsNoTracking().ToListAsync();
            var periodRes = periods.Where(p => !string.IsNullOrEmpty(p.PeriodStartDateGregorian) && !string.IsNullOrEmpty(p.PeriodEndDateGregorian) &&
                DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(p.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
                && DateTime.ParseExact(Date, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(p.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)
             );

            return periodRes.Any(); //true if contains any period, otherWise false
        }

        public async Task<IResult<AccJournalMaster>> AddACCJournalMaster(AccJournalMasterDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var masterItem = _mapper.Map<AccJournalMaster>(entity);

                //create code
                var numberByDocType = await GetPropertyValue(91);
                if (numberByDocType == "0")
                    return await Result<AccJournalMaster>.FailAsync("Please Adjust Numbering from configration for journal enteries if you wont by documnet type or not #91.");

                var numberByPeriod = await GetPropertyValue(197);

                var numberByBranch = await GetPropertyValue(92);
                if (numberByBranch == "0")
                    return await Result<AccJournalMaster>.FailAsync("Please Adjust Numbering from configration for journal enteries if you wont by branch or not #92.");

                var numberDocByBranch = await GetPropertyValue(93);
                if (numberDocByBranch == "0")
                    return await Result<AccJournalMaster>.FailAsync("Please Adjust Numbering from configration for documentes if you wont by branch or not #93.");

                var saveOldNo = await GetPropertyValue(90);
                if (saveOldNo == "0")
                    return await Result<AccJournalMaster>.FailAsync("Please Adjust Numbering from configration for jounral enteries auto if you wont creat new numner or not #90.");


                var chkPeriodAndYear = await context.AccPeriods.Where(p => p.FinYear == masterItem.FinYear && p.FlagDelete == false && p.PeriodId == masterItem.PeriodId).Select(p => p.PeriodId).FirstOrDefaultAsync();
                ////////////////////
                if (!(chkPeriodAndYear > 0))
                    return await Result<AccJournalMaster>.FailAsync("السنة المالية تختلف على الفترة المحاسبية المحددة");

                if (!string.IsNullOrEmpty(entity.JDateGregorian))
                {
                    bool chkFinYear = await CheckDateInFinancialYear(entity.FinYear ?? 0, entity.JDateGregorian);
                    if (!chkFinYear)
                        return await Result<AccJournalMaster>.FailAsync($"{localization.GetResource1("التاريخ للقيد خارج إطار تاريخ السنة")}");

                    bool chkPeriod = await CheckDateInPeriodByYear(entity.FinYear ?? 0, entity.JDateGregorian);

                    if (!chkPeriod)
                        return await Result<AccJournalMaster>.FailAsync("التاريخ للقيد خارج إطار تاريخ الفترة");
                }

                if (masterItem.CcId == null || masterItem.CcId == 0)
                    return await Result<AccJournalMaster>.FailAsync("الفرع غير محدد التسجيل");

                if (masterItem.ExchangeRate == 0)
                    return await Result<AccJournalMaster>.FailAsync("التعادل يجب ان يكون اكبر من الصفر");
                var chkPeriodState = await context.AccPeriods.Where(p => p.PeriodId == masterItem.PeriodId && p.FlagDelete == false && p.PeriodState == 2).Select(p => p.PeriodId).FirstOrDefaultAsync();
                if (chkPeriodState > 0)
                    return await Result<AccJournalMaster>.FailAsync("حالة الفترة المحاسبية مغلقة ولايمكن العمل عليها");
                var chkFinYearState = await context.AccFinancialYears.Where(f => f.FinYear == masterItem.FinYear && f.IsDeleted == false && f.FinState == 2).Select(f => f.FinYear).FirstOrDefaultAsync();
                if (chkFinYearState > 0)
                    return await Result<AccJournalMaster>.FailAsync("حالة السنة المالية مغلقة ولايمكن العمل عليها");


                long jCodeAut = 0;
                var allMasterJournals = await context.Set<AccJournalMaster>()
                    .Where(j => (numberByDocType != "1" || j.DocTypeId == masterItem.DocTypeId)
            && (numberByBranch != "1" || j.CcId == masterItem.CcId)
            && (numberByPeriod != "1" || j.PeriodId == masterItem.PeriodId)
            && j.FacilityId == masterItem.FacilityId
            && j.FinYear == masterItem.FinYear).Select(j => j.JCode).ToListAsync();

                if (allMasterJournals.Any())
                    jCodeAut = allMasterJournals.Max(j => Convert.ToInt64(j));

                jCodeAut += 1;

                var numberingTheSequence = await GetPropertyValue(333);
                if (numberingTheSequence == "1" && masterItem.DocTypeId == 2)
                {
                    string year = ""; string month = "";
                    if (!string.IsNullOrEmpty(masterItem.JDateGregorian))
                    {
                        year = masterItem.JDateGregorian.Substring(0, 4).Substring(2);
                        month = masterItem.JDateGregorian.Substring(0, 7).Substring(5, 2);


                        var allMasterJournalsRef = await context.Set<AccJournalMaster>().Where(j => j.DocTypeId == masterItem.DocTypeId
                        && j.FacilityId == masterItem.FacilityId
                        && j.FinYear == masterItem.FinYear
                        && !string.IsNullOrEmpty(j.JDateGregorian)
                        && j.JDateGregorian.Substring(1, 7) == masterItem.JDateGregorian.Substring(1, 7)).Select(j => j.ReferenceNo).ToListAsync();

                        masterItem.ReferenceNo = 0;
                        if (allMasterJournalsRef.Any())
                            masterItem.ReferenceNo = allMasterJournalsRef.Max(j => Convert.ToInt64(j));
                        masterItem.ReferenceNo += 1;
                        if (masterItem.ReferenceNo == 1)
                            masterItem.ReferenceNo = Convert.ToInt64(year + month + masterItem.ReferenceNo ?? 0.ToString().PadLeft(4, '0'));
                    }
                }
                else
                {
                    if (masterItem.DocTypeId == 1 || masterItem.DocTypeId == 2 || masterItem.DocTypeId == 3)
                    {
                        var allMasterJournalsRef = await context.Set<AccJournalMaster>().Where(j => j.DocTypeId == masterItem.DocTypeId
                             && j.PeriodId == masterItem.PeriodId
                             && j.FacilityId == masterItem.FacilityId
                             && j.FinYear == masterItem.FinYear
                             && (numberDocByBranch != "1" || j.CcId == masterItem.CcId)).Select(j => j.ReferenceNo).ToListAsync();

                        masterItem.ReferenceNo = 0;
                        if (allMasterJournalsRef.Any())
                            masterItem.ReferenceNo = allMasterJournalsRef.Max(j => Convert.ToInt64(j));
                        masterItem.ReferenceNo += 1;
                    }
                }

                string stringJCodeAut = jCodeAut.ToString();
                if (stringJCodeAut.Length <= 5)
                    masterItem.JCode = $"{jCodeAut:D5}";
                else
                    masterItem.JCode = stringJCodeAut;

                string jCodeOld = "";

                var allJournal = await context.Set<AccJournalMaster>().Where(j => j.DocTypeId == masterItem.DocTypeId
                && j.ReferenceNo == masterItem.ReferenceNo
                && j.FlagDelete == false).ToListAsync();

                int[] excludeDocTypes = new int[] { 1, 2, 3, 4, 27, 35 };
                var chkJournalExist = allJournal.Where(j => !excludeDocTypes.Contains(j.DocTypeId ?? 0) && j.FacilityId == masterItem.FacilityId);
                if (chkJournalExist.Any())
                {
                    jCodeOld = allJournal.Last().JCode ?? "";
                    List<long> jIds = new();
                    //delete master journal
                    foreach (var journalItem in allJournal)
                    {
                        jIds.Add(journalItem.JId);

                        journalItem.FlagDelete = true;
                        context.AccJournalMasters.Update(journalItem);
                        //await context.SaveChangesAsync(cancellationToken);
                    }
                    //delete details
                    var allDetail = await context.Set<AccJournalDetaile>().Where(d => d.IsDeleted == false && jIds.Contains(d.JId ?? 0)).ToListAsync();
                    if (allDetail.Any())
                    {
                        foreach (var detail in allDetail)
                        {
                            detail.IsDeleted = true;
                            context.AccJournalDetailes.Update(detail);
                            //await context.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                if (saveOldNo == "1" && !string.IsNullOrEmpty(jCodeOld))
                {
                    masterItem.JCode = jCodeOld;
                }

                masterItem.InsertDate = DateTime.Now;
                masterItem.InsertUserId = Convert.ToInt32(session.UserId);
                masterItem.JTime = DateTime.Now.ToString("HH:mm:ss");

                var journal = context.AccJournalMasters.Add(masterItem).Entity;
                //await context.SaveChangesAsync(cancellationToken);
                return await Result<AccJournalMaster>.SuccessAsync(journal, "item added successfully");
            }
            catch (Exception ex)
            {
                return await Result<AccJournalMaster>.FailAsync($"{ex.Message}");
            }

        }


        public async Task<string?> GetJCodeByReferenceNo(long ReferenceNo, int DocTypeID)
        {
            return await context.AccJournalMasterVws.Where(x => x.ReferenceNo == ReferenceNo && x.DocTypeId == DocTypeID && x.FlagDelete == false).Select(x => x.JCode).FirstOrDefaultAsync();
        }
        public async Task<AccJournalMaster> DeleteJournalWithDetailsByJId(long JId)
        {

            var masterJournal = await context.AccJournalMasters
                .Where(x => x.JId == JId && x.FlagDelete == false)
                .SingleOrDefaultAsync();

            if (masterJournal != null)
            {
                masterJournal.FlagDelete = true;
                masterJournal.DeleteUserId = (int?)session.UserId;
                masterJournal.DeleteDate = DateTime.Now;

                context.AccJournalMasters.Update(masterJournal);

                // Delete details
                var journalDetails = await context.AccJournalDetailes
                    .Where(x => x.JId == masterJournal.JId && x.IsDeleted == false)
                    .ToListAsync();

                foreach (var detail in journalDetails)
                {
                    detail.IsDeleted = true;
                    detail.ModifiedBy = session.UserId;
                    detail.ModifiedOn = DateTime.Now;

                    detail.DeleteUserId = (int?)session.UserId;
                    detail.DeleteDate = DateTime.Now;
                }

                if (journalDetails.Any())
                {
                    context.AccJournalDetailes.UpdateRange(journalDetails);
                }


                return masterJournal;
            }

            return null;


        }
        public async Task<IResult<AccJournalMaster>> UpdateACCJournalMaster(AccJournalMasterEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return await Result<AccJournalMaster>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!");

            try
            {
                var AccJournalMasterDto = entity;
                if (AccJournalMasterDto == null)
                    return await Result<AccJournalMaster>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!");

                var item = await context.Set<AccJournalMaster>().FindAsync(AccJournalMasterDto.JId);

                if (item == null)
                    return await Result<AccJournalMaster>.FailAsync($"--- there is no Data with this id: {AccJournalMasterDto.JId}---");

                //====التشييك على حالة القيد  
                var journalStatus = await GetJournalMasterStatuseByJId(AccJournalMasterDto.JId);
                if (journalStatus == 2)
                    return await Result<AccJournalMaster>.FailAsync($"{localization.GetResource1("NotAbleAnyAction")}");
                //------------------------تشيك التاريخ

                if (await DateHelper.CheckDate(AccJournalMasterDto.JDateGregorian, session.FacilityId, session.CalendarType) == true)
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<AccJournalMaster>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //--------------------اضافة قيد موقت
                int? Status_Id = 0;
                if (entity.StatusId != 5)
                {
                    Status_Id = await GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = entity.StatusId;
                }
                AccJournalMasterDto.StatusId = Status_Id;
                AccJournalMasterDto.FacilityId = session.FacilityId;
                AccJournalMasterDto.FinYear = session.FinYear;
                AccJournalMasterDto.FacilityId = session.FacilityId;

                _mapper.Map(AccJournalMasterDto, item);
                item.UpdateDate = DateTime.Now;
                item.UpdateUserId = Convert.ToInt32(session.UserId);
                item.JTime = DateTime.Now.ToString("HH:mm:ss");
                context.Update(item);



                return await Result<AccJournalMaster>.SuccessAsync(_mapper.Map<AccJournalMaster>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccJournalMaster>.FailAsync($"EXP in {this.GetType()}, Message: {exc.Message}");
            }
        }
        public async Task<int?> GetJournalMasterStatuseByJId(long JId)
        {
            try
            {
                return await context.AccJournalMasters.Where(X => X.JId == JId && X.FlagDelete == false).Select(x => x.StatusId).SingleOrDefaultAsync();

            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<int> NumberExists(string referenceCode, long ccId, int docTypeId, long facilityId, long periodId)
        {
            var count = await context.AccJournalMasters
                .Where(j => j.ReferenceCode == referenceCode && j.CcId == ccId && j.DocTypeId == docTypeId && j.FacilityId == facilityId && j.PeriodId == periodId && j.FlagDelete == false)
                .CountAsync();

            return count;
        }

        public async Task<int> GetBookSerial(long facilityId, long branchId, int DocTypeId)
        {
            var newBookNoList = context.AccJournalMasters
                .Where(j => j.FlagDelete == false
                         && j.ReferenceCode.Length > 0
                         && j.ReferenceCode != null
                         && j.FacilityId == facilityId
                         && j.DocTypeId == DocTypeId
                         && j.CcId == branchId)
                .Select(j => j.ReferenceCode)
                .AsEnumerable() // Switch to LINQ to Objects
                .Select(code => long.TryParse(code, out var parsedRefCode) ? parsedRefCode : 0)
                .DefaultIfEmpty(0)
                .ToList();

            var maxNewBookNo = newBookNoList.Max() + 1;

            return (int)maxNewBookNo;
        }

        public async Task<decimal> GetBalanceForAccount(long accAccountId, long facilityId, long finYear)
        {
            var balance = await context.AccBalanceSheets
                .Where(b => b.AccAccountId == accAccountId &&
                            b.FlagDelete == false &&
                            b.FacilityId == facilityId &&
                            b.FinYear == finYear)
                .GroupBy(b => 1) // Group by a constant to calculate the aggregate over all rows
                .Select(g => g.Sum(b => b.NatureAccount > 0 ? b.Credit - b.Debit : b.Debit - b.Credit))
                .FirstOrDefaultAsync();

            return balance ?? 0;
        }

        public async Task<AccJournalMaster> SelectACCJournalFacilityByID(long ReferenceMappingID)
        {
            var result = await context.AccJournalMasters
                .Where(j => j.FlagDelete == false &&
                            j.ReferenceMappingId != null &&
                            j.ReferenceMappingId != 0 &&
                            j.ReferenceMappingId == ReferenceMappingID &&
                            j.ReferenceMappingId != j.JId)
                .FirstOrDefaultAsync();

            return result;
        }
        public async Task<long> GetJIDByJCode2(string JCode, int DocTypeId, long facilityId, long Finyear)
        {
            var JID = await context.AccJournalMasters
                .Where(j => j.FlagDelete == false
                        && j.JCode == JCode
                        && j.FacilityId == facilityId
                        && j.DocTypeId == DocTypeId
                        && j.FinYear == Finyear)
                .Select(j => j.JId)
                .FirstOrDefaultAsync();

            return JID;
        }

        public async Task<IResult<AccJournalMaster>> TransferOprations(AccJournalMasterEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return await Result<AccJournalMaster>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            try
            {

                var item = await context.Set<AccJournalMaster>().FindAsync(entity.JId);

                if (item == null)
                {
                    return await Result<AccJournalMaster>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}");
                }


                var chkPeriodState = await context.AccPeriods
                    .Where(p => p.PeriodId == item.PeriodId && p.FlagDelete == false && p.PeriodState == 2)
                    .Select(p => p.PeriodId)
                    .FirstOrDefaultAsync();

                if (chkPeriodState > 0)
                {
                    return await Result<AccJournalMaster>.FailAsync("حالة الفترة المحاسبية مغلقة ولا يمكن العمل عليها");
                }

                var chkFinYearState = await context.AccFinancialYears
                    .Where(f => f.FinYear == item.FinYear && f.IsDeleted == false && f.FinState == 2)
                    .Select(f => f.FinYear)
                    .FirstOrDefaultAsync();

                if (chkFinYearState > 0)
                {
                    return await Result<AccJournalMaster>.FailAsync("حالة السنة المالية مغلقة ولا يمكن العمل عليها");
                }


                item.StatusId = entity.StatusId;
                item.UpdateDate = DateTime.Now;
                item.UpdateUserId = Convert.ToInt32(session.UserId);
                item.JTime = DateTime.Now.ToString("HH:mm:ss");
                context.Update(item);



                return await Result<AccJournalMaster>.SuccessAsync(_mapper.Map<AccJournalMaster>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccJournalMaster>.FailAsync($"EXP in {this.GetType()}, Message: {exc.Message}");
            }
        }


        public async Task<int> CheckFinyearHasTransaction(long FinYear)
        {
            var count = await context.AccJournalMasters
                .Where(j => j.FinYear == FinYear && j.FlagDelete == false)
                .CountAsync();

            return count;
        }


        public async Task<int> CheckPreiodHasTransaction(long periodId)
        {
            var count = await context.AccJournalMasters
                .Where(j => j.PeriodId == periodId && j.FlagDelete == false)
                .CountAsync();

            return count;
        }
        public async Task<int> CheckCostCenterHasTransaction(long CCID)
        {
            var count = await context.AccJournalDetailesVws
                .Where(j => j.FlagDelete == false && (j.CcId == CCID || j.Cc2Id == CCID || j.Cc3Id == CCID || j.Cc4Id == CCID || j.Cc5Id == CCID))
                .CountAsync();

            return count;
        }



    }
}
