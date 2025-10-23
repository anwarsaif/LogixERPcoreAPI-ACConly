using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccJournalDetaileRepository : GenericRepository<AccJournalDetaile, AccJournalDetailesVw>, IAccJournalDetaileRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccJournalDetaileRepository(ApplicationDbContext context,
            IMapper _mapper,
            ICurrentData session, ILocalizationService localization) : base(context)
        {
            this.context = context;
            this.mapper = _mapper;
            this.session = session;
            this.localization = localization;
        }

        public async Task<IEnumerable<AccJournalDetailesVw>> GetAllFromView(Expression<Func<AccJournalDetailesVw, bool>> expression)
        {
            try
            {
                return await context.Set<AccJournalDetailesVw>().Where(expression).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                return new List<AccJournalDetailesVw>();
            }
        }


        public async Task<IResult<AccJournalDetaile>> AddAccJournalDetail(AccJournalDetaileDto obj, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = mapper.Map<AccJournalDetaile>(obj);

                if (entity.AccAccountId == null || entity.AccAccountId == 0)
                    return await Result<AccJournalDetaile>.FailAsync($"الحساب الموجود في طرف القيد غير موجود في قائمة الحسابات");

                long facilityId = 0;
                facilityId = await context.AccJournalMasters.Where(j => j.JId == entity.JId).Select(j => j.FacilityId ?? 0).FirstOrDefaultAsync();

                var chkAccountExist = await context.AccAccounts
                            .Where(a => a.AccAccountId == entity.AccAccountId && a.IsDeleted == false && a.FacilityId == facilityId)
                            .Select(j => j.AccAccountId)
                            .FirstOrDefaultAsync();

                if (chkAccountExist == 0)
                {
                    string msg = "الحساب الموجود في طرف القيد غير موجود في قائمة الحسابات لهذه المنشآة. Acc_Account_ID = ";
                    msg += entity.AccAccountId.ToString();
                    return await Result<AccJournalDetaile>.FailAsync(msg);
                }
                entity.CcId = entity.CcId ?? 0;
                entity.Cc2Id = entity.Cc2Id ?? 0;
                entity.Cc3Id = entity.Cc3Id ?? 0;
                entity.Cc4Id = entity.Cc4Id ?? 0;
                entity.Cc5Id = entity.Cc5Id ?? 0;
                entity.EmpId = entity.EmpId ?? 0;
                entity.AssestId = entity.AssestId ?? 0;
                entity.ActivityId = entity.ActivityId ?? 0;
                entity.BranchId = entity.BranchId ?? 0;
                entity.ExchangeRate = entity.ExchangeRate ?? 0;
                entity.CreatedBy = session.UserId;
                entity.CreatedOn = DateTime.Now;
                var jDetaile = context.AccJournalDetailes.Add(entity).Entity;

                //await context.SaveChangesAsync(cancellationToken);
                //var jDetaile = await _accRepositoryManager.AccJournalDetaileRepository.AddAndReturn(masterItem);
                //await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                //return jDetaile;
                return await Result<AccJournalDetaile>.SuccessAsync(jDetaile, "item added successfully");
            }
            catch (Exception ex)
            {
                return await Result<AccJournalDetaile>.FailAsync($"{ex.Message}");
            }
        }

        public async Task<IResult<AccJournalDetaile>> UpdateAccJournalDetail(AccJournalDetaileDto entity, CancellationToken cancellationToken = default)
        {


            try
            {

                if (entity == null)
                    return await Result<AccJournalDetaile>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

                var item = await context.Set<AccJournalDetaile>().FindAsync(entity.JDetailesId);

                if (item == null)
                    return await Result<AccJournalDetaile>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

                if (entity.AccAccountId == null || entity.AccAccountId == 0)
                    return await Result<AccJournalDetaile>.FailAsync($"{localization.GetAccResource("AccAccountNotfind")}");

                long facilityId = 0;
                facilityId = await context.AccJournalMasters.Where(j => j.JId == entity.JId).Select(j => j.FacilityId ?? 0).FirstOrDefaultAsync();
                entity.CcId = entity.CcId ?? 0;
                entity.Cc2Id = entity.Cc2Id ?? 0;
                entity.Cc3Id = entity.Cc3Id ?? 0;
                entity.Cc4Id = entity.Cc4Id ?? 0;
                entity.Cc5Id = entity.Cc5Id ?? 0;
                entity.EmpId = entity.EmpId ?? 0;
                entity.AssestId = entity.AssestId ?? 0;
                entity.ActivityId = entity.ActivityId ?? 0;
                entity.BranchId = entity.BranchId ?? 0;
                entity.ExchangeRate = entity.ExchangeRate ?? 0;
                entity.ModifiedBy = (int)session.UserId;
                entity.ModifiedOn = DateTime.Now;
                mapper.Map(entity, item);

                context.Update(item);



                return await Result<AccJournalDetaile>.SuccessAsync(mapper.Map<AccJournalDetaile>(item), "Item updated successfully");
            }
            catch (Exception exc)
            {
                return await Result<AccJournalDetaile>.FailAsync($"EXP in {this.GetType()}, Message: {exc.Message}");
            }
        }

        public async Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesDebitor(long JID)
        {
            var journalDetails = await context.AccJournalDetailesVws
                .Where(x => x.FlagDelete == false && x.JId == JID && x.Debit > 0)
                .FirstOrDefaultAsync();

            if (journalDetails != null)
            {
                return await Result<AccJournalDetailesVw>.SuccessAsync(journalDetails, "تمت إسترجاع العنصر بنجاح");
            }
            else
            {
                return Result<AccJournalDetailesVw>.Fail("لم يتم العثور على تفاصيل المدين المحدد");
            }
        }

        public async Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesCredit(long JID)
        {
            var journalDetails = await context.AccJournalDetailesVws
                .Where(x => x.FlagDelete == false && x.JId == JID && x.Credit > 0)
                .FirstOrDefaultAsync();

            if (journalDetails != null)
            {
                return await Result<AccJournalDetailesVw>.SuccessAsync(journalDetails, "تمت إسترجاع العنصر بنجاح");
            }
            else
            {
                return Result<AccJournalDetailesVw>.Fail("لم يتم العثور على تفاصيل المدين المحدد");
            }
        }

        public async Task<List<AccJournalDetaile>> SelectACCJournalDetailesFacilityByID(long JId)
        {
            var results = await context.AccJournalDetailes
                .Where(d => d.IsDeleted == false && d.JId == JId)
                .ToListAsync();

            return results;
        }
        public async Task<List<long>> GetJournalIds(string jIds, long facilityId)
        {
            List<long> journalIds = new List<long>();

            try
            {
                journalIds = context.AccJournalDetailesVws
                    .Where(x => x.FlagDelete == false && x.JId != null && x.FacilityId == facilityId)
                    .GroupBy(x => new { x.JId, x.DocTypeId })
                    .Where(group => group.Sum(y => y.Debit - y.Credit) == 0 || group.All(y => y.DocTypeId == 4))
                    .Select(group => group.Key.JId)
                    .Where(jId => jIds.Contains(jId.ToString()))
                    .Select(jId => jId ?? 0)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return await Task.FromResult(journalIds);
        }






        public async Task<bool> AccountHasTransactions(long? accountId)
        {
            if (accountId == null)
                return false;

            try
            {
                var hasTransactions = await context.AccJournalDetailes
                    .AsNoTracking()
                    .AnyAsync(x => x.AccAccountId == accountId && x.IsDeleted == false)
                    .ConfigureAwait(false);

                return hasTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if the account has journal transactions.", ex);
            }
        }
        public async Task<int> GetCountAccJournalDetailes(long facilityId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await context.AccJournalDetailes
                    .AsNoTracking().Where(jd => jd.IsDeleted == false &&
                                 context.AccAccounts.Any(acc =>
                                     acc.AccAccountId == jd.AccAccountId &&
                                     acc.FacilityId == facilityId &&
                                     acc.SystemId == 2))
                    .CountAsync(cancellationToken);

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while counting journal details with account and facility conditions.", ex);
            }
        }
        public async Task<bool> ValdJournalDetailes(string accAccountCode, string costCenterCode)
        {



            bool ret = false;

            string parent0 = accAccountCode.Length >= 1 ? accAccountCode.Substring(0, 1) : "";
            string parent1 = accAccountCode.Length >= 2 ? accAccountCode.Substring(0, 2) : "";
            string parent2 = accAccountCode.Length >= 4 ? accAccountCode.Substring(0, 4) : "";

            if (parent2 == "3101")
            {
                if (costCenterCode == "80" || costCenterCode == "90")
                    ret = true;
                else
                    ret = false;
            }
            else if (parent2 == "3201")
            {
                if (costCenterCode == "80")
                    ret = false;
                else
                    ret = true;
            }
            else if (parent1 == "41")
            {
                if (costCenterCode == "90")
                    ret = false;
                else
                    ret = true;
            }
            else if (parent0 == "1" || parent0 == "2")
            {
                if (!string.IsNullOrEmpty(costCenterCode))
                    ret = true;
                else
                    ret = false;
            }

            return ret;
        }



    }
}