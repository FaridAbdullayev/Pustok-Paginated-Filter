using PustokHomework.Models;

namespace PustokHomework.ViewModel
{
    public class ShopViewModel
    {
        public PaginatedShop<Book> PaginatedBooks { get; set; }
        public List<Author> Authors { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
