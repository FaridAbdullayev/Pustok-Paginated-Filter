using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokHomework.Data;
using PustokHomework.Models;
using PustokHomework.ViewModel;

namespace PustokHomework.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }


        public IActionResult Index(int? genreId = null, List<int>? authorIds = null, decimal? minPrice = null, decimal? maxPrice = null,int page =1)
        {
            ShopViewModel model = new ShopViewModel()
            {
                Genres = _context.Genres.ToList(),
                Authors = _context.Author.ToList(),
            };
            var query = _context.Books.Include(x => x.Images.Where(bi => bi.PosterStatus != null))
                                      .Include(x => x.Author).Where(x=>!x.IsDeleted).AsQueryable();

            if (genreId != null)
                query = query.Where(x => x.GenreId == genreId);
            if (authorIds != null)
                query = query.Where(x => authorIds.Contains(x.AuthorId));
            if (minPrice != null && maxPrice != null)
                query = query.Where(x => x.SalePrice >= minPrice && x.SalePrice <= maxPrice);


            ViewBag.GenreId = genreId;
            ViewBag.AuthorIds = authorIds;
            ViewBag.MinPrice = _context.Books.Where(x => !x.IsDeleted).Min(x => x.SalePrice);
            ViewBag.MaxPrice = _context.Books.Where(x => !x.IsDeleted).Max(x => x.SalePrice);
            ViewBag.SelectedMinPrice = minPrice ?? ViewBag.MinPrice;
            ViewBag.SelectedMaxPrice = maxPrice ?? ViewBag.MaxPrice;

            switch (Request.Query["sortBy"])
            {
                case "nameAZ":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "nameZA":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case "priceLowHigh":
                    query = query.OrderBy(x => x.SalePrice);
                    break;
                case "priceHighLow":
                    query = query.OrderByDescending(x => x.SalePrice);
                    break;
                default:
                    break;
            }
            model.PaginatedBooks = PaginatedShop<Book>.Create(query, page, 6);

            return View(model);
        }
    }
}
