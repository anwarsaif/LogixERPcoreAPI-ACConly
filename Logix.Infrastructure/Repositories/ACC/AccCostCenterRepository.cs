using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;


namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccCostCenterRepository : GenericRepository<AccCostCenter, AccCostCenterVws>, IAccCostCenterRepository
    {
        private readonly ApplicationDbContext context;

        public AccCostCenterRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<long> GetCostCenterIdByCode(string code, long facilityId)
        {
            // return 0 if no acount with this code or an exception occur
            try
            {
                return await context.AccCostCenteHelpVws
                .Where(a => a.CostCenterCode == code && a.Isdel == false && a.FacilityId == facilityId)
                .Select(x => x.CcId).SingleOrDefaultAsync();
            }
            catch
            {
                return 0;
            }
        }
        public async Task<CostCenterCodeResult> GetCostCenterCode(long FacilityId, long parentId)
        {
            string costCenterCode = null;
            string costCenterCodeParent = "0";
            long costCenterLevel = 0;
            int noOfDigit = 0;
            long cntAccount = 0;

            try
            {
                // Get max CostCenter_Code under the parent
                var maxCostCenters = await context.AccCostCenter
                    .Where(x => x.CcParentId == parentId && x.FacilityId == FacilityId)
                    .ToListAsync();

                if (maxCostCenters.Any())
                {
                    cntAccount = maxCostCenters
                        .Select(c => long.TryParse(c.CostCenterCode, out var code) ? code : 0)
                        .DefaultIfEmpty(0)
                        .Max();
                }

                if (parentId != 0)
                {
                    var parent = await context.AccCostCenter.FirstOrDefaultAsync(x => x.CcId == parentId);
                    if (parent != null)
                    {
                        costCenterCodeParent = parent.CostCenterCode ?? "0";
                        costCenterLevel = parent.CostCenterLevel ?? 0;
                    }
                }

                // حذف بادئة الأب من cntAccount كما في SQL
                string cntAccountStr = cntAccount.ToString();
                int position = cntAccountStr.IndexOf(costCenterCodeParent);
                if (position >= 0)
                {
                    cntAccountStr = cntAccountStr.Remove(position, costCenterCodeParent.Length);
                    long.TryParse(cntAccountStr, out cntAccount);
                }

                costCenterLevel += 1;

                // Get NoOfDigit from Acc_CostCenters_Level
                var levelEntity = await context.AccCostCentersLevel
                    .FirstOrDefaultAsync(x => x.LevelId == costCenterLevel);

                noOfDigit = levelEntity?.NoOfDigit ?? 0;

                if (string.IsNullOrEmpty(costCenterCode))
                {
                    if (noOfDigit != 0)
                    {
                        if (noOfDigit <= costCenterCodeParent.Length)
                            throw new Exception("عدد الخانات المحدد في الإعدادات غير كافٍ لتوليد رمز جديد.");

                        int accountIsFound = 0;
                        int attemptsCounter = 0;
                        int maxAttempts = 10000;

                        while (accountIsFound == 0 && attemptsCounter < maxAttempts)
                        {
                            attemptsCounter++;

                            // توليد الكود مع الحشو
                            long newAccountNumber = cntAccount + 1;
                            string paddedNumber = newAccountNumber.ToString().PadLeft(noOfDigit - costCenterCodeParent.Length, '0');
                            costCenterCode = costCenterCodeParent + paddedNumber;

                            var exists = await context.AccCostCenter
                                .AnyAsync(x => x.CostCenterCode == costCenterCode && x.FacilityId == FacilityId);

                            if (!exists)
                                accountIsFound = 1;
                            else
                                cntAccount++;
                        }

                        if (attemptsCounter >= maxAttempts)
                            throw new Exception("تعذر العثور على رمز مركز تكلفة جديد بعد 10000 محاولة.");
                    }
                    else
                    {
                        costCenterCode = costCenterCodeParent + (cntAccount + 1).ToString();
                    }
                }
            }
            catch (Exception)
            {
                // بديل في حال فشل الإنشاء وفقًا للإعدادات
                costCenterLevel = 0;
                costCenterCodeParent = "0";

                var maxCostCenters = await context.AccCostCenter
                    .Where(x => x.CcParentId == parentId && x.FacilityId == FacilityId)
                    .ToListAsync();

                cntAccount = maxCostCenters
                    .Select(c => long.TryParse(c.CostCenterCode, out var code) ? code : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                var parent = await context.AccCostCenter.FirstOrDefaultAsync(x => x.CcId == parentId);
                if (parent != null)
                {
                    costCenterCodeParent = parent.CostCenterCode ?? "0";
                    costCenterLevel = (parent.CostCenterLevel ?? 0) + 1;
                }

                if (string.IsNullOrEmpty(costCenterCodeParent) || costCenterCodeParent == "0")
                {
                    bool codeFound = false;
                    while (!codeFound)
                    {
                        cntAccount++;
                        costCenterCode = cntAccount.ToString();
                        bool exists = await context.AccCostCenter
                            .AnyAsync(x => x.CostCenterCode == costCenterCode && x.FacilityId == FacilityId);
                        if (!exists)
                            codeFound = true;
                    }
                }
                else
                {
                    long newNo = cntAccount + 1;
                    costCenterCode = costCenterCodeParent + newNo.ToString().PadLeft(5, '0');
                }
            }

            return new CostCenterCodeResult
            {
                CostCenterCode = costCenterCode,
                CostCenterLevel = (int)costCenterLevel
            };
        }

        //public async Task<CostCenterCodeResult> GetCostCenterCode(long FacilityId, long parentId)
        //{
        //    string costCenterCode = null;
        //    string costCenterCodeParent = "0";
        //    long costCenterLevel = 0;
        //    int noOfDigit = 0;
        //    long cntAccount = 0;

        //    try
        //    {
        //        // Get max CostCenter_Code under the parent
        //        var maxCostCenters = await context.AccCostCenter
        //            .Where(x => x.CcParentId == parentId && x.FacilityId == FacilityId)
        //            .ToListAsync();

        //        if (maxCostCenters.Any())
        //        {
        //            cntAccount = maxCostCenters
        //                .Select(c => long.TryParse(c.CostCenterCode, out var code) ? code : 0)
        //                .DefaultIfEmpty(0)
        //                .Max();
        //        }

        //        if (parentId != 0)
        //        {
        //            var parent = await context.AccCostCenter.FirstOrDefaultAsync(x => x.CcId == parentId);
        //            if (parent != null)
        //            {
        //                costCenterCodeParent = parent.CostCenterCode ?? "0";
        //                costCenterLevel = parent.CostCenterLevel ?? 0;
        //            }
        //        }

        //        costCenterLevel += 1;

        //        // Get NoOfDigit from Acc_CostCenters_Level
        //        var levelEntity = await context.AccCostCentersLevel
        //            .FirstOrDefaultAsync(x => x.LevelId == costCenterLevel);

        //        noOfDigit = levelEntity?.NoOfDigit ?? 0;

        //        if (string.IsNullOrEmpty(costCenterCode))
        //        {
        //            if (noOfDigit != 0)
        //            {
        //                if (noOfDigit <= costCenterCodeParent.Length)
        //                    throw new Exception("عدد الخانات المحدد في الإعدادات غير كافٍ لتوليد رمز جديد.");

        //                int accountIsFound = 0;
        //                int attemptsCounter = 0;
        //                int maxAttempts = 10000;

        //                while (accountIsFound == 0 && attemptsCounter < maxAttempts)
        //                {
        //                    attemptsCounter++;
        //                    string nextCodeNumber = (cntAccount + 1).ToString();
        //                    int paddingLength = noOfDigit - costCenterCodeParent.Length - nextCodeNumber.Length;
        //                    string paddedNumber = new string('0', Math.Max(0, paddingLength)) + nextCodeNumber;

        //                    costCenterCode = costCenterCodeParent + paddedNumber;

        //                    var exists = await context.AccCostCenter
        //                        .AnyAsync(x => x.CostCenterCode == costCenterCode && x.FacilityId == FacilityId);

        //                    if (!exists)
        //                        accountIsFound = 1;
        //                    else
        //                        cntAccount++;
        //                }

        //                if (attemptsCounter >= maxAttempts)
        //                    throw new Exception("تعذر العثور على رمز مركز تكلفة جديد بعد 10000 محاولة.");
        //            }
        //            else
        //            {
        //                costCenterCode = costCenterCodeParent + (cntAccount + 1).ToString();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // بديل في حال فشل الإنشاء وفقًا للإعدادات
        //        costCenterLevel = 0;
        //        costCenterCodeParent = "0";
        //        var maxCostCenters = await context.AccCostCenter
        //            .Where(x => x.CcParentId == parentId && x.FacilityId == FacilityId)
        //            .ToListAsync();

        //        cntAccount = maxCostCenters
        //            .Select(c => long.TryParse(c.CostCenterCode, out var code) ? code : 0)
        //            .DefaultIfEmpty(0)
        //            .Max();

        //        var parent = await context.AccCostCenter.FirstOrDefaultAsync(x => x.CcId == parentId);
        //        if (parent != null)
        //        {
        //            costCenterCodeParent = parent.CostCenterCode ?? "0";
        //            costCenterLevel = (parent.CostCenterLevel ?? 0) + 1;
        //        }

        //        if (string.IsNullOrEmpty(costCenterCodeParent))
        //        {
        //            bool codeFound = false;
        //            while (!codeFound)
        //            {
        //                cntAccount++;
        //                costCenterCode = cntAccount.ToString();
        //                bool exists = await context.AccCostCenter
        //                    .AnyAsync(x => x.CostCenterCode == costCenterCode && x.FacilityId == FacilityId);
        //                if (!exists)
        //                    codeFound = true;
        //            }
        //        }
        //        else
        //        {
        //            long newNo = cntAccount + 1;
        //            costCenterCode = costCenterCodeParent + newNo.ToString().PadLeft(5, '0');
        //        }
        //    }
        //    return new CostCenterCodeResult
        //    {
        //        CostCenterCode = costCenterCode,
        //        CostCenterLevel = (int)costCenterLevel
        //    };

        //}
        public async Task<IEnumerable<AccCostCenterVws>> GetAllVW()
        {
            return await context.AccCostCenterVws.ToListAsync();
        }
        public async Task<long> getCostCenterByCode(string code, long facilityId)
        {
            try
            {
                return await context.AccCostCenter
                .Where(a => a.CostCenterCode == code && a.IsDeleted == false && a.FacilityId == facilityId && a.IsActive == true)
                .Select(x => x.CcId).SingleOrDefaultAsync();
            }
            catch
            {
                return 0;
            }
        }
    }
}

