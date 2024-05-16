using PustokHomework.Models;

namespace PustokHomework.ViewModel
{
    public class ProfileViewModel
    {
        public ProfileEditViewModel ProfileEditVM { get; set; }
        public List<Order> Orders { get; set; }
    }
}
