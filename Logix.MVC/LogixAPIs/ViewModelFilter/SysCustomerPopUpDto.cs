

namespace Logix.MVC.LogixAPIs.ViewModelFilter
{
    public class SysCustomerPopUpDto
    {
        // used in PopupApiController/GetCustomersPopUp & PopupApiController/CustomerCodeChanged
        public long? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }

        public string? Email { get; set; }
        public string? Mobile { get; set; }
    }
}
