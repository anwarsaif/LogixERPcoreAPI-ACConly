using Logix.Application.DTOs.ACC;

namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class AccFacilityEditVM
    {
        public AccFacilityEditDto AccFacilityEditDto { get; set; }
        //public AccFacilityEditFileDto AccFacilityEditFileDto { get; set; }
        public AccFacilityEditVM()
        {
            AccFacilityEditDto = new AccFacilityEditDto();
            //  AccFacilityEditFileDto = new AccFacilityEditFileDto();
        }
    }
}
