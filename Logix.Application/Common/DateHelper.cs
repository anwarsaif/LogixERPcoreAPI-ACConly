using System.Globalization;
using Logix.Application.Interfaces.IRepositories;
namespace Logix.Application.Common
{
    public static class DateHelper
    {
        public static IMainRepositoryManager mainRepositoryManager { get; set; }
        public static CultureInfo arCul { get; set; } = new CultureInfo("en-US");
        public static CultureInfo enCul { get; set; } = new CultureInfo("ar-SA");

        public static void Initialize(IMainRepositoryManager _mainRepositoryManager)
        {
            mainRepositoryManager = _mainRepositoryManager;


        }

        // Created By Mohammed ALshrik in 2025_07_14
        public static DateTime? SafeParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            string[] formats = { "yyyy/MM/dd", "yyyy-MM-dd", "dd/MM/yyyy" };

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        public static string DateToString(DateTime date)
        {
            return date.ToString("yyyy/MM/dd");
        }

        public static string DateToString(DateTime date, CultureInfo culture)
        {
            return date.ToString("yyyy/MM/dd", culture);
        }

        public static string DateDashToString(DateTime date, CultureInfo culture)
        {
            return date.ToString("yyyy-MM-dd", culture);
        }


        public static async Task<string> DateFormattYYYYMMDD_H_G(string DateH)
        {
            try
            {
                var getDate = await mainRepositoryManager.SysCalendarRepository.GetOne(x => x.GDate, x => x.HDate == DateH);
                return getDate ?? "";
            }
            catch (Exception)
            {
                return "Error In DateFormattYYYYMMDD_H_G Function.";
            }
        }

        public static string DateFormattYYYYMMDD_H_G2(string dateH)
        {
            string year = dateH.Substring(0, 4);
            string month = dateH.Substring(5, 2);
            string day = dateH.Substring(8, 2);

            if ((day == "30" && month == "06") || (day == "30" && month == "02") || (day == "30" && month == "04") ||
                (day == "30" && month == "08") || (day == "30" && month == "10") || (day == "31" && month == "10") ||
                (day == "30" && month == "12") || (day == "31" && month == "12") || (day == "30" && month == "11"))
            {
                day = "29";
            }

            string newDateHi = $"{year}/{day}/{month}";

            DateTime newDate = DateTime.ParseExact(newDateHi, "yyyy/dd/MM", CultureInfo.InvariantCulture);

            return $"{newDate.Year}/{newDate.Month.ToString("00")}/{newDate.Day.ToString("00")}";
        }

        public static async Task<string> DateFormattYYYYMMDD_G_H(string DateG)
        {
            try
            {
                var getDate = await mainRepositoryManager.SysCalendarRepository.GetOne(x => x.HDate, x => x.GDate == DateG);
                return getDate ?? "";
            }
            catch (Exception)
            {
                return "Error In DateFormattYYYYMMDD_G_H Function.";
            }
        }

        public static async Task<string> DateGregorian(string Date)
        {
            try
            {
                var getDate = await mainRepositoryManager.SysCalendarRepository.GetOne(x => x.GDate, x => x.GDate == Date || x.HDate == Date);
                return getDate ?? "";
            }
            catch (Exception)
            {
                return "Error In DateGregorian Function.";
            }
        }

        public static string DateFormattYYYYMMDD_G_H2(string dateG)
        {
            // Splitting the date string into year, month, and day components
            string[] parts = dateG.Split('/');

            if (parts.Length != 3)
                throw new ArgumentException("Invalid date format");

            string year = parts[0];
            string month = parts[1];
            string day = parts[2];

            // Adjusting the day if necessary
            if ((day == "30" && (month == "06" || month == "02" || month == "04" || month == "08" || month == "10" || month == "12")) ||
                (day == "31" && (month == "10" || month == "12")) ||
                (day == "30" && month == "11"))
            {
                day = "29";
            }

            // Formatting and returning the date string
            return $"{year}/{month.PadLeft(2, '0')}/{day.PadLeft(2, '0')}";
        }

        /// <summary>
        /// to use it you must call this:::
        //DateHelper.Initialize(mainRepositoryManager);

        /// </summary>

        /// <returns></returns>
        public async static Task<int> DateDiff_day2(string SDate, string EDate)
        {
            try
            {
                var getData = await mainRepositoryManager.SysCalendarRepository.GetAll(x => x.GDate != null && (x.GDate.Contains(SDate.Substring(0, 4)) || x.GDate.Contains(EDate.Substring(0, 4))));
                var getDatacOUNT = getData.AsEnumerable()
                    .Where(x => x.GDate != null && DateHelper.StringToDate(x.GDate) >= DateHelper.StringToDate(SDate) && DateHelper.StringToDate(x.GDate) <= DateHelper.StringToDate(EDate)
                             );
                return getDatacOUNT.Count();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public static string CleanDate(string date)
        {
            date = date.Trim();
            char[] bidiCharacters = new char[] { '\u200E', '\u200F', '\u202A', '\u202B', '\u202C', '\u202D', '\u202E', '\u2066', '\u2067', '\u2068', '\u2069' };
            foreach (char bidiChar in bidiCharacters)
            {
                date = date.Replace(bidiChar.ToString(), "");
            }
            return date;
        }
        public static int DateDiff_day(string oldDate, string newDate)
        {

            string[] dateParts = oldDate.Split('/');
            string[] newDateParts = newDate.Split('/');

            int d1 = int.Parse(CleanDate(dateParts[2]));

            //int d1 = int.Parse(oldDate.Substring(8, 2));
            int m1 = int.Parse(CleanDate(dateParts[1]));
            int y1 = int.Parse(CleanDate(dateParts[0]));
            int d2 = int.Parse(CleanDate(newDateParts[2]));
            int m2 = int.Parse(CleanDate(newDateParts[1]));
            int y2 = int.Parse(CleanDate(newDateParts[0]));

            int years = y2 - y1;

            if (m2 < m1)
            {
                years--;
                int months = (m2 + 12) - m1;
                if (d2 < d1)
                {
                    months--;
                    int days = (d2 + 30) - d1;
                    return days + (months * 30) + (years * 360);
                }
                else
                {
                    int days = d2 - d1;
                    return days + (months * 30) + (years * 360);
                }
            }
            else
            {
                int months = m2 - m1;
                if (d2 < d1)
                {
                    months--;
                    int days = (d2 + 30) - d1;
                    return days + (months * 30) + (years * 360);
                }
                else
                {
                    int days = d2 - d1;
                    return days + (months * 30) + (years * 360);
                }
            }
        }

        public static async Task<bool> CheckDate(string curDate, long FacilityId, string CalendarType)
        {
            bool ret = false;
            try
            {
                //------------------------------------نوع التقويم المعتمد


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
        //public static async Task<bool> CheckDateH(string curDate)
        //{
        //    bool ret = false;
        //    try
        //    {
        //        int year = int.Parse(curDate.Substring(0, 4));
        //        if (year >= 1300 && year <= 1500)
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            ret = false;
        //        }

        //        int month = int.Parse(curDate.Substring(6, 2));

        //        if (month >= 1 && month <= 12)
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            ret = false;
        //        }
        //        int day = int.Parse(curDate.Substring(9, 2));
        //        if (day >= 1 && day <= 31)
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            ret = false;
        //        }

        //        String slash = curDate.Substring(5, 1);
        //        if(slash == "/")
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            ret = false;
        //        }
        //         slash = curDate.Substring(8, 1);
        //        if (slash == "/")
        //        {
        //            ret = true;
        //        }
        //        else
        //        {
        //            ret = false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ret = false;
        //        throw new Exception(" التاريخ المحدد غير صالح ", ex);
        //    }

        //    return ret;
        //}
        public static async Task<bool> CheckDateH(string curDate)
        {
            bool ret = false;
            try
            {
                int year = int.Parse(curDate.Substring(0, 4));
                int month = int.Parse(curDate.Substring(5, 2));
                int day = int.Parse(curDate.Substring(8, 2));
                char slash1 = curDate[4];
                char slash2 = curDate[7];

                if (year >= 1300 && year <= 1500 && month >= 1 && month <= 12 && day >= 1 && day <= 31 && slash1 == '/' && slash2 == '/')
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                throw new Exception("التاريخ المحدد غير صالح", ex);
            }

            return ret;
        }
        public static string? FixConvertDateFormate(string str_date)
        {
            DateTime date;
            if (
                DateTime.TryParseExact(str_date, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)

           )
            {


            }
            else
            {
                return null;
            }


            return date.ToString("yyyy/MM/dd");
        }
        public static string GetMonthName(int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {

                    throw new ArgumentException($"Invalid Month Number");
                }
                switch (month)
                {
                    case 1:
                        return "January";
                    case 2:
                        return "February";
                    case 3:
                        return "March";
                    case 4:
                        return "April";
                    case 5:
                        return "May";
                    case 6:
                        return "June";
                    case 7:
                        return "July";
                    case 8:
                        return "August";
                    case 9:
                        return "September";
                    case 10:
                        return "October";
                    case 11:
                        return "November";
                    case 12:
                        return "December";
                    default:
                        throw new ArgumentException($"Invalid Month Number");

                }
            }

            catch (Exception exp)
            {

                throw new ArgumentException($"Exception in : {exp.Message.ToString()}");
            }
        }

        public static string GetArMonthName(int month)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    throw new ArgumentException($"Invalid Month Number");
                }
                switch (month)
                {
                    case 1:
                        return "يناير";
                    case 2:
                        return "فبراير";
                    case 3:
                        return "مارس";
                    case 4:
                        return "أبريل";
                    case 5:
                        return "مايو";
                    case 6:
                        return "يونيو";
                    case 7:
                        return "يوليو";
                    case 8:
                        return "أغسطس";
                    case 9:
                        return "سبتمبر";
                    case 10:
                        return "أكتوبر";
                    case 11:
                        return "نوفمبر";
                    case 12:
                        return "ديسمبر";
                    default:
                        throw new ArgumentException($"Invalid Month Number");
                }
            }
            catch (Exception exp)
            {
                throw new ArgumentException($"Exception in : {exp.Message.ToString()}");
            }
        }
        public static string ChangeFormatDate(string xDate, string time)
        {
            try
            {
                // Extract year, month, and day from the xDate string
                string year = xDate.Substring(0, 4);
                string month = xDate.Substring(5, 2);
                string day = xDate.Substring(8, 2);

                // Concatenate the date parts with the provided time and milliseconds
                string formattedDateTime = $"{year}-{month}-{day} {time}:00.000";

                return formattedDateTime;
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or rethrow if necessary
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }



        public static string? FixConvertDateFormateToDash(string str_date)
        {
            DateTime date;
            if (
                DateTime.TryParseExact(str_date, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                || DateTime.TryParseExact(str_date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)

           )
            {


            }
            else
            {
                return null;
            }

            return date.ToString("yyyy-MM-dd");
        }
        public static DateTime StringToDate(string? dateString)
        {

            if (string.IsNullOrEmpty(dateString))
            {
                throw new ArgumentException("Date string cannot be null or empty.");
            }

            char[] separators = { '/', '-' };
            string[] dateParts = dateString.Split(separators);

            // Validate the length of the date parts
            if (dateParts.Length != 3)
            {
                throw new ArgumentException($"Invalid date string format: {dateString}");
            }

            int year, month, day;

            // Check if the year part has four digits to determine the format
            if (dateParts[0].Length == 4)
            {
                // yyyy/MM/dd format
                if (!int.TryParse(dateParts[0], out year) ||
                    !int.TryParse(dateParts[1], out month) ||
                    !int.TryParse(dateParts[2], out day))
                {
                    throw new ArgumentException($"Invalid date string format: {dateString}");
                }
            }
            else
            {
                // dd/MM/yyyy format
                if (!int.TryParse(dateParts[2], out year) ||
                    !int.TryParse(dateParts[1], out month) ||
                    !int.TryParse(dateParts[0], out day))
                {
                    throw new ArgumentException($"Invalid date string format: {dateString}");
                }
            }

            // Validate month and day ranges
            if (month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
            {
                throw new ArgumentException($"Invalid date values in date string format: {dateString}");
            }

            return new DateTime(year, month, day);
        }
        public static int CalculateMinutesDifference(string timeIn, string timeOut)
        {
            try
            {
                var timeInDate = DateTime.ParseExact(timeIn, "HH:mm", CultureInfo.InvariantCulture);
                var timeOutDate = DateTime.ParseExact(timeOut, "HH:mm", CultureInfo.InvariantCulture);

                return (int)(timeOutDate - timeInDate).TotalMinutes;
            }
            catch (Exception)
            {
                // Log the exception details
                return 0; // Return a default value in case of error
            }
        }

        public static string GetDateGregorianDotNow()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        }
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;

        }

        public static string? HijriToGreg(string? hijri)
        {
            return hijri;

        }


        public static int GetCountYears(DateTime startDate, DateTime endDate)
        {
            try
            {
                int months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;

                if (endDate.Day < startDate.Day)
                {
                    months -= 1;
                }

                int years = months / 12;
                return years;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while calculating years: {ex.Message}");
                return 0; // Or any default value you deem appropriate
            }
        }

        public static int GetCountMonths(DateTime startDate, DateTime endDate)
        {
            try
            {
                int months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;

                if (endDate.Day < startDate.Day)
                {
                    months -= 1;
                }

                months = months % 12;
                return months;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while calculating months: {ex.Message}");
                return 0; // Or any default value you deem appropriate
            }
        }

        public static int GetCountDays(DateTime startDate, DateTime endDate)
        {
            try
            {
                int days;

                if (endDate.Day < startDate.Day)
                {
                    int daysInEndMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
                    days = daysInEndMonth - startDate.Day + endDate.Day;
                }
                else
                {
                    days = endDate.Day - startDate.Day;
                }

                return days;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"An error occurred while calculating days: {ex.Message}");
                return 0; // Or any default value you deem appropriate
            }
        }

        public static int YearHijri(string CalendarType = "1")
        {
            try
            {
                string res = "";
                if (CalendarType == "2")
                    res = DateTime.Now.AddDays(-1).ToString("yyyy", enCul.DateTimeFormat);
                else
                    res = DateTime.Now.AddDays(-1).ToString("yyyy", arCul.DateTimeFormat);

                return Convert.ToInt32(res);
            }
            catch
            {
                return 0;
            }
        }


    }
}

