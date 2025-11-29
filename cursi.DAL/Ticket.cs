namespace cursi.DAL
{
    public class Ticket
    {
        public int Id { get; set; }
        public int SeatId { get; set; }
        public string BuyerName { get; set; }
        public decimal Price { get; set; }
    }
}
