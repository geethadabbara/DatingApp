namespace DatingApp.API.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPageIndex { get; set; }
        public int TotalCount { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }

        public PaginationHeader(int currentPage, int itemsPerPage, int totalCount, int totalPages)
        {
            this.CurrentPageIndex = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalPages = totalPages;
            this.TotalCount = totalCount;
        }
    }
}