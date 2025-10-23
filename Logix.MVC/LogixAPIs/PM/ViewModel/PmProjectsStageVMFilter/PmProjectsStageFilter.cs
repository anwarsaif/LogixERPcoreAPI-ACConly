using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.PM.ViewModel.PmProjectsStageVMFilter
{
    public class PmProjectsStageFilter
    {

        public long Id { get; set; } = 0;
        [StringLength(250)]
        public string? Name { get; set; } = "";
    } 
    public class PmProjectsSubStageFilter
    {

        public long Id { get; set; }
        public long? ParentId { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
    }
}
