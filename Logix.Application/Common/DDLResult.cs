using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logix.Application.Common
{
    public class DDLResult<TValue>
    {
        public List<DDLItem<TValue>> Items { get; set; } = new();
        public TValue? LastSeenId { get; set; }
    }

    public class DDLDataPagedResult
    {
        public SelectList Items { get; set; }
        public long LastSeenId { get; set; }
    }
}
