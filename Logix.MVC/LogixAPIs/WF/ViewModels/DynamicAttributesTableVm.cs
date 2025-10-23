using Logix.Domain.WF;

namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class DynamicAttributesTableVm
    {
        public string TableName { get; set; } = "";
        public List<WfDynamicAttributesTableVw> TableAttributes { get; set; }

        public DynamicAttributesTableVm()
        {
            TableAttributes = new List<WfDynamicAttributesTableVw>();
        }
    }
}
