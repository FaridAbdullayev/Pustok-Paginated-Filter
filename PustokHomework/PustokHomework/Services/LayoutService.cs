using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PustokHomework.Data;
using PustokHomework.Models;
using PustokHomework.ViewModel;
using System.Net;
using System.Security.Claims;

namespace PustokHomework.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext  appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<Genre> GetGenre()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<string,string> GetSettings()
        {
            return _context.Settings.ToDictionary(x=>x.Key,x=>x.Value);
        }
        public  BasketViewModel GetBasket()
        {
            BasketViewModel viewModel = new BasketViewModel();
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.IsInRole("member"))
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var basketItems = _context.Baskets
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Images.Where(x => x.PosterStatus == true))
                    .Where(c => c.AppUserId == userId)
                    .ToList();


                viewModel.Items = basketItems.Where(x => x != null && x.Book != null).Select(x => new BasketDetailViewModel
                {
                    Id = x.BookId,
                    Name = x.Book.Name,
                    Price = x.Book.DiscountPercent > 0 ? (x.Book.SalePrice * (100 - x.Book.DiscountPercent) / 100) : x.Book.SalePrice,
                    Image = x.Book.Images.FirstOrDefault(x => x.PosterStatus == true)?.Name,
                    Count = x.Count,
                    Total = x.Book.SalePrice * x.Count,
                    StockStatus = x.Book.StockStatus
                }).ToList();
                viewModel.TotalPrice = viewModel.Items.Sum(x => x.Count * x.Price);
            }
            else
            {
                List<BasketCookiesViewModel> basketProduct = new();
                if (_httpContextAccessor.HttpContext.Request.Cookies["Product"] != null)
                {
                    basketProduct = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(_httpContextAccessor.HttpContext.Request.Cookies["Product"]);
                }
                else
                {
                    basketProduct = new List<BasketCookiesViewModel>();
                }

                foreach (var item in basketProduct)
                {
                    Book? dbProduct = _context.Books.Include(x => x.Images.Where(bi => bi.PosterStatus == true)).FirstOrDefault(v => v.Id == item.ProductId && !v.IsDeleted);

                    if (dbProduct != null)
                    {
                        BasketDetailViewModel basketDetailViewModel = new()
                        {
                            Id = dbProduct.Id,
                            Name = dbProduct.Name,
                            StockStatus = dbProduct.StockStatus,
                            Price = dbProduct.DiscountPercent > 0 ? (dbProduct.SalePrice * (100 - dbProduct.DiscountPercent) / 100) : dbProduct.SalePrice,
                            Image = dbProduct.Images.Where(x => x.PosterStatus == true).FirstOrDefault().Name,
                            Count = item.Count,
                            Total = dbProduct.SalePrice * item.Count,
                        };
                        viewModel.Items.Add(basketDetailViewModel);
                    }
                }
                viewModel.TotalPrice = viewModel.Items.Sum(x => x.Count * x.Price);
            }
            return viewModel;
        }
    }
}
