namespace PustokHomework.ViewModel
{
    public class BasketViewModel
    {
        public List<BasketDetailViewModel> Items { get; set; } = new List<BasketDetailViewModel>();
        public decimal TotalPrice { get; set; }
    }
}
