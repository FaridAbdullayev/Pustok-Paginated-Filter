using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokHomework.Data;
using PustokHomework.Models;
using PustokHomework.ViewModel;
using System.Net;
using System.Security.Claims;

namespace PustokHomework.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        //public async Task<IActionResult> Index()
        //{
        //    BasketViewModel viewModel = new BasketViewModel();
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        var basketItems = _context.Baskets
        //            .Include(x => x.Book)
        //            .ThenInclude(x => x.Images.Where(x => x.PosterStatus == true))
        //            .Where(c => c.AppUserId == userId)
        //            .ToList();


        //        viewModel.Items = basketItems.Where(x => x != null && x.Book != null).Select(x => new BasketDetailViewModel
        //        {
        //            Id = x.BookId,
        //            Name = x.Book.Name,
        //            Price = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
        //            Image = x.Book.Images.FirstOrDefault(x => x.PosterStatus == true)?.Name,
        //            Count = x.Count,
        //            Total = x.Book.SalePrice * x.Count,
        //            StockStatus = x.Book.StockStatus
        //        }).ToList();

        //    }
        //    else
        //    {
        //        List<BasketCookiesViewModel> basketProduct = new();
        //        if (HttpContext.Request.Cookies["Product"] != null)
        //        {
        //            basketProduct = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(HttpContext.Request.Cookies["Product"]);
        //        }
        //        else
        //        {
        //            basketProduct = new List<BasketCookiesViewModel>();
        //        }

        //        foreach (var item in basketProduct)
        //        {
        //            var dbProduct = await _context.Books.Include(x => x.Images.Where(bi => bi.PosterStatus == true)).FirstOrDefaultAsync(v => v.Id == item.ProductId && !v.IsDeleted);

        //            if (dbProduct != null)
        //            {
        //                BasketDetailViewModel basketDetailViewModel = new()
        //                {
        //                    Id = dbProduct.Id,
        //                    Name = dbProduct.Name,
        //                    StockStatus = dbProduct.StockStatus,
        //                    Price = dbProduct.SalePrice,
        //                    Image = dbProduct.Images.Where(x => x.PosterStatus == true).FirstOrDefault().Name,
        //                    Count = item.Count,
        //                    Total = dbProduct.SalePrice * item.Count,
        //                };
        //                viewModel.Items.Add(basketDetailViewModel);
        //            }
        //        }
        //    }
        //    return View(viewModel);
        //}

        //public IActionResult DeleteBasket(int bookId)
        //{
        //    if (User.Identity.IsAuthenticated && User.IsInRole("member"))
        //    {
        //        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        var basketItemToDelete = _context.Baskets.FirstOrDefault(b => b.AppUserId == userId && b.BookId == bookId);
        //        if (basketItemToDelete != null)
        //        {
        //            _context.Baskets.Remove(basketItemToDelete);
        //            _context.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        List<BasketCookiesViewModel> basketProduct = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(HttpContext.Request.Cookies["Product"]);

        //        var deleteProduct = basketProduct.FirstOrDefault(x => x.ProductId == bookId);

        //        basketProduct.Remove(deleteProduct);

        //        HttpContext.Response.Cookies.Append("Product", JsonConvert.SerializeObject(basketProduct));
        //    }
        //    return View("index");
        //}

    }
}
