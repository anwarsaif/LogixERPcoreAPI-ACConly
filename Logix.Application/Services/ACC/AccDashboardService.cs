using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.RPT;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IRepositories.Main;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.GB;
using Logix.Domain.Main;
using Logix.Domain.RPT;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Services.ACC
{
    public class AccDashboardService: IAccDashboardService
    {
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IRptRepositoryManager rptRepositoryManager;
        private readonly IMapper mapper;
        private readonly ICurrentData session;

        public AccDashboardService(IAccRepositoryManager accRepositoryManager,  IMainRepositoryManager mainRepositoryManager
            , IRptRepositoryManager rptRepositoryManager, IMapper mapper, ICurrentData session)
        {
            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this.rptRepositoryManager = rptRepositoryManager;
            this.mapper = mapper;
            this.session = session;

        }    
        public async Task<IResult<IEnumerable<AccStatisticsDto>>> GetStatistics(long facilityId, long finYearId)
        {
            if (facilityId == 0 || finYearId == 0)
                return await Result<IEnumerable<AccStatisticsDto>>.FailAsync("خطأ, يرجى إدخال البيانات المطلوبة");

            try
            {
                string startDate;
                string endDate;

                int finYearGregorian = session.FinyearGregorian;
                int yearHijri = int.Parse(Bahsas.YearHijri(session));

                if (finYearGregorian < yearHijri)
                {
                    startDate = $"{finYearGregorian}/01/01";
                    endDate = $"{finYearGregorian}/12/31";
                }
                else
                {
                    startDate = $"{yearHijri}/01/01";
                    endDate = Bahsas.HDateNow3(session);
                }

                var statisticsList = new List<AccStatisticsDto>();

                try
                {
                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "green",
                        Count = accRepositoryManager.AccJournalMasterRepository.GetCount(j =>
                            j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId),
                        Icon = "icon-bar-chart",
                        StatusId = 4,
                        StatusName = "إجمالي عدد القيود",
                        StatusName2 = "Entries",
                        Route = "/Apps/Accounting/Reports/GL?Status_ID=4"
                    });

                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "red",
                        Count = accRepositoryManager.AccJournalMasterRepository.GetCount(j =>
                            j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId && j.StatusId == 1),
                        Icon = "icon-bar-chart",
                        StatusId = 2,
                        StatusName = "القيود غير المرحلة",
                        StatusName2 = "Entries Pending",
                        Route = "/Apps/Accounting/Reports/GL?Status_ID=2"
                    });

                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "blue",
                        Count = accRepositoryManager.AccJournalMasterRepository.GetCount(j =>
                            j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId && j.StatusId == 3),
                        Icon = "icon-bar-chart",
                        StatusId = 6,
                        StatusName = "القيود تحت المراجعة",
                        StatusName2 = "Entries under review",
                        Route = "/Apps/Accounting/GL/Journal_Review"
                    });

                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "yellow",
                        Count = accRepositoryManager.AccJournalMasterRepository.GetCount(j =>
                            j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId && j.StatusId == 0),
                        Icon = "icon-bar-chart",
                        StatusId = 5,
                        StatusName = "القيود المحفوظة مؤقتاً",
                        StatusName2 = "Entries Temporarily saved",
                        Route = "/Apps/Accounting/Reports/GL?Status_ID=5"
                    });

                   
                    string Posting_By_User_Doc_Type = "1";
                    var property = await mainRepositoryManager.SysPropertyValueRepository.GetByProperty(20, session.FacilityId);

                    if (property != null && !string.IsNullOrEmpty(property.PropertyValue))
                    {
                        Posting_By_User_Doc_Type = property.PropertyValue;
                    }

                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "red",
                        Count = accRepositoryManager.AccJournalMasterRepository.GetCount(j =>
                            j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId && j.StatusId == 1
                            &&j.ApprovelBy1 ==null && j.InsertUserId!=session.UserId
                            && (Posting_By_User_Doc_Type.Equals("1") || Posting_By_User_Doc_Type.Contains(j.DocTypeId.ToString()))
                            ),
                        Icon = "icon-bar-chart",
                        StatusId = 7,
                        StatusName = "قيود تحت التدقيق",
                        StatusName2 = "Review Journal Entries",
                        Route = "/Apps/Accounting/GL/Review_to_general_ledger"
                    });
                }
                catch (Exception)
                {
                    // إذا حصل خطأ في قراءة القيود، نضيف القيم صفرية
                    statisticsList.AddRange(new[]
                    {
                new AccStatisticsDto
                {
                    Color = "green",
                    Count = 0,
                    Icon = "icon-bar-chart",
                    StatusId = 4,
                    StatusName = "إجمالي عدد القيود",
                    StatusName2 = "Entries",
                     Route = "/Apps/Accounting/Reports/GL?Status_ID=4"
                },
                new AccStatisticsDto
                {
                    Color = "red",
                    Count = 0,
                    Icon = "icon-bar-chart",
                    StatusId = 2,
                    StatusName = "القيود غير المرحلة",
                    StatusName2 = "Entries Pending",
                     Route = "/Apps/Accounting/Reports/GL?Status_ID=2"
                },
                new AccStatisticsDto
                {
                    Color = "blue",
                    Count = 0,
                    Icon = "icon-bar-chart",
                    StatusId = 6,
                    StatusName = "القيود تحت المراجعة",
                    StatusName2 = "Entries under review",
                     Route = "/Apps/Accounting/GL/Journal_Review"
                },
                new AccStatisticsDto
                {
                    Color = "yellow",
                    Count = 0,
                    Icon = "icon-bar-chart",
                    StatusId = 5,
                    StatusName = "القيود المحفوظة مؤقتاً",
                    StatusName2 = "Entries Temporarily saved",
                     Route = "/Apps/Accounting/Reports/GL?Status_ID=5"
                },
                  new AccStatisticsDto
                    {
                        Color = "red",
                        Count =0,
                        Icon = "icon-bar-chart",
                        StatusId = 7,
                        StatusName = "قيود تحت التدقيق",
                        StatusName2 = "Review Journal Entries",
                         Route = "/Apps/Accounting/GL/Review_to_general_ledger"
                    }
            });
                }

                try
                {
                    var scheduledCount = await accRepositoryManager.AccsettlementscheduleRepository
                        .GetCountSettlement(facilityId, startDate, endDate);

                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "yellow",
                        Count = scheduledCount,
                        Icon = "icon-bar-chart",
                        StatusId = 10,
                        StatusName = "القيود المجدولة",
                        StatusName2 = "Settlement Journal",
                        Route = "/Apps/Accounting/Settlement_Schedule/Settlement_Journal.aspx"
                    });
                }
                catch (Exception)
                {
                    statisticsList.Add(new AccStatisticsDto
                    {
                        Color = "yellow",
                        Count = 0,
                        Icon = "icon-warning",
                        StatusId = 10,
                        StatusName = " القيود المجدولة",
                        StatusName2 = "Error in Settlement Journal",
                        Route = "/Apps/Accounting/Settlement_Schedule/Settlement_Journal.aspx"
                    });
                }

                return await Result<IEnumerable<AccStatisticsDto>>.SuccessAsync(statisticsList);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AccStatisticsDto>>.FailAsync($"حدث خطأ أثناء معالجة البيانات: {ex.Message}");
            }
        }


        public async Task<IResult<IEnumerable<RptReportDto>>> GetReports(long systemId, string groupId)
        {
            if (systemId == 0 || string.IsNullOrEmpty(groupId)) return await Result<IEnumerable<RptReportDto>>.FailAsync("خطأ, يرجى ادخال البيانات المطلوبة");
            try
            {
                IEnumerable<RptReport> reportsList = new List<RptReport>();
                var reports = await rptRepositoryManager.RptReportRepository.GetAll(d=> d.SystemId == systemId && d.IsDeleted == false);
                if(reports != null)
                {
                    reportsList = reports.ToList().Where(r => r.SysGroupId != null && r.SysGroupId.Split(",").Contains(groupId)).ToList();
                    
                }
                return await Result<IEnumerable<RptReportDto>>.SuccessAsync(mapper.Map<List<RptReportDto>>(reportsList), "");

            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<RptReportDto>>.FailAsync($"Exp in acc GetStatistics: {ex.Message}");
            }
        }

        public int CheckPage(long screenId)
        {
            try
            {
                if (screenId == 70)
                {
                    long facilityId = session.FacilityId;
                    long finYearId = session.FinYear;
                    var count = accRepositoryManager.AccJournalMasterRepository.GetCount(j => j.FlagDelete == false && j.FacilityId == facilityId && j.FinYear == finYearId && j.StatusId == 1);
                    return count;
                }

                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<IResult<IEnumerable<BudgTransactionVw>>> GetAllBudgTransactionVw(CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await rptRepositoryManager.RptReportRepository.GetAllBudgTransactionVw();
                return await Result<IEnumerable<BudgTransactionVw>>.SuccessAsync(res);
            }
            catch (Exception exp)
            {
                return await Result<IEnumerable<BudgTransactionVw>>.FailAsync(exp.Message);
            }
        }
    }
}
