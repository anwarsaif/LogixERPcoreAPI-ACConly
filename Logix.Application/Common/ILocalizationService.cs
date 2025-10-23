using Microsoft.AspNetCore.Builder;
using System.Globalization;

namespace Logix.Application.Common
{
    public interface ILocalizationService
    {
        IList<CultureInfo> GetSupportedCultures();
        void ConfigureLocalization(IApplicationBuilder app);
        string GetLocalizedResource(string key, string resource, CultureInfo culture = default);
        string GetMessagesResource(string key, CultureInfo culture = default);
        string GetMainResource(string key, CultureInfo culture = default);
        string GetAccResource(string key, CultureInfo culture = default);
        string GetCommonResource(string key, CultureInfo culture = default);
        string GetHrResource(string key, CultureInfo culture = default);
        string GetPMResource(string key, CultureInfo culture = default);
        string GetSALResource(string key, CultureInfo culture = default);
        string GetResource1(string key, CultureInfo culture = default);
        string GetCoreResource(string key, CultureInfo culture = default);

        string GetInventoryResource(string key, CultureInfo culture = default);
        string GetPUResource(string key, CultureInfo culture = default);
        string GetSSResources(string key, CultureInfo culture = default);
    }
}
