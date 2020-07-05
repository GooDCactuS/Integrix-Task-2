namespace Wali_Express.Core.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ClientFullName { get; set; }
        public int Amount { get; set; }
    }
}