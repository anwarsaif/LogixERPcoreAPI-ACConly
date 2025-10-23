using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccRequestRepository : GenericRepository<AccRequest, AccRequestVw>, IAccRequestRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData currentData;

        public AccRequestRepository(ApplicationDbContext context, ICurrentData currentData) : base(context)
        {
            this.context = context;
            this.currentData = currentData;
        }
        private async Task<string> GetPropertyValue(long propertyId)
        {
            try
            {
                string? propertyValue;
                propertyValue = await context.SysPropertyValues.Where(p => p.PropertyId == propertyId && p.FacilityId == currentData.FacilityId).Select(p => p.PropertyValue).FirstOrDefaultAsync();

                return propertyValue ?? "";

            }
            catch
            {
                return "";
            }
        }
        public async Task<long> GetAccRequestCode(string AppDate)
        {
            string numberingByYear = "0";

            numberingByYear = await GetPropertyValue(115);
            if (string.IsNullOrEmpty(numberingByYear))
            {
                numberingByYear = "0";
            }

            long codeAut = 0;

            var maxAppCode = await context.AccRequests.Where(j => j.AppDate.Substring(1, 4) == (numberingByYear == "1" ? AppDate.Substring(1, 4) : j.AppDate.Substring(1, 4))
                                     && (j.IsDeleted == null || j.IsDeleted == false) && j.FacilityId == currentData.FacilityId)
                                  .MaxAsync(j => (int?)j.AppCode) ?? 0;

            codeAut = maxAppCode + 1;





            return codeAut;
        }

        public async Task<List<AccRequestVw>> GetRequestWaitingTransfer(long facilityId, int transTypeId, long statusId, long appCode)
        {
            var filteredData = await context.AccRequestVws
                .Where(r => r.IsDeleted == false &&
                            r.FacilityId == facilityId &&
                            r.TransTypeId == transTypeId &&
                            r.StatusId == statusId &&
                            !context.AccRequests
                                .Where(ar => ar.IsDeleted == false && ar.TransTypeId == 2)
                                .Select(ar => ar.RefranceId ?? 0)
                                .Contains(r.Id) &&
                            (appCode == 0 || r.AppCode == appCode))

                .ToListAsync();

            return filteredData;
        }


        public async Task<List<TransactionResult>> GetUnPaidPO(string transTypeIds, string code, string dateText)
        {
            try
            {
                var transTypeIdList = transTypeIds.Split(',').Select(int.Parse).ToList();

                var query = from t in context.PurTransactionsVws
                            where t.IsDeleted == false
                                  && t.TransTypeId.HasValue
                                  && transTypeIdList.Contains(t.TransTypeId.Value)
                            select new
                            {
                                t.Id,
                                t.Code,
                                t.Date1,
                                t.Subtotal,
                                t.VatAmount,
                                t.SupplierCode,
                                t.SupplierName,
                                Paid = context.purTransactionsPayments
                                        .Where(p => p.TransactionId == t.Id && p.IsDeleted == false)
                                        .Sum(p => (decimal?)p.AmountReceived) ?? 0
                            };

                if (!string.IsNullOrEmpty(code))
                {
                    query = query.Where(q => q.Code == code);
                }

                if (!string.IsNullOrEmpty(dateText) && DateTime.TryParse(dateText, out var parsedDate))
                {
                    var formattedDate = parsedDate.ToString("yyyy-MM-dd");
                    query = query.Where(q => q.Date1 == formattedDate);
                }

                // تنفيذ الاستعلام على مستوى قاعدة البيانات
                var list = await query
                    .Where(q => (q.Subtotal ?? 0) > q.Paid)
                    .OrderBy(q => q.Code)
                    .ToListAsync();

                // بعد تحميل النتائج إلى الذاكرة، تطبيق الترقيم
                var resultList = list
                    .Select((q, index) => new TransactionResult
                    {
                        RowNumber = index + 1,
                        Code = q.Code,
                        Date1 = q.Date1,
                        Subtotal = q.Subtotal ?? 0,
                        Paid = q.Paid,
                        Total = (q.Subtotal + q.VatAmount) ?? 0,
                        SupplierCode = q.SupplierCode,
                        SupplierName = currentData.Language == 1 ? q.SupplierName : q.SupplierName
                    })
                    .ToList();

                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<TransactionResult>();
            }
        }
        public async Task<List<TransactionUnPaidResult>> GetUnPaidSubEx(string code, string dateText, int paymentTermsId = 0)
        {
            try
            {
                var query = from item in context.PmExtractTransactionsVws
                            where item.IsDeleted == false
                                  && item.TransTypeId.HasValue
                                  && (paymentTermsId == 0 || item.PaymentTermsId == paymentTermsId)
                            select new
                            {
                                item.Id,
                                item.Code,
                                item.Date1,
                                item.CustomerCode,
                                item.CustomerName,
                                item.ProjectName,
                                item.Total,
                                item.Vat,
                                item.ParentCcCode,
                                item.ParentCcName,
                                item.CusTypeId,

                                // الحسابات الفرعية
                                Paid = context.PmExtractTransactionsPayments
                                             .Where(p => p.TransactionId == item.Id && p.IsDeleted == false)
                                             .Sum(p => (decimal?)p.AmountReceived) ?? 0,

                                Discount = context.PmExtractTransactionsDiscounts
                                                  .Where(d => d.TransactionId == item.Id && d.IsDeleted == false)
                                                  .Sum(d => (decimal?)d.DiscountAmount) ?? 0
                            };

                if (!string.IsNullOrEmpty(code))
                {
                    query = query.Where(q => q.Code == code);
                }

                if (!string.IsNullOrEmpty(dateText) && DateTime.TryParse(dateText, out var parsedDate))
                {
                    var formattedDate = parsedDate.ToString("yyyy-MM-dd");
                    query = query.Where(q => q.Date1 == formattedDate);
                }

                var list = await query
                    .Where(q => q.Paid < q.Total)
                    .OrderBy(q => q.Code)
                    .ToListAsync();

                var resultList = list
                    .Select((item, index) => new TransactionUnPaidResult
                    {
                        RowNumber = index + 1,
                        Code = item.Code,
                        Date1 = item.Date1,
                        CustomerCode = item.CustomerCode,
                        CustomerName = item.CustomerName,
                        ProjectName = item.ProjectName,
                        Total = item.Total ?? 0,
                        Paid = item.Paid,
                        Subtotal = ((item.Total ?? 0) + (item.Vat ?? 0) - item.Discount - item.Paid),
                        ParentCCCode = item.ParentCcCode,
                        ParentCCName = item.ParentCcName,
                        CusTypeId = item.CusTypeId
                    })
                    .ToList();

                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<TransactionUnPaidResult>();
            }
        }


        public async Task<List<PayrollResultpopup>> GetApprovedPayroll(PayrollResultPopupFilter filter)
        {
            try
            {
                var query = from item in context.HrPayrollDVw
                            where item.IsDeleted == false
                                  && item.State == 4
                                  && item.FacilityId == currentData.FacilityId
                                  && (item.BranchId == 0 || currentData.Branches.Contains(item.BranchId.ToString()))
                            select item;

                if (filter.MsId.HasValue)
                    query = query.Where(q => q.MsId == filter.MsId.Value);


                if (!string.IsNullOrEmpty(filter.MsMonth))
                    query = query.Where(q => q.MsMonth == filter.MsMonth);
                if (filter.FinancialYear.HasValue)
                    query = query.Where(q => q.FinancelYear == filter.FinancialYear);


                if (filter.PayrollTypeId.HasValue)
                    query = query.Where(q => q.PayrollTypeId == filter.PayrollTypeId.Value);

                //if (!string.IsNullOrEmpty(filter.ApplicationCode))
                //    query = query.Where(q => q.ApplicationCode == filter.ApplicationCode);

                query = query.Where(q => !context.AccRequestVws
                    .Any(r =>
                        r.IsDeleted == false &&
                        r.StatusId != 4 &&
                        r.RefranceNo == q.MsCode.ToString() &&
                        r.ReferenceNo == q.EmpId));

                query = query.Where(q => !context.AccRequestEmployeeVw
                    .Any(r =>
                        r.IsDeleted == false
                        &&
                        r.StatusId != 4 &&
                        r.RefranceNo == q.MsCode.ToString() &&
                        r.ReferenceNo == q.EmpId

                        ));

                var list = await query.OrderBy(q => q.MsId).ToListAsync();

                var resultList = list
                    .Select((item, index) => new PayrollResultpopup
                    {
                        RowNumber = index + 1,
                        MsId = item.MsId,
                        MsCode = item.MsCode,
                        MsTitle = item.MsTitle,
                        MsDate = item.MsDate,
                        FinancelYear = item.FinancelYear,
                        MonthName = item.MonthName,
                        EmpCode = item.EmpCode,
                        EmpName = item.EmpName,
                        Amount =
                            (item.Salary ?? 0) +
                            (item.Allowance ?? 0) +
                            (item.OverTime ?? 0) +
                            (item.Commission ?? 0) -
                            (item.Absence ?? 0) -
                            (item.Delay ?? 0) -
                            (item.Penalties ?? 0) -
                            (item.Loan ?? 0) -
                            (item.Deduction ?? 0) +
                            (item.Mandate ?? 0),

                    })
                    .ToList();

                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<PayrollResultpopup>();
            }
        }


    }

}
