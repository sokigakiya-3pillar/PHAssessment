namespace Propeller.Models.Metadata
{
    public class PaginationMeta
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public PaginationMeta(int totalRecords, int pageSize, int currentPage)
        {
            TotalRecords = totalRecords;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

    }
}
