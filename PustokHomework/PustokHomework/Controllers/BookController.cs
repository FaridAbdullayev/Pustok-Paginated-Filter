using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pustok;
using Pustok.ViewModels;
using PustokHomework.Data;
using PustokHomework.Models;

namespace PustokHomework.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly CountService countService;
        private readonly CountManageService countManageService;

        public BookController(AppDbContext appDbContext,UserManager<AppUser> userManager)
        {
            _context = appDbContext;
            _userManager = userManager;
        }
        public IActionResult GetBookById(int id)
        {
            Book book = _context.Books
                .Include(x=>x.Genre)
                .Include(x=>x.Images
                .Where(x=>x.PosterStatus == true))
                .FirstOrDefault(x=>x.Id == id);
            return PartialView("_BookModalContentPartial", book);
        }

        public IActionResult Detail(int id)
        {
            var vm = getBookDetailVM(id);

            if (vm.Book == null) return RedirectToAction("notfound", "error");

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Review(BookReview review)
        {
            AppUser? user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
                return RedirectToAction("login", "account", new { returnUrl = Url.Action("detail", "book", new { id = review.BookId }) });

            if (!_context.Books.Any(x => x.Id == review.BookId && !x.IsDeleted))
                return RedirectToAction("notfound", "error");

            if (_context.BookReviews.Any(x => x.Id == review.BookId && x.AppUserId == user.Id))
                return RedirectToAction("notfound", "error");


            if (!ModelState.IsValid)
            {
                var vm = getBookDetailVM(review.BookId);
                vm.Review = review;
                return View("detail", vm);
            }


            review.AppUserId = user.Id;
            review.CreatedAt = DateTime.Now;

            _context.BookReviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("detail", new { id = review.BookId });
        }


        private BookDetailViewModel getBookDetailVM(int bookId)
        {


            Book? book = _context.Books
              .Include(x => x.Genre)
              .Include(x => x.Author)
              .Include(x => x.Images)
              .Include(x => x.BookReviews.Take(2)).ThenInclude(r => r.AppUser)
              .Include(x => x.BookTags).ThenInclude(bt => bt.Tag)
              .FirstOrDefault(x => x.Id == bookId && !x.IsDeleted);

            BookDetailViewModel vm = new BookDetailViewModel
            {
                Book = book,
                RelatedBooks = _context.Books
                       .Include(x => x.Author)
                       .Include(x => x.Images
                               .Where(bi => bi.PosterStatus != null))
                       .Where(x => book != null && x.GenreId == book.GenreId)
                       .Take(5).ToList(),
                Review = new BookReview { BookId = bookId }
            };

            AppUser? user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return vm;
            }

            if (_userManager.IsInRoleAsync(user, "member").Result && _context.BookReviews.Any(x => x.BookId == bookId  && x.AppUserId == user.Id && x.Status != Models.Enums.ReviewStatus.Rejected))
            {
                vm.HasUserReview = true;
            }

          

            vm.TotalReviewsCount = _context.BookReviews.Count(x => x.BookId == bookId);
            if (vm.TotalReviewsCount > 0)
                vm.AvgRate = (int)Math.Ceiling(_context.BookReviews.Where(x => x.BookId == bookId).Average(x => x.Rate));
            else
                vm.TotalReviewsCount = 0;


            return vm;
        }


        //public async Task<IActionResult> LoadMore()
        //{
        //   IEnumerable<Book> book = await _context.Books
        //    .Include(x => x.Genre)
        //    .Include(x => x.Author)
        //    .Include(x => x.Images)
        //    .Include(x => x.BookReviews.Take(2)).ThenInclude(r => r.AppUser)
        //    .Include(x => x.BookTags).ThenInclude(bt => bt.Tag).ToListAsync();


        //    return Json("_BookModalPartial",book);
           
        //}
    }
}
