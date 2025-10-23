using Logix.Application.Common;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccAccountRepository : GenericRepository<AccAccount, AccAccountsVw>, IAccAccountRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData session;

        public AccAccountRepository(ApplicationDbContext context, ICurrentData session) : base(context)
        {
            this.context = context;
            this.session = session;
        }
        public async Task<string> GetAccountCode(long facilityId, long AccAccountParentId)
        {
            string maxAccAccountCode = "";
            try
            {
                string accAccountCodeParent = "";
                long LevelId = 0;
                int? noOfDigit = 0;
                string accAccountCode = null;
                long maxAccountCode = 0;

                // Retrieve max account code and account level
                var maxAccountCodes = await context.AccAccounts.Where(x => x.AccAccountParentId == AccAccountParentId && x.FacilityId == facilityId && x.SystemId == 2).ToListAsync();
                if (maxAccountCodes != null && maxAccountCodes.Any())
                {
                    maxAccountCode = maxAccountCodes.Select(t => !string.IsNullOrEmpty(t.AccAccountCode) ? Convert.ToInt64(t.AccAccountCode) : 0).DefaultIfEmpty(0).Max();
                    accAccountCodeParent = maxAccountCode.ToString();
                    LevelId = maxAccountCodes.First().AccountLevel ?? 0;
                }

                // Determine number of digits
                var LevelDigitEntity = await context.AccAccountsLevels.FirstOrDefaultAsync(s => s.LevelId == LevelId);
                if (LevelDigitEntity != null)
                {
                    noOfDigit = LevelDigitEntity.NoOfDigit;
                }

                // Generate new account code
                int cntAccount = 0;
                if (string.IsNullOrEmpty(accAccountCode))
                {
                    if (noOfDigit != 0)
                    {
                        int accountIsFound = 0;
                        while (accountIsFound == 0)
                        {
                            int newAccountNumber = cntAccount + 1;
                            string newAccountCode = newAccountNumber.ToString();

                            // Calculate the remaining digits needed for padding
                            int remainingDigits = Math.Max(0, noOfDigit.Value - accAccountCodeParent.Length - newAccountCode.Length);
                            string paddedAccountCode = new string('0', remainingDigits) + newAccountCode;

                            long accAccountCodeParentValue = Convert.ToInt64(accAccountCodeParent);
                            long paddedAccountCodeValue = Convert.ToInt64(paddedAccountCode);

                            accAccountCode = (accAccountCodeParentValue + paddedAccountCodeValue).ToString();

                            // Check if the account code already exists
                            var existingAccount = await context.AccAccounts.FirstOrDefaultAsync(X => X.AccAccountCode == accAccountCode && X.FacilityId == facilityId && X.SystemId == 2);
                            if (existingAccount == null)
                            {
                                accountIsFound = 1;
                            }
                            else
                            {
                                cntAccount++;
                            }
                        }
                    }
                    else
                    {
                        accAccountCode = accAccountCodeParent + (cntAccount + 1).ToString();
                    }
                }

                maxAccAccountCode = accAccountCode;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return maxAccAccountCode;
        }
        public async Task<long> GetAccountIdByCode(string code, long facilityId)
        {
            // return 0 if no acount with this code or an exception occur
            try
            {
                return await context.AccAccountsSubHelpeVws
                .Where(a => a.AccAccountCode == code && a.Isdel == false && a.FacilityId == facilityId && a.IsActive == true && a.SystemId == 2)
                .Select(x => x.AccAccountId).SingleOrDefaultAsync();
            }
            catch
            {
                return 0;
            }
        }
        public async Task<long?> GetAccGroupId(long AccAccountParentId, long facilityId)
        {
            try
            {
                if (facilityId > 0)
                {
                    return await context.AccAccounts.Where(X => X.AccAccountId == AccAccountParentId && X.FacilityId == facilityId && X.IsDeleted == false && X.SystemId == 2).Select(x => x.AccGroupId).SingleOrDefaultAsync();
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
        public async Task<long?> GetAccountLevel(long AccAccountParentId, long facilityId)
        {
            try
            {
                if (facilityId > 0)
                {
                    return await context.AccAccounts.Where(X => X.AccAccountId == AccAccountParentId && X.FacilityId == facilityId && X.IsDeleted == false && X.SystemId == 2).Select(x => x.AccountLevel).SingleOrDefaultAsync();
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

        public async Task<bool> ISHelpAccount(string accAccountCode, long facilityId)
        {
            try
            {
                if (facilityId > 0)
                {
                    var isHelpAccount = await context.AccAccounts
                        .Where(x => x.AccAccountCode.Equals(accAccountCode) && x.FacilityId == facilityId && x.IsDeleted == false && x.SystemId == 2)
                        .Select(x => x.IsHelpAccount)
                        .SingleOrDefaultAsync();

                    return isHelpAccount ?? false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if account is a help account.", ex);
            }
        }





        public async Task<long> GetCuurenyAccountCode(long AccountType, string code, long facilityId)
        {
            try
            {
                if (AccountType > 0)
                {
                    return await context.AccAccounts
                        .Where(x => x.IsDeleted == false && x.FacilityId == facilityId && x.AccAccountCode.Equals(code) && x.SystemId == 2)
                        .Select(x => x.CurrencyId)
                        .SingleOrDefaultAsync() ?? 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving Currency AccountCode.", ex);
            }
        }
        public async Task<int> GetCuureny(long? AccountID, long? facilityId)
        {

            try
            {
                AccountID ??= 0;
                if (facilityId == 0 || facilityId == null)
                    facilityId = session.FacilityId;
                // اذا كان رقم الحساب فارغ نقوم بجلب العملة الافتراضية
                if (AccountID == 0)
                {
                    return (int)await context.SysExchangeRates.Select(x => x.CurrencyToID).FirstOrDefaultAsync();

                }
                int? CurrencyId = await context.AccAccounts.Where(c => c.IsDeleted == false && c.FacilityId == facilityId && c.AccAccountId == AccountID)
                .Select(x => x.CurrencyId).SingleOrDefaultAsync();

                return (int)CurrencyId;
            }
            catch (Exception ex)
            {
                //  في حال حدوث أي مشاكل نرجع له العمله رقم 1
                return 1;
            }
        }

        public async Task<string?> GetAccountCodeById(long AccountId, CancellationToken cancellationToken = default)
        {
            return await context.AccAccounts
                .Where(x => x.AccAccountId == AccountId && x.SystemId == 2)
                .Select(x => x.AccAccountCode)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        public async Task<long?> GetAccLevelDigits(int LevelID, CancellationToken cancellationToken = default)
        {
            var digits = await context.AccAccountsLevels.Where(x => x.LevelId == LevelID).Select(x => (long?)x.NoOfDigit)
         .FirstOrDefaultAsync(cancellationToken)
         .ConfigureAwait(false);

            return digits;
        }

        public async Task<bool> IsAccAccountParent(long? accountId, long facilityId)
        {
            if (accountId == null)
                return false;

            try
            {
                var hasTransactions = await context.AccAccounts
                    .AsNoTracking()
                    .AnyAsync(x => x.FacilityId == facilityId && x.AccAccountParentId == accountId && x.IsDeleted == false && x.SystemId == 2)
                    .ConfigureAwait(false);

                return hasTransactions;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if the Is Acc Account Parent.", ex);
            }
        }
        public async Task<int> DeleteAllAccAccounts(long facilityId, long userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var accounts = await context.AccAccounts
                    .Where(a => a.FacilityId == facilityId)
                    .ToListAsync(cancellationToken);

                foreach (var account in accounts)
                {
                    account.IsDeleted = true;
                    account.DeleteUserId = (int)userId;
                    account.DeleteDate = DateTime.Now;
                }

                await context.SaveChangesAsync(cancellationToken);

                return accounts.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while soft-deleting all ACC_Accounts records.", ex);
            }
        }

    }
}
