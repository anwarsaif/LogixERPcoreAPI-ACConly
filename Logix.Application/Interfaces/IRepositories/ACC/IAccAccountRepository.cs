using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccAccountRepository : IGenericRepository<AccAccount, AccAccountsVw>
    {
        Task<long> GetAccountIdByCode(string code, long facilityId);
        Task<long?> GetAccGroupId(long AccAccountParentId, long facilityId);
        Task<long?> GetAccountLevel(long AccAccountParentId, long facilityId);

        Task<bool> ISHelpAccount(string code, long facilityId);
        Task<long> GetCuurenyAccountCode(long AccountType, string code, long facilityId);


        //  جلب العملة على حسب رقم الحساب واذا كان رقم الحساب فارغ نجلب العملة الافتراضية
        Task<int> GetCuureny(long? AccountID, long? facilityId);
        Task<string> GetAccountCode(long facilityId, long AccAccountParentId);

        Task<string?> GetAccountCodeById(long AccountId, CancellationToken cancellationToken = default);
        Task<long?> GetAccLevelDigits(int LevelID, CancellationToken cancellationToken = default);

        Task<bool> IsAccAccountParent(long? accountId, long facilityId);
        Task<int> DeleteAllAccAccounts(long facilityId, long userId, CancellationToken cancellationToken = default);
    }
}