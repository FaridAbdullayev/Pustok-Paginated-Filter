using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using PustokHomework.Data;
using PustokHomework.Models;
using PustokHomework.ViewModel;
using System.Diagnostics;
using System.Security.Claims;

namespace PustokHomework.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context;
        public HomeController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel
            {
                Sliders = _context.Sliders.ToList(),
                Features = _context.Features.ToList(),
                FeaturedBooks = _context.Books.Include(x => x.Author).Include(x => x.Images.Where(bi => bi.PosterStatus != null)).Where(x=>!x.IsDeleted).Where(x => x.IsFeatured).Take(25).ToList(),
                NewBooks = _context.Books.Include(x => x.Author).Include(x => x.Images.Where(bi => bi.PosterStatus != null)).Where(x => !x.IsDeleted).Where(x => x.IsNew).Take(25).ToList(),
                DiscountedBooks = _context.Books.Include(x => x.Author).Include(x => x.Images.Where(bi => bi.PosterStatus != null)).Where(x => !x.IsDeleted).Where(x => x.DiscountPercent > 0).OrderByDescending(x => x.DiscountPercent).Take(25).ToList(),
            };
            return View(model);
        }
        
        public IActionResult AddBasket(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

            if (book == null) return RedirectToAction("notfound", "error");
            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var existingBasketEntity = _context.Baskets.FirstOrDefault(b => b.AppUserId == userId && b.BookId == id);

                if (existingBasketEntity != null)
                {
                    existingBasketEntity.Count++;
                }
                else
                {
                    Basket basketEntity = new Basket
                    {
                        AppUserId = userId,
                        BookId = id,
                        Count = 1
                    };
                    _context.Baskets.Add(basketEntity);
                }

                _context.SaveChanges();
                return PartialView("_BasketPartial", getBasket());
            }
            else
            {
                List<BasketCookiesViewModel> cookies = null;

                if (HttpContext.Request.Cookies["Product"] != null)
                {
                    cookies = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(HttpContext.Request.Cookies["Product"]);
                }
                else
                {
                    cookies = new List<BasketCookiesViewModel>();
                }

                var existCookies = cookies.FirstOrDefault(x => x.ProductId == id);

                if (existCookies != null)
                {
                    existCookies.Count++;
                }
                else
                {

                    BasketCookiesViewModel basket = new BasketCookiesViewModel()
                    {
                        ProductId = id,
                        Count = 1,
                    };

                    cookies.Add(basket);
                }


                HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(cookies));
                return PartialView("_BasketPartial", getBasket(cookies));

            }
        }

        private BasketViewModel getBasket(List<BasketCookiesViewModel>? items = null)
        {
            BasketViewModel vm = new BasketViewModel();

            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var basketItems = _context.Baskets
               .Include(x => x.Book).ThenInclude(b => b.Images.Where(bi => bi.PosterStatus == true))
               .Where(x => x.AppUserId == userId)
               .ToList();

                vm.Items = basketItems.Select(x => new BasketDetailViewModel
                {
                    Id = x.BookId,
                    Name = x.Book.Name,
                    Price = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
                    Image = x.Book.Images.FirstOrDefault(x => x.PosterStatus == true)?.Name,
                    Count = x.Count
                }).ToList();

                vm.TotalPrice = vm.Items.Sum(x => x.Count * x.Price);
            }
            else
            {
                if (items != null)
                {
                    foreach (var cookieItem in items)
                    {
                        Book? book = _context.Books.Include(x => x.Images.Where(bi => bi.PosterStatus == true)).FirstOrDefault(x => x.Id == cookieItem.ProductId && !x.IsDeleted);

                        if (book != null)
                        {
                            BasketDetailViewModel itemVM = new BasketDetailViewModel
                            {
                                Id = cookieItem.ProductId,
                                Count = cookieItem.Count,
                                Name = book.Name,
                                Price = book.DiscountPercent > 0 ? (book.SalePrice * (100 - book.DiscountPercent) / 100) : book.SalePrice,
                                Image = book.Images.FirstOrDefault(x => x.PosterStatus == true)?.Name
                            };
                            vm.Items.Add(itemVM);
                        }

                    }

                    vm.TotalPrice = vm.Items.Sum(x => x.Count * x.Price);
                }
            }

            return vm;
        }

    }
}
