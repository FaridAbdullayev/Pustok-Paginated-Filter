using PustokHomework.Areas.Manage.ViewModels;

namespace PustokHomework.ViewModel
{
    public class PaginatedShop<T>
    {
        public PaginatedShop(List<T> items, int totalPages, int pageIndex, int pageSize)
        {
            this.Items = items;
            this.TotalPages = totalPages;
            this.PageIndex = pageIndex;
            PageSize = pageSize;
        }
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
        public static PaginatedShop<T> Create(IQueryable<T> query, int pageIndex, int pageSize)
        {
            int totalPages = (int)Math.Ceiling(query.Count() / 2d);
            var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedShop<T>(items, totalPages, pageIndex, pageSize);
        }
    }
}
