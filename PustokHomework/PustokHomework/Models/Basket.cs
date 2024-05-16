namespace PustokHomework.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int BookId { get; set;}
        public int Count {  get; set;}

        public Book? Book { get; set;}
        public AppUser AppUser { get; set;}
    }
}
