using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokHomework.Data;
using PustokHomework.Models;
using PustokHomework.ViewModel;
using System.Security.Claims;

namespace PustokHomework.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            CheckoutViewModel vm = new CheckoutViewModel
            {
                BasketViewModel = GetBasket()
            };

            return View(vm);
        }

        [Authorize(Roles = "member")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Checkout(OrderCreateViewModel orderVM)
        {
            if (!ModelState.IsValid)
            {
                CheckoutViewModel vm = new CheckoutViewModel
                {
                    BasketViewModel = GetBasket(),
                    Order = orderVM
                };
                return View(vm);
            }

            AppUser user = await _userManager.GetUserAsync(User);

            if (user == null) return RedirectToAction("login", "account");

            Order order = new Order
            {
                Address = orderVM.Address,
                Phone = orderVM.Phone,
                CreatedAt = DateTime.Now,
                AppUserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Note = orderVM.Note,
                Status = Models.Enums.OrderStatus.Pending
            };

            var basketItems = _context.Baskets.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToList();

            order.OrderItems = basketItems.Select(x => new OrderItem
            {
                BookId = x.BookId,
                Count = x.Count,
                SalePrice = x.Book.SalePrice,
                DiscountPercent = x.Book.DiscountPercent,
                CostPrice = x.Book.CostPrice,
            }).ToList();

            _context.Orders.Add(order);
            _context.Baskets.RemoveRange(basketItems);

            _context.SaveChanges();

            return RedirectToAction("profile", "account", new { tab = "orders" });
        }
        public BasketViewModel GetBasket()
        {
            BasketViewModel viewModel = new BasketViewModel();
            if (User.Identity.IsAuthenticated && User.IsInRole("member"))
            {
                string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                if (HttpContext.Request.Cookies["Product"] != null)
                {
                    basketProduct = JsonConvert.DeserializeObject<List<BasketCookiesViewModel>>(HttpContext.Request.Cookies["Product"]);
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
                            Price = dbProduct.SalePrice,
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

        [Authorize(Roles = "member")]
        public IActionResult GetOrderItems(int orderId)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;

            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(oi => oi.Book).FirstOrDefault(x => x.Id == orderId && x.AppUserId == user.Id);

            if (order == null) return RedirectToAction("notfound", "error");

            return PartialView("_OrderDetailPartial", order.OrderItems);
        }

        [Authorize(Roles = "member")]
        public IActionResult Cancel(int id)
        {
            AppUser user = _userManager.GetUserAsync(User).Result;

            Order order = _context.Orders.FirstOrDefault(x => x.Id == id && x.AppUserId == user.Id && x.Status == Models.Enums.OrderStatus.Pending);

            if (order == null) return RedirectToAction("notfound", "error");

            order.Status = Models.Enums.OrderStatus.Canceled;
            _context.SaveChanges();
            return RedirectToAction("myAccount", "account", new { tab = "orders" });
        }
    }
}
