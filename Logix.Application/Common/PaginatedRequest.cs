namespace Logix.Application.Common
{

    public interface IPaginatedRequest<T>
    {
        T Filter { get; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
    }

    public class PaginatedRequest<T> : IPaginatedRequest<T>
    {
        public T Filter { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public long LastSeenId { get; set; } = 0;  // Use long for LastSeenId
    }


}

